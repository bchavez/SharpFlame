namespace SharpFlame.Mapping
{
    public class clsGatewayChange
    {
        public enum enumType
        {
            Added,
            Deleted
        }

        public enumType Type;
        public clsGateway Gateway;
    }
}