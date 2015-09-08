using System;
using Eto.Forms;
using Ninject;

namespace SharpFlame.Gui.Actions
{
	public class SettingsCommand : Command
	{
		public SettingsCommand()
		{
			ID = "settings";
			MenuText = "&Settings";
			ToolBarText = "Settings";
			Shortcut = Keys.F12;
		}

		protected override void OnExecuted(EventArgs e)
		{
			base.OnExecuted(e);
			// show the settings dialog
			App.Kernel.Get<Dialogs.Settings>().Show();
		}
	}
}