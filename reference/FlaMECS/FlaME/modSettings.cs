namespace FlaME
{
    using Microsoft.VisualBasic.CompilerServices;
    using OpenTK;
    using System;
    using System.Collections;
    using System.Drawing;
    using System.IO;

    [StandardModule]
    public sealed class modSettings
    {
        public static clsSettings InitializeSettings;
        public static clsOptionGroup Options_Settings = new clsOptionGroup();
        public static clsOption<bool> Setting_AutoSaveCompress;
        public static clsOption<bool> Setting_AutoSaveEnabled;
        public static clsOption<uint> Setting_AutoSaveMinChanges;
        public static clsOption<uint> Setting_AutoSaveMinInterval_s;
        public static clsOption<int> Setting_DefaultObjectDataPathNum;
        public static clsOption<int> Setting_DefaultTilesetsPathNum;
        public static clsOption<bool> Setting_DirectoriesPrompt;
        public static clsOption<bool> Setting_DirectPointer;
        public static clsOption<bool> Setting_FontBold;
        public static clsOption<FontFamily> Setting_FontFamily;
        public static clsOption<bool> Setting_FontItalic;
        public static clsOption_FontSize Setting_FontSize;
        public static clsOption_FOVDefault Setting_FOVDefault;
        public static clsOption<int> Setting_MapViewBPP;
        public static clsOption<int> Setting_MapViewDepth;
        public static clsOption<clsRGBA_sng> Setting_MinimapCliffColour;
        public static clsOption<clsRGBA_sng> Setting_MinimapSelectedObjectsColour;
        public static clsOption_MinimapSize Setting_MinimapSize;
        public static clsOption<bool> Setting_MinimapTeamColours;
        public static clsOption<bool> Setting_MinimapTeamColoursExceptFeatures;
        public static clsOption<bool> Setting_Mipmaps;
        public static clsOption<bool> Setting_MipmapsHardware;
        public static clsOption<modLists.SimpleList<string>> Setting_ObjectDataDirectories;
        public static clsOption<string> Setting_OpenPath;
        public static clsOption<bool> Setting_PickOrientation;
        public static clsOption<string> Setting_SavePath;
        public static clsOption<int> Setting_TextureViewBPP;
        public static clsOption<int> Setting_TextureViewDepth;
        public static clsOption<modLists.SimpleList<string>> Setting_TilesetDirectories;
        public static clsOption<uint> Setting_UndoLimit;
        public static clsSettings Settings;

        private static clsOption<T> CreateSetting<T>(string saveKey, T defaultValue)
        {
            clsOption<T> option2 = new clsOptionCreator<T> { SaveKey = saveKey, DefaultValue = defaultValue }.Create();
            Options_Settings.Options.Add(option2.GroupLink);
            return option2;
        }

        private static clsOption<T> CreateSetting<T>(clsOptionCreator<T> creator, string saveKey, T defaultValue)
        {
            creator.SaveKey = saveKey;
            creator.DefaultValue = defaultValue;
            clsOption<T> option2 = creator.Create();
            Options_Settings.Options.Add(option2.GroupLink);
            return option2;
        }

        public static void CreateSettingOptions()
        {
            Setting_AutoSaveEnabled = CreateSetting<bool>("AutoSave", true);
            Setting_AutoSaveCompress = CreateSetting<bool>("AutoSaveCompress", false);
            Setting_AutoSaveMinInterval_s = CreateSetting<uint>("AutoSaveMinInterval", 180);
            Setting_AutoSaveMinChanges = CreateSetting<uint>("AutoSaveMinChanges", 20);
            Setting_UndoLimit = CreateSetting<uint>("UndoLimit", 0x100);
            Setting_DirectoriesPrompt = CreateSetting<bool>("DirectoriesPrompt", true);
            Setting_DirectPointer = CreateSetting<bool>("DirectPointer", true);
            Setting_FontFamily = CreateSetting<FontFamily>("FontFamily", FontFamily.GenericSerif);
            Setting_FontBold = CreateSetting<bool>("FontBold", true);
            Setting_FontItalic = CreateSetting<bool>("FontItalic", false);
            Setting_FontSize = (clsOption_FontSize) CreateSetting<float>(new clsOptionCreator_FontSize(), "FontSize", 20f);
            Setting_MinimapSize = (clsOption_MinimapSize) CreateSetting<int>(new clsOptionCreator_MinimapSize(), "MinimapSize", 160);
            Setting_MinimapTeamColours = CreateSetting<bool>("MinimapTeamColours", true);
            Setting_MinimapTeamColoursExceptFeatures = CreateSetting<bool>("MinimapTeamColoursExceptFeatures", true);
            Setting_MinimapCliffColour = CreateSetting<clsRGBA_sng>("MinimapCliffColour", new clsRGBA_sng(1f, 0.25f, 0.25f, 0.5f));
            Setting_MinimapSelectedObjectsColour = CreateSetting<clsRGBA_sng>("MinimapSelectedObjectsColour", new clsRGBA_sng(1f, 1f, 1f, 0.75f));
            Setting_FOVDefault = (clsOption_FOVDefault) CreateSetting<double>(new clsOptionCreator_FOVDefault(), "FOVDefault", 0.00066666666666666664);
            Setting_Mipmaps = CreateSetting<bool>("Mipmaps", false);
            Setting_MipmapsHardware = CreateSetting<bool>("MipmapsHardware", false);
            Setting_OpenPath = CreateSetting<string>("OpenPath", null);
            Setting_SavePath = CreateSetting<string>("SavePath", null);
            Setting_MapViewBPP = CreateSetting<int>("MapViewBPP", DisplayDevice.Default.BitsPerPixel);
            Setting_TextureViewBPP = CreateSetting<int>("TextureViewBPP", DisplayDevice.Default.BitsPerPixel);
            Setting_MapViewDepth = CreateSetting<int>("MapViewDepth", 0x18);
            Setting_TextureViewDepth = CreateSetting<int>("TextureViewDepth", 0x18);
            Setting_TilesetDirectories = CreateSetting<modLists.SimpleList<string>>("TilesetsPath", new modLists.SimpleList<string>());
            Setting_ObjectDataDirectories = CreateSetting<modLists.SimpleList<string>>("ObjectDataPath", new modLists.SimpleList<string>());
            Setting_DefaultTilesetsPathNum = CreateSetting<int>("DefaultTilesetsPathNum", -1);
            Setting_DefaultObjectDataPathNum = CreateSetting<int>("DefaultObjectDataPathNum", -1);
            Setting_PickOrientation = CreateSetting<bool>("PickOrientation", true);
        }

        public static clsResult Read_Settings(StreamReader File, ref clsSettings Result)
        {
            IEnumerator enumerator;
            clsResult result2 = new clsResult("Reading settings");
            clsINIRead read = new clsINIRead();
            result2.Take(read.ReadFile(File));
            Result = new clsSettings();
            result2.Take(read.RootSection.Translate(Result));
            try
            {
                enumerator = read.Sections.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    clsINIRead.clsSection current = (clsINIRead.clsSection) enumerator.Current;
                    if (current.Name.ToLower() == "keyboardcontrols")
                    {
                        clsResult resultToMerge = new clsResult("Keyboard controls");
                        resultToMerge.Take(current.Translate(modControls.KeyboardProfile));
                        result2.Take(resultToMerge);
                    }
                }
            }
            finally
            {
                if (enumerator is IDisposable)
                {
                    (enumerator as IDisposable).Dispose();
                }
            }
            return result2;
        }

        private static clsResult Serialize_Settings(clsINIWrite File)
        {
            clsResult result = new clsResult("Serializing settings");
            result.Take(Settings.INIWrite(File));
            if (modControls.KeyboardProfile.IsAnythingChanged)
            {
                File.SectionName_Append("KeyboardControls");
                result.Take(modControls.KeyboardProfile.INIWrite(File));
            }
            return result;
        }

        private static void SetFont(Font newFont)
        {
            if (modProgram.UnitLabelFont != null)
            {
                modProgram.UnitLabelFont.Deallocate();
            }
            modProgram.UnitLabelFont = modMain.frmMainInstance.MapView.CreateGLFont(newFont);
        }

        public static clsResult Settings_Load(ref clsSettings Result)
        {
            StreamReader reader;
            clsResult result = new clsResult("Loading settings from \"" + modProgram.SettingsPath + "\"");
            try
            {
                reader = new StreamReader(modProgram.SettingsPath);
            }
            catch (Exception exception1)
            {
                ProjectData.SetProjectError(exception1);
                Result = new clsSettings();
                clsResult result2 = result;
                ProjectData.ClearProjectError();
                return result2;
            }
            result.Take(Read_Settings(reader, ref Result));
            reader.Close();
            return result;
        }

        public static clsResult Settings_Write()
        {
            clsINIWrite write;
            clsResult result = new clsResult("Writing settings to \"" + modProgram.SettingsPath + "\"");
            try
            {
                write = clsINIWrite.CreateFile(File.Create(modProgram.SettingsPath));
            }
            catch (Exception exception1)
            {
                ProjectData.SetProjectError(exception1);
                Exception exception = exception1;
                result.ProblemAdd(exception.Message);
                clsResult result2 = result;
                ProjectData.ClearProjectError();
                return result2;
            }
            result.Take(Serialize_Settings(write));
            write.File.Close();
            return result;
        }

        public static void UpdateSettings(clsSettings NewSettings)
        {
            bool flag;
            if (Settings == null)
            {
                flag = true;
            }
            else if (Settings.FontFamily == null)
            {
                flag = NewSettings.FontFamily != null;
            }
            else if (NewSettings.FontFamily == null)
            {
                flag = true;
            }
            else if ((((Settings.FontFamily.Name == NewSettings.FontFamily.Name) & (Settings.FontBold == NewSettings.FontBold)) & (Settings.FontItalic == NewSettings.FontItalic)) & (Settings.FontSize == NewSettings.FontSize))
            {
                flag = false;
            }
            else
            {
                flag = true;
            }
            if (flag)
            {
                SetFont(NewSettings.MakeFont());
            }
            Settings = NewSettings;
        }

        public class clsOption_FontSize : clsOption<float>
        {
            public clsOption_FontSize(string saveKey, float defaultValue) : base(saveKey, defaultValue)
            {
            }

            public override bool IsValueValid(object value)
            {
                return (Conversions.ToSingle(value) >= 0f);
            }
        }

        public class clsOption_FOVDefault : clsOption<double>
        {
            public clsOption_FOVDefault(string saveKey, double defaultValue) : base(saveKey, defaultValue)
            {
            }

            public override bool IsValueValid(object value)
            {
                double num = Conversions.ToDouble(value);
                return ((num >= 5E-05) & (num <= 0.005));
            }
        }

        public class clsOption_MinimapSize : clsOption<int>
        {
            public clsOption_MinimapSize(string saveKey, int defaultValue) : base(saveKey, defaultValue)
            {
            }

            public override bool IsValueValid(object value)
            {
                int num = Conversions.ToInteger(value);
                return ((num >= 0) & (num <= 0x200));
            }
        }

        public class clsOptionCreator_FontSize : clsOptionCreator<float>
        {
            public override clsOption<float> Create()
            {
                return new modSettings.clsOption_FontSize(base.SaveKey, base.DefaultValue);
            }
        }

        public class clsOptionCreator_FOVDefault : clsOptionCreator<double>
        {
            public override clsOption<double> Create()
            {
                return new modSettings.clsOption_FOVDefault(base.SaveKey, base.DefaultValue);
            }
        }

        public class clsOptionCreator_MinimapSize : clsOptionCreator<int>
        {
            public override clsOption<int> Create()
            {
                return new modSettings.clsOption_MinimapSize(base.SaveKey, base.DefaultValue);
            }
        }

        public class clsSettings : clsOptionProfile
        {
            public clsSettings() : base(modSettings.Options_Settings)
            {
            }

            public Font MakeFont()
            {
                FontStyle regular = FontStyle.Regular;
                if (this.FontBold)
                {
                    regular |= FontStyle.Bold;
                }
                if (this.FontItalic)
                {
                    regular |= FontStyle.Italic;
                }
                return new Font(this.FontFamily, this.FontSize, regular, GraphicsUnit.Point);
            }

            public bool AutoSaveCompress
            {
                get
                {
                    return Conversions.ToBoolean(this.get_Value(modSettings.Setting_AutoSaveCompress));
                }
            }

            public bool AutoSaveEnabled
            {
                get
                {
                    return Conversions.ToBoolean(this.get_Value(modSettings.Setting_AutoSaveEnabled));
                }
            }

            public uint AutoSaveMinChanges
            {
                get
                {
                    return Conversions.ToUInteger(this.get_Value(modSettings.Setting_AutoSaveMinChanges));
                }
            }

            public uint AutoSaveMinInterval_s
            {
                get
                {
                    return Conversions.ToUInteger(this.get_Value(modSettings.Setting_AutoSaveMinInterval_s));
                }
            }

            public bool DirectoriesPrompt
            {
                get
                {
                    return Conversions.ToBoolean(this.get_Value(modSettings.Setting_DirectoriesPrompt));
                }
            }

            public bool DirectPointer
            {
                get
                {
                    return Conversions.ToBoolean(this.get_Value(modSettings.Setting_DirectPointer));
                }
            }

            public bool FontBold
            {
                get
                {
                    return Conversions.ToBoolean(this.get_Value(modSettings.Setting_FontBold));
                }
            }

            public System.Drawing.FontFamily FontFamily
            {
                get
                {
                    return (System.Drawing.FontFamily) this.get_Value(modSettings.Setting_FontFamily);
                }
            }

            public bool FontItalic
            {
                get
                {
                    return Conversions.ToBoolean(this.get_Value(modSettings.Setting_FontItalic));
                }
            }

            public float FontSize
            {
                get
                {
                    return Conversions.ToSingle(this.get_Value(modSettings.Setting_FontSize));
                }
            }

            public double FOVDefault
            {
                get
                {
                    return Conversions.ToDouble(this.get_Value(modSettings.Setting_FOVDefault));
                }
            }

            public int MapViewBPP
            {
                get
                {
                    return Conversions.ToInteger(this.get_Value(modSettings.Setting_MapViewBPP));
                }
            }

            public int MapViewDepth
            {
                get
                {
                    return Conversions.ToInteger(this.get_Value(modSettings.Setting_MapViewDepth));
                }
            }

            public clsRGBA_sng MinimapCliffColour
            {
                get
                {
                    return (clsRGBA_sng) this.get_Value(modSettings.Setting_MinimapCliffColour);
                }
            }

            public clsRGBA_sng MinimapSelectedObjectsColour
            {
                get
                {
                    return (clsRGBA_sng) this.get_Value(modSettings.Setting_MinimapSelectedObjectsColour);
                }
            }

            public int MinimapSize
            {
                get
                {
                    return Conversions.ToInteger(this.get_Value(modSettings.Setting_MinimapSize));
                }
            }

            public bool MinimapTeamColours
            {
                get
                {
                    return Conversions.ToBoolean(this.get_Value(modSettings.Setting_MinimapTeamColours));
                }
            }

            public bool MinimapTeamColoursExceptFeatures
            {
                get
                {
                    return Conversions.ToBoolean(this.get_Value(modSettings.Setting_MinimapTeamColoursExceptFeatures));
                }
            }

            public bool Mipmaps
            {
                get
                {
                    return Conversions.ToBoolean(this.get_Value(modSettings.Setting_Mipmaps));
                }
            }

            public bool MipmapsHardware
            {
                get
                {
                    return Conversions.ToBoolean(this.get_Value(modSettings.Setting_MipmapsHardware));
                }
            }

            public modLists.SimpleList<string> ObjectDataDirectories
            {
                get
                {
                    return (modLists.SimpleList<string>) this.get_Value(modSettings.Setting_ObjectDataDirectories);
                }
            }

            public string OpenPath
            {
                get
                {
                    return Conversions.ToString(this.get_Value(modSettings.Setting_OpenPath));
                }
                set
                {
                    this.set_Changes(modSettings.Setting_OpenPath, new clsOptionProfile.clsChange<string>(value));
                }
            }

            public bool PickOrientation
            {
                get
                {
                    return Conversions.ToBoolean(this.get_Value(modSettings.Setting_PickOrientation));
                }
            }

            public string SavePath
            {
                get
                {
                    return Conversions.ToString(this.get_Value(modSettings.Setting_SavePath));
                }
                set
                {
                    this.set_Changes(modSettings.Setting_SavePath, new clsOptionProfile.clsChange<string>(value));
                }
            }

            public int TextureViewBPP
            {
                get
                {
                    return Conversions.ToInteger(this.get_Value(modSettings.Setting_TextureViewBPP));
                }
            }

            public int TextureViewDepth
            {
                get
                {
                    return Conversions.ToInteger(this.get_Value(modSettings.Setting_TextureViewDepth));
                }
            }

            public modLists.SimpleList<string> TilesetDirectories
            {
                get
                {
                    return (modLists.SimpleList<string>) this.get_Value(modSettings.Setting_TilesetDirectories);
                }
            }

            public uint UndoLimit
            {
                get
                {
                    return Conversions.ToUInteger(this.get_Value(modSettings.Setting_UndoLimit));
                }
            }
        }

        public class clsSettingsCreator : clsOptionProfileCreator
        {
            public override clsOptionProfile Create()
            {
                return new modSettings.clsSettings();
            }
        }
    }
}

