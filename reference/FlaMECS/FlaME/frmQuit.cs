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
    public class frmQuit : Form
    {
        [AccessedThroughProperty("btnAsk")]
        private Button _btnAsk;
        [AccessedThroughProperty("btnCancel")]
        private Button _btnCancel;
        [AccessedThroughProperty("btnClose")]
        private Button _btnClose;
        [AccessedThroughProperty("Label1")]
        private Label _Label1;
        private IContainer components;

        public frmQuit()
        {
            this.InitializeComponent();
            this.DialogResult = DialogResult.Cancel;
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
            this.Label1 = new Label();
            this.btnAsk = new Button();
            this.btnClose = new Button();
            this.btnCancel = new Button();
            this.SuspendLayout();
            this.Label1.AutoSize = true;
            Point point2 = new Point(40, 20);
            this.Label1.Location = point2;
            this.Label1.Name = "Label1";
            Size size2 = new Size(0x81, 20);
            this.Label1.Size = size2;
            this.Label1.TabIndex = 0;
            this.Label1.Text = "Maps have changed.";
            this.Label1.UseCompatibleTextRendering = true;
            this.btnAsk.DialogResult = DialogResult.Yes;
            point2 = new Point(0x72, 0x3f);
            this.btnAsk.Location = point2;
            this.btnAsk.Name = "btnAsk";
            size2 = new Size(0x60, 0x20);
            this.btnAsk.Size = size2;
            this.btnAsk.TabIndex = 1;
            this.btnAsk.Text = "Ask";
            this.btnAsk.UseCompatibleTextRendering = true;
            this.btnAsk.UseVisualStyleBackColor = true;
            this.btnClose.DialogResult = DialogResult.No;
            point2 = new Point(0xd8, 0x3f);
            this.btnClose.Location = point2;
            this.btnClose.Name = "btnClose";
            size2 = new Size(0x60, 0x20);
            this.btnClose.Size = size2;
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "Close All";
            this.btnClose.UseCompatibleTextRendering = true;
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnCancel.DialogResult = DialogResult.Cancel;
            point2 = new Point(0x13e, 0x3f);
            this.btnCancel.Location = point2;
            this.btnCancel.Name = "btnCancel";
            size2 = new Size(0x60, 0x20);
            this.btnCancel.Size = size2;
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseCompatibleTextRendering = true;
            this.btnCancel.UseVisualStyleBackColor = true;
            this.AutoScaleMode = AutoScaleMode.None;
            this.CancelButton = this.btnCancel;
            size2 = new Size(0x1a9, 0x6b);
            this.ClientSize = size2;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnAsk);
            this.Controls.Add(this.Label1);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmQuit";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Quit";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        internal virtual Button btnAsk
        {
            get
            {
                return this._btnAsk;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._btnAsk = value;
            }
        }

        internal virtual Button btnCancel
        {
            get
            {
                return this._btnCancel;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._btnCancel = value;
            }
        }

        internal virtual Button btnClose
        {
            get
            {
                return this._btnClose;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._btnClose = value;
            }
        }

        internal virtual Label Label1
        {
            get
            {
                return this._Label1;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._Label1 = value;
            }
        }
    }
}

