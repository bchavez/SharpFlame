using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace SharpFlame.Tests.Gui
{
    public partial class frmMain : Form
    {
        private Bitmap bitmap = new Bitmap(@"C:\Code\Projects\Public\SharpFlame\source\Data\tilesets\tertilesc1hw\tertilesc1hw-128\tile-01.png");
        private int texture;

        private bool glcLoaded = false;

        public frmMain()
        {
            InitializeComponent();
            this.glc.Dock = DockStyle.Fill;
        }

        private void glc_Load( object sender, EventArgs e )
        {
            this.glcLoaded = true;

            glc.MakeCurrent();

            GL.ClearColor( Color.SkyBlue );
            GL.Enable(EnableCap.Texture2D);

            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

            GL.GenTextures(1, out texture);
            GL.BindTexture(TextureTarget.Texture2D, texture);
            GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Linear );
            GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear );


            var data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

            bitmap.UnlockBits(data);

            var test = 1 + 1;
        }

        private void SetupViewPort()
        {
            var w = this.glc.Width;
            var h = this.glc.Height;

            //glc.MakeCurrent();

            //GL.MatrixMode((All)MatrixMode.Projection);
            //GL.LoadIdentity();
            //GL.Ortho(0, w, 0, h, -1, 1);
            //GL.Viewport(0, 0, w, h);

        }

        private void glc_Paint( object sender, PaintEventArgs e )
        {
            if ( !glcLoaded )
                return;

            GL.Clear(ClearBufferMask.ColorBufferBit);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.BindTexture(TextureTarget.Texture2D, texture);


            GL.Begin(BeginMode.Quads);


            GL.TexCoord2( 0.0f, 1.0f ); GL.Vertex2( -0.6f, -0.4f );
            GL.TexCoord2( 1.0f, 1.0f ); GL.Vertex2( 0.6f, -0.4f );
            GL.TexCoord2( 1.0f, 0.0f ); GL.Vertex2( 0.6f, 0.4f );
            GL.TexCoord2( 0.0f, 0.0f ); GL.Vertex2( -0.6f, 0.4f );

            GL.End();

            glc.SwapBuffers();

            var test = 1 + 1;
        }

        private void glc_Resize( object sender, EventArgs e )
        {
            GL.Viewport(0, 0, Width, Height);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(-1.0, 1.0, -1.0, 1.0, 0.0, 4.0);
        }
    }
}
