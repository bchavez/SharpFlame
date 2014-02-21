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

        private readonly SimpleList<clsResultItemInterface> items = new SimpleList<clsResultItemInterface>();
        private bool bad;
        public string Text;

        public clsResult(string text, bool log = true)
        {
            if ( log )
            {
                logger.Debug(text);
            }
            items.MaintainOrder = true;

            this.Text = text;
        }

        public override string GetText
        {
            get { return Text; }
        }

        public bool HasWarnings
        {
            get { return items.Count > 0; }
        }

        public bool HasProblems
        {
            get { return bad; }
        }

        public void AddBypass(clsResult resultToAdd)
        {
            if ( resultToAdd.HasWarnings )
            {
                items.Add(resultToAdd);
            }
        }

        public void Add(clsResult resultToAdd)
        {
            if ( resultToAdd.HasProblems )
            {
                bad = true;
            }
            if ( resultToAdd.HasWarnings )
            {
                items.Add(resultToAdd);
            }
        }

        public void Take(clsResult resultToMerge)
        {
            if ( resultToMerge.HasProblems )
            {
                bad = true;
            }
            items.AddRange(resultToMerge.items);
        }

        public void ProblemAdd(string text, bool log = true)
        {
            if ( log )
            {
                logger.Error(text);
            }
            var problem = new clsProblem();
            problem.Text = text;
            ItemAdd(problem);
        }

        public void WarningAdd(string text, bool log = true)
        {
            if ( log )
            {
                logger.Warn(text);
            }
            var warning = new clsWarning();
            warning.Text = text;
            ItemAdd(warning);
        }

        public void ItemAdd(clsResultItemInterface item)
        {
            if ( item is clsProblem )
            {
                bad = true;
            }
            items.Add(item);
        }

        public TreeNode MakeNodes(TreeNodeCollection owner)
        {
            var node = new TreeNode();
            node.Text = Text;
            owner.Add(node);
            for ( var i = 0; i <= items.Count - 1; i++ )
            {
                var item = items[i];
                var childNode = new TreeNode();
                childNode.Tag = item;
                if ( item is clsProblem )
                {
                    childNode.Text = item.GetText;
                    node.Nodes.Add(childNode);
                    childNode.StateImageKey = "problem";
                }
                else if ( item is clsWarning )
                {
                    childNode.Text = item.GetText;
                    node.Nodes.Add(childNode);
                    childNode.StateImageKey = "warning";
                }
                else if ( item is clsResult )
                {
                    ((clsResult)item).MakeNodes(node.Nodes);
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