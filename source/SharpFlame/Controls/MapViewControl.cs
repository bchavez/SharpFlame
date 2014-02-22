#region

using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using SharpFlame.AppSettings;
using SharpFlame.Colors;
using SharpFlame.Core.Domain;
using SharpFlame.Core.Domain.Colors;
using SharpFlame.Domain;
using SharpFlame.Graphics.OpenGL;
using SharpFlame.Mapping;
using SharpFlame.Mapping.Objects;
using SharpFlame.Mapping.Tools;
using SharpFlame.Maths;
using SharpFlame.Util;
using Message = SharpFlame.Mapping.Message;

#endregion

namespace SharpFlame.Controls
{
    public partial class MapViewControl
    {
        private readonly ContextMenuStrip listSelect;
        private readonly frmMain owner;
        private readonly Timer tmrDraw;
        private readonly Timer tmrDrawDelay;

        public bool DrawPending;

        //public float GLSize_XPerY; //seems redundant, since OpenGLControl has a field called AspectRatio

        public bool DrawViewEnabled = false;

        private Timer GLInitializeDelayTimer;
        public XYInt GLSize;
        public bool IsGLInitialized = false;
        private bool listSelectIsPicker;
        private ToolStripItem[] listSelectItems = new ToolStripItem[0];

        public GLControl OpenGLControl;
        public Timer UndoMessageTimer;

        public MapViewControl(frmMain Owner)
        {
            owner = Owner;

            GLSize = new XYInt(0, 0);

            InitializeComponent();

            listSelect = new ContextMenuStrip();
            listSelect.ItemClicked += ListSelect_Click;
            listSelect.Closed += ListSelect_Close;
            UndoMessageTimer = new Timer();
            UndoMessageTimer.Tick += RemoveUndoMessage;

            OpenGLControl = Program.OpenGL1;
            pnlDraw.Controls.Add(OpenGLControl);

            GLInitializeDelayTimer = new Timer();
            GLInitializeDelayTimer.Interval = 50;
            GLInitializeDelayTimer.Tick += GLInitialize;
            GLInitializeDelayTimer.Enabled = true;

            tmrDraw = new Timer();
            tmrDraw.Tick += tmrDraw_Tick;
            tmrDraw.Interval = 1;

            tmrDrawDelay = new Timer();
            tmrDrawDelay.Tick += tmrDrawDelay_Tick;
            tmrDrawDelay.Interval = 30;

            UndoMessageTimer.Interval = 4000;
        }

        private Map MainMap
        {
            get { return owner.MainMap; }
        }

        public void ResizeOpenGL()
        {
            if ( OpenGLControl.Context == null )
            {
                return;
            }

            OpenGLControl.Width = pnlDraw.Width;
            OpenGLControl.Height = pnlDraw.Height;
        }

        public void DrawViewSetEnabled(bool value)
        {
            if ( value )
            {
                if ( !DrawViewEnabled )
                {
                    DrawViewEnabled = true;
                    DrawViewLater();
                }
            }
            else
            {
                tmrDraw.Enabled = false;
                DrawViewEnabled = false;
            }
        }

        public void DrawViewLater()
        {
            DrawPending = true;
            if ( !tmrDrawDelay.Enabled )
            {
                tmrDraw.Enabled = true;
            }
        }

        private void tmrDraw_Tick(Object sender, EventArgs e)
        {
            tmrDraw.Enabled = false;
            if ( DrawPending )
            {
                DrawView();
                DrawPending = false;
                tmrDrawDelay.Enabled = true;
            }
        }

        private void GLInitialize(object sender, EventArgs e)
        {
            if ( OpenGLControl.Context == null )
            {
                return;
            }

            GLInitializeDelayTimer.Enabled = false;
            GLInitializeDelayTimer.Tick -= GLInitialize;
            GLInitializeDelayTimer.Dispose();
            GLInitializeDelayTimer = null;

            ResizeOpenGL();

            OpenGLControl.MouseDown += OpenGL_MouseDown;
            OpenGLControl.MouseUp += OpenGL_MouseUp;
            OpenGLControl.MouseWheel += OpenGL_MouseWheel;
            OpenGLControl.MouseMove += OpenGL_MouseMove;
            OpenGLControl.MouseEnter += OpenGL_MouseEnter;
            OpenGLControl.MouseLeave += OpenGL_MouseLeave;
            OpenGLControl.Resize += OpenGL_Resize;
            OpenGLControl.Leave += OpenGL_LostFocus;
            OpenGLControl.PreviewKeyDown += OpenGL_KeyDown;
            OpenGLControl.KeyUp += OpenGL_KeyUp;

            if ( GraphicsContext.CurrentContext != OpenGLControl.Context )
            {
                OpenGLControl.MakeCurrent();
            }

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

            IsGLInitialized = true;
        }

        public void Viewport_Resize()
        {
            if ( !App.ProgramInitialized )
            {
                return;
            }

            if ( GraphicsContext.CurrentContext != OpenGLControl.Context )
            {
                OpenGLControl.MakeCurrent();
            }
            GL.Viewport(0, 0, GLSize.X, GLSize.Y);

            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.Flush();
            OpenGLControl.SwapBuffers();
            Refresh();

            DrawViewLater();
        }

        private void DrawView()
        {
            if ( !(DrawViewEnabled && IsGLInitialized) )
            {
                return;
            }

            if ( GraphicsContext.CurrentContext != OpenGLControl.Context )
            {
                OpenGLControl.MakeCurrent();
            }

            var map = MainMap;
            var bgColour = new SRgb();

            if ( map == null )
            {
                bgColour.Red = 0.5F;
                bgColour.Green = 0.5F;
                bgColour.Blue = 0.5F;
            }
            else if ( map.Tileset == null )
            {
                bgColour.Red = 0.5F;
                bgColour.Green = 0.5F;
                bgColour.Blue = 0.5F;
            }
            else
            {
                bgColour = map.Tileset.BGColour;
            }

            GL.ClearColor(bgColour.Red, bgColour.Green, bgColour.Blue, 1.0F);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            if ( map != null )
            {
                map.GLDraw();
            }

            GL.Flush();
            OpenGLControl.SwapBuffers();

            Refresh();
        }

        public void OpenGL_MouseMove(object sender, MouseEventArgs e)
        {
            var map = MainMap;

            if ( map == null )
            {
                return;
            }

            map.ViewInfo.Map.ViewInfo.MouseOver = new clsViewInfo.clsMouseOver();
            map.ViewInfo.MouseOver.ScreenPos.X = e.X;
            map.ViewInfo.MouseOver.ScreenPos.Y = e.Y;

            map.ViewInfo.MouseOverPosCalc();

            DrawViewLater ();
        }

        public void Pos_Display_Update()
        {
            var map = MainMap;
            var mouseOverTerrain = map.ViewInfo.GetMouseOverTerrain();

            if ( mouseOverTerrain == null )
            {
                lblTile.Text = "";
                lblVertex.Text = "";
                lblPos.Text = "";
            }
            else
            {
                lblTile.Text = "Tile x:" + Convert.ToString(mouseOverTerrain.Tile.Normal.X) + ", y:" + Convert.ToString(mouseOverTerrain.Tile.Normal.Y);
                lblVertex.Text = "Vertex  x:" + Convert.ToString(mouseOverTerrain.Vertex.Normal.X) + ", y:" +
                                 Convert.ToString(mouseOverTerrain.Vertex.Normal.Y) + ", alt:" +
                                 map.Terrain.Vertices[mouseOverTerrain.Vertex.Normal.X, mouseOverTerrain.Vertex.Normal.Y].Height * map.HeightMultiplier + " (" +
                                 Convert.ToString(map.Terrain.Vertices[mouseOverTerrain.Vertex.Normal.X, mouseOverTerrain.Vertex.Normal.Y].Height) + "x" +
                                 Convert.ToString(map.HeightMultiplier) + ")";
                lblPos.Text = "Pos x:" + Convert.ToString(mouseOverTerrain.Pos.Horizontal.X) + ", y:" +
                              Convert.ToString(mouseOverTerrain.Pos.Horizontal.Y) + ", alt:" + Convert.ToString(mouseOverTerrain.Pos.Altitude) +
                              ", slope: " +
                              Convert.ToString(Math.Round(map.GetTerrainSlopeAngle(mouseOverTerrain.Pos.Horizontal) / MathUtil.RadOf1Deg * 10.0D) / 10.0D) +
                              "°";
            }
        }

        public void OpenGL_LostFocus(Object eventSender, EventArgs eventArgs)
        {
            var map = MainMap;

            if ( map == null )
            {
                return;
            }

            map.SuppressMinimap = false;

            map.ViewInfo.MouseOver = null;
            map.ViewInfo.MouseLeftDown = null;
            map.ViewInfo.MouseRightDown = null;

            App.ViewKeyDown_Clear();
        }

        private void ListSelect_Click(object Sender, ToolStripItemClickedEventArgs e)
        {
            var button = e.ClickedItem;
            var unit = (Unit)button.Tag;

            if ( listSelectIsPicker )
            {
                Program.frmMainInstance.ObjectPicker(unit.TypeBase);
            }
            else
            {
                if ( unit.MapSelectedUnitLink.IsConnected )
                {
                    unit.MapDeselect();
                }
                else
                {
                    unit.MapSelect();
                }
                Program.frmMainInstance.SelectedObject_Changed();
                DrawViewLater();
            }
        }

        private void ListSelect_Close(object sender, ToolStripDropDownClosedEventArgs e)
        {
            var a = 0;

            for ( a = 0; a <= listSelectItems.GetUpperBound(0); a++ )
            {
                listSelectItems[a].Tag = null;
                listSelectItems[a].Dispose();
            }
            listSelect.Items.Clear();
            listSelectItems = new ToolStripItem[0];

            App.ViewKeyDown_Clear();
        }

        private void OpenGL_MouseDown(object sender, MouseEventArgs e)
        {
            var map = MainMap;

            if ( map == null )
            {
                return;
            }

            map.ViewInfo.MouseDown(e);
        }

        private void OpenGL_KeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            var map = MainMap;

            if ( map == null )
            {
                return;
            }

            var matrixA = new Matrix3DMath.Matrix3D();
            var mouseOverTerrain = map.ViewInfo.GetMouseOverTerrain();

            App.IsViewKeyDown.Keys[(int)e.KeyCode] = true;

            foreach ( Option<KeyboardControl> control in KeyboardManager.OptionsKeyboardControls.Options )
            {
                ((KeyboardControl)(KeyboardManager.KeyboardProfile.GetValue(control))).KeysChanged(App.IsViewKeyDown);
            }

            if ( KeyboardManager.KeyboardProfile.Active(KeyboardManager.Undo) )
            {
                var message = "";
                if ( map.UndoPosition > 0 )
                {
                    message = "Undid: " + map.Undos[map.UndoPosition - 1].Name;
                    var mapMessage = new Message();
                    mapMessage.Text = message;
                    map.Messages.Add(mapMessage);
                    map.UndoPerform();
                    DrawViewLater();
                }
                else
                {
                    message = "Nothing to undo";
                }
                DisplayUndoMessage(message);
            }
            if ( KeyboardManager.KeyboardProfile.Active(KeyboardManager.Redo) )
            {
                var message = "";
                if ( map.UndoPosition < map.Undos.Count )
                {
                    message = "Redid: " + map.Undos[map.UndoPosition].Name;
                    var mapMessage = new Message();
                    mapMessage.Text = message;
                    map.Messages.Add(mapMessage);
                    map.RedoPerform();
                    DrawViewLater();
                }
                else
                {
                    message = "Nothing to redo";
                }
                DisplayUndoMessage(message);
            }
            if ( App.IsViewKeyDown.Keys[(int)Keys.ControlKey] )
            {
                if ( e.KeyCode == Keys.D1 )
                {
                    App.VisionRadius_2E = 6;
                }
                else if ( e.KeyCode == Keys.D2 )
                {
                    App.VisionRadius_2E = 7;
                }
                else if ( e.KeyCode == Keys.D3 )
                {
                    App.VisionRadius_2E = 8;
                }
                else if ( e.KeyCode == Keys.D4 )
                {
                    App.VisionRadius_2E = 9;
                }
                else if ( e.KeyCode == Keys.D5 )
                {
                    App.VisionRadius_2E = 10;
                }
                else if ( e.KeyCode == Keys.D6 )
                {
                    App.VisionRadius_2E = 11;
                }
                else if ( e.KeyCode == Keys.D7 )
                {
                    App.VisionRadius_2E = 12;
                }
                else if ( e.KeyCode == Keys.D8 )
                {
                    App.VisionRadius_2E = 13;
                }
                else if ( e.KeyCode == Keys.D9 )
                {
                    App.VisionRadius_2E = 14;
                }
                else if ( e.KeyCode == Keys.D0 )
                {
                    App.VisionRadius_2E = 15;
                }
                App.VisionRadius_2E_Changed();
            }

            if ( KeyboardManager.KeyboardProfile.Active(KeyboardManager.ViewMoveType) )
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
            if ( KeyboardManager.KeyboardProfile.Active(KeyboardManager.ViewRotateType) )
            {
                App.RTSOrbit = !App.RTSOrbit;
            }
            if ( KeyboardManager.KeyboardProfile.Active(KeyboardManager.ViewReset) )
            {
                map.ViewInfo.FovMultiplierSet(SettingsManager.Settings.FOVDefault);
                if ( App.ViewMoveType == ViewMoveType.Free )
                {
                    Matrix3DMath.MatrixSetToXAngle(matrixA, Math.Atan(2.0D));
                    map.ViewInfo.ViewAngleSetRotate(matrixA);
                }
                else if ( App.ViewMoveType == ViewMoveType.RTS )
                {
                    Matrix3DMath.MatrixSetToXAngle(matrixA, Math.Atan(2.0D));
                    map.ViewInfo.ViewAngleSetRotate(matrixA);
                }
            }
            if ( KeyboardManager.KeyboardProfile.Active(KeyboardManager.ViewTextures) )
            {
                App.Draw_TileTextures = !App.Draw_TileTextures;
                DrawViewLater();
            }
            if ( KeyboardManager.KeyboardProfile.Active(KeyboardManager.ViewWireframe) )
            {
                App.Draw_TileWireframe = !App.Draw_TileWireframe;
                DrawViewLater();
            }
            if ( KeyboardManager.KeyboardProfile.Active(KeyboardManager.ViewUnits) )
            {
                App.Draw_Units = !App.Draw_Units;
                
                var sectorNum = new XYInt();
                for (var y = 0; y <= map.SectorCount.Y - 1; y++ )
                {
                    for (var  x = 0; x <= map.SectorCount.X - 1; x++ )
                    {
                        foreach ( var connection in map.Sectors[x, y].Units )
                        {    
                            var Unit = connection.Unit;
                            if ( Unit.TypeBase.Type == UnitType.PlayerStructure )
                            {
                                if ( ((StructureTypeBase)Unit.TypeBase).StructureBasePlate != null )
                                {
                                    sectorNum.X = x;
                                    sectorNum.Y = y;
                                    map.SectorGraphicsChanges.Changed(sectorNum);
                                    break;
                                }
                            }
                        }
                    }
                }
                map.Update();
                DrawViewLater();
            }
            if ( KeyboardManager.KeyboardProfile.Active(KeyboardManager.ViewScriptMarkers) )
            {
                App.Draw_ScriptMarkers = !App.Draw_ScriptMarkers;
                DrawViewLater();
            }
            if ( KeyboardManager.KeyboardProfile.Active(KeyboardManager.ViewLighting) )
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
                DrawViewLater();
            }
            if ( modTools.Tool == modTools.Tools.TextureBrush )
            {
                if ( mouseOverTerrain != null )
                {
                    if ( KeyboardManager.KeyboardProfile.Active(KeyboardManager.Clockwise) )
                    {
                        map.ViewInfo.ApplyTextureClockwise();
                    }
                    if ( KeyboardManager.KeyboardProfile.Active(KeyboardManager.CounterClockwise) )
                    {
                        map.ViewInfo.ApplyTextureCounterClockwise();
                    }
                    if ( KeyboardManager.KeyboardProfile.Active(KeyboardManager.TextureFlip) )
                    {
                        map.ViewInfo.ApplyTextureFlipX();
                    }
                    if ( KeyboardManager.KeyboardProfile.Active(KeyboardManager.TriFlip) )
                    {
                        map.ViewInfo.ApplyTriFlip();
                    }
                }
            }
            if ( modTools.Tool == modTools.Tools.ObjectSelect )
            {
                if ( KeyboardManager.KeyboardProfile.Active(KeyboardManager.UnitDelete) )
                {
                    if ( map.SelectedUnits.Count > 0 )
                    {
                        foreach ( var unit in map.SelectedUnits.GetItemsAsSimpleList() )
                        {
                            
                            map.UnitRemoveStoreChange(unit.MapLink.ArrayPosition);
                        }
                        Program.frmMainInstance.SelectedObject_Changed();
                        map.UndoStepCreate("Object Deleted");
                        map.Update();
                        map.MinimapMakeLater();
                        DrawViewLater();
                    }
                }
                if ( KeyboardManager.KeyboardProfile.Active(KeyboardManager.UnitMove) )
                {
                    if ( mouseOverTerrain != null )
                    {
                        if ( map.SelectedUnits.Count > 0 )
                        {
                            var centre = App.CalcUnitsCentrePos(map.SelectedUnits.GetItemsAsSimpleList());
                            var offset = new XYInt();
                            offset.X = ((int)(Math.Round(Convert.ToDouble((mouseOverTerrain.Pos.Horizontal.X - centre.X) / Constants.TerrainGridSpacing)))) *
                                       Constants.TerrainGridSpacing;
                            offset.Y = ((int)(Math.Round(Convert.ToDouble((mouseOverTerrain.Pos.Horizontal.Y - centre.Y) / Constants.TerrainGridSpacing)))) *
                                       Constants.TerrainGridSpacing;
                            var objectPosOffset = new clsObjectPosOffset
                                {
                                    Map = map,
                                    Offset = offset
                                };
                            map.SelectedUnitsAction(objectPosOffset);

                            map.UndoStepCreate("Objects Moved");
                            map.Update();
                            map.MinimapMakeLater();
                            Program.frmMainInstance.SelectedObject_Changed();
                            DrawViewLater();
                        }
                    }
                }
                if ( KeyboardManager.KeyboardProfile.Active(KeyboardManager.Clockwise) )
                {
                    var objectRotationOffset = new clsObjectRotationOffset
                        {
                            Map = map,
                            Offset = -90
                        };
                    map.SelectedUnitsAction(objectRotationOffset);
                    map.Update();
                    Program.frmMainInstance.SelectedObject_Changed();
                    map.UndoStepCreate("Object Rotated");
                    DrawViewLater();
                }
                if ( KeyboardManager.KeyboardProfile.Active(KeyboardManager.CounterClockwise) )
                {
                    var objectRotationOffset = new clsObjectRotationOffset
                        {
                            Map = map,
                            Offset = 90
                        };
                    map.SelectedUnitsAction(objectRotationOffset);
                    map.Update();
                    Program.frmMainInstance.SelectedObject_Changed();
                    map.UndoStepCreate("Object Rotated");
                    DrawViewLater();
                }
            }

            if ( KeyboardManager.KeyboardProfile.Active(KeyboardManager.Deselect) )
            {
                modTools.Tool = modTools.Tools.ObjectSelect;
                DrawViewLater();
            }

            if ( KeyboardManager.KeyboardProfile.Active(KeyboardManager.PreviousTool) )
            {
                modTools.Tool = modTools.PreviousTool;
                DrawViewLater();
            }
        }

        private void OpenGL_KeyUp(object sender, KeyEventArgs e)
        {
            App.IsViewKeyDown.Keys[(int)e.KeyCode] = false;

            foreach ( Option<KeyboardControl> control in KeyboardManager.OptionsKeyboardControls.Options )
            {
                ((KeyboardControl)(KeyboardManager.KeyboardProfile.GetValue(control))).KeysChanged(App.IsViewKeyDown);
            }
        }

        private void OpenGL_MouseUp(object sender, MouseEventArgs e)
        {
            var map = MainMap;

            if ( map == null )
            {
                return;
            }

            var mouseOverTerrain = map.ViewInfo.GetMouseOverTerrain();

            map.SuppressMinimap = false;

            if ( e.Button == MouseButtons.Left )
            {
                if ( map.ViewInfo.GetMouseLeftDownOverMinimap() != null )
                {
                }
                else
                {
                    if ( modTools.Tool == modTools.Tools.TerrainBrush )
                    {
                        map.UndoStepCreate("Ground Painted");
                    }
                    else if ( modTools.Tool == modTools.Tools.CliffTriangle )
                    {
                        map.UndoStepCreate("Cliff Triangles");
                    }
                    else if ( modTools.Tool == modTools.Tools.CliffBrush )
                    {
                        map.UndoStepCreate("Cliff Brush");
                    }
                    else if ( modTools.Tool == modTools.Tools.CliffRemove )
                    {
                        map.UndoStepCreate("Cliff Remove Brush");
                    }
                    else if ( modTools.Tool == modTools.Tools.HeightChangeBrush )
                    {
                        map.UndoStepCreate("Height Change");
                    }
                    else if ( modTools.Tool == modTools.Tools.HeightSetBrush )
                    {
                        map.UndoStepCreate("Height Set");
                    }
                    else if ( modTools.Tool == modTools.Tools.HeightSmoothBrush )
                    {
                        map.UndoStepCreate("Height Smooth");
                    }
                    else if ( modTools.Tool == modTools.Tools.TextureBrush )
                    {
                        map.UndoStepCreate("Texture");
                    }
                    else if ( modTools.Tool == modTools.Tools.RoadRemove )
                    {
                        map.UndoStepCreate("Road Remove");
                    }
                    else if ( modTools.Tool == modTools.Tools.ObjectSelect )
                    {
                        if ( map.UnitSelectedAreaVertexA != null )
                        {
                            if ( mouseOverTerrain != null )
                            {
                                SelectUnits(map.UnitSelectedAreaVertexA, mouseOverTerrain.Vertex.Normal);
                            }
                            map.UnitSelectedAreaVertexA = null;
                        }
                    }
                }
                map.ViewInfo.MouseLeftDown = null;
            }
            else if ( e.Button == MouseButtons.Right )
            {
                if ( map.ViewInfo.GetMouseRightDownOverMinimap() != null )
                {
                }
                else
                {
                    if ( modTools.Tool == modTools.Tools.HeightChangeBrush )
                    {
                        map.UndoStepCreate("Height Change");
                    }
                    else if ( modTools.Tool == modTools.Tools.HeightSetBrush )
                    {
                        map.UndoStepCreate("Height Set");
                    }
                }
                map.ViewInfo.MouseRightDown = null;
            }
        }

        private void SelectUnits(XYInt vertexA, XYInt vertexB)
        {
            var map = MainMap;
            var mouseOverTerrain = map.ViewInfo.GetMouseOverTerrain();
            var sectorNum = new XYInt();
            Unit unit;
            var sectorStart = new XYInt();
            var sectorFinish = new XYInt();
            var startPos = new XYInt();
            var finishPos = new XYInt();
            var startVertex = new XYInt();
            var finishVertex = new XYInt();

            if ( Math.Abs(vertexA.X - vertexB.X) <= 1 &&
                 Math.Abs(vertexA.Y - vertexB.Y) <= 1 &&
                 mouseOverTerrain != null )
            {
                if ( mouseOverTerrain.Units.Count > 0 )
                {
                    if ( mouseOverTerrain.Units.Count == 1 )
                    {
                        unit = mouseOverTerrain.Units[0];
                        if ( unit.MapSelectedUnitLink.IsConnected )
                        {
                            unit.MapDeselect();
                        }
                        else
                        {
                            unit.MapSelect();
                        }
                    }
                    else
                    {
                        ListSelectBegin(false);
                    }
                }
            }
            else
            {
                MathUtil.ReorderXY(vertexA, vertexB, ref startVertex, ref finishVertex);
                startPos.X = startVertex.X * Constants.TerrainGridSpacing;
                startPos.Y = startVertex.Y * Constants.TerrainGridSpacing;
                finishPos.X = finishVertex.X * Constants.TerrainGridSpacing;
                finishPos.Y = finishVertex.Y * Constants.TerrainGridSpacing;
                sectorStart.X = Math.Min(startVertex.X / Constants.SectorTileSize, map.SectorCount.X - 1);
                sectorStart.Y = Math.Min(startVertex.Y / Constants.SectorTileSize, map.SectorCount.Y - 1);
                sectorFinish.X = Math.Min(finishVertex.X / Constants.SectorTileSize, map.SectorCount.X - 1);
                sectorFinish.Y = Math.Min(finishVertex.Y / Constants.SectorTileSize, map.SectorCount.Y - 1);
                for ( sectorNum.Y = sectorStart.Y; sectorNum.Y <= sectorFinish.Y; sectorNum.Y++ )
                {
                    for ( sectorNum.X = sectorStart.X; sectorNum.X <= sectorFinish.X; sectorNum.X++ )
                    {
                        foreach ( var connection in map.Sectors[sectorNum.X, sectorNum.Y].Units )
                        {
                            unit = connection.Unit;
                            if ( unit.Pos.Horizontal.X >= startPos.X & unit.Pos.Horizontal.Y >= startPos.Y &
                                 unit.Pos.Horizontal.X <= finishPos.X & unit.Pos.Horizontal.Y <= finishPos.Y )
                            {
                                if ( !unit.MapSelectedUnitLink.IsConnected )
                                {
                                    unit.MapSelect();
                                }
                            }
                        }
                    }
                }
            }
            Program.frmMainInstance.SelectedObject_Changed();
            DrawViewLater();
        }

        private void tmrDrawDelay_Tick(Object sender, EventArgs e)
        {
            if ( DrawPending )
            {
                DrawPending = false;
                DrawView();
            }
            else
            {
                tmrDrawDelay.Enabled = false;
            }
        }

        public void pnlDraw_Resize(object sender, EventArgs e)
        {
            if ( OpenGLControl != null )
            {
                ResizeOpenGL();
            }
        }

        public void OpenGL_Resize(Object eventSender, EventArgs eventArgs)
        {
            var map = MainMap;

            GLSize.X = OpenGLControl.Width;
            GLSize.Y = OpenGLControl.Height;
            //if (GLSize.Y != 0)
            //{
            //    GLSize_XPerY = (float)(GLSize.X / GLSize.Y);
            //}
            Viewport_Resize();
            if ( map != null )
            {
                map.ViewInfo.FovCalc();
            }
            DrawViewLater();
        }

        public void OpenGL_MouseEnter(object sender, EventArgs e)
        {
            if ( Form.ActiveForm == Program.frmMainInstance )
            {
                OpenGLControl.Focus();
            }

            DrawViewLater ();
        }

        public void OpenGL_MouseWheel(object sender, MouseEventArgs e)
        {
            var map = MainMap;

            if ( map == null )
            {
                return;
            }

            var move = new XYZInt(0, 0, 0);
            var xyzDbl = default(XYZDouble);
            var a = 0;

            for ( a = 0; a <= (int)(Math.Abs(e.Delta / 120.0D)); a++ )
            {
                Matrix3DMath.VectorForwardsRotationByMatrix(map.ViewInfo.ViewAngleMatrix,
                    Convert.ToDouble(Math.Sign(e.Delta) * Math.Max(map.ViewInfo.ViewPos.Y, 512.0D) / 24.0D), ref xyzDbl);
                move.SetDbl(xyzDbl);
                map.ViewInfo.ViewPosChange(move);
            }
        }

        public GLFont CreateGLFont(Font baseFont)
        {
            return new GLFont(new Font(baseFont.FontFamily, 24.0F, baseFont.Style, GraphicsUnit.Pixel));
        }

        public void RemoveUndoMessage(object sender, EventArgs e)
        {
            UndoMessageTimer.Enabled = false;
            lblUndo.Text = "";
        }

        public void DisplayUndoMessage(string text)
        {
            lblUndo.Text = text;
            UndoMessageTimer.Enabled = false;
            UndoMessageTimer.Enabled = true;
        }

        private void OpenGL_MouseLeave(object sender, EventArgs e)
        {
            var Map = MainMap;

            if ( Map == null )
            {
                return;
            }

            Map.ViewInfo.MouseOver = null;
        }

        public void ListSelectBegin(bool isPicker)
        {
            var Map = MainMap;
            var MouseOverTerrain = Map.ViewInfo.GetMouseOverTerrain();

            if ( MouseOverTerrain == null )
            {
                Debugger.Break();
                return;
            }

            var a = 0;

            listSelect.Close();
            listSelect.Items.Clear();
            listSelectItems = new ToolStripItem[MouseOverTerrain.Units.Count];
            for ( a = 0; a <= MouseOverTerrain.Units.Count - 1; a++ )
            {
                var unit = MouseOverTerrain.Units[a];
                listSelectItems[a] = new ToolStripButton(unit.TypeBase.GetDisplayTextCode());
                listSelectItems[a].Tag = unit;
                listSelect.Items.Add(listSelectItems[a]);
            }
            listSelectIsPicker = isPicker;
            listSelect.Show(this, new Point(Map.ViewInfo.MouseOver.ScreenPos.X, Map.ViewInfo.MouseOver.ScreenPos.Y));
        }

        public void tabMaps_SelectedIndexChanged(Object sender, EventArgs e)
        {
            if ( !tabMaps.Enabled )
            {
                return;
            }

            if ( tabMaps.SelectedTab == null )
            {
                owner.SetMainMap(null);
                return;
            }

            var map = (Map)tabMaps.SelectedTab.Tag;

            owner.SetMainMap(map);
        }

        public void btnClose_Click(Object sender, EventArgs e)
        {
            var map = MainMap;

            if ( map == null )
            {
                return;
            }
            if ( !map.FrmMainLink.IsConnected )
            {
                MessageBox.Show("Error: Map should be closed already.");
                return;
            }

            if ( !map.ClosePrompt() )
            {
                return;
            }

            map.Deallocate();
        }

        public void UpdateTabs()
        {
            Map map;

            tabMaps.Enabled = false;
            tabMaps.TabPages.Clear();
            foreach ( var tempMap in owner.LoadedMaps )
            {
                map = tempMap;
                tabMaps.TabPages.Add(map.MapViewTabPage);
            }
            map = MainMap;
            if ( map != null )
            {
                tabMaps.SelectedIndex = map.FrmMainLink.ArrayPosition;
            }
            else
            {
                tabMaps.SelectedIndex = -1;
            }
            tabMaps.Enabled = true;
        }
    }
}