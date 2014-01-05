namespace FlaME
{
    using Microsoft.VisualBasic.CompilerServices;
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Runtime.CompilerServices;
    using System.Windows.Forms;

    [DesignerGenerated]
    public class frmKeyboardControl : Form
    {
        [AccessedThroughProperty("btnCancel")]
        private Button _btnCancel;
        [AccessedThroughProperty("btnSave")]
        private Button _btnSave;
        [AccessedThroughProperty("Label1")]
        private Label _Label1;
        [AccessedThroughProperty("lblKeys")]
        private Label _lblKeys;
        private IContainer components;
        public modLists.SimpleList<clsContainer<Keys>> Results;

        public frmKeyboardControl()
        {
            base.KeyDown += new KeyEventHandler(this.frmKeyboardControl_KeyDown);
            this.Results = new modLists.SimpleList<clsContainer<Keys>>();
            this.InitializeComponent();
            this.Icon = modProgram.ProgramIcon;
            this.UpdateLabel();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
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

        private void frmKeyboardControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.Results.Count <= 8)
            {
                IEnumerator enumerator;
                try
                {
                    enumerator = this.Results.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        clsContainer<Keys> current = (clsContainer<Keys>) enumerator.Current;
                        if (((Keys) current.Item) == e.KeyCode)
                        {
                            return;
                        }
                    }
                }
                finally
                {
                    if (enumerator is IDisposable)
                    {
                        (enumerator as IDisposable).Dispose();
                    }
                }
                this.Results.Add(new clsContainer<Keys>(e.KeyCode));
                this.UpdateLabel();
            }
        }

        [DebuggerStepThrough]
        private void InitializeComponent()
        {
            this.Label1 = new Label();
            this.lblKeys = new Label();
            this.btnCancel = new Button();
            this.btnSave = new Button();
            this.SuspendLayout();
            Point point2 = new Point(12, 9);
            this.Label1.Location = point2;
            this.Label1.Name = "Label1";
            Size size2 = new Size(0xe1, 0x15);
            this.Label1.Size = size2;
            this.Label1.TabIndex = 0;
            this.Label1.Text = "Press keys...";
            this.Label1.UseCompatibleTextRendering = true;
            point2 = new Point(0x23, 30);
            this.lblKeys.Location = point2;
            this.lblKeys.Name = "lblKeys";
            size2 = new Size(320, 0x2c);
            this.lblKeys.Size = size2;
            this.lblKeys.TabIndex = 1;
            this.lblKeys.Text = "keys...";
            this.lblKeys.UseCompatibleTextRendering = true;
            this.btnCancel.DialogResult = DialogResult.No;
            point2 = new Point(0xfb, 0x45);
            this.btnCancel.Location = point2;
            this.btnCancel.Name = "btnCancel";
            size2 = new Size(0x60, 0x20);
            this.btnCancel.Size = size2;
            this.btnCancel.TabIndex = 5;
            this.btnCancel.TabStop = false;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseCompatibleTextRendering = true;
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnSave.DialogResult = DialogResult.OK;
            point2 = new Point(0x95, 0x45);
            this.btnSave.Location = point2;
            this.btnSave.Name = "btnSave";
            size2 = new Size(0x60, 0x20);
            this.btnSave.Size = size2;
            this.btnSave.TabIndex = 4;
            this.btnSave.TabStop = false;
            this.btnSave.Text = "Accept";
            this.btnSave.UseCompatibleTextRendering = true;
            this.btnSave.UseVisualStyleBackColor = true;
            this.AutoScaleMode = AutoScaleMode.None;
            size2 = new Size(0x167, 0x71);
            this.ClientSize = size2;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.lblKeys);
            this.Controls.Add(this.Label1);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "frmKeyboardControl";
            this.Text = "Keyboard Control";
            this.ResumeLayout(false);
        }

        private void UpdateLabel()
        {
            this.lblKeys.Text = "";
            int num2 = this.Results.Count - 1;
            for (int i = 0; i <= num2; i++)
            {
                Label lblKeys;
                Keys item = this.Results[i].Item;
                string name = Enum.GetName(typeof(Keys), item);
                if (name == null)
                {
                    lblKeys = this.lblKeys;
                    lblKeys.Text = lblKeys.Text + modIO.InvariantToString_int(this.Results[i].Item);
                }
                else
                {
                    lblKeys = this.lblKeys;
                    lblKeys.Text = lblKeys.Text + name;
                }
                lblKeys = this.lblKeys;
                lblKeys.Text = lblKeys.Text + " ";
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
                EventHandler handler = new EventHandler(this.btnCancel_Click);
                if (this._btnCancel != null)
                {
                    this._btnCancel.Click -= handler;
                }
                this._btnCancel = value;
                if (this._btnCancel != null)
                {
                    this._btnCancel.Click += handler;
                }
            }
        }

        internal virtual Button btnSave
        {
            get
            {
                return this._btnSave;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.btnSave_Click);
                if (this._btnSave != null)
                {
                    this._btnSave.Click -= handler;
                }
                this._btnSave = value;
                if (this._btnSave != null)
                {
                    this._btnSave.Click += handler;
                }
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

        internal virtual Label lblKeys
        {
            get
            {
                return this._lblKeys;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._lblKeys = value;
            }
        }
    }
}

