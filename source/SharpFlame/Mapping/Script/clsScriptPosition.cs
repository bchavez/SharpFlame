using System;
using OpenTK.Graphics.OpenGL;
using SharpFlame.Collections;
using SharpFlame.Colors;
using SharpFlame.Core.Domain;
using SharpFlame.FileIO;
using SharpFlame.FileIO.Ini;
using SharpFlame.Mapping.Drawing;
using SharpFlame.Maths;
using SharpFlame.Util;

namespace SharpFlame.Mapping.Script
{
    public class clsScriptPosition
    {
        public clsScriptPosition()
        {
            parentMapLink = new ConnectedListLink<clsScriptPosition, clsMap>(this);
        }

        private ConnectedListLink<clsScriptPosition, clsMap> parentMapLink;

        public ConnectedListLink<clsScriptPosition, clsMap> ParentMap
        {
            get { return parentMapLink; }
        }

        private string label;

        public string Label
        {
            get { return label; }
        }
        
        public sResult SetLabel(string Text)
        {
            sResult Result = new sResult();

            Result = parentMapLink.Source.ScriptLabelIsValid(Text);
            if ( Result.Success )
            {
                label = Text;
            }
            return Result;
        }

        private XYInt pos;

        public int PosX
        {
            get { return pos.X; }
            set
            {
                pos.X = MathUtil.Clamp_int(value, 0,
                    Convert.ToInt32(Convert.ToInt32(parentMapLink.Source.Terrain.TileSize.X * App.TerrainGridSpacing) - 1));
            }
        }

        public int PosY
        {
            get { return pos.Y; }
            set
            {
                pos.Y = MathUtil.Clamp_int(value, 0,
                    Convert.ToInt32(Convert.ToInt32(parentMapLink.Source.Terrain.TileSize.Y * App.TerrainGridSpacing) - 1));
            }
        }

        public clsScriptPosition(clsMap map) {
            label = map.GetDefaultScriptLabel("Position");

            parentMapLink.Connect(map.ScriptPositions);
        }

        public void GLDraw()
        {
            clsDrawHorizontalPosOnTerrain Drawer = new clsDrawHorizontalPosOnTerrain();
            Drawer.Map = parentMapLink.Source;
            Drawer.Horizontal = pos;
            if ( Program.frmMainInstance.SelectedScriptMarker == this )
            {
                GL.LineWidth(4.5F);
                Drawer.Colour = new sRGBA_sng(1.0F, 1.0F, 0.5F, 1.0F);
            }
            else
            {
                GL.LineWidth(3.0F);
                Drawer.Colour = new sRGBA_sng(1.0F, 1.0F, 0.0F, 0.75F);
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
            File.AppendSectionName("position_" + parentMapLink.ArrayPosition.ToStringInvariant());
            File.AppendProperty("pos", pos.X.ToStringInvariant() + ", " + pos.Y.ToStringInvariant());
            File.AppendProperty("label", label);
            File.Gap_Append();
        }

        public void Deallocate()
        {
            parentMapLink.Deallocate();
        }
    }
}