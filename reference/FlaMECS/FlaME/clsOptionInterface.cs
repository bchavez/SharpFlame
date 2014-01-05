namespace FlaME
{
    using System;

    public abstract class clsOptionInterface
    {
        protected clsOptionInterface()
        {
        }

        public abstract bool IsValueValid(object value);

        public abstract object DefaultValueObject { get; }

        public abstract modLists.ConnectedListLink<clsOptionInterface, clsOptionGroup> GroupLink { get; }

        public abstract string SaveKey { get; }
    }
}

