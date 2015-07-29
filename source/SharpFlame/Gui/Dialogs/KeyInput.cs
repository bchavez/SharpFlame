using System;
using Eto.Drawing;
using Eto.Forms;
using SharpFlame.Settings;

namespace SharpFlame.Gui.Dialogs
{
    public class KeyInput : Dialog
    {
        private Label lblKey;

        private CheckBox chkCtrl;
        private CheckBox chkAlt;
        private CheckBox chkShift;

        public Keys KeyData { get; set; }

        public KeyInput()
        {
            XomlReader.Load(this);
        }

        void chkModifer_Changed(object sender, EventArgs e)
        {
            var chk = sender as CheckBox;
            var modifier = (Keys)chk.Tag;
            if( true == chk.Checked )
            {
                this.KeyData |= modifier;
            }
            else
            {
                this.KeyData &= ~modifier;
            }

            this.lblKey.Text = this.KeyData.ToShortcutString();
        }

        private void cmdClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Dialog_KeyUp(object sender, KeyEventArgs e)
        {
            if( e.KeyData == Keys.None ) //BUG: in eto, we get by pressing CTRL/ALT
                return;

            this.KeyData = e.KeyData;

            this.chkCtrl.Checked = e.Control;
            this.chkAlt.Checked = e.Alt;
            this.chkShift.Checked = e.Shift;

            lblKey.Text = this.KeyData.ToShortcutString();
        }
    }
}

