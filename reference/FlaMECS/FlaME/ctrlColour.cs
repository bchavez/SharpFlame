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
    public class ctrlColour : UserControl
    {
        [AccessedThroughProperty("nudAlpha")]
        private NumericUpDown _nudAlpha;
        [AccessedThroughProperty("pnlColour")]
        private Panel _pnlColour;
        private clsRGB_sng Colour;
        private Graphics ColourBoxGraphics;
        private Color ColourColor;
        private IContainer components;

        public ctrlColour(clsRGB_sng NewColour)
        {
            this.InitializeComponent();
            if (NewColour == null)
            {
                Debugger.Break();
                this.Hide();
            }
            else
            {
                this.Colour = NewColour;
                int red = (int) Math.Round(modMath.Clamp_dbl(this.Colour.Red * 255.0, 0.0, 255.0));
                int green = (int) Math.Round(modMath.Clamp_dbl(this.Colour.Green * 255.0, 0.0, 255.0));
                int blue = (int) Math.Round(modMath.Clamp_dbl(this.Colour.Blue * 255.0, 0.0, 255.0));
                this.ColourColor = ColorTranslator.FromOle(modColour.OSRGB(red, green, blue));
                if (this.Colour is clsRGBA_sng)
                {
                    this.nudAlpha.Value = new decimal(((clsRGBA_sng) this.Colour).Alpha);
                    this.nudAlpha.ValueChanged += new EventHandler(this.nudAlpha_Changed);
                    this.nudAlpha.Leave += new EventHandler(this.nudAlpha_Changed);
                }
                else
                {
                    this.nudAlpha.Hide();
                }
                this.ColourBoxGraphics = this.pnlColour.CreateGraphics();
                this.ColourBoxRedraw();
            }
        }

        private void ColourBoxRedraw()
        {
            this.ColourBoxGraphics.Clear(this.ColourColor);
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
            this.pnlColour = new Panel();
            this.nudAlpha = new NumericUpDown();
            this.nudAlpha.BeginInit();
            this.SuspendLayout();
            this.pnlColour.BorderStyle = BorderStyle.Fixed3D;
            Point point2 = new Point(0, 0);
            this.pnlColour.Location = point2;
            Padding padding2 = new Padding(0);
            this.pnlColour.Margin = padding2;
            this.pnlColour.Name = "pnlColour";
            Size size2 = new Size(0x33, 0x18);
            this.pnlColour.Size = size2;
            this.pnlColour.TabIndex = 1;
            this.nudAlpha.DecimalPlaces = 2;
            decimal num = new decimal(new int[] { 1, 0, 0, 0x10000 });
            this.nudAlpha.Increment = num;
            point2 = new Point(0x36, 0);
            this.nudAlpha.Location = point2;
            num = new decimal(new int[] { 1, 0, 0, 0 });
            this.nudAlpha.Maximum = num;
            this.nudAlpha.Name = "nudAlpha";
            size2 = new Size(50, 0x16);
            this.nudAlpha.Size = size2;
            this.nudAlpha.TabIndex = 2;
            this.nudAlpha.TextAlign = HorizontalAlignment.Right;
            this.AutoScaleMode = AutoScaleMode.None;
            this.Controls.Add(this.nudAlpha);
            this.Controls.Add(this.pnlColour);
            padding2 = new Padding(0);
            this.Margin = padding2;
            this.Name = "ctrlColour";
            size2 = new Size(0xd3, 0x27);
            this.Size = size2;
            this.nudAlpha.EndInit();
            this.ResumeLayout(false);
        }

        private void nudAlpha_Changed(object sender, EventArgs e)
        {
            ((clsRGBA_sng) this.Colour).Alpha = Convert.ToSingle(this.nudAlpha.Value);
        }

        private void pnlColour_Paint(object sender, PaintEventArgs e)
        {
            this.ColourBoxRedraw();
        }

        private void SelectColour(object sender, EventArgs e)
        {
            ColorDialog dialog = new ColorDialog {
                Color = this.ColourColor
            };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                this.ColourColor = dialog.Color;
                this.Colour.Red = (float) (((double) this.ColourColor.R) / 255.0);
                this.Colour.Green = (float) (((double) this.ColourColor.G) / 255.0);
                this.Colour.Blue = (float) (((double) this.ColourColor.B) / 255.0);
                this.ColourBoxRedraw();
            }
        }

        internal virtual NumericUpDown nudAlpha
        {
            get
            {
                return this._nudAlpha;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._nudAlpha = value;
            }
        }

        internal virtual Panel pnlColour
        {
            get
            {
                return this._pnlColour;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                PaintEventHandler handler = new PaintEventHandler(this.pnlColour_Paint);
                EventHandler handler2 = new EventHandler(this.SelectColour);
                if (this._pnlColour != null)
                {
                    this._pnlColour.Paint -= handler;
                    this._pnlColour.Click -= handler2;
                }
                this._pnlColour = value;
                if (this._pnlColour != null)
                {
                    this._pnlColour.Paint += handler;
                    this._pnlColour.Click += handler2;
                }
            }
        }
    }
}

