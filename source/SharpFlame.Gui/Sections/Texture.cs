using System;
using System.ComponentModel;
using System.Collections.Generic;
using Eto;
using Eto.Drawing;
using Eto.Forms;

namespace SharpFlame.Gui.Sections
{
	public class Texture : Panel
	{
		public Texture() {
			var layout = new DynamicLayout();
			layout.Padding = new Padding (0);
			layout.Spacing = new Size (0, 0);

			var row = layout.AddSeparateRow (null,
			                                 new Label { Text = "Tileset:", VerticalAlign = VerticalAlign.Middle },
											 TextureComboBox (),
											 null);
			row.Table.Visible = false;

			var circularButton = new Button { Text = "Circular" };
			circularButton.Enabled = false;
			var squareButton = new Button { Text = "Square" };
			circularButton.Click += (sender, e) => { 
				circularButton.Enabled = false;
				squareButton.Enabled = true;
			};
			squareButton.Click += (sender, e) => { 
				squareButton.Enabled = false;
				circularButton.Enabled = true;
			};

			layout.BeginVertical();
			layout.AddRow (null,
			               new Label { Text = "Radius:", VerticalAlign = VerticalAlign.Middle }, 
						  new NumericUpDown {Size = new Size(-1, -1) }, 
			              circularButton, 
			              squareButton,
						 null);
			layout.EndVertical ();

			var textureOrientationLayout = new DynamicLayout ();
			textureOrientationLayout.Padding = new Padding (0);
			textureOrientationLayout.Spacing = new Size (0, 0);

			textureOrientationLayout.Add (null);
			textureOrientationLayout.BeginHorizontal ();
			textureOrientationLayout.AddRow (null, 
			                                new CheckBox (),
			                                new Label { Text = "Set Texture", VerticalAlign = VerticalAlign.Middle },
											null);
			textureOrientationLayout.EndHorizontal ();

			textureOrientationLayout.BeginHorizontal ();
			textureOrientationLayout.AddRow (null, 
			                                 new CheckBox (),
			                                 new Label { Text = "Set Orientation", VerticalAlign = VerticalAlign.Middle },
											 null);
			textureOrientationLayout.EndHorizontal ();
			textureOrientationLayout.Add (null);

			var buttonsRandomize = new DynamicLayout ();
			buttonsRandomize.Padding = new Padding (0);
			buttonsRandomize.Spacing = new Size (0, 0);

			buttonsRandomize.Add (null);
			buttonsRandomize.BeginVertical();
			buttonsRandomize.AddRow(null,
			                              BtnRotateAntiClockwise (),
			                              BtnRotateClockwise (),
			                              BtnFlipX(),
						   				  null);
			buttonsRandomize.EndVertical ();

			buttonsRandomize.BeginVertical();
			buttonsRandomize.AddRow (null, new CheckBox (), 
	               		new Label { Text = "Randomize", VerticalAlign = VerticalAlign.Middle }, null);
			buttonsRandomize.EndVertical ();
			buttonsRandomize.Add (null);

			var terrainModifier = new RadioButtonList ();
			terrainModifier.Orientation = RadioButtonListOrientation.Vertical;
			terrainModifier.Items.Add(new ListItem { Text = "Ignore Terrain" });
			terrainModifier.Items.Add(new ListItem { Text = "Reinterpret" });
			terrainModifier.Items.Add(new ListItem { Text = "Remove Terrain" });
			terrainModifier.SelectedIndex = 1;

			row = layout.AddSeparateRow(null,
			        textureOrientationLayout,
			        buttonsRandomize,
			        TableLayout.AutoSized(terrainModifier),
			        null);
			row.Table.Visible = false;

			var mainLayout = new DynamicLayout ();
			mainLayout.Padding = new Padding (0);
			mainLayout.Spacing = new Size (0, 0);
			mainLayout.Add (layout);
			mainLayout.Add(null);

			Content = mainLayout;
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

		Control TextureComboBox()
		{
			var control = new ComboBox { };
			control.Items.Add(new ListItem { Text = "Arizona" });
			control.Items.Add(new ListItem { Text = "Urban" });
			control.Items.Add(new ListItem { Text = "Rocky Mountains" });
			return control;
		}

		Control BtnRotateAntiClockwise()
		{
			var image = Resources.BtnRotateAntiClockwise ();
			var control = new ImageView {
				Image = image,
				Size = new Size (image.Width, image.Height)
			};

			// var control = new Button { Image = image };
			// control.Size = new Size(30, 30);
			control.MouseDown += delegate {
				Console.WriteLine("Mousedown");
			};

			control.MouseEnter += (sender, e) => {
				var s = (ImageView)sender;
				s.BackgroundColor = Colors.Gray;
			};

			control.MouseLeave += (sender, e) => {
				var s = (ImageView)sender;
				s.BackgroundColor = Colors.Transparent;
			};

			return TableLayout.AutoSized(control);
		}

		Control BtnRotateClockwise()
		{
			var image = Resources.BtnRotateClockwise ();
			var control = new ImageView {
				Image = image,
				Size = new Size (image.Width, image.Height)
			};
				
			// var control = new Button { Image = image };
			// control.Size = new Size(30, 30);
			control.MouseDown += delegate {
				Console.WriteLine("Mousedown");
			};

			control.MouseEnter += (sender, e) => {
				var s = (ImageView)sender;
				s.BackgroundColor = Colors.Gray;
			};

			control.MouseLeave += (sender, e) => {
				var s = (ImageView)sender;
				s.BackgroundColor = Colors.Transparent;
			};

			return TableLayout.AutoSized(control);
		}

		Control BtnFlipX()
		{
			var image = Resources.BtnFlipX ();
			var control = new ImageView {
				Image = image,
				Size = new Size (image.Width, image.Height)
			};

			// var control = new Button { Image = image };
			// control.Size = new Size(30, 30);
			control.MouseDown += delegate {
				Console.WriteLine("Mousedown");
			};

			control.MouseEnter += (sender, e) => {
				var s = (ImageView)sender;
				s.BackgroundColor = Colors.Gray;
			};

			control.MouseLeave += (sender, e) => {
				var s = (ImageView)sender;
				s.BackgroundColor = Colors.Transparent;
			};

			return TableLayout.AutoSized(control);
		}
	}
}

