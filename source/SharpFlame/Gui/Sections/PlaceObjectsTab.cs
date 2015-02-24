using System;
using System.Collections.Specialized;
using Appccelerate.EventBroker;
using Appccelerate.EventBroker.Handlers;
using Appccelerate.Events;
using Eto.Drawing;
using Eto.Forms;
using Eto.Gl;
using Ninject;
using Ninject.Extensions.Logging;
using SharpFlame.Core;
using SharpFlame.Domain.ObjData;
using SharpFlame.Settings;
using SharpFlame.UiOptions;

namespace SharpFlame.Gui.Sections
{
	public class PlaceObjectsTab : TabPage
	{
        [Inject]
        internal Options UiOptions { get; set; }

		[Inject]
		internal SettingsManager SettingsManager { get; set; }

		[Inject]
		internal ILogger Logger { get; set; }

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
			gFeatures.Columns.Add(new GridColumn { HeaderText = "Internal Name", DataCell = new TextBoxCell("Name"), Editable = false, Sortable = true,  });
			gFeatures.Columns.Add(new GridColumn { HeaderText = "In-Game Name", DataCell = new TextBoxCell("Code"), Editable = false, Sortable = true });
			gStructures.Columns.Add(new GridColumn { HeaderText = "Internal Name", DataCell = new TextBoxCell("InternalName"), Editable = false, Sortable = true });
			gStructures.Columns.Add(new GridColumn { HeaderText = "In-Game Name", DataCell = new TextBoxCell("InGameName"), Editable = false, Sortable = true });
			gDroids.Columns.Add(new GridColumn { HeaderText = "Internal Name", DataCell = new TextBoxCell("InternalName"), Editable = false, Sortable = true });
			gDroids.Columns.Add(new GridColumn { HeaderText = "In-Game Name", DataCell = new TextBoxCell("InGameName"), Editable = false, Sortable = true });
        }




		private Panel panDroids;
		private Panel panStructs;
		private Panel panFeatures;

		private GridView gFeatures;
		private GridView gStructures;
		private GridView gDroids;

		protected override void OnPreLoad(EventArgs e)
		{
			base.OnPreLoad(e);
			
			this.nRotation.Bind(n => n.Enabled, this.chkRandom.CheckedBinding.Convert(g => !g.Value));
		}

		public void RefreshGridViews()
		{
			var objFeatures = App.ObjectData.FeatureTypes.GetItemsAsSimpleList();
			var dsc = new DataStoreCollection<object>(objFeatures);

			this.gFeatures.DataStore = dsc;
		}

	    protected override void OnLoadComplete(EventArgs lcEventArgs)
        {
            base.OnLoadComplete(lcEventArgs);
			
		    /*// Rotation buttons
            btnRotation0.Click += delegate {
                nudRotation.Value = 0D;
            };

            btnRotation90.Click += delegate {
                nudRotation.Value = 90D;
            };


            btnRotation180.Click += delegate {
                nudRotation.Value = 180D;
            };

            btnRotation270.Click += delegate {
                nudRotation.Value = 270D;
            };

            // Set Mousetool, when we are shown.
            Shown += delegate {
                UiOptions.MouseTool = MouseTool.Default;
            };

            App.ObjectDataChanged += delegate
            {
                var objFeatures = App.ObjectData.FeatureTypes.GetItemsAsSimpleList();
                var gicFeatures = new DataStoreCollection<object>();
                foreach (var obj in objFeatures) 
                {
                    string code = null;
                    obj.GetCode(ref code);
                    gicFeatures.Add(new MyGridItem(code, obj.GetName().Replace("*", "")));
                }
                grvFeatures.DataStore = gicFeatures;

                var objStructures = App.ObjectData.StructureTypes.GetItemsAsSimpleList();
                var gicStructures = new DataStoreCollection<object>();
                foreach (var obj in objStructures) 
                {
                    string code = null;
                    obj.GetCode(ref code);
                    gicStructures.Add(new MyGridItem(code, obj.GetName().Replace("*", "")));
                }
                grvStructures.DataStore = gicStructures;

                var objDroids = App.ObjectData.DroidTemplates.GetItemsAsSimpleList();
                var gicDroids = new DataStoreCollection<object>();
                foreach (var obj in objDroids) 
                {
                    string code = null;
                    obj.GetCode(ref code);
                    gicDroids.Add(new MyGridItem(code, obj.GetName().Replace("*", "")));
                }
                grvDroids.DataStore = gicDroids;
            };
			 */

        }

		void AnyPlayer_Click(object sender, EventArgs e)
		{
			
		}

		void AnyPlayer_PreLoad(object sender, EventArgs e)
		{
			
		}

		void ToolSelection_Click(object sender, EventArgs e)
		{
			
		}

		/*
        class MyGridItem
        {
            public string InternalName
            {
                get;
                set;
            }

            public string InGameName
            {
                get;
                set;
            }

            // used for owner-drawn cells
            public MyGridItem(string internalName, string inGameName)
            {
                // initialize to random values
                InternalName = internalName;
                InGameName = inGameName;
            }
        }

        DynamicLayout FeaturesPanel() {
            var mainLayout = new DynamicLayout ();

            grvFeatures = new GridView { Size = new Size(300, 100) };
            grvFeatures.Columns.Add(new GridColumn { HeaderText = "Internal Name", DataCell = new TextBoxCell("InternalName"), Editable = false, Sortable = true });
            grvFeatures.Columns.Add(new GridColumn { HeaderText = "In-Game Name", DataCell = new TextBoxCell("InGameName"), Editable = false, Sortable = true });

            filterText.TextChanged += (s, e) =>
            {
                var filterItems = (filterText.Text ?? "").Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                // Set the filter delegate on the GridView
                grvFeatures.Filter = (filterItems.Length == 0) ? (Func<object, bool>)null : o =>
                {
                    var i = o as MyGridItem;
                    var matches = true;

                    // Every item in the split filter string should be within the Text property
                    matches = filterItems.Any(f => i.InternalName.IndexOf(f, StringComparison.CurrentCultureIgnoreCase) != -1 ||
                        i.InGameName.IndexOf(f, StringComparison.CurrentCultureIgnoreCase) != -1);

                    return matches;
                };
            };

            mainLayout.Add (grvFeatures);

            return mainLayout;
        }

        DynamicLayout StructuresPanel() {
            var mainLayout = new DynamicLayout ();
            grvStructures = new GridView { Size = new Size(300, 100) };
            grvStructures.Columns.Add(new GridColumn { HeaderText = "Internal Name", DataCell = new TextBoxCell("InternalName"), Editable = false, Sortable = true });
            grvStructures.Columns.Add(new GridColumn { HeaderText = "In-Game Name", DataCell = new TextBoxCell("InGameName"), Editable = false, Sortable = true });

            filterText.TextChanged += (s, e) =>
            {
                var filterItems = (filterText.Text ?? "").Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                // Set the filter delegate on the GridView
                grvStructures.Filter = (filterItems.Length == 0) ? (Func<object, bool>)null : o =>
                {
                    var i = o as MyGridItem;
                    var matches = true;

                    // Every item in the split filter string should be within the Text property
                    matches = filterItems.Any(f => i.InternalName.IndexOf(f, StringComparison.CurrentCultureIgnoreCase) != -1 ||
                        i.InGameName.IndexOf(f, StringComparison.CurrentCultureIgnoreCase) != -1);

                    return matches;
                };
            };

            mainLayout.Add (grvStructures);
            return mainLayout;
        }

        DynamicLayout DroidsPanel() {
            var mainLayout = new DynamicLayout ();
            grvDroids = new GridView { Size = new Size(300, 100) };
            grvDroids.Columns.Add(new GridColumn { HeaderText = "Internal Name", DataCell = new TextBoxCell("InternalName"), Editable = false, Sortable = true, Width = 200 });
            grvDroids.Columns.Add(new GridColumn { HeaderText = "In-Game Name", DataCell = new TextBoxCell("InGameName"), Editable = false, Sortable = true, Width = 200 });

            filterText.TextChanged += (s, e) =>
            {
                var filterItems = (filterText.Text ?? "").Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                // Set the filter delegate on the GridView
                grvDroids.Filter = (filterItems.Length == 0) ? (Func<object, bool>)null : o =>
                {
                    var i = o as MyGridItem;
                    var matches = true;

                    // Every item in the split filter string should be within the Text property
                    matches = filterItems.Any(f => i.InternalName.IndexOf(f, StringComparison.CurrentCultureIgnoreCase) != -1 ||
                        i.InGameName.IndexOf(f, StringComparison.CurrentCultureIgnoreCase) != -1);

                    return matches;
                };
            };

            mainLayout.Add (grvDroids);
            return mainLayout;
        }


        Control ToolGroupBox() {
            var control = new GroupBox { Text = "Tool" };
            var nLayout1 = new DynamicLayout ();
            var rbList = new RadioButtonList { Orientation = RadioButtonListOrientation.Vertical };
            rbList.Items.Add(new ListItem { Text = "Place" });
            rbList.Items.Add(new ListItem { Text = "Lines" });
            rbList.SelectedIndex = 0;
            nLayout1.Add (rbList);

            control.Content = nLayout1;
            return control;
        }*/
	}
}

