using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using SharpFlame.Test.UI;
using SharpFlame.UI.Controls;

namespace SharpFlame.UI.Windows
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            var winGenrator = new Eto.Platform.Windows.Generator();
            winGenrator.Add<IGLSurface>(() => new WinGLSurfaceHandler());


            var app = new SharpFlameEtoApplication(winGenrator);

            app.Run(args);
        }
    }
}
