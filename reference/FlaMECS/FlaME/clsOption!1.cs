namespace FlaME
{
    using System;

    public class clsOption<ValueType> : clsOptionInterface
    {
        private ValueType _DefaultValue;
        private modLists.ConnectedListLink<clsOptionInterface, clsOptionGroup> _GroupLink;
        private string _SaveKey;

        public clsOption(string saveKey, ValueType defaultValue)
        {
            this._GroupLink = new modLists.ConnectedListLink<clsOptionInterface, clsOptionGroup>(this);
            this._SaveKey = saveKey;
            this._DefaultValue = defaultValue;
        }

        public override bool IsValueValid(object value)
        {
            return true;
        }

        public ValueType DefaultValue
        {
            get
            {
                return this._DefaultValue;
            }
        }

        public override object DefaultValueObject
        {
            get
            {
                return this._DefaultValue;
            }
        }

        public override modLists.ConnectedListLink<clsOptionInterface, clsOptionGroup> GroupLink
        {
            get
            {
                return this._GroupLink;
            }
        }

        public override string SaveKey
        {
            get
            {
                return this._SaveKey;
            }
        }
    }
}

