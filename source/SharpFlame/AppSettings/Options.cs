#region

using Newtonsoft.Json;
using SharpFlame.Collections;
using SharpFlame.Core.Collections;

#endregion

namespace SharpFlame.AppSettings
{
    public class Option<ValueType> : OptionInterface
    {
        private readonly ValueType defaultValue;
        private readonly ConnectedListLink<OptionInterface, OptionGroup> groupLink;

        private readonly string saveKey;

        public Option(string saveKey, ValueType defaultValue)
        {
            groupLink = new ConnectedListLink<OptionInterface, OptionGroup>(this);


            this.saveKey = saveKey;
            this.defaultValue = defaultValue;
        }

        [JsonIgnore]
        public override ConnectedListLink<OptionInterface, OptionGroup> GroupLink
        {
            get { return groupLink; }
        }

        [JsonIgnore]
        public ValueType DefaultValue
        {
            get { return defaultValue; }
        }

        [JsonIgnore]
        public override object DefaultValueObject
        {
            get { return defaultValue; }
        }

        [JsonIgnore]
        public override string SaveKey
        {
            get { return saveKey; }
        }

        public override bool IsValueValid(object value)
        {
            return true;
        }
    }
}