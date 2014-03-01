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
using OpenTK;
using OpenTK.Graphics.OpenGL;
using SharpFlame.Core.Domain;
using SharpFlame.Old;
using SharpFlame.Old.Graphics.OpenGL;
using SharpFlame.Old.Mapping.Tiles;
using SharpFlame.Old.UiOptions;

namespace SharpFlame.Gui.Sections
{
	public class TextureTab : Panel
	{
        readonly SharpFlameApplication application;

        readonly CheckBox chkTexture;
        readonly CheckBox chkOrientation;
        readonly CheckBox chkRandomize;
        readonly CheckBox chkDisplayTileTypes;
        readonly CheckBox chkDisplayTileNumbers;

        readonly Button btnCircular;
        readonly Button btnSquare;

        readonly ImageView btnRotateAntiClockwise;
        readonly ImageView btnRotateClockwise;
        readonly ImageView btnFlipX;

        readonly NumericUpDown nudRadius;

        readonly RadioButtonList rblTerrainModifier;

        readonly ComboBox cbTileset;

        readonly Scrollable scrollTextureView;

		public TextureTab(SharpFlameApplication a) 
        {
            application = a;

			var layout = new DynamicLayout { Padding = Padding.Empty, Spacing = Size.Empty};

            var row = layout.AddSeparateRow (null,
			                                 new Label { Text = "Tileset:", VerticalAlign = VerticalAlign.Middle },
											 cbTileset = textureComboBox (),
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
			textureOrientationLayout.AddRow (null, chkTexture = new CheckBox { Text = "Set Texture" }, null);
			textureOrientationLayout.EndHorizontal ();

			textureOrientationLayout.BeginHorizontal ();
			textureOrientationLayout.AddRow (null, chkOrientation = new CheckBox { Text = "Set Orientation", Checked = true }, null);
			textureOrientationLayout.EndHorizontal ();
			textureOrientationLayout.Add (null);

            var buttonsRandomize = new DynamicLayout { Padding = Padding.Empty, Spacing = Size.Empty };

			buttonsRandomize.Add (null);
			buttonsRandomize.BeginVertical();
			buttonsRandomize.AddRow(null,
                                    TableLayout.AutoSized(btnRotateAntiClockwise = makeBtnRotateAntiClockwise ()),
                                    TableLayout.AutoSized(btnRotateClockwise = makeBtnRotateClockwise ()),
                                    TableLayout.AutoSized(btnFlipX = makeBtnFlipX()),
						   				  null);
			buttonsRandomize.EndVertical ();

			buttonsRandomize.BeginVertical();
            buttonsRandomize.AddRow (null, chkRandomize = new CheckBox { Text = "Randomize" }, null);
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
			tileTypeCombo.Add (tileTypeComboBox());
			tileTypeCombo.EndHorizontal ();

			var tileTypeCheckBoxes = new DynamicLayout ();
			tileTypeCheckBoxes.BeginHorizontal ();
            tileTypeCheckBoxes.Add (chkDisplayTileTypes = new CheckBox { Text = "Display Tile Types" });
			tileTypeCheckBoxes.Add (null);
			tileTypeCheckBoxes.Add (chkDisplayTileNumbers = new CheckBox { Text = "Display Tile Numbers" });
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
            scrollTextureView = new Scrollable { Content = a.GlTexturesView };
            mainLayout.Add (scrollTextureView, true, true);
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

            // Orientation buttons
            btnRotateClockwise.MouseDown += delegate {
                texturesOptions.TextureOrientation.RotateClockwise();
                DrawTexturesView();
            };

            btnRotateAntiClockwise.MouseDown += delegate {
                texturesOptions.TextureOrientation.RotateAntiClockwise();
                DrawTexturesView();
            };

            btnFlipX.MouseDown += delegate {
                texturesOptions.TextureOrientation.FlipX();
                DrawTexturesView();
            };

            // Checkboxes
            chkTexture.Bind (r => r.Checked, texturesOptions, t => t.SetTexture);
            chkOrientation.Bind (r => r.Checked, texturesOptions, t => t.SetOrientation);
            chkRandomize.Bind (r => r.Checked, texturesOptions, t => t.Randomize);

            // RadiobuttonList 
            rblTerrainModifier.SelectedIndexChanged += delegate
            {
                texturesOptions.TerrainMode = (TerrainMode)rblTerrainModifier.SelectedIndex;
            };

            // NumericUpDown radius
            nudRadius.Bind (r => r.Value, texturesOptions, t => t.Radius);

            // Read Tileset Combobox
            App.Tilesets.CollectionChanged += (sender, e) => 
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

            // Bind tileset combobox.
            cbTileset.Bind (r => r.SelectedIndex, texturesOptions, t => t.TilesetNum);
            cbTileset.SelectedIndexChanged += delegate
            {
                DrawTexturesView ();
            };

            chkDisplayTileTypes.CheckedChanged += delegate
            {
                DrawTexturesView ();
            };

            chkDisplayTileNumbers.CheckedChanged += delegate
            {
                DrawTexturesView ();
            };

            // Set Mousetool, when we are shown.
            Shown += delegate {
                App.UiOptions.MouseTool = MouseTool.TextureBrush;
            };

            application.GlTexturesView.Resize += delegate {
                DrawTexturesView();
            };
		}

        void DrawTexturesView() {
            if (App.UiOptions.Textures.TilesetNum == -1)
            {
                GL.Clear(ClearBufferMask.ColorBufferBit);
                GL.Flush();
                application.GlTexturesView.SwapBuffers ();
                return;
            }

            application.GlTexturesView.MakeCurrent ();

            var tileset = App.Tilesets[App.UiOptions.Textures.TilesetNum];

            var textureCount = new XYInt (0, 0);
            textureCount.X = (int)(Math.Floor((scrollTextureView.Size.Width - 20) / 64.0D));
            textureCount.Y = (int)(Math.Ceiling((double)tileset.Tiles.Count / textureCount.X));

            var height = textureCount.Y * 64;
            // TODO: See how thick the scroll is on winforms and mac, 20px seems to be right on GTK.
            var glSize = application.GlTexturesView.Size = new Size (scrollTextureView.Size.Width - 20, height);

            // send the resize event to the Graphics card.
            GL.Viewport( 0, 0, glSize.Width, glSize.Height );
            GL.MatrixMode( MatrixMode.Projection );
            GL.LoadIdentity();
            GL.Ortho( -1.0, 1.0, -1.0, 1.0, 0.0, 4.0 );

            GL.Clear(ClearBufferMask.ColorBufferBit);

            var xyInt = new XYInt();
            var unrotatedPos = new XYDouble();
            var texCoord0 = new XYDouble();
            var texCoord1 = new XYDouble();
            var texCoord2 = new XYDouble();
            var texCoord3 = new XYDouble();

            GL.MatrixMode(MatrixMode.Projection);
            var temp_mat = Matrix4.CreateOrthographicOffCenter(0.0F, glSize.Width, glSize.Height, 0.0F, -1.0F, 1.0F);
            // var temp_mat = Matrix4.CreateOrthographic(glSize.Width, glSize.Height, 1.0f, 1000.0f);
            GL.LoadMatrix(ref temp_mat);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            TileUtil.GetTileRotatedTexCoords(App.UiOptions.Textures.TextureOrientation, ref texCoord0, ref texCoord1, ref texCoord2, ref texCoord3);

            GL.Enable(EnableCap.Texture2D);
            GL.Color4(0.0F, 0.0F, 0.0F, 1.0F);

            for ( var y = 0; y < textureCount.Y; y++ )
            {
                for ( var x = 0; x < textureCount.X; x++ )
                {
                    var num = y * textureCount.X + x;
                    if ( num >= tileset.Tiles.Count )
                    {
                        goto EndOfTextures1;
                    }
                    GL.BindTexture(TextureTarget.Texture2D, tileset.Tiles[num].GlTextureNum);
                    GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (int)TextureEnvMode.Decal);
                    GL.Begin(BeginMode.Quads);
                    //Top-Right
                    GL.TexCoord2(texCoord0.X, texCoord0.Y);
                    GL.Vertex2(x * 64, y * 64);

                    //Top-Left
                    GL.TexCoord2(texCoord1.X, texCoord1.Y);
                    GL.Vertex2(x * 64 + 64, y * 64);

                    //Bottom-Left
                    GL.TexCoord2(texCoord3.X, texCoord3.Y);
                    GL.Vertex2(x * 64 + 64, y * 64 + 64);

                    //Bottom-Right
                    GL.TexCoord2(texCoord2.X, texCoord2.Y);
                    GL.Vertex2(x * 64, y * 64 + 64);

                    GL.End();
                }
            }

            EndOfTextures1:
            GL.Disable(EnableCap.Texture2D);

            if ( (bool)chkDisplayTileTypes.Checked )
            {
                GL.Begin(BeginMode.Quads);
                for ( var y = 0; y <= textureCount.Y - 1; y++ )
                {
                    for ( var x = 0; x <= textureCount.X - 1; x++ )
                    {
                        var num = y * textureCount.X + x;
                        if ( num >= tileset.Tiles.Count )
                        {
                            goto EndOfTextures2;
                        }

                        // TODO: Change this to a per map value once we have a map.
                        num = tileset.Tiles [num].DefaultType;
                        GL.Color3(App.TileTypes[num].DisplayColour.Red, App.TileTypes[num].DisplayColour.Green, App.TileTypes[num].DisplayColour.Blue);

                        GL.Vertex2(x * 64 + 24, y * 64 + 24);
                        GL.Vertex2(x * 64 + 24, y * 64 + 40);
                        GL.Vertex2(x * 64 + 40, y * 64 + 40);
                        GL.Vertex2(x * 64 + 40, y * 64 + 24);
                    }
                }
                EndOfTextures2:
                    GL.End();
            }

            if ( App.DisplayTileOrientation )
            {
                GL.Disable(EnableCap.CullFace);

                unrotatedPos.X = 0.25F;
                unrotatedPos.Y = 0.25F;
                var vertex0 = TileUtil.GetTileRotatedPos_sng(App.TextureOrientation, unrotatedPos);
                unrotatedPos.X = 0.5F;
                unrotatedPos.Y = 0.25F;
                var vertex1 = TileUtil.GetTileRotatedPos_sng(App.TextureOrientation, unrotatedPos);
                unrotatedPos.X = 0.5F;
                unrotatedPos.Y = 0.5F;
                var vertex2 = TileUtil.GetTileRotatedPos_sng(App.TextureOrientation, unrotatedPos);

                GL.Begin(BeginMode.Triangles);
                GL.Color3(1.0F, 1.0F, 0.0F);
                for ( var y = 0; y <= textureCount.Y - 1; y++ )
                {
                    for ( var x = 0; x <= textureCount.X - 1; x++ )
                    {
                        var num = y * textureCount.X + x;
                        if ( num >= tileset.Tiles.Count )
                        {
                            goto EndOfTextures3;
                        }
                        GL.Vertex2(x * 64 + vertex0.X * 64, y * 64 + vertex0.Y * 64);
                        GL.Vertex2(x * 64 + vertex2.X * 64, y * 64 + vertex2.Y * 64);
                        GL.Vertex2(x * 64 + vertex1.X * 64, y * 64 + vertex1.Y * 64);
                    }
                }
                EndOfTextures3:
                    GL.End();

                GL.Enable(EnableCap.CullFace);
            }

            if ( (bool)chkDisplayTileNumbers.Checked && App.UnitLabelFont != null ) //TextureViewFont IsNot Nothing Then
            {
                GL.Enable(EnableCap.Texture2D);
                for ( var y = 0; y <= textureCount.Y - 1; y++ )
                {
                    for ( var x = 0; x <= textureCount.X - 1; x++ )
                    {
                        var num = y * textureCount.X + x;
                        if ( num >= tileset.Tiles.Count )
                        {
                            goto EndOfTextures4;
                        }
                        clsTextLabel textLabel = new clsTextLabel();
                        textLabel.Text = num.ToString ();
                        textLabel.SizeY = 24.0F;
                        textLabel.Colour.Red = 1.0F;
                        textLabel.Colour.Green = 1.0F;
                        textLabel.Colour.Blue = 0.0F;
                        textLabel.Colour.Alpha = 1.0F;
                        textLabel.Pos.X = x * 64;
                        textLabel.Pos.Y = y * 64;
                        textLabel.TextFont = App.UnitLabelFont; //TextureViewFont
                        textLabel.Draw();
                    }
                }
                EndOfTextures4:
                    GL.Disable(EnableCap.Texture2D);
            }

            if ( App.SelectedTextureNum >= 0 & textureCount.X > 0 )
            {
                var a = App.SelectedTextureNum - 1 * textureCount.X;
                xyInt.X = a - a / textureCount.X * textureCount.X;
                xyInt.Y = a / textureCount.X;
                GL.Begin(BeginMode.LineLoop);
                GL.Color3(1.0F, 1.0F, 0.0F);
                GL.Vertex2(xyInt.X * 64, xyInt.Y * 64);
                GL.Vertex2(xyInt.X * 64, xyInt.Y * 64.0D + 64);
                GL.Vertex2(xyInt.X * 64 + 64, xyInt.Y * 64 + 64);
                GL.Vertex2(xyInt.X * 64 + 64, xyInt.Y * 64);
                GL.End();
            }

            GL.Flush();
            application.GlTexturesView.SwapBuffers ();
        }

		ComboBox textureComboBox()
		{
			var control = new ComboBox();
            if (App.Tilesets != null)
            {
                control.Items.AddRange (App.Tilesets);
            }
			return control;
		}

		Control tileTypeComboBox()
		{
			var control = new ComboBox();
			return control;
		}

        ImageView makeBtnRotateAntiClockwise()
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

			return control;
		}

        ImageView makeBtnRotateClockwise()
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

			return control;
		}

        ImageView makeBtnFlipX()
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

			return control;
		}
	}
}

