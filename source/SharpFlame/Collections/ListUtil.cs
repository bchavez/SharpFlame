namespace SharpFlame.Collections
{
    public sealed class ListUtil
    {
        public static SimpleList<ItemType> MonoWorkaroundSimpleList<ItemType>(object value)
        {
            return ((SimpleList<ItemType>)value);
        }

        public static ConnectedList<ItemType, SourceType> MonoWorkaroundConnectedList<ItemType, SourceType>(object value) where ItemType : class
            where SourceType : class
        {
            return ((ConnectedList<ItemType, SourceType>)value);
        }
    }
}