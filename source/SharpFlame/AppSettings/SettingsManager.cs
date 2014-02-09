using System;
using System.Drawing;
using System.IO;
using Microsoft.VisualBasic;
using NLog;
using OpenTK;
using SharpFlame.Collections;
using SharpFlame.Colors;
using SharpFlame.FileIO;
using SharpFlame.FileIO.Ini;

namespace SharpFlame.AppSettings
{
    public sealed class SettingsManager
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

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
            OptionCreator<T> creator = new OptionCreator<T>();
            creator.SaveKey = saveKey;
            creator.DefaultValue = defaultValue;
            Option<T> result = creator.Create();
            Options_Settings.Options.Add(result.GroupLink);
            return result;
        }

        private static Option<T> CreateSetting<T>(OptionCreator<T> creator, string saveKey, T defaultValue)
        {
            creator.SaveKey = saveKey;
            creator.DefaultValue = defaultValue;
            Option<T> result = creator.Create();
            Options_Settings.Options.Add(result.GroupLink);
            return result;
        }

        public static void CreateSettingOptions()
        {
            Setting_AutoSaveEnabled = CreateSetting<bool>("AutoSave", true);
            Setting_AutoSaveCompress = CreateSetting<bool>("AutoSaveCompress", false);
            Setting_AutoSaveMinInterval_s = CreateSetting<UInt32>("AutoSaveMinInterval", 180U);
            Setting_AutoSaveMinChanges = CreateSetting<UInt32>("AutoSaveMinChanges", 20U);
            Setting_UndoLimit = CreateSetting<UInt32>("UndoLimit", 256U);
            Setting_DirectoriesPrompt = CreateSetting<bool>("DirectoriesPrompt", true);
            Setting_DirectPointer = CreateSetting<bool>("DirectPointer", true);
            Setting_FontFamily = CreateSetting<FontFamily>("FontFamily", FontFamily.GenericSerif);
            Setting_FontBold = CreateSetting<bool>("FontBold", true);
            Setting_FontItalic = CreateSetting<bool>("FontItalic", false);
            Setting_FontSize = (OptionFontSize)(CreateSetting<Single>(new OptionCreatorFontSize(), "FontSize", 20.0F));
            Setting_MinimapSize = (OptionMinimapSize)(CreateSetting<int>(new OptionCreatorMinimapSize(), "MinimapSize", 160));
            Setting_MinimapTeamColours = CreateSetting<bool>("MinimapTeamColours", true);
            Setting_MinimapTeamColoursExceptFeatures = CreateSetting<bool>("MinimapTeamColoursExceptFeatures", true);
            Setting_MinimapCliffColour = CreateSetting<clsRGBA_sng>("MinimapCliffColour", new clsRGBA_sng(1.0F, 0.25F, 0.25F, 0.5F));
            Setting_MinimapSelectedObjectsColour = CreateSetting<clsRGBA_sng>("MinimapSelectedObjectsColour", new clsRGBA_sng(1.0F, 1.0F, 1.0F, 0.75F));
            Setting_FOVDefault = (OptionFovDefault)(CreateSetting<double>(new OptionCreatorFovDefault(), "FOVDefault", 30.0D / (50.0D * 900.0D)));
            //screenVerticalSize/(screenDist*screenVerticalPixels)
            Setting_Mipmaps = CreateSetting<bool>("Mipmaps", false);
            Setting_MipmapsHardware = CreateSetting<bool>("MipmapsHardware", false);
            Setting_OpenPath = CreateSetting<string>("OpenPath", null);
            Setting_SavePath = CreateSetting<string>("SavePath", null);
            Setting_MapViewBPP = CreateSetting<int>("MapViewBPP", DisplayDevice.Default.BitsPerPixel);
            Setting_TextureViewBPP = CreateSetting<int>("TextureViewBPP", DisplayDevice.Default.BitsPerPixel);
            Setting_MapViewDepth = CreateSetting<int>("MapViewDepth", 24);
            Setting_TextureViewDepth = CreateSetting<int>("TextureViewDepth", 24);
            Setting_TilesetDirectories = CreateSetting<SimpleList<string>>("TilesetsPath", new SimpleList<string>());
            Setting_ObjectDataDirectories = CreateSetting<SimpleList<string>>("ObjectDataPath", new SimpleList<string>());
            Setting_DefaultTilesetsPathNum = CreateSetting<int>("DefaultTilesetsPathNum", -1);
            Setting_DefaultObjectDataPathNum = CreateSetting<int>("DefaultObjectDataPathNum", -1);
            Setting_PickOrientation = CreateSetting<bool>("PickOrientation", true);
        }

        public static clsResult Read_Settings(StreamReader File, ref clsSettings Result)
        {
            clsResult ReturnResult = new clsResult("Reading settings", false);
            logger.Info ("Reading settings");

            IniReader INIReader = new IniReader();
            ReturnResult.Take(INIReader.ReadFile(File));
            Result = new clsSettings();
            ReturnResult.Take(INIReader.RootSection.Translate(Result));
            foreach ( Section section in INIReader.Sections )
            {
                if ( section.Name.ToLower() == "keyboardcontrols" )
                {
                    clsResult keyResults = new clsResult("Keyboard controls", false);
                    logger.Debug ("Reading keyboard controls");
                    keyResults.Take(section.Translate(KeyboardManager.KeyboardProfile));
                    ReturnResult.Take(keyResults);
                }
            }

            return ReturnResult;
        }

        public static void UpdateSettings(clsSettings NewSettings)
        {
            bool FontChanged = default(bool);

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
            clsResult ReturnResult =
                new clsResult("Writing settings to " + Convert.ToString(ControlChars.Quote) + App.SettingsPath +
                              Convert.ToString(ControlChars.Quote), false);
            logger.Info ("Writing settings to " + Convert.ToString (ControlChars.Quote) + App.SettingsPath +
                Convert.ToString (ControlChars.Quote));

#if !Portable
            if ( !Directory.Exists(App.MyDocumentsProgramPath) )
            {
                try
                {
                    Directory.CreateDirectory(App.MyDocumentsProgramPath);
                }
                catch ( Exception ex )
                {
                    ReturnResult.ProblemAdd("Unable to create folder " + Convert.ToString(ControlChars.Quote) + App.MyDocumentsProgramPath +
                                            Convert.ToString(ControlChars.Quote) + ": " + ex.Message);
                    return ReturnResult;
                }
            }
#endif

            IniWriter INI_Settings = default(IniWriter);

            try
            {
                INI_Settings = IniWriter.CreateFile(File.Create(App.SettingsPath));
            }
            catch ( Exception ex )
            {
                ReturnResult.ProblemAdd(ex.Message);
                return ReturnResult;
            }

            ReturnResult.Take(Serialize_Settings(INI_Settings));
            INI_Settings.File.Close();

            return ReturnResult;
        }

        private static clsResult Serialize_Settings(IniWriter File)
        {
            clsResult ReturnResult = new clsResult("Serializing settings", false);
            logger.Info ("Serializing settings");

            ReturnResult.Take(Settings.INIWrite(File));
            if ( KeyboardManager.KeyboardProfile.IsAnythingChanged )
            {
                File.AppendSectionName("KeyboardControls");
                ReturnResult.Take(KeyboardManager.KeyboardProfile.INIWrite(File));
            }

            return ReturnResult;
        }

        public static clsResult Settings_Load(ref clsSettings Result)
        {
            clsResult ReturnResult =
                new clsResult("Loading settings from " + Convert.ToString(ControlChars.Quote) + App.SettingsPath +
                              Convert.ToString(ControlChars.Quote), false);
            logger.Info ("Loading settings from " + Convert.ToString (ControlChars.Quote) + App.SettingsPath +
                Convert.ToString (ControlChars.Quote));

            StreamReader File_Settings = default(StreamReader);
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
            FontStyle style = FontStyle.Regular;
            if ( FontBold )
            {
                style = (FontStyle)(style | FontStyle.Bold);
            }
            if ( FontItalic )
            {
                style = (FontStyle)(style | FontStyle.Italic);
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
            double dblValue = Convert.ToDouble(value);
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
            int intValue = Convert.ToInt32(value);
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