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
    public class frmWarnings : Form
    {
        [AccessedThroughProperty("tvwWarnings")]
        private TreeView _tvwWarnings;
        private IContainer components;

        public frmWarnings(clsResult result, string windowTitle)
        {
            base.FormClosed += new FormClosedEventHandler(this.frmWarnings_FormClosed);
            this.InitializeComponent();
            this.Icon = modProgram.ProgramIcon;
            this.Text = windowTitle;
            this.tvwWarnings.StateImageList = modWarnings.WarningImages;
            result.MakeNodes(this.tvwWarnings.Nodes);
            this.tvwWarnings.ExpandAll();
            this.tvwWarnings.NodeMouseDoubleClick += new TreeNodeMouseClickEventHandler(this.NodeDoubleClicked);
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

        private void frmWarnings_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.tvwWarnings.NodeMouseDoubleClick -= new TreeNodeMouseClickEventHandler(this.NodeDoubleClicked);
        }

        [DebuggerStepThrough]
        private void InitializeComponent()
        {
            this.tvwWarnings = new TreeView();
            this.SuspendLayout();
            this.tvwWarnings.Dock = DockStyle.Fill;
            Point point2 = new Point(0, 0);
            this.tvwWarnings.Location = point2;
            Padding padding2 = new Padding(3, 2, 3, 2);
            this.tvwWarnings.Margin = padding2;
            this.tvwWarnings.Name = "tvwWarnings";
            Size size2 = new Size(0x1ad, 0xf7);
            this.tvwWarnings.Size = size2;
            this.tvwWarnings.TabIndex = 0;
            SizeF ef2 = new SizeF(8f, 16f);
            this.AutoScaleDimensions = ef2;
            this.AutoScaleMode = AutoScaleMode.Font;
            size2 = new Size(0x1ad, 0xf7);
            this.ClientSize = size2;
            this.Controls.Add(this.tvwWarnings);
            padding2 = new Padding(3, 2, 3, 2);
            this.Margin = padding2;
            this.Name = "frmWarnings";
            this.TopMost = true;
            this.ResumeLayout(false);
        }

        private void NodeDoubleClicked(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                clsResultItemInterface tag = (clsResultItemInterface) e.Node.Tag;
                if (tag != null)
                {
                    tag.DoubleClicked();
                }
            }
        }

        internal virtual TreeView tvwWarnings
        {
            get
            {
                return this._tvwWarnings;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._tvwWarnings = value;
            }
        }
    }
}

