using Eto.Drawing;
using Eto.Forms;
using Eto.WinForms.Forms;
using OpenTK.Graphics;

namespace Eto.Gl.Windows
{
    public class WinGLSurfaceHandler : WindowsControl<WinGLUserControl, GLSurface, GLSurface.ICallback>, GLSurface.IHandler
    {
	    private GraphicsMode mode;
	    private int major;
	    private int minor;
	    private GraphicsContextFlags flags;

	    protected override void Initialize()
        {
            var c = new WinGLUserControl(mode, major, minor, flags);

            c.Initialized += (sender, args) =>
                {
                    this.Callback.OnInitialized(Widget, args);
                };
            c.Resize += (sender, args) =>
                {
                    this.Callback.OnSizeChanged(Widget, args);
                };
            c.ShuttingDown += (sender, args) =>
                {
                    this.Callback.OnShuttingDown(Widget, args);
                };

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
            set { this.Control.Size = value.ToSD(); }
        }

        public bool IsInitialized
        {
            get { return Control.IsInitialized; }
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