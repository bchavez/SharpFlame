#region

using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using SharpFlame.AppSettings;
using SharpFlame.Colors;
using SharpFlame.Core.Domain;
using SharpFlame.FileIO;
using SharpFlame.Graphics.OpenGL;
using SharpFlame.Mapping.Drawing;
using SharpFlame.Mapping.Objects;
using SharpFlame.Mapping.Script;
using SharpFlame.Mapping.Tools;
using SharpFlame.Maths;
using SharpFlame.Util;

#endregion

namespace SharpFlame.Mapping
{
    public partial class clsMap
    {
        public clsViewInfo ViewInfo;

        public void GLDraw()
        {
            var xyzDbl = default(XYZDouble);
            var x2 = 0;
            var y2 = 0;
            var a = 0;
            var b = 0;
            var D = 0;
            sRGBA_sng colourA;
            sRGBA_sng colourB;
            var showMinimapViewPosBox = default(bool);
            var viewCorner0 = default(XYDouble);
            var viewCorner1 = default(XYDouble);
            var viewCorner2 = default(XYDouble);
            var viewCorner3 = default(XYDouble);
            double dblTemp = 0;
            var vertex0 = default(XYZDouble);
            var vertex1 = default(XYZDouble);
            var vertex2 = default(XYZDouble);
            var vertex3 = default(XYZDouble);
            var screenPos = new XYInt();
            var xyzDbl2 = default(XYZDouble);
            var posA = default(XYDouble);
            var posB = default(XYDouble);
            var posC = default(XYDouble);
            var posD = default(XYDouble);
            var minimapSizeXy = new XYInt();
            clsUnit unit;
            var startXy = new XYInt();
            var finishXy = new XYInt();
            bool drawIt;
            var drawCentreSector = new clsBrush.sPosNum();
            var selectionLabel = new clsTextLabel();
            var lightPosition = new float[4];
            var matrixB = new Matrix3DMath.Matrix3D();
            var mapAction = default(clsAction);
            float zNearFar = 0;
            var mapViewControl = ViewInfo.MapViewControl;
            var glSize = ViewInfo.MapViewControl.GLSize;
            var drawCentre = default(XYDouble);

            dblTemp = SettingsManager.Settings.MinimapSize;
            ViewInfo.TilesPerMinimapPixel = Math.Sqrt(Terrain.TileSize.X * Terrain.TileSize.X + Terrain.TileSize.Y * Terrain.TileSize.Y) /
                                               (MathUtil.RootTwo * dblTemp);
            if ( Minimap_Texture_Size > 0 & ViewInfo.TilesPerMinimapPixel > 0.0D )
            {
                minimapSizeXy.X = (int)(Terrain.TileSize.X / ViewInfo.TilesPerMinimapPixel);
                minimapSizeXy.Y = (int)(Terrain.TileSize.Y / ViewInfo.TilesPerMinimapPixel);
            }

            if ( !ViewInfo.ScreenXYGetViewPlanePos(new XYInt((int)(glSize.X / 2.0D), (int)(glSize.Y / 2.0D)), dblTemp, ref drawCentre) )
            {
                Matrix3DMath.VectorForwardsRotationByMatrix(ViewInfo.ViewAngleMatrix, ref xyzDbl);
                var dblTemp2 = App.VisionRadius * 2.0D / Math.Sqrt(xyzDbl.X * xyzDbl.X + xyzDbl.Z * xyzDbl.Z);
                drawCentre.X = ViewInfo.ViewPos.X + xyzDbl.X * dblTemp2;
                drawCentre.Y = ViewInfo.ViewPos.Z + xyzDbl.Z * dblTemp2;
            }
            drawCentre.X = MathUtil.ClampDbl(drawCentre.X, 0.0D, Terrain.TileSize.X * Constants.TerrainGridSpacing - 1.0D);
            drawCentre.Y = MathUtil.ClampDbl(Convert.ToDouble(- drawCentre.Y), 0.0D, Terrain.TileSize.Y * Constants.TerrainGridSpacing - 1.0D);
            drawCentreSector.Normal = GetPosSectorNum(new XYInt((int)drawCentre.X, (int)drawCentre.Y));
            drawCentreSector.Alignment =
                GetPosSectorNum(new XYInt((int)(drawCentre.X - Constants.SectorTileSize * Constants.TerrainGridSpacing / 2.0D),
                    (int)(drawCentre.Y - Constants.SectorTileSize * Constants.TerrainGridSpacing / 2.0D)));

            var DrawObjects = new clsDrawSectorObjects();
            DrawObjects.Map = this;
            DrawObjects.UnitTextLabels = new clsTextLabels(64);
            DrawObjects.Start();

            xyzDbl.X = drawCentre.X - ViewInfo.ViewPos.X;
            xyzDbl.Y = 128 - ViewInfo.ViewPos.Y;
            xyzDbl.Z = - drawCentre.Y - ViewInfo.ViewPos.Z;
            zNearFar = (float)(xyzDbl.GetMagnitude());

            GL.Enable(EnableCap.DepthTest);
            GL.MatrixMode(MatrixMode.Projection);
            var temp_mat = Matrix4.CreatePerspectiveFieldOfView(ViewInfo.FieldOfViewY, mapViewControl.OpenGLControl.AspectRatio, zNearFar / 128.0F, zNearFar * 128.0F);
            GL.LoadMatrix(ref temp_mat);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            Matrix3DMath.MatrixRotationByMatrix(ViewInfo.ViewAngleMatrixInverted, App.SunAngleMatrix, matrixB);
            Matrix3DMath.VectorForwardsRotationByMatrix(matrixB, ref xyzDbl);
            lightPosition[0] = (float)xyzDbl.X;
            lightPosition[1] = (float)xyzDbl.Y;
            lightPosition[2] = Convert.ToSingle(- xyzDbl.Z);
            lightPosition[3] = 0.0F;
            GL.Light(LightName.Light0, LightParameter.Position, lightPosition);
            GL.Light(LightName.Light1, LightParameter.Position, lightPosition);

            GL.Disable(EnableCap.Light0);
            GL.Disable(EnableCap.Light1);
            if ( App.Draw_Lighting != DrawLighting.Off )
            {
                if ( App.Draw_Lighting == DrawLighting.Half )
                {
                    GL.Enable(EnableCap.Light0);
                }
                else if ( App.Draw_Lighting == DrawLighting.Normal )
                {
                    GL.Enable(EnableCap.Light1);
                }
                GL.Enable(EnableCap.Lighting);
            }
            else
            {
                GL.Disable(EnableCap.Lighting);
            }

            dblTemp = 127.5D * HeightMultiplier;
            if ( ViewInfo.ScreenXYGetViewPlanePosForwardDownOnly(0, 0, dblTemp, ref viewCorner0)
                 && ViewInfo.ScreenXYGetViewPlanePosForwardDownOnly(glSize.X, 0, dblTemp, ref viewCorner1)
                 && ViewInfo.ScreenXYGetViewPlanePosForwardDownOnly(glSize.X, glSize.Y, dblTemp, ref viewCorner2)
                 && ViewInfo.ScreenXYGetViewPlanePosForwardDownOnly(0, glSize.Y, dblTemp, ref viewCorner3) )
            {
                showMinimapViewPosBox = true;
            }
            else
            {
                showMinimapViewPosBox = false;
            }

            GL.Rotate((float)(ViewInfo.ViewAngleRPY.Roll / MathUtil.RadOf1Deg), 0.0F, 0.0F, -1.0F);
            GL.Rotate((float)(ViewInfo.ViewAngleRPY.Pitch / MathUtil.RadOf1Deg), 1.0F, 0.0F, 0.0F);
            GL.Rotate((float)(ViewInfo.ViewAngleRPY.Yaw / MathUtil.RadOf1Deg), 0.0F, 1.0F, 0.0F);
            GL.Translate(Convert.ToDouble(- ViewInfo.ViewPos.X), Convert.ToDouble(- ViewInfo.ViewPos.Y), ViewInfo.ViewPos.Z);

            GL.Enable(EnableCap.CullFace);

            DebugGLError("Matrix modes");

            if ( App.Draw_TileTextures )
            {
                GL.Color3(1.0F, 1.0F, 1.0F);
                GL.Enable(EnableCap.Texture2D);
                mapAction = new clsDrawCallTerrain();
                mapAction.Map = this;
                App.VisionSectors.PerformActionMapSectors(mapAction, drawCentreSector);
                GL.Disable(EnableCap.Texture2D);

                DebugGLError("Tile textures");
            }

            GL.Disable(EnableCap.DepthTest);
            GL.Disable(EnableCap.Lighting);

            if ( App.Draw_TileWireframe )
            {
                GL.Color3(0.0F, 1.0F, 0.0F);
                GL.LineWidth(1.0F);
                var DrawCallTerrainWireframe = new clsDrawCallTerrainWireframe();
                DrawCallTerrainWireframe.Map = this;
                App.VisionSectors.PerformActionMapSectors(DrawCallTerrainWireframe, drawCentreSector);

                DebugGLError("Wireframe");
            }

            //draw tile orientation markers

            if ( App.DisplayTileOrientation )
            {
                GL.Disable(EnableCap.CullFace);

                GL.Begin(BeginMode.Triangles);
                GL.Color3(1.0F, 1.0F, 0.0F);
                mapAction = new clsDrawTileOrientation();
                mapAction.Map = this;
                App.VisionSectors.PerformActionMapSectors(mapAction, drawCentreSector);
                GL.End();

                GL.Enable(EnableCap.CullFace);

                DebugGLError("Tile orientation");
            }

            //draw painted texture terrain type markers

            var RGB_sng = new sRGB_sng();

            var MouseOverTerrain = ViewInfo.GetMouseOverTerrain();

            if ( App.Draw_VertexTerrain )
            {
                GL.LineWidth(1.0F);
                var DrawVertexTerran = new clsDrawVertexTerrain();
                DrawVertexTerran.Map = this;
                DrawVertexTerran.ViewAngleMatrix = ViewInfo.ViewAngleMatrix;
                App.VisionSectors.PerformActionMapSectors(DrawVertexTerran, drawCentreSector);
                DebugGLError("Terrain type markers");
            }

            selectionLabel.Text = "";

            if ( Selected_Area_VertexA != null )
            {
                drawIt = false;
                if ( Selected_Area_VertexB != null )
                {
                    //area is selected
                    MathUtil.ReorderXY(Selected_Area_VertexA, Selected_Area_VertexB, ref startXy, ref finishXy);
                    xyzDbl.X = Selected_Area_VertexB.X * Constants.TerrainGridSpacing - ViewInfo.ViewPos.X;
                    xyzDbl.Z = - Selected_Area_VertexB.Y * Constants.TerrainGridSpacing - ViewInfo.ViewPos.Z;
                    xyzDbl.Y = GetVertexAltitude(Selected_Area_VertexB) - ViewInfo.ViewPos.Y;
                    drawIt = true;
                }
                else if ( modTools.Tool == modTools.Tools.TerrainSelect )
                {
                    if ( MouseOverTerrain != null )
                    {
                        //selection is changing under pointer
                        MathUtil.ReorderXY(Selected_Area_VertexA, MouseOverTerrain.Vertex.Normal, ref startXy, ref finishXy);
                        xyzDbl.X = MouseOverTerrain.Vertex.Normal.X * Constants.TerrainGridSpacing - ViewInfo.ViewPos.X;
                        xyzDbl.Z = - MouseOverTerrain.Vertex.Normal.Y * Constants.TerrainGridSpacing - ViewInfo.ViewPos.Z;
                        xyzDbl.Y = GetVertexAltitude(MouseOverTerrain.Vertex.Normal) - ViewInfo.ViewPos.Y;
                        drawIt = true;
                    }
                }
                if ( drawIt )
                {
                    Matrix3DMath.VectorRotationByMatrix(ViewInfo.ViewAngleMatrixInverted, xyzDbl, ref xyzDbl2);
                    if ( ViewInfo.PosGetScreenXY(xyzDbl2, ref screenPos) )
                    {
                        if ( screenPos.X >= 0 & screenPos.X <= glSize.X & screenPos.Y >= 0 & screenPos.Y <= glSize.Y )
                        {
                            selectionLabel.Colour.Red = 1.0F;
                            selectionLabel.Colour.Green = 1.0F;
                            selectionLabel.Colour.Blue = 1.0F;
                            selectionLabel.Colour.Alpha = 1.0F;
                            selectionLabel.TextFont = App.UnitLabelFont;
                            selectionLabel.SizeY = SettingsManager.Settings.FontSize;
                            selectionLabel.Pos = screenPos;
                            selectionLabel.Text = finishXy.X - startXy.X + "x" + Convert.ToString(finishXy.Y - startXy.Y);
                        }
                    }
                    GL.LineWidth(3.0F);
                    var DrawSelection = new clsDrawTileAreaOutline();
                    DrawSelection.Map = this;
                    DrawSelection.StartXY = startXy;
                    DrawSelection.FinishXY = finishXy;
                    DrawSelection.Colour = new sRGBA_sng(1.0F, 1.0F, 1.0F, 1.0F);
                    DrawSelection.ActionPerform();
                }

                DebugGLError("Terrain selection box");
            }

            if ( modTools.Tool == modTools.Tools.TerrainSelect )
            {
                if ( MouseOverTerrain != null )
                {
                    //draw mouseover vertex
                    GL.LineWidth(3.0F);

                    vertex0.X = MouseOverTerrain.Vertex.Normal.X * Constants.TerrainGridSpacing;
                    vertex0.Y = Convert.ToDouble(Terrain.Vertices[MouseOverTerrain.Vertex.Normal.X, MouseOverTerrain.Vertex.Normal.Y].Height * HeightMultiplier);
                    vertex0.Z = - MouseOverTerrain.Vertex.Normal.Y * Constants.TerrainGridSpacing;
                    GL.Begin(BeginMode.Lines);
                    GL.Color3(1.0F, 1.0F, 1.0F);
                    GL.Vertex3(vertex0.X - 8.0D, vertex0.Y, Convert.ToDouble(- vertex0.Z));
                    GL.Vertex3(vertex0.X + 8.0D, vertex0.Y, Convert.ToDouble(- vertex0.Z));
                    GL.Vertex3(vertex0.X, vertex0.Y, - vertex0.Z - 8.0D);
                    GL.Vertex3(vertex0.X, vertex0.Y, - vertex0.Z + 8.0D);
                    GL.End();
                }
                DebugGLError("Terrain selection vertex");
            }

            if ( App.Draw_Gateways )
            {
                GL.LineWidth(2.0F);
                foreach ( var gateway in Gateways )
                {
                    var c = 0;
                    if ( gateway.PosA.X == gateway.PosB.X )
                    {
                        if ( gateway.PosA.Y <= gateway.PosB.Y )
                        {
                            c = gateway.PosA.Y;
                            D = gateway.PosB.Y;
                        }
                        else
                        {
                            c = gateway.PosB.Y;
                            D = gateway.PosA.Y;
                        }
                        x2 = gateway.PosA.X;
                        for ( y2 = c; y2 <= D; y2++ )
                        {
                            vertex0.X = x2 * Constants.TerrainGridSpacing;
                            vertex0.Y = Convert.ToDouble(Terrain.Vertices[x2, y2].Height * HeightMultiplier);
                            vertex0.Z = - y2 * Constants.TerrainGridSpacing;
                            vertex1.X = (x2 + 1) * Constants.TerrainGridSpacing;
                            vertex1.Y = Convert.ToDouble(Terrain.Vertices[x2 + 1, y2].Height * HeightMultiplier);
                            vertex1.Z = - y2 * Constants.TerrainGridSpacing;
                            vertex2.X = x2 * Constants.TerrainGridSpacing;
                            vertex2.Y = Convert.ToDouble(Terrain.Vertices[x2, y2 + 1].Height * HeightMultiplier);
                            vertex2.Z = - (y2 + 1) * Constants.TerrainGridSpacing;
                            vertex3.X = (x2 + 1) * Constants.TerrainGridSpacing;
                            vertex3.Y = Convert.ToDouble(Terrain.Vertices[x2 + 1, y2 + 1].Height * HeightMultiplier);
                            vertex3.Z = - (y2 + 1) * Constants.TerrainGridSpacing;
                            GL.Begin(BeginMode.LineLoop);
                            GL.Color3(0.75F, 1.0F, 0.0F);
                            GL.Vertex3(vertex0.X, vertex0.Y, Convert.ToDouble(- vertex0.Z));
                            GL.Vertex3(vertex1.X, vertex1.Y, Convert.ToDouble(- vertex1.Z));
                            GL.Vertex3(vertex3.X, vertex3.Y, Convert.ToDouble(- vertex3.Z));
                            GL.Vertex3(vertex2.X, vertex2.Y, Convert.ToDouble(- vertex2.Z));
                            GL.End();
                        }
                    }
                    else if ( gateway.PosA.Y == gateway.PosB.Y )
                    {
                        if ( gateway.PosA.X <= gateway.PosB.X )
                        {
                            c = gateway.PosA.X;
                            D = gateway.PosB.X;
                        }
                        else
                        {
                            c = gateway.PosB.X;
                            D = gateway.PosA.X;
                        }
                        y2 = gateway.PosA.Y;
                        for ( x2 = c; x2 <= D; x2++ )
                        {
                            vertex0.X = x2 * Constants.TerrainGridSpacing;
                            vertex0.Y = Convert.ToDouble(Terrain.Vertices[x2, y2].Height * HeightMultiplier);
                            vertex0.Z = - y2 * Constants.TerrainGridSpacing;
                            vertex1.X = (x2 + 1) * Constants.TerrainGridSpacing;
                            vertex1.Y = Convert.ToDouble(Terrain.Vertices[x2 + 1, y2].Height * HeightMultiplier);
                            vertex1.Z = - y2 * Constants.TerrainGridSpacing;
                            vertex2.X = x2 * Constants.TerrainGridSpacing;
                            vertex2.Y = Convert.ToDouble(Terrain.Vertices[x2, y2 + 1].Height * HeightMultiplier);
                            vertex2.Z = - (y2 + 1) * Constants.TerrainGridSpacing;
                            vertex3.X = (x2 + 1) * Constants.TerrainGridSpacing;
                            vertex3.Y = Convert.ToDouble(Terrain.Vertices[x2 + 1, y2 + 1].Height * HeightMultiplier);
                            vertex3.Z = - (y2 + 1) * Constants.TerrainGridSpacing;
                            GL.Begin(BeginMode.LineLoop);
                            GL.Color3(0.75F, 1.0F, 0.0F);
                            GL.Vertex3(vertex0.X, vertex0.Y, Convert.ToDouble(- vertex0.Z));
                            GL.Vertex3(vertex1.X, vertex1.Y, Convert.ToDouble(- vertex1.Z));
                            GL.Vertex3(vertex3.X, vertex3.Y, Convert.ToDouble(- vertex3.Z));
                            GL.Vertex3(vertex2.X, vertex2.Y, Convert.ToDouble(- vertex2.Z));
                            GL.End();
                        }
                    }
                    else
                    {
                        //draw invalid gateways as red tile borders
                        x2 = gateway.PosA.X;
                        y2 = gateway.PosA.Y;

                        vertex0.X = x2 * Constants.TerrainGridSpacing;
                        vertex0.Y = Convert.ToDouble(Terrain.Vertices[x2, y2].Height * HeightMultiplier);
                        vertex0.Z = - y2 * Constants.TerrainGridSpacing;
                        vertex1.X = (x2 + 1) * Constants.TerrainGridSpacing;
                        vertex1.Y = Convert.ToDouble(Terrain.Vertices[x2 + 1, y2].Height * HeightMultiplier);
                        vertex1.Z = - y2 * Constants.TerrainGridSpacing;
                        vertex2.X = x2 * Constants.TerrainGridSpacing;
                        vertex2.Y = Convert.ToDouble(Terrain.Vertices[x2, y2 + 1].Height * HeightMultiplier);
                        vertex2.Z = - (y2 + 1) * Constants.TerrainGridSpacing;
                        vertex3.X = (x2 + 1) * Constants.TerrainGridSpacing;
                        vertex3.Y = Convert.ToDouble(Terrain.Vertices[x2 + 1, y2 + 1].Height * HeightMultiplier);
                        vertex3.Z = - (y2 + 1) * Constants.TerrainGridSpacing;
                        GL.Begin(BeginMode.LineLoop);
                        GL.Color3(1.0F, 0.0F, 0.0F);
                        GL.Vertex3(vertex0.X, vertex0.Y, Convert.ToDouble(- vertex0.Z));
                        GL.Vertex3(vertex1.X, vertex1.Y, Convert.ToDouble(- vertex1.Z));
                        GL.Vertex3(vertex3.X, vertex3.Y, Convert.ToDouble(- vertex3.Z));
                        GL.Vertex3(vertex2.X, vertex2.Y, Convert.ToDouble(- vertex2.Z));
                        GL.End();

                        x2 = gateway.PosB.X;
                        y2 = gateway.PosB.Y;

                        vertex0.X = x2 * Constants.TerrainGridSpacing;
                        vertex0.Y = Convert.ToDouble(Terrain.Vertices[x2, y2].Height * HeightMultiplier);
                        vertex0.Z = - y2 * Constants.TerrainGridSpacing;
                        vertex1.X = (x2 + 1) * Constants.TerrainGridSpacing;
                        vertex1.Y = Convert.ToDouble(Terrain.Vertices[x2 + 1, y2].Height * HeightMultiplier);
                        vertex1.Z = - y2 * Constants.TerrainGridSpacing;
                        vertex2.X = x2 * Constants.TerrainGridSpacing;
                        vertex2.Y = Convert.ToDouble(Terrain.Vertices[x2, y2 + 1].Height * HeightMultiplier);
                        vertex2.Z = - (y2 + 1) * Constants.TerrainGridSpacing;
                        vertex3.X = (x2 + 1) * Constants.TerrainGridSpacing;
                        vertex3.Y = Convert.ToDouble(Terrain.Vertices[x2 + 1, y2 + 1].Height * HeightMultiplier);
                        vertex3.Z = - (y2 + 1) * Constants.TerrainGridSpacing;
                        GL.Begin(BeginMode.LineLoop);
                        GL.Color3(1.0F, 0.0F, 0.0F);
                        GL.Vertex3(vertex0.X, vertex0.Y, Convert.ToDouble(- vertex0.Z));
                        GL.Vertex3(vertex1.X, vertex1.Y, Convert.ToDouble(- vertex1.Z));
                        GL.Vertex3(vertex3.X, vertex3.Y, Convert.ToDouble(- vertex3.Z));
                        GL.Vertex3(vertex2.X, vertex2.Y, Convert.ToDouble(- vertex2.Z));
                        GL.End();
                    }
                }
                DebugGLError("Gateways");
            }

            if ( MouseOverTerrain != null )
            {
                if ( modTools.Tool == modTools.Tools.ObjectSelect )
                {
                    if ( Unit_Selected_Area_VertexA != null )
                    {
                        //selection is changing under pointer
                        MathUtil.ReorderXY(Unit_Selected_Area_VertexA, MouseOverTerrain.Vertex.Normal, ref startXy, ref finishXy);
                        GL.LineWidth(2.0F);
                        GL.Color3(0.0F, 1.0F, 1.0F);
                        var x = 0;
                        for ( x = startXy.X; x <= finishXy.X - 1; x++ )
                        {
                            vertex0.X = x * Constants.TerrainGridSpacing;
                            vertex0.Y = Convert.ToDouble(Terrain.Vertices[x, startXy.Y].Height * HeightMultiplier);
                            vertex0.Z = - startXy.Y * Constants.TerrainGridSpacing;
                            vertex1.X = (x + 1) * Constants.TerrainGridSpacing;
                            vertex1.Y = Convert.ToDouble(Terrain.Vertices[x + 1, startXy.Y].Height * HeightMultiplier);
                            vertex1.Z = - startXy.Y * Constants.TerrainGridSpacing;
                            GL.Begin(BeginMode.Lines);
                            GL.Vertex3(vertex0.X, vertex0.Y, Convert.ToDouble(- vertex0.Z));
                            GL.Vertex3(vertex1.X, vertex1.Y, Convert.ToDouble(- vertex1.Z));
                            GL.End();
                        }
                        for ( x = startXy.X; x <= finishXy.X - 1; x++ )
                        {
                            vertex0.X = x * Constants.TerrainGridSpacing;
                            vertex0.Y = Convert.ToDouble(Terrain.Vertices[x, finishXy.Y].Height * HeightMultiplier);
                            vertex0.Z = - finishXy.Y * Constants.TerrainGridSpacing;
                            vertex1.X = (x + 1) * Constants.TerrainGridSpacing;
                            vertex1.Y = Convert.ToDouble(Terrain.Vertices[x + 1, finishXy.Y].Height * HeightMultiplier);
                            vertex1.Z = - finishXy.Y * Constants.TerrainGridSpacing;
                            GL.Begin(BeginMode.Lines);
                            GL.Vertex3(vertex0.X, vertex0.Y, Convert.ToDouble(- vertex0.Z));
                            GL.Vertex3(vertex1.X, vertex1.Y, Convert.ToDouble(- vertex1.Z));
                            GL.End();
                        }
                        var y = 0;
                        for ( y = startXy.Y; y <= finishXy.Y - 1; y++ )
                        {
                            vertex0.X = startXy.X * Constants.TerrainGridSpacing;
                            vertex0.Y = Convert.ToDouble(Terrain.Vertices[startXy.X, y].Height * HeightMultiplier);
                            vertex0.Z = - y * Constants.TerrainGridSpacing;
                            vertex1.X = startXy.X * Constants.TerrainGridSpacing;
                            vertex1.Y = Convert.ToDouble(Terrain.Vertices[startXy.X, y + 1].Height * HeightMultiplier);
                            vertex1.Z = - (y + 1) * Constants.TerrainGridSpacing;
                            GL.Begin(BeginMode.Lines);
                            GL.Vertex3(vertex0.X, vertex0.Y, Convert.ToDouble(- vertex0.Z));
                            GL.Vertex3(vertex1.X, vertex1.Y, Convert.ToDouble(- vertex1.Z));
                            GL.End();
                        }
                        for ( y = startXy.Y; y <= finishXy.Y - 1; y++ )
                        {
                            vertex0.X = finishXy.X * Constants.TerrainGridSpacing;
                            vertex0.Y = Convert.ToDouble(Terrain.Vertices[finishXy.X, y].Height * HeightMultiplier);
                            vertex0.Z = - y * Constants.TerrainGridSpacing;
                            vertex1.X = finishXy.X * Constants.TerrainGridSpacing;
                            vertex1.Y = Convert.ToDouble(Terrain.Vertices[finishXy.X, y + 1].Height * HeightMultiplier);
                            vertex1.Z = - (y + 1) * Constants.TerrainGridSpacing;
                            GL.Begin(BeginMode.Lines);
                            GL.Vertex3(vertex0.X, vertex0.Y, Convert.ToDouble(- vertex0.Z));
                            GL.Vertex3(vertex1.X, vertex1.Y, Convert.ToDouble(- vertex1.Z));
                            GL.End();
                        }

                        DebugGLError("Object selection box");
                    }
                    else
                    {
                        GL.LineWidth(2.0F);
                        GL.Color3(0.0F, 1.0F, 1.0F);
                        GL.Begin(BeginMode.Lines);
                        GL.Vertex3(MouseOverTerrain.Pos.Horizontal.X - 16.0D, MouseOverTerrain.Pos.Altitude, MouseOverTerrain.Pos.Horizontal.Y - 16.0D);
                        GL.Vertex3(MouseOverTerrain.Pos.Horizontal.X + 16.0D, MouseOverTerrain.Pos.Altitude, MouseOverTerrain.Pos.Horizontal.Y + 16.0D);
                        GL.Vertex3(MouseOverTerrain.Pos.Horizontal.X + 16.0D, MouseOverTerrain.Pos.Altitude, MouseOverTerrain.Pos.Horizontal.Y - 16.0D);
                        GL.Vertex3(MouseOverTerrain.Pos.Horizontal.X - 16.0D, MouseOverTerrain.Pos.Altitude, MouseOverTerrain.Pos.Horizontal.Y + 16.0D);
                        GL.End();

                        DebugGLError("Mouse over position");
                    }
                }

                if ( modTools.Tool == modTools.Tools.RoadPlace )
                {
                    GL.LineWidth(2.0F);

                    if ( MouseOverTerrain.Side_IsV )
                    {
                        vertex0.X = MouseOverTerrain.Side_Num.X * Constants.TerrainGridSpacing;
                        vertex0.Y = Convert.ToDouble(Terrain.Vertices[MouseOverTerrain.Side_Num.X, MouseOverTerrain.Side_Num.Y].Height * HeightMultiplier);
                        vertex0.Z = - MouseOverTerrain.Side_Num.Y * Constants.TerrainGridSpacing;
                        vertex1.X = MouseOverTerrain.Side_Num.X * Constants.TerrainGridSpacing;
                        vertex1.Y = Convert.ToDouble(Terrain.Vertices[MouseOverTerrain.Side_Num.X, MouseOverTerrain.Side_Num.Y + 1].Height * HeightMultiplier);
                        vertex1.Z = - (MouseOverTerrain.Side_Num.Y + 1) * Constants.TerrainGridSpacing;
                    }
                    else
                    {
                        vertex0.X = MouseOverTerrain.Side_Num.X * Constants.TerrainGridSpacing;
                        vertex0.Y = Convert.ToDouble(Terrain.Vertices[MouseOverTerrain.Side_Num.X, MouseOverTerrain.Side_Num.Y].Height * HeightMultiplier);
                        vertex0.Z = - MouseOverTerrain.Side_Num.Y * Constants.TerrainGridSpacing;
                        vertex1.X = (MouseOverTerrain.Side_Num.X + 1) * Constants.TerrainGridSpacing;
                        vertex1.Y = Convert.ToDouble(Terrain.Vertices[MouseOverTerrain.Side_Num.X + 1, MouseOverTerrain.Side_Num.Y].Height * HeightMultiplier);
                        vertex1.Z = - MouseOverTerrain.Side_Num.Y * Constants.TerrainGridSpacing;
                    }

                    GL.Begin(BeginMode.Lines);
                    GL.Color3(0.0F, 1.0F, 1.0F);
                    GL.Vertex3(vertex0.X, vertex0.Y, Convert.ToDouble(- vertex0.Z));
                    GL.Vertex3(vertex1.X, vertex1.Y, Convert.ToDouble(- vertex1.Z));
                    GL.End();

                    DebugGLError("Road place brush");
                }
                else if ( modTools.Tool == modTools.Tools.RoadLines || modTools.Tool == modTools.Tools.Gateways || modTools.Tool == modTools.Tools.ObjectLines )
                {
                    GL.LineWidth(2.0F);

                    if ( Selected_Tile_A != null )
                    {
                        x2 = Selected_Tile_A.X;
                        y2 = Selected_Tile_A.Y;

                        vertex0.X = x2 * Constants.TerrainGridSpacing;
                        vertex0.Y = Convert.ToDouble(Terrain.Vertices[x2, y2].Height * HeightMultiplier);
                        vertex0.Z = - y2 * Constants.TerrainGridSpacing;
                        vertex1.X = (x2 + 1) * Constants.TerrainGridSpacing;
                        vertex1.Y = Convert.ToDouble(Terrain.Vertices[x2 + 1, y2].Height * HeightMultiplier);
                        vertex1.Z = - y2 * Constants.TerrainGridSpacing;
                        vertex2.X = x2 * Constants.TerrainGridSpacing;
                        vertex2.Y = Convert.ToDouble(Terrain.Vertices[x2, y2 + 1].Height * HeightMultiplier);
                        vertex2.Z = - (y2 + 1) * Constants.TerrainGridSpacing;
                        vertex3.X = (x2 + 1) * Constants.TerrainGridSpacing;
                        vertex3.Y = Convert.ToDouble(Terrain.Vertices[x2 + 1, y2 + 1].Height * HeightMultiplier);
                        vertex3.Z = - (y2 + 1) * Constants.TerrainGridSpacing;
                        GL.Begin(BeginMode.LineLoop);
                        GL.Color3(0.0F, 1.0F, 1.0F);
                        GL.Vertex3(vertex0.X, vertex0.Y, Convert.ToDouble(- vertex0.Z));
                        GL.Vertex3(vertex1.X, vertex1.Y, Convert.ToDouble(- vertex1.Z));
                        GL.Vertex3(vertex3.X, vertex3.Y, Convert.ToDouble(- vertex3.Z));
                        GL.Vertex3(vertex2.X, vertex2.Y, Convert.ToDouble(- vertex2.Z));
                        GL.End();

                        if ( MouseOverTerrain.Tile.Normal.X == Selected_Tile_A.X )
                        {
                            if ( MouseOverTerrain.Tile.Normal.Y <= Selected_Tile_A.Y )
                            {
                                a = MouseOverTerrain.Tile.Normal.Y;
                                b = Selected_Tile_A.Y;
                            }
                            else
                            {
                                a = Selected_Tile_A.Y;
                                b = MouseOverTerrain.Tile.Normal.Y;
                            }
                            x2 = Selected_Tile_A.X;
                            for ( y2 = a; y2 <= b; y2++ )
                            {
                                vertex0.X = x2 * Constants.TerrainGridSpacing;
                                vertex0.Y = Convert.ToDouble(Terrain.Vertices[x2, y2].Height * HeightMultiplier);
                                vertex0.Z = - y2 * Constants.TerrainGridSpacing;
                                vertex1.X = (x2 + 1) * Constants.TerrainGridSpacing;
                                vertex1.Y = Convert.ToDouble(Terrain.Vertices[x2 + 1, y2].Height * HeightMultiplier);
                                vertex1.Z = - y2 * Constants.TerrainGridSpacing;
                                vertex2.X = x2 * Constants.TerrainGridSpacing;
                                vertex2.Y = Convert.ToDouble(Terrain.Vertices[x2, y2 + 1].Height * HeightMultiplier);
                                vertex2.Z = - (y2 + 1) * Constants.TerrainGridSpacing;
                                vertex3.X = (x2 + 1) * Constants.TerrainGridSpacing;
                                vertex3.Y = Convert.ToDouble(Terrain.Vertices[x2 + 1, y2 + 1].Height * HeightMultiplier);
                                vertex3.Z = - (y2 + 1) * Constants.TerrainGridSpacing;
                                GL.Begin(BeginMode.LineLoop);
                                GL.Color3(0.0F, 1.0F, 1.0F);
                                GL.Vertex3(vertex0.X, vertex0.Y, Convert.ToDouble(- vertex0.Z));
                                GL.Vertex3(vertex1.X, vertex1.Y, Convert.ToDouble(- vertex1.Z));
                                GL.Vertex3(vertex3.X, vertex3.Y, Convert.ToDouble(- vertex3.Z));
                                GL.Vertex3(vertex2.X, vertex2.Y, Convert.ToDouble(- vertex2.Z));
                                GL.End();
                            }
                        }
                        else if ( MouseOverTerrain.Tile.Normal.Y == Selected_Tile_A.Y )
                        {
                            if ( MouseOverTerrain.Tile.Normal.X <= Selected_Tile_A.X )
                            {
                                a = MouseOverTerrain.Tile.Normal.X;
                                b = Selected_Tile_A.X;
                            }
                            else
                            {
                                a = Selected_Tile_A.X;
                                b = MouseOverTerrain.Tile.Normal.X;
                            }
                            y2 = Selected_Tile_A.Y;
                            for ( x2 = a; x2 <= b; x2++ )
                            {
                                vertex0.X = x2 * Constants.TerrainGridSpacing;
                                vertex0.Y = Convert.ToDouble(Terrain.Vertices[x2, y2].Height * HeightMultiplier);
                                vertex0.Z = - y2 * Constants.TerrainGridSpacing;
                                vertex1.X = (x2 + 1) * Constants.TerrainGridSpacing;
                                vertex1.Y = Convert.ToDouble(Terrain.Vertices[x2 + 1, y2].Height * HeightMultiplier);
                                vertex1.Z = - y2 * Constants.TerrainGridSpacing;
                                vertex2.X = x2 * Constants.TerrainGridSpacing;
                                vertex2.Y = Convert.ToDouble(Terrain.Vertices[x2, y2 + 1].Height * HeightMultiplier);
                                vertex2.Z = - (y2 + 1) * Constants.TerrainGridSpacing;
                                vertex3.X = (x2 + 1) * Constants.TerrainGridSpacing;
                                vertex3.Y = Convert.ToDouble(Terrain.Vertices[x2 + 1, y2 + 1].Height * HeightMultiplier);
                                vertex3.Z = - (y2 + 1) * Constants.TerrainGridSpacing;
                                GL.Begin(BeginMode.LineLoop);
                                GL.Color3(0.0F, 1.0F, 1.0F);
                                GL.Vertex3(vertex0.X, vertex0.Y, Convert.ToDouble(- vertex0.Z));
                                GL.Vertex3(vertex1.X, vertex1.Y, Convert.ToDouble(- vertex1.Z));
                                GL.Vertex3(vertex3.X, vertex3.Y, Convert.ToDouble(- vertex3.Z));
                                GL.Vertex3(vertex2.X, vertex2.Y, Convert.ToDouble(- vertex2.Z));
                                GL.End();
                            }
                        }
                    }
                    else
                    {
                        x2 = MouseOverTerrain.Tile.Normal.X;
                        y2 = MouseOverTerrain.Tile.Normal.Y;

                        vertex0.X = x2 * Constants.TerrainGridSpacing;
                        vertex0.Y = Convert.ToDouble(Terrain.Vertices[x2, y2].Height * HeightMultiplier);
                        vertex0.Z = - y2 * Constants.TerrainGridSpacing;
                        vertex1.X = (x2 + 1) * Constants.TerrainGridSpacing;
                        vertex1.Y = Convert.ToDouble(Terrain.Vertices[x2 + 1, y2].Height * HeightMultiplier);
                        vertex1.Z = - y2 * Constants.TerrainGridSpacing;
                        vertex2.X = x2 * Constants.TerrainGridSpacing;
                        vertex2.Y = Convert.ToDouble(Terrain.Vertices[x2, y2 + 1].Height * HeightMultiplier);
                        vertex2.Z = - (y2 + 1) * Constants.TerrainGridSpacing;
                        vertex3.X = (x2 + 1) * Constants.TerrainGridSpacing;
                        vertex3.Y = Convert.ToDouble(Terrain.Vertices[x2 + 1, y2 + 1].Height * HeightMultiplier);
                        vertex3.Z = - (y2 + 1) * Constants.TerrainGridSpacing;
                        GL.Begin(BeginMode.LineLoop);
                        GL.Color3(0.0F, 1.0F, 1.0F);
                        GL.Vertex3(vertex0.X, vertex0.Y, Convert.ToDouble(- vertex0.Z));
                        GL.Vertex3(vertex1.X, vertex1.Y, Convert.ToDouble(- vertex1.Z));
                        GL.Vertex3(vertex3.X, vertex3.Y, Convert.ToDouble(- vertex3.Z));
                        GL.Vertex3(vertex2.X, vertex2.Y, Convert.ToDouble(- vertex2.Z));
                        GL.End();
                    }
                    DebugGLError("Line brush");
                }

                //draw mouseover tiles

                var ToolBrush = default(clsBrush);

                if ( modTools.Tool == modTools.Tools.TextureBrush )
                {
                    ToolBrush = App.TextureBrush;
                }
                else if ( modTools.Tool == modTools.Tools.CliffBrush )
                {
                    ToolBrush = App.CliffBrush;
                }
                else if ( modTools.Tool == modTools.Tools.CliffRemove )
                {
                    ToolBrush = App.CliffBrush;
                }
                else if ( modTools.Tool == modTools.Tools.RoadRemove )
                {
                    ToolBrush = App.CliffBrush;
                }
                else
                {
                    ToolBrush = null;
                }

                if ( ToolBrush != null )
                {
                    GL.LineWidth(2.0F);
                    var DrawTileOutline = new clsDrawTileOutline();
                    DrawTileOutline.Map = this;
                    DrawTileOutline.Colour.Red = 0.0F;
                    DrawTileOutline.Colour.Green = 1.0F;
                    DrawTileOutline.Colour.Blue = 1.0F;
                    DrawTileOutline.Colour.Alpha = 1.0F;
                    ToolBrush.PerformActionMapTiles(DrawTileOutline, MouseOverTerrain.Tile);

                    DebugGLError("Brush tiles");
                }

                //draw mouseover vertex
                if ( modTools.Tool == modTools.Tools.TerrainFill )
                {
                    GL.LineWidth(2.0F);

                    vertex0.X = MouseOverTerrain.Vertex.Normal.X * Constants.TerrainGridSpacing;
                    vertex0.Y = Convert.ToDouble(Terrain.Vertices[MouseOverTerrain.Vertex.Normal.X, MouseOverTerrain.Vertex.Normal.Y].Height * HeightMultiplier);
                    vertex0.Z = - MouseOverTerrain.Vertex.Normal.Y * Constants.TerrainGridSpacing;
                    GL.Begin(BeginMode.Lines);
                    GL.Color3(0.0F, 1.0F, 1.0F);
                    GL.Vertex3(vertex0.X - 8.0D, vertex0.Y, Convert.ToDouble(- vertex0.Z));
                    GL.Vertex3(vertex0.X + 8.0D, vertex0.Y, Convert.ToDouble(- vertex0.Z));
                    GL.Vertex3(vertex0.X, vertex0.Y, Convert.ToDouble(- vertex0.Z - 8.0D));
                    GL.Vertex3(vertex0.X, vertex0.Y, - vertex0.Z + 8.0D);
                    GL.End();

                    DebugGLError("Mouse over vertex");
                }

                if ( modTools.Tool == modTools.Tools.TerrainBrush )
                {
                    ToolBrush = App.TerrainBrush;
                }
                else if ( modTools.Tool == modTools.Tools.HeightSetBrush )
                {
                    ToolBrush = App.HeightBrush;
                }
                else if ( modTools.Tool == modTools.Tools.HeightChangeBrush )
                {
                    ToolBrush = App.HeightBrush;
                }
                else if ( modTools.Tool == modTools.Tools.HeightSmoothBrush )
                {
                    ToolBrush = App.HeightBrush;
                }
                else
                {
                    ToolBrush = null;
                }

                if ( ToolBrush != null )
                {
                    GL.LineWidth(2.0F);
                    var DrawVertexMarker = new clsDrawVertexMarker();
                    DrawVertexMarker.Map = this;
                    DrawVertexMarker.Colour.Red = 0.0F;
                    DrawVertexMarker.Colour.Green = 1.0F;
                    DrawVertexMarker.Colour.Blue = 1.0F;
                    DrawVertexMarker.Colour.Alpha = 1.0F;
                    ToolBrush.PerformActionMapVertices(DrawVertexMarker, MouseOverTerrain.Vertex);

                    DebugGLError("Brush vertices");
                }
            }

            GL.Enable(EnableCap.DepthTest);

            GL.Disable(EnableCap.CullFace);

            GL.LoadIdentity();
            GL.Rotate((float)(ViewInfo.ViewAngleRPY.Roll / MathUtil.RadOf1Deg), 0.0F, 0.0F, -1.0F);
            GL.Rotate((float)(ViewInfo.ViewAngleRPY.Pitch / MathUtil.RadOf1Deg), 1.0F, 0.0F, 0.0F);
            GL.Rotate((float)(ViewInfo.ViewAngleRPY.Yaw / MathUtil.RadOf1Deg), 0.0F, 1.0F, 0.0F);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            DebugGLError("Object matrix modes");

            if ( App.Draw_Units )
            {
                GL.Color3(1.0F, 1.0F, 1.0F);
                GL.Enable(EnableCap.Texture2D);
                App.VisionSectors.PerformActionMapSectors(DrawObjects, drawCentreSector);
                GL.Disable(EnableCap.Texture2D);
                DebugGLError("Objects");
            }

            if ( MouseOverTerrain != null )
            {
                GL.Enable(EnableCap.Texture2D);
                if ( modTools.Tool == modTools.Tools.ObjectPlace )
                {
                    var placeObject = Program.frmMainInstance.SingleSelectedObjectTypeBase;
                    if ( placeObject != null )
                    {
                        var rotation = 0;
                        try
                        {
                            IOUtil.InvariantParse(Program.frmMainInstance.txtNewObjectRotation.Text, ref rotation);
                            if ( rotation < 0 | rotation > 359 )
                            {
                                rotation = 0;
                            }
                        }
                        catch
                        {
                            rotation = 0;
                        }
                        WorldPos worldPos = TileAlignedPosFromMapPos(MouseOverTerrain.Pos.Horizontal, placeObject.GetGetFootprintSelected(rotation));
                        GL.PushMatrix();
                        GL.Translate(worldPos.Horizontal.X - ViewInfo.ViewPos.X, worldPos.Altitude - ViewInfo.ViewPos.Y + 2.0D,
                            ViewInfo.ViewPos.Z + worldPos.Horizontal.Y);
                        placeObject.GLDraw(rotation);
                        GL.PopMatrix();
                    }
                }
                GL.Disable(EnableCap.Texture2D);
                DebugGLError("Mouse over object");
            }

            GL.Disable(EnableCap.DepthTest);

            var ScriptMarkerTextLabels = new clsTextLabels(256);
            var TextLabel = default(clsTextLabel);
            if ( App.Draw_ScriptMarkers )
            {
                var ScriptPosition = default(clsScriptPosition);
                var ScriptArea = default(clsScriptArea);
                GL.PushMatrix();
                GL.Translate(Convert.ToDouble(- ViewInfo.ViewPos.X), Convert.ToDouble(- ViewInfo.ViewPos.Y), ViewInfo.ViewPos.Z);
                foreach ( var tempLoopVar_ScriptPosition in ScriptPositions )
                {
                    ScriptPosition = tempLoopVar_ScriptPosition;
                    ScriptPosition.GLDraw();
                }
                foreach ( var tempLoopVar_ScriptArea in ScriptAreas )
                {
                    ScriptArea = tempLoopVar_ScriptArea;
                    ScriptArea.GLDraw();
                }
                foreach ( var tempLoopVar_ScriptPosition in ScriptPositions )
                {
                    ScriptPosition = tempLoopVar_ScriptPosition;
                    if ( ScriptMarkerTextLabels.AtMaxCount() )
                    {
                        break;
                    }
                    xyzDbl.X = ScriptPosition.PosX - ViewInfo.ViewPos.X;
                    xyzDbl.Z = - ScriptPosition.PosY - ViewInfo.ViewPos.Z;
                    xyzDbl.Y = GetTerrainHeight(new XYInt(ScriptPosition.PosX, ScriptPosition.PosY)) - ViewInfo.ViewPos.Y;
                    Matrix3DMath.VectorRotationByMatrix(ViewInfo.ViewAngleMatrixInverted, xyzDbl, ref xyzDbl2);
                    if ( ViewInfo.PosGetScreenXY(xyzDbl2, ref screenPos) )
                    {
                        if ( screenPos.X >= 0 & screenPos.X <= glSize.X & screenPos.Y >= 0 & screenPos.Y <= glSize.Y )
                        {
                            TextLabel = new clsTextLabel();
                            TextLabel.Colour.Red = 1.0F;
                            TextLabel.Colour.Green = 1.0F;
                            TextLabel.Colour.Blue = 0.5F;
                            TextLabel.Colour.Alpha = 0.75F;
                            TextLabel.TextFont = App.UnitLabelFont;
                            TextLabel.SizeY = SettingsManager.Settings.FontSize;
                            TextLabel.Pos = screenPos;
                            TextLabel.Text = ScriptPosition.Label;
                            ScriptMarkerTextLabels.Add(TextLabel);
                        }
                    }
                }
                DebugGLError("Script positions");
                foreach ( var tempLoopVar_ScriptArea in ScriptAreas )
                {
                    ScriptArea = tempLoopVar_ScriptArea;
                    if ( ScriptMarkerTextLabels.AtMaxCount() )
                    {
                        break;
                    }
                    xyzDbl.X = ScriptArea.PosAX - ViewInfo.ViewPos.X;
                    xyzDbl.Z = - ScriptArea.PosAY - ViewInfo.ViewPos.Z;
                    xyzDbl.Y = GetTerrainHeight(new XYInt(ScriptArea.PosAX, ScriptArea.PosAY)) - ViewInfo.ViewPos.Y;
                    Matrix3DMath.VectorRotationByMatrix(ViewInfo.ViewAngleMatrixInverted, xyzDbl, ref xyzDbl2);
                    if ( ViewInfo.PosGetScreenXY(xyzDbl2, ref screenPos) )
                    {
                        if ( screenPos.X >= 0 & screenPos.X <= glSize.X & screenPos.Y >= 0 & screenPos.Y <= glSize.Y )
                        {
                            TextLabel = new clsTextLabel();
                            TextLabel.Colour.Red = 1.0F;
                            TextLabel.Colour.Green = 1.0F;
                            TextLabel.Colour.Blue = 0.5F;
                            TextLabel.Colour.Alpha = 0.75F;
                            TextLabel.TextFont = App.UnitLabelFont;
                            TextLabel.SizeY = SettingsManager.Settings.FontSize;
                            TextLabel.Pos = screenPos;
                            TextLabel.Text = ScriptArea.Label;
                            ScriptMarkerTextLabels.Add(TextLabel);
                        }
                    }
                }
                GL.PopMatrix();

                DebugGLError("Script areas");
            }

            var MessageTextLabels = new clsTextLabels(24);

            b = 0;
            for ( a = Math.Max(Messages.Count - MessageTextLabels.MaxCount, 0); a <= Messages.Count - 1; a++ )
            {
                if ( !MessageTextLabels.AtMaxCount() )
                {
                    TextLabel = new clsTextLabel();
                    TextLabel.Colour.Red = 0.875F;
                    TextLabel.Colour.Green = 0.875F;
                    TextLabel.Colour.Blue = 1.0F;
                    TextLabel.Colour.Alpha = 1.0F;
                    TextLabel.TextFont = App.UnitLabelFont;
                    TextLabel.SizeY = SettingsManager.Settings.FontSize;
                    TextLabel.Pos.X = 32 + minimapSizeXy.X;
                    TextLabel.Pos.Y = 32 + (int)(Math.Ceiling((decimal)(b * TextLabel.SizeY)));
                    TextLabel.Text = Convert.ToString(Messages[a].Text);
                    MessageTextLabels.Add(TextLabel);
                    b++;
                }
            }

            //draw unit selection

            GL.Begin(BeginMode.Quads);
            foreach ( var tempLoopVar_Unit in SelectedUnits )
            {
                unit = tempLoopVar_Unit;
                RGB_sng = GetUnitGroupColour(unit.UnitGroup);
                colourA = new sRGBA_sng((1.0F + RGB_sng.Red) / 2.0F, (1.0F + RGB_sng.Green) / 2.0F, (1.0F + RGB_sng.Blue) / 2.0F, 0.75F);
                colourB = new sRGBA_sng(RGB_sng.Red, RGB_sng.Green, RGB_sng.Blue, 0.75F);
                DrawUnitRectangle(unit, 8, colourA, colourB);
            }
            if ( MouseOverTerrain != null )
            {
                foreach ( var tempLoopVar_Unit in MouseOverTerrain.Units )
                {
                    unit = tempLoopVar_Unit;
                    if ( unit != null && modTools.Tool == modTools.Tools.ObjectSelect )
                    {
                        RGB_sng = GetUnitGroupColour(unit.UnitGroup);
                        GL.Color4((0.5F + RGB_sng.Red) / 1.5F, (0.5F + RGB_sng.Green) / 1.5F, (0.5F + RGB_sng.Blue) / 1.5F, 0.75F);
                        colourA = new sRGBA_sng((1.0F + RGB_sng.Red) / 2.0F, (1.0F + RGB_sng.Green) / 2.0F, (1.0F + RGB_sng.Blue) / 2.0F, 0.75F);
                        colourB = new sRGBA_sng(RGB_sng.Red, RGB_sng.Green, RGB_sng.Blue, 0.875F);
                        DrawUnitRectangle(unit, 16, colourA, colourB);
                    }
                }
            }
            GL.End();

            DebugGLError("Unit selection");

            GL.MatrixMode(MatrixMode.Projection);
            var temp_mat2 = Matrix4.CreateOrthographicOffCenter(0.0F, glSize.X, glSize.Y, 0.0F, -1.0F, 1.0F);
            GL.LoadMatrix(ref temp_mat2);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            DebugGLError("Text label matrix modes");

            GL.Enable(EnableCap.Texture2D);

            ScriptMarkerTextLabels.Draw();
            DrawObjects.UnitTextLabels.Draw();
            selectionLabel.Draw();
            MessageTextLabels.Draw();

            DebugGLError("Text labels");

            GL.Disable(EnableCap.Texture2D);

            GL.Disable(EnableCap.Blend);

            //draw minimap

            GL.MatrixMode(MatrixMode.Projection);
            var temp_mat3 = Matrix4.CreateOrthographicOffCenter(0.0F, glSize.X, 0.0F, glSize.Y, -1.0F, 1.0F);
            GL.LoadMatrix(ref temp_mat3);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            DebugGLError("Minimap matrix modes");

            if ( Minimap_Texture_Size > 0 & ViewInfo.TilesPerMinimapPixel > 0.0D )
            {
                GL.Translate(0.0F, glSize.Y - minimapSizeXy.Y, 0.0F);

                xyzDbl.X = (double)Terrain.TileSize.X / Minimap_Texture_Size;
                xyzDbl.Z = (double)Terrain.TileSize.Y / Minimap_Texture_Size;

                if ( Minimap_GLTexture > 0 )
                {
                    GL.Enable(EnableCap.Texture2D);
                    GL.BindTexture(TextureTarget.Texture2D, Minimap_GLTexture);
                    GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (int)TextureEnvMode.Decal);

                    GL.Begin(BeginMode.Quads);

                    GL.TexCoord2(0.0F, 0.0F);
                    GL.Vertex2(0, minimapSizeXy.Y);

                    GL.TexCoord2((float)xyzDbl.X, 0.0F);
                    GL.Vertex2(minimapSizeXy.X, minimapSizeXy.Y);

                    GL.TexCoord2((float)xyzDbl.X, (float)xyzDbl.Z);
                    GL.Vertex2(minimapSizeXy.X, 0);

                    GL.TexCoord2(0.0F, (float)xyzDbl.Z);
                    GL.Vertex2(0, 0);

                    GL.End();

                    GL.Disable(EnableCap.Texture2D);

                    DebugGLError("Minimap");
                }

                //draw minimap border

                GL.LineWidth(1.0F);
                GL.Begin(BeginMode.Lines);
                GL.Color3(0.75F, 0.75F, 0.75F);
                GL.Vertex2(minimapSizeXy.X, 0.0F);
                GL.Vertex2(minimapSizeXy.X, minimapSizeXy.Y);
                GL.Vertex2(0.0F, 0.0F);
                GL.Vertex2(minimapSizeXy.X, 0.0F);
                GL.End();

                DebugGLError("Minimap border");

                //draw minimap view pos box

                if ( showMinimapViewPosBox )
                {
                    dblTemp = Constants.TerrainGridSpacing * ViewInfo.TilesPerMinimapPixel;

                    posA.X = viewCorner0.X / dblTemp;
                    posA.Y = minimapSizeXy.Y + viewCorner0.Y / dblTemp;
                    posB.X = viewCorner1.X / dblTemp;
                    posB.Y = minimapSizeXy.Y + viewCorner1.Y / dblTemp;
                    posC.X = viewCorner2.X / dblTemp;
                    posC.Y = minimapSizeXy.Y + viewCorner2.Y / dblTemp;
                    posD.X = viewCorner3.X / dblTemp;
                    posD.Y = minimapSizeXy.Y + viewCorner3.Y / dblTemp;

                    GL.LineWidth(1.0F);
                    GL.Begin(BeginMode.LineLoop);
                    GL.Color3(1.0F, 1.0F, 1.0F);
                    GL.Vertex2(posA.X, posA.Y);
                    GL.Vertex2(posB.X, posB.Y);
                    GL.Vertex2(posC.X, posC.Y);
                    GL.Vertex2(posD.X, posD.Y);
                    GL.End();

                    DebugGLError("Minimap view position polygon");
                }

                if ( Selected_Area_VertexA != null )
                {
                    drawIt = false;
                    if ( Selected_Area_VertexB != null )
                    {
                        //area is selected
                        MathUtil.ReorderXY(Selected_Area_VertexA, Selected_Area_VertexB, ref startXy, ref finishXy);
                        drawIt = true;
                    }
                    else if ( modTools.Tool == modTools.Tools.TerrainSelect )
                    {
                        if ( MouseOverTerrain != null )
                        {
                            //selection is changing under mouse
                            MathUtil.ReorderXY(Selected_Area_VertexA, MouseOverTerrain.Vertex.Normal, ref startXy, ref finishXy);
                            drawIt = true;
                        }
                    }
                    if ( drawIt )
                    {
                        GL.LineWidth(1.0F);
                        posA.X = startXy.X / ViewInfo.TilesPerMinimapPixel;
                        posA.Y = minimapSizeXy.Y - startXy.Y / ViewInfo.TilesPerMinimapPixel;
                        posB.X = finishXy.X / ViewInfo.TilesPerMinimapPixel;
                        posB.Y = minimapSizeXy.Y - startXy.Y / ViewInfo.TilesPerMinimapPixel;
                        posC.X = finishXy.X / ViewInfo.TilesPerMinimapPixel;
                        posC.Y = minimapSizeXy.Y - finishXy.Y / ViewInfo.TilesPerMinimapPixel;
                        posD.X = startXy.X / ViewInfo.TilesPerMinimapPixel;
                        posD.Y = minimapSizeXy.Y - finishXy.Y / ViewInfo.TilesPerMinimapPixel;
                        GL.Begin(BeginMode.LineLoop);
                        GL.Color3(1.0F, 1.0F, 1.0F);
                        GL.Vertex2(posA.X, posA.Y);
                        GL.Vertex2(posB.X, posB.Y);
                        GL.Vertex2(posC.X, posC.Y);
                        GL.Vertex2(posD.X, posD.Y);
                        GL.End();

                        DebugGLError("Minimap selection box");
                    }
                }
            }
        }

        private void DebugGLError(string Name)
        {
            if ( App.DebugGL )
            {
                if ( Messages.Count < 8 )
                {
                    if ( GL.GetError() != ErrorCode.NoError )
                    {
                        var NewMessage = new clsMessage();
                        NewMessage.Text = "OpenGL Error (" + Name + ")";
                        Messages.Add(NewMessage);
                    }
                }
            }
        }

        public void DrawUnitRectangle(clsUnit Unit, int BorderInsideThickness, sRGBA_sng InsideColour, sRGBA_sng OutsideColour)
        {
            var PosA = new XYInt();
            var PosB = new XYInt();
            var A = 0;
            var Altitude = Unit.Pos.Altitude - ViewInfo.ViewPos.Y;

            GetFootprintTileRangeClamped(Unit.Pos.Horizontal, Unit.TypeBase.GetGetFootprintSelected(Unit.Rotation), ref PosA, ref PosB);
            A = PosA.Y;
            PosA.X = (int)((PosA.X + 0.125D) * Constants.TerrainGridSpacing - ViewInfo.ViewPos.X);
            PosA.Y = (int)((PosB.Y + 0.875D) * - Constants.TerrainGridSpacing - ViewInfo.ViewPos.Z);
            PosB.X = (int)((PosB.X + 0.875D) * Constants.TerrainGridSpacing - ViewInfo.ViewPos.X);
            PosB.Y = (int)((A + 0.125D) * - Constants.TerrainGridSpacing - ViewInfo.ViewPos.Z);

            GL.Color4(OutsideColour.Red, OutsideColour.Green, OutsideColour.Blue, OutsideColour.Alpha);
            GL.Vertex3(PosB.X, Altitude, Convert.ToInt32(- PosA.Y));
            GL.Vertex3(PosA.X, Altitude, Convert.ToInt32(- PosA.Y));
            GL.Color4(InsideColour.Red, InsideColour.Green, InsideColour.Blue, InsideColour.Alpha);
            GL.Vertex3(PosA.X + BorderInsideThickness, Altitude, Convert.ToInt32(- (PosA.Y + BorderInsideThickness)));
            GL.Vertex3(PosB.X - BorderInsideThickness, Altitude, Convert.ToInt32(- (PosA.Y + BorderInsideThickness)));

            GL.Color4(OutsideColour.Red, OutsideColour.Green, OutsideColour.Blue, OutsideColour.Alpha);
            GL.Vertex3(PosA.X, Altitude, Convert.ToInt32(- PosA.Y));
            GL.Vertex3(PosA.X, Altitude, Convert.ToInt32(- PosB.Y));
            GL.Color4(InsideColour.Red, InsideColour.Green, InsideColour.Blue, InsideColour.Alpha);
            GL.Vertex3(PosA.X + BorderInsideThickness, Altitude, Convert.ToInt32(- (PosB.Y - BorderInsideThickness)));
            GL.Vertex3(PosA.X + BorderInsideThickness, Altitude, Convert.ToInt32(- (PosA.Y + BorderInsideThickness)));

            GL.Color4(OutsideColour.Red, OutsideColour.Green, OutsideColour.Blue, OutsideColour.Alpha);
            GL.Vertex3(PosB.X, Altitude, Convert.ToInt32(- PosB.Y));
            GL.Vertex3(PosB.X, Altitude, Convert.ToInt32(- PosA.Y));
            GL.Color4(InsideColour.Red, InsideColour.Green, InsideColour.Blue, InsideColour.Alpha);
            GL.Vertex3(PosB.X - BorderInsideThickness, Altitude, - (PosA.Y + BorderInsideThickness));
            GL.Vertex3(PosB.X - BorderInsideThickness, Altitude, - (PosB.Y - BorderInsideThickness));

            GL.Color4(OutsideColour.Red, OutsideColour.Green, OutsideColour.Blue, OutsideColour.Alpha);
            GL.Vertex3(PosA.X, Altitude, Convert.ToInt32(- PosB.Y));
            GL.Vertex3(PosB.X, Altitude, Convert.ToInt32(- PosB.Y));
            GL.Color4(InsideColour.Red, InsideColour.Green, InsideColour.Blue, InsideColour.Alpha);
            GL.Vertex3(PosB.X - BorderInsideThickness, Altitude, - (PosB.Y - BorderInsideThickness));
            GL.Vertex3(PosA.X + BorderInsideThickness, Altitude, Convert.ToInt32(- (PosB.Y - BorderInsideThickness)));
        }
    }
}