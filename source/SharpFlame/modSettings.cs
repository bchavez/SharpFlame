using System;
using System.Drawing;
using Microsoft.VisualBasic;

namespace SharpFlame
{
	public sealed class modSettings
	{
		
		public static clsOptionGroup Options_Settings = new clsOptionGroup();
		public static clsSettings InitializeSettings;
		public static clsSettings Settings;
		
		public static clsOption<bool> Setting_AutoSaveEnabled;
		public static clsOption<bool> Setting_AutoSaveCompress;
		public static clsOption<UInt32> Setting_AutoSaveMinInterval_s;
		public static clsOption<UInt32> Setting_AutoSaveMinChanges;
		public static clsOption<UInt32> Setting_UndoLimit;
		public static clsOption<bool> Setting_DirectoriesPrompt;
		public static clsOption<bool> Setting_DirectPointer;
		public static clsOption<FontFamily> Setting_FontFamily;
		public static clsOption<bool> Setting_FontBold;
		public static clsOption<bool> Setting_FontItalic;
		public class clsOption_FontSize : clsOption<Single>
		{
			
			public clsOption_FontSize(string saveKey, float defaultValue) : base(saveKey, defaultValue)
			{
			}
			
			public override bool IsValueValid(object value)
			{
				return System.Convert.ToSingle(value) >= 0.0F;
			}
		}
		public class clsOptionCreator_FontSize : clsOptionCreator<Single>
		{
			
			public override clsOption<float> Create()
			{
				return new clsOption_FontSize(SaveKey, DefaultValue);
			}
		}
		public static clsOption_FontSize Setting_FontSize;
		public class clsOption_MinimapSize : clsOption<int>
		{
			
			public clsOption_MinimapSize(string saveKey, int defaultValue) : base(saveKey, defaultValue)
			{
			}
			
			public override bool IsValueValid(object value)
			{
				int intValue = System.Convert.ToInt32(value);
				return intValue >= 0 & intValue <= modProgram.MinimapMaxSize;
			}
		}
		public class clsOptionCreator_MinimapSize : clsOptionCreator<int>
		{
			
			public override clsOption<int> Create()
			{
				return new clsOption_MinimapSize(SaveKey, DefaultValue);
			}
		}
		public static clsOption_MinimapSize Setting_MinimapSize;
		public static clsOption<bool> Setting_MinimapTeamColours;
		public static clsOption<bool> Setting_MinimapTeamColoursExceptFeatures;
		public static clsOption<clsRGBA_sng> Setting_MinimapCliffColour;
		public static clsOption<clsRGBA_sng> Setting_MinimapSelectedObjectsColour;
		public class clsOption_FOVDefault : clsOption<double>
		{
			
			public clsOption_FOVDefault(string saveKey, double defaultValue) : base(saveKey, defaultValue)
			{
			}
			
			public override bool IsValueValid(object value)
			{
				double dblValue = System.Convert.ToDouble(value);
				return dblValue >= 0.00005D & dblValue <= 0.005D;
			}
		}
		public class clsOptionCreator_FOVDefault : clsOptionCreator<double>
		{
			
			public override clsOption<double> Create()
			{
				return new clsOption_FOVDefault(SaveKey, DefaultValue);
			}
		}
		public static clsOption_FOVDefault Setting_FOVDefault;
		public static clsOption<bool> Setting_Mipmaps;
		public static clsOption<bool> Setting_MipmapsHardware;
		public static clsOption<string> Setting_OpenPath;
		public static clsOption<string> Setting_SavePath;
		public static clsOption<int> Setting_MapViewBPP;
		public static clsOption<int> Setting_TextureViewBPP;
		public static clsOption<int> Setting_MapViewDepth;
		public static clsOption<int> Setting_TextureViewDepth;
		public static clsOption<modLists.SimpleList<string>> Setting_TilesetDirectories;
		public static clsOption<modLists.SimpleList<string>> Setting_ObjectDataDirectories;
		public static clsOption<int> Setting_DefaultTilesetsPathNum;
		public static clsOption<int> Setting_DefaultObjectDataPathNum;
		public static clsOption<bool> Setting_PickOrientation;
		
		public class clsSettings : clsOptionProfile
		{
			
			public clsSettings() : base(Options_Settings)
			{
			}
			
public bool AutoSaveEnabled
			{
				get
				{
					return System.Convert.ToBoolean(this.get_Value(Setting_AutoSaveEnabled));
				}
			}
public bool AutoSaveCompress
			{
				get
				{
					return System.Convert.ToBoolean(this.get_Value(Setting_AutoSaveCompress));
				}
			}
public UInt32 AutoSaveMinInterval_s
			{
				get
				{
					return System.Convert.ToUInt32(this.get_Value(Setting_AutoSaveMinInterval_s));
				}
			}
public UInt32 AutoSaveMinChanges
			{
				get
				{
					return System.Convert.ToUInt32(this.get_Value(Setting_AutoSaveMinChanges));
				}
			}
public UInt32 UndoLimit
			{
				get
				{
					return System.Convert.ToUInt32(this.get_Value(Setting_UndoLimit));
				}
			}
public bool DirectoriesPrompt
			{
				get
				{
					return System.Convert.ToBoolean(this.get_Value(Setting_DirectoriesPrompt));
				}
			}
public bool DirectPointer
			{
				get
				{
					return System.Convert.ToBoolean(this.get_Value(Setting_DirectPointer));
				}
			}
public FontFamily FontFamily
			{
				get
				{
					return ((FontFamily) (this.get_Value(Setting_FontFamily)));
				}
			}
public bool FontBold
			{
				get
				{
					return System.Convert.ToBoolean(this.get_Value(Setting_FontBold));
				}
			}
public bool FontItalic
			{
				get
				{
					return System.Convert.ToBoolean(this.get_Value(Setting_FontItalic));
				}
			}
public float FontSize
			{
				get
				{
					return System.Convert.ToSingle( System.Convert.ToSingle(this.get_Value(Setting_FontSize)));
				}
			}
public int MinimapSize
			{
				get
				{
					return System.Convert.ToInt32(this.get_Value(Setting_MinimapSize));
				}
			}
public bool MinimapTeamColours
			{
				get
				{
					return System.Convert.ToBoolean(this.get_Value(Setting_MinimapTeamColours));
				}
			}
public bool MinimapTeamColoursExceptFeatures
			{
				get
				{
					return System.Convert.ToBoolean(this.get_Value(Setting_MinimapTeamColoursExceptFeatures));
				}
			}
public clsRGBA_sng MinimapCliffColour
			{
				get
				{
					return ((clsRGBA_sng) (this.get_Value(Setting_MinimapCliffColour)));
				}
			}
public clsRGBA_sng MinimapSelectedObjectsColour
			{
				get
				{
					return ((clsRGBA_sng) (this.get_Value(Setting_MinimapSelectedObjectsColour)));
				}
			}
public double FOVDefault
			{
				get
				{
					return System.Convert.ToDouble(this.get_Value(Setting_FOVDefault));
				}
			}
public bool Mipmaps
			{
				get
				{
					return System.Convert.ToBoolean(this.get_Value(Setting_Mipmaps));
				}
			}
public bool MipmapsHardware
			{
				get
				{
					return System.Convert.ToBoolean(this.get_Value(Setting_MipmapsHardware));
				}
			}
public string OpenPath
			{
				get
				{
					return System.Convert.ToString(this.get_Value(Setting_OpenPath));
				}
				set
				{
					set_Changes(Setting_OpenPath, new clsOptionProfile.clsChange<string>(value));
				}
			}
public string SavePath
			{
				get
				{
					return System.Convert.ToString(this.get_Value(Setting_SavePath));
				}
				set
				{
					set_Changes(Setting_SavePath, new clsOptionProfile.clsChange<string>(value));
				}
			}
public int MapViewBPP
			{
				get
				{
					return System.Convert.ToInt32(this.get_Value(Setting_MapViewBPP));
				}
			}
public int TextureViewBPP
			{
				get
				{
					return System.Convert.ToInt32(this.get_Value(Setting_TextureViewBPP));
				}
			}
public int MapViewDepth
			{
				get
				{
                    return System.Convert.ToInt32( this.get_Value( Setting_MapViewDepth ) );
				}
			}
public int TextureViewDepth
			{
				get
				{
                    return System.Convert.ToInt32( this.get_Value( Setting_TextureViewDepth ) );
				}
			}
public modLists.SimpleList<string> TilesetDirectories
			{
				get
				{
                    return ( (modLists.SimpleList<string>)( this.get_Value( Setting_TilesetDirectories ) ) );
				}
			}
public modLists.SimpleList<string> ObjectDataDirectories
			{
				get
				{
                    return ( (modLists.SimpleList<string>)( this.get_Value( Setting_ObjectDataDirectories ) ) );
				}
			}
public bool PickOrientation
			{
				get
				{
                    return System.Convert.ToBoolean( this.get_Value( Setting_PickOrientation ) );
				}
			}
			
			public Font MakeFont()
			{
				
				FontStyle style = FontStyle.Regular;
				if (FontBold)
				{
					style = (FontStyle) (style | FontStyle.Bold);
				}
				if (FontItalic)
				{
					style = (FontStyle) (style | FontStyle.Italic);
				}
				return new Font(FontFamily, FontSize, style, GraphicsUnit.Point);
			}
		}
		public class clsSettingsCreator : clsOptionProfileCreator
		{
			
			public override clsOptionProfile Create()
			{
				return new clsSettings();
			}
		}
		
		private static clsOption<T> CreateSetting<T>(string saveKey, T defaultValue)
		{
			
			clsOptionCreator<T> creator = new clsOptionCreator<T>();
			creator.SaveKey = saveKey;
			creator.DefaultValue = defaultValue;
			clsOption<T> result = creator.Create();
			Options_Settings.Options.Add(result.GroupLink);
			return result;
		}
		
		private static clsOption<T> CreateSetting<T>(clsOptionCreator<T> creator, string saveKey, T defaultValue)
		{
			
			creator.SaveKey = saveKey;
			creator.DefaultValue = defaultValue;
			clsOption<T> result = creator.Create();
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
			Setting_FontSize = (clsOption_FontSize) (CreateSetting<Single>(new clsOptionCreator_FontSize (), "FontSize", 20.0F));
			Setting_MinimapSize = (clsOption_MinimapSize) (CreateSetting<int>(new clsOptionCreator_MinimapSize (), "MinimapSize", 160));
			Setting_MinimapTeamColours = CreateSetting<bool>("MinimapTeamColours", true);
			Setting_MinimapTeamColoursExceptFeatures = CreateSetting<bool>("MinimapTeamColoursExceptFeatures", true);
			Setting_MinimapCliffColour = CreateSetting<clsRGBA_sng>("MinimapCliffColour", new clsRGBA_sng(1.0F, 0.25F, 0.25F, 0.5F));
			Setting_MinimapSelectedObjectsColour = CreateSetting<clsRGBA_sng>("MinimapSelectedObjectsColour", new clsRGBA_sng(1.0F, 1.0F, 1.0F, 0.75F));
			Setting_FOVDefault = (clsOption_FOVDefault) (CreateSetting<double>(new clsOptionCreator_FOVDefault (), "FOVDefault", 30.0D / (50.0D * 900.0D))); //screenVerticalSize/(screenDist*screenVerticalPixels)
			Setting_Mipmaps = CreateSetting<bool>("Mipmaps", false);
			Setting_MipmapsHardware = CreateSetting<bool>("MipmapsHardware", false);
			Setting_OpenPath = CreateSetting<string>("OpenPath", null);
			Setting_SavePath = CreateSetting<string>("SavePath", null);
			Setting_MapViewBPP = CreateSetting<int>("MapViewBPP", OpenTK.DisplayDevice.Default.BitsPerPixel);
			Setting_TextureViewBPP = CreateSetting<int>("TextureViewBPP", OpenTK.DisplayDevice.Default.BitsPerPixel);
			Setting_MapViewDepth = CreateSetting<int>("MapViewDepth", 24);
			Setting_TextureViewDepth = CreateSetting<int>("TextureViewDepth", 24);
			Setting_TilesetDirectories = CreateSetting<modLists.SimpleList<string>>("TilesetsPath", new modLists.SimpleList<string>());
			Setting_ObjectDataDirectories = CreateSetting<modLists.SimpleList<string>>("ObjectDataPath", new modLists.SimpleList<string>());
			Setting_DefaultTilesetsPathNum = CreateSetting<int>("DefaultTilesetsPathNum", -1);
			Setting_DefaultObjectDataPathNum = CreateSetting<int>("DefaultObjectDataPathNum", -1);
			Setting_PickOrientation = CreateSetting<bool>("PickOrientation", true);
		}
		
		public static clsResult Read_Settings(System.IO.StreamReader File, ref clsSettings Result)
		{
			clsResult ReturnResult = new clsResult("Reading settings");
			
			clsINIRead INIReader = new clsINIRead();
			ReturnResult.Take(INIReader.ReadFile(File));
			Result = new clsSettings();
			ReturnResult.Take(INIReader.RootSection.Translate(Result));
			foreach (clsINIRead.clsSection section in INIReader.Sections)
			{
				if (section.Name.ToLower() == "keyboardcontrols")
				{
					clsResult keyResults = new clsResult("Keyboard controls");
					keyResults.Take(section.Translate(modControls.KeyboardProfile));
					ReturnResult.Take(keyResults);
				}
			}
			
			return ReturnResult;
		}
		
		public static void UpdateSettings(clsSettings NewSettings)
		{
			bool FontChanged = default(bool);
			
			if (Settings == null)
			{
				FontChanged = true;
			}
			else
			{
				if (Settings.FontFamily == null)
				{
					FontChanged = NewSettings.FontFamily != null;
				}
				else
				{
					if (NewSettings.FontFamily == null)
					{
						FontChanged = true;
					}
					else
					{
						if (Settings.FontFamily.Name == NewSettings.FontFamily.Name 
							&& Settings.FontBold == NewSettings.FontBold 
							&& Settings.FontItalic == NewSettings.FontItalic 
							&& Settings.FontSize == NewSettings.FontSize)
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
			if (FontChanged)
			{
				SetFont(NewSettings.MakeFont());
			}
			
			Settings = NewSettings;
		}
		
		private static void SetFont(Font newFont)
		{
			
			if (modProgram.UnitLabelFont != null)
			{
				modProgram.UnitLabelFont.Deallocate();
			}
			modProgram.UnitLabelFont = modMain.frmMainInstance.MapView.CreateGLFont(newFont);
		}
		
		public static clsResult Settings_Write()
		{
			clsResult ReturnResult = new clsResult("Writing settings to " + System.Convert.ToString(ControlChars.Quote) + modProgram.SettingsPath + System.Convert.ToString(ControlChars.Quote));
			
#if !Portable
			if (!System.IO.Directory.Exists(modProgram.MyDocumentsProgramPath))
			{
				try
				{
					System.IO.Directory.CreateDirectory(modProgram.MyDocumentsProgramPath);
				}
				catch (Exception ex)
				{
					ReturnResult.ProblemAdd("Unable to create folder " + System.Convert.ToString(ControlChars.Quote) + modProgram.MyDocumentsProgramPath + System.Convert.ToString(ControlChars.Quote) + ": " + ex.Message);
					return ReturnResult;
				}
			}
#endif
			
			clsINIWrite INI_Settings = default(clsINIWrite);
			
			try
			{
				INI_Settings = clsINIWrite.CreateFile(System.IO.File.Create(modProgram.SettingsPath));
			}
			catch (Exception ex)
			{
				ReturnResult.ProblemAdd(ex.Message);
				return ReturnResult;
			}
			
			ReturnResult.Take(Serialize_Settings(INI_Settings));
			INI_Settings.File.Close();
			
			return ReturnResult;
		}
		
		private static clsResult Serialize_Settings(clsINIWrite File)
		{
			clsResult ReturnResult = new clsResult("Serializing settings");
			
			ReturnResult.Take(Settings.INIWrite(File));
			if (modControls.KeyboardProfile.IsAnythingChanged)
			{
				File.SectionName_Append("KeyboardControls");
				ReturnResult.Take(modControls.KeyboardProfile.INIWrite(File));
			}
			
			return ReturnResult;
		}
		
		public static clsResult Settings_Load(ref clsSettings Result)
		{
			clsResult ReturnResult = new clsResult("Loading settings from " + System.Convert.ToString(ControlChars.Quote) + modProgram.SettingsPath + System.Convert.ToString(ControlChars.Quote));
			
			System.IO.StreamReader File_Settings = default(System.IO.StreamReader);
			try
			{
				File_Settings = new System.IO.StreamReader(modProgram.SettingsPath);
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
	
}
