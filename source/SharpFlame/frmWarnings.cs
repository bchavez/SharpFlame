#region

using System.Windows.Forms;
using SharpFlame.Core;
using Sprache;
using Result = SharpFlame.Core.Result;

#endregion

namespace SharpFlame
{
    public sealed class modWarnings
    {
        public static ImageList WarningImages = new ImageList();
    }

    public partial class frmWarnings
    {
        public frmWarnings(Result result, string windowTitle)
        {
            InitializeComponent();

            Icon = App.ProgramIcon;

            Text = windowTitle;

            tvwWarnings.StateImageList = modWarnings.WarningImages;
            makeNodes(tvwWarnings.Nodes, result);
            tvwWarnings.ExpandAll();

            tvwWarnings.NodeMouseDoubleClick += NodeDoubleClicked;
        }

        private TreeNode makeNodes(TreeNodeCollection owner, Result result)
        {
            var node = new TreeNode();
            node.Text = Text;
            owner.Add(node);
            foreach (var item in result.Items) {
                var childNode = new TreeNode();
                childNode.Tag = item;
                if (item is Result.Problem)
                {
                    childNode.Text = item.GetText;
                    node.Nodes.Add(childNode);
                    childNode.StateImageKey = "problem";
                }
                else if (item is Result.Warning)
                {
                    childNode.Text = item.GetText;
                    node.Nodes.Add(childNode);
                    childNode.StateImageKey = "warning";
                }
                else if (item is Result)
                {
                    makeNodes(node.Nodes, (Result)item);
                }
            }
            return node;
        }

        private void NodeDoubleClicked(object sender, TreeNodeMouseClickEventArgs e)
        {
            if ( e.Button != MouseButtons.Left )
            {
                return;
            }
            var item = (IResultItem)e.Node.Tag;
            if ( item == null )
            {
                return;
            }
            item.DoubleClicked();
        }

        public void frmWarnings_FormClosed(object sender, FormClosedEventArgs e)
        {
            tvwWarnings.NodeMouseDoubleClick -= NodeDoubleClicked;
        }
    }
}