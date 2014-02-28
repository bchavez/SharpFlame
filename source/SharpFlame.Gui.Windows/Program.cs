using System;
using Eto;
using SharpFlame.Gui;

namespace SharpFlame.Gui.Windows
{
    class Startup
    {
        [STAThread]
        static void Main(string[] args)
        {
            var generator = Generator.GetGenerator(Generators.WinAssembly);
            var app = new SharpFlameApplication(generator);
            app.Run(args);
            
        }
    }
}