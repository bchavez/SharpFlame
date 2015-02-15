using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using Eto.Forms;
using Ninject;
using Ninject.Extensions.Logging;
using SharpFlame.Core;
using SharpFlame.Core.Extensions;
using SharpFlame.Gui.Sections;
using SharpFlame;
using SharpFlame.Mapping;
using SharpFlame.Mapping.IO;
using SharpFlame.Mapping.IO.FMap;
using SharpFlame.Mapping.IO.LND;
using SharpFlame.Mapping.IO.Wz;
using SharpFlame.Settings;

namespace SharpFlame.Gui.Actions
{
    public class LoadMap : Command
    {
        private readonly ILogger logger;

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

                var map = Kernel.Get<Map>();
                IIOLoader loader;
                switch(Path.GetExtension(dialog.FileName).ToLower())
                {
                case ".fmap":
                    loader = Kernel.Get<FMapLoader>();
                    break;
                case ".wz":
                    loader = Kernel.Get<WzLoader>();
                    break;
                case ".game":
                    loader = Kernel.Get<GameLoader>();
                    break;
                case ".lnd":
                    loader = Kernel.Get<LNDLoader>();
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
                    App.StatusDialog = new Dialogs.Status(loadResult.ToResult());
                    App.StatusDialog.Show();
                }

                if(!loadResult.HasProblems)
                {
                    loadResult.Value.PathInfo = new PathInfo(dialog.FileName, true);
                    MainMapView.MainMap = loadResult.Value;
                } else
                {
                    loadResult.Value.Deallocate();
                }
            }
        }
    }
}

