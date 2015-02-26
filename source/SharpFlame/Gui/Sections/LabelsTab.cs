using System;
using System.Collections.Generic;
using System.Linq;
using Appccelerate.EventBroker;
using Appccelerate.EventBroker.Handlers;
using Appccelerate.Events;
using Eto;
using Eto.Drawing;
using Eto.Forms;
using Ninject;
using SharpFlame.Core;
using SharpFlame.Core.Domain;
using SharpFlame.Mapping;
using SharpFlame.Mapping.Script;
using SharpFlame.MouseTools;


namespace SharpFlame.Gui.Sections
{
    public class LabelsTab : TabPage
    {
        [Inject]
        internal ToolOptions ToolOptions { get; set; }

        [Inject]
        internal IEventBroker EventBroker { get; set; }

        public LabelsTab()
        {
            XomlReader.Load(this);

			this.txtLabel.Bind(l => l.Enabled, Binding.Delegate(() => this.DataContext != null,
				addChangeEvent: handlerToExecuteWhenSourceChanges => this.DataContextChanged += handlerToExecuteWhenSourceChanges,
				removeChangeEvent: handlerToExecuteWhenSourceChanges => this.DataContextChanged -= handlerToExecuteWhenSourceChanges));

			this.numX.Bind(n => n.Enabled, Binding.Delegate(() => this.DataContext != null,
				addChangeEvent: a => this.DataContextChanged += a,
				removeChangeEvent: a => this.DataContextChanged -= a));

			this.numY.Bind(n => n.Enabled, Binding.Delegate(() => this.DataContext != null,
				addChangeEvent: a => this.DataContextChanged += a,
				removeChangeEvent: a => this.DataContextChanged -= a));

			this.numX2.Bind(n => n.Enabled, Binding.Delegate(() => this.DataContext is clsScriptArea,
				addChangeEvent: a => this.DataContextChanged += a,
				removeChangeEvent: a => this.DataContextChanged -= a));

			this.numY2.Bind(n => n.Enabled, Binding.Delegate(() => this.DataContext is clsScriptArea,
				addChangeEvent: a => this.DataContextChanged += a,
				removeChangeEvent: a => this.DataContextChanged -= a));

	        this.txtLabel.TextBinding.BindDataContext<object>(o =>
		        {
			        if( o == null ) return string.Empty;
			        if( o is clsScriptPosition )
			        {
				        var pos = o as clsScriptPosition;
				        return pos.Label;
			        }
			        if( o is clsScriptArea )
			        {
				        var pos = o as clsScriptArea;
				        return pos.Label;
			        }
			        return string.Empty;
		        }, (o, v) =>
			        {
			        }
		        );

	        this.numX.ValueBinding.BindDataContext<object>(o =>
		        {
			        if( o == null ) return 0;
			        if( o is clsScriptPosition )
			        {
				        var pos = o as clsScriptPosition;
				        return pos.PosX;
			        }
			        if( o is clsScriptArea )
			        {
				        var pos = o as clsScriptArea;
				        return pos.PosAX;
			        }
			        return 0;
		        }, (o, v) => { });

	        this.numY.ValueBinding.BindDataContext<object>(o =>
		        {
			        if( o == null ) return 0;
			        if( o is clsScriptPosition )
			        {
				        var pos = o as clsScriptPosition;
				        return pos.PosY;
			        }
			        if( o is clsScriptArea )
			        {
				        var pos = o as clsScriptArea;
				        return pos.PosAY;
			        }
			        return 0;
		        }, (o, v) => { });

	        this.numX2.ValueBinding.BindDataContext<object>(o =>
		        {
			        if( o == null ) return 0;
			        if( o is clsScriptArea )
			        {
				        var pos = o as clsScriptArea;
				        return pos.PosBX;
			        }
			        return 0;
		        }, (o, v) => { });

	        this.numX2.ValueBinding.BindDataContext<object>(o =>
		        {
			        if( o == null ) return 0;
			        if( o is clsScriptArea )
			        {
				        var pos = o as clsScriptArea;
				        return pos.PosBY;
			        }
			        return 0;
		        }, (o, v) => { });

	        this.cmdRemove.Bind(n => n.Enabled, Binding.Delegate(() => this.DataContext != null,
		        addChangeEvent: a => this.DataContextChanged += a,
		        removeChangeEvent: a => this.DataContextChanged -= a));

			//this.lstScriptPositions.SelectedValueChanged += (sender, args) =>
			//	{
			//		this.DataContext = lstScriptPositions.SelectedValue;
			//	};
			//this.lstScriptAreas.SelectedValueChanged += (Sender, args) =>
			//	{
			//		this.DataContext = lstScriptAreas.SelectedValue;
			//	};

	        this.lstScriptPositions.SelectedValueBinding.Bind(() => null,
		        v => {
			             this.DataContext = v;
		        }, mode: DualBindingMode.OneWayToSource);

	        this.lstScriptAreas.SelectedValueBinding.Bind(() => null,
		        v => {
			             this.DataContext = v;
		        }, mode: DualBindingMode.OneWayToSource);
        }

	    protected Button cmdRemove;

        protected override void OnLoadComplete(EventArgs lcEventArgs)
        {
            base.OnLoadComplete(lcEventArgs);            

            // Set Mousetool, when we are shown.
            Shown += delegate {
                                  ToolOptions.MouseTool = MouseTool.Default;
            };
        }

        [EventSubscription(EventTopics.OnMapLoad, typeof(OnPublisher))]
        public void HandleMapLoad(Map newMap)
        {
            this.map = newMap;
        }


        protected Button cmdScriptAreaCreate;
		
        private Map map;
        
        public void cmdScriptAreaCreate_Click(Object sender, EventArgs e)
        {
            if( !cmdScriptAreaCreate.Enabled )
            {
                return;
            }
            if( map == null )
            {
                return;
            }

            if( map.SelectedAreaVertexA == null )
            {
                MessageBox.Show("Select something first.");
                return;
            }
            if( map.SelectedAreaVertexB == null )
            {
                MessageBox.Show("Select something first.");
                return;
            }

            var newArea = new clsScriptArea(map);
            if( newArea == null )
            {
                MessageBox.Show("Error: Creating area failed.");
                return;
            }

            newArea.SetPositions(
                new XYInt(map.SelectedAreaVertexA.X * Constants.TerrainGridSpacing, map.SelectedAreaVertexA.Y * Constants.TerrainGridSpacing),
                new XYInt(map.SelectedAreaVertexB.X * Constants.TerrainGridSpacing, map.SelectedAreaVertexB.Y * Constants.TerrainGridSpacing));

            this.EventBroker.ScriptMarkerUpdate(this);
            ScriptMarkerLists_Update();

            map.SetChanged(); //todo: remove if areas become undoable
            this.EventBroker.DrawLater(this);
        }

        protected ListBox lstScriptPositions;
        protected ListBox lstScriptAreas;

        [EventSubscription(EventTopics.OnScriptMarkerUpdate, typeof(OnPublisher))]
        public void HandleScriptMarkerUpdate(EventArgs e)
        {
            this.ScriptMarkerLists_Update();
        }

        public void ScriptMarkerLists_Update()
        {
	        this.lstScriptPositions.DataStore =
		        this.map.ScriptPositions.Select(i => new ListItem {Text = i.Label, Tag = i});

	        this.lstScriptAreas.DataStore =
		        this.map.ScriptAreas.Select(i => new ListItem {Text = i.Label, Tag = i});

	        SelectedScriptMarker_Update();
        }

	    private object marker;

		protected NumericUpDown numX;
		protected NumericUpDown numY;
		protected NumericUpDown numX2;
		protected NumericUpDown numY2;

		protected TextBox txtLabel;
	    protected GroupBox grpSelectedMarker;

		public void SelectedScriptMarker_Update()
		{
			/*txtScriptMarkerLabel.Enabled = false;
			txtScriptMarkerX.Enabled = false;
			txtScriptMarkerY.Enabled = false;
			txtScriptMarkerX2.Enabled = false;
			txtScriptMarkerY2.Enabled = false;
			txtScriptMarkerLabel.Text = "";
			txtScriptMarkerX.Text = "";
			txtScriptMarkerY.Text = "";
			txtScriptMarkerX2.Text = "";
			txtScriptMarkerY2.Text = "";
			if( marker != null )
			{
				if( marker is clsScriptPosition )
				{
					var ScriptPosition = (clsScriptPosition)marker;
					txtScriptMarkerLabel.Text = ScriptPosition.Label;
					txtScriptMarkerX.Text = ScriptPosition.PosX.ToStringInvariant();
					txtScriptMarkerY.Text = ScriptPosition.PosY.ToStringInvariant();
					txtScriptMarkerLabel.Enabled = true;
					txtScriptMarkerX.Enabled = true;
					txtScriptMarkerY.Enabled = true;
				}
				else if( marker is clsScriptArea )
				{
					var ScriptArea = (clsScriptArea)marker;
					txtScriptMarkerLabel.Text = ScriptArea.Label;
					txtScriptMarkerX.Text = ScriptArea.PosAX.ToStringInvariant();
					txtScriptMarkerY.Text = ScriptArea.PosAY.ToStringInvariant();
					txtScriptMarkerX2.Text = ScriptArea.PosBX.ToStringInvariant();
					txtScriptMarkerY2.Text = ScriptArea.PosBY.ToStringInvariant();
					txtScriptMarkerLabel.Enabled = true;
					txtScriptMarkerX.Enabled = true;
					txtScriptMarkerY.Enabled = true;
					txtScriptMarkerX2.Enabled = true;
					txtScriptMarkerY2.Enabled = true;
				}
			}*/
		}

	    public void cmdRemove_Click(object sender, EventArgs e)
	    {
		    //this.grpSelectedMarker.DataContext = new object();
	    }

    }
}

