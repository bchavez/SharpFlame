using System;
using Eto.Gl;
using Eto.Gl.Mac;
using Ninject;
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
            var macGenerator = Eto.Platform.Detect;
			macGenerator.Add<GLSurface.IHandler>( () => new MacGLSurfaceHandler() );

            //var app = new SharpFlameApplication( macGenrator );

            var kernel = Bootstrap.KernelWith(macGenerator);

            var app = kernel.Get<SharpFlameApplication>();

            app.Run( args );
        }
    }
}
