namespace FlaME
{
    using Microsoft.VisualBasic.CompilerServices;
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    [DesignerGenerated]
    public class frmWZLoad : Form
    {
        [AccessedThroughProperty("lstMap")]
        private ListBox _lstMap;
        private IContainer components;
        public string[] lstMap_MapName;
        public clsOutput Output;

        public frmWZLoad(string[] MapNames, clsOutput NewOutput, string FormTitle)
        {
            this.InitializeComponent();
            this.Icon = modProgram.ProgramIcon;
            this.Output = NewOutput;
            this.Output.Result = -1;
            this.lstMap.Items.Clear();
            this.lstMap_MapName = MapNames;
            int upperBound = MapNames.GetUpperBound(0);
            for (int i = 0; i <= upperBound; i++)
            {
                this.lstMap.Items.Add(MapNames[i]);
            }
            this.Text = FormTitle;
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
            this.lstMap = new ListBox();
            this.SuspendLayout();
            this.lstMap.Dock = DockStyle.Fill;
            this.lstMap.FormattingEnabled = true;
            this.lstMap.ItemHeight = 0x10;
            Point point2 = new Point(0, 0);
            this.lstMap.Location = point2;
            Padding padding2 = new Padding(4);
            this.lstMap.Margin = padding2;
            this.lstMap.Name = "lstMap";
            Size size2 = new Size(0x26b, 0x13b);
            this.lstMap.Size = size2;
            this.lstMap.TabIndex = 1;
            SizeF ef2 = new SizeF(8f, 16f);
            this.AutoScaleDimensions = ef2;
            this.AutoScaleMode = AutoScaleMode.Font;
            size2 = new Size(0x26b, 0x13b);
            this.ClientSize = size2;
            this.Controls.Add(this.lstMap);
            padding2 = new Padding(4);
            this.Margin = padding2;
            this.Name = "frmWZLoad";
            this.Text = "frmWZLoad";
            this.ResumeLayout(false);
        }

        private void lstMaps_DoubleClick(object sender, EventArgs e)
        {
            if (this.lstMap.SelectedIndex >= 0)
            {
                this.Output.Result = this.lstMap.SelectedIndex;
                this.Close();
            }
        }

        public virtual ListBox lstMap
        {
            get
            {
                return this._lstMap;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.lstMaps_DoubleClick);
                if (this._lstMap != null)
                {
                    this._lstMap.DoubleClick -= handler;
                }
                this._lstMap = value;
                if (this._lstMap != null)
                {
                    this._lstMap.DoubleClick += handler;
                }
            }
        }

        public class clsOutput
        {
            public int Result;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct sMapNameList
        {
            public string[] Names;
        }
    }
}

