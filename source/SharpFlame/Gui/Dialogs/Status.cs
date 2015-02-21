using Eto.Drawing;
using Eto.Forms;
using SharpFlame.Core;

namespace SharpFlame.Gui.Dialogs
{
    public class Status : Form
    {
        readonly TreeView treeView;

        public Status (Result result)
        {
            Title = result.Text;
            Resizable = true;
            Size = new Size (500, 300);
            Topmost = true;
            ShowInTaskbar = false;
            Location = new Point (200, 100);

            var layout = new DynamicLayout { BackgroundColor = Eto.Drawing.Colors.White, Padding = Padding.Empty } ;
            treeView = new TreeView ();      
            layout.Add (treeView, true, true);
            var item = new TreeItem { Text = result.Text, Expanded = true };
            item.Children.Add (iterResult (result));
            treeView.DataStore = item;

            Content = layout;
        }

        TreeItem iterResult(Result result) {
            var item = new TreeItem
            {
                Text = result.Text,
                Expanded = true,
                // Image = Resources.IconProblem()
            };

            foreach (var lItem in ((Result)result).Items)
            {
                if (lItem is Result.Problem)
                {
                    item.Children.Add (new TreeItem { 
                        Text = ((Result.Problem)lItem).GetText,
                        Expanded = true,
                        Image = Resources.Problem
                    });
                } else if (lItem is Result.Warning)
                {
                    item.Children.Add (new TreeItem { 
                        Text = ((Result.Warning)lItem).GetText,
                        Expanded = true,
                        Image = Resources.Warning
                    });
                } else if (lItem is Result)
                {
                    item.Children.Add (iterResult ((Result)lItem));
                }
            }


            return item;
        }
    }
}

