using Eto;

namespace SharpFlame.Gui.Dialogs
{
    public static class XomlReader
    {
        public static T Load<T>() where T : Widget, new()
        {
            var type = typeof(T);
            var stream = type.Assembly.GetManifestResourceStream(type.FullName + ".xoml");
            return Eto.Serialization.Xaml.XamlReader.Load<T>(stream, null);
        }

        public static T Load<T>(T instance) where T : Widget
        {
            var type = typeof(T);
            var stream = type.Assembly.GetManifestResourceStream(type.FullName + ".xoml");
            return Eto.Serialization.Xaml.XamlReader.Load<T>(stream, instance);
        }
    }
}