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
using Eto.Forms;
using NLog;

namespace SharpFlame.Core
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

    public class KeyboardKey {
        public Keys Key { get; set; }
        public char KeyChar { get; set; }
        public bool Active { get; set; }
        public bool Invalid { get; set; }

        public KeyboardKey(Keys? key = null, char? keyChar = null) 
        {
            if(key != null)
            {
                Key = (Keys)key;
            } else if(keyChar != null)
            {
                KeyChar = (char)keyChar;
            } else
            {
                // This should never happen as this is checked in KeyboardManager.Create.
                throw new InvalidKeyException("Give either a key or a keyChar.");
            }
            Active = false;
            Invalid = false;
        }
    }

    public class KeyboardManager
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public readonly Dictionary<string, KeyboardKey> Keys;
        readonly Dictionary<Keys, KeyboardKey> keyLookupTable;
        readonly Dictionary<char, KeyboardKey> charLookupTable;

        public KeyboardManager()
        {
            Keys = new Dictionary<string, KeyboardKey> ();
            keyLookupTable = new Dictionary<Keys, KeyboardKey> ();
            charLookupTable = new Dictionary<char, KeyboardKey>();
        }

        public bool Create(string name, Keys? key = null, char? keyChar = null) {
            KeyboardKey kkey;
            if(key == null && keyChar == null)
            {
                kkey = new KeyboardKey(Eto.Forms.Keys.None, null);
                kkey.Invalid = true;
                Keys.Add(name, kkey);
                return false;
            }

            kkey = new KeyboardKey (key, keyChar);
            Keys.Add (name, kkey);
            if(key != null)
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
            } else
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
            }

            return true;
        }

        /// <summary>
        /// Updates the specified Key.
        /// It set the key back to the old value if the key already exists and returns false.
        /// On Success it returns true.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="key">Key.</param>
        public bool Update(string name, Keys? key = null, char? keyChar = null)
        {
            if (!Keys.ContainsKey(name)) {
                throw new Exception(string.Format("The key \"{0}\" does not exist.", name));
            }

            var kkey = Keys [name];
            Keys.Remove (name);
            keyLookupTable.Remove (kkey.Key);

            if (!Create (name, key))
            {
                Keys.Remove (name);
                Create (name, kkey.Key);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Determines whether the key "name" is active
        /// </summary>
        /// <returns><c>true</c> if key is active name; otherwise, <c>false</c>.</returns>
        /// <param name="name">Name.</param>
        public bool IsKeyActive(string name) {
            if (!Keys.ContainsKey(name)) {
                throw new Exception(string.Format("The key \"{0}\" does not exist.", name));
            }

            return Keys [name].Active;
        }
    }
}

