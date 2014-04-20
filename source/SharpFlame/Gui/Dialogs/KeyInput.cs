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

