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
            var task = Task.Factory.StartNew(() => { 
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
            var task = Task.Factory.StartNew(() =>
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
