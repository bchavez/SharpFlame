namespace SharpFlame.AppSettings
{
    public abstract class ChangeInterface
    {
        public abstract object ValueObject { get; }
        public abstract ChangeInterface GetCopy();
    }
}