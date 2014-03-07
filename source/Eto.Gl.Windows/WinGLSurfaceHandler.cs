using Eto.Platform.Windows;
namespace Eto.Gl.Windows
{
    public class WinGLSurfaceHandler : WindowsControl<WinGLUserControl, GLSurface>, IGLSurfaceHandler
    {
        public override WinGLUserControl CreateControl()
        {
            return new WinGLUserControl();
        }
    }
}