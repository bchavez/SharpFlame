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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using Appccelerate.EventBroker;
using Appccelerate.EventBroker.Handlers;
using Eto;
using Eto.Forms;
using Eto.Gl;
using Ninject;
using Ninject.Extensions.Logging;
using OpenTK;
using SharpFlame.Core;
using SharpFlame.Gui.Forms;
using SharpFlame.Gui.Controls;
using SharpFlame.Gui.Infrastructure;
using SharpFlame.Old;
using SharpFlame.Old.Domain.ObjData;
using SharpFlame.Old.Graphics.OpenGL;
using SharpFlame.Old.Settings;
using Size = Eto.Drawing.Size;
using Font = System.Drawing.Font;
using FontStyle = System.Drawing.FontStyle;

namespace SharpFlame.Gui
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

        private Result initializeResult = new Result("Startup result", false);

        [Inject]
        public SharpFlameApplication(IKernel kernel, Generator generator, ILoggerFactory logFactory)
            : base(generator)
        {
            kernel.Inject(this); //inject properties also, not just constructor.

            // TODO: Remove me once everthing is inectable.
            App.Kernel = kernel;
            App.Settings = this.Settings;
            App.KeyboardManager = this.KeyboardManager;
            App.MapViewGlSurface = this.GlMapView;

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

            GlTexturesView.Initialized += OnGLControlInitialized;

            SetupEventHandlers();

            initializeResult.Add(Settings.Load(App.SettingsPath));
            // initializeResult.Add (SettingsManager.SettingsLoad (ref SettingsManager.InitializeSettings));           
        }

        public override void OnInitialized(EventArgs e)
        {
            this.MainForm = App.Kernel.Get<MainForm>();

            base.OnInitialized(e);

            // show the main form
            MainForm.Show();

            if( Settings.ShowOptionsAtStartup )
            {
                new Dialogs.Settings().Show();
            }
        }

        public override void OnTerminating(CancelEventArgs e)
        {
            base.OnTerminating(e);

            var result = MessageBox.Show(MainForm, "Are you sure you want to quit?", MessageBoxButtons.YesNo, MessageBoxType.Question);
            if( result == DialogResult.No )
                e.Cancel = true;
        }

        /// <summary>
        /// Ons the GL control initialized.
        /// </summary>
        /// <param name="o">Not used.</param>
        /// <param name="e">Not used.</param>
        private void OnGLControlInitialized(object o, EventArgs e)
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

            // Make the GL Font.
            MakeGlFont();

            if( initializeResult.HasProblems )
            {
                logger.Error(initializeResult.ToString());
                new Dialogs.Status(initializeResult).Show();
            }
            else if( initializeResult.HasWarnings )
            {
                logger.Warn(initializeResult.ToString());
                new Dialogs.Status(initializeResult).Show();
            }
            else
            {
                logger.Debug(initializeResult.ToString());
            }
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
                                    new Dialogs.Status(result).Show();
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
                            new Dialogs.Status(result).Show();
                        }
                    }
                    catch( Exception ex )
                    {
                        logger.Error(ex, "Got an Exception while loading object data.");
                    }
                };
        }

        private void MakeGlFont()
        {
            if(!GlTexturesView.IsInitialized)
            {
                return;
            }

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
    }
}