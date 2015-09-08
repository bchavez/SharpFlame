using System;
using System.IO;
using Appccelerate.EventBroker;
using Appccelerate.Events;
using Eto.Forms;
using Ninject;
using Ninject.Extensions.Logging;
using SharpFlame.Core;
using SharpFlame.Mapping;
using SharpFlame.Mapping.IO;
using SharpFlame.Settings;

namespace SharpFlame.Gui.Actions
{
	public class LoadMapCommand : Command
	{
		[EventPublication(EventTopics.OnMapLoad)]
		public event EventHandler<EventArgs<Map>> OnMapLoad = delegate {  };

		[Inject]
		internal ILogger Logger { get; set; }

		[Inject]
		internal SettingsManager Settings { get; set; }

		[Inject]
		internal IKernel Kernel { get; set; }

		public LoadMapCommand()
		{
			ID = "loadMap";
			MenuText = "&Open";
			ToolBarText = "Open";
		}

		protected override void OnExecuted(EventArgs e)
		{
			base.OnExecuted(e);
			// show the about dialog

			var dialog = new OpenFileDialog
				{
					Directory = new Uri(Settings.OpenPath),
					Filters =
						{
							new FileDialogFilter("All Supported Formats", "fmap", "wz", "game", "lnd"),
							new FileDialogFilter("FMAP Files", "fmap"),
							new FileDialogFilter("WZ Files", "wz"),
							new FileDialogFilter("Game Files", "game"),
							new FileDialogFilter("LND Files", "lnd")
						}
				};

			var result = dialog.ShowDialog(Application.Instance.MainForm);
			if(result == DialogResult.Ok)
			{
				// Set Openpath to the directory of the selected file and save the settings.
				Settings.OpenPath = new Uri(Path.GetDirectoryName(dialog.FileName)).ToString();
				var returnResult = Settings.Save(App.SettingsPath);
				if(returnResult.HasProblems)
				{
					App.StatusDialog = new Dialogs.Status(returnResult);
					App.StatusDialog.Show();
				}
				var ext = Path.GetExtension(dialog.FileName).ToLower();
				var loader = this.Kernel.Get<IIOLoader>(ext);
                
				if( loader == null )
				{
					returnResult = new Result(string.Format("Loading \"{0}\"", Path.GetExtension(dialog.FileName)), false);
					returnResult.ProblemAdd(string.Format("UNKNOWN File type: can\'t load file \"{0}\"", dialog.FileName));
					App.StatusDialog = new Dialogs.Status(returnResult);
					App.StatusDialog.Show();
					Logger.Error("Loading \"{0}\", UNKNOWN File type: can\'t load file \"{1}\"", Path.GetExtension(dialog.FileName), dialog.FileName);
					return;
				}

				var loadResult = loader.Load(dialog.FileName);
				if(loadResult.HasProblems || loadResult.HasWarnings)
				{
					App.StatusDialog = new Dialogs.Status(loadResult.ToResult());
					App.StatusDialog.Show();
				}

				if(!loadResult.HasProblems)
				{
					loadResult.Value.PathInfo = new PathInfo(dialog.FileName, true);
					OnMapLoad(this, new EventArgs<Map>(loadResult.Value));
				} else
				{
					loadResult.Value.Deallocate();
				}
			}
		}
	}
}