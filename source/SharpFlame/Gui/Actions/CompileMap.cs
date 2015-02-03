using System;
using Eto.Forms;
using SharpFlame.Gui.Dialogs;
using SharpFlame.Mapping;

namespace SharpFlame.Gui.Actions
{
    public class CompileMapCommand : Command
    {
        public CompileMapCommand()
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

            var compileMap = new Dialogs.CompileMapDialog();
            var options = compileMap.ShowModal();
            if( options.CompileType == CompileType.Campaign )
            {
                CompileCampaign(options);
            }
            else
            {
                CompileMultiPlayer(options);
            }
        }

        private void CompileMultiPlayer(CompileOptions options)
        {
            
        }

        private void CompileCampaign(CompileOptions options)
        {
            
        }
    }
}