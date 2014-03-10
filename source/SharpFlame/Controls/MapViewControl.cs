#region

using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using SharpFlame.Colors;
using SharpFlame.Graphics.OpenGL;
using SharpFlame.Core;
using SharpFlame.Core.Domain;
using SharpFlame.Core.Domain.Colors;
using SharpFlame.Domain;
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

//            map.viewInfo.MouseOver = new ViewInfo.clsMouseOver();
//            map.viewInfo.MouseOver.ScreenPos.X = e.X;
//            map.viewInfo.MouseOver.ScreenPos.Y = e.Y;
//
//            map.viewInfo.MouseOverPosCalc();

            DrawViewLater ();
        }


        public void OpenGL_LostFocus(Object eventSender, EventArgs eventArgs)
        {
            var map = MainMap;

            if ( map == null )
            {
                return;
            }

//            map.viewInfo.MouseOver = null;
//            map.viewInfo.MouseLeftDown = null;
//            map.viewInfo.MouseRightDown = null;

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

            // map.ViewInfo.MouseDown(sender, e);
        }

        private void OpenGL_KeyDown(object sender, PreviewKeyDownEventArgs e)
        {
        }

        private void OpenGL_KeyUp(object sender, KeyEventArgs e)
        {
            App.IsViewKeyDown.Keys[(int)e.KeyCode] = false;

//            foreach ( Option<KeyboardControl> control in KeyboardManager.OptionsKeyboardControls.Options )
//            {
//                ((KeyboardControl)(KeyboardManager.KeyboardProfile.GetValue(control))).KeysChanged(App.IsViewKeyDown);
//            }
        }

        private void OpenGL_MouseUp(object sender, MouseEventArgs e)
        {
        }

        private void SelectUnits(XYInt vertexA, XYInt vertexB)
        {
//            var map = MainMap;
//            var mouseOverTerrain = map.viewInfo.GetMouseOverTerrain();
//            var sectorNum = new XYInt();
//            Unit unit;
//            var sectorStart = new XYInt();
//            var sectorFinish = new XYInt();
//            var startPos = new XYInt();
//            var finishPos = new XYInt();
//            var startVertex = new XYInt();
//            var finishVertex = new XYInt();
//
//            if ( Math.Abs(vertexA.X - vertexB.X) <= 1 &&
//                 Math.Abs(vertexA.Y - vertexB.Y) <= 1 &&
//                 mouseOverTerrain != null )
//            {
//                if ( mouseOverTerrain.Units.Count > 0 )
//                {
//                    if ( mouseOverTerrain.Units.Count == 1 )
//                    {
//                        unit = mouseOverTerrain.Units[0];
//                        if ( unit.MapSelectedUnitLink.IsConnected )
//                        {
//                            unit.MapDeselect();
//                        }
//                        else
//                        {
//                            unit.MapSelect();
//                        }
//                    }
//                    else
//                    {
//                        ListSelectBegin(false);
//                    }
//                }
//            }
//            else
//            {
//                MathUtil.ReorderXY(vertexA, vertexB, ref startVertex, ref finishVertex);
//                startPos.X = startVertex.X * Constants.TerrainGridSpacing;
//                startPos.Y = startVertex.Y * Constants.TerrainGridSpacing;
//                finishPos.X = finishVertex.X * Constants.TerrainGridSpacing;
//                finishPos.Y = finishVertex.Y * Constants.TerrainGridSpacing;
//                sectorStart.X = Math.Min(startVertex.X / Constants.SectorTileSize, map.SectorCount.X - 1);
//                sectorStart.Y = Math.Min(startVertex.Y / Constants.SectorTileSize, map.SectorCount.Y - 1);
//                sectorFinish.X = Math.Min(finishVertex.X / Constants.SectorTileSize, map.SectorCount.X - 1);
//                sectorFinish.Y = Math.Min(finishVertex.Y / Constants.SectorTileSize, map.SectorCount.Y - 1);
//                for ( sectorNum.Y = sectorStart.Y; sectorNum.Y <= sectorFinish.Y; sectorNum.Y++ )
//                {
//                    for ( sectorNum.X = sectorStart.X; sectorNum.X <= sectorFinish.X; sectorNum.X++ )
//                    {
//                        foreach ( var connection in map.Sectors[sectorNum.X, sectorNum.Y].Units )
//                        {
//                            unit = connection.Unit;
//                            if ( unit.Pos.Horizontal.X >= startPos.X & unit.Pos.Horizontal.Y >= startPos.Y &
//                                 unit.Pos.Horizontal.X <= finishPos.X & unit.Pos.Horizontal.Y <= finishPos.Y )
//                            {
//                                if ( !unit.MapSelectedUnitLink.IsConnected )
//                                {
//                                    unit.MapSelect();
//                                }
//                            }
//                        }
//                    }
//                }
//            }
//            Program.frmMainInstance.SelectedObject_Changed();
//            DrawViewLater();
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
        }

        public void OpenGL_MouseEnter(object sender, EventArgs e)
        {
        }

        public void OpenGL_MouseWheel(object sender, MouseEventArgs e)
        {
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
        }

        public void ListSelectBegin(bool isPicker)
        {
//            var Map = MainMap;
//            var MouseOverTerrain = Map.viewInfo.GetMouseOverTerrain();
//
//            if ( MouseOverTerrain == null )
//            {
//                Debugger.Break();
//                return;
//            }
//
//            var a = 0;
//
//            listSelect.Close();
//            listSelect.Items.Clear();
//            listSelectItems = new ToolStripItem[MouseOverTerrain.Units.Count];
//            for ( a = 0; a <= MouseOverTerrain.Units.Count - 1; a++ )
//            {
//                var unit = MouseOverTerrain.Units[a];
//                listSelectItems[a] = new ToolStripButton(unit.TypeBase.GetDisplayTextCode());
//                listSelectItems[a].Tag = unit;
//                listSelect.Items.Add(listSelectItems[a]);
//            }
//            listSelectIsPicker = isPicker;
//            listSelect.Show(this, new Point(Map.viewInfo.MouseOver.ScreenPos.X, Map.viewInfo.MouseOver.ScreenPos.Y));
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

            if ( !map.ClosePrompt() )
            {
                return;
            }

            map.Deallocate();
        }

        public void UpdateTabs()
        {
            // Should never happen as we gona change that code.
            throw new NotImplementedException();
        }
    }
}