using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
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

            using (var zip = ZipFile.Read(path))
            {
                foreach (ZipEntry e in zip)
                {
                    if (e.IsDirectory)
                    {
                        continue;
                    }                  

                    splitPath = new ZipSplitPath (e.FileName);
                    logger.Debug ("Found file \"{0}\".", e.FileName);
                    // Find the maps .lev
                    if (splitPath.FileExtension != "lev" || splitPath.PartCount != 1)
                    {
                        continue;
                    }

                    // Buggy file > 1MB
                    if (e.UncompressedSize > 1 * 1024 * 1024)
                    {
                        returnResult.ProblemAdd ("lev file is too large.");
                        return returnResult;
                    }

                    using (var s = e.OpenReader())
                    {
                        var myresult = new clsResult (string.Format ("Parsing file \"{0}\"", e.FileName), false);
                        logger.Info ("Parsing file \"{0}\"", e.FileName);

                        try
                        {
                            var r = new StreamReader (s);
                            var text = r.ReadToEnd ();
                            var levFile = LevGrammar.Lev.Parse (text);

                            if (levFile.Levels.Count < 1)
                            {
                                myresult.ProblemAdd ("No maps found in file.");
                                returnResult.Add (myresult);
                                return returnResult;
                            }

                            // Group games by the Game key.
                            var groupGames = levFile.Levels.GroupBy (level => level.Game);

                            // Load default map if only one Game file is found.
                            if (groupGames.Count () == 1)
                            {
                                var level = groupGames.First ().First (); //first group, first level

                                mapLoadName = level.Game;

                                switch (level.Dataset.Substring (level.Dataset.Length - 1, 1))
                                {
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
                            } else
                            {
                                //prompt user for which of the entries to load
                                frmWZLoad.clsOutput selectToLoadResult = new frmWZLoad.clsOutput ();

                                var names = groupGames
                                    .Select (gameGroup => gameGroup.First ().Name)
                                    .ToArray ();

                                frmWZLoad selectToLoadForm = new frmWZLoad (names, selectToLoadResult,
                                                    "Select a map from " + new sSplitPath (path).FileTitle);
                                selectToLoadForm.ShowDialog ();
                                if (selectToLoadResult.Result < 0)
                                {
                                    returnResult.ProblemAdd ("No map selected.");
                                    return returnResult;
                                }

                                var level = groupGames.ElementAt (selectToLoadResult.Result).First ();
                                
                                mapLoadName = level.Game;

                                switch (level.Dataset.Substring (level.Dataset.Length - 1, 1))
                                {
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
                        } catch (Exception ex)
                        {
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
                if (gameZipEntry == null)
                {
                    returnResult.ProblemAdd (string.Format ("Game file \"{0}\" not found.", mapLoadName), false);
                    logger.Error ("Game file \"{0}\" not found.", mapLoadName);
                    return returnResult;
                }
                using (Stream s = gameZipEntry.OpenReader())
                {              
                    BinaryReader reader = new BinaryReader (s);
                    subResult = Read_WZ_gam (reader);
                    reader.Close ();
                    if (!subResult.Success)
                    {
                        returnResult.ProblemAdd (subResult.Problem);
                        return returnResult;
                    }
                }


                var gameMapZipEntry = zip [gameFilesPath + "game.map"];
                if (gameMapZipEntry == null)
                {
                    returnResult.ProblemAdd (string.Format ("{0}game.map file not found", gameFilesPath));
                    return returnResult;
                }
                using (Stream s = gameMapZipEntry.OpenReader())
                {              
                    BinaryReader reader = new BinaryReader (s);
                    subResult = Read_WZ_map (reader);
                    reader.Close ();
                    if (!subResult.Success)
                    {
                        returnResult.ProblemAdd (subResult.Problem);
                        return returnResult;
                    }
                }

                SimpleClassList<clsWZBJOUnit> BJOUnits = new SimpleClassList<clsWZBJOUnit> ();

                IniFeatures iniFeatures = null;
                var featureIniZipEntry = zip [gameFilesPath + "feature.ini"];
                if (featureIniZipEntry != null)
                {
                    iniFeatures = new IniFeatures ();
                    using (var reader = new StreamReader (featureIniZipEntry.OpenReader()))
                    {
                        var text = reader.ReadToEnd ();
                        returnResult.Add (read_INI_Features (text, ref iniFeatures));
                    }
                }
                              
                if (iniFeatures == null)
                {
                    clsResult Result = new clsResult ("feat.bjo", false);
                    logger.Info ("Loading feat.bjo");

                    var featBJOZipEntry = zip [gameFilesPath + "feat.bjo"];
                    if (featBJOZipEntry == null)
                    {
                        Result.WarningAdd (string.Format ("{0}feat.bjo / feature.ini file not found", gameFilesPath));
                    } else
                    {
                        using (Stream s = featBJOZipEntry.OpenReader())
                        {
                            BinaryReader reader = new BinaryReader (s);
                            subResult = Read_WZ_Features (reader, BJOUnits);
                            reader.Close ();
                            if (!subResult.Success)
                            {
                                Result.WarningAdd (subResult.Problem);
                            }
                        }
                    }
                    returnResult.Add (Result);
                }

                clsResult result = new clsResult ("ttypes.ttp", false);
                logger.Info ("Loading ttypes.ttp");
                var ttypesEntry = zip [gameFilesPath + "ttypes.ttp"];
                if (ttypesEntry == null)
                {
                    result.WarningAdd (string.Format ("{0}ttypes.ttp file not found", gameFilesPath));
                } else
                {
                    using (Stream s = ttypesEntry.OpenReader())
                    {
                        BinaryReader reader = new BinaryReader (s);
                        subResult = Read_WZ_TileTypes (reader);
                        reader.Close ();
                        if (!subResult.Success)
                        {
                            result.WarningAdd (subResult.Problem);
                        }
                    }
                }
                returnResult.Add (result);

                IniStructures iniStructures = null;
                var structIniEntry = zip [gameFilesPath + "struct.ini"];
                if (structIniEntry != null)
                {
                    iniStructures = new IniStructures ();
                    using (var reader = new StreamReader (structIniEntry.OpenReader()))
                    {
                        var text = reader.ReadToEnd ();
                        returnResult.Add (read_INI_Structures (text, ref iniStructures));
                    }
                }

                if (iniStructures == null)
                {
                    clsResult Result = new clsResult ("struct.bjo", false);
                    logger.Info ("Loading struct.bjo");
                    var structBjoEntry = zip [gameFilesPath + "struct.bjo"];
                    if (structBjoEntry == null)
                    {
                        Result.WarningAdd (string.Format ("{0}struct.bjo / struct.ini file not found", gameFilesPath));
                    } else
                    {
                        using (Stream s = structBjoEntry.OpenReader())
                        {
                            BinaryReader reader = new BinaryReader (s);
                            subResult = Read_WZ_Structures (reader, BJOUnits);
                            reader.Close ();
                            if (!subResult.Success)
                            {
                                Result.WarningAdd (subResult.Problem);
                            }
                        }
                    }
                    returnResult.Add (Result);
                }

                IniDroids iniDroids = null;              
                if (structIniEntry != null)
                {
                    var droidIniEntry = zip [gameFilesPath + "droid.ini"];
                    if (droidIniEntry != null)
                    {
                        iniDroids = new IniDroids ();
                        using (var reader = new StreamReader (droidIniEntry.OpenReader()))
                        {
                            var text = reader.ReadToEnd ();
                            returnResult.Add (read_INI_Droids (text, ref iniDroids));
                        }
                    }
                }

                if (iniDroids == null)
                {
                    clsResult Result = new clsResult ("dinit.bjo", false);
                    logger.Info ("Loading dinit.bjo");
                    var diniBjoEntry = zip [gameFilesPath + "dinit.bjo"];
                    if (diniBjoEntry == null)
                    {
                        Result.WarningAdd (string.Format ("{0}dinit.bjo / droid.ini file not found", gameFilesPath));
                    } else
                    {
                        using (Stream s = diniBjoEntry.OpenReader())
                        {
                            BinaryReader reader = new BinaryReader (s);
                            subResult = Read_WZ_Droids (reader, BJOUnits);
                            reader.Close ();
                            if (!subResult.Success)
                            {
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
                if (labelsIniEntry != null)
                {
                    using (var reader = new StreamReader(labelsIniEntry.OpenReader()))
                    {
                        var text = reader.ReadToEnd ();
                        returnResult.Add (Read_INI_Labels (text, false));
                    }
                }                       
            }

            return returnResult;
        }

        public clsResult Load_Game(string Path)
        {
            clsResult returnResult =
                new clsResult ("Loading game file from \"{0}\"".Format2 (Path), false);
            logger.Info ("Loading game file from \"{0}\"", Path);
            sResult SubResult = new sResult ();

            Tileset = null;

            TileType_Reset ();
            SetPainterToDefaults ();

            sSplitPath GameSplitPath = new sSplitPath (Path);
            string GameFilesPath = GameSplitPath.FilePath + GameSplitPath.FileTitleWithoutExtension + Convert.ToString (App.PlatformPathSeparator);
            string MapDirectory = "";
            FileStream File = null;

            SubResult = IOUtil.TryOpenFileStream (Path, ref File);
            if (!SubResult.Success)
            {
                returnResult.ProblemAdd ("Game file not found: " + SubResult.Problem);
                return returnResult;
            } else
            {
                BinaryReader Map_Reader = new BinaryReader (File);
                SubResult = Read_WZ_gam (Map_Reader);
                Map_Reader.Close ();

                if (!SubResult.Success)
                {
                    returnResult.ProblemAdd (SubResult.Problem);
                    return returnResult;
                }
            }

            SubResult = IOUtil.TryOpenFileStream (GameFilesPath + "game.map", ref File);
            if (!SubResult.Success)
            {
                if (MessageBox.Show ("game.map file not found at \"{0}\"\n" +
                    "Do you want to select another directory to load the underlying map from?".Format2 (GameFilesPath), 
                         "", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                {
                    returnResult.ProblemAdd ("Aborted.");
                    return returnResult;
                }
                FolderBrowserDialog DirectorySelect = new FolderBrowserDialog ();
                DirectorySelect.SelectedPath = GameFilesPath;
                if (DirectorySelect.ShowDialog () != DialogResult.OK)
                {
                    returnResult.ProblemAdd ("Aborted.");
                    return returnResult;
                }
                MapDirectory = DirectorySelect.SelectedPath + Convert.ToString (App.PlatformPathSeparator);

                SubResult = IOUtil.TryOpenFileStream (MapDirectory + "game.map", ref File);
                if (!SubResult.Success)
                {
                    returnResult.ProblemAdd ("game.map file not found: " + SubResult.Problem);
                    return returnResult;
                }
            } else
            {
                MapDirectory = GameFilesPath;
            }

            BinaryReader Map_ReaderB = new BinaryReader (File);
            SubResult = Read_WZ_map (Map_ReaderB);
            Map_ReaderB.Close ();

            if (!SubResult.Success)
            {
                returnResult.ProblemAdd (SubResult.Problem);
                return returnResult;
            }

            SimpleClassList<clsWZBJOUnit> BJOUnits = new SimpleClassList<clsWZBJOUnit> ();

            IniFeatures iniFeatures = null;
            SubResult = IOUtil.TryOpenFileStream (GameFilesPath + "feature.ini", ref File);
            if (SubResult.Success)
            {
                iniFeatures = new IniFeatures ();			
                using (var reader = new StreamReader(File))
                {
                    var text = reader.ReadToEnd ();
                    returnResult.Add (read_INI_Features (text, ref iniFeatures));
                }
            }

            if (iniFeatures == null)
            {
                clsResult Result = new clsResult ("feat.bjo", false);
                logger.Info ("Loading feat.bjo");
                SubResult = IOUtil.TryOpenFileStream (GameFilesPath + "feat.bjo", ref File);
                if (!SubResult.Success)
                {
                    Result.WarningAdd ("file not found");
                } else
                {
                    BinaryReader Features_Reader = new BinaryReader (File);
                    SubResult = Read_WZ_Features (Features_Reader, BJOUnits);
                    Features_Reader.Close ();
                    if (!SubResult.Success)
                    {
                        Result.WarningAdd (SubResult.Problem);
                    }
                }
                returnResult.Add (Result);
            }

            if (true)
            {
                clsResult Result = new clsResult ("ttypes.ttp", false);
                logger.Info ("Loading ttypes.ttp");
                SubResult = IOUtil.TryOpenFileStream (MapDirectory + "ttypes.ttp", ref File);
                if (!SubResult.Success)
                {
                    Result.WarningAdd ("file not found");
                } else
                {
                    BinaryReader TileTypes_Reader = new BinaryReader (File);
                    SubResult = Read_WZ_TileTypes (TileTypes_Reader);
                    TileTypes_Reader.Close ();
                    if (!SubResult.Success)
                    {
                        Result.WarningAdd (SubResult.Problem);
                    }
                }
                returnResult.Add (Result);
            }

            IniStructures iniStructures = null;
            SubResult = IOUtil.TryOpenFileStream (GameFilesPath + "struct.ini", ref File);
            if (SubResult.Success)
            {
                iniStructures = new IniStructures ();
                using (var reader = new StreamReader(File))
                {
                    var text = reader.ReadToEnd ();
                    returnResult.Add (read_INI_Structures (text, ref iniStructures));
                }
            }

            if (iniStructures == null)
            {
                clsResult Result = new clsResult ("struct.bjo", false);
                logger.Info ("Loading struct.bjo");
                SubResult = IOUtil.TryOpenFileStream (GameFilesPath + "struct.bjo", ref File);
                if (!SubResult.Success)
                {
                    Result.WarningAdd ("struct.bjo file not found.");
                } else
                {
                    BinaryReader Structures_Reader = new BinaryReader (File);
                    SubResult = Read_WZ_Structures (Structures_Reader, BJOUnits);
                    Structures_Reader.Close ();
                    if (!SubResult.Success)
                    {
                        Result.WarningAdd (SubResult.Problem);
                    }
                }
                returnResult.Add (Result);
            }

            IniDroids iniDroids = null;

            SubResult = IOUtil.TryOpenFileStream (GameFilesPath + "droid.ini", ref File);
            if (SubResult.Success)
            {
                iniDroids = new IniDroids ();
                using (var reader = new StreamReader(File))
                {
                    var text = reader.ReadToEnd ();
                    returnResult.Add (read_INI_Droids (text, ref iniDroids));
                }
            }

            if (iniStructures == null)
            {
                clsResult Result = new clsResult ("dinit.bjo", false);
                logger.Info ("Loading dinit.bjo");
                SubResult = IOUtil.TryOpenFileStream (GameFilesPath + "dinit.bjo", ref File);
                if (!SubResult.Success)
                {
                    Result.WarningAdd ("dinit.bjo file not found.");
                } else
                {
                    BinaryReader Droids_Reader = new BinaryReader (File);
                    SubResult = Read_WZ_Droids (Droids_Reader, BJOUnits);
                    Droids_Reader.Close ();
                    if (!SubResult.Success)
                    {
                        Result.WarningAdd (SubResult.Problem);
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

            //map objects are modified by this and must already exist
            SubResult = IOUtil.TryOpenFileStream (GameFilesPath + "labels.ini", ref File);
            if (SubResult.Success)
            {
                using (var reader = new StreamReader(File))
                {
                    var text = reader.ReadToEnd ();
                    returnResult.Add (Read_INI_Labels (text, false));
                }
            }

            return returnResult;
        }

        public clsResult CreateWZObjects(sCreateWZObjectsArgs Args)
        {
            clsResult ReturnResult = new clsResult ("Creating objects", false);
            logger.Info ("Creating objects");

            clsUnit newUnit = default(clsUnit);
            UInt32 availableID = 0;
            SimpleClassList<clsWZBJOUnit> bjoUnits = Args.BJOUnits;
            IniStructures iniStructures = Args.INIStructures;
            IniDroids iniDroids = Args.INIDroids;
            IniFeatures iniFeatures = Args.INIFeatures;
            clsUnitAdd unitAdd = new clsUnitAdd ();
            int a = 0;
            int b = 0;
            clsWZBJOUnit bjoUnit = default(clsWZBJOUnit);

            unitAdd.Map = this;

            availableID = 1U;
            foreach (clsWZBJOUnit tempLoopVar_BJOUnit in bjoUnits)
            {
                bjoUnit = tempLoopVar_BJOUnit;
                if (bjoUnit.ID >= availableID)
                {
                    availableID = bjoUnit.ID + 1U;
                }
            }
            if (iniStructures != null)
            {
                var structMaxId = iniStructures.Structures.Max (w => w.ID) + 10;
                if (structMaxId > availableID)
                {
                    availableID = structMaxId;
                }
            }
            if (iniFeatures != null)
            {
                var featuresMaxId = iniFeatures.Features.Max (w => w.ID) + 10;
                if (featuresMaxId > availableID)
                {
                    availableID = featuresMaxId;
                }
            }
            if (iniDroids != null)
            {
                var droidsMaxId = iniDroids.Droids.Max (w => w.ID) + 10;
                if (droidsMaxId > availableID)
                {
                    availableID += droidsMaxId;
                }
            }

            foreach (clsWZBJOUnit tempLoopVar_BJOUnit in bjoUnits)
            {
                bjoUnit = tempLoopVar_BJOUnit;
                newUnit = new clsUnit ();
                newUnit.ID = bjoUnit.ID;
                newUnit.TypeBase = App.ObjectData.FindOrCreateUnitType (bjoUnit.Code, bjoUnit.ObjectType, -1);
                if (newUnit.TypeBase == null)
                {
                    ReturnResult.ProblemAdd ("Unable to create object type.");
                    return ReturnResult;
                }
                if (bjoUnit.Player >= Constants.PlayerCountMax)
                {
                    newUnit.UnitGroup = ScavengerUnitGroup;
                } else
                {
                    newUnit.UnitGroup = UnitGroups [Convert.ToInt32 (bjoUnit.Player)];
                }
                newUnit.Pos = bjoUnit.Pos;
                newUnit.Rotation = (int)(Math.Min (bjoUnit.Rotation, 359U));
                if (bjoUnit.ID == 0U)
                {
                    bjoUnit.ID = availableID;
                    App.ZeroIDWarning (newUnit, bjoUnit.ID, ReturnResult);
                }
                unitAdd.NewUnit = newUnit;
                unitAdd.ID = bjoUnit.ID;
                unitAdd.Perform ();
                App.ErrorIDChange (bjoUnit.ID, newUnit, "CreateWZObjects");
                if (availableID == bjoUnit.ID)
                {
                    availableID = newUnit.ID + 1U;
                }
            }

            StructureTypeBase structureTypeBase = default(StructureTypeBase);
            DroidDesign droidType = default(DroidDesign);
            FeatureTypeBase featureTypeBase = default(FeatureTypeBase);
            DroidDesign.sLoadPartsArgs loadPartsArgs = new DroidDesign.sLoadPartsArgs ();
            UnitTypeBase unitTypeBase = null;
            int errorCount = 0;
            int unknownDroidComponentCount = 0;
            int unknownDroidTypeCount = 0;
            int droidBadPositionCount = 0;
            int structureBadPositionCount = 0;
            int structureBadModulesCount = 0;
            int featureBadPositionCount = 0;
            int moduleLimit = 0;
            XYInt zeroPos = new XYInt (0, 0);
            StructureTypeBase moduleTypeBase = default(StructureTypeBase);
            clsUnit newModule = default(clsUnit);

            StructureTypeBase factoryModule = App.ObjectData.FindFirstStructureType (StructureTypeBase.enumStructureType.FactoryModule);
            StructureTypeBase researchModule = App.ObjectData.FindFirstStructureType (StructureTypeBase.enumStructureType.ResearchModule);
            StructureTypeBase powerModule = App.ObjectData.FindFirstStructureType (StructureTypeBase.enumStructureType.PowerModule);

            if (factoryModule == null)
            {
                ReturnResult.WarningAdd ("No factory module loaded.");
            }
            if (researchModule == null)
            {
                ReturnResult.WarningAdd ("No research module loaded.");
            }
            if (powerModule == null)
            {
                ReturnResult.WarningAdd ("No power module loaded.");
            }

            if (iniStructures != null)
            {
                for (a = 0; a <= iniStructures.StructureCount - 1; a++)
                {
                    if (iniStructures.Structures [a].Pos == null)
                    {
                        logger.Debug ("{0} pos was null", iniStructures.Structures [a].Code);
                        structureBadPositionCount++;
                    } else if (!App.PosIsWithinTileArea (iniStructures.Structures [a].Pos, zeroPos, Terrain.TileSize))
                    {
                        logger.Debug ("{0} structure pos x{1} y{2}, is wrong.", iniStructures.Structures [a].Code, iniStructures.Structures [a].Pos.X, iniStructures.Structures [a].Pos.Y);
                        structureBadPositionCount++;
                    } else
                    {
                        unitTypeBase = App.ObjectData.FindOrCreateUnitType (Convert.ToString (iniStructures.Structures [a].Code),
                                                          UnitType.PlayerStructure, iniStructures.Structures [a].WallType);
                        if (unitTypeBase.Type == UnitType.PlayerStructure)
                        {
                            structureTypeBase = (StructureTypeBase)unitTypeBase;
                        } else
                        {
                            structureTypeBase = null;
                        }
                        if (structureTypeBase == null)
                        {
                            errorCount++;
                        } else
                        {
                            newUnit = new clsUnit ();
                            newUnit.TypeBase = structureTypeBase;
                            if (iniStructures.Structures [a].UnitGroup == null)
                            {
                                newUnit.UnitGroup = ScavengerUnitGroup;
                            } else
                            {
                                newUnit.UnitGroup = iniStructures.Structures [a].UnitGroup;
                            }
                            newUnit.Pos = new WorldPos (iniStructures.Structures [a].Pos, iniStructures.Structures [a].Pos.Z);
                            newUnit.Rotation = Convert.ToInt32 (iniStructures.Structures [a].Rotation.Direction * 360.0D / Constants.INIRotationMax);
                            if (newUnit.Rotation == 360)
                            {
                                newUnit.Rotation = 0;
                            }
                            if (iniStructures.Structures [a].HealthPercent >= 0)
                            {
                                newUnit.Health = MathUtil.Clamp_dbl (iniStructures.Structures [a].HealthPercent / 100.0D, 0.01D, 1.0D);
                            }
                            if (iniStructures.Structures [a].ID == 0U)
                            {
                                iniStructures.Structures [a].ID = availableID;
                                App.ZeroIDWarning (newUnit, iniStructures.Structures [a].ID, ReturnResult);
                            }
                            unitAdd.NewUnit = newUnit;
                            unitAdd.ID = iniStructures.Structures [a].ID;
                            unitAdd.Perform ();
                            App.ErrorIDChange (iniStructures.Structures [a].ID, newUnit, "Load_WZ->INIStructures");
                            if (availableID == iniStructures.Structures [a].ID)
                            {
                                availableID = newUnit.ID + 1U;
                            }
                            //create modules
                            switch (structureTypeBase.StructureType)
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
                            if (iniStructures.Structures [a].ModuleCount > moduleLimit)
                            {
                                iniStructures.Structures [a].ModuleCount = moduleLimit;
                                structureBadModulesCount++;
                            } else if (iniStructures.Structures [a].ModuleCount < 0)
                            {
                                iniStructures.Structures [a].ModuleCount = 0;
                                structureBadModulesCount++;
                            }
                            if (moduleTypeBase != null)
                            {
                                for (b = 0; b <= iniStructures.Structures[a].ModuleCount - 1; b++)
                                {
                                    newModule = new clsUnit ();
                                    newModule.TypeBase = moduleTypeBase;
                                    newModule.UnitGroup = newUnit.UnitGroup;
                                    newModule.Pos = newUnit.Pos;
                                    newModule.Rotation = newUnit.Rotation;
                                    unitAdd.NewUnit = newModule;
                                    unitAdd.ID = availableID;
                                    unitAdd.Perform ();
                                    availableID = newModule.ID + 1U;
                                }
                            }
                        }
                    }
                }
                if (structureBadPositionCount > 0)
                {
                    ReturnResult.WarningAdd (structureBadPositionCount + " structures had an invalid position and were removed.");
                }
                if (structureBadModulesCount > 0)
                {
                    ReturnResult.WarningAdd (structureBadModulesCount + " structures had an invalid number of modules.");
                }
            }
            if (iniFeatures != null)
            {
                for (a = 0; a <= iniFeatures.FeatureCount - 1; a++)
                {
                    if (iniFeatures.Features [a].Pos == null)
                    {
                        featureBadPositionCount++;
                    } else if (!App.PosIsWithinTileArea (iniFeatures.Features [a].Pos, zeroPos, Terrain.TileSize))
                    {
                        featureBadPositionCount++;
                    } else
                    {
                        unitTypeBase = App.ObjectData.FindOrCreateUnitType (Convert.ToString (iniFeatures.Features [a].Code), UnitType.Feature, -1);
                        if (unitTypeBase.Type == UnitType.Feature)
                        {
                            featureTypeBase = (FeatureTypeBase)unitTypeBase;
                        } else
                        {
                            featureTypeBase = null;
                        }
                        if (featureTypeBase == null)
                        {
                            errorCount++;
                        } else
                        {
                            newUnit = new clsUnit ();
                            newUnit.TypeBase = featureTypeBase;
                            newUnit.UnitGroup = ScavengerUnitGroup;
                            newUnit.Pos = new WorldPos ((XYInt)iniFeatures.Features [a].Pos, iniFeatures.Features [a].Pos.Z);
                            newUnit.Rotation = Convert.ToInt32 (iniFeatures.Features [a].Rotation.Direction * 360.0D / Constants.INIRotationMax);
                            if (newUnit.Rotation == 360)
                            {
                                newUnit.Rotation = 0;
                            }
                            if (iniFeatures.Features [a].HealthPercent >= 0)
                            {
                                newUnit.Health = MathUtil.Clamp_dbl (iniFeatures.Features [a].HealthPercent / 100.0D, 0.01D, 1.0D);
                            }
                            if (iniFeatures.Features [a].ID == 0U)
                            {
                                iniFeatures.Features [a].ID = availableID;
                                App.ZeroIDWarning (newUnit, iniFeatures.Features [a].ID, ReturnResult);
                            }
                            unitAdd.NewUnit = newUnit;
                            unitAdd.ID = iniFeatures.Features [a].ID;
                            unitAdd.Perform ();
                            App.ErrorIDChange (iniFeatures.Features [a].ID, newUnit, "Load_WZ->INIFeatures");
                            if (availableID == iniFeatures.Features [a].ID)
                            {
                                availableID = newUnit.ID + 1U;
                            }
                        }
                    }
                }
                if (featureBadPositionCount > 0)
                {
                    ReturnResult.WarningAdd (featureBadPositionCount + " features had an invalid position and were removed.");
                }
            }
            if (iniDroids != null)
            {
                for (a = 0; a <= iniDroids.DroidCount - 1; a++)
                {
                    if (iniDroids.Droids [a].Pos == null)
                    {
                        droidBadPositionCount++;
                    } else if (!App.PosIsWithinTileArea (iniDroids.Droids [a].Pos, zeroPos, Terrain.TileSize))
                    {
                        droidBadPositionCount++;
                    } else
                    {
                        if (iniDroids.Droids [a].Template == null || iniDroids.Droids [a].Template == "")
                        {
                            droidType = new DroidDesign ();
                            if (!droidType.SetDroidType ((enumDroidType)(iniDroids.Droids [a].DroidType)))
                            {
                                unknownDroidTypeCount++;
                            }
                            loadPartsArgs.Body = App.ObjectData.FindOrCreateBody (iniDroids.Droids [a].Body);
                            if (loadPartsArgs.Body == null)
                            {
                                unknownDroidComponentCount++;
                            } else
                            {
                                if (loadPartsArgs.Body.IsUnknown)
                                {
                                    unknownDroidComponentCount++;
                                }
                            }
                            loadPartsArgs.Propulsion = App.ObjectData.FindOrCreatePropulsion (Convert.ToString (iniDroids.Droids [a].Propulsion));
                            if (loadPartsArgs.Propulsion == null)
                            {
                                unknownDroidComponentCount++;
                            } else
                            {
                                if (loadPartsArgs.Propulsion.IsUnknown)
                                {
                                    unknownDroidComponentCount++;
                                }
                            }
                            loadPartsArgs.Construct = App.ObjectData.FindOrCreateConstruct (Convert.ToString (iniDroids.Droids [a].Construct));
                            if (loadPartsArgs.Construct == null)
                            {
                                unknownDroidComponentCount++;
                            } else
                            {
                                if (loadPartsArgs.Construct.IsUnknown)
                                {
                                    unknownDroidComponentCount++;
                                }
                            }
                            loadPartsArgs.Repair = App.ObjectData.FindOrCreateRepair (iniDroids.Droids [a].Repair);
                            if (loadPartsArgs.Repair == null)
                            {
                                unknownDroidComponentCount++;
                            } else
                            {
                                if (loadPartsArgs.Repair.IsUnknown)
                                {
                                    unknownDroidComponentCount++;
                                }
                            }
                            loadPartsArgs.Sensor = App.ObjectData.FindOrCreateSensor (iniDroids.Droids [a].Sensor);
                            if (loadPartsArgs.Sensor == null)
                            {
                                unknownDroidComponentCount++;
                            } else
                            {
                                if (loadPartsArgs.Sensor.IsUnknown)
                                {
                                    unknownDroidComponentCount++;
                                }
                            }
                            loadPartsArgs.Brain = App.ObjectData.FindOrCreateBrain (iniDroids.Droids [a].Brain);
                            if (loadPartsArgs.Brain == null)
                            {
                                unknownDroidComponentCount++;
                            } else
                            {
                                if (loadPartsArgs.Brain.IsUnknown)
                                {
                                    unknownDroidComponentCount++;
                                }
                            }
                            loadPartsArgs.ECM = App.ObjectData.FindOrCreateECM (Convert.ToString (iniDroids.Droids [a].ECM));
                            if (loadPartsArgs.ECM == null)
                            {
                                unknownDroidComponentCount++;
                            } else
                            {
                                if (loadPartsArgs.ECM.IsUnknown)
                                {
                                    unknownDroidComponentCount++;
                                }
                            }
                            loadPartsArgs.Weapon1 = App.ObjectData.FindOrCreateWeapon (Convert.ToString (iniDroids.Droids [a].Weapons [0]));
                            if (loadPartsArgs.Weapon1 == null)
                            {
                                unknownDroidComponentCount++;
                            } else
                            {
                                if (loadPartsArgs.Weapon1.IsUnknown)
                                {
                                    unknownDroidComponentCount++;
                                }
                            }
                            loadPartsArgs.Weapon2 = App.ObjectData.FindOrCreateWeapon (Convert.ToString (iniDroids.Droids [a].Weapons [1]));
                            if (loadPartsArgs.Weapon2 == null)
                            {
                                unknownDroidComponentCount++;
                            } else
                            {
                                if (loadPartsArgs.Weapon2.IsUnknown)
                                {
                                    unknownDroidComponentCount++;
                                }
                            }
                            loadPartsArgs.Weapon3 = App.ObjectData.FindOrCreateWeapon (Convert.ToString (iniDroids.Droids [a].Weapons [2]));
                            if (loadPartsArgs.Weapon3 == null)
                            {
                                unknownDroidComponentCount++;
                            } else
                            {
                                if (loadPartsArgs.Weapon3.IsUnknown)
                                {
                                    unknownDroidComponentCount++;
                                }
                            }
                            droidType.LoadParts (loadPartsArgs);
                        } else
                        {
                            unitTypeBase = App.ObjectData.FindOrCreateUnitType (iniDroids.Droids [a].Template, UnitType.PlayerDroid, -1);
                            if (unitTypeBase == null)
                            {
                                droidType = null;
                            } else
                            {
                                if (unitTypeBase.Type == UnitType.PlayerDroid)
                                {
                                    droidType = (DroidDesign)unitTypeBase;
                                } else
                                {
                                    droidType = null;
                                }
                            }
                        }
                        if (droidType == null)
                        {
                            errorCount++;
                        } else
                        {
                            newUnit = new clsUnit ();
                            newUnit.TypeBase = droidType;
                            if (iniDroids.Droids [a].UnitGroup == null)
                            {
                                newUnit.UnitGroup = ScavengerUnitGroup;
                            } else
                            {
                                newUnit.UnitGroup = iniDroids.Droids [a].UnitGroup;
                            }
                            newUnit.Pos = new WorldPos (iniDroids.Droids [a].Pos, iniDroids.Droids [a].Pos.Z);
                            newUnit.Rotation = Convert.ToInt32 (iniDroids.Droids [a].Rotation.Direction * 360.0D / Constants.INIRotationMax);
                            if (newUnit.Rotation == 360)
                            {
                                newUnit.Rotation = 0;
                            }
                            if (iniDroids.Droids [a].HealthPercent >= 0)
                            {
                                newUnit.Health = MathUtil.Clamp_dbl (iniDroids.Droids [a].HealthPercent / 100.0D, 0.01D, 1.0D);
                            }
                            if (iniDroids.Droids [a].ID == 0U)
                            {
                                iniDroids.Droids [a].ID = availableID;
                                App.ZeroIDWarning (newUnit, iniDroids.Droids [a].ID, ReturnResult);
                            }
                            unitAdd.NewUnit = newUnit;
                            unitAdd.ID = iniDroids.Droids [a].ID;
                            unitAdd.Perform ();
                            App.ErrorIDChange (iniDroids.Droids [a].ID, newUnit, "Load_WZ->INIDroids");
                            if (availableID == iniDroids.Droids [a].ID)
                            {
                                availableID = newUnit.ID + 1U;
                            }
                        }
                    }
                }
                if (droidBadPositionCount > 0)
                {
                    ReturnResult.WarningAdd (droidBadPositionCount + " droids had an invalid position and were removed.");
                }
                if (unknownDroidTypeCount > 0)
                {
                    ReturnResult.WarningAdd (unknownDroidTypeCount + " droid designs had an unrecognised droidType and were removed.");
                }
                if (unknownDroidComponentCount > 0)
                {
                    ReturnResult.WarningAdd (unknownDroidComponentCount + " droid designs had components that are not loaded.");
                }
            }

            if (errorCount > 0)
            {
                ReturnResult.WarningAdd ("Object Create Error.");
            }

            return ReturnResult;
        }

        private clsResult read_INI_Features(string iniText, ref IniFeatures resultData)
        {
            var resultObject = new clsResult ("Reading feature.ini.", false);
            logger.Info ("Reading feature.ini");

            try
            {
                var iniSections = SharpFlame.Core.Parsers.Ini.IniReader.ReadString (iniText);
                foreach (var iniSection in iniSections)
                {
                    var feature = new IniFeatures.Feature ();
                    feature.HealthPercent = -1;
                    var invalid = false;
                    foreach (var iniToken in iniSection.Data)
                    {
                        if (invalid)
                        {
                            break;
                        }

                        try
                        {
                            switch (iniToken.Name)
                            {
                            case "id":
                                feature.ID = uint.Parse (iniToken.Data);
                                break;
                            case "name":
                                feature.Code = iniToken.Data;
                                break;
                            case "position":
                                feature.Pos = XYZInt.FromString (iniToken.Data);
                                break;
                            case "rotation":
                                feature.Rotation = Rotation.FromString (iniToken.Data);
                                break;
                            case "health":
                                feature.HealthPercent = SharpFlame.Core.Parsers.Ini.IniReader.ReadHealthPercent (iniToken.Data); 
                                if (feature.HealthPercent < 0 || feature.HealthPercent > 100)
                                {
                                    resultObject.WarningAdd (string.Format ("#{0} invalid health: \"{1}\"", iniSection.Name, feature.HealthPercent), false);
                                    logger.Warn ("#{0} invalid health: \"{1}\"", iniSection.Name, feature.HealthPercent);
                                    invalid = true;
                                    continue;
                                }
                                break;
                            default:
                                resultObject.WarningAdd (string.Format ("Found an invalid key: {0}={1}", iniToken.Name, iniToken.Data), false);
                                logger.Warn ("Found an invalid key: {0} = {1}", iniToken.Name, iniToken.Data);
                                break;
                            }
                        } catch (Exception ex)
                        {
                            Debugger.Break ();
                            resultObject.WarningAdd (string.Format ("#{0} invalid {2}: \"{3}\", got exception: {2}", iniSection.Name, iniToken.Name, iniToken.Data, ex.Message), false);
                            logger.WarnException (string.Format ("#{0} invalid {2} \"{1}\"", iniSection.Name, iniToken.Name, iniToken.Data), ex);                                    
                            invalid = true;
                            continue;
                        }
                    }

                    if (!invalid)
                    {
                        resultData.Features.Add (feature);
                    }
                }
            } catch (Exception ex)
            {
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

            try
            {
                var iniSections = SharpFlame.Core.Parsers.Ini.IniReader.ReadString (iniText);            
                foreach (var iniSection in iniSections)
                {                
                    var droid = new IniDroids.Droid ();
                    droid.HealthPercent = -1;
                    var invalid = false;
                    foreach (var iniToken in iniSection.Data)
                    {                        
                        if (invalid)
                        {
                            break;
                        }

                        try
                        {
                            switch (iniToken.Name)
                            {
                            
                            case "id":
                                droid.ID = uint.Parse (iniToken.Data);
                                break;
                            
                            case "startpos":
                                var tmpStartPos = int.Parse (iniToken.Data);
                                if (tmpStartPos < 0 | tmpStartPos >= Constants.PlayerCountMax)
                                {
                                    resultObject.WarningAdd (string.Format ("#{0} invalid startpos {1}", iniSection.Name, tmpStartPos), false);
                                    logger.Warn ("#{0} invalid startpos {1}", iniSection.Name, tmpStartPos);                                    
                                    invalid = true;
                                    continue;
                                }
                                droid.UnitGroup = UnitGroups [tmpStartPos];
                                break;

                            case "template":
                                droid.Template = iniToken.Data;
                                break;

                            case "position":
                                droid.Pos = XYZInt.FromString (iniToken.Data);
                                break;

                            case "rotation":
                                droid.Rotation = Rotation.FromString (iniToken.Data);
                                break;

                            case "player":
                                if (iniToken.Data.ToLower () == "scavenger")
                                {
                                    droid.UnitGroup = ScavengerUnitGroup;
                                } else
                                {
                                    resultObject.WarningAdd (string.Format ("#{0} invalid player: \"{1}\"", iniToken.Name, iniToken.Data), false);
                                    logger.Warn ("#{0} invalid player \"{1}\"", iniToken.Name, iniToken.Data);                                    
                                    invalid = true;
                                    continue;
                                }
                                break;

                            case "name":
                                // ignore
                                break;

                            case "health":
                                droid.HealthPercent = SharpFlame.Core.Parsers.Ini.IniReader.ReadHealthPercent (iniToken.Data); 
                                if (droid.HealthPercent < 0 || droid.HealthPercent > 100)
                                {
                                    resultObject.WarningAdd (string.Format ("#{0} invalid health: \"{1}\"", iniSection.Name, droid.HealthPercent), false);
                                    invalid = true;
                                    continue;
                                }
                                break;
                            
                            case "droidtype":
                                droid.DroidType = Numerics.Int.Parse (iniToken.Data);
                                break;

                            case "weapons":
                                droid.WeaponCount = Numerics.Int.Parse (iniToken.Data);
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
                                if (droid.Weapons == null)
                                {
                                    droid.Weapons = new string[3];
                                }
                                droid.Weapons [0] = iniToken.Data;
                                break;
                            
                            case "parts\\weapon\\2":
                                if (droid.Weapons == null)
                                {
                                    droid.Weapons = new string[3];
                                }

                                droid.Weapons [1] = iniToken.Data;
                                break;

                            case "parts\\weapon\\3":
                                if (droid.Weapons == null)
                                {
                                    droid.Weapons = new string[3];
                                }

                                droid.Weapons [2] = iniToken.Data;
                                break;
                            
                            default:
                                resultObject.WarningAdd (string.Format ("Found an invalid key: {0} = {1}", iniToken.Name, iniToken.Data), false);
                                logger.Warn ("Found an invalid key: {0} = {1}", iniToken.Name, iniToken.Data);
                                break;                        
                            }
                        } catch (Exception ex)
                        {
                            Debugger.Break ();
                            resultObject.WarningAdd (string.Format ("#{0} invalid {2}: \"{3}\", got exception: {2}", iniSection.Name, iniToken.Name, iniToken.Data, ex.Message), false);
                            logger.ErrorException (string.Format ("#{0} invalid {2} \"{1}\"", iniSection.Name, iniToken.Name, iniToken.Data), ex);                                    
                            invalid = true;
                            continue;
                        }
                    }

                    if (!invalid)
                    {
                        resultData.Droids.Add (droid);
                    }
                }            
            } catch (Exception ex)
            {
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

            try
            {
                var iniSections = SharpFlame.Core.Parsers.Ini.IniReader.ReadString (iniText);
                foreach (var iniSection in iniSections)
                {
                    var structure = new IniStructures.Structure ();
                    structure.WallType = -1;
                    structure.HealthPercent = -1;
                    var invalid = false;
                    foreach (var iniToken in iniSection.Data)
                    {
                        if (invalid)
                        {
                            break;
                        }

                        try
                        {
                            switch (iniToken.Name)
                            {
                            case "id":
                                structure.ID = uint.Parse (iniToken.Data);
                                break;

                            case "name":
                                structure.Code = iniToken.Data;
                                break;

                            case "startpos":
                                var tmpStartPos = int.Parse (iniToken.Data);
                                if (tmpStartPos < 0 | tmpStartPos >= Constants.PlayerCountMax)
                                {
                                    resultObject.WarningAdd (string.Format ("#{0} invalid startpos {1}", iniSection.Name, tmpStartPos), false);
                                    logger.Warn ("#{0} invalid startpos {1}", iniSection.Name, tmpStartPos);                                    
                                    invalid = true;
                                    continue;
                                }
                                structure.UnitGroup = UnitGroups [tmpStartPos];
                                break;

                            case "player":
                                if (iniToken.Data.ToLower () == "scavenger")
                                {
                                    structure.UnitGroup = ScavengerUnitGroup;
                                } else
                                {
                                    resultObject.WarningAdd (string.Format ("#{0} invalid player: \"{1}\"", iniToken.Name, iniToken.Data), false);
                                    logger.Warn ("#{0} invalid player \"{1}\"", iniToken.Name, iniToken.Data);                                    
                                    invalid = true;
                                    continue;
                                }
                                break;

                            case "position":
                                structure.Pos = XYZInt.FromString (iniToken.Data);
                                break;

                            case "rotation":
                                structure.Rotation = Rotation.FromString (iniToken.Data);
                                break;

                            case "modules":
                                structure.ModuleCount = int.Parse (iniToken.Data);
                                break;

                            case "health":
                                structure.HealthPercent = SharpFlame.Core.Parsers.Ini.IniReader.ReadHealthPercent (iniToken.Data); 
                                if (structure.HealthPercent < 0 || structure.HealthPercent > 100)
                                {
                                    resultObject.WarningAdd (string.Format ("#{0} invalid health: \"{1}\"", iniSection.Name, structure.HealthPercent), false);
                                    invalid = true;
                                    continue;
                                }
                                break;

                            case "wall/type":
                                structure.WallType = int.Parse (iniToken.Data);
                                if (structure.WallType < 0)
                                {
                                    resultObject.WarningAdd (string.Format ("#{0} invalid wall/type: \"{1}\"", iniSection.Name, structure.WallType), false);
                                    invalid = true;
                                    continue;
                                }
                                break;

                            default:
                                resultObject.WarningAdd (string.Format ("Found an invalid key: {0} = {1}", iniToken.Name, iniToken.Data), false);
                                logger.Warn ("Found an invalid key: {0} = {1}", iniToken.Name, iniToken.Data);
                                break;                        
                            }

                        } catch (Exception ex)
                        {
                            Debugger.Break ();
                            resultObject.WarningAdd (string.Format ("#{0} invalid {2}: \"{3}\", got exception: {2}", iniSection.Name, iniToken.Name, iniToken.Data, ex.Message), false);
                            logger.WarnException (string.Format ("#{0} invalid {2} \"{1}\"", iniSection.Name, iniToken.Name, iniToken.Data), ex);                                    
                            invalid = true;
                            continue;
                        }
                    }

                    if (!invalid)
                    {
                        resultData.Structures.Add (structure);
                    }
                }
            } catch (Exception ex)
            {
                Debugger.Break ();
                logger.ErrorException ("Got exception while reading droid.ini", ex);
                resultObject.ProblemAdd (string.Format ("Got exception: {0}", ex.Message), false);
                return resultObject;
            }

            return resultObject;
        }

        public clsResult Read_INI_Labels(string iniText, bool isFMap)
        {
            clsResult resultObject = new clsResult ("Reading labels", false);
            logger.Info ("Reading labels.");

            int typeNum = 0;
            clsScriptPosition NewPosition = default(clsScriptPosition);
            clsScriptArea NewArea = default(clsScriptArea);
            string nameText = "";
            string strLabel = "";
            string strPosA = "";
            string strPosB = "";
            string idText = "";
            UInt32 idNum = 0;
            XYInt xyIntA = null;
            XYInt xyIntB = null;

            int failedCount = 0;
            int modifiedCount = 0;

            try
            {
                var iniSections = SharpFlame.Core.Parsers.Ini.IniReader.ReadString (iniText);
                foreach (var iniSection in iniSections)
                {
                    var idx = iniSection.Name.IndexOf ('_');
                    if (idx > 0) {
                        nameText = iniSection.Name.Substring (0, idx);
                    } else {
                        nameText = iniSection.Name;
                    }
                    switch (nameText)
                    {
                    case "position":
                        typeNum = 0;
                        break;
                    case "area":
                        typeNum = 1;
                        break;
                    case "object":
                        if (isFMap)
                        {
                            typeNum = int.MaxValue;
                            failedCount++;
                            continue;
                        } else
                        {
                            typeNum = 2;
                        }
                        break;
                    default:
                        typeNum = int.MaxValue;
                        failedCount++;
                        continue;
                    }

                    // Raised an exception if nothing was found
                    try
                    {
                        strLabel = iniSection.Data.Where (d => d.Name == "label").First ().Data;
                    } catch (Exception ex)
                    {
                        resultObject.WarningAdd (string.Format ("Failed to parse \"label\", error was: {0}", ex.Message));
                        logger.WarnException ("Failed to parse \"label\", error was", ex);
                        failedCount++;
                        continue;
                    }
                    strLabel = strLabel.Replace ("\"", "");

                    switch (typeNum)
                    {
                    case 0: //position
                        strPosA = iniSection.Data.Where (d => d.Name == "pos").First ().Data;
                        if (strPosA == null)
                        {
                            failedCount++;
                            continue;
                        }
                        try
                        {
                            xyIntA = XYInt.FromString (strPosA);
                            NewPosition = new clsScriptPosition (this);
                            NewPosition.PosX = xyIntA.X;
                            NewPosition.PosY = xyIntA.Y;
                            NewPosition.SetLabel (strLabel);
                            if (NewPosition.Label != strLabel || 
                                NewPosition.PosX != xyIntA.X || NewPosition.PosY != xyIntA.Y)
                            {
                                modifiedCount++;
                            }
                        } catch (Exception ex)
                        {
                            resultObject.WarningAdd (string.Format ("Failed to parse \"pos\", error was: {0}", ex.Message));
                            logger.WarnException ("Failed to parse \"pos\", error was", ex);
                            failedCount++;
                            continue;
                        }
                        break;
                    case 1: //area
                        try
                        {
                            strPosA = iniSection.Data.Where (d => d.Name == "pos1").First ().Data;
                            strPosB = iniSection.Data.Where (d => d.Name == "pos2").First ().Data;

                            xyIntA = XYInt.FromString (strPosA);
                            xyIntB = XYInt.FromString (strPosA);
                            NewArea = new clsScriptArea (this);
                            NewArea.SetPositions (xyIntA, xyIntB);
                            NewArea.SetLabel (strLabel);
                            if (NewArea.Label != strLabel || NewArea.PosAX != xyIntA.X | NewArea.PosAY != xyIntA.Y
                                | NewArea.PosBX != xyIntB.X | NewArea.PosBY != xyIntB.Y)
                            {
                                modifiedCount++;
                            }
                        } catch (Exception ex)
                        {
                            Debugger.Break ();
                            resultObject.WarningAdd (string.Format ("Failed to parse \"pos1\" or \"pos2\", error was: {0}", ex.Message));
                            logger.WarnException ("Failed to parse \"pos1\" or \"pos2\".", ex);
                            failedCount++;
                            continue;
                        }
                        break;
                    case 2: //object
                        idText = iniSection.Data.Where (d => d.Name == "id").First ().Data;
                        if (IOUtil.InvariantParse (idText, ref idNum))
                        {
                            clsUnit Unit = IDUsage (idNum);
                            if (Unit != null)
                            {
                                if (!Unit.SetLabel (strLabel).Success)
                                {
                                    failedCount++;
                                    continue;
                                }
                            } else
                            {
                                failedCount++;
                                continue;
                            }
                        }
                        break;
                    default:
                        resultObject.WarningAdd ("Error! Bad type number for script label.");
                        break;
                    }
                }
            } catch (Exception ex)
            {
                Debugger.Break ();
                logger.ErrorException ("Got exception while reading labels.ini", ex);
                resultObject.ProblemAdd (string.Format ("Got exception: {0}", ex.Message), false);
                return resultObject;
            }

            if (failedCount > 0)
            {
                resultObject.WarningAdd (string.Format ("Unable to translate {0} script labels.", failedCount));
            }
            if (modifiedCount > 0)
            {
                resultObject.WarningAdd (string.Format ("{0} script labels had invalid values and were modified.", modifiedCount));
            }

            return resultObject;
        }

        private sResult Read_WZ_gam(BinaryReader File)
        {
            sResult ReturnResult = new sResult ();
            ReturnResult.Success = false;
            ReturnResult.Problem = "";

            string strTemp = "";
            UInt32 Version = 0;

            try
            {
                strTemp = IOUtil.ReadOldTextOfLength (File, 4);
                if (strTemp != "game")
                {
                    ReturnResult.Problem = "Unknown game identifier.";
                    return ReturnResult;
                }

                Version = File.ReadUInt32 ();
                if (Version != 8U)
                {
                    if (MessageBox.Show ("Game file version is unknown. Continue?", "", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                    {
                        ReturnResult.Problem = "Aborted.";
                        return ReturnResult;
                    }
                }

                if (InterfaceOptions == null)
                {
                    InterfaceOptions = new clsInterfaceOptions ();
                }

                File.ReadInt32 (); //game time
                InterfaceOptions.CampaignGameType = File.ReadInt32 ();
                InterfaceOptions.AutoScrollLimits = false;
                InterfaceOptions.ScrollMin.X = File.ReadInt32 ();
                InterfaceOptions.ScrollMin.Y = File.ReadInt32 ();
                InterfaceOptions.ScrollMax.X = File.ReadUInt32 ();
                InterfaceOptions.ScrollMax.Y = File.ReadUInt32 ();
            } catch (Exception ex)
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
            sResult ReturnResult = new sResult ();
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
            XYInt PosA = new XYInt ();
            XYInt PosB = new XYInt ();

            try
            {
                strTemp = IOUtil.ReadOldTextOfLength (File, 4);
                if (strTemp != "map ")
                {
                    ReturnResult.Problem = "Unknown game.map identifier.";
                    return ReturnResult;
                }

                Version = File.ReadUInt32 ();
                if (Version != 10U)
                {
                    if (MessageBox.Show ("game.map version is unknown. Continue?", "", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                    {
                        ReturnResult.Problem = "Aborted.";
                        return ReturnResult;
                    }
                }
                MapWidth = File.ReadUInt32 ();
                MapHeight = File.ReadUInt32 ();
                if (MapWidth < 1U || MapWidth > Constants.MapMaxSize || MapHeight < 1U || MapHeight > Constants.MapMaxSize)
                {
                    ReturnResult.Problem = "Map size out of range.";
                    return ReturnResult;
                }

                TerrainBlank (new XYInt (Convert.ToInt32 (MapWidth), Convert.ToInt32 (MapHeight)));

                for (Y = 0; Y <= Terrain.TileSize.Y - 1; Y++)
                {
                    for (X = 0; X <= Terrain.TileSize.X - 1; X++)
                    {
                        TextureNum = File.ReadByte ();
                        Terrain.Tiles [X, Y].Texture.TextureNum = TextureNum;
                        Flip = File.ReadByte ();
                        Terrain.Vertices [X, Y].Height = File.ReadByte ();
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
                        TileUtil.OldOrientation_To_TileOrientation (Rotate, FlipX, FlipZ, ref Terrain.Tiles [X, Y].Texture.Orientation);
                        //get tri direction
                        A = (int)((Flip / 8.0D));
                        Flip -= (byte)(A * 8);
                        Terrain.Tiles [X, Y].Tri = A == 1;
                    }
                }

                if (Version != 2U)
                {
                    uintTemp = File.ReadUInt32 ();
                    if (uintTemp != 1)
                    {
                        ReturnResult.Problem = "Bad gateway version number.";
                        return ReturnResult;
                    }

                    uintTemp = File.ReadUInt32 ();

                    for (A = 0; A <= (Convert.ToInt32(uintTemp)) - 1; A++)
                    {
                        PosA.X = File.ReadByte ();
                        PosA.Y = File.ReadByte ();
                        PosB.X = File.ReadByte ();
                        PosB.Y = File.ReadByte ();
                        if (GatewayCreate (PosA, PosB) == null)
                        {
                            ReturnResult.Problem = "Gateway placement error.";
                            return ReturnResult;
                        }
                    }
                }
            } catch (Exception ex)
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
            sResult ReturnResult = new sResult ();
            ReturnResult.Success = false;
            ReturnResult.Problem = "";

            string strTemp = null;
            UInt32 Version = 0;
            UInt32 uintTemp = 0;
            int A = 0;
            clsWZBJOUnit WZBJOUnit = default(clsWZBJOUnit);

            try
            {
                strTemp = IOUtil.ReadOldTextOfLength (File, 4);
                if (strTemp != "feat")
                {
                    ReturnResult.Problem = "Unknown feat.bjo identifier.";
                    return ReturnResult;
                }

                Version = File.ReadUInt32 ();
                if (Version != 8U)
                {
                    if (MessageBox.Show ("feat.bjo version is unknown. Continue?", "", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                    {
                        ReturnResult.Problem = "Aborted.";
                        return ReturnResult;
                    }
                }

                uintTemp = File.ReadUInt32 ();
                for (A = 0; A <= (Convert.ToInt32(uintTemp)) - 1; A++)
                {
                    WZBJOUnit = new clsWZBJOUnit ();
                    WZBJOUnit.ObjectType = UnitType.Feature;
                    WZBJOUnit.Code = IOUtil.ReadOldTextOfLength (File, 40);
                    WZBJOUnit.Code = WZBJOUnit.Code.Substring (0, WZBJOUnit.Code.IndexOf ('\0'));
                    WZBJOUnit.ID = File.ReadUInt32 ();
                    WZBJOUnit.Pos.Horizontal.X = (int)(File.ReadUInt32 ());
                    WZBJOUnit.Pos.Horizontal.Y = (int)(File.ReadUInt32 ());
                    WZBJOUnit.Pos.Altitude = (int)(File.ReadUInt32 ());
                    WZBJOUnit.Rotation = File.ReadUInt32 ();
                    WZBJOUnit.Player = File.ReadUInt32 ();
                    File.ReadBytes (12);
                    WZUnits.Add (WZBJOUnit);
                }
            } catch (Exception ex)
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
            sResult ReturnResult = new sResult ();
            ReturnResult.Success = false;
            ReturnResult.Problem = "";

            string strTemp = null;
            UInt32 Version = 0;
            UInt32 uintTemp = 0;
            UInt16 ushortTemp = 0;
            int A = 0;

            try
            {
                strTemp = IOUtil.ReadOldTextOfLength (File, 4);
                if (strTemp != "ttyp")
                {
                    ReturnResult.Problem = "Unknown ttypes.ttp identifier.";
                    return ReturnResult;
                }

                Version = File.ReadUInt32 ();
                if (Version != 8U)
                {
                    //Load_WZ.Problem = "Unknown ttypes.ttp version."
                    //Exit Function
                    if (MessageBox.Show ("ttypes.ttp version is unknown. Continue?", "", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                    {
                        ReturnResult.Problem = "Aborted.";
                        return ReturnResult;
                    }
                }

                uintTemp = File.ReadUInt32 ();

                if (Tileset != null)
                {
                    for (A = 0; A <= Math.Min(Convert.ToInt32(uintTemp), Tileset.TileCount) - 1; A++)
                    {
                        ushortTemp = File.ReadUInt16 ();
                        if (ushortTemp > 11U)
                        {
                            ReturnResult.Problem = "Unknown tile type.";
                            return ReturnResult;
                        }
                        Tile_TypeNum [A] = (byte)ushortTemp;
                    }
                }
            } catch (Exception ex)
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
            sResult returnResult = new sResult ();
            returnResult.Success = false;
            returnResult.Problem = "";

            string strTemp = null;
            UInt32 version = 0;
            UInt32 uintTemp = 0;
            int a = 0;
            clsWZBJOUnit wzBJOUnit = default(clsWZBJOUnit);

            try
            {
                strTemp = IOUtil.ReadOldTextOfLength (File, 4);
                if (strTemp != "stru")
                {
                    returnResult.Problem = "Unknown struct.bjo identifier.";
                    return returnResult;
                }

                version = File.ReadUInt32 ();
                if (version != 8U)
                {
                    if (MessageBox.Show ("struct.bjo version is unknown. Continue?", "", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                    {
                        returnResult.Problem = "Aborted.";
                        return returnResult;
                    }
                }

                uintTemp = File.ReadUInt32 ();
                for (a = 0; a <= (Convert.ToInt32(uintTemp)) - 1; a++)
                {
                    wzBJOUnit = new clsWZBJOUnit ();
                    wzBJOUnit.ObjectType = UnitType.PlayerStructure;
                    wzBJOUnit.Code = IOUtil.ReadOldTextOfLength (File, 40);
                    wzBJOUnit.Code = wzBJOUnit.Code.Substring (0, wzBJOUnit.Code.IndexOf ('\0'));
                    wzBJOUnit.ID = File.ReadUInt32 ();
                    wzBJOUnit.Pos.Horizontal.X = (int)(File.ReadUInt32 ());
                    wzBJOUnit.Pos.Horizontal.Y = (int)(File.ReadUInt32 ());
                    wzBJOUnit.Pos.Altitude = (int)(File.ReadUInt32 ());
                    wzBJOUnit.Rotation = File.ReadUInt32 ();
                    wzBJOUnit.Player = File.ReadUInt32 ();
                    File.ReadBytes (56);
                    WZUnits.Add (wzBJOUnit);
                }
            } catch (Exception ex)
            {
                returnResult.Problem = ex.Message;
                logger.ErrorException ("Got an exception", ex);
                return returnResult;
            }

            returnResult.Success = true;
            return returnResult;
        }

        private sResult Read_WZ_Droids(BinaryReader File, SimpleClassList<clsWZBJOUnit> WZUnits)
        {
            sResult ReturnResult = new sResult ();
            ReturnResult.Success = false;
            ReturnResult.Problem = "";

            string strTemp = null;
            UInt32 Version = 0;
            UInt32 uintTemp = 0;
            int A = 0;
            clsWZBJOUnit WZBJOUnit = default(clsWZBJOUnit);

            try
            {
                strTemp = IOUtil.ReadOldTextOfLength (File, 4);
                if (strTemp != "dint")
                {
                    ReturnResult.Problem = "Unknown dinit.bjo identifier.";
                    return ReturnResult;
                }

                Version = File.ReadUInt32 ();
                if (Version > 19U)
                {
                    if (MessageBox.Show ("dinit.bjo version is unknown. Continue?", "", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                    {
                        ReturnResult.Problem = "Aborted.";
                        return ReturnResult;
                    }
                }

                uintTemp = File.ReadUInt32 ();
                for (A = 0; A <= (Convert.ToInt32(uintTemp)) - 1; A++)
                {
                    WZBJOUnit = new clsWZBJOUnit ();
                    WZBJOUnit.ObjectType = UnitType.PlayerDroid;
                    WZBJOUnit.Code = IOUtil.ReadOldTextOfLength (File, 40);
                    WZBJOUnit.Code = WZBJOUnit.Code.Substring (0, WZBJOUnit.Code.IndexOf ('\0'));
                    WZBJOUnit.ID = File.ReadUInt32 ();
                    WZBJOUnit.Pos.Horizontal.X = (int)(File.ReadUInt32 ());
                    WZBJOUnit.Pos.Horizontal.Y = (int)(File.ReadUInt32 ());
                    WZBJOUnit.Pos.Altitude = (int)(File.ReadUInt32 ());
                    WZBJOUnit.Rotation = File.ReadUInt32 ();
                    WZBJOUnit.Player = File.ReadUInt32 ();
                    File.ReadBytes (12);
                    WZUnits.Add (WZBJOUnit);
                }
            } catch (Exception ex)
            {
                ReturnResult.Problem = ex.Message;
                logger.ErrorException ("Got an exception", ex);
                return ReturnResult;
            }

            ReturnResult.Success = true;
            return ReturnResult;
        }

        public clsResult Serialize_WZ_StructuresINI(IniWriter File, int PlayerCount)
        {
            clsResult ReturnResult = new clsResult ("Serializing structures INI", false);
            logger.Info ("Serializing structures INI");

            StructureTypeBase structureTypeBase = default(StructureTypeBase);
            int[] unitModuleCount = new int[Units.Count];
            XYInt sectorNum = new XYInt ();
            StructureTypeBase otherStructureTypeBase = default(StructureTypeBase);
            clsUnit otherUnit = default(clsUnit);
            XYInt moduleMin = new XYInt ();
            XYInt moduleMax = new XYInt ();
            XYInt footprint = new XYInt ();
            int A = 0;
            StructureTypeBase.enumStructureType[] underneathTypes = new StructureTypeBase.enumStructureType[2];
            int underneathTypeCount = 0;
            int badModuleCount = 0;
            clsObjectPriorityOrderList priorityOrder = new clsObjectPriorityOrderList ();

            foreach (var unit in Units.Where(d => d.TypeBase.Type == UnitType.PlayerStructure))
            {
                structureTypeBase = (StructureTypeBase)unit.TypeBase;
                switch (structureTypeBase.StructureType)
                {
                case StructureTypeBase.enumStructureType.FactoryModule:
                    underneathTypes [0] = StructureTypeBase.enumStructureType.Factory;
                    underneathTypes [1] = StructureTypeBase.enumStructureType.VTOLFactory;
                    underneathTypeCount = 2;
                    break;
                case StructureTypeBase.enumStructureType.PowerModule:
                    underneathTypes [0] = StructureTypeBase.enumStructureType.PowerGenerator;
                    underneathTypeCount = 1;
                    break;
                case StructureTypeBase.enumStructureType.ResearchModule:
                    underneathTypes [0] = StructureTypeBase.enumStructureType.Research;
                    underneathTypeCount = 1;
                    break;
                default:
                    underneathTypeCount = 0;
                    break;
                }

                if (underneathTypeCount == 0)
                {
                    priorityOrder.SetItem (unit);
                    priorityOrder.ActionPerform ();
                } else
                {
                    // IS module.
                    sectorNum = GetPosSectorNum (unit.Pos.Horizontal);
                    clsUnit underneath = null;
                    clsUnitSectorConnection connection = default(clsUnitSectorConnection);
                    foreach (clsUnitSectorConnection tempLoopVar_Connection in Sectors[sectorNum.X, sectorNum.Y].Units)
                    {
                        connection = tempLoopVar_Connection;
                        otherUnit = connection.Unit;
                        if (otherUnit.TypeBase.Type == UnitType.PlayerStructure)
                        {
                            otherStructureTypeBase = (StructureTypeBase)otherUnit.TypeBase;
                            if (otherUnit.UnitGroup == unit.UnitGroup)
                            {
                                for (A = 0; A <= underneathTypeCount - 1; A++)
                                {
                                    if (otherStructureTypeBase.StructureType == underneathTypes [A])
                                    {
                                        break;
                                    }
                                }
                                if (A < underneathTypeCount)
                                {
                                    footprint = otherStructureTypeBase.get_GetFootprintSelected (otherUnit.Rotation);
                                    moduleMin.X = otherUnit.Pos.Horizontal.X - (int)(footprint.X * Constants.TerrainGridSpacing / 2.0D);
                                    moduleMin.Y = otherUnit.Pos.Horizontal.Y - (int)(footprint.Y * Constants.TerrainGridSpacing / 2.0D);
                                    moduleMax.X = otherUnit.Pos.Horizontal.X + (int)(footprint.X * Constants.TerrainGridSpacing / 2.0D);
                                    moduleMax.Y = otherUnit.Pos.Horizontal.Y + (int)(footprint.Y * Constants.TerrainGridSpacing / 2.0D);
                                    if (unit.Pos.Horizontal.X >= moduleMin.X & unit.Pos.Horizontal.X < moduleMax.X &
                                        unit.Pos.Horizontal.Y >= moduleMin.Y & unit.Pos.Horizontal.Y < moduleMax.Y)
                                    {
                                        unitModuleCount [otherUnit.MapLink.ArrayPosition]++;
                                        underneath = otherUnit;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    if (underneath == null)
                    {
                        badModuleCount++;
                    }
                }              
            }

            if (badModuleCount > 0)
            {
                ReturnResult.WarningAdd (badModuleCount + " modules had no underlying structure.");
            }

            int tooManyModulesWarningCount = 0;
            int tooManyModulesWarningMaxCount = 16;
            int moduleCount = 0;
            int moduleLimit = 0;

            for (A = 0; A <= priorityOrder.Result.Count - 1; A++)
            {
                var unit = priorityOrder.Result [A];
                structureTypeBase = (StructureTypeBase)unit.TypeBase;
                if (unit.ID <= 0)
                {
                    ReturnResult.WarningAdd ("Error. A structure\'s ID was zero. It was NOT saved. Delete and replace it to allow save.");
                } else
                {
                    File.AddSection ("structure_" + unit.ID.ToStringInvariant ());
                    File.AddProperty ("id", unit.ID.ToStringInvariant ());
                    if (unit.UnitGroup == ScavengerUnitGroup || (PlayerCount >= 0 & unit.UnitGroup.WZ_StartPos >= PlayerCount))
                    {
                        File.AddProperty ("player", "scavenger");
                    } else
                    {
                        File.AddProperty ("startpos", unit.UnitGroup.WZ_StartPos.ToStringInvariant ());
                    }
                    File.AddProperty ("name", structureTypeBase.Code);
                    if (structureTypeBase.WallLink.IsConnected)
                    {
                        File.AddProperty ("wall/type", structureTypeBase.WallLink.ArrayPosition.ToStringInvariant ());
                    }
                    File.AddProperty ("position", unit.GetINIPosition ());
                    File.AddProperty ("rotation", unit.GetINIRotation ());
                    if (unit.Health < 1.0D)
                    {
                        File.AddProperty ("health", unit.GetINIHealthPercent ());
                    }
                    switch (structureTypeBase.StructureType)
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
                    if (unitModuleCount [unit.MapLink.ArrayPosition] > moduleLimit)
                    {
                        moduleCount = moduleLimit;
                        if (tooManyModulesWarningCount < tooManyModulesWarningMaxCount)
                        {
                            ReturnResult.WarningAdd (string.Format("Structure {0} at {1} has too many modules ({2})", 
                                                                   structureTypeBase.GetDisplayTextCode (), 
                                                                   unit.GetPosText (), 
                                                                   unitModuleCount [unit.MapLink.ArrayPosition]));
                        }
                        tooManyModulesWarningCount++;
                    } else
                    {
                        moduleCount = unitModuleCount [unit.MapLink.ArrayPosition];
                    }
                    File.AddProperty ("modules", moduleCount.ToStringInvariant ());
                }
            }

            if (tooManyModulesWarningCount > tooManyModulesWarningMaxCount)
            {
                ReturnResult.WarningAdd (tooManyModulesWarningCount + " structures had too many modules.");
            }

            return ReturnResult;
        }

        public clsResult Serialize_WZ_DroidsINI(IniWriter File, int PlayerCount)
        {
            clsResult ReturnResult = new clsResult ("Serializing droids INI", false);
            logger.Info ("Serializing droids INI");

            DroidDesign Droid = default(DroidDesign);
            DroidTemplate Template = default(DroidTemplate);
            string Text = "";
            clsUnit Unit = default(clsUnit);
            bool AsPartsNotTemplate = default(bool);
            bool ValidDroid = default(bool);
            int InvalidPartCount = 0;
            Brain Brain = default(Brain);

            foreach (clsUnit tempLoopVar_Unit in Units)
            {
                Unit = tempLoopVar_Unit;
                if (Unit.TypeBase.Type == UnitType.PlayerDroid)
                {
                    Droid = (DroidDesign)Unit.TypeBase;
                    ValidDroid = true;
                    if (Unit.ID <= 0)
                    {
                        ValidDroid = false;
                        ReturnResult.WarningAdd ("Error. A droid\'s ID was zero. It was NOT saved. Delete and replace it to allow save.");
                    }
                    if (Droid.IsTemplate)
                    {
                        Template = (DroidTemplate)Droid;
                        AsPartsNotTemplate = Unit.PreferPartsOutput;
                    } else
                    {
                        Template = null;
                        AsPartsNotTemplate = true;
                    }
                    if (AsPartsNotTemplate)
                    {
                        if (Droid.Body == null)
                        {
                            ValidDroid = false;
                            InvalidPartCount++;
                        } else if (Droid.Propulsion == null)
                        {
                            ValidDroid = false;
                            InvalidPartCount++;
                        } else if (Droid.TurretCount >= 1)
                        {
                            if (Droid.Turret1 == null)
                            {
                                ValidDroid = false;
                                InvalidPartCount++;
                            }
                        } else if (Droid.TurretCount >= 2)
                        {
                            if (Droid.Turret2 == null)
                            {
                                ValidDroid = false;
                                InvalidPartCount++;
                            } else if (Droid.Turret2.TurretType != enumTurretType.Weapon)
                            {
                                ValidDroid = false;
                                InvalidPartCount++;
                            }
                        } else if (Droid.TurretCount >= 3 && Droid.Turret3 == null)
                        {
                            if (Droid.Turret3 == null)
                            {
                                ValidDroid = false;
                                InvalidPartCount++;
                            } else if (Droid.Turret3.TurretType != enumTurretType.Weapon)
                            {
                                ValidDroid = false;
                                InvalidPartCount++;
                            }
                        }
                    }
                    if (ValidDroid)
                    {
                        File.AddSection ("droid_" + Unit.ID.ToStringInvariant ());
                        File.AddProperty ("id", Unit.ID.ToStringInvariant ());
                        if (Unit.UnitGroup == ScavengerUnitGroup || (PlayerCount >= 0 & Unit.UnitGroup.WZ_StartPos >= PlayerCount))
                        {
                            File.AddProperty ("player", "scavenger");
                        } else
                        {
                            File.AddProperty ("startpos", Unit.UnitGroup.WZ_StartPos.ToStringInvariant ());
                        }
                        if (AsPartsNotTemplate)
                        {
                            File.AddProperty ("name", Droid.GenerateName ());
                        } else
                        {
                            Template = (DroidTemplate)Droid;
                            File.AddProperty ("template", Template.Code);
                        }
                        File.AddProperty ("position", Unit.GetINIPosition ());
                        File.AddProperty ("rotation", Unit.GetINIRotation ());
                        if (Unit.Health < 1.0D)
                        {
                            File.AddProperty ("health", Unit.GetINIHealthPercent ());
                        }
                        if (AsPartsNotTemplate)
                        {
                            File.AddProperty ("droidType", Convert.ToInt32 (Droid.GetDroidType ()).ToStringInvariant ());
                            if (Droid.TurretCount == 0)
                            {
                                Text = "0";
                            } else
                            {
                                if (Droid.Turret1.TurretType == enumTurretType.Brain)
                                {
                                    if (((Brain)Droid.Turret1).Weapon == null)
                                    {
                                        Text = "0";
                                    } else
                                    {
                                        Text = "1";
                                    }
                                } else
                                {
                                    if (Droid.Turret1.TurretType == enumTurretType.Weapon)
                                    {
                                        Text = Droid.TurretCount.ToStringInvariant ();
                                    } else
                                    {
                                        Text = "0";
                                    }
                                }
                            }
                            File.AddProperty ("weapons", Text);
                            File.AddProperty ("parts\\body", Droid.Body.Code);
                            File.AddProperty ("parts\\propulsion", Droid.Propulsion.Code);
                            File.AddProperty ("parts\\sensor", Droid.GetSensorCode ());
                            File.AddProperty ("parts\\construct", Droid.GetConstructCode ());
                            File.AddProperty ("parts\\repair", Droid.GetRepairCode ());
                            File.AddProperty ("parts\\brain", Droid.GetBrainCode ());
                            File.AddProperty ("parts\\ecm", Droid.GetECMCode ());
                            if (Droid.TurretCount >= 1)
                            {
                                if (Droid.Turret1.TurretType == enumTurretType.Weapon)
                                {
                                    File.AddProperty ("parts\\weapon\\1", Droid.Turret1.Code);
                                    if (Droid.TurretCount >= 2)
                                    {
                                        if (Droid.Turret2.TurretType == enumTurretType.Weapon)
                                        {
                                            File.AddProperty ("parts\\weapon\\2", Droid.Turret2.Code);
                                            if (Droid.TurretCount >= 3)
                                            {
                                                if (Droid.Turret3.TurretType == enumTurretType.Weapon)
                                                {
                                                    File.AddProperty ("parts\\weapon\\3", Droid.Turret3.Code);
                                                }
                                            }
                                        }
                                    }
                                } else if (Droid.Turret1.TurretType == enumTurretType.Brain)
                                {
                                    Brain = (Brain)Droid.Turret1;
                                    if (Brain.Weapon == null)
                                    {
                                        Text = "ZNULLWEAPON";
                                    } else
                                    {
                                        Text = Brain.Weapon.Code;
                                    }
                                    File.AddProperty ("parts\\weapon\\1", Text);
                                }
                            }
                        }
                    }
                }
            }

            if (InvalidPartCount > 0)
            {
                ReturnResult.WarningAdd ("There were " + Convert.ToString (InvalidPartCount) + " droids with parts missing. They were not saved.");
            }

            return ReturnResult;
        }

        public clsResult Serialize_WZ_FeaturesINI(IniWriter File)
        {
            clsResult ReturnResult = new clsResult ("Serializing features INI", false);
            logger.Info ("Serializing features INI");
            FeatureTypeBase featureTypeBase = default(FeatureTypeBase);
            clsUnit Unit = default(clsUnit);
            bool Valid = default(bool);

            foreach (clsUnit tempLoopVar_Unit in Units)
            {
                Unit = tempLoopVar_Unit;
                if (Unit.TypeBase.Type != UnitType.Feature)
                {
                    continue;
                }

                featureTypeBase = (FeatureTypeBase)Unit.TypeBase;
                Valid = true;
                if (Unit.ID <= 0)
                {
                    Valid = false;
                    ReturnResult.WarningAdd ("Error. A features\'s ID was zero. It was NOT saved. Delete and replace it to allow save.");
                }
                if (Valid)
                {
                    File.AddSection ("feature_" + Unit.ID.ToStringInvariant ());
                    File.AddProperty ("id", Unit.ID.ToStringInvariant ());
                    File.AddProperty ("position", Unit.GetINIPosition ());
                    File.AddProperty ("rotation", Unit.GetINIRotation ());
                    File.AddProperty ("name", featureTypeBase.Code);
                    if (Unit.Health < 1.0D)
                    {
                        File.AddProperty ("health", Unit.GetINIHealthPercent ());
                    }
                }
            }

            return ReturnResult;
        }

        public clsResult Serialize_WZ_LabelsINI(IniWriter File, int PlayerCount)
        {
            clsResult returnResult = new clsResult ("Serializing labels INI", false);
            logger.Info ("Serializing labels INI");

            try
            {
                clsScriptPosition scriptPosition = default(clsScriptPosition);
                foreach (clsScriptPosition tempLoopVar_ScriptPosition in ScriptPositions)
                {
                    scriptPosition = tempLoopVar_ScriptPosition;
                    scriptPosition.WriteWZ (File);
                }
                clsScriptArea ScriptArea = default(clsScriptArea);
                foreach (clsScriptArea tempLoopVar_ScriptArea in ScriptAreas)
                {
                    ScriptArea = tempLoopVar_ScriptArea;
                    ScriptArea.WriteWZ (File);
                }
                if (PlayerCount >= 0) //not an FMap
                {
                    clsUnit Unit = default(clsUnit);
                    foreach (clsUnit tempLoopVar_Unit in Units)
                    {
                        Unit = tempLoopVar_Unit;
                        Unit.WriteWZLabel (File, PlayerCount);
                    }
                }
            } catch (Exception ex)
            {
                returnResult.WarningAdd (ex.Message);
                logger.ErrorException ("Got an exception", ex);
            }

            return returnResult;
        }

        public clsResult Serialize_WZ_LEV(Stream stream, int playercount, string authorname, string license, string mapName)
        {
            clsResult returnResult = new clsResult ("Serializing .lev", false);
            logger.Info ("Serializing .lev");

            StreamWriter fileLEV = new StreamWriter (stream, Encoding.UTF8);

            var playersText = playercount.ToString();
            var playersPrefix = playersText + "c-";
            string fog = "";
            string tilesetNum = "";
            string endChar = "\n";

            if (Tileset == App.Tileset_Arizona) {
                fog = "fog1.wrf";
                tilesetNum = "1";
            } else if (Tileset == App.Tileset_Urban) {
                fog = "fog2.wrf";
                tilesetNum = "2";
            } else if (Tileset == App.Tileset_Rockies) {
                fog = "fog3.wrf";
                tilesetNum = "3";
            } else {
                returnResult.ProblemAdd ("Map must have a tileset, or unknown tileset selected.");
                return returnResult;
            }

            fileLEV.Write (string.Format ("// Made with {0} {1} {2}{3}", Constants.ProgramName, Constants.ProgramVersionNumber, Constants.ProgramPlatform, Convert.ToString (endChar)));
            DateTime DateNow = DateTime.Now;
            fileLEV.Write (string.Format ("// Date: {0}/{1}/{2} {3}:{4}:{5}{6}", DateNow.Year, 
                                          App.MinDigits (DateNow.Month, 2), App.MinDigits (DateNow.Day, 2), 
                                          App.MinDigits (DateNow.Hour, 2), App.MinDigits (DateNow.Minute, 2), 
                                          App.MinDigits (DateNow.Second, 2), endChar));
            fileLEV.Write (string.Format ("// Author: {0}{1}", authorname, endChar));
            fileLEV.Write (string.Format ("// License: {0}{1}", license, endChar));
            fileLEV.Write (endChar);
            fileLEV.Write (string.Format ("level   {0}-T1{1}", mapName, endChar));
            fileLEV.Write (string.Format ("players {0}{1}", playersText, endChar));
            fileLEV.Write (string.Format ("type    14{0}", endChar));
            fileLEV.Write (string.Format ("dataset MULTI_CAM_{0}{1}", tilesetNum, endChar));
            fileLEV.Write (string.Format ("game    \"multiplay/maps/{0}{1}.gam\"{2}", playersPrefix, mapName, endChar));
            fileLEV.Write (string.Format ("data    \"wrf/multi/skirmish{0}.wrf\"{1}", playersText, endChar));
            fileLEV.Write (string.Format ("data    \"wrf/multi/{0}\"{1}", fog, endChar));
            fileLEV.Write (endChar);
            fileLEV.Write (string.Format ("level   {0}-T2{1}", mapName, endChar));
            fileLEV.Write (string.Format ("players {0}{1}", playersText, endChar));
            fileLEV.Write (string.Format ("type    18{0}", endChar));
            fileLEV.Write (string.Format ("dataset MULTI_T2_C{0}{1}", tilesetNum, endChar));
            fileLEV.Write (string.Format ("game    \"multiplay/maps/{0}{1}.gam\"{2}", playersPrefix, mapName, endChar));
            fileLEV.Write (string.Format ("data    \"wrf/multi/t2-skirmish{0}.wrf\"{1}", playersText, endChar));
            fileLEV.Write (string.Format ("data    \"wrf/multi/{0}\"{1}", fog, endChar));
            fileLEV.Write (endChar);
            fileLEV.Write (string.Format ("level   {0}-T3{1}", mapName, endChar));
            fileLEV.Write (string.Format ("players {0}{1}", playersText, endChar));
            fileLEV.Write (string.Format ("type    19{0}", endChar));
            fileLEV.Write (string.Format ("dataset MULTI_T3_C{0}{1}", tilesetNum, endChar));
            fileLEV.Write (string.Format ("game    \"multiplay/maps/{0}{1}.gam\"{2}", playersPrefix, mapName, endChar));
            fileLEV.Write (string.Format ("data    \"wrf/multi/t3-skirmish{0}.wrf\"{1}", playersText, endChar));
            fileLEV.Write (string.Format ("data    \"wrf/multi/{0}\"{1}", fog, endChar));
            fileLEV.Flush ();

            return returnResult;
        }

        public clsResult Write_WZ(sWrite_WZ_Args Args)
        {
            clsResult returnResult =
                new clsResult ("Compiling to \"{0}\"".Format2 (Args.Path), false);
            logger.Info ("Compiling to \"{0}\"".Format2 (Args.Path));

            try
            {
                switch (Args.CompileType)
                {
                case sWrite_WZ_Args.enumCompileType.Multiplayer:
                    if (Args.Multiplayer == null)
                    {
                        returnResult.ProblemAdd ("Multiplayer arguments were not passed.");
                        return returnResult;
                    }
                    if (Args.Multiplayer.PlayerCount < 2 | Args.Multiplayer.PlayerCount > Constants.PlayerCountMax)
                    {
                        returnResult.ProblemAdd (string.Format("Number of players was below 2 or above {0}.", Constants.PlayerCountMax));
                        return returnResult;
                    }
                    if (!Args.Multiplayer.IsBetaPlayerFormat)
                    {
                        if (!(Args.Multiplayer.PlayerCount == 2 | Args.Multiplayer.PlayerCount == 4 | Args.Multiplayer.PlayerCount == 8))
                        {
                            returnResult.ProblemAdd ("Number of players was not 2, 4 or 8 in original format.");
                            return returnResult;
                        }
                    }
                    break;
                case sWrite_WZ_Args.enumCompileType.Campaign:
                    if (Args.Campaign == null)
                    {
                        returnResult.ProblemAdd ("Campaign arguments were not passed.");
                        return returnResult;
                    }
                    break;
                default:
                    returnResult.ProblemAdd ("Unknown compile method.");
                    return returnResult;
                }

                if (!Args.Overwrite)
                {
                    if (File.Exists (Args.Path))
                    {
                        returnResult.ProblemAdd ("The selected file already exists.");
                        return returnResult;
                    }
                }

                MemoryStream fileMAPMemory = new MemoryStream ();
                BinaryWriter fileMAP = new BinaryWriter (fileMAPMemory, App.ASCIIEncoding);
                MemoryStream fileGAMMemory = new MemoryStream ();
                BinaryWriter fileGAM = new BinaryWriter (fileGAMMemory, App.ASCIIEncoding);
                MemoryStream filefeatBJOMemory = new MemoryStream ();
                BinaryWriter fileFeatBJO = new BinaryWriter (filefeatBJOMemory, App.ASCIIEncoding);
                MemoryStream iniFeatureMemory = new MemoryStream ();
                IniWriter iniFeature = new IniWriter(iniFeatureMemory);
                MemoryStream fileTTPMemory = new MemoryStream ();
                BinaryWriter fileTTP = new BinaryWriter (fileTTPMemory, App.ASCIIEncoding);
                MemoryStream filestructBJOMemory = new MemoryStream ();
                BinaryWriter fileStructBJO = new BinaryWriter (filestructBJOMemory, App.ASCIIEncoding);
                MemoryStream iniStructMemory = new MemoryStream ();
                IniWriter iniStruct = new IniWriter(iniStructMemory);
                MemoryStream fileDroidBJOMemory = new MemoryStream ();
                BinaryWriter fileDroidBJO = new BinaryWriter (fileDroidBJOMemory, App.ASCIIEncoding);
                MemoryStream iniDroidMemory = new MemoryStream ();
                IniWriter iniDroid = new IniWriter(iniDroidMemory);
                MemoryStream iniLabelsMemory = new MemoryStream ();
                IniWriter iniLabels = new IniWriter(iniLabelsMemory);



                byte[] GameZeroBytes = new byte[20];

                IOUtil.WriteText (fileGAM, false, "game");
                fileGAM.Write (8U);
                fileGAM.Write (0U); //Time
                if (Args.CompileType == sWrite_WZ_Args.enumCompileType.Multiplayer)
                {
                    fileGAM.Write (0U);
                } else if (Args.CompileType == sWrite_WZ_Args.enumCompileType.Campaign)
                {
                    fileGAM.Write (Args.Campaign.GAMType);
                }
                fileGAM.Write (Args.ScrollMin.X);
                fileGAM.Write (Args.ScrollMin.Y);
                fileGAM.Write (Args.ScrollMax.X);
                fileGAM.Write (Args.ScrollMax.Y);
                fileGAM.Write (GameZeroBytes);

                int A = 0;
                int X = 0;
                int Y = 0;

                IOUtil.WriteText (fileMAP, false, "map ");
                fileMAP.Write (10U);
                fileMAP.Write ((uint)Terrain.TileSize.X);
                fileMAP.Write ((uint)Terrain.TileSize.Y);
                byte Flip = 0;
                byte Rotation = 0;
                bool DoFlipX = default(bool);
                int InvalidTileCount = 0;
                int TextureNum = 0;
                for (Y = 0; Y <= Terrain.TileSize.Y - 1; Y++)
                {
                    for (X = 0; X <= Terrain.TileSize.X - 1; X++)
                    {
                        TileUtil.TileOrientation_To_OldOrientation (Terrain.Tiles [X, Y].Texture.Orientation, ref Rotation, ref DoFlipX);
                        Flip = (byte)0;
                        if (Terrain.Tiles [X, Y].Tri)
                        {
                            Flip += (byte)8;
                        }
                        Flip += (byte)(Rotation * 16);
                        if (DoFlipX)
                        {
                            Flip += (byte)128;
                        }
                        TextureNum = Terrain.Tiles [X, Y].Texture.TextureNum;
                        if (TextureNum < 0 | TextureNum > 255)
                        {
                            TextureNum = 0;
                            if (InvalidTileCount < 16)
                            {
                                returnResult.WarningAdd ("Tile texture number " + Convert.ToString (Terrain.Tiles [X, Y].Texture.TextureNum) +
                                    " is invalid on tile " + Convert.ToString (X) + ", " + Convert.ToString (Y) +
                                    " and was compiled as texture number " + Convert.ToString (TextureNum) + ".");
                            }
                            InvalidTileCount++;
                        }
                        fileMAP.Write ((byte)TextureNum);
                        fileMAP.Write (Flip);
                        fileMAP.Write (Terrain.Vertices [X, Y].Height);
                    }
                }
                if (InvalidTileCount > 0)
                {
                    returnResult.WarningAdd (InvalidTileCount + " tile texture numbers were invalid.");
                }
                fileMAP.Write (1U); //gateway version
                fileMAP.Write ((uint)Gateways.Count);
                foreach (clsGateway gateway in Gateways)
                {
                    fileMAP.Write ((byte)(MathUtil.Clamp_int (gateway.PosA.X, 0, 255)));
                    fileMAP.Write ((byte)(MathUtil.Clamp_int (gateway.PosA.Y, 0, 255)));
                    fileMAP.Write ((byte)(MathUtil.Clamp_int (gateway.PosB.X, 0, 255)));
                    fileMAP.Write ((byte)(MathUtil.Clamp_int (gateway.PosB.Y, 0, 255)));
                }

                FeatureTypeBase featureTypeBase;
                StructureTypeBase structureTypeBase;
                DroidDesign DroidType;
                DroidTemplate DroidTemplate;
                clsUnit Unit;
                clsStructureWriteWZ StructureWrite = new clsStructureWriteWZ {
                    File = fileStructBJO, 
                    CompileType = Args.CompileType
                };
                if (Args.CompileType == sWrite_WZ_Args.enumCompileType.Multiplayer)
                {
                    StructureWrite.PlayerCount = Args.Multiplayer.PlayerCount;
                } else
                {
                    StructureWrite.PlayerCount = 0;
                }

                byte[] FeatZeroBytes = new byte[12];

                IOUtil.WriteText (fileFeatBJO, false, "feat");
                fileFeatBJO.Write (8U);
                clsObjectPriorityOrderList FeatureOrder = new clsObjectPriorityOrderList ();
                foreach (clsUnit tempLoopVar_Unit in Units)
                {
                    Unit = tempLoopVar_Unit;
                    if (Unit.TypeBase.Type == UnitType.Feature)
                    {
                        FeatureOrder.SetItem (Unit);
                        FeatureOrder.ActionPerform ();
                    }
                }
                fileFeatBJO.Write ((uint)FeatureOrder.Result.Count);
                for (A = 0; A <= FeatureOrder.Result.Count - 1; A++)
                {
                    Unit = FeatureOrder.Result [A];
                    featureTypeBase = (FeatureTypeBase)Unit.TypeBase;
                    IOUtil.WriteTextOfLength (fileFeatBJO, 40, featureTypeBase.Code);
                    fileFeatBJO.Write (Unit.ID);
                    fileFeatBJO.Write ((uint)Unit.Pos.Horizontal.X);
                    fileFeatBJO.Write ((uint)Unit.Pos.Horizontal.Y);
                    fileFeatBJO.Write ((uint)Unit.Pos.Altitude);
                    fileFeatBJO.Write ((uint)Unit.Rotation);
                    switch (Args.CompileType)
                    {
                    case sWrite_WZ_Args.enumCompileType.Multiplayer:
                        fileFeatBJO.Write (Unit.GetBJOMultiplayerPlayerNum (Args.Multiplayer.PlayerCount));
                        break;
                    case sWrite_WZ_Args.enumCompileType.Campaign:
                        fileFeatBJO.Write (Unit.GetBJOCampaignPlayerNum ());
                        break;
                    default:
                        Debugger.Break ();
                        break;
                    }
                    fileFeatBJO.Write (FeatZeroBytes);
                }

                IOUtil.WriteText (fileTTP, false, "ttyp");
                fileTTP.Write (8U);
                fileTTP.Write ((uint)Tileset.TileCount);
                for (A = 0; A <= Tileset.TileCount - 1; A++)
                {
                    fileTTP.Write ((ushort)Tile_TypeNum [A]);
                }

                IOUtil.WriteText (fileStructBJO, false, "stru");
                fileStructBJO.Write (8U);
                clsObjectPriorityOrderList NonModuleStructureOrder = new clsObjectPriorityOrderList ();
                //non-module structures
                foreach (clsUnit tempLoopVar_Unit in Units)
                {
                    Unit = tempLoopVar_Unit;
                    if (Unit.TypeBase.Type == UnitType.PlayerStructure)
                    {
                        structureTypeBase = (StructureTypeBase)Unit.TypeBase;
                        if (!structureTypeBase.IsModule ())
                        {
                            NonModuleStructureOrder.SetItem (Unit);
                            NonModuleStructureOrder.ActionPerform ();
                        }
                    }
                }
                clsObjectPriorityOrderList ModuleStructureOrder = new clsObjectPriorityOrderList ();
                //module structures
                foreach (clsUnit tempLoopVar_Unit in Units)
                {
                    Unit = tempLoopVar_Unit;
                    if (Unit.TypeBase.Type == UnitType.PlayerStructure)
                    {
                        structureTypeBase = (StructureTypeBase)Unit.TypeBase;
                        if (structureTypeBase.IsModule ())
                        {
                            ModuleStructureOrder.SetItem (Unit);
                            ModuleStructureOrder.ActionPerform ();
                        }
                    }
                }
                fileStructBJO.Write ((uint)(NonModuleStructureOrder.Result.Count + ModuleStructureOrder.Result.Count));
                NonModuleStructureOrder.Result.PerformTool (StructureWrite);
                ModuleStructureOrder.Result.PerformTool (StructureWrite);

                byte[] DintZeroBytes = new byte[12];

                IOUtil.WriteText (fileDroidBJO, false, "dint");
                fileDroidBJO.Write (8U);
                clsObjectPriorityOrderList Droids = new clsObjectPriorityOrderList ();
                foreach (clsUnit tempLoopVar_Unit in Units)
                {
                    Unit = tempLoopVar_Unit;
                    if (Unit.TypeBase.Type == UnitType.PlayerDroid)
                    {
                        DroidType = (DroidDesign)Unit.TypeBase;
                        if (DroidType.IsTemplate)
                        {
                            Droids.SetItem (Unit);
                            Droids.ActionPerform ();
                        }
                    }
                }
                fileDroidBJO.Write ((uint)Droids.Result.Count);
                for (A = 0; A <= Droids.Result.Count - 1; A++)
                {
                    Unit = Droids.Result [A];
                    DroidTemplate = (DroidTemplate)Unit.TypeBase;
                    IOUtil.WriteTextOfLength (fileDroidBJO, 40, DroidTemplate.Code);
                    fileDroidBJO.Write (Unit.ID);
                    fileDroidBJO.Write ((uint)Unit.Pos.Horizontal.X);
                    fileDroidBJO.Write ((uint)Unit.Pos.Horizontal.Y);
                    fileDroidBJO.Write ((uint)Unit.Pos.Altitude);
                    fileDroidBJO.Write ((uint)Unit.Rotation);
                    switch (Args.CompileType)
                    {
                    case sWrite_WZ_Args.enumCompileType.Multiplayer:
                        fileDroidBJO.Write (Unit.GetBJOMultiplayerPlayerNum (Args.Multiplayer.PlayerCount));
                        break;
                    case sWrite_WZ_Args.enumCompileType.Campaign:
                        fileDroidBJO.Write (Unit.GetBJOCampaignPlayerNum ());
                        break;
                    default:
                        Debugger.Break ();
                        break;
                    }
                    fileDroidBJO.Write (DintZeroBytes);
                }

                returnResult.Add (Serialize_WZ_FeaturesINI (iniFeature));
                iniFeature.Flush();
                if (Args.CompileType == sWrite_WZ_Args.enumCompileType.Multiplayer)
                {
                    returnResult.Add (Serialize_WZ_StructuresINI (iniStruct, Args.Multiplayer.PlayerCount));
                    iniStruct.Flush();
                    returnResult.Add (Serialize_WZ_DroidsINI (iniDroid, Args.Multiplayer.PlayerCount));
                    iniDroid.Flush();
                    returnResult.Add (Serialize_WZ_LabelsINI (iniLabels, Args.Multiplayer.PlayerCount));
                    iniLabels.Flush();
                } else if (Args.CompileType == sWrite_WZ_Args.enumCompileType.Campaign)
                {
                    returnResult.Add (Serialize_WZ_StructuresINI (iniStruct, -1));
                    returnResult.Add (Serialize_WZ_DroidsINI (iniDroid, -1));
                    returnResult.Add (Serialize_WZ_LabelsINI (iniLabels, 0)); //interprets -1 players as an FMap
                }

                fileMAP.Flush ();
                fileGAM.Flush ();
                fileFeatBJO.Flush ();
                fileTTP.Flush ();
                fileStructBJO.Flush ();
                fileDroidBJO.Flush ();

                if (Args.CompileType == sWrite_WZ_Args.enumCompileType.Multiplayer)
                {
                    if (!Args.Overwrite)
                    {
                        if (File.Exists (Args.Path))
                        {
                            returnResult.ProblemAdd ("A file already exists at: " + Args.Path);
                            return returnResult;
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
                            if (Args.Multiplayer.IsBetaPlayerFormat)
                            {
                                zipPath = string.Format ("{0}c-{1}.xplayers.lev", Args.Multiplayer.PlayerCount, Args.MapName);
                            } else
                            {
                                zipPath = string.Format ("{0}c-{1}.addon.lev", Args.Multiplayer.PlayerCount, Args.MapName);
                            }
                            if (Args.CompileType == sWrite_WZ_Args.enumCompileType.Multiplayer)
                            {
                                zip.PutNextEntry (zipPath);
                                returnResult.Add (Serialize_WZ_LEV(zip, Args.Multiplayer.PlayerCount, 
                                                                   Args.Multiplayer.AuthorName, Args.Multiplayer.License, 
                                                                   Args.MapName));
                            }

                            var path = string.Format ("multiplay/maps/{0}c-{1}", Args.Multiplayer.PlayerCount, Args.MapName);
                            zip.PutNextEntry (string.Format ("{0}.gam", path));
                            fileGAMMemory.WriteTo (zip);
                            fileGAMMemory.Flush ();

                            zip.PutNextEntry (string.Format ("{0}/dinit.bjo", path));
                            fileDroidBJOMemory.WriteTo (zip);
                            fileDroidBJOMemory.Flush ();

                            zip.PutNextEntry (string.Format ("{0}/droid.ini", path));
                            iniDroidMemory.WriteTo (zip);
                            iniDroidMemory.Flush ();

                            zip.PutNextEntry (string.Format ("{0}/feat.bjo", path));
                            filefeatBJOMemory.WriteTo (zip);
                            filefeatBJOMemory.Flush ();

                            zip.PutNextEntry (string.Format ("{0}/feature.ini", path));
                            iniFeatureMemory.WriteTo (zip);
                            iniFeatureMemory.Flush ();

                            zip.PutNextEntry (string.Format ("{0}/game.map", path));
                            fileMAPMemory.WriteTo (zip);
                            fileMAPMemory.Flush ();

                            zip.PutNextEntry (string.Format ("{0}/struct.bjo", path));
                            filestructBJOMemory.WriteTo (zip);
                            filestructBJOMemory.Flush ();

                            zip.PutNextEntry (string.Format ("{0}/struct.ini", path));
                            iniStructMemory.WriteTo (zip);
                            iniStructMemory.Flush ();

                            zip.PutNextEntry (string.Format ("{0}/ttypes.ttp", path));
                            fileTTPMemory.WriteTo (zip);
                            fileTTPMemory.Flush ();

                            if (iniLabelsMemory.Length > 0)
                            {
                                zip.PutNextEntry (string.Format ("{0}/labels.ini", path));
                                iniLabelsMemory.WriteTo (zip);
                                iniLabelsMemory.Flush ();
                            }
                        }                    
                    } catch (Exception ex)
                    {
                        returnResult.ProblemAdd (ex.Message);
                        logger.ErrorException ("Got an exception", ex);
                        return returnResult;
                    }

                    return returnResult;
                } else if (Args.CompileType == sWrite_WZ_Args.enumCompileType.Campaign)
                {
                    string CampDirectory = PathUtil.EndWithPathSeperator (Args.Path);

                    if (!Directory.Exists (CampDirectory))
                    {
                        returnResult.ProblemAdd ("Directory " + CampDirectory + " does not exist.");
                        return returnResult;
                    }

                    string FilePath = "";

                    FilePath = CampDirectory + Args.MapName + ".gam";
                    returnResult.Add (IOUtil.WriteMemoryToNewFile (fileGAMMemory, CampDirectory + Args.MapName + ".gam"));

                    CampDirectory += Args.MapName + Convert.ToString (App.PlatformPathSeparator);
                    try
                    {
                        Directory.CreateDirectory (CampDirectory);
                    } catch (Exception ex)
                    {
                        returnResult.ProblemAdd ("Unable to create directory " + CampDirectory);
                        logger.ErrorException ("Got an exception", ex);
                        return returnResult;
                    }

                    FilePath = CampDirectory + "dinit.bjo";
                    returnResult.Add (IOUtil.WriteMemoryToNewFile (fileDroidBJOMemory, FilePath));

                    FilePath = CampDirectory + "droid.ini";
                    returnResult.Add (IOUtil.WriteMemoryToNewFile (iniDroidMemory, FilePath));

                    FilePath = CampDirectory + "feat.bjo";
                    returnResult.Add (IOUtil.WriteMemoryToNewFile (filefeatBJOMemory, FilePath));

                    FilePath = CampDirectory + "feature.ini";
                    returnResult.Add (IOUtil.WriteMemoryToNewFile (iniFeatureMemory, FilePath));

                    FilePath = CampDirectory + "game.map";
                    returnResult.Add (IOUtil.WriteMemoryToNewFile (fileMAPMemory, FilePath));

                    FilePath = CampDirectory + "struct.bjo";
                    returnResult.Add (IOUtil.WriteMemoryToNewFile (filestructBJOMemory, FilePath));

                    FilePath = CampDirectory + "struct.ini";
                    returnResult.Add (IOUtil.WriteMemoryToNewFile (iniStructMemory, FilePath));

                    FilePath = CampDirectory + "ttypes.ttp";
                    returnResult.Add (IOUtil.WriteMemoryToNewFile (fileTTPMemory, FilePath));

                    FilePath = CampDirectory + "labels.ini";
                    returnResult.Add (IOUtil.WriteMemoryToNewFile (iniLabelsMemory, FilePath));
                }
            } catch (Exception ex)
            {
                Debugger.Break ();
                returnResult.ProblemAdd (ex.Message);
                logger.ErrorException ("Got an exception", ex);
                return returnResult;
            }

            return returnResult;
        }

        private sResult Read_TTP(BinaryReader File)
        {
            sResult ReturnResult = new sResult ();
            ReturnResult.Success = false;
            ReturnResult.Problem = "";

            string strTemp = "";
            UInt32 uintTemp = 0;
            UInt16 ushortTemp = 0;
            int A = 0;

            try
            {
                strTemp = IOUtil.ReadOldTextOfLength (File, 4);
                if (strTemp != "ttyp")
                {
                    ReturnResult.Problem = "Incorrect identifier.";
                    return ReturnResult;
                }

                uintTemp = File.ReadUInt32 ();
                if (uintTemp != 8U)
                {
                    ReturnResult.Problem = "Unknown version.";
                    return ReturnResult;
                }
                uintTemp = File.ReadUInt32 ();
                for (A = 0; A <= ((int)(Math.Min(uintTemp, (uint)Tileset.TileCount))) - 1; A++)
                {
                    ushortTemp = File.ReadUInt16 ();
                    if (ushortTemp > 11)
                    {
                        ReturnResult.Problem = "Unknown tile type number.";
                        return ReturnResult;
                    }
                    Tile_TypeNum [A] = (byte)ushortTemp;
                }
            } catch (Exception ex)
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
