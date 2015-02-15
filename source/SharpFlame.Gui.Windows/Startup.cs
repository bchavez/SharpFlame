using System;
using Eto.Gl;
using Eto.Gl.Windows;
using Ninject;
using SharpFlame.Gui.Windows.EtoCustom;
using SharpFlame.Infrastructure;

namespace SharpFlame.Gui.Windows
{
    class Startup
    {
        [STAThread]
        static void Main(string[] args)
        {
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

            var generator = Eto.Platform.Detect;

            generator.Add<GLSurface.IHandler>(() => new WinGLSurfaceHandler());
            generator.Add<Eto.Forms.Panel.IHandler>(() => new WinPanelHandler());

            var kernel = Bootstrap.KernelWith(Eto.Platform.Instance);
            
            var app = kernel.Get<SharpFlameApplication>();

            app.Run(args);
            
        }
    }
}