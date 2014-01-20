using System;
using OpenTK.Graphics.OpenGL;
using SharpFlame.Collections;
using SharpFlame.Colors;
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
            _ParentMapLink = new ConnectedListLink<clsScriptPosition, clsMap>(this);
        }

        private ConnectedListLink<clsScriptPosition, clsMap> _ParentMapLink;

        public ConnectedListLink<clsScriptPosition, clsMap> ParentMap
        {
            get { return _ParentMapLink; }
        }

        private string _Label;

        public string Label
        {
            get { return _Label; }
        }

        private sXY_int _Pos;

        public int PosX
        {
            get { return _Pos.X; }
            set
            {
                _Pos.X = MathUtil.Clamp_int(value, 0,
                    Convert.ToInt32(Convert.ToInt32(_ParentMapLink.Source.Terrain.TileSize.X * App.TerrainGridSpacing) - 1));
            }
        }

        public int PosY
        {
            get { return _Pos.Y; }
            set
            {
                _Pos.Y = MathUtil.Clamp_int(value, 0,
                    Convert.ToInt32(Convert.ToInt32(_ParentMapLink.Source.Terrain.TileSize.Y * App.TerrainGridSpacing) - 1));
            }
        }

        public static clsScriptPosition Create(clsMap Map)
        {
            clsScriptPosition Result = new clsScriptPosition();

            Result._Label = Map.GetDefaultScriptLabel("Position");

            Result._ParentMapLink.Connect(Map.ScriptPositions);

            return Result;
        }

        public void GLDraw()
        {
            clsDrawHorizontalPosOnTerrain Drawer = new clsDrawHorizontalPosOnTerrain();
            Drawer.Map = _ParentMapLink.Source;
            Drawer.Horizontal = _Pos;
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

        public void MapResizing(sXY_int PosOffset)
        {
            PosX = _Pos.X - PosOffset.X;
            PosY = _Pos.Y - PosOffset.Y;
        }

        public void WriteWZ(IniWriter File)
        {
            File.AppendSectionName("position_" + _ParentMapLink.ArrayPosition.ToStringInvariant());
            File.AppendProperty("pos", _Pos.X.ToStringInvariant() + ", " + _Pos.Y.ToStringInvariant());
            File.AppendProperty("label", _Label);
            File.Gap_Append();
        }

        public sResult SetLabel(string Text)
        {
            sResult Result = new sResult();

            Result = _ParentMapLink.Source.ScriptLabelIsValid(Text);
            if ( Result.Success )
            {
                _Label = Text;
            }
            return Result;
        }

        public void Deallocate()
        {
            _ParentMapLink.Deallocate();
        }
    }
}