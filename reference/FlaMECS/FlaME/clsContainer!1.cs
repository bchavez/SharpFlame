namespace FlaME
{
    using System;

    public class clsContainer<ItemType>
    {
        public ItemType Item;

        public clsContainer(ItemType item)
        {
            this.Item = item;
        }
    }
}

