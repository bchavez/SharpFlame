#region

using System.Windows.Forms;
using NLog;
using SharpFlame.Collections;
using SharpFlame.Core.Domain;
using SharpFlame.Mapping.Objects;

#endregion

namespace SharpFlame
{
    public abstract class clsResultItemInterface
    {
        public abstract string GetText { get; }
        public abstract void DoubleClicked();
    }

    public class clsResult : clsResultItemInterface
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly SimpleList<clsResultItemInterface> Items = new SimpleList<clsResultItemInterface>();
        private bool Bad;
        public string Text;

        public clsResult(string Text, bool log = true)
        {
            if ( log )
            {
                logger.Debug(Text);
            }
            Items.MaintainOrder = true;

            this.Text = Text;
        }

        public override string GetText
        {
            get { return Text; }
        }

        public bool HasWarnings
        {
            get { return Items.Count > 0; }
        }

        public bool HasProblems
        {
            get { return Bad; }
        }

        public void AddBypass(clsResult ResultToAdd)
        {
            if ( ResultToAdd.HasWarnings )
            {
                Items.Add(ResultToAdd);
            }
        }

        public void Add(clsResult ResultToAdd)
        {
            if ( ResultToAdd.HasProblems )
            {
                Bad = true;
            }
            if ( ResultToAdd.HasWarnings )
            {
                Items.Add(ResultToAdd);
            }
        }

        public void Take(clsResult ResultToMerge)
        {
            if ( ResultToMerge.HasProblems )
            {
                Bad = true;
            }
            Items.AddSimpleList(ResultToMerge.Items);
        }

        public void ProblemAdd(string Text, bool log = true)
        {
            if ( log )
            {
                logger.Error(Text);
            }
            var Problem = new clsProblem();
            Problem.Text = Text;
            ItemAdd(Problem);
        }

        public void WarningAdd(string Text, bool log = true)
        {
            if ( log )
            {
                logger.Warn(Text);
            }
            var Warning = new clsWarning();
            Warning.Text = Text;
            ItemAdd(Warning);
        }

        public void ItemAdd(clsResultItemInterface item)
        {
            if ( item is clsProblem )
            {
                Bad = true;
            }
            Items.Add(item);
        }

        public TreeNode MakeNodes(TreeNodeCollection owner)
        {
            var node = new TreeNode();
            node.Text = Text;
            owner.Add(node);
            var item = default(clsResultItemInterface);
            for ( var i = 0; i <= Items.Count - 1; i++ )
            {
                item = Items[i];
                var ChildNode = new TreeNode();
                ChildNode.Tag = item;
                if ( item is clsProblem )
                {
                    ChildNode.Text = item.GetText;
                    node.Nodes.Add(ChildNode);
                    ChildNode.StateImageKey = "problem";
                }
                else if ( item is clsWarning )
                {
                    ChildNode.Text = item.GetText;
                    node.Nodes.Add(ChildNode);
                    ChildNode.StateImageKey = "warning";
                }
                else if ( item is clsResult )
                {
                    ChildNode = ((clsResult)item).MakeNodes(node.Nodes);
                }
            }
            return node;
        }

        public override void DoubleClicked()
        {
        }

        public class clsProblem : clsResultItemInterface
        {
            public string Text;

            public override string GetText
            {
                get { return Text; }
            }

            public override void DoubleClicked()
            {
            }
        }

        public class clsWarning : clsResultItemInterface
        {
            public string Text;

            public override string GetText
            {
                get { return Text; }
            }

            public override void DoubleClicked()
            {
            }
        }
    }

    public class clsResultWarningGoto<GotoType> : clsResult.clsWarning where GotoType : clsResultItemGotoInterface
    {
        public GotoType MapGoto;

        public override void DoubleClicked()
        {
            base.DoubleClicked();

            MapGoto.Perform();
        }
    }

    public class clsResultProblemGoto<GotoType> : clsResult.clsProblem where GotoType : clsResultItemGotoInterface
    {
        public GotoType MapGoto;

        public override void DoubleClicked()
        {
            base.DoubleClicked();

            MapGoto.Perform();
        }
    }

    public abstract class clsResultItemGotoInterface
    {
        public abstract void Perform();
    }

    public class clsResultItemTileGoto : clsResultItemGotoInterface
    {
        public XYInt TileNum;
        public clsViewInfo View;

        public override void Perform()
        {
            View.LookAtTile(TileNum);
        }
    }

    public class clsResultItemPosGoto : clsResultItemGotoInterface
    {
        public XYInt Horizontal;
        public clsViewInfo View;

        public override void Perform()
        {
            View.LookAtPos(Horizontal);
        }
    }

    public sealed class modResults
    {
        public static clsResultProblemGoto<clsResultItemPosGoto> CreateResultProblemGotoForObject(clsUnit unit)
        {
            var resultGoto = new clsResultItemPosGoto();
            resultGoto.View = unit.MapLink.Source.ViewInfo;
            resultGoto.Horizontal = unit.Pos.Horizontal;
            var resultProblem = new clsResultProblemGoto<clsResultItemPosGoto>();
            resultProblem.MapGoto = resultGoto;
            return resultProblem;
        }
    }
}