namespace SharpFlame
{
    public partial class frmSplash
    {
        public frmSplash()
        {
            InitializeComponent();

            Text = modProgram.ProgramName + " " + modProgram.ProgramVersionNumber + " Loading";
            lblVersion.Text = modProgram.ProgramVersionNumber;
        }
    }
}