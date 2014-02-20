#region

using System;
using System.Drawing;
using System.IO;
using NLog;
using OpenTK;
using SharpFlame.Collections;
using SharpFlame.Colors;
using SharpFlame.Core.Parsers.Ini;
using IniReader = SharpFlame.FileIO.Ini.IniReader;

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
        public static Option<SimpleList<string>> Setting_TilesetDirectories;
        public static Option<SimpleList<string>> Setting_ObjectDataDirectories;
        public static Option<int> Setting_DefaultTilesetsPathNum;
        public static Option<int> Setting_DefaultObjectDataPathNum;
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
            Setting_TilesetDirectories = CreateSetting("TilesetsPath", new SimpleList<string>());
            Setting_ObjectDataDirectories = CreateSetting("ObjectDataPath", new SimpleList<string>());
            Setting_DefaultTilesetsPathNum = CreateSetting("DefaultTilesetsPathNum", -1);
            Setting_DefaultObjectDataPathNum = CreateSetting("DefaultObjectDataPathNum", -1);
            Setting_PickOrientation = CreateSetting("PickOrientation", true);
        }

        public static clsResult Read_Settings(StreamReader File, ref clsSettings Result)
        {
            var ReturnResult = new clsResult("Reading settings", false);
            logger.Info("Reading settings");

            var INIReader = new IniReader();
            ReturnResult.Take(INIReader.ReadFile(File));
            Result = new clsSettings();
            ReturnResult.Take(INIReader.RootSection.Translate(Result));
            foreach ( var section in INIReader.Sections )
            {
                if ( section.Name.ToLower() == "keyboardcontrols" )
                {
                    var keyResults = new clsResult("Keyboard controls", false);
                    logger.Debug("Reading keyboard controls");
                    keyResults.Take(section.Translate(KeyboardManager.KeyboardProfile));
                    ReturnResult.Take(keyResults);
                }
            }

            return ReturnResult;
        }

        public static void UpdateSettings(clsSettings NewSettings)
        {
            var FontChanged = default(bool);

            if ( Settings == null )
            {
                FontChanged = true;
            }
            else
            {
                if ( Settings.FontFamily == null )
                {
                    FontChanged = NewSettings.FontFamily != null;
                }
                else
                {
                    if ( NewSettings.FontFamily == null )
                    {
                        FontChanged = true;
                    }
                    else
                    {
                        if ( Settings.FontFamily.Name == NewSettings.FontFamily.Name
                             && Settings.FontBold == NewSettings.FontBold
                             && Settings.FontItalic == NewSettings.FontItalic
                             && Settings.FontSize == NewSettings.FontSize )
                        {
                            FontChanged = false;
                        }
                        else
                        {
                            FontChanged = true;
                        }
                    }
                }
            }
            if ( FontChanged )
            {
                SetFont(NewSettings.MakeFont());
            }

            Settings = NewSettings;
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

            try
            {
                using ( var file = File.Create(App.SettingsPath) )
                {
                    var iniSettings = new IniWriter(file);
                    ReturnResult.Take(Serialize_Settings(iniSettings));
                    iniSettings.Flush();
                }
            }
            catch ( Exception ex )
            {
                ReturnResult.ProblemAdd(ex.Message);
                return ReturnResult;
            }

            return ReturnResult;
        }

        private static clsResult Serialize_Settings(IniWriter File)
        {
            var returnResult = new clsResult("Serializing settings", false);
            logger.Info("Serializing settings");

            returnResult.Take(Settings.INIWrite(File));
            if ( KeyboardManager.KeyboardProfile.IsAnythingChanged )
            {
                File.AddSection("KeyboardControls");
                returnResult.Take(KeyboardManager.KeyboardProfile.INIWrite(File));
            }

            return returnResult;
        }

        public static clsResult Settings_Load(ref clsSettings Result)
        {
            var ReturnResult = new clsResult("Loading settings from \"{0}\"".Format2(App.SettingsPath), false);
            logger.Info("Loading settings from \"{0}\"".Format2(App.SettingsPath));

            var File_Settings = default(StreamReader);
            try
            {
                File_Settings = new StreamReader(App.SettingsPath);
            }
            catch
            {
                Result = new clsSettings();
                return ReturnResult;
            }

            ReturnResult.Take(Read_Settings(File_Settings, ref Result));

            File_Settings.Close();

            return ReturnResult;
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
        public clsSettings() : base(SettingsManager.Options_Settings)
        {
        }

        public bool AutoSaveEnabled
        {
            get { return Convert.ToBoolean(get_Value(SettingsManager.Setting_AutoSaveEnabled)); }
        }

        public bool AutoSaveCompress
        {
            get { return Convert.ToBoolean(get_Value(SettingsManager.Setting_AutoSaveCompress)); }
        }

        public UInt32 AutoSaveMinInterval_s
        {
            get { return Convert.ToUInt32(get_Value(SettingsManager.Setting_AutoSaveMinInterval_s)); }
        }

        public UInt32 AutoSaveMinChanges
        {
            get { return Convert.ToUInt32(get_Value(SettingsManager.Setting_AutoSaveMinChanges)); }
        }

        public UInt32 UndoLimit
        {
            get { return Convert.ToUInt32(get_Value(SettingsManager.Setting_UndoLimit)); }
        }

        public bool DirectoriesPrompt
        {
            get { return Convert.ToBoolean(get_Value(SettingsManager.Setting_DirectoriesPrompt)); }
        }

        public bool DirectPointer
        {
            get { return Convert.ToBoolean(get_Value(SettingsManager.Setting_DirectPointer)); }
        }

        public FontFamily FontFamily
        {
            get { return ((FontFamily)(get_Value(SettingsManager.Setting_FontFamily))); }
        }

        public bool FontBold
        {
            get { return Convert.ToBoolean(get_Value(SettingsManager.Setting_FontBold)); }
        }

        public bool FontItalic
        {
            get { return Convert.ToBoolean(get_Value(SettingsManager.Setting_FontItalic)); }
        }

        public float FontSize
        {
            get { return Convert.ToSingle(Convert.ToSingle(get_Value(SettingsManager.Setting_FontSize))); }
        }

        public int MinimapSize
        {
            get { return Convert.ToInt32(get_Value(SettingsManager.Setting_MinimapSize)); }
        }

        public bool MinimapTeamColours
        {
            get { return Convert.ToBoolean(get_Value(SettingsManager.Setting_MinimapTeamColours)); }
        }

        public bool MinimapTeamColoursExceptFeatures
        {
            get { return Convert.ToBoolean(get_Value(SettingsManager.Setting_MinimapTeamColoursExceptFeatures)); }
        }

        public clsRGBA_sng MinimapCliffColour
        {
            get { return ((clsRGBA_sng)(get_Value(SettingsManager.Setting_MinimapCliffColour))); }
        }

        public clsRGBA_sng MinimapSelectedObjectsColour
        {
            get { return ((clsRGBA_sng)(get_Value(SettingsManager.Setting_MinimapSelectedObjectsColour))); }
        }

        public double FOVDefault
        {
            get { return Convert.ToDouble(get_Value(SettingsManager.Setting_FOVDefault)); }
        }

        public bool Mipmaps
        {
            get { return Convert.ToBoolean(get_Value(SettingsManager.Setting_Mipmaps)); }
        }

        public bool MipmapsHardware
        {
            get { return Convert.ToBoolean(get_Value(SettingsManager.Setting_MipmapsHardware)); }
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
        }

        public int TextureViewBPP
        {
            get { return Convert.ToInt32(get_Value(SettingsManager.Setting_TextureViewBPP)); }
        }

        public int MapViewDepth
        {
            get { return Convert.ToInt32(get_Value(SettingsManager.Setting_MapViewDepth)); }
        }

        public int TextureViewDepth
        {
            get { return Convert.ToInt32(get_Value(SettingsManager.Setting_TextureViewDepth)); }
        }

        public SimpleList<string> TilesetDirectories
        {
            get { return ((SimpleList<string>)(get_Value(SettingsManager.Setting_TilesetDirectories))); }
        }

        public SimpleList<string> ObjectDataDirectories
        {
            get { return ((SimpleList<string>)(get_Value(SettingsManager.Setting_ObjectDataDirectories))); }
        }

        public bool PickOrientation
        {
            get { return Convert.ToBoolean(get_Value(SettingsManager.Setting_PickOrientation)); }
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