using System.Diagnostics;
using SharpFlame.Gui;
using SharpFlame.Gui.Controls;
using OpenTK;

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

            var app = new SharpFlameApplication (generator);
			app.Run (args);
		}
	}
}

