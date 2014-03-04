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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using bbv.Common.EventBroker;
using Eto.Forms;
using Newtonsoft.Json;
using NLog;
using OpenTK;
using SharpFlame.Core;
using SharpFlame.Core.Domain.Colors;

namespace SharpFlame.Old.Settings
{
    public class SettingsManager: INotifyPropertyChanged
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #region Properties
        public bool AutoSaveEnabled {  get; set; }
        public bool AutoSaveCompress { get; set; }
        public UInt32 AutoSaveMinIntervalSeconds { get; set; }
        public UInt32 AutoSaveMinChanges { get; set; }
        public UInt32 UndoLimit { get; set; }
        public bool ShowOptionsAtStartup { get; set; }
        public bool DirectPointer { get; set; }
        private FontFamily fontFamily;
        public FontFamily FontFamily { 
            get { return fontFamily; }
            set { SetField(ref fontFamily, value, () => FontFamily); }
        }
        private bool fontBold;
        public bool FontBold { 
            get { return fontBold; }
            set { SetField(ref fontBold, value, () => FontBold); }
        }
        private bool fontItalic;
        public bool FontItalic { 
            get { return fontItalic; }
            set { SetField(ref fontItalic, value, () => FontItalic); }
        }
        private float fontSize;
        public float FontSize { 
            get { return fontSize; }
            set { SetField(ref fontSize, value, () => FontSize); }
        }
        public int MinimapSize { get; set; }
        public bool MinimapTeamColours { get; set; }
        public bool MinimapTeamColoursExceptFeatures { get; set; }
        public Rgba MinimapCliffColour { get; set; }
        public Rgba MinimapSelectedObjectsColour { get; set; }
        public double FOVDefault { get; set; }
        public bool Mipmaps { get; set; }
        public bool MipmapsHardware { get; set; }
        public string OpenPath { get; set; }
        public string SavePath { get; set; }
        public int MapViewBPP { get; set; }
        public int TextureViewBPP { get; set; }
        public int MapViewDepth { get; set; }
        public int TextureViewDepth { get; set; }
        public ObservableCollection<string> TilesetDirectories { get; private set; }
        public ObservableCollection<string> ObjectDataDirectories { get; private set; }
        public bool PickOrientation { get; set; }

        [JsonIgnore]
        public Font Font { get; private set; }
        #endregion

        #region INotifyPropertyChanged

        [EventPublication(EventTopics.SettingsChanged)]
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, string propertyName)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        protected virtual void OnPropertyChanged<T>(Expression<Func<T>> selectorExpression)
        {
            if (selectorExpression == null)
                throw new ArgumentNullException("selectorExpression");
            MemberExpression body = selectorExpression.Body as MemberExpression;
            if (body == null)
                throw new ArgumentException("The body must be a member expression");
            OnPropertyChanged(body.Member.Name);
        }

        protected bool SetField<T>(ref T field, T value, Expression<Func<T>> selectorExpression)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(selectorExpression);
            return true;
        }
        #endregion

        #region Public Methods
        public SettingsManager ()
        {
            SetDefaults ();
        }

        public void SetDefaults()
        {
            AutoSaveEnabled = true;
            AutoSaveCompress = false;
            AutoSaveMinIntervalSeconds = 180U;
            AutoSaveMinChanges = 20U;
            UndoLimit = 256U;
            ShowOptionsAtStartup = true;
            DirectPointer = true;
            FontFamily = FontFamily.GenericSerif;
            FontBold = true;
            FontItalic = true;
            FontSize = 20.0F;
            MinimapSize = 160;
            MinimapTeamColours = true;
            MinimapTeamColoursExceptFeatures = true;
            MinimapCliffColour = new Rgba (1.0F, 0.25F, 0.25F, 0.5F);
            MinimapSelectedObjectsColour = new Rgba (1.0F, 1.0F, 1.0F, 0.75F);
            FOVDefault = 30.0D / (50.0D * 900.0D);
            Mipmaps = true;
            MipmapsHardware = true;
            OpenPath = null;
            SavePath = null;
            MapViewBPP = DisplayDevice.Default.BitsPerPixel;
            TextureViewBPP = DisplayDevice.Default.BitsPerPixel;
            MapViewDepth = 24;
            TextureViewDepth = 24;
            TilesetDirectories = new ObservableCollection<string> ();
            ObjectDataDirectories = new ObservableCollection<string> ();
            PickOrientation = true;
        }

        public Result Save(string path)
        {
            var returnResult = new Result(string.Format("Writing settings to \"{0}\"",  path), false);
            logger.Info("Writing settings to \"{0}\"", path);

            try {
                var json = JsonConvert.SerializeObject (this, Formatting.Indented);
                using (var file = new StreamWriter(path)) {
                    file.Write (json);
                }
            }
            catch (Exception ex) {
                Debugger.Break ();
                returnResult.ProblemAdd (string.Format("Got an exception: {0}.", ex.Message));
                logger.ErrorException ("Got an exception while saving the settings.", ex);
            }           

            return returnResult;
        }

        public Result Load(string path)
        {
            var returnResult = new Result(string.Format("Loading settings from \"{0}\"", path), false);
            logger.Info(string.Format("Loading settings from \"{0}\"", path));

            try
            {
                using (var file = new StreamReader(path)) {
                    var text = file.ReadToEnd();
                    JsonConvert.PopulateObject(text, this, null);
                }
            }
            catch (Exception ex)
            {
                returnResult.ProblemAdd (string.Format("Got an exception: {0}.", ex.Message));
                return returnResult;
            }

            return returnResult;
        }
        #endregion       
    }
}

