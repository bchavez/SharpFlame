using Eto.Drawing;
using Eto.Forms;
using Eto.WinForms.Forms;

namespace Eto.Gl.Windows
{
    public class WinGLSurfaceHandler : WindowsControl<WinGLUserControl, GLSurface, GLSurface.ICallback>, GLSurface.IHandler
    {
        protected override void Initialize()
        {

            var c = new WinGLUserControl();
            c.Initialized += (sender, args) => Widget.OnInitialized(sender, args);
            c.Resize += (sender, args) => Widget.OnResize(sender, args);
            c.ShuttingDown += (sender, args) => Widget.OnShuttingDown(sender, args);
            this.Control = c;

            base.Initialize();

        }

        public Size GLSize
        {
            get { return this.Control.GLSize; }
            set { this.Control.Size = value.ToSD(); }
        }

        public bool IsInitialized
        {
            get { return Control.IsInitialized; }
        }

        public void MakeCurrent()
        {
            this.Control.MakeCurrent();
        }

        public void SwapBuffers()
        {
            this.Control.SwapBuffers();
        }
    }
}