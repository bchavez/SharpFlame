#region

using SharpFlame.Mapping.Objects;

#endregion

namespace SharpFlame.Mapping
{
    public class clsUnitChange
    {
        public UnitChangeType Type;
        public clsUnit Unit;
    }

    public enum UnitChangeType
    {
        Added,
        Deleted
    }
}