#region

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

#endregion

namespace SharpFlame.Mapping.Script
{
    public class clsScriptArea
    {
        private readonly ConnectedListLink<clsScriptArea, Map> _ParentMapLink;

        private XYInt _PosA;
        private XYInt _PosB;

        public clsScriptArea(Map map)
        {
            _ParentMapLink = new ConnectedListLink<clsScriptArea, Map>(this);
            Label = map.GetDefaultScriptLabel("Area");
            _ParentMapLink.Connect(map.ScriptAreas);
        }

        public ConnectedListLink<clsScriptArea, Map> ParentMap
        {
            get { return _ParentMapLink; }
        }

        public string Label { get; private set; }

        public XYInt PosA
        {
            set
            {
                var Map = _ParentMapLink.Source;
                _PosA.X = MathUtil.ClampInt(value.X, 0, Map.Terrain.TileSize.X * Constants.TerrainGridSpacing - 1);
                _PosA.Y = MathUtil.ClampInt(value.Y, 0, Map.Terrain.TileSize.Y * Constants.TerrainGridSpacing - 1);
                MathUtil.ReorderXY(_PosA, _PosB, ref _PosA, ref _PosB);
            }
        }

        public XYInt PosB
        {
            set
            {
                var Map = _ParentMapLink.Source;
                _PosB.X = MathUtil.ClampInt(value.X, 0, Map.Terrain.TileSize.X * Constants.TerrainGridSpacing - 1);
                _PosB.Y = MathUtil.ClampInt(value.Y, 0, Map.Terrain.TileSize.Y * Constants.TerrainGridSpacing - 1);
                MathUtil.ReorderXY(_PosA, _PosB, ref _PosA, ref _PosB);
            }
        }

        public int PosAX
        {
            get { return _PosA.X; }
            set
            {
                _PosA.X = MathUtil.ClampInt(value, 0,
                    Convert.ToInt32(Convert.ToInt32(_ParentMapLink.Source.Terrain.TileSize.X * Constants.TerrainGridSpacing) - 1));
                MathUtil.ReorderXY(_PosA, _PosB, ref _PosA, ref _PosB);
            }
        }

        public int PosAY
        {
            get { return _PosA.Y; }
            set
            {
                _PosA.Y = MathUtil.ClampInt(value, 0,
                    Convert.ToInt32(Convert.ToInt32(_ParentMapLink.Source.Terrain.TileSize.Y * Constants.TerrainGridSpacing) - 1));
                MathUtil.ReorderXY(_PosA, _PosB, ref _PosA, ref _PosB);
            }
        }

        public int PosBX
        {
            get { return _PosB.X; }
            set
            {
                _PosB.X = MathUtil.ClampInt(value, 0,
                    Convert.ToInt32(Convert.ToInt32(_ParentMapLink.Source.Terrain.TileSize.X * Constants.TerrainGridSpacing) - 1));
                MathUtil.ReorderXY(_PosA, _PosB, ref _PosA, ref _PosB);
            }
        }

        public int PosBY
        {
            get { return _PosB.Y; }
            set
            {
                _PosB.Y = MathUtil.ClampInt(value, 0,
                    Convert.ToInt32(Convert.ToInt32(_ParentMapLink.Source.Terrain.TileSize.Y * Constants.TerrainGridSpacing) - 1));
                MathUtil.ReorderXY(_PosA, _PosB, ref _PosA, ref _PosB);
            }
        }

        public void SetPositions(XYInt posA, XYInt posB)
        {
            var map = _ParentMapLink.Source;

            posA.X = MathUtil.ClampInt(posA.X, 0, map.Terrain.TileSize.X * Constants.TerrainGridSpacing - 1);
            posA.Y = MathUtil.ClampInt(posA.Y, 0, map.Terrain.TileSize.Y * Constants.TerrainGridSpacing - 1);
            posB.X = MathUtil.ClampInt(posB.X, 0, map.Terrain.TileSize.X * Constants.TerrainGridSpacing - 1);
            posB.Y = MathUtil.ClampInt(posB.Y, 0, map.Terrain.TileSize.Y * Constants.TerrainGridSpacing - 1);

            MathUtil.ReorderXY(posA, posB, ref _PosA, ref _PosB);
        }

        public void GLDraw()
        {
            var Drawer = new clsDrawTerrainLine();
            Drawer.Map = _ParentMapLink.Source;
            if ( Program.frmMainInstance.SelectedScriptMarker == this )
            {
                GL.LineWidth(4.5F);
                Drawer.Colour = new SRgba(1.0F, 1.0F, 0.5F, 0.75F);
            }
            else
            {
                GL.LineWidth(3.0F);
                Drawer.Colour = new SRgba(1.0F, 1.0F, 0.0F, 0.5F);
            }

            Drawer.StartXY = _PosA;
            Drawer.FinishXY.X = _PosB.X;
            Drawer.FinishXY.Y = _PosA.Y;
            Drawer.ActionPerform();

            Drawer.StartXY = _PosA;
            Drawer.FinishXY.X = _PosA.X;
            Drawer.FinishXY.Y = _PosB.Y;
            Drawer.ActionPerform();

            Drawer.StartXY.X = _PosB.X;
            Drawer.StartXY.Y = _PosA.Y;
            Drawer.FinishXY = _PosB;
            Drawer.ActionPerform();

            Drawer.StartXY.X = _PosA.X;
            Drawer.StartXY.Y = _PosB.Y;
            Drawer.FinishXY = _PosB;
            Drawer.ActionPerform();
        }

        public void MapResizing(XYInt posOffset)
        {
            SetPositions(new XYInt(_PosA.X - posOffset.X, _PosA.Y - posOffset.Y), new XYInt(_PosB.X - posOffset.X, _PosB.Y - posOffset.Y));
        }

        public void WriteWZ(IniWriter file)
        {
            file.AddSection("area_" + _ParentMapLink.ArrayPosition.ToStringInvariant());
            file.AddProperty("pos1", _PosA.X.ToStringInvariant() + ", " + _PosA.Y.ToStringInvariant());
            file.AddProperty("pos2", _PosB.X.ToStringInvariant() + ", " + _PosB.Y.ToStringInvariant());
            file.AddProperty("label", Label);
        }

        public SimpleResult SetLabel(string text)
        {
            var Result = new SimpleResult();

            Result = _ParentMapLink.Source.ScriptLabelIsValid(text);
            if ( Result.Success )
            {
                Label = text;
            }
            return Result;
        }

        public void Deallocate()
        {
            _ParentMapLink.Deallocate();
        }
    }
}