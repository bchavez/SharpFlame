#region

using System;
using SharpFlame.Core;

#endregion

namespace SharpFlame.Old
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