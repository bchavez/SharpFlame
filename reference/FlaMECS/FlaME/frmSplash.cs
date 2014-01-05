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
    public class frmSplash : Form
    {
        [AccessedThroughProperty("lblStatus")]
        private Label _lblStatus;
        [AccessedThroughProperty("lblVersion")]
        private Label _lblVersion;
        private IContainer components;

        public frmSplash()
        {
            this.InitializeComponent();
            this.Text = "FlaME 1.29 Loading";
            this.lblVersion.Text = "1.29";
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
            ComponentResourceManager manager = new ComponentResourceManager(typeof(frmSplash));
            this.lblVersion = new Label();
            this.lblStatus = new Label();
            this.SuspendLayout();
            this.lblVersion.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            this.lblVersion.BackColor = Color.Transparent;
            this.lblVersion.ForeColor = Color.Black;
            Point point2 = new Point(0x138, 0xa4);
            this.lblVersion.Location = point2;
            this.lblVersion.Name = "lblVersion";
            Size size2 = new Size(0x48, 0x21);
            this.lblVersion.Size = size2;
            this.lblVersion.TabIndex = 0;
            this.lblVersion.Text = "#.## ";
            this.lblVersion.TextAlign = ContentAlignment.MiddleRight;
            this.lblVersion.UseCompatibleTextRendering = true;
            this.lblStatus.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            this.lblStatus.BackColor = Color.Transparent;
            this.lblStatus.ForeColor = Color.Black;
            point2 = new Point(12, 0xa4);
            this.lblStatus.Location = point2;
            this.lblStatus.Name = "lblStatus";
            size2 = new Size(0xe0, 0x21);
            this.lblStatus.Size = size2;
            this.lblStatus.TabIndex = 1;
            this.lblStatus.Text = "Status";
            this.lblStatus.TextAlign = ContentAlignment.MiddleLeft;
            this.lblStatus.UseCompatibleTextRendering = true;
            this.AutoScaleMode = AutoScaleMode.None;
            this.BackColor = Color.White;
            this.BackgroundImage = (Image) manager.GetObject("$this.BackgroundImage");
            this.BackgroundImageLayout = ImageLayout.None;
            size2 = new Size(0x18c, 0xc5);
            this.ClientSize = size2;
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.lblVersion);
            this.Font = new Font("Verdana", 10.2f, FontStyle.Bold, GraphicsUnit.Point, 0);
            this.FormBorderStyle = FormBorderStyle.None;
            this.Name = "frmSplash";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.TransparencyKey = Color.White;
            this.ResumeLayout(false);
        }

        internal virtual Label lblStatus
        {
            get
            {
                return this._lblStatus;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._lblStatus = value;
            }
        }

        internal virtual Label lblVersion
        {
            get
            {
                return this._lblVersion;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._lblVersion = value;
            }
        }
    }
}

