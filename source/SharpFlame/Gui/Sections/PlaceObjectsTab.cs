using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Appccelerate.EventBroker;
using Appccelerate.EventBroker.Handlers;
using Appccelerate.Events;
using Eto.Forms;
using Eto.Gl;
using Ninject;
using Ninject.Extensions.Logging;
using SharpFlame.Core;
using SharpFlame.Domain;
using SharpFlame.Domain.ObjData;
using SharpFlame.MouseTools;
using SharpFlame.Settings;
using Z.ExtensionMethods;
using Z.ExtensionMethods.Object;

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

		private Button cmdPlaceOne;
		private Button cmdPlaceRow;
		private NumericUpDown nRotation;
		private CheckBox chkRandom;
		private CheckBox chkRotateFootprints;
		private CheckBox chkAutoWalls;

		void ObjectDataDirectories_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			try
			{
				var result = new Result("Reloading object data.", false);

				// Just reload Object Data.
				App.ObjectData = new ObjectData();
				foreach( var path in this.SettingsManager.ObjectDataDirectories )
				{
					if( !string.IsNullOrEmpty(path) )
					{
						result.Add(App.ObjectData.LoadDirectory(path));
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

		[EventSubscription(EventTopics.OnOpenGLInitalized,typeof(OnPublisher))]
		public void HandleOpenGlInitalized(object sender, EventArgs<GLSurface> gl)
		{
			// Load Object Data.
			foreach( var path in this.SettingsManager.ObjectDataDirectories )
			{
				if( !string.IsNullOrEmpty(path) )
				{
					SharpFlameApplication.InitializeResult.Add(App.ObjectData.LoadDirectory(path));
				}
			}
			RefreshGridViews();
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
			
		
	        /*PlayerSelector playerSelector;
            if (this.Platform.IsWinForms)
            {
                playerSelector = new PlayerSelectorWinforms (Constants.PlayerCountMax);
            } else
            {
                playerSelector = new PlayerSelector (Constants.PlayerCountMax);
            }
            playerSelector.SelectedPlayer = "S";

            var topLayout = new DynamicLayout ();
            var nLayout1 = new DynamicLayout { Padding = Padding.Empty };
            nLayout1.AddRow(new Label { Text = "Player", VerticalAlign = VerticalAlign.Middle }, 
                            playerSelector, 
                            TableLayout.AutoSized(new Button { Text = "Select Player Objects" }, centered: true)
            );
            nLayout1.AddRow (null, 
                             null, 
                             ToolGroupBox()
            );

			topLayout.AddRow (null, nLayout1, null);

            var nLayout2 = new DynamicLayout { Padding = Padding.Empty, Spacing = Size.Empty };
            var nLayout3 = new DynamicLayout { Spacing = Size.Empty };
            var nLayout4 = new DynamicLayout ();

            nLayout2.AddSeparateRow (TableLayout.AutoSized (btnRotation0 = new Button { Text = "0" }),
                                     TableLayout.AutoSized (btnRotation90 = new Button { Text = "90" }),
                                     TableLayout.AutoSized (btnRotation180 = new Button { Text = "180" }),
                                     TableLayout.AutoSized (btnRotation270 = new Button { Text = "270" }));

            nLayout2.AddSeparateRow(new Label { Text = "Rotation:", VerticalAlign = VerticalAlign.Middle },
                            nudRotation = new NumericUpDown { MinValue = 0, MaxValue = 360, Value = 0, Size = new Size(-1, -1) },
                            new CheckBox { Text = "Random" }
            );
            var gbRotation = new GroupBox { Text = "Rotation", Content = nLayout2 };

            nLayout3.Add (new CheckBox { Text = "Rotate Footprints" });
            nLayout3.Add (new CheckBox { Text = "Automatic Walls" });
            nLayout4.AddRow (gbRotation, nLayout3);

            topLayout.AddRow (null, nLayout4, null);

            var nLayout5 = new DynamicLayout { Padding = Padding.Empty };
            filterText = new SearchBox { PlaceholderText = "Filter", Size = new Size(235, 27) };
            nLayout5.AddRow (filterText, TableLayout.AutoSized(new Button { Text = "Select this Objects" }));

            var tabControl = new TabControl ();
            tabControl.TabPages.Add (new TabPage { Text = "Features", Content = FeaturesPanel() });
            tabControl.TabPages.Add (new TabPage { Text = "Structures", Content = StructuresPanel() });
            tabControl.TabPages.Add (new TabPage { Text = "Droids", Content = DroidsPanel() });

            var mainLayout = new DynamicLayout ();
            mainLayout.Add (topLayout);
            mainLayout.AddRow (nLayout5);
			mainLayout.Add (tabControl);

			Content = mainLayout;
			 * */
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

		protected override void OnPreLoad(EventArgs e)
		{
			base.OnPreLoad(e);
			
			this.nRotation.Bind(n => n.Enabled, this.chkRandom.CheckedBinding.Convert(g => !g.Value));
			this.nRotation.ValueBinding.Bind(this.ToolOptions.PlaceObject, p => p.Rotation);
			this.chkAutoWalls.CheckedBinding.Bind(this.ToolOptions.PlaceObject, p => p.AutoWalls);
			this.chkRandom.CheckedBinding.Bind(this.ToolOptions.PlaceObject, p => p.RotationRandom);
			this.chkRotateFootprints.CheckedBinding.Bind(this.ToolOptions.PlaceObject, p => p.RotateFootprints);
		}

		public void RefreshGridViews()
		{
			var objFeatures = App.ObjectData.FeatureTypes.GetItemsAsSimpleList()
				.ConvertAll(f => new PlaceObjectGridViewItem(f));

			this.features.Clear();
			this.features.AddRange(objFeatures);
			
			this.gFeatures.DataStore = features;


			var objStrcuts = App.ObjectData.StructureTypes.GetItemsAsSimpleList()
				.ConvertAll(f => new PlaceObjectGridViewItem(f));

			this.structs.Clear();
			this.structs.AddRange(objStrcuts);

			this.gStructures.DataStore = this.structs;


			var objDroids = App.ObjectData.DroidTemplates.GetItemsAsSimpleList()
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

		private IEnumerable<object> selected;

		void AnyPlayer_Click(object sender, EventArgs e)
		{
			
		}

		void AnyPlayer_PreLoad(object sender, EventArgs e)
		{
			
		}

		void AnyGrid_SelectionChanged(object sender, EventArgs e)
		{
			var grid = sender as PlaceObjectGridView;
			var types = grid.SelectedItems
				.Select(gvi => gvi.UnitBaseType).ToList();
			
			//NULL reference here.... not sure how to resolve this yet.
			this.ToolOptions.PlaceObject.SelectedObjectTypes = types;

			this.EventBroker.DrawLater(this);
		}

		void ToolSelection_Click(object sender, EventArgs e)
		{
			var button = sender as Button;

			var tool = button.Tag.To<MouseTool>();

			ToolOptions.MouseTool = tool;
		}


		private FilterCollection<PlaceObjectGridViewItem> features;
		private FilterCollection<PlaceObjectGridViewItem> structs;
		private FilterCollection<PlaceObjectGridViewItem> droids; 

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

