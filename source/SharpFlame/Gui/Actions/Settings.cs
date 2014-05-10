using System;
using Eto.Forms;
using Ninject;
using SharpFlame;

namespace SharpFlame.Gui.Actions
{
    public class Settings : Command
    {
        public Settings()
        {
            ID = "settings";
            MenuText = "&Settings";
            ToolBarText = "Settings";
            Shortcut = Keys.F12;
        }

        public override void OnExecuted(EventArgs e)
        {
            base.OnExecuted(e);
            // show the settings dialog
            App.Kernel.Get<Dialogs.Settings>().Show();
        }
    }
}

