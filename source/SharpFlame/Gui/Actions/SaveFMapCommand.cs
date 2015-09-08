using System;
using Appccelerate.EventBroker;
using Appccelerate.Events;
using Eto;
using Eto.Forms;
using Ninject;
using SharpFlame.Core;
using SharpFlame.Gui.Forms;
using SharpFlame.Mapping;
using SharpFlame.Mapping.IO.FMap;
using SharpFlame.Settings;

namespace SharpFlame.Gui.Actions
{
	public class SaveFMapCommand : Command
	{
		private readonly MainForm m;
		private readonly IEventBroker eventBroker;

		public SaveFMapCommand(MainForm m, IEventBroker eventBroker)
		{
			this.m = m;
			this.eventBroker = eventBroker;
		}

		private Map Map => m.MapPanel.MainMap;


		protected override void OnExecuted(EventArgs e)
		{
			base.OnExecuted(e);

			if( Save_FMap_Quick() )
			{
				
			}
		}

		public bool Save_FMap_Quick()
		{
			var saver = App.Kernel.Get<FMapSaver>();

			if (Map.PathInfo == null)
			{
				return Save_FMap_Prompt(saver);
			}

			if (Map.PathInfo.IsFMap)
			{
				var result = saver.Save(Map.PathInfo.Path, Map, true, true);
				if (!result.HasProblems)
				{
					Map.ChangedSinceSave = false;
				}
				App.ShowWarnings(result);
				return !result.HasProblems;
			}
			return Save_FMap_Prompt(saver);
		}

		public bool Save_FMap_Prompt(FMapSaver saver)
		{
			var dialog = new SaveFileDialog
				{
					Filters =
						{
							new FileDialogFilter("SharpFlame Map", ".sflame")
						},
					Directory = new Uri(Eto.EtoEnvironment.GetFolderPath(EtoSpecialFolder.Documents)),
					FileName = "",
				};
			if( dialog.ShowDialog(this.m) != DialogResult.Ok )
			{
				return false;
			}
			var savedir = System.IO.Path.GetDirectoryName(dialog.FileName);

			eventBroker.Fire(EventTopics.OnMapSave, this, new EventArgs<string>(savedir));

			var result = saver.Save(dialog.FileName, this.Map, true, true);
			if( !result.HasProblems )
			{
				Map.PathInfo = new PathInfo(dialog.FileName, true);
				Map.ChangedSinceSave = false;
			}

			App.ShowWarnings(result);
			return !result.HasProblems;
		}
	}
}