using System;
using Eto.Platform.GtkSharp;
using SharpFlame.Test.UI;
using SharpFlame.UI.Controls;

namespace SharpFlame.UI.Linux
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            var linuxGenerator = new Eto.Platform.GtkSharp.Generator();
            linuxGenerator.Add<IGLSurface>(()=> new LinuxGLSurfaceHandler());

            var app = new SharpFlameEtoApplication(linuxGenerator);
            app.Run(args);
        }
    }

    internal class LinuxGLSurfaceHandler : GtkControl<GLDrawingArea, GLSurface>, IGLSurface
    {
    }
}
