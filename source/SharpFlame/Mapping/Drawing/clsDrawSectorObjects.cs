using System;
using System.Diagnostics;
using Matrix3D;
using OpenTK.Graphics.OpenGL;
using SharpFlame.AppSettings;
using SharpFlame.Domain;
using SharpFlame.Graphics.OpenGL;
using SharpFlame.Mapping.Objects;
using SharpFlame.Mapping.Tools;
using SharpFlame.Maths;

namespace SharpFlame.Mapping.Drawing
{
    public class clsDrawSectorObjects : clsAction
    {
        public clsTextLabels UnitTextLabels;

        private bool[] UnitDrawn;
        private bool Started;

        public void Start()
        {
            UnitDrawn = new bool[Map.Units.Count];

            Started = true;
        }

        public override void ActionPerform()
        {
            if ( !Started )
            {
                Debugger.Break();
                return;
            }

            clsUnit Unit = default(clsUnit);
            clsSector Sector = Map.Sectors[PosNum.X, PosNum.Y];
            bool DrawUnitLabel = default(bool);
            clsViewInfo ViewInfo = Map.ViewInfo;
            clsViewInfo.clsMouseOver.clsOverTerrain MouseOverTerrain = ViewInfo.GetMouseOverTerrain();
            clsTextLabel TextLabel = default(clsTextLabel);
            Position.XYZ_dbl XYZ_dbl = default(Position.XYZ_dbl);
            Position.XYZ_dbl XYZ_dbl2 = default(Position.XYZ_dbl);
            sXY_int ScreenPos = new sXY_int();
            clsUnitSectorConnection Connection = default(clsUnitSectorConnection);

            foreach ( clsUnitSectorConnection tempLoopVar_Connection in Sector.Units )
            {
                Connection = tempLoopVar_Connection;
                Unit = Connection.Unit;
                if ( !UnitDrawn[Unit.MapLink.ArrayPosition] )
                {
                    UnitDrawn[Unit.MapLink.ArrayPosition] = true;
                    XYZ_dbl.X = Unit.Pos.Horizontal.X - ViewInfo.ViewPos.X;
                    XYZ_dbl.Y = Unit.Pos.Altitude - ViewInfo.ViewPos.Y;
                    XYZ_dbl.Z = - Unit.Pos.Horizontal.Y - ViewInfo.ViewPos.Z;
                    DrawUnitLabel = false;
                    if ( Unit.TypeBase.IsUnknown )
                    {
                        DrawUnitLabel = true;
                    }
                    else
                    {
                        GL.PushMatrix();
                        GL.Translate(XYZ_dbl.X, XYZ_dbl.Y, Convert.ToDouble(- XYZ_dbl.Z));
                        Unit.TypeBase.GLDraw(Unit.Rotation);
                        GL.PopMatrix();
                        if ( Unit.TypeBase.Type == UnitType.PlayerDroid )
                        {
                            if ( ((DroidDesign)Unit.TypeBase).AlwaysDrawTextLabel )
                            {
                                DrawUnitLabel = true;
                            }
                        }
                        if ( MouseOverTerrain != null )
                        {
                            if ( MouseOverTerrain.Units.Count > 0 )
                            {
                                if ( MouseOverTerrain.Units[0] == Unit )
                                {
                                    DrawUnitLabel = true;
                                }
                            }
                        }
                    }
                    if ( DrawUnitLabel && !UnitTextLabels.AtMaxCount() )
                    {
                        Matrix3DMath.VectorRotationByMatrix(ViewInfo.ViewAngleMatrix_Inverted, XYZ_dbl, ref XYZ_dbl2);
                        if ( ViewInfo.Pos_Get_Screen_XY(XYZ_dbl2, ScreenPos) )
                        {
                            if ( ScreenPos.X >= 0 & ScreenPos.X <= ViewInfo.MapViewControl.GLSize.X & ScreenPos.Y >= 0 & ScreenPos.Y <= ViewInfo.MapViewControl.GLSize.Y )
                            {
                                TextLabel = new clsTextLabel();
                                TextLabel.TextFont = App.UnitLabelFont;
                                TextLabel.SizeY = SettingsManager.Settings.FontSize;
                                TextLabel.Colour.Red = 1.0F;
                                TextLabel.Colour.Green = 1.0F;
                                TextLabel.Colour.Blue = 1.0F;
                                TextLabel.Colour.Alpha = 1.0F;
                                TextLabel.Pos.X = ScreenPos.X + 32;
                                TextLabel.Pos.Y = ScreenPos.Y;
                                TextLabel.Text = Unit.TypeBase.GetDisplayTextCode();
                                UnitTextLabels.Add(TextLabel);
                            }
                        }
                    }
                }
            }
        }
    }
}