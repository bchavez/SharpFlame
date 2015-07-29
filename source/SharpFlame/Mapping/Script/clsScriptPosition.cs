

using System;
using OpenTK.Graphics.OpenGL;
using SharpFlame.Collections;
using SharpFlame.Colors;
using SharpFlame.FileIO;
using SharpFlame.Core;
using SharpFlame.Core.Collections;
using SharpFlame.Core.Domain;
using SharpFlame.Core.Domain.Colors;
using SharpFlame.Core.Parsers.Ini;
using SharpFlame.Mapping.Drawing;
using SharpFlame.Maths;
using SharpFlame.Util;


namespace SharpFlame.Mapping.Script
{
    public class clsScriptPosition
    {
        private readonly ConnectedListItem<clsScriptPosition, Map> parentMapLink;

        private string label;

        private XYInt pos;

        public clsScriptPosition()
        {
            parentMapLink = new ConnectedListItem<clsScriptPosition, Map>(this);
        }

        public clsScriptPosition(Map map)
        {
            label = map.GetDefaultScriptLabel("Position");

            parentMapLink.Connect(map.ScriptPositions);
        }

        public ConnectedListItem<clsScriptPosition, Map> ParentMap
        {
            get { return parentMapLink; }
        }

        public string Label
        {
            get { return label; }
        }

        public int PosX
        {
            get { return pos.X; }
            set
            {
                pos.X = MathUtil.ClampInt(value, 0,
                    Convert.ToInt32(Convert.ToInt32(parentMapLink.Owner.Terrain.TileSize.X * Constants.TerrainGridSpacing) - 1));
            }
        }

        public int PosY
        {
            get { return pos.Y; }
            set
            {
                pos.Y = MathUtil.ClampInt(value, 0,
                    Convert.ToInt32(Convert.ToInt32(parentMapLink.Owner.Terrain.TileSize.Y * Constants.TerrainGridSpacing) - 1));
            }
        }

        public SimpleResult SetLabel(string Text)
        {
            var Result = new SimpleResult();

            Result = parentMapLink.Owner.ScriptLabelIsValid(Text);
            if ( Result.Success )
            {
                label = Text;
            }
            return Result;
        }

        public void GLDraw()
        {
            var Drawer = new clsDrawHorizontalPosOnTerrain();
            Drawer.Map = parentMapLink.Owner;
            Drawer.Horizontal = pos;
            if ( Program.frmMainInstance.SelectedScriptMarker == this )
            {
                GL.LineWidth(4.5F);
                Drawer.Colour = new SRgba(1.0F, 1.0F, 0.5F, 1.0F);
            }
            else
            {
                GL.LineWidth(3.0F);
                Drawer.Colour = new SRgba(1.0F, 1.0F, 0.0F, 0.75F);
            }
            Drawer.ActionPerform();
        }

        public void MapResizing(XYInt PosOffset)
        {
            PosX = pos.X - PosOffset.X;
            PosY = pos.Y - PosOffset.Y;
        }

        public void WriteWZ(IniWriter File)
        {
            File.AddSection("position_" + parentMapLink.Position.ToStringInvariant());
            File.AddProperty("pos", pos.X.ToStringInvariant() + ", " + pos.Y.ToStringInvariant());
            File.AddProperty("label", label);
        }

        public void Deallocate()
        {
            parentMapLink.Deallocate();
        }
    }
}