using System;
using System.Windows.Forms;

namespace SharpFlame
{
    public partial class ctrlPathSet
    {
        public int SelectedNum
        {
            get { return lstPaths.SelectedIndex; }
            set { lstPaths.SelectedIndex = value; }
        }

        public string SelectedPath
        {
            get
            {
                if ( lstPaths.SelectedIndex < 0 )
                {
                    return null;
                }
                else
                {
                    return (lstPaths.Items[lstPaths.SelectedIndex]).ToString();
                }
            }
        }

        public string[] GetPaths
        {
            get
            {
                string[] Paths = new string[lstPaths.Items.Count];
                int A = 0;
                for ( A = 0; A <= lstPaths.Items.Count - 1; A++ )
                {
                    Paths[A] = (lstPaths.Items[A]).ToString();
                }
                return Paths;
            }
        }

        public ctrlPathSet(string Title)
        {
            InitializeComponent();

            gbxTitle.Text = Title;
        }

        public void SetPaths(modLists.SimpleList<string> NewPaths)
        {
            int A = 0;

            lstPaths.Items.Clear();
            for ( A = 0; A <= NewPaths.Count - 1; A++ )
            {
                lstPaths.Items.Add(NewPaths[A]);
            }
        }

        public void btnAdd_Click(Object sender, EventArgs e)
        {
            FolderBrowserDialog DirSelect = new FolderBrowserDialog();

            if ( lstPaths.Items.Count > 0 )
            {
                DirSelect.SelectedPath = Convert.ToString(lstPaths.Items[lstPaths.Items.Count - 1]);
            }

            if ( DirSelect.ShowDialog() != DialogResult.OK )
            {
                return;
            }

            lstPaths.Items.Add(DirSelect.SelectedPath);
            lstPaths.SelectedIndex = lstPaths.Items.Count - 1;
        }

        public void btnRemove_Click(Object sender, EventArgs e)
        {
            if ( lstPaths.SelectedIndex < 0 )
            {
                return;
            }

            lstPaths.Items.RemoveAt(lstPaths.SelectedIndex);
        }
    }
}