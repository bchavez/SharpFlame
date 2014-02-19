using System;
using System.Windows.Forms;
using SharpFlame.Collections;
using SharpFlame.FileIO;
using SharpFlame.Util;

namespace SharpFlame
{
    public partial class frmKeyboardControl
    {
        public SimpleList<Keys> Results = new SimpleList<Keys>();

        public frmKeyboardControl()
        {
            InitializeComponent();

            Icon = App.ProgramIcon;

            UpdateLabel();
        }

        private void UpdateLabel()
        {
            lblKeys.Text = "";
            for ( int i = 0; i <= Results.Count - 1; i++ )
            {
                Keys key = Keys.A;
                string text = Enum.GetName(typeof(Keys), key);
                if ( text == null )
                {
                    lblKeys.Text += Convert.ToInt32(Results[i]).ToStringInvariant();
                }
                else
                {
                    lblKeys.Text += text;
                }
                lblKeys.Text += " ";
            }
        }

        public void btnSave_Click(Object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        public void btnCancel_Click(Object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        public void frmKeyboardControl_KeyDown(object sender, KeyEventArgs e)
        {
            if ( Results.Count > 8 )
            {
                return;
            }
            foreach ( var key in Results )
            {
                if ( key == e.KeyCode )
                {
                    return;
                }
            }
            Results.Add(e.KeyCode);
            UpdateLabel();
        }
    }
}