namespace FlaME
{
    using Microsoft.VisualBasic.CompilerServices;
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Runtime.CompilerServices;
    using System.Windows.Forms;

    [DesignerGenerated]
    public class ctrlPathSet : UserControl
    {
        [AccessedThroughProperty("btnAdd")]
        private Button _btnAdd;
        [AccessedThroughProperty("btnRemove")]
        private Button _btnRemove;
        [AccessedThroughProperty("gbxTitle")]
        private GroupBox _gbxTitle;
        [AccessedThroughProperty("lstPaths")]
        private ListBox _lstPaths;
        [AccessedThroughProperty("TableLayoutPanel1")]
        private TableLayoutPanel _TableLayoutPanel1;
        [AccessedThroughProperty("TableLayoutPanel2")]
        private TableLayoutPanel _TableLayoutPanel2;
        private IContainer components;

        public ctrlPathSet(string Title)
        {
            this.InitializeComponent();
            this.gbxTitle.Text = Title;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (this.lstPaths.Items.Count > 0)
            {
                dialog.SelectedPath = Conversions.ToString(this.lstPaths.Items[this.lstPaths.Items.Count - 1]);
            }
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                this.lstPaths.Items.Add(dialog.SelectedPath);
                this.lstPaths.SelectedIndex = this.lstPaths.Items.Count - 1;
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (this.lstPaths.SelectedIndex >= 0)
            {
                this.lstPaths.Items.RemoveAt(this.lstPaths.SelectedIndex);
            }
        }

        [DebuggerNonUserCode]
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && (this.components != null))
                {
                    this.components.Dispose();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        [DebuggerStepThrough]
        private void InitializeComponent()
        {
            this.gbxTitle = new GroupBox();
            this.TableLayoutPanel1 = new TableLayoutPanel();
            this.lstPaths = new ListBox();
            this.TableLayoutPanel2 = new TableLayoutPanel();
            this.btnRemove = new Button();
            this.btnAdd = new Button();
            this.gbxTitle.SuspendLayout();
            this.TableLayoutPanel1.SuspendLayout();
            this.TableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            this.gbxTitle.Controls.Add(this.TableLayoutPanel1);
            this.gbxTitle.Dock = DockStyle.Fill;
            Point point2 = new Point(0, 0);
            this.gbxTitle.Location = point2;
            Padding padding2 = new Padding(0);
            this.gbxTitle.Margin = padding2;
            this.gbxTitle.Name = "gbxTitle";
            padding2 = new Padding(8);
            this.gbxTitle.Padding = padding2;
            Size size2 = new Size(0x1e6, 180);
            this.gbxTitle.Size = size2;
            this.gbxTitle.TabIndex = 0;
            this.gbxTitle.TabStop = false;
            this.gbxTitle.Text = "Path Set Title";
            this.TableLayoutPanel1.ColumnCount = 2;
            this.TableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            this.TableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 96f));
            this.TableLayoutPanel1.Controls.Add(this.lstPaths, 0, 0);
            this.TableLayoutPanel1.Controls.Add(this.TableLayoutPanel2, 1, 0);
            this.TableLayoutPanel1.Dock = DockStyle.Fill;
            point2 = new Point(8, 0x17);
            this.TableLayoutPanel1.Location = point2;
            this.TableLayoutPanel1.Name = "TableLayoutPanel1";
            this.TableLayoutPanel1.RowCount = 1;
            this.TableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            size2 = new Size(470, 0x95);
            this.TableLayoutPanel1.Size = size2;
            this.TableLayoutPanel1.TabIndex = 5;
            this.lstPaths.Dock = DockStyle.Fill;
            this.lstPaths.FormattingEnabled = true;
            this.lstPaths.ItemHeight = 0x10;
            point2 = new Point(3, 3);
            this.lstPaths.Location = point2;
            this.lstPaths.Name = "lstPaths";
            size2 = new Size(0x170, 0x8f);
            this.lstPaths.Size = size2;
            this.lstPaths.TabIndex = 2;
            this.TableLayoutPanel2.ColumnCount = 1;
            this.TableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            this.TableLayoutPanel2.Controls.Add(this.btnRemove, 0, 1);
            this.TableLayoutPanel2.Controls.Add(this.btnAdd, 0, 0);
            this.TableLayoutPanel2.Dock = DockStyle.Fill;
            point2 = new Point(0x176, 0);
            this.TableLayoutPanel2.Location = point2;
            padding2 = new Padding(0);
            this.TableLayoutPanel2.Margin = padding2;
            this.TableLayoutPanel2.Name = "TableLayoutPanel2";
            this.TableLayoutPanel2.RowCount = 3;
            this.TableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 40f));
            this.TableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 40f));
            this.TableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            size2 = new Size(0x60, 0x95);
            this.TableLayoutPanel2.Size = size2;
            this.TableLayoutPanel2.TabIndex = 3;
            this.btnRemove.Dock = DockStyle.Fill;
            point2 = new Point(3, 0x2b);
            this.btnRemove.Location = point2;
            this.btnRemove.Name = "btnRemove";
            size2 = new Size(90, 0x22);
            this.btnRemove.Size = size2;
            this.btnRemove.TabIndex = 4;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseCompatibleTextRendering = true;
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnAdd.Dock = DockStyle.Fill;
            point2 = new Point(3, 3);
            this.btnAdd.Location = point2;
            this.btnAdd.Name = "btnAdd";
            size2 = new Size(90, 0x22);
            this.btnAdd.Size = size2;
            this.btnAdd.TabIndex = 3;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseCompatibleTextRendering = true;
            this.btnAdd.UseVisualStyleBackColor = true;
            this.AutoScaleMode = AutoScaleMode.None;
            this.Controls.Add(this.gbxTitle);
            this.Name = "ctrlPathSet";
            size2 = new Size(0x1e6, 180);
            this.Size = size2;
            this.gbxTitle.ResumeLayout(false);
            this.TableLayoutPanel1.ResumeLayout(false);
            this.TableLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        public void SetPaths(modLists.SimpleList<string> NewPaths)
        {
            this.lstPaths.Items.Clear();
            int num2 = NewPaths.Count - 1;
            for (int i = 0; i <= num2; i++)
            {
                this.lstPaths.Items.Add(NewPaths[i]);
            }
        }

        public virtual Button btnAdd
        {
            get
            {
                return this._btnAdd;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.btnAdd_Click);
                if (this._btnAdd != null)
                {
                    this._btnAdd.Click -= handler;
                }
                this._btnAdd = value;
                if (this._btnAdd != null)
                {
                    this._btnAdd.Click += handler;
                }
            }
        }

        public virtual Button btnRemove
        {
            get
            {
                return this._btnRemove;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.btnRemove_Click);
                if (this._btnRemove != null)
                {
                    this._btnRemove.Click -= handler;
                }
                this._btnRemove = value;
                if (this._btnRemove != null)
                {
                    this._btnRemove.Click += handler;
                }
            }
        }

        public virtual GroupBox gbxTitle
        {
            get
            {
                return this._gbxTitle;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._gbxTitle = value;
            }
        }

        public string[] GetPaths
        {
            get
            {
                string[] strArray2 = new string[(this.lstPaths.Items.Count - 1) + 1];
                int num2 = this.lstPaths.Items.Count - 1;
                for (int i = 0; i <= num2; i++)
                {
                    strArray2[i] = Conversions.ToString(this.lstPaths.Items[i]);
                }
                return strArray2;
            }
        }

        private ListBox lstPaths
        {
            get
            {
                return this._lstPaths;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._lstPaths = value;
            }
        }

        public int SelectedNum
        {
            get
            {
                return this.lstPaths.SelectedIndex;
            }
            set
            {
                this.lstPaths.SelectedIndex = value;
            }
        }

        public string SelectedPath
        {
            get
            {
                if (this.lstPaths.SelectedIndex < 0)
                {
                    return null;
                }
                return Conversions.ToString(this.lstPaths.Items[this.lstPaths.SelectedIndex]);
            }
        }

        public virtual TableLayoutPanel TableLayoutPanel1
        {
            get
            {
                return this._TableLayoutPanel1;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._TableLayoutPanel1 = value;
            }
        }

        public virtual TableLayoutPanel TableLayoutPanel2
        {
            get
            {
                return this._TableLayoutPanel2;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._TableLayoutPanel2 = value;
            }
        }
    }
}

