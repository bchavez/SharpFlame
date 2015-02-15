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
        private readonly ILogger logger;

        [Inject]
        internal IEventBroker EventBroker { get; set; }

        [Inject]
        internal SettingsManager Settings { get; set; }

        [Inject]
        internal KeyboardManager KeyboardManager { get; set; }

        private readonly IKernel kernel;

        public static Result InitializeResult = new Result("Startup result", false);

        [Inject]
        public SharpFlameApplication(IKernel myKernel, Platform platform, ILoggerFactory logFactory)
            : base(platform)
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
				
			myKernel.Inject(this); //inject properties also, not just constructor.
			kernel = myKernel;

			// TODO: Remove me once everthing is inectable.
			App.Kernel = myKernel;
			App.SettingsManager = this.Settings;
			App.KeyboardManager = this.KeyboardManager;
			App.Random = new Random();

            logger = logFactory.GetCurrentClassLogger();

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

            EventBroker.Register(this.Settings);
            EventBroker.Register(this.KeyboardManager);

            #if DEBUG
            var keylogger = kernel.Get<Keylogger>();
            EventBroker.Register(keylogger);
            #endif

            App.SetProgramSubDirs();

            InitializeResult.Add(Settings.Load(App.SettingsPath));
        }

        protected override void OnInitialized(EventArgs e)
        {
            this.MainForm = kernel.Get<MainForm>();

            base.OnInitialized(e);

            // show the main form
            MainForm.Show();

            if( Settings.ShowOptionsAtStartup )
            {
                kernel.Get<Gui.Dialogs.Settings>().Show();
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