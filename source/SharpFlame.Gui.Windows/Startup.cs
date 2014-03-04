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

            var kernel = Bootstrap.Kernel(generator);
            
            var app = kernel.Get<SharpFlameApplication>();

            kernel.Inject(app);
            

            app.Run(args);
            
        }
    }
}