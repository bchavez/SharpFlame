namespace SharpFlame.AppSettings
{
    public class OptionCreator<ValueType>
    {
        public string SaveKey;
        public ValueType DefaultValue;

        public virtual Option<ValueType> Create()
        {
            return new Option<ValueType>(SaveKey, DefaultValue);
        }
    }
}