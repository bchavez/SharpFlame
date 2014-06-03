

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Eto.Forms;
using Ionic.Zip;
using Ionic.Zlib;
using NLog;
using SharpFlame.Core.Extensions;
using SharpFlame.Domain;
using SharpFlame.FileIO;
using SharpFlame.Mapping.IO.TTP;
using SharpFlame.Mapping.Objects;
using SharpFlame.Mapping.Script;
using SharpFlame.Mapping.Tiles;
using SharpFlame.Mapping.Tools;
using SharpFlame.Maths;
using SharpFlame.Core;
using SharpFlame.Core.Domain;
using SharpFlame.Core.Interfaces.Mapping.IO;
using SharpFlame.Core.Parsers;
using SharpFlame.Core.Parsers.Ini;
using SharpFlame.Core.Parsers.Lev;
using SharpFlame.Domain;
using SharpFlame.FileIO;
using SharpFlame.Mapping.IO.TTP;
using SharpFlame.Mapping.Objects;
using SharpFlame.Mapping.Script;
using SharpFlame.Mapping.Tiles;
using SharpFlame.Maths;
using SharpFlame.Util;
using SharpFlame.Util;
using Sprache;
using Result = SharpFlame.Core.Result;



namespace SharpFlame.Mapping.IO.Wz
{
    public class WzLoader : IIOLoader
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        protected readonly Map map;

        public WzLoader(Map newMap)
        {
            map = newMap;
        }

        public virtual Core.Result Load(string path)
        {
            var returnResult = new Core.Result(string.Format("Loading WZ from '{0}'.", path), false);
            logger.Info("Loading WZ from '{0}'.", path);
            var subResult = new SimpleResult();

            map.InterfaceOptions.FilePath = path;

            ZipSplitPath splitPath;
            var mapLoadName = "";

            using ( var zip = ZipFile.Read(path) )
            {
                foreach ( var e in zip )
                {
                    if ( e.IsDirectory )
                    {
                        continue;
                    }

                    splitPath = new ZipSplitPath(e.FileName);
                    logger.Debug("Found file \"{0}\".", e.FileName);
                    // Find the maps .lev
                    if ( splitPath.FileExtension != "lev" || splitPath.PartCount != 1 )
                    {
                        continue;
                    }

                    // Buggy file > 1MB
                    if ( e.UncompressedSize > 1 * 1024 * 1024 )
                    {
                        returnResult.ProblemAdd("lev file is too large.");
                        return returnResult;
                    }

                    using ( var s = e.OpenReader() )
                    {
                        var myresult = new Core.Result(string.Format("Parsing file \"{0}\"", e.FileName), false);
                        logger.Info("Parsing file \"{0}\"", e.FileName);

                        try
                        {
                            var r = new StreamReader(s);
                            var text = r.ReadToEnd();
                            var levFile = LevGrammar.Lev.Parse(text);

                            if ( levFile.Levels.Count < 1 )
                            {
                                myresult.ProblemAdd("No maps found in file.");
                                returnResult.Add(myresult);
                                return returnResult;
                            }

                            // Group games by the Game key.
                            var groupGames = levFile.Levels.GroupBy(level => level.Game);

                            // Load default map if only one Game file is found.
                            if ( groupGames.Count() == 1 )
                            {
                                var level = groupGames.First().First(); //first group, first level

                                mapLoadName = level.Game;

                                switch ( level.Dataset.Substring(level.Dataset.Length - 1, 1) )
                                {
                                    case "1":
                                        map.Tileset = App.Tileset_Arizona;
                                        break;
                                    case "2":
                                        map.Tileset = App.Tileset_Urban;
                                        break;
                                    case "3":
                                        map.Tileset = App.Tileset_Urban;
                                        break;
                                    default:
                                        myresult.ProblemAdd("Unknown tileset.");
                                        returnResult.Add(myresult);
                                        return returnResult;
                                }
                            }
                            else
                            {
                                //prompt user for which of the entries to load
                                var selectToLoadResult = new frmWZLoad.clsOutput();

                                var names = groupGames
                                    .Select(gameGroup => gameGroup.First().Name)
                                    .ToArray();

                                var selectToLoadForm = new frmWZLoad(names, selectToLoadResult,
                                    "Select a map from " + new sSplitPath(path).FileTitle);
                                selectToLoadForm.ShowDialog();
                                if ( selectToLoadResult.Result < 0 )
                                {
                                    returnResult.ProblemAdd("No map selected.");
                                    return returnResult;
                                }

                                var level = groupGames.ElementAt(selectToLoadResult.Result).First();

                                mapLoadName = level.Game;

                                switch ( level.Dataset.Substring(level.Dataset.Length - 1, 1) )
                                {
                                    case "1":
                                        map.Tileset = App.Tileset_Arizona;
                                        break;
                                    case "2":
                                        map.Tileset = App.Tileset_Urban;
                                        break;
                                    case "3":
                                        map.Tileset = App.Tileset_Urban;
                                        break;
                                    default:
                                        myresult.ProblemAdd("Unknown tileset.");
                                        returnResult.Add(myresult);
                                        return returnResult;
                                }
                            }
                        }
                        catch ( Exception ex )
                        {
                            myresult.ProblemAdd(string.Format("Got an exception while parsing the .lev file: {0}", ex), false);
                            returnResult.Add(myresult);
                            logger.ErrorException("Got an exception while parsing the .lev file", ex);
                            Debugger.Break();
                        }
                    }
                }

                map.TileType_Reset();
                map.SetPainterToDefaults();

                // mapLoadName is now multiplay/maps/<mapname>.gam (thats "game" from the .lev file
                var gameSplitPath = new ZipSplitPath(mapLoadName);
                var gameFilesPath = gameSplitPath.FilePath + gameSplitPath.FileTitleWithoutExtension + "/";

                var gameZipEntry = zip[mapLoadName];
                if ( gameZipEntry == null )
                {
                    returnResult.ProblemAdd(string.Format("Game file \"{0}\" not found.", mapLoadName), false);
                    logger.Error("Game file \"{0}\" not found.", mapLoadName);
                    return returnResult;
                }
                using ( Stream s = gameZipEntry.OpenReader() )
                {
                    var reader = new BinaryReader(s);
                    subResult = read_WZ_gam(reader);
                    reader.Close();
                    if ( !subResult.Success )
                    {
                        returnResult.ProblemAdd(subResult.Problem);
                        return returnResult;
                    }
                }


                var gameMapZipEntry = zip[gameFilesPath + "game.map"];
                if ( gameMapZipEntry == null )
                {
                    returnResult.ProblemAdd(string.Format("{0}game.map file not found", gameFilesPath));
                    return returnResult;
                }
                using ( Stream s = gameMapZipEntry.OpenReader() )
                {
                    var reader = new BinaryReader(s);
                    subResult = read_WZ_map(reader);
                    reader.Close();
                    if ( !subResult.Success )
                    {
                        returnResult.ProblemAdd(subResult.Problem);
                        return returnResult;
                    }
                }

                var bjoUnits = new List<WZBJOUnit>();

                var iniFeatures = new List<IniFeature>();
                var featureIniZipEntry = zip[gameFilesPath + "feature.ini"];
                if ( featureIniZipEntry != null )
                {
                    using ( var reader = new StreamReader(featureIniZipEntry.OpenReader()) )
                    {
                        var text = reader.ReadToEnd();
                        returnResult.Add(read_INI_Features(text, iniFeatures));
                    }
                }

                if ( iniFeatures.Count() == 0 ) // no feature.ini
                {
                    var Result = new Core.Result("feat.bjo", false);
                    logger.Info("Loading feat.bjo");

                    var featBJOZipEntry = zip[gameFilesPath + "feat.bjo"];
                    if ( featBJOZipEntry == null )
                    {
                        Result.WarningAdd(string.Format("{0}feat.bjo / feature.ini file not found", gameFilesPath));
                    }
                    else
                    {
                        using ( Stream s = featBJOZipEntry.OpenReader() )
                        {
                            var reader = new BinaryReader(s);
                            subResult = read_WZ_Features(reader, bjoUnits);
                            reader.Close();
                            if ( !subResult.Success )
                            {
                                Result.WarningAdd(subResult.Problem);
                            }
                        }
                    }
                    returnResult.Add(Result);
                }

                var ttypesEntry = zip[gameFilesPath + "ttypes.ttp"];
                if ( ttypesEntry == null )
                {
                    returnResult.WarningAdd(string.Format("{0}ttypes.ttp file not found", gameFilesPath));
                }
                else
                {
                    using ( var reader = new BinaryReader(ttypesEntry.OpenReader()) )
                    {
                        var ttpLoader = new TTPLoader (map);
                        returnResult.Add(ttpLoader.Load(reader));
                    }
                }

                var iniStructures = new List<IniStructure>();
                var structIniEntry = zip[gameFilesPath + "struct.ini"];
                if ( structIniEntry != null )
                {
                    using ( var reader = new StreamReader(structIniEntry.OpenReader()) )
                    {
                        var text = reader.ReadToEnd();
                        returnResult.Add(read_INI_Structures(text, iniStructures));
                    }
                }

                if ( iniStructures.Count() == 0 )
                {
                    var Result = new Core.Result("struct.bjo", false);
                    logger.Info("Loading struct.bjo");
                    var structBjoEntry = zip[gameFilesPath + "struct.bjo"];
                    if ( structBjoEntry == null )
                    {
                        Result.WarningAdd(string.Format("{0}struct.bjo / struct.ini file not found", gameFilesPath));
                    }
                    else
                    {
                        using ( Stream s = structBjoEntry.OpenReader() )
                        {
                            var reader = new BinaryReader(s);
                            subResult = read_WZ_Structures(reader, bjoUnits);
                            reader.Close();
                            if ( !subResult.Success )
                            {
                                Result.WarningAdd(subResult.Problem);
                            }
                        }
                    }
                    returnResult.Add(Result);
                }

                var iniDroids = new List<IniDroid>();
                if ( structIniEntry != null )
                {
                    var droidIniEntry = zip[gameFilesPath + "droid.ini"];
                    if ( droidIniEntry != null )
                    {
                        using ( var reader = new StreamReader(droidIniEntry.OpenReader()) )
                        {
                            var text = reader.ReadToEnd();
                            returnResult.Add(read_INI_Droids(text, iniDroids));
                        }
                    }
                }

                if ( iniDroids.Count() == 0 ) // No droid.ini
                {
                    var Result = new Core.Result("dinit.bjo", false);
                    logger.Info("Loading dinit.bjo");
                    var diniBjoEntry = zip[gameFilesPath + "dinit.bjo"];
                    if ( diniBjoEntry == null )
                    {
                        Result.WarningAdd(string.Format("{0}dinit.bjo / droid.ini file not found", gameFilesPath));
                    }
                    else
                    {
                        using ( Stream s = diniBjoEntry.OpenReader() )
                        {
                            var reader = new BinaryReader(s);
                            subResult = read_WZ_Droids(reader, bjoUnits);
                            reader.Close();
                            if ( !subResult.Success )
                            {
                                Result.WarningAdd(subResult.Problem);
                            }
                        }
                    }
                    returnResult.Add(Result);
                }

                returnResult.Add(createWZObjects(bjoUnits, iniStructures, iniDroids, iniFeatures));

                //objects are modified by this and must already exist
                var labelsIniEntry = zip[gameFilesPath + "labels.ini"];
                if ( labelsIniEntry != null )
                {
                    using ( var reader = new StreamReader(labelsIniEntry.OpenReader()) )
                    {
                        var text = reader.ReadToEnd();
                        returnResult.Add(read_INI_Labels(text));
                    }
                }
            }

            return returnResult;
        }

        protected Core.Result createWZObjects(List<WZBJOUnit> bjoUnits, List<IniStructure> iniStructures, List<IniDroid> iniDroids, List<IniFeature> iniFeatures)
        {
            var ReturnResult = new Core.Result("Creating objects", false);
            logger.Info("Creating objects");

            var newUnit = default(Unit);
            UInt32 availableID = 0;
            var unitAdd = new clsUnitAdd();
            var b = 0;

            unitAdd.Map = map;

            availableID = 1U;
            foreach ( var bjoUnit in bjoUnits )
            {
                if ( bjoUnit.ID >= availableID )
                {
                    availableID = bjoUnit.ID + 1U;
                }
            }
            if ( iniStructures.Count > 0)
            {
                var structMaxId = iniStructures.Max(w => w.ID) + 10;
                if ( structMaxId > availableID )
                {
                    availableID = structMaxId;
                }
            }
            if ( iniFeatures.Count > 0 )
            {
                var featuresMaxId = iniFeatures.Max(w => w.ID) + 10;
                if ( featuresMaxId > availableID )
                {
                    availableID = featuresMaxId;
                }
            }
            if ( iniDroids.Count > 0 )
            {
                var droidsMaxId = iniDroids.Max(w => w.ID) + 10;
                if ( droidsMaxId > availableID )
                {
                    availableID += droidsMaxId;
                }
            }

            foreach ( var bjoUnit in bjoUnits )
            {
                newUnit = new Unit();
                newUnit.ID = bjoUnit.ID;
                newUnit.TypeBase = App.ObjectData.FindOrCreateUnitType(bjoUnit.Code, bjoUnit.ObjectType, -1);
                if ( newUnit.TypeBase == null )
                {
                    ReturnResult.ProblemAdd("Unable to create object type.");
                    return ReturnResult;
                }
                if ( bjoUnit.Player >= Constants.PlayerCountMax )
                {
                    newUnit.UnitGroup = map.ScavengerUnitGroup;
                }
                else
                {
                    newUnit.UnitGroup = map.UnitGroups[Convert.ToInt32(bjoUnit.Player)];
                }
                newUnit.Pos = bjoUnit.Pos;
                newUnit.Rotation = Convert.ToInt32(Math.Min(bjoUnit.Rotation, 359U));
                if ( bjoUnit.ID == 0U )
                {
                    bjoUnit.ID = availableID;
                    App.ZeroIDWarning(newUnit, bjoUnit.ID, ReturnResult);
                }
                unitAdd.NewUnit = newUnit;
                unitAdd.ID = bjoUnit.ID;
                unitAdd.Perform();
                App.ErrorIDChange(bjoUnit.ID, newUnit, "CreateWZObjects");
                if ( availableID == bjoUnit.ID )
                {
                    availableID = newUnit.ID + 1U;
                }
            }

            var structureTypeBase = default(StructureTypeBase);
            var droidType = default(DroidDesign);
            var featureTypeBase = default(FeatureTypeBase);
            var loadPartsArgs = new DroidDesign.sLoadPartsArgs();
            UnitTypeBase unitTypeBase = null;
            var errorCount = 0;
            var unknownDroidComponentCount = 0;
            var unknownDroidTypeCount = 0;
            var droidBadPositionCount = 0;
            var structureBadPositionCount = 0;
            var structureBadModulesCount = 0;
            var featureBadPositionCount = 0;
            var moduleLimit = 0;
            var zeroPos = new XYInt(0, 0);
            var moduleTypeBase = default(StructureTypeBase);
            var newModule = default(Unit);

            var factoryModule = App.ObjectData.FindFirstStructureType(StructureType.FactoryModule);
            var researchModule = App.ObjectData.FindFirstStructureType(StructureType.ResearchModule);
            var powerModule = App.ObjectData.FindFirstStructureType(StructureType.PowerModule);

            if ( factoryModule == null )
            {
                ReturnResult.WarningAdd("No factory module loaded.");
            }
            if ( researchModule == null )
            {
                ReturnResult.WarningAdd("No research module loaded.");
            }
            if ( powerModule == null )
            {
                ReturnResult.WarningAdd("No power module loaded.");
            }

            foreach ( var iniStructure in iniStructures )
            {
                if ( iniStructure.Pos == null )
                {
                    logger.Debug("{0} pos was null", iniStructure.Code);
                    structureBadPositionCount++;
                }
                else if ( !App.PosIsWithinTileArea(iniStructure.Pos, zeroPos, map.Terrain.TileSize) )
                {
                    logger.Debug("{0} structure pos x{1} y{2}, is wrong.", iniStructure.Code, iniStructure.Pos.X,
                        iniStructure.Pos.Y);
                    structureBadPositionCount++;
                }
                else
                {
                    unitTypeBase = App.ObjectData.FindOrCreateUnitType(Convert.ToString(iniStructure.Code),
                        UnitType.PlayerStructure, iniStructure.WallType);
                    if ( unitTypeBase.Type == UnitType.PlayerStructure )
                    {
                        structureTypeBase = (StructureTypeBase)unitTypeBase;
                    }
                    else
                    {
                        structureTypeBase = null;
                    }
                    if ( structureTypeBase == null )
                    {
                        errorCount++;
                    }
                    else
                    {
                        newUnit = new Unit();
                        newUnit.TypeBase = structureTypeBase;
                        if ( iniStructure.UnitGroup == null )
                        {
                            newUnit.UnitGroup = map.ScavengerUnitGroup;
                        }
                        else
                        {
                            newUnit.UnitGroup = iniStructure.UnitGroup;
                        }
                        newUnit.Pos = new WorldPos(iniStructure.Pos, iniStructure.Pos.Z);
                        newUnit.Rotation = Convert.ToInt32(iniStructure.Rotation.Direction * 360.0D / Constants.IniRotationMax);
                        if ( newUnit.Rotation == 360 )
                        {
                            newUnit.Rotation = 0;
                        }
                        if ( iniStructure.HealthPercent >= 0 )
                        {
                            newUnit.Health = MathUtil.ClampDbl(iniStructure.HealthPercent / 100.0D, 0.01D, 1.0D);
                        }
                        if ( iniStructure.ID == 0U )
                        {
                            iniStructure.ID = availableID;
                            App.ZeroIDWarning(newUnit, iniStructure.ID, ReturnResult);
                        }
                        unitAdd.NewUnit = newUnit;
                        unitAdd.ID = iniStructure.ID;
                        unitAdd.Perform();
                        App.ErrorIDChange(iniStructure.ID, newUnit, "Load_WZ->INIStructures");
                        if ( availableID == iniStructure.ID )
                        {
                            availableID = newUnit.ID + 1U;
                        }
                        //create modules
                        switch ( structureTypeBase.StructureType )
                        {
                            case StructureType.Factory:
                                moduleLimit = 2;
                                moduleTypeBase = factoryModule;
                                break;
                            case StructureType.VTOLFactory:
                                moduleLimit = 2;
                                moduleTypeBase = factoryModule;
                                break;
                            case StructureType.PowerGenerator:
                                moduleLimit = 1;
                                moduleTypeBase = powerModule;
                                break;
                            case StructureType.Research:
                                moduleLimit = 1;
                                moduleTypeBase = researchModule;
                                break;
                            default:
                                moduleLimit = 0;
                                moduleTypeBase = null;
                                break;
                        }
                        if ( iniStructure.ModuleCount > moduleLimit )
                        {
                            iniStructure.ModuleCount = moduleLimit;
                            structureBadModulesCount++;
                        }
                        else if ( iniStructure.ModuleCount < 0 )
                        {
                            iniStructure.ModuleCount = 0;
                            structureBadModulesCount++;
                        }
                        if ( moduleTypeBase != null )
                        {
                            for ( b = 0; b <= iniStructure.ModuleCount - 1; b++ )
                            {
                                newModule = new Unit();
                                newModule.TypeBase = moduleTypeBase;
                                newModule.UnitGroup = newUnit.UnitGroup;
                                newModule.Pos = newUnit.Pos;
                                newModule.Rotation = newUnit.Rotation;
                                unitAdd.NewUnit = newModule;
                                unitAdd.ID = availableID;
                                unitAdd.Perform();
                                availableID = newModule.ID + 1U;
                            }
                        }
                    }
                }
            }
            if ( structureBadPositionCount > 0 )
            {
                ReturnResult.WarningAdd(structureBadPositionCount + " structures had an invalid position and were removed.");
            }
            if ( structureBadModulesCount > 0 )
            {
                ReturnResult.WarningAdd(structureBadModulesCount + " structures had an invalid number of modules.");
            }

            foreach ( var iniFeature in iniFeatures )
            {
                if ( iniFeature.Pos == null )
                {
                    featureBadPositionCount++;
                }
                else if ( !App.PosIsWithinTileArea(iniFeature.Pos, zeroPos, map.Terrain.TileSize) )
                {
                    featureBadPositionCount++;
                }
                else
                {
                    unitTypeBase = App.ObjectData.FindOrCreateUnitType(Convert.ToString(iniFeature.Code), UnitType.Feature, -1);
                    if ( unitTypeBase.Type == UnitType.Feature )
                    {
                        featureTypeBase = (FeatureTypeBase)unitTypeBase;
                    }
                    else
                    {
                        featureTypeBase = null;
                    }
                    if ( featureTypeBase == null )
                    {
                        errorCount++;
                    }
                    else
                    {
                        newUnit = new Unit();
                        newUnit.TypeBase = featureTypeBase;
                        newUnit.UnitGroup = map.ScavengerUnitGroup;
                        newUnit.Pos = new WorldPos(iniFeature.Pos, iniFeature.Pos.Z);
                        newUnit.Rotation = Convert.ToInt32(iniFeature.Rotation.Direction * 360.0D / Constants.IniRotationMax);
                        if ( newUnit.Rotation == 360 )
                        {
                            newUnit.Rotation = 0;
                        }
                        if ( iniFeature.HealthPercent >= 0 )
                        {
                            newUnit.Health = MathUtil.ClampDbl(iniFeature.HealthPercent / 100.0D, 0.01D, 1.0D);
                        }
                        if ( iniFeature.ID == 0U )
                        {
                            iniFeature.ID = availableID;
                            App.ZeroIDWarning(newUnit, iniFeature.ID, ReturnResult);
                        }
                        unitAdd.NewUnit = newUnit;
                        unitAdd.ID = iniFeature.ID;
                        unitAdd.Perform();
                        App.ErrorIDChange(iniFeature.ID, newUnit, "Load_WZ->INIFeatures");
                        if ( availableID == iniFeature.ID )
                        {
                            availableID = newUnit.ID + 1U;
                        }
                    }
                }
            }
            if ( featureBadPositionCount > 0 )
            {
                ReturnResult.WarningAdd(featureBadPositionCount + " features had an invalid position and were removed.");
            }

            foreach ( var iniDroid in iniDroids )
            {
                if ( iniDroid.Pos == null )
                {
                    droidBadPositionCount++;
                }
                else if ( !App.PosIsWithinTileArea(iniDroid.Pos, zeroPos, map.Terrain.TileSize) )
                {
                    droidBadPositionCount++;
                }
                else
                {
                    if ( iniDroid.Template == null || iniDroid.Template == "" )
                    {
                        droidType = new DroidDesign();
                        if ( !droidType.SetDroidType((DroidType)(iniDroid.DroidType)) )
                        {
                            unknownDroidTypeCount++;
                        }
                        loadPartsArgs.Body = App.ObjectData.FindOrCreateBody(iniDroid.Body);
                        if ( loadPartsArgs.Body == null )
                        {
                            unknownDroidComponentCount++;
                        }
                        else
                        {
                            if ( loadPartsArgs.Body.IsUnknown )
                            {
                                unknownDroidComponentCount++;
                            }
                        }
                        loadPartsArgs.Propulsion = App.ObjectData.FindOrCreatePropulsion(Convert.ToString(iniDroid.Propulsion));
                        if ( loadPartsArgs.Propulsion == null )
                        {
                            unknownDroidComponentCount++;
                        }
                        else
                        {
                            if ( loadPartsArgs.Propulsion.IsUnknown )
                            {
                                unknownDroidComponentCount++;
                            }
                        }
                        loadPartsArgs.Construct = App.ObjectData.FindOrCreateConstruct(Convert.ToString(iniDroid.Construct));
                        if ( loadPartsArgs.Construct == null )
                        {
                            unknownDroidComponentCount++;
                        }
                        else
                        {
                            if ( loadPartsArgs.Construct.IsUnknown )
                            {
                                unknownDroidComponentCount++;
                            }
                        }
                        loadPartsArgs.Repair = App.ObjectData.FindOrCreateRepair(iniDroid.Repair);
                        if ( loadPartsArgs.Repair == null )
                        {
                            unknownDroidComponentCount++;
                        }
                        else
                        {
                            if ( loadPartsArgs.Repair.IsUnknown )
                            {
                                unknownDroidComponentCount++;
                            }
                        }
                        loadPartsArgs.Sensor = App.ObjectData.FindOrCreateSensor(iniDroid.Sensor);
                        if ( loadPartsArgs.Sensor == null )
                        {
                            unknownDroidComponentCount++;
                        }
                        else
                        {
                            if ( loadPartsArgs.Sensor.IsUnknown )
                            {
                                unknownDroidComponentCount++;
                            }
                        }
                        loadPartsArgs.Brain = App.ObjectData.FindOrCreateBrain(iniDroid.Brain);
                        if ( loadPartsArgs.Brain == null )
                        {
                            unknownDroidComponentCount++;
                        }
                        else
                        {
                            if ( loadPartsArgs.Brain.IsUnknown )
                            {
                                unknownDroidComponentCount++;
                            }
                        }
                        loadPartsArgs.ECM = App.ObjectData.FindOrCreateECM(Convert.ToString(iniDroid.ECM));
                        if ( loadPartsArgs.ECM == null )
                        {
                            unknownDroidComponentCount++;
                        }
                        else
                        {
                            if ( loadPartsArgs.ECM.IsUnknown )
                            {
                                unknownDroidComponentCount++;
                            }
                        }
                        loadPartsArgs.Weapon1 = App.ObjectData.FindOrCreateWeapon(Convert.ToString(iniDroid.Weapons[0]));
                        if ( loadPartsArgs.Weapon1 == null )
                        {
                            unknownDroidComponentCount++;
                        }
                        else
                        {
                            if ( loadPartsArgs.Weapon1.IsUnknown )
                            {
                                unknownDroidComponentCount++;
                            }
                        }
                        loadPartsArgs.Weapon2 = App.ObjectData.FindOrCreateWeapon(Convert.ToString(iniDroid.Weapons[1]));
                        if ( loadPartsArgs.Weapon2 == null )
                        {
                            unknownDroidComponentCount++;
                        }
                        else
                        {
                            if ( loadPartsArgs.Weapon2.IsUnknown )
                            {
                                unknownDroidComponentCount++;
                            }
                        }
                        loadPartsArgs.Weapon3 = App.ObjectData.FindOrCreateWeapon(Convert.ToString(iniDroid.Weapons[2]));
                        if ( loadPartsArgs.Weapon3 == null )
                        {
                            unknownDroidComponentCount++;
                        }
                        else
                        {
                            if ( loadPartsArgs.Weapon3.IsUnknown )
                            {
                                unknownDroidComponentCount++;
                            }
                        }
                        droidType.LoadParts(loadPartsArgs);
                    }
                    else
                    {
                        unitTypeBase = App.ObjectData.FindOrCreateUnitType(iniDroid.Template, UnitType.PlayerDroid, -1);
                        if ( unitTypeBase == null )
                        {
                            droidType = null;
                        }
                        else
                        {
                            if ( unitTypeBase.Type == UnitType.PlayerDroid )
                            {
                                droidType = (DroidDesign)unitTypeBase;
                            }
                            else
                            {
                                droidType = null;
                            }
                        }
                    }
                    if ( droidType == null )
                    {
                        errorCount++;
                    }
                    else
                    {
                        newUnit = new Unit();
                        newUnit.TypeBase = droidType;
                        if ( iniDroid.UnitGroup == null )
                        {
                            newUnit.UnitGroup = map.ScavengerUnitGroup;
                        }
                        else
                        {
                            newUnit.UnitGroup = iniDroid.UnitGroup;
                        }
                        newUnit.Pos = new WorldPos(iniDroid.Pos, iniDroid.Pos.Z);
                        newUnit.Rotation = Convert.ToInt32(iniDroid.Rotation.Direction * 360.0D / Constants.IniRotationMax);
                        if ( newUnit.Rotation == 360 )
                        {
                            newUnit.Rotation = 0;
                        }
                        if ( iniDroid.HealthPercent >= 0 )
                        {
                            newUnit.Health = MathUtil.ClampDbl(iniDroid.HealthPercent / 100.0D, 0.01D, 1.0D);
                        }
                        if ( iniDroid.ID == 0U )
                        {
                            iniDroid.ID = availableID;
                            App.ZeroIDWarning(newUnit, iniDroid.ID, ReturnResult);
                        }
                        unitAdd.NewUnit = newUnit;
                        unitAdd.ID = iniDroid.ID;
                        unitAdd.Perform();
                        App.ErrorIDChange(iniDroid.ID, newUnit, "Load_WZ->INIDroids");
                        if ( availableID == iniDroid.ID )
                        {
                            availableID = newUnit.ID + 1U;
                        }
                    }
                }
            }
            if ( droidBadPositionCount > 0 )
            {
                ReturnResult.WarningAdd(droidBadPositionCount + " droids had an invalid position and were removed.");
            }
            if ( unknownDroidTypeCount > 0 )
            {
                ReturnResult.WarningAdd(unknownDroidTypeCount + " droid designs had an unrecognised droidType and were removed.");
            }
            if ( unknownDroidComponentCount > 0 )
            {
                ReturnResult.WarningAdd(unknownDroidComponentCount + " droid designs had components that are not loaded.");
            }

            if ( errorCount > 0 )
            {
                ReturnResult.WarningAdd("Object Create Error.");
            }

            return ReturnResult;
        }

        protected Core.Result read_INI_Features(string iniText, List<IniFeature> resultData)
        {
            var resultObject = new Core.Result("Reading feature.ini.", false);
            logger.Info("Reading feature.ini");

            try
            {
                var iniSections = IniReader.ReadString(iniText);
                foreach ( var iniSection in iniSections )
                {
                    var feature = new IniFeature();
                    feature.HealthPercent = -1;
                    var invalid = false;
                    foreach ( var iniToken in iniSection.Data )
                    {
                        if ( invalid )
                        {
                            break;
                        }

                        try
                        {
                            switch ( iniToken.Name )
                            {
                                case "id":
                                    feature.ID = uint.Parse(iniToken.Data);
                                    break;
                                case "name":
                                    feature.Code = iniToken.Data;
                                    break;
                                case "position":
                                    feature.Pos = XYZInt.FromString(iniToken.Data);
                                    break;
                                case "rotation":
                                    feature.Rotation = Rotation.FromString(iniToken.Data);
                                    break;
                                case "health":
                                    feature.HealthPercent = IniReader.ReadHealthPercent(iniToken.Data);
                                    if ( feature.HealthPercent < 0 || feature.HealthPercent > 100 )
                                    {
                                        resultObject.WarningAdd(string.Format("#{0} invalid health: \"{1}\"", iniSection.Name, feature.HealthPercent), false);
                                        logger.Warn("#{0} invalid health: \"{1}\"", iniSection.Name, feature.HealthPercent);
                                        invalid = true;
                                    }
                                    break;
                                default:
                                    resultObject.WarningAdd(string.Format("Found an invalid key: {0}={1}", iniToken.Name, iniToken.Data), false);
                                    logger.Warn("Found an invalid key: {0} = {1}", iniToken.Name, iniToken.Data);
                                    break;
                            }
                        }
                        catch ( Exception ex )
                        {
                            Debugger.Break();
                            resultObject.WarningAdd(
                                string.Format("#{0} invalid {2}: \"{3}\", got exception: {2}", iniSection.Name, iniToken.Name, iniToken.Data, ex.Message), false);
                            logger.WarnException(string.Format("#{0} invalid {2} \"{1}\"", iniSection.Name, iniToken.Name, iniToken.Data), ex);
                            invalid = true;
                        }
                    }

                    if ( !invalid )
                    {
                        resultData.Add(feature);
                    }
                }
            }
            catch ( Exception ex )
            {
                Debugger.Break();
                logger.ErrorException("Got exception while reading feature.ini", ex);
                resultObject.ProblemAdd(string.Format("Got exception: {0}", ex.Message), false);
                return resultObject;
            }

            return resultObject;
        }

        protected Core.Result read_INI_Droids(string iniText, List<IniDroid> resultData)
        {
            var resultObject = new Core.Result("Reading droids.ini.", false);

            try
            {
                var iniSections = IniReader.ReadString(iniText);
                foreach ( var iniSection in iniSections )
                {
                    var droid = new IniDroid();
                    droid.HealthPercent = -1;
                    var invalid = false;
                    foreach ( var iniToken in iniSection.Data )
                    {
                        if ( invalid )
                        {
                            break;
                        }

                        try
                        {
                            switch ( iniToken.Name )
                            {
                                case "id":
                                    droid.ID = uint.Parse(iniToken.Data);
                                    break;

                                case "startpos":
                                    var tmpStartPos = int.Parse(iniToken.Data);
                                    if ( tmpStartPos < 0 | tmpStartPos >= Constants.PlayerCountMax )
                                    {
                                        resultObject.WarningAdd(string.Format("#{0} invalid startpos {1}", iniSection.Name, tmpStartPos), false);
                                        logger.Warn("#{0} invalid startpos {1}", iniSection.Name, tmpStartPos);
                                        invalid = true;
                                        continue;
                                    }
                                    droid.UnitGroup = map.UnitGroups[tmpStartPos];
                                    break;

                                case "template":
                                    droid.Template = iniToken.Data;
                                    break;

                                case "position":
                                    droid.Pos = XYZInt.FromString(iniToken.Data);
                                    break;

                                case "rotation":
                                    droid.Rotation = Rotation.FromString(iniToken.Data);
                                    break;

                                case "player":
                                    if ( iniToken.Data.ToLower() == "scavenger" )
                                    {
                                        droid.UnitGroup = map.ScavengerUnitGroup;
                                    }
                                    else
                                    {
                                        resultObject.WarningAdd(string.Format("#{0} invalid player: \"{1}\"", iniToken.Name, iniToken.Data), false);
                                        logger.Warn("#{0} invalid player \"{1}\"", iniToken.Name, iniToken.Data);
                                        invalid = true;
                                    }
                                    break;

                                case "name":
                                    // ignore
                                    break;

                                case "health":
                                    droid.HealthPercent = IniReader.ReadHealthPercent(iniToken.Data);
                                    if ( droid.HealthPercent < 0 || droid.HealthPercent > 100 )
                                    {
                                        resultObject.WarningAdd(string.Format("#{0} invalid health: \"{1}\"", iniSection.Name, droid.HealthPercent), false);
                                        invalid = true;
                                    }
                                    break;

                                case "droidtype":
                                    droid.DroidType = Numerics.Int.Parse(iniToken.Data);
                                    break;

                                case "weapons":
                                    droid.WeaponCount = Numerics.Int.Parse(iniToken.Data);
                                    break;

                                case "parts\\body":
                                    droid.Body = iniToken.Data;
                                    break;

                                case "parts\\propulsion":
                                    droid.Propulsion = iniToken.Data;
                                    break;

                                case "parts\\brain":
                                    droid.Brain = iniToken.Data;
                                    break;

                                case "parts\\repair":
                                    droid.Repair = iniToken.Data;
                                    break;

                                case "parts\\ecm":
                                    droid.ECM = iniToken.Data;
                                    break;

                                case "parts\\sensor":
                                    droid.Sensor = iniToken.Data;
                                    break;

                                case "parts\\construct":
                                    droid.Construct = iniToken.Data;
                                    break;

                                case "parts\\weapon\\1":
                                    if ( droid.Weapons == null )
                                    {
                                        droid.Weapons = new string[3];
                                    }
                                    droid.Weapons[0] = iniToken.Data;
                                    break;

                                case "parts\\weapon\\2":
                                    if ( droid.Weapons == null )
                                    {
                                        droid.Weapons = new string[3];
                                    }

                                    droid.Weapons[1] = iniToken.Data;
                                    break;

                                case "parts\\weapon\\3":
                                    if ( droid.Weapons == null )
                                    {
                                        droid.Weapons = new string[3];
                                    }

                                    droid.Weapons[2] = iniToken.Data;
                                    break;

                                default:
                                    resultObject.WarningAdd(string.Format("Found an invalid key: {0} = {1}", iniToken.Name, iniToken.Data), false);
                                    logger.Warn("Found an invalid key: {0} = {1}", iniToken.Name, iniToken.Data);
                                    break;
                            }
                        }
                        catch ( Exception ex )
                        {
                            Debugger.Break();
                            resultObject.WarningAdd(
                                string.Format("#{0} invalid {2}: \"{3}\", got exception: {2}", iniSection.Name, iniToken.Name, iniToken.Data, ex.Message), false);
                            logger.ErrorException(string.Format("#{0} invalid {2} \"{1}\"", iniSection.Name, iniToken.Name, iniToken.Data), ex);
                            invalid = true;
                        }
                    }

                    if ( !invalid )
                    {
                        resultData.Add(droid);
                    }
                }
            }
            catch ( Exception ex )
            {
                Debugger.Break();
                logger.ErrorException("Got exception while reading droid.ini", ex);
                resultObject.ProblemAdd(string.Format("Got exception: {0}", ex.Message), false);
                return resultObject;
            }

            return resultObject;
        }

        protected Core.Result read_INI_Structures(string iniText, List<IniStructure> resultData)
        {
            var resultObject = new Core.Result("Reading struct.ini.", false);
            logger.Info("Reading struct.ini");

            try
            {
                var iniSections = IniReader.ReadString(iniText);
                foreach ( var iniSection in iniSections )
                {
                    var structure = new IniStructure();
                    structure.WallType = -1;
                    structure.HealthPercent = -1;
                    var invalid = false;
                    foreach ( var iniToken in iniSection.Data )
                    {
                        if ( invalid )
                        {
                            break;
                        }

                        try
                        {
                            switch ( iniToken.Name )
                            {
                                case "id":
                                    structure.ID = uint.Parse(iniToken.Data);
                                    break;

                                case "name":
                                    structure.Code = iniToken.Data;
                                    break;

                                case "startpos":
                                    var tmpStartPos = int.Parse(iniToken.Data);
                                    if ( tmpStartPos < 0 | tmpStartPos >= Constants.PlayerCountMax )
                                    {
                                        resultObject.WarningAdd(string.Format("#{0} invalid startpos {1}", iniSection.Name, tmpStartPos), false);
                                        logger.Warn("#{0} invalid startpos {1}", iniSection.Name, tmpStartPos);
                                        invalid = true;
                                        continue;
                                    }
                                    structure.UnitGroup = map.UnitGroups[tmpStartPos];
                                    break;

                                case "player":
                                    if ( iniToken.Data.ToLower() == "scavenger" )
                                    {
                                        structure.UnitGroup = map.ScavengerUnitGroup;
                                    }
                                    else
                                    {
                                        resultObject.WarningAdd(string.Format("#{0} invalid player: \"{1}\"", iniToken.Name, iniToken.Data), false);
                                        logger.Warn("#{0} invalid player \"{1}\"", iniToken.Name, iniToken.Data);
                                        invalid = true;
                                    }
                                    break;

                                case "position":
                                    structure.Pos = XYZInt.FromString(iniToken.Data);
                                    break;

                                case "rotation":
                                    structure.Rotation = Rotation.FromString(iniToken.Data);
                                    break;

                                case "modules":
                                    structure.ModuleCount = int.Parse(iniToken.Data);
                                    break;

                                case "health":
                                    structure.HealthPercent = IniReader.ReadHealthPercent(iniToken.Data);
                                    if ( structure.HealthPercent < 0 || structure.HealthPercent > 100 )
                                    {
                                        resultObject.WarningAdd(string.Format("#{0} invalid health: \"{1}\"", iniSection.Name, structure.HealthPercent), false);
                                        invalid = true;
                                    }
                                    break;

                                case "wall/type":
                                    structure.WallType = int.Parse(iniToken.Data);
                                    if ( structure.WallType < 0 )
                                    {
                                        resultObject.WarningAdd(string.Format("#{0} invalid wall/type: \"{1}\"", iniSection.Name, structure.WallType), false);
                                        invalid = true;
                                    }
                                    break;

                                default:
                                    resultObject.WarningAdd(string.Format("Found an invalid key: {0} = {1}", iniToken.Name, iniToken.Data), false);
                                    logger.Warn("Found an invalid key: {0} = {1}", iniToken.Name, iniToken.Data);
                                    break;
                            }
                        }
                        catch ( Exception ex )
                        {
                            Debugger.Break();
                            resultObject.WarningAdd(
                                string.Format("#{0} invalid {2}: \"{3}\", got exception: {2}", iniSection.Name, iniToken.Name, iniToken.Data, ex.Message), false);
                            logger.WarnException(string.Format("#{0} invalid {2} \"{1}\"", iniSection.Name, iniToken.Name, iniToken.Data), ex);
                            invalid = true;
                        }
                    }

                    if ( !invalid )
                    {
                        resultData.Add(structure);
                    }
                }
            }
            catch ( Exception ex )
            {
                Debugger.Break();
                logger.ErrorException("Got exception while reading droid.ini", ex);
                resultObject.ProblemAdd(string.Format("Got exception: {0}", ex.Message), false);
                return resultObject;
            }

            return resultObject;
        }

        protected Core.Result read_INI_Labels(string iniText)
        {
            var resultObject = new Core.Result("Reading labels", false);
            logger.Info("Reading labels.");

            var typeNum = 0;
            var NewPosition = default(clsScriptPosition);
            var NewArea = default(clsScriptArea);
            var nameText = "";
            var strLabel = "";
            var strPosA = "";
            var strPosB = "";
            var idText = "";
            UInt32 idNum = 0;
            XYInt xyIntA = null;
            XYInt xyIntB = null;

            var failedCount = 0;
            var modifiedCount = 0;

            try
            {
                var iniSections = IniReader.ReadString(iniText);
                foreach ( var iniSection in iniSections )
                {
                    var idx = iniSection.Name.IndexOf('_');
                    if ( idx > 0 )
                    {
                        nameText = iniSection.Name.Substring(0, idx);
                    }
                    else
                    {
                        nameText = iniSection.Name;
                    }
                    switch ( nameText )
                    {
                        case "position":
                            typeNum = 0;
                            break;
                        case "area":
                            typeNum = 1;
                            break;
                        case "object":
                            typeNum = 2;
                            break;
                        default:
                            typeNum = int.MaxValue;
                            failedCount++;
                            continue;
                    }

                    // Raised an exception if nothing was found
                    try
                    {
                        strLabel = iniSection.Data.Where(d => d.Name == "label").First().Data;
                    }
                    catch ( Exception ex )
                    {
                        resultObject.WarningAdd(string.Format("Failed to parse \"label\", error was: {0}", ex.Message));
                        logger.WarnException("Failed to parse \"label\", error was", ex);
                        failedCount++;
                        continue;
                    }
                    strLabel = strLabel.Replace("\"", "");

                    switch ( typeNum )
                    {
                        case 0: //position
                            strPosA = iniSection.Data.Where(d => d.Name == "pos").First().Data;
                            if ( strPosA == null )
                            {
                                failedCount++;
                                continue;
                            }
                            try
                            {
                                xyIntA = XYInt.FromString(strPosA);
                                NewPosition = new clsScriptPosition(map);
                                NewPosition.PosX = xyIntA.X;
                                NewPosition.PosY = xyIntA.Y;
                                NewPosition.SetLabel(strLabel);
                                if ( NewPosition.Label != strLabel ||
                                     NewPosition.PosX != xyIntA.X || NewPosition.PosY != xyIntA.Y )
                                {
                                    modifiedCount++;
                                }
                            }
                            catch ( Exception ex )
                            {
                                resultObject.WarningAdd(string.Format("Failed to parse \"pos\", error was: {0}", ex.Message));
                                logger.WarnException("Failed to parse \"pos\", error was", ex);
                                failedCount++;
                            }
                            break;
                        case 1: //area
                            try
                            {
                                strPosA = iniSection.Data.Where(d => d.Name == "pos1").First().Data;
                                strPosB = iniSection.Data.Where(d => d.Name == "pos2").First().Data;

                                xyIntA = XYInt.FromString(strPosA);
                                xyIntB = XYInt.FromString(strPosA);
                                NewArea = new clsScriptArea(map);
                                NewArea.SetPositions(xyIntA, xyIntB);
                                NewArea.SetLabel(strLabel);
                                if ( NewArea.Label != strLabel || NewArea.PosAX != xyIntA.X | NewArea.PosAY != xyIntA.Y
                                     | NewArea.PosBX != xyIntB.X | NewArea.PosBY != xyIntB.Y )
                                {
                                    modifiedCount++;
                                }
                            }
                            catch ( Exception ex )
                            {
                                Debugger.Break();
                                resultObject.WarningAdd(string.Format("Failed to parse \"pos1\" or \"pos2\", error was: {0}", ex.Message));
                                logger.WarnException("Failed to parse \"pos1\" or \"pos2\".", ex);
                                failedCount++;
                            }
                            break;
                        case 2: //object
                            idText = iniSection.Data.Where(d => d.Name == "id").First().Data;
                            if ( IOUtil.InvariantParse(idText, ref idNum) )
                            {
                                var Unit = map.IDUsage(idNum);
                                if ( Unit != null )
                                {
                                    if ( !Unit.SetLabel(strLabel).Success )
                                    {
                                        failedCount++;
                                    }
                                }
                                else
                                {
                                    failedCount++;
                                }
                            }
                            break;
                        default:
                            resultObject.WarningAdd("Error! Bad type number for script label.");
                            break;
                    }
                }
            }
            catch ( Exception ex )
            {
                Debugger.Break();
                logger.ErrorException("Got exception while reading labels.ini", ex);
                resultObject.ProblemAdd(string.Format("Got exception: {0}", ex.Message), false);
                return resultObject;
            }

            if ( failedCount > 0 )
            {
                resultObject.WarningAdd(string.Format("Unable to translate {0} script labels.", failedCount));
            }
            if ( modifiedCount > 0 )
            {
                resultObject.WarningAdd(string.Format("{0} script labels had invalid values and were modified.", modifiedCount));
            }

            return resultObject;
        }

        protected SimpleResult read_WZ_gam(BinaryReader File)
        {
            var ReturnResult = new SimpleResult();
            ReturnResult.Success = false;
            ReturnResult.Problem = "";

            var strTemp = "";
            UInt32 Version = 0;

            try
            {
                strTemp = IOUtil.ReadOldTextOfLength(File, 4);
                if ( strTemp != "game" )
                {
                    ReturnResult.Problem = "Unknown game identifier.";
                    return ReturnResult;
                }

                Version = File.ReadUInt32();
                if ( Version != 8U )
                {
                    if ( MessageBox.Show("Game file version is unknown. Continue?", "", MessageBoxButtons.OKCancel, MessageBoxType.Question) == DialogResult.Ok )
                    {
                        ReturnResult.Problem = "Aborted.";
                        return ReturnResult;
                    }
                }

                if ( map.InterfaceOptions == null )
                {
                    map.InterfaceOptions = new InterfaceOptions();
                }

                File.ReadInt32(); //game time
                map.InterfaceOptions.CampaignGameType = File.ReadInt32();
                map.InterfaceOptions.AutoScrollLimits = false;
                map.InterfaceOptions.ScrollMin.X = File.ReadInt32();
                map.InterfaceOptions.ScrollMin.Y = File.ReadInt32();
                map.InterfaceOptions.ScrollMax.X = File.ReadUInt32();
                map.InterfaceOptions.ScrollMax.Y = File.ReadUInt32();
            }
            catch ( Exception ex )
            {
                ReturnResult.Problem = ex.Message;
                logger.ErrorException("Got an exception", ex);
                return ReturnResult;
            }

            ReturnResult.Success = true;
            return ReturnResult;
        }

        protected SimpleResult read_WZ_map(BinaryReader File)
        {
            var returnResult = new SimpleResult();
            returnResult.Success = false;
            returnResult.Problem = "";

            string strTemp = null;
            UInt32 version = 0;
            UInt32 mapWidth = 0;
            UInt32 mapHeight = 0;
            UInt32 uintTemp = 0;
            byte flip = 0;
            var flipX = default(bool);
            var flipZ = default(bool);
            byte rotate = 0;
            byte textureNum = 0;
            var a = 0;
            var x = 0;
            var y = 0;
            var posA = new XYInt();
            var posB = new XYInt();

            try
            {
                strTemp = IOUtil.ReadOldTextOfLength(File, 4);
                if ( strTemp != "map " )
                {
                    returnResult.Problem = "Unknown game.map identifier.";
                    return returnResult;
                }

                version = File.ReadUInt32();
                if ( version != 10U )
                {
                    if ( MessageBox.Show("game.map version is unknown. Continue?", "", MessageBoxButtons.OKCancel, MessageBoxType.Question) == DialogResult.Ok )
                    {
                        returnResult.Problem = "Aborted.";
                        return returnResult;
                    }
                }
                mapWidth = File.ReadUInt32();
                mapHeight = File.ReadUInt32();
                if ( mapWidth < 1U || mapWidth > Constants.MapMaxSize || mapHeight < 1U || mapHeight > Constants.MapMaxSize )
                {
                    returnResult.Problem = "Map size out of range.";
                    return returnResult;
                }

                map.TerrainBlank(new XYInt(Convert.ToInt32(mapWidth), Convert.ToInt32(mapHeight)));

                for ( y = 0; y <= map.Terrain.TileSize.Y - 1; y++ )
                {
                    for ( x = 0; x <= map.Terrain.TileSize.X - 1; x++ )
                    {
                        textureNum = File.ReadByte();
                        map.Terrain.Tiles[x, y].Texture.TextureNum = textureNum;
                        flip = File.ReadByte();
                        map.Terrain.Vertices[x, y].Height = File.ReadByte();
                        //get flipx
                        a = Math.Floor(flip / 128.0D).ToInt();
                        flip -= (byte)(a * 128);
                        flipX = a == 1;
                        //get flipy
                        a = Math.Floor(flip / 64.0D).ToInt();
                        flip -= (byte)(a * 64);
                        flipZ = a == 1;
                        //get rotation
                        a = Math.Floor(flip / 16.0D).ToInt();
                        flip -= (byte)(a * 16);
                        rotate = (byte)a;
                        TileUtil.OldOrientation_To_TileOrientation(rotate, flipX, flipZ, ref map.Terrain.Tiles[x, y].Texture.Orientation);
                        //get tri direction
                        a = Math.Floor(flip / 8.0D).ToInt();
                        flip -= (byte)(a * 8);
                        map.Terrain.Tiles[x, y].Tri = a == 1;
                    }
                }

                if ( version != 2U )
                {
                    uintTemp = File.ReadUInt32();
                    if ( uintTemp != 1 )
                    {
                        returnResult.Problem = "Bad gateway version number.";
                        return returnResult;
                    }

                    uintTemp = File.ReadUInt32();

                    for ( a = 0; a <= (Convert.ToInt32(uintTemp)) - 1; a++ )
                    {
                        posA.X = File.ReadByte();
                        posA.Y = File.ReadByte();
                        posB.X = File.ReadByte();
                        posB.Y = File.ReadByte();
                        if ( map.GatewayCreate(posA, posB) == null )
                        {
                            returnResult.Problem = "Gateway placement error.";
                            return returnResult;
                        }
                    }
                }
            }
            catch ( Exception ex )
            {
                returnResult.Problem = ex.Message;
                logger.ErrorException("Got an exception", ex);
                return returnResult;
            }

            returnResult.Success = true;
            return returnResult;
        }

        protected SimpleResult read_WZ_Features(BinaryReader file, List<WZBJOUnit> wzUnits)
        {
            var returnResult = new SimpleResult();
            returnResult.Success = false;
            returnResult.Problem = "";

            string strTemp = null;
            UInt32 version = 0;
            UInt32 uintTemp = 0;
            var a = 0;
            WZBJOUnit wzbJOUnit = null;

            try
            {
                strTemp = IOUtil.ReadOldTextOfLength(file, 4);
                if ( strTemp != "feat" )
                {
                    returnResult.Problem = "Unknown feat.bjo identifier.";
                    return returnResult;
                }

                version = file.ReadUInt32();
                if ( version != 8U )
                {
                    if ( MessageBox.Show("feat.bjo version is unknown. Continue?", "", MessageBoxButtons.OKCancel, MessageBoxType.Question) == DialogResult.Ok )
                    {
                        returnResult.Problem = "Aborted.";
                        return returnResult;
                    }
                }

                uintTemp = file.ReadUInt32();
                for ( a = 0; a <= (Convert.ToInt32(uintTemp)) - 1; a++ )
                {
                    wzbJOUnit = new WZBJOUnit();
                    wzbJOUnit.ObjectType = UnitType.Feature;
                    wzbJOUnit.Code = IOUtil.ReadOldTextOfLength(file, 40);
                    wzbJOUnit.Code = wzbJOUnit.Code.Substring(0, wzbJOUnit.Code.IndexOf('\0'));
                    wzbJOUnit.ID = file.ReadUInt32();
                    wzbJOUnit.Pos.Horizontal.X = (int)file.ReadUInt32();
                    wzbJOUnit.Pos.Horizontal.Y = (int)(file.ReadUInt32());
                    wzbJOUnit.Pos.Altitude = (int)(file.ReadUInt32());
                    wzbJOUnit.Rotation = file.ReadUInt32();
                    wzbJOUnit.Player = file.ReadUInt32();
                    file.ReadBytes(12);
                    wzUnits.Add(wzbJOUnit);
                }
            }
            catch ( Exception ex )
            {
                returnResult.Problem = ex.Message;
                logger.ErrorException("Got an exception", ex);
                return returnResult;
            }

            returnResult.Success = true;
            return returnResult;
        }

        protected SimpleResult read_WZ_Structures(BinaryReader File, List<WZBJOUnit> WZUnits)
        {
            var returnResult = new SimpleResult();
            returnResult.Success = false;
            returnResult.Problem = "";

            string strTemp = null;
            UInt32 version = 0;
            UInt32 uintTemp = 0;
            var a = 0;
            var wzBJOUnit = default(WZBJOUnit);

            try
            {
                strTemp = IOUtil.ReadOldTextOfLength(File, 4);
                if ( strTemp != "stru" )
                {
                    returnResult.Problem = "Unknown struct.bjo identifier.";
                    return returnResult;
                }

                version = File.ReadUInt32();
                if ( version != 8U )
                {
                    if ( MessageBox.Show("struct.bjo version is unknown. Continue?", "", MessageBoxButtons.OKCancel, MessageBoxType.Question) == DialogResult.Ok )
                    {
                        returnResult.Problem = "Aborted.";
                        return returnResult;
                    }
                }

                uintTemp = File.ReadUInt32();
                for ( a = 0; a <= (Convert.ToInt32(uintTemp)) - 1; a++ )
                {
                    wzBJOUnit = new WZBJOUnit();
                    wzBJOUnit.ObjectType = UnitType.PlayerStructure;
                    wzBJOUnit.Code = IOUtil.ReadOldTextOfLength(File, 40);
                    wzBJOUnit.Code = wzBJOUnit.Code.Substring(0, wzBJOUnit.Code.IndexOf('\0'));
                    wzBJOUnit.ID = File.ReadUInt32();
                    wzBJOUnit.Pos.Horizontal.X = (int)(File.ReadUInt32());
                    wzBJOUnit.Pos.Horizontal.Y = (int)(File.ReadUInt32());
                    wzBJOUnit.Pos.Altitude = (int)(File.ReadUInt32());
                    wzBJOUnit.Rotation = File.ReadUInt32();
                    wzBJOUnit.Player = File.ReadUInt32();
                    File.ReadBytes(56);
                    WZUnits.Add(wzBJOUnit);
                }
            }
            catch ( Exception ex )
            {
                returnResult.Problem = ex.Message;
                logger.ErrorException("Got an exception", ex);
                return returnResult;
            }

            returnResult.Success = true;
            return returnResult;
        }

        protected SimpleResult read_WZ_Droids(BinaryReader File, List<WZBJOUnit> WZUnits)
        {
            var ReturnResult = new SimpleResult();
            ReturnResult.Success = false;
            ReturnResult.Problem = "";

            string strTemp = null;
            UInt32 Version = 0;
            UInt32 uintTemp = 0;
            var A = 0;
            var WZBJOUnit = default(WZBJOUnit);

            try
            {
                strTemp = IOUtil.ReadOldTextOfLength(File, 4);
                if ( strTemp != "dint" )
                {
                    ReturnResult.Problem = "Unknown dinit.bjo identifier.";
                    return ReturnResult;
                }

                Version = File.ReadUInt32();
                if ( Version > 19U )
                {
                    if ( MessageBox.Show("dinit.bjo version is unknown. Continue?", "", MessageBoxButtons.OKCancel, MessageBoxType.Question) == DialogResult.Ok )
                    {
                        ReturnResult.Problem = "Aborted.";
                        return ReturnResult;
                    }
                }

                uintTemp = File.ReadUInt32();
                for ( A = 0; A <= (Convert.ToInt32(uintTemp)) - 1; A++ )
                {
                    WZBJOUnit = new WZBJOUnit();
                    WZBJOUnit.ObjectType = UnitType.PlayerDroid;
                    WZBJOUnit.Code = IOUtil.ReadOldTextOfLength(File, 40);
                    WZBJOUnit.Code = WZBJOUnit.Code.Substring(0, WZBJOUnit.Code.IndexOf('\0'));
                    WZBJOUnit.ID = File.ReadUInt32();
                    WZBJOUnit.Pos.Horizontal.X = (int)(File.ReadUInt32());
                    WZBJOUnit.Pos.Horizontal.Y = (int)(File.ReadUInt32());
                    WZBJOUnit.Pos.Altitude = (int)(File.ReadUInt32());
                    WZBJOUnit.Rotation = File.ReadUInt32();
                    WZBJOUnit.Player = File.ReadUInt32();
                    File.ReadBytes(12);
                    WZUnits.Add(WZBJOUnit);
                }
            }
            catch ( Exception ex )
            {
                ReturnResult.Problem = ex.Message;
                logger.ErrorException("Got an exception", ex);
                return ReturnResult;
            }

            ReturnResult.Success = true;
            return ReturnResult;
        }       
    }
}