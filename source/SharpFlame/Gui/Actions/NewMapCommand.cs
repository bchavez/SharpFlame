using System;
using Appccelerate.EventBroker;
using Appccelerate.Events;
using Eto.Forms;
using Ninject;
using Ninject.Extensions.Logging;
using SharpFlame.Core;
using SharpFlame.Core.Domain;
using SharpFlame.Mapping;

namespace SharpFlame.Gui.Actions
{
    public class NewMapCommand : Command
    {
        [EventPublication(EventTopics.OnMapLoad)]
        public event EventHandler<EventArgs<Map>> OnMapLoad = delegate { };

        [Inject]
        internal ILoggerFactory LoggerFactory { get; set; }

        [Inject]
        internal IEventBroker EventBroker { get; set; }

        [Inject]
        internal ILogger Logger { get; set; }

        [Inject]
        internal IKernel Kernel { get; set; }

        public NewMapCommand()
        {
            this.ID = "newMap";
            this.MenuText = "&New Map";
            this.ToolBarText = "New Map";
        }

        protected override void OnExecuted(EventArgs e)
        {
            base.OnExecuted(e);

            var map = new Map(LoggerFactory, EventBroker);
            map.Create(new XYInt(64, 64));

            this.OnMapLoad(this, new EventArgs<Map>(map));

            map.RandomizeTileOrientations();
            map.Update(null);
            map.UndoClear();
        }
    }
}