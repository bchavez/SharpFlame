using Appccelerate.EventBroker;
using Appccelerate.EventBroker.Handlers;
using Ninject;
using Ninject.Extensions.Logging;

namespace SharpFlame.Settings
{
    public class Keylogger
    {
        private readonly ILogger logger;

        public Keylogger(ILoggerFactory logFactory) 
        {
            logger = logFactory.GetCurrentClassLogger();
        }

        [EventSubscription(KeyboardManagerEvents.OnKeyDown, typeof(OnPublisher))]
        public void HandleKeyDown(object sender, KeyboardEventArgs e)
        {
            logger.Debug("KeyDown: \"{0}\"=\"{1}\"", e.Key.Name, e.Key.ToString());
        }

        [EventSubscription(KeyboardManagerEvents.OnKeyUp, typeof(OnPublisher))]
        public void HandleKeyUp(object sender, KeyboardEventArgs e)
        {
            logger.Debug("KeyUp: \"{0}\"=\"{1}\"", e.Key.Name, e.Key.ToString());
        }
    }
}