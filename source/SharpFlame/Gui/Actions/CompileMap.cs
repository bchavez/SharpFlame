using System;
using Eto.Forms;

namespace SharpFlame.Gui.Actions
{
    public class CompileMap : Command
    {
        public CompileMap()
        {
            ID = "compile";
            MenuText = "&Compile Map ...";
            ToolBarText = "&Compile Map ...";
            ToolTip = "Compile Map";
            Shortcut = Keys.C | Application.Instance.CommonModifier;
        }

        protected override void OnExecuted(EventArgs e)
        {
            base.OnExecuted(e);

            var compileMap = new Dialogs.CompileMap();
            compileMap.ShowModal(Application.Instance.MainForm);
        }
    }
}