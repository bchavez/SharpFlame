#region

using System;
using System.Windows.Forms;
using SharpFlame.Old.FileIO;
using SharpFlame.Old.Mapping.Objects;
using SharpFlame.Core;
using SharpFlame.Old.Mapping;
using SharpFlame.Old.Mapping.Objects;

#endregion

namespace SharpFlame.Old.Controls
{
    public partial class PlayerNumControl
    {
        public const int ScavButtonNum = 10;
        private clsUnitGroupContainer target;
        public ToolStripButton[] TsbNumber = new ToolStripButton[11];

        public PlayerNumControl()
        {
            InitializeComponent();

            var a = 0;
            const int buttonsPerRow = 5;

            for ( a = 0; a <= buttonsPerRow - 1; a++ )
            {
                TsbNumber[a] = new ToolStripButton();
                TsbNumber[a].DisplayStyle = ToolStripItemDisplayStyle.Text;
                TsbNumber[a].Text = a.ToStringInvariant();
                TsbNumber[a].AutoToolTip = false;
                TsbNumber[a].Click += tsbNumber_Clicked;
                tsPlayerNum1.Items.Add(TsbNumber[a]);

                var b = a + buttonsPerRow;
                TsbNumber[b] = new ToolStripButton();
                TsbNumber[b].DisplayStyle = ToolStripItemDisplayStyle.Text;
                TsbNumber[b].Text = b.ToStringInvariant();
                TsbNumber[b].AutoToolTip = false;
                TsbNumber[b].Click += tsbNumber_Clicked;
                tsPlayerNum2.Items.Add(TsbNumber[b]);
            }

            a = 10;

            TsbNumber[a] = new ToolStripButton
                {
                    DisplayStyle = ToolStripItemDisplayStyle.Text,
                    Text = "S",
                    AutoToolTip = false
                };
            TsbNumber[a].Click += tsbNumber_Clicked;
            tsPlayerNum2.Items.Add(TsbNumber[a]);

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
                    TsbNumber[a].Checked = false;
                }
            }
            else
            {
                for ( a = 0; a <= 10; a++ )
                {
                    TsbNumber[a].Checked = TsbNumber[a].Tag == unitGroup;
                }
            }
        }

        public void SetMap(Map newMap)
        {
            var a = 0;

            if ( newMap == null )
            {
                for ( a = 0; a <= Constants.PlayerCountMax - 1; a++ )
                {
                    TsbNumber[a].Tag = null;
                }
                TsbNumber[ScavButtonNum].Tag = null;
            }
            else
            {
                for ( a = 0; a <= Constants.PlayerCountMax - 1; a++ )
                {
                    TsbNumber[a].Tag = newMap.UnitGroups[a];
                }
                TsbNumber[ScavButtonNum].Tag = newMap.ScavengerUnitGroup;
            }
            SelectedChanged();
        }
    }
}