using System.Windows.Forms;

namespace SharpFlame
{
    public partial class frmClose
    {
        public frmClose(string WindowTitle)
        {
            InitializeComponent();

            Text = WindowTitle;

            DialogResult = DialogResult.Cancel;
        }
    }
}