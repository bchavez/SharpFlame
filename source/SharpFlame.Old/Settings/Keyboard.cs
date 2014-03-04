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
using otkKey = OpenTK.Input.Key;

namespace SharpFlame.Core
{
    public class KeyboardKey {
        public Keys Key { get; set; }
        public bool Active { get; set; }
        public bool Invalid { get; set; }

        public KeyboardKey(Keys key) 
        {
            Key = key;
            Active = false;
            Invalid = false;
        }
    }

    public class Keyboard
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public readonly Dictionary<string, KeyboardKey> Keys;
        readonly Dictionary<Keys, KeyboardKey> keyLookupTable;

        public Keyboard ()
        {
            Keys = new Dictionary<string, KeyboardKey> ();
            keyLookupTable = new Dictionary<Keys, KeyboardKey> ();
        }

        public bool Create(string name, Keys key) {
            var kkey = new KeyboardKey (key);
            Keys.Add (name, kkey);
            try {
                keyLookupTable.Add (key, kkey);
            } catch (System.ArgumentException) {
                kkey.Invalid = true;
                logger.Error("Tried to add key \"{0}\", key: \"{1}\" but it already exists.", name, key.ToShortcutString());
                return false;
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
        public bool Update(string name, Keys key)
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

