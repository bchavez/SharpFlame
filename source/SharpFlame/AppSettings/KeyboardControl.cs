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
        private bool _Active;

        public KeyboardControl(Keys[] Keys)
        {
            this.Keys = Keys;
            UnlessKeys = new Keys[0];
        }

        public KeyboardControl(Keys[] Keys, Keys[] UnlessKeys)
        {
            this.Keys = Keys;
            this.UnlessKeys = UnlessKeys;
        }

        public bool Active
        {
            get { return _Active; }
        }

        private bool IsPressed(clsKeysActive KeysDown)
        {
            foreach ( var keys in Keys )
            {
                if ( !KeysDown.Keys[(int)keys] )
                {
                    return false;
                }
            }
            foreach ( var keys in UnlessKeys )
            {
                if ( KeysDown.Keys[(int)keys] )
                {
                    return false;
                }
            }

            return true;
        }

        public void KeysChanged(clsKeysActive KeysDown)
        {
            _Active = IsPressed(KeysDown);
        }
    }
}