namespace SharpFlame.Old
{
	partial class frmGenerator : System.Windows.Forms.Form
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
			this.Label1 = new System.Windows.Forms.Label();
			this.FormClosing += frmGenerator_FormClosing;
			this.Load += frmWZMapGen_Load;
			this.txtWidth = new System.Windows.Forms.TextBox();
			this.txtHeight = new System.Windows.Forms.TextBox();
			this.Label2 = new System.Windows.Forms.Label();
			this.txt1x = new System.Windows.Forms.TextBox();
			this.Label3 = new System.Windows.Forms.Label();
			this.Label4 = new System.Windows.Forms.Label();
			this.Label5 = new System.Windows.Forms.Label();
			this.rdoPlayer2 = new System.Windows.Forms.RadioButton();
			this.rdoPlayer2.CheckedChanged += this.rdoPlayer2_CheckedChanged;
			this.txt1y = new System.Windows.Forms.TextBox();
			this.txt2y = new System.Windows.Forms.TextBox();
			this.txt2x = new System.Windows.Forms.TextBox();
			this.txt3y = new System.Windows.Forms.TextBox();
			this.txt3x = new System.Windows.Forms.TextBox();
			this.rdoPlayer3 = new System.Windows.Forms.RadioButton();
			this.rdoPlayer3.CheckedChanged += this.rdoPlayer3_CheckedChanged;
			this.txt4y = new System.Windows.Forms.TextBox();
			this.txt4x = new System.Windows.Forms.TextBox();
			this.rdoPlayer4 = new System.Windows.Forms.RadioButton();
			this.rdoPlayer4.CheckedChanged += this.rdoPlayer4_CheckedChanged;
			this.txt5y = new System.Windows.Forms.TextBox();
			this.txt5x = new System.Windows.Forms.TextBox();
			this.rdoPlayer5 = new System.Windows.Forms.RadioButton();
			this.rdoPlayer5.CheckedChanged += this.rdoPlayer5_CheckedChanged;
			this.txt6y = new System.Windows.Forms.TextBox();
			this.txt6x = new System.Windows.Forms.TextBox();
			this.rdoPlayer6 = new System.Windows.Forms.RadioButton();
			this.rdoPlayer6.CheckedChanged += this.rdoPlayer6_CheckedChanged;
			this.txt7y = new System.Windows.Forms.TextBox();
			this.txt7x = new System.Windows.Forms.TextBox();
			this.rdoPlayer7 = new System.Windows.Forms.RadioButton();
			this.rdoPlayer7.CheckedChanged += this.rdoPlayer7_CheckedChanged;
			this.txt8y = new System.Windows.Forms.TextBox();
			this.txt8x = new System.Windows.Forms.TextBox();
			this.rdoPlayer8 = new System.Windows.Forms.RadioButton();
			this.rdoPlayer8.CheckedChanged += this.rdoPlayer8_CheckedChanged;
			this.Label7 = new System.Windows.Forms.Label();
			this.txtLevels = new System.Windows.Forms.TextBox();
			this.txtLevelFrequency = new System.Windows.Forms.TextBox();
			this.Label8 = new System.Windows.Forms.Label();
			this.txtRampDistance = new System.Windows.Forms.TextBox();
			this.Label9 = new System.Windows.Forms.Label();
			this.txtBaseOil = new System.Windows.Forms.TextBox();
			this.Label10 = new System.Windows.Forms.Label();
			this.txtOilElsewhere = new System.Windows.Forms.TextBox();
			this.Label11 = new System.Windows.Forms.Label();
			this.txtOilClusterMin = new System.Windows.Forms.TextBox();
			this.Label12 = new System.Windows.Forms.Label();
			this.Label13 = new System.Windows.Forms.Label();
			this.Label14 = new System.Windows.Forms.Label();
			this.txtOilClusterMax = new System.Windows.Forms.TextBox();
			this.txtBaseLevel = new System.Windows.Forms.TextBox();
			this.Label17 = new System.Windows.Forms.Label();
			this.txtOilDispersion = new System.Windows.Forms.TextBox();
			this.Label18 = new System.Windows.Forms.Label();
			this.txtFScatterChance = new System.Windows.Forms.TextBox();
			this.Label19 = new System.Windows.Forms.Label();
			this.txtFClusterChance = new System.Windows.Forms.TextBox();
			this.Label20 = new System.Windows.Forms.Label();
			this.txtFClusterMin = new System.Windows.Forms.TextBox();
			this.Label21 = new System.Windows.Forms.Label();
			this.txtFClusterMax = new System.Windows.Forms.TextBox();
			this.Label22 = new System.Windows.Forms.Label();
			this.txtTrucks = new System.Windows.Forms.TextBox();
			this.Label23 = new System.Windows.Forms.Label();
			this.Label24 = new System.Windows.Forms.Label();
			this.cboTileset = new System.Windows.Forms.ComboBox();
			this.txtFlatness = new System.Windows.Forms.TextBox();
			this.Label25 = new System.Windows.Forms.Label();
			this.txtWaterQuantity = new System.Windows.Forms.TextBox();
			this.Label26 = new System.Windows.Forms.Label();
			this.Label27 = new System.Windows.Forms.Label();
			this.txtVariation = new System.Windows.Forms.TextBox();
			this.Label28 = new System.Windows.Forms.Label();
			this.rdoPlayer1 = new System.Windows.Forms.RadioButton();
			this.rdoPlayer1.CheckedChanged += this.rdoPlayer1_CheckedChanged;
			this.cboSymmetry = new System.Windows.Forms.ComboBox();
			this.Label6 = new System.Windows.Forms.Label();
			this.txtOilAtATime = new System.Windows.Forms.TextBox();
			this.Label31 = new System.Windows.Forms.Label();
			this.txtRampBase = new System.Windows.Forms.TextBox();
			this.Label33 = new System.Windows.Forms.Label();
			this.txtConnectedWater = new System.Windows.Forms.TextBox();
			this.Label34 = new System.Windows.Forms.Label();
			this.txt9y = new System.Windows.Forms.TextBox();
			this.txt9x = new System.Windows.Forms.TextBox();
			this.rdoPlayer9 = new System.Windows.Forms.RadioButton();
			this.rdoPlayer9.CheckedChanged += this.rdoPlayer9_CheckedChanged;
			this.txt10y = new System.Windows.Forms.TextBox();
			this.txt10x = new System.Windows.Forms.TextBox();
			this.rdoPlayer10 = new System.Windows.Forms.RadioButton();
			this.rdoPlayer10.CheckedChanged += this.rdoPlayer10_CheckedChanged;
			this.cbxMasterTexture = new System.Windows.Forms.CheckBox();
			this.txtBaseArea = new System.Windows.Forms.TextBox();
			this.Label15 = new System.Windows.Forms.Label();
			this.btnGenerateLayout = new System.Windows.Forms.Button();
			this.btnGenerateLayout.Click += this.btnGenerateLayout_Click;
			this.btnGenerateObjects = new System.Windows.Forms.Button();
			this.btnGenerateObjects.Click += this.btnGenerateObjects_Click;
			this.btnStop = new System.Windows.Forms.Button();
			this.btnStop.Click += this.btnStop_Click;
			this.lstResult = new System.Windows.Forms.ListBox();
			this.btnGenerateRamps = new System.Windows.Forms.Button();
			this.btnGenerateRamps.Click += this.btnGenerateRamps_Click;
			this.TabControl1 = new System.Windows.Forms.TabControl();
			this.TabPage1 = new System.Windows.Forms.TabPage();
			this.TabPage2 = new System.Windows.Forms.TabPage();
			this.TabPage4 = new System.Windows.Forms.TabPage();
			this.btnGenerateTextures = new System.Windows.Forms.Button();
			this.btnGenerateTextures.Click += this.btnGenerateTextures_Click;
			this.TabPage3 = new System.Windows.Forms.TabPage();
			this.Label16 = new System.Windows.Forms.Label();
			this.txtFScatterGap = new System.Windows.Forms.TextBox();
			this.TabControl1.SuspendLayout();
			this.TabPage1.SuspendLayout();
			this.TabPage2.SuspendLayout();
			this.TabPage4.SuspendLayout();
			this.TabPage3.SuspendLayout();
			this.SuspendLayout();
			//
			//Label1
			//
			this.Label1.AutoSize = true;
			this.Label1.Location = new System.Drawing.Point(36, 40);
			this.Label1.Name = "Label1";
			this.Label1.Size = new System.Drawing.Size(39, 20);
			this.Label1.TabIndex = 5;
			this.Label1.Text = "Width";
			this.Label1.UseCompatibleTextRendering = true;
			//
			//txtWidth
			//
			this.txtWidth.Location = new System.Drawing.Point(86, 37);
			this.txtWidth.Name = "txtWidth";
			this.txtWidth.Size = new System.Drawing.Size(46, 22);
			this.txtWidth.TabIndex = 1;
			this.txtWidth.Text = "128";
			this.txtWidth.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			//
			//txtHeight
			//
			this.txtHeight.Location = new System.Drawing.Point(188, 37);
			this.txtHeight.Name = "txtHeight";
			this.txtHeight.Size = new System.Drawing.Size(46, 22);
			this.txtHeight.TabIndex = 2;
			this.txtHeight.Text = "128";
			this.txtHeight.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			//
			//Label2
			//
			this.Label2.AutoSize = true;
			this.Label2.Location = new System.Drawing.Point(138, 40);
			this.Label2.Name = "Label2";
			this.Label2.Size = new System.Drawing.Size(44, 20);
			this.Label2.TabIndex = 8;
			this.Label2.Text = "Height";
			this.Label2.UseCompatibleTextRendering = true;
			//
			//txt1x
			//
			this.txt1x.Location = new System.Drawing.Point(123, 97);
			this.txt1x.Name = "txt1x";
			this.txt1x.Size = new System.Drawing.Size(46, 22);
			this.txt1x.TabIndex = 4;
			this.txt1x.Text = "0";
			this.txt1x.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			//
			//Label3
			//
			this.Label3.AutoSize = true;
			this.Label3.Location = new System.Drawing.Point(13, 77);
			this.Label3.Name = "Label3";
			this.Label3.Size = new System.Drawing.Size(93, 20);
			this.Label3.TabIndex = 10;
			this.Label3.Text = "Base Positions";
			this.Label3.UseCompatibleTextRendering = true;
			//
			//Label4
			//
			this.Label4.AutoSize = true;
			this.Label4.Location = new System.Drawing.Point(120, 76);
			this.Label4.Name = "Label4";
			this.Label4.Size = new System.Drawing.Size(12, 20);
			this.Label4.TabIndex = 12;
			this.Label4.Text = "x";
			this.Label4.UseCompatibleTextRendering = true;
			//
			//Label5
			//
			this.Label5.AutoSize = true;
			this.Label5.Location = new System.Drawing.Point(172, 76);
			this.Label5.Name = "Label5";
			this.Label5.Size = new System.Drawing.Size(12, 20);
			this.Label5.TabIndex = 13;
			this.Label5.Text = "y";
			this.Label5.UseCompatibleTextRendering = true;
			//
			//rdoPlayer2
			//
			this.rdoPlayer2.AutoSize = true;
			this.rdoPlayer2.Location = new System.Drawing.Point(28, 126);
			this.rdoPlayer2.Name = "rdoPlayer2";
			this.rdoPlayer2.Size = new System.Drawing.Size(75, 21);
			this.rdoPlayer2.TabIndex = 54;
			this.rdoPlayer2.Text = "Player 2";
			this.rdoPlayer2.UseCompatibleTextRendering = true;
			this.rdoPlayer2.UseVisualStyleBackColor = true;
			//
			//txt1y
			//
			this.txt1y.Location = new System.Drawing.Point(175, 96);
			this.txt1y.Name = "txt1y";
			this.txt1y.Size = new System.Drawing.Size(46, 22);
			this.txt1y.TabIndex = 5;
			this.txt1y.Text = "0";
			this.txt1y.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			//
			//txt2y
			//
			this.txt2y.Location = new System.Drawing.Point(175, 124);
			this.txt2y.Name = "txt2y";
			this.txt2y.Size = new System.Drawing.Size(46, 22);
			this.txt2y.TabIndex = 8;
			this.txt2y.Text = "999";
			this.txt2y.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			//
			//txt2x
			//
			this.txt2x.Location = new System.Drawing.Point(123, 125);
			this.txt2x.Name = "txt2x";
			this.txt2x.Size = new System.Drawing.Size(46, 22);
			this.txt2x.TabIndex = 7;
			this.txt2x.Text = "999";
			this.txt2x.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			//
			//txt3y
			//
			this.txt3y.Location = new System.Drawing.Point(175, 152);
			this.txt3y.Name = "txt3y";
			this.txt3y.Size = new System.Drawing.Size(46, 22);
			this.txt3y.TabIndex = 11;
			this.txt3y.Text = "0";
			this.txt3y.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			//
			//txt3x
			//
			this.txt3x.Location = new System.Drawing.Point(123, 153);
			this.txt3x.Name = "txt3x";
			this.txt3x.Size = new System.Drawing.Size(46, 22);
			this.txt3x.TabIndex = 10;
			this.txt3x.Text = "999";
			this.txt3x.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			//
			//rdoPlayer3
			//
			this.rdoPlayer3.AutoSize = true;
			this.rdoPlayer3.Location = new System.Drawing.Point(28, 154);
			this.rdoPlayer3.Name = "rdoPlayer3";
			this.rdoPlayer3.Size = new System.Drawing.Size(75, 21);
			this.rdoPlayer3.TabIndex = 55;
			this.rdoPlayer3.Text = "Player 3";
			this.rdoPlayer3.UseCompatibleTextRendering = true;
			this.rdoPlayer3.UseVisualStyleBackColor = true;
			//
			//txt4y
			//
			this.txt4y.Location = new System.Drawing.Point(175, 180);
			this.txt4y.Name = "txt4y";
			this.txt4y.Size = new System.Drawing.Size(46, 22);
			this.txt4y.TabIndex = 14;
			this.txt4y.Text = "999";
			this.txt4y.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			//
			//txt4x
			//
			this.txt4x.Location = new System.Drawing.Point(123, 181);
			this.txt4x.Name = "txt4x";
			this.txt4x.Size = new System.Drawing.Size(46, 22);
			this.txt4x.TabIndex = 13;
			this.txt4x.Text = "0";
			this.txt4x.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			//
			//rdoPlayer4
			//
			this.rdoPlayer4.AutoSize = true;
			this.rdoPlayer4.Checked = true;
			this.rdoPlayer4.Location = new System.Drawing.Point(28, 182);
			this.rdoPlayer4.Name = "rdoPlayer4";
			this.rdoPlayer4.Size = new System.Drawing.Size(75, 21);
			this.rdoPlayer4.TabIndex = 56;
			this.rdoPlayer4.TabStop = true;
			this.rdoPlayer4.Text = "Player 4";
			this.rdoPlayer4.UseCompatibleTextRendering = true;
			this.rdoPlayer4.UseVisualStyleBackColor = true;
			//
			//txt5y
			//
			this.txt5y.Location = new System.Drawing.Point(175, 208);
			this.txt5y.Name = "txt5y";
			this.txt5y.Size = new System.Drawing.Size(46, 22);
			this.txt5y.TabIndex = 17;
			this.txt5y.Text = "y";
			this.txt5y.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			//
			//txt5x
			//
			this.txt5x.Location = new System.Drawing.Point(123, 209);
			this.txt5x.Name = "txt5x";
			this.txt5x.Size = new System.Drawing.Size(46, 22);
			this.txt5x.TabIndex = 16;
			this.txt5x.Text = "x";
			this.txt5x.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			//
			//rdoPlayer5
			//
			this.rdoPlayer5.AutoSize = true;
			this.rdoPlayer5.Location = new System.Drawing.Point(28, 210);
			this.rdoPlayer5.Name = "rdoPlayer5";
			this.rdoPlayer5.Size = new System.Drawing.Size(75, 21);
			this.rdoPlayer5.TabIndex = 57;
			this.rdoPlayer5.Text = "Player 5";
			this.rdoPlayer5.UseCompatibleTextRendering = true;
			this.rdoPlayer5.UseVisualStyleBackColor = true;
			//
			//txt6y
			//
			this.txt6y.Location = new System.Drawing.Point(175, 236);
			this.txt6y.Name = "txt6y";
			this.txt6y.Size = new System.Drawing.Size(46, 22);
			this.txt6y.TabIndex = 20;
			this.txt6y.Text = "y";
			this.txt6y.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			//
			//txt6x
			//
			this.txt6x.Location = new System.Drawing.Point(123, 237);
			this.txt6x.Name = "txt6x";
			this.txt6x.Size = new System.Drawing.Size(46, 22);
			this.txt6x.TabIndex = 19;
			this.txt6x.Text = "x";
			this.txt6x.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			//
			//rdoPlayer6
			//
			this.rdoPlayer6.AutoSize = true;
			this.rdoPlayer6.Location = new System.Drawing.Point(28, 238);
			this.rdoPlayer6.Name = "rdoPlayer6";
			this.rdoPlayer6.Size = new System.Drawing.Size(75, 21);
			this.rdoPlayer6.TabIndex = 58;
			this.rdoPlayer6.Text = "Player 6";
			this.rdoPlayer6.UseCompatibleTextRendering = true;
			this.rdoPlayer6.UseVisualStyleBackColor = true;
			//
			//txt7y
			//
			this.txt7y.Location = new System.Drawing.Point(175, 264);
			this.txt7y.Name = "txt7y";
			this.txt7y.Size = new System.Drawing.Size(46, 22);
			this.txt7y.TabIndex = 23;
			this.txt7y.Text = "y";
			this.txt7y.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			//
			//txt7x
			//
			this.txt7x.Location = new System.Drawing.Point(123, 265);
			this.txt7x.Name = "txt7x";
			this.txt7x.Size = new System.Drawing.Size(46, 22);
			this.txt7x.TabIndex = 22;
			this.txt7x.Text = "x";
			this.txt7x.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			//
			//rdoPlayer7
			//
			this.rdoPlayer7.AutoSize = true;
			this.rdoPlayer7.Location = new System.Drawing.Point(28, 266);
			this.rdoPlayer7.Name = "rdoPlayer7";
			this.rdoPlayer7.Size = new System.Drawing.Size(75, 21);
			this.rdoPlayer7.TabIndex = 59;
			this.rdoPlayer7.Text = "Player 7";
			this.rdoPlayer7.UseCompatibleTextRendering = true;
			this.rdoPlayer7.UseVisualStyleBackColor = true;
			//
			//txt8y
			//
			this.txt8y.Location = new System.Drawing.Point(175, 292);
			this.txt8y.Name = "txt8y";
			this.txt8y.Size = new System.Drawing.Size(46, 22);
			this.txt8y.TabIndex = 26;
			this.txt8y.Text = "y";
			this.txt8y.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			//
			//txt8x
			//
			this.txt8x.Location = new System.Drawing.Point(123, 293);
			this.txt8x.Name = "txt8x";
			this.txt8x.Size = new System.Drawing.Size(46, 22);
			this.txt8x.TabIndex = 25;
			this.txt8x.Text = "x";
			this.txt8x.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			//
			//rdoPlayer8
			//
			this.rdoPlayer8.AutoSize = true;
			this.rdoPlayer8.Location = new System.Drawing.Point(28, 294);
			this.rdoPlayer8.Name = "rdoPlayer8";
			this.rdoPlayer8.Size = new System.Drawing.Size(75, 21);
			this.rdoPlayer8.TabIndex = 60;
			this.rdoPlayer8.Text = "Player 8";
			this.rdoPlayer8.UseCompatibleTextRendering = true;
			this.rdoPlayer8.UseVisualStyleBackColor = true;
			//
			//Label7
			//
			this.Label7.Location = new System.Drawing.Point(253, 23);
			this.Label7.Name = "Label7";
			this.Label7.Size = new System.Drawing.Size(115, 20);
			this.Label7.TabIndex = 37;
			this.Label7.Text = "Height Levels";
			this.Label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.Label7.UseCompatibleTextRendering = true;
			//
			//txtLevels
			//
			this.txtLevels.Location = new System.Drawing.Point(374, 24);
			this.txtLevels.Name = "txtLevels";
			this.txtLevels.Size = new System.Drawing.Size(46, 22);
			this.txtLevels.TabIndex = 28;
			this.txtLevels.Text = "4";
			this.txtLevels.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			//
			//txtLevelFrequency
			//
			this.txtLevelFrequency.Location = new System.Drawing.Point(374, 168);
			this.txtLevelFrequency.Name = "txtLevelFrequency";
			this.txtLevelFrequency.Size = new System.Drawing.Size(46, 22);
			this.txtLevelFrequency.TabIndex = 31;
			this.txtLevelFrequency.Text = "1";
			this.txtLevelFrequency.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			//
			//Label8
			//
			this.Label8.Location = new System.Drawing.Point(282, 167);
			this.Label8.Name = "Label8";
			this.Label8.Size = new System.Drawing.Size(86, 20);
			this.Label8.TabIndex = 39;
			this.Label8.Text = "Passages";
			this.Label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.Label8.UseCompatibleTextRendering = true;
			//
			//txtRampDistance
			//
			this.txtRampDistance.Location = new System.Drawing.Point(129, 41);
			this.txtRampDistance.Name = "txtRampDistance";
			this.txtRampDistance.Size = new System.Drawing.Size(46, 22);
			this.txtRampDistance.TabIndex = 36;
			this.txtRampDistance.Text = "80";
			this.txtRampDistance.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			//
			//Label9
			//
			this.Label9.Location = new System.Drawing.Point(0, 18);
			this.Label9.Name = "Label9";
			this.Label9.Size = new System.Drawing.Size(176, 20);
			this.Label9.TabIndex = 41;
			this.Label9.Text = "Ramp Distance At Base";
			this.Label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.Label9.UseCompatibleTextRendering = true;
			//
			//txtBaseOil
			//
			this.txtBaseOil.Location = new System.Drawing.Point(186, 25);
			this.txtBaseOil.Name = "txtBaseOil";
			this.txtBaseOil.Size = new System.Drawing.Size(46, 22);
			this.txtBaseOil.TabIndex = 38;
			this.txtBaseOil.Text = "4";
			this.txtBaseOil.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			//
			//Label10
			//
			this.Label10.Location = new System.Drawing.Point(74, 24);
			this.Label10.Name = "Label10";
			this.Label10.Size = new System.Drawing.Size(106, 20);
			this.Label10.TabIndex = 43;
			this.Label10.Text = "Oil In Base";
			this.Label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.Label10.UseCompatibleTextRendering = true;
			//
			//txtOilElsewhere
			//
			this.txtOilElsewhere.Location = new System.Drawing.Point(186, 53);
			this.txtOilElsewhere.Name = "txtOilElsewhere";
			this.txtOilElsewhere.Size = new System.Drawing.Size(46, 22);
			this.txtOilElsewhere.TabIndex = 39;
			this.txtOilElsewhere.Text = "40";
			this.txtOilElsewhere.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			//
			//Label11
			//
			this.Label11.Location = new System.Drawing.Point(61, 52);
			this.Label11.Name = "Label11";
			this.Label11.Size = new System.Drawing.Size(119, 20);
			this.Label11.TabIndex = 45;
			this.Label11.Text = "Oil Elsewhere";
			this.Label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.Label11.UseCompatibleTextRendering = true;
			//
			//txtOilClusterMin
			//
			this.txtOilClusterMin.Location = new System.Drawing.Point(186, 81);
			this.txtOilClusterMin.Name = "txtOilClusterMin";
			this.txtOilClusterMin.Size = new System.Drawing.Size(22, 22);
			this.txtOilClusterMin.TabIndex = 40;
			this.txtOilClusterMin.Text = "1";
			this.txtOilClusterMin.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			//
			//Label12
			//
			this.Label12.Location = new System.Drawing.Point(18, 80);
			this.Label12.Name = "Label12";
			this.Label12.Size = new System.Drawing.Size(114, 20);
			this.Label12.TabIndex = 47;
			this.Label12.Text = "Oil Cluster Size";
			this.Label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.Label12.UseCompatibleTextRendering = true;
			//
			//Label13
			//
			this.Label13.AutoSize = true;
			this.Label13.Location = new System.Drawing.Point(150, 84);
			this.Label13.Name = "Label13";
			this.Label13.Size = new System.Drawing.Size(26, 20);
			this.Label13.TabIndex = 49;
			this.Label13.Text = "Min";
			this.Label13.UseCompatibleTextRendering = true;
			//
			//Label14
			//
			this.Label14.AutoSize = true;
			this.Label14.Location = new System.Drawing.Point(223, 84);
			this.Label14.Name = "Label14";
			this.Label14.Size = new System.Drawing.Size(33, 17);
			this.Label14.TabIndex = 51;
			this.Label14.Text = "Max";
			//
			//txtOilClusterMax
			//
			this.txtOilClusterMax.Location = new System.Drawing.Point(259, 81);
			this.txtOilClusterMax.Name = "txtOilClusterMax";
			this.txtOilClusterMax.Size = new System.Drawing.Size(22, 22);
			this.txtOilClusterMax.TabIndex = 41;
			this.txtOilClusterMax.Text = "1";
			this.txtOilClusterMax.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			//
			//txtBaseLevel
			//
			this.txtBaseLevel.Location = new System.Drawing.Point(374, 50);
			this.txtBaseLevel.Name = "txtBaseLevel";
			this.txtBaseLevel.Size = new System.Drawing.Size(46, 22);
			this.txtBaseLevel.TabIndex = 29;
			this.txtBaseLevel.Text = "-1";
			this.txtBaseLevel.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			//
			//Label17
			//
			this.Label17.Location = new System.Drawing.Point(237, 49);
			this.Label17.Name = "Label17";
			this.Label17.Size = new System.Drawing.Size(131, 20);
			this.Label17.TabIndex = 58;
			this.Label17.Text = "Base Height Level";
			this.Label17.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.Label17.UseCompatibleTextRendering = true;
			//
			//txtOilDispersion
			//
			this.txtOilDispersion.Location = new System.Drawing.Point(186, 109);
			this.txtOilDispersion.Name = "txtOilDispersion";
			this.txtOilDispersion.Size = new System.Drawing.Size(46, 22);
			this.txtOilDispersion.TabIndex = 42;
			this.txtOilDispersion.Text = "100";
			this.txtOilDispersion.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			//
			//Label18
			//
			this.Label18.Location = new System.Drawing.Point(44, 108);
			this.Label18.Name = "Label18";
			this.Label18.Size = new System.Drawing.Size(136, 20);
			this.Label18.TabIndex = 60;
			this.Label18.Text = "Oil Dispersion";
			this.Label18.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.Label18.UseCompatibleTextRendering = true;
			//
			//txtFScatterChance
			//
			this.txtFScatterChance.Location = new System.Drawing.Point(186, 185);
			this.txtFScatterChance.Name = "txtFScatterChance";
			this.txtFScatterChance.Size = new System.Drawing.Size(46, 22);
			this.txtFScatterChance.TabIndex = 43;
			this.txtFScatterChance.Text = "9999";
			this.txtFScatterChance.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			//
			//Label19
			//
			this.Label19.Location = new System.Drawing.Point(18, 184);
			this.Label19.Name = "Label19";
			this.Label19.Size = new System.Drawing.Size(162, 20);
			this.Label19.TabIndex = 62;
			this.Label19.Text = "Scattered Features";
			this.Label19.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.Label19.UseCompatibleTextRendering = true;
			//
			//txtFClusterChance
			//
			this.txtFClusterChance.Location = new System.Drawing.Point(186, 240);
			this.txtFClusterChance.Name = "txtFClusterChance";
			this.txtFClusterChance.Size = new System.Drawing.Size(46, 22);
			this.txtFClusterChance.TabIndex = 44;
			this.txtFClusterChance.Text = "0";
			this.txtFClusterChance.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			//
			//Label20
			//
			this.Label20.Location = new System.Drawing.Point(-4, 239);
			this.Label20.Name = "Label20";
			this.Label20.Size = new System.Drawing.Size(184, 20);
			this.Label20.TabIndex = 64;
			this.Label20.Text = "Feature Cluster Chance %";
			this.Label20.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.Label20.UseCompatibleTextRendering = true;
			//
			//txtFClusterMin
			//
			this.txtFClusterMin.Location = new System.Drawing.Point(186, 292);
			this.txtFClusterMin.Name = "txtFClusterMin";
			this.txtFClusterMin.Size = new System.Drawing.Size(46, 22);
			this.txtFClusterMin.TabIndex = 45;
			this.txtFClusterMin.Text = "2";
			this.txtFClusterMin.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			//
			//Label21
			//
			this.Label21.Location = new System.Drawing.Point(44, 269);
			this.Label21.Name = "Label21";
			this.Label21.Size = new System.Drawing.Size(164, 20);
			this.Label21.TabIndex = 66;
			this.Label21.Text = "Feature Cluster Size";
			this.Label21.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.Label21.UseCompatibleTextRendering = true;
			//
			//txtFClusterMax
			//
			this.txtFClusterMax.Location = new System.Drawing.Point(186, 320);
			this.txtFClusterMax.Name = "txtFClusterMax";
			this.txtFClusterMax.Size = new System.Drawing.Size(46, 22);
			this.txtFClusterMax.TabIndex = 46;
			this.txtFClusterMax.Text = "5";
			this.txtFClusterMax.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			//
			//Label22
			//
			this.Label22.Location = new System.Drawing.Point(130, 291);
			this.Label22.Name = "Label22";
			this.Label22.Size = new System.Drawing.Size(50, 20);
			this.Label22.TabIndex = 68;
			this.Label22.Text = "Min";
			this.Label22.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.Label22.UseCompatibleTextRendering = true;
			//
			//txtTrucks
			//
			this.txtTrucks.Location = new System.Drawing.Point(398, 25);
			this.txtTrucks.Name = "txtTrucks";
			this.txtTrucks.Size = new System.Drawing.Size(46, 22);
			this.txtTrucks.TabIndex = 47;
			this.txtTrucks.Text = "2";
			this.txtTrucks.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			//
			//Label23
			//
			this.Label23.Location = new System.Drawing.Point(284, 24);
			this.Label23.Name = "Label23";
			this.Label23.Size = new System.Drawing.Size(108, 20);
			this.Label23.TabIndex = 70;
			this.Label23.Text = "Base Trucks";
			this.Label23.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.Label23.UseCompatibleTextRendering = true;
			//
			//Label24
			//
			this.Label24.AutoSize = true;
			this.Label24.Location = new System.Drawing.Point(24, 22);
			this.Label24.Name = "Label24";
			this.Label24.Size = new System.Drawing.Size(44, 20);
			this.Label24.TabIndex = 72;
			this.Label24.Text = "Tileset";
			this.Label24.UseCompatibleTextRendering = true;
			//
			//cboTileset
			//
			this.cboTileset.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboTileset.FormattingEnabled = true;
			this.cboTileset.Items.AddRange(new object[] {"Arizona", "Urban", "Rockies"});
			this.cboTileset.Location = new System.Drawing.Point(80, 19);
			this.cboTileset.Name = "cboTileset";
			this.cboTileset.Size = new System.Drawing.Size(149, 24);
			this.cboTileset.TabIndex = 27;
			//
			//txtFlatness
			//
			this.txtFlatness.Location = new System.Drawing.Point(374, 140);
			this.txtFlatness.Name = "txtFlatness";
			this.txtFlatness.Size = new System.Drawing.Size(46, 22);
			this.txtFlatness.TabIndex = 30;
			this.txtFlatness.Text = "0";
			this.txtFlatness.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			//
			//Label25
			//
			this.Label25.Location = new System.Drawing.Point(312, 139);
			this.Label25.Name = "Label25";
			this.Label25.Size = new System.Drawing.Size(56, 20);
			this.Label25.TabIndex = 74;
			this.Label25.Text = "Flats";
			this.Label25.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.Label25.UseCompatibleTextRendering = true;
			//
			//txtWaterQuantity
			//
			this.txtWaterQuantity.Location = new System.Drawing.Point(374, 238);
			this.txtWaterQuantity.Name = "txtWaterQuantity";
			this.txtWaterQuantity.Size = new System.Drawing.Size(46, 22);
			this.txtWaterQuantity.TabIndex = 35;
			this.txtWaterQuantity.Text = "0";
			this.txtWaterQuantity.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			//
			//Label26
			//
			this.Label26.Location = new System.Drawing.Point(237, 237);
			this.Label26.Name = "Label26";
			this.Label26.Size = new System.Drawing.Size(131, 20);
			this.Label26.TabIndex = 78;
			this.Label26.Text = "Water Spawns";
			this.Label26.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.Label26.UseCompatibleTextRendering = true;
			//
			//Label27
			//
			this.Label27.Location = new System.Drawing.Point(121, 319);
			this.Label27.Name = "Label27";
			this.Label27.Size = new System.Drawing.Size(59, 20);
			this.Label27.TabIndex = 80;
			this.Label27.Text = "Max";
			this.Label27.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.Label27.UseCompatibleTextRendering = true;
			//
			//txtVariation
			//
			this.txtVariation.Location = new System.Drawing.Point(374, 196);
			this.txtVariation.Name = "txtVariation";
			this.txtVariation.Size = new System.Drawing.Size(46, 22);
			this.txtVariation.TabIndex = 32;
			this.txtVariation.Text = "1";
			this.txtVariation.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			//
			//Label28
			//
			this.Label28.Location = new System.Drawing.Point(276, 196);
			this.Label28.Name = "Label28";
			this.Label28.Size = new System.Drawing.Size(92, 20);
			this.Label28.TabIndex = 81;
			this.Label28.Text = "Variation";
			this.Label28.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.Label28.UseCompatibleTextRendering = true;
			//
			//rdoPlayer1
			//
			this.rdoPlayer1.AutoSize = true;
			this.rdoPlayer1.Location = new System.Drawing.Point(28, 100);
			this.rdoPlayer1.Name = "rdoPlayer1";
			this.rdoPlayer1.Size = new System.Drawing.Size(75, 21);
			this.rdoPlayer1.TabIndex = 53;
			this.rdoPlayer1.Text = "Player 1";
			this.rdoPlayer1.UseCompatibleTextRendering = true;
			this.rdoPlayer1.UseVisualStyleBackColor = true;
			//
			//cboSymmetry
			//
			this.cboSymmetry.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboSymmetry.FormattingEnabled = true;
			this.cboSymmetry.Items.AddRange(new object[] {"None", "Horizontal Rotation", "Vertical Rotation", "Horizontal Flip", "Vertical Flip", "Quarters Rotation", "Quarters Flip"});
			this.cboSymmetry.Location = new System.Drawing.Point(85, 7);
			this.cboSymmetry.Name = "cboSymmetry";
			this.cboSymmetry.Size = new System.Drawing.Size(149, 24);
			this.cboSymmetry.TabIndex = 0;
			//
			//Label6
			//
			this.Label6.AutoSize = true;
			this.Label6.Location = new System.Drawing.Point(9, 10);
			this.Label6.Name = "Label6";
			this.Label6.Size = new System.Drawing.Size(65, 20);
			this.Label6.TabIndex = 88;
			this.Label6.Text = "Symmetry";
			this.Label6.UseCompatibleTextRendering = true;
			//
			//txtOilAtATime
			//
			this.txtOilAtATime.Location = new System.Drawing.Point(186, 137);
			this.txtOilAtATime.Name = "txtOilAtATime";
			this.txtOilAtATime.Size = new System.Drawing.Size(46, 22);
			this.txtOilAtATime.TabIndex = 89;
			this.txtOilAtATime.Text = "1";
			this.txtOilAtATime.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			//
			//Label31
			//
			this.Label31.Location = new System.Drawing.Point(18, 136);
			this.Label31.Name = "Label31";
			this.Label31.Size = new System.Drawing.Size(162, 20);
			this.Label31.TabIndex = 90;
			this.Label31.Text = "Oil Clusters At A Time";
			this.Label31.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.Label31.UseCompatibleTextRendering = true;
			//
			//txtRampBase
			//
			this.txtRampBase.Location = new System.Drawing.Point(129, 96);
			this.txtRampBase.Name = "txtRampBase";
			this.txtRampBase.Size = new System.Drawing.Size(46, 22);
			this.txtRampBase.TabIndex = 93;
			this.txtRampBase.Text = "100";
			this.txtRampBase.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			//
			//Label33
			//
			this.Label33.Location = new System.Drawing.Point(-4, 73);
			this.Label33.Name = "Label33";
			this.Label33.Size = new System.Drawing.Size(179, 20);
			this.Label33.TabIndex = 94;
			this.Label33.Text = "Ramp Multiplier % Per 8";
			this.Label33.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.Label33.UseCompatibleTextRendering = true;
			//
			//txtConnectedWater
			//
			this.txtConnectedWater.Location = new System.Drawing.Point(374, 266);
			this.txtConnectedWater.Name = "txtConnectedWater";
			this.txtConnectedWater.Size = new System.Drawing.Size(46, 22);
			this.txtConnectedWater.TabIndex = 95;
			this.txtConnectedWater.Text = "0";
			this.txtConnectedWater.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			//
			//Label34
			//
			this.Label34.Location = new System.Drawing.Point(237, 265);
			this.Label34.Name = "Label34";
			this.Label34.Size = new System.Drawing.Size(131, 20);
			this.Label34.TabIndex = 96;
			this.Label34.Text = "Total Water";
			this.Label34.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.Label34.UseCompatibleTextRendering = true;
			//
			//txt9y
			//
			this.txt9y.Location = new System.Drawing.Point(175, 320);
			this.txt9y.Name = "txt9y";
			this.txt9y.Size = new System.Drawing.Size(46, 22);
			this.txt9y.TabIndex = 98;
			this.txt9y.Text = "y";
			this.txt9y.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			//
			//txt9x
			//
			this.txt9x.Location = new System.Drawing.Point(123, 321);
			this.txt9x.Name = "txt9x";
			this.txt9x.Size = new System.Drawing.Size(46, 22);
			this.txt9x.TabIndex = 97;
			this.txt9x.Text = "x";
			this.txt9x.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			//
			//rdoPlayer9
			//
			this.rdoPlayer9.AutoSize = true;
			this.rdoPlayer9.Location = new System.Drawing.Point(28, 322);
			this.rdoPlayer9.Name = "rdoPlayer9";
			this.rdoPlayer9.Size = new System.Drawing.Size(75, 21);
			this.rdoPlayer9.TabIndex = 99;
			this.rdoPlayer9.Text = "Player 9";
			this.rdoPlayer9.UseCompatibleTextRendering = true;
			this.rdoPlayer9.UseVisualStyleBackColor = true;
			//
			//txt10y
			//
			this.txt10y.Location = new System.Drawing.Point(175, 348);
			this.txt10y.Name = "txt10y";
			this.txt10y.Size = new System.Drawing.Size(46, 22);
			this.txt10y.TabIndex = 101;
			this.txt10y.Text = "y";
			this.txt10y.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			//
			//txt10x
			//
			this.txt10x.Location = new System.Drawing.Point(123, 349);
			this.txt10x.Name = "txt10x";
			this.txt10x.Size = new System.Drawing.Size(46, 22);
			this.txt10x.TabIndex = 100;
			this.txt10x.Text = "x";
			this.txt10x.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			//
			//rdoPlayer10
			//
			this.rdoPlayer10.AutoSize = true;
			this.rdoPlayer10.Location = new System.Drawing.Point(28, 350);
			this.rdoPlayer10.Name = "rdoPlayer10";
			this.rdoPlayer10.Size = new System.Drawing.Size(82, 21);
			this.rdoPlayer10.TabIndex = 102;
			this.rdoPlayer10.Text = "Player 10";
			this.rdoPlayer10.UseCompatibleTextRendering = true;
			this.rdoPlayer10.UseVisualStyleBackColor = true;
			//
			//cbxMasterTexture
			//
			this.cbxMasterTexture.AutoSize = true;
			this.cbxMasterTexture.Location = new System.Drawing.Point(45, 64);
			this.cbxMasterTexture.Name = "cbxMasterTexture";
			this.cbxMasterTexture.Size = new System.Drawing.Size(127, 21);
			this.cbxMasterTexture.TabIndex = 103;
			this.cbxMasterTexture.Text = "Master Texturing";
			this.cbxMasterTexture.UseCompatibleTextRendering = true;
			this.cbxMasterTexture.UseVisualStyleBackColor = true;
			//
			//txtBaseArea
			//
			this.txtBaseArea.Location = new System.Drawing.Point(374, 97);
			this.txtBaseArea.Name = "txtBaseArea";
			this.txtBaseArea.Size = new System.Drawing.Size(46, 22);
			this.txtBaseArea.TabIndex = 104;
			this.txtBaseArea.Text = "3";
			this.txtBaseArea.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			//
			//Label15
			//
			this.Label15.Location = new System.Drawing.Point(249, 97);
			this.Label15.Name = "Label15";
			this.Label15.Size = new System.Drawing.Size(119, 20);
			this.Label15.TabIndex = 105;
			this.Label15.Text = "Base Flat Area";
			this.Label15.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.Label15.UseCompatibleTextRendering = true;
			//
			//btnGenerateLayout
			//
			this.btnGenerateLayout.Location = new System.Drawing.Point(253, 338);
			this.btnGenerateLayout.Name = "btnGenerateLayout";
			this.btnGenerateLayout.Size = new System.Drawing.Size(146, 33);
			this.btnGenerateLayout.TabIndex = 106;
			this.btnGenerateLayout.Text = "Generate Layout";
			this.btnGenerateLayout.UseCompatibleTextRendering = true;
			this.btnGenerateLayout.UseVisualStyleBackColor = true;
			//
			//btnGenerateObjects
			//
			this.btnGenerateObjects.Location = new System.Drawing.Point(271, 348);
			this.btnGenerateObjects.Name = "btnGenerateObjects";
			this.btnGenerateObjects.Size = new System.Drawing.Size(146, 33);
			this.btnGenerateObjects.TabIndex = 107;
			this.btnGenerateObjects.Text = "Generate Objects";
			this.btnGenerateObjects.UseCompatibleTextRendering = true;
			this.btnGenerateObjects.UseVisualStyleBackColor = true;
			//
			//btnStop
			//
			this.btnStop.Location = new System.Drawing.Point(406, 338);
			this.btnStop.Margin = new System.Windows.Forms.Padding(4);
			this.btnStop.Name = "btnStop";
			this.btnStop.Size = new System.Drawing.Size(58, 33);
			this.btnStop.TabIndex = 51;
			this.btnStop.Text = "Stop";
			this.btnStop.UseCompatibleTextRendering = true;
			this.btnStop.UseVisualStyleBackColor = true;
			//
			//lstResult
			//
			this.lstResult.FormattingEnabled = true;
			this.lstResult.ItemHeight = 16;
			this.lstResult.Location = new System.Drawing.Point(12, 442);
			this.lstResult.Name = "lstResult";
			this.lstResult.Size = new System.Drawing.Size(501, 116);
			this.lstResult.TabIndex = 108;
			//
			//btnGenerateRamps
			//
			this.btnGenerateRamps.Location = new System.Drawing.Point(270, 343);
			this.btnGenerateRamps.Name = "btnGenerateRamps";
			this.btnGenerateRamps.Size = new System.Drawing.Size(146, 33);
			this.btnGenerateRamps.TabIndex = 109;
			this.btnGenerateRamps.Text = "Generate Ramps";
			this.btnGenerateRamps.UseCompatibleTextRendering = true;
			this.btnGenerateRamps.UseVisualStyleBackColor = true;
			//
			//TabControl1
			//
			this.TabControl1.Controls.Add(this.TabPage1);
			this.TabControl1.Controls.Add(this.TabPage2);
			this.TabControl1.Controls.Add(this.TabPage4);
			this.TabControl1.Controls.Add(this.TabPage3);
			this.TabControl1.Location = new System.Drawing.Point(12, 12);
			this.TabControl1.Name = "TabControl1";
			this.TabControl1.SelectedIndex = 0;
			this.TabControl1.Size = new System.Drawing.Size(501, 424);
			this.TabControl1.TabIndex = 110;
			//
			//TabPage1
			//
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
			this.TabPage1.Location = new System.Drawing.Point(4, 25);
			this.TabPage1.Name = "TabPage1";
			this.TabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.TabPage1.Size = new System.Drawing.Size(493, 395);
			this.TabPage1.TabIndex = 0;
			this.TabPage1.Text = "Layout";
			this.TabPage1.UseVisualStyleBackColor = true;
			//
			//TabPage2
			//
			this.TabPage2.Controls.Add(this.Label9);
			this.TabPage2.Controls.Add(this.btnGenerateRamps);
			this.TabPage2.Controls.Add(this.txtRampDistance);
			this.TabPage2.Controls.Add(this.Label33);
			this.TabPage2.Controls.Add(this.txtRampBase);
			this.TabPage2.Location = new System.Drawing.Point(4, 25);
			this.TabPage2.Name = "TabPage2";
			this.TabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.TabPage2.Size = new System.Drawing.Size(493, 395);
			this.TabPage2.TabIndex = 1;
			this.TabPage2.Text = "Ramps";
			this.TabPage2.UseVisualStyleBackColor = true;
			//
			//TabPage4
			//
			this.TabPage4.Controls.Add(this.btnGenerateTextures);
			this.TabPage4.Controls.Add(this.cboTileset);
			this.TabPage4.Controls.Add(this.Label24);
			this.TabPage4.Controls.Add(this.cbxMasterTexture);
			this.TabPage4.Location = new System.Drawing.Point(4, 25);
			this.TabPage4.Name = "TabPage4";
			this.TabPage4.Padding = new System.Windows.Forms.Padding(3);
			this.TabPage4.Size = new System.Drawing.Size(493, 395);
			this.TabPage4.TabIndex = 3;
			this.TabPage4.Text = "Textures";
			this.TabPage4.UseVisualStyleBackColor = true;
			//
			//btnGenerateTextures
			//
			this.btnGenerateTextures.Location = new System.Drawing.Point(253, 342);
			this.btnGenerateTextures.Name = "btnGenerateTextures";
			this.btnGenerateTextures.Size = new System.Drawing.Size(146, 33);
			this.btnGenerateTextures.TabIndex = 108;
			this.btnGenerateTextures.Text = "Generate Textures";
			this.btnGenerateTextures.UseCompatibleTextRendering = true;
			this.btnGenerateTextures.UseVisualStyleBackColor = true;
			//
			//TabPage3
			//
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
			this.TabPage3.Location = new System.Drawing.Point(4, 25);
			this.TabPage3.Name = "TabPage3";
			this.TabPage3.Size = new System.Drawing.Size(493, 395);
			this.TabPage3.TabIndex = 2;
			this.TabPage3.Text = "Objects";
			this.TabPage3.UseVisualStyleBackColor = true;
			//
			//Label16
			//
			this.Label16.Location = new System.Drawing.Point(-9, 211);
			this.Label16.Name = "Label16";
			this.Label16.Size = new System.Drawing.Size(189, 20);
			this.Label16.TabIndex = 109;
			this.Label16.Text = "Scattered Feature Spacing";
			this.Label16.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.Label16.UseCompatibleTextRendering = true;
			//
			//txtFScatterGap
			//
			this.txtFScatterGap.Location = new System.Drawing.Point(186, 212);
			this.txtFScatterGap.Name = "txtFScatterGap";
			this.txtFScatterGap.Size = new System.Drawing.Size(46, 22);
			this.txtFScatterGap.TabIndex = 108;
			this.txtFScatterGap.Text = "4";
			this.txtFScatterGap.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			//
			//frmGenerator
			//
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(527, 567);
			this.Controls.Add(this.TabControl1);
			this.Controls.Add(this.lstResult);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Margin = new System.Windows.Forms.Padding(4);
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
		public System.Windows.Forms.Label Label1;
		public System.Windows.Forms.TextBox txtWidth;
		public System.Windows.Forms.TextBox txtHeight;
		public System.Windows.Forms.Label Label2;
		public System.Windows.Forms.TextBox txt1x;
		public System.Windows.Forms.Label Label3;
		public System.Windows.Forms.Label Label4;
		public System.Windows.Forms.Label Label5;
		public System.Windows.Forms.RadioButton rdoPlayer2;
		public System.Windows.Forms.TextBox txt1y;
		public System.Windows.Forms.TextBox txt2y;
		public System.Windows.Forms.TextBox txt2x;
		public System.Windows.Forms.TextBox txt3y;
		public System.Windows.Forms.TextBox txt3x;
		public System.Windows.Forms.RadioButton rdoPlayer3;
		public System.Windows.Forms.TextBox txt4y;
		public System.Windows.Forms.TextBox txt4x;
		public System.Windows.Forms.RadioButton rdoPlayer4;
		public System.Windows.Forms.TextBox txt5y;
		public System.Windows.Forms.TextBox txt5x;
		public System.Windows.Forms.RadioButton rdoPlayer5;
		public System.Windows.Forms.TextBox txt6y;
		public System.Windows.Forms.TextBox txt6x;
		public System.Windows.Forms.RadioButton rdoPlayer6;
		public System.Windows.Forms.TextBox txt7y;
		public System.Windows.Forms.TextBox txt7x;
		public System.Windows.Forms.RadioButton rdoPlayer7;
		public System.Windows.Forms.TextBox txt8y;
		public System.Windows.Forms.TextBox txt8x;
		public System.Windows.Forms.RadioButton rdoPlayer8;
		public System.Windows.Forms.Label Label7;
		public System.Windows.Forms.TextBox txtLevels;
		public System.Windows.Forms.TextBox txtLevelFrequency;
		public System.Windows.Forms.Label Label8;
		public System.Windows.Forms.TextBox txtRampDistance;
		public System.Windows.Forms.Label Label9;
		public System.Windows.Forms.TextBox txtBaseOil;
		public System.Windows.Forms.Label Label10;
		public System.Windows.Forms.TextBox txtOilElsewhere;
		public System.Windows.Forms.Label Label11;
		public System.Windows.Forms.TextBox txtOilClusterMin;
		public System.Windows.Forms.Label Label12;
		public System.Windows.Forms.Label Label13;
		public System.Windows.Forms.Label Label14;
		public System.Windows.Forms.TextBox txtOilClusterMax;
		public System.Windows.Forms.TextBox txtBaseLevel;
		public System.Windows.Forms.Label Label17;
		public System.Windows.Forms.TextBox txtOilDispersion;
		public System.Windows.Forms.Label Label18;
		public System.Windows.Forms.TextBox txtFScatterChance;
		public System.Windows.Forms.Label Label19;
		public System.Windows.Forms.TextBox txtFClusterChance;
		public System.Windows.Forms.Label Label20;
		public System.Windows.Forms.TextBox txtFClusterMin;
		public System.Windows.Forms.Label Label21;
		public System.Windows.Forms.TextBox txtFClusterMax;
		public System.Windows.Forms.Label Label22;
		public System.Windows.Forms.TextBox txtTrucks;
		public System.Windows.Forms.Label Label23;
		public System.Windows.Forms.Label Label24;
		public System.Windows.Forms.ComboBox cboTileset;
		public System.Windows.Forms.TextBox txtFlatness;
		public System.Windows.Forms.Label Label25;
		public System.Windows.Forms.TextBox txtWaterQuantity;
		public System.Windows.Forms.Label Label26;
		public System.Windows.Forms.Label Label27;
		public System.Windows.Forms.TextBox txtVariation;
		public System.Windows.Forms.Label Label28;
		public System.Windows.Forms.RadioButton rdoPlayer1;
		public System.Windows.Forms.ComboBox cboSymmetry;
		public System.Windows.Forms.Label Label6;
		public System.Windows.Forms.TextBox txtOilAtATime;
		public System.Windows.Forms.Label Label31;
		public System.Windows.Forms.TextBox txtRampBase;
		public System.Windows.Forms.Label Label33;
		public System.Windows.Forms.TextBox txtConnectedWater;
		public System.Windows.Forms.Label Label34;
		public System.Windows.Forms.TextBox txt9y;
		public System.Windows.Forms.TextBox txt9x;
		public System.Windows.Forms.RadioButton rdoPlayer9;
		public System.Windows.Forms.TextBox txt10y;
		public System.Windows.Forms.TextBox txt10x;
		public System.Windows.Forms.RadioButton rdoPlayer10;
		public System.Windows.Forms.CheckBox cbxMasterTexture;
		public System.Windows.Forms.TextBox txtBaseArea;
		public System.Windows.Forms.Label Label15;
		internal System.Windows.Forms.Button btnGenerateLayout;
		internal System.Windows.Forms.Button btnGenerateObjects;
		public System.Windows.Forms.Button btnStop;
		internal System.Windows.Forms.ListBox lstResult;
		internal System.Windows.Forms.Button btnGenerateRamps;
		internal System.Windows.Forms.TabControl TabControl1;
		internal System.Windows.Forms.TabPage TabPage1;
		internal System.Windows.Forms.TabPage TabPage2;
		internal System.Windows.Forms.TabPage TabPage3;
		internal System.Windows.Forms.TabPage TabPage4;
		internal System.Windows.Forms.Button btnGenerateTextures;
		public System.Windows.Forms.Label Label16;
		public System.Windows.Forms.TextBox txtFScatterGap;
	}
}
