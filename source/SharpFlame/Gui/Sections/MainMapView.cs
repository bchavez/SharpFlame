using System;
using System.ComponentModel;
using System.Diagnostics;
using Appccelerate.EventBroker;
using Appccelerate.EventBroker.Handlers;
using Eto.Forms;
using Eto.Gl;
using Ninject;
using Ninject.Extensions.Logging;
using OpenTK.Graphics.OpenGL;
using SharpFlame.Core;
using SharpFlame.Core.Domain;
using SharpFlame.Core.Domain.Colors;
using SharpFlame.Core.Extensions;
using SharpFlame.Domain;
using SharpFlame.Graphics.OpenGL;
using SharpFlame.Gui.Forms;
using SharpFlame.Infrastructure;
using SharpFlame.Mapping;
using SharpFlame.Mapping.Minimap;
using SharpFlame.Mapping.Tools;
using SharpFlame.Maths;
using SharpFlame.Settings;
using SharpFlame.Util;
using SharpFlame.UiOptions;

namespace SharpFlame.Gui.Sections
{
    public class MainMapView : Panel
	{
        public GLSurface GLSurface { get; set; }

        private Map mainMap;
        public Map MainMap { 
            get { return mainMap; }
            set
            {
                // Unload the previous map, until we have multimap support.
                if(mainMap != null)
                {
//                    if ( !map.ClosePrompt() )
//                    {
//                        return;
//                    }

                    mainMap.Deallocate();
                }

                mainMap = value;

                viewInfo.Map = mainMap;
                minimapCreator.Map = mainMap;

                if(mainMap != null)
                {
                    mainMap.InitializeUserInput();
                    mainMap.SectorGraphicsChanges.SetAllChanged();
                    mainMap.Update();

                    // Change the tileset in the TexturesView.
                    uiOptions.Textures.TilesetNum = App.Tilesets.IndexOf(mainMap.Tileset);

                    // Update the title.
                    mainForm.MainMapName = mainMap.InterfaceOptions.FileName;
                } else
                {
                    uiOptions.Textures.TilesetNum = -1;
                    mainForm.MainMapName = "No Map";
                }

                DrawLater();
            }
        }

        /// <summary>
        /// These get injected by Ninject over the constructor.
        /// </summary>
        private readonly ILogger logger;
        private readonly KeyboardManager keyboardManager;
        private readonly ViewInfo viewInfo;
        private readonly MinimapCreator minimapCreator;
        private readonly UiOptions.Options uiOptions;
        private readonly MainForm mainForm;

        private UITimer tmrDraw;
        private UITimer tmrKey;
        private UITimer tmrTool;

        private readonly Label lblMinimap;
        private readonly Label lblTile;
        private readonly Label lblVertex;
        private readonly Label lblPos;

        private bool drawPending = false;

        [Inject]
        internal SettingsManager Settings { get; set; }

        public MainMapView(IKernel kernel, ILoggerFactory logFactory, 
            KeyboardManager kbm, ViewInfo argViewInfo,
            MinimapCreator mmc, UiOptions.Options argUiOptions, MainForm argMainForm)
        {
            kernel.Inject(this); // For GLSurface
            logger = logFactory.GetCurrentClassLogger();
            keyboardManager = kbm;
            viewInfo = argViewInfo;
            viewInfo.MainMapView = this; // They need each other.
            minimapCreator = mmc;
            uiOptions = argUiOptions;
            mainForm = argMainForm;

            var mainLayout = new DynamicLayout();
            mainLayout.AddSeparateRow(
                lblMinimap = new Label { Text = "Minimap" }
            );
            mainLayout.Add(GLSurface, true, true);
            mainLayout.AddSeparateRow(
                lblTile = new Label { },
                null,
                lblVertex = new Label { },
                null,
                lblPos = new Label { }
            );

            Content = mainLayout;
            App.MapGLSurface = this.GLSurface;
            this.GLSurface.Initialized += GLSurface_Initialized;
            SetupEventHandlers();
        }

        private void SetupEventHandlers()
        {
            Settings.PropertyChanged += (object sender, PropertyChangedEventArgs e) =>
                {
#if DEBUG
                    Console.WriteLine("Setting {0} changed ", e.PropertyName);
#endif

                    if( e.PropertyName.StartsWith("Font") )
                    {
                        MakeGlFont();
                    }
                };
        }

        void GLSurface_Initialized(object sender, EventArgs e)
        {
             this.GLSurface.MakeCurrent();
            // Set Vision radius
            App.VisionRadius_2E = 10;
            App.VisionRadius_2E_Changed();

            Matrix3DMath.MatrixSetToPY(App.SunAngleMatrix, new Angles.AnglePY(-22.5D * MathUtil.RadOf1Deg, 157.5D * MathUtil.RadOf1Deg));

            // Make the GL Font.
            MakeGlFont();
        }
        private void MakeGlFont()
        {
            if(!this.GLSurface.IsInitialized)
            {
                return;
            }
            this.GLSurface.MakeCurrent();

            //TODO: Remove depedency on SD try for ETO
            var style = System.Drawing.FontStyle.Regular;
            if( Settings.FontBold )
            {
                style = style | System.Drawing.FontStyle.Bold;
            }
            if( Settings.FontItalic )
            {
                style = style | System.Drawing.FontStyle.Italic;
            }
            App.UnitLabelFont = new GLFont(new System.Drawing.Font(Settings.FontFamily, Settings.FontSize, style, System.Drawing.GraphicsUnit.Pixel));
        }
        protected override void OnLoadComplete(EventArgs lcEventArgs)
        {
            base.OnLoadComplete(lcEventArgs);

            GLSurface.KeyDown += keyboardManager.HandleKeyDown;
            GLSurface.KeyUp += keyboardManager.HandleKeyUp;

            GLSurface.MouseEnter += (sender, e) =>
            {
                GLSurface.Focus();
            };
            GLSurface.MouseDown += viewInfo.HandleMouseDown;
            GLSurface.MouseUp += viewInfo.HandleMouseUp;
            GLSurface.MouseMove += viewInfo.HandleMouseMove;
            GLSurface.MouseMove += HandleMouseMove;
            GLSurface.MouseWheel += viewInfo.HandleMouseWheel;

            GLSurface.LostFocus += viewInfo.HandleLostFocus;
            GLSurface.MouseLeave += viewInfo.HandleMouseLeave;

            GLSurface.Initialized += InitalizeGlSurface;
            GLSurface.SizeChanged += ResizeMapView;

            keyboardManager.KeyDown += HandleKeyDown;

            lblMinimap.MouseDown += delegate
            {
                var menu = CreateMinimapContextMenu();
                menu.Show(lblMinimap);
            };
        }

        private ContextMenu CreateMinimapContextMenu() {
            var menu = new ContextMenu();

            var cmiTextures = new CheckMenuItem(new CheckCommand {
                MenuText = "Show Textures",
                Checked = uiOptions.Minimap.Textures
            });
            cmiTextures.Click += delegate
            {
                uiOptions.Minimap.Textures = !uiOptions.Minimap.Textures;
            };
            menu.Items.Add(cmiTextures);

            var cmiHeights = new CheckMenuItem(new CheckCommand {
                MenuText = "Show Heights",
                Checked = uiOptions.Minimap.Heights
            });
            cmiHeights.Click += delegate
            {
                uiOptions.Minimap.Heights = !uiOptions.Minimap.Heights;
            };
            menu.Items.Add(cmiHeights);

            var cmiCliffs = new CheckMenuItem(new CheckCommand {
                MenuText = "Show Cliffs",
                Checked = uiOptions.Minimap.Cliffs
            });
            cmiCliffs.Click += delegate
            {
                uiOptions.Minimap.Cliffs = !uiOptions.Minimap.Cliffs;
            };
            menu.Items.Add(cmiCliffs);

            var cmiObjects = new CheckMenuItem(new CheckCommand {
                MenuText = "Show Objects",
                Checked = uiOptions.Minimap.Objects
            });
            cmiObjects.Click += delegate
            {
                uiOptions.Minimap.Objects = !uiOptions.Minimap.Objects;
            };
            menu.Items.Add(cmiObjects);

            var cmiGateways = new CheckMenuItem(new CheckCommand {
                MenuText = "Show Gateways",
                Checked = uiOptions.Minimap.Gateways
            });
            cmiGateways.Click += delegate
            {
                uiOptions.Minimap.Gateways = !uiOptions.Minimap.Gateways;
            };
            menu.Items.Add(cmiGateways);

            return menu;
        }

        private void InitalizeGlSurface(object sender, EventArgs e)
        {
            this.GLSurface.MakeCurrent();

            GL.PixelStore(PixelStoreParameter.PackAlignment, 1);
            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
            GL.ClearColor(0.0F, 0.0F, 0.0F, 1.0F);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.ShadeModel(ShadingModel.Smooth);
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
            GL.Enable(EnableCap.DepthTest);

            var ambient = new float[4];
            var specular = new float[4];
            var diffuse = new float[4];

            ambient[0] = 0.333333343F;
            ambient[1] = 0.333333343F;
            ambient[2] = 0.333333343F;
            ambient[3] = 1.0F;
            specular[0] = 0.6666667F;
            specular[1] = 0.6666667F;
            specular[2] = 0.6666667F;
            specular[3] = 1.0F;
            diffuse[0] = 0.75F;
            diffuse[1] = 0.75F;
            diffuse[2] = 0.75F;
            diffuse[3] = 1.0F;
            GL.Light(LightName.Light0, LightParameter.Diffuse, diffuse);
            GL.Light(LightName.Light0, LightParameter.Specular, specular);
            GL.Light(LightName.Light0, LightParameter.Ambient, ambient);

            ambient[0] = 0.25F;
            ambient[1] = 0.25F;
            ambient[2] = 0.25F;
            ambient[3] = 1.0F;
            specular[0] = 0.5F;
            specular[1] = 0.5F;
            specular[2] = 0.5F;
            specular[3] = 1.0F;
            diffuse[0] = 0.5625F;
            diffuse[1] = 0.5625F;
            diffuse[2] = 0.5625F;
            diffuse[3] = 1.0F;
            GL.Light(LightName.Light1, LightParameter.Diffuse, diffuse);
            GL.Light(LightName.Light1, LightParameter.Specular, specular);
            GL.Light(LightName.Light1, LightParameter.Ambient, ambient);

            var mat_diffuse = new float[4];
            var mat_specular = new float[4];
            var mat_ambient = new float[4];
            var mat_shininess = new float[1];

            mat_specular[0] = 0.0F;
            mat_specular[1] = 0.0F;
            mat_specular[2] = 0.0F;
            mat_specular[3] = 0.0F;
            mat_ambient[0] = 1.0F;
            mat_ambient[1] = 1.0F;
            mat_ambient[2] = 1.0F;
            mat_ambient[3] = 1.0F;
            mat_diffuse[0] = 1.0F;
            mat_diffuse[1] = 1.0F;
            mat_diffuse[2] = 1.0F;
            mat_diffuse[3] = 1.0F;
            mat_shininess[0] = 0.0F;

            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Ambient, mat_ambient);
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Specular, mat_specular);
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Diffuse, mat_diffuse);
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Shininess, mat_shininess);

            tmrDraw = new UITimer { Interval = 0.013 }; // Every Millisecond.
            tmrDraw.Elapsed += timedDraw;
            tmrDraw.Start();

            tmrKey = new UITimer { Interval = 0.030 }; // Every 30 milliseconds.
            tmrKey.Elapsed += timedKey;
            tmrKey.Start();

            tmrTool = new UITimer { Interval = 0.1 }; // Every 100 milliseconds.
            tmrTool.Elapsed += timedTool;
            tmrTool.Start();
        }

        private void ResizeMapView(object sender, EventArgs e)
        {
            GLSurface.MakeCurrent();

            var glSize = GLSurface.Size;

            // send the resize event to the Graphics card.
            GL.Viewport(0, 0, glSize.Width, glSize.Height);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.Flush();

            GLSurface.SwapBuffers();

            if( viewInfo.Map != null )
            {
                viewInfo.FovCalc();
            }

            DrawLater();
        }

        public void DrawLater()
        {
            drawPending = true;
        }

        private void timedDraw(object sender, EventArgs e)
        {
            if( !drawPending ||
                !this.GLSurface.IsInitialized )
            {
                return;
            }

            this.GLSurface.MakeCurrent();

            var bgColour = new SRgb();
            if( mainMap == null || mainMap.Tileset == null )
            {
                bgColour.Red = 0.5F;
                bgColour.Green = 0.5F;
                bgColour.Blue = 0.5F;
            }
            else
            {
                bgColour = mainMap.Tileset.BGColour;
            }

            GL.ClearColor(bgColour.Red, bgColour.Green, bgColour.Blue, 1.0F);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            if( mainMap != null ) // Else just clear the screen.
            {
                try
                {
                    MainMap.GLDraw();
                }
                catch( Exception ex )
                {
                    Debugger.Break();
                    logger.Error(ex, "Got an exception");
                }
            }

            GL.Flush();
            this.GLSurface.SwapBuffers();

            drawPending = false;
        }

        private void timedTool(object sender, EventArgs e)
        {
            if( viewInfo.Map == null )
            {
                return;
            }
            this.GLSurface.MakeCurrent();

            viewInfo.TimedTools();
        }

        private void timedKey(object sender, EventArgs e)
        {
            if( viewInfo.Map == null )
            {
                return;
            }
            this.GLSurface.MakeCurrent();

            double Rate = 0;
            double Zoom = 0;
            double Move = 0;
            double Roll = 0;
            double Pan = 0;
            double OrbitRate = 0;

            if( keyboardManager.Keys[KeyboardKeys.ViewFast].Active )
            {
                if( keyboardManager.Keys[KeyboardKeys.ViewSlow].Active )
                {
                    Rate = 8.0D;
                }
                else
                {
                    Rate = 4.0D;
                }
            }
            else if( keyboardManager.Keys[KeyboardKeys.ViewSlow].Active )
            {
                Rate = 0.25D;
            }
            else
            {
                Rate = 1.0D;
            }

            Zoom = tmrKey.Interval * 1000 * 0.002D;
            Move = tmrKey.Interval * 1000 * Rate / 2048.0D;
            Roll = 5.0D * MathUtil.RadOf1Deg;
            Pan = 1.0D / 16.0D;
            OrbitRate = 1.0D / 32.0D;

            viewInfo.TimedActions(Zoom, Move, Pan, Roll, OrbitRate);

            if( MainMap.CheckMessages() )
            {
                DrawLater();
            }
        }

        protected override void Dispose(bool disposing) 
        {
            if(disposing)
            {
                if(GLSurface.IsInitialized)
                {
                    tmrDraw.Stop();
                    tmrDraw = null;

                    tmrKey.Stop();
                    tmrKey = null;
                }
            }

            base.Dispose(disposing);
        }

        private void HandleMouseMove(object sender, MouseEventArgs e)
        {
            var mouseOverTerrain = viewInfo.GetMouseOverTerrain();

            if(mouseOverTerrain == null) // Map is maybe null here
            {
                lblTile.Text = "";
                lblVertex.Text = "";
                lblPos.Text = "";
            } else
            {
                lblTile.Text = string.Format("Tile x:{0}, y:{1}", 
                    mouseOverTerrain.Tile.Normal.X, 
                    mouseOverTerrain.Tile.Normal.Y
                );
                lblVertex.Text = string.Format("Vertex  x:{0}, y:{1}, alt:{2} ({3}x{4})", 
                    mouseOverTerrain.Vertex.Normal.X, 
                    mouseOverTerrain.Vertex.Normal.Y, 
                    mainMap.Terrain.Vertices[mouseOverTerrain.Vertex.Normal.X, mouseOverTerrain.Vertex.Normal.Y].Height * mainMap.HeightMultiplier, 
                    mainMap.Terrain.Vertices[mouseOverTerrain.Vertex.Normal.X, mouseOverTerrain.Vertex.Normal.Y].Height, 
                    mainMap.HeightMultiplier
                );
                lblPos.Text = string.Format("Pos x:{0}, y:{1}, alt:{2}, slope: {3}°", 
                    mouseOverTerrain.Pos.Horizontal.X, 
                    mouseOverTerrain.Pos.Horizontal.Y, 
                    mouseOverTerrain.Pos.Altitude, 
                    Math.Round(mainMap.GetTerrainSlopeAngle(mouseOverTerrain.Pos.Horizontal) / MathUtil.RadOf1Deg * 10.0D) / 10.0D
                );
            }
        }

        private void HandleKeyDown(object sender, KeyboardEventArgs e)
        {
            if(mainMap == null)
            {
                return;
            }

            if(keyboardManager.Keys[KeyboardKeys.VisionRadius6].Active)
            {
                App.VisionRadius_2E = 6;
                App.VisionRadius_2E_Changed();
                DrawLater();
            }
            if(keyboardManager.Keys[KeyboardKeys.VisionRadius7].Active)
            {
                App.VisionRadius_2E = 7;
                App.VisionRadius_2E_Changed();
                DrawLater();
            }
            if(keyboardManager.Keys[KeyboardKeys.VisionRadius8].Active)
            {
                App.VisionRadius_2E = 8;
                App.VisionRadius_2E_Changed();
                DrawLater();
            }
            if(keyboardManager.Keys[KeyboardKeys.VisionRadius9].Active)
            {
                App.VisionRadius_2E = 9;
                App.VisionRadius_2E_Changed();
                DrawLater();
            }
            if(keyboardManager.Keys[KeyboardKeys.VisionRadius10].Active)
            {
                App.VisionRadius_2E = 10;
                App.VisionRadius_2E_Changed();
                DrawLater();
            }
            if(keyboardManager.Keys[KeyboardKeys.VisionRadius11].Active)
            {
                App.VisionRadius_2E = 11;
                App.VisionRadius_2E_Changed();
                DrawLater();
            }
            if(keyboardManager.Keys[KeyboardKeys.VisionRadius12].Active)
            {
                App.VisionRadius_2E = 12;
                App.VisionRadius_2E_Changed();
                DrawLater();
            }
            if(keyboardManager.Keys[KeyboardKeys.VisionRadius13].Active)
            {
                App.VisionRadius_2E = 13;
                App.VisionRadius_2E_Changed();
                DrawLater();
            }
            if(keyboardManager.Keys[KeyboardKeys.VisionRadius14].Active)
            {
                App.VisionRadius_2E = 14;
                App.VisionRadius_2E_Changed();
                DrawLater();
            }
            if(keyboardManager.Keys[KeyboardKeys.VisionRadius15].Active)
            {
                App.VisionRadius_2E = 15;
                App.VisionRadius_2E_Changed();
                DrawLater();
            }

            if (keyboardManager.Keys[KeyboardKeys.ViewMoveMode].Active)
            {
                if ( App.ViewMoveType == ViewMoveType.Free )
                {
                    App.ViewMoveType = ViewMoveType.RTS;
                }
                else if ( App.ViewMoveType == ViewMoveType.RTS )
                {
                    App.ViewMoveType = ViewMoveType.Free;
                }
            }
            if (keyboardManager.Keys[KeyboardKeys.ViewRotateMode].Active)
            {
                App.RTSOrbit = !App.RTSOrbit;
            }
            if (keyboardManager.Keys[KeyboardKeys.ViewReset].Active)
            {
                var matrixA = new Matrix3DMath.Matrix3D();

                viewInfo.FovMultiplierSet(App.SettingsManager.FOVDefault);
                if ( App.ViewMoveType == ViewMoveType.Free )
                {
                    Matrix3DMath.MatrixSetToXAngle(matrixA, Math.Atan(2.0D));
                    viewInfo.ViewAngleSetRotate(matrixA);
                }
                else if ( App.ViewMoveType == ViewMoveType.RTS )
                {
                    Matrix3DMath.MatrixSetToXAngle(matrixA, Math.Atan(2.0D));
                    viewInfo.ViewAngleSetRotate(matrixA);
                }
            }
            if (keyboardManager.Keys[KeyboardKeys.ShowTextures].Active)
            {
                App.Draw_TileTextures = !App.Draw_TileTextures;
                DrawLater();
            }
            if (keyboardManager.Keys[KeyboardKeys.ShowWireframe].Active)
            {
                App.Draw_TileWireframe = !App.Draw_TileWireframe;
                DrawLater();
            }
            if (keyboardManager.Keys[KeyboardKeys.ShowObjects].Active)
            {
                App.Draw_Units = !App.Draw_Units;
                
                var sectorNum = new XYInt();
                for (var y = 0; y <= mainMap.SectorCount.Y - 1; y++ )
                {
                    for (var  x = 0; x <= mainMap.SectorCount.X - 1; x++ )
                    {
                        foreach ( var connection in mainMap.Sectors[x, y].Units )
                        {    
                            var Unit = connection.Unit;
                            if ( Unit.TypeBase.Type == UnitType.PlayerStructure )
                            {
                                if ( ((StructureTypeBase)Unit.TypeBase).StructureBasePlate != null )
                                {
                                    sectorNum.X = x;
                                    sectorNum.Y = y;
                                    mainMap.SectorGraphicsChanges.Changed(sectorNum);
                                    break;
                                }
                            }
                        }
                    }
                }
                mainMap.Update();
                DrawLater();
            }
            if (keyboardManager.Keys[KeyboardKeys.ShowLabels].Active)
            {
                App.Draw_ScriptMarkers = !App.Draw_ScriptMarkers;
                DrawLater();
            }
            if (keyboardManager.Keys[KeyboardKeys.ShowLighting].Active)
            {
                if ( App.Draw_Lighting == DrawLighting.Off )
                {
                    App.Draw_Lighting = DrawLighting.Half;
                }
                else if ( App.Draw_Lighting == DrawLighting.Half )
                {
                    App.Draw_Lighting = DrawLighting.Normal;
                }
                else if ( App.Draw_Lighting == DrawLighting.Normal )
                {
                    App.Draw_Lighting = DrawLighting.Off;
                }
                DrawLater();
            }

            var mouseOverTerrain = viewInfo.GetMouseOverTerrain();
            if (uiOptions.MouseTool == MouseTool.TextureBrush)
            {
                if ( mouseOverTerrain != null )
                {
                    if (keyboardManager.Keys[KeyboardKeys.Clockwise].Active)
                    {
                        viewInfo.ApplyTextureClockwise();
                    }
                    if (keyboardManager.Keys[KeyboardKeys.CounterClockwise].Active)
                    {
                        viewInfo.ApplyTextureCounterClockwise();
                    }
                    if (keyboardManager.Keys[KeyboardKeys.TextureFlip].Active)
                    {
                        viewInfo.ApplyTextureFlipX();
                    }
                    if (keyboardManager.Keys[KeyboardKeys.TriangleFlip].Active)
                    {
                        viewInfo.ApplyTriFlip();
                    }
                }
            }
            if (uiOptions.MouseTool == MouseTool.ObjectSelect)
            {
                if (keyboardManager.Keys[KeyboardKeys.DeleteObjects].Active)
                {
                    if ( mainMap.SelectedUnits.Count > 0 )
                    {
                        foreach ( var unit in mainMap.SelectedUnits.GetItemsAsSimpleList() )
                        {
                            
                            mainMap.UnitRemoveStoreChange(unit.MapLink.ArrayPosition);
                        }
                        Program.frmMainInstance.SelectedObject_Changed();
                        mainMap.UndoStepCreate("Object Deleted");
                        mainMap.Update();
                        DrawLater();
                    }
                }
                if (keyboardManager.Keys[KeyboardKeys.MoveObjects].Active)
                {
                    if ( mouseOverTerrain != null )
                    {
                        if ( mainMap.SelectedUnits.Count > 0 )
                        {
                            var centre = App.CalcUnitsCentrePos(mainMap.SelectedUnits.GetItemsAsSimpleList());
                            var offset = new XYInt();
                            offset.X = Math.Round(Convert.ToDouble((mouseOverTerrain.Pos.Horizontal.X - centre.X) / Constants.TerrainGridSpacing)).ToInt() * Constants.TerrainGridSpacing;
                            offset.Y = Math.Round(Convert.ToDouble((mouseOverTerrain.Pos.Horizontal.Y - centre.Y) / Constants.TerrainGridSpacing)).ToInt() * Constants.TerrainGridSpacing;
                            var objectPosOffset = new clsObjectPosOffset
                                {
                                    Map = mainMap,
                                    Offset = offset
                                };
                            mainMap.SelectedUnitsAction(objectPosOffset);

                            mainMap.UndoStepCreate("Objects Moved");
                            mainMap.Update();
                            Program.frmMainInstance.SelectedObject_Changed();
                            DrawLater();
                        }
                    }
                }
                if (keyboardManager.Keys[KeyboardKeys.Clockwise].Active)
                {
                    var objectRotationOffset = new clsObjectRotationOffset
                        {
                            Map = mainMap,
                            Offset = -90
                        };
                    mainMap.SelectedUnitsAction(objectRotationOffset);
                    mainMap.Update();
                    Program.frmMainInstance.SelectedObject_Changed();
                    mainMap.UndoStepCreate("Object Rotated");
                    DrawLater();
                }
                if (keyboardManager.Keys[KeyboardKeys.CounterClockwise].Active)
                {
                    var objectRotationOffset = new clsObjectRotationOffset
                        {
                            Map = mainMap,
                            Offset = 90
                        };
                    mainMap.SelectedUnitsAction(objectRotationOffset);
                    mainMap.Update();
                    Program.frmMainInstance.SelectedObject_Changed();
                    mainMap.UndoStepCreate("Object Rotated");
                    DrawLater();
                }
            }

            if (keyboardManager.Keys[KeyboardKeys.ObjectSelectTool].Active)
            {
                uiOptions.MouseTool = MouseTool.ObjectSelect;
                DrawLater();
            }
        }
	}
}

