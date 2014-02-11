namespace SharpFlame.Controls
{
    [global::Microsoft.VisualBasic.CompilerServices.DesignerGenerated()]public 
    partial class MapViewControl : System.Windows.Forms.UserControl
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
            this.ssStatus = new System.Windows.Forms.StatusStrip();
            this.lblTile = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblVertex = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblPos = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblUndo = new System.Windows.Forms.ToolStripStatusLabel();
            this.pnlDraw = new System.Windows.Forms.Panel();
            this.pnlDraw.Resize += this.pnlDraw_Resize;
            this.TableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.TableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tabMaps = new System.Windows.Forms.TabControl();
            this.tabMaps.SelectedIndexChanged += this.tabMaps_SelectedIndexChanged;
            this.btnClose = new System.Windows.Forms.Button();
            this.btnClose.Click += this.btnClose_Click;
            this.ssStatus.SuspendLayout();
            this.TableLayoutPanel1.SuspendLayout();
            this.TableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            //
            //ssStatus
            //
            this.ssStatus.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {this.lblTile, this.lblVertex, this.lblPos, this.lblUndo});
            this.ssStatus.Location = new System.Drawing.Point(0, 392);
            this.ssStatus.Name = "ssStatus";
            this.ssStatus.Size = new System.Drawing.Size(1308, 32);
            this.ssStatus.TabIndex = 0;
            this.ssStatus.Text = "StatusStrip1";
            //
            //lblTile
            //
            this.lblTile.AutoSize = false;
            this.lblTile.Font = new System.Drawing.Font("Microsoft Sans Serif", (float) (9.0F), System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, System.Convert.ToByte(0));
            this.lblTile.Margin = new System.Windows.Forms.Padding(2, 3, 2, 2);
            this.lblTile.Name = "lblTile";
            this.lblTile.Size = new System.Drawing.Size(192, 27);
            this.lblTile.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            //lblVertex
            //
            this.lblVertex.AutoSize = false;
            this.lblVertex.Font = new System.Drawing.Font("Microsoft Sans Serif", (float) (9.0F), System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, System.Convert.ToByte(0));
            this.lblVertex.Margin = new System.Windows.Forms.Padding(2, 3, 2, 2);
            this.lblVertex.Name = "lblVertex";
            this.lblVertex.Size = new System.Drawing.Size(256, 27);
            this.lblVertex.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            //lblPos
            //
            this.lblPos.AutoSize = false;
            this.lblPos.Font = new System.Drawing.Font("Microsoft Sans Serif", (float) (9.0F), System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, System.Convert.ToByte(0));
            this.lblPos.Margin = new System.Windows.Forms.Padding(2, 3, 2, 2);
            this.lblPos.Name = "lblPos";
            this.lblPos.Size = new System.Drawing.Size(320, 27);
            this.lblPos.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            //lblUndo
            //
            this.lblUndo.AutoSize = false;
            this.lblUndo.Font = new System.Drawing.Font("Microsoft Sans Serif", (float) (9.0F), System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, System.Convert.ToByte(0));
            this.lblUndo.Name = "lblUndo";
            this.lblUndo.Size = new System.Drawing.Size(256, 27);
            this.lblUndo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            //pnlDraw
            //
            this.pnlDraw.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlDraw.Location = new System.Drawing.Point(0, 28);
            this.pnlDraw.Margin = new System.Windows.Forms.Padding(0);
            this.pnlDraw.Name = "pnlDraw";
            this.pnlDraw.Size = new System.Drawing.Size(1308, 364);
            this.pnlDraw.TabIndex = 1;
            //
            //TableLayoutPanel1
            //
            this.TableLayoutPanel1.ColumnCount = 1;
            this.TableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, (float) (100.0F)));
            this.TableLayoutPanel1.Controls.Add(this.pnlDraw, 0, 1);
            this.TableLayoutPanel1.Controls.Add(this.TableLayoutPanel2, 0, 0);
            this.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.TableLayoutPanel1.Name = "TableLayoutPanel1";
            this.TableLayoutPanel1.RowCount = 2;
            this.TableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, (float) (28.0F)));
            this.TableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, (float) (100.0F)));
            this.TableLayoutPanel1.Size = new System.Drawing.Size(1308, 392);
            this.TableLayoutPanel1.TabIndex = 2;
            //
            //TableLayoutPanel2
            //
            this.TableLayoutPanel2.ColumnCount = 2;
            this.TableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, (float) (100.0F)));
            this.TableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, (float) (32.0F)));
            this.TableLayoutPanel2.Controls.Add(this.tabMaps, 0, 0);
            this.TableLayoutPanel2.Controls.Add(this.btnClose, 1, 0);
            this.TableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.TableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.TableLayoutPanel2.Name = "TableLayoutPanel2";
            this.TableLayoutPanel2.RowCount = 1;
            this.TableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, (float) (100.0F)));
            this.TableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, (float) (20.0F)));
            this.TableLayoutPanel2.Size = new System.Drawing.Size(1308, 28);
            this.TableLayoutPanel2.TabIndex = 2;
            //
            //tabMaps
            //
            this.tabMaps.Appearance = System.Windows.Forms.TabAppearance.Buttons;
            this.tabMaps.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabMaps.Location = new System.Drawing.Point(3, 3);
            this.tabMaps.Name = "tabMaps";
            this.tabMaps.SelectedIndex = 0;
            this.tabMaps.Size = new System.Drawing.Size(1270, 22);
            this.tabMaps.TabIndex = 2;
            //
            //btnClose
            //
            this.btnClose.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnClose.Location = new System.Drawing.Point(1276, 0);
            this.btnClose.Margin = new System.Windows.Forms.Padding(0);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(32, 28);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "X";
            this.btnClose.UseVisualStyleBackColor = true;
            //
            //ctrlMapView
            //
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.TableLayoutPanel1);
            this.Controls.Add(this.ssStatus);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "MapViewControl";
            this.Size = new System.Drawing.Size(1308, 424);
            this.ssStatus.ResumeLayout(false);
            this.ssStatus.PerformLayout();
            this.TableLayoutPanel1.ResumeLayout(false);
            this.TableLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
            
        }
        public System.Windows.Forms.StatusStrip ssStatus;
        public System.Windows.Forms.ToolStripStatusLabel lblTile;
        public System.Windows.Forms.ToolStripStatusLabel lblVertex;
        public System.Windows.Forms.ToolStripStatusLabel lblPos;
        public System.Windows.Forms.Panel pnlDraw;
        public System.Windows.Forms.ToolStripStatusLabel lblUndo;
        internal System.Windows.Forms.TableLayoutPanel TableLayoutPanel1;
        internal System.Windows.Forms.TabControl tabMaps;
        internal System.Windows.Forms.TableLayoutPanel TableLayoutPanel2;
        internal System.Windows.Forms.Button btnClose;
    }
    
}
