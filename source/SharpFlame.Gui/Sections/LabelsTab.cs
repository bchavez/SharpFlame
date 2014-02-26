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

using Eto.Forms;
using Eto.Drawing;

namespace SharpFlame.Gui.Sections
{
    public class LabelsTab : Panel
    {
        public LabelsTab ()
        {
            var mainLayout = new DynamicLayout ();

            var nLayout0 = new DynamicLayout ();
            nLayout0.AddRow(new Button { Text = "Create Area From Selection" }, 
                            new Label { Text = "Hold P and click to make positions.", VerticalAlign = VerticalAlign.Middle, Size = new Size (150, 28) });
            mainLayout.AddRow (null, nLayout0, null);

            var nLayout1 = new DynamicLayout ();
            nLayout1.AddRow (new Label { Text = "Positions:", VerticalAlign = VerticalAlign.Bottom },
                             new Label { Text = "Areas:", VerticalAlign = VerticalAlign.Bottom });
            nLayout1.AddRow (new ListBox { Size = new Size (170, 150) }, new ListBox { Size = new Size (170, 150) });

            var smGroupBox = new GroupBox { Text = "Selected Marker" };
            var nLayout2 = new DynamicLayout ();
            var nLayout3 = new DynamicLayout ();
            nLayout3.AddRow (new Label { Text = "Label:", VerticalAlign = VerticalAlign.Middle },
                             new TextBox());
            nLayout2.Add (nLayout3);

            var nLayout4 = new DynamicLayout ();
            nLayout4.AddRow(new Label { Text = "x:", VerticalAlign = VerticalAlign.Bottom },
                            new Label { Text = "y:", VerticalAlign = VerticalAlign.Bottom });
            nLayout4.AddRow(new TextBox {Size = new Size (75, 25)},
                            new TextBox {Size = new Size (75, 25)});

            nLayout4.AddRow(new Label { Text = "x2:", VerticalAlign = VerticalAlign.Bottom },
                            new Label { Text = "y2:", VerticalAlign = VerticalAlign.Bottom });
            nLayout4.AddRow(new TextBox {Size = new Size (75, 25)},
                            new TextBox {Size = new Size (75, 25)});

            nLayout2.Add (nLayout4);
            nLayout2.Add (new Button { Text = "Remove" });

            smGroupBox.Content = nLayout2;

            nLayout1.AddRow (smGroupBox);


            mainLayout.AddRow (null, nLayout1, null);

            mainLayout.Add (null);

            Content = mainLayout;
        }
    }
}

