using System;
using Appccelerate.EventBroker;
using Appccelerate.EventBroker.Handlers;
using Appccelerate.Events;
using Eto;
using Eto.Drawing;
using Eto.Forms;
using Ninject;
using SharpFlame.Core;
using SharpFlame;
using SharpFlame.Core.Domain;
using SharpFlame.Core.Extensions;
using SharpFlame.FileIO;
using SharpFlame.Mapping;
using SharpFlame.Maths;
using SharpFlame.MouseTools;

namespace SharpFlame.Gui.Sections
{
	public class ResizeTab : TabPage
	{
        [Inject]
        public ToolOptions ToolOptions { get; set; }

        [Inject]
        public IEventBroker EventBroker { get; set; }

	    protected NumericUpDown numSizeX;
	    protected NumericUpDown numSizeY;
	    protected NumericUpDown numOffsetX;
	    protected NumericUpDown numOffsetY;
	    private Map map;

	    public ResizeTab ()
        {
            XomlReader.Load(this);

            this.numSizeX.ValueBinding.Bind(() =>
                {
                    if( map == null ) return 0;
                    return this.map.Terrain.TileSize.X;
                }, d => { }, mode: DualBindingMode.OneWay);
            this.numSizeY.ValueBinding.Bind(() =>
                {
                    if( map == null ) return 0;
                    return this.map.Terrain.TileSize.Y;
                }, d => { }, mode: DualBindingMode.OneWay);
		}

        [EventSubscription(EventTopics.OnMapLoad, typeof(OnPublisher))]
        public void OnMapLoad(Map args)
        {
	        this.map = args;
        }

	    protected void cmdResize_Click(object sender, EventArgs e)
	    {
	        var newSize = new XYInt(Convert.ToInt32(numSizeX.Value), Convert.ToInt32(numSizeY.Value));
            var offset = new XYInt(Convert.ToInt32(numOffsetX.Value), Convert.ToInt32(numOffsetY.Value));

            Map_Resize(offset, newSize);
	    }

        private void Map_Resize(XYInt offset, XYInt newSize)
        {
            if( map == null )
            {
                return;
            }

            if( MessageBox.Show("Resizing can't be undone. Continue?", "", MessageBoxButtons.OKCancel) != DialogResult.Ok )
            {
                return;
            }

            if( newSize.X < 1 | newSize.Y < 1 )
            {
                MessageBox.Show("Map sizes must be at least 1.", "", MessageBoxButtons.OK);
                return;
            }
            if( newSize.X > Constants.WzMapMaxSize | newSize.Y > Constants.WzMapMaxSize )
            {
                if( MessageBox.Show(
                    "Warzone doesn't support map sizes above {0} Continue anyway?".Format2(Constants.WzMapMaxSize),
                    "", MessageBoxButtons.YesNo) != DialogResult.Yes )
                {
                    return;
                }
            }

            map.TerrainResize(offset, newSize);

            this.EventBroker.ScriptMarkerUpdate(this);

            map.SectorGraphicsChanges.SetAllChanged();

            this.EventBroker.UpdateMap(this);
            this.EventBroker.DrawLater(this);
        }

	    protected void cmdResizeSelection_Click(object sender, EventArgs e)
	    {
            if( map == null )
            {
                return;
            }

            if( map.SelectedAreaVertexA == null || map.SelectedAreaVertexB == null )
            {
                MessageBox.Show("You haven't selected anything.");
                return;
            }

            var start = new XYInt();
            var finish = new XYInt();
            MathUtil.ReorderXY(map.SelectedAreaVertexA, map.SelectedAreaVertexB, ref start, ref finish);
	        var area = new XYInt
	            {
	                X = finish.X - start.X,
	                Y = finish.Y - start.Y
	            };

	        Map_Resize(start, area);   
	    }

	    protected override void OnLoadComplete(EventArgs lcEventArgs)
        {
            base.OnLoadComplete(lcEventArgs);

            // Set Mousetool, when we are shown.
            this.Click += delegate {
                ToolOptions.MouseTool = MouseTool.Default;
            };
        }
	}
}

