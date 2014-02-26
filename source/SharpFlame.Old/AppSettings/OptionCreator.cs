namespace SharpFlame.Old.AppSettings
{
    public class OptionCreator<ValueType>
    {
        public ValueType DefaultValue;
        public string SaveKey;

        public virtual Option<ValueType> Create()
        {
            return new Option<ValueType>(SaveKey, DefaultValue);
        }
    }
}