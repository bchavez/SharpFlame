using System;
using System.Diagnostics;
using System.Drawing;
using Eto.Forms;
using Eto.Platform;
using Eto.Platform.Mac;
using Eto.Platform.Mac.Forms;
using MonoMac.AppKit;
using MonoMac.OpenGL;
using SharpFlame.Gui.Controls;
using Size = Eto.Drawing.Size;

namespace SharpFlame.Gui.Mac
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            var macGenrator = new Eto.Platform.Mac.Generator();
			macGenrator.Add<IGLSurfaceHandler>( () => new MacGLSurfaceHandler() );

            var app = new SharpFlameApplication( macGenrator );

            app.Run( args );
        }
    }
		

	internal class MacGLSurfaceHandler : MacView<MacGLView, GLSurface>, IGLSurfaceHandler
    {
		public override MacGLView CreateControl()
		{
			return new MacGLView ();
		}

        public override void AttachEvent( string id )
        {
            switch( id )
            {
                case "Control.MouseEnter":
                    break;

                case "Control.MouseLeave":

                    break;
                case "Control.MouseMove":
                    break;

                case "Control.SizeChanged":
                    break;

                case "Control.MouseDown":
                    this.Control.GLMouseDown += Control_GLMouseDown;
                    break;

                case "Control.MouseUp":
                    break;

                case "Control.MouseDoubleClick":
                    break;

                case "Control.MouseWheel":
                    break;

                case "Control.KeyDown":
                    break;

                case "Control.KeyUp":
                    break;

                case "Control.LostFocus":
                    break;

                case "Control.GotFocus":
                    break;

                case "Control.Shown":
                    break;

                default:
                    base.AttachEvent( id );
                    break;
            }

        }

        void Control_GLMouseDown( MacGLView sender, NSEvent args )
        {
            var mouseEvent = Eto.Platform.Mac.Conversions.GetMouseEvent(sender, args, false);
            this.Widget.OnMouseDown(mouseEvent);
        }

        public override bool Enabled{	get;	set;	}

		public override NSView ContainerControl
		{
			get {
				return this.Control;
			}
		}

    }


    internal class MacGLView : NSOpenGLView, IGLSurface
    {
	    public delegate void GLEventHandler(MacGLView sender, NSEvent args );

        public delegate void DrawRectHandler(System.Drawing.RectangleF dirtyRect);

        public event DrawRectHandler DrawNow = delegate { };

        public override void DrawRect(System.Drawing.RectangleF dirtyRect)
        {
            this.DrawNow(dirtyRect);
        }
        
        public override void PrepareOpenGL()
        {
            base.PrepareOpenGL();
            this.IsInitialized = true;
            this.Initialized( this, EventArgs.Empty );
        }

        public override void Reshape()
        {
            base.Reshape();
            this.Resize( this, EventArgs.Empty );
        }

        public override void ViewDidEndLiveResize()
        {
            base.ViewDidEndLiveResize();
            this.Resize(this, EventArgs.Empty);
        }


        public Size GLSize
        {
            get { return this.Bounds.Size.ToSize().ToEto(); }
            set
            {
                this.Bounds =
                    new RectangleF(this.Bounds.X, this.Bounds.Y, value.Width, value.Height);
            }
        }

        public bool IsInitialized { get; private set; }

        public void MakeCurrent()
        {
            this.OpenGLContext.MakeCurrentContext();
        }

        public void SwapBuffers()
        {
            this.OpenGLContext.FlushBuffer();
        }

        public event EventHandler Initialized = delegate { };

        public event EventHandler Resize = delegate {  };

        public event EventHandler ShuttingDown = delegate { };


        public event GLEventHandler GLKeyDown = delegate { };
        public override void KeyDown( NSEvent theEvent )
        {
            base.KeyDown( theEvent );
            GLKeyDown(this, theEvent);
        }

        public event GLEventHandler GLMouseDown = delegate { };
        public override void MouseDown( NSEvent theEvent )
        {
            base.MouseDown( theEvent );
            GLMouseDown(this, theEvent);
        }
        public event GLEventHandler GLMouseUp = delegate { };
        public override void MouseUp( NSEvent theEvent )
        {
            base.MouseUp( theEvent );
            GLMouseUp(this, theEvent);
        }

        public event GLEventHandler GLMouseDragged = delegate { };
        public override void MouseDragged( NSEvent theEvent )
        {
            base.MouseDragged( theEvent );
            GLMouseDragged(this, theEvent);
        }
        public event GLEventHandler GLMouseMoved = delegate { };
        public override void MouseMoved( NSEvent theEvent )
        {
            base.MouseMoved( theEvent );
            GLMouseMoved(this, theEvent);
        }

        public event GLEventHandler GLScrollWheel = delegate { };
        public override void ScrollWheel(NSEvent theEvent)
        {
            base.ScrollWheel(theEvent);
            GLScrollWheel(this, theEvent);
        }
    }
}
