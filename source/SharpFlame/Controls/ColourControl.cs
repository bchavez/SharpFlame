using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using SharpFlame.Colors;
using SharpFlame.MathExtra;

namespace SharpFlame.Controls
{
    public partial class ColourControl
    {
        private clsRGB_sng Colour;
        private Color ColourColor;

        private Graphics ColourBoxGraphics;

        public ColourControl(clsRGB_sng NewColour)
        {
            InitializeComponent();

            if ( NewColour == null )
            {
                Debugger.Break();
                Hide();
                return;
            }

            Colour = NewColour;
            int Red = (int)(MathUtil.Clamp_dbl(Colour.Red * 255.0D, 0.0D, 255.0D));
            int Green = (int)(MathUtil.Clamp_dbl(Colour.Green * 255.0D, 0.0D, 255.0D));
            int Blue = (int)(MathUtil.Clamp_dbl(Colour.Blue * 255.0D, 0.0D, 255.0D));
            ColourColor = ColorTranslator.FromOle(ColorUtil.OSRGB(Red, Green, Blue));

            if ( Colour is clsRGBA_sng )
            {
                nudAlpha.Value = (decimal)(((clsRGBA_sng)Colour).Alpha);
                nudAlpha.ValueChanged += nudAlpha_Changed;
                nudAlpha.Leave += nudAlpha_Changed;
            }
            else
            {
                nudAlpha.Hide();
            }

            ColourBoxGraphics = pnlColour.CreateGraphics();

            ColourBoxRedraw();
        }

        public void SelectColour(Object sender, EventArgs e)
        {
            ColorDialog ColourSelect = new ColorDialog();

            ColourSelect.Color = ColourColor;
            DialogResult Result = ColourSelect.ShowDialog();
            if ( Result != DialogResult.OK )
            {
                return;
            }
            ColourColor = ColourSelect.Color;
            Colour.Red = (float)(ColourColor.R / 255.0D);
            Colour.Green = (float)(ColourColor.G / 255.0D);
            Colour.Blue = (float)(ColourColor.B / 255.0D);
            ColourBoxRedraw();
        }

        private void nudAlpha_Changed(object sender, EventArgs e)
        {
            ((clsRGBA_sng)Colour).Alpha = (float)nudAlpha.Value;
        }

        public void pnlColour_Paint(object sender, PaintEventArgs e)
        {
            ColourBoxRedraw();
        }

        private void ColourBoxRedraw()
        {
            ColourBoxGraphics.Clear(ColourColor);
        }
    }
}