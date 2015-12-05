using System;
using System.Diagnostics;
using Eto.Gl;
using Eto.Gl.Gtk;
using Ninject;
using SharpFlame.Gui;
using OpenTK;
using SharpFlame.Infrastructure;

namespace SharpFlame.Gui.Linux
{
	class Startup
	{
		[STAThread]
		static void Main (string [] args)
        {
            // SDL does not currently support embedding
            // on external windows. If Open.Toolkit is not yet
            // initialized, we'll try to request a native backend
            // that supports embedding.
            // Most people are using GLControl through the
            // WinForms designer in Visual Studio. This approach
            // works perfectly in that case.
            Toolkit.Init(new ToolkitOptions
            {
                Backend = PlatformBackend.PreferNative
            });

			#if DEBUG
			Debug.Listeners.Add (new ConsoleTraceListener());
			#endif

			var p = Eto.Platform.Detect;
            p.Add<GLSurface.IHandler>(()=> new GtkGlSurfaceHandler());

		    var kernel = Bootstrap.KernelWith(Eto.Platform.Instance);

            //var app = new SharpFlameApplication (generator);
		    var app = new SharpFlameApplication();

		    app.Run();
        }
	}
}

