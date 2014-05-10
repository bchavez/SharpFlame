

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Eto;
using Eto.Drawing;
using Eto.Forms;
using Eto.Gl;
using Ninject;
using OpenTK.Graphics.OpenGL;
using SharpFlame.Core.Domain;
using SharpFlame.Infrastructure;
using SharpFlame;
using SharpFlame.Core;
using SharpFlame.Graphics.OpenGL;
using SharpFlame.Mapping.Tiles;
using SharpFlame.Settings;
using SharpFlame.UiOptions;

namespace SharpFlame.Gui.Sections
{
    public class TextureTab : Panel
    {
        private readonly Options uiOptions;
        private readonly MainMapView mainMapView;

        private CheckBox chkTexture;
        private CheckBox chkOrientation;
        private CheckBox chkRandomize;
        private CheckBox chkDisplayTileTypes;
        private CheckBox chkDisplayTileNumbers;

        private Button btnCircular;
        private Button btnSquare;

        private ImageView btnRotateAntiClockwise;
        private ImageView btnRotateClockwise;
        private ImageView btnFlipX;

        private NumericUpDown nudRadius;

        private RadioButtonList rblTerrainModifier;

        private ComboBox cbTileset;

        private Scrollable scrollTextureView;

        [Inject, Named(NamedBinding.TextureView)]
        internal GLSurface GLSurface { get; set; }

        private XYInt TextureCount { get; set; }

        public TextureTab(IKernel kernel, Options argUiOptions, MainMapView mmv)
        {
            uiOptions = argUiOptions;
            kernel.Inject(this); // GLSurface
            mainMapView = mmv;

            this.TextureCount = new XYInt(0, 0);

            var layout = new DynamicLayout {Padding = Padding.Empty, Spacing = Size.Empty};

            this.cbTileset = TextureComboBox();
            var row = layout.AddSeparateRow(null,
                new Label {Text = "Tileset:", VerticalAlign = VerticalAlign.Middle},
                this.cbTileset,
                null);
            row.Table.Visible = false;


            layout.BeginVertical();
            this.nudRadius = new NumericUpDown {
                Size = new Size(-1, -1), 
                MinValue = 0, 
                MaxValue = Constants.MapMaxSize, 
                Value = uiOptions.Textures.Brush.Radius
            };
            this.btnCircular = new Button {Text = "Circular", Enabled = false};
            this.btnSquare = new Button {Text = "Square"};

            layout.AddRow(null,
                new Label {Text = "Radius:", VerticalAlign = VerticalAlign.Middle},
                this.nudRadius,
                this.btnCircular,
                this.btnSquare,
                null);
            layout.EndVertical();

            var textureOrientationLayout = new DynamicLayout {Padding = Padding.Empty, Spacing = Size.Empty};

            textureOrientationLayout.Add(null);
            textureOrientationLayout.BeginHorizontal();
            textureOrientationLayout.AddRow(null, chkTexture = new CheckBox {Text = "Set Texture"}, null);
            textureOrientationLayout.EndHorizontal();

            textureOrientationLayout.BeginHorizontal();
            textureOrientationLayout.AddRow(null, chkOrientation = new CheckBox {Text = "Set Orientation", Checked = true}, null);
            textureOrientationLayout.EndHorizontal();
            textureOrientationLayout.Add(null);

            var buttonsRandomize = new DynamicLayout {Padding = Padding.Empty, Spacing = Size.Empty};

            buttonsRandomize.Add(null);
            buttonsRandomize.BeginVertical();
            this.btnRotateAntiClockwise = MakeBtnRotateAntiClockwise();
            this.btnRotateClockwise = MakeBtnRotateClockwise();
            this.btnFlipX = MakeBtnFlipX();
            buttonsRandomize.AddRow(null,
                TableLayout.AutoSized(this.btnRotateAntiClockwise),
                TableLayout.AutoSized(this.btnRotateClockwise),
                TableLayout.AutoSized(this.btnFlipX),
                null);
            buttonsRandomize.EndVertical();

            buttonsRandomize.BeginVertical();
            this.chkRandomize = new CheckBox {Text = "Randomize"};
            buttonsRandomize.AddRow(null, this.chkRandomize, null);
            buttonsRandomize.EndVertical();
            buttonsRandomize.Add(null);

            this.rblTerrainModifier = new RadioButtonList
                {
                    Spacing = new Size(0, 0),
                    Orientation = RadioButtonListOrientation.Vertical,
                    Items =
                        {
                            new ListItem {Text = "Ignore Terrain"},
                            new ListItem {Text = "Reinterpret"},
                            new ListItem {Text = "Remove Terrain"}
                        },
                    SelectedIndex = 1
                };

            row = layout.AddSeparateRow(null,
                textureOrientationLayout,
                buttonsRandomize,
                TableLayout.AutoSized(this.rblTerrainModifier),
                null);
            row.Table.Visible = false;

            var mainLayout = new DynamicLayout {Padding = Padding.Empty, Spacing = Size.Empty};

            var tileTypeCombo = new DynamicLayout();
            tileTypeCombo.BeginHorizontal();
            tileTypeCombo.Add(new Label
                {
                    Text = "Tile Type:",
                    VerticalAlign = VerticalAlign.Middle
                });
            tileTypeCombo.Add(TileTypeComboBox());
            tileTypeCombo.EndHorizontal();

            var tileTypeCheckBoxes = new DynamicLayout();
            tileTypeCheckBoxes.BeginHorizontal();
            this.chkDisplayTileTypes = new CheckBox {Text = "Display Tile Types"};
            tileTypeCheckBoxes.Add(this.chkDisplayTileTypes);
            tileTypeCheckBoxes.Add(null);
            this.chkDisplayTileNumbers = new CheckBox {Text = "Display Tile Numbers"};
            tileTypeCheckBoxes.Add(this.chkDisplayTileNumbers);
            tileTypeCheckBoxes.EndHorizontal();

            var tileTypeSetter = new DynamicLayout {Padding = Padding.Empty, Spacing = Size.Empty};
            tileTypeSetter.BeginHorizontal();
            tileTypeSetter.Add(null);
            tileTypeSetter.Add(tileTypeCombo);
            tileTypeSetter.Add(null);
            tileTypeSetter.EndHorizontal();
            tileTypeSetter.BeginHorizontal();
            tileTypeSetter.Add(null);
            tileTypeSetter.Add(tileTypeCheckBoxes);
            tileTypeSetter.Add(null);
            tileTypeSetter.EndHorizontal();

            mainLayout.Add(layout);
            this.scrollTextureView = new Scrollable {Content = this.GLSurface};
            mainLayout.Add(this.scrollTextureView, true, true);
            mainLayout.Add(tileTypeSetter);
            //mainLayout.Add();

            // Set the bindings to UiOptions.Textures
            SetupEventHandlers();

            Content = mainLayout;
        }

        /// <summary>
        /// Sets the Bindings to uiOptions.Textures;
        /// </summary>
        private void SetupEventHandlers()
        {
            Textures texturesOptions = uiOptions.Textures;

            // Circular / Square Button
            btnCircular.Click += (sender, e) =>
            {
                btnCircular.Enabled = false;
                btnSquare.Enabled = true;

                texturesOptions.Brush.Shape = ShapeType.Circle;
            };
            btnSquare.Click += (sender, e) =>
            {
                btnSquare.Enabled = false;
                btnCircular.Enabled = true;
                texturesOptions.Brush.Shape = ShapeType.Square;
            };

            nudRadius.ValueChanged += delegate
            {
                texturesOptions.Brush.Radius = nudRadius.Value;
            };

            // Orientation buttons
            btnRotateClockwise.MouseDown += delegate
                {
                    texturesOptions.TextureOrientation.RotateClockwise();
                    DrawTexturesView();
                };

            btnRotateAntiClockwise.MouseDown += delegate
                {
                    texturesOptions.TextureOrientation.RotateAntiClockwise();
                    DrawTexturesView();
                };

            btnFlipX.MouseDown += delegate
                {
                    texturesOptions.TextureOrientation.FlipX();
                    DrawTexturesView();
                };

            // Checkboxes
            chkTexture.Bind(r => r.Checked, texturesOptions, t => t.SetTexture);
            chkOrientation.Bind(r => r.Checked, texturesOptions, t => t.SetOrientation);
            chkRandomize.Bind(r => r.Checked, texturesOptions, t => t.Randomize);

            // RadiobuttonList 
            rblTerrainModifier.SelectedIndexChanged += delegate
                {
                    texturesOptions.TerrainMode = (TerrainMode)rblTerrainModifier.SelectedIndex;
                };

            // Read Tileset Combobox
            App.Tilesets.CollectionChanged += (sender, e) =>
                {
                    if( e.Action == NotifyCollectionChangedAction.Add )
                    {
                        var list = new List<IListItem>();
                        foreach( var item in e.NewItems )
                        {
                            list.Add((IListItem)item);
                        }
                        cbTileset.Items.AddRange(list);
                        cbTileset.Visible = false;
                        cbTileset.Visible = true;
                    }
                    else if( e.Action == NotifyCollectionChangedAction.Remove )
                    {
                        foreach( var item in e.OldItems )
                        {
                            cbTileset.Items.Remove((IListItem)item);
                        }

                        cbTileset.Visible = false;
                        cbTileset.Visible = true;
                    }
                };

            uiOptions.Textures.TilesetNumChanged += delegate
            {
                if (cbTileset.SelectedIndex == uiOptions.Textures.TilesetNum) {
                    return;
                }

                cbTileset.SelectedIndex = uiOptions.Textures.TilesetNum;
            };

            // Bind tileset combobox.
            cbTileset.Bind(r => r.SelectedIndex, texturesOptions, t => t.TilesetNum);
            cbTileset.SelectedIndexChanged += delegate
            {
                if (mainMapView.MainMap != null && 
                    mainMapView.MainMap.Tileset != App.Tilesets[texturesOptions.TilesetNum]) 
                {
                    var map = mainMapView.MainMap;
                    map.Tileset = App.Tilesets[texturesOptions.TilesetNum];
                    map.TileType_Reset();

                    map.SetPainterToDefaults();

                    map.SectorGraphicsChanges.SetAllChanged();
                    map.Update();

                    mainMapView.DrawLater();
                }
                DrawTexturesView();
            };

            chkDisplayTileTypes.CheckedChanged += delegate
                {
                    DrawTexturesView();
                };

            chkDisplayTileNumbers.CheckedChanged += delegate
                {
                    DrawTexturesView();
                };

            this.GLSurface.MouseDown += (sender, e) =>
                {
                    if( uiOptions.Textures.TilesetNum == -1 )
                    {
                        return;
                    }

                    var args = (MouseEventArgs)e;

                    var x = (int)Math.Floor(args.Location.X / 64);
                    var y = (int)Math.Floor(args.Location.Y / 64);
                    var tile = x + (y * TextureCount.X);
                    if( tile >= App.Tilesets[uiOptions.Textures.TilesetNum].Tiles.Count )
                    {
                        return;
                    }
                    uiOptions.Textures.SelectedTile = tile;
                    DrawTexturesView();
                };

            // Set Mousetool, when we are shown.
            Shown += (sender, args) =>
                {
                    uiOptions.MouseTool = MouseTool.TextureBrush;
                };


            this.GLSurface.Resize += (sender, args) =>
                {
                    DrawTexturesView();
                };
           
        }

        private void DrawTexturesView()
        {
            if( uiOptions.Textures.TilesetNum == -1 )
            {
                GL.Clear(ClearBufferMask.ColorBufferBit);
                GL.Flush();
                this.GLSurface.SwapBuffers();
                return;
            }

            this.GLSurface.MakeCurrent();

            var tileset = App.Tilesets[uiOptions.Textures.TilesetNum];

            var glSize = new Size (0, 0);
            TextureCount = new XYInt {
                X = (int)(Math.Floor (scrollTextureView.ClientSize.Width / 64.0D)),
                Y = (int)(Math.Ceiling ((double)tileset.Tiles.Count / TextureCount.X))
            };        

            if (TextureCount.Y * 64 >= scrollTextureView.Size.Height)
            {
                glSize = GLSurface.GLSize = new Size(scrollTextureView.ClientSize.Width, TextureCount.Y * 64);
            }
            else
            {
                TextureCount = new XYInt {
                    X = (int)(Math.Floor (GLSurface.Size.Width / 64.0D)),
                    Y = (int)(Math.Ceiling (GLSurface.Size.Height / 64.0D))
                };        
                glSize = GLSurface.GLSize = scrollTextureView.ClientSize;
            }
                
            // send the resize event to the Graphics card.
            GL.Viewport(0, 0, glSize.Width, glSize.Height);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(-1.0, 1.0, -1.0, 1.0, 0.0, 4.0);
            GL.MatrixMode (MatrixMode.Modelview);

            GL.Disable (EnableCap.DepthTest);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            var xyInt = new XYInt();
            var unrotatedPos = new XYDouble();
            var texCoord0 = new XYDouble();
            var texCoord1 = new XYDouble();
            var texCoord2 = new XYDouble();
            var texCoord3 = new XYDouble();

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho (0, glSize.Width, glSize.Height, 0, 0, 1);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            TileUtil.GetTileRotatedTexCoords(uiOptions.Textures.TextureOrientation, ref texCoord0, ref texCoord1, ref texCoord2, ref texCoord3);

            GL.Enable(EnableCap.Texture2D);
            GL.Color4(0.0F, 0.0F, 0.0F, 1.0F);

            for( var y = 0; y < TextureCount.Y; y++ )
            {
                for( var x = 0; x < TextureCount.X; x++ )
                {
                    var num = y * TextureCount.X + x;
                    if( num >= tileset.Tiles.Count )
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

            if( (bool)chkDisplayTileTypes.Checked )
            {
                GL.Begin(BeginMode.Quads);
                for( var y = 0; y <= TextureCount.Y - 1; y++ )
                {
                    for( var x = 0; x <= TextureCount.X - 1; x++ )
                    {
                        var num = y * TextureCount.X + x;
                        if( num >= tileset.Tiles.Count )
                        {
                            goto EndOfTextures2;
                        }

                        // TODO: Change this to a per map value once we have a map.
                        num = tileset.Tiles[num].DefaultType;
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

            if( App.DisplayTileOrientation )
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
                for( var y = 0; y <= TextureCount.Y - 1; y++ )
                {
                    for( var x = 0; x <= TextureCount.X - 1; x++ )
                    {
                        var num = y * TextureCount.X + x;
                        if( num >= tileset.Tiles.Count )
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

            if( (bool)chkDisplayTileNumbers.Checked && App.UnitLabelFont != null ) //TextureViewFont IsNot Nothing Then
            {
                GL.Enable(EnableCap.Texture2D);
                for( var y = 0; y <= TextureCount.Y - 1; y++ )
                {
                    for( var x = 0; x <= TextureCount.X - 1; x++ )
                    {
                        var num = y * TextureCount.X + x;
                        if( num >= tileset.Tiles.Count )
                        {
                            goto EndOfTextures4;
                        }
                        clsTextLabel textLabel = new clsTextLabel();
                        textLabel.Text = num.ToString();
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

            if( uiOptions.Textures.SelectedTile >= 0 & TextureCount.X > 0 )
            {
                xyInt.X = uiOptions.Textures.SelectedTile % TextureCount.X;
                xyInt.Y = uiOptions.Textures.SelectedTile / TextureCount.X;
                GL.Begin(BeginMode.LineLoop);
                GL.Color3(1.0F, 1.0F, 0.0F);
                GL.Vertex2(xyInt.X * 64, xyInt.Y * 64);
                GL.Vertex2(xyInt.X * 64, xyInt.Y * 64.0D + 64);
                GL.Vertex2(xyInt.X * 64 + 64, xyInt.Y * 64 + 64);
                GL.Vertex2(xyInt.X * 64 + 64, xyInt.Y * 64);
                GL.End();
            }

            GL.Enable (EnableCap.DepthTest);

            GL.Flush();
            this.GLSurface.SwapBuffers();
        }

        private static ComboBox TextureComboBox()
        {
            var control = new ComboBox();
            if( App.Tilesets != null )
            {
                control.Items.AddRange(App.Tilesets);
            }
            return control;
        }

        private static Control TileTypeComboBox()
        {
            var control = new ComboBox();
            return control;
        }

        private static ImageView MakeBtnRotateAntiClockwise()
        {
            var image = Resources.BtnRotateAntiClockwise();
            var control = new ImageView
            {
                Image = image,
                Size = new Size(image.Width, image.Height)
            };

            control.MouseEnter += (sender, e) =>
            {
                ((ImageView)sender).BackgroundColor = Eto.Drawing.Colors.Gray;
            };

            control.MouseLeave += (sender, e) =>
            {
                ((ImageView)sender).BackgroundColor = Eto.Drawing.Colors.Transparent;
            };

            return control;
        }

        private static ImageView MakeBtnRotateClockwise()
        {
            var image = Resources.BtnRotateClockwise();
            var control = new ImageView
                {
                    Image = image,
                    Size = new Size(image.Width, image.Height)
                };

            control.MouseEnter += (sender, e) =>
            {
                ((ImageView)sender).BackgroundColor = Eto.Drawing.Colors.Gray;
            };

            control.MouseLeave += (sender, e) =>
            {
                ((ImageView)sender).BackgroundColor = Eto.Drawing.Colors.Transparent;
            };

            return control;
        }

        private static ImageView MakeBtnFlipX()
        {
            var image = Resources.BtnFlipX();
            var control = new ImageView
                {
                    Image = image,
                    Size = new Size(image.Width, image.Height)
                };

            control.MouseEnter += (sender, e) =>
            {
                ((ImageView)sender).BackgroundColor = Eto.Drawing.Colors.Gray;
            };

            control.MouseLeave += (sender, e) =>
            {
                ((ImageView)sender).BackgroundColor = Eto.Drawing.Colors.Transparent;
            };

            return control;
        }
    }
}