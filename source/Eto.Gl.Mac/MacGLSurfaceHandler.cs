
using Eto.Drawing;
using MonoMac.AppKit;
using Eto.Mac.Forms;

namespace Eto.Gl.Mac
{
	public class MacGLSurfaceHandler : MacView<MacGLView7, GLSurface, GLSurface.ICallback>, GLSurface.IHandler
    {
        protected override void Initialize()
        {

            var c = new MacGLView7();
            c.Initialized += (sender, args) => Widget.OnInitialized(sender, args);
            c.Resize += (sender, args) => Widget.OnResize(sender, args);
            c.ShuttingDown += (sender, args) => Widget.OnShuttingDown(sender, args);

            this.Control = c;

            base.Initialize();
        }

        private void Control_GLMouseDown(MacGLView2 sender, NSEvent args)
        {
            var mouseEvent = Eto.Mac.Conversions.GetMouseEvent(sender, args, false);
            this.Callback.OnMouseDown(this.Widget, mouseEvent);
        }

        public override bool Enabled { get; set; }

        public override NSView ContainerControl
        {
            get { return this.Control; }
        }

        public Size GLSize
        {
            get { return this.Control.GLSize; }
            set
            {

            }
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