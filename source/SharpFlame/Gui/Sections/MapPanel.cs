using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Appccelerate.EventBroker;
using Appccelerate.EventBroker.Handlers;
using Eto.Forms;
using Eto.Gl;
using Ninject;
using Ninject.Extensions.Logging;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using SharpFlame.Core;
using SharpFlame.Core.Domain;
using SharpFlame.Core.Domain.Colors;
using SharpFlame.Core.Extensions;
using SharpFlame.Domain;
using SharpFlame.Graphics;
using SharpFlame.Graphics.OpenGL;
using SharpFlame.Gui.Actions;
using SharpFlame.Gui.Controls;
using SharpFlame.Mapping;
using SharpFlame.Mapping.Minimap;
using SharpFlame.Mapping.Tiles;
using SharpFlame.Mapping.Tools;
using SharpFlame.Maths;
using SharpFlame.MouseTools;
using SharpFlame.Settings;
using SharpFlame.Util;

namespace SharpFlame.Gui.Sections
{
	public class MapPanel : Panel
	{
		public class MinimapMenu : ContextMenu
		{
			public MinimapMenu()
			{
				XomlReader.Load(this);
			}
			protected void Click(object sender, EventArgs e)
			{
				var chk = sender as CheckMenuItem;
				chk.Checked = !chk.Checked;
				chk.UpdateBindings();
			}
			public CheckMenuItem Textures { get; set; }
			public CheckMenuItem Heights { get; set; }
			public CheckMenuItem Cliffs { get; set; }
			public CheckMenuItem Objects { get; set; }
			public CheckMenuItem Gateways { get; set; }
		}

		public class PasteOptions : ContextMenu
		{
			public PasteOptions()
			{
				XomlReader.Load(this);
			}

			protected void Toggle(object sender, EventArgs e)
			{
				var chk = sender as CheckMenuItem;
				chk.Checked = !chk.Checked;
				chk.UpdateBindings();
			}
			protected void ExclusiveToggle(object sender, EventArgs e)
			{
				//clear
				this.RotateAllObjects.Checked = false;
				this.RotateWallsOnly.Checked = false;
				this.NoObjectRotation.Checked = false;

				var chk = sender as CheckMenuItem;
				chk.Checked = !chk.Checked;

				this.RotateAllObjects.UpdateBindings();
				this.RotateWallsOnly.UpdateBindings();
				this.NoObjectRotation.UpdateBindings();
			}

			public ObjectRotateMode PasteRotateObjects
			{
				get
				{
					if( this.RotateAllObjects.Checked )
						return ObjectRotateMode.All;
					if( this.RotateWallsOnly.Checked)
						return ObjectRotateMode.Walls;
					if( this.NoObjectRotation.Checked)
						return ObjectRotateMode.None;
					return ObjectRotateMode.None;
				}
			}

			public CheckMenuItem RotateAllObjects { get; set; }
			public CheckMenuItem RotateWallsOnly { get; set; }
			public CheckMenuItem NoObjectRotation { get; set; }

			public CheckMenuItem PasteHeights { get; set; }
			public CheckMenuItem PasteTextures { get; set; }
			public CheckMenuItem PasteObjects { get; set; }
			public CheckMenuItem PasteGateways { get; set; }
			public CheckMenuItem DeleteExistingObjects { get; set; }
			public CheckMenuItem DeleteExistingGateways { get; set; }
		}

		[EventPublication(EventTopics.OnTextureDrawLater)]
		public event EventHandler OnTextureDrawLater;

		public event EventHandler OnSaveFMap;

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
			this.panel.Content = this.GLSurface;

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
				ToolOptions.Textures.TilesetNum = App.Tilesets.IndexOf(mainMap.Tileset);
			}
			else
			{
				ToolOptions.Textures.TilesetNum = -1;
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
		internal ToolOptions ToolOptions { get; set; }

		private UITimer timDraw;

		private UITimer timKey;

		private UITimer timTool;

		protected Label lblTile;

		protected Label lblVertex;

		protected Label lblPos;

		private bool drawPending = false;

		private EmptyPlaceHolder emptyPlaceHolder;

		protected Panel panel;

		protected MinimapMenu miniMenu;

		protected PasteOptions pasteOptions;

		protected ImageButton cmdGateways;

		protected ImageButton cmdDrawAutoTexture;

		protected ImageButton cmdDrawTileOrentation;

		protected ImageButton cmdSave;

		protected ObjectRotateMode pasteRotateObjects = ObjectRotateMode.Walls;

		[Inject]
		internal SettingsManager Settings { get; set; }

		[Inject]
		internal IEventBroker EventBroker { get; set; }

		[Inject]
		internal ViewInfo ViewInfo { get; set; }

		public MapPanel()
		{
			XomlReader.Load(this);

			miniMenu = new MinimapMenu();
			pasteOptions = new PasteOptions();
			emptyPlaceHolder = new EmptyPlaceHolder();

		    this.GLSurface = new GLSurface();
		    this.panel.Content = this.emptyPlaceHolder;

			SetupEventHandlers();
			SetupBindings();
		}

		protected void MapPanelTool_Selection(object sender, EventArgs e)
		{
			this.ToolOptions.MouseTool = MouseTool.TerrainSelect;
		}

		protected void MapPanelTool_SelectionCopy(object sender, EventArgs e)
		{
			var map = MainMap;

			if (map == null)
			{
				return;
			}
			if (map.SelectedAreaVertexA == null || map.SelectedAreaVertexB == null)
			{
				return;
			}
			if (App.Copied_Map != null)
			{
				App.Copied_Map.Deallocate();
			}
			var area = new XYInt();
			var start = new XYInt();
			var finish = new XYInt();
			MathUtil.ReorderXY(map.SelectedAreaVertexA, map.SelectedAreaVertexB, ref start, ref finish);
			area.X = finish.X - start.X;
			area.Y = finish.Y - start.Y;
			App.Copied_Map = App.Kernel.Get<Map>().Copy(map, start, area);
		}

		protected void MapPanelTool_SelectionPaste(object sender, EventArgs e)
		{
			var map = MainMap;

			if (map == null)
			{
				return;
			}
			if (map.SelectedAreaVertexA == null || map.SelectedAreaVertexB == null)
			{
				return;
			}
			if (App.Copied_Map == null)
			{
				MessageBox.Show("Nothing to paste.");
				return;
			}
			if (
				!(this.pasteOptions.PasteHeights.Checked
				  || this.pasteOptions.PasteTextures.Checked
				  || this.pasteOptions.PasteObjects.Checked
				  || this.pasteOptions.DeleteExistingObjects.Checked
				  || this.pasteOptions.PasteGateways.Checked
				  || this.pasteOptions.DeleteExistingGateways.Checked))
			{
				return;
			}
			var area = new XYInt();
			var start = new XYInt();
			var finish = new XYInt();
			MathUtil.ReorderXY(map.SelectedAreaVertexA, map.SelectedAreaVertexB, ref start, ref finish);
			area.X = finish.X - start.X;
			area.Y = finish.Y - start.Y;
			map.MapInsert(
				App.Copied_Map,
				start,
				area,
				this.pasteOptions.PasteHeights.Checked,
				this.pasteOptions.PasteTextures.Checked,
				this.pasteOptions.PasteObjects.Checked,
				this.pasteOptions.DeleteExistingObjects.Checked,
				this.pasteOptions.PasteGateways.Checked,
				this.pasteOptions.DeleteExistingGateways.Checked);

			this.EventBroker.SelectedUnitsChanged(this);

			map.UndoStepCreate("Paste");

			this.EventBroker.DrawLater(this);
		}

		protected void MapPanelTool_SelectionPasteOptions(object sender, EventArgs e)
		{
			this.pasteOptions.Show((Control)sender);
		}

		protected void MapPanelTool_SelectionRotateAntiClockwise(object sender, EventArgs e)
		{
			if (App.Copied_Map == null)
			{
				MessageBox.Show("Nothing to rotate.");
				return;
			}

			App.Copied_Map.Rotate(TileUtil.CounterClockwise, this.pasteOptions.PasteRotateObjects);
		}
		protected void MapPanelTool_SelectionRotateClockwise(object sender, EventArgs e)
		{
			if (App.Copied_Map == null)
			{
				MessageBox.Show("Nothing to rotate.");
				return;
			}

			App.Copied_Map.Rotate(TileUtil.Clockwise, this.pasteOptions.PasteRotateObjects);
		}
		protected void MapPanelTool_SelectionFlipX(object sender, EventArgs e)
		{
			if (App.Copied_Map == null)
			{
				MessageBox.Show("Nothing to flip.");
				return;
			}

			App.Copied_Map.Rotate(TileUtil.FlipX, this.pasteOptions.PasteRotateObjects);
		}
		protected void MapPanelTool_ObjectsSelect(object sender, EventArgs e)
		{
			var map = MainMap;

			if (map == null)
			{
				return;
			}

			if (map.SelectedAreaVertexA == null || map.SelectedAreaVertexB == null)
			{
				return;
			}
			var start = new XYInt();
			var finish = new XYInt();

			MathUtil.ReorderXY(map.SelectedAreaVertexA, map.SelectedAreaVertexB, ref start, ref finish);
			for (var i = 0; i <= map.Units.Count - 1; i++)
			{
				if (App.PosIsWithinTileArea(map.Units[i].Pos.Horizontal, start, finish))
				{
					if (!map.Units[i].MapSelectedUnitLink.IsConnected)
					{
						map.Units[i].MapSelectedUnitLink.Connect(map.SelectedUnits);
					}
				}
			}
			this.EventBroker.SelectedUnitsChanged(this);
			this.ToolOptions.MouseTool = MouseTool.ObjectSelect;
			this.EventBroker.DrawLater(this);
		}
		protected void MapPanelTool_Gateways(object sender, EventArgs e)
		{
			if (this.ToolOptions.MouseTool == MouseTool.Gateways)
			{
				App.Draw_Gateways = false;
				this.ToolOptions.MouseTool = MouseTool.ObjectSelect;
				this.cmdGateways.Pressed = false;
				this.cmdGateways.UpdateBindings(BindingUpdateMode.Destination);
				//this.cmdGateways.Toggle = false;
				//tsbGateways.Checked = false;
			}
			else
			{
				App.Draw_Gateways = true;
				this.ToolOptions.MouseTool = MouseTool.Gateways;
				this.cmdGateways.Pressed = true;
				this.cmdGateways.UpdateBindings(BindingUpdateMode.Destination);
				//this.cmdGateways.Toggle = true;
				//tsbGateways.Checked = true;
			}
			var map = MainMap;
			if (map != null)
			{
				map.SelectedTileA = null;
				map.SelectedTileB = null;
				this.EventBroker.DrawLater(this);
			}
		}
		protected void MapPanelTool_DrawAutoTexture(object sender, EventArgs e)
		{
			this.cmdDrawAutoTexture.Toggle = !this.cmdDrawAutoTexture.Toggle;
			this.cmdDrawAutoTexture.Pressed = this.cmdDrawAutoTexture.Toggle;
            this.cmdDrawAutoTexture.UpdateBindings();
			this.EventBroker.DrawLater(this);
		}

		protected void MapPanelTool_DrawTileOrentation(object sender, EventArgs e)
		{
			this.cmdDrawTileOrentation.Toggle = !this.cmdDrawTileOrentation.Toggle;
			this.cmdDrawTileOrentation.Pressed = this.cmdDrawTileOrentation.Toggle;
			this.cmdDrawTileOrentation.UpdateBindings();

			this.EventBroker.DrawLater(this);
			this.OnTextureDrawLater?.Invoke(this, EventArgs.Empty);
		}

		protected void MapPanelTool_Save(object sender, EventArgs e)
		{
			this.OnSaveFMap?.Invoke(this, EventArgs.Empty);
			//this.cmdSave.Enabled = this.mainMap.ChangedSinceSave;
		}

		protected void minimapOptions_Click(object sender, EventArgs e)
		{
			miniMenu.Show((Control)sender);
		}

		protected override void OnPreLoad(EventArgs e)
		{
			this.minimapGl = new MinimapGl(this.Settings, this.ToolOptions, this.GLSurface);
			this.ParentWindow.GotFocus += ParentWindow_GotFocus;
			base.OnPreLoad(e);
		}

		void ParentWindow_GotFocus(object sender, EventArgs e)
		{
			this.DrawLater();
		}

		private void SetupBindings()
		{
			this.miniMenu.Textures.Bind(cm => cm.Checked, Binding.Property(ToolOptions.MinimapOpts, m => m.Textures));
			this.miniMenu.Heights.Bind(cm => cm.Checked, Binding.Property(ToolOptions.MinimapOpts, m => m.Heights));
			this.miniMenu.Cliffs.Bind(cm => cm.Checked, Binding.Property(ToolOptions.MinimapOpts, m => m.Cliffs));
			this.miniMenu.Objects.Bind(cm => cm.Checked, Binding.Property(ToolOptions.MinimapOpts, m => m.Objects));
			this.miniMenu.Gateways.Bind(cm => cm.Checked, Binding.Property(ToolOptions.MinimapOpts, m => m.Gateways));

			this.cmdGateways.Bind(ib => ib.Toggle, Binding.Delegate(() => App.Draw_Gateways, val => App.Draw_Gateways = val ));
			this.cmdDrawAutoTexture.Bind(ib => ib.Toggle, Binding.Delegate(() => App.Draw_VertexTerrain, val => App.Draw_VertexTerrain = val));
			this.cmdDrawTileOrentation.Bind(ib => ib.Toggle, Binding.Delegate(() => App.DisplayTileOrientation, val => App.DisplayTileOrientation = val));
			
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

			this.pasteOptions.RotateAllObjects.Click += (sender, args) => this.pasteRotateObjects = ObjectRotateMode.All;
			this.pasteOptions.RotateWallsOnly.Click += (sender, args) => this.pasteRotateObjects = ObjectRotateMode.Walls;
			this.pasteOptions.NoObjectRotation.Click += (sender, args) => this.pasteRotateObjects = ObjectRotateMode.None;
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
                    //make sure this manager sees the mouse event first
                    //to get modifers such as CTRL/ALT/SHIFT and activate
                    //any keys necessary before ViewInfo finds out
                    //and queries if CTRL/ALT/SHIFT is active.
			        KeyboardManager.HandleMouseDown(sender, args);
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

			GLSurface.GLInitalized += InitalizeGlSurface;
			GLSurface.SizeChanged += ResizeMapView;

			KeyboardManager.KeyDown += HandleKeyDown;
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
			//SetViewPort();
			//DrawLater();
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

		private Eto.Drawing.Size GLSize
		{
			get { return this.GLSurface.Size; }
		}
		
		private void DrawPlaceHolder()
		{
			if( !this.GLSurface.IsInitialized )
				return;

			this.panel.Content = this.emptyPlaceHolder;
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
							ToolOptions = this.ToolOptions
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

			if( KeyboardManager.Commands[CommandName.ViewFast].Active )
			{
				if( KeyboardManager.Commands[CommandName.ViewSlow].Active )
				{
					Rate = 8.0D;
				}
				else
				{
					Rate = 4.0D;
				}
			}
			else if( KeyboardManager.Commands[CommandName.ViewSlow].Active )
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

			if(KeyboardManager.Commands[CommandName.VisionRadius6].Active)
			{
				App.VisionRadius_2E = 6;
				App.VisionRadius_2E_Changed();
				DrawLater();
			}
			if(KeyboardManager.Commands[CommandName.VisionRadius7].Active)
			{
				App.VisionRadius_2E = 7;
				App.VisionRadius_2E_Changed();
				DrawLater();
			}
			if(KeyboardManager.Commands[CommandName.VisionRadius8].Active)
			{
				App.VisionRadius_2E = 8;
				App.VisionRadius_2E_Changed();
				DrawLater();
			}
			if(KeyboardManager.Commands[CommandName.VisionRadius9].Active)
			{
				App.VisionRadius_2E = 9;
				App.VisionRadius_2E_Changed();
				DrawLater();
			}
			if(KeyboardManager.Commands[CommandName.VisionRadius10].Active)
			{
				App.VisionRadius_2E = 10;
				App.VisionRadius_2E_Changed();
				DrawLater();
			}
			if(KeyboardManager.Commands[CommandName.VisionRadius11].Active)
			{
				App.VisionRadius_2E = 11;
				App.VisionRadius_2E_Changed();
				DrawLater();
			}
			if(KeyboardManager.Commands[CommandName.VisionRadius12].Active)
			{
				App.VisionRadius_2E = 12;
				App.VisionRadius_2E_Changed();
				DrawLater();
			}
			if(KeyboardManager.Commands[CommandName.VisionRadius13].Active)
			{
				App.VisionRadius_2E = 13;
				App.VisionRadius_2E_Changed();
				DrawLater();
			}
			if(KeyboardManager.Commands[CommandName.VisionRadius14].Active)
			{
				App.VisionRadius_2E = 14;
				App.VisionRadius_2E_Changed();
				DrawLater();
			}
			if(KeyboardManager.Commands[CommandName.VisionRadius15].Active)
			{
				App.VisionRadius_2E = 15;
				App.VisionRadius_2E_Changed();
				DrawLater();
			}

			if (KeyboardManager.Commands[CommandName.ViewMoveMode].Active)
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
			if (KeyboardManager.Commands[CommandName.ViewRotateMode].Active)
			{
				App.RTSOrbit = !App.RTSOrbit;
			}
			if (KeyboardManager.Commands[CommandName.ViewReset].Active)
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
			if (KeyboardManager.Commands[CommandName.ShowTextures].Active)
			{
				App.Draw_TileTextures = !App.Draw_TileTextures;
				DrawLater();
			}
			if (KeyboardManager.Commands[CommandName.ShowWireframe].Active)
			{
				App.Draw_TileWireframe = !App.Draw_TileWireframe;
				DrawLater();
			}
			if (KeyboardManager.Commands[CommandName.ShowObjects].Active)
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
			if (KeyboardManager.Commands[CommandName.ShowLabels].Active)
			{
				App.Draw_ScriptMarkers = !App.Draw_ScriptMarkers;
				DrawLater();
			}
			if (KeyboardManager.Commands[CommandName.ShowLighting].Active)
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
			if (ToolOptions.MouseTool == MouseTool.TextureBrush)
			{
				if ( mouseOverTerrain != null )
				{
					if (KeyboardManager.Commands[CommandName.Clockwise].Active)
					{
						ViewInfo.ApplyTextureClockwise();
					}
					if (KeyboardManager.Commands[CommandName.CounterClockwise].Active)
					{
						ViewInfo.ApplyTextureCounterClockwise();
					}
					if (KeyboardManager.Commands[CommandName.TextureFlip].Active)
					{
						ViewInfo.ApplyTextureFlipX();
					}
					if (KeyboardManager.Commands[CommandName.TriangleFlip].Active)
					{
						ViewInfo.ApplyTriFlip();
					}
				}
			}
			if (ToolOptions.MouseTool == MouseTool.ObjectSelect)
			{
				if (KeyboardManager.Commands[CommandName.DeleteObjects].Active)
				{
					if ( mainMap.SelectedUnits.Count > 0 )
					{
						foreach ( var unit in mainMap.SelectedUnits.CopyList() )
						{
                            
							mainMap.UnitRemoveStoreChange(unit.MapLink.Position);
						}
						Program.frmMainInstance.SelectedObject_Changed();
						mainMap.UndoStepCreate("Object Deleted");
						this.UpdateMap();
						this.DrawLater();
					}
				}
				if (KeyboardManager.Commands[CommandName.MoveObjects].Active)
				{
					if ( mouseOverTerrain != null )
					{
						if ( mainMap.SelectedUnits.Count > 0 )
						{
							var centre = App.CalcUnitsCentrePos(mainMap.SelectedUnits.CopyList());
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
				if (KeyboardManager.Commands[CommandName.Clockwise].Active)
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
				if (KeyboardManager.Commands[CommandName.CounterClockwise].Active)
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

			if (KeyboardManager.Commands[CommandName.ObjectSelectTool].Active)
			{
				ToolOptions.MouseTool = MouseTool.ObjectSelect;
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