using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using NLog;
using SharpFlame.Collections;
using SharpFlame.Core.Domain;
using SharpFlame.Core.Parsers;
using SharpFlame.Core.Parsers.Ini;
using SharpFlame.Core.Parsers.Lev;
using SharpFlame.Domain;
using SharpFlame.FileIO;
using SharpFlame.FileIO.Ini;
using SharpFlame.Mapping.Objects;
using SharpFlame.Mapping.Script;
using SharpFlame.Mapping.Tiles;
using SharpFlame.Mapping.Tools;
using SharpFlame.Mapping.Wz;
using SharpFlame.Maths;
using SharpFlame.Util;
using Sprache;
using Ionic.Zip;

namespace SharpFlame.Mapping
{
    public partial class clsMap
    {
        public clsResult Load_WZ(string path)
        {
            var returnResult = new clsResult (string.Format ("Loading WZ from '{0}'.", path), false);
            logger.Info ("Loading WZ from '{0}'.", path);
            sResult subResult = new sResult ();         

            ZipSplitPath splitPath;
            var mapLoadName = "";

            using (var zip = ZipFile.Read(path)) {
                foreach (ZipEntry e in zip) {
                    if (e.IsDirectory) {
                        continue;
                    }                  

                    splitPath = new ZipSplitPath (e.FileName);
                    logger.Debug ("Found file \"{0}\".", e.FileName);
                    // Find the maps .lev
                    if (splitPath.FileExtension != "lev" || splitPath.PartCount != 1) {
                        continue;
                    }

                    // Buggy file > 1MB
                    if (e.UncompressedSize > 1 * 1024 * 1024) {
                        returnResult.ProblemAdd ("lev file is too large.");
                        return returnResult;
                    }

                    using (var s = e.OpenReader()) {
                        var myresult = new clsResult (string.Format ("Parsing file \"{0}\"", e.FileName), false);
                        logger.Info ("Parsing file \"{0}\"", e.FileName);

                        try {
                            var r = new StreamReader (s);
                            var text = r.ReadToEnd ();
                            var levFile = LevGrammar.Lev.Parse(text);

                            if (levFile.Levels.Count < 1) {
                                myresult.ProblemAdd ("No maps found in file.");
                                returnResult.Add (myresult);
                                return returnResult;
                            }

                            // Group games by the Game key.
                            var groupGames = levFile.Levels.GroupBy(level => level.Game);

                            // Load default map if only one Game file is found.
                            if (groupGames.Count() == 1) {
                                var level = groupGames.First().First(); //first group, first level

                                mapLoadName = level.Game;

                                switch (level.Dataset.Substring (level.Dataset.Length - 1, 1)) {
                                case "1":
                                    Tileset = App.Tileset_Arizona;
                                    break;
                                case "2":
                                    Tileset = App.Tileset_Urban;
                                    break;
                                case "3":
                                    Tileset = App.Tileset_Urban;
                                    break;  
                                default:
                                    myresult.ProblemAdd ("Unknown tileset.");
                                    returnResult.Add (myresult);
                                    return returnResult;
                                }
                            } else {
                                //prompt user for which of the entries to load
                                frmWZLoad.clsOutput selectToLoadResult = new frmWZLoad.clsOutput ();

                                var names = groupGames
                                    .Select(gameGroup => gameGroup.First().Name)
                                    .ToArray();

                                frmWZLoad selectToLoadForm = new frmWZLoad (names, selectToLoadResult,
                                                                            "Select a map from " + new sSplitPath (path).FileTitle);
                                selectToLoadForm.ShowDialog ();
                                if (selectToLoadResult.Result < 0) {
                                    returnResult.ProblemAdd ("No map selected.");
                                    return returnResult;
                                }

                                var level = groupGames.ElementAt(selectToLoadResult.Result).First();
                                
                                mapLoadName = level.Game;

                                switch (level.Dataset.Substring (level.Dataset.Length - 1, 1)) {
                                    case "1":
                                    Tileset = App.Tileset_Arizona;
                                    break;
                                    case "2":
                                    Tileset = App.Tileset_Urban;
                                    break;
                                    case "3":
                                    Tileset = App.Tileset_Urban;
                                    break;  
                                    default:
                                        myresult.ProblemAdd ("Unknown tileset.");
                                        returnResult.Add (myresult);
                                        return returnResult;
                                }
                            }                                                   
                        } catch (Exception ex) {
                            myresult.ProblemAdd (string.Format ("Got an exception while parsing the .lev file: {0}", ex), false);
                            returnResult.Add (myresult);
                            logger.ErrorException ("Got an exception while parsing the .lev file", ex);
                            Debugger.Break ();
                        }                       
                    }
                }
                                             
                TileType_Reset ();
                SetPainterToDefaults ();

                // mapLoadName is now multiplay/maps/<mapname>.gam (thats "game" from the .lev file
                ZipSplitPath gameSplitPath = new ZipSplitPath (mapLoadName);
                string gameFilesPath = gameSplitPath.FilePath + gameSplitPath.FileTitleWithoutExtension + "/";

                var gameZipEntry = zip [mapLoadName]; 
                if (gameZipEntry == null) {
                    returnResult.ProblemAdd (string.Format("Game file \"{0}\" not found.", mapLoadName), false);
                    logger.Error ("Game file \"{0}\" not found.", mapLoadName);
                    return returnResult;
                }
                using (Stream s = gameZipEntry.OpenReader()) {              
                    BinaryReader reader = new BinaryReader (s);
                    subResult = Read_WZ_gam (reader);
                    reader.Close ();
                    if (!subResult.Success) {
                        returnResult.ProblemAdd (subResult.Problem);
                        return returnResult;
                    }
                }


                var gameMapZipEntry = zip [gameFilesPath + "game.map"];
                if (gameMapZipEntry == null) {
                    returnResult.ProblemAdd (string.Format ("{0}game.map file not found", gameFilesPath));
                    return returnResult;
                }
                using (Stream s = gameMapZipEntry.OpenReader()) {              
                    BinaryReader reader = new BinaryReader (s);
                    subResult = Read_WZ_map (reader);
                    reader.Close ();
                    if (!subResult.Success) {
                        returnResult.ProblemAdd (subResult.Problem);
                        return returnResult;
                    }
                }

                SimpleClassList<clsWZBJOUnit> BJOUnits = new SimpleClassList<clsWZBJOUnit> ();

                IniFeatures iniFeatures = null;
                var featureIniZipEntry = zip [gameFilesPath + "feature.ini"];
                if (featureIniZipEntry != null) {
                    iniFeatures = new IniFeatures ();
                    using (Stream s = featureIniZipEntry.OpenReader()) {
                        var reader = new StreamReader (s);
                        var text = reader.ReadToEnd ();
                        reader.Close ();
                        returnResult.Add (read_INI_Features (text, ref iniFeatures));
                    }
                }
                              
                if (iniFeatures == null) {
                    clsResult Result = new clsResult ("feat.bjo", false);
                    logger.Info ("Loading feat.bjo");

                    var featBJOZipEntry = zip [gameFilesPath + "feat.bjo"];
                    if (featBJOZipEntry == null) {
                        Result.WarningAdd (string.Format ("{0}feat.bjo / feature.ini file not found", gameFilesPath));
                    } else {
                        using (Stream s = featBJOZipEntry.OpenReader()) {
                            BinaryReader reader = new BinaryReader (s);
                            subResult = Read_WZ_Features (reader, BJOUnits);
                            reader.Close ();
                            if (!subResult.Success) {
                                Result.WarningAdd (subResult.Problem);
                            }
                        }
                    }
                    returnResult.Add (Result);
                }

                clsResult result = new clsResult ("ttypes.ttp", false);
                logger.Info ("Loading ttypes.ttp");
                var ttypesEntry = zip [gameFilesPath + "ttypes.ttp"];
                if (ttypesEntry == null) {
                    result.WarningAdd (string.Format ("{0}ttypes.ttp file not found", gameFilesPath));
                } else {
                    using (Stream s = ttypesEntry.OpenReader()) {
                        BinaryReader reader = new BinaryReader (s);
                        subResult = Read_WZ_TileTypes (reader);
                        reader.Close ();
                        if (!subResult.Success) {
                            result.WarningAdd (subResult.Problem);
                        }
                    }
                }
                returnResult.Add (result);

                IniStructures iniStructures = null;
                var structIniEntry = zip [gameFilesPath + "struct.ini"];
                if (structIniEntry != null) {
                    iniStructures = new IniStructures ();
                    using (Stream s = structIniEntry.OpenReader()) {
                        var reader = new StreamReader (s);
                        var text = reader.ReadToEnd ();
                        reader.Close ();
                        returnResult.Add (read_INI_Structures (text, ref iniStructures));
                    }
                }

                if (iniStructures == null) {
                    clsResult Result = new clsResult ("struct.bjo", false);
                    logger.Info ("Loading struct.bjo");
                    var structBjoEntry = zip [gameFilesPath + "struct.bjo"];
                    if (structBjoEntry == null) {
                        Result.WarningAdd (string.Format ("{0}struct.bjo / struct.ini file not found", gameFilesPath));
                    } else {
                        using (Stream s = structBjoEntry.OpenReader()) {
                            BinaryReader reader = new BinaryReader (s);
                            subResult = Read_WZ_Structures (reader, BJOUnits);
                            reader.Close ();
                            if (!subResult.Success) {
                                Result.WarningAdd (subResult.Problem);
                            }
                        }
                    }
                    returnResult.Add (Result);
                }

                IniDroids iniDroids = null;              
                if (structIniEntry != null) {
                    var droidIniEntry = zip [gameFilesPath + "droid.ini"];
                    if (droidIniEntry != null) {
                        iniDroids = new IniDroids ();
                        using (Stream s = droidIniEntry.OpenReader()) {
                            var reader = new StreamReader (s);
                            var text = reader.ReadToEnd ();
                            reader.Close ();
                            returnResult.Add (read_INI_Droids (text, ref iniDroids));
                        }
                    }
                }

                if (iniDroids == null) {
                    clsResult Result = new clsResult ("dinit.bjo", false);
                    logger.Info ("Loading dinit.bjo");
                    var diniBjoEntry = zip [gameFilesPath + "dinit.bjo"];
                    if (diniBjoEntry == null) {
                        Result.WarningAdd (string.Format ("{0}dinit.bjo / droid.ini file not found", gameFilesPath));
                    } else {
                        using (Stream s = diniBjoEntry.OpenReader()) {
                            BinaryReader reader = new BinaryReader (s);
                            subResult = Read_WZ_Droids (reader, BJOUnits);
                            reader.Close ();
                            if (!subResult.Success) {
                                Result.WarningAdd (subResult.Problem);
                            }
                        }
                    }
                    returnResult.Add (Result);
                }

                sCreateWZObjectsArgs CreateObjectsArgs = new sCreateWZObjectsArgs ();
                CreateObjectsArgs.BJOUnits = BJOUnits;
                CreateObjectsArgs.INIStructures = iniStructures;
                CreateObjectsArgs.INIDroids = iniDroids;
                CreateObjectsArgs.INIFeatures = iniFeatures;
                returnResult.Add (CreateWZObjects (CreateObjectsArgs));

                //objects are modified by this and must already exist
                var labelsIniEntry = zip [gameFilesPath + "labels.ini"];
                if (labelsIniEntry != null) {
                    using (Stream s = labelsIniEntry.OpenReader()) {
                        clsResult Result = new clsResult ("labels.ini", false);
                        logger.Info ("Loading labels.ini");
                        var LabelsINI = new SharpFlame.FileIO.Ini.IniReader ();
                        StreamReader reader = new StreamReader (s);
                        Result.Take (LabelsINI.ReadFile (reader));
                        reader.Close ();
                        Result.Take (Read_WZ_Labels (LabelsINI, false));
                        returnResult.Add (Result);
                    }
                }                       
            }

            return returnResult;
        }

        public clsResult Load_Game(string Path)
        {
            clsResult returnResult =
                new clsResult("Loading game file from \"{0}\"".Format2(Path), false);
            logger.Info ("Loading game file from \"{0}\"", Path);
            sResult SubResult = new sResult();

            Tileset = null;

            TileType_Reset();
            SetPainterToDefaults();

            sSplitPath GameSplitPath = new sSplitPath(Path);
            string GameFilesPath = GameSplitPath.FilePath + GameSplitPath.FileTitleWithoutExtension + Convert.ToString(App.PlatformPathSeparator);
            string MapDirectory = "";
            FileStream File = null;

            SubResult = IOUtil.TryOpenFileStream(Path, ref File);
            if ( !SubResult.Success )
            {
                returnResult.ProblemAdd("Game file not found: " + SubResult.Problem);
                return returnResult;
            }
            else
            {
                BinaryReader Map_Reader = new BinaryReader(File);
                SubResult = Read_WZ_gam(Map_Reader);
                Map_Reader.Close();

                if ( !SubResult.Success )
                {
                    returnResult.ProblemAdd(SubResult.Problem);
                    return returnResult;
                }
            }

            SubResult = IOUtil.TryOpenFileStream(GameFilesPath + "game.map", ref File);
            if ( !SubResult.Success )
            {
                if (MessageBox.Show("game.map file not found at \"{0}\"\n" +
                    "Do you want to select another directory to load the underlying map from?".Format2(GameFilesPath), 
                                    "", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                {
                    returnResult.ProblemAdd("Aborted.");
                    return returnResult;
                }
                FolderBrowserDialog DirectorySelect = new FolderBrowserDialog();
                DirectorySelect.SelectedPath = GameFilesPath;
                if ( DirectorySelect.ShowDialog() != DialogResult.OK )
                {
                    returnResult.ProblemAdd("Aborted.");
                    return returnResult;
                }
                MapDirectory = DirectorySelect.SelectedPath + Convert.ToString(App.PlatformPathSeparator);

                SubResult = IOUtil.TryOpenFileStream(MapDirectory + "game.map", ref File);
                if ( !SubResult.Success )
                {
                    returnResult.ProblemAdd("game.map file not found: " + SubResult.Problem);
                    return returnResult;
                }
            }
            else
            {
                MapDirectory = GameFilesPath;
            }

            BinaryReader Map_ReaderB = new BinaryReader(File);
            SubResult = Read_WZ_map(Map_ReaderB);
            Map_ReaderB.Close();

            if ( !SubResult.Success )
            {
                returnResult.ProblemAdd(SubResult.Problem);
                return returnResult;
            }

            SimpleClassList<clsWZBJOUnit> BJOUnits = new SimpleClassList<clsWZBJOUnit>();

            IniFeatures iniFeatures = null;
            SubResult = IOUtil.TryOpenFileStream(GameFilesPath + "feature.ini", ref File);
            if ( SubResult.Success )
            {
                iniFeatures = new IniFeatures ();
                using (File) {
                    var reader = new StreamReader (File);
                    var text = reader.ReadToEnd ();
                    reader.Close ();
                    returnResult.Add (read_INI_Features (text, ref iniFeatures));
                }
            }

            if ( iniFeatures == null )
            {
                clsResult Result = new clsResult("feat.bjo", false);
                logger.Info ("Loading feat.bjo");
                SubResult = IOUtil.TryOpenFileStream(GameFilesPath + "feat.bjo", ref File);
                if ( !SubResult.Success )
                {
                    Result.WarningAdd("file not found");
                }
                else
                {
                    BinaryReader Features_Reader = new BinaryReader(File);
                    SubResult = Read_WZ_Features(Features_Reader, BJOUnits);
                    Features_Reader.Close();
                    if ( !SubResult.Success )
                    {
                        Result.WarningAdd(SubResult.Problem);
                    }
                }
                returnResult.Add(Result);
            }

            if ( true )
            {
                clsResult Result = new clsResult("ttypes.ttp", false);
                logger.Info ("Loading ttypes.ttp");
                SubResult = IOUtil.TryOpenFileStream(MapDirectory + "ttypes.ttp", ref File);
                if ( !SubResult.Success )
                {
                    Result.WarningAdd("file not found");
                }
                else
                {
                    BinaryReader TileTypes_Reader = new BinaryReader(File);
                    SubResult = Read_WZ_TileTypes(TileTypes_Reader);
                    TileTypes_Reader.Close();
                    if ( !SubResult.Success )
                    {
                        Result.WarningAdd(SubResult.Problem);
                    }
                }
                returnResult.Add(Result);
            }

            IniStructures iniStructures = null;
            SubResult = IOUtil.TryOpenFileStream(GameFilesPath + "struct.ini", ref File);
            if (SubResult.Success) {
                iniStructures = new IniStructures ();
                using (File) {
                    var reader = new StreamReader (File);
                    var text = reader.ReadToEnd ();
                    reader.Close ();
                    returnResult.Add (read_INI_Structures (text, ref iniStructures));
                }
            }

            if ( iniStructures == null )
            {
                clsResult Result = new clsResult("struct.bjo", false);
                logger.Info ("Loading struct.bjo");
                SubResult = IOUtil.TryOpenFileStream(GameFilesPath + "struct.bjo", ref File);
                if ( !SubResult.Success )
                {
                    Result.WarningAdd("struct.bjo file not found.");
                }
                else
                {
                    BinaryReader Structures_Reader = new BinaryReader(File);
                    SubResult = Read_WZ_Structures(Structures_Reader, BJOUnits);
                    Structures_Reader.Close();
                    if ( !SubResult.Success )
                    {
                        Result.WarningAdd(SubResult.Problem);
                    }
                }
                returnResult.Add(Result);
            }

            IniDroids iniDroids = null;

            SubResult = IOUtil.TryOpenFileStream(GameFilesPath + "droid.ini", ref File);
            if ( SubResult.Success )
            {
                iniDroids = new IniDroids ();
                using (File) {
                    var reader = new StreamReader (File);
                    var text = reader.ReadToEnd ();
                    reader.Close ();
                    returnResult.Add (read_INI_Droids (text, ref iniDroids));
                }
            }

            if ( iniStructures == null )
            {
                clsResult Result = new clsResult("dinit.bjo", false);
                logger.Info ("Loading dinit.bjo");
                SubResult = IOUtil.TryOpenFileStream(GameFilesPath + "dinit.bjo", ref File);
                if ( !SubResult.Success )
                {
                    Result.WarningAdd("dinit.bjo file not found.");
                }
                else
                {
                    BinaryReader Droids_Reader = new BinaryReader(File);
                    SubResult = Read_WZ_Droids(Droids_Reader, BJOUnits);
                    Droids_Reader.Close();
                    if ( !SubResult.Success )
                    {
                        Result.WarningAdd(SubResult.Problem);
                    }
                }
                returnResult.Add(Result);
            }

            sCreateWZObjectsArgs CreateObjectsArgs = new sCreateWZObjectsArgs();
            CreateObjectsArgs.BJOUnits = BJOUnits;
            CreateObjectsArgs.INIStructures = iniStructures;
            CreateObjectsArgs.INIDroids = iniDroids;
            CreateObjectsArgs.INIFeatures = iniFeatures;
            returnResult.Add(CreateWZObjects(CreateObjectsArgs));

            //map objects are modified by this and must already exist
            SubResult = IOUtil.TryOpenFileStream(GameFilesPath + "labels.ini", ref File);
            if ( !SubResult.Success )
            {
            }
            else
            {
                clsResult Result = new clsResult("labels.ini", false);
                logger.Info ("Loading labels.ini");
                var LabelsINI = new SharpFlame.FileIO.Ini.IniReader();
                StreamReader LabelsINI_Reader = new StreamReader(File);
                Result.Take(LabelsINI.ReadFile(LabelsINI_Reader));
                LabelsINI_Reader.Close();
                Result.Take(Read_WZ_Labels(LabelsINI, false));
                returnResult.Add(Result);
            }

            return returnResult;
        }

        public clsResult CreateWZObjects(sCreateWZObjectsArgs Args)
        {
            clsResult ReturnResult = new clsResult("Creating objects", false);
            logger.Info ("Creating objects");
            clsUnit NewUnit = default(clsUnit);
            UInt32 AvailableID = 0;
            SimpleClassList<clsWZBJOUnit> BJOUnits = Args.BJOUnits;
            IniStructures INIStructures = Args.INIStructures;
            IniDroids INIDroids = Args.INIDroids;
            IniFeatures INIFeatures = Args.INIFeatures;
            clsUnitAdd UnitAdd = new clsUnitAdd();
            int A = 0;
            int B = 0;
            clsWZBJOUnit BJOUnit = default(clsWZBJOUnit);

            UnitAdd.Map = this;

            AvailableID = 1U;
            foreach ( clsWZBJOUnit tempLoopVar_BJOUnit in BJOUnits )
            {
                BJOUnit = tempLoopVar_BJOUnit;
                if ( BJOUnit.ID >= AvailableID )
                {
                    AvailableID = BJOUnit.ID + 1U;
                }
            }
            if ( INIStructures != null )
            {
                var structMaxId = INIStructures.Structures.Max(w => w.ID)+10;
                if (structMaxId > AvailableID) {
                    AvailableID = structMaxId;
                }
            }
            if ( INIFeatures != null )
            {
                var featuresMaxId = INIFeatures.Features.Max (w => w.ID) + 10;
                if (featuresMaxId > AvailableID) {
                    AvailableID = featuresMaxId;
                }
            }
            if ( INIDroids != null )
            {
                var droidsMaxId = INIDroids.Droids.Max (w => w.ID) + 10;
                if (droidsMaxId > AvailableID) {
                    AvailableID += droidsMaxId;
                }
            }

            foreach ( clsWZBJOUnit tempLoopVar_BJOUnit in BJOUnits )
            {
                BJOUnit = tempLoopVar_BJOUnit;
                NewUnit = new clsUnit();
                NewUnit.ID = BJOUnit.ID;
                NewUnit.TypeBase = App.ObjectData.FindOrCreateUnitType(BJOUnit.Code, BJOUnit.ObjectType, -1);
                if ( NewUnit.TypeBase == null )
                {
                    ReturnResult.ProblemAdd("Unable to create object type.");
                    return ReturnResult;
                }
                if ( BJOUnit.Player >= Constants.PlayerCountMax )
                {
                    NewUnit.UnitGroup = ScavengerUnitGroup;
                }
                else
                {
                    NewUnit.UnitGroup = UnitGroups[Convert.ToInt32(BJOUnit.Player)];
                }
                NewUnit.Pos = BJOUnit.Pos;
                NewUnit.Rotation = (int)(Math.Min(BJOUnit.Rotation, 359U));
                if ( BJOUnit.ID == 0U )
                {
                    BJOUnit.ID = AvailableID;
                    App.ZeroIDWarning(NewUnit, BJOUnit.ID, ReturnResult);
                }
                UnitAdd.NewUnit = NewUnit;
                UnitAdd.ID = BJOUnit.ID;
                UnitAdd.Perform();
                App.ErrorIDChange(BJOUnit.ID, NewUnit, "CreateWZObjects");
                if ( AvailableID == BJOUnit.ID )
                {
                    AvailableID = NewUnit.ID + 1U;
                }
            }

            StructureTypeBase structureTypeBase = default(StructureTypeBase);
            DroidDesign DroidType = default(DroidDesign);
            FeatureTypeBase featureTypeBase = default(FeatureTypeBase);
            DroidDesign.sLoadPartsArgs LoadPartsArgs = new DroidDesign.sLoadPartsArgs();
            UnitTypeBase unitTypeBase = null;
            int ErrorCount = 0;
            int UnknownDroidComponentCount = 0;
            int UnknownDroidTypeCount = 0;
            int DroidBadPositionCount = 0;
            int StructureBadPositionCount = 0;
            int StructureBadModulesCount = 0;
            int FeatureBadPositionCount = 0;
            int ModuleLimit = 0;
            XYInt ZeroPos = new XYInt(0, 0);
            StructureTypeBase moduleTypeBase = default(StructureTypeBase);
            clsUnit NewModule = default(clsUnit);

            StructureTypeBase FactoryModule = App.ObjectData.FindFirstStructureType(StructureTypeBase.enumStructureType.FactoryModule);
            StructureTypeBase ResearchModule = App.ObjectData.FindFirstStructureType(StructureTypeBase.enumStructureType.ResearchModule);
            StructureTypeBase PowerModule = App.ObjectData.FindFirstStructureType(StructureTypeBase.enumStructureType.PowerModule);

            if ( FactoryModule == null )
            {
                ReturnResult.WarningAdd("No factory module loaded.");
            }
            if ( ResearchModule == null )
            {
                ReturnResult.WarningAdd("No research module loaded.");
            }
            if ( PowerModule == null )
            {
                ReturnResult.WarningAdd("No power module loaded.");
            }

            if ( INIStructures != null )
            {
                for ( A = 0; A <= INIStructures.StructureCount - 1; A++ )
                {
                    if ( INIStructures.Structures[A].Pos == null )
                    {
                        logger.Debug ("{0} pos was null", INIStructures.Structures[A].Code);
                        StructureBadPositionCount++;
                    }
                    else if ( !App.PosIsWithinTileArea(INIStructures.Structures[A].Pos, ZeroPos, Terrain.TileSize) )
                    {
                        logger.Debug ("{0} structure pos x{1} y{2}, is wrong.", INIStructures.Structures[A].Code, INIStructures.Structures [A].Pos.X, INIStructures.Structures [A].Pos.Y);
                        StructureBadPositionCount++;
                    }
                    else
                    {
                        unitTypeBase = App.ObjectData.FindOrCreateUnitType(Convert.ToString(INIStructures.Structures[A].Code),
                            UnitType.PlayerStructure, INIStructures.Structures[A].WallType);
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
                            ErrorCount++;
                        }
                        else
                        {
                            NewUnit = new clsUnit();
                            NewUnit.TypeBase = structureTypeBase;
                            if ( INIStructures.Structures[A].UnitGroup == null )
                            {
                                NewUnit.UnitGroup = ScavengerUnitGroup;
                            }
                            else
                            {
                                NewUnit.UnitGroup = INIStructures.Structures[A].UnitGroup;
                            }
							NewUnit.Pos = new WorldPos(INIStructures.Structures[A].Pos, INIStructures.Structures[A].Pos.Z);
                            NewUnit.Rotation = Convert.ToInt32(INIStructures.Structures[A].Rotation.Direction * 360.0D / App.INIRotationMax);
                            if ( NewUnit.Rotation == 360 )
                            {
                                NewUnit.Rotation = 0;
                            }
                            if ( INIStructures.Structures[A].HealthPercent >= 0 )
                            {
                                NewUnit.Health = MathUtil.Clamp_dbl(INIStructures.Structures[A].HealthPercent / 100.0D, 0.01D, 1.0D);
                            }
                            if ( INIStructures.Structures[A].ID == 0U )
                            {
                                INIStructures.Structures[A].ID = AvailableID;
                                App.ZeroIDWarning(NewUnit, INIStructures.Structures[A].ID, ReturnResult);
                            }
                            UnitAdd.NewUnit = NewUnit;
                            UnitAdd.ID = INIStructures.Structures[A].ID;
                            UnitAdd.Perform();
                            App.ErrorIDChange(INIStructures.Structures[A].ID, NewUnit, "Load_WZ->INIStructures");
                            if ( AvailableID == INIStructures.Structures[A].ID )
                            {
                                AvailableID = NewUnit.ID + 1U;
                            }
                            //create modules
                            switch ( structureTypeBase.StructureType )
                            {
                                case StructureTypeBase.enumStructureType.Factory:
                                    ModuleLimit = 2;
                                    moduleTypeBase = FactoryModule;
                                    break;
                                case StructureTypeBase.enumStructureType.VTOLFactory:
                                    ModuleLimit = 2;
                                    moduleTypeBase = FactoryModule;
                                    break;
                                case StructureTypeBase.enumStructureType.PowerGenerator:
                                    ModuleLimit = 1;
                                    moduleTypeBase = PowerModule;
                                    break;
                                case StructureTypeBase.enumStructureType.Research:
                                    ModuleLimit = 1;
                                    moduleTypeBase = ResearchModule;
                                    break;
                                default:
                                    ModuleLimit = 0;
                                    moduleTypeBase = null;
                                    break;
                            }
                            if ( INIStructures.Structures[A].ModuleCount > ModuleLimit )
                            {
                                INIStructures.Structures[A].ModuleCount = ModuleLimit;
                                StructureBadModulesCount++;
                            }
                            else if ( INIStructures.Structures[A].ModuleCount < 0 )
                            {
                                INIStructures.Structures[A].ModuleCount = 0;
                                StructureBadModulesCount++;
                            }
                            if ( moduleTypeBase != null )
                            {
                                for ( B = 0; B <= INIStructures.Structures[A].ModuleCount - 1; B++ )
                                {
                                    NewModule = new clsUnit();
                                    NewModule.TypeBase = moduleTypeBase;
                                    NewModule.UnitGroup = NewUnit.UnitGroup;
                                    NewModule.Pos = NewUnit.Pos;
                                    NewModule.Rotation = NewUnit.Rotation;
                                    UnitAdd.NewUnit = NewModule;
                                    UnitAdd.ID = AvailableID;
                                    UnitAdd.Perform();
                                    AvailableID = NewModule.ID + 1U;
                                }
                            }
                        }
                    }
                }
                if ( StructureBadPositionCount > 0 )
                {
                    ReturnResult.WarningAdd(StructureBadPositionCount + " structures had an invalid position and were removed.");
                }
                if ( StructureBadModulesCount > 0 )
                {
                    ReturnResult.WarningAdd(StructureBadModulesCount + " structures had an invalid number of modules.");
                }
            }
            if ( INIFeatures != null )
            {
                for ( A = 0; A <= INIFeatures.FeatureCount - 1; A++ )
                {
                    if ( INIFeatures.Features[A].Pos == null )
                    {
                        FeatureBadPositionCount++;
                    }
                    else if ( !App.PosIsWithinTileArea(INIFeatures.Features[A].Pos, ZeroPos, Terrain.TileSize) )
                    {
                        FeatureBadPositionCount++;
                    }
                    else
                    {
                        unitTypeBase = App.ObjectData.FindOrCreateUnitType(Convert.ToString(INIFeatures.Features[A].Code), UnitType.Feature, -1);
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
                            ErrorCount++;
                        }
                        else
                        {
                            NewUnit = new clsUnit();
                            NewUnit.TypeBase = featureTypeBase;
                            NewUnit.UnitGroup = ScavengerUnitGroup;
							NewUnit.Pos = new WorldPos ((XYInt)INIFeatures.Features [A].Pos, INIFeatures.Features [A].Pos.Z);
                            NewUnit.Rotation = Convert.ToInt32(INIFeatures.Features[A].Rotation.Direction * 360.0D / App.INIRotationMax);
                            if ( NewUnit.Rotation == 360 )
                            {
                                NewUnit.Rotation = 0;
                            }
                            if ( INIFeatures.Features[A].HealthPercent >= 0 )
                            {
                                NewUnit.Health = MathUtil.Clamp_dbl(INIFeatures.Features[A].HealthPercent / 100.0D, 0.01D, 1.0D);
                            }
                            if ( INIFeatures.Features[A].ID == 0U )
                            {
                                INIFeatures.Features[A].ID = AvailableID;
                                App.ZeroIDWarning(NewUnit, INIFeatures.Features[A].ID, ReturnResult);
                            }
                            UnitAdd.NewUnit = NewUnit;
                            UnitAdd.ID = INIFeatures.Features[A].ID;
                            UnitAdd.Perform();
                            App.ErrorIDChange(INIFeatures.Features[A].ID, NewUnit, "Load_WZ->INIFeatures");
                            if ( AvailableID == INIFeatures.Features[A].ID )
                            {
                                AvailableID = NewUnit.ID + 1U;
                            }
                        }
                    }
                }
                if ( FeatureBadPositionCount > 0 )
                {
                    ReturnResult.WarningAdd(FeatureBadPositionCount + " features had an invalid position and were removed.");
                }
            }
            if ( INIDroids != null )
            {
                for ( A = 0; A <= INIDroids.DroidCount - 1; A++ )
                {
                    if ( INIDroids.Droids[A].Pos == null )
                    {
                        DroidBadPositionCount++;
                    }
                    else if ( !App.PosIsWithinTileArea(INIDroids.Droids[A].Pos, ZeroPos, Terrain.TileSize) )
                    {
                        DroidBadPositionCount++;
                    }
                    else
                    {
                        if ( INIDroids.Droids[A].Template == null || INIDroids.Droids[A].Template == "" )
                        {
                            DroidType = new DroidDesign();
                            if ( !DroidType.SetDroidType((enumDroidType)(INIDroids.Droids[A].DroidType)) )
                            {
                                UnknownDroidTypeCount++;
                            }
                            LoadPartsArgs.Body = App.ObjectData.FindOrCreateBody(INIDroids.Droids[A].Body);
                            if ( LoadPartsArgs.Body == null )
                            {
                                UnknownDroidComponentCount++;
                            }
                            else
                            {
                                if ( LoadPartsArgs.Body.IsUnknown )
                                {
                                    UnknownDroidComponentCount++;
                                }
                            }
                            LoadPartsArgs.Propulsion = App.ObjectData.FindOrCreatePropulsion(Convert.ToString(INIDroids.Droids[A].Propulsion));
                            if ( LoadPartsArgs.Propulsion == null )
                            {
                                UnknownDroidComponentCount++;
                            }
                            else
                            {
                                if ( LoadPartsArgs.Propulsion.IsUnknown )
                                {
                                    UnknownDroidComponentCount++;
                                }
                            }
                            LoadPartsArgs.Construct = App.ObjectData.FindOrCreateConstruct(Convert.ToString(INIDroids.Droids[A].Construct));
                            if ( LoadPartsArgs.Construct == null )
                            {
                                UnknownDroidComponentCount++;
                            }
                            else
                            {
                                if ( LoadPartsArgs.Construct.IsUnknown )
                                {
                                    UnknownDroidComponentCount++;
                                }
                            }
                            LoadPartsArgs.Repair = App.ObjectData.FindOrCreateRepair(INIDroids.Droids[A].Repair);
                            if ( LoadPartsArgs.Repair == null )
                            {
                                UnknownDroidComponentCount++;
                            }
                            else
                            {
                                if ( LoadPartsArgs.Repair.IsUnknown )
                                {
                                    UnknownDroidComponentCount++;
                                }
                            }
                            LoadPartsArgs.Sensor = App.ObjectData.FindOrCreateSensor(INIDroids.Droids[A].Sensor);
                            if ( LoadPartsArgs.Sensor == null )
                            {
                                UnknownDroidComponentCount++;
                            }
                            else
                            {
                                if ( LoadPartsArgs.Sensor.IsUnknown )
                                {
                                    UnknownDroidComponentCount++;
                                }
                            }
                            LoadPartsArgs.Brain = App.ObjectData.FindOrCreateBrain(INIDroids.Droids[A].Brain);
                            if ( LoadPartsArgs.Brain == null )
                            {
                                UnknownDroidComponentCount++;
                            }
                            else
                            {
                                if ( LoadPartsArgs.Brain.IsUnknown )
                                {
                                    UnknownDroidComponentCount++;
                                }
                            }
                            LoadPartsArgs.ECM = App.ObjectData.FindOrCreateECM(Convert.ToString(INIDroids.Droids[A].ECM));
                            if ( LoadPartsArgs.ECM == null )
                            {
                                UnknownDroidComponentCount++;
                            }
                            else
                            {
                                if ( LoadPartsArgs.ECM.IsUnknown )
                                {
                                    UnknownDroidComponentCount++;
                                }
                            }
                            LoadPartsArgs.Weapon1 = App.ObjectData.FindOrCreateWeapon(Convert.ToString(INIDroids.Droids[A].Weapons[0]));
                            if ( LoadPartsArgs.Weapon1 == null )
                            {
                                UnknownDroidComponentCount++;
                            }
                            else
                            {
                                if ( LoadPartsArgs.Weapon1.IsUnknown )
                                {
                                    UnknownDroidComponentCount++;
                                }
                            }
                            LoadPartsArgs.Weapon2 = App.ObjectData.FindOrCreateWeapon(Convert.ToString(INIDroids.Droids[A].Weapons[1]));
                            if ( LoadPartsArgs.Weapon2 == null )
                            {
                                UnknownDroidComponentCount++;
                            }
                            else
                            {
                                if ( LoadPartsArgs.Weapon2.IsUnknown )
                                {
                                    UnknownDroidComponentCount++;
                                }
                            }
                            LoadPartsArgs.Weapon3 = App.ObjectData.FindOrCreateWeapon(Convert.ToString(INIDroids.Droids[A].Weapons[2]));
                            if ( LoadPartsArgs.Weapon3 == null )
                            {
                                UnknownDroidComponentCount++;
                            }
                            else
                            {
                                if ( LoadPartsArgs.Weapon3.IsUnknown )
                                {
                                    UnknownDroidComponentCount++;
                                }
                            }
                            DroidType.LoadParts(LoadPartsArgs);
                        }
                        else
                        {
                            unitTypeBase = App.ObjectData.FindOrCreateUnitType(INIDroids.Droids[A].Template, UnitType.PlayerDroid, -1);
                            if ( unitTypeBase == null )
                            {
                                DroidType = null;
                            }
                            else
                            {
                                if ( unitTypeBase.Type == UnitType.PlayerDroid )
                                {
                                    DroidType = (DroidDesign)unitTypeBase;
                                }
                                else
                                {
                                    DroidType = null;
                                }
                            }
                        }
                        if ( DroidType == null )
                        {
                            ErrorCount++;
                        }
                        else
                        {
                            NewUnit = new clsUnit();
                            NewUnit.TypeBase = DroidType;
                            if ( INIDroids.Droids[A].UnitGroup == null )
                            {
                                NewUnit.UnitGroup = ScavengerUnitGroup;
                            }
                            else
                            {
                                NewUnit.UnitGroup = INIDroids.Droids[A].UnitGroup;
                            }
							NewUnit.Pos = new WorldPos(INIDroids.Droids[A].Pos, INIDroids.Droids[A].Pos.Z);
                            NewUnit.Rotation = Convert.ToInt32(INIDroids.Droids[A].Rotation.Direction * 360.0D / App.INIRotationMax);
                            if ( NewUnit.Rotation == 360 )
                            {
                                NewUnit.Rotation = 0;
                            }
                            if ( INIDroids.Droids[A].HealthPercent >= 0 )
                            {
                                NewUnit.Health = MathUtil.Clamp_dbl(INIDroids.Droids[A].HealthPercent / 100.0D, 0.01D, 1.0D);
                            }
                            if ( INIDroids.Droids[A].ID == 0U )
                            {
                                INIDroids.Droids[A].ID = AvailableID;
                                App.ZeroIDWarning(NewUnit, INIDroids.Droids[A].ID, ReturnResult);
                            }
                            UnitAdd.NewUnit = NewUnit;
                            UnitAdd.ID = INIDroids.Droids[A].ID;
                            UnitAdd.Perform();
                            App.ErrorIDChange(INIDroids.Droids[A].ID, NewUnit, "Load_WZ->INIDroids");
                            if ( AvailableID == INIDroids.Droids[A].ID )
                            {
                                AvailableID = NewUnit.ID + 1U;
                            }
                        }
                    }
                }
                if ( DroidBadPositionCount > 0 )
                {
                    ReturnResult.WarningAdd(DroidBadPositionCount + " droids had an invalid position and were removed.");
                }
                if ( UnknownDroidTypeCount > 0 )
                {
                    ReturnResult.WarningAdd(UnknownDroidTypeCount + " droid designs had an unrecognised droidType and were removed.");
                }
                if ( UnknownDroidComponentCount > 0 )
                {
                    ReturnResult.WarningAdd(UnknownDroidComponentCount + " droid designs had components that are not loaded.");
                }
            }

            if ( ErrorCount > 0 )
            {
                ReturnResult.WarningAdd("Object Create Error.");
            }

            return ReturnResult;
        }

        private clsResult read_INI_Features(string iniText, ref IniFeatures resultData)
        {
            var resultObject = new clsResult ("Reading feature.ini.", false);
            logger.Info ("Reading feature.ini");

            try {
                var iniSections = SharpFlame.Core.Parsers.Ini.IniReader.ReadString (iniText);
                foreach (var iniSection in iniSections) {
                    var feature = new IniFeatures.Feature();
                    feature.HealthPercent = -1;
                    var invalid = false;
                    foreach (var iniToken in iniSection.Data) {
                        if (invalid) {
                            break;
                        }

                        try {
                            switch (iniToken.Name) {
                            case "id":
                                feature.ID = uint.Parse(iniToken.Data);
                                break;
                            case "name":
                                feature.Code = iniToken.Data;
                                break;
                            case "position":
								feature.Pos =  XYZInt.FromString(iniToken.Data);
                                break;
                            case "rotation":
								feature.Rotation = Rotation.FromString(iniToken.Data);
                                break;
                            case "health":
                                feature.HealthPercent = SharpFlame.Core.Parsers.Ini.IniReader.ReadHealthPercent(iniToken.Data); 
                                if (feature.HealthPercent < 0 || feature.HealthPercent > 100) {
                                    resultObject.WarningAdd(string.Format("#{0} invalid health: \"{1}\"", iniSection.Name, feature.HealthPercent), false);
                                    logger.Warn("#{0} invalid health: \"{1}\"", iniSection.Name, feature.HealthPercent);
                                    invalid = true;
                                    continue;
                                }
                                break;
                            default:
                                resultObject.WarningAdd(string.Format("Found an invalid key: {0}={1}", iniToken.Name, iniToken.Data), false);
                                logger.Warn("Found an invalid key: {0} = {1}", iniToken.Name, iniToken.Data);
                                break;
                            }
                        } catch (Exception ex) {
                            Debugger.Break();
                            resultObject.WarningAdd(string.Format("#{0} invalid {2}: \"{3}\", got exception: {2}", iniSection.Name, iniToken.Name, iniToken.Data, ex.Message), false);
                            logger.WarnException(string.Format("#{0} invalid {2} \"{1}\"", iniSection.Name, iniToken.Name, iniToken.Data), ex);                                    
                            invalid = true;
                            continue;
                        }
                    }

                    if (!invalid) {
                        resultData.Features.Add(feature);
                    }
                }
            }
            catch (Exception ex) {
                Debugger.Break ();
                logger.ErrorException ("Got exception while reading feature.ini", ex);
                resultObject.ProblemAdd (string.Format ("Got exception: {0}", ex.Message), false);
                return resultObject;
            }

            return resultObject;
        }

        private clsResult read_INI_Droids(string iniText, ref IniDroids resultData)
        {
            var resultObject = new clsResult ("Reading droids.ini.", false);

            try {
                var iniSections = SharpFlame.Core.Parsers.Ini.IniReader.ReadString (iniText);            
                foreach (var iniSection in iniSections) {                
                    var droid = new IniDroids.Droid();
                    droid.HealthPercent = -1;
                    var invalid = false;
                    foreach (var iniToken in iniSection.Data) {                        
                        if (invalid) {
                            break;
                        }

                        try {
                            switch (iniToken.Name) {
                            
                            case "id":
                                droid.ID = uint.Parse (iniToken.Data);
                                break;
                            
                            case "startpos":
                                var tmpStartPos = int.Parse (iniToken.Data);
                                if (tmpStartPos < 0 | tmpStartPos >= Constants.PlayerCountMax) {
                                    resultObject.WarningAdd(string.Format("#{0} invalid startpos {1}", iniSection.Name, tmpStartPos), false);
                                    logger.Warn("#{0} invalid startpos {1}", iniSection.Name, tmpStartPos);                                    
                                    invalid = true;
                                    continue;
                                }
                                droid.UnitGroup = UnitGroups[tmpStartPos];
                                break;

                            case "template":
                                droid.Template = iniToken.Data;
                                break;

                            case "position":
								droid.Pos =  XYZInt.FromString(iniToken.Data);
                                break;

                            case "rotation":
								droid.Rotation = Rotation.FromString(iniToken.Data);
                                break;

                            case "player":
                                if (iniToken.Data.ToLower() == "scavenger") {
                                    droid.UnitGroup = ScavengerUnitGroup;
                                } else {
                                    resultObject.WarningAdd(string.Format("#{0} invalid player: \"{1}\"", iniToken.Name, iniToken.Data), false);
                                    logger.Warn("#{0} invalid player \"{1}\"", iniToken.Name, iniToken.Data);                                    
                                    invalid = true;
                                    continue;
                                }
                                break;

                            case "name":
                                // ignore
                                break;

                            case "health":
                                droid.HealthPercent = SharpFlame.Core.Parsers.Ini.IniReader.ReadHealthPercent(iniToken.Data); 
                                if (droid.HealthPercent < 0 || droid.HealthPercent > 100) {
                                    resultObject.WarningAdd(string.Format("#{0} invalid health: \"{1}\"", iniSection.Name, droid.HealthPercent), false);
                                    invalid = true;
                                    continue;
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
                                if (droid.Weapons == null) {
                                    droid.Weapons = new string[3];
                                }
                                droid.Weapons[0] = iniToken.Data;
                                break;
                            
                            case "parts\\weapon\\2":
                                if (droid.Weapons == null) {
                                    droid.Weapons = new string[3];
                                }

                                droid.Weapons[1] = iniToken.Data;
                                break;

                            case "parts\\weapon\\3":
                                if (droid.Weapons == null) {
                                    droid.Weapons = new string[3];
                                }

                                droid.Weapons[2] = iniToken.Data;
                                break;
                            
                            default:
                                resultObject.WarningAdd(string.Format("Found an invalid key: {0} = {1}", iniToken.Name, iniToken.Data), false);
                                logger.Warn("Found an invalid key: {0} = {1}", iniToken.Name, iniToken.Data);
                                break;                        
                            }
                        } catch (Exception ex) {
                            Debugger.Break();
                            resultObject.WarningAdd(string.Format("#{0} invalid {2}: \"{3}\", got exception: {2}", iniSection.Name, iniToken.Name, iniToken.Data, ex.Message), false);
                            logger.ErrorException(string.Format("#{0} invalid {2} \"{1}\"", iniSection.Name, iniToken.Name, iniToken.Data), ex);                                    
                            invalid = true;
                            continue;
                        }
                    }

                    if (!invalid) {
                        resultData.Droids.Add(droid);
                    }
                }            
            }
            catch (Exception ex) {
                Debugger.Break ();
                logger.ErrorException ("Got exception while reading droid.ini", ex);
                resultObject.ProblemAdd (string.Format ("Got exception: {0}", ex.Message), false);
                return resultObject;
            }

            return resultObject;
        }

        private clsResult read_INI_Structures(string iniText, ref IniStructures resultData)
        {
            var resultObject = new clsResult ("Reading struct.ini.", false);
            logger.Info ("Reading struct.ini");

            try {
                var iniSections = SharpFlame.Core.Parsers.Ini.IniReader.ReadString (iniText);
                foreach (var iniSection in iniSections) {
                    var structure = new IniStructures.Structure();
                    structure.WallType = -1;
                    structure.HealthPercent = -1;
                    var invalid = false;
                    foreach (var iniToken in iniSection.Data) {
                        if (invalid) {
                            break;
                        }

                        try {
                            switch (iniToken.Name) {
                            case "id":
                                structure.ID = uint.Parse (iniToken.Data);
                                break;

                            case "name":
                                structure.Code = iniToken.Data;
                                break;

                            case "startpos":
                                var tmpStartPos = int.Parse (iniToken.Data);
                                if (tmpStartPos < 0 | tmpStartPos >= Constants.PlayerCountMax) {
                                    resultObject.WarningAdd(string.Format("#{0} invalid startpos {1}", iniSection.Name, tmpStartPos), false);
                                    logger.Warn("#{0} invalid startpos {1}", iniSection.Name, tmpStartPos);                                    
                                    invalid = true;
                                    continue;
                                }
                                structure.UnitGroup = UnitGroups[tmpStartPos];
                                break;

                            case "player":
                                if (iniToken.Data.ToLower() == "scavenger") {
                                    structure.UnitGroup = ScavengerUnitGroup;
                                } else {
                                    resultObject.WarningAdd(string.Format("#{0} invalid player: \"{1}\"", iniToken.Name, iniToken.Data), false);
                                    logger.Warn("#{0} invalid player \"{1}\"", iniToken.Name, iniToken.Data);                                    
                                    invalid = true;
                                    continue;
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
                                structure.HealthPercent = SharpFlame.Core.Parsers.Ini.IniReader.ReadHealthPercent(iniToken.Data); 
                                if (structure.HealthPercent < 0 || structure.HealthPercent > 100) {
                                    resultObject.WarningAdd(string.Format("#{0} invalid health: \"{1}\"", iniSection.Name, structure.HealthPercent), false);
                                    invalid = true;
                                    continue;
                                }
                                break;

                            case "wall/type":
                                structure.WallType = int.Parse(iniToken.Data);
                                if (structure.WallType < 0) {
                                    resultObject.WarningAdd(string.Format("#{0} invalid wall/type: \"{1}\"", iniSection.Name, structure.WallType), false);
                                    invalid = true;
                                    continue;
                                }
                                break;

                            default:
                                resultObject.WarningAdd(string.Format("Found an invalid key: {0} = {1}", iniToken.Name, iniToken.Data), false);
                                logger.Warn("Found an invalid key: {0} = {1}", iniToken.Name, iniToken.Data);
                                break;                        
                            }

                        } catch (Exception ex) {
                            Debugger.Break();
                            resultObject.WarningAdd(string.Format("#{0} invalid {2}: \"{3}\", got exception: {2}", iniSection.Name, iniToken.Name, iniToken.Data, ex.Message), false);
                            logger.WarnException(string.Format("#{0} invalid {2} \"{1}\"", iniSection.Name, iniToken.Name, iniToken.Data), ex);                                    
                            invalid = true;
                            continue;
                        }
                    }

                    if (!invalid) {
                        resultData.Structures.Add(structure);
                    }
                }
            }
            catch (Exception ex) {
                Debugger.Break ();
                logger.ErrorException ("Got exception while reading droid.ini", ex);
                resultObject.ProblemAdd (string.Format ("Got exception: {0}", ex.Message), false);
                return resultObject;
            }

            return resultObject;
        }

        private sResult Read_WZ_gam(BinaryReader File)
        {
            sResult ReturnResult = new sResult();
            ReturnResult.Success = false;
            ReturnResult.Problem = "";

            string strTemp = "";
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

                if ( InterfaceOptions == null )
                {
                    InterfaceOptions = new clsInterfaceOptions();
                }

                File.ReadInt32(); //game time
                InterfaceOptions.CampaignGameType = File.ReadInt32();
                InterfaceOptions.AutoScrollLimits = false;
                InterfaceOptions.ScrollMin.X = File.ReadInt32();
                InterfaceOptions.ScrollMin.Y = File.ReadInt32();
                InterfaceOptions.ScrollMax.X = File.ReadUInt32();
                InterfaceOptions.ScrollMax.Y = File.ReadUInt32();
            }
            catch ( Exception ex )
            {
                ReturnResult.Problem = ex.Message;
				logger.ErrorException ("Got an exception", ex);
                return ReturnResult;
            }

            ReturnResult.Success = true;
            return ReturnResult;
        }

        private sResult Read_WZ_map(BinaryReader File)
        {
            sResult ReturnResult = new sResult();
            ReturnResult.Success = false;
            ReturnResult.Problem = "";

            string strTemp = null;
            UInt32 Version = 0;
            UInt32 MapWidth = 0;
            UInt32 MapHeight = 0;
            UInt32 uintTemp = 0;
            byte Flip = 0;
            bool FlipX = default(bool);
            bool FlipZ = default(bool);
            byte Rotate = 0;
            byte TextureNum = 0;
            int A = 0;
            int X = 0;
            int Y = 0;
            XYInt PosA = new XYInt();
            XYInt PosB = new XYInt();

            try
            {
                strTemp = IOUtil.ReadOldTextOfLength(File, 4);
                if ( strTemp != "map " )
                {
                    ReturnResult.Problem = "Unknown game.map identifier.";
                    return ReturnResult;
                }

                Version = File.ReadUInt32();
                if ( Version != 10U )
                {
                    if ( MessageBox.Show("game.map version is unknown. Continue?", "", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK )
                    {
                        ReturnResult.Problem = "Aborted.";
                        return ReturnResult;
                    }
                }
                MapWidth = File.ReadUInt32();
                MapHeight = File.ReadUInt32();
                if ( MapWidth < 1U || MapWidth > Constants.MapMaxSize || MapHeight < 1U || MapHeight > Constants.MapMaxSize )
                {
                    ReturnResult.Problem = "Map size out of range.";
                    return ReturnResult;
                }

                TerrainBlank(new XYInt(Convert.ToInt32(MapWidth), Convert.ToInt32(MapHeight)));

                for ( Y = 0; Y <= Terrain.TileSize.Y - 1; Y++ )
                {
                    for ( X = 0; X <= Terrain.TileSize.X - 1; X++ )
                    {
                        TextureNum = File.ReadByte();
                        Terrain.Tiles[X, Y].Texture.TextureNum = TextureNum;
                        Flip = File.ReadByte();
                        Terrain.Vertices[X, Y].Height = File.ReadByte();
                        //get flipx
                        A = (int)((Flip / 128.0D));
                        Flip -= (byte)(A * 128);
                        FlipX = A == 1;
                        //get flipy
                        A = (int)((Flip / 64.0D));
                        Flip -= (byte)(A * 64);
                        FlipZ = A == 1;
                        //get rotation
                        A = (int)((Flip / 16.0D));
                        Flip -= (byte)(A * 16);
                        Rotate = (byte)A;
                        TileUtil.OldOrientation_To_TileOrientation(Rotate, FlipX, FlipZ, ref Terrain.Tiles[X, Y].Texture.Orientation);
                        //get tri direction
                        A = (int)((Flip / 8.0D));
                        Flip -= (byte)(A * 8);
                        Terrain.Tiles[X, Y].Tri = A == 1;
                    }
                }

                if ( Version != 2U )
                {
                    uintTemp = File.ReadUInt32();
                    if ( uintTemp != 1 )
                    {
                        ReturnResult.Problem = "Bad gateway version number.";
                        return ReturnResult;
                    }

                    uintTemp = File.ReadUInt32();

                    for ( A = 0; A <= (Convert.ToInt32(uintTemp)) - 1; A++ )
                    {
                        PosA.X = File.ReadByte();
                        PosA.Y = File.ReadByte();
                        PosB.X = File.ReadByte();
                        PosB.Y = File.ReadByte();
                        if ( GatewayCreate(PosA, PosB) == null )
                        {
                            ReturnResult.Problem = "Gateway placement error.";
                            return ReturnResult;
                        }
                    }
                }
            }
            catch ( Exception ex )
            {
                ReturnResult.Problem = ex.Message;
				logger.ErrorException ("Got an exception", ex);
                return ReturnResult;
            }

            ReturnResult.Success = true;
            return ReturnResult;
        }

        private sResult Read_WZ_Features(BinaryReader File, SimpleClassList<clsWZBJOUnit> WZUnits)
        {
            sResult ReturnResult = new sResult();
            ReturnResult.Success = false;
            ReturnResult.Problem = "";

            string strTemp = null;
            UInt32 Version = 0;
            UInt32 uintTemp = 0;
            int A = 0;
            clsWZBJOUnit WZBJOUnit = default(clsWZBJOUnit);

            try
            {
                strTemp = IOUtil.ReadOldTextOfLength(File, 4);
                if ( strTemp != "feat" )
                {
                    ReturnResult.Problem = "Unknown feat.bjo identifier.";
                    return ReturnResult;
                }

                Version = File.ReadUInt32();
                if ( Version != 8U )
                {
                    if ( MessageBox.Show("feat.bjo version is unknown. Continue?", "", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK )
                    {
                        ReturnResult.Problem = "Aborted.";
                        return ReturnResult;
                    }
                }

                uintTemp = File.ReadUInt32();
                for ( A = 0; A <= (Convert.ToInt32(uintTemp)) - 1; A++ )
                {
                    WZBJOUnit = new clsWZBJOUnit();
                    WZBJOUnit.ObjectType = UnitType.Feature;
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
				logger.ErrorException ("Got an exception", ex);
                return ReturnResult;
            }

            ReturnResult.Success = true;
            return ReturnResult;
        }

        private sResult Read_WZ_TileTypes(BinaryReader File)
        {
            sResult ReturnResult = new sResult();
            ReturnResult.Success = false;
            ReturnResult.Problem = "";

            string strTemp = null;
            UInt32 Version = 0;
            UInt32 uintTemp = 0;
            UInt16 ushortTemp = 0;
            int A = 0;

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

                if ( Tileset != null )
                {
                    for ( A = 0; A <= Math.Min(Convert.ToInt32(uintTemp), Tileset.TileCount) - 1; A++ )
                    {
                        ushortTemp = File.ReadUInt16();
                        if ( ushortTemp > 11U )
                        {
                            ReturnResult.Problem = "Unknown tile type.";
                            return ReturnResult;
                        }
                        Tile_TypeNum[A] = (byte)ushortTemp;
                    }
                }
            }
            catch ( Exception ex )
            {
                ReturnResult.Problem = ex.Message;
				logger.ErrorException ("Got an exception", ex);
                return ReturnResult;
            }

            ReturnResult.Success = true;
            return ReturnResult;
        }

        private sResult Read_WZ_Structures(BinaryReader File, SimpleClassList<clsWZBJOUnit> WZUnits)
        {
            sResult ReturnResult = new sResult();
            ReturnResult.Success = false;
            ReturnResult.Problem = "";

            string strTemp = null;
            UInt32 Version = 0;
            UInt32 uintTemp = 0;
            int A = 0;
            clsWZBJOUnit WZBJOUnit = default(clsWZBJOUnit);

            try
            {
                strTemp = IOUtil.ReadOldTextOfLength(File, 4);
                if ( strTemp != "stru" )
                {
                    ReturnResult.Problem = "Unknown struct.bjo identifier.";
                    return ReturnResult;
                }

                Version = File.ReadUInt32();
                if ( Version != 8U )
                {
                    if ( MessageBox.Show("struct.bjo version is unknown. Continue?", "", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK )
                    {
                        ReturnResult.Problem = "Aborted.";
                        return ReturnResult;
                    }
                }

                uintTemp = File.ReadUInt32();
                for ( A = 0; A <= (Convert.ToInt32(uintTemp)) - 1; A++ )
                {
                    WZBJOUnit = new clsWZBJOUnit();
                    WZBJOUnit.ObjectType = UnitType.PlayerStructure;
                    WZBJOUnit.Code = IOUtil.ReadOldTextOfLength(File, 40);
                    WZBJOUnit.Code = WZBJOUnit.Code.Substring(0, WZBJOUnit.Code.IndexOf('\0'));
                    WZBJOUnit.ID = File.ReadUInt32();
                    WZBJOUnit.Pos.Horizontal.X = (int)(File.ReadUInt32());
                    WZBJOUnit.Pos.Horizontal.Y = (int)(File.ReadUInt32());
                    WZBJOUnit.Pos.Altitude = (int)(File.ReadUInt32());
                    WZBJOUnit.Rotation = File.ReadUInt32();
                    WZBJOUnit.Player = File.ReadUInt32();
                    File.ReadBytes(56);
                    WZUnits.Add(WZBJOUnit);
                }
            }
            catch ( Exception ex )
            {
                ReturnResult.Problem = ex.Message;
				logger.ErrorException ("Got an exception", ex);
                return ReturnResult;
            }

            ReturnResult.Success = true;
            return ReturnResult;
        }

        private sResult Read_WZ_Droids(BinaryReader File, SimpleClassList<clsWZBJOUnit> WZUnits)
        {
            sResult ReturnResult = new sResult();
            ReturnResult.Success = false;
            ReturnResult.Problem = "";

            string strTemp = null;
            UInt32 Version = 0;
            UInt32 uintTemp = 0;
            int A = 0;
            clsWZBJOUnit WZBJOUnit = default(clsWZBJOUnit);

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
                    WZBJOUnit = new clsWZBJOUnit();
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
				logger.ErrorException ("Got an exception", ex);
                return ReturnResult;
            }

            ReturnResult.Success = true;
            return ReturnResult;
        }

        public clsResult Read_WZ_Labels(SharpFlame.FileIO.Ini.IniReader INI, bool IsFMap)
        {
            clsResult ReturnResult = new clsResult("Reading labels", false);
            logger.Info ("Reading labels.");

            PositionFromText PositionsA = default(PositionFromText);
            PositionFromText PositionsB = default(PositionFromText);
            int TypeNum = 0;
            clsScriptPosition NewPosition = default(clsScriptPosition);
            clsScriptArea NewArea = default(clsScriptArea);
            string NameText = "";
            string strLabel = "";
            string strPosA = "";
            string strPosB = "";
            string IDText = "";
            UInt32 IDNum = 0;

            int FailedCount = 0;
            int ModifiedCount = 0;

            SharpFlame.FileIO.Ini.Section INISection = default(SharpFlame.FileIO.Ini.Section);
            foreach ( SharpFlame.FileIO.Ini.Section tempLoopVar_INISection in INI.Sections )
            {
                INISection = tempLoopVar_INISection;
                NameText = INISection.Name.Substring(0, NameText.IndexOf('_') - 1);
                switch ( NameText )
                {
                    case "position":
                        TypeNum = 0;
                        break;
                    case "area":
                        TypeNum = 1;
                        break;
                    case "object":
                        if ( IsFMap )
                        {
                            TypeNum = int.MaxValue;
                            FailedCount++;
                            continue;
                        }
                        else
                        {
                            TypeNum = 2;
                        }
                        break;
                    default:
                        TypeNum = int.MaxValue;
                        FailedCount++;
                        continue;
                }
                strLabel = Convert.ToString(INISection.GetLastPropertyValue("label"));
                if ( strLabel == null )
                {
                    FailedCount++;
                    continue;
                }
                strLabel = strLabel.Replace("\"", "");
                switch ( TypeNum )
                {
                    case 0: //position
                        strPosA = Convert.ToString(INISection.GetLastPropertyValue("pos"));
                        if ( strPosA == null )
                        {
                            FailedCount++;
                            continue;
                        }
                        PositionsA = new PositionFromText();
                        if ( PositionsA.Translate(strPosA) )
                        {
                            NewPosition = new clsScriptPosition(this);
                            NewPosition.PosX = PositionsA.Pos.X;
                            NewPosition.PosY = PositionsA.Pos.Y;
                            NewPosition.SetLabel(strLabel);
                            if ( NewPosition.Label != strLabel || NewPosition.PosX != PositionsA.Pos.X | NewPosition.PosY != PositionsA.Pos.Y )
                            {
                                ModifiedCount++;
                            }
                        }
                        else
                        {
                            FailedCount++;
                            continue;
                        }
                        break;
                    case 1: //area
                        strPosA = Convert.ToString(INISection.GetLastPropertyValue("pos1"));
                        if ( strPosA == null )
                        {
                            FailedCount++;
                            continue;
                        }
                        strPosB = Convert.ToString(INISection.GetLastPropertyValue("pos2"));
                        if ( strPosB == null )
                        {
                            FailedCount++;
                            continue;
                        }
                        PositionsA = new PositionFromText();
                        PositionsB = new PositionFromText();
                        if ( PositionsA.Translate(strPosA) && PositionsB.Translate(strPosB) )
                        {
                            NewArea = clsScriptArea.Create(this);
                            NewArea.SetPositions(PositionsA.Pos, PositionsB.Pos);
                            NewArea.SetLabel(strLabel);
                            if ( NewArea.Label != strLabel || NewArea.PosAX != PositionsA.Pos.X | NewArea.PosAY != PositionsA.Pos.Y
                                 | NewArea.PosBX != PositionsB.Pos.X | NewArea.PosBY != PositionsB.Pos.Y )
                            {
                                ModifiedCount++;
                            }
                        }
                        else
                        {
                            FailedCount++;
                            continue;
                        }
                        break;
                    case 2: //object
                        IDText = Convert.ToString(INISection.GetLastPropertyValue("id"));
                        if ( IOUtil.InvariantParse(IDText, ref IDNum) )
                        {
                            clsUnit Unit = IDUsage(IDNum);
                            if ( Unit != null )
                            {
                                if ( !Unit.SetLabel(strLabel).Success )
                                {
                                    FailedCount++;
                                    continue;
                                }
                            }
                            else
                            {
                                FailedCount++;
                                continue;
                            }
                        }
                        break;
                    default:
                        ReturnResult.WarningAdd("Error! Bad type number for script label.");
                        break;
                }
            }

            if ( FailedCount > 0 )
            {
                ReturnResult.WarningAdd("Unable to translate " + Convert.ToString(FailedCount) + " script labels.");
            }
            if ( ModifiedCount > 0 )
            {
                ReturnResult.WarningAdd(ModifiedCount + " script labels had invalid values and were modified.");
            }

            return ReturnResult;
        }

        public clsResult Serialize_WZ_StructuresINI(IniWriter File, int PlayerCount)
        {
            clsResult ReturnResult = new clsResult("Serializing structures INI", false);
            logger.Info ("Serializing structures INI");

            StructureTypeBase structureTypeBase = default(StructureTypeBase);
            clsUnit Unit = default(clsUnit);
            bool[] UnitIsModule = new bool[Units.Count];
            int[] UnitModuleCount = new int[Units.Count];
            XYInt SectorNum = new XYInt();
            StructureTypeBase otherStructureTypeBase = default(StructureTypeBase);
            clsUnit OtherUnit = default(clsUnit);
            XYInt ModuleMin = new XYInt();
            XYInt ModuleMax = new XYInt();
            XYInt Footprint = new XYInt();
            int A = 0;
            StructureTypeBase.enumStructureType[] UnderneathTypes = new StructureTypeBase.enumStructureType[2];
            int UnderneathTypeCount = 0;
            int BadModuleCount = 0;
            clsObjectPriorityOrderList PriorityOrder = new clsObjectPriorityOrderList();

            foreach ( clsUnit tempLoopVar_Unit in Units )
            {
                Unit = tempLoopVar_Unit;
                if ( Unit.TypeBase.Type == UnitType.PlayerStructure )
                {
                    structureTypeBase = (StructureTypeBase)Unit.TypeBase;
                    switch ( structureTypeBase.StructureType )
                    {
                        case StructureTypeBase.enumStructureType.FactoryModule:
                            UnderneathTypes[0] = StructureTypeBase.enumStructureType.Factory;
                            UnderneathTypes[1] = StructureTypeBase.enumStructureType.VTOLFactory;
                            UnderneathTypeCount = 2;
                            break;
                        case StructureTypeBase.enumStructureType.PowerModule:
                            UnderneathTypes[0] = StructureTypeBase.enumStructureType.PowerGenerator;
                            UnderneathTypeCount = 1;
                            break;
                        case StructureTypeBase.enumStructureType.ResearchModule:
                            UnderneathTypes[0] = StructureTypeBase.enumStructureType.Research;
                            UnderneathTypeCount = 1;
                            break;
                        default:
                            UnderneathTypeCount = 0;
                            break;
                    }
                    if ( UnderneathTypeCount == 0 )
                    {
                        PriorityOrder.SetItem(Unit);
                        PriorityOrder.ActionPerform();
                    }
                    else
                    {
                        UnitIsModule[Unit.MapLink.ArrayPosition] = true;
                        SectorNum = GetPosSectorNum(Unit.Pos.Horizontal);
                        clsUnit Underneath = null;
                        clsUnitSectorConnection Connection = default(clsUnitSectorConnection);
                        foreach ( clsUnitSectorConnection tempLoopVar_Connection in Sectors[SectorNum.X, SectorNum.Y].Units )
                        {
                            Connection = tempLoopVar_Connection;
                            OtherUnit = Connection.Unit;
                            if ( OtherUnit.TypeBase.Type == UnitType.PlayerStructure )
                            {
                                otherStructureTypeBase = (StructureTypeBase)OtherUnit.TypeBase;
                                if ( OtherUnit.UnitGroup == Unit.UnitGroup )
                                {
                                    for ( A = 0; A <= UnderneathTypeCount - 1; A++ )
                                    {
                                        if ( otherStructureTypeBase.StructureType == UnderneathTypes[A] )
                                        {
                                            break;
                                        }
                                    }
                                    if ( A < UnderneathTypeCount )
                                    {
                                        Footprint = otherStructureTypeBase.get_GetFootprintSelected(OtherUnit.Rotation);
                                        ModuleMin.X = OtherUnit.Pos.Horizontal.X - (int)(Footprint.X * App.TerrainGridSpacing / 2.0D);
                                        ModuleMin.Y = OtherUnit.Pos.Horizontal.Y - (int)(Footprint.Y * App.TerrainGridSpacing / 2.0D);
                                        ModuleMax.X = OtherUnit.Pos.Horizontal.X + (int)(Footprint.X * App.TerrainGridSpacing / 2.0D);
                                        ModuleMax.Y = OtherUnit.Pos.Horizontal.Y + (int)(Footprint.Y * App.TerrainGridSpacing / 2.0D);
                                        if ( Unit.Pos.Horizontal.X >= ModuleMin.X & Unit.Pos.Horizontal.X < ModuleMax.X &
                                             Unit.Pos.Horizontal.Y >= ModuleMin.Y & Unit.Pos.Horizontal.Y < ModuleMax.Y )
                                        {
                                            UnitModuleCount[OtherUnit.MapLink.ArrayPosition]++;
                                            Underneath = OtherUnit;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        if ( Underneath == null )
                        {
                            BadModuleCount++;
                        }
                    }
                }
            }

            if ( BadModuleCount > 0 )
            {
                ReturnResult.WarningAdd(BadModuleCount + " modules had no underlying structure.");
            }

            int TooManyModulesWarningCount = 0;
            int TooManyModulesWarningMaxCount = 16;
            int ModuleCount = 0;
            int ModuleLimit = 0;

            for ( A = 0; A <= PriorityOrder.Result.Count - 1; A++ )
            {
                Unit = PriorityOrder.Result[A];
                structureTypeBase = (StructureTypeBase)Unit.TypeBase;
                if ( Unit.ID <= 0 )
                {
                    ReturnResult.WarningAdd("Error. A structure\'s ID was zero. It was NOT saved. Delete and replace it to allow save.");
                }
                else
                {
                    File.AppendSectionName("structure_" + Unit.ID.ToStringInvariant());
                    File.AppendProperty("id", Unit.ID.ToStringInvariant());
                    if ( Unit.UnitGroup == ScavengerUnitGroup || (PlayerCount >= 0 & Unit.UnitGroup.WZ_StartPos >= PlayerCount) )
                    {
                        File.AppendProperty("player", "scavenger");
                    }
                    else
                    {
                        File.AppendProperty("startpos", Unit.UnitGroup.WZ_StartPos.ToStringInvariant());
                    }
                    File.AppendProperty("name", structureTypeBase.Code);
                    if ( structureTypeBase.WallLink.IsConnected )
                    {
                        File.AppendProperty("wall/type", structureTypeBase.WallLink.ArrayPosition.ToStringInvariant());
                    }
                    File.AppendProperty("position", Unit.GetINIPosition());
                    File.AppendProperty("rotation", Unit.GetINIRotation());
                    if ( Unit.Health < 1.0D )
                    {
                        File.AppendProperty("health", Unit.GetINIHealthPercent());
                    }
                    switch ( structureTypeBase.StructureType )
                    {
                        case StructureTypeBase.enumStructureType.Factory:
                            ModuleLimit = 2;
                            break;
                        case StructureTypeBase.enumStructureType.VTOLFactory:
                            ModuleLimit = 2;
                            break;
                        case StructureTypeBase.enumStructureType.PowerGenerator:
                            ModuleLimit = 1;
                            break;
                        case StructureTypeBase.enumStructureType.Research:
                            ModuleLimit = 1;
                            break;
                        default:
                            ModuleLimit = 0;
                            break;
                    }
                    if ( UnitModuleCount[Unit.MapLink.ArrayPosition] > ModuleLimit )
                    {
                        ModuleCount = ModuleLimit;
                        if ( TooManyModulesWarningCount < TooManyModulesWarningMaxCount )
                        {
                            ReturnResult.WarningAdd("Structure " + structureTypeBase.GetDisplayTextCode() + " at " + Unit.GetPosText() + " has too many modules (" +
                                                    Convert.ToString(UnitModuleCount[Unit.MapLink.ArrayPosition]) + ").");
                        }
                        TooManyModulesWarningCount++;
                    }
                    else
                    {
                        ModuleCount = UnitModuleCount[Unit.MapLink.ArrayPosition];
                    }
                    File.AppendProperty("modules", ModuleCount.ToStringInvariant());
                    File.Gap_Append();
                }
            }

            if ( TooManyModulesWarningCount > TooManyModulesWarningMaxCount )
            {
                ReturnResult.WarningAdd(TooManyModulesWarningCount + " structures had too many modules.");
            }

            return ReturnResult;
        }

        public clsResult Serialize_WZ_DroidsINI(IniWriter File, int PlayerCount)
        {
            clsResult ReturnResult = new clsResult("Serializing droids INI", false);
            logger.Info ("Serializing droids INI");

            DroidDesign Droid = default(DroidDesign);
            DroidTemplate Template = default(DroidTemplate);
            string Text = "";
            clsUnit Unit = default(clsUnit);
            bool AsPartsNotTemplate = default(bool);
            bool ValidDroid = default(bool);
            int InvalidPartCount = 0;
            Brain Brain = default(Brain);

            foreach ( clsUnit tempLoopVar_Unit in Units )
            {
                Unit = tempLoopVar_Unit;
                if ( Unit.TypeBase.Type == UnitType.PlayerDroid )
                {
                    Droid = (DroidDesign)Unit.TypeBase;
                    ValidDroid = true;
                    if ( Unit.ID <= 0 )
                    {
                        ValidDroid = false;
                        ReturnResult.WarningAdd("Error. A droid\'s ID was zero. It was NOT saved. Delete and replace it to allow save.");
                    }
                    if ( Droid.IsTemplate )
                    {
                        Template = (DroidTemplate)Droid;
                        AsPartsNotTemplate = Unit.PreferPartsOutput;
                    }
                    else
                    {
                        Template = null;
                        AsPartsNotTemplate = true;
                    }
                    if ( AsPartsNotTemplate )
                    {
                        if ( Droid.Body == null )
                        {
                            ValidDroid = false;
                            InvalidPartCount++;
                        }
                        else if ( Droid.Propulsion == null )
                        {
                            ValidDroid = false;
                            InvalidPartCount++;
                        }
                        else if ( Droid.TurretCount >= 1 )
                        {
                            if ( Droid.Turret1 == null )
                            {
                                ValidDroid = false;
                                InvalidPartCount++;
                            }
                        }
                        else if ( Droid.TurretCount >= 2 )
                        {
                            if ( Droid.Turret2 == null )
                            {
                                ValidDroid = false;
                                InvalidPartCount++;
                            }
                            else if ( Droid.Turret2.TurretType != enumTurretType.Weapon )
                            {
                                ValidDroid = false;
                                InvalidPartCount++;
                            }
                        }
                        else if ( Droid.TurretCount >= 3 && Droid.Turret3 == null )
                        {
                            if ( Droid.Turret3 == null )
                            {
                                ValidDroid = false;
                                InvalidPartCount++;
                            }
                            else if ( Droid.Turret3.TurretType != enumTurretType.Weapon )
                            {
                                ValidDroid = false;
                                InvalidPartCount++;
                            }
                        }
                    }
                    if ( ValidDroid )
                    {
                        File.AppendSectionName("droid_" + Unit.ID.ToStringInvariant());
                        File.AppendProperty("id", Unit.ID.ToStringInvariant());
                        if ( Unit.UnitGroup == ScavengerUnitGroup || (PlayerCount >= 0 & Unit.UnitGroup.WZ_StartPos >= PlayerCount) )
                        {
                            File.AppendProperty("player", "scavenger");
                        }
                        else
                        {
                            File.AppendProperty("startpos", Unit.UnitGroup.WZ_StartPos.ToStringInvariant());
                        }
                        if ( AsPartsNotTemplate )
                        {
                            File.AppendProperty("name", Droid.GenerateName());
                        }
                        else
                        {
                            Template = (DroidTemplate)Droid;
                            File.AppendProperty("template", Template.Code);
                        }
                        File.AppendProperty("position", Unit.GetINIPosition());
                        File.AppendProperty("rotation", Unit.GetINIRotation());
                        if ( Unit.Health < 1.0D )
                        {
                            File.AppendProperty("health", Unit.GetINIHealthPercent());
                        }
                        if ( AsPartsNotTemplate )
                        {
                            File.AppendProperty("droidType", Convert.ToInt32(Droid.GetDroidType()).ToStringInvariant());
                            if ( Droid.TurretCount == 0 )
                            {
                                Text = "0";
                            }
                            else
                            {
                                if ( Droid.Turret1.TurretType == enumTurretType.Brain )
                                {
                                    if ( ((Brain)Droid.Turret1).Weapon == null )
                                    {
                                        Text = "0";
                                    }
                                    else
                                    {
                                        Text = "1";
                                    }
                                }
                                else
                                {
                                    if ( Droid.Turret1.TurretType == enumTurretType.Weapon )
                                    {
                                        Text = Droid.TurretCount.ToStringInvariant();
                                    }
                                    else
                                    {
                                        Text = "0";
                                    }
                                }
                            }
                            File.AppendProperty("weapons", Text);
                            File.AppendProperty("parts\\body", Droid.Body.Code);
                            File.AppendProperty("parts\\propulsion", Droid.Propulsion.Code);
                            File.AppendProperty("parts\\sensor", Droid.GetSensorCode());
                            File.AppendProperty("parts\\construct", Droid.GetConstructCode());
                            File.AppendProperty("parts\\repair", Droid.GetRepairCode());
                            File.AppendProperty("parts\\brain", Droid.GetBrainCode());
                            File.AppendProperty("parts\\ecm", Droid.GetECMCode());
                            if ( Droid.TurretCount >= 1 )
                            {
                                if ( Droid.Turret1.TurretType == enumTurretType.Weapon )
                                {
                                    File.AppendProperty("parts\\weapon\\1", Droid.Turret1.Code);
                                    if ( Droid.TurretCount >= 2 )
                                    {
                                        if ( Droid.Turret2.TurretType == enumTurretType.Weapon )
                                        {
                                            File.AppendProperty("parts\\weapon\\2", Droid.Turret2.Code);
                                            if ( Droid.TurretCount >= 3 )
                                            {
                                                if ( Droid.Turret3.TurretType == enumTurretType.Weapon )
                                                {
                                                    File.AppendProperty("parts\\weapon\\3", Droid.Turret3.Code);
                                                }
                                            }
                                        }
                                    }
                                }
                                else if ( Droid.Turret1.TurretType == enumTurretType.Brain )
                                {
                                    Brain = (Brain)Droid.Turret1;
                                    if ( Brain.Weapon == null )
                                    {
                                        Text = "ZNULLWEAPON";
                                    }
                                    else
                                    {
                                        Text = Brain.Weapon.Code;
                                    }
                                    File.AppendProperty("parts\\weapon\\1", Text);
                                }
                            }
                        }
                        File.Gap_Append();
                    }
                }
            }

            if ( InvalidPartCount > 0 )
            {
                ReturnResult.WarningAdd("There were " + Convert.ToString(InvalidPartCount) + " droids with parts missing. They were not saved.");
            }

            return ReturnResult;
        }

        public clsResult Serialize_WZ_FeaturesINI(IniWriter File)
        {
            clsResult ReturnResult = new clsResult("Serializing features INI", false);
            logger.Info ("Serializing features INI");
            FeatureTypeBase featureTypeBase = default(FeatureTypeBase);
            clsUnit Unit = default(clsUnit);
            bool Valid = default(bool);

            foreach ( clsUnit tempLoopVar_Unit in Units )
            {
                Unit = tempLoopVar_Unit;
                if ( Unit.TypeBase.Type == UnitType.Feature )
                {
                    featureTypeBase = (FeatureTypeBase)Unit.TypeBase;
                    Valid = true;
                    if ( Unit.ID <= 0 )
                    {
                        Valid = false;
                        ReturnResult.WarningAdd("Error. A features\'s ID was zero. It was NOT saved. Delete and replace it to allow save.");
                    }
                    if ( Valid )
                    {
                        File.AppendSectionName("feature_" + Unit.ID.ToStringInvariant());
                        File.AppendProperty("id", Unit.ID.ToStringInvariant());
                        File.AppendProperty("position", Unit.GetINIPosition());
                        File.AppendProperty("rotation", Unit.GetINIRotation());
                        File.AppendProperty("name", featureTypeBase.Code);
                        if ( Unit.Health < 1.0D )
                        {
                            File.AppendProperty("health", Unit.GetINIHealthPercent());
                        }
                        File.Gap_Append();
                    }
                }
            }

            return ReturnResult;
        }

        public clsResult Serialize_WZ_LabelsINI(IniWriter File, int PlayerCount)
        {
            clsResult ReturnResult = new clsResult("Serializing labels INI", false);
            logger.Info ("Serializing labels INI");

            try
            {
                clsScriptPosition ScriptPosition = default(clsScriptPosition);
                foreach ( clsScriptPosition tempLoopVar_ScriptPosition in ScriptPositions )
                {
                    ScriptPosition = tempLoopVar_ScriptPosition;
                    ScriptPosition.WriteWZ(File);
                }
                clsScriptArea ScriptArea = default(clsScriptArea);
                foreach ( clsScriptArea tempLoopVar_ScriptArea in ScriptAreas )
                {
                    ScriptArea = tempLoopVar_ScriptArea;
                    ScriptArea.WriteWZ(File);
                }
                if ( PlayerCount >= 0 ) //not an FMap
                {
                    clsUnit Unit = default(clsUnit);
                    foreach ( clsUnit tempLoopVar_Unit in Units )
                    {
                        Unit = tempLoopVar_Unit;
                        Unit.WriteWZLabel(File, PlayerCount);
                    }
                }
            }
            catch ( Exception ex )
            {
                ReturnResult.WarningAdd(ex.Message);
				logger.ErrorException ("Got an exception", ex);
            }

            return ReturnResult;
        }

        public clsResult Write_WZ(sWrite_WZ_Args Args)
        {
            clsResult ReturnResult =
                new clsResult("Compiling to \"{0}\"".Format2(Args.Path), false);
            logger.Info ("Compiling to \"{0}\"".Format2(Args.Path));

            try
            {
                switch ( Args.CompileType )
                {
                    case sWrite_WZ_Args.enumCompileType.Multiplayer:
                        if ( Args.Multiplayer == null )
                        {
                            ReturnResult.ProblemAdd("Multiplayer arguments were not passed.");
                            return ReturnResult;
                        }
                        if ( Args.Multiplayer.PlayerCount < 2 | Args.Multiplayer.PlayerCount > 10 )
                        {
                            ReturnResult.ProblemAdd("Number of players was below 2 or above 10.");
                            return ReturnResult;
                        }
                        if ( !Args.Multiplayer.IsBetaPlayerFormat )
                        {
                            if ( !(Args.Multiplayer.PlayerCount == 2 | Args.Multiplayer.PlayerCount == 4 | Args.Multiplayer.PlayerCount == 8) )
                            {
                                ReturnResult.ProblemAdd("Number of players was not 2, 4 or 8 in original format.");
                                return ReturnResult;
                            }
                        }
                        break;
                    case sWrite_WZ_Args.enumCompileType.Campaign:
                        if ( Args.Campaign == null )
                        {
                            ReturnResult.ProblemAdd("Campaign arguments were not passed.");
                            return ReturnResult;
                        }
                        break;
                    default:
                        ReturnResult.ProblemAdd("Unknown compile method.");
                        return ReturnResult;
                }

                if ( !Args.Overwrite )
                {
                    if ( File.Exists(Args.Path) )
                    {
                        ReturnResult.ProblemAdd("The selected file already exists.");
                        return ReturnResult;
                    }
                }

                char Quote = '\"';
                char EndChar = '\n';
                string Text = "";

                MemoryStream File_LEV_Memory = new MemoryStream();
                StreamWriter File_LEV = new StreamWriter(File_LEV_Memory, App.UTF8Encoding);
                MemoryStream File_MAP_Memory = new MemoryStream();
                BinaryWriter File_MAP = new BinaryWriter(File_MAP_Memory, App.ASCIIEncoding);
                MemoryStream File_GAM_Memory = new MemoryStream();
                BinaryWriter File_GAM = new BinaryWriter(File_GAM_Memory, App.ASCIIEncoding);
                MemoryStream File_featBJO_Memory = new MemoryStream();
                BinaryWriter File_featBJO = new BinaryWriter(File_featBJO_Memory, App.ASCIIEncoding);
                MemoryStream INI_feature_Memory = new MemoryStream();
                IniWriter INI_feature = IniWriter.CreateFile(INI_feature_Memory);
                MemoryStream File_TTP_Memory = new MemoryStream();
                BinaryWriter File_TTP = new BinaryWriter(File_TTP_Memory, App.ASCIIEncoding);
                MemoryStream File_structBJO_Memory = new MemoryStream();
                BinaryWriter File_structBJO = new BinaryWriter(File_structBJO_Memory, App.ASCIIEncoding);
                MemoryStream INI_struct_Memory = new MemoryStream();
                IniWriter INI_struct = IniWriter.CreateFile(INI_struct_Memory);
                MemoryStream File_droidBJO_Memory = new MemoryStream();
                BinaryWriter File_droidBJO = new BinaryWriter(File_droidBJO_Memory, App.ASCIIEncoding);
                MemoryStream INI_droid_Memory = new MemoryStream();
                IniWriter INI_droid = IniWriter.CreateFile(INI_droid_Memory);
                MemoryStream INI_Labels_Memory = new MemoryStream();
                IniWriter INI_Labels = IniWriter.CreateFile(INI_Labels_Memory);

                string PlayersPrefix = "";
                string PlayersText = "";

                if ( Args.CompileType == sWrite_WZ_Args.enumCompileType.Multiplayer )
                {
                    PlayersText = Args.Multiplayer.PlayerCount.ToStringInvariant();
                    PlayersPrefix = PlayersText + "c-";
                    string fog = "";
                    string TilesetNum = "";
                    if ( Tileset == null )
                    {
                        ReturnResult.ProblemAdd("Map must have a tileset.");
                        return ReturnResult;
                    }
                    else if ( Tileset == App.Tileset_Arizona )
                    {
                        fog = "fog1.wrf";
                        TilesetNum = "1";
                    }
                    else if ( Tileset == App.Tileset_Urban )
                    {
                        fog = "fog2.wrf";
                        TilesetNum = "2";
                    }
                    else if ( Tileset == App.Tileset_Rockies )
                    {
                        fog = "fog3.wrf";
                        TilesetNum = "3";
                    }
                    else
                    {
                        ReturnResult.ProblemAdd("Unknown tileset selected.");
                        return ReturnResult;
                    }

                    Text = "// Made with " + Constants.ProgramName + " " + Constants.ProgramVersionNumber + " " + Constants.ProgramPlatform +
                           Convert.ToString(EndChar);
                    File_LEV.Write(Text);
                    DateTime DateNow = DateTime.Now;
                    Text = "// Date: " + Convert.ToString(DateNow.Year) + "/" + App.MinDigits(DateNow.Month, 2) + "/" +
                           App.MinDigits(DateNow.Day, 2) + " " + App.MinDigits(DateNow.Hour, 2) + ":" + App.MinDigits(DateNow.Minute, 2) + ":" +
                           App.MinDigits(DateNow.Second, 2) + Convert.ToString(EndChar);
                    File_LEV.Write(Text);
                    Text = "// Author: " + Args.Multiplayer.AuthorName + Convert.ToString(EndChar);
                    File_LEV.Write(Text);
                    Text = "// License: " + Args.Multiplayer.License + Convert.ToString(EndChar);
                    File_LEV.Write(Text);
                    Text = EndChar.ToString();
                    File_LEV.Write(Text);
                    Text = "level   " + Args.MapName + "-T1" + Convert.ToString(EndChar);
                    File_LEV.Write(Text);
                    Text = "players " + PlayersText + Convert.ToString(EndChar);
                    File_LEV.Write(Text);
                    Text = "type    14" + Convert.ToString(EndChar);
                    File_LEV.Write(Text);
                    Text = "dataset MULTI_CAM_" + TilesetNum + Convert.ToString(EndChar);
                    File_LEV.Write(Text);
                    Text = "game    " + Convert.ToString(Quote) + "multiplay/maps/" + PlayersPrefix + Args.MapName + ".gam" + Convert.ToString(Quote) +
                           Convert.ToString(EndChar);
                    File_LEV.Write(Text);
                    Text = "data    " + Convert.ToString(Quote) + "wrf/multi/skirmish" + PlayersText + ".wrf" + Convert.ToString(Quote) +
                           Convert.ToString(EndChar);
                    File_LEV.Write(Text);
                    Text = "data    " + Convert.ToString(Quote) + "wrf/multi/" + fog + Convert.ToString(Quote) + Convert.ToString(EndChar);
                    File_LEV.Write(Text);
                    Text = EndChar.ToString();
                    File_LEV.Write(Text);
                    Text = "level   " + Args.MapName + "-T2" + Convert.ToString(EndChar);
                    File_LEV.Write(Text);
                    Text = "players " + PlayersText + Convert.ToString(EndChar);
                    File_LEV.Write(Text);
                    Text = "type    18" + Convert.ToString(EndChar);
                    File_LEV.Write(Text);
                    Text = "dataset MULTI_T2_C" + TilesetNum + Convert.ToString(EndChar);
                    File_LEV.Write(Text);
                    Text = "game    " + Convert.ToString(Quote) + "multiplay/maps/" + PlayersPrefix + Args.MapName + ".gam" + Convert.ToString(Quote) +
                           Convert.ToString(EndChar);
                    File_LEV.Write(Text);
                    Text = "data    " + Convert.ToString(Quote) + "wrf/multi/t2-skirmish" + PlayersText + ".wrf" + Convert.ToString(Quote) +
                           Convert.ToString(EndChar);
                    File_LEV.Write(Text);
                    Text = "data    " + Convert.ToString(Quote) + "wrf/multi/" + fog + Convert.ToString(Quote) + Convert.ToString(EndChar);
                    File_LEV.Write(Text);
                    Text = EndChar.ToString();
                    File_LEV.Write(Text);
                    Text = "level   " + Args.MapName + "-T3" + Convert.ToString(EndChar);
                    File_LEV.Write(Text);
                    Text = "players " + PlayersText + Convert.ToString(EndChar);
                    File_LEV.Write(Text);
                    Text = "type    19" + Convert.ToString(EndChar);
                    File_LEV.Write(Text);
                    Text = "dataset MULTI_T3_C" + TilesetNum + Convert.ToString(EndChar);
                    File_LEV.Write(Text);
                    Text = "game    " + Convert.ToString(Quote) + "multiplay/maps/" + PlayersPrefix + Args.MapName + ".gam" + Convert.ToString(Quote) +
                           Convert.ToString(EndChar);
                    File_LEV.Write(Text);
                    Text = "data    " + Convert.ToString(Quote) + "wrf/multi/t3-skirmish" + PlayersText + ".wrf" + Convert.ToString(Quote) +
                           Convert.ToString(EndChar);
                    File_LEV.Write(Text);
                    Text = "data    " + Convert.ToString(Quote) + "wrf/multi/" + fog + Convert.ToString(Quote) + Convert.ToString(EndChar);
                    File_LEV.Write(Text);
                }

                byte[] GameZeroBytes = new byte[20];

                IOUtil.WriteText(File_GAM, false, "game");
                File_GAM.Write(8U);
                File_GAM.Write(0U); //Time
                if ( Args.CompileType == sWrite_WZ_Args.enumCompileType.Multiplayer )
                {
                    File_GAM.Write(0U);
                }
                else if ( Args.CompileType == sWrite_WZ_Args.enumCompileType.Campaign )
                {
                    File_GAM.Write(Args.Campaign.GAMType);
                }
                File_GAM.Write(Args.ScrollMin.X);
                File_GAM.Write(Args.ScrollMin.Y);
                File_GAM.Write(Args.ScrollMax.X);
                File_GAM.Write(Args.ScrollMax.Y);
                File_GAM.Write(GameZeroBytes);

                int A = 0;
                int X = 0;
                int Y = 0;

                IOUtil.WriteText(File_MAP, false, "map ");
                File_MAP.Write(10U);
                File_MAP.Write((uint)Terrain.TileSize.X);
                File_MAP.Write((uint)Terrain.TileSize.Y);
                byte Flip = 0;
                byte Rotation = 0;
                bool DoFlipX = default(bool);
                int InvalidTileCount = 0;
                int TextureNum = 0;
                for ( Y = 0; Y <= Terrain.TileSize.Y - 1; Y++ )
                {
                    for ( X = 0; X <= Terrain.TileSize.X - 1; X++ )
                    {
                        TileUtil.TileOrientation_To_OldOrientation(Terrain.Tiles[X, Y].Texture.Orientation, ref Rotation, ref DoFlipX);
                        Flip = (byte)0;
                        if ( Terrain.Tiles[X, Y].Tri )
                        {
                            Flip += (byte)8;
                        }
                        Flip += (byte)(Rotation * 16);
                        if ( DoFlipX )
                        {
                            Flip += (byte)128;
                        }
                        TextureNum = Terrain.Tiles[X, Y].Texture.TextureNum;
                        if ( TextureNum < 0 | TextureNum > 255 )
                        {
                            TextureNum = 0;
                            if ( InvalidTileCount < 16 )
                            {
                                ReturnResult.WarningAdd("Tile texture number " + Convert.ToString(Terrain.Tiles[X, Y].Texture.TextureNum) +
                                                        " is invalid on tile " + Convert.ToString(X) + ", " + Convert.ToString(Y) +
                                                        " and was compiled as texture number " + Convert.ToString(TextureNum) + ".");
                            }
                            InvalidTileCount++;
                        }
                        File_MAP.Write((byte)TextureNum);
                        File_MAP.Write(Flip);
                        File_MAP.Write(Terrain.Vertices[X, Y].Height);
                    }
                }
                if ( InvalidTileCount > 0 )
                {
                    ReturnResult.WarningAdd(InvalidTileCount + " tile texture numbers were invalid.");
                }
                File_MAP.Write(1U); //gateway version
                File_MAP.Write((uint)Gateways.Count);
                foreach ( clsGateway gateway in Gateways )
                {
                    File_MAP.Write((byte)(MathUtil.Clamp_int(gateway.PosA.X, 0, 255)));
                    File_MAP.Write((byte)(MathUtil.Clamp_int(gateway.PosA.Y, 0, 255)));
                    File_MAP.Write((byte)(MathUtil.Clamp_int(gateway.PosB.X, 0, 255)));
                    File_MAP.Write((byte)(MathUtil.Clamp_int(gateway.PosB.Y, 0, 255)));
                }

                FeatureTypeBase featureTypeBase;
                StructureTypeBase structureTypeBase;
                DroidDesign DroidType;
                DroidTemplate DroidTemplate;
                clsUnit Unit;
                clsStructureWriteWZ StructureWrite = new clsStructureWriteWZ
                    {
                        File = File_structBJO, 
                        CompileType = Args.CompileType
                    };
                if ( Args.CompileType == sWrite_WZ_Args.enumCompileType.Multiplayer )
                {
                    StructureWrite.PlayerCount = Args.Multiplayer.PlayerCount;
                }
                else
                {
                    StructureWrite.PlayerCount = 0;
                }

                byte[] FeatZeroBytes = new byte[12];

                IOUtil.WriteText(File_featBJO, false, "feat");
                File_featBJO.Write(8U);
                clsObjectPriorityOrderList FeatureOrder = new clsObjectPriorityOrderList();
                foreach ( clsUnit tempLoopVar_Unit in Units )
                {
                    Unit = tempLoopVar_Unit;
                    if ( Unit.TypeBase.Type == UnitType.Feature )
                    {
                        FeatureOrder.SetItem(Unit);
                        FeatureOrder.ActionPerform();
                    }
                }
                File_featBJO.Write((uint)FeatureOrder.Result.Count);
                for ( A = 0; A <= FeatureOrder.Result.Count - 1; A++ )
                {
                    Unit = FeatureOrder.Result[A];
                    featureTypeBase = (FeatureTypeBase)Unit.TypeBase;
                    IOUtil.WriteTextOfLength(File_featBJO, 40, featureTypeBase.Code);
                    File_featBJO.Write(Unit.ID);
                    File_featBJO.Write((uint)Unit.Pos.Horizontal.X);
                    File_featBJO.Write((uint)Unit.Pos.Horizontal.Y);
                    File_featBJO.Write((uint)Unit.Pos.Altitude);
                    File_featBJO.Write((uint)Unit.Rotation);
                    switch ( Args.CompileType )
                    {
                        case sWrite_WZ_Args.enumCompileType.Multiplayer:
                            File_featBJO.Write(Unit.GetBJOMultiplayerPlayerNum(Args.Multiplayer.PlayerCount));
                            break;
                        case sWrite_WZ_Args.enumCompileType.Campaign:
                            File_featBJO.Write(Unit.GetBJOCampaignPlayerNum());
                            break;
                        default:
                            Debugger.Break();
                            break;
                    }
                    File_featBJO.Write(FeatZeroBytes);
                }

                IOUtil.WriteText(File_TTP, false, "ttyp");
                File_TTP.Write(8U);
                File_TTP.Write((uint)Tileset.TileCount);
                for ( A = 0; A <= Tileset.TileCount - 1; A++ )
                {
                    File_TTP.Write((ushort)Tile_TypeNum[A]);
                }

                IOUtil.WriteText(File_structBJO, false, "stru");
                File_structBJO.Write(8U);
                clsObjectPriorityOrderList NonModuleStructureOrder = new clsObjectPriorityOrderList();
                //non-module structures
                foreach ( clsUnit tempLoopVar_Unit in Units )
                {
                    Unit = tempLoopVar_Unit;
                    if ( Unit.TypeBase.Type == UnitType.PlayerStructure )
                    {
                        structureTypeBase = (StructureTypeBase)Unit.TypeBase;
                        if ( !structureTypeBase.IsModule() )
                        {
                            NonModuleStructureOrder.SetItem(Unit);
                            NonModuleStructureOrder.ActionPerform();
                        }
                    }
                }
                clsObjectPriorityOrderList ModuleStructureOrder = new clsObjectPriorityOrderList();
                //module structures
                foreach ( clsUnit tempLoopVar_Unit in Units )
                {
                    Unit = tempLoopVar_Unit;
                    if ( Unit.TypeBase.Type == UnitType.PlayerStructure )
                    {
                        structureTypeBase = (StructureTypeBase)Unit.TypeBase;
                        if ( structureTypeBase.IsModule() )
                        {
                            ModuleStructureOrder.SetItem(Unit);
                            ModuleStructureOrder.ActionPerform();
                        }
                    }
                }
                File_structBJO.Write((uint)(NonModuleStructureOrder.Result.Count + ModuleStructureOrder.Result.Count));
                NonModuleStructureOrder.Result.PerformTool(StructureWrite);
                ModuleStructureOrder.Result.PerformTool(StructureWrite);

                byte[] DintZeroBytes = new byte[12];

                IOUtil.WriteText(File_droidBJO, false, "dint");
                File_droidBJO.Write(8U);
                clsObjectPriorityOrderList Droids = new clsObjectPriorityOrderList();
                foreach ( clsUnit tempLoopVar_Unit in Units )
                {
                    Unit = tempLoopVar_Unit;
                    if ( Unit.TypeBase.Type == UnitType.PlayerDroid )
                    {
                        DroidType = (DroidDesign)Unit.TypeBase;
                        if ( DroidType.IsTemplate )
                        {
                            Droids.SetItem(Unit);
                            Droids.ActionPerform();
                        }
                    }
                }
                File_droidBJO.Write((uint)Droids.Result.Count);
                for ( A = 0; A <= Droids.Result.Count - 1; A++ )
                {
                    Unit = Droids.Result[A];
                    DroidTemplate = (DroidTemplate)Unit.TypeBase;
                    IOUtil.WriteTextOfLength(File_droidBJO, 40, DroidTemplate.Code);
                    File_droidBJO.Write(Unit.ID);
                    File_droidBJO.Write((uint)Unit.Pos.Horizontal.X);
                    File_droidBJO.Write((uint)Unit.Pos.Horizontal.Y);
                    File_droidBJO.Write((uint)Unit.Pos.Altitude);
                    File_droidBJO.Write((uint)Unit.Rotation);
                    switch ( Args.CompileType )
                    {
                        case sWrite_WZ_Args.enumCompileType.Multiplayer:
                            File_droidBJO.Write(Unit.GetBJOMultiplayerPlayerNum(Args.Multiplayer.PlayerCount));
                            break;
                        case sWrite_WZ_Args.enumCompileType.Campaign:
                            File_droidBJO.Write(Unit.GetBJOCampaignPlayerNum());
                            break;
                        default:
                            Debugger.Break();
                            break;
                    }
                    File_droidBJO.Write(DintZeroBytes);
                }

                ReturnResult.Add(Serialize_WZ_FeaturesINI(INI_feature));
                if ( Args.CompileType == sWrite_WZ_Args.enumCompileType.Multiplayer )
                {
                    ReturnResult.Add(Serialize_WZ_StructuresINI(INI_struct, Args.Multiplayer.PlayerCount));
                    ReturnResult.Add(Serialize_WZ_DroidsINI(INI_droid, Args.Multiplayer.PlayerCount));
                    ReturnResult.Add(Serialize_WZ_LabelsINI(INI_Labels, Args.Multiplayer.PlayerCount));
                }
                else if ( Args.CompileType == sWrite_WZ_Args.enumCompileType.Campaign )
                {
                    ReturnResult.Add(Serialize_WZ_StructuresINI(INI_struct, -1));
                    ReturnResult.Add(Serialize_WZ_DroidsINI(INI_droid, -1));
                    ReturnResult.Add(Serialize_WZ_LabelsINI(INI_Labels, 0)); //interprets -1 players as an FMap
                }

                File_LEV.Flush();
                File_MAP.Flush();
                File_GAM.Flush();
                File_featBJO.Flush();
                INI_feature.File.Flush();
                File_TTP.Flush();
                File_structBJO.Flush();
                INI_struct.File.Flush();
                File_droidBJO.Flush();
                INI_droid.File.Flush();
                INI_Labels.File.Flush();

                if ( Args.CompileType == sWrite_WZ_Args.enumCompileType.Multiplayer )
                {
                    if ( !Args.Overwrite )
                    {
                        if ( File.Exists(Args.Path) )
                        {
                            ReturnResult.ProblemAdd("A file already exists at: " + Args.Path);
                            return ReturnResult;
                        }
                    }
   
                    try
                    {
                        using (var zip = new ZipOutputStream(Args.Path)) 
                        {
                            // Set encoding
                            zip.AlternateEncoding = System.Text.Encoding.GetEncoding ("UTF-8");
                            zip.AlternateEncodingUsage = ZipOption.Always;

                            // Set compression
                            zip.CompressionLevel = Ionic.Zlib.CompressionLevel.BestCompression;

                            // .addon.lev / .xplayers.lev
                            string zipPath = "";
                            if ( Args.Multiplayer.IsBetaPlayerFormat )
                            {
                                zipPath = PlayersPrefix + Args.MapName + ".xplayers.lev";
                            }
                            else
                            {
                                zipPath = PlayersPrefix + Args.MapName + ".addon.lev";
                            }
                            zip.PutNextEntry (zipPath);
                            File_LEV_Memory.WriteTo(zip);                       
                            File_LEV_Memory.Flush();

                            zip.PutNextEntry("multiplay/maps/" + PlayersPrefix + Args.MapName + ".gam");
                            File_GAM_Memory.WriteTo(zip);
                            File_GAM_Memory.Flush();

                            zip.PutNextEntry("multiplay/maps/" + PlayersPrefix + Args.MapName + "/" + "dinit.bjo");
                            File_droidBJO_Memory.WriteTo(zip);
                            File_droidBJO_Memory.Flush();

                            zip.PutNextEntry("multiplay/maps/" + PlayersPrefix + Args.MapName + "/" + "droid.ini");
                            INI_droid_Memory.WriteTo(zip);
                            INI_droid_Memory.Flush();

                            zip.PutNextEntry("multiplay/maps/" + PlayersPrefix + Args.MapName + "/" + "feat.bjo");
                            File_featBJO_Memory.WriteTo(zip);
                            File_featBJO_Memory.Flush();

                            zip.PutNextEntry("multiplay/maps/" + PlayersPrefix + Args.MapName + "/" + "feature.ini");
                            INI_feature_Memory.WriteTo(zip);
                            INI_feature_Memory.Flush();

                            zip.PutNextEntry("multiplay/maps/" + PlayersPrefix + Args.MapName + "/" + "game.map");
                            File_MAP_Memory.WriteTo(zip);
                            File_MAP_Memory.Flush();

                            zip.PutNextEntry("multiplay/maps/" + PlayersPrefix + Args.MapName + "/" + "struct.bjo");
                            File_structBJO_Memory.WriteTo(zip);
                            File_structBJO_Memory.Flush();

                            zip.PutNextEntry("multiplay/maps/" + PlayersPrefix + Args.MapName + "/" + "struct.ini");
                            INI_struct_Memory.WriteTo(zip);
                            INI_struct_Memory.Flush();

                            zip.PutNextEntry("multiplay/maps/" + PlayersPrefix + Args.MapName + "/" + "ttypes.ttp");
                            File_TTP_Memory.WriteTo(zip);
                            File_TTP_Memory.Flush();

                            if ( INI_Labels_Memory.Length > 0 ) 
                            {
                                zip.PutNextEntry("multiplay/maps/" + PlayersPrefix + Args.MapName + "/" + "labels.ini");
                                INI_Labels_Memory.WriteTo(zip);
                                INI_Labels_Memory.Flush();
                            }
                        }                    
                    }
                    catch ( Exception ex )
                    {
                        ReturnResult.ProblemAdd(ex.Message);
						logger.ErrorException ("Got an exception", ex);
                        return ReturnResult;
                    }

                    return ReturnResult;
                }
                else if ( Args.CompileType == sWrite_WZ_Args.enumCompileType.Campaign )
                {
                    string CampDirectory = PathUtil.EndWithPathSeperator(Args.Path);

                    if ( !Directory.Exists(CampDirectory) )
                    {
                        ReturnResult.ProblemAdd("Directory " + CampDirectory + " does not exist.");
                        return ReturnResult;
                    }

                    string FilePath = "";

                    FilePath = CampDirectory + Args.MapName + ".gam";
                    ReturnResult.Add(IOUtil.WriteMemoryToNewFile(File_GAM_Memory, CampDirectory + Args.MapName + ".gam"));

                    CampDirectory += Args.MapName + Convert.ToString(App.PlatformPathSeparator);
                    try
                    {
                        Directory.CreateDirectory(CampDirectory);
                    }
                    catch ( Exception ex)
                    {
                        ReturnResult.ProblemAdd("Unable to create directory " + CampDirectory);
						logger.ErrorException ("Got an exception", ex);
                        return ReturnResult;
                    }

                    FilePath = CampDirectory + "dinit.bjo";
                    ReturnResult.Add(IOUtil.WriteMemoryToNewFile(File_droidBJO_Memory, FilePath));

                    FilePath = CampDirectory + "droid.ini";
                    ReturnResult.Add(IOUtil.WriteMemoryToNewFile(INI_droid_Memory, FilePath));

                    FilePath = CampDirectory + "feat.bjo";
                    ReturnResult.Add(IOUtil.WriteMemoryToNewFile(File_featBJO_Memory, FilePath));

                    FilePath = CampDirectory + "feature.ini";
                    ReturnResult.Add(IOUtil.WriteMemoryToNewFile(INI_feature_Memory, FilePath));

                    FilePath = CampDirectory + "game.map";
                    ReturnResult.Add(IOUtil.WriteMemoryToNewFile(File_MAP_Memory, FilePath));

                    FilePath = CampDirectory + "struct.bjo";
                    ReturnResult.Add(IOUtil.WriteMemoryToNewFile(File_structBJO_Memory, FilePath));

                    FilePath = CampDirectory + "struct.ini";
                    ReturnResult.Add(IOUtil.WriteMemoryToNewFile(INI_struct_Memory, FilePath));

                    FilePath = CampDirectory + "ttypes.ttp";
                    ReturnResult.Add(IOUtil.WriteMemoryToNewFile(File_TTP_Memory, FilePath));

                    FilePath = CampDirectory + "labels.ini";
                    ReturnResult.Add(IOUtil.WriteMemoryToNewFile(INI_Labels_Memory, FilePath));
                }
            }
            catch ( Exception ex )
            {
                Debugger.Break();
                ReturnResult.ProblemAdd(ex.Message);
				logger.ErrorException ("Got an exception", ex);
                return ReturnResult;
            }

            return ReturnResult;
        }

        private sResult Read_TTP(BinaryReader File)
        {
            sResult ReturnResult = new sResult();
            ReturnResult.Success = false;
            ReturnResult.Problem = "";

            string strTemp = "";
            UInt32 uintTemp = 0;
            UInt16 ushortTemp = 0;
            int A = 0;

            try
            {
                strTemp = IOUtil.ReadOldTextOfLength(File, 4);
                if ( strTemp != "ttyp" )
                {
                    ReturnResult.Problem = "Incorrect identifier.";
                    return ReturnResult;
                }

                uintTemp = File.ReadUInt32();
                if ( uintTemp != 8U )
                {
                    ReturnResult.Problem = "Unknown version.";
                    return ReturnResult;
                }
                uintTemp = File.ReadUInt32();
                for ( A = 0; A <= ((int)(Math.Min(uintTemp, (uint)Tileset.TileCount))) - 1; A++ )
                {
                    ushortTemp = File.ReadUInt16();
                    if ( ushortTemp > 11 )
                    {
                        ReturnResult.Problem = "Unknown tile type number.";
                        return ReturnResult;
                    }
                    Tile_TypeNum[A] = (byte)ushortTemp;
                }
            }
            catch ( Exception ex )
            {
                ReturnResult.Problem = ex.Message;
				logger.ErrorException ("Got an exception", ex);
                return ReturnResult;
            }

            ReturnResult.Success = true;
            return ReturnResult;
        }
    }
}
