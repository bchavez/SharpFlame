namespace SharpFlame.Old
{
	partial class frmMain : System.Windows.Forms.Form
	{
		
		//Form overrides dispose to clean up the component list.
		[System.Diagnostics.DebuggerNonUserCode()]protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing && components != null)
				{
					components.Dispose();
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}
		
		//Required by the Windows Form Designer
		private System.ComponentModel.Container components = null;
		
		//NOTE: The following procedure is required by the Windows Form Designer
		//It can be modified using the Windows Form Designer.
		//Do not modify it using the code editor.
		
		[System.Diagnostics.DebuggerStepThrough()]private void InitializeComponent()
		{
			this.SplitContainer1 = new System.Windows.Forms.SplitContainer();
			base.Leave += Me_LostFocus;
			this.TabControl = new System.Windows.Forms.TabControl();
			this.TabControl.SelectedIndexChanged += this.TabControl_SelectedIndexChanged;
			this.tpTextures = new System.Windows.Forms.TabPage();
			this.TableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
			this.Panel5 = new System.Windows.Forms.Panel();
			this.rdoTextureRemoveTerrain = new System.Windows.Forms.RadioButton();
			this.rdoTextureRemoveTerrain.Click += this.rdoTextureRemoveTerrain_Click;
			this.rdoTextureReinterpretTerrain = new System.Windows.Forms.RadioButton();
			this.rdoTextureReinterpretTerrain.Click += this.rdoTextureReinterpretTerrain_Click;
			this.rdoTextureIgnoreTerrain = new System.Windows.Forms.RadioButton();
			this.rdoTextureIgnoreTerrain.Click += this.rdoTextureIgnoreTerrain_Click;
			this.pnlTextureBrush = new System.Windows.Forms.Panel();
			this.chkTextureOrientationRandomize = new System.Windows.Forms.CheckBox();
			this.btnTextureFlipX = new System.Windows.Forms.Button();
			this.btnTextureFlipX.Click += this.btnTextureFlipX_Click;
			this.btnTextureClockwise = new System.Windows.Forms.Button();
			this.btnTextureClockwise.Click += this.btnTextureClockwise_Click;
			this.btnTextureAnticlockwise = new System.Windows.Forms.Button();
			this.btnTextureAnticlockwise.Click += this.btnTextureAnticlockwise_Click;
			this.chkSetTextureOrientation = new System.Windows.Forms.CheckBox();
			this.chkSetTexture = new System.Windows.Forms.CheckBox();
			this.Label21 = new System.Windows.Forms.Label();
			this.cboTileset = new System.Windows.Forms.ComboBox();
			this.cboTileset.SelectedIndexChanged += this.cboTileset_SelectedIndexChanged;
			this.Panel6 = new System.Windows.Forms.Panel();
			this.cbxTileNumbers = new System.Windows.Forms.CheckBox();
			this.cbxTileNumbers.CheckedChanged += this.chkTileNumbers_CheckedChanged;
			this.cbxTileTypes = new System.Windows.Forms.CheckBox();
			this.cbxTileTypes.CheckedChanged += this.chkTileTypes_CheckedChanged;
			this.Label20 = new System.Windows.Forms.Label();
			this.cboTileType = new System.Windows.Forms.ComboBox();
			this.cboTileType.SelectedIndexChanged += this.cmbTileType_SelectedIndexChanged;
			this.tpAutoTexture = new System.Windows.Forms.TabPage();
			this.Panel15 = new System.Windows.Forms.Panel();
			this.cbxFillInside = new System.Windows.Forms.CheckBox();
			this.rdoFillCliffIgnore = new System.Windows.Forms.RadioButton();
			this.rdoFillCliffIgnore.CheckedChanged += this.rdoFillCliffIgnore_CheckedChanged;
			this.rdoFillCliffStopBefore = new System.Windows.Forms.RadioButton();
			this.rdoFillCliffStopBefore.CheckedChanged += this.rdoFillCliffStopBefore_CheckedChanged;
			this.rdoFillCliffStopAfter = new System.Windows.Forms.RadioButton();
			this.rdoFillCliffStopAfter.CheckedChanged += this.rdoFillCliffStopAfter_CheckedChanged;
			this.rdoCliffTriBrush = new System.Windows.Forms.RadioButton();
			this.rdoCliffTriBrush.CheckedChanged += this.rdoCliffTriBrush_CheckedChanged;
			this.rdoRoadRemove = new System.Windows.Forms.RadioButton();
			this.rdoRoadRemove.Click += this.rdoRoadRemove_Click;
			this.pnlCliffRemoveBrush = new System.Windows.Forms.Panel();
			this.pnlTerrainBrush = new System.Windows.Forms.Panel();
			this.cbxInvalidTiles = new System.Windows.Forms.CheckBox();
			this.cbxAutoTexSetHeight = new System.Windows.Forms.CheckBox();
			this.cbxCliffTris = new System.Windows.Forms.CheckBox();
			this.Label29 = new System.Windows.Forms.Label();
			this.rdoAutoRoadLine = new System.Windows.Forms.RadioButton();
			this.rdoAutoRoadLine.Click += this.rdoAutoRoadLine_Click;
			this.btnAutoTextureRemove = new System.Windows.Forms.Button();
			this.btnAutoTextureRemove.Click += this.btnAutoTextureRemove_Click;
			this.btnAutoRoadRemove = new System.Windows.Forms.Button();
			this.btnAutoRoadRemove.Click += this.btnAutoRoadRemove_Click;
			this.rdoAutoRoadPlace = new System.Windows.Forms.RadioButton();
			this.rdoAutoRoadPlace.Click += this.rdoAutoRoadPlace_Click;
			this.lstAutoRoad = new System.Windows.Forms.ListBox();
			this.lstAutoRoad.Click += this.lstAutoRoad_Click;
			this.lstAutoRoad.SelectedIndexChanged += this.lstAutoRoad_SelectedIndexChanged;
			this.rdoAutoTexturePlace = new System.Windows.Forms.RadioButton();
			this.rdoAutoTexturePlace.Click += this.rdoAutoTexturePlace_Click;
			this.rdoAutoTextureFill = new System.Windows.Forms.RadioButton();
			this.rdoAutoTextureFill.Click += this.rdoAutoTextureFill_CheckedChanged;
			this.rdoAutoCliffBrush = new System.Windows.Forms.RadioButton();
			this.rdoAutoCliffBrush.Click += this.rdoAutoCliffBrush_Click;
			this.rdoAutoCliffRemove = new System.Windows.Forms.RadioButton();
			this.rdoAutoCliffRemove.Click += this.rdoAutoCliffRemove_Click;
			this.txtAutoCliffSlope = new System.Windows.Forms.TextBox();
			this.Label1 = new System.Windows.Forms.Label();
			this.lstAutoTexture = new System.Windows.Forms.ListBox();
			this.lstAutoTexture.Click += this.lstAutoTexture_SelectedIndexChanged;
			this.lstAutoTexture.SelectedIndexChanged += this.lstAutoTexture_SelectedIndexChanged_1;
			this.Label3 = new System.Windows.Forms.Label();
			this.tpHeight = new System.Windows.Forms.TabPage();
			this.cbxHeightChangeFade = new System.Windows.Forms.CheckBox();
			this.pnlHeightSetBrush = new System.Windows.Forms.Panel();
			this.btnHeightsMultiplySelection = new System.Windows.Forms.Button();
			this.btnHeightsMultiplySelection.Click += this.btnHeightsMultiplySelection_Click;
			this.btnHeightOffsetSelection = new System.Windows.Forms.Button();
			this.btnHeightOffsetSelection.Click += this.btnHeightOffsetSelection_Click;
			this.tabHeightSetR = new System.Windows.Forms.TabControl();
			this.tabHeightSetR.SelectedIndexChanged += this.tabHeightSetR_SelectedIndexChanged;
			this.TabPage25 = new System.Windows.Forms.TabPage();
			this.TabPage26 = new System.Windows.Forms.TabPage();
			this.TabPage27 = new System.Windows.Forms.TabPage();
			this.TabPage28 = new System.Windows.Forms.TabPage();
			this.TabPage29 = new System.Windows.Forms.TabPage();
			this.TabPage30 = new System.Windows.Forms.TabPage();
			this.TabPage31 = new System.Windows.Forms.TabPage();
			this.TabPage32 = new System.Windows.Forms.TabPage();
			this.tabHeightSetL = new System.Windows.Forms.TabControl();
			this.tabHeightSetL.SelectedIndexChanged += this.tabHeightSetL_SelectedIndexChanged;
			this.TabPage9 = new System.Windows.Forms.TabPage();
			this.TabPage10 = new System.Windows.Forms.TabPage();
			this.TabPage11 = new System.Windows.Forms.TabPage();
			this.TabPage12 = new System.Windows.Forms.TabPage();
			this.TabPage17 = new System.Windows.Forms.TabPage();
			this.TabPage18 = new System.Windows.Forms.TabPage();
			this.TabPage19 = new System.Windows.Forms.TabPage();
			this.TabPage20 = new System.Windows.Forms.TabPage();
			this.txtHeightSetR = new System.Windows.Forms.TextBox();
			this.txtHeightSetR.Leave += this.txtHeightSetR_LostFocus;
			this.Label27 = new System.Windows.Forms.Label();
			this.Label10 = new System.Windows.Forms.Label();
			this.txtHeightOffset = new System.Windows.Forms.TextBox();
			this.Label9 = new System.Windows.Forms.Label();
			this.txtHeightMultiply = new System.Windows.Forms.TextBox();
			this.txtHeightChangeRate = new System.Windows.Forms.TextBox();
			this.Label18 = new System.Windows.Forms.Label();
			this.rdoHeightChange = new System.Windows.Forms.RadioButton();
			this.rdoHeightChange.Click += this.rdoHeightChange_CheckedChanged;
			this.Label16 = new System.Windows.Forms.Label();
			this.txtSmoothRate = new System.Windows.Forms.TextBox();
			this.Label6 = new System.Windows.Forms.Label();
			this.rdoHeightSmooth = new System.Windows.Forms.RadioButton();
			this.rdoHeightSmooth.Click += this.rdoHeightSmooth_CheckedChanged;
			this.rdoHeightSet = new System.Windows.Forms.RadioButton();
			this.rdoHeightSet.Click += this.rdoHeightSet_CheckedChanged;
			this.txtHeightSetL = new System.Windows.Forms.TextBox();
			this.txtHeightSetL.Leave += this.txtHeightSetL_LostFocus;
			this.Label5 = new System.Windows.Forms.Label();
			this.tpResize = new System.Windows.Forms.TabPage();
			this.btnSelResize = new System.Windows.Forms.Button();
			this.btnSelResize.Click += this.btnSelResize_Click;
			this.btnResize = new System.Windows.Forms.Button();
			this.btnResize.Click += this.btnResize_Click;
			this.txtOffsetY = new System.Windows.Forms.TextBox();
			this.Label15 = new System.Windows.Forms.Label();
			this.txtOffsetX = new System.Windows.Forms.TextBox();
			this.Label14 = new System.Windows.Forms.Label();
			this.txtSizeY = new System.Windows.Forms.TextBox();
			this.Label13 = new System.Windows.Forms.Label();
			this.txtSizeX = new System.Windows.Forms.TextBox();
			this.Label12 = new System.Windows.Forms.Label();
			this.tpObjects = new System.Windows.Forms.TabPage();
			this.TableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.Panel1 = new System.Windows.Forms.Panel();
			this.btnPlayerSelectObjects = new System.Windows.Forms.Button();
			this.btnPlayerSelectObjects.Click += this.btnPlayerSelectObjects_Click;
			this.Label44 = new System.Windows.Forms.Label();
			this.GroupBox3 = new System.Windows.Forms.GroupBox();
			this.rdoObjectPlace = new System.Windows.Forms.RadioButton();
			this.rdoObjectPlace.Click += this.rdoObjectPlace_Click;
			this.rdoObjectLines = new System.Windows.Forms.RadioButton();
			this.rdoObjectLines.Click += this.rdoObjectLine_Click;
			this.txtObjectFind = new System.Windows.Forms.TextBox();
			this.txtObjectFind.KeyDown += this.txtObjectFind_KeyDown;
			this.txtObjectFind.Leave += this.txtObjectFind_Leave;
			this.cbxFootprintRotate = new System.Windows.Forms.CheckBox();
			this.txtNewObjectRotation = new System.Windows.Forms.TextBox();
			this.Label19 = new System.Windows.Forms.Label();
			this.cbxAutoWalls = new System.Windows.Forms.CheckBox();
			this.cbxObjectRandomRotation = new System.Windows.Forms.CheckBox();
			this.Label32 = new System.Windows.Forms.Label();
			this.Label22 = new System.Windows.Forms.Label();
			this.Panel2 = new System.Windows.Forms.Panel();
			this.btnObjectTypeSelect = new System.Windows.Forms.Button();
			this.btnObjectTypeSelect.Click += this.btnObjectTypeSelect_Click;
			this.TabControl1 = new System.Windows.Forms.TabControl();
			this.TabControl1.SelectedIndexChanged += this.TabControl1_SelectedIndexChanged;
			this.TabPage1 = new System.Windows.Forms.TabPage();
			this.dgvFeatures = new System.Windows.Forms.DataGridView();
			this.dgvFeatures.Click += this.dgvFeatures_SelectionChanged;
			this.TabPage2 = new System.Windows.Forms.TabPage();
			this.dgvStructures = new System.Windows.Forms.DataGridView();
			this.dgvStructures.Click += this.dgvStructures_SelectionChanged;
			this.TabPage3 = new System.Windows.Forms.TabPage();
			this.dgvDroids = new System.Windows.Forms.DataGridView();
			this.dgvDroids.Click += this.dgvDroids_SelectionChanged;
			this.tpObject = new System.Windows.Forms.TabPage();
			this.TableLayoutPanel8 = new System.Windows.Forms.TableLayoutPanel();
			this.TableLayoutPanel9 = new System.Windows.Forms.TableLayoutPanel();
			this.cboDroidTurret3 = new System.Windows.Forms.ComboBox();
			this.cboDroidTurret3.SelectedIndexChanged += this.cboDroidTurret3_SelectedIndexChanged;
			this.cboDroidTurret2 = new System.Windows.Forms.ComboBox();
			this.cboDroidTurret2.SelectedIndexChanged += this.cboDroidTurret2_SelectedIndexChanged;
			this.Panel13 = new System.Windows.Forms.Panel();
			this.rdoDroidTurret3 = new System.Windows.Forms.RadioButton();
			this.rdoDroidTurret3.CheckedChanged += this.rdoDroidTurret3_CheckedChanged;
			this.cboDroidTurret1 = new System.Windows.Forms.ComboBox();
			this.cboDroidTurret1.SelectedIndexChanged += this.cboDroidTurret1_SelectedIndexChanged;
			this.cboDroidPropulsion = new System.Windows.Forms.ComboBox();
			this.cboDroidPropulsion.SelectedIndexChanged += this.cboDroidPropulsion_SelectedIndexChanged;
			this.cboDroidBody = new System.Windows.Forms.ComboBox();
			this.cboDroidBody.SelectedIndexChanged += this.cboDroidBody_SelectedIndexChanged;
			this.cboDroidType = new System.Windows.Forms.ComboBox();
			this.cboDroidType.SelectedIndexChanged += this.cboDroidType_SelectedIndexChanged;
			this.Panel12 = new System.Windows.Forms.Panel();
			this.rdoDroidTurret0 = new System.Windows.Forms.RadioButton();
			this.rdoDroidTurret0.CheckedChanged += this.rdoDroidTurret0_CheckedChanged;
			this.rdoDroidTurret2 = new System.Windows.Forms.RadioButton();
			this.rdoDroidTurret2.CheckedChanged += this.rdoDroidTurret2_CheckedChanged;
			this.Panel11 = new System.Windows.Forms.Panel();
			this.Label39 = new System.Windows.Forms.Label();
			this.rdoDroidTurret1 = new System.Windows.Forms.RadioButton();
			this.rdoDroidTurret1.CheckedChanged += this.rdoDroidTurret1_CheckedChanged;
			this.Panel10 = new System.Windows.Forms.Panel();
			this.Label38 = new System.Windows.Forms.Label();
			this.Panel9 = new System.Windows.Forms.Panel();
			this.Label37 = new System.Windows.Forms.Label();
			this.Panel8 = new System.Windows.Forms.Panel();
			this.Label40 = new System.Windows.Forms.Label();
			this.Panel14 = new System.Windows.Forms.Panel();
			this.btnFlatSelected = new System.Windows.Forms.Button();
			this.btnFlatSelected.Click += this.btnFlatSelected_Click;
			this.btnAlignObjects = new System.Windows.Forms.Button();
			this.btnAlignObjects.Click += this.btnAlignObjects_Click;
			this.Label31 = new System.Windows.Forms.Label();
			this.Label30 = new System.Windows.Forms.Label();
			this.cbxDesignableOnly = new System.Windows.Forms.CheckBox();
			this.cbxDesignableOnly.CheckedChanged += this.cbxDesignableOnly_CheckedChanged;
			this.Label17 = new System.Windows.Forms.Label();
			this.txtObjectLabel = new System.Windows.Forms.TextBox();
			this.txtObjectLabel.Leave += this.txtObjectLabel_LostFocus;
			this.Label35 = new System.Windows.Forms.Label();
			this.btnDroidToDesign = new System.Windows.Forms.Button();
			this.btnDroidToDesign.Click += this.btnDroidToDesign_Click;
			this.Label24 = new System.Windows.Forms.Label();
			this.lblObjectType = new System.Windows.Forms.Label();
			this.Label36 = new System.Windows.Forms.Label();
			this.Label23 = new System.Windows.Forms.Label();
			this.txtObjectHealth = new System.Windows.Forms.TextBox();
			this.txtObjectHealth.Leave += this.txtObjectHealth_LostFocus;
			this.txtObjectRotation = new System.Windows.Forms.TextBox();
			this.txtObjectRotation.Leave += this.txtObjectRotation_LostFocus;
			this.Label34 = new System.Windows.Forms.Label();
			this.Label28 = new System.Windows.Forms.Label();
			this.txtObjectPriority = new System.Windows.Forms.TextBox();
			this.txtObjectPriority.Leave += this.txtObjectPriority_LostFocus;
			this.Label25 = new System.Windows.Forms.Label();
			this.Label33 = new System.Windows.Forms.Label();
			this.txtObjectID = new System.Windows.Forms.TextBox();
			this.Label26 = new System.Windows.Forms.Label();
			this.tpLabels = new System.Windows.Forms.TabPage();
			this.Label11 = new System.Windows.Forms.Label();
			this.Label43 = new System.Windows.Forms.Label();
			this.Label42 = new System.Windows.Forms.Label();
			this.lstScriptAreas = new System.Windows.Forms.ListBox();
			this.lstScriptAreas.SelectedIndexChanged += this.lstScriptAreas_SelectedIndexChanged;
			this.lstScriptPositions = new System.Windows.Forms.ListBox();
			this.lstScriptPositions.SelectedIndexChanged += this.lstScriptPositions_SelectedIndexChanged;
			this.btnScriptAreaCreate = new System.Windows.Forms.Button();
			this.btnScriptAreaCreate.Click += this.btnScriptAreaCreate_Click;
			this.GroupBox1 = new System.Windows.Forms.GroupBox();
			this.txtScriptMarkerLabel = new System.Windows.Forms.TextBox();
			this.txtScriptMarkerLabel.Leave += this.txtScriptMarkerLabel_LostFocus;
			this.Label41 = new System.Windows.Forms.Label();
			this.btnScriptMarkerRemove = new System.Windows.Forms.Button();
			this.btnScriptMarkerRemove.Click += this.btnScriptMarkerRemove_Click;
			this.Label2 = new System.Windows.Forms.Label();
			this.txtScriptMarkerY = new System.Windows.Forms.TextBox();
			this.txtScriptMarkerY.Leave += this.txtScriptMarkerY_LostFocus;
			this.txtScriptMarkerX = new System.Windows.Forms.TextBox();
			this.txtScriptMarkerX.Leave += this.txtScriptMarkerX_LostFocus;
			this.Label7 = new System.Windows.Forms.Label();
			this.txtScriptMarkerY2 = new System.Windows.Forms.TextBox();
			this.txtScriptMarkerY2.Leave += this.txtScriptMarkerY2_LostFocus;
			this.Label4 = new System.Windows.Forms.Label();
			this.Label8 = new System.Windows.Forms.Label();
			this.txtScriptMarkerX2 = new System.Windows.Forms.TextBox();
			this.txtScriptMarkerX2.Leave += this.txtScriptMarkerX2_LostFocus;
			this.TableLayoutPanel7 = new System.Windows.Forms.TableLayoutPanel();
			this.Panel7 = new System.Windows.Forms.Panel();
			this.tsTools = new System.Windows.Forms.ToolStrip();
			this.tsbGateways = new System.Windows.Forms.ToolStripButton();
			this.tsbGateways.Click += this.tsbGateways_Click;
			this.tsbDrawAutotexture = new System.Windows.Forms.ToolStripButton();
			this.tsbDrawAutotexture.Click += this.tsbDrawAutotexture_Click;
			this.tsbDrawTileOrientation = new System.Windows.Forms.ToolStripButton();
			this.tsbDrawTileOrientation.Click += this.tsbDrawTileOrientation_Click;
			this.tsFile = new System.Windows.Forms.ToolStrip();
			this.tsbSave = new System.Windows.Forms.ToolStripButton();
			this.tsbSave.Click += this.tsbSave_Click;
			this.tsSelection = new System.Windows.Forms.ToolStrip();
			this.ToolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
			this.tsbSelection = new System.Windows.Forms.ToolStripButton();
			this.tsbSelection.Click += this.tsbSelection_Click;
			this.tsbSelectionCopy = new System.Windows.Forms.ToolStripButton();
			this.tsbSelectionCopy.Click += this.tsbSelectionCopy_Click;
			this.tsbSelectionPasteOptions = new System.Windows.Forms.ToolStripDropDownButton();
			this.menuRotateUnits = new System.Windows.Forms.ToolStripMenuItem();
			this.menuRotateUnits.Click += this.menuRotateUnits_Click;
			this.menuRotateWalls = new System.Windows.Forms.ToolStripMenuItem();
			this.menuRotateWalls.Click += this.menuRotateWalls_Click;
			this.menuRotateNothing = new System.Windows.Forms.ToolStripMenuItem();
			this.menuRotateNothing.Click += this.menuRotateNothing_Click;
			this.ToolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
			this.menuSelPasteHeights = new System.Windows.Forms.ToolStripMenuItem();
			this.menuSelPasteTextures = new System.Windows.Forms.ToolStripMenuItem();
			this.menuSelPasteUnits = new System.Windows.Forms.ToolStripMenuItem();
			this.menuSelPasteGateways = new System.Windows.Forms.ToolStripMenuItem();
			this.menuSelPasteDeleteUnits = new System.Windows.Forms.ToolStripMenuItem();
			this.menuSelPasteDeleteGateways = new System.Windows.Forms.ToolStripMenuItem();
			this.tsbSelectionPaste = new System.Windows.Forms.ToolStripButton();
			this.tsbSelectionPaste.Click += this.tsbSelectionPaste_Click;
			this.tsbSelectionRotateCounterClockwise = new System.Windows.Forms.ToolStripButton();
			this.tsbSelectionRotateCounterClockwise.Click += this.tsbSelectionRotateAnticlockwise_Click;
			this.tsbSelectionRotateClockwise = new System.Windows.Forms.ToolStripButton();
			this.tsbSelectionRotateClockwise.Click += this.tsbSelectionRotateClockwise_Click;
			this.tsbSelectionFlipX = new System.Windows.Forms.ToolStripButton();
			this.tsbSelectionFlipX.Click += this.tsbSelectionFlipX_Click;
			this.tsbSelectionObjects = new System.Windows.Forms.ToolStripButton();
			this.tsbSelectionObjects.Click += this.tsbSelectionObjects_Click;
			this.tsMinimap = new System.Windows.Forms.ToolStrip();
			this.menuMinimap = new System.Windows.Forms.ToolStripDropDownButton();
			this.menuMiniShowTex = new System.Windows.Forms.ToolStripMenuItem();
			this.menuMiniShowTex.Click += this.menuMiniShowTex_Click;
			this.menuMiniShowHeight = new System.Windows.Forms.ToolStripMenuItem();
			this.menuMiniShowHeight.Click += this.menuMiniShowHeight_Click;
			this.menuMiniShowCliffs = new System.Windows.Forms.ToolStripMenuItem();
			this.menuMiniShowCliffs.Click += this.menuMiniShowCliffs_Click;
			this.menuMiniShowUnits = new System.Windows.Forms.ToolStripMenuItem();
			this.menuMiniShowUnits.Click += this.menuMiniShowUnits_Click;
			this.menuMiniShowGateways = new System.Windows.Forms.ToolStripMenuItem();
			this.menuMiniShowGateways.Click += this.menuMiniShowGateways_Click;
			this.pnlView = new System.Windows.Forms.Panel();
			this.menuMain = new System.Windows.Forms.MenuStrip();
			this.menuFile = new System.Windows.Forms.ToolStripMenuItem();
			this.NewMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.NewMapToolStripMenuItem.Click += this.NewMapToolStripMenuItem_Click;
			this.ToolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.OpenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.OpenToolStripMenuItem.Click += this.OpenToolStripMenuItem_Click;
			this.ToolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.SaveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.menuSaveFMap = new System.Windows.Forms.ToolStripMenuItem();
			this.menuSaveFMap.Click += this.FMapToolStripMenuItem_Click;
			this.ToolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
			this.menuSaveFMapQuick = new System.Windows.Forms.ToolStripMenuItem();
			this.menuSaveFMapQuick.Click += this.menuSaveFMapQuick_Click;
			this.ToolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
			this.MapLNDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.MapLNDToolStripMenuItem.Click += this.LNDToolStripMenuItem1_Click;
			this.ToolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
			this.menuExportMapTileTypes = new System.Windows.Forms.ToolStripMenuItem();
			this.menuExportMapTileTypes.Click += this.menuExportMapTileTypes_Click;
			this.ToolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.MinimapBMPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.MinimapBMPToolStripMenuItem.Click += this.MinimapBMPToolStripMenuItem_Click;
			this.ToolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
			this.ToolStripMenuItem3.Click += this.ToolStripMenuItem3_Click;
			this.ToolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.ToolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
			this.ImportHeightmapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ImportHeightmapToolStripMenuItem.Click += this.ImportHeightmapToolStripMenuItem_Click;
			this.ToolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
			this.menuImportTileTypes = new System.Windows.Forms.ToolStripMenuItem();
			this.menuImportTileTypes.Click += this.menuImportTileTypes_Click;
			this.ToolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
			this.MapWZToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.MapWZToolStripMenuItem.Click += this.MapWZToolStripMenuItem_Click;
			this.ToolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			this.CloseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.CloseToolStripMenuItem.Click += this.CloseToolStripMenuItem_Click;
			this.menuTools = new System.Windows.Forms.ToolStripMenuItem();
			this.menuReinterpret = new System.Windows.Forms.ToolStripMenuItem();
			this.menuReinterpret.Click += this.ReinterpretTerrainToolStripMenuItem_Click;
			this.menuWaterCorrection = new System.Windows.Forms.ToolStripMenuItem();
			this.menuWaterCorrection.Click += this.WaterTriangleCorrectionToolStripMenuItem_Click;
			this.ToolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
			this.menuFlatOil = new System.Windows.Forms.ToolStripMenuItem();
			this.menuFlatOil.Click += this.ToolStripMenuItem5_Click;
			this.menuFlatStructures = new System.Windows.Forms.ToolStripMenuItem();
			this.menuFlatStructures.Click += this.ToolStripMenuItem6_Click;
			this.ToolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
			this.menuGenerator = new System.Windows.Forms.ToolStripMenuItem();
			this.menuGenerator.Click += this.GeneratorToolStripMenuItem_Click;
			this.menuOptions = new System.Windows.Forms.ToolStripMenuItem();
			this.TableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
			this.TabPage13 = new System.Windows.Forms.TabPage();
			this.TabPage14 = new System.Windows.Forms.TabPage();
			this.TabPage15 = new System.Windows.Forms.TabPage();
			this.TabPage16 = new System.Windows.Forms.TabPage();
			this.TabPage21 = new System.Windows.Forms.TabPage();
			this.TabPage22 = new System.Windows.Forms.TabPage();
			this.TabPage23 = new System.Windows.Forms.TabPage();
			this.TabPage24 = new System.Windows.Forms.TabPage();
			this.SplitContainer1.Panel1.SuspendLayout();
			this.SplitContainer1.Panel2.SuspendLayout();
			this.SplitContainer1.SuspendLayout();
			this.TabControl.SuspendLayout();
			this.tpTextures.SuspendLayout();
			this.TableLayoutPanel6.SuspendLayout();
			this.Panel5.SuspendLayout();
			this.Panel6.SuspendLayout();
			this.tpAutoTexture.SuspendLayout();
			this.Panel15.SuspendLayout();
			this.tpHeight.SuspendLayout();
			this.tabHeightSetR.SuspendLayout();
			this.tabHeightSetL.SuspendLayout();
			this.tpResize.SuspendLayout();
			this.tpObjects.SuspendLayout();
			this.TableLayoutPanel1.SuspendLayout();
			this.Panel1.SuspendLayout();
			this.GroupBox3.SuspendLayout();
			this.Panel2.SuspendLayout();
			this.TabControl1.SuspendLayout();
			this.TabPage1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize) this.dgvFeatures).BeginInit();
			this.TabPage2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize) this.dgvStructures).BeginInit();
			this.TabPage3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize) this.dgvDroids).BeginInit();
			this.tpObject.SuspendLayout();
			this.TableLayoutPanel8.SuspendLayout();
			this.TableLayoutPanel9.SuspendLayout();
			this.Panel13.SuspendLayout();
			this.Panel12.SuspendLayout();
			this.Panel11.SuspendLayout();
			this.Panel10.SuspendLayout();
			this.Panel9.SuspendLayout();
			this.Panel8.SuspendLayout();
			this.Panel14.SuspendLayout();
			this.tpLabels.SuspendLayout();
			this.GroupBox1.SuspendLayout();
			this.TableLayoutPanel7.SuspendLayout();
			this.Panel7.SuspendLayout();
			this.tsTools.SuspendLayout();
			this.tsFile.SuspendLayout();
			this.tsSelection.SuspendLayout();
			this.tsMinimap.SuspendLayout();
			this.menuMain.SuspendLayout();
			this.TableLayoutPanel5.SuspendLayout();
			this.SuspendLayout();
			//
			//SplitContainer1
			//
			this.SplitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.SplitContainer1.Location = new System.Drawing.Point(4, 35);
			this.SplitContainer1.Margin = new System.Windows.Forms.Padding(4);
			this.SplitContainer1.Name = "SplitContainer1";
			//
			//SplitContainer1.Panel1
			//
			this.SplitContainer1.Panel1.Controls.Add(this.TabControl);
			//
			//SplitContainer1.Panel2
			//
			this.SplitContainer1.Panel2.BackColor = System.Drawing.SystemColors.Control;
			this.SplitContainer1.Panel2.Controls.Add(this.TableLayoutPanel7);
			this.SplitContainer1.Size = new System.Drawing.Size(1288, 616);
			this.SplitContainer1.SplitterDistance = 422;
			this.SplitContainer1.TabIndex = 0;
			//
			//TabControl
			//
			this.TabControl.Appearance = System.Windows.Forms.TabAppearance.Buttons;
			this.TabControl.Controls.Add(this.tpTextures);
			this.TabControl.Controls.Add(this.tpAutoTexture);
			this.TabControl.Controls.Add(this.tpHeight);
			this.TabControl.Controls.Add(this.tpResize);
			this.TabControl.Controls.Add(this.tpObjects);
			this.TabControl.Controls.Add(this.tpObject);
			this.TabControl.Controls.Add(this.tpLabels);
			this.TabControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.TabControl.ItemSize = new System.Drawing.Size(72, 22);
			this.TabControl.Location = new System.Drawing.Point(0, 0);
			this.TabControl.Margin = new System.Windows.Forms.Padding(0);
			this.TabControl.Multiline = true;
			this.TabControl.Name = "TabControl";
			this.TabControl.Padding = new System.Drawing.Point(0, 0);
			this.TabControl.SelectedIndex = 0;
			this.TabControl.Size = new System.Drawing.Size(418, 612);
			this.TabControl.TabIndex = 0;
			//
			//tpTextures
			//
			this.tpTextures.Controls.Add(this.TableLayoutPanel6);
			this.tpTextures.Location = new System.Drawing.Point(4, 51);
			this.tpTextures.Margin = new System.Windows.Forms.Padding(0);
			this.tpTextures.Name = "tpTextures";
			this.tpTextures.Padding = new System.Windows.Forms.Padding(4);
			this.tpTextures.Size = new System.Drawing.Size(410, 557);
			this.tpTextures.TabIndex = 0;
			this.tpTextures.Text = "Textures";
			this.tpTextures.UseVisualStyleBackColor = true;
			//
			//TableLayoutPanel6
			//
			this.TableLayoutPanel6.ColumnCount = 1;
			this.TableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, (float) (100.0F)));
			this.TableLayoutPanel6.Controls.Add(this.Panel5, 0, 0);
			this.TableLayoutPanel6.Controls.Add(this.Panel6, 0, 2);
			this.TableLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
			this.TableLayoutPanel6.Location = new System.Drawing.Point(4, 4);
			this.TableLayoutPanel6.Margin = new System.Windows.Forms.Padding(0);
			this.TableLayoutPanel6.Name = "TableLayoutPanel6";
			this.TableLayoutPanel6.RowCount = 3;
			this.TableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, (float) (175.0F)));
			this.TableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, (float) (100.0F)));
			this.TableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, (float) (79.0F)));
			this.TableLayoutPanel6.Size = new System.Drawing.Size(402, 549);
			this.TableLayoutPanel6.TabIndex = 8;
			//
			//Panel5
			//
			this.Panel5.Controls.Add(this.rdoTextureRemoveTerrain);
			this.Panel5.Controls.Add(this.rdoTextureReinterpretTerrain);
			this.Panel5.Controls.Add(this.rdoTextureIgnoreTerrain);
			this.Panel5.Controls.Add(this.pnlTextureBrush);
			this.Panel5.Controls.Add(this.chkTextureOrientationRandomize);
			this.Panel5.Controls.Add(this.btnTextureFlipX);
			this.Panel5.Controls.Add(this.btnTextureClockwise);
			this.Panel5.Controls.Add(this.btnTextureAnticlockwise);
			this.Panel5.Controls.Add(this.chkSetTextureOrientation);
			this.Panel5.Controls.Add(this.chkSetTexture);
			this.Panel5.Controls.Add(this.Label21);
			this.Panel5.Controls.Add(this.cboTileset);
			this.Panel5.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Panel5.Location = new System.Drawing.Point(4, 4);
			this.Panel5.Margin = new System.Windows.Forms.Padding(4);
			this.Panel5.Name = "Panel5";
			this.Panel5.Size = new System.Drawing.Size(394, 167);
			this.Panel5.TabIndex = 0;
			//
			//rdoTextureRemoveTerrain
			//
			this.rdoTextureRemoveTerrain.AutoSize = true;
			this.rdoTextureRemoveTerrain.Location = new System.Drawing.Point(280, 129);
			this.rdoTextureRemoveTerrain.Margin = new System.Windows.Forms.Padding(4);
			this.rdoTextureRemoveTerrain.Name = "rdoTextureRemoveTerrain";
			this.rdoTextureRemoveTerrain.Size = new System.Drawing.Size(122, 21);
			this.rdoTextureRemoveTerrain.TabIndex = 48;
			this.rdoTextureRemoveTerrain.Text = "Remove Terrain";
			this.rdoTextureRemoveTerrain.UseCompatibleTextRendering = true;
			this.rdoTextureRemoveTerrain.UseVisualStyleBackColor = true;
			//
			//rdoTextureReinterpretTerrain
			//
			this.rdoTextureReinterpretTerrain.AutoSize = true;
			this.rdoTextureReinterpretTerrain.Checked = true;
			this.rdoTextureReinterpretTerrain.Location = new System.Drawing.Point(280, 109);
			this.rdoTextureReinterpretTerrain.Margin = new System.Windows.Forms.Padding(4);
			this.rdoTextureReinterpretTerrain.Name = "rdoTextureReinterpretTerrain";
			this.rdoTextureReinterpretTerrain.Size = new System.Drawing.Size(92, 21);
			this.rdoTextureReinterpretTerrain.TabIndex = 47;
			this.rdoTextureReinterpretTerrain.TabStop = true;
			this.rdoTextureReinterpretTerrain.Text = "Reinterpret";
			this.rdoTextureReinterpretTerrain.UseCompatibleTextRendering = true;
			this.rdoTextureReinterpretTerrain.UseVisualStyleBackColor = true;
			//
			//rdoTextureIgnoreTerrain
			//
			this.rdoTextureIgnoreTerrain.AutoSize = true;
			this.rdoTextureIgnoreTerrain.Location = new System.Drawing.Point(280, 87);
			this.rdoTextureIgnoreTerrain.Margin = new System.Windows.Forms.Padding(4);
			this.rdoTextureIgnoreTerrain.Name = "rdoTextureIgnoreTerrain";
			this.rdoTextureIgnoreTerrain.Size = new System.Drawing.Size(110, 21);
			this.rdoTextureIgnoreTerrain.TabIndex = 46;
			this.rdoTextureIgnoreTerrain.Text = "Ignore Terrain";
			this.rdoTextureIgnoreTerrain.UseCompatibleTextRendering = true;
			this.rdoTextureIgnoreTerrain.UseVisualStyleBackColor = true;
			//
			//pnlTextureBrush
			//
			this.pnlTextureBrush.Location = new System.Drawing.Point(25, 44);
			this.pnlTextureBrush.Name = "pnlTextureBrush";
			this.pnlTextureBrush.Size = new System.Drawing.Size(341, 36);
			this.pnlTextureBrush.TabIndex = 45;
			//
			//chkTextureOrientationRandomize
			//
			this.chkTextureOrientationRandomize.AutoSize = true;
			this.chkTextureOrientationRandomize.Location = new System.Drawing.Point(157, 137);
			this.chkTextureOrientationRandomize.Margin = new System.Windows.Forms.Padding(4);
			this.chkTextureOrientationRandomize.Name = "chkTextureOrientationRandomize";
			this.chkTextureOrientationRandomize.Size = new System.Drawing.Size(95, 21);
			this.chkTextureOrientationRandomize.TabIndex = 44;
			this.chkTextureOrientationRandomize.Text = "Randomize";
			this.chkTextureOrientationRandomize.UseCompatibleTextRendering = true;
			this.chkTextureOrientationRandomize.UseVisualStyleBackColor = true;
			//
			//btnTextureFlipX
			//
			this.btnTextureFlipX.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnTextureFlipX.Location = new System.Drawing.Point(237, 103);
			this.btnTextureFlipX.Margin = new System.Windows.Forms.Padding(0);
			this.btnTextureFlipX.Name = "btnTextureFlipX";
			this.btnTextureFlipX.Size = new System.Drawing.Size(32, 30);
			this.btnTextureFlipX.TabIndex = 43;
			this.btnTextureFlipX.UseCompatibleTextRendering = true;
			this.btnTextureFlipX.UseVisualStyleBackColor = true;
			//
			//btnTextureClockwise
			//
			this.btnTextureClockwise.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnTextureClockwise.Location = new System.Drawing.Point(197, 103);
			this.btnTextureClockwise.Margin = new System.Windows.Forms.Padding(0);
			this.btnTextureClockwise.Name = "btnTextureClockwise";
			this.btnTextureClockwise.Size = new System.Drawing.Size(32, 30);
			this.btnTextureClockwise.TabIndex = 42;
			this.btnTextureClockwise.UseCompatibleTextRendering = true;
			this.btnTextureClockwise.UseVisualStyleBackColor = true;
			//
			//btnTextureAnticlockwise
			//
			this.btnTextureAnticlockwise.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnTextureAnticlockwise.Location = new System.Drawing.Point(157, 103);
			this.btnTextureAnticlockwise.Margin = new System.Windows.Forms.Padding(0);
			this.btnTextureAnticlockwise.Name = "btnTextureAnticlockwise";
			this.btnTextureAnticlockwise.Size = new System.Drawing.Size(32, 30);
			this.btnTextureAnticlockwise.TabIndex = 41;
			this.btnTextureAnticlockwise.UseCompatibleTextRendering = true;
			this.btnTextureAnticlockwise.UseVisualStyleBackColor = true;
			//
			//chkSetTextureOrientation
			//
			this.chkSetTextureOrientation.AutoSize = true;
			this.chkSetTextureOrientation.Checked = true;
			this.chkSetTextureOrientation.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkSetTextureOrientation.Location = new System.Drawing.Point(25, 110);
			this.chkSetTextureOrientation.Margin = new System.Windows.Forms.Padding(4);
			this.chkSetTextureOrientation.Name = "chkSetTextureOrientation";
			this.chkSetTextureOrientation.Size = new System.Drawing.Size(116, 21);
			this.chkSetTextureOrientation.TabIndex = 40;
			this.chkSetTextureOrientation.Text = "Set Orientation";
			this.chkSetTextureOrientation.UseCompatibleTextRendering = true;
			this.chkSetTextureOrientation.UseVisualStyleBackColor = true;
			//
			//chkSetTexture
			//
			this.chkSetTexture.AutoSize = true;
			this.chkSetTexture.Checked = true;
			this.chkSetTexture.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkSetTexture.Location = new System.Drawing.Point(25, 81);
			this.chkSetTexture.Margin = new System.Windows.Forms.Padding(4);
			this.chkSetTexture.Name = "chkSetTexture";
			this.chkSetTexture.Size = new System.Drawing.Size(96, 21);
			this.chkSetTexture.TabIndex = 39;
			this.chkSetTexture.Text = "Set Texture";
			this.chkSetTexture.UseCompatibleTextRendering = true;
			this.chkSetTexture.UseVisualStyleBackColor = true;
			//
			//Label21
			//
			this.Label21.Location = new System.Drawing.Point(21, 17);
			this.Label21.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.Label21.Name = "Label21";
			this.Label21.Size = new System.Drawing.Size(64, 20);
			this.Label21.TabIndex = 8;
			this.Label21.Text = "Tileset:";
			this.Label21.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.Label21.UseCompatibleTextRendering = true;
			//
			//cboTileset
			//
			this.cboTileset.DropDownHeight = 512;
			this.cboTileset.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboTileset.DropDownWidth = 384;
			this.cboTileset.FormattingEnabled = true;
			this.cboTileset.IntegralHeight = false;
			this.cboTileset.Items.AddRange(new object[] {"Arizona", "Urban", "Rocky Mountains"});
			this.cboTileset.Location = new System.Drawing.Point(103, 16);
			this.cboTileset.Margin = new System.Windows.Forms.Padding(4);
			this.cboTileset.Name = "cboTileset";
			this.cboTileset.Size = new System.Drawing.Size(161, 24);
			this.cboTileset.TabIndex = 0;
			//
			//Panel6
			//
			this.Panel6.Controls.Add(this.cbxTileNumbers);
			this.Panel6.Controls.Add(this.cbxTileTypes);
			this.Panel6.Controls.Add(this.Label20);
			this.Panel6.Controls.Add(this.cboTileType);
			this.Panel6.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Panel6.Location = new System.Drawing.Point(4, 474);
			this.Panel6.Margin = new System.Windows.Forms.Padding(4);
			this.Panel6.Name = "Panel6";
			this.Panel6.Size = new System.Drawing.Size(394, 71);
			this.Panel6.TabIndex = 2;
			//
			//cbxTileNumbers
			//
			this.cbxTileNumbers.Location = new System.Drawing.Point(175, 37);
			this.cbxTileNumbers.Margin = new System.Windows.Forms.Padding(4);
			this.cbxTileNumbers.Name = "cbxTileNumbers";
			this.cbxTileNumbers.Size = new System.Drawing.Size(185, 28);
			this.cbxTileNumbers.TabIndex = 3;
			this.cbxTileNumbers.Text = "Display Tile Numbers";
			this.cbxTileNumbers.UseCompatibleTextRendering = true;
			this.cbxTileNumbers.UseVisualStyleBackColor = true;
			//
			//cbxTileTypes
			//
			this.cbxTileTypes.Location = new System.Drawing.Point(8, 37);
			this.cbxTileTypes.Margin = new System.Windows.Forms.Padding(4);
			this.cbxTileTypes.Name = "cbxTileTypes";
			this.cbxTileTypes.Size = new System.Drawing.Size(159, 28);
			this.cbxTileTypes.TabIndex = 2;
			this.cbxTileTypes.Text = "Display Tile Types";
			this.cbxTileTypes.UseCompatibleTextRendering = true;
			this.cbxTileTypes.UseVisualStyleBackColor = true;
			//
			//Label20
			//
			this.Label20.Location = new System.Drawing.Point(4, 6);
			this.Label20.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.Label20.Name = "Label20";
			this.Label20.Size = new System.Drawing.Size(91, 26);
			this.Label20.TabIndex = 1;
			this.Label20.Text = "Tile Type:";
			this.Label20.UseCompatibleTextRendering = true;
			//
			//cboTileType
			//
			this.cboTileType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboTileType.Enabled = false;
			this.cboTileType.FormattingEnabled = true;
			this.cboTileType.Location = new System.Drawing.Point(103, 4);
			this.cboTileType.Margin = new System.Windows.Forms.Padding(4);
			this.cboTileType.Name = "cboTileType";
			this.cboTileType.Size = new System.Drawing.Size(151, 24);
			this.cboTileType.TabIndex = 0;
			//
			//tpAutoTexture
			//
			this.tpAutoTexture.AutoScroll = true;
			this.tpAutoTexture.Controls.Add(this.Panel15);
			this.tpAutoTexture.Controls.Add(this.rdoCliffTriBrush);
			this.tpAutoTexture.Controls.Add(this.rdoRoadRemove);
			this.tpAutoTexture.Controls.Add(this.pnlCliffRemoveBrush);
			this.tpAutoTexture.Controls.Add(this.pnlTerrainBrush);
			this.tpAutoTexture.Controls.Add(this.cbxInvalidTiles);
			this.tpAutoTexture.Controls.Add(this.cbxAutoTexSetHeight);
			this.tpAutoTexture.Controls.Add(this.cbxCliffTris);
			this.tpAutoTexture.Controls.Add(this.Label29);
			this.tpAutoTexture.Controls.Add(this.rdoAutoRoadLine);
			this.tpAutoTexture.Controls.Add(this.btnAutoTextureRemove);
			this.tpAutoTexture.Controls.Add(this.btnAutoRoadRemove);
			this.tpAutoTexture.Controls.Add(this.rdoAutoRoadPlace);
			this.tpAutoTexture.Controls.Add(this.lstAutoRoad);
			this.tpAutoTexture.Controls.Add(this.rdoAutoTexturePlace);
			this.tpAutoTexture.Controls.Add(this.rdoAutoTextureFill);
			this.tpAutoTexture.Controls.Add(this.rdoAutoCliffBrush);
			this.tpAutoTexture.Controls.Add(this.rdoAutoCliffRemove);
			this.tpAutoTexture.Controls.Add(this.txtAutoCliffSlope);
			this.tpAutoTexture.Controls.Add(this.Label1);
			this.tpAutoTexture.Controls.Add(this.lstAutoTexture);
			this.tpAutoTexture.Controls.Add(this.Label3);
			this.tpAutoTexture.Location = new System.Drawing.Point(4, 51);
			this.tpAutoTexture.Margin = new System.Windows.Forms.Padding(4);
			this.tpAutoTexture.Name = "tpAutoTexture";
			this.tpAutoTexture.Size = new System.Drawing.Size(410, 557);
			this.tpAutoTexture.TabIndex = 2;
			this.tpAutoTexture.Text = "Terrain";
			this.tpAutoTexture.UseVisualStyleBackColor = true;
			//
			//Panel15
			//
			this.Panel15.Controls.Add(this.cbxFillInside);
			this.Panel15.Controls.Add(this.rdoFillCliffIgnore);
			this.Panel15.Controls.Add(this.rdoFillCliffStopBefore);
			this.Panel15.Controls.Add(this.rdoFillCliffStopAfter);
			this.Panel15.Location = new System.Drawing.Point(73, 244);
			this.Panel15.Name = "Panel15";
			this.Panel15.Size = new System.Drawing.Size(313, 81);
			this.Panel15.TabIndex = 53;
			//
			//cbxFillInside
			//
			this.cbxFillInside.Location = new System.Drawing.Point(135, 4);
			this.cbxFillInside.Margin = new System.Windows.Forms.Padding(4);
			this.cbxFillInside.Name = "cbxFillInside";
			this.cbxFillInside.Size = new System.Drawing.Size(147, 21);
			this.cbxFillInside.TabIndex = 54;
			this.cbxFillInside.Text = "Stop Before Edge";
			this.cbxFillInside.UseCompatibleTextRendering = true;
			this.cbxFillInside.UseVisualStyleBackColor = true;
			//
			//rdoFillCliffIgnore
			//
			this.rdoFillCliffIgnore.AutoSize = true;
			this.rdoFillCliffIgnore.Checked = true;
			this.rdoFillCliffIgnore.Location = new System.Drawing.Point(4, 4);
			this.rdoFillCliffIgnore.Margin = new System.Windows.Forms.Padding(4);
			this.rdoFillCliffIgnore.Name = "rdoFillCliffIgnore";
			this.rdoFillCliffIgnore.Size = new System.Drawing.Size(91, 21);
			this.rdoFillCliffIgnore.TabIndex = 52;
			this.rdoFillCliffIgnore.TabStop = true;
			this.rdoFillCliffIgnore.Text = "Ignore Cliff";
			this.rdoFillCliffIgnore.UseCompatibleTextRendering = true;
			this.rdoFillCliffIgnore.UseVisualStyleBackColor = true;
			//
			//rdoFillCliffStopBefore
			//
			this.rdoFillCliffStopBefore.AutoSize = true;
			this.rdoFillCliffStopBefore.Location = new System.Drawing.Point(4, 24);
			this.rdoFillCliffStopBefore.Margin = new System.Windows.Forms.Padding(4);
			this.rdoFillCliffStopBefore.Name = "rdoFillCliffStopBefore";
			this.rdoFillCliffStopBefore.Size = new System.Drawing.Size(123, 21);
			this.rdoFillCliffStopBefore.TabIndex = 50;
			this.rdoFillCliffStopBefore.Text = "Stop Before Cliff";
			this.rdoFillCliffStopBefore.UseCompatibleTextRendering = true;
			this.rdoFillCliffStopBefore.UseVisualStyleBackColor = true;
			//
			//rdoFillCliffStopAfter
			//
			this.rdoFillCliffStopAfter.AutoSize = true;
			this.rdoFillCliffStopAfter.Location = new System.Drawing.Point(4, 43);
			this.rdoFillCliffStopAfter.Margin = new System.Windows.Forms.Padding(4);
			this.rdoFillCliffStopAfter.Name = "rdoFillCliffStopAfter";
			this.rdoFillCliffStopAfter.Size = new System.Drawing.Size(112, 21);
			this.rdoFillCliffStopAfter.TabIndex = 51;
			this.rdoFillCliffStopAfter.Text = "Stop After Cliff";
			this.rdoFillCliffStopAfter.UseCompatibleTextRendering = true;
			this.rdoFillCliffStopAfter.UseVisualStyleBackColor = true;
			//
			//rdoCliffTriBrush
			//
			this.rdoCliffTriBrush.AutoSize = true;
			this.rdoCliffTriBrush.Location = new System.Drawing.Point(11, 491);
			this.rdoCliffTriBrush.Margin = new System.Windows.Forms.Padding(4);
			this.rdoCliffTriBrush.Name = "rdoCliffTriBrush";
			this.rdoCliffTriBrush.Size = new System.Drawing.Size(101, 21);
			this.rdoCliffTriBrush.TabIndex = 49;
			this.rdoCliffTriBrush.Text = "Cliff Triangle";
			this.rdoCliffTriBrush.UseCompatibleTextRendering = true;
			this.rdoCliffTriBrush.UseVisualStyleBackColor = true;
			//
			//rdoRoadRemove
			//
			this.rdoRoadRemove.AutoSize = true;
			this.rdoRoadRemove.Location = new System.Drawing.Point(11, 452);
			this.rdoRoadRemove.Margin = new System.Windows.Forms.Padding(4);
			this.rdoRoadRemove.Name = "rdoRoadRemove";
			this.rdoRoadRemove.Size = new System.Drawing.Size(76, 21);
			this.rdoRoadRemove.TabIndex = 48;
			this.rdoRoadRemove.Text = "Remove";
			this.rdoRoadRemove.UseCompatibleTextRendering = true;
			this.rdoRoadRemove.UseVisualStyleBackColor = true;
			//
			//pnlCliffRemoveBrush
			//
			this.pnlCliffRemoveBrush.Location = new System.Drawing.Point(32, 598);
			this.pnlCliffRemoveBrush.Name = "pnlCliffRemoveBrush";
			this.pnlCliffRemoveBrush.Size = new System.Drawing.Size(341, 38);
			this.pnlCliffRemoveBrush.TabIndex = 47;
			//
			//pnlTerrainBrush
			//
			this.pnlTerrainBrush.Location = new System.Drawing.Point(14, 3);
			this.pnlTerrainBrush.Name = "pnlTerrainBrush";
			this.pnlTerrainBrush.Size = new System.Drawing.Size(341, 38);
			this.pnlTerrainBrush.TabIndex = 46;
			//
			//cbxInvalidTiles
			//
			this.cbxInvalidTiles.Checked = true;
			this.cbxInvalidTiles.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbxInvalidTiles.Location = new System.Drawing.Point(183, 59);
			this.cbxInvalidTiles.Margin = new System.Windows.Forms.Padding(4);
			this.cbxInvalidTiles.Name = "cbxInvalidTiles";
			this.cbxInvalidTiles.Size = new System.Drawing.Size(152, 21);
			this.cbxInvalidTiles.TabIndex = 38;
			this.cbxInvalidTiles.Text = "Make Invalid Tiles";
			this.cbxInvalidTiles.UseCompatibleTextRendering = true;
			this.cbxInvalidTiles.UseVisualStyleBackColor = true;
			//
			//cbxAutoTexSetHeight
			//
			this.cbxAutoTexSetHeight.Location = new System.Drawing.Point(96, 215);
			this.cbxAutoTexSetHeight.Margin = new System.Windows.Forms.Padding(4);
			this.cbxAutoTexSetHeight.Name = "cbxAutoTexSetHeight";
			this.cbxAutoTexSetHeight.Size = new System.Drawing.Size(127, 21);
			this.cbxAutoTexSetHeight.TabIndex = 36;
			this.cbxAutoTexSetHeight.Text = "Set Height";
			this.cbxAutoTexSetHeight.UseCompatibleTextRendering = true;
			this.cbxAutoTexSetHeight.UseVisualStyleBackColor = true;
			//
			//cbxCliffTris
			//
			this.cbxCliffTris.Checked = true;
			this.cbxCliffTris.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbxCliffTris.Location = new System.Drawing.Point(161, 543);
			this.cbxCliffTris.Margin = new System.Windows.Forms.Padding(4);
			this.cbxCliffTris.Name = "cbxCliffTris";
			this.cbxCliffTris.Size = new System.Drawing.Size(127, 21);
			this.cbxCliffTris.TabIndex = 35;
			this.cbxCliffTris.Text = "Set Tris";
			this.cbxCliffTris.UseCompatibleTextRendering = true;
			this.cbxCliffTris.UseVisualStyleBackColor = true;
			//
			//Label29
			//
			this.Label29.Location = new System.Drawing.Point(11, 328);
			this.Label29.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.Label29.Name = "Label29";
			this.Label29.Size = new System.Drawing.Size(107, 20);
			this.Label29.TabIndex = 33;
			this.Label29.Text = "Road Type:";
			this.Label29.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			//
			//rdoAutoRoadLine
			//
			this.rdoAutoRoadLine.AutoSize = true;
			this.rdoAutoRoadLine.Location = new System.Drawing.Point(11, 423);
			this.rdoAutoRoadLine.Margin = new System.Windows.Forms.Padding(4);
			this.rdoAutoRoadLine.Name = "rdoAutoRoadLine";
			this.rdoAutoRoadLine.Size = new System.Drawing.Size(58, 21);
			this.rdoAutoRoadLine.TabIndex = 32;
			this.rdoAutoRoadLine.Text = "Lines";
			this.rdoAutoRoadLine.UseCompatibleTextRendering = true;
			this.rdoAutoRoadLine.UseVisualStyleBackColor = true;
			//
			//btnAutoTextureRemove
			//
			this.btnAutoTextureRemove.Location = new System.Drawing.Point(178, 177);
			this.btnAutoTextureRemove.Margin = new System.Windows.Forms.Padding(4);
			this.btnAutoTextureRemove.Name = "btnAutoTextureRemove";
			this.btnAutoTextureRemove.Size = new System.Drawing.Size(85, 30);
			this.btnAutoTextureRemove.TabIndex = 31;
			this.btnAutoTextureRemove.Text = "Erase";
			this.btnAutoTextureRemove.UseCompatibleTextRendering = true;
			this.btnAutoTextureRemove.UseVisualStyleBackColor = true;
			//
			//btnAutoRoadRemove
			//
			this.btnAutoRoadRemove.Location = new System.Drawing.Point(85, 396);
			this.btnAutoRoadRemove.Margin = new System.Windows.Forms.Padding(4);
			this.btnAutoRoadRemove.Name = "btnAutoRoadRemove";
			this.btnAutoRoadRemove.Size = new System.Drawing.Size(85, 30);
			this.btnAutoRoadRemove.TabIndex = 30;
			this.btnAutoRoadRemove.Text = "Erase";
			this.btnAutoRoadRemove.UseCompatibleTextRendering = true;
			this.btnAutoRoadRemove.UseVisualStyleBackColor = true;
			//
			//rdoAutoRoadPlace
			//
			this.rdoAutoRoadPlace.AutoSize = true;
			this.rdoAutoRoadPlace.Location = new System.Drawing.Point(11, 394);
			this.rdoAutoRoadPlace.Margin = new System.Windows.Forms.Padding(4);
			this.rdoAutoRoadPlace.Name = "rdoAutoRoadPlace";
			this.rdoAutoRoadPlace.Size = new System.Drawing.Size(59, 21);
			this.rdoAutoRoadPlace.TabIndex = 29;
			this.rdoAutoRoadPlace.Text = "Sides";
			this.rdoAutoRoadPlace.UseCompatibleTextRendering = true;
			this.rdoAutoRoadPlace.UseVisualStyleBackColor = true;
			//
			//lstAutoRoad
			//
			this.lstAutoRoad.FormattingEnabled = true;
			this.lstAutoRoad.ItemHeight = 16;
			this.lstAutoRoad.Location = new System.Drawing.Point(11, 350);
			this.lstAutoRoad.Margin = new System.Windows.Forms.Padding(4);
			this.lstAutoRoad.Name = "lstAutoRoad";
			this.lstAutoRoad.ScrollAlwaysVisible = true;
			this.lstAutoRoad.Size = new System.Drawing.Size(159, 36);
			this.lstAutoRoad.TabIndex = 27;
			//
			//rdoAutoTexturePlace
			//
			this.rdoAutoTexturePlace.AutoSize = true;
			this.rdoAutoTexturePlace.Location = new System.Drawing.Point(11, 214);
			this.rdoAutoTexturePlace.Margin = new System.Windows.Forms.Padding(4);
			this.rdoAutoTexturePlace.Name = "rdoAutoTexturePlace";
			this.rdoAutoTexturePlace.Size = new System.Drawing.Size(59, 21);
			this.rdoAutoTexturePlace.TabIndex = 26;
			this.rdoAutoTexturePlace.Text = "Place";
			this.rdoAutoTexturePlace.UseCompatibleTextRendering = true;
			this.rdoAutoTexturePlace.UseVisualStyleBackColor = true;
			//
			//rdoAutoTextureFill
			//
			this.rdoAutoTextureFill.AutoSize = true;
			this.rdoAutoTextureFill.Location = new System.Drawing.Point(11, 244);
			this.rdoAutoTextureFill.Margin = new System.Windows.Forms.Padding(4);
			this.rdoAutoTextureFill.Name = "rdoAutoTextureFill";
			this.rdoAutoTextureFill.Size = new System.Drawing.Size(43, 21);
			this.rdoAutoTextureFill.TabIndex = 25;
			this.rdoAutoTextureFill.Text = "Fill";
			this.rdoAutoTextureFill.UseCompatibleTextRendering = true;
			this.rdoAutoTextureFill.UseVisualStyleBackColor = true;
			//
			//rdoAutoCliffBrush
			//
			this.rdoAutoCliffBrush.AutoSize = true;
			this.rdoAutoCliffBrush.Location = new System.Drawing.Point(11, 540);
			this.rdoAutoCliffBrush.Margin = new System.Windows.Forms.Padding(4);
			this.rdoAutoCliffBrush.Name = "rdoAutoCliffBrush";
			this.rdoAutoCliffBrush.Size = new System.Drawing.Size(88, 21);
			this.rdoAutoCliffBrush.TabIndex = 22;
			this.rdoAutoCliffBrush.Text = "Cliff Brush";
			this.rdoAutoCliffBrush.UseCompatibleTextRendering = true;
			this.rdoAutoCliffBrush.UseVisualStyleBackColor = true;
			//
			//rdoAutoCliffRemove
			//
			this.rdoAutoCliffRemove.AutoSize = true;
			this.rdoAutoCliffRemove.Location = new System.Drawing.Point(11, 570);
			this.rdoAutoCliffRemove.Margin = new System.Windows.Forms.Padding(4);
			this.rdoAutoCliffRemove.Name = "rdoAutoCliffRemove";
			this.rdoAutoCliffRemove.Size = new System.Drawing.Size(102, 21);
			this.rdoAutoCliffRemove.TabIndex = 21;
			this.rdoAutoCliffRemove.Text = "Cliff Remove";
			this.rdoAutoCliffRemove.UseCompatibleTextRendering = true;
			this.rdoAutoCliffRemove.UseVisualStyleBackColor = true;
			//
			//txtAutoCliffSlope
			//
			this.txtAutoCliffSlope.Location = new System.Drawing.Point(117, 516);
			this.txtAutoCliffSlope.Margin = new System.Windows.Forms.Padding(4);
			this.txtAutoCliffSlope.Name = "txtAutoCliffSlope";
			this.txtAutoCliffSlope.Size = new System.Drawing.Size(52, 22);
			this.txtAutoCliffSlope.TabIndex = 7;
			this.txtAutoCliffSlope.Text = "35";
			this.txtAutoCliffSlope.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			//
			//Label1
			//
			this.Label1.Location = new System.Drawing.Point(11, 516);
			this.Label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.Label1.Name = "Label1";
			this.Label1.Size = new System.Drawing.Size(96, 20);
			this.Label1.TabIndex = 6;
			this.Label1.Text = "Cliff Angle";
			this.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.Label1.UseCompatibleTextRendering = true;
			//
			//lstAutoTexture
			//
			this.lstAutoTexture.FormattingEnabled = true;
			this.lstAutoTexture.ItemHeight = 16;
			this.lstAutoTexture.Location = new System.Drawing.Point(11, 59);
			this.lstAutoTexture.Margin = new System.Windows.Forms.Padding(4);
			this.lstAutoTexture.Name = "lstAutoTexture";
			this.lstAutoTexture.ScrollAlwaysVisible = true;
			this.lstAutoTexture.Size = new System.Drawing.Size(159, 148);
			this.lstAutoTexture.TabIndex = 4;
			//
			//Label3
			//
			this.Label3.Location = new System.Drawing.Point(11, 39);
			this.Label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.Label3.Name = "Label3";
			this.Label3.Size = new System.Drawing.Size(107, 20);
			this.Label3.TabIndex = 3;
			this.Label3.Text = "Ground Type";
			this.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.Label3.UseCompatibleTextRendering = true;
			//
			//tpHeight
			//
			this.tpHeight.AutoScroll = true;
			this.tpHeight.Controls.Add(this.cbxHeightChangeFade);
			this.tpHeight.Controls.Add(this.pnlHeightSetBrush);
			this.tpHeight.Controls.Add(this.btnHeightsMultiplySelection);
			this.tpHeight.Controls.Add(this.btnHeightOffsetSelection);
			this.tpHeight.Controls.Add(this.tabHeightSetR);
			this.tpHeight.Controls.Add(this.tabHeightSetL);
			this.tpHeight.Controls.Add(this.txtHeightSetR);
			this.tpHeight.Controls.Add(this.Label27);
			this.tpHeight.Controls.Add(this.Label10);
			this.tpHeight.Controls.Add(this.txtHeightOffset);
			this.tpHeight.Controls.Add(this.Label9);
			this.tpHeight.Controls.Add(this.txtHeightMultiply);
			this.tpHeight.Controls.Add(this.txtHeightChangeRate);
			this.tpHeight.Controls.Add(this.Label18);
			this.tpHeight.Controls.Add(this.rdoHeightChange);
			this.tpHeight.Controls.Add(this.Label16);
			this.tpHeight.Controls.Add(this.txtSmoothRate);
			this.tpHeight.Controls.Add(this.Label6);
			this.tpHeight.Controls.Add(this.rdoHeightSmooth);
			this.tpHeight.Controls.Add(this.rdoHeightSet);
			this.tpHeight.Controls.Add(this.txtHeightSetL);
			this.tpHeight.Controls.Add(this.Label5);
			this.tpHeight.Location = new System.Drawing.Point(4, 51);
			this.tpHeight.Margin = new System.Windows.Forms.Padding(4);
			this.tpHeight.Name = "tpHeight";
			this.tpHeight.Padding = new System.Windows.Forms.Padding(4);
			this.tpHeight.Size = new System.Drawing.Size(410, 557);
			this.tpHeight.TabIndex = 1;
			this.tpHeight.Text = "Height";
			this.tpHeight.UseVisualStyleBackColor = true;
			//
			//cbxHeightChangeFade
			//
			this.cbxHeightChangeFade.Checked = true;
			this.cbxHeightChangeFade.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbxHeightChangeFade.Location = new System.Drawing.Point(177, 248);
			this.cbxHeightChangeFade.Margin = new System.Windows.Forms.Padding(4);
			this.cbxHeightChangeFade.Name = "cbxHeightChangeFade";
			this.cbxHeightChangeFade.Size = new System.Drawing.Size(152, 21);
			this.cbxHeightChangeFade.TabIndex = 47;
			this.cbxHeightChangeFade.Text = "Fading";
			this.cbxHeightChangeFade.UseCompatibleTextRendering = true;
			this.cbxHeightChangeFade.UseVisualStyleBackColor = true;
			//
			//pnlHeightSetBrush
			//
			this.pnlHeightSetBrush.Location = new System.Drawing.Point(29, 8);
			this.pnlHeightSetBrush.Name = "pnlHeightSetBrush";
			this.pnlHeightSetBrush.Size = new System.Drawing.Size(341, 38);
			this.pnlHeightSetBrush.TabIndex = 46;
			//
			//btnHeightsMultiplySelection
			//
			this.btnHeightsMultiplySelection.Location = new System.Drawing.Point(121, 383);
			this.btnHeightsMultiplySelection.Margin = new System.Windows.Forms.Padding(4);
			this.btnHeightsMultiplySelection.Name = "btnHeightsMultiplySelection";
			this.btnHeightsMultiplySelection.Size = new System.Drawing.Size(75, 30);
			this.btnHeightsMultiplySelection.TabIndex = 38;
			this.btnHeightsMultiplySelection.Text = "Do";
			this.btnHeightsMultiplySelection.UseVisualStyleBackColor = true;
			//
			//btnHeightOffsetSelection
			//
			this.btnHeightOffsetSelection.Location = new System.Drawing.Point(121, 452);
			this.btnHeightOffsetSelection.Margin = new System.Windows.Forms.Padding(4);
			this.btnHeightOffsetSelection.Name = "btnHeightOffsetSelection";
			this.btnHeightOffsetSelection.Size = new System.Drawing.Size(75, 30);
			this.btnHeightOffsetSelection.TabIndex = 37;
			this.btnHeightOffsetSelection.Text = "Do";
			this.btnHeightOffsetSelection.UseVisualStyleBackColor = true;
			//
			//tabHeightSetR
			//
			this.tabHeightSetR.Appearance = System.Windows.Forms.TabAppearance.Buttons;
			this.tabHeightSetR.Controls.Add(this.TabPage25);
			this.tabHeightSetR.Controls.Add(this.TabPage26);
			this.tabHeightSetR.Controls.Add(this.TabPage27);
			this.tabHeightSetR.Controls.Add(this.TabPage28);
			this.tabHeightSetR.Controls.Add(this.TabPage29);
			this.tabHeightSetR.Controls.Add(this.TabPage30);
			this.tabHeightSetR.Controls.Add(this.TabPage31);
			this.tabHeightSetR.Controls.Add(this.TabPage32);
			this.tabHeightSetR.ItemSize = new System.Drawing.Size(28, 20);
			this.tabHeightSetR.Location = new System.Drawing.Point(29, 169);
			this.tabHeightSetR.Margin = new System.Windows.Forms.Padding(0);
			this.tabHeightSetR.Multiline = true;
			this.tabHeightSetR.Name = "tabHeightSetR";
			this.tabHeightSetR.Padding = new System.Drawing.Point(0, 0);
			this.tabHeightSetR.SelectedIndex = 0;
			this.tabHeightSetR.Size = new System.Drawing.Size(439, 25);
			this.tabHeightSetR.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
			this.tabHeightSetR.TabIndex = 35;
			//
			//TabPage25
			//
			this.TabPage25.Location = new System.Drawing.Point(4, 24);
			this.TabPage25.Margin = new System.Windows.Forms.Padding(4);
			this.TabPage25.Name = "TabPage25";
			this.TabPage25.Padding = new System.Windows.Forms.Padding(4);
			this.TabPage25.Size = new System.Drawing.Size(431, 0);
			this.TabPage25.TabIndex = 0;
			this.TabPage25.Text = "1";
			this.TabPage25.UseVisualStyleBackColor = true;
			//
			//TabPage26
			//
			this.TabPage26.Location = new System.Drawing.Point(4, 24);
			this.TabPage26.Margin = new System.Windows.Forms.Padding(4);
			this.TabPage26.Name = "TabPage26";
			this.TabPage26.Padding = new System.Windows.Forms.Padding(4);
			this.TabPage26.Size = new System.Drawing.Size(431, 0);
			this.TabPage26.TabIndex = 1;
			this.TabPage26.Text = "2";
			this.TabPage26.UseVisualStyleBackColor = true;
			//
			//TabPage27
			//
			this.TabPage27.Location = new System.Drawing.Point(4, 24);
			this.TabPage27.Margin = new System.Windows.Forms.Padding(4);
			this.TabPage27.Name = "TabPage27";
			this.TabPage27.Size = new System.Drawing.Size(431, 0);
			this.TabPage27.TabIndex = 2;
			this.TabPage27.Text = "3";
			this.TabPage27.UseVisualStyleBackColor = true;
			//
			//TabPage28
			//
			this.TabPage28.Location = new System.Drawing.Point(4, 24);
			this.TabPage28.Margin = new System.Windows.Forms.Padding(4);
			this.TabPage28.Name = "TabPage28";
			this.TabPage28.Size = new System.Drawing.Size(431, 0);
			this.TabPage28.TabIndex = 3;
			this.TabPage28.Text = "4";
			this.TabPage28.UseVisualStyleBackColor = true;
			//
			//TabPage29
			//
			this.TabPage29.Location = new System.Drawing.Point(4, 24);
			this.TabPage29.Margin = new System.Windows.Forms.Padding(4);
			this.TabPage29.Name = "TabPage29";
			this.TabPage29.Size = new System.Drawing.Size(431, 0);
			this.TabPage29.TabIndex = 4;
			this.TabPage29.Text = "5";
			this.TabPage29.UseVisualStyleBackColor = true;
			//
			//TabPage30
			//
			this.TabPage30.Location = new System.Drawing.Point(4, 24);
			this.TabPage30.Margin = new System.Windows.Forms.Padding(4);
			this.TabPage30.Name = "TabPage30";
			this.TabPage30.Size = new System.Drawing.Size(431, 0);
			this.TabPage30.TabIndex = 5;
			this.TabPage30.Text = "6";
			this.TabPage30.UseVisualStyleBackColor = true;
			//
			//TabPage31
			//
			this.TabPage31.Location = new System.Drawing.Point(4, 24);
			this.TabPage31.Margin = new System.Windows.Forms.Padding(4);
			this.TabPage31.Name = "TabPage31";
			this.TabPage31.Size = new System.Drawing.Size(431, 0);
			this.TabPage31.TabIndex = 6;
			this.TabPage31.Text = "7";
			this.TabPage31.UseVisualStyleBackColor = true;
			//
			//TabPage32
			//
			this.TabPage32.Location = new System.Drawing.Point(4, 24);
			this.TabPage32.Margin = new System.Windows.Forms.Padding(4);
			this.TabPage32.Name = "TabPage32";
			this.TabPage32.Size = new System.Drawing.Size(431, 0);
			this.TabPage32.TabIndex = 7;
			this.TabPage32.Text = "8";
			this.TabPage32.UseVisualStyleBackColor = true;
			//
			//tabHeightSetL
			//
			this.tabHeightSetL.Appearance = System.Windows.Forms.TabAppearance.Buttons;
			this.tabHeightSetL.Controls.Add(this.TabPage9);
			this.tabHeightSetL.Controls.Add(this.TabPage10);
			this.tabHeightSetL.Controls.Add(this.TabPage11);
			this.tabHeightSetL.Controls.Add(this.TabPage12);
			this.tabHeightSetL.Controls.Add(this.TabPage17);
			this.tabHeightSetL.Controls.Add(this.TabPage18);
			this.tabHeightSetL.Controls.Add(this.TabPage19);
			this.tabHeightSetL.Controls.Add(this.TabPage20);
			this.tabHeightSetL.ItemSize = new System.Drawing.Size(28, 20);
			this.tabHeightSetL.Location = new System.Drawing.Point(29, 107);
			this.tabHeightSetL.Margin = new System.Windows.Forms.Padding(0);
			this.tabHeightSetL.Multiline = true;
			this.tabHeightSetL.Name = "tabHeightSetL";
			this.tabHeightSetL.Padding = new System.Drawing.Point(0, 0);
			this.tabHeightSetL.SelectedIndex = 0;
			this.tabHeightSetL.Size = new System.Drawing.Size(439, 25);
			this.tabHeightSetL.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
			this.tabHeightSetL.TabIndex = 34;
			//
			//TabPage9
			//
			this.TabPage9.Location = new System.Drawing.Point(4, 24);
			this.TabPage9.Margin = new System.Windows.Forms.Padding(4);
			this.TabPage9.Name = "TabPage9";
			this.TabPage9.Padding = new System.Windows.Forms.Padding(4);
			this.TabPage9.Size = new System.Drawing.Size(431, 0);
			this.TabPage9.TabIndex = 0;
			this.TabPage9.Text = "1";
			this.TabPage9.UseVisualStyleBackColor = true;
			//
			//TabPage10
			//
			this.TabPage10.Location = new System.Drawing.Point(4, 24);
			this.TabPage10.Margin = new System.Windows.Forms.Padding(4);
			this.TabPage10.Name = "TabPage10";
			this.TabPage10.Padding = new System.Windows.Forms.Padding(4);
			this.TabPage10.Size = new System.Drawing.Size(431, 0);
			this.TabPage10.TabIndex = 1;
			this.TabPage10.Text = "2";
			this.TabPage10.UseVisualStyleBackColor = true;
			//
			//TabPage11
			//
			this.TabPage11.Location = new System.Drawing.Point(4, 24);
			this.TabPage11.Margin = new System.Windows.Forms.Padding(4);
			this.TabPage11.Name = "TabPage11";
			this.TabPage11.Size = new System.Drawing.Size(431, 0);
			this.TabPage11.TabIndex = 2;
			this.TabPage11.Text = "3";
			this.TabPage11.UseVisualStyleBackColor = true;
			//
			//TabPage12
			//
			this.TabPage12.Location = new System.Drawing.Point(4, 24);
			this.TabPage12.Margin = new System.Windows.Forms.Padding(4);
			this.TabPage12.Name = "TabPage12";
			this.TabPage12.Size = new System.Drawing.Size(431, 0);
			this.TabPage12.TabIndex = 3;
			this.TabPage12.Text = "4";
			this.TabPage12.UseVisualStyleBackColor = true;
			//
			//TabPage17
			//
			this.TabPage17.Location = new System.Drawing.Point(4, 24);
			this.TabPage17.Margin = new System.Windows.Forms.Padding(4);
			this.TabPage17.Name = "TabPage17";
			this.TabPage17.Size = new System.Drawing.Size(431, 0);
			this.TabPage17.TabIndex = 4;
			this.TabPage17.Text = "5";
			this.TabPage17.UseVisualStyleBackColor = true;
			//
			//TabPage18
			//
			this.TabPage18.Location = new System.Drawing.Point(4, 24);
			this.TabPage18.Margin = new System.Windows.Forms.Padding(4);
			this.TabPage18.Name = "TabPage18";
			this.TabPage18.Size = new System.Drawing.Size(431, 0);
			this.TabPage18.TabIndex = 5;
			this.TabPage18.Text = "6";
			this.TabPage18.UseVisualStyleBackColor = true;
			//
			//TabPage19
			//
			this.TabPage19.Location = new System.Drawing.Point(4, 24);
			this.TabPage19.Margin = new System.Windows.Forms.Padding(4);
			this.TabPage19.Name = "TabPage19";
			this.TabPage19.Size = new System.Drawing.Size(431, 0);
			this.TabPage19.TabIndex = 6;
			this.TabPage19.Text = "7";
			this.TabPage19.UseVisualStyleBackColor = true;
			//
			//TabPage20
			//
			this.TabPage20.Location = new System.Drawing.Point(4, 24);
			this.TabPage20.Margin = new System.Windows.Forms.Padding(4);
			this.TabPage20.Name = "TabPage20";
			this.TabPage20.Size = new System.Drawing.Size(431, 0);
			this.TabPage20.TabIndex = 7;
			this.TabPage20.Text = "8";
			this.TabPage20.UseVisualStyleBackColor = true;
			//
			//txtHeightSetR
			//
			this.txtHeightSetR.Location = new System.Drawing.Point(121, 140);
			this.txtHeightSetR.Margin = new System.Windows.Forms.Padding(4);
			this.txtHeightSetR.Name = "txtHeightSetR";
			this.txtHeightSetR.Size = new System.Drawing.Size(73, 22);
			this.txtHeightSetR.TabIndex = 31;
			this.txtHeightSetR.Text = "#";
			this.txtHeightSetR.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			//
			//Label27
			//
			this.Label27.Location = new System.Drawing.Point(11, 140);
			this.Label27.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.Label27.Name = "Label27";
			this.Label27.Size = new System.Drawing.Size(99, 20);
			this.Label27.TabIndex = 30;
			this.Label27.Text = "RMB Height";
			this.Label27.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.Label27.UseCompatibleTextRendering = true;
			//
			//Label10
			//
			this.Label10.Location = new System.Drawing.Point(15, 431);
			this.Label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.Label10.Name = "Label10";
			this.Label10.Size = new System.Drawing.Size(209, 17);
			this.Label10.TabIndex = 29;
			this.Label10.Text = "Offset Heights of Selection";
			this.Label10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			//
			//txtHeightOffset
			//
			this.txtHeightOffset.Location = new System.Drawing.Point(19, 455);
			this.txtHeightOffset.Margin = new System.Windows.Forms.Padding(4);
			this.txtHeightOffset.Name = "txtHeightOffset";
			this.txtHeightOffset.Size = new System.Drawing.Size(89, 22);
			this.txtHeightOffset.TabIndex = 27;
			this.txtHeightOffset.Text = "0";
			this.txtHeightOffset.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			//
			//Label9
			//
			this.Label9.Location = new System.Drawing.Point(11, 359);
			this.Label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.Label9.Name = "Label9";
			this.Label9.Size = new System.Drawing.Size(209, 20);
			this.Label9.TabIndex = 25;
			this.Label9.Text = "Multiply Heights of Selection";
			this.Label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.Label9.UseCompatibleTextRendering = true;
			//
			//txtHeightMultiply
			//
			this.txtHeightMultiply.Location = new System.Drawing.Point(19, 386);
			this.txtHeightMultiply.Margin = new System.Windows.Forms.Padding(4);
			this.txtHeightMultiply.Name = "txtHeightMultiply";
			this.txtHeightMultiply.Size = new System.Drawing.Size(89, 22);
			this.txtHeightMultiply.TabIndex = 24;
			this.txtHeightMultiply.Text = "1";
			this.txtHeightMultiply.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			//
			//txtHeightChangeRate
			//
			this.txtHeightChangeRate.Location = new System.Drawing.Point(96, 246);
			this.txtHeightChangeRate.Margin = new System.Windows.Forms.Padding(4);
			this.txtHeightChangeRate.Name = "txtHeightChangeRate";
			this.txtHeightChangeRate.Size = new System.Drawing.Size(73, 22);
			this.txtHeightChangeRate.TabIndex = 23;
			this.txtHeightChangeRate.Text = "16";
			this.txtHeightChangeRate.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			//
			//Label18
			//
			this.Label18.Location = new System.Drawing.Point(11, 246);
			this.Label18.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.Label18.Name = "Label18";
			this.Label18.Size = new System.Drawing.Size(75, 20);
			this.Label18.TabIndex = 22;
			this.Label18.Text = "Rate";
			this.Label18.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.Label18.UseCompatibleTextRendering = true;
			//
			//rdoHeightChange
			//
			this.rdoHeightChange.AutoSize = true;
			this.rdoHeightChange.Location = new System.Drawing.Point(11, 217);
			this.rdoHeightChange.Margin = new System.Windows.Forms.Padding(4);
			this.rdoHeightChange.Name = "rdoHeightChange";
			this.rdoHeightChange.Size = new System.Drawing.Size(73, 21);
			this.rdoHeightChange.TabIndex = 21;
			this.rdoHeightChange.Text = "Change";
			this.rdoHeightChange.UseCompatibleTextRendering = true;
			this.rdoHeightChange.UseVisualStyleBackColor = true;
			//
			//Label16
			//
			this.Label16.Location = new System.Drawing.Point(121, 49);
			this.Label16.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.Label16.Name = "Label16";
			this.Label16.Size = new System.Drawing.Size(75, 21);
			this.Label16.TabIndex = 20;
			this.Label16.Text = "(0-255)";
			this.Label16.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.Label16.UseCompatibleTextRendering = true;
			//
			//txtSmoothRate
			//
			this.txtSmoothRate.Location = new System.Drawing.Point(100, 311);
			this.txtSmoothRate.Margin = new System.Windows.Forms.Padding(4);
			this.txtSmoothRate.Name = "txtSmoothRate";
			this.txtSmoothRate.Size = new System.Drawing.Size(73, 22);
			this.txtSmoothRate.TabIndex = 10;
			this.txtSmoothRate.Text = "3";
			this.txtSmoothRate.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			//
			//Label6
			//
			this.Label6.Location = new System.Drawing.Point(15, 311);
			this.Label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.Label6.Name = "Label6";
			this.Label6.Size = new System.Drawing.Size(75, 20);
			this.Label6.TabIndex = 9;
			this.Label6.Text = "Rate";
			this.Label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.Label6.UseCompatibleTextRendering = true;
			//
			//rdoHeightSmooth
			//
			this.rdoHeightSmooth.AutoSize = true;
			this.rdoHeightSmooth.Location = new System.Drawing.Point(11, 286);
			this.rdoHeightSmooth.Margin = new System.Windows.Forms.Padding(4);
			this.rdoHeightSmooth.Name = "rdoHeightSmooth";
			this.rdoHeightSmooth.Size = new System.Drawing.Size(72, 21);
			this.rdoHeightSmooth.TabIndex = 8;
			this.rdoHeightSmooth.Text = "Smooth";
			this.rdoHeightSmooth.UseCompatibleTextRendering = true;
			this.rdoHeightSmooth.UseVisualStyleBackColor = true;
			//
			//rdoHeightSet
			//
			this.rdoHeightSet.AutoSize = true;
			this.rdoHeightSet.Checked = true;
			this.rdoHeightSet.Location = new System.Drawing.Point(11, 49);
			this.rdoHeightSet.Margin = new System.Windows.Forms.Padding(4);
			this.rdoHeightSet.Name = "rdoHeightSet";
			this.rdoHeightSet.Size = new System.Drawing.Size(46, 21);
			this.rdoHeightSet.TabIndex = 7;
			this.rdoHeightSet.TabStop = true;
			this.rdoHeightSet.Text = "Set";
			this.rdoHeightSet.UseCompatibleTextRendering = true;
			this.rdoHeightSet.UseVisualStyleBackColor = true;
			//
			//txtHeightSetL
			//
			this.txtHeightSetL.Location = new System.Drawing.Point(121, 79);
			this.txtHeightSetL.Margin = new System.Windows.Forms.Padding(4);
			this.txtHeightSetL.Name = "txtHeightSetL";
			this.txtHeightSetL.Size = new System.Drawing.Size(73, 22);
			this.txtHeightSetL.TabIndex = 6;
			this.txtHeightSetL.Text = "#";
			this.txtHeightSetL.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			//
			//Label5
			//
			this.Label5.Location = new System.Drawing.Point(11, 79);
			this.Label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.Label5.Name = "Label5";
			this.Label5.Size = new System.Drawing.Size(99, 20);
			this.Label5.TabIndex = 5;
			this.Label5.Text = "LMB Height";
			this.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.Label5.UseCompatibleTextRendering = true;
			//
			//tpResize
			//
			this.tpResize.Controls.Add(this.btnSelResize);
			this.tpResize.Controls.Add(this.btnResize);
			this.tpResize.Controls.Add(this.txtOffsetY);
			this.tpResize.Controls.Add(this.Label15);
			this.tpResize.Controls.Add(this.txtOffsetX);
			this.tpResize.Controls.Add(this.Label14);
			this.tpResize.Controls.Add(this.txtSizeY);
			this.tpResize.Controls.Add(this.Label13);
			this.tpResize.Controls.Add(this.txtSizeX);
			this.tpResize.Controls.Add(this.Label12);
			this.tpResize.Location = new System.Drawing.Point(4, 51);
			this.tpResize.Margin = new System.Windows.Forms.Padding(4);
			this.tpResize.Name = "tpResize";
			this.tpResize.Size = new System.Drawing.Size(410, 557);
			this.tpResize.TabIndex = 4;
			this.tpResize.Text = "Resize";
			this.tpResize.UseVisualStyleBackColor = true;
			//
			//btnSelResize
			//
			this.btnSelResize.Location = new System.Drawing.Point(21, 175);
			this.btnSelResize.Margin = new System.Windows.Forms.Padding(4);
			this.btnSelResize.Name = "btnSelResize";
			this.btnSelResize.Size = new System.Drawing.Size(180, 30);
			this.btnSelResize.TabIndex = 17;
			this.btnSelResize.Text = "Resize To Selection";
			this.btnSelResize.UseCompatibleTextRendering = true;
			this.btnSelResize.UseVisualStyleBackColor = true;
			//
			//btnResize
			//
			this.btnResize.Location = new System.Drawing.Point(21, 138);
			this.btnResize.Margin = new System.Windows.Forms.Padding(4);
			this.btnResize.Name = "btnResize";
			this.btnResize.Size = new System.Drawing.Size(148, 30);
			this.btnResize.TabIndex = 16;
			this.btnResize.Text = "Resize";
			this.btnResize.UseCompatibleTextRendering = true;
			this.btnResize.UseVisualStyleBackColor = true;
			//
			//txtOffsetY
			//
			this.txtOffsetY.Location = new System.Drawing.Point(117, 98);
			this.txtOffsetY.Margin = new System.Windows.Forms.Padding(4);
			this.txtOffsetY.Name = "txtOffsetY";
			this.txtOffsetY.Size = new System.Drawing.Size(52, 22);
			this.txtOffsetY.TabIndex = 15;
			this.txtOffsetY.Text = "0";
			//
			//Label15
			//
			this.Label15.Location = new System.Drawing.Point(11, 98);
			this.Label15.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.Label15.Name = "Label15";
			this.Label15.Size = new System.Drawing.Size(96, 20);
			this.Label15.TabIndex = 14;
			this.Label15.Text = "Offset Y";
			this.Label15.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.Label15.UseCompatibleTextRendering = true;
			//
			//txtOffsetX
			//
			this.txtOffsetX.Location = new System.Drawing.Point(117, 69);
			this.txtOffsetX.Margin = new System.Windows.Forms.Padding(4);
			this.txtOffsetX.Name = "txtOffsetX";
			this.txtOffsetX.Size = new System.Drawing.Size(52, 22);
			this.txtOffsetX.TabIndex = 13;
			this.txtOffsetX.Text = "0";
			//
			//Label14
			//
			this.Label14.Location = new System.Drawing.Point(11, 69);
			this.Label14.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.Label14.Name = "Label14";
			this.Label14.Size = new System.Drawing.Size(96, 20);
			this.Label14.TabIndex = 12;
			this.Label14.Text = "Offset X";
			this.Label14.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.Label14.UseCompatibleTextRendering = true;
			//
			//txtSizeY
			//
			this.txtSizeY.Location = new System.Drawing.Point(117, 39);
			this.txtSizeY.Margin = new System.Windows.Forms.Padding(4);
			this.txtSizeY.Name = "txtSizeY";
			this.txtSizeY.Size = new System.Drawing.Size(52, 22);
			this.txtSizeY.TabIndex = 11;
			this.txtSizeY.Text = "0";
			//
			//Label13
			//
			this.Label13.Location = new System.Drawing.Point(11, 39);
			this.Label13.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.Label13.Name = "Label13";
			this.Label13.Size = new System.Drawing.Size(96, 20);
			this.Label13.TabIndex = 10;
			this.Label13.Text = "Size Y";
			this.Label13.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.Label13.UseCompatibleTextRendering = true;
			//
			//txtSizeX
			//
			this.txtSizeX.Location = new System.Drawing.Point(117, 10);
			this.txtSizeX.Margin = new System.Windows.Forms.Padding(4);
			this.txtSizeX.Name = "txtSizeX";
			this.txtSizeX.Size = new System.Drawing.Size(52, 22);
			this.txtSizeX.TabIndex = 9;
			this.txtSizeX.Text = "0";
			//
			//Label12
			//
			this.Label12.Location = new System.Drawing.Point(11, 10);
			this.Label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.Label12.Name = "Label12";
			this.Label12.Size = new System.Drawing.Size(96, 20);
			this.Label12.TabIndex = 8;
			this.Label12.Text = "Size X";
			this.Label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.Label12.UseCompatibleTextRendering = true;
			//
			//tpObjects
			//
			this.tpObjects.AutoScroll = true;
			this.tpObjects.Controls.Add(this.TableLayoutPanel1);
			this.tpObjects.Location = new System.Drawing.Point(4, 51);
			this.tpObjects.Margin = new System.Windows.Forms.Padding(4);
			this.tpObjects.Name = "tpObjects";
			this.tpObjects.Size = new System.Drawing.Size(410, 557);
			this.tpObjects.TabIndex = 5;
			this.tpObjects.Text = "Place Objects";
			this.tpObjects.UseVisualStyleBackColor = true;
			//
			//TableLayoutPanel1
			//
			this.TableLayoutPanel1.ColumnCount = 1;
			this.TableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, (float) (100.0F)));
			this.TableLayoutPanel1.Controls.Add(this.Panel1, 0, 0);
			this.TableLayoutPanel1.Controls.Add(this.Panel2, 0, 1);
			this.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.TableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.TableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4);
			this.TableLayoutPanel1.Name = "TableLayoutPanel1";
			this.TableLayoutPanel1.RowCount = 2;
			this.TableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, (float) (192.0F)));
			this.TableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, (float) (100.0F)));
			this.TableLayoutPanel1.Size = new System.Drawing.Size(410, 557);
			this.TableLayoutPanel1.TabIndex = 16;
			//
			//Panel1
			//
			this.Panel1.Controls.Add(this.btnPlayerSelectObjects);
			this.Panel1.Controls.Add(this.Label44);
			this.Panel1.Controls.Add(this.GroupBox3);
			this.Panel1.Controls.Add(this.txtObjectFind);
			this.Panel1.Controls.Add(this.cbxFootprintRotate);
			this.Panel1.Controls.Add(this.txtNewObjectRotation);
			this.Panel1.Controls.Add(this.Label19);
			this.Panel1.Controls.Add(this.cbxAutoWalls);
			this.Panel1.Controls.Add(this.cbxObjectRandomRotation);
			this.Panel1.Controls.Add(this.Label32);
			this.Panel1.Controls.Add(this.Label22);
			this.Panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Panel1.Location = new System.Drawing.Point(4, 4);
			this.Panel1.Margin = new System.Windows.Forms.Padding(4);
			this.Panel1.Name = "Panel1";
			this.Panel1.Size = new System.Drawing.Size(402, 184);
			this.Panel1.TabIndex = 0;
			//
			//btnPlayerSelectObjects
			//
			this.btnPlayerSelectObjects.Location = new System.Drawing.Point(282, 10);
			this.btnPlayerSelectObjects.Name = "btnPlayerSelectObjects";
			this.btnPlayerSelectObjects.Size = new System.Drawing.Size(111, 35);
			this.btnPlayerSelectObjects.TabIndex = 17;
			this.btnPlayerSelectObjects.Text = "Select All";
			this.btnPlayerSelectObjects.UseCompatibleTextRendering = true;
			this.btnPlayerSelectObjects.UseVisualStyleBackColor = true;
			//
			//Label44
			//
			this.Label44.Location = new System.Drawing.Point(3, 163);
			this.Label44.Name = "Label44";
			this.Label44.Size = new System.Drawing.Size(38, 21);
			this.Label44.TabIndex = 57;
			this.Label44.Text = "Find:";
			this.Label44.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.Label44.UseCompatibleTextRendering = true;
			//
			//GroupBox3
			//
			this.GroupBox3.Controls.Add(this.rdoObjectPlace);
			this.GroupBox3.Controls.Add(this.rdoObjectLines);
			this.GroupBox3.Location = new System.Drawing.Point(282, 51);
			this.GroupBox3.Name = "GroupBox3";
			this.GroupBox3.Size = new System.Drawing.Size(120, 75);
			this.GroupBox3.TabIndex = 56;
			this.GroupBox3.TabStop = false;
			this.GroupBox3.Text = "Tool";
			this.GroupBox3.UseCompatibleTextRendering = true;
			//
			//rdoObjectPlace
			//
			this.rdoObjectPlace.AutoSize = true;
			this.rdoObjectPlace.Checked = true;
			this.rdoObjectPlace.Location = new System.Drawing.Point(7, 22);
			this.rdoObjectPlace.Margin = new System.Windows.Forms.Padding(4);
			this.rdoObjectPlace.Name = "rdoObjectPlace";
			this.rdoObjectPlace.Size = new System.Drawing.Size(59, 21);
			this.rdoObjectPlace.TabIndex = 54;
			this.rdoObjectPlace.TabStop = true;
			this.rdoObjectPlace.Text = "Place";
			this.rdoObjectPlace.UseCompatibleTextRendering = true;
			this.rdoObjectPlace.UseVisualStyleBackColor = true;
			//
			//rdoObjectLines
			//
			this.rdoObjectLines.AutoSize = true;
			this.rdoObjectLines.Location = new System.Drawing.Point(7, 47);
			this.rdoObjectLines.Margin = new System.Windows.Forms.Padding(4);
			this.rdoObjectLines.Name = "rdoObjectLines";
			this.rdoObjectLines.Size = new System.Drawing.Size(58, 21);
			this.rdoObjectLines.TabIndex = 55;
			this.rdoObjectLines.Text = "Lines";
			this.rdoObjectLines.UseCompatibleTextRendering = true;
			this.rdoObjectLines.UseVisualStyleBackColor = true;
			//
			//txtObjectFind
			//
			this.txtObjectFind.Location = new System.Drawing.Point(47, 160);
			this.txtObjectFind.Name = "txtObjectFind";
			this.txtObjectFind.Size = new System.Drawing.Size(175, 22);
			this.txtObjectFind.TabIndex = 53;
			//
			//cbxFootprintRotate
			//
			this.cbxFootprintRotate.Location = new System.Drawing.Point(244, 133);
			this.cbxFootprintRotate.Margin = new System.Windows.Forms.Padding(4);
			this.cbxFootprintRotate.Name = "cbxFootprintRotate";
			this.cbxFootprintRotate.Size = new System.Drawing.Size(201, 21);
			this.cbxFootprintRotate.TabIndex = 52;
			this.cbxFootprintRotate.Text = "Rotate Footprints (3.1+)";
			this.cbxFootprintRotate.UseCompatibleTextRendering = true;
			this.cbxFootprintRotate.UseVisualStyleBackColor = true;
			//
			//txtNewObjectRotation
			//
			this.txtNewObjectRotation.Location = new System.Drawing.Point(85, 122);
			this.txtNewObjectRotation.Margin = new System.Windows.Forms.Padding(4);
			this.txtNewObjectRotation.Name = "txtNewObjectRotation";
			this.txtNewObjectRotation.Size = new System.Drawing.Size(41, 22);
			this.txtNewObjectRotation.TabIndex = 51;
			this.txtNewObjectRotation.Text = "0";
			this.txtNewObjectRotation.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			//
			//Label19
			//
			this.Label19.Location = new System.Drawing.Point(16, 125);
			this.Label19.Name = "Label19";
			this.Label19.Size = new System.Drawing.Size(62, 21);
			this.Label19.TabIndex = 50;
			this.Label19.Text = "Rotation:";
			this.Label19.UseCompatibleTextRendering = true;
			//
			//cbxAutoWalls
			//
			this.cbxAutoWalls.Location = new System.Drawing.Point(244, 161);
			this.cbxAutoWalls.Margin = new System.Windows.Forms.Padding(4);
			this.cbxAutoWalls.Name = "cbxAutoWalls";
			this.cbxAutoWalls.Size = new System.Drawing.Size(152, 21);
			this.cbxAutoWalls.TabIndex = 49;
			this.cbxAutoWalls.Text = "Automatic Walls";
			this.cbxAutoWalls.UseCompatibleTextRendering = true;
			this.cbxAutoWalls.UseVisualStyleBackColor = true;
			//
			//cbxObjectRandomRotation
			//
			this.cbxObjectRandomRotation.Location = new System.Drawing.Point(134, 124);
			this.cbxObjectRandomRotation.Margin = new System.Windows.Forms.Padding(4);
			this.cbxObjectRandomRotation.Name = "cbxObjectRandomRotation";
			this.cbxObjectRandomRotation.Size = new System.Drawing.Size(89, 21);
			this.cbxObjectRandomRotation.TabIndex = 48;
			this.cbxObjectRandomRotation.Text = "Random";
			this.cbxObjectRandomRotation.UseCompatibleTextRendering = true;
			this.cbxObjectRandomRotation.UseVisualStyleBackColor = true;
			//
			//Label32
			//
			this.Label32.Location = new System.Drawing.Point(16, 82);
			this.Label32.Name = "Label32";
			this.Label32.Size = new System.Drawing.Size(219, 38);
			this.Label32.TabIndex = 16;
			this.Label32.Text = "Players 8 and 9 only work with versions 3.1+";
			this.Label32.UseCompatibleTextRendering = true;
			//
			//Label22
			//
			this.Label22.Location = new System.Drawing.Point(16, 10);
			this.Label22.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.Label22.Name = "Label22";
			this.Label22.Size = new System.Drawing.Size(85, 25);
			this.Label22.TabIndex = 14;
			this.Label22.Text = "Player";
			this.Label22.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.Label22.UseCompatibleTextRendering = true;
			//
			//Panel2
			//
			this.Panel2.Controls.Add(this.btnObjectTypeSelect);
			this.Panel2.Controls.Add(this.TabControl1);
			this.Panel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Panel2.Location = new System.Drawing.Point(3, 195);
			this.Panel2.Name = "Panel2";
			this.Panel2.Size = new System.Drawing.Size(404, 359);
			this.Panel2.TabIndex = 1;
			//
			//btnObjectTypeSelect
			//
			this.btnObjectTypeSelect.Anchor = (System.Windows.Forms.AnchorStyles) (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			this.btnObjectTypeSelect.Location = new System.Drawing.Point(313, 0);
			this.btnObjectTypeSelect.Name = "btnObjectTypeSelect";
			this.btnObjectTypeSelect.Size = new System.Drawing.Size(90, 28);
			this.btnObjectTypeSelect.TabIndex = 58;
			this.btnObjectTypeSelect.Text = "Select";
			this.btnObjectTypeSelect.UseCompatibleTextRendering = true;
			this.btnObjectTypeSelect.UseVisualStyleBackColor = true;
			//
			//TabControl1
			//
			this.TabControl1.Appearance = System.Windows.Forms.TabAppearance.Buttons;
			this.TabControl1.Controls.Add(this.TabPage1);
			this.TabControl1.Controls.Add(this.TabPage2);
			this.TabControl1.Controls.Add(this.TabPage3);
			this.TabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.TabControl1.Location = new System.Drawing.Point(0, 0);
			this.TabControl1.Name = "TabControl1";
			this.TabControl1.SelectedIndex = 0;
			this.TabControl1.Size = new System.Drawing.Size(404, 359);
			this.TabControl1.TabIndex = 0;
			//
			//TabPage1
			//
			this.TabPage1.Controls.Add(this.dgvFeatures);
			this.TabPage1.Location = new System.Drawing.Point(4, 28);
			this.TabPage1.Name = "TabPage1";
			this.TabPage1.Size = new System.Drawing.Size(396, 327);
			this.TabPage1.TabIndex = 0;
			this.TabPage1.Text = "Features";
			this.TabPage1.UseVisualStyleBackColor = true;
			//
			//dgvFeatures
			//
			this.dgvFeatures.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
			this.dgvFeatures.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvFeatures.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvFeatures.Location = new System.Drawing.Point(0, 0);
			this.dgvFeatures.Name = "dgvFeatures";
			this.dgvFeatures.ReadOnly = true;
			this.dgvFeatures.RowHeadersVisible = false;
			this.dgvFeatures.RowTemplate.Height = 24;
			this.dgvFeatures.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dgvFeatures.Size = new System.Drawing.Size(396, 327);
			this.dgvFeatures.TabIndex = 0;
			//
			//TabPage2
			//
			this.TabPage2.Controls.Add(this.dgvStructures);
			this.TabPage2.Location = new System.Drawing.Point(4, 28);
			this.TabPage2.Name = "TabPage2";
			this.TabPage2.Size = new System.Drawing.Size(396, 327);
			this.TabPage2.TabIndex = 1;
			this.TabPage2.Text = "Structures";
			this.TabPage2.UseVisualStyleBackColor = true;
			//
			//dgvStructures
			//
			this.dgvStructures.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
			this.dgvStructures.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvStructures.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvStructures.Location = new System.Drawing.Point(0, 0);
			this.dgvStructures.Name = "dgvStructures";
			this.dgvStructures.ReadOnly = true;
			this.dgvStructures.RowHeadersVisible = false;
			this.dgvStructures.RowTemplate.Height = 24;
			this.dgvStructures.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dgvStructures.Size = new System.Drawing.Size(396, 327);
			this.dgvStructures.TabIndex = 1;
			//
			//TabPage3
			//
			this.TabPage3.Controls.Add(this.dgvDroids);
			this.TabPage3.Location = new System.Drawing.Point(4, 28);
			this.TabPage3.Name = "TabPage3";
			this.TabPage3.Size = new System.Drawing.Size(396, 327);
			this.TabPage3.TabIndex = 2;
			this.TabPage3.Text = "Droids";
			this.TabPage3.UseVisualStyleBackColor = true;
			//
			//dgvDroids
			//
			this.dgvDroids.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
			this.dgvDroids.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvDroids.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvDroids.Location = new System.Drawing.Point(0, 0);
			this.dgvDroids.Name = "dgvDroids";
			this.dgvDroids.ReadOnly = true;
			this.dgvDroids.RowHeadersVisible = false;
			this.dgvDroids.RowTemplate.Height = 24;
			this.dgvDroids.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dgvDroids.Size = new System.Drawing.Size(396, 327);
			this.dgvDroids.TabIndex = 1;
			//
			//tpObject
			//
			this.tpObject.Controls.Add(this.TableLayoutPanel8);
			this.tpObject.Location = new System.Drawing.Point(4, 51);
			this.tpObject.Margin = new System.Windows.Forms.Padding(4);
			this.tpObject.Name = "tpObject";
			this.tpObject.Size = new System.Drawing.Size(410, 557);
			this.tpObject.TabIndex = 6;
			this.tpObject.Text = "Object";
			this.tpObject.UseVisualStyleBackColor = true;
			//
			//TableLayoutPanel8
			//
			this.TableLayoutPanel8.ColumnCount = 1;
			this.TableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, (float) (100.0F)));
			this.TableLayoutPanel8.Controls.Add(this.TableLayoutPanel9, 0, 1);
			this.TableLayoutPanel8.Controls.Add(this.Panel14, 0, 0);
			this.TableLayoutPanel8.Dock = System.Windows.Forms.DockStyle.Fill;
			this.TableLayoutPanel8.Location = new System.Drawing.Point(0, 0);
			this.TableLayoutPanel8.Name = "TableLayoutPanel8";
			this.TableLayoutPanel8.RowCount = 3;
			this.TableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, (float) (350.0F)));
			this.TableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, (float) (192.0F)));
			this.TableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, (float) (100.0F)));
			this.TableLayoutPanel8.Size = new System.Drawing.Size(410, 557);
			this.TableLayoutPanel8.TabIndex = 55;
			//
			//TableLayoutPanel9
			//
			this.TableLayoutPanel9.ColumnCount = 2;
			this.TableLayoutPanel9.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, (float) (96.0F)));
			this.TableLayoutPanel9.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, (float) (100.0F)));
			this.TableLayoutPanel9.Controls.Add(this.cboDroidTurret3, 1, 5);
			this.TableLayoutPanel9.Controls.Add(this.cboDroidTurret2, 1, 4);
			this.TableLayoutPanel9.Controls.Add(this.Panel13, 0, 5);
			this.TableLayoutPanel9.Controls.Add(this.cboDroidTurret1, 1, 3);
			this.TableLayoutPanel9.Controls.Add(this.cboDroidPropulsion, 1, 2);
			this.TableLayoutPanel9.Controls.Add(this.cboDroidBody, 1, 1);
			this.TableLayoutPanel9.Controls.Add(this.cboDroidType, 1, 0);
			this.TableLayoutPanel9.Controls.Add(this.Panel12, 0, 4);
			this.TableLayoutPanel9.Controls.Add(this.Panel11, 0, 3);
			this.TableLayoutPanel9.Controls.Add(this.Panel10, 0, 2);
			this.TableLayoutPanel9.Controls.Add(this.Panel9, 0, 1);
			this.TableLayoutPanel9.Controls.Add(this.Panel8, 0, 0);
			this.TableLayoutPanel9.Dock = System.Windows.Forms.DockStyle.Fill;
			this.TableLayoutPanel9.Location = new System.Drawing.Point(3, 353);
			this.TableLayoutPanel9.Name = "TableLayoutPanel9";
			this.TableLayoutPanel9.RowCount = 6;
			this.TableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, (float) (16.66417F)));
			this.TableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, (float) (16.66717F)));
			this.TableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, (float) (16.66717F)));
			this.TableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, (float) (16.66717F)));
			this.TableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, (float) (16.66717F)));
			this.TableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, (float) (16.66717F)));
			this.TableLayoutPanel9.Size = new System.Drawing.Size(404, 186);
			this.TableLayoutPanel9.TabIndex = 0;
			//
			//cboDroidTurret3
			//
			this.cboDroidTurret3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.cboDroidTurret3.DropDownHeight = 512;
			this.cboDroidTurret3.DropDownWidth = 384;
			this.cboDroidTurret3.FormattingEnabled = true;
			this.cboDroidTurret3.IntegralHeight = false;
			this.cboDroidTurret3.Location = new System.Drawing.Point(100, 158);
			this.cboDroidTurret3.Margin = new System.Windows.Forms.Padding(4);
			this.cboDroidTurret3.Name = "cboDroidTurret3";
			this.cboDroidTurret3.Size = new System.Drawing.Size(300, 24);
			this.cboDroidTurret3.TabIndex = 48;
			//
			//cboDroidTurret2
			//
			this.cboDroidTurret2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.cboDroidTurret2.DropDownHeight = 512;
			this.cboDroidTurret2.DropDownWidth = 384;
			this.cboDroidTurret2.FormattingEnabled = true;
			this.cboDroidTurret2.IntegralHeight = false;
			this.cboDroidTurret2.Location = new System.Drawing.Point(100, 127);
			this.cboDroidTurret2.Margin = new System.Windows.Forms.Padding(4);
			this.cboDroidTurret2.Name = "cboDroidTurret2";
			this.cboDroidTurret2.Size = new System.Drawing.Size(300, 24);
			this.cboDroidTurret2.TabIndex = 47;
			//
			//Panel13
			//
			this.Panel13.Controls.Add(this.rdoDroidTurret3);
			this.Panel13.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Panel13.Location = new System.Drawing.Point(3, 157);
			this.Panel13.Name = "Panel13";
			this.Panel13.Size = new System.Drawing.Size(90, 26);
			this.Panel13.TabIndex = 5;
			//
			//rdoDroidTurret3
			//
			this.rdoDroidTurret3.AutoSize = true;
			this.rdoDroidTurret3.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.rdoDroidTurret3.Location = new System.Drawing.Point(48, 2);
			this.rdoDroidTurret3.Name = "rdoDroidTurret3";
			this.rdoDroidTurret3.Size = new System.Drawing.Size(33, 21);
			this.rdoDroidTurret3.TabIndex = 51;
			this.rdoDroidTurret3.TabStop = true;
			this.rdoDroidTurret3.Text = "3";
			this.rdoDroidTurret3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.rdoDroidTurret3.UseCompatibleTextRendering = true;
			this.rdoDroidTurret3.UseVisualStyleBackColor = true;
			//
			//cboDroidTurret1
			//
			this.cboDroidTurret1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.cboDroidTurret1.DropDownHeight = 512;
			this.cboDroidTurret1.DropDownWidth = 384;
			this.cboDroidTurret1.FormattingEnabled = true;
			this.cboDroidTurret1.IntegralHeight = false;
			this.cboDroidTurret1.Location = new System.Drawing.Point(100, 96);
			this.cboDroidTurret1.Margin = new System.Windows.Forms.Padding(4);
			this.cboDroidTurret1.Name = "cboDroidTurret1";
			this.cboDroidTurret1.Size = new System.Drawing.Size(300, 24);
			this.cboDroidTurret1.TabIndex = 45;
			//
			//cboDroidPropulsion
			//
			this.cboDroidPropulsion.Dock = System.Windows.Forms.DockStyle.Fill;
			this.cboDroidPropulsion.DropDownHeight = 512;
			this.cboDroidPropulsion.DropDownWidth = 384;
			this.cboDroidPropulsion.FormattingEnabled = true;
			this.cboDroidPropulsion.IntegralHeight = false;
			this.cboDroidPropulsion.Location = new System.Drawing.Point(100, 65);
			this.cboDroidPropulsion.Margin = new System.Windows.Forms.Padding(4);
			this.cboDroidPropulsion.Name = "cboDroidPropulsion";
			this.cboDroidPropulsion.Size = new System.Drawing.Size(300, 24);
			this.cboDroidPropulsion.TabIndex = 43;
			//
			//cboDroidBody
			//
			this.cboDroidBody.Dock = System.Windows.Forms.DockStyle.Fill;
			this.cboDroidBody.DropDownHeight = 512;
			this.cboDroidBody.DropDownWidth = 384;
			this.cboDroidBody.FormattingEnabled = true;
			this.cboDroidBody.IntegralHeight = false;
			this.cboDroidBody.Location = new System.Drawing.Point(100, 34);
			this.cboDroidBody.Margin = new System.Windows.Forms.Padding(4);
			this.cboDroidBody.Name = "cboDroidBody";
			this.cboDroidBody.Size = new System.Drawing.Size(300, 24);
			this.cboDroidBody.TabIndex = 41;
			//
			//cboDroidType
			//
			this.cboDroidType.Dock = System.Windows.Forms.DockStyle.Fill;
			this.cboDroidType.DropDownHeight = 512;
			this.cboDroidType.DropDownWidth = 384;
			this.cboDroidType.FormattingEnabled = true;
			this.cboDroidType.IntegralHeight = false;
			this.cboDroidType.Location = new System.Drawing.Point(100, 4);
			this.cboDroidType.Margin = new System.Windows.Forms.Padding(4);
			this.cboDroidType.Name = "cboDroidType";
			this.cboDroidType.Size = new System.Drawing.Size(300, 24);
			this.cboDroidType.TabIndex = 52;
			//
			//Panel12
			//
			this.Panel12.Controls.Add(this.rdoDroidTurret0);
			this.Panel12.Controls.Add(this.rdoDroidTurret2);
			this.Panel12.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Panel12.Location = new System.Drawing.Point(3, 126);
			this.Panel12.Name = "Panel12";
			this.Panel12.Size = new System.Drawing.Size(90, 25);
			this.Panel12.TabIndex = 4;
			//
			//rdoDroidTurret0
			//
			this.rdoDroidTurret0.AutoSize = true;
			this.rdoDroidTurret0.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.rdoDroidTurret0.Location = new System.Drawing.Point(7, 1);
			this.rdoDroidTurret0.Name = "rdoDroidTurret0";
			this.rdoDroidTurret0.Size = new System.Drawing.Size(33, 21);
			this.rdoDroidTurret0.TabIndex = 54;
			this.rdoDroidTurret0.TabStop = true;
			this.rdoDroidTurret0.Text = "0";
			this.rdoDroidTurret0.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.rdoDroidTurret0.UseCompatibleTextRendering = true;
			this.rdoDroidTurret0.UseVisualStyleBackColor = true;
			//
			//rdoDroidTurret2
			//
			this.rdoDroidTurret2.AutoSize = true;
			this.rdoDroidTurret2.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.rdoDroidTurret2.Location = new System.Drawing.Point(48, 1);
			this.rdoDroidTurret2.Name = "rdoDroidTurret2";
			this.rdoDroidTurret2.Size = new System.Drawing.Size(33, 21);
			this.rdoDroidTurret2.TabIndex = 50;
			this.rdoDroidTurret2.TabStop = true;
			this.rdoDroidTurret2.Text = "2";
			this.rdoDroidTurret2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.rdoDroidTurret2.UseCompatibleTextRendering = true;
			this.rdoDroidTurret2.UseVisualStyleBackColor = true;
			//
			//Panel11
			//
			this.Panel11.Controls.Add(this.Label39);
			this.Panel11.Controls.Add(this.rdoDroidTurret1);
			this.Panel11.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Panel11.Location = new System.Drawing.Point(3, 95);
			this.Panel11.Name = "Panel11";
			this.Panel11.Size = new System.Drawing.Size(90, 25);
			this.Panel11.TabIndex = 3;
			//
			//Label39
			//
			this.Label39.Location = new System.Drawing.Point(0, 0);
			this.Label39.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.Label39.Name = "Label39";
			this.Label39.Size = new System.Drawing.Size(49, 25);
			this.Label39.TabIndex = 46;
			this.Label39.Text = "Turrets";
			this.Label39.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.Label39.UseCompatibleTextRendering = true;
			//
			//rdoDroidTurret1
			//
			this.rdoDroidTurret1.AutoSize = true;
			this.rdoDroidTurret1.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.rdoDroidTurret1.Location = new System.Drawing.Point(49, 1);
			this.rdoDroidTurret1.Name = "rdoDroidTurret1";
			this.rdoDroidTurret1.Size = new System.Drawing.Size(33, 21);
			this.rdoDroidTurret1.TabIndex = 49;
			this.rdoDroidTurret1.TabStop = true;
			this.rdoDroidTurret1.Text = "1";
			this.rdoDroidTurret1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.rdoDroidTurret1.UseCompatibleTextRendering = true;
			this.rdoDroidTurret1.UseVisualStyleBackColor = true;
			//
			//Panel10
			//
			this.Panel10.Controls.Add(this.Label38);
			this.Panel10.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Panel10.Location = new System.Drawing.Point(3, 64);
			this.Panel10.Name = "Panel10";
			this.Panel10.Size = new System.Drawing.Size(90, 25);
			this.Panel10.TabIndex = 2;
			//
			//Label38
			//
			this.Label38.Location = new System.Drawing.Point(7, 1);
			this.Label38.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.Label38.Name = "Label38";
			this.Label38.Size = new System.Drawing.Size(75, 25);
			this.Label38.TabIndex = 44;
			this.Label38.Text = "Propulsion";
			this.Label38.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.Label38.UseCompatibleTextRendering = true;
			//
			//Panel9
			//
			this.Panel9.Controls.Add(this.Label37);
			this.Panel9.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Panel9.Location = new System.Drawing.Point(3, 33);
			this.Panel9.Name = "Panel9";
			this.Panel9.Size = new System.Drawing.Size(90, 25);
			this.Panel9.TabIndex = 1;
			//
			//Label37
			//
			this.Label37.Location = new System.Drawing.Point(8, 1);
			this.Label37.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.Label37.Name = "Label37";
			this.Label37.Size = new System.Drawing.Size(75, 25);
			this.Label37.TabIndex = 42;
			this.Label37.Text = "Body";
			this.Label37.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.Label37.UseCompatibleTextRendering = true;
			//
			//Panel8
			//
			this.Panel8.Controls.Add(this.Label40);
			this.Panel8.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Panel8.Location = new System.Drawing.Point(3, 3);
			this.Panel8.Name = "Panel8";
			this.Panel8.Size = new System.Drawing.Size(90, 24);
			this.Panel8.TabIndex = 0;
			//
			//Label40
			//
			this.Label40.Location = new System.Drawing.Point(8, 0);
			this.Label40.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.Label40.Name = "Label40";
			this.Label40.Size = new System.Drawing.Size(75, 25);
			this.Label40.TabIndex = 53;
			this.Label40.Text = "Type";
			this.Label40.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.Label40.UseCompatibleTextRendering = true;
			//
			//Panel14
			//
			this.Panel14.Controls.Add(this.btnFlatSelected);
			this.Panel14.Controls.Add(this.btnAlignObjects);
			this.Panel14.Controls.Add(this.Label31);
			this.Panel14.Controls.Add(this.Label30);
			this.Panel14.Controls.Add(this.cbxDesignableOnly);
			this.Panel14.Controls.Add(this.Label17);
			this.Panel14.Controls.Add(this.txtObjectLabel);
			this.Panel14.Controls.Add(this.Label35);
			this.Panel14.Controls.Add(this.btnDroidToDesign);
			this.Panel14.Controls.Add(this.Label24);
			this.Panel14.Controls.Add(this.lblObjectType);
			this.Panel14.Controls.Add(this.Label36);
			this.Panel14.Controls.Add(this.Label23);
			this.Panel14.Controls.Add(this.txtObjectHealth);
			this.Panel14.Controls.Add(this.txtObjectRotation);
			this.Panel14.Controls.Add(this.Label34);
			this.Panel14.Controls.Add(this.Label28);
			this.Panel14.Controls.Add(this.txtObjectPriority);
			this.Panel14.Controls.Add(this.Label25);
			this.Panel14.Controls.Add(this.Label33);
			this.Panel14.Controls.Add(this.txtObjectID);
			this.Panel14.Controls.Add(this.Label26);
			this.Panel14.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Panel14.Location = new System.Drawing.Point(3, 3);
			this.Panel14.Name = "Panel14";
			this.Panel14.Size = new System.Drawing.Size(404, 344);
			this.Panel14.TabIndex = 1;
			//
			//btnFlatSelected
			//
			this.btnFlatSelected.Location = new System.Drawing.Point(238, 147);
			this.btnFlatSelected.Margin = new System.Windows.Forms.Padding(4);
			this.btnFlatSelected.Name = "btnFlatSelected";
			this.btnFlatSelected.Size = new System.Drawing.Size(120, 30);
			this.btnFlatSelected.TabIndex = 7;
			this.btnFlatSelected.Text = "Flatten Terrain";
			this.btnFlatSelected.UseCompatibleTextRendering = true;
			this.btnFlatSelected.UseVisualStyleBackColor = true;
			//
			//btnAlignObjects
			//
			this.btnAlignObjects.Location = new System.Drawing.Point(238, 117);
			this.btnAlignObjects.Margin = new System.Windows.Forms.Padding(4);
			this.btnAlignObjects.Name = "btnAlignObjects";
			this.btnAlignObjects.Size = new System.Drawing.Size(120, 30);
			this.btnAlignObjects.TabIndex = 6;
			this.btnAlignObjects.Text = "Realign";
			this.btnAlignObjects.UseCompatibleTextRendering = true;
			this.btnAlignObjects.UseVisualStyleBackColor = true;
			//
			//Label31
			//
			this.Label31.Location = new System.Drawing.Point(238, 179);
			this.Label31.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.Label31.Name = "Label31";
			this.Label31.Size = new System.Drawing.Size(83, 27);
			this.Label31.TabIndex = 46;
			this.Label31.Text = "3.1+ only";
			this.Label31.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.Label31.UseCompatibleTextRendering = true;
			//
			//Label30
			//
			this.Label30.Location = new System.Drawing.Point(153, 213);
			this.Label30.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.Label30.Name = "Label30";
			this.Label30.Size = new System.Drawing.Size(88, 27);
			this.Label30.TabIndex = 45;
			this.Label30.Text = "2.3 only";
			this.Label30.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.Label30.UseCompatibleTextRendering = true;
			//
			//cbxDesignableOnly
			//
			this.cbxDesignableOnly.AutoSize = true;
			this.cbxDesignableOnly.Checked = true;
			this.cbxDesignableOnly.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbxDesignableOnly.Location = new System.Drawing.Point(258, 310);
			this.cbxDesignableOnly.Name = "cbxDesignableOnly";
			this.cbxDesignableOnly.Size = new System.Drawing.Size(132, 21);
			this.cbxDesignableOnly.TabIndex = 44;
			this.cbxDesignableOnly.Text = "Designables Only";
			this.cbxDesignableOnly.UseCompatibleTextRendering = true;
			this.cbxDesignableOnly.UseVisualStyleBackColor = true;
			//
			//Label17
			//
			this.Label17.Location = new System.Drawing.Point(21, 180);
			this.Label17.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.Label17.Name = "Label17";
			this.Label17.Size = new System.Drawing.Size(62, 25);
			this.Label17.TabIndex = 43;
			this.Label17.Text = "Label:";
			this.Label17.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.Label17.UseCompatibleTextRendering = true;
			//
			//txtObjectLabel
			//
			this.txtObjectLabel.Location = new System.Drawing.Point(91, 181);
			this.txtObjectLabel.Margin = new System.Windows.Forms.Padding(4);
			this.txtObjectLabel.Name = "txtObjectLabel";
			this.txtObjectLabel.Size = new System.Drawing.Size(137, 22);
			this.txtObjectLabel.TabIndex = 42;
			//
			//Label35
			//
			this.Label35.Location = new System.Drawing.Point(153, 243);
			this.Label35.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.Label35.Name = "Label35";
			this.Label35.Size = new System.Drawing.Size(88, 27);
			this.Label35.TabIndex = 41;
			this.Label35.Text = "3.1+ only";
			this.Label35.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.Label35.UseCompatibleTextRendering = true;
			//
			//btnDroidToDesign
			//
			this.btnDroidToDesign.Location = new System.Drawing.Point(21, 300);
			this.btnDroidToDesign.Name = "btnDroidToDesign";
			this.btnDroidToDesign.Size = new System.Drawing.Size(231, 31);
			this.btnDroidToDesign.TabIndex = 40;
			this.btnDroidToDesign.Text = "Convert Templates To Design";
			this.btnDroidToDesign.UseCompatibleTextRendering = true;
			this.btnDroidToDesign.UseVisualStyleBackColor = true;
			//
			//Label24
			//
			this.Label24.Location = new System.Drawing.Point(4, 9);
			this.Label24.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.Label24.Name = "Label24";
			this.Label24.Size = new System.Drawing.Size(107, 20);
			this.Label24.TabIndex = 18;
			this.Label24.Text = "Type:";
			this.Label24.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.Label24.UseCompatibleTextRendering = true;
			//
			//lblObjectType
			//
			this.lblObjectType.Location = new System.Drawing.Point(4, 29);
			this.lblObjectType.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lblObjectType.Name = "lblObjectType";
			this.lblObjectType.Size = new System.Drawing.Size(304, 26);
			this.lblObjectType.TabIndex = 20;
			this.lblObjectType.Text = "Object Type";
			this.lblObjectType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lblObjectType.UseCompatibleTextRendering = true;
			//
			//Label36
			//
			this.Label36.Location = new System.Drawing.Point(21, 270);
			this.Label36.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.Label36.Name = "Label36";
			this.Label36.Size = new System.Drawing.Size(300, 27);
			this.Label36.TabIndex = 39;
			this.Label36.Text = "Designed droids will only exist in 3.1+";
			this.Label36.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.Label36.UseCompatibleTextRendering = true;
			//
			//Label23
			//
			this.Label23.Location = new System.Drawing.Point(8, 120);
			this.Label23.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.Label23.Name = "Label23";
			this.Label23.Size = new System.Drawing.Size(75, 25);
			this.Label23.TabIndex = 21;
			this.Label23.Text = "Rotation:";
			this.Label23.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.Label23.UseCompatibleTextRendering = true;
			//
			//txtObjectHealth
			//
			this.txtObjectHealth.Location = new System.Drawing.Point(91, 245);
			this.txtObjectHealth.Margin = new System.Windows.Forms.Padding(4);
			this.txtObjectHealth.Name = "txtObjectHealth";
			this.txtObjectHealth.Size = new System.Drawing.Size(54, 22);
			this.txtObjectHealth.TabIndex = 37;
			this.txtObjectHealth.Text = "#";
			this.txtObjectHealth.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			//
			//txtObjectRotation
			//
			this.txtObjectRotation.Location = new System.Drawing.Point(91, 121);
			this.txtObjectRotation.Margin = new System.Windows.Forms.Padding(4);
			this.txtObjectRotation.Name = "txtObjectRotation";
			this.txtObjectRotation.Size = new System.Drawing.Size(41, 22);
			this.txtObjectRotation.TabIndex = 25;
			this.txtObjectRotation.Text = "#";
			this.txtObjectRotation.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			//
			//Label34
			//
			this.Label34.Location = new System.Drawing.Point(20, 244);
			this.Label34.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.Label34.Name = "Label34";
			this.Label34.Size = new System.Drawing.Size(63, 25);
			this.Label34.TabIndex = 36;
			this.Label34.Text = "Health %";
			this.Label34.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.Label34.UseCompatibleTextRendering = true;
			//
			//Label28
			//
			this.Label28.Location = new System.Drawing.Point(4, 58);
			this.Label28.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.Label28.Name = "Label28";
			this.Label28.Size = new System.Drawing.Size(53, 25);
			this.Label28.TabIndex = 28;
			this.Label28.Text = "Player:";
			this.Label28.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.Label28.UseCompatibleTextRendering = true;
			//
			//txtObjectPriority
			//
			this.txtObjectPriority.Location = new System.Drawing.Point(91, 215);
			this.txtObjectPriority.Margin = new System.Windows.Forms.Padding(4);
			this.txtObjectPriority.Name = "txtObjectPriority";
			this.txtObjectPriority.Size = new System.Drawing.Size(54, 22);
			this.txtObjectPriority.TabIndex = 35;
			this.txtObjectPriority.Text = "#";
			this.txtObjectPriority.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			//
			//Label25
			//
			this.Label25.Location = new System.Drawing.Point(47, 150);
			this.Label25.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.Label25.Name = "Label25";
			this.Label25.Size = new System.Drawing.Size(36, 25);
			this.Label25.TabIndex = 30;
			this.Label25.Text = "ID:";
			this.Label25.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.Label25.UseCompatibleTextRendering = true;
			//
			//Label33
			//
			this.Label33.Location = new System.Drawing.Point(20, 214);
			this.Label33.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.Label33.Name = "Label33";
			this.Label33.Size = new System.Drawing.Size(63, 25);
			this.Label33.TabIndex = 34;
			this.Label33.Text = "Priority:";
			this.Label33.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.Label33.UseCompatibleTextRendering = true;
			//
			//txtObjectID
			//
			this.txtObjectID.Location = new System.Drawing.Point(91, 151);
			this.txtObjectID.Margin = new System.Windows.Forms.Padding(4);
			this.txtObjectID.Name = "txtObjectID";
			this.txtObjectID.Size = new System.Drawing.Size(80, 22);
			this.txtObjectID.TabIndex = 31;
			this.txtObjectID.Text = "#";
			this.txtObjectID.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			//
			//Label26
			//
			this.Label26.Location = new System.Drawing.Point(140, 120);
			this.Label26.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.Label26.Name = "Label26";
			this.Label26.Size = new System.Drawing.Size(88, 25);
			this.Label26.TabIndex = 33;
			this.Label26.Text = "(0-359)";
			this.Label26.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.Label26.UseCompatibleTextRendering = true;
			//
			//tpLabels
			//
			this.tpLabels.Controls.Add(this.Label11);
			this.tpLabels.Controls.Add(this.Label43);
			this.tpLabels.Controls.Add(this.Label42);
			this.tpLabels.Controls.Add(this.lstScriptAreas);
			this.tpLabels.Controls.Add(this.lstScriptPositions);
			this.tpLabels.Controls.Add(this.btnScriptAreaCreate);
			this.tpLabels.Controls.Add(this.GroupBox1);
			this.tpLabels.Location = new System.Drawing.Point(4, 51);
			this.tpLabels.Name = "tpLabels";
			this.tpLabels.Size = new System.Drawing.Size(410, 557);
			this.tpLabels.TabIndex = 7;
			this.tpLabels.Text = "Labels";
			this.tpLabels.UseVisualStyleBackColor = true;
			//
			//Label11
			//
			this.Label11.Location = new System.Drawing.Point(238, 11);
			this.Label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.Label11.Name = "Label11";
			this.Label11.Size = new System.Drawing.Size(142, 55);
			this.Label11.TabIndex = 53;
			this.Label11.Text = "Hold P and click to make positions.";
			this.Label11.UseCompatibleTextRendering = true;
			//
			//Label43
			//
			this.Label43.AutoSize = true;
			this.Label43.Location = new System.Drawing.Point(203, 66);
			this.Label43.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.Label43.Name = "Label43";
			this.Label43.Size = new System.Drawing.Size(44, 20);
			this.Label43.TabIndex = 52;
			this.Label43.Text = "Areas:";
			this.Label43.UseCompatibleTextRendering = true;
			//
			//Label42
			//
			this.Label42.AutoSize = true;
			this.Label42.Location = new System.Drawing.Point(18, 66);
			this.Label42.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.Label42.Name = "Label42";
			this.Label42.Size = new System.Drawing.Size(63, 20);
			this.Label42.TabIndex = 51;
			this.Label42.Text = "Positions:";
			this.Label42.UseCompatibleTextRendering = true;
			//
			//lstScriptAreas
			//
			this.lstScriptAreas.FormattingEnabled = true;
			this.lstScriptAreas.ItemHeight = 16;
			this.lstScriptAreas.Location = new System.Drawing.Point(202, 89);
			this.lstScriptAreas.Name = "lstScriptAreas";
			this.lstScriptAreas.Size = new System.Drawing.Size(178, 148);
			this.lstScriptAreas.TabIndex = 47;
			//
			//lstScriptPositions
			//
			this.lstScriptPositions.FormattingEnabled = true;
			this.lstScriptPositions.ItemHeight = 16;
			this.lstScriptPositions.Location = new System.Drawing.Point(18, 89);
			this.lstScriptPositions.Name = "lstScriptPositions";
			this.lstScriptPositions.Size = new System.Drawing.Size(178, 148);
			this.lstScriptPositions.TabIndex = 46;
			//
			//btnScriptAreaCreate
			//
			this.btnScriptAreaCreate.Location = new System.Drawing.Point(18, 20);
			this.btnScriptAreaCreate.Name = "btnScriptAreaCreate";
			this.btnScriptAreaCreate.Size = new System.Drawing.Size(201, 31);
			this.btnScriptAreaCreate.TabIndex = 45;
			this.btnScriptAreaCreate.Text = "Create Area From Selection";
			this.btnScriptAreaCreate.UseCompatibleTextRendering = true;
			this.btnScriptAreaCreate.UseVisualStyleBackColor = true;
			//
			//GroupBox1
			//
			this.GroupBox1.Controls.Add(this.txtScriptMarkerLabel);
			this.GroupBox1.Controls.Add(this.Label41);
			this.GroupBox1.Controls.Add(this.btnScriptMarkerRemove);
			this.GroupBox1.Controls.Add(this.Label2);
			this.GroupBox1.Controls.Add(this.txtScriptMarkerY);
			this.GroupBox1.Controls.Add(this.txtScriptMarkerX);
			this.GroupBox1.Controls.Add(this.Label7);
			this.GroupBox1.Controls.Add(this.txtScriptMarkerY2);
			this.GroupBox1.Controls.Add(this.Label4);
			this.GroupBox1.Controls.Add(this.Label8);
			this.GroupBox1.Controls.Add(this.txtScriptMarkerX2);
			this.GroupBox1.Location = new System.Drawing.Point(18, 243);
			this.GroupBox1.Name = "GroupBox1";
			this.GroupBox1.Size = new System.Drawing.Size(178, 231);
			this.GroupBox1.TabIndex = 44;
			this.GroupBox1.TabStop = false;
			this.GroupBox1.Text = "Selected Marker";
			this.GroupBox1.UseCompatibleTextRendering = true;
			//
			//txtScriptMarkerLabel
			//
			this.txtScriptMarkerLabel.Location = new System.Drawing.Point(56, 32);
			this.txtScriptMarkerLabel.Margin = new System.Windows.Forms.Padding(4);
			this.txtScriptMarkerLabel.Name = "txtScriptMarkerLabel";
			this.txtScriptMarkerLabel.Size = new System.Drawing.Size(96, 22);
			this.txtScriptMarkerLabel.TabIndex = 50;
			//
			//Label41
			//
			this.Label41.AutoSize = true;
			this.Label41.Location = new System.Drawing.Point(7, 35);
			this.Label41.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.Label41.Name = "Label41";
			this.Label41.Size = new System.Drawing.Size(41, 20);
			this.Label41.TabIndex = 49;
			this.Label41.Text = "Label:";
			this.Label41.UseCompatibleTextRendering = true;
			//
			//btnScriptMarkerRemove
			//
			this.btnScriptMarkerRemove.Location = new System.Drawing.Point(10, 185);
			this.btnScriptMarkerRemove.Name = "btnScriptMarkerRemove";
			this.btnScriptMarkerRemove.Size = new System.Drawing.Size(142, 31);
			this.btnScriptMarkerRemove.TabIndex = 48;
			this.btnScriptMarkerRemove.Text = "Remove";
			this.btnScriptMarkerRemove.UseCompatibleTextRendering = true;
			this.btnScriptMarkerRemove.UseVisualStyleBackColor = true;
			//
			//Label2
			//
			this.Label2.AutoSize = true;
			this.Label2.Location = new System.Drawing.Point(22, 66);
			this.Label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.Label2.Name = "Label2";
			this.Label2.Size = new System.Drawing.Size(15, 20);
			this.Label2.TabIndex = 33;
			this.Label2.Text = "x:";
			this.Label2.UseCompatibleTextRendering = true;
			//
			//txtScriptMarkerY
			//
			this.txtScriptMarkerY.Location = new System.Drawing.Point(91, 87);
			this.txtScriptMarkerY.Margin = new System.Windows.Forms.Padding(4);
			this.txtScriptMarkerY.Name = "txtScriptMarkerY";
			this.txtScriptMarkerY.Size = new System.Drawing.Size(61, 22);
			this.txtScriptMarkerY.TabIndex = 34;
			//
			//txtScriptMarkerX
			//
			this.txtScriptMarkerX.Location = new System.Drawing.Point(22, 87);
			this.txtScriptMarkerX.Margin = new System.Windows.Forms.Padding(4);
			this.txtScriptMarkerX.Name = "txtScriptMarkerX";
			this.txtScriptMarkerX.Size = new System.Drawing.Size(61, 22);
			this.txtScriptMarkerX.TabIndex = 32;
			//
			//Label7
			//
			this.Label7.AutoSize = true;
			this.Label7.Location = new System.Drawing.Point(91, 66);
			this.Label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.Label7.Name = "Label7";
			this.Label7.Size = new System.Drawing.Size(15, 20);
			this.Label7.TabIndex = 35;
			this.Label7.Text = "y:";
			this.Label7.UseCompatibleTextRendering = true;
			//
			//txtScriptMarkerY2
			//
			this.txtScriptMarkerY2.Location = new System.Drawing.Point(91, 143);
			this.txtScriptMarkerY2.Margin = new System.Windows.Forms.Padding(4);
			this.txtScriptMarkerY2.Name = "txtScriptMarkerY2";
			this.txtScriptMarkerY2.Size = new System.Drawing.Size(61, 22);
			this.txtScriptMarkerY2.TabIndex = 38;
			//
			//Label4
			//
			this.Label4.AutoSize = true;
			this.Label4.Location = new System.Drawing.Point(91, 122);
			this.Label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.Label4.Name = "Label4";
			this.Label4.Size = new System.Drawing.Size(23, 20);
			this.Label4.TabIndex = 39;
			this.Label4.Text = "y2:";
			this.Label4.UseCompatibleTextRendering = true;
			//
			//Label8
			//
			this.Label8.AutoSize = true;
			this.Label8.Location = new System.Drawing.Point(22, 122);
			this.Label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.Label8.Name = "Label8";
			this.Label8.Size = new System.Drawing.Size(23, 20);
			this.Label8.TabIndex = 37;
			this.Label8.Text = "x2:";
			this.Label8.UseCompatibleTextRendering = true;
			//
			//txtScriptMarkerX2
			//
			this.txtScriptMarkerX2.Location = new System.Drawing.Point(22, 143);
			this.txtScriptMarkerX2.Margin = new System.Windows.Forms.Padding(4);
			this.txtScriptMarkerX2.Name = "txtScriptMarkerX2";
			this.txtScriptMarkerX2.Size = new System.Drawing.Size(61, 22);
			this.txtScriptMarkerX2.TabIndex = 36;
			//
			//TableLayoutPanel7
			//
			this.TableLayoutPanel7.ColumnCount = 1;
			this.TableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, (float) (100.0F)));
			this.TableLayoutPanel7.Controls.Add(this.Panel7, 0, 0);
			this.TableLayoutPanel7.Controls.Add(this.pnlView, 0, 1);
			this.TableLayoutPanel7.Dock = System.Windows.Forms.DockStyle.Fill;
			this.TableLayoutPanel7.Location = new System.Drawing.Point(0, 0);
			this.TableLayoutPanel7.Margin = new System.Windows.Forms.Padding(4);
			this.TableLayoutPanel7.Name = "TableLayoutPanel7";
			this.TableLayoutPanel7.RowCount = 2;
			this.TableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, (float) (30.0F)));
			this.TableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, (float) (100.0F)));
			this.TableLayoutPanel7.Size = new System.Drawing.Size(858, 612);
			this.TableLayoutPanel7.TabIndex = 2;
			//
			//Panel7
			//
			this.Panel7.Controls.Add(this.tsTools);
			this.Panel7.Controls.Add(this.tsFile);
			this.Panel7.Controls.Add(this.tsSelection);
			this.Panel7.Controls.Add(this.tsMinimap);
			this.Panel7.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Panel7.Location = new System.Drawing.Point(0, 0);
			this.Panel7.Margin = new System.Windows.Forms.Padding(0);
			this.Panel7.Name = "Panel7";
			this.Panel7.Size = new System.Drawing.Size(858, 30);
			this.Panel7.TabIndex = 0;
			//
			//tsTools
			//
			this.tsTools.Dock = System.Windows.Forms.DockStyle.None;
			this.tsTools.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.tsTools.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {this.tsbGateways, this.tsbDrawAutotexture, this.tsbDrawTileOrientation});
			this.tsTools.Location = new System.Drawing.Point(372, 2);
			this.tsTools.Name = "tsTools";
			this.tsTools.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.tsTools.Size = new System.Drawing.Size(72, 25);
			this.tsTools.TabIndex = 2;
			//
			//tsbGateways
			//
			this.tsbGateways.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbGateways.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbGateways.Name = "tsbGateways";
			this.tsbGateways.Size = new System.Drawing.Size(23, 22);
			this.tsbGateways.Text = "Gateways";
			//
			//tsbDrawAutotexture
			//
			this.tsbDrawAutotexture.CheckOnClick = true;
			this.tsbDrawAutotexture.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbDrawAutotexture.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbDrawAutotexture.Name = "tsbDrawAutotexture";
			this.tsbDrawAutotexture.Size = new System.Drawing.Size(23, 22);
			this.tsbDrawAutotexture.Text = "Display Painted Texture Markers";
			//
			//tsbDrawTileOrientation
			//
			this.tsbDrawTileOrientation.CheckOnClick = true;
			this.tsbDrawTileOrientation.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbDrawTileOrientation.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbDrawTileOrientation.Name = "tsbDrawTileOrientation";
			this.tsbDrawTileOrientation.Size = new System.Drawing.Size(23, 22);
			this.tsbDrawTileOrientation.Text = "Display Texture Orientations";
			//
			//tsFile
			//
			this.tsFile.Dock = System.Windows.Forms.DockStyle.None;
			this.tsFile.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.tsFile.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {this.tsbSave});
			this.tsFile.Location = new System.Drawing.Point(453, 2);
			this.tsFile.Name = "tsFile";
			this.tsFile.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.tsFile.Size = new System.Drawing.Size(26, 25);
			this.tsFile.TabIndex = 3;
			//
			//tsbSave
			//
			this.tsbSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbSave.Enabled = false;
			this.tsbSave.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbSave.Name = "tsbSave";
			this.tsbSave.Size = new System.Drawing.Size(23, 22);
			//
			//tsSelection
			//
			this.tsSelection.Dock = System.Windows.Forms.DockStyle.None;
			this.tsSelection.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.tsSelection.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {this.ToolStripLabel1, this.tsbSelection, this.tsbSelectionCopy, this.tsbSelectionPasteOptions, this.tsbSelectionPaste, this.tsbSelectionRotateCounterClockwise, this.tsbSelectionRotateClockwise, this.tsbSelectionFlipX, this.tsbSelectionObjects});
			this.tsSelection.Location = new System.Drawing.Point(98, 0);
			this.tsSelection.Name = "tsSelection";
			this.tsSelection.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.tsSelection.Size = new System.Drawing.Size(250, 25);
			this.tsSelection.TabIndex = 0;
			this.tsSelection.Text = "ToolStrip1";
			//
			//ToolStripLabel1
			//
			this.ToolStripLabel1.Name = "ToolStripLabel1";
			this.ToolStripLabel1.Size = new System.Drawing.Size(73, 22);
			this.ToolStripLabel1.Text = "Selection:";
			//
			//tsbSelection
			//
			this.tsbSelection.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbSelection.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbSelection.Name = "tsbSelection";
			this.tsbSelection.Size = new System.Drawing.Size(23, 22);
			this.tsbSelection.Text = "Selection Tool";
			//
			//tsbSelectionCopy
			//
			this.tsbSelectionCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbSelectionCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbSelectionCopy.Name = "tsbSelectionCopy";
			this.tsbSelectionCopy.Size = new System.Drawing.Size(23, 22);
			this.tsbSelectionCopy.Text = "Copy Selection";
			//
			//tsbSelectionPasteOptions
			//
			this.tsbSelectionPasteOptions.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbSelectionPasteOptions.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {this.menuRotateUnits, this.menuRotateWalls, this.menuRotateNothing, this.ToolStripSeparator10, this.menuSelPasteHeights, this.menuSelPasteTextures, this.menuSelPasteUnits, this.menuSelPasteGateways, this.menuSelPasteDeleteUnits, this.menuSelPasteDeleteGateways});
			this.tsbSelectionPasteOptions.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbSelectionPasteOptions.Name = "tsbSelectionPasteOptions";
			this.tsbSelectionPasteOptions.Size = new System.Drawing.Size(13, 22);
			this.tsbSelectionPasteOptions.Text = "Paste Options";
			//
			//menuRotateUnits
			//
			this.menuRotateUnits.Name = "menuRotateUnits";
			this.menuRotateUnits.Size = new System.Drawing.Size(244, 24);
			this.menuRotateUnits.Text = "Rotate All Objects";
			//
			//menuRotateWalls
			//
			this.menuRotateWalls.Checked = true;
			this.menuRotateWalls.CheckState = System.Windows.Forms.CheckState.Checked;
			this.menuRotateWalls.Name = "menuRotateWalls";
			this.menuRotateWalls.Size = new System.Drawing.Size(244, 24);
			this.menuRotateWalls.Text = "Rotate Walls Only";
			//
			//menuRotateNothing
			//
			this.menuRotateNothing.Name = "menuRotateNothing";
			this.menuRotateNothing.Size = new System.Drawing.Size(244, 24);
			this.menuRotateNothing.Text = "No Object Rotation";
			//
			//ToolStripSeparator10
			//
			this.ToolStripSeparator10.Name = "ToolStripSeparator10";
			this.ToolStripSeparator10.Size = new System.Drawing.Size(241, 6);
			//
			//menuSelPasteHeights
			//
			this.menuSelPasteHeights.Checked = true;
			this.menuSelPasteHeights.CheckOnClick = true;
			this.menuSelPasteHeights.CheckState = System.Windows.Forms.CheckState.Checked;
			this.menuSelPasteHeights.Name = "menuSelPasteHeights";
			this.menuSelPasteHeights.Size = new System.Drawing.Size(244, 24);
			this.menuSelPasteHeights.Text = "Paste Heights";
			//
			//menuSelPasteTextures
			//
			this.menuSelPasteTextures.Checked = true;
			this.menuSelPasteTextures.CheckOnClick = true;
			this.menuSelPasteTextures.CheckState = System.Windows.Forms.CheckState.Checked;
			this.menuSelPasteTextures.Name = "menuSelPasteTextures";
			this.menuSelPasteTextures.Size = new System.Drawing.Size(244, 24);
			this.menuSelPasteTextures.Text = "Paste Textures";
			//
			//menuSelPasteUnits
			//
			this.menuSelPasteUnits.CheckOnClick = true;
			this.menuSelPasteUnits.Name = "menuSelPasteUnits";
			this.menuSelPasteUnits.Size = new System.Drawing.Size(244, 24);
			this.menuSelPasteUnits.Text = "Paste Objects";
			//
			//menuSelPasteGateways
			//
			this.menuSelPasteGateways.CheckOnClick = true;
			this.menuSelPasteGateways.Name = "menuSelPasteGateways";
			this.menuSelPasteGateways.Size = new System.Drawing.Size(244, 24);
			this.menuSelPasteGateways.Text = "Paste Gateways";
			//
			//menuSelPasteDeleteUnits
			//
			this.menuSelPasteDeleteUnits.CheckOnClick = true;
			this.menuSelPasteDeleteUnits.Name = "menuSelPasteDeleteUnits";
			this.menuSelPasteDeleteUnits.Size = new System.Drawing.Size(244, 24);
			this.menuSelPasteDeleteUnits.Text = "Delete Existing Objects";
			//
			//menuSelPasteDeleteGateways
			//
			this.menuSelPasteDeleteGateways.CheckOnClick = true;
			this.menuSelPasteDeleteGateways.Name = "menuSelPasteDeleteGateways";
			this.menuSelPasteDeleteGateways.Size = new System.Drawing.Size(244, 24);
			this.menuSelPasteDeleteGateways.Text = "Delete Existing Gateways";
			//
			//tsbSelectionPaste
			//
			this.tsbSelectionPaste.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbSelectionPaste.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbSelectionPaste.Name = "tsbSelectionPaste";
			this.tsbSelectionPaste.Size = new System.Drawing.Size(23, 22);
			this.tsbSelectionPaste.Text = "Paste To Selection";
			//
			//tsbSelectionRotateCounterClockwise
			//
			this.tsbSelectionRotateCounterClockwise.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbSelectionRotateCounterClockwise.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbSelectionRotateCounterClockwise.Name = "tsbSelectionRotateCounterClockwise";
			this.tsbSelectionRotateCounterClockwise.Size = new System.Drawing.Size(23, 22);
			this.tsbSelectionRotateCounterClockwise.Text = "Rotate Copy Counter Clockwise";
			//
			//tsbSelectionRotateClockwise
			//
			this.tsbSelectionRotateClockwise.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbSelectionRotateClockwise.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbSelectionRotateClockwise.Name = "tsbSelectionRotateClockwise";
			this.tsbSelectionRotateClockwise.Size = new System.Drawing.Size(23, 22);
			this.tsbSelectionRotateClockwise.Text = "Rotate Copy Clockwise";
			//
			//tsbSelectionFlipX
			//
			this.tsbSelectionFlipX.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbSelectionFlipX.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbSelectionFlipX.Name = "tsbSelectionFlipX";
			this.tsbSelectionFlipX.Size = new System.Drawing.Size(23, 22);
			this.tsbSelectionFlipX.Text = "Flip Copy Horizontally";
			//
			//tsbSelectionObjects
			//
			this.tsbSelectionObjects.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbSelectionObjects.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbSelectionObjects.Name = "tsbSelectionObjects";
			this.tsbSelectionObjects.Size = new System.Drawing.Size(23, 22);
			this.tsbSelectionObjects.Text = "Select Objects";
			//
			//tsMinimap
			//
			this.tsMinimap.Dock = System.Windows.Forms.DockStyle.None;
			this.tsMinimap.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.tsMinimap.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {this.menuMinimap});
			this.tsMinimap.Location = new System.Drawing.Point(0, 0);
			this.tsMinimap.Name = "tsMinimap";
			this.tsMinimap.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.tsMinimap.Size = new System.Drawing.Size(84, 27);
			this.tsMinimap.TabIndex = 1;
			//
			//menuMinimap
			//
			this.menuMinimap.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.menuMinimap.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {this.menuMiniShowTex, this.menuMiniShowHeight, this.menuMiniShowCliffs, this.menuMiniShowUnits, this.menuMiniShowGateways});
			this.menuMinimap.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.menuMinimap.Name = "menuMinimap";
			this.menuMinimap.Size = new System.Drawing.Size(81, 24);
			this.menuMinimap.Text = "Minimap";
			//
			//menuMiniShowTex
			//
			this.menuMiniShowTex.Checked = true;
			this.menuMiniShowTex.CheckOnClick = true;
			this.menuMiniShowTex.CheckState = System.Windows.Forms.CheckState.Checked;
			this.menuMiniShowTex.Name = "menuMiniShowTex";
			this.menuMiniShowTex.Size = new System.Drawing.Size(181, 24);
			this.menuMiniShowTex.Text = "Show Textures";
			//
			//menuMiniShowHeight
			//
			this.menuMiniShowHeight.Checked = true;
			this.menuMiniShowHeight.CheckOnClick = true;
			this.menuMiniShowHeight.CheckState = System.Windows.Forms.CheckState.Checked;
			this.menuMiniShowHeight.Name = "menuMiniShowHeight";
			this.menuMiniShowHeight.Size = new System.Drawing.Size(181, 24);
			this.menuMiniShowHeight.Text = "Show Heights";
			//
			//menuMiniShowCliffs
			//
			this.menuMiniShowCliffs.CheckOnClick = true;
			this.menuMiniShowCliffs.Name = "menuMiniShowCliffs";
			this.menuMiniShowCliffs.Size = new System.Drawing.Size(181, 24);
			this.menuMiniShowCliffs.Text = "Show Cliffs";
			//
			//menuMiniShowUnits
			//
			this.menuMiniShowUnits.Checked = true;
			this.menuMiniShowUnits.CheckOnClick = true;
			this.menuMiniShowUnits.CheckState = System.Windows.Forms.CheckState.Checked;
			this.menuMiniShowUnits.Name = "menuMiniShowUnits";
			this.menuMiniShowUnits.Size = new System.Drawing.Size(181, 24);
			this.menuMiniShowUnits.Text = "Show Objects";
			//
			//menuMiniShowGateways
			//
			this.menuMiniShowGateways.CheckOnClick = true;
			this.menuMiniShowGateways.Name = "menuMiniShowGateways";
			this.menuMiniShowGateways.Size = new System.Drawing.Size(181, 24);
			this.menuMiniShowGateways.Text = "Show Gateways";
			//
			//pnlView
			//
			this.pnlView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlView.Location = new System.Drawing.Point(0, 30);
			this.pnlView.Margin = new System.Windows.Forms.Padding(0);
			this.pnlView.Name = "pnlView";
			this.pnlView.Size = new System.Drawing.Size(858, 582);
			this.pnlView.TabIndex = 1;
			//
			//menuMain
			//
			this.menuMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.menuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {this.menuFile, this.menuTools, this.menuOptions});
			this.menuMain.Location = new System.Drawing.Point(0, 0);
			this.menuMain.Name = "menuMain";
			this.menuMain.Padding = new System.Windows.Forms.Padding(8, 2, 0, 2);
			this.menuMain.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.menuMain.Size = new System.Drawing.Size(1296, 31);
			this.menuMain.TabIndex = 0;
			this.menuMain.Text = "MenuStrip1";
			//
			//menuFile
			//
			this.menuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {this.NewMapToolStripMenuItem, this.ToolStripSeparator3, this.OpenToolStripMenuItem, this.ToolStripSeparator2, this.SaveToolStripMenuItem, this.ToolStripSeparator1, this.ToolStripMenuItem4, this.ToolStripMenuItem2, this.MapWZToolStripMenuItem, this.ToolStripSeparator4, this.CloseToolStripMenuItem});
			this.menuFile.Name = "menuFile";
			this.menuFile.Size = new System.Drawing.Size(44, 27);
			this.menuFile.Text = "File";
			//
			//NewMapToolStripMenuItem
			//
			this.NewMapToolStripMenuItem.Name = "NewMapToolStripMenuItem";
			this.NewMapToolStripMenuItem.Size = new System.Drawing.Size(177, 24);
			this.NewMapToolStripMenuItem.Text = "New Map";
			//
			//ToolStripSeparator3
			//
			this.ToolStripSeparator3.Name = "ToolStripSeparator3";
			this.ToolStripSeparator3.Size = new System.Drawing.Size(174, 6);
			//
			//OpenToolStripMenuItem
			//
			this.OpenToolStripMenuItem.Name = "OpenToolStripMenuItem";
			this.OpenToolStripMenuItem.Size = new System.Drawing.Size(177, 24);
			this.OpenToolStripMenuItem.Text = "Open...";
			//
			//ToolStripSeparator2
			//
			this.ToolStripSeparator2.Name = "ToolStripSeparator2";
			this.ToolStripSeparator2.Size = new System.Drawing.Size(174, 6);
			//
			//SaveToolStripMenuItem
			//
			this.SaveToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {this.menuSaveFMap, this.ToolStripSeparator7, this.menuSaveFMapQuick, this.ToolStripSeparator11, this.MapLNDToolStripMenuItem, this.ToolStripSeparator6, this.menuExportMapTileTypes, this.ToolStripMenuItem1, this.MinimapBMPToolStripMenuItem, this.ToolStripMenuItem3});
			this.SaveToolStripMenuItem.Name = "SaveToolStripMenuItem";
			this.SaveToolStripMenuItem.Size = new System.Drawing.Size(177, 24);
			this.SaveToolStripMenuItem.Text = "Save";
			//
			//menuSaveFMap
			//
			this.menuSaveFMap.Name = "menuSaveFMap";
			this.menuSaveFMap.Size = new System.Drawing.Size(284, 24);
			this.menuSaveFMap.Text = "Map fmap...";
			//
			//ToolStripSeparator7
			//
			this.ToolStripSeparator7.Name = "ToolStripSeparator7";
			this.ToolStripSeparator7.Size = new System.Drawing.Size(281, 6);
			//
			//menuSaveFMapQuick
			//
			this.menuSaveFMapQuick.Name = "menuSaveFMapQuick";
			this.menuSaveFMapQuick.Size = new System.Drawing.Size(284, 24);
			this.menuSaveFMapQuick.Text = "Quick Save fmap";
			//
			//ToolStripSeparator11
			//
			this.ToolStripSeparator11.Name = "ToolStripSeparator11";
			this.ToolStripSeparator11.Size = new System.Drawing.Size(281, 6);
			//
			//MapLNDToolStripMenuItem
			//
			this.MapLNDToolStripMenuItem.Name = "MapLNDToolStripMenuItem";
			this.MapLNDToolStripMenuItem.Size = new System.Drawing.Size(284, 24);
			this.MapLNDToolStripMenuItem.Text = "Export Map LND...";
			//
			//ToolStripSeparator6
			//
			this.ToolStripSeparator6.Name = "ToolStripSeparator6";
			this.ToolStripSeparator6.Size = new System.Drawing.Size(281, 6);
			//
			//menuExportMapTileTypes
			//
			this.menuExportMapTileTypes.Name = "menuExportMapTileTypes";
			this.menuExportMapTileTypes.Size = new System.Drawing.Size(284, 24);
			this.menuExportMapTileTypes.Text = "Export Tile Types...";
			//
			//ToolStripMenuItem1
			//
			this.ToolStripMenuItem1.Name = "ToolStripMenuItem1";
			this.ToolStripMenuItem1.Size = new System.Drawing.Size(281, 6);
			//
			//MinimapBMPToolStripMenuItem
			//
			this.MinimapBMPToolStripMenuItem.Name = "MinimapBMPToolStripMenuItem";
			this.MinimapBMPToolStripMenuItem.Size = new System.Drawing.Size(284, 24);
			this.MinimapBMPToolStripMenuItem.Text = "Minimap Bitmap...";
			//
			//ToolStripMenuItem3
			//
			this.ToolStripMenuItem3.Name = "ToolStripMenuItem3";
			this.ToolStripMenuItem3.Size = new System.Drawing.Size(284, 24);
			this.ToolStripMenuItem3.Text = "Heightmap Bitmap...";
			//
			//ToolStripSeparator1
			//
			this.ToolStripSeparator1.Name = "ToolStripSeparator1";
			this.ToolStripSeparator1.Size = new System.Drawing.Size(174, 6);
			//
			//ToolStripMenuItem4
			//
			this.ToolStripMenuItem4.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {this.ImportHeightmapToolStripMenuItem, this.ToolStripSeparator8, this.menuImportTileTypes});
			this.ToolStripMenuItem4.Name = "ToolStripMenuItem4";
			this.ToolStripMenuItem4.Size = new System.Drawing.Size(177, 24);
			this.ToolStripMenuItem4.Text = "Import";
			//
			//ImportHeightmapToolStripMenuItem
			//
			this.ImportHeightmapToolStripMenuItem.Name = "ImportHeightmapToolStripMenuItem";
			this.ImportHeightmapToolStripMenuItem.Size = new System.Drawing.Size(162, 24);
			this.ImportHeightmapToolStripMenuItem.Text = "Heightmap...";
			//
			//ToolStripSeparator8
			//
			this.ToolStripSeparator8.Name = "ToolStripSeparator8";
			this.ToolStripSeparator8.Size = new System.Drawing.Size(159, 6);
			//
			//menuImportTileTypes
			//
			this.menuImportTileTypes.Name = "menuImportTileTypes";
			this.menuImportTileTypes.Size = new System.Drawing.Size(162, 24);
			this.menuImportTileTypes.Text = "Tile Types...";
			//
			//ToolStripMenuItem2
			//
			this.ToolStripMenuItem2.Name = "ToolStripMenuItem2";
			this.ToolStripMenuItem2.Size = new System.Drawing.Size(174, 6);
			//
			//MapWZToolStripMenuItem
			//
			this.MapWZToolStripMenuItem.Name = "MapWZToolStripMenuItem";
			this.MapWZToolStripMenuItem.Size = new System.Drawing.Size(177, 24);
			this.MapWZToolStripMenuItem.Text = "Compile Map...";
			//
			//ToolStripSeparator4
			//
			this.ToolStripSeparator4.Name = "ToolStripSeparator4";
			this.ToolStripSeparator4.Size = new System.Drawing.Size(174, 6);
			//
			//CloseToolStripMenuItem
			//
			this.CloseToolStripMenuItem.Name = "CloseToolStripMenuItem";
			this.CloseToolStripMenuItem.Size = new System.Drawing.Size(177, 24);
			this.CloseToolStripMenuItem.Text = "Quit";
			//
			//menuTools
			//
			this.menuTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {this.menuReinterpret, this.menuWaterCorrection, this.ToolStripSeparator9, this.menuFlatOil, this.menuFlatStructures, this.ToolStripSeparator12, this.menuGenerator});
			this.menuTools.Name = "menuTools";
			this.menuTools.Size = new System.Drawing.Size(57, 27);
			this.menuTools.Text = "Tools";
			//
			//menuReinterpret
			//
			this.menuReinterpret.Name = "menuReinterpret";
			this.menuReinterpret.Size = new System.Drawing.Size(249, 24);
			this.menuReinterpret.Text = "Reinterpret Terrain";
			//
			//menuWaterCorrection
			//
			this.menuWaterCorrection.Name = "menuWaterCorrection";
			this.menuWaterCorrection.Size = new System.Drawing.Size(249, 24);
			this.menuWaterCorrection.Text = "Water Triangle Correction";
			//
			//ToolStripSeparator9
			//
			this.ToolStripSeparator9.Name = "ToolStripSeparator9";
			this.ToolStripSeparator9.Size = new System.Drawing.Size(246, 6);
			//
			//menuFlatOil
			//
			this.menuFlatOil.Name = "menuFlatOil";
			this.menuFlatOil.Size = new System.Drawing.Size(249, 24);
			this.menuFlatOil.Text = "Flatten Under Oils";
			//
			//menuFlatStructures
			//
			this.menuFlatStructures.Name = "menuFlatStructures";
			this.menuFlatStructures.Size = new System.Drawing.Size(249, 24);
			this.menuFlatStructures.Text = "Flatten Under Structures";
			//
			//ToolStripSeparator12
			//
			this.ToolStripSeparator12.Name = "ToolStripSeparator12";
			this.ToolStripSeparator12.Size = new System.Drawing.Size(246, 6);
			//
			//menuGenerator
			//
			this.menuGenerator.Name = "menuGenerator";
			this.menuGenerator.Size = new System.Drawing.Size(249, 24);
			this.menuGenerator.Text = "Generator...";
			//
			//menuOptions
			//
			this.menuOptions.Name = "menuOptions";
			this.menuOptions.Size = new System.Drawing.Size(82, 27);
			this.menuOptions.Text = "Options...";
			//
			//TableLayoutPanel5
			//
			this.TableLayoutPanel5.ColumnCount = 1;
			this.TableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, (float) (100.0F)));
			this.TableLayoutPanel5.Controls.Add(this.menuMain, 0, 0);
			this.TableLayoutPanel5.Controls.Add(this.SplitContainer1, 0, 1);
			this.TableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
			this.TableLayoutPanel5.Location = new System.Drawing.Point(0, 0);
			this.TableLayoutPanel5.Margin = new System.Windows.Forms.Padding(4);
			this.TableLayoutPanel5.Name = "TableLayoutPanel5";
			this.TableLayoutPanel5.RowCount = 2;
			this.TableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, (float) (31.0F)));
			this.TableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, (float) (100.0F)));
			this.TableLayoutPanel5.Size = new System.Drawing.Size(1296, 655);
			this.TableLayoutPanel5.TabIndex = 1;
			//
			//TabPage13
			//
			this.TabPage13.Location = new System.Drawing.Point(4, 24);
			this.TabPage13.Name = "TabPage13";
			this.TabPage13.Padding = new System.Windows.Forms.Padding(3);
			this.TabPage13.Size = new System.Drawing.Size(193, 0);
			this.TabPage13.TabIndex = 0;
			this.TabPage13.Text = "1";
			this.TabPage13.UseVisualStyleBackColor = true;
			//
			//TabPage14
			//
			this.TabPage14.Location = new System.Drawing.Point(4, 24);
			this.TabPage14.Name = "TabPage14";
			this.TabPage14.Padding = new System.Windows.Forms.Padding(3);
			this.TabPage14.Size = new System.Drawing.Size(193, 0);
			this.TabPage14.TabIndex = 1;
			this.TabPage14.Text = "2";
			this.TabPage14.UseVisualStyleBackColor = true;
			//
			//TabPage15
			//
			this.TabPage15.Location = new System.Drawing.Point(4, 24);
			this.TabPage15.Name = "TabPage15";
			this.TabPage15.Size = new System.Drawing.Size(193, 0);
			this.TabPage15.TabIndex = 2;
			this.TabPage15.Text = "3";
			this.TabPage15.UseVisualStyleBackColor = true;
			//
			//TabPage16
			//
			this.TabPage16.Location = new System.Drawing.Point(4, 24);
			this.TabPage16.Name = "TabPage16";
			this.TabPage16.Size = new System.Drawing.Size(193, 0);
			this.TabPage16.TabIndex = 3;
			this.TabPage16.Text = "4";
			this.TabPage16.UseVisualStyleBackColor = true;
			//
			//TabPage21
			//
			this.TabPage21.Location = new System.Drawing.Point(4, 24);
			this.TabPage21.Name = "TabPage21";
			this.TabPage21.Size = new System.Drawing.Size(193, 0);
			this.TabPage21.TabIndex = 4;
			this.TabPage21.Text = "5";
			this.TabPage21.UseVisualStyleBackColor = true;
			//
			//TabPage22
			//
			this.TabPage22.Location = new System.Drawing.Point(4, 24);
			this.TabPage22.Name = "TabPage22";
			this.TabPage22.Size = new System.Drawing.Size(193, 0);
			this.TabPage22.TabIndex = 5;
			this.TabPage22.Text = "6";
			this.TabPage22.UseVisualStyleBackColor = true;
			//
			//TabPage23
			//
			this.TabPage23.Location = new System.Drawing.Point(4, 24);
			this.TabPage23.Name = "TabPage23";
			this.TabPage23.Size = new System.Drawing.Size(193, 0);
			this.TabPage23.TabIndex = 6;
			this.TabPage23.Text = "7";
			this.TabPage23.UseVisualStyleBackColor = true;
			//
			//TabPage24
			//
			this.TabPage24.Location = new System.Drawing.Point(4, 24);
			this.TabPage24.Name = "TabPage24";
			this.TabPage24.Size = new System.Drawing.Size(193, 0);
			this.TabPage24.TabIndex = 7;
			this.TabPage24.Text = "8";
			this.TabPage24.UseVisualStyleBackColor = true;
			//
			//frmMain
			//
			this.AllowDrop = true;
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(1296, 655);
			this.Controls.Add(this.TableLayoutPanel5);
			this.MainMenuStrip = this.menuMain;
			this.Margin = new System.Windows.Forms.Padding(4);
			this.Name = "frmMain";
			this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
			this.SplitContainer1.Panel1.ResumeLayout(false);
			this.SplitContainer1.Panel2.ResumeLayout(false);
			this.SplitContainer1.ResumeLayout(false);
			this.TabControl.ResumeLayout(false);
			this.tpTextures.ResumeLayout(false);
			this.TableLayoutPanel6.ResumeLayout(false);
			this.Panel5.ResumeLayout(false);
			this.Panel5.PerformLayout();
			this.Panel6.ResumeLayout(false);
			this.tpAutoTexture.ResumeLayout(false);
			this.tpAutoTexture.PerformLayout();
			this.Panel15.ResumeLayout(false);
			this.Panel15.PerformLayout();
			this.tpHeight.ResumeLayout(false);
			this.tpHeight.PerformLayout();
			this.tabHeightSetR.ResumeLayout(false);
			this.tabHeightSetL.ResumeLayout(false);
			this.tpResize.ResumeLayout(false);
			this.tpResize.PerformLayout();
			this.tpObjects.ResumeLayout(false);
			this.TableLayoutPanel1.ResumeLayout(false);
			this.Panel1.ResumeLayout(false);
			this.Panel1.PerformLayout();
			this.GroupBox3.ResumeLayout(false);
			this.GroupBox3.PerformLayout();
			this.Panel2.ResumeLayout(false);
			this.TabControl1.ResumeLayout(false);
			this.TabPage1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize) this.dgvFeatures).EndInit();
			this.TabPage2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize) this.dgvStructures).EndInit();
			this.TabPage3.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize) this.dgvDroids).EndInit();
			this.tpObject.ResumeLayout(false);
			this.TableLayoutPanel8.ResumeLayout(false);
			this.TableLayoutPanel9.ResumeLayout(false);
			this.Panel13.ResumeLayout(false);
			this.Panel13.PerformLayout();
			this.Panel12.ResumeLayout(false);
			this.Panel12.PerformLayout();
			this.Panel11.ResumeLayout(false);
			this.Panel11.PerformLayout();
			this.Panel10.ResumeLayout(false);
			this.Panel9.ResumeLayout(false);
			this.Panel8.ResumeLayout(false);
			this.Panel14.ResumeLayout(false);
			this.Panel14.PerformLayout();
			this.tpLabels.ResumeLayout(false);
			this.tpLabels.PerformLayout();
			this.GroupBox1.ResumeLayout(false);
			this.GroupBox1.PerformLayout();
			this.TableLayoutPanel7.ResumeLayout(false);
			this.Panel7.ResumeLayout(false);
			this.Panel7.PerformLayout();
			this.tsTools.ResumeLayout(false);
			this.tsTools.PerformLayout();
			this.tsFile.ResumeLayout(false);
			this.tsFile.PerformLayout();
			this.tsSelection.ResumeLayout(false);
			this.tsSelection.PerformLayout();
			this.tsMinimap.ResumeLayout(false);
			this.tsMinimap.PerformLayout();
			this.menuMain.ResumeLayout(false);
			this.menuMain.PerformLayout();
			this.TableLayoutPanel5.ResumeLayout(false);
			this.TableLayoutPanel5.PerformLayout();
			this.ResumeLayout(false);
			
		}
		public System.Windows.Forms.SplitContainer SplitContainer1;
		public System.Windows.Forms.TabPage tpTextures;
		public System.Windows.Forms.TabPage tpHeight;
		public System.Windows.Forms.TabPage tpAutoTexture;
		public System.Windows.Forms.ListBox lstAutoTexture;
		public System.Windows.Forms.Label Label3;
		public System.Windows.Forms.ComboBox cboTileset;
		public System.Windows.Forms.TextBox txtAutoCliffSlope;
		public System.Windows.Forms.Label Label1;
		public System.Windows.Forms.TextBox txtHeightSetL;
		public System.Windows.Forms.Label Label5;
		public System.Windows.Forms.TextBox txtSmoothRate;
		public System.Windows.Forms.Label Label6;
		public System.Windows.Forms.RadioButton rdoHeightSmooth;
		public System.Windows.Forms.RadioButton rdoHeightSet;
		public System.Windows.Forms.TabPage tpResize;
		public System.Windows.Forms.TextBox txtOffsetY;
		public System.Windows.Forms.Label Label15;
		public System.Windows.Forms.TextBox txtOffsetX;
		public System.Windows.Forms.Label Label14;
		public System.Windows.Forms.TextBox txtSizeY;
		public System.Windows.Forms.Label Label13;
		public System.Windows.Forms.TextBox txtSizeX;
		public System.Windows.Forms.Label Label12;
		public System.Windows.Forms.Button btnResize;
		public System.Windows.Forms.Label Label16;
		public System.Windows.Forms.RadioButton rdoAutoCliffRemove;
		public System.Windows.Forms.RadioButton rdoAutoCliffBrush;
		public System.Windows.Forms.ToolStripMenuItem HeightmapBMPToolStripMenuItem;
		public System.Windows.Forms.RadioButton rdoAutoTextureFill;
		public System.Windows.Forms.TextBox txtHeightChangeRate;
		public System.Windows.Forms.Label Label18;
		public System.Windows.Forms.RadioButton rdoHeightChange;
		public System.Windows.Forms.Label Label10;
		public System.Windows.Forms.TextBox txtHeightOffset;
		public System.Windows.Forms.Label Label9;
		public System.Windows.Forms.TextBox txtHeightMultiply;
		public System.Windows.Forms.RadioButton rdoAutoTexturePlace;
		public System.Windows.Forms.RadioButton rdoAutoRoadPlace;
		public System.Windows.Forms.ListBox lstAutoRoad;
		public System.Windows.Forms.Button btnAutoRoadRemove;
		public System.Windows.Forms.Button btnAutoTextureRemove;
		public System.Windows.Forms.MenuStrip menuMain;
		public System.Windows.Forms.ToolStripMenuItem menuFile;
		public System.Windows.Forms.ToolStripMenuItem NewMapToolStripMenuItem;
		public System.Windows.Forms.ToolStripMenuItem OpenToolStripMenuItem;
		public System.Windows.Forms.ToolStripMenuItem SaveToolStripMenuItem;
		public System.Windows.Forms.ToolStripMenuItem MapLNDToolStripMenuItem;
		public System.Windows.Forms.ToolStripMenuItem menuSaveFMap;
		public System.Windows.Forms.ToolStripSeparator ToolStripMenuItem1;
		public System.Windows.Forms.ToolStripMenuItem MinimapBMPToolStripMenuItem;
		public System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem3;
		public System.Windows.Forms.ToolStripSeparator ToolStripMenuItem2;
		public System.Windows.Forms.ToolStripMenuItem CloseToolStripMenuItem;
		public System.Windows.Forms.TabPage tpObjects;
		public System.Windows.Forms.Label Label22;
		public System.Windows.Forms.TabPage tpObject;
		public System.Windows.Forms.Label lblObjectType;
		public System.Windows.Forms.Label Label24;
		public System.Windows.Forms.TextBox txtObjectRotation;
		public System.Windows.Forms.Label Label23;
		public System.Windows.Forms.Label Label28;
		public System.Windows.Forms.RadioButton rdoAutoRoadLine;
		public System.Windows.Forms.Label Label29;
		public System.Windows.Forms.TableLayoutPanel TableLayoutPanel1;
		public System.Windows.Forms.ToolStripSeparator ToolStripSeparator3;
		public System.Windows.Forms.ToolStripSeparator ToolStripSeparator2;
		public System.Windows.Forms.ToolStripSeparator ToolStripSeparator1;
		public System.Windows.Forms.ToolStripMenuItem MapWZToolStripMenuItem;
		public System.Windows.Forms.ToolStripSeparator ToolStripSeparator4;
		public System.Windows.Forms.ToolStripMenuItem menuOptions;
		public System.Windows.Forms.Button btnSelResize;
		public System.Windows.Forms.ToolStrip tsSelection;
		public System.Windows.Forms.ToolStripLabel ToolStripLabel1;
		public System.Windows.Forms.ToolStripButton tsbSelection;
		public System.Windows.Forms.ToolStripButton tsbSelectionCopy;
		public System.Windows.Forms.ToolStripDropDownButton tsbSelectionPasteOptions;
		public System.Windows.Forms.ToolStripMenuItem menuSelPasteHeights;
		public System.Windows.Forms.ToolStripMenuItem menuSelPasteTextures;
		public System.Windows.Forms.ToolStripMenuItem menuSelPasteUnits;
		public System.Windows.Forms.ToolStripMenuItem menuSelPasteDeleteUnits;
		public System.Windows.Forms.ToolStripButton tsbSelectionPaste;
		public System.Windows.Forms.ToolStripButton tsbSelectionRotateCounterClockwise;
		public System.Windows.Forms.ToolStripButton tsbSelectionRotateClockwise;
		public System.Windows.Forms.ToolStrip tsMinimap;
		public System.Windows.Forms.ToolStripDropDownButton menuMinimap;
		public System.Windows.Forms.ToolStripMenuItem menuMiniShowTex;
		public System.Windows.Forms.ToolStripMenuItem menuMiniShowHeight;
		public System.Windows.Forms.ToolStripMenuItem menuMiniShowUnits;
		public System.Windows.Forms.TableLayoutPanel TableLayoutPanel5;
		public System.Windows.Forms.ToolStripMenuItem menuMiniShowGateways;
		public System.Windows.Forms.ToolStrip tsTools;
		public System.Windows.Forms.ToolStripButton tsbGateways;
		public System.Windows.Forms.TableLayoutPanel TableLayoutPanel6;
		public System.Windows.Forms.Panel Panel5;
		public System.Windows.Forms.Panel Panel6;
		public System.Windows.Forms.Label Label20;
		public System.Windows.Forms.ComboBox cboTileType;
		public System.Windows.Forms.CheckBox cbxTileTypes;
		public System.Windows.Forms.ToolStripSeparator ToolStripSeparator6;
		public System.Windows.Forms.ToolStripMenuItem menuExportMapTileTypes;
		public System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem4;
		public System.Windows.Forms.ToolStripMenuItem ImportHeightmapToolStripMenuItem;
		public System.Windows.Forms.ToolStripMenuItem menuImportTileTypes;
		public System.Windows.Forms.CheckBox cbxCliffTris;
		public System.Windows.Forms.ToolStrip tsFile;
		public System.Windows.Forms.ToolStripButton tsbSave;
		public System.Windows.Forms.Label Label21;
		public System.Windows.Forms.Panel Panel1;
		public System.Windows.Forms.CheckBox cbxAutoTexSetHeight;
		public System.Windows.Forms.TextBox txtObjectID;
		public System.Windows.Forms.Label Label25;
		public System.Windows.Forms.TableLayoutPanel TableLayoutPanel7;
		public System.Windows.Forms.Panel Panel7;
		public System.Windows.Forms.Panel pnlView;
		public System.Windows.Forms.Label Label26;
		public System.Windows.Forms.TextBox txtHeightSetR;
		public System.Windows.Forms.Label Label27;
		public System.Windows.Forms.TabControl tabHeightSetL;
		public System.Windows.Forms.TabPage TabPage9;
		public System.Windows.Forms.TabPage TabPage10;
		public System.Windows.Forms.TabPage TabPage11;
		public System.Windows.Forms.TabPage TabPage12;
		public System.Windows.Forms.TabControl tabHeightSetR;
		public System.Windows.Forms.TabPage TabPage25;
		public System.Windows.Forms.TabPage TabPage26;
		public System.Windows.Forms.TabPage TabPage27;
		public System.Windows.Forms.TabPage TabPage28;
		public System.Windows.Forms.TabPage TabPage29;
		public System.Windows.Forms.TabPage TabPage30;
		public System.Windows.Forms.TabPage TabPage31;
		public System.Windows.Forms.TabPage TabPage32;
		public System.Windows.Forms.TabPage TabPage17;
		public System.Windows.Forms.TabPage TabPage18;
		public System.Windows.Forms.TabPage TabPage19;
		public System.Windows.Forms.TabPage TabPage20;
		public System.Windows.Forms.TabPage TabPage13;
		public System.Windows.Forms.TabPage TabPage14;
		public System.Windows.Forms.TabPage TabPage15;
		public System.Windows.Forms.TabPage TabPage16;
		public System.Windows.Forms.TabPage TabPage21;
		public System.Windows.Forms.TabPage TabPage22;
		public System.Windows.Forms.TabPage TabPage23;
		public System.Windows.Forms.TabPage TabPage24;
		private System.Windows.Forms.TabControl TabControl;
		public System.Windows.Forms.ToolStripButton tsbSelectionObjects;
		public System.Windows.Forms.ToolStripButton tsbDrawAutotexture;
		public System.Windows.Forms.ToolStripButton tsbSelectionFlipX;
		public System.Windows.Forms.Button btnHeightOffsetSelection;
		public System.Windows.Forms.Button btnHeightsMultiplySelection;
		public System.Windows.Forms.ToolStripButton tsbDrawTileOrientation;
		public System.Windows.Forms.CheckBox chkTextureOrientationRandomize;
		public System.Windows.Forms.Button btnTextureFlipX;
		public System.Windows.Forms.Button btnTextureAnticlockwise;
		public System.Windows.Forms.CheckBox chkSetTextureOrientation;
		public System.Windows.Forms.CheckBox chkSetTexture;
		public System.Windows.Forms.CheckBox cbxTileNumbers;
		public System.Windows.Forms.Label Label32;
		public System.Windows.Forms.TextBox txtObjectPriority;
		public System.Windows.Forms.Label Label33;
		public System.Windows.Forms.ToolStripMenuItem menuRotateUnits;
		public System.Windows.Forms.ToolStripSeparator ToolStripSeparator10;
		public System.Windows.Forms.ToolStripMenuItem menuSelPasteGateways;
		public System.Windows.Forms.ToolStripMenuItem menuSelPasteDeleteGateways;
		public System.Windows.Forms.ToolStripMenuItem menuMiniShowCliffs;
		public System.Windows.Forms.CheckBox cbxInvalidTiles;
		public System.Windows.Forms.ToolStripMenuItem menuRotateWalls;
		public System.Windows.Forms.ToolStripMenuItem menuRotateNothing;
		public System.Windows.Forms.TextBox txtObjectHealth;
		public System.Windows.Forms.Label Label34;
		public System.Windows.Forms.Label Label39;
		public System.Windows.Forms.ComboBox cboDroidTurret1;
		public System.Windows.Forms.Label Label38;
		public System.Windows.Forms.ComboBox cboDroidPropulsion;
		public System.Windows.Forms.Label Label37;
		public System.Windows.Forms.ComboBox cboDroidBody;
		public System.Windows.Forms.Button btnDroidToDesign;
		public System.Windows.Forms.Label Label36;
		public System.Windows.Forms.ComboBox cboDroidTurret3;
		public System.Windows.Forms.ComboBox cboDroidTurret2;
		public System.Windows.Forms.RadioButton rdoDroidTurret3;
		public System.Windows.Forms.RadioButton rdoDroidTurret2;
		public System.Windows.Forms.RadioButton rdoDroidTurret1;
		public System.Windows.Forms.Label Label40;
		public System.Windows.Forms.ComboBox cboDroidType;
		public System.Windows.Forms.RadioButton rdoDroidTurret0;
		public System.Windows.Forms.TableLayoutPanel TableLayoutPanel8;
		public System.Windows.Forms.TableLayoutPanel TableLayoutPanel9;
		public System.Windows.Forms.Panel Panel13;
		public System.Windows.Forms.Panel Panel12;
		public System.Windows.Forms.Panel Panel11;
		public System.Windows.Forms.Panel Panel10;
		public System.Windows.Forms.Panel Panel9;
		public System.Windows.Forms.Panel Panel8;
		public System.Windows.Forms.Panel Panel14;
		public System.Windows.Forms.Label Label35;
		public System.Windows.Forms.Panel pnlTextureBrush;
		public System.Windows.Forms.Panel pnlCliffRemoveBrush;
		public System.Windows.Forms.Panel pnlTerrainBrush;
		public System.Windows.Forms.Panel pnlHeightSetBrush;
		public System.Windows.Forms.RadioButton rdoRoadRemove;
		public System.Windows.Forms.RadioButton rdoTextureRemoveTerrain;
		public System.Windows.Forms.RadioButton rdoTextureReinterpretTerrain;
		public System.Windows.Forms.RadioButton rdoTextureIgnoreTerrain;
		public System.Windows.Forms.CheckBox cbxHeightChangeFade;
		public System.Windows.Forms.Button btnPlayerSelectObjects;
		public System.Windows.Forms.CheckBox cbxObjectRandomRotation;
		public System.Windows.Forms.ToolStripSeparator ToolStripSeparator11;
		public System.Windows.Forms.RadioButton rdoCliffTriBrush;
		public System.Windows.Forms.Button btnTextureClockwise;
		public System.Windows.Forms.RadioButton rdoFillCliffIgnore;
		public System.Windows.Forms.RadioButton rdoFillCliffStopAfter;
		public System.Windows.Forms.RadioButton rdoFillCliffStopBefore;
		internal System.Windows.Forms.Panel Panel15;
		internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator8;
		internal System.Windows.Forms.TabPage tpLabels;
		internal System.Windows.Forms.GroupBox GroupBox1;
		public System.Windows.Forms.Label Label2;
		public System.Windows.Forms.TextBox txtScriptMarkerY;
		public System.Windows.Forms.TextBox txtScriptMarkerX;
		public System.Windows.Forms.Label Label7;
		public System.Windows.Forms.TextBox txtScriptMarkerY2;
		public System.Windows.Forms.Label Label4;
		public System.Windows.Forms.Label Label8;
		public System.Windows.Forms.TextBox txtScriptMarkerX2;
		public System.Windows.Forms.CheckBox cbxFillInside;
		public System.Windows.Forms.Button btnScriptAreaCreate;
		internal System.Windows.Forms.ListBox lstScriptAreas;
		internal System.Windows.Forms.ListBox lstScriptPositions;
		public System.Windows.Forms.Button btnScriptMarkerRemove;
		public System.Windows.Forms.TextBox txtScriptMarkerLabel;
		public System.Windows.Forms.Label Label41;
		public System.Windows.Forms.Label Label43;
		public System.Windows.Forms.Label Label42;
		public System.Windows.Forms.Label Label11;
		public System.Windows.Forms.TextBox txtObjectLabel;
		public System.Windows.Forms.Label Label17;
		public System.Windows.Forms.Button btnAlignObjects;
		public System.Windows.Forms.Button btnFlatSelected;
		internal System.Windows.Forms.TabControl TabControl1;
		internal System.Windows.Forms.TabPage TabPage1;
		internal System.Windows.Forms.TabPage TabPage2;
		internal System.Windows.Forms.TabPage TabPage3;
		public System.Windows.Forms.CheckBox cbxAutoWalls;
		public System.Windows.Forms.TextBox txtNewObjectRotation;
		public System.Windows.Forms.Label Label19;
		internal System.Windows.Forms.CheckBox cbxDesignableOnly;
		public System.Windows.Forms.Label Label31;
		public System.Windows.Forms.Label Label30;
		public System.Windows.Forms.CheckBox cbxFootprintRotate;
		internal System.Windows.Forms.TextBox txtObjectFind;
		internal System.Windows.Forms.ToolStripMenuItem menuSaveFMapQuick;
		internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator7;
		public System.Windows.Forms.RadioButton rdoObjectPlace;
		public System.Windows.Forms.RadioButton rdoObjectLines;
		internal System.Windows.Forms.GroupBox GroupBox3;
		internal System.Windows.Forms.DataGridView dgvFeatures;
		internal System.Windows.Forms.DataGridView dgvStructures;
		internal System.Windows.Forms.DataGridView dgvDroids;
		public System.Windows.Forms.Label Label44;
		internal System.Windows.Forms.ToolStripMenuItem menuTools;
		internal System.Windows.Forms.ToolStripMenuItem menuReinterpret;
		internal System.Windows.Forms.ToolStripMenuItem menuWaterCorrection;
		internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator9;
		internal System.Windows.Forms.ToolStripMenuItem menuGenerator;
		internal System.Windows.Forms.ToolStripMenuItem menuFlatOil;
		internal System.Windows.Forms.ToolStripMenuItem menuFlatStructures;
		internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator12;
		internal System.Windows.Forms.Panel Panel2;
		public System.Windows.Forms.Button btnObjectTypeSelect;
	}
	
}
