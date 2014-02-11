namespace SharpFlame.Controls
{
    [global::Microsoft.VisualBasic.CompilerServices.DesignerGenerated()]public 
    partial class PathSetControl : System.Windows.Forms.UserControl
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
            this.gbxTitle = new System.Windows.Forms.GroupBox();
            this.TableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lstPaths = new System.Windows.Forms.ListBox();
            this.TableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnRemove.Click += this.btnRemove_Click;
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnAdd.Click += this.btnAdd_Click;
            this.gbxTitle.SuspendLayout();
            this.TableLayoutPanel1.SuspendLayout();
            this.TableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            //
            //gbxTitle
            //
            this.gbxTitle.Controls.Add(this.TableLayoutPanel1);
            this.gbxTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbxTitle.Location = new System.Drawing.Point(0, 0);
            this.gbxTitle.Margin = new System.Windows.Forms.Padding(0);
            this.gbxTitle.Name = "gbxTitle";
            this.gbxTitle.Padding = new System.Windows.Forms.Padding(8);
            this.gbxTitle.Size = new System.Drawing.Size(486, 180);
            this.gbxTitle.TabIndex = 0;
            this.gbxTitle.TabStop = false;
            this.gbxTitle.Text = "Path Set Title";
            //
            //TableLayoutPanel1
            //
            this.TableLayoutPanel1.ColumnCount = 2;
            this.TableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, (float) (100.0F)));
            this.TableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, (float) (96.0F)));
            this.TableLayoutPanel1.Controls.Add(this.lstPaths, 0, 0);
            this.TableLayoutPanel1.Controls.Add(this.TableLayoutPanel2, 1, 0);
            this.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TableLayoutPanel1.Location = new System.Drawing.Point(8, 23);
            this.TableLayoutPanel1.Name = "TableLayoutPanel1";
            this.TableLayoutPanel1.RowCount = 1;
            this.TableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, (float) (100.0F)));
            this.TableLayoutPanel1.Size = new System.Drawing.Size(470, 149);
            this.TableLayoutPanel1.TabIndex = 5;
            //
            //lstPaths
            //
            this.lstPaths.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstPaths.FormattingEnabled = true;
            this.lstPaths.ItemHeight = 16;
            this.lstPaths.Location = new System.Drawing.Point(3, 3);
            this.lstPaths.Name = "lstPaths";
            this.lstPaths.Size = new System.Drawing.Size(368, 143);
            this.lstPaths.TabIndex = 2;
            //
            //TableLayoutPanel2
            //
            this.TableLayoutPanel2.ColumnCount = 1;
            this.TableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, (float) (100.0F)));
            this.TableLayoutPanel2.Controls.Add(this.btnRemove, 0, 1);
            this.TableLayoutPanel2.Controls.Add(this.btnAdd, 0, 0);
            this.TableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TableLayoutPanel2.Location = new System.Drawing.Point(374, 0);
            this.TableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.TableLayoutPanel2.Name = "TableLayoutPanel2";
            this.TableLayoutPanel2.RowCount = 3;
            this.TableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, (float) (40.0F)));
            this.TableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, (float) (40.0F)));
            this.TableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, (float) (100.0F)));
            this.TableLayoutPanel2.Size = new System.Drawing.Size(96, 149);
            this.TableLayoutPanel2.TabIndex = 3;
            //
            //btnRemove
            //
            this.btnRemove.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnRemove.Location = new System.Drawing.Point(3, 43);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(90, 34);
            this.btnRemove.TabIndex = 4;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseCompatibleTextRendering = true;
            this.btnRemove.UseVisualStyleBackColor = true;
            //
            //btnAdd
            //
            this.btnAdd.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnAdd.Location = new System.Drawing.Point(3, 3);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(90, 34);
            this.btnAdd.TabIndex = 3;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseCompatibleTextRendering = true;
            this.btnAdd.UseVisualStyleBackColor = true;
            //
            //ctrlPathSet
            //
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.gbxTitle);
            this.Name = "PathSetControl";
            this.Size = new System.Drawing.Size(486, 180);
            this.gbxTitle.ResumeLayout(false);
            this.TableLayoutPanel1.ResumeLayout(false);
            this.TableLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);
            
        }
        public System.Windows.Forms.GroupBox gbxTitle;
        public System.Windows.Forms.TableLayoutPanel TableLayoutPanel1;
        public System.Windows.Forms.TableLayoutPanel TableLayoutPanel2;
        public System.Windows.Forms.Button btnRemove;
        public System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.ListBox lstPaths;
        
    }
    
}
