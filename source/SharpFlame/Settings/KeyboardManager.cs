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
using System.Collections.Generic;
using System.Diagnostics;
using Appccelerate.EventBroker;
using Eto.Forms;
using Ninject.Extensions.Logging;

namespace SharpFlame.Settings
{
    public sealed class InvalidKeyException : Exception
    {
        public InvalidKeyException()
        {
        }

        public InvalidKeyException(string message) 
            : base(message)
        {
        }

        public InvalidKeyException(string message,
            Exception innerException): base(message, innerException)
        {
        }
    }

    public class KeyboardEventArgs : EventArgs {
        public KeyboardKey Key;

        public KeyboardEventArgs()
        {
        }
    }

    public class KeyboardKey {
        public string Name { get; set; }

        private Keys? key;
        public Keys? Key { 
            get { return key; } 
            private set { 
                key = value; 
                IsChar = false;
            }
        }

        private char? keyChar;
        public char? KeyChar { 
            get { return keyChar; }
            private set { 
                keyChar = value;
                IsChar = true;
            }
        }

        public bool IsChar { get; private set; }
        public bool Invalid { get; set; }
        public bool Repeat { get; set; }
        public bool Active { get; set; }

        public KeyboardKey(string name, Keys? key = null, char? keyChar = null, bool repeat = false) 
        {
            Name = name;

            if(key != null)
            {
                Key = key;
            } else if(keyChar != null)
            {
                KeyChar = keyChar;
            } else
            {
                throw new InvalidKeyException("Give either a key or a keyChar.");
            }
            Invalid = false;
            Active = false;
            Repeat = repeat;
        }

        public new string ToString() 
        {
            string text = Invalid ? "!! " : "";
            if (IsChar) {
                return text + ((char)KeyChar).ToString();
            } else {
                return text + ((Keys)Key).ToShortcutString();
            }
        }
    }

    public class KeyboardManager
    {
        public readonly Dictionary<string, KeyboardKey> Keys;
        public KeyboardKey ActiveKey { get; set; }

        [EventPublication(KeyboardManagerEvents.OnKeyUp)]
        public event EventHandler<KeyboardEventArgs> KeyUp = delegate {};
        [EventPublication(KeyboardManagerEvents.OnKeyDown)]
        public event EventHandler<KeyboardEventArgs> KeyDown = delegate {};

        private readonly ILogger logger;
        private readonly Dictionary<Keys, KeyboardKey> keyLookupTable;
        private readonly Dictionary<char, KeyboardKey> charLookupTable;

        public KeyboardManager(ILoggerFactory logFactory)
        {
            logger = logFactory.GetCurrentClassLogger();

            Keys = new Dictionary<string, KeyboardKey> ();
            keyLookupTable = new Dictionary<Keys, KeyboardKey> ();
            charLookupTable = new Dictionary<char, KeyboardKey>();
        }

        public bool Create(string name, Keys? key = null, char? keyChar = null, bool repeat = false) 
        {
            if (Keys.ContainsKey(name)) {
                throw new Exception(string.Format("The key \"{0}\" does exist.", name));
            }

            KeyboardKey kkey;
            if(key == null && keyChar == null)
            {
                kkey = new KeyboardKey(name, Eto.Forms.Keys.None, null);
                kkey.Invalid = true;
                Keys.Add(name, kkey);
                return false;
            }

            kkey = new KeyboardKey (name, key, keyChar, repeat);
            Keys.Add (name, kkey);
            if(kkey.IsChar)
            {
                try
                {
                    charLookupTable.Add((char)keyChar, kkey);
                } catch(System.ArgumentException)
                {
                    kkey.Invalid = true;
                    logger.Error("Tried to add key \"{0}\", keyChar: \"{1}\" but it already exists.", name, keyChar);
                    return false;
                }
            } else
            {
                try
                {
                    keyLookupTable.Add((Keys)key, kkey);
                } catch(System.ArgumentException)
                {
                    kkey.Invalid = true;
                    logger.Error("Tried to add key \"{0}\", key: \"{1}\" but it already exists.", name, ((Keys)key).ToShortcutString());
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Updates the specified Key.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="key">Key.</param>
        public void Update(string name, Keys? key = null, char? keyChar = null, bool? repeat = null)
        {
            if (!Keys.ContainsKey(name)) {
                throw new Exception(string.Format("The key \"{0}\" does not exist.", name));
            }

            var kkey = Keys [name];
            if(repeat == null)
            {
                repeat = kkey.Repeat;
            }

            Keys.Remove (name);
            if(kkey.IsChar)
            {
                charLookupTable.Remove((char)kkey.KeyChar);
            } else
            {
                keyLookupTable.Remove((Keys)kkey.Key);
            }

            Create(name, key, keyChar, (bool)repeat);
        }

        public void Update(string name, KeyboardKey kkey) {
            Update(name, kkey.Key, kkey.KeyChar, kkey.Repeat);
        }
                   
        public void HandleKeyUp(object sender, KeyEventArgs e)
        {
            if(ActiveKey != null)
            {
                KeyUp(sender, new KeyboardEventArgs { Key = ActiveKey });
                ActiveKey = null;
            }
            e.Handled = true;
        }
            
        public void HandleKeyDown(object sender, KeyEventArgs e)
        {
            KeyboardKey myActiveKey = null;
            var currentKeyOnly = e.KeyData & Eto.Forms.Keys.KeyMask;
            if (currentKeyOnly != Eto.Forms.Keys.None) {
                // Is known key.
                if (keyLookupTable.ContainsKey(e.KeyData)) {
                    myActiveKey = keyLookupTable[e.KeyData];
                }
            } else if (e.IsChar) {
                // Is Char
                if(charLookupTable.ContainsKey(e.KeyChar))
                {
                    myActiveKey = charLookupTable[e.KeyChar];
                }
            } else {
                // Is modifier only
                if (keyLookupTable.ContainsKey(e.KeyData)) {
                    myActiveKey = keyLookupTable[e.KeyData];
                }
            }

            if(myActiveKey == null)
            {
                return;
            }

            if(myActiveKey == ActiveKey)
            {
                if(ActiveKey.Repeat)
                {
                    KeyDown(sender, new KeyboardEventArgs { Key = ActiveKey });
                }
            } else
            {
                ActiveKey = myActiveKey;
                KeyDown(sender, new KeyboardEventArgs { Key = ActiveKey });
            }

            e.Handled = true;
        }

    }
}

