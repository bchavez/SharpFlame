#region

using System;
using System.Windows.Forms;
using SharpFlame.FileIO;
using SharpFlame.Mapping;
using SharpFlame.Mapping.Objects;

#endregion

namespace SharpFlame.Controls
{
    public partial class PlayerNumControl
    {
        public const int ScavButtonNum = 10;
        private clsUnitGroupContainer target;
        public ToolStripButton[] tsbNumber = new ToolStripButton[11];

        public PlayerNumControl()
        {
            InitializeComponent();

            var a = 0;
            const int ButtonsPerRow = 5;

            for ( a = 0; a <= ButtonsPerRow - 1; a++ )
            {
                tsbNumber[a] = new ToolStripButton();
                tsbNumber[a].DisplayStyle = ToolStripItemDisplayStyle.Text;
                tsbNumber[a].Text = a.ToStringInvariant();
                tsbNumber[a].AutoToolTip = false;
                tsbNumber[a].Click += tsbNumber_Clicked;
                tsPlayerNum1.Items.Add(tsbNumber[a]);

                var b = a + ButtonsPerRow;
                tsbNumber[b] = new ToolStripButton();
                tsbNumber[b].DisplayStyle = ToolStripItemDisplayStyle.Text;
                tsbNumber[b].Text = b.ToStringInvariant();
                tsbNumber[b].AutoToolTip = false;
                tsbNumber[b].Click += tsbNumber_Clicked;
                tsPlayerNum2.Items.Add(tsbNumber[b]);
            }

            a = 10;

            tsbNumber[a] = new ToolStripButton();
            tsbNumber[a].DisplayStyle = ToolStripItemDisplayStyle.Text;
            tsbNumber[a].Text = "S";
            tsbNumber[a].AutoToolTip = false;
            tsbNumber[a].Click += tsbNumber_Clicked;
            tsPlayerNum2.Items.Add(tsbNumber[a]);

            Width = 24 * 6;
            Height = 25 * 2;
        }

        public clsUnitGroupContainer Target
        {
            get { return target; }
            set
            {
                if ( value == target )
                {
                    return;
                }
                if ( target != null )
                {
                    target.Changed -= SelectedChanged;
                }
                target = value;
                if ( target != null )
                {
                    target.Changed += SelectedChanged;
                }
                SelectedChanged();
            }
        }

        private void tsbNumber_Clicked(object sender, EventArgs e)
        {
            if ( target == null )
            {
                return;
            }

            var tsb = (ToolStripButton)sender;
            var unitGroup = (clsUnitGroup)tsb.Tag;

            target.Item = unitGroup;
        }

        private void SelectedChanged()
        {
            var a = 0;
            var unitGroup = default(clsUnitGroup);

            if ( target != null )
            {
                unitGroup = target.Item;
            }

            if ( unitGroup == null )
            {
                for ( a = 0; a <= 10; a++ )
                {
                    tsbNumber[a].Checked = false;
                }
            }
            else
            {
                for ( a = 0; a <= 10; a++ )
                {
                    tsbNumber[a].Checked = tsbNumber[a].Tag == unitGroup;
                }
            }
        }

        public void SetMap(clsMap newMap)
        {
            var a = 0;

            if ( newMap == null )
            {
                for ( a = 0; a <= Constants.PlayerCountMax - 1; a++ )
                {
                    tsbNumber[a].Tag = null;
                }
                tsbNumber[ScavButtonNum].Tag = null;
            }
            else
            {
                for ( a = 0; a <= Constants.PlayerCountMax - 1; a++ )
                {
                    tsbNumber[a].Tag = newMap.UnitGroups[a];
                }
                tsbNumber[ScavButtonNum].Tag = newMap.ScavengerUnitGroup;
            }
            SelectedChanged();
        }
    }
}