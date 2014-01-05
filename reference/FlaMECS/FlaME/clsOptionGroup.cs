namespace FlaME
{
    using System;

    public class clsOptionGroup
    {
        public modLists.ConnectedList<clsOptionInterface, clsOptionGroup> Options;

        public clsOptionGroup()
        {
            this.Options = new modLists.ConnectedList<clsOptionInterface, clsOptionGroup>(this);
        }
    }
}

