#region License
/*
The MIT License (MIT)

Copyright (c) 2013-2014 The SharpFlame Authors.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
#endregion

using System;
using System.Collections.Generic;
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
        public SettingsManager Settings { get; set; }

        [Inject]
        public MainMapView MainMapView { get; set; }

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
            #if Portable
            dialog.Directory = new UriBuilder { Scheme = Uri.UriSchemeFile, Path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Settings.OpenPath) }.Uri;
            #else
            dialog.Directory = new UriBuilder { Scheme = Uri.UriSchemeFile, Path = Settings.OpenPath }.Uri;
            #endif
            dialog.Filters = GetFilters("All Supported Formats");

            var result = dialog.ShowDialog(Application.Instance.MainForm);
            if(result == DialogResult.Ok)
            {
                // Set Openpath to the directory of the selected file and save the settings.
                #if Portable 
                Settings.OpenPath = Uri.UnescapeDataString(dialog.Directory.MakeRelativeUriToBinPath().ToString());
                #else
				Settings.OpenPath = Uri.UnescapeDataString(dialog.Directory.AbsolutePath);
                #endif
                var returnResult = Settings.Save(App.SettingsPath);
                if(returnResult.HasProblems)
                {
                    new Dialogs.Status(returnResult).Show();
                }

                var map = new Map();
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
                    logger.Error("Loading \"{0}\", UNKNOWN File type: can\'t load file \"{1}\"", Path.GetExtension(dialog.FileName), dialog.FileName);
                    return;
                }

                loader.Load(dialog.FileName);
                map.InitializeUserInput();

                MainMapView.Map = map;
            }
        }
    }
}

