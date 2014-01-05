using System;
using System.Windows.Forms;
using SharpFlame.FileIO;
using SharpFlame.Mapping;
using SharpFlame.Mapping.Objects;

namespace SharpFlame.Controls
{
    public partial class PlayerNumControl
    {
        public ToolStripButton[] tsbNumber = new ToolStripButton[11];

        private clsUnitGroupContainer _Target;

        public const int ScavButtonNum = 10;

        public PlayerNumControl()
        {
            InitializeComponent();

            int A = 0;
            int B = 0;
            int ButtonsPerRow = 5;

            for ( A = 0; A <= ButtonsPerRow - 1; A++ )
            {
                tsbNumber[A] = new ToolStripButton();
                tsbNumber[A].DisplayStyle = ToolStripItemDisplayStyle.Text;
                tsbNumber[A].Text = A.ToStringInvariant();
                tsbNumber[A].AutoToolTip = false;
                tsbNumber[A].Click += tsbNumber_Clicked;
                tsPlayerNum1.Items.Add(tsbNumber[A]);

                B = A + ButtonsPerRow;
                tsbNumber[B] = new ToolStripButton();
                tsbNumber[B].DisplayStyle = ToolStripItemDisplayStyle.Text;
                tsbNumber[B].Text = B.ToStringInvariant();
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
            clsUnitGroup UnitGroup = (clsUnitGroup)tsb.Tag;

            _Target.Item = UnitGroup;
        }

        public clsUnitGroupContainer Target
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
            clsUnitGroup UnitGroup = default(clsUnitGroup);

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
                    tsbNumber[A].Checked = ((clsUnitGroup)(tsbNumber[A].Tag)) == UnitGroup;
                }
            }
        }

        public void SetMap(clsMap NewMap)
        {
            int A = 0;

            if ( NewMap == null )
            {
                for ( A = 0; A <= Constants.PlayerCountMax - 1; A++ )
                {
                    tsbNumber[A].Tag = null;
                }
                tsbNumber[ScavButtonNum].Tag = null;
            }
            else
            {
                for ( A = 0; A <= Constants.PlayerCountMax - 1; A++ )
                {
                    tsbNumber[A].Tag = NewMap.UnitGroups[A];
                }
                tsbNumber[ScavButtonNum].Tag = NewMap.ScavengerUnitGroup;
            }
            SelectedChanged();
        }
    }
}