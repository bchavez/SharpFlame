namespace SharpFlame
{
	partial class frmWZLoad : System.Windows.Forms.Form
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
			this.lstMap = new System.Windows.Forms.ListBox();
			this.lstMap.DoubleClick += this.lstMaps_DoubleClick;
			this.SuspendLayout();
			//
			//lstMap
			//
			this.lstMap.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lstMap.FormattingEnabled = true;
			this.lstMap.ItemHeight = 16;
			this.lstMap.Location = new System.Drawing.Point(0, 0);
			this.lstMap.Margin = new System.Windows.Forms.Padding(4);
			this.lstMap.Name = "lstMap";
			this.lstMap.Size = new System.Drawing.Size(619, 315);
			this.lstMap.TabIndex = 1;
			//
			//frmWZLoad
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF((float) (8.0F), (float) (16.0F));
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(619, 315);
			this.Controls.Add(this.lstMap);
			this.Margin = new System.Windows.Forms.Padding(4);
			this.Name = "frmWZLoad";
			this.Text = "frmWZLoad";
			this.ResumeLayout(false);
			
		}
		public System.Windows.Forms.ListBox lstMap;
	}
	
}
