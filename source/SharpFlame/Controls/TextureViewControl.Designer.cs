namespace SharpFlame.Controls
{
	partial class TextureViewControl : System.Windows.Forms.UserControl
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
			this.TableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.TextureScroll = new System.Windows.Forms.VScrollBar();
			this.TextureScroll.ValueChanged += this.TextureScroll_ValueChanged;
			this.pnlDraw = new System.Windows.Forms.Panel();
			this.pnlDraw.Resize += this.pnlDraw_Resize;
			this.TableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			//
			//TableLayoutPanel1
			//
			this.TableLayoutPanel1.ColumnCount = 2;
			this.TableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, (float) (100.0F)));
			this.TableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, (float) (21.0F)));
			this.TableLayoutPanel1.Controls.Add(this.TextureScroll, 1, 0);
			this.TableLayoutPanel1.Controls.Add(this.pnlDraw, 0, 0);
			this.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.TableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.TableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
			this.TableLayoutPanel1.Name = "TableLayoutPanel1";
			this.TableLayoutPanel1.RowCount = 1;
			this.TableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, (float) (100.0F)));
			this.TableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, (float) (25.0F)));
			this.TableLayoutPanel1.Size = new System.Drawing.Size(280, 384);
			this.TableLayoutPanel1.TabIndex = 0;
			//
			//TextureScroll
			//
			this.TextureScroll.Dock = System.Windows.Forms.DockStyle.Fill;
			this.TextureScroll.Location = new System.Drawing.Point(259, 0);
			this.TextureScroll.Name = "TextureScroll";
			this.TextureScroll.Size = new System.Drawing.Size(21, 384);
			this.TextureScroll.TabIndex = 1;
			//
			//pnlDraw
			//
			this.pnlDraw.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlDraw.Location = new System.Drawing.Point(0, 0);
			this.pnlDraw.Margin = new System.Windows.Forms.Padding(0);
			this.pnlDraw.Name = "pnlDraw";
			this.pnlDraw.Size = new System.Drawing.Size(259, 384);
			this.pnlDraw.TabIndex = 2;
			//
			//ctrlTextureView
			//
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.Controls.Add(this.TableLayoutPanel1);
			this.Margin = new System.Windows.Forms.Padding(0);
			this.Name = "TextureViewControl";
			this.Size = new System.Drawing.Size(280, 384);
			this.TableLayoutPanel1.ResumeLayout(false);
			this.ResumeLayout(false);
			
		}
		public System.Windows.Forms.TableLayoutPanel TableLayoutPanel1;
		public System.Windows.Forms.VScrollBar TextureScroll;
		public System.Windows.Forms.Panel pnlDraw;
	}
	
}
