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

using System;
using Eto.Forms;
using Eto.Drawing;

namespace SharpFlame.Gui.Dialogs
{
    public class KeyInput : Dialog
    {
        private Keys key;
        public Keys Key { 
            get { return key; }
            set { 
                key = value;
                lblKey.Text = key.ToShortcutString ();
            }
        }

        private readonly Label lblKey;

        public KeyInput ()
        {
            Title = "Enter a key";
            Resizable = true;
            Topmost = true;
            Size = new Size (220, 70);

            var layout = new DynamicLayout ();
            layout.AddCentered(lblKey = new Label { VerticalAlign = VerticalAlign.Middle });

            var keyDown = Keys.None;
            KeyUp += (object sender, KeyEventArgs e) => 
            {
                if (e.KeyData.HasFlag(Keys.Control) && keyDown.HasFlag(Keys.Control)) {
                    Key = keyDown;
                } else if (e.KeyData.HasFlag(Keys.Shift) && keyDown.HasFlag(Keys.Shift)) {
                    Key = keyDown;
                } else if (e.KeyData.HasFlag(Keys.Alt) && keyDown.HasFlag(Keys.Alt)) {
                    Key = keyDown;
                } else {
                    Key = e.KeyData;
                }

                Console.WriteLine ("UP Key: {0}, Char: {1}, Handled: {2}", e.KeyData, e.IsChar ? e.KeyChar.ToString() : "no char", e.Handled);
            };

            KeyDown += (object sender, KeyEventArgs e) => 
            {
                keyDown = e.KeyData;

                Console.WriteLine ("DOWN Key: {0}, Char: {1}, Handled: {2}", e.KeyData, e.IsChar ? e.KeyChar.ToString() : "no char", e.Handled);
            };

            Content = layout;
        }
    }
}

