#region

using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using SharpFlame.Colors;
using SharpFlame.Maths;

#endregion

namespace SharpFlame.Controls
{
    public partial class ColourControl
    {
        private readonly clsRGB_sng Colour;

        private readonly System.Drawing.Graphics ColourBoxGraphics;
        private Color ColourColor;

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
            var Red = (int)(MathUtil.ClampDbl(Colour.Red * 255.0D, 0.0D, 255.0D));
            var Green = (int)(MathUtil.ClampDbl(Colour.Green * 255.0D, 0.0D, 255.0D));
            var Blue = (int)(MathUtil.ClampDbl(Colour.Blue * 255.0D, 0.0D, 255.0D));
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
            var ColourSelect = new ColorDialog();

            ColourSelect.Color = ColourColor;
            var Result = ColourSelect.ShowDialog();
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