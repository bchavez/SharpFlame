

using System;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using SharpFlame.Core.Extensions;
using SharpFlame.FileIO;
using SharpFlame.Core.Domain;
using SharpFlame.Graphics.OpenGL;
using SharpFlame.Mapping;
using SharpFlame.Mapping.Tiles;
using SharpFlame.Util;
using SharpFlame.UiOptions;



namespace SharpFlame.Controls
{
    public partial class TextureViewControl
    {
        private readonly frmMain owner;
        private readonly Timer tmrDraw;
        private readonly Timer tmrDrawDelay;
        public bool DisplayTileNumbers = false;
        public bool DisplayTileTypes = false;

        public bool DrawPending;

        public bool DrawView_Enabled = false;

        private Timer GLInitializeDelayTimer;
        public XYInt GLSize;
        public double GLSize_XPerY;
        public bool IsGLInitialized = false;

        public GLControl OpenGLControl;
        public XYInt TextureCount;
        public int TextureYOffset;
        public XYInt ViewPos;

        public TextureViewControl(frmMain owner)
        {
            this.owner = owner;
            TextureCount = new XYInt(0, 0);
            GLSize = new XYInt(0, 0);

            InitializeComponent();

            OpenGLControl = Program.OpenGL2;
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
        }

        private Map MainMap
        {
            get { return owner.MainMap; }
        }

        public void OpenGL_Size_Calc()
        {
            if ( OpenGLControl.Context == null )
            {
                return;
            }

            OpenGLControl.Width = pnlDraw.Width;
            OpenGLControl.Height = pnlDraw.Height;

            Viewport_Resize();
        }

        public void DrawView_SetEnabled(bool Value)
        {
            if ( Value )
            {
                if ( !DrawView_Enabled )
                {
                    DrawView_Enabled = true;
                    DrawViewLater();
                }
            }
            else
            {
                tmrDraw.Enabled = false;
                DrawView_Enabled = false;
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
                DrawPending = false;
                DrawView();
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

            OpenGL_Size_Calc();

            OpenGLControl.MouseDown += OpenGL_MouseDown;
            OpenGLControl.Resize += OpenGL_Resize;

            if ( GraphicsContext.CurrentContext != OpenGLControl.Context )
            {
                OpenGLControl.MakeCurrent();
            }

            GL.ClearColor(0.0F, 0.0F, 0.0F, 1.0F);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            //GL.Enable(EnableCap.Blend);
            //GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            //GL.Enable(EnableCap.CullFace);

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
            if ( !(DrawView_Enabled && IsGLInitialized) )
            {
                return;
            }

            if ( GraphicsContext.CurrentContext != OpenGLControl.Context )
            {
                OpenGLControl.MakeCurrent();
            }

            GL.Clear(ClearBufferMask.ColorBufferBit);

            var map = MainMap;

            if ( map == null )
            {
                GL.Flush();
                OpenGLControl.SwapBuffers();
                Refresh();
                return;
            }

            var xyInt = new XYInt();
            var unrotatedPos = new XYDouble();
            var texCoord0 = new XYDouble();
            var texCoord1 = new XYDouble();
            var texCoord2 = new XYDouble();
            var texCoord3 = new XYDouble();

            GL.MatrixMode(MatrixMode.Projection);
            var temp_mat = Matrix4.CreateOrthographicOffCenter(0.0F, GLSize.X, GLSize.Y, 0.0F, -1.0F, 1.0F);
            GL.LoadMatrix(ref temp_mat);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            if ( map.Tileset != null )
            {
                TileUtil.GetTileRotatedTexCoords(App.TextureOrientation, ref texCoord0, ref texCoord1, ref texCoord2, ref texCoord3);

                GL.Enable(EnableCap.Texture2D);
                GL.Color4(0.0F, 0.0F, 0.0F, 1.0F);

                var x = 0;
                var y = 0;
                var num = 0;
                var a = 0;
                for ( y = 0; y <= TextureCount.Y - 1; y++ )
                {
                    for ( x = 0; x <= TextureCount.X - 1; x++ )
                    {
                        num = (TextureYOffset + y) * TextureCount.X + x;
                        if ( num >= map.Tileset.Tiles.Count )
                        {
                            goto EndOfTextures1;
                        }
                        a = map.Tileset.Tiles[num].GlTextureNum;
                        GL.BindTexture(TextureTarget.Texture2D, a);
                        GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (int)TextureEnvMode.Decal);
                        GL.Begin(BeginMode.Quads);
                        GL.TexCoord2(texCoord0.X, texCoord0.Y);
                        GL.Vertex2(x * 64, y * 64); // Top Left
                        GL.TexCoord2(texCoord1.X, texCoord1.Y);
                        GL.Vertex2(x * 64 + 64, y * 64); // Bottom Left
                        GL.TexCoord2(texCoord3.X, texCoord3.Y);
                        GL.Vertex2(x * 64 + 64, y * 64 + 64); // Bottom right
                        GL.TexCoord2(texCoord2.X, texCoord2.Y);
                        GL.Vertex2(x * 64, y * 64 + 64); // Top right

                        GL.End();
                    }
                }

                EndOfTextures1:

                GL.Disable(EnableCap.Texture2D);

                if ( DisplayTileTypes )
                {
                    GL.Begin(BeginMode.Quads);
                    for ( y = 0; y <= TextureCount.Y - 1; y++ )
                    {
                        for ( x = 0; x <= TextureCount.X - 1; x++ )
                        {
                            num = (TextureYOffset + y) * TextureCount.X + x;
                            if ( num >= map.Tileset.Tiles.Count )
                            {
                                goto EndOfTextures2;
                            }
                            a = map.TileTypeNum[num];
                            GL.Color3(App.TileTypes[a].DisplayColour.Red, App.TileTypes[a].DisplayColour.Green, App.TileTypes[a].DisplayColour.Blue);
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
                    for ( y = 0; y <= TextureCount.Y - 1; y++ )
                    {
                        for ( x = 0; x <= TextureCount.X - 1; x++ )
                        {
                            num = (TextureYOffset + y) * TextureCount.X + x;
                            if ( num >= map.Tileset.Tiles.Count )
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

                if ( DisplayTileNumbers && App.UnitLabelFont != null ) //TextureViewFont IsNot Nothing Then
                {
                    GL.Enable(EnableCap.Texture2D);
                    for ( y = 0; y <= TextureCount.Y - 1; y++ )
                    {
                        for ( x = 0; x <= TextureCount.X - 1; x++ )
                        {
                            num = (TextureYOffset + y) * TextureCount.X + x;
                            if ( num >= map.Tileset.Tiles.Count )
                            {
                                goto EndOfTextures4;
                            }
                            clsTextLabel textLabel = new clsTextLabel();
                            textLabel.Text = num.ToStringInvariant();
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

                if ( App.SelectedTextureNum >= 0 & TextureCount.X > 0 )
                {
                    a = App.SelectedTextureNum - TextureYOffset * TextureCount.X;
                    xyInt.X = a - a / TextureCount.X * TextureCount.X;
                    xyInt.Y = a / TextureCount.X;
                    GL.Begin(BeginMode.LineLoop);
                    GL.Color3(1.0F, 1.0F, 0.0F);
                    GL.Vertex2(xyInt.X * 64, xyInt.Y * 64);
                    GL.Vertex2(xyInt.X * 64, xyInt.Y * 64.0D + 64);
                    GL.Vertex2(xyInt.X * 64 + 64, xyInt.Y * 64 + 64);
                    GL.Vertex2(xyInt.X * 64 + 64, xyInt.Y * 64);
                    GL.End();
                }
            }

            GL.Flush();
            OpenGLControl.SwapBuffers();

            Refresh();
        }

        public void OpenGL_MouseDown(object sender, MouseEventArgs e)
        {
            var map = MainMap;

            if ( map == null )
            {
                return;
            }

            if ( map.Tileset == null )
            {
                App.SelectedTextureNum = -1;
            }
            else if ( e.X >= 0 & e.X < TextureCount.X * 64 & e.Y >= 0 & e.Y < TextureCount.Y * 64 )
            {
                App.SelectedTextureNum = TextureYOffset + Math.Floor(e.Y / 64.0D).ToInt() * TextureCount.X + Math.Floor(e.X / 64.0D).ToInt();
                if ( App.SelectedTextureNum >= map.Tileset.Tiles.Count )
                {
                    App.SelectedTextureNum = -1;
                }
                else
                {
                    App.UiOptions.MouseTool = MouseTool.TextureBrush;
                }
            }
            else
            {
                App.SelectedTextureNum = -1;
            }

            if ( App.SelectedTextureNum >= 0 )
            {
                Program.frmMainInstance.cboTileType.Enabled = false;
                Program.frmMainInstance.cboTileType.SelectedIndex = map.TileTypeNum[App.SelectedTextureNum];
                Program.frmMainInstance.cboTileType.Enabled = true;
            }
            else
            {
                Program.frmMainInstance.cboTileType.Enabled = false;
                Program.frmMainInstance.cboTileType.SelectedIndex = -1;
            }

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
                OpenGL_Size_Calc();
                TextureCount.X = Math.Floor(GLSize.X / 64.0D).ToInt();
                TextureCount.Y = Math.Ceiling(GLSize.Y / 64.0D).ToInt();
            }
            else
            {
                TextureCount.X = 0;
                TextureCount.Y = 0;
            }

            ScrollUpdate();
        }

        public void ScrollUpdate()
        {
            var map = MainMap;

            if ( map == null )
            {
                return;
            }

            var flag = false;

            if ( TextureCount.X > 0 & TextureCount.Y > 0 )
            {
                if ( map.Tileset == null )
                {
                    flag = true;
                }
            }
            else
            {
                flag = true;
            }
            if ( flag )
            {
                TextureScroll.Maximum = 0;
                TextureScroll.LargeChange = 0;
                TextureScroll.Enabled = false;
            }
            else
            {
                TextureScroll.Maximum = Math.Ceiling((double)map.Tileset.TileCount / TextureCount.X).ToInt();
                TextureScroll.LargeChange = TextureCount.Y;
                TextureScroll.Enabled = true;
            }
        }

        public void OpenGL_Resize(object sender, EventArgs e)
        {
            GLSize.X = OpenGLControl.Width;
            GLSize.Y = OpenGLControl.Height;
            if ( GLSize.Y != 0 )
            {
                GLSize_XPerY = (double)GLSize.X / GLSize.Y;
            }
            Viewport_Resize();
        }

        public void TextureScroll_ValueChanged(object sender, EventArgs e)
        {
            TextureYOffset = TextureScroll.Value;

            DrawViewLater();
        }
    }
}