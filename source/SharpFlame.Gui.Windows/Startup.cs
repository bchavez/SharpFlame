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
            var generator = Eto.Platform.Detect;

            generator.Add<GLSurface.IHandler>(() => new WinGLSurfaceHandler());
            generator.Add<Eto.Forms.Panel.IHandler>(() => new WinPanelHandler());

            var kernel = Bootstrap.KernelWith(Eto.Platform.Instance);

            var app = new SharpFlameApplication();

            app.Run(args);
            
        }
    }
}