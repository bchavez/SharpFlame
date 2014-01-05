namespace FlaME
{
    using Microsoft.VisualBasic;
    using Microsoft.VisualBasic.CompilerServices;
    using OpenTK;
    using OpenTK.Graphics;
    using OpenTK.Graphics.OpenGL;
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Runtime.CompilerServices;
    using System.Windows.Forms;

    [DesignerGenerated]
    public class ctrlTextureView : UserControl
    {
        private frmMain _Owner;
        [AccessedThroughProperty("pnlDraw")]
        private Panel _pnlDraw;
        [AccessedThroughProperty("TableLayoutPanel1")]
        private TableLayoutPanel _TableLayoutPanel1;
        [AccessedThroughProperty("TextureScroll")]
        private VScrollBar _TextureScroll;
        [AccessedThroughProperty("tmrDraw")]
        private Timer _tmrDraw;
        [AccessedThroughProperty("tmrDrawDelay")]
        private Timer _tmrDrawDelay;
        private IContainer components;
        public bool DisplayTileNumbers = false;
        public bool DisplayTileTypes = false;
        public bool DrawPending;
        public bool DrawView_Enabled = false;
        private Timer GLInitializeDelayTimer;
        public modMath.sXY_int GLSize;
        public double GLSize_XPerY;
        public bool IsGLInitialized = false;
        public GLControl OpenGLControl;
        public modMath.sXY_int TextureCount;
        public int TextureYOffset;
        public modMath.sXY_int View_Pos;

        public ctrlTextureView(frmMain Owner)
        {
            this._Owner = Owner;
            this.InitializeComponent();
            this.OpenGLControl = modMain.OpenGL2;
            this.pnlDraw.Controls.Add(this.OpenGLControl);
            this.GLInitializeDelayTimer = new Timer();
            this.GLInitializeDelayTimer.Interval = 50;
            this.GLInitializeDelayTimer.Tick += new EventHandler(this.GLInitialize);
            this.GLInitializeDelayTimer.Enabled = true;
            this.tmrDraw = new Timer();
            this.tmrDraw.Interval = 1;
            this.tmrDrawDelay = new Timer();
            this.tmrDrawDelay.Interval = 30;
        }

        [DebuggerNonUserCode]
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && (this.components != null))
                {
                    this.components.Dispose();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        private void DrawView()
        {
            if (this.DrawView_Enabled & this.IsGLInitialized)
            {
                if (OpenTK.Graphics.GraphicsContext.CurrentContext != this.OpenGLControl.Context)
                {
                    this.OpenGLControl.MakeCurrent();
                }
                GL.Clear(ClearBufferMask.ColorBufferBit);
                clsMap mainMap = this.MainMap;
                if (mainMap == null)
                {
                    GL.Flush();
                    this.OpenGLControl.SwapBuffers();
                    this.Refresh();
                }
                else
                {
                    GL.MatrixMode(OpenTK.Graphics.OpenGL.MatrixMode.Projection);
                    GL.LoadMatrix(ref Matrix4.CreateOrthographicOffCenter(0f, (float) this.GLSize.X, (float) this.GLSize.Y, 0f, -1f, 1f));
                    GL.MatrixMode(OpenTK.Graphics.OpenGL.MatrixMode.Modelview);
                    GL.LoadIdentity();
                    if (mainMap.Tileset != null)
                    {
                        int num;
                        int num2;
                        modMath.sXY_sng _sng;
                        modMath.sXY_sng _sng2;
                        modMath.sXY_sng _sng3;
                        modMath.sXY_sng _sng4;
                        int num3;
                        int num4;
                        TileOrientation.GetTileRotatedTexCoords(modProgram.TextureOrientation, ref _sng, ref _sng2, ref _sng3, ref _sng4);
                        GL.Enable(EnableCap.Texture2D);
                        GL.Color4((float) 0f, (float) 0f, (float) 0f, (float) 1f);
                        int num5 = this.TextureCount.Y - 1;
                        for (num4 = 0; num4 <= num5; num4++)
                        {
                            int num6 = this.TextureCount.X - 1;
                            num3 = 0;
                            while (num3 <= num6)
                            {
                                num2 = ((this.TextureYOffset + num4) * this.TextureCount.X) + num3;
                                if (num2 >= mainMap.Tileset.TileCount)
                                {
                                    break;
                                }
                                num = mainMap.Tileset.Tiles[num2].TextureView_GL_Texture_Num;
                                if (num == 0)
                                {
                                    GL.BindTexture(TextureTarget.Texture2D, 0);
                                }
                                else
                                {
                                    GL.BindTexture(TextureTarget.Texture2D, num);
                                }
                                GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, 0x2101);
                                GL.Begin(BeginMode.Quads);
                                GL.TexCoord2(_sng.X, _sng.Y);
                                GL.Vertex2((int) (num3 * 0x40), (int) (num4 * 0x40));
                                GL.TexCoord2(_sng3.X, _sng3.Y);
                                GL.Vertex2((int) (num3 * 0x40), (int) ((num4 * 0x40) + 0x40));
                                GL.TexCoord2(_sng4.X, _sng4.Y);
                                GL.Vertex2((int) ((num3 * 0x40) + 0x40), (int) ((num4 * 0x40) + 0x40));
                                GL.TexCoord2(_sng2.X, _sng2.Y);
                                GL.Vertex2((int) ((num3 * 0x40) + 0x40), (int) (num4 * 0x40));
                                GL.End();
                                num3++;
                            }
                        }
                        GL.Disable(EnableCap.Texture2D);
                        if (this.DisplayTileTypes)
                        {
                            GL.Begin(BeginMode.Quads);
                            int num7 = this.TextureCount.Y - 1;
                            for (num4 = 0; num4 <= num7; num4++)
                            {
                                int num8 = this.TextureCount.X - 1;
                                num3 = 0;
                                while (num3 <= num8)
                                {
                                    num2 = ((this.TextureYOffset + num4) * this.TextureCount.X) + num3;
                                    if (num2 >= mainMap.Tileset.TileCount)
                                    {
                                        break;
                                    }
                                    num = mainMap.Tile_TypeNum[num2];
                                    GL.Color3(modProgram.TileTypes[num].DisplayColour.Red, modProgram.TileTypes[num].DisplayColour.Green, modProgram.TileTypes[num].DisplayColour.Blue);
                                    GL.Vertex2((int) ((num3 * 0x40) + 0x18), (int) ((num4 * 0x40) + 0x18));
                                    GL.Vertex2((int) ((num3 * 0x40) + 0x18), (int) ((num4 * 0x40) + 40));
                                    GL.Vertex2((int) ((num3 * 0x40) + 40), (int) ((num4 * 0x40) + 40));
                                    GL.Vertex2((int) ((num3 * 0x40) + 40), (int) ((num4 * 0x40) + 0x18));
                                    num3++;
                                }
                            }
                            GL.End();
                        }
                        if (modProgram.DisplayTileOrientation)
                        {
                            modMath.sXY_sng _sng5;
                            GL.Disable(EnableCap.CullFace);
                            _sng5.X = 0.25f;
                            _sng5.Y = 0.25f;
                            modMath.sXY_sng _sng6 = TileOrientation.GetTileRotatedPos_sng(modProgram.TextureOrientation, _sng5);
                            _sng5.X = 0.5f;
                            _sng5.Y = 0.25f;
                            modMath.sXY_sng _sng7 = TileOrientation.GetTileRotatedPos_sng(modProgram.TextureOrientation, _sng5);
                            _sng5.X = 0.5f;
                            _sng5.Y = 0.5f;
                            modMath.sXY_sng _sng8 = TileOrientation.GetTileRotatedPos_sng(modProgram.TextureOrientation, _sng5);
                            GL.Begin(BeginMode.Triangles);
                            GL.Color3((float) 1f, (float) 1f, (float) 0f);
                            int num9 = this.TextureCount.Y - 1;
                            for (num4 = 0; num4 <= num9; num4++)
                            {
                                int num10 = this.TextureCount.X - 1;
                                num3 = 0;
                                while (num3 <= num10)
                                {
                                    num2 = ((this.TextureYOffset + num4) * this.TextureCount.X) + num3;
                                    if (num2 >= mainMap.Tileset.TileCount)
                                    {
                                        break;
                                    }
                                    GL.Vertex2((float) ((num3 * 0x40) + (_sng6.X * 64f)), (float) ((num4 * 0x40) + (_sng6.Y * 64f)));
                                    GL.Vertex2((float) ((num3 * 0x40) + (_sng8.X * 64f)), (float) ((num4 * 0x40) + (_sng8.Y * 64f)));
                                    GL.Vertex2((float) ((num3 * 0x40) + (_sng7.X * 64f)), (float) ((num4 * 0x40) + (_sng7.Y * 64f)));
                                    num3++;
                                }
                            }
                            GL.End();
                            GL.Enable(EnableCap.CullFace);
                        }
                        if (this.DisplayTileNumbers & (modProgram.UnitLabelFont != null))
                        {
                            GL.Enable(EnableCap.Texture2D);
                            int num11 = this.TextureCount.Y - 1;
                            for (num4 = 0; num4 <= num11; num4++)
                            {
                                int num12 = this.TextureCount.X - 1;
                                for (num3 = 0; num3 <= num12; num3++)
                                {
                                    num2 = ((this.TextureYOffset + num4) * this.TextureCount.X) + num3;
                                    if (num2 >= mainMap.Tileset.TileCount)
                                    {
                                        break;
                                    }
                                    clsTextLabel label = new clsTextLabel {
                                        Text = modIO.InvariantToString_int(num2),
                                        SizeY = 24f
                                    };
                                    label.Colour.Red = 1f;
                                    label.Colour.Green = 1f;
                                    label.Colour.Blue = 0f;
                                    label.Colour.Alpha = 1f;
                                    label.Pos.X = num3 * 0x40;
                                    label.Pos.Y = num4 * 0x40;
                                    label.TextFont = modProgram.UnitLabelFont;
                                    label.Draw();
                                }
                            }
                            GL.Disable(EnableCap.Texture2D);
                        }
                        if ((modProgram.SelectedTextureNum >= 0) & (this.TextureCount.X > 0))
                        {
                            modMath.sXY_int _int;
                            num = modProgram.SelectedTextureNum - (this.TextureYOffset * this.TextureCount.X);
                            _int.X = num - (((int) Math.Round(Conversion.Int((double) (((double) num) / ((double) this.TextureCount.X))))) * this.TextureCount.X);
                            _int.Y = (int) Math.Round(Conversion.Int((double) (((double) num) / ((double) this.TextureCount.X))));
                            GL.Begin(BeginMode.LineLoop);
                            GL.Color3((float) 1f, (float) 1f, (float) 0f);
                            GL.Vertex2((int) (_int.X * 0x40), (int) (_int.Y * 0x40));
                            GL.Vertex2((double) (_int.X * 0x40), (_int.Y * 64.0) + 64.0);
                            GL.Vertex2((int) ((_int.X * 0x40) + 0x40), (int) ((_int.Y * 0x40) + 0x40));
                            GL.Vertex2((int) ((_int.X * 0x40) + 0x40), (int) (_int.Y * 0x40));
                            GL.End();
                        }
                    }
                    GL.Flush();
                    this.OpenGLControl.SwapBuffers();
                    this.Refresh();
                }
            }
        }

        public void DrawView_SetEnabled(bool Value)
        {
            if (Value)
            {
                if (!this.DrawView_Enabled)
                {
                    this.DrawView_Enabled = true;
                    this.DrawViewLater();
                }
            }
            else
            {
                this.tmrDraw.Enabled = false;
                this.DrawView_Enabled = false;
            }
        }

        public void DrawViewLater()
        {
            this.DrawPending = true;
            if (!this.tmrDrawDelay.Enabled)
            {
                this.tmrDraw.Enabled = true;
            }
        }

        private void GLInitialize(object sender, EventArgs e)
        {
            if (this.OpenGLControl.Context != null)
            {
                this.GLInitializeDelayTimer.Enabled = false;
                this.GLInitializeDelayTimer.Tick -= new EventHandler(this.GLInitialize);
                this.GLInitializeDelayTimer.Dispose();
                this.GLInitializeDelayTimer = null;
                this.OpenGL_Size_Calc();
                this.OpenGLControl.MouseDown += new MouseEventHandler(this.OpenGL_MouseDown);
                this.OpenGLControl.Resize += new EventHandler(this.OpenGL_Resize);
                if (OpenTK.Graphics.GraphicsContext.CurrentContext != this.OpenGLControl.Context)
                {
                    this.OpenGLControl.MakeCurrent();
                }
                GL.ClearColor(0f, 0f, 0f, 1f);
                GL.Clear(ClearBufferMask.ColorBufferBit);
                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
                GL.Enable(EnableCap.CullFace);
                this.IsGLInitialized = true;
            }
        }

        [DebuggerStepThrough]
        private void InitializeComponent()
        {
            this.TableLayoutPanel1 = new TableLayoutPanel();
            this.TextureScroll = new VScrollBar();
            this.pnlDraw = new Panel();
            this.TableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            this.TableLayoutPanel1.ColumnCount = 2;
            this.TableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            this.TableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 21f));
            this.TableLayoutPanel1.Controls.Add(this.TextureScroll, 1, 0);
            this.TableLayoutPanel1.Controls.Add(this.pnlDraw, 0, 0);
            this.TableLayoutPanel1.Dock = DockStyle.Fill;
            Point point2 = new Point(0, 0);
            this.TableLayoutPanel1.Location = point2;
            Padding padding2 = new Padding(0);
            this.TableLayoutPanel1.Margin = padding2;
            this.TableLayoutPanel1.Name = "TableLayoutPanel1";
            this.TableLayoutPanel1.RowCount = 1;
            this.TableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            this.TableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 25f));
            Size size2 = new Size(280, 0x180);
            this.TableLayoutPanel1.Size = size2;
            this.TableLayoutPanel1.TabIndex = 0;
            this.TextureScroll.Dock = DockStyle.Fill;
            point2 = new Point(0x103, 0);
            this.TextureScroll.Location = point2;
            this.TextureScroll.Name = "TextureScroll";
            size2 = new Size(0x15, 0x180);
            this.TextureScroll.Size = size2;
            this.TextureScroll.TabIndex = 1;
            this.pnlDraw.Dock = DockStyle.Fill;
            point2 = new Point(0, 0);
            this.pnlDraw.Location = point2;
            padding2 = new Padding(0);
            this.pnlDraw.Margin = padding2;
            this.pnlDraw.Name = "pnlDraw";
            size2 = new Size(0x103, 0x180);
            this.pnlDraw.Size = size2;
            this.pnlDraw.TabIndex = 2;
            this.AutoScaleMode = AutoScaleMode.None;
            this.BorderStyle = BorderStyle.Fixed3D;
            this.Controls.Add(this.TableLayoutPanel1);
            padding2 = new Padding(0);
            this.Margin = padding2;
            this.Name = "ctrlTextureView";
            size2 = new Size(280, 0x180);
            this.Size = size2;
            this.TableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        public void OpenGL_MouseDown(object sender, MouseEventArgs e)
        {
            clsMap mainMap = this.MainMap;
            if (mainMap != null)
            {
                if (mainMap == null)
                {
                    modProgram.SelectedTextureNum = -1;
                }
                else if (mainMap.Tileset == null)
                {
                    modProgram.SelectedTextureNum = -1;
                }
                else if ((((e.X >= 0) & (e.X < (this.TextureCount.X * 0x40))) & (e.Y >= 0)) & (e.Y < (this.TextureCount.Y * 0x40)))
                {
                    modProgram.SelectedTextureNum = ((this.TextureYOffset + ((int) Math.Round(Conversion.Int((double) (((double) e.Y) / 64.0))))) * this.TextureCount.X) + ((int) Math.Round(Conversion.Int((double) (((double) e.X) / 64.0))));
                    if (modProgram.SelectedTextureNum >= mainMap.Tileset.TileCount)
                    {
                        modProgram.SelectedTextureNum = -1;
                    }
                    else
                    {
                        modTools.Tool = modTools.Tools.TextureBrush;
                    }
                }
                else
                {
                    modProgram.SelectedTextureNum = -1;
                }
                if (modProgram.SelectedTextureNum >= 0)
                {
                    modMain.frmMainInstance.cboTileType.Enabled = false;
                    modMain.frmMainInstance.cboTileType.SelectedIndex = mainMap.Tile_TypeNum[modProgram.SelectedTextureNum];
                    modMain.frmMainInstance.cboTileType.Enabled = true;
                }
                else
                {
                    modMain.frmMainInstance.cboTileType.Enabled = false;
                    modMain.frmMainInstance.cboTileType.SelectedIndex = -1;
                }
                this.DrawViewLater();
            }
        }

        public void OpenGL_Resize(object sender, EventArgs e)
        {
            this.GLSize.X = this.OpenGLControl.Width;
            this.GLSize.Y = this.OpenGLControl.Height;
            if (this.GLSize.Y != 0)
            {
                this.GLSize_XPerY = ((double) this.GLSize.X) / ((double) this.GLSize.Y);
            }
            this.Viewport_Resize();
        }

        public void OpenGL_Size_Calc()
        {
            if (this.OpenGLControl.Context != null)
            {
                this.OpenGLControl.Width = this.pnlDraw.Width;
                this.OpenGLControl.Height = this.pnlDraw.Height;
                this.Viewport_Resize();
            }
        }

        private void pnlDraw_Resize(object sender, EventArgs e)
        {
            if (this.OpenGLControl != null)
            {
                this.OpenGL_Size_Calc();
                this.TextureCount.X = (int) Math.Round(Math.Floor((double) (((double) this.GLSize.X) / 64.0)));
                this.TextureCount.Y = (int) Math.Round(Math.Ceiling((double) (((double) this.GLSize.Y) / 64.0)));
            }
            else
            {
                this.TextureCount.X = 0;
                this.TextureCount.Y = 0;
            }
            this.ScrollUpdate();
        }

        public void ScrollUpdate()
        {
            clsMap mainMap = this.MainMap;
            if (mainMap != null)
            {
                bool flag;
                if ((this.TextureCount.X > 0) & (this.TextureCount.Y > 0))
                {
                    if (mainMap == null)
                    {
                        flag = true;
                    }
                    else if (mainMap.Tileset == null)
                    {
                        flag = true;
                    }
                    else
                    {
                        flag = false;
                    }
                }
                else
                {
                    flag = true;
                }
                if (flag)
                {
                    this.TextureScroll.Maximum = 0;
                    this.TextureScroll.LargeChange = 0;
                    this.TextureScroll.Enabled = false;
                }
                else
                {
                    this.TextureScroll.Maximum = (int) Math.Round(Math.Ceiling((double) (((double) mainMap.Tileset.TileCount) / ((double) this.TextureCount.X))));
                    this.TextureScroll.LargeChange = this.TextureCount.Y;
                    this.TextureScroll.Enabled = true;
                }
            }
        }

        private void TextureScroll_ValueChanged(object sender, EventArgs e)
        {
            this.TextureYOffset = this.TextureScroll.Value;
            this.DrawViewLater();
        }

        private void tmrDraw_Tick(object sender, EventArgs e)
        {
            this.tmrDraw.Enabled = false;
            if (this.DrawPending)
            {
                this.DrawPending = false;
                this.DrawView();
                this.tmrDrawDelay.Enabled = true;
            }
        }

        private void tmrDrawDelay_Tick(object sender, EventArgs e)
        {
            if (this.DrawPending)
            {
                this.DrawPending = false;
                this.DrawView();
            }
            else
            {
                this.tmrDrawDelay.Enabled = false;
            }
        }

        public void Viewport_Resize()
        {
            if (modProgram.ProgramInitialized)
            {
                if (OpenTK.Graphics.GraphicsContext.CurrentContext != this.OpenGLControl.Context)
                {
                    this.OpenGLControl.MakeCurrent();
                }
                GL.Viewport(0, 0, this.GLSize.X, this.GLSize.Y);
                GL.Clear(ClearBufferMask.ColorBufferBit);
                GL.Flush();
                this.OpenGLControl.SwapBuffers();
                this.Refresh();
                this.DrawViewLater();
            }
        }

        private clsMap MainMap
        {
            get
            {
                return this._Owner.MainMap;
            }
        }

        public virtual Panel pnlDraw
        {
            get
            {
                return this._pnlDraw;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.pnlDraw_Resize);
                if (this._pnlDraw != null)
                {
                    this._pnlDraw.Resize -= handler;
                }
                this._pnlDraw = value;
                if (this._pnlDraw != null)
                {
                    this._pnlDraw.Resize += handler;
                }
            }
        }

        public virtual TableLayoutPanel TableLayoutPanel1
        {
            get
            {
                return this._TableLayoutPanel1;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._TableLayoutPanel1 = value;
            }
        }

        public virtual VScrollBar TextureScroll
        {
            get
            {
                return this._TextureScroll;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.TextureScroll_ValueChanged);
                if (this._TextureScroll != null)
                {
                    this._TextureScroll.ValueChanged -= handler;
                }
                this._TextureScroll = value;
                if (this._TextureScroll != null)
                {
                    this._TextureScroll.ValueChanged += handler;
                }
            }
        }

        private Timer tmrDraw
        {
            get
            {
                return this._tmrDraw;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.tmrDraw_Tick);
                if (this._tmrDraw != null)
                {
                    this._tmrDraw.Tick -= handler;
                }
                this._tmrDraw = value;
                if (this._tmrDraw != null)
                {
                    this._tmrDraw.Tick += handler;
                }
            }
        }

        private Timer tmrDrawDelay
        {
            get
            {
                return this._tmrDrawDelay;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.tmrDrawDelay_Tick);
                if (this._tmrDrawDelay != null)
                {
                    this._tmrDrawDelay.Tick -= handler;
                }
                this._tmrDrawDelay = value;
                if (this._tmrDrawDelay != null)
                {
                    this._tmrDrawDelay.Tick += handler;
                }
            }
        }
    }
}

