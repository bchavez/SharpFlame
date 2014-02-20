#region

using Newtonsoft.Json;
using SharpFlame.Collections;

#endregion

namespace SharpFlame.AppSettings
{
    public class Option<ValueType> : OptionInterface
    {
        private readonly ValueType _DefaultValue;
        private readonly ConnectedListLink<OptionInterface, OptionGroup> _GroupLink;

        private readonly string _SaveKey;

        public Option(string saveKey, ValueType defaultValue)
        {
            _GroupLink = new ConnectedListLink<OptionInterface, OptionGroup>(this);


            _SaveKey = saveKey;
            _DefaultValue = defaultValue;
        }

        [JsonIgnore]
        public override ConnectedListLink<OptionInterface, OptionGroup> GroupLink
        {
            get { return _GroupLink; }
        }

        [JsonIgnore]
        public ValueType DefaultValue
        {
            get { return _DefaultValue; }
        }

        [JsonIgnore]
        public override object DefaultValueObject
        {
            get { return _DefaultValue; }
        }

        [JsonIgnore]
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