using System;
using System.Collections;
using System.Linq;
using Appccelerate.EventBroker;
using Appccelerate.EventBroker.Handlers;
using Eto;
using Eto.Drawing;
using Eto.Forms;
using Ninject;
using SharpFlame.Core;
using SharpFlame.Domain;
using SharpFlame.FileIO;
using SharpFlame.Gui.Controls;
using SharpFlame;
using SharpFlame.Mapping;
using SharpFlame.Mapping.Objects;
using SharpFlame.Mapping.Tools;
using SharpFlame.Maths;
using SharpFlame.MouseTools;
using SharpFlame.Settings;
using Z.ExtensionMethods;
using Z.ExtensionMethods.Object;

namespace SharpFlame.Gui.Sections
{
    public class ObjectTab : TabPage
    {
        [Inject]
        internal ToolOptions ToolOptions { get; set; }

		[Inject]
		internal ObjectManager ObjectManager { get; set; }

		[Inject]
		internal IEventBroker EventBroker { get; set; }

	    private Label lblObjectName;
	    private Map map;
	    private GroupBox grpPlayers;

	    private NumericUpDown txtRotation;
	    private TextBox txtId;
	    private TextBox txtLabel;
	    private NumericUpDown txtHealth;

	    private DropDown ddlType;
	    private DropDown ddlBody;
	    private DropDown ddlProp;
	    private DropDown ddlTurret1;
	    private DropDown ddlTurret2;
		private DropDown ddlTurret3;
		private NumericUpDown txtTurrets;
	    private CheckBox chkDesignable;

	    public ObjectTab ()
	    {
		    XomlReader.Load(this);
		    /*var mainLayout = new DynamicLayout ();

            var nLayout0 = new DynamicLayout { Padding = Padding.Empty };
            var nLayout1 = new DynamicLayout ();
            nLayout1.AddRow(
                new Label { Text = "Type:", VerticalAlign = VerticalAlign.Middle },
                new Label { Text = "A0CyborgFactory (Cyborg Factory)", VerticalAlign = VerticalAlign.Middle });
           

            nLayout0.AddRow (nLayout1);

            PlayerSelector playerSelector;
            if (this.Platform.IsWinForms)
            {
                playerSelector = new PlayerSelectorWinforms (Constants.PlayerCountMax);
            } else
            {
                playerSelector = new PlayerSelector (Constants.PlayerCountMax);
            }
            playerSelector.SelectedPlayer = "S";

            var nLayout2 = new DynamicLayout { Padding = Padding.Empty };
            nLayout2.AddRow(new Label { Text = "Player", VerticalAlign = VerticalAlign.Middle }, 
                            TableLayout.AutoSized(playerSelector)
            );

            nLayout0.AddRow (nLayout2);

            var nLayout3 = new DynamicLayout ();
            var nLayout4 = new DynamicLayout { Padding = Padding.Empty };
            nLayout4.AddRow(TableLayout.AutoSized(new NumericUpDown { MaxValue = 360, MinValue = 0, Value = 0, Size = new Size(-1, -1) }),
                            new Label { Text = "(0-359)", VerticalAlign = VerticalAlign.Middle });

            nLayout3.AddRow(new Label { Text = "Rotation:", VerticalAlign = VerticalAlign.Middle },
                            nLayout4,
                            new Button { Text = "Realign" });                            

            nLayout3.AddRow(new Label { Text = "ID:", VerticalAlign = VerticalAlign.Middle },
                            new TextBox { Text = "1200", Enabled = false },
                            new Button { Text = "Flatten Terrain" });

            nLayout3.AddRow(new Label { Text = "Label:", VerticalAlign = VerticalAlign.Middle },
                            new TextBox ());

            nLayout3.AddRow(new Label { Text = "Health%:", VerticalAlign = VerticalAlign.Middle },
                            TableLayout.AutoSized(new NumericUpDown { MinValue = 0, MaxValue = 100, Value = 100, Size = new Size(-1, -1) }));

            nLayout0.AddRow (nLayout3);

            mainLayout.AddRow (null, nLayout0, null);

            var nLayout5 = new DynamicLayout ();
            nLayout5.AddRow(TableLayout.AutoSized (new Button { Text = "Convert Templates To Design" }),
                            new CheckBox { Text = "Designables Only", Checked = true });
            mainLayout.AddRow (null, nLayout5, null);

            var nLayout6 = new DynamicLayout ();
            nLayout6.AddRow (new Label { Text = "Type", VerticalAlign = VerticalAlign.Middle }, new ComboBox ());
            nLayout6.AddRow (new Label { Text = "Body", VerticalAlign = VerticalAlign.Middle }, new ComboBox ());
            nLayout6.AddRow (new Label { Text = "Propulsion", VerticalAlign = VerticalAlign.Middle }, new ComboBox ());

            var radio0 = new RadioButton { Text = "0" };
            var radio1 = new RadioButton (radio0) { Text = "1" };
            var radio2 = new RadioButton (radio1) { Text = "2" };
            var radio3 = new RadioButton (radio2) { Text = "3" };

            var nLayout7 = new DynamicLayout { Padding = Padding.Empty };
            nLayout7.AddRow(new Label { Text = "Turrets", VerticalAlign = VerticalAlign.Middle }, radio1);
            nLayout6.AddRow(nLayout7, new ComboBox ());
    
            var nLayout8 = new DynamicLayout { Padding = Padding.Empty };
            nLayout8.AddRow(radio0, radio2);
            nLayout6.AddRow(nLayout8, new ComboBox ());

            nLayout6.AddRow(radio3, new ComboBox ());

            mainLayout.AddRow(null, nLayout6, null);
            mainLayout.Add (null);

            Content = mainLayout;*/
	    }

		private StackLayout slTurret1;
		private StackLayout slTurret2;
		private StackLayout slTurret3;

		protected override void OnPreLoad(EventArgs e)
		{
			base.OnPreLoad(e);

			this.slTurret1.Bind(t => t.Visible, this.txtTurrets.ValueBinding.Convert(d => d >= 1));
			this.slTurret2.Bind(t => t.Visible, this.txtTurrets.ValueBinding.Convert(d => d >= 2));
			this.slTurret3.Bind(t => t.Visible, this.txtTurrets.ValueBinding.Convert(d => d >= 3));

			this.lblObjectName.TextBinding.BindDataContext<Map>(getValue: m =>
				{
					if( m.SelectedUnits.Count == 0 )
					{
						return "<none selected>";
					}
					if( m.SelectedUnits.Count > 1 )
					{
						return "Multiple Objects";
					}
					if( m.SelectedUnits.Count == 1 )
					{
						var u = m.SelectedUnits[0];
						return u.TypeBase.GetDisplayTextCode();
					}
					return string.Empty;
				}, setValue: (m, s) => { }, mode: DualBindingMode.OneWay);

			this.txtId.TextBinding.BindDataContext<Map>(getValue: m =>
				{
					if( m.SelectedUnits.Count == 1 )
					{
						this.txtId.Enabled = true;
						var u = m.SelectedUnits[0];
						return u.ID.ToStringInvariant();
					}
					this.txtId.Enabled = false;
					return string.Empty;
				}, setValue: null, mode: DualBindingMode.OneWay);



			this.txtLabel.TextBinding.BindDataContext<Map>(getValue: m =>
				{
					if( m.SelectedUnits.Count == 1 )
					{
						var u = m.SelectedUnits[0];

						var labelEnabled = u.TypeBase.Type != UnitType.PlayerStructure;

						if( labelEnabled )
						{
							this.txtLabel.Enabled = true;
							return u.Label;
						}
					}
					this.txtLabel.Enabled = false;
					return string.Empty;
				}, setValue: null, mode: DualBindingMode.OneWay);
			//this.txtLabel.BindDataContext(t => t.Enabled, Binding.Delegate((Map m) =>
			//	{
			//		if( m.SelectedUnits.Count == 1 )
			//		{
			//			var u = m.SelectedUnits[0];
			//			return u.TypeBase.Type != UnitType.PlayerStructure;
			//		}
			//		return false;
			//	}, setValue: null), mode: DualBindingMode.OneWay);


			this.txtHealth.ValueBinding.BindDataContext<Map>(getValue: m =>
				{
					if( m.SelectedUnits.Count == 1 )
					{
						this.txtHealth.Enabled = true;

						var u = m.SelectedUnits[0];
						return u.Health;
					}
					this.txtHealth.Enabled = false;
					return 1;
				}, setValue: null, defaultGetValue:100, mode: DualBindingMode.OneWay);

			this.grpDroidEditor.BindDataContext(t => t.Enabled, Binding.Delegate((Map m) =>
				{
					if( m.SelectedUnits.Count == 1 )
					{
						var u = m.SelectedUnits[0];
						return u.TypeBase.Type == UnitType.PlayerDroid;
					}
					return false;
				}, setValue: null), mode: DualBindingMode.OneWay);

			this.ddlType.SelectedIndexBinding.BindDataContext(getValue: (Map m) =>
				{
					if( m.SelectedUnits.Count == 1 )
					{
						//left off here.
						var u = m.SelectedUnits[0];
						if( u.TypeBase.Type == UnitType.PlayerDroid )
						{
							var droidType = (DroidDesign)u.TypeBase;
							if( droidType.TemplateDroidType != null )
							{
								return droidType.TemplateDroidType.Num;
							}
						}
					}
					return -1;
				}
				, setValue: null);

			this.ddlBody.SelectedKeyBinding.BindDataContext(getValue: (Map m) =>
				{
					if( m.SelectedUnits.Count == 1 )
					{
						var u = m.SelectedUnits[0];
						if( u.TypeBase.Type == UnitType.PlayerDroid )
						{
							var droidType = (DroidDesign)u.TypeBase;
							if( droidType.Body != null )
							{
								return droidType.Body.Code;
							}
						}
					}
					return null;
				}
				, setValue: null);


			this.ddlProp.SelectedKeyBinding.BindDataContext(getValue: (Map m) =>
				{
					if( m.SelectedUnits.Count == 1 )
					{
						var u = m.SelectedUnits[0];
						if( u.TypeBase.Type == UnitType.PlayerDroid )
						{
							var droidType = (DroidDesign)u.TypeBase;
							if( droidType.Propulsion != null )
							{
								return droidType.Propulsion.Code;
							}
						}
					}
					return null;
				}
				, setValue: null);

			this.ddlTurret1.SelectedKeyBinding.BindDataContext(getValue: (Map m) =>
				{
					if( m.SelectedUnits.Count == 1 )
					{
						var u = m.SelectedUnits[0];
						if( u.TypeBase.Type == UnitType.PlayerDroid )
						{
							var droidType = (DroidDesign)u.TypeBase;
							if( droidType.Turret1 != null )
							{
								return droidType.Turret1.Code;
							}
						}
					}
					return null;
				}
				, setValue: null);

			this.ddlTurret2.SelectedKeyBinding.BindDataContext(getValue: (Map m) =>
				{
					if( m.SelectedUnits.Count == 1 )
					{
						var u = m.SelectedUnits[0];
						if( u.TypeBase.Type == UnitType.PlayerDroid )
						{
							var droidType = (DroidDesign)u.TypeBase;
							if( droidType.Turret2 != null )
							{
								return droidType.Turret2.Code;
							}
						}
					}
					return null;
				}
				, setValue: null);

			this.ddlTurret3.SelectedKeyBinding.BindDataContext(getValue: (Map m) =>
				{
					if( m.SelectedUnits.Count == 1 )
					{
						var u = m.SelectedUnits[0];
						if( u.TypeBase.Type == UnitType.PlayerDroid )
						{
							var droidType = (DroidDesign)u.TypeBase;
							if( droidType.Turret3 != null )
							{
								return droidType.Turret3.Code;
							}
						}
					}
					return null;
				}
				, setValue: null);


			this.cmdConvertToDroid.BindDataContext(t => t.Enabled, Binding.Delegate((Map m) =>
				{
					if( m.SelectedUnits.Count == 1 )
					{
						var u = m.SelectedUnits[0];
						if( u.TypeBase.Type == UnitType.PlayerDroid )
						{
							var drodType = u.TypeBase as DroidDesign;
							if( drodType.IsTemplate )
							{
								return true;
							}
						}
					}
					return false;
				}, setValue: null), mode: DualBindingMode.OneWay);
		}

	    private Button cmdConvertToDroid;

	    private GroupBox grpDroidEditor;

        protected override void OnLoadComplete(EventArgs lcEventArgs)
        {
            base.OnLoadComplete(lcEventArgs);

            // Set Mousetool, when we are shown.
            Shown += delegate {
                ToolOptions.MouseTool = MouseTool.ObjectSelect;
            };
        }

		[EventSubscription(EventTopics.OnMapLoad, typeof(OnPublisher))]
		public void HandleMapLoad(Map args)
		{
			this.map = args;
		}

		[EventSubscription(EventTopics.OnObjectManagerLoaded, typeof(OnPublisher))]
		public void HandleObjectManagerLoaded( object sender, EventArgs e)
		{
			this.RefreshDroidEditor();
		}


	    public void RefreshDroidEditor()
	    {
		    if( App.ObjectData == null )
			    return;

			var types = App.TemplateDroidTypes
				.Select(t => new ListItem { Key = t.TemplateCode, Text = t.Name, Tag = t })
				.ToList();
			this.ddlType.DataStore = types;
			
		    var bodies = this.ObjectManager.ObjectData.Bodies
			    .Where(b => b.Designable || !this.chkDesignable.Checked.Value)
			    .Select(b => new ListItem
				    {
					    Key = b.Code,
					    Text = "({0}) {1}".FormatWith(b.Name, b.Code),
					    Tag = b
				    }).ToList();

		    this.ddlBody.DataStore = bodies;

		    var propulsion = this.ObjectManager.ObjectData.Propulsions
			    .Where(p => p.Designable || !this.chkDesignable.Checked.Value)
			    .Select(p => new ListItem
				    {
					    Key = p.Code,
					    Text = "({0}) {1}".FormatWith(p.Name, p.Code),
					    Tag = p
				    }).ToList();

		    ddlProp.DataStore = propulsion;

			var turrets = this.ObjectManager.ObjectData.Turrets
				.Where(t => t.Designable || !this.chkDesignable.Checked.Value)
				.Select(t =>
					{
						var typeName = string.Empty;
						t.GetTurretTypeName(ref typeName);

						var l = new ListItem
							{
								Key = t.Code,
								Text = "({0} - {1}) {2}".FormatWith( typeName, t.Name, t.Code),
								Tag = t
							};

						return l;
					}).ToList();

			this.ddlTurret1.DataStore = turrets;
			this.ddlTurret2.DataStore = turrets;
			this.ddlTurret3.DataStore = turrets;
		}


		public void SelectedObject_Changed()
		{
			this.OnDataContextChanged(EventArgs.Empty);
		}

	    void cmdRealign_Click(object sender, EventArgs e)
	    {
			if( this.map == null ) return;

		    var align = new clsObjectAlignment
			    {
				    Map = this.map
			    };
		    this.map.SelectedUnits.GetItemsAsSimpleList().PerformTool(align);
		    this.EventBroker.UpdateMap(this);
		    this.map.UndoStepCreate("Align Objects");
	    }

	    void cmdFlatten_Click(object sender, EventArgs e)
	    {
			if( this.map == null ) return;

		    var flatten = new clsObjectFlattenTerrain();
		    this.map.SelectedUnits.GetItemsAsSimpleClassList().PerformTool(flatten);
		    this.EventBroker.UpdateMap(this);
		    this.map.UndoStepCreate("Flatten Under Structures");
	    }

	    void txtRotation_ValueChanged(object sender, EventArgs e)
	    {
			var angleInt = Convert.ToInt32(this.txtRotation.Value);

		    var angle = MathUtil.ClampInt(angleInt, 0, 359);

		    if( this.map.SelectedUnits.Count > 1 )
		    {
			    if( MessageBox.Show(this, "Change rotation of multiple objects?", MessageBoxButtons.YesNo) != DialogResult.Yes )
			    {
				    return;
			    }
		    }

		    var objRotation = new clsObjectRotation
			    {
				    Angle = angle,
					Map = this.map
			    };

		    this.map.SelectedUnitsAction(objRotation);

		    this.EventBroker.UpdateMap(this);

		    SelectedObject_Changed();
		    this.map.UndoStepCreate("Object Rotation");
		    
			this.EventBroker.DrawLater(this);
	    }


		void AnyPlayer_Click(object sender, EventArgs e)
		{
			if( this.map == null )
				return;

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

    }
}

