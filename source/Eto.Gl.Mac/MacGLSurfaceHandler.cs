using System;
using System.Drawing;
using Eto.Platform;
using Eto.Platform.Mac.Forms;
using MonoMac.AppKit;
using Size = Eto.Drawing.Size;

namespace Eto.Gl.Mac
{
	public class MacGLSurfaceHandler : MacView<MacGLView3, GLSurface>, IGLSurfacePlatformHandler
    {
		public override MacGLView3 CreateControl()
        {
			var c = new MacGLView3();
            c.Initialized += (sender, args) => Widget.OnInitialized(sender, args);
            c.Resize += (sender, args) => Widget.OnResize(sender, args);
            c.ShuttingDown += (sender, args) => Widget.OnShuttingDown(sender, args);
		    return c;
        }

        private void Control_GLMouseDown(MacGLView2 sender, NSEvent args)
        {
            var mouseEvent = Eto.Platform.Mac.Conversions.GetMouseEvent(sender, args, false);
            this.Widget.OnMouseDown(mouseEvent);
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