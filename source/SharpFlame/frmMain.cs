using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Matrix3D;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using OpenTK.Graphics.OpenGL;

namespace SharpFlame
{
    public partial class frmMain
    {
        public class clsMaps : modLists.ConnectedList<clsMap, frmMain>
        {
            private clsMap _MainMap;

            public override void Add(modLists.ConnectedListItem<clsMap, frmMain> NewItem)
            {
                clsMap NewMap = NewItem.Item;

                if ( !NewMap.ReadyForUserInput )
                {
                    NewMap.InitializeUserInput();
                }

                NewMap.MapView_TabPage = new TabPage();
                NewMap.MapView_TabPage.Tag = NewMap;

                NewMap.SetTabText();

                base.Add(NewItem);

                Owner.MapView.UpdateTabs();
            }

            public override void Remove(int Position)
            {
                clsMap Map = this[Position];

                base.Remove(Position);

                if ( Map == _MainMap )
                {
                    int NewNum = Math.Min(System.Convert.ToInt32(Owner.MapView.tabMaps.SelectedIndex), Count - 1);
                    if ( NewNum < 0 )
                    {
                        MainMap = null;
                    }
                    else
                    {
                        MainMap = this[NewNum];
                    }
                }

                Map.MapView_TabPage.Tag = null;
                Map.MapView_TabPage = null;

                Owner.MapView.UpdateTabs();
            }

            public clsMap MainMap
            {
                get { return _MainMap; }
                set
                {
                    if ( value == _MainMap )
                    {
                        return;
                    }
                    frmMain MainForm = Owner;
                    MainForm.MainMapBeforeChanged();
                    if ( value == null )
                    {
                        _MainMap = null;
                    }
                    else
                    {
                        if ( value.frmMainLink.Source != Owner )
                        {
                            MessageBox.Show("Error: Assigning map to wrong main form.");
                            _MainMap = null;
                        }
                        else
                        {
                            _MainMap = value;
                        }
                    }
                    MainForm.MainMapAfterChanged();
                }
            }

            public clsMaps(frmMain Owner) : base(Owner)
            {
                MaintainOrder = true;
            }
        }

        private clsMaps _LoadedMaps;

        public ctrlMapView MapView;
        public ctrlTextureView TextureView;

        public clsBody[] cboBody_Objects;
        public clsPropulsion[] cboPropulsion_Objects;
        public clsTurret[] cboTurret_Objects;

        public byte[] HeightSetPalette = new byte[8];

        public modLists.ConnectedList<clsUnitType, frmMain> SelectedObjectTypes;

        public modProgram.enumTextureTerrainAction TextureTerrainAction = modProgram.enumTextureTerrainAction.Reinterpret;

        public modProgram.enumFillCliffAction FillCliffAction = modProgram.enumFillCliffAction.Ignore;

        public Timer tmrKey;
        public Timer tmrTool;

        public ctrlPlayerNum NewPlayerNum;
        public ctrlPlayerNum ObjectPlayerNum;

        public ctrlBrush ctrlTextureBrush;
        public ctrlBrush ctrlTerrainBrush;
        public ctrlBrush ctrlCliffRemoveBrush;
        public ctrlBrush ctrlHeightBrush;

        public frmMain()
        {
            _LoadedMaps = new clsMaps(this);
            SelectedObjectTypes = new modLists.ConnectedList<clsUnitType, frmMain>(this);

            InitializeComponent();

#if Mono
			int A = 0;
			for (A = 0; A <= TabControl.TabPages.Count - 1; A++)
			{
				TabControl.TabPages[A].Text += " ";
			}
			for (A = 0; A <= TabControl1.TabPages.Count - 1; A++)
			{
				TabControl1.TabPages[A].Text += " ";
			}
#endif

            MapView = new ctrlMapView(this);
            TextureView = new ctrlTextureView(this);

            modMain.frmGeneratorInstance = new frmGenerator(this);

            tmrKey = new Timer();
            tmrKey.Tick += tmrKey_Tick;
            tmrKey.Interval = 30;

            tmrTool = new Timer();
            tmrTool.Tick += tmrTool_Tick;
            tmrTool.Interval = 100;

            NewPlayerNum = new ctrlPlayerNum();
            ObjectPlayerNum = new ctrlPlayerNum();
        }

        private clsResult LoadInterfaceImages()
        {
            clsResult ReturnResult = new clsResult("Loading interface images");

            Bitmap InterfaceImage_DisplayAutoTexture = null;
            Bitmap InterfaceImage_DrawTileOrientation = null;
            Bitmap InterfaceImage_QuickSave = null;
            Bitmap InterfaceImage_Selection = null;
            Bitmap InterfaceImage_ObjectSelect = null;
            Bitmap InterfaceImage_SelectionCopy = null;
            Bitmap InterfaceImage_SelectionFlipX = null;
            Bitmap InterfaceImage_SelectionRotateClockwise = null;
            Bitmap InterfaceImage_SelectionRotateCounterClockwise = null;
            Bitmap InterfaceImage_SelectionPaste = null;
            Bitmap InterfaceImage_SelectionPasteOptions = null;
            Bitmap InterfaceImage_Gateways = null;

            LoadInterfaceImage(modProgram.InterfaceImagesPath + "displayautotexture.png", ref InterfaceImage_DisplayAutoTexture, ReturnResult);
            LoadInterfaceImage(modProgram.InterfaceImagesPath + "drawtileorientation.png", ref InterfaceImage_DrawTileOrientation, ReturnResult);
            LoadInterfaceImage(modProgram.InterfaceImagesPath + "save.png", ref InterfaceImage_QuickSave, ReturnResult);
            LoadInterfaceImage(modProgram.InterfaceImagesPath + "selection.png", ref InterfaceImage_Selection, ReturnResult);
            LoadInterfaceImage(modProgram.InterfaceImagesPath + "objectsselect.png", ref InterfaceImage_ObjectSelect, ReturnResult);
            LoadInterfaceImage(modProgram.InterfaceImagesPath + "selectioncopy.png", ref InterfaceImage_SelectionCopy, ReturnResult);
            LoadInterfaceImage(modProgram.InterfaceImagesPath + "selectionflipx.png", ref InterfaceImage_SelectionFlipX, ReturnResult);
            LoadInterfaceImage(modProgram.InterfaceImagesPath + "selectionrotateclockwise.png", ref InterfaceImage_SelectionRotateClockwise, ReturnResult);
            LoadInterfaceImage(modProgram.InterfaceImagesPath + "selectionrotateanticlockwise.png", ref InterfaceImage_SelectionRotateCounterClockwise, ReturnResult);
            LoadInterfaceImage(modProgram.InterfaceImagesPath + "selectionpaste.png", ref InterfaceImage_SelectionPaste, ReturnResult);
            LoadInterfaceImage(modProgram.InterfaceImagesPath + "selectionpasteoptions.png", ref InterfaceImage_SelectionPasteOptions, ReturnResult);
            LoadInterfaceImage(modProgram.InterfaceImagesPath + "gateways.png", ref InterfaceImage_Gateways, ReturnResult);

            Bitmap InterfaceImage_Problem = null;
            LoadInterfaceImage(modProgram.InterfaceImagesPath + "problem.png", ref InterfaceImage_Problem, ReturnResult);
            Bitmap InterfaceImage_Warning = null;
            LoadInterfaceImage(modProgram.InterfaceImagesPath + "warning.png", ref InterfaceImage_Warning, ReturnResult);
            modWarnings.WarningImages.ImageSize = new System.Drawing.Size(16, 16);
            if ( InterfaceImage_Problem != null )
            {
                modWarnings.WarningImages.Images.Add("problem", InterfaceImage_Problem);
            }
            if ( InterfaceImage_Warning != null )
            {
                modWarnings.WarningImages.Images.Add("warning", InterfaceImage_Warning);
            }

            tsbDrawAutotexture.Image = InterfaceImage_DisplayAutoTexture;
            tsbDrawTileOrientation.Image = InterfaceImage_DrawTileOrientation;
            tsbSave.Image = InterfaceImage_QuickSave;
            tsbSelection.Image = InterfaceImage_Selection;
            tsbSelectionObjects.Image = InterfaceImage_ObjectSelect;
            tsbSelectionCopy.Image = InterfaceImage_SelectionCopy;
            tsbSelectionFlipX.Image = InterfaceImage_SelectionFlipX;
            tsbSelectionRotateClockwise.Image = InterfaceImage_SelectionRotateClockwise;
            tsbSelectionRotateCounterClockwise.Image = InterfaceImage_SelectionRotateCounterClockwise;
            tsbSelectionPaste.Image = InterfaceImage_SelectionPaste;
            tsbSelectionPasteOptions.Image = InterfaceImage_SelectionPasteOptions;
            tsbGateways.Image = InterfaceImage_Gateways;
            btnTextureAnticlockwise.Image = InterfaceImage_SelectionRotateCounterClockwise;
            btnTextureClockwise.Image = InterfaceImage_SelectionRotateClockwise;
            btnTextureFlipX.Image = InterfaceImage_SelectionFlipX;

            return ReturnResult;
        }

        public void frmMain_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            bool ChangedPrompt = false;

            clsMap Map = default(clsMap);
            foreach ( clsMap tempLoopVar_Map in _LoadedMaps )
            {
                Map = tempLoopVar_Map;
                if ( Map.ChangedSinceSave )
                {
                    ChangedPrompt = true;
                    break;
                }
            }
            if ( ChangedPrompt )
            {
                frmQuit QuitPrompt = new frmQuit();
                DialogResult QuitResult = QuitPrompt.ShowDialog(modMain.frmMainInstance);
                switch ( QuitResult )
                {
                    case System.Windows.Forms.DialogResult.Yes:
                        while ( _LoadedMaps.Count > 0 )
                        {
                            clsMap RemoveMap = _LoadedMaps[0];
                            SetMainMap(RemoveMap);
                            if ( !RemoveMap.ClosePrompt() )
                            {
                                e.Cancel = true;
                                return;
                            }
                            RemoveMap.Deallocate();
                        }
                        break;
                    case System.Windows.Forms.DialogResult.No:
                        break;

                    case System.Windows.Forms.DialogResult.Cancel:
                        e.Cancel = true;
                        return;
                }
            }

            clsResult Result = modSettings.Settings_Write();
        }

#if !Mono
        public class clsSplashScreen
        {
            public frmSplash Form = new frmSplash();

            public clsSplashScreen()
            {
                Form.Icon = modProgram.ProgramIcon;
            }
        }

        private void ShowThreadedSplashScreen()
        {
            clsSplashScreen SplashScreen = new clsSplashScreen();

            SplashScreen.Form.Show();
            SplashScreen.Form.Activate();
            while ( !modProgram.ProgramInitializeFinished )
            {
                SplashScreen.Form.lblStatus.Text = InitializeStatus;
                Application.DoEvents();
                System.Threading.Thread.Sleep(200);
            }
            SplashScreen.Form.Close();
        }
#endif

        public string InitializeStatus = "";

        public void Initialize(object sender, EventArgs e)
        {
            if ( modProgram.ProgramInitialized )
            {
                Debugger.Break();
                return;
            }
            if ( !(MapView.IsGLInitialized && TextureView.IsGLInitialized) )
            {
                return;
            }

#if !Mono
            Hide();
            System.Threading.Thread SplashThread = new System.Threading.Thread(new System.Threading.ThreadStart(ShowThreadedSplashScreen));
            SplashThread.IsBackground = true;
            SplashThread.Start();
#endif

            modProgram.ProgramInitialized = true;

            modMain.InitializeDelay.Enabled = false;
            modMain.InitializeDelay.Tick -= Initialize;
            modMain.InitializeDelay.Dispose();
            modMain.InitializeDelay = null;

            modMain.InitializeResult.Add(LoadInterfaceImages());

            modTools.CreateTools();

            Matrix3DMath.MatrixSetToPY(modProgram.SunAngleMatrix, new Matrix3D.Angles.AnglePY(-22.5D * modMath.RadOf1Deg, 157.5D * modMath.RadOf1Deg));

            NewPlayerNum.Left = 112;
            NewPlayerNum.Top = 10;
            Panel1.Controls.Add(NewPlayerNum);

            ObjectPlayerNum.Left = 72;
            ObjectPlayerNum.Top = 60;
            ObjectPlayerNum.Target = new clsMap.clsUnitGroupContainer();
            ObjectPlayerNum.Target.Changed += tabPlayerNum_SelectedIndexChanged;
            Panel14.Controls.Add(ObjectPlayerNum);

            ctrlTextureBrush = new ctrlBrush(modProgram.TextureBrush);
            pnlTextureBrush.Controls.Add(ctrlTextureBrush);

            ctrlTerrainBrush = new ctrlBrush(modProgram.TerrainBrush);
            pnlTerrainBrush.Controls.Add(ctrlTerrainBrush);

            ctrlCliffRemoveBrush = new ctrlBrush(modProgram.CliffBrush);
            pnlCliffRemoveBrush.Controls.Add(ctrlCliffRemoveBrush);

            ctrlHeightBrush = new ctrlBrush(modProgram.HeightBrush);
            pnlHeightSetBrush.Controls.Add(ctrlHeightBrush);

            VBMath.Randomize();

            CreateTileTypes();

            for ( int i = 0; i <= 15; i++ )
            {
                modProgram.PlayerColour[i] = new modProgram.clsPlayer();
            }
            modProgram.PlayerColour[0].Colour.Red = 0.0F;
            modProgram.PlayerColour[0].Colour.Green = 96.0F / 255.0F;
            modProgram.PlayerColour[0].Colour.Blue = 0.0F;
            modProgram.PlayerColour[1].Colour.Red = 160.0F / 255.0F;
            modProgram.PlayerColour[1].Colour.Green = 112.0F / 255.0F;
            modProgram.PlayerColour[1].Colour.Blue = 0.0F;
            modProgram.PlayerColour[2].Colour.Red = 128.0F / 255.0F;
            modProgram.PlayerColour[2].Colour.Green = 128.0F / 255.0F;
            modProgram.PlayerColour[2].Colour.Blue = 128.0F / 255.0F;
            modProgram.PlayerColour[3].Colour.Red = 0.0F;
            modProgram.PlayerColour[3].Colour.Green = 0.0F;
            modProgram.PlayerColour[3].Colour.Blue = 0.0F;
            modProgram.PlayerColour[4].Colour.Red = 128.0F / 255.0F;
            modProgram.PlayerColour[4].Colour.Green = 0.0F;
            modProgram.PlayerColour[4].Colour.Blue = 0.0F;
            modProgram.PlayerColour[5].Colour.Red = 32.0F / 255.0F;
            modProgram.PlayerColour[5].Colour.Green = 48.0F / 255.0F;
            modProgram.PlayerColour[5].Colour.Blue = 96.0F / 255.0F;
            modProgram.PlayerColour[6].Colour.Red = 144.0F / 255.0F;
            modProgram.PlayerColour[6].Colour.Green = 0.0F;
            modProgram.PlayerColour[6].Colour.Blue = 112 / 255.0F;
            modProgram.PlayerColour[7].Colour.Red = 0.0F;
            modProgram.PlayerColour[7].Colour.Green = 128.0F / 255.0F;
            modProgram.PlayerColour[7].Colour.Blue = 128.0F / 255.0F;
            modProgram.PlayerColour[8].Colour.Red = 128.0F / 255.0F;
            modProgram.PlayerColour[8].Colour.Green = 192.0F / 255.0F;
            modProgram.PlayerColour[8].Colour.Blue = 0.0F;
            modProgram.PlayerColour[9].Colour.Red = 176.0F / 255.0F;
            modProgram.PlayerColour[9].Colour.Green = 112.0F / 255.0F;
            modProgram.PlayerColour[9].Colour.Blue = 112.0F / 255.0F;
            modProgram.PlayerColour[10].Colour.Red = 224.0F / 255.0F;
            modProgram.PlayerColour[10].Colour.Green = 224.0F / 255.0F;
            modProgram.PlayerColour[10].Colour.Blue = 224.0F / 255.0F;
            modProgram.PlayerColour[11].Colour.Red = 32.0F / 255.0F;
            modProgram.PlayerColour[11].Colour.Green = 32.0F / 255.0F;
            modProgram.PlayerColour[11].Colour.Blue = 255.0F / 255.0F;
            modProgram.PlayerColour[12].Colour.Red = 0.0F;
            modProgram.PlayerColour[12].Colour.Green = 160.0F / 255.0F;
            modProgram.PlayerColour[12].Colour.Blue = 0.0F;
            modProgram.PlayerColour[13].Colour.Red = 64.0F / 255.0F;
            modProgram.PlayerColour[13].Colour.Green = 0.0F;
            modProgram.PlayerColour[13].Colour.Blue = 0.0F;
            modProgram.PlayerColour[14].Colour.Red = 16.0F / 255.0F;
            modProgram.PlayerColour[14].Colour.Green = 0.0F;
            modProgram.PlayerColour[14].Colour.Blue = 64.0F / 255.0F;
            modProgram.PlayerColour[15].Colour.Red = 64.0F / 255.0F;
            modProgram.PlayerColour[15].Colour.Green = 96.0F / 255.0F;
            modProgram.PlayerColour[15].Colour.Blue = 0.0F;
            for ( int i = 0; i <= 15; i++ )
            {
                modProgram.PlayerColour[i].CalcMinimapColour();
            }

            modProgram.MinimapFeatureColour.Red = 0.5F;
            modProgram.MinimapFeatureColour.Green = 0.5F;
            modProgram.MinimapFeatureColour.Blue = 0.5F;

            modSettings.UpdateSettings(modSettings.InitializeSettings);
            modSettings.InitializeSettings = null;

            if ( modSettings.Settings.DirectoriesPrompt )
            {
                modMain.frmOptionsInstance = new frmOptions();
                modMain.frmOptionsInstance.FormClosing += modMain.frmOptionsInstance.frmOptions_FormClosing;
                if ( modMain.frmOptionsInstance.ShowDialog() == System.Windows.Forms.DialogResult.Cancel )
                {
                    ProjectData.EndApp();
                }
            }

            int TilesetNum = System.Convert.ToInt32(modSettings.Settings.get_Value(modSettings.Setting_DefaultTilesetsPathNum));
            modLists.SimpleList<string> TilesetsList = (modLists.SimpleList<string>)(modSettings.Settings.get_Value(modSettings.Setting_TilesetDirectories));
            if ( TilesetNum >= 0 & TilesetNum < TilesetsList.Count )
            {
                string TilesetsPath = TilesetsList[TilesetNum];
                if ( TilesetsPath != null && TilesetsPath != "" )
                {
                    InitializeStatus = "Loading tilesets";
                    modMain.InitializeResult.Add(modProgram.LoadTilesets(modProgram.EndWithPathSeperator(TilesetsPath)));
                    InitializeStatus = "";
                }
            }

            cboTileset_Update(-1);

            modMain.InitializeResult.Add(NoTile_Texture_Load());
            cboTileType_Update();

            modProgram.CreateTemplateDroidTypes(); //do before loading data

            modProgram.ObjectData = new clsObjectData();
            int ObjectDataNum = System.Convert.ToInt32(modSettings.Settings.get_Value(modSettings.Setting_DefaultObjectDataPathNum));
            modLists.SimpleList<string> ObjectDataList = (modLists.SimpleList<string>)(modSettings.Settings.get_Value(modSettings.Setting_ObjectDataDirectories));
            if ( ObjectDataNum >= 0 & ObjectDataNum < TilesetsList.Count )
            {
                string ObjectDataPath = ObjectDataList[ObjectDataNum];
                if ( ObjectDataPath != null && ObjectDataPath != "" )
                {
                    InitializeStatus = "Loading object data";
                    modMain.InitializeResult.Add(modProgram.ObjectData.LoadDirectory(ObjectDataPath));
                    InitializeStatus = "";
                }
            }

            modGenerator.CreateGeneratorTilesets();
            modPainters.CreatePainterArizona();
            modPainters.CreatePainterUrban();
            modPainters.CreatePainterRockies();

            Components_Update();

            MapView.Dock = DockStyle.Fill;
            pnlView.Controls.Add(MapView);

            modProgram.VisionRadius_2E = 10;
            modProgram.VisionRadius_2E_Changed();

            HeightSetPalette[0] = (byte)0;
            HeightSetPalette[1] = (byte)85;
            HeightSetPalette[2] = (byte)170;
            HeightSetPalette[3] = (byte)255;
            HeightSetPalette[4] = (byte)64;
            HeightSetPalette[5] = (byte)128;
            HeightSetPalette[6] = (byte)192;
            HeightSetPalette[7] = (byte)255;
            for ( int A = 0; A <= 7; A++ )
            {
                tabHeightSetL.TabPages[A].Text = modIO.InvariantToString_byte(HeightSetPalette[A]);
                tabHeightSetR.TabPages[A].Text = modIO.InvariantToString_byte(HeightSetPalette[A]);
            }
            tabHeightSetL.SelectedIndex = 1;
            tabHeightSetR.SelectedIndex = 0;
            tabHeightSetL_SelectedIndexChanged(null, null);
            tabHeightSetR_SelectedIndexChanged(null, null);

            if ( modProgram.CommandLinePaths.Count >= 1 )
            {
                string Path = "";
                clsResult LoadResult = new clsResult("Loading startup command-line maps");
                foreach ( string tempLoopVar_Path in modProgram.CommandLinePaths )
                {
                    Path = tempLoopVar_Path;
                    LoadResult.Take(LoadMap(Path));
                }
                modProgram.ShowWarnings(LoadResult);
            }

            TextureView.Dock = DockStyle.Fill;
            TableLayoutPanel6.Controls.Add(TextureView, 0, 1);

            MainMapAfterChanged();

            MapView.DrawView_SetEnabled(true);
            TextureView.DrawView_SetEnabled(true);

            WindowState = FormWindowState.Maximized;
#if !Mono
            Show();
#endif
            Activate();

            tmrKey.Enabled = true;
            tmrTool.Enabled = true;

            modProgram.ShowWarnings(modMain.InitializeResult);

            modProgram.ProgramInitializeFinished = true;
        }

        public void Me_LostFocus(System.Object eventSender, System.EventArgs eventArgs)
        {
            modProgram.ViewKeyDown_Clear();
        }

        private void tmrKey_Tick(System.Object sender, System.EventArgs e)
        {
            if ( !modProgram.ProgramInitialized )
            {
                return;
            }
            clsMap Map = MainMap;
            if ( Map == null )
            {
                return;
            }

            double Rate = 0;
            double Zoom = 0;
            double Move = 0;
            double Roll = 0;
            double Pan = 0;
            double OrbitRate = 0;

            if ( modControls.KeyboardProfile.Active(modControls.Control_Fast) )
            {
                if ( modControls.KeyboardProfile.Active(modControls.Control_Slow) )
                {
                    Rate = 8.0D;
                }
                else
                {
                    Rate = 4.0D;
                }
            }
            else if ( modControls.KeyboardProfile.Active(modControls.Control_Slow) )
            {
                Rate = 0.25D;
            }
            else
            {
                Rate = 1.0D;
            }

            Zoom = tmrKey.Interval * 0.002D;
            Move = tmrKey.Interval * Rate / 2048.0D;
            Roll = 5.0D * modMath.RadOf1Deg;
            Pan = 1.0D / 16.0D;
            OrbitRate = 1.0D / 32.0D;

            Map.ViewInfo.TimedActions(Zoom, Move, Pan, Roll, OrbitRate);

            if ( Map.CheckMessages() )
            {
                View_DrawViewLater();
            }
        }

        public void Load_Map_Prompt()
        {
            OpenFileDialog Dialog = new OpenFileDialog();

            Dialog.InitialDirectory = modSettings.Settings.OpenPath;
            Dialog.FileName = "";
            Dialog.Filter = "Warzone Map Files (*.fmap, *.fme, *.wz, *.gam, *.lnd)|*.fmap;*.fme;*.wz;*.gam;*.lnd|All Files (*.*)|*.*";
            Dialog.Multiselect = true;
            if ( Dialog.ShowDialog(this) != System.Windows.Forms.DialogResult.OK )
            {
                return;
            }
            modSettings.Settings.OpenPath = System.IO.Path.GetDirectoryName(Dialog.FileName);
            string FileName = "";
            clsResult Results = new clsResult("Loading maps");
            foreach ( string tempLoopVar_FileName in Dialog.FileNames )
            {
                FileName = tempLoopVar_FileName;
                Results.Take(LoadMap(FileName));
            }
            modProgram.ShowWarnings(Results);
        }

        public void Load_Heightmap_Prompt()
        {
            OpenFileDialog Dialog = new OpenFileDialog();

            Dialog.InitialDirectory = modSettings.Settings.OpenPath;
            Dialog.FileName = "";
            Dialog.Filter = "Image Files (*.bmp, *.png)|*.bmp;*.png|All Files (*.*)|*.*";
            if ( Dialog.ShowDialog(this) != System.Windows.Forms.DialogResult.OK )
            {
                return;
            }
            modSettings.Settings.OpenPath = System.IO.Path.GetDirectoryName(Dialog.FileName);

            Bitmap HeightmapBitmap = null;
            modProgram.sResult Result = modBitmap.LoadBitmap(Dialog.FileName, ref HeightmapBitmap);
            if ( !Result.Success )
            {
                MessageBox.Show("Failed to load image: " + Result.Problem);
                return;
            }

            clsMap Map = MainMap;
            clsMap ApplyToMap = null;
            if ( Map == null )
            {
            }
            else if ( Interaction.MsgBox("Apply heightmap to the current map?", MsgBoxStyle.YesNo, null) == MsgBoxResult.Yes )
            {
                ApplyToMap = Map;
            }
            if ( ApplyToMap == null )
            {
                ApplyToMap = new clsMap(new modMath.sXY_int(HeightmapBitmap.Width - 1, HeightmapBitmap.Height - 1));
            }

            int X = 0;
            int Y = 0;
            Color PixelColor = new Color();

            for ( Y = 0; Y <= Math.Min(HeightmapBitmap.Height - 1, ApplyToMap.Terrain.TileSize.Y); Y++ )
            {
                for ( X = 0; X <= Math.Min(HeightmapBitmap.Width - 1, ApplyToMap.Terrain.TileSize.X); X++ )
                {
                    PixelColor = HeightmapBitmap.GetPixel(X, Y);
                    ApplyToMap.Terrain.Vertices[X, Y].Height =
                        (byte)Math.Min(Math.Round(System.Convert.ToDouble(((PixelColor.R) + PixelColor.G + PixelColor.B) / 3.0D)), byte.MaxValue);
                }
            }

            if ( ApplyToMap == Map )
            {
                ApplyToMap.SectorTerrainUndoChanges.SetAllChanged();
                ApplyToMap.SectorUnitHeightsChanges.SetAllChanged();
                ApplyToMap.SectorGraphicsChanges.SetAllChanged();
                ApplyToMap.Update();
                ApplyToMap.UndoStepCreate("Apply heightmap");
            }
            else
            {
                NewMainMap(ApplyToMap);
                ApplyToMap.Update();
            }

            View_DrawViewLater();
        }

        public void Load_TTP_Prompt()
        {
            OpenFileDialog Dialog = new OpenFileDialog();

            clsMap Map = MainMap;

            if ( Map == null )
            {
                return;
            }

            Dialog.InitialDirectory = modSettings.Settings.OpenPath;
            Dialog.FileName = "";
            Dialog.Filter = "TTP Files (*.ttp)|*.ttp|All Files (*.*)|*.*";
            if ( !(Dialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK) )
            {
                return;
            }
            modSettings.Settings.OpenPath = System.IO.Path.GetDirectoryName(Dialog.FileName);
            modProgram.sResult Result = Map.Load_TTP(Dialog.FileName);
            if ( Result.Success )
            {
                TextureView.DrawViewLater();
            }
            else
            {
                MessageBox.Show("Importing tile types failed: " + Result.Problem);
            }
        }

        public void cboTileset_Update(int NewSelectedIndex)
        {
            int A = 0;

            cboTileset.Items.Clear();
            for ( A = 0; A <= modProgram.Tilesets.Count - 1; A++ )
            {
                cboTileset.Items.Add(modProgram.Tilesets[A].Name);
            }
            cboTileset.SelectedIndex = NewSelectedIndex;
        }

        public void MainMapTilesetChanged()
        {
            int A = 0;
            clsMap Map = MainMap;

            if ( Map == null )
            {
                cboTileset.SelectedIndex = -1;
                return;
            }

            for ( A = 0; A <= modProgram.Tilesets.Count - 1; A++ )
            {
                if ( modProgram.Tilesets[A] == Map.Tileset )
                {
                    break;
                }
            }
            if ( A == modProgram.Tilesets.Count )
            {
                cboTileset.SelectedIndex = -1;
            }
            else
            {
                cboTileset.SelectedIndex = A;
            }
        }

        public void cboTileset_SelectedIndexChanged(System.Object sender, System.EventArgs e)
        {
            clsTileset NewTileset = default(clsTileset);
            clsMap Map = MainMap;

            if ( Map == null )
            {
                return;
            }

            if ( cboTileset.SelectedIndex < 0 )
            {
                NewTileset = null;
            }
            else
            {
                NewTileset = modProgram.Tilesets[cboTileset.SelectedIndex];
            }
            if ( NewTileset != Map.Tileset )
            {
                Map.Tileset = NewTileset;
                if ( Map.Tileset != null )
                {
                    modProgram.SelectedTextureNum = Math.Min(0, Map.Tileset.TileCount - 1);
                }
                Map.TileType_Reset();

                Map.SetPainterToDefaults();
                PainterTerrains_Refresh(-1, -1);

                Map.SectorGraphicsChanges.SetAllChanged();
                Map.Update();
                Map.MinimapMakeLater();
                View_DrawViewLater();
                TextureView.ScrollUpdate();
                TextureView.DrawViewLater();
            }
        }

        public void PainterTerrains_Refresh(int Terrain_NewSelectedIndex, int Road_NewSelectedIndex)
        {
            lstAutoTexture.Items.Clear();
            lstAutoRoad.Items.Clear();

            clsMap Map = MainMap;
            if ( Map == null )
            {
                return;
            }

            int A = 0;
            for ( A = 0; A <= Map.Painter.TerrainCount - 1; A++ )
            {
                lstAutoTexture.Items.Add(Map.Painter.Terrains[A].Name);
            }
            for ( A = 0; A <= Map.Painter.RoadCount - 1; A++ )
            {
                lstAutoRoad.Items.Add(Map.Painter.Roads[A].Name);
            }
            lstAutoTexture.SelectedIndex = Terrain_NewSelectedIndex;
            lstAutoRoad.SelectedIndex = Road_NewSelectedIndex;
        }

        public void TabControl_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if ( TabControl.SelectedTab == tpTextures )
            {
                modTools.Tool = modTools.Tools.TextureBrush;
                TextureView.DrawViewLater();
            }
            else if ( TabControl.SelectedTab == tpHeight )
            {
                if ( rdoHeightSet.Checked )
                {
                    modTools.Tool = modTools.Tools.HeightSetBrush;
                }
                else if ( rdoHeightSmooth.Checked )
                {
                    modTools.Tool = modTools.Tools.HeightSmoothBrush;
                }
                else if ( rdoHeightChange.Checked )
                {
                    modTools.Tool = modTools.Tools.HeightChangeBrush;
                }
            }
            else if ( TabControl.SelectedTab == tpAutoTexture )
            {
                if ( rdoAutoTexturePlace.Checked )
                {
                    modTools.Tool = modTools.Tools.TerrainBrush;
                }
                else if ( rdoAutoRoadPlace.Checked )
                {
                    modTools.Tool = modTools.Tools.RoadPlace;
                }
                else if ( rdoCliffTriBrush.Checked )
                {
                    modTools.Tool = modTools.Tools.CliffTriangle;
                }
                else if ( rdoAutoCliffBrush.Checked )
                {
                    modTools.Tool = modTools.Tools.CliffBrush;
                }
                else if ( rdoAutoCliffRemove.Checked )
                {
                    modTools.Tool = modTools.Tools.CliffRemove;
                }
                else if ( rdoAutoTextureFill.Checked )
                {
                    modTools.Tool = modTools.Tools.TerrainFill;
                }
                else if ( rdoAutoRoadLine.Checked )
                {
                    modTools.Tool = modTools.Tools.RoadLines;
                }
                else if ( rdoRoadRemove.Checked )
                {
                    modTools.Tool = modTools.Tools.RoadRemove;
                }
                else
                {
                    modTools.Tool = modTools.Tools.ObjectSelect;
                }
            }
            else if ( TabControl.SelectedTab == tpObjects )
            {
                ObjectsUpdate();
                if ( rdoObjectPlace.Checked )
                {
                    modTools.Tool = modTools.Tools.ObjectPlace;
                }
                else if ( rdoObjectLines.Checked )
                {
                    modTools.Tool = modTools.Tools.ObjectLines;
                }
                else
                {
                    modTools.Tool = modTools.Tools.ObjectSelect;
                }
            }
            else
            {
                modTools.Tool = modTools.Tools.ObjectSelect;
            }
        }

        public void rdoHeightSet_CheckedChanged(System.Object sender, System.EventArgs e)
        {
            if ( rdoHeightSet.Checked )
            {
                rdoHeightSmooth.Checked = false;
                rdoHeightChange.Checked = false;

                modTools.Tool = modTools.Tools.HeightSetBrush;
            }
        }

        public void rdoHeightChange_CheckedChanged(System.Object sender, System.EventArgs e)
        {
            if ( rdoHeightChange.Checked )
            {
                rdoHeightSet.Checked = false;
                rdoHeightSmooth.Checked = false;

                modTools.Tool = modTools.Tools.HeightChangeBrush;
            }
        }

        public void rdoHeightSmooth_CheckedChanged(System.Object sender, System.EventArgs e)
        {
            if ( rdoHeightSmooth.Checked )
            {
                rdoHeightSet.Checked = false;
                rdoHeightChange.Checked = false;

                modTools.Tool = modTools.Tools.HeightSmoothBrush;
            }
        }

        private void tmrTool_Tick(System.Object sender, System.EventArgs e)
        {
            if ( !modProgram.ProgramInitialized )
            {
                return;
            }

            clsMap Map = MainMap;

            if ( Map == null )
            {
                return;
            }

            Map.ViewInfo.TimedTools();
        }

        public void btnResize_Click(System.Object sender, System.EventArgs e)
        {
            modMath.sXY_int NewSize = new modMath.sXY_int();
            modMath.sXY_int Offset = new modMath.sXY_int();
            double Max = modProgram.MapMaxSize;

            if ( !modIO.InvariantParse_int(txtSizeX.Text, ref NewSize.X) )
            {
                return;
            }
            if ( !modIO.InvariantParse_int(txtSizeY.Text, ref NewSize.Y) )
            {
                return;
            }
            if ( !modIO.InvariantParse_int(txtOffsetX.Text, ref Offset.X) )
            {
                return;
            }
            if ( !modIO.InvariantParse_int(txtOffsetY.Text, ref Offset.Y) )
            {
                return;
            }

            Map_Resize(Offset, NewSize);
        }

        public void Map_Resize(modMath.sXY_int Offset, modMath.sXY_int NewSize)
        {
            clsMap Map = MainMap;

            if ( Map == null )
            {
                return;
            }

            if ( Interaction.MsgBox("Resizing can\'t be undone. Continue?", (Microsoft.VisualBasic.MsgBoxStyle)(MsgBoxStyle.OkCancel | MsgBoxStyle.Question), "") !=
                 MsgBoxResult.Ok )
            {
                return;
            }

            if ( NewSize.X < 1 | NewSize.Y < 1 )
            {
                Interaction.MsgBox("Map sizes must be at least 1.", MsgBoxStyle.OkOnly, "");
                return;
            }
            if ( NewSize.X > modProgram.WZMapMaxSize | NewSize.Y > modProgram.WZMapMaxSize )
            {
                if (
                    Interaction.MsgBox("Warzone doesn\'t support map sizes above " + System.Convert.ToString(modProgram.WZMapMaxSize) + ". Continue anyway?",
                        MsgBoxStyle.YesNo, "") != MsgBoxResult.Yes )
                {
                    return;
                }
            }

            Map.TerrainResize(Offset, NewSize);

            Resize_Update();
            ScriptMarkerLists_Update();

            Map.SectorGraphicsChanges.SetAllChanged();

            Map.Update();

            View_DrawViewLater();
        }

        public void Resize_Update()
        {
            clsMap Map = MainMap;

            if ( Map == null )
            {
                txtSizeX.Text = "";
                txtSizeY.Text = "";
                txtOffsetX.Text = "";
                txtOffsetY.Text = "";
            }
            else
            {
                txtSizeX.Text = modIO.InvariantToString_int(Map.Terrain.TileSize.X);
                txtSizeY.Text = modIO.InvariantToString_int(Map.Terrain.TileSize.Y);
                txtOffsetX.Text = "0";
                txtOffsetY.Text = "0";
            }
        }

        public void CloseToolStripMenuItem_Click(System.Object sender, System.EventArgs e)
        {
            Close();
        }

        public void OpenToolStripMenuItem_Click(System.Object sender, System.EventArgs e)
        {
            Load_Map_Prompt();
        }

        public void LNDToolStripMenuItem1_Click(System.Object sender, System.EventArgs e)
        {
            Save_LND_Prompt();
        }

        public void Save_LND_Prompt()
        {
            clsMap Map = MainMap;

            if ( Map == null )
            {
                return;
            }

            SaveFileDialog Dialog = new SaveFileDialog();

            Dialog.InitialDirectory = modSettings.Settings.SavePath;
            Dialog.FileName = "";
            Dialog.Filter = "Editworld Files (*.lnd)|*.lnd";
            if ( Dialog.ShowDialog(this) != System.Windows.Forms.DialogResult.OK )
            {
                return;
            }
            modSettings.Settings.SavePath = System.IO.Path.GetDirectoryName(Dialog.FileName);

            clsResult Result = Map.Write_LND(Dialog.FileName, true);

            modProgram.ShowWarnings(Result);
        }

        private void Save_FME_Prompt()
        {
            clsMap Map = MainMap;

            if ( Map == null )
            {
                return;
            }

            SaveFileDialog Dialog = new SaveFileDialog();

            Dialog.InitialDirectory = modSettings.Settings.SavePath;
            Dialog.FileName = "";
            Dialog.Filter = modProgram.ProgramName + " FME Map Files (*.fme)|*.fme";
            if ( Dialog.ShowDialog(this) != System.Windows.Forms.DialogResult.OK )
            {
                return;
            }
            modSettings.Settings.SavePath = System.IO.Path.GetDirectoryName(Dialog.FileName);
            string strScavenger = Interaction.InputBox("Enter the player number for scavenger units:", "", "", -1, -1);
            byte ScavengerNum = 0;
            if ( !modIO.InvariantParse_byte(strScavenger, ref ScavengerNum) )
            {
                MessageBox.Show("Unable to save FME: entered scavenger number is not a number.");
                return;
            }
            ScavengerNum = Math.Min(ScavengerNum, (byte)10);
            modProgram.sResult Result = new modProgram.sResult();
            Result = Map.Write_FME(Dialog.FileName, true, ScavengerNum);
            if ( !Result.Success )
            {
                MessageBox.Show("Unable to save FME: " + Result.Problem);
            }
        }

        public void Save_Minimap_Prompt()
        {
            clsMap Map = MainMap;

            if ( Map == null )
            {
                return;
            }

            SaveFileDialog Dialog = new SaveFileDialog();

            Dialog.InitialDirectory = modSettings.Settings.SavePath;
            Dialog.FileName = "";
            Dialog.Filter = "Bitmap File (*.bmp)|*.bmp";
            if ( Dialog.ShowDialog(this) != System.Windows.Forms.DialogResult.OK )
            {
                return;
            }
            modSettings.Settings.SavePath = System.IO.Path.GetDirectoryName(Dialog.FileName);
            modProgram.sResult Result = new modProgram.sResult();
            Result = Map.Write_MinimapFile(Dialog.FileName, true);
            if ( !Result.Success )
            {
                MessageBox.Show("There was a problem saving the minimap bitmap: " + Result.Problem);
            }
        }

        public void Save_Heightmap_Prompt()
        {
            clsMap Map = MainMap;

            if ( Map == null )
            {
                return;
            }

            SaveFileDialog Dialog = new SaveFileDialog();

            Dialog.InitialDirectory = modSettings.Settings.SavePath;
            Dialog.FileName = "";
            Dialog.Filter = "Bitmap File (*.bmp)|*.bmp";
            if ( Dialog.ShowDialog(this) != System.Windows.Forms.DialogResult.OK )
            {
                return;
            }
            modSettings.Settings.SavePath = System.IO.Path.GetDirectoryName(Dialog.FileName);
            modProgram.sResult Result = new modProgram.sResult();
            Result = Map.Write_Heightmap(Dialog.FileName, true);
            if ( !Result.Success )
            {
                MessageBox.Show("There was a problem saving the heightmap bitmap: " + Result.Problem);
            }
        }

        public void PromptSave_TTP()
        {
            clsMap Map = MainMap;

            if ( Map == null )
            {
                return;
            }

            SaveFileDialog Dialog = new SaveFileDialog();

            Dialog.InitialDirectory = modSettings.Settings.SavePath;
            Dialog.FileName = "";
            Dialog.Filter = "TTP Files (*.ttp)|*.ttp";
            if ( Dialog.ShowDialog(this) != System.Windows.Forms.DialogResult.OK )
            {
                return;
            }
            modSettings.Settings.SavePath = System.IO.Path.GetDirectoryName(Dialog.FileName);
            modProgram.sResult Result = new modProgram.sResult();
            Result = Map.Write_TTP(Dialog.FileName, true);
            if ( !Result.Success )
            {
                MessageBox.Show("There was a problem saving the tile types: " + Result.Problem);
            }
        }

        public void New_Prompt()
        {
            NewMap();
        }

        public void NewMap()
        {
            clsMap NewMap = new clsMap(new modMath.sXY_int(64, 64));
            NewMainMap(NewMap);

            NewMap.RandomizeTileOrientations();
            NewMap.Update();
            NewMap.UndoClear();
        }

        public void rdoAutoCliffRemove_Click(System.Object sender, System.EventArgs e)
        {
            modTools.Tool = modTools.Tools.CliffRemove;
        }

        public void rdoAutoCliffBrush_Click(System.Object sender, System.EventArgs e)
        {
            modTools.Tool = modTools.Tools.CliffBrush;
        }

        public void MinimapBMPToolStripMenuItem_Click(System.Object sender, System.EventArgs e)
        {
            Save_Minimap_Prompt();
        }

        public void FMapToolStripMenuItem_Click(System.Object sender, System.EventArgs e)
        {
            clsMap Map = MainMap;

            if ( Map == null )
            {
                return;
            }

            if ( Map.Save_FMap_Prompt() )
            {
                modMain.frmMainInstance.tsbSave.Enabled = false;
                TitleTextUpdate();
            }
        }

        public void menuSaveFMapQuick_Click(System.Object sender, System.EventArgs e)
        {
            QuickSave();
        }

        public void MapWZToolStripMenuItem_Click(System.Object sender, System.EventArgs e)
        {
            clsMap Map = MainMap;

            if ( Map == null )
            {
                return;
            }

            if ( Map.CompileScreen == null )
            {
                frmCompile NewCompile = frmCompile.Create(Map);
                NewCompile.Show();
            }
            else
            {
                Map.CompileScreen.Activate();
            }
        }

        public void rdoAutoTextureFill_CheckedChanged(System.Object sender, System.EventArgs e)
        {
            modTools.Tool = modTools.Tools.TerrainFill;
        }

        public void btnHeightOffsetSelection_Click(System.Object sender, System.EventArgs e)
        {
            clsMap Map = MainMap;

            if ( Map == null )
            {
                return;
            }

            if ( Map.Selected_Area_VertexA == null || Map.Selected_Area_VertexB == null )
            {
                return;
            }

            int X = 0;
            int Y = 0;
            double Offset = 0;
            modMath.sXY_int StartXY = new modMath.sXY_int();
            modMath.sXY_int FinishXY = new modMath.sXY_int();
            modMath.sXY_int Pos = new modMath.sXY_int();

            if ( !modIO.InvariantParse_dbl(txtHeightOffset.Text, ref Offset) )
            {
                return;
            }

            modMath.ReorderXY(Map.Selected_Area_VertexA.XY, Map.Selected_Area_VertexB.XY, StartXY, FinishXY);
            for ( Y = StartXY.Y; Y <= FinishXY.Y; Y++ )
            {
                for ( X = StartXY.X; X <= FinishXY.X; X++ )
                {
                    Map.Terrain.Vertices[X, Y].Height =
                        (byte)(Math.Round(modMath.Clamp_dbl(System.Convert.ToDouble(Map.Terrain.Vertices[X, Y].Height + Offset), byte.MinValue, byte.MaxValue)));
                    Pos.X = X;
                    Pos.Y = Y;
                    Map.SectorGraphicsChanges.VertexAndNormalsChanged(Pos);
                    Map.SectorUnitHeightsChanges.VertexChanged(Pos);
                    Map.SectorTerrainUndoChanges.VertexChanged(Pos);
                }
            }

            Map.Update();

            Map.UndoStepCreate("Selection Heights Offset");

            View_DrawViewLater();
        }

        public void rdoAutoTexturePlace_Click(System.Object sender, System.EventArgs e)
        {
            modTools.Tool = modTools.Tools.TerrainBrush;
        }

        public void rdoAutoRoadPlace_Click(System.Object sender, System.EventArgs e)
        {
            modTools.Tool = modTools.Tools.RoadPlace;
        }

        public void rdoAutoRoadLine_Click(System.Object sender, System.EventArgs e)
        {
            modTools.Tool = modTools.Tools.RoadLines;
            clsMap Map = MainMap;
            if ( Map != null )
            {
                Map.Selected_Tile_A = null;
                Map.Selected_Tile_B = null;
            }
        }

        public void rdoRoadRemove_Click(System.Object sender, System.EventArgs e)
        {
            modTools.Tool = modTools.Tools.RoadRemove;
        }

        public void btnAutoRoadRemove_Click(System.Object sender, System.EventArgs e)
        {
            lstAutoRoad.SelectedIndex = -1;
        }

        public void btnAutoTextureRemove_Click(System.Object sender, System.EventArgs e)
        {
            lstAutoTexture.SelectedIndex = -1;
        }

        public void NewMapToolStripMenuItem_Click(System.Object sender, System.EventArgs e)
        {
            New_Prompt();
        }

        public void ToolStripMenuItem3_Click(System.Object sender, System.EventArgs e)
        {
            Save_Heightmap_Prompt();
        }

        public void Components_Update()
        {
            if ( modProgram.ObjectData == null )
            {
                return;
            }

            clsBody Body = default(clsBody);
            clsPropulsion Propulsion = default(clsPropulsion);
            clsTurret Turret = default(clsTurret);
            string Text = "";
            string TypeName = "";
            int ListPosition = 0;

            cboDroidBody.Items.Clear();
            cboBody_Objects = new clsBody[modProgram.ObjectData.Bodies.Count];
            foreach ( clsBody tempLoopVar_Body in modProgram.ObjectData.Bodies )
            {
                Body = tempLoopVar_Body;
                if ( Body.Designable || (!cbxDesignableOnly.Checked) )
                {
                    ListPosition = cboDroidBody.Items.Add("(" + Body.Name + ") " + Body.Code);
                    cboBody_Objects[ListPosition] = Body;
                }
            }
            Array.Resize(ref cboBody_Objects, cboDroidBody.Items.Count);

            cboDroidPropulsion.Items.Clear();
            cboPropulsion_Objects = new clsPropulsion[modProgram.ObjectData.Propulsions.Count];
            foreach ( clsPropulsion tempLoopVar_Propulsion in modProgram.ObjectData.Propulsions )
            {
                Propulsion = tempLoopVar_Propulsion;
                if ( Propulsion.Designable || (!cbxDesignableOnly.Checked) )
                {
                    ListPosition = cboDroidPropulsion.Items.Add("(" + Propulsion.Name + ") " + Propulsion.Code);
                    cboPropulsion_Objects[ListPosition] = Propulsion;
                }
            }
            Array.Resize(ref cboPropulsion_Objects, cboDroidPropulsion.Items.Count);

            cboDroidTurret1.Items.Clear();
            cboDroidTurret2.Items.Clear();
            cboDroidTurret3.Items.Clear();
            cboTurret_Objects = new clsTurret[modProgram.ObjectData.Turrets.Count];
            foreach ( clsTurret tempLoopVar_Turret in modProgram.ObjectData.Turrets )
            {
                Turret = tempLoopVar_Turret;
                if ( Turret.Designable || (!cbxDesignableOnly.Checked) )
                {
                    TypeName = null;
                    Turret.GetTurretTypeName(ref TypeName);
                    Text = "(" + TypeName + " - " + Turret.Name + ") " + Turret.Code;
                    ListPosition = cboDroidTurret1.Items.Add(Text);
                    cboDroidTurret2.Items.Add(Text);
                    cboDroidTurret3.Items.Add(Text);
                    cboTurret_Objects[ListPosition] = Turret;
                }
            }
            Array.Resize(ref cboTurret_Objects, cboDroidTurret1.Items.Count);

            cboDroidType.Items.Clear();
            for ( int A = 0; A <= modProgram.TemplateDroidTypeCount - 1; A++ )
            {
                cboDroidType.Items.Add(modProgram.TemplateDroidTypes[A].Name);
            }
        }

        private void ObjectListFill<ObjectType>(modLists.SimpleList<ObjectType> objects, DataGridView gridView) where ObjectType : clsUnitType
        {
            modLists.SimpleList<ObjectType> filtered = default(modLists.SimpleList<ObjectType>);
            string searchText = txtObjectFind.Text;
            bool doSearch = default(bool);
            if ( searchText == null )
            {
                doSearch = false;
            }
            else if ( string.IsNullOrEmpty(searchText) )
            {
                doSearch = false;
            }
            else
            {
                doSearch = true;
            }
            if ( doSearch )
            {
                filtered = ObjectFindText<ObjectType>(objects, searchText);
            }
            else
            {
                filtered = objects;
            }

            DataTable table = new DataTable();
            table.Columns.Add("Item", typeof(clsUnitType));
            table.Columns.Add("Internal Name", typeof(string));
            table.Columns.Add("In-Game Name", typeof(string));
            //table.Columns.Add("Type")
            for ( int i = 0; i <= filtered.Count - 1; i++ )
            {
                ObjectType item = filtered[i];
                string code = null;
                item.GetCode(ref code);
                table.Rows.Add(new object[] {item, code, item.GetName().Replace("*", "")});
            }
            gridView.DataSource = table;
            gridView.Columns[0].Visible = false;
            gridView.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            gridView.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

#if !Mono
            ObjectTypeSelectionUpdate(gridView);
#else
			gridView.ClearSelection(); //mono selects its rows too late
#endif
        }

        public modLists.SimpleList<ItemType> ObjectFindText<ItemType>(modLists.SimpleList<ItemType> list, string text) where ItemType : clsUnitType
        {
            modLists.SimpleList<ItemType> result = new modLists.SimpleList<ItemType>();
            result.MaintainOrder = true;

            text = text.ToLower();

            for ( int i = 0; i <= list.Count - 1; i++ )
            {
                ItemType item = list[i];
                string code = null;
                if ( item.GetCode(ref code) )
                {
                    if ( code.ToLower().IndexOf(text) >= 0 || item.GetName().ToLower().IndexOf(text) >= 0 )
                    {
                        result.Add(item);
                    }
                }
                else if ( item.GetName().ToLower().IndexOf(text) >= 0 )
                {
                    result.Add(item);
                }
            }

            return result;
        }

        public void txtObjectRotation_LostFocus(System.Object sender, System.EventArgs e)
        {
            if ( !txtObjectRotation.Enabled )
            {
                return;
            }
            if ( txtObjectRotation.Text == "" )
            {
                return;
            }

            clsMap Map = MainMap;

            if ( Map == null )
            {
                return;
            }
            if ( Map.SelectedUnits.Count <= 0 )
            {
                return;
            }

            int Angle = 0;
            if ( !modIO.InvariantParse_int(txtObjectRotation.Text, ref Angle) )
            {
                //MsgBox("Invalid rotation value.", (MsgBoxStyle.OkOnly or MsgBoxStyle.Information), "")
                //SelectedObject_Changed()
                //todo
                return;
            }

            Angle = modMath.Clamp_int(Angle, 0, 359);

            if ( Map.SelectedUnits.Count > 1 )
            {
                if ( Interaction.MsgBox("Change rotation of multiple objects?", MsgBoxStyle.OkCancel, "") != MsgBoxResult.Ok )
                {
                    //SelectedObject_Changed()
                    return;
                }
            }

            clsMap.clsObjectRotation ObjectRotation = new clsMap.clsObjectRotation();
            ObjectRotation.Map = Map;
            ObjectRotation.Angle = Angle;
            Map.SelectedUnitsAction(ObjectRotation);

            Map.Update();
            SelectedObject_Changed();
            Map.UndoStepCreate("Object Rotated");
            View_DrawViewLater();
        }

        public void SelectedObject_Changed()
        {
            bool ClearControls = default(bool);
            clsMap Map = MainMap;

            lblObjectType.Enabled = false;
            ObjectPlayerNum.Enabled = false;
            txtObjectRotation.Enabled = false;
            txtObjectID.Enabled = false;
            txtObjectLabel.Enabled = false;
            txtObjectPriority.Enabled = false;
            txtObjectHealth.Enabled = false;
            btnDroidToDesign.Enabled = false;
            cboDroidType.Enabled = false;
            cboDroidBody.Enabled = false;
            cboDroidPropulsion.Enabled = false;
            cboDroidTurret1.Enabled = false;
            cboDroidTurret2.Enabled = false;
            cboDroidTurret3.Enabled = false;
            rdoDroidTurret0.Enabled = false;
            rdoDroidTurret1.Enabled = false;
            rdoDroidTurret2.Enabled = false;
            rdoDroidTurret3.Enabled = false;
            if ( Map == null )
            {
                ClearControls = true;
            }
            else if ( Map.SelectedUnits.Count == 0 )
            {
                ClearControls = true;
            }
            if ( ClearControls )
            {
                lblObjectType.Text = "";
                ObjectPlayerNum.Target.Item = null;
                txtObjectRotation.Text = "";
                txtObjectID.Text = "";
                txtObjectLabel.Text = "";
                txtObjectPriority.Text = "";
                txtObjectHealth.Text = "";
                cboDroidType.SelectedIndex = -1;
                cboDroidBody.SelectedIndex = -1;
                cboDroidPropulsion.SelectedIndex = -1;
                cboDroidTurret1.SelectedIndex = -1;
                cboDroidTurret2.SelectedIndex = -1;
                cboDroidTurret3.SelectedIndex = -1;
                rdoDroidTurret0.Checked = false;
                rdoDroidTurret1.Checked = false;
                rdoDroidTurret2.Checked = false;
                rdoDroidTurret3.Checked = false;
            }
            else if ( Map.SelectedUnits.Count > 1 )
            {
                lblObjectType.Text = "Multiple objects";
                int A = 0;
                clsMap.clsUnitGroup UnitGroup = Map.SelectedUnits[0].UnitGroup;
                for ( A = 1; A <= Map.SelectedUnits.Count - 1; A++ )
                {
                    if ( Map.SelectedUnits[A].UnitGroup != UnitGroup )
                    {
                        break;
                    }
                }
                if ( A == Map.SelectedUnits.Count )
                {
                    ObjectPlayerNum.Target.Item = UnitGroup;
                }
                else
                {
                    ObjectPlayerNum.Target.Item = null;
                }
                txtObjectRotation.Text = "";
                txtObjectID.Text = "";
                txtObjectLabel.Text = "";
                lblObjectType.Enabled = true;
                ObjectPlayerNum.Enabled = true;
                txtObjectRotation.Enabled = true;
                txtObjectPriority.Text = "";
                txtObjectPriority.Enabled = true;
                txtObjectHealth.Text = "";
                txtObjectHealth.Enabled = true;
                //design
                clsMap.clsUnit Unit = default(clsMap.clsUnit);
                foreach ( clsMap.clsUnit tempLoopVar_Unit in Map.SelectedUnits )
                {
                    Unit = tempLoopVar_Unit;
                    if ( Unit.Type.Type == clsUnitType.enumType.PlayerDroid )
                    {
                        if ( ((clsDroidDesign)Unit.Type).IsTemplate )
                        {
                            break;
                        }
                    }
                }
                if ( A < Map.SelectedUnits.Count )
                {
                    btnDroidToDesign.Enabled = true;
                }

                foreach ( clsMap.clsUnit tempLoopVar_Unit in Map.SelectedUnits )
                {
                    Unit = tempLoopVar_Unit;
                    if ( Unit.Type.Type == clsUnitType.enumType.PlayerDroid )
                    {
                        if ( !((clsDroidDesign)Unit.Type).IsTemplate )
                        {
                            break;
                        }
                    }
                }
                if ( A < Map.SelectedUnits.Count )
                {
                    cboDroidType.SelectedIndex = -1;
                    cboDroidBody.SelectedIndex = -1;
                    cboDroidPropulsion.SelectedIndex = -1;
                    cboDroidTurret1.SelectedIndex = -1;
                    cboDroidTurret2.SelectedIndex = -1;
                    cboDroidTurret3.SelectedIndex = -1;
                    rdoDroidTurret1.Checked = false;
                    rdoDroidTurret2.Checked = false;
                    rdoDroidTurret3.Checked = false;
                    cboDroidType.Enabled = true;
                    cboDroidBody.Enabled = true;
                    cboDroidPropulsion.Enabled = true;
                    cboDroidTurret1.Enabled = true;
                    cboDroidTurret2.Enabled = true;
                    cboDroidTurret3.Enabled = true;
                    rdoDroidTurret0.Enabled = true;
                    rdoDroidTurret1.Enabled = true;
                    rdoDroidTurret2.Enabled = true;
                    rdoDroidTurret3.Enabled = true;
                }
            }
            else if ( Map.SelectedUnits.Count == 1 )
            {
                int A = 0;
                clsMap.clsUnit with_1 = Map.SelectedUnits[0];
                lblObjectType.Text = System.Convert.ToString(with_1.Type.GetDisplayTextCode());
                ObjectPlayerNum.Target.Item = with_1.UnitGroup;
                txtObjectRotation.Text = modIO.InvariantToString_int(System.Convert.ToInt32(with_1.Rotation));
                txtObjectID.Text = modIO.InvariantToString_uint(with_1.ID);
                txtObjectPriority.Text = modIO.InvariantToString_int(System.Convert.ToInt32(with_1.SavePriority));
                txtObjectHealth.Text = modIO.InvariantToString_dbl(System.Convert.ToDouble(with_1.Health * 100.0D));
                lblObjectType.Enabled = true;
                ObjectPlayerNum.Enabled = true;
                txtObjectRotation.Enabled = true;
                //txtObjectID.Enabled = True 'no known need to change IDs
                txtObjectPriority.Enabled = true;
                txtObjectHealth.Enabled = true;
                bool LabelEnabled = true;
                if ( with_1.Type.Type == clsUnitType.enumType.PlayerStructure )
                {
                    if ( ((clsStructureType)with_1.Type).IsModule() )
                    {
                        LabelEnabled = false;
                    }
                }
                if ( LabelEnabled )
                {
                    txtObjectLabel.Text = System.Convert.ToString(with_1.Label);
                    txtObjectLabel.Enabled = true;
                }
                else
                {
                    txtObjectLabel.Text = "";
                }
                bool ClearDesignControls = false;
                if ( with_1.Type.Type == clsUnitType.enumType.PlayerDroid )
                {
                    clsDroidDesign DroidType = (clsDroidDesign)with_1.Type;
                    if ( DroidType.IsTemplate )
                    {
                        btnDroidToDesign.Enabled = true;
                        ClearDesignControls = true;
                    }
                    else
                    {
                        if ( DroidType.TemplateDroidType == null )
                        {
                            cboDroidType.SelectedIndex = -1;
                        }
                        else
                        {
                            cboDroidType.SelectedIndex = DroidType.TemplateDroidType.Num;
                        }

                        if ( DroidType.Body == null )
                        {
                            cboDroidBody.SelectedIndex = -1;
                        }
                        else
                        {
                            for ( A = 0; A <= cboDroidBody.Items.Count - 1; A++ )
                            {
                                if ( cboBody_Objects[A] == DroidType.Body )
                                {
                                    break;
                                }
                            }
                            if ( A < 0 )
                            {
                            }
                            else if ( A < cboDroidBody.Items.Count )
                            {
                                cboDroidBody.SelectedIndex = A;
                            }
                            else
                            {
                                cboDroidBody.SelectedIndex = -1;
                                cboDroidBody.Text = DroidType.Body.Code;
                            }
                        }

                        if ( DroidType.Propulsion == null )
                        {
                            cboDroidPropulsion.SelectedIndex = -1;
                        }
                        else
                        {
                            for ( A = 0; A <= cboDroidPropulsion.Items.Count - 1; A++ )
                            {
                                if ( cboPropulsion_Objects[A] == DroidType.Propulsion )
                                {
                                    break;
                                }
                            }
                            if ( A < cboDroidPropulsion.Items.Count )
                            {
                                cboDroidPropulsion.SelectedIndex = A;
                            }
                            else
                            {
                                cboDroidPropulsion.SelectedIndex = -1;
                                cboDroidPropulsion.Text = DroidType.Propulsion.Code;
                            }
                        }

                        if ( DroidType.Turret1 == null )
                        {
                            cboDroidTurret1.SelectedIndex = -1;
                        }
                        else
                        {
                            for ( A = 0; A <= cboDroidTurret1.Items.Count - 1; A++ )
                            {
                                if ( cboTurret_Objects[A] == DroidType.Turret1 )
                                {
                                    break;
                                }
                            }
                            if ( A < cboDroidTurret1.Items.Count )
                            {
                                cboDroidTurret1.SelectedIndex = A;
                            }
                            else
                            {
                                cboDroidTurret1.SelectedIndex = -1;
                                cboDroidTurret1.Text = DroidType.Turret1.Code;
                            }
                        }

                        if ( DroidType.Turret2 == null )
                        {
                            cboDroidTurret2.SelectedIndex = -1;
                        }
                        else
                        {
                            for ( A = 0; A <= cboDroidTurret2.Items.Count - 1; A++ )
                            {
                                if ( cboTurret_Objects[A] == DroidType.Turret2 )
                                {
                                    break;
                                }
                            }
                            if ( A < cboDroidTurret2.Items.Count )
                            {
                                cboDroidTurret2.SelectedIndex = A;
                            }
                            else
                            {
                                cboDroidTurret2.SelectedIndex = -1;
                                cboDroidTurret2.Text = DroidType.Turret2.Code;
                            }
                        }

                        if ( DroidType.Turret3 == null )
                        {
                            cboDroidTurret3.SelectedIndex = -1;
                        }
                        else
                        {
                            for ( A = 0; A <= cboDroidTurret3.Items.Count - 1; A++ )
                            {
                                if ( cboTurret_Objects[A] == DroidType.Turret3 )
                                {
                                    break;
                                }
                            }
                            if ( A < cboDroidTurret3.Items.Count )
                            {
                                cboDroidTurret3.SelectedIndex = A;
                            }
                            else
                            {
                                cboDroidTurret3.SelectedIndex = -1;
                                cboDroidTurret3.Text = DroidType.Turret3.Code;
                            }
                        }

                        if ( DroidType.TurretCount == 3 )
                        {
                            rdoDroidTurret0.Checked = false;
                            rdoDroidTurret1.Checked = false;
                            rdoDroidTurret2.Checked = false;
                            rdoDroidTurret3.Checked = true;
                        }
                        else if ( DroidType.TurretCount == 2 )
                        {
                            rdoDroidTurret0.Checked = false;
                            rdoDroidTurret1.Checked = false;
                            rdoDroidTurret2.Checked = true;
                            rdoDroidTurret3.Checked = false;
                        }
                        else if ( DroidType.TurretCount == 1 )
                        {
                            rdoDroidTurret0.Checked = false;
                            rdoDroidTurret1.Checked = true;
                            rdoDroidTurret2.Checked = false;
                            rdoDroidTurret3.Checked = false;
                        }
                        else if ( DroidType.TurretCount == 0 )
                        {
                            rdoDroidTurret0.Checked = true;
                            rdoDroidTurret1.Checked = false;
                            rdoDroidTurret2.Checked = false;
                            rdoDroidTurret3.Checked = false;
                        }
                        else
                        {
                            rdoDroidTurret0.Checked = false;
                            rdoDroidTurret1.Checked = false;
                            rdoDroidTurret2.Checked = false;
                            rdoDroidTurret3.Checked = false;
                        }

                        cboDroidType.Enabled = true;
                        cboDroidBody.Enabled = true;
                        cboDroidPropulsion.Enabled = true;
                        cboDroidTurret1.Enabled = true;
                        cboDroidTurret2.Enabled = true;
                        cboDroidTurret3.Enabled = true;
                        rdoDroidTurret0.Enabled = true;
                        rdoDroidTurret1.Enabled = true;
                        rdoDroidTurret2.Enabled = true;
                        rdoDroidTurret3.Enabled = true;
                    }
                }
                else
                {
                    ClearDesignControls = true;
                }
                if ( ClearDesignControls )
                {
                    cboDroidType.SelectedIndex = -1;
                    cboDroidBody.SelectedIndex = -1;
                    cboDroidPropulsion.SelectedIndex = -1;
                    cboDroidTurret1.SelectedIndex = -1;
                    cboDroidTurret2.SelectedIndex = -1;
                    cboDroidTurret3.SelectedIndex = -1;
                    rdoDroidTurret1.Checked = false;
                    rdoDroidTurret2.Checked = false;
                    rdoDroidTurret3.Checked = false;
                }
            }
        }

        public void tsbSelection_Click(System.Object sender, System.EventArgs e)
        {
            modTools.Tool = modTools.Tools.TerrainSelect;
        }

        public void tsbSelectionCopy_Click(System.Object sender, System.EventArgs e)
        {
            clsMap Map = MainMap;

            if ( Map == null )
            {
                return;
            }
            if ( Map.Selected_Area_VertexA == null || Map.Selected_Area_VertexB == null )
            {
                return;
            }
            if ( modProgram.Copied_Map != null )
            {
                modProgram.Copied_Map.Deallocate();
            }
            modMath.sXY_int Area = new modMath.sXY_int();
            modMath.sXY_int Start = new modMath.sXY_int();
            modMath.sXY_int Finish = new modMath.sXY_int();
            modMath.ReorderXY(Map.Selected_Area_VertexA.XY, Map.Selected_Area_VertexB.XY, Start, Finish);
            Area.X = Finish.X - Start.X;
            Area.Y = Finish.Y - Start.Y;
            modProgram.Copied_Map = new clsMap(Map, Start, Area);
        }

        public void tsbSelectionPaste_Click(System.Object sender, System.EventArgs e)
        {
            clsMap Map = MainMap;

            if ( Map == null )
            {
                return;
            }
            if ( Map.Selected_Area_VertexA == null || Map.Selected_Area_VertexB == null )
            {
                return;
            }
            if ( modProgram.Copied_Map == null )
            {
                MessageBox.Show("Nothing to paste.");
                return;
            }
            if (
                !(menuSelPasteHeights.Checked || menuSelPasteTextures.Checked || menuSelPasteUnits.Checked || menuSelPasteDeleteUnits.Checked ||
                  menuSelPasteGateways.Checked || menuSelPasteDeleteGateways.Checked) )
            {
                return;
            }
            modMath.sXY_int Area = new modMath.sXY_int();
            modMath.sXY_int Start = new modMath.sXY_int();
            modMath.sXY_int Finish = new modMath.sXY_int();
            modMath.ReorderXY(Map.Selected_Area_VertexA.XY, Map.Selected_Area_VertexB.XY, Start, Finish);
            Area.X = Finish.X - Start.X;
            Area.Y = Finish.Y - Start.Y;
            Map.MapInsert(modProgram.Copied_Map, Start, Area, menuSelPasteHeights.Checked, menuSelPasteTextures.Checked, menuSelPasteUnits.Checked,
                menuSelPasteDeleteUnits.Checked, menuSelPasteGateways.Checked, menuSelPasteDeleteGateways.Checked);

            SelectedObject_Changed();
            Map.UndoStepCreate("Paste");

            View_DrawViewLater();
        }

        public void btnSelResize_Click(System.Object sender, System.EventArgs e)
        {
            clsMap Map = MainMap;

            if ( Map == null )
            {
                return;
            }

            if ( Map.Selected_Area_VertexA == null || Map.Selected_Area_VertexB == null )
            {
                Interaction.MsgBox("You haven\'t selected anything.", MsgBoxStyle.OkOnly, "");
                return;
            }

            modMath.sXY_int Start = new modMath.sXY_int();
            modMath.sXY_int Finish = new modMath.sXY_int();
            modMath.ReorderXY(Map.Selected_Area_VertexA.XY, Map.Selected_Area_VertexB.XY, Start, Finish);
            modMath.sXY_int Area = new modMath.sXY_int();
            Area.X = Finish.X - Start.X;
            Area.Y = Finish.Y - Start.Y;

            Map_Resize(Start, Area);
        }

        public void tsbSelectionRotateClockwise_Click(System.Object sender, System.EventArgs e)
        {
            if ( modProgram.Copied_Map == null )
            {
                MessageBox.Show("Nothing to rotate.");
                return;
            }

            modProgram.Copied_Map.Rotate(TileOrientation.Orientation_Clockwise, modMain.frmMainInstance.PasteRotateObjects);
        }

        public void tsbSelectionRotateAnticlockwise_Click(System.Object sender, System.EventArgs e)
        {
            if ( modProgram.Copied_Map == null )
            {
                MessageBox.Show("Nothing to rotate.");
                return;
            }

            modProgram.Copied_Map.Rotate(TileOrientation.Orientation_CounterClockwise, modMain.frmMainInstance.PasteRotateObjects);
        }

        public void menuMiniShowTex_Click(System.Object sender, System.EventArgs e)
        {
            UpdateMinimap();
        }

        public void menuMiniShowHeight_Click(System.Object sender, System.EventArgs e)
        {
            UpdateMinimap();
        }

        public void menuMiniShowUnits_Click(System.Object sender, System.EventArgs e)
        {
            UpdateMinimap();
        }

        private clsResult NoTile_Texture_Load()
        {
            clsResult ReturnResult = new clsResult("Loading error terrain textures");

            Bitmap Bitmap = null;

            modBitmap.sBitmapGLTexture BitmapTextureArgs = new modBitmap.sBitmapGLTexture();

            BitmapTextureArgs.MagFilter = TextureMagFilter.Nearest;
            BitmapTextureArgs.MinFilter = TextureMinFilter.Nearest;
            BitmapTextureArgs.TextureNum = 0;
            BitmapTextureArgs.MipMapLevel = 0;

            if (
                modBitmap.LoadBitmap(
                    modProgram.EndWithPathSeperator((new Microsoft.VisualBasic.ApplicationServices.ConsoleApplicationBase()).Info.DirectoryPath) + "notile.png",
                    ref Bitmap).Success )
            {
                clsResult Result = new clsResult("notile.png");
                Result.Take(modBitmap.BitmapIsGLCompatible(Bitmap));
                ReturnResult.Add(Result);
                BitmapTextureArgs.Texture = Bitmap;
                BitmapTextureArgs.Perform();
                modProgram.GLTexture_NoTile = BitmapTextureArgs.TextureNum;
            }
            if (
                modBitmap.LoadBitmap(
                    modProgram.EndWithPathSeperator((new Microsoft.VisualBasic.ApplicationServices.ConsoleApplicationBase()).Info.DirectoryPath) + "overflow.png",
                    ref Bitmap).Success )
            {
                clsResult Result = new clsResult("overflow.png");
                Result.Take(modBitmap.BitmapIsGLCompatible(Bitmap));
                ReturnResult.Add(Result);
                BitmapTextureArgs.Texture = Bitmap;
                BitmapTextureArgs.Perform();
                modProgram.GLTexture_OverflowTile = BitmapTextureArgs.TextureNum;
            }

            return ReturnResult;
        }

        public void tsbGateways_Click(System.Object sender, System.EventArgs e)
        {
            if ( modTools.Tool == modTools.Tools.Gateways )
            {
                modProgram.Draw_Gateways = false;
                modTools.Tool = modTools.Tools.ObjectSelect;
                tsbGateways.Checked = false;
            }
            else
            {
                modProgram.Draw_Gateways = true;
                modTools.Tool = modTools.Tools.Gateways;
                tsbGateways.Checked = true;
            }
            clsMap Map = MainMap;
            if ( Map != null )
            {
                Map.Selected_Tile_A = null;
                Map.Selected_Tile_B = null;
                View_DrawViewLater();
            }
        }

        public void tsbDrawAutotexture_Click(System.Object sender, System.EventArgs e)
        {
            if ( MapView != null )
            {
                if ( modProgram.Draw_VertexTerrain != tsbDrawAutotexture.Checked )
                {
                    modProgram.Draw_VertexTerrain = tsbDrawAutotexture.Checked;
                    View_DrawViewLater();
                }
            }
        }

        public void tsbDrawTileOrientation_Click(System.Object sender, System.EventArgs e)
        {
            if ( MapView != null )
            {
                if ( modProgram.DisplayTileOrientation != tsbDrawTileOrientation.Checked )
                {
                    modProgram.DisplayTileOrientation = tsbDrawTileOrientation.Checked;
                    View_DrawViewLater();
                    TextureView.DrawViewLater();
                }
            }
        }

        public void menuMiniShowGateways_Click(System.Object sender, System.EventArgs e)
        {
            UpdateMinimap();
        }

        public void menuMiniShowCliffs_Click(object sender, System.EventArgs e)
        {
            UpdateMinimap();
        }

        private void UpdateMinimap()
        {
            clsMap Map = MainMap;

            if ( Map == null )
            {
                return;
            }

            Map.MinimapMakeLater();
        }

        public void cmbTileType_SelectedIndexChanged(System.Object sender, System.EventArgs e)
        {
            if ( !cboTileType.Enabled )
            {
                return;
            }

            clsMap Map = MainMap;

            if ( Map == null )
            {
                return;
            }
            if ( cboTileType.SelectedIndex < 0 )
            {
                return;
            }
            if ( modProgram.SelectedTextureNum < 0 | modProgram.SelectedTextureNum >= Map.Tileset.TileCount )
            {
                return;
            }

            Map.Tile_TypeNum[modProgram.SelectedTextureNum] = (byte)cboTileType.SelectedIndex;

            TextureView.DrawViewLater();
        }

        public void chkTileTypes_CheckedChanged(System.Object sender, System.EventArgs e)
        {
            TextureView.DisplayTileTypes = cbxTileTypes.Checked;
            TextureView.DrawViewLater();
        }

        public void chkTileNumbers_CheckedChanged(System.Object sender, System.EventArgs e)
        {
            TextureView.DisplayTileNumbers = cbxTileNumbers.Checked;
            TextureView.DrawViewLater();
        }

        private void cboTileType_Update()
        {
            cboTileType.Items.Clear();
            modProgram.clsTileType tileType = default(modProgram.clsTileType);
            foreach ( modProgram.clsTileType tempLoopVar_tileType in modProgram.TileTypes )
            {
                tileType = tempLoopVar_tileType;
                cboTileType.Items.Add(tileType.Name);
            }
        }

        private void CreateTileTypes()
        {
            modProgram.clsTileType NewTileType = default(modProgram.clsTileType);

            NewTileType = new modProgram.clsTileType();
            NewTileType.Name = "Sand";
            NewTileType.DisplayColour.Red = 1.0F;
            NewTileType.DisplayColour.Green = 1.0F;
            NewTileType.DisplayColour.Blue = 0.0F;
            modProgram.TileTypes.Add(NewTileType);

            NewTileType = new modProgram.clsTileType();
            NewTileType.Name = "Sandy Brush";
            NewTileType.DisplayColour.Red = 0.5F;
            NewTileType.DisplayColour.Green = 0.5F;
            NewTileType.DisplayColour.Blue = 0.0F;
            modProgram.TileTypes.Add(NewTileType);

            NewTileType = new modProgram.clsTileType();
            NewTileType.Name = "Rubble";
            NewTileType.DisplayColour.Red = 0.25F;
            NewTileType.DisplayColour.Green = 0.25F;
            NewTileType.DisplayColour.Blue = 0.25F;
            modProgram.TileTypes.Add(NewTileType);

            NewTileType = new modProgram.clsTileType();
            NewTileType.Name = "Green Mud";
            NewTileType.DisplayColour.Red = 0.0F;
            NewTileType.DisplayColour.Green = 0.5F;
            NewTileType.DisplayColour.Blue = 0.0F;
            modProgram.TileTypes.Add(NewTileType);

            NewTileType = new modProgram.clsTileType();
            NewTileType.Name = "Red Brush";
            NewTileType.DisplayColour.Red = 1.0F;
            NewTileType.DisplayColour.Green = 0.0F;
            NewTileType.DisplayColour.Blue = 0.0F;
            modProgram.TileTypes.Add(NewTileType);

            NewTileType = new modProgram.clsTileType();
            NewTileType.Name = "Pink Rock";
            NewTileType.DisplayColour.Red = 1.0F;
            NewTileType.DisplayColour.Green = 0.5F;
            NewTileType.DisplayColour.Blue = 0.5F;
            modProgram.TileTypes.Add(NewTileType);

            NewTileType = new modProgram.clsTileType();
            NewTileType.Name = "Road";
            NewTileType.DisplayColour.Red = 0.0F;
            NewTileType.DisplayColour.Green = 0.0F;
            NewTileType.DisplayColour.Blue = 0.0F;
            modProgram.TileTypes.Add(NewTileType);

            NewTileType = new modProgram.clsTileType();
            NewTileType.Name = "Water";
            NewTileType.DisplayColour.Red = 0.0F;
            NewTileType.DisplayColour.Green = 0.0F;
            NewTileType.DisplayColour.Blue = 1.0F;
            modProgram.TileTypes.Add(NewTileType);

            NewTileType = new modProgram.clsTileType();
            NewTileType.Name = "Cliff Face";
            NewTileType.DisplayColour.Red = 0.5F;
            NewTileType.DisplayColour.Green = 0.5F;
            NewTileType.DisplayColour.Blue = 0.5F;
            modProgram.TileTypes.Add(NewTileType);

            NewTileType = new modProgram.clsTileType();
            NewTileType.Name = "Baked Earth";
            NewTileType.DisplayColour.Red = 0.5F;
            NewTileType.DisplayColour.Green = 0.0F;
            NewTileType.DisplayColour.Blue = 0.0F;
            modProgram.TileTypes.Add(NewTileType);

            NewTileType = new modProgram.clsTileType();
            NewTileType.Name = "Sheet Ice";
            NewTileType.DisplayColour.Red = 1.0F;
            NewTileType.DisplayColour.Green = 1.0F;
            NewTileType.DisplayColour.Blue = 1.0F;
            modProgram.TileTypes.Add(NewTileType);

            NewTileType = new modProgram.clsTileType();
            NewTileType.Name = "Slush";
            NewTileType.DisplayColour.Red = 0.75F;
            NewTileType.DisplayColour.Green = 0.75F;
            NewTileType.DisplayColour.Blue = 0.75F;
            modProgram.TileTypes.Add(NewTileType);
        }

        public void menuExportMapTileTypes_Click(System.Object sender, System.EventArgs e)
        {
            PromptSave_TTP();
        }

        public void ImportHeightmapToolStripMenuItem_Click(System.Object sender, System.EventArgs e)
        {
            Load_Heightmap_Prompt();
        }

        public void menuImportTileTypes_Click(System.Object sender, System.EventArgs e)
        {
            Load_TTP_Prompt();
        }

        public void tsbSave_Click(System.Object sender, System.EventArgs e)
        {
            QuickSave();
        }

        private void QuickSave()
        {
            clsMap Map = MainMap;

            if ( Map == null )
            {
                return;
            }

            if ( Map.Save_FMap_Quick() )
            {
                modMain.frmMainInstance.tsbSave.Enabled = false;
                TitleTextUpdate();
            }
        }

        public void TitleTextUpdate()
        {
            clsMap Map = MainMap;
            string MapFileTitle = "";

            menuSaveFMapQuick.Text = "Quick Save fmap";
            menuSaveFMapQuick.Enabled = false;
            if ( Map == null )
            {
                MapFileTitle = "No Map";
                tsbSave.ToolTipText = "No Map";
            }
            else
            {
                if ( Map.PathInfo == null )
                {
                    MapFileTitle = "Unsaved map";
                    tsbSave.ToolTipText = "Save FMap...";
                }
                else
                {
                    modProgram.sSplitPath SplitPath = new modProgram.sSplitPath(Map.PathInfo.Path);
                    if ( Map.PathInfo.IsFMap )
                    {
                        MapFileTitle = SplitPath.FileTitleWithoutExtension;
                        string quickSavePath = Map.PathInfo.Path;
                        tsbSave.ToolTipText = "Quick save FMap to " + System.Convert.ToString(ControlChars.Quote) + quickSavePath +
                                              System.Convert.ToString(ControlChars.Quote);
                        menuSaveFMapQuick.Text = "Quick Save fmap to " + System.Convert.ToString(ControlChars.Quote);
                        if ( quickSavePath.Length <= 32 )
                        {
                            menuSaveFMapQuick.Text += quickSavePath;
                        }
                        else
                        {
                            menuSaveFMapQuick.Text += quickSavePath.Substring(0, 10) + "..." + quickSavePath.Substring(quickSavePath.Length - 20, 20);
                        }
                        menuSaveFMapQuick.Text += ControlChars.Quote.ToString();
                        menuSaveFMapQuick.Enabled = true;
                    }
                    else
                    {
                        MapFileTitle = SplitPath.FileTitle;
                        tsbSave.ToolTipText = "Save FMap...";
                    }
                }
                Map.SetTabText();
            }

            Text = MapFileTitle + " - " + modProgram.ProgramName + " " + modProgram.ProgramVersionNumber;
        }

        public void lstAutoTexture_SelectedIndexChanged(System.Object sender, System.EventArgs e)
        {
            if ( !lstAutoTexture.Enabled )
            {
                return;
            }

            if ( !(modTools.Tool == modTools.Tools.TerrainBrush || modTools.Tool == modTools.Tools.TerrainFill) )
            {
                rdoAutoTexturePlace.Checked = true;
                rdoAutoTexturePlace_Click(null, null);
            }
        }

        public void lstAutoRoad_Click(System.Object sender, System.EventArgs e)
        {
            if ( !(modTools.Tool == modTools.Tools.RoadPlace || modTools.Tool == modTools.Tools.RoadLines) )
            {
                rdoAutoRoadLine.Checked = true;
                rdoAutoRoadLine_Click(null, null);
            }
        }

        private void tabPlayerNum_SelectedIndexChanged()
        {
            //ObjectPlayerNum.Focus() 'so that the rotation textbox and anything else loses focus, and performs its effects

            if ( !ObjectPlayerNum.Enabled )
            {
                return;
            }

            clsMap Map = MainMap;

            if ( Map == null )
            {
                return;
            }
            if ( Map.SelectedUnits.Count <= 0 )
            {
                return;
            }
            if ( ObjectPlayerNum.Target.Item == null )
            {
                return;
            }

            if ( Map.SelectedUnits.Count > 1 )
            {
                if ( Interaction.MsgBox("Change player of multiple objects?", MsgBoxStyle.OkCancel, "") != MsgBoxResult.Ok )
                {
                    //SelectedObject_Changed()
                    //todo
                    return;
                }
            }

            clsMap.clsObjectUnitGroup ObjectUnitGroup = new clsMap.clsObjectUnitGroup();
            ObjectUnitGroup.Map = Map;
            ObjectUnitGroup.UnitGroup = ObjectPlayerNum.Target.Item;
            Map.SelectedUnitsAction(ObjectUnitGroup);

            SelectedObject_Changed();
            Map.UndoStepCreate("Object Player Changed");
            if ( modSettings.Settings.MinimapTeamColours )
            {
                Map.MinimapMakeLater();
            }
            View_DrawViewLater();
        }

        public void txtHeightSetL_LostFocus(object sender, System.EventArgs e)
        {
            byte Height = 0;
            string Text = "";
            double Height_dbl = 0;

            if ( !modIO.InvariantParse_dbl(txtHeightSetL.Text, ref Height_dbl) )
            {
                return;
            }
            Height = (byte)(modMath.Clamp_dbl(Height_dbl, byte.MinValue, byte.MaxValue));
            HeightSetPalette[tabHeightSetL.SelectedIndex] = Height;
            if ( tabHeightSetL.SelectedIndex == tabHeightSetL.SelectedIndex )
            {
                tabHeightSetL_SelectedIndexChanged(null, null);
            }
            Text = modIO.InvariantToString_byte(Height);
            tabHeightSetL.TabPages[tabHeightSetL.SelectedIndex].Text = Text;
            tabHeightSetR.TabPages[tabHeightSetL.SelectedIndex].Text = Text;
        }

        public void txtHeightSetR_LostFocus(object sender, System.EventArgs e)
        {
            byte Height = 0;
            string Text = "";
            double Height_dbl = 0;

            if ( !modIO.InvariantParse_dbl(txtHeightSetL.Text, ref Height_dbl) )
            {
                return;
            }
            Height = (byte)(modMath.Clamp_dbl(Height_dbl, byte.MinValue, byte.MaxValue));
            HeightSetPalette[tabHeightSetR.SelectedIndex] = Height;
            if ( tabHeightSetL.SelectedIndex == tabHeightSetR.SelectedIndex )
            {
                tabHeightSetL_SelectedIndexChanged(null, null);
            }
            Text = modIO.InvariantToString_byte(Height);
            tabHeightSetL.TabPages[tabHeightSetR.SelectedIndex].Text = Text;
            tabHeightSetR.TabPages[tabHeightSetR.SelectedIndex].Text = Text;
        }

        public void tabHeightSetL_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            txtHeightSetL.Text = modIO.InvariantToString_byte(HeightSetPalette[tabHeightSetL.SelectedIndex]);
        }

        public void tabHeightSetR_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            txtHeightSetR.Text = modIO.InvariantToString_byte(HeightSetPalette[tabHeightSetR.SelectedIndex]);
        }

        public void tsbSelectionObjects_Click(System.Object sender, System.EventArgs e)
        {
            clsMap Map = MainMap;

            if ( Map == null )
            {
                return;
            }

            if ( Map.Selected_Area_VertexA == null || Map.Selected_Area_VertexB == null )
            {
                return;
            }
            modMath.sXY_int Start = new modMath.sXY_int();
            modMath.sXY_int Finish = new modMath.sXY_int();
            int A = 0;

            modMath.ReorderXY(Map.Selected_Area_VertexA.XY, Map.Selected_Area_VertexB.XY, Start, Finish);
            for ( A = 0; A <= Map.Units.Count - 1; A++ )
            {
                if ( modProgram.PosIsWithinTileArea(Map.Units[A].Pos.Horizontal, Start, Finish) )
                {
                    if ( !Map.Units[A].MapSelectedUnitLink.IsConnected )
                    {
                        Map.Units[A].MapSelectedUnitLink.Connect(Map.SelectedUnits);
                    }
                }
            }

            SelectedObject_Changed();
            modTools.Tool = modTools.Tools.ObjectSelect;
            View_DrawViewLater();
        }

        public void View_DrawViewLater()
        {
            if ( MapView != null )
            {
                MapView.DrawViewLater();
            }
        }

        public void tsbSelectionFlipX_Click(System.Object sender, System.EventArgs e)
        {
            if ( modProgram.Copied_Map == null )
            {
                MessageBox.Show("Nothing to flip.");
                return;
            }

            modProgram.Copied_Map.Rotate(TileOrientation.Orientation_FlipX, modMain.frmMainInstance.PasteRotateObjects);
        }

        public void btnHeightsMultiplySelection_Click(System.Object sender, System.EventArgs e)
        {
            clsMap Map = MainMap;

            if ( Map == null )
            {
                return;
            }

            if ( Map.Selected_Area_VertexA == null || Map.Selected_Area_VertexB == null )
            {
                return;
            }

            int X = 0;
            int Y = 0;
            double Multiplier = 0;
            modMath.sXY_int StartXY = new modMath.sXY_int();
            modMath.sXY_int FinishXY = new modMath.sXY_int();
            modMath.sXY_int Pos = new modMath.sXY_int();
            double dblTemp = 0;

            if ( !modIO.InvariantParse_dbl(txtHeightMultiply.Text, ref dblTemp) )
            {
                return;
            }
            Multiplier = modMath.Clamp_dbl(dblTemp, 0.0D, 255.0D);
            modMath.ReorderXY(Map.Selected_Area_VertexA.XY, Map.Selected_Area_VertexB.XY, StartXY, FinishXY);
            for ( Y = StartXY.Y; Y <= FinishXY.Y; Y++ )
            {
                for ( X = StartXY.X; X <= FinishXY.X; X++ )
                {
                    Map.Terrain.Vertices[X, Y].Height =
                        (byte)(Math.Round(modMath.Clamp_dbl(System.Convert.ToDouble(Map.Terrain.Vertices[X, Y].Height * Multiplier), byte.MinValue, byte.MaxValue)));
                    Pos.X = X;
                    Pos.Y = Y;
                    Map.SectorGraphicsChanges.VertexAndNormalsChanged(Pos);
                    Map.SectorUnitHeightsChanges.VertexChanged(Pos);
                    Map.SectorTerrainUndoChanges.VertexChanged(Pos);
                }
            }

            Map.Update();

            Map.UndoStepCreate("Selection Heights Multiply");

            View_DrawViewLater();
        }

        public void btnTextureAnticlockwise_Click(System.Object sender, System.EventArgs e)
        {
            modProgram.TextureOrientation.RotateAnticlockwise();

            TextureView.DrawViewLater();
        }

        public void btnTextureClockwise_Click(System.Object sender, System.EventArgs e)
        {
            modProgram.TextureOrientation.RotateClockwise();

            TextureView.DrawViewLater();
        }

        public void btnTextureFlipX_Click(System.Object sender, System.EventArgs e)
        {
            if ( modProgram.TextureOrientation.SwitchedAxes )
            {
                modProgram.TextureOrientation.ResultYFlip = !modProgram.TextureOrientation.ResultYFlip;
            }
            else
            {
                modProgram.TextureOrientation.ResultXFlip = !modProgram.TextureOrientation.ResultXFlip;
            }

            TextureView.DrawViewLater();
        }

        public void lstAutoTexture_SelectedIndexChanged_1(System.Object sender, System.EventArgs e)
        {
            clsMap Map = MainMap;

            if ( Map == null )
            {
                return;
            }

            if ( lstAutoTexture.SelectedIndex < 0 )
            {
                modProgram.SelectedTerrain = null;
            }
            else if ( lstAutoTexture.SelectedIndex < Map.Painter.TerrainCount )
            {
                modProgram.SelectedTerrain = Map.Painter.Terrains[lstAutoTexture.SelectedIndex];
            }
            else
            {
                Debugger.Break();
                modProgram.SelectedTerrain = null;
            }
        }

        public void lstAutoRoad_SelectedIndexChanged(System.Object sender, System.EventArgs e)
        {
            clsMap Map = MainMap;

            if ( Map == null )
            {
                return;
            }

            if ( lstAutoRoad.SelectedIndex < 0 )
            {
                modProgram.SelectedRoad = null;
            }
            else if ( lstAutoRoad.SelectedIndex < Map.Painter.RoadCount )
            {
                modProgram.SelectedRoad = Map.Painter.Roads[lstAutoRoad.SelectedIndex];
            }
            else
            {
                Debugger.Break();
                modProgram.SelectedRoad = null;
            }
        }

        public void txtObjectPriority_LostFocus(System.Object sender, System.EventArgs e)
        {
            if ( !txtObjectPriority.Enabled )
            {
                return;
            }
            clsMap Map = MainMap;
            if ( Map == null )
            {
                return;
            }
            if ( Map.SelectedUnits.Count <= 0 )
            {
                return;
            }
            int Priority = 0;
            if ( !modIO.InvariantParse_int(txtObjectPriority.Text, ref Priority) )
            {
                //MsgBox("Entered text is not a valid number.", (MsgBoxStyle.OkOnly or MsgBoxStyle.Information), "")
                //SelectedObject_Changed()
                //todo
                return;
            }

            if ( Map.SelectedUnits.Count > 1 )
            {
                if ( Interaction.MsgBox("Change priority of multiple objects?", (Microsoft.VisualBasic.MsgBoxStyle)(MsgBoxStyle.OkCancel | MsgBoxStyle.Question), "") !=
                     MsgBoxResult.Ok )
                {
                    //SelectedObject_Changed()
                    //todo
                    return;
                }
            }
            else if ( Map.SelectedUnits.Count == 1 )
            {
                if ( Priority == Map.SelectedUnits[0].SavePriority )
                {
                    return;
                }
            }

            clsMap.clsObjectPriority ObjectPriority = new clsMap.clsObjectPriority();
            ObjectPriority.Map = Map;
            ObjectPriority.Priority = Priority;
            Map.SelectedUnitsAction(ObjectPriority);

            SelectedObject_Changed();
            Map.UndoStepCreate("Object Priority Changed");
            View_DrawViewLater();
        }

        public void txtObjectHealth_LostFocus(System.Object sender, System.EventArgs e)
        {
            if ( !txtObjectHealth.Enabled )
            {
                return;
            }
            clsMap Map = MainMap;
            if ( Map == null )
            {
                return;
            }
            if ( Map.SelectedUnits.Count <= 0 )
            {
                return;
            }

            double Health = 0;
            if ( !modIO.InvariantParse_dbl(txtObjectHealth.Text, ref Health) )
            {
                //SelectedObject_Changed()
                //todo
                return;
            }

            Health = modMath.Clamp_dbl(Health, 1.0D, 100.0D) / 100.0D;

            if ( Map.SelectedUnits.Count > 1 )
            {
                if ( Interaction.MsgBox("Change health of multiple objects?", (Microsoft.VisualBasic.MsgBoxStyle)(MsgBoxStyle.OkCancel | MsgBoxStyle.Question), "") !=
                     MsgBoxResult.Ok )
                {
                    //SelectedObject_Changed()
                    //todo
                    return;
                }
            }

            clsMap.clsObjectHealth ObjectHealth = new clsMap.clsObjectHealth();
            ObjectHealth.Map = Map;
            ObjectHealth.Health = Health;
            Map.SelectedUnitsAction(ObjectHealth);

            SelectedObject_Changed();
            Map.UndoStepCreate("Object Health Changed");
            View_DrawViewLater();
        }

        public void btnDroidToDesign_Click(System.Object sender, System.EventArgs e)
        {
            clsMap Map = MainMap;

            if ( Map == null )
            {
                return;
            }
            if ( Map.SelectedUnits.Count <= 0 )
            {
                return;
            }

            if ( Map.SelectedUnits.Count > 1 )
            {
                if ( Interaction.MsgBox("Change design of multiple droids?", (Microsoft.VisualBasic.MsgBoxStyle)(MsgBoxStyle.OkCancel | MsgBoxStyle.Question), "") !=
                     MsgBoxResult.Ok )
                {
                    return;
                }
            }
            else
            {
                if ( Interaction.MsgBox("Change design of a droid?", (Microsoft.VisualBasic.MsgBoxStyle)(MsgBoxStyle.OkCancel | MsgBoxStyle.Question), "") !=
                     MsgBoxResult.Ok )
                {
                    return;
                }
            }

            clsMap.clsObjectTemplateToDesign ObjectTemplateToDesign = new clsMap.clsObjectTemplateToDesign();
            ObjectTemplateToDesign.Map = Map;
            Map.SelectedUnitsAction(ObjectTemplateToDesign);

            SelectedObject_Changed();
            if ( ObjectTemplateToDesign.ActionPerformed )
            {
                Map.UndoStepCreate("Object Template Removed");
                View_DrawViewLater();
            }
        }

        public modProgram.enumObjectRotateMode PasteRotateObjects = modProgram.enumObjectRotateMode.Walls;

        public void menuRotateUnits_Click(System.Object sender, System.EventArgs e)
        {
            PasteRotateObjects = modProgram.enumObjectRotateMode.All;
            menuRotateUnits.Checked = true;
            menuRotateWalls.Checked = false;
            menuRotateNothing.Checked = false;
        }

        public void menuRotateWalls_Click(System.Object sender, System.EventArgs e)
        {
            PasteRotateObjects = modProgram.enumObjectRotateMode.Walls;
            menuRotateUnits.Checked = false;
            menuRotateWalls.Checked = true;
            menuRotateNothing.Checked = false;
        }

        public void menuRotateNothing_Click(System.Object sender, System.EventArgs e)
        {
            PasteRotateObjects = modProgram.enumObjectRotateMode.None;
            menuRotateUnits.Checked = false;
            menuRotateWalls.Checked = false;
            menuRotateNothing.Checked = true;
        }

        public void cboDroidBody_SelectedIndexChanged(System.Object sender, System.EventArgs e)
        {
            if ( !cboDroidBody.Enabled )
            {
                return;
            }
            if ( cboDroidBody.SelectedIndex < 0 )
            {
                return;
            }
            clsMap Map = MainMap;
            if ( Map == null )
            {
                return;
            }
            if ( Map.SelectedUnits.Count <= 0 )
            {
                return;
            }

            if ( Map.SelectedUnits.Count > 1 )
            {
                if ( Interaction.MsgBox("Change body of multiple droids?", (Microsoft.VisualBasic.MsgBoxStyle)(MsgBoxStyle.OkCancel | MsgBoxStyle.Question), "") !=
                     MsgBoxResult.Ok )
                {
                    return;
                }
            }

            clsMap.clsObjectBody ObjectBody = new clsMap.clsObjectBody();
            ObjectBody.Map = Map;
            ObjectBody.Body = cboBody_Objects[cboDroidBody.SelectedIndex];
            Map.SelectedUnitsAction(ObjectBody);

            SelectedObject_Changed();
            if ( ObjectBody.ActionPerformed )
            {
                Map.UndoStepCreate("Object Body Changed");
                View_DrawViewLater();
            }
        }

        public void cboDroidPropulsion_SelectedIndexChanged(System.Object sender, System.EventArgs e)
        {
            if ( !cboDroidPropulsion.Enabled )
            {
                return;
            }
            if ( cboDroidPropulsion.SelectedIndex < 0 )
            {
                return;
            }
            clsMap Map = MainMap;
            if ( Map == null )
            {
                return;
            }
            if ( Map.SelectedUnits.Count <= 0 )
            {
                return;
            }

            if ( Map.SelectedUnits.Count > 1 )
            {
                if (
                    Interaction.MsgBox("Change propulsion of multiple droids?", (Microsoft.VisualBasic.MsgBoxStyle)(MsgBoxStyle.OkCancel | MsgBoxStyle.Question), "") !=
                    MsgBoxResult.Ok )
                {
                    return;
                }
            }

            clsMap.clsObjectPropulsion ObjectPropulsion = new clsMap.clsObjectPropulsion();
            ObjectPropulsion.Map = Map;
            ObjectPropulsion.Propulsion = cboPropulsion_Objects[cboDroidPropulsion.SelectedIndex];
            Map.SelectedUnitsAction(ObjectPropulsion);

            SelectedObject_Changed();
            if ( ObjectPropulsion.ActionPerformed )
            {
                Map.UndoStepCreate("Object Body Changed");
                View_DrawViewLater();
            }
            SelectedObject_Changed();
            if ( ObjectPropulsion.ActionPerformed )
            {
                Map.UndoStepCreate("Object Propulsion Changed");
                View_DrawViewLater();
            }
        }

        public void cboDroidTurret1_SelectedIndexChanged(System.Object sender, System.EventArgs e)
        {
            if ( !cboDroidTurret1.Enabled )
            {
                return;
            }
            if ( cboDroidTurret1.SelectedIndex < 0 )
            {
                return;
            }
            clsMap Map = MainMap;
            if ( Map == null )
            {
                return;
            }
            if ( Map.SelectedUnits.Count <= 0 )
            {
                return;
            }

            if ( Map.SelectedUnits.Count > 1 )
            {
                if ( Interaction.MsgBox("Change turret of multiple droids?", (Microsoft.VisualBasic.MsgBoxStyle)(MsgBoxStyle.OkCancel | MsgBoxStyle.Question), "") !=
                     MsgBoxResult.Ok )
                {
                    return;
                }
            }

            clsMap.clsObjectTurret ObjectTurret = new clsMap.clsObjectTurret();
            ObjectTurret.Map = Map;
            ObjectTurret.Turret = cboTurret_Objects[cboDroidTurret1.SelectedIndex];
            ObjectTurret.TurretNum = 0;
            Map.SelectedUnitsAction(ObjectTurret);

            SelectedObject_Changed();
            if ( ObjectTurret.ActionPerformed )
            {
                Map.UndoStepCreate("Object Turret Changed");
                View_DrawViewLater();
            }
        }

        public void cboDroidTurret2_SelectedIndexChanged(System.Object sender, System.EventArgs e)
        {
            if ( !cboDroidTurret2.Enabled )
            {
                return;
            }
            if ( cboDroidTurret2.SelectedIndex < 0 )
            {
                return;
            }
            clsMap Map = MainMap;
            if ( Map == null )
            {
                return;
            }
            if ( Map.SelectedUnits.Count <= 0 )
            {
                return;
            }

            if ( Map.SelectedUnits.Count > 1 )
            {
                if ( Interaction.MsgBox("Change turret of multiple droids?", (Microsoft.VisualBasic.MsgBoxStyle)(MsgBoxStyle.OkCancel | MsgBoxStyle.Question), "") !=
                     MsgBoxResult.Ok )
                {
                    return;
                }
            }

            clsMap.clsObjectTurret ObjectTurret = new clsMap.clsObjectTurret();
            ObjectTurret.Map = Map;
            ObjectTurret.Turret = cboTurret_Objects[cboDroidTurret2.SelectedIndex];
            ObjectTurret.TurretNum = 1;
            Map.SelectedUnitsAction(ObjectTurret);

            SelectedObject_Changed();
            if ( ObjectTurret.ActionPerformed )
            {
                Map.UndoStepCreate("Object Turret Changed");
                View_DrawViewLater();
            }
        }

        public void cboDroidTurret3_SelectedIndexChanged(System.Object sender, System.EventArgs e)
        {
            if ( !cboDroidTurret3.Enabled )
            {
                return;
            }
            if ( cboDroidTurret3.SelectedIndex < 0 )
            {
                return;
            }
            clsMap Map = MainMap;
            if ( Map == null )
            {
                return;
            }
            if ( Map.SelectedUnits.Count <= 0 )
            {
                return;
            }

            if ( Map.SelectedUnits.Count > 1 )
            {
                if ( Interaction.MsgBox("Change turret of multiple droids?", (Microsoft.VisualBasic.MsgBoxStyle)(MsgBoxStyle.OkCancel | MsgBoxStyle.Question), "") !=
                     MsgBoxResult.Ok )
                {
                    return;
                }
            }

            clsMap.clsObjectTurret ObjectTurret = new clsMap.clsObjectTurret();
            ObjectTurret.Map = Map;
            ObjectTurret.Turret = cboTurret_Objects[cboDroidTurret3.SelectedIndex];
            ObjectTurret.TurretNum = 2;
            Map.SelectedUnitsAction(ObjectTurret);

            SelectedObject_Changed();
            if ( ObjectTurret.ActionPerformed )
            {
                Map.UndoStepCreate("Object Turret Changed");
                View_DrawViewLater();
            }
        }

        public void rdoDroidTurret0_CheckedChanged(System.Object sender, System.EventArgs e)
        {
            if ( !rdoDroidTurret0.Enabled )
            {
                return;
            }
            clsMap Map = MainMap;
            if ( Map == null )
            {
                return;
            }
            if ( Map.SelectedUnits.Count <= 0 )
            {
                return;
            }

            if ( !rdoDroidTurret0.Checked )
            {
                return;
            }

            rdoDroidTurret1.Checked = false;
            rdoDroidTurret2.Checked = false;
            rdoDroidTurret3.Checked = false;

            if ( Map.SelectedUnits.Count > 1 )
            {
                if (
                    Interaction.MsgBox("Change number of turrets of multiple droids?", (Microsoft.VisualBasic.MsgBoxStyle)(MsgBoxStyle.OkCancel | MsgBoxStyle.Question),
                        "") != MsgBoxResult.Ok )
                {
                    return;
                }
            }

            SelectedObjects_SetTurretCount((byte)0);
        }

        public void rdoDroidTurret1_CheckedChanged(System.Object sender, System.EventArgs e)
        {
            if ( !rdoDroidTurret1.Enabled )
            {
                return;
            }
            clsMap Map = MainMap;
            if ( Map == null )
            {
                return;
            }
            if ( Map.SelectedUnits.Count <= 0 )
            {
                return;
            }

            if ( !rdoDroidTurret1.Checked )
            {
                return;
            }

            rdoDroidTurret0.Checked = false;
            rdoDroidTurret2.Checked = false;
            rdoDroidTurret3.Checked = false;

            if ( Map.SelectedUnits.Count > 1 )
            {
                if (
                    Interaction.MsgBox("Change number of turrets of multiple droids?", (Microsoft.VisualBasic.MsgBoxStyle)(MsgBoxStyle.OkCancel | MsgBoxStyle.Question),
                        "") != MsgBoxResult.Ok )
                {
                    return;
                }
            }

            SelectedObjects_SetTurretCount((byte)1);
        }

        public void rdoDroidTurret2_CheckedChanged(System.Object sender, System.EventArgs e)
        {
            if ( !rdoDroidTurret2.Enabled )
            {
                return;
            }
            clsMap Map = MainMap;
            if ( Map == null )
            {
                return;
            }
            if ( Map.SelectedUnits.Count <= 0 )
            {
                return;
            }

            if ( !rdoDroidTurret2.Checked )
            {
                return;
            }

            rdoDroidTurret0.Checked = false;
            rdoDroidTurret1.Checked = false;
            rdoDroidTurret3.Checked = false;

            if ( Map.SelectedUnits.Count > 1 )
            {
                if (
                    Interaction.MsgBox("Change number of turrets of multiple droids?", (Microsoft.VisualBasic.MsgBoxStyle)(MsgBoxStyle.OkCancel | MsgBoxStyle.Question),
                        "") != MsgBoxResult.Ok )
                {
                    return;
                }
            }

            SelectedObjects_SetTurretCount((byte)2);
        }

        public void rdoDroidTurret3_CheckedChanged(System.Object sender, System.EventArgs e)
        {
            if ( !rdoDroidTurret2.Enabled )
            {
                return;
            }
            clsMap Map = MainMap;
            if ( Map == null )
            {
                return;
            }
            if ( Map.SelectedUnits.Count <= 0 )
            {
                return;
            }

            if ( !rdoDroidTurret3.Checked )
            {
                return;
            }

            rdoDroidTurret0.Checked = false;
            rdoDroidTurret1.Checked = false;
            rdoDroidTurret2.Checked = false;

            if ( Map.SelectedUnits.Count > 1 )
            {
                if (
                    Interaction.MsgBox("Change number of turrets of multiple droids?", (Microsoft.VisualBasic.MsgBoxStyle)(MsgBoxStyle.OkCancel | MsgBoxStyle.Question),
                        "") != MsgBoxResult.Ok )
                {
                    return;
                }
            }

            SelectedObjects_SetTurretCount((byte)3);
        }

        private void SelectedObjects_SetTurretCount(byte Count)
        {
            clsMap Map = MainMap;

            if ( Map == null )
            {
                return;
            }

            clsMap.clsObjectTurretCount ObjectTurretCount = new clsMap.clsObjectTurretCount();
            ObjectTurretCount.Map = Map;
            ObjectTurretCount.TurretCount = Count;
            Map.SelectedUnitsAction(ObjectTurretCount);

            SelectedObject_Changed();
            if ( ObjectTurretCount.ActionPerformed )
            {
                Map.UndoStepCreate("Object Number Of Turrets Changed");
                View_DrawViewLater();
            }
        }

        private void SelectedObjects_SetDroidType(clsDroidDesign.clsTemplateDroidType NewType)
        {
            clsMap Map = MainMap;

            if ( Map == null )
            {
                return;
            }

            clsMap.clsObjectDroidType ObjectDroidType = new clsMap.clsObjectDroidType();
            ObjectDroidType.Map = Map;
            ObjectDroidType.DroidType = NewType;
            Map.SelectedUnitsAction(ObjectDroidType);

            SelectedObject_Changed();
            if ( ObjectDroidType.ActionPerformed )
            {
                Map.UndoStepCreate("Object Number Of Turrets Changed");
                View_DrawViewLater();
            }
        }

        public void cboDroidType_SelectedIndexChanged(System.Object sender, System.EventArgs e)
        {
            if ( !cboDroidType.Enabled )
            {
                return;
            }
            if ( cboDroidType.SelectedIndex < 0 )
            {
                return;
            }
            clsMap Map = MainMap;
            if ( Map == null )
            {
                return;
            }
            if ( Map.SelectedUnits.Count <= 0 )
            {
                return;
            }

            if ( Map.SelectedUnits.Count > 1 )
            {
                if ( Interaction.MsgBox("Change type of multiple droids?", (Microsoft.VisualBasic.MsgBoxStyle)(MsgBoxStyle.OkCancel | MsgBoxStyle.Question), "") !=
                     MsgBoxResult.Ok )
                {
                    return;
                }
            }

            SelectedObjects_SetDroidType(modProgram.TemplateDroidTypes[cboDroidType.SelectedIndex]);
        }

        public void rdoTextureIgnoreTerrain_Click(System.Object sender, System.EventArgs e)
        {
            if ( rdoTextureIgnoreTerrain.Checked )
            {
                TextureTerrainAction = modProgram.enumTextureTerrainAction.Ignore;
                rdoTextureReinterpretTerrain.Checked = false;
                rdoTextureRemoveTerrain.Checked = false;
            }
        }

        public void rdoTextureReinterpretTerrain_Click(System.Object sender, System.EventArgs e)
        {
            if ( rdoTextureReinterpretTerrain.Checked )
            {
                TextureTerrainAction = modProgram.enumTextureTerrainAction.Reinterpret;
                rdoTextureIgnoreTerrain.Checked = false;
                rdoTextureRemoveTerrain.Checked = false;
            }
        }

        public void rdoTextureRemoveTerrain_Click(System.Object sender, System.EventArgs e)
        {
            if ( rdoTextureRemoveTerrain.Checked )
            {
                TextureTerrainAction = modProgram.enumTextureTerrainAction.Remove;
                rdoTextureIgnoreTerrain.Checked = false;
                rdoTextureReinterpretTerrain.Checked = false;
            }
        }

        public void btnPlayerSelectObjects_Click(System.Object sender, System.EventArgs e)
        {
            clsMap Map = MainMap;

            if ( Map == null )
            {
                return;
            }

            if ( !modControls.KeyboardProfile.Active(modControls.Control_Unit_Multiselect) )
            {
                Map.SelectedUnits.Clear();
            }

            clsMap.clsUnitGroup UnitGroup = Map.SelectedUnitGroup.Item;
            clsMap.clsUnit Unit = default(clsMap.clsUnit);
            foreach ( clsMap.clsUnit tempLoopVar_Unit in Map.Units )
            {
                Unit = tempLoopVar_Unit;
                if ( Unit.UnitGroup == UnitGroup )
                {
                    if ( Unit.Type.Type != clsUnitType.enumType.Feature )
                    {
                        if ( !Unit.MapSelectedUnitLink.IsConnected )
                        {
                            Unit.MapSelectedUnitLink.Connect(Map.SelectedUnits);
                        }
                    }
                }
            }

            View_DrawViewLater();
        }

        public void menuSaveFME_Click(System.Object sender, System.EventArgs e)
        {
            Save_FME_Prompt();
        }

        public void menuOptions_Click(System.Object sender, System.EventArgs e)
        {
            if ( modMain.frmOptionsInstance != null )
            {
                modMain.frmOptionsInstance.Activate();
                return;
            }
            modMain.frmOptionsInstance = new frmOptions();
            modMain.frmOptionsInstance.FormClosing += modMain.frmOptionsInstance.frmOptions_FormClosing;
            modMain.frmOptionsInstance.Show();
        }

        public void rdoCliffTriBrush_CheckedChanged(System.Object sender, System.EventArgs e)
        {
            modTools.Tool = modTools.Tools.CliffTriangle;
        }

        private void LoadInterfaceImage(string ImagePath, ref Bitmap ResultBitmap, clsResult Result)
        {
            modProgram.sResult BitmapResult = new modProgram.sResult();

            ResultBitmap = null;
            BitmapResult = modBitmap.LoadBitmap(ImagePath, ref ResultBitmap);
            if ( !BitmapResult.Success )
            {
                Result.WarningAdd("Unable to load image " + System.Convert.ToString(ControlChars.Quote) + ImagePath + System.Convert.ToString(ControlChars.Quote));
            }
        }

        public void MainMapAfterChanged()
        {
            clsMap Map = MainMap;

            MapView.UpdateTabs();

            modProgram.SelectedTerrain = null;
            modProgram.SelectedRoad = null;

            Resize_Update();
            MainMapTilesetChanged();
            PainterTerrains_Refresh(-1, -1);
            ScriptMarkerLists_Update();

            NewPlayerNum.Enabled = false;
            ObjectPlayerNum.Enabled = false;
            if ( Map != null )
            {
                Map.CheckMessages();
                Map.ViewInfo.FOV_Calc();
                Map.SectorGraphicsChanges.SetAllChanged();
                Map.Update();
                Map.MinimapMakeLater();
                tsbSave.Enabled = Map.ChangedSinceSave;
                NewPlayerNum.SetMap(Map);
                NewPlayerNum.Target = Map.SelectedUnitGroup;
                ObjectPlayerNum.SetMap(Map);
                MainMap.Changed += MainMap_Modified;
            }
            else
            {
                tsbSave.Enabled = false;
                NewPlayerNum.SetMap(null);
                NewPlayerNum.Target = null;
                ObjectPlayerNum.SetMap(null);
            }
            NewPlayerNum.Enabled = true;
            ObjectPlayerNum.Enabled = true;

            SelectedObject_Changed();

            TitleTextUpdate();

            TextureView.ScrollUpdate();

            TextureView.DrawViewLater();
            View_DrawViewLater();
        }

        public void MainMapBeforeChanged()
        {
            clsMap Map = MainMap;

            MapView.OpenGLControl.Focus(); //take focus from controls to trigger their lostfocuses

            if ( Map == null )
            {
                return;
            }

            MainMap.Changed -= MainMap_Modified;

            if ( Map.ReadyForUserInput )
            {
                Map.SectorAll_GLLists_Delete();
                Map.SectorGraphicsChanges.SetAllChanged();
            }
        }

        private void MainMap_Modified()
        {
            tsbSave.Enabled = true;
        }

        public void ObjectPicker(clsUnitType UnitType)
        {
            modTools.Tool = modTools.Tools.ObjectPlace;
            if ( !modControls.KeyboardProfile.Active(modControls.Control_Unit_Multiselect) )
            {
                dgvFeatures.ClearSelection();
                dgvStructures.ClearSelection();
                dgvDroids.ClearSelection();
            }
            SelectedObjectTypes.Clear();
            SelectedObjectTypes.Add(UnitType.UnitType_frmMainSelectedLink);
            clsMap Map = MainMap;
            if ( Map != null )
            {
                Map.MinimapMakeLater(); //for unit highlight
                View_DrawViewLater();
            }
        }

        public void TerrainPicker()
        {
            clsMap Map = MainMap;

            clsViewInfo.clsMouseOver.clsOverTerrain MouseOverTerrain = Map.ViewInfo.GetMouseOverTerrain();

            if ( MouseOverTerrain == null )
            {
                return;
            }

            modMath.sXY_int Vertex = MouseOverTerrain.Vertex.Normal;
            int A = 0;

            lstAutoTexture.Enabled = false;
            for ( A = 0; A <= lstAutoTexture.Items.Count - 1; A++ )
            {
                if ( Map.Painter.Terrains[A] == Map.Terrain.Vertices[Vertex.X, Vertex.Y].Terrain )
                {
                    lstAutoTexture.SelectedIndex = A;
                    break;
                }
            }
            if ( A == lstAutoTexture.Items.Count )
            {
                lstAutoTexture.SelectedIndex = -1;
            }
            lstAutoTexture.Enabled = true;
            modProgram.SelectedTerrain = Map.Terrain.Vertices[Vertex.X, Vertex.Y].Terrain;
        }

        public void TexturePicker()
        {
            clsMap Map = MainMap;

            clsViewInfo.clsMouseOver.clsOverTerrain MouseOverTerrain = Map.ViewInfo.GetMouseOverTerrain();

            if ( MouseOverTerrain == null )
            {
                return;
            }

            modMath.sXY_int Tile = MouseOverTerrain.Tile.Normal;

            if ( Map.Tileset != null )
            {
                if ( Map.Terrain.Tiles[Tile.X, Tile.Y].Texture.TextureNum < Map.Tileset.TileCount )
                {
                    modProgram.SelectedTextureNum = Map.Terrain.Tiles[Tile.X, Tile.Y].Texture.TextureNum;
                    TextureView.DrawViewLater();
                }
            }

            if ( modSettings.Settings.PickOrientation )
            {
                modProgram.TextureOrientation = Map.Terrain.Tiles[Tile.X, Tile.Y].Texture.Orientation;
                TextureView.DrawViewLater();
            }
        }

        public void HeightPickerL()
        {
            clsMap Map = MainMap;

            clsViewInfo.clsMouseOver.clsOverTerrain MouseOverTerrain = Map.ViewInfo.GetMouseOverTerrain();

            if ( MouseOverTerrain == null )
            {
                return;
            }

            txtHeightSetL.Text =
                modIO.InvariantToString_byte(System.Convert.ToByte(Map.Terrain.Vertices[MouseOverTerrain.Vertex.Normal.X, MouseOverTerrain.Vertex.Normal.Y].Height));
            txtHeightSetL.Focus();
            MapView.OpenGLControl.Focus();
        }

        public void HeightPickerR()
        {
            clsMap Map = MainMap;

            clsViewInfo.clsMouseOver.clsOverTerrain MouseOverTerrain = Map.ViewInfo.GetMouseOverTerrain();
            if ( MouseOverTerrain == null )
            {
                return;
            }

            txtHeightSetR.Text = modIO.InvariantToString_byte(Map.Terrain.Vertices[MouseOverTerrain.Vertex.Normal.X, MouseOverTerrain.Vertex.Normal.Y].Height);
            txtHeightSetR.Focus();
            MapView.OpenGLControl.Focus();
        }

        public void OpenGL_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
        {
            if ( e.Data.GetDataPresent(DataFormats.FileDrop) )
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        public void OpenGL_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
        {
            string[] Paths = (string[])(e.Data.GetData(DataFormats.FileDrop));
            clsResult Result = new clsResult("Loading drag-dropped maps");
            string Path = "";

            foreach ( string tempLoopVar_Path in Paths )
            {
                Path = tempLoopVar_Path;
                Result.Take(LoadMap(Path));
            }
            modProgram.ShowWarnings(Result);
        }

        public void btnFlatSelected_Click(object sender, EventArgs e)
        {
            clsMap Map = MainMap;

            if ( Map == null )
            {
                return;
            }

            clsMap.clsObjectFlattenTerrain FlattenTool = new clsMap.clsObjectFlattenTerrain();
            Map.SelectedUnits.GetItemsAsSimpleClassList().PerformTool(FlattenTool);

            Map.Update();
            Map.UndoStepCreate("Flatten Under Structures");
        }

        public void rdoFillCliffIgnore_CheckedChanged(System.Object sender, System.EventArgs e)
        {
            FillCliffAction = modProgram.enumFillCliffAction.Ignore;
        }

        public void rdoFillCliffStopBefore_CheckedChanged(System.Object sender, System.EventArgs e)
        {
            FillCliffAction = modProgram.enumFillCliffAction.StopBefore;
        }

        public void rdoFillCliffStopAfter_CheckedChanged(System.Object sender, System.EventArgs e)
        {
            FillCliffAction = modProgram.enumFillCliffAction.StopAfter;
        }

        public void btnScriptAreaCreate_Click(System.Object sender, System.EventArgs e)
        {
            if ( !btnScriptAreaCreate.Enabled )
            {
                return;
            }
            clsMap Map = MainMap;
            if ( Map == null )
            {
                return;
            }

            if ( Map.Selected_Area_VertexA == null )
            {
                MessageBox.Show("Select something first.");
                return;
            }
            if ( Map.Selected_Area_VertexB == null )
            {
                MessageBox.Show("Select something first.");
                return;
            }

            clsMap.clsScriptArea NewArea = clsMap.clsScriptArea.Create(Map);
            if ( NewArea == null )
            {
                MessageBox.Show("Error: Creating area failed.");
                return;
            }

            NewArea.SetPositions(
                new modMath.sXY_int(Map.Selected_Area_VertexA.X * modProgram.TerrainGridSpacing, Map.Selected_Area_VertexA.Y * modProgram.TerrainGridSpacing),
                new modMath.sXY_int(Map.Selected_Area_VertexB.X * modProgram.TerrainGridSpacing, Map.Selected_Area_VertexB.Y * modProgram.TerrainGridSpacing));

            ScriptMarkerLists_Update();

            Map.SetChanged(); //todo: remove if areas become undoable
            View_DrawViewLater();
        }

        private clsMap.clsScriptPosition[] lstScriptPositions_Objects = new clsMap.clsScriptPosition[0];
        private clsMap.clsScriptArea[] lstScriptAreas_Objects = new clsMap.clsScriptArea[0];

        public void ScriptMarkerLists_Update()
        {
            clsMap Map = MainMap;

            lstScriptPositions.Enabled = false;
            lstScriptAreas.Enabled = false;

            lstScriptPositions.Items.Clear();
            lstScriptAreas.Items.Clear();

            if ( Map == null )
            {
                _SelectedScriptMarker = null;
                return;
            }

            int ListPosition = 0;
            clsMap.clsScriptPosition ScriptPosition = default(clsMap.clsScriptPosition);
            clsMap.clsScriptArea ScriptArea = default(clsMap.clsScriptArea);
            object NewSelectedScriptMarker = null;

            lstScriptPositions_Objects = new clsMap.clsScriptPosition[Map.ScriptPositions.Count];
            lstScriptAreas_Objects = new clsMap.clsScriptArea[Map.ScriptAreas.Count];

            foreach ( clsMap.clsScriptPosition tempLoopVar_ScriptPosition in Map.ScriptPositions )
            {
                ScriptPosition = tempLoopVar_ScriptPosition;
                ListPosition = lstScriptPositions.Items.Add(ScriptPosition.Label);
                lstScriptPositions_Objects[ListPosition] = ScriptPosition;
                if ( ScriptPosition == _SelectedScriptMarker )
                {
                    NewSelectedScriptMarker = ScriptPosition;
                    lstScriptPositions.SelectedIndex = ListPosition;
                }
            }

            foreach ( clsMap.clsScriptArea tempLoopVar_ScriptArea in Map.ScriptAreas )
            {
                ScriptArea = tempLoopVar_ScriptArea;
                ListPosition = lstScriptAreas.Items.Add(ScriptArea.Label);
                lstScriptAreas_Objects[ListPosition] = ScriptArea;
                if ( ScriptArea == _SelectedScriptMarker )
                {
                    NewSelectedScriptMarker = ScriptArea;
                    lstScriptAreas.SelectedIndex = ListPosition;
                }
            }

            lstScriptPositions.Enabled = true;
            lstScriptAreas.Enabled = true;

            _SelectedScriptMarker = NewSelectedScriptMarker;

            SelectedScriptMarker_Update();
        }

        private object _SelectedScriptMarker;

        public object SelectedScriptMarker
        {
            get { return _SelectedScriptMarker; }
        }

        public void lstScriptPositions_SelectedIndexChanged(System.Object sender, System.EventArgs e)
        {
            if ( !lstScriptPositions.Enabled )
            {
                return;
            }

            _SelectedScriptMarker = lstScriptPositions_Objects[lstScriptPositions.SelectedIndex];

            lstScriptAreas.Enabled = false;
            lstScriptAreas.SelectedIndex = -1;
            lstScriptAreas.Enabled = true;

            SelectedScriptMarker_Update();

            View_DrawViewLater();
        }

        public void lstScriptAreas_SelectedIndexChanged(System.Object sender, System.EventArgs e)
        {
            if ( !lstScriptAreas.Enabled )
            {
                return;
            }

            _SelectedScriptMarker = lstScriptAreas_Objects[lstScriptAreas.SelectedIndex];

            lstScriptPositions.Enabled = false;
            lstScriptPositions.SelectedIndex = -1;
            lstScriptPositions.Enabled = true;

            SelectedScriptMarker_Update();

            View_DrawViewLater();
        }

        public void SelectedScriptMarker_Update()
        {
            txtScriptMarkerLabel.Enabled = false;
            txtScriptMarkerX.Enabled = false;
            txtScriptMarkerY.Enabled = false;
            txtScriptMarkerX2.Enabled = false;
            txtScriptMarkerY2.Enabled = false;
            txtScriptMarkerLabel.Text = "";
            txtScriptMarkerX.Text = "";
            txtScriptMarkerY.Text = "";
            txtScriptMarkerX2.Text = "";
            txtScriptMarkerY2.Text = "";
            if ( _SelectedScriptMarker != null )
            {
                if ( _SelectedScriptMarker is clsMap.clsScriptPosition )
                {
                    clsMap.clsScriptPosition ScriptPosition = (clsMap.clsScriptPosition)_SelectedScriptMarker;
                    txtScriptMarkerLabel.Text = ScriptPosition.Label;
                    txtScriptMarkerX.Text = modIO.InvariantToString_int(ScriptPosition.PosX);
                    txtScriptMarkerY.Text = modIO.InvariantToString_int(ScriptPosition.PosY);
                    txtScriptMarkerLabel.Enabled = true;
                    txtScriptMarkerX.Enabled = true;
                    txtScriptMarkerY.Enabled = true;
                }
                else if ( _SelectedScriptMarker is clsMap.clsScriptArea )
                {
                    clsMap.clsScriptArea ScriptArea = (clsMap.clsScriptArea)_SelectedScriptMarker;
                    txtScriptMarkerLabel.Text = ScriptArea.Label;
                    txtScriptMarkerX.Text = modIO.InvariantToString_int(ScriptArea.PosAX);
                    txtScriptMarkerY.Text = modIO.InvariantToString_int(ScriptArea.PosAY);
                    txtScriptMarkerX2.Text = modIO.InvariantToString_int(ScriptArea.PosBX);
                    txtScriptMarkerY2.Text = modIO.InvariantToString_int(ScriptArea.PosBY);
                    txtScriptMarkerLabel.Enabled = true;
                    txtScriptMarkerX.Enabled = true;
                    txtScriptMarkerY.Enabled = true;
                    txtScriptMarkerX2.Enabled = true;
                    txtScriptMarkerY2.Enabled = true;
                }
            }
        }

        public void btnScriptMarkerRemove_Click(System.Object sender, System.EventArgs e)
        {
            if ( _SelectedScriptMarker == null )
            {
                return;
            }

            clsMap Map = MainMap;
            if ( Map == null )
            {
                return;
            }

            int Number = 0;
            if ( _SelectedScriptMarker is clsMap.clsScriptPosition )
            {
                clsMap.clsScriptPosition ScripPosition = (clsMap.clsScriptPosition)_SelectedScriptMarker;
                Number = ScripPosition.ParentMap.ArrayPosition;
                ScripPosition.Deallocate();
                if ( Map.ScriptPositions.Count > 0 )
                {
                    _SelectedScriptMarker = Map.ScriptPositions[modMath.Clamp_int(Number, 0, Map.ScriptPositions.Count - 1)];
                }
                else
                {
                    _SelectedScriptMarker = null;
                }
            }
            else if ( _SelectedScriptMarker is clsMap.clsScriptArea )
            {
                clsMap.clsScriptArea ScriptArea = (clsMap.clsScriptArea)_SelectedScriptMarker;
                Number = ScriptArea.ParentMap.ArrayPosition;
                ScriptArea.Deallocate();
                if ( Map.ScriptAreas.Count > 0 )
                {
                    _SelectedScriptMarker = Map.ScriptAreas[modMath.Clamp_int(Number, 0, Map.ScriptAreas.Count - 1)];
                }
                else
                {
                    _SelectedScriptMarker = null;
                }
            }

            ScriptMarkerLists_Update();

            View_DrawViewLater();
        }

        public void txtScriptMarkerLabel_LostFocus(System.Object sender, System.EventArgs e)
        {
            if ( !txtScriptMarkerLabel.Enabled )
            {
                return;
            }
            if ( _SelectedScriptMarker == null )
            {
                return;
            }

            modProgram.sResult Result = new modProgram.sResult();
            if ( _SelectedScriptMarker is clsMap.clsScriptPosition )
            {
                clsMap.clsScriptPosition ScriptPosition = (clsMap.clsScriptPosition)_SelectedScriptMarker;
                if ( ScriptPosition.Label == txtScriptMarkerLabel.Text )
                {
                    return;
                }
                Result = ScriptPosition.SetLabel(txtScriptMarkerLabel.Text);
            }
            else if ( _SelectedScriptMarker is clsMap.clsScriptArea )
            {
                clsMap.clsScriptArea ScriptArea = (clsMap.clsScriptArea)_SelectedScriptMarker;
                if ( ScriptArea.Label == txtScriptMarkerLabel.Text )
                {
                    return;
                }
                Result = ScriptArea.SetLabel(txtScriptMarkerLabel.Text);
            }
            else
            {
                return;
            }

            if ( !Result.Success )
            {
                MessageBox.Show("Unable to change label: " + Result.Problem);
                SelectedScriptMarker_Update();
                return;
            }

            ScriptMarkerLists_Update();
        }

        public void txtScriptMarkerX_LostFocus(System.Object sender, System.EventArgs e)
        {
            if ( !txtScriptMarkerX.Enabled )
            {
                return;
            }
            if ( _SelectedScriptMarker == null )
            {
                return;
            }

            if ( _SelectedScriptMarker is clsMap.clsScriptPosition )
            {
                clsMap.clsScriptPosition ScriptPosition = (clsMap.clsScriptPosition)_SelectedScriptMarker;
                int temp_Result = ScriptPosition.PosX;
                modIO.InvariantParse_int(txtScriptMarkerX.Text, ref temp_Result);
                ScriptPosition.PosX = temp_Result;
            }
            else if ( _SelectedScriptMarker is clsMap.clsScriptArea )
            {
                clsMap.clsScriptArea ScriptArea = (clsMap.clsScriptArea)_SelectedScriptMarker;
                int temp_Result2 = ScriptArea.PosAX;
                modIO.InvariantParse_int(txtScriptMarkerX.Text, ref temp_Result2);
                ScriptArea.PosAX = temp_Result2;
            }
            else
            {
                MessageBox.Show("Error: unhandled type.");
            }

            SelectedScriptMarker_Update();
            View_DrawViewLater();
        }

        public void txtScriptMarkerY_LostFocus(System.Object sender, System.EventArgs e)
        {
            if ( !txtScriptMarkerY.Enabled )
            {
                return;
            }
            if ( _SelectedScriptMarker == null )
            {
                return;
            }

            if ( _SelectedScriptMarker is clsMap.clsScriptPosition )
            {
                clsMap.clsScriptPosition ScriptPosition = (clsMap.clsScriptPosition)_SelectedScriptMarker;
                int temp_Result = ScriptPosition.PosY;
                modIO.InvariantParse_int(txtScriptMarkerY.Text, ref temp_Result);
                ScriptPosition.PosY = temp_Result;
            }
            else if ( _SelectedScriptMarker is clsMap.clsScriptArea )
            {
                clsMap.clsScriptArea ScriptArea = (clsMap.clsScriptArea)_SelectedScriptMarker;
                int temp_Result2 = ScriptArea.PosAY;
                modIO.InvariantParse_int(txtScriptMarkerY.Text, ref temp_Result2);
                ScriptArea.PosAY = temp_Result2;
            }
            else
            {
                MessageBox.Show("Error: unhandled type.");
            }

            SelectedScriptMarker_Update();
            View_DrawViewLater();
        }

        public void txtScriptMarkerX2_LostFocus(System.Object sender, System.EventArgs e)
        {
            if ( !txtScriptMarkerX2.Enabled )
            {
                return;
            }
            if ( _SelectedScriptMarker == null )
            {
                return;
            }

            if ( _SelectedScriptMarker is clsMap.clsScriptArea )
            {
                clsMap.clsScriptArea ScriptArea = (clsMap.clsScriptArea)_SelectedScriptMarker;
                int temp_Result = ScriptArea.PosBX;
                modIO.InvariantParse_int(txtScriptMarkerX2.Text, ref temp_Result);
                ScriptArea.PosBX = temp_Result;
            }
            else
            {
                MessageBox.Show("Error: unhandled type.");
            }

            SelectedScriptMarker_Update();
            View_DrawViewLater();
        }

        public void txtScriptMarkerY2_LostFocus(System.Object sender, System.EventArgs e)
        {
            if ( !txtScriptMarkerY2.Enabled )
            {
                return;
            }
            if ( _SelectedScriptMarker == null )
            {
                return;
            }

            if ( _SelectedScriptMarker is clsMap.clsScriptArea )
            {
                clsMap.clsScriptArea ScriptArea = (clsMap.clsScriptArea)_SelectedScriptMarker;
                int temp_Result = ScriptArea.PosBY;
                modIO.InvariantParse_int(txtScriptMarkerY2.Text, ref temp_Result);
                ScriptArea.PosBY = temp_Result;
            }
            else
            {
                MessageBox.Show("Error: unhandled type.");
            }

            SelectedScriptMarker_Update();
            View_DrawViewLater();
        }

        public void txtObjectLabel_LostFocus(System.Object sender, System.EventArgs e)
        {
            if ( !txtObjectLabel.Enabled )
            {
                return;
            }
            clsMap Map = MainMap;
            if ( Map == null )
            {
                return;
            }
            if ( Map.SelectedUnits.Count != 1 )
            {
                return;
            }

            if ( txtObjectLabel.Text == Map.SelectedUnits[0].Label )
            {
                return;
            }

            clsMap.clsUnit OldUnit = Map.SelectedUnits[0];
            clsMap.clsUnit ResultUnit = new clsMap.clsUnit(OldUnit, Map);
            Map.UnitSwap(OldUnit, ResultUnit);
            modProgram.sResult Result = ResultUnit.SetLabel(txtObjectLabel.Text);
            if ( !Result.Success )
            {
                MessageBox.Show("Unable to set label: " + Result.Problem);
            }

            Map.SelectedUnits.Clear();
            ResultUnit.MapSelect();

            SelectedObject_Changed();

            Map.UndoStepCreate("Object Label Changed");
            View_DrawViewLater();
        }

        public void NewMainMap(clsMap NewMap)
        {
            NewMap.frmMainLink.Connect(_LoadedMaps);
            SetMainMap(NewMap);
        }

        public clsMap MainMap
        {
            get { return _LoadedMaps.MainMap; }
        }

        public clsMaps LoadedMaps
        {
            get { return _LoadedMaps; }
        }

        public void SetMainMap(clsMap Map)
        {
            _LoadedMaps.MainMap = Map;
        }

        public clsResult LoadMap(string Path)
        {
            clsResult ReturnResult = new clsResult("");
            modProgram.sSplitPath SplitPath = new modProgram.sSplitPath(Path);
            clsMap ResultMap = new clsMap();
            string Extension = SplitPath.FileExtension.ToLower();

            switch ( Extension )
            {
                case "fmap":
                    ReturnResult.Add(ResultMap.Load_FMap(Path));
                    ResultMap.PathInfo = new clsMap.clsPathInfo(Path, true);
                    break;
                case "fme":
                    ReturnResult.Add(ResultMap.Load_FME(Path));
                    ResultMap.PathInfo = new clsMap.clsPathInfo(Path, false);
                    break;
                case "wz":
                    ReturnResult.Add(ResultMap.Load_WZ(Path));
                    ResultMap.PathInfo = new clsMap.clsPathInfo(Path, false);
                    break;
                case "gam":
                    ReturnResult.Add(ResultMap.Load_Game(Path));
                    ResultMap.PathInfo = new clsMap.clsPathInfo(Path, false);
                    break;
                case "lnd":
                    ReturnResult.Add(ResultMap.Load_LND(Path));
                    ResultMap.PathInfo = new clsMap.clsPathInfo(Path, false);
                    break;
                default:
                    ReturnResult.ProblemAdd("File extension not recognised.");
                    break;
            }

            if ( ReturnResult.HasProblems )
            {
                ResultMap.Deallocate();
            }
            else
            {
                NewMainMap(ResultMap);
            }

            return ReturnResult;
        }

        public void Load_Autosave_Prompt()
        {
            if ( !System.IO.Directory.Exists(modProgram.AutoSavePath) )
            {
                Interaction.MsgBox("Autosave directory does not exist. There are no autosaves.", MsgBoxStyle.OkOnly, "");
                return;
            }
            OpenFileDialog Dialog = new OpenFileDialog();

            Dialog.FileName = "";
            Dialog.Filter = modProgram.ProgramName + " Files (*.fmap, *.fme)|*.fmap;*.fme|All Files (*.*)|*.*";
            Dialog.InitialDirectory = modProgram.AutoSavePath;
            if ( Dialog.ShowDialog(this) != System.Windows.Forms.DialogResult.OK )
            {
                return;
            }
            modSettings.Settings.OpenPath = System.IO.Path.GetDirectoryName(Dialog.FileName);
            clsResult Result = new clsResult("Loading map");
            Result.Take(LoadMap(Dialog.FileName));
            modProgram.ShowWarnings(Result);
        }

        public void btnAlignObjects_Click(System.Object sender, System.EventArgs e)
        {
            clsMap Map = MainMap;

            if ( Map == null )
            {
                return;
            }

            clsMap.clsObjectAlignment AlignTool = new clsMap.clsObjectAlignment();
            AlignTool.Map = Map;
            Map.SelectedUnits.GetItemsAsSimpleList().PerformTool(AlignTool);

            Map.Update();
            Map.UndoStepCreate("Align Objects");
        }

        public void cbxDesignableOnly_CheckedChanged(System.Object sender, System.EventArgs e)
        {
            Components_Update();
        }

        private void ObjectsUpdate()
        {
            if ( !modProgram.ProgramInitializeFinished )
            {
                return;
            }

            switch ( TabControl1.SelectedIndex )
            {
                case 0:
                    ObjectListFill<clsFeatureType>(modProgram.ObjectData.FeatureTypes.GetItemsAsSimpleList(), dgvFeatures);
                    break;
                case 1:
                    ObjectListFill<clsStructureType>(modProgram.ObjectData.StructureTypes.GetItemsAsSimpleList(), dgvStructures);
                    break;
                case 2:
                    ObjectListFill<clsDroidTemplate>(modProgram.ObjectData.DroidTemplates.GetItemsAsSimpleList(), dgvDroids);
                    break;
            }
        }

        public void txtObjectFind_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if ( !txtObjectFind.Enabled )
            {
                return;
            }

            if ( e.KeyCode != Keys.Enter )
            {
                return;
            }

            txtObjectFind.Enabled = false;

            ObjectsUpdate();

            txtObjectFind.Enabled = true;
        }

        public void txtObjectFind_Leave(object sender, System.EventArgs e)
        {
            if ( !txtObjectFind.Enabled )
            {
                return;
            }

            txtObjectFind.Enabled = false;

            ObjectsUpdate();

            txtObjectFind.Enabled = true;
        }

        public void rdoObjectPlace_Click(System.Object sender, System.EventArgs e)
        {
            modTools.Tool = modTools.Tools.ObjectPlace;
        }

        public void rdoObjectLine_Click(System.Object sender, System.EventArgs e)
        {
            modTools.Tool = modTools.Tools.ObjectLines;
        }

        private void rdoObjectsSortNone_Click(System.Object sender, System.EventArgs e)
        {
            if ( !modProgram.ProgramInitializeFinished )
            {
                return;
            }

            ObjectsUpdate();
        }

        private void rdoObjectsSortInternal_Click(System.Object sender, System.EventArgs e)
        {
            if ( !modProgram.ProgramInitializeFinished )
            {
                return;
            }

            ObjectsUpdate();
        }

        private void rdoObjectsSortInGame_Click(System.Object sender, System.EventArgs e)
        {
            if ( !modProgram.ProgramInitializeFinished )
            {
                return;
            }

            ObjectsUpdate();
        }

        private void ActivateObjectTool()
        {
            if ( rdoObjectPlace.Checked )
            {
                modTools.Tool = modTools.Tools.ObjectPlace;
            }
            else if ( rdoObjectLines.Checked )
            {
                modTools.Tool = modTools.Tools.ObjectLines;
            }
        }

        public void TabControl1_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            ActivateObjectTool();
            ObjectsUpdate();
        }

        public void GeneratorToolStripMenuItem_Click(System.Object sender, System.EventArgs e)
        {
            modMain.frmGeneratorInstance.Show();
            modMain.frmGeneratorInstance.Activate();
        }

        public void ReinterpretTerrainToolStripMenuItem_Click(System.Object sender, System.EventArgs e)
        {
            clsMap Map = MainMap;

            if ( Map == null )
            {
                return;
            }

            Map.TerrainInterpretChanges.SetAllChanged();

            Map.Update();

            Map.UndoStepCreate("Interpret Terrain");
        }

        public void WaterTriangleCorrectionToolStripMenuItem_Click(System.Object sender, System.EventArgs e)
        {
            clsMap Map = MainMap;

            if ( Map == null )
            {
                return;
            }

            Map.WaterTriCorrection();

            Map.Update();

            Map.UndoStepCreate("Water Triangle Correction");

            View_DrawViewLater();
        }

        public void ToolStripMenuItem5_Click(System.Object sender, System.EventArgs e)
        {
            clsMap Map = MainMap;

            if ( Map == null )
            {
                return;
            }

            if ( modGenerator.UnitType_OilResource == null )
            {
                MessageBox.Show("Unable. Oil resource is not loaded.");
                return;
            }

            modLists.SimpleClassList<clsMap.clsUnit> OilList = new modLists.SimpleClassList<clsMap.clsUnit>();
            clsMap.clsUnit Unit = default(clsMap.clsUnit);
            foreach ( clsMap.clsUnit tempLoopVar_Unit in Map.Units )
            {
                Unit = tempLoopVar_Unit;
                if ( Unit.Type == modGenerator.UnitType_OilResource )
                {
                    OilList.Add(Unit);
                }
            }
            clsMap.clsObjectFlattenTerrain FlattenTool = new clsMap.clsObjectFlattenTerrain();
            OilList.PerformTool(FlattenTool);

            Map.Update();
            Map.UndoStepCreate("Flatten Under Oil");
        }

        public void ToolStripMenuItem6_Click(System.Object sender, System.EventArgs e)
        {
            clsMap Map = MainMap;

            if ( Map == null )
            {
                return;
            }

            modLists.SimpleClassList<clsMap.clsUnit> StructureList = new modLists.SimpleClassList<clsMap.clsUnit>();
            clsMap.clsUnit Unit = default(clsMap.clsUnit);
            foreach ( clsMap.clsUnit tempLoopVar_Unit in Map.Units )
            {
                Unit = tempLoopVar_Unit;
                if ( Unit.Type.Type == clsUnitType.enumType.PlayerStructure )
                {
                    StructureList.Add(Unit);
                }
            }
            clsMap.clsObjectFlattenTerrain FlattenTool = new clsMap.clsObjectFlattenTerrain();
            StructureList.PerformTool(FlattenTool);

            Map.Update();
            Map.UndoStepCreate("Flatten Under Structures");
        }

        public void btnObjectTypeSelect_Click(System.Object sender, System.EventArgs e)
        {
            clsMap Map = MainMap;

            if ( Map == null )
            {
                return;
            }

            if ( !modControls.KeyboardProfile.Active(modControls.Control_Unit_Multiselect) )
            {
                Map.SelectedUnits.Clear();
            }
            foreach ( clsMap.clsUnit Unit in Map.Units )
            {
                if ( Unit.Type.UnitType_frmMainSelectedLink.IsConnected )
                {
                    if ( !Unit.MapSelectedUnitLink.IsConnected )
                    {
                        Unit.MapSelectedUnitLink.Connect(Map.SelectedUnits);
                    }
                }
            }

            View_DrawViewLater();
        }

        public clsUnitType SingleSelectedObjectType
        {
            get
            {
                if ( SelectedObjectTypes.Count == 1 )
                {
                    return SelectedObjectTypes[0];
                }
                else
                {
                    return null;
                }
            }
        }

        private void ObjectTypeSelectionUpdate(DataGridView dataView)
        {
            SelectedObjectTypes.Clear();
            foreach ( DataGridViewRow selection in dataView.SelectedRows )
            {
                clsUnitType objectType = (clsUnitType)(selection.Cells[0].Value);
                if ( !objectType.UnitType_frmMainSelectedLink.IsConnected )
                {
                    SelectedObjectTypes.Add(objectType.UnitType_frmMainSelectedLink);
                }
            }
            clsMap Map = MainMap;
            if ( Map != null )
            {
                Map.MinimapMakeLater(); //for unit highlight
                View_DrawViewLater();
            }
        }

        public void dgvFeatures_SelectionChanged(object sender, System.EventArgs e)
        {
            ActivateObjectTool();
            ObjectTypeSelectionUpdate(dgvFeatures);
        }

        public void dgvStructures_SelectionChanged(object sender, System.EventArgs e)
        {
            ActivateObjectTool();
            ObjectTypeSelectionUpdate(dgvStructures);
        }

        public void dgvDroids_SelectionChanged(object sender, System.EventArgs e)
        {
            ActivateObjectTool();
            ObjectTypeSelectionUpdate(dgvDroids);
        }
    }
}