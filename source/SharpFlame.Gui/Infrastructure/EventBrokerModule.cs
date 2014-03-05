//Pulled from : ninject / Ninject.Extensions.AppccelerateEventBroker

using System;
using Ninject.Extensions.ContextPreservation;
using Ninject.Extensions.NamedScope;
using Ninject.Modules;

namespace SharpFlame.Gui.Infrastructure
{
    /// <summary>
    /// Module for the event broker extension.
    /// </summary>
    public class EventBrokerModule : NinjectModule
    {
        /// <summary>
        /// The name of the default global event broker
        /// </summary>
        public const string DefaultGlobalEventBrokerName = "GlobalEventBroker";

        /// <summary>
        /// Loads the module into the kernel.
        /// </summary>
        public override void Load()
        {
            this.Kernel.AddGlobalEventBroker(DefaultGlobalEventBrokerName);
        }

        /// <summary>
        /// Called after loading the modules. A module can verify here if all other required modules are loaded.
        /// </summary>
        public override void VerifyRequiredModulesAreLoaded()
        {
            if( !this.Kernel.HasModule(typeof(NamedScopeModule).FullName) )
            {
                throw new InvalidOperationException("The EventBrokerModule requires NamedScopeModule!");
            }

            if( !this.Kernel.HasModule(typeof(ContextPreservationModule).FullName) )
            {
                throw new InvalidOperationException("The EventBrokerModule requires ContextPreservationModule!");
            }
        }
    }
}