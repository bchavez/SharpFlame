using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Eto.Platform.Mac.Forms;
using Eto.Platform.Mac.Forms.Controls;
using MonoMac.AppKit;
using SharpFlame.Test.UI;
using SharpFlame.UI.Controls;

namespace SharpFlame.UI.Mac
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            var macGenrator = new Eto.Platform.Mac.Generator();
            macGenrator.Add<IGLSurface>( () => new MacGLSurfaceHandler() );


            var app = new SharpFlameEtoApplication( macGenrator );

            app.Run( args );
        }
    }

    internal class MacGLSurfaceHandler : MacControl<NSControl, GLSurface>, IGLSurface
    {
    }

    internal class GLViewHandler : MacView<NSOpenGLView, GLSurface>, IGLSurface
    {

        public override NSView ContainerControl
        {
            get { throw new NotImplementedException(); }
        }

        public override bool Enabled { get; set; }
    }
}
