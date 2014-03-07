#region

using System;
using System.Collections.Generic;
using System.Windows.Forms;

#endregion

namespace SharpFlame.Controls
{
    public partial class PathSetControl
    {
        public PathSetControl(string title)
        {
            InitializeComponent();

            gbxTitle.Text = title;
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
                var paths = new string[lstPaths.Items.Count];
                for (var a = 0; a <= lstPaths.Items.Count - 1; a++ )
                {
                    paths[a] = (lstPaths.Items[a]).ToString();
                }
                return paths;
            }
        }

        public void SetPaths(List<string> newPaths)
        {
            lstPaths.Items.Clear();
            for (var a = 0; a <= newPaths.Count - 1; a++ )
            {
                lstPaths.Items.Add(newPaths[a]);
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