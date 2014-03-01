#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using NLog;
using OpenTK;
using OpenTK.Graphics;
using SharpFlame.Core.Extensions;
using SharpFlame.Old.AppSettings;
using SharpFlame.Core;
using SharpFlame.Core.Collections;
using SharpFlame.Core.Domain;
using SharpFlame.Core.Domain.Colors;
using SharpFlame.Old.Domain;
using SharpFlame.Old.Domain.ObjData;
using SharpFlame.Old.FileIO;
using SharpFlame.Old.Graphics.OpenGL;
using SharpFlame.Old.Mapping;
using SharpFlame.Old.Mapping.Objects;
using SharpFlame.Old.Mapping.Tiles;
using SharpFlame.Old.Maths;
using SharpFlame.Old.Painters;
using SharpFlame.Old.Util;
using SharpFlame.Old.UiOptions;

#endregion

namespace SharpFlame.Old
{
    public sealed class App
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public static SRgb MinimapFeatureColour;

        public static char PlatformPathSeparator = Path.DirectorySeparatorChar;

        public static bool DebugGL = false;

        public static string SettingsPath;
        public static string AutoSavePath;

        public static Random Random;

        public static bool ProgramInitialized = false;
        public static bool ProgramInitializeFinished = false;

        public static Icon ProgramIcon;

        public static SimpleList<string> CommandLinePaths = new SimpleList<string>();

        public static int GLTexture_NoTile;
        public static int GLTexture_OverflowTile;

        public static clsKeysActive IsViewKeyDown = new clsKeysActive();

        public static clsBrush TextureBrush = new clsBrush(0.0D, ShapeType.Circle);
        public static clsBrush TerrainBrush = new clsBrush(2.0D, ShapeType.Circle);
        public static clsBrush HeightBrush = new clsBrush(2.0D, ShapeType.Circle);
        public static clsBrush CliffBrush = new clsBrush(2.0D, ShapeType.Circle);

        public static clsBrush SmoothRadius = new clsBrush(1.0D, ShapeType.Square);

        public static bool DisplayTileOrientation;

        public static clsObjectData ObjectData;

        public static int SelectedTextureNum = -1;
        public static TileOrientation TextureOrientation = new TileOrientation(false, false, false);

        public static Terrain SelectedTerrain;
        public static Road SelectedRoad;

        public static SimpleList<clsTileType> TileTypes = new SimpleList<clsTileType>();

        public static DroidDesign.clsTemplateDroidType[] TemplateDroidTypes = new DroidDesign.clsTemplateDroidType[0];
        public static int TemplateDroidTypeCount;

        public static readonly UTF8Encoding UTF8Encoding = new UTF8Encoding(false, false);
        public static readonly ASCIIEncoding ASCIIEncoding = new ASCIIEncoding();

        public static int VisionRadius_2E;
        public static double VisionRadius;

        public static Map Copied_Map;

        public static Tileset Tileset_Arizona;
        public static Tileset Tileset_Urban;
        public static Tileset Tileset_Rockies;

        public static Painter Painter_Arizona;
        public static Painter Painter_Urban;
        public static Painter Painter_Rockies;

        public static Font UnitLabelBaseFont;
        public static GLFont UnitLabelFont;
        //Public TextureViewFont As GLFont

        public static Player[] PlayerColour = new Player[16];

        public static DroidDesign.clsTemplateDroidType TemplateDroidType_Droid;
        public static DroidDesign.clsTemplateDroidType TemplateDroidType_Cyborg;
        public static DroidDesign.clsTemplateDroidType TemplateDroidType_CyborgConstruct;
        public static DroidDesign.clsTemplateDroidType TemplateDroidType_CyborgRepair;
        public static DroidDesign.clsTemplateDroidType TemplateDroidType_CyborgSuper;
        public static DroidDesign.clsTemplateDroidType TemplateDroidType_Transporter;
        public static DroidDesign.clsTemplateDroidType TemplateDroidType_Person;
        public static DroidDesign.clsTemplateDroidType TemplateDroidType_Null;

        public static bool ShowIDErrorMessage = true;

        public static bool Draw_TileTextures = true;

        public static DrawLighting Draw_Lighting = DrawLighting.Half;
        public static bool Draw_TileWireframe;
        public static bool Draw_Units = true;
        public static bool Draw_VertexTerrain;
        public static bool Draw_Gateways;
        public static bool Draw_ScriptMarkers = true;

        public static ViewMoveType ViewMoveType = ViewMoveType.RTS;
        public static bool RTSOrbit = true;

        public static Matrix3DMath.Matrix3D SunAngleMatrix = new Matrix3DMath.Matrix3D();
        public static clsBrush VisionSectors = new clsBrush(0.0D, ShapeType.Circle);

        public static sLayerList LayerList;

        private static ObservableCollection<Tileset> tilesets = new ObservableCollection<Tileset>();
        public static ObservableCollection<Tileset> Tilesets { get { return tilesets; } }

        public static Options UiOptions = new Options();

        public static void Initalize ()
        {
            createTileTypes ();
        }

        private static void createTileTypes()
        {
            clsTileType newTileType;

            newTileType = new clsTileType();
            newTileType.Name = "Sand";
            newTileType.DisplayColour.Red = 1.0F;
            newTileType.DisplayColour.Green = 1.0F;
            newTileType.DisplayColour.Blue = 0.0F;
            App.TileTypes.Add(newTileType);

            newTileType = new clsTileType();
            newTileType.Name = "Sandy Brush";
            newTileType.DisplayColour.Red = 0.5F;
            newTileType.DisplayColour.Green = 0.5F;
            newTileType.DisplayColour.Blue = 0.0F;
            App.TileTypes.Add(newTileType);

            newTileType = new clsTileType();
            newTileType.Name = "Rubble";
            newTileType.DisplayColour.Red = 0.25F;
            newTileType.DisplayColour.Green = 0.25F;
            newTileType.DisplayColour.Blue = 0.25F;
            App.TileTypes.Add(newTileType);

            newTileType = new clsTileType();
            newTileType.Name = "Green Mud";
            newTileType.DisplayColour.Red = 0.0F;
            newTileType.DisplayColour.Green = 0.5F;
            newTileType.DisplayColour.Blue = 0.0F;
            App.TileTypes.Add(newTileType);

            newTileType = new clsTileType();
            newTileType.Name = "Red Brush";
            newTileType.DisplayColour.Red = 1.0F;
            newTileType.DisplayColour.Green = 0.0F;
            newTileType.DisplayColour.Blue = 0.0F;
            App.TileTypes.Add(newTileType);

            newTileType = new clsTileType();
            newTileType.Name = "Pink Rock";
            newTileType.DisplayColour.Red = 1.0F;
            newTileType.DisplayColour.Green = 0.5F;
            newTileType.DisplayColour.Blue = 0.5F;
            App.TileTypes.Add(newTileType);

            newTileType = new clsTileType();
            newTileType.Name = "Road";
            newTileType.DisplayColour.Red = 0.0F;
            newTileType.DisplayColour.Green = 0.0F;
            newTileType.DisplayColour.Blue = 0.0F;
            App.TileTypes.Add(newTileType);

            newTileType = new clsTileType();
            newTileType.Name = "Water";
            newTileType.DisplayColour.Red = 0.0F;
            newTileType.DisplayColour.Green = 0.0F;
            newTileType.DisplayColour.Blue = 1.0F;
            App.TileTypes.Add(newTileType);

            newTileType = new clsTileType();
            newTileType.Name = "Cliff Face";
            newTileType.DisplayColour.Red = 0.5F;
            newTileType.DisplayColour.Green = 0.5F;
            newTileType.DisplayColour.Blue = 0.5F;
            App.TileTypes.Add(newTileType);

            newTileType = new clsTileType();
            newTileType.Name = "Baked Earth";
            newTileType.DisplayColour.Red = 0.5F;
            newTileType.DisplayColour.Green = 0.0F;
            newTileType.DisplayColour.Blue = 0.0F;
            App.TileTypes.Add(newTileType);

            newTileType = new clsTileType();
            newTileType.Name = "Sheet Ice";
            newTileType.DisplayColour.Red = 1.0F;
            newTileType.DisplayColour.Green = 1.0F;
            newTileType.DisplayColour.Blue = 1.0F;
            App.TileTypes.Add(newTileType);

            newTileType = new clsTileType();
            newTileType.Name = "Slush";
            newTileType.DisplayColour.Red = 0.75F;
            newTileType.DisplayColour.Green = 0.75F;
            newTileType.DisplayColour.Blue = 0.75F;
            App.TileTypes.Add(newTileType);
        }


        public static void SetProgramSubDirs()
        {
#if !Portable
            var myDocumentsProgramPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).CombinePathWith(".flaME", true);
            SettingsPath = myDocumentsProgramPath.CombinePathWith("settings.json");
            AutoSavePath = myDocumentsProgramPath.CombinePathWith("autosave", true);

#else
            SettingsPath = AppDomain.CurrentDomain.BaseDirectory.CombinePathWith("settings.json");
            AutoSavePath = AppDomain.CurrentDomain.BaseDirectory.CombinePathWith("autosave", true);
#endif
            // Create the directories.
            if ( !Directory.Exists(AutoSavePath) )
            {
                try
                {
                    Directory.CreateDirectory(AutoSavePath);
                }
                catch ( Exception ex )
                {
                    logger.Error("Unable to create folder \"{0}\": {1}", AutoSavePath, ex.Message);
                    Application.Exit();
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
            var ReturnResult = Number.ToStringInvariant();
            ReturnResult = ReturnResult.PadLeft(Digits, '0');

            return ReturnResult;
        }

        public static void ViewKeyDown_Clear()
        {
            IsViewKeyDown.Deactivate();

            foreach ( Option<KeyboardControl> control in KeyboardManager.OptionsKeyboardControls.Options )
            {
                ((KeyboardControl)(KeyboardManager.KeyboardProfile.GetValue(control))).KeysChanged(IsViewKeyDown);
            }
        }

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

        public static DroidDesign.clsTemplateDroidType GetTemplateDroidTypeFromTemplateCode(string code)
        {
            var lCaseCode = code.ToLower();

            for (var a = 0; a <= TemplateDroidTypeCount - 1; a++ )
            {
                if ( TemplateDroidTypes[a].TemplateCode.ToLower() == lCaseCode )
                {
                    return TemplateDroidTypes[a];
                }
            }
            return null;
        }

        public static int TemplateDroidType_Add(DroidDesign.clsTemplateDroidType NewDroidType)
        {
            Array.Resize(ref TemplateDroidTypes, TemplateDroidTypeCount + 1);
            TemplateDroidTypes[TemplateDroidTypeCount] = NewDroidType;
            var returnResult = TemplateDroidTypeCount;
            TemplateDroidTypeCount++;

            return returnResult;
        }

        public static void ShowWarnings(Result result)
        {
            if ( !result.HasWarnings )
            {
                return;
            }

            var warningsForm = new frmWarnings(result, result.Text);
            warningsForm.Show();
            warningsForm.Activate();
        }

        public static TurretType GetTurretTypeFromName(string turretTypeName)
        {
            switch ( turretTypeName.ToLower() )
            {
                case "weapon":
                    return TurretType.Weapon;
                case "construct":
                    return TurretType.Construct;
                case "repair":
                    return TurretType.Repair;
                case "sensor":
                    return TurretType.Sensor;
                case "brain":
                    return TurretType.Brain;
                case "ecm":
                    return TurretType.ECM;
                default:
                    return TurretType.Unknown;
            }
        }

        public static void ErrorIDChange(UInt32 intendedID, Unit idUnit, string nameOfErrorSource)
        {
            if ( !ShowIDErrorMessage )
            {
                return;
            }

            if ( idUnit.ID == intendedID )
            {
                return;
            }

            var messageText = "An object\'s ID has been changed unexpectedly. The error was in \"{0}\"\n\n" +
                              "The object is of type {1} and is at map position {2}. " +
                              "It\'s ID was {3}, but is now {4}.\n\n" +
                              "Click Cancel to stop seeing this message. Otherwise, click OK.".Format2(nameOfErrorSource, idUnit.TypeBase.GetDisplayTextCode(),
                                  idUnit.GetPosText(), intendedID.ToStringInvariant(), idUnit.ID.ToStringInvariant());
            const string caption = "An object\'s ID has been changed unexpectedly.";

            var result = MessageBox.Show(messageText, caption, MessageBoxButtons.OKCancel, MessageBoxIcon.None);
            if ( result == DialogResult.Cancel )
            {
                ShowIDErrorMessage = false;
            }
        }

        public static void ZeroIDWarning(Unit IDUnit, UInt32 NewID, Result Output)
        {
            var MessageText = string.Format ("An object\'s ID has been changed from 0 to {0}. Zero is not a valid ID. The object is of type {1} and is at map position {2}.", NewID.ToStringInvariant (), IDUnit.TypeBase.GetDisplayTextCode (), IDUnit.GetPosText ());

            //MsgBox(MessageText, MsgBoxStyle.OkOnly)
            Output.WarningAdd(MessageText);
        }

        public static bool PosIsWithinTileArea(XYInt worldHorizontal, XYInt startTile, XYInt finishTile)
        {
            return worldHorizontal.X >= startTile.X * Constants.TerrainGridSpacing &
                   worldHorizontal.Y >= startTile.Y * Constants.TerrainGridSpacing &
                   worldHorizontal.X < finishTile.X * Constants.TerrainGridSpacing &
                   worldHorizontal.Y < finishTile.Y * Constants.TerrainGridSpacing;
        }

        public static bool SizeIsPowerOf2(int Size)
        {
            var Power = Math.Log(Size) / Math.Log(2.0D);
            return Power == (int)Power;
        }

        public static Result LoadTilesets(string TilesetsPath)
        {
            var returnResult = new Result("Loading tilesets", false);
            logger.Info("Loading tilesets");

            string[] tilesetDirs;
            try
            {
                tilesetDirs = Directory.GetDirectories(TilesetsPath);
            }
            catch ( Exception ex )
            {
                returnResult.ProblemAdd(ex.Message);
                return returnResult;
            }

            var tmpTilesets = new List<Tileset> ();;
            foreach ( var path in tilesetDirs )
            {
                var tileset = new Tileset();
                var loader = new TilesetLoader (ref tileset);
                var result = loader.Load (path);
                returnResult.Add(result);
                if ( !result.HasProblems )
                {
                    tmpTilesets.Add(tileset);
                }
            }

            foreach ( var tileset in tmpTilesets )
            {
                if ( tileset.Name == "tertilesc1hw" )
                {
                    tileset.Name = "Arizona";
                    Tileset_Arizona = tileset;
                    tileset.IsOriginal = true;
                    tileset.BGColour = new SRgb(204.0f / 255.0f, 149.0f / 255.0f, 70.0f / 255.0f);
                }
                else if ( tileset.Name == "tertilesc2hw" )
                {
                    tileset.Name = "Urban";
                    Tileset_Urban = tileset;
                    tileset.IsOriginal = true;
                    tileset.BGColour = new SRgb(118.0f / 255.0f, 165.0f / 255.0f, 203.0f / 255.0f);
                }
                else if ( tileset.Name == "tertilesc3hw" )
                {
                    tileset.Name = "Rocky Mountains";
                    Tileset_Rockies = tileset;
                    tileset.IsOriginal = true;
                    tileset.BGColour = new SRgb(182.0f / 255.0f, 225.0f / 255.0f, 236.0f / 255.0f);
                }

                Tilesets.Add (tileset);
            }

            if ( Tileset_Arizona == null )
            {
                returnResult.WarningAdd("Arizona tileset is missing.");
            }
            if ( Tileset_Urban == null )
            {
                returnResult.WarningAdd("Urban tileset is missing.");
            }
            if ( Tileset_Rockies == null )
            {
                returnResult.WarningAdd("Rocky Mountains tileset is missing.");
            }

            return returnResult;
        }

        public static void View_Radius_Set(double radius)
        {
            VisionSectors.Radius = radius / (Constants.TerrainGridSpacing * Constants.SectorTileSize);
        }

        public static XYDouble CalcUnitsCentrePos(SimpleList<Unit> units)
        {
            var result = default(XYDouble);

            result.X = 0.0D;
            result.Y = 0.0D;
            foreach ( var unit in units )
            {
                result += unit.Pos.Horizontal.ToDoubles();
            }
            result /= units.Count;

            return result;
        }
    }
}