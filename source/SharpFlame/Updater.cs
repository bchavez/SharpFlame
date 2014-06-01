#region License
/*
The MIT License (MIT)

Copyright (c) 2014 The SharpFlame Authors.

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
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Eto;
using Ninject;
using Ninject.Extensions.Logging;
using NAppUpdate.Framework;
using NAppUpdate.Framework.Common;
using NAppUpdate.Framework.Sources;

namespace SharpFlame
{
    public class Updater
    {
        private ILogger logger;
        private UpdateManager updManager;

        private bool ready;

        public Updater(ILoggerFactory logFactory) {
            logger = logFactory.GetCurrentClassLogger ();

            try
            {
                updManager = UpdateManager.Instance;
                if (Generator.Current.IsGtk) {
                    updManager.UpdateSource = new SimpleWebSource("http://rene.kistl.at/downloads/SharpFlame/Gtk/UpdateFeed.xml");
                } else if (Generator.Current.IsWinForms) {
                    updManager.UpdateSource = new SimpleWebSource("http://rene.kistl.at/downloads/SharpFlame/Winforms/UpdateFeed.xml");
                } else { 
                    ready = false;
                    return;
                }
                updManager.Config.TempFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SME-Rauchfrei");

                // If you don't call this method, the updater.exe will continually attempt to connect the named pipe and get stuck.
                // Therefore you should always implement this method call.               
                updManager.ReinstateIfRestarted();

                if (updManager.State == UpdateManager.UpdateProcessState.Checked ||
                    updManager.State == UpdateManager.UpdateProcessState.AfterRestart ||
                    updManager.State == UpdateManager.UpdateProcessState.AppliedSuccessfully)
                { 
                    updManager.CleanUp ();
                }
            }
            catch (Exception ex)
            {
                logger.Error (ex, "Got an Exception while configureing the Updater.");
                ready = false;
            }

            ready = true;
        }

        public Task<int> CheckForUpdatesAsync()
        {
            var task = new Task<int>(() => { 
                if (!ready)
                {
                    return 0;
                }

                Thread.Sleep(1000);

                logger.Info ("Checking for Updates.");
                try {
                    updManager.CheckForUpdates();
                } 
                catch (Exception ex)
                {
                    logger.Error (ex, "Got an Exception while checking for updates.");
                    return 0;
                }

                logger.Info ("Found \"{0}\" Updates.", updManager.UpdatesAvailable);
                logUpdateManagerState ();

                return updManager.UpdatesAvailable;
            });

            return task;
        }

        public Task<bool> PrepareUpdatesAsync()
        {
            var task = new Task<bool>(() =>
            {
                if (!ready)
                {
                    return false;
                }

                logger.Info ("Downloading updates.");
                try
                {
                    updManager.PrepareUpdates ();
                } catch (Exception ex)
                {
                    logger.Error (ex, "Got an Exception while preparing updates.");
                    return false;
                }
                logger.Info ("Download done.");

                return true;
            });

            return task;
        }

        public void DoUpdate()
        {
            // This is a synchronous method by design, make sure to save all user work before calling
            // it as it might restart your application
            logger.Info ("Applying the update.");
            try
            {
                updManager.ApplyUpdates (true, true, true);
            } catch (Exception ex)
            {
                logger.Error (ex, "Got an Exception while trying to install updates.");
            }

            if (UpdateManager.Instance.State == UpdateManager.UpdateProcessState.RollbackRequired)
                UpdateManager.Instance.RollbackUpdates ();
        }

        private void logUpdateManagerState()
        {
            logger.Debug("UpdateManager.Instance.State: {0}", UpdateManager.Instance.State);
        }
    }
}
