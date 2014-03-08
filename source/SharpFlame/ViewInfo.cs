#region

using System;
using System.Windows.Forms;
using Eto.Gl;
using Ninject;
using Ninject.Extensions.Logging;
using SharpFlame.Collections;
using SharpFlame.Controls;
using SharpFlame.Core;
using SharpFlame.Core.Collections;
using SharpFlame.Core.Domain;
using SharpFlame.FileIO;
using SharpFlame.Gui.Sections;
using SharpFlame.Infrastructure;
using SharpFlame.Mapping;
using SharpFlame.Mapping.Objects;
using SharpFlame.Mapping.Script;
using SharpFlame.Mapping.Tiles;
using SharpFlame.Mapping.Tools;
using SharpFlame.Maths;
using SharpFlame.Util;
using SharpFlame.Painters;
using SharpFlame.Settings;

#endregion

namespace SharpFlame
{
    /// <summary>
    /// Seems like this is the map user control class.
    /// </summary>
    public class ViewInfo
    {
        private readonly ILogger logger;
        public double FOVMultiplier;
        public double FOVMultiplierExponent;
        public float FieldOfViewY;
        public clsMouseDown MouseLeftDown;
        public clsMouseOver MouseOver;
        public clsMouseDown MouseRightDown;
        public double TilesPerMinimapPixel;
        public Matrix3DMath.Matrix3D ViewAngleMatrix = new Matrix3DMath.Matrix3D();
        public Matrix3DMath.Matrix3D ViewAngleMatrixInverted = new Matrix3DMath.Matrix3D();
        public Angles.AngleRPY ViewAngleRPY;
        public XYZInt ViewPos;

        private Map map;

        public MainMapView MainMapView { get; set; }

        [Inject]
        public KeyboardManager KeyboardManager { get; set; }

        public ViewInfo(Map myMap, ILoggerFactory logFactory, MainMapView mmv)
        {
            logger = logFactory.GetCurrentClassLogger();
            map = myMap;
            MainMapView = mmv;

            ViewPos = new XYZInt(0, 3072, 0);
            FovMultiplierSet(App.SettingsManager.FOVDefault);
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
            FOVMultiplier = Math.Tan(radians / 2.0D) / mapViewControl.Size.Height * 2.0D;
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

            FieldOfViewY = (float)(Math.Atan(MainMapView.GLSurface.Size.Height * FOVMultiplier / 2.0D) * 2.0D);
            if ( FieldOfViewY < min )
            {
                FieldOfViewY = min;
                if ( MainMapView.GLSurface.Size.Height > 0 )
                {
                    FOVMultiplier = 2.0D * Math.Tan(FieldOfViewY / 2.0D) / MainMapView.GLSurface.Size.Height;
                    FOVMultiplierExponent = Math.Log(FOVMultiplier) / Math.Log(2.0D);
                }
            }
            else if ( FieldOfViewY > max )
            {
                FieldOfViewY = max;
                if ( MainMapView.GLSurface.Size.Height > 0 )
                {
                    FOVMultiplier = 2.0D * Math.Tan(FieldOfViewY / 2.0D) / MainMapView.GLSurface.Size.Height;
                    FOVMultiplierExponent = Math.Log(FOVMultiplier) / Math.Log(2.0D);
                }
            }

            MainMapView.Draw();
        }

        public void ViewPosSet(XYZInt newViewPos)
        {
            ViewPos = newViewPos;
            viewPosClamp();

            MainMapView.Draw();
        }

        public void ViewPosChange(XYZInt displacement)
        {
            ViewPos.X += displacement.X;
            ViewPos.Z += displacement.Z;
            ViewPos.Y += displacement.Y;
            viewPosClamp();

            MainMapView.Draw();
        }

        private void viewPosClamp()
        {
            const int maxHeight = 1048576;
            const int maxDist = 1048576;

            ViewPos.X = MathUtil.ClampInt(ViewPos.X, Convert.ToInt32(- maxDist), map.Terrain.TileSize.X * Constants.TerrainGridSpacing + maxDist);
            ViewPos.Z = MathUtil.ClampInt(ViewPos.Z, - map.Terrain.TileSize.Y * Constants.TerrainGridSpacing - maxDist, maxDist);
            ViewPos.Y = MathUtil.ClampInt(ViewPos.Y, ((int)(Math.Ceiling(map.GetTerrainHeight(new XYInt(ViewPos.X, - ViewPos.Z))))) + 16, maxHeight);
        }

        public void ViewAngleSet(Matrix3DMath.Matrix3D newMatrix)
        {
            Matrix3DMath.MatrixCopy(newMatrix, ViewAngleMatrix);
            Matrix3DMath.MatrixNormalize(ViewAngleMatrix);
            Matrix3DMath.MatrixInvert(ViewAngleMatrix, ViewAngleMatrixInverted);
            Matrix3DMath.MatrixToRPY(ViewAngleMatrix, ref ViewAngleRPY);

            MainMapView.Draw();
        }

        public void ViewAngleSetToDefault()
        {
            var matrixA = new Matrix3DMath.Matrix3D();
            Matrix3DMath.MatrixSetToXAngle(matrixA, Math.Atan(2.0D));
            ViewAngleSet(matrixA);

            MainMapView.Draw();
        }

        public void ViewAngleSetRotate(Matrix3DMath.Matrix3D newMatrix)
        {
            var flag = false;
            XYZDouble xyzDbl = default(XYZDouble);
            XYDouble xyDbl = default(XYDouble);

            if ( App.ViewMoveType == ViewMoveType.RTS & App.RTSOrbit )
            {
                if ( ScreenXYGetViewPlanePosForwardDownOnly((int)((MainMapView.GLSurface.Size.Width / 2.0D)), (int)((MainMapView.GLSurface.Size.Height / 2.0D)), 127.5D,
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

            MainMapView.Draw();
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
            var dblTemp = map.GetTerrainHeight(horizontal);
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

        public void MoveToViewTerrainPosFromDistance(XYZDouble terrainPos, double distance)
        {
            var xyzDbl = default(XYZDouble);
            var xyzInt = new XYZInt(0, 0, 0);

            Matrix3DMath.VectorForwardsRotationByMatrix(ViewAngleMatrix, ref xyzDbl);

            xyzInt.X = (int)(terrainPos.X - xyzDbl.X * distance);
            xyzInt.Y = (int)(terrainPos.Y - xyzDbl.Y * distance);
            xyzInt.Z = (int)(- terrainPos.Z - xyzDbl.Z * distance);

            ViewPosSet(xyzInt);
        }

        public bool PosGetScreenXY(XYZDouble pos, ref XYInt result)
        {
            if ( pos.Z <= 0.0D )
            {
                return false;
            }

            try
            {
                var ratioZpx = 1.0D / (FOVMultiplier * pos.Z);
                result.X = (int)(MainMapView.GLSurface.Size.Height / 2.0D + (pos.X * ratioZpx));
                result.Y = (int)(MainMapView.GLSurface.Size.Width / 2.0D - (pos.Y * ratioZpx));
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
                xyzDbl.X = (screenPos.X - MainMapView.GLSurface.Size.Width / 2.0D) * FOVMultiplier;
                xyzDbl.Y = (MainMapView.GLSurface.Size.Height / 2.0D - screenPos.Y) * FOVMultiplier;
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
            var xyzDbl = default(XYZDouble);
            var terrainViewVector = default(XYZDouble);
            var limitA = default(XYDouble);
            var limitB = default(XYDouble);
            var min = new XYInt();
            var max = new XYInt();
            var bestPos = default(XYZDouble);
            var tilePos = default(XYDouble);
            var terrainViewPos = default(XYZDouble);

            try
            {
                terrainViewPos.X = ViewPos.X;
                terrainViewPos.Y = ViewPos.Y;
                terrainViewPos.Z = Convert.ToDouble(- ViewPos.Z);

                //convert screen pos to vector of one pos unit
                xyzDbl.X = (screenPos.X - MainMapView.GLSurface.Size.Height/ 2.0D) * FOVMultiplier;
                xyzDbl.Y = (MainMapView.GLSurface.Size.Width / 2.0D - screenPos.Y) * FOVMultiplier;
                xyzDbl.Z = 1.0D;
                //rotate the vector so that it points forward and level
                Matrix3DMath.VectorRotationByMatrix(ViewAngleMatrix, xyzDbl, ref terrainViewVector);
                terrainViewVector.Y = Convert.ToDouble(- terrainViewVector.Y); //get the amount of looking down, not up
                terrainViewVector.Z = Convert.ToDouble(- terrainViewVector.Z); //convert to terrain coordinates from view coordinates
                //get range of possible tiles
                double dblTemp = (terrainViewPos.Y - 255 * map.HeightMultiplier) / terrainViewVector.Y;
                limitA.X = terrainViewPos.X + terrainViewVector.X * dblTemp;
                limitA.Y = terrainViewPos.Z + terrainViewVector.Z * dblTemp;
                dblTemp = terrainViewPos.Y / terrainViewVector.Y;
                limitB.X = terrainViewPos.X + terrainViewVector.X * dblTemp;
                limitB.Y = terrainViewPos.Z + terrainViewVector.Z * dblTemp;
                min.X = Math.Max(Convert.ToInt32((Math.Min(limitA.X, limitB.X) / Constants.TerrainGridSpacing)), 0);
                min.Y = Math.Max((int)((Math.Min(limitA.Y, limitB.Y) / Constants.TerrainGridSpacing)), 0);
                max.X = Math.Min(Convert.ToInt32((Math.Max(limitA.X, limitB.X) / Constants.TerrainGridSpacing)), map.Terrain.TileSize.X - 1);
                max.Y = Math.Min(Convert.ToInt32((Math.Max(limitA.Y, limitB.Y) / Constants.TerrainGridSpacing)), map.Terrain.TileSize.Y - 1);
                //find the nearest valid tile to the view
                double bestDist = double.MaxValue;
                bestPos.X = double.NaN;
                bestPos.Y = double.NaN;
                bestPos.Z = double.NaN;
                var y = 0;
                for ( y = min.Y; y <= max.Y; y++ )
                {
                    var x = 0;
                    for ( x = min.X; x <= max.X; x++ )
                    {
                        tilePos.X = x * Constants.TerrainGridSpacing;
                        tilePos.Y = y * Constants.TerrainGridSpacing;

                        double triGradientX = 0;
                        double triGradientZ = 0;
                        double triHeightOffset = 0;
                        double dist = 0;
                        double inTileX = 0;
                        XYZDouble dif;
                        double inTileZ = 0;
                        if ( map.Terrain.Tiles[x, y].Tri )
                        {
                            triHeightOffset = Convert.ToDouble(map.Terrain.Vertices[x, y].Height * map.HeightMultiplier);
                            triGradientX = Convert.ToDouble(map.Terrain.Vertices[x + 1, y].Height * map.HeightMultiplier - triHeightOffset);
                            triGradientZ = Convert.ToDouble(map.Terrain.Vertices[x, y + 1].Height * map.HeightMultiplier - triHeightOffset);
                            xyzDbl.Y = (triHeightOffset +
                                         (triGradientX * (terrainViewPos.X - tilePos.X) + triGradientZ * (terrainViewPos.Z - tilePos.Y) +
                                          (triGradientX * terrainViewVector.X + triGradientZ * terrainViewVector.Z) * terrainViewPos.Y / terrainViewVector.Y) /
                                         Constants.TerrainGridSpacing) /
                                        (1.0D +
                                         (triGradientX * terrainViewVector.X + triGradientZ * terrainViewVector.Z) / (terrainViewVector.Y * Constants.TerrainGridSpacing));
                            xyzDbl.X = terrainViewPos.X + terrainViewVector.X * (terrainViewPos.Y - xyzDbl.Y) / terrainViewVector.Y;
                            xyzDbl.Z = terrainViewPos.Z + terrainViewVector.Z * (terrainViewPos.Y - xyzDbl.Y) / terrainViewVector.Y;
                            inTileX = xyzDbl.X / Constants.TerrainGridSpacing - x;
                            inTileZ = xyzDbl.Z / Constants.TerrainGridSpacing - y;
                            if ( inTileZ <= 1.0D - inTileX & inTileX >= 0.0D & inTileZ >= 0.0D & inTileX <= 1.0D & inTileZ <= 1.0D )
                            {
                                dif = xyzDbl - terrainViewPos;
                                dist = dif.GetMagnitude();
                                if ( dist < bestDist )
                                {
                                    bestDist = dist;
                                    bestPos = xyzDbl;
                                }
                            }

                            triHeightOffset = Convert.ToDouble(map.Terrain.Vertices[x + 1, y + 1].Height * map.HeightMultiplier);
                            triGradientX = Convert.ToDouble(map.Terrain.Vertices[x, y + 1].Height * map.HeightMultiplier - triHeightOffset);
                            triGradientZ = Convert.ToDouble(map.Terrain.Vertices[x + 1, y].Height * map.HeightMultiplier - triHeightOffset);
                            xyzDbl.Y = (triHeightOffset + triGradientX + triGradientZ +
                                         (triGradientX * (tilePos.X - terrainViewPos.X) + triGradientZ * (tilePos.Y - terrainViewPos.Z) -
                                          (triGradientX * terrainViewVector.X + triGradientZ * terrainViewVector.Z) * terrainViewPos.Y / terrainViewVector.Y) /
                                         Constants.TerrainGridSpacing) /
                                        (1.0D -
                                         (triGradientX * terrainViewVector.X + triGradientZ * terrainViewVector.Z) / (terrainViewVector.Y * Constants.TerrainGridSpacing));
                            xyzDbl.X = terrainViewPos.X + terrainViewVector.X * (terrainViewPos.Y - xyzDbl.Y) / terrainViewVector.Y;
                            xyzDbl.Z = terrainViewPos.Z + terrainViewVector.Z * (terrainViewPos.Y - xyzDbl.Y) / terrainViewVector.Y;
                            inTileX = xyzDbl.X / Constants.TerrainGridSpacing - x;
                            inTileZ = xyzDbl.Z / Constants.TerrainGridSpacing - y;
                            if ( inTileZ >= 1.0D - inTileX & inTileX >= 0.0D & inTileZ >= 0.0D & inTileX <= 1.0D & inTileZ <= 1.0D )
                            {
                                dif = xyzDbl - terrainViewPos;
                                dist = dif.GetMagnitude();
                                if ( dist < bestDist )
                                {
                                    bestDist = dist;
                                    bestPos = xyzDbl;
                                }
                            }
                        }
                        else
                        {
                            triHeightOffset = Convert.ToDouble(map.Terrain.Vertices[x + 1, y].Height * map.HeightMultiplier);
                            triGradientX = Convert.ToDouble(map.Terrain.Vertices[x, y].Height * map.HeightMultiplier - triHeightOffset);
                            triGradientZ = Convert.ToDouble(map.Terrain.Vertices[x + 1, y + 1].Height * map.HeightMultiplier - triHeightOffset);
                            xyzDbl.Y = (triHeightOffset + triGradientX +
                                         (triGradientX * (tilePos.X - terrainViewPos.X) + triGradientZ * (terrainViewPos.Z - tilePos.Y) -
                                          (triGradientX * terrainViewVector.X - triGradientZ * terrainViewVector.Z) * terrainViewPos.Y / terrainViewVector.Y) /
                                         Constants.TerrainGridSpacing) /
                                        (1.0D -
                                         (triGradientX * terrainViewVector.X - triGradientZ * terrainViewVector.Z) / (terrainViewVector.Y * Constants.TerrainGridSpacing));
                            xyzDbl.X = terrainViewPos.X + terrainViewVector.X * (terrainViewPos.Y - xyzDbl.Y) / terrainViewVector.Y;
                            xyzDbl.Z = terrainViewPos.Z + terrainViewVector.Z * (terrainViewPos.Y - xyzDbl.Y) / terrainViewVector.Y;
                            inTileX = xyzDbl.X / Constants.TerrainGridSpacing - x;
                            inTileZ = xyzDbl.Z / Constants.TerrainGridSpacing - y;
                            if ( inTileZ <= inTileX & inTileX >= 0.0D & inTileZ >= 0.0D & inTileX <= 1.0D & inTileZ <= 1.0D )
                            {
                                dif = xyzDbl - terrainViewPos;
                                dist = dif.GetMagnitude();
                                if ( dist < bestDist )
                                {
                                    bestDist = dist;
                                    bestPos = xyzDbl;
                                }
                            }

                            triHeightOffset = Convert.ToDouble(map.Terrain.Vertices[x, y + 1].Height * map.HeightMultiplier);
                            triGradientX = Convert.ToDouble(map.Terrain.Vertices[x + 1, y + 1].Height * map.HeightMultiplier - triHeightOffset);
                            triGradientZ = Convert.ToDouble(map.Terrain.Vertices[x, y].Height * map.HeightMultiplier - triHeightOffset);
                            xyzDbl.Y = (triHeightOffset + triGradientZ +
                                         (triGradientX * (terrainViewPos.X - tilePos.X) + triGradientZ * (tilePos.Y - terrainViewPos.Z) +
                                          (triGradientX * terrainViewVector.X - triGradientZ * terrainViewVector.Z) * terrainViewPos.Y / terrainViewVector.Y) /
                                         Constants.TerrainGridSpacing) /
                                        (1.0D +
                                         (triGradientX * terrainViewVector.X - triGradientZ * terrainViewVector.Z) / (terrainViewVector.Y * Constants.TerrainGridSpacing));
                            xyzDbl.X = terrainViewPos.X + terrainViewVector.X * (terrainViewPos.Y - xyzDbl.Y) / terrainViewVector.Y;
                            xyzDbl.Z = terrainViewPos.Z + terrainViewVector.Z * (terrainViewPos.Y - xyzDbl.Y) / terrainViewVector.Y;
                            inTileX = xyzDbl.X / Constants.TerrainGridSpacing - x;
                            inTileZ = xyzDbl.Z / Constants.TerrainGridSpacing - y;
                            if ( inTileZ >= inTileX & inTileX >= 0.0D & inTileZ >= 0.0D & inTileX <= 1.0D & inTileZ <= 1.0D )
                            {
                                dif = xyzDbl - terrainViewPos;
                                dist = dif.GetMagnitude();
                                if ( dist < bestDist )
                                {
                                    bestDist = dist;
                                    bestPos = xyzDbl;
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
            var xyzDouble = default(XYZDouble);
            var xyzDbl2 = default(XYZDouble);

            if ( ViewPos.Y < planeHeight )
            {
                return false;
            }

            try
            {
                //convert screen pos to vector of one pos unit
                double dblTemp2 = FOVMultiplier;
                xyzDouble.X = (screenX - MainMapView.GLSurface.Size.Width / 2.0D) * dblTemp2;
                xyzDouble.Y = (MainMapView.GLSurface.Size.Height / 2.0D - screenY) * dblTemp2;
                xyzDouble.Z = 1.0D;
                //factor in the view angle
                Matrix3DMath.VectorRotationByMatrix(ViewAngleMatrix, xyzDouble, ref xyzDbl2);
                //get distance to cover the height
                if ( xyzDbl2.Y > 0.0D )
                {
                    return false;
                }
                double dblTemp = (planeHeight - ViewPos.Y) / xyzDbl2.Y;
                resultPos.X = ViewPos.X + xyzDbl2.X * dblTemp;
                resultPos.Y = ViewPos.Z + xyzDbl2.Z * dblTemp;
            }
            catch
            {
                return false;
            }
            return true;
        }

        public void MouseOverPosCalc()
        {
            var xyDouble = default(XYDouble);
            var flag = false;
            var mouseLeftDownOverMinimap = GetMouseLeftDownOverMinimap();

            if ( mouseLeftDownOverMinimap != null )
            {
                if ( MouseOver == null )
                {
                }
                else if ( IsViewPosOverMinimap(MouseOver.ScreenPos) )
                {
                    var pos = new XYInt((int)(MouseOver.ScreenPos.X * TilesPerMinimapPixel),
                        (int)((MouseOver.ScreenPos.Y * TilesPerMinimapPixel)));
                    map.TileNumClampToMap(pos);
                    LookAtTile(pos);
                }
            }
            else
            {
                var mouseOverTerrain = new clsMouseOver.clsOverTerrain();
                if ( App.SettingsManager.DirectPointer )
                {
                    if ( ScreenXYGetTerrainPos(MouseOver.ScreenPos, ref mouseOverTerrain.Pos) )
                    {
                        if ( map.PosIsOnMap(mouseOverTerrain.Pos.Horizontal) )
                        {
                            flag = true;
                        }
                    }
                }
                else
                {
                    mouseOverTerrain.Pos.Altitude = (int)(255.0D / 2.0D * map.HeightMultiplier);
                    if ( ScreenXYGetViewPlanePos(MouseOver.ScreenPos, mouseOverTerrain.Pos.Altitude, ref xyDouble) )
                    {
                        mouseOverTerrain.Pos.Horizontal.X = (int)xyDouble.X;
                        mouseOverTerrain.Pos.Horizontal.Y = Convert.ToInt32(- xyDouble.Y);
                        if ( map.PosIsOnMap(mouseOverTerrain.Pos.Horizontal) )
                        {
                            mouseOverTerrain.Pos.Altitude = (int)(map.GetTerrainHeight(mouseOverTerrain.Pos.Horizontal));
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
                    mouseOverTerrain.Triangle = map.GetTerrainTri(mouseOverTerrain.Pos.Horizontal);
                    xyDouble.X = mouseOverTerrain.Pos.Horizontal.X - mouseOverTerrain.Vertex.Normal.X * Constants.TerrainGridSpacing;
                    xyDouble.Y = mouseOverTerrain.Pos.Horizontal.Y - mouseOverTerrain.Vertex.Normal.Y * Constants.TerrainGridSpacing;
                    if ( Math.Abs(xyDouble.Y) <= Math.Abs(xyDouble.X) )
                    {
                        mouseOverTerrain.SideIsV = false;
                        mouseOverTerrain.SideNum.X = mouseOverTerrain.Tile.Normal.X;
                        mouseOverTerrain.SideNum.Y = mouseOverTerrain.Vertex.Normal.Y;
                    }
                    else
                    {
                        mouseOverTerrain.SideIsV = true;
                        mouseOverTerrain.SideNum.X = mouseOverTerrain.Vertex.Normal.X;
                        mouseOverTerrain.SideNum.Y = mouseOverTerrain.Tile.Normal.Y;
                    }
                    var sectorNum = map.GetPosSectorNum(mouseOverTerrain.Pos.Horizontal);
                    var unit = default(Unit);
                    var connection = default(clsUnitSectorConnection);
                    foreach ( var tempLoopVar_Connection in map.Sectors[sectorNum.X, sectorNum.Y].Units )
                    {
                        connection = tempLoopVar_Connection;
                        unit = connection.Unit;
                        xyDouble.X = unit.Pos.Horizontal.X - mouseOverTerrain.Pos.Horizontal.X;
                        xyDouble.Y = unit.Pos.Horizontal.Y - mouseOverTerrain.Pos.Horizontal.Y;
                        var footprint = unit.TypeBase.GetGetFootprintSelected(unit.Rotation);
                        if ( Math.Abs(xyDouble.X) <= Math.Max(footprint.X / 2.0D, 0.5D) * Constants.TerrainGridSpacing
                             && Math.Abs(xyDouble.Y) <= Math.Max(footprint.Y / 2.0D, 0.5D) * Constants.TerrainGridSpacing )
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
            // MapViewControl.Pos_Display_Update();
            MainMapView.Draw();
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
            if ( pos.X >= 0 & pos.X < map.Terrain.TileSize.X / TilesPerMinimapPixel
                 & pos.Y >= 0 & pos.Y < map.Terrain.TileSize.Y / TilesPerMinimapPixel )
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

            var applyVertexTerrain = new clsApplyVertexTerrain
                {
                    Map = map, VertexTerrain = App.SelectedTerrain
                };
            App.TerrainBrush.PerformActionMapVertices(applyVertexTerrain, mouseOverTerrain.Vertex);

            map.Update();

            MainMapView.Draw();
        }

        public void ApplyRoad()
        {
            var mouseOverTerrain = GetMouseOverTerrain();

            if ( mouseOverTerrain == null )
            {
                return;
            }

            var sideNum = mouseOverTerrain.SideNum;
            var tileNum = new XYInt();

            if ( mouseOverTerrain.SideIsV )
            {
                if ( map.Terrain.SideV[sideNum.X, sideNum.Y].Road != App.SelectedRoad )
                {
                    map.Terrain.SideV[sideNum.X, sideNum.Y].Road = App.SelectedRoad;

                    if ( sideNum.X > 0 )
                    {
                        tileNum.X = sideNum.X - 1;
                        tileNum.Y = sideNum.Y;
                        map.AutoTextureChanges.TileChanged(tileNum);
                        map.SectorGraphicsChanges.TileChanged(tileNum);
                        map.SectorTerrainUndoChanges.TileChanged(tileNum);
                    }
                    if ( sideNum.X < map.Terrain.TileSize.X )
                    {
                        tileNum = sideNum;
                        map.AutoTextureChanges.TileChanged(tileNum);
                        map.SectorGraphicsChanges.TileChanged(tileNum);
                        map.SectorTerrainUndoChanges.TileChanged(tileNum);
                    }

                    map.Update();

                    map.UndoStepCreate("Road Side");

                    MainMapView.Draw();
                }
            }
            else
            {
                if ( map.Terrain.SideH[sideNum.X, sideNum.Y].Road != App.SelectedRoad )
                {
                    map.Terrain.SideH[sideNum.X, sideNum.Y].Road = App.SelectedRoad;

                    if ( sideNum.Y > 0 )
                    {
                        tileNum.X = sideNum.X;
                        tileNum.Y = sideNum.Y - 1;
                        map.AutoTextureChanges.TileChanged(tileNum);
                        map.SectorGraphicsChanges.TileChanged(tileNum);
                        map.SectorTerrainUndoChanges.TileChanged(tileNum);
                    }
                    if ( sideNum.Y < map.Terrain.TileSize.X )
                    {
                        tileNum = sideNum;
                        map.AutoTextureChanges.TileChanged(tileNum);
                        map.SectorGraphicsChanges.TileChanged(tileNum);
                        map.SectorTerrainUndoChanges.TileChanged(tileNum);
                    }

                    map.Update();

                    map.UndoStepCreate("Road Side");

                    MainMapView.Draw();
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

            var tile = mouseOverTerrian.Tile.Normal;
            var sideNum = new XYInt();

            if ( map.SelectedTileA != null )
            {
                var num = 0;
                var a = 0;
                var b = 0;
                if ( tile.X == map.SelectedTileA.X )
                {
                    if ( tile.Y <= map.SelectedTileA.Y )
                    {
                        a = tile.Y;
                        b = map.SelectedTileA.Y;
                    }
                    else
                    {
                        a = map.SelectedTileA.Y;
                        b = tile.Y;
                    }
                    for ( num = a + 1; num <= b; num++ )
                    {
                        map.Terrain.SideH[map.SelectedTileA.X, num].Road = App.SelectedRoad;
                        sideNum.X = map.SelectedTileA.X;
                        sideNum.Y = num;
                        map.AutoTextureChanges.SideHChanged(sideNum);
                        map.SectorGraphicsChanges.SideHChanged(sideNum);
                        map.SectorTerrainUndoChanges.SideHChanged(sideNum);
                    }

                    map.Update();

                    map.UndoStepCreate("Road Line");

                    map.SelectedTileA = null;
                    MainMapView.Draw();
                }
                else if ( tile.Y == map.SelectedTileA.Y )
                {
                    if ( tile.X <= map.SelectedTileA.X )
                    {
                        a = tile.X;
                        b = map.SelectedTileA.X;
                    }
                    else
                    {
                        a = map.SelectedTileA.X;
                        b = tile.X;
                    }
                    for ( num = a + 1; num <= b; num++ )
                    {
                        map.Terrain.SideV[num, map.SelectedTileA.Y].Road = App.SelectedRoad;
                        sideNum.X = num;
                        sideNum.Y = map.SelectedTileA.Y;
                        map.AutoTextureChanges.SideVChanged(sideNum);
                        map.SectorGraphicsChanges.SideVChanged(sideNum);
                        map.SectorTerrainUndoChanges.SideVChanged(sideNum);
                    }

                    map.Update();

                    map.UndoStepCreate("Road Line");

                    map.SelectedTileA = null;
                    MainMapView.Draw();
                }
            }
            else
            {
                map.SelectedTileA = tile;
            }
        }

        public void ApplyTerrainFill(FillCliffAction cliffAction, bool inside)
        {
            var mouseOverTerrain = GetMouseOverTerrain();

            if ( mouseOverTerrain == null )
            {
                return;
            }

            var startVertex = mouseOverTerrain.Vertex.Normal;

            var fillType = App.SelectedTerrain;
            var replaceType = map.Terrain.Vertices[startVertex.X, startVertex.Y].Terrain;
            if ( fillType == replaceType )
            {
                return; //otherwise will cause endless loop
            }

            var sourceOfFill = new XYInt[524289];
            var sourceOfFillCount = 0;
            var sourceOfFillNum = 0;
            var nextSource = new XYInt();

            sourceOfFill[0] = startVertex;
            sourceOfFillCount = 1;
            sourceOfFillNum = 0;
            while ( sourceOfFillNum < sourceOfFillCount )
            {
                var currentSource = sourceOfFill[sourceOfFillNum];

                bool stopForCliff;
                if ( cliffAction == FillCliffAction.StopBefore )
                {
                    stopForCliff = map.VertexIsCliffEdge(currentSource);
                }
                else
                {
                    stopForCliff = false;
                }
                var stopForEdge = false;
                if ( inside )
                {
                    if ( currentSource.X > 0 )
                    {
                        if ( currentSource.Y > 0 )
                        {
                            if ( map.Terrain.Vertices[currentSource.X - 1, currentSource.Y - 1].Terrain != replaceType
                                 && map.Terrain.Vertices[currentSource.X - 1, currentSource.Y - 1].Terrain != fillType )
                            {
                                stopForEdge = true;
                            }
                        }
                        if ( map.Terrain.Vertices[currentSource.X - 1, currentSource.Y].Terrain != replaceType
                             && map.Terrain.Vertices[currentSource.X - 1, currentSource.Y].Terrain != fillType )
                        {
                            stopForEdge = true;
                        }
                        if ( currentSource.Y < map.Terrain.TileSize.Y )
                        {
                            if ( map.Terrain.Vertices[currentSource.X - 1, currentSource.Y + 1].Terrain != replaceType
                                 && map.Terrain.Vertices[currentSource.X - 1, currentSource.Y + 1].Terrain != fillType )
                            {
                                stopForEdge = true;
                            }
                        }
                    }
                    if ( currentSource.Y > 0 )
                    {
                        if ( map.Terrain.Vertices[currentSource.X, currentSource.Y - 1].Terrain != replaceType
                             && map.Terrain.Vertices[currentSource.X, currentSource.Y - 1].Terrain != fillType )
                        {
                            stopForEdge = true;
                        }
                    }
                    if ( currentSource.X < map.Terrain.TileSize.X )
                    {
                        if ( currentSource.Y > 0 )
                        {
                            if ( map.Terrain.Vertices[currentSource.X + 1, currentSource.Y - 1].Terrain != replaceType
                                 && map.Terrain.Vertices[currentSource.X + 1, currentSource.Y - 1].Terrain != fillType )
                            {
                                stopForEdge = true;
                            }
                        }
                        if ( map.Terrain.Vertices[currentSource.X + 1, currentSource.Y].Terrain != replaceType
                             && map.Terrain.Vertices[currentSource.X + 1, currentSource.Y].Terrain != fillType )
                        {
                            stopForEdge = true;
                        }
                        if ( currentSource.Y < map.Terrain.TileSize.Y )
                        {
                            if ( map.Terrain.Vertices[currentSource.X + 1, currentSource.Y + 1].Terrain != replaceType
                                 && map.Terrain.Vertices[currentSource.X + 1, currentSource.Y + 1].Terrain != fillType )
                            {
                                stopForEdge = true;
                            }
                        }
                    }
                    if ( currentSource.Y < map.Terrain.TileSize.Y )
                    {
                        if ( map.Terrain.Vertices[currentSource.X, currentSource.Y + 1].Terrain != replaceType
                             && map.Terrain.Vertices[currentSource.X, currentSource.Y + 1].Terrain != fillType )
                        {
                            stopForEdge = true;
                        }
                    }
                }

                if ( !(stopForCliff || stopForEdge) )
                {
                    if ( map.Terrain.Vertices[currentSource.X, currentSource.Y].Terrain == replaceType )
                    {
                        map.Terrain.Vertices[currentSource.X, currentSource.Y].Terrain = fillType;
                        map.SectorGraphicsChanges.VertexChanged(currentSource);
                        map.SectorTerrainUndoChanges.VertexChanged(currentSource);
                        map.AutoTextureChanges.VertexChanged(currentSource);

                        nextSource.X = currentSource.X + 1;
                        nextSource.Y = currentSource.Y;
                        if ( nextSource.X >= 0 & nextSource.X <= map.Terrain.TileSize.X
                             & nextSource.Y >= 0 & nextSource.Y <= map.Terrain.TileSize.Y )
                        {
                            if ( cliffAction == FillCliffAction.StopAfter )
                            {
                                stopForCliff = map.SideHIsCliffOnBothSides(new XYInt(currentSource.X, currentSource.Y));
                            }
                            else
                            {
                                stopForCliff = false;
                            }
                            if ( !stopForCliff )
                            {
                                if ( map.Terrain.Vertices[nextSource.X, nextSource.Y].Terrain == replaceType )
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
                        if ( nextSource.X >= 0 & nextSource.X <= map.Terrain.TileSize.X
                             & nextSource.Y >= 0 & nextSource.Y <= map.Terrain.TileSize.Y )
                        {
                            if ( cliffAction == FillCliffAction.StopAfter )
                            {
                                stopForCliff = map.SideHIsCliffOnBothSides(new XYInt(currentSource.X - 1, currentSource.Y));
                            }
                            else
                            {
                                stopForCliff = false;
                            }
                            if ( !stopForCliff )
                            {
                                if ( map.Terrain.Vertices[nextSource.X, nextSource.Y].Terrain == replaceType )
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
                        if ( nextSource.X >= 0 & nextSource.X <= map.Terrain.TileSize.X
                             & nextSource.Y >= 0 & nextSource.Y <= map.Terrain.TileSize.Y )
                        {
                            if ( cliffAction == FillCliffAction.StopAfter )
                            {
                                stopForCliff = map.SideVIsCliffOnBothSides(new XYInt(currentSource.X, currentSource.Y));
                            }
                            else
                            {
                                stopForCliff = false;
                            }
                            if ( !stopForCliff )
                            {
                                if ( map.Terrain.Vertices[nextSource.X, nextSource.Y].Terrain == replaceType )
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
                        if ( nextSource.X >= 0 & nextSource.X <= map.Terrain.TileSize.X
                             & nextSource.Y >= 0 & nextSource.Y <= map.Terrain.TileSize.Y )
                        {
                            if ( cliffAction == FillCliffAction.StopAfter )
                            {
                                stopForCliff = map.SideVIsCliffOnBothSides(new XYInt(currentSource.X, currentSource.Y - 1));
                            }
                            else
                            {
                                stopForCliff = false;
                            }
                            if ( !stopForCliff )
                            {
                                if ( map.Terrain.Vertices[nextSource.X, nextSource.Y].Terrain == replaceType )
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
                    var remainingCount = sourceOfFillCount - sourceOfFillNum;
                    var moveCount = Math.Min(sourceOfFillNum, remainingCount);
                    var moveOffset = sourceOfFillCount - moveCount;
                    var a = 0;
                    for ( a = 0; a <= moveCount - 1; a++ )
                    {
                        sourceOfFill[a] = sourceOfFill[moveOffset + a];
                    }
                    sourceOfFillCount -= sourceOfFillNum;
                    sourceOfFillNum = 0;
                    if ( sourceOfFillCount * 3 < sourceOfFill.GetUpperBound(0) + 1 )
                    {
                        Array.Resize(ref sourceOfFill, sourceOfFillCount * 2 + 1 + 1);
                    }
                }
            }

            map.Update();

            map.UndoStepCreate("Ground Fill");

            MainMapView.Draw();
        }

        public void ApplyTexture()
        {
            var mouseOverTerrain = GetMouseOverTerrain();

            if ( mouseOverTerrain == null )
            {
                return;
            }

            var applyTexture = new clsApplyTexture
                {
                    Map = map,
                    TextureNum = App.SelectedTextureNum,
                    SetTexture = Program.frmMainInstance.chkSetTexture.Checked,
                    Orientation = RelativeToViewAngle(App.TextureOrientation),
                    RandomOrientation = Program.frmMainInstance.chkTextureOrientationRandomize.Checked,
                    SetOrientation = Program.frmMainInstance.chkSetTextureOrientation.Checked,
                    TerrainAction = Program.frmMainInstance.TextureTerrainAction
                };

            App.TextureBrush.PerformActionMapTiles(applyTexture, mouseOverTerrain.Tile);

            map.Update();

            MainMapView.Draw();
        }

        /// <summary>
        /// Given the current ViewAngleMatrix:Yaw, this method ensures the correct tile orientation 
        /// respecting where the view camera is oriented. 
        /// </summary>
        /// <param name="currentOrientation"></param>
        /// <returns></returns>
        private TileOrientation RelativeToViewAngle(TileOrientation currentOrientation)
        {
            var anglePY = new Angles.AnglePY();
            Matrix3DMath.MatrixToPY(ViewAngleMatrix, ref anglePY);

            logger.Debug( "View Angle: Yaw:{1}", anglePY.Pitch, anglePY.Yaw );

            var yaw = anglePY.Yaw;
            
            //heading/viewing north, no relative change.

            //heading east
            if ( yaw > Angles.RadOf90Deg / 4 && yaw < 3 * Angles.Pi / 4 )
            {
                currentOrientation.RotateClockwise();
            }
            //heading west
            else if ( yaw < -Angles.RadOf90Deg / 4 && yaw > -3 * Angles.Pi / 4 )
            {
                currentOrientation.RotateAntiClockwise();
            }
            //heading south
            else if( yaw > 3 * Angles.Pi / 4 )
            {
                currentOrientation.RotateClockwise();
                currentOrientation.RotateClockwise();
            }
            else if ( yaw < -3 * Angles.Pi / 4 )
            {
                currentOrientation.RotateAntiClockwise();
                currentOrientation.RotateAntiClockwise();
            }

            return currentOrientation;
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
                var applyCliffTriangleRemove = new clsApplyCliffTriangleRemove
                    {
                        Map = map, 
                        PosNum = mouseOverTerrain.Tile.Normal,
                        Triangle = mouseOverTerrain.Triangle
                    };
                applyCliffTriangleRemove.ActionPerform();
            }
            else
            {
                var applyCliffTriangle = new clsApplyCliffTriangle
                    {
                        Map = map, 
                        PosNum = mouseOverTerrain.Tile.Normal,
                        Triangle = mouseOverTerrain.Triangle
                    };
                applyCliffTriangle.ActionPerform();
            }

            map.Update();

            MainMapView.Draw();
        }

        public void ApplyCliff()
        {
            var mouseOverTerrain = GetMouseOverTerrain();

            if ( mouseOverTerrain == null )
            {
                return;
            }

            var applyCliff = new clsApplyCliff();
            applyCliff.Map = map;
            double angle = 0;
            if ( !IOUtil.InvariantParse(Program.frmMainInstance.txtAutoCliffSlope.Text, ref angle) )
            {
                return;
            }
            applyCliff.Angle = MathUtil.ClampDbl(angle * MathUtil.RadOf1Deg, 0.0D, MathUtil.RadOf90Deg);
            applyCliff.SetTris = Program.frmMainInstance.cbxCliffTris.Checked;
            App.CliffBrush.PerformActionMapTiles(applyCliff, mouseOverTerrain.Tile);

            map.Update();

            MainMapView.Draw();
        }

        public void ApplyCliffRemove()
        {
            var mouseOverTerrain = GetMouseOverTerrain();

            if ( mouseOverTerrain == null )
            {
                return;
            }

            var applyCliffRemove = new clsApplyCliffRemove {Map = map};
            App.CliffBrush.PerformActionMapTiles(applyCliffRemove, mouseOverTerrain.Tile);

            map.Update();

            MainMapView.Draw();
        }

        public void ApplyRoadRemove()
        {
            var mouseOverTerrain = GetMouseOverTerrain();

            if ( mouseOverTerrain == null )
            {
                return;
            }

            var applyRoadRemove = new clsApplyRoadRemove {Map = map};
            App.CliffBrush.PerformActionMapTiles(applyRoadRemove, mouseOverTerrain.Tile);

            map.Update();

            MainMapView.Draw();
        }

        public void ApplyTextureClockwise()
        {
            var mouseOverTerrain = GetMouseOverTerrain();

            if ( mouseOverTerrain == null )
            {
                return;
            }

            var tile = mouseOverTerrain.Tile.Normal;

            map.Terrain.Tiles[tile.X, tile.Y].Texture.Orientation.RotateClockwise();
            map.TileTextureChangeTerrainAction(tile, Program.frmMainInstance.TextureTerrainAction);

            map.SectorGraphicsChanges.TileChanged(tile);
            map.SectorTerrainUndoChanges.TileChanged(tile);

            map.Update();

            map.UndoStepCreate("Texture Rotate");

            MainMapView.Draw();
        }

        public void ApplyTextureCounterClockwise()
        {
            var mouseOverTerrain = GetMouseOverTerrain();

            if ( mouseOverTerrain == null )
            {
                return;
            }

            var tile = mouseOverTerrain.Tile.Normal;

            map.Terrain.Tiles[tile.X, tile.Y].Texture.Orientation.RotateAntiClockwise();
            map.TileTextureChangeTerrainAction(tile, Program.frmMainInstance.TextureTerrainAction);

            map.SectorGraphicsChanges.TileChanged(tile);
            map.SectorTerrainUndoChanges.TileChanged(tile);

            map.Update();

            map.UndoStepCreate("Texture Rotate");

            MainMapView.Draw();
        }

        public void ApplyTextureFlipX()
        {
            var mouseOverTerrain = GetMouseOverTerrain();

            if ( mouseOverTerrain == null )
            {
                return;
            }

            var tile = mouseOverTerrain.Tile.Normal;

            map.Terrain.Tiles[tile.X, tile.Y].Texture.Orientation.XFlip = !map.Terrain.Tiles[tile.X, tile.Y].Texture.Orientation.XFlip;
            map.TileTextureChangeTerrainAction(tile, Program.frmMainInstance.TextureTerrainAction);

            map.SectorGraphicsChanges.TileChanged(tile);
            map.SectorTerrainUndoChanges.TileChanged(tile);

            map.Update();

            map.UndoStepCreate("Texture Rotate");

            MainMapView.Draw();
        }

        public void ApplyTriFlip()
        {
            var mouseOverTerrain = GetMouseOverTerrain();

            if ( mouseOverTerrain == null )
            {
                return;
            }

            var tile = mouseOverTerrain.Tile.Normal;

            map.Terrain.Tiles[tile.X, tile.Y].Tri = !map.Terrain.Tiles[tile.X, tile.Y].Tri;

            map.SectorGraphicsChanges.TileChanged(tile);
            map.SectorTerrainUndoChanges.TileChanged(tile);

            map.Update();

            map.UndoStepCreate("Triangle Flip");

            MainMapView.Draw();
        }

        public void ApplyHeightSmoothing(double ratio)
        {
            var mouseOverTerrain = GetMouseOverTerrain();

            if ( mouseOverTerrain == null )
            {
                return;
            }

            var applyHeightSmoothing = new clsApplyHeightSmoothing();
            applyHeightSmoothing.Map = map;
            applyHeightSmoothing.Ratio = ratio;
            var radius = (int)(Math.Ceiling(App.HeightBrush.Radius));
            var posNum = App.HeightBrush.GetPosNum(mouseOverTerrain.Vertex);
            applyHeightSmoothing.Offset.X = MathUtil.ClampInt(posNum.X - radius, 0, map.Terrain.TileSize.X);
            applyHeightSmoothing.Offset.Y = MathUtil.ClampInt(posNum.Y - radius, 0, map.Terrain.TileSize.Y);
            var posEnd = new XYInt
                {
                    X = MathUtil.ClampInt(posNum.X + radius, 0, map.Terrain.TileSize.X),
                    Y = MathUtil.ClampInt(posNum.Y + radius, 0, map.Terrain.TileSize.Y)
                };
            applyHeightSmoothing.AreaTileSize.X = posEnd.X - applyHeightSmoothing.Offset.X;
            applyHeightSmoothing.AreaTileSize.Y = posEnd.Y - applyHeightSmoothing.Offset.Y;
            applyHeightSmoothing.Start();
            App.HeightBrush.PerformActionMapVertices(applyHeightSmoothing, mouseOverTerrain.Vertex);
            applyHeightSmoothing.Finish();

            map.Update();

            MainMapView.Draw();
        }

        public void ApplyHeightChange(double rate)
        {
            var mouseOverTerrain = GetMouseOverTerrain();

            if ( mouseOverTerrain == null )
            {
                return;
            }

            var applyHeightChange = new clsApplyHeightChange();
            applyHeightChange.Map = map;
            applyHeightChange.Rate = rate;
            applyHeightChange.UseEffect = Program.frmMainInstance.cbxHeightChangeFade.Checked;
            App.HeightBrush.PerformActionMapVertices(applyHeightChange, mouseOverTerrain.Vertex);

            map.Update();

            MainMapView.Draw();
        }

        public void ApplyHeightSet(clsBrush brush, byte height)
        {
            var mouseOverTerrain = GetMouseOverTerrain();

            if ( mouseOverTerrain == null )
            {
                return;
            }

            var applyHeightSet = new clsApplyHeightSet
                {
                    Map = map,
                    Height = height
                };
            brush.PerformActionMapVertices(applyHeightSet, mouseOverTerrain.Vertex);

            map.Update();

            MainMapView.Draw();
        }

        public void ApplyGateway()
        {
            var mouseOverTerrain = GetMouseOverTerrain();

            if ( mouseOverTerrain == null )
            {
                return;
            }

            var tile = mouseOverTerrain.Tile.Normal;

            if ( KeyboardManager.ActiveKey.Name == KeyboardKeys.GatewayDelete )
            {
                var a = 0;
                var low = new XYInt();
                var high = new XYInt();
                a = 0;
                while ( a < map.Gateways.Count )
                {
                    MathUtil.ReorderXY(map.Gateways[a].PosA, map.Gateways[a].PosB, ref low, ref high);
                    if ( low.X <= tile.X
                         & high.X >= tile.X
                         & low.Y <= tile.Y
                         & high.Y >= tile.Y )
                    {
                        map.GatewayRemoveStoreChange(a);
                        map.UndoStepCreate("Gateway Delete");
                        map.MinimapMakeLater();
                        MainMapView.Draw();
                        break;
                    }
                    a++;
                }
            }
            else
            {
                if ( map.SelectedTileA == null )
                {
                    map.SelectedTileA = tile;
                    MainMapView.Draw();
                }
                else if ( tile.X == map.SelectedTileA.X | tile.Y == map.SelectedTileA.Y )
                {
                    if ( map.GatewayCreateStoreChange(map.SelectedTileA, tile) != null )
                    {
                        map.UndoStepCreate("Gateway Place");
                        map.SelectedTileA = null;
                        map.SelectedTileB = null;
                        map.MinimapMakeLater();
                        MainMapView.Draw();
                    }
                }
            }
        }

        public void MouseDown(MouseEventArgs e)
        {
            var screenPos = new XYInt();

            map.SuppressMinimap = true;

            screenPos.X = e.X;
            screenPos.Y = e.Y;
            if ( e.Button == MouseButtons.Left )
            {
                MouseLeftDown = new clsMouseDown();
                if ( IsViewPosOverMinimap(screenPos) )
                {
                    MouseLeftDown.OverMinimap = new clsMouseDown.clsOverMinimap
                        {
                            DownPos = screenPos
                        };
                    var pos = new XYInt((int)((screenPos.X * TilesPerMinimapPixel)),
                        (int)(screenPos.Y * TilesPerMinimapPixel));
                    map.TileNumClampToMap(pos);
                    LookAtTile(pos);
                }
                else
                {
                    var mouseOverTerrain = GetMouseOverTerrain();
                    if ( mouseOverTerrain != null )
                    {
                        MouseLeftDown.OverTerrain = new clsMouseDown.clsOverTerrain
                            {
                                DownPos = mouseOverTerrain.Pos
                            };
                        if ( modTools.Tool == modTools.Tools.ObjectSelect )
                        {
                            if ( KeyboardManager.ActiveKey.Name == KeyboardKeys.Picker )
                            {
                                if ( mouseOverTerrain.Units.Count > 0 )
                                {
                                    if ( mouseOverTerrain.Units.Count == 1 )
                                    {
                                        Program.frmMainInstance.ObjectPicker(mouseOverTerrain.Units[0].TypeBase);
                                    }
                                    else
                                    {
                                        MapViewControl.ListSelectBegin(true);
                                    }
                                }
                            }
                            else if ( KeyboardManager.ActiveKey.Name == KeyboardKeys.PositionLabel )
                            {
                                var newPosition = new clsScriptPosition(Map)
                                    {
                                        PosX = MouseLeftDown.OverTerrain.DownPos.Horizontal.X,
                                        PosY = MouseLeftDown.OverTerrain.DownPos.Horizontal.Y
                                    };
                                Program.frmMainInstance.ScriptMarkerLists_Update();
                            }
                            else
                            {
                                if ( !KeyboardManager.ActiveKey.Name == KeyboardKeys.Multiselect )
                                {
                                    Map.SelectedUnits.Clear();
                                }
                                Program.frmMainInstance.SelectedObject_Changed();
                                Map.UnitSelectedAreaVertexA = mouseOverTerrain.Vertex.Normal;
                                MapViewControl.DrawViewLater();
                            }
                        }
                        else if ( modTools.Tool == modTools.Tools.TerrainBrush )
                        {
                            if ( Map.Tileset != null )
                            {
                                if ( KeyboardManager.ActiveKey.Name == KeyboardKeys.Picker )
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
                            if ( KeyboardManager.ActiveKey.Name == KeyboardKeys.Picker )
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
                                if ( KeyboardManager.ActiveKey.Name == KeyboardKeys.Picker )
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
                            if ( map.Tileset != null )
                            {
                                if ( KeyboardManager.ActiveKey.Name == KeyboardKeys.Picker )
                                {
                                    Program.frmMainInstance.TerrainPicker();
                                }
                                else
                                {
                                    ApplyTerrainFill(Program.frmMainInstance.FillCliffAction, Program.frmMainInstance.cbxFillInside.Checked);
                                    MainMapView.Draw();
                                }
                            }
                        }
                        else if ( modTools.Tool == modTools.Tools.RoadPlace )
                        {
                            if ( map.Tileset != null )
                            {
                                ApplyRoad();
                            }
                        }
                        else if ( modTools.Tool == modTools.Tools.RoadLines )
                        {
                            if ( map.Tileset != null )
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
                            if ( Program.frmMainInstance.SingleSelectedObjectTypeBase != null && map.SelectedUnitGroup != null )
                            {
                                var objectCreator = new clsUnitCreate();
                                map.SetObjectCreatorDefaults(objectCreator);
                                objectCreator.Horizontal = mouseOverTerrain.Pos.Horizontal;
                                objectCreator.Perform();
                                map.UndoStepCreate("Place Object");
                                map.Update();
                                map.MinimapMakeLater();
                                MainMapView.Draw();
                            }
                        }
                        else if ( modTools.Tool == modTools.Tools.ObjectLines )
                        {
                            ApplyObjectLine();
                        }
                        else if ( modTools.Tool == modTools.Tools.TerrainSelect )
                        {
                            if ( map.SelectedAreaVertexA == null )
                            {
                                map.SelectedAreaVertexA = mouseOverTerrain.Vertex.Normal;
                                MainMapView.Draw();
                            }
                            else if ( map.SelectedAreaVertexB == null )
                            {
                                map.SelectedAreaVertexB = mouseOverTerrain.Vertex.Normal;
                                MainMapView.Draw();
                            }
                            else
                            {
                                map.SelectedAreaVertexA = null;
                                map.SelectedAreaVertexB = null;
                                MainMapView.Draw();
                            }
                        }
                        else if ( modTools.Tool == modTools.Tools.Gateways )
                        {
                            ApplyGateway();
                        }
                    }
                    else if ( modTools.Tool == modTools.Tools.ObjectSelect )
                    {
                        map.SelectedUnits.Clear();
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
                    var mouseOverTerrain = GetMouseOverTerrain();
                    if ( mouseOverTerrain != null )
                    {
                        MouseRightDown.OverTerrain = new clsMouseDown.clsOverTerrain();
                        MouseRightDown.OverTerrain.DownPos = mouseOverTerrain.Pos;
                    }
                }
                if ( modTools.Tool == modTools.Tools.RoadLines || modTools.Tool == modTools.Tools.ObjectLines )
                {
                    map.SelectedTileA = null;
                    MainMapView.Draw();
                }
                else if ( modTools.Tool == modTools.Tools.TerrainSelect )
                {
                    map.SelectedAreaVertexA = null;
                    map.SelectedAreaVertexB = null;
                    MainMapView.Draw();
                }
                else if ( modTools.Tool == modTools.Tools.CliffTriangle )
                {
                    ApplyCliffTriangle(true);
                }
                else if ( modTools.Tool == modTools.Tools.Gateways )
                {
                    map.SelectedTileA = null;
                    map.SelectedTileB = null;
                    MainMapView.Draw();
                }
                else if ( modTools.Tool == modTools.Tools.HeightSetBrush )
                {
                    if ( KeyboardManager.ActiveKey.Name == KeyboardKeys.Picker )
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

        public void TimedActions(double zoom, double move, double pan, double roll, double orbitRate)
        {
            var xyzDbl = new XYZDouble();
            var panRate = pan * FieldOfViewY;
            var anglePY = default(Angles.AnglePY);
            var matrixA = new Matrix3DMath.Matrix3D();
            var matrixB = new Matrix3DMath.Matrix3D();
            var viewAngleChange = default(XYZDouble);
            var viewPosChangeXyz = new XYZInt(0, 0, 0);
            var angleChanged = default(bool);

            move *= FOVMultiplier * (MainMapView.GLSurface.Size.Width + MainMapView.GLSurface.Size.Height) * Math.Max(Math.Abs(ViewPos.Y), 512.0D);

            if ( KeyboardManager.ActiveKey.Name == KeyboardKeys.ViewZoomIn )
            {
                FovScale_2EChange(Convert.ToDouble(- zoom));
            }
            if ( KeyboardManager.ActiveKey.Name == KeyboardKeys.ViewZoomOut )
            {
                FovScale_2EChange(zoom);
            }

            if ( App.ViewMoveType == ViewMoveType.Free )
            {
                viewPosChangeXyz.X = 0;
                viewPosChangeXyz.Y = 0;
                viewPosChangeXyz.Z = 0;
                if ( KeyboardManager.ActiveKey.Name == KeyboardKeys.ViewMoveForwards )
                {
                    Matrix3DMath.VectorForwardsRotationByMatrix(ViewAngleMatrix, move, ref xyzDbl);
                    viewPosChangeXyz.AddDbl(xyzDbl);
                }
                if ( KeyboardManager.ActiveKey.Name == KeyboardKeys.ViewMoveBackwards )
                {
                    Matrix3DMath.VectorBackwardsRotationByMatrix(ViewAngleMatrix, move, ref xyzDbl);
                    viewPosChangeXyz.AddDbl(xyzDbl);
                }
                if ( KeyboardManager.ActiveKey.Name == KeyboardKeys.ViewMoveLeft )
                {
                    Matrix3DMath.VectorLeftRotationByMatrix(ViewAngleMatrix, move, ref xyzDbl);
                    viewPosChangeXyz.AddDbl(xyzDbl);
                }
                if ( KeyboardManager.ActiveKey.Name == KeyboardKeys.ViewMoveRight )
                {
                    Matrix3DMath.VectorRightRotationByMatrix(ViewAngleMatrix, move, ref xyzDbl);
                    viewPosChangeXyz.AddDbl(xyzDbl);
                }
                if ( KeyboardManager.ActiveKey.Name == KeyboardKeys.ViewMoveUp )
                {
                    Matrix3DMath.VectorUpRotationByMatrix(ViewAngleMatrix, move, ref xyzDbl);
                    viewPosChangeXyz.AddDbl(xyzDbl);
                }
                if ( KeyboardManager.ActiveKey.Name == KeyboardKeys.ViewMoveDown )
                {
                    Matrix3DMath.VectorDownRotationByMatrix(ViewAngleMatrix, move, ref xyzDbl);
                    viewPosChangeXyz.AddDbl(xyzDbl);
                }

                viewAngleChange.X = 0.0D;
                viewAngleChange.Y = 0.0D;
                viewAngleChange.Z = 0.0D;
                if ( KeyboardManager.ActiveKey.Name == KeyboardKeys.ViewRotateLeft )
                {
                    Matrix3DMath.VectorForwardsRotationByMatrix(ViewAngleMatrix, roll, ref xyzDbl);
                    viewAngleChange += xyzDbl;
                }
                if ( KeyboardManager.ActiveKey.Name == KeyboardKeys.ViewRotateRight )
                {
                    Matrix3DMath.VectorBackwardsRotationByMatrix(ViewAngleMatrix, roll, ref xyzDbl);
                    viewAngleChange += xyzDbl;
                }
                if ( KeyboardManager.ActiveKey.Name == KeyboardKeys.ViewRotateBackwards )
                {
                    Matrix3DMath.VectorLeftRotationByMatrix(ViewAngleMatrix, panRate, ref xyzDbl);
                    viewAngleChange += xyzDbl;
                }
                if ( KeyboardManager.ActiveKey.Name == KeyboardKeys.ViewRotateForwards )
                {
                    Matrix3DMath.VectorRightRotationByMatrix(ViewAngleMatrix, panRate, ref xyzDbl);
                    viewAngleChange += xyzDbl;
                }
                if ( KeyboardManager.ActiveKey.Name == KeyboardKeys.ViewRollLeft )
                {
                    Matrix3DMath.VectorDownRotationByMatrix(ViewAngleMatrix, panRate, ref xyzDbl);
                    viewAngleChange += xyzDbl;
                }
                if ( KeyboardManager.ActiveKey.Name == KeyboardKeys.ViewRollRight )
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
                if ( KeyboardManager.ActiveKey.Name == KeyboardKeys.ViewMoveForwards )
                {
                    Matrix3DMath.VectorForwardsRotationByMatrix(matrixA, move, ref xyzDbl);
                    viewPosChangeXyz.AddDbl(xyzDbl);
                }
                if ( KeyboardManager.ActiveKey.Name == KeyboardKeys.ViewMoveBackwards )
                {
                    Matrix3DMath.VectorBackwardsRotationByMatrix(matrixA, move, ref xyzDbl);
                    viewPosChangeXyz.AddDbl(xyzDbl);
                }
                if ( KeyboardManager.ActiveKey.Name == KeyboardKeys.ViewMoveLeft )
                {
                    Matrix3DMath.VectorLeftRotationByMatrix(matrixA, move, ref xyzDbl);
                    viewPosChangeXyz.AddDbl(xyzDbl);
                }
                if ( KeyboardManager.ActiveKey.Name == KeyboardKeys.ViewMoveRight )
                {
                    Matrix3DMath.VectorRightRotationByMatrix(matrixA, move, ref xyzDbl);
                    viewPosChangeXyz.AddDbl(xyzDbl);
                }
                if ( KeyboardManager.ActiveKey.Name == KeyboardKeys.ViewMoveUp )
                {
                    viewPosChangeXyz.Y += (int)move;
                }
                if ( KeyboardManager.ActiveKey.Name == KeyboardKeys.ViewMoveDown )
                {
                    viewPosChangeXyz.Y -= (int)move;
                }

                if ( App.RTSOrbit )
                {
                    if ( KeyboardManager.ActiveKey.Name == KeyboardKeys.ViewRotateForwards )
                    {
                        anglePY.Pitch = MathUtil.ClampDbl(anglePY.Pitch + orbitRate, Convert.ToDouble(- MathUtil.RadOf90Deg + 0.03125D * MathUtil.RadOf1Deg),
                            MathUtil.RadOf90Deg - 0.03125D * MathUtil.RadOf1Deg);
                        angleChanged = true;
                    }
                    if ( KeyboardManager.ActiveKey.Name == KeyboardKeys.ViewRotateBackwards )
                    {
                        anglePY.Pitch = MathUtil.ClampDbl(anglePY.Pitch - orbitRate, Convert.ToDouble(- MathUtil.RadOf90Deg + 0.03125D * MathUtil.RadOf1Deg),
                            MathUtil.RadOf90Deg - 0.03125D * MathUtil.RadOf1Deg);
                        angleChanged = true;
                    }
                    if ( KeyboardManager.ActiveKey.Name == KeyboardKeys.ViewRotateLeft )
                    {
                        anglePY.Yaw = MathUtil.AngleClamp(anglePY.Yaw + orbitRate);
                        angleChanged = true;
                    }
                    if ( KeyboardManager.ActiveKey.Name == KeyboardKeys.ViewRotateRight )
                    {
                        anglePY.Yaw = MathUtil.AngleClamp(anglePY.Yaw - orbitRate);
                        angleChanged = true;
                    }
                }
                else
                {
                    if ( KeyboardManager.ActiveKey.Name == KeyboardKeys.ViewRotateForwards )
                    {
                        anglePY.Pitch = MathUtil.ClampDbl(anglePY.Pitch - orbitRate, Convert.ToDouble(- MathUtil.RadOf90Deg + 0.03125D * MathUtil.RadOf1Deg),
                            MathUtil.RadOf90Deg - 0.03125D * MathUtil.RadOf1Deg);
                        angleChanged = true;
                    }
                    if ( KeyboardManager.ActiveKey.Name == KeyboardKeys.ViewRotateBackwards )
                    {
                        anglePY.Pitch = MathUtil.ClampDbl(anglePY.Pitch + orbitRate, Convert.ToDouble(- MathUtil.RadOf90Deg + 0.03125D * MathUtil.RadOf1Deg),
                            MathUtil.RadOf90Deg - 0.03125D * MathUtil.RadOf1Deg);
                        angleChanged = true;
                    }
                    if ( KeyboardManager.ActiveKey.Name == KeyboardKeys.ViewRotateLeft )
                    {
                        anglePY.Yaw = MathUtil.AngleClamp(anglePY.Yaw - orbitRate);
                        angleChanged = true;
                    }
                    if ( KeyboardManager.ActiveKey.Name == KeyboardKeys.ViewRotateRight )
                    {
                        anglePY.Yaw = MathUtil.AngleClamp(anglePY.Yaw + orbitRate);
                        angleChanged = true;
                    }
                }

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
            if ( Program.frmMainInstance.SingleSelectedObjectTypeBase != null && map.SelectedUnitGroup != null )
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

                if ( map.SelectedTileA != null )
                {
                    if ( tile.X == map.SelectedTileA.X )
                    {
                        if ( tile.Y <= map.SelectedTileA.Y )
                        {
                            a = tile.Y;
                            b = map.SelectedTileA.Y;
                        }
                        else
                        {
                            a = map.SelectedTileA.Y;
                            b = tile.Y;
                        }
                        var objectCreator = new clsUnitCreate();
                        map.SetObjectCreatorDefaults(objectCreator);
                        for ( num = a; num <= b; num++ )
                        {
                            objectCreator.Horizontal.X = (int)((tile.X + 0.5D) * Constants.TerrainGridSpacing);
                            objectCreator.Horizontal.Y = (int)((num + 0.5D) * Constants.TerrainGridSpacing);
                            objectCreator.Perform();
                        }

                        map.UndoStepCreate("Object Line");
                        map.Update();
                        map.MinimapMakeLater();
                        map.SelectedTileA = null;
                        MainMapView.Draw();
                    }
                    else if ( tile.Y == map.SelectedTileA.Y )
                    {
                        if ( tile.X <= map.SelectedTileA.X )
                        {
                            a = tile.X;
                            b = map.SelectedTileA.X;
                        }
                        else
                        {
                            a = map.SelectedTileA.X;
                            b = tile.X;
                        }
                        var objectCreator = new clsUnitCreate();
                        map.SetObjectCreatorDefaults(objectCreator);
                        for ( num = a; num <= b; num++ )
                        {
                            objectCreator.Horizontal.X = (int)((num + 0.5D) * Constants.TerrainGridSpacing);
                            objectCreator.Horizontal.Y = (int)((tile.Y + 0.5D) * Constants.TerrainGridSpacing);
                            objectCreator.Perform();
                        }

                        map.UndoStepCreate("Object Line");
                        map.Update();
                        map.MinimapMakeLater();
                        map.SelectedTileA = null;
                        MainMapView.Draw();
                    }
                }
                else
                {
                    map.SelectedTileA = tile;
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
                public bool SideIsV;
                public XYInt SideNum;
                public clsBrush.sPosNum Tile;
                public bool Triangle;
                public SimpleClassList<Unit> Units = new SimpleClassList<Unit>();
                public clsBrush.sPosNum Vertex;
            }
        }
    }
}