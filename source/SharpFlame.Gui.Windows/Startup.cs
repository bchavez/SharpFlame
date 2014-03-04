using System;
using System.Collections.Generic;
using Eto.Forms;
using Ninject;
using Ninject.Components;
using Ninject.Modules;
using Ninject.Parameters;
using SharpFlame.Gui.Controls;
using SharpFlame.Gui.NinjectBindings;
using SharpFlame.Gui.Windows.EtoCustom;

namespace SharpFlame.Gui.Windows
{
    class Startup
    {
        [STAThread]
        static void Main(string[] args)
        {
            var generator = new Eto.Platform.Windows.Generator();

            generator.Add<IGLSurfaceHandler>(() => new WinGLSurfaceHandler());
            generator.Add<IPanel>(() => new WinPanelHandler());


            var settings = new NinjectSettings
                {
                    InjectNonPublic = true,
                    LoadExtensions = false,
                };

            var kernelModules = new List<INinjectModule>
                {
                    new Ninject.Extensions.NamedScope.NamedScopeModule(),
                    new Ninject.Extensions.ContextPreservation.ContextPreservationModule(),
                    new Ninject.Extensions.bbvEventBroker.EventBrokerModule(),
                    new Ninject.Extensions.Logging.NLog2.NLogModule(),
                    new SharpFlameModule(),
                };

            var kernel = new StandardKernel(settings, kernelModules.ToArray());

            NinjectHook.HookGenerator(generator, kernel);

            var app = new SharpFlameApplication(generator);

            kernel.Inject(app);
            

            app.Run(args);
            
        }
    }
}