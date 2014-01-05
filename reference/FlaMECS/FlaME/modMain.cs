namespace FlaME
{
    using FlaME.My;
    using Microsoft.VisualBasic.CompilerServices;
    using OpenTK;
    using OpenTK.Graphics;
    using System;
    using System.Drawing;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Windows.Forms;

    [StandardModule]
    public sealed class modMain
    {
        public static frmGenerator frmGeneratorInstance;
        public static frmMain frmMainInstance;
        public static frmOptions frmOptionsInstance;
        public static System.Windows.Forms.Timer InitializeDelay;
        public static clsResult InitializeResult = new clsResult("Startup result");
        public static GLControl OpenGL1;
        public static GLControl OpenGL2;

        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining), STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            modProgram.PlatformPathSeparator = System.IO.Path.DirectorySeparatorChar;
            modProgram.SetProgramSubDirs();
            modSettings.CreateSettingOptions();
            modControls.CreateControls();
            clsResult resultToAdd = modSettings.Settings_Load(ref modSettings.InitializeSettings);
            InitializeResult.Add(resultToAdd);
            ColorFormat color = new ColorFormat(modSettings.InitializeSettings.MapViewBPP);
            OpenGL1 = new GLControl(new GraphicsMode(color, modSettings.InitializeSettings.MapViewDepth, 0));
            color = new ColorFormat(modSettings.InitializeSettings.TextureViewBPP);
            OpenGL2 = new GLControl(new GraphicsMode(color, modSettings.InitializeSettings.TextureViewDepth, 0));
            while ((OpenGL1.Context == null) | (OpenGL2.Context == null))
            {
            }
            frmMainInstance = new frmMain();
            try
            {
                modProgram.ProgramIcon = new Icon(MyProject.Application.Info.DirectoryPath + Conversions.ToString(modProgram.PlatformPathSeparator) + "flaME.ico");
            }
            catch (Exception exception1)
            {
                ProjectData.SetProjectError(exception1);
                Exception exception = exception1;
                InitializeResult.WarningAdd("FlaME icon is missing: " + exception.Message);
                ProjectData.ClearProjectError();
            }
            frmMainInstance.Icon = modProgram.ProgramIcon;
            frmGeneratorInstance.Icon = modProgram.ProgramIcon;
            InitializeDelay = new System.Windows.Forms.Timer();
            InitializeDelay.Tick += new EventHandler(frmMainInstance.Initialize);
            InitializeDelay.Interval = 50;
            InitializeDelay.Enabled = true;
            while (!modProgram.ProgramInitializeFinished)
            {
                Thread.Sleep(50);
                Application.DoEvents();
            }
            Application.Run(frmMainInstance);
        }
    }
}

