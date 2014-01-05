namespace SharpFlame
{
    public partial class frmClose
    {
        public frmClose(string WindowTitle)
        {
            InitializeComponent();

            Text = WindowTitle;

            DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }
    }
}