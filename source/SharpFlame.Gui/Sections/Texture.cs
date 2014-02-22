using System;
using System.ComponentModel;
using Eto;
using Eto.Drawing;
using Eto.Forms;

namespace SharpFlame.Gui.Sections
{
	public class Texture : Panel
	{
		public Texture() {
			var layout = new DynamicLayout();
			layout.BeginHorizontal ();
			layout.Add(new Label { Text = "Tileset:", VerticalAlign = VerticalAlign.Middle });
			layout.Add (TextureCheckbox ());
			layout.EndHorizontal ();

			layout.BeginHorizontal ();
			layout.AddAutoSized(CircularButton(), centered: true);
			layout.AddAutoSized(SquareButon(), centered: true);
			layout.EndHorizontal ();

			layout.BeginHorizontal ();
			layout.AddSeparateRow(null, TableLayout.AutoSized(RotateAntiClockwise()), TableLayout.AutoSized(RotateClockwise()), null);
			layout.EndHorizontal ();

			layout.Add(null);

			Content = layout;
		}

		Control CircularButton()
		{
			var control = new Button { Text = "Circular" };
			return control;
		}

		Control SquareButon()
		{
			var control = new Button { Text = "Square" };
			return control;
		}

		Control TextureCheckbox()
		{
			var control = new ComboBox { };
			control.Items.Add(new ListItem { Text = "Arizona" });
			control.Items.Add(new ListItem { Text = "Urban" });
			control.Items.Add(new ListItem { Text = "Rocky Mountains" });
			return control;
		}

		Control RotateAntiClockwise()
		{
			var image = Resources.SelectionRotateAntiClockwise ();
			var control = new Button { Image = image };
			control.Size = new Size(30, 30);
			return control;
		}

		Control RotateClockwise()
		{
			var image = Resources.SelectionRotateClockwise ();
			var control = new ImageView {
				Image = image,
				Size = new Size (image.Width, image.Height)
			};

			control.MouseDown += delegate {
				Console.WriteLine("Mousedown");
			};

			control.MouseEnter += (sender, e) => {
				var s = (ImageView)sender;
				s.BackgroundColor = Colors.Blue;
			};

			control.MouseLeave += (sender, e) => {
				var s = (ImageView)sender;
				s.BackgroundColor = Colors.Transparent;
			};

			// var control = new Button { Image = image };
			// control.Size = new Size(30, 30);
			return control;
		}
	}
}

