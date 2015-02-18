using System;
using System.ComponentModel;
using Appccelerate.EventBroker;
using Eto;
using Eto.Forms;
using Ninject;
using Ninject.Extensions.Logging;
using OpenTK;
using SharpFlame.Core;
using SharpFlame.Gui.Forms;
using SharpFlame.Settings;
using Size = Eto.Drawing.Size;

namespace SharpFlame
{
    public class SharpFlameApplication : Application
    {
        private ILogger logger;

        [Inject]
        internal IEventBroker EventBroker { get; set; }

        [Inject]
        internal SettingsManager Settings { get; set; }

        [Inject]
        internal KeyboardManager KeyboardManager { get; set; }

        [Inject]
        internal IKernel Kernel { get; set; }

        [Inject]
        internal ILoggerFactory LogFactory { get; set; }

        public static Result InitializeResult = new Result("Startup result", false);

        protected override void OnInitialized(EventArgs e)
        {
            try
            {
                Toolkit.Init();
            }
            catch( Exception ex )
            {
                logger.Error(ex, "Got an exception while initializing OpenTK");
                // initializeResult.ProblemAdd (string.Format("Failure while loading opentk, error was: {0}", ex.Message));
                Instance.Quit();
            }

            // TODO: Remove me once everthing is inectable.
            App.Kernel = this.Kernel;
            App.SettingsManager = this.Settings;
            App.KeyboardManager = this.KeyboardManager;
            App.Random = new Random();

            logger = LogFactory.GetCurrentClassLogger();

            // Allows manual Button size on GTK2.
            Button.DefaultSize = new Size(1, 1);

            Name = string.Format("No Map - {0} {1}", Constants.ProgramName, Constants.ProgramVersion());
            Style = "application";

            // Run this before everything else.
            App.Initalize();

            // Uncomment me to debug the EventBroker.
            //            #if DEBUG
            //            EventBroker.AddExtension(new SharpFlame.Core.Extensions.EventBrokerLogExtension());
            //            #endif

            App.SetProgramSubDirs();

            InitializeResult.Add(Settings.Load(App.SettingsPath));

            this.MainForm = new MainForm();

            base.OnInitialized(e);

            // show the main form
            MainForm.Show();

            if( Settings.ShowOptionsAtStartup )
            {
                Kernel.Get<Gui.Dialogs.Settings>().Show();
            }
        }

        protected override void OnTerminating(CancelEventArgs e)
        {
            base.OnTerminating(e);

            var result = MessageBox.Show(MainForm, "Are you sure you want to quit?", MessageBoxButtons.YesNo, MessageBoxType.Question);
            if( result == DialogResult.No )
                e.Cancel = true;
        }
    }
}