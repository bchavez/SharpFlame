using Eto.Platform.Windows;
using SharpFlame.Gui.Controls;

namespace SharpFlame.Gui.Windows.EtoCustom
{
    public class WinGLSurfaceHandler : WindowsControl<WinGLUserControl, GLSurface>, IGLSurfaceHandler
    {
        public override WinGLUserControl CreateControl()
        {
            return new WinGLUserControl();
        }
    }
}