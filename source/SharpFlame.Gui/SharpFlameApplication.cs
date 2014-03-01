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
using Eto;
using Eto.Drawing;
using Eto.Forms;
using NLog;
using OpenTK;
using SharpFlame.Core;
using SharpFlame.Gui.Forms;
using SharpFlame.Gui.Controls;
using SharpFlame.Old;
using SharpFlame.Old.AppSettings;

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

			Name = string.Format ("No Map - {0} {1}", Constants.ProgramName, Constants.ProgramVersionNumber);
			Style = "application";

            // Run this before everything else.
            App.Initalize ();

            App.SetProgramSubDirs();

            SettingsManager.CreateSettingOptions();
            KeyboardManager.CreateControls(); //needed to load key control settings

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

            var SettingsLoadResult = SettingsManager.SettingsLoad(ref SettingsManager.InitializeSettings);                  
            initializeResult.Add(SettingsLoadResult);

            GlTexturesView = new GLSurface ();
            GlTexturesView.Initialized += onGLControlInitialized;

            GlMapView = new GLSurface ();
		}

        /// <summary>
        /// Ons the GL control initialized.
        /// </summary>
        /// <param name="o">Not used.</param>
        /// <param name="e">Not used.</param>
        void onGLControlInitialized(object o, EventArgs e) {
            GlTexturesView.MakeCurrent ();

            var tilesetsList = (List<string>)SettingsManager.Settings.GetValue(SettingsManager.SettingTilesetDirectories);
            foreach (var path in tilesetsList) {
                if (path != null && path != "") {
                    initializeResult.Add (App.LoadTilesets (PathUtil.EndWithPathSeperator (path)));
                }
            }

            App.UnitLabelFont = GlTexturesView.CreateGLFont (App.UnitLabelBaseFont);

            if (initializeResult.HasProblems)
            {
                logger.Error (initializeResult.ToString ());
                var statusDialog = new Dialogs.Status (initializeResult);
                statusDialog.Show();
            } else if (initializeResult.HasWarnings)
            {
                logger.Warn (initializeResult.ToString ());
                var statusDialog = new Dialogs.Status (initializeResult);
                statusDialog.Show();
            } else
            {
                logger.Debug (initializeResult.ToString ());
            }           
        }

		public override void OnInitialized(EventArgs e)
		{
			MainForm = new MainForm(this);

			base.OnInitialized(e);          

            SettingsManager.UpdateSettings(SettingsManager.InitializeSettings);
            SettingsManager.InitializeSettings = null;

			// show the main form
			MainForm.Show();
		}

		public override void OnTerminating(System.ComponentModel.CancelEventArgs e)
		{
			base.OnTerminating(e);

			var result = MessageBox.Show(MainForm, "Are you sure you want to quit?", MessageBoxButtons.YesNo, MessageBoxType.Question);
			if (result == DialogResult.No)
				e.Cancel = true;
		}
	}
}

