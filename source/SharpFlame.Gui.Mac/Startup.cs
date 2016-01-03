using System;
using Eto.Gl;
using Eto.Gl.Mac;
using Ninject;
using OpenTK;
using SharpFlame.Infrastructure;

namespace SharpFlame.Gui.Mac
{
    public static class Startup
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
        {
	        Toolkit.Init();

            var p = Eto.Platform.Detect;
			p.Add<GLSurface.IHandler>( () => new MacGLSurfaceHandler() );

            var kernel = Bootstrap.KernelWith(Eto.Platform.Instance);

            var app = new SharpFlameApplication();

            app.Run();
        }
    }
}
