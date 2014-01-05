namespace SharpFlame.AppSettings
{
    public class Change<ValueType> : ChangeInterface
    {
        public ValueType Value;

        public override object ValueObject
        {
            get { return Value; }
        }

        public Change(ValueType value)
        {
            Value = value;
        }

        public override ChangeInterface GetCopy()
        {
            return new Change<ValueType>(Value);
        }
    }
}