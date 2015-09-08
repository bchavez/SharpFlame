using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq.Expressions;
using Appccelerate.EventBroker;
using Appccelerate.EventBroker.Handlers;
using Eto.Forms;
using Newtonsoft.Json;
using Ninject.Extensions.Logging;
using OpenTK;
using SharpFlame.Core;
using SharpFlame.Core.Domain.Colors;

namespace SharpFlame.Settings
{
	public class SettingsManager : INotifyPropertyChanged
	{
		private readonly ILogger logger;

		public bool AutoSaveEnabled { get; set; }
		public bool AutoSaveCompress { get; set; }
		public UInt32 AutoSaveMinIntervalSeconds { get; set; }
		public UInt32 AutoSaveMinChanges { get; set; }
		public UInt32 UndoLimit { get; set; }
		public bool ShowOptionsAtStartup { get; set; }
		public bool DirectPointer { get; set; }
		private FontFamily fontFamily;

		public FontFamily FontFamily
		{
			get { return fontFamily; }
			set { SetField(ref fontFamily, value, () => FontFamily); }
		}

		private bool fontBold;

		public bool FontBold
		{
			get { return fontBold; }
			set { SetField(ref fontBold, value, () => FontBold); }
		}

		private bool fontItalic;

		public bool FontItalic
		{
			get { return fontItalic; }
			set { SetField(ref fontItalic, value, () => FontItalic); }
		}

		private float fontSize;

		public float FontSize
		{
			get { return fontSize; }
			set { SetField(ref fontSize, value, () => FontSize); }
		}

		public int MinimapSize { get; set; }
		public bool MinimapTeamColours { get; set; }
		public bool MinimapTeamColoursExceptFeatures { get; set; }
		public Rgba MinimapCliffColour { get; set; }
		public Rgba MinimapSelectedObjectsColour { get; set; }

		public string FOVDefaultStr
		{
			get { return this.FOVDefault.ToString(); }
			set { this.FOVDefault = double.Parse(value); }
		}

		public double FOVDefault { get; set; }
		public bool Mipmaps { get; set; }
		public bool MipmapsHardware { get; set; }

		public string openPath;

		public string OpenPath
		{
			get { return this.openPath; }
			set
			{
				new Uri(value);
				this.openPath = value;
			}
		}

		public string SavePath { get; set; }
		public int MapViewBPP { get; set; }
		public int TextureViewBPP { get; set; }
		public int MapViewDepth { get; set; }
		public int TextureViewDepth { get; set; }
		public ObservableCollection<string> TilesetDirectories { get; private set; }
		public ObservableCollection<string> ObjectDataDirectories { get; private set; }
		public bool PickOrientation { get; set; }

		public bool UpdateOnStartup { get; set; }

		[JsonIgnore]
		public Font Font { get; private set; }


		public event PropertyChangedEventHandler PropertyChanged = delegate { };

		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		protected bool SetField<T>(ref T field, T value, string propertyName)
		{
			if( EqualityComparer<T>.Default.Equals(field, value) ) return false;
			field = value;
			OnPropertyChanged(propertyName);
			return true;
		}

		protected virtual void OnPropertyChanged<T>(Expression<Func<T>> selectorExpression)
		{
			if( selectorExpression == null )
				throw new ArgumentNullException("selectorExpression");
			MemberExpression body = selectorExpression.Body as MemberExpression;
			if( body == null )
				throw new ArgumentException("The body must be a member expression");
			OnPropertyChanged(body.Member.Name);
		}

		protected bool SetField<T>(ref T field, T value, Expression<Func<T>> selectorExpression)
		{
			if( EqualityComparer<T>.Default.Equals(field, value) ) return false;
			field = value;
			OnPropertyChanged(selectorExpression);
			return true;
		}

		public SettingsManager(ILoggerFactory logFactory, KeyboardManager keyboardManager)
		{
			logger = logFactory.GetCurrentClassLogger();
			TilesetDirectories = new ObservableCollection<string>();
			ObjectDataDirectories = new ObservableCollection<string>();

			SetToDefaults(keyboardManager);
		}

		public void SetToDefaults(KeyboardManager keyboardManager)
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
			MinimapCliffColour = new Rgba(1.0F, 0.25F, 0.25F, 0.5F);
			MinimapSelectedObjectsColour = new Rgba(1.0F, 1.0F, 1.0F, 0.75F);
			FOVDefault = 30.0D / (50.0D * 900.0D);
			Mipmaps = true;
			MipmapsHardware = true;
			OpenPath = new Uri(AppDomain.CurrentDomain.BaseDirectory).ToString();
			SavePath = "";
			MapViewBPP = DisplayDevice.Default.BitsPerPixel;
			TextureViewBPP = DisplayDevice.Default.BitsPerPixel;
			MapViewDepth = 24;
			TextureViewDepth = 24;
			PickOrientation = true;
			UpdateOnStartup = true;

			//Remove "old" keys
			keyboardManager.RegisterClearAll();

			//interface controls
			keyboardManager.Register(CommandName.ObjectSelectTool, Keys.Escape);

			//selected unit controls
			keyboardManager.Register(CommandName.MoveObjects, Keys.M);
			keyboardManager.Register(CommandName.DeleteObjects, Keys.Delete);
			keyboardManager.Register(CommandName.Multiselect, Keys.Shift);

			//generalised controls
			keyboardManager.Register(CommandName.ViewSlow, Keys.R);
			keyboardManager.Register(CommandName.ViewFast, Keys.F);

			//picker controls
			keyboardManager.Register(CommandName.Picker, Keys.Control);

			//view controls
			keyboardManager.Register(CommandName.ShowTextures, Keys.F5);
			keyboardManager.Register(CommandName.ShowLighting, Keys.F8);
			keyboardManager.Register(CommandName.ShowWireframe, Keys.F6);
			keyboardManager.Register(CommandName.ShowObjects, Keys.F7);
			keyboardManager.Register(CommandName.ShowLabels, Keys.F4);
			keyboardManager.Register(CommandName.ViewMoveMode, Keys.F1);
			keyboardManager.Register(CommandName.ViewRotateMode, Keys.F2);
            keyboardManager.Register(CommandName.ViewMoveLeft, Keys.A, true);
            keyboardManager.Register(CommandName.ViewMoveRight, Keys.D, true);
            keyboardManager.Register(CommandName.ViewMoveForwards, Keys.W, true);
            keyboardManager.Register(CommandName.ViewMoveBackwards, Keys.S, true);
            keyboardManager.Register(CommandName.ViewMoveUp, Keys.E, true);
            keyboardManager.Register(CommandName.ViewMoveDown, Keys.C, true);
            keyboardManager.Register(CommandName.ViewZoomIn, Keys.Home, true);
            keyboardManager.Register(CommandName.ViewZoomOut, Keys.End, true);
            keyboardManager.Register(CommandName.ViewRotateLeft, Keys.Left, true);
            keyboardManager.Register(CommandName.ViewRotateRight, Keys.Right, true);
            keyboardManager.Register(CommandName.ViewRotateForwards, Keys.Up, true);
            keyboardManager.Register(CommandName.ViewRotateBackwards, Keys.Down, true);
            keyboardManager.Register(CommandName.ViewRotateUp, Keys.PageUp, true);
            keyboardManager.Register(CommandName.ViewRotateDown, Keys.PageDown, true);
            keyboardManager.Register(CommandName.ViewRollLeft, Keys.LeftBracket, true);
            keyboardManager.Register(CommandName.ViewRollRight, Keys.RightBracket, true);
            keyboardManager.Register(CommandName.ViewReset, Keys.Backspace);

            //texture controls
            keyboardManager.Register(CommandName.CounterClockwise, Keys.D1);
			keyboardManager.Register(CommandName.Clockwise, Keys.D2);
			keyboardManager.Register(CommandName.TextureFlip, Keys.D3);
			keyboardManager.Register(CommandName.TriangleFlip, Keys.D4);
			keyboardManager.Register(CommandName.GatewayDelete, Keys.D5);

			//undo controls
			keyboardManager.Register(CommandName.Undo, Keys.Z);
			keyboardManager.Register(CommandName.Redo, Keys.Y);
			keyboardManager.Register(CommandName.PositionLabel, Keys.P);

			// Vision Radius
			keyboardManager.Register(CommandName.VisionRadius6, Keys.Control | Keys.D1);
			keyboardManager.Register(CommandName.VisionRadius7, Keys.Control | Keys.D2);
			keyboardManager.Register(CommandName.VisionRadius8, Keys.Control | Keys.D3);
			keyboardManager.Register(CommandName.VisionRadius9, Keys.Control | Keys.D4);
			keyboardManager.Register(CommandName.VisionRadius10, Keys.Control | Keys.D5);
			keyboardManager.Register(CommandName.VisionRadius11, Keys.Control | Keys.D6);
			keyboardManager.Register(CommandName.VisionRadius12, Keys.Control | Keys.D7);
			keyboardManager.Register(CommandName.VisionRadius13, Keys.Control | Keys.D8);
			keyboardManager.Register(CommandName.VisionRadius14, Keys.Control | Keys.D9);
			keyboardManager.Register(CommandName.VisionRadius15, Keys.Control | Keys.D0);
		}

		public Result Save(string path)
		{
			var returnResult = new Result(string.Format("Writing settings to \"{0}\"", path), false);
			logger.Info("Writing settings to \"{0}\"", path);

			try
			{
				var json = JsonConvert.SerializeObject(this, Formatting.Indented);
				File.WriteAllText(path, json);
			}
			catch( Exception ex )
			{
				Debugger.Break();
				returnResult.ProblemAdd(string.Format("Got an exception: {0}.", ex.Message));
				logger.Error(ex, "Got an exception while saving the settings.");
			}

			return returnResult;
		}

		public Result Load(string path)
		{
			var returnResult = new Result(string.Format("Loading settings from \"{0}\"", path), false);
			logger.Info(string.Format("Loading settings from \"{0}\"", path));

			try
			{
				var txt = File.ReadAllText(path);
				JsonConvert.PopulateObject(txt, this);
			}
			catch( Exception ex )
			{
				returnResult.ProblemAdd(string.Format("Got an exception: {0}.", ex.Message));
				return returnResult;
			}

			return returnResult;
		}

		[EventSubscription(EventTopics.OnMapSave, typeof(OnPublisher))]
		public void HandleMapSaved(string path)
		{
			this.SavePath = path;
		}
	}
}