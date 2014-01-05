namespace SharpFlame
{
	[global::Microsoft.VisualBasic.CompilerServices.DesignerGenerated()]public 
	partial class ctrlPlayerNum : System.Windows.Forms.UserControl
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
			this.tsPlayerNum1 = new System.Windows.Forms.ToolStrip();
			this.tsPlayerNum2 = new System.Windows.Forms.ToolStrip();
			this.SuspendLayout();
			//
			//tsPlayerNum1
			//
			this.tsPlayerNum1.Font = new System.Drawing.Font("Microsoft Sans Serif", (float) (7.8F), System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, System.Convert.ToByte(0));
			this.tsPlayerNum1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.tsPlayerNum1.Location = new System.Drawing.Point(0, 0);
			this.tsPlayerNum1.Name = "tsPlayerNum1";
			this.tsPlayerNum1.Size = new System.Drawing.Size(56, 25);
			this.tsPlayerNum1.TabIndex = 0;
			this.tsPlayerNum1.Text = "ToolStrip1";
			//
			//tsPlayerNum2
			//
			this.tsPlayerNum2.Font = new System.Drawing.Font("Microsoft Sans Serif", (float) (7.8F), System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, System.Convert.ToByte(0));
			this.tsPlayerNum2.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.tsPlayerNum2.Location = new System.Drawing.Point(0, 25);
			this.tsPlayerNum2.Name = "tsPlayerNum2";
			this.tsPlayerNum2.Size = new System.Drawing.Size(56, 25);
			this.tsPlayerNum2.TabIndex = 1;
			this.tsPlayerNum2.Text = "ToolStrip1";
			//
			//ctrlPlayerNum
			//
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.Controls.Add(this.tsPlayerNum2);
			this.Controls.Add(this.tsPlayerNum1);
			this.Name = "ctrlPlayerNum";
			this.Size = new System.Drawing.Size(56, 50);
			this.ResumeLayout(false);
			this.PerformLayout();
			
		}
		public System.Windows.Forms.ToolStrip tsPlayerNum1;
		public System.Windows.Forms.ToolStrip tsPlayerNum2;
		
	}
	
}
