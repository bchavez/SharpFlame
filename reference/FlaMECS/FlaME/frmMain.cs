namespace FlaME
{
    using FlaME.My;
    using Matrix3D;
    using Microsoft.VisualBasic;
    using Microsoft.VisualBasic.CompilerServices;
    using OpenTK.Graphics.OpenGL;
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Data;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Windows.Forms;

    [DesignerGenerated]
    public class frmMain : Form
    {
        [AccessedThroughProperty("btnAlignObjects")]
        private Button _btnAlignObjects;
        [AccessedThroughProperty("btnAutoRoadRemove")]
        private Button _btnAutoRoadRemove;
        [AccessedThroughProperty("btnAutoTextureRemove")]
        private Button _btnAutoTextureRemove;
        [AccessedThroughProperty("btnDroidToDesign")]
        private Button _btnDroidToDesign;
        [AccessedThroughProperty("btnFlatSelected")]
        private Button _btnFlatSelected;
        [AccessedThroughProperty("btnHeightOffsetSelection")]
        private Button _btnHeightOffsetSelection;
        [AccessedThroughProperty("btnHeightsMultiplySelection")]
        private Button _btnHeightsMultiplySelection;
        [AccessedThroughProperty("btnObjectTypeSelect")]
        private Button _btnObjectTypeSelect;
        [AccessedThroughProperty("btnPlayerSelectObjects")]
        private Button _btnPlayerSelectObjects;
        [AccessedThroughProperty("btnResize")]
        private Button _btnResize;
        [AccessedThroughProperty("btnScriptAreaCreate")]
        private Button _btnScriptAreaCreate;
        [AccessedThroughProperty("btnScriptMarkerRemove")]
        private Button _btnScriptMarkerRemove;
        [AccessedThroughProperty("btnSelResize")]
        private Button _btnSelResize;
        [AccessedThroughProperty("btnTextureAnticlockwise")]
        private Button _btnTextureAnticlockwise;
        [AccessedThroughProperty("btnTextureClockwise")]
        private Button _btnTextureClockwise;
        [AccessedThroughProperty("btnTextureFlipX")]
        private Button _btnTextureFlipX;
        [AccessedThroughProperty("cboDroidBody")]
        private ComboBox _cboDroidBody;
        [AccessedThroughProperty("cboDroidPropulsion")]
        private ComboBox _cboDroidPropulsion;
        [AccessedThroughProperty("cboDroidTurret1")]
        private ComboBox _cboDroidTurret1;
        [AccessedThroughProperty("cboDroidTurret2")]
        private ComboBox _cboDroidTurret2;
        [AccessedThroughProperty("cboDroidTurret3")]
        private ComboBox _cboDroidTurret3;
        [AccessedThroughProperty("cboDroidType")]
        private ComboBox _cboDroidType;
        [AccessedThroughProperty("cboTileset")]
        private ComboBox _cboTileset;
        [AccessedThroughProperty("cboTileType")]
        private ComboBox _cboTileType;
        [AccessedThroughProperty("cbxAutoTexSetHeight")]
        private CheckBox _cbxAutoTexSetHeight;
        [AccessedThroughProperty("cbxAutoWalls")]
        private CheckBox _cbxAutoWalls;
        [AccessedThroughProperty("cbxCliffTris")]
        private CheckBox _cbxCliffTris;
        [AccessedThroughProperty("cbxDesignableOnly")]
        private CheckBox _cbxDesignableOnly;
        [AccessedThroughProperty("cbxFillInside")]
        private CheckBox _cbxFillInside;
        [AccessedThroughProperty("cbxFootprintRotate")]
        private CheckBox _cbxFootprintRotate;
        [AccessedThroughProperty("cbxHeightChangeFade")]
        private CheckBox _cbxHeightChangeFade;
        [AccessedThroughProperty("cbxInvalidTiles")]
        private CheckBox _cbxInvalidTiles;
        [AccessedThroughProperty("cbxObjectRandomRotation")]
        private CheckBox _cbxObjectRandomRotation;
        [AccessedThroughProperty("cbxTileNumbers")]
        private CheckBox _cbxTileNumbers;
        [AccessedThroughProperty("cbxTileTypes")]
        private CheckBox _cbxTileTypes;
        [AccessedThroughProperty("chkSetTexture")]
        private CheckBox _chkSetTexture;
        [AccessedThroughProperty("chkSetTextureOrientation")]
        private CheckBox _chkSetTextureOrientation;
        [AccessedThroughProperty("chkTextureOrientationRandomize")]
        private CheckBox _chkTextureOrientationRandomize;
        [AccessedThroughProperty("CloseToolStripMenuItem")]
        private ToolStripMenuItem _CloseToolStripMenuItem;
        [AccessedThroughProperty("dgvDroids")]
        private DataGridView _dgvDroids;
        [AccessedThroughProperty("dgvFeatures")]
        private DataGridView _dgvFeatures;
        [AccessedThroughProperty("dgvStructures")]
        private DataGridView _dgvStructures;
        [AccessedThroughProperty("GroupBox1")]
        private GroupBox _GroupBox1;
        [AccessedThroughProperty("GroupBox3")]
        private GroupBox _GroupBox3;
        [AccessedThroughProperty("HeightmapBMPToolStripMenuItem")]
        private ToolStripMenuItem _HeightmapBMPToolStripMenuItem;
        [AccessedThroughProperty("ImportHeightmapToolStripMenuItem")]
        private ToolStripMenuItem _ImportHeightmapToolStripMenuItem;
        [AccessedThroughProperty("Label1")]
        private Label _Label1;
        [AccessedThroughProperty("Label10")]
        private Label _Label10;
        [AccessedThroughProperty("Label11")]
        private Label _Label11;
        [AccessedThroughProperty("Label12")]
        private Label _Label12;
        [AccessedThroughProperty("Label13")]
        private Label _Label13;
        [AccessedThroughProperty("Label14")]
        private Label _Label14;
        [AccessedThroughProperty("Label15")]
        private Label _Label15;
        [AccessedThroughProperty("Label16")]
        private Label _Label16;
        [AccessedThroughProperty("Label17")]
        private Label _Label17;
        [AccessedThroughProperty("Label18")]
        private Label _Label18;
        [AccessedThroughProperty("Label19")]
        private Label _Label19;
        [AccessedThroughProperty("Label2")]
        private Label _Label2;
        [AccessedThroughProperty("Label20")]
        private Label _Label20;
        [AccessedThroughProperty("Label21")]
        private Label _Label21;
        [AccessedThroughProperty("Label22")]
        private Label _Label22;
        [AccessedThroughProperty("Label23")]
        private Label _Label23;
        [AccessedThroughProperty("Label24")]
        private Label _Label24;
        [AccessedThroughProperty("Label25")]
        private Label _Label25;
        [AccessedThroughProperty("Label26")]
        private Label _Label26;
        [AccessedThroughProperty("Label27")]
        private Label _Label27;
        [AccessedThroughProperty("Label28")]
        private Label _Label28;
        [AccessedThroughProperty("Label29")]
        private Label _Label29;
        [AccessedThroughProperty("Label3")]
        private Label _Label3;
        [AccessedThroughProperty("Label30")]
        private Label _Label30;
        [AccessedThroughProperty("Label31")]
        private Label _Label31;
        [AccessedThroughProperty("Label32")]
        private Label _Label32;
        [AccessedThroughProperty("Label33")]
        private Label _Label33;
        [AccessedThroughProperty("Label34")]
        private Label _Label34;
        [AccessedThroughProperty("Label35")]
        private Label _Label35;
        [AccessedThroughProperty("Label36")]
        private Label _Label36;
        [AccessedThroughProperty("Label37")]
        private Label _Label37;
        [AccessedThroughProperty("Label38")]
        private Label _Label38;
        [AccessedThroughProperty("Label39")]
        private Label _Label39;
        [AccessedThroughProperty("Label4")]
        private Label _Label4;
        [AccessedThroughProperty("Label40")]
        private Label _Label40;
        [AccessedThroughProperty("Label41")]
        private Label _Label41;
        [AccessedThroughProperty("Label42")]
        private Label _Label42;
        [AccessedThroughProperty("Label43")]
        private Label _Label43;
        [AccessedThroughProperty("Label44")]
        private Label _Label44;
        [AccessedThroughProperty("Label5")]
        private Label _Label5;
        [AccessedThroughProperty("Label6")]
        private Label _Label6;
        [AccessedThroughProperty("Label7")]
        private Label _Label7;
        [AccessedThroughProperty("Label8")]
        private Label _Label8;
        [AccessedThroughProperty("Label9")]
        private Label _Label9;
        [AccessedThroughProperty("lblObjectType")]
        private Label _lblObjectType;
        private clsMaps _LoadedMaps;
        [AccessedThroughProperty("lstAutoRoad")]
        private ListBox _lstAutoRoad;
        [AccessedThroughProperty("lstAutoTexture")]
        private ListBox _lstAutoTexture;
        [AccessedThroughProperty("lstScriptAreas")]
        private ListBox _lstScriptAreas;
        [AccessedThroughProperty("lstScriptPositions")]
        private ListBox _lstScriptPositions;
        [AccessedThroughProperty("MapLNDToolStripMenuItem")]
        private ToolStripMenuItem _MapLNDToolStripMenuItem;
        [AccessedThroughProperty("MapWZToolStripMenuItem")]
        private ToolStripMenuItem _MapWZToolStripMenuItem;
        [AccessedThroughProperty("menuExportMapTileTypes")]
        private ToolStripMenuItem _menuExportMapTileTypes;
        [AccessedThroughProperty("menuFile")]
        private ToolStripMenuItem _menuFile;
        [AccessedThroughProperty("menuFlatOil")]
        private ToolStripMenuItem _menuFlatOil;
        [AccessedThroughProperty("menuFlatStructures")]
        private ToolStripMenuItem _menuFlatStructures;
        [AccessedThroughProperty("menuGenerator")]
        private ToolStripMenuItem _menuGenerator;
        [AccessedThroughProperty("menuImportTileTypes")]
        private ToolStripMenuItem _menuImportTileTypes;
        [AccessedThroughProperty("menuMain")]
        private MenuStrip _menuMain;
        [AccessedThroughProperty("menuMinimap")]
        private ToolStripDropDownButton _menuMinimap;
        [AccessedThroughProperty("menuMiniShowCliffs")]
        private ToolStripMenuItem _menuMiniShowCliffs;
        [AccessedThroughProperty("menuMiniShowGateways")]
        private ToolStripMenuItem _menuMiniShowGateways;
        [AccessedThroughProperty("menuMiniShowHeight")]
        private ToolStripMenuItem _menuMiniShowHeight;
        [AccessedThroughProperty("menuMiniShowTex")]
        private ToolStripMenuItem _menuMiniShowTex;
        [AccessedThroughProperty("menuMiniShowUnits")]
        private ToolStripMenuItem _menuMiniShowUnits;
        [AccessedThroughProperty("menuOptions")]
        private ToolStripMenuItem _menuOptions;
        [AccessedThroughProperty("menuReinterpret")]
        private ToolStripMenuItem _menuReinterpret;
        [AccessedThroughProperty("menuRotateNothing")]
        private ToolStripMenuItem _menuRotateNothing;
        [AccessedThroughProperty("menuRotateUnits")]
        private ToolStripMenuItem _menuRotateUnits;
        [AccessedThroughProperty("menuRotateWalls")]
        private ToolStripMenuItem _menuRotateWalls;
        [AccessedThroughProperty("menuSaveFMap")]
        private ToolStripMenuItem _menuSaveFMap;
        [AccessedThroughProperty("menuSaveFMapQuick")]
        private ToolStripMenuItem _menuSaveFMapQuick;
        [AccessedThroughProperty("menuSaveFME")]
        private ToolStripMenuItem _menuSaveFME;
        [AccessedThroughProperty("menuSelPasteDeleteGateways")]
        private ToolStripMenuItem _menuSelPasteDeleteGateways;
        [AccessedThroughProperty("menuSelPasteDeleteUnits")]
        private ToolStripMenuItem _menuSelPasteDeleteUnits;
        [AccessedThroughProperty("menuSelPasteGateways")]
        private ToolStripMenuItem _menuSelPasteGateways;
        [AccessedThroughProperty("menuSelPasteHeights")]
        private ToolStripMenuItem _menuSelPasteHeights;
        [AccessedThroughProperty("menuSelPasteTextures")]
        private ToolStripMenuItem _menuSelPasteTextures;
        [AccessedThroughProperty("menuSelPasteUnits")]
        private ToolStripMenuItem _menuSelPasteUnits;
        [AccessedThroughProperty("menuTools")]
        private ToolStripMenuItem _menuTools;
        [AccessedThroughProperty("menuWaterCorrection")]
        private ToolStripMenuItem _menuWaterCorrection;
        [AccessedThroughProperty("MinimapBMPToolStripMenuItem")]
        private ToolStripMenuItem _MinimapBMPToolStripMenuItem;
        [AccessedThroughProperty("NewMapToolStripMenuItem")]
        private ToolStripMenuItem _NewMapToolStripMenuItem;
        [AccessedThroughProperty("OpenToolStripMenuItem")]
        private ToolStripMenuItem _OpenToolStripMenuItem;
        [AccessedThroughProperty("Panel1")]
        private Panel _Panel1;
        [AccessedThroughProperty("Panel10")]
        private Panel _Panel10;
        [AccessedThroughProperty("Panel11")]
        private Panel _Panel11;
        [AccessedThroughProperty("Panel12")]
        private Panel _Panel12;
        [AccessedThroughProperty("Panel13")]
        private Panel _Panel13;
        [AccessedThroughProperty("Panel14")]
        private Panel _Panel14;
        [AccessedThroughProperty("Panel15")]
        private Panel _Panel15;
        [AccessedThroughProperty("Panel2")]
        private Panel _Panel2;
        [AccessedThroughProperty("Panel5")]
        private Panel _Panel5;
        [AccessedThroughProperty("Panel6")]
        private Panel _Panel6;
        [AccessedThroughProperty("Panel7")]
        private Panel _Panel7;
        [AccessedThroughProperty("Panel8")]
        private Panel _Panel8;
        [AccessedThroughProperty("Panel9")]
        private Panel _Panel9;
        [AccessedThroughProperty("pnlCliffRemoveBrush")]
        private Panel _pnlCliffRemoveBrush;
        [AccessedThroughProperty("pnlHeightSetBrush")]
        private Panel _pnlHeightSetBrush;
        [AccessedThroughProperty("pnlTerrainBrush")]
        private Panel _pnlTerrainBrush;
        [AccessedThroughProperty("pnlTextureBrush")]
        private Panel _pnlTextureBrush;
        [AccessedThroughProperty("pnlView")]
        private Panel _pnlView;
        [AccessedThroughProperty("rdoAutoCliffBrush")]
        private RadioButton _rdoAutoCliffBrush;
        [AccessedThroughProperty("rdoAutoCliffRemove")]
        private RadioButton _rdoAutoCliffRemove;
        [AccessedThroughProperty("rdoAutoRoadLine")]
        private RadioButton _rdoAutoRoadLine;
        [AccessedThroughProperty("rdoAutoRoadPlace")]
        private RadioButton _rdoAutoRoadPlace;
        [AccessedThroughProperty("rdoAutoTextureFill")]
        private RadioButton _rdoAutoTextureFill;
        [AccessedThroughProperty("rdoAutoTexturePlace")]
        private RadioButton _rdoAutoTexturePlace;
        [AccessedThroughProperty("rdoCliffTriBrush")]
        private RadioButton _rdoCliffTriBrush;
        [AccessedThroughProperty("rdoDroidTurret0")]
        private RadioButton _rdoDroidTurret0;
        [AccessedThroughProperty("rdoDroidTurret1")]
        private RadioButton _rdoDroidTurret1;
        [AccessedThroughProperty("rdoDroidTurret2")]
        private RadioButton _rdoDroidTurret2;
        [AccessedThroughProperty("rdoDroidTurret3")]
        private RadioButton _rdoDroidTurret3;
        [AccessedThroughProperty("rdoFillCliffIgnore")]
        private RadioButton _rdoFillCliffIgnore;
        [AccessedThroughProperty("rdoFillCliffStopAfter")]
        private RadioButton _rdoFillCliffStopAfter;
        [AccessedThroughProperty("rdoFillCliffStopBefore")]
        private RadioButton _rdoFillCliffStopBefore;
        [AccessedThroughProperty("rdoHeightChange")]
        private RadioButton _rdoHeightChange;
        [AccessedThroughProperty("rdoHeightSet")]
        private RadioButton _rdoHeightSet;
        [AccessedThroughProperty("rdoHeightSmooth")]
        private RadioButton _rdoHeightSmooth;
        [AccessedThroughProperty("rdoObjectLines")]
        private RadioButton _rdoObjectLines;
        [AccessedThroughProperty("rdoObjectPlace")]
        private RadioButton _rdoObjectPlace;
        [AccessedThroughProperty("rdoRoadRemove")]
        private RadioButton _rdoRoadRemove;
        [AccessedThroughProperty("rdoTextureIgnoreTerrain")]
        private RadioButton _rdoTextureIgnoreTerrain;
        [AccessedThroughProperty("rdoTextureReinterpretTerrain")]
        private RadioButton _rdoTextureReinterpretTerrain;
        [AccessedThroughProperty("rdoTextureRemoveTerrain")]
        private RadioButton _rdoTextureRemoveTerrain;
        [AccessedThroughProperty("SaveToolStripMenuItem")]
        private ToolStripMenuItem _SaveToolStripMenuItem;
        private object _SelectedScriptMarker;
        [AccessedThroughProperty("SplitContainer1")]
        private SplitContainer _SplitContainer1;
        [AccessedThroughProperty("TabControl")]
        private System.Windows.Forms.TabControl _TabControl;
        [AccessedThroughProperty("TabControl1")]
        private System.Windows.Forms.TabControl _TabControl1;
        [AccessedThroughProperty("tabHeightSetL")]
        private System.Windows.Forms.TabControl _tabHeightSetL;
        [AccessedThroughProperty("tabHeightSetR")]
        private System.Windows.Forms.TabControl _tabHeightSetR;
        [AccessedThroughProperty("TableLayoutPanel1")]
        private TableLayoutPanel _TableLayoutPanel1;
        [AccessedThroughProperty("TableLayoutPanel5")]
        private TableLayoutPanel _TableLayoutPanel5;
        [AccessedThroughProperty("TableLayoutPanel6")]
        private TableLayoutPanel _TableLayoutPanel6;
        [AccessedThroughProperty("TableLayoutPanel7")]
        private TableLayoutPanel _TableLayoutPanel7;
        [AccessedThroughProperty("TableLayoutPanel8")]
        private TableLayoutPanel _TableLayoutPanel8;
        [AccessedThroughProperty("TableLayoutPanel9")]
        private TableLayoutPanel _TableLayoutPanel9;
        [AccessedThroughProperty("TabPage1")]
        private TabPage _TabPage1;
        [AccessedThroughProperty("TabPage10")]
        private TabPage _TabPage10;
        [AccessedThroughProperty("TabPage11")]
        private TabPage _TabPage11;
        [AccessedThroughProperty("TabPage12")]
        private TabPage _TabPage12;
        [AccessedThroughProperty("TabPage13")]
        private TabPage _TabPage13;
        [AccessedThroughProperty("TabPage14")]
        private TabPage _TabPage14;
        [AccessedThroughProperty("TabPage15")]
        private TabPage _TabPage15;
        [AccessedThroughProperty("TabPage16")]
        private TabPage _TabPage16;
        [AccessedThroughProperty("TabPage17")]
        private TabPage _TabPage17;
        [AccessedThroughProperty("TabPage18")]
        private TabPage _TabPage18;
        [AccessedThroughProperty("TabPage19")]
        private TabPage _TabPage19;
        [AccessedThroughProperty("TabPage2")]
        private TabPage _TabPage2;
        [AccessedThroughProperty("TabPage20")]
        private TabPage _TabPage20;
        [AccessedThroughProperty("TabPage21")]
        private TabPage _TabPage21;
        [AccessedThroughProperty("TabPage22")]
        private TabPage _TabPage22;
        [AccessedThroughProperty("TabPage23")]
        private TabPage _TabPage23;
        [AccessedThroughProperty("TabPage24")]
        private TabPage _TabPage24;
        [AccessedThroughProperty("TabPage25")]
        private TabPage _TabPage25;
        [AccessedThroughProperty("TabPage26")]
        private TabPage _TabPage26;
        [AccessedThroughProperty("TabPage27")]
        private TabPage _TabPage27;
        [AccessedThroughProperty("TabPage28")]
        private TabPage _TabPage28;
        [AccessedThroughProperty("TabPage29")]
        private TabPage _TabPage29;
        [AccessedThroughProperty("TabPage3")]
        private TabPage _TabPage3;
        [AccessedThroughProperty("TabPage30")]
        private TabPage _TabPage30;
        [AccessedThroughProperty("TabPage31")]
        private TabPage _TabPage31;
        [AccessedThroughProperty("TabPage32")]
        private TabPage _TabPage32;
        [AccessedThroughProperty("TabPage9")]
        private TabPage _TabPage9;
        [AccessedThroughProperty("tmrKey")]
        private System.Windows.Forms.Timer _tmrKey;
        [AccessedThroughProperty("tmrTool")]
        private System.Windows.Forms.Timer _tmrTool;
        [AccessedThroughProperty("ToolStripLabel1")]
        private ToolStripLabel _ToolStripLabel1;
        [AccessedThroughProperty("ToolStripMenuItem1")]
        private ToolStripSeparator _ToolStripMenuItem1;
        [AccessedThroughProperty("ToolStripMenuItem2")]
        private ToolStripSeparator _ToolStripMenuItem2;
        [AccessedThroughProperty("ToolStripMenuItem3")]
        private ToolStripMenuItem _ToolStripMenuItem3;
        [AccessedThroughProperty("ToolStripMenuItem4")]
        private ToolStripMenuItem _ToolStripMenuItem4;
        [AccessedThroughProperty("ToolStripSeparator1")]
        private ToolStripSeparator _ToolStripSeparator1;
        [AccessedThroughProperty("ToolStripSeparator10")]
        private ToolStripSeparator _ToolStripSeparator10;
        [AccessedThroughProperty("ToolStripSeparator11")]
        private ToolStripSeparator _ToolStripSeparator11;
        [AccessedThroughProperty("ToolStripSeparator12")]
        private ToolStripSeparator _ToolStripSeparator12;
        [AccessedThroughProperty("ToolStripSeparator2")]
        private ToolStripSeparator _ToolStripSeparator2;
        [AccessedThroughProperty("ToolStripSeparator3")]
        private ToolStripSeparator _ToolStripSeparator3;
        [AccessedThroughProperty("ToolStripSeparator4")]
        private ToolStripSeparator _ToolStripSeparator4;
        [AccessedThroughProperty("ToolStripSeparator5")]
        private ToolStripSeparator _ToolStripSeparator5;
        [AccessedThroughProperty("ToolStripSeparator6")]
        private ToolStripSeparator _ToolStripSeparator6;
        [AccessedThroughProperty("ToolStripSeparator7")]
        private ToolStripSeparator _ToolStripSeparator7;
        [AccessedThroughProperty("ToolStripSeparator8")]
        private ToolStripSeparator _ToolStripSeparator8;
        [AccessedThroughProperty("ToolStripSeparator9")]
        private ToolStripSeparator _ToolStripSeparator9;
        [AccessedThroughProperty("tpAutoTexture")]
        private TabPage _tpAutoTexture;
        [AccessedThroughProperty("tpHeight")]
        private TabPage _tpHeight;
        [AccessedThroughProperty("tpLabels")]
        private TabPage _tpLabels;
        [AccessedThroughProperty("tpObject")]
        private TabPage _tpObject;
        [AccessedThroughProperty("tpObjects")]
        private TabPage _tpObjects;
        [AccessedThroughProperty("tpResize")]
        private TabPage _tpResize;
        [AccessedThroughProperty("tpTextures")]
        private TabPage _tpTextures;
        [AccessedThroughProperty("tsbDrawAutotexture")]
        private ToolStripButton _tsbDrawAutotexture;
        [AccessedThroughProperty("tsbDrawTileOrientation")]
        private ToolStripButton _tsbDrawTileOrientation;
        [AccessedThroughProperty("tsbGateways")]
        private ToolStripButton _tsbGateways;
        [AccessedThroughProperty("tsbSave")]
        private ToolStripButton _tsbSave;
        [AccessedThroughProperty("tsbSelection")]
        private ToolStripButton _tsbSelection;
        [AccessedThroughProperty("tsbSelectionCopy")]
        private ToolStripButton _tsbSelectionCopy;
        [AccessedThroughProperty("tsbSelectionFlipX")]
        private ToolStripButton _tsbSelectionFlipX;
        [AccessedThroughProperty("tsbSelectionObjects")]
        private ToolStripButton _tsbSelectionObjects;
        [AccessedThroughProperty("tsbSelectionPaste")]
        private ToolStripButton _tsbSelectionPaste;
        [AccessedThroughProperty("tsbSelectionPasteOptions")]
        private ToolStripDropDownButton _tsbSelectionPasteOptions;
        [AccessedThroughProperty("tsbSelectionRotateClockwise")]
        private ToolStripButton _tsbSelectionRotateClockwise;
        [AccessedThroughProperty("tsbSelectionRotateCounterClockwise")]
        private ToolStripButton _tsbSelectionRotateCounterClockwise;
        [AccessedThroughProperty("tsFile")]
        private ToolStrip _tsFile;
        [AccessedThroughProperty("tsMinimap")]
        private ToolStrip _tsMinimap;
        [AccessedThroughProperty("tsSelection")]
        private ToolStrip _tsSelection;
        [AccessedThroughProperty("tsTools")]
        private ToolStrip _tsTools;
        [AccessedThroughProperty("txtAutoCliffSlope")]
        private TextBox _txtAutoCliffSlope;
        [AccessedThroughProperty("txtHeightChangeRate")]
        private TextBox _txtHeightChangeRate;
        [AccessedThroughProperty("txtHeightMultiply")]
        private TextBox _txtHeightMultiply;
        [AccessedThroughProperty("txtHeightOffset")]
        private TextBox _txtHeightOffset;
        [AccessedThroughProperty("txtHeightSetL")]
        private TextBox _txtHeightSetL;
        [AccessedThroughProperty("txtHeightSetR")]
        private TextBox _txtHeightSetR;
        [AccessedThroughProperty("txtNewObjectRotation")]
        private TextBox _txtNewObjectRotation;
        [AccessedThroughProperty("txtObjectFind")]
        private TextBox _txtObjectFind;
        [AccessedThroughProperty("txtObjectHealth")]
        private TextBox _txtObjectHealth;
        [AccessedThroughProperty("txtObjectID")]
        private TextBox _txtObjectID;
        [AccessedThroughProperty("txtObjectLabel")]
        private TextBox _txtObjectLabel;
        [AccessedThroughProperty("txtObjectPriority")]
        private TextBox _txtObjectPriority;
        [AccessedThroughProperty("txtObjectRotation")]
        private TextBox _txtObjectRotation;
        [AccessedThroughProperty("txtOffsetX")]
        private TextBox _txtOffsetX;
        [AccessedThroughProperty("txtOffsetY")]
        private TextBox _txtOffsetY;
        [AccessedThroughProperty("txtScriptMarkerLabel")]
        private TextBox _txtScriptMarkerLabel;
        [AccessedThroughProperty("txtScriptMarkerX")]
        private TextBox _txtScriptMarkerX;
        [AccessedThroughProperty("txtScriptMarkerX2")]
        private TextBox _txtScriptMarkerX2;
        [AccessedThroughProperty("txtScriptMarkerY")]
        private TextBox _txtScriptMarkerY;
        [AccessedThroughProperty("txtScriptMarkerY2")]
        private TextBox _txtScriptMarkerY2;
        [AccessedThroughProperty("txtSizeX")]
        private TextBox _txtSizeX;
        [AccessedThroughProperty("txtSizeY")]
        private TextBox _txtSizeY;
        [AccessedThroughProperty("txtSmoothRate")]
        private TextBox _txtSmoothRate;
        public clsBody[] cboBody_Objects;
        public clsPropulsion[] cboPropulsion_Objects;
        public clsTurret[] cboTurret_Objects;
        private IContainer components;
        public ctrlBrush ctrlCliffRemoveBrush;
        public ctrlBrush ctrlHeightBrush;
        public ctrlBrush ctrlTerrainBrush;
        public ctrlBrush ctrlTextureBrush;
        public modProgram.enumFillCliffAction FillCliffAction;
        public byte[] HeightSetPalette;
        public string InitializeStatus;
        private clsMap.clsScriptArea[] lstScriptAreas_Objects;
        private clsMap.clsScriptPosition[] lstScriptPositions_Objects;
        public ctrlMapView MapView;
        public ctrlPlayerNum NewPlayerNum;
        public ctrlPlayerNum ObjectPlayerNum;
        public modProgram.enumObjectRotateMode PasteRotateObjects;
        public modLists.ConnectedList<clsUnitType, frmMain> SelectedObjectTypes;
        public modProgram.enumTextureTerrainAction TextureTerrainAction;
        public ctrlTextureView TextureView;

        public frmMain()
        {
            base.Leave += new EventHandler(this.Me_LostFocus);
            base.DragEnter += new DragEventHandler(this.OpenGL_DragEnter);
            base.DragDrop += new DragEventHandler(this.OpenGL_DragDrop);
            base.FormClosing += new FormClosingEventHandler(this.frmMain_FormClosing);
            this._LoadedMaps = new clsMaps(this);
            this.HeightSetPalette = new byte[8];
            this.SelectedObjectTypes = new modLists.ConnectedList<clsUnitType, frmMain>(this);
            this.TextureTerrainAction = modProgram.enumTextureTerrainAction.Reinterpret;
            this.FillCliffAction = modProgram.enumFillCliffAction.Ignore;
            this.InitializeStatus = "";
            this.PasteRotateObjects = modProgram.enumObjectRotateMode.Walls;
            this.lstScriptPositions_Objects = new clsMap.clsScriptPosition[0];
            this.lstScriptAreas_Objects = new clsMap.clsScriptArea[0];
            this.InitializeComponent();
            this.MapView = new ctrlMapView(this);
            this.TextureView = new ctrlTextureView(this);
            modMain.frmGeneratorInstance = new frmGenerator(this);
            this.tmrKey = new System.Windows.Forms.Timer();
            this.tmrKey.Interval = 30;
            this.tmrTool = new System.Windows.Forms.Timer();
            this.tmrTool.Interval = 100;
            this.NewPlayerNum = new ctrlPlayerNum();
            this.ObjectPlayerNum = new ctrlPlayerNum();
        }

        private void ActivateObjectTool()
        {
            if (this.rdoObjectPlace.Checked)
            {
                modTools.Tool = modTools.Tools.ObjectPlace;
            }
            else if (this.rdoObjectLines.Checked)
            {
                modTools.Tool = modTools.Tools.ObjectLines;
            }
        }

        private void btnAlignObjects_Click(object sender, EventArgs e)
        {
            clsMap mainMap = this.MainMap;
            if (mainMap != null)
            {
                clsMap.clsObjectAlignment tool = new clsMap.clsObjectAlignment {
                    Map = mainMap
                };
                mainMap.SelectedUnits.GetItemsAsSimpleList().PerformTool(tool);
                mainMap.Update();
                mainMap.UndoStepCreate("Align Objects");
            }
        }

        private void btnAutoRoadRemove_Click(object sender, EventArgs e)
        {
            this.lstAutoRoad.SelectedIndex = -1;
        }

        private void btnAutoTextureRemove_Click(object sender, EventArgs e)
        {
            this.lstAutoTexture.SelectedIndex = -1;
        }

        private void btnDroidToDesign_Click(object sender, EventArgs e)
        {
            clsMap mainMap = this.MainMap;
            if ((mainMap != null) && (mainMap.SelectedUnits.Count > 0))
            {
                if (mainMap.SelectedUnits.Count > 1)
                {
                    if (Interaction.MsgBox("Change design of multiple droids?", MsgBoxStyle.Question | MsgBoxStyle.OkCancel, "") != MsgBoxResult.Ok)
                    {
                        return;
                    }
                }
                else if (Interaction.MsgBox("Change design of a droid?", MsgBoxStyle.Question | MsgBoxStyle.OkCancel, "") != MsgBoxResult.Ok)
                {
                    return;
                }
                clsMap.clsObjectTemplateToDesign tool = new clsMap.clsObjectTemplateToDesign {
                    Map = mainMap
                };
                mainMap.SelectedUnitsAction(tool);
                this.SelectedObject_Changed();
                if (tool.ActionPerformed)
                {
                    mainMap.UndoStepCreate("Object Template Removed");
                    this.View_DrawViewLater();
                }
            }
        }

        private void btnFlatSelected_Click(object sender, EventArgs e)
        {
            clsMap mainMap = this.MainMap;
            if (mainMap != null)
            {
                clsMap.clsObjectFlattenTerrain tool = new clsMap.clsObjectFlattenTerrain();
                mainMap.SelectedUnits.GetItemsAsSimpleClassList().PerformTool(tool);
                mainMap.Update();
                mainMap.UndoStepCreate("Flatten Under Structures");
            }
        }

        private void btnHeightOffsetSelection_Click(object sender, EventArgs e)
        {
            double num;
            clsMap mainMap = this.MainMap;
            if (((mainMap != null) && !((mainMap.Selected_Area_VertexA == null) | (mainMap.Selected_Area_VertexB == null))) && modIO.InvariantParse_dbl(this.txtHeightOffset.Text, ref num))
            {
                modMath.sXY_int _int;
                modMath.sXY_int _int3;
                modMath.ReorderXY(mainMap.Selected_Area_VertexA.XY, mainMap.Selected_Area_VertexB.XY, ref _int3, ref _int);
                int y = _int.Y;
                for (int i = _int3.Y; i <= y; i++)
                {
                    int x = _int.X;
                    for (int j = _int3.X; j <= x; j++)
                    {
                        modMath.sXY_int _int2;
                        mainMap.Terrain.Vertices[j, i].Height = (byte) Math.Round(Math.Round(modMath.Clamp_dbl(mainMap.Terrain.Vertices[j, i].Height + num, 0.0, 255.0)));
                        _int2.X = j;
                        _int2.Y = i;
                        mainMap.SectorGraphicsChanges.VertexAndNormalsChanged(_int2);
                        mainMap.SectorUnitHeightsChanges.VertexChanged(_int2);
                        mainMap.SectorTerrainUndoChanges.VertexChanged(_int2);
                    }
                }
                mainMap.Update();
                mainMap.UndoStepCreate("Selection Heights Offset");
                this.View_DrawViewLater();
            }
        }

        private void btnHeightsMultiplySelection_Click(object sender, EventArgs e)
        {
            double num;
            clsMap mainMap = this.MainMap;
            if (((mainMap != null) && !((mainMap.Selected_Area_VertexA == null) | (mainMap.Selected_Area_VertexB == null))) && modIO.InvariantParse_dbl(this.txtHeightMultiply.Text, ref num))
            {
                modMath.sXY_int _int;
                modMath.sXY_int _int3;
                double num2 = modMath.Clamp_dbl(num, 0.0, 255.0);
                modMath.ReorderXY(mainMap.Selected_Area_VertexA.XY, mainMap.Selected_Area_VertexB.XY, ref _int3, ref _int);
                int y = _int.Y;
                for (int i = _int3.Y; i <= y; i++)
                {
                    int x = _int.X;
                    for (int j = _int3.X; j <= x; j++)
                    {
                        modMath.sXY_int _int2;
                        mainMap.Terrain.Vertices[j, i].Height = (byte) Math.Round(Math.Round(modMath.Clamp_dbl(mainMap.Terrain.Vertices[j, i].Height * num2, 0.0, 255.0)));
                        _int2.X = j;
                        _int2.Y = i;
                        mainMap.SectorGraphicsChanges.VertexAndNormalsChanged(_int2);
                        mainMap.SectorUnitHeightsChanges.VertexChanged(_int2);
                        mainMap.SectorTerrainUndoChanges.VertexChanged(_int2);
                    }
                }
                mainMap.Update();
                mainMap.UndoStepCreate("Selection Heights Multiply");
                this.View_DrawViewLater();
            }
        }

        private void btnObjectTypeSelect_Click(object sender, EventArgs e)
        {
            clsMap mainMap = this.MainMap;
            if (mainMap != null)
            {
                IEnumerator enumerator;
                if (!modControls.KeyboardProfile.Active(modControls.Control_Unit_Multiselect))
                {
                    mainMap.SelectedUnits.Clear();
                }
                try
                {
                    enumerator = mainMap.Units.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        clsMap.clsUnit current = (clsMap.clsUnit) enumerator.Current;
                        if (current.Type.UnitType_frmMainSelectedLink.IsConnected && !current.MapSelectedUnitLink.IsConnected)
                        {
                            current.MapSelectedUnitLink.Connect(mainMap.SelectedUnits);
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
                this.View_DrawViewLater();
            }
        }

        private void btnPlayerSelectObjects_Click(object sender, EventArgs e)
        {
            clsMap mainMap = this.MainMap;
            if (mainMap != null)
            {
                IEnumerator enumerator;
                if (!modControls.KeyboardProfile.Active(modControls.Control_Unit_Multiselect))
                {
                    mainMap.SelectedUnits.Clear();
                }
                clsMap.clsUnitGroup item = mainMap.SelectedUnitGroup.Item;
                try
                {
                    enumerator = mainMap.Units.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        clsMap.clsUnit current = (clsMap.clsUnit) enumerator.Current;
                        if (((current.UnitGroup == item) && (current.Type.Type != clsUnitType.enumType.Feature)) && !current.MapSelectedUnitLink.IsConnected)
                        {
                            current.MapSelectedUnitLink.Connect(mainMap.SelectedUnits);
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
                this.View_DrawViewLater();
            }
        }

        private void btnResize_Click(object sender, EventArgs e)
        {
            modMath.sXY_int _int;
            modMath.sXY_int _int2;
            if (((modIO.InvariantParse_int(this.txtSizeX.Text, ref _int.X) && modIO.InvariantParse_int(this.txtSizeY.Text, ref _int.Y)) && modIO.InvariantParse_int(this.txtOffsetX.Text, ref _int2.X)) && modIO.InvariantParse_int(this.txtOffsetY.Text, ref _int2.Y))
            {
                this.Map_Resize(_int2, _int);
            }
        }

        private void btnScriptAreaCreate_Click(object sender, EventArgs e)
        {
            if (this.btnScriptAreaCreate.Enabled)
            {
                clsMap mainMap = this.MainMap;
                if (mainMap != null)
                {
                    if (mainMap.Selected_Area_VertexA == null)
                    {
                        Interaction.MsgBox("Select something first.", MsgBoxStyle.ApplicationModal, null);
                    }
                    else if (mainMap.Selected_Area_VertexB == null)
                    {
                        Interaction.MsgBox("Select something first.", MsgBoxStyle.ApplicationModal, null);
                    }
                    else
                    {
                        clsMap.clsScriptArea area = clsMap.clsScriptArea.Create(mainMap);
                        if (area == null)
                        {
                            Interaction.MsgBox("Error: Creating area failed.", MsgBoxStyle.ApplicationModal, null);
                        }
                        else
                        {
                            modMath.sXY_int posA = new modMath.sXY_int(mainMap.Selected_Area_VertexA.X * 0x80, mainMap.Selected_Area_VertexA.Y * 0x80);
                            modMath.sXY_int posB = new modMath.sXY_int(mainMap.Selected_Area_VertexB.X * 0x80, mainMap.Selected_Area_VertexB.Y * 0x80);
                            area.SetPositions(posA, posB);
                            this.ScriptMarkerLists_Update();
                            mainMap.SetChanged();
                            this.View_DrawViewLater();
                        }
                    }
                }
            }
        }

        private void btnScriptMarkerRemove_Click(object sender, EventArgs e)
        {
            if (this._SelectedScriptMarker != null)
            {
                clsMap mainMap = this.MainMap;
                if (mainMap != null)
                {
                    int arrayPosition;
                    if (this._SelectedScriptMarker is clsMap.clsScriptPosition)
                    {
                        clsMap.clsScriptPosition position = (clsMap.clsScriptPosition) this._SelectedScriptMarker;
                        arrayPosition = position.ParentMap.ArrayPosition;
                        position.Deallocate();
                        if (mainMap.ScriptPositions.Count > 0)
                        {
                            this._SelectedScriptMarker = mainMap.ScriptPositions[modMath.Clamp_int(arrayPosition, 0, mainMap.ScriptPositions.Count - 1)];
                        }
                        else
                        {
                            this._SelectedScriptMarker = null;
                        }
                    }
                    else if (this._SelectedScriptMarker is clsMap.clsScriptArea)
                    {
                        clsMap.clsScriptArea area = (clsMap.clsScriptArea) this._SelectedScriptMarker;
                        arrayPosition = area.ParentMap.ArrayPosition;
                        area.Deallocate();
                        if (mainMap.ScriptAreas.Count > 0)
                        {
                            this._SelectedScriptMarker = mainMap.ScriptAreas[modMath.Clamp_int(arrayPosition, 0, mainMap.ScriptAreas.Count - 1)];
                        }
                        else
                        {
                            this._SelectedScriptMarker = null;
                        }
                    }
                    this.ScriptMarkerLists_Update();
                    this.View_DrawViewLater();
                }
            }
        }

        private void btnSelResize_Click(object sender, EventArgs e)
        {
            clsMap mainMap = this.MainMap;
            if (mainMap != null)
            {
                if ((mainMap.Selected_Area_VertexA == null) | (mainMap.Selected_Area_VertexB == null))
                {
                    Interaction.MsgBox("You haven't selected anything.", MsgBoxStyle.ApplicationModal, "");
                }
                else
                {
                    modMath.sXY_int _int;
                    modMath.sXY_int _int2;
                    modMath.sXY_int _int3;
                    modMath.ReorderXY(mainMap.Selected_Area_VertexA.XY, mainMap.Selected_Area_VertexB.XY, ref _int3, ref _int2);
                    _int.X = _int2.X - _int3.X;
                    _int.Y = _int2.Y - _int3.Y;
                    this.Map_Resize(_int3, _int);
                }
            }
        }

        private void btnTextureAnticlockwise_Click(object sender, EventArgs e)
        {
            modProgram.TextureOrientation.RotateAnticlockwise();
            this.TextureView.DrawViewLater();
        }

        private void btnTextureClockwise_Click(object sender, EventArgs e)
        {
            modProgram.TextureOrientation.RotateClockwise();
            this.TextureView.DrawViewLater();
        }

        private void btnTextureFlipX_Click(object sender, EventArgs e)
        {
            if (modProgram.TextureOrientation.SwitchedAxes)
            {
                modProgram.TextureOrientation.ResultYFlip = !modProgram.TextureOrientation.ResultYFlip;
            }
            else
            {
                modProgram.TextureOrientation.ResultXFlip = !modProgram.TextureOrientation.ResultXFlip;
            }
            this.TextureView.DrawViewLater();
        }

        private void cboDroidBody_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cboDroidBody.Enabled && (this.cboDroidBody.SelectedIndex >= 0))
            {
                clsMap mainMap = this.MainMap;
                if (((mainMap != null) && (mainMap.SelectedUnits.Count > 0)) && ((mainMap.SelectedUnits.Count <= 1) || (Interaction.MsgBox("Change body of multiple droids?", MsgBoxStyle.Question | MsgBoxStyle.OkCancel, "") == MsgBoxResult.Ok)))
                {
                    clsMap.clsObjectBody tool = new clsMap.clsObjectBody {
                        Map = mainMap,
                        Body = this.cboBody_Objects[this.cboDroidBody.SelectedIndex]
                    };
                    mainMap.SelectedUnitsAction(tool);
                    this.SelectedObject_Changed();
                    if (tool.ActionPerformed)
                    {
                        mainMap.UndoStepCreate("Object Body Changed");
                        this.View_DrawViewLater();
                    }
                }
            }
        }

        private void cboDroidPropulsion_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cboDroidPropulsion.Enabled && (this.cboDroidPropulsion.SelectedIndex >= 0))
            {
                clsMap mainMap = this.MainMap;
                if (((mainMap != null) && (mainMap.SelectedUnits.Count > 0)) && ((mainMap.SelectedUnits.Count <= 1) || (Interaction.MsgBox("Change propulsion of multiple droids?", MsgBoxStyle.Question | MsgBoxStyle.OkCancel, "") == MsgBoxResult.Ok)))
                {
                    clsMap.clsObjectPropulsion tool = new clsMap.clsObjectPropulsion {
                        Map = mainMap,
                        Propulsion = this.cboPropulsion_Objects[this.cboDroidPropulsion.SelectedIndex]
                    };
                    mainMap.SelectedUnitsAction(tool);
                    this.SelectedObject_Changed();
                    if (tool.ActionPerformed)
                    {
                        mainMap.UndoStepCreate("Object Body Changed");
                        this.View_DrawViewLater();
                    }
                    this.SelectedObject_Changed();
                    if (tool.ActionPerformed)
                    {
                        mainMap.UndoStepCreate("Object Propulsion Changed");
                        this.View_DrawViewLater();
                    }
                }
            }
        }

        private void cboDroidTurret1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cboDroidTurret1.Enabled && (this.cboDroidTurret1.SelectedIndex >= 0))
            {
                clsMap mainMap = this.MainMap;
                if (((mainMap != null) && (mainMap.SelectedUnits.Count > 0)) && ((mainMap.SelectedUnits.Count <= 1) || (Interaction.MsgBox("Change turret of multiple droids?", MsgBoxStyle.Question | MsgBoxStyle.OkCancel, "") == MsgBoxResult.Ok)))
                {
                    clsMap.clsObjectTurret tool = new clsMap.clsObjectTurret {
                        Map = mainMap,
                        Turret = this.cboTurret_Objects[this.cboDroidTurret1.SelectedIndex],
                        TurretNum = 0
                    };
                    mainMap.SelectedUnitsAction(tool);
                    this.SelectedObject_Changed();
                    if (tool.ActionPerformed)
                    {
                        mainMap.UndoStepCreate("Object Turret Changed");
                        this.View_DrawViewLater();
                    }
                }
            }
        }

        private void cboDroidTurret2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cboDroidTurret2.Enabled && (this.cboDroidTurret2.SelectedIndex >= 0))
            {
                clsMap mainMap = this.MainMap;
                if (((mainMap != null) && (mainMap.SelectedUnits.Count > 0)) && ((mainMap.SelectedUnits.Count <= 1) || (Interaction.MsgBox("Change turret of multiple droids?", MsgBoxStyle.Question | MsgBoxStyle.OkCancel, "") == MsgBoxResult.Ok)))
                {
                    clsMap.clsObjectTurret tool = new clsMap.clsObjectTurret {
                        Map = mainMap,
                        Turret = this.cboTurret_Objects[this.cboDroidTurret2.SelectedIndex],
                        TurretNum = 1
                    };
                    mainMap.SelectedUnitsAction(tool);
                    this.SelectedObject_Changed();
                    if (tool.ActionPerformed)
                    {
                        mainMap.UndoStepCreate("Object Turret Changed");
                        this.View_DrawViewLater();
                    }
                }
            }
        }

        private void cboDroidTurret3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cboDroidTurret3.Enabled && (this.cboDroidTurret3.SelectedIndex >= 0))
            {
                clsMap mainMap = this.MainMap;
                if (((mainMap != null) && (mainMap.SelectedUnits.Count > 0)) && ((mainMap.SelectedUnits.Count <= 1) || (Interaction.MsgBox("Change turret of multiple droids?", MsgBoxStyle.Question | MsgBoxStyle.OkCancel, "") == MsgBoxResult.Ok)))
                {
                    clsMap.clsObjectTurret tool = new clsMap.clsObjectTurret {
                        Map = mainMap,
                        Turret = this.cboTurret_Objects[this.cboDroidTurret3.SelectedIndex],
                        TurretNum = 2
                    };
                    mainMap.SelectedUnitsAction(tool);
                    this.SelectedObject_Changed();
                    if (tool.ActionPerformed)
                    {
                        mainMap.UndoStepCreate("Object Turret Changed");
                        this.View_DrawViewLater();
                    }
                }
            }
        }

        private void cboDroidType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cboDroidType.Enabled && (this.cboDroidType.SelectedIndex >= 0))
            {
                clsMap mainMap = this.MainMap;
                if (((mainMap != null) && (mainMap.SelectedUnits.Count > 0)) && ((mainMap.SelectedUnits.Count <= 1) || (Interaction.MsgBox("Change type of multiple droids?", MsgBoxStyle.Question | MsgBoxStyle.OkCancel, "") == MsgBoxResult.Ok)))
                {
                    this.SelectedObjects_SetDroidType(modProgram.TemplateDroidTypes[this.cboDroidType.SelectedIndex]);
                }
            }
        }

        private void cboTileset_SelectedIndexChanged(object sender, EventArgs e)
        {
            clsMap mainMap = this.MainMap;
            if (mainMap != null)
            {
                clsTileset tileset;
                if (this.cboTileset.SelectedIndex < 0)
                {
                    tileset = null;
                }
                else
                {
                    tileset = modProgram.Tilesets[this.cboTileset.SelectedIndex];
                }
                if (tileset != mainMap.Tileset)
                {
                    mainMap.Tileset = tileset;
                    if (mainMap.Tileset != null)
                    {
                        modProgram.SelectedTextureNum = Math.Min(0, mainMap.Tileset.TileCount - 1);
                    }
                    mainMap.TileType_Reset();
                    mainMap.SetPainterToDefaults();
                    this.PainterTerrains_Refresh(-1, -1);
                    mainMap.SectorGraphicsChanges.SetAllChanged();
                    mainMap.Update();
                    mainMap.MinimapMakeLater();
                    this.View_DrawViewLater();
                    this.TextureView.ScrollUpdate();
                    this.TextureView.DrawViewLater();
                }
            }
        }

        public void cboTileset_Update(int NewSelectedIndex)
        {
            this.cboTileset.Items.Clear();
            int num2 = modProgram.Tilesets.Count - 1;
            for (int i = 0; i <= num2; i++)
            {
                this.cboTileset.Items.Add(modProgram.Tilesets[i].Name);
            }
            this.cboTileset.SelectedIndex = NewSelectedIndex;
        }

        private void cboTileType_Update()
        {
            IEnumerator enumerator;
            this.cboTileType.Items.Clear();
            try
            {
                enumerator = modProgram.TileTypes.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    modProgram.clsTileType current = (modProgram.clsTileType) enumerator.Current;
                    this.cboTileType.Items.Add(current.Name);
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

        private void cbxDesignableOnly_CheckedChanged(object sender, EventArgs e)
        {
            this.Components_Update();
        }

        private void chkTileNumbers_CheckedChanged(object sender, EventArgs e)
        {
            this.TextureView.DisplayTileNumbers = this.cbxTileNumbers.Checked;
            this.TextureView.DrawViewLater();
        }

        private void chkTileTypes_CheckedChanged(object sender, EventArgs e)
        {
            this.TextureView.DisplayTileTypes = this.cbxTileTypes.Checked;
            this.TextureView.DrawViewLater();
        }

        private void CloseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmbTileType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cboTileType.Enabled)
            {
                clsMap mainMap = this.MainMap;
                if (((mainMap != null) && (this.cboTileType.SelectedIndex >= 0)) && !((modProgram.SelectedTextureNum < 0) | (modProgram.SelectedTextureNum >= mainMap.Tileset.TileCount)))
                {
                    mainMap.Tile_TypeNum[modProgram.SelectedTextureNum] = (byte) this.cboTileType.SelectedIndex;
                    this.TextureView.DrawViewLater();
                }
            }
        }

        public void Components_Update()
        {
            if (modProgram.ObjectData != null)
            {
                int num;
                IEnumerator enumerator;
                IEnumerator enumerator2;
                IEnumerator enumerator3;
                this.cboDroidBody.Items.Clear();
                this.cboBody_Objects = new clsBody[(modProgram.ObjectData.Bodies.Count - 1) + 1];
                try
                {
                    enumerator = modProgram.ObjectData.Bodies.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        clsBody current = (clsBody) enumerator.Current;
                        if (current.Designable | !this.cbxDesignableOnly.Checked)
                        {
                            num = this.cboDroidBody.Items.Add("(" + current.Name + ") " + current.Code);
                            this.cboBody_Objects[num] = current;
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
                this.cboBody_Objects = (clsBody[]) Utils.CopyArray((Array) this.cboBody_Objects, new clsBody[(this.cboDroidBody.Items.Count - 1) + 1]);
                this.cboDroidPropulsion.Items.Clear();
                this.cboPropulsion_Objects = new clsPropulsion[(modProgram.ObjectData.Propulsions.Count - 1) + 1];
                try
                {
                    enumerator2 = modProgram.ObjectData.Propulsions.GetEnumerator();
                    while (enumerator2.MoveNext())
                    {
                        clsPropulsion propulsion = (clsPropulsion) enumerator2.Current;
                        if (propulsion.Designable | !this.cbxDesignableOnly.Checked)
                        {
                            num = this.cboDroidPropulsion.Items.Add("(" + propulsion.Name + ") " + propulsion.Code);
                            this.cboPropulsion_Objects[num] = propulsion;
                        }
                    }
                }
                finally
                {
                    if (enumerator2 is IDisposable)
                    {
                        (enumerator2 as IDisposable).Dispose();
                    }
                }
                this.cboPropulsion_Objects = (clsPropulsion[]) Utils.CopyArray((Array) this.cboPropulsion_Objects, new clsPropulsion[(this.cboDroidPropulsion.Items.Count - 1) + 1]);
                this.cboDroidTurret1.Items.Clear();
                this.cboDroidTurret2.Items.Clear();
                this.cboDroidTurret3.Items.Clear();
                this.cboTurret_Objects = new clsTurret[(modProgram.ObjectData.Turrets.Count - 1) + 1];
                try
                {
                    enumerator3 = modProgram.ObjectData.Turrets.GetEnumerator();
                    while (enumerator3.MoveNext())
                    {
                        clsTurret turret = (clsTurret) enumerator3.Current;
                        if (turret.Designable | !this.cbxDesignableOnly.Checked)
                        {
                            string result = null;
                            turret.GetTurretTypeName(ref result);
                            string item = "(" + result + " - " + turret.Name + ") " + turret.Code;
                            num = this.cboDroidTurret1.Items.Add(item);
                            this.cboDroidTurret2.Items.Add(item);
                            this.cboDroidTurret3.Items.Add(item);
                            this.cboTurret_Objects[num] = turret;
                        }
                    }
                }
                finally
                {
                    if (enumerator3 is IDisposable)
                    {
                        (enumerator3 as IDisposable).Dispose();
                    }
                }
                this.cboTurret_Objects = (clsTurret[]) Utils.CopyArray((Array) this.cboTurret_Objects, new clsTurret[(this.cboDroidTurret1.Items.Count - 1) + 1]);
                this.cboDroidType.Items.Clear();
                int num3 = modProgram.TemplateDroidTypeCount - 1;
                for (int i = 0; i <= num3; i++)
                {
                    this.cboDroidType.Items.Add(modProgram.TemplateDroidTypes[i].Name);
                }
            }
        }

        private void CreateTileTypes()
        {
            modProgram.clsTileType newItem = new modProgram.clsTileType();
            modProgram.clsTileType type2 = newItem;
            type2.Name = "Sand";
            type2.DisplayColour.Red = 1f;
            type2.DisplayColour.Green = 1f;
            type2.DisplayColour.Blue = 0f;
            type2 = null;
            modProgram.TileTypes.Add(newItem);
            newItem = new modProgram.clsTileType();
            modProgram.clsTileType type3 = newItem;
            type3.Name = "Sandy Brush";
            type3.DisplayColour.Red = 0.5f;
            type3.DisplayColour.Green = 0.5f;
            type3.DisplayColour.Blue = 0f;
            type3 = null;
            modProgram.TileTypes.Add(newItem);
            newItem = new modProgram.clsTileType();
            modProgram.clsTileType type4 = newItem;
            type4.Name = "Rubble";
            type4.DisplayColour.Red = 0.25f;
            type4.DisplayColour.Green = 0.25f;
            type4.DisplayColour.Blue = 0.25f;
            type4 = null;
            modProgram.TileTypes.Add(newItem);
            newItem = new modProgram.clsTileType();
            modProgram.clsTileType type5 = newItem;
            type5.Name = "Green Mud";
            type5.DisplayColour.Red = 0f;
            type5.DisplayColour.Green = 0.5f;
            type5.DisplayColour.Blue = 0f;
            type5 = null;
            modProgram.TileTypes.Add(newItem);
            newItem = new modProgram.clsTileType();
            modProgram.clsTileType type6 = newItem;
            type6.Name = "Red Brush";
            type6.DisplayColour.Red = 1f;
            type6.DisplayColour.Green = 0f;
            type6.DisplayColour.Blue = 0f;
            type6 = null;
            modProgram.TileTypes.Add(newItem);
            newItem = new modProgram.clsTileType();
            modProgram.clsTileType type7 = newItem;
            type7.Name = "Pink Rock";
            type7.DisplayColour.Red = 1f;
            type7.DisplayColour.Green = 0.5f;
            type7.DisplayColour.Blue = 0.5f;
            type7 = null;
            modProgram.TileTypes.Add(newItem);
            newItem = new modProgram.clsTileType();
            modProgram.clsTileType type8 = newItem;
            type8.Name = "Road";
            type8.DisplayColour.Red = 0f;
            type8.DisplayColour.Green = 0f;
            type8.DisplayColour.Blue = 0f;
            type8 = null;
            modProgram.TileTypes.Add(newItem);
            newItem = new modProgram.clsTileType();
            modProgram.clsTileType type9 = newItem;
            type9.Name = "Water";
            type9.DisplayColour.Red = 0f;
            type9.DisplayColour.Green = 0f;
            type9.DisplayColour.Blue = 1f;
            type9 = null;
            modProgram.TileTypes.Add(newItem);
            newItem = new modProgram.clsTileType();
            modProgram.clsTileType type10 = newItem;
            type10.Name = "Cliff Face";
            type10.DisplayColour.Red = 0.5f;
            type10.DisplayColour.Green = 0.5f;
            type10.DisplayColour.Blue = 0.5f;
            type10 = null;
            modProgram.TileTypes.Add(newItem);
            newItem = new modProgram.clsTileType();
            modProgram.clsTileType type11 = newItem;
            type11.Name = "Baked Earth";
            type11.DisplayColour.Red = 0.5f;
            type11.DisplayColour.Green = 0f;
            type11.DisplayColour.Blue = 0f;
            type11 = null;
            modProgram.TileTypes.Add(newItem);
            newItem = new modProgram.clsTileType();
            modProgram.clsTileType type12 = newItem;
            type12.Name = "Sheet Ice";
            type12.DisplayColour.Red = 1f;
            type12.DisplayColour.Green = 1f;
            type12.DisplayColour.Blue = 1f;
            type12 = null;
            modProgram.TileTypes.Add(newItem);
            newItem = new modProgram.clsTileType();
            modProgram.clsTileType type13 = newItem;
            type13.Name = "Slush";
            type13.DisplayColour.Red = 0.75f;
            type13.DisplayColour.Green = 0.75f;
            type13.DisplayColour.Blue = 0.75f;
            type13 = null;
            modProgram.TileTypes.Add(newItem);
        }

        private void dgvDroids_SelectionChanged(object sender, EventArgs e)
        {
            this.ActivateObjectTool();
            this.ObjectTypeSelectionUpdate(this.dgvDroids);
        }

        private void dgvFeatures_SelectionChanged(object sender, EventArgs e)
        {
            this.ActivateObjectTool();
            this.ObjectTypeSelectionUpdate(this.dgvFeatures);
        }

        private void dgvStructures_SelectionChanged(object sender, EventArgs e)
        {
            this.ActivateObjectTool();
            this.ObjectTypeSelectionUpdate(this.dgvStructures);
        }

        [DebuggerNonUserCode]
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && (this.components != null))
                {
                    this.components.Dispose();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        private void FMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            clsMap mainMap = this.MainMap;
            if ((mainMap != null) && mainMap.Save_FMap_Prompt())
            {
                modMain.frmMainInstance.tsbSave.Enabled = false;
                this.TitleTextUpdate();
            }
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            IEnumerator enumerator;
            bool flag = false;
            try
            {
                enumerator = this._LoadedMaps.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    clsMap current = (clsMap) enumerator.Current;
                    if (current.ChangedSinceSave)
                    {
                        flag = true;
                        goto Label_004B;
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
        Label_004B:
            if (flag)
            {
                frmQuit quit = new frmQuit();
                switch (quit.ShowDialog(modMain.frmMainInstance))
                {
                    case DialogResult.Cancel:
                        e.Cancel = true;
                        return;

                    case DialogResult.Yes:
                        while (this._LoadedMaps.Count > 0)
                        {
                            clsMap map = this._LoadedMaps[0];
                            this.SetMainMap(map);
                            if (!map.ClosePrompt())
                            {
                                e.Cancel = true;
                                return;
                            }
                            map.Deallocate();
                        }
                        break;
                }
            }
            clsResult result = modSettings.Settings_Write();
        }

        private void GeneratorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            modMain.frmGeneratorInstance.Show();
            modMain.frmGeneratorInstance.Activate();
        }

        public void HeightPickerL()
        {
            clsMap mainMap = this.MainMap;
            clsViewInfo.clsMouseOver.clsOverTerrain mouseOverTerrain = mainMap.ViewInfo.GetMouseOverTerrain();
            if (mouseOverTerrain != null)
            {
                this.txtHeightSetL.Text = modIO.InvariantToString_byte(mainMap.Terrain.Vertices[mouseOverTerrain.Vertex.Normal.X, mouseOverTerrain.Vertex.Normal.Y].Height);
                this.txtHeightSetL.Focus();
                this.MapView.OpenGLControl.Focus();
            }
        }

        public void HeightPickerR()
        {
            clsMap mainMap = this.MainMap;
            clsViewInfo.clsMouseOver.clsOverTerrain mouseOverTerrain = mainMap.ViewInfo.GetMouseOverTerrain();
            if (mouseOverTerrain != null)
            {
                this.txtHeightSetR.Text = modIO.InvariantToString_byte(mainMap.Terrain.Vertices[mouseOverTerrain.Vertex.Normal.X, mouseOverTerrain.Vertex.Normal.Y].Height);
                this.txtHeightSetR.Focus();
                this.MapView.OpenGLControl.Focus();
            }
        }

        private void ImportHeightmapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Load_Heightmap_Prompt();
        }

        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        public void Initialize(object sender, EventArgs e)
        {
            if (modProgram.ProgramInitialized)
            {
                Debugger.Break();
            }
            else if (this.MapView.IsGLInitialized & this.TextureView.IsGLInitialized)
            {
                this.Hide();
                new Thread(new ThreadStart(this.ShowThreadedSplashScreen)) { IsBackground = true }.Start();
                modProgram.ProgramInitialized = true;
                modMain.InitializeDelay.Enabled = false;
                modMain.InitializeDelay.Tick -= new EventHandler(this.Initialize);
                modMain.InitializeDelay.Dispose();
                modMain.InitializeDelay = null;
                modMain.InitializeResult.Add(this.LoadInterfaceImages());
                modTools.CreateTools();
                Angles.AnglePY anglePY = new Angles.AnglePY(-0.39269908169872414, 2.748893571891069);
                Matrix3DMath.MatrixSetToPY(modProgram.SunAngleMatrix, anglePY);
                this.NewPlayerNum.Left = 0x70;
                this.NewPlayerNum.Top = 10;
                this.Panel1.Controls.Add(this.NewPlayerNum);
                this.ObjectPlayerNum.Left = 0x48;
                this.ObjectPlayerNum.Top = 60;
                this.ObjectPlayerNum.Target = new clsMap.clsUnitGroupContainer();
                this.ObjectPlayerNum.Target.Changed += new clsMap.clsUnitGroupContainer.ChangedEventHandler(this.tabPlayerNum_SelectedIndexChanged);
                this.Panel14.Controls.Add(this.ObjectPlayerNum);
                this.ctrlTextureBrush = new ctrlBrush(modProgram.TextureBrush);
                this.pnlTextureBrush.Controls.Add(this.ctrlTextureBrush);
                this.ctrlTerrainBrush = new ctrlBrush(modProgram.TerrainBrush);
                this.pnlTerrainBrush.Controls.Add(this.ctrlTerrainBrush);
                this.ctrlCliffRemoveBrush = new ctrlBrush(modProgram.CliffBrush);
                this.pnlCliffRemoveBrush.Controls.Add(this.ctrlCliffRemoveBrush);
                this.ctrlHeightBrush = new ctrlBrush(modProgram.HeightBrush);
                this.pnlHeightSetBrush.Controls.Add(this.ctrlHeightBrush);
                VBMath.Randomize();
                this.CreateTileTypes();
                int index = 0;
                do
                {
                    modProgram.PlayerColour[index] = new modProgram.clsPlayer();
                    index++;
                }
                while (index <= 15);
                modProgram.PlayerColour[0].Colour.Red = 0f;
                modProgram.PlayerColour[0].Colour.Green = 0.3764706f;
                modProgram.PlayerColour[0].Colour.Blue = 0f;
                modProgram.PlayerColour[1].Colour.Red = 0.627451f;
                modProgram.PlayerColour[1].Colour.Green = 0.4392157f;
                modProgram.PlayerColour[1].Colour.Blue = 0f;
                modProgram.PlayerColour[2].Colour.Red = 0.5019608f;
                modProgram.PlayerColour[2].Colour.Green = 0.5019608f;
                modProgram.PlayerColour[2].Colour.Blue = 0.5019608f;
                modProgram.PlayerColour[3].Colour.Red = 0f;
                modProgram.PlayerColour[3].Colour.Green = 0f;
                modProgram.PlayerColour[3].Colour.Blue = 0f;
                modProgram.PlayerColour[4].Colour.Red = 0.5019608f;
                modProgram.PlayerColour[4].Colour.Green = 0f;
                modProgram.PlayerColour[4].Colour.Blue = 0f;
                modProgram.PlayerColour[5].Colour.Red = 0.1254902f;
                modProgram.PlayerColour[5].Colour.Green = 0.1882353f;
                modProgram.PlayerColour[5].Colour.Blue = 0.3764706f;
                modProgram.PlayerColour[6].Colour.Red = 0.5647059f;
                modProgram.PlayerColour[6].Colour.Green = 0f;
                modProgram.PlayerColour[6].Colour.Blue = 0.4392157f;
                modProgram.PlayerColour[7].Colour.Red = 0f;
                modProgram.PlayerColour[7].Colour.Green = 0.5019608f;
                modProgram.PlayerColour[7].Colour.Blue = 0.5019608f;
                modProgram.PlayerColour[8].Colour.Red = 0.5019608f;
                modProgram.PlayerColour[8].Colour.Green = 0.7529412f;
                modProgram.PlayerColour[8].Colour.Blue = 0f;
                modProgram.PlayerColour[9].Colour.Red = 0.6901961f;
                modProgram.PlayerColour[9].Colour.Green = 0.4392157f;
                modProgram.PlayerColour[9].Colour.Blue = 0.4392157f;
                modProgram.PlayerColour[10].Colour.Red = 0.8784314f;
                modProgram.PlayerColour[10].Colour.Green = 0.8784314f;
                modProgram.PlayerColour[10].Colour.Blue = 0.8784314f;
                modProgram.PlayerColour[11].Colour.Red = 0.1254902f;
                modProgram.PlayerColour[11].Colour.Green = 0.1254902f;
                modProgram.PlayerColour[11].Colour.Blue = 1f;
                modProgram.PlayerColour[12].Colour.Red = 0f;
                modProgram.PlayerColour[12].Colour.Green = 0.627451f;
                modProgram.PlayerColour[12].Colour.Blue = 0f;
                modProgram.PlayerColour[13].Colour.Red = 0.2509804f;
                modProgram.PlayerColour[13].Colour.Green = 0f;
                modProgram.PlayerColour[13].Colour.Blue = 0f;
                modProgram.PlayerColour[14].Colour.Red = 0.0627451f;
                modProgram.PlayerColour[14].Colour.Green = 0f;
                modProgram.PlayerColour[14].Colour.Blue = 0.2509804f;
                modProgram.PlayerColour[15].Colour.Red = 0.2509804f;
                modProgram.PlayerColour[15].Colour.Green = 0.3764706f;
                modProgram.PlayerColour[15].Colour.Blue = 0f;
                int num4 = 0;
                do
                {
                    modProgram.PlayerColour[num4].CalcMinimapColour();
                    num4++;
                }
                while (num4 <= 15);
                modProgram.MinimapFeatureColour.Red = 0.5f;
                modProgram.MinimapFeatureColour.Green = 0.5f;
                modProgram.MinimapFeatureColour.Blue = 0.5f;
                modSettings.UpdateSettings(modSettings.InitializeSettings);
                modSettings.InitializeSettings = null;
                if (modSettings.Settings.DirectoriesPrompt)
                {
                    modMain.frmOptionsInstance = new frmOptions();
                    if (modMain.frmOptionsInstance.ShowDialog() == DialogResult.Cancel)
                    {
                        ProjectData.EndApp();
                    }
                }
                int num2 = Conversions.ToInteger(modSettings.Settings.get_Value(modSettings.Setting_DefaultTilesetsPathNum));
                modLists.SimpleList<string> list2 = (modLists.SimpleList<string>) modSettings.Settings.get_Value(modSettings.Setting_TilesetDirectories);
                if ((num2 >= 0) & (num2 < list2.Count))
                {
                    string text = list2[num2];
                    if ((text != null) & (text != ""))
                    {
                        this.InitializeStatus = "Loading tilesets";
                        modMain.InitializeResult.Add(modProgram.LoadTilesets(modProgram.EndWithPathSeperator(text)));
                        this.InitializeStatus = "";
                    }
                }
                this.cboTileset_Update(-1);
                modMain.InitializeResult.Add(this.NoTile_Texture_Load());
                this.cboTileType_Update();
                modProgram.CreateTemplateDroidTypes();
                modProgram.ObjectData = new clsObjectData();
                int num = Conversions.ToInteger(modSettings.Settings.get_Value(modSettings.Setting_DefaultObjectDataPathNum));
                modLists.SimpleList<string> list = (modLists.SimpleList<string>) modSettings.Settings.get_Value(modSettings.Setting_ObjectDataDirectories);
                if ((num >= 0) & (num < list2.Count))
                {
                    string path = list[num];
                    if ((path != null) & (path != ""))
                    {
                        this.InitializeStatus = "Loading object data";
                        modMain.InitializeResult.Add(modProgram.ObjectData.LoadDirectory(path));
                        this.InitializeStatus = "";
                    }
                }
                modGenerator.CreateGeneratorTilesets();
                modPainters.CreatePainterArizona();
                modPainters.CreatePainterUrban();
                modPainters.CreatePainterRockies();
                this.Components_Update();
                this.MapView.Dock = DockStyle.Fill;
                this.pnlView.Controls.Add(this.MapView);
                modProgram.VisionRadius_2E = 10;
                modProgram.VisionRadius_2E_Changed();
                this.HeightSetPalette[0] = 0;
                this.HeightSetPalette[1] = 0x55;
                this.HeightSetPalette[2] = 170;
                this.HeightSetPalette[3] = 0xff;
                this.HeightSetPalette[4] = 0x40;
                this.HeightSetPalette[5] = 0x80;
                this.HeightSetPalette[6] = 0xc0;
                this.HeightSetPalette[7] = 0xff;
                int num5 = 0;
                do
                {
                    this.tabHeightSetL.TabPages[num5].Text = modIO.InvariantToString_byte(this.HeightSetPalette[num5]);
                    this.tabHeightSetR.TabPages[num5].Text = modIO.InvariantToString_byte(this.HeightSetPalette[num5]);
                    num5++;
                }
                while (num5 <= 7);
                this.tabHeightSetL.SelectedIndex = 1;
                this.tabHeightSetR.SelectedIndex = 0;
                this.tabHeightSetL_SelectedIndexChanged(null, null);
                this.tabHeightSetR_SelectedIndexChanged(null, null);
                if (modProgram.CommandLinePaths.Count >= 1)
                {
                    IEnumerator enumerator;
                    clsResult result = new clsResult("Loading startup command-line maps");
                    try
                    {
                        enumerator = modProgram.CommandLinePaths.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            string str3 = Conversions.ToString(enumerator.Current);
                            result.Take(this.LoadMap(str3));
                        }
                    }
                    finally
                    {
                        if (enumerator is IDisposable)
                        {
                            (enumerator as IDisposable).Dispose();
                        }
                    }
                    modProgram.ShowWarnings(result);
                }
                this.TextureView.Dock = DockStyle.Fill;
                this.TableLayoutPanel6.Controls.Add(this.TextureView, 0, 1);
                this.MainMapAfterChanged();
                this.MapView.DrawView_SetEnabled(true);
                this.TextureView.DrawView_SetEnabled(true);
                this.WindowState = FormWindowState.Maximized;
                this.Show();
                this.Activate();
                this.tmrKey.Enabled = true;
                this.tmrTool.Enabled = true;
                modProgram.ShowWarnings(modMain.InitializeResult);
                modProgram.ProgramInitializeFinished = true;
            }
        }

        [DebuggerStepThrough]
        private void InitializeComponent()
        {
            this.SplitContainer1 = new SplitContainer();
            this.TabControl = new System.Windows.Forms.TabControl();
            this.tpTextures = new TabPage();
            this.TableLayoutPanel6 = new TableLayoutPanel();
            this.Panel5 = new Panel();
            this.rdoTextureRemoveTerrain = new RadioButton();
            this.rdoTextureReinterpretTerrain = new RadioButton();
            this.rdoTextureIgnoreTerrain = new RadioButton();
            this.pnlTextureBrush = new Panel();
            this.chkTextureOrientationRandomize = new CheckBox();
            this.btnTextureFlipX = new Button();
            this.btnTextureClockwise = new Button();
            this.btnTextureAnticlockwise = new Button();
            this.chkSetTextureOrientation = new CheckBox();
            this.chkSetTexture = new CheckBox();
            this.Label21 = new Label();
            this.cboTileset = new ComboBox();
            this.Panel6 = new Panel();
            this.cbxTileNumbers = new CheckBox();
            this.cbxTileTypes = new CheckBox();
            this.Label20 = new Label();
            this.cboTileType = new ComboBox();
            this.tpAutoTexture = new TabPage();
            this.Panel15 = new Panel();
            this.cbxFillInside = new CheckBox();
            this.rdoFillCliffIgnore = new RadioButton();
            this.rdoFillCliffStopBefore = new RadioButton();
            this.rdoFillCliffStopAfter = new RadioButton();
            this.rdoCliffTriBrush = new RadioButton();
            this.rdoRoadRemove = new RadioButton();
            this.pnlCliffRemoveBrush = new Panel();
            this.pnlTerrainBrush = new Panel();
            this.cbxInvalidTiles = new CheckBox();
            this.cbxAutoTexSetHeight = new CheckBox();
            this.cbxCliffTris = new CheckBox();
            this.Label29 = new Label();
            this.rdoAutoRoadLine = new RadioButton();
            this.btnAutoTextureRemove = new Button();
            this.btnAutoRoadRemove = new Button();
            this.rdoAutoRoadPlace = new RadioButton();
            this.lstAutoRoad = new ListBox();
            this.rdoAutoTexturePlace = new RadioButton();
            this.rdoAutoTextureFill = new RadioButton();
            this.rdoAutoCliffBrush = new RadioButton();
            this.rdoAutoCliffRemove = new RadioButton();
            this.txtAutoCliffSlope = new TextBox();
            this.Label1 = new Label();
            this.lstAutoTexture = new ListBox();
            this.Label3 = new Label();
            this.tpHeight = new TabPage();
            this.cbxHeightChangeFade = new CheckBox();
            this.pnlHeightSetBrush = new Panel();
            this.btnHeightsMultiplySelection = new Button();
            this.btnHeightOffsetSelection = new Button();
            this.tabHeightSetR = new System.Windows.Forms.TabControl();
            this.TabPage25 = new TabPage();
            this.TabPage26 = new TabPage();
            this.TabPage27 = new TabPage();
            this.TabPage28 = new TabPage();
            this.TabPage29 = new TabPage();
            this.TabPage30 = new TabPage();
            this.TabPage31 = new TabPage();
            this.TabPage32 = new TabPage();
            this.tabHeightSetL = new System.Windows.Forms.TabControl();
            this.TabPage9 = new TabPage();
            this.TabPage10 = new TabPage();
            this.TabPage11 = new TabPage();
            this.TabPage12 = new TabPage();
            this.TabPage17 = new TabPage();
            this.TabPage18 = new TabPage();
            this.TabPage19 = new TabPage();
            this.TabPage20 = new TabPage();
            this.txtHeightSetR = new TextBox();
            this.Label27 = new Label();
            this.Label10 = new Label();
            this.txtHeightOffset = new TextBox();
            this.Label9 = new Label();
            this.txtHeightMultiply = new TextBox();
            this.txtHeightChangeRate = new TextBox();
            this.Label18 = new Label();
            this.rdoHeightChange = new RadioButton();
            this.Label16 = new Label();
            this.txtSmoothRate = new TextBox();
            this.Label6 = new Label();
            this.rdoHeightSmooth = new RadioButton();
            this.rdoHeightSet = new RadioButton();
            this.txtHeightSetL = new TextBox();
            this.Label5 = new Label();
            this.tpResize = new TabPage();
            this.btnSelResize = new Button();
            this.btnResize = new Button();
            this.txtOffsetY = new TextBox();
            this.Label15 = new Label();
            this.txtOffsetX = new TextBox();
            this.Label14 = new Label();
            this.txtSizeY = new TextBox();
            this.Label13 = new Label();
            this.txtSizeX = new TextBox();
            this.Label12 = new Label();
            this.tpObjects = new TabPage();
            this.TableLayoutPanel1 = new TableLayoutPanel();
            this.Panel1 = new Panel();
            this.btnPlayerSelectObjects = new Button();
            this.Label44 = new Label();
            this.GroupBox3 = new GroupBox();
            this.rdoObjectPlace = new RadioButton();
            this.rdoObjectLines = new RadioButton();
            this.txtObjectFind = new TextBox();
            this.cbxFootprintRotate = new CheckBox();
            this.txtNewObjectRotation = new TextBox();
            this.Label19 = new Label();
            this.cbxAutoWalls = new CheckBox();
            this.cbxObjectRandomRotation = new CheckBox();
            this.Label32 = new Label();
            this.Label22 = new Label();
            this.Panel2 = new Panel();
            this.btnObjectTypeSelect = new Button();
            this.TabControl1 = new System.Windows.Forms.TabControl();
            this.TabPage1 = new TabPage();
            this.dgvFeatures = new DataGridView();
            this.TabPage2 = new TabPage();
            this.dgvStructures = new DataGridView();
            this.TabPage3 = new TabPage();
            this.dgvDroids = new DataGridView();
            this.tpObject = new TabPage();
            this.TableLayoutPanel8 = new TableLayoutPanel();
            this.TableLayoutPanel9 = new TableLayoutPanel();
            this.cboDroidTurret3 = new ComboBox();
            this.cboDroidTurret2 = new ComboBox();
            this.Panel13 = new Panel();
            this.rdoDroidTurret3 = new RadioButton();
            this.cboDroidTurret1 = new ComboBox();
            this.cboDroidPropulsion = new ComboBox();
            this.cboDroidBody = new ComboBox();
            this.cboDroidType = new ComboBox();
            this.Panel12 = new Panel();
            this.rdoDroidTurret0 = new RadioButton();
            this.rdoDroidTurret2 = new RadioButton();
            this.Panel11 = new Panel();
            this.Label39 = new Label();
            this.rdoDroidTurret1 = new RadioButton();
            this.Panel10 = new Panel();
            this.Label38 = new Label();
            this.Panel9 = new Panel();
            this.Label37 = new Label();
            this.Panel8 = new Panel();
            this.Label40 = new Label();
            this.Panel14 = new Panel();
            this.btnFlatSelected = new Button();
            this.btnAlignObjects = new Button();
            this.Label31 = new Label();
            this.Label30 = new Label();
            this.cbxDesignableOnly = new CheckBox();
            this.Label17 = new Label();
            this.txtObjectLabel = new TextBox();
            this.Label35 = new Label();
            this.btnDroidToDesign = new Button();
            this.Label24 = new Label();
            this.lblObjectType = new Label();
            this.Label36 = new Label();
            this.Label23 = new Label();
            this.txtObjectHealth = new TextBox();
            this.txtObjectRotation = new TextBox();
            this.Label34 = new Label();
            this.Label28 = new Label();
            this.txtObjectPriority = new TextBox();
            this.Label25 = new Label();
            this.Label33 = new Label();
            this.txtObjectID = new TextBox();
            this.Label26 = new Label();
            this.tpLabels = new TabPage();
            this.Label11 = new Label();
            this.Label43 = new Label();
            this.Label42 = new Label();
            this.lstScriptAreas = new ListBox();
            this.lstScriptPositions = new ListBox();
            this.btnScriptAreaCreate = new Button();
            this.GroupBox1 = new GroupBox();
            this.txtScriptMarkerLabel = new TextBox();
            this.Label41 = new Label();
            this.btnScriptMarkerRemove = new Button();
            this.Label2 = new Label();
            this.txtScriptMarkerY = new TextBox();
            this.txtScriptMarkerX = new TextBox();
            this.Label7 = new Label();
            this.txtScriptMarkerY2 = new TextBox();
            this.Label4 = new Label();
            this.Label8 = new Label();
            this.txtScriptMarkerX2 = new TextBox();
            this.TableLayoutPanel7 = new TableLayoutPanel();
            this.Panel7 = new Panel();
            this.tsTools = new ToolStrip();
            this.tsbGateways = new ToolStripButton();
            this.tsbDrawAutotexture = new ToolStripButton();
            this.tsbDrawTileOrientation = new ToolStripButton();
            this.tsFile = new ToolStrip();
            this.tsbSave = new ToolStripButton();
            this.tsSelection = new ToolStrip();
            this.ToolStripLabel1 = new ToolStripLabel();
            this.tsbSelection = new ToolStripButton();
            this.tsbSelectionCopy = new ToolStripButton();
            this.tsbSelectionPasteOptions = new ToolStripDropDownButton();
            this.menuRotateUnits = new ToolStripMenuItem();
            this.menuRotateWalls = new ToolStripMenuItem();
            this.menuRotateNothing = new ToolStripMenuItem();
            this.ToolStripSeparator10 = new ToolStripSeparator();
            this.menuSelPasteHeights = new ToolStripMenuItem();
            this.menuSelPasteTextures = new ToolStripMenuItem();
            this.menuSelPasteUnits = new ToolStripMenuItem();
            this.menuSelPasteGateways = new ToolStripMenuItem();
            this.menuSelPasteDeleteUnits = new ToolStripMenuItem();
            this.menuSelPasteDeleteGateways = new ToolStripMenuItem();
            this.tsbSelectionPaste = new ToolStripButton();
            this.tsbSelectionRotateCounterClockwise = new ToolStripButton();
            this.tsbSelectionRotateClockwise = new ToolStripButton();
            this.tsbSelectionFlipX = new ToolStripButton();
            this.tsbSelectionObjects = new ToolStripButton();
            this.tsMinimap = new ToolStrip();
            this.menuMinimap = new ToolStripDropDownButton();
            this.menuMiniShowTex = new ToolStripMenuItem();
            this.menuMiniShowHeight = new ToolStripMenuItem();
            this.menuMiniShowCliffs = new ToolStripMenuItem();
            this.menuMiniShowUnits = new ToolStripMenuItem();
            this.menuMiniShowGateways = new ToolStripMenuItem();
            this.pnlView = new Panel();
            this.menuMain = new MenuStrip();
            this.menuFile = new ToolStripMenuItem();
            this.NewMapToolStripMenuItem = new ToolStripMenuItem();
            this.ToolStripSeparator3 = new ToolStripSeparator();
            this.OpenToolStripMenuItem = new ToolStripMenuItem();
            this.ToolStripSeparator2 = new ToolStripSeparator();
            this.SaveToolStripMenuItem = new ToolStripMenuItem();
            this.menuSaveFMap = new ToolStripMenuItem();
            this.ToolStripSeparator7 = new ToolStripSeparator();
            this.menuSaveFMapQuick = new ToolStripMenuItem();
            this.ToolStripSeparator11 = new ToolStripSeparator();
            this.menuSaveFME = new ToolStripMenuItem();
            this.ToolStripSeparator5 = new ToolStripSeparator();
            this.MapLNDToolStripMenuItem = new ToolStripMenuItem();
            this.ToolStripSeparator6 = new ToolStripSeparator();
            this.menuExportMapTileTypes = new ToolStripMenuItem();
            this.ToolStripMenuItem1 = new ToolStripSeparator();
            this.MinimapBMPToolStripMenuItem = new ToolStripMenuItem();
            this.ToolStripMenuItem3 = new ToolStripMenuItem();
            this.ToolStripSeparator1 = new ToolStripSeparator();
            this.ToolStripMenuItem4 = new ToolStripMenuItem();
            this.ImportHeightmapToolStripMenuItem = new ToolStripMenuItem();
            this.ToolStripSeparator8 = new ToolStripSeparator();
            this.menuImportTileTypes = new ToolStripMenuItem();
            this.ToolStripMenuItem2 = new ToolStripSeparator();
            this.MapWZToolStripMenuItem = new ToolStripMenuItem();
            this.ToolStripSeparator4 = new ToolStripSeparator();
            this.CloseToolStripMenuItem = new ToolStripMenuItem();
            this.menuTools = new ToolStripMenuItem();
            this.menuReinterpret = new ToolStripMenuItem();
            this.menuWaterCorrection = new ToolStripMenuItem();
            this.ToolStripSeparator9 = new ToolStripSeparator();
            this.menuFlatOil = new ToolStripMenuItem();
            this.menuFlatStructures = new ToolStripMenuItem();
            this.ToolStripSeparator12 = new ToolStripSeparator();
            this.menuGenerator = new ToolStripMenuItem();
            this.menuOptions = new ToolStripMenuItem();
            this.TableLayoutPanel5 = new TableLayoutPanel();
            this.TabPage13 = new TabPage();
            this.TabPage14 = new TabPage();
            this.TabPage15 = new TabPage();
            this.TabPage16 = new TabPage();
            this.TabPage21 = new TabPage();
            this.TabPage22 = new TabPage();
            this.TabPage23 = new TabPage();
            this.TabPage24 = new TabPage();
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
            ((ISupportInitialize) this.dgvFeatures).BeginInit();
            this.TabPage2.SuspendLayout();
            ((ISupportInitialize) this.dgvStructures).BeginInit();
            this.TabPage3.SuspendLayout();
            ((ISupportInitialize) this.dgvDroids).BeginInit();
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
            this.SplitContainer1.BorderStyle = BorderStyle.Fixed3D;
            this.SplitContainer1.Dock = DockStyle.Fill;
            Point point2 = new Point(4, 0x23);
            this.SplitContainer1.Location = point2;
            Padding padding2 = new Padding(4);
            this.SplitContainer1.Margin = padding2;
            this.SplitContainer1.Name = "SplitContainer1";
            this.SplitContainer1.Panel1.Controls.Add(this.TabControl);
            this.SplitContainer1.Panel2.BackColor = SystemColors.Control;
            this.SplitContainer1.Panel2.Controls.Add(this.TableLayoutPanel7);
            Size size2 = new Size(0x508, 0x268);
            this.SplitContainer1.Size = size2;
            this.SplitContainer1.SplitterDistance = 0x1a6;
            this.SplitContainer1.TabIndex = 0;
            this.TabControl.Appearance = TabAppearance.Buttons;
            this.TabControl.Controls.Add(this.tpTextures);
            this.TabControl.Controls.Add(this.tpAutoTexture);
            this.TabControl.Controls.Add(this.tpHeight);
            this.TabControl.Controls.Add(this.tpResize);
            this.TabControl.Controls.Add(this.tpObjects);
            this.TabControl.Controls.Add(this.tpObject);
            this.TabControl.Controls.Add(this.tpLabels);
            this.TabControl.Dock = DockStyle.Fill;
            size2 = new Size(0x48, 0x16);
            this.TabControl.ItemSize = size2;
            point2 = new Point(0, 0);
            this.TabControl.Location = point2;
            padding2 = new Padding(0);
            this.TabControl.Margin = padding2;
            this.TabControl.Multiline = true;
            this.TabControl.Name = "TabControl";
            point2 = new Point(0, 0);
            this.TabControl.Padding = point2;
            this.TabControl.SelectedIndex = 0;
            size2 = new Size(0x1a2, 0x264);
            this.TabControl.Size = size2;
            this.TabControl.TabIndex = 0;
            this.tpTextures.Controls.Add(this.TableLayoutPanel6);
            point2 = new Point(4, 0x33);
            this.tpTextures.Location = point2;
            padding2 = new Padding(0);
            this.tpTextures.Margin = padding2;
            this.tpTextures.Name = "tpTextures";
            padding2 = new Padding(4);
            this.tpTextures.Padding = padding2;
            size2 = new Size(410, 0x22d);
            this.tpTextures.Size = size2;
            this.tpTextures.TabIndex = 0;
            this.tpTextures.Text = "Textures";
            this.tpTextures.UseVisualStyleBackColor = true;
            this.TableLayoutPanel6.ColumnCount = 1;
            this.TableLayoutPanel6.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            this.TableLayoutPanel6.Controls.Add(this.Panel5, 0, 0);
            this.TableLayoutPanel6.Controls.Add(this.Panel6, 0, 2);
            this.TableLayoutPanel6.Dock = DockStyle.Fill;
            point2 = new Point(4, 4);
            this.TableLayoutPanel6.Location = point2;
            padding2 = new Padding(0);
            this.TableLayoutPanel6.Margin = padding2;
            this.TableLayoutPanel6.Name = "TableLayoutPanel6";
            this.TableLayoutPanel6.RowCount = 3;
            this.TableLayoutPanel6.RowStyles.Add(new RowStyle(SizeType.Absolute, 175f));
            this.TableLayoutPanel6.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            this.TableLayoutPanel6.RowStyles.Add(new RowStyle(SizeType.Absolute, 79f));
            size2 = new Size(0x192, 0x225);
            this.TableLayoutPanel6.Size = size2;
            this.TableLayoutPanel6.TabIndex = 8;
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
            this.Panel5.Dock = DockStyle.Fill;
            point2 = new Point(4, 4);
            this.Panel5.Location = point2;
            padding2 = new Padding(4);
            this.Panel5.Margin = padding2;
            this.Panel5.Name = "Panel5";
            size2 = new Size(0x18a, 0xa7);
            this.Panel5.Size = size2;
            this.Panel5.TabIndex = 0;
            this.rdoTextureRemoveTerrain.AutoSize = true;
            point2 = new Point(280, 0x81);
            this.rdoTextureRemoveTerrain.Location = point2;
            padding2 = new Padding(4);
            this.rdoTextureRemoveTerrain.Margin = padding2;
            this.rdoTextureRemoveTerrain.Name = "rdoTextureRemoveTerrain";
            size2 = new Size(0x7a, 0x15);
            this.rdoTextureRemoveTerrain.Size = size2;
            this.rdoTextureRemoveTerrain.TabIndex = 0x30;
            this.rdoTextureRemoveTerrain.Text = "Remove Terrain";
            this.rdoTextureRemoveTerrain.UseCompatibleTextRendering = true;
            this.rdoTextureRemoveTerrain.UseVisualStyleBackColor = true;
            this.rdoTextureReinterpretTerrain.AutoSize = true;
            this.rdoTextureReinterpretTerrain.Checked = true;
            point2 = new Point(280, 0x6d);
            this.rdoTextureReinterpretTerrain.Location = point2;
            padding2 = new Padding(4);
            this.rdoTextureReinterpretTerrain.Margin = padding2;
            this.rdoTextureReinterpretTerrain.Name = "rdoTextureReinterpretTerrain";
            size2 = new Size(0x5c, 0x15);
            this.rdoTextureReinterpretTerrain.Size = size2;
            this.rdoTextureReinterpretTerrain.TabIndex = 0x2f;
            this.rdoTextureReinterpretTerrain.TabStop = true;
            this.rdoTextureReinterpretTerrain.Text = "Reinterpret";
            this.rdoTextureReinterpretTerrain.UseCompatibleTextRendering = true;
            this.rdoTextureReinterpretTerrain.UseVisualStyleBackColor = true;
            this.rdoTextureIgnoreTerrain.AutoSize = true;
            point2 = new Point(280, 0x57);
            this.rdoTextureIgnoreTerrain.Location = point2;
            padding2 = new Padding(4);
            this.rdoTextureIgnoreTerrain.Margin = padding2;
            this.rdoTextureIgnoreTerrain.Name = "rdoTextureIgnoreTerrain";
            size2 = new Size(110, 0x15);
            this.rdoTextureIgnoreTerrain.Size = size2;
            this.rdoTextureIgnoreTerrain.TabIndex = 0x2e;
            this.rdoTextureIgnoreTerrain.Text = "Ignore Terrain";
            this.rdoTextureIgnoreTerrain.UseCompatibleTextRendering = true;
            this.rdoTextureIgnoreTerrain.UseVisualStyleBackColor = true;
            point2 = new Point(0x19, 0x2c);
            this.pnlTextureBrush.Location = point2;
            this.pnlTextureBrush.Name = "pnlTextureBrush";
            size2 = new Size(0x155, 0x24);
            this.pnlTextureBrush.Size = size2;
            this.pnlTextureBrush.TabIndex = 0x2d;
            this.chkTextureOrientationRandomize.AutoSize = true;
            point2 = new Point(0x9d, 0x89);
            this.chkTextureOrientationRandomize.Location = point2;
            padding2 = new Padding(4);
            this.chkTextureOrientationRandomize.Margin = padding2;
            this.chkTextureOrientationRandomize.Name = "chkTextureOrientationRandomize";
            size2 = new Size(0x5f, 0x15);
            this.chkTextureOrientationRandomize.Size = size2;
            this.chkTextureOrientationRandomize.TabIndex = 0x2c;
            this.chkTextureOrientationRandomize.Text = "Randomize";
            this.chkTextureOrientationRandomize.UseCompatibleTextRendering = true;
            this.chkTextureOrientationRandomize.UseVisualStyleBackColor = true;
            this.btnTextureFlipX.FlatStyle = FlatStyle.Flat;
            point2 = new Point(0xed, 0x67);
            this.btnTextureFlipX.Location = point2;
            padding2 = new Padding(0);
            this.btnTextureFlipX.Margin = padding2;
            this.btnTextureFlipX.Name = "btnTextureFlipX";
            size2 = new Size(0x20, 30);
            this.btnTextureFlipX.Size = size2;
            this.btnTextureFlipX.TabIndex = 0x2b;
            this.btnTextureFlipX.UseCompatibleTextRendering = true;
            this.btnTextureFlipX.UseVisualStyleBackColor = true;
            this.btnTextureClockwise.FlatStyle = FlatStyle.Flat;
            point2 = new Point(0xc5, 0x67);
            this.btnTextureClockwise.Location = point2;
            padding2 = new Padding(0);
            this.btnTextureClockwise.Margin = padding2;
            this.btnTextureClockwise.Name = "btnTextureClockwise";
            size2 = new Size(0x20, 30);
            this.btnTextureClockwise.Size = size2;
            this.btnTextureClockwise.TabIndex = 0x2a;
            this.btnTextureClockwise.UseCompatibleTextRendering = true;
            this.btnTextureClockwise.UseVisualStyleBackColor = true;
            this.btnTextureAnticlockwise.FlatStyle = FlatStyle.Flat;
            point2 = new Point(0x9d, 0x67);
            this.btnTextureAnticlockwise.Location = point2;
            padding2 = new Padding(0);
            this.btnTextureAnticlockwise.Margin = padding2;
            this.btnTextureAnticlockwise.Name = "btnTextureAnticlockwise";
            size2 = new Size(0x20, 30);
            this.btnTextureAnticlockwise.Size = size2;
            this.btnTextureAnticlockwise.TabIndex = 0x29;
            this.btnTextureAnticlockwise.UseCompatibleTextRendering = true;
            this.btnTextureAnticlockwise.UseVisualStyleBackColor = true;
            this.chkSetTextureOrientation.AutoSize = true;
            this.chkSetTextureOrientation.Checked = true;
            this.chkSetTextureOrientation.CheckState = CheckState.Checked;
            point2 = new Point(0x19, 110);
            this.chkSetTextureOrientation.Location = point2;
            padding2 = new Padding(4);
            this.chkSetTextureOrientation.Margin = padding2;
            this.chkSetTextureOrientation.Name = "chkSetTextureOrientation";
            size2 = new Size(0x74, 0x15);
            this.chkSetTextureOrientation.Size = size2;
            this.chkSetTextureOrientation.TabIndex = 40;
            this.chkSetTextureOrientation.Text = "Set Orientation";
            this.chkSetTextureOrientation.UseCompatibleTextRendering = true;
            this.chkSetTextureOrientation.UseVisualStyleBackColor = true;
            this.chkSetTexture.AutoSize = true;
            this.chkSetTexture.Checked = true;
            this.chkSetTexture.CheckState = CheckState.Checked;
            point2 = new Point(0x19, 0x51);
            this.chkSetTexture.Location = point2;
            padding2 = new Padding(4);
            this.chkSetTexture.Margin = padding2;
            this.chkSetTexture.Name = "chkSetTexture";
            size2 = new Size(0x60, 0x15);
            this.chkSetTexture.Size = size2;
            this.chkSetTexture.TabIndex = 0x27;
            this.chkSetTexture.Text = "Set Texture";
            this.chkSetTexture.UseCompatibleTextRendering = true;
            this.chkSetTexture.UseVisualStyleBackColor = true;
            point2 = new Point(0x15, 0x11);
            this.Label21.Location = point2;
            padding2 = new Padding(4, 0, 4, 0);
            this.Label21.Margin = padding2;
            this.Label21.Name = "Label21";
            size2 = new Size(0x40, 20);
            this.Label21.Size = size2;
            this.Label21.TabIndex = 8;
            this.Label21.Text = "Tileset:";
            this.Label21.TextAlign = ContentAlignment.MiddleRight;
            this.Label21.UseCompatibleTextRendering = true;
            this.cboTileset.DropDownHeight = 0x200;
            this.cboTileset.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cboTileset.DropDownWidth = 0x180;
            this.cboTileset.FormattingEnabled = true;
            this.cboTileset.IntegralHeight = false;
            this.cboTileset.Items.AddRange(new object[] { "Arizona", "Urban", "Rocky Mountains" });
            point2 = new Point(0x67, 0x10);
            this.cboTileset.Location = point2;
            padding2 = new Padding(4);
            this.cboTileset.Margin = padding2;
            this.cboTileset.Name = "cboTileset";
            size2 = new Size(0xa1, 0x18);
            this.cboTileset.Size = size2;
            this.cboTileset.TabIndex = 0;
            this.Panel6.Controls.Add(this.cbxTileNumbers);
            this.Panel6.Controls.Add(this.cbxTileTypes);
            this.Panel6.Controls.Add(this.Label20);
            this.Panel6.Controls.Add(this.cboTileType);
            this.Panel6.Dock = DockStyle.Fill;
            point2 = new Point(4, 0x1da);
            this.Panel6.Location = point2;
            padding2 = new Padding(4);
            this.Panel6.Margin = padding2;
            this.Panel6.Name = "Panel6";
            size2 = new Size(0x18a, 0x47);
            this.Panel6.Size = size2;
            this.Panel6.TabIndex = 2;
            point2 = new Point(0xaf, 0x25);
            this.cbxTileNumbers.Location = point2;
            padding2 = new Padding(4);
            this.cbxTileNumbers.Margin = padding2;
            this.cbxTileNumbers.Name = "cbxTileNumbers";
            size2 = new Size(0xb9, 0x1c);
            this.cbxTileNumbers.Size = size2;
            this.cbxTileNumbers.TabIndex = 3;
            this.cbxTileNumbers.Text = "Display Tile Numbers";
            this.cbxTileNumbers.UseCompatibleTextRendering = true;
            this.cbxTileNumbers.UseVisualStyleBackColor = true;
            point2 = new Point(8, 0x25);
            this.cbxTileTypes.Location = point2;
            padding2 = new Padding(4);
            this.cbxTileTypes.Margin = padding2;
            this.cbxTileTypes.Name = "cbxTileTypes";
            size2 = new Size(0x9f, 0x1c);
            this.cbxTileTypes.Size = size2;
            this.cbxTileTypes.TabIndex = 2;
            this.cbxTileTypes.Text = "Display Tile Types";
            this.cbxTileTypes.UseCompatibleTextRendering = true;
            this.cbxTileTypes.UseVisualStyleBackColor = true;
            point2 = new Point(4, 6);
            this.Label20.Location = point2;
            padding2 = new Padding(4, 0, 4, 0);
            this.Label20.Margin = padding2;
            this.Label20.Name = "Label20";
            size2 = new Size(0x5b, 0x1a);
            this.Label20.Size = size2;
            this.Label20.TabIndex = 1;
            this.Label20.Text = "Tile Type:";
            this.Label20.UseCompatibleTextRendering = true;
            this.cboTileType.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cboTileType.Enabled = false;
            this.cboTileType.FormattingEnabled = true;
            point2 = new Point(0x67, 4);
            this.cboTileType.Location = point2;
            padding2 = new Padding(4);
            this.cboTileType.Margin = padding2;
            this.cboTileType.Name = "cboTileType";
            size2 = new Size(0x97, 0x18);
            this.cboTileType.Size = size2;
            this.cboTileType.TabIndex = 0;
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
            point2 = new Point(4, 0x33);
            this.tpAutoTexture.Location = point2;
            padding2 = new Padding(4);
            this.tpAutoTexture.Margin = padding2;
            this.tpAutoTexture.Name = "tpAutoTexture";
            size2 = new Size(410, 0x22d);
            this.tpAutoTexture.Size = size2;
            this.tpAutoTexture.TabIndex = 2;
            this.tpAutoTexture.Text = "Terrain";
            this.tpAutoTexture.UseVisualStyleBackColor = true;
            this.Panel15.Controls.Add(this.cbxFillInside);
            this.Panel15.Controls.Add(this.rdoFillCliffIgnore);
            this.Panel15.Controls.Add(this.rdoFillCliffStopBefore);
            this.Panel15.Controls.Add(this.rdoFillCliffStopAfter);
            point2 = new Point(0x49, 0xf4);
            this.Panel15.Location = point2;
            this.Panel15.Name = "Panel15";
            size2 = new Size(0x139, 0x51);
            this.Panel15.Size = size2;
            this.Panel15.TabIndex = 0x35;
            point2 = new Point(0x87, 4);
            this.cbxFillInside.Location = point2;
            padding2 = new Padding(4);
            this.cbxFillInside.Margin = padding2;
            this.cbxFillInside.Name = "cbxFillInside";
            size2 = new Size(0x93, 0x15);
            this.cbxFillInside.Size = size2;
            this.cbxFillInside.TabIndex = 0x36;
            this.cbxFillInside.Text = "Stop Before Edge";
            this.cbxFillInside.UseCompatibleTextRendering = true;
            this.cbxFillInside.UseVisualStyleBackColor = true;
            this.rdoFillCliffIgnore.AutoSize = true;
            this.rdoFillCliffIgnore.Checked = true;
            point2 = new Point(4, 4);
            this.rdoFillCliffIgnore.Location = point2;
            padding2 = new Padding(4);
            this.rdoFillCliffIgnore.Margin = padding2;
            this.rdoFillCliffIgnore.Name = "rdoFillCliffIgnore";
            size2 = new Size(0x5b, 0x15);
            this.rdoFillCliffIgnore.Size = size2;
            this.rdoFillCliffIgnore.TabIndex = 0x34;
            this.rdoFillCliffIgnore.TabStop = true;
            this.rdoFillCliffIgnore.Text = "Ignore Cliff";
            this.rdoFillCliffIgnore.UseCompatibleTextRendering = true;
            this.rdoFillCliffIgnore.UseVisualStyleBackColor = true;
            this.rdoFillCliffStopBefore.AutoSize = true;
            point2 = new Point(4, 0x18);
            this.rdoFillCliffStopBefore.Location = point2;
            padding2 = new Padding(4);
            this.rdoFillCliffStopBefore.Margin = padding2;
            this.rdoFillCliffStopBefore.Name = "rdoFillCliffStopBefore";
            size2 = new Size(0x7b, 0x15);
            this.rdoFillCliffStopBefore.Size = size2;
            this.rdoFillCliffStopBefore.TabIndex = 50;
            this.rdoFillCliffStopBefore.Text = "Stop Before Cliff";
            this.rdoFillCliffStopBefore.UseCompatibleTextRendering = true;
            this.rdoFillCliffStopBefore.UseVisualStyleBackColor = true;
            this.rdoFillCliffStopAfter.AutoSize = true;
            point2 = new Point(4, 0x2b);
            this.rdoFillCliffStopAfter.Location = point2;
            padding2 = new Padding(4);
            this.rdoFillCliffStopAfter.Margin = padding2;
            this.rdoFillCliffStopAfter.Name = "rdoFillCliffStopAfter";
            size2 = new Size(0x70, 0x15);
            this.rdoFillCliffStopAfter.Size = size2;
            this.rdoFillCliffStopAfter.TabIndex = 0x33;
            this.rdoFillCliffStopAfter.Text = "Stop After Cliff";
            this.rdoFillCliffStopAfter.UseCompatibleTextRendering = true;
            this.rdoFillCliffStopAfter.UseVisualStyleBackColor = true;
            this.rdoCliffTriBrush.AutoSize = true;
            point2 = new Point(11, 0x1eb);
            this.rdoCliffTriBrush.Location = point2;
            padding2 = new Padding(4);
            this.rdoCliffTriBrush.Margin = padding2;
            this.rdoCliffTriBrush.Name = "rdoCliffTriBrush";
            size2 = new Size(0x65, 0x15);
            this.rdoCliffTriBrush.Size = size2;
            this.rdoCliffTriBrush.TabIndex = 0x31;
            this.rdoCliffTriBrush.Text = "Cliff Triangle";
            this.rdoCliffTriBrush.UseCompatibleTextRendering = true;
            this.rdoCliffTriBrush.UseVisualStyleBackColor = true;
            this.rdoRoadRemove.AutoSize = true;
            point2 = new Point(11, 0x1c4);
            this.rdoRoadRemove.Location = point2;
            padding2 = new Padding(4);
            this.rdoRoadRemove.Margin = padding2;
            this.rdoRoadRemove.Name = "rdoRoadRemove";
            size2 = new Size(0x4c, 0x15);
            this.rdoRoadRemove.Size = size2;
            this.rdoRoadRemove.TabIndex = 0x30;
            this.rdoRoadRemove.Text = "Remove";
            this.rdoRoadRemove.UseCompatibleTextRendering = true;
            this.rdoRoadRemove.UseVisualStyleBackColor = true;
            point2 = new Point(0x20, 0x256);
            this.pnlCliffRemoveBrush.Location = point2;
            this.pnlCliffRemoveBrush.Name = "pnlCliffRemoveBrush";
            size2 = new Size(0x155, 0x26);
            this.pnlCliffRemoveBrush.Size = size2;
            this.pnlCliffRemoveBrush.TabIndex = 0x2f;
            point2 = new Point(14, 3);
            this.pnlTerrainBrush.Location = point2;
            this.pnlTerrainBrush.Name = "pnlTerrainBrush";
            size2 = new Size(0x155, 0x26);
            this.pnlTerrainBrush.Size = size2;
            this.pnlTerrainBrush.TabIndex = 0x2e;
            this.cbxInvalidTiles.Checked = true;
            this.cbxInvalidTiles.CheckState = CheckState.Checked;
            point2 = new Point(0xb7, 0x3b);
            this.cbxInvalidTiles.Location = point2;
            padding2 = new Padding(4);
            this.cbxInvalidTiles.Margin = padding2;
            this.cbxInvalidTiles.Name = "cbxInvalidTiles";
            size2 = new Size(0x98, 0x15);
            this.cbxInvalidTiles.Size = size2;
            this.cbxInvalidTiles.TabIndex = 0x26;
            this.cbxInvalidTiles.Text = "Make Invalid Tiles";
            this.cbxInvalidTiles.UseCompatibleTextRendering = true;
            this.cbxInvalidTiles.UseVisualStyleBackColor = true;
            point2 = new Point(0x60, 0xd7);
            this.cbxAutoTexSetHeight.Location = point2;
            padding2 = new Padding(4);
            this.cbxAutoTexSetHeight.Margin = padding2;
            this.cbxAutoTexSetHeight.Name = "cbxAutoTexSetHeight";
            size2 = new Size(0x7f, 0x15);
            this.cbxAutoTexSetHeight.Size = size2;
            this.cbxAutoTexSetHeight.TabIndex = 0x24;
            this.cbxAutoTexSetHeight.Text = "Set Height";
            this.cbxAutoTexSetHeight.UseCompatibleTextRendering = true;
            this.cbxAutoTexSetHeight.UseVisualStyleBackColor = true;
            this.cbxCliffTris.Checked = true;
            this.cbxCliffTris.CheckState = CheckState.Checked;
            point2 = new Point(0xa1, 0x21f);
            this.cbxCliffTris.Location = point2;
            padding2 = new Padding(4);
            this.cbxCliffTris.Margin = padding2;
            this.cbxCliffTris.Name = "cbxCliffTris";
            size2 = new Size(0x7f, 0x15);
            this.cbxCliffTris.Size = size2;
            this.cbxCliffTris.TabIndex = 0x23;
            this.cbxCliffTris.Text = "Set Tris";
            this.cbxCliffTris.UseCompatibleTextRendering = true;
            this.cbxCliffTris.UseVisualStyleBackColor = true;
            point2 = new Point(11, 0x148);
            this.Label29.Location = point2;
            padding2 = new Padding(4, 0, 4, 0);
            this.Label29.Margin = padding2;
            this.Label29.Name = "Label29";
            size2 = new Size(0x6b, 20);
            this.Label29.Size = size2;
            this.Label29.TabIndex = 0x21;
            this.Label29.Text = "Road Type:";
            this.Label29.TextAlign = ContentAlignment.MiddleLeft;
            this.rdoAutoRoadLine.AutoSize = true;
            point2 = new Point(11, 0x1a7);
            this.rdoAutoRoadLine.Location = point2;
            padding2 = new Padding(4);
            this.rdoAutoRoadLine.Margin = padding2;
            this.rdoAutoRoadLine.Name = "rdoAutoRoadLine";
            size2 = new Size(0x3a, 0x15);
            this.rdoAutoRoadLine.Size = size2;
            this.rdoAutoRoadLine.TabIndex = 0x20;
            this.rdoAutoRoadLine.Text = "Lines";
            this.rdoAutoRoadLine.UseCompatibleTextRendering = true;
            this.rdoAutoRoadLine.UseVisualStyleBackColor = true;
            point2 = new Point(0xb2, 0xb1);
            this.btnAutoTextureRemove.Location = point2;
            padding2 = new Padding(4);
            this.btnAutoTextureRemove.Margin = padding2;
            this.btnAutoTextureRemove.Name = "btnAutoTextureRemove";
            size2 = new Size(0x55, 30);
            this.btnAutoTextureRemove.Size = size2;
            this.btnAutoTextureRemove.TabIndex = 0x1f;
            this.btnAutoTextureRemove.Text = "Erase";
            this.btnAutoTextureRemove.UseCompatibleTextRendering = true;
            this.btnAutoTextureRemove.UseVisualStyleBackColor = true;
            point2 = new Point(0x55, 0x18c);
            this.btnAutoRoadRemove.Location = point2;
            padding2 = new Padding(4);
            this.btnAutoRoadRemove.Margin = padding2;
            this.btnAutoRoadRemove.Name = "btnAutoRoadRemove";
            size2 = new Size(0x55, 30);
            this.btnAutoRoadRemove.Size = size2;
            this.btnAutoRoadRemove.TabIndex = 30;
            this.btnAutoRoadRemove.Text = "Erase";
            this.btnAutoRoadRemove.UseCompatibleTextRendering = true;
            this.btnAutoRoadRemove.UseVisualStyleBackColor = true;
            this.rdoAutoRoadPlace.AutoSize = true;
            point2 = new Point(11, 0x18a);
            this.rdoAutoRoadPlace.Location = point2;
            padding2 = new Padding(4);
            this.rdoAutoRoadPlace.Margin = padding2;
            this.rdoAutoRoadPlace.Name = "rdoAutoRoadPlace";
            size2 = new Size(0x3b, 0x15);
            this.rdoAutoRoadPlace.Size = size2;
            this.rdoAutoRoadPlace.TabIndex = 0x1d;
            this.rdoAutoRoadPlace.Text = "Sides";
            this.rdoAutoRoadPlace.UseCompatibleTextRendering = true;
            this.rdoAutoRoadPlace.UseVisualStyleBackColor = true;
            this.lstAutoRoad.FormattingEnabled = true;
            this.lstAutoRoad.ItemHeight = 0x10;
            point2 = new Point(11, 350);
            this.lstAutoRoad.Location = point2;
            padding2 = new Padding(4);
            this.lstAutoRoad.Margin = padding2;
            this.lstAutoRoad.Name = "lstAutoRoad";
            this.lstAutoRoad.ScrollAlwaysVisible = true;
            size2 = new Size(0x9f, 0x24);
            this.lstAutoRoad.Size = size2;
            this.lstAutoRoad.TabIndex = 0x1b;
            this.rdoAutoTexturePlace.AutoSize = true;
            point2 = new Point(11, 0xd6);
            this.rdoAutoTexturePlace.Location = point2;
            padding2 = new Padding(4);
            this.rdoAutoTexturePlace.Margin = padding2;
            this.rdoAutoTexturePlace.Name = "rdoAutoTexturePlace";
            size2 = new Size(0x3b, 0x15);
            this.rdoAutoTexturePlace.Size = size2;
            this.rdoAutoTexturePlace.TabIndex = 0x1a;
            this.rdoAutoTexturePlace.Text = "Place";
            this.rdoAutoTexturePlace.UseCompatibleTextRendering = true;
            this.rdoAutoTexturePlace.UseVisualStyleBackColor = true;
            this.rdoAutoTextureFill.AutoSize = true;
            point2 = new Point(11, 0xf4);
            this.rdoAutoTextureFill.Location = point2;
            padding2 = new Padding(4);
            this.rdoAutoTextureFill.Margin = padding2;
            this.rdoAutoTextureFill.Name = "rdoAutoTextureFill";
            size2 = new Size(0x2b, 0x15);
            this.rdoAutoTextureFill.Size = size2;
            this.rdoAutoTextureFill.TabIndex = 0x19;
            this.rdoAutoTextureFill.Text = "Fill";
            this.rdoAutoTextureFill.UseCompatibleTextRendering = true;
            this.rdoAutoTextureFill.UseVisualStyleBackColor = true;
            this.rdoAutoCliffBrush.AutoSize = true;
            point2 = new Point(11, 540);
            this.rdoAutoCliffBrush.Location = point2;
            padding2 = new Padding(4);
            this.rdoAutoCliffBrush.Margin = padding2;
            this.rdoAutoCliffBrush.Name = "rdoAutoCliffBrush";
            size2 = new Size(0x58, 0x15);
            this.rdoAutoCliffBrush.Size = size2;
            this.rdoAutoCliffBrush.TabIndex = 0x16;
            this.rdoAutoCliffBrush.Text = "Cliff Brush";
            this.rdoAutoCliffBrush.UseCompatibleTextRendering = true;
            this.rdoAutoCliffBrush.UseVisualStyleBackColor = true;
            this.rdoAutoCliffRemove.AutoSize = true;
            point2 = new Point(11, 570);
            this.rdoAutoCliffRemove.Location = point2;
            padding2 = new Padding(4);
            this.rdoAutoCliffRemove.Margin = padding2;
            this.rdoAutoCliffRemove.Name = "rdoAutoCliffRemove";
            size2 = new Size(0x66, 0x15);
            this.rdoAutoCliffRemove.Size = size2;
            this.rdoAutoCliffRemove.TabIndex = 0x15;
            this.rdoAutoCliffRemove.Text = "Cliff Remove";
            this.rdoAutoCliffRemove.UseCompatibleTextRendering = true;
            this.rdoAutoCliffRemove.UseVisualStyleBackColor = true;
            point2 = new Point(0x75, 0x204);
            this.txtAutoCliffSlope.Location = point2;
            padding2 = new Padding(4);
            this.txtAutoCliffSlope.Margin = padding2;
            this.txtAutoCliffSlope.Name = "txtAutoCliffSlope";
            size2 = new Size(0x34, 0x16);
            this.txtAutoCliffSlope.Size = size2;
            this.txtAutoCliffSlope.TabIndex = 7;
            this.txtAutoCliffSlope.Text = "35";
            this.txtAutoCliffSlope.TextAlign = HorizontalAlignment.Right;
            point2 = new Point(11, 0x204);
            this.Label1.Location = point2;
            padding2 = new Padding(4, 0, 4, 0);
            this.Label1.Margin = padding2;
            this.Label1.Name = "Label1";
            size2 = new Size(0x60, 20);
            this.Label1.Size = size2;
            this.Label1.TabIndex = 6;
            this.Label1.Text = "Cliff Angle";
            this.Label1.TextAlign = ContentAlignment.MiddleRight;
            this.Label1.UseCompatibleTextRendering = true;
            this.lstAutoTexture.FormattingEnabled = true;
            this.lstAutoTexture.ItemHeight = 0x10;
            point2 = new Point(11, 0x3b);
            this.lstAutoTexture.Location = point2;
            padding2 = new Padding(4);
            this.lstAutoTexture.Margin = padding2;
            this.lstAutoTexture.Name = "lstAutoTexture";
            this.lstAutoTexture.ScrollAlwaysVisible = true;
            size2 = new Size(0x9f, 0x94);
            this.lstAutoTexture.Size = size2;
            this.lstAutoTexture.TabIndex = 4;
            point2 = new Point(11, 0x27);
            this.Label3.Location = point2;
            padding2 = new Padding(4, 0, 4, 0);
            this.Label3.Margin = padding2;
            this.Label3.Name = "Label3";
            size2 = new Size(0x6b, 20);
            this.Label3.Size = size2;
            this.Label3.TabIndex = 3;
            this.Label3.Text = "Ground Type";
            this.Label3.TextAlign = ContentAlignment.MiddleLeft;
            this.Label3.UseCompatibleTextRendering = true;
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
            point2 = new Point(4, 0x33);
            this.tpHeight.Location = point2;
            padding2 = new Padding(4);
            this.tpHeight.Margin = padding2;
            this.tpHeight.Name = "tpHeight";
            padding2 = new Padding(4);
            this.tpHeight.Padding = padding2;
            size2 = new Size(410, 0x22d);
            this.tpHeight.Size = size2;
            this.tpHeight.TabIndex = 1;
            this.tpHeight.Text = "Height";
            this.tpHeight.UseVisualStyleBackColor = true;
            this.cbxHeightChangeFade.Checked = true;
            this.cbxHeightChangeFade.CheckState = CheckState.Checked;
            point2 = new Point(0xb1, 0xf8);
            this.cbxHeightChangeFade.Location = point2;
            padding2 = new Padding(4);
            this.cbxHeightChangeFade.Margin = padding2;
            this.cbxHeightChangeFade.Name = "cbxHeightChangeFade";
            size2 = new Size(0x98, 0x15);
            this.cbxHeightChangeFade.Size = size2;
            this.cbxHeightChangeFade.TabIndex = 0x2f;
            this.cbxHeightChangeFade.Text = "Fading";
            this.cbxHeightChangeFade.UseCompatibleTextRendering = true;
            this.cbxHeightChangeFade.UseVisualStyleBackColor = true;
            point2 = new Point(0x1d, 8);
            this.pnlHeightSetBrush.Location = point2;
            this.pnlHeightSetBrush.Name = "pnlHeightSetBrush";
            size2 = new Size(0x155, 0x26);
            this.pnlHeightSetBrush.Size = size2;
            this.pnlHeightSetBrush.TabIndex = 0x2e;
            point2 = new Point(0x79, 0x17f);
            this.btnHeightsMultiplySelection.Location = point2;
            padding2 = new Padding(4);
            this.btnHeightsMultiplySelection.Margin = padding2;
            this.btnHeightsMultiplySelection.Name = "btnHeightsMultiplySelection";
            size2 = new Size(0x4b, 30);
            this.btnHeightsMultiplySelection.Size = size2;
            this.btnHeightsMultiplySelection.TabIndex = 0x26;
            this.btnHeightsMultiplySelection.Text = "Do";
            this.btnHeightsMultiplySelection.UseVisualStyleBackColor = true;
            point2 = new Point(0x79, 0x1c4);
            this.btnHeightOffsetSelection.Location = point2;
            padding2 = new Padding(4);
            this.btnHeightOffsetSelection.Margin = padding2;
            this.btnHeightOffsetSelection.Name = "btnHeightOffsetSelection";
            size2 = new Size(0x4b, 30);
            this.btnHeightOffsetSelection.Size = size2;
            this.btnHeightOffsetSelection.TabIndex = 0x25;
            this.btnHeightOffsetSelection.Text = "Do";
            this.btnHeightOffsetSelection.UseVisualStyleBackColor = true;
            this.tabHeightSetR.Appearance = TabAppearance.Buttons;
            this.tabHeightSetR.Controls.Add(this.TabPage25);
            this.tabHeightSetR.Controls.Add(this.TabPage26);
            this.tabHeightSetR.Controls.Add(this.TabPage27);
            this.tabHeightSetR.Controls.Add(this.TabPage28);
            this.tabHeightSetR.Controls.Add(this.TabPage29);
            this.tabHeightSetR.Controls.Add(this.TabPage30);
            this.tabHeightSetR.Controls.Add(this.TabPage31);
            this.tabHeightSetR.Controls.Add(this.TabPage32);
            size2 = new Size(0x1c, 20);
            this.tabHeightSetR.ItemSize = size2;
            point2 = new Point(0x1d, 0xa9);
            this.tabHeightSetR.Location = point2;
            padding2 = new Padding(0);
            this.tabHeightSetR.Margin = padding2;
            this.tabHeightSetR.Multiline = true;
            this.tabHeightSetR.Name = "tabHeightSetR";
            point2 = new Point(0, 0);
            this.tabHeightSetR.Padding = point2;
            this.tabHeightSetR.SelectedIndex = 0;
            size2 = new Size(0x1b7, 0x19);
            this.tabHeightSetR.Size = size2;
            this.tabHeightSetR.SizeMode = TabSizeMode.Fixed;
            this.tabHeightSetR.TabIndex = 0x23;
            point2 = new Point(4, 0x18);
            this.TabPage25.Location = point2;
            padding2 = new Padding(4);
            this.TabPage25.Margin = padding2;
            this.TabPage25.Name = "TabPage25";
            padding2 = new Padding(4);
            this.TabPage25.Padding = padding2;
            size2 = new Size(0x1af, 0);
            this.TabPage25.Size = size2;
            this.TabPage25.TabIndex = 0;
            this.TabPage25.Text = "1";
            this.TabPage25.UseVisualStyleBackColor = true;
            point2 = new Point(4, 0x18);
            this.TabPage26.Location = point2;
            padding2 = new Padding(4);
            this.TabPage26.Margin = padding2;
            this.TabPage26.Name = "TabPage26";
            padding2 = new Padding(4);
            this.TabPage26.Padding = padding2;
            size2 = new Size(0x1af, 0);
            this.TabPage26.Size = size2;
            this.TabPage26.TabIndex = 1;
            this.TabPage26.Text = "2";
            this.TabPage26.UseVisualStyleBackColor = true;
            point2 = new Point(4, 0x18);
            this.TabPage27.Location = point2;
            padding2 = new Padding(4);
            this.TabPage27.Margin = padding2;
            this.TabPage27.Name = "TabPage27";
            size2 = new Size(0x1af, 0);
            this.TabPage27.Size = size2;
            this.TabPage27.TabIndex = 2;
            this.TabPage27.Text = "3";
            this.TabPage27.UseVisualStyleBackColor = true;
            point2 = new Point(4, 0x18);
            this.TabPage28.Location = point2;
            padding2 = new Padding(4);
            this.TabPage28.Margin = padding2;
            this.TabPage28.Name = "TabPage28";
            size2 = new Size(0x1af, 0);
            this.TabPage28.Size = size2;
            this.TabPage28.TabIndex = 3;
            this.TabPage28.Text = "4";
            this.TabPage28.UseVisualStyleBackColor = true;
            point2 = new Point(4, 0x18);
            this.TabPage29.Location = point2;
            padding2 = new Padding(4);
            this.TabPage29.Margin = padding2;
            this.TabPage29.Name = "TabPage29";
            size2 = new Size(0x1af, 0);
            this.TabPage29.Size = size2;
            this.TabPage29.TabIndex = 4;
            this.TabPage29.Text = "5";
            this.TabPage29.UseVisualStyleBackColor = true;
            point2 = new Point(4, 0x18);
            this.TabPage30.Location = point2;
            padding2 = new Padding(4);
            this.TabPage30.Margin = padding2;
            this.TabPage30.Name = "TabPage30";
            size2 = new Size(0x1af, 0);
            this.TabPage30.Size = size2;
            this.TabPage30.TabIndex = 5;
            this.TabPage30.Text = "6";
            this.TabPage30.UseVisualStyleBackColor = true;
            point2 = new Point(4, 0x18);
            this.TabPage31.Location = point2;
            padding2 = new Padding(4);
            this.TabPage31.Margin = padding2;
            this.TabPage31.Name = "TabPage31";
            size2 = new Size(0x1af, 0);
            this.TabPage31.Size = size2;
            this.TabPage31.TabIndex = 6;
            this.TabPage31.Text = "7";
            this.TabPage31.UseVisualStyleBackColor = true;
            point2 = new Point(4, 0x18);
            this.TabPage32.Location = point2;
            padding2 = new Padding(4);
            this.TabPage32.Margin = padding2;
            this.TabPage32.Name = "TabPage32";
            size2 = new Size(0x1af, 0);
            this.TabPage32.Size = size2;
            this.TabPage32.TabIndex = 7;
            this.TabPage32.Text = "8";
            this.TabPage32.UseVisualStyleBackColor = true;
            this.tabHeightSetL.Appearance = TabAppearance.Buttons;
            this.tabHeightSetL.Controls.Add(this.TabPage9);
            this.tabHeightSetL.Controls.Add(this.TabPage10);
            this.tabHeightSetL.Controls.Add(this.TabPage11);
            this.tabHeightSetL.Controls.Add(this.TabPage12);
            this.tabHeightSetL.Controls.Add(this.TabPage17);
            this.tabHeightSetL.Controls.Add(this.TabPage18);
            this.tabHeightSetL.Controls.Add(this.TabPage19);
            this.tabHeightSetL.Controls.Add(this.TabPage20);
            size2 = new Size(0x1c, 20);
            this.tabHeightSetL.ItemSize = size2;
            point2 = new Point(0x1d, 0x6b);
            this.tabHeightSetL.Location = point2;
            padding2 = new Padding(0);
            this.tabHeightSetL.Margin = padding2;
            this.tabHeightSetL.Multiline = true;
            this.tabHeightSetL.Name = "tabHeightSetL";
            point2 = new Point(0, 0);
            this.tabHeightSetL.Padding = point2;
            this.tabHeightSetL.SelectedIndex = 0;
            size2 = new Size(0x1b7, 0x19);
            this.tabHeightSetL.Size = size2;
            this.tabHeightSetL.SizeMode = TabSizeMode.Fixed;
            this.tabHeightSetL.TabIndex = 0x22;
            point2 = new Point(4, 0x18);
            this.TabPage9.Location = point2;
            padding2 = new Padding(4);
            this.TabPage9.Margin = padding2;
            this.TabPage9.Name = "TabPage9";
            padding2 = new Padding(4);
            this.TabPage9.Padding = padding2;
            size2 = new Size(0x1af, 0);
            this.TabPage9.Size = size2;
            this.TabPage9.TabIndex = 0;
            this.TabPage9.Text = "1";
            this.TabPage9.UseVisualStyleBackColor = true;
            point2 = new Point(4, 0x18);
            this.TabPage10.Location = point2;
            padding2 = new Padding(4);
            this.TabPage10.Margin = padding2;
            this.TabPage10.Name = "TabPage10";
            padding2 = new Padding(4);
            this.TabPage10.Padding = padding2;
            size2 = new Size(0x1af, 0);
            this.TabPage10.Size = size2;
            this.TabPage10.TabIndex = 1;
            this.TabPage10.Text = "2";
            this.TabPage10.UseVisualStyleBackColor = true;
            point2 = new Point(4, 0x18);
            this.TabPage11.Location = point2;
            padding2 = new Padding(4);
            this.TabPage11.Margin = padding2;
            this.TabPage11.Name = "TabPage11";
            size2 = new Size(0x1af, 0);
            this.TabPage11.Size = size2;
            this.TabPage11.TabIndex = 2;
            this.TabPage11.Text = "3";
            this.TabPage11.UseVisualStyleBackColor = true;
            point2 = new Point(4, 0x18);
            this.TabPage12.Location = point2;
            padding2 = new Padding(4);
            this.TabPage12.Margin = padding2;
            this.TabPage12.Name = "TabPage12";
            size2 = new Size(0x1af, 0);
            this.TabPage12.Size = size2;
            this.TabPage12.TabIndex = 3;
            this.TabPage12.Text = "4";
            this.TabPage12.UseVisualStyleBackColor = true;
            point2 = new Point(4, 0x18);
            this.TabPage17.Location = point2;
            padding2 = new Padding(4);
            this.TabPage17.Margin = padding2;
            this.TabPage17.Name = "TabPage17";
            size2 = new Size(0x1af, 0);
            this.TabPage17.Size = size2;
            this.TabPage17.TabIndex = 4;
            this.TabPage17.Text = "5";
            this.TabPage17.UseVisualStyleBackColor = true;
            point2 = new Point(4, 0x18);
            this.TabPage18.Location = point2;
            padding2 = new Padding(4);
            this.TabPage18.Margin = padding2;
            this.TabPage18.Name = "TabPage18";
            size2 = new Size(0x1af, 0);
            this.TabPage18.Size = size2;
            this.TabPage18.TabIndex = 5;
            this.TabPage18.Text = "6";
            this.TabPage18.UseVisualStyleBackColor = true;
            point2 = new Point(4, 0x18);
            this.TabPage19.Location = point2;
            padding2 = new Padding(4);
            this.TabPage19.Margin = padding2;
            this.TabPage19.Name = "TabPage19";
            size2 = new Size(0x1af, 0);
            this.TabPage19.Size = size2;
            this.TabPage19.TabIndex = 6;
            this.TabPage19.Text = "7";
            this.TabPage19.UseVisualStyleBackColor = true;
            point2 = new Point(4, 0x18);
            this.TabPage20.Location = point2;
            padding2 = new Padding(4);
            this.TabPage20.Margin = padding2;
            this.TabPage20.Name = "TabPage20";
            size2 = new Size(0x1af, 0);
            this.TabPage20.Size = size2;
            this.TabPage20.TabIndex = 7;
            this.TabPage20.Text = "8";
            this.TabPage20.UseVisualStyleBackColor = true;
            point2 = new Point(0x79, 140);
            this.txtHeightSetR.Location = point2;
            padding2 = new Padding(4);
            this.txtHeightSetR.Margin = padding2;
            this.txtHeightSetR.Name = "txtHeightSetR";
            size2 = new Size(0x49, 0x16);
            this.txtHeightSetR.Size = size2;
            this.txtHeightSetR.TabIndex = 0x1f;
            this.txtHeightSetR.Text = "#";
            this.txtHeightSetR.TextAlign = HorizontalAlignment.Right;
            point2 = new Point(11, 140);
            this.Label27.Location = point2;
            padding2 = new Padding(4, 0, 4, 0);
            this.Label27.Margin = padding2;
            this.Label27.Name = "Label27";
            size2 = new Size(0x63, 20);
            this.Label27.Size = size2;
            this.Label27.TabIndex = 30;
            this.Label27.Text = "RMB Height";
            this.Label27.TextAlign = ContentAlignment.MiddleRight;
            this.Label27.UseCompatibleTextRendering = true;
            point2 = new Point(15, 0x1af);
            this.Label10.Location = point2;
            padding2 = new Padding(4, 0, 4, 0);
            this.Label10.Margin = padding2;
            this.Label10.Name = "Label10";
            size2 = new Size(0xd1, 0x11);
            this.Label10.Size = size2;
            this.Label10.TabIndex = 0x1d;
            this.Label10.Text = "Offset Heights of Selection";
            this.Label10.TextAlign = ContentAlignment.MiddleLeft;
            point2 = new Point(0x13, 0x1c7);
            this.txtHeightOffset.Location = point2;
            padding2 = new Padding(4);
            this.txtHeightOffset.Margin = padding2;
            this.txtHeightOffset.Name = "txtHeightOffset";
            size2 = new Size(0x59, 0x16);
            this.txtHeightOffset.Size = size2;
            this.txtHeightOffset.TabIndex = 0x1b;
            this.txtHeightOffset.Text = "0";
            this.txtHeightOffset.TextAlign = HorizontalAlignment.Right;
            point2 = new Point(11, 0x167);
            this.Label9.Location = point2;
            padding2 = new Padding(4, 0, 4, 0);
            this.Label9.Margin = padding2;
            this.Label9.Name = "Label9";
            size2 = new Size(0xd1, 20);
            this.Label9.Size = size2;
            this.Label9.TabIndex = 0x19;
            this.Label9.Text = "Multiply Heights of Selection";
            this.Label9.TextAlign = ContentAlignment.MiddleLeft;
            this.Label9.UseCompatibleTextRendering = true;
            point2 = new Point(0x13, 0x182);
            this.txtHeightMultiply.Location = point2;
            padding2 = new Padding(4);
            this.txtHeightMultiply.Margin = padding2;
            this.txtHeightMultiply.Name = "txtHeightMultiply";
            size2 = new Size(0x59, 0x16);
            this.txtHeightMultiply.Size = size2;
            this.txtHeightMultiply.TabIndex = 0x18;
            this.txtHeightMultiply.Text = "1";
            this.txtHeightMultiply.TextAlign = HorizontalAlignment.Right;
            point2 = new Point(0x60, 0xf6);
            this.txtHeightChangeRate.Location = point2;
            padding2 = new Padding(4);
            this.txtHeightChangeRate.Margin = padding2;
            this.txtHeightChangeRate.Name = "txtHeightChangeRate";
            size2 = new Size(0x49, 0x16);
            this.txtHeightChangeRate.Size = size2;
            this.txtHeightChangeRate.TabIndex = 0x17;
            this.txtHeightChangeRate.Text = "16";
            this.txtHeightChangeRate.TextAlign = HorizontalAlignment.Right;
            point2 = new Point(11, 0xf6);
            this.Label18.Location = point2;
            padding2 = new Padding(4, 0, 4, 0);
            this.Label18.Margin = padding2;
            this.Label18.Name = "Label18";
            size2 = new Size(0x4b, 20);
            this.Label18.Size = size2;
            this.Label18.TabIndex = 0x16;
            this.Label18.Text = "Rate";
            this.Label18.TextAlign = ContentAlignment.MiddleRight;
            this.Label18.UseCompatibleTextRendering = true;
            this.rdoHeightChange.AutoSize = true;
            point2 = new Point(11, 0xd9);
            this.rdoHeightChange.Location = point2;
            padding2 = new Padding(4);
            this.rdoHeightChange.Margin = padding2;
            this.rdoHeightChange.Name = "rdoHeightChange";
            size2 = new Size(0x49, 0x15);
            this.rdoHeightChange.Size = size2;
            this.rdoHeightChange.TabIndex = 0x15;
            this.rdoHeightChange.Text = "Change";
            this.rdoHeightChange.UseCompatibleTextRendering = true;
            this.rdoHeightChange.UseVisualStyleBackColor = true;
            point2 = new Point(0x79, 0x31);
            this.Label16.Location = point2;
            padding2 = new Padding(4, 0, 4, 0);
            this.Label16.Margin = padding2;
            this.Label16.Name = "Label16";
            size2 = new Size(0x4b, 0x15);
            this.Label16.Size = size2;
            this.Label16.TabIndex = 20;
            this.Label16.Text = "(0-255)";
            this.Label16.TextAlign = ContentAlignment.MiddleRight;
            this.Label16.UseCompatibleTextRendering = true;
            point2 = new Point(100, 0x137);
            this.txtSmoothRate.Location = point2;
            padding2 = new Padding(4);
            this.txtSmoothRate.Margin = padding2;
            this.txtSmoothRate.Name = "txtSmoothRate";
            size2 = new Size(0x49, 0x16);
            this.txtSmoothRate.Size = size2;
            this.txtSmoothRate.TabIndex = 10;
            this.txtSmoothRate.Text = "3";
            this.txtSmoothRate.TextAlign = HorizontalAlignment.Right;
            point2 = new Point(15, 0x137);
            this.Label6.Location = point2;
            padding2 = new Padding(4, 0, 4, 0);
            this.Label6.Margin = padding2;
            this.Label6.Name = "Label6";
            size2 = new Size(0x4b, 20);
            this.Label6.Size = size2;
            this.Label6.TabIndex = 9;
            this.Label6.Text = "Rate";
            this.Label6.TextAlign = ContentAlignment.MiddleRight;
            this.Label6.UseCompatibleTextRendering = true;
            this.rdoHeightSmooth.AutoSize = true;
            point2 = new Point(11, 0x11e);
            this.rdoHeightSmooth.Location = point2;
            padding2 = new Padding(4);
            this.rdoHeightSmooth.Margin = padding2;
            this.rdoHeightSmooth.Name = "rdoHeightSmooth";
            size2 = new Size(0x48, 0x15);
            this.rdoHeightSmooth.Size = size2;
            this.rdoHeightSmooth.TabIndex = 8;
            this.rdoHeightSmooth.Text = "Smooth";
            this.rdoHeightSmooth.UseCompatibleTextRendering = true;
            this.rdoHeightSmooth.UseVisualStyleBackColor = true;
            this.rdoHeightSet.AutoSize = true;
            this.rdoHeightSet.Checked = true;
            point2 = new Point(11, 0x31);
            this.rdoHeightSet.Location = point2;
            padding2 = new Padding(4);
            this.rdoHeightSet.Margin = padding2;
            this.rdoHeightSet.Name = "rdoHeightSet";
            size2 = new Size(0x2e, 0x15);
            this.rdoHeightSet.Size = size2;
            this.rdoHeightSet.TabIndex = 7;
            this.rdoHeightSet.TabStop = true;
            this.rdoHeightSet.Text = "Set";
            this.rdoHeightSet.UseCompatibleTextRendering = true;
            this.rdoHeightSet.UseVisualStyleBackColor = true;
            point2 = new Point(0x79, 0x4f);
            this.txtHeightSetL.Location = point2;
            padding2 = new Padding(4);
            this.txtHeightSetL.Margin = padding2;
            this.txtHeightSetL.Name = "txtHeightSetL";
            size2 = new Size(0x49, 0x16);
            this.txtHeightSetL.Size = size2;
            this.txtHeightSetL.TabIndex = 6;
            this.txtHeightSetL.Text = "#";
            this.txtHeightSetL.TextAlign = HorizontalAlignment.Right;
            point2 = new Point(11, 0x4f);
            this.Label5.Location = point2;
            padding2 = new Padding(4, 0, 4, 0);
            this.Label5.Margin = padding2;
            this.Label5.Name = "Label5";
            size2 = new Size(0x63, 20);
            this.Label5.Size = size2;
            this.Label5.TabIndex = 5;
            this.Label5.Text = "LMB Height";
            this.Label5.TextAlign = ContentAlignment.MiddleRight;
            this.Label5.UseCompatibleTextRendering = true;
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
            point2 = new Point(4, 0x33);
            this.tpResize.Location = point2;
            padding2 = new Padding(4);
            this.tpResize.Margin = padding2;
            this.tpResize.Name = "tpResize";
            size2 = new Size(410, 0x22d);
            this.tpResize.Size = size2;
            this.tpResize.TabIndex = 4;
            this.tpResize.Text = "Resize";
            this.tpResize.UseVisualStyleBackColor = true;
            point2 = new Point(0x15, 0xaf);
            this.btnSelResize.Location = point2;
            padding2 = new Padding(4);
            this.btnSelResize.Margin = padding2;
            this.btnSelResize.Name = "btnSelResize";
            size2 = new Size(180, 30);
            this.btnSelResize.Size = size2;
            this.btnSelResize.TabIndex = 0x11;
            this.btnSelResize.Text = "Resize To Selection";
            this.btnSelResize.UseCompatibleTextRendering = true;
            this.btnSelResize.UseVisualStyleBackColor = true;
            point2 = new Point(0x15, 0x8a);
            this.btnResize.Location = point2;
            padding2 = new Padding(4);
            this.btnResize.Margin = padding2;
            this.btnResize.Name = "btnResize";
            size2 = new Size(0x94, 30);
            this.btnResize.Size = size2;
            this.btnResize.TabIndex = 0x10;
            this.btnResize.Text = "Resize";
            this.btnResize.UseCompatibleTextRendering = true;
            this.btnResize.UseVisualStyleBackColor = true;
            point2 = new Point(0x75, 0x62);
            this.txtOffsetY.Location = point2;
            padding2 = new Padding(4);
            this.txtOffsetY.Margin = padding2;
            this.txtOffsetY.Name = "txtOffsetY";
            size2 = new Size(0x34, 0x16);
            this.txtOffsetY.Size = size2;
            this.txtOffsetY.TabIndex = 15;
            this.txtOffsetY.Text = "0";
            point2 = new Point(11, 0x62);
            this.Label15.Location = point2;
            padding2 = new Padding(4, 0, 4, 0);
            this.Label15.Margin = padding2;
            this.Label15.Name = "Label15";
            size2 = new Size(0x60, 20);
            this.Label15.Size = size2;
            this.Label15.TabIndex = 14;
            this.Label15.Text = "Offset Y";
            this.Label15.TextAlign = ContentAlignment.MiddleRight;
            this.Label15.UseCompatibleTextRendering = true;
            point2 = new Point(0x75, 0x45);
            this.txtOffsetX.Location = point2;
            padding2 = new Padding(4);
            this.txtOffsetX.Margin = padding2;
            this.txtOffsetX.Name = "txtOffsetX";
            size2 = new Size(0x34, 0x16);
            this.txtOffsetX.Size = size2;
            this.txtOffsetX.TabIndex = 13;
            this.txtOffsetX.Text = "0";
            point2 = new Point(11, 0x45);
            this.Label14.Location = point2;
            padding2 = new Padding(4, 0, 4, 0);
            this.Label14.Margin = padding2;
            this.Label14.Name = "Label14";
            size2 = new Size(0x60, 20);
            this.Label14.Size = size2;
            this.Label14.TabIndex = 12;
            this.Label14.Text = "Offset X";
            this.Label14.TextAlign = ContentAlignment.MiddleRight;
            this.Label14.UseCompatibleTextRendering = true;
            point2 = new Point(0x75, 0x27);
            this.txtSizeY.Location = point2;
            padding2 = new Padding(4);
            this.txtSizeY.Margin = padding2;
            this.txtSizeY.Name = "txtSizeY";
            size2 = new Size(0x34, 0x16);
            this.txtSizeY.Size = size2;
            this.txtSizeY.TabIndex = 11;
            this.txtSizeY.Text = "0";
            point2 = new Point(11, 0x27);
            this.Label13.Location = point2;
            padding2 = new Padding(4, 0, 4, 0);
            this.Label13.Margin = padding2;
            this.Label13.Name = "Label13";
            size2 = new Size(0x60, 20);
            this.Label13.Size = size2;
            this.Label13.TabIndex = 10;
            this.Label13.Text = "Size Y";
            this.Label13.TextAlign = ContentAlignment.MiddleRight;
            this.Label13.UseCompatibleTextRendering = true;
            point2 = new Point(0x75, 10);
            this.txtSizeX.Location = point2;
            padding2 = new Padding(4);
            this.txtSizeX.Margin = padding2;
            this.txtSizeX.Name = "txtSizeX";
            size2 = new Size(0x34, 0x16);
            this.txtSizeX.Size = size2;
            this.txtSizeX.TabIndex = 9;
            this.txtSizeX.Text = "0";
            point2 = new Point(11, 10);
            this.Label12.Location = point2;
            padding2 = new Padding(4, 0, 4, 0);
            this.Label12.Margin = padding2;
            this.Label12.Name = "Label12";
            size2 = new Size(0x60, 20);
            this.Label12.Size = size2;
            this.Label12.TabIndex = 8;
            this.Label12.Text = "Size X";
            this.Label12.TextAlign = ContentAlignment.MiddleRight;
            this.Label12.UseCompatibleTextRendering = true;
            this.tpObjects.AutoScroll = true;
            this.tpObjects.Controls.Add(this.TableLayoutPanel1);
            point2 = new Point(4, 0x33);
            this.tpObjects.Location = point2;
            padding2 = new Padding(4);
            this.tpObjects.Margin = padding2;
            this.tpObjects.Name = "tpObjects";
            size2 = new Size(410, 0x22d);
            this.tpObjects.Size = size2;
            this.tpObjects.TabIndex = 5;
            this.tpObjects.Text = "Place Objects";
            this.tpObjects.UseVisualStyleBackColor = true;
            this.TableLayoutPanel1.ColumnCount = 1;
            this.TableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            this.TableLayoutPanel1.Controls.Add(this.Panel1, 0, 0);
            this.TableLayoutPanel1.Controls.Add(this.Panel2, 0, 1);
            this.TableLayoutPanel1.Dock = DockStyle.Fill;
            point2 = new Point(0, 0);
            this.TableLayoutPanel1.Location = point2;
            padding2 = new Padding(4);
            this.TableLayoutPanel1.Margin = padding2;
            this.TableLayoutPanel1.Name = "TableLayoutPanel1";
            this.TableLayoutPanel1.RowCount = 2;
            this.TableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 192f));
            this.TableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            size2 = new Size(410, 0x22d);
            this.TableLayoutPanel1.Size = size2;
            this.TableLayoutPanel1.TabIndex = 0x10;
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
            this.Panel1.Dock = DockStyle.Fill;
            point2 = new Point(4, 4);
            this.Panel1.Location = point2;
            padding2 = new Padding(4);
            this.Panel1.Margin = padding2;
            this.Panel1.Name = "Panel1";
            size2 = new Size(0x192, 0xb8);
            this.Panel1.Size = size2;
            this.Panel1.TabIndex = 0;
            point2 = new Point(0x11a, 10);
            this.btnPlayerSelectObjects.Location = point2;
            this.btnPlayerSelectObjects.Name = "btnPlayerSelectObjects";
            size2 = new Size(0x6f, 0x23);
            this.btnPlayerSelectObjects.Size = size2;
            this.btnPlayerSelectObjects.TabIndex = 0x11;
            this.btnPlayerSelectObjects.Text = "Select All";
            this.btnPlayerSelectObjects.UseCompatibleTextRendering = true;
            this.btnPlayerSelectObjects.UseVisualStyleBackColor = true;
            point2 = new Point(3, 0xa3);
            this.Label44.Location = point2;
            this.Label44.Name = "Label44";
            size2 = new Size(0x26, 0x15);
            this.Label44.Size = size2;
            this.Label44.TabIndex = 0x39;
            this.Label44.Text = "Find:";
            this.Label44.TextAlign = ContentAlignment.MiddleRight;
            this.Label44.UseCompatibleTextRendering = true;
            this.GroupBox3.Controls.Add(this.rdoObjectPlace);
            this.GroupBox3.Controls.Add(this.rdoObjectLines);
            point2 = new Point(0x11a, 0x33);
            this.GroupBox3.Location = point2;
            this.GroupBox3.Name = "GroupBox3";
            size2 = new Size(120, 0x4b);
            this.GroupBox3.Size = size2;
            this.GroupBox3.TabIndex = 0x38;
            this.GroupBox3.TabStop = false;
            this.GroupBox3.Text = "Tool";
            this.GroupBox3.UseCompatibleTextRendering = true;
            this.rdoObjectPlace.AutoSize = true;
            this.rdoObjectPlace.Checked = true;
            point2 = new Point(7, 0x16);
            this.rdoObjectPlace.Location = point2;
            padding2 = new Padding(4);
            this.rdoObjectPlace.Margin = padding2;
            this.rdoObjectPlace.Name = "rdoObjectPlace";
            size2 = new Size(0x3b, 0x15);
            this.rdoObjectPlace.Size = size2;
            this.rdoObjectPlace.TabIndex = 0x36;
            this.rdoObjectPlace.TabStop = true;
            this.rdoObjectPlace.Text = "Place";
            this.rdoObjectPlace.UseCompatibleTextRendering = true;
            this.rdoObjectPlace.UseVisualStyleBackColor = true;
            this.rdoObjectLines.AutoSize = true;
            point2 = new Point(7, 0x2f);
            this.rdoObjectLines.Location = point2;
            padding2 = new Padding(4);
            this.rdoObjectLines.Margin = padding2;
            this.rdoObjectLines.Name = "rdoObjectLines";
            size2 = new Size(0x3a, 0x15);
            this.rdoObjectLines.Size = size2;
            this.rdoObjectLines.TabIndex = 0x37;
            this.rdoObjectLines.Text = "Lines";
            this.rdoObjectLines.UseCompatibleTextRendering = true;
            this.rdoObjectLines.UseVisualStyleBackColor = true;
            point2 = new Point(0x2f, 160);
            this.txtObjectFind.Location = point2;
            this.txtObjectFind.Name = "txtObjectFind";
            size2 = new Size(0xaf, 0x16);
            this.txtObjectFind.Size = size2;
            this.txtObjectFind.TabIndex = 0x35;
            point2 = new Point(0xf4, 0x85);
            this.cbxFootprintRotate.Location = point2;
            padding2 = new Padding(4);
            this.cbxFootprintRotate.Margin = padding2;
            this.cbxFootprintRotate.Name = "cbxFootprintRotate";
            size2 = new Size(0xc9, 0x15);
            this.cbxFootprintRotate.Size = size2;
            this.cbxFootprintRotate.TabIndex = 0x34;
            this.cbxFootprintRotate.Text = "Rotate Footprints (3.1+)";
            this.cbxFootprintRotate.UseCompatibleTextRendering = true;
            this.cbxFootprintRotate.UseVisualStyleBackColor = true;
            point2 = new Point(0x55, 0x7a);
            this.txtNewObjectRotation.Location = point2;
            padding2 = new Padding(4);
            this.txtNewObjectRotation.Margin = padding2;
            this.txtNewObjectRotation.Name = "txtNewObjectRotation";
            size2 = new Size(0x29, 0x16);
            this.txtNewObjectRotation.Size = size2;
            this.txtNewObjectRotation.TabIndex = 0x33;
            this.txtNewObjectRotation.Text = "0";
            this.txtNewObjectRotation.TextAlign = HorizontalAlignment.Right;
            point2 = new Point(0x10, 0x7d);
            this.Label19.Location = point2;
            this.Label19.Name = "Label19";
            size2 = new Size(0x3e, 0x15);
            this.Label19.Size = size2;
            this.Label19.TabIndex = 50;
            this.Label19.Text = "Rotation:";
            this.Label19.UseCompatibleTextRendering = true;
            point2 = new Point(0xf4, 0xa1);
            this.cbxAutoWalls.Location = point2;
            padding2 = new Padding(4);
            this.cbxAutoWalls.Margin = padding2;
            this.cbxAutoWalls.Name = "cbxAutoWalls";
            size2 = new Size(0x98, 0x15);
            this.cbxAutoWalls.Size = size2;
            this.cbxAutoWalls.TabIndex = 0x31;
            this.cbxAutoWalls.Text = "Automatic Walls";
            this.cbxAutoWalls.UseCompatibleTextRendering = true;
            this.cbxAutoWalls.UseVisualStyleBackColor = true;
            point2 = new Point(0x86, 0x7c);
            this.cbxObjectRandomRotation.Location = point2;
            padding2 = new Padding(4);
            this.cbxObjectRandomRotation.Margin = padding2;
            this.cbxObjectRandomRotation.Name = "cbxObjectRandomRotation";
            size2 = new Size(0x59, 0x15);
            this.cbxObjectRandomRotation.Size = size2;
            this.cbxObjectRandomRotation.TabIndex = 0x30;
            this.cbxObjectRandomRotation.Text = "Random";
            this.cbxObjectRandomRotation.UseCompatibleTextRendering = true;
            this.cbxObjectRandomRotation.UseVisualStyleBackColor = true;
            point2 = new Point(0x10, 0x52);
            this.Label32.Location = point2;
            this.Label32.Name = "Label32";
            size2 = new Size(0xdb, 0x26);
            this.Label32.Size = size2;
            this.Label32.TabIndex = 0x10;
            this.Label32.Text = "Players 8 and 9 only work with versions 3.1+";
            this.Label32.UseCompatibleTextRendering = true;
            point2 = new Point(0x10, 10);
            this.Label22.Location = point2;
            padding2 = new Padding(4, 0, 4, 0);
            this.Label22.Margin = padding2;
            this.Label22.Name = "Label22";
            size2 = new Size(0x55, 0x19);
            this.Label22.Size = size2;
            this.Label22.TabIndex = 14;
            this.Label22.Text = "Player";
            this.Label22.TextAlign = ContentAlignment.MiddleLeft;
            this.Label22.UseCompatibleTextRendering = true;
            this.Panel2.Controls.Add(this.btnObjectTypeSelect);
            this.Panel2.Controls.Add(this.TabControl1);
            this.Panel2.Dock = DockStyle.Fill;
            point2 = new Point(3, 0xc3);
            this.Panel2.Location = point2;
            this.Panel2.Name = "Panel2";
            size2 = new Size(0x194, 0x167);
            this.Panel2.Size = size2;
            this.Panel2.TabIndex = 1;
            this.btnObjectTypeSelect.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            point2 = new Point(0x139, 0);
            this.btnObjectTypeSelect.Location = point2;
            this.btnObjectTypeSelect.Name = "btnObjectTypeSelect";
            size2 = new Size(90, 0x1c);
            this.btnObjectTypeSelect.Size = size2;
            this.btnObjectTypeSelect.TabIndex = 0x3a;
            this.btnObjectTypeSelect.Text = "Select";
            this.btnObjectTypeSelect.UseCompatibleTextRendering = true;
            this.btnObjectTypeSelect.UseVisualStyleBackColor = true;
            this.TabControl1.Appearance = TabAppearance.Buttons;
            this.TabControl1.Controls.Add(this.TabPage1);
            this.TabControl1.Controls.Add(this.TabPage2);
            this.TabControl1.Controls.Add(this.TabPage3);
            this.TabControl1.Dock = DockStyle.Fill;
            point2 = new Point(0, 0);
            this.TabControl1.Location = point2;
            this.TabControl1.Name = "TabControl1";
            this.TabControl1.SelectedIndex = 0;
            size2 = new Size(0x194, 0x167);
            this.TabControl1.Size = size2;
            this.TabControl1.TabIndex = 0;
            this.TabPage1.Controls.Add(this.dgvFeatures);
            point2 = new Point(4, 0x1c);
            this.TabPage1.Location = point2;
            this.TabPage1.Name = "TabPage1";
            size2 = new Size(0x18c, 0x147);
            this.TabPage1.Size = size2;
            this.TabPage1.TabIndex = 0;
            this.TabPage1.Text = "Features";
            this.TabPage1.UseVisualStyleBackColor = true;
            this.dgvFeatures.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            this.dgvFeatures.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvFeatures.Dock = DockStyle.Fill;
            point2 = new Point(0, 0);
            this.dgvFeatures.Location = point2;
            this.dgvFeatures.Name = "dgvFeatures";
            this.dgvFeatures.ReadOnly = true;
            this.dgvFeatures.RowHeadersVisible = false;
            this.dgvFeatures.RowTemplate.Height = 0x18;
            this.dgvFeatures.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            size2 = new Size(0x18c, 0x147);
            this.dgvFeatures.Size = size2;
            this.dgvFeatures.TabIndex = 0;
            this.TabPage2.Controls.Add(this.dgvStructures);
            point2 = new Point(4, 0x1c);
            this.TabPage2.Location = point2;
            this.TabPage2.Name = "TabPage2";
            size2 = new Size(0x18c, 0x147);
            this.TabPage2.Size = size2;
            this.TabPage2.TabIndex = 1;
            this.TabPage2.Text = "Structures";
            this.TabPage2.UseVisualStyleBackColor = true;
            this.dgvStructures.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            this.dgvStructures.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvStructures.Dock = DockStyle.Fill;
            point2 = new Point(0, 0);
            this.dgvStructures.Location = point2;
            this.dgvStructures.Name = "dgvStructures";
            this.dgvStructures.ReadOnly = true;
            this.dgvStructures.RowHeadersVisible = false;
            this.dgvStructures.RowTemplate.Height = 0x18;
            this.dgvStructures.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            size2 = new Size(0x18c, 0x147);
            this.dgvStructures.Size = size2;
            this.dgvStructures.TabIndex = 1;
            this.TabPage3.Controls.Add(this.dgvDroids);
            point2 = new Point(4, 0x1c);
            this.TabPage3.Location = point2;
            this.TabPage3.Name = "TabPage3";
            size2 = new Size(0x18c, 0x147);
            this.TabPage3.Size = size2;
            this.TabPage3.TabIndex = 2;
            this.TabPage3.Text = "Droids";
            this.TabPage3.UseVisualStyleBackColor = true;
            this.dgvDroids.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            this.dgvDroids.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDroids.Dock = DockStyle.Fill;
            point2 = new Point(0, 0);
            this.dgvDroids.Location = point2;
            this.dgvDroids.Name = "dgvDroids";
            this.dgvDroids.ReadOnly = true;
            this.dgvDroids.RowHeadersVisible = false;
            this.dgvDroids.RowTemplate.Height = 0x18;
            this.dgvDroids.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            size2 = new Size(0x18c, 0x147);
            this.dgvDroids.Size = size2;
            this.dgvDroids.TabIndex = 1;
            this.tpObject.Controls.Add(this.TableLayoutPanel8);
            point2 = new Point(4, 0x33);
            this.tpObject.Location = point2;
            padding2 = new Padding(4);
            this.tpObject.Margin = padding2;
            this.tpObject.Name = "tpObject";
            size2 = new Size(410, 0x22d);
            this.tpObject.Size = size2;
            this.tpObject.TabIndex = 6;
            this.tpObject.Text = "Object";
            this.tpObject.UseVisualStyleBackColor = true;
            this.TableLayoutPanel8.ColumnCount = 1;
            this.TableLayoutPanel8.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            this.TableLayoutPanel8.Controls.Add(this.TableLayoutPanel9, 0, 1);
            this.TableLayoutPanel8.Controls.Add(this.Panel14, 0, 0);
            this.TableLayoutPanel8.Dock = DockStyle.Fill;
            point2 = new Point(0, 0);
            this.TableLayoutPanel8.Location = point2;
            this.TableLayoutPanel8.Name = "TableLayoutPanel8";
            this.TableLayoutPanel8.RowCount = 3;
            this.TableLayoutPanel8.RowStyles.Add(new RowStyle(SizeType.Absolute, 350f));
            this.TableLayoutPanel8.RowStyles.Add(new RowStyle(SizeType.Absolute, 192f));
            this.TableLayoutPanel8.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            size2 = new Size(410, 0x22d);
            this.TableLayoutPanel8.Size = size2;
            this.TableLayoutPanel8.TabIndex = 0x37;
            this.TableLayoutPanel9.ColumnCount = 2;
            this.TableLayoutPanel9.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 96f));
            this.TableLayoutPanel9.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
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
            this.TableLayoutPanel9.Dock = DockStyle.Fill;
            point2 = new Point(3, 0x161);
            this.TableLayoutPanel9.Location = point2;
            this.TableLayoutPanel9.Name = "TableLayoutPanel9";
            this.TableLayoutPanel9.RowCount = 6;
            this.TableLayoutPanel9.RowStyles.Add(new RowStyle(SizeType.Percent, 16.66417f));
            this.TableLayoutPanel9.RowStyles.Add(new RowStyle(SizeType.Percent, 16.66717f));
            this.TableLayoutPanel9.RowStyles.Add(new RowStyle(SizeType.Percent, 16.66717f));
            this.TableLayoutPanel9.RowStyles.Add(new RowStyle(SizeType.Percent, 16.66717f));
            this.TableLayoutPanel9.RowStyles.Add(new RowStyle(SizeType.Percent, 16.66717f));
            this.TableLayoutPanel9.RowStyles.Add(new RowStyle(SizeType.Percent, 16.66717f));
            size2 = new Size(0x194, 0xba);
            this.TableLayoutPanel9.Size = size2;
            this.TableLayoutPanel9.TabIndex = 0;
            this.cboDroidTurret3.Dock = DockStyle.Fill;
            this.cboDroidTurret3.DropDownHeight = 0x200;
            this.cboDroidTurret3.DropDownWidth = 0x180;
            this.cboDroidTurret3.FormattingEnabled = true;
            this.cboDroidTurret3.IntegralHeight = false;
            point2 = new Point(100, 0x9e);
            this.cboDroidTurret3.Location = point2;
            padding2 = new Padding(4);
            this.cboDroidTurret3.Margin = padding2;
            this.cboDroidTurret3.Name = "cboDroidTurret3";
            size2 = new Size(300, 0x18);
            this.cboDroidTurret3.Size = size2;
            this.cboDroidTurret3.TabIndex = 0x30;
            this.cboDroidTurret2.Dock = DockStyle.Fill;
            this.cboDroidTurret2.DropDownHeight = 0x200;
            this.cboDroidTurret2.DropDownWidth = 0x180;
            this.cboDroidTurret2.FormattingEnabled = true;
            this.cboDroidTurret2.IntegralHeight = false;
            point2 = new Point(100, 0x7f);
            this.cboDroidTurret2.Location = point2;
            padding2 = new Padding(4);
            this.cboDroidTurret2.Margin = padding2;
            this.cboDroidTurret2.Name = "cboDroidTurret2";
            size2 = new Size(300, 0x18);
            this.cboDroidTurret2.Size = size2;
            this.cboDroidTurret2.TabIndex = 0x2f;
            this.Panel13.Controls.Add(this.rdoDroidTurret3);
            this.Panel13.Dock = DockStyle.Fill;
            point2 = new Point(3, 0x9d);
            this.Panel13.Location = point2;
            this.Panel13.Name = "Panel13";
            size2 = new Size(90, 0x1a);
            this.Panel13.Size = size2;
            this.Panel13.TabIndex = 5;
            this.rdoDroidTurret3.AutoSize = true;
            this.rdoDroidTurret3.CheckAlign = ContentAlignment.MiddleRight;
            point2 = new Point(0x30, 2);
            this.rdoDroidTurret3.Location = point2;
            this.rdoDroidTurret3.Name = "rdoDroidTurret3";
            size2 = new Size(0x21, 0x15);
            this.rdoDroidTurret3.Size = size2;
            this.rdoDroidTurret3.TabIndex = 0x33;
            this.rdoDroidTurret3.TabStop = true;
            this.rdoDroidTurret3.Text = "3";
            this.rdoDroidTurret3.TextAlign = ContentAlignment.MiddleRight;
            this.rdoDroidTurret3.UseCompatibleTextRendering = true;
            this.rdoDroidTurret3.UseVisualStyleBackColor = true;
            this.cboDroidTurret1.Dock = DockStyle.Fill;
            this.cboDroidTurret1.DropDownHeight = 0x200;
            this.cboDroidTurret1.DropDownWidth = 0x180;
            this.cboDroidTurret1.FormattingEnabled = true;
            this.cboDroidTurret1.IntegralHeight = false;
            point2 = new Point(100, 0x60);
            this.cboDroidTurret1.Location = point2;
            padding2 = new Padding(4);
            this.cboDroidTurret1.Margin = padding2;
            this.cboDroidTurret1.Name = "cboDroidTurret1";
            size2 = new Size(300, 0x18);
            this.cboDroidTurret1.Size = size2;
            this.cboDroidTurret1.TabIndex = 0x2d;
            this.cboDroidPropulsion.Dock = DockStyle.Fill;
            this.cboDroidPropulsion.DropDownHeight = 0x200;
            this.cboDroidPropulsion.DropDownWidth = 0x180;
            this.cboDroidPropulsion.FormattingEnabled = true;
            this.cboDroidPropulsion.IntegralHeight = false;
            point2 = new Point(100, 0x41);
            this.cboDroidPropulsion.Location = point2;
            padding2 = new Padding(4);
            this.cboDroidPropulsion.Margin = padding2;
            this.cboDroidPropulsion.Name = "cboDroidPropulsion";
            size2 = new Size(300, 0x18);
            this.cboDroidPropulsion.Size = size2;
            this.cboDroidPropulsion.TabIndex = 0x2b;
            this.cboDroidBody.Dock = DockStyle.Fill;
            this.cboDroidBody.DropDownHeight = 0x200;
            this.cboDroidBody.DropDownWidth = 0x180;
            this.cboDroidBody.FormattingEnabled = true;
            this.cboDroidBody.IntegralHeight = false;
            point2 = new Point(100, 0x22);
            this.cboDroidBody.Location = point2;
            padding2 = new Padding(4);
            this.cboDroidBody.Margin = padding2;
            this.cboDroidBody.Name = "cboDroidBody";
            size2 = new Size(300, 0x18);
            this.cboDroidBody.Size = size2;
            this.cboDroidBody.TabIndex = 0x29;
            this.cboDroidType.Dock = DockStyle.Fill;
            this.cboDroidType.DropDownHeight = 0x200;
            this.cboDroidType.DropDownWidth = 0x180;
            this.cboDroidType.FormattingEnabled = true;
            this.cboDroidType.IntegralHeight = false;
            point2 = new Point(100, 4);
            this.cboDroidType.Location = point2;
            padding2 = new Padding(4);
            this.cboDroidType.Margin = padding2;
            this.cboDroidType.Name = "cboDroidType";
            size2 = new Size(300, 0x18);
            this.cboDroidType.Size = size2;
            this.cboDroidType.TabIndex = 0x34;
            this.Panel12.Controls.Add(this.rdoDroidTurret0);
            this.Panel12.Controls.Add(this.rdoDroidTurret2);
            this.Panel12.Dock = DockStyle.Fill;
            point2 = new Point(3, 0x7e);
            this.Panel12.Location = point2;
            this.Panel12.Name = "Panel12";
            size2 = new Size(90, 0x19);
            this.Panel12.Size = size2;
            this.Panel12.TabIndex = 4;
            this.rdoDroidTurret0.AutoSize = true;
            this.rdoDroidTurret0.CheckAlign = ContentAlignment.MiddleRight;
            point2 = new Point(7, 1);
            this.rdoDroidTurret0.Location = point2;
            this.rdoDroidTurret0.Name = "rdoDroidTurret0";
            size2 = new Size(0x21, 0x15);
            this.rdoDroidTurret0.Size = size2;
            this.rdoDroidTurret0.TabIndex = 0x36;
            this.rdoDroidTurret0.TabStop = true;
            this.rdoDroidTurret0.Text = "0";
            this.rdoDroidTurret0.TextAlign = ContentAlignment.MiddleRight;
            this.rdoDroidTurret0.UseCompatibleTextRendering = true;
            this.rdoDroidTurret0.UseVisualStyleBackColor = true;
            this.rdoDroidTurret2.AutoSize = true;
            this.rdoDroidTurret2.CheckAlign = ContentAlignment.MiddleRight;
            point2 = new Point(0x30, 1);
            this.rdoDroidTurret2.Location = point2;
            this.rdoDroidTurret2.Name = "rdoDroidTurret2";
            size2 = new Size(0x21, 0x15);
            this.rdoDroidTurret2.Size = size2;
            this.rdoDroidTurret2.TabIndex = 50;
            this.rdoDroidTurret2.TabStop = true;
            this.rdoDroidTurret2.Text = "2";
            this.rdoDroidTurret2.TextAlign = ContentAlignment.MiddleRight;
            this.rdoDroidTurret2.UseCompatibleTextRendering = true;
            this.rdoDroidTurret2.UseVisualStyleBackColor = true;
            this.Panel11.Controls.Add(this.Label39);
            this.Panel11.Controls.Add(this.rdoDroidTurret1);
            this.Panel11.Dock = DockStyle.Fill;
            point2 = new Point(3, 0x5f);
            this.Panel11.Location = point2;
            this.Panel11.Name = "Panel11";
            size2 = new Size(90, 0x19);
            this.Panel11.Size = size2;
            this.Panel11.TabIndex = 3;
            point2 = new Point(0, 0);
            this.Label39.Location = point2;
            padding2 = new Padding(4, 0, 4, 0);
            this.Label39.Margin = padding2;
            this.Label39.Name = "Label39";
            size2 = new Size(0x31, 0x19);
            this.Label39.Size = size2;
            this.Label39.TabIndex = 0x2e;
            this.Label39.Text = "Turrets";
            this.Label39.TextAlign = ContentAlignment.MiddleRight;
            this.Label39.UseCompatibleTextRendering = true;
            this.rdoDroidTurret1.AutoSize = true;
            this.rdoDroidTurret1.CheckAlign = ContentAlignment.MiddleRight;
            point2 = new Point(0x31, 1);
            this.rdoDroidTurret1.Location = point2;
            this.rdoDroidTurret1.Name = "rdoDroidTurret1";
            size2 = new Size(0x21, 0x15);
            this.rdoDroidTurret1.Size = size2;
            this.rdoDroidTurret1.TabIndex = 0x31;
            this.rdoDroidTurret1.TabStop = true;
            this.rdoDroidTurret1.Text = "1";
            this.rdoDroidTurret1.TextAlign = ContentAlignment.MiddleRight;
            this.rdoDroidTurret1.UseCompatibleTextRendering = true;
            this.rdoDroidTurret1.UseVisualStyleBackColor = true;
            this.Panel10.Controls.Add(this.Label38);
            this.Panel10.Dock = DockStyle.Fill;
            point2 = new Point(3, 0x40);
            this.Panel10.Location = point2;
            this.Panel10.Name = "Panel10";
            size2 = new Size(90, 0x19);
            this.Panel10.Size = size2;
            this.Panel10.TabIndex = 2;
            point2 = new Point(7, 1);
            this.Label38.Location = point2;
            padding2 = new Padding(4, 0, 4, 0);
            this.Label38.Margin = padding2;
            this.Label38.Name = "Label38";
            size2 = new Size(0x4b, 0x19);
            this.Label38.Size = size2;
            this.Label38.TabIndex = 0x2c;
            this.Label38.Text = "Propulsion";
            this.Label38.TextAlign = ContentAlignment.MiddleRight;
            this.Label38.UseCompatibleTextRendering = true;
            this.Panel9.Controls.Add(this.Label37);
            this.Panel9.Dock = DockStyle.Fill;
            point2 = new Point(3, 0x21);
            this.Panel9.Location = point2;
            this.Panel9.Name = "Panel9";
            size2 = new Size(90, 0x19);
            this.Panel9.Size = size2;
            this.Panel9.TabIndex = 1;
            point2 = new Point(8, 1);
            this.Label37.Location = point2;
            padding2 = new Padding(4, 0, 4, 0);
            this.Label37.Margin = padding2;
            this.Label37.Name = "Label37";
            size2 = new Size(0x4b, 0x19);
            this.Label37.Size = size2;
            this.Label37.TabIndex = 0x2a;
            this.Label37.Text = "Body";
            this.Label37.TextAlign = ContentAlignment.MiddleRight;
            this.Label37.UseCompatibleTextRendering = true;
            this.Panel8.Controls.Add(this.Label40);
            this.Panel8.Dock = DockStyle.Fill;
            point2 = new Point(3, 3);
            this.Panel8.Location = point2;
            this.Panel8.Name = "Panel8";
            size2 = new Size(90, 0x18);
            this.Panel8.Size = size2;
            this.Panel8.TabIndex = 0;
            point2 = new Point(8, 0);
            this.Label40.Location = point2;
            padding2 = new Padding(4, 0, 4, 0);
            this.Label40.Margin = padding2;
            this.Label40.Name = "Label40";
            size2 = new Size(0x4b, 0x19);
            this.Label40.Size = size2;
            this.Label40.TabIndex = 0x35;
            this.Label40.Text = "Type";
            this.Label40.TextAlign = ContentAlignment.MiddleRight;
            this.Label40.UseCompatibleTextRendering = true;
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
            this.Panel14.Dock = DockStyle.Fill;
            point2 = new Point(3, 3);
            this.Panel14.Location = point2;
            this.Panel14.Name = "Panel14";
            size2 = new Size(0x194, 0x158);
            this.Panel14.Size = size2;
            this.Panel14.TabIndex = 1;
            point2 = new Point(0xee, 0x93);
            this.btnFlatSelected.Location = point2;
            padding2 = new Padding(4);
            this.btnFlatSelected.Margin = padding2;
            this.btnFlatSelected.Name = "btnFlatSelected";
            size2 = new Size(120, 30);
            this.btnFlatSelected.Size = size2;
            this.btnFlatSelected.TabIndex = 7;
            this.btnFlatSelected.Text = "Flatten Terrain";
            this.btnFlatSelected.UseCompatibleTextRendering = true;
            this.btnFlatSelected.UseVisualStyleBackColor = true;
            point2 = new Point(0xee, 0x75);
            this.btnAlignObjects.Location = point2;
            padding2 = new Padding(4);
            this.btnAlignObjects.Margin = padding2;
            this.btnAlignObjects.Name = "btnAlignObjects";
            size2 = new Size(120, 30);
            this.btnAlignObjects.Size = size2;
            this.btnAlignObjects.TabIndex = 6;
            this.btnAlignObjects.Text = "Realign";
            this.btnAlignObjects.UseCompatibleTextRendering = true;
            this.btnAlignObjects.UseVisualStyleBackColor = true;
            point2 = new Point(0xee, 0xb3);
            this.Label31.Location = point2;
            padding2 = new Padding(4, 0, 4, 0);
            this.Label31.Margin = padding2;
            this.Label31.Name = "Label31";
            size2 = new Size(0x53, 0x1b);
            this.Label31.Size = size2;
            this.Label31.TabIndex = 0x2e;
            this.Label31.Text = "3.1+ only";
            this.Label31.TextAlign = ContentAlignment.MiddleLeft;
            this.Label31.UseCompatibleTextRendering = true;
            point2 = new Point(0x99, 0xd5);
            this.Label30.Location = point2;
            padding2 = new Padding(4, 0, 4, 0);
            this.Label30.Margin = padding2;
            this.Label30.Name = "Label30";
            size2 = new Size(0x58, 0x1b);
            this.Label30.Size = size2;
            this.Label30.TabIndex = 0x2d;
            this.Label30.Text = "2.3 only";
            this.Label30.TextAlign = ContentAlignment.MiddleLeft;
            this.Label30.UseCompatibleTextRendering = true;
            this.cbxDesignableOnly.AutoSize = true;
            this.cbxDesignableOnly.Checked = true;
            this.cbxDesignableOnly.CheckState = CheckState.Checked;
            point2 = new Point(0x102, 310);
            this.cbxDesignableOnly.Location = point2;
            this.cbxDesignableOnly.Name = "cbxDesignableOnly";
            size2 = new Size(0x84, 0x15);
            this.cbxDesignableOnly.Size = size2;
            this.cbxDesignableOnly.TabIndex = 0x2c;
            this.cbxDesignableOnly.Text = "Designables Only";
            this.cbxDesignableOnly.UseCompatibleTextRendering = true;
            this.cbxDesignableOnly.UseVisualStyleBackColor = true;
            point2 = new Point(0x15, 180);
            this.Label17.Location = point2;
            padding2 = new Padding(4, 0, 4, 0);
            this.Label17.Margin = padding2;
            this.Label17.Name = "Label17";
            size2 = new Size(0x3e, 0x19);
            this.Label17.Size = size2;
            this.Label17.TabIndex = 0x2b;
            this.Label17.Text = "Label:";
            this.Label17.TextAlign = ContentAlignment.MiddleRight;
            this.Label17.UseCompatibleTextRendering = true;
            point2 = new Point(0x5b, 0xb5);
            this.txtObjectLabel.Location = point2;
            padding2 = new Padding(4);
            this.txtObjectLabel.Margin = padding2;
            this.txtObjectLabel.Name = "txtObjectLabel";
            size2 = new Size(0x89, 0x16);
            this.txtObjectLabel.Size = size2;
            this.txtObjectLabel.TabIndex = 0x2a;
            point2 = new Point(0x99, 0xf3);
            this.Label35.Location = point2;
            padding2 = new Padding(4, 0, 4, 0);
            this.Label35.Margin = padding2;
            this.Label35.Name = "Label35";
            size2 = new Size(0x58, 0x1b);
            this.Label35.Size = size2;
            this.Label35.TabIndex = 0x29;
            this.Label35.Text = "3.1+ only";
            this.Label35.TextAlign = ContentAlignment.MiddleLeft;
            this.Label35.UseCompatibleTextRendering = true;
            point2 = new Point(0x15, 300);
            this.btnDroidToDesign.Location = point2;
            this.btnDroidToDesign.Name = "btnDroidToDesign";
            size2 = new Size(0xe7, 0x1f);
            this.btnDroidToDesign.Size = size2;
            this.btnDroidToDesign.TabIndex = 40;
            this.btnDroidToDesign.Text = "Convert Templates To Design";
            this.btnDroidToDesign.UseCompatibleTextRendering = true;
            this.btnDroidToDesign.UseVisualStyleBackColor = true;
            point2 = new Point(4, 9);
            this.Label24.Location = point2;
            padding2 = new Padding(4, 0, 4, 0);
            this.Label24.Margin = padding2;
            this.Label24.Name = "Label24";
            size2 = new Size(0x6b, 20);
            this.Label24.Size = size2;
            this.Label24.TabIndex = 0x12;
            this.Label24.Text = "Type:";
            this.Label24.TextAlign = ContentAlignment.MiddleLeft;
            this.Label24.UseCompatibleTextRendering = true;
            point2 = new Point(4, 0x1d);
            this.lblObjectType.Location = point2;
            padding2 = new Padding(4, 0, 4, 0);
            this.lblObjectType.Margin = padding2;
            this.lblObjectType.Name = "lblObjectType";
            size2 = new Size(0x130, 0x1a);
            this.lblObjectType.Size = size2;
            this.lblObjectType.TabIndex = 20;
            this.lblObjectType.Text = "Object Type";
            this.lblObjectType.TextAlign = ContentAlignment.MiddleLeft;
            this.lblObjectType.UseCompatibleTextRendering = true;
            point2 = new Point(0x15, 270);
            this.Label36.Location = point2;
            padding2 = new Padding(4, 0, 4, 0);
            this.Label36.Margin = padding2;
            this.Label36.Name = "Label36";
            size2 = new Size(300, 0x1b);
            this.Label36.Size = size2;
            this.Label36.TabIndex = 0x27;
            this.Label36.Text = "Designed droids will only exist in 3.1+";
            this.Label36.TextAlign = ContentAlignment.MiddleLeft;
            this.Label36.UseCompatibleTextRendering = true;
            point2 = new Point(8, 120);
            this.Label23.Location = point2;
            padding2 = new Padding(4, 0, 4, 0);
            this.Label23.Margin = padding2;
            this.Label23.Name = "Label23";
            size2 = new Size(0x4b, 0x19);
            this.Label23.Size = size2;
            this.Label23.TabIndex = 0x15;
            this.Label23.Text = "Rotation:";
            this.Label23.TextAlign = ContentAlignment.MiddleRight;
            this.Label23.UseCompatibleTextRendering = true;
            point2 = new Point(0x5b, 0xf5);
            this.txtObjectHealth.Location = point2;
            padding2 = new Padding(4);
            this.txtObjectHealth.Margin = padding2;
            this.txtObjectHealth.Name = "txtObjectHealth";
            size2 = new Size(0x36, 0x16);
            this.txtObjectHealth.Size = size2;
            this.txtObjectHealth.TabIndex = 0x25;
            this.txtObjectHealth.Text = "#";
            this.txtObjectHealth.TextAlign = HorizontalAlignment.Right;
            point2 = new Point(0x5b, 0x79);
            this.txtObjectRotation.Location = point2;
            padding2 = new Padding(4);
            this.txtObjectRotation.Margin = padding2;
            this.txtObjectRotation.Name = "txtObjectRotation";
            size2 = new Size(0x29, 0x16);
            this.txtObjectRotation.Size = size2;
            this.txtObjectRotation.TabIndex = 0x19;
            this.txtObjectRotation.Text = "#";
            this.txtObjectRotation.TextAlign = HorizontalAlignment.Right;
            point2 = new Point(20, 0xf4);
            this.Label34.Location = point2;
            padding2 = new Padding(4, 0, 4, 0);
            this.Label34.Margin = padding2;
            this.Label34.Name = "Label34";
            size2 = new Size(0x3f, 0x19);
            this.Label34.Size = size2;
            this.Label34.TabIndex = 0x24;
            this.Label34.Text = "Health %";
            this.Label34.TextAlign = ContentAlignment.MiddleRight;
            this.Label34.UseCompatibleTextRendering = true;
            point2 = new Point(4, 0x3a);
            this.Label28.Location = point2;
            padding2 = new Padding(4, 0, 4, 0);
            this.Label28.Margin = padding2;
            this.Label28.Name = "Label28";
            size2 = new Size(0x35, 0x19);
            this.Label28.Size = size2;
            this.Label28.TabIndex = 0x1c;
            this.Label28.Text = "Player:";
            this.Label28.TextAlign = ContentAlignment.MiddleLeft;
            this.Label28.UseCompatibleTextRendering = true;
            point2 = new Point(0x5b, 0xd7);
            this.txtObjectPriority.Location = point2;
            padding2 = new Padding(4);
            this.txtObjectPriority.Margin = padding2;
            this.txtObjectPriority.Name = "txtObjectPriority";
            size2 = new Size(0x36, 0x16);
            this.txtObjectPriority.Size = size2;
            this.txtObjectPriority.TabIndex = 0x23;
            this.txtObjectPriority.Text = "#";
            this.txtObjectPriority.TextAlign = HorizontalAlignment.Right;
            point2 = new Point(0x2f, 150);
            this.Label25.Location = point2;
            padding2 = new Padding(4, 0, 4, 0);
            this.Label25.Margin = padding2;
            this.Label25.Name = "Label25";
            size2 = new Size(0x24, 0x19);
            this.Label25.Size = size2;
            this.Label25.TabIndex = 30;
            this.Label25.Text = "ID:";
            this.Label25.TextAlign = ContentAlignment.MiddleRight;
            this.Label25.UseCompatibleTextRendering = true;
            point2 = new Point(20, 0xd6);
            this.Label33.Location = point2;
            padding2 = new Padding(4, 0, 4, 0);
            this.Label33.Margin = padding2;
            this.Label33.Name = "Label33";
            size2 = new Size(0x3f, 0x19);
            this.Label33.Size = size2;
            this.Label33.TabIndex = 0x22;
            this.Label33.Text = "Priority:";
            this.Label33.TextAlign = ContentAlignment.MiddleRight;
            this.Label33.UseCompatibleTextRendering = true;
            point2 = new Point(0x5b, 0x97);
            this.txtObjectID.Location = point2;
            padding2 = new Padding(4);
            this.txtObjectID.Margin = padding2;
            this.txtObjectID.Name = "txtObjectID";
            size2 = new Size(80, 0x16);
            this.txtObjectID.Size = size2;
            this.txtObjectID.TabIndex = 0x1f;
            this.txtObjectID.Text = "#";
            this.txtObjectID.TextAlign = HorizontalAlignment.Right;
            point2 = new Point(140, 120);
            this.Label26.Location = point2;
            padding2 = new Padding(4, 0, 4, 0);
            this.Label26.Margin = padding2;
            this.Label26.Name = "Label26";
            size2 = new Size(0x58, 0x19);
            this.Label26.Size = size2;
            this.Label26.TabIndex = 0x21;
            this.Label26.Text = "(0-359)";
            this.Label26.TextAlign = ContentAlignment.MiddleLeft;
            this.Label26.UseCompatibleTextRendering = true;
            this.tpLabels.Controls.Add(this.Label11);
            this.tpLabels.Controls.Add(this.Label43);
            this.tpLabels.Controls.Add(this.Label42);
            this.tpLabels.Controls.Add(this.lstScriptAreas);
            this.tpLabels.Controls.Add(this.lstScriptPositions);
            this.tpLabels.Controls.Add(this.btnScriptAreaCreate);
            this.tpLabels.Controls.Add(this.GroupBox1);
            point2 = new Point(4, 0x33);
            this.tpLabels.Location = point2;
            this.tpLabels.Name = "tpLabels";
            size2 = new Size(410, 0x22d);
            this.tpLabels.Size = size2;
            this.tpLabels.TabIndex = 7;
            this.tpLabels.Text = "Labels";
            this.tpLabels.UseVisualStyleBackColor = true;
            point2 = new Point(0xee, 11);
            this.Label11.Location = point2;
            padding2 = new Padding(4, 0, 4, 0);
            this.Label11.Margin = padding2;
            this.Label11.Name = "Label11";
            size2 = new Size(0x8e, 0x37);
            this.Label11.Size = size2;
            this.Label11.TabIndex = 0x35;
            this.Label11.Text = "Hold P and click to make positions.";
            this.Label11.UseCompatibleTextRendering = true;
            this.Label43.AutoSize = true;
            point2 = new Point(0xcb, 0x42);
            this.Label43.Location = point2;
            padding2 = new Padding(4, 0, 4, 0);
            this.Label43.Margin = padding2;
            this.Label43.Name = "Label43";
            size2 = new Size(0x2c, 20);
            this.Label43.Size = size2;
            this.Label43.TabIndex = 0x34;
            this.Label43.Text = "Areas:";
            this.Label43.UseCompatibleTextRendering = true;
            this.Label42.AutoSize = true;
            point2 = new Point(0x12, 0x42);
            this.Label42.Location = point2;
            padding2 = new Padding(4, 0, 4, 0);
            this.Label42.Margin = padding2;
            this.Label42.Name = "Label42";
            size2 = new Size(0x3f, 20);
            this.Label42.Size = size2;
            this.Label42.TabIndex = 0x33;
            this.Label42.Text = "Positions:";
            this.Label42.UseCompatibleTextRendering = true;
            this.lstScriptAreas.FormattingEnabled = true;
            this.lstScriptAreas.ItemHeight = 0x10;
            point2 = new Point(0xca, 0x59);
            this.lstScriptAreas.Location = point2;
            this.lstScriptAreas.Name = "lstScriptAreas";
            size2 = new Size(0xb2, 0x94);
            this.lstScriptAreas.Size = size2;
            this.lstScriptAreas.TabIndex = 0x2f;
            this.lstScriptPositions.FormattingEnabled = true;
            this.lstScriptPositions.ItemHeight = 0x10;
            point2 = new Point(0x12, 0x59);
            this.lstScriptPositions.Location = point2;
            this.lstScriptPositions.Name = "lstScriptPositions";
            size2 = new Size(0xb2, 0x94);
            this.lstScriptPositions.Size = size2;
            this.lstScriptPositions.TabIndex = 0x2e;
            point2 = new Point(0x12, 20);
            this.btnScriptAreaCreate.Location = point2;
            this.btnScriptAreaCreate.Name = "btnScriptAreaCreate";
            size2 = new Size(0xc9, 0x1f);
            this.btnScriptAreaCreate.Size = size2;
            this.btnScriptAreaCreate.TabIndex = 0x2d;
            this.btnScriptAreaCreate.Text = "Create Area From Selection";
            this.btnScriptAreaCreate.UseCompatibleTextRendering = true;
            this.btnScriptAreaCreate.UseVisualStyleBackColor = true;
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
            point2 = new Point(0x12, 0xf3);
            this.GroupBox1.Location = point2;
            this.GroupBox1.Name = "GroupBox1";
            size2 = new Size(0xb2, 0xe7);
            this.GroupBox1.Size = size2;
            this.GroupBox1.TabIndex = 0x2c;
            this.GroupBox1.TabStop = false;
            this.GroupBox1.Text = "Selected Marker";
            this.GroupBox1.UseCompatibleTextRendering = true;
            point2 = new Point(0x38, 0x20);
            this.txtScriptMarkerLabel.Location = point2;
            padding2 = new Padding(4);
            this.txtScriptMarkerLabel.Margin = padding2;
            this.txtScriptMarkerLabel.Name = "txtScriptMarkerLabel";
            size2 = new Size(0x60, 0x16);
            this.txtScriptMarkerLabel.Size = size2;
            this.txtScriptMarkerLabel.TabIndex = 50;
            this.Label41.AutoSize = true;
            point2 = new Point(7, 0x23);
            this.Label41.Location = point2;
            padding2 = new Padding(4, 0, 4, 0);
            this.Label41.Margin = padding2;
            this.Label41.Name = "Label41";
            size2 = new Size(0x29, 20);
            this.Label41.Size = size2;
            this.Label41.TabIndex = 0x31;
            this.Label41.Text = "Label:";
            this.Label41.UseCompatibleTextRendering = true;
            point2 = new Point(10, 0xb9);
            this.btnScriptMarkerRemove.Location = point2;
            this.btnScriptMarkerRemove.Name = "btnScriptMarkerRemove";
            size2 = new Size(0x8e, 0x1f);
            this.btnScriptMarkerRemove.Size = size2;
            this.btnScriptMarkerRemove.TabIndex = 0x30;
            this.btnScriptMarkerRemove.Text = "Remove";
            this.btnScriptMarkerRemove.UseCompatibleTextRendering = true;
            this.btnScriptMarkerRemove.UseVisualStyleBackColor = true;
            this.Label2.AutoSize = true;
            point2 = new Point(0x16, 0x42);
            this.Label2.Location = point2;
            padding2 = new Padding(4, 0, 4, 0);
            this.Label2.Margin = padding2;
            this.Label2.Name = "Label2";
            size2 = new Size(15, 20);
            this.Label2.Size = size2;
            this.Label2.TabIndex = 0x21;
            this.Label2.Text = "x:";
            this.Label2.UseCompatibleTextRendering = true;
            point2 = new Point(0x5b, 0x57);
            this.txtScriptMarkerY.Location = point2;
            padding2 = new Padding(4);
            this.txtScriptMarkerY.Margin = padding2;
            this.txtScriptMarkerY.Name = "txtScriptMarkerY";
            size2 = new Size(0x3d, 0x16);
            this.txtScriptMarkerY.Size = size2;
            this.txtScriptMarkerY.TabIndex = 0x22;
            point2 = new Point(0x16, 0x57);
            this.txtScriptMarkerX.Location = point2;
            padding2 = new Padding(4);
            this.txtScriptMarkerX.Margin = padding2;
            this.txtScriptMarkerX.Name = "txtScriptMarkerX";
            size2 = new Size(0x3d, 0x16);
            this.txtScriptMarkerX.Size = size2;
            this.txtScriptMarkerX.TabIndex = 0x20;
            this.Label7.AutoSize = true;
            point2 = new Point(0x5b, 0x42);
            this.Label7.Location = point2;
            padding2 = new Padding(4, 0, 4, 0);
            this.Label7.Margin = padding2;
            this.Label7.Name = "Label7";
            size2 = new Size(15, 20);
            this.Label7.Size = size2;
            this.Label7.TabIndex = 0x23;
            this.Label7.Text = "y:";
            this.Label7.UseCompatibleTextRendering = true;
            point2 = new Point(0x5b, 0x8f);
            this.txtScriptMarkerY2.Location = point2;
            padding2 = new Padding(4);
            this.txtScriptMarkerY2.Margin = padding2;
            this.txtScriptMarkerY2.Name = "txtScriptMarkerY2";
            size2 = new Size(0x3d, 0x16);
            this.txtScriptMarkerY2.Size = size2;
            this.txtScriptMarkerY2.TabIndex = 0x26;
            this.Label4.AutoSize = true;
            point2 = new Point(0x5b, 0x7a);
            this.Label4.Location = point2;
            padding2 = new Padding(4, 0, 4, 0);
            this.Label4.Margin = padding2;
            this.Label4.Name = "Label4";
            size2 = new Size(0x17, 20);
            this.Label4.Size = size2;
            this.Label4.TabIndex = 0x27;
            this.Label4.Text = "y2:";
            this.Label4.UseCompatibleTextRendering = true;
            this.Label8.AutoSize = true;
            point2 = new Point(0x16, 0x7a);
            this.Label8.Location = point2;
            padding2 = new Padding(4, 0, 4, 0);
            this.Label8.Margin = padding2;
            this.Label8.Name = "Label8";
            size2 = new Size(0x17, 20);
            this.Label8.Size = size2;
            this.Label8.TabIndex = 0x25;
            this.Label8.Text = "x2:";
            this.Label8.UseCompatibleTextRendering = true;
            point2 = new Point(0x16, 0x8f);
            this.txtScriptMarkerX2.Location = point2;
            padding2 = new Padding(4);
            this.txtScriptMarkerX2.Margin = padding2;
            this.txtScriptMarkerX2.Name = "txtScriptMarkerX2";
            size2 = new Size(0x3d, 0x16);
            this.txtScriptMarkerX2.Size = size2;
            this.txtScriptMarkerX2.TabIndex = 0x24;
            this.TableLayoutPanel7.ColumnCount = 1;
            this.TableLayoutPanel7.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            this.TableLayoutPanel7.Controls.Add(this.Panel7, 0, 0);
            this.TableLayoutPanel7.Controls.Add(this.pnlView, 0, 1);
            this.TableLayoutPanel7.Dock = DockStyle.Fill;
            point2 = new Point(0, 0);
            this.TableLayoutPanel7.Location = point2;
            padding2 = new Padding(4);
            this.TableLayoutPanel7.Margin = padding2;
            this.TableLayoutPanel7.Name = "TableLayoutPanel7";
            this.TableLayoutPanel7.RowCount = 2;
            this.TableLayoutPanel7.RowStyles.Add(new RowStyle(SizeType.Absolute, 30f));
            this.TableLayoutPanel7.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            size2 = new Size(0x35a, 0x264);
            this.TableLayoutPanel7.Size = size2;
            this.TableLayoutPanel7.TabIndex = 2;
            this.Panel7.Controls.Add(this.tsTools);
            this.Panel7.Controls.Add(this.tsFile);
            this.Panel7.Controls.Add(this.tsSelection);
            this.Panel7.Controls.Add(this.tsMinimap);
            this.Panel7.Dock = DockStyle.Fill;
            point2 = new Point(0, 0);
            this.Panel7.Location = point2;
            padding2 = new Padding(0);
            this.Panel7.Margin = padding2;
            this.Panel7.Name = "Panel7";
            size2 = new Size(0x35a, 30);
            this.Panel7.Size = size2;
            this.Panel7.TabIndex = 0;
            this.tsTools.Dock = DockStyle.None;
            this.tsTools.GripStyle = ToolStripGripStyle.Hidden;
            this.tsTools.Items.AddRange(new ToolStripItem[] { this.tsbGateways, this.tsbDrawAutotexture, this.tsbDrawTileOrientation });
            point2 = new Point(0x174, 2);
            this.tsTools.Location = point2;
            this.tsTools.Name = "tsTools";
            this.tsTools.RenderMode = ToolStripRenderMode.System;
            size2 = new Size(0x48, 0x19);
            this.tsTools.Size = size2;
            this.tsTools.TabIndex = 2;
            this.tsbGateways.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.tsbGateways.ImageTransparentColor = Color.Magenta;
            this.tsbGateways.Name = "tsbGateways";
            size2 = new Size(0x17, 0x16);
            this.tsbGateways.Size = size2;
            this.tsbGateways.Text = "Gateways";
            this.tsbDrawAutotexture.CheckOnClick = true;
            this.tsbDrawAutotexture.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.tsbDrawAutotexture.ImageTransparentColor = Color.Magenta;
            this.tsbDrawAutotexture.Name = "tsbDrawAutotexture";
            size2 = new Size(0x17, 0x16);
            this.tsbDrawAutotexture.Size = size2;
            this.tsbDrawAutotexture.Text = "Display Painted Texture Markers";
            this.tsbDrawTileOrientation.CheckOnClick = true;
            this.tsbDrawTileOrientation.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.tsbDrawTileOrientation.ImageTransparentColor = Color.Magenta;
            this.tsbDrawTileOrientation.Name = "tsbDrawTileOrientation";
            size2 = new Size(0x17, 0x16);
            this.tsbDrawTileOrientation.Size = size2;
            this.tsbDrawTileOrientation.Text = "Display Texture Orientations";
            this.tsFile.Dock = DockStyle.None;
            this.tsFile.GripStyle = ToolStripGripStyle.Hidden;
            this.tsFile.Items.AddRange(new ToolStripItem[] { this.tsbSave });
            point2 = new Point(0x1c5, 2);
            this.tsFile.Location = point2;
            this.tsFile.Name = "tsFile";
            this.tsFile.RenderMode = ToolStripRenderMode.System;
            size2 = new Size(0x1a, 0x19);
            this.tsFile.Size = size2;
            this.tsFile.TabIndex = 3;
            this.tsbSave.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.tsbSave.Enabled = false;
            this.tsbSave.ImageTransparentColor = Color.Magenta;
            this.tsbSave.Name = "tsbSave";
            size2 = new Size(0x17, 0x16);
            this.tsbSave.Size = size2;
            this.tsSelection.Dock = DockStyle.None;
            this.tsSelection.GripStyle = ToolStripGripStyle.Hidden;
            this.tsSelection.Items.AddRange(new ToolStripItem[] { this.ToolStripLabel1, this.tsbSelection, this.tsbSelectionCopy, this.tsbSelectionPasteOptions, this.tsbSelectionPaste, this.tsbSelectionRotateCounterClockwise, this.tsbSelectionRotateClockwise, this.tsbSelectionFlipX, this.tsbSelectionObjects });
            point2 = new Point(0x62, 0);
            this.tsSelection.Location = point2;
            this.tsSelection.Name = "tsSelection";
            this.tsSelection.RenderMode = ToolStripRenderMode.System;
            size2 = new Size(250, 0x19);
            this.tsSelection.Size = size2;
            this.tsSelection.TabIndex = 0;
            this.tsSelection.Text = "ToolStrip1";
            this.ToolStripLabel1.Name = "ToolStripLabel1";
            size2 = new Size(0x49, 0x16);
            this.ToolStripLabel1.Size = size2;
            this.ToolStripLabel1.Text = "Selection:";
            this.tsbSelection.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.tsbSelection.ImageTransparentColor = Color.Magenta;
            this.tsbSelection.Name = "tsbSelection";
            size2 = new Size(0x17, 0x16);
            this.tsbSelection.Size = size2;
            this.tsbSelection.Text = "Selection Tool";
            this.tsbSelectionCopy.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.tsbSelectionCopy.ImageTransparentColor = Color.Magenta;
            this.tsbSelectionCopy.Name = "tsbSelectionCopy";
            size2 = new Size(0x17, 0x16);
            this.tsbSelectionCopy.Size = size2;
            this.tsbSelectionCopy.Text = "Copy Selection";
            this.tsbSelectionPasteOptions.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.tsbSelectionPasteOptions.DropDownItems.AddRange(new ToolStripItem[] { this.menuRotateUnits, this.menuRotateWalls, this.menuRotateNothing, this.ToolStripSeparator10, this.menuSelPasteHeights, this.menuSelPasteTextures, this.menuSelPasteUnits, this.menuSelPasteGateways, this.menuSelPasteDeleteUnits, this.menuSelPasteDeleteGateways });
            this.tsbSelectionPasteOptions.ImageTransparentColor = Color.Magenta;
            this.tsbSelectionPasteOptions.Name = "tsbSelectionPasteOptions";
            size2 = new Size(13, 0x16);
            this.tsbSelectionPasteOptions.Size = size2;
            this.tsbSelectionPasteOptions.Text = "Paste Options";
            this.menuRotateUnits.Name = "menuRotateUnits";
            size2 = new Size(0xf4, 0x18);
            this.menuRotateUnits.Size = size2;
            this.menuRotateUnits.Text = "Rotate All Objects";
            this.menuRotateWalls.Checked = true;
            this.menuRotateWalls.CheckState = CheckState.Checked;
            this.menuRotateWalls.Name = "menuRotateWalls";
            size2 = new Size(0xf4, 0x18);
            this.menuRotateWalls.Size = size2;
            this.menuRotateWalls.Text = "Rotate Walls Only";
            this.menuRotateNothing.Name = "menuRotateNothing";
            size2 = new Size(0xf4, 0x18);
            this.menuRotateNothing.Size = size2;
            this.menuRotateNothing.Text = "No Object Rotation";
            this.ToolStripSeparator10.Name = "ToolStripSeparator10";
            size2 = new Size(0xf1, 6);
            this.ToolStripSeparator10.Size = size2;
            this.menuSelPasteHeights.Checked = true;
            this.menuSelPasteHeights.CheckOnClick = true;
            this.menuSelPasteHeights.CheckState = CheckState.Checked;
            this.menuSelPasteHeights.Name = "menuSelPasteHeights";
            size2 = new Size(0xf4, 0x18);
            this.menuSelPasteHeights.Size = size2;
            this.menuSelPasteHeights.Text = "Paste Heights";
            this.menuSelPasteTextures.Checked = true;
            this.menuSelPasteTextures.CheckOnClick = true;
            this.menuSelPasteTextures.CheckState = CheckState.Checked;
            this.menuSelPasteTextures.Name = "menuSelPasteTextures";
            size2 = new Size(0xf4, 0x18);
            this.menuSelPasteTextures.Size = size2;
            this.menuSelPasteTextures.Text = "Paste Textures";
            this.menuSelPasteUnits.CheckOnClick = true;
            this.menuSelPasteUnits.Name = "menuSelPasteUnits";
            size2 = new Size(0xf4, 0x18);
            this.menuSelPasteUnits.Size = size2;
            this.menuSelPasteUnits.Text = "Paste Objects";
            this.menuSelPasteGateways.CheckOnClick = true;
            this.menuSelPasteGateways.Name = "menuSelPasteGateways";
            size2 = new Size(0xf4, 0x18);
            this.menuSelPasteGateways.Size = size2;
            this.menuSelPasteGateways.Text = "Paste Gateways";
            this.menuSelPasteDeleteUnits.CheckOnClick = true;
            this.menuSelPasteDeleteUnits.Name = "menuSelPasteDeleteUnits";
            size2 = new Size(0xf4, 0x18);
            this.menuSelPasteDeleteUnits.Size = size2;
            this.menuSelPasteDeleteUnits.Text = "Delete Existing Objects";
            this.menuSelPasteDeleteGateways.CheckOnClick = true;
            this.menuSelPasteDeleteGateways.Name = "menuSelPasteDeleteGateways";
            size2 = new Size(0xf4, 0x18);
            this.menuSelPasteDeleteGateways.Size = size2;
            this.menuSelPasteDeleteGateways.Text = "Delete Existing Gateways";
            this.tsbSelectionPaste.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.tsbSelectionPaste.ImageTransparentColor = Color.Magenta;
            this.tsbSelectionPaste.Name = "tsbSelectionPaste";
            size2 = new Size(0x17, 0x16);
            this.tsbSelectionPaste.Size = size2;
            this.tsbSelectionPaste.Text = "Paste To Selection";
            this.tsbSelectionRotateCounterClockwise.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.tsbSelectionRotateCounterClockwise.ImageTransparentColor = Color.Magenta;
            this.tsbSelectionRotateCounterClockwise.Name = "tsbSelectionRotateCounterClockwise";
            size2 = new Size(0x17, 0x16);
            this.tsbSelectionRotateCounterClockwise.Size = size2;
            this.tsbSelectionRotateCounterClockwise.Text = "Rotate Copy Counter Clockwise";
            this.tsbSelectionRotateClockwise.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.tsbSelectionRotateClockwise.ImageTransparentColor = Color.Magenta;
            this.tsbSelectionRotateClockwise.Name = "tsbSelectionRotateClockwise";
            size2 = new Size(0x17, 0x16);
            this.tsbSelectionRotateClockwise.Size = size2;
            this.tsbSelectionRotateClockwise.Text = "Rotate Copy Clockwise";
            this.tsbSelectionFlipX.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.tsbSelectionFlipX.ImageTransparentColor = Color.Magenta;
            this.tsbSelectionFlipX.Name = "tsbSelectionFlipX";
            size2 = new Size(0x17, 0x16);
            this.tsbSelectionFlipX.Size = size2;
            this.tsbSelectionFlipX.Text = "Flip Copy Horizontally";
            this.tsbSelectionObjects.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.tsbSelectionObjects.ImageTransparentColor = Color.Magenta;
            this.tsbSelectionObjects.Name = "tsbSelectionObjects";
            size2 = new Size(0x17, 0x16);
            this.tsbSelectionObjects.Size = size2;
            this.tsbSelectionObjects.Text = "Select Objects";
            this.tsMinimap.Dock = DockStyle.None;
            this.tsMinimap.GripStyle = ToolStripGripStyle.Hidden;
            this.tsMinimap.Items.AddRange(new ToolStripItem[] { this.menuMinimap });
            point2 = new Point(0, 0);
            this.tsMinimap.Location = point2;
            this.tsMinimap.Name = "tsMinimap";
            this.tsMinimap.RenderMode = ToolStripRenderMode.System;
            size2 = new Size(0x54, 0x1b);
            this.tsMinimap.Size = size2;
            this.tsMinimap.TabIndex = 1;
            this.menuMinimap.DisplayStyle = ToolStripItemDisplayStyle.Text;
            this.menuMinimap.DropDownItems.AddRange(new ToolStripItem[] { this.menuMiniShowTex, this.menuMiniShowHeight, this.menuMiniShowCliffs, this.menuMiniShowUnits, this.menuMiniShowGateways });
            this.menuMinimap.ImageTransparentColor = Color.Magenta;
            this.menuMinimap.Name = "menuMinimap";
            size2 = new Size(0x51, 0x18);
            this.menuMinimap.Size = size2;
            this.menuMinimap.Text = "Minimap";
            this.menuMiniShowTex.Checked = true;
            this.menuMiniShowTex.CheckOnClick = true;
            this.menuMiniShowTex.CheckState = CheckState.Checked;
            this.menuMiniShowTex.Name = "menuMiniShowTex";
            size2 = new Size(0xb5, 0x18);
            this.menuMiniShowTex.Size = size2;
            this.menuMiniShowTex.Text = "Show Textures";
            this.menuMiniShowHeight.Checked = true;
            this.menuMiniShowHeight.CheckOnClick = true;
            this.menuMiniShowHeight.CheckState = CheckState.Checked;
            this.menuMiniShowHeight.Name = "menuMiniShowHeight";
            size2 = new Size(0xb5, 0x18);
            this.menuMiniShowHeight.Size = size2;
            this.menuMiniShowHeight.Text = "Show Heights";
            this.menuMiniShowCliffs.CheckOnClick = true;
            this.menuMiniShowCliffs.Name = "menuMiniShowCliffs";
            size2 = new Size(0xb5, 0x18);
            this.menuMiniShowCliffs.Size = size2;
            this.menuMiniShowCliffs.Text = "Show Cliffs";
            this.menuMiniShowUnits.Checked = true;
            this.menuMiniShowUnits.CheckOnClick = true;
            this.menuMiniShowUnits.CheckState = CheckState.Checked;
            this.menuMiniShowUnits.Name = "menuMiniShowUnits";
            size2 = new Size(0xb5, 0x18);
            this.menuMiniShowUnits.Size = size2;
            this.menuMiniShowUnits.Text = "Show Objects";
            this.menuMiniShowGateways.CheckOnClick = true;
            this.menuMiniShowGateways.Name = "menuMiniShowGateways";
            size2 = new Size(0xb5, 0x18);
            this.menuMiniShowGateways.Size = size2;
            this.menuMiniShowGateways.Text = "Show Gateways";
            this.pnlView.Dock = DockStyle.Fill;
            point2 = new Point(0, 30);
            this.pnlView.Location = point2;
            padding2 = new Padding(0);
            this.pnlView.Margin = padding2;
            this.pnlView.Name = "pnlView";
            size2 = new Size(0x35a, 0x246);
            this.pnlView.Size = size2;
            this.pnlView.TabIndex = 1;
            this.menuMain.Dock = DockStyle.Fill;
            this.menuMain.Items.AddRange(new ToolStripItem[] { this.menuFile, this.menuTools, this.menuOptions });
            point2 = new Point(0, 0);
            this.menuMain.Location = point2;
            this.menuMain.Name = "menuMain";
            padding2 = new Padding(8, 2, 0, 2);
            this.menuMain.Padding = padding2;
            this.menuMain.RenderMode = ToolStripRenderMode.System;
            size2 = new Size(0x510, 0x1f);
            this.menuMain.Size = size2;
            this.menuMain.TabIndex = 0;
            this.menuMain.Text = "MenuStrip1";
            this.menuFile.DropDownItems.AddRange(new ToolStripItem[] { this.NewMapToolStripMenuItem, this.ToolStripSeparator3, this.OpenToolStripMenuItem, this.ToolStripSeparator2, this.SaveToolStripMenuItem, this.ToolStripSeparator1, this.ToolStripMenuItem4, this.ToolStripMenuItem2, this.MapWZToolStripMenuItem, this.ToolStripSeparator4, this.CloseToolStripMenuItem });
            this.menuFile.Name = "menuFile";
            size2 = new Size(0x2c, 0x1b);
            this.menuFile.Size = size2;
            this.menuFile.Text = "File";
            this.NewMapToolStripMenuItem.Name = "NewMapToolStripMenuItem";
            size2 = new Size(0xb1, 0x18);
            this.NewMapToolStripMenuItem.Size = size2;
            this.NewMapToolStripMenuItem.Text = "New Map";
            this.ToolStripSeparator3.Name = "ToolStripSeparator3";
            size2 = new Size(0xae, 6);
            this.ToolStripSeparator3.Size = size2;
            this.OpenToolStripMenuItem.Name = "OpenToolStripMenuItem";
            size2 = new Size(0xb1, 0x18);
            this.OpenToolStripMenuItem.Size = size2;
            this.OpenToolStripMenuItem.Text = "Open...";
            this.ToolStripSeparator2.Name = "ToolStripSeparator2";
            size2 = new Size(0xae, 6);
            this.ToolStripSeparator2.Size = size2;
            this.SaveToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { this.menuSaveFMap, this.ToolStripSeparator7, this.menuSaveFMapQuick, this.ToolStripSeparator11, this.menuSaveFME, this.ToolStripSeparator5, this.MapLNDToolStripMenuItem, this.ToolStripSeparator6, this.menuExportMapTileTypes, this.ToolStripMenuItem1, this.MinimapBMPToolStripMenuItem, this.ToolStripMenuItem3 });
            this.SaveToolStripMenuItem.Name = "SaveToolStripMenuItem";
            size2 = new Size(0xb1, 0x18);
            this.SaveToolStripMenuItem.Size = size2;
            this.SaveToolStripMenuItem.Text = "Save";
            this.menuSaveFMap.Name = "menuSaveFMap";
            size2 = new Size(0x11c, 0x18);
            this.menuSaveFMap.Size = size2;
            this.menuSaveFMap.Text = "Map fmap...";
            this.ToolStripSeparator7.Name = "ToolStripSeparator7";
            size2 = new Size(0x119, 6);
            this.ToolStripSeparator7.Size = size2;
            this.menuSaveFMapQuick.Name = "menuSaveFMapQuick";
            size2 = new Size(0x11c, 0x18);
            this.menuSaveFMapQuick.Size = size2;
            this.menuSaveFMapQuick.Text = "Quick Save fmap";
            this.ToolStripSeparator11.Name = "ToolStripSeparator11";
            size2 = new Size(0x119, 6);
            this.ToolStripSeparator11.Size = size2;
            this.menuSaveFME.Name = "menuSaveFME";
            size2 = new Size(0x11c, 0x18);
            this.menuSaveFME.Size = size2;
            this.menuSaveFME.Text = "Map fme (1.19 compatability)...";
            this.ToolStripSeparator5.Name = "ToolStripSeparator5";
            size2 = new Size(0x119, 6);
            this.ToolStripSeparator5.Size = size2;
            this.MapLNDToolStripMenuItem.Name = "MapLNDToolStripMenuItem";
            size2 = new Size(0x11c, 0x18);
            this.MapLNDToolStripMenuItem.Size = size2;
            this.MapLNDToolStripMenuItem.Text = "Export Map LND...";
            this.ToolStripSeparator6.Name = "ToolStripSeparator6";
            size2 = new Size(0x119, 6);
            this.ToolStripSeparator6.Size = size2;
            this.menuExportMapTileTypes.Name = "menuExportMapTileTypes";
            size2 = new Size(0x11c, 0x18);
            this.menuExportMapTileTypes.Size = size2;
            this.menuExportMapTileTypes.Text = "Export Tile Types...";
            this.ToolStripMenuItem1.Name = "ToolStripMenuItem1";
            size2 = new Size(0x119, 6);
            this.ToolStripMenuItem1.Size = size2;
            this.MinimapBMPToolStripMenuItem.Name = "MinimapBMPToolStripMenuItem";
            size2 = new Size(0x11c, 0x18);
            this.MinimapBMPToolStripMenuItem.Size = size2;
            this.MinimapBMPToolStripMenuItem.Text = "Minimap Bitmap...";
            this.ToolStripMenuItem3.Name = "ToolStripMenuItem3";
            size2 = new Size(0x11c, 0x18);
            this.ToolStripMenuItem3.Size = size2;
            this.ToolStripMenuItem3.Text = "Heightmap Bitmap...";
            this.ToolStripSeparator1.Name = "ToolStripSeparator1";
            size2 = new Size(0xae, 6);
            this.ToolStripSeparator1.Size = size2;
            this.ToolStripMenuItem4.DropDownItems.AddRange(new ToolStripItem[] { this.ImportHeightmapToolStripMenuItem, this.ToolStripSeparator8, this.menuImportTileTypes });
            this.ToolStripMenuItem4.Name = "ToolStripMenuItem4";
            size2 = new Size(0xb1, 0x18);
            this.ToolStripMenuItem4.Size = size2;
            this.ToolStripMenuItem4.Text = "Import";
            this.ImportHeightmapToolStripMenuItem.Name = "ImportHeightmapToolStripMenuItem";
            size2 = new Size(0xa2, 0x18);
            this.ImportHeightmapToolStripMenuItem.Size = size2;
            this.ImportHeightmapToolStripMenuItem.Text = "Heightmap...";
            this.ToolStripSeparator8.Name = "ToolStripSeparator8";
            size2 = new Size(0x9f, 6);
            this.ToolStripSeparator8.Size = size2;
            this.menuImportTileTypes.Name = "menuImportTileTypes";
            size2 = new Size(0xa2, 0x18);
            this.menuImportTileTypes.Size = size2;
            this.menuImportTileTypes.Text = "Tile Types...";
            this.ToolStripMenuItem2.Name = "ToolStripMenuItem2";
            size2 = new Size(0xae, 6);
            this.ToolStripMenuItem2.Size = size2;
            this.MapWZToolStripMenuItem.Name = "MapWZToolStripMenuItem";
            size2 = new Size(0xb1, 0x18);
            this.MapWZToolStripMenuItem.Size = size2;
            this.MapWZToolStripMenuItem.Text = "Compile Map...";
            this.ToolStripSeparator4.Name = "ToolStripSeparator4";
            size2 = new Size(0xae, 6);
            this.ToolStripSeparator4.Size = size2;
            this.CloseToolStripMenuItem.Name = "CloseToolStripMenuItem";
            size2 = new Size(0xb1, 0x18);
            this.CloseToolStripMenuItem.Size = size2;
            this.CloseToolStripMenuItem.Text = "Quit";
            this.menuTools.DropDownItems.AddRange(new ToolStripItem[] { this.menuReinterpret, this.menuWaterCorrection, this.ToolStripSeparator9, this.menuFlatOil, this.menuFlatStructures, this.ToolStripSeparator12, this.menuGenerator });
            this.menuTools.Name = "menuTools";
            size2 = new Size(0x39, 0x1b);
            this.menuTools.Size = size2;
            this.menuTools.Text = "Tools";
            this.menuReinterpret.Name = "menuReinterpret";
            size2 = new Size(0xf9, 0x18);
            this.menuReinterpret.Size = size2;
            this.menuReinterpret.Text = "Reinterpret Terrain";
            this.menuWaterCorrection.Name = "menuWaterCorrection";
            size2 = new Size(0xf9, 0x18);
            this.menuWaterCorrection.Size = size2;
            this.menuWaterCorrection.Text = "Water Triangle Correction";
            this.ToolStripSeparator9.Name = "ToolStripSeparator9";
            size2 = new Size(0xf6, 6);
            this.ToolStripSeparator9.Size = size2;
            this.menuFlatOil.Name = "menuFlatOil";
            size2 = new Size(0xf9, 0x18);
            this.menuFlatOil.Size = size2;
            this.menuFlatOil.Text = "Flatten Under Oils";
            this.menuFlatStructures.Name = "menuFlatStructures";
            size2 = new Size(0xf9, 0x18);
            this.menuFlatStructures.Size = size2;
            this.menuFlatStructures.Text = "Flatten Under Structures";
            this.ToolStripSeparator12.Name = "ToolStripSeparator12";
            size2 = new Size(0xf6, 6);
            this.ToolStripSeparator12.Size = size2;
            this.menuGenerator.Name = "menuGenerator";
            size2 = new Size(0xf9, 0x18);
            this.menuGenerator.Size = size2;
            this.menuGenerator.Text = "Generator...";
            this.menuOptions.Name = "menuOptions";
            size2 = new Size(0x52, 0x1b);
            this.menuOptions.Size = size2;
            this.menuOptions.Text = "Options...";
            this.TableLayoutPanel5.ColumnCount = 1;
            this.TableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            this.TableLayoutPanel5.Controls.Add(this.menuMain, 0, 0);
            this.TableLayoutPanel5.Controls.Add(this.SplitContainer1, 0, 1);
            this.TableLayoutPanel5.Dock = DockStyle.Fill;
            point2 = new Point(0, 0);
            this.TableLayoutPanel5.Location = point2;
            padding2 = new Padding(4);
            this.TableLayoutPanel5.Margin = padding2;
            this.TableLayoutPanel5.Name = "TableLayoutPanel5";
            this.TableLayoutPanel5.RowCount = 2;
            this.TableLayoutPanel5.RowStyles.Add(new RowStyle(SizeType.Absolute, 31f));
            this.TableLayoutPanel5.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            size2 = new Size(0x510, 0x28f);
            this.TableLayoutPanel5.Size = size2;
            this.TableLayoutPanel5.TabIndex = 1;
            point2 = new Point(4, 0x18);
            this.TabPage13.Location = point2;
            this.TabPage13.Name = "TabPage13";
            padding2 = new Padding(3);
            this.TabPage13.Padding = padding2;
            size2 = new Size(0xc1, 0);
            this.TabPage13.Size = size2;
            this.TabPage13.TabIndex = 0;
            this.TabPage13.Text = "1";
            this.TabPage13.UseVisualStyleBackColor = true;
            point2 = new Point(4, 0x18);
            this.TabPage14.Location = point2;
            this.TabPage14.Name = "TabPage14";
            padding2 = new Padding(3);
            this.TabPage14.Padding = padding2;
            size2 = new Size(0xc1, 0);
            this.TabPage14.Size = size2;
            this.TabPage14.TabIndex = 1;
            this.TabPage14.Text = "2";
            this.TabPage14.UseVisualStyleBackColor = true;
            point2 = new Point(4, 0x18);
            this.TabPage15.Location = point2;
            this.TabPage15.Name = "TabPage15";
            size2 = new Size(0xc1, 0);
            this.TabPage15.Size = size2;
            this.TabPage15.TabIndex = 2;
            this.TabPage15.Text = "3";
            this.TabPage15.UseVisualStyleBackColor = true;
            point2 = new Point(4, 0x18);
            this.TabPage16.Location = point2;
            this.TabPage16.Name = "TabPage16";
            size2 = new Size(0xc1, 0);
            this.TabPage16.Size = size2;
            this.TabPage16.TabIndex = 3;
            this.TabPage16.Text = "4";
            this.TabPage16.UseVisualStyleBackColor = true;
            point2 = new Point(4, 0x18);
            this.TabPage21.Location = point2;
            this.TabPage21.Name = "TabPage21";
            size2 = new Size(0xc1, 0);
            this.TabPage21.Size = size2;
            this.TabPage21.TabIndex = 4;
            this.TabPage21.Text = "5";
            this.TabPage21.UseVisualStyleBackColor = true;
            point2 = new Point(4, 0x18);
            this.TabPage22.Location = point2;
            this.TabPage22.Name = "TabPage22";
            size2 = new Size(0xc1, 0);
            this.TabPage22.Size = size2;
            this.TabPage22.TabIndex = 5;
            this.TabPage22.Text = "6";
            this.TabPage22.UseVisualStyleBackColor = true;
            point2 = new Point(4, 0x18);
            this.TabPage23.Location = point2;
            this.TabPage23.Name = "TabPage23";
            size2 = new Size(0xc1, 0);
            this.TabPage23.Size = size2;
            this.TabPage23.TabIndex = 6;
            this.TabPage23.Text = "7";
            this.TabPage23.UseVisualStyleBackColor = true;
            point2 = new Point(4, 0x18);
            this.TabPage24.Location = point2;
            this.TabPage24.Name = "TabPage24";
            size2 = new Size(0xc1, 0);
            this.TabPage24.Size = size2;
            this.TabPage24.TabIndex = 7;
            this.TabPage24.Text = "8";
            this.TabPage24.UseVisualStyleBackColor = true;
            this.AllowDrop = true;
            this.AutoScaleMode = AutoScaleMode.None;
            size2 = new Size(0x510, 0x28f);
            this.ClientSize = size2;
            this.Controls.Add(this.TableLayoutPanel5);
            this.MainMenuStrip = this.menuMain;
            padding2 = new Padding(4);
            this.Margin = padding2;
            this.Name = "frmMain";
            this.WindowState = FormWindowState.Minimized;
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
            ((ISupportInitialize) this.dgvFeatures).EndInit();
            this.TabPage2.ResumeLayout(false);
            ((ISupportInitialize) this.dgvStructures).EndInit();
            this.TabPage3.ResumeLayout(false);
            ((ISupportInitialize) this.dgvDroids).EndInit();
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

        private void LNDToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.Save_LND_Prompt();
        }

        public void Load_Autosave_Prompt()
        {
            if (!Directory.Exists(modProgram.AutoSavePath))
            {
                Interaction.MsgBox("Autosave directory does not exist. There are no autosaves.", MsgBoxStyle.ApplicationModal, "");
            }
            else
            {
                OpenFileDialog dialog = new OpenFileDialog {
                    FileName = "",
                    Filter = "FlaME Files (*.fmap, *.fme)|*.fmap;*.fme|All Files (*.*)|*.*",
                    InitialDirectory = modProgram.AutoSavePath
                };
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    modSettings.Settings.OpenPath = System.IO.Path.GetDirectoryName(dialog.FileName);
                    clsResult result = new clsResult("Loading map");
                    result.Take(this.LoadMap(dialog.FileName));
                    modProgram.ShowWarnings(result);
                }
            }
        }

        public void Load_Heightmap_Prompt()
        {
            OpenFileDialog dialog = new OpenFileDialog {
                InitialDirectory = modSettings.Settings.OpenPath,
                FileName = "",
                Filter = "Image Files (*.bmp, *.png)|*.bmp;*.png|All Files (*.*)|*.*"
            };
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                modSettings.Settings.OpenPath = System.IO.Path.GetDirectoryName(dialog.FileName);
                System.Drawing.Bitmap resultBitmap = null;
                modProgram.sResult result = modBitmap.LoadBitmap(dialog.FileName, ref resultBitmap);
                if (!result.Success)
                {
                    Interaction.MsgBox("Failed to load image: " + result.Problem, MsgBoxStyle.ApplicationModal, null);
                }
                else
                {
                    clsMap mainMap = this.MainMap;
                    clsMap newMap = null;
                    if ((mainMap != null) && (Interaction.MsgBox("Apply heightmap to the current map?", MsgBoxStyle.YesNo, null) == MsgBoxResult.Yes))
                    {
                        newMap = mainMap;
                    }
                    if (newMap == null)
                    {
                        modMath.sXY_int tileSize = new modMath.sXY_int(resultBitmap.Width - 1, resultBitmap.Height - 1);
                        newMap = new clsMap(tileSize);
                    }
                    int num3 = Math.Min(resultBitmap.Height - 1, newMap.Terrain.TileSize.Y);
                    for (int i = 0; i <= num3; i++)
                    {
                        int num4 = Math.Min(resultBitmap.Width - 1, newMap.Terrain.TileSize.X);
                        for (int j = 0; j <= num4; j++)
                        {
                            Color pixel = resultBitmap.GetPixel(j, i);
                            newMap.Terrain.Vertices[j, i].Height = (byte) Math.Round(Math.Min(Math.Round((double) (((double) ((pixel.R + pixel.G) + pixel.B)) / 3.0)), 255.0));
                        }
                    }
                    if (newMap == mainMap)
                    {
                        newMap.SectorTerrainUndoChanges.SetAllChanged();
                        newMap.SectorUnitHeightsChanges.SetAllChanged();
                        newMap.SectorGraphicsChanges.SetAllChanged();
                        newMap.Update();
                        newMap.UndoStepCreate("Apply heightmap");
                    }
                    else
                    {
                        this.NewMainMap(newMap);
                        newMap.Update();
                    }
                    this.View_DrawViewLater();
                }
            }
        }

        public void Load_Map_Prompt()
        {
            OpenFileDialog dialog = new OpenFileDialog {
                InitialDirectory = modSettings.Settings.OpenPath,
                FileName = "",
                Filter = "Warzone Map Files (*.fmap, *.fme, *.wz, *.gam, *.lnd)|*.fmap;*.fme;*.wz;*.gam;*.lnd|All Files (*.*)|*.*",
                Multiselect = true
            };
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                modSettings.Settings.OpenPath = System.IO.Path.GetDirectoryName(dialog.FileName);
                clsResult result = new clsResult("Loading maps");
                foreach (string str in dialog.FileNames)
                {
                    result.Take(this.LoadMap(str));
                }
                modProgram.ShowWarnings(result);
            }
        }

        public void Load_TTP_Prompt()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            clsMap mainMap = this.MainMap;
            if (mainMap != null)
            {
                dialog.InitialDirectory = modSettings.Settings.OpenPath;
                dialog.FileName = "";
                dialog.Filter = "TTP Files (*.ttp)|*.ttp|All Files (*.*)|*.*";
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    modSettings.Settings.OpenPath = System.IO.Path.GetDirectoryName(dialog.FileName);
                    modProgram.sResult result = mainMap.Load_TTP(dialog.FileName);
                    if (result.Success)
                    {
                        this.TextureView.DrawViewLater();
                    }
                    else
                    {
                        Interaction.MsgBox("Importing tile types failed: " + result.Problem, MsgBoxStyle.ApplicationModal, null);
                    }
                }
            }
        }

        private void LoadInterfaceImage(string ImagePath, ref System.Drawing.Bitmap ResultBitmap, clsResult Result)
        {
            ResultBitmap = null;
            if (!modBitmap.LoadBitmap(ImagePath, ref ResultBitmap).Success)
            {
                Result.WarningAdd("Unable to load image \"" + ImagePath + "\"");
            }
        }

        private clsResult LoadInterfaceImages()
        {
            clsResult result = new clsResult("Loading interface images");
            System.Drawing.Bitmap resultBitmap = null;
            System.Drawing.Bitmap bitmap2 = null;
            System.Drawing.Bitmap bitmap6 = null;
            System.Drawing.Bitmap bitmap7 = null;
            System.Drawing.Bitmap bitmap4 = null;
            System.Drawing.Bitmap bitmap8 = null;
            System.Drawing.Bitmap bitmap9 = null;
            System.Drawing.Bitmap bitmap12 = null;
            System.Drawing.Bitmap bitmap13 = null;
            System.Drawing.Bitmap bitmap10 = null;
            System.Drawing.Bitmap bitmap11 = null;
            System.Drawing.Bitmap bitmap3 = null;
            this.LoadInterfaceImage(modProgram.InterfaceImagesPath + "displayautotexture.png", ref resultBitmap, result);
            this.LoadInterfaceImage(modProgram.InterfaceImagesPath + "drawtileorientation.png", ref bitmap2, result);
            this.LoadInterfaceImage(modProgram.InterfaceImagesPath + "save.png", ref bitmap6, result);
            this.LoadInterfaceImage(modProgram.InterfaceImagesPath + "selection.png", ref bitmap7, result);
            this.LoadInterfaceImage(modProgram.InterfaceImagesPath + "objectsselect.png", ref bitmap4, result);
            this.LoadInterfaceImage(modProgram.InterfaceImagesPath + "selectioncopy.png", ref bitmap8, result);
            this.LoadInterfaceImage(modProgram.InterfaceImagesPath + "selectionflipx.png", ref bitmap9, result);
            this.LoadInterfaceImage(modProgram.InterfaceImagesPath + "selectionrotateclockwise.png", ref bitmap12, result);
            this.LoadInterfaceImage(modProgram.InterfaceImagesPath + "selectionrotateanticlockwise.png", ref bitmap13, result);
            this.LoadInterfaceImage(modProgram.InterfaceImagesPath + "selectionpaste.png", ref bitmap10, result);
            this.LoadInterfaceImage(modProgram.InterfaceImagesPath + "selectionpasteoptions.png", ref bitmap11, result);
            this.LoadInterfaceImage(modProgram.InterfaceImagesPath + "gateways.png", ref bitmap3, result);
            System.Drawing.Bitmap bitmap5 = null;
            this.LoadInterfaceImage(modProgram.InterfaceImagesPath + "problem.png", ref bitmap5, result);
            System.Drawing.Bitmap bitmap14 = null;
            this.LoadInterfaceImage(modProgram.InterfaceImagesPath + "warning.png", ref bitmap14, result);
            Size size2 = new Size(0x10, 0x10);
            modWarnings.WarningImages.ImageSize = size2;
            if (bitmap5 != null)
            {
                modWarnings.WarningImages.Images.Add("problem", bitmap5);
            }
            if (bitmap14 != null)
            {
                modWarnings.WarningImages.Images.Add("warning", bitmap14);
            }
            this.tsbDrawAutotexture.Image = resultBitmap;
            this.tsbDrawTileOrientation.Image = bitmap2;
            this.tsbSave.Image = bitmap6;
            this.tsbSelection.Image = bitmap7;
            this.tsbSelectionObjects.Image = bitmap4;
            this.tsbSelectionCopy.Image = bitmap8;
            this.tsbSelectionFlipX.Image = bitmap9;
            this.tsbSelectionRotateClockwise.Image = bitmap12;
            this.tsbSelectionRotateCounterClockwise.Image = bitmap13;
            this.tsbSelectionPaste.Image = bitmap10;
            this.tsbSelectionPasteOptions.Image = bitmap11;
            this.tsbGateways.Image = bitmap3;
            this.btnTextureAnticlockwise.Image = bitmap13;
            this.btnTextureClockwise.Image = bitmap12;
            this.btnTextureFlipX.Image = bitmap9;
            return result;
        }

        public clsResult LoadMap(string Path)
        {
            clsResult result2 = new clsResult("");
            modProgram.sSplitPath path = new modProgram.sSplitPath(Path);
            clsMap newMap = new clsMap();
            string str2 = path.FileExtension.ToLower();
            if (str2 == "fmap")
            {
                result2.Add(newMap.Load_FMap(Path));
                newMap.PathInfo = new clsMap.clsPathInfo(Path, true);
            }
            else if (str2 == "fme")
            {
                result2.Add(newMap.Load_FME(Path));
                newMap.PathInfo = new clsMap.clsPathInfo(Path, false);
            }
            else if (str2 == "wz")
            {
                result2.Add(newMap.Load_WZ(Path));
                newMap.PathInfo = new clsMap.clsPathInfo(Path, false);
            }
            else if (str2 == "gam")
            {
                result2.Add(newMap.Load_Game(Path));
                newMap.PathInfo = new clsMap.clsPathInfo(Path, false);
            }
            else if (str2 == "lnd")
            {
                result2.Add(newMap.Load_LND(Path));
                newMap.PathInfo = new clsMap.clsPathInfo(Path, false);
            }
            else
            {
                result2.ProblemAdd("File extension not recognised.");
            }
            if (result2.HasProblems)
            {
                newMap.Deallocate();
                return result2;
            }
            this.NewMainMap(newMap);
            return result2;
        }

        private void lstAutoRoad_Click(object sender, EventArgs e)
        {
            if (!((modTools.Tool == modTools.Tools.RoadPlace) | (modTools.Tool == modTools.Tools.RoadLines)))
            {
                this.rdoAutoRoadLine.Checked = true;
                this.rdoAutoRoadLine_Click(null, null);
            }
        }

        private void lstAutoRoad_SelectedIndexChanged(object sender, EventArgs e)
        {
            clsMap mainMap = this.MainMap;
            if (mainMap != null)
            {
                if (this.lstAutoRoad.SelectedIndex < 0)
                {
                    modProgram.SelectedRoad = null;
                }
                else if (this.lstAutoRoad.SelectedIndex < mainMap.Painter.RoadCount)
                {
                    modProgram.SelectedRoad = mainMap.Painter.Roads[this.lstAutoRoad.SelectedIndex];
                }
                else
                {
                    Debugger.Break();
                    modProgram.SelectedRoad = null;
                }
            }
        }

        private void lstAutoTexture_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.lstAutoTexture.Enabled && !((modTools.Tool == modTools.Tools.TerrainBrush) | (modTools.Tool == modTools.Tools.TerrainFill)))
            {
                this.rdoAutoTexturePlace.Checked = true;
                this.rdoAutoTexturePlace_Click(null, null);
            }
        }

        private void lstAutoTexture_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            clsMap mainMap = this.MainMap;
            if (mainMap != null)
            {
                if (this.lstAutoTexture.SelectedIndex < 0)
                {
                    modProgram.SelectedTerrain = null;
                }
                else if (this.lstAutoTexture.SelectedIndex < mainMap.Painter.TerrainCount)
                {
                    modProgram.SelectedTerrain = mainMap.Painter.Terrains[this.lstAutoTexture.SelectedIndex];
                }
                else
                {
                    Debugger.Break();
                    modProgram.SelectedTerrain = null;
                }
            }
        }

        private void lstScriptAreas_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.lstScriptAreas.Enabled)
            {
                this._SelectedScriptMarker = this.lstScriptAreas_Objects[this.lstScriptAreas.SelectedIndex];
                this.lstScriptPositions.Enabled = false;
                this.lstScriptPositions.SelectedIndex = -1;
                this.lstScriptPositions.Enabled = true;
                this.SelectedScriptMarker_Update();
                this.View_DrawViewLater();
            }
        }

        private void lstScriptPositions_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.lstScriptPositions.Enabled)
            {
                this._SelectedScriptMarker = this.lstScriptPositions_Objects[this.lstScriptPositions.SelectedIndex];
                this.lstScriptAreas.Enabled = false;
                this.lstScriptAreas.SelectedIndex = -1;
                this.lstScriptAreas.Enabled = true;
                this.SelectedScriptMarker_Update();
                this.View_DrawViewLater();
            }
        }

        private void MainMap_Modified()
        {
            this.tsbSave.Enabled = true;
        }

        public void MainMapAfterChanged()
        {
            clsMap mainMap = this.MainMap;
            this.MapView.UpdateTabs();
            modProgram.SelectedTerrain = null;
            modProgram.SelectedRoad = null;
            this.Resize_Update();
            this.MainMapTilesetChanged();
            this.PainterTerrains_Refresh(-1, -1);
            this.ScriptMarkerLists_Update();
            this.NewPlayerNum.Enabled = false;
            this.ObjectPlayerNum.Enabled = false;
            if (mainMap != null)
            {
                mainMap.CheckMessages();
                mainMap.ViewInfo.FOV_Calc();
                mainMap.SectorGraphicsChanges.SetAllChanged();
                mainMap.Update();
                mainMap.MinimapMakeLater();
                this.tsbSave.Enabled = mainMap.ChangedSinceSave;
                this.NewPlayerNum.SetMap(mainMap);
                this.NewPlayerNum.Target = mainMap.SelectedUnitGroup;
                this.ObjectPlayerNum.SetMap(mainMap);
                this.MainMap.Changed += new clsMap.ChangedEventHandler(this.MainMap_Modified);
            }
            else
            {
                this.tsbSave.Enabled = false;
                this.NewPlayerNum.SetMap(null);
                this.NewPlayerNum.Target = null;
                this.ObjectPlayerNum.SetMap(null);
            }
            this.NewPlayerNum.Enabled = true;
            this.ObjectPlayerNum.Enabled = true;
            this.SelectedObject_Changed();
            this.TitleTextUpdate();
            this.TextureView.ScrollUpdate();
            this.TextureView.DrawViewLater();
            this.View_DrawViewLater();
        }

        public void MainMapBeforeChanged()
        {
            clsMap mainMap = this.MainMap;
            this.MapView.OpenGLControl.Focus();
            if (mainMap != null)
            {
                this.MainMap.Changed -= new clsMap.ChangedEventHandler(this.MainMap_Modified);
                if (mainMap.ReadyForUserInput)
                {
                    mainMap.SectorAll_GLLists_Delete();
                    mainMap.SectorGraphicsChanges.SetAllChanged();
                }
            }
        }

        public void MainMapTilesetChanged()
        {
            clsMap mainMap = this.MainMap;
            if (mainMap == null)
            {
                this.cboTileset.SelectedIndex = -1;
            }
            else
            {
                int num2 = modProgram.Tilesets.Count - 1;
                int num = 0;
                while (num <= num2)
                {
                    if (modProgram.Tilesets[num] == mainMap.Tileset)
                    {
                        break;
                    }
                    num++;
                }
                if (num == modProgram.Tilesets.Count)
                {
                    this.cboTileset.SelectedIndex = -1;
                }
                else
                {
                    this.cboTileset.SelectedIndex = num;
                }
            }
        }

        public void Map_Resize(modMath.sXY_int Offset, modMath.sXY_int NewSize)
        {
            clsMap mainMap = this.MainMap;
            if ((mainMap != null) && (Interaction.MsgBox("Resizing can't be undone. Continue?", MsgBoxStyle.Question | MsgBoxStyle.OkCancel, "") == MsgBoxResult.Ok))
            {
                if ((NewSize.X < 1) | (NewSize.Y < 1))
                {
                    Interaction.MsgBox("Map sizes must be at least 1.", MsgBoxStyle.ApplicationModal, "");
                }
                else if (!((NewSize.X > 250) | (NewSize.Y > 250)) || (Interaction.MsgBox("Warzone doesn't support map sizes above " + Conversions.ToString(250) + ". Continue anyway?", MsgBoxStyle.YesNo, "") == MsgBoxResult.Yes))
                {
                    mainMap.TerrainResize(Offset, NewSize);
                    this.Resize_Update();
                    this.ScriptMarkerLists_Update();
                    mainMap.SectorGraphicsChanges.SetAllChanged();
                    mainMap.Update();
                    this.View_DrawViewLater();
                }
            }
        }

        private void MapWZToolStripMenuItem_Click(object sender, EventArgs e)
        {
            clsMap mainMap = this.MainMap;
            if (mainMap != null)
            {
                if (mainMap.CompileScreen == null)
                {
                    frmCompile.Create(mainMap).Show();
                }
                else
                {
                    mainMap.CompileScreen.Activate();
                }
            }
        }

        private void Me_LostFocus(object eventSender, EventArgs eventArgs)
        {
            modProgram.ViewKeyDown_Clear();
        }

        private void menuExportMapTileTypes_Click(object sender, EventArgs e)
        {
            this.PromptSave_TTP();
        }

        private void menuImportTileTypes_Click(object sender, EventArgs e)
        {
            this.Load_TTP_Prompt();
        }

        private void menuMiniShowCliffs_Click(object sender, EventArgs e)
        {
            this.UpdateMinimap();
        }

        private void menuMiniShowGateways_Click(object sender, EventArgs e)
        {
            this.UpdateMinimap();
        }

        private void menuMiniShowHeight_Click(object sender, EventArgs e)
        {
            this.UpdateMinimap();
        }

        private void menuMiniShowTex_Click(object sender, EventArgs e)
        {
            this.UpdateMinimap();
        }

        private void menuMiniShowUnits_Click(object sender, EventArgs e)
        {
            this.UpdateMinimap();
        }

        private void menuOptions_Click(object sender, EventArgs e)
        {
            if (modMain.frmOptionsInstance != null)
            {
                modMain.frmOptionsInstance.Activate();
            }
            else
            {
                modMain.frmOptionsInstance = new frmOptions();
                modMain.frmOptionsInstance.Show();
            }
        }

        private void menuRotateNothing_Click(object sender, EventArgs e)
        {
            this.PasteRotateObjects = modProgram.enumObjectRotateMode.None;
            this.menuRotateUnits.Checked = false;
            this.menuRotateWalls.Checked = false;
            this.menuRotateNothing.Checked = true;
        }

        private void menuRotateUnits_Click(object sender, EventArgs e)
        {
            this.PasteRotateObjects = modProgram.enumObjectRotateMode.All;
            this.menuRotateUnits.Checked = true;
            this.menuRotateWalls.Checked = false;
            this.menuRotateNothing.Checked = false;
        }

        private void menuRotateWalls_Click(object sender, EventArgs e)
        {
            this.PasteRotateObjects = modProgram.enumObjectRotateMode.Walls;
            this.menuRotateUnits.Checked = false;
            this.menuRotateWalls.Checked = true;
            this.menuRotateNothing.Checked = false;
        }

        private void menuSaveFMapQuick_Click(object sender, EventArgs e)
        {
            this.QuickSave();
        }

        private void menuSaveFME_Click(object sender, EventArgs e)
        {
            this.Save_FME_Prompt();
        }

        private void MinimapBMPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Save_Minimap_Prompt();
        }

        public void New_Prompt()
        {
            this.NewMap();
        }

        public void NewMainMap(clsMap NewMap)
        {
            NewMap.frmMainLink.Connect(this._LoadedMaps);
            this.SetMainMap(NewMap);
        }

        public void NewMap()
        {
            modMath.sXY_int tileSize = new modMath.sXY_int(0x40, 0x40);
            clsMap newMap = new clsMap(tileSize);
            this.NewMainMap(newMap);
            newMap.RandomizeTileOrientations();
            newMap.Update();
            newMap.UndoClear();
        }

        private void NewMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.New_Prompt();
        }

        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        private clsResult NoTile_Texture_Load()
        {
            modBitmap.sBitmapGLTexture texture;
            clsResult result2 = new clsResult("Loading error terrain textures");
            System.Drawing.Bitmap resultBitmap = null;
            texture.MagFilter = TextureMagFilter.Nearest;
            texture.MinFilter = TextureMinFilter.Nearest;
            texture.TextureNum = 0;
            texture.MipMapLevel = 0;
            if (modBitmap.LoadBitmap(modProgram.EndWithPathSeperator(MyProject.Application.Info.DirectoryPath) + "notile.png", ref resultBitmap).Success)
            {
                clsResult resultToAdd = new clsResult("notile.png");
                resultToAdd.Take(modBitmap.BitmapIsGLCompatible(resultBitmap));
                result2.Add(resultToAdd);
                texture.Texture = resultBitmap;
                texture.Perform();
                modProgram.GLTexture_NoTile = texture.TextureNum;
            }
            if (modBitmap.LoadBitmap(modProgram.EndWithPathSeperator(MyProject.Application.Info.DirectoryPath) + "overflow.png", ref resultBitmap).Success)
            {
                clsResult result4 = new clsResult("overflow.png");
                result4.Take(modBitmap.BitmapIsGLCompatible(resultBitmap));
                result2.Add(result4);
                texture.Texture = resultBitmap;
                texture.Perform();
                modProgram.GLTexture_OverflowTile = texture.TextureNum;
            }
            return result2;
        }

        public modLists.SimpleList<ItemType> ObjectFindText<ItemType>(modLists.SimpleList<ItemType> list, string text) where ItemType: clsUnitType
        {
            modLists.SimpleList<ItemType> list3 = new modLists.SimpleList<ItemType> {
                MaintainOrder = true
            };
            text = text.ToLower();
            int num2 = list.Count - 1;
            for (int i = 0; i <= num2; i++)
            {
                ItemType newItem = list[i];
                string result = null;
                if (newItem.GetCode(ref result))
                {
                    if ((result.ToLower().IndexOf(text) >= 0) | (newItem.GetName().ToLower().IndexOf(text) >= 0))
                    {
                        list3.Add(newItem);
                    }
                }
                else if (newItem.GetName().ToLower().IndexOf(text) >= 0)
                {
                    list3.Add(newItem);
                }
            }
            return list3;
        }

        private void ObjectListFill<ObjectType>(modLists.SimpleList<ObjectType> objects, DataGridView gridView) where ObjectType: clsUnitType
        {
            bool flag;
            modLists.SimpleList<ObjectType> list;
            string text = this.txtObjectFind.Text;
            if (text == null)
            {
                flag = false;
            }
            else if (text == "")
            {
                flag = false;
            }
            else
            {
                flag = true;
            }
            if (flag)
            {
                list = this.ObjectFindText<ObjectType>(objects, text);
            }
            else
            {
                list = objects;
            }
            DataTable table = new DataTable();
            table.Columns.Add("Item", typeof(clsUnitType));
            table.Columns.Add("Internal Name", typeof(string));
            table.Columns.Add("In-Game Name", typeof(string));
            int num2 = list.Count - 1;
            for (int i = 0; i <= num2; i++)
            {
                ObjectType local = list[i];
                string result = null;
                local.GetCode(ref result);
                table.Rows.Add(new object[] { local, result, local.GetName().Replace("*", "") });
            }
            gridView.DataSource = table;
            gridView.Columns[0].Visible = false;
            gridView.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            gridView.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.ObjectTypeSelectionUpdate(gridView);
        }

        public void ObjectPicker(clsUnitType UnitType)
        {
            modTools.Tool = modTools.Tools.ObjectPlace;
            if (!modControls.KeyboardProfile.Active(modControls.Control_Unit_Multiselect))
            {
                this.dgvFeatures.ClearSelection();
                this.dgvStructures.ClearSelection();
                this.dgvDroids.ClearSelection();
            }
            this.SelectedObjectTypes.Clear();
            this.SelectedObjectTypes.Add(UnitType.UnitType_frmMainSelectedLink);
            clsMap mainMap = this.MainMap;
            if (mainMap != null)
            {
                mainMap.MinimapMakeLater();
                this.View_DrawViewLater();
            }
        }

        private void ObjectsUpdate()
        {
            if (modProgram.ProgramInitializeFinished)
            {
                switch (this.TabControl1.SelectedIndex)
                {
                    case 0:
                        this.ObjectListFill<clsFeatureType>(modProgram.ObjectData.FeatureTypes.GetItemsAsSimpleList(), this.dgvFeatures);
                        break;

                    case 1:
                        this.ObjectListFill<clsStructureType>(modProgram.ObjectData.StructureTypes.GetItemsAsSimpleList(), this.dgvStructures);
                        break;

                    case 2:
                        this.ObjectListFill<clsDroidTemplate>(modProgram.ObjectData.DroidTemplates.GetItemsAsSimpleList(), this.dgvDroids);
                        break;
                }
            }
        }

        private void ObjectTypeSelectionUpdate(DataGridView dataView)
        {
            IEnumerator enumerator;
            this.SelectedObjectTypes.Clear();
            try
            {
                enumerator = dataView.SelectedRows.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    DataGridViewRow current = (DataGridViewRow) enumerator.Current;
                    clsUnitType type = (clsUnitType) current.Cells[0].Value;
                    if (!type.UnitType_frmMainSelectedLink.IsConnected)
                    {
                        this.SelectedObjectTypes.Add(type.UnitType_frmMainSelectedLink);
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
            clsMap mainMap = this.MainMap;
            if (mainMap != null)
            {
                mainMap.MinimapMakeLater();
                this.View_DrawViewLater();
            }
        }

        private void OpenGL_DragDrop(object sender, DragEventArgs e)
        {
            string[] data = (string[]) e.Data.GetData(DataFormats.FileDrop);
            clsResult result = new clsResult("Loading drag-dropped maps");
            foreach (string str in data)
            {
                result.Take(this.LoadMap(str));
            }
            modProgram.ShowWarnings(result);
        }

        private void OpenGL_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Load_Map_Prompt();
        }

        public void PainterTerrains_Refresh(int Terrain_NewSelectedIndex, int Road_NewSelectedIndex)
        {
            this.lstAutoTexture.Items.Clear();
            this.lstAutoRoad.Items.Clear();
            clsMap mainMap = this.MainMap;
            if (mainMap != null)
            {
                int num2 = mainMap.Painter.TerrainCount - 1;
                int index = 0;
                while (index <= num2)
                {
                    this.lstAutoTexture.Items.Add(mainMap.Painter.Terrains[index].Name);
                    index++;
                }
                int num3 = mainMap.Painter.RoadCount - 1;
                for (index = 0; index <= num3; index++)
                {
                    this.lstAutoRoad.Items.Add(mainMap.Painter.Roads[index].Name);
                }
                this.lstAutoTexture.SelectedIndex = Terrain_NewSelectedIndex;
                this.lstAutoRoad.SelectedIndex = Road_NewSelectedIndex;
            }
        }

        public void PromptSave_TTP()
        {
            clsMap mainMap = this.MainMap;
            if (mainMap != null)
            {
                SaveFileDialog dialog = new SaveFileDialog {
                    InitialDirectory = modSettings.Settings.SavePath,
                    FileName = "",
                    Filter = "TTP Files (*.ttp)|*.ttp"
                };
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    modSettings.Settings.SavePath = System.IO.Path.GetDirectoryName(dialog.FileName);
                    modProgram.sResult result = mainMap.Write_TTP(dialog.FileName, true);
                    if (!result.Success)
                    {
                        Interaction.MsgBox("There was a problem saving the tile types: " + result.Problem, MsgBoxStyle.ApplicationModal, null);
                    }
                }
            }
        }

        private void QuickSave()
        {
            clsMap mainMap = this.MainMap;
            if ((mainMap != null) && mainMap.Save_FMap_Quick())
            {
                modMain.frmMainInstance.tsbSave.Enabled = false;
                this.TitleTextUpdate();
            }
        }

        private void rdoAutoCliffBrush_Click(object sender, EventArgs e)
        {
            modTools.Tool = modTools.Tools.CliffBrush;
        }

        private void rdoAutoCliffRemove_Click(object sender, EventArgs e)
        {
            modTools.Tool = modTools.Tools.CliffRemove;
        }

        private void rdoAutoRoadLine_Click(object sender, EventArgs e)
        {
            modTools.Tool = modTools.Tools.RoadLines;
            clsMap mainMap = this.MainMap;
            if (mainMap != null)
            {
                mainMap.Selected_Tile_A = null;
                mainMap.Selected_Tile_B = null;
            }
        }

        private void rdoAutoRoadPlace_Click(object sender, EventArgs e)
        {
            modTools.Tool = modTools.Tools.RoadPlace;
        }

        private void rdoAutoTextureFill_CheckedChanged(object sender, EventArgs e)
        {
            modTools.Tool = modTools.Tools.TerrainFill;
        }

        private void rdoAutoTexturePlace_Click(object sender, EventArgs e)
        {
            modTools.Tool = modTools.Tools.TerrainBrush;
        }

        private void rdoCliffTriBrush_CheckedChanged(object sender, EventArgs e)
        {
            modTools.Tool = modTools.Tools.CliffTriangle;
        }

        private void rdoDroidTurret0_CheckedChanged(object sender, EventArgs e)
        {
            if (this.rdoDroidTurret0.Enabled)
            {
                clsMap mainMap = this.MainMap;
                if (((mainMap != null) && (mainMap.SelectedUnits.Count > 0)) && this.rdoDroidTurret0.Checked)
                {
                    this.rdoDroidTurret1.Checked = false;
                    this.rdoDroidTurret2.Checked = false;
                    this.rdoDroidTurret3.Checked = false;
                    if ((mainMap.SelectedUnits.Count <= 1) || (Interaction.MsgBox("Change number of turrets of multiple droids?", MsgBoxStyle.Question | MsgBoxStyle.OkCancel, "") == MsgBoxResult.Ok))
                    {
                        this.SelectedObjects_SetTurretCount(0);
                    }
                }
            }
        }

        private void rdoDroidTurret1_CheckedChanged(object sender, EventArgs e)
        {
            if (this.rdoDroidTurret1.Enabled)
            {
                clsMap mainMap = this.MainMap;
                if (((mainMap != null) && (mainMap.SelectedUnits.Count > 0)) && this.rdoDroidTurret1.Checked)
                {
                    this.rdoDroidTurret0.Checked = false;
                    this.rdoDroidTurret2.Checked = false;
                    this.rdoDroidTurret3.Checked = false;
                    if ((mainMap.SelectedUnits.Count <= 1) || (Interaction.MsgBox("Change number of turrets of multiple droids?", MsgBoxStyle.Question | MsgBoxStyle.OkCancel, "") == MsgBoxResult.Ok))
                    {
                        this.SelectedObjects_SetTurretCount(1);
                    }
                }
            }
        }

        private void rdoDroidTurret2_CheckedChanged(object sender, EventArgs e)
        {
            if (this.rdoDroidTurret2.Enabled)
            {
                clsMap mainMap = this.MainMap;
                if (((mainMap != null) && (mainMap.SelectedUnits.Count > 0)) && this.rdoDroidTurret2.Checked)
                {
                    this.rdoDroidTurret0.Checked = false;
                    this.rdoDroidTurret1.Checked = false;
                    this.rdoDroidTurret3.Checked = false;
                    if ((mainMap.SelectedUnits.Count <= 1) || (Interaction.MsgBox("Change number of turrets of multiple droids?", MsgBoxStyle.Question | MsgBoxStyle.OkCancel, "") == MsgBoxResult.Ok))
                    {
                        this.SelectedObjects_SetTurretCount(2);
                    }
                }
            }
        }

        private void rdoDroidTurret3_CheckedChanged(object sender, EventArgs e)
        {
            if (this.rdoDroidTurret2.Enabled)
            {
                clsMap mainMap = this.MainMap;
                if (((mainMap != null) && (mainMap.SelectedUnits.Count > 0)) && this.rdoDroidTurret3.Checked)
                {
                    this.rdoDroidTurret0.Checked = false;
                    this.rdoDroidTurret1.Checked = false;
                    this.rdoDroidTurret2.Checked = false;
                    if ((mainMap.SelectedUnits.Count <= 1) || (Interaction.MsgBox("Change number of turrets of multiple droids?", MsgBoxStyle.Question | MsgBoxStyle.OkCancel, "") == MsgBoxResult.Ok))
                    {
                        this.SelectedObjects_SetTurretCount(3);
                    }
                }
            }
        }

        private void rdoFillCliffIgnore_CheckedChanged(object sender, EventArgs e)
        {
            this.FillCliffAction = modProgram.enumFillCliffAction.Ignore;
        }

        private void rdoFillCliffStopAfter_CheckedChanged(object sender, EventArgs e)
        {
            this.FillCliffAction = modProgram.enumFillCliffAction.StopAfter;
        }

        private void rdoFillCliffStopBefore_CheckedChanged(object sender, EventArgs e)
        {
            this.FillCliffAction = modProgram.enumFillCliffAction.StopBefore;
        }

        private void rdoHeightChange_CheckedChanged(object sender, EventArgs e)
        {
            if (this.rdoHeightChange.Checked)
            {
                this.rdoHeightSet.Checked = false;
                this.rdoHeightSmooth.Checked = false;
                modTools.Tool = modTools.Tools.HeightChangeBrush;
            }
        }

        private void rdoHeightSet_CheckedChanged(object sender, EventArgs e)
        {
            if (this.rdoHeightSet.Checked)
            {
                this.rdoHeightSmooth.Checked = false;
                this.rdoHeightChange.Checked = false;
                modTools.Tool = modTools.Tools.HeightSetBrush;
            }
        }

        private void rdoHeightSmooth_CheckedChanged(object sender, EventArgs e)
        {
            if (this.rdoHeightSmooth.Checked)
            {
                this.rdoHeightSet.Checked = false;
                this.rdoHeightChange.Checked = false;
                modTools.Tool = modTools.Tools.HeightSmoothBrush;
            }
        }

        private void rdoObjectLine_Click(object sender, EventArgs e)
        {
            modTools.Tool = modTools.Tools.ObjectLines;
        }

        private void rdoObjectPlace_Click(object sender, EventArgs e)
        {
            modTools.Tool = modTools.Tools.ObjectPlace;
        }

        private void rdoObjectsSortInGame_Click(object sender, EventArgs e)
        {
            if (modProgram.ProgramInitializeFinished)
            {
                this.ObjectsUpdate();
            }
        }

        private void rdoObjectsSortInternal_Click(object sender, EventArgs e)
        {
            if (modProgram.ProgramInitializeFinished)
            {
                this.ObjectsUpdate();
            }
        }

        private void rdoObjectsSortNone_Click(object sender, EventArgs e)
        {
            if (modProgram.ProgramInitializeFinished)
            {
                this.ObjectsUpdate();
            }
        }

        private void rdoRoadRemove_Click(object sender, EventArgs e)
        {
            modTools.Tool = modTools.Tools.RoadRemove;
        }

        private void rdoTextureIgnoreTerrain_Click(object sender, EventArgs e)
        {
            if (this.rdoTextureIgnoreTerrain.Checked)
            {
                this.TextureTerrainAction = modProgram.enumTextureTerrainAction.Ignore;
                this.rdoTextureReinterpretTerrain.Checked = false;
                this.rdoTextureRemoveTerrain.Checked = false;
            }
        }

        private void rdoTextureReinterpretTerrain_Click(object sender, EventArgs e)
        {
            if (this.rdoTextureReinterpretTerrain.Checked)
            {
                this.TextureTerrainAction = modProgram.enumTextureTerrainAction.Reinterpret;
                this.rdoTextureIgnoreTerrain.Checked = false;
                this.rdoTextureRemoveTerrain.Checked = false;
            }
        }

        private void rdoTextureRemoveTerrain_Click(object sender, EventArgs e)
        {
            if (this.rdoTextureRemoveTerrain.Checked)
            {
                this.TextureTerrainAction = modProgram.enumTextureTerrainAction.Remove;
                this.rdoTextureIgnoreTerrain.Checked = false;
                this.rdoTextureReinterpretTerrain.Checked = false;
            }
        }

        private void ReinterpretTerrainToolStripMenuItem_Click(object sender, EventArgs e)
        {
            clsMap mainMap = this.MainMap;
            if (mainMap != null)
            {
                mainMap.TerrainInterpretChanges.SetAllChanged();
                mainMap.Update();
                mainMap.UndoStepCreate("Interpret Terrain");
            }
        }

        public void Resize_Update()
        {
            clsMap mainMap = this.MainMap;
            if (mainMap == null)
            {
                this.txtSizeX.Text = "";
                this.txtSizeY.Text = "";
                this.txtOffsetX.Text = "";
                this.txtOffsetY.Text = "";
            }
            else
            {
                this.txtSizeX.Text = modIO.InvariantToString_int(mainMap.Terrain.TileSize.X);
                this.txtSizeY.Text = modIO.InvariantToString_int(mainMap.Terrain.TileSize.Y);
                this.txtOffsetX.Text = "0";
                this.txtOffsetY.Text = "0";
            }
        }

        private void Save_FME_Prompt()
        {
            clsMap mainMap = this.MainMap;
            if (mainMap != null)
            {
                SaveFileDialog dialog = new SaveFileDialog {
                    InitialDirectory = modSettings.Settings.SavePath,
                    FileName = "",
                    Filter = "FlaME FME Map Files (*.fme)|*.fme"
                };
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    byte num;
                    modSettings.Settings.SavePath = System.IO.Path.GetDirectoryName(dialog.FileName);
                    if (!modIO.InvariantParse_byte(Interaction.InputBox("Enter the player number for scavenger units:", "", "", -1, -1), ref num))
                    {
                        Interaction.MsgBox("Unable to save FME: entered scavenger number is not a number.", MsgBoxStyle.ApplicationModal, null);
                    }
                    else
                    {
                        num = Math.Min(num, 10);
                        modProgram.sResult result = mainMap.Write_FME(dialog.FileName, true, num);
                        if (!result.Success)
                        {
                            Interaction.MsgBox("Unable to save FME: " + result.Problem, MsgBoxStyle.ApplicationModal, null);
                        }
                    }
                }
            }
        }

        public void Save_Heightmap_Prompt()
        {
            clsMap mainMap = this.MainMap;
            if (mainMap != null)
            {
                SaveFileDialog dialog = new SaveFileDialog {
                    InitialDirectory = modSettings.Settings.SavePath,
                    FileName = "",
                    Filter = "Bitmap File (*.bmp)|*.bmp"
                };
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    modSettings.Settings.SavePath = System.IO.Path.GetDirectoryName(dialog.FileName);
                    modProgram.sResult result = mainMap.Write_Heightmap(dialog.FileName, true);
                    if (!result.Success)
                    {
                        Interaction.MsgBox("There was a problem saving the heightmap bitmap: " + result.Problem, MsgBoxStyle.ApplicationModal, null);
                    }
                }
            }
        }

        public void Save_LND_Prompt()
        {
            clsMap mainMap = this.MainMap;
            if (mainMap != null)
            {
                SaveFileDialog dialog = new SaveFileDialog {
                    InitialDirectory = modSettings.Settings.SavePath,
                    FileName = "",
                    Filter = "Editworld Files (*.lnd)|*.lnd"
                };
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    modSettings.Settings.SavePath = System.IO.Path.GetDirectoryName(dialog.FileName);
                    modProgram.ShowWarnings(mainMap.Write_LND(dialog.FileName, true));
                }
            }
        }

        public void Save_Minimap_Prompt()
        {
            clsMap mainMap = this.MainMap;
            if (mainMap != null)
            {
                SaveFileDialog dialog = new SaveFileDialog {
                    InitialDirectory = modSettings.Settings.SavePath,
                    FileName = "",
                    Filter = "Bitmap File (*.bmp)|*.bmp"
                };
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    modSettings.Settings.SavePath = System.IO.Path.GetDirectoryName(dialog.FileName);
                    modProgram.sResult result = mainMap.Write_MinimapFile(dialog.FileName, true);
                    if (!result.Success)
                    {
                        Interaction.MsgBox("There was a problem saving the minimap bitmap: " + result.Problem, MsgBoxStyle.ApplicationModal, null);
                    }
                }
            }
        }

        public void ScriptMarkerLists_Update()
        {
            clsMap mainMap = this.MainMap;
            this.lstScriptPositions.Enabled = false;
            this.lstScriptAreas.Enabled = false;
            this.lstScriptPositions.Items.Clear();
            this.lstScriptAreas.Items.Clear();
            if (mainMap == null)
            {
                this._SelectedScriptMarker = null;
            }
            else
            {
                int num;
                IEnumerator enumerator;
                IEnumerator enumerator2;
                object obj2 = null;
                this.lstScriptPositions_Objects = new clsMap.clsScriptPosition[(mainMap.ScriptPositions.Count - 1) + 1];
                this.lstScriptAreas_Objects = new clsMap.clsScriptArea[(mainMap.ScriptAreas.Count - 1) + 1];
                try
                {
                    enumerator = mainMap.ScriptPositions.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        clsMap.clsScriptPosition current = (clsMap.clsScriptPosition) enumerator.Current;
                        num = this.lstScriptPositions.Items.Add(current.Label);
                        this.lstScriptPositions_Objects[num] = current;
                        if (current == this._SelectedScriptMarker)
                        {
                            obj2 = current;
                            this.lstScriptPositions.SelectedIndex = num;
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
                try
                {
                    enumerator2 = mainMap.ScriptAreas.GetEnumerator();
                    while (enumerator2.MoveNext())
                    {
                        clsMap.clsScriptArea area = (clsMap.clsScriptArea) enumerator2.Current;
                        num = this.lstScriptAreas.Items.Add(area.Label);
                        this.lstScriptAreas_Objects[num] = area;
                        if (area == this._SelectedScriptMarker)
                        {
                            obj2 = area;
                            this.lstScriptAreas.SelectedIndex = num;
                        }
                    }
                }
                finally
                {
                    if (enumerator2 is IDisposable)
                    {
                        (enumerator2 as IDisposable).Dispose();
                    }
                }
                this.lstScriptPositions.Enabled = true;
                this.lstScriptAreas.Enabled = true;
                this._SelectedScriptMarker = RuntimeHelpers.GetObjectValue(obj2);
                this.SelectedScriptMarker_Update();
            }
        }

        public void SelectedObject_Changed()
        {
            bool flag;
            clsMap.clsUnit current;
            IEnumerator enumerator;
            IEnumerator enumerator2;
            clsMap mainMap = this.MainMap;
            this.lblObjectType.Enabled = false;
            this.ObjectPlayerNum.Enabled = false;
            this.txtObjectRotation.Enabled = false;
            this.txtObjectID.Enabled = false;
            this.txtObjectLabel.Enabled = false;
            this.txtObjectPriority.Enabled = false;
            this.txtObjectHealth.Enabled = false;
            this.btnDroidToDesign.Enabled = false;
            this.cboDroidType.Enabled = false;
            this.cboDroidBody.Enabled = false;
            this.cboDroidPropulsion.Enabled = false;
            this.cboDroidTurret1.Enabled = false;
            this.cboDroidTurret2.Enabled = false;
            this.cboDroidTurret3.Enabled = false;
            this.rdoDroidTurret0.Enabled = false;
            this.rdoDroidTurret1.Enabled = false;
            this.rdoDroidTurret2.Enabled = false;
            this.rdoDroidTurret3.Enabled = false;
            if (mainMap == null)
            {
                flag = true;
            }
            else if (mainMap.SelectedUnits.Count == 0)
            {
                flag = true;
            }
            if (flag)
            {
                this.lblObjectType.Text = "";
                this.ObjectPlayerNum.Target.Item = null;
                this.txtObjectRotation.Text = "";
                this.txtObjectID.Text = "";
                this.txtObjectLabel.Text = "";
                this.txtObjectPriority.Text = "";
                this.txtObjectHealth.Text = "";
                this.cboDroidType.SelectedIndex = -1;
                this.cboDroidBody.SelectedIndex = -1;
                this.cboDroidPropulsion.SelectedIndex = -1;
                this.cboDroidTurret1.SelectedIndex = -1;
                this.cboDroidTurret2.SelectedIndex = -1;
                this.cboDroidTurret3.SelectedIndex = -1;
                this.rdoDroidTurret0.Checked = false;
                this.rdoDroidTurret1.Checked = false;
                this.rdoDroidTurret2.Checked = false;
                this.rdoDroidTurret3.Checked = false;
                return;
            }
            if (mainMap.SelectedUnits.Count <= 1)
            {
                if (mainMap.SelectedUnits.Count == 1)
                {
                    clsMap.clsUnit unit2 = mainMap.SelectedUnits[0];
                    this.lblObjectType.Text = unit2.Type.GetDisplayTextCode();
                    this.ObjectPlayerNum.Target.Item = unit2.UnitGroup;
                    this.txtObjectRotation.Text = modIO.InvariantToString_int(unit2.Rotation);
                    this.txtObjectID.Text = modIO.InvariantToString_uint(unit2.ID);
                    this.txtObjectPriority.Text = modIO.InvariantToString_int(unit2.SavePriority);
                    this.txtObjectHealth.Text = modIO.InvariantToString_dbl(unit2.Health * 100.0);
                    this.lblObjectType.Enabled = true;
                    this.ObjectPlayerNum.Enabled = true;
                    this.txtObjectRotation.Enabled = true;
                    this.txtObjectPriority.Enabled = true;
                    this.txtObjectHealth.Enabled = true;
                    bool flag3 = true;
                    if ((unit2.Type.Type == clsUnitType.enumType.PlayerStructure) && ((clsStructureType) unit2.Type).IsModule())
                    {
                        flag3 = false;
                    }
                    if (flag3)
                    {
                        this.txtObjectLabel.Text = unit2.Label;
                        this.txtObjectLabel.Enabled = true;
                    }
                    else
                    {
                        this.txtObjectLabel.Text = "";
                    }
                    bool flag2 = false;
                    if (unit2.Type.Type != clsUnitType.enumType.PlayerDroid)
                    {
                        flag2 = true;
                    }
                    else
                    {
                        clsDroidDesign type = (clsDroidDesign) unit2.Type;
                        if (type.IsTemplate)
                        {
                            this.btnDroidToDesign.Enabled = true;
                            flag2 = true;
                        }
                        else
                        {
                            int num2;
                            if (type.TemplateDroidType == null)
                            {
                                this.cboDroidType.SelectedIndex = -1;
                            }
                            else
                            {
                                this.cboDroidType.SelectedIndex = type.TemplateDroidType.Num;
                            }
                            if (type.Body == null)
                            {
                                this.cboDroidBody.SelectedIndex = -1;
                            }
                            else
                            {
                                int num4 = this.cboDroidBody.Items.Count - 1;
                                num2 = 0;
                                while (num2 <= num4)
                                {
                                    if (this.cboBody_Objects[num2] == type.Body)
                                    {
                                        break;
                                    }
                                    num2++;
                                }
                                if (num2 >= 0)
                                {
                                    if (num2 < this.cboDroidBody.Items.Count)
                                    {
                                        this.cboDroidBody.SelectedIndex = num2;
                                    }
                                    else
                                    {
                                        this.cboDroidBody.SelectedIndex = -1;
                                        this.cboDroidBody.Text = type.Body.Code;
                                    }
                                }
                            }
                            if (type.Propulsion == null)
                            {
                                this.cboDroidPropulsion.SelectedIndex = -1;
                            }
                            else
                            {
                                int num5 = this.cboDroidPropulsion.Items.Count - 1;
                                num2 = 0;
                                while (num2 <= num5)
                                {
                                    if (this.cboPropulsion_Objects[num2] == type.Propulsion)
                                    {
                                        break;
                                    }
                                    num2++;
                                }
                                if (num2 < this.cboDroidPropulsion.Items.Count)
                                {
                                    this.cboDroidPropulsion.SelectedIndex = num2;
                                }
                                else
                                {
                                    this.cboDroidPropulsion.SelectedIndex = -1;
                                    this.cboDroidPropulsion.Text = type.Propulsion.Code;
                                }
                            }
                            if (type.Turret1 == null)
                            {
                                this.cboDroidTurret1.SelectedIndex = -1;
                            }
                            else
                            {
                                int num6 = this.cboDroidTurret1.Items.Count - 1;
                                num2 = 0;
                                while (num2 <= num6)
                                {
                                    if (this.cboTurret_Objects[num2] == type.Turret1)
                                    {
                                        break;
                                    }
                                    num2++;
                                }
                                if (num2 < this.cboDroidTurret1.Items.Count)
                                {
                                    this.cboDroidTurret1.SelectedIndex = num2;
                                }
                                else
                                {
                                    this.cboDroidTurret1.SelectedIndex = -1;
                                    this.cboDroidTurret1.Text = type.Turret1.Code;
                                }
                            }
                            if (type.Turret2 == null)
                            {
                                this.cboDroidTurret2.SelectedIndex = -1;
                            }
                            else
                            {
                                int num7 = this.cboDroidTurret2.Items.Count - 1;
                                num2 = 0;
                                while (num2 <= num7)
                                {
                                    if (this.cboTurret_Objects[num2] == type.Turret2)
                                    {
                                        break;
                                    }
                                    num2++;
                                }
                                if (num2 < this.cboDroidTurret2.Items.Count)
                                {
                                    this.cboDroidTurret2.SelectedIndex = num2;
                                }
                                else
                                {
                                    this.cboDroidTurret2.SelectedIndex = -1;
                                    this.cboDroidTurret2.Text = type.Turret2.Code;
                                }
                            }
                            if (type.Turret3 == null)
                            {
                                this.cboDroidTurret3.SelectedIndex = -1;
                            }
                            else
                            {
                                int num8 = this.cboDroidTurret3.Items.Count - 1;
                                num2 = 0;
                                while (num2 <= num8)
                                {
                                    if (this.cboTurret_Objects[num2] == type.Turret3)
                                    {
                                        break;
                                    }
                                    num2++;
                                }
                                if (num2 < this.cboDroidTurret3.Items.Count)
                                {
                                    this.cboDroidTurret3.SelectedIndex = num2;
                                }
                                else
                                {
                                    this.cboDroidTurret3.SelectedIndex = -1;
                                    this.cboDroidTurret3.Text = type.Turret3.Code;
                                }
                            }
                            if (type.TurretCount == 3)
                            {
                                this.rdoDroidTurret0.Checked = false;
                                this.rdoDroidTurret1.Checked = false;
                                this.rdoDroidTurret2.Checked = false;
                                this.rdoDroidTurret3.Checked = true;
                            }
                            else if (type.TurretCount == 2)
                            {
                                this.rdoDroidTurret0.Checked = false;
                                this.rdoDroidTurret1.Checked = false;
                                this.rdoDroidTurret2.Checked = true;
                                this.rdoDroidTurret3.Checked = false;
                            }
                            else if (type.TurretCount == 1)
                            {
                                this.rdoDroidTurret0.Checked = false;
                                this.rdoDroidTurret1.Checked = true;
                                this.rdoDroidTurret2.Checked = false;
                                this.rdoDroidTurret3.Checked = false;
                            }
                            else if (type.TurretCount == 0)
                            {
                                this.rdoDroidTurret0.Checked = true;
                                this.rdoDroidTurret1.Checked = false;
                                this.rdoDroidTurret2.Checked = false;
                                this.rdoDroidTurret3.Checked = false;
                            }
                            else
                            {
                                this.rdoDroidTurret0.Checked = false;
                                this.rdoDroidTurret1.Checked = false;
                                this.rdoDroidTurret2.Checked = false;
                                this.rdoDroidTurret3.Checked = false;
                            }
                            this.cboDroidType.Enabled = true;
                            this.cboDroidBody.Enabled = true;
                            this.cboDroidPropulsion.Enabled = true;
                            this.cboDroidTurret1.Enabled = true;
                            this.cboDroidTurret2.Enabled = true;
                            this.cboDroidTurret3.Enabled = true;
                            this.rdoDroidTurret0.Enabled = true;
                            this.rdoDroidTurret1.Enabled = true;
                            this.rdoDroidTurret2.Enabled = true;
                            this.rdoDroidTurret3.Enabled = true;
                        }
                    }
                    if (flag2)
                    {
                        this.cboDroidType.SelectedIndex = -1;
                        this.cboDroidBody.SelectedIndex = -1;
                        this.cboDroidPropulsion.SelectedIndex = -1;
                        this.cboDroidTurret1.SelectedIndex = -1;
                        this.cboDroidTurret2.SelectedIndex = -1;
                        this.cboDroidTurret3.SelectedIndex = -1;
                        this.rdoDroidTurret1.Checked = false;
                        this.rdoDroidTurret2.Checked = false;
                        this.rdoDroidTurret3.Checked = false;
                    }
                    unit2 = null;
                }
                return;
            }
            this.lblObjectType.Text = "Multiple objects";
            clsMap.clsUnitGroup unitGroup = mainMap.SelectedUnits[0].UnitGroup;
            int num3 = mainMap.SelectedUnits.Count - 1;
            int num = 1;
            while (num <= num3)
            {
                if (mainMap.SelectedUnits[num].UnitGroup != unitGroup)
                {
                    break;
                }
                num++;
            }
            if (num == mainMap.SelectedUnits.Count)
            {
                this.ObjectPlayerNum.Target.Item = unitGroup;
            }
            else
            {
                this.ObjectPlayerNum.Target.Item = null;
            }
            this.txtObjectRotation.Text = "";
            this.txtObjectID.Text = "";
            this.txtObjectLabel.Text = "";
            this.lblObjectType.Enabled = true;
            this.ObjectPlayerNum.Enabled = true;
            this.txtObjectRotation.Enabled = true;
            this.txtObjectPriority.Text = "";
            this.txtObjectPriority.Enabled = true;
            this.txtObjectHealth.Text = "";
            this.txtObjectHealth.Enabled = true;
            try
            {
                enumerator = mainMap.SelectedUnits.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    current = (clsMap.clsUnit) enumerator.Current;
                    if ((current.Type.Type == clsUnitType.enumType.PlayerDroid) && ((clsDroidDesign) current.Type).IsTemplate)
                    {
                        goto Label_036F;
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
        Label_036F:
            if (num < mainMap.SelectedUnits.Count)
            {
                this.btnDroidToDesign.Enabled = true;
            }
            try
            {
                enumerator2 = mainMap.SelectedUnits.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    current = (clsMap.clsUnit) enumerator2.Current;
                    if ((current.Type.Type == clsUnitType.enumType.PlayerDroid) && !((clsDroidDesign) current.Type).IsTemplate)
                    {
                        goto Label_03E8;
                    }
                }
            }
            finally
            {
                if (enumerator2 is IDisposable)
                {
                    (enumerator2 as IDisposable).Dispose();
                }
            }
        Label_03E8:
            if (num < mainMap.SelectedUnits.Count)
            {
                this.cboDroidType.SelectedIndex = -1;
                this.cboDroidBody.SelectedIndex = -1;
                this.cboDroidPropulsion.SelectedIndex = -1;
                this.cboDroidTurret1.SelectedIndex = -1;
                this.cboDroidTurret2.SelectedIndex = -1;
                this.cboDroidTurret3.SelectedIndex = -1;
                this.rdoDroidTurret1.Checked = false;
                this.rdoDroidTurret2.Checked = false;
                this.rdoDroidTurret3.Checked = false;
                this.cboDroidType.Enabled = true;
                this.cboDroidBody.Enabled = true;
                this.cboDroidPropulsion.Enabled = true;
                this.cboDroidTurret1.Enabled = true;
                this.cboDroidTurret2.Enabled = true;
                this.cboDroidTurret3.Enabled = true;
                this.rdoDroidTurret0.Enabled = true;
                this.rdoDroidTurret1.Enabled = true;
                this.rdoDroidTurret2.Enabled = true;
                this.rdoDroidTurret3.Enabled = true;
            }
        }

        private void SelectedObjects_SetDroidType(clsDroidDesign.clsTemplateDroidType NewType)
        {
            clsMap mainMap = this.MainMap;
            if (mainMap != null)
            {
                clsMap.clsObjectDroidType tool = new clsMap.clsObjectDroidType {
                    Map = mainMap,
                    DroidType = NewType
                };
                mainMap.SelectedUnitsAction(tool);
                this.SelectedObject_Changed();
                if (tool.ActionPerformed)
                {
                    mainMap.UndoStepCreate("Object Number Of Turrets Changed");
                    this.View_DrawViewLater();
                }
            }
        }

        private void SelectedObjects_SetTurretCount(byte Count)
        {
            clsMap mainMap = this.MainMap;
            if (mainMap != null)
            {
                clsMap.clsObjectTurretCount tool = new clsMap.clsObjectTurretCount {
                    Map = mainMap,
                    TurretCount = Count
                };
                mainMap.SelectedUnitsAction(tool);
                this.SelectedObject_Changed();
                if (tool.ActionPerformed)
                {
                    mainMap.UndoStepCreate("Object Number Of Turrets Changed");
                    this.View_DrawViewLater();
                }
            }
        }

        public void SelectedScriptMarker_Update()
        {
            this.txtScriptMarkerLabel.Enabled = false;
            this.txtScriptMarkerX.Enabled = false;
            this.txtScriptMarkerY.Enabled = false;
            this.txtScriptMarkerX2.Enabled = false;
            this.txtScriptMarkerY2.Enabled = false;
            this.txtScriptMarkerLabel.Text = "";
            this.txtScriptMarkerX.Text = "";
            this.txtScriptMarkerY.Text = "";
            this.txtScriptMarkerX2.Text = "";
            this.txtScriptMarkerY2.Text = "";
            if (this._SelectedScriptMarker != null)
            {
                if (this._SelectedScriptMarker is clsMap.clsScriptPosition)
                {
                    clsMap.clsScriptPosition position = (clsMap.clsScriptPosition) this._SelectedScriptMarker;
                    this.txtScriptMarkerLabel.Text = position.Label;
                    this.txtScriptMarkerX.Text = modIO.InvariantToString_int(position.PosX);
                    this.txtScriptMarkerY.Text = modIO.InvariantToString_int(position.PosY);
                    this.txtScriptMarkerLabel.Enabled = true;
                    this.txtScriptMarkerX.Enabled = true;
                    this.txtScriptMarkerY.Enabled = true;
                }
                else if (this._SelectedScriptMarker is clsMap.clsScriptArea)
                {
                    clsMap.clsScriptArea area = (clsMap.clsScriptArea) this._SelectedScriptMarker;
                    this.txtScriptMarkerLabel.Text = area.Label;
                    this.txtScriptMarkerX.Text = modIO.InvariantToString_int(area.PosAX);
                    this.txtScriptMarkerY.Text = modIO.InvariantToString_int(area.PosAY);
                    this.txtScriptMarkerX2.Text = modIO.InvariantToString_int(area.PosBX);
                    this.txtScriptMarkerY2.Text = modIO.InvariantToString_int(area.PosBY);
                    this.txtScriptMarkerLabel.Enabled = true;
                    this.txtScriptMarkerX.Enabled = true;
                    this.txtScriptMarkerY.Enabled = true;
                    this.txtScriptMarkerX2.Enabled = true;
                    this.txtScriptMarkerY2.Enabled = true;
                }
            }
        }

        public void SetMainMap(clsMap Map)
        {
            this._LoadedMaps.MainMap = Map;
        }

        private void ShowThreadedSplashScreen()
        {
            clsSplashScreen screen = new clsSplashScreen();
            screen.Form.Show();
            screen.Form.Activate();
            while (!modProgram.ProgramInitializeFinished)
            {
                screen.Form.lblStatus.Text = this.InitializeStatus;
                Application.DoEvents();
                Thread.Sleep(200);
            }
            screen.Form.Close();
        }

        private void TabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.TabControl.SelectedTab == this.tpTextures)
            {
                modTools.Tool = modTools.Tools.TextureBrush;
                this.TextureView.DrawViewLater();
            }
            else if (this.TabControl.SelectedTab == this.tpHeight)
            {
                if (this.rdoHeightSet.Checked)
                {
                    modTools.Tool = modTools.Tools.HeightSetBrush;
                }
                else if (this.rdoHeightSmooth.Checked)
                {
                    modTools.Tool = modTools.Tools.HeightSmoothBrush;
                }
                else if (this.rdoHeightChange.Checked)
                {
                    modTools.Tool = modTools.Tools.HeightChangeBrush;
                }
            }
            else if (this.TabControl.SelectedTab == this.tpAutoTexture)
            {
                if (this.rdoAutoTexturePlace.Checked)
                {
                    modTools.Tool = modTools.Tools.TerrainBrush;
                }
                else if (this.rdoAutoRoadPlace.Checked)
                {
                    modTools.Tool = modTools.Tools.RoadPlace;
                }
                else if (this.rdoCliffTriBrush.Checked)
                {
                    modTools.Tool = modTools.Tools.CliffTriangle;
                }
                else if (this.rdoAutoCliffBrush.Checked)
                {
                    modTools.Tool = modTools.Tools.CliffBrush;
                }
                else if (this.rdoAutoCliffRemove.Checked)
                {
                    modTools.Tool = modTools.Tools.CliffRemove;
                }
                else if (this.rdoAutoTextureFill.Checked)
                {
                    modTools.Tool = modTools.Tools.TerrainFill;
                }
                else if (this.rdoAutoRoadLine.Checked)
                {
                    modTools.Tool = modTools.Tools.RoadLines;
                }
                else if (this.rdoRoadRemove.Checked)
                {
                    modTools.Tool = modTools.Tools.RoadRemove;
                }
                else
                {
                    modTools.Tool = modTools.Tools.ObjectSelect;
                }
            }
            else if (this.TabControl.SelectedTab == this.tpObjects)
            {
                this.ObjectsUpdate();
                if (this.rdoObjectPlace.Checked)
                {
                    modTools.Tool = modTools.Tools.ObjectPlace;
                }
                else if (this.rdoObjectLines.Checked)
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

        private void TabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.ActivateObjectTool();
            this.ObjectsUpdate();
        }

        private void tabHeightSetL_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.txtHeightSetL.Text = modIO.InvariantToString_byte(this.HeightSetPalette[this.tabHeightSetL.SelectedIndex]);
        }

        private void tabHeightSetR_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.txtHeightSetR.Text = modIO.InvariantToString_byte(this.HeightSetPalette[this.tabHeightSetR.SelectedIndex]);
        }

        private void tabPlayerNum_SelectedIndexChanged()
        {
            if (this.ObjectPlayerNum.Enabled)
            {
                clsMap mainMap = this.MainMap;
                if ((((mainMap != null) && (mainMap.SelectedUnits.Count > 0)) && (this.ObjectPlayerNum.Target.Item != null)) && ((mainMap.SelectedUnits.Count <= 1) || (Interaction.MsgBox("Change player of multiple objects?", MsgBoxStyle.OkCancel, "") == MsgBoxResult.Ok)))
                {
                    clsMap.clsObjectUnitGroup tool = new clsMap.clsObjectUnitGroup {
                        Map = mainMap,
                        UnitGroup = this.ObjectPlayerNum.Target.Item
                    };
                    mainMap.SelectedUnitsAction(tool);
                    this.SelectedObject_Changed();
                    mainMap.UndoStepCreate("Object Player Changed");
                    if (modSettings.Settings.MinimapTeamColours)
                    {
                        mainMap.MinimapMakeLater();
                    }
                    this.View_DrawViewLater();
                }
            }
        }

        public void TerrainPicker()
        {
            clsMap mainMap = this.MainMap;
            clsViewInfo.clsMouseOver.clsOverTerrain mouseOverTerrain = mainMap.ViewInfo.GetMouseOverTerrain();
            if (mouseOverTerrain != null)
            {
                modMath.sXY_int normal = mouseOverTerrain.Vertex.Normal;
                this.lstAutoTexture.Enabled = false;
                int num2 = this.lstAutoTexture.Items.Count - 1;
                int index = 0;
                while (index <= num2)
                {
                    if (mainMap.Painter.Terrains[index] == mainMap.Terrain.Vertices[normal.X, normal.Y].Terrain)
                    {
                        this.lstAutoTexture.SelectedIndex = index;
                        break;
                    }
                    index++;
                }
                if (index == this.lstAutoTexture.Items.Count)
                {
                    this.lstAutoTexture.SelectedIndex = -1;
                }
                this.lstAutoTexture.Enabled = true;
                modProgram.SelectedTerrain = mainMap.Terrain.Vertices[normal.X, normal.Y].Terrain;
            }
        }

        public void TexturePicker()
        {
            clsMap mainMap = this.MainMap;
            clsViewInfo.clsMouseOver.clsOverTerrain mouseOverTerrain = mainMap.ViewInfo.GetMouseOverTerrain();
            if (mouseOverTerrain != null)
            {
                modMath.sXY_int normal = mouseOverTerrain.Tile.Normal;
                if ((mainMap.Tileset != null) && (mainMap.Terrain.Tiles[normal.X, normal.Y].Texture.TextureNum < mainMap.Tileset.TileCount))
                {
                    modProgram.SelectedTextureNum = mainMap.Terrain.Tiles[normal.X, normal.Y].Texture.TextureNum;
                    this.TextureView.DrawViewLater();
                }
                if (modSettings.Settings.PickOrientation)
                {
                    modProgram.TextureOrientation = mainMap.Terrain.Tiles[normal.X, normal.Y].Texture.Orientation;
                    this.TextureView.DrawViewLater();
                }
            }
        }

        public void TitleTextUpdate()
        {
            string fileTitleWithoutExtension;
            clsMap mainMap = this.MainMap;
            this.menuSaveFMapQuick.Text = "Quick Save fmap";
            this.menuSaveFMapQuick.Enabled = false;
            if (mainMap == null)
            {
                fileTitleWithoutExtension = "No Map";
                this.tsbSave.ToolTipText = "No Map";
            }
            else
            {
                if (mainMap.PathInfo == null)
                {
                    fileTitleWithoutExtension = "Unsaved map";
                    this.tsbSave.ToolTipText = "Save FMap...";
                }
                else
                {
                    modProgram.sSplitPath path = new modProgram.sSplitPath(mainMap.PathInfo.Path);
                    if (mainMap.PathInfo.IsFMap)
                    {
                        ToolStripMenuItem menuSaveFMapQuick;
                        fileTitleWithoutExtension = path.FileTitleWithoutExtension;
                        string str2 = mainMap.PathInfo.Path;
                        this.tsbSave.ToolTipText = "Quick save FMap to \"" + str2 + "\"";
                        this.menuSaveFMapQuick.Text = "Quick Save fmap to \"";
                        if (str2.Length <= 0x20)
                        {
                            menuSaveFMapQuick = this.menuSaveFMapQuick;
                            menuSaveFMapQuick.Text = menuSaveFMapQuick.Text + str2;
                        }
                        else
                        {
                            menuSaveFMapQuick = this.menuSaveFMapQuick;
                            menuSaveFMapQuick.Text = menuSaveFMapQuick.Text + str2.Substring(0, 10) + "..." + str2.Substring(str2.Length - 20, 20);
                        }
                        menuSaveFMapQuick = this.menuSaveFMapQuick;
                        menuSaveFMapQuick.Text = menuSaveFMapQuick.Text + "\"";
                        this.menuSaveFMapQuick.Enabled = true;
                    }
                    else
                    {
                        fileTitleWithoutExtension = path.FileTitle;
                        this.tsbSave.ToolTipText = "Save FMap...";
                    }
                }
                mainMap.SetTabText();
            }
            this.Text = fileTitleWithoutExtension + " - FlaME 1.29";
        }

        private void tmrKey_Tick(object sender, EventArgs e)
        {
            if (modProgram.ProgramInitialized)
            {
                clsMap mainMap = this.MainMap;
                if (mainMap != null)
                {
                    double num4;
                    if (modControls.KeyboardProfile.Active(modControls.Control_Fast))
                    {
                        if (modControls.KeyboardProfile.Active(modControls.Control_Slow))
                        {
                            num4 = 8.0;
                        }
                        else
                        {
                            num4 = 4.0;
                        }
                    }
                    else if (modControls.KeyboardProfile.Active(modControls.Control_Slow))
                    {
                        num4 = 0.25;
                    }
                    else
                    {
                        num4 = 1.0;
                    }
                    double zoom = this.tmrKey.Interval * 0.002;
                    double move = (this.tmrKey.Interval * num4) / 2048.0;
                    double roll = 0.087266462599716474;
                    double pan = 0.0625;
                    double orbitRate = 0.03125;
                    mainMap.ViewInfo.TimedActions(zoom, move, pan, roll, orbitRate);
                    if (mainMap.CheckMessages())
                    {
                        this.View_DrawViewLater();
                    }
                }
            }
        }

        private void tmrTool_Tick(object sender, EventArgs e)
        {
            if (modProgram.ProgramInitialized)
            {
                clsMap mainMap = this.MainMap;
                if (mainMap != null)
                {
                    mainMap.ViewInfo.TimedTools();
                }
            }
        }

        private void ToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            this.Save_Heightmap_Prompt();
        }

        private void ToolStripMenuItem5_Click(object sender, EventArgs e)
        {
            clsMap mainMap = this.MainMap;
            if (mainMap != null)
            {
                if (modGenerator.UnitType_OilResource == null)
                {
                    Interaction.MsgBox("Unable. Oil resource is not loaded.", MsgBoxStyle.ApplicationModal, null);
                }
                else
                {
                    IEnumerator enumerator;
                    modLists.SimpleClassList<clsMap.clsUnit> list = new modLists.SimpleClassList<clsMap.clsUnit>();
                    try
                    {
                        enumerator = mainMap.Units.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            clsMap.clsUnit current = (clsMap.clsUnit) enumerator.Current;
                            if (current.Type == modGenerator.UnitType_OilResource)
                            {
                                list.Add(current);
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
                    clsMap.clsObjectFlattenTerrain tool = new clsMap.clsObjectFlattenTerrain();
                    list.PerformTool(tool);
                    mainMap.Update();
                    mainMap.UndoStepCreate("Flatten Under Oil");
                }
            }
        }

        private void ToolStripMenuItem6_Click(object sender, EventArgs e)
        {
            clsMap mainMap = this.MainMap;
            if (mainMap != null)
            {
                IEnumerator enumerator;
                modLists.SimpleClassList<clsMap.clsUnit> list = new modLists.SimpleClassList<clsMap.clsUnit>();
                try
                {
                    enumerator = mainMap.Units.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        clsMap.clsUnit current = (clsMap.clsUnit) enumerator.Current;
                        if (current.Type.Type == clsUnitType.enumType.PlayerStructure)
                        {
                            list.Add(current);
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
                clsMap.clsObjectFlattenTerrain tool = new clsMap.clsObjectFlattenTerrain();
                list.PerformTool(tool);
                mainMap.Update();
                mainMap.UndoStepCreate("Flatten Under Structures");
            }
        }

        private void tsbDrawAutotexture_Click(object sender, EventArgs e)
        {
            if ((this.MapView != null) && (modProgram.Draw_VertexTerrain != this.tsbDrawAutotexture.Checked))
            {
                modProgram.Draw_VertexTerrain = this.tsbDrawAutotexture.Checked;
                this.View_DrawViewLater();
            }
        }

        private void tsbDrawTileOrientation_Click(object sender, EventArgs e)
        {
            if ((this.MapView != null) && (modProgram.DisplayTileOrientation != this.tsbDrawTileOrientation.Checked))
            {
                modProgram.DisplayTileOrientation = this.tsbDrawTileOrientation.Checked;
                this.View_DrawViewLater();
                this.TextureView.DrawViewLater();
            }
        }

        private void tsbGateways_Click(object sender, EventArgs e)
        {
            if (modTools.Tool == modTools.Tools.Gateways)
            {
                modProgram.Draw_Gateways = false;
                modTools.Tool = modTools.Tools.ObjectSelect;
                this.tsbGateways.Checked = false;
            }
            else
            {
                modProgram.Draw_Gateways = true;
                modTools.Tool = modTools.Tools.Gateways;
                this.tsbGateways.Checked = true;
            }
            clsMap mainMap = this.MainMap;
            if (mainMap != null)
            {
                mainMap.Selected_Tile_A = null;
                mainMap.Selected_Tile_B = null;
                this.View_DrawViewLater();
            }
        }

        private void tsbSave_Click(object sender, EventArgs e)
        {
            this.QuickSave();
        }

        private void tsbSelection_Click(object sender, EventArgs e)
        {
            modTools.Tool = modTools.Tools.TerrainSelect;
        }

        private void tsbSelectionCopy_Click(object sender, EventArgs e)
        {
            clsMap mainMap = this.MainMap;
            if ((mainMap != null) && !((mainMap.Selected_Area_VertexA == null) | (mainMap.Selected_Area_VertexB == null)))
            {
                modMath.sXY_int _int;
                modMath.sXY_int _int2;
                modMath.sXY_int _int3;
                if (modProgram.Copied_Map != null)
                {
                    modProgram.Copied_Map.Deallocate();
                }
                modMath.ReorderXY(mainMap.Selected_Area_VertexA.XY, mainMap.Selected_Area_VertexB.XY, ref _int3, ref _int2);
                _int.X = _int2.X - _int3.X;
                _int.Y = _int2.Y - _int3.Y;
                modProgram.Copied_Map = new clsMap(mainMap, _int3, _int);
            }
        }

        private void tsbSelectionFlipX_Click(object sender, EventArgs e)
        {
            if (modProgram.Copied_Map == null)
            {
                Interaction.MsgBox("Nothing to flip.", MsgBoxStyle.ApplicationModal, null);
            }
            else
            {
                modProgram.Copied_Map.Rotate(TileOrientation.Orientation_FlipX, modMain.frmMainInstance.PasteRotateObjects);
            }
        }

        private void tsbSelectionObjects_Click(object sender, EventArgs e)
        {
            clsMap mainMap = this.MainMap;
            if ((mainMap != null) && !((mainMap.Selected_Area_VertexA == null) | (mainMap.Selected_Area_VertexB == null)))
            {
                modMath.sXY_int _int;
                modMath.sXY_int _int2;
                modMath.ReorderXY(mainMap.Selected_Area_VertexA.XY, mainMap.Selected_Area_VertexB.XY, ref _int2, ref _int);
                int num2 = mainMap.Units.Count - 1;
                for (int i = 0; i <= num2; i++)
                {
                    if (modProgram.PosIsWithinTileArea(mainMap.Units[i].Pos.Horizontal, _int2, _int) && !mainMap.Units[i].MapSelectedUnitLink.IsConnected)
                    {
                        mainMap.Units[i].MapSelectedUnitLink.Connect(mainMap.SelectedUnits);
                    }
                }
                this.SelectedObject_Changed();
                modTools.Tool = modTools.Tools.ObjectSelect;
                this.View_DrawViewLater();
            }
        }

        private void tsbSelectionPaste_Click(object sender, EventArgs e)
        {
            clsMap mainMap = this.MainMap;
            if ((mainMap != null) && !((mainMap.Selected_Area_VertexA == null) | (mainMap.Selected_Area_VertexB == null)))
            {
                if (modProgram.Copied_Map == null)
                {
                    Interaction.MsgBox("Nothing to paste.", MsgBoxStyle.ApplicationModal, null);
                }
                else if (((((this.menuSelPasteHeights.Checked | this.menuSelPasteTextures.Checked) | this.menuSelPasteUnits.Checked) | this.menuSelPasteDeleteUnits.Checked) | this.menuSelPasteGateways.Checked) | this.menuSelPasteDeleteGateways.Checked)
                {
                    modMath.sXY_int _int;
                    modMath.sXY_int _int2;
                    modMath.sXY_int _int3;
                    modMath.ReorderXY(mainMap.Selected_Area_VertexA.XY, mainMap.Selected_Area_VertexB.XY, ref _int3, ref _int2);
                    _int.X = _int2.X - _int3.X;
                    _int.Y = _int2.Y - _int3.Y;
                    mainMap.MapInsert(modProgram.Copied_Map, _int3, _int, this.menuSelPasteHeights.Checked, this.menuSelPasteTextures.Checked, this.menuSelPasteUnits.Checked, this.menuSelPasteDeleteUnits.Checked, this.menuSelPasteGateways.Checked, this.menuSelPasteDeleteGateways.Checked);
                    this.SelectedObject_Changed();
                    mainMap.UndoStepCreate("Paste");
                    this.View_DrawViewLater();
                }
            }
        }

        private void tsbSelectionRotateAnticlockwise_Click(object sender, EventArgs e)
        {
            if (modProgram.Copied_Map == null)
            {
                Interaction.MsgBox("Nothing to rotate.", MsgBoxStyle.ApplicationModal, null);
            }
            else
            {
                modProgram.Copied_Map.Rotate(TileOrientation.Orientation_CounterClockwise, modMain.frmMainInstance.PasteRotateObjects);
            }
        }

        private void tsbSelectionRotateClockwise_Click(object sender, EventArgs e)
        {
            if (modProgram.Copied_Map == null)
            {
                Interaction.MsgBox("Nothing to rotate.", MsgBoxStyle.ApplicationModal, null);
            }
            else
            {
                modProgram.Copied_Map.Rotate(TileOrientation.Orientation_Clockwise, modMain.frmMainInstance.PasteRotateObjects);
            }
        }

        private void txtHeightSetL_LostFocus(object sender, EventArgs e)
        {
            double num2;
            if (modIO.InvariantParse_dbl(this.txtHeightSetL.Text, ref num2))
            {
                byte num = (byte) Math.Round(modMath.Clamp_dbl(num2, 0.0, 255.0));
                this.HeightSetPalette[this.tabHeightSetL.SelectedIndex] = num;
                if (this.tabHeightSetL.SelectedIndex == this.tabHeightSetL.SelectedIndex)
                {
                    this.tabHeightSetL_SelectedIndexChanged(null, null);
                }
                string str = modIO.InvariantToString_byte(num);
                this.tabHeightSetL.TabPages[this.tabHeightSetL.SelectedIndex].Text = str;
                this.tabHeightSetR.TabPages[this.tabHeightSetL.SelectedIndex].Text = str;
            }
        }

        private void txtHeightSetR_LostFocus(object sender, EventArgs e)
        {
            double num2;
            if (modIO.InvariantParse_dbl(this.txtHeightSetL.Text, ref num2))
            {
                byte num = (byte) Math.Round(modMath.Clamp_dbl(num2, 0.0, 255.0));
                this.HeightSetPalette[this.tabHeightSetR.SelectedIndex] = num;
                if (this.tabHeightSetL.SelectedIndex == this.tabHeightSetR.SelectedIndex)
                {
                    this.tabHeightSetL_SelectedIndexChanged(null, null);
                }
                string str = modIO.InvariantToString_byte(num);
                this.tabHeightSetL.TabPages[this.tabHeightSetR.SelectedIndex].Text = str;
                this.tabHeightSetR.TabPages[this.tabHeightSetR.SelectedIndex].Text = str;
            }
        }

        private void txtObjectFind_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.txtObjectFind.Enabled && (e.KeyCode == Keys.Enter))
            {
                this.txtObjectFind.Enabled = false;
                this.ObjectsUpdate();
                this.txtObjectFind.Enabled = true;
            }
        }

        private void txtObjectFind_Leave(object sender, EventArgs e)
        {
            if (this.txtObjectFind.Enabled)
            {
                this.txtObjectFind.Enabled = false;
                this.ObjectsUpdate();
                this.txtObjectFind.Enabled = true;
            }
        }

        private void txtObjectHealth_LostFocus(object sender, EventArgs e)
        {
            if (this.txtObjectHealth.Enabled)
            {
                double num;
                clsMap mainMap = this.MainMap;
                if (((mainMap != null) && (mainMap.SelectedUnits.Count > 0)) && modIO.InvariantParse_dbl(this.txtObjectHealth.Text, ref num))
                {
                    num = modMath.Clamp_dbl(num, 1.0, 100.0) / 100.0;
                    if ((mainMap.SelectedUnits.Count <= 1) || (Interaction.MsgBox("Change health of multiple objects?", MsgBoxStyle.Question | MsgBoxStyle.OkCancel, "") == MsgBoxResult.Ok))
                    {
                        clsMap.clsObjectHealth tool = new clsMap.clsObjectHealth {
                            Map = mainMap,
                            Health = num
                        };
                        mainMap.SelectedUnitsAction(tool);
                        this.SelectedObject_Changed();
                        mainMap.UndoStepCreate("Object Health Changed");
                        this.View_DrawViewLater();
                    }
                }
            }
        }

        private void txtObjectLabel_LostFocus(object sender, EventArgs e)
        {
            if (this.txtObjectLabel.Enabled)
            {
                clsMap mainMap = this.MainMap;
                if (((mainMap != null) && (mainMap.SelectedUnits.Count == 1)) && (this.txtObjectLabel.Text != mainMap.SelectedUnits[0].Label))
                {
                    clsMap.clsUnit unitToCopy = mainMap.SelectedUnits[0];
                    clsMap.clsUnit newUnit = new clsMap.clsUnit(unitToCopy, mainMap);
                    mainMap.UnitSwap(unitToCopy, newUnit);
                    modProgram.sResult result = newUnit.SetLabel(this.txtObjectLabel.Text);
                    if (!result.Success)
                    {
                        Interaction.MsgBox("Unable to set label: " + result.Problem, MsgBoxStyle.ApplicationModal, null);
                    }
                    mainMap.SelectedUnits.Clear();
                    newUnit.MapSelect();
                    this.SelectedObject_Changed();
                    mainMap.UndoStepCreate("Object Label Changed");
                    this.View_DrawViewLater();
                }
            }
        }

        private void txtObjectPriority_LostFocus(object sender, EventArgs e)
        {
            if (this.txtObjectPriority.Enabled)
            {
                int num;
                clsMap mainMap = this.MainMap;
                if (((mainMap != null) && (mainMap.SelectedUnits.Count > 0)) && modIO.InvariantParse_int(this.txtObjectPriority.Text, ref num))
                {
                    if (mainMap.SelectedUnits.Count > 1)
                    {
                        if (Interaction.MsgBox("Change priority of multiple objects?", MsgBoxStyle.Question | MsgBoxStyle.OkCancel, "") != MsgBoxResult.Ok)
                        {
                            return;
                        }
                    }
                    else if ((mainMap.SelectedUnits.Count == 1) && (num == mainMap.SelectedUnits[0].SavePriority))
                    {
                        return;
                    }
                    clsMap.clsObjectPriority tool = new clsMap.clsObjectPriority {
                        Map = mainMap,
                        Priority = num
                    };
                    mainMap.SelectedUnitsAction(tool);
                    this.SelectedObject_Changed();
                    mainMap.UndoStepCreate("Object Priority Changed");
                    this.View_DrawViewLater();
                }
            }
        }

        private void txtObjectRotation_LostFocus(object sender, EventArgs e)
        {
            if (this.txtObjectRotation.Enabled && (this.txtObjectRotation.Text != ""))
            {
                int num;
                clsMap mainMap = this.MainMap;
                if (((mainMap != null) && (mainMap.SelectedUnits.Count > 0)) && modIO.InvariantParse_int(this.txtObjectRotation.Text, ref num))
                {
                    num = modMath.Clamp_int(num, 0, 0x167);
                    if ((mainMap.SelectedUnits.Count <= 1) || (Interaction.MsgBox("Change rotation of multiple objects?", MsgBoxStyle.OkCancel, "") == MsgBoxResult.Ok))
                    {
                        clsMap.clsObjectRotation tool = new clsMap.clsObjectRotation {
                            Map = mainMap,
                            Angle = num
                        };
                        mainMap.SelectedUnitsAction(tool);
                        mainMap.Update();
                        this.SelectedObject_Changed();
                        mainMap.UndoStepCreate("Object Rotated");
                        this.View_DrawViewLater();
                    }
                }
            }
        }

        private void txtScriptMarkerLabel_LostFocus(object sender, EventArgs e)
        {
            if (this.txtScriptMarkerLabel.Enabled && (this._SelectedScriptMarker != null))
            {
                modProgram.sResult result;
                if (this._SelectedScriptMarker is clsMap.clsScriptPosition)
                {
                    clsMap.clsScriptPosition position = (clsMap.clsScriptPosition) this._SelectedScriptMarker;
                    if (position.Label == this.txtScriptMarkerLabel.Text)
                    {
                        return;
                    }
                    result = position.SetLabel(this.txtScriptMarkerLabel.Text);
                }
                else
                {
                    if (!(this._SelectedScriptMarker is clsMap.clsScriptArea))
                    {
                        return;
                    }
                    clsMap.clsScriptArea area = (clsMap.clsScriptArea) this._SelectedScriptMarker;
                    if (area.Label == this.txtScriptMarkerLabel.Text)
                    {
                        return;
                    }
                    result = area.SetLabel(this.txtScriptMarkerLabel.Text);
                }
                if (!result.Success)
                {
                    Interaction.MsgBox("Unable to change label: " + result.Problem, MsgBoxStyle.ApplicationModal, null);
                    this.SelectedScriptMarker_Update();
                }
                else
                {
                    this.ScriptMarkerLists_Update();
                }
            }
        }

        private void txtScriptMarkerX_LostFocus(object sender, EventArgs e)
        {
            if (this.txtScriptMarkerX.Enabled && (this._SelectedScriptMarker != null))
            {
                int posX;
                if (this._SelectedScriptMarker is clsMap.clsScriptPosition)
                {
                    clsMap.clsScriptPosition position2 = (clsMap.clsScriptPosition) this._SelectedScriptMarker;
                    posX = position2.PosX;
                    modIO.InvariantParse_int(this.txtScriptMarkerX.Text, ref posX);
                    position2.PosX = posX;
                }
                else if (this._SelectedScriptMarker is clsMap.clsScriptArea)
                {
                    clsMap.clsScriptArea area2 = (clsMap.clsScriptArea) this._SelectedScriptMarker;
                    posX = area2.PosAX;
                    modIO.InvariantParse_int(this.txtScriptMarkerX.Text, ref posX);
                    area2.PosAX = posX;
                }
                else
                {
                    Interaction.MsgBox("Error: unhandled type.", MsgBoxStyle.ApplicationModal, null);
                }
                this.SelectedScriptMarker_Update();
                this.View_DrawViewLater();
            }
        }

        private void txtScriptMarkerX2_LostFocus(object sender, EventArgs e)
        {
            if (this.txtScriptMarkerX2.Enabled && (this._SelectedScriptMarker != null))
            {
                if (this._SelectedScriptMarker is clsMap.clsScriptArea)
                {
                    clsMap.clsScriptArea area2 = (clsMap.clsScriptArea) this._SelectedScriptMarker;
                    int posBX = area2.PosBX;
                    modIO.InvariantParse_int(this.txtScriptMarkerX2.Text, ref posBX);
                    area2.PosBX = posBX;
                }
                else
                {
                    Interaction.MsgBox("Error: unhandled type.", MsgBoxStyle.ApplicationModal, null);
                }
                this.SelectedScriptMarker_Update();
                this.View_DrawViewLater();
            }
        }

        private void txtScriptMarkerY_LostFocus(object sender, EventArgs e)
        {
            if (this.txtScriptMarkerY.Enabled && (this._SelectedScriptMarker != null))
            {
                int posY;
                if (this._SelectedScriptMarker is clsMap.clsScriptPosition)
                {
                    clsMap.clsScriptPosition position2 = (clsMap.clsScriptPosition) this._SelectedScriptMarker;
                    posY = position2.PosY;
                    modIO.InvariantParse_int(this.txtScriptMarkerY.Text, ref posY);
                    position2.PosY = posY;
                }
                else if (this._SelectedScriptMarker is clsMap.clsScriptArea)
                {
                    clsMap.clsScriptArea area2 = (clsMap.clsScriptArea) this._SelectedScriptMarker;
                    posY = area2.PosAY;
                    modIO.InvariantParse_int(this.txtScriptMarkerY.Text, ref posY);
                    area2.PosAY = posY;
                }
                else
                {
                    Interaction.MsgBox("Error: unhandled type.", MsgBoxStyle.ApplicationModal, null);
                }
                this.SelectedScriptMarker_Update();
                this.View_DrawViewLater();
            }
        }

        private void txtScriptMarkerY2_LostFocus(object sender, EventArgs e)
        {
            if (this.txtScriptMarkerY2.Enabled && (this._SelectedScriptMarker != null))
            {
                if (this._SelectedScriptMarker is clsMap.clsScriptArea)
                {
                    clsMap.clsScriptArea area2 = (clsMap.clsScriptArea) this._SelectedScriptMarker;
                    int posBY = area2.PosBY;
                    modIO.InvariantParse_int(this.txtScriptMarkerY2.Text, ref posBY);
                    area2.PosBY = posBY;
                }
                else
                {
                    Interaction.MsgBox("Error: unhandled type.", MsgBoxStyle.ApplicationModal, null);
                }
                this.SelectedScriptMarker_Update();
                this.View_DrawViewLater();
            }
        }

        private void UpdateMinimap()
        {
            clsMap mainMap = this.MainMap;
            if (mainMap != null)
            {
                mainMap.MinimapMakeLater();
            }
        }

        public void View_DrawViewLater()
        {
            if (this.MapView != null)
            {
                this.MapView.DrawViewLater();
            }
        }

        private void WaterTriangleCorrectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            clsMap mainMap = this.MainMap;
            if (mainMap != null)
            {
                mainMap.WaterTriCorrection();
                mainMap.Update();
                mainMap.UndoStepCreate("Water Triangle Correction");
                this.View_DrawViewLater();
            }
        }

        public virtual Button btnAlignObjects
        {
            get
            {
                return this._btnAlignObjects;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.btnAlignObjects_Click);
                if (this._btnAlignObjects != null)
                {
                    this._btnAlignObjects.Click -= handler;
                }
                this._btnAlignObjects = value;
                if (this._btnAlignObjects != null)
                {
                    this._btnAlignObjects.Click += handler;
                }
            }
        }

        public virtual Button btnAutoRoadRemove
        {
            get
            {
                return this._btnAutoRoadRemove;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.btnAutoRoadRemove_Click);
                if (this._btnAutoRoadRemove != null)
                {
                    this._btnAutoRoadRemove.Click -= handler;
                }
                this._btnAutoRoadRemove = value;
                if (this._btnAutoRoadRemove != null)
                {
                    this._btnAutoRoadRemove.Click += handler;
                }
            }
        }

        public virtual Button btnAutoTextureRemove
        {
            get
            {
                return this._btnAutoTextureRemove;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.btnAutoTextureRemove_Click);
                if (this._btnAutoTextureRemove != null)
                {
                    this._btnAutoTextureRemove.Click -= handler;
                }
                this._btnAutoTextureRemove = value;
                if (this._btnAutoTextureRemove != null)
                {
                    this._btnAutoTextureRemove.Click += handler;
                }
            }
        }

        public virtual Button btnDroidToDesign
        {
            get
            {
                return this._btnDroidToDesign;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.btnDroidToDesign_Click);
                if (this._btnDroidToDesign != null)
                {
                    this._btnDroidToDesign.Click -= handler;
                }
                this._btnDroidToDesign = value;
                if (this._btnDroidToDesign != null)
                {
                    this._btnDroidToDesign.Click += handler;
                }
            }
        }

        public virtual Button btnFlatSelected
        {
            get
            {
                return this._btnFlatSelected;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.btnFlatSelected_Click);
                if (this._btnFlatSelected != null)
                {
                    this._btnFlatSelected.Click -= handler;
                }
                this._btnFlatSelected = value;
                if (this._btnFlatSelected != null)
                {
                    this._btnFlatSelected.Click += handler;
                }
            }
        }

        public virtual Button btnHeightOffsetSelection
        {
            get
            {
                return this._btnHeightOffsetSelection;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.btnHeightOffsetSelection_Click);
                if (this._btnHeightOffsetSelection != null)
                {
                    this._btnHeightOffsetSelection.Click -= handler;
                }
                this._btnHeightOffsetSelection = value;
                if (this._btnHeightOffsetSelection != null)
                {
                    this._btnHeightOffsetSelection.Click += handler;
                }
            }
        }

        public virtual Button btnHeightsMultiplySelection
        {
            get
            {
                return this._btnHeightsMultiplySelection;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.btnHeightsMultiplySelection_Click);
                if (this._btnHeightsMultiplySelection != null)
                {
                    this._btnHeightsMultiplySelection.Click -= handler;
                }
                this._btnHeightsMultiplySelection = value;
                if (this._btnHeightsMultiplySelection != null)
                {
                    this._btnHeightsMultiplySelection.Click += handler;
                }
            }
        }

        public virtual Button btnObjectTypeSelect
        {
            get
            {
                return this._btnObjectTypeSelect;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.btnObjectTypeSelect_Click);
                if (this._btnObjectTypeSelect != null)
                {
                    this._btnObjectTypeSelect.Click -= handler;
                }
                this._btnObjectTypeSelect = value;
                if (this._btnObjectTypeSelect != null)
                {
                    this._btnObjectTypeSelect.Click += handler;
                }
            }
        }

        public virtual Button btnPlayerSelectObjects
        {
            get
            {
                return this._btnPlayerSelectObjects;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.btnPlayerSelectObjects_Click);
                if (this._btnPlayerSelectObjects != null)
                {
                    this._btnPlayerSelectObjects.Click -= handler;
                }
                this._btnPlayerSelectObjects = value;
                if (this._btnPlayerSelectObjects != null)
                {
                    this._btnPlayerSelectObjects.Click += handler;
                }
            }
        }

        public virtual Button btnResize
        {
            get
            {
                return this._btnResize;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.btnResize_Click);
                if (this._btnResize != null)
                {
                    this._btnResize.Click -= handler;
                }
                this._btnResize = value;
                if (this._btnResize != null)
                {
                    this._btnResize.Click += handler;
                }
            }
        }

        public virtual Button btnScriptAreaCreate
        {
            get
            {
                return this._btnScriptAreaCreate;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.btnScriptAreaCreate_Click);
                if (this._btnScriptAreaCreate != null)
                {
                    this._btnScriptAreaCreate.Click -= handler;
                }
                this._btnScriptAreaCreate = value;
                if (this._btnScriptAreaCreate != null)
                {
                    this._btnScriptAreaCreate.Click += handler;
                }
            }
        }

        public virtual Button btnScriptMarkerRemove
        {
            get
            {
                return this._btnScriptMarkerRemove;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.btnScriptMarkerRemove_Click);
                if (this._btnScriptMarkerRemove != null)
                {
                    this._btnScriptMarkerRemove.Click -= handler;
                }
                this._btnScriptMarkerRemove = value;
                if (this._btnScriptMarkerRemove != null)
                {
                    this._btnScriptMarkerRemove.Click += handler;
                }
            }
        }

        public virtual Button btnSelResize
        {
            get
            {
                return this._btnSelResize;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.btnSelResize_Click);
                if (this._btnSelResize != null)
                {
                    this._btnSelResize.Click -= handler;
                }
                this._btnSelResize = value;
                if (this._btnSelResize != null)
                {
                    this._btnSelResize.Click += handler;
                }
            }
        }

        public virtual Button btnTextureAnticlockwise
        {
            get
            {
                return this._btnTextureAnticlockwise;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.btnTextureAnticlockwise_Click);
                if (this._btnTextureAnticlockwise != null)
                {
                    this._btnTextureAnticlockwise.Click -= handler;
                }
                this._btnTextureAnticlockwise = value;
                if (this._btnTextureAnticlockwise != null)
                {
                    this._btnTextureAnticlockwise.Click += handler;
                }
            }
        }

        public virtual Button btnTextureClockwise
        {
            get
            {
                return this._btnTextureClockwise;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.btnTextureClockwise_Click);
                if (this._btnTextureClockwise != null)
                {
                    this._btnTextureClockwise.Click -= handler;
                }
                this._btnTextureClockwise = value;
                if (this._btnTextureClockwise != null)
                {
                    this._btnTextureClockwise.Click += handler;
                }
            }
        }

        public virtual Button btnTextureFlipX
        {
            get
            {
                return this._btnTextureFlipX;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.btnTextureFlipX_Click);
                if (this._btnTextureFlipX != null)
                {
                    this._btnTextureFlipX.Click -= handler;
                }
                this._btnTextureFlipX = value;
                if (this._btnTextureFlipX != null)
                {
                    this._btnTextureFlipX.Click += handler;
                }
            }
        }

        public virtual ComboBox cboDroidBody
        {
            get
            {
                return this._cboDroidBody;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.cboDroidBody_SelectedIndexChanged);
                if (this._cboDroidBody != null)
                {
                    this._cboDroidBody.SelectedIndexChanged -= handler;
                }
                this._cboDroidBody = value;
                if (this._cboDroidBody != null)
                {
                    this._cboDroidBody.SelectedIndexChanged += handler;
                }
            }
        }

        public virtual ComboBox cboDroidPropulsion
        {
            get
            {
                return this._cboDroidPropulsion;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.cboDroidPropulsion_SelectedIndexChanged);
                if (this._cboDroidPropulsion != null)
                {
                    this._cboDroidPropulsion.SelectedIndexChanged -= handler;
                }
                this._cboDroidPropulsion = value;
                if (this._cboDroidPropulsion != null)
                {
                    this._cboDroidPropulsion.SelectedIndexChanged += handler;
                }
            }
        }

        public virtual ComboBox cboDroidTurret1
        {
            get
            {
                return this._cboDroidTurret1;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.cboDroidTurret1_SelectedIndexChanged);
                if (this._cboDroidTurret1 != null)
                {
                    this._cboDroidTurret1.SelectedIndexChanged -= handler;
                }
                this._cboDroidTurret1 = value;
                if (this._cboDroidTurret1 != null)
                {
                    this._cboDroidTurret1.SelectedIndexChanged += handler;
                }
            }
        }

        public virtual ComboBox cboDroidTurret2
        {
            get
            {
                return this._cboDroidTurret2;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.cboDroidTurret2_SelectedIndexChanged);
                if (this._cboDroidTurret2 != null)
                {
                    this._cboDroidTurret2.SelectedIndexChanged -= handler;
                }
                this._cboDroidTurret2 = value;
                if (this._cboDroidTurret2 != null)
                {
                    this._cboDroidTurret2.SelectedIndexChanged += handler;
                }
            }
        }

        public virtual ComboBox cboDroidTurret3
        {
            get
            {
                return this._cboDroidTurret3;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.cboDroidTurret3_SelectedIndexChanged);
                if (this._cboDroidTurret3 != null)
                {
                    this._cboDroidTurret3.SelectedIndexChanged -= handler;
                }
                this._cboDroidTurret3 = value;
                if (this._cboDroidTurret3 != null)
                {
                    this._cboDroidTurret3.SelectedIndexChanged += handler;
                }
            }
        }

        public virtual ComboBox cboDroidType
        {
            get
            {
                return this._cboDroidType;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.cboDroidType_SelectedIndexChanged);
                if (this._cboDroidType != null)
                {
                    this._cboDroidType.SelectedIndexChanged -= handler;
                }
                this._cboDroidType = value;
                if (this._cboDroidType != null)
                {
                    this._cboDroidType.SelectedIndexChanged += handler;
                }
            }
        }

        public virtual ComboBox cboTileset
        {
            get
            {
                return this._cboTileset;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.cboTileset_SelectedIndexChanged);
                if (this._cboTileset != null)
                {
                    this._cboTileset.SelectedIndexChanged -= handler;
                }
                this._cboTileset = value;
                if (this._cboTileset != null)
                {
                    this._cboTileset.SelectedIndexChanged += handler;
                }
            }
        }

        public virtual ComboBox cboTileType
        {
            get
            {
                return this._cboTileType;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.cmbTileType_SelectedIndexChanged);
                if (this._cboTileType != null)
                {
                    this._cboTileType.SelectedIndexChanged -= handler;
                }
                this._cboTileType = value;
                if (this._cboTileType != null)
                {
                    this._cboTileType.SelectedIndexChanged += handler;
                }
            }
        }

        public virtual CheckBox cbxAutoTexSetHeight
        {
            get
            {
                return this._cbxAutoTexSetHeight;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._cbxAutoTexSetHeight = value;
            }
        }

        public virtual CheckBox cbxAutoWalls
        {
            get
            {
                return this._cbxAutoWalls;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._cbxAutoWalls = value;
            }
        }

        public virtual CheckBox cbxCliffTris
        {
            get
            {
                return this._cbxCliffTris;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._cbxCliffTris = value;
            }
        }

        internal virtual CheckBox cbxDesignableOnly
        {
            get
            {
                return this._cbxDesignableOnly;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.cbxDesignableOnly_CheckedChanged);
                if (this._cbxDesignableOnly != null)
                {
                    this._cbxDesignableOnly.CheckedChanged -= handler;
                }
                this._cbxDesignableOnly = value;
                if (this._cbxDesignableOnly != null)
                {
                    this._cbxDesignableOnly.CheckedChanged += handler;
                }
            }
        }

        public virtual CheckBox cbxFillInside
        {
            get
            {
                return this._cbxFillInside;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._cbxFillInside = value;
            }
        }

        public virtual CheckBox cbxFootprintRotate
        {
            get
            {
                return this._cbxFootprintRotate;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._cbxFootprintRotate = value;
            }
        }

        public virtual CheckBox cbxHeightChangeFade
        {
            get
            {
                return this._cbxHeightChangeFade;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._cbxHeightChangeFade = value;
            }
        }

        public virtual CheckBox cbxInvalidTiles
        {
            get
            {
                return this._cbxInvalidTiles;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._cbxInvalidTiles = value;
            }
        }

        public virtual CheckBox cbxObjectRandomRotation
        {
            get
            {
                return this._cbxObjectRandomRotation;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._cbxObjectRandomRotation = value;
            }
        }

        public virtual CheckBox cbxTileNumbers
        {
            get
            {
                return this._cbxTileNumbers;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.chkTileNumbers_CheckedChanged);
                if (this._cbxTileNumbers != null)
                {
                    this._cbxTileNumbers.CheckedChanged -= handler;
                }
                this._cbxTileNumbers = value;
                if (this._cbxTileNumbers != null)
                {
                    this._cbxTileNumbers.CheckedChanged += handler;
                }
            }
        }

        public virtual CheckBox cbxTileTypes
        {
            get
            {
                return this._cbxTileTypes;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.chkTileTypes_CheckedChanged);
                if (this._cbxTileTypes != null)
                {
                    this._cbxTileTypes.CheckedChanged -= handler;
                }
                this._cbxTileTypes = value;
                if (this._cbxTileTypes != null)
                {
                    this._cbxTileTypes.CheckedChanged += handler;
                }
            }
        }

        public virtual CheckBox chkSetTexture
        {
            get
            {
                return this._chkSetTexture;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._chkSetTexture = value;
            }
        }

        public virtual CheckBox chkSetTextureOrientation
        {
            get
            {
                return this._chkSetTextureOrientation;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._chkSetTextureOrientation = value;
            }
        }

        public virtual CheckBox chkTextureOrientationRandomize
        {
            get
            {
                return this._chkTextureOrientationRandomize;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._chkTextureOrientationRandomize = value;
            }
        }

        public virtual ToolStripMenuItem CloseToolStripMenuItem
        {
            get
            {
                return this._CloseToolStripMenuItem;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.CloseToolStripMenuItem_Click);
                if (this._CloseToolStripMenuItem != null)
                {
                    this._CloseToolStripMenuItem.Click -= handler;
                }
                this._CloseToolStripMenuItem = value;
                if (this._CloseToolStripMenuItem != null)
                {
                    this._CloseToolStripMenuItem.Click += handler;
                }
            }
        }

        internal virtual DataGridView dgvDroids
        {
            get
            {
                return this._dgvDroids;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.dgvDroids_SelectionChanged);
                if (this._dgvDroids != null)
                {
                    this._dgvDroids.Click -= handler;
                }
                this._dgvDroids = value;
                if (this._dgvDroids != null)
                {
                    this._dgvDroids.Click += handler;
                }
            }
        }

        internal virtual DataGridView dgvFeatures
        {
            get
            {
                return this._dgvFeatures;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.dgvFeatures_SelectionChanged);
                if (this._dgvFeatures != null)
                {
                    this._dgvFeatures.Click -= handler;
                }
                this._dgvFeatures = value;
                if (this._dgvFeatures != null)
                {
                    this._dgvFeatures.Click += handler;
                }
            }
        }

        internal virtual DataGridView dgvStructures
        {
            get
            {
                return this._dgvStructures;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.dgvStructures_SelectionChanged);
                if (this._dgvStructures != null)
                {
                    this._dgvStructures.Click -= handler;
                }
                this._dgvStructures = value;
                if (this._dgvStructures != null)
                {
                    this._dgvStructures.Click += handler;
                }
            }
        }

        internal virtual GroupBox GroupBox1
        {
            get
            {
                return this._GroupBox1;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._GroupBox1 = value;
            }
        }

        internal virtual GroupBox GroupBox3
        {
            get
            {
                return this._GroupBox3;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._GroupBox3 = value;
            }
        }

        public virtual ToolStripMenuItem HeightmapBMPToolStripMenuItem
        {
            get
            {
                return this._HeightmapBMPToolStripMenuItem;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._HeightmapBMPToolStripMenuItem = value;
            }
        }

        public virtual ToolStripMenuItem ImportHeightmapToolStripMenuItem
        {
            get
            {
                return this._ImportHeightmapToolStripMenuItem;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.ImportHeightmapToolStripMenuItem_Click);
                if (this._ImportHeightmapToolStripMenuItem != null)
                {
                    this._ImportHeightmapToolStripMenuItem.Click -= handler;
                }
                this._ImportHeightmapToolStripMenuItem = value;
                if (this._ImportHeightmapToolStripMenuItem != null)
                {
                    this._ImportHeightmapToolStripMenuItem.Click += handler;
                }
            }
        }

        public virtual Label Label1
        {
            get
            {
                return this._Label1;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._Label1 = value;
            }
        }

        public virtual Label Label10
        {
            get
            {
                return this._Label10;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._Label10 = value;
            }
        }

        public virtual Label Label11
        {
            get
            {
                return this._Label11;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._Label11 = value;
            }
        }

        public virtual Label Label12
        {
            get
            {
                return this._Label12;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._Label12 = value;
            }
        }

        public virtual Label Label13
        {
            get
            {
                return this._Label13;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._Label13 = value;
            }
        }

        public virtual Label Label14
        {
            get
            {
                return this._Label14;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._Label14 = value;
            }
        }

        public virtual Label Label15
        {
            get
            {
                return this._Label15;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._Label15 = value;
            }
        }

        public virtual Label Label16
        {
            get
            {
                return this._Label16;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._Label16 = value;
            }
        }

        public virtual Label Label17
        {
            get
            {
                return this._Label17;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._Label17 = value;
            }
        }

        public virtual Label Label18
        {
            get
            {
                return this._Label18;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._Label18 = value;
            }
        }

        public virtual Label Label19
        {
            get
            {
                return this._Label19;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._Label19 = value;
            }
        }

        public virtual Label Label2
        {
            get
            {
                return this._Label2;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._Label2 = value;
            }
        }

        public virtual Label Label20
        {
            get
            {
                return this._Label20;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._Label20 = value;
            }
        }

        public virtual Label Label21
        {
            get
            {
                return this._Label21;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._Label21 = value;
            }
        }

        public virtual Label Label22
        {
            get
            {
                return this._Label22;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._Label22 = value;
            }
        }

        public virtual Label Label23
        {
            get
            {
                return this._Label23;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._Label23 = value;
            }
        }

        public virtual Label Label24
        {
            get
            {
                return this._Label24;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._Label24 = value;
            }
        }

        public virtual Label Label25
        {
            get
            {
                return this._Label25;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._Label25 = value;
            }
        }

        public virtual Label Label26
        {
            get
            {
                return this._Label26;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._Label26 = value;
            }
        }

        public virtual Label Label27
        {
            get
            {
                return this._Label27;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._Label27 = value;
            }
        }

        public virtual Label Label28
        {
            get
            {
                return this._Label28;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._Label28 = value;
            }
        }

        public virtual Label Label29
        {
            get
            {
                return this._Label29;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._Label29 = value;
            }
        }

        public virtual Label Label3
        {
            get
            {
                return this._Label3;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._Label3 = value;
            }
        }

        public virtual Label Label30
        {
            get
            {
                return this._Label30;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._Label30 = value;
            }
        }

        public virtual Label Label31
        {
            get
            {
                return this._Label31;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._Label31 = value;
            }
        }

        public virtual Label Label32
        {
            get
            {
                return this._Label32;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._Label32 = value;
            }
        }

        public virtual Label Label33
        {
            get
            {
                return this._Label33;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._Label33 = value;
            }
        }

        public virtual Label Label34
        {
            get
            {
                return this._Label34;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._Label34 = value;
            }
        }

        public virtual Label Label35
        {
            get
            {
                return this._Label35;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._Label35 = value;
            }
        }

        public virtual Label Label36
        {
            get
            {
                return this._Label36;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._Label36 = value;
            }
        }

        public virtual Label Label37
        {
            get
            {
                return this._Label37;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._Label37 = value;
            }
        }

        public virtual Label Label38
        {
            get
            {
                return this._Label38;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._Label38 = value;
            }
        }

        public virtual Label Label39
        {
            get
            {
                return this._Label39;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._Label39 = value;
            }
        }

        public virtual Label Label4
        {
            get
            {
                return this._Label4;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._Label4 = value;
            }
        }

        public virtual Label Label40
        {
            get
            {
                return this._Label40;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._Label40 = value;
            }
        }

        public virtual Label Label41
        {
            get
            {
                return this._Label41;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._Label41 = value;
            }
        }

        public virtual Label Label42
        {
            get
            {
                return this._Label42;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._Label42 = value;
            }
        }

        public virtual Label Label43
        {
            get
            {
                return this._Label43;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._Label43 = value;
            }
        }

        public virtual Label Label44
        {
            get
            {
                return this._Label44;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._Label44 = value;
            }
        }

        public virtual Label Label5
        {
            get
            {
                return this._Label5;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._Label5 = value;
            }
        }

        public virtual Label Label6
        {
            get
            {
                return this._Label6;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._Label6 = value;
            }
        }

        public virtual Label Label7
        {
            get
            {
                return this._Label7;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._Label7 = value;
            }
        }

        public virtual Label Label8
        {
            get
            {
                return this._Label8;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._Label8 = value;
            }
        }

        public virtual Label Label9
        {
            get
            {
                return this._Label9;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._Label9 = value;
            }
        }

        public virtual Label lblObjectType
        {
            get
            {
                return this._lblObjectType;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._lblObjectType = value;
            }
        }

        public clsMaps LoadedMaps
        {
            get
            {
                return this._LoadedMaps;
            }
        }

        public virtual ListBox lstAutoRoad
        {
            get
            {
                return this._lstAutoRoad;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.lstAutoRoad_SelectedIndexChanged);
                EventHandler handler2 = new EventHandler(this.lstAutoRoad_Click);
                if (this._lstAutoRoad != null)
                {
                    this._lstAutoRoad.SelectedIndexChanged -= handler;
                    this._lstAutoRoad.Click -= handler2;
                }
                this._lstAutoRoad = value;
                if (this._lstAutoRoad != null)
                {
                    this._lstAutoRoad.SelectedIndexChanged += handler;
                    this._lstAutoRoad.Click += handler2;
                }
            }
        }

        public virtual ListBox lstAutoTexture
        {
            get
            {
                return this._lstAutoTexture;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.lstAutoTexture_SelectedIndexChanged_1);
                EventHandler handler2 = new EventHandler(this.lstAutoTexture_SelectedIndexChanged);
                if (this._lstAutoTexture != null)
                {
                    this._lstAutoTexture.SelectedIndexChanged -= handler;
                    this._lstAutoTexture.Click -= handler2;
                }
                this._lstAutoTexture = value;
                if (this._lstAutoTexture != null)
                {
                    this._lstAutoTexture.SelectedIndexChanged += handler;
                    this._lstAutoTexture.Click += handler2;
                }
            }
        }

        internal virtual ListBox lstScriptAreas
        {
            get
            {
                return this._lstScriptAreas;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.lstScriptAreas_SelectedIndexChanged);
                if (this._lstScriptAreas != null)
                {
                    this._lstScriptAreas.SelectedIndexChanged -= handler;
                }
                this._lstScriptAreas = value;
                if (this._lstScriptAreas != null)
                {
                    this._lstScriptAreas.SelectedIndexChanged += handler;
                }
            }
        }

        internal virtual ListBox lstScriptPositions
        {
            get
            {
                return this._lstScriptPositions;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.lstScriptPositions_SelectedIndexChanged);
                if (this._lstScriptPositions != null)
                {
                    this._lstScriptPositions.SelectedIndexChanged -= handler;
                }
                this._lstScriptPositions = value;
                if (this._lstScriptPositions != null)
                {
                    this._lstScriptPositions.SelectedIndexChanged += handler;
                }
            }
        }

        public clsMap MainMap
        {
            get
            {
                return this._LoadedMaps.MainMap;
            }
        }

        public virtual ToolStripMenuItem MapLNDToolStripMenuItem
        {
            get
            {
                return this._MapLNDToolStripMenuItem;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.LNDToolStripMenuItem1_Click);
                if (this._MapLNDToolStripMenuItem != null)
                {
                    this._MapLNDToolStripMenuItem.Click -= handler;
                }
                this._MapLNDToolStripMenuItem = value;
                if (this._MapLNDToolStripMenuItem != null)
                {
                    this._MapLNDToolStripMenuItem.Click += handler;
                }
            }
        }

        public virtual ToolStripMenuItem MapWZToolStripMenuItem
        {
            get
            {
                return this._MapWZToolStripMenuItem;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.MapWZToolStripMenuItem_Click);
                if (this._MapWZToolStripMenuItem != null)
                {
                    this._MapWZToolStripMenuItem.Click -= handler;
                }
                this._MapWZToolStripMenuItem = value;
                if (this._MapWZToolStripMenuItem != null)
                {
                    this._MapWZToolStripMenuItem.Click += handler;
                }
            }
        }

        public virtual ToolStripMenuItem menuExportMapTileTypes
        {
            get
            {
                return this._menuExportMapTileTypes;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.menuExportMapTileTypes_Click);
                if (this._menuExportMapTileTypes != null)
                {
                    this._menuExportMapTileTypes.Click -= handler;
                }
                this._menuExportMapTileTypes = value;
                if (this._menuExportMapTileTypes != null)
                {
                    this._menuExportMapTileTypes.Click += handler;
                }
            }
        }

        public virtual ToolStripMenuItem menuFile
        {
            get
            {
                return this._menuFile;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._menuFile = value;
            }
        }

        internal virtual ToolStripMenuItem menuFlatOil
        {
            get
            {
                return this._menuFlatOil;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.ToolStripMenuItem5_Click);
                if (this._menuFlatOil != null)
                {
                    this._menuFlatOil.Click -= handler;
                }
                this._menuFlatOil = value;
                if (this._menuFlatOil != null)
                {
                    this._menuFlatOil.Click += handler;
                }
            }
        }

        internal virtual ToolStripMenuItem menuFlatStructures
        {
            get
            {
                return this._menuFlatStructures;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.ToolStripMenuItem6_Click);
                if (this._menuFlatStructures != null)
                {
                    this._menuFlatStructures.Click -= handler;
                }
                this._menuFlatStructures = value;
                if (this._menuFlatStructures != null)
                {
                    this._menuFlatStructures.Click += handler;
                }
            }
        }

        internal virtual ToolStripMenuItem menuGenerator
        {
            get
            {
                return this._menuGenerator;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.GeneratorToolStripMenuItem_Click);
                if (this._menuGenerator != null)
                {
                    this._menuGenerator.Click -= handler;
                }
                this._menuGenerator = value;
                if (this._menuGenerator != null)
                {
                    this._menuGenerator.Click += handler;
                }
            }
        }

        public virtual ToolStripMenuItem menuImportTileTypes
        {
            get
            {
                return this._menuImportTileTypes;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.menuImportTileTypes_Click);
                if (this._menuImportTileTypes != null)
                {
                    this._menuImportTileTypes.Click -= handler;
                }
                this._menuImportTileTypes = value;
                if (this._menuImportTileTypes != null)
                {
                    this._menuImportTileTypes.Click += handler;
                }
            }
        }

        public virtual MenuStrip menuMain
        {
            get
            {
                return this._menuMain;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._menuMain = value;
            }
        }

        public virtual ToolStripDropDownButton menuMinimap
        {
            get
            {
                return this._menuMinimap;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._menuMinimap = value;
            }
        }

        public virtual ToolStripMenuItem menuMiniShowCliffs
        {
            get
            {
                return this._menuMiniShowCliffs;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.menuMiniShowCliffs_Click);
                if (this._menuMiniShowCliffs != null)
                {
                    this._menuMiniShowCliffs.Click -= handler;
                }
                this._menuMiniShowCliffs = value;
                if (this._menuMiniShowCliffs != null)
                {
                    this._menuMiniShowCliffs.Click += handler;
                }
            }
        }

        public virtual ToolStripMenuItem menuMiniShowGateways
        {
            get
            {
                return this._menuMiniShowGateways;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.menuMiniShowGateways_Click);
                if (this._menuMiniShowGateways != null)
                {
                    this._menuMiniShowGateways.Click -= handler;
                }
                this._menuMiniShowGateways = value;
                if (this._menuMiniShowGateways != null)
                {
                    this._menuMiniShowGateways.Click += handler;
                }
            }
        }

        public virtual ToolStripMenuItem menuMiniShowHeight
        {
            get
            {
                return this._menuMiniShowHeight;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.menuMiniShowHeight_Click);
                if (this._menuMiniShowHeight != null)
                {
                    this._menuMiniShowHeight.Click -= handler;
                }
                this._menuMiniShowHeight = value;
                if (this._menuMiniShowHeight != null)
                {
                    this._menuMiniShowHeight.Click += handler;
                }
            }
        }

        public virtual ToolStripMenuItem menuMiniShowTex
        {
            get
            {
                return this._menuMiniShowTex;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.menuMiniShowTex_Click);
                if (this._menuMiniShowTex != null)
                {
                    this._menuMiniShowTex.Click -= handler;
                }
                this._menuMiniShowTex = value;
                if (this._menuMiniShowTex != null)
                {
                    this._menuMiniShowTex.Click += handler;
                }
            }
        }

        public virtual ToolStripMenuItem menuMiniShowUnits
        {
            get
            {
                return this._menuMiniShowUnits;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.menuMiniShowUnits_Click);
                if (this._menuMiniShowUnits != null)
                {
                    this._menuMiniShowUnits.Click -= handler;
                }
                this._menuMiniShowUnits = value;
                if (this._menuMiniShowUnits != null)
                {
                    this._menuMiniShowUnits.Click += handler;
                }
            }
        }

        public virtual ToolStripMenuItem menuOptions
        {
            get
            {
                return this._menuOptions;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.menuOptions_Click);
                if (this._menuOptions != null)
                {
                    this._menuOptions.Click -= handler;
                }
                this._menuOptions = value;
                if (this._menuOptions != null)
                {
                    this._menuOptions.Click += handler;
                }
            }
        }

        internal virtual ToolStripMenuItem menuReinterpret
        {
            get
            {
                return this._menuReinterpret;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.ReinterpretTerrainToolStripMenuItem_Click);
                if (this._menuReinterpret != null)
                {
                    this._menuReinterpret.Click -= handler;
                }
                this._menuReinterpret = value;
                if (this._menuReinterpret != null)
                {
                    this._menuReinterpret.Click += handler;
                }
            }
        }

        public virtual ToolStripMenuItem menuRotateNothing
        {
            get
            {
                return this._menuRotateNothing;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.menuRotateNothing_Click);
                if (this._menuRotateNothing != null)
                {
                    this._menuRotateNothing.Click -= handler;
                }
                this._menuRotateNothing = value;
                if (this._menuRotateNothing != null)
                {
                    this._menuRotateNothing.Click += handler;
                }
            }
        }

        public virtual ToolStripMenuItem menuRotateUnits
        {
            get
            {
                return this._menuRotateUnits;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.menuRotateUnits_Click);
                if (this._menuRotateUnits != null)
                {
                    this._menuRotateUnits.Click -= handler;
                }
                this._menuRotateUnits = value;
                if (this._menuRotateUnits != null)
                {
                    this._menuRotateUnits.Click += handler;
                }
            }
        }

        public virtual ToolStripMenuItem menuRotateWalls
        {
            get
            {
                return this._menuRotateWalls;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.menuRotateWalls_Click);
                if (this._menuRotateWalls != null)
                {
                    this._menuRotateWalls.Click -= handler;
                }
                this._menuRotateWalls = value;
                if (this._menuRotateWalls != null)
                {
                    this._menuRotateWalls.Click += handler;
                }
            }
        }

        public virtual ToolStripMenuItem menuSaveFMap
        {
            get
            {
                return this._menuSaveFMap;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.FMapToolStripMenuItem_Click);
                if (this._menuSaveFMap != null)
                {
                    this._menuSaveFMap.Click -= handler;
                }
                this._menuSaveFMap = value;
                if (this._menuSaveFMap != null)
                {
                    this._menuSaveFMap.Click += handler;
                }
            }
        }

        internal virtual ToolStripMenuItem menuSaveFMapQuick
        {
            get
            {
                return this._menuSaveFMapQuick;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.menuSaveFMapQuick_Click);
                if (this._menuSaveFMapQuick != null)
                {
                    this._menuSaveFMapQuick.Click -= handler;
                }
                this._menuSaveFMapQuick = value;
                if (this._menuSaveFMapQuick != null)
                {
                    this._menuSaveFMapQuick.Click += handler;
                }
            }
        }

        public virtual ToolStripMenuItem menuSaveFME
        {
            get
            {
                return this._menuSaveFME;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.menuSaveFME_Click);
                if (this._menuSaveFME != null)
                {
                    this._menuSaveFME.Click -= handler;
                }
                this._menuSaveFME = value;
                if (this._menuSaveFME != null)
                {
                    this._menuSaveFME.Click += handler;
                }
            }
        }

        public virtual ToolStripMenuItem menuSelPasteDeleteGateways
        {
            get
            {
                return this._menuSelPasteDeleteGateways;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._menuSelPasteDeleteGateways = value;
            }
        }

        public virtual ToolStripMenuItem menuSelPasteDeleteUnits
        {
            get
            {
                return this._menuSelPasteDeleteUnits;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._menuSelPasteDeleteUnits = value;
            }
        }

        public virtual ToolStripMenuItem menuSelPasteGateways
        {
            get
            {
                return this._menuSelPasteGateways;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._menuSelPasteGateways = value;
            }
        }

        public virtual ToolStripMenuItem menuSelPasteHeights
        {
            get
            {
                return this._menuSelPasteHeights;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._menuSelPasteHeights = value;
            }
        }

        public virtual ToolStripMenuItem menuSelPasteTextures
        {
            get
            {
                return this._menuSelPasteTextures;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._menuSelPasteTextures = value;
            }
        }

        public virtual ToolStripMenuItem menuSelPasteUnits
        {
            get
            {
                return this._menuSelPasteUnits;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._menuSelPasteUnits = value;
            }
        }

        internal virtual ToolStripMenuItem menuTools
        {
            get
            {
                return this._menuTools;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._menuTools = value;
            }
        }

        internal virtual ToolStripMenuItem menuWaterCorrection
        {
            get
            {
                return this._menuWaterCorrection;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.WaterTriangleCorrectionToolStripMenuItem_Click);
                if (this._menuWaterCorrection != null)
                {
                    this._menuWaterCorrection.Click -= handler;
                }
                this._menuWaterCorrection = value;
                if (this._menuWaterCorrection != null)
                {
                    this._menuWaterCorrection.Click += handler;
                }
            }
        }

        public virtual ToolStripMenuItem MinimapBMPToolStripMenuItem
        {
            get
            {
                return this._MinimapBMPToolStripMenuItem;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.MinimapBMPToolStripMenuItem_Click);
                if (this._MinimapBMPToolStripMenuItem != null)
                {
                    this._MinimapBMPToolStripMenuItem.Click -= handler;
                }
                this._MinimapBMPToolStripMenuItem = value;
                if (this._MinimapBMPToolStripMenuItem != null)
                {
                    this._MinimapBMPToolStripMenuItem.Click += handler;
                }
            }
        }

        public virtual ToolStripMenuItem NewMapToolStripMenuItem
        {
            get
            {
                return this._NewMapToolStripMenuItem;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.NewMapToolStripMenuItem_Click);
                if (this._NewMapToolStripMenuItem != null)
                {
                    this._NewMapToolStripMenuItem.Click -= handler;
                }
                this._NewMapToolStripMenuItem = value;
                if (this._NewMapToolStripMenuItem != null)
                {
                    this._NewMapToolStripMenuItem.Click += handler;
                }
            }
        }

        public virtual ToolStripMenuItem OpenToolStripMenuItem
        {
            get
            {
                return this._OpenToolStripMenuItem;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.OpenToolStripMenuItem_Click);
                if (this._OpenToolStripMenuItem != null)
                {
                    this._OpenToolStripMenuItem.Click -= handler;
                }
                this._OpenToolStripMenuItem = value;
                if (this._OpenToolStripMenuItem != null)
                {
                    this._OpenToolStripMenuItem.Click += handler;
                }
            }
        }

        public virtual Panel Panel1
        {
            get
            {
                return this._Panel1;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._Panel1 = value;
            }
        }

        public virtual Panel Panel10
        {
            get
            {
                return this._Panel10;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._Panel10 = value;
            }
        }

        public virtual Panel Panel11
        {
            get
            {
                return this._Panel11;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._Panel11 = value;
            }
        }

        public virtual Panel Panel12
        {
            get
            {
                return this._Panel12;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._Panel12 = value;
            }
        }

        public virtual Panel Panel13
        {
            get
            {
                return this._Panel13;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._Panel13 = value;
            }
        }

        public virtual Panel Panel14
        {
            get
            {
                return this._Panel14;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._Panel14 = value;
            }
        }

        internal virtual Panel Panel15
        {
            get
            {
                return this._Panel15;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._Panel15 = value;
            }
        }

        internal virtual Panel Panel2
        {
            get
            {
                return this._Panel2;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._Panel2 = value;
            }
        }

        public virtual Panel Panel5
        {
            get
            {
                return this._Panel5;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._Panel5 = value;
            }
        }

        public virtual Panel Panel6
        {
            get
            {
                return this._Panel6;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._Panel6 = value;
            }
        }

        public virtual Panel Panel7
        {
            get
            {
                return this._Panel7;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._Panel7 = value;
            }
        }

        public virtual Panel Panel8
        {
            get
            {
                return this._Panel8;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._Panel8 = value;
            }
        }

        public virtual Panel Panel9
        {
            get
            {
                return this._Panel9;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._Panel9 = value;
            }
        }

        public virtual Panel pnlCliffRemoveBrush
        {
            get
            {
                return this._pnlCliffRemoveBrush;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._pnlCliffRemoveBrush = value;
            }
        }

        public virtual Panel pnlHeightSetBrush
        {
            get
            {
                return this._pnlHeightSetBrush;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._pnlHeightSetBrush = value;
            }
        }

        public virtual Panel pnlTerrainBrush
        {
            get
            {
                return this._pnlTerrainBrush;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._pnlTerrainBrush = value;
            }
        }

        public virtual Panel pnlTextureBrush
        {
            get
            {
                return this._pnlTextureBrush;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._pnlTextureBrush = value;
            }
        }

        public virtual Panel pnlView
        {
            get
            {
                return this._pnlView;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._pnlView = value;
            }
        }

        public virtual RadioButton rdoAutoCliffBrush
        {
            get
            {
                return this._rdoAutoCliffBrush;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.rdoAutoCliffBrush_Click);
                if (this._rdoAutoCliffBrush != null)
                {
                    this._rdoAutoCliffBrush.Click -= handler;
                }
                this._rdoAutoCliffBrush = value;
                if (this._rdoAutoCliffBrush != null)
                {
                    this._rdoAutoCliffBrush.Click += handler;
                }
            }
        }

        public virtual RadioButton rdoAutoCliffRemove
        {
            get
            {
                return this._rdoAutoCliffRemove;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.rdoAutoCliffRemove_Click);
                if (this._rdoAutoCliffRemove != null)
                {
                    this._rdoAutoCliffRemove.Click -= handler;
                }
                this._rdoAutoCliffRemove = value;
                if (this._rdoAutoCliffRemove != null)
                {
                    this._rdoAutoCliffRemove.Click += handler;
                }
            }
        }

        public virtual RadioButton rdoAutoRoadLine
        {
            get
            {
                return this._rdoAutoRoadLine;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.rdoAutoRoadLine_Click);
                if (this._rdoAutoRoadLine != null)
                {
                    this._rdoAutoRoadLine.Click -= handler;
                }
                this._rdoAutoRoadLine = value;
                if (this._rdoAutoRoadLine != null)
                {
                    this._rdoAutoRoadLine.Click += handler;
                }
            }
        }

        public virtual RadioButton rdoAutoRoadPlace
        {
            get
            {
                return this._rdoAutoRoadPlace;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.rdoAutoRoadPlace_Click);
                if (this._rdoAutoRoadPlace != null)
                {
                    this._rdoAutoRoadPlace.Click -= handler;
                }
                this._rdoAutoRoadPlace = value;
                if (this._rdoAutoRoadPlace != null)
                {
                    this._rdoAutoRoadPlace.Click += handler;
                }
            }
        }

        public virtual RadioButton rdoAutoTextureFill
        {
            get
            {
                return this._rdoAutoTextureFill;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.rdoAutoTextureFill_CheckedChanged);
                if (this._rdoAutoTextureFill != null)
                {
                    this._rdoAutoTextureFill.Click -= handler;
                }
                this._rdoAutoTextureFill = value;
                if (this._rdoAutoTextureFill != null)
                {
                    this._rdoAutoTextureFill.Click += handler;
                }
            }
        }

        public virtual RadioButton rdoAutoTexturePlace
        {
            get
            {
                return this._rdoAutoTexturePlace;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.rdoAutoTexturePlace_Click);
                if (this._rdoAutoTexturePlace != null)
                {
                    this._rdoAutoTexturePlace.Click -= handler;
                }
                this._rdoAutoTexturePlace = value;
                if (this._rdoAutoTexturePlace != null)
                {
                    this._rdoAutoTexturePlace.Click += handler;
                }
            }
        }

        public virtual RadioButton rdoCliffTriBrush
        {
            get
            {
                return this._rdoCliffTriBrush;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.rdoCliffTriBrush_CheckedChanged);
                if (this._rdoCliffTriBrush != null)
                {
                    this._rdoCliffTriBrush.CheckedChanged -= handler;
                }
                this._rdoCliffTriBrush = value;
                if (this._rdoCliffTriBrush != null)
                {
                    this._rdoCliffTriBrush.CheckedChanged += handler;
                }
            }
        }

        public virtual RadioButton rdoDroidTurret0
        {
            get
            {
                return this._rdoDroidTurret0;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.rdoDroidTurret0_CheckedChanged);
                if (this._rdoDroidTurret0 != null)
                {
                    this._rdoDroidTurret0.CheckedChanged -= handler;
                }
                this._rdoDroidTurret0 = value;
                if (this._rdoDroidTurret0 != null)
                {
                    this._rdoDroidTurret0.CheckedChanged += handler;
                }
            }
        }

        public virtual RadioButton rdoDroidTurret1
        {
            get
            {
                return this._rdoDroidTurret1;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.rdoDroidTurret1_CheckedChanged);
                if (this._rdoDroidTurret1 != null)
                {
                    this._rdoDroidTurret1.CheckedChanged -= handler;
                }
                this._rdoDroidTurret1 = value;
                if (this._rdoDroidTurret1 != null)
                {
                    this._rdoDroidTurret1.CheckedChanged += handler;
                }
            }
        }

        public virtual RadioButton rdoDroidTurret2
        {
            get
            {
                return this._rdoDroidTurret2;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.rdoDroidTurret2_CheckedChanged);
                if (this._rdoDroidTurret2 != null)
                {
                    this._rdoDroidTurret2.CheckedChanged -= handler;
                }
                this._rdoDroidTurret2 = value;
                if (this._rdoDroidTurret2 != null)
                {
                    this._rdoDroidTurret2.CheckedChanged += handler;
                }
            }
        }

        public virtual RadioButton rdoDroidTurret3
        {
            get
            {
                return this._rdoDroidTurret3;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.rdoDroidTurret3_CheckedChanged);
                if (this._rdoDroidTurret3 != null)
                {
                    this._rdoDroidTurret3.CheckedChanged -= handler;
                }
                this._rdoDroidTurret3 = value;
                if (this._rdoDroidTurret3 != null)
                {
                    this._rdoDroidTurret3.CheckedChanged += handler;
                }
            }
        }

        public virtual RadioButton rdoFillCliffIgnore
        {
            get
            {
                return this._rdoFillCliffIgnore;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.rdoFillCliffIgnore_CheckedChanged);
                if (this._rdoFillCliffIgnore != null)
                {
                    this._rdoFillCliffIgnore.CheckedChanged -= handler;
                }
                this._rdoFillCliffIgnore = value;
                if (this._rdoFillCliffIgnore != null)
                {
                    this._rdoFillCliffIgnore.CheckedChanged += handler;
                }
            }
        }

        public virtual RadioButton rdoFillCliffStopAfter
        {
            get
            {
                return this._rdoFillCliffStopAfter;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.rdoFillCliffStopAfter_CheckedChanged);
                if (this._rdoFillCliffStopAfter != null)
                {
                    this._rdoFillCliffStopAfter.CheckedChanged -= handler;
                }
                this._rdoFillCliffStopAfter = value;
                if (this._rdoFillCliffStopAfter != null)
                {
                    this._rdoFillCliffStopAfter.CheckedChanged += handler;
                }
            }
        }

        public virtual RadioButton rdoFillCliffStopBefore
        {
            get
            {
                return this._rdoFillCliffStopBefore;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.rdoFillCliffStopBefore_CheckedChanged);
                if (this._rdoFillCliffStopBefore != null)
                {
                    this._rdoFillCliffStopBefore.CheckedChanged -= handler;
                }
                this._rdoFillCliffStopBefore = value;
                if (this._rdoFillCliffStopBefore != null)
                {
                    this._rdoFillCliffStopBefore.CheckedChanged += handler;
                }
            }
        }

        public virtual RadioButton rdoHeightChange
        {
            get
            {
                return this._rdoHeightChange;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.rdoHeightChange_CheckedChanged);
                if (this._rdoHeightChange != null)
                {
                    this._rdoHeightChange.Click -= handler;
                }
                this._rdoHeightChange = value;
                if (this._rdoHeightChange != null)
                {
                    this._rdoHeightChange.Click += handler;
                }
            }
        }

        public virtual RadioButton rdoHeightSet
        {
            get
            {
                return this._rdoHeightSet;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.rdoHeightSet_CheckedChanged);
                if (this._rdoHeightSet != null)
                {
                    this._rdoHeightSet.Click -= handler;
                }
                this._rdoHeightSet = value;
                if (this._rdoHeightSet != null)
                {
                    this._rdoHeightSet.Click += handler;
                }
            }
        }

        public virtual RadioButton rdoHeightSmooth
        {
            get
            {
                return this._rdoHeightSmooth;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.rdoHeightSmooth_CheckedChanged);
                if (this._rdoHeightSmooth != null)
                {
                    this._rdoHeightSmooth.Click -= handler;
                }
                this._rdoHeightSmooth = value;
                if (this._rdoHeightSmooth != null)
                {
                    this._rdoHeightSmooth.Click += handler;
                }
            }
        }

        public virtual RadioButton rdoObjectLines
        {
            get
            {
                return this._rdoObjectLines;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.rdoObjectLine_Click);
                if (this._rdoObjectLines != null)
                {
                    this._rdoObjectLines.Click -= handler;
                }
                this._rdoObjectLines = value;
                if (this._rdoObjectLines != null)
                {
                    this._rdoObjectLines.Click += handler;
                }
            }
        }

        public virtual RadioButton rdoObjectPlace
        {
            get
            {
                return this._rdoObjectPlace;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.rdoObjectPlace_Click);
                if (this._rdoObjectPlace != null)
                {
                    this._rdoObjectPlace.Click -= handler;
                }
                this._rdoObjectPlace = value;
                if (this._rdoObjectPlace != null)
                {
                    this._rdoObjectPlace.Click += handler;
                }
            }
        }

        public virtual RadioButton rdoRoadRemove
        {
            get
            {
                return this._rdoRoadRemove;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.rdoRoadRemove_Click);
                if (this._rdoRoadRemove != null)
                {
                    this._rdoRoadRemove.Click -= handler;
                }
                this._rdoRoadRemove = value;
                if (this._rdoRoadRemove != null)
                {
                    this._rdoRoadRemove.Click += handler;
                }
            }
        }

        public virtual RadioButton rdoTextureIgnoreTerrain
        {
            get
            {
                return this._rdoTextureIgnoreTerrain;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.rdoTextureIgnoreTerrain_Click);
                if (this._rdoTextureIgnoreTerrain != null)
                {
                    this._rdoTextureIgnoreTerrain.Click -= handler;
                }
                this._rdoTextureIgnoreTerrain = value;
                if (this._rdoTextureIgnoreTerrain != null)
                {
                    this._rdoTextureIgnoreTerrain.Click += handler;
                }
            }
        }

        public virtual RadioButton rdoTextureReinterpretTerrain
        {
            get
            {
                return this._rdoTextureReinterpretTerrain;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.rdoTextureReinterpretTerrain_Click);
                if (this._rdoTextureReinterpretTerrain != null)
                {
                    this._rdoTextureReinterpretTerrain.Click -= handler;
                }
                this._rdoTextureReinterpretTerrain = value;
                if (this._rdoTextureReinterpretTerrain != null)
                {
                    this._rdoTextureReinterpretTerrain.Click += handler;
                }
            }
        }

        public virtual RadioButton rdoTextureRemoveTerrain
        {
            get
            {
                return this._rdoTextureRemoveTerrain;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.rdoTextureRemoveTerrain_Click);
                if (this._rdoTextureRemoveTerrain != null)
                {
                    this._rdoTextureRemoveTerrain.Click -= handler;
                }
                this._rdoTextureRemoveTerrain = value;
                if (this._rdoTextureRemoveTerrain != null)
                {
                    this._rdoTextureRemoveTerrain.Click += handler;
                }
            }
        }

        public virtual ToolStripMenuItem SaveToolStripMenuItem
        {
            get
            {
                return this._SaveToolStripMenuItem;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._SaveToolStripMenuItem = value;
            }
        }

        public object SelectedScriptMarker
        {
            get
            {
                return this._SelectedScriptMarker;
            }
        }

        public clsUnitType SingleSelectedObjectType
        {
            get
            {
                if (this.SelectedObjectTypes.Count == 1)
                {
                    return this.SelectedObjectTypes[0];
                }
                return null;
            }
        }

        public virtual SplitContainer SplitContainer1
        {
            get
            {
                return this._SplitContainer1;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._SplitContainer1 = value;
            }
        }

        private System.Windows.Forms.TabControl TabControl
        {
            get
            {
                return this._TabControl;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.TabControl_SelectedIndexChanged);
                if (this._TabControl != null)
                {
                    this._TabControl.SelectedIndexChanged -= handler;
                }
                this._TabControl = value;
                if (this._TabControl != null)
                {
                    this._TabControl.SelectedIndexChanged += handler;
                }
            }
        }

        internal virtual System.Windows.Forms.TabControl TabControl1
        {
            get
            {
                return this._TabControl1;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.TabControl1_SelectedIndexChanged);
                if (this._TabControl1 != null)
                {
                    this._TabControl1.SelectedIndexChanged -= handler;
                }
                this._TabControl1 = value;
                if (this._TabControl1 != null)
                {
                    this._TabControl1.SelectedIndexChanged += handler;
                }
            }
        }

        public virtual System.Windows.Forms.TabControl tabHeightSetL
        {
            get
            {
                return this._tabHeightSetL;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.tabHeightSetL_SelectedIndexChanged);
                if (this._tabHeightSetL != null)
                {
                    this._tabHeightSetL.SelectedIndexChanged -= handler;
                }
                this._tabHeightSetL = value;
                if (this._tabHeightSetL != null)
                {
                    this._tabHeightSetL.SelectedIndexChanged += handler;
                }
            }
        }

        public virtual System.Windows.Forms.TabControl tabHeightSetR
        {
            get
            {
                return this._tabHeightSetR;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.tabHeightSetR_SelectedIndexChanged);
                if (this._tabHeightSetR != null)
                {
                    this._tabHeightSetR.SelectedIndexChanged -= handler;
                }
                this._tabHeightSetR = value;
                if (this._tabHeightSetR != null)
                {
                    this._tabHeightSetR.SelectedIndexChanged += handler;
                }
            }
        }

        public virtual TableLayoutPanel TableLayoutPanel1
        {
            get
            {
                return this._TableLayoutPanel1;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._TableLayoutPanel1 = value;
            }
        }

        public virtual TableLayoutPanel TableLayoutPanel5
        {
            get
            {
                return this._TableLayoutPanel5;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._TableLayoutPanel5 = value;
            }
        }

        public virtual TableLayoutPanel TableLayoutPanel6
        {
            get
            {
                return this._TableLayoutPanel6;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._TableLayoutPanel6 = value;
            }
        }

        public virtual TableLayoutPanel TableLayoutPanel7
        {
            get
            {
                return this._TableLayoutPanel7;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._TableLayoutPanel7 = value;
            }
        }

        public virtual TableLayoutPanel TableLayoutPanel8
        {
            get
            {
                return this._TableLayoutPanel8;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._TableLayoutPanel8 = value;
            }
        }

        public virtual TableLayoutPanel TableLayoutPanel9
        {
            get
            {
                return this._TableLayoutPanel9;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._TableLayoutPanel9 = value;
            }
        }

        internal virtual TabPage TabPage1
        {
            get
            {
                return this._TabPage1;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._TabPage1 = value;
            }
        }

        public virtual TabPage TabPage10
        {
            get
            {
                return this._TabPage10;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._TabPage10 = value;
            }
        }

        public virtual TabPage TabPage11
        {
            get
            {
                return this._TabPage11;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._TabPage11 = value;
            }
        }

        public virtual TabPage TabPage12
        {
            get
            {
                return this._TabPage12;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._TabPage12 = value;
            }
        }

        public virtual TabPage TabPage13
        {
            get
            {
                return this._TabPage13;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._TabPage13 = value;
            }
        }

        public virtual TabPage TabPage14
        {
            get
            {
                return this._TabPage14;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._TabPage14 = value;
            }
        }

        public virtual TabPage TabPage15
        {
            get
            {
                return this._TabPage15;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._TabPage15 = value;
            }
        }

        public virtual TabPage TabPage16
        {
            get
            {
                return this._TabPage16;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._TabPage16 = value;
            }
        }

        public virtual TabPage TabPage17
        {
            get
            {
                return this._TabPage17;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._TabPage17 = value;
            }
        }

        public virtual TabPage TabPage18
        {
            get
            {
                return this._TabPage18;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._TabPage18 = value;
            }
        }

        public virtual TabPage TabPage19
        {
            get
            {
                return this._TabPage19;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._TabPage19 = value;
            }
        }

        internal virtual TabPage TabPage2
        {
            get
            {
                return this._TabPage2;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._TabPage2 = value;
            }
        }

        public virtual TabPage TabPage20
        {
            get
            {
                return this._TabPage20;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._TabPage20 = value;
            }
        }

        public virtual TabPage TabPage21
        {
            get
            {
                return this._TabPage21;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._TabPage21 = value;
            }
        }

        public virtual TabPage TabPage22
        {
            get
            {
                return this._TabPage22;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._TabPage22 = value;
            }
        }

        public virtual TabPage TabPage23
        {
            get
            {
                return this._TabPage23;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._TabPage23 = value;
            }
        }

        public virtual TabPage TabPage24
        {
            get
            {
                return this._TabPage24;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._TabPage24 = value;
            }
        }

        public virtual TabPage TabPage25
        {
            get
            {
                return this._TabPage25;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._TabPage25 = value;
            }
        }

        public virtual TabPage TabPage26
        {
            get
            {
                return this._TabPage26;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._TabPage26 = value;
            }
        }

        public virtual TabPage TabPage27
        {
            get
            {
                return this._TabPage27;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._TabPage27 = value;
            }
        }

        public virtual TabPage TabPage28
        {
            get
            {
                return this._TabPage28;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._TabPage28 = value;
            }
        }

        public virtual TabPage TabPage29
        {
            get
            {
                return this._TabPage29;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._TabPage29 = value;
            }
        }

        internal virtual TabPage TabPage3
        {
            get
            {
                return this._TabPage3;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._TabPage3 = value;
            }
        }

        public virtual TabPage TabPage30
        {
            get
            {
                return this._TabPage30;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._TabPage30 = value;
            }
        }

        public virtual TabPage TabPage31
        {
            get
            {
                return this._TabPage31;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._TabPage31 = value;
            }
        }

        public virtual TabPage TabPage32
        {
            get
            {
                return this._TabPage32;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._TabPage32 = value;
            }
        }

        public virtual TabPage TabPage9
        {
            get
            {
                return this._TabPage9;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._TabPage9 = value;
            }
        }

        public virtual System.Windows.Forms.Timer tmrKey
        {
            get
            {
                return this._tmrKey;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.tmrKey_Tick);
                if (this._tmrKey != null)
                {
                    this._tmrKey.Tick -= handler;
                }
                this._tmrKey = value;
                if (this._tmrKey != null)
                {
                    this._tmrKey.Tick += handler;
                }
            }
        }

        public virtual System.Windows.Forms.Timer tmrTool
        {
            get
            {
                return this._tmrTool;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.tmrTool_Tick);
                if (this._tmrTool != null)
                {
                    this._tmrTool.Tick -= handler;
                }
                this._tmrTool = value;
                if (this._tmrTool != null)
                {
                    this._tmrTool.Tick += handler;
                }
            }
        }

        public virtual ToolStripLabel ToolStripLabel1
        {
            get
            {
                return this._ToolStripLabel1;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._ToolStripLabel1 = value;
            }
        }

        public virtual ToolStripSeparator ToolStripMenuItem1
        {
            get
            {
                return this._ToolStripMenuItem1;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._ToolStripMenuItem1 = value;
            }
        }

        public virtual ToolStripSeparator ToolStripMenuItem2
        {
            get
            {
                return this._ToolStripMenuItem2;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._ToolStripMenuItem2 = value;
            }
        }

        public virtual ToolStripMenuItem ToolStripMenuItem3
        {
            get
            {
                return this._ToolStripMenuItem3;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.ToolStripMenuItem3_Click);
                if (this._ToolStripMenuItem3 != null)
                {
                    this._ToolStripMenuItem3.Click -= handler;
                }
                this._ToolStripMenuItem3 = value;
                if (this._ToolStripMenuItem3 != null)
                {
                    this._ToolStripMenuItem3.Click += handler;
                }
            }
        }

        public virtual ToolStripMenuItem ToolStripMenuItem4
        {
            get
            {
                return this._ToolStripMenuItem4;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._ToolStripMenuItem4 = value;
            }
        }

        public virtual ToolStripSeparator ToolStripSeparator1
        {
            get
            {
                return this._ToolStripSeparator1;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._ToolStripSeparator1 = value;
            }
        }

        public virtual ToolStripSeparator ToolStripSeparator10
        {
            get
            {
                return this._ToolStripSeparator10;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._ToolStripSeparator10 = value;
            }
        }

        public virtual ToolStripSeparator ToolStripSeparator11
        {
            get
            {
                return this._ToolStripSeparator11;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._ToolStripSeparator11 = value;
            }
        }

        internal virtual ToolStripSeparator ToolStripSeparator12
        {
            get
            {
                return this._ToolStripSeparator12;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._ToolStripSeparator12 = value;
            }
        }

        public virtual ToolStripSeparator ToolStripSeparator2
        {
            get
            {
                return this._ToolStripSeparator2;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._ToolStripSeparator2 = value;
            }
        }

        public virtual ToolStripSeparator ToolStripSeparator3
        {
            get
            {
                return this._ToolStripSeparator3;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._ToolStripSeparator3 = value;
            }
        }

        public virtual ToolStripSeparator ToolStripSeparator4
        {
            get
            {
                return this._ToolStripSeparator4;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._ToolStripSeparator4 = value;
            }
        }

        public virtual ToolStripSeparator ToolStripSeparator5
        {
            get
            {
                return this._ToolStripSeparator5;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._ToolStripSeparator5 = value;
            }
        }

        public virtual ToolStripSeparator ToolStripSeparator6
        {
            get
            {
                return this._ToolStripSeparator6;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._ToolStripSeparator6 = value;
            }
        }

        internal virtual ToolStripSeparator ToolStripSeparator7
        {
            get
            {
                return this._ToolStripSeparator7;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._ToolStripSeparator7 = value;
            }
        }

        internal virtual ToolStripSeparator ToolStripSeparator8
        {
            get
            {
                return this._ToolStripSeparator8;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._ToolStripSeparator8 = value;
            }
        }

        internal virtual ToolStripSeparator ToolStripSeparator9
        {
            get
            {
                return this._ToolStripSeparator9;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._ToolStripSeparator9 = value;
            }
        }

        public virtual TabPage tpAutoTexture
        {
            get
            {
                return this._tpAutoTexture;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._tpAutoTexture = value;
            }
        }

        public virtual TabPage tpHeight
        {
            get
            {
                return this._tpHeight;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._tpHeight = value;
            }
        }

        internal virtual TabPage tpLabels
        {
            get
            {
                return this._tpLabels;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._tpLabels = value;
            }
        }

        public virtual TabPage tpObject
        {
            get
            {
                return this._tpObject;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._tpObject = value;
            }
        }

        public virtual TabPage tpObjects
        {
            get
            {
                return this._tpObjects;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._tpObjects = value;
            }
        }

        public virtual TabPage tpResize
        {
            get
            {
                return this._tpResize;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._tpResize = value;
            }
        }

        public virtual TabPage tpTextures
        {
            get
            {
                return this._tpTextures;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._tpTextures = value;
            }
        }

        public virtual ToolStripButton tsbDrawAutotexture
        {
            get
            {
                return this._tsbDrawAutotexture;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.tsbDrawAutotexture_Click);
                if (this._tsbDrawAutotexture != null)
                {
                    this._tsbDrawAutotexture.Click -= handler;
                }
                this._tsbDrawAutotexture = value;
                if (this._tsbDrawAutotexture != null)
                {
                    this._tsbDrawAutotexture.Click += handler;
                }
            }
        }

        public virtual ToolStripButton tsbDrawTileOrientation
        {
            get
            {
                return this._tsbDrawTileOrientation;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.tsbDrawTileOrientation_Click);
                if (this._tsbDrawTileOrientation != null)
                {
                    this._tsbDrawTileOrientation.Click -= handler;
                }
                this._tsbDrawTileOrientation = value;
                if (this._tsbDrawTileOrientation != null)
                {
                    this._tsbDrawTileOrientation.Click += handler;
                }
            }
        }

        public virtual ToolStripButton tsbGateways
        {
            get
            {
                return this._tsbGateways;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.tsbGateways_Click);
                if (this._tsbGateways != null)
                {
                    this._tsbGateways.Click -= handler;
                }
                this._tsbGateways = value;
                if (this._tsbGateways != null)
                {
                    this._tsbGateways.Click += handler;
                }
            }
        }

        public virtual ToolStripButton tsbSave
        {
            get
            {
                return this._tsbSave;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.tsbSave_Click);
                if (this._tsbSave != null)
                {
                    this._tsbSave.Click -= handler;
                }
                this._tsbSave = value;
                if (this._tsbSave != null)
                {
                    this._tsbSave.Click += handler;
                }
            }
        }

        public virtual ToolStripButton tsbSelection
        {
            get
            {
                return this._tsbSelection;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.tsbSelection_Click);
                if (this._tsbSelection != null)
                {
                    this._tsbSelection.Click -= handler;
                }
                this._tsbSelection = value;
                if (this._tsbSelection != null)
                {
                    this._tsbSelection.Click += handler;
                }
            }
        }

        public virtual ToolStripButton tsbSelectionCopy
        {
            get
            {
                return this._tsbSelectionCopy;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.tsbSelectionCopy_Click);
                if (this._tsbSelectionCopy != null)
                {
                    this._tsbSelectionCopy.Click -= handler;
                }
                this._tsbSelectionCopy = value;
                if (this._tsbSelectionCopy != null)
                {
                    this._tsbSelectionCopy.Click += handler;
                }
            }
        }

        public virtual ToolStripButton tsbSelectionFlipX
        {
            get
            {
                return this._tsbSelectionFlipX;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.tsbSelectionFlipX_Click);
                if (this._tsbSelectionFlipX != null)
                {
                    this._tsbSelectionFlipX.Click -= handler;
                }
                this._tsbSelectionFlipX = value;
                if (this._tsbSelectionFlipX != null)
                {
                    this._tsbSelectionFlipX.Click += handler;
                }
            }
        }

        public virtual ToolStripButton tsbSelectionObjects
        {
            get
            {
                return this._tsbSelectionObjects;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.tsbSelectionObjects_Click);
                if (this._tsbSelectionObjects != null)
                {
                    this._tsbSelectionObjects.Click -= handler;
                }
                this._tsbSelectionObjects = value;
                if (this._tsbSelectionObjects != null)
                {
                    this._tsbSelectionObjects.Click += handler;
                }
            }
        }

        public virtual ToolStripButton tsbSelectionPaste
        {
            get
            {
                return this._tsbSelectionPaste;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.tsbSelectionPaste_Click);
                if (this._tsbSelectionPaste != null)
                {
                    this._tsbSelectionPaste.Click -= handler;
                }
                this._tsbSelectionPaste = value;
                if (this._tsbSelectionPaste != null)
                {
                    this._tsbSelectionPaste.Click += handler;
                }
            }
        }

        public virtual ToolStripDropDownButton tsbSelectionPasteOptions
        {
            get
            {
                return this._tsbSelectionPasteOptions;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._tsbSelectionPasteOptions = value;
            }
        }

        public virtual ToolStripButton tsbSelectionRotateClockwise
        {
            get
            {
                return this._tsbSelectionRotateClockwise;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.tsbSelectionRotateClockwise_Click);
                if (this._tsbSelectionRotateClockwise != null)
                {
                    this._tsbSelectionRotateClockwise.Click -= handler;
                }
                this._tsbSelectionRotateClockwise = value;
                if (this._tsbSelectionRotateClockwise != null)
                {
                    this._tsbSelectionRotateClockwise.Click += handler;
                }
            }
        }

        public virtual ToolStripButton tsbSelectionRotateCounterClockwise
        {
            get
            {
                return this._tsbSelectionRotateCounterClockwise;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.tsbSelectionRotateAnticlockwise_Click);
                if (this._tsbSelectionRotateCounterClockwise != null)
                {
                    this._tsbSelectionRotateCounterClockwise.Click -= handler;
                }
                this._tsbSelectionRotateCounterClockwise = value;
                if (this._tsbSelectionRotateCounterClockwise != null)
                {
                    this._tsbSelectionRotateCounterClockwise.Click += handler;
                }
            }
        }

        public virtual ToolStrip tsFile
        {
            get
            {
                return this._tsFile;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._tsFile = value;
            }
        }

        public virtual ToolStrip tsMinimap
        {
            get
            {
                return this._tsMinimap;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._tsMinimap = value;
            }
        }

        public virtual ToolStrip tsSelection
        {
            get
            {
                return this._tsSelection;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._tsSelection = value;
            }
        }

        public virtual ToolStrip tsTools
        {
            get
            {
                return this._tsTools;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._tsTools = value;
            }
        }

        public virtual TextBox txtAutoCliffSlope
        {
            get
            {
                return this._txtAutoCliffSlope;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._txtAutoCliffSlope = value;
            }
        }

        public virtual TextBox txtHeightChangeRate
        {
            get
            {
                return this._txtHeightChangeRate;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._txtHeightChangeRate = value;
            }
        }

        public virtual TextBox txtHeightMultiply
        {
            get
            {
                return this._txtHeightMultiply;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._txtHeightMultiply = value;
            }
        }

        public virtual TextBox txtHeightOffset
        {
            get
            {
                return this._txtHeightOffset;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._txtHeightOffset = value;
            }
        }

        public virtual TextBox txtHeightSetL
        {
            get
            {
                return this._txtHeightSetL;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.txtHeightSetL_LostFocus);
                if (this._txtHeightSetL != null)
                {
                    this._txtHeightSetL.Leave -= handler;
                }
                this._txtHeightSetL = value;
                if (this._txtHeightSetL != null)
                {
                    this._txtHeightSetL.Leave += handler;
                }
            }
        }

        public virtual TextBox txtHeightSetR
        {
            get
            {
                return this._txtHeightSetR;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.txtHeightSetR_LostFocus);
                if (this._txtHeightSetR != null)
                {
                    this._txtHeightSetR.Leave -= handler;
                }
                this._txtHeightSetR = value;
                if (this._txtHeightSetR != null)
                {
                    this._txtHeightSetR.Leave += handler;
                }
            }
        }

        public virtual TextBox txtNewObjectRotation
        {
            get
            {
                return this._txtNewObjectRotation;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._txtNewObjectRotation = value;
            }
        }

        internal virtual TextBox txtObjectFind
        {
            get
            {
                return this._txtObjectFind;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.txtObjectFind_Leave);
                KeyEventHandler handler2 = new KeyEventHandler(this.txtObjectFind_KeyDown);
                if (this._txtObjectFind != null)
                {
                    this._txtObjectFind.Leave -= handler;
                    this._txtObjectFind.KeyDown -= handler2;
                }
                this._txtObjectFind = value;
                if (this._txtObjectFind != null)
                {
                    this._txtObjectFind.Leave += handler;
                    this._txtObjectFind.KeyDown += handler2;
                }
            }
        }

        public virtual TextBox txtObjectHealth
        {
            get
            {
                return this._txtObjectHealth;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.txtObjectHealth_LostFocus);
                if (this._txtObjectHealth != null)
                {
                    this._txtObjectHealth.Leave -= handler;
                }
                this._txtObjectHealth = value;
                if (this._txtObjectHealth != null)
                {
                    this._txtObjectHealth.Leave += handler;
                }
            }
        }

        public virtual TextBox txtObjectID
        {
            get
            {
                return this._txtObjectID;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._txtObjectID = value;
            }
        }

        public virtual TextBox txtObjectLabel
        {
            get
            {
                return this._txtObjectLabel;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.txtObjectLabel_LostFocus);
                if (this._txtObjectLabel != null)
                {
                    this._txtObjectLabel.Leave -= handler;
                }
                this._txtObjectLabel = value;
                if (this._txtObjectLabel != null)
                {
                    this._txtObjectLabel.Leave += handler;
                }
            }
        }

        public virtual TextBox txtObjectPriority
        {
            get
            {
                return this._txtObjectPriority;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.txtObjectPriority_LostFocus);
                if (this._txtObjectPriority != null)
                {
                    this._txtObjectPriority.Leave -= handler;
                }
                this._txtObjectPriority = value;
                if (this._txtObjectPriority != null)
                {
                    this._txtObjectPriority.Leave += handler;
                }
            }
        }

        public virtual TextBox txtObjectRotation
        {
            get
            {
                return this._txtObjectRotation;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.txtObjectRotation_LostFocus);
                if (this._txtObjectRotation != null)
                {
                    this._txtObjectRotation.Leave -= handler;
                }
                this._txtObjectRotation = value;
                if (this._txtObjectRotation != null)
                {
                    this._txtObjectRotation.Leave += handler;
                }
            }
        }

        public virtual TextBox txtOffsetX
        {
            get
            {
                return this._txtOffsetX;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._txtOffsetX = value;
            }
        }

        public virtual TextBox txtOffsetY
        {
            get
            {
                return this._txtOffsetY;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._txtOffsetY = value;
            }
        }

        public virtual TextBox txtScriptMarkerLabel
        {
            get
            {
                return this._txtScriptMarkerLabel;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.txtScriptMarkerLabel_LostFocus);
                if (this._txtScriptMarkerLabel != null)
                {
                    this._txtScriptMarkerLabel.Leave -= handler;
                }
                this._txtScriptMarkerLabel = value;
                if (this._txtScriptMarkerLabel != null)
                {
                    this._txtScriptMarkerLabel.Leave += handler;
                }
            }
        }

        public virtual TextBox txtScriptMarkerX
        {
            get
            {
                return this._txtScriptMarkerX;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.txtScriptMarkerX_LostFocus);
                if (this._txtScriptMarkerX != null)
                {
                    this._txtScriptMarkerX.Leave -= handler;
                }
                this._txtScriptMarkerX = value;
                if (this._txtScriptMarkerX != null)
                {
                    this._txtScriptMarkerX.Leave += handler;
                }
            }
        }

        public virtual TextBox txtScriptMarkerX2
        {
            get
            {
                return this._txtScriptMarkerX2;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.txtScriptMarkerX2_LostFocus);
                if (this._txtScriptMarkerX2 != null)
                {
                    this._txtScriptMarkerX2.Leave -= handler;
                }
                this._txtScriptMarkerX2 = value;
                if (this._txtScriptMarkerX2 != null)
                {
                    this._txtScriptMarkerX2.Leave += handler;
                }
            }
        }

        public virtual TextBox txtScriptMarkerY
        {
            get
            {
                return this._txtScriptMarkerY;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.txtScriptMarkerY_LostFocus);
                if (this._txtScriptMarkerY != null)
                {
                    this._txtScriptMarkerY.Leave -= handler;
                }
                this._txtScriptMarkerY = value;
                if (this._txtScriptMarkerY != null)
                {
                    this._txtScriptMarkerY.Leave += handler;
                }
            }
        }

        public virtual TextBox txtScriptMarkerY2
        {
            get
            {
                return this._txtScriptMarkerY2;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.txtScriptMarkerY2_LostFocus);
                if (this._txtScriptMarkerY2 != null)
                {
                    this._txtScriptMarkerY2.Leave -= handler;
                }
                this._txtScriptMarkerY2 = value;
                if (this._txtScriptMarkerY2 != null)
                {
                    this._txtScriptMarkerY2.Leave += handler;
                }
            }
        }

        public virtual TextBox txtSizeX
        {
            get
            {
                return this._txtSizeX;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._txtSizeX = value;
            }
        }

        public virtual TextBox txtSizeY
        {
            get
            {
                return this._txtSizeY;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._txtSizeY = value;
            }
        }

        public virtual TextBox txtSmoothRate
        {
            get
            {
                return this._txtSmoothRate;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._txtSmoothRate = value;
            }
        }

        public class clsMaps : modLists.ConnectedList<clsMap, frmMain>
        {
            private clsMap _MainMap;

            public clsMaps(frmMain Owner) : base(Owner)
            {
                this.MaintainOrder = true;
            }

            public override void Add(modLists.ConnectedListItem<clsMap, frmMain> NewItem)
            {
                clsMap item = NewItem.Item;
                if (!item.ReadyForUserInput)
                {
                    item.InitializeUserInput();
                }
                item.MapView_TabPage = new TabPage();
                item.MapView_TabPage.Tag = item;
                item.SetTabText();
                base.Add(NewItem);
                this.Owner.MapView.UpdateTabs();
            }

            public override void Remove(int Position)
            {
                clsMap map = this[Position];
                base.Remove(Position);
                if (map == this._MainMap)
                {
                    int num = Math.Min(this.Owner.MapView.tabMaps.SelectedIndex, this.Count - 1);
                    if (num < 0)
                    {
                        this.MainMap = null;
                    }
                    else
                    {
                        this.MainMap = this[num];
                    }
                }
                map.MapView_TabPage.Tag = null;
                map.MapView_TabPage = null;
                this.Owner.MapView.UpdateTabs();
            }

            public clsMap MainMap
            {
                get
                {
                    return this._MainMap;
                }
                set
                {
                    if (value != this._MainMap)
                    {
                        frmMain owner = this.Owner;
                        owner.MainMapBeforeChanged();
                        if (value == null)
                        {
                            this._MainMap = null;
                        }
                        else if (value.frmMainLink.Source != this.Owner)
                        {
                            Interaction.MsgBox("Error: Assigning map to wrong main form.", MsgBoxStyle.ApplicationModal, null);
                            this._MainMap = null;
                        }
                        else
                        {
                            this._MainMap = value;
                        }
                        owner.MainMapAfterChanged();
                    }
                }
            }
        }

        public class clsSplashScreen
        {
            public frmSplash Form = new frmSplash();

            public clsSplashScreen()
            {
                this.Form.Icon = modProgram.ProgramIcon;
            }
        }
    }
}

