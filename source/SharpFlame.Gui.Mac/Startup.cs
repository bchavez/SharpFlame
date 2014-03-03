using System;
using SharpFlame.Gui.Controls;

namespace SharpFlame.Gui.Mac
{
    static class Startup
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
}
