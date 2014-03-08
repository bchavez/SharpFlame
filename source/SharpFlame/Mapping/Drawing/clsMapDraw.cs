#region

using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using SharpFlame.Colors;
using SharpFlame.Graphics.OpenGL;
using SharpFlame.Core;
using SharpFlame.Core.Domain;
using SharpFlame.Core.Domain.Colors;
using SharpFlame.FileIO;
using SharpFlame.Mapping.Drawing;
using SharpFlame.Mapping.Objects;
using SharpFlame.Mapping.Script;
using SharpFlame.Mapping.Tools;
using SharpFlame.Maths;
using SharpFlame.Util;

#endregion

namespace SharpFlame.Mapping
{
    public partial class Map
    {
        public ViewInfo ViewInfo;

        public void GLDraw()
        {
            var xyzDbl = default(XYZDouble);
            var x2 = 0;
            var y2 = 0;
            var a = 0;
            var b = 0;
            var D = 0;
            SRgba colourA;
            SRgba colourB;
            bool showMinimapViewPosBox;
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
            Unit unit;
            var startXy = new XYInt();
            var finishXy = new XYInt();
            bool drawIt;
            var drawCentreSector = new clsBrush.sPosNum();
            var selectionLabel = new clsTextLabel();
            var lightPosition = new float[4];
            var matrixB = new Matrix3DMath.Matrix3D();
            clsAction mapAction;
            float zNearFar = 0;
            var mapViewControl = App.MapViewGlSurface;
            var glSize = mapViewControl.Size;
            var drawCentre = default(XYDouble);

            dblTemp = App.SettingsManager.MinimapSize;
            ViewInfo.TilesPerMinimapPixel = Math.Sqrt(Terrain.TileSize.X * Terrain.TileSize.X + Terrain.TileSize.Y * Terrain.TileSize.Y) /
                                               (MathUtil.RootTwo * dblTemp);
            if ( MinimapTextureSize > 0 & ViewInfo.TilesPerMinimapPixel > 0.0D )
            {
                minimapSizeXy.X = (int)(Terrain.TileSize.X / ViewInfo.TilesPerMinimapPixel);
                minimapSizeXy.Y = (int)(Terrain.TileSize.Y / ViewInfo.TilesPerMinimapPixel);
            }
            if ( !ViewInfo.ScreenXYGetViewPlanePos(new XYInt((int)(glSize.Width / 2.0D), (int)(glSize.Height / 2.0D)), dblTemp, ref drawCentre) )
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

            var drawObjects = new clsDrawSectorObjects();
            drawObjects.Map = this;
            drawObjects.UnitTextLabels = new clsTextLabels(64);
            drawObjects.Start();

            xyzDbl.X = drawCentre.X - ViewInfo.ViewPos.X;
            xyzDbl.Y = 128 - ViewInfo.ViewPos.Y;
            xyzDbl.Z = - drawCentre.Y - ViewInfo.ViewPos.Z;
            zNearFar = (float)(xyzDbl.GetMagnitude());

            GL.Enable(EnableCap.DepthTest);
            GL.MatrixMode(MatrixMode.Projection);
            float aspectRatio = (float)glSize.Width / (float)glSize.Height;
            var temp_mat = Matrix4.CreatePerspectiveFieldOfView(ViewInfo.FieldOfViewY, aspectRatio, zNearFar / 128.0F, zNearFar * 128.0F);
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
                && ViewInfo.ScreenXYGetViewPlanePosForwardDownOnly(glSize.Width, 0, dblTemp, ref viewCorner1)
                && ViewInfo.ScreenXYGetViewPlanePosForwardDownOnly(glSize.Width, glSize.Height, dblTemp, ref viewCorner2)
                && ViewInfo.ScreenXYGetViewPlanePosForwardDownOnly(0, glSize.Height, dblTemp, ref viewCorner3) )
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

            debugGLError("Matrix modes");

            if ( App.Draw_TileTextures )
            {
                GL.Color3(1.0F, 1.0F, 1.0F);
                GL.Enable(EnableCap.Texture2D);
                mapAction = new clsDrawCallTerrain();
                mapAction.Map = this;
                App.VisionSectors.PerformActionMapSectors(mapAction, drawCentreSector);
                GL.Disable(EnableCap.Texture2D);

                debugGLError("Tile textures");
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

                debugGLError("Wireframe");
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

                debugGLError("Tile orientation");
            }

            //draw painted texture terrain type markers

            SRgb rgb;

            var mouseOverTerrain = ViewInfo.GetMouseOverTerrain();

            if ( App.Draw_VertexTerrain )
            {
                GL.LineWidth(1.0F);
                var DrawVertexTerran = new clsDrawVertexTerrain();
                DrawVertexTerran.Map = this;
                DrawVertexTerran.ViewAngleMatrix = ViewInfo.ViewAngleMatrix;
                App.VisionSectors.PerformActionMapSectors(DrawVertexTerran, drawCentreSector);
                debugGLError("Terrain type markers");
            }

            selectionLabel.Text = "";

            if ( SelectedAreaVertexA != null )
            {
                drawIt = false;
                if ( SelectedAreaVertexB != null )
                {
                    //area is selected
                    MathUtil.ReorderXY(SelectedAreaVertexA, SelectedAreaVertexB, ref startXy, ref finishXy);
                    xyzDbl.X = SelectedAreaVertexB.X * Constants.TerrainGridSpacing - ViewInfo.ViewPos.X;
                    xyzDbl.Z = - SelectedAreaVertexB.Y * Constants.TerrainGridSpacing - ViewInfo.ViewPos.Z;
                    xyzDbl.Y = GetVertexAltitude(SelectedAreaVertexB) - ViewInfo.ViewPos.Y;
                    drawIt = true;
                }
                else if ( modTools.Tool == modTools.Tools.TerrainSelect )
                {
                    if ( mouseOverTerrain != null )
                    {
                        //selection is changing under pointer
                        MathUtil.ReorderXY(SelectedAreaVertexA, mouseOverTerrain.Vertex.Normal, ref startXy, ref finishXy);
                        xyzDbl.X = mouseOverTerrain.Vertex.Normal.X * Constants.TerrainGridSpacing - ViewInfo.ViewPos.X;
                        xyzDbl.Z = - mouseOverTerrain.Vertex.Normal.Y * Constants.TerrainGridSpacing - ViewInfo.ViewPos.Z;
                        xyzDbl.Y = GetVertexAltitude(mouseOverTerrain.Vertex.Normal) - ViewInfo.ViewPos.Y;
                        drawIt = true;
                    }
                }
                if ( drawIt )
                {
                    Matrix3DMath.VectorRotationByMatrix(ViewInfo.ViewAngleMatrixInverted, xyzDbl, ref xyzDbl2);
                    if ( ViewInfo.PosGetScreenXY(xyzDbl2, ref screenPos) )
                    {
                        if ( screenPos.X >= 0 & screenPos.X <= glSize.Width & screenPos.Y >= 0 & screenPos.Y <= glSize.Height )
                        {
                            selectionLabel.Colour.Red = 1.0F;
                            selectionLabel.Colour.Green = 1.0F;
                            selectionLabel.Colour.Blue = 1.0F;
                            selectionLabel.Colour.Alpha = 1.0F;
                            selectionLabel.TextFont = App.UnitLabelFont;
                            selectionLabel.SizeY = App.SettingsManager.FontSize;
                            selectionLabel.Pos = screenPos;
                            selectionLabel.Text = finishXy.X - startXy.X + "x" + Convert.ToString(finishXy.Y - startXy.Y);
                        }
                    }
                    GL.LineWidth(3.0F);
                    var drawSelection = new clsDrawTileAreaOutline
                        {
                            Map = this, 
                            StartXY = startXy,
                            FinishXY = finishXy,
                            Colour = new SRgba(1.0F, 1.0F, 1.0F, 1.0F)
                        };
                    drawSelection.ActionPerform();
                }

                debugGLError("Terrain selection box");
            }

            if ( modTools.Tool == modTools.Tools.TerrainSelect )
            {
                if ( mouseOverTerrain != null )
                {
                    //draw mouseover vertex
                    GL.LineWidth(3.0F);

                    vertex0.X = mouseOverTerrain.Vertex.Normal.X * Constants.TerrainGridSpacing;
                    vertex0.Y = Convert.ToDouble(Terrain.Vertices[mouseOverTerrain.Vertex.Normal.X, mouseOverTerrain.Vertex.Normal.Y].Height * HeightMultiplier);
                    vertex0.Z = - mouseOverTerrain.Vertex.Normal.Y * Constants.TerrainGridSpacing;
                    GL.Begin(BeginMode.Lines);
                    GL.Color3(1.0F, 1.0F, 1.0F);
                    GL.Vertex3(vertex0.X - 8.0D, vertex0.Y, Convert.ToDouble(- vertex0.Z));
                    GL.Vertex3(vertex0.X + 8.0D, vertex0.Y, Convert.ToDouble(- vertex0.Z));
                    GL.Vertex3(vertex0.X, vertex0.Y, - vertex0.Z - 8.0D);
                    GL.Vertex3(vertex0.X, vertex0.Y, - vertex0.Z + 8.0D);
                    GL.End();
                }
                debugGLError("Terrain selection vertex");
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
                debugGLError("Gateways");
            }

            if ( mouseOverTerrain != null )
            {
                if ( modTools.Tool == modTools.Tools.ObjectSelect )
                {
                    if ( UnitSelectedAreaVertexA != null )
                    {
                        //selection is changing under pointer
                        MathUtil.ReorderXY(UnitSelectedAreaVertexA, mouseOverTerrain.Vertex.Normal, ref startXy, ref finishXy);
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

                        debugGLError("Object selection box");
                    }
                    else
                    {
                        GL.LineWidth(2.0F);
                        GL.Color3(0.0F, 1.0F, 1.0F);
                        GL.Begin(BeginMode.Lines);
                        GL.Vertex3(mouseOverTerrain.Pos.Horizontal.X - 16.0D, mouseOverTerrain.Pos.Altitude, mouseOverTerrain.Pos.Horizontal.Y - 16.0D);
                        GL.Vertex3(mouseOverTerrain.Pos.Horizontal.X + 16.0D, mouseOverTerrain.Pos.Altitude, mouseOverTerrain.Pos.Horizontal.Y + 16.0D);
                        GL.Vertex3(mouseOverTerrain.Pos.Horizontal.X + 16.0D, mouseOverTerrain.Pos.Altitude, mouseOverTerrain.Pos.Horizontal.Y - 16.0D);
                        GL.Vertex3(mouseOverTerrain.Pos.Horizontal.X - 16.0D, mouseOverTerrain.Pos.Altitude, mouseOverTerrain.Pos.Horizontal.Y + 16.0D);
                        GL.End();

                        debugGLError("Mouse over position");
                    }
                }

                if ( modTools.Tool == modTools.Tools.RoadPlace )
                {
                    GL.LineWidth(2.0F);

                    if ( mouseOverTerrain.SideIsV )
                    {
                        vertex0.X = mouseOverTerrain.SideNum.X * Constants.TerrainGridSpacing;
                        vertex0.Y = Convert.ToDouble(Terrain.Vertices[mouseOverTerrain.SideNum.X, mouseOverTerrain.SideNum.Y].Height * HeightMultiplier);
                        vertex0.Z = - mouseOverTerrain.SideNum.Y * Constants.TerrainGridSpacing;
                        vertex1.X = mouseOverTerrain.SideNum.X * Constants.TerrainGridSpacing;
                        vertex1.Y = Convert.ToDouble(Terrain.Vertices[mouseOverTerrain.SideNum.X, mouseOverTerrain.SideNum.Y + 1].Height * HeightMultiplier);
                        vertex1.Z = - (mouseOverTerrain.SideNum.Y + 1) * Constants.TerrainGridSpacing;
                    }
                    else
                    {
                        vertex0.X = mouseOverTerrain.SideNum.X * Constants.TerrainGridSpacing;
                        vertex0.Y = Convert.ToDouble(Terrain.Vertices[mouseOverTerrain.SideNum.X, mouseOverTerrain.SideNum.Y].Height * HeightMultiplier);
                        vertex0.Z = - mouseOverTerrain.SideNum.Y * Constants.TerrainGridSpacing;
                        vertex1.X = (mouseOverTerrain.SideNum.X + 1) * Constants.TerrainGridSpacing;
                        vertex1.Y = Convert.ToDouble(Terrain.Vertices[mouseOverTerrain.SideNum.X + 1, mouseOverTerrain.SideNum.Y].Height * HeightMultiplier);
                        vertex1.Z = - mouseOverTerrain.SideNum.Y * Constants.TerrainGridSpacing;
                    }

                    GL.Begin(BeginMode.Lines);
                    GL.Color3(0.0F, 1.0F, 1.0F);
                    GL.Vertex3(vertex0.X, vertex0.Y, Convert.ToDouble(- vertex0.Z));
                    GL.Vertex3(vertex1.X, vertex1.Y, Convert.ToDouble(- vertex1.Z));
                    GL.End();

                    debugGLError("Road place brush");
                }
                else if ( modTools.Tool == modTools.Tools.RoadLines || modTools.Tool == modTools.Tools.Gateways || modTools.Tool == modTools.Tools.ObjectLines )
                {
                    GL.LineWidth(2.0F);

                    if ( SelectedTileA != null )
                    {
                        x2 = SelectedTileA.X;
                        y2 = SelectedTileA.Y;

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

                        if ( mouseOverTerrain.Tile.Normal.X == SelectedTileA.X )
                        {
                            if ( mouseOverTerrain.Tile.Normal.Y <= SelectedTileA.Y )
                            {
                                a = mouseOverTerrain.Tile.Normal.Y;
                                b = SelectedTileA.Y;
                            }
                            else
                            {
                                a = SelectedTileA.Y;
                                b = mouseOverTerrain.Tile.Normal.Y;
                            }
                            x2 = SelectedTileA.X;
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
                        else if ( mouseOverTerrain.Tile.Normal.Y == SelectedTileA.Y )
                        {
                            if ( mouseOverTerrain.Tile.Normal.X <= SelectedTileA.X )
                            {
                                a = mouseOverTerrain.Tile.Normal.X;
                                b = SelectedTileA.X;
                            }
                            else
                            {
                                a = SelectedTileA.X;
                                b = mouseOverTerrain.Tile.Normal.X;
                            }
                            y2 = SelectedTileA.Y;
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
                        x2 = mouseOverTerrain.Tile.Normal.X;
                        y2 = mouseOverTerrain.Tile.Normal.Y;

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
                    debugGLError("Line brush");
                }

                //draw mouseover tiles

                var toolBrush = default(clsBrush);

                if ( modTools.Tool == modTools.Tools.TextureBrush )
                {
                    toolBrush = App.TextureBrush;
                }
                else if ( modTools.Tool == modTools.Tools.CliffBrush )
                {
                    toolBrush = App.CliffBrush;
                }
                else if ( modTools.Tool == modTools.Tools.CliffRemove )
                {
                    toolBrush = App.CliffBrush;
                }
                else if ( modTools.Tool == modTools.Tools.RoadRemove )
                {
                    toolBrush = App.CliffBrush;
                }
                else
                {
                    toolBrush = null;
                }

                if ( toolBrush != null )
                {
                    GL.LineWidth(2.0F);
                    var drawTileOutline = new clsDrawTileOutline
                        {
                            Map = this, 
                            Colour = {Red = 0.0F, Green = 1.0F, Blue = 1.0F, Alpha = 1.0F}
                        };
                    toolBrush.PerformActionMapTiles(drawTileOutline, mouseOverTerrain.Tile);

                    debugGLError("Brush tiles");
                }

                //draw mouseover vertex
                if ( modTools.Tool == modTools.Tools.TerrainFill )
                {
                    GL.LineWidth(2.0F);

                    vertex0.X = mouseOverTerrain.Vertex.Normal.X * Constants.TerrainGridSpacing;
                    vertex0.Y = Convert.ToDouble(Terrain.Vertices[mouseOverTerrain.Vertex.Normal.X, mouseOverTerrain.Vertex.Normal.Y].Height * HeightMultiplier);
                    vertex0.Z = - mouseOverTerrain.Vertex.Normal.Y * Constants.TerrainGridSpacing;
                    GL.Begin(BeginMode.Lines);
                    GL.Color3(0.0F, 1.0F, 1.0F);
                    GL.Vertex3(vertex0.X - 8.0D, vertex0.Y, Convert.ToDouble(- vertex0.Z));
                    GL.Vertex3(vertex0.X + 8.0D, vertex0.Y, Convert.ToDouble(- vertex0.Z));
                    GL.Vertex3(vertex0.X, vertex0.Y, Convert.ToDouble(- vertex0.Z - 8.0D));
                    GL.Vertex3(vertex0.X, vertex0.Y, - vertex0.Z + 8.0D);
                    GL.End();

                    debugGLError("Mouse over vertex");
                }

                if ( modTools.Tool == modTools.Tools.TerrainBrush )
                {
                    toolBrush = App.TerrainBrush;
                }
                else if ( modTools.Tool == modTools.Tools.HeightSetBrush )
                {
                    toolBrush = App.HeightBrush;
                }
                else if ( modTools.Tool == modTools.Tools.HeightChangeBrush )
                {
                    toolBrush = App.HeightBrush;
                }
                else if ( modTools.Tool == modTools.Tools.HeightSmoothBrush )
                {
                    toolBrush = App.HeightBrush;
                }
                else
                {
                    toolBrush = null;
                }

                if ( toolBrush != null )
                {
                    GL.LineWidth(2.0F);
                    var drawVertexMarker = new clsDrawVertexMarker
                        {
                            Map = this, 
                            Colour = {Red = 0.0F, Green = 1.0F, Blue = 1.0F, Alpha = 1.0F}
                        };
                    toolBrush.PerformActionMapVertices(drawVertexMarker, mouseOverTerrain.Vertex);

                    debugGLError("Brush vertices");
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

            debugGLError("Object matrix modes");

            if ( App.Draw_Units )
            {
                GL.Color3(1.0F, 1.0F, 1.0F);
                GL.Enable(EnableCap.Texture2D);
                App.VisionSectors.PerformActionMapSectors(drawObjects, drawCentreSector);
                GL.Disable(EnableCap.Texture2D);
                debugGLError("Objects");
            }

            if ( mouseOverTerrain != null )
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
                        WorldPos worldPos = TileAlignedPosFromMapPos(mouseOverTerrain.Pos.Horizontal, placeObject.GetGetFootprintSelected(rotation));
                        GL.PushMatrix();
                        GL.Translate(worldPos.Horizontal.X - ViewInfo.ViewPos.X, worldPos.Altitude - ViewInfo.ViewPos.Y + 2.0D,
                            ViewInfo.ViewPos.Z + worldPos.Horizontal.Y);
                        placeObject.GLDraw(rotation);
                        GL.PopMatrix();
                    }
                }
                GL.Disable(EnableCap.Texture2D);
                debugGLError("Mouse over object");
            }

            GL.Disable(EnableCap.DepthTest);

            var scriptMarkerTextLabels = new clsTextLabels(256);
            clsTextLabel textLabel;
            if ( App.Draw_ScriptMarkers )
            {
                clsScriptPosition scriptPosition;
                clsScriptArea scriptArea;
                GL.PushMatrix();
                GL.Translate(Convert.ToDouble(- ViewInfo.ViewPos.X), Convert.ToDouble(- ViewInfo.ViewPos.Y), ViewInfo.ViewPos.Z);
                foreach ( var tempLoopVar_ScriptPosition in ScriptPositions )
                {
                    scriptPosition = tempLoopVar_ScriptPosition;
                    scriptPosition.GLDraw();
                }
                foreach ( var tempLoopVar_ScriptArea in ScriptAreas )
                {
                    scriptArea = tempLoopVar_ScriptArea;
                    scriptArea.GLDraw();
                }
                foreach ( var tempLoopVar_ScriptPosition in ScriptPositions )
                {
                    scriptPosition = tempLoopVar_ScriptPosition;
                    if ( scriptMarkerTextLabels.AtMaxCount() )
                    {
                        break;
                    }
                    xyzDbl.X = scriptPosition.PosX - ViewInfo.ViewPos.X;
                    xyzDbl.Z = - scriptPosition.PosY - ViewInfo.ViewPos.Z;
                    xyzDbl.Y = GetTerrainHeight(new XYInt(scriptPosition.PosX, scriptPosition.PosY)) - ViewInfo.ViewPos.Y;
                    Matrix3DMath.VectorRotationByMatrix(ViewInfo.ViewAngleMatrixInverted, xyzDbl, ref xyzDbl2);
                    if ( ViewInfo.PosGetScreenXY(xyzDbl2, ref screenPos) )
                    {
                        if ( screenPos.X >= 0 & screenPos.X <= glSize.Width & screenPos.Y >= 0 & screenPos.Y <= glSize.Height )
                        {
                            textLabel = new clsTextLabel();
                            textLabel.Colour.Red = 1.0F;
                            textLabel.Colour.Green = 1.0F;
                            textLabel.Colour.Blue = 0.5F;
                            textLabel.Colour.Alpha = 0.75F;
                            textLabel.TextFont = App.UnitLabelFont;
                            textLabel.SizeY = App.SettingsManager.FontSize;
                            textLabel.Pos = screenPos;
                            textLabel.Text = scriptPosition.Label;
                            scriptMarkerTextLabels.Add(textLabel);
                        }
                    }
                }
                debugGLError("Script positions");
                foreach ( var tempLoopVar_ScriptArea in ScriptAreas )
                {
                    scriptArea = tempLoopVar_ScriptArea;
                    if ( scriptMarkerTextLabels.AtMaxCount() )
                    {
                        break;
                    }
                    xyzDbl.X = scriptArea.PosAX - ViewInfo.ViewPos.X;
                    xyzDbl.Z = - scriptArea.PosAY - ViewInfo.ViewPos.Z;
                    xyzDbl.Y = GetTerrainHeight(new XYInt(scriptArea.PosAX, scriptArea.PosAY)) - ViewInfo.ViewPos.Y;
                    Matrix3DMath.VectorRotationByMatrix(ViewInfo.ViewAngleMatrixInverted, xyzDbl, ref xyzDbl2);
                    if ( ViewInfo.PosGetScreenXY(xyzDbl2, ref screenPos) )
                    {
                        if ( screenPos.X >= 0 & screenPos.X <= glSize.Width & screenPos.Y >= 0 & screenPos.Y <= glSize.Height )
                        {
                            textLabel = new clsTextLabel();
                            textLabel.Colour.Red = 1.0F;
                            textLabel.Colour.Green = 1.0F;
                            textLabel.Colour.Blue = 0.5F;
                            textLabel.Colour.Alpha = 0.75F;
                            textLabel.TextFont = App.UnitLabelFont;
                            textLabel.SizeY = App.SettingsManager.FontSize;
                            textLabel.Pos = screenPos;
                            textLabel.Text = scriptArea.Label;
                            scriptMarkerTextLabels.Add(textLabel);
                        }
                    }
                }
                GL.PopMatrix();

                debugGLError("Script areas");
            }

            var messageTextLabels = new clsTextLabels(24);

            b = 0;
            for ( a = Math.Max(Messages.Count - messageTextLabels.MaxCount, 0); a <= Messages.Count - 1; a++ )
            {
                if ( !messageTextLabels.AtMaxCount() )
                {
                    textLabel = new clsTextLabel();
                    textLabel.Colour.Red = 0.875F;
                    textLabel.Colour.Green = 0.875F;
                    textLabel.Colour.Blue = 1.0F;
                    textLabel.Colour.Alpha = 1.0F;
                    textLabel.TextFont = App.UnitLabelFont;
                    textLabel.SizeY = App.SettingsManager.FontSize;
                    textLabel.Pos.X = 32 + minimapSizeXy.X;
                    textLabel.Pos.Y = 32 + (int)(Math.Ceiling((decimal)(b * textLabel.SizeY)));
                    textLabel.Text = Convert.ToString(Messages[a].Text);
                    messageTextLabels.Add(textLabel);
                    b++;
                }
            }

            //draw unit selection

            GL.Begin(BeginMode.Quads);
            foreach ( var tempLoopVar_Unit in SelectedUnits )
            {
                unit = tempLoopVar_Unit;
                rgb = GetUnitGroupColour(unit.UnitGroup);
                colourA = new SRgba((1.0F + rgb.Red) / 2.0F, (1.0F + rgb.Green) / 2.0F, (1.0F + rgb.Blue) / 2.0F, 0.75F);
                colourB = new SRgba(rgb.Red, rgb.Green, rgb.Blue, 0.75F);
                DrawUnitRectangle(unit, 8, colourA, colourB);
            }
            if ( mouseOverTerrain != null )
            {
                foreach ( var tempLoopVar_Unit in mouseOverTerrain.Units )
                {
                    unit = tempLoopVar_Unit;
                    if ( unit != null && modTools.Tool == modTools.Tools.ObjectSelect )
                    {
                        rgb = GetUnitGroupColour(unit.UnitGroup);
                        GL.Color4((0.5F + rgb.Red) / 1.5F, (0.5F + rgb.Green) / 1.5F, (0.5F + rgb.Blue) / 1.5F, 0.75F);
                        colourA = new SRgba((1.0F + rgb.Red) / 2.0F, (1.0F + rgb.Green) / 2.0F, (1.0F + rgb.Blue) / 2.0F, 0.75F);
                        colourB = new SRgba(rgb.Red, rgb.Green, rgb.Blue, 0.875F);
                        DrawUnitRectangle(unit, 16, colourA, colourB);
                    }
                }
            }
            GL.End();

            debugGLError("Unit selection");

            GL.MatrixMode(MatrixMode.Projection);
            var tempMat2 = Matrix4.CreateOrthographicOffCenter(0.0F, glSize.Width, glSize.Height, 0.0F, -1.0F, 1.0F);
            GL.LoadMatrix(ref tempMat2);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            debugGLError("Text label matrix modes");

            GL.Enable(EnableCap.Texture2D);

            scriptMarkerTextLabels.Draw();
            drawObjects.UnitTextLabels.Draw();
            selectionLabel.Draw();
            messageTextLabels.Draw();

            debugGLError("Text labels");

            GL.Disable(EnableCap.Texture2D);

            GL.Disable(EnableCap.Blend);

            //draw minimap

            GL.MatrixMode(MatrixMode.Projection);
            var tempMat3 = Matrix4.CreateOrthographicOffCenter(0.0F, glSize.Width, 0.0F, glSize.Height, -1.0F, 1.0F);
            GL.LoadMatrix(ref tempMat3);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            debugGLError("Minimap matrix modes");

            if ( MinimapTextureSize > 0 & ViewInfo.TilesPerMinimapPixel > 0.0D )
            {
                GL.Translate(0.0F, glSize.Height - minimapSizeXy.Y, 0.0F);

                xyzDbl.X = (double)Terrain.TileSize.X / MinimapTextureSize;
                xyzDbl.Z = (double)Terrain.TileSize.Y / MinimapTextureSize;

                if ( MinimapGlTexture > 0 )
                {
                    GL.Enable(EnableCap.Texture2D);
                    GL.BindTexture(TextureTarget.Texture2D, MinimapGlTexture);
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

                    debugGLError("Minimap");
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

                debugGLError("Minimap border");

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

                    debugGLError("Minimap view position polygon");
                }

                if ( SelectedAreaVertexA != null )
                {
                    drawIt = false;
                    if ( SelectedAreaVertexB != null )
                    {
                        //area is selected
                        MathUtil.ReorderXY(SelectedAreaVertexA, SelectedAreaVertexB, ref startXy, ref finishXy);
                        drawIt = true;
                    }
                    else if ( modTools.Tool == modTools.Tools.TerrainSelect )
                    {
                        if ( mouseOverTerrain != null )
                        {
                            //selection is changing under mouse
                            MathUtil.ReorderXY(SelectedAreaVertexA, mouseOverTerrain.Vertex.Normal, ref startXy, ref finishXy);
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

                        debugGLError("Minimap selection box");
                    }
                }
            }
        }

        private void debugGLError(string Name)
        {
            if ( App.DebugGL )
            {
                if ( Messages.Count < 8 )
                {
                    if ( GL.GetError() != ErrorCode.NoError )
                    {
                        var NewMessage = new Message();
                        NewMessage.Text = "OpenGL Error (" + Name + ")";
                        Messages.Add(NewMessage);
                        logger.Error(NewMessage.Text);
                    }
                }
            }
        }

        public void DrawUnitRectangle(Unit Unit, int BorderInsideThickness, SRgba InsideColour, SRgba OutsideColour)
        {
            var posA = new XYInt();
            var posB = new XYInt();
            var a = 0;
            var altitude = Unit.Pos.Altitude - ViewInfo.ViewPos.Y;

            GetFootprintTileRangeClamped(Unit.Pos.Horizontal, Unit.TypeBase.GetGetFootprintSelected(Unit.Rotation), ref posA, ref posB);
            a = posA.Y;
            posA.X = (int)((posA.X + 0.125D) * Constants.TerrainGridSpacing - ViewInfo.ViewPos.X);
            posA.Y = (int)((posB.Y + 0.875D) * - Constants.TerrainGridSpacing - ViewInfo.ViewPos.Z);
            posB.X = (int)((posB.X + 0.875D) * Constants.TerrainGridSpacing - ViewInfo.ViewPos.X);
            posB.Y = (int)((a + 0.125D) * - Constants.TerrainGridSpacing - ViewInfo.ViewPos.Z);

            GL.Color4(OutsideColour.Red, OutsideColour.Green, OutsideColour.Blue, OutsideColour.Alpha);
            GL.Vertex3(posB.X, altitude, Convert.ToInt32(- posA.Y));
            GL.Vertex3(posA.X, altitude, Convert.ToInt32(- posA.Y));
            GL.Color4(InsideColour.Red, InsideColour.Green, InsideColour.Blue, InsideColour.Alpha);
            GL.Vertex3(posA.X + BorderInsideThickness, altitude, Convert.ToInt32(- (posA.Y + BorderInsideThickness)));
            GL.Vertex3(posB.X - BorderInsideThickness, altitude, Convert.ToInt32(- (posA.Y + BorderInsideThickness)));

            GL.Color4(OutsideColour.Red, OutsideColour.Green, OutsideColour.Blue, OutsideColour.Alpha);
            GL.Vertex3(posA.X, altitude, Convert.ToInt32(- posA.Y));
            GL.Vertex3(posA.X, altitude, Convert.ToInt32(- posB.Y));
            GL.Color4(InsideColour.Red, InsideColour.Green, InsideColour.Blue, InsideColour.Alpha);
            GL.Vertex3(posA.X + BorderInsideThickness, altitude, Convert.ToInt32(- (posB.Y - BorderInsideThickness)));
            GL.Vertex3(posA.X + BorderInsideThickness, altitude, Convert.ToInt32(- (posA.Y + BorderInsideThickness)));

            GL.Color4(OutsideColour.Red, OutsideColour.Green, OutsideColour.Blue, OutsideColour.Alpha);
            GL.Vertex3(posB.X, altitude, Convert.ToInt32(- posB.Y));
            GL.Vertex3(posB.X, altitude, Convert.ToInt32(- posA.Y));
            GL.Color4(InsideColour.Red, InsideColour.Green, InsideColour.Blue, InsideColour.Alpha);
            GL.Vertex3(posB.X - BorderInsideThickness, altitude, - (posA.Y + BorderInsideThickness));
            GL.Vertex3(posB.X - BorderInsideThickness, altitude, - (posB.Y - BorderInsideThickness));

            GL.Color4(OutsideColour.Red, OutsideColour.Green, OutsideColour.Blue, OutsideColour.Alpha);
            GL.Vertex3(posA.X, altitude, Convert.ToInt32(- posB.Y));
            GL.Vertex3(posB.X, altitude, Convert.ToInt32(- posB.Y));
            GL.Color4(InsideColour.Red, InsideColour.Green, InsideColour.Blue, InsideColour.Alpha);
            GL.Vertex3(posB.X - BorderInsideThickness, altitude, - (posB.Y - BorderInsideThickness));
            GL.Vertex3(posA.X + BorderInsideThickness, altitude, Convert.ToInt32(- (posB.Y - BorderInsideThickness)));
        }
    }
}