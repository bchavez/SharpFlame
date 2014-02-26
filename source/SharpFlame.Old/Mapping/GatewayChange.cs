namespace SharpFlame.Old.Mapping
{
    public class GatewayChange
    {
        public Gateway Gateway;
        public GatewayChangeType Type;
    }

    public enum GatewayChangeType
    {
        Added,
        Deleted
    }
}