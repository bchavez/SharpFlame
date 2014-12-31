

using System;
using Eto.Forms;

namespace SharpFlame.Gui.Actions
{
	public class Quit : Command
	{
		public Quit()
		{
			ID = "quit";
			MenuText = "&Quit";
			ToolBarText = "Quit";
			ToolTip = "Close the application";
			Shortcut = Keys.Q | Application.Instance.CommonModifier;
		}

	    protected override void OnExecuted(EventArgs e)
		{
			base.OnExecuted(e);
			Application.Instance.Quit();
		}
	}
}

