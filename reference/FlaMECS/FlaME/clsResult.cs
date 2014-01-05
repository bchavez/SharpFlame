namespace FlaME
{
    using System;
    using System.Windows.Forms;

    public class clsResult : clsResultItemInterface
    {
        private bool Bad = false;
        private modLists.SimpleList<clsResultItemInterface> Items = new modLists.SimpleList<clsResultItemInterface>();
        public string Text;

        public clsResult(string Text)
        {
            this.Items.MaintainOrder = true;
            this.Text = Text;
        }

        public void Add(clsResult ResultToAdd)
        {
            if (ResultToAdd.HasProblems)
            {
                this.Bad = true;
            }
            if (ResultToAdd.HasWarnings)
            {
                this.Items.Add(ResultToAdd);
            }
        }

        public void AddBypass(clsResult ResultToAdd)
        {
            if (ResultToAdd.HasWarnings)
            {
                this.Items.Add(ResultToAdd);
            }
        }

        public override void DoubleClicked()
        {
        }

        public void ItemAdd(clsResultItemInterface item)
        {
            if (item is clsProblem)
            {
                this.Bad = true;
            }
            this.Items.Add(item);
        }

        public TreeNode MakeNodes(TreeNodeCollection owner)
        {
            TreeNode node = new TreeNode {
                Text = this.Text
            };
            owner.Add(node);
            int num2 = this.Items.Count - 1;
            for (int i = 0; i <= num2; i++)
            {
                clsResultItemInterface interface2 = this.Items[i];
                TreeNode node3 = new TreeNode {
                    Tag = interface2
                };
                if (interface2 is clsProblem)
                {
                    node3.Text = interface2.GetText;
                    node.Nodes.Add(node3);
                    node3.StateImageKey = "problem";
                }
                else if (interface2 is clsWarning)
                {
                    node3.Text = interface2.GetText;
                    node.Nodes.Add(node3);
                    node3.StateImageKey = "warning";
                }
                else if (interface2 is clsResult)
                {
                    node3 = ((clsResult) interface2).MakeNodes(node.Nodes);
                }
            }
            return node;
        }

        public void ProblemAdd(string Text)
        {
            clsProblem item = new clsProblem {
                Text = Text
            };
            this.ItemAdd(item);
        }

        public void Take(clsResult ResultToMerge)
        {
            if (ResultToMerge.HasProblems)
            {
                this.Bad = true;
            }
            this.Items.AddSimpleList(ResultToMerge.Items);
        }

        public void WarningAdd(string Text)
        {
            clsWarning item = new clsWarning {
                Text = Text
            };
            this.ItemAdd(item);
        }

        public override string GetText
        {
            get
            {
                return this.Text;
            }
        }

        public bool HasProblems
        {
            get
            {
                return this.Bad;
            }
        }

        public bool HasWarnings
        {
            get
            {
                return (this.Items.Count > 0);
            }
        }

        public class clsProblem : clsResultItemInterface
        {
            public string Text;

            public override void DoubleClicked()
            {
            }

            public override string GetText
            {
                get
                {
                    return this.Text;
                }
            }
        }

        public class clsWarning : clsResultItemInterface
        {
            public string Text;

            public override void DoubleClicked()
            {
            }

            public override string GetText
            {
                get
                {
                    return this.Text;
                }
            }
        }
    }
}

