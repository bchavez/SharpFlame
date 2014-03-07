#region

using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using SharpFlame.Colors;
using SharpFlame.Colors;
using SharpFlame.Core.Domain.Colors;
using SharpFlame.Maths;

#endregion

namespace SharpFlame.Controls
{
    public partial class ColourControl
    {
        private readonly Rgb colour;

        private readonly System.Drawing.Graphics colourBoxGraphics;
        private Color colourColor;

        public ColourControl(Rgb newColour)
        {
            InitializeComponent();

            if ( newColour == null )
            {
                Debugger.Break();
                Hide();
                return;
            }

            colour = newColour;
            var red = (int)(MathUtil.ClampDbl(colour.Red * 255.0D, 0.0D, 255.0D));
            var green = (int)(MathUtil.ClampDbl(colour.Green * 255.0D, 0.0D, 255.0D));
            var blue = (int)(MathUtil.ClampDbl(colour.Blue * 255.0D, 0.0D, 255.0D));
            colourColor = ColorTranslator.FromOle(ColorUtil.OsRgb(red, green, blue));

            if ( colour is Rgba )
            {
                nudAlpha.Value = (decimal)(((Rgba)colour).Alpha);
                nudAlpha.ValueChanged += nudAlpha_Changed;
                nudAlpha.Leave += nudAlpha_Changed;
            }
            else
            {
                nudAlpha.Hide();
            }

            colourBoxGraphics = pnlColour.CreateGraphics();

            ColourBoxRedraw();
        }

        public void SelectColour(Object sender, EventArgs e)
        {
            var colourSelect = new ColorDialog
                {
                    Color = colourColor
                };

            var result = colourSelect.ShowDialog();
            if ( result != DialogResult.OK )
            {
                return;
            }
            colourColor = colourSelect.Color;
            colour.Red = (float)(colourColor.R / 255.0D);
            colour.Green = (float)(colourColor.G / 255.0D);
            colour.Blue = (float)(colourColor.B / 255.0D);
            ColourBoxRedraw();
        }

        private void nudAlpha_Changed(object sender, EventArgs e)
        {
            ((Rgba)colour).Alpha = (float)nudAlpha.Value;
        }

        public void pnlColour_Paint(object sender, PaintEventArgs e)
        {
            ColourBoxRedraw();
        }

        private void ColourBoxRedraw()
        {
            colourBoxGraphics.Clear(colourColor);
        }
    }
}