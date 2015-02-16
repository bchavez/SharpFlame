using System;
using System.Diagnostics;
using Eto.Drawing;
using OpenTK.Graphics.OpenGL;
using SharpFlame.Core.Domain;
using SharpFlame.Domain;
using SharpFlame.Graphics.OpenGL;
using SharpFlame.Mapping.Tools;
using SharpFlame.Maths;


namespace SharpFlame.Mapping.Drawing
{
    public class clsDrawSectorObjects : clsAction
    {
        private readonly Size glSize;
        private bool Started;
        private bool[] UnitDrawn;
        public clsTextLabels UnitTextLabels;

        internal ViewInfo ViewInfo { get; set; }

        public clsDrawSectorObjects(Size glSize, ViewInfo viewInfo)
        {
            this.glSize = glSize;
            this.ViewInfo = viewInfo;
        }

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

            var Sector = Map.Sectors[PosNum.X, PosNum.Y];
            var DrawUnitLabel = default(bool);
            var MouseOverTerrain = ViewInfo.GetMouseOverTerrain();
            var TextLabel = default(clsTextLabel);
            var XYZ_dbl = default(XYZDouble);
            var XYZ_dbl2 = default(XYZDouble);
            var ScreenPos = new XYInt();

            foreach ( var connection in Sector.Units )
            {
                var unit = connection.Unit;
                if ( !UnitDrawn[unit.MapLink.ArrayPosition] )
                {
                    UnitDrawn[unit.MapLink.ArrayPosition] = true;
                    XYZ_dbl.X = unit.Pos.Horizontal.X - ViewInfo.ViewPos.X;
                    XYZ_dbl.Y = unit.Pos.Altitude - ViewInfo.ViewPos.Y;
                    XYZ_dbl.Z = - unit.Pos.Horizontal.Y - ViewInfo.ViewPos.Z;
                    DrawUnitLabel = false;
                    if ( unit.TypeBase.IsUnknown )
                    {
                        DrawUnitLabel = true;
                    }
                    else
                    {
                        GL.PushMatrix();
                        GL.Translate(XYZ_dbl.X, XYZ_dbl.Y, Convert.ToDouble(- XYZ_dbl.Z));
                        unit.TypeBase.GLDraw(unit.Rotation);
                        GL.PopMatrix();
                        if ( unit.TypeBase.Type == UnitType.PlayerDroid )
                        {
                            if ( ((DroidDesign)unit.TypeBase).AlwaysDrawTextLabel )
                            {
                                DrawUnitLabel = true;
                            }
                        }
                        if ( MouseOverTerrain != null )
                        {
                            if ( MouseOverTerrain.Units.Count > 0 )
                            {
                                if ( MouseOverTerrain.Units[0] == unit )
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
                            if ( ScreenPos.X >= 0 & ScreenPos.X <= glSize.Width & ScreenPos.Y >= 0 & ScreenPos.Y <= glSize.Height )
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
                                TextLabel.Text = unit.TypeBase.GetDisplayTextCode();
                                UnitTextLabels.Add(TextLabel);
                            }
                        }
                    }
                }
            }
        }
    }
}