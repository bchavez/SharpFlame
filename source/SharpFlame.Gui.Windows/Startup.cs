using System;
using Eto.Forms;
using Ninject;
using SharpFlame.Gui.Controls;
using SharpFlame.Gui.Infrastructure;
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

            var kernel = Bootstrap.KernelWith(generator);
            
            var app = kernel.Get<SharpFlameApplication>();

            kernel.Inject(app);
            

            app.Run(args);
            
        }
    }
}