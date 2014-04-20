//pulled from: ninject / Ninject.Extensions.AppccelerateEventBroker

using System.Globalization;
using Appccelerate.EventBroker;
using Ninject.Extensions.ContextPreservation;
using Ninject.Extensions.NamedScope;
using Ninject.Syntax;

namespace SharpFlame.Infrastructure
{
    /// <summary>
    /// Extension methods for registering objects on the event broker.
    /// </summary>
    public static class EventBrokerExtensionMethods
    {
        /// <summary>
        /// Defines that the object created by the binding shall be registered on the specified event broker.
        /// </summary>
        /// <typeparam name="T">The type of the binding.</typeparam>
        /// <param name="syntax">The syntax.</param>
        /// <param name="eventBrokerName">Name of the event broker.</param>
        /// <returns>The syntax.</returns>
        public static IBindingOnSyntax<T> RegisterOnEventBroker<T>(
            this IBindingOnSyntax<T> syntax, string eventBrokerName)
        {
            return
                syntax.OnActivation((ctx, instance) => ctx.ContextPreservingGet<IEventBroker>(eventBrokerName).Register(instance))
                    .OnDeactivation((ctx, instance) => ctx.ContextPreservingGet<IEventBroker>(eventBrokerName).Unregister(instance));
        }

        /// <summary>
        /// Defines that the object created by the binding shall be registered on the default global event broker.
        /// </summary>
        /// <typeparam name="T">The type of the binding.</typeparam>
        /// <param name="syntax">The syntax.</param>
        /// <returns>The syntax.</returns>
        public static IBindingOnSyntax<T> RegisterOnGlobalEventBroker<T>(
            this IBindingOnSyntax<T> syntax)
        {
            return RegisterOnEventBroker(syntax, EventBrokerModule.DefaultGlobalEventBrokerName);
        }

        /// <summary>
        /// Adds a global event broker to the kernel.
        /// </summary>
        /// <param name="bindingRoot">The binding root.</param>
        /// <param name="eventBrokerName">Name of the event broker.</param>
        public static void AddGlobalEventBroker(this IBindingRoot bindingRoot, string eventBrokerName)
        {
            bindingRoot.Bind<IEventBroker>().To<EventBroker>().InSingletonScope().Named(eventBrokerName);
            bindingRoot.Bind<IEventBroker>().ToMethod(ctx => ctx.ContextPreservingGet<IEventBroker>(eventBrokerName)).WhenTargetNamed(eventBrokerName);
        }

        /// <summary>
        /// Defines that the object created by a binding owns an event broker.
        /// Object created in the object tree below this binding can use this event broker.
        /// </summary>
        /// <typeparam name="T">The type of the binding.</typeparam>
        /// <param name="syntax">The syntax.</param>
        /// <param name="eventBrokerName">Name of the event broker.</param>
        /// <returns>The syntax</returns>
        public static IBindingOnSyntax<T> OwnsEventBroker<T>(this IBindingOnSyntax<T> syntax, string eventBrokerName)
        {
            string namedScopeName = "EventBrokerScope" + eventBrokerName;
            syntax.DefinesNamedScope(namedScopeName);
            syntax.Kernel.Bind<IEventBroker>().To<EventBroker>().InNamedScope(namedScopeName).Named(eventBrokerName);
            syntax.Kernel.Bind<IEventBroker>().ToMethod(ctx => ctx.ContextPreservingGet<IEventBroker>(eventBrokerName)).WhenTargetNamed(eventBrokerName);
            return syntax;
        }

        /// <summary>
        /// Condition that matches when the target has the given name.
        /// </summary>
        /// <typeparam name="T">The type of the binding.</typeparam>
        /// <param name="syntax">The syntax.</param>
        /// <param name="name">The name.</param>
        /// <returns>The syntax to define more things for the binding.</returns>
        public static IBindingInNamedWithOrOnSyntax<T> WhenTargetNamed<T>(this IBindingWhenSyntax<T> syntax, string name)
        {
            return syntax.When(
                request =>
                    request.Target != null &&
                    request.Target.Name.ToUpper(CultureInfo.InvariantCulture) == name.ToUpper(CultureInfo.InvariantCulture));
        }
    }
}