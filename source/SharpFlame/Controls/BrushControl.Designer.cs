namespace SharpFlame.Controls
{
	partial class BrushControl : System.Windows.Forms.UserControl
	{
		
		//UserControl overrides dispose to clean up the component list.
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
			this.tabShape = new System.Windows.Forms.TabControl();
			this.tabShape.SelectedIndexChanged += this.tabShape_SelectedIndexChanged;
			this.TabPage37 = new System.Windows.Forms.TabPage();
			this.TabPage38 = new System.Windows.Forms.TabPage();
			this.Label1 = new System.Windows.Forms.Label();
			this.nudRadius = new System.Windows.Forms.NumericUpDown();
			this.tabShape.SuspendLayout();
			((System.ComponentModel.ISupportInitialize) this.nudRadius).BeginInit();
			this.SuspendLayout();
			//
			//tabShape
			//
			this.tabShape.Appearance = System.Windows.Forms.TabAppearance.Buttons;
			this.tabShape.Controls.Add(this.TabPage37);
			this.tabShape.Controls.Add(this.TabPage38);
			this.tabShape.ItemSize = new System.Drawing.Size(64, 24);
			this.tabShape.Location = new System.Drawing.Point(139, 0);
			this.tabShape.Margin = new System.Windows.Forms.Padding(0);
			this.tabShape.Multiline = true;
			this.tabShape.Name = "tabShape";
			this.tabShape.Padding = new System.Drawing.Point(0, 0);
			this.tabShape.SelectedIndex = 0;
			this.tabShape.Size = new System.Drawing.Size(303, 42);
			this.tabShape.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
			this.tabShape.TabIndex = 41;
			//
			//TabPage37
			//
			this.TabPage37.Location = new System.Drawing.Point(4, 28);
			this.TabPage37.Margin = new System.Windows.Forms.Padding(4);
			this.TabPage37.Name = "TabPage37";
			this.TabPage37.Padding = new System.Windows.Forms.Padding(4);
			this.TabPage37.Size = new System.Drawing.Size(295, 10);
			this.TabPage37.TabIndex = 0;
			this.TabPage37.Text = "Circular";
			this.TabPage37.UseVisualStyleBackColor = true;
			//
			//TabPage38
			//
			this.TabPage38.Location = new System.Drawing.Point(4, 28);
			this.TabPage38.Margin = new System.Windows.Forms.Padding(4);
			this.TabPage38.Name = "TabPage38";
			this.TabPage38.Padding = new System.Windows.Forms.Padding(4);
			this.TabPage38.Size = new System.Drawing.Size(295, 10);
			this.TabPage38.TabIndex = 1;
			this.TabPage38.Text = "Square";
			this.TabPage38.UseVisualStyleBackColor = true;
			//
			//Label1
			//
			this.Label1.Location = new System.Drawing.Point(0, 0);
			this.Label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.Label1.Name = "Label1";
			this.Label1.Size = new System.Drawing.Size(52, 20);
			this.Label1.TabIndex = 39;
			this.Label1.Text = "Radius";
			this.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.Label1.UseCompatibleTextRendering = true;
			//
			//nudRadius
			//
			this.nudRadius.DecimalPlaces = 2;
			this.nudRadius.Increment = new decimal(new int[] {5, 0, 0, 65536});
			this.nudRadius.Location = new System.Drawing.Point(60, 0);
			this.nudRadius.Margin = new System.Windows.Forms.Padding(4);
			this.nudRadius.Maximum = new decimal(new int[] {512, 0, 0, 0});
			this.nudRadius.Name = "nudRadius";
			this.nudRadius.Size = new System.Drawing.Size(75, 22);
			this.nudRadius.TabIndex = 40;
			this.nudRadius.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			//
			//ctrlBrush
			//
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.Controls.Add(this.tabShape);
			this.Controls.Add(this.Label1);
			this.Controls.Add(this.nudRadius);
			this.Name = "BrushControl";
			this.Size = new System.Drawing.Size(695, 87);
			this.tabShape.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize) this.nudRadius).EndInit();
			this.ResumeLayout(false);
			
		}
		public System.Windows.Forms.TabControl tabShape;
		public System.Windows.Forms.TabPage TabPage37;
		public System.Windows.Forms.TabPage TabPage38;
		public System.Windows.Forms.Label Label1;
		public System.Windows.Forms.NumericUpDown nudRadius;
	}
	
}
