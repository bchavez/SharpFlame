using System;
using Appccelerate.EventBroker;
using Appccelerate.EventBroker.Handlers;
using Appccelerate.Events;
using Eto.Forms;
using Eto.Gl;
using NLog;
using SharpFlame.Core;
using SharpFlame.Core.Collections;
using SharpFlame.Core.Domain;
using SharpFlame.Core.Extensions;
using SharpFlame.FileIO;
using SharpFlame.Gui.Sections;
using SharpFlame.Mapping;
using SharpFlame.Mapping.Objects;
using SharpFlame.Mapping.Script;
using SharpFlame.Mapping.Tiles;
using SharpFlame.Mapping.Tools;
using SharpFlame.Maths;
using SharpFlame.Util;
using SharpFlame.Settings;
using SharpFlame.UiOptions;



namespace SharpFlame
{
    /// <summary>
    /// Seems like this is the map user control class.
    /// </summary>
    public class ViewInfo
    {
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
        public Map Map
        {
            get { return map; }
        }

        [EventSubscription(EventTopics.OnMapLoad, typeof(OnPublisher))]
        public void OnMapLoad(object sender, EventArgs<Map> args)
        {
            map = args.Value;
            ViewPos = new XYZInt(0, 3072, 0);
            FovMultiplierSet(settings.FOVDefault);
            ViewAngleSetToDefault();
            LookAtPos(new XYInt(
                (Map.Terrain.TileSize.X * Constants.TerrainGridSpacing / 2.0D).ToInt(),
                (Map.Terrain.TileSize.Y * Constants.TerrainGridSpacing / 2.0D).ToInt())
                );
            // Reset Mouse on map change.
            HandleLostFocus(this, EventArgs.Empty);
        }

        private GLSurface glSurface;
        private readonly KeyboardManager keyboardManager;
        private readonly Options uiOptions;
        private readonly SettingsManager settings;
        private readonly IEventBroker broker;
        private readonly UITimer tmrMouseMove;

        private bool enableMouseMove = false;

        public ViewInfo(KeyboardManager kbm, Options argUiOptions, SettingsManager argSettings, IEventBroker broker, GLSurface glSurface)
        {
            this.keyboardManager = kbm;
            this.uiOptions = argUiOptions;
            this.settings = argSettings;
            this.broker = broker;
            this.glSurface = glSurface;

            tmrMouseMove = new UITimer { Interval = 0.1 };
            tmrMouseMove.Elapsed += EnableMouseMove;
            tmrMouseMove.Start();
        }

        private void fovScale_2EChange(double powerChange)
        {
            FOVMultiplierExponent += powerChange;
            FOVMultiplier = Math.Pow(2.0D, FOVMultiplierExponent);

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

            FieldOfViewY = (float)(Math.Atan(glSurface.Size.Height * FOVMultiplier / 2.0D) * 2.0D);
            if ( FieldOfViewY < min )
            {
                FieldOfViewY = min;
                if ( glSurface.Size.Height > 0 )
                {
                    FOVMultiplier = 2.0D * Math.Tan(FieldOfViewY / 2.0D) / glSurface.Size.Height;
                    FOVMultiplierExponent = Math.Log(FOVMultiplier) / Math.Log(2.0D);
                }
            }
            else if ( FieldOfViewY > max )
            {
                FieldOfViewY = max;
                if ( glSurface.Size.Height > 0 )
                {
                    FOVMultiplier = 2.0D * Math.Tan(FieldOfViewY / 2.0D) / glSurface.Size.Height;
                    FOVMultiplierExponent = Math.Log(FOVMultiplier) / Math.Log(2.0D);
                }
            }

            broker.DrawLater(this);
        }

        private void ViewPosSet(XYZInt newViewPos)
        {
            ViewPos = newViewPos;
            ViewPosClamp();

            broker.DrawLater(this);
        }

        public void ViewPosChange(XYZInt displacement)
        {
            ViewPos.X += displacement.X;
            ViewPos.Z += displacement.Z;
            ViewPos.Y += displacement.Y;
            ViewPosClamp();

            broker.DrawLater(this);
        }

        private void ViewPosClamp()
        {
            const int maxHeight = 1048576;
            const int maxDist = 1048576;

            ViewPos.X = MathUtil.ClampInt(ViewPos.X, Convert.ToInt32(- maxDist), Map.Terrain.TileSize.X * Constants.TerrainGridSpacing + maxDist);
            ViewPos.Z = MathUtil.ClampInt(ViewPos.Z, - Map.Terrain.TileSize.Y * Constants.TerrainGridSpacing - maxDist, maxDist);
            ViewPos.Y = MathUtil.ClampInt(ViewPos.Y, (Convert.ToInt32(Math.Ceiling(Map.GetTerrainHeight(new XYInt(ViewPos.X, - ViewPos.Z))))) + 16, maxHeight);
        }

        private void viewAngleSet(Matrix3DMath.Matrix3D newMatrix)
        {
            Matrix3DMath.MatrixCopy(newMatrix, ViewAngleMatrix);
            Matrix3DMath.MatrixNormalize(ViewAngleMatrix);
            Matrix3DMath.MatrixInvert(ViewAngleMatrix, ViewAngleMatrixInverted);
            Matrix3DMath.MatrixToRPY(ViewAngleMatrix, ref ViewAngleRPY);

            broker.DrawLater(this);
        }

        private void ViewAngleSetToDefault()
        {
            var matrixA = new Matrix3DMath.Matrix3D();
            Matrix3DMath.MatrixSetToXAngle(matrixA, Math.Atan(2.0D));
            viewAngleSet(matrixA);

            broker.DrawLater(this);
        }

        public void ViewAngleSetRotate(Matrix3DMath.Matrix3D newMatrix)
        {
            var flag = false;
            XYZDouble xyzDbl = default(XYZDouble);
            XYDouble xyDbl = default(XYDouble);

            if ( App.ViewMoveType == ViewMoveType.RTS & App.RTSOrbit )
            {
                if ( ScreenXYGetViewPlanePosForwardDownOnly(Math.Floor(glSurface.Size.Width / 2.0D).ToInt(), Math.Floor(glSurface.Size.Height / 2.0D).ToInt(), 127.5D, ref xyDbl) )
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

            broker.DrawLater(this);
        }

        public void LookAtTile(XYInt tileNum)
        {
            var pos = new XYInt(0, 0);

            pos.X = Convert.ToInt32((tileNum.X + 0.5D) * Constants.TerrainGridSpacing);
            pos.Y = Convert.ToInt32((tileNum.Y + 0.5D) * Constants.TerrainGridSpacing);
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
            var i = (Convert.ToInt32(Math.Ceiling(dblTemp))) + 128;
            if ( ViewPos.Y < i )
            {
                ViewPos.Y = i;
            }
            if ( xyzDbl.Y > -0.33333333333333331D )
            {
                xyzDbl.Y = -0.33333333333333331D;
                Matrix3DMath.VectorToPY(xyzDbl, ref anglePy);
                Matrix3DMath.MatrixSetToPY(matrixA, anglePy);
                viewAngleSet(matrixA);
            }
            dblTemp = (ViewPos.Y - dblTemp) / xyzDbl.Y;

            xyzInt.X = Convert.ToInt32(horizontal.X + dblTemp * xyzDbl.X);
            xyzInt.Y = ViewPos.Y;
            xyzInt.Z = Convert.ToInt32(- horizontal.Y + dblTemp * xyzDbl.Z);

            ViewPosSet(xyzInt);
        }

        private void MoveToViewTerrainPosFromDistance(XYZDouble terrainPos, double distance)
        {
            var xyzDbl = default(XYZDouble);
            var xyzInt = new XYZInt(0, 0, 0);

            Matrix3DMath.VectorForwardsRotationByMatrix(ViewAngleMatrix, ref xyzDbl);

            xyzInt.X = Convert.ToInt32(terrainPos.X - xyzDbl.X * distance);
            xyzInt.Y = Convert.ToInt32(terrainPos.Y - xyzDbl.Y * distance);
            xyzInt.Z = Convert.ToInt32(- terrainPos.Z - xyzDbl.Z * distance);

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
                result.X = Convert.ToInt32(glSurface.Size.Width / 2.0D + (pos.X * ratioZpx));
                result.Y = Convert.ToInt32(glSurface.Size.Height / 2.0D - (pos.Y * ratioZpx));
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
                xyzDbl.X = (screenPos.X - glSurface.Size.Width / 2.0D) * FOVMultiplier;
                xyzDbl.Y = (glSurface.Size.Height / 2.0D - screenPos.Y) * FOVMultiplier;
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

        public bool ScreenXyGetTerrainPos(XYInt screenPos, ref WorldPos resultPos)
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
                xyzDbl.X = (screenPos.X - glSurface.Size.Width / 2.0D) * FOVMultiplier;
                xyzDbl.Y = ( glSurface.Size.Height / 2.0D - screenPos.Y ) * FOVMultiplier;
                xyzDbl.Z = 1.0D;
                //rotate the vector so that it points forward and level
                Matrix3DMath.VectorRotationByMatrix(ViewAngleMatrix, xyzDbl, ref terrainViewVector);
                terrainViewVector.Y = Convert.ToDouble(- terrainViewVector.Y); //get the amount of looking down, not up
                terrainViewVector.Z = Convert.ToDouble(- terrainViewVector.Z); //convert to terrain coordinates from view coordinates
                //get range of possible tiles
                double dblTemp = (terrainViewPos.Y - 255 * Map.HeightMultiplier) / terrainViewVector.Y;
                limitA.X = terrainViewPos.X + terrainViewVector.X * dblTemp;
                limitA.Y = terrainViewPos.Z + terrainViewVector.Z * dblTemp;
                dblTemp = terrainViewPos.Y / terrainViewVector.Y;
                limitB.X = terrainViewPos.X + terrainViewVector.X * dblTemp;
                limitB.Y = terrainViewPos.Z + terrainViewVector.Z * dblTemp;
                min.X = Math.Max(Convert.ToInt32( Math.Floor(Math.Min(limitA.X, limitB.X) / Constants.TerrainGridSpacing) ), 0);
                min.Y = Math.Max(Convert.ToInt32( Math.Floor(Math.Min(limitA.Y, limitB.Y) / Constants.TerrainGridSpacing)), 0);
                max.X = Math.Min(Convert.ToInt32( Math.Floor(Math.Max(limitA.X, limitB.X) / Constants.TerrainGridSpacing)), Map.Terrain.TileSize.X - 1);
                max.Y = Math.Min(Convert.ToInt32( Math.Floor(Math.Max(limitA.Y, limitB.Y) / Constants.TerrainGridSpacing)), Map.Terrain.TileSize.Y - 1);
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
                        if ( Map.Terrain.Tiles[x, y].Tri )
                        {
                            triHeightOffset = Convert.ToDouble(Map.Terrain.Vertices[x, y].Height * Map.HeightMultiplier);
                            triGradientX = Convert.ToDouble(Map.Terrain.Vertices[x + 1, y].Height * Map.HeightMultiplier - triHeightOffset);
                            triGradientZ = Convert.ToDouble(Map.Terrain.Vertices[x, y + 1].Height * Map.HeightMultiplier - triHeightOffset);
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

                            triHeightOffset = Convert.ToDouble(Map.Terrain.Vertices[x + 1, y + 1].Height * Map.HeightMultiplier);
                            triGradientX = Convert.ToDouble(Map.Terrain.Vertices[x, y + 1].Height * Map.HeightMultiplier - triHeightOffset);
                            triGradientZ = Convert.ToDouble(Map.Terrain.Vertices[x + 1, y].Height * Map.HeightMultiplier - triHeightOffset);
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
                            triHeightOffset = Convert.ToDouble(Map.Terrain.Vertices[x + 1, y].Height * Map.HeightMultiplier);
                            triGradientX = Convert.ToDouble(Map.Terrain.Vertices[x, y].Height * Map.HeightMultiplier - triHeightOffset);
                            triGradientZ = Convert.ToDouble(Map.Terrain.Vertices[x + 1, y + 1].Height * Map.HeightMultiplier - triHeightOffset);
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

                            triHeightOffset = Convert.ToDouble(Map.Terrain.Vertices[x, y + 1].Height * Map.HeightMultiplier);
                            triGradientX = Convert.ToDouble(Map.Terrain.Vertices[x + 1, y + 1].Height * Map.HeightMultiplier - triHeightOffset);
                            triGradientZ = Convert.ToDouble(Map.Terrain.Vertices[x, y].Height * Map.HeightMultiplier - triHeightOffset);
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

                if ( double.IsNaN(bestPos.X) )
                {
                    return false;
                }

                resultPos.Horizontal.X = Convert.ToInt32(bestPos.X);
                resultPos.Altitude = Convert.ToInt32(bestPos.Y);
                resultPos.Horizontal.Y = Convert.ToInt32(bestPos.Z);
                //logger.Info("ScreenPos [X {3}, Y {4}], WorldPos: X {0}, Y {1}, Z {2}", resultPos.Horizontal.X, resultPos.Horizontal.Y, resultPos.Altitude, screenPos.X, screenPos.Y);
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
                xyzDouble.X = (screenX - glSurface.Size.Width / 2.0D) * dblTemp2;
                xyzDouble.Y = (glSurface.Size.Height / 2.0D - screenY) * dblTemp2;
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

        public clsMouseOver.clsOverTerrain GetMouseOverTerrain()
        {
            if (Map == null || MouseOver == null)
            {
                return null;
            }
            return MouseOver.OverTerrain;
        }

        public clsMouseDown.clsOverTerrain GetMouseLeftDownOverTerrain()
        {
            if (Map == null || MouseLeftDown == null)
            {
                return null;
            }
            return MouseLeftDown.OverTerrain;
        }

        public clsMouseDown.clsOverTerrain GetMouseRightDownOverTerrain()
        {
            if (Map == null || MouseRightDown == null)
            {
                return null;
            }
            return MouseRightDown.OverTerrain;
        }

        public clsMouseDown.clsOverMinimap GetMouseLeftDownOverMinimap()
        {
            if (Map == null || MouseLeftDown == null)
            {
                return null;
            }
            return MouseLeftDown.OverMinimap;
        }

        public clsMouseDown.clsOverMinimap GetMouseRightDownOverMinimap()
        {
            if (Map == null ||  MouseRightDown == null)
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

        private void ApplyTerrain()
        {
            var mouseOverTerrain = GetMouseOverTerrain();

            if ( mouseOverTerrain == null )
            {
                return;
            }

            var applyVertexTerrain = new clsApplyVertexTerrain
                {
                    Map = Map, VertexTerrain = App.SelectedTerrain
                };
            uiOptions.Terrain.Brush.PerformActionMapVertices(applyVertexTerrain, mouseOverTerrain.Vertex);

            broker.UpdateMap(this);
            broker.DrawLater(this);
        }

        private void ApplyRoad()
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
                if ( Map.Terrain.SideV[sideNum.X, sideNum.Y].Road != App.SelectedRoad )
                {
                    Map.Terrain.SideV[sideNum.X, sideNum.Y].Road = App.SelectedRoad;

                    if ( sideNum.X > 0 )
                    {
                        tileNum.X = sideNum.X - 1;
                        tileNum.Y = sideNum.Y;
                        Map.AutoTextureChanges.TileChanged(tileNum);
                        Map.SectorGraphicsChanges.TileChanged(tileNum);
                        Map.SectorTerrainUndoChanges.TileChanged(tileNum);
                    }
                    if ( sideNum.X < Map.Terrain.TileSize.X )
                    {
                        tileNum = sideNum;
                        Map.AutoTextureChanges.TileChanged(tileNum);
                        Map.SectorGraphicsChanges.TileChanged(tileNum);
                        Map.SectorTerrainUndoChanges.TileChanged(tileNum);
                    }

                    broker.UpdateMap(this);

                    Map.UndoStepCreate("Road Side");

                    broker.DrawLater(this);
                }
            }
            else
            {
                if ( Map.Terrain.SideH[sideNum.X, sideNum.Y].Road != App.SelectedRoad )
                {
                    Map.Terrain.SideH[sideNum.X, sideNum.Y].Road = App.SelectedRoad;

                    if ( sideNum.Y > 0 )
                    {
                        tileNum.X = sideNum.X;
                        tileNum.Y = sideNum.Y - 1;
                        Map.AutoTextureChanges.TileChanged(tileNum);
                        Map.SectorGraphicsChanges.TileChanged(tileNum);
                        Map.SectorTerrainUndoChanges.TileChanged(tileNum);
                    }
                    if ( sideNum.Y < Map.Terrain.TileSize.X )
                    {
                        tileNum = sideNum;
                        Map.AutoTextureChanges.TileChanged(tileNum);
                        Map.SectorGraphicsChanges.TileChanged(tileNum);
                        Map.SectorTerrainUndoChanges.TileChanged(tileNum);
                    }

                    broker.UpdateMap(this);

                    Map.UndoStepCreate("Road Side");

                    broker.DrawLater(this);
                }
            }
        }

        private void ApplyRoadLineSelection()
        {
            var mouseOverTerrian = GetMouseOverTerrain();

            if ( mouseOverTerrian == null )
            {
                return;
            }

            var tile = mouseOverTerrian.Tile.Normal;
            var sideNum = new XYInt();

            if ( Map.SelectedTileA != null )
            {
                var num = 0;
                var a = 0;
                var b = 0;
                if ( tile.X == Map.SelectedTileA.X )
                {
                    if ( tile.Y <= Map.SelectedTileA.Y )
                    {
                        a = tile.Y;
                        b = Map.SelectedTileA.Y;
                    }
                    else
                    {
                        a = Map.SelectedTileA.Y;
                        b = tile.Y;
                    }
                    for ( num = a + 1; num <= b; num++ )
                    {
                        Map.Terrain.SideH[Map.SelectedTileA.X, num].Road = App.SelectedRoad;
                        sideNum.X = Map.SelectedTileA.X;
                        sideNum.Y = num;
                        Map.AutoTextureChanges.SideHChanged(sideNum);
                        Map.SectorGraphicsChanges.SideHChanged(sideNum);
                        Map.SectorTerrainUndoChanges.SideHChanged(sideNum);
                    }

                    broker.UpdateMap(this);

                    Map.UndoStepCreate("Road Line");

                    Map.SelectedTileA = null;

                    broker.DrawLater(this);
                }
                else if ( tile.Y == Map.SelectedTileA.Y )
                {
                    if ( tile.X <= Map.SelectedTileA.X )
                    {
                        a = tile.X;
                        b = Map.SelectedTileA.X;
                    }
                    else
                    {
                        a = Map.SelectedTileA.X;
                        b = tile.X;
                    }
                    for ( num = a + 1; num <= b; num++ )
                    {
                        Map.Terrain.SideV[num, Map.SelectedTileA.Y].Road = App.SelectedRoad;
                        sideNum.X = num;
                        sideNum.Y = Map.SelectedTileA.Y;
                        Map.AutoTextureChanges.SideVChanged(sideNum);
                        Map.SectorGraphicsChanges.SideVChanged(sideNum);
                        Map.SectorTerrainUndoChanges.SideVChanged(sideNum);
                    }

                    broker.UpdateMap(this);

                    Map.UndoStepCreate("Road Line");

                    Map.SelectedTileA = null;

                    broker.DrawLater(this);
                }
            }
            else
            {
                Map.SelectedTileA = tile;
            }
        }

        private void ApplyTerrainFill(FillCliffAction cliffAction, bool inside)
        {
            var mouseOverTerrain = GetMouseOverTerrain();

            if ( mouseOverTerrain == null )
            {
                return;
            }

            var startVertex = mouseOverTerrain.Vertex.Normal;

            var fillType = App.SelectedTerrain;
            var replaceType = Map.Terrain.Vertices[startVertex.X, startVertex.Y].Terrain;
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
                    stopForCliff = Map.VertexIsCliffEdge(currentSource);
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
                            if ( cliffAction == FillCliffAction.StopAfter )
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
                            if ( cliffAction == FillCliffAction.StopAfter )
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
                            if ( cliffAction == FillCliffAction.StopAfter )
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
                            if ( cliffAction == FillCliffAction.StopAfter )
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

            broker.UpdateMap(this);

            Map.UndoStepCreate("Ground Fill");

            broker.DrawLater(this);
        }

        private void ApplyTexture()
        {
            var mouseOverTerrain = GetMouseOverTerrain();

            if ( mouseOverTerrain == null )
            {
                return;
            }

            var applyTexture = new clsApplyTexture
            {
                Map = Map,
                TextureNum = uiOptions.Textures.SelectedTile,
                SetTexture = uiOptions.Textures.SetTexture,
                Orientation = RelativeToViewAngle(uiOptions.Textures.TextureOrientation),
                RandomOrientation = uiOptions.Textures.Randomize,
                SetOrientation = uiOptions.Textures.SetOrientation,
                TerrainAction = uiOptions.Textures.TerrainMode
            };

            uiOptions.Textures.Brush.PerformActionMapTiles(applyTexture, mouseOverTerrain.Tile);

            broker.UpdateMap(this);
            broker.DrawLater(this);
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
                        Map = Map, 
                        PosNum = mouseOverTerrain.Tile.Normal,
                        Triangle = mouseOverTerrain.Triangle
                    };
                applyCliffTriangleRemove.ActionPerform();
            }
            else
            {
                var applyCliffTriangle = new clsApplyCliffTriangle
                    {
                        Map = Map, 
                        PosNum = mouseOverTerrain.Tile.Normal,
                        Triangle = mouseOverTerrain.Triangle
                    };
                applyCliffTriangle.ActionPerform();
            }

            broker.UpdateMap(this);
            broker.DrawLater(this);
        }

        private void ApplyCliff()
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
            uiOptions.Terrain.CliffBrush.PerformActionMapTiles(applyCliff, mouseOverTerrain.Tile);

            broker.UpdateMap(this);
            broker.DrawLater(this);
        }

        private void ApplyCliffRemove()
        {
            var mouseOverTerrain = GetMouseOverTerrain();

            if ( mouseOverTerrain == null )
            {
                return;
            }

            var applyCliffRemove = new clsApplyCliffRemove {Map = Map};
            uiOptions.Terrain.CliffBrush.PerformActionMapTiles(applyCliffRemove, mouseOverTerrain.Tile);

            broker.UpdateMap(this);
            broker.DrawLater(this);
        }

        private void applyRoadRemove()
        {
            var mouseOverTerrain = GetMouseOverTerrain();

            if ( mouseOverTerrain == null )
            {
                return;
            }

            var applyRoadRemove = new clsApplyRoadRemove {Map = Map};
            uiOptions.Terrain.CliffBrush.PerformActionMapTiles(applyRoadRemove, mouseOverTerrain.Tile);

            broker.UpdateMap(this);
            broker.DrawLater(this);
        }

        public void ApplyTextureClockwise()
        {
            var mouseOverTerrain = GetMouseOverTerrain();

            if ( mouseOverTerrain == null )
            {
                return;
            }

            var tile = mouseOverTerrain.Tile.Normal;

            Map.Terrain.Tiles[tile.X, tile.Y].Texture.Orientation.RotateClockwise();
            Map.TileTextureChangeTerrainAction(tile, uiOptions.Textures.TerrainMode);

            Map.SectorGraphicsChanges.TileChanged(tile);
            Map.SectorTerrainUndoChanges.TileChanged(tile);

            broker.UpdateMap(this);

            Map.UndoStepCreate("Texture Rotate");

            broker.DrawLater(this);
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
            Map.TileTextureChangeTerrainAction(tile, uiOptions.Textures.TerrainMode);

            Map.SectorGraphicsChanges.TileChanged(tile);
            Map.SectorTerrainUndoChanges.TileChanged(tile);

            broker.UpdateMap(this);

            Map.UndoStepCreate("Texture Rotate");

            broker.DrawLater(this);
        }

        public void ApplyTextureFlipX()
        {
            var mouseOverTerrain = GetMouseOverTerrain();

            if ( mouseOverTerrain == null )
            {
                return;
            }

            var tile = mouseOverTerrain.Tile.Normal;

            Map.Terrain.Tiles[tile.X, tile.Y].Texture.Orientation.XFlip = !Map.Terrain.Tiles[tile.X, tile.Y].Texture.Orientation.XFlip;
            Map.TileTextureChangeTerrainAction(tile, uiOptions.Textures.TerrainMode);

            Map.SectorGraphicsChanges.TileChanged(tile);
            Map.SectorTerrainUndoChanges.TileChanged(tile);

            broker.UpdateMap(this);

            Map.UndoStepCreate("Texture Rotate");

            broker.DrawLater(this);
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

            broker.UpdateMap(this);

            Map.UndoStepCreate("Triangle Flip");

            broker.DrawLater(this);
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
            var radius = Convert.ToInt32(Math.Ceiling(uiOptions.Height.Brush.Radius));
            var posNum = uiOptions.Height.Brush.GetPosNum(mouseOverTerrain.Vertex);
            applyHeightSmoothing.Offset.X = MathUtil.ClampInt(posNum.X - radius, 0, Map.Terrain.TileSize.X);
            applyHeightSmoothing.Offset.Y = MathUtil.ClampInt(posNum.Y - radius, 0, Map.Terrain.TileSize.Y);
            var posEnd = new XYInt
                {
                    X = MathUtil.ClampInt(posNum.X + radius, 0, Map.Terrain.TileSize.X),
                    Y = MathUtil.ClampInt(posNum.Y + radius, 0, Map.Terrain.TileSize.Y)
                };
            applyHeightSmoothing.AreaTileSize.X = posEnd.X - applyHeightSmoothing.Offset.X;
            applyHeightSmoothing.AreaTileSize.Y = posEnd.Y - applyHeightSmoothing.Offset.Y;
            applyHeightSmoothing.Start();
            uiOptions.Height.Brush.PerformActionMapVertices(applyHeightSmoothing, mouseOverTerrain.Vertex);
            applyHeightSmoothing.Finish();

            broker.UpdateMap(this);
            broker.DrawLater(this);
        }

        private void applyHeightChange(double rate)
        {
            var mouseOverTerrain = GetMouseOverTerrain();

            if ( mouseOverTerrain == null )
            {
                return;
            }

            var applyHeightChange = new clsApplyHeightChange();
            applyHeightChange.Map = Map;
            applyHeightChange.Rate = rate;
            applyHeightChange.UseEffect = uiOptions.Height.ChangeFade;
            uiOptions.Height.Brush.PerformActionMapVertices(applyHeightChange, mouseOverTerrain.Vertex);

            broker.UpdateMap(this);
            broker.DrawLater(this);
        }

        private void applyHeightSet(clsBrush brush, byte height)
        {
            var mouseOverTerrain = GetMouseOverTerrain();

            if ( mouseOverTerrain == null )
            {
                return;
            }

            var applyHeightSet = new clsApplyHeightSet
                {
                    Map = Map,
                    Height = height
                };
            brush.PerformActionMapVertices(applyHeightSet, mouseOverTerrain.Vertex);

            broker.UpdateMap(this);
            broker.DrawLater(this);
        }

        private void applyGateway()
        {
            var mouseOverTerrain = GetMouseOverTerrain();

            if ( mouseOverTerrain == null )
            {
                return;
            }

            var tile = mouseOverTerrain.Tile.Normal;

            if (keyboardManager.Keys[KeyboardKeys.GatewayDelete].Active)
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
                        
                        broker.RefreshMinimap(this);
                        broker.DrawLater(this);
                        break;
                    }
                    a++;
                }
            }
            else
            {
                if ( Map.SelectedTileA == null )
                {
                    Map.SelectedTileA = tile;
                    broker.DrawLater(this);
                }
                else if ( tile.X == Map.SelectedTileA.X | tile.Y == Map.SelectedTileA.Y )
                {
                    if ( Map.GatewayCreateStoreChange(Map.SelectedTileA, tile) != null )
                    {
                        Map.UndoStepCreate("Gateway Place");
                        Map.SelectedTileA = null;
                        Map.SelectedTileB = null;
                        broker.RefreshMinimap(this);
                        broker.DrawLater(this);
                    }
                }
            }
        }

        public void EnableMouseMove(object sender, EventArgs e)
        {
            if(Map == null)
            {
                return;
            }

            enableMouseMove = true;
        }

        public static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public void HandleMouseMove(object sender, MouseEventArgs e)
        {
            if( Map == null || !enableMouseMove )
            {
                return;
            }

            MouseOver = new clsMouseOver();
            MouseOver.ScreenPos.X = e.Location.X.ToInt();
            MouseOver.ScreenPos.Y = e.Location.Y.ToInt();
            //logger.Info("Mouse: {0} {1}", e.Location.X, e.Location.Y);

            MouseOverPosCalc();
        }

        public void MouseOverPosCalc()
        {
            if(Map == null || !enableMouseMove)
            {
                return;
            }
            enableMouseMove = false;

            var xyDouble = default(XYDouble);
            var flag = false;

            var mouseLeftDownOverMinimap = GetMouseLeftDownOverMinimap();
            if ( mouseLeftDownOverMinimap != null )
            {
                if ( MouseOver != null && IsViewPosOverMinimap(MouseOver.ScreenPos) )
                {
                    var pos = new XYInt(
                        (MouseOver.ScreenPos.X * TilesPerMinimapPixel).ToInt(),
                        (MouseOver.ScreenPos.Y * TilesPerMinimapPixel).ToInt());
                    Map.TileNumClampToMap(pos);
                    LookAtTile(pos);
                }
            }
            else
            {
                var mouseOverTerrain = new clsMouseOver.clsOverTerrain();
                if ( App.SettingsManager.DirectPointer )
                {
                    if ( ScreenXyGetTerrainPos(MouseOver.ScreenPos, ref mouseOverTerrain.Pos) )
                    {
                        if ( Map.PosIsOnMap(mouseOverTerrain.Pos.Horizontal) )
                        {
                            flag = true;
                        }
                    }
                }
                else
                {
                    mouseOverTerrain.Pos.Altitude = (255.0D / 2.0D * Map.HeightMultiplier).ToInt();
                    if ( ScreenXYGetViewPlanePos(MouseOver.ScreenPos, mouseOverTerrain.Pos.Altitude, ref xyDouble) )
                    {
                        mouseOverTerrain.Pos.Horizontal.X = xyDouble.X.ToInt();
                        mouseOverTerrain.Pos.Horizontal.Y = (-xyDouble.Y).ToInt();
                        if ( Map.PosIsOnMap(mouseOverTerrain.Pos.Horizontal) )
                        {
                            mouseOverTerrain.Pos.Altitude = Map.GetTerrainHeight(mouseOverTerrain.Pos.Horizontal).ToInt();
                            flag = true;
                        }
                    }
                }
                if ( flag )
                {
                    MouseOver.OverTerrain = mouseOverTerrain;
                    mouseOverTerrain.Tile.Normal.X = Math.Floor((double)mouseOverTerrain.Pos.Horizontal.X / Constants.TerrainGridSpacing).ToInt();
                    mouseOverTerrain.Tile.Normal.Y = Math.Floor((double)mouseOverTerrain.Pos.Horizontal.Y / Constants.TerrainGridSpacing).ToInt();
                    mouseOverTerrain.Vertex.Normal.X = Math.Round((double)mouseOverTerrain.Pos.Horizontal.X / Constants.TerrainGridSpacing).ToInt();
                    mouseOverTerrain.Vertex.Normal.Y = Math.Round((double)mouseOverTerrain.Pos.Horizontal.Y / Constants.TerrainGridSpacing).ToInt();
                    mouseOverTerrain.Tile.Alignment = mouseOverTerrain.Vertex.Normal;
                    mouseOverTerrain.Vertex.Alignment = new XYInt(mouseOverTerrain.Tile.Normal.X + 1, mouseOverTerrain.Tile.Normal.Y + 1);
                    mouseOverTerrain.Triangle = Map.GetTerrainTri(mouseOverTerrain.Pos.Horizontal);
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
                    var sectorNum = Map.GetPosSectorNum(mouseOverTerrain.Pos.Horizontal);
                    foreach ( var connection in Map.Sectors[sectorNum.X, sectorNum.Y].Units )
                    {
                        var unit = connection.Unit;
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
                        if (uiOptions.MouseTool == MouseTool.TerrainBrush)
                        {
                            ApplyTerrain();
                            // TODO: implement me.
//                            if (Program.frmMainInstance.cbxAutoTexSetHeight.Checked)
//                            {
//                                applyHeightSet(App.TerrainBrush, (byte)uiOptions.Height.LmbHeight);
//                            }
                        }
                        else if (uiOptions.MouseTool == MouseTool.HeightSetBrush)
                        {
                            applyHeightSet(uiOptions.Height.Brush, (byte)uiOptions.Height.LmbHeight);
                        }
                        else if (uiOptions.MouseTool == MouseTool.TextureBrush)
                        {
                            ApplyTexture();
                        }
                        else if (uiOptions.MouseTool == MouseTool.CliffTriangle)
                        {
                            ApplyCliffTriangle(false);
                        }
                        else if (uiOptions.MouseTool == MouseTool.CliffBrush)
                        {
                            ApplyCliff();
                        }
                        else if (uiOptions.MouseTool == MouseTool.CliffRemove)
                        {
                            ApplyCliffRemove();
                        }
                        else if (uiOptions.MouseTool == MouseTool.RoadPlace)
                        {
                            ApplyRoad();
                        }
                        else if (uiOptions.MouseTool == MouseTool.RoadRemove)
                        {
                            applyRoadRemove();
                        }
                    }
                    if ( MouseRightDown != null )
                    {
                        if (uiOptions.MouseTool == MouseTool.HeightSetBrush)
                        {
                            if ( MouseLeftDown == null )
                            {
                                applyHeightSet(uiOptions.Height.Brush, (byte)uiOptions.Height.RmbHeight);
                            }
                        }
                        else if (uiOptions.MouseTool == MouseTool.CliffTriangle)
                        {
                            ApplyCliffTriangle(true);
                        }
                    }
                }
            }

            broker.DrawLater(this);
        }

        public void HandleMouseUp(object sender, MouseEventArgs e)
        {
            if(Map == null)
            {
                return;
            }

            var mouseOverTerrain = GetMouseOverTerrain();

            if ( e.Buttons == MouseButtons.Primary )
            {
                if ( GetMouseLeftDownOverMinimap() != null )
                {
                }
                else
                {
                    if (uiOptions.MouseTool == MouseTool.TerrainBrush)
                    {
                        Map.UndoStepCreate("Ground Painted");
                    }
                    else if (uiOptions.MouseTool == MouseTool.CliffTriangle)
                    {
                        Map.UndoStepCreate("Cliff Triangles");
                    }
                    else if (uiOptions.MouseTool == MouseTool.CliffBrush)
                    {
                        Map.UndoStepCreate("Cliff Brush");
                    }
                    else if (uiOptions.MouseTool == MouseTool.CliffRemove)
                    {
                        Map.UndoStepCreate("Cliff Remove Brush");
                    }
                    else if (uiOptions.MouseTool == MouseTool.HeightChangeBrush)
                    {
                        Map.UndoStepCreate("Height Change");
                    }
                    else if (uiOptions.MouseTool == MouseTool.HeightSetBrush)
                    {
                        Map.UndoStepCreate("Height Set");
                    }
                    else if (uiOptions.MouseTool == MouseTool.HeightSmoothBrush)
                    {
                        Map.UndoStepCreate("Height Smooth");
                    }
                    else if (uiOptions.MouseTool == MouseTool.TextureBrush)
                    {
                        Map.UndoStepCreate("Texture");
                    }
                    else if (uiOptions.MouseTool == MouseTool.RoadRemove)
                    {
                        Map.UndoStepCreate("Road Remove");
                    }
                    else if (uiOptions.MouseTool == MouseTool.ObjectSelect)
                    {
                        if ( Map.UnitSelectedAreaVertexA != null )
                        {
                            if ( mouseOverTerrain != null )
                            {
                                SelectUnits(Map.UnitSelectedAreaVertexA, mouseOverTerrain.Vertex.Normal);
                            }
                            Map.UnitSelectedAreaVertexA = null;
                        }
                    }
                }
                MouseLeftDown = null;
            }
            else if ( e.Buttons == MouseButtons.Alternate )
            {
                if ( GetMouseRightDownOverMinimap() != null )
                {
                }
                else
                {
                    if (uiOptions.MouseTool == MouseTool.HeightChangeBrush)
                    {
                        Map.UndoStepCreate("Height Change");
                    }
                    else if (uiOptions.MouseTool == MouseTool.HeightSetBrush)
                    {
                        Map.UndoStepCreate("Height Set");
                    }
                }
                MouseRightDown = null;
            }
        }


        public void HandleMouseDown(object sender, MouseEventArgs e)
        {
            if(Map == null)
            {
                return;
            }

            var screenPos = new XYInt();

            screenPos.X = (int)e.Location.X;
            screenPos.Y = (int)e.Location.Y;
            if ( e.Buttons == MouseButtons.Primary )
            {
                MouseLeftDown = new clsMouseDown();
                if ( IsViewPosOverMinimap(screenPos) )
                {
                    MouseLeftDown.OverMinimap = new clsMouseDown.clsOverMinimap
                        {
                            DownPos = screenPos
                        };
                    var pos = new XYInt(Math.Floor(screenPos.X * TilesPerMinimapPixel).ToInt(), Math.Floor(screenPos.Y * TilesPerMinimapPixel).ToInt());
                    Map.TileNumClampToMap(pos);
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
                        if (uiOptions.MouseTool == MouseTool.ObjectSelect)
                        {
                            if (keyboardManager.Keys[KeyboardKeys.Picker].Active)
                            {
                                if ( mouseOverTerrain.Units.Count > 0 )
                                {
                                    if ( mouseOverTerrain.Units.Count == 1 )
                                    {
                                        Program.frmMainInstance.ObjectPicker(mouseOverTerrain.Units[0].TypeBase);
                                    }
                                    else
                                    {
                                        // TODO: Implement me.
                                        // MapViewControl.ListSelectBegin(true);
                                    }
                                }
                            }
                            else if (keyboardManager.Keys[KeyboardKeys.PositionLabel].Active)
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
                                if (keyboardManager.Keys[KeyboardKeys.Multiselect].Active)
                                {
                                    Map.SelectedUnits.Clear();
                                }
                                // Program.frmMainInstance.SelectedObject_Changed(); // TODO: Implement me.
                                Map.UnitSelectedAreaVertexA = mouseOverTerrain.Vertex.Normal;
                                broker.DrawLater(this);
                            }
                        }
                        else if (uiOptions.MouseTool == MouseTool.TerrainBrush)
                        {
                            if ( Map.Tileset != null )
                            {
                                if (keyboardManager.Keys[KeyboardKeys.Picker].Active)
                                {
                                    Program.frmMainInstance.TerrainPicker();
                                }
                                else
                                {
                                    ApplyTerrain();
                                    if ( Program.frmMainInstance.cbxAutoTexSetHeight.Checked )
                                    {
                                        applyHeightSet(uiOptions.Terrain.Brush, (byte)uiOptions.Height.LmbHeight);
                                    }
                                }
                            }
                        }
                        else if (uiOptions.MouseTool == MouseTool.HeightSetBrush)
                        {
                            if (keyboardManager.Keys[KeyboardKeys.Picker].Active)
                            {
                                Program.frmMainInstance.HeightPickerL();
                            }
                            else
                            {
                                applyHeightSet(uiOptions.Height.Brush, (byte)uiOptions.Height.LmbHeight);
                            }
                        }
                        else if (uiOptions.MouseTool == MouseTool.TextureBrush)
                        {
                            if ( Map.Tileset != null )
                            {
                                if (keyboardManager.Keys[KeyboardKeys.Picker].Active)
                                {
                                    Program.frmMainInstance.TexturePicker();
                                }
                                else
                                {
                                    ApplyTexture();
                                }
                            }
                        }
                        else if (uiOptions.MouseTool == MouseTool.CliffTriangle)
                        {
                            ApplyCliffTriangle(false);
                        }
                        else if (uiOptions.MouseTool == MouseTool.CliffBrush)
                        {
                            ApplyCliff();
                        }
                        else if (uiOptions.MouseTool == MouseTool.CliffRemove)
                        {
                            ApplyCliffRemove();
                        }
                        else if (uiOptions.MouseTool == MouseTool.TerrainFill)
                        {
                            if ( Map.Tileset != null )
                            {
                                if (keyboardManager.Keys[KeyboardKeys.Picker].Active)
                                {
                                    Program.frmMainInstance.TerrainPicker();
                                }
                                else
                                {
                                    ApplyTerrainFill(Program.frmMainInstance.FillCliffAction, Program.frmMainInstance.cbxFillInside.Checked);
                                    broker.DrawLater(this);
                                }
                            }
                        }
                        else if (uiOptions.MouseTool == MouseTool.RoadPlace)
                        {
                            if ( Map.Tileset != null )
                            {
                                ApplyRoad();
                            }
                        }
                        else if (uiOptions.MouseTool == MouseTool.RoadLines)
                        {
                            if ( Map.Tileset != null )
                            {
                                ApplyRoadLineSelection();
                            }
                        }
                        else if (uiOptions.MouseTool == MouseTool.RoadRemove)
                        {
                            applyRoadRemove();
                        }
                        else if (uiOptions.MouseTool == MouseTool.ObjectPlace)
                        {
                            if ( Program.frmMainInstance.SingleSelectedObjectTypeBase != null && Map.SelectedUnitGroup != null )
                            {
                                var objectCreator = new clsUnitCreate();
                                Map.SetObjectCreatorDefaults(objectCreator);
                                objectCreator.Horizontal = mouseOverTerrain.Pos.Horizontal;
                                objectCreator.Perform();
                                Map.UndoStepCreate("Place Object");
                                broker.UpdateMap(this);
                                broker.DrawLater(this);
                            }
                        }
                        else if (uiOptions.MouseTool == MouseTool.ObjectLines)
                        {
                            applyObjectLine();
                        }
                        else if (uiOptions.MouseTool == MouseTool.TerrainSelect)
                        {
                            if ( Map.SelectedAreaVertexA == null )
                            {
                                Map.SelectedAreaVertexA = mouseOverTerrain.Vertex.Normal;
                                broker.DrawLater(this);
                            }
                            else if ( Map.SelectedAreaVertexB == null )
                            {
                                Map.SelectedAreaVertexB = mouseOverTerrain.Vertex.Normal;
                                broker.DrawLater(this);
                            }
                            else
                            {
                                Map.SelectedAreaVertexA = null;
                                Map.SelectedAreaVertexB = null;
                                broker.DrawLater(this);
                            }
                        }
                        else if (uiOptions.MouseTool == MouseTool.Gateways)
                        {
                            applyGateway();
                        }
                    }
                    else if (uiOptions.MouseTool == MouseTool.ObjectSelect)
                    {
                        Map.SelectedUnits.Clear();
                        Program.frmMainInstance.SelectedObject_Changed();
                    }
                }
            }
            else if ( e.Buttons == MouseButtons.Alternate )
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
                if (uiOptions.MouseTool == MouseTool.RoadLines || uiOptions.MouseTool == MouseTool.ObjectLines)
                {
                    Map.SelectedTileA = null;
                    broker.DrawLater(this);
                }
                else if (uiOptions.MouseTool == MouseTool.TerrainSelect)
                {
                    Map.SelectedAreaVertexA = null;
                    Map.SelectedAreaVertexB = null;
                    broker.DrawLater(this);
                }
                else if (uiOptions.MouseTool == MouseTool.CliffTriangle)
                {
                    ApplyCliffTriangle(true);
                }
                else if (uiOptions.MouseTool == MouseTool.Gateways)
                {
                    Map.SelectedTileA = null;
                    Map.SelectedTileB = null;
                    broker.DrawLater(this);
                }
                else if (uiOptions.MouseTool == MouseTool.HeightSetBrush)
                {
                    if (keyboardManager.Keys[KeyboardKeys.Picker].Active)
                    {
                        Program.frmMainInstance.HeightPickerR();
                    }
                    else
                    {
                        applyHeightSet(uiOptions.Height.Brush, (byte)uiOptions.Height.RmbHeight);
                    }
                }
            }
        }

        public void HandleLostFocus(object sender, EventArgs e)
        {
            MouseOver = null;
            MouseLeftDown = null;
            MouseRightDown = null;
        }

        public void HandleMouseWheel(object sender, MouseEventArgs e)
        {
            if(Map == null)
            {
                return;
            }

            var move = new XYZInt(0, 0, 0);
            var xyzDbl = default(XYZDouble);
            var a = 0;

            for ( a = 0; a <= Math.Abs(e.Delta.Height).ToInt(); a++ )
            {
                Matrix3DMath.VectorForwardsRotationByMatrix(ViewAngleMatrix, 
                    (Math.Sign(e.Delta.Height * 120f) * Math.Max(ViewPos.Y, 512.0D) / 24.0D), ref xyzDbl);
                move.SetDbl(xyzDbl);
                ViewPosChange(move);
            }
        }

        public void HandleMouseLeave(object sender, MouseEventArgs e)
        {
            MouseOver = null;
        }

        private void SelectUnits(XYInt vertexA, XYInt vertexB)
        {
            var mouseOverTerrain = GetMouseOverTerrain();
            var sectorNum = new XYInt();
            Unit unit;
            var sectorStart = new XYInt();
            var sectorFinish = new XYInt();
            var startPos = new XYInt();
            var finishPos = new XYInt();
            var startVertex = new XYInt();
            var finishVertex = new XYInt();

            if ( Math.Abs(vertexA.X - vertexB.X) <= 1 &&
                Math.Abs(vertexA.Y - vertexB.Y) <= 1 &&
                mouseOverTerrain != null )
            {
                if ( mouseOverTerrain.Units.Count > 0 )
                {
                    if ( mouseOverTerrain.Units.Count == 1 )
                    {
                        unit = mouseOverTerrain.Units[0];
                        if ( unit.MapSelectedUnitLink.IsConnected )
                        {
                            unit.MapDeselect();
                        }
                        else
                        {
                            unit.MapSelect();
                        }
                    }
                    else
                    {
                        // TODO: Implement me - ref MapViewControl.ListSelectBegin
                        // ListSelectBegin(false);
                    }
                }
            }
            else
            {
                MathUtil.ReorderXY(vertexA, vertexB, ref startVertex, ref finishVertex);
                startPos.X = startVertex.X * Constants.TerrainGridSpacing;
                startPos.Y = startVertex.Y * Constants.TerrainGridSpacing;
                finishPos.X = finishVertex.X * Constants.TerrainGridSpacing;
                finishPos.Y = finishVertex.Y * Constants.TerrainGridSpacing;
                sectorStart.X = Math.Min(startVertex.X / Constants.SectorTileSize, Map.SectorCount.X - 1);
                sectorStart.Y = Math.Min(startVertex.Y / Constants.SectorTileSize, Map.SectorCount.Y - 1);
                sectorFinish.X = Math.Min(finishVertex.X / Constants.SectorTileSize, Map.SectorCount.X - 1);
                sectorFinish.Y = Math.Min(finishVertex.Y / Constants.SectorTileSize, Map.SectorCount.Y - 1);
                for ( sectorNum.Y = sectorStart.Y; sectorNum.Y <= sectorFinish.Y; sectorNum.Y++ )
                {
                    for ( sectorNum.X = sectorStart.X; sectorNum.X <= sectorFinish.X; sectorNum.X++ )
                    {
                        foreach ( var connection in Map.Sectors[sectorNum.X, sectorNum.Y].Units )
                        {
                            unit = connection.Unit;
                            if ( unit.Pos.Horizontal.X >= startPos.X & unit.Pos.Horizontal.Y >= startPos.Y &
                                unit.Pos.Horizontal.X <= finishPos.X & unit.Pos.Horizontal.Y <= finishPos.Y )
                            {
                                if ( !unit.MapSelectedUnitLink.IsConnected )
                                {
                                    unit.MapSelect();
                                }
                            }
                        }
                    }
                }
            }

            //Program.frmMainInstance.SelectedObject_Changed(); // TODO: Implement with UiOptions
            broker.DrawLater(this);
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

            move *= FOVMultiplier * (glSurface.Size.Width + glSurface.Size.Height) * Math.Max(Math.Abs(ViewPos.Y), 512.0D);

            if (keyboardManager.Keys[KeyboardKeys.ViewZoomIn].Active)
            {
                fovScale_2EChange(Convert.ToDouble(- zoom));
            }
            if (keyboardManager.Keys[KeyboardKeys.ViewZoomOut].Active)
            {
                fovScale_2EChange(zoom);
            }

            if ( App.ViewMoveType == ViewMoveType.Free )
            {
                viewPosChangeXyz.X = 0;
                viewPosChangeXyz.Y = 0;
                viewPosChangeXyz.Z = 0;
                if (keyboardManager.Keys[KeyboardKeys.ViewMoveForwards].Active)
                {
                    Matrix3DMath.VectorForwardsRotationByMatrix(ViewAngleMatrix, move, ref xyzDbl);
                    viewPosChangeXyz.AddDbl(xyzDbl);
                }
                if (keyboardManager.Keys[KeyboardKeys.ViewMoveBackwards].Active)
                {
                    Matrix3DMath.VectorBackwardsRotationByMatrix(ViewAngleMatrix, move, ref xyzDbl);
                    viewPosChangeXyz.AddDbl(xyzDbl);
                }
                if (keyboardManager.Keys[KeyboardKeys.ViewMoveLeft].Active)
                {
                    Matrix3DMath.VectorLeftRotationByMatrix(ViewAngleMatrix, move, ref xyzDbl);
                    viewPosChangeXyz.AddDbl(xyzDbl);
                }
                if (keyboardManager.Keys[KeyboardKeys.ViewMoveRight].Active)
                {
                    Matrix3DMath.VectorRightRotationByMatrix(ViewAngleMatrix, move, ref xyzDbl);
                    viewPosChangeXyz.AddDbl(xyzDbl);
                }
                if (keyboardManager.Keys[KeyboardKeys.ViewMoveUp].Active)
                {
                    Matrix3DMath.VectorUpRotationByMatrix(ViewAngleMatrix, move, ref xyzDbl);
                    viewPosChangeXyz.AddDbl(xyzDbl);
                }
                if (keyboardManager.Keys[KeyboardKeys.ViewMoveDown].Active)
                {
                    Matrix3DMath.VectorDownRotationByMatrix(ViewAngleMatrix, move, ref xyzDbl);
                    viewPosChangeXyz.AddDbl(xyzDbl);
                }

                viewAngleChange.X = 0.0D;
                viewAngleChange.Y = 0.0D;
                viewAngleChange.Z = 0.0D;
                if (keyboardManager.Keys[KeyboardKeys.ViewRotateLeft].Active)
                {
                    Matrix3DMath.VectorForwardsRotationByMatrix(ViewAngleMatrix, roll, ref xyzDbl);
                    viewAngleChange += xyzDbl;
                }
                if (keyboardManager.Keys[KeyboardKeys.ViewRotateRight].Active)
                {
                    Matrix3DMath.VectorBackwardsRotationByMatrix(ViewAngleMatrix, roll, ref xyzDbl);
                    viewAngleChange += xyzDbl;
                }
                if (keyboardManager.Keys[KeyboardKeys.ViewRotateBackwards].Active)
                {
                    Matrix3DMath.VectorLeftRotationByMatrix(ViewAngleMatrix, panRate, ref xyzDbl);
                    viewAngleChange += xyzDbl;
                }
                if (keyboardManager.Keys[KeyboardKeys.ViewRotateForwards].Active)
                {
                    Matrix3DMath.VectorRightRotationByMatrix(ViewAngleMatrix, panRate, ref xyzDbl);
                    viewAngleChange += xyzDbl;
                }
                if (keyboardManager.Keys[KeyboardKeys.ViewRollLeft].Active)
                {
                    Matrix3DMath.VectorDownRotationByMatrix(ViewAngleMatrix, panRate, ref xyzDbl);
                    viewAngleChange += xyzDbl;
                }
                if (keyboardManager.Keys[KeyboardKeys.ViewRollRight].Active)
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
                if (keyboardManager.Keys[KeyboardKeys.ViewMoveForwards].Active)
                {
                    Matrix3DMath.VectorForwardsRotationByMatrix(matrixA, move, ref xyzDbl);
                    viewPosChangeXyz.AddDbl(xyzDbl);
                }
                if (keyboardManager.Keys[KeyboardKeys.ViewMoveBackwards].Active)
                {
                    Matrix3DMath.VectorBackwardsRotationByMatrix(matrixA, move, ref xyzDbl);
                    viewPosChangeXyz.AddDbl(xyzDbl);
                }
                if (keyboardManager.Keys[KeyboardKeys.ViewMoveLeft].Active)
                {
                    Matrix3DMath.VectorLeftRotationByMatrix(matrixA, move, ref xyzDbl);
                    viewPosChangeXyz.AddDbl(xyzDbl);
                }
                if (keyboardManager.Keys[KeyboardKeys.ViewMoveRight].Active)
                {
                    Matrix3DMath.VectorRightRotationByMatrix(matrixA, move, ref xyzDbl);
                    viewPosChangeXyz.AddDbl(xyzDbl);
                }
                if (keyboardManager.Keys[KeyboardKeys.ViewMoveUp].Active)
                {
                    viewPosChangeXyz.Y += Convert.ToInt32(move);
                }
                if (keyboardManager.Keys[KeyboardKeys.ViewMoveDown].Active)
                {
                    viewPosChangeXyz.Y -= Convert.ToInt32(move);
                }

                if ( App.RTSOrbit )
                {
                    if (keyboardManager.Keys[KeyboardKeys.ViewRotateForwards].Active)
                    {
                        anglePY.Pitch = MathUtil.ClampDbl(anglePY.Pitch + orbitRate, Convert.ToDouble(- MathUtil.RadOf90Deg + 0.03125D * MathUtil.RadOf1Deg),
                            MathUtil.RadOf90Deg - 0.03125D * MathUtil.RadOf1Deg);
                        angleChanged = true;
                    }
                    if (keyboardManager.Keys[KeyboardKeys.ViewRotateBackwards].Active)
                    {
                        anglePY.Pitch = MathUtil.ClampDbl(anglePY.Pitch - orbitRate, Convert.ToDouble(- MathUtil.RadOf90Deg + 0.03125D * MathUtil.RadOf1Deg),
                            MathUtil.RadOf90Deg - 0.03125D * MathUtil.RadOf1Deg);
                        angleChanged = true;
                    }
                    if (keyboardManager.Keys[KeyboardKeys.ViewRotateLeft].Active)
                    {
                        anglePY.Yaw = MathUtil.AngleClamp(anglePY.Yaw + orbitRate);
                        angleChanged = true;
                    }
                    if (keyboardManager.Keys[KeyboardKeys.ViewRotateRight].Active)
                    {
                        anglePY.Yaw = MathUtil.AngleClamp(anglePY.Yaw - orbitRate);
                        angleChanged = true;
                    }
                }
                else
                {
                    if (keyboardManager.Keys[KeyboardKeys.ViewRotateForwards].Active)
                    {
                        anglePY.Pitch = MathUtil.ClampDbl(anglePY.Pitch - orbitRate, Convert.ToDouble(- MathUtil.RadOf90Deg + 0.03125D * MathUtil.RadOf1Deg),
                            MathUtil.RadOf90Deg - 0.03125D * MathUtil.RadOf1Deg);
                        angleChanged = true;
                    }
                    if (keyboardManager.Keys[KeyboardKeys.ViewRotateBackwards].Active)
                    {
                        anglePY.Pitch = MathUtil.ClampDbl(anglePY.Pitch + orbitRate, Convert.ToDouble(- MathUtil.RadOf90Deg + 0.03125D * MathUtil.RadOf1Deg),
                            MathUtil.RadOf90Deg - 0.03125D * MathUtil.RadOf1Deg);
                        angleChanged = true;
                    }
                    if (keyboardManager.Keys[KeyboardKeys.ViewRotateLeft].Active)
                    {
                        anglePY.Yaw = MathUtil.AngleClamp(anglePY.Yaw - orbitRate);
                        angleChanged = true;
                    }
                    if (keyboardManager.Keys[KeyboardKeys.ViewRotateRight].Active)
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
            if(GetMouseOverTerrain() == null)
            {
                return;
            }

            if (uiOptions.MouseTool == MouseTool.HeightSmoothBrush)
            {
                if ( GetMouseLeftDownOverTerrain() != null )
                {
                    ApplyHeightSmoothing(MathUtil.ClampDbl(uiOptions.Height.SmoothRate * 0.1D, 0.0D, 1.0D));
                }
            }
            else if (uiOptions.MouseTool == MouseTool.HeightChangeBrush)
            {
                if ( GetMouseLeftDownOverTerrain() != null )
                {
                    applyHeightChange(MathUtil.ClampDbl(uiOptions.Height.ChangeRate, -255.0D, 255.0D));
                }
                else if ( GetMouseRightDownOverTerrain() != null )
                {
                    applyHeightChange(MathUtil.ClampDbl(Convert.ToDouble(-uiOptions.Height.ChangeRate), -255.0D, 255.0D));
                }
            }
        }

        private void applyObjectLine()
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

                if ( Map.SelectedTileA != null )
                {
                    if ( tile.X == Map.SelectedTileA.X )
                    {
                        if ( tile.Y <= Map.SelectedTileA.Y )
                        {
                            a = tile.Y;
                            b = Map.SelectedTileA.Y;
                        }
                        else
                        {
                            a = Map.SelectedTileA.Y;
                            b = tile.Y;
                        }
                        var objectCreator = new clsUnitCreate();
                        Map.SetObjectCreatorDefaults(objectCreator);
                        for ( num = a; num <= b; num++ )
                        {
                            objectCreator.Horizontal.X = Convert.ToInt32((tile.X + 0.5D) * Constants.TerrainGridSpacing);
                            objectCreator.Horizontal.Y = Convert.ToInt32((num + 0.5D) * Constants.TerrainGridSpacing);
                            objectCreator.Perform();
                        }

                        Map.UndoStepCreate("Object Line");
                        broker.UpdateMap(this);
                        Map.SelectedTileA = null;
                        broker.DrawLater(this);
                    }
                    else if ( tile.Y == Map.SelectedTileA.Y )
                    {
                        if ( tile.X <= Map.SelectedTileA.X )
                        {
                            a = tile.X;
                            b = Map.SelectedTileA.X;
                        }
                        else
                        {
                            a = Map.SelectedTileA.X;
                            b = tile.X;
                        }
                        var objectCreator = new clsUnitCreate();
                        Map.SetObjectCreatorDefaults(objectCreator);
                        for ( num = a; num <= b; num++ )
                        {
                            objectCreator.Horizontal.X = Convert.ToInt32((num + 0.5D) * Constants.TerrainGridSpacing);
                            objectCreator.Horizontal.Y = Convert.ToInt32((tile.Y + 0.5D) * Constants.TerrainGridSpacing);
                            objectCreator.Perform();
                        }

                        Map.UndoStepCreate("Object Line");
                        broker.UpdateMap(this);
                        Map.SelectedTileA = null;
                        broker.DrawLater(this);
                    }
                }
                else
                {
                    Map.SelectedTileA = tile;
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