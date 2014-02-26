#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using NLog;
using OpenTK;
using SharpFlame.Old.Colors;
using Newtonsoft.Json;
using SharpFlame.Core.Extensions;
using SharpFlame.Core;
using SharpFlame.Core.Domain.Colors;

#endregion

namespace SharpFlame.Old.AppSettings
{
    public sealed class SettingsManager
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public static OptionGroup OptionsSettings = new OptionGroup();
        public static clsSettings InitializeSettings;
        public static clsSettings Settings;

        public static Option<bool> SettingAutoSaveEnabled;
        public static Option<bool> SettingAutoSaveCompress;
        public static Option<UInt32> SettingAutoSaveMinIntervalS;
        public static Option<UInt32> SettingAutoSaveMinChanges;
        public static Option<UInt32> SettingUndoLimit;
        public static Option<bool> SettingDirectoriesPrompt;
        public static Option<bool> SettingDirectPointer;
        public static Option<FontFamily> SettingFontFamily;
        public static Option<bool> SettingFontBold;
        public static Option<bool> SettingFontItalic;

        public static OptionFontSize SettingFontSize;

        public static OptionMinimapSize SettingMinimapSize;
        public static Option<bool> SettingMinimapTeamColours;
        public static Option<bool> SettingMinimapTeamColoursExceptFeatures;
        public static Option<Rgba> SettingMinimapCliffColour;
        public static Option<Rgba> SettingMinimapSelectedObjectsColour;

        public static OptionFovDefault SettingFovDefault;
        public static Option<bool> SettingMipmaps;
        public static Option<bool> SettingMipmapsHardware;
        public static Option<string> SettingOpenPath;
        public static Option<string> SettingSavePath;
        public static Option<int> SettingMapViewBpp;
        public static Option<int> SettingTextureViewBpp;
        public static Option<int> SettingMapViewDepth;
        public static Option<int> SettingTextureViewDepth;
        public static Option<List<string>> SettingTilesetDirectories;
        public static Option<List<string>> SettingObjectDataDirectories;
        public static Option<bool> SettingPickOrientation;

        private static Option<T> CreateSetting<T>(string saveKey, T defaultValue)
        {
            var creator = new OptionCreator<T>();
            creator.SaveKey = saveKey;
            creator.DefaultValue = defaultValue;
            var result = creator.Create();
            OptionsSettings.Options.Add(result.GroupLink);
            return result;
        }

        private static Option<T> CreateSetting<T>(OptionCreator<T> creator, string saveKey, T defaultValue)
        {
            creator.SaveKey = saveKey;
            creator.DefaultValue = defaultValue;
            var result = creator.Create();
            OptionsSettings.Options.Add(result.GroupLink);
            return result;
        }

        public static void CreateSettingOptions()
        {
            SettingAutoSaveEnabled = CreateSetting("AutoSave", true);
            SettingAutoSaveCompress = CreateSetting("AutoSaveCompress", false);
            SettingAutoSaveMinIntervalS = CreateSetting("AutoSaveMinInterval", 180U);
            SettingAutoSaveMinChanges = CreateSetting("AutoSaveMinChanges", 20U);
            SettingUndoLimit = CreateSetting("UndoLimit", 256U);
            SettingDirectoriesPrompt = CreateSetting("DirectoriesPrompt", true);
            SettingDirectPointer = CreateSetting("DirectPointer", true);
            SettingFontFamily = CreateSetting("FontFamily", FontFamily.GenericSerif);
            SettingFontBold = CreateSetting("FontBold", true);
            SettingFontItalic = CreateSetting("FontItalic", false);
            SettingFontSize = (OptionFontSize)(CreateSetting(new OptionCreatorFontSize(), "FontSize", 20.0F));
            SettingMinimapSize = (OptionMinimapSize)(CreateSetting(new OptionCreatorMinimapSize(), "MinimapSize", 160));
            SettingMinimapTeamColours = CreateSetting("MinimapTeamColours", true);
            SettingMinimapTeamColoursExceptFeatures = CreateSetting("MinimapTeamColoursExceptFeatures", true);
            SettingMinimapCliffColour = CreateSetting("MinimapCliffColour", new Rgba(1.0F, 0.25F, 0.25F, 0.5F));
            SettingMinimapSelectedObjectsColour = CreateSetting("MinimapSelectedObjectsColour", new Rgba(1.0F, 1.0F, 1.0F, 0.75F));
            SettingFovDefault = (OptionFovDefault)(CreateSetting(new OptionCreatorFovDefault(), "FOVDefault", 30.0D / (50.0D * 900.0D)));
            //screenVerticalSize/(screenDist*screenVerticalPixels)
            SettingMipmaps = CreateSetting("Mipmaps", false);
            SettingMipmapsHardware = CreateSetting("MipmapsHardware", false);
            SettingOpenPath = CreateSetting<string>("OpenPath", null);
            SettingSavePath = CreateSetting<string>("SavePath", null);
            SettingMapViewBpp = CreateSetting("MapViewBPP", DisplayDevice.Default.BitsPerPixel);
            SettingTextureViewBpp = CreateSetting("TextureViewBPP", DisplayDevice.Default.BitsPerPixel);
            SettingMapViewDepth = CreateSetting("MapViewDepth", 24);
            SettingTextureViewDepth = CreateSetting("TextureViewDepth", 24);
            SettingTilesetDirectories = CreateSetting("TilesetsPath", new List<string>());
            SettingObjectDataDirectories = CreateSetting("ObjectDataPath", new List<string>());
            SettingPickOrientation = CreateSetting("PickOrientation", true);
        }

        public static void UpdateSettings(clsSettings newSettings)
        {
            var fontChanged = false;

            if ( Settings == null )
            {
                fontChanged = true;
            }
            else
            {
                if ( Settings.FontFamily == null )
                {
                    fontChanged = newSettings.FontFamily != null;
                }
                else
                {
                    if ( newSettings.FontFamily == null )
                    {
                        fontChanged = true;
                    }
                    else
                    {
                        if ( Settings.FontFamily.Name != newSettings.FontFamily.Name
                             || Settings.FontBold != newSettings.FontBold
                             || Settings.FontItalic != newSettings.FontItalic
                             || Settings.FontSize != newSettings.FontSize )
                        {
                            fontChanged = true;
                        }
                    }
                }
            }
            if ( fontChanged )
            {
                SetFont(newSettings.MakeFont());
            }

            Settings = newSettings;
        }

        private static void SetFont(Font newFont)
        {
            if ( App.UnitLabelFont != null )
            {
                App.UnitLabelFont.Deallocate();
            }
            App.UnitLabelFont = Program.frmMainInstance.MapViewControl.CreateGLFont(newFont);
        }

        public static Result SettingsWrite()
        {
            var ReturnResult = new Result("Writing settings to \"{0}\"".Format2(App.SettingsPath), false);
            logger.Info("Writing settings to \"{0}\"".Format2(App.SettingsPath));

            try {
                var json = JsonConvert.SerializeObject (Settings, Formatting.Indented);
                using (var file = new StreamWriter(App.SettingsPath)) {
                    file.Write (json);
                }
            }
            catch (Exception ex) {
                Debugger.Break ();
                ReturnResult.ProblemAdd (string.Format("Got an exception while saving settings: {0}.", ex.Message));
                logger.ErrorException ("Got an exception while saving the settings.", ex);
            }           

            return ReturnResult;
        }

        public static Result SettingsLoad(ref clsSettings result)
        {
            var returnResult = new Result("Loading settings from \"{0}\"".Format2(App.SettingsPath), false);
            logger.Info("Loading settings from \"{0}\"".Format2(App.SettingsPath));

            try
            {
                using (var file = new StreamReader(App.SettingsPath)) {
                    var text = file.ReadToEnd();
                    result = JsonConvert.DeserializeObject<clsSettings>(text);
                }
            }
            catch
            {
                result = new clsSettings();
                return returnResult;
            }

            return returnResult;
        }
    }

    public class SettingsCreator : OptionProfileCreator
    {
        public override OptionProfile Create()
        {
            return new clsSettings();
        }
    }

    public class clsSettings : OptionProfile
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public clsSettings() : base(SettingsManager.OptionsSettings)
        {
        }

        public bool AutoSaveEnabled
        {
            get { return Convert.ToBoolean(GetValue(SettingsManager.SettingAutoSaveEnabled)); }
            set { SetChanges (SettingsManager.SettingAutoSaveEnabled, new Change<bool>(value)); }
        }

        public bool AutoSaveCompress
        {
            get { return Convert.ToBoolean(GetValue(SettingsManager.SettingAutoSaveCompress)); }
            set { SetChanges (SettingsManager.SettingAutoSaveCompress, new Change<bool>(value)); }
        }

        public UInt32 AutoSaveMinIntervalSeconds
        {
            get { return Convert.ToUInt32(GetValue(SettingsManager.SettingAutoSaveMinIntervalS)); }
            set { SetChanges (SettingsManager.SettingAutoSaveMinIntervalS, new Change<UInt32> (value)); }
        }

        public UInt32 AutoSaveMinChanges
        {
            get { return Convert.ToUInt32(GetValue(SettingsManager.SettingAutoSaveMinChanges)); }
            set { SetChanges (SettingsManager.SettingAutoSaveMinChanges, new Change<UInt32>(value)); }
        }

        public UInt32 UndoLimit
        {
            get { return Convert.ToUInt32(GetValue(SettingsManager.SettingUndoLimit)); }
            set { SetChanges (SettingsManager.SettingUndoLimit, new Change<UInt32>(value)); }
        }

        public bool DirectoriesPrompt
        {
            get { return Convert.ToBoolean(GetValue(SettingsManager.SettingDirectoriesPrompt)); }
            set { SetChanges (SettingsManager.SettingDirectoriesPrompt, new Change<bool>(value)); }
        }

        public bool DirectPointer
        {
            get { return Convert.ToBoolean(GetValue(SettingsManager.SettingDirectPointer)); }
            set { SetChanges (SettingsManager.SettingDirectPointer, new Change<bool>(value)); }
        }

        public FontFamily FontFamily
        {
            get { return ((FontFamily)(GetValue(SettingsManager.SettingFontFamily))); }
            set { SetChanges (SettingsManager.SettingFontFamily, new Change<FontFamily>(value)); }
        }

        public bool FontBold
        {
            get { return Convert.ToBoolean(GetValue(SettingsManager.SettingFontBold)); }
            set { SetChanges (SettingsManager.SettingFontBold, new Change<bool>(value)); }
        }

        public bool FontItalic
        {
            get { return Convert.ToBoolean(GetValue(SettingsManager.SettingFontItalic)); }
            set { SetChanges (SettingsManager.SettingFontItalic, new Change<bool>(value)); }
        }

        public float FontSize
        {
            get { return Convert.ToSingle(Convert.ToSingle(GetValue(SettingsManager.SettingFontSize))); }
            set { SetChanges (SettingsManager.SettingFontSize, new Change<float>(value)); }
        }

        public int MinimapSize
        {
            get { return Convert.ToInt32(GetValue(SettingsManager.SettingMinimapSize)); }
            set { SetChanges (SettingsManager.SettingMinimapSize, new Change<int>(value)); }
        }

        public bool MinimapTeamColours
        {
            get { return Convert.ToBoolean(GetValue(SettingsManager.SettingMinimapTeamColours)); }
            set { SetChanges (SettingsManager.SettingMinimapTeamColours, new Change<bool>(value)); }
        }

        public bool MinimapTeamColoursExceptFeatures
        {
            get { return Convert.ToBoolean(GetValue(SettingsManager.SettingMinimapTeamColoursExceptFeatures)); }
            set { SetChanges (SettingsManager.SettingMinimapTeamColoursExceptFeatures, new Change<bool>(value)); }
        }

        public Rgba MinimapCliffColour
        {
            get { return ((Rgba)(GetValue(SettingsManager.SettingMinimapCliffColour))); }
            set { SetChanges (SettingsManager.SettingMinimapCliffColour, new Change<Rgba>(value)); }
        }

        public Rgba MinimapSelectedObjectsColour
        {
            get { return ((Rgba)(GetValue(SettingsManager.SettingMinimapSelectedObjectsColour))); }
            set { SetChanges (SettingsManager.SettingMinimapSelectedObjectsColour, new Change<Rgba>(value)); }
        }

        public double FOVDefault
        {
            get { return Convert.ToDouble(GetValue(SettingsManager.SettingFovDefault)); }
            set { SetChanges (SettingsManager.SettingFovDefault, new Change<double>(value)); }
        }

        public bool Mipmaps
        {
            get { return Convert.ToBoolean(GetValue(SettingsManager.SettingMipmaps)); }
            set { SetChanges (SettingsManager.SettingMipmaps, new Change<bool>(value)); }
        }

        public bool MipmapsHardware
        {
            get { return Convert.ToBoolean(GetValue(SettingsManager.SettingMipmapsHardware)); }
            set { SetChanges (SettingsManager.SettingMipmapsHardware, new Change<bool>(value)); }
        }

        public string OpenPath
        {
            get { return Convert.ToString(GetValue(SettingsManager.SettingOpenPath)); }
            set { SetChanges(SettingsManager.SettingOpenPath, new Change<string>(value)); }
        }

        public string SavePath
        {
            get { return Convert.ToString(GetValue(SettingsManager.SettingSavePath)); }
            set { SetChanges(SettingsManager.SettingSavePath, new Change<string>(value)); }
        }

        public int MapViewBPP
        {
            get { return Convert.ToInt32(GetValue(SettingsManager.SettingMapViewBpp)); }
            set { SetChanges (SettingsManager.SettingMapViewBpp, new Change<int>(value)); }
        }

        public int TextureViewBPP
        {
            get { return Convert.ToInt32(GetValue(SettingsManager.SettingTextureViewBpp)); }
            set { SetChanges (SettingsManager.SettingTextureViewBpp, new Change<int>(value)); }
        }

        public int MapViewDepth
        {
            get { return Convert.ToInt32(GetValue(SettingsManager.SettingMapViewDepth)); }
            set { SetChanges (SettingsManager.SettingMapViewDepth, new Change<int>(value)); }
        }

        public int TextureViewDepth
        {
            get { return Convert.ToInt32(GetValue(SettingsManager.SettingTextureViewDepth)); }
            set { SetChanges (SettingsManager.SettingTextureViewDepth, new Change<int>(value)); }
        }

        public List<string> TilesetDirectories
        {
            get { return ((List<string>)(GetValue(SettingsManager.SettingTilesetDirectories))); }
            set { 
                logger.Info("Test");
                SetChanges (SettingsManager.SettingTilesetDirectories, new Change<List<string>>(value)); 
            }
        }

        public List<string> ObjectDataDirectories
        {
            get { return ((List<string>)(GetValue(SettingsManager.SettingObjectDataDirectories))); }
            set { SetChanges (SettingsManager.SettingObjectDataDirectories, new Change<List<string>>(value)); }
        }

        public bool PickOrientation
        {
            get { return Convert.ToBoolean(GetValue(SettingsManager.SettingPickOrientation)); }
            set { SetChanges (SettingsManager.SettingPickOrientation, new Change<bool>(value)); }
        }

        public Font MakeFont()
        {
            var style = FontStyle.Regular;
            if ( FontBold )
            {
                style = style | FontStyle.Bold;
            }
            if ( FontItalic )
            {
                style = style | FontStyle.Italic;
            }
            return new Font(FontFamily, FontSize, style, GraphicsUnit.Point);
        }
    }

    public class OptionCreatorFovDefault : OptionCreator<double>
    {
        public override Option<double> Create()
        {
            return new OptionFovDefault(SaveKey, DefaultValue);
        }
    }

    public class OptionFovDefault : Option<double>
    {
        public OptionFovDefault(string saveKey, double defaultValue) : base(saveKey, defaultValue)
        {
        }

        public override bool IsValueValid(object value)
        {
            var dblValue = Convert.ToDouble(value);
            return dblValue >= 0.00005D & dblValue <= 0.005D;
        }
    }

    public class OptionCreatorMinimapSize : OptionCreator<int>
    {
        public override Option<int> Create()
        {
            return new OptionMinimapSize(SaveKey, DefaultValue);
        }
    }

    public class OptionMinimapSize : Option<int>
    {
        public OptionMinimapSize(string saveKey, int defaultValue) : base(saveKey, defaultValue)
        {
        }

        public override bool IsValueValid(object value)
        {
            var intValue = Convert.ToInt32(value);
            return intValue >= 0 & intValue <= Constants.MinimapMaxSize;
        }
    }

    public class OptionCreatorFontSize : OptionCreator<Single>
    {
        public override Option<float> Create()
        {
            return new OptionFontSize(SaveKey, DefaultValue);
        }
    }

    public class OptionFontSize : Option<Single>
    {
        public OptionFontSize(string saveKey, float defaultValue) : base(saveKey, defaultValue)
        {
        }

        public override bool IsValueValid(object value)
        {
            return Convert.ToSingle(value) >= 0.0F;
        }
    }
}