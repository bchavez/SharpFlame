namespace FlaME
{
    using System;
    using System.Windows.Forms;

    public class clsKeyboardControl
    {
        private bool _Active;
        public System.Windows.Forms.Keys[] Keys;
        public System.Windows.Forms.Keys[] UnlessKeys;

        public clsKeyboardControl(System.Windows.Forms.Keys[] Keys)
        {
            this.Keys = Keys;
            this.UnlessKeys = new System.Windows.Forms.Keys[0];
        }

        public clsKeyboardControl(System.Windows.Forms.Keys[] Keys, System.Windows.Forms.Keys[] UnlessKeys)
        {
            this.Keys = Keys;
            this.UnlessKeys = UnlessKeys;
        }

        private bool IsPressed(modProgram.clsKeysActive KeysDown)
        {
            foreach (System.Windows.Forms.Keys keys in this.Keys)
            {
                if (!KeysDown.Keys[(int) keys])
                {
                    return false;
                }
            }
            foreach (System.Windows.Forms.Keys keys2 in this.UnlessKeys)
            {
                if (KeysDown.Keys[(int) keys2])
                {
                    return false;
                }
            }
            return true;
        }

        public void KeysChanged(modProgram.clsKeysActive KeysDown)
        {
            this._Active = this.IsPressed(KeysDown);
        }

        public bool Active
        {
            get
            {
                return this._Active;
            }
        }
    }
}

