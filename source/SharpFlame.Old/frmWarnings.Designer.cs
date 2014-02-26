namespace SharpFlame.Old
{
	partial class frmWarnings : System.Windows.Forms.Form
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
			this.tvwWarnings = new System.Windows.Forms.TreeView();
			this.FormClosed += frmWarnings_FormClosed;
			this.SuspendLayout();
			//
			//tvwWarnings
			//
			this.tvwWarnings.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tvwWarnings.Location = new System.Drawing.Point(0, 0);
			this.tvwWarnings.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.tvwWarnings.Name = "tvwWarnings";
			this.tvwWarnings.Size = new System.Drawing.Size(429, 247);
			this.tvwWarnings.TabIndex = 0;
			//
			//frmWarnings
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF((float) (8.0F), (float) (16.0F));
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(429, 247);
			this.Controls.Add(this.tvwWarnings);
			this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.Name = "frmWarnings";
			this.TopMost = true;
			this.ResumeLayout(false);
			
		}
		internal System.Windows.Forms.TreeView tvwWarnings;
	}
	
}
