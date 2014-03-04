using Ninject;

namespace SharpFlame.Gui.NinjectBindings
{
    public static class NinjectHook
    {
        public static void HookGenerator(Eto.Generator eto, IKernel k)
        {
            //actually, this is when the handler is created 
            // from the generator factory.

            eto.WidgetCreated += (o, args) =>
                {
                    var newObject = args.Instance;
                    k.Inject(newObject); //this is usually the platform handler.

                    var asWidgetHandler = newObject as Eto.IWidget;
                    if( asWidgetHandler != null )
                    {
                        var widget = asWidgetHandler.Widget;
                        //widget willa ways be null b/c 
                        //widget poreprty is set AFTER widget created is fired.
                        if( widget != null )
                        {
                            k.Inject(widget); // and inject the widget too.
                        }
                    }
                };
        }
    }
}