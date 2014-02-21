#region

using System.Windows.Forms;
using SharpFlame.Util;

#endregion

namespace SharpFlame.AppSettings
{
    public class KeyboardControl
    {
        public Keys[] Keys;
        public Keys[] UnlessKeys;
        private bool active;

        public KeyboardControl(Keys[] keys)
        {
            this.Keys = keys;
            UnlessKeys = new Keys[0];
        }

        public KeyboardControl(Keys[] keys, Keys[] unlessKeys)
        {
            this.Keys = keys;
            this.UnlessKeys = unlessKeys;
        }

        public bool Active
        {
            get { return active; }
        }

        private bool IsPressed(clsKeysActive keysDown)
        {
            foreach ( var keys in Keys )
            {
                if ( !keysDown.Keys[(int)keys] )
                {
                    return false;
                }
            }
            foreach ( var keys in UnlessKeys )
            {
                if ( keysDown.Keys[(int)keys] )
                {
                    return false;
                }
            }

            return true;
        }

        public void KeysChanged(clsKeysActive keysDown)
        {
            active = IsPressed(keysDown);
        }
    }
}