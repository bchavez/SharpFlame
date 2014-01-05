using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.VisualBasic;
using SharpFlame.Collections;
using SharpFlame.FileIO;
using SharpFlame.MathExtra;

namespace SharpFlame
{
    public partial class clsMap
    {
        public class clsWZBJOUnit
        {
            public string Code;
            public UInt32 ID;
            public modProgram.sWorldPos Pos;
            public UInt32 Rotation;
            public UInt32 Player;
            public clsUnitType.enumType ObjectType;
        }

        public class clsWZMapEntry
        {
            public string Name;
            public clsTileset Tileset;
        }

        public clsResult Load_WZ(string Path)
        {
            clsResult ReturnResult =
                new clsResult("Loading WZ from " + Convert.ToString(ControlChars.Quote) + Path + Convert.ToString(ControlChars.Quote));
            modProgram.sResult SubResult = new modProgram.sResult();
            string Quote = ControlChars.Quote.ToString();
            ZipEntry ZipEntry = default(ZipEntry);
            bool GameFound = default(bool);
            bool DatasetFound = default(bool);
            SimpleList<clsWZMapEntry> Maps = new SimpleList<clsWZMapEntry>();
            clsTileset GameTileset = null;
            string GameName = "";
            string strTemp = "";
            modProgram.sZipSplitPath SplitPath;
            int A = 0;
            int B = 0;
            int C = 0;
            int D = 0;

            FileStream File = default(FileStream);
            try
            {
                File = System.IO.File.OpenRead(Path);
            }
            catch ( Exception ex )
            {
                ReturnResult.ProblemAdd(ex.Message);
                return ReturnResult;
            }

            ZipInputStream ZipStream = new ZipInputStream(File);

            //get all usable lev entries
            do
            {
                ZipEntry = ZipStream.GetNextEntry();
                if ( ZipEntry == null )
                {
                    break;
                }

                SplitPath = new modProgram.sZipSplitPath(ZipEntry.Name);

                if ( SplitPath.FileExtension == "lev" && SplitPath.PartCount == 1 )
                {
                    if ( ZipEntry.Size > 10 * 1024 * 1024 )
                    {
                        ReturnResult.ProblemAdd("lev file is too large.");
                        ZipStream.Close();
                        return ReturnResult;
                    }
                    BinaryReader Reader = new BinaryReader(ZipStream);
                    SimpleList<string> LineData = IOUtil.BytesToLinesRemoveComments(Reader);
                    //find each level block
                    for ( A = 0; A <= LineData.Count - 1; A++ )
                    {
                        if ( Strings.LCase(Strings.Left(LineData[A], 5)) == "level" )
                        {
                            //find each levels game file
                            GameFound = false;
                            B = 1;
                            while ( A + B < LineData.Count )
                            {
                                if ( Strings.LCase(Strings.Left(Convert.ToString(LineData[A + B]), 4)) == "game" )
                                {
                                    C = Strings.InStr(Convert.ToString(LineData[A + B]), Quote, (CompareMethod)0);
                                    D = Strings.InStrRev(Convert.ToString(LineData[A + B]), Quote, -1, (CompareMethod)0);
                                    if ( C > 0 & D > 0 & D - C > 1 )
                                    {
                                        GameName = Strings.LCase(Strings.Mid(Convert.ToString(LineData[A + B]), C + 1, D - C - 1));
                                        //see if map is already counted
                                        for ( C = 0; C <= Maps.Count - 1; C++ )
                                        {
                                            if ( GameName == Maps[C].Name )
                                            {
                                                break;
                                            }
                                        }
                                        if ( C == Maps.Count )
                                        {
                                            GameFound = true;
                                        }
                                    }
                                    break;
                                }
                                else if ( Strings.LCase(Strings.Left(Convert.ToString(LineData[A + B]), 5)) == "level" )
                                {
                                    break;
                                }
                                B++;
                            }
                            if ( GameFound )
                            {
                                //find the dataset (determines tileset)
                                DatasetFound = false;
                                B = 1;
                                while ( A + B < LineData.Count )
                                {
                                    if ( Strings.LCase(Strings.Left(Convert.ToString(LineData[A + B]), 7)) == "dataset" )
                                    {
                                        strTemp = Strings.LCase(Strings.Right(Convert.ToString(LineData[A + B]), 1));
                                        if ( strTemp == "1" )
                                        {
                                            GameTileset = modProgram.Tileset_Arizona;
                                            DatasetFound = true;
                                        }
                                        else if ( strTemp == "2" )
                                        {
                                            GameTileset = modProgram.Tileset_Urban;
                                            DatasetFound = true;
                                        }
                                        else if ( strTemp == "3" )
                                        {
                                            GameTileset = modProgram.Tileset_Rockies;
                                            DatasetFound = true;
                                        }
                                        break;
                                    }
                                    else if ( Strings.LCase(Strings.Left(Convert.ToString(LineData[A + B]), 5)) == "level" )
                                    {
                                        break;
                                    }
                                    B++;
                                }
                                if ( DatasetFound )
                                {
                                    clsWZMapEntry NewMap = new clsWZMapEntry();
                                    NewMap.Name = GameName;
                                    NewMap.Tileset = GameTileset;
                                    Maps.Add(NewMap);
                                }
                            }
                        }
                    }
                }
            } while ( true );
            ZipStream.Close();

            string MapLoadName = "";

            //prompt user for which of the entries to load
            if ( Maps.Count < 1 )
            {
                ReturnResult.ProblemAdd("No maps found in file.");
                return ReturnResult;
            }
            else if ( Maps.Count == 1 )
            {
                MapLoadName = Convert.ToString(Maps[0].Name);
                Tileset = Maps[0].Tileset;
            }
            else
            {
                frmWZLoad.clsOutput SelectToLoadResult = new frmWZLoad.clsOutput();
                string[] Names = new string[Maps.Count];
                for ( A = 0; A <= Maps.Count - 1; A++ )
                {
                    Names[A] = Convert.ToString(Maps[A].Name);
                }
                frmWZLoad SelectToLoadForm = new frmWZLoad(Names, SelectToLoadResult,
                    "Select a map from " + Convert.ToString(new modProgram.sSplitPath(Path).FileTitle));
                SelectToLoadForm.ShowDialog();
                if ( SelectToLoadResult.Result < 0 )
                {
                    ReturnResult.ProblemAdd("No map selected.");
                    return ReturnResult;
                }
                MapLoadName = Convert.ToString(Maps[SelectToLoadResult.Result].Name);
                Tileset = Maps[SelectToLoadResult.Result].Tileset;
            }

            TileType_Reset();
            SetPainterToDefaults();

            modProgram.sZipSplitPath GameSplitPath = new modProgram.sZipSplitPath(MapLoadName);
            string GameFilesPath = GameSplitPath.FilePath + GameSplitPath.FileTitleWithoutExtension + "/";

            ZipStreamEntry ZipSearchResult = default(ZipStreamEntry);

            ZipSearchResult = IOUtil.FindZipEntryFromPath(Path, MapLoadName);
            if ( ZipSearchResult == null )
            {
                ReturnResult.ProblemAdd("Game file not found.");
                return ReturnResult;
            }
            else
            {
                BinaryReader Map_Reader = new BinaryReader(ZipSearchResult.Stream);
                SubResult = Read_WZ_gam(Map_Reader);
                Map_Reader.Close();

                if ( !SubResult.Success )
                {
                    ReturnResult.ProblemAdd(SubResult.Problem);
                    return ReturnResult;
                }
            }

            ZipSearchResult = IOUtil.FindZipEntryFromPath(Path, GameFilesPath + "game.map");
            if ( ZipSearchResult == null )
            {
                ReturnResult.ProblemAdd("game.map file not found");
                return ReturnResult;
            }
            else
            {
                BinaryReader Map_Reader = new BinaryReader(ZipSearchResult.Stream);
                SubResult = Read_WZ_map(Map_Reader);
                Map_Reader.Close();

                if ( !SubResult.Success )
                {
                    ReturnResult.ProblemAdd(SubResult.Problem);
                    return ReturnResult;
                }
            }

            SimpleClassList<clsWZBJOUnit> BJOUnits = new SimpleClassList<clsWZBJOUnit>();

            clsINIFeatures INIFeatures = null;

            ZipSearchResult = IOUtil.FindZipEntryFromPath(Path, GameFilesPath + "feature.ini");
            if ( ZipSearchResult == null )
            {
            }
            else
            {
                clsResult Result = new clsResult("feature.ini");
                clsINIRead FeaturesINI = new clsINIRead();
                StreamReader FeaturesINI_Reader = new StreamReader(ZipSearchResult.Stream);
                Result.Take(FeaturesINI.ReadFile(FeaturesINI_Reader));
                FeaturesINI_Reader.Close();
                INIFeatures = new clsINIFeatures(FeaturesINI.Sections.Count);
                Result.Take(FeaturesINI.Translate(INIFeatures));
                ReturnResult.Add(Result);
            }

            if ( INIFeatures == null )
            {
                clsResult Result = new clsResult("feat.bjo");
                ZipSearchResult = IOUtil.FindZipEntryFromPath(Path, GameFilesPath + "feat.bjo");
                if ( ZipSearchResult == null )
                {
                    Result.WarningAdd("file not found");
                }
                else
                {
                    BinaryReader Features_Reader = new BinaryReader(ZipSearchResult.Stream);
                    SubResult = Read_WZ_Features(Features_Reader, BJOUnits);
                    Features_Reader.Close();
                    if ( !SubResult.Success )
                    {
                        Result.WarningAdd(SubResult.Problem);
                    }
                }
                ReturnResult.Add(Result);
            }

            if ( true )
            {
                clsResult Result = new clsResult("ttypes.ttp");
                ZipSearchResult = IOUtil.FindZipEntryFromPath(Path, GameFilesPath + "ttypes.ttp");
                if ( ZipSearchResult == null )
                {
                    Result.WarningAdd("file not found");
                }
                else
                {
                    BinaryReader TileTypes_Reader = new BinaryReader(ZipSearchResult.Stream);
                    SubResult = Read_WZ_TileTypes(TileTypes_Reader);
                    TileTypes_Reader.Close();
                    if ( !SubResult.Success )
                    {
                        Result.WarningAdd(SubResult.Problem);
                    }
                }
                ReturnResult.Add(Result);
            }

            clsINIStructures INIStructures = null;

            ZipSearchResult = IOUtil.FindZipEntryFromPath(Path, GameFilesPath + "struct.ini");
            if ( ZipSearchResult == null )
            {
            }
            else
            {
                clsResult Result = new clsResult("struct.ini");
                clsINIRead StructuresINI = new clsINIRead();
                StreamReader StructuresINI_Reader = new StreamReader(ZipSearchResult.Stream);
                Result.Take(StructuresINI.ReadFile(StructuresINI_Reader));
                StructuresINI_Reader.Close();
                INIStructures = new clsINIStructures(StructuresINI.Sections.Count, this);
                Result.Take(StructuresINI.Translate(INIStructures));
                ReturnResult.Add(Result);
            }

            if ( INIStructures == null )
            {
                clsResult Result = new clsResult("struct.bjo");
                ZipSearchResult = IOUtil.FindZipEntryFromPath(Path, GameFilesPath + "struct.bjo");
                if ( ZipSearchResult == null )
                {
                    Result.WarningAdd("file not found");
                }
                else
                {
                    BinaryReader Structures_Reader = new BinaryReader(ZipSearchResult.Stream);
                    SubResult = Read_WZ_Structures(Structures_Reader, BJOUnits);
                    Structures_Reader.Close();
                    if ( !SubResult.Success )
                    {
                        Result.WarningAdd(SubResult.Problem);
                    }
                }
                ReturnResult.Add(Result);
            }

            clsINIDroids INIDroids = null;

            ZipSearchResult = IOUtil.FindZipEntryFromPath(Path, GameFilesPath + "droid.ini");
            if ( ZipSearchResult == null )
            {
            }
            else
            {
                clsResult Result = new clsResult("droid.ini");
                clsINIRead DroidsINI = new clsINIRead();
                StreamReader DroidsINI_Reader = new StreamReader(ZipSearchResult.Stream);
                Result.Take(DroidsINI.ReadFile(DroidsINI_Reader));
                DroidsINI_Reader.Close();
                INIDroids = new clsINIDroids(DroidsINI.Sections.Count, this);
                Result.Take(DroidsINI.Translate(INIDroids));
                ReturnResult.Add(Result);
            }

            if ( INIDroids == null )
            {
                clsResult Result = new clsResult("dinit.bjo");
                ZipSearchResult = IOUtil.FindZipEntryFromPath(Path, GameFilesPath + "dinit.bjo");
                if ( ZipSearchResult == null )
                {
                    Result.WarningAdd("file not found");
                }
                else
                {
                    BinaryReader Droids_Reader = new BinaryReader(ZipSearchResult.Stream);
                    SubResult = Read_WZ_Droids(Droids_Reader, BJOUnits);
                    Droids_Reader.Close();
                    if ( !SubResult.Success )
                    {
                        Result.WarningAdd(SubResult.Problem);
                    }
                }
                ReturnResult.Add(Result);
            }

            sCreateWZObjectsArgs CreateObjectsArgs = new sCreateWZObjectsArgs();
            CreateObjectsArgs.BJOUnits = BJOUnits;
            CreateObjectsArgs.INIStructures = INIStructures;
            CreateObjectsArgs.INIDroids = INIDroids;
            CreateObjectsArgs.INIFeatures = INIFeatures;
            ReturnResult.Add(CreateWZObjects(CreateObjectsArgs));

            //objects are modified by this and must already exist
            ZipSearchResult = IOUtil.FindZipEntryFromPath(Path, GameFilesPath + "labels.ini");
            if ( ZipSearchResult == null )
            {
            }
            else
            {
                clsResult Result = new clsResult("labels.ini");
                clsINIRead LabelsINI = new clsINIRead();
                StreamReader LabelsINI_Reader = new StreamReader(ZipSearchResult.Stream);
                Result.Take(LabelsINI.ReadFile(LabelsINI_Reader));
                LabelsINI_Reader.Close();
                Result.Take(Read_WZ_Labels(LabelsINI, false));
                ReturnResult.Add(Result);
            }

            return ReturnResult;
        }

        public clsResult Load_Game(string Path)
        {
            clsResult ReturnResult =
                new clsResult("Loading game file from " + Convert.ToString(ControlChars.Quote) + Path + Convert.ToString(ControlChars.Quote));
            modProgram.sResult SubResult = new modProgram.sResult();
            string Quote = ControlChars.Quote.ToString();

            Tileset = null;

            TileType_Reset();
            SetPainterToDefaults();

            modProgram.sSplitPath GameSplitPath = new modProgram.sSplitPath(Path);
            string GameFilesPath = GameSplitPath.FilePath + GameSplitPath.FileTitleWithoutExtension + Convert.ToString(modProgram.PlatformPathSeparator);
            string MapDirectory = "";
            FileStream File = null;

            SubResult = IOUtil.TryOpenFileStream(Path, ref File);
            if ( !SubResult.Success )
            {
                ReturnResult.ProblemAdd("Game file not found: " + SubResult.Problem);
                return ReturnResult;
            }
            else
            {
                BinaryReader Map_Reader = new BinaryReader(File);
                SubResult = Read_WZ_gam(Map_Reader);
                Map_Reader.Close();

                if ( !SubResult.Success )
                {
                    ReturnResult.ProblemAdd(SubResult.Problem);
                    return ReturnResult;
                }
            }

            SubResult = IOUtil.TryOpenFileStream(GameFilesPath + "game.map", ref File);
            if ( !SubResult.Success )
            {
                MsgBoxResult PromptResult =
                    Interaction.MsgBox(
                        "game.map file not found at " + GameFilesPath + ControlChars.NewLine + "Do you want to select another directory to load the underlying map from?",
                        (MsgBoxStyle)(MsgBoxStyle.OkCancel | MsgBoxStyle.Question), null);
                if ( PromptResult != MsgBoxResult.Ok )
                {
                    ReturnResult.ProblemAdd("Aborted.");
                    return ReturnResult;
                }
                FolderBrowserDialog DirectorySelect = new FolderBrowserDialog();
                DirectorySelect.SelectedPath = GameFilesPath;
                if ( DirectorySelect.ShowDialog() != DialogResult.OK )
                {
                    ReturnResult.ProblemAdd("Aborted.");
                    return ReturnResult;
                }
                MapDirectory = DirectorySelect.SelectedPath + Convert.ToString(modProgram.PlatformPathSeparator);

                SubResult = IOUtil.TryOpenFileStream(MapDirectory + "game.map", ref File);
                if ( !SubResult.Success )
                {
                    ReturnResult.ProblemAdd("game.map file not found: " + SubResult.Problem);
                    return ReturnResult;
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
                ReturnResult.ProblemAdd(SubResult.Problem);
                return ReturnResult;
            }

            SimpleClassList<clsWZBJOUnit> BJOUnits = new SimpleClassList<clsWZBJOUnit>();

            clsINIFeatures INIFeatures = null;

            SubResult = IOUtil.TryOpenFileStream(GameFilesPath + "feature.ini", ref File);
            if ( !SubResult.Success )
            {
            }
            else
            {
                clsResult Result = new clsResult("feature.ini");
                clsINIRead FeaturesINI = new clsINIRead();
                StreamReader FeaturesINI_Reader = new StreamReader(File);
                Result.Take(FeaturesINI.ReadFile(FeaturesINI_Reader));
                FeaturesINI_Reader.Close();
                INIFeatures = new clsINIFeatures(FeaturesINI.Sections.Count);
                Result.Take(FeaturesINI.Translate(INIFeatures));
                ReturnResult.Add(Result);
            }

            if ( INIFeatures == null )
            {
                clsResult Result = new clsResult("feat.bjo");
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
                ReturnResult.Add(Result);
            }

            if ( true )
            {
                clsResult Result = new clsResult("ttypes.ttp");
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
                ReturnResult.Add(Result);
            }

            clsINIStructures INIStructures = null;

            SubResult = IOUtil.TryOpenFileStream(GameFilesPath + "struct.ini", ref File);
            if ( !SubResult.Success )
            {
            }
            else
            {
                clsResult Result = new clsResult("struct.ini");
                clsINIRead StructuresINI = new clsINIRead();
                StreamReader StructuresINI_Reader = new StreamReader(File);
                Result.Take(StructuresINI.ReadFile(StructuresINI_Reader));
                StructuresINI_Reader.Close();
                INIStructures = new clsINIStructures(StructuresINI.Sections.Count, this);
                Result.Take(StructuresINI.Translate(INIStructures));
                ReturnResult.Add(Result);
            }

            if ( INIStructures == null )
            {
                clsResult Result = new clsResult("struct.bjo");
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
                ReturnResult.Add(Result);
            }

            clsINIDroids INIDroids = null;

            SubResult = IOUtil.TryOpenFileStream(GameFilesPath + "droid.ini", ref File);
            if ( !SubResult.Success )
            {
            }
            else
            {
                clsResult Result = new clsResult("droid.ini");
                clsINIRead DroidsINI = new clsINIRead();
                StreamReader DroidsINI_Reader = new StreamReader(File);
                Result.Take(DroidsINI.ReadFile(DroidsINI_Reader));
                DroidsINI_Reader.Close();
                INIDroids = new clsINIDroids(DroidsINI.Sections.Count, this);
                Result.Take(DroidsINI.Translate(INIDroids));
                ReturnResult.Add(Result);
            }

            if ( INIStructures == null )
            {
                clsResult Result = new clsResult("dinit.bjo");
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
                ReturnResult.Add(Result);
            }

            sCreateWZObjectsArgs CreateObjectsArgs = new sCreateWZObjectsArgs();
            CreateObjectsArgs.BJOUnits = BJOUnits;
            CreateObjectsArgs.INIStructures = INIStructures;
            CreateObjectsArgs.INIDroids = INIDroids;
            CreateObjectsArgs.INIFeatures = INIFeatures;
            ReturnResult.Add(CreateWZObjects(CreateObjectsArgs));

            //map objects are modified by this and must already exist
            SubResult = IOUtil.TryOpenFileStream(GameFilesPath + "labels.ini", ref File);
            if ( !SubResult.Success )
            {
            }
            else
            {
                clsResult Result = new clsResult("labels.ini");
                clsINIRead LabelsINI = new clsINIRead();
                StreamReader LabelsINI_Reader = new StreamReader(File);
                Result.Take(LabelsINI.ReadFile(LabelsINI_Reader));
                LabelsINI_Reader.Close();
                Result.Take(Read_WZ_Labels(LabelsINI, false));
                ReturnResult.Add(Result);
            }

            return ReturnResult;
        }

        public struct sCreateWZObjectsArgs
        {
            public SimpleClassList<clsWZBJOUnit> BJOUnits;
            public clsINIStructures INIStructures;
            public clsINIDroids INIDroids;
            public clsINIFeatures INIFeatures;
        }

        public clsResult CreateWZObjects(sCreateWZObjectsArgs Args)
        {
            clsResult ReturnResult = new clsResult("Creating objects");
            clsUnit NewUnit = default(clsUnit);
            UInt32 AvailableID = 0;
            SimpleClassList<clsWZBJOUnit> BJOUnits = Args.BJOUnits;
            clsINIStructures INIStructures = Args.INIStructures;
            clsINIDroids INIDroids = Args.INIDroids;
            clsINIFeatures INIFeatures = Args.INIFeatures;
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
                for ( A = 0; A <= INIStructures.StructureCount - 1; A++ )
                {
                    if ( INIStructures.Structures[A].ID >= AvailableID )
                    {
                        AvailableID = INIStructures.Structures[A].ID + 1U;
                    }
                }
            }
            if ( INIFeatures != null )
            {
                for ( A = 0; A <= INIFeatures.FeatureCount - 1; A++ )
                {
                    if ( INIFeatures.Features[A].ID >= AvailableID )
                    {
                        AvailableID = INIFeatures.Features[A].ID + 1U;
                    }
                }
            }
            if ( INIDroids != null )
            {
                for ( A = 0; A <= INIDroids.DroidCount - 1; A++ )
                {
                    if ( INIDroids.Droids[A].ID >= AvailableID )
                    {
                        AvailableID = INIDroids.Droids[A].ID + 1U;
                    }
                }
            }

            foreach ( clsWZBJOUnit tempLoopVar_BJOUnit in BJOUnits )
            {
                BJOUnit = tempLoopVar_BJOUnit;
                NewUnit = new clsUnit();
                NewUnit.ID = BJOUnit.ID;
                NewUnit.Type = modProgram.ObjectData.FindOrCreateUnitType(BJOUnit.Code, BJOUnit.ObjectType, -1);
                if ( NewUnit.Type == null )
                {
                    ReturnResult.ProblemAdd("Unable to create object type.");
                    return ReturnResult;
                }
                if ( BJOUnit.Player >= modProgram.PlayerCountMax )
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
                    modProgram.ZeroIDWarning(NewUnit, BJOUnit.ID, ReturnResult);
                }
                UnitAdd.NewUnit = NewUnit;
                UnitAdd.ID = BJOUnit.ID;
                UnitAdd.Perform();
                modProgram.ErrorIDChange(BJOUnit.ID, NewUnit, "CreateWZObjects");
                if ( AvailableID == BJOUnit.ID )
                {
                    AvailableID = NewUnit.ID + 1U;
                }
            }

            clsStructureType StructureType = default(clsStructureType);
            clsDroidDesign DroidType = default(clsDroidDesign);
            clsFeatureType FeatureType = default(clsFeatureType);
            clsDroidDesign.sLoadPartsArgs LoadPartsArgs = new clsDroidDesign.sLoadPartsArgs();
            clsUnitType UnitType = null;
            int ErrorCount = 0;
            int UnknownDroidComponentCount = 0;
            int UnknownDroidTypeCount = 0;
            int DroidBadPositionCount = 0;
            int StructureBadPositionCount = 0;
            int StructureBadModulesCount = 0;
            int FeatureBadPositionCount = 0;
            int ModuleLimit = 0;
            sXY_int ZeroPos = new sXY_int(0, 0);
            clsStructureType ModuleType = default(clsStructureType);
            clsUnit NewModule = default(clsUnit);

            clsStructureType FactoryModule = modProgram.ObjectData.FindFirstStructureType(clsStructureType.enumStructureType.FactoryModule);
            clsStructureType ResearchModule = modProgram.ObjectData.FindFirstStructureType(clsStructureType.enumStructureType.ResearchModule);
            clsStructureType PowerModule = modProgram.ObjectData.FindFirstStructureType(clsStructureType.enumStructureType.PowerModule);

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
                        StructureBadPositionCount++;
                    }
                    else if ( !modProgram.PosIsWithinTileArea(INIStructures.Structures[A].Pos.WorldPos.Horizontal, ZeroPos, Terrain.TileSize) )
                    {
                        StructureBadPositionCount++;
                    }
                    else
                    {
                        UnitType = modProgram.ObjectData.FindOrCreateUnitType(Convert.ToString(INIStructures.Structures[A].Code),
                            clsUnitType.enumType.PlayerStructure, INIStructures.Structures[A].WallType);
                        if ( UnitType.Type == clsUnitType.enumType.PlayerStructure )
                        {
                            StructureType = (clsStructureType)UnitType;
                        }
                        else
                        {
                            StructureType = null;
                        }
                        if ( StructureType == null )
                        {
                            ErrorCount++;
                        }
                        else
                        {
                            NewUnit = new clsUnit();
                            NewUnit.Type = StructureType;
                            if ( INIStructures.Structures[A].UnitGroup == null )
                            {
                                NewUnit.UnitGroup = ScavengerUnitGroup;
                            }
                            else
                            {
                                NewUnit.UnitGroup = INIStructures.Structures[A].UnitGroup;
                            }
                            NewUnit.Pos = INIStructures.Structures[A].Pos.WorldPos;
                            NewUnit.Rotation = Convert.ToInt32(INIStructures.Structures[A].Rotation.Direction * 360.0D / modProgram.INIRotationMax);
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
                                modProgram.ZeroIDWarning(NewUnit, INIStructures.Structures[A].ID, ReturnResult);
                            }
                            UnitAdd.NewUnit = NewUnit;
                            UnitAdd.ID = INIStructures.Structures[A].ID;
                            UnitAdd.Perform();
                            modProgram.ErrorIDChange(INIStructures.Structures[A].ID, NewUnit, "Load_WZ->INIStructures");
                            if ( AvailableID == INIStructures.Structures[A].ID )
                            {
                                AvailableID = NewUnit.ID + 1U;
                            }
                            //create modules
                            switch ( StructureType.StructureType )
                            {
                                case clsStructureType.enumStructureType.Factory:
                                    ModuleLimit = 2;
                                    ModuleType = FactoryModule;
                                    break;
                                case clsStructureType.enumStructureType.VTOLFactory:
                                    ModuleLimit = 2;
                                    ModuleType = FactoryModule;
                                    break;
                                case clsStructureType.enumStructureType.PowerGenerator:
                                    ModuleLimit = 1;
                                    ModuleType = PowerModule;
                                    break;
                                case clsStructureType.enumStructureType.Research:
                                    ModuleLimit = 1;
                                    ModuleType = ResearchModule;
                                    break;
                                default:
                                    ModuleLimit = 0;
                                    ModuleType = null;
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
                            if ( ModuleType != null )
                            {
                                for ( B = 0; B <= INIStructures.Structures[A].ModuleCount - 1; B++ )
                                {
                                    NewModule = new clsUnit();
                                    NewModule.Type = ModuleType;
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
                    else if ( !modProgram.PosIsWithinTileArea(INIFeatures.Features[A].Pos.WorldPos.Horizontal, ZeroPos, Terrain.TileSize) )
                    {
                        FeatureBadPositionCount++;
                    }
                    else
                    {
                        UnitType = modProgram.ObjectData.FindOrCreateUnitType(Convert.ToString(INIFeatures.Features[A].Code), clsUnitType.enumType.Feature, -1);
                        if ( UnitType.Type == clsUnitType.enumType.Feature )
                        {
                            FeatureType = (clsFeatureType)UnitType;
                        }
                        else
                        {
                            FeatureType = null;
                        }
                        if ( FeatureType == null )
                        {
                            ErrorCount++;
                        }
                        else
                        {
                            NewUnit = new clsUnit();
                            NewUnit.Type = FeatureType;
                            NewUnit.UnitGroup = ScavengerUnitGroup;
                            NewUnit.Pos = INIFeatures.Features[A].Pos.WorldPos;
                            NewUnit.Rotation = Convert.ToInt32(INIFeatures.Features[A].Rotation.Direction * 360.0D / modProgram.INIRotationMax);
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
                                modProgram.ZeroIDWarning(NewUnit, INIFeatures.Features[A].ID, ReturnResult);
                            }
                            UnitAdd.NewUnit = NewUnit;
                            UnitAdd.ID = INIFeatures.Features[A].ID;
                            UnitAdd.Perform();
                            modProgram.ErrorIDChange(INIFeatures.Features[A].ID, NewUnit, "Load_WZ->INIFeatures");
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
                    else if ( !modProgram.PosIsWithinTileArea(INIDroids.Droids[A].Pos.WorldPos.Horizontal, ZeroPos, Terrain.TileSize) )
                    {
                        DroidBadPositionCount++;
                    }
                    else
                    {
                        if ( INIDroids.Droids[A].Template == null || INIDroids.Droids[A].Template == "" )
                        {
                            DroidType = new clsDroidDesign();
                            if ( !DroidType.SetDroidType((modProgram.enumDroidType)(INIDroids.Droids[A].DroidType)) )
                            {
                                UnknownDroidTypeCount++;
                            }
                            LoadPartsArgs.Body = modProgram.ObjectData.FindOrCreateBody(INIDroids.Droids[A].Body);
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
                            LoadPartsArgs.Propulsion = modProgram.ObjectData.FindOrCreatePropulsion(Convert.ToString(INIDroids.Droids[A].Propulsion));
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
                            LoadPartsArgs.Construct = modProgram.ObjectData.FindOrCreateConstruct(Convert.ToString(INIDroids.Droids[A].Construct));
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
                            LoadPartsArgs.Repair = modProgram.ObjectData.FindOrCreateRepair(INIDroids.Droids[A].Repair);
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
                            LoadPartsArgs.Sensor = modProgram.ObjectData.FindOrCreateSensor(INIDroids.Droids[A].Sensor);
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
                            LoadPartsArgs.Brain = modProgram.ObjectData.FindOrCreateBrain(INIDroids.Droids[A].Brain);
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
                            LoadPartsArgs.ECM = modProgram.ObjectData.FindOrCreateECM(Convert.ToString(INIDroids.Droids[A].ECM));
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
                            LoadPartsArgs.Weapon1 = modProgram.ObjectData.FindOrCreateWeapon(Convert.ToString(INIDroids.Droids[A].Weapons[0]));
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
                            LoadPartsArgs.Weapon2 = modProgram.ObjectData.FindOrCreateWeapon(Convert.ToString(INIDroids.Droids[A].Weapons[1]));
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
                            LoadPartsArgs.Weapon3 = modProgram.ObjectData.FindOrCreateWeapon(Convert.ToString(INIDroids.Droids[A].Weapons[2]));
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
                            UnitType = modProgram.ObjectData.FindOrCreateUnitType(INIDroids.Droids[A].Template, clsUnitType.enumType.PlayerDroid, -1);
                            if ( UnitType == null )
                            {
                                DroidType = null;
                            }
                            else
                            {
                                if ( UnitType.Type == clsUnitType.enumType.PlayerDroid )
                                {
                                    DroidType = (clsDroidDesign)UnitType;
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
                            NewUnit.Type = DroidType;
                            if ( INIDroids.Droids[A].UnitGroup == null )
                            {
                                NewUnit.UnitGroup = ScavengerUnitGroup;
                            }
                            else
                            {
                                NewUnit.UnitGroup = INIDroids.Droids[A].UnitGroup;
                            }
                            NewUnit.Pos = INIDroids.Droids[A].Pos.WorldPos;
                            NewUnit.Rotation = Convert.ToInt32(INIDroids.Droids[A].Rotation.Direction * 360.0D / modProgram.INIRotationMax);
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
                                modProgram.ZeroIDWarning(NewUnit, INIDroids.Droids[A].ID, ReturnResult);
                            }
                            UnitAdd.NewUnit = NewUnit;
                            UnitAdd.ID = INIDroids.Droids[A].ID;
                            UnitAdd.Perform();
                            modProgram.ErrorIDChange(INIDroids.Droids[A].ID, NewUnit, "Load_WZ->INIDroids");
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

        public class clsINIStructures : clsINIRead.clsSectionTranslator
        {
            private clsMap ParentMap;

            public struct sStructure
            {
                public UInt32 ID;
                public string Code;
                public clsUnitGroup UnitGroup;
                public modProgram.clsWorldPos Pos;
                public modProgram.sWZAngle Rotation;
                public int ModuleCount;
                public int HealthPercent;
                public int WallType;
            }

            public sStructure[] Structures;
            public int StructureCount;

            public clsINIStructures(int NewStructureCount, clsMap NewParentMap)
            {
                int A = 0;

                ParentMap = NewParentMap;

                StructureCount = NewStructureCount;
                Structures = new sStructure[StructureCount];
                for ( A = 0; A <= StructureCount - 1; A++ )
                {
                    Structures[A].HealthPercent = -1;
                    Structures[A].WallType = -1;
                }
            }

            public override clsINIRead.enumTranslatorResult Translate(int INISectionNum, clsINIRead.clsSection.sProperty INIProperty)
            {
                if ( (string)INIProperty.Name == "id" )
                {
                    UInt32 uintTemp = 0;
                    if ( IOUtil.InvariantParse_uint(Convert.ToString(INIProperty.Value), uintTemp) )
                    {
                        if ( uintTemp > 0 )
                        {
                            Structures[INISectionNum].ID = uintTemp;
                        }
                    }
                    else
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                }
                else if ( (string)INIProperty.Name == "name" )
                {
                    Structures[INISectionNum].Code = Convert.ToString(INIProperty.Value);
                }
                else if ( (string)INIProperty.Name == "startpos" )
                {
                    int StartPos = 0;
                    if ( !IOUtil.InvariantParse_int(Convert.ToString(INIProperty.Value), ref StartPos) )
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                    if ( StartPos < 0 | StartPos >= modProgram.PlayerCountMax )
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                    Structures[INISectionNum].UnitGroup = ParentMap.UnitGroups[StartPos];
                }
                else if ( (string)INIProperty.Name == "player" )
                {
                    if ( INIProperty.Value.ToLower() != "scavenger" )
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                    Structures[INISectionNum].UnitGroup = ParentMap.ScavengerUnitGroup;
                }
                else if ( (string)INIProperty.Name == "position" )
                {
                    modProgram.clsWorldPos temp_Result = Structures[INISectionNum].Pos;
                    if ( !IOUtil.WorldPosFromINIText(Convert.ToString(INIProperty.Value), ref temp_Result) )
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                }
                else if ( (string)INIProperty.Name == "rotation" )
                {
                    modProgram.sWZAngle temp_Result2 = Structures[INISectionNum].Rotation;
                    if ( !IOUtil.WZAngleFromINIText(Convert.ToString(INIProperty.Value), ref temp_Result2) )
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                }
                else if ( (string)INIProperty.Name == "modules" )
                {
                    int ModuleCount = 0;
                    if ( !IOUtil.InvariantParse_int(Convert.ToString(INIProperty.Value), ref ModuleCount) )
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                    if ( ModuleCount < 0 )
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                    Structures[INISectionNum].ModuleCount = ModuleCount;
                }
                else if ( (string)INIProperty.Name == "health" )
                {
                    int temp_Result3 = Structures[INISectionNum].HealthPercent;
                    if ( !IOUtil.HealthFromINIText(Convert.ToString(INIProperty.Value), ref temp_Result3) )
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                }
                else if ( (string)INIProperty.Name == "wall/type" )
                {
                    int WallType = 0;
                    if ( !IOUtil.InvariantParse_int(Convert.ToString(INIProperty.Value), ref WallType) )
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                    if ( WallType < 0 )
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                    Structures[INISectionNum].WallType = WallType;
                }
                else
                {
                    return clsINIRead.enumTranslatorResult.NameUnknown;
                }
                return clsINIRead.enumTranslatorResult.Translated;
            }
        }

        public class clsINIDroids : clsINIRead.clsSectionTranslator
        {
            private clsMap ParentMap;

            public struct sDroid
            {
                public UInt32 ID;
                public string Template;
                public clsUnitGroup UnitGroup;
                public modProgram.clsWorldPos Pos;
                public modProgram.sWZAngle Rotation;
                public int HealthPercent;
                public int DroidType;
                public string Body;
                public string Propulsion;
                public string Brain;
                public string Repair;
                public string ECM;
                public string Sensor;
                public string Construct;
                public string[] Weapons;
                public int WeaponCount;
            }

            public sDroid[] Droids;
            public int DroidCount;

            public clsINIDroids(int NewDroidCount, clsMap NewParentMap)
            {
                int A = 0;

                ParentMap = NewParentMap;

                DroidCount = NewDroidCount;
                Droids = new sDroid[DroidCount];
                for ( A = 0; A <= DroidCount - 1; A++ )
                {
                    Droids[A].HealthPercent = -1;
                    Droids[A].DroidType = -1;
                    Droids[A].Weapons = new string[3];
                }
            }

            public override clsINIRead.enumTranslatorResult Translate(int INISectionNum, clsINIRead.clsSection.sProperty INIProperty)
            {
                if ( (string)INIProperty.Name == "id" )
                {
                    UInt32 uintTemp = 0;
                    if ( IOUtil.InvariantParse_uint(Convert.ToString(INIProperty.Value), uintTemp) )
                    {
                        if ( uintTemp > 0 )
                        {
                            Droids[INISectionNum].ID = uintTemp;
                        }
                    }
                    else
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                }
                else if ( (string)INIProperty.Name == "template" )
                {
                    Droids[INISectionNum].Template = Convert.ToString(INIProperty.Value);
                }
                else if ( (string)INIProperty.Name == "startpos" )
                {
                    int StartPos = 0;
                    if ( !IOUtil.InvariantParse_int(Convert.ToString(INIProperty.Value), ref StartPos) )
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                    if ( StartPos < 0 | StartPos >= modProgram.PlayerCountMax )
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                    Droids[INISectionNum].UnitGroup = ParentMap.UnitGroups[StartPos];
                }
                else if ( (string)INIProperty.Name == "player" )
                {
                    if ( INIProperty.Value.ToLower() != "scavenger" )
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                    Droids[INISectionNum].UnitGroup = ParentMap.ScavengerUnitGroup;
                }
                else if ( (string)INIProperty.Name == "name" )
                {
                    //ignore
                }
                else if ( (string)INIProperty.Name == "position" )
                {
                    modProgram.clsWorldPos temp_Result = Droids[INISectionNum].Pos;
                    if ( !IOUtil.WorldPosFromINIText(Convert.ToString(INIProperty.Value), ref temp_Result) )
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                }
                else if ( (string)INIProperty.Name == "rotation" )
                {
                    modProgram.sWZAngle temp_Result2 = Droids[INISectionNum].Rotation;
                    if ( !IOUtil.WZAngleFromINIText(Convert.ToString(INIProperty.Value), ref temp_Result2) )
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                }
                else if ( (string)INIProperty.Name == "health" )
                {
                    int temp_Result3 = Droids[INISectionNum].HealthPercent;
                    if ( !IOUtil.HealthFromINIText(Convert.ToString(INIProperty.Value), ref temp_Result3) )
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                }
                else if ( (string)INIProperty.Name == "droidtype" )
                {
                    int temp_Result4 = Droids[INISectionNum].DroidType;
                    if ( !IOUtil.InvariantParse_int(Convert.ToString(INIProperty.Value), ref temp_Result4) )
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                }
                else if ( (string)INIProperty.Name == "weapons" )
                {
                    Int32 temp_Result5 = Droids[INISectionNum].WeaponCount;
                    if ( !IOUtil.InvariantParse_int(Convert.ToString(INIProperty.Value), ref temp_Result5) )
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                }
                else if ( (string)INIProperty.Name == "parts\\body" )
                {
                    Droids[INISectionNum].Body = Convert.ToString(INIProperty.Value);
                }
                else if ( (string)INIProperty.Name == "parts\\propulsion" )
                {
                    Droids[INISectionNum].Propulsion = Convert.ToString(INIProperty.Value);
                }
                else if ( (string)INIProperty.Name == "parts\\brain" )
                {
                    Droids[INISectionNum].Brain = Convert.ToString(INIProperty.Value);
                }
                else if ( (string)INIProperty.Name == "parts\\repair" )
                {
                    Droids[INISectionNum].Repair = Convert.ToString(INIProperty.Value);
                }
                else if ( (string)INIProperty.Name == "parts\\ecm" )
                {
                    Droids[INISectionNum].ECM = Convert.ToString(INIProperty.Value);
                }
                else if ( (string)INIProperty.Name == "parts\\sensor" )
                {
                    Droids[INISectionNum].Sensor = Convert.ToString(INIProperty.Value);
                }
                else if ( (string)INIProperty.Name == "parts\\construct" )
                {
                    Droids[INISectionNum].Construct = Convert.ToString(INIProperty.Value);
                }
                else if ( (string)INIProperty.Name == "parts\\weapon\\1" )
                {
                    Droids[INISectionNum].Weapons[0] = INIProperty.Value;
                }
                else if ( (string)INIProperty.Name == "parts\\weapon\\2" )
                {
                    Droids[INISectionNum].Weapons[1] = INIProperty.Value;
                }
                else if ( (string)INIProperty.Name == "parts\\weapon\\3" )
                {
                    Droids[INISectionNum].Weapons[2] = INIProperty.Value;
                }
                else
                {
                    return clsINIRead.enumTranslatorResult.NameUnknown;
                }
                return clsINIRead.enumTranslatorResult.Translated;
            }
        }

        public class clsINIFeatures : clsINIRead.clsSectionTranslator
        {
            public struct sFeatures
            {
                public UInt32 ID;
                public string Code;
                public modProgram.clsWorldPos Pos;
                public modProgram.sWZAngle Rotation;
                public int HealthPercent;
            }

            public sFeatures[] Features;
            public int FeatureCount;

            public clsINIFeatures(int NewFeatureCount)
            {
                int A = 0;

                FeatureCount = NewFeatureCount;
                Features = new sFeatures[FeatureCount];
                for ( A = 0; A <= FeatureCount - 1; A++ )
                {
                    Features[A].HealthPercent = -1;
                }
            }

            public override clsINIRead.enumTranslatorResult Translate(int INISectionNum, clsINIRead.clsSection.sProperty INIProperty)
            {
                if ( (string)INIProperty.Name == "id" )
                {
                    UInt32 uintTemp = 0;
                    if ( IOUtil.InvariantParse_uint(Convert.ToString(INIProperty.Value), uintTemp) )
                    {
                        if ( uintTemp > 0 )
                        {
                            Features[INISectionNum].ID = uintTemp;
                        }
                    }
                    else
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                }
                else if ( (string)INIProperty.Name == "name" )
                {
                    Features[INISectionNum].Code = Convert.ToString(INIProperty.Value);
                }
                else if ( (string)INIProperty.Name == "position" )
                {
                    modProgram.clsWorldPos temp_Result = Features[INISectionNum].Pos;
                    if ( !IOUtil.WorldPosFromINIText(Convert.ToString(INIProperty.Value), ref temp_Result) )
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                }
                else if ( (string)INIProperty.Name == "rotation" )
                {
                    modProgram.sWZAngle temp_Result2 = Features[INISectionNum].Rotation;
                    if ( !IOUtil.WZAngleFromINIText(Convert.ToString(INIProperty.Value), ref temp_Result2) )
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                }
                else if ( (string)INIProperty.Name == "health" )
                {
                    Int32 temp_Result3 = Features[INISectionNum].HealthPercent;
                    if ( !IOUtil.HealthFromINIText(Convert.ToString(INIProperty.Value), ref temp_Result3) )
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                }
                else
                {
                    return clsINIRead.enumTranslatorResult.NameUnknown;
                }
                return clsINIRead.enumTranslatorResult.Translated;
            }
        }

        private modProgram.sResult Read_WZ_gam(BinaryReader File)
        {
            modProgram.sResult ReturnResult = new modProgram.sResult();
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
                    if (
                        Interaction.MsgBox("Game file version is unknown. Continue?", (MsgBoxStyle)(MsgBoxStyle.OkCancel | MsgBoxStyle.Question),
                            null) != MsgBoxResult.Ok )
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
                return ReturnResult;
            }

            ReturnResult.Success = true;
            return ReturnResult;
        }

        private modProgram.sResult Read_WZ_map(BinaryReader File)
        {
            modProgram.sResult ReturnResult = new modProgram.sResult();
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
            sXY_int PosA = new sXY_int();
            sXY_int PosB = new sXY_int();

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
                    if (
                        Interaction.MsgBox("game.map version is unknown. Continue?", (MsgBoxStyle)(MsgBoxStyle.OkCancel | MsgBoxStyle.Question),
                            null) != MsgBoxResult.Ok )
                    {
                        ReturnResult.Problem = "Aborted.";
                        return ReturnResult;
                    }
                }
                MapWidth = File.ReadUInt32();
                MapHeight = File.ReadUInt32();
                if ( MapWidth < 1U || MapWidth > modProgram.MapMaxSize || MapHeight < 1U || MapHeight > modProgram.MapMaxSize )
                {
                    ReturnResult.Problem = "Map size out of range.";
                    return ReturnResult;
                }

                TerrainBlank(new sXY_int(Convert.ToInt32(MapWidth), Convert.ToInt32(MapHeight)));

                for ( Y = 0; Y <= Terrain.TileSize.Y - 1; Y++ )
                {
                    for ( X = 0; X <= Terrain.TileSize.X - 1; X++ )
                    {
                        TextureNum = File.ReadByte();
                        Terrain.Tiles[X, Y].Texture.TextureNum = TextureNum;
                        Flip = File.ReadByte();
                        Terrain.Vertices[X, Y].Height = File.ReadByte();
                        //get flipx
                        A = (int)(Conversion.Int(Flip / 128.0D));
                        Flip -= (byte)(A * 128);
                        FlipX = A == 1;
                        //get flipy
                        A = (int)(Conversion.Int(Flip / 64.0D));
                        Flip -= (byte)(A * 64);
                        FlipZ = A == 1;
                        //get rotation
                        A = (int)(Conversion.Int(Flip / 16.0D));
                        Flip -= (byte)(A * 16);
                        Rotate = (byte)A;
                        TileOrientation.OldOrientation_To_TileOrientation(Rotate, FlipX, FlipZ, Terrain.Tiles[X, Y].Texture.Orientation);
                        //get tri direction
                        A = (int)(Conversion.Int(Flip / 8.0D));
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
                return ReturnResult;
            }

            ReturnResult.Success = true;
            return ReturnResult;
        }

        private modProgram.sResult Read_WZ_Features(BinaryReader File, SimpleClassList<clsWZBJOUnit> WZUnits)
        {
            modProgram.sResult ReturnResult = new modProgram.sResult();
            ReturnResult.Success = false;
            ReturnResult.Problem = "";

            string strTemp = null;
            UInt32 Version = 0;
            UInt32 uintTemp = 0;
            int A = 0;
            int B = 0;
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
                    if (
                        Interaction.MsgBox("feat.bjo version is unknown. Continue?", (MsgBoxStyle)(MsgBoxStyle.OkCancel | MsgBoxStyle.Question),
                            null) != MsgBoxResult.Ok )
                    {
                        ReturnResult.Problem = "Aborted.";
                        return ReturnResult;
                    }
                }

                uintTemp = File.ReadUInt32();
                for ( A = 0; A <= (Convert.ToInt32(uintTemp)) - 1; A++ )
                {
                    WZBJOUnit = new clsWZBJOUnit();
                    WZBJOUnit.ObjectType = clsUnitType.enumType.Feature;
                    WZBJOUnit.Code = IOUtil.ReadOldTextOfLength(File, 40);
                    B = Strings.InStr(WZBJOUnit.Code, Convert.ToString('\0'), (CompareMethod)0);
                    if ( B > 0 )
                    {
                        WZBJOUnit.Code = Strings.Left(WZBJOUnit.Code, B - 1);
                    }
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
                return ReturnResult;
            }

            ReturnResult.Success = true;
            return ReturnResult;
        }

        private modProgram.sResult Read_WZ_TileTypes(BinaryReader File)
        {
            modProgram.sResult ReturnResult = new modProgram.sResult();
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
                    if (
                        Interaction.MsgBox("ttypes.ttp version is unknown. Continue?", (MsgBoxStyle)(MsgBoxStyle.OkCancel | MsgBoxStyle.Question),
                            null) != MsgBoxResult.Ok )
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
                return ReturnResult;
            }

            ReturnResult.Success = true;
            return ReturnResult;
        }

        private modProgram.sResult Read_WZ_Structures(BinaryReader File, SimpleClassList<clsWZBJOUnit> WZUnits)
        {
            modProgram.sResult ReturnResult = new modProgram.sResult();
            ReturnResult.Success = false;
            ReturnResult.Problem = "";

            string strTemp = null;
            UInt32 Version = 0;
            UInt32 uintTemp = 0;
            int A = 0;
            int B = 0;
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
                    if (
                        Interaction.MsgBox("struct.bjo version is unknown. Continue?", (MsgBoxStyle)(MsgBoxStyle.OkCancel | MsgBoxStyle.Question),
                            null) != MsgBoxResult.Ok )
                    {
                        ReturnResult.Problem = "Aborted.";
                        return ReturnResult;
                    }
                }

                uintTemp = File.ReadUInt32();
                for ( A = 0; A <= (Convert.ToInt32(uintTemp)) - 1; A++ )
                {
                    WZBJOUnit = new clsWZBJOUnit();
                    WZBJOUnit.ObjectType = clsUnitType.enumType.PlayerStructure;
                    WZBJOUnit.Code = IOUtil.ReadOldTextOfLength(File, 40);
                    B = Strings.InStr(WZBJOUnit.Code, Convert.ToString('\0'), (CompareMethod)0);
                    if ( B > 0 )
                    {
                        WZBJOUnit.Code = Strings.Left(WZBJOUnit.Code, B - 1);
                    }
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
                return ReturnResult;
            }

            ReturnResult.Success = true;
            return ReturnResult;
        }

        private modProgram.sResult Read_WZ_Droids(BinaryReader File, SimpleClassList<clsWZBJOUnit> WZUnits)
        {
            modProgram.sResult ReturnResult = new modProgram.sResult();
            ReturnResult.Success = false;
            ReturnResult.Problem = "";

            string strTemp = null;
            UInt32 Version = 0;
            UInt32 uintTemp = 0;
            int A = 0;
            int B = 0;
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
                    if (
                        Interaction.MsgBox("dinit.bjo version is unknown. Continue?", (MsgBoxStyle)(MsgBoxStyle.OkCancel | MsgBoxStyle.Question),
                            null) != MsgBoxResult.Ok )
                    {
                        ReturnResult.Problem = "Aborted.";
                        return ReturnResult;
                    }
                }

                uintTemp = File.ReadUInt32();
                for ( A = 0; A <= (Convert.ToInt32(uintTemp)) - 1; A++ )
                {
                    WZBJOUnit = new clsWZBJOUnit();
                    WZBJOUnit.ObjectType = clsUnitType.enumType.PlayerDroid;
                    WZBJOUnit.Code = IOUtil.ReadOldTextOfLength(File, 40);
                    B = Strings.InStr(WZBJOUnit.Code, Convert.ToString('\0'), (CompareMethod)0);
                    if ( B > 0 )
                    {
                        WZBJOUnit.Code = Strings.Left(WZBJOUnit.Code, B - 1);
                    }
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
                return ReturnResult;
            }

            ReturnResult.Success = true;
            return ReturnResult;
        }

        public clsResult Read_WZ_Labels(clsINIRead INI, bool IsFMap)
        {
            clsResult ReturnResult = new clsResult("Reading labels");

            int CharNum = 0;
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

            clsINIRead.clsSection INISection = default(clsINIRead.clsSection);
            foreach ( clsINIRead.clsSection tempLoopVar_INISection in INI.Sections )
            {
                INISection = tempLoopVar_INISection;
                NameText = INISection.Name;
                CharNum = NameText.IndexOf('_');
                NameText = Strings.Left(NameText, CharNum);
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
                        break;
                }
                strLabel = Convert.ToString(INISection.GetLastPropertyValue("label"));
                if ( strLabel == null )
                {
                    FailedCount++;
                    continue;
                }
                strLabel = strLabel.Replace((ControlChars.Quote).ToString(), "");
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
                            NewPosition = clsScriptPosition.Create(this);
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
                        if ( IOUtil.InvariantParse_uint(IDText, IDNum) )
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

        public clsResult Serialize_WZ_StructuresINI(clsINIWrite File, int PlayerCount)
        {
            clsResult ReturnResult = new clsResult("Serializing structures INI");

            clsStructureType StructureType = default(clsStructureType);
            clsUnit Unit = default(clsUnit);
            bool[] UnitIsModule = new bool[Units.Count];
            int[] UnitModuleCount = new int[Units.Count];
            sXY_int SectorNum = new sXY_int();
            clsStructureType OtherStructureType = default(clsStructureType);
            clsUnit OtherUnit = default(clsUnit);
            sXY_int ModuleMin = new sXY_int();
            sXY_int ModuleMax = new sXY_int();
            sXY_int Footprint = new sXY_int();
            int A = 0;
            clsStructureType.enumStructureType[] UnderneathTypes = new clsStructureType.enumStructureType[2];
            int UnderneathTypeCount = 0;
            int BadModuleCount = 0;
            clsObjectPriorityOrderList PriorityOrder = new clsObjectPriorityOrderList();

            foreach ( clsUnit tempLoopVar_Unit in Units )
            {
                Unit = tempLoopVar_Unit;
                if ( Unit.Type.Type == clsUnitType.enumType.PlayerStructure )
                {
                    StructureType = (clsStructureType)Unit.Type;
                    switch ( StructureType.StructureType )
                    {
                        case clsStructureType.enumStructureType.FactoryModule:
                            UnderneathTypes[0] = clsStructureType.enumStructureType.Factory;
                            UnderneathTypes[1] = clsStructureType.enumStructureType.VTOLFactory;
                            UnderneathTypeCount = 2;
                            break;
                        case clsStructureType.enumStructureType.PowerModule:
                            UnderneathTypes[0] = clsStructureType.enumStructureType.PowerGenerator;
                            UnderneathTypeCount = 1;
                            break;
                        case clsStructureType.enumStructureType.ResearchModule:
                            UnderneathTypes[0] = clsStructureType.enumStructureType.Research;
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
                            if ( OtherUnit.Type.Type == clsUnitType.enumType.PlayerStructure )
                            {
                                OtherStructureType = (clsStructureType)OtherUnit.Type;
                                if ( OtherUnit.UnitGroup == Unit.UnitGroup )
                                {
                                    for ( A = 0; A <= UnderneathTypeCount - 1; A++ )
                                    {
                                        if ( OtherStructureType.StructureType == UnderneathTypes[A] )
                                        {
                                            break;
                                        }
                                    }
                                    if ( A < UnderneathTypeCount )
                                    {
                                        Footprint = OtherStructureType.get_GetFootprintSelected(OtherUnit.Rotation);
                                        ModuleMin.X = OtherUnit.Pos.Horizontal.X - (int)(Footprint.X * modProgram.TerrainGridSpacing / 2.0D);
                                        ModuleMin.Y = OtherUnit.Pos.Horizontal.Y - (int)(Footprint.Y * modProgram.TerrainGridSpacing / 2.0D);
                                        ModuleMax.X = OtherUnit.Pos.Horizontal.X + (int)(Footprint.X * modProgram.TerrainGridSpacing / 2.0D);
                                        ModuleMax.Y = OtherUnit.Pos.Horizontal.Y + (int)(Footprint.Y * modProgram.TerrainGridSpacing / 2.0D);
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
                StructureType = (clsStructureType)Unit.Type;
                if ( Unit.ID <= 0 )
                {
                    ReturnResult.WarningAdd("Error. A structure\'s ID was zero. It was NOT saved. Delete and replace it to allow save.");
                }
                else
                {
                    File.SectionName_Append("structure_" + IOUtil.InvariantToString_uint(Unit.ID));
                    File.Property_Append("id", IOUtil.InvariantToString_uint(Unit.ID));
                    if ( Unit.UnitGroup == ScavengerUnitGroup || (PlayerCount >= 0 & Unit.UnitGroup.WZ_StartPos >= PlayerCount) )
                    {
                        File.Property_Append("player", "scavenger");
                    }
                    else
                    {
                        File.Property_Append("startpos", IOUtil.InvariantToString_int(Unit.UnitGroup.WZ_StartPos));
                    }
                    File.Property_Append("name", StructureType.Code);
                    if ( StructureType.WallLink.IsConnected )
                    {
                        File.Property_Append("wall/type", IOUtil.InvariantToString_int(StructureType.WallLink.ArrayPosition));
                    }
                    File.Property_Append("position", Unit.GetINIPosition());
                    File.Property_Append("rotation", Unit.GetINIRotation());
                    if ( Unit.Health < 1.0D )
                    {
                        File.Property_Append("health", Unit.GetINIHealthPercent());
                    }
                    switch ( StructureType.StructureType )
                    {
                        case clsStructureType.enumStructureType.Factory:
                            ModuleLimit = 2;
                            break;
                        case clsStructureType.enumStructureType.VTOLFactory:
                            ModuleLimit = 2;
                            break;
                        case clsStructureType.enumStructureType.PowerGenerator:
                            ModuleLimit = 1;
                            break;
                        case clsStructureType.enumStructureType.Research:
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
                            ReturnResult.WarningAdd("Structure " + StructureType.GetDisplayTextCode() + " at " + Unit.GetPosText() + " has too many modules (" +
                                                    Convert.ToString(UnitModuleCount[Unit.MapLink.ArrayPosition]) + ").");
                        }
                        TooManyModulesWarningCount++;
                    }
                    else
                    {
                        ModuleCount = UnitModuleCount[Unit.MapLink.ArrayPosition];
                    }
                    File.Property_Append("modules", IOUtil.InvariantToString_int(ModuleCount));
                    File.Gap_Append();
                }
            }

            if ( TooManyModulesWarningCount > TooManyModulesWarningMaxCount )
            {
                ReturnResult.WarningAdd(TooManyModulesWarningCount + " structures had too many modules.");
            }

            return ReturnResult;
        }

        public clsResult Serialize_WZ_DroidsINI(clsINIWrite File, int PlayerCount)
        {
            clsResult ReturnResult = new clsResult("Serializing droids INI");

            clsDroidDesign Droid = default(clsDroidDesign);
            clsDroidTemplate Template = default(clsDroidTemplate);
            string Text = "";
            clsUnit Unit = default(clsUnit);
            bool AsPartsNotTemplate = default(bool);
            bool ValidDroid = default(bool);
            int InvalidPartCount = 0;
            clsBrain Brain = default(clsBrain);

            foreach ( clsUnit tempLoopVar_Unit in Units )
            {
                Unit = tempLoopVar_Unit;
                if ( Unit.Type.Type == clsUnitType.enumType.PlayerDroid )
                {
                    Droid = (clsDroidDesign)Unit.Type;
                    ValidDroid = true;
                    if ( Unit.ID <= 0 )
                    {
                        ValidDroid = false;
                        ReturnResult.WarningAdd("Error. A droid\'s ID was zero. It was NOT saved. Delete and replace it to allow save.");
                    }
                    if ( Droid.IsTemplate )
                    {
                        Template = (clsDroidTemplate)Droid;
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
                            else if ( Droid.Turret2.TurretType != clsTurret.enumTurretType.Weapon )
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
                            else if ( Droid.Turret3.TurretType != clsTurret.enumTurretType.Weapon )
                            {
                                ValidDroid = false;
                                InvalidPartCount++;
                            }
                        }
                    }
                    if ( ValidDroid )
                    {
                        File.SectionName_Append("droid_" + IOUtil.InvariantToString_uint(Unit.ID));
                        File.Property_Append("id", IOUtil.InvariantToString_uint(Unit.ID));
                        if ( Unit.UnitGroup == ScavengerUnitGroup || (PlayerCount >= 0 & Unit.UnitGroup.WZ_StartPos >= PlayerCount) )
                        {
                            File.Property_Append("player", "scavenger");
                        }
                        else
                        {
                            File.Property_Append("startpos", IOUtil.InvariantToString_int(Unit.UnitGroup.WZ_StartPos));
                        }
                        if ( AsPartsNotTemplate )
                        {
                            File.Property_Append("name", Droid.GenerateName());
                        }
                        else
                        {
                            Template = (clsDroidTemplate)Droid;
                            File.Property_Append("template", Template.Code);
                        }
                        File.Property_Append("position", Unit.GetINIPosition());
                        File.Property_Append("rotation", Unit.GetINIRotation());
                        if ( Unit.Health < 1.0D )
                        {
                            File.Property_Append("health", Unit.GetINIHealthPercent());
                        }
                        if ( AsPartsNotTemplate )
                        {
                            File.Property_Append("droidType", IOUtil.InvariantToString_int(Convert.ToInt32(Droid.GetDroidType())));
                            if ( Droid.TurretCount == 0 )
                            {
                                Text = "0";
                            }
                            else
                            {
                                if ( Droid.Turret1.TurretType == clsTurret.enumTurretType.Brain )
                                {
                                    if ( ((clsBrain)Droid.Turret1).Weapon == null )
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
                                    if ( Droid.Turret1.TurretType == clsTurret.enumTurretType.Weapon )
                                    {
                                        Text = IOUtil.InvariantToString_byte(Droid.TurretCount);
                                    }
                                    else
                                    {
                                        Text = "0";
                                    }
                                }
                            }
                            File.Property_Append("weapons", Text);
                            File.Property_Append("parts\\body", Droid.Body.Code);
                            File.Property_Append("parts\\propulsion", Droid.Propulsion.Code);
                            File.Property_Append("parts\\sensor", Droid.GetSensorCode());
                            File.Property_Append("parts\\construct", Droid.GetConstructCode());
                            File.Property_Append("parts\\repair", Droid.GetRepairCode());
                            File.Property_Append("parts\\brain", Droid.GetBrainCode());
                            File.Property_Append("parts\\ecm", Droid.GetECMCode());
                            if ( Droid.TurretCount >= 1 )
                            {
                                if ( Droid.Turret1.TurretType == clsTurret.enumTurretType.Weapon )
                                {
                                    File.Property_Append("parts\\weapon\\1", Droid.Turret1.Code);
                                    if ( Droid.TurretCount >= 2 )
                                    {
                                        if ( Droid.Turret2.TurretType == clsTurret.enumTurretType.Weapon )
                                        {
                                            File.Property_Append("parts\\weapon\\2", Droid.Turret2.Code);
                                            if ( Droid.TurretCount >= 3 )
                                            {
                                                if ( Droid.Turret3.TurretType == clsTurret.enumTurretType.Weapon )
                                                {
                                                    File.Property_Append("parts\\weapon\\3", Droid.Turret3.Code);
                                                }
                                            }
                                        }
                                    }
                                }
                                else if ( Droid.Turret1.TurretType == clsTurret.enumTurretType.Brain )
                                {
                                    Brain = (clsBrain)Droid.Turret1;
                                    if ( Brain.Weapon == null )
                                    {
                                        Text = "ZNULLWEAPON";
                                    }
                                    else
                                    {
                                        Text = Brain.Weapon.Code;
                                    }
                                    File.Property_Append("parts\\weapon\\1", Text);
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

        public clsResult Serialize_WZ_FeaturesINI(clsINIWrite File)
        {
            clsResult ReturnResult = new clsResult("Serializing features INI");
            clsFeatureType FeatureType = default(clsFeatureType);
            clsUnit Unit = default(clsUnit);
            bool Valid = default(bool);

            foreach ( clsUnit tempLoopVar_Unit in Units )
            {
                Unit = tempLoopVar_Unit;
                if ( Unit.Type.Type == clsUnitType.enumType.Feature )
                {
                    FeatureType = (clsFeatureType)Unit.Type;
                    Valid = true;
                    if ( Unit.ID <= 0 )
                    {
                        Valid = false;
                        ReturnResult.WarningAdd("Error. A features\'s ID was zero. It was NOT saved. Delete and replace it to allow save.");
                    }
                    if ( Valid )
                    {
                        File.SectionName_Append("feature_" + IOUtil.InvariantToString_uint(Unit.ID));
                        File.Property_Append("id", IOUtil.InvariantToString_uint(Unit.ID));
                        File.Property_Append("position", Unit.GetINIPosition());
                        File.Property_Append("rotation", Unit.GetINIRotation());
                        File.Property_Append("name", FeatureType.Code);
                        if ( Unit.Health < 1.0D )
                        {
                            File.Property_Append("health", Unit.GetINIHealthPercent());
                        }
                        File.Gap_Append();
                    }
                }
            }

            return ReturnResult;
        }

        public clsResult Serialize_WZ_LabelsINI(clsINIWrite File, int PlayerCount)
        {
            clsResult ReturnResult = new clsResult("Serializing labels INI");

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
            }

            return ReturnResult;
        }

        public struct sWrite_WZ_Args
        {
            public string Path;
            public bool Overwrite;
            public string MapName;

            public class clsMultiplayer
            {
                public int PlayerCount;
                public string AuthorName;
                public string License;
                public bool IsBetaPlayerFormat;
            }

            public clsMultiplayer Multiplayer;

            public class clsCampaign
            {
                //Public GAMTime As UInteger
                public UInt32 GAMType;
            }

            public clsCampaign Campaign;

            public enum enumCompileType
            {
                Unspecified,
                Multiplayer,
                Campaign
            }

            public sXY_int ScrollMin;
            public sXY_uint ScrollMax;
            public enumCompileType CompileType;
        }

        public clsResult Write_WZ(sWrite_WZ_Args Args)
        {
            clsResult ReturnResult =
                new clsResult("Compiling to " + Convert.ToString(ControlChars.Quote) + Args.Path + Convert.ToString(ControlChars.Quote));

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

                char Quote = ControlChars.Quote;
                char EndChar = '\n';
                string Text = "";

                MemoryStream File_LEV_Memory = new MemoryStream();
                StreamWriter File_LEV = new StreamWriter(File_LEV_Memory, modProgram.UTF8Encoding);
                MemoryStream File_MAP_Memory = new MemoryStream();
                BinaryWriter File_MAP = new BinaryWriter(File_MAP_Memory, modProgram.ASCIIEncoding);
                MemoryStream File_GAM_Memory = new MemoryStream();
                BinaryWriter File_GAM = new BinaryWriter(File_GAM_Memory, modProgram.ASCIIEncoding);
                MemoryStream File_featBJO_Memory = new MemoryStream();
                BinaryWriter File_featBJO = new BinaryWriter(File_featBJO_Memory, modProgram.ASCIIEncoding);
                MemoryStream INI_feature_Memory = new MemoryStream();
                clsINIWrite INI_feature = clsINIWrite.CreateFile(INI_feature_Memory);
                MemoryStream File_TTP_Memory = new MemoryStream();
                BinaryWriter File_TTP = new BinaryWriter(File_TTP_Memory, modProgram.ASCIIEncoding);
                MemoryStream File_structBJO_Memory = new MemoryStream();
                BinaryWriter File_structBJO = new BinaryWriter(File_structBJO_Memory, modProgram.ASCIIEncoding);
                MemoryStream INI_struct_Memory = new MemoryStream();
                clsINIWrite INI_struct = clsINIWrite.CreateFile(INI_struct_Memory);
                MemoryStream File_droidBJO_Memory = new MemoryStream();
                BinaryWriter File_droidBJO = new BinaryWriter(File_droidBJO_Memory, modProgram.ASCIIEncoding);
                MemoryStream INI_droid_Memory = new MemoryStream();
                clsINIWrite INI_droid = clsINIWrite.CreateFile(INI_droid_Memory);
                MemoryStream INI_Labels_Memory = new MemoryStream();
                clsINIWrite INI_Labels = clsINIWrite.CreateFile(INI_Labels_Memory);

                string PlayersPrefix = "";
                string PlayersText = "";

                if ( Args.CompileType == sWrite_WZ_Args.enumCompileType.Multiplayer )
                {
                    PlayersText = IOUtil.InvariantToString_int(Args.Multiplayer.PlayerCount);
                    PlayersPrefix = PlayersText + "c-";
                    string fog = "";
                    string TilesetNum = "";
                    if ( Tileset == null )
                    {
                        ReturnResult.ProblemAdd("Map must have a tileset.");
                        return ReturnResult;
                    }
                    else if ( Tileset == modProgram.Tileset_Arizona )
                    {
                        fog = "fog1.wrf";
                        TilesetNum = "1";
                    }
                    else if ( Tileset == modProgram.Tileset_Urban )
                    {
                        fog = "fog2.wrf";
                        TilesetNum = "2";
                    }
                    else if ( Tileset == modProgram.Tileset_Rockies )
                    {
                        fog = "fog3.wrf";
                        TilesetNum = "3";
                    }
                    else
                    {
                        ReturnResult.ProblemAdd("Unknown tileset selected.");
                        return ReturnResult;
                    }

                    Text = "// Made with " + modProgram.ProgramName + " " + modProgram.ProgramVersionNumber + " " + modProgram.ProgramPlatform +
                           Convert.ToString(EndChar);
                    File_LEV.Write(Text);
                    DateTime DateNow = DateTime.Now;
                    Text = "// Date: " + Convert.ToString(DateNow.Year) + "/" + modProgram.MinDigits(DateNow.Month, 2) + "/" +
                           modProgram.MinDigits(DateNow.Day, 2) + " " + modProgram.MinDigits(DateNow.Hour, 2) + ":" + modProgram.MinDigits(DateNow.Minute, 2) + ":" +
                           modProgram.MinDigits(DateNow.Second, 2) + Convert.ToString(EndChar);
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
                File_MAP.Write(Convert.ToBoolean((uint)Terrain.TileSize.X));
                File_MAP.Write(Convert.ToBoolean((uint)Terrain.TileSize.Y));
                byte Flip = 0;
                byte Rotation = 0;
                bool DoFlipX = default(bool);
                int InvalidTileCount = 0;
                int TextureNum = 0;
                for ( Y = 0; Y <= Terrain.TileSize.Y - 1; Y++ )
                {
                    for ( X = 0; X <= Terrain.TileSize.X - 1; X++ )
                    {
                        TileOrientation.TileOrientation_To_OldOrientation(Terrain.Tiles[X, Y].Texture.Orientation, ref Rotation, ref DoFlipX);
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
                        File_MAP.Write(Convert.ToBoolean(Terrain.Vertices[X, Y].Height));
                    }
                }
                if ( InvalidTileCount > 0 )
                {
                    ReturnResult.WarningAdd(InvalidTileCount + " tile texture numbers were invalid.");
                }
                File_MAP.Write(1U); //gateway version
                File_MAP.Write(Convert.ToBoolean((uint)Gateways.Count));
                clsGateway Gateway = default(clsGateway);
                foreach ( clsGateway tempLoopVar_Gateway in Gateways )
                {
                    Gateway = tempLoopVar_Gateway;
                    File_MAP.Write((byte)(MathUtil.Clamp_int(Gateway.PosA.X, 0, 255)));
                    File_MAP.Write((byte)(MathUtil.Clamp_int(Gateway.PosA.Y, 0, 255)));
                    File_MAP.Write((byte)(MathUtil.Clamp_int(Gateway.PosB.X, 0, 255)));
                    File_MAP.Write((byte)(MathUtil.Clamp_int(Gateway.PosB.Y, 0, 255)));
                }

                clsFeatureType FeatureType = default(clsFeatureType);
                clsStructureType StructureType = default(clsStructureType);
                clsDroidDesign DroidType = default(clsDroidDesign);
                clsDroidTemplate DroidTemplate = default(clsDroidTemplate);
                clsUnit Unit = default(clsUnit);
                clsStructureWriteWZ StructureWrite = new clsStructureWriteWZ();
                StructureWrite.File = File_structBJO;
                StructureWrite.CompileType = Args.CompileType;
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
                    if ( Unit.Type.Type == clsUnitType.enumType.Feature )
                    {
                        FeatureOrder.SetItem(Unit);
                        FeatureOrder.ActionPerform();
                    }
                }
                File_featBJO.Write(Convert.ToBoolean((uint)FeatureOrder.Result.Count));
                for ( A = 0; A <= FeatureOrder.Result.Count - 1; A++ )
                {
                    Unit = FeatureOrder.Result[A];
                    FeatureType = (clsFeatureType)Unit.Type;
                    IOUtil.WriteTextOfLength(File_featBJO, 40, FeatureType.Code);
                    File_featBJO.Write(Unit.ID);
                    File_featBJO.Write(Convert.ToBoolean((uint)Unit.Pos.Horizontal.X));
                    File_featBJO.Write(Convert.ToBoolean((uint)Unit.Pos.Horizontal.Y));
                    File_featBJO.Write(Convert.ToBoolean((uint)Unit.Pos.Altitude));
                    File_featBJO.Write(Convert.ToBoolean((uint)Unit.Rotation));
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
                File_TTP.Write(Convert.ToBoolean((uint)Tileset.TileCount));
                for ( A = 0; A <= Tileset.TileCount - 1; A++ )
                {
                    File_TTP.Write(Convert.ToBoolean(Tile_TypeNum[A]));
                }

                IOUtil.WriteText(File_structBJO, false, "stru");
                File_structBJO.Write(8U);
                clsObjectPriorityOrderList NonModuleStructureOrder = new clsObjectPriorityOrderList();
                //non-module structures
                foreach ( clsUnit tempLoopVar_Unit in Units )
                {
                    Unit = tempLoopVar_Unit;
                    if ( Unit.Type.Type == clsUnitType.enumType.PlayerStructure )
                    {
                        StructureType = (clsStructureType)Unit.Type;
                        if ( !StructureType.IsModule() )
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
                    if ( Unit.Type.Type == clsUnitType.enumType.PlayerStructure )
                    {
                        StructureType = (clsStructureType)Unit.Type;
                        if ( StructureType.IsModule() )
                        {
                            ModuleStructureOrder.SetItem(Unit);
                            ModuleStructureOrder.ActionPerform();
                        }
                    }
                }
                File_structBJO.Write(Convert.ToBoolean((uint)(NonModuleStructureOrder.Result.Count + ModuleStructureOrder.Result.Count)));
                NonModuleStructureOrder.Result.PerformTool(StructureWrite);
                ModuleStructureOrder.Result.PerformTool(StructureWrite);

                byte[] DintZeroBytes = new byte[12];

                IOUtil.WriteText(File_droidBJO, false, "dint");
                File_droidBJO.Write(8U);
                clsObjectPriorityOrderList Droids = new clsObjectPriorityOrderList();
                foreach ( clsUnit tempLoopVar_Unit in Units )
                {
                    Unit = tempLoopVar_Unit;
                    if ( Unit.Type.Type == clsUnitType.enumType.PlayerDroid )
                    {
                        DroidType = (clsDroidDesign)Unit.Type;
                        if ( DroidType.IsTemplate )
                        {
                            Droids.SetItem(Unit);
                            Droids.ActionPerform();
                        }
                    }
                }
                File_droidBJO.Write(Convert.ToBoolean((uint)Droids.Result.Count));
                for ( A = 0; A <= Droids.Result.Count - 1; A++ )
                {
                    Unit = Droids.Result[A];
                    DroidTemplate = (clsDroidTemplate)Unit.Type;
                    IOUtil.WriteTextOfLength(File_droidBJO, 40, DroidTemplate.Code);
                    File_droidBJO.Write(Unit.ID);
                    File_droidBJO.Write(Convert.ToBoolean((uint)Unit.Pos.Horizontal.X));
                    File_droidBJO.Write(Convert.ToBoolean((uint)Unit.Pos.Horizontal.Y));
                    File_droidBJO.Write(Convert.ToBoolean((uint)Unit.Pos.Altitude));
                    File_droidBJO.Write(Convert.ToBoolean((uint)Unit.Rotation));
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
                    else
                    {
                        if ( File.Exists(Args.Path) )
                        {
                            try
                            {
                                File.Delete(Args.Path);
                            }
                            catch ( Exception ex )
                            {
                                ReturnResult.ProblemAdd("Unable to delete existing file: " + ex.Message);
                                return ReturnResult;
                            }
                        }
                    }

                    ZipOutputStream WZStream = default(ZipOutputStream);

                    try
                    {
                        WZStream = new ZipOutputStream(File.Create(Args.Path));
                    }
                    catch ( Exception ex )
                    {
                        ReturnResult.ProblemAdd(ex.Message);
                        return ReturnResult;
                    }

                    WZStream.SetLevel(9);
                    WZStream.UseZip64 = UseZip64.Off; //warzone crashes without this

                    try
                    {
                        string ZipPath = "";
                        ZipEntry ZipEntry = default(ZipEntry);

                        if ( Args.Multiplayer.IsBetaPlayerFormat )
                        {
                            ZipPath = PlayersPrefix + Args.MapName + ".xplayers.lev";
                        }
                        else
                        {
                            ZipPath = PlayersPrefix + Args.MapName + ".addon.lev";
                        }
                        ZipEntry = IOUtil.ZipMakeEntry(WZStream, ZipPath, ReturnResult);
                        if ( ZipEntry != null )
                        {
                            File_LEV_Memory.WriteTo(WZStream);
                            WZStream.Flush();
                            WZStream.CloseEntry();
                        }

                        ZipEntry = new ZipEntry("multiplay/");
                        WZStream.PutNextEntry(ZipEntry);
                        ZipEntry = new ZipEntry("multiplay/maps/");
                        WZStream.PutNextEntry(ZipEntry);
                        ZipEntry = new ZipEntry("multiplay/maps/" + PlayersPrefix + Args.MapName + "/");
                        WZStream.PutNextEntry(ZipEntry);

                        ZipPath = "multiplay/maps/" + PlayersPrefix + Args.MapName + ".gam";
                        ZipEntry = IOUtil.ZipMakeEntry(WZStream, ZipPath, ReturnResult);
                        if ( ZipEntry != null )
                        {
                            ReturnResult.Add(IOUtil.WriteMemoryToZipEntryAndFlush(File_GAM_Memory, WZStream));
                        }
                        else
                        {
                            ReturnResult.ProblemAdd("Unable to make entry " + ZipPath);
                        }

                        ZipPath = "multiplay/maps/" + PlayersPrefix + Args.MapName + "/" + "dinit.bjo";
                        ZipEntry = IOUtil.ZipMakeEntry(WZStream, ZipPath, ReturnResult);
                        if ( ZipEntry != null )
                        {
                            ReturnResult.Add(IOUtil.WriteMemoryToZipEntryAndFlush(File_droidBJO_Memory, WZStream));
                        }
                        else
                        {
                            ReturnResult.ProblemAdd("Unable to make entry " + ZipPath);
                        }

                        ZipPath = "multiplay/maps/" + PlayersPrefix + Args.MapName + "/" + "droid.ini";
                        ZipEntry = IOUtil.ZipMakeEntry(WZStream, ZipPath, ReturnResult);
                        if ( ZipEntry != null )
                        {
                            ReturnResult.Add(IOUtil.WriteMemoryToZipEntryAndFlush(INI_droid_Memory, WZStream));
                        }
                        else
                        {
                            ReturnResult.ProblemAdd("Unable to make entry " + ZipPath);
                        }

                        ZipPath = "multiplay/maps/" + PlayersPrefix + Args.MapName + "/" + "feat.bjo";
                        ZipEntry = IOUtil.ZipMakeEntry(WZStream, ZipPath, ReturnResult);
                        if ( ZipEntry != null )
                        {
                            ReturnResult.Add(IOUtil.WriteMemoryToZipEntryAndFlush(File_featBJO_Memory, WZStream));
                        }
                        else
                        {
                            ReturnResult.ProblemAdd("Unable to make entry " + ZipPath);
                        }

                        ZipPath = "multiplay/maps/" + PlayersPrefix + Args.MapName + "/" + "feature.ini";
                        ZipEntry = IOUtil.ZipMakeEntry(WZStream, ZipPath, ReturnResult);
                        if ( ZipEntry != null )
                        {
                            ReturnResult.Add(IOUtil.WriteMemoryToZipEntryAndFlush(INI_feature_Memory, WZStream));
                        }
                        else
                        {
                            ReturnResult.ProblemAdd("Unable to make entry " + ZipPath);
                        }

                        ZipPath = "multiplay/maps/" + PlayersPrefix + Args.MapName + "/" + "game.map";
                        ZipEntry = IOUtil.ZipMakeEntry(WZStream, ZipPath, ReturnResult);
                        if ( ZipEntry != null )
                        {
                            ReturnResult.Add(IOUtil.WriteMemoryToZipEntryAndFlush(File_MAP_Memory, WZStream));
                        }
                        else
                        {
                            ReturnResult.ProblemAdd("Unable to make entry " + ZipPath);
                        }

                        ZipPath = "multiplay/maps/" + PlayersPrefix + Args.MapName + "/" + "struct.bjo";
                        ZipEntry = IOUtil.ZipMakeEntry(WZStream, ZipPath, ReturnResult);
                        if ( ZipEntry != null )
                        {
                            ReturnResult.Add(IOUtil.WriteMemoryToZipEntryAndFlush(File_structBJO_Memory, WZStream));
                        }
                        else
                        {
                            ReturnResult.ProblemAdd("Unable to make entry " + ZipPath);
                        }

                        ZipPath = "multiplay/maps/" + PlayersPrefix + Args.MapName + "/" + "struct.ini";
                        ZipEntry = IOUtil.ZipMakeEntry(WZStream, ZipPath, ReturnResult);
                        if ( ZipEntry != null )
                        {
                            ReturnResult.Add(IOUtil.WriteMemoryToZipEntryAndFlush(INI_struct_Memory, WZStream));
                        }
                        else
                        {
                            ReturnResult.ProblemAdd("Unable to make entry " + ZipPath);
                        }

                        ZipPath = "multiplay/maps/" + PlayersPrefix + Args.MapName + "/" + "ttypes.ttp";
                        ZipEntry = IOUtil.ZipMakeEntry(WZStream, ZipPath, ReturnResult);
                        if ( ZipEntry != null )
                        {
                            ReturnResult.Add(IOUtil.WriteMemoryToZipEntryAndFlush(File_TTP_Memory, WZStream));
                        }
                        else
                        {
                            ReturnResult.ProblemAdd("Unable to make entry " + ZipPath);
                        }

                        if ( INI_Labels_Memory.Length > 0 )
                        {
                            ZipPath = "multiplay/maps/" + PlayersPrefix + Args.MapName + "/" + "labels.ini";
                            ZipEntry = IOUtil.ZipMakeEntry(WZStream, ZipPath, ReturnResult);
                            if ( ZipEntry != null )
                            {
                                ReturnResult.Add(IOUtil.WriteMemoryToZipEntryAndFlush(INI_Labels_Memory, WZStream));
                            }
                            else
                            {
                                ReturnResult.ProblemAdd("Unable to make entry " + ZipPath);
                            }
                        }

                        WZStream.Finish();
                        WZStream.Close();
                        return ReturnResult;
                    }
                    catch ( Exception ex )
                    {
                        WZStream.Close();
                        ReturnResult.ProblemAdd(ex.Message);
                        return ReturnResult;
                    }
                }
                else if ( Args.CompileType == sWrite_WZ_Args.enumCompileType.Campaign )
                {
                    string CampDirectory = modProgram.EndWithPathSeperator(Args.Path);

                    if ( !Directory.Exists(CampDirectory) )
                    {
                        ReturnResult.ProblemAdd("Directory " + CampDirectory + " does not exist.");
                        return ReturnResult;
                    }

                    string FilePath = "";

                    FilePath = CampDirectory + Args.MapName + ".gam";
                    ReturnResult.Add(IOUtil.WriteMemoryToNewFile(File_GAM_Memory, CampDirectory + Args.MapName + ".gam"));

                    CampDirectory += Args.MapName + Convert.ToString(modProgram.PlatformPathSeparator);
                    try
                    {
                        Directory.CreateDirectory(CampDirectory);
                    }
                    catch ( Exception )
                    {
                        ReturnResult.ProblemAdd("Unable to create directory " + CampDirectory);
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
                return ReturnResult;
            }

            return ReturnResult;
        }

        private modProgram.sResult Read_TTP(BinaryReader File)
        {
            modProgram.sResult ReturnResult = new modProgram.sResult();
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
                return ReturnResult;
            }

            ReturnResult.Success = true;
            return ReturnResult;
        }
    }
}