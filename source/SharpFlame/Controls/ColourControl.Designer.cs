namespace SharpFlame.Controls
{
	partial class ColourControl : System.Windows.Forms.UserControl
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
			this.pnlColour = new System.Windows.Forms.Panel();
			this.pnlColour.Click += this.SelectColour;
			this.pnlColour.Paint += this.pnlColour_Paint;
			this.nudAlpha = new System.Windows.Forms.NumericUpDown();
			((System.ComponentModel.ISupportInitialize) this.nudAlpha).BeginInit();
			this.SuspendLayout();
			//
			//pnlColour
			//
			this.pnlColour.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.pnlColour.Location = new System.Drawing.Point(0, 0);
			this.pnlColour.Margin = new System.Windows.Forms.Padding(0);
			this.pnlColour.Name = "pnlColour";
			this.pnlColour.Size = new System.Drawing.Size(51, 24);
			this.pnlColour.TabIndex = 1;
			//
			//nudAlpha
			//
			this.nudAlpha.DecimalPlaces = 2;
			this.nudAlpha.Increment = new decimal(new int[] {1, 0, 0, 65536});
			this.nudAlpha.Location = new System.Drawing.Point(54, 0);
			this.nudAlpha.Maximum = new decimal(new int[] {1, 0, 0, 0});
			this.nudAlpha.Name = "nudAlpha";
			this.nudAlpha.Size = new System.Drawing.Size(50, 22);
			this.nudAlpha.TabIndex = 2;
			this.nudAlpha.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			//
			//ctrlColour
			//
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.Controls.Add(this.nudAlpha);
			this.Controls.Add(this.pnlColour);
			this.Margin = new System.Windows.Forms.Padding(0);
			this.Name = "ColourControl";
			this.Size = new System.Drawing.Size(211, 39);
			((System.ComponentModel.ISupportInitialize) this.nudAlpha).EndInit();
			this.ResumeLayout(false);
			
		}
		internal System.Windows.Forms.Panel pnlColour;
		internal System.Windows.Forms.NumericUpDown nudAlpha;
	}
	
}
