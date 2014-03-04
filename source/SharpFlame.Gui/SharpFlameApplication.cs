#region License
// /*
// The MIT License (MIT)
//
// Copyright (c) 2013-2014 The SharpFlame Authors.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// */
#endregion

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using Eto;
using Eto.Drawing;
using Eto.Forms;
using NLog;
using OpenTK;
using SharpFlame.Core;
using SharpFlame.Gui.Forms;
using SharpFlame.Gui.Controls;
using SharpFlame.Old;
using SharpFlame.Old.Domain.ObjData;
using Size = Eto.Drawing.Size;
using Font = System.Drawing.Font;
using FontStyle = System.Drawing.FontStyle;

namespace SharpFlame.Gui
{
	public class SharpFlameApplication : Application
	{
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public readonly GLSurface GlTexturesView;
        public readonly GLSurface GlMapView;

        private Result initializeResult = new Result("Startup result", false);

		public SharpFlameApplication(Generator generator)
			: base(generator)
		{
			// Allows manual Button size on GTK2.
			Button.DefaultSize = new Size (1, 1);

			Name = string.Format ("No Map - {0} {1}", Constants.ProgramName, Constants.ProgramVersion());
			Style = "application";

            // Run this before everything else.
            App.Initalize ();

            App.SetProgramSubDirs();

            try
            {
                Toolkit.Init();
            }
            catch (Exception ex)
            {
                logger.ErrorException ("Got an exception while initalizing OpenTK", ex);
                // initializeResult.ProblemAdd (string.Format("Failure while loading opentk, error was: {0}", ex.Message));
                Instance.Quit();
            }

            // Set them up before you call settingsBindings().
            GlTexturesView = new GLSurface ();
            GlMapView = new GLSurface ();
            GlTexturesView.Initialized += onGLControlInitialized;           

            settingsBindings ();

            initializeResult.Add (App.Settings.Load (App.SettingsPath));
            // initializeResult.Add (SettingsManager.SettingsLoad (ref SettingsManager.InitializeSettings));           
		}          

		public override void OnInitialized(EventArgs e)
		{
			MainForm = new MainForm(this);

			base.OnInitialized(e);          

			// show the main form
			MainForm.Show();

            if (App.Settings.ShowOptionsAtStartup)
            {
                new Dialogs.Settings ().Show ();
            }
		}

		public override void OnTerminating(System.ComponentModel.CancelEventArgs e)
		{
			base.OnTerminating(e);

			var result = MessageBox.Show(MainForm, "Are you sure you want to quit?", MessageBoxButtons.YesNo, MessageBoxType.Question);
			if (result == DialogResult.No)
				e.Cancel = true;
		}

        /// <summary>
        /// Ons the GL control initialized.
        /// </summary>
        /// <param name="o">Not used.</param>
        /// <param name="e">Not used.</param>
        void onGLControlInitialized(object o, EventArgs e) {
            GlTexturesView.MakeCurrent ();

            // Load tileset directories.
            foreach (var path in App.Settings.TilesetDirectories) {
                if (path != null && path != "") {
                    initializeResult.Add (App.LoadTilesets (path));
                }
            }

            // Load Object Data.
            foreach (var path in App.Settings.ObjectDataDirectories) {
                if (path != null && path != "") {
                    initializeResult.Add(App.ObjectData.LoadDirectory(path));
                }
            }

            // Make the GL Font.
            makeGlFont ();

            if (initializeResult.HasProblems)
            {
                logger.Error (initializeResult.ToString ());
                new Dialogs.Status (initializeResult).Show();
            } else if (initializeResult.HasWarnings)
            {
                logger.Warn (initializeResult.ToString ());
                new Dialogs.Status (initializeResult).Show();
            } else
            {
                logger.Debug (initializeResult.ToString ());
            }           
        }

        void settingsBindings() {
            App.Settings.PropertyChanged += (object sender, PropertyChangedEventArgs e) => {
                #if DEBUG
                Console.WriteLine("Setting {0} changed ", e.PropertyName);
                #endif

                if (e.PropertyName.StartsWith("Font") && GlTexturesView.IsInitialized) {
                    makeGlFont();
                }
            };

            App.Settings.TilesetDirectories.CollectionChanged += (sender, e) => 
            {
                if (!GlTexturesView.IsInitialized) {
                    return;
                }

                try {
                    if (e.Action == NotifyCollectionChangedAction.Add) {
                        foreach (var item in e.NewItems) {
                            var result = App.LoadTilesets ((string)item);
                            if (result.HasProblems || result.HasWarnings) {
                                new Dialogs.Status (result).Show();
                                App.Settings.TilesetDirectories.Remove((string)item);
                            }
                        }
                    } else if (e.Action == NotifyCollectionChangedAction.Remove) {
                        foreach (var item in e.OldItems) {
                            var found = App.Tilesets.Where(w => w.Directory.StartsWith((string)item)).ToList();
                            foreach (var foundItem in found) {
                                App.Tilesets.Remove(foundItem);
                            }
                        }
                    }
                } catch (Exception ex) {
                    logger.ErrorException("Got an Exception", ex);
                }
            };

            App.Settings.ObjectDataDirectories.CollectionChanged += (sender, e) =>
            {
                if (!GlTexturesView.IsInitialized) {
                    return;
                }

                try {
                    var result = new Result("Reloading object data.", false);
                    if (e.Action == NotifyCollectionChangedAction.Add) {
                        // Just reload Object Data.
                        App.ObjectData = new ObjectData();
                        foreach (var path in App.Settings.ObjectDataDirectories) {
                            if (path != null && path != "") {
                                result.Add(App.ObjectData.LoadDirectory(path));
                            }
                        }
                    } else if (e.Action == NotifyCollectionChangedAction.Remove) {
                        // Just reload Object Data.
                        App.ObjectData = new ObjectData();
                        foreach (var path in App.Settings.ObjectDataDirectories) {
                            if (path != null && path != "") {
                                result.Add(App.ObjectData.LoadDirectory(path));
                            }
                        }
                        // Need to send an objectchanged event as LoadDirectory may never occurs.
                        App.OnObjectDataChanged(this, EventArgs.Empty);
                    }

                    if (result.HasProblems || result.HasWarnings) {
                        new Dialogs.Status (result).Show();
                    }
                } catch (Exception ex) {
                    logger.ErrorException("Got an Exception", ex);
                }
            };
        }

        void makeGlFont()
        {
            var style = FontStyle.Regular;
            if ( App.Settings.FontBold )
            {
                style = style | FontStyle.Bold;
            }
            if ( App.Settings.FontItalic )
            {
                style = style | FontStyle.Italic;
            }
            App.UnitLabelFont = GlTexturesView.CreateGLFont (new Font(App.Settings.FontFamily, App.Settings.FontSize, style, GraphicsUnit.Point));
        }
	}
}

