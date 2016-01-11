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

        public override void AttachEvent(string id)
        {
            switch (id)
            {
                case GLSurface.GLInitializedEvent:
                    this.Control.Initialized += (sender, args) => Callback.OnInitialized(this.Widget, args);
                    break;

                case GLSurface.GLShuttingDownEvent:
                    this.Control.ShuttingDown += (sender, args) => Callback.OnShuttingDown(this.Widget, args);
                    break;

                case Eto.Forms.Control.SizeChangedEvent:
                    this.Control.Resize += (sender, args) => Callback.OnResize(this.Widget, args);
                    break;

                default:
                    base.AttachEvent(id);
                    break;
            }
        }
    }

}
