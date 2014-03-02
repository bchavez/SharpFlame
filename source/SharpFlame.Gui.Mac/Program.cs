using System;
using System.Diagnostics;
using System.Drawing;
using Eto.Platform;
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
        public MacGLSurfaceHandler()
        {
            Debugger.Break();
        }

		public override MacGLView CreateControl()
		{
			return new MacGLView ();
		}

		protected override void Initialize()
		{
			base.Initialize ();
			this.Control.DrawNow += ctrl_DrawNow;	
		}

		private void ctrl_DrawNow(System.Drawing.RectangleF rect){

			this.Control.OpenGLContext.MakeCurrentContext ();

			GL.ClearColor(Color.Brown);

			GL.ClearColor (0, 0, 0, 0);
			GL.Clear(ClearBufferMask.ColorBufferBit);

			DrawTriangle ();

			GL.Flush ();

		}


		private void DrawTriangle(){
			GL.Color3 (1.0f, 0.85f, 0.35f);
			GL.Begin (BeginMode.Triangles);

			GL.Vertex3 (0.0, 0.6, 0.0);
			GL.Vertex3 (-0.2, -0.3, 0.0);
			GL.Vertex3 (0.2, -0.3 ,0.0);

			GL.End ();


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

        public MacGLView()
        {
            Debugger.Break();
        }

        public delegate void DrawRectHandler(System.Drawing.RectangleF dirtyRect);

        public event DrawRectHandler DrawNow = delegate { };

        public override void DrawRect(System.Drawing.RectangleF dirtyRect)
        {
            this.DrawNow(dirtyRect);
        }
        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            this.IsInitialized = true;
            this.Initialized(this, EventArgs.Empty);
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
    }
}
