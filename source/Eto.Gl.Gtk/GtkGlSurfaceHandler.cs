using Eto.Drawing;
using Eto.GtkSharp;
using Eto.GtkSharp.Forms;

namespace Eto.Gl.Gtk
{
    public class GtkGlSurfaceHandler : GtkControl<GLDrawingArea, GLSurface, GLSurface.ICallback>, GLSurface.IHandler
    {
        protected override void Initialize()
        {
            base.Initialize();

            var c = new GLDrawingArea();
            c.Initialized += (sender, args) => Widget.OnInitialized(sender, args);
            c.Resize += (sender, args) => Widget.OnResize(sender, args);
            c.ShuttingDown += (sender, args) => Widget.OnShuttingDown(sender, args);
            this.Control = c;
        }

        public Size GLSize
        {
            get { return this.Control.GLSize; }
            set { this.Control.GLSize = value; }
        }

        public bool IsInitialized
        {
            get { return this.Control.IsInitialized; }
        }

        public void MakeCurrent()
        {
            this.Control.MakeCurrent();
        }

        public void SwapBuffers()
        {
            this.Control.SwapBuffers();
        }
    }

}
