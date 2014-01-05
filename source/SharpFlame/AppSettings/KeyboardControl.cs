using System.Windows.Forms;

namespace SharpFlame.AppSettings
{
    public class KeyboardControl
    {
        public Keys[] Keys;
        public Keys[] UnlessKeys;

        private bool IsPressed(App.clsKeysActive KeysDown)
        {
            foreach ( Keys keys in Keys )
            {
                if ( !KeysDown.Keys[(int)keys] )
                {
                    return false;
                }
            }
            foreach ( Keys keys in UnlessKeys )
            {
                if ( KeysDown.Keys[(int)keys] )
                {
                    return false;
                }
            }

            return true;
        }

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

        private bool _Active;

        public bool Active
        {
            get { return _Active; }
        }

        public void KeysChanged(App.clsKeysActive KeysDown)
        {
            _Active = IsPressed(KeysDown);
        }
    }
}