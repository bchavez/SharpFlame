#region License
// /*
// The MIT License (MIT)
//
// Copyright (c) 2013-2014 The SharpFlame Authors.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// */
#endregion

using Eto.Drawing;
using Eto.Forms;
using SharpFlame.Core;
using SharpFlame.Gui.UiOptions;

namespace SharpFlame.Gui.Sections
{
	public class ResizeTab : Panel
	{
		public ResizeTab ()
		{
			var mainLayout = new DynamicLayout ();

			var nLayout1 = new DynamicLayout ();
			nLayout1.AddRow(new Label { Text = "Size X:" }, 
							new NumericUpDown { Size = new Size(-1, -1), Value = 0, MaxValue = Constants.MapMaxSize, MinValue = 1 });

			nLayout1.AddRow(new Label { Text = "Size Y:" }, 
							new NumericUpDown { Size = new Size(-1, -1), Value = 0, MaxValue = Constants.MapMaxSize, MinValue = 1 });

			nLayout1.AddRow(new Label { Text = "Offset X:" }, 
			new NumericUpDown { Size = new Size(-1, -1), Value = 0, MaxValue = Constants.MapMaxSize, MinValue = 1 });

			nLayout1.AddRow(new Label { Text = "Offset Y:" }, 
			new NumericUpDown { Size = new Size(-1, -1), Value = 0, MaxValue = Constants.MapMaxSize, MinValue = 1 });

			mainLayout.AddRow (null, nLayout1, null);

			mainLayout.AddRow (null, new Button { Text = "Resize" }, null);
			mainLayout.AddRow (null, new Button { Text = "Resize To Selection" }, null);


			mainLayout.Add (null);

            setBindings ();

			Content = mainLayout;
		}

        void setBindings() {
            // Set Mousetool, when we are shown.
            Shown += delegate {
                App.UiOptions.MouseTool = MouseTool.Default;
            };
        }
	}
}

