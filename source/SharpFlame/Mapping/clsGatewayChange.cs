namespace SharpFlame.Mapping
{
    public class clsGatewayChange
    {
        public enum enumType
        {
            Added,
            Deleted
        }

        public clsGateway Gateway;
        public enumType Type;
    }
}