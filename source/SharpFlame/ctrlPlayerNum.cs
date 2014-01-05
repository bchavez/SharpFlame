using System;
using System.Windows.Forms;
using SharpFlame.FileIO;

namespace SharpFlame
{
    public partial class ctrlPlayerNum
    {
        public ToolStripButton[] tsbNumber = new ToolStripButton[11];

        private clsMap.clsUnitGroupContainer _Target;

        public const int ScavButtonNum = 10;

        public ctrlPlayerNum()
        {
            InitializeComponent();

            int A = 0;
            int B = 0;
            int ButtonsPerRow = 5;

            for ( A = 0; A <= ButtonsPerRow - 1; A++ )
            {
                tsbNumber[A] = new ToolStripButton();
                tsbNumber[A].DisplayStyle = ToolStripItemDisplayStyle.Text;
                tsbNumber[A].Text = IOUtil.InvariantToString_int(A);
                tsbNumber[A].AutoToolTip = false;
                tsbNumber[A].Click += tsbNumber_Clicked;
                tsPlayerNum1.Items.Add(tsbNumber[A]);

                B = A + ButtonsPerRow;
                tsbNumber[B] = new ToolStripButton();
                tsbNumber[B].DisplayStyle = ToolStripItemDisplayStyle.Text;
                tsbNumber[B].Text = IOUtil.InvariantToString_int(B);
                tsbNumber[B].AutoToolTip = false;
                tsbNumber[B].Click += tsbNumber_Clicked;
                tsPlayerNum2.Items.Add(tsbNumber[B]);
            }

            A = 10;

            tsbNumber[A] = new ToolStripButton();
            tsbNumber[A].DisplayStyle = ToolStripItemDisplayStyle.Text;
            tsbNumber[A].Text = "S";
            tsbNumber[A].AutoToolTip = false;
            tsbNumber[A].Click += tsbNumber_Clicked;
            tsPlayerNum2.Items.Add(tsbNumber[A]);

            Width = 24 * 6;
            Height = 25 * 2;
        }

        private void tsbNumber_Clicked(object sender, EventArgs e)
        {
            if ( _Target == null )
            {
                return;
            }

            ToolStripButton tsb = (ToolStripButton)sender;
            clsMap.clsUnitGroup UnitGroup = (clsMap.clsUnitGroup)tsb.Tag;

            _Target.Item = UnitGroup;
        }

        public clsMap.clsUnitGroupContainer Target
        {
            get { return _Target; }
            set
            {
                if ( value == _Target )
                {
                    return;
                }
                if ( _Target != null )
                {
                    _Target.Changed -= SelectedChanged;
                }
                _Target = value;
                if ( _Target != null )
                {
                    _Target.Changed += SelectedChanged;
                }
                SelectedChanged();
            }
        }

        private void SelectedChanged()
        {
            int A = 0;
            clsMap.clsUnitGroup UnitGroup = default(clsMap.clsUnitGroup);

            if ( _Target == null )
            {
                UnitGroup = null;
            }
            else
            {
                UnitGroup = _Target.Item;
            }

            if ( UnitGroup == null )
            {
                for ( A = 0; A <= 10; A++ )
                {
                    tsbNumber[A].Checked = false;
                }
            }
            else
            {
                for ( A = 0; A <= 10; A++ )
                {
                    tsbNumber[A].Checked = ((clsMap.clsUnitGroup)(tsbNumber[A].Tag)) == UnitGroup;
                }
            }
        }

        public void SetMap(clsMap NewMap)
        {
            int A = 0;

            if ( NewMap == null )
            {
                for ( A = 0; A <= modProgram.PlayerCountMax - 1; A++ )
                {
                    tsbNumber[A].Tag = null;
                }
                tsbNumber[ScavButtonNum].Tag = null;
            }
            else
            {
                for ( A = 0; A <= modProgram.PlayerCountMax - 1; A++ )
                {
                    tsbNumber[A].Tag = NewMap.UnitGroups[A];
                }
                tsbNumber[ScavButtonNum].Tag = NewMap.ScavengerUnitGroup;
            }
            SelectedChanged();
        }
    }
}