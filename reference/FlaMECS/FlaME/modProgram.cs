namespace FlaME
{
    using FlaME.My;
    using Matrix3D;
    using Microsoft.VisualBasic;
    using Microsoft.VisualBasic.CompilerServices;
    using System;
    using System.Collections;
    using System.Drawing;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;

    [StandardModule]
    public sealed class modProgram
    {
        public static readonly System.Text.ASCIIEncoding ASCIIEncoding = new System.Text.ASCIIEncoding();
        public static string AutoSavePath;
        public static clsBrush CliffBrush = new clsBrush(2.0, clsBrush.enumShape.Circle);
        public static modLists.SimpleList<string> CommandLinePaths = new modLists.SimpleList<string>();
        public static clsMap Copied_Map;
        public static bool Debug_GL = false;
        public const int DefaultHeightMultiplier = 2;
        public static bool DisplayTileOrientation;
        public static bool Draw_Gateways;
        public static enumDrawLighting Draw_Lighting = enumDrawLighting.Half;
        public static bool Draw_ScriptMarkers = true;
        public static bool Draw_TileTextures = true;
        public static bool Draw_TileWireframe;
        public static bool Draw_Units = true;
        public static bool Draw_VertexTerrain;
        public const int GameTypeCount = 3;
        public static int GLTexture_NoTile;
        public static int GLTexture_OverflowTile;
        public static clsBrush HeightBrush = new clsBrush(2.0, clsBrush.enumShape.Circle);
        public const int INIRotationMax = 0x10000;
        public static string InterfaceImagesPath;
        public static clsKeysActive IsViewKeyDown = new clsKeysActive();
        public static sLayerList LayerList;
        public const int MapMaxSize = 0x200;
        public const int MaxDroidWeapons = 3;
        public const int MinimapDelay = 100;
        public static sRGB_sng MinimapFeatureColour;
        public const int MinimapMaxSize = 0x200;
        public static string MyDocumentsProgramPath;
        public static clsObjectData ObjectData;
        public static clsPainter Painter_Arizona;
        public static clsPainter Painter_Rockies;
        public static clsPainter Painter_Urban;
        public static char PlatformPathSeparator;
        public static clsPlayer[] PlayerColour = new clsPlayer[0x10];
        public const int PlayerCountMax = 10;
        public static Icon ProgramIcon;
        public static bool ProgramInitialized = false;
        public static bool ProgramInitializeFinished = false;
        public const string ProgramName = "FlaME";
        public const string ProgramPlatform = "Windows";
        public const string ProgramVersionNumber = "1.29";
        public static bool RTSOrbit = true;
        public const int SectorTileSize = 8;
        public static clsPainter.clsRoad SelectedRoad;
        public static clsPainter.clsTerrain SelectedTerrain;
        public static int SelectedTextureNum = -1;
        public static string SettingsPath;
        public static bool ShowIDErrorMessage = true;
        public static clsBrush SmoothRadius = new clsBrush(1.0, clsBrush.enumShape.Square);
        public static Matrix3DMath.Matrix3D SunAngleMatrix = new Matrix3DMath.Matrix3D();
        public static clsDroidDesign.clsTemplateDroidType TemplateDroidType_Cyborg;
        public static clsDroidDesign.clsTemplateDroidType TemplateDroidType_CyborgConstruct;
        public static clsDroidDesign.clsTemplateDroidType TemplateDroidType_CyborgRepair;
        public static clsDroidDesign.clsTemplateDroidType TemplateDroidType_CyborgSuper;
        public static clsDroidDesign.clsTemplateDroidType TemplateDroidType_Droid;
        public static clsDroidDesign.clsTemplateDroidType TemplateDroidType_Null;
        public static clsDroidDesign.clsTemplateDroidType TemplateDroidType_Person;
        public static clsDroidDesign.clsTemplateDroidType TemplateDroidType_Transporter;
        public static int TemplateDroidTypeCount;
        public static clsDroidDesign.clsTemplateDroidType[] TemplateDroidTypes = new clsDroidDesign.clsTemplateDroidType[0];
        public static clsBrush TerrainBrush = new clsBrush(2.0, clsBrush.enumShape.Circle);
        public const int TerrainGridSpacing = 0x80;
        public static clsBrush TextureBrush = new clsBrush(0.0, clsBrush.enumShape.Circle);
        public static TileOrientation.sTileOrientation TextureOrientation = new TileOrientation.sTileOrientation(false, false, false);
        public static clsTileset Tileset_Arizona;
        public static clsTileset Tileset_Rockies;
        public static clsTileset Tileset_Urban;
        public static modLists.SimpleList<clsTileset> Tilesets = new modLists.SimpleList<clsTileset>();
        public const int TileTypeNum_Cliff = 8;
        public const int TileTypeNum_Water = 7;
        public static modLists.SimpleList<clsTileType> TileTypes = new modLists.SimpleList<clsTileType>();
        public static GLFont UnitLabelFont;
        public static readonly System.Text.UTF8Encoding UTF8Encoding = new System.Text.UTF8Encoding(false, false);
        public static enumView_Move_Type ViewMoveType = enumView_Move_Type.RTS;
        public static double VisionRadius;
        public static int VisionRadius_2E;
        public static clsBrush VisionSectors = new clsBrush(0.0, clsBrush.enumShape.Circle);
        public const int WZMapMaxSize = 250;

        public static Position.XY_dbl CalcUnitsCentrePos(modLists.SimpleList<clsMap.clsUnit> Units)
        {
            Position.XY_dbl _dbl2;
            IEnumerator enumerator;
            _dbl2.X = 0.0;
            _dbl2.Y = 0.0;
            try
            {
                enumerator = Units.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    clsMap.clsUnit current = (clsMap.clsUnit) enumerator.Current;
                    _dbl2 += current.Pos.Horizontal.ToDoubles();
                }
            }
            finally
            {
                if (enumerator is IDisposable)
                {
                    (enumerator as IDisposable).Dispose();
                }
            }
            return (Position.XY_dbl) (_dbl2 / ((double) Units.Count));
        }

        public static void CreateTemplateDroidTypes()
        {
            TemplateDroidType_Droid = new clsDroidDesign.clsTemplateDroidType("Droid", "DROID");
            TemplateDroidType_Droid.Num = TemplateDroidType_Add(TemplateDroidType_Droid);
            TemplateDroidType_Cyborg = new clsDroidDesign.clsTemplateDroidType("Cyborg", "CYBORG");
            TemplateDroidType_Cyborg.Num = TemplateDroidType_Add(TemplateDroidType_Cyborg);
            TemplateDroidType_CyborgConstruct = new clsDroidDesign.clsTemplateDroidType("Cyborg Construct", "CYBORG_CONSTRUCT");
            TemplateDroidType_CyborgConstruct.Num = TemplateDroidType_Add(TemplateDroidType_CyborgConstruct);
            TemplateDroidType_CyborgRepair = new clsDroidDesign.clsTemplateDroidType("Cyborg Repair", "CYBORG_REPAIR");
            TemplateDroidType_CyborgRepair.Num = TemplateDroidType_Add(TemplateDroidType_CyborgRepair);
            TemplateDroidType_CyborgSuper = new clsDroidDesign.clsTemplateDroidType("Cyborg Super", "CYBORG_SUPER");
            TemplateDroidType_CyborgSuper.Num = TemplateDroidType_Add(TemplateDroidType_CyborgSuper);
            TemplateDroidType_Transporter = new clsDroidDesign.clsTemplateDroidType("Transporter", "TRANSPORTER");
            TemplateDroidType_Transporter.Num = TemplateDroidType_Add(TemplateDroidType_Transporter);
            TemplateDroidType_Person = new clsDroidDesign.clsTemplateDroidType("Person", "PERSON");
            TemplateDroidType_Person.Num = TemplateDroidType_Add(TemplateDroidType_Person);
            TemplateDroidType_Null = new clsDroidDesign.clsTemplateDroidType("Null Droid", "ZNULLDROID");
            TemplateDroidType_Null.Num = TemplateDroidType_Add(TemplateDroidType_Null);
        }

        public static string EndWithPathSeperator(string Text)
        {
            if (Strings.Right(Text, 1) == Conversions.ToString(PlatformPathSeparator))
            {
                return Text;
            }
            return (Text + Conversions.ToString(PlatformPathSeparator));
        }

        public static void ErrorIDChange(uint IntendedID, clsMap.clsUnit IDUnit, string NameOfErrorSource)
        {
            if ((ShowIDErrorMessage && (IDUnit.ID != IntendedID)) && (Interaction.MsgBox("An object's ID has been changed unexpectedly. The error was in \"" + NameOfErrorSource + "\".\r\n\r\nThe object is of type " + IDUnit.Type.GetDisplayTextCode() + " and is at map position " + IDUnit.GetPosText() + ". It's ID was " + modIO.InvariantToString_uint(IntendedID) + ", but is now " + modIO.InvariantToString_uint(IDUnit.ID) + ".\r\n\r\nClick Cancel to stop seeing this message. Otherwise, click OK.", MsgBoxStyle.OkCancel, null) == MsgBoxResult.Cancel))
            {
                ShowIDErrorMessage = false;
            }
        }

        public static clsDroidDesign.clsTemplateDroidType GetTemplateDroidTypeFromTemplateCode(string Code)
        {
            string str = Code.ToLower();
            int num2 = TemplateDroidTypeCount - 1;
            for (int i = 0; i <= num2; i++)
            {
                if (TemplateDroidTypes[i].TemplateCode.ToLower() == str)
                {
                    return TemplateDroidTypes[i];
                }
            }
            return null;
        }

        public static clsTurret.enumTurretType GetTurretTypeFromName(string TurretTypeName)
        {
            switch (TurretTypeName.ToLower())
            {
                case "weapon":
                    return clsTurret.enumTurretType.Weapon;

                case "construct":
                    return clsTurret.enumTurretType.Construct;

                case "repair":
                    return clsTurret.enumTurretType.Repair;

                case "sensor":
                    return clsTurret.enumTurretType.Sensor;

                case "brain":
                    return clsTurret.enumTurretType.Brain;

                case "ecm":
                    return clsTurret.enumTurretType.ECM;
            }
            return clsTurret.enumTurretType.Unknown;
        }

        public static clsResult LoadTilesets(string TilesetsPath)
        {
            string[] directories;
            clsResult result3 = new clsResult("Loading tilesets");
            try
            {
                directories = Directory.GetDirectories(TilesetsPath);
            }
            catch (Exception exception1)
            {
                ProjectData.SetProjectError(exception1);
                Exception exception = exception1;
                result3.ProblemAdd(exception.Message);
                clsResult result = result3;
                ProjectData.ClearProjectError();
                return result;
            }
            if (directories != null)
            {
                clsTileset current;
                IEnumerator enumerator;
                foreach (string str in directories)
                {
                    current = new clsTileset();
                    clsResult resultToAdd = current.LoadDirectory(str);
                    result3.Add(resultToAdd);
                    if (!resultToAdd.HasProblems)
                    {
                        Tilesets.Add(current);
                    }
                }
                try
                {
                    enumerator = Tilesets.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        current = (clsTileset) enumerator.Current;
                        if (current.Name == "tertilesc1hw")
                        {
                            current.Name = "Arizona";
                            Tileset_Arizona = current;
                            current.IsOriginal = true;
                            current.BGColour = new sRGB_sng(0.8f, 0.5843138f, 0.2745098f);
                        }
                        else
                        {
                            if (current.Name == "tertilesc2hw")
                            {
                                current.Name = "Urban";
                                Tileset_Urban = current;
                                current.IsOriginal = true;
                                current.BGColour = new sRGB_sng(0.4627451f, 0.6470588f, 0.7960784f);
                                continue;
                            }
                            if (current.Name == "tertilesc3hw")
                            {
                                current.Name = "Rocky Mountains";
                                Tileset_Rockies = current;
                                current.IsOriginal = true;
                                current.BGColour = new sRGB_sng(0.7137255f, 0.8823529f, 0.9254902f);
                            }
                        }
                    }
                }
                finally
                {
                    if (enumerator is IDisposable)
                    {
                        (enumerator as IDisposable).Dispose();
                    }
                }
                if (Tileset_Arizona == null)
                {
                    result3.WarningAdd("Arizona tileset is missing.");
                }
                if (Tileset_Urban == null)
                {
                    result3.WarningAdd("Urban tileset is missing.");
                }
                if (Tileset_Rockies == null)
                {
                    result3.WarningAdd("Rocky Mountains tileset is missing.");
                }
            }
            return result3;
        }

        public static string MinDigits(int Number, int Digits)
        {
            string str2 = modIO.InvariantToString_int(Number);
            int number = Digits - str2.Length;
            if (number > 0)
            {
                str2 = Strings.StrDup(number, '0') + str2;
            }
            return str2;
        }

        public static bool PosIsWithinTileArea(modMath.sXY_int WorldHorizontal, modMath.sXY_int StartTile, modMath.sXY_int FinishTile)
        {
            return ((((WorldHorizontal.X >= (StartTile.X * 0x80)) & (WorldHorizontal.Y >= (StartTile.Y * 0x80))) & (WorldHorizontal.X < (FinishTile.X * 0x80))) & (WorldHorizontal.Y < (FinishTile.Y * 0x80)));
        }

        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        public static void SetProgramSubDirs()
        {
            MyDocumentsProgramPath = MyProject.Computer.FileSystem.SpecialDirectories.MyDocuments + Conversions.ToString(PlatformPathSeparator) + ".flaME";
            SettingsPath = MyProject.Application.Info.DirectoryPath + Conversions.ToString(PlatformPathSeparator) + "settings.ini";
            AutoSavePath = MyProject.Application.Info.DirectoryPath + Conversions.ToString(PlatformPathSeparator) + "autosave" + Conversions.ToString(PlatformPathSeparator);
            InterfaceImagesPath = MyProject.Application.Info.DirectoryPath + Conversions.ToString(PlatformPathSeparator) + "interface" + Conversions.ToString(PlatformPathSeparator);
        }

        public static void ShowWarnings(clsResult Result)
        {
            if (Result.HasWarnings)
            {
                frmWarnings warnings = new frmWarnings(Result, Result.Text);
                warnings.Show();
                warnings.Activate();
            }
        }

        public static bool SizeIsPowerOf2(int Size)
        {
            double a = Math.Log((double) Size) / Math.Log(2.0);
            return (a == ((int) Math.Round(a)));
        }

        public static int TemplateDroidType_Add(clsDroidDesign.clsTemplateDroidType NewDroidType)
        {
            TemplateDroidTypes = (clsDroidDesign.clsTemplateDroidType[]) Utils.CopyArray((Array) TemplateDroidTypes, new clsDroidDesign.clsTemplateDroidType[TemplateDroidTypeCount + 1]);
            TemplateDroidTypes[TemplateDroidTypeCount] = NewDroidType;
            int templateDroidTypeCount = TemplateDroidTypeCount;
            TemplateDroidTypeCount++;
            return templateDroidTypeCount;
        }

        public static void View_Radius_Set(double Radius)
        {
            VisionSectors.Radius = Radius / 1024.0;
        }

        public static void ViewKeyDown_Clear()
        {
            IEnumerator enumerator;
            IsViewKeyDown.Deactivate();
            try
            {
                enumerator = modControls.Options_KeyboardControls.Options.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    clsOption<clsKeyboardControl> current = (clsOption<clsKeyboardControl>) enumerator.Current;
                    ((clsKeyboardControl) modControls.KeyboardProfile.get_Value(current)).KeysChanged(IsViewKeyDown);
                }
            }
            finally
            {
                if (enumerator is IDisposable)
                {
                    (enumerator as IDisposable).Dispose();
                }
            }
        }

        public static void VisionRadius_2E_Changed()
        {
            VisionRadius = 256.0 * Math.Pow(2.0, ((double) VisionRadius_2E) / 2.0);
            if (modMain.frmMainInstance.MapView != null)
            {
                View_Radius_Set(VisionRadius);
                modMain.frmMainInstance.View_DrawViewLater();
            }
        }

        public static void ZeroIDWarning(clsMap.clsUnit IDUnit, uint NewID, clsResult Output)
        {
            string text = "An object's ID has been changed from 0 to " + modIO.InvariantToString_uint(NewID) + ". Zero is not a valid ID. The object is of type " + IDUnit.Type.GetDisplayTextCode() + " and is at map position " + IDUnit.GetPosText() + ".";
            Output.WarningAdd(text);
        }

        public class clsKeysActive
        {
            public bool[] Keys = new bool[0x100];

            public void Deactivate()
            {
                int index = 0;
                do
                {
                    this.Keys[index] = false;
                    index++;
                }
                while (index <= 0xff);
            }
        }

        public class clsPlayer
        {
            public sRGB_sng Colour;
            public sRGB_sng MinimapColour;

            public void CalcMinimapColour()
            {
                this.MinimapColour.Red = Math.Min((float) ((this.Colour.Red * 0.6666667f) + 0.3333333f), (float) 1f);
                this.MinimapColour.Green = Math.Min((float) ((this.Colour.Green * 0.6666667f) + 0.3333333f), (float) 1f);
                this.MinimapColour.Blue = Math.Min((float) ((this.Colour.Blue * 0.6666667f) + 0.3333333f), (float) 1f);
            }
        }

        public class clsTileType
        {
            public sRGB_sng DisplayColour;
            public string Name;
        }

        public class clsWorldPos
        {
            public modProgram.sWorldPos WorldPos;

            public clsWorldPos(modProgram.sWorldPos NewWorldPos)
            {
                this.WorldPos = NewWorldPos;
            }
        }

        public enum enumDrawLighting : byte
        {
            Half = 1,
            Normal = 2,
            Off = 0
        }

        public enum enumDroidType : byte
        {
            Command = 7,
            Construct = 3,
            Cyborg = 5,
            Cyborg_Construct = 10,
            Cyborg_Repair = 11,
            Cyborg_Super = 12,
            Default_ = 9,
            ECM = 2,
            Person = 4,
            Repair = 8,
            Sensor = 1,
            Transporter = 6,
            Weapon = 0
        }

        public enum enumFillCliffAction : byte
        {
            Ignore = 0,
            StopAfter = 2,
            StopBefore = 1
        }

        public enum enumObjectRotateMode : byte
        {
            All = 2,
            None = 0,
            Walls = 1
        }

        public enum enumTextureTerrainAction : byte
        {
            Ignore = 0,
            Reinterpret = 1,
            Remove = 2
        }

        public enum enumTileWalls
        {
            Bottom = 8,
            Left = 1,
            None = 0,
            Right = 2,
            Top = 4
        }

        public enum enumView_Move_Type : byte
        {
            Free = 0,
            RTS = 1
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct sLayerList
        {
            public clsLayer[] Layers;
            public int LayerCount;
            public void Layer_Insert(int PositionNum, clsLayer NewLayer)
            {
                int num;
                this.Layers = (clsLayer[]) Utils.CopyArray((Array) this.Layers, new clsLayer[this.LayerCount + 1]);
                int num3 = PositionNum;
                for (num = this.LayerCount - 1; num >= num3; num += -1)
                {
                    this.Layers[num + 1] = this.Layers[num];
                }
                this.Layers[PositionNum] = NewLayer;
                this.LayerCount++;
                int num4 = this.LayerCount - 1;
                for (num = 0; num <= num4; num++)
                {
                    if (this.Layers[num].WithinLayer >= PositionNum)
                    {
                        this.Layers[num].WithinLayer++;
                    }
                    this.Layers[num].AvoidLayers = (bool[]) Utils.CopyArray((Array) this.Layers[num].AvoidLayers, new bool[(this.LayerCount - 1) + 1]);
                    int num5 = PositionNum;
                    for (int i = this.LayerCount - 2; i >= num5; i += -1)
                    {
                        this.Layers[num].AvoidLayers[i + 1] = this.Layers[num].AvoidLayers[i];
                    }
                    this.Layers[num].AvoidLayers[PositionNum] = false;
                }
            }

            public void Layer_Remove(int Layer_Num)
            {
                int num;
                this.LayerCount--;
                int num3 = this.LayerCount - 1;
                for (num = Layer_Num; num <= num3; num++)
                {
                    this.Layers[num] = this.Layers[num + 1];
                }
                this.Layers = (clsLayer[]) Utils.CopyArray((Array) this.Layers, new clsLayer[(this.LayerCount - 1) + 1]);
                int num4 = this.LayerCount - 1;
                for (num = 0; num <= num4; num++)
                {
                    if (this.Layers[num].WithinLayer == Layer_Num)
                    {
                        this.Layers[num].WithinLayer = -1;
                    }
                    else if (this.Layers[num].WithinLayer > Layer_Num)
                    {
                        this.Layers[num].WithinLayer--;
                    }
                    int num5 = this.LayerCount - 1;
                    for (int i = Layer_Num; i <= num5; i++)
                    {
                        this.Layers[num].AvoidLayers[i] = this.Layers[num].AvoidLayers[i + 1];
                    }
                    this.Layers[num].AvoidLayers = (bool[]) Utils.CopyArray((Array) this.Layers[num].AvoidLayers, new bool[(this.LayerCount - 1) + 1]);
                }
            }

            public void Layer_Move(int Layer_Num, int Layer_Dest_Num)
            {
                int num;
                int num2;
                bool flag;
                clsLayer layer;
                if (Layer_Dest_Num < Layer_Num)
                {
                    layer = this.Layers[Layer_Num];
                    int num3 = Layer_Dest_Num;
                    for (num = Layer_Num - 1; num >= num3; num += -1)
                    {
                        this.Layers[num + 1] = this.Layers[num];
                    }
                    this.Layers[Layer_Dest_Num] = layer;
                    int num4 = this.LayerCount - 1;
                    for (num = 0; num <= num4; num++)
                    {
                        if (this.Layers[num].WithinLayer == Layer_Num)
                        {
                            this.Layers[num].WithinLayer = Layer_Dest_Num;
                        }
                        else if ((this.Layers[num].WithinLayer >= Layer_Dest_Num) & (this.Layers[num].WithinLayer < Layer_Num))
                        {
                            this.Layers[num].WithinLayer++;
                        }
                        flag = this.Layers[num].AvoidLayers[Layer_Num];
                        int num5 = Layer_Dest_Num;
                        num2 = Layer_Num - 1;
                        while (num2 >= num5)
                        {
                            this.Layers[num].AvoidLayers[num2 + 1] = this.Layers[num].AvoidLayers[num2];
                            num2 += -1;
                        }
                        this.Layers[num].AvoidLayers[Layer_Dest_Num] = flag;
                    }
                }
                else if (Layer_Dest_Num > Layer_Num)
                {
                    layer = this.Layers[Layer_Num];
                    int num6 = Layer_Dest_Num - 1;
                    for (num = Layer_Num; num <= num6; num++)
                    {
                        this.Layers[num] = this.Layers[num + 1];
                    }
                    this.Layers[Layer_Dest_Num] = layer;
                    int num7 = this.LayerCount - 1;
                    for (num = 0; num <= num7; num++)
                    {
                        if (this.Layers[num].WithinLayer == Layer_Num)
                        {
                            this.Layers[num].WithinLayer = Layer_Dest_Num;
                        }
                        else if ((this.Layers[num].WithinLayer > Layer_Num) & (this.Layers[num].WithinLayer <= Layer_Dest_Num))
                        {
                            this.Layers[num].WithinLayer--;
                        }
                        flag = this.Layers[num].AvoidLayers[Layer_Num];
                        int num8 = Layer_Dest_Num - 1;
                        for (num2 = Layer_Num; num2 <= num8; num2++)
                        {
                            this.Layers[num].AvoidLayers[num2] = this.Layers[num].AvoidLayers[num2 + 1];
                        }
                        this.Layers[num].AvoidLayers[Layer_Dest_Num] = flag;
                    }
                }
            }
            public class clsLayer
            {
                public bool[] AvoidLayers;
                public float Density;
                public float HeightMax;
                public float HeightMin;
                public float Scale;
                public float SlopeMax;
                public float SlopeMin;
                public clsPainter.clsTerrain Terrain;
                public clsBooleanMap Terrainmap;
                public int WithinLayer;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct sResult
        {
            public bool Success;
            public string Problem;
        }

        [StructLayout(LayoutKind.Sequential)]
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
                this = new modProgram.sSplitPath();
                this.Parts = Path.Split(new char[] { modProgram.PlatformPathSeparator });
                this.PartCount = this.Parts.GetUpperBound(0) + 1;
                this.FilePath = "";
                int num2 = this.PartCount - 2;
                int index = 0;
                while (index <= num2)
                {
                    this.FilePath = this.FilePath + this.Parts[index] + Conversions.ToString(modProgram.PlatformPathSeparator);
                    index++;
                }
                this.FileTitle = this.Parts[index];
                index = Strings.InStrRev(this.FileTitle, ".", -1, CompareMethod.Binary);
                if (index > 0)
                {
                    this.FileExtension = Strings.Right(this.FileTitle, this.FileTitle.Length - index);
                    this.FileTitleWithoutExtension = Strings.Left(this.FileTitle, index - 1);
                }
                else
                {
                    this.FileExtension = "";
                    this.FileTitleWithoutExtension = this.FileTitle;
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct sWorldPos
        {
            public modMath.sXY_int Horizontal;
            public int Altitude;
            public sWorldPos(modMath.sXY_int NewHorizontal, int NewAltitude)
            {
                this = new modProgram.sWorldPos();
                this.Horizontal = NewHorizontal;
                this.Altitude = NewAltitude;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct sWZAngle
        {
            public ushort Direction;
            public ushort Pitch;
            public ushort Roll;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct sZipSplitPath
        {
            public string[] Parts;
            public int PartCount;
            public string FilePath;
            public string FileTitle;
            public string FileTitleWithoutExtension;
            public string FileExtension;
            public sZipSplitPath(string Path)
            {
                this = new modProgram.sZipSplitPath();
                this.Parts = Path.ToLower().Replace('\\', '/').Split(new char[] { '/' });
                this.PartCount = this.Parts.GetUpperBound(0) + 1;
                this.FilePath = "";
                int num2 = this.PartCount - 2;
                int index = 0;
                while (index <= num2)
                {
                    this.FilePath = this.FilePath + this.Parts[index] + "/";
                    index++;
                }
                this.FileTitle = this.Parts[index];
                index = Strings.InStrRev(this.FileTitle, ".", -1, CompareMethod.Binary);
                if (index > 0)
                {
                    this.FileExtension = Strings.Right(this.FileTitle, this.FileTitle.Length - index);
                    this.FileTitleWithoutExtension = Strings.Left(this.FileTitle, index - 1);
                }
                else
                {
                    this.FileExtension = "";
                    this.FileTitleWithoutExtension = this.FileTitle;
                }
            }
        }
    }
}

