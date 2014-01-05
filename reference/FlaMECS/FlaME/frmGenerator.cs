namespace FlaME
{
    using Matrix3D;
    using Microsoft.VisualBasic;
    using Microsoft.VisualBasic.CompilerServices;
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Runtime.CompilerServices;
    using System.Windows.Forms;

    [DesignerGenerated]
    public class frmGenerator : Form
    {
        [AccessedThroughProperty("btnGenerateLayout")]
        private Button _btnGenerateLayout;
        [AccessedThroughProperty("btnGenerateObjects")]
        private Button _btnGenerateObjects;
        [AccessedThroughProperty("btnGenerateRamps")]
        private Button _btnGenerateRamps;
        [AccessedThroughProperty("btnGenerateTextures")]
        private Button _btnGenerateTextures;
        [AccessedThroughProperty("btnStop")]
        private Button _btnStop;
        [AccessedThroughProperty("cboSymmetry")]
        private ComboBox _cboSymmetry;
        [AccessedThroughProperty("cboTileset")]
        private ComboBox _cboTileset;
        [AccessedThroughProperty("cbxMasterTexture")]
        private CheckBox _cbxMasterTexture;
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
        [AccessedThroughProperty("Label3")]
        private Label _Label3;
        [AccessedThroughProperty("Label31")]
        private Label _Label31;
        [AccessedThroughProperty("Label33")]
        private Label _Label33;
        [AccessedThroughProperty("Label34")]
        private Label _Label34;
        [AccessedThroughProperty("Label4")]
        private Label _Label4;
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
        [AccessedThroughProperty("lstResult")]
        private ListBox _lstResult;
        private frmMain _Owner;
        [AccessedThroughProperty("rdoPlayer1")]
        private RadioButton _rdoPlayer1;
        [AccessedThroughProperty("rdoPlayer10")]
        private RadioButton _rdoPlayer10;
        [AccessedThroughProperty("rdoPlayer2")]
        private RadioButton _rdoPlayer2;
        [AccessedThroughProperty("rdoPlayer3")]
        private RadioButton _rdoPlayer3;
        [AccessedThroughProperty("rdoPlayer4")]
        private RadioButton _rdoPlayer4;
        [AccessedThroughProperty("rdoPlayer5")]
        private RadioButton _rdoPlayer5;
        [AccessedThroughProperty("rdoPlayer6")]
        private RadioButton _rdoPlayer6;
        [AccessedThroughProperty("rdoPlayer7")]
        private RadioButton _rdoPlayer7;
        [AccessedThroughProperty("rdoPlayer8")]
        private RadioButton _rdoPlayer8;
        [AccessedThroughProperty("rdoPlayer9")]
        private RadioButton _rdoPlayer9;
        [AccessedThroughProperty("TabControl1")]
        private TabControl _TabControl1;
        [AccessedThroughProperty("TabPage1")]
        private TabPage _TabPage1;
        [AccessedThroughProperty("TabPage2")]
        private TabPage _TabPage2;
        [AccessedThroughProperty("TabPage3")]
        private TabPage _TabPage3;
        [AccessedThroughProperty("TabPage4")]
        private TabPage _TabPage4;
        [AccessedThroughProperty("txt10x")]
        private TextBox _txt10x;
        [AccessedThroughProperty("txt10y")]
        private TextBox _txt10y;
        [AccessedThroughProperty("txt1x")]
        private TextBox _txt1x;
        [AccessedThroughProperty("txt1y")]
        private TextBox _txt1y;
        [AccessedThroughProperty("txt2x")]
        private TextBox _txt2x;
        [AccessedThroughProperty("txt2y")]
        private TextBox _txt2y;
        [AccessedThroughProperty("txt3x")]
        private TextBox _txt3x;
        [AccessedThroughProperty("txt3y")]
        private TextBox _txt3y;
        [AccessedThroughProperty("txt4x")]
        private TextBox _txt4x;
        [AccessedThroughProperty("txt4y")]
        private TextBox _txt4y;
        [AccessedThroughProperty("txt5x")]
        private TextBox _txt5x;
        [AccessedThroughProperty("txt5y")]
        private TextBox _txt5y;
        [AccessedThroughProperty("txt6x")]
        private TextBox _txt6x;
        [AccessedThroughProperty("txt6y")]
        private TextBox _txt6y;
        [AccessedThroughProperty("txt7x")]
        private TextBox _txt7x;
        [AccessedThroughProperty("txt7y")]
        private TextBox _txt7y;
        [AccessedThroughProperty("txt8x")]
        private TextBox _txt8x;
        [AccessedThroughProperty("txt8y")]
        private TextBox _txt8y;
        [AccessedThroughProperty("txt9x")]
        private TextBox _txt9x;
        [AccessedThroughProperty("txt9y")]
        private TextBox _txt9y;
        [AccessedThroughProperty("txtBaseArea")]
        private TextBox _txtBaseArea;
        [AccessedThroughProperty("txtBaseLevel")]
        private TextBox _txtBaseLevel;
        [AccessedThroughProperty("txtBaseOil")]
        private TextBox _txtBaseOil;
        [AccessedThroughProperty("txtConnectedWater")]
        private TextBox _txtConnectedWater;
        [AccessedThroughProperty("txtFClusterChance")]
        private TextBox _txtFClusterChance;
        [AccessedThroughProperty("txtFClusterMax")]
        private TextBox _txtFClusterMax;
        [AccessedThroughProperty("txtFClusterMin")]
        private TextBox _txtFClusterMin;
        [AccessedThroughProperty("txtFlatness")]
        private TextBox _txtFlatness;
        [AccessedThroughProperty("txtFScatterChance")]
        private TextBox _txtFScatterChance;
        [AccessedThroughProperty("txtFScatterGap")]
        private TextBox _txtFScatterGap;
        [AccessedThroughProperty("txtHeight")]
        private TextBox _txtHeight;
        [AccessedThroughProperty("txtLevelFrequency")]
        private TextBox _txtLevelFrequency;
        [AccessedThroughProperty("txtLevels")]
        private TextBox _txtLevels;
        [AccessedThroughProperty("txtOilAtATime")]
        private TextBox _txtOilAtATime;
        [AccessedThroughProperty("txtOilClusterMax")]
        private TextBox _txtOilClusterMax;
        [AccessedThroughProperty("txtOilClusterMin")]
        private TextBox _txtOilClusterMin;
        [AccessedThroughProperty("txtOilDispersion")]
        private TextBox _txtOilDispersion;
        [AccessedThroughProperty("txtOilElsewhere")]
        private TextBox _txtOilElsewhere;
        [AccessedThroughProperty("txtRampBase")]
        private TextBox _txtRampBase;
        [AccessedThroughProperty("txtRampDistance")]
        private TextBox _txtRampDistance;
        [AccessedThroughProperty("txtTrucks")]
        private TextBox _txtTrucks;
        [AccessedThroughProperty("txtVariation")]
        private TextBox _txtVariation;
        [AccessedThroughProperty("txtWaterQuantity")]
        private TextBox _txtWaterQuantity;
        [AccessedThroughProperty("txtWidth")]
        private TextBox _txtWidth;
        private IContainer components;
        private clsGenerateMap Generator;
        private int PlayerCount;
        private bool StopTrying;

        public frmGenerator(frmMain Owner)
        {
            base.FormClosing += new FormClosingEventHandler(this.frmGenerator_FormClosing);
            base.Load += new EventHandler(this.frmWZMapGen_Load);
            this.PlayerCount = 4;
            this.Generator = new clsGenerateMap();
            this.InitializeComponent();
            this._Owner = Owner;
        }

        private void btnGenerateLayout_Click(object sender, EventArgs e)
        {
            this.lstResult.Items.Clear();
            this.btnGenerateLayout.Enabled = false;
            this.lstResult_AddText("Generating layout.");
            Application.DoEvents();
            this.StopTrying = false;
            this.Generator.ClearLayout();
            this.Generator.GenerateTileset = null;
            this.Generator.Map = null;
            this.Generator.TopLeftPlayerCount = this.PlayerCount;
            switch (this.cboSymmetry.SelectedIndex)
            {
                case 0:
                    this.Generator.SymmetryBlockCountXY.X = 1;
                    this.Generator.SymmetryBlockCountXY.Y = 1;
                    this.Generator.SymmetryBlockCount = 1;
                    this.Generator.SymmetryBlocks = new clsGenerateMap.sSymmetryBlock[(this.Generator.SymmetryBlockCount - 1) + 1];
                    this.Generator.SymmetryBlocks[0].XYNum = new modMath.sXY_int(0, 0);
                    this.Generator.SymmetryBlocks[0].Orientation = new TileOrientation.sTileOrientation(false, false, false);
                    this.Generator.SymmetryIsRotational = false;
                    break;

                case 1:
                    this.Generator.SymmetryBlockCountXY.X = 2;
                    this.Generator.SymmetryBlockCountXY.Y = 1;
                    this.Generator.SymmetryBlockCount = 2;
                    this.Generator.SymmetryBlocks = new clsGenerateMap.sSymmetryBlock[(this.Generator.SymmetryBlockCount - 1) + 1];
                    this.Generator.SymmetryBlocks[0].XYNum = new modMath.sXY_int(0, 0);
                    this.Generator.SymmetryBlocks[0].Orientation = new TileOrientation.sTileOrientation(false, false, false);
                    this.Generator.SymmetryBlocks[0].ReflectToNum = new int[(((int) Math.Round((double) (((double) this.Generator.SymmetryBlockCount) / 2.0))) - 1) + 1];
                    this.Generator.SymmetryBlocks[0].ReflectToNum[0] = 1;
                    this.Generator.SymmetryBlocks[1].XYNum = new modMath.sXY_int(1, 0);
                    this.Generator.SymmetryBlocks[1].Orientation = new TileOrientation.sTileOrientation(true, true, false);
                    this.Generator.SymmetryBlocks[1].ReflectToNum = new int[(((int) Math.Round((double) (((double) this.Generator.SymmetryBlockCount) / 2.0))) - 1) + 1];
                    this.Generator.SymmetryBlocks[1].ReflectToNum[0] = 0;
                    this.Generator.SymmetryIsRotational = true;
                    break;

                case 2:
                    this.Generator.SymmetryBlockCountXY.X = 1;
                    this.Generator.SymmetryBlockCountXY.Y = 2;
                    this.Generator.SymmetryBlockCount = 2;
                    this.Generator.SymmetryBlocks = new clsGenerateMap.sSymmetryBlock[(this.Generator.SymmetryBlockCount - 1) + 1];
                    this.Generator.SymmetryBlocks[0].XYNum = new modMath.sXY_int(0, 0);
                    this.Generator.SymmetryBlocks[0].Orientation = new TileOrientation.sTileOrientation(false, false, false);
                    this.Generator.SymmetryBlocks[0].ReflectToNum = new int[(((int) Math.Round((double) (((double) this.Generator.SymmetryBlockCount) / 2.0))) - 1) + 1];
                    this.Generator.SymmetryBlocks[0].ReflectToNum[0] = 1;
                    this.Generator.SymmetryBlocks[1].XYNum = new modMath.sXY_int(0, 1);
                    this.Generator.SymmetryBlocks[1].Orientation = new TileOrientation.sTileOrientation(true, true, false);
                    this.Generator.SymmetryBlocks[1].ReflectToNum = new int[(((int) Math.Round((double) (((double) this.Generator.SymmetryBlockCount) / 2.0))) - 1) + 1];
                    this.Generator.SymmetryBlocks[1].ReflectToNum[0] = 0;
                    this.Generator.SymmetryIsRotational = true;
                    break;

                case 3:
                    this.Generator.SymmetryBlockCountXY.X = 2;
                    this.Generator.SymmetryBlockCountXY.Y = 1;
                    this.Generator.SymmetryBlockCount = 2;
                    this.Generator.SymmetryBlocks = new clsGenerateMap.sSymmetryBlock[(this.Generator.SymmetryBlockCount - 1) + 1];
                    this.Generator.SymmetryBlocks[0].XYNum = new modMath.sXY_int(0, 0);
                    this.Generator.SymmetryBlocks[0].Orientation = new TileOrientation.sTileOrientation(false, false, false);
                    this.Generator.SymmetryBlocks[0].ReflectToNum = new int[(((int) Math.Round((double) (((double) this.Generator.SymmetryBlockCount) / 2.0))) - 1) + 1];
                    this.Generator.SymmetryBlocks[0].ReflectToNum[0] = 1;
                    this.Generator.SymmetryBlocks[1].XYNum = new modMath.sXY_int(1, 0);
                    this.Generator.SymmetryBlocks[1].Orientation = new TileOrientation.sTileOrientation(true, false, false);
                    this.Generator.SymmetryBlocks[1].ReflectToNum = new int[(((int) Math.Round((double) (((double) this.Generator.SymmetryBlockCount) / 2.0))) - 1) + 1];
                    this.Generator.SymmetryBlocks[1].ReflectToNum[0] = 0;
                    this.Generator.SymmetryIsRotational = false;
                    break;

                case 4:
                    this.Generator.SymmetryBlockCountXY.X = 1;
                    this.Generator.SymmetryBlockCountXY.Y = 2;
                    this.Generator.SymmetryBlockCount = 2;
                    this.Generator.SymmetryBlocks = new clsGenerateMap.sSymmetryBlock[(this.Generator.SymmetryBlockCount - 1) + 1];
                    this.Generator.SymmetryBlocks[0].XYNum = new modMath.sXY_int(0, 0);
                    this.Generator.SymmetryBlocks[0].Orientation = new TileOrientation.sTileOrientation(false, false, false);
                    this.Generator.SymmetryBlocks[0].ReflectToNum = new int[(((int) Math.Round((double) (((double) this.Generator.SymmetryBlockCount) / 2.0))) - 1) + 1];
                    this.Generator.SymmetryBlocks[0].ReflectToNum[0] = 1;
                    this.Generator.SymmetryBlocks[1].XYNum = new modMath.sXY_int(0, 1);
                    this.Generator.SymmetryBlocks[1].Orientation = new TileOrientation.sTileOrientation(false, true, false);
                    this.Generator.SymmetryBlocks[1].ReflectToNum = new int[(((int) Math.Round((double) (((double) this.Generator.SymmetryBlockCount) / 2.0))) - 1) + 1];
                    this.Generator.SymmetryBlocks[1].ReflectToNum[0] = 0;
                    this.Generator.SymmetryIsRotational = false;
                    break;

                case 5:
                    this.Generator.SymmetryBlockCountXY.X = 2;
                    this.Generator.SymmetryBlockCountXY.Y = 2;
                    this.Generator.SymmetryBlockCount = 4;
                    this.Generator.SymmetryBlocks = new clsGenerateMap.sSymmetryBlock[(this.Generator.SymmetryBlockCount - 1) + 1];
                    this.Generator.SymmetryBlocks[0].XYNum = new modMath.sXY_int(0, 0);
                    this.Generator.SymmetryBlocks[0].Orientation = new TileOrientation.sTileOrientation(false, false, false);
                    this.Generator.SymmetryBlocks[0].ReflectToNum = new int[(((int) Math.Round((double) (((double) this.Generator.SymmetryBlockCount) / 2.0))) - 1) + 1];
                    this.Generator.SymmetryBlocks[0].ReflectToNum[0] = 1;
                    this.Generator.SymmetryBlocks[0].ReflectToNum[1] = 2;
                    this.Generator.SymmetryBlocks[1].XYNum = new modMath.sXY_int(1, 0);
                    this.Generator.SymmetryBlocks[1].Orientation = new TileOrientation.sTileOrientation(true, false, true);
                    this.Generator.SymmetryBlocks[1].ReflectToNum = new int[(((int) Math.Round((double) (((double) this.Generator.SymmetryBlockCount) / 2.0))) - 1) + 1];
                    this.Generator.SymmetryBlocks[1].ReflectToNum[0] = 3;
                    this.Generator.SymmetryBlocks[1].ReflectToNum[1] = 0;
                    this.Generator.SymmetryBlocks[2].XYNum = new modMath.sXY_int(0, 1);
                    this.Generator.SymmetryBlocks[2].Orientation = new TileOrientation.sTileOrientation(false, true, true);
                    this.Generator.SymmetryBlocks[2].ReflectToNum = new int[(((int) Math.Round((double) (((double) this.Generator.SymmetryBlockCount) / 2.0))) - 1) + 1];
                    this.Generator.SymmetryBlocks[2].ReflectToNum[0] = 0;
                    this.Generator.SymmetryBlocks[2].ReflectToNum[1] = 3;
                    this.Generator.SymmetryBlocks[3].XYNum = new modMath.sXY_int(1, 1);
                    this.Generator.SymmetryBlocks[3].Orientation = new TileOrientation.sTileOrientation(true, true, false);
                    this.Generator.SymmetryBlocks[3].ReflectToNum = new int[(((int) Math.Round((double) (((double) this.Generator.SymmetryBlockCount) / 2.0))) - 1) + 1];
                    this.Generator.SymmetryBlocks[3].ReflectToNum[0] = 2;
                    this.Generator.SymmetryBlocks[3].ReflectToNum[1] = 1;
                    this.Generator.SymmetryIsRotational = true;
                    break;

                case 6:
                    this.Generator.SymmetryBlockCountXY.X = 2;
                    this.Generator.SymmetryBlockCountXY.Y = 2;
                    this.Generator.SymmetryBlockCount = 4;
                    this.Generator.SymmetryBlocks = new clsGenerateMap.sSymmetryBlock[(this.Generator.SymmetryBlockCount - 1) + 1];
                    this.Generator.SymmetryBlocks[0].XYNum = new modMath.sXY_int(0, 0);
                    this.Generator.SymmetryBlocks[0].Orientation = new TileOrientation.sTileOrientation(false, false, false);
                    this.Generator.SymmetryBlocks[0].ReflectToNum = new int[(((int) Math.Round((double) (((double) this.Generator.SymmetryBlockCount) / 2.0))) - 1) + 1];
                    this.Generator.SymmetryBlocks[0].ReflectToNum[0] = 1;
                    this.Generator.SymmetryBlocks[0].ReflectToNum[1] = 2;
                    this.Generator.SymmetryBlocks[1].XYNum = new modMath.sXY_int(1, 0);
                    this.Generator.SymmetryBlocks[1].Orientation = new TileOrientation.sTileOrientation(true, false, false);
                    this.Generator.SymmetryBlocks[1].ReflectToNum = new int[(((int) Math.Round((double) (((double) this.Generator.SymmetryBlockCount) / 2.0))) - 1) + 1];
                    this.Generator.SymmetryBlocks[1].ReflectToNum[0] = 0;
                    this.Generator.SymmetryBlocks[1].ReflectToNum[1] = 3;
                    this.Generator.SymmetryBlocks[2].XYNum = new modMath.sXY_int(0, 1);
                    this.Generator.SymmetryBlocks[2].Orientation = new TileOrientation.sTileOrientation(false, true, false);
                    this.Generator.SymmetryBlocks[2].ReflectToNum = new int[(((int) Math.Round((double) (((double) this.Generator.SymmetryBlockCount) / 2.0))) - 1) + 1];
                    this.Generator.SymmetryBlocks[2].ReflectToNum[0] = 3;
                    this.Generator.SymmetryBlocks[2].ReflectToNum[1] = 0;
                    this.Generator.SymmetryBlocks[3].XYNum = new modMath.sXY_int(1, 1);
                    this.Generator.SymmetryBlocks[3].Orientation = new TileOrientation.sTileOrientation(true, true, false);
                    this.Generator.SymmetryBlocks[3].ReflectToNum = new int[(((int) Math.Round((double) (((double) this.Generator.SymmetryBlockCount) / 2.0))) - 1) + 1];
                    this.Generator.SymmetryBlocks[3].ReflectToNum[0] = 2;
                    this.Generator.SymmetryBlocks[3].ReflectToNum[1] = 1;
                    this.Generator.SymmetryIsRotational = false;
                    break;

                default:
                    Interaction.MsgBox("Select symmetry", MsgBoxStyle.ApplicationModal, null);
                    this.btnGenerateLayout.Enabled = true;
                    return;
            }
            if ((this.Generator.TopLeftPlayerCount * this.Generator.SymmetryBlockCount) < 2)
            {
                Interaction.MsgBox("That configuration only produces 1 player.", MsgBoxStyle.ApplicationModal, null);
                this.btnGenerateLayout.Enabled = true;
            }
            else if ((this.Generator.TopLeftPlayerCount * this.Generator.SymmetryBlockCount) > 10)
            {
                Interaction.MsgBox("That configuration produces more than 10 players.", MsgBoxStyle.ApplicationModal, null);
                this.btnGenerateLayout.Enabled = true;
            }
            else
            {
                this.Generator.TileSize.X = this.ValidateTextbox(this.txtWidth, 48.0, 250.0, 1.0);
                this.Generator.TileSize.Y = this.ValidateTextbox(this.txtHeight, 48.0, 250.0, 1.0);
                if ((this.Generator.SymmetryBlockCount == 4) && ((this.Generator.TileSize.X != this.Generator.TileSize.Y) & this.Generator.SymmetryIsRotational))
                {
                    Interaction.MsgBox("Width and height must be equal if map is rotated on two axes.", MsgBoxStyle.ApplicationModal, null);
                    this.btnGenerateLayout.Enabled = true;
                }
                else
                {
                    clsResult result;
                    this.Generator.PlayerBasePos = new modMath.sXY_int[(this.Generator.TopLeftPlayerCount - 1) + 1];
                    double min = 12.0;
                    double x = Math.Min((double) (((double) this.Generator.TileSize.X) / ((double) this.Generator.SymmetryBlockCountXY.X)), (double) (this.Generator.TileSize.X - 12.0));
                    Position.XY_dbl _dbl = new Position.XY_dbl(x, Math.Min((double) (((double) this.Generator.TileSize.Y) / ((double) this.Generator.SymmetryBlockCountXY.Y)), (double) (this.Generator.TileSize.Y - 12.0)));
                    this.Generator.PlayerBasePos[0] = new modMath.sXY_int(this.ValidateTextbox(this.txt1x, min, _dbl.X, 128.0), this.ValidateTextbox(this.txt1y, min, _dbl.X, 128.0));
                    if (this.Generator.TopLeftPlayerCount >= 2)
                    {
                        this.Generator.PlayerBasePos[1] = new modMath.sXY_int(this.ValidateTextbox(this.txt2x, min, _dbl.X, 128.0), this.ValidateTextbox(this.txt2y, min, _dbl.Y, 128.0));
                        if (this.Generator.TopLeftPlayerCount >= 3)
                        {
                            this.Generator.PlayerBasePos[2] = new modMath.sXY_int(this.ValidateTextbox(this.txt3x, min, _dbl.X, 128.0), this.ValidateTextbox(this.txt3y, min, _dbl.Y, 128.0));
                            if (this.Generator.TopLeftPlayerCount >= 4)
                            {
                                this.Generator.PlayerBasePos[3] = new modMath.sXY_int(this.ValidateTextbox(this.txt4x, min, _dbl.X, 128.0), this.ValidateTextbox(this.txt4y, min, _dbl.Y, 128.0));
                                if (this.Generator.TopLeftPlayerCount >= 5)
                                {
                                    this.Generator.PlayerBasePos[4] = new modMath.sXY_int(this.ValidateTextbox(this.txt5x, min, _dbl.X, 128.0), this.ValidateTextbox(this.txt5y, min, _dbl.Y, 128.0));
                                    if (this.Generator.TopLeftPlayerCount >= 6)
                                    {
                                        this.Generator.PlayerBasePos[5] = new modMath.sXY_int(this.ValidateTextbox(this.txt6x, min, _dbl.X, 128.0), this.ValidateTextbox(this.txt6y, min, _dbl.Y, 128.0));
                                        if (this.Generator.TopLeftPlayerCount >= 7)
                                        {
                                            this.Generator.PlayerBasePos[6] = new modMath.sXY_int(this.ValidateTextbox(this.txt7x, min, _dbl.X, 128.0), this.ValidateTextbox(this.txt7y, min, _dbl.Y, 128.0));
                                            if (this.Generator.TopLeftPlayerCount >= 8)
                                            {
                                                this.Generator.PlayerBasePos[7] = new modMath.sXY_int(this.ValidateTextbox(this.txt8x, min, _dbl.X, 128.0), this.ValidateTextbox(this.txt8y, min, _dbl.Y, 128.0));
                                                if (this.Generator.TopLeftPlayerCount >= 9)
                                                {
                                                    this.Generator.PlayerBasePos[8] = new modMath.sXY_int(this.ValidateTextbox(this.txt9x, min, _dbl.X, 128.0), this.ValidateTextbox(this.txt9y, min, _dbl.Y, 128.0));
                                                    if (this.Generator.TopLeftPlayerCount >= 10)
                                                    {
                                                        this.Generator.PlayerBasePos[9] = new modMath.sXY_int(this.ValidateTextbox(this.txt10x, min, _dbl.X, 128.0), this.ValidateTextbox(this.txt10y, min, _dbl.Y, 128.0));
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    this.Generator.LevelCount = this.ValidateTextbox(this.txtLevels, 3.0, 5.0, 1.0);
                    this.Generator.BaseLevel = this.ValidateTextbox(this.txtBaseLevel, -1.0, (double) (this.Generator.LevelCount - 1), 1.0);
                    this.Generator.JitterScale = 1;
                    this.Generator.MaxLevelTransition = 2;
                    this.Generator.PassagesChance = this.ValidateTextbox(this.txtLevelFrequency, 0.0, 100.0, 1.0);
                    this.Generator.VariationChance = this.ValidateTextbox(this.txtVariation, 0.0, 100.0, 1.0);
                    this.Generator.FlatsChance = this.ValidateTextbox(this.txtFlatness, 0.0, 100.0, 1.0);
                    this.Generator.BaseFlatArea = this.ValidateTextbox(this.txtBaseArea, 1.0, 16.0, 1.0);
                    this.Generator.NodeScale = 4f;
                    this.Generator.WaterSpawnQuantity = this.ValidateTextbox(this.txtWaterQuantity, 0.0, 9999.0, 1.0);
                    this.Generator.TotalWaterQuantity = this.ValidateTextbox(this.txtConnectedWater, 0.0, 9999.0, 1.0);
                    Application.DoEvents();
                    int num2 = 0;
                    while (true)
                    {
                        result = new clsResult("");
                        result = this.Generator.GenerateLayout();
                        if (!result.HasProblems)
                        {
                            clsResult resultToAdd = this.FinishHeights();
                            result.Add(resultToAdd);
                            if (!resultToAdd.HasProblems)
                            {
                                this.lstResult_AddResult(result);
                                this.lstResult_AddText("Done.");
                                this.btnGenerateLayout.Enabled = true;
                                break;
                            }
                        }
                        num2++;
                        this.lstResult_AddText("Attempt " + Conversions.ToString(num2) + " failed.");
                        Application.DoEvents();
                        if (this.StopTrying)
                        {
                            this.Generator.Map = null;
                            this.lstResult_AddResult(result);
                            this.lstResult_AddText("Stopped.");
                            this.btnGenerateLayout.Enabled = true;
                            return;
                        }
                        this.lstResult_AddResult(result);
                        this.lstResult_AddText("Retrying...");
                        Application.DoEvents();
                        this.Generator.ClearLayout();
                    }
                    this.lstResult_AddResult(result);
                }
            }
        }

        private void btnGenerateObjects_Click(object sender, EventArgs e)
        {
            if (!((this.Generator.Map == null) | (this.Generator.GenerateTileset == null)) && this.Generator.Map.frmMainLink.IsConnected)
            {
                this.Generator.BaseOilCount = this.ValidateTextbox(this.txtBaseOil, 0.0, 16.0, 1.0);
                this.Generator.ExtraOilCount = this.ValidateTextbox(this.txtOilElsewhere, 0.0, 9999.0, 1.0);
                this.Generator.ExtraOilClusterSizeMax = this.ValidateTextbox(this.txtOilClusterMax, 0.0, 99.0, 1.0);
                this.Generator.ExtraOilClusterSizeMin = this.ValidateTextbox(this.txtOilClusterMin, 0.0, (double) this.Generator.ExtraOilClusterSizeMax, 1.0);
                this.Generator.OilDispersion = ((float) this.ValidateTextbox(this.txtOilDispersion, 0.0, 9999.0, 1.0)) / 100f;
                this.Generator.OilAtATime = this.ValidateTextbox(this.txtOilAtATime, 1.0, 2.0, 1.0);
                this.Generator.FeatureClusterChance = ((float) this.ValidateTextbox(this.txtFClusterChance, 0.0, 100.0, 1.0)) / 100f;
                this.Generator.FeatureClusterMaxUnits = this.ValidateTextbox(this.txtFClusterMax, 0.0, 99.0, 1.0);
                this.Generator.FeatureClusterMinUnits = this.ValidateTextbox(this.txtFClusterMin, 0.0, (double) this.Generator.FeatureClusterMaxUnits, 1.0);
                this.Generator.FeatureScatterCount = this.ValidateTextbox(this.txtFScatterChance, 0.0, 99999.0, 1.0);
                this.Generator.FeatureScatterGap = this.ValidateTextbox(this.txtFScatterGap, 0.0, 99999.0, 1.0);
                this.Generator.BaseTruckCount = this.ValidateTextbox(this.txtTrucks, 0.0, 15.0, 1.0);
                this.Generator.GenerateTilePathMap();
                this.Generator.TerrainBlockPaths();
                this.Generator.BlockEdgeTiles();
                this.Generator.GenerateGateways();
                this.lstResult_AddText("Generating objects.");
                clsResult result = new clsResult("");
                result.Take(this.Generator.GenerateOil());
                result.Take(this.Generator.GenerateUnits());
                this.lstResult_AddResult(result);
                if (result.HasProblems)
                {
                    this.lstResult_AddText("Failed.");
                }
                else
                {
                    this.lstResult_AddText("Done.");
                }
                this.Generator.Map.SectorGraphicsChanges.SetAllChanged();
                this.Generator.Map.Update();
                this.Generator.Map.UndoStepCreate("Generator objects");
            }
        }

        private void btnGenerateRamps_Click(object sender, EventArgs e)
        {
            if (this.Generator.Map != null)
            {
                this.Generator.MaxDisconnectionDist = this.ValidateTextbox(this.txtRampDistance, 0.0, 99999.0, 128.0);
                this.Generator.RampBase = ((double) this.ValidateTextbox(this.txtRampBase, 10.0, 1000.0, 10.0)) / 1000.0;
                clsResult result = new clsResult("");
                this.lstResult_AddText("Generating ramps.");
                result = this.Generator.GenerateRamps();
                if (!result.HasProblems)
                {
                    result.Add(this.FinishHeights());
                }
                this.lstResult_AddResult(result);
                if (result.HasProblems)
                {
                    this.lstResult_AddText("Failed.");
                }
                else
                {
                    this.lstResult_AddText("Done.");
                }
            }
        }

        private void btnGenerateTextures_Click(object sender, EventArgs e)
        {
            if ((this.Generator.Map != null) && this.Generator.Map.frmMainLink.IsConnected)
            {
                this.lstResult_AddResult(this.FinishTextures());
                modMain.frmMainInstance.View_DrawViewLater();
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            this.StopTrying = true;
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

        private clsResult FinishHeights()
        {
            clsResult result2 = new clsResult("");
            result2.Take(this.Generator.GenerateLayoutTerrain());
            if (!result2.HasProblems)
            {
                this.Generator.Map.RandomizeHeights(this.Generator.LevelCount);
                this.Generator.Map.InterfaceOptions = new clsMap.clsInterfaceOptions();
                this.Generator.Map.InterfaceOptions.CompileMultiPlayers = modIO.InvariantToString_int(this.Generator.GetTotalPlayerCount);
                this._Owner.NewMainMap(this.Generator.Map);
            }
            return result2;
        }

        private clsResult FinishTextures()
        {
            int num;
            clsBrush.sPosNum num2;
            clsResult result2 = new clsResult("");
            if (!this.cbxMasterTexture.Checked)
            {
                switch (this.cboTileset.SelectedIndex)
                {
                    case 0:
                        this.Generator.Map.Tileset = modProgram.Tileset_Arizona;
                        this.Generator.GenerateTileset = modGenerator.Generator_TilesetArizona;
                        goto Label_023D;

                    case 1:
                        this.Generator.Map.Tileset = modProgram.Tileset_Urban;
                        this.Generator.GenerateTileset = modGenerator.Generator_TilesetUrban;
                        goto Label_023D;

                    case 2:
                        this.Generator.Map.Tileset = modProgram.Tileset_Rockies;
                        this.Generator.GenerateTileset = modGenerator.Generator_TilesetRockies;
                        goto Label_023D;
                }
                result2.ProblemAdd("Error: bad tileset selection.");
                this.btnGenerateLayout.Enabled = true;
                return result2;
            }
            switch (this.cboTileset.SelectedIndex)
            {
                case 0:
                    this.Generator.GenerateTileset = modGenerator.Generator_TilesetArizona;
                    modGenerator.TerrainStyle_Arizona.Watermap = this.Generator.GetWaterMap();
                    modGenerator.TerrainStyle_Arizona.LevelCount = this.Generator.LevelCount;
                    this.Generator.Map.GenerateMasterTerrain(ref modGenerator.TerrainStyle_Arizona);
                    modGenerator.TerrainStyle_Arizona.Watermap = null;
                    break;

                case 1:
                    this.Generator.GenerateTileset = modGenerator.Generator_TilesetUrban;
                    modGenerator.TerrainStyle_Urban.Watermap = this.Generator.GetWaterMap();
                    modGenerator.TerrainStyle_Urban.LevelCount = this.Generator.LevelCount;
                    this.Generator.Map.GenerateMasterTerrain(ref modGenerator.TerrainStyle_Urban);
                    modGenerator.TerrainStyle_Urban.Watermap = null;
                    break;

                case 2:
                    this.Generator.GenerateTileset = modGenerator.Generator_TilesetRockies;
                    modGenerator.TerrainStyle_Rockies.Watermap = this.Generator.GetWaterMap();
                    modGenerator.TerrainStyle_Rockies.LevelCount = this.Generator.LevelCount;
                    this.Generator.Map.GenerateMasterTerrain(ref modGenerator.TerrainStyle_Rockies);
                    modGenerator.TerrainStyle_Rockies.Watermap = null;
                    break;

                default:
                    result2.ProblemAdd("Error: bad tileset selection.");
                    this.btnGenerateLayout.Enabled = true;
                    return result2;
            }
            this.Generator.Map.TileType_Reset();
            this.Generator.Map.SetPainterToDefaults();
            goto Label_054A;
        Label_023D:
            this.Generator.Map.TileType_Reset();
            this.Generator.Map.SetPainterToDefaults();
            double num3 = Math.Atan((255.0 * this.Generator.Map.HeightMultiplier) / ((2.0 * (this.Generator.LevelCount - 1.0)) * 128.0)) - 0.017453292519943295;
            clsBrush brush = new clsBrush(Math.Max(this.Generator.Map.Terrain.TileSize.X, this.Generator.Map.Terrain.TileSize.Y) * 1.1, clsBrush.enumShape.Square);
            clsMap.clsApplyCliff tool = new clsMap.clsApplyCliff {
                Map = this.Generator.Map,
                Angle = num3,
                SetTris = true
            };
            num2.Normal = new modMath.sXY_int((int) Math.Round(Conversion.Int((double) (((double) this.Generator.Map.Terrain.TileSize.X) / 2.0))), (int) Math.Round(Conversion.Int((double) (((double) this.Generator.Map.Terrain.TileSize.Y) / 2.0))));
            num2.Alignment = num2.Normal;
            brush.PerformActionMapTiles(tool, num2);
            clsBooleanMap exterior = new clsBooleanMap();
            clsBooleanMap map = new clsBooleanMap();
            exterior = this.Generator.GetWaterMap();
            clsGeneratorTileset generateTileset = this.Generator.GenerateTileset;
            bool[] flagArray2 = new bool[(generateTileset.OldTextureLayers.LayerCount - 1) + 1];
            bool[] flagArray = new bool[(generateTileset.OldTextureLayers.LayerCount - 1) + 1];
            int num6 = generateTileset.OldTextureLayers.LayerCount - 1;
            for (num = 0; num <= num6; num++)
            {
                modProgram.sLayerList.clsLayer layer = generateTileset.OldTextureLayers.Layers[num];
                layer.Terrainmap = this.Generator.Map.GenerateTerrainMap(layer.Scale, layer.Density);
                if (layer.SlopeMax < 0f)
                {
                    layer.SlopeMax = (float) (num3 - 0.017453292519943295);
                    if (layer.HeightMax < 0f)
                    {
                        layer.HeightMax = 255f;
                        map.Within(layer.Terrainmap, exterior);
                        layer.Terrainmap.ValueData = map.ValueData;
                        map.ValueData = new clsBooleanMap.clsValueData();
                        flagArray[num] = true;
                    }
                    flagArray2[num] = true;
                }
                layer = null;
            }
            this.Generator.Map.MapTexturer(ref generateTileset.OldTextureLayers);
            int num7 = generateTileset.OldTextureLayers.LayerCount - 1;
            for (num = 0; num <= num7; num++)
            {
                modProgram.sLayerList.clsLayer layer2 = generateTileset.OldTextureLayers.Layers[num];
                layer2.Terrainmap = null;
                if (flagArray2[num])
                {
                    layer2.SlopeMax = -1f;
                }
                if (flagArray[num])
                {
                    layer2.HeightMax = -1f;
                }
                layer2 = null;
            }
            generateTileset = null;
        Label_054A:
            this.Generator.Map.LevelWater();
            this.Generator.Map.WaterTriCorrection();
            this.Generator.Map.SectorGraphicsChanges.SetAllChanged();
            this.Generator.Map.SectorUnitHeightsChanges.SetAllChanged();
            this.Generator.Map.Update();
            this.Generator.Map.UndoStepCreate("Generated Textures");
            if (this.Generator.Map == this._Owner.MainMap)
            {
                modMain.frmMainInstance.PainterTerrains_Refresh(-1, -1);
                modMain.frmMainInstance.MainMapTilesetChanged();
            }
            return result2;
        }

        private void frmGenerator_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }

        private void frmWZMapGen_Load(object sender, EventArgs e)
        {
            this.cboTileset.SelectedIndex = 0;
            this.cboSymmetry.SelectedIndex = 0;
        }

        [DebuggerStepThrough]
        private void InitializeComponent()
        {
            this.Label1 = new Label();
            this.txtWidth = new TextBox();
            this.txtHeight = new TextBox();
            this.Label2 = new Label();
            this.txt1x = new TextBox();
            this.Label3 = new Label();
            this.Label4 = new Label();
            this.Label5 = new Label();
            this.rdoPlayer2 = new RadioButton();
            this.txt1y = new TextBox();
            this.txt2y = new TextBox();
            this.txt2x = new TextBox();
            this.txt3y = new TextBox();
            this.txt3x = new TextBox();
            this.rdoPlayer3 = new RadioButton();
            this.txt4y = new TextBox();
            this.txt4x = new TextBox();
            this.rdoPlayer4 = new RadioButton();
            this.txt5y = new TextBox();
            this.txt5x = new TextBox();
            this.rdoPlayer5 = new RadioButton();
            this.txt6y = new TextBox();
            this.txt6x = new TextBox();
            this.rdoPlayer6 = new RadioButton();
            this.txt7y = new TextBox();
            this.txt7x = new TextBox();
            this.rdoPlayer7 = new RadioButton();
            this.txt8y = new TextBox();
            this.txt8x = new TextBox();
            this.rdoPlayer8 = new RadioButton();
            this.Label7 = new Label();
            this.txtLevels = new TextBox();
            this.txtLevelFrequency = new TextBox();
            this.Label8 = new Label();
            this.txtRampDistance = new TextBox();
            this.Label9 = new Label();
            this.txtBaseOil = new TextBox();
            this.Label10 = new Label();
            this.txtOilElsewhere = new TextBox();
            this.Label11 = new Label();
            this.txtOilClusterMin = new TextBox();
            this.Label12 = new Label();
            this.Label13 = new Label();
            this.Label14 = new Label();
            this.txtOilClusterMax = new TextBox();
            this.txtBaseLevel = new TextBox();
            this.Label17 = new Label();
            this.txtOilDispersion = new TextBox();
            this.Label18 = new Label();
            this.txtFScatterChance = new TextBox();
            this.Label19 = new Label();
            this.txtFClusterChance = new TextBox();
            this.Label20 = new Label();
            this.txtFClusterMin = new TextBox();
            this.Label21 = new Label();
            this.txtFClusterMax = new TextBox();
            this.Label22 = new Label();
            this.txtTrucks = new TextBox();
            this.Label23 = new Label();
            this.Label24 = new Label();
            this.cboTileset = new ComboBox();
            this.txtFlatness = new TextBox();
            this.Label25 = new Label();
            this.txtWaterQuantity = new TextBox();
            this.Label26 = new Label();
            this.Label27 = new Label();
            this.txtVariation = new TextBox();
            this.Label28 = new Label();
            this.rdoPlayer1 = new RadioButton();
            this.cboSymmetry = new ComboBox();
            this.Label6 = new Label();
            this.txtOilAtATime = new TextBox();
            this.Label31 = new Label();
            this.txtRampBase = new TextBox();
            this.Label33 = new Label();
            this.txtConnectedWater = new TextBox();
            this.Label34 = new Label();
            this.txt9y = new TextBox();
            this.txt9x = new TextBox();
            this.rdoPlayer9 = new RadioButton();
            this.txt10y = new TextBox();
            this.txt10x = new TextBox();
            this.rdoPlayer10 = new RadioButton();
            this.cbxMasterTexture = new CheckBox();
            this.txtBaseArea = new TextBox();
            this.Label15 = new Label();
            this.btnGenerateLayout = new Button();
            this.btnGenerateObjects = new Button();
            this.btnStop = new Button();
            this.lstResult = new ListBox();
            this.btnGenerateRamps = new Button();
            this.TabControl1 = new TabControl();
            this.TabPage1 = new TabPage();
            this.TabPage2 = new TabPage();
            this.TabPage4 = new TabPage();
            this.btnGenerateTextures = new Button();
            this.TabPage3 = new TabPage();
            this.Label16 = new Label();
            this.txtFScatterGap = new TextBox();
            this.TabControl1.SuspendLayout();
            this.TabPage1.SuspendLayout();
            this.TabPage2.SuspendLayout();
            this.TabPage4.SuspendLayout();
            this.TabPage3.SuspendLayout();
            this.SuspendLayout();
            this.Label1.AutoSize = true;
            Point point2 = new Point(0x24, 40);
            this.Label1.Location = point2;
            this.Label1.Name = "Label1";
            Size size2 = new Size(0x27, 20);
            this.Label1.Size = size2;
            this.Label1.TabIndex = 5;
            this.Label1.Text = "Width";
            this.Label1.UseCompatibleTextRendering = true;
            point2 = new Point(0x56, 0x25);
            this.txtWidth.Location = point2;
            this.txtWidth.Name = "txtWidth";
            size2 = new Size(0x2e, 0x16);
            this.txtWidth.Size = size2;
            this.txtWidth.TabIndex = 1;
            this.txtWidth.Text = "128";
            this.txtWidth.TextAlign = HorizontalAlignment.Right;
            point2 = new Point(0xbc, 0x25);
            this.txtHeight.Location = point2;
            this.txtHeight.Name = "txtHeight";
            size2 = new Size(0x2e, 0x16);
            this.txtHeight.Size = size2;
            this.txtHeight.TabIndex = 2;
            this.txtHeight.Text = "128";
            this.txtHeight.TextAlign = HorizontalAlignment.Right;
            this.Label2.AutoSize = true;
            point2 = new Point(0x8a, 40);
            this.Label2.Location = point2;
            this.Label2.Name = "Label2";
            size2 = new Size(0x2c, 20);
            this.Label2.Size = size2;
            this.Label2.TabIndex = 8;
            this.Label2.Text = "Height";
            this.Label2.UseCompatibleTextRendering = true;
            point2 = new Point(0x7b, 0x61);
            this.txt1x.Location = point2;
            this.txt1x.Name = "txt1x";
            size2 = new Size(0x2e, 0x16);
            this.txt1x.Size = size2;
            this.txt1x.TabIndex = 4;
            this.txt1x.Text = "0";
            this.txt1x.TextAlign = HorizontalAlignment.Right;
            this.Label3.AutoSize = true;
            point2 = new Point(13, 0x4d);
            this.Label3.Location = point2;
            this.Label3.Name = "Label3";
            size2 = new Size(0x5d, 20);
            this.Label3.Size = size2;
            this.Label3.TabIndex = 10;
            this.Label3.Text = "Base Positions";
            this.Label3.UseCompatibleTextRendering = true;
            this.Label4.AutoSize = true;
            point2 = new Point(120, 0x4c);
            this.Label4.Location = point2;
            this.Label4.Name = "Label4";
            size2 = new Size(12, 20);
            this.Label4.Size = size2;
            this.Label4.TabIndex = 12;
            this.Label4.Text = "x";
            this.Label4.UseCompatibleTextRendering = true;
            this.Label5.AutoSize = true;
            point2 = new Point(0xac, 0x4c);
            this.Label5.Location = point2;
            this.Label5.Name = "Label5";
            size2 = new Size(12, 20);
            this.Label5.Size = size2;
            this.Label5.TabIndex = 13;
            this.Label5.Text = "y";
            this.Label5.UseCompatibleTextRendering = true;
            this.rdoPlayer2.AutoSize = true;
            point2 = new Point(0x1c, 0x7e);
            this.rdoPlayer2.Location = point2;
            this.rdoPlayer2.Name = "rdoPlayer2";
            size2 = new Size(0x4b, 0x15);
            this.rdoPlayer2.Size = size2;
            this.rdoPlayer2.TabIndex = 0x36;
            this.rdoPlayer2.Text = "Player 2";
            this.rdoPlayer2.UseCompatibleTextRendering = true;
            this.rdoPlayer2.UseVisualStyleBackColor = true;
            point2 = new Point(0xaf, 0x60);
            this.txt1y.Location = point2;
            this.txt1y.Name = "txt1y";
            size2 = new Size(0x2e, 0x16);
            this.txt1y.Size = size2;
            this.txt1y.TabIndex = 5;
            this.txt1y.Text = "0";
            this.txt1y.TextAlign = HorizontalAlignment.Right;
            point2 = new Point(0xaf, 0x7c);
            this.txt2y.Location = point2;
            this.txt2y.Name = "txt2y";
            size2 = new Size(0x2e, 0x16);
            this.txt2y.Size = size2;
            this.txt2y.TabIndex = 8;
            this.txt2y.Text = "999";
            this.txt2y.TextAlign = HorizontalAlignment.Right;
            point2 = new Point(0x7b, 0x7d);
            this.txt2x.Location = point2;
            this.txt2x.Name = "txt2x";
            size2 = new Size(0x2e, 0x16);
            this.txt2x.Size = size2;
            this.txt2x.TabIndex = 7;
            this.txt2x.Text = "999";
            this.txt2x.TextAlign = HorizontalAlignment.Right;
            point2 = new Point(0xaf, 0x98);
            this.txt3y.Location = point2;
            this.txt3y.Name = "txt3y";
            size2 = new Size(0x2e, 0x16);
            this.txt3y.Size = size2;
            this.txt3y.TabIndex = 11;
            this.txt3y.Text = "0";
            this.txt3y.TextAlign = HorizontalAlignment.Right;
            point2 = new Point(0x7b, 0x99);
            this.txt3x.Location = point2;
            this.txt3x.Name = "txt3x";
            size2 = new Size(0x2e, 0x16);
            this.txt3x.Size = size2;
            this.txt3x.TabIndex = 10;
            this.txt3x.Text = "999";
            this.txt3x.TextAlign = HorizontalAlignment.Right;
            this.rdoPlayer3.AutoSize = true;
            point2 = new Point(0x1c, 0x9a);
            this.rdoPlayer3.Location = point2;
            this.rdoPlayer3.Name = "rdoPlayer3";
            size2 = new Size(0x4b, 0x15);
            this.rdoPlayer3.Size = size2;
            this.rdoPlayer3.TabIndex = 0x37;
            this.rdoPlayer3.Text = "Player 3";
            this.rdoPlayer3.UseCompatibleTextRendering = true;
            this.rdoPlayer3.UseVisualStyleBackColor = true;
            point2 = new Point(0xaf, 180);
            this.txt4y.Location = point2;
            this.txt4y.Name = "txt4y";
            size2 = new Size(0x2e, 0x16);
            this.txt4y.Size = size2;
            this.txt4y.TabIndex = 14;
            this.txt4y.Text = "999";
            this.txt4y.TextAlign = HorizontalAlignment.Right;
            point2 = new Point(0x7b, 0xb5);
            this.txt4x.Location = point2;
            this.txt4x.Name = "txt4x";
            size2 = new Size(0x2e, 0x16);
            this.txt4x.Size = size2;
            this.txt4x.TabIndex = 13;
            this.txt4x.Text = "0";
            this.txt4x.TextAlign = HorizontalAlignment.Right;
            this.rdoPlayer4.AutoSize = true;
            this.rdoPlayer4.Checked = true;
            point2 = new Point(0x1c, 0xb6);
            this.rdoPlayer4.Location = point2;
            this.rdoPlayer4.Name = "rdoPlayer4";
            size2 = new Size(0x4b, 0x15);
            this.rdoPlayer4.Size = size2;
            this.rdoPlayer4.TabIndex = 0x38;
            this.rdoPlayer4.TabStop = true;
            this.rdoPlayer4.Text = "Player 4";
            this.rdoPlayer4.UseCompatibleTextRendering = true;
            this.rdoPlayer4.UseVisualStyleBackColor = true;
            point2 = new Point(0xaf, 0xd0);
            this.txt5y.Location = point2;
            this.txt5y.Name = "txt5y";
            size2 = new Size(0x2e, 0x16);
            this.txt5y.Size = size2;
            this.txt5y.TabIndex = 0x11;
            this.txt5y.Text = "y";
            this.txt5y.TextAlign = HorizontalAlignment.Right;
            point2 = new Point(0x7b, 0xd1);
            this.txt5x.Location = point2;
            this.txt5x.Name = "txt5x";
            size2 = new Size(0x2e, 0x16);
            this.txt5x.Size = size2;
            this.txt5x.TabIndex = 0x10;
            this.txt5x.Text = "x";
            this.txt5x.TextAlign = HorizontalAlignment.Right;
            this.rdoPlayer5.AutoSize = true;
            point2 = new Point(0x1c, 210);
            this.rdoPlayer5.Location = point2;
            this.rdoPlayer5.Name = "rdoPlayer5";
            size2 = new Size(0x4b, 0x15);
            this.rdoPlayer5.Size = size2;
            this.rdoPlayer5.TabIndex = 0x39;
            this.rdoPlayer5.Text = "Player 5";
            this.rdoPlayer5.UseCompatibleTextRendering = true;
            this.rdoPlayer5.UseVisualStyleBackColor = true;
            point2 = new Point(0xaf, 0xec);
            this.txt6y.Location = point2;
            this.txt6y.Name = "txt6y";
            size2 = new Size(0x2e, 0x16);
            this.txt6y.Size = size2;
            this.txt6y.TabIndex = 20;
            this.txt6y.Text = "y";
            this.txt6y.TextAlign = HorizontalAlignment.Right;
            point2 = new Point(0x7b, 0xed);
            this.txt6x.Location = point2;
            this.txt6x.Name = "txt6x";
            size2 = new Size(0x2e, 0x16);
            this.txt6x.Size = size2;
            this.txt6x.TabIndex = 0x13;
            this.txt6x.Text = "x";
            this.txt6x.TextAlign = HorizontalAlignment.Right;
            this.rdoPlayer6.AutoSize = true;
            point2 = new Point(0x1c, 0xee);
            this.rdoPlayer6.Location = point2;
            this.rdoPlayer6.Name = "rdoPlayer6";
            size2 = new Size(0x4b, 0x15);
            this.rdoPlayer6.Size = size2;
            this.rdoPlayer6.TabIndex = 0x3a;
            this.rdoPlayer6.Text = "Player 6";
            this.rdoPlayer6.UseCompatibleTextRendering = true;
            this.rdoPlayer6.UseVisualStyleBackColor = true;
            point2 = new Point(0xaf, 0x108);
            this.txt7y.Location = point2;
            this.txt7y.Name = "txt7y";
            size2 = new Size(0x2e, 0x16);
            this.txt7y.Size = size2;
            this.txt7y.TabIndex = 0x17;
            this.txt7y.Text = "y";
            this.txt7y.TextAlign = HorizontalAlignment.Right;
            point2 = new Point(0x7b, 0x109);
            this.txt7x.Location = point2;
            this.txt7x.Name = "txt7x";
            size2 = new Size(0x2e, 0x16);
            this.txt7x.Size = size2;
            this.txt7x.TabIndex = 0x16;
            this.txt7x.Text = "x";
            this.txt7x.TextAlign = HorizontalAlignment.Right;
            this.rdoPlayer7.AutoSize = true;
            point2 = new Point(0x1c, 0x10a);
            this.rdoPlayer7.Location = point2;
            this.rdoPlayer7.Name = "rdoPlayer7";
            size2 = new Size(0x4b, 0x15);
            this.rdoPlayer7.Size = size2;
            this.rdoPlayer7.TabIndex = 0x3b;
            this.rdoPlayer7.Text = "Player 7";
            this.rdoPlayer7.UseCompatibleTextRendering = true;
            this.rdoPlayer7.UseVisualStyleBackColor = true;
            point2 = new Point(0xaf, 0x124);
            this.txt8y.Location = point2;
            this.txt8y.Name = "txt8y";
            size2 = new Size(0x2e, 0x16);
            this.txt8y.Size = size2;
            this.txt8y.TabIndex = 0x1a;
            this.txt8y.Text = "y";
            this.txt8y.TextAlign = HorizontalAlignment.Right;
            point2 = new Point(0x7b, 0x125);
            this.txt8x.Location = point2;
            this.txt8x.Name = "txt8x";
            size2 = new Size(0x2e, 0x16);
            this.txt8x.Size = size2;
            this.txt8x.TabIndex = 0x19;
            this.txt8x.Text = "x";
            this.txt8x.TextAlign = HorizontalAlignment.Right;
            this.rdoPlayer8.AutoSize = true;
            point2 = new Point(0x1c, 0x126);
            this.rdoPlayer8.Location = point2;
            this.rdoPlayer8.Name = "rdoPlayer8";
            size2 = new Size(0x4b, 0x15);
            this.rdoPlayer8.Size = size2;
            this.rdoPlayer8.TabIndex = 60;
            this.rdoPlayer8.Text = "Player 8";
            this.rdoPlayer8.UseCompatibleTextRendering = true;
            this.rdoPlayer8.UseVisualStyleBackColor = true;
            point2 = new Point(0xfd, 0x17);
            this.Label7.Location = point2;
            this.Label7.Name = "Label7";
            size2 = new Size(0x73, 20);
            this.Label7.Size = size2;
            this.Label7.TabIndex = 0x25;
            this.Label7.Text = "Height Levels";
            this.Label7.TextAlign = ContentAlignment.MiddleRight;
            this.Label7.UseCompatibleTextRendering = true;
            point2 = new Point(0x176, 0x18);
            this.txtLevels.Location = point2;
            this.txtLevels.Name = "txtLevels";
            size2 = new Size(0x2e, 0x16);
            this.txtLevels.Size = size2;
            this.txtLevels.TabIndex = 0x1c;
            this.txtLevels.Text = "4";
            this.txtLevels.TextAlign = HorizontalAlignment.Right;
            point2 = new Point(0x176, 0xa8);
            this.txtLevelFrequency.Location = point2;
            this.txtLevelFrequency.Name = "txtLevelFrequency";
            size2 = new Size(0x2e, 0x16);
            this.txtLevelFrequency.Size = size2;
            this.txtLevelFrequency.TabIndex = 0x1f;
            this.txtLevelFrequency.Text = "1";
            this.txtLevelFrequency.TextAlign = HorizontalAlignment.Right;
            point2 = new Point(0x11a, 0xa7);
            this.Label8.Location = point2;
            this.Label8.Name = "Label8";
            size2 = new Size(0x56, 20);
            this.Label8.Size = size2;
            this.Label8.TabIndex = 0x27;
            this.Label8.Text = "Passages";
            this.Label8.TextAlign = ContentAlignment.MiddleRight;
            this.Label8.UseCompatibleTextRendering = true;
            point2 = new Point(0x81, 0x29);
            this.txtRampDistance.Location = point2;
            this.txtRampDistance.Name = "txtRampDistance";
            size2 = new Size(0x2e, 0x16);
            this.txtRampDistance.Size = size2;
            this.txtRampDistance.TabIndex = 0x24;
            this.txtRampDistance.Text = "80";
            this.txtRampDistance.TextAlign = HorizontalAlignment.Right;
            point2 = new Point(0, 0x12);
            this.Label9.Location = point2;
            this.Label9.Name = "Label9";
            size2 = new Size(0xb0, 20);
            this.Label9.Size = size2;
            this.Label9.TabIndex = 0x29;
            this.Label9.Text = "Ramp Distance At Base";
            this.Label9.TextAlign = ContentAlignment.MiddleRight;
            this.Label9.UseCompatibleTextRendering = true;
            point2 = new Point(0xba, 0x19);
            this.txtBaseOil.Location = point2;
            this.txtBaseOil.Name = "txtBaseOil";
            size2 = new Size(0x2e, 0x16);
            this.txtBaseOil.Size = size2;
            this.txtBaseOil.TabIndex = 0x26;
            this.txtBaseOil.Text = "4";
            this.txtBaseOil.TextAlign = HorizontalAlignment.Right;
            point2 = new Point(0x4a, 0x18);
            this.Label10.Location = point2;
            this.Label10.Name = "Label10";
            size2 = new Size(0x6a, 20);
            this.Label10.Size = size2;
            this.Label10.TabIndex = 0x2b;
            this.Label10.Text = "Oil In Base";
            this.Label10.TextAlign = ContentAlignment.MiddleRight;
            this.Label10.UseCompatibleTextRendering = true;
            point2 = new Point(0xba, 0x35);
            this.txtOilElsewhere.Location = point2;
            this.txtOilElsewhere.Name = "txtOilElsewhere";
            size2 = new Size(0x2e, 0x16);
            this.txtOilElsewhere.Size = size2;
            this.txtOilElsewhere.TabIndex = 0x27;
            this.txtOilElsewhere.Text = "40";
            this.txtOilElsewhere.TextAlign = HorizontalAlignment.Right;
            point2 = new Point(0x3d, 0x34);
            this.Label11.Location = point2;
            this.Label11.Name = "Label11";
            size2 = new Size(0x77, 20);
            this.Label11.Size = size2;
            this.Label11.TabIndex = 0x2d;
            this.Label11.Text = "Oil Elsewhere";
            this.Label11.TextAlign = ContentAlignment.MiddleRight;
            this.Label11.UseCompatibleTextRendering = true;
            point2 = new Point(0xba, 0x51);
            this.txtOilClusterMin.Location = point2;
            this.txtOilClusterMin.Name = "txtOilClusterMin";
            size2 = new Size(0x16, 0x16);
            this.txtOilClusterMin.Size = size2;
            this.txtOilClusterMin.TabIndex = 40;
            this.txtOilClusterMin.Text = "1";
            this.txtOilClusterMin.TextAlign = HorizontalAlignment.Right;
            point2 = new Point(0x12, 80);
            this.Label12.Location = point2;
            this.Label12.Name = "Label12";
            size2 = new Size(0x72, 20);
            this.Label12.Size = size2;
            this.Label12.TabIndex = 0x2f;
            this.Label12.Text = "Oil Cluster Size";
            this.Label12.TextAlign = ContentAlignment.MiddleRight;
            this.Label12.UseCompatibleTextRendering = true;
            this.Label13.AutoSize = true;
            point2 = new Point(150, 0x54);
            this.Label13.Location = point2;
            this.Label13.Name = "Label13";
            size2 = new Size(0x1a, 20);
            this.Label13.Size = size2;
            this.Label13.TabIndex = 0x31;
            this.Label13.Text = "Min";
            this.Label13.UseCompatibleTextRendering = true;
            this.Label14.AutoSize = true;
            point2 = new Point(0xdf, 0x54);
            this.Label14.Location = point2;
            this.Label14.Name = "Label14";
            size2 = new Size(0x21, 0x11);
            this.Label14.Size = size2;
            this.Label14.TabIndex = 0x33;
            this.Label14.Text = "Max";
            point2 = new Point(0x103, 0x51);
            this.txtOilClusterMax.Location = point2;
            this.txtOilClusterMax.Name = "txtOilClusterMax";
            size2 = new Size(0x16, 0x16);
            this.txtOilClusterMax.Size = size2;
            this.txtOilClusterMax.TabIndex = 0x29;
            this.txtOilClusterMax.Text = "1";
            this.txtOilClusterMax.TextAlign = HorizontalAlignment.Right;
            point2 = new Point(0x176, 50);
            this.txtBaseLevel.Location = point2;
            this.txtBaseLevel.Name = "txtBaseLevel";
            size2 = new Size(0x2e, 0x16);
            this.txtBaseLevel.Size = size2;
            this.txtBaseLevel.TabIndex = 0x1d;
            this.txtBaseLevel.Text = "-1";
            this.txtBaseLevel.TextAlign = HorizontalAlignment.Right;
            point2 = new Point(0xed, 0x31);
            this.Label17.Location = point2;
            this.Label17.Name = "Label17";
            size2 = new Size(0x83, 20);
            this.Label17.Size = size2;
            this.Label17.TabIndex = 0x3a;
            this.Label17.Text = "Base Height Level";
            this.Label17.TextAlign = ContentAlignment.MiddleRight;
            this.Label17.UseCompatibleTextRendering = true;
            point2 = new Point(0xba, 0x6d);
            this.txtOilDispersion.Location = point2;
            this.txtOilDispersion.Name = "txtOilDispersion";
            size2 = new Size(0x2e, 0x16);
            this.txtOilDispersion.Size = size2;
            this.txtOilDispersion.TabIndex = 0x2a;
            this.txtOilDispersion.Text = "100";
            this.txtOilDispersion.TextAlign = HorizontalAlignment.Right;
            point2 = new Point(0x2c, 0x6c);
            this.Label18.Location = point2;
            this.Label18.Name = "Label18";
            size2 = new Size(0x88, 20);
            this.Label18.Size = size2;
            this.Label18.TabIndex = 60;
            this.Label18.Text = "Oil Dispersion";
            this.Label18.TextAlign = ContentAlignment.MiddleRight;
            this.Label18.UseCompatibleTextRendering = true;
            point2 = new Point(0xba, 0xb9);
            this.txtFScatterChance.Location = point2;
            this.txtFScatterChance.Name = "txtFScatterChance";
            size2 = new Size(0x2e, 0x16);
            this.txtFScatterChance.Size = size2;
            this.txtFScatterChance.TabIndex = 0x2b;
            this.txtFScatterChance.Text = "9999";
            this.txtFScatterChance.TextAlign = HorizontalAlignment.Right;
            point2 = new Point(0x12, 0xb8);
            this.Label19.Location = point2;
            this.Label19.Name = "Label19";
            size2 = new Size(0xa2, 20);
            this.Label19.Size = size2;
            this.Label19.TabIndex = 0x3e;
            this.Label19.Text = "Scattered Features";
            this.Label19.TextAlign = ContentAlignment.MiddleRight;
            this.Label19.UseCompatibleTextRendering = true;
            point2 = new Point(0xba, 240);
            this.txtFClusterChance.Location = point2;
            this.txtFClusterChance.Name = "txtFClusterChance";
            size2 = new Size(0x2e, 0x16);
            this.txtFClusterChance.Size = size2;
            this.txtFClusterChance.TabIndex = 0x2c;
            this.txtFClusterChance.Text = "0";
            this.txtFClusterChance.TextAlign = HorizontalAlignment.Right;
            point2 = new Point(-4, 0xef);
            this.Label20.Location = point2;
            this.Label20.Name = "Label20";
            size2 = new Size(0xb8, 20);
            this.Label20.Size = size2;
            this.Label20.TabIndex = 0x40;
            this.Label20.Text = "Feature Cluster Chance %";
            this.Label20.TextAlign = ContentAlignment.MiddleRight;
            this.Label20.UseCompatibleTextRendering = true;
            point2 = new Point(0xba, 0x124);
            this.txtFClusterMin.Location = point2;
            this.txtFClusterMin.Name = "txtFClusterMin";
            size2 = new Size(0x2e, 0x16);
            this.txtFClusterMin.Size = size2;
            this.txtFClusterMin.TabIndex = 0x2d;
            this.txtFClusterMin.Text = "2";
            this.txtFClusterMin.TextAlign = HorizontalAlignment.Right;
            point2 = new Point(0x2c, 0x10d);
            this.Label21.Location = point2;
            this.Label21.Name = "Label21";
            size2 = new Size(0xa4, 20);
            this.Label21.Size = size2;
            this.Label21.TabIndex = 0x42;
            this.Label21.Text = "Feature Cluster Size";
            this.Label21.TextAlign = ContentAlignment.MiddleRight;
            this.Label21.UseCompatibleTextRendering = true;
            point2 = new Point(0xba, 320);
            this.txtFClusterMax.Location = point2;
            this.txtFClusterMax.Name = "txtFClusterMax";
            size2 = new Size(0x2e, 0x16);
            this.txtFClusterMax.Size = size2;
            this.txtFClusterMax.TabIndex = 0x2e;
            this.txtFClusterMax.Text = "5";
            this.txtFClusterMax.TextAlign = HorizontalAlignment.Right;
            point2 = new Point(130, 0x123);
            this.Label22.Location = point2;
            this.Label22.Name = "Label22";
            size2 = new Size(50, 20);
            this.Label22.Size = size2;
            this.Label22.TabIndex = 0x44;
            this.Label22.Text = "Min";
            this.Label22.TextAlign = ContentAlignment.MiddleRight;
            this.Label22.UseCompatibleTextRendering = true;
            point2 = new Point(0x18e, 0x19);
            this.txtTrucks.Location = point2;
            this.txtTrucks.Name = "txtTrucks";
            size2 = new Size(0x2e, 0x16);
            this.txtTrucks.Size = size2;
            this.txtTrucks.TabIndex = 0x2f;
            this.txtTrucks.Text = "2";
            this.txtTrucks.TextAlign = HorizontalAlignment.Right;
            point2 = new Point(0x11c, 0x18);
            this.Label23.Location = point2;
            this.Label23.Name = "Label23";
            size2 = new Size(0x6c, 20);
            this.Label23.Size = size2;
            this.Label23.TabIndex = 70;
            this.Label23.Text = "Base Trucks";
            this.Label23.TextAlign = ContentAlignment.MiddleRight;
            this.Label23.UseCompatibleTextRendering = true;
            this.Label24.AutoSize = true;
            point2 = new Point(0x18, 0x16);
            this.Label24.Location = point2;
            this.Label24.Name = "Label24";
            size2 = new Size(0x2c, 20);
            this.Label24.Size = size2;
            this.Label24.TabIndex = 0x48;
            this.Label24.Text = "Tileset";
            this.Label24.UseCompatibleTextRendering = true;
            this.cboTileset.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cboTileset.FormattingEnabled = true;
            this.cboTileset.Items.AddRange(new object[] { "Arizona", "Urban", "Rockies" });
            point2 = new Point(80, 0x13);
            this.cboTileset.Location = point2;
            this.cboTileset.Name = "cboTileset";
            size2 = new Size(0x95, 0x18);
            this.cboTileset.Size = size2;
            this.cboTileset.TabIndex = 0x1b;
            point2 = new Point(0x176, 140);
            this.txtFlatness.Location = point2;
            this.txtFlatness.Name = "txtFlatness";
            size2 = new Size(0x2e, 0x16);
            this.txtFlatness.Size = size2;
            this.txtFlatness.TabIndex = 30;
            this.txtFlatness.Text = "0";
            this.txtFlatness.TextAlign = HorizontalAlignment.Right;
            point2 = new Point(0x138, 0x8b);
            this.Label25.Location = point2;
            this.Label25.Name = "Label25";
            size2 = new Size(0x38, 20);
            this.Label25.Size = size2;
            this.Label25.TabIndex = 0x4a;
            this.Label25.Text = "Flats";
            this.Label25.TextAlign = ContentAlignment.MiddleRight;
            this.Label25.UseCompatibleTextRendering = true;
            point2 = new Point(0x176, 0xee);
            this.txtWaterQuantity.Location = point2;
            this.txtWaterQuantity.Name = "txtWaterQuantity";
            size2 = new Size(0x2e, 0x16);
            this.txtWaterQuantity.Size = size2;
            this.txtWaterQuantity.TabIndex = 0x23;
            this.txtWaterQuantity.Text = "0";
            this.txtWaterQuantity.TextAlign = HorizontalAlignment.Right;
            point2 = new Point(0xed, 0xed);
            this.Label26.Location = point2;
            this.Label26.Name = "Label26";
            size2 = new Size(0x83, 20);
            this.Label26.Size = size2;
            this.Label26.TabIndex = 0x4e;
            this.Label26.Text = "Water Spawns";
            this.Label26.TextAlign = ContentAlignment.MiddleRight;
            this.Label26.UseCompatibleTextRendering = true;
            point2 = new Point(0x79, 0x13f);
            this.Label27.Location = point2;
            this.Label27.Name = "Label27";
            size2 = new Size(0x3b, 20);
            this.Label27.Size = size2;
            this.Label27.TabIndex = 80;
            this.Label27.Text = "Max";
            this.Label27.TextAlign = ContentAlignment.MiddleRight;
            this.Label27.UseCompatibleTextRendering = true;
            point2 = new Point(0x176, 0xc4);
            this.txtVariation.Location = point2;
            this.txtVariation.Name = "txtVariation";
            size2 = new Size(0x2e, 0x16);
            this.txtVariation.Size = size2;
            this.txtVariation.TabIndex = 0x20;
            this.txtVariation.Text = "1";
            this.txtVariation.TextAlign = HorizontalAlignment.Right;
            point2 = new Point(0x114, 0xc4);
            this.Label28.Location = point2;
            this.Label28.Name = "Label28";
            size2 = new Size(0x5c, 20);
            this.Label28.Size = size2;
            this.Label28.TabIndex = 0x51;
            this.Label28.Text = "Variation";
            this.Label28.TextAlign = ContentAlignment.MiddleRight;
            this.Label28.UseCompatibleTextRendering = true;
            this.rdoPlayer1.AutoSize = true;
            point2 = new Point(0x1c, 100);
            this.rdoPlayer1.Location = point2;
            this.rdoPlayer1.Name = "rdoPlayer1";
            size2 = new Size(0x4b, 0x15);
            this.rdoPlayer1.Size = size2;
            this.rdoPlayer1.TabIndex = 0x35;
            this.rdoPlayer1.Text = "Player 1";
            this.rdoPlayer1.UseCompatibleTextRendering = true;
            this.rdoPlayer1.UseVisualStyleBackColor = true;
            this.cboSymmetry.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cboSymmetry.FormattingEnabled = true;
            this.cboSymmetry.Items.AddRange(new object[] { "None", "Horizontal Rotation", "Vertical Rotation", "Horizontal Flip", "Vertical Flip", "Quarters Rotation", "Quarters Flip" });
            point2 = new Point(0x55, 7);
            this.cboSymmetry.Location = point2;
            this.cboSymmetry.Name = "cboSymmetry";
            size2 = new Size(0x95, 0x18);
            this.cboSymmetry.Size = size2;
            this.cboSymmetry.TabIndex = 0;
            this.Label6.AutoSize = true;
            point2 = new Point(9, 10);
            this.Label6.Location = point2;
            this.Label6.Name = "Label6";
            size2 = new Size(0x41, 20);
            this.Label6.Size = size2;
            this.Label6.TabIndex = 0x58;
            this.Label6.Text = "Symmetry";
            this.Label6.UseCompatibleTextRendering = true;
            point2 = new Point(0xba, 0x89);
            this.txtOilAtATime.Location = point2;
            this.txtOilAtATime.Name = "txtOilAtATime";
            size2 = new Size(0x2e, 0x16);
            this.txtOilAtATime.Size = size2;
            this.txtOilAtATime.TabIndex = 0x59;
            this.txtOilAtATime.Text = "1";
            this.txtOilAtATime.TextAlign = HorizontalAlignment.Right;
            point2 = new Point(0x12, 0x88);
            this.Label31.Location = point2;
            this.Label31.Name = "Label31";
            size2 = new Size(0xa2, 20);
            this.Label31.Size = size2;
            this.Label31.TabIndex = 90;
            this.Label31.Text = "Oil Clusters At A Time";
            this.Label31.TextAlign = ContentAlignment.MiddleRight;
            this.Label31.UseCompatibleTextRendering = true;
            point2 = new Point(0x81, 0x60);
            this.txtRampBase.Location = point2;
            this.txtRampBase.Name = "txtRampBase";
            size2 = new Size(0x2e, 0x16);
            this.txtRampBase.Size = size2;
            this.txtRampBase.TabIndex = 0x5d;
            this.txtRampBase.Text = "100";
            this.txtRampBase.TextAlign = HorizontalAlignment.Right;
            point2 = new Point(-4, 0x49);
            this.Label33.Location = point2;
            this.Label33.Name = "Label33";
            size2 = new Size(0xb3, 20);
            this.Label33.Size = size2;
            this.Label33.TabIndex = 0x5e;
            this.Label33.Text = "Ramp Multiplier % Per 8";
            this.Label33.TextAlign = ContentAlignment.MiddleRight;
            this.Label33.UseCompatibleTextRendering = true;
            point2 = new Point(0x176, 0x10a);
            this.txtConnectedWater.Location = point2;
            this.txtConnectedWater.Name = "txtConnectedWater";
            size2 = new Size(0x2e, 0x16);
            this.txtConnectedWater.Size = size2;
            this.txtConnectedWater.TabIndex = 0x5f;
            this.txtConnectedWater.Text = "0";
            this.txtConnectedWater.TextAlign = HorizontalAlignment.Right;
            point2 = new Point(0xed, 0x109);
            this.Label34.Location = point2;
            this.Label34.Name = "Label34";
            size2 = new Size(0x83, 20);
            this.Label34.Size = size2;
            this.Label34.TabIndex = 0x60;
            this.Label34.Text = "Total Water";
            this.Label34.TextAlign = ContentAlignment.MiddleRight;
            this.Label34.UseCompatibleTextRendering = true;
            point2 = new Point(0xaf, 320);
            this.txt9y.Location = point2;
            this.txt9y.Name = "txt9y";
            size2 = new Size(0x2e, 0x16);
            this.txt9y.Size = size2;
            this.txt9y.TabIndex = 0x62;
            this.txt9y.Text = "y";
            this.txt9y.TextAlign = HorizontalAlignment.Right;
            point2 = new Point(0x7b, 0x141);
            this.txt9x.Location = point2;
            this.txt9x.Name = "txt9x";
            size2 = new Size(0x2e, 0x16);
            this.txt9x.Size = size2;
            this.txt9x.TabIndex = 0x61;
            this.txt9x.Text = "x";
            this.txt9x.TextAlign = HorizontalAlignment.Right;
            this.rdoPlayer9.AutoSize = true;
            point2 = new Point(0x1c, 0x142);
            this.rdoPlayer9.Location = point2;
            this.rdoPlayer9.Name = "rdoPlayer9";
            size2 = new Size(0x4b, 0x15);
            this.rdoPlayer9.Size = size2;
            this.rdoPlayer9.TabIndex = 0x63;
            this.rdoPlayer9.Text = "Player 9";
            this.rdoPlayer9.UseCompatibleTextRendering = true;
            this.rdoPlayer9.UseVisualStyleBackColor = true;
            point2 = new Point(0xaf, 0x15c);
            this.txt10y.Location = point2;
            this.txt10y.Name = "txt10y";
            size2 = new Size(0x2e, 0x16);
            this.txt10y.Size = size2;
            this.txt10y.TabIndex = 0x65;
            this.txt10y.Text = "y";
            this.txt10y.TextAlign = HorizontalAlignment.Right;
            point2 = new Point(0x7b, 0x15d);
            this.txt10x.Location = point2;
            this.txt10x.Name = "txt10x";
            size2 = new Size(0x2e, 0x16);
            this.txt10x.Size = size2;
            this.txt10x.TabIndex = 100;
            this.txt10x.Text = "x";
            this.txt10x.TextAlign = HorizontalAlignment.Right;
            this.rdoPlayer10.AutoSize = true;
            point2 = new Point(0x1c, 350);
            this.rdoPlayer10.Location = point2;
            this.rdoPlayer10.Name = "rdoPlayer10";
            size2 = new Size(0x52, 0x15);
            this.rdoPlayer10.Size = size2;
            this.rdoPlayer10.TabIndex = 0x66;
            this.rdoPlayer10.Text = "Player 10";
            this.rdoPlayer10.UseCompatibleTextRendering = true;
            this.rdoPlayer10.UseVisualStyleBackColor = true;
            this.cbxMasterTexture.AutoSize = true;
            point2 = new Point(0x2d, 0x40);
            this.cbxMasterTexture.Location = point2;
            this.cbxMasterTexture.Name = "cbxMasterTexture";
            size2 = new Size(0x7f, 0x15);
            this.cbxMasterTexture.Size = size2;
            this.cbxMasterTexture.TabIndex = 0x67;
            this.cbxMasterTexture.Text = "Master Texturing";
            this.cbxMasterTexture.UseCompatibleTextRendering = true;
            this.cbxMasterTexture.UseVisualStyleBackColor = true;
            point2 = new Point(0x176, 0x61);
            this.txtBaseArea.Location = point2;
            this.txtBaseArea.Name = "txtBaseArea";
            size2 = new Size(0x2e, 0x16);
            this.txtBaseArea.Size = size2;
            this.txtBaseArea.TabIndex = 0x68;
            this.txtBaseArea.Text = "3";
            this.txtBaseArea.TextAlign = HorizontalAlignment.Right;
            point2 = new Point(0xf9, 0x61);
            this.Label15.Location = point2;
            this.Label15.Name = "Label15";
            size2 = new Size(0x77, 20);
            this.Label15.Size = size2;
            this.Label15.TabIndex = 0x69;
            this.Label15.Text = "Base Flat Area";
            this.Label15.TextAlign = ContentAlignment.MiddleRight;
            this.Label15.UseCompatibleTextRendering = true;
            point2 = new Point(0xfd, 0x152);
            this.btnGenerateLayout.Location = point2;
            this.btnGenerateLayout.Name = "btnGenerateLayout";
            size2 = new Size(0x92, 0x21);
            this.btnGenerateLayout.Size = size2;
            this.btnGenerateLayout.TabIndex = 0x6a;
            this.btnGenerateLayout.Text = "Generate Layout";
            this.btnGenerateLayout.UseCompatibleTextRendering = true;
            this.btnGenerateLayout.UseVisualStyleBackColor = true;
            point2 = new Point(0x10f, 0x15c);
            this.btnGenerateObjects.Location = point2;
            this.btnGenerateObjects.Name = "btnGenerateObjects";
            size2 = new Size(0x92, 0x21);
            this.btnGenerateObjects.Size = size2;
            this.btnGenerateObjects.TabIndex = 0x6b;
            this.btnGenerateObjects.Text = "Generate Objects";
            this.btnGenerateObjects.UseCompatibleTextRendering = true;
            this.btnGenerateObjects.UseVisualStyleBackColor = true;
            point2 = new Point(0x196, 0x152);
            this.btnStop.Location = point2;
            Padding padding2 = new Padding(4);
            this.btnStop.Margin = padding2;
            this.btnStop.Name = "btnStop";
            size2 = new Size(0x3a, 0x21);
            this.btnStop.Size = size2;
            this.btnStop.TabIndex = 0x33;
            this.btnStop.Text = "Stop";
            this.btnStop.UseCompatibleTextRendering = true;
            this.btnStop.UseVisualStyleBackColor = true;
            this.lstResult.FormattingEnabled = true;
            this.lstResult.ItemHeight = 0x10;
            point2 = new Point(12, 0x1ba);
            this.lstResult.Location = point2;
            this.lstResult.Name = "lstResult";
            size2 = new Size(0x1f5, 0x74);
            this.lstResult.Size = size2;
            this.lstResult.TabIndex = 0x6c;
            point2 = new Point(270, 0x157);
            this.btnGenerateRamps.Location = point2;
            this.btnGenerateRamps.Name = "btnGenerateRamps";
            size2 = new Size(0x92, 0x21);
            this.btnGenerateRamps.Size = size2;
            this.btnGenerateRamps.TabIndex = 0x6d;
            this.btnGenerateRamps.Text = "Generate Ramps";
            this.btnGenerateRamps.UseCompatibleTextRendering = true;
            this.btnGenerateRamps.UseVisualStyleBackColor = true;
            this.TabControl1.Controls.Add(this.TabPage1);
            this.TabControl1.Controls.Add(this.TabPage2);
            this.TabControl1.Controls.Add(this.TabPage4);
            this.TabControl1.Controls.Add(this.TabPage3);
            point2 = new Point(12, 12);
            this.TabControl1.Location = point2;
            this.TabControl1.Name = "TabControl1";
            this.TabControl1.SelectedIndex = 0;
            size2 = new Size(0x1f5, 0x1a8);
            this.TabControl1.Size = size2;
            this.TabControl1.TabIndex = 110;
            this.TabPage1.Controls.Add(this.Label6);
            this.TabPage1.Controls.Add(this.Label1);
            this.TabPage1.Controls.Add(this.txtWidth);
            this.TabPage1.Controls.Add(this.Label2);
            this.TabPage1.Controls.Add(this.btnGenerateLayout);
            this.TabPage1.Controls.Add(this.btnStop);
            this.TabPage1.Controls.Add(this.txtHeight);
            this.TabPage1.Controls.Add(this.txtBaseArea);
            this.TabPage1.Controls.Add(this.txtConnectedWater);
            this.TabPage1.Controls.Add(this.Label3);
            this.TabPage1.Controls.Add(this.Label34);
            this.TabPage1.Controls.Add(this.Label15);
            this.TabPage1.Controls.Add(this.txt1x);
            this.TabPage1.Controls.Add(this.Label4);
            this.TabPage1.Controls.Add(this.txt10y);
            this.TabPage1.Controls.Add(this.txtWaterQuantity);
            this.TabPage1.Controls.Add(this.Label5);
            this.TabPage1.Controls.Add(this.Label26);
            this.TabPage1.Controls.Add(this.txt10x);
            this.TabPage1.Controls.Add(this.rdoPlayer2);
            this.TabPage1.Controls.Add(this.rdoPlayer10);
            this.TabPage1.Controls.Add(this.txt1y);
            this.TabPage1.Controls.Add(this.txtVariation);
            this.TabPage1.Controls.Add(this.txt9y);
            this.TabPage1.Controls.Add(this.Label28);
            this.TabPage1.Controls.Add(this.txt2x);
            this.TabPage1.Controls.Add(this.txt9x);
            this.TabPage1.Controls.Add(this.txt2y);
            this.TabPage1.Controls.Add(this.rdoPlayer9);
            this.TabPage1.Controls.Add(this.txtFlatness);
            this.TabPage1.Controls.Add(this.Label25);
            this.TabPage1.Controls.Add(this.rdoPlayer3);
            this.TabPage1.Controls.Add(this.txt3x);
            this.TabPage1.Controls.Add(this.txt3y);
            this.TabPage1.Controls.Add(this.rdoPlayer4);
            this.TabPage1.Controls.Add(this.txt4x);
            this.TabPage1.Controls.Add(this.txt4y);
            this.TabPage1.Controls.Add(this.rdoPlayer5);
            this.TabPage1.Controls.Add(this.cboSymmetry);
            this.TabPage1.Controls.Add(this.txt5x);
            this.TabPage1.Controls.Add(this.txt5y);
            this.TabPage1.Controls.Add(this.rdoPlayer1);
            this.TabPage1.Controls.Add(this.rdoPlayer6);
            this.TabPage1.Controls.Add(this.txt6x);
            this.TabPage1.Controls.Add(this.txt6y);
            this.TabPage1.Controls.Add(this.rdoPlayer7);
            this.TabPage1.Controls.Add(this.txt7x);
            this.TabPage1.Controls.Add(this.txt7y);
            this.TabPage1.Controls.Add(this.rdoPlayer8);
            this.TabPage1.Controls.Add(this.txt8x);
            this.TabPage1.Controls.Add(this.txt8y);
            this.TabPage1.Controls.Add(this.Label7);
            this.TabPage1.Controls.Add(this.txtLevels);
            this.TabPage1.Controls.Add(this.Label17);
            this.TabPage1.Controls.Add(this.txtLevelFrequency);
            this.TabPage1.Controls.Add(this.txtBaseLevel);
            this.TabPage1.Controls.Add(this.Label8);
            point2 = new Point(4, 0x19);
            this.TabPage1.Location = point2;
            this.TabPage1.Name = "TabPage1";
            padding2 = new Padding(3);
            this.TabPage1.Padding = padding2;
            size2 = new Size(0x1ed, 0x18b);
            this.TabPage1.Size = size2;
            this.TabPage1.TabIndex = 0;
            this.TabPage1.Text = "Layout";
            this.TabPage1.UseVisualStyleBackColor = true;
            this.TabPage2.Controls.Add(this.Label9);
            this.TabPage2.Controls.Add(this.btnGenerateRamps);
            this.TabPage2.Controls.Add(this.txtRampDistance);
            this.TabPage2.Controls.Add(this.Label33);
            this.TabPage2.Controls.Add(this.txtRampBase);
            point2 = new Point(4, 0x19);
            this.TabPage2.Location = point2;
            this.TabPage2.Name = "TabPage2";
            padding2 = new Padding(3);
            this.TabPage2.Padding = padding2;
            size2 = new Size(0x1ed, 0x18b);
            this.TabPage2.Size = size2;
            this.TabPage2.TabIndex = 1;
            this.TabPage2.Text = "Ramps";
            this.TabPage2.UseVisualStyleBackColor = true;
            this.TabPage4.Controls.Add(this.btnGenerateTextures);
            this.TabPage4.Controls.Add(this.cboTileset);
            this.TabPage4.Controls.Add(this.Label24);
            this.TabPage4.Controls.Add(this.cbxMasterTexture);
            point2 = new Point(4, 0x19);
            this.TabPage4.Location = point2;
            this.TabPage4.Name = "TabPage4";
            padding2 = new Padding(3);
            this.TabPage4.Padding = padding2;
            size2 = new Size(0x1ed, 0x18b);
            this.TabPage4.Size = size2;
            this.TabPage4.TabIndex = 3;
            this.TabPage4.Text = "Textures";
            this.TabPage4.UseVisualStyleBackColor = true;
            point2 = new Point(0xfd, 0x156);
            this.btnGenerateTextures.Location = point2;
            this.btnGenerateTextures.Name = "btnGenerateTextures";
            size2 = new Size(0x92, 0x21);
            this.btnGenerateTextures.Size = size2;
            this.btnGenerateTextures.TabIndex = 0x6c;
            this.btnGenerateTextures.Text = "Generate Textures";
            this.btnGenerateTextures.UseCompatibleTextRendering = true;
            this.btnGenerateTextures.UseVisualStyleBackColor = true;
            this.TabPage3.Controls.Add(this.Label16);
            this.TabPage3.Controls.Add(this.txtFScatterGap);
            this.TabPage3.Controls.Add(this.Label10);
            this.TabPage3.Controls.Add(this.txtBaseOil);
            this.TabPage3.Controls.Add(this.btnGenerateObjects);
            this.TabPage3.Controls.Add(this.Label11);
            this.TabPage3.Controls.Add(this.txtOilElsewhere);
            this.TabPage3.Controls.Add(this.Label12);
            this.TabPage3.Controls.Add(this.txtOilClusterMin);
            this.TabPage3.Controls.Add(this.Label27);
            this.TabPage3.Controls.Add(this.txtOilAtATime);
            this.TabPage3.Controls.Add(this.txtTrucks);
            this.TabPage3.Controls.Add(this.Label13);
            this.TabPage3.Controls.Add(this.Label23);
            this.TabPage3.Controls.Add(this.Label31);
            this.TabPage3.Controls.Add(this.txtFClusterMax);
            this.TabPage3.Controls.Add(this.txtOilClusterMax);
            this.TabPage3.Controls.Add(this.Label22);
            this.TabPage3.Controls.Add(this.Label14);
            this.TabPage3.Controls.Add(this.txtFClusterMin);
            this.TabPage3.Controls.Add(this.Label18);
            this.TabPage3.Controls.Add(this.Label21);
            this.TabPage3.Controls.Add(this.txtOilDispersion);
            this.TabPage3.Controls.Add(this.txtFClusterChance);
            this.TabPage3.Controls.Add(this.Label19);
            this.TabPage3.Controls.Add(this.Label20);
            this.TabPage3.Controls.Add(this.txtFScatterChance);
            point2 = new Point(4, 0x19);
            this.TabPage3.Location = point2;
            this.TabPage3.Name = "TabPage3";
            size2 = new Size(0x1ed, 0x18b);
            this.TabPage3.Size = size2;
            this.TabPage3.TabIndex = 2;
            this.TabPage3.Text = "Objects";
            this.TabPage3.UseVisualStyleBackColor = true;
            point2 = new Point(-9, 0xd3);
            this.Label16.Location = point2;
            this.Label16.Name = "Label16";
            size2 = new Size(0xbd, 20);
            this.Label16.Size = size2;
            this.Label16.TabIndex = 0x6d;
            this.Label16.Text = "Scattered Feature Spacing";
            this.Label16.TextAlign = ContentAlignment.MiddleRight;
            this.Label16.UseCompatibleTextRendering = true;
            point2 = new Point(0xba, 0xd4);
            this.txtFScatterGap.Location = point2;
            this.txtFScatterGap.Name = "txtFScatterGap";
            size2 = new Size(0x2e, 0x16);
            this.txtFScatterGap.Size = size2;
            this.txtFScatterGap.TabIndex = 0x6c;
            this.txtFScatterGap.Text = "4";
            this.txtFScatterGap.TextAlign = HorizontalAlignment.Right;
            this.AutoScaleMode = AutoScaleMode.None;
            size2 = new Size(0x20f, 0x237);
            this.ClientSize = size2;
            this.Controls.Add(this.TabControl1);
            this.Controls.Add(this.lstResult);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            padding2 = new Padding(4);
            this.Margin = padding2;
            this.MaximizeBox = false;
            this.Name = "frmGenerator";
            this.Text = "Generator";
            this.TabControl1.ResumeLayout(false);
            this.TabPage1.ResumeLayout(false);
            this.TabPage1.PerformLayout();
            this.TabPage2.ResumeLayout(false);
            this.TabPage2.PerformLayout();
            this.TabPage4.ResumeLayout(false);
            this.TabPage4.PerformLayout();
            this.TabPage3.ResumeLayout(false);
            this.TabPage3.PerformLayout();
            this.ResumeLayout(false);
        }

        private void lstResult_AddResult(clsResult Result)
        {
            this.lstResult.SelectedIndex = this.lstResult.Items.Count - 1;
        }

        private void lstResult_AddText(string Text)
        {
            this.lstResult.Items.Add(Text);
            this.lstResult.SelectedIndex = this.lstResult.Items.Count - 1;
        }

        private void rdoPlayer1_CheckedChanged(object sender, EventArgs e)
        {
            if (this.rdoPlayer1.Checked)
            {
                this.PlayerCount = 1;
                this.rdoPlayer2.Checked = false;
                this.rdoPlayer3.Checked = false;
                this.rdoPlayer4.Checked = false;
                this.rdoPlayer5.Checked = false;
                this.rdoPlayer6.Checked = false;
                this.rdoPlayer7.Checked = false;
                this.rdoPlayer8.Checked = false;
                this.rdoPlayer9.Checked = false;
                this.rdoPlayer10.Checked = false;
            }
        }

        private void rdoPlayer10_CheckedChanged(object sender, EventArgs e)
        {
            if (this.rdoPlayer10.Checked)
            {
                this.PlayerCount = 10;
                this.rdoPlayer1.Checked = false;
                this.rdoPlayer2.Checked = false;
                this.rdoPlayer3.Checked = false;
                this.rdoPlayer4.Checked = false;
                this.rdoPlayer5.Checked = false;
                this.rdoPlayer6.Checked = false;
                this.rdoPlayer7.Checked = false;
                this.rdoPlayer8.Checked = false;
                this.rdoPlayer9.Checked = false;
            }
        }

        private void rdoPlayer2_CheckedChanged(object sender, EventArgs e)
        {
            if (this.rdoPlayer2.Checked)
            {
                this.PlayerCount = 2;
                this.rdoPlayer1.Checked = false;
                this.rdoPlayer3.Checked = false;
                this.rdoPlayer4.Checked = false;
                this.rdoPlayer5.Checked = false;
                this.rdoPlayer6.Checked = false;
                this.rdoPlayer7.Checked = false;
                this.rdoPlayer8.Checked = false;
                this.rdoPlayer9.Checked = false;
                this.rdoPlayer10.Checked = false;
            }
        }

        private void rdoPlayer3_CheckedChanged(object sender, EventArgs e)
        {
            if (this.rdoPlayer3.Checked)
            {
                this.PlayerCount = 3;
                this.rdoPlayer1.Checked = false;
                this.rdoPlayer2.Checked = false;
                this.rdoPlayer4.Checked = false;
                this.rdoPlayer5.Checked = false;
                this.rdoPlayer6.Checked = false;
                this.rdoPlayer7.Checked = false;
                this.rdoPlayer8.Checked = false;
                this.rdoPlayer9.Checked = false;
                this.rdoPlayer10.Checked = false;
            }
        }

        private void rdoPlayer4_CheckedChanged(object sender, EventArgs e)
        {
            if (this.rdoPlayer4.Checked)
            {
                this.PlayerCount = 4;
                this.rdoPlayer1.Checked = false;
                this.rdoPlayer2.Checked = false;
                this.rdoPlayer3.Checked = false;
                this.rdoPlayer5.Checked = false;
                this.rdoPlayer6.Checked = false;
                this.rdoPlayer7.Checked = false;
                this.rdoPlayer8.Checked = false;
                this.rdoPlayer9.Checked = false;
                this.rdoPlayer10.Checked = false;
            }
        }

        private void rdoPlayer5_CheckedChanged(object sender, EventArgs e)
        {
            if (this.rdoPlayer5.Checked)
            {
                this.PlayerCount = 5;
                this.rdoPlayer1.Checked = false;
                this.rdoPlayer2.Checked = false;
                this.rdoPlayer3.Checked = false;
                this.rdoPlayer4.Checked = false;
                this.rdoPlayer6.Checked = false;
                this.rdoPlayer7.Checked = false;
                this.rdoPlayer8.Checked = false;
                this.rdoPlayer9.Checked = false;
                this.rdoPlayer10.Checked = false;
            }
        }

        private void rdoPlayer6_CheckedChanged(object sender, EventArgs e)
        {
            if (this.rdoPlayer6.Checked)
            {
                this.PlayerCount = 6;
                this.rdoPlayer1.Checked = false;
                this.rdoPlayer2.Checked = false;
                this.rdoPlayer3.Checked = false;
                this.rdoPlayer4.Checked = false;
                this.rdoPlayer5.Checked = false;
                this.rdoPlayer7.Checked = false;
                this.rdoPlayer8.Checked = false;
                this.rdoPlayer9.Checked = false;
                this.rdoPlayer10.Checked = false;
            }
        }

        private void rdoPlayer7_CheckedChanged(object sender, EventArgs e)
        {
            if (this.rdoPlayer7.Checked)
            {
                this.PlayerCount = 7;
                this.rdoPlayer1.Checked = false;
                this.rdoPlayer2.Checked = false;
                this.rdoPlayer3.Checked = false;
                this.rdoPlayer4.Checked = false;
                this.rdoPlayer5.Checked = false;
                this.rdoPlayer6.Checked = false;
                this.rdoPlayer8.Checked = false;
                this.rdoPlayer9.Checked = false;
                this.rdoPlayer10.Checked = false;
            }
        }

        private void rdoPlayer8_CheckedChanged(object sender, EventArgs e)
        {
            if (this.rdoPlayer8.Checked)
            {
                this.PlayerCount = 8;
                this.rdoPlayer1.Checked = false;
                this.rdoPlayer2.Checked = false;
                this.rdoPlayer3.Checked = false;
                this.rdoPlayer4.Checked = false;
                this.rdoPlayer5.Checked = false;
                this.rdoPlayer6.Checked = false;
                this.rdoPlayer7.Checked = false;
                this.rdoPlayer9.Checked = false;
                this.rdoPlayer10.Checked = false;
            }
        }

        private void rdoPlayer9_CheckedChanged(object sender, EventArgs e)
        {
            if (this.rdoPlayer9.Checked)
            {
                this.PlayerCount = 9;
                this.rdoPlayer1.Checked = false;
                this.rdoPlayer2.Checked = false;
                this.rdoPlayer3.Checked = false;
                this.rdoPlayer4.Checked = false;
                this.rdoPlayer5.Checked = false;
                this.rdoPlayer6.Checked = false;
                this.rdoPlayer7.Checked = false;
                this.rdoPlayer8.Checked = false;
                this.rdoPlayer10.Checked = false;
            }
        }

        private int ValidateTextbox(TextBox TextBoxToValidate, double Min, double Max, double Multiplier)
        {
            double num;
            if (!modIO.InvariantParse_dbl(TextBoxToValidate.Text, ref num))
            {
                return 0;
            }
            int num2 = (int) Math.Round(Conversion.Int((double) (modMath.Clamp_dbl(num, Min, Max) * Multiplier)));
            TextBoxToValidate.Text = modIO.InvariantToString_sng((float) (((double) num2) / Multiplier));
            return num2;
        }

        internal virtual Button btnGenerateLayout
        {
            get
            {
                return this._btnGenerateLayout;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.btnGenerateLayout_Click);
                if (this._btnGenerateLayout != null)
                {
                    this._btnGenerateLayout.Click -= handler;
                }
                this._btnGenerateLayout = value;
                if (this._btnGenerateLayout != null)
                {
                    this._btnGenerateLayout.Click += handler;
                }
            }
        }

        internal virtual Button btnGenerateObjects
        {
            get
            {
                return this._btnGenerateObjects;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.btnGenerateObjects_Click);
                if (this._btnGenerateObjects != null)
                {
                    this._btnGenerateObjects.Click -= handler;
                }
                this._btnGenerateObjects = value;
                if (this._btnGenerateObjects != null)
                {
                    this._btnGenerateObjects.Click += handler;
                }
            }
        }

        internal virtual Button btnGenerateRamps
        {
            get
            {
                return this._btnGenerateRamps;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.btnGenerateRamps_Click);
                if (this._btnGenerateRamps != null)
                {
                    this._btnGenerateRamps.Click -= handler;
                }
                this._btnGenerateRamps = value;
                if (this._btnGenerateRamps != null)
                {
                    this._btnGenerateRamps.Click += handler;
                }
            }
        }

        internal virtual Button btnGenerateTextures
        {
            get
            {
                return this._btnGenerateTextures;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.btnGenerateTextures_Click);
                if (this._btnGenerateTextures != null)
                {
                    this._btnGenerateTextures.Click -= handler;
                }
                this._btnGenerateTextures = value;
                if (this._btnGenerateTextures != null)
                {
                    this._btnGenerateTextures.Click += handler;
                }
            }
        }

        public virtual Button btnStop
        {
            get
            {
                return this._btnStop;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.btnStop_Click);
                if (this._btnStop != null)
                {
                    this._btnStop.Click -= handler;
                }
                this._btnStop = value;
                if (this._btnStop != null)
                {
                    this._btnStop.Click += handler;
                }
            }
        }

        public virtual ComboBox cboSymmetry
        {
            get
            {
                return this._cboSymmetry;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._cboSymmetry = value;
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
                this._cboTileset = value;
            }
        }

        public virtual CheckBox cbxMasterTexture
        {
            get
            {
                return this._cbxMasterTexture;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._cbxMasterTexture = value;
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

        internal virtual ListBox lstResult
        {
            get
            {
                return this._lstResult;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._lstResult = value;
            }
        }

        public virtual RadioButton rdoPlayer1
        {
            get
            {
                return this._rdoPlayer1;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.rdoPlayer1_CheckedChanged);
                if (this._rdoPlayer1 != null)
                {
                    this._rdoPlayer1.CheckedChanged -= handler;
                }
                this._rdoPlayer1 = value;
                if (this._rdoPlayer1 != null)
                {
                    this._rdoPlayer1.CheckedChanged += handler;
                }
            }
        }

        public virtual RadioButton rdoPlayer10
        {
            get
            {
                return this._rdoPlayer10;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.rdoPlayer10_CheckedChanged);
                if (this._rdoPlayer10 != null)
                {
                    this._rdoPlayer10.CheckedChanged -= handler;
                }
                this._rdoPlayer10 = value;
                if (this._rdoPlayer10 != null)
                {
                    this._rdoPlayer10.CheckedChanged += handler;
                }
            }
        }

        public virtual RadioButton rdoPlayer2
        {
            get
            {
                return this._rdoPlayer2;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.rdoPlayer2_CheckedChanged);
                if (this._rdoPlayer2 != null)
                {
                    this._rdoPlayer2.CheckedChanged -= handler;
                }
                this._rdoPlayer2 = value;
                if (this._rdoPlayer2 != null)
                {
                    this._rdoPlayer2.CheckedChanged += handler;
                }
            }
        }

        public virtual RadioButton rdoPlayer3
        {
            get
            {
                return this._rdoPlayer3;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.rdoPlayer3_CheckedChanged);
                if (this._rdoPlayer3 != null)
                {
                    this._rdoPlayer3.CheckedChanged -= handler;
                }
                this._rdoPlayer3 = value;
                if (this._rdoPlayer3 != null)
                {
                    this._rdoPlayer3.CheckedChanged += handler;
                }
            }
        }

        public virtual RadioButton rdoPlayer4
        {
            get
            {
                return this._rdoPlayer4;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.rdoPlayer4_CheckedChanged);
                if (this._rdoPlayer4 != null)
                {
                    this._rdoPlayer4.CheckedChanged -= handler;
                }
                this._rdoPlayer4 = value;
                if (this._rdoPlayer4 != null)
                {
                    this._rdoPlayer4.CheckedChanged += handler;
                }
            }
        }

        public virtual RadioButton rdoPlayer5
        {
            get
            {
                return this._rdoPlayer5;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.rdoPlayer5_CheckedChanged);
                if (this._rdoPlayer5 != null)
                {
                    this._rdoPlayer5.CheckedChanged -= handler;
                }
                this._rdoPlayer5 = value;
                if (this._rdoPlayer5 != null)
                {
                    this._rdoPlayer5.CheckedChanged += handler;
                }
            }
        }

        public virtual RadioButton rdoPlayer6
        {
            get
            {
                return this._rdoPlayer6;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.rdoPlayer6_CheckedChanged);
                if (this._rdoPlayer6 != null)
                {
                    this._rdoPlayer6.CheckedChanged -= handler;
                }
                this._rdoPlayer6 = value;
                if (this._rdoPlayer6 != null)
                {
                    this._rdoPlayer6.CheckedChanged += handler;
                }
            }
        }

        public virtual RadioButton rdoPlayer7
        {
            get
            {
                return this._rdoPlayer7;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.rdoPlayer7_CheckedChanged);
                if (this._rdoPlayer7 != null)
                {
                    this._rdoPlayer7.CheckedChanged -= handler;
                }
                this._rdoPlayer7 = value;
                if (this._rdoPlayer7 != null)
                {
                    this._rdoPlayer7.CheckedChanged += handler;
                }
            }
        }

        public virtual RadioButton rdoPlayer8
        {
            get
            {
                return this._rdoPlayer8;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.rdoPlayer8_CheckedChanged);
                if (this._rdoPlayer8 != null)
                {
                    this._rdoPlayer8.CheckedChanged -= handler;
                }
                this._rdoPlayer8 = value;
                if (this._rdoPlayer8 != null)
                {
                    this._rdoPlayer8.CheckedChanged += handler;
                }
            }
        }

        public virtual RadioButton rdoPlayer9
        {
            get
            {
                return this._rdoPlayer9;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.rdoPlayer9_CheckedChanged);
                if (this._rdoPlayer9 != null)
                {
                    this._rdoPlayer9.CheckedChanged -= handler;
                }
                this._rdoPlayer9 = value;
                if (this._rdoPlayer9 != null)
                {
                    this._rdoPlayer9.CheckedChanged += handler;
                }
            }
        }

        internal virtual TabControl TabControl1
        {
            get
            {
                return this._TabControl1;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._TabControl1 = value;
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

        internal virtual TabPage TabPage4
        {
            get
            {
                return this._TabPage4;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._TabPage4 = value;
            }
        }

        public virtual TextBox txt10x
        {
            get
            {
                return this._txt10x;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._txt10x = value;
            }
        }

        public virtual TextBox txt10y
        {
            get
            {
                return this._txt10y;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._txt10y = value;
            }
        }

        public virtual TextBox txt1x
        {
            get
            {
                return this._txt1x;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._txt1x = value;
            }
        }

        public virtual TextBox txt1y
        {
            get
            {
                return this._txt1y;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._txt1y = value;
            }
        }

        public virtual TextBox txt2x
        {
            get
            {
                return this._txt2x;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._txt2x = value;
            }
        }

        public virtual TextBox txt2y
        {
            get
            {
                return this._txt2y;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._txt2y = value;
            }
        }

        public virtual TextBox txt3x
        {
            get
            {
                return this._txt3x;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._txt3x = value;
            }
        }

        public virtual TextBox txt3y
        {
            get
            {
                return this._txt3y;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._txt3y = value;
            }
        }

        public virtual TextBox txt4x
        {
            get
            {
                return this._txt4x;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._txt4x = value;
            }
        }

        public virtual TextBox txt4y
        {
            get
            {
                return this._txt4y;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._txt4y = value;
            }
        }

        public virtual TextBox txt5x
        {
            get
            {
                return this._txt5x;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._txt5x = value;
            }
        }

        public virtual TextBox txt5y
        {
            get
            {
                return this._txt5y;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._txt5y = value;
            }
        }

        public virtual TextBox txt6x
        {
            get
            {
                return this._txt6x;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._txt6x = value;
            }
        }

        public virtual TextBox txt6y
        {
            get
            {
                return this._txt6y;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._txt6y = value;
            }
        }

        public virtual TextBox txt7x
        {
            get
            {
                return this._txt7x;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._txt7x = value;
            }
        }

        public virtual TextBox txt7y
        {
            get
            {
                return this._txt7y;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._txt7y = value;
            }
        }

        public virtual TextBox txt8x
        {
            get
            {
                return this._txt8x;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._txt8x = value;
            }
        }

        public virtual TextBox txt8y
        {
            get
            {
                return this._txt8y;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._txt8y = value;
            }
        }

        public virtual TextBox txt9x
        {
            get
            {
                return this._txt9x;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._txt9x = value;
            }
        }

        public virtual TextBox txt9y
        {
            get
            {
                return this._txt9y;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._txt9y = value;
            }
        }

        public virtual TextBox txtBaseArea
        {
            get
            {
                return this._txtBaseArea;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._txtBaseArea = value;
            }
        }

        public virtual TextBox txtBaseLevel
        {
            get
            {
                return this._txtBaseLevel;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._txtBaseLevel = value;
            }
        }

        public virtual TextBox txtBaseOil
        {
            get
            {
                return this._txtBaseOil;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._txtBaseOil = value;
            }
        }

        public virtual TextBox txtConnectedWater
        {
            get
            {
                return this._txtConnectedWater;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._txtConnectedWater = value;
            }
        }

        public virtual TextBox txtFClusterChance
        {
            get
            {
                return this._txtFClusterChance;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._txtFClusterChance = value;
            }
        }

        public virtual TextBox txtFClusterMax
        {
            get
            {
                return this._txtFClusterMax;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._txtFClusterMax = value;
            }
        }

        public virtual TextBox txtFClusterMin
        {
            get
            {
                return this._txtFClusterMin;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._txtFClusterMin = value;
            }
        }

        public virtual TextBox txtFlatness
        {
            get
            {
                return this._txtFlatness;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._txtFlatness = value;
            }
        }

        public virtual TextBox txtFScatterChance
        {
            get
            {
                return this._txtFScatterChance;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._txtFScatterChance = value;
            }
        }

        public virtual TextBox txtFScatterGap
        {
            get
            {
                return this._txtFScatterGap;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._txtFScatterGap = value;
            }
        }

        public virtual TextBox txtHeight
        {
            get
            {
                return this._txtHeight;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._txtHeight = value;
            }
        }

        public virtual TextBox txtLevelFrequency
        {
            get
            {
                return this._txtLevelFrequency;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._txtLevelFrequency = value;
            }
        }

        public virtual TextBox txtLevels
        {
            get
            {
                return this._txtLevels;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._txtLevels = value;
            }
        }

        public virtual TextBox txtOilAtATime
        {
            get
            {
                return this._txtOilAtATime;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._txtOilAtATime = value;
            }
        }

        public virtual TextBox txtOilClusterMax
        {
            get
            {
                return this._txtOilClusterMax;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._txtOilClusterMax = value;
            }
        }

        public virtual TextBox txtOilClusterMin
        {
            get
            {
                return this._txtOilClusterMin;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._txtOilClusterMin = value;
            }
        }

        public virtual TextBox txtOilDispersion
        {
            get
            {
                return this._txtOilDispersion;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._txtOilDispersion = value;
            }
        }

        public virtual TextBox txtOilElsewhere
        {
            get
            {
                return this._txtOilElsewhere;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._txtOilElsewhere = value;
            }
        }

        public virtual TextBox txtRampBase
        {
            get
            {
                return this._txtRampBase;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._txtRampBase = value;
            }
        }

        public virtual TextBox txtRampDistance
        {
            get
            {
                return this._txtRampDistance;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._txtRampDistance = value;
            }
        }

        public virtual TextBox txtTrucks
        {
            get
            {
                return this._txtTrucks;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._txtTrucks = value;
            }
        }

        public virtual TextBox txtVariation
        {
            get
            {
                return this._txtVariation;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._txtVariation = value;
            }
        }

        public virtual TextBox txtWaterQuantity
        {
            get
            {
                return this._txtWaterQuantity;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._txtWaterQuantity = value;
            }
        }

        public virtual TextBox txtWidth
        {
            get
            {
                return this._txtWidth;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._txtWidth = value;
            }
        }
    }
}

