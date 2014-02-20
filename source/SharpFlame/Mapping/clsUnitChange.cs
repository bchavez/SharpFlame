#region

using SharpFlame.Mapping.Objects;

#endregion

namespace SharpFlame.Mapping
{
    public class clsUnitChange
    {
        public enum enumType
        {
            Added,
            Deleted
        }

        public enumType Type;
        public clsUnit Unit;
    }
}