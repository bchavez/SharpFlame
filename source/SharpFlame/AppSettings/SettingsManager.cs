#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using NLog;
using OpenTK;
using SharpFlame.Colors;
using Newtonsoft.Json;
using SharpFlame.Core.Extensions;

#endregion

namespace SharpFlame.AppSettings
{
    public sealed class SettingsManager
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public static OptionGroup Options_Settings = new OptionGroup();
        public static clsSettings InitializeSettings;
        public static clsSettings Settings;

        public static Option<bool> Setting_AutoSaveEnabled;
        public static Option<bool> Setting_AutoSaveCompress;
        public static Option<UInt32> Setting_AutoSaveMinInterval_s;
        public static Option<UInt32> Setting_AutoSaveMinChanges;
        public static Option<UInt32> Setting_UndoLimit;
        public static Option<bool> Setting_DirectoriesPrompt;
        public static Option<bool> Setting_DirectPointer;
        public static Option<FontFamily> Setting_FontFamily;
        public static Option<bool> Setting_FontBold;
        public static Option<bool> Setting_FontItalic;

        public static OptionFontSize Setting_FontSize;

        public static OptionMinimapSize Setting_MinimapSize;
        public static Option<bool> Setting_MinimapTeamColours;
        public static Option<bool> Setting_MinimapTeamColoursExceptFeatures;
        public static Option<clsRGBA_sng> Setting_MinimapCliffColour;
        public static Option<clsRGBA_sng> Setting_MinimapSelectedObjectsColour;

        public static OptionFovDefault Setting_FOVDefault;
        public static Option<bool> Setting_Mipmaps;
        public static Option<bool> Setting_MipmapsHardware;
        public static Option<string> Setting_OpenPath;
        public static Option<string> Setting_SavePath;
        public static Option<int> Setting_MapViewBPP;
        public static Option<int> Setting_TextureViewBPP;
        public static Option<int> Setting_MapViewDepth;
        public static Option<int> Setting_TextureViewDepth;
        public static Option<List<string>> Setting_TilesetDirectories;
        public static Option<List<string>> Setting_ObjectDataDirectories;
        public static Option<bool> Setting_PickOrientation;

        private static Option<T> CreateSetting<T>(string saveKey, T defaultValue)
        {
            var creator = new OptionCreator<T>();
            creator.SaveKey = saveKey;
            creator.DefaultValue = defaultValue;
            var result = creator.Create();
            Options_Settings.Options.Add(result.GroupLink);
            return result;
        }

        private static Option<T> CreateSetting<T>(OptionCreator<T> creator, string saveKey, T defaultValue)
        {
            creator.SaveKey = saveKey;
            creator.DefaultValue = defaultValue;
            var result = creator.Create();
            Options_Settings.Options.Add(result.GroupLink);
            return result;
        }

        public static void CreateSettingOptions()
        {
            Setting_AutoSaveEnabled = CreateSetting("AutoSave", true);
            Setting_AutoSaveCompress = CreateSetting("AutoSaveCompress", false);
            Setting_AutoSaveMinInterval_s = CreateSetting("AutoSaveMinInterval", 180U);
            Setting_AutoSaveMinChanges = CreateSetting("AutoSaveMinChanges", 20U);
            Setting_UndoLimit = CreateSetting("UndoLimit", 256U);
            Setting_DirectoriesPrompt = CreateSetting("DirectoriesPrompt", true);
            Setting_DirectPointer = CreateSetting("DirectPointer", true);
            Setting_FontFamily = CreateSetting("FontFamily", FontFamily.GenericSerif);
            Setting_FontBold = CreateSetting("FontBold", true);
            Setting_FontItalic = CreateSetting("FontItalic", false);
            Setting_FontSize = (OptionFontSize)(CreateSetting(new OptionCreatorFontSize(), "FontSize", 20.0F));
            Setting_MinimapSize = (OptionMinimapSize)(CreateSetting(new OptionCreatorMinimapSize(), "MinimapSize", 160));
            Setting_MinimapTeamColours = CreateSetting("MinimapTeamColours", true);
            Setting_MinimapTeamColoursExceptFeatures = CreateSetting("MinimapTeamColoursExceptFeatures", true);
            Setting_MinimapCliffColour = CreateSetting("MinimapCliffColour", new clsRGBA_sng(1.0F, 0.25F, 0.25F, 0.5F));
            Setting_MinimapSelectedObjectsColour = CreateSetting("MinimapSelectedObjectsColour", new clsRGBA_sng(1.0F, 1.0F, 1.0F, 0.75F));
            Setting_FOVDefault = (OptionFovDefault)(CreateSetting(new OptionCreatorFovDefault(), "FOVDefault", 30.0D / (50.0D * 900.0D)));
            //screenVerticalSize/(screenDist*screenVerticalPixels)
            Setting_Mipmaps = CreateSetting("Mipmaps", false);
            Setting_MipmapsHardware = CreateSetting("MipmapsHardware", false);
            Setting_OpenPath = CreateSetting<string>("OpenPath", null);
            Setting_SavePath = CreateSetting<string>("SavePath", null);
            Setting_MapViewBPP = CreateSetting("MapViewBPP", DisplayDevice.Default.BitsPerPixel);
            Setting_TextureViewBPP = CreateSetting("TextureViewBPP", DisplayDevice.Default.BitsPerPixel);
            Setting_MapViewDepth = CreateSetting("MapViewDepth", 24);
            Setting_TextureViewDepth = CreateSetting("TextureViewDepth", 24);
            Setting_TilesetDirectories = CreateSetting("TilesetsPath", new List<string>());
            Setting_ObjectDataDirectories = CreateSetting("ObjectDataPath", new List<string>());
            Setting_PickOrientation = CreateSetting("PickOrientation", true);
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

        public static clsResult Settings_Write()
        {
            var ReturnResult = new clsResult("Writing settings to \"{0}\"".Format2(App.SettingsPath), false);
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

        public static clsResult Settings_Load(ref clsSettings result)
        {
            var returnResult = new clsResult("Loading settings from \"{0}\"".Format2(App.SettingsPath), false);
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

        public clsSettings() : base(SettingsManager.Options_Settings)
        {
        }

        public bool AutoSaveEnabled
        {
            get { return Convert.ToBoolean(get_Value(SettingsManager.Setting_AutoSaveEnabled)); }
            set { set_Changes (SettingsManager.Setting_AutoSaveEnabled, new Change<bool>(value)); }
        }

        public bool AutoSaveCompress
        {
            get { return Convert.ToBoolean(get_Value(SettingsManager.Setting_AutoSaveCompress)); }
            set { set_Changes (SettingsManager.Setting_AutoSaveCompress, new Change<bool>(value)); }
        }

        public UInt32 AutoSaveMinInterval_s
        {
            get { return Convert.ToUInt32(get_Value(SettingsManager.Setting_AutoSaveMinInterval_s)); }
            set { set_Changes (SettingsManager.Setting_AutoSaveMinInterval_s, new Change<UInt32> (value)); }
        }

        public UInt32 AutoSaveMinChanges
        {
            get { return Convert.ToUInt32(get_Value(SettingsManager.Setting_AutoSaveMinChanges)); }
            set { set_Changes (SettingsManager.Setting_AutoSaveMinChanges, new Change<UInt32>(value)); }
        }

        public UInt32 UndoLimit
        {
            get { return Convert.ToUInt32(get_Value(SettingsManager.Setting_UndoLimit)); }
            set { set_Changes (SettingsManager.Setting_UndoLimit, new Change<UInt32>(value)); }
        }

        public bool DirectoriesPrompt
        {
            get { return Convert.ToBoolean(get_Value(SettingsManager.Setting_DirectoriesPrompt)); }
            set { set_Changes (SettingsManager.Setting_DirectoriesPrompt, new Change<bool>(value)); }
        }

        public bool DirectPointer
        {
            get { return Convert.ToBoolean(get_Value(SettingsManager.Setting_DirectPointer)); }
            set { set_Changes (SettingsManager.Setting_DirectPointer, new Change<bool>(value)); }
        }

        public FontFamily FontFamily
        {
            get { return ((FontFamily)(get_Value(SettingsManager.Setting_FontFamily))); }
            set { set_Changes (SettingsManager.Setting_FontFamily, new Change<FontFamily>(value)); }
        }

        public bool FontBold
        {
            get { return Convert.ToBoolean(get_Value(SettingsManager.Setting_FontBold)); }
            set { set_Changes (SettingsManager.Setting_FontBold, new Change<bool>(value)); }
        }

        public bool FontItalic
        {
            get { return Convert.ToBoolean(get_Value(SettingsManager.Setting_FontItalic)); }
            set { set_Changes (SettingsManager.Setting_FontItalic, new Change<bool>(value)); }
        }

        public float FontSize
        {
            get { return Convert.ToSingle(Convert.ToSingle(get_Value(SettingsManager.Setting_FontSize))); }
            set { set_Changes (SettingsManager.Setting_FontSize, new Change<float>(value)); }
        }

        public int MinimapSize
        {
            get { return Convert.ToInt32(get_Value(SettingsManager.Setting_MinimapSize)); }
            set { set_Changes (SettingsManager.Setting_MinimapSize, new Change<int>(value)); }
        }

        public bool MinimapTeamColours
        {
            get { return Convert.ToBoolean(get_Value(SettingsManager.Setting_MinimapTeamColours)); }
            set { set_Changes (SettingsManager.Setting_MinimapTeamColours, new Change<bool>(value)); }
        }

        public bool MinimapTeamColoursExceptFeatures
        {
            get { return Convert.ToBoolean(get_Value(SettingsManager.Setting_MinimapTeamColoursExceptFeatures)); }
            set { set_Changes (SettingsManager.Setting_MinimapTeamColoursExceptFeatures, new Change<bool>(value)); }
        }

        public clsRGBA_sng MinimapCliffColour
        {
            get { return ((clsRGBA_sng)(get_Value(SettingsManager.Setting_MinimapCliffColour))); }
            set { set_Changes (SettingsManager.Setting_MinimapCliffColour, new Change<clsRGBA_sng>(value)); }
        }

        public clsRGBA_sng MinimapSelectedObjectsColour
        {
            get { return ((clsRGBA_sng)(get_Value(SettingsManager.Setting_MinimapSelectedObjectsColour))); }
            set { set_Changes (SettingsManager.Setting_MinimapSelectedObjectsColour, new Change<clsRGBA_sng>(value)); }
        }

        public double FOVDefault
        {
            get { return Convert.ToDouble(get_Value(SettingsManager.Setting_FOVDefault)); }
            set { set_Changes (SettingsManager.Setting_FOVDefault, new Change<double>(value)); }
        }

        public bool Mipmaps
        {
            get { return Convert.ToBoolean(get_Value(SettingsManager.Setting_Mipmaps)); }
            set { set_Changes (SettingsManager.Setting_Mipmaps, new Change<bool>(value)); }
        }

        public bool MipmapsHardware
        {
            get { return Convert.ToBoolean(get_Value(SettingsManager.Setting_MipmapsHardware)); }
            set { set_Changes (SettingsManager.Setting_MipmapsHardware, new Change<bool>(value)); }
        }

        public string OpenPath
        {
            get { return Convert.ToString(get_Value(SettingsManager.Setting_OpenPath)); }
            set { set_Changes(SettingsManager.Setting_OpenPath, new Change<string>(value)); }
        }

        public string SavePath
        {
            get { return Convert.ToString(get_Value(SettingsManager.Setting_SavePath)); }
            set { set_Changes(SettingsManager.Setting_SavePath, new Change<string>(value)); }
        }

        public int MapViewBPP
        {
            get { return Convert.ToInt32(get_Value(SettingsManager.Setting_MapViewBPP)); }
            set { set_Changes (SettingsManager.Setting_MapViewBPP, new Change<int>(value)); }
        }

        public int TextureViewBPP
        {
            get { return Convert.ToInt32(get_Value(SettingsManager.Setting_TextureViewBPP)); }
            set { set_Changes (SettingsManager.Setting_TextureViewBPP, new Change<int>(value)); }
        }

        public int MapViewDepth
        {
            get { return Convert.ToInt32(get_Value(SettingsManager.Setting_MapViewDepth)); }
            set { set_Changes (SettingsManager.Setting_MapViewDepth, new Change<int>(value)); }
        }

        public int TextureViewDepth
        {
            get { return Convert.ToInt32(get_Value(SettingsManager.Setting_TextureViewDepth)); }
            set { set_Changes (SettingsManager.Setting_TextureViewDepth, new Change<int>(value)); }
        }

        public List<string> TilesetDirectories
        {
            get { return ((List<string>)(get_Value(SettingsManager.Setting_TilesetDirectories))); }
            set { 
                logger.Info("Test");
                set_Changes (SettingsManager.Setting_TilesetDirectories, new Change<List<string>>(value)); 
            }
        }

        public List<string> ObjectDataDirectories
        {
            get { return ((List<string>)(get_Value(SettingsManager.Setting_ObjectDataDirectories))); }
            set { set_Changes (SettingsManager.Setting_ObjectDataDirectories, new Change<List<string>>(value)); }
        }

        public bool PickOrientation
        {
            get { return Convert.ToBoolean(get_Value(SettingsManager.Setting_PickOrientation)); }
            set { set_Changes (SettingsManager.Setting_PickOrientation, new Change<bool>(value)); }
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