using System;
using Eto.Drawing;
using Eto.Forms;

namespace SharpFlame.Gui.Actions
{
	public class About : Command
	{
		public About()
		{
			ID = "about";
			MenuText = "About Test Application";
			ToolBarText = "About";
			Shortcut = Keys.F11;
		}

		public override void OnExecuted(EventArgs e)
		{
			base.OnExecuted(e);
			// show the about dialog
			var about = new Dialogs.About();
			about.ShowDialog (Application.Instance.MainForm);
		}
	}
}

