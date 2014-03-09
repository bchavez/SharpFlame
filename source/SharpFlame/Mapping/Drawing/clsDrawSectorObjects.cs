#region

using System;
using System.Diagnostics;
using Ninject;
using OpenTK.Graphics.OpenGL;
using SharpFlame.Core.Domain;
using SharpFlame.Gui.Sections;
using SharpFlame.Domain;
using SharpFlame.Graphics.OpenGL;
using SharpFlame.Mapping.Objects;
using SharpFlame.Mapping.Tools;
using SharpFlame.Maths;

#endregion

namespace SharpFlame.Mapping.Drawing
{
    public class clsDrawSectorObjects : clsAction
    {
        private bool Started;
        private bool[] UnitDrawn;
        public clsTextLabels UnitTextLabels;

        [Inject]
        internal MainMapView MainMapView { get; set; }

        [Inject]
        internal ViewInfo ViewInfo { get; set; }

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

            // Not the current map?
            if(ViewInfo.Map != Map)
            {
                Debugger.Break();
                return;
            }

            var Unit = default(Unit);
            var Sector = Map.Sectors[PosNum.X, PosNum.Y];
            var DrawUnitLabel = default(bool);
            var MouseOverTerrain = ViewInfo.GetMouseOverTerrain();
            var TextLabel = default(clsTextLabel);
            var XYZ_dbl = default(XYZDouble);
            var XYZ_dbl2 = default(XYZDouble);
            var ScreenPos = new XYInt();
            var Connection = default(clsUnitSectorConnection);

            foreach ( var tempLoopVar_Connection in Sector.Units )
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
                        Matrix3DMath.VectorRotationByMatrix(ViewInfo.ViewAngleMatrixInverted, XYZ_dbl, ref XYZ_dbl2);
                        if ( ViewInfo.PosGetScreenXY(XYZ_dbl2, ref ScreenPos) )
                        {
                            if ( ScreenPos.X >= 0 & ScreenPos.X <= MainMapView.GLSurface.Size.Width & ScreenPos.Y >= 0 & ScreenPos.Y <= MainMapView.GLSurface.Size.Height )
                            {
                                TextLabel = new clsTextLabel();
                                TextLabel.TextFont = App.UnitLabelFont;
                                TextLabel.SizeY = App.SettingsManager.FontSize;
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