using System.Collections.Generic;
using Ninject;
using Ninject.Modules;

namespace SharpFlame.Gui.Infrastructure
{
    public static class Bootstrap
    {
        public static IKernel KernelWith(Eto.Generator generator)
        {
            var settings = new NinjectSettings
                {
                    InjectNonPublic = true,
                    LoadExtensions = false,
                };

            var kernelModules = new List<INinjectModule>
                {
                    new Ninject.Extensions.NamedScope.NamedScopeModule(),
                    new Ninject.Extensions.ContextPreservation.ContextPreservationModule(),
                    new Ninject.Extensions.Logging.NLog2.NLogModule(),
                    new EventBrokerModule(),
                    new SharpFlameModule(),
                };

            var kernel = new StandardKernel(settings, kernelModules.ToArray());

            kernel.Bind<Eto.Generator>().ToMethod(ctx => generator);

            HookEtoGenerator(generator, kernel);

            return kernel;
        }

        private static void HookEtoGenerator(Eto.Generator eto, IKernel k)
        {
            //actually, WidgetCreated is fired when the *generator handler* is created 
            //from the generator factory.

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