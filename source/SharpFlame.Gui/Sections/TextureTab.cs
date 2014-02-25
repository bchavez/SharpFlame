#region License
// /*
// The MIT License (MIT)
//
// Copyright (c) 2013-2014 The SharpFlame Authors.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// */
#endregion

using System;
using Eto.Drawing;
using Eto.Forms;

namespace SharpFlame.Gui.Sections
{
	public class TextureTab : Panel
	{
		public TextureTab() {
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
						  new NumericUpDown {Size = new Size(-1, -1), MinValue = 1, MaxValue = 512 }, 
			              circularButton, 
			              squareButton,
						 null);
			layout.EndVertical ();

			var textureOrientationLayout = new DynamicLayout ();
			textureOrientationLayout.Padding = new Padding (0);
			textureOrientationLayout.Spacing = new Size (0, 0);

			textureOrientationLayout.Add (null);
			textureOrientationLayout.BeginHorizontal ();
			textureOrientationLayout.AddRow (null, new CheckBox { Text = "Set Texture" }, null);
			textureOrientationLayout.EndHorizontal ();

			textureOrientationLayout.BeginHorizontal ();
		    textureOrientationLayout.AddRow (null, new CheckBox { Text = "Set Orientation" }, null);
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
			buttonsRandomize.AddRow (null, new CheckBox { Text = "Randomize" }, null);
			buttonsRandomize.EndVertical ();
			buttonsRandomize.Add (null);

			var terrainModifier = new RadioButtonList ();
            terrainModifier.Spacing = new Size(0, 0);
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

			var textureSelector = new Drawable ();
			textureSelector.BackgroundColor = Colors.Black;

			var tileTypeCombo = new DynamicLayout ();
			tileTypeCombo.BeginHorizontal ();
			tileTypeCombo.Add (new Label {
				Text = "Tile Type:",
				VerticalAlign = VerticalAlign.Middle
			});
			tileTypeCombo.Add (TileTypeComboBox());
			tileTypeCombo.EndHorizontal ();

			var tileTypeCheckBoxes = new DynamicLayout ();
			tileTypeCheckBoxes.BeginHorizontal ();
			tileTypeCheckBoxes.Add (new CheckBox { Text = "Display Tile Types" });
			tileTypeCheckBoxes.Add (null);
			tileTypeCheckBoxes.Add (new CheckBox { Text = "Display Tile Numbers" });
			tileTypeCheckBoxes.EndHorizontal ();

			var tileTypeSetter = new DynamicLayout ();
			tileTypeSetter.Padding = new Padding (0);
			tileTypeSetter.Spacing = new Size (0, 0);
			tileTypeSetter.BeginHorizontal ();
			tileTypeSetter.Add (null);
			tileTypeSetter.Add (tileTypeCombo);
			tileTypeSetter.Add (null);
			tileTypeSetter.EndHorizontal ();
			tileTypeSetter.BeginHorizontal ();
			tileTypeSetter.Add (null);
			tileTypeSetter.Add (tileTypeCheckBoxes);
			tileTypeSetter.Add (null);
			tileTypeSetter.EndHorizontal ();

			mainLayout.Add (layout);
			mainLayout.BeginVertical (xscale: true, yscale: true);
			mainLayout.Add (textureSelector);
			mainLayout.EndVertical ();
			mainLayout.Add (tileTypeSetter);
			//mainLayout.Add();

			Content = mainLayout;
		}

		Control TextureComboBox()
		{
			var control = new ComboBox();
			control.Items.Add(new ListItem { Text = "Arizona" });
			control.Items.Add(new ListItem { Text = "Urban" });
			control.Items.Add(new ListItem { Text = "Rocky Mountains" });
			return control;
		}

		Control TileTypeComboBox()
		{
			var control = new ComboBox();
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
				((ImageView)sender).BackgroundColor = Colors.Gray;
			};

			control.MouseLeave += (sender, e) => {
				((ImageView)sender).BackgroundColor = Colors.Transparent;
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
				((ImageView)sender).BackgroundColor = Colors.Gray;
			};

			control.MouseLeave += (sender, e) => {
				((ImageView)sender).BackgroundColor = Colors.Transparent;
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
				((ImageView)sender).BackgroundColor = Colors.Gray;
			};

			control.MouseLeave += (sender, e) => {
				((ImageView)sender).BackgroundColor = Colors.Transparent;
			};

			return TableLayout.AutoSized(control);
		}
	}
}

