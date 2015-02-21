using System;
using Eto.Drawing;
using Eto.Gl;
using Eto.Gl.Windows;
using SharpFlame.Gui.Windows.EtoCustom;
using SharpFlame.Infrastructure;
using swf = System.Windows.Forms;
using Eto.WinForms;

namespace SharpFlame.Gui.Windows
{
    class Startup
    {
        [STAThread]
        static void Main(string[] args)
        {
            var p = Eto.Platform.Detect;

            p.Add<GLSurface.IHandler>(() => new WinGLSurfaceHandler());
            p.Add<Eto.Forms.Panel.IHandler>(() => new WinPanelHandler());

	        ApplyOsStyles();

            var kernel = Bootstrap.KernelWith(Eto.Platform.Instance);

            var app = new SharpFlameApplication();

            app.Run(args);
            
        }

	    private static void ApplyOsStyles()
	    {	
		    Eto.Style.Add<Eto.Forms.Button>("toggle", b =>
			    {
				    var msize = swf.TextRenderer.MeasureText("255", b.Font.ToSD());
				    var esize = msize.ToEto();
				    b.Size = new Size((esize.Width + b.Size.Width) / 2, b.Height);
				    Console.WriteLine();
			    });
	    }
    }
}