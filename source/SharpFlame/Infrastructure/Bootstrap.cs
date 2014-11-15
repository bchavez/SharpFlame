using System.Collections.Generic;
using Ninject;
using Ninject.Modules;
using Ninject.Planning.Bindings.Resolvers;

namespace SharpFlame.Infrastructure
{
    public static class Bootstrap
    {
        public static IKernel KernelWith(Eto.Platform generator)
        {
            var settings = new NinjectSettings
                {
                    InjectNonPublic = true,
                    LoadExtensions = false,
                    AllowNullInjection = false,
                };

            var kernelModules = new List<INinjectModule>
                {
                    new Ninject.Extensions.NamedScope.NamedScopeModule(),
                    new Ninject.Extensions.ContextPreservation.ContextPreservationModule(),
                    new Ninject.Extensions.Logging.NLog2.NLogModule(),
                    new EventBrokerModule(),
                    new SharpFlameModule(),
                };

            var kernel = new ExplicitKernel(settings, kernelModules.ToArray());

            kernel.Bind<Eto.Generator>().ToMethod(ctx => generator);

            HookEtoGenerator(generator, kernel);

            return kernel;
        }

        private static void HookEtoGenerator(Eto.Platform eto, IKernel k)
        {
            //actually, WidgetCreated is fired when the *generator handler* is created 
            //from the generator factory.

            eto.WidgetCreated += (o, args) =>
                {
                    var newObject = args.Instance;
                    k.Inject(newObject); //this is usually the platform handler.

                    var asWidgetHandler = newObject as Eto.IControlObjectSource;
                    if( asWidgetHandler != null )
                    {
                        var widget = asWidgetHandler.ControlObject;
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

    public class ExplicitKernel : StandardKernel
    {
        public ExplicitKernel(INinjectSettings settings, params INinjectModule[] modules) : base(settings, modules)
        {
        }

        protected override bool HandleMissingBinding(Ninject.Activation.IRequest request)
        {
			System.Console.WriteLine ("RESOLVING: " + request.Service.ToString());
            return false;
        }
    }
}