namespace SharpFlame.Old
{
	partial class frmCompile : System.Windows.Forms.Form
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
			this.txtName = new System.Windows.Forms.TextBox();
			this.FormClosed += frmCompile_FormClosed;
			this.FormClosing += frmCompile_FormClosing;
			this.Label1 = new System.Windows.Forms.Label();
			this.Label2 = new System.Windows.Forms.Label();
			this.txtMultiPlayers = new System.Windows.Forms.TextBox();
			this.btnCompileMultiplayer = new System.Windows.Forms.Button();
			this.btnCompileMultiplayer.Click += this.btnCompile_Click;
			this.Label5 = new System.Windows.Forms.Label();
			this.txtScrollMaxX = new System.Windows.Forms.TextBox();
			this.txtScrollMaxY = new System.Windows.Forms.TextBox();
			this.txtScrollMinY = new System.Windows.Forms.TextBox();
			this.Label10 = new System.Windows.Forms.Label();
			this.txtScrollMinX = new System.Windows.Forms.TextBox();
			this.Label11 = new System.Windows.Forms.Label();
			this.Label12 = new System.Windows.Forms.Label();
			this.txtAuthor = new System.Windows.Forms.TextBox();
			this.Label4 = new System.Windows.Forms.Label();
			this.Label13 = new System.Windows.Forms.Label();
			this.cboLicense = new System.Windows.Forms.ComboBox();
			this.Label7 = new System.Windows.Forms.Label();
			this.Label8 = new System.Windows.Forms.Label();
			this.cboCampType = new System.Windows.Forms.ComboBox();
			this.TabControl1 = new System.Windows.Forms.TabControl();
			this.TabPage1 = new System.Windows.Forms.TabPage();
			this.TabPage2 = new System.Windows.Forms.TabPage();
			this.btnCompileCampaign = new System.Windows.Forms.Button();
			this.btnCompileCampaign.Click += this.btnCompileCampaign_Click;
			this.GroupBox1 = new System.Windows.Forms.GroupBox();
			this.cbxAutoScrollLimits = new System.Windows.Forms.CheckBox();
			this.cbxAutoScrollLimits.CheckedChanged += this.cbxAutoScrollLimits_CheckedChanged;
			this.TabControl1.SuspendLayout();
			this.TabPage1.SuspendLayout();
			this.TabPage2.SuspendLayout();
			this.GroupBox1.SuspendLayout();
			this.SuspendLayout();
			//
			//txtName
			//
			this.txtName.Location = new System.Drawing.Point(107, 15);
			this.txtName.Margin = new System.Windows.Forms.Padding(4);
			this.txtName.Name = "txtName";
			this.txtName.Size = new System.Drawing.Size(140, 22);
			this.txtName.TabIndex = 0;
			//
			//Label1
			//
			this.Label1.AutoSize = true;
			this.Label1.Location = new System.Drawing.Point(25, 18);
			this.Label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.Label1.Name = "Label1";
			this.Label1.Size = new System.Drawing.Size(74, 20);
			this.Label1.TabIndex = 1;
			this.Label1.Text = "Map Name:";
			this.Label1.UseCompatibleTextRendering = true;
			//
			//Label2
			//
			this.Label2.AutoSize = true;
			this.Label2.Location = new System.Drawing.Point(18, 20);
			this.Label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.Label2.Name = "Label2";
			this.Label2.Size = new System.Drawing.Size(53, 20);
			this.Label2.TabIndex = 2;
			this.Label2.Text = "Players:";
			this.Label2.UseCompatibleTextRendering = true;
			//
			//txtMultiPlayers
			//
			this.txtMultiPlayers.Location = new System.Drawing.Point(81, 16);
			this.txtMultiPlayers.Margin = new System.Windows.Forms.Padding(4);
			this.txtMultiPlayers.Name = "txtMultiPlayers";
			this.txtMultiPlayers.Size = new System.Drawing.Size(42, 22);
			this.txtMultiPlayers.TabIndex = 5;
			//
			//btnCompileMultiplayer
			//
			this.btnCompileMultiplayer.Location = new System.Drawing.Point(251, 128);
			this.btnCompileMultiplayer.Margin = new System.Windows.Forms.Padding(4);
			this.btnCompileMultiplayer.Name = "btnCompileMultiplayer";
			this.btnCompileMultiplayer.Size = new System.Drawing.Size(128, 30);
			this.btnCompileMultiplayer.TabIndex = 10;
			this.btnCompileMultiplayer.Text = "Compile";
			this.btnCompileMultiplayer.UseCompatibleTextRendering = true;
			this.btnCompileMultiplayer.UseVisualStyleBackColor = true;
			//
			//Label5
			//
			this.Label5.AutoSize = true;
			this.Label5.Location = new System.Drawing.Point(15, 23);
			this.Label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.Label5.Name = "Label5";
			this.Label5.Size = new System.Drawing.Size(38, 20);
			this.Label5.TabIndex = 14;
			this.Label5.Text = "Type:";
			this.Label5.UseCompatibleTextRendering = true;
			//
			//txtScrollMaxX
			//
			this.txtScrollMaxX.Location = new System.Drawing.Point(82, 88);
			this.txtScrollMaxX.Margin = new System.Windows.Forms.Padding(4);
			this.txtScrollMaxX.Name = "txtScrollMaxX";
			this.txtScrollMaxX.Size = new System.Drawing.Size(61, 22);
			this.txtScrollMaxX.TabIndex = 15;
			//
			//txtScrollMaxY
			//
			this.txtScrollMaxY.Location = new System.Drawing.Point(151, 88);
			this.txtScrollMaxY.Margin = new System.Windows.Forms.Padding(4);
			this.txtScrollMaxY.Name = "txtScrollMaxY";
			this.txtScrollMaxY.Size = new System.Drawing.Size(61, 22);
			this.txtScrollMaxY.TabIndex = 18;
			//
			//txtScrollMinY
			//
			this.txtScrollMinY.Location = new System.Drawing.Point(151, 48);
			this.txtScrollMinY.Margin = new System.Windows.Forms.Padding(4);
			this.txtScrollMinY.Name = "txtScrollMinY";
			this.txtScrollMinY.Size = new System.Drawing.Size(61, 22);
			this.txtScrollMinY.TabIndex = 22;
			//
			//Label10
			//
			this.Label10.AutoSize = true;
			this.Label10.Location = new System.Drawing.Point(82, 27);
			this.Label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.Label10.Name = "Label10";
			this.Label10.Size = new System.Drawing.Size(15, 20);
			this.Label10.TabIndex = 21;
			this.Label10.Text = "x:";
			this.Label10.UseCompatibleTextRendering = true;
			//
			//txtScrollMinX
			//
			this.txtScrollMinX.Location = new System.Drawing.Point(82, 48);
			this.txtScrollMinX.Margin = new System.Windows.Forms.Padding(4);
			this.txtScrollMinX.Name = "txtScrollMinX";
			this.txtScrollMinX.Size = new System.Drawing.Size(61, 22);
			this.txtScrollMinX.TabIndex = 20;
			//
			//Label11
			//
			this.Label11.AutoSize = true;
			this.Label11.Location = new System.Drawing.Point(10, 51);
			this.Label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.Label11.Name = "Label11";
			this.Label11.Size = new System.Drawing.Size(63, 20);
			this.Label11.TabIndex = 24;
			this.Label11.Text = "Minimum:";
			this.Label11.UseCompatibleTextRendering = true;
			//
			//Label12
			//
			this.Label12.AutoSize = true;
			this.Label12.Location = new System.Drawing.Point(7, 88);
			this.Label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.Label12.Name = "Label12";
			this.Label12.Size = new System.Drawing.Size(67, 20);
			this.Label12.TabIndex = 25;
			this.Label12.Text = "Maximum:";
			this.Label12.UseCompatibleTextRendering = true;
			//
			//txtAuthor
			//
			this.txtAuthor.Location = new System.Drawing.Point(81, 48);
			this.txtAuthor.Margin = new System.Windows.Forms.Padding(4);
			this.txtAuthor.Name = "txtAuthor";
			this.txtAuthor.Size = new System.Drawing.Size(123, 22);
			this.txtAuthor.TabIndex = 27;
			//
			//Label4
			//
			this.Label4.AutoSize = true;
			this.Label4.Location = new System.Drawing.Point(22, 52);
			this.Label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.Label4.Name = "Label4";
			this.Label4.Size = new System.Drawing.Size(48, 20);
			this.Label4.TabIndex = 26;
			this.Label4.Text = "Author:";
			this.Label4.UseCompatibleTextRendering = true;
			//
			//Label13
			//
			this.Label13.AutoSize = true;
			this.Label13.Location = new System.Drawing.Point(14, 84);
			this.Label13.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.Label13.Name = "Label13";
			this.Label13.Size = new System.Drawing.Size(55, 20);
			this.Label13.TabIndex = 28;
			this.Label13.Text = "License:";
			this.Label13.UseCompatibleTextRendering = true;
			//
			//cboLicense
			//
			this.cboLicense.FormattingEnabled = true;
			this.cboLicense.Items.AddRange(new object[] {"GPL 2+", "CC BY 3.0 + GPL v2+", "CC BY-SA 3.0 + GPL v2+", "CC0"});
			this.cboLicense.Location = new System.Drawing.Point(81, 80);
			this.cboLicense.Margin = new System.Windows.Forms.Padding(4);
			this.cboLicense.Name = "cboLicense";
			this.cboLicense.Size = new System.Drawing.Size(172, 24);
			this.cboLicense.TabIndex = 29;
			//
			//Label7
			//
			this.Label7.AutoSize = true;
			this.Label7.Location = new System.Drawing.Point(151, 27);
			this.Label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.Label7.Name = "Label7";
			this.Label7.Size = new System.Drawing.Size(15, 20);
			this.Label7.TabIndex = 31;
			this.Label7.Text = "y:";
			this.Label7.UseCompatibleTextRendering = true;
			//
			//Label8
			//
			this.Label8.Location = new System.Drawing.Point(262, 80);
			this.Label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.Label8.Name = "Label8";
			this.Label8.Size = new System.Drawing.Size(132, 41);
			this.Label8.TabIndex = 32;
			this.Label8.Text = "Select from the list or type another.";
			this.Label8.UseCompatibleTextRendering = true;
			//
			//cboCampType
			//
			this.cboCampType.DropDownHeight = 512;
			this.cboCampType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboCampType.DropDownWidth = 384;
			this.cboCampType.FormattingEnabled = true;
			this.cboCampType.IntegralHeight = false;
			this.cboCampType.Items.AddRange(new object[] {"Initial scenario state", "Scenario scroll area expansion", "Stand alone mission"});
			this.cboCampType.Location = new System.Drawing.Point(68, 20);
			this.cboCampType.Margin = new System.Windows.Forms.Padding(4);
			this.cboCampType.Name = "cboCampType";
			this.cboCampType.Size = new System.Drawing.Size(160, 24);
			this.cboCampType.TabIndex = 33;
			//
			//TabControl1
			//
			this.TabControl1.Controls.Add(this.TabPage1);
			this.TabControl1.Controls.Add(this.TabPage2);
			this.TabControl1.Location = new System.Drawing.Point(12, 53);
			this.TabControl1.Name = "TabControl1";
			this.TabControl1.SelectedIndex = 0;
			this.TabControl1.Size = new System.Drawing.Size(415, 194);
			this.TabControl1.TabIndex = 34;
			//
			//TabPage1
			//
			this.TabPage1.Controls.Add(this.Label2);
			this.TabPage1.Controls.Add(this.txtMultiPlayers);
			this.TabPage1.Controls.Add(this.Label8);
			this.TabPage1.Controls.Add(this.Label4);
			this.TabPage1.Controls.Add(this.txtAuthor);
			this.TabPage1.Controls.Add(this.Label13);
			this.TabPage1.Controls.Add(this.cboLicense);
			this.TabPage1.Controls.Add(this.btnCompileMultiplayer);
			this.TabPage1.Location = new System.Drawing.Point(4, 25);
			this.TabPage1.Name = "TabPage1";
			this.TabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.TabPage1.Size = new System.Drawing.Size(407, 165);
			this.TabPage1.TabIndex = 0;
			this.TabPage1.Text = "Multiplayer";
			this.TabPage1.UseVisualStyleBackColor = true;
			//
			//TabPage2
			//
			this.TabPage2.Controls.Add(this.btnCompileCampaign);
			this.TabPage2.Controls.Add(this.cboCampType);
			this.TabPage2.Controls.Add(this.Label5);
			this.TabPage2.Location = new System.Drawing.Point(4, 25);
			this.TabPage2.Name = "TabPage2";
			this.TabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.TabPage2.Size = new System.Drawing.Size(407, 165);
			this.TabPage2.TabIndex = 1;
			this.TabPage2.Text = "Campaign";
			this.TabPage2.UseVisualStyleBackColor = true;
			//
			//btnCompileCampaign
			//
			this.btnCompileCampaign.Location = new System.Drawing.Point(253, 128);
			this.btnCompileCampaign.Margin = new System.Windows.Forms.Padding(4);
			this.btnCompileCampaign.Name = "btnCompileCampaign";
			this.btnCompileCampaign.Size = new System.Drawing.Size(128, 30);
			this.btnCompileCampaign.TabIndex = 11;
			this.btnCompileCampaign.Text = "Compile";
			this.btnCompileCampaign.UseVisualStyleBackColor = true;
			//
			//GroupBox1
			//
			this.GroupBox1.Controls.Add(this.Label12);
			this.GroupBox1.Controls.Add(this.txtScrollMaxX);
			this.GroupBox1.Controls.Add(this.Label7);
			this.GroupBox1.Controls.Add(this.txtScrollMaxY);
			this.GroupBox1.Controls.Add(this.txtScrollMinX);
			this.GroupBox1.Controls.Add(this.Label11);
			this.GroupBox1.Controls.Add(this.Label10);
			this.GroupBox1.Controls.Add(this.txtScrollMinY);
			this.GroupBox1.Location = new System.Drawing.Point(12, 280);
			this.GroupBox1.Name = "GroupBox1";
			this.GroupBox1.Size = new System.Drawing.Size(257, 132);
			this.GroupBox1.TabIndex = 35;
			this.GroupBox1.TabStop = false;
			this.GroupBox1.Text = "Scroll Limits";
			this.GroupBox1.UseCompatibleTextRendering = true;
			//
			//cbxAutoScrollLimits
			//
			this.cbxAutoScrollLimits.AutoSize = true;
			this.cbxAutoScrollLimits.Location = new System.Drawing.Point(12, 253);
			this.cbxAutoScrollLimits.Name = "cbxAutoScrollLimits";
			this.cbxAutoScrollLimits.Size = new System.Drawing.Size(206, 21);
			this.cbxAutoScrollLimits.TabIndex = 32;
			this.cbxAutoScrollLimits.Text = "Set Scroll Limits Automatically";
			this.cbxAutoScrollLimits.UseCompatibleTextRendering = true;
			this.cbxAutoScrollLimits.UseVisualStyleBackColor = true;
			//
			//frmCompile
			//
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(436, 429);
			this.Controls.Add(this.cbxAutoScrollLimits);
			this.Controls.Add(this.GroupBox1);
			this.Controls.Add(this.TabControl1);
			this.Controls.Add(this.Label1);
			this.Controls.Add(this.txtName);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Margin = new System.Windows.Forms.Padding(4);
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
		public System.Windows.Forms.TextBox txtName;
		public System.Windows.Forms.Label Label1;
		public System.Windows.Forms.Label Label2;
		public System.Windows.Forms.TextBox txtMultiPlayers;
		public System.Windows.Forms.Button btnCompileMultiplayer;
		public System.Windows.Forms.Label Label5;
		public System.Windows.Forms.TextBox txtScrollMaxX;
		public System.Windows.Forms.TextBox txtScrollMaxY;
		public System.Windows.Forms.TextBox txtScrollMinY;
		public System.Windows.Forms.Label Label10;
		public System.Windows.Forms.TextBox txtScrollMinX;
		public System.Windows.Forms.Label Label11;
		public System.Windows.Forms.Label Label12;
		public System.Windows.Forms.TextBox txtAuthor;
		public System.Windows.Forms.Label Label4;
		public System.Windows.Forms.Label Label13;
		public System.Windows.Forms.ComboBox cboLicense;
		public System.Windows.Forms.Label Label7;
		public System.Windows.Forms.Label Label8;
		public System.Windows.Forms.ComboBox cboCampType;
		public System.Windows.Forms.TabControl TabControl1;
		public System.Windows.Forms.TabPage TabPage1;
		public System.Windows.Forms.TabPage TabPage2;
		public System.Windows.Forms.Button btnCompileCampaign;
		public System.Windows.Forms.GroupBox GroupBox1;
		public System.Windows.Forms.CheckBox cbxAutoScrollLimits;
	}
	
}
