using System;
using System.Drawing.Imaging;
using Eto.Platform.Windows;
using OpenTK.Graphics.OpenGL;
using SharpFlame.Gui.Controls;

namespace SharpFlame.Gui.Windows
{
    public class WinGLSurfaceHandler : WindowsControl<WinGLUserControl, GLSurface>, IGLSurfaceHandler
    {
        public override WinGLUserControl CreateControl()
        {
            return new WinGLUserControl();
        }
        protected override void Initialize()
        {
            base.Initialize();


            //this.Control.Load += GL_Load;
            //this.Control.Resize += GL_Resize;
            //this.Control.Paint += GL_Paint;
        }

        //void GL_Paint( object sender, System.Windows.Forms.PaintEventArgs e )
        //{

        //}

        //void GL_Resize( object sender, EventArgs e )
        //{

        //}

        //void GL_Load( object sender, EventArgs e )
        //{

        //}
    }
}