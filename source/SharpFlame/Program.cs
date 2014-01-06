using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Microsoft.VisualBasic.ApplicationServices;
using My.Resources;
using OpenTK;
using OpenTK.Graphics;
using SharpFlame.AppSettings;


namespace SharpFlame
{
    public sealed class Program
    {
        public static Timer InitializeDelay;
        public static clsResult InitializeResult = new clsResult("Startup result");

        public static frmMain frmMainInstance;
        public static frmGenerator frmGeneratorInstance;
        public static frmOptions frmOptionsInstance;

        public static GLControl OpenGL1;
        public static GLControl OpenGL2;

        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();

            App.PlatformPathSeparator = Path.DirectorySeparatorChar;
            App.SetProgramSubDirs();

            SettingsManager.CreateSettingOptions();
            KeyboardManager.CreateControls(); //needed to load key control settings
            clsResult SettingsLoadResult = SettingsManager.Settings_Load(ref SettingsManager.InitializeSettings);
            InitializeResult.Add(SettingsLoadResult);

            OpenGL1 =
                new GLControl(new GraphicsMode(new ColorFormat(SettingsManager.InitializeSettings.MapViewBPP), SettingsManager.InitializeSettings.MapViewDepth, 0));
            OpenGL2 =
                new GLControl(new GraphicsMode(new ColorFormat(SettingsManager.InitializeSettings.TextureViewBPP), SettingsManager.InitializeSettings.TextureViewDepth,
                    0));

            while ( OpenGL1.Context == null || OpenGL2.Context == null )
            {
                //todo, why is this needed
            }

            frmMainInstance = new frmMain();
            frmMainInstance.FormClosing += frmMainInstance.frmMain_FormClosing;
            frmMainInstance.DragEnter += frmMainInstance.OpenGL_DragEnter;
            frmMainInstance.DragDrop += frmMainInstance.OpenGL_DragDrop;

            try
            {
                App.ProgramIcon = Resources.flaME;
            }
            catch ( Exception ex )
            {
                InitializeResult.WarningAdd(Constants.ProgramName + " icon is missing: " + ex.Message);
            }
            frmMainInstance.Icon = App.ProgramIcon;
            frmGeneratorInstance.Icon = App.ProgramIcon;

            InitializeDelay = new Timer();
            InitializeDelay.Tick += frmMainInstance.Initialize;
            InitializeDelay.Interval = 50;
            InitializeDelay.Enabled = true;

            while ( !App.ProgramInitializeFinished )
            {
                System.Threading.Thread.Sleep(50);
                Application.DoEvents();
            }

            Application.Run(frmMainInstance);
        }
    }
}