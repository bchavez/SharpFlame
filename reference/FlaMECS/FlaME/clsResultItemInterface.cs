namespace FlaME
{
    using System;

    public abstract class clsResultItemInterface
    {
        protected clsResultItemInterface()
        {
        }

        public abstract void DoubleClicked();

        public abstract string GetText { get; }
    }
}

