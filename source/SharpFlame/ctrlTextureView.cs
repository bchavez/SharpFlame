using System;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace SharpFlame
{
    public partial class ctrlTextureView
    {
        private frmMain _Owner;

        public bool DrawPending;

        public modMath.sXY_int GLSize;
        public double GLSize_XPerY;

        public modMath.sXY_int View_Pos;

        public modMath.sXY_int TextureCount;
        public int TextureYOffset;

        public bool DrawView_Enabled = false;

        public bool DisplayTileTypes = false;
        public bool DisplayTileNumbers = false;

        private Timer GLInitializeDelayTimer;
        public bool IsGLInitialized = false;

        private Timer tmrDraw;
        private Timer tmrDrawDelay;

        public GLControl OpenGLControl;

        public ctrlTextureView(frmMain Owner)
        {
            _Owner = Owner;

            InitializeComponent();

            OpenGLControl = modMain.OpenGL2;
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
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.Enable(EnableCap.CullFace);

            IsGLInitialized = true;
        }

        public void Viewport_Resize()
        {
            if ( !modProgram.ProgramInitialized )
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

            clsMap Map = MainMap;

            if ( Map == null )
            {
                GL.Flush();
                OpenGLControl.SwapBuffers();
                Refresh();
                return;
            }

            int X = 0;
            int Y = 0;
            int Num = 0;
            modMath.sXY_int XY_int = new modMath.sXY_int();
            int A = 0;
            modMath.sXY_sng Vertex0 = new modMath.sXY_sng();
            modMath.sXY_sng Vertex1 = new modMath.sXY_sng();
            modMath.sXY_sng Vertex2 = new modMath.sXY_sng();
            modMath.sXY_sng UnrotatedPos = new modMath.sXY_sng();
            modMath.sXY_sng TexCoord0 = new modMath.sXY_sng();
            modMath.sXY_sng TexCoord1 = new modMath.sXY_sng();
            modMath.sXY_sng TexCoord2 = new modMath.sXY_sng();
            modMath.sXY_sng TexCoord3 = new modMath.sXY_sng();

            GL.MatrixMode(MatrixMode.Projection);
            Matrix4 temp_mat = Matrix4.CreateOrthographicOffCenter(0.0F, GLSize.X, GLSize.Y, 0.0F, -1.0F, 1.0F);
            GL.LoadMatrix(ref temp_mat);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            if ( Map.Tileset != null )
            {
                TileOrientation.GetTileRotatedTexCoords(modProgram.TextureOrientation, TexCoord0, TexCoord1, TexCoord2, TexCoord3);

                GL.Enable(EnableCap.Texture2D);
                GL.Color4(0.0F, 0.0F, 0.0F, 1.0F);

                for ( Y = 0; Y <= TextureCount.Y - 1; Y++ )
                {
                    for ( X = 0; X <= TextureCount.X - 1; X++ )
                    {
                        Num = (TextureYOffset + Y) * TextureCount.X + X;
                        if ( Num >= Map.Tileset.TileCount )
                        {
                            goto EndOfTextures1;
                        }
                        A = Map.Tileset.Tiles[Num].TextureView_GL_Texture_Num;
                        if ( A == 0 )
                        {
                            GL.BindTexture(TextureTarget.Texture2D, 0);
                        }
                        else
                        {
                            GL.BindTexture(TextureTarget.Texture2D, A);
                        }
                        GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (int)TextureEnvMode.Decal);
                        GL.Begin(BeginMode.Quads);
                        GL.TexCoord2(TexCoord0.X, TexCoord0.Y);
                        GL.Vertex2(X * 64, Y * 64);
                        GL.TexCoord2(TexCoord2.X, TexCoord2.Y);
                        GL.Vertex2(X * 64, Y * 64 + 64);
                        GL.TexCoord2(TexCoord3.X, TexCoord3.Y);
                        GL.Vertex2(X * 64 + 64, Y * 64 + 64);
                        GL.TexCoord2(TexCoord1.X, TexCoord1.Y);
                        GL.Vertex2(X * 64 + 64, Y * 64);
                        GL.End();
                    }
                }

                EndOfTextures1:

                GL.Disable(EnableCap.Texture2D);

                if ( DisplayTileTypes )
                {
                    GL.Begin(BeginMode.Quads);
                    for ( Y = 0; Y <= TextureCount.Y - 1; Y++ )
                    {
                        for ( X = 0; X <= TextureCount.X - 1; X++ )
                        {
                            Num = (TextureYOffset + Y) * TextureCount.X + X;
                            if ( Num >= Map.Tileset.TileCount )
                            {
                                goto EndOfTextures2;
                            }
                            A = Map.Tile_TypeNum[Num];
                            GL.Color3(modProgram.TileTypes[A].DisplayColour.Red, modProgram.TileTypes[A].DisplayColour.Green, modProgram.TileTypes[A].DisplayColour.Blue);
                            GL.Vertex2(X * 64 + 24, Y * 64 + 24);
                            GL.Vertex2(X * 64 + 24, Y * 64 + 40);
                            GL.Vertex2(X * 64 + 40, Y * 64 + 40);
                            GL.Vertex2(X * 64 + 40, Y * 64 + 24);
                        }
                    }
                    EndOfTextures2:
                    GL.End();
                }

                if ( modProgram.DisplayTileOrientation )
                {
                    GL.Disable(EnableCap.CullFace);

                    UnrotatedPos.X = 0.25F;
                    UnrotatedPos.Y = 0.25F;
                    Vertex0 = TileOrientation.GetTileRotatedPos_sng(modProgram.TextureOrientation, UnrotatedPos);
                    UnrotatedPos.X = 0.5F;
                    UnrotatedPos.Y = 0.25F;
                    Vertex1 = TileOrientation.GetTileRotatedPos_sng(modProgram.TextureOrientation, UnrotatedPos);
                    UnrotatedPos.X = 0.5F;
                    UnrotatedPos.Y = 0.5F;
                    Vertex2 = TileOrientation.GetTileRotatedPos_sng(modProgram.TextureOrientation, UnrotatedPos);

                    GL.Begin(BeginMode.Triangles);
                    GL.Color3(1.0F, 1.0F, 0.0F);
                    for ( Y = 0; Y <= TextureCount.Y - 1; Y++ )
                    {
                        for ( X = 0; X <= TextureCount.X - 1; X++ )
                        {
                            Num = (TextureYOffset + Y) * TextureCount.X + X;
                            if ( Num >= Map.Tileset.TileCount )
                            {
                                goto EndOfTextures3;
                            }
                            GL.Vertex2(X * 64 + Vertex0.X * 64, Y * 64 + Vertex0.Y * 64);
                            GL.Vertex2(X * 64 + Vertex2.X * 64, Y * 64 + Vertex2.Y * 64);
                            GL.Vertex2(X * 64 + Vertex1.X * 64, Y * 64 + Vertex1.Y * 64);
                        }
                    }
                    EndOfTextures3:
                    GL.End();

                    GL.Enable(EnableCap.CullFace);
                }

                if ( DisplayTileNumbers && modProgram.UnitLabelFont != null ) //TextureViewFont IsNot Nothing Then
                {
                    clsTextLabel TextLabel = default(clsTextLabel);
                    GL.Enable(EnableCap.Texture2D);
                    for ( Y = 0; Y <= TextureCount.Y - 1; Y++ )
                    {
                        for ( X = 0; X <= TextureCount.X - 1; X++ )
                        {
                            Num = (TextureYOffset + Y) * TextureCount.X + X;
                            if ( Num >= Map.Tileset.TileCount )
                            {
                                goto EndOfTextures4;
                            }
                            TextLabel = new clsTextLabel();
                            TextLabel.Text = modIO.InvariantToString_int(Num);
                            TextLabel.SizeY = 24.0F;
                            TextLabel.Colour.Red = 1.0F;
                            TextLabel.Colour.Green = 1.0F;
                            TextLabel.Colour.Blue = 0.0F;
                            TextLabel.Colour.Alpha = 1.0F;
                            TextLabel.Pos.X = X * 64;
                            TextLabel.Pos.Y = Y * 64;
                            TextLabel.TextFont = modProgram.UnitLabelFont; //TextureViewFont
                            TextLabel.Draw();
                        }
                    }
                    EndOfTextures4:
                    GL.Disable(EnableCap.Texture2D);
                }

                if ( modProgram.SelectedTextureNum >= 0 & TextureCount.X > 0 )
                {
                    A = modProgram.SelectedTextureNum - TextureYOffset * TextureCount.X;
                    XY_int.X = A - ((int)(Conversion.Int(A / TextureCount.X))) * TextureCount.X;
                    XY_int.Y = (int)(Conversion.Int(A / TextureCount.X));
                    GL.Begin(BeginMode.LineLoop);
                    GL.Color3(1.0F, 1.0F, 0.0F);
                    GL.Vertex2(XY_int.X * 64, XY_int.Y * 64);
                    GL.Vertex2(XY_int.X * 64, XY_int.Y * 64.0D + 64);
                    GL.Vertex2(XY_int.X * 64 + 64, XY_int.Y * 64 + 64);
                    GL.Vertex2(XY_int.X * 64 + 64, XY_int.Y * 64);
                    GL.End();
                }
            }

            GL.Flush();
            OpenGLControl.SwapBuffers();

            Refresh();
        }

        public void OpenGL_MouseDown(object sender, MouseEventArgs e)
        {
            clsMap Map = MainMap;

            if ( Map == null )
            {
                return;
            }

            if ( Map == null )
            {
                modProgram.SelectedTextureNum = -1;
            }
            else if ( Map.Tileset == null )
            {
                modProgram.SelectedTextureNum = -1;
            }
            else if ( e.X >= 0 & e.X < TextureCount.X * 64 & e.Y >= 0 & e.Y < TextureCount.Y * 64 )
            {
                modProgram.SelectedTextureNum = (int)((TextureYOffset + (int)(Conversion.Int(e.Y / 64.0D))) * TextureCount.X + Conversion.Int(e.X / 64.0D));
                if ( modProgram.SelectedTextureNum >= Map.Tileset.TileCount )
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

            if ( modProgram.SelectedTextureNum >= 0 )
            {
                modMain.frmMainInstance.cboTileType.Enabled = false;
                modMain.frmMainInstance.cboTileType.SelectedIndex = Map.Tile_TypeNum[modProgram.SelectedTextureNum];
                modMain.frmMainInstance.cboTileType.Enabled = true;
            }
            else
            {
                modMain.frmMainInstance.cboTileType.Enabled = false;
                modMain.frmMainInstance.cboTileType.SelectedIndex = -1;
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
                TextureCount.X = (int)(Math.Floor(GLSize.X / 64.0D));
                TextureCount.Y = (int)(Math.Ceiling(GLSize.Y / 64.0D));
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
            clsMap Map = MainMap;

            if ( Map == null )
            {
                return;
            }

            bool Flag = default(bool);

            if ( TextureCount.X > 0 & TextureCount.Y > 0 )
            {
                if ( Map == null )
                {
                    Flag = true;
                }
                else if ( Map.Tileset == null )
                {
                    Flag = true;
                }
                else
                {
                    Flag = false;
                }
            }
            else
            {
                Flag = true;
            }
            if ( Flag )
            {
                TextureScroll.Maximum = 0;
                TextureScroll.LargeChange = 0;
                TextureScroll.Enabled = false;
            }
            else
            {
                TextureScroll.Maximum = (int)(Math.Ceiling((double)(Map.Tileset.TileCount / TextureCount.X)));
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
                GLSize_XPerY = GLSize.X / GLSize.Y;
            }
            Viewport_Resize();
        }

        public void TextureScroll_ValueChanged(object sender, EventArgs e)
        {
            TextureYOffset = TextureScroll.Value;

            DrawViewLater();
        }

        private clsMap MainMap
        {
            get { return _Owner.MainMap; }
        }
    }
}