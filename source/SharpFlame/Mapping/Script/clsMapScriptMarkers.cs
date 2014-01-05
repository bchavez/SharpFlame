using System;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;
using SharpFlame.Collections;
using SharpFlame.Colors;
using SharpFlame.FileIO.Ini;
using SharpFlame.Mapping.Drawing;
using SharpFlame.Mapping.Objects;
using SharpFlame.Maths;

namespace SharpFlame.Mapping
{
    public partial class clsMap
    {
        public ConnectedList<clsScriptPosition, clsMap> ScriptPositions;
        public ConnectedList<clsScriptArea, clsMap> ScriptAreas;

        public string GetDefaultScriptLabel(string Prefix)
        {
            int Number = 1;
            App.sResult Valid = new App.sResult();
            string Label = "";

            do
            {
                Label = Prefix + Number.ToStringInvariant();
                Valid = ScriptLabelIsValid(Label);
                if ( Valid.Success )
                {
                    return Label;
                }
                Number++;
                if ( Number >= 16384 )
                {
                    MessageBox.Show("Error: Unable to set default script label.");
                    return "";
                }
            } while ( true );
        }

        public App.sResult ScriptLabelIsValid(string Text)
        {
            App.sResult ReturnResult = new App.sResult();
            ReturnResult.Success = false;
            ReturnResult.Problem = "";

            if ( Text == null )
            {
                ReturnResult.Problem = "Label cannot be nothing.";
                return ReturnResult;
            }

            string LCaseText = Text.ToLower();

            if ( LCaseText.Length < 1 )
            {
                ReturnResult.Problem = "Label cannot be nothing.";
                return ReturnResult;
            }

            char CurrentChar = (char)0;
            bool Invalid = default(bool);

            Invalid = false;
            foreach ( char tempLoopVar_CurrentChar in LCaseText )
            {
                CurrentChar = tempLoopVar_CurrentChar;
                if ( !((CurrentChar >= 'a' && CurrentChar <= 'z') || (CurrentChar >= '0' && CurrentChar <= '9') || CurrentChar == '_') )
                {
                    Invalid = true;
                    break;
                }
            }
            if ( Invalid )
            {
                ReturnResult.Problem = "Label contains invalid characters. Use only letters, numbers or underscores.";
                return ReturnResult;
            }

            clsUnit Unit = default(clsUnit);

            foreach ( clsUnit tempLoopVar_Unit in Units )
            {
                Unit = tempLoopVar_Unit;
                if ( Unit.Label != null )
                {
                    if ( LCaseText == Unit.Label.ToLower() )
                    {
                        ReturnResult.Problem = "Label text is already in use.";
                        return ReturnResult;
                    }
                }
            }

            clsScriptPosition ScriptPosition = default(clsScriptPosition);

            foreach ( clsScriptPosition tempLoopVar_ScriptPosition in ScriptPositions )
            {
                ScriptPosition = tempLoopVar_ScriptPosition;
                if ( LCaseText == ScriptPosition.Label.ToLower() )
                {
                    ReturnResult.Problem = "Label text is already in use.";
                    return ReturnResult;
                }
            }

            clsScriptArea ScriptArea = default(clsScriptArea);

            foreach ( clsScriptArea tempLoopVar_ScriptArea in ScriptAreas )
            {
                ScriptArea = tempLoopVar_ScriptArea;
                if ( LCaseText == ScriptArea.Label.ToLower() )
                {
                    ReturnResult.Problem = "Label text is already in use.";
                    return ReturnResult;
                }
            }

            ReturnResult.Success = true;
            return ReturnResult;
        }

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

            //private clsScriptPosition()
            //{

            //_ParentMapLink = new ConnectedListLink<clsScriptPosition, clsMap>(this);


            //}

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

            public App.sResult SetLabel(string Text)
            {
                App.sResult Result = new App.sResult();

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

        public class clsScriptArea
        {
            public clsScriptArea()
            {
                _ParentMapLink = new ConnectedListLink<clsScriptArea, clsMap>(this);
            }

            private ConnectedListLink<clsScriptArea, clsMap> _ParentMapLink;

            public ConnectedListLink<clsScriptArea, clsMap> ParentMap
            {
                get { return _ParentMapLink; }
            }

            private string _Label;

            public string Label
            {
                get { return _Label; }
            }

            private sXY_int _PosA;
            private sXY_int _PosB;

            public sXY_int PosA
            {
                set
                {
                    clsMap Map = _ParentMapLink.Source;
                    _PosA.X = MathUtil.Clamp_int(value.X, 0, Map.Terrain.TileSize.X * App.TerrainGridSpacing - 1);
                    _PosA.Y = MathUtil.Clamp_int(value.Y, 0, Map.Terrain.TileSize.Y * App.TerrainGridSpacing - 1);
                    MathUtil.ReorderXY(_PosA, _PosB, _PosA, _PosB);
                }
            }

            public sXY_int PosB
            {
                set
                {
                    clsMap Map = _ParentMapLink.Source;
                    _PosB.X = MathUtil.Clamp_int(value.X, 0, Map.Terrain.TileSize.X * App.TerrainGridSpacing - 1);
                    _PosB.Y = MathUtil.Clamp_int(value.Y, 0, Map.Terrain.TileSize.Y * App.TerrainGridSpacing - 1);
                    MathUtil.ReorderXY(_PosA, _PosB, _PosA, _PosB);
                }
            }

            public int PosAX
            {
                get { return _PosA.X; }
                set
                {
                    _PosA.X = MathUtil.Clamp_int(value, 0,
                        Convert.ToInt32(Convert.ToInt32(_ParentMapLink.Source.Terrain.TileSize.X * App.TerrainGridSpacing) - 1));
                    MathUtil.ReorderXY(_PosA, _PosB, _PosA, _PosB);
                }
            }

            public int PosAY
            {
                get { return _PosA.Y; }
                set
                {
                    _PosA.Y = MathUtil.Clamp_int(value, 0,
                        Convert.ToInt32(Convert.ToInt32(_ParentMapLink.Source.Terrain.TileSize.Y * App.TerrainGridSpacing) - 1));
                    MathUtil.ReorderXY(_PosA, _PosB, _PosA, _PosB);
                }
            }

            public int PosBX
            {
                get { return _PosB.X; }
                set
                {
                    _PosB.X = MathUtil.Clamp_int(value, 0,
                        Convert.ToInt32(Convert.ToInt32(_ParentMapLink.Source.Terrain.TileSize.X * App.TerrainGridSpacing) - 1));
                    MathUtil.ReorderXY(_PosA, _PosB, _PosA, _PosB);
                }
            }

            public int PosBY
            {
                get { return _PosB.Y; }
                set
                {
                    _PosB.Y = MathUtil.Clamp_int(value, 0,
                        Convert.ToInt32(Convert.ToInt32(_ParentMapLink.Source.Terrain.TileSize.Y * App.TerrainGridSpacing) - 1));
                    MathUtil.ReorderXY(_PosA, _PosB, _PosA, _PosB);
                }
            }

            //protected clsScriptArea()
            //{

            //_ParentMapLink = new ConnectedListLink<clsScriptArea, clsMap>(this);


            //}

            public static clsScriptArea Create(clsMap Map)
            {
                clsScriptArea Result = new clsScriptArea();

                Result._Label = Map.GetDefaultScriptLabel("Area");

                Result._ParentMapLink.Connect(Map.ScriptAreas);

                return Result;
            }

            public void SetPositions(sXY_int PosA, sXY_int PosB)
            {
                clsMap Map = _ParentMapLink.Source;

                PosA.X = MathUtil.Clamp_int(PosA.X, 0, Map.Terrain.TileSize.X * App.TerrainGridSpacing - 1);
                PosA.Y = MathUtil.Clamp_int(PosA.Y, 0, Map.Terrain.TileSize.Y * App.TerrainGridSpacing - 1);
                PosB.X = MathUtil.Clamp_int(PosB.X, 0, Map.Terrain.TileSize.X * App.TerrainGridSpacing - 1);
                PosB.Y = MathUtil.Clamp_int(PosB.Y, 0, Map.Terrain.TileSize.Y * App.TerrainGridSpacing - 1);

                MathUtil.ReorderXY(PosA, PosB, _PosA, _PosB);
            }

            public void GLDraw()
            {
                clsDrawTerrainLine Drawer = new clsDrawTerrainLine();
                Drawer.Map = _ParentMapLink.Source;
                if ( Program.frmMainInstance.SelectedScriptMarker == this )
                {
                    GL.LineWidth(4.5F);
                    Drawer.Colour = new sRGBA_sng(1.0F, 1.0F, 0.5F, 0.75F);
                }
                else
                {
                    GL.LineWidth(3.0F);
                    Drawer.Colour = new sRGBA_sng(1.0F, 1.0F, 0.0F, 0.5F);
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

            public void MapResizing(sXY_int PosOffset)
            {
                SetPositions(new sXY_int(_PosA.X - PosOffset.X, _PosA.Y - PosOffset.Y), new sXY_int(_PosB.X - PosOffset.X, _PosB.Y - PosOffset.Y));
            }

            public void WriteWZ(IniWriter File)
            {
                File.AppendSectionName("area_" + _ParentMapLink.ArrayPosition.ToStringInvariant());
                File.AppendProperty("pos1", _PosA.X.ToStringInvariant() + ", " + _PosA.Y.ToStringInvariant());
                File.AppendProperty("pos2", _PosB.X.ToStringInvariant() + ", " + _PosB.Y.ToStringInvariant());
                File.AppendProperty("label", _Label);
                File.Gap_Append();
            }

            public App.sResult SetLabel(string Text)
            {
                App.sResult Result = new App.sResult();

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
}