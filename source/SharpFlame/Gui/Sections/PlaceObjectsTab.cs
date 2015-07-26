using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Appccelerate.EventBroker;
using Appccelerate.EventBroker.Handlers;
using Appccelerate.Events;
using Eto;
using Eto.Forms;
using Eto.Gl;
using Ninject;
using Ninject.Extensions.Logging;
using SharpFlame.Core;
using SharpFlame.Domain;
using SharpFlame.Domain.ObjData;
using SharpFlame.Mapping;
using SharpFlame.Mapping.Objects;
using SharpFlame.Mapping.Tools;
using SharpFlame.MouseTools;
using SharpFlame.Settings;
using Z.ExtensionMethods.ObjectExtensions;

namespace SharpFlame.Gui.Sections
{

	public class PlaceObjectGridViewItem
	{
		public PlaceObjectGridViewItem(UnitTypeBase obj)
		{
			string code = null;
			obj.GetCode(ref code);
			this.InternalName = code;
			this.InGameName = obj.GetName().Replace("*", "");
			this.UnitBaseType = obj;
		}

		public UnitTypeBase UnitBaseType { get; set; }

		public string InternalName { get; set; }
		public string InGameName { get; set; }
	}

	public class PlaceObjectGridView : GridView<PlaceObjectGridViewItem>
	{
	}

	public class PlaceObjectsTab : TabPage
	{
		public enum FilterType
		{
			Structs,
			Feature,
			Droids
		}

        [Inject]
        internal ToolOptions ToolOptions { get; set; }

		[Inject]
		internal SettingsManager SettingsManager { get; set; }

		[Inject]
		internal ILogger Logger { get; set; }

		[Inject]
		internal IEventBroker EventBroker { get; set; }

		[Inject]
		internal KeyboardManager KeyboardManager { get; set; }

		[Inject]
		internal ObjectManager ObjectManager { get; set; }

		private Button cmdPlaceOne;
		private Button cmdPlaceRow;
		private NumericUpDown nRotation;
		private CheckBox chkRandom;
		private CheckBox chkRotateFootprints;
		private CheckBox chkAutoWalls;
		private GroupBox grpPlayers;

		void ObjectDataDirectories_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			try
			{
				var result = new Result("Reloading object data.", false);

				// Just reload Object Data.
				this.ObjectManager.ObjectData = new ObjectData();
				foreach( var path in this.SettingsManager.ObjectDataDirectories )
				{
					if( !string.IsNullOrEmpty(path) )
					{
						result.Add(this.ObjectManager.ObjectData.LoadDirectory(path));
					}
				}
				
				if( result.HasProblems || result.HasWarnings )
				{
					App.StatusDialog = new Gui.Dialogs.Status(result);
					App.StatusDialog.Show();
				}
			}
			catch( Exception ex )
			{
				Logger.Error(ex, "Got an Exception while loading object data.");
			}
		}

		[EventSubscription(EventTopics.OnMapLoad, typeof(OnPublisher))]
		public void HandleMapLoad(Map args)
		{
			this.map = args;

			//load preset groups
			this.grpPlayers.Children.OfType<Button>()
				.ForEach(b =>
					{
						if( b.Text.StartsWith("P") )
						{
							var player = b.Text.Substring(1).ToInt32();
							b.Tag = this.map.UnitGroups[player];
						}
						else if( b.Text.StartsWith("Scav") )
						{
							b.Tag = this.map.ScavengerUnitGroup;
						}
					});
		}

		[EventSubscription(EventTopics.OnObjectManagerLoaded, typeof(OnPublisher))]
		public void HandleObjectManagerLoaded(object sender, EventArgs e)
		{
			this.RefreshGridViews();
		}

		public PlaceObjectsTab ()
        {
	        XomlReader.Load(this);

	        this.cmdPlaceOne.Image = Resources.Place;
	        this.cmdPlaceRow.Image = Resources.Line;

			this.SettingsManager.ObjectDataDirectories.CollectionChanged += ObjectDataDirectories_CollectionChanged;
			this.features = new FilterCollection<PlaceObjectGridViewItem>();
			this.structs = new FilterCollection<PlaceObjectGridViewItem>();
			this.droids = new FilterCollection<PlaceObjectGridViewItem>();
			
			gFeatures.Columns.Add(new GridColumn { HeaderText = "Internal Name", DataCell = new TextBoxCell("InternalName"), Editable = false, Sortable = true, AutoSize  = true});
			gFeatures.Columns.Add(new GridColumn { HeaderText = "In-Game Name", DataCell = new TextBoxCell("InGameName"), Editable = false, Sortable = true, AutoSize = true});
			gStructures.Columns.Add(new GridColumn { HeaderText = "Internal Name", DataCell = new TextBoxCell("InternalName"), Editable = false, Sortable = true });
			gStructures.Columns.Add(new GridColumn { HeaderText = "In-Game Name", DataCell = new TextBoxCell("InGameName"), Editable = false, Sortable = true });
			gDroids.Columns.Add(new GridColumn { HeaderText = "Internal Name", DataCell = new TextBoxCell("InternalName"), Editable = false, Sortable = true });
			gDroids.Columns.Add(new GridColumn { HeaderText = "In-Game Name", DataCell = new TextBoxCell("InGameName"), Editable = false, Sortable = true });
        }

		private Panel panDroids;
		private Panel panStructs;
		private Panel panFeatures;

		private PlaceObjectGridView gFeatures;
		private PlaceObjectGridView gStructures;
		private PlaceObjectGridView gDroids;

		private LinkButton cmdSelectAll;

		protected override void OnPreLoad(EventArgs e)
		{
			base.OnPreLoad(e);
			
			this.nRotation.Bind(n => n.Enabled, this.chkRandom.CheckedBinding.Convert(g => !g.Value));
			this.nRotation.ValueBinding.Bind(this.ToolOptions.PlaceObject, p => p.Rotation);
			this.chkAutoWalls.CheckedBinding.Bind(this.ToolOptions.PlaceObject, p => p.AutoWalls);
			this.chkRandom.CheckedBinding.Bind(this.ToolOptions.PlaceObject, p => p.RotationRandom);
			this.chkRotateFootprints.CheckedBinding.Bind(this.ToolOptions.PlaceObject, p => p.RotateFootprints);

			this.cmdSelectAll.TextBinding.BindDataContext<Button>(getValue: b =>
				{
					return "Select All {0} Units".FormatWith(b.Text);
				}, setValue: null, mode: DualBindingMode.OneWay, defaultGetValue:"");
			

		}

		public void RefreshGridViews()
		{
			var objFeatures = this.ObjectManager.ObjectData.FeatureTypes.GetItemsAsSimpleList()
				.ConvertAll(f => new PlaceObjectGridViewItem(f));

			this.features.Clear();
			this.features.AddRange(objFeatures);
			
			this.gFeatures.DataStore = features;


			var objStrcuts = this.ObjectManager.ObjectData.StructureTypes.GetItemsAsSimpleList()
				.ConvertAll(f => new PlaceObjectGridViewItem(f));

			this.structs.Clear();
			this.structs.AddRange(objStrcuts);

			this.gStructures.DataStore = this.structs;


			var objDroids = this.ObjectManager.ObjectData.DroidTemplates.GetItemsAsSimpleList()
				.ConvertAll(f => new PlaceObjectGridViewItem(f));

			this.droids.Clear();
			this.droids.AddRange(objDroids);

			this.gDroids.DataStore = this.droids;
		}

	    protected override void OnLoadComplete(EventArgs lcEventArgs)
        {
            base.OnLoadComplete(lcEventArgs);

		    this.Shown += (sender, args) =>
			    {
				    ToolOptions.MouseTool = MouseTool.Default;
			    };
        }

		void AnyPlayer_Click(object sender, EventArgs e)
		{
			if( this.map == null )
				return;

			this.grpPlayers.DataContext = sender;

			var button = sender as Button;
			var group = button.Tag.To<clsUnitGroup>();

			this.map.SelectedUnitGroup.Item = group;


			if( map.SelectedUnits.Count <= 0 )
				return;

			if( this.map.SelectedUnits.Count > 1 )
			{
				if( MessageBox.Show(this, "Change player of multiple objects?", MessageBoxButtons.YesNo) != DialogResult.Yes )
				{
					return;
				}
			}

			var objUnitGroup = new clsObjectUnitGroup()
				{
					Map = this.map,
					UnitGroup = group
				};
			
			this.map.SelectedUnitsAction(objUnitGroup);

			this.map.UndoStepCreate("Object Player Changed");

			if( this.SettingsManager.MinimapTeamColours )
			{
				this.EventBroker.RefreshMinimap(this);
			}

			this.EventBroker.DrawLater(this);
		}

		void AnyPlayer_PreLoad(object sender, EventArgs e)
		{
			var button = sender as Button;

			button.Bind(b => b.Enabled,
				Binding.Delegate(() => this.grpPlayers.DataContext != button,
				addChangeEvent: handlerToExecuteWhenSourceChanges => this.grpPlayers.DataContextChanged += handlerToExecuteWhenSourceChanges,
				removeChangeEvent: handlerToExecuteWhenSourceChanges => this.grpPlayers.DataContextChanged += handlerToExecuteWhenSourceChanges));

			button.Bind(b => b.BackgroundColor, Binding.Delegate(() =>
			{
				if( this.grpPlayers.DataContext == button )
				{
					return Eto.Drawing.Colors.SkyBlue;
				}
				return Eto.Drawing.Colors.Transparent;
			},
				addChangeEvent: h => this.grpPlayers.DataContextChanged += h,
				removeChangeEvent: h => this.grpPlayers.DataContextChanged += h));
		}

		void cmdSelectAll_Click(object sender, EventArgs e)
		{
			if( this.map == null ) return;

			if( !KeyboardManager.Keys[KeyboardKeys.Multiselect].Active )
			{
				this.map.SelectedUnits.Clear();
			}

			var unitGroup = this.map.SelectedUnitGroup.Item;

			foreach( var unit in this.map.Units )
			{
				if( unit.UnitGroup == unitGroup )
				{
					if( unit.TypeBase.Type != UnitType.Feature )
					{
						if( !unit.MapSelectedUnitLink.IsConnected )
						{
							unit.MapSelectedUnitLink.Connect(this.map.SelectedUnits);
						}
					}
				}
			}

			this.EventBroker.DrawLater(this);
		}

		void AnyGrid_SelectionChanged(object sender, EventArgs e)
		{
			var grid = sender as PlaceObjectGridView;
			var types = grid.SelectedItems
				.Select(gvi => gvi.UnitBaseType).ToList();
			
			this.ToolOptions.PlaceObject.SelectedObjectTypes = types;

			this.EventBroker.DrawLater(this);
		}

		void ToolSelection_Click(object sender, EventArgs e)
		{
			var button = sender as Button;

			var tool = button.Tag.To<MouseTool>();

			ToolOptions.MouseTool = tool;
			if( sender == this.cmdPlaceOne )
			{
				this.cmdPlaceOne.Enabled = false;
				this.cmdPlaceRow.Enabled = true;
				this.cmdPlaceOne.BackgroundColor = Eto.Drawing.Colors.Coral;
				this.cmdPlaceRow.BackgroundColor = Eto.Drawing.Colors.Transparent;
			}
			else if( sender == this.cmdPlaceRow )
			{
				this.cmdPlaceRow.Enabled = false;
				this.cmdPlaceOne.Enabled = true;
				this.cmdPlaceRow.BackgroundColor = Eto.Drawing.Colors.Coral;
				this.cmdPlaceOne.BackgroundColor = Eto.Drawing.Colors.Transparent;
			}
		}


		private FilterCollection<PlaceObjectGridViewItem> features;
		private FilterCollection<PlaceObjectGridViewItem> structs;
		private FilterCollection<PlaceObjectGridViewItem> droids;
		private Map map;

		void Filter_KeyUp(object sender, KeyEventArgs e)
		{
			var search = sender as SearchBox;

			var searchType = search.Tag.To<FilterType>();

			FilterCollection<PlaceObjectGridViewItem> collection = null;
			if( searchType == FilterType.Feature)
			{
				collection = features;
			}
			else if( searchType == FilterType.Structs )
			{
				collection = structs;
			}
			else if( searchType == FilterType.Droids )
			{
				collection = droids;
			}

			if( search.Text.IsNullOrWhiteSpace() )
			{
				collection.Filter = null;
				return;
			}

			var searchTokens = search.Text.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);

			collection.Filter = item =>
				{
					return searchTokens.Any(t => item.InGameName.IndexOf(t, StringComparison.OrdinalIgnoreCase) >= 0);
				};
		}


	}
}

