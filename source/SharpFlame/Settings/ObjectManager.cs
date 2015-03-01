using System;
using Appccelerate.EventBroker;
using Appccelerate.EventBroker.Handlers;
using Appccelerate.Events;
using Eto.Gl;
using Ninject;
using SharpFlame.Core;
using SharpFlame.Domain.ObjData;

namespace SharpFlame.Settings
{
	public class ObjectManager
	{
		public ObjectData ObjectData { get; set; }

		public ObjectManager()
		{
			this.ObjectData = new ObjectData();
		}

		[Inject]
		internal SettingsManager SettingsManager { get; set; }

		[EventSubscription(EventTopics.OnOpenGLInitalized, typeof(OnPublisher))]
		public void HandleOpenGlInitalized(object sender, EventArgs<GLSurface> gl)
		{
			// Load Object Data. We need to know when GL is initialized
			// because of the texture loading in the object data directory.
			foreach( var path in this.SettingsManager.ObjectDataDirectories )
			{
				if( !string.IsNullOrEmpty(path) )
				{
					SharpFlameApplication.InitializeResult.Add(this.ObjectData.LoadDirectory(path));
				}
			}

			App.ObjectData = this.ObjectData;

			this.OnObjectManagerLoaded(this, EventArgs.Empty);
		}

		[EventPublication(EventTopics.OnObjectManagerLoaded)]
		public event EventHandler<EventArgs> OnObjectManagerLoaded = delegate { };
	}
}