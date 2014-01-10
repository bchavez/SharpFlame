using System;
using System.Drawing;
using System.IO;
using System.Text;
using Matrix3D;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.ApplicationServices;
using Microsoft.VisualBasic.Devices;
using SharpFlame.AppSettings;
using SharpFlame.Collections;
using SharpFlame.Collections.Specialized;
using SharpFlame.Colors;
using SharpFlame.Domain;
using SharpFlame.FileIO;
using SharpFlame.Graphics.OpenGL;
using SharpFlame.Mapping;
using SharpFlame.Mapping.Objects;
using SharpFlame.Mapping.Tiles;
using SharpFlame.Maths;
using SharpFlame.Painters;

namespace SharpFlame
{
    public sealed class App
    {
        public static sRGB_sng MinimapFeatureColour;

        public static char PlatformPathSeparator;

        public static bool Debug_GL = false;

        public static string MyDocumentsProgramPath;

        public static string SettingsPath;
        public static string AutoSavePath;
        public static string InterfaceImagesPath;

        public static void SetProgramSubDirs()
        {
            MyDocumentsProgramPath = (new ServerComputer()).FileSystem.SpecialDirectories.MyDocuments +
                                     Convert.ToString(PlatformPathSeparator) + ".flaME";
#if !Portable
            SettingsPath = MyDocumentsProgramPath + Convert.ToString(PlatformPathSeparator) + "settings.ini";
            AutoSavePath = MyDocumentsProgramPath + Convert.ToString(PlatformPathSeparator) + "autosave" + Convert.ToString(PlatformPathSeparator);
#else
			SettingsPath = (new Microsoft.VisualBasic.ApplicationServices.ConsoleApplicationBase()).Info.DirectoryPath + System.Convert.ToString(PlatformPathSeparator) + "settings.ini";
			AutoSavePath = (new Microsoft.VisualBasic.ApplicationServices.ConsoleApplicationBase()).Info.DirectoryPath + System.Convert.ToString(PlatformPathSeparator) + "autosave" + System.Convert.ToString(PlatformPathSeparator);
#endif
            InterfaceImagesPath = (new ConsoleApplicationBase()).Info.DirectoryPath +
                                  Convert.ToString(PlatformPathSeparator) + "interface" + Convert.ToString(PlatformPathSeparator);
        }

        public static bool ProgramInitialized = false;
        public static bool ProgramInitializeFinished = false;

        public static Icon ProgramIcon;

        public static SimpleList<string> CommandLinePaths = new SimpleList<string>();

        public static int GLTexture_NoTile;
        public static int GLTexture_OverflowTile;

        public static clsKeysActive IsViewKeyDown = new clsKeysActive();

        public static clsBrush TextureBrush = new clsBrush(0.0D, clsBrush.enumShape.Circle);
        public static clsBrush TerrainBrush = new clsBrush(2.0D, clsBrush.enumShape.Circle);
        public static clsBrush HeightBrush = new clsBrush(2.0D, clsBrush.enumShape.Circle);
        public static clsBrush CliffBrush = new clsBrush(2.0D, clsBrush.enumShape.Circle);

        public static clsBrush SmoothRadius = new clsBrush(1.0D, clsBrush.enumShape.Square);

        public static bool DisplayTileOrientation;

        public static clsObjectData ObjectData;

        public static int SelectedTextureNum = -1;
        public static TileOrientation TextureOrientation = new TileOrientation(false, false, false);

        public static Terrain SelectedTerrain;
        public static Road SelectedRoad;

        public class clsTileType
        {
            public string Name;
            public sRGB_sng DisplayColour;
        }

        public static SimpleList<clsTileType> TileTypes = new SimpleList<clsTileType>();

        public const int TileTypeNum_Water = 7;
        public const int TileTypeNum_Cliff = 8;

        public static DroidDesign.clsTemplateDroidType[] TemplateDroidTypes = new DroidDesign.clsTemplateDroidType[0];
        public static int TemplateDroidTypeCount;

        public static readonly UTF8Encoding UTF8Encoding = new UTF8Encoding(false, false);
        public static readonly ASCIIEncoding ASCIIEncoding = new ASCIIEncoding();

        public const int INIRotationMax = 65536;

        public enum enumTileWalls
        {
            None = 0,
            Left = 1,
            Right = 2,
            Top = 4,
            Bottom = 8
        }

        public enum enumObjectRotateMode
        {
            None,
            Walls,
            All
        }

        public enum enumTextureTerrainAction
        {
            Ignore,
            Reinterpret,
            Remove
        }

        public enum enumFillCliffAction
        {
            Ignore,
            StopBefore,
            StopAfter
        }

        public struct sResult
        {
            public bool Success;
            public string Problem;
        }

        public struct sWZAngle
        {
            public UInt16 Direction;
            public UInt16 Pitch;
            public UInt16 Roll;
        }

        public const int TerrainGridSpacing = 128;

        public static int VisionRadius_2E;
        public static double VisionRadius;

        public static clsMap Copied_Map;

        public static SimpleList<clsTileset> Tilesets = new SimpleList<clsTileset>();

        public static clsTileset Tileset_Arizona;
        public static clsTileset Tileset_Urban;
        public static clsTileset Tileset_Rockies;

        public static Painter Painter_Arizona;
        public static Painter Painter_Urban;
        public static Painter Painter_Rockies;

        public static GLFont UnitLabelFont;
        //Public TextureViewFont As GLFont

        public class clsPlayer
        {
            public sRGB_sng Colour;
            public sRGB_sng MinimapColour;

            public void CalcMinimapColour()
            {
                MinimapColour.Red = Math.Min(Colour.Red * 0.6666667F + 0.333333343F, 1.0F);
                MinimapColour.Green = Math.Min(Colour.Green * 0.6666667F + 0.333333343F, 1.0F);
                MinimapColour.Blue = Math.Min(Colour.Blue * 0.6666667F + 0.333333343F, 1.0F);
            }
        }

        public static clsPlayer[] PlayerColour = new clsPlayer[16];

        public struct sSplitPath
        {
            public string[] Parts;
            public int PartCount;
            public string FilePath;
            public string FileTitle;
            public string FileTitleWithoutExtension;
            public string FileExtension;

            public sSplitPath(string Path)
            {
                int A = 0;

                Parts = Path.Split(PlatformPathSeparator);
                PartCount = Parts.GetUpperBound(0) + 1;
                FilePath = "";
                for ( A = 0; A <= PartCount - 2; A++ )
                {
                    FilePath += Parts[A] + Convert.ToString(PlatformPathSeparator);
                }
                FileTitle = Parts[A];
                A = Strings.InStrRev(FileTitle, ".", -1, (CompareMethod)0);
                if ( A > 0 )
                {
                    FileExtension = Strings.Right(FileTitle, FileTitle.Length - A);
                    FileTitleWithoutExtension = Strings.Left(FileTitle, A - 1);
                }
                else
                {
                    FileExtension = "";
                    FileTitleWithoutExtension = FileTitle;
                }
            }
        }

        public static void VisionRadius_2E_Changed()
        {
            VisionRadius = 256.0D * Math.Pow(2.0D, (VisionRadius_2E / 2.0D));
            if ( Program.frmMainInstance.MapViewControl != null )
            {
                View_Radius_Set(VisionRadius);
                Program.frmMainInstance.View_DrawViewLater();
            }
        }

        public static string MinDigits(int Number, int Digits)
        {
            string ReturnResult = "";
            int A = 0;

            ReturnResult = Number.ToStringInvariant();
            A = Digits - ReturnResult.Length;
            if ( A > 0 )
            {
                ReturnResult = Strings.StrDup(A, '0') + ReturnResult;
            }
            return ReturnResult;
        }

        public static void ViewKeyDown_Clear()
        {
            IsViewKeyDown.Deactivate();

            foreach ( Option<KeyboardControl> control in KeyboardManager.OptionsKeyboardControls.Options )
            {
                ((KeyboardControl)(KeyboardManager.KeyboardProfile.get_Value(control))).KeysChanged(IsViewKeyDown);
            }
        }

        public static DroidDesign.clsTemplateDroidType TemplateDroidType_Droid;
        public static DroidDesign.clsTemplateDroidType TemplateDroidType_Cyborg;
        public static DroidDesign.clsTemplateDroidType TemplateDroidType_CyborgConstruct;
        public static DroidDesign.clsTemplateDroidType TemplateDroidType_CyborgRepair;
        public static DroidDesign.clsTemplateDroidType TemplateDroidType_CyborgSuper;
        public static DroidDesign.clsTemplateDroidType TemplateDroidType_Transporter;
        public static DroidDesign.clsTemplateDroidType TemplateDroidType_Person;
        public static DroidDesign.clsTemplateDroidType TemplateDroidType_Null;

        public static void CreateTemplateDroidTypes()
        {
            TemplateDroidType_Droid = new DroidDesign.clsTemplateDroidType("Droid", "DROID");
            TemplateDroidType_Droid.Num = TemplateDroidType_Add(TemplateDroidType_Droid);

            TemplateDroidType_Cyborg = new DroidDesign.clsTemplateDroidType("Cyborg", "CYBORG");
            TemplateDroidType_Cyborg.Num = TemplateDroidType_Add(TemplateDroidType_Cyborg);

            TemplateDroidType_CyborgConstruct = new DroidDesign.clsTemplateDroidType("Cyborg Construct", "CYBORG_CONSTRUCT");
            TemplateDroidType_CyborgConstruct.Num = TemplateDroidType_Add(TemplateDroidType_CyborgConstruct);

            TemplateDroidType_CyborgRepair = new DroidDesign.clsTemplateDroidType("Cyborg Repair", "CYBORG_REPAIR");
            TemplateDroidType_CyborgRepair.Num = TemplateDroidType_Add(TemplateDroidType_CyborgRepair);

            TemplateDroidType_CyborgSuper = new DroidDesign.clsTemplateDroidType("Cyborg Super", "CYBORG_SUPER");
            TemplateDroidType_CyborgSuper.Num = TemplateDroidType_Add(TemplateDroidType_CyborgSuper);

            TemplateDroidType_Transporter = new DroidDesign.clsTemplateDroidType("Transporter", "TRANSPORTER");
            TemplateDroidType_Transporter.Num = TemplateDroidType_Add(TemplateDroidType_Transporter);

            TemplateDroidType_Person = new DroidDesign.clsTemplateDroidType("Person", "PERSON");
            TemplateDroidType_Person.Num = TemplateDroidType_Add(TemplateDroidType_Person);

            TemplateDroidType_Null = new DroidDesign.clsTemplateDroidType("Null Droid", "ZNULLDROID");
            TemplateDroidType_Null.Num = TemplateDroidType_Add(TemplateDroidType_Null);
        }

        public static DroidDesign.clsTemplateDroidType GetTemplateDroidTypeFromTemplateCode(string Code)
        {
            string LCaseCode = Code.ToLower();
            int A = 0;

            for ( A = 0; A <= TemplateDroidTypeCount - 1; A++ )
            {
                if ( TemplateDroidTypes[A].TemplateCode.ToLower() == LCaseCode )
                {
                    return TemplateDroidTypes[A];
                }
            }
            return null;
        }

        public static int TemplateDroidType_Add(DroidDesign.clsTemplateDroidType NewDroidType)
        {
            int ReturnResult = 0;

            Array.Resize(ref TemplateDroidTypes, TemplateDroidTypeCount + 1);
            TemplateDroidTypes[TemplateDroidTypeCount] = NewDroidType;
            ReturnResult = TemplateDroidTypeCount;
            TemplateDroidTypeCount++;

            return ReturnResult;
        }

        public enum enumDroidType
        {
            Weapon = 0,
            Sensor = 1,
            ECM = 2,
            Construct = 3,
            Person = 4,
            Cyborg = 5,
            Transporter = 6,
            Command = 7,
            Repair = 8,
            Default_ = 9,
            Cyborg_Construct = 10,
            Cyborg_Repair = 11,
            Cyborg_Super = 12
        }

        public static void ShowWarnings(clsResult Result)
        {
            if ( !Result.HasWarnings )
            {
                return;
            }

            frmWarnings WarningsForm = new frmWarnings(Result, Result.Text);
            WarningsForm.Show();
            WarningsForm.Activate();
        }

        public static Turret.enumTurretType GetTurretTypeFromName(string TurretTypeName)
        {
            switch ( TurretTypeName.ToLower() )
            {
                case "weapon":
                    return Turret.enumTurretType.Weapon;
                case "construct":
                    return Turret.enumTurretType.Construct;
                case "repair":
                    return Turret.enumTurretType.Repair;
                case "sensor":
                    return Turret.enumTurretType.Sensor;
                case "brain":
                    return Turret.enumTurretType.Brain;
                case "ecm":
                    return Turret.enumTurretType.ECM;
                default:
                    return Turret.enumTurretType.Unknown;
            }
        }

        public static bool ShowIDErrorMessage = true;

        public static void ErrorIDChange(UInt32 IntendedID, clsUnit IDUnit, string NameOfErrorSource)
        {
            if ( !ShowIDErrorMessage )
            {
                return;
            }

            if ( IDUnit.ID == IntendedID )
            {
                return;
            }

            string MessageText = "";

            MessageText = "An object\'s ID has been changed unexpectedly. The error was in " + Convert.ToString(ControlChars.Quote) + NameOfErrorSource +
                          Convert.ToString(ControlChars.Quote) + "." + ControlChars.CrLf + ControlChars.CrLf + "The object is of type " +
                          IDUnit.TypeBase.GetDisplayTextCode() + " and is at map position " + IDUnit.GetPosText() + ". It\'s ID was " +
                          IntendedID.ToStringInvariant() + ", but is now " + IDUnit.ID.ToStringInvariant() + "." + ControlChars.CrLf +
                          ControlChars.CrLf + "Click Cancel to stop seeing this message. Otherwise, click OK.";

            if ( Interaction.MsgBox(MessageText, MsgBoxStyle.OkCancel, null) == MsgBoxResult.Cancel )
            {
                ShowIDErrorMessage = false;
            }
        }

        public static void ZeroIDWarning(clsUnit IDUnit, UInt32 NewID, clsResult Output)
        {
            string MessageText = "";

            MessageText = "An object\'s ID has been changed from 0 to " + NewID.ToStringInvariant() + ". Zero is not a valid ID. The object is of type " +
                          IDUnit.TypeBase.GetDisplayTextCode() + " and is at map position " + IDUnit.GetPosText() + ".";

            //MsgBox(MessageText, MsgBoxStyle.OkOnly)
            Output.WarningAdd(MessageText);
        }

        public struct sWorldPos
        {
            public sXY_int Horizontal;
            public int Altitude;

            public sWorldPos(sXY_int NewHorizontal, int NewAltitude)
            {
                Horizontal = NewHorizontal;
                Altitude = NewAltitude;
            }
        }

        public class clsWorldPos
        {
            public sWorldPos WorldPos;

            public clsWorldPos(sWorldPos NewWorldPos)
            {
                WorldPos = NewWorldPos;
            }
        }

        public static bool PosIsWithinTileArea(sXY_int WorldHorizontal, sXY_int StartTile, sXY_int FinishTile)
        {
            return WorldHorizontal.X >= StartTile.X * TerrainGridSpacing &
                   WorldHorizontal.Y >= StartTile.Y * TerrainGridSpacing &
                   WorldHorizontal.X < FinishTile.X * TerrainGridSpacing &
                   WorldHorizontal.Y < FinishTile.Y * TerrainGridSpacing;
        }

        public static bool SizeIsPowerOf2(int Size)
        {
            double Power = Math.Log(Size) / Math.Log(2.0D);
            return Power == (int)Power;
        }

        public class clsKeysActive
        {
            public bool[] Keys = new bool[256];

            public void Deactivate()
            {
                for ( int i = 0; i <= 255; i++ )
                {
                    Keys[i] = false;
                }
            }
        }

        public static clsResult LoadTilesets(string TilesetsPath)
        {
            clsResult ReturnResult = new clsResult("Loading tilesets");

            string[] TilesetDirs = null;
            try
            {
                TilesetDirs = Directory.GetDirectories(TilesetsPath);
            }
            catch ( Exception ex )
            {
                ReturnResult.ProblemAdd(ex.Message);
                return ReturnResult;
            }

            if ( TilesetDirs == null )
            {
                return ReturnResult;
            }

            clsResult Result = default(clsResult);
            string Path = "";
            clsTileset Tileset = default(clsTileset);

            foreach ( string tempLoopVar_Path in TilesetDirs )
            {
                Path = tempLoopVar_Path;
                Tileset = new clsTileset();
                Result = Tileset.LoadDirectory(Path);
                ReturnResult.Add(Result);
                if ( !Result.HasProblems )
                {
                    Tilesets.Add(Tileset);
                }
            }

            foreach ( clsTileset tempLoopVar_Tileset in Tilesets )
            {
                Tileset = tempLoopVar_Tileset;
                if ( Tileset.Name == "tertilesc1hw" )
                {
                    Tileset.Name = "Arizona";
                    Tileset_Arizona = Tileset;
                    Tileset.IsOriginal = true;
                    Tileset.BGColour = new sRGB_sng(204.0f / 255.0f, 149.0f / 255.0f, 70.0f / 255.0f);
                }
                else if ( Tileset.Name == "tertilesc2hw" )
                {
                    Tileset.Name = "Urban";
                    Tileset_Urban = Tileset;
                    Tileset.IsOriginal = true;
                    Tileset.BGColour = new sRGB_sng(118.0f / 255.0f, 165.0f / 255.0f, 203.0f / 255.0f);
                }
                else if ( Tileset.Name == "tertilesc3hw" )
                {
                    Tileset.Name = "Rocky Mountains";
                    Tileset_Rockies = Tileset;
                    Tileset.IsOriginal = true;
                    Tileset.BGColour = new sRGB_sng(182.0f / 255.0f, 225.0f / 255.0f, 236.0f / 255.0f);
                }
            }

            if ( Tileset_Arizona == null )
            {
                ReturnResult.WarningAdd("Arizona tileset is missing.");
            }
            if ( Tileset_Urban == null )
            {
                ReturnResult.WarningAdd("Urban tileset is missing.");
            }
            if ( Tileset_Rockies == null )
            {
                ReturnResult.WarningAdd("Rocky Mountains tileset is missing.");
            }

            return ReturnResult;
        }

        public static bool Draw_TileTextures = true;

        public enum enumDrawLighting
        {
            Off,
            Half,
            Normal
        }

        public static enumDrawLighting Draw_Lighting = enumDrawLighting.Half;
        public static bool Draw_TileWireframe;
        public static bool Draw_Units = true;
        public static bool Draw_VertexTerrain;
        public static bool Draw_Gateways;
        public static bool Draw_ScriptMarkers = true;

        public enum enumView_Move_Type
        {
            Free,
            RTS
        }

        public static enumView_Move_Type ViewMoveType = enumView_Move_Type.RTS;
        public static bool RTSOrbit = true;

        public static Matrix3DMath.Matrix3D SunAngleMatrix = new Matrix3DMath.Matrix3D();
        public static clsBrush VisionSectors = new clsBrush(0.0D, clsBrush.enumShape.Circle);

        public static void View_Radius_Set(double Radius)
        {
            VisionSectors.Radius = Radius / (TerrainGridSpacing * Constants.SectorTileSize);
        }

        public struct sLayerList
        {
            public class clsLayer
            {
                public int WithinLayer;
                public bool[] AvoidLayers;
                public Terrain Terrain;
                public BooleanMap Terrainmap;
                public float HeightMin;
                public float HeightMax;
                public float SlopeMin;
                public float SlopeMax;
                //for generator only
                public float Scale;
                public float Density;
            }

            public clsLayer[] Layers;
            public int LayerCount;

            public void Layer_Insert(int PositionNum, clsLayer NewLayer)
            {
                int A = 0;
                int B = 0;

                Array.Resize(ref Layers, LayerCount + 1);
                //shift the ones below down
                for ( A = LayerCount - 1; A >= PositionNum; A-- )
                {
                    Layers[A + 1] = Layers[A];
                }
                //insert the new entry
                Layers[PositionNum] = NewLayer;
                LayerCount++;

                for ( A = 0; A <= LayerCount - 1; A++ )
                {
                    if ( Layers[A].WithinLayer >= PositionNum )
                    {
                        Layers[A].WithinLayer = Layers[A].WithinLayer + 1;
                    }
                    Array.Resize(ref Layers[A].AvoidLayers, LayerCount);
                    for ( B = LayerCount - 2; B >= PositionNum; B-- )
                    {
                        Layers[A].AvoidLayers[B + 1] = Layers[A].AvoidLayers[B];
                    }
                    Layers[A].AvoidLayers[PositionNum] = false;
                }
            }

            public void Layer_Remove(int Layer_Num)
            {
                int A = 0;
                int B = 0;

                LayerCount--;
                for ( A = Layer_Num; A <= LayerCount - 1; A++ )
                {
                    Layers[A] = Layers[A + 1];
                }
                Array.Resize(ref Layers, LayerCount);

                for ( A = 0; A <= LayerCount - 1; A++ )
                {
                    if ( Layers[A].WithinLayer == Layer_Num )
                    {
                        Layers[A].WithinLayer = -1;
                    }
                    else if ( Layers[A].WithinLayer > Layer_Num )
                    {
                        Layers[A].WithinLayer = Layers[A].WithinLayer - 1;
                    }
                    for ( B = Layer_Num; B <= LayerCount - 1; B++ )
                    {
                        Layers[A].AvoidLayers[B] = Layers[A].AvoidLayers[B + 1];
                    }
                    Array.Resize(ref Layers[A].AvoidLayers, LayerCount);
                }
            }

            public void Layer_Move(int Layer_Num, int Layer_Dest_Num)
            {
                clsLayer Layer_Temp = default(clsLayer);
                bool boolTemp = default(bool);
                int A = 0;
                int B = 0;

                if ( Layer_Dest_Num < Layer_Num )
                {
                    //move the variables
                    Layer_Temp = Layers[Layer_Num];
                    for ( A = Layer_Num - 1; A >= Layer_Dest_Num; A-- )
                    {
                        Layers[A + 1] = Layers[A];
                    }
                    Layers[Layer_Dest_Num] = Layer_Temp;
                    //update the layer nums
                    for ( A = 0; A <= LayerCount - 1; A++ )
                    {
                        if ( Layers[A].WithinLayer == Layer_Num )
                        {
                            Layers[A].WithinLayer = Layer_Dest_Num;
                        }
                        else if ( Layers[A].WithinLayer >= Layer_Dest_Num && Layers[A].WithinLayer < Layer_Num )
                        {
                            Layers[A].WithinLayer = Layers[A].WithinLayer + 1;
                        }
                        boolTemp = Convert.ToBoolean(Layers[A].AvoidLayers[Layer_Num]);
                        for ( B = Layer_Num - 1; B >= Layer_Dest_Num; B-- )
                        {
                            Layers[A].AvoidLayers[B + 1] = Layers[A].AvoidLayers[B];
                        }
                        Layers[A].AvoidLayers[Layer_Dest_Num] = boolTemp;
                    }
                }
                else if ( Layer_Dest_Num > Layer_Num )
                {
                    //move the variables
                    Layer_Temp = Layers[Layer_Num];
                    for ( A = Layer_Num; A <= Layer_Dest_Num - 1; A++ )
                    {
                        Layers[A] = Layers[A + 1];
                    }
                    Layers[Layer_Dest_Num] = Layer_Temp;
                    //update the layer nums
                    for ( A = 0; A <= LayerCount - 1; A++ )
                    {
                        if ( Layers[A].WithinLayer == Layer_Num )
                        {
                            Layers[A].WithinLayer = Layer_Dest_Num;
                        }
                        else if ( Layers[A].WithinLayer > Layer_Num && Layers[A].WithinLayer <= Layer_Dest_Num )
                        {
                            Layers[A].WithinLayer = Layers[A].WithinLayer - 1;
                        }
                        boolTemp = Convert.ToBoolean(Layers[A].AvoidLayers[Layer_Num]);
                        for ( B = Layer_Num; B <= Layer_Dest_Num - 1; B++ )
                        {
                            Layers[A].AvoidLayers[B] = Layers[A].AvoidLayers[B + 1];
                        }
                        Layers[A].AvoidLayers[Layer_Dest_Num] = boolTemp;
                    }
                }
            }
        }

        public static sLayerList LayerList;

        public static Position.XY_dbl CalcUnitsCentrePos(SimpleList<clsUnit> Units)
        {
            Position.XY_dbl Result = default(Position.XY_dbl);

            Result.X = 0.0D;
            Result.Y = 0.0D;
            clsUnit Unit = default(clsUnit);
            foreach ( clsUnit tempLoopVar_Unit in Units )
            {
                Unit = tempLoopVar_Unit;
                Result += Unit.Pos.Horizontal.ToDoubles();
            }
            Result /= Units.Count;

            return Result;
        }
    }

    public class clsContainer<ItemType>
    {
        public ItemType Item;

        public clsContainer(ItemType item)
        {
            Item = item;
        }
    }
}