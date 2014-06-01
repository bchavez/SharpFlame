

using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using Appccelerate.EventBroker;
using Appccelerate.EventBroker.Handlers;
using Eto;
using Eto.Forms;
using Eto.Gl;
using Ninject;
using Ninject.Extensions.Logging;
using OpenTK;
using SharpFlame.Core;
using SharpFlame.Generators;
using SharpFlame.Gui;
using SharpFlame.Gui.Forms;
using SharpFlame.Gui.Sections;
using SharpFlame.Infrastructure;
using SharpFlame;
using SharpFlame.Domain.ObjData;
using SharpFlame.Graphics.OpenGL;
using SharpFlame.Maths;
using SharpFlame.Painters;
using SharpFlame.Settings;
using SharpFlame.Util;
using Size = Eto.Drawing.Size;
using Font = System.Drawing.Font;
using FontStyle = System.Drawing.FontStyle;

namespace SharpFlame
{
    public class SharpFlameApplication : Application
    {
        private readonly ILogger logger;

        [Inject, Named(NamedBinding.TextureView)]
        internal GLSurface GlTexturesView { get; set; }

        [Inject, Named(NamedBinding.MapView)]
        internal GLSurface GlMapView { get; set; }

        [Inject]
        internal IEventBroker EventBroker { get; set; }

        [Inject]
        internal SettingsManager Settings { get; set; }

        [Inject]
        internal KeyboardManager KeyboardManager { get; set; }

        private readonly IKernel kernel;

        private Result initializeResult = new Result("Startup result", false);

        [Inject]
        public SharpFlameApplication(IKernel myKernel, Generator generator, ILoggerFactory logFactory)
            : base(generator)
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

            // Make a SynchronizationContext for System.Threading.Tasks.
            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());

			// TODO: Remove me once everthing is inectable.
			App.Kernel = myKernel;
			App.SettingsManager = this.Settings;
			App.KeyboardManager = this.KeyboardManager;
			App.MapViewGlSurface = this.GlMapView;
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


            GlTexturesView.Initialized += TextureView_OnGLControlInitialized;
            GlMapView.Initialized += GlMapView_Initialized;

            SetupEventHandlers();

            initializeResult.Add(Settings.Load(App.SettingsPath));
        }

        public override void OnInitialized(EventArgs e)
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

        public override void OnTerminating(CancelEventArgs e)
        {
            base.OnTerminating(e);

            var result = MessageBox.Show(MainForm, "Are you sure you want to quit?", MessageBoxButtons.YesNo, MessageBoxType.Question);
            if( result == DialogResult.No )
                e.Cancel = true;
        }

        void GlMapView_Initialized(object sender, EventArgs e)
        {
            this.GlMapView.MakeCurrent();
            // Set Vision radius
            App.VisionRadius_2E = 10;
            App.VisionRadius_2E_Changed();

            Matrix3DMath.MatrixSetToPY(App.SunAngleMatrix, new Angles.AnglePY(-22.5D * MathUtil.RadOf1Deg, 157.5D * MathUtil.RadOf1Deg));

            // Make the GL Font.
            MakeGlFont();
        }

        /// <summary>
        /// Ons the GL control initialized.
        /// </summary>
        /// <param name="o">Not used.</param>
        /// <param name="e">Not used.</param>
        private void TextureView_OnGLControlInitialized(object o, EventArgs e)
        {
            GlTexturesView.MakeCurrent();

            // Load tileset directories.
            foreach( var path in Settings.TilesetDirectories )
            {
                if( !string.IsNullOrEmpty(path) )
                {
                    initializeResult.Add(App.LoadTilesets(path));
                }
            }

            // Load Object Data.
            foreach( var path in Settings.ObjectDataDirectories )
            {
                if( !string.IsNullOrEmpty(path) )
                {
                    initializeResult.Add(App.ObjectData.LoadDirectory(path));
                }
            }

            DefaultGenerator.CreateGeneratorTilesets();

            // Create Painters for the known tilesets.
            PainterFactory.CreatePainterArizona();
            PainterFactory.CreatePainterUrban();
            PainterFactory.CreatePainterRockies();

            // Show initialize problems.
//            if( initializeResult.HasProblems )
//            {
//                logger.Error(initializeResult.ToString());
//                App.StatusDialog = new Gui.Dialogs.Status(initializeResult);
//                App.StatusDialog.Show();
//            }
//            else if( initializeResult.HasWarnings )
//            {
//                logger.Warn(initializeResult.ToString());
//                App.StatusDialog = new Gui.Dialogs.Status(initializeResult);
//                App.StatusDialog.Show();
//            }
//            else
//            {
//                logger.Debug(initializeResult.ToString());
//            }
        }

        private void MakeGlFont()
        {
            if(!GlMapView.IsInitialized)
            {
                return;
            }
            GlMapView.MakeCurrent();

            var style = FontStyle.Regular;
            if( Settings.FontBold )
            {
                style = style | FontStyle.Bold;
            }
            if( Settings.FontItalic )
            {
                style = style | FontStyle.Italic;
            }
            App.UnitLabelFont = new GLFont(new System.Drawing.Font(Settings.FontFamily, Settings.FontSize, style, System.Drawing.GraphicsUnit.Pixel));
        }

        private void SetupEventHandlers()
        {
            Settings.PropertyChanged += (object sender, PropertyChangedEventArgs e) =>
            {
                #if DEBUG
                Console.WriteLine("Setting {0} changed ", e.PropertyName);
                #endif

                if(e.PropertyName.StartsWith("Font"))
                {
                    MakeGlFont();
                }
            };
                
            Settings.TilesetDirectories.CollectionChanged += (sender, e) =>
                {
                    if( !GlTexturesView.IsInitialized )
                    {
                        return;
                    }

                    try
                    {
                        if( e.Action == NotifyCollectionChangedAction.Add )
                        {
                            foreach( var item in e.NewItems )
                            {
                                var result = App.LoadTilesets((string)item);
                                if( result.HasProblems || result.HasWarnings )
                                {
                                    App.StatusDialog = new Gui.Dialogs.Status(result);
                                    App.StatusDialog.Show();
                                    Settings.TilesetDirectories.Remove((string)item);
                                }
                            }
                        }
                        else if( e.Action == NotifyCollectionChangedAction.Remove )
                        {
                            foreach( var item in e.OldItems )
                            {
                                var found = App.Tilesets.Where(w => w.Directory.StartsWith((string)item)).ToList();
                                foreach( var foundItem in found )
                                {
                                    App.Tilesets.Remove(foundItem);
                                }
                            }
                        }
                    }
                    catch( Exception ex )
                    {
                        logger.Error(ex, "Got an exception while loading tilesets.");
                    }
                };

            Settings.ObjectDataDirectories.CollectionChanged += (sender, e) =>
                {
                    if( !GlTexturesView.IsInitialized )
                    {
                        return;
                    }

                    try
                    {
                        var result = new Result("Reloading object data.", false);
                        if( e.Action == NotifyCollectionChangedAction.Add )
                        {
                            // Just reload Object Data.
                            App.ObjectData = new ObjectData();
                            foreach( var path in Settings.ObjectDataDirectories )
                            {
                                if( path != null && path != "" )
                                {
                                    result.Add(App.ObjectData.LoadDirectory(path));
                                }
                            }
                        }
                        else if( e.Action == NotifyCollectionChangedAction.Remove )
                        {
                            // Just reload Object Data.
                            App.ObjectData = new ObjectData();
                            foreach( var path in Settings.ObjectDataDirectories )
                            {
                                if( path != null && path != "" )
                                {
                                    result.Add(App.ObjectData.LoadDirectory(path));
                                }
                            }
                            // Need to send an objectchanged event as LoadDirectory may never occurs.
                            App.OnObjectDataChanged(this, EventArgs.Empty);
                        }

                        if( result.HasProblems || result.HasWarnings )
                        {
                            App.StatusDialog = new Gui.Dialogs.Status(result);
                            App.StatusDialog.Show();
                        }
                    }
                    catch( Exception ex )
                    {
                        logger.Error(ex, "Got an Exception while loading object data.");
                    }
                };
        }
    }
}