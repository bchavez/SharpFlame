namespace SharpFlame.Mapping
{
    public class clsGatewayChange
    {
        public clsGateway Gateway;
        public GatewayChangeType Type;
    }

    public enum GatewayChangeType
    {
        Added,
        Deleted
    }
}