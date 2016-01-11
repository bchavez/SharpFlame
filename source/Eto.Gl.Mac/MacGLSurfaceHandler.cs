
using System.Diagnostics;
using Eto.Drawing;
using MonoMac.AppKit;
using Eto.Mac.Forms;
using OpenTK.Graphics;

namespace Eto.Gl.Mac
{
	public class MacGLSurfaceHandler : MacView<MacGLView8, GLSurface, GLSurface.ICallback>, GLSurface.IHandler
    {
	    private GraphicsMode mode;
	    private int major;
	    private int minor;
	    private GraphicsContextFlags flags;

		public MacGLSurfaceHandler()
		{
			//Debugger.Break();
		}

		protected override void Initialize()
        {
            var c = new MacGLView8();

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

	    public override bool Enabled { get; set; }

	    public override NSView ContainerControl
        {
            get { return this.Control; }
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

                default:
                    base.AttachEvent(id);
                    break;
            }
        }
    }
}