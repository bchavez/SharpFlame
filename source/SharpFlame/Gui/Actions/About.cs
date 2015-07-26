using System;
using Eto.Forms;
using SharpFlame.Core;

namespace SharpFlame.Gui.Actions
{
	public class About : Command
	{
		public About()
		{
			ID = "about";
			MenuText = string.Format("About {0}", Constants.ProgramName);
			ToolBarText = "About";
			Shortcut = Keys.F11;
		}

	    protected override void OnExecuted(EventArgs e)
		{
			base.OnExecuted(e);
			// show the about dialog
			var about = new Dialogs.About();
	        about.ShowModal(Application.Instance.MainForm);
		}
	}
}

