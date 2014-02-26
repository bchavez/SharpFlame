#region

using SharpFlame.Old.Mapping.Objects;

#endregion

namespace SharpFlame.Old.Mapping
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