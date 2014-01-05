namespace FlaME
{
    using System;
    using System.Diagnostics;

    public class clsTextLabels
    {
        public int ItemCount = 0;
        public clsTextLabel[] Items;
        public int MaxCount;

        public clsTextLabels(int MaxItemCount)
        {
            this.MaxCount = MaxItemCount;
            this.Items = new clsTextLabel[(this.MaxCount - 1) + 1];
        }

        public void Add(clsTextLabel NewItem)
        {
            if (this.ItemCount == this.MaxCount)
            {
                Debugger.Break();
            }
            else
            {
                this.Items[this.ItemCount] = NewItem;
                this.ItemCount++;
            }
        }

        public bool AtMaxCount()
        {
            return (this.ItemCount >= this.MaxCount);
        }

        public void Draw()
        {
            int num2 = this.ItemCount - 1;
            for (int i = 0; i <= num2; i++)
            {
                this.Items[i].Draw();
            }
        }
    }
}

