using System;
using Eto.Drawing;
using Eto.Forms;

namespace SharpFlame.Gui.Controls
{
	public class ImageButtonBase : Drawable
	{
		bool pressed;
		bool hover;
		bool mouseDown;
		public static Color DisabledColor = Color.FromGrayscale(0.4f, 0.3f);
		public static Color EnabledColor = Eto.Drawing.Colors.Black;

		public override bool Enabled
		{
			get
			{
				return base.Enabled;
			}
			set
			{
				if (base.Enabled != value)
				{
					base.Enabled = value;
					if (Loaded)
						Invalidate();
				}
			}
		}

		public bool Pressed
		{
			get { return pressed; }
			set
			{
				if (pressed != value)
				{
					pressed = value;
					mouseDown = false;
					if (Loaded)
						Invalidate();
				}
			}
		}

		public Color DrawColor
		{
			get { return Enabled ? EnabledColor : DisabledColor; }
		}

		public bool Toggle { get; set; }

		public bool Persistent { get; set; }

		public event EventHandler<EventArgs> Click;

		protected virtual void OnClick(EventArgs e)
		{
			if (Click != null)
				Click(this, e);
		}

		public ImageButtonBase()
		{
			Enabled = true;
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);
			if (Loaded)
				Invalidate();
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			if (Enabled)
			{
				mouseDown = true;
				Invalidate();
			}
		}

		protected override void OnMouseEnter(MouseEventArgs e)
		{
			base.OnMouseEnter(e);
			hover = true;
			Invalidate();
		}

		protected override void OnMouseLeave(MouseEventArgs e)
		{
			base.OnMouseLeave(e);
			hover = false;
			Invalidate();
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);
			var rect = new Rectangle(this.Size);
			if (mouseDown && rect.Contains((Point)e.Location))
			{
				if (Toggle)
					pressed = !pressed;
				else if (Persistent)
					pressed = true;
				else
					pressed = false;
				mouseDown = false;

				this.Invalidate();
				if (Enabled)
					OnClick(EventArgs.Empty);
			}
			else
			{
				mouseDown = false;
				this.Invalidate();
			}
		}

		protected override void OnPaint(PaintEventArgs pe)
		{
			var rect = new Rectangle(this.Size);
			var col = Color.FromGrayscale(hover && Enabled ? 0.95f : 0.8f);
			if (Enabled && (pressed || mouseDown))
			{
				pe.Graphics.FillRectangle(col, rect);
				pe.Graphics.DrawInsetRectangle(Eto.Drawing.Colors.Gray, Eto.Drawing.Colors.White, rect);
			}
			else if (hover && Enabled)
			{
				pe.Graphics.FillRectangle(col, rect);
				pe.Graphics.DrawInsetRectangle(Eto.Drawing.Colors.White, Eto.Drawing.Colors.Gray, rect);
			}

			base.OnPaint(pe);
		}
	}
}