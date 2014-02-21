#region

using System;
using System.Windows.Forms;
using SharpFlame.AppSettings;
using SharpFlame.Collections;
using SharpFlame.Controls;
using SharpFlame.Core.Domain;
using SharpFlame.FileIO;
using SharpFlame.Mapping;
using SharpFlame.Mapping.Objects;
using SharpFlame.Mapping.Script;
using SharpFlame.Mapping.Tools;
using SharpFlame.Maths;
using SharpFlame.Painters;
using SharpFlame.Util;

#endregion

namespace SharpFlame
{
    public class clsViewInfo
    {
        public double FOVMultiplier;
        public double FOVMultiplierExponent;
        public float FieldOfViewY;
        public clsMap Map;
        public MapViewControl MapViewControl;
        public clsMouseDown MouseLeftDown;
        public clsMouseOver MouseOver;
        public clsMouseDown MouseRightDown;
        public double TilesPerMinimapPixel;
        public Matrix3DMath.Matrix3D ViewAngleMatrix = new Matrix3DMath.Matrix3D();
        public Matrix3DMath.Matrix3D ViewAngleMatrixInverted = new Matrix3DMath.Matrix3D();
        public Angles.AngleRPY ViewAngleRPY;
        public XYZInt ViewPos;

        public clsViewInfo(clsMap map, MapViewControl mapViewControl)
        {
            Map = map;
            MapViewControl = mapViewControl;

            ViewPos = new XYZInt(0, 3072, 0);
            FovMultiplierSet(SettingsManager.Settings.FOVDefault);
            ViewAngleSetToDefault();
            LookAtPos(new XYInt((int)(map.Terrain.TileSize.X * Constants.TerrainGridSpacing / 2.0D),
                (int)(map.Terrain.TileSize.Y * Constants.TerrainGridSpacing / 2.0D)));
        }

        public void FovScale_2ESet(double power)
        {
            FOVMultiplierExponent = power;
            FOVMultiplier = Math.Pow(2.0D, FOVMultiplierExponent);

            FovCalc();
        }

        public void FovScale_2EChange(double powerChange)
        {
            FOVMultiplierExponent += powerChange;
            FOVMultiplier = Math.Pow(2.0D, FOVMultiplierExponent);

            FovCalc();
        }

        public void FovSet(double radians, MapViewControl mapViewControl)
        {
            FOVMultiplier = Math.Tan(radians / 2.0D) / mapViewControl.GLSize.Y * 2.0D;
            FOVMultiplierExponent = Math.Log(FOVMultiplier) / Math.Log(2.0D);

            FovCalc();
        }

        public void FovMultiplierSet(double value)
        {
            FOVMultiplier = value;
            FOVMultiplierExponent = Math.Log(FOVMultiplier) / Math.Log(2.0D);

            FovCalc();
        }

        public void FovCalc()
        {
            const float min = (float)(0.1d * MathUtil.RadOf1Deg);
            const float max = (float)(179.0d * MathUtil.RadOf1Deg);

            FieldOfViewY = (float)(Math.Atan(MapViewControl.GLSize.Y * FOVMultiplier / 2.0D) * 2.0D);
            if ( FieldOfViewY < min )
            {
                FieldOfViewY = min;
                if ( MapViewControl.GLSize.Y > 0 )
                {
                    FOVMultiplier = 2.0D * Math.Tan(FieldOfViewY / 2.0D) / MapViewControl.GLSize.Y;
                    FOVMultiplierExponent = Math.Log(FOVMultiplier) / Math.Log(2.0D);
                }
            }
            else if ( FieldOfViewY > max )
            {
                FieldOfViewY = max;
                if ( MapViewControl.GLSize.Y > 0 )
                {
                    FOVMultiplier = 2.0D * Math.Tan(FieldOfViewY / 2.0D) / MapViewControl.GLSize.Y;
                    FOVMultiplierExponent = Math.Log(FOVMultiplier) / Math.Log(2.0D);
                }
            }

            MapViewControl.DrawViewLater();
        }

        public void ViewPosSet(XYZInt newViewPos)
        {
            ViewPos = newViewPos;
            ViewPosClamp();

            MapViewControl.DrawViewLater();
        }

        public void ViewPosChange(XYZInt displacement)
        {
            ViewPos.X += displacement.X;
            ViewPos.Z += displacement.Z;
            ViewPos.Y += displacement.Y;
            ViewPosClamp();

            MapViewControl.DrawViewLater();
        }

        private void ViewPosClamp()
        {
            const int maxHeight = 1048576;
            const int maxDist = 1048576;

            ViewPos.X = MathUtil.ClampInt(ViewPos.X, Convert.ToInt32(- maxDist), Map.Terrain.TileSize.X * Constants.TerrainGridSpacing + maxDist);
            ViewPos.Z = MathUtil.ClampInt(ViewPos.Z, - Map.Terrain.TileSize.Y * Constants.TerrainGridSpacing - maxDist, maxDist);
            ViewPos.Y = MathUtil.ClampInt(ViewPos.Y, ((int)(Math.Ceiling(Map.GetTerrainHeight(new XYInt(ViewPos.X, - ViewPos.Z))))) + 16, maxHeight);
        }

        public void ViewAngleSet(Matrix3DMath.Matrix3D newMatrix)
        {
            Matrix3DMath.MatrixCopy(newMatrix, ViewAngleMatrix);
            Matrix3DMath.MatrixNormalize(ViewAngleMatrix);
            Matrix3DMath.MatrixInvert(ViewAngleMatrix, ViewAngleMatrixInverted);
            Matrix3DMath.MatrixToRPY(ViewAngleMatrix, ref ViewAngleRPY);

            MapViewControl.DrawViewLater();
        }

        public void ViewAngleSetToDefault()
        {
            var matrixA = new Matrix3DMath.Matrix3D();
            Matrix3DMath.MatrixSetToXAngle(matrixA, Math.Atan(2.0D));
            ViewAngleSet(matrixA);

            MapViewControl.DrawViewLater();
        }

        public void ViewAngleSetRotate(Matrix3DMath.Matrix3D newMatrix)
        {
            var flag = default(bool);
            XYZDouble xyzDbl = default(XYZDouble);
            XYDouble xyDbl = default(XYDouble);

            if ( App.ViewMoveType == ViewMoveType.RTS & App.RTSOrbit )
            {
                if ( ScreenXYGetViewPlanePosForwardDownOnly((int)((MapViewControl.GLSize.X / 2.0D)), (int)((MapViewControl.GLSize.Y / 2.0D)), 127.5D,
                    ref xyDbl) )
                {
                    xyzDbl.X = xyDbl.X;
                    xyzDbl.Y = 127.5D;
                    xyzDbl.Z = Convert.ToDouble(- xyDbl.Y);
                    flag = true;
                }
            }

            Matrix3DMath.MatrixToRPY(newMatrix, ref ViewAngleRPY);
            if ( flag )
            {
                if ( ViewAngleRPY.Pitch < MathUtil.RadOf1Deg * 10.0D )
                {
                    ViewAngleRPY.Pitch = MathUtil.RadOf1Deg * 10.0D;
                }
            }
            Matrix3DMath.MatrixSetToRPY(ViewAngleMatrix, ViewAngleRPY);
            Matrix3DMath.MatrixInvert(ViewAngleMatrix, ViewAngleMatrixInverted);

            if ( flag )
            {
                var xyzDbl2 = new XYZDouble(ViewPos.X, ViewPos.Y, Convert.ToDouble(- ViewPos.Z));
                MoveToViewTerrainPosFromDistance(xyzDbl, Convert.ToDouble((xyzDbl2 - xyzDbl).GetMagnitude()));
            }

            MapViewControl.DrawViewLater();
        }

        public void LookAtTile(XYInt tileNum)
        {
            var pos = new XYInt(0, 0);

            pos.X = (int)((tileNum.X + 0.5D) * Constants.TerrainGridSpacing);
            pos.Y = (int)((tileNum.Y + 0.5D) * Constants.TerrainGridSpacing);
            LookAtPos(pos);
        }

        public void LookAtPos(XYInt horizontal)
        {
            var xyzDbl = default(XYZDouble);
            var xyzInt = new XYZInt(0, 0, 0);
            var matrixA = new Matrix3DMath.Matrix3D();
            var anglePy = default(Angles.AnglePY);

            Matrix3DMath.VectorForwardsRotationByMatrix(ViewAngleMatrix, ref xyzDbl);
            var dblTemp = Map.GetTerrainHeight(horizontal);
            var i = ((int)(Math.Ceiling(dblTemp))) + 128;
            if ( ViewPos.Y < i )
            {
                ViewPos.Y = i;
            }
            if ( xyzDbl.Y > -0.33333333333333331D )
            {
                xyzDbl.Y = -0.33333333333333331D;
                Matrix3DMath.VectorToPY(xyzDbl, ref anglePy);
                Matrix3DMath.MatrixSetToPY(matrixA, anglePy);
                ViewAngleSet(matrixA);
            }
            dblTemp = (ViewPos.Y - dblTemp) / xyzDbl.Y;

            xyzInt.X = (int)(horizontal.X + dblTemp * xyzDbl.X);
            xyzInt.Y = ViewPos.Y;
            xyzInt.Z = (int)(- horizontal.Y + dblTemp * xyzDbl.Z);

            ViewPosSet(xyzInt);
        }

        public void MoveToViewTerrainPosFromDistance(XYZDouble TerrainPos, double Distance)
        {
            var XYZ_dbl = default(XYZDouble);
            var XYZ_int = new XYZInt(0, 0, 0);

            Matrix3DMath.VectorForwardsRotationByMatrix(ViewAngleMatrix, ref XYZ_dbl);

            XYZ_int.X = (int)(TerrainPos.X - XYZ_dbl.X * Distance);
            XYZ_int.Y = (int)(TerrainPos.Y - XYZ_dbl.Y * Distance);
            XYZ_int.Z = (int)(- TerrainPos.Z - XYZ_dbl.Z * Distance);

            ViewPosSet(XYZ_int);
        }

        public bool PosGetScreenXY(XYZDouble Pos, ref XYInt Result)
        {
            if ( Pos.Z <= 0.0D )
            {
                return false;
            }

            try
            {
                var RatioZ_px = 1.0D / (FOVMultiplier * Pos.Z);
                Result.X = (int)(MapViewControl.GLSize.X / 2.0D + (Pos.X * RatioZ_px));
                Result.Y = (int)(MapViewControl.GLSize.Y / 2.0D - (Pos.Y * RatioZ_px));
                return true;
            }
            catch
            {
            }

            return false;
        }

        public bool ScreenXYGetViewPlanePos(XYInt screenPos, double planeHeight, ref XYDouble resultPos)
        {
            double dblTemp;
            var xyzDbl = default(XYZDouble);
            var xyzDbl2 = default(XYZDouble);

            try
            {
                //convert screen pos to vector of one pos unit
                xyzDbl.X = (screenPos.X - MapViewControl.GLSize.X / 2.0D) * FOVMultiplier;
                xyzDbl.Y = (MapViewControl.GLSize.Y / 2.0D - screenPos.Y) * FOVMultiplier;
                xyzDbl.Z = 1.0D;
                //factor in the view angle
                Matrix3DMath.VectorRotationByMatrix(ViewAngleMatrix, xyzDbl, ref xyzDbl2);
                //get distance to cover the height
                dblTemp = (planeHeight - ViewPos.Y) / xyzDbl2.Y;
                resultPos.X = ViewPos.X + xyzDbl2.X * dblTemp;
                resultPos.Y = ViewPos.Z + xyzDbl2.Z * dblTemp;
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool ScreenXYGetTerrainPos(XYInt screenPos, ref WorldPos resultPos)
        {
            double dblTemp = 0;
            var xYZ_dbl = default(XYZDouble);
            var terrainViewVector = default(XYZDouble);
            var x = 0;
            var y = 0;
            var limitA = default(XYDouble);
            var limitB = default(XYDouble);
            var min = new XYInt();
            var max = new XYInt();
            double triGradientX = 0;
            double triGradientZ = 0;
            double triHeightOffset = 0;
            double dist = 0;
            var bestPos = default(XYZDouble);
            double bestDist = 0;
            var dif = default(XYZDouble);
            double inTileX = 0;
            double inTileZ = 0;
            var tilePos = default(XYDouble);
            var terrainViewPos = default(XYZDouble);

            try
            {
                terrainViewPos.X = ViewPos.X;
                terrainViewPos.Y = ViewPos.Y;
                terrainViewPos.Z = Convert.ToDouble(- ViewPos.Z);

                //convert screen pos to vector of one pos unit
                xYZ_dbl.X = (screenPos.X - MapViewControl.GLSize.X / 2.0D) * FOVMultiplier;
                xYZ_dbl.Y = (MapViewControl.GLSize.Y / 2.0D - screenPos.Y) * FOVMultiplier;
                xYZ_dbl.Z = 1.0D;
                //rotate the vector so that it points forward and level
                Matrix3DMath.VectorRotationByMatrix(ViewAngleMatrix, xYZ_dbl, ref terrainViewVector);
                terrainViewVector.Y = Convert.ToDouble(- terrainViewVector.Y); //get the amount of looking down, not up
                terrainViewVector.Z = Convert.ToDouble(- terrainViewVector.Z); //convert to terrain coordinates from view coordinates
                //get range of possible tiles
                dblTemp = (terrainViewPos.Y - 255 * Map.HeightMultiplier) / terrainViewVector.Y;
                limitA.X = terrainViewPos.X + terrainViewVector.X * dblTemp;
                limitA.Y = terrainViewPos.Z + terrainViewVector.Z * dblTemp;
                dblTemp = terrainViewPos.Y / terrainViewVector.Y;
                limitB.X = terrainViewPos.X + terrainViewVector.X * dblTemp;
                limitB.Y = terrainViewPos.Z + terrainViewVector.Z * dblTemp;
                min.X = Math.Max(Convert.ToInt32((Math.Min(limitA.X, limitB.X) / Constants.TerrainGridSpacing)), 0);
                min.Y = Math.Max((int)((Math.Min(limitA.Y, limitB.Y) / Constants.TerrainGridSpacing)), 0);
                max.X = Math.Min(Convert.ToInt32((Math.Max(limitA.X, limitB.X) / Constants.TerrainGridSpacing)), Map.Terrain.TileSize.X - 1);
                max.Y = Math.Min(Convert.ToInt32((Math.Max(limitA.Y, limitB.Y) / Constants.TerrainGridSpacing)), Map.Terrain.TileSize.Y - 1);
                //find the nearest valid tile to the view
                bestDist = double.MaxValue;
                bestPos.X = double.NaN;
                bestPos.Y = double.NaN;
                bestPos.Z = double.NaN;
                for ( y = min.Y; y <= max.Y; y++ )
                {
                    for ( x = min.X; x <= max.X; x++ )
                    {
                        tilePos.X = x * Constants.TerrainGridSpacing;
                        tilePos.Y = y * Constants.TerrainGridSpacing;

                        if ( Map.Terrain.Tiles[x, y].Tri )
                        {
                            triHeightOffset = Convert.ToDouble(Map.Terrain.Vertices[x, y].Height * Map.HeightMultiplier);
                            triGradientX = Convert.ToDouble(Map.Terrain.Vertices[x + 1, y].Height * Map.HeightMultiplier - triHeightOffset);
                            triGradientZ = Convert.ToDouble(Map.Terrain.Vertices[x, y + 1].Height * Map.HeightMultiplier - triHeightOffset);
                            xYZ_dbl.Y = (triHeightOffset +
                                         (triGradientX * (terrainViewPos.X - tilePos.X) + triGradientZ * (terrainViewPos.Z - tilePos.Y) +
                                          (triGradientX * terrainViewVector.X + triGradientZ * terrainViewVector.Z) * terrainViewPos.Y / terrainViewVector.Y) /
                                         Constants.TerrainGridSpacing) /
                                        (1.0D +
                                         (triGradientX * terrainViewVector.X + triGradientZ * terrainViewVector.Z) / (terrainViewVector.Y * Constants.TerrainGridSpacing));
                            xYZ_dbl.X = terrainViewPos.X + terrainViewVector.X * (terrainViewPos.Y - xYZ_dbl.Y) / terrainViewVector.Y;
                            xYZ_dbl.Z = terrainViewPos.Z + terrainViewVector.Z * (terrainViewPos.Y - xYZ_dbl.Y) / terrainViewVector.Y;
                            inTileX = xYZ_dbl.X / Constants.TerrainGridSpacing - x;
                            inTileZ = xYZ_dbl.Z / Constants.TerrainGridSpacing - y;
                            if ( inTileZ <= 1.0D - inTileX & inTileX >= 0.0D & inTileZ >= 0.0D & inTileX <= 1.0D & inTileZ <= 1.0D )
                            {
                                dif = xYZ_dbl - terrainViewPos;
                                dist = dif.GetMagnitude();
                                if ( dist < bestDist )
                                {
                                    bestDist = dist;
                                    bestPos = xYZ_dbl;
                                }
                            }

                            triHeightOffset = Convert.ToDouble(Map.Terrain.Vertices[x + 1, y + 1].Height * Map.HeightMultiplier);
                            triGradientX = Convert.ToDouble(Map.Terrain.Vertices[x, y + 1].Height * Map.HeightMultiplier - triHeightOffset);
                            triGradientZ = Convert.ToDouble(Map.Terrain.Vertices[x + 1, y].Height * Map.HeightMultiplier - triHeightOffset);
                            xYZ_dbl.Y = (triHeightOffset + triGradientX + triGradientZ +
                                         (triGradientX * (tilePos.X - terrainViewPos.X) + triGradientZ * (tilePos.Y - terrainViewPos.Z) -
                                          (triGradientX * terrainViewVector.X + triGradientZ * terrainViewVector.Z) * terrainViewPos.Y / terrainViewVector.Y) /
                                         Constants.TerrainGridSpacing) /
                                        (1.0D -
                                         (triGradientX * terrainViewVector.X + triGradientZ * terrainViewVector.Z) / (terrainViewVector.Y * Constants.TerrainGridSpacing));
                            xYZ_dbl.X = terrainViewPos.X + terrainViewVector.X * (terrainViewPos.Y - xYZ_dbl.Y) / terrainViewVector.Y;
                            xYZ_dbl.Z = terrainViewPos.Z + terrainViewVector.Z * (terrainViewPos.Y - xYZ_dbl.Y) / terrainViewVector.Y;
                            inTileX = xYZ_dbl.X / Constants.TerrainGridSpacing - x;
                            inTileZ = xYZ_dbl.Z / Constants.TerrainGridSpacing - y;
                            if ( inTileZ >= 1.0D - inTileX & inTileX >= 0.0D & inTileZ >= 0.0D & inTileX <= 1.0D & inTileZ <= 1.0D )
                            {
                                dif = xYZ_dbl - terrainViewPos;
                                dist = dif.GetMagnitude();
                                if ( dist < bestDist )
                                {
                                    bestDist = dist;
                                    bestPos = xYZ_dbl;
                                }
                            }
                        }
                        else
                        {
                            triHeightOffset = Convert.ToDouble(Map.Terrain.Vertices[x + 1, y].Height * Map.HeightMultiplier);
                            triGradientX = Convert.ToDouble(Map.Terrain.Vertices[x, y].Height * Map.HeightMultiplier - triHeightOffset);
                            triGradientZ = Convert.ToDouble(Map.Terrain.Vertices[x + 1, y + 1].Height * Map.HeightMultiplier - triHeightOffset);
                            xYZ_dbl.Y = (triHeightOffset + triGradientX +
                                         (triGradientX * (tilePos.X - terrainViewPos.X) + triGradientZ * (terrainViewPos.Z - tilePos.Y) -
                                          (triGradientX * terrainViewVector.X - triGradientZ * terrainViewVector.Z) * terrainViewPos.Y / terrainViewVector.Y) /
                                         Constants.TerrainGridSpacing) /
                                        (1.0D -
                                         (triGradientX * terrainViewVector.X - triGradientZ * terrainViewVector.Z) / (terrainViewVector.Y * Constants.TerrainGridSpacing));
                            xYZ_dbl.X = terrainViewPos.X + terrainViewVector.X * (terrainViewPos.Y - xYZ_dbl.Y) / terrainViewVector.Y;
                            xYZ_dbl.Z = terrainViewPos.Z + terrainViewVector.Z * (terrainViewPos.Y - xYZ_dbl.Y) / terrainViewVector.Y;
                            inTileX = xYZ_dbl.X / Constants.TerrainGridSpacing - x;
                            inTileZ = xYZ_dbl.Z / Constants.TerrainGridSpacing - y;
                            if ( inTileZ <= inTileX & inTileX >= 0.0D & inTileZ >= 0.0D & inTileX <= 1.0D & inTileZ <= 1.0D )
                            {
                                dif = xYZ_dbl - terrainViewPos;
                                dist = dif.GetMagnitude();
                                if ( dist < bestDist )
                                {
                                    bestDist = dist;
                                    bestPos = xYZ_dbl;
                                }
                            }

                            triHeightOffset = Convert.ToDouble(Map.Terrain.Vertices[x, y + 1].Height * Map.HeightMultiplier);
                            triGradientX = Convert.ToDouble(Map.Terrain.Vertices[x + 1, y + 1].Height * Map.HeightMultiplier - triHeightOffset);
                            triGradientZ = Convert.ToDouble(Map.Terrain.Vertices[x, y].Height * Map.HeightMultiplier - triHeightOffset);
                            xYZ_dbl.Y = (triHeightOffset + triGradientZ +
                                         (triGradientX * (terrainViewPos.X - tilePos.X) + triGradientZ * (tilePos.Y - terrainViewPos.Z) +
                                          (triGradientX * terrainViewVector.X - triGradientZ * terrainViewVector.Z) * terrainViewPos.Y / terrainViewVector.Y) /
                                         Constants.TerrainGridSpacing) /
                                        (1.0D +
                                         (triGradientX * terrainViewVector.X - triGradientZ * terrainViewVector.Z) / (terrainViewVector.Y * Constants.TerrainGridSpacing));
                            xYZ_dbl.X = terrainViewPos.X + terrainViewVector.X * (terrainViewPos.Y - xYZ_dbl.Y) / terrainViewVector.Y;
                            xYZ_dbl.Z = terrainViewPos.Z + terrainViewVector.Z * (terrainViewPos.Y - xYZ_dbl.Y) / terrainViewVector.Y;
                            inTileX = xYZ_dbl.X / Constants.TerrainGridSpacing - x;
                            inTileZ = xYZ_dbl.Z / Constants.TerrainGridSpacing - y;
                            if ( inTileZ >= inTileX & inTileX >= 0.0D & inTileZ >= 0.0D & inTileX <= 1.0D & inTileZ <= 1.0D )
                            {
                                dif = xYZ_dbl - terrainViewPos;
                                dist = dif.GetMagnitude();
                                if ( dist < bestDist )
                                {
                                    bestDist = dist;
                                    bestPos = xYZ_dbl;
                                }
                            }
                        }
                    }
                }

                if ( bestPos.X == double.NaN )
                {
                    return false;
                }

                resultPos.Horizontal.X = (int)bestPos.X;
                resultPos.Altitude = (int)bestPos.Y;
                resultPos.Horizontal.Y = (int)bestPos.Z;
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool ScreenXYGetViewPlanePosForwardDownOnly(int screenX, int screenY, double planeHeight, ref XYDouble resultPos)
        {
            double dblTemp = 0;
            var xYZ_dbl = default(XYZDouble);
            var xYZ_dbl2 = default(XYZDouble);
            double dblTemp2 = 0;

            if ( ViewPos.Y < planeHeight )
            {
                return false;
            }

            try
            {
                //convert screen pos to vector of one pos unit
                dblTemp2 = FOVMultiplier;
                xYZ_dbl.X = (screenX - MapViewControl.GLSize.X / 2.0D) * dblTemp2;
                xYZ_dbl.Y = (MapViewControl.GLSize.Y / 2.0D - screenY) * dblTemp2;
                xYZ_dbl.Z = 1.0D;
                //factor in the view angle
                Matrix3DMath.VectorRotationByMatrix(ViewAngleMatrix, xYZ_dbl, ref xYZ_dbl2);
                //get distance to cover the height
                if ( xYZ_dbl2.Y > 0.0D )
                {
                    return false;
                }
                dblTemp = (planeHeight - ViewPos.Y) / xYZ_dbl2.Y;
                resultPos.X = ViewPos.X + xYZ_dbl2.X * dblTemp;
                resultPos.Y = ViewPos.Z + xYZ_dbl2.Z * dblTemp;
            }
            catch
            {
                return false;
            }
            return true;
        }

        public void MouseOverPosCalc()
        {
            var xYDbl = default(XYDouble);
            var flag = false;
            var footprint = new XYInt();
            var mouseLeftDownOverMinimap = GetMouseLeftDownOverMinimap();

            if ( mouseLeftDownOverMinimap != null )
            {
                if ( MouseOver == null )
                {
                }
                else if ( IsViewPosOverMinimap(MouseOver.ScreenPos) )
                {
                    var Pos = new XYInt((int)(MouseOver.ScreenPos.X * TilesPerMinimapPixel),
                        (int)((MouseOver.ScreenPos.Y * TilesPerMinimapPixel)));
                    Map.TileNumClampToMap(Pos);
                    LookAtTile(Pos);
                }
            }
            else
            {
                var mouseOverTerrain = new clsMouseOver.clsOverTerrain();
                if ( SettingsManager.Settings.DirectPointer )
                {
                    if ( ScreenXYGetTerrainPos(MouseOver.ScreenPos, ref mouseOverTerrain.Pos) )
                    {
                        if ( Map.PosIsOnMap(mouseOverTerrain.Pos.Horizontal) )
                        {
                            flag = true;
                        }
                    }
                }
                else
                {
                    mouseOverTerrain.Pos.Altitude = (int)(255.0D / 2.0D * Map.HeightMultiplier);
                    if ( ScreenXYGetViewPlanePos(MouseOver.ScreenPos, mouseOverTerrain.Pos.Altitude, ref xYDbl) )
                    {
                        mouseOverTerrain.Pos.Horizontal.X = (int)xYDbl.X;
                        mouseOverTerrain.Pos.Horizontal.Y = Convert.ToInt32(- xYDbl.Y);
                        if ( Map.PosIsOnMap(mouseOverTerrain.Pos.Horizontal) )
                        {
                            mouseOverTerrain.Pos.Altitude = (int)(Map.GetTerrainHeight(mouseOverTerrain.Pos.Horizontal));
                            flag = true;
                        }
                    }
                }
                if ( flag )
                {
                    MouseOver.OverTerrain = mouseOverTerrain;
                    mouseOverTerrain.Tile.Normal.X = (int)((double)mouseOverTerrain.Pos.Horizontal.X / Constants.TerrainGridSpacing);
                    mouseOverTerrain.Tile.Normal.Y = (int)(((double)mouseOverTerrain.Pos.Horizontal.Y / Constants.TerrainGridSpacing));
                    mouseOverTerrain.Vertex.Normal.X = (int)(Math.Round(((double)mouseOverTerrain.Pos.Horizontal.X / Constants.TerrainGridSpacing)));
                    mouseOverTerrain.Vertex.Normal.Y = (int)(Math.Round(((double)mouseOverTerrain.Pos.Horizontal.Y / Constants.TerrainGridSpacing)));
                    mouseOverTerrain.Tile.Alignment = mouseOverTerrain.Vertex.Normal;
                    mouseOverTerrain.Vertex.Alignment = new XYInt(mouseOverTerrain.Tile.Normal.X + 1, mouseOverTerrain.Tile.Normal.Y + 1);
                    mouseOverTerrain.Triangle = Map.GetTerrainTri(mouseOverTerrain.Pos.Horizontal);
                    xYDbl.X = mouseOverTerrain.Pos.Horizontal.X - mouseOverTerrain.Vertex.Normal.X * Constants.TerrainGridSpacing;
                    xYDbl.Y = mouseOverTerrain.Pos.Horizontal.Y - mouseOverTerrain.Vertex.Normal.Y * Constants.TerrainGridSpacing;
                    if ( Math.Abs(xYDbl.Y) <= Math.Abs(xYDbl.X) )
                    {
                        mouseOverTerrain.Side_IsV = false;
                        mouseOverTerrain.Side_Num.X = mouseOverTerrain.Tile.Normal.X;
                        mouseOverTerrain.Side_Num.Y = mouseOverTerrain.Vertex.Normal.Y;
                    }
                    else
                    {
                        mouseOverTerrain.Side_IsV = true;
                        mouseOverTerrain.Side_Num.X = mouseOverTerrain.Vertex.Normal.X;
                        mouseOverTerrain.Side_Num.Y = mouseOverTerrain.Tile.Normal.Y;
                    }
                    var sectorNum = Map.GetPosSectorNum(mouseOverTerrain.Pos.Horizontal);
                    var unit = default(clsUnit);
                    var connection = default(clsUnitSectorConnection);
                    foreach ( var tempLoopVar_Connection in Map.Sectors[sectorNum.X, sectorNum.Y].Units )
                    {
                        connection = tempLoopVar_Connection;
                        unit = connection.Unit;
                        xYDbl.X = unit.Pos.Horizontal.X - mouseOverTerrain.Pos.Horizontal.X;
                        xYDbl.Y = unit.Pos.Horizontal.Y - mouseOverTerrain.Pos.Horizontal.Y;
                        footprint = unit.TypeBase.get_GetFootprintSelected(unit.Rotation);
                        if ( Math.Abs(xYDbl.X) <= Math.Max(footprint.X / 2.0D, 0.5D) * Constants.TerrainGridSpacing
                             && Math.Abs(xYDbl.Y) <= Math.Max(footprint.Y / 2.0D, 0.5D) * Constants.TerrainGridSpacing )
                        {
                            mouseOverTerrain.Units.Add(unit);
                        }
                    }

                    if ( MouseLeftDown != null )
                    {
                        if ( modTools.Tool == modTools.Tools.TerrainBrush )
                        {
                            ApplyTerrain();
                            if ( Program.frmMainInstance.cbxAutoTexSetHeight.Checked )
                            {
                                ApplyHeightSet(App.TerrainBrush, Program.frmMainInstance.HeightSetPalette[Program.frmMainInstance.tabHeightSetL.SelectedIndex]);
                            }
                        }
                        else if ( modTools.Tool == modTools.Tools.HeightSetBrush )
                        {
                            ApplyHeightSet(App.HeightBrush, Program.frmMainInstance.HeightSetPalette[Program.frmMainInstance.tabHeightSetL.SelectedIndex]);
                        }
                        else if ( modTools.Tool == modTools.Tools.TextureBrush )
                        {
                            ApplyTexture();
                        }
                        else if ( modTools.Tool == modTools.Tools.CliffTriangle )
                        {
                            ApplyCliffTriangle(false);
                        }
                        else if ( modTools.Tool == modTools.Tools.CliffBrush )
                        {
                            ApplyCliff();
                        }
                        else if ( modTools.Tool == modTools.Tools.CliffRemove )
                        {
                            ApplyCliffRemove();
                        }
                        else if ( modTools.Tool == modTools.Tools.RoadPlace )
                        {
                            ApplyRoad();
                        }
                        else if ( modTools.Tool == modTools.Tools.RoadRemove )
                        {
                            ApplyRoadRemove();
                        }
                    }
                    if ( MouseRightDown != null )
                    {
                        if ( modTools.Tool == modTools.Tools.HeightSetBrush )
                        {
                            if ( MouseLeftDown == null )
                            {
                                ApplyHeightSet(App.HeightBrush, Program.frmMainInstance.HeightSetPalette[Program.frmMainInstance.tabHeightSetR.SelectedIndex]);
                            }
                        }
                        else if ( modTools.Tool == modTools.Tools.CliffTriangle )
                        {
                            ApplyCliffTriangle(true);
                        }
                    }
                }
            }
            MapViewControl.Pos_Display_Update();
            MapViewControl.DrawViewLater();
        }

        public clsMouseOver.clsOverTerrain GetMouseOverTerrain()
        {
            if ( MouseOver == null )
            {
                return null;
            }
            return MouseOver.OverTerrain;
        }

        public clsMouseDown.clsOverTerrain GetMouseLeftDownOverTerrain()
        {
            if ( MouseLeftDown == null )
            {
                return null;
            }
            return MouseLeftDown.OverTerrain;
        }

        public clsMouseDown.clsOverTerrain GetMouseRightDownOverTerrain()
        {
            if ( MouseRightDown == null )
            {
                return null;
            }
            return MouseRightDown.OverTerrain;
        }

        public clsMouseDown.clsOverMinimap GetMouseLeftDownOverMinimap()
        {
            if ( MouseLeftDown == null )
            {
                return null;
            }
            return MouseLeftDown.OverMinimap;
        }

        public clsMouseDown.clsOverMinimap GetMouseRightDownOverMinimap()
        {
            if ( MouseRightDown == null )
            {
                return null;
            }
            return MouseRightDown.OverMinimap;
        }

        public bool IsViewPosOverMinimap(XYInt pos)
        {
            if ( pos.X >= 0 & pos.X < Map.Terrain.TileSize.X / TilesPerMinimapPixel
                 & pos.Y >= 0 & pos.Y < Map.Terrain.TileSize.Y / TilesPerMinimapPixel )
            {
                return true;
            }
            return false;
        }

        public void ApplyTerrain()
        {
            var mouseOverTerrain = GetMouseOverTerrain();

            if ( mouseOverTerrain == null )
            {
                return;
            }

            var applyVertexTerrain = new clsApplyVertexTerrain();
            applyVertexTerrain.Map = Map;
            applyVertexTerrain.VertexTerrain = App.SelectedTerrain;
            App.TerrainBrush.PerformActionMapVertices(applyVertexTerrain, mouseOverTerrain.Vertex);

            Map.Update();

            MapViewControl.DrawViewLater();
        }

        public void ApplyRoad()
        {
            var mouseOverTerrain = GetMouseOverTerrain();

            if ( mouseOverTerrain == null )
            {
                return;
            }

            var side_Num = mouseOverTerrain.Side_Num;
            var tileNum = new XYInt();

            if ( mouseOverTerrain.Side_IsV )
            {
                if ( Map.Terrain.SideV[side_Num.X, side_Num.Y].Road != App.SelectedRoad )
                {
                    Map.Terrain.SideV[side_Num.X, side_Num.Y].Road = App.SelectedRoad;

                    if ( side_Num.X > 0 )
                    {
                        tileNum.X = side_Num.X - 1;
                        tileNum.Y = side_Num.Y;
                        Map.AutoTextureChanges.TileChanged(tileNum);
                        Map.SectorGraphicsChanges.TileChanged(tileNum);
                        Map.SectorTerrainUndoChanges.TileChanged(tileNum);
                    }
                    if ( side_Num.X < Map.Terrain.TileSize.X )
                    {
                        tileNum = side_Num;
                        Map.AutoTextureChanges.TileChanged(tileNum);
                        Map.SectorGraphicsChanges.TileChanged(tileNum);
                        Map.SectorTerrainUndoChanges.TileChanged(tileNum);
                    }

                    Map.Update();

                    Map.UndoStepCreate("Road Side");

                    MapViewControl.DrawViewLater();
                }
            }
            else
            {
                if ( Map.Terrain.SideH[side_Num.X, side_Num.Y].Road != App.SelectedRoad )
                {
                    Map.Terrain.SideH[side_Num.X, side_Num.Y].Road = App.SelectedRoad;

                    if ( side_Num.Y > 0 )
                    {
                        tileNum.X = side_Num.X;
                        tileNum.Y = side_Num.Y - 1;
                        Map.AutoTextureChanges.TileChanged(tileNum);
                        Map.SectorGraphicsChanges.TileChanged(tileNum);
                        Map.SectorTerrainUndoChanges.TileChanged(tileNum);
                    }
                    if ( side_Num.Y < Map.Terrain.TileSize.X )
                    {
                        tileNum = side_Num;
                        Map.AutoTextureChanges.TileChanged(tileNum);
                        Map.SectorGraphicsChanges.TileChanged(tileNum);
                        Map.SectorTerrainUndoChanges.TileChanged(tileNum);
                    }

                    Map.Update();

                    Map.UndoStepCreate("Road Side");

                    MapViewControl.DrawViewLater();
                }
            }
        }

        public void ApplyRoadLineSelection()
        {
            var mouseOverTerrian = GetMouseOverTerrain();

            if ( mouseOverTerrian == null )
            {
                return;
            }

            var num = 0;
            var a = 0;
            var b = 0;
            var Tile = mouseOverTerrian.Tile.Normal;
            var SideNum = new XYInt();

            if ( Map.Selected_Tile_A != null )
            {
                if ( Tile.X == Map.Selected_Tile_A.X )
                {
                    if ( Tile.Y <= Map.Selected_Tile_A.Y )
                    {
                        a = Tile.Y;
                        b = Map.Selected_Tile_A.Y;
                    }
                    else
                    {
                        a = Map.Selected_Tile_A.Y;
                        b = Tile.Y;
                    }
                    for ( num = a + 1; num <= b; num++ )
                    {
                        Map.Terrain.SideH[Map.Selected_Tile_A.X, num].Road = App.SelectedRoad;
                        SideNum.X = Map.Selected_Tile_A.X;
                        SideNum.Y = num;
                        Map.AutoTextureChanges.SideHChanged(SideNum);
                        Map.SectorGraphicsChanges.SideHChanged(SideNum);
                        Map.SectorTerrainUndoChanges.SideHChanged(SideNum);
                    }

                    Map.Update();

                    Map.UndoStepCreate("Road Line");

                    Map.Selected_Tile_A = null;
                    MapViewControl.DrawViewLater();
                }
                else if ( Tile.Y == Map.Selected_Tile_A.Y )
                {
                    if ( Tile.X <= Map.Selected_Tile_A.X )
                    {
                        a = Tile.X;
                        b = Map.Selected_Tile_A.X;
                    }
                    else
                    {
                        a = Map.Selected_Tile_A.X;
                        b = Tile.X;
                    }
                    for ( num = a + 1; num <= b; num++ )
                    {
                        Map.Terrain.SideV[num, Map.Selected_Tile_A.Y].Road = App.SelectedRoad;
                        SideNum.X = num;
                        SideNum.Y = Map.Selected_Tile_A.Y;
                        Map.AutoTextureChanges.SideVChanged(SideNum);
                        Map.SectorGraphicsChanges.SideVChanged(SideNum);
                        Map.SectorTerrainUndoChanges.SideVChanged(SideNum);
                    }

                    Map.Update();

                    Map.UndoStepCreate("Road Line");

                    Map.Selected_Tile_A = null;
                    MapViewControl.DrawViewLater();
                }
            }
            else
            {
                Map.Selected_Tile_A = Tile;
            }
        }

        public void ApplyTerrainFill(enumFillCliffAction CliffAction, bool Inside)
        {
            var mouseOverTerrain = GetMouseOverTerrain();

            if ( mouseOverTerrain == null )
            {
                return;
            }

            var fillType = default(Terrain);
            var replaceType = default(Terrain);
            var startVertex = mouseOverTerrain.Vertex.Normal;

            fillType = App.SelectedTerrain;
            replaceType = Map.Terrain.Vertices[startVertex.X, startVertex.Y].Terrain;
            if ( fillType == replaceType )
            {
                return; //otherwise will cause endless loop
            }

            var A = 0;
            var sourceOfFill = new XYInt[524289];
            var sourceOfFillCount = 0;
            var sourceOfFillNum = 0;
            var moveCount = 0;
            var remainingCount = 0;
            var moveOffset = 0;
            var currentSource = new XYInt();
            var nextSource = new XYInt();
            var stopForCliff = default(bool);
            var stopForEdge = default(bool);

            sourceOfFill[0] = startVertex;
            sourceOfFillCount = 1;
            sourceOfFillNum = 0;
            while ( sourceOfFillNum < sourceOfFillCount )
            {
                currentSource = sourceOfFill[sourceOfFillNum];

                if ( CliffAction == enumFillCliffAction.StopBefore )
                {
                    stopForCliff = Map.VertexIsCliffEdge(currentSource);
                }
                else
                {
                    stopForCliff = false;
                }
                stopForEdge = false;
                if ( Inside )
                {
                    if ( currentSource.X > 0 )
                    {
                        if ( currentSource.Y > 0 )
                        {
                            if ( Map.Terrain.Vertices[currentSource.X - 1, currentSource.Y - 1].Terrain != replaceType
                                 && Map.Terrain.Vertices[currentSource.X - 1, currentSource.Y - 1].Terrain != fillType )
                            {
                                stopForEdge = true;
                            }
                        }
                        if ( Map.Terrain.Vertices[currentSource.X - 1, currentSource.Y].Terrain != replaceType
                             && Map.Terrain.Vertices[currentSource.X - 1, currentSource.Y].Terrain != fillType )
                        {
                            stopForEdge = true;
                        }
                        if ( currentSource.Y < Map.Terrain.TileSize.Y )
                        {
                            if ( Map.Terrain.Vertices[currentSource.X - 1, currentSource.Y + 1].Terrain != replaceType
                                 && Map.Terrain.Vertices[currentSource.X - 1, currentSource.Y + 1].Terrain != fillType )
                            {
                                stopForEdge = true;
                            }
                        }
                    }
                    if ( currentSource.Y > 0 )
                    {
                        if ( Map.Terrain.Vertices[currentSource.X, currentSource.Y - 1].Terrain != replaceType
                             && Map.Terrain.Vertices[currentSource.X, currentSource.Y - 1].Terrain != fillType )
                        {
                            stopForEdge = true;
                        }
                    }
                    if ( currentSource.X < Map.Terrain.TileSize.X )
                    {
                        if ( currentSource.Y > 0 )
                        {
                            if ( Map.Terrain.Vertices[currentSource.X + 1, currentSource.Y - 1].Terrain != replaceType
                                 && Map.Terrain.Vertices[currentSource.X + 1, currentSource.Y - 1].Terrain != fillType )
                            {
                                stopForEdge = true;
                            }
                        }
                        if ( Map.Terrain.Vertices[currentSource.X + 1, currentSource.Y].Terrain != replaceType
                             && Map.Terrain.Vertices[currentSource.X + 1, currentSource.Y].Terrain != fillType )
                        {
                            stopForEdge = true;
                        }
                        if ( currentSource.Y < Map.Terrain.TileSize.Y )
                        {
                            if ( Map.Terrain.Vertices[currentSource.X + 1, currentSource.Y + 1].Terrain != replaceType
                                 && Map.Terrain.Vertices[currentSource.X + 1, currentSource.Y + 1].Terrain != fillType )
                            {
                                stopForEdge = true;
                            }
                        }
                    }
                    if ( currentSource.Y < Map.Terrain.TileSize.Y )
                    {
                        if ( Map.Terrain.Vertices[currentSource.X, currentSource.Y + 1].Terrain != replaceType
                             && Map.Terrain.Vertices[currentSource.X, currentSource.Y + 1].Terrain != fillType )
                        {
                            stopForEdge = true;
                        }
                    }
                }

                if ( !(stopForCliff || stopForEdge) )
                {
                    if ( Map.Terrain.Vertices[currentSource.X, currentSource.Y].Terrain == replaceType )
                    {
                        Map.Terrain.Vertices[currentSource.X, currentSource.Y].Terrain = fillType;
                        Map.SectorGraphicsChanges.VertexChanged(currentSource);
                        Map.SectorTerrainUndoChanges.VertexChanged(currentSource);
                        Map.AutoTextureChanges.VertexChanged(currentSource);

                        nextSource.X = currentSource.X + 1;
                        nextSource.Y = currentSource.Y;
                        if ( nextSource.X >= 0 & nextSource.X <= Map.Terrain.TileSize.X
                             & nextSource.Y >= 0 & nextSource.Y <= Map.Terrain.TileSize.Y )
                        {
                            if ( CliffAction == enumFillCliffAction.StopAfter )
                            {
                                stopForCliff = Map.SideHIsCliffOnBothSides(new XYInt(currentSource.X, currentSource.Y));
                            }
                            else
                            {
                                stopForCliff = false;
                            }
                            if ( !stopForCliff )
                            {
                                if ( Map.Terrain.Vertices[nextSource.X, nextSource.Y].Terrain == replaceType )
                                {
                                    if ( sourceOfFill.GetUpperBound(0) < sourceOfFillCount )
                                    {
                                        Array.Resize(ref sourceOfFill, sourceOfFillCount * 2 + 1 + 1);
                                    }
                                    sourceOfFill[sourceOfFillCount] = nextSource;
                                    sourceOfFillCount++;
                                }
                            }
                        }

                        nextSource.X = currentSource.X - 1;
                        nextSource.Y = currentSource.Y;
                        if ( nextSource.X >= 0 & nextSource.X <= Map.Terrain.TileSize.X
                             & nextSource.Y >= 0 & nextSource.Y <= Map.Terrain.TileSize.Y )
                        {
                            if ( CliffAction == enumFillCliffAction.StopAfter )
                            {
                                stopForCliff = Map.SideHIsCliffOnBothSides(new XYInt(currentSource.X - 1, currentSource.Y));
                            }
                            else
                            {
                                stopForCliff = false;
                            }
                            if ( !stopForCliff )
                            {
                                if ( Map.Terrain.Vertices[nextSource.X, nextSource.Y].Terrain == replaceType )
                                {
                                    if ( sourceOfFill.GetUpperBound(0) < sourceOfFillCount )
                                    {
                                        Array.Resize(ref sourceOfFill, sourceOfFillCount * 2 + 1 + 1);
                                    }
                                    sourceOfFill[sourceOfFillCount] = nextSource;
                                    sourceOfFillCount++;
                                }
                            }
                        }

                        nextSource.X = currentSource.X;
                        nextSource.Y = currentSource.Y + 1;
                        if ( nextSource.X >= 0 & nextSource.X <= Map.Terrain.TileSize.X
                             & nextSource.Y >= 0 & nextSource.Y <= Map.Terrain.TileSize.Y )
                        {
                            if ( CliffAction == enumFillCliffAction.StopAfter )
                            {
                                stopForCliff = Map.SideVIsCliffOnBothSides(new XYInt(currentSource.X, currentSource.Y));
                            }
                            else
                            {
                                stopForCliff = false;
                            }
                            if ( !stopForCliff )
                            {
                                if ( Map.Terrain.Vertices[nextSource.X, nextSource.Y].Terrain == replaceType )
                                {
                                    if ( sourceOfFill.GetUpperBound(0) < sourceOfFillCount )
                                    {
                                        Array.Resize(ref sourceOfFill, sourceOfFillCount * 2 + 1 + 1);
                                    }
                                    sourceOfFill[sourceOfFillCount] = nextSource;
                                    sourceOfFillCount++;
                                }
                            }
                        }

                        nextSource.X = currentSource.X;
                        nextSource.Y = currentSource.Y - 1;
                        if ( nextSource.X >= 0 & nextSource.X <= Map.Terrain.TileSize.X
                             & nextSource.Y >= 0 & nextSource.Y <= Map.Terrain.TileSize.Y )
                        {
                            if ( CliffAction == enumFillCliffAction.StopAfter )
                            {
                                stopForCliff = Map.SideVIsCliffOnBothSides(new XYInt(currentSource.X, currentSource.Y - 1));
                            }
                            else
                            {
                                stopForCliff = false;
                            }
                            if ( !stopForCliff )
                            {
                                if ( Map.Terrain.Vertices[nextSource.X, nextSource.Y].Terrain == replaceType )
                                {
                                    if ( sourceOfFill.GetUpperBound(0) < sourceOfFillCount )
                                    {
                                        Array.Resize(ref sourceOfFill, sourceOfFillCount * 2 + 1 + 1);
                                    }
                                    sourceOfFill[sourceOfFillCount] = nextSource;
                                    sourceOfFillCount++;
                                }
                            }
                        }
                    }
                }

                sourceOfFillNum++;

                if ( sourceOfFillNum >= 131072 )
                {
                    remainingCount = sourceOfFillCount - sourceOfFillNum;
                    moveCount = Math.Min(sourceOfFillNum, remainingCount);
                    moveOffset = sourceOfFillCount - moveCount;
                    for ( A = 0; A <= moveCount - 1; A++ )
                    {
                        sourceOfFill[A] = sourceOfFill[moveOffset + A];
                    }
                    sourceOfFillCount -= sourceOfFillNum;
                    sourceOfFillNum = 0;
                    if ( sourceOfFillCount * 3 < sourceOfFill.GetUpperBound(0) + 1 )
                    {
                        Array.Resize(ref sourceOfFill, sourceOfFillCount * 2 + 1 + 1);
                    }
                }
            }

            Map.Update();

            Map.UndoStepCreate("Ground Fill");

            MapViewControl.DrawViewLater();
        }

        public void ApplyTexture()
        {
            var mouseOverTerrain = GetMouseOverTerrain();

            if ( mouseOverTerrain == null )
            {
                return;
            }

            var applyTexture = new clsApplyTexture();
            applyTexture.Map = Map;
            applyTexture.TextureNum = App.SelectedTextureNum;
            applyTexture.SetTexture = Program.frmMainInstance.chkSetTexture.Checked;
            applyTexture.Orientation = App.TextureOrientation;
            applyTexture.RandomOrientation = Program.frmMainInstance.chkTextureOrientationRandomize.Checked;
            applyTexture.SetOrientation = Program.frmMainInstance.chkSetTextureOrientation.Checked;
            applyTexture.TerrainAction = Program.frmMainInstance.TextureTerrainAction;
            App.TextureBrush.PerformActionMapTiles(applyTexture, mouseOverTerrain.Tile);

            Map.Update();

            MapViewControl.DrawViewLater();
        }

        private void ApplyCliffTriangle(bool remove)
        {
            var mouseOverTerrain = GetMouseOverTerrain();

            if ( mouseOverTerrain == null )
            {
                return;
            }

            if ( remove )
            {
                var ApplyCliffTriangleRemove = new clsApplyCliffTriangleRemove();
                ApplyCliffTriangleRemove.Map = Map;
                ApplyCliffTriangleRemove.PosNum = mouseOverTerrain.Tile.Normal;
                ApplyCliffTriangleRemove.Triangle = mouseOverTerrain.Triangle;
                ApplyCliffTriangleRemove.ActionPerform();
            }
            else
            {
                var ApplyCliffTriangle = new clsApplyCliffTriangle();
                ApplyCliffTriangle.Map = Map;
                ApplyCliffTriangle.PosNum = mouseOverTerrain.Tile.Normal;
                ApplyCliffTriangle.Triangle = mouseOverTerrain.Triangle;
                ApplyCliffTriangle.ActionPerform();
            }

            Map.Update();

            MapViewControl.DrawViewLater();
        }

        public void ApplyCliff()
        {
            var mouseOverTerrain = GetMouseOverTerrain();

            if ( mouseOverTerrain == null )
            {
                return;
            }

            var applyCliff = new clsApplyCliff();
            applyCliff.Map = Map;
            double angle = 0;
            if ( !IOUtil.InvariantParse(Program.frmMainInstance.txtAutoCliffSlope.Text, ref angle) )
            {
                return;
            }
            applyCliff.Angle = MathUtil.ClampDbl(angle * MathUtil.RadOf1Deg, 0.0D, MathUtil.RadOf90Deg);
            applyCliff.SetTris = Program.frmMainInstance.cbxCliffTris.Checked;
            App.CliffBrush.PerformActionMapTiles(applyCliff, mouseOverTerrain.Tile);

            Map.Update();

            MapViewControl.DrawViewLater();
        }

        public void ApplyCliffRemove()
        {
            var mouseOverTerrain = GetMouseOverTerrain();

            if ( mouseOverTerrain == null )
            {
                return;
            }

            var applyCliffRemove = new clsApplyCliffRemove();
            applyCliffRemove.Map = Map;
            App.CliffBrush.PerformActionMapTiles(applyCliffRemove, mouseOverTerrain.Tile);

            Map.Update();

            MapViewControl.DrawViewLater();
        }

        public void ApplyRoadRemove()
        {
            var mouseOverTerrain = GetMouseOverTerrain();

            if ( mouseOverTerrain == null )
            {
                return;
            }

            var applyRoadRemove = new clsApplyRoadRemove();
            applyRoadRemove.Map = Map;
            App.CliffBrush.PerformActionMapTiles(applyRoadRemove, mouseOverTerrain.Tile);

            Map.Update();

            MapViewControl.DrawViewLater();
        }

        public void ApplyTextureClockwise()
        {
            var mouseOverTerrain = GetMouseOverTerrain();

            if ( mouseOverTerrain == null )
            {
                return;
            }

            var Tile = mouseOverTerrain.Tile.Normal;

            Map.Terrain.Tiles[Tile.X, Tile.Y].Texture.Orientation.RotateClockwise();
            Map.TileTextureChangeTerrainAction(Tile, Program.frmMainInstance.TextureTerrainAction);

            Map.SectorGraphicsChanges.TileChanged(Tile);
            Map.SectorTerrainUndoChanges.TileChanged(Tile);

            Map.Update();

            Map.UndoStepCreate("Texture Rotate");

            MapViewControl.DrawViewLater();
        }

        public void ApplyTextureCounterClockwise()
        {
            var mouseOverTerrain = GetMouseOverTerrain();

            if ( mouseOverTerrain == null )
            {
                return;
            }

            var tile = mouseOverTerrain.Tile.Normal;

            Map.Terrain.Tiles[tile.X, tile.Y].Texture.Orientation.RotateAntiClockwise();
            Map.TileTextureChangeTerrainAction(tile, Program.frmMainInstance.TextureTerrainAction);

            Map.SectorGraphicsChanges.TileChanged(tile);
            Map.SectorTerrainUndoChanges.TileChanged(tile);

            Map.Update();

            Map.UndoStepCreate("Texture Rotate");

            MapViewControl.DrawViewLater();
        }

        public void ApplyTextureFlipX()
        {
            var mouseOverTerrain = GetMouseOverTerrain();

            if ( mouseOverTerrain == null )
            {
                return;
            }

            var tile = mouseOverTerrain.Tile.Normal;

            Map.Terrain.Tiles[tile.X, tile.Y].Texture.Orientation.ResultXFlip = !Map.Terrain.Tiles[tile.X, tile.Y].Texture.Orientation.ResultXFlip;
            Map.TileTextureChangeTerrainAction(tile, Program.frmMainInstance.TextureTerrainAction);

            Map.SectorGraphicsChanges.TileChanged(tile);
            Map.SectorTerrainUndoChanges.TileChanged(tile);

            Map.Update();

            Map.UndoStepCreate("Texture Rotate");

            MapViewControl.DrawViewLater();
        }

        public void ApplyTriFlip()
        {
            var mouseOverTerrain = GetMouseOverTerrain();

            if ( mouseOverTerrain == null )
            {
                return;
            }

            var tile = mouseOverTerrain.Tile.Normal;

            Map.Terrain.Tiles[tile.X, tile.Y].Tri = !Map.Terrain.Tiles[tile.X, tile.Y].Tri;

            Map.SectorGraphicsChanges.TileChanged(tile);
            Map.SectorTerrainUndoChanges.TileChanged(tile);

            Map.Update();

            Map.UndoStepCreate("Triangle Flip");

            MapViewControl.DrawViewLater();
        }

        public void ApplyHeightSmoothing(double ratio)
        {
            var mouseOverTerrain = GetMouseOverTerrain();

            if ( mouseOverTerrain == null )
            {
                return;
            }

            var applyHeightSmoothing = new clsApplyHeightSmoothing();
            applyHeightSmoothing.Map = Map;
            applyHeightSmoothing.Ratio = ratio;
            var radius = (int)(Math.Ceiling(App.HeightBrush.Radius));
            var posNum = App.HeightBrush.GetPosNum(mouseOverTerrain.Vertex);
            applyHeightSmoothing.Offset.X = MathUtil.ClampInt(posNum.X - radius, 0, Map.Terrain.TileSize.X);
            applyHeightSmoothing.Offset.Y = MathUtil.ClampInt(posNum.Y - radius, 0, Map.Terrain.TileSize.Y);
            var posEnd = new XYInt();
            posEnd.X = MathUtil.ClampInt(posNum.X + radius, 0, Map.Terrain.TileSize.X);
            posEnd.Y = MathUtil.ClampInt(posNum.Y + radius, 0, Map.Terrain.TileSize.Y);
            applyHeightSmoothing.AreaTileSize.X = posEnd.X - applyHeightSmoothing.Offset.X;
            applyHeightSmoothing.AreaTileSize.Y = posEnd.Y - applyHeightSmoothing.Offset.Y;
            applyHeightSmoothing.Start();
            App.HeightBrush.PerformActionMapVertices(applyHeightSmoothing, mouseOverTerrain.Vertex);
            applyHeightSmoothing.Finish();

            Map.Update();

            MapViewControl.DrawViewLater();
        }

        public void ApplyHeightChange(double rate)
        {
            var mouseOverTerrain = GetMouseOverTerrain();

            if ( mouseOverTerrain == null )
            {
                return;
            }

            var applyHeightChange = new clsApplyHeightChange();
            applyHeightChange.Map = Map;
            applyHeightChange.Rate = rate;
            applyHeightChange.UseEffect = Program.frmMainInstance.cbxHeightChangeFade.Checked;
            App.HeightBrush.PerformActionMapVertices(applyHeightChange, mouseOverTerrain.Vertex);

            Map.Update();

            MapViewControl.DrawViewLater();
        }

        public void ApplyHeightSet(clsBrush brush, byte height)
        {
            var mouseOverTerrain = GetMouseOverTerrain();

            if ( mouseOverTerrain == null )
            {
                return;
            }

            var applyHeightSet = new clsApplyHeightSet();
            applyHeightSet.Map = Map;
            applyHeightSet.Height = height;
            brush.PerformActionMapVertices(applyHeightSet, mouseOverTerrain.Vertex);

            Map.Update();

            MapViewControl.DrawViewLater();
        }

        public void ApplyGateway()
        {
            var mouseOverTerrain = GetMouseOverTerrain();

            if ( mouseOverTerrain == null )
            {
                return;
            }

            var tile = mouseOverTerrain.Tile.Normal;

            if ( KeyboardManager.KeyboardProfile.Active(KeyboardManager.Gateway_Delete) )
            {
                var a = 0;
                var low = new XYInt();
                var high = new XYInt();
                a = 0;
                while ( a < Map.Gateways.Count )
                {
                    MathUtil.ReorderXY(Map.Gateways[a].PosA, Map.Gateways[a].PosB, ref low, ref high);
                    if ( low.X <= tile.X
                         & high.X >= tile.X
                         & low.Y <= tile.Y
                         & high.Y >= tile.Y )
                    {
                        Map.GatewayRemoveStoreChange(a);
                        Map.UndoStepCreate("Gateway Delete");
                        Map.MinimapMakeLater();
                        MapViewControl.DrawViewLater();
                        break;
                    }
                    a++;
                }
            }
            else
            {
                if ( Map.Selected_Tile_A == null )
                {
                    Map.Selected_Tile_A = tile;
                    MapViewControl.DrawViewLater();
                }
                else if ( tile.X == Map.Selected_Tile_A.X | tile.Y == Map.Selected_Tile_A.Y )
                {
                    if ( Map.GatewayCreateStoreChange(Map.Selected_Tile_A, tile) != null )
                    {
                        Map.UndoStepCreate("Gateway Place");
                        Map.Selected_Tile_A = null;
                        Map.Selected_Tile_B = null;
                        Map.MinimapMakeLater();
                        MapViewControl.DrawViewLater();
                    }
                }
            }
        }

        public void MouseDown(MouseEventArgs e)
        {
            var screenPos = new XYInt();

            Map.SuppressMinimap = true;

            screenPos.X = e.X;
            screenPos.Y = e.Y;
            if ( e.Button == MouseButtons.Left )
            {
                MouseLeftDown = new clsMouseDown();
                if ( IsViewPosOverMinimap(screenPos) )
                {
                    MouseLeftDown.OverMinimap = new clsMouseDown.clsOverMinimap();
                    MouseLeftDown.OverMinimap.DownPos = screenPos;
                    var Pos = new XYInt((int)((screenPos.X * TilesPerMinimapPixel)),
                        (int)(screenPos.Y * TilesPerMinimapPixel));
                    Map.TileNumClampToMap(Pos);
                    LookAtTile(Pos);
                }
                else
                {
                    var MouseOverTerrain = GetMouseOverTerrain();
                    if ( MouseOverTerrain != null )
                    {
                        MouseLeftDown.OverTerrain = new clsMouseDown.clsOverTerrain();
                        MouseLeftDown.OverTerrain.DownPos = MouseOverTerrain.Pos;
                        if ( modTools.Tool == modTools.Tools.ObjectSelect )
                        {
                            if ( KeyboardManager.KeyboardProfile.Active(KeyboardManager.Picker) )
                            {
                                if ( MouseOverTerrain.Units.Count > 0 )
                                {
                                    if ( MouseOverTerrain.Units.Count == 1 )
                                    {
                                        Program.frmMainInstance.ObjectPicker(MouseOverTerrain.Units[0].TypeBase);
                                    }
                                    else
                                    {
                                        MapViewControl.ListSelectBegin(true);
                                    }
                                }
                            }
                            else if ( KeyboardManager.KeyboardProfile.Active(KeyboardManager.ScriptPosition) )
                            {
                                var NewPosition = new clsScriptPosition(Map);
                                NewPosition.PosX = MouseLeftDown.OverTerrain.DownPos.Horizontal.X;
                                NewPosition.PosY = MouseLeftDown.OverTerrain.DownPos.Horizontal.Y;
                                Program.frmMainInstance.ScriptMarkerLists_Update();
                            }
                            else
                            {
                                if ( !KeyboardManager.KeyboardProfile.Active(KeyboardManager.UnitMultiselect) )
                                {
                                    Map.SelectedUnits.Clear();
                                }
                                Program.frmMainInstance.SelectedObject_Changed();
                                Map.Unit_Selected_Area_VertexA = MouseOverTerrain.Vertex.Normal;
                                MapViewControl.DrawViewLater();
                            }
                        }
                        else if ( modTools.Tool == modTools.Tools.TerrainBrush )
                        {
                            if ( Map.Tileset != null )
                            {
                                if ( KeyboardManager.KeyboardProfile.Active(KeyboardManager.Picker) )
                                {
                                    Program.frmMainInstance.TerrainPicker();
                                }
                                else
                                {
                                    ApplyTerrain();
                                    if ( Program.frmMainInstance.cbxAutoTexSetHeight.Checked )
                                    {
                                        ApplyHeightSet(App.TerrainBrush,
                                            Program.frmMainInstance.HeightSetPalette[Program.frmMainInstance.tabHeightSetL.SelectedIndex]);
                                    }
                                }
                            }
                        }
                        else if ( modTools.Tool == modTools.Tools.HeightSetBrush )
                        {
                            if ( KeyboardManager.KeyboardProfile.Active(KeyboardManager.Picker) )
                            {
                                Program.frmMainInstance.HeightPickerL();
                            }
                            else
                            {
                                ApplyHeightSet(App.HeightBrush, Program.frmMainInstance.HeightSetPalette[Program.frmMainInstance.tabHeightSetL.SelectedIndex]);
                            }
                        }
                        else if ( modTools.Tool == modTools.Tools.TextureBrush )
                        {
                            if ( Map.Tileset != null )
                            {
                                if ( KeyboardManager.KeyboardProfile.Active(KeyboardManager.Picker) )
                                {
                                    Program.frmMainInstance.TexturePicker();
                                }
                                else
                                {
                                    ApplyTexture();
                                }
                            }
                        }
                        else if ( modTools.Tool == modTools.Tools.CliffTriangle )
                        {
                            ApplyCliffTriangle(false);
                        }
                        else if ( modTools.Tool == modTools.Tools.CliffBrush )
                        {
                            ApplyCliff();
                        }
                        else if ( modTools.Tool == modTools.Tools.CliffRemove )
                        {
                            ApplyCliffRemove();
                        }
                        else if ( modTools.Tool == modTools.Tools.TerrainFill )
                        {
                            if ( Map.Tileset != null )
                            {
                                if ( KeyboardManager.KeyboardProfile.Active(KeyboardManager.Picker) )
                                {
                                    Program.frmMainInstance.TerrainPicker();
                                }
                                else
                                {
                                    ApplyTerrainFill(Program.frmMainInstance.FillCliffAction, Program.frmMainInstance.cbxFillInside.Checked);
                                    MapViewControl.DrawViewLater();
                                }
                            }
                        }
                        else if ( modTools.Tool == modTools.Tools.RoadPlace )
                        {
                            if ( Map.Tileset != null )
                            {
                                ApplyRoad();
                            }
                        }
                        else if ( modTools.Tool == modTools.Tools.RoadLines )
                        {
                            if ( Map.Tileset != null )
                            {
                                ApplyRoadLineSelection();
                            }
                        }
                        else if ( modTools.Tool == modTools.Tools.RoadRemove )
                        {
                            ApplyRoadRemove();
                        }
                        else if ( modTools.Tool == modTools.Tools.ObjectPlace )
                        {
                            if ( Program.frmMainInstance.SingleSelectedObjectTypeBase != null && Map.SelectedUnitGroup != null )
                            {
                                var objectCreator = new clsUnitCreate();
                                Map.SetObjectCreatorDefaults(objectCreator);
                                objectCreator.Horizontal = MouseOverTerrain.Pos.Horizontal;
                                objectCreator.Perform();
                                Map.UndoStepCreate("Place Object");
                                Map.Update();
                                Map.MinimapMakeLater();
                                MapViewControl.DrawViewLater();
                            }
                        }
                        else if ( modTools.Tool == modTools.Tools.ObjectLines )
                        {
                            ApplyObjectLine();
                        }
                        else if ( modTools.Tool == modTools.Tools.TerrainSelect )
                        {
                            if ( Map.Selected_Area_VertexA == null )
                            {
                                Map.Selected_Area_VertexA = MouseOverTerrain.Vertex.Normal;
                                MapViewControl.DrawViewLater();
                            }
                            else if ( Map.Selected_Area_VertexB == null )
                            {
                                Map.Selected_Area_VertexB = MouseOverTerrain.Vertex.Normal;
                                MapViewControl.DrawViewLater();
                            }
                            else
                            {
                                Map.Selected_Area_VertexA = null;
                                Map.Selected_Area_VertexB = null;
                                MapViewControl.DrawViewLater();
                            }
                        }
                        else if ( modTools.Tool == modTools.Tools.Gateways )
                        {
                            ApplyGateway();
                        }
                    }
                    else if ( modTools.Tool == modTools.Tools.ObjectSelect )
                    {
                        Map.SelectedUnits.Clear();
                        Program.frmMainInstance.SelectedObject_Changed();
                    }
                }
            }
            else if ( e.Button == MouseButtons.Right )
            {
                MouseRightDown = new clsMouseDown();
                if ( IsViewPosOverMinimap(screenPos) )
                {
                    MouseRightDown.OverMinimap = new clsMouseDown.clsOverMinimap();
                    MouseRightDown.OverMinimap.DownPos = screenPos;
                }
                else
                {
                    var MouseOverTerrain = GetMouseOverTerrain();
                    if ( MouseOverTerrain != null )
                    {
                        MouseRightDown.OverTerrain = new clsMouseDown.clsOverTerrain();
                        MouseRightDown.OverTerrain.DownPos = MouseOverTerrain.Pos;
                    }
                }
                if ( modTools.Tool == modTools.Tools.RoadLines || modTools.Tool == modTools.Tools.ObjectLines )
                {
                    Map.Selected_Tile_A = null;
                    MapViewControl.DrawViewLater();
                }
                else if ( modTools.Tool == modTools.Tools.TerrainSelect )
                {
                    Map.Selected_Area_VertexA = null;
                    Map.Selected_Area_VertexB = null;
                    MapViewControl.DrawViewLater();
                }
                else if ( modTools.Tool == modTools.Tools.CliffTriangle )
                {
                    ApplyCliffTriangle(true);
                }
                else if ( modTools.Tool == modTools.Tools.Gateways )
                {
                    Map.Selected_Tile_A = null;
                    Map.Selected_Tile_B = null;
                    MapViewControl.DrawViewLater();
                }
                else if ( modTools.Tool == modTools.Tools.HeightSetBrush )
                {
                    if ( KeyboardManager.KeyboardProfile.Active(KeyboardManager.Picker) )
                    {
                        Program.frmMainInstance.HeightPickerR();
                    }
                    else
                    {
                        ApplyHeightSet(App.HeightBrush, Program.frmMainInstance.HeightSetPalette[Program.frmMainInstance.tabHeightSetR.SelectedIndex]);
                    }
                }
            }
        }

        public void TimedActions(double Zoom, double Move, double Pan, double Roll, double OrbitRate)
        {
            var xyzDbl = new XYZDouble();
            var panRate = Pan * FieldOfViewY;
            var anglePY = default(Angles.AnglePY);
            var matrixA = new Matrix3DMath.Matrix3D();
            var matrixB = new Matrix3DMath.Matrix3D();
            var viewAngleChange = default(XYZDouble);
            var viewPosChangeXyz = new XYZInt(0, 0, 0);
            var angleChanged = default(bool);

            Move *= FOVMultiplier * (MapViewControl.GLSize.X + MapViewControl.GLSize.Y) * Math.Max(Math.Abs(ViewPos.Y), 512.0D);

            if ( KeyboardManager.KeyboardProfile.Active(KeyboardManager.ViewZoomIn) )
            {
                FovScale_2EChange(Convert.ToDouble(- Zoom));
            }
            if ( KeyboardManager.KeyboardProfile.Active(KeyboardManager.ViewZoomOut) )
            {
                FovScale_2EChange(Zoom);
            }

            if ( App.ViewMoveType == ViewMoveType.Free )
            {
                viewPosChangeXyz.X = 0;
                viewPosChangeXyz.Y = 0;
                viewPosChangeXyz.Z = 0;
                if ( KeyboardManager.KeyboardProfile.Active(KeyboardManager.ViewMoveForward) )
                {
                    Matrix3DMath.VectorForwardsRotationByMatrix(ViewAngleMatrix, Move, ref xyzDbl);
                    viewPosChangeXyz.AddDbl(xyzDbl);
                }
                if ( KeyboardManager.KeyboardProfile.Active(KeyboardManager.ViewMoveBackward) )
                {
                    Matrix3DMath.VectorBackwardsRotationByMatrix(ViewAngleMatrix, Move, ref xyzDbl);
                    viewPosChangeXyz.AddDbl(xyzDbl);
                }
                if ( KeyboardManager.KeyboardProfile.Active(KeyboardManager.ViewMoveLeft) )
                {
                    Matrix3DMath.VectorLeftRotationByMatrix(ViewAngleMatrix, Move, ref xyzDbl);
                    viewPosChangeXyz.AddDbl(xyzDbl);
                }
                if ( KeyboardManager.KeyboardProfile.Active(KeyboardManager.ViewMoveRight) )
                {
                    Matrix3DMath.VectorRightRotationByMatrix(ViewAngleMatrix, Move, ref xyzDbl);
                    viewPosChangeXyz.AddDbl(xyzDbl);
                }
                if ( KeyboardManager.KeyboardProfile.Active(KeyboardManager.ViewMoveUp) )
                {
                    Matrix3DMath.VectorUpRotationByMatrix(ViewAngleMatrix, Move, ref xyzDbl);
                    viewPosChangeXyz.AddDbl(xyzDbl);
                }
                if ( KeyboardManager.KeyboardProfile.Active(KeyboardManager.ViewMoveDown) )
                {
                    Matrix3DMath.VectorDownRotationByMatrix(ViewAngleMatrix, Move, ref xyzDbl);
                    viewPosChangeXyz.AddDbl(xyzDbl);
                }

                viewAngleChange.X = 0.0D;
                viewAngleChange.Y = 0.0D;
                viewAngleChange.Z = 0.0D;
                if ( KeyboardManager.KeyboardProfile.Active(KeyboardManager.ViewLeft) )
                {
                    Matrix3DMath.VectorForwardsRotationByMatrix(ViewAngleMatrix, Roll, ref xyzDbl);
                    viewAngleChange += xyzDbl;
                }
                if ( KeyboardManager.KeyboardProfile.Active(KeyboardManager.ViewRight) )
                {
                    Matrix3DMath.VectorBackwardsRotationByMatrix(ViewAngleMatrix, Roll, ref xyzDbl);
                    viewAngleChange += xyzDbl;
                }
                if ( KeyboardManager.KeyboardProfile.Active(KeyboardManager.ViewBackward) )
                {
                    Matrix3DMath.VectorLeftRotationByMatrix(ViewAngleMatrix, panRate, ref xyzDbl);
                    viewAngleChange += xyzDbl;
                }
                if ( KeyboardManager.KeyboardProfile.Active(KeyboardManager.ViewForward) )
                {
                    Matrix3DMath.VectorRightRotationByMatrix(ViewAngleMatrix, panRate, ref xyzDbl);
                    viewAngleChange += xyzDbl;
                }
                if ( KeyboardManager.KeyboardProfile.Active(KeyboardManager.ViewRollLeft) )
                {
                    Matrix3DMath.VectorDownRotationByMatrix(ViewAngleMatrix, panRate, ref xyzDbl);
                    viewAngleChange += xyzDbl;
                }
                if ( KeyboardManager.KeyboardProfile.Active(KeyboardManager.ViewRollRight) )
                {
                    Matrix3DMath.VectorUpRotationByMatrix(ViewAngleMatrix, panRate, ref xyzDbl);
                    viewAngleChange += xyzDbl;
                }

                if ( viewPosChangeXyz.X != 0.0D | viewPosChangeXyz.Y != 0.0D | viewPosChangeXyz.Z != 0.0D )
                {
                    ViewPosChange(viewPosChangeXyz);
                }
                //do rotation
                if ( viewAngleChange.X != 0.0D | viewAngleChange.Y != 0.0D | viewAngleChange.Z != 0.0D )
                {
                    Matrix3DMath.VectorToPY(viewAngleChange, ref anglePY);
                    Matrix3DMath.MatrixSetToPY(matrixA, anglePY);
                    Matrix3DMath.MatrixRotationAroundAxis(ViewAngleMatrix, matrixA, viewAngleChange.GetMagnitude(), matrixB);
                    ViewAngleSetRotate(matrixB);
                }
            }
            else if ( App.ViewMoveType == ViewMoveType.RTS )
            {
                viewPosChangeXyz = new XYZInt(0, 0, 0);

                Matrix3DMath.MatrixToPY(ViewAngleMatrix, ref anglePY);
                Matrix3DMath.MatrixSetToYAngle(matrixA, anglePY.Yaw);
                if ( KeyboardManager.KeyboardProfile.Active(KeyboardManager.ViewMoveForward) )
                {
                    Matrix3DMath.VectorForwardsRotationByMatrix(matrixA, Move, ref xyzDbl);
                    viewPosChangeXyz.AddDbl(xyzDbl);
                }
                if ( KeyboardManager.KeyboardProfile.Active(KeyboardManager.ViewMoveBackward) )
                {
                    Matrix3DMath.VectorBackwardsRotationByMatrix(matrixA, Move, ref xyzDbl);
                    viewPosChangeXyz.AddDbl(xyzDbl);
                }
                if ( KeyboardManager.KeyboardProfile.Active(KeyboardManager.ViewMoveLeft) )
                {
                    Matrix3DMath.VectorLeftRotationByMatrix(matrixA, Move, ref xyzDbl);
                    viewPosChangeXyz.AddDbl(xyzDbl);
                }
                if ( KeyboardManager.KeyboardProfile.Active(KeyboardManager.ViewMoveRight) )
                {
                    Matrix3DMath.VectorRightRotationByMatrix(matrixA, Move, ref xyzDbl);
                    viewPosChangeXyz.AddDbl(xyzDbl);
                }
                if ( KeyboardManager.KeyboardProfile.Active(KeyboardManager.ViewMoveUp) )
                {
                    viewPosChangeXyz.Y += (int)Move;
                }
                if ( KeyboardManager.KeyboardProfile.Active(KeyboardManager.ViewMoveDown) )
                {
                    viewPosChangeXyz.Y -= (int)Move;
                }

                if ( App.RTSOrbit )
                {
                    if ( KeyboardManager.KeyboardProfile.Active(KeyboardManager.ViewForward) )
                    {
                        anglePY.Pitch = MathUtil.ClampDbl(anglePY.Pitch + OrbitRate, Convert.ToDouble(- MathUtil.RadOf90Deg + 0.03125D * MathUtil.RadOf1Deg),
                            MathUtil.RadOf90Deg - 0.03125D * MathUtil.RadOf1Deg);
                        angleChanged = true;
                    }
                    if ( KeyboardManager.KeyboardProfile.Active(KeyboardManager.ViewBackward) )
                    {
                        anglePY.Pitch = MathUtil.ClampDbl(anglePY.Pitch - OrbitRate, Convert.ToDouble(- MathUtil.RadOf90Deg + 0.03125D * MathUtil.RadOf1Deg),
                            MathUtil.RadOf90Deg - 0.03125D * MathUtil.RadOf1Deg);
                        angleChanged = true;
                    }
                    if ( KeyboardManager.KeyboardProfile.Active(KeyboardManager.ViewLeft) )
                    {
                        anglePY.Yaw = MathUtil.AngleClamp(anglePY.Yaw + OrbitRate);
                        angleChanged = true;
                    }
                    if ( KeyboardManager.KeyboardProfile.Active(KeyboardManager.ViewRight) )
                    {
                        anglePY.Yaw = MathUtil.AngleClamp(anglePY.Yaw - OrbitRate);
                        angleChanged = true;
                    }
                }
                else
                {
                    if ( KeyboardManager.KeyboardProfile.Active(KeyboardManager.ViewForward) )
                    {
                        anglePY.Pitch = MathUtil.ClampDbl(anglePY.Pitch - OrbitRate, Convert.ToDouble(- MathUtil.RadOf90Deg + 0.03125D * MathUtil.RadOf1Deg),
                            MathUtil.RadOf90Deg - 0.03125D * MathUtil.RadOf1Deg);
                        angleChanged = true;
                    }
                    if ( KeyboardManager.KeyboardProfile.Active(KeyboardManager.ViewBackward) )
                    {
                        anglePY.Pitch = MathUtil.ClampDbl(anglePY.Pitch + OrbitRate, Convert.ToDouble(- MathUtil.RadOf90Deg + 0.03125D * MathUtil.RadOf1Deg),
                            MathUtil.RadOf90Deg - 0.03125D * MathUtil.RadOf1Deg);
                        angleChanged = true;
                    }
                    if ( KeyboardManager.KeyboardProfile.Active(KeyboardManager.ViewLeft) )
                    {
                        anglePY.Yaw = MathUtil.AngleClamp(anglePY.Yaw - OrbitRate);
                        angleChanged = true;
                    }
                    if ( KeyboardManager.KeyboardProfile.Active(KeyboardManager.ViewRight) )
                    {
                        anglePY.Yaw = MathUtil.AngleClamp(anglePY.Yaw + OrbitRate);
                        angleChanged = true;
                    }
                }

                //Dim HeightChange As Double
                //HeightChange = Map.Terrain_Height_Get(view.View_Pos.X + ViewPosChange.X, view.View_Pos.Z + ViewPosChange.Z) - Map.Terrain_Height_Get(view.View_Pos.X, view.View_Pos.Z)

                //ViewPosChange.Y = ViewPosChange.Y + HeightChange

                if ( viewPosChangeXyz.X != 0.0D | viewPosChangeXyz.Y != 0.0D | viewPosChangeXyz.Z != 0.0D )
                {
                    ViewPosChange(viewPosChangeXyz);
                }
                if ( angleChanged )
                {
                    Matrix3DMath.MatrixSetToPY(matrixA, anglePY);
                    ViewAngleSetRotate(matrixA);
                }
            }
        }

        public void TimedTools()
        {
            if ( modTools.Tool == modTools.Tools.HeightSmoothBrush )
            {
                if ( GetMouseOverTerrain() != null )
                {
                    if ( GetMouseLeftDownOverTerrain() != null )
                    {
                        double dblTemp = 0;
                        if ( !IOUtil.InvariantParse(Program.frmMainInstance.txtSmoothRate.Text, ref dblTemp) )
                        {
                            return;
                        }
                        ApplyHeightSmoothing(MathUtil.ClampDbl(dblTemp * Program.frmMainInstance.tmrTool.Interval / 1000.0D, 0.0D, 1.0D));
                    }
                }
            }
            else if ( modTools.Tool == modTools.Tools.HeightChangeBrush )
            {
                if ( GetMouseOverTerrain() != null )
                {
                    double dblTemp = 0;
                    if ( !IOUtil.InvariantParse(Program.frmMainInstance.txtHeightChangeRate.Text, ref dblTemp) )
                    {
                        return;
                    }
                    if ( GetMouseLeftDownOverTerrain() != null )
                    {
                        ApplyHeightChange(MathUtil.ClampDbl(dblTemp, -255.0D, 255.0D));
                    }
                    else if ( GetMouseRightDownOverTerrain() != null )
                    {
                        ApplyHeightChange(MathUtil.ClampDbl(Convert.ToDouble(- dblTemp), -255.0D, 255.0D));
                    }
                }
            }
        }

        public void ApplyObjectLine()
        {
            if ( Program.frmMainInstance.SingleSelectedObjectTypeBase != null && Map.SelectedUnitGroup != null )
            {
                var mouseOverTerrian = GetMouseOverTerrain();

                if ( mouseOverTerrian == null )
                {
                    return;
                }

                var num = 0;
                var a = 0;
                var b = 0;
                var tile = mouseOverTerrian.Tile.Normal;

                if ( Map.Selected_Tile_A != null )
                {
                    if ( tile.X == Map.Selected_Tile_A.X )
                    {
                        if ( tile.Y <= Map.Selected_Tile_A.Y )
                        {
                            a = tile.Y;
                            b = Map.Selected_Tile_A.Y;
                        }
                        else
                        {
                            a = Map.Selected_Tile_A.Y;
                            b = tile.Y;
                        }
                        var objectCreator = new clsUnitCreate();
                        Map.SetObjectCreatorDefaults(objectCreator);
                        for ( num = a; num <= b; num++ )
                        {
                            objectCreator.Horizontal.X = (int)((tile.X + 0.5D) * Constants.TerrainGridSpacing);
                            objectCreator.Horizontal.Y = (int)((num + 0.5D) * Constants.TerrainGridSpacing);
                            objectCreator.Perform();
                        }

                        Map.UndoStepCreate("Object Line");
                        Map.Update();
                        Map.MinimapMakeLater();
                        Map.Selected_Tile_A = null;
                        MapViewControl.DrawViewLater();
                    }
                    else if ( tile.Y == Map.Selected_Tile_A.Y )
                    {
                        if ( tile.X <= Map.Selected_Tile_A.X )
                        {
                            a = tile.X;
                            b = Map.Selected_Tile_A.X;
                        }
                        else
                        {
                            a = Map.Selected_Tile_A.X;
                            b = tile.X;
                        }
                        var objectCreator = new clsUnitCreate();
                        Map.SetObjectCreatorDefaults(objectCreator);
                        for ( num = a; num <= b; num++ )
                        {
                            objectCreator.Horizontal.X = (int)((num + 0.5D) * Constants.TerrainGridSpacing);
                            objectCreator.Horizontal.Y = (int)((tile.Y + 0.5D) * Constants.TerrainGridSpacing);
                            objectCreator.Perform();
                        }

                        Map.UndoStepCreate("Object Line");
                        Map.Update();
                        Map.MinimapMakeLater();
                        Map.Selected_Tile_A = null;
                        MapViewControl.DrawViewLater();
                    }
                }
                else
                {
                    Map.Selected_Tile_A = tile;
                }
            }
        }

        public class clsMouseDown
        {
            public clsOverMinimap OverMinimap;
            public clsOverTerrain OverTerrain;

            public class clsOverMinimap
            {
                public XYInt DownPos;
            }

            public class clsOverTerrain
            {
                public WorldPos DownPos;
            }
        }

        public class clsMouseOver
        {
            public clsOverTerrain OverTerrain;
            public XYInt ScreenPos;

            public clsMouseOver()
            {
                ScreenPos = new XYInt(0, 0);
            }

            public class clsOverTerrain
            {
                public WorldPos Pos;
                public bool Side_IsV;
                public XYInt Side_Num;
                public clsBrush.sPosNum Tile;
                public bool Triangle;
                public SimpleClassList<clsUnit> Units = new SimpleClassList<clsUnit>();
                public clsBrush.sPosNum Vertex;
            }
        }
    }
}