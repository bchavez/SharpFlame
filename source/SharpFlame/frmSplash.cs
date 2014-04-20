

using System;
using SharpFlame.Core;


namespace SharpFlame
{
    public partial class frmSplash
    {
        public frmSplash()
        {
            InitializeComponent();

            Text = Constants.ProgramName + " " + Constants.ProgramVersion() + " Loading";
            lblVersion.Text = Constants.ProgramVersion();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
        }
    }
}