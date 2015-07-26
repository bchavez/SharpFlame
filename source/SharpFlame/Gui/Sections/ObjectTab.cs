using System;
using System.Collections.Generic;
using System.Linq;
using Appccelerate.EventBroker;
using Appccelerate.EventBroker.Handlers;
using Eto.Forms;
using Ninject;
using SharpFlame.Core;
using SharpFlame.Domain;
using SharpFlame.FileIO;
using SharpFlame.Mapping;
using SharpFlame.Mapping.Objects;
using SharpFlame.Mapping.Tools;
using SharpFlame.Maths;
using SharpFlame.MouseTools;
using SharpFlame.Settings;
using Z.ExtensionMethods.ObjectExtensions;

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
            //    {
            //        if( m.SelectedUnits.Count == 1 )
            //        {
            //            var u = m.SelectedUnits[0];
            //            return u.TypeBase.Type != UnitType.PlayerStructure;
            //        }
            //        return false;
            //    }, setValue: null), mode: DualBindingMode.OneWay);


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
                    DroidDesign droidType = null;
                    if( TryGetDroidDesign(out droidType) )
                    {
                        return !droidType.IsTemplate;
                    }
					return false;
				}, setValue: null), mode: DualBindingMode.OneWay);

		    this.ddlType.SelectedIndexBinding.BindDataContext(getValue: (Map m) =>
		        {
		            DroidDesign droidType = null;
		            if( TryGetDroidDesign(out droidType) && droidType.TemplateDroidType != null )
		            {
		                return droidType.TemplateDroidType.Num;
		            }
		            return -1;
		        }
		        , setValue: null, defaultGetValue: -1);

		    this.ddlBody.SelectedKeyBinding.BindDataContext(
		        getValue: (Map m) =>
		            {
		                DroidDesign droidType = null;
		                if( TryGetDroidDesign(out droidType) && droidType.Body != null )
		                {
		                    return droidType.Body?.Code;
		                }

		                return null;
		            }
		        ,
		        setValue: ddlBody_Set);


			this.ddlProp.SelectedKeyBinding.BindDataContext(getValue: (Map m) =>
			    {
			        DroidDesign droidType = null;
			        if( TryGetDroidDesign(out droidType) )
			        {
			            return droidType.Propulsion?.Code;
			        }

			        return null;
			    }
				, setValue: ddlProp_Set);

			this.ddlTurret1.SelectedKeyBinding.BindDataContext(getValue: (Map m) =>
			    {
			        DroidDesign droidType = null;
			        if( TryGetDroidDesign(out droidType) )
			        {
			            return droidType.Turret1?.Code;
			        }

					return null;
				}
				, setValue: null);

			this.ddlTurret2.SelectedKeyBinding.BindDataContext(getValue: (Map m) =>
			    {
			        DroidDesign droidType = null;
			        if( TryGetDroidDesign(out droidType) )
			        {
			            return droidType.Turret2?.Code;
			        }

					return null;
				}
				, setValue: null);

			this.ddlTurret3.SelectedKeyBinding.BindDataContext(getValue: (Map m) =>
				{
                    DroidDesign droidType = null;
                    if( TryGetDroidDesign(out droidType) )
                    {
                        return droidType.Turret3?.Code;
                    }

					return null;
				}
				, setValue: null);


			this.cmdConvertToDroid.BindDataContext(t => t.Enabled, Binding.Delegate((Map m) =>
				{
                    DroidDesign droidType = null;
                    if( TryGetDroidDesign(out droidType) )
                    {
                        return droidType.IsTemplate;
                    }

					return false;
				}, setValue: null), mode: DualBindingMode.OneWay);
		}

        private void ddlProp_Set(Map m, string prop)
        {
            if( !CanSetDesign(m, "propulsion") )
                return;

            var objectPropulsion = new clsObjectPropulsion();
            objectPropulsion.Map = m;
            objectPropulsion.Propulsion = this.propulsion[ddlProp.SelectedIndex];
            m.SelectedUnitsAction(objectPropulsion);

            this.EventBroker.SelectedUnitsChanged(this);
            if( objectPropulsion.ActionPerformed )
            {
                m.UndoStepCreate("Object Body Changed");
                this.EventBroker.DrawLater(this);
            }
            this.EventBroker.SelectedUnitsChanged(this);
            if( objectPropulsion.ActionPerformed )
            {
                m.UndoStepCreate("Object Propulsion Changed");
                this.EventBroker.DrawLater(this);
            }
        }

        private bool CanSetDesign(Map m, string checktype)
        {
            if( m == null )
                return false;
            if( !this.ddlBody.Enabled )
                return false;
            if( this.ddlBody.SelectedIndex < 0 )
                return false;
            if( m.SelectedUnits.Count <= 0 )
                return false;
            if( m.SelectedUnits.Count > 1 )
            {
                if( MessageBox.Show($"Change {checktype} of multiple droids?", "", MessageBoxButtons.OKCancel, MessageBoxType.Question) != DialogResult.Ok )
                {
                    return false;
                }
            }
            return true;
        }

        private void ddlBody_Set(Map m, string body)
        {
            if( !CanSetDesign(m, "body") )
                return;

            var objectBody = new clsObjectBody();
            objectBody.Map = m;
            objectBody.Body = bodies[ddlBody.SelectedIndex];
            this.map.SelectedUnitsAction(objectBody);

            this.EventBroker.SelectedUnitsChanged(this);
            if( objectBody.ActionPerformed )
            {
                this.map.UndoStepCreate("Object Body Changed");
                this.EventBroker.DrawLater(this);
            }
        }

        private Button cmdConvertToDroid;

	    private GroupBox grpDroidEditor;
        private List<Body> bodies;
        private List<Propulsion> propulsion;

        private bool TryGetUnit(out Unit u)
        {
            u = this.map.SelectedUnits[0];
            return u != null;
        }
        private bool TryGetDroidDesign(out DroidDesign dd)
        {
            dd = null;
            if( this.map.SelectedUnits.Count == 1 )
            {
                var u = map.SelectedUnits[0];
                if( u.TypeBase.Type == UnitType.PlayerDroid )
                {
                    dd = (DroidDesign)u.TypeBase;
                }
            }
            return dd != null;
        }

        public void Tab_Click(object sender, EventArgs e)
        {
            ToolOptions.MouseTool = MouseTool.ObjectSelect;
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
			
		    bodies = this.ObjectManager.ObjectData.Bodies
			    .Where(b => b.Designable || !this.chkDesignable.Checked.Value)
                .ToList();

            this.ddlBody.DataStore = bodies.Select(b => new ListItem
                {
                    Key = b.Code,
                    Text = "({0}) {1}".FormatWith(b.Name, b.Code),
                    Tag = b
                }).ToList();

            this.propulsion = this.ObjectManager.ObjectData.Propulsions
			    .Where(p => p.Designable || !this.chkDesignable.Checked.Value).ToList();


            ddlProp.DataStore = propulsion.Select(p => new ListItem
                {
                    Key = p.Code,
                    Text = "({0}) {1}".FormatWith(p.Name, p.Code),
                    Tag = p
                }).ToList();

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

        [EventSubscription(EventTopics.OnSelectedObjectChanged, typeof(OnPublisher))]
        public void SelectedObject_Changed(object sender, EventArgs e)
        {
            this.DataContext = this.map;
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

        void cmdConvertToDroid_Click(object sender, EventArgs e)
        {
            if( this.map == null )
            {
                return;
            }
            if( this.map.SelectedUnits.Count <= 0 )
            {
                return;
            }

            if( this.map.SelectedUnits.Count > 1 )
            {
                if( MessageBox.Show("Change design of multiple droids?", "", MessageBoxButtons.OKCancel,MessageBoxType.Question) != DialogResult.Ok )
                {
                    return;
                }
            }
            else
            {
                if( MessageBox.Show("Change design of a droid?", "", MessageBoxButtons.OKCancel, MessageBoxType.Question) != DialogResult.Ok )
                {
                    return;
                }
            }

            var ObjectTemplateToDesign = new clsObjectTemplateToDesign();
            ObjectTemplateToDesign.Map = this.map;
            this.map.SelectedUnitsAction(ObjectTemplateToDesign);

            this.EventBroker.SelectedUnitsChanged(this);

            if( ObjectTemplateToDesign.ActionPerformed )
            {
                this.map.UndoStepCreate("Object Template Removed");
                this.EventBroker.DrawLater(this);
            }
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

	        this.EventBroker.SelectedUnitsChanged(this);
		    this.map.UndoStepCreate("Object Rotation");
		    
			this.EventBroker.DrawLater(this);
	    }


		void AnyPlayer_Click(object sender, EventArgs e)
		{
			if( this.map == null )
				return;

		    this.grpPlayers.DataContext = sender;

		    if( this.map.SelectedUnits.Count <= 0 )
		    {
		        return;
		    }

		    if( this.grpPlayers.DataContext == null )
		    {
		        return;
		    }

		    if( this.map.SelectedUnits.Count > 1 )
		    {
                if( MessageBox.Show("Change player of multiple objects?", "", MessageBoxButtons.OKCancel) != DialogResult.Ok )
                {
                    //SelectedObject_Changed()
                    //todo
                    return;
                }
            }


		    var button = sender as Button;
		    var group = button.Tag.To<clsUnitGroup>();


            var ObjectUnitGroup = new clsObjectUnitGroup();
            ObjectUnitGroup.Map = this.map;
            ObjectUnitGroup.UnitGroup = group;
            this.map.SelectedUnitsAction(ObjectUnitGroup);

		    this.EventBroker.SelectedUnitsChanged(this);
            this.map.UndoStepCreate("Object Player Changed");
            if( App.SettingsManager.MinimapTeamColours )
            {
                // Map.MinimapMakeLater();
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

    }
}

