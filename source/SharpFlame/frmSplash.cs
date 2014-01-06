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

        private void pictureBox1_Click( object sender, System.EventArgs e )
        {

        }
    }
}