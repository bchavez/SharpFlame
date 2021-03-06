

using System;
using Eto.Drawing;
using Ninject;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using SharpFlame.Colors;
using SharpFlame.Core.Extensions;
using SharpFlame.Graphics.OpenGL;
using SharpFlame.Core;
using SharpFlame.Core.Domain;
using SharpFlame.Core.Domain.Colors;
using SharpFlame.FileIO;
using SharpFlame.Mapping.Drawing;
using SharpFlame.Mapping.Minimap;
using SharpFlame.Mapping.Objects;
using SharpFlame.Mapping.Script;
using SharpFlame.Mapping.Tools;
using SharpFlame.Maths;
using SharpFlame.MouseTools;
using SharpFlame.Util;


namespace SharpFlame.Mapping
{
    public class DrawContext
    {
        public MinimapGl MinimapGl { get; set; }
        public Size GlSize { get; set; }
        public ViewInfo ViewInfo { get; set; }
		public ToolOptions ToolOptions { get; set; }
    }
    public partial class Map
    {
        public void GLDraw(DrawContext ctx)
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
            var glSize = ctx.GlSize;
            var drawCentre = default(XYDouble);

            dblTemp = App.SettingsManager.MinimapSize;
            ctx.ViewInfo.TilesPerMinimapPixel = Math.Sqrt(Terrain.TileSize.X * Terrain.TileSize.X + Terrain.TileSize.Y * Terrain.TileSize.Y) /
                                               (MathUtil.RootTwo * dblTemp);
            if ( ctx.MinimapGl.TextureSize > 0 & ctx.ViewInfo.TilesPerMinimapPixel > 0.0D )
            {
                minimapSizeXy.X = (Terrain.TileSize.X / ctx.ViewInfo.TilesPerMinimapPixel).ToInt();
                minimapSizeXy.Y = (Terrain.TileSize.Y / ctx.ViewInfo.TilesPerMinimapPixel).ToInt();
            }

            if ( !ctx.ViewInfo.ScreenXYGetViewPlanePos(new XYInt((glSize.Width / 2.0D).ToInt(), (glSize.Height / 2.0D).ToInt()), dblTemp, ref drawCentre) )
            {
                Matrix3DMath.VectorForwardsRotationByMatrix(ctx.ViewInfo.ViewAngleMatrix, ref xyzDbl);
                var dblTemp2 = App.VisionRadius * 2.0D / Math.Sqrt(xyzDbl.X * xyzDbl.X + xyzDbl.Z * xyzDbl.Z);
                drawCentre.X = ctx.ViewInfo.ViewPos.X + xyzDbl.X * dblTemp2;
                drawCentre.Y = ctx.ViewInfo.ViewPos.Z + xyzDbl.Z * dblTemp2;
            }
            drawCentre.X = MathUtil.ClampDbl(drawCentre.X, 0.0D, Terrain.TileSize.X * Constants.TerrainGridSpacing - 1.0D);
            drawCentre.Y = MathUtil.ClampDbl(Convert.ToDouble(- drawCentre.Y), 0.0D, Terrain.TileSize.Y * Constants.TerrainGridSpacing - 1.0D);
            drawCentreSector.Normal = GetPosSectorNum(new XYInt(drawCentre.X.ToInt(), drawCentre.Y.ToInt()));
            drawCentreSector.Alignment = GetPosSectorNum(new XYInt((drawCentre.X - Constants.SectorTileSize * Constants.TerrainGridSpacing / 2.0D).ToInt(), (drawCentre.Y - Constants.SectorTileSize * Constants.TerrainGridSpacing / 2.0D).ToInt()));

            var drawObjects = new clsDrawSectorObjects(ctx.GlSize, ctx.ViewInfo)
                {
                    Map = this,
                    UnitTextLabels = new clsTextLabels(64)
                };

            drawObjects.Start();

            xyzDbl.X = drawCentre.X - ctx.ViewInfo.ViewPos.X;
            xyzDbl.Y = 128 - ctx.ViewInfo.ViewPos.Y;
            xyzDbl.Z = - drawCentre.Y - ctx.ViewInfo.ViewPos.Z;
            zNearFar = Convert.ToSingle(xyzDbl.GetMagnitude());

            GL.Enable(EnableCap.DepthTest);
            GL.MatrixMode(MatrixMode.Projection);
            float aspectRatio = (float)glSize.Width / (float)glSize.Height;
            var temp_mat = Matrix4.CreatePerspectiveFieldOfView(ctx.ViewInfo.FieldOfViewY, aspectRatio, zNearFar / 128.0F, zNearFar * 128.0F);
            GL.LoadMatrix(ref temp_mat);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            Matrix3DMath.MatrixRotationByMatrix(ctx.ViewInfo.ViewAngleMatrixInverted, App.SunAngleMatrix, matrixB);
            Matrix3DMath.VectorForwardsRotationByMatrix(matrixB, ref xyzDbl);
            lightPosition[0] = Convert.ToSingle(xyzDbl.X);
            lightPosition[1] = Convert.ToSingle(xyzDbl.Y);
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
            if ( ctx.ViewInfo.ScreenXYGetViewPlanePosForwardDownOnly(0, 0, dblTemp, ref viewCorner0)
                && ctx.ViewInfo.ScreenXYGetViewPlanePosForwardDownOnly(glSize.Width, 0, dblTemp, ref viewCorner1)
                && ctx.ViewInfo.ScreenXYGetViewPlanePosForwardDownOnly(glSize.Width, glSize.Height, dblTemp, ref viewCorner2)
                && ctx.ViewInfo.ScreenXYGetViewPlanePosForwardDownOnly(0, glSize.Height, dblTemp, ref viewCorner3) )
            {
                showMinimapViewPosBox = true;
            }
            else
            {
                showMinimapViewPosBox = false;
            }

            GL.Rotate((float)(ctx.ViewInfo.ViewAngleRPY.Roll / MathUtil.RadOf1Deg), 0.0F, 0.0F, -1.0F);
            GL.Rotate((float)(ctx.ViewInfo.ViewAngleRPY.Pitch / MathUtil.RadOf1Deg), 1.0F, 0.0F, 0.0F);
            GL.Rotate((float)(ctx.ViewInfo.ViewAngleRPY.Yaw / MathUtil.RadOf1Deg), 0.0F, 1.0F, 0.0F);
            GL.Translate(Convert.ToDouble(- ctx.ViewInfo.ViewPos.X), Convert.ToDouble(- ctx.ViewInfo.ViewPos.Y), ctx.ViewInfo.ViewPos.Z);

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

            var mouseOverTerrain = ctx.ViewInfo.GetMouseOverTerrain();

            if ( App.Draw_VertexTerrain )
            {
                GL.LineWidth(1.0F);
                var drawVertexTerran = new clsDrawVertexTerrain();
                drawVertexTerran.Map = this;
                drawVertexTerran.ViewAngleMatrix = ctx.ViewInfo.ViewAngleMatrix;
                App.VisionSectors.PerformActionMapSectors(drawVertexTerran, drawCentreSector);
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
                    xyzDbl.X = SelectedAreaVertexB.X * Constants.TerrainGridSpacing - ctx.ViewInfo.ViewPos.X;
                    xyzDbl.Z = - SelectedAreaVertexB.Y * Constants.TerrainGridSpacing - ctx.ViewInfo.ViewPos.Z;
                    xyzDbl.Y = GetVertexAltitude(SelectedAreaVertexB) - ctx.ViewInfo.ViewPos.Y;
                    drawIt = true;
                }
                else if (ctx.ToolOptions.MouseTool == MouseTool.TerrainSelect)
                {
                    if ( mouseOverTerrain != null )
                    {
                        //selection is changing under pointer
                        MathUtil.ReorderXY(SelectedAreaVertexA, mouseOverTerrain.Vertex.Normal, ref startXy, ref finishXy);
                        xyzDbl.X = mouseOverTerrain.Vertex.Normal.X * Constants.TerrainGridSpacing - ctx.ViewInfo.ViewPos.X;
                        xyzDbl.Z = - mouseOverTerrain.Vertex.Normal.Y * Constants.TerrainGridSpacing - ctx.ViewInfo.ViewPos.Z;
                        xyzDbl.Y = GetVertexAltitude(mouseOverTerrain.Vertex.Normal) - ctx.ViewInfo.ViewPos.Y;
                        drawIt = true;
                    }
                }
                if ( drawIt )
                {
                    Matrix3DMath.VectorRotationByMatrix(ctx.ViewInfo.ViewAngleMatrixInverted, xyzDbl, ref xyzDbl2);
                    if ( ctx.ViewInfo.PosGetScreenXY(xyzDbl2, ref screenPos) )
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

            if (ctx.ToolOptions.MouseTool == MouseTool.TerrainSelect)
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
                if (ctx.ToolOptions.MouseTool == MouseTool.ObjectSelect)
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

                if (ctx.ToolOptions.MouseTool == MouseTool.RoadPlace)
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
                else if (ctx.ToolOptions.MouseTool == MouseTool.RoadLines || 
                    ctx.ToolOptions.MouseTool == MouseTool.Gateways || 
                    ctx.ToolOptions.MouseTool == MouseTool.ObjectLines )
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

                clsBrush toolBrush;
                if (ctx.ToolOptions.MouseTool == MouseTool.TextureBrush)
                {
                    toolBrush = ctx.ToolOptions.Textures.Brush;
                }
                else if (ctx.ToolOptions.MouseTool == MouseTool.CliffBrush)
                {
                    toolBrush = ctx.ToolOptions.Terrain.CliffBrush;
                }
                else if (ctx.ToolOptions.MouseTool == MouseTool.CliffRemove)
                {
                    toolBrush = ctx.ToolOptions.Terrain.CliffBrush;
                }
                else if (ctx.ToolOptions.MouseTool == MouseTool.RoadRemove)
                {
                    toolBrush = ctx.ToolOptions.Terrain.CliffBrush;
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
                if (ctx.ToolOptions.MouseTool == MouseTool.TerrainFill)
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

                if (ctx.ToolOptions.MouseTool == MouseTool.TerrainBrush)
                {
                    toolBrush = ctx.ToolOptions.Terrain.Brush;
                }
                else if (ctx.ToolOptions.MouseTool == MouseTool.HeightSetBrush)
                {
                    toolBrush = ctx.ToolOptions.Height.Brush;
                }
                else if (ctx.ToolOptions.MouseTool == MouseTool.HeightChangeBrush)
                {
                    toolBrush = ctx.ToolOptions.Height.Brush;
                }
                else if (ctx.ToolOptions.MouseTool == MouseTool.HeightSmoothBrush)
                {
                    toolBrush = ctx.ToolOptions.Height.Brush;
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
            GL.Rotate((float)(ctx.ViewInfo.ViewAngleRPY.Roll / MathUtil.RadOf1Deg), 0.0F, 0.0F, -1.0F);
            GL.Rotate((float)(ctx.ViewInfo.ViewAngleRPY.Pitch / MathUtil.RadOf1Deg), 1.0F, 0.0F, 0.0F);
            GL.Rotate((float)(ctx.ViewInfo.ViewAngleRPY.Yaw / MathUtil.RadOf1Deg), 0.0F, 1.0F, 0.0F);

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
                if (ctx.ToolOptions.MouseTool == MouseTool.ObjectPlace)
                {
                    var placeObject = ctx.ToolOptions.PlaceObject.SingleSelectedObjectTypeBase;
                    if ( placeObject != null )
                    {
                        var rotation = Convert.ToInt32(ctx.ToolOptions.PlaceObject.Rotation);
                        try
                        {
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
                        GL.Translate(worldPos.Horizontal.X - ctx.ViewInfo.ViewPos.X, worldPos.Altitude - ctx.ViewInfo.ViewPos.Y + 2.0D,
                            ctx.ViewInfo.ViewPos.Z + worldPos.Horizontal.Y);
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
                GL.Translate(Convert.ToDouble(- ctx.ViewInfo.ViewPos.X), Convert.ToDouble(- ctx.ViewInfo.ViewPos.Y), ctx.ViewInfo.ViewPos.Z);
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
                    xyzDbl.X = scriptPosition.PosX - ctx.ViewInfo.ViewPos.X;
                    xyzDbl.Z = - scriptPosition.PosY - ctx.ViewInfo.ViewPos.Z;
                    xyzDbl.Y = GetTerrainHeight(new XYInt(scriptPosition.PosX, scriptPosition.PosY)) - ctx.ViewInfo.ViewPos.Y;
                    Matrix3DMath.VectorRotationByMatrix(ctx.ViewInfo.ViewAngleMatrixInverted, xyzDbl, ref xyzDbl2);
                    if ( ctx.ViewInfo.PosGetScreenXY(xyzDbl2, ref screenPos) )
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
                    xyzDbl.X = scriptArea.PosAX - ctx.ViewInfo.ViewPos.X;
                    xyzDbl.Z = - scriptArea.PosAY - ctx.ViewInfo.ViewPos.Z;
                    xyzDbl.Y = GetTerrainHeight(new XYInt(scriptArea.PosAX, scriptArea.PosAY)) - ctx.ViewInfo.ViewPos.Y;
                    Matrix3DMath.VectorRotationByMatrix(ctx.ViewInfo.ViewAngleMatrixInverted, xyzDbl, ref xyzDbl2);
                    if ( ctx.ViewInfo.PosGetScreenXY(xyzDbl2, ref screenPos) )
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
                    textLabel.Pos.Y = 32 + Math.Ceiling((decimal)(b * textLabel.SizeY)).ToInt();
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
                DrawUnitRectangle(unit, 8, colourA, colourB, ctx.ViewInfo);
            }
            if ( mouseOverTerrain != null )
            {
                foreach ( var tempLoopVar_Unit in mouseOverTerrain.Units )
                {
                    unit = tempLoopVar_Unit;
                    if ( unit != null && ctx.ToolOptions.MouseTool == MouseTool.ObjectSelect)
                    {
                        rgb = GetUnitGroupColour(unit.UnitGroup);
                        GL.Color4((0.5F + rgb.Red) / 1.5F, (0.5F + rgb.Green) / 1.5F, (0.5F + rgb.Blue) / 1.5F, 0.75F);
                        colourA = new SRgba((1.0F + rgb.Red) / 2.0F, (1.0F + rgb.Green) / 2.0F, (1.0F + rgb.Blue) / 2.0F, 0.75F);
                        colourB = new SRgba(rgb.Red, rgb.Green, rgb.Blue, 0.875F);
                        DrawUnitRectangle(unit, 16, colourA, colourB, ctx.ViewInfo);
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

            if ( ctx.MinimapGl.TextureSize > 0 & ctx.ViewInfo.TilesPerMinimapPixel > 0.0D )
            {
                GL.Translate(0.0F, glSize.Height - minimapSizeXy.Y, 0.0F);

                xyzDbl.X = (double)Terrain.TileSize.X / ctx.MinimapGl.TextureSize;
                xyzDbl.Z = (double)Terrain.TileSize.Y / ctx.MinimapGl.TextureSize;

                if ( ctx.MinimapGl.GLTexture > 0 )
                {
                    GL.Enable(EnableCap.Texture2D);
                    GL.BindTexture(TextureTarget.Texture2D, ctx.MinimapGl.GLTexture);
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
                    dblTemp = Constants.TerrainGridSpacing * ctx.ViewInfo.TilesPerMinimapPixel;

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
                    else if (ctx.ToolOptions.MouseTool == MouseTool.TerrainSelect)
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
                        posA.X = startXy.X / ctx.ViewInfo.TilesPerMinimapPixel;
                        posA.Y = minimapSizeXy.Y - startXy.Y / ctx.ViewInfo.TilesPerMinimapPixel;
                        posB.X = finishXy.X / ctx.ViewInfo.TilesPerMinimapPixel;
                        posB.Y = minimapSizeXy.Y - startXy.Y / ctx.ViewInfo.TilesPerMinimapPixel;
                        posC.X = finishXy.X / ctx.ViewInfo.TilesPerMinimapPixel;
                        posC.Y = minimapSizeXy.Y - finishXy.Y / ctx.ViewInfo.TilesPerMinimapPixel;
                        posD.X = startXy.X / ctx.ViewInfo.TilesPerMinimapPixel;
                        posD.Y = minimapSizeXy.Y - finishXy.Y / ctx.ViewInfo.TilesPerMinimapPixel;
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

        public void DrawUnitRectangle(Unit Unit, int BorderInsideThickness, SRgba InsideColour, SRgba OutsideColour, ViewInfo viewInfo)
        {
            var posA = new XYInt();
            var posB = new XYInt();
            var a = 0;
            var altitude = Unit.Pos.Altitude - viewInfo.ViewPos.Y;

            GetFootprintTileRangeClamped(Unit.Pos.Horizontal, Unit.TypeBase.GetGetFootprintSelected(Unit.Rotation), ref posA, ref posB);
            a = posA.Y;
            posA.X = ((posA.X + 0.125D) * Constants.TerrainGridSpacing - viewInfo.ViewPos.X).ToInt();
            posA.Y = ((posB.Y + 0.875D) * - Constants.TerrainGridSpacing - viewInfo.ViewPos.Z).ToInt();
            posB.X = ((posB.X + 0.875D) * Constants.TerrainGridSpacing - viewInfo.ViewPos.X).ToInt();
            posB.Y = ((a + 0.125D) * - Constants.TerrainGridSpacing - viewInfo.ViewPos.Z).ToInt();

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