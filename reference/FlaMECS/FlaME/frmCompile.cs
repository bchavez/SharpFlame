namespace FlaME
{
    using Microsoft.VisualBasic;
    using Microsoft.VisualBasic.CompilerServices;
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Runtime.CompilerServices;
    using System.Windows.Forms;

    [DesignerGenerated]
    public class frmCompile : Form
    {
        [AccessedThroughProperty("btnCompileCampaign")]
        private Button _btnCompileCampaign;
        [AccessedThroughProperty("btnCompileMultiplayer")]
        private Button _btnCompileMultiplayer;
        [AccessedThroughProperty("cboCampType")]
        private ComboBox _cboCampType;
        [AccessedThroughProperty("cboLicense")]
        private ComboBox _cboLicense;
        [AccessedThroughProperty("cbxAutoScrollLimits")]
        private CheckBox _cbxAutoScrollLimits;
        [AccessedThroughProperty("cbxLevFormat")]
        private CheckBox _cbxLevFormat;
        [AccessedThroughProperty("GroupBox1")]
        private GroupBox _GroupBox1;
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
        [AccessedThroughProperty("Label2")]
        private Label _Label2;
        [AccessedThroughProperty("Label4")]
        private Label _Label4;
        [AccessedThroughProperty("Label5")]
        private Label _Label5;
        [AccessedThroughProperty("Label7")]
        private Label _Label7;
        [AccessedThroughProperty("Label8")]
        private Label _Label8;
        [AccessedThroughProperty("TabControl1")]
        private TabControl _TabControl1;
        [AccessedThroughProperty("TabPage1")]
        private TabPage _TabPage1;
        [AccessedThroughProperty("TabPage2")]
        private TabPage _TabPage2;
        [AccessedThroughProperty("txtAuthor")]
        private TextBox _txtAuthor;
        [AccessedThroughProperty("txtMultiPlayers")]
        private TextBox _txtMultiPlayers;
        [AccessedThroughProperty("txtName")]
        private TextBox _txtName;
        [AccessedThroughProperty("txtScrollMaxX")]
        private TextBox _txtScrollMaxX;
        [AccessedThroughProperty("txtScrollMaxY")]
        private TextBox _txtScrollMaxY;
        [AccessedThroughProperty("txtScrollMinX")]
        private TextBox _txtScrollMinX;
        [AccessedThroughProperty("txtScrollMinY")]
        private TextBox _txtScrollMinY;
        private IContainer components;
        private clsMap Map;

        private frmCompile(clsMap Map)
        {
            base.FormClosed += new FormClosedEventHandler(this.frmCompile_FormClosed);
            base.FormClosing += new FormClosingEventHandler(this.frmCompile_FormClosing);
            this.InitializeComponent();
            this.Icon = modProgram.ProgramIcon;
            this.Map = Map;
            Map.CompileScreen = this;
            this.UpdateControls();
        }

        public void AutoScrollLimits_Update()
        {
            if (this.cbxAutoScrollLimits.Checked)
            {
                this.txtScrollMinX.Enabled = false;
                this.txtScrollMaxX.Enabled = false;
                this.txtScrollMinY.Enabled = false;
                this.txtScrollMaxY.Enabled = false;
            }
            else
            {
                this.txtScrollMinX.Enabled = true;
                this.txtScrollMaxX.Enabled = true;
                this.txtScrollMinY.Enabled = true;
                this.txtScrollMaxY.Enabled = true;
            }
        }

        private void btnCompile_Click(object sender, EventArgs e)
        {
            int num2;
            clsResult result = new clsResult("Compile multiplayer");
            this.SaveToMap();
            string text = this.cboLicense.Text;
            if (!modIO.InvariantParse_int(this.txtMultiPlayers.Text, ref num2))
            {
                num2 = 0;
            }
            bool isXPlayerFormat = this.cbxLevFormat.Checked;
            if ((num2 < 2) | (num2 > 10))
            {
                result.ProblemAdd("The number of players must be from 2 to " + Conversions.ToString(10));
            }
            if (!isXPlayerFormat && (((num2 != 2) & (num2 != 4)) & (num2 != 8)))
            {
                result.ProblemAdd("You must enable support for this number of players.");
            }
            int num = this.ValidateMap_WaterTris();
            if (num > 0)
            {
                result.WarningAdd(Conversions.ToString(num) + " water tiles have an incorrect triangle direction. There might be in-game graphical glitches on those tiles.");
            }
            result.Add(this.ValidateMap());
            result.Add(this.ValidateMap_UnitPositions());
            result.Add(this.ValidateMap_Multiplayer(num2, isXPlayerFormat));
            string str2 = this.txtName.Text;
            int num3 = str2.Length - 1;
            num = 0;
            while (num <= num3)
            {
                char ch = str2[num];
                if (!((((ch >= 'a') & (ch <= 'z')) | ((ch >= 'A') & (ch <= 'Z'))) | ((num >= 1) & ((((ch >= '0') & (ch <= '9')) | (ch == '-')) | (ch == '_')))))
                {
                    break;
                }
                num++;
            }
            if (num < str2.Length)
            {
                result.ProblemAdd("The map's name must contain only letters, numbers, underscores and hyphens, and must begin with a letter.");
            }
            if ((str2.Length < 1) | (str2.Length > 0x10))
            {
                result.ProblemAdd("Map name must be from 1 to 16 characters.");
            }
            if (text == "")
            {
                result.ProblemAdd("Enter a valid license.");
            }
            if (result.HasProblems)
            {
                modProgram.ShowWarnings(result);
            }
            else
            {
                SaveFileDialog dialog = new SaveFileDialog();
                if (this.Map.PathInfo != null)
                {
                    dialog.InitialDirectory = this.Map.PathInfo.Path;
                }
                dialog.FileName = Conversions.ToString(num2) + "c-" + str2;
                dialog.Filter = "WZ Files (*.wz)|*.wz";
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    clsMap.sWrite_WZ_Args args = new clsMap.sWrite_WZ_Args {
                        MapName = str2,
                        Path = dialog.FileName,
                        Overwrite = true
                    };
                    this.SetScrollLimits(ref args.ScrollMin, ref args.ScrollMax);
                    args.Multiplayer = new clsMap.sWrite_WZ_Args.clsMultiplayer();
                    args.Multiplayer.AuthorName = this.txtAuthor.Text;
                    args.Multiplayer.PlayerCount = num2;
                    args.Multiplayer.IsBetaPlayerFormat = isXPlayerFormat;
                    args.Multiplayer.License = text;
                    args.CompileType = clsMap.sWrite_WZ_Args.enumCompileType.Multiplayer;
                    result.Add(this.Map.Write_WZ(args));
                    modProgram.ShowWarnings(result);
                    if (!result.HasWarnings)
                    {
                        this.Close();
                    }
                }
            }
        }

        private void btnCompileCampaign_Click(object sender, EventArgs e)
        {
            clsResult result = new clsResult("Compile campaign");
            this.SaveToMap();
            int num = this.ValidateMap_WaterTris();
            if (num > 0)
            {
                result.WarningAdd(Conversions.ToString(num) + " water tiles have an incorrect triangle direction. There might be in-game graphical glitches on those tiles.");
            }
            result.Add(this.ValidateMap());
            result.Add(this.ValidateMap_UnitPositions());
            string text = this.txtName.Text;
            if (text.Length < 1)
            {
                result.ProblemAdd("Enter a name for the campaign files.");
            }
            int selectedIndex = this.cboCampType.SelectedIndex;
            if ((selectedIndex < 0) | (selectedIndex > 2))
            {
                result.ProblemAdd("Select a campaign type.");
            }
            if (result.HasProblems)
            {
                modProgram.ShowWarnings(result);
            }
            else
            {
                FolderBrowserDialog dialog = new FolderBrowserDialog();
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    clsMap.sWrite_WZ_Args args = new clsMap.sWrite_WZ_Args {
                        MapName = text,
                        Path = dialog.SelectedPath,
                        Overwrite = false
                    };
                    this.SetScrollLimits(ref args.ScrollMin, ref args.ScrollMax);
                    args.Campaign = new clsMap.sWrite_WZ_Args.clsCampaign();
                    args.Campaign.GAMType = (uint) selectedIndex;
                    args.CompileType = clsMap.sWrite_WZ_Args.enumCompileType.Campaign;
                    result.Add(this.Map.Write_WZ(args));
                    modProgram.ShowWarnings(result);
                    if (!result.HasWarnings)
                    {
                        this.Close();
                    }
                }
            }
        }

        private void cbxAutoScrollLimits_CheckedChanged(object sender, EventArgs e)
        {
            if (this.cbxAutoScrollLimits.Enabled)
            {
                this.AutoScrollLimits_Update();
            }
        }

        public static frmCompile Create(clsMap Map)
        {
            if (Map == null)
            {
                Debugger.Break();
                return null;
            }
            if (Map.CompileScreen != null)
            {
                Debugger.Break();
                return null;
            }
            return new frmCompile(Map);
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

        private void frmCompile_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Map.CompileScreen = null;
            this.Map = null;
        }

        private void frmCompile_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.SaveToMap();
        }

        [DebuggerStepThrough]
        private void InitializeComponent()
        {
            this.txtName = new TextBox();
            this.Label1 = new Label();
            this.Label2 = new Label();
            this.txtMultiPlayers = new TextBox();
            this.btnCompileMultiplayer = new Button();
            this.Label5 = new Label();
            this.txtScrollMaxX = new TextBox();
            this.txtScrollMaxY = new TextBox();
            this.txtScrollMinY = new TextBox();
            this.Label10 = new Label();
            this.txtScrollMinX = new TextBox();
            this.Label11 = new Label();
            this.Label12 = new Label();
            this.txtAuthor = new TextBox();
            this.Label4 = new Label();
            this.Label13 = new Label();
            this.cboLicense = new ComboBox();
            this.cbxLevFormat = new CheckBox();
            this.Label7 = new Label();
            this.Label8 = new Label();
            this.cboCampType = new ComboBox();
            this.TabControl1 = new TabControl();
            this.TabPage1 = new TabPage();
            this.TabPage2 = new TabPage();
            this.btnCompileCampaign = new Button();
            this.GroupBox1 = new GroupBox();
            this.cbxAutoScrollLimits = new CheckBox();
            this.TabControl1.SuspendLayout();
            this.TabPage1.SuspendLayout();
            this.TabPage2.SuspendLayout();
            this.GroupBox1.SuspendLayout();
            this.SuspendLayout();
            Point point2 = new Point(0x6b, 15);
            this.txtName.Location = point2;
            Padding padding2 = new Padding(4);
            this.txtName.Margin = padding2;
            this.txtName.Name = "txtName";
            Size size2 = new Size(140, 0x16);
            this.txtName.Size = size2;
            this.txtName.TabIndex = 0;
            this.Label1.AutoSize = true;
            point2 = new Point(0x19, 0x12);
            this.Label1.Location = point2;
            padding2 = new Padding(4, 0, 4, 0);
            this.Label1.Margin = padding2;
            this.Label1.Name = "Label1";
            size2 = new Size(0x4a, 20);
            this.Label1.Size = size2;
            this.Label1.TabIndex = 1;
            this.Label1.Text = "Map Name:";
            this.Label1.UseCompatibleTextRendering = true;
            this.Label2.AutoSize = true;
            point2 = new Point(0x12, 20);
            this.Label2.Location = point2;
            padding2 = new Padding(4, 0, 4, 0);
            this.Label2.Margin = padding2;
            this.Label2.Name = "Label2";
            size2 = new Size(0x35, 20);
            this.Label2.Size = size2;
            this.Label2.TabIndex = 2;
            this.Label2.Text = "Players:";
            this.Label2.UseCompatibleTextRendering = true;
            point2 = new Point(0x51, 0x10);
            this.txtMultiPlayers.Location = point2;
            padding2 = new Padding(4);
            this.txtMultiPlayers.Margin = padding2;
            this.txtMultiPlayers.Name = "txtMultiPlayers";
            size2 = new Size(0x2a, 0x16);
            this.txtMultiPlayers.Size = size2;
            this.txtMultiPlayers.TabIndex = 5;
            point2 = new Point(0xfb, 0x80);
            this.btnCompileMultiplayer.Location = point2;
            padding2 = new Padding(4);
            this.btnCompileMultiplayer.Margin = padding2;
            this.btnCompileMultiplayer.Name = "btnCompileMultiplayer";
            size2 = new Size(0x80, 30);
            this.btnCompileMultiplayer.Size = size2;
            this.btnCompileMultiplayer.TabIndex = 10;
            this.btnCompileMultiplayer.Text = "Compile";
            this.btnCompileMultiplayer.UseCompatibleTextRendering = true;
            this.btnCompileMultiplayer.UseVisualStyleBackColor = true;
            this.Label5.AutoSize = true;
            point2 = new Point(15, 0x17);
            this.Label5.Location = point2;
            padding2 = new Padding(4, 0, 4, 0);
            this.Label5.Margin = padding2;
            this.Label5.Name = "Label5";
            size2 = new Size(0x26, 20);
            this.Label5.Size = size2;
            this.Label5.TabIndex = 14;
            this.Label5.Text = "Type:";
            this.Label5.UseCompatibleTextRendering = true;
            point2 = new Point(0x52, 0x58);
            this.txtScrollMaxX.Location = point2;
            padding2 = new Padding(4);
            this.txtScrollMaxX.Margin = padding2;
            this.txtScrollMaxX.Name = "txtScrollMaxX";
            size2 = new Size(0x3d, 0x16);
            this.txtScrollMaxX.Size = size2;
            this.txtScrollMaxX.TabIndex = 15;
            point2 = new Point(0x97, 0x58);
            this.txtScrollMaxY.Location = point2;
            padding2 = new Padding(4);
            this.txtScrollMaxY.Margin = padding2;
            this.txtScrollMaxY.Name = "txtScrollMaxY";
            size2 = new Size(0x3d, 0x16);
            this.txtScrollMaxY.Size = size2;
            this.txtScrollMaxY.TabIndex = 0x12;
            point2 = new Point(0x97, 0x30);
            this.txtScrollMinY.Location = point2;
            padding2 = new Padding(4);
            this.txtScrollMinY.Margin = padding2;
            this.txtScrollMinY.Name = "txtScrollMinY";
            size2 = new Size(0x3d, 0x16);
            this.txtScrollMinY.Size = size2;
            this.txtScrollMinY.TabIndex = 0x16;
            this.Label10.AutoSize = true;
            point2 = new Point(0x52, 0x1b);
            this.Label10.Location = point2;
            padding2 = new Padding(4, 0, 4, 0);
            this.Label10.Margin = padding2;
            this.Label10.Name = "Label10";
            size2 = new Size(15, 20);
            this.Label10.Size = size2;
            this.Label10.TabIndex = 0x15;
            this.Label10.Text = "x:";
            this.Label10.UseCompatibleTextRendering = true;
            point2 = new Point(0x52, 0x30);
            this.txtScrollMinX.Location = point2;
            padding2 = new Padding(4);
            this.txtScrollMinX.Margin = padding2;
            this.txtScrollMinX.Name = "txtScrollMinX";
            size2 = new Size(0x3d, 0x16);
            this.txtScrollMinX.Size = size2;
            this.txtScrollMinX.TabIndex = 20;
            this.Label11.AutoSize = true;
            point2 = new Point(10, 0x33);
            this.Label11.Location = point2;
            padding2 = new Padding(4, 0, 4, 0);
            this.Label11.Margin = padding2;
            this.Label11.Name = "Label11";
            size2 = new Size(0x3f, 20);
            this.Label11.Size = size2;
            this.Label11.TabIndex = 0x18;
            this.Label11.Text = "Minimum:";
            this.Label11.UseCompatibleTextRendering = true;
            this.Label12.AutoSize = true;
            point2 = new Point(7, 0x58);
            this.Label12.Location = point2;
            padding2 = new Padding(4, 0, 4, 0);
            this.Label12.Margin = padding2;
            this.Label12.Name = "Label12";
            size2 = new Size(0x43, 20);
            this.Label12.Size = size2;
            this.Label12.TabIndex = 0x19;
            this.Label12.Text = "Maximum:";
            this.Label12.UseCompatibleTextRendering = true;
            point2 = new Point(0x51, 0x30);
            this.txtAuthor.Location = point2;
            padding2 = new Padding(4);
            this.txtAuthor.Margin = padding2;
            this.txtAuthor.Name = "txtAuthor";
            size2 = new Size(0x7b, 0x16);
            this.txtAuthor.Size = size2;
            this.txtAuthor.TabIndex = 0x1b;
            this.Label4.AutoSize = true;
            point2 = new Point(0x16, 0x34);
            this.Label4.Location = point2;
            padding2 = new Padding(4, 0, 4, 0);
            this.Label4.Margin = padding2;
            this.Label4.Name = "Label4";
            size2 = new Size(0x30, 20);
            this.Label4.Size = size2;
            this.Label4.TabIndex = 0x1a;
            this.Label4.Text = "Author:";
            this.Label4.UseCompatibleTextRendering = true;
            this.Label13.AutoSize = true;
            point2 = new Point(14, 0x54);
            this.Label13.Location = point2;
            padding2 = new Padding(4, 0, 4, 0);
            this.Label13.Margin = padding2;
            this.Label13.Name = "Label13";
            size2 = new Size(0x37, 20);
            this.Label13.Size = size2;
            this.Label13.TabIndex = 0x1c;
            this.Label13.Text = "License:";
            this.Label13.UseCompatibleTextRendering = true;
            this.cboLicense.FormattingEnabled = true;
            this.cboLicense.Items.AddRange(new object[] { "GPL 2+", "CC BY 3.0 + GPL v2+", "CC BY-SA 3.0 + GPL v2+", "CC0" });
            point2 = new Point(0x51, 80);
            this.cboLicense.Location = point2;
            padding2 = new Padding(4);
            this.cboLicense.Margin = padding2;
            this.cboLicense.Name = "cboLicense";
            size2 = new Size(0xac, 0x18);
            this.cboLicense.Size = size2;
            this.cboLicense.TabIndex = 0x1d;
            point2 = new Point(0x83, 15);
            this.cbxLevFormat.Location = point2;
            padding2 = new Padding(4);
            this.cbxLevFormat.Margin = padding2;
            this.cbxLevFormat.Name = "cbxLevFormat";
            size2 = new Size(0x110, 0x19);
            this.cbxLevFormat.Size = size2;
            this.cbxLevFormat.TabIndex = 30;
            this.cbxLevFormat.Text = "3,5,6,7,9,10 player support (3.1+ only)";
            this.cbxLevFormat.UseCompatibleTextRendering = true;
            this.cbxLevFormat.UseVisualStyleBackColor = true;
            this.Label7.AutoSize = true;
            point2 = new Point(0x97, 0x1b);
            this.Label7.Location = point2;
            padding2 = new Padding(4, 0, 4, 0);
            this.Label7.Margin = padding2;
            this.Label7.Name = "Label7";
            size2 = new Size(15, 20);
            this.Label7.Size = size2;
            this.Label7.TabIndex = 0x1f;
            this.Label7.Text = "y:";
            this.Label7.UseCompatibleTextRendering = true;
            point2 = new Point(0x106, 80);
            this.Label8.Location = point2;
            padding2 = new Padding(4, 0, 4, 0);
            this.Label8.Margin = padding2;
            this.Label8.Name = "Label8";
            size2 = new Size(0x84, 0x29);
            this.Label8.Size = size2;
            this.Label8.TabIndex = 0x20;
            this.Label8.Text = "Select from the list or type another.";
            this.Label8.UseCompatibleTextRendering = true;
            this.cboCampType.DropDownHeight = 0x200;
            this.cboCampType.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cboCampType.DropDownWidth = 0x180;
            this.cboCampType.FormattingEnabled = true;
            this.cboCampType.IntegralHeight = false;
            this.cboCampType.Items.AddRange(new object[] { "Initial scenario state", "Scenario scroll area expansion", "Stand alone mission" });
            point2 = new Point(0x44, 20);
            this.cboCampType.Location = point2;
            padding2 = new Padding(4);
            this.cboCampType.Margin = padding2;
            this.cboCampType.Name = "cboCampType";
            size2 = new Size(160, 0x18);
            this.cboCampType.Size = size2;
            this.cboCampType.TabIndex = 0x21;
            this.TabControl1.Controls.Add(this.TabPage1);
            this.TabControl1.Controls.Add(this.TabPage2);
            point2 = new Point(12, 0x35);
            this.TabControl1.Location = point2;
            this.TabControl1.Name = "TabControl1";
            this.TabControl1.SelectedIndex = 0;
            size2 = new Size(0x19f, 0xc2);
            this.TabControl1.Size = size2;
            this.TabControl1.TabIndex = 0x22;
            this.TabPage1.Controls.Add(this.Label2);
            this.TabPage1.Controls.Add(this.txtMultiPlayers);
            this.TabPage1.Controls.Add(this.Label8);
            this.TabPage1.Controls.Add(this.Label4);
            this.TabPage1.Controls.Add(this.txtAuthor);
            this.TabPage1.Controls.Add(this.cbxLevFormat);
            this.TabPage1.Controls.Add(this.Label13);
            this.TabPage1.Controls.Add(this.cboLicense);
            this.TabPage1.Controls.Add(this.btnCompileMultiplayer);
            point2 = new Point(4, 0x19);
            this.TabPage1.Location = point2;
            this.TabPage1.Name = "TabPage1";
            padding2 = new Padding(3);
            this.TabPage1.Padding = padding2;
            size2 = new Size(0x197, 0xa5);
            this.TabPage1.Size = size2;
            this.TabPage1.TabIndex = 0;
            this.TabPage1.Text = "Multiplayer";
            this.TabPage1.UseVisualStyleBackColor = true;
            this.TabPage2.Controls.Add(this.btnCompileCampaign);
            this.TabPage2.Controls.Add(this.cboCampType);
            this.TabPage2.Controls.Add(this.Label5);
            point2 = new Point(4, 0x19);
            this.TabPage2.Location = point2;
            this.TabPage2.Name = "TabPage2";
            padding2 = new Padding(3);
            this.TabPage2.Padding = padding2;
            size2 = new Size(0x197, 0xa5);
            this.TabPage2.Size = size2;
            this.TabPage2.TabIndex = 1;
            this.TabPage2.Text = "Campaign";
            this.TabPage2.UseVisualStyleBackColor = true;
            point2 = new Point(0xfd, 0x80);
            this.btnCompileCampaign.Location = point2;
            padding2 = new Padding(4);
            this.btnCompileCampaign.Margin = padding2;
            this.btnCompileCampaign.Name = "btnCompileCampaign";
            size2 = new Size(0x80, 30);
            this.btnCompileCampaign.Size = size2;
            this.btnCompileCampaign.TabIndex = 11;
            this.btnCompileCampaign.Text = "Compile";
            this.btnCompileCampaign.UseVisualStyleBackColor = true;
            this.GroupBox1.Controls.Add(this.Label12);
            this.GroupBox1.Controls.Add(this.txtScrollMaxX);
            this.GroupBox1.Controls.Add(this.Label7);
            this.GroupBox1.Controls.Add(this.txtScrollMaxY);
            this.GroupBox1.Controls.Add(this.txtScrollMinX);
            this.GroupBox1.Controls.Add(this.Label11);
            this.GroupBox1.Controls.Add(this.Label10);
            this.GroupBox1.Controls.Add(this.txtScrollMinY);
            point2 = new Point(12, 280);
            this.GroupBox1.Location = point2;
            this.GroupBox1.Name = "GroupBox1";
            size2 = new Size(0x101, 0x84);
            this.GroupBox1.Size = size2;
            this.GroupBox1.TabIndex = 0x23;
            this.GroupBox1.TabStop = false;
            this.GroupBox1.Text = "Scroll Limits";
            this.GroupBox1.UseCompatibleTextRendering = true;
            this.cbxAutoScrollLimits.AutoSize = true;
            point2 = new Point(12, 0xfd);
            this.cbxAutoScrollLimits.Location = point2;
            this.cbxAutoScrollLimits.Name = "cbxAutoScrollLimits";
            size2 = new Size(0xce, 0x15);
            this.cbxAutoScrollLimits.Size = size2;
            this.cbxAutoScrollLimits.TabIndex = 0x20;
            this.cbxAutoScrollLimits.Text = "Set Scroll Limits Automatically";
            this.cbxAutoScrollLimits.UseCompatibleTextRendering = true;
            this.cbxAutoScrollLimits.UseVisualStyleBackColor = true;
            this.AutoScaleMode = AutoScaleMode.None;
            size2 = new Size(0x1b4, 0x1ad);
            this.ClientSize = size2;
            this.Controls.Add(this.cbxAutoScrollLimits);
            this.Controls.Add(this.GroupBox1);
            this.Controls.Add(this.TabControl1);
            this.Controls.Add(this.Label1);
            this.Controls.Add(this.txtName);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            padding2 = new Padding(4);
            this.Margin = padding2;
            this.MaximizeBox = false;
            this.Name = "frmCompile";
            this.Text = "Compile Map";
            this.TabControl1.ResumeLayout(false);
            this.TabPage1.ResumeLayout(false);
            this.TabPage1.PerformLayout();
            this.TabPage2.ResumeLayout(false);
            this.TabPage2.PerformLayout();
            this.GroupBox1.ResumeLayout(false);
            this.GroupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void SaveToMap()
        {
            this.Map.InterfaceOptions.CompileName = this.txtName.Text;
            this.Map.InterfaceOptions.CompileMultiPlayers = this.txtMultiPlayers.Text;
            this.Map.InterfaceOptions.CompileMultiXPlayers = this.cbxLevFormat.Checked;
            this.Map.InterfaceOptions.CompileMultiAuthor = this.txtAuthor.Text;
            this.Map.InterfaceOptions.CompileMultiLicense = this.cboLicense.Text;
            this.Map.InterfaceOptions.CampaignGameType = this.cboCampType.SelectedIndex;
            bool flag = false;
            try
            {
                this.Map.InterfaceOptions.ScrollMin.X = Conversions.ToInteger(this.txtScrollMinX.Text);
            }
            catch (Exception exception1)
            {
                ProjectData.SetProjectError(exception1);
                Exception exception = exception1;
                flag = true;
                this.Map.InterfaceOptions.ScrollMin.X = 0;
                ProjectData.ClearProjectError();
            }
            try
            {
                this.Map.InterfaceOptions.ScrollMin.Y = Conversions.ToInteger(this.txtScrollMinY.Text);
            }
            catch (Exception exception5)
            {
                ProjectData.SetProjectError(exception5);
                Exception exception2 = exception5;
                flag = true;
                this.Map.InterfaceOptions.ScrollMin.Y = 0;
                ProjectData.ClearProjectError();
            }
            try
            {
                this.Map.InterfaceOptions.ScrollMax.X = Conversions.ToUInteger(this.txtScrollMaxX.Text);
            }
            catch (Exception exception6)
            {
                ProjectData.SetProjectError(exception6);
                Exception exception3 = exception6;
                flag = true;
                this.Map.InterfaceOptions.ScrollMax.X = 0;
                ProjectData.ClearProjectError();
            }
            try
            {
                this.Map.InterfaceOptions.ScrollMax.Y = Conversions.ToUInteger(this.txtScrollMaxY.Text);
            }
            catch (Exception exception7)
            {
                ProjectData.SetProjectError(exception7);
                Exception exception4 = exception7;
                flag = true;
                this.Map.InterfaceOptions.ScrollMax.Y = 0;
                ProjectData.ClearProjectError();
            }
            this.Map.InterfaceOptions.AutoScrollLimits = this.cbxAutoScrollLimits.Checked | flag;
            this.Map.SetChanged();
            this.UpdateControls();
        }

        private void SetScrollLimits(ref modMath.sXY_int Min, ref modMath.sXY_uint Max)
        {
            Min.X = 0;
            Min.Y = 0;
            Max.X = (uint) this.Map.Terrain.TileSize.X;
            Max.Y = (uint) this.Map.Terrain.TileSize.Y;
            if (!this.cbxAutoScrollLimits.Checked)
            {
                modIO.InvariantParse_int(this.txtScrollMinX.Text, ref Min.X);
                modIO.InvariantParse_int(this.txtScrollMinY.Text, ref Min.Y);
                modIO.InvariantParse_uint(this.txtScrollMaxX.Text, ref Max.X);
                modIO.InvariantParse_uint(this.txtScrollMaxY.Text, ref Max.Y);
            }
        }

        private void UpdateControls()
        {
            this.txtName.Text = this.Map.InterfaceOptions.CompileName;
            this.txtMultiPlayers.Text = this.Map.InterfaceOptions.CompileMultiPlayers;
            this.cbxLevFormat.Checked = this.Map.InterfaceOptions.CompileMultiXPlayers;
            this.txtAuthor.Text = this.Map.InterfaceOptions.CompileMultiAuthor;
            this.cboLicense.Text = this.Map.InterfaceOptions.CompileMultiLicense;
            this.cboCampType.SelectedIndex = this.Map.InterfaceOptions.CampaignGameType;
            this.cbxAutoScrollLimits.Checked = this.Map.InterfaceOptions.AutoScrollLimits;
            this.AutoScrollLimits_Update();
            this.txtScrollMinX.Text = modIO.InvariantToString_int(this.Map.InterfaceOptions.ScrollMin.X);
            this.txtScrollMinY.Text = modIO.InvariantToString_int(this.Map.InterfaceOptions.ScrollMin.Y);
            this.txtScrollMaxX.Text = modIO.InvariantToString_uint(this.Map.InterfaceOptions.ScrollMax.X);
            this.txtScrollMaxY.Text = modIO.InvariantToString_uint(this.Map.InterfaceOptions.ScrollMax.Y);
        }

        private clsResult ValidateMap()
        {
            clsStructureType type;
            IEnumerator enumerator;
            IEnumerator enumerator2;
            clsResult result = new clsResult("Validate map");
            if (this.Map.Terrain.TileSize.X > 250)
            {
                result.WarningAdd("Map width is too large. The maximum is " + Conversions.ToString(250) + ".");
            }
            if (this.Map.Terrain.TileSize.Y > 250)
            {
                result.WarningAdd("Map height is too large. The maximum is " + Conversions.ToString(250) + ".");
            }
            if (this.Map.Tileset == null)
            {
                result.ProblemAdd("No tileset selected.");
            }
            int[,] numArray = new int[10, (modProgram.ObjectData.StructureTypes.Count - 1) + 1];
            int[] numArray2 = new int[(modProgram.ObjectData.StructureTypes.Count - 1) + 1];
            try
            {
                enumerator = this.Map.Units.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    clsMap.clsUnit current = (clsMap.clsUnit) enumerator.Current;
                    if (current.Type.Type == clsUnitType.enumType.PlayerStructure)
                    {
                        int arrayPosition;
                        type = (clsStructureType) current.Type;
                        if (current.UnitGroup == this.Map.ScavengerUnitGroup)
                        {
                            int[] numArray3 = numArray2;
                            arrayPosition = type.StructureType_ObjectDataLink.ArrayPosition;
                            numArray3[arrayPosition]++;
                        }
                        else
                        {
                            int[,] numArray4 = numArray;
                            arrayPosition = current.UnitGroup.WZ_StartPos;
                            int num4 = type.StructureType_ObjectDataLink.ArrayPosition;
                            numArray4[arrayPosition, num4]++;
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
            try
            {
                enumerator2 = modProgram.ObjectData.StructureTypes.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    type = (clsStructureType) enumerator2.Current;
                    int index = type.StructureType_ObjectDataLink.ArrayPosition;
                    int num = 0;
                    do
                    {
                        if (numArray[num, index] > 0xff)
                        {
                            result.ProblemAdd("Player " + Conversions.ToString(num) + " has too many (" + Conversions.ToString(numArray[num, index]) + ") of structure \"" + type.Code + "\". The limit is 255 of any one structure type.");
                        }
                        num++;
                    }
                    while (num <= 9);
                    if (numArray2[index] > 0xff)
                    {
                        result.ProblemAdd("Scavengers have too many (" + Conversions.ToString(numArray2[index]) + ") of structure \"" + type.Code + "\". The limit is 255 of any one structure type.");
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
            return result;
        }

        private clsResult ValidateMap_Multiplayer(int PlayerCount, bool IsXPlayerFormat)
        {
            IEnumerator enumerator;
            clsResult result = new clsResult("Validate for multiplayer");
            if ((PlayerCount < 2) | (PlayerCount > 10))
            {
                result.ProblemAdd("Unable to evaluate for multiplayer due to bad number of players.");
                return result;
            }
            int[] numArray2 = new int[10];
            int[] numArray = new int[10];
            int[] numArray3 = new int[10];
            int num = 0;
            int num3 = 0;
            int num2 = Math.Max(PlayerCount, 7);
            try
            {
                enumerator = this.Map.Units.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    clsMap.clsUnit current = (clsMap.clsUnit) enumerator.Current;
                    if (current.UnitGroup != this.Map.ScavengerUnitGroup)
                    {
                        int[] numArray4;
                        int num5;
                        if (current.Type.Type == clsUnitType.enumType.PlayerDroid)
                        {
                            clsDroidDesign design = (clsDroidDesign) current.Type;
                            if (((((design.Body != null) & (design.Propulsion != null)) & (design.Turret1 != null)) & (design.TurretCount == 1)) && (design.Turret1.TurretType == clsTurret.enumTurretType.Construct))
                            {
                                numArray4 = numArray3;
                                num5 = current.UnitGroup.WZ_StartPos;
                                numArray4[num5]++;
                                if (design.IsTemplate)
                                {
                                    numArray4 = numArray;
                                    num5 = current.UnitGroup.WZ_StartPos;
                                    numArray4[num5]++;
                                }
                            }
                        }
                        else if (current.Type.Type == clsUnitType.enumType.PlayerStructure)
                        {
                            clsStructureType type = (clsStructureType) current.Type;
                            if (type.Code == "A0CommandCentre")
                            {
                                numArray4 = numArray2;
                                num5 = current.UnitGroup.WZ_StartPos;
                                numArray4[num5]++;
                            }
                        }
                    }
                    if (current.Type.Type != clsUnitType.enumType.Feature)
                    {
                        if ((current.UnitGroup.WZ_StartPos == num2) | (current.UnitGroup == this.Map.ScavengerUnitGroup))
                        {
                            num++;
                        }
                        else if ((current.UnitGroup.WZ_StartPos >= PlayerCount) && (num3 < 0x20))
                        {
                            num3++;
                            clsResultProblemGoto<clsResultItemPosGoto> item = modResults.CreateResultProblemGotoForObject(current);
                            item.Text = "An unused player (" + Conversions.ToString(current.UnitGroup.WZ_StartPos) + ") has a unit at " + current.GetPosText() + ".";
                            result.ItemAdd(item);
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
            if (!((num2 <= 7) | IsXPlayerFormat) && (num > 0))
            {
                result.ProblemAdd("Scavengers are not supported on a map with this number of players without enabling X player support.");
            }
            int num6 = PlayerCount - 1;
            for (int i = 0; i <= num6; i++)
            {
                if (numArray2[i] == 0)
                {
                    result.ProblemAdd("There is no Command Centre for player " + Conversions.ToString(i) + ".");
                }
                if (numArray3[i] == 0)
                {
                    result.ProblemAdd("There are no constructor units for player " + Conversions.ToString(i) + ".");
                }
                else if (numArray[i] == 0)
                {
                    result.WarningAdd("All constructor units for player " + Conversions.ToString(i) + " will only exist in master.");
                }
            }
            return result;
        }

        private clsResult ValidateMap_UnitPositions()
        {
            clsStructureType.enumStructureType structureType;
            clsMap.clsUnit current;
            IEnumerator enumerator;
            IEnumerator enumerator2;
            IEnumerator enumerator3;
            clsResult result = new clsResult("Validate unit positions");
            bool[,] flagArray = new bool[(this.Map.Terrain.TileSize.X - 1) + 1, (this.Map.Terrain.TileSize.Y - 1) + 1];
            clsStructureType[,] typeArray2 = new clsStructureType[(this.Map.Terrain.TileSize.X - 1) + 1, (this.Map.Terrain.TileSize.Y - 1) + 1];
            clsFeatureType[,] typeArray = new clsFeatureType[(this.Map.Terrain.TileSize.X - 1) + 1, (this.Map.Terrain.TileSize.Y - 1) + 1];
            clsMap.clsUnitGroup[,] groupArray = new clsMap.clsUnitGroup[(this.Map.Terrain.TileSize.X - 1) + 1, (this.Map.Terrain.TileSize.Y - 1) + 1];
            bool[] flagArray2 = new bool[(this.Map.Units.Count - 1) + 1];
            try
            {
                enumerator = this.Map.Units.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    current = (clsMap.clsUnit) enumerator.Current;
                    if (current.Type.Type == clsUnitType.enumType.PlayerStructure)
                    {
                        clsStructureType type = (clsStructureType) current.Type;
                        structureType = type.StructureType;
                        flagArray2[current.MapLink.ArrayPosition] = type.IsModule() | (structureType == clsStructureType.enumStructureType.ResourceExtractor);
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
                enumerator2 = this.Map.Units.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    current = (clsMap.clsUnit) enumerator2.Current;
                    if (!flagArray2[current.MapLink.ArrayPosition])
                    {
                        modMath.sXY_int _int2;
                        modMath.sXY_int _int4;
                        modMath.sXY_int footprint = current.Type.get_GetFootprintSelected(current.Rotation);
                        this.Map.GetFootprintTileRange(current.Pos.Horizontal, footprint, ref _int4, ref _int2);
                        if ((((_int4.X < 0) | (_int2.X >= this.Map.Terrain.TileSize.X)) | (_int4.Y < 0)) | (_int2.Y >= this.Map.Terrain.TileSize.Y))
                        {
                            clsResultProblemGoto<clsResultItemPosGoto> item = modResults.CreateResultProblemGotoForObject(current);
                            item.Text = "Unit off map at position " + current.GetPosText() + ".";
                            result.ItemAdd(item);
                        }
                        else
                        {
                            int y = _int2.Y;
                            for (int i = _int4.Y; i <= y; i++)
                            {
                                int x = _int2.X;
                                for (int j = _int4.X; j <= x; j++)
                                {
                                    if (flagArray[j, i])
                                    {
                                        clsResultProblemGoto<clsResultItemPosGoto> goto2 = modResults.CreateResultProblemGotoForObject(current);
                                        goto2.Text = "Bad unit overlap on tile " + Conversions.ToString(j) + ", " + Conversions.ToString(i) + ".";
                                        result.ItemAdd(goto2);
                                    }
                                    else
                                    {
                                        flagArray[j, i] = true;
                                        if (current.Type.Type == clsUnitType.enumType.PlayerStructure)
                                        {
                                            typeArray2[j, i] = (clsStructureType) current.Type;
                                        }
                                        else if (current.Type.Type == clsUnitType.enumType.Feature)
                                        {
                                            typeArray[j, i] = (clsFeatureType) current.Type;
                                        }
                                        groupArray[j, i] = current.UnitGroup;
                                    }
                                }
                            }
                        }
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
            try
            {
                enumerator3 = this.Map.Units.GetEnumerator();
                while (enumerator3.MoveNext())
                {
                    modMath.sXY_int _int;
                    bool flag;
                    current = (clsMap.clsUnit) enumerator3.Current;
                    if (!flagArray2[current.MapLink.ArrayPosition])
                    {
                        continue;
                    }
                    structureType = ((clsStructureType) current.Type).StructureType;
                    _int.X = (int) Math.Round(((double) (((double) current.Pos.Horizontal.X) / 128.0)));
                    _int.Y = (int) Math.Round(((double) (((double) current.Pos.Horizontal.Y) / 128.0)));
                    if ((((_int.X < 0) | (_int.X >= this.Map.Terrain.TileSize.X)) | (_int.Y < 0)) | (_int.Y >= this.Map.Terrain.TileSize.Y))
                    {
                        clsResultProblemGoto<clsResultItemPosGoto> goto3 = modResults.CreateResultProblemGotoForObject(current);
                        goto3.Text = "Module off map at position " + current.GetPosText() + ".";
                        result.ItemAdd(goto3);
                        continue;
                    }
                    if (typeArray2[_int.X, _int.Y] != null)
                    {
                        if (groupArray[_int.X, _int.Y] == current.UnitGroup)
                        {
                            switch (structureType)
                            {
                                case clsStructureType.enumStructureType.FactoryModule:
                                    if ((typeArray2[_int.X, _int.Y].StructureType == clsStructureType.enumStructureType.Factory) | (typeArray2[_int.X, _int.Y].StructureType == clsStructureType.enumStructureType.VTOLFactory))
                                    {
                                        flag = true;
                                    }
                                    else
                                    {
                                        flag = false;
                                    }
                                    goto Label_0648;

                                case clsStructureType.enumStructureType.PowerModule:
                                    if (typeArray2[_int.X, _int.Y].StructureType == clsStructureType.enumStructureType.PowerGenerator)
                                    {
                                        flag = true;
                                    }
                                    else
                                    {
                                        flag = false;
                                    }
                                    goto Label_0648;

                                case clsStructureType.enumStructureType.ResearchModule:
                                    if (typeArray2[_int.X, _int.Y].StructureType == clsStructureType.enumStructureType.Research)
                                    {
                                        flag = true;
                                    }
                                    else
                                    {
                                        flag = false;
                                    }
                                    goto Label_0648;
                            }
                            flag = false;
                        }
                        else
                        {
                            flag = false;
                        }
                    }
                    else if (typeArray[_int.X, _int.Y] != null)
                    {
                        if (structureType == clsStructureType.enumStructureType.ResourceExtractor)
                        {
                            if (typeArray[_int.X, _int.Y].FeatureType == clsFeatureType.enumFeatureType.OilResource)
                            {
                                flag = true;
                            }
                            else
                            {
                                flag = false;
                            }
                        }
                        else
                        {
                            flag = false;
                        }
                    }
                    else if (structureType == clsStructureType.enumStructureType.ResourceExtractor)
                    {
                        flag = true;
                    }
                    else
                    {
                        flag = false;
                    }
                Label_0648:
                    if (!flag)
                    {
                        clsResultProblemGoto<clsResultItemPosGoto> goto4 = modResults.CreateResultProblemGotoForObject(current);
                        goto4.Text = "Bad module on tile " + Conversions.ToString(_int.X) + ", " + Conversions.ToString(_int.Y) + ".";
                        result.ItemAdd(goto4);
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
            return result;
        }

        private int ValidateMap_WaterTris()
        {
            int num;
            if (this.Map.Tileset == null)
            {
                return 0;
            }
            int num5 = this.Map.Terrain.TileSize.Y - 1;
            for (int i = 0; i <= num5; i++)
            {
                int num6 = this.Map.Terrain.TileSize.X - 1;
                for (int j = 0; j <= num6; j++)
                {
                    if ((this.Map.Terrain.Tiles[j, i].Tri && ((this.Map.Terrain.Tiles[j, i].Texture.TextureNum >= 0) & (this.Map.Terrain.Tiles[j, i].Texture.TextureNum < this.Map.Tileset.TileCount))) && (this.Map.Tileset.Tiles[this.Map.Terrain.Tiles[j, i].Texture.TextureNum].Default_Type == 7))
                    {
                        num++;
                    }
                }
            }
            return num;
        }

        public virtual Button btnCompileCampaign
        {
            get
            {
                return this._btnCompileCampaign;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.btnCompileCampaign_Click);
                if (this._btnCompileCampaign != null)
                {
                    this._btnCompileCampaign.Click -= handler;
                }
                this._btnCompileCampaign = value;
                if (this._btnCompileCampaign != null)
                {
                    this._btnCompileCampaign.Click += handler;
                }
            }
        }

        public virtual Button btnCompileMultiplayer
        {
            get
            {
                return this._btnCompileMultiplayer;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.btnCompile_Click);
                if (this._btnCompileMultiplayer != null)
                {
                    this._btnCompileMultiplayer.Click -= handler;
                }
                this._btnCompileMultiplayer = value;
                if (this._btnCompileMultiplayer != null)
                {
                    this._btnCompileMultiplayer.Click += handler;
                }
            }
        }

        public virtual ComboBox cboCampType
        {
            get
            {
                return this._cboCampType;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._cboCampType = value;
            }
        }

        public virtual ComboBox cboLicense
        {
            get
            {
                return this._cboLicense;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._cboLicense = value;
            }
        }

        public virtual CheckBox cbxAutoScrollLimits
        {
            get
            {
                return this._cbxAutoScrollLimits;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.cbxAutoScrollLimits_CheckedChanged);
                if (this._cbxAutoScrollLimits != null)
                {
                    this._cbxAutoScrollLimits.CheckedChanged -= handler;
                }
                this._cbxAutoScrollLimits = value;
                if (this._cbxAutoScrollLimits != null)
                {
                    this._cbxAutoScrollLimits.CheckedChanged += handler;
                }
            }
        }

        public virtual CheckBox cbxLevFormat
        {
            get
            {
                return this._cbxLevFormat;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._cbxLevFormat = value;
            }
        }

        public virtual GroupBox GroupBox1
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

        public virtual TabControl TabControl1
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

        public virtual TabPage TabPage1
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

        public virtual TabPage TabPage2
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

        public virtual TextBox txtAuthor
        {
            get
            {
                return this._txtAuthor;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._txtAuthor = value;
            }
        }

        public virtual TextBox txtMultiPlayers
        {
            get
            {
                return this._txtMultiPlayers;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._txtMultiPlayers = value;
            }
        }

        public virtual TextBox txtName
        {
            get
            {
                return this._txtName;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._txtName = value;
            }
        }

        public virtual TextBox txtScrollMaxX
        {
            get
            {
                return this._txtScrollMaxX;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._txtScrollMaxX = value;
            }
        }

        public virtual TextBox txtScrollMaxY
        {
            get
            {
                return this._txtScrollMaxY;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._txtScrollMaxY = value;
            }
        }

        public virtual TextBox txtScrollMinX
        {
            get
            {
                return this._txtScrollMinX;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._txtScrollMinX = value;
            }
        }

        public virtual TextBox txtScrollMinY
        {
            get
            {
                return this._txtScrollMinY;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._txtScrollMinY = value;
            }
        }
    }
}

