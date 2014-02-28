using System;
using System.Diagnostics;
using Eto.Platform.Mac.Forms;
using Eto.Platform.Mac.Forms.Controls;
using MonoMac.AppKit;
using SharpFlame.UI.Controls;
using MonoMac.OpenGL;
using System.Drawing;

namespace SharpFlame.UI.Mac
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
			macGenrator.Add<IGLSurface>( () => new GLViewHandler() );


            var app = new SharpFlameEtoApplication( macGenrator );

            app.Run( args );
        }
    }
		

	internal class GLViewHandler : MacView<MacGLView, GLSurface>, IGLSurface
    {
        public GLViewHandler()
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




	class MacGLView : NSOpenGLView{


		public MacGLView(){
			Debugger.Break ();
		}

		public delegate void DrawRectHandler(System.Drawing.RectangleF dirtyRect);
		public event DrawRectHandler DrawNow = delegate{};
		public override void DrawRect(System.Drawing.RectangleF dirtyRect){
			this.DrawNow (dirtyRect);
		}

		
	}



}
