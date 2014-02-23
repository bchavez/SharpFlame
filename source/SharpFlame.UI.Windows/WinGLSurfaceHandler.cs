using System;
using System.Drawing.Imaging;
using Eto.Platform.Windows;
using OpenTK.Graphics.OpenGL;
using SharpFlame.UI.Controls;

namespace SharpFlame.UI.Windows
{
    //                                              <NativePlatformType, ETOControl>
    public class WinGLSurfaceHandler : WindowsControl<WinGLUserControl, GLSurface>, IGLSurface
    {
        public int texture;
        public System.Drawing.Bitmap bitmap;

        public override WinGLUserControl CreateControl()
        {
            return new WinGLUserControl();
        }
        protected override void Initialize()
        {
            base.Initialize();

            this.bitmap = SharpFlame.UI.Properties.Resources.TileTest;

            this.Control.Load += GL_Load;
            this.Control.Resize += GL_Resize;
            this.Control.Paint += GL_Paint;
        }

        void GL_Paint( object sender, System.Windows.Forms.PaintEventArgs e )
        {
            GL.Clear( ClearBufferMask.ColorBufferBit );

            GL.MatrixMode( MatrixMode.Modelview );
            GL.LoadIdentity();
            GL.BindTexture( TextureTarget.Texture2D, texture );


            GL.Begin( BeginMode.Quads );


            GL.TexCoord2( 0.0f, 1.0f ); GL.Vertex2( -0.6f, -0.4f );
            GL.TexCoord2( 1.0f, 1.0f ); GL.Vertex2( 0.6f, -0.4f );
            GL.TexCoord2( 1.0f, 0.0f ); GL.Vertex2( 0.6f, 0.4f );
            GL.TexCoord2( 0.0f, 0.0f ); GL.Vertex2( -0.6f, 0.4f );

            GL.End();

            this.Control.SwapBuffers();

            var test = 1 + 1;
        }

        void GL_Resize( object sender, EventArgs e )
        {
            GL.Viewport( 0, 0, this.Size.Width, this.Size.Height );
            GL.MatrixMode( MatrixMode.Projection );
            GL.LoadIdentity();
            GL.Ortho( -1.0, 1.0, -1.0, 1.0, 0.0, 4.0 );
        }

        void GL_Load( object sender, EventArgs e )
        {
            var w = this.Control.Width;
            var h = this.Control.Height;

            this.Control.MakeCurrent();

            GL.ClearColor( System.Drawing.Color.SkyBlue );
            GL.Enable( EnableCap.Texture2D );

            GL.Hint( HintTarget.PerspectiveCorrectionHint, HintMode.Nicest );

            GL.GenTextures( 1, out texture );
            GL.BindTexture( TextureTarget.Texture2D, texture );
            GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Linear );
            GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear );


            var data = bitmap.LockBits( new System.Drawing.Rectangle( 0, 0, bitmap.Width, bitmap.Height ),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb );

            GL.TexImage2D( TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0 );

            bitmap.UnlockBits( data );

            var test = 1 + 1;
        }
    }
}