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
using SharpFlame.Mapping;
using SharpFlame.Mapping.Minimap;
using SharpFlame.Mapping.Tools;
using SharpFlame.Maths;
using SharpFlame.Settings;
using SharpFlame.UiOptions;
using SharpFlame.Util;

namespace SharpFlame.Gui.Sections
{
	public class MapPanel : Panel
	{
		public GLSurface GLSurface { get; set; }

		private Map mainMap;
		public Map MainMap { 
			get { return mainMap; }
		}

		[EventSubscription(EventTopics.OnMapLoad, typeof(OnPublisher))]
		public void HandleMapLoad(Map newMap)
		{
			// Unload the previous map, until we have multimap support.
			if( mainMap != null )
			{
				//                    if ( !map.ClosePrompt() )
				//                    {
				//                        return;
				//                    }

				mainMap.Deallocate();
			}

			mainMap = newMap;
			mainMap.IsMainMap = true;
			
			this.ViewInfo.SetGlSize(this.GLSurface.Size);
			
			mainMap.ViewInfo = this.ViewInfo;

			if( mainMap != null )
			{
				mainMap.InitializeUserInput();
				mainMap.SectorGraphicsChanges.SetAllChanged();
				this.UpdateMap();

				// Change the tileset in the TexturesView.
				UiOptions.Textures.TilesetNum = App.Tilesets.IndexOf(mainMap.Tileset);
			}
			else
			{
				UiOptions.Textures.TilesetNum = -1;
			}
			SetViewPort();
			this.minimapGl.HandleMapLoad(mainMap);
			DrawLater();
		}

		/// <summary>
		/// These get injected by Ninject over the constructor.
		/// </summary>
		[Inject]
		internal ILogger Logger { get; set; }

		[Inject]
		internal KeyboardManager KeyboardManager { get; set; }
		
		private MinimapGl minimapGl;
        
		[Inject]
		internal Options UiOptions { get; set; }

		private UITimer timDraw;
		private UITimer timKey;
		private UITimer timTool;

		private readonly Label lblMinimap;
		private readonly Label lblTile;
		private readonly Label lblVertex;
		private readonly Label lblPos;

		private bool drawPending = false;

		[Inject]
		internal SettingsManager Settings { get; set; }

		[Inject]
		internal IEventBroker EventBroker { get; set; }

		[Inject]
		internal ViewInfo ViewInfo { get; set; }

		public MapPanel()
		{
			this.GLSurface = new GLSurface();
            
			var mainLayout = new DynamicLayout();
			mainLayout.AddSeparateRow(
				lblMinimap = new Label { Text = "Minimap" }
				);
			mainLayout.Add(this.GLSurface, true, true);
			mainLayout.AddSeparateRow(
				lblTile = new Label { },
				null,
				lblVertex = new Label { },
				null,
				lblPos = new Label { }
				);

			Content = mainLayout;

			SetupEventHandlers();
		}


		protected override void OnPreLoad(EventArgs e)
		{
			this.minimapGl = new MinimapGl(this.Settings, this.UiOptions, this.GLSurface);
			this.ParentWindow.GotFocus += ParentWindow_GotFocus;
			base.OnPreLoad(e);
		}

		void ParentWindow_GotFocus(object sender, EventArgs e)
		{
			this.DrawLater();
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

			GLSurface.KeyDown += KeyboardManager.HandleKeyDown;
			GLSurface.KeyUp += KeyboardManager.HandleKeyUp;

			GLSurface.MouseEnter += (sender, e) =>
				{
					GLSurface.Focus();
				};
			GLSurface.MouseDown += (sender, args) =>
				{
					this.minimapGl.Suppress = true;
					ViewInfo.HandleMouseDown(sender, args);
				};
			GLSurface.MouseUp += (sender, args) =>
				{
					this.minimapGl.Suppress = false;
					ViewInfo.HandleMouseUp(sender, args);
				};
			GLSurface.MouseMove += ViewInfo.HandleMouseMove;
			GLSurface.MouseMove += HandleMouseMove;
			GLSurface.MouseWheel += ViewInfo.HandleMouseWheel;

			GLSurface.LostFocus += ViewInfo.HandleLostFocus;
			GLSurface.MouseLeave += ViewInfo.HandleMouseLeave;

			GLSurface.Initialized += InitalizeGlSurface;
			GLSurface.SizeChanged += ResizeMapView;

			KeyboardManager.KeyDown += HandleKeyDown;

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
				Checked = UiOptions.MinimapOpts.Textures
			});
			cmiTextures.Click += delegate
				{
					UiOptions.MinimapOpts.Textures = !UiOptions.MinimapOpts.Textures;
				};
			menu.Items.Add(cmiTextures);

			var cmiHeights = new CheckMenuItem(new CheckCommand {
				MenuText = "Show Heights",
				Checked = UiOptions.MinimapOpts.Heights
			});
			cmiHeights.Click += delegate
				{
					UiOptions.MinimapOpts.Heights = !UiOptions.MinimapOpts.Heights;
				};
			menu.Items.Add(cmiHeights);

			var cmiCliffs = new CheckMenuItem(new CheckCommand {
				MenuText = "Show Cliffs",
				Checked = UiOptions.MinimapOpts.Cliffs
			});
			cmiCliffs.Click += delegate
				{
					UiOptions.MinimapOpts.Cliffs = !UiOptions.MinimapOpts.Cliffs;
				};
			menu.Items.Add(cmiCliffs);

			var cmiObjects = new CheckMenuItem(new CheckCommand {
				MenuText = "Show Objects",
				Checked = UiOptions.MinimapOpts.Objects
			});
			cmiObjects.Click += delegate
				{
					UiOptions.MinimapOpts.Objects = !UiOptions.MinimapOpts.Objects;
				};
			menu.Items.Add(cmiObjects);

			var cmiGateways = new CheckMenuItem(new CheckCommand {
				MenuText = "Show Gateways",
				Checked = UiOptions.MinimapOpts.Gateways
			});
			cmiGateways.Click += delegate
				{
					UiOptions.MinimapOpts.Gateways = !UiOptions.MinimapOpts.Gateways;
				};
			menu.Items.Add(cmiGateways);

			return menu;
		}

		private void InitalizeGlSurface(object sender, EventArgs e)
		{
			this.GLSurface.MakeCurrent();

			GL.PixelStore(PixelStoreParameter.PackAlignment, 1);
			GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
			//GL.ClearColor(0.0F, 0.0F, 0.0F, 1.0F);
			GL.ClearColor(OpenTK.Graphics.Color4.CornflowerBlue);
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

			var matDiffuse = new float[4];
			var matSpecular = new float[4];
			var matAmbient = new float[4];
			var matShininess = new float[1];

			matSpecular[0] = 0.0F;
			matSpecular[1] = 0.0F;
			matSpecular[2] = 0.0F;
			matSpecular[3] = 0.0F;
			matAmbient[0] = 1.0F;
			matAmbient[1] = 1.0F;
			matAmbient[2] = 1.0F;
			matAmbient[3] = 1.0F;
			matDiffuse[0] = 1.0F;
			matDiffuse[1] = 1.0F;
			matDiffuse[2] = 1.0F;
			matDiffuse[3] = 1.0F;
			matShininess[0] = 0.0F;

			GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Ambient, matAmbient);
			GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Specular, matSpecular);
			GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Diffuse, matDiffuse);
			GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Shininess, matShininess);

			timDraw = new UITimer { Interval = 0.013 }; // Every Millisecond.
			timDraw.Elapsed += timDraw_Elapsed;
			timDraw.Start();

			timKey = new UITimer { Interval = 0.030 }; // Every 30 milliseconds.
			timKey.Elapsed += timKey_Elapsed;
			timKey.Start();

			timTool = new UITimer { Interval = 0.1 }; // Every 100 milliseconds.
			timTool.Elapsed += timTool_Elapsed;
			timTool.Start();

			// Set Vision radius
			App.VisionRadius_2E = 10;
			App.VisionRadius_2E_Changed();

			Matrix3DMath.MatrixSetToPY(App.SunAngleMatrix, new Angles.AnglePY(-22.5D * MathUtil.RadOf1Deg, 157.5D * MathUtil.RadOf1Deg));

			// Make the GL Font.
			MakeGlFont();
			SetViewPort();
			DrawLater();
		}

		private void ResizeMapView(object sender, EventArgs e)
		{
			SetViewPort();
			DrawLater();
		}

		private void SetViewPort()
		{
			if( !this.GLSurface.IsInitialized )
				return;

			this.GLSurface.MakeCurrent();

			var glSize = GLSurface.Size;

			// send the resize event to the Graphics card.
			GL.Viewport(0, 0, glSize.Width, glSize.Height);
			GL.Clear(ClearBufferMask.ColorBufferBit);
			GL.Flush();

			this.GLSurface.SwapBuffers();

			if( this.mainMap != null )
			{
				this.ViewInfo.SetGlSize(glSize);
				this.ViewInfo.FovCalc();
			}
		}

		
		private void DrawPlaceHolder()
		{
			if( !this.GLSurface.IsInitialized )
				return;

			this.GLSurface.MakeCurrent();
	
			GL.ClearColor(OpenTK.Graphics.Color4.DimGray);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			
			this.GLSurface.SwapBuffers();
		}

		[EventSubscription(EventTopics.OnMapDrawLater, typeof(OnPublisher))]
		public void HandleDrawLater(EventArgs e)
		{
			this.DrawLater();
		}

		private void DrawLater()
		{
			drawPending = true;
		}

		private void timDraw_Elapsed(object sender, EventArgs e)
		{
			if( !drawPending || !this.GLSurface.IsInitialized )
			{
				return;
			}
			if( this.mainMap == null )
			{
				DrawPlaceHolder();
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
					MainMap.GLDraw(new DrawContext
						{
							GlSize = this.GLSurface.Size,
							MinimapGl = this.minimapGl,
							ViewInfo = this.ViewInfo,
							Options = this.UiOptions
						});
				}
				catch( Exception ex )
				{
					Debugger.Break();
					Logger.Error(ex, "Got an exception");
				}
			}

			GL.Flush();
			this.GLSurface.SwapBuffers();

			drawPending = false;
		}

		private void timTool_Elapsed(object sender, EventArgs e)
		{
			if( this.mainMap == null )
			{
				return;
			}
			this.GLSurface.MakeCurrent();

			ViewInfo.TimedTools();
		}

		private void timKey_Elapsed(object sender, EventArgs e)
		{
			if( this.mainMap == null )
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

			if( KeyboardManager.Keys[KeyboardKeys.ViewFast].Active )
			{
				if( KeyboardManager.Keys[KeyboardKeys.ViewSlow].Active )
				{
					Rate = 8.0D;
				}
				else
				{
					Rate = 4.0D;
				}
			}
			else if( KeyboardManager.Keys[KeyboardKeys.ViewSlow].Active )
			{
				Rate = 0.25D;
			}
			else
			{
				Rate = 1.0D;
			}

			Zoom = timKey.Interval * 1000 * 0.002D;
			Move = timKey.Interval * 1000 * Rate / 2048.0D;
			Roll = 5.0D * MathUtil.RadOf1Deg;
			Pan = 1.0D / 16.0D;
			OrbitRate = 1.0D / 32.0D;

			ViewInfo.TimedActions(Zoom, Move, Pan, Roll, OrbitRate);

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
					timDraw.Stop();
					timDraw = null;

					timKey.Stop();
					timKey = null;
				}
			}

			base.Dispose(disposing);
		}

		private void HandleMouseMove(object sender, MouseEventArgs e)
		{
			var mouseOverTerrain = ViewInfo.GetMouseOverTerrain();

			if(mouseOverTerrain == null) // Map is maybe null here
			{
				lblTile.Text = "Tile x: -, y: -";
				lblVertex.Text = "Vertex x: -, y: -";
				lblPos.Text = "Pos x: -, y: -, alt: -, slope: -";
			}
			else
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

			if(KeyboardManager.Keys[KeyboardKeys.VisionRadius6].Active)
			{
				App.VisionRadius_2E = 6;
				App.VisionRadius_2E_Changed();
				DrawLater();
			}
			if(KeyboardManager.Keys[KeyboardKeys.VisionRadius7].Active)
			{
				App.VisionRadius_2E = 7;
				App.VisionRadius_2E_Changed();
				DrawLater();
			}
			if(KeyboardManager.Keys[KeyboardKeys.VisionRadius8].Active)
			{
				App.VisionRadius_2E = 8;
				App.VisionRadius_2E_Changed();
				DrawLater();
			}
			if(KeyboardManager.Keys[KeyboardKeys.VisionRadius9].Active)
			{
				App.VisionRadius_2E = 9;
				App.VisionRadius_2E_Changed();
				DrawLater();
			}
			if(KeyboardManager.Keys[KeyboardKeys.VisionRadius10].Active)
			{
				App.VisionRadius_2E = 10;
				App.VisionRadius_2E_Changed();
				DrawLater();
			}
			if(KeyboardManager.Keys[KeyboardKeys.VisionRadius11].Active)
			{
				App.VisionRadius_2E = 11;
				App.VisionRadius_2E_Changed();
				DrawLater();
			}
			if(KeyboardManager.Keys[KeyboardKeys.VisionRadius12].Active)
			{
				App.VisionRadius_2E = 12;
				App.VisionRadius_2E_Changed();
				DrawLater();
			}
			if(KeyboardManager.Keys[KeyboardKeys.VisionRadius13].Active)
			{
				App.VisionRadius_2E = 13;
				App.VisionRadius_2E_Changed();
				DrawLater();
			}
			if(KeyboardManager.Keys[KeyboardKeys.VisionRadius14].Active)
			{
				App.VisionRadius_2E = 14;
				App.VisionRadius_2E_Changed();
				DrawLater();
			}
			if(KeyboardManager.Keys[KeyboardKeys.VisionRadius15].Active)
			{
				App.VisionRadius_2E = 15;
				App.VisionRadius_2E_Changed();
				DrawLater();
			}

			if (KeyboardManager.Keys[KeyboardKeys.ViewMoveMode].Active)
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
			if (KeyboardManager.Keys[KeyboardKeys.ViewRotateMode].Active)
			{
				App.RTSOrbit = !App.RTSOrbit;
			}
			if (KeyboardManager.Keys[KeyboardKeys.ViewReset].Active)
			{
				var matrixA = new Matrix3DMath.Matrix3D();

				ViewInfo.FovMultiplierSet(App.SettingsManager.FOVDefault);
				if ( App.ViewMoveType == ViewMoveType.Free )
				{
					Matrix3DMath.MatrixSetToXAngle(matrixA, Math.Atan(2.0D));
					ViewInfo.ViewAngleSetRotate(matrixA);
				}
				else if ( App.ViewMoveType == ViewMoveType.RTS )
				{
					Matrix3DMath.MatrixSetToXAngle(matrixA, Math.Atan(2.0D));
					ViewInfo.ViewAngleSetRotate(matrixA);
				}
			}
			if (KeyboardManager.Keys[KeyboardKeys.ShowTextures].Active)
			{
				App.Draw_TileTextures = !App.Draw_TileTextures;
				DrawLater();
			}
			if (KeyboardManager.Keys[KeyboardKeys.ShowWireframe].Active)
			{
				App.Draw_TileWireframe = !App.Draw_TileWireframe;
				DrawLater();
			}
			if (KeyboardManager.Keys[KeyboardKeys.ShowObjects].Active)
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
				this.UpdateMap();
				this.DrawLater();
			}
			if (KeyboardManager.Keys[KeyboardKeys.ShowLabels].Active)
			{
				App.Draw_ScriptMarkers = !App.Draw_ScriptMarkers;
				DrawLater();
			}
			if (KeyboardManager.Keys[KeyboardKeys.ShowLighting].Active)
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

			var mouseOverTerrain = ViewInfo.GetMouseOverTerrain();
			if (UiOptions.MouseTool == MouseTool.TextureBrush)
			{
				if ( mouseOverTerrain != null )
				{
					if (KeyboardManager.Keys[KeyboardKeys.Clockwise].Active)
					{
						ViewInfo.ApplyTextureClockwise();
					}
					if (KeyboardManager.Keys[KeyboardKeys.CounterClockwise].Active)
					{
						ViewInfo.ApplyTextureCounterClockwise();
					}
					if (KeyboardManager.Keys[KeyboardKeys.TextureFlip].Active)
					{
						ViewInfo.ApplyTextureFlipX();
					}
					if (KeyboardManager.Keys[KeyboardKeys.TriangleFlip].Active)
					{
						ViewInfo.ApplyTriFlip();
					}
				}
			}
			if (UiOptions.MouseTool == MouseTool.ObjectSelect)
			{
				if (KeyboardManager.Keys[KeyboardKeys.DeleteObjects].Active)
				{
					if ( mainMap.SelectedUnits.Count > 0 )
					{
						foreach ( var unit in mainMap.SelectedUnits.GetItemsAsSimpleList() )
						{
                            
							mainMap.UnitRemoveStoreChange(unit.MapLink.ArrayPosition);
						}
						Program.frmMainInstance.SelectedObject_Changed();
						mainMap.UndoStepCreate("Object Deleted");
						this.UpdateMap();
						this.DrawLater();
					}
				}
				if (KeyboardManager.Keys[KeyboardKeys.MoveObjects].Active)
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
							this.UpdateMap();
							Program.frmMainInstance.SelectedObject_Changed();
							this.DrawLater();
						}
					}
				}
				if (KeyboardManager.Keys[KeyboardKeys.Clockwise].Active)
				{
					var objectRotationOffset = new clsObjectRotationOffset
						{
							Map = mainMap,
							Offset = -90
						};
					mainMap.SelectedUnitsAction(objectRotationOffset);
					this.UpdateMap();
					Program.frmMainInstance.SelectedObject_Changed();
					mainMap.UndoStepCreate("Object Rotated");
					this.DrawLater();
				}
				if (KeyboardManager.Keys[KeyboardKeys.CounterClockwise].Active)
				{
					var objectRotationOffset = new clsObjectRotationOffset
						{
							Map = mainMap,
							Offset = 90
						};
					mainMap.SelectedUnitsAction(objectRotationOffset);
					this.UpdateMap();
					Program.frmMainInstance.SelectedObject_Changed();
					mainMap.UndoStepCreate("Object Rotated");
					this.DrawLater();
				}
			}

			if (KeyboardManager.Keys[KeyboardKeys.ObjectSelectTool].Active)
			{
				UiOptions.MouseTool = MouseTool.ObjectSelect;
				DrawLater();
			}
		}

		public void RefreshMinimap()
		{
			this.minimapGl.Refresh = true;
		}

		[EventSubscription(EventTopics.OnMapUpdate, typeof(OnPublisher))]
		public void HandleMapUpdate(EventArgs e)
		{
			this.UpdateMap();
		}

		private void UpdateMap()
		{
			this.mainMap.Update(this.minimapGl);
		}
	}
}