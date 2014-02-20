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
using SharpFlame.Core.Parsers;
using SharpFlame.Core.Parsers.Ini;
using SharpFlame.Core.Parsers.Lev;
using SharpFlame.Domain;
using SharpFlame.FileIO;
using SharpFlame.Mapping.Objects;
using SharpFlame.Mapping.Script;
using SharpFlame.Mapping.Tiles;
using SharpFlame.Mapping.Tools;
using SharpFlame.Maths;
using SharpFlame.Util;
using Sprache;

#endregion

namespace SharpFlame.Mapping.IO.Wz
{
    public class Wz
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        protected readonly clsMap map;

        public Wz(clsMap newMap)
        {
            map = newMap;
        }

        public virtual clsResult Load(string path)
        {
            var returnResult = new clsResult(string.Format("Loading WZ from '{0}'.", path), false);
            logger.Info("Loading WZ from '{0}'.", path);
            var subResult = new sResult();

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
                        var myresult = new clsResult(string.Format("Parsing file \"{0}\"", e.FileName), false);
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
                    var Result = new clsResult("feat.bjo", false);
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

                var result = new clsResult("ttypes.ttp", false);
                logger.Info("Loading ttypes.ttp");
                var ttypesEntry = zip[gameFilesPath + "ttypes.ttp"];
                if ( ttypesEntry == null )
                {
                    result.WarningAdd(string.Format("{0}ttypes.ttp file not found", gameFilesPath));
                }
                else
                {
                    using ( Stream s = ttypesEntry.OpenReader() )
                    {
                        var reader = new BinaryReader(s);
                        subResult = read_WZ_TileTypes(reader);
                        reader.Close();
                        if ( !subResult.Success )
                        {
                            result.WarningAdd(subResult.Problem);
                        }
                    }
                }
                returnResult.Add(result);

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
                    var Result = new clsResult("struct.bjo", false);
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
                    var Result = new clsResult("dinit.bjo", false);
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

        public clsResult Save(sWrite_WZ_Args Args)
        {
            var returnResult =
                new clsResult("Compiling to \"{0}\"".Format2(Args.Path), false);
            logger.Info("Compiling to \"{0}\"".Format2(Args.Path));

            try
            {
                switch ( Args.CompileType )
                {
                    case sWrite_WZ_Args.enumCompileType.Multiplayer:
                        if ( Args.Multiplayer == null )
                        {
                            returnResult.ProblemAdd("Multiplayer arguments were not passed.");
                            return returnResult;
                        }
                        if ( Args.Multiplayer.PlayerCount < 2 | Args.Multiplayer.PlayerCount > Constants.PlayerCountMax )
                        {
                            returnResult.ProblemAdd(string.Format("Number of players was below 2 or above {0}.", Constants.PlayerCountMax));
                            return returnResult;
                        }
                        break;
                    case sWrite_WZ_Args.enumCompileType.Campaign:
                        if ( Args.Campaign == null )
                        {
                            returnResult.ProblemAdd("Campaign arguments were not passed.");
                            return returnResult;
                        }
                        break;
                    default:
                        returnResult.ProblemAdd("Unknown compile method.");
                        return returnResult;
                }

                if ( !Args.Overwrite )
                {
                    if ( File.Exists(Args.Path) )
                    {
                        returnResult.ProblemAdd("The selected file already exists.");
                        return returnResult;
                    }
                }

                if ( Args.CompileType == sWrite_WZ_Args.enumCompileType.Multiplayer )
                {
                    if ( !Args.Overwrite )
                    {
                        if ( File.Exists(Args.Path) )
                        {
                            returnResult.ProblemAdd(string.Format("A file already exists at: {0}", Args.Path));
                            return returnResult;
                        }
                    }

                    try
                    {
                        using ( var zip = new ZipOutputStream(Args.Path) )
                        {
                            // Set encoding
                            zip.AlternateEncoding = Encoding.GetEncoding("UTF-8");
                            zip.AlternateEncodingUsage = ZipOption.Always;

                            // Set compression
                            zip.CompressionLevel = CompressionLevel.BestCompression;

                            // .xplayers.lev
                            var zipPath = string.Format("{0}c-{1}.xplayers.lev", Args.Multiplayer.PlayerCount, Args.MapName);
                            if ( Args.CompileType == sWrite_WZ_Args.enumCompileType.Multiplayer )
                            {
                                zip.PutNextEntry(zipPath);
                                returnResult.Add(Serialize_WZ_LEV(zip, Args.Multiplayer.PlayerCount,
                                    Args.Multiplayer.AuthorName, Args.Multiplayer.License,
                                    Args.MapName));
                            }

                            var path = string.Format("multiplay/maps/{0}c-{1}", Args.Multiplayer.PlayerCount, Args.MapName);
                            zip.PutNextEntry(string.Format("{0}.gam", path));
                            returnResult.Add(Serialize_WZ_Gam(zip, 0U,
                                Args.CompileType, Args.ScrollMin, Args.ScrollMax));


                            zip.PutNextEntry(string.Format("{0}/struct.ini", path));
                            var iniStruct = new IniWriter(zip);
                            returnResult.Add(Serialize_WZ_StructuresINI(iniStruct, Args.Multiplayer.PlayerCount));
                            iniStruct.Flush();

                            zip.PutNextEntry(string.Format("{0}/droid.ini", path));
                            var iniDroid = new IniWriter(zip);
                            returnResult.Add(Serialize_WZ_DroidsINI(iniDroid, Args.Multiplayer.PlayerCount));
                            iniDroid.Flush();

                            zip.PutNextEntry(string.Format("{0}/labels.ini", path));
                            var iniLabels = new IniWriter(zip);
                            returnResult.Add(Serialize_WZ_LabelsINI(iniLabels, Args.Multiplayer.PlayerCount));
                            iniLabels.Flush();

                            zip.PutNextEntry(string.Format("{0}/feature.ini", path));
                            var iniFeature = new IniWriter(zip);
                            returnResult.Add(Serialize_WZ_FeaturesINI(iniFeature));
                            iniFeature.Flush();

                            zip.PutNextEntry(string.Format("{0}/game.map", path));
                            returnResult.Add(Serialize_WZ_Map(zip));

                            zip.PutNextEntry(string.Format("{0}/ttypes.ttp", path));
                            var ttpSaver = new TTP.TTP(map);
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
                if ( Args.CompileType == sWrite_WZ_Args.enumCompileType.Campaign )
                {
                    var CampDirectory = PathUtil.EndWithPathSeperator(Args.Path);

                    if ( !Directory.Exists(CampDirectory) )
                    {
                        returnResult.ProblemAdd(string.Format("Directory {0} does not exist.", CampDirectory));
                        return returnResult;
                    }

                    var filePath = string.Format("{0}{1}.gam", CampDirectory, Args.MapName);
                    using ( var file = File.Open(filePath, FileMode.Open | FileMode.CreateNew) )
                    {
                        returnResult.Add(Serialize_WZ_Gam(file, Args.Campaign.GAMType,
                            Args.CompileType, Args.ScrollMin, Args.ScrollMax));
                    }

                    CampDirectory += Args.MapName + Convert.ToString(App.PlatformPathSeparator);
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
                    var ttpSaver = new TTP.TTP(map);
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

        protected clsResult createWZObjects(List<WZBJOUnit> bjoUnits, List<IniStructure> iniStructures, List<IniDroid> iniDroids, List<IniFeature> iniFeatures)
        {
            var ReturnResult = new clsResult("Creating objects", false);
            logger.Info("Creating objects");

            var newUnit = default(clsUnit);
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
                newUnit = new clsUnit();
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
                newUnit.Rotation = (int)(Math.Min(bjoUnit.Rotation, 359U));
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
            var newModule = default(clsUnit);

            var factoryModule = App.ObjectData.FindFirstStructureType(StructureTypeBase.enumStructureType.FactoryModule);
            var researchModule = App.ObjectData.FindFirstStructureType(StructureTypeBase.enumStructureType.ResearchModule);
            var powerModule = App.ObjectData.FindFirstStructureType(StructureTypeBase.enumStructureType.PowerModule);

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
                        newUnit = new clsUnit();
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
                        newUnit.Rotation = Convert.ToInt32(iniStructure.Rotation.Direction * 360.0D / Constants.INIRotationMax);
                        if ( newUnit.Rotation == 360 )
                        {
                            newUnit.Rotation = 0;
                        }
                        if ( iniStructure.HealthPercent >= 0 )
                        {
                            newUnit.Health = MathUtil.Clamp_dbl(iniStructure.HealthPercent / 100.0D, 0.01D, 1.0D);
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
                            case StructureTypeBase.enumStructureType.Factory:
                                moduleLimit = 2;
                                moduleTypeBase = factoryModule;
                                break;
                            case StructureTypeBase.enumStructureType.VTOLFactory:
                                moduleLimit = 2;
                                moduleTypeBase = factoryModule;
                                break;
                            case StructureTypeBase.enumStructureType.PowerGenerator:
                                moduleLimit = 1;
                                moduleTypeBase = powerModule;
                                break;
                            case StructureTypeBase.enumStructureType.Research:
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
                                newModule = new clsUnit();
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
                        newUnit = new clsUnit();
                        newUnit.TypeBase = featureTypeBase;
                        newUnit.UnitGroup = map.ScavengerUnitGroup;
                        newUnit.Pos = new WorldPos(iniFeature.Pos, iniFeature.Pos.Z);
                        newUnit.Rotation = Convert.ToInt32(iniFeature.Rotation.Direction * 360.0D / Constants.INIRotationMax);
                        if ( newUnit.Rotation == 360 )
                        {
                            newUnit.Rotation = 0;
                        }
                        if ( iniFeature.HealthPercent >= 0 )
                        {
                            newUnit.Health = MathUtil.Clamp_dbl(iniFeature.HealthPercent / 100.0D, 0.01D, 1.0D);
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
                        if ( !droidType.SetDroidType((enumDroidType)(iniDroid.DroidType)) )
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
                        newUnit = new clsUnit();
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
                        newUnit.Rotation = Convert.ToInt32(iniDroid.Rotation.Direction * 360.0D / Constants.INIRotationMax);
                        if ( newUnit.Rotation == 360 )
                        {
                            newUnit.Rotation = 0;
                        }
                        if ( iniDroid.HealthPercent >= 0 )
                        {
                            newUnit.Health = MathUtil.Clamp_dbl(iniDroid.HealthPercent / 100.0D, 0.01D, 1.0D);
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

        protected clsResult read_INI_Features(string iniText, List<IniFeature> resultData)
        {
            var resultObject = new clsResult("Reading feature.ini.", false);
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

        protected clsResult read_INI_Droids(string iniText, List<IniDroid> resultData)
        {
            var resultObject = new clsResult("Reading droids.ini.", false);

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

        protected clsResult read_INI_Structures(string iniText, List<IniStructure> resultData)
        {
            var resultObject = new clsResult("Reading struct.ini.", false);
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

        protected clsResult read_INI_Labels(string iniText)
        {
            var resultObject = new clsResult("Reading labels", false);
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

        protected sResult read_WZ_gam(BinaryReader File)
        {
            var ReturnResult = new sResult();
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
                    if ( MessageBox.Show("Game file version is unknown. Continue?", "", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK )
                    {
                        ReturnResult.Problem = "Aborted.";
                        return ReturnResult;
                    }
                }

                if ( map.InterfaceOptions == null )
                {
                    map.InterfaceOptions = new clsInterfaceOptions();
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

        protected sResult read_WZ_map(BinaryReader File)
        {
            var returnResult = new sResult();
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
                    if ( MessageBox.Show("game.map version is unknown. Continue?", "", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK )
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
                        a = (int)((flip / 128.0D));
                        flip -= (byte)(a * 128);
                        flipX = a == 1;
                        //get flipy
                        a = (int)((flip / 64.0D));
                        flip -= (byte)(a * 64);
                        flipZ = a == 1;
                        //get rotation
                        a = (int)((flip / 16.0D));
                        flip -= (byte)(a * 16);
                        rotate = (byte)a;
                        TileUtil.OldOrientation_To_TileOrientation(rotate, flipX, flipZ, ref map.Terrain.Tiles[x, y].Texture.Orientation);
                        //get tri direction
                        a = (int)((flip / 8.0D));
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

        protected sResult read_WZ_Features(BinaryReader file, List<WZBJOUnit> wzUnits)
        {
            var returnResult = new sResult();
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
                    if ( MessageBox.Show("feat.bjo version is unknown. Continue?", "", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK )
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
                    wzbJOUnit.Pos.Horizontal.X = (int)(file.ReadUInt32());
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

        protected sResult read_WZ_TileTypes(BinaryReader File)
        {
            var ReturnResult = new sResult();
            ReturnResult.Success = false;
            ReturnResult.Problem = "";

            string strTemp = null;
            UInt32 Version = 0;
            UInt32 uintTemp = 0;
            UInt16 ushortTemp = 0;
            var A = 0;

            try
            {
                strTemp = IOUtil.ReadOldTextOfLength(File, 4);
                if ( strTemp != "ttyp" )
                {
                    ReturnResult.Problem = "Unknown ttypes.ttp identifier.";
                    return ReturnResult;
                }

                Version = File.ReadUInt32();
                if ( Version != 8U )
                {
                    //Load_WZ.Problem = "Unknown ttypes.ttp version."
                    //Exit Function
                    if ( MessageBox.Show("ttypes.ttp version is unknown. Continue?", "", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK )
                    {
                        ReturnResult.Problem = "Aborted.";
                        return ReturnResult;
                    }
                }

                uintTemp = File.ReadUInt32();

                if ( map.Tileset != null )
                {
                    for ( A = 0; A <= Math.Min(Convert.ToInt32(uintTemp), map.Tileset.TileCount) - 1; A++ )
                    {
                        ushortTemp = File.ReadUInt16();
                        if ( ushortTemp > 11U )
                        {
                            ReturnResult.Problem = "Unknown tile type.";
                            return ReturnResult;
                        }
                        map.Tile_TypeNum[A] = (byte)ushortTemp;
                    }
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

        protected sResult read_WZ_Structures(BinaryReader File, List<WZBJOUnit> WZUnits)
        {
            var returnResult = new sResult();
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
                    if ( MessageBox.Show("struct.bjo version is unknown. Continue?", "", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK )
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

        protected sResult read_WZ_Droids(BinaryReader File, List<WZBJOUnit> WZUnits)
        {
            var ReturnResult = new sResult();
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
                    if ( MessageBox.Show("dinit.bjo version is unknown. Continue?", "", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK )
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

        private clsResult Serialize_WZ_Gam(Stream stream, UInt32 gamType, sWrite_WZ_Args.enumCompileType compileType, XYInt scrollMin, sXY_uint scrollMax)
        {
            var returnResult = new clsResult("Serializing .gam", false);
            logger.Info("Serializing .gam");

            var fileGAM = new BinaryWriter(stream, App.ASCIIEncoding);

            IOUtil.WriteText(fileGAM, false, "game");
            fileGAM.Write(8U);
            fileGAM.Write(0U); //Time
            if ( compileType == sWrite_WZ_Args.enumCompileType.Multiplayer )
            {
                fileGAM.Write(0U);
            }
            else if ( compileType == sWrite_WZ_Args.enumCompileType.Campaign )
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
                fileMAP.Write((byte)(MathUtil.Clamp_int(gateway.PosA.X, 0, 255)));
                fileMAP.Write((byte)(MathUtil.Clamp_int(gateway.PosA.Y, 0, 255)));
                fileMAP.Write((byte)(MathUtil.Clamp_int(gateway.PosB.X, 0, 255)));
                fileMAP.Write((byte)(MathUtil.Clamp_int(gateway.PosB.Y, 0, 255)));
            }
            fileMAP.Flush();

            return returnResult;
        }

        private clsResult Serialize_WZ_LEV(Stream stream, int playercount, string authorname, string license, string mapName)
        {
            var returnResult = new clsResult("Serializing .lev", false);
            logger.Info("Serializing .lev");

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