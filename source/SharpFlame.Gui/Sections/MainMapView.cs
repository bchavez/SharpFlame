#region License
 /*
 The MIT License (MIT)

 Copyright (c) 2013-2014 The SharpFlame Authors.

 Permission is hereby granted, free of charge, to any person obtaining a copy
 of this software and associated documentation files (the "Software"), to deal
 in the Software without restriction, including without limitation the rights
 to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 copies of the Software, and to permit persons to whom the Software is
 furnished to do so, subject to the following conditions:

 The above copyright notice and this permission notice shall be included in
 all copies or substantial portions of the Software.

 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 THE SOFTWARE.
 */
#endregion

using Eto.Forms;
using Ninject;
using OpenTK.Graphics.OpenGL;
using SharpFlame.Gui.Controls;
using SharpFlame.Gui.Infrastructure;
using SharpFlame.Old.Settings;

namespace SharpFlame.Gui.Sections
{
    public class MainMapView : Panel, IInitializable
	{
        [Inject]
        internal KeyboardManager KeyboardManager { get; set; }

        [Inject, Named(NamedBinding.MapView)]
        internal GLSurface GLSurface { get; set; }

        /// <summary>
        /// Ninject initialize this instance.
        /// </summary>
        void IInitializable.Initialize()
        {
		    var mainLayout = new DynamicLayout();
            mainLayout.AddSeparateRow(
                new Label { }
            );
            mainLayout.Add(GLSurface, true, true);
            mainLayout.AddSeparateRow(
                new Label { }
            );

		    Content = mainLayout;           

            setBindings();
		}

        void setBindings() 
        {
            this.GLSurface.KeyDown += this.KeyboardManager.HandleKeyDown;
            this.GLSurface.KeyUp += this.KeyboardManager.HandleKeyUp;

            this.GLSurface.Resize += (sender, args) =>
            {
                ResizeMapView();
            };
        }

        private void ResizeMapView()
        {
            this.GLSurface.MakeCurrent();

            var glSize = this.Size;

            // send the resize event to the Graphics card.
            GL.Viewport(0, 0, glSize.Width, glSize.Height);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(-1.0, 1.0, -1.0, 1.0, 0.0, 4.0);
            GL.MatrixMode (MatrixMode.Modelview);

            DrawMapView();
        }
 
        private void DrawMapView()
        {
            this.GLSurface.MakeCurrent();

            GL.Clear(ClearBufferMask.ColorBufferBit);

            this.GLSurface.SwapBuffers();
        }
	}
}

