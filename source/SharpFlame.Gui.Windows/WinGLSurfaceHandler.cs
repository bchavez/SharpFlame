using System;
using System.Drawing.Imaging;
using Eto.Platform.Windows;
using OpenTK.Graphics.OpenGL;
using SharpFlame.Gui.Controls;

namespace SharpFlame.Gui.Windows
{
    public class WinGLSurfaceHandler : WindowsControl<WinGLUserControl, GLSurface>, IGLSurfaceHandler
    {
        public override WinGLUserControl CreateControl()
        {
            return new WinGLUserControl();
        }
    }
}