#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Ionic.Zip;
using Ionic.Zlib;
using NLog;
using SharpFlame.Core.Domain;
using SharpFlame.Core.Extensions;
using SharpFlame.Core.Parsers;
using SharpFlame.Core.Parsers.Ini;
using SharpFlame.Core.Parsers.Lev;
using SharpFlame.Domain;
using SharpFlame.FileIO;
using SharpFlame.Mapping;
using SharpFlame.Mapping.IO;
using SharpFlame.Mapping.IO.TTP;
using SharpFlame.Mapping.Objects;
using SharpFlame.Mapping.Script;
using SharpFlame.Mapping.Tiles;
using SharpFlame.Mapping.Tools;
using SharpFlame.Maths;
using SharpFlame.Util;

#endregion

namespace SharpFlame.Mapping.IO.Wz
{
    public class WzSaver: IIOSaver
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        protected readonly clsMap map;

        public WzSaver(clsMap newMap)
        {
            map = newMap;
        }

        public clsResult Save(string path, bool overwrite, bool compress) // compress is not implemented.
        {
            var returnResult =
                new clsResult("Compiling to \"{0}\"".Format2(path), false);
            logger.Info("Compiling to \"{0}\"".Format2(path));

            try
            {
                switch ( map.InterfaceOptions.CompileType )
                {
                    case clsInterfaceOptions.EnumCompileType.Multiplayer:
                    if ( int.Parse(map.InterfaceOptions.CompileMultiPlayers) < 2 | 
                        int.Parse(map.InterfaceOptions.CompileMultiPlayers) > Constants.PlayerCountMax )
                    {
                        returnResult.ProblemAdd(string.Format("Number of players was below 2 or above {0}.", Constants.PlayerCountMax));
                        return returnResult;
                    }
                    break;
                    case clsInterfaceOptions.EnumCompileType.Campaign:
                    break;
                    default:
                    returnResult.ProblemAdd("Unknown compile method.");
                    return returnResult;
                }

                if ( !overwrite )
                {
                    if ( File.Exists(path) )
                    {
                        returnResult.ProblemAdd("The selected file already exists.");
                        return returnResult;
                    }
                }

                if ( map.InterfaceOptions.CompileType == clsInterfaceOptions.EnumCompileType.Multiplayer )
                {
                    if ( !overwrite )
                    {
                        if ( File.Exists(path) )
                        {
                            returnResult.ProblemAdd(string.Format("A file already exists at: {0}", path));
                            return returnResult;
                        }
                    }

                    try
                    {
                        using ( var zip = new ZipOutputStream(path) )
                        {
                            // Set encoding
                            zip.AlternateEncoding = Encoding.GetEncoding("UTF-8");
                            zip.AlternateEncodingUsage = ZipOption.Always;

                            // Set compression
                            zip.CompressionLevel = CompressionLevel.BestCompression;

                            // .xplayers.lev
                            var zipPath = string.Format("{0}c-{1}.xplayers.lev", map.InterfaceOptions.CompileMultiPlayers, map.InterfaceOptions.CompileName);
                            if ( map.InterfaceOptions.CompileType == clsInterfaceOptions.EnumCompileType.Multiplayer )
                            {
                                zip.PutNextEntry(zipPath);
                                returnResult.Add(Serialize_WZ_LEV(zip));
                            }

                            var inZipPath = string.Format("multiplay/maps/{0}c-{1}", map.InterfaceOptions.CompileMultiPlayers, map.InterfaceOptions.CompileName);
                            zip.PutNextEntry(string.Format("{0}.gam", inZipPath));
                            returnResult.Add(Serialize_WZ_Gam(zip, 0U,
                                                              map.InterfaceOptions.CompileType, map.InterfaceOptions.ScrollMin, map.InterfaceOptions.ScrollMax));


                            zip.PutNextEntry(string.Format("{0}/struct.ini", inZipPath));
                            var iniStruct = new IniWriter(zip);
                            returnResult.Add(Serialize_WZ_StructuresINI(iniStruct, int.Parse(map.InterfaceOptions.CompileMultiPlayers)));
                            iniStruct.Flush();

                            zip.PutNextEntry(string.Format("{0}/droid.ini", inZipPath));
                            var iniDroid = new IniWriter(zip);
                            returnResult.Add(Serialize_WZ_DroidsINI(iniDroid, int.Parse(map.InterfaceOptions.CompileMultiPlayers)));
                            iniDroid.Flush();

                            zip.PutNextEntry(string.Format("{0}/labels.ini", inZipPath));
                            var iniLabels = new IniWriter(zip);
                            returnResult.Add(Serialize_WZ_LabelsINI(iniLabels, int.Parse(map.InterfaceOptions.CompileMultiPlayers)));
                            iniLabels.Flush();

                            zip.PutNextEntry(string.Format("{0}/feature.ini", inZipPath));
                            var iniFeature = new IniWriter(zip);
                            returnResult.Add(Serialize_WZ_FeaturesINI(iniFeature));
                            iniFeature.Flush();

                            zip.PutNextEntry(string.Format("{0}/game.map", inZipPath));
                            returnResult.Add(Serialize_WZ_Map(zip));

                            zip.PutNextEntry(string.Format("{0}/ttypes.ttp", inZipPath));
                            var ttpSaver = new TTPSaver (map);
                            returnResult.Add(ttpSaver.Save(zip));
                        }
                    }
                    catch ( Exception ex )
                    {
                        Debugger.Break();
                        returnResult.ProblemAdd(ex.Message);
                        logger.ErrorException("Got an exception", ex);
                        return returnResult;
                    }

                    return returnResult;
                }
                if ( map.InterfaceOptions.CompileType == clsInterfaceOptions.EnumCompileType.Campaign )
                {
                    var CampDirectory = PathUtil.EndWithPathSeperator(path);

                    if ( !Directory.Exists(CampDirectory) )
                    {
                        returnResult.ProblemAdd(string.Format("Directory {0} does not exist.", CampDirectory));
                        return returnResult;
                    }

                    var filePath = string.Format("{0}{1}.gam", CampDirectory, map.InterfaceOptions.CompileName);
                    using ( var file = File.Open(filePath, FileMode.Open | FileMode.CreateNew) )
                    {
                        returnResult.Add(Serialize_WZ_Gam(file, (UInt32)map.InterfaceOptions.CampaignGameType,
                                                          map.InterfaceOptions.CompileType, map.InterfaceOptions.ScrollMin, map.InterfaceOptions.ScrollMax));
                    }

                    CampDirectory += map.InterfaceOptions.CompileName + Convert.ToString(App.PlatformPathSeparator);
                    try
                    {
                        Directory.CreateDirectory(CampDirectory);
                    }
                    catch ( Exception ex )
                    {
                        returnResult.ProblemAdd(string.Format("Unable to create directory {0}", CampDirectory));
                        logger.ErrorException("Got an exception", ex);
                        return returnResult;
                    }

                    filePath = CampDirectory + "droid.ini";
                    using ( var file = File.Open(filePath, FileMode.Open | FileMode.CreateNew) )
                    {
                        var iniDroid = new IniWriter(file);
                        returnResult.Add(Serialize_WZ_DroidsINI(iniDroid, -1));
                        iniDroid.Flush();
                    }

                    filePath = CampDirectory + "feature.ini";
                    using ( var file = File.Open(filePath, FileMode.Open | FileMode.CreateNew) )
                    {
                        var iniFeatures = new IniWriter(file);
                        returnResult.Add(Serialize_WZ_FeaturesINI(iniFeatures));
                        iniFeatures.Flush();
                    }

                    filePath = CampDirectory + "game.map";
                    using ( var file = File.Open(filePath, FileMode.Open | FileMode.CreateNew) )
                    {
                        returnResult.Add(Serialize_WZ_Map(file));
                    }

                    filePath = CampDirectory + "struct.ini";
                    using ( var file = File.Open(filePath, FileMode.Open | FileMode.CreateNew) )
                    {
                        var iniStruct = new IniWriter(file);
                        returnResult.Add(Serialize_WZ_StructuresINI(iniStruct, -1));
                        iniStruct.Flush();
                    }

                    filePath = CampDirectory + "ttypes.ttp";
                    var ttpSaver = new TTPSaver(map);
                    returnResult.Add(ttpSaver.Save(filePath, false));

                    filePath = CampDirectory + "labels.ini";
                    using ( var file = File.Open(filePath, FileMode.Open | FileMode.CreateNew) )
                    {
                        var iniLabels = new IniWriter(file);
                        returnResult.Add(Serialize_WZ_LabelsINI(iniLabels, 0));
                        iniLabels.Flush();
                    }
                }
            }
            catch ( Exception ex )
            {
                Debugger.Break();
                returnResult.ProblemAdd(ex.Message);
                logger.ErrorException("Got an exception", ex);
                return returnResult;
            }

            return returnResult;
        }

        private clsResult Serialize_WZ_StructuresINI(IniWriter File, int PlayerCount)
        {
            var ReturnResult = new clsResult("Serializing structures INI", false);
            logger.Info("Serializing structures INI");

            var structureTypeBase = default(StructureTypeBase);
            var unitModuleCount = new int[map.Units.Count];
            var sectorNum = new XYInt();
            var otherStructureTypeBase = default(StructureTypeBase);
            var otherUnit = default(clsUnit);
            var moduleMin = new XYInt();
            var moduleMax = new XYInt();
            var footprint = new XYInt();
            var A = 0;
            var underneathTypes = new StructureTypeBase.enumStructureType[2];
            var underneathTypeCount = 0;
            var badModuleCount = 0;
            var priorityOrder = new clsObjectPriorityOrderList();

            foreach ( var unit in map.Units.Where(d => d.TypeBase.Type == UnitType.PlayerStructure) )
            {
                structureTypeBase = (StructureTypeBase)unit.TypeBase;
                switch ( structureTypeBase.StructureType )
                {
                    case StructureTypeBase.enumStructureType.FactoryModule:
                    underneathTypes[0] = StructureTypeBase.enumStructureType.Factory;
                    underneathTypes[1] = StructureTypeBase.enumStructureType.VTOLFactory;
                    underneathTypeCount = 2;
                    break;
                    case StructureTypeBase.enumStructureType.PowerModule:
                    underneathTypes[0] = StructureTypeBase.enumStructureType.PowerGenerator;
                    underneathTypeCount = 1;
                    break;
                    case StructureTypeBase.enumStructureType.ResearchModule:
                    underneathTypes[0] = StructureTypeBase.enumStructureType.Research;
                    underneathTypeCount = 1;
                    break;
                    default:
                    underneathTypeCount = 0;
                    break;
                }

                if ( underneathTypeCount == 0 )
                {
                    priorityOrder.SetItem(unit);
                    priorityOrder.ActionPerform();
                }
                else
                {
                    // IS module.
                    sectorNum = map.GetPosSectorNum(unit.Pos.Horizontal);
                    clsUnit underneath = null;
                    var connection = default(clsUnitSectorConnection);
                    foreach ( var tempLoopVar_Connection in map.Sectors[sectorNum.X, sectorNum.Y].Units )
                    {
                        connection = tempLoopVar_Connection;
                        otherUnit = connection.Unit;
                        if ( otherUnit.TypeBase.Type == UnitType.PlayerStructure )
                        {
                            otherStructureTypeBase = (StructureTypeBase)otherUnit.TypeBase;
                            if ( otherUnit.UnitGroup == unit.UnitGroup )
                            {
                                for ( A = 0; A <= underneathTypeCount - 1; A++ )
                                {
                                    if ( otherStructureTypeBase.StructureType == underneathTypes[A] )
                                    {
                                        break;
                                    }
                                }
                                if ( A < underneathTypeCount )
                                {
                                    footprint = otherStructureTypeBase.get_GetFootprintSelected(otherUnit.Rotation);
                                    moduleMin.X = otherUnit.Pos.Horizontal.X - (int)(footprint.X * Constants.TerrainGridSpacing / 2.0D);
                                    moduleMin.Y = otherUnit.Pos.Horizontal.Y - (int)(footprint.Y * Constants.TerrainGridSpacing / 2.0D);
                                    moduleMax.X = otherUnit.Pos.Horizontal.X + (int)(footprint.X * Constants.TerrainGridSpacing / 2.0D);
                                    moduleMax.Y = otherUnit.Pos.Horizontal.Y + (int)(footprint.Y * Constants.TerrainGridSpacing / 2.0D);
                                    if ( unit.Pos.Horizontal.X >= moduleMin.X & unit.Pos.Horizontal.X < moduleMax.X &
                                        unit.Pos.Horizontal.Y >= moduleMin.Y & unit.Pos.Horizontal.Y < moduleMax.Y )
                                    {
                                        unitModuleCount[otherUnit.MapLink.ArrayPosition]++;
                                        underneath = otherUnit;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    if ( underneath == null )
                    {
                        badModuleCount++;
                    }
                }
            }

            if ( badModuleCount > 0 )
            {
                ReturnResult.WarningAdd(string.Format("{0} modules had no underlying structure.", badModuleCount));
            }

            var tooManyModulesWarningCount = 0;
            var tooManyModulesWarningMaxCount = 16;
            var moduleCount = 0;
            var moduleLimit = 0;

            for ( A = 0; A <= priorityOrder.Result.Count - 1; A++ )
            {
                var unit = priorityOrder.Result[A];
                structureTypeBase = (StructureTypeBase)unit.TypeBase;
                if ( unit.ID <= 0 )
                {
                    ReturnResult.WarningAdd("Error. A structure\'s ID was zero. It was NOT saved. Delete and replace it to allow save.");
                }
                else
                {
                    File.AddSection("structure_" + unit.ID.ToStringInvariant());
                    File.AddProperty("id", unit.ID.ToStringInvariant());
                    if ( unit.UnitGroup == map.ScavengerUnitGroup || (PlayerCount >= 0 & unit.UnitGroup.WZ_StartPos >= PlayerCount) )
                    {
                        File.AddProperty("player", "scavenger");
                    }
                    else
                    {
                        File.AddProperty("startpos", unit.UnitGroup.WZ_StartPos.ToStringInvariant());
                    }
                    File.AddProperty("name", structureTypeBase.Code);
                    if ( structureTypeBase.WallLink.IsConnected )
                    {
                        File.AddProperty("wall/type", structureTypeBase.WallLink.ArrayPosition.ToStringInvariant());
                    }
                    File.AddProperty("position", unit.GetINIPosition());
                    File.AddProperty("rotation", unit.GetINIRotation());
                    if ( unit.Health < 1.0D )
                    {
                        File.AddProperty("health", unit.GetINIHealthPercent());
                    }
                    switch ( structureTypeBase.StructureType )
                    {
                        case StructureTypeBase.enumStructureType.Factory:
                        moduleLimit = 2;
                        break;
                        case StructureTypeBase.enumStructureType.VTOLFactory:
                        moduleLimit = 2;
                        break;
                        case StructureTypeBase.enumStructureType.PowerGenerator:
                        moduleLimit = 1;
                        break;
                        case StructureTypeBase.enumStructureType.Research:
                        moduleLimit = 1;
                        break;
                        default:
                        moduleLimit = 0;
                        break;
                    }
                    if ( unitModuleCount[unit.MapLink.ArrayPosition] > moduleLimit )
                    {
                        moduleCount = moduleLimit;
                        if ( tooManyModulesWarningCount < tooManyModulesWarningMaxCount )
                        {
                            ReturnResult.WarningAdd(string.Format("Structure {0} at {1} has too many modules ({2})",
                                                                  structureTypeBase.GetDisplayTextCode(),
                                                                  unit.GetPosText(),
                                                                  unitModuleCount[unit.MapLink.ArrayPosition]));
                        }
                        tooManyModulesWarningCount++;
                    }
                    else
                    {
                        moduleCount = unitModuleCount[unit.MapLink.ArrayPosition];
                    }
                    File.AddProperty("modules", moduleCount.ToStringInvariant());
                }
            }

            if ( tooManyModulesWarningCount > tooManyModulesWarningMaxCount )
            {
                ReturnResult.WarningAdd(string.Format("{0} structures had too many modules.", tooManyModulesWarningCount));
            }

            return ReturnResult;
        }

        private clsResult Serialize_WZ_DroidsINI(IniWriter ini, int playerCount)
        {
            var returnResult = new clsResult("Serializing droids INI", false);
            logger.Info("Serializing droids INI");

            var droid = default(DroidDesign);
            var template = default(DroidTemplate);
            var text = "";
            var unit = default(clsUnit);
            var asPartsNotTemplate = default(bool);
            var validDroid = false;
            var invalidPartCount = 0;
            var brain = default(Brain);

            foreach ( var tempLoopVar_Unit in map.Units )
            {
                unit = tempLoopVar_Unit;
                if ( unit.TypeBase.Type == UnitType.PlayerDroid )
                {
                    droid = (DroidDesign)unit.TypeBase;
                    validDroid = true;
                    if ( unit.ID <= 0 )
                    {
                        validDroid = false;
                        returnResult.WarningAdd("Error. A droid\'s ID was zero. It was NOT saved. Delete and replace it to allow save.");
                    }
                    if ( droid.IsTemplate )
                    {
                        template = (DroidTemplate)droid;
                        asPartsNotTemplate = unit.PreferPartsOutput;
                    }
                    else
                    {
                        template = null;
                        asPartsNotTemplate = true;
                    }
                    if ( asPartsNotTemplate )
                    {
                        if ( droid.Body == null )
                        {
                            validDroid = false;
                            invalidPartCount++;
                        }
                        else if ( droid.Propulsion == null )
                        {
                            validDroid = false;
                            invalidPartCount++;
                        }
                        else if ( droid.TurretCount >= 1 )
                        {
                            if ( droid.Turret1 == null )
                            {
                                validDroid = false;
                                invalidPartCount++;
                            }
                        }
                        else if ( droid.TurretCount >= 2 )
                        {
                            if ( droid.Turret2 == null )
                            {
                                validDroid = false;
                                invalidPartCount++;
                            }
                            else if ( droid.Turret2.TurretType != enumTurretType.Weapon )
                            {
                                validDroid = false;
                                invalidPartCount++;
                            }
                        }
                        else if ( droid.TurretCount >= 3 && droid.Turret3 == null )
                        {
                            if ( droid.Turret3 == null )
                            {
                                validDroid = false;
                                invalidPartCount++;
                            }
                            else if ( droid.Turret3.TurretType != enumTurretType.Weapon )
                            {
                                validDroid = false;
                                invalidPartCount++;
                            }
                        }
                    }
                    if ( validDroid )
                    {
                        ini.AddSection("droid_" + unit.ID.ToStringInvariant());
                        ini.AddProperty("id", unit.ID.ToStringInvariant());
                        if ( unit.UnitGroup == map.ScavengerUnitGroup || (playerCount >= 0 & unit.UnitGroup.WZ_StartPos >= playerCount) )
                        {
                            ini.AddProperty("player", "scavenger");
                        }
                        else
                        {
                            ini.AddProperty("startpos", unit.UnitGroup.WZ_StartPos.ToStringInvariant());
                        }
                        if ( asPartsNotTemplate )
                        {
                            ini.AddProperty("name", droid.GenerateName());
                        }
                        else
                        {
                            template = (DroidTemplate)droid;
                            ini.AddProperty("template", template.Code);
                        }
                        ini.AddProperty("position", unit.GetINIPosition());
                        ini.AddProperty("rotation", unit.GetINIRotation());
                        if ( unit.Health < 1.0D )
                        {
                            ini.AddProperty("health", unit.GetINIHealthPercent());
                        }
                        if ( asPartsNotTemplate )
                        {
                            ini.AddProperty("droidType", Convert.ToInt32(droid.GetDroidType()).ToStringInvariant());
                            if ( droid.TurretCount == 0 )
                            {
                                text = "0";
                            }
                            else
                            {
                                if ( droid.Turret1.TurretType == enumTurretType.Brain )
                                {
                                    if ( ((Brain)droid.Turret1).Weapon == null )
                                    {
                                        text = "0";
                                    }
                                    else
                                    {
                                        text = "1";
                                    }
                                }
                                else
                                {
                                    if ( droid.Turret1.TurretType == enumTurretType.Weapon )
                                    {
                                        text = droid.TurretCount.ToStringInvariant();
                                    }
                                    else
                                    {
                                        text = "0";
                                    }
                                }
                            }
                            ini.AddProperty("weapons", text);
                            ini.AddProperty("parts\\body", droid.Body.Code);
                            ini.AddProperty("parts\\propulsion", droid.Propulsion.Code);
                            ini.AddProperty("parts\\sensor", droid.GetSensorCode());
                            ini.AddProperty("parts\\construct", droid.GetConstructCode());
                            ini.AddProperty("parts\\repair", droid.GetRepairCode());
                            ini.AddProperty("parts\\brain", droid.GetBrainCode());
                            ini.AddProperty("parts\\ecm", droid.GetECMCode());
                            if ( droid.TurretCount >= 1 )
                            {
                                if ( droid.Turret1.TurretType == enumTurretType.Weapon )
                                {
                                    ini.AddProperty("parts\\weapon\\1", droid.Turret1.Code);
                                    if ( droid.TurretCount >= 2 )
                                    {
                                        if ( droid.Turret2.TurretType == enumTurretType.Weapon )
                                        {
                                            ini.AddProperty("parts\\weapon\\2", droid.Turret2.Code);
                                            if ( droid.TurretCount >= 3 )
                                            {
                                                if ( droid.Turret3.TurretType == enumTurretType.Weapon )
                                                {
                                                    ini.AddProperty("parts\\weapon\\3", droid.Turret3.Code);
                                                }
                                            }
                                        }
                                    }
                                }
                                else if ( droid.Turret1.TurretType == enumTurretType.Brain )
                                {
                                    brain = (Brain)droid.Turret1;
                                    if ( brain.Weapon == null )
                                    {
                                        text = "ZNULLWEAPON";
                                    }
                                    else
                                    {
                                        text = brain.Weapon.Code;
                                    }
                                    ini.AddProperty("parts\\weapon\\1", text);
                                }
                            }
                        }
                    }
                }
            }

            if ( invalidPartCount > 0 )
            {
                returnResult.WarningAdd(string.Format("There were {0} droids with parts missing. They were not saved.", invalidPartCount));
            }

            return returnResult;
        }

        private clsResult Serialize_WZ_FeaturesINI(IniWriter File)
        {
            var ReturnResult = new clsResult("Serializing features INI", false);
            logger.Info("Serializing features INI");
            var featureTypeBase = default(FeatureTypeBase);
            var Unit = default(clsUnit);
            var Valid = default(bool);

            foreach ( var tempLoopVar_Unit in map.Units )
            {
                Unit = tempLoopVar_Unit;
                if ( Unit.TypeBase.Type != UnitType.Feature )
                {
                    continue;
                }

                featureTypeBase = (FeatureTypeBase)Unit.TypeBase;
                Valid = true;
                if ( Unit.ID <= 0 )
                {
                    Valid = false;
                    ReturnResult.WarningAdd("Error. A features\'s ID was zero. It was NOT saved. Delete and replace it to allow save.");
                }
                if ( Valid )
                {
                    File.AddSection("feature_" + Unit.ID.ToStringInvariant());
                    File.AddProperty("id", Unit.ID.ToStringInvariant());
                    File.AddProperty("position", Unit.GetINIPosition());
                    File.AddProperty("rotation", Unit.GetINIRotation());
                    File.AddProperty("name", featureTypeBase.Code);
                    if ( Unit.Health < 1.0D )
                    {
                        File.AddProperty("health", Unit.GetINIHealthPercent());
                    }
                }
            }

            return ReturnResult;
        }

        private clsResult Serialize_WZ_LabelsINI(IniWriter File, int PlayerCount)
        {
            var returnResult = new clsResult("Serializing labels INI", false);
            logger.Info("Serializing labels INI");

            try
            {
                var scriptPosition = default(clsScriptPosition);
                foreach ( var tempLoopVar_ScriptPosition in map.ScriptPositions )
                {
                    scriptPosition = tempLoopVar_ScriptPosition;
                    scriptPosition.WriteWZ(File);
                }
                var ScriptArea = default(clsScriptArea);
                foreach ( var tempLoopVar_ScriptArea in map.ScriptAreas )
                {
                    ScriptArea = tempLoopVar_ScriptArea;
                    ScriptArea.WriteWZ(File);
                }
                if ( PlayerCount >= 0 ) //not an FMap
                {
                    var Unit = default(clsUnit);
                    foreach ( var tempLoopVar_Unit in map.Units )
                    {
                        Unit = tempLoopVar_Unit;
                        Unit.WriteWZLabel(File, PlayerCount);
                    }
                }
            }
            catch ( Exception ex )
            {
                returnResult.WarningAdd(ex.Message);
                logger.ErrorException("Got an exception", ex);
            }

            return returnResult;
        }

        private clsResult Serialize_WZ_Gam(Stream stream, UInt32 gamType, clsInterfaceOptions.EnumCompileType compileType, XYInt scrollMin, sXY_uint scrollMax)
        {
            var returnResult = new clsResult("Serializing .gam", false);
            logger.Info("Serializing .gam");

            var fileGAM = new BinaryWriter(stream, App.ASCIIEncoding);

            IOUtil.WriteText(fileGAM, false, "game");
            fileGAM.Write(8U);
            fileGAM.Write(0U); //Time
            if ( compileType == clsInterfaceOptions.EnumCompileType.Multiplayer )
            {
                fileGAM.Write(0U);
            }
            else if ( compileType == clsInterfaceOptions.EnumCompileType.Campaign )
            {
                fileGAM.Write(gamType);
            }
            fileGAM.Write(scrollMin.X);
            fileGAM.Write(scrollMin.Y);
            fileGAM.Write(scrollMax.X);
            fileGAM.Write(scrollMax.Y);
            fileGAM.Write(new byte[20]);
            fileGAM.Flush();

            return returnResult;
        }

        private clsResult Serialize_WZ_Map(Stream stream)
        {
            var returnResult = new clsResult("Serializing game.map", false);
            logger.Info("Serializing game.map");

            var fileMAP = new BinaryWriter(stream, App.ASCIIEncoding);

            var x = 0;
            var y = 0;

            IOUtil.WriteText(fileMAP, false, "map ");
            fileMAP.Write(10U);
            fileMAP.Write((uint)map.Terrain.TileSize.X);
            fileMAP.Write((uint)map.Terrain.TileSize.Y);
            byte flip = 0;
            byte rotation = 0;
            var doFlipX = default(bool);
            var invalidTileCount = 0;
            var textureNum = 0;
            for ( y = 0; y <= map.Terrain.TileSize.Y - 1; y++ )
            {
                for ( x = 0; x <= map.Terrain.TileSize.X - 1; x++ )
                {
                    TileUtil.TileOrientation_To_OldOrientation(map.Terrain.Tiles[x, y].Texture.Orientation, ref rotation, ref doFlipX);
                    flip = 0;
                    if ( map.Terrain.Tiles[x, y].Tri )
                    {
                        flip += 8;
                    }
                    flip += (byte)(rotation * 16);
                    if ( doFlipX )
                    {
                        flip += 128;
                    }
                    textureNum = map.Terrain.Tiles[x, y].Texture.TextureNum;
                    if ( textureNum < 0 | textureNum > 255 )
                    {
                        textureNum = 0;
                        if ( invalidTileCount < 16 )
                        {
                            returnResult.WarningAdd(string.Format("Tile texture number {0} is invalid on tile {1}, {2} and was compiled as texture number {3}.",
                                                                  map.Terrain.Tiles[x, y].Texture.TextureNum, x, y, textureNum));
                        }
                        invalidTileCount++;
                    }
                    fileMAP.Write((byte)textureNum);
                    fileMAP.Write(flip);
                    fileMAP.Write(map.Terrain.Vertices[x, y].Height);
                }
            }
            if ( invalidTileCount > 0 )
            {
                returnResult.WarningAdd(string.Format("{0} tile texture numbers were invalid.", invalidTileCount));
            }
            fileMAP.Write(1U); //gateway version
            fileMAP.Write((uint)map.Gateways.Count);
            foreach ( var gateway in map.Gateways )
            {
                fileMAP.Write((byte)(MathUtil.ClampInt(gateway.PosA.X, 0, 255)));
                fileMAP.Write((byte)(MathUtil.ClampInt(gateway.PosA.Y, 0, 255)));
                fileMAP.Write((byte)(MathUtil.ClampInt(gateway.PosB.X, 0, 255)));
                fileMAP.Write((byte)(MathUtil.ClampInt(gateway.PosB.Y, 0, 255)));
            }
            fileMAP.Flush();

            return returnResult;
        }

        private clsResult Serialize_WZ_LEV(Stream stream)
        {
            var returnResult = new clsResult("Serializing .lev", false);
            logger.Info("Serializing .lev");

            var playercount = map.InterfaceOptions.CompileMultiPlayers;
            var authorname = map.InterfaceOptions.CompileMultiAuthor;
            var license = map.InterfaceOptions.CompileMultiLicense;
            var mapName = map.InterfaceOptions.CompileName;


            var fileLEV = new StreamWriter(stream, App.UTF8Encoding);

            var playersText = playercount.ToString();
            var playersPrefix = playersText + "c-";
            var fog = "";
            var tilesetNum = "";
            var endChar = "\n";

            if ( map.Tileset == App.Tileset_Arizona )
            {
                fog = "fog1.wrf";
                tilesetNum = "1";
            }
            else if ( map.Tileset == App.Tileset_Urban )
            {
                fog = "fog2.wrf";
                tilesetNum = "2";
            }
            else if ( map.Tileset == App.Tileset_Rockies )
            {
                fog = "fog3.wrf";
                tilesetNum = "3";
            }
            else
            {
                returnResult.ProblemAdd("Map must have a tileset, or unknown tileset selected.");
                return returnResult;
            }

            fileLEV.Write("// Made with {0} {1} {2}{3}", Constants.ProgramName, Constants.ProgramVersionNumber, Constants.ProgramPlatform, Convert.ToString(endChar));
            var DateNow = DateTime.Now;
            fileLEV.Write("// Date: {0}/{1}/{2} {3}:{4}:{5}{6}", DateNow.Year, App.MinDigits(DateNow.Month, 2), App.MinDigits(DateNow.Day, 2),
                          App.MinDigits(DateNow.Hour, 2), App.MinDigits(DateNow.Minute, 2), App.MinDigits(DateNow.Second, 2), endChar);
            fileLEV.Write("// Author: {0}{1}", authorname, endChar);
            fileLEV.Write("// License: {0}{1}", license, endChar);
            fileLEV.Write(endChar);
            fileLEV.Write("level   {0}-T1{1}", mapName, endChar);
            fileLEV.Write("players {0}{1}", playersText, endChar);
            fileLEV.Write("type    14{0}", endChar);
            fileLEV.Write("dataset MULTI_CAM_{0}{1}", tilesetNum, endChar);
            fileLEV.Write("game    \"multiplay/maps/{0}{1}.gam\"{2}", playersPrefix, mapName, endChar);
            fileLEV.Write("data    \"wrf/multi/skirmish{0}.wrf\"{1}", playersText, endChar);
            fileLEV.Write("data    \"wrf/multi/{0}\"{1}", fog, endChar);
            fileLEV.Write(endChar);
            fileLEV.Write("level   {0}-T2{1}", mapName, endChar);
            fileLEV.Write("players {0}{1}", playersText, endChar);
            fileLEV.Write("type    18{0}", endChar);
            fileLEV.Write("dataset MULTI_T2_C{0}{1}", tilesetNum, endChar);
            fileLEV.Write("game    \"multiplay/maps/{0}{1}.gam\"{2}", playersPrefix, mapName, endChar);
            fileLEV.Write("data    \"wrf/multi/t2-skirmish{0}.wrf\"{1}", playersText, endChar);
            fileLEV.Write("data    \"wrf/multi/{0}\"{1}", fog, endChar);
            fileLEV.Write(endChar);
            fileLEV.Write("level   {0}-T3{1}", mapName, endChar);
            fileLEV.Write("players {0}{1}", playersText, endChar);
            fileLEV.Write("type    19{0}", endChar);
            fileLEV.Write("dataset MULTI_T3_C{0}{1}", tilesetNum, endChar);
            fileLEV.Write("game    \"multiplay/maps/{0}{1}.gam\"{2}", playersPrefix, mapName, endChar);
            fileLEV.Write("data    \"wrf/multi/t3-skirmish{0}.wrf\"{1}", playersText, endChar);
            fileLEV.Write("data    \"wrf/multi/{0}\"{1}", fog, endChar);
            fileLEV.Flush();

            return returnResult;
        }
    }
}

