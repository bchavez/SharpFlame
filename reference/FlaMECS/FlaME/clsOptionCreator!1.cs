namespace FlaME
{
    using System;

    public class clsOptionCreator<ValueType>
    {
        public ValueType DefaultValue;
        public string SaveKey;

        public virtual clsOption<ValueType> Create()
        {
            return new clsOption<ValueType>(this.SaveKey, this.DefaultValue);
        }
    }
}

