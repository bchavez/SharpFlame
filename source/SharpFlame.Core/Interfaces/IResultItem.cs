namespace SharpFlame.Core.Interfaces
{
    public abstract class IResultItem
    {
        public abstract string GetText { get; }
        public abstract void DoubleClicked();
    }
}
