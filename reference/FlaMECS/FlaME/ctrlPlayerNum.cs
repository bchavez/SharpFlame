namespace FlaME
{
    using Microsoft.VisualBasic.CompilerServices;
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Runtime.CompilerServices;
    using System.Windows.Forms;

    [DesignerGenerated]
    public class ctrlPlayerNum : UserControl
    {
        private clsMap.clsUnitGroupContainer _Target;
        [AccessedThroughProperty("tsPlayerNum1")]
        private ToolStrip _tsPlayerNum1;
        [AccessedThroughProperty("tsPlayerNum2")]
        private ToolStrip _tsPlayerNum2;
        private IContainer components;
        public const int ScavButtonNum = 10;
        public ToolStripButton[] tsbNumber;

        public ctrlPlayerNum()
        {
            int num;
            this.tsbNumber = new ToolStripButton[11];
            this.InitializeComponent();
            int num3 = 5;
            int num4 = num3 - 1;
            for (num = 0; num <= num4; num++)
            {
                this.tsbNumber[num] = new ToolStripButton();
                this.tsbNumber[num].DisplayStyle = ToolStripItemDisplayStyle.Text;
                this.tsbNumber[num].Text = modIO.InvariantToString_int(num);
                this.tsbNumber[num].AutoToolTip = false;
                this.tsbNumber[num].Click += new EventHandler(this.tsbNumber_Clicked);
                this.tsPlayerNum1.Items.Add(this.tsbNumber[num]);
                int index = num + num3;
                this.tsbNumber[index] = new ToolStripButton();
                this.tsbNumber[index].DisplayStyle = ToolStripItemDisplayStyle.Text;
                this.tsbNumber[index].Text = modIO.InvariantToString_int(index);
                this.tsbNumber[index].AutoToolTip = false;
                this.tsbNumber[index].Click += new EventHandler(this.tsbNumber_Clicked);
                this.tsPlayerNum2.Items.Add(this.tsbNumber[index]);
            }
            num = 10;
            this.tsbNumber[num] = new ToolStripButton();
            this.tsbNumber[num].DisplayStyle = ToolStripItemDisplayStyle.Text;
            this.tsbNumber[num].Text = "S";
            this.tsbNumber[num].AutoToolTip = false;
            this.tsbNumber[num].Click += new EventHandler(this.tsbNumber_Clicked);
            this.tsPlayerNum2.Items.Add(this.tsbNumber[num]);
            this.Width = 0x90;
            this.Height = 50;
        }

        [DebuggerNonUserCode]
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && (this.components != null))
                {
                    this.components.Dispose();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        [DebuggerStepThrough]
        private void InitializeComponent()
        {
            this.tsPlayerNum1 = new ToolStrip();
            this.tsPlayerNum2 = new ToolStrip();
            this.SuspendLayout();
            this.tsPlayerNum1.Font = new Font("Microsoft Sans Serif", 7.8f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.tsPlayerNum1.GripStyle = ToolStripGripStyle.Hidden;
            Point point2 = new Point(0, 0);
            this.tsPlayerNum1.Location = point2;
            this.tsPlayerNum1.Name = "tsPlayerNum1";
            Size size2 = new Size(0x38, 0x19);
            this.tsPlayerNum1.Size = size2;
            this.tsPlayerNum1.TabIndex = 0;
            this.tsPlayerNum1.Text = "ToolStrip1";
            this.tsPlayerNum2.Font = new Font("Microsoft Sans Serif", 7.8f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.tsPlayerNum2.GripStyle = ToolStripGripStyle.Hidden;
            point2 = new Point(0, 0x19);
            this.tsPlayerNum2.Location = point2;
            this.tsPlayerNum2.Name = "tsPlayerNum2";
            size2 = new Size(0x38, 0x19);
            this.tsPlayerNum2.Size = size2;
            this.tsPlayerNum2.TabIndex = 1;
            this.tsPlayerNum2.Text = "ToolStrip1";
            this.AutoScaleMode = AutoScaleMode.None;
            this.Controls.Add(this.tsPlayerNum2);
            this.Controls.Add(this.tsPlayerNum1);
            this.Name = "ctrlPlayerNum";
            size2 = new Size(0x38, 50);
            this.Size = size2;
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void SelectedChanged()
        {
            int num;
            clsMap.clsUnitGroup item;
            if (this._Target == null)
            {
                item = null;
            }
            else
            {
                item = this._Target.Item;
            }
            if (item == null)
            {
                num = 0;
                do
                {
                    this.tsbNumber[num].Checked = false;
                    num++;
                }
                while (num <= 10);
            }
            else
            {
                num = 0;
                do
                {
                    this.tsbNumber[num].Checked = ((clsMap.clsUnitGroup) this.tsbNumber[num].Tag) == item;
                    num++;
                }
                while (num <= 10);
            }
        }

        public void SetMap(clsMap NewMap)
        {
            int num;
            if (NewMap == null)
            {
                num = 0;
                do
                {
                    this.tsbNumber[num].Tag = null;
                    num++;
                }
                while (num <= 9);
                this.tsbNumber[10].Tag = null;
            }
            else
            {
                num = 0;
                do
                {
                    this.tsbNumber[num].Tag = NewMap.UnitGroups[num];
                    num++;
                }
                while (num <= 9);
                this.tsbNumber[10].Tag = NewMap.ScavengerUnitGroup;
            }
            this.SelectedChanged();
        }

        private void tsbNumber_Clicked(object sender, EventArgs e)
        {
            if (this._Target != null)
            {
                ToolStripButton button = (ToolStripButton) sender;
                clsMap.clsUnitGroup tag = (clsMap.clsUnitGroup) button.Tag;
                this._Target.Item = tag;
            }
        }

        public clsMap.clsUnitGroupContainer Target
        {
            get
            {
                return this._Target;
            }
            set
            {
                if (value != this._Target)
                {
                    if (this._Target != null)
                    {
                        this._Target.Changed -= new clsMap.clsUnitGroupContainer.ChangedEventHandler(this.SelectedChanged);
                    }
                    this._Target = value;
                    if (this._Target != null)
                    {
                        this._Target.Changed += new clsMap.clsUnitGroupContainer.ChangedEventHandler(this.SelectedChanged);
                    }
                    this.SelectedChanged();
                }
            }
        }

        public virtual ToolStrip tsPlayerNum1
        {
            get
            {
                return this._tsPlayerNum1;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._tsPlayerNum1 = value;
            }
        }

        public virtual ToolStrip tsPlayerNum2
        {
            get
            {
                return this._tsPlayerNum2;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._tsPlayerNum2 = value;
            }
        }
    }
}

