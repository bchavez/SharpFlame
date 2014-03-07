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
using Eto.Drawing;
using Eto.Forms;
using SharpFlame.Settings;

namespace SharpFlame.Gui.Dialogs
{
    public class KeyInput : Dialog
    {
        private KeyboardKey key;
        public KeyboardKey Key { 
            get { return key; }
            set { 
                key = value;
                lblKey.Text = key.ToString();
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

            KeyboardKey keyDown = null;
            KeyUp += (object sender, KeyEventArgs e) => 
            {
                var currentKeyOnly = e.KeyData & Keys.KeyMask;
                var currentModifier = e.KeyData & Keys.ModifierMask;
                Keys keyDownModifier;
                if (keyDown.Key != null) {
                    keyDownModifier = (Keys)keyDown.Key & Keys.ModifierMask;
                }
                else
                {
                    keyDownModifier = Keys.None;
                }

                if (currentKeyOnly != Keys.None) {
                    // Is known key.
                    Key = new KeyboardKey("", e.KeyData, null, Key.Repeat);
                    Console.WriteLine("keyUP={0}", Key.ToString());
                } else if (e.IsChar) {
                    // Is Char
                    Key = new KeyboardKey("", null, e.KeyChar, Key.Repeat);
                    Console.WriteLine("keyUP={0}", Key.ToString());
                } else if (keyDownModifier != Keys.None && currentModifier != keyDownModifier) {
                    // Is modifier only
                    Key = new KeyboardKey("", e.KeyData, null, Key.Repeat);
                    Console.WriteLine("keyUP={0}", Key.ToString());
                } 
            };

            KeyDown += (object sender, KeyEventArgs e) => 
            {
                var currentKeyOnly = e.KeyData & Keys.KeyMask;
                if (currentKeyOnly != Keys.None) {
                    // Is known key.
                    keyDown = new KeyboardKey("", e.KeyData, null);
                    Console.WriteLine("keyDown={0}", keyDown.ToString());
                } else if (e.IsChar) {
                    // Is Char
                    keyDown = new KeyboardKey("", null, e.KeyChar);
                    Console.WriteLine("keyDown={0}", keyDown.ToString());
                } else {
                    // Is modifier only
                    keyDown = new KeyboardKey("", e.KeyData, null);
                    Console.WriteLine("keyDown={0}", keyDown.ToString());                   
                }
            };

            Content = layout;
        }
    }
}

