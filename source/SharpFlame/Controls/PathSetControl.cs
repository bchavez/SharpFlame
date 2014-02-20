#region

using System;
using System.Collections.Generic;
using System.Windows.Forms;

#endregion

namespace SharpFlame.Controls
{
    public partial class PathSetControl
    {
        public PathSetControl(string Title)
        {
            InitializeComponent();

            gbxTitle.Text = Title;
        }

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
                return (lstPaths.Items[lstPaths.SelectedIndex]).ToString();
            }
        }

        public string[] GetPaths
        {
            get
            {
                var Paths = new string[lstPaths.Items.Count];
                var A = 0;
                for ( A = 0; A <= lstPaths.Items.Count - 1; A++ )
                {
                    Paths[A] = (lstPaths.Items[A]).ToString();
                }
                return Paths;
            }
        }

        public void SetPaths(List<string> NewPaths)
        {
            var A = 0;

            lstPaths.Items.Clear();
            for ( A = 0; A <= NewPaths.Count - 1; A++ )
            {
                lstPaths.Items.Add(NewPaths[A]);
            }
        }

        public void btnAdd_Click(Object sender, EventArgs e)
        {
            var dialog = new FolderBrowserDialog
                {
                    SelectedPath = Application.StartupPath
                };

            if ( lstPaths.Items.Count > 0 )
            {
                dialog.SelectedPath = Convert.ToString(lstPaths.Items[lstPaths.Items.Count - 1]);
            }

            if ( dialog.ShowDialog() != DialogResult.OK )
            {
                return;
            }

            lstPaths.Items.Add(dialog.SelectedPath);
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