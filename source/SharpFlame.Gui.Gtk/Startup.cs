using System;
using System.Diagnostics;
using Eto.Gl;
using Ninject;
using SharpFlame.Gui;
using OpenTK;
using SharpFlame.Infrastructure;

namespace SharpFlame.Gui.Gtk
{
	class Startup
	{
		//[STAThread]
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
			var generator = new Eto.Platform.GtkSharp.Generator ();
            generator.Add<IGLSurfaceHandler>(()=> new LinuxGLSurfaceHandler());

		    var kernel = Bootstrap.KernelWith(generator);

            //var app = new SharpFlameApplication (generator);
		    var app = kernel.Get<SharpFlameApplication>();

            app.Run (args);
		}
	}
}

