using System.Collections.Generic;
using Ninject;
using Ninject.Modules;
using Ninject.Planning.Bindings.Resolvers;

namespace SharpFlame.Infrastructure
{
    public static class Bootstrap
    {
        public static IKernel KernelWith(Eto.Platform platform)
        {
            var settings = new NinjectSettings
                {
                    InjectNonPublic = true,
                    LoadExtensions = false,
                    AllowNullInjection = false
                };

            var kernelModules = new List<INinjectModule>
                {
                    new Ninject.Extensions.NamedScope.NamedScopeModule(),
                    new Ninject.Extensions.ContextPreservation.ContextPreservationModule(),
                    new Ninject.Extensions.Logging.NLog2.NLogModule(),
                    new EventBrokerModule(),
                    new SharpFlameModule(),
                };

            var k = new StandardKernel(settings, kernelModules.ToArray());
            //only allow injection of explicitly bound types
            k.Components.Remove<IMissingBindingResolver, SelfBindingResolver>();

            k.Bind<Eto.Platform>().ToMethod(ctx => platform);
            
            HookEtoGenerator(platform, k);

            return k;
        }

        private static void HookEtoGenerator(Eto.Platform eto, IKernel k)
        {
            eto.WidgetCreated += (sender, args) =>
                {
                    k.Inject(args.Instance);
                };
        }
    }
}