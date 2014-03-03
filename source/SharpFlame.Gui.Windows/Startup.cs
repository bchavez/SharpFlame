using System;
using Eto;
using Eto.Forms;
using SharpFlame.Gui;
using SharpFlame.Gui.Controls;
using SharpFlame.Gui.Windows.Eto;

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

            var app = new SharpFlameApplication(generator);

            app.Run(args);
            
        }
    }
}