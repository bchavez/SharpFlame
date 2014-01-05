using SharpFlame.Collections;

namespace SharpFlame.AppSettings
{
    public class Option<ValueType> : OptionInterface
    {
        private ConnectedListLink<OptionInterface, OptionGroup> _GroupLink;

        public override ConnectedListLink<OptionInterface, OptionGroup> GroupLink
        {
            get { return _GroupLink; }
        }

        private string _SaveKey;
        private ValueType _DefaultValue;

        public ValueType DefaultValue
        {
            get { return _DefaultValue; }
        }

        public Option(string saveKey, ValueType defaultValue)
        {
            _GroupLink = new ConnectedListLink<OptionInterface, OptionGroup>(this);


            _SaveKey = saveKey;
            _DefaultValue = defaultValue;
        }

        public override object DefaultValueObject
        {
            get { return _DefaultValue; }
        }

        public override string SaveKey
        {
            get { return _SaveKey; }
        }

        public override bool IsValueValid(object value)
        {
            return true;
        }
    }
}