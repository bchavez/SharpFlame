using System;
using Eto;

namespace SharpFlame.Gui.Wpf
{
    class Startup
    {
        [STAThread]
        static void Main(string[] args)
        {
            var generator = Generator.GetGenerator(Generators.WpfAssembly);
            var app = new SharpFlameApplication(generator);
            app.Run(args);
        }
    }
}