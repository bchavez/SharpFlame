namespace SharpFlame.Old.AppSettings
{
    public class Change<ValueType> : ChangeInterface
    {
        public ValueType Value;

        public Change(ValueType value)
        {
            Value = value;
        }

        public override object ValueObject
        {
            get { return Value; }
        }

        public override ChangeInterface GetCopy()
        {
            return new Change<ValueType>(Value);
        }
    }
}