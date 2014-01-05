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
    }
}