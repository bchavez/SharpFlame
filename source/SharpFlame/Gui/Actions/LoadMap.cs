using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using Eto.Forms;
using Ninject;
using Ninject.Extensions.Logging;
using SharpFlame.Core;
using SharpFlame.Core.Extensions;
using SharpFlame.Core.Interfaces.Mapping.IO;
using SharpFlame.Gui.Sections;
using SharpFlame;
using SharpFlame.Mapping;
using SharpFlame.Mapping.IO.FMap;
using SharpFlame.Mapping.IO.LND;
using SharpFlame.Mapping.IO.Wz;
using SharpFlame.Settings;

namespace SharpFlame.Gui.Actions
{
    public class LoadMap : Command
    {
        private readonly ILogger logger;

        static IEnumerable<IFileDialogFilter> GetFilters(string allFormatDescription)
        {
            yield return new FileDialogFilter(allFormatDescription, "fmap", "wz", "game", "lnd");
            yield return new FileDialogFilter("FMAP Files", "fmap");
            yield return new FileDialogFilter("WZ Files", "wz");
            yield return new FileDialogFilter("Game Files", "game");
            yield return new FileDialogFilter("LND Files", "lnd");
        }

        [Inject]
        internal SettingsManager Settings { get; set; }

        [Inject]
        internal MainMapView MainMapView { get; set; }

        [Inject]
        internal IKernel Kernel { get; set; }

        public LoadMap(ILoggerFactory logFactory)
        {
            logger = logFactory.GetCurrentClassLogger();

            ID = "loadMap";
            MenuText = "&Open";
            ToolBarText = "Open";
        }

        public override void OnExecuted(EventArgs e)
        {
            base.OnExecuted(e);
            // show the about dialog

            var dialog = new OpenFileDialog();
            
            dialog.Directory = new Uri( Settings.OpenPath );
            dialog.Filters = GetFilters("All Supported Formats");

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

                var map = Kernel.Get<Map>();
                IIOLoader loader;
                switch(Path.GetExtension(dialog.FileName))
                {
                case ".fmap":
                    loader = new FMapLoader(map);
                    break;
                case ".wz":
                    loader = new WzLoader(map);
                    break;
                case ".game":
                    loader = new GameLoader(map);
                    break;
                case ".lnd":
                    loader = new LNDLoader(map);
                    break;
                default:
                    returnResult = new Result(string.Format("Loading \"{0}\"", Path.GetExtension(dialog.FileName)), false);
                    returnResult.ProblemAdd(string.Format("UNKNOWN File type: can\'t load file \"{0}\"", dialog.FileName));
                    App.StatusDialog = new Dialogs.Status(returnResult);
                    App.StatusDialog.Show();
                    logger.Error("Loading \"{0}\", UNKNOWN File type: can\'t load file \"{1}\"", Path.GetExtension(dialog.FileName), dialog.FileName);
                    return;
                }

                var loadResult = loader.Load(dialog.FileName);
                if(loadResult.HasProblems || loadResult.HasWarnings)
                {
                    App.StatusDialog = new Dialogs.Status(loadResult);
                    App.StatusDialog.Show();
                }

                if(!loadResult.HasProblems)
                {
                    map.InitializeUserInput();
                    map.Update(); // TODO: Remove me once map drawing works.
                    MainMapView.MainMap = map;
                }
            }
        }
    }
}

