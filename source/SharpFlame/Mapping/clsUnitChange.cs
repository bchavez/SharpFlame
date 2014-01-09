using SharpFlame.Mapping.Objects;

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