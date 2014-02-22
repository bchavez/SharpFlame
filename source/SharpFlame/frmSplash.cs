#region

using System;
using SharpFlame.Core;

#endregion

namespace SharpFlame
{
    public partial class frmSplash
    {
        public frmSplash()
        {
            InitializeComponent();

            Text = Constants.ProgramName + " " + Constants.ProgramVersionNumber + " Loading";
            lblVersion.Text = Constants.ProgramVersionNumber;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
        }
    }
}