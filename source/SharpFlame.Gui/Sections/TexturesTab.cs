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
using System.Collections.Generic;
using System.Collections.Specialized;
using Eto;
using Eto.Forms;
using Eto.Drawing;
using SharpFlame.Gui.Forms;
using SharpFlame.Gui.Controls;
using SharpFlame.Old;
using SharpFlame.Old.AppSettings;
using SharpFlame.Old.UiOptions;
using SharpFlame.Core.Domain;

namespace SharpFlame.Gui.Sections
{
	public class TextureTab : Panel
	{
        readonly SharpFlameApplication application;

        readonly CheckBox chkBoxTexture;
        readonly CheckBox chkBoxOrientation;
        readonly CheckBox chkBoxRandomize;

        readonly Button btnCircular;
        readonly Button btnSquare;

        readonly NumericUpDown nudRadius;

        readonly RadioButtonList rblTerrainModifier;

        readonly ComboBox cbTileset;

		public TextureTab(SharpFlameApplication a) 
        {
            application = a;

			var layout = new DynamicLayout { Padding = Padding.Empty, Spacing = Size.Empty};

            var row = layout.AddSeparateRow (null,
			                                 new Label { Text = "Tileset:", VerticalAlign = VerticalAlign.Middle },
											 cbTileset = TextureComboBox (),
											 null);
			row.Table.Visible = false;


			layout.BeginVertical();
			layout.AddRow (null,
			               new Label { Text = "Radius:", VerticalAlign = VerticalAlign.Middle }, 
                           nudRadius = new NumericUpDown {Size = new Size(-1, -1), MinValue = 0, MaxValue = 512 }, 
                          btnCircular = new Button { Text = "Circular", Enabled = false }, 
                          btnSquare = new Button { Text = "Square" },
						 null);
			layout.EndVertical ();

			var textureOrientationLayout = new DynamicLayout { Padding = Padding.Empty, Spacing = Size.Empty };

			textureOrientationLayout.Add (null);
			textureOrientationLayout.BeginHorizontal ();
			textureOrientationLayout.AddRow (null, chkBoxTexture = new CheckBox { Text = "Set Texture" }, null);
			textureOrientationLayout.EndHorizontal ();

			textureOrientationLayout.BeginHorizontal ();
			textureOrientationLayout.AddRow (null, chkBoxOrientation = new CheckBox { Text = "Set Orientation", Checked = true }, null);
			textureOrientationLayout.EndHorizontal ();
			textureOrientationLayout.Add (null);

            var buttonsRandomize = new DynamicLayout { Padding = Padding.Empty, Spacing = Size.Empty };

			buttonsRandomize.Add (null);
			buttonsRandomize.BeginVertical();
			buttonsRandomize.AddRow(null,
			                              BtnRotateAntiClockwise (),
			                              BtnRotateClockwise (),
			                              BtnFlipX(),
						   				  null);
			buttonsRandomize.EndVertical ();

			buttonsRandomize.BeginVertical();
            buttonsRandomize.AddRow (null, chkBoxRandomize = new CheckBox { Text = "Randomize" }, null);
			buttonsRandomize.EndVertical ();
			buttonsRandomize.Add (null);

            rblTerrainModifier = new RadioButtonList ();
            rblTerrainModifier.Spacing = new Size(0, 0);
			rblTerrainModifier.Orientation = RadioButtonListOrientation.Vertical;
			rblTerrainModifier.Items.Add(new ListItem { Text = "Ignore Terrain" });
			rblTerrainModifier.Items.Add(new ListItem { Text = "Reinterpret" });
			rblTerrainModifier.Items.Add(new ListItem { Text = "Remove Terrain" });
			rblTerrainModifier.SelectedIndex = 1;

			row = layout.AddSeparateRow(null,
			        textureOrientationLayout,
			        buttonsRandomize,
			        TableLayout.AutoSized(rblTerrainModifier),
			        null);
			row.Table.Visible = false;

		    var mainLayout = new DynamicLayout {Padding = Padding.Empty, Spacing = Size.Empty};

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

			var tileTypeSetter = new DynamicLayout { Padding = Padding.Empty, Spacing = Size.Empty };
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
            mainLayout.Add (a.GlTexturesView);
			mainLayout.EndVertical ();
			mainLayout.Add (tileTypeSetter);
			//mainLayout.Add();

			// Set the bindings to UiOptions.Textures
			setBindings ();

			Content = mainLayout;
		}

		/// <summary>
		/// Sets the Bindings to App.UiOptions.Textures;
		/// </summary>
		void setBindings() 
		{
            TexturesOptions texturesOptions = App.UiOptions.Textures; 

            // Circular / Square Button
            btnCircular.Click += (sender, e) => { 
                btnCircular.Enabled = false;
                btnSquare.Enabled = true;
                texturesOptions.TerrainMouseMode = TerrainMouseMode.Circular;
            };
            btnSquare.Click += (sender, e) => { 
                btnSquare.Enabled = false;
                btnCircular.Enabled = true;
                texturesOptions.TerrainMouseMode = TerrainMouseMode.Square;
            };

            // Checkboxes
            chkBoxTexture.Bind (r => r.Checked, texturesOptions, t => t.SetTexture);
            chkBoxOrientation.Bind (r => r.Checked, texturesOptions, t => t.SetOrientation);
            chkBoxRandomize.Bind (r => r.Checked, texturesOptions, t => t.Randomize);

            // RadiobuttonList 
            rblTerrainModifier.SelectedIndexChanged += delegate
            {
                texturesOptions.TerrainMode = (TerrainMode)rblTerrainModifier.SelectedIndex;
            };

            // NumericUpDown radius
            nudRadius.Bind (r => r.Value, texturesOptions, t => t.Radius);

            // Read Tileset Combobox
            App.Tilesets.CollectionChanged += (object sender, NotifyCollectionChangedEventArgs e) => 
            {
                if (e.Action == NotifyCollectionChangedAction.Add) {
                    var list = new List<IListItem>();
                    foreach (var item in e.NewItems) {
                        list.Add((IListItem)item);
                    }
                    cbTileset.Items.AddRange (list);
                    cbTileset.Visible = false;
                    cbTileset.Visible = true;
                } else if (e.Action == NotifyCollectionChangedAction.Remove) {
                    foreach (var item in e.OldItems) {
                        cbTileset.Items.Remove((IListItem)item);
                    }

                    cbTileset.Visible = false;
                    cbTileset.Visible = true;
                }
            };

            // Set Mousetool, when we are shown.
            Shown += delegate {
                App.UiOptions.MouseTool = MouseTool.TextureBrush;
            };
		}

		ComboBox TextureComboBox()
		{
			var control = new ComboBox();
            if (App.Tilesets != null)
            {
                control.Items.AddRange (App.Tilesets);
            }
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

