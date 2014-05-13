using Eto.Drawing;
using Eto.Platform.GtkSharp;

namespace Eto.Gl.Gtk
{
    public class GtkGlSurfaceHandler : GtkControl<GLDrawingArea, GLSurface>, IGLSurfacePlatformHandler
    {
        public override GLDrawingArea CreateControl()
        {
            var c = new GLDrawingArea();
            c.Initialized += (sender, args) => Widget.OnInitialized(sender, args);
            c.Resize += (sender, args) => Widget.OnResize(sender, args);
            c.ShuttingDown += (sender, args) => Widget.OnShuttingDown(sender, args);
            return c;
        }

        public Size GLSize
        {
            get { return this.Control.GLSize; }
            set { this.GLSize = value; }
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
