#region

using System.Windows.Forms;

#endregion

namespace SharpFlame.Old
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