using Eto.Drawing;
using Eto.GtkSharp.Forms;
using OpenTK.Graphics;

namespace Eto.Gl.Gtk
{
    public class GtkGlSurfaceHandler : GtkControl<GLDrawingArea, GLSurface, GLSurface.ICallback>, GLSurface.IHandler
    {
	    private GraphicsMode mode;
	    private int major;
	    private int minor;
	    private GraphicsContextFlags flags;

	    protected override void Initialize()
        {
		    var c = new GLDrawingArea(mode, major, minor, flags);
            c.Initialized += (sender, args) => Widget.OnInitialized(sender, args);
            c.Resize += (sender, args) => Widget.OnResize(sender, args);
            c.ShuttingDown += (sender, args) => Widget.OnShuttingDown(sender, args);
            this.Control = c;

            base.Initialize();
        }

	    public void CreateWithParams(GraphicsMode mode, int major, int minor, GraphicsContextFlags flags)
	    {
		    this.mode = mode;
		    this.major = major;
		    this.minor = minor;
		    this.flags = flags;
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
