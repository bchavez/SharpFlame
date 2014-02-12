namespace SharpFlame
{
	partial class frmOptions : System.Windows.Forms.Form
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
			this.TabControl1 = new System.Windows.Forms.TabControl();
			this.TabPage3 = new System.Windows.Forms.TabPage();
			this.Label12 = new System.Windows.Forms.Label();
			this.cbxAskDirectories = new System.Windows.Forms.CheckBox();
			this.TableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.TabPage1 = new System.Windows.Forms.TabPage();
			this.GroupBox3 = new System.Windows.Forms.GroupBox();
			this.cbxPickerOrientation = new System.Windows.Forms.CheckBox();
			this.GroupBox8 = new System.Windows.Forms.GroupBox();
			this.Label13 = new System.Windows.Forms.Label();
			this.txtTexturesDepth = new System.Windows.Forms.TextBox();
			this.txtTexturesBPP = new System.Windows.Forms.TextBox();
			this.Label10 = new System.Windows.Forms.Label();
			this.txtMapDepth = new System.Windows.Forms.TextBox();
			this.txtMapBPP = new System.Windows.Forms.TextBox();
			this.Label8 = new System.Windows.Forms.Label();
			this.Label9 = new System.Windows.Forms.Label();
			this.cbxMipmapsHardware = new System.Windows.Forms.CheckBox();
			this.cbxMipmaps = new System.Windows.Forms.CheckBox();
			this.GroupBox7 = new System.Windows.Forms.GroupBox();
			this.txtFOV = new System.Windows.Forms.TextBox();
			this.Label4 = new System.Windows.Forms.Label();
			this.GroupBox6 = new System.Windows.Forms.GroupBox();
			this.cbxPointerDirect = new System.Windows.Forms.CheckBox();
			this.GroupBox5 = new System.Windows.Forms.GroupBox();
			this.Label6 = new System.Windows.Forms.Label();
			this.pnlMinimapSelectedObjectColour = new System.Windows.Forms.Panel();
			this.Label5 = new System.Windows.Forms.Label();
			this.pnlMinimapCliffColour = new System.Windows.Forms.Panel();
			this.txtMinimapSize = new System.Windows.Forms.TextBox();
			this.cbxMinimapTeamColourFeatures = new System.Windows.Forms.CheckBox();
			this.cbxMinimapObjectColours = new System.Windows.Forms.CheckBox();
			this.Label3 = new System.Windows.Forms.Label();
			this.GroupBox4 = new System.Windows.Forms.GroupBox();
			this.lblFont = new System.Windows.Forms.Label();
			this.btnFont = new System.Windows.Forms.Button();
			this.btnFont.Click += this.btnFont_Click;
			this.GroupBox2 = new System.Windows.Forms.GroupBox();
			this.txtAutosaveInterval = new System.Windows.Forms.TextBox();
			this.txtAutosaveChanges = new System.Windows.Forms.TextBox();
			this.btnAutosaveOpen = new System.Windows.Forms.Button();
			this.btnAutosaveOpen.Click += this.btnAutosaveOpen_Click;
			this.cbxAutosaveCompression = new System.Windows.Forms.CheckBox();
			this.Label2 = new System.Windows.Forms.Label();
			this.cbxAutosaveEnabled = new System.Windows.Forms.CheckBox();
			this.Label1 = new System.Windows.Forms.Label();
			this.GroupBox1 = new System.Windows.Forms.GroupBox();
			this.txtUndoSteps = new System.Windows.Forms.TextBox();
			this.Label11 = new System.Windows.Forms.Label();
			this.TabPage2 = new System.Windows.Forms.TabPage();
			this.btnKeyControlChangeDefault = new System.Windows.Forms.Button();
			this.btnKeyControlChangeDefault.Click += this.btnKeyControlChangeDefault_Click;
			this.Label7 = new System.Windows.Forms.Label();
			this.btnKeyControlChangeUnless = new System.Windows.Forms.Button();
			this.btnKeyControlChangeUnless.Click += this.btnKeyControlChangeUnless_Click;
			this.btnKeyControlChange = new System.Windows.Forms.Button();
			this.btnKeyControlChange.Click += this.btnKeyControlChange_Click;
			this.lstKeyboardControls = new System.Windows.Forms.ListBox();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnCancel.Click += this.btnCancel_Click;
			this.btnSave = new System.Windows.Forms.Button();
			this.btnSave.Click += this.btnSave_Click;
			this.TabControl1.SuspendLayout();
			this.TabPage3.SuspendLayout();
			this.TabPage1.SuspendLayout();
			this.GroupBox3.SuspendLayout();
			this.GroupBox8.SuspendLayout();
			this.GroupBox7.SuspendLayout();
			this.GroupBox6.SuspendLayout();
			this.GroupBox5.SuspendLayout();
			this.GroupBox4.SuspendLayout();
			this.GroupBox2.SuspendLayout();
			this.GroupBox1.SuspendLayout();
			this.TabPage2.SuspendLayout();
			this.SuspendLayout();
			//
			//TabControl1
			//
			this.TabControl1.Controls.Add(this.TabPage3);
			this.TabControl1.Controls.Add(this.TabPage1);
			this.TabControl1.Controls.Add(this.TabPage2);
			this.TabControl1.Location = new System.Drawing.Point(12, 12);
			this.TabControl1.Name = "TabControl1";
			this.TabControl1.SelectedIndex = 0;
			this.TabControl1.Size = new System.Drawing.Size(637, 398);
			this.TabControl1.TabIndex = 35;
			//
			//TabPage3
			//
			this.TabPage3.Controls.Add(this.Label12);
			this.TabPage3.Controls.Add(this.cbxAskDirectories);
			this.TabPage3.Controls.Add(this.TableLayoutPanel1);
			this.TabPage3.Location = new System.Drawing.Point(4, 25);
			this.TabPage3.Margin = new System.Windows.Forms.Padding(0);
			this.TabPage3.Name = "TabPage3";
			this.TabPage3.Size = new System.Drawing.Size(629, 369);
			this.TabPage3.TabIndex = 2;
			this.TabPage3.Text = "Directories";
			this.TabPage3.UseVisualStyleBackColor = true;
			//
			//Label12
			//
			this.Label12.AutoSize = true;
			this.Label12.Location = new System.Drawing.Point(302, 15);
			this.Label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.Label12.Name = "Label12";
			this.Label12.Size = new System.Drawing.Size(245, 20);
			this.Label12.TabIndex = 42;
			this.Label12.Text = "Options on this tab take effect on restart.";
			this.Label12.UseCompatibleTextRendering = true;
			//
			//cbxAskDirectories
			//
			this.cbxAskDirectories.AutoSize = true;
			this.cbxAskDirectories.Location = new System.Drawing.Point(24, 14);
			this.cbxAskDirectories.Margin = new System.Windows.Forms.Padding(4);
			this.cbxAskDirectories.Name = "cbxAskDirectories";
			this.cbxAskDirectories.Size = new System.Drawing.Size(225, 21);
			this.cbxAskDirectories.TabIndex = 39;
			this.cbxAskDirectories.Text = "Show options before loading data";
			this.cbxAskDirectories.UseCompatibleTextRendering = true;
			this.cbxAskDirectories.UseVisualStyleBackColor = true;
			//
			//TableLayoutPanel1
			//
			this.TableLayoutPanel1.ColumnCount = 1;
			this.TableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, (float) (100.0F)));
			this.TableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, (float) (20.0F)));
			this.TableLayoutPanel1.Location = new System.Drawing.Point(3, 42);
			this.TableLayoutPanel1.Name = "TableLayoutPanel1";
			this.TableLayoutPanel1.RowCount = 2;
			this.TableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, (float) (50.0F)));
			this.TableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, (float) (50.0F)));
			this.TableLayoutPanel1.Size = new System.Drawing.Size(623, 324);
			this.TableLayoutPanel1.TabIndex = 41;
			//
			//TabPage1
			//
			this.TabPage1.Controls.Add(this.GroupBox3);
			this.TabPage1.Controls.Add(this.GroupBox8);
			this.TabPage1.Controls.Add(this.GroupBox7);
			this.TabPage1.Controls.Add(this.GroupBox6);
			this.TabPage1.Controls.Add(this.GroupBox5);
			this.TabPage1.Controls.Add(this.GroupBox4);
			this.TabPage1.Controls.Add(this.GroupBox2);
			this.TabPage1.Controls.Add(this.GroupBox1);
			this.TabPage1.Location = new System.Drawing.Point(4, 25);
			this.TabPage1.Name = "TabPage1";
			this.TabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.TabPage1.Size = new System.Drawing.Size(629, 369);
			this.TabPage1.TabIndex = 0;
			this.TabPage1.Text = "General";
			this.TabPage1.UseVisualStyleBackColor = true;
			//
			//GroupBox3
			//
			this.GroupBox3.Controls.Add(this.cbxPickerOrientation);
			this.GroupBox3.Location = new System.Drawing.Point(316, 288);
			this.GroupBox3.Name = "GroupBox3";
			this.GroupBox3.Size = new System.Drawing.Size(304, 54);
			this.GroupBox3.TabIndex = 45;
			this.GroupBox3.TabStop = false;
			this.GroupBox3.Text = "Picker";
			this.GroupBox3.UseCompatibleTextRendering = true;
			//
			//cbxPickerOrientation
			//
			this.cbxPickerOrientation.AutoSize = true;
			this.cbxPickerOrientation.Location = new System.Drawing.Point(8, 22);
			this.cbxPickerOrientation.Margin = new System.Windows.Forms.Padding(4);
			this.cbxPickerOrientation.Name = "cbxPickerOrientation";
			this.cbxPickerOrientation.Size = new System.Drawing.Size(192, 21);
			this.cbxPickerOrientation.TabIndex = 51;
			this.cbxPickerOrientation.Text = "Capture texture orientations";
			this.cbxPickerOrientation.UseCompatibleTextRendering = true;
			this.cbxPickerOrientation.UseVisualStyleBackColor = true;
			//
			//GroupBox8
			//
			this.GroupBox8.Controls.Add(this.Label13);
			this.GroupBox8.Controls.Add(this.txtTexturesDepth);
			this.GroupBox8.Controls.Add(this.txtTexturesBPP);
			this.GroupBox8.Controls.Add(this.Label10);
			this.GroupBox8.Controls.Add(this.txtMapDepth);
			this.GroupBox8.Controls.Add(this.txtMapBPP);
			this.GroupBox8.Controls.Add(this.Label8);
			this.GroupBox8.Controls.Add(this.Label9);
			this.GroupBox8.Controls.Add(this.cbxMipmapsHardware);
			this.GroupBox8.Controls.Add(this.cbxMipmaps);
			this.GroupBox8.Location = new System.Drawing.Point(6, 244);
			this.GroupBox8.Name = "GroupBox8";
			this.GroupBox8.Size = new System.Drawing.Size(304, 119);
			this.GroupBox8.TabIndex = 45;
			this.GroupBox8.TabStop = false;
			this.GroupBox8.Text = "Graphics";
			this.GroupBox8.UseCompatibleTextRendering = true;
			//
			//Label13
			//
			this.Label13.AutoSize = true;
			this.Label13.Location = new System.Drawing.Point(7, 96);
			this.Label13.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.Label13.Name = "Label13";
			this.Label13.Size = new System.Drawing.Size(90, 20);
			this.Label13.TabIndex = 50;
			this.Label13.Text = "Textures View";
			this.Label13.UseCompatibleTextRendering = true;
			//
			//txtTexturesDepth
			//
			this.txtTexturesDepth.Location = new System.Drawing.Point(180, 93);
			this.txtTexturesDepth.Margin = new System.Windows.Forms.Padding(4);
			this.txtTexturesDepth.Name = "txtTexturesDepth";
			this.txtTexturesDepth.Size = new System.Drawing.Size(61, 22);
			this.txtTexturesDepth.TabIndex = 49;
			//
			//txtTexturesBPP
			//
			this.txtTexturesBPP.Location = new System.Drawing.Point(105, 93);
			this.txtTexturesBPP.Margin = new System.Windows.Forms.Padding(4);
			this.txtTexturesBPP.Name = "txtTexturesBPP";
			this.txtTexturesBPP.Size = new System.Drawing.Size(61, 22);
			this.txtTexturesBPP.TabIndex = 48;
			//
			//Label10
			//
			this.Label10.AutoSize = true;
			this.Label10.Location = new System.Drawing.Point(33, 67);
			this.Label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.Label10.Name = "Label10";
			this.Label10.Size = new System.Drawing.Size(64, 20);
			this.Label10.TabIndex = 46;
			this.Label10.Text = "Map View";
			this.Label10.UseCompatibleTextRendering = true;
			//
			//txtMapDepth
			//
			this.txtMapDepth.Location = new System.Drawing.Point(180, 67);
			this.txtMapDepth.Margin = new System.Windows.Forms.Padding(4);
			this.txtMapDepth.Name = "txtMapDepth";
			this.txtMapDepth.Size = new System.Drawing.Size(61, 22);
			this.txtMapDepth.TabIndex = 44;
			//
			//txtMapBPP
			//
			this.txtMapBPP.Location = new System.Drawing.Point(105, 67);
			this.txtMapBPP.Margin = new System.Windows.Forms.Padding(4);
			this.txtMapBPP.Name = "txtMapBPP";
			this.txtMapBPP.Size = new System.Drawing.Size(61, 22);
			this.txtMapBPP.TabIndex = 42;
			//
			//Label8
			//
			this.Label8.AutoSize = true;
			this.Label8.Location = new System.Drawing.Point(96, 47);
			this.Label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.Label8.Name = "Label8";
			this.Label8.Size = new System.Drawing.Size(70, 20);
			this.Label8.TabIndex = 45;
			this.Label8.Text = "Colour Bits";
			this.Label8.UseCompatibleTextRendering = true;
			//
			//Label9
			//
			this.Label9.AutoSize = true;
			this.Label9.Location = new System.Drawing.Point(174, 47);
			this.Label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.Label9.Name = "Label9";
			this.Label9.Size = new System.Drawing.Size(67, 20);
			this.Label9.TabIndex = 43;
			this.Label9.Text = "Depth Bits";
			this.Label9.UseCompatibleTextRendering = true;
			//
			//cbxMipmapsHardware
			//
			this.cbxMipmapsHardware.AutoSize = true;
			this.cbxMipmapsHardware.Location = new System.Drawing.Point(169, 22);
			this.cbxMipmapsHardware.Margin = new System.Windows.Forms.Padding(4);
			this.cbxMipmapsHardware.Name = "cbxMipmapsHardware";
			this.cbxMipmapsHardware.Size = new System.Drawing.Size(112, 21);
			this.cbxMipmapsHardware.TabIndex = 41;
			this.cbxMipmapsHardware.Text = "Use Hardware";
			this.cbxMipmapsHardware.UseCompatibleTextRendering = true;
			this.cbxMipmapsHardware.UseVisualStyleBackColor = true;
			//
			//cbxMipmaps
			//
			this.cbxMipmaps.AutoSize = true;
			this.cbxMipmaps.Location = new System.Drawing.Point(8, 22);
			this.cbxMipmaps.Margin = new System.Windows.Forms.Padding(4);
			this.cbxMipmaps.Name = "cbxMipmaps";
			this.cbxMipmaps.Size = new System.Drawing.Size(141, 21);
			this.cbxMipmaps.TabIndex = 40;
			this.cbxMipmaps.Text = "Generate mipmaps";
			this.cbxMipmaps.UseCompatibleTextRendering = true;
			this.cbxMipmaps.UseVisualStyleBackColor = true;
			//
			//GroupBox7
			//
			this.GroupBox7.Controls.Add(this.txtFOV);
			this.GroupBox7.Controls.Add(this.Label4);
			this.GroupBox7.Location = new System.Drawing.Point(316, 232);
			this.GroupBox7.Name = "GroupBox7";
			this.GroupBox7.Size = new System.Drawing.Size(304, 50);
			this.GroupBox7.TabIndex = 44;
			this.GroupBox7.TabStop = false;
			this.GroupBox7.Text = "Field Of View";
			this.GroupBox7.UseCompatibleTextRendering = true;
			//
			//txtFOV
			//
			this.txtFOV.Location = new System.Drawing.Point(159, 15);
			this.txtFOV.Margin = new System.Windows.Forms.Padding(4);
			this.txtFOV.Name = "txtFOV";
			this.txtFOV.Size = new System.Drawing.Size(138, 22);
			this.txtFOV.TabIndex = 25;
			//
			//Label4
			//
			this.Label4.AutoSize = true;
			this.Label4.Location = new System.Drawing.Point(8, 18);
			this.Label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.Label4.Name = "Label4";
			this.Label4.Size = new System.Drawing.Size(105, 20);
			this.Label4.TabIndex = 26;
			this.Label4.Text = "Default Multiplier";
			this.Label4.UseCompatibleTextRendering = true;
			//
			//GroupBox6
			//
			this.GroupBox6.Controls.Add(this.cbxPointerDirect);
			this.GroupBox6.Location = new System.Drawing.Point(316, 176);
			this.GroupBox6.Name = "GroupBox6";
			this.GroupBox6.Size = new System.Drawing.Size(304, 50);
			this.GroupBox6.TabIndex = 43;
			this.GroupBox6.TabStop = false;
			this.GroupBox6.Text = "Pointer";
			this.GroupBox6.UseCompatibleTextRendering = true;
			//
			//cbxPointerDirect
			//
			this.cbxPointerDirect.AutoSize = true;
			this.cbxPointerDirect.Location = new System.Drawing.Point(7, 22);
			this.cbxPointerDirect.Margin = new System.Windows.Forms.Padding(4);
			this.cbxPointerDirect.Name = "cbxPointerDirect";
			this.cbxPointerDirect.Size = new System.Drawing.Size(62, 21);
			this.cbxPointerDirect.TabIndex = 40;
			this.cbxPointerDirect.Text = "Direct";
			this.cbxPointerDirect.UseCompatibleTextRendering = true;
			this.cbxPointerDirect.UseVisualStyleBackColor = true;
			//
			//GroupBox5
			//
			this.GroupBox5.Controls.Add(this.Label6);
			this.GroupBox5.Controls.Add(this.pnlMinimapSelectedObjectColour);
			this.GroupBox5.Controls.Add(this.Label5);
			this.GroupBox5.Controls.Add(this.pnlMinimapCliffColour);
			this.GroupBox5.Controls.Add(this.txtMinimapSize);
			this.GroupBox5.Controls.Add(this.cbxMinimapTeamColourFeatures);
			this.GroupBox5.Controls.Add(this.cbxMinimapObjectColours);
			this.GroupBox5.Controls.Add(this.Label3);
			this.GroupBox5.Location = new System.Drawing.Point(316, 7);
			this.GroupBox5.Name = "GroupBox5";
			this.GroupBox5.Size = new System.Drawing.Size(304, 163);
			this.GroupBox5.TabIndex = 42;
			this.GroupBox5.TabStop = false;
			this.GroupBox5.Text = "Minimap";
			this.GroupBox5.UseCompatibleTextRendering = true;
			//
			//Label6
			//
			this.Label6.AutoSize = true;
			this.Label6.Location = new System.Drawing.Point(8, 100);
			this.Label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.Label6.Name = "Label6";
			this.Label6.Size = new System.Drawing.Size(99, 20);
			this.Label6.TabIndex = 45;
			this.Label6.Text = "Object Highlight";
			this.Label6.UseCompatibleTextRendering = true;
			//
			//pnlMinimapSelectedObjectColour
			//
			this.pnlMinimapSelectedObjectColour.Location = new System.Drawing.Point(132, 100);
			this.pnlMinimapSelectedObjectColour.Name = "pnlMinimapSelectedObjectColour";
			this.pnlMinimapSelectedObjectColour.Size = new System.Drawing.Size(164, 29);
			this.pnlMinimapSelectedObjectColour.TabIndex = 44;
			//
			//Label5
			//
			this.Label5.AutoSize = true;
			this.Label5.Location = new System.Drawing.Point(8, 67);
			this.Label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.Label5.Name = "Label5";
			this.Label5.Size = new System.Drawing.Size(71, 20);
			this.Label5.TabIndex = 43;
			this.Label5.Text = "Cliff Colour";
			this.Label5.UseCompatibleTextRendering = true;
			//
			//pnlMinimapCliffColour
			//
			this.pnlMinimapCliffColour.Location = new System.Drawing.Point(132, 67);
			this.pnlMinimapCliffColour.Name = "pnlMinimapCliffColour";
			this.pnlMinimapCliffColour.Size = new System.Drawing.Size(164, 29);
			this.pnlMinimapCliffColour.TabIndex = 42;
			//
			//txtMinimapSize
			//
			this.txtMinimapSize.Location = new System.Drawing.Point(159, 15);
			this.txtMinimapSize.Margin = new System.Windows.Forms.Padding(4);
			this.txtMinimapSize.Name = "txtMinimapSize";
			this.txtMinimapSize.Size = new System.Drawing.Size(61, 22);
			this.txtMinimapSize.TabIndex = 25;
			//
			//cbxMinimapTeamColourFeatures
			//
			this.cbxMinimapTeamColourFeatures.AutoSize = true;
			this.cbxMinimapTeamColourFeatures.Location = new System.Drawing.Point(147, 42);
			this.cbxMinimapTeamColourFeatures.Margin = new System.Windows.Forms.Padding(4);
			this.cbxMinimapTeamColourFeatures.Name = "cbxMinimapTeamColourFeatures";
			this.cbxMinimapTeamColourFeatures.Size = new System.Drawing.Size(139, 21);
			this.cbxMinimapTeamColourFeatures.TabIndex = 41;
			this.cbxMinimapTeamColourFeatures.Text = "Except for features";
			this.cbxMinimapTeamColourFeatures.UseCompatibleTextRendering = true;
			this.cbxMinimapTeamColourFeatures.UseVisualStyleBackColor = true;
			//
			//cbxMinimapObjectColours
			//
			this.cbxMinimapObjectColours.AutoSize = true;
			this.cbxMinimapObjectColours.Location = new System.Drawing.Point(8, 42);
			this.cbxMinimapObjectColours.Margin = new System.Windows.Forms.Padding(4);
			this.cbxMinimapObjectColours.Name = "cbxMinimapObjectColours";
			this.cbxMinimapObjectColours.Size = new System.Drawing.Size(131, 21);
			this.cbxMinimapObjectColours.TabIndex = 40;
			this.cbxMinimapObjectColours.Text = "Use team colours";
			this.cbxMinimapObjectColours.UseCompatibleTextRendering = true;
			this.cbxMinimapObjectColours.UseVisualStyleBackColor = true;
			//
			//Label3
			//
			this.Label3.AutoSize = true;
			this.Label3.Location = new System.Drawing.Point(8, 18);
			this.Label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.Label3.Name = "Label3";
			this.Label3.Size = new System.Drawing.Size(31, 20);
			this.Label3.TabIndex = 26;
			this.Label3.Text = "Size";
			this.Label3.UseCompatibleTextRendering = true;
			//
			//GroupBox4
			//
			this.GroupBox4.Controls.Add(this.lblFont);
			this.GroupBox4.Controls.Add(this.btnFont);
			this.GroupBox4.Location = new System.Drawing.Point(6, 176);
			this.GroupBox4.Name = "GroupBox4";
			this.GroupBox4.Size = new System.Drawing.Size(304, 62);
			this.GroupBox4.TabIndex = 41;
			this.GroupBox4.TabStop = false;
			this.GroupBox4.Text = "Display Font";
			this.GroupBox4.UseCompatibleTextRendering = true;
			//
			//lblFont
			//
			this.lblFont.Location = new System.Drawing.Point(8, 27);
			this.lblFont.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lblFont.Name = "lblFont";
			this.lblFont.Size = new System.Drawing.Size(182, 29);
			this.lblFont.TabIndex = 39;
			this.lblFont.Text = "Current font";
			this.lblFont.UseCompatibleTextRendering = true;
			//
			//btnFont
			//
			this.btnFont.Location = new System.Drawing.Point(208, 21);
			this.btnFont.Name = "btnFont";
			this.btnFont.Size = new System.Drawing.Size(89, 29);
			this.btnFont.TabIndex = 38;
			this.btnFont.Text = "Select";
			this.btnFont.UseCompatibleTextRendering = true;
			this.btnFont.UseVisualStyleBackColor = true;
			//
			//GroupBox2
			//
			this.GroupBox2.Controls.Add(this.txtAutosaveInterval);
			this.GroupBox2.Controls.Add(this.txtAutosaveChanges);
			this.GroupBox2.Controls.Add(this.btnAutosaveOpen);
			this.GroupBox2.Controls.Add(this.cbxAutosaveCompression);
			this.GroupBox2.Controls.Add(this.Label2);
			this.GroupBox2.Controls.Add(this.cbxAutosaveEnabled);
			this.GroupBox2.Controls.Add(this.Label1);
			this.GroupBox2.Location = new System.Drawing.Point(6, 63);
			this.GroupBox2.Name = "GroupBox2";
			this.GroupBox2.Size = new System.Drawing.Size(304, 107);
			this.GroupBox2.TabIndex = 37;
			this.GroupBox2.TabStop = false;
			this.GroupBox2.Text = "Autosave";
			this.GroupBox2.UseCompatibleTextRendering = true;
			//
			//txtAutosaveInterval
			//
			this.txtAutosaveInterval.Location = new System.Drawing.Point(140, 74);
			this.txtAutosaveInterval.Margin = new System.Windows.Forms.Padding(4);
			this.txtAutosaveInterval.Name = "txtAutosaveInterval";
			this.txtAutosaveInterval.Size = new System.Drawing.Size(61, 22);
			this.txtAutosaveInterval.TabIndex = 25;
			//
			//txtAutosaveChanges
			//
			this.txtAutosaveChanges.Location = new System.Drawing.Point(140, 51);
			this.txtAutosaveChanges.Margin = new System.Windows.Forms.Padding(4);
			this.txtAutosaveChanges.Name = "txtAutosaveChanges";
			this.txtAutosaveChanges.Size = new System.Drawing.Size(61, 22);
			this.txtAutosaveChanges.TabIndex = 22;
			//
			//btnAutosaveOpen
			//
			this.btnAutosaveOpen.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnAutosaveOpen.Location = new System.Drawing.Point(209, 71);
			this.btnAutosaveOpen.Name = "btnAutosaveOpen";
			this.btnAutosaveOpen.Size = new System.Drawing.Size(89, 29);
			this.btnAutosaveOpen.TabIndex = 39;
			this.btnAutosaveOpen.Text = "Open Map";
			this.btnAutosaveOpen.UseCompatibleTextRendering = true;
			this.btnAutosaveOpen.UseVisualStyleBackColor = true;
			//
			//cbxAutosaveCompression
			//
			this.cbxAutosaveCompression.AutoSize = true;
			this.cbxAutosaveCompression.Location = new System.Drawing.Point(140, 19);
			this.cbxAutosaveCompression.Margin = new System.Windows.Forms.Padding(4);
			this.cbxAutosaveCompression.Name = "cbxAutosaveCompression";
			this.cbxAutosaveCompression.Size = new System.Drawing.Size(130, 21);
			this.cbxAutosaveCompression.TabIndex = 27;
			this.cbxAutosaveCompression.Text = "Use compression";
			this.cbxAutosaveCompression.UseCompatibleTextRendering = true;
			this.cbxAutosaveCompression.UseVisualStyleBackColor = true;
			//
			//Label2
			//
			this.Label2.AutoSize = true;
			this.Label2.Location = new System.Drawing.Point(7, 54);
			this.Label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.Label2.Name = "Label2";
			this.Label2.Size = new System.Drawing.Size(125, 20);
			this.Label2.TabIndex = 26;
			this.Label2.Text = "Number of changes:";
			this.Label2.UseCompatibleTextRendering = true;
			//
			//cbxAutosaveEnabled
			//
			this.cbxAutosaveEnabled.AutoSize = true;
			this.cbxAutosaveEnabled.Location = new System.Drawing.Point(7, 22);
			this.cbxAutosaveEnabled.Margin = new System.Windows.Forms.Padding(4);
			this.cbxAutosaveEnabled.Name = "cbxAutosaveEnabled";
			this.cbxAutosaveEnabled.Size = new System.Drawing.Size(76, 21);
			this.cbxAutosaveEnabled.TabIndex = 3;
			this.cbxAutosaveEnabled.Text = "Enabled";
			this.cbxAutosaveEnabled.UseCompatibleTextRendering = true;
			this.cbxAutosaveEnabled.UseVisualStyleBackColor = true;
			//
			//Label1
			//
			this.Label1.AutoSize = true;
			this.Label1.Location = new System.Drawing.Point(7, 74);
			this.Label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.Label1.Name = "Label1";
			this.Label1.Size = new System.Drawing.Size(104, 20);
			this.Label1.TabIndex = 24;
			this.Label1.Text = "Time interval (s):";
			this.Label1.UseCompatibleTextRendering = true;
			//
			//GroupBox1
			//
			this.GroupBox1.Controls.Add(this.txtUndoSteps);
			this.GroupBox1.Controls.Add(this.Label11);
			this.GroupBox1.Location = new System.Drawing.Point(6, 6);
			this.GroupBox1.Name = "GroupBox1";
			this.GroupBox1.Size = new System.Drawing.Size(304, 51);
			this.GroupBox1.TabIndex = 36;
			this.GroupBox1.TabStop = false;
			this.GroupBox1.Text = "Undo";
			this.GroupBox1.UseCompatibleTextRendering = true;
			//
			//txtUndoSteps
			//
			this.txtUndoSteps.Location = new System.Drawing.Point(158, 15);
			this.txtUndoSteps.Margin = new System.Windows.Forms.Padding(4);
			this.txtUndoSteps.Name = "txtUndoSteps";
			this.txtUndoSteps.Size = new System.Drawing.Size(61, 22);
			this.txtUndoSteps.TabIndex = 22;
			//
			//Label11
			//
			this.Label11.AutoSize = true;
			this.Label11.Location = new System.Drawing.Point(7, 18);
			this.Label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.Label11.Name = "Label11";
			this.Label11.Size = new System.Drawing.Size(143, 20);
			this.Label11.TabIndex = 24;
			this.Label11.Text = "Maximum stored steps:";
			this.Label11.UseCompatibleTextRendering = true;
			//
			//TabPage2
			//
			this.TabPage2.Controls.Add(this.btnKeyControlChangeDefault);
			this.TabPage2.Controls.Add(this.Label7);
			this.TabPage2.Controls.Add(this.btnKeyControlChangeUnless);
			this.TabPage2.Controls.Add(this.btnKeyControlChange);
			this.TabPage2.Controls.Add(this.lstKeyboardControls);
			this.TabPage2.Location = new System.Drawing.Point(4, 25);
			this.TabPage2.Name = "TabPage2";
			this.TabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.TabPage2.Size = new System.Drawing.Size(629, 369);
			this.TabPage2.TabIndex = 1;
			this.TabPage2.Text = "Keyboard";
			this.TabPage2.UseVisualStyleBackColor = true;
			//
			//btnKeyControlChangeDefault
			//
			this.btnKeyControlChangeDefault.Location = new System.Drawing.Point(417, 156);
			this.btnKeyControlChangeDefault.Name = "btnKeyControlChangeDefault";
			this.btnKeyControlChangeDefault.Size = new System.Drawing.Size(160, 35);
			this.btnKeyControlChangeDefault.TabIndex = 4;
			this.btnKeyControlChangeDefault.Text = "Set To Default";
			this.btnKeyControlChangeDefault.UseCompatibleTextRendering = true;
			this.btnKeyControlChangeDefault.UseVisualStyleBackColor = true;
			//
			//Label7
			//
			this.Label7.Location = new System.Drawing.Point(419, 98);
			this.Label7.Name = "Label7";
			this.Label7.Size = new System.Drawing.Size(158, 66);
			this.Label7.TabIndex = 3;
			this.Label7.Text = "The key combination will be ignored while an \"unless key\" is pressed.";
			this.Label7.UseCompatibleTextRendering = true;
			//
			//btnKeyControlChangeUnless
			//
			this.btnKeyControlChangeUnless.Location = new System.Drawing.Point(417, 59);
			this.btnKeyControlChangeUnless.Name = "btnKeyControlChangeUnless";
			this.btnKeyControlChangeUnless.Size = new System.Drawing.Size(160, 35);
			this.btnKeyControlChangeUnless.TabIndex = 2;
			this.btnKeyControlChangeUnless.Text = "Change Unless Keys";
			this.btnKeyControlChangeUnless.UseCompatibleTextRendering = true;
			this.btnKeyControlChangeUnless.UseVisualStyleBackColor = true;
			//
			//btnKeyControlChange
			//
			this.btnKeyControlChange.Location = new System.Drawing.Point(417, 18);
			this.btnKeyControlChange.Name = "btnKeyControlChange";
			this.btnKeyControlChange.Size = new System.Drawing.Size(160, 35);
			this.btnKeyControlChange.TabIndex = 1;
			this.btnKeyControlChange.Text = "Change Keys";
			this.btnKeyControlChange.UseCompatibleTextRendering = true;
			this.btnKeyControlChange.UseVisualStyleBackColor = true;
			//
			//lstKeyboardControls
			//
			this.lstKeyboardControls.FormattingEnabled = true;
			this.lstKeyboardControls.ItemHeight = 16;
			this.lstKeyboardControls.Location = new System.Drawing.Point(17, 18);
			this.lstKeyboardControls.Name = "lstKeyboardControls";
			this.lstKeyboardControls.ScrollAlwaysVisible = true;
			this.lstKeyboardControls.Size = new System.Drawing.Size(394, 324);
			this.lstKeyboardControls.TabIndex = 0;
			//
			//btnCancel
			//
			this.btnCancel.Location = new System.Drawing.Point(549, 416);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(100, 29);
			this.btnCancel.TabIndex = 39;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseCompatibleTextRendering = true;
			this.btnCancel.UseVisualStyleBackColor = true;
			//
			//btnSave
			//
			this.btnSave.Location = new System.Drawing.Point(443, 416);
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(100, 29);
			this.btnSave.TabIndex = 40;
			this.btnSave.Text = "Accept";
			this.btnSave.UseCompatibleTextRendering = true;
			this.btnSave.UseVisualStyleBackColor = true;
			//
			//frmOptions
			//
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(659, 452);
			this.Controls.Add(this.btnSave);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.TabControl1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmOptions";
			this.Text = "Options";
			this.TabControl1.ResumeLayout(false);
			this.TabPage3.ResumeLayout(false);
			this.TabPage3.PerformLayout();
			this.TabPage1.ResumeLayout(false);
			this.GroupBox3.ResumeLayout(false);
			this.GroupBox3.PerformLayout();
			this.GroupBox8.ResumeLayout(false);
			this.GroupBox8.PerformLayout();
			this.GroupBox7.ResumeLayout(false);
			this.GroupBox7.PerformLayout();
			this.GroupBox6.ResumeLayout(false);
			this.GroupBox6.PerformLayout();
			this.GroupBox5.ResumeLayout(false);
			this.GroupBox5.PerformLayout();
			this.GroupBox4.ResumeLayout(false);
			this.GroupBox2.ResumeLayout(false);
			this.GroupBox2.PerformLayout();
			this.GroupBox1.ResumeLayout(false);
			this.GroupBox1.PerformLayout();
			this.TabPage2.ResumeLayout(false);
			this.ResumeLayout(false);
			
		}
		public System.Windows.Forms.TabControl TabControl1;
		public System.Windows.Forms.TabPage TabPage1;
		public System.Windows.Forms.GroupBox GroupBox1;
		public System.Windows.Forms.Label Label11;
		public System.Windows.Forms.TextBox txtUndoSteps;
		public System.Windows.Forms.GroupBox GroupBox2;
		public System.Windows.Forms.CheckBox cbxAutosaveCompression;
		public System.Windows.Forms.Label Label2;
		public System.Windows.Forms.TextBox txtAutosaveInterval;
		public System.Windows.Forms.CheckBox cbxAutosaveEnabled;
		public System.Windows.Forms.Label Label1;
		public System.Windows.Forms.TextBox txtAutosaveChanges;
		public System.Windows.Forms.GroupBox GroupBox4;
		public System.Windows.Forms.Label lblFont;
		public System.Windows.Forms.Button btnFont;
		public System.Windows.Forms.CheckBox cbxAskDirectories;
		public System.Windows.Forms.Button btnCancel;
		public System.Windows.Forms.Button btnSave;
		public System.Windows.Forms.GroupBox GroupBox5;
		public System.Windows.Forms.CheckBox cbxMinimapObjectColours;
		public System.Windows.Forms.Label Label3;
		public System.Windows.Forms.TextBox txtMinimapSize;
		public System.Windows.Forms.GroupBox GroupBox7;
		public System.Windows.Forms.Label Label4;
		public System.Windows.Forms.TextBox txtFOV;
		public System.Windows.Forms.GroupBox GroupBox6;
		public System.Windows.Forms.CheckBox cbxPointerDirect;
		public System.Windows.Forms.Button btnAutosaveOpen;
		public System.Windows.Forms.CheckBox cbxMinimapTeamColourFeatures;
		internal System.Windows.Forms.Panel pnlMinimapCliffColour;
		public System.Windows.Forms.Label Label5;
		public System.Windows.Forms.Label Label6;
		internal System.Windows.Forms.Panel pnlMinimapSelectedObjectColour;
		public System.Windows.Forms.GroupBox GroupBox8;
		public System.Windows.Forms.CheckBox cbxMipmapsHardware;
		public System.Windows.Forms.CheckBox cbxMipmaps;
		internal System.Windows.Forms.TabPage TabPage2;
		internal System.Windows.Forms.ListBox lstKeyboardControls;
		internal System.Windows.Forms.Button btnKeyControlChange;
		internal System.Windows.Forms.Button btnKeyControlChangeUnless;
		internal System.Windows.Forms.Label Label7;
		internal System.Windows.Forms.Button btnKeyControlChangeDefault;
		internal System.Windows.Forms.TabPage TabPage3;
		internal System.Windows.Forms.TableLayoutPanel TableLayoutPanel1;
		public System.Windows.Forms.Label Label13;
		public System.Windows.Forms.TextBox txtTexturesDepth;
		public System.Windows.Forms.TextBox txtTexturesBPP;
		public System.Windows.Forms.Label Label10;
		public System.Windows.Forms.TextBox txtMapDepth;
		public System.Windows.Forms.TextBox txtMapBPP;
		public System.Windows.Forms.Label Label8;
		public System.Windows.Forms.Label Label9;
		public System.Windows.Forms.GroupBox GroupBox3;
		public System.Windows.Forms.CheckBox cbxPickerOrientation;
		public System.Windows.Forms.Label Label12;
	}
	
}
