using System;
using Eto.Forms;
using Eto.Gl;
using Eto.Gl.Windows;
using Ninject;
using SharpFlame.Infrastructure;

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