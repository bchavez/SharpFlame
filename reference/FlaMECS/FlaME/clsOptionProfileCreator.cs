namespace FlaME
{
    using System;

    public class clsOptionProfileCreator
    {
        public clsOptionGroup Options;

        public clsOptionProfileCreator()
        {
        }

        public clsOptionProfileCreator(clsOptionGroup options)
        {
            this.Options = options;
        }

        public virtual clsOptionProfile Create()
        {
            return new clsOptionProfile(this.Options);
        }
    }
}

