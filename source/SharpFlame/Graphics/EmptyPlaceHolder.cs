using Eto.Drawing;
using Eto.Forms;
using SharpFlame.Gui;

namespace SharpFlame.Graphics
{
	public class EmptyPlaceHolder : Drawable
	{
		public EmptyPlaceHolder()
		{
			this.background = Resources.NoMap;
			this.font = new Font(FontFamilies.Monospace, 12);
		}

		private Bitmap background;
		private Font font;

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			var gpx = e.Graphics;

			for( int y = 0; y < this.ClientSize.Height; y += background.Height )
			{
				for( int x = 0; x < this.ClientSize.Width; x+= background.Width )
				{
					gpx.DrawImage(background, x, y);
				}
			}

			gpx.DrawText(font,
				Eto.Drawing.Colors.White,
				this.ClientSize.Width / 2,
				this.ClientSize.Height / 2,
				"No Map");
		}
	}
}