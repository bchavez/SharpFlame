using System;
using Appccelerate.EventBroker;
using Appccelerate.EventBroker.Handlers;

namespace SharpFlame.Core
{
    public static class EventTopics
    {
        public const string OnMapLoad = "topic://Map/Load";
        public const string OnMapDrawLater = "topic://Map/DrawLater";
        public const string OnMapUpdate = "topic://Map/Update";
        public const string OnMinimapRefresh = "topic://Minimap/Refresh";
        public const string OnScriptMarkerUpdate = "topic://Labels/Update";

	    public const string OnOpenGLInitalized = "topic://GL/Init";

	    public const string OnObjectManagerLoaded = "topic://Object/Loaded";

        public static void UpdateMap(this IEventBroker broker, object sender)
        {
            broker.Fire(OnMapUpdate, new OnPublisher(), HandlerRestriction.None, sender, EventArgs.Empty);
        }
        public static void DrawLater(this IEventBroker broker, object sender)
        {
            broker.Fire(OnMapDrawLater, new OnPublisher(), HandlerRestriction.None, sender, EventArgs.Empty);
        }
        public static void RefreshMinimap(this IEventBroker broker, object sender)
        {
            broker.Fire(OnMinimapRefresh, new OnPublisher(), HandlerRestriction.None, sender, EventArgs.Empty);
        }
        public static void ScriptMarkerUpdate(this IEventBroker broker, object sender)
        {
            broker.Fire(OnScriptMarkerUpdate, new OnPublisher(), HandlerRestriction.None, sender, EventArgs.Empty);
        }
    }
}