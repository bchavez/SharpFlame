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
    public class ctrlBrush : UserControl
    {
        [AccessedThroughProperty("Label1")]
        private Label _Label1;
        [AccessedThroughProperty("nudRadius")]
        private NumericUpDown _nudRadius;
        [AccessedThroughProperty("TabPage37")]
        private TabPage _TabPage37;
        [AccessedThroughProperty("TabPage38")]
        private TabPage _TabPage38;
        [AccessedThroughProperty("tabShape")]
        private TabControl _tabShape;
        private clsBrush Brush;
        private IContainer components;
        private bool nudRadiusIsBusy = false;

        public ctrlBrush(clsBrush NewBrush)
        {
            this.InitializeComponent();
            this.Brush = NewBrush;
            this.UpdateControlValues();
            this.nudRadius.ValueChanged += new EventHandler(this.nudRadius_Changed);
            this.nudRadius.Leave += new EventHandler(this.nudRadius_Changed);
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
            this.tabShape = new TabControl();
            this.TabPage37 = new TabPage();
            this.TabPage38 = new TabPage();
            this.Label1 = new Label();
            this.nudRadius = new NumericUpDown();
            this.tabShape.SuspendLayout();
            this.nudRadius.BeginInit();
            this.SuspendLayout();
            this.tabShape.Appearance = TabAppearance.Buttons;
            this.tabShape.Controls.Add(this.TabPage37);
            this.tabShape.Controls.Add(this.TabPage38);
            Size size2 = new Size(0x40, 0x18);
            this.tabShape.ItemSize = size2;
            Point point2 = new Point(0x8b, 0);
            this.tabShape.Location = point2;
            Padding padding2 = new Padding(0);
            this.tabShape.Margin = padding2;
            this.tabShape.Multiline = true;
            this.tabShape.Name = "tabShape";
            point2 = new Point(0, 0);
            this.tabShape.Padding = point2;
            this.tabShape.SelectedIndex = 0;
            size2 = new Size(0x12f, 0x2a);
            this.tabShape.Size = size2;
            this.tabShape.SizeMode = TabSizeMode.Fixed;
            this.tabShape.TabIndex = 0x29;
            point2 = new Point(4, 0x1c);
            this.TabPage37.Location = point2;
            padding2 = new Padding(4);
            this.TabPage37.Margin = padding2;
            this.TabPage37.Name = "TabPage37";
            padding2 = new Padding(4);
            this.TabPage37.Padding = padding2;
            size2 = new Size(0x127, 10);
            this.TabPage37.Size = size2;
            this.TabPage37.TabIndex = 0;
            this.TabPage37.Text = "Circular";
            this.TabPage37.UseVisualStyleBackColor = true;
            point2 = new Point(4, 0x1c);
            this.TabPage38.Location = point2;
            padding2 = new Padding(4);
            this.TabPage38.Margin = padding2;
            this.TabPage38.Name = "TabPage38";
            padding2 = new Padding(4);
            this.TabPage38.Padding = padding2;
            size2 = new Size(0x127, 10);
            this.TabPage38.Size = size2;
            this.TabPage38.TabIndex = 1;
            this.TabPage38.Text = "Square";
            this.TabPage38.UseVisualStyleBackColor = true;
            point2 = new Point(0, 0);
            this.Label1.Location = point2;
            padding2 = new Padding(4, 0, 4, 0);
            this.Label1.Margin = padding2;
            this.Label1.Name = "Label1";
            size2 = new Size(0x34, 20);
            this.Label1.Size = size2;
            this.Label1.TabIndex = 0x27;
            this.Label1.Text = "Radius";
            this.Label1.TextAlign = ContentAlignment.MiddleRight;
            this.Label1.UseCompatibleTextRendering = true;
            this.nudRadius.DecimalPlaces = 2;
            decimal num = new decimal(new int[] { 5, 0, 0, 0x10000 });
            this.nudRadius.Increment = num;
            point2 = new Point(60, 0);
            this.nudRadius.Location = point2;
            padding2 = new Padding(4);
            this.nudRadius.Margin = padding2;
            num = new decimal(new int[] { 0x200, 0, 0, 0 });
            this.nudRadius.Maximum = num;
            this.nudRadius.Name = "nudRadius";
            size2 = new Size(0x4b, 0x16);
            this.nudRadius.Size = size2;
            this.nudRadius.TabIndex = 40;
            this.nudRadius.TextAlign = HorizontalAlignment.Right;
            this.AutoScaleMode = AutoScaleMode.None;
            this.Controls.Add(this.tabShape);
            this.Controls.Add(this.Label1);
            this.Controls.Add(this.nudRadius);
            this.Name = "ctrlBrush";
            size2 = new Size(0x2b7, 0x57);
            this.Size = size2;
            this.tabShape.ResumeLayout(false);
            this.nudRadius.EndInit();
            this.ResumeLayout(false);
        }

        private void nudRadius_Changed(object sender, EventArgs e)
        {
            if (!this.nudRadiusIsBusy)
            {
                double num;
                this.nudRadiusIsBusy = true;
                bool flag = false;
                try
                {
                    num = Convert.ToDouble(this.nudRadius.Value);
                    flag = true;
                }
                catch (Exception exception1)
                {
                    ProjectData.SetProjectError(exception1);
                    ProjectData.ClearProjectError();
                }
                if (flag)
                {
                    this.Brush.Radius = num;
                }
                this.nudRadiusIsBusy = false;
            }
        }

        private void tabShape_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.tabShape.Enabled)
            {
                switch (this.tabShape.SelectedIndex)
                {
                    case 0:
                        this.Brush.Shape = clsBrush.enumShape.Circle;
                        break;

                    case 1:
                        this.Brush.Shape = clsBrush.enumShape.Square;
                        break;
                }
            }
        }

        public void UpdateControlValues()
        {
            this.nudRadius.Enabled = false;
            this.tabShape.Enabled = false;
            if (this.Brush != null)
            {
                this.nudRadius.Value = new decimal(modMath.Clamp_dbl(this.Brush.Radius, Convert.ToDouble(this.nudRadius.Minimum), Convert.ToDouble(this.nudRadius.Maximum)));
                switch (this.Brush.Shape)
                {
                    case clsBrush.enumShape.Circle:
                        this.tabShape.SelectedIndex = 0;
                        break;

                    case clsBrush.enumShape.Square:
                        this.tabShape.SelectedIndex = 1;
                        break;
                }
                this.nudRadius.Enabled = true;
                this.tabShape.Enabled = true;
            }
        }

        public virtual Label Label1
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

        public virtual NumericUpDown nudRadius
        {
            get
            {
                return this._nudRadius;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._nudRadius = value;
            }
        }

        public virtual TabPage TabPage37
        {
            get
            {
                return this._TabPage37;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._TabPage37 = value;
            }
        }

        public virtual TabPage TabPage38
        {
            get
            {
                return this._TabPage38;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._TabPage38 = value;
            }
        }

        public virtual TabControl tabShape
        {
            get
            {
                return this._tabShape;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.tabShape_SelectedIndexChanged);
                if (this._tabShape != null)
                {
                    this._tabShape.SelectedIndexChanged -= handler;
                }
                this._tabShape = value;
                if (this._tabShape != null)
                {
                    this._tabShape.SelectedIndexChanged += handler;
                }
            }
        }
    }
}

