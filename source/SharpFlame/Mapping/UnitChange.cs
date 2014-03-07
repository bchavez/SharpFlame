#region

using SharpFlame.Mapping.Objects;

#endregion

namespace SharpFlame.Mapping
{
    public class UnitChange
    {
        public UnitChangeType Type;
        public Unit Unit;
    }

    public enum UnitChangeType
    {
        Added,
        Deleted
    }
}