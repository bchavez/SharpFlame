using System;
using System.Windows.Forms;
using SharpFlame.Collections;

namespace SharpFlame
{
    public partial class frmKeyboardControl
    {
        public SimpleList<clsContainer<Keys>> Results = new SimpleList<clsContainer<Keys>>();

        public frmKeyboardControl()
        {
            InitializeComponent();

            Icon = modProgram.ProgramIcon;

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
                    lblKeys.Text += modIO.InvariantToString_int(Convert.ToInt32(Results[i].Item));
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
            foreach ( clsContainer<Keys> key in Results )
            {
                if ( key.Item == e.KeyCode )
                {
                    return;
                }
            }
            Results.Add(new clsContainer<Keys>(e.KeyCode));
            UpdateLabel();
        }
    }
}