

using Eto.Platform.GtkSharp;

namespace Eto.Gl
{
    public class GtkGlSurfaceHandler : GtkControl<GLDrawingArea, GLSurface>, IGLSurfaceHandler
    {
        public GtkGlSurfaceHandler()
        {
            this.Control = new GLDrawingArea();
        }
    }
}
