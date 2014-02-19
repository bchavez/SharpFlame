using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using SharpFlame.AppSettings;
using SharpFlame.Colors;
using SharpFlame.Core.Domain;
using SharpFlame.Controls;
using SharpFlame.Domain;
using SharpFlame.FileIO;
using SharpFlame.Graphics.OpenGL;
using SharpFlame.Mapping.Drawing;
using SharpFlame.Mapping.Objects;
using SharpFlame.Mapping.Script;
using SharpFlame.Mapping.Tools;
using SharpFlame.Maths;
using SharpFlame.Util;

namespace SharpFlame.Mapping
{
    public partial class clsMap
    {
        public clsViewInfo ViewInfo;

        public void GLDraw()
        {
            XYZDouble XYZ_dbl = default(XYZDouble);
            int X = 0;
            int Y = 0;
            int X2 = 0;
            int Y2 = 0;
            int A = 0;
            int B = 0;
            int C = 0;
            int D = 0;
            sRGBA_sng ColourA = new sRGBA_sng();
            sRGBA_sng ColourB = new sRGBA_sng();
            bool ShowMinimapViewPosBox = default(bool);
            XYDouble ViewCorner0 = default(XYDouble);
            XYDouble ViewCorner1 = default(XYDouble);
            XYDouble ViewCorner2 = default(XYDouble);
            XYDouble ViewCorner3 = default(XYDouble);
            double dblTemp = 0;
            XYZDouble Vertex0 = default(XYZDouble);
            XYZDouble Vertex1 = default(XYZDouble);
            XYZDouble Vertex2 = default(XYZDouble);
            XYZDouble Vertex3 = default(XYZDouble);
            XYInt ScreenPos = new XYInt();
            XYZDouble XYZ_dbl2 = default(XYZDouble);
            WorldPos WorldPos = new WorldPos();
            XYDouble PosA = default(XYDouble);
            XYDouble PosB = default(XYDouble);
            XYDouble PosC = default(XYDouble);
            XYDouble PosD = default(XYDouble);
            XYInt MinimapSizeXY = new XYInt();
            clsUnit Unit = default(clsUnit);
            XYInt StartXY = new XYInt();
            XYInt FinishXY = new XYInt();
            bool DrawIt = default(bool);
            clsBrush.sPosNum DrawCentreSector = new clsBrush.sPosNum();
            clsTextLabel SelectionLabel = new clsTextLabel();
            float[] light_position = new float[4];
            Matrix3DMath.Matrix3D matrixB = new Matrix3DMath.Matrix3D();
            clsAction MapAction = default(clsAction);
            float ZNearFar = 0;
            MapViewControl MapViewControl = ViewInfo.MapViewControl;
            XYInt GLSize = ViewInfo.MapViewControl.GLSize;
            XYDouble DrawCentre = default(XYDouble);
            double dblTemp2 = 0;

            dblTemp = SettingsManager.Settings.MinimapSize;
            ViewInfo.Tiles_Per_Minimap_Pixel = Math.Sqrt(Terrain.TileSize.X * Terrain.TileSize.X + Terrain.TileSize.Y * Terrain.TileSize.Y) /
                                               (MathUtil.RootTwo * dblTemp);
            if ( Minimap_Texture_Size > 0 & ViewInfo.Tiles_Per_Minimap_Pixel > 0.0D )
            {
                MinimapSizeXY.X = (int)(Terrain.TileSize.X / ViewInfo.Tiles_Per_Minimap_Pixel);
                MinimapSizeXY.Y = (int)(Terrain.TileSize.Y / ViewInfo.Tiles_Per_Minimap_Pixel);
            }

            if ( !ViewInfo.ScreenXY_Get_ViewPlanePos(new XYInt((int)(GLSize.X / 2.0D), (int)(GLSize.Y / 2.0D)), dblTemp, ref DrawCentre) )
            {
                Matrix3DMath.VectorForwardsRotationByMatrix(ViewInfo.ViewAngleMatrix, ref XYZ_dbl);
                dblTemp2 = App.VisionRadius * 2.0D / Math.Sqrt(XYZ_dbl.X * XYZ_dbl.X + XYZ_dbl.Z * XYZ_dbl.Z);
                DrawCentre.X = ViewInfo.ViewPos.X + XYZ_dbl.X * dblTemp2;
                DrawCentre.Y = ViewInfo.ViewPos.Z + XYZ_dbl.Z * dblTemp2;
            }
            DrawCentre.X = MathUtil.Clamp_dbl(DrawCentre.X, 0.0D, Terrain.TileSize.X * Constants.TerrainGridSpacing - 1.0D);
            DrawCentre.Y = MathUtil.Clamp_dbl(Convert.ToDouble(- DrawCentre.Y), 0.0D, Terrain.TileSize.Y * Constants.TerrainGridSpacing - 1.0D);
            DrawCentreSector.Normal = GetPosSectorNum(new XYInt((int)DrawCentre.X, (int)DrawCentre.Y));
            DrawCentreSector.Alignment =
                GetPosSectorNum(new XYInt((int)(DrawCentre.X - Constants.SectorTileSize * Constants.TerrainGridSpacing / 2.0D),
                    (int)(DrawCentre.Y - Constants.SectorTileSize * Constants.TerrainGridSpacing / 2.0D)));

            clsDrawSectorObjects DrawObjects = new clsDrawSectorObjects();
            DrawObjects.Map = this;
            DrawObjects.UnitTextLabels = new clsTextLabels(64);
            DrawObjects.Start();

            XYZ_dbl.X = DrawCentre.X - ViewInfo.ViewPos.X;
            XYZ_dbl.Y = 128 - ViewInfo.ViewPos.Y;
            XYZ_dbl.Z = - DrawCentre.Y - ViewInfo.ViewPos.Z;
            ZNearFar = (float)(XYZ_dbl.GetMagnitude());

            GL.Enable(EnableCap.DepthTest);
            GL.MatrixMode(MatrixMode.Projection);
            Matrix4 temp_mat = Matrix4.CreatePerspectiveFieldOfView(ViewInfo.FieldOfViewY, MapViewControl.OpenGLControl.AspectRatio, ZNearFar / 128.0F, ZNearFar * 128.0F);
            GL.LoadMatrix(ref temp_mat);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            Matrix3DMath.MatrixRotationByMatrix(ViewInfo.ViewAngleMatrix_Inverted, App.SunAngleMatrix, matrixB);
            Matrix3DMath.VectorForwardsRotationByMatrix(matrixB, ref XYZ_dbl);
            light_position[0] = (float)XYZ_dbl.X;
            light_position[1] = (float)XYZ_dbl.Y;
            light_position[2] = Convert.ToSingle(- XYZ_dbl.Z);
            light_position[3] = 0.0F;
            GL.Light(LightName.Light0, LightParameter.Position, light_position);
            GL.Light(LightName.Light1, LightParameter.Position, light_position);

            GL.Disable(EnableCap.Light0);
            GL.Disable(EnableCap.Light1);
            if ( App.Draw_Lighting != enumDrawLighting.Off )
            {
                if ( App.Draw_Lighting == enumDrawLighting.Half )
                {
                    GL.Enable(EnableCap.Light0);
                }
                else if ( App.Draw_Lighting == enumDrawLighting.Normal )
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
            if ( ViewInfo.ScreenXY_Get_ViewPlanePos_ForwardDownOnly(0, 0, dblTemp, ref ViewCorner0)
                 && ViewInfo.ScreenXY_Get_ViewPlanePos_ForwardDownOnly(GLSize.X, 0, dblTemp, ref ViewCorner1)
                 && ViewInfo.ScreenXY_Get_ViewPlanePos_ForwardDownOnly(GLSize.X, GLSize.Y, dblTemp, ref ViewCorner2)
                 && ViewInfo.ScreenXY_Get_ViewPlanePos_ForwardDownOnly(0, GLSize.Y, dblTemp, ref ViewCorner3) )
            {
                ShowMinimapViewPosBox = true;
            }
            else
            {
                ShowMinimapViewPosBox = false;
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
                MapAction = new clsDrawCallTerrain();
                MapAction.Map = this;
                App.VisionSectors.PerformActionMapSectors(MapAction, DrawCentreSector);
                GL.Disable(EnableCap.Texture2D);

                DebugGLError("Tile textures");
            }

            GL.Disable(EnableCap.DepthTest);
            GL.Disable(EnableCap.Lighting);

            if ( App.Draw_TileWireframe )
            {
                GL.Color3(0.0F, 1.0F, 0.0F);
                GL.LineWidth(1.0F);
                clsDrawCallTerrainWireframe DrawCallTerrainWireframe = new clsDrawCallTerrainWireframe();
                DrawCallTerrainWireframe.Map = this;
                App.VisionSectors.PerformActionMapSectors(DrawCallTerrainWireframe, DrawCentreSector);

                DebugGLError("Wireframe");
            }

            //draw tile orientation markers

            if ( App.DisplayTileOrientation )
            {
                GL.Disable(EnableCap.CullFace);

                GL.Begin(BeginMode.Triangles);
                GL.Color3(1.0F, 1.0F, 0.0F);
                MapAction = new clsDrawTileOrientation();
                MapAction.Map = this;
                App.VisionSectors.PerformActionMapSectors(MapAction, DrawCentreSector);
                GL.End();

                GL.Enable(EnableCap.CullFace);

                DebugGLError("Tile orientation");
            }

            //draw painted texture terrain type markers

            sRGB_sng RGB_sng = new sRGB_sng();

            clsViewInfo.clsMouseOver.clsOverTerrain MouseOverTerrain = ViewInfo.GetMouseOverTerrain();

            if ( App.Draw_VertexTerrain )
            {
                GL.LineWidth(1.0F);
                clsDrawVertexTerrain DrawVertexTerran = new clsDrawVertexTerrain();
                DrawVertexTerran.Map = this;
                DrawVertexTerran.ViewAngleMatrix = ViewInfo.ViewAngleMatrix;
                App.VisionSectors.PerformActionMapSectors(DrawVertexTerran, DrawCentreSector);
                DebugGLError("Terrain type markers");
            }

            SelectionLabel.Text = "";

            if ( Selected_Area_VertexA != null )
            {
                DrawIt = false;
                if ( Selected_Area_VertexB != null )
                {
                    //area is selected
                    MathUtil.ReorderXY(Selected_Area_VertexA, Selected_Area_VertexB, ref StartXY, ref FinishXY);
                    XYZ_dbl.X = Selected_Area_VertexB.X * Constants.TerrainGridSpacing - ViewInfo.ViewPos.X;
                    XYZ_dbl.Z = - Selected_Area_VertexB.Y * Constants.TerrainGridSpacing - ViewInfo.ViewPos.Z;
                    XYZ_dbl.Y = GetVertexAltitude(Selected_Area_VertexB) - ViewInfo.ViewPos.Y;
                    DrawIt = true;
                }
                else if ( modTools.Tool == modTools.Tools.TerrainSelect )
                {
                    if ( MouseOverTerrain != null )
                    {
                        //selection is changing under pointer
                        MathUtil.ReorderXY(Selected_Area_VertexA, MouseOverTerrain.Vertex.Normal, ref StartXY, ref FinishXY);
                        XYZ_dbl.X = MouseOverTerrain.Vertex.Normal.X * Constants.TerrainGridSpacing - ViewInfo.ViewPos.X;
                        XYZ_dbl.Z = - MouseOverTerrain.Vertex.Normal.Y * Constants.TerrainGridSpacing - ViewInfo.ViewPos.Z;
                        XYZ_dbl.Y = GetVertexAltitude(MouseOverTerrain.Vertex.Normal) - ViewInfo.ViewPos.Y;
                        DrawIt = true;
                    }
                }
                if ( DrawIt )
                {
                    Matrix3DMath.VectorRotationByMatrix(ViewInfo.ViewAngleMatrix_Inverted, XYZ_dbl, ref XYZ_dbl2);
                    if ( ViewInfo.Pos_Get_Screen_XY(XYZ_dbl2, ref ScreenPos) )
                    {
                        if ( ScreenPos.X >= 0 & ScreenPos.X <= GLSize.X & ScreenPos.Y >= 0 & ScreenPos.Y <= GLSize.Y )
                        {
                            SelectionLabel.Colour.Red = 1.0F;
                            SelectionLabel.Colour.Green = 1.0F;
                            SelectionLabel.Colour.Blue = 1.0F;
                            SelectionLabel.Colour.Alpha = 1.0F;
                            SelectionLabel.TextFont = App.UnitLabelFont;
                            SelectionLabel.SizeY = SettingsManager.Settings.FontSize;
                            SelectionLabel.Pos = ScreenPos;
                            SelectionLabel.Text = FinishXY.X - StartXY.X + "x" + Convert.ToString(FinishXY.Y - StartXY.Y);
                        }
                    }
                    GL.LineWidth(3.0F);
                    clsDrawTileAreaOutline DrawSelection = new clsDrawTileAreaOutline();
                    DrawSelection.Map = this;
                    DrawSelection.StartXY = StartXY;
                    DrawSelection.FinishXY = FinishXY;
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

                    Vertex0.X = MouseOverTerrain.Vertex.Normal.X * Constants.TerrainGridSpacing;
                    Vertex0.Y = Convert.ToDouble(Terrain.Vertices[MouseOverTerrain.Vertex.Normal.X, MouseOverTerrain.Vertex.Normal.Y].Height * HeightMultiplier);
                    Vertex0.Z = - MouseOverTerrain.Vertex.Normal.Y * Constants.TerrainGridSpacing;
                    GL.Begin(BeginMode.Lines);
                    GL.Color3(1.0F, 1.0F, 1.0F);
                    GL.Vertex3(Vertex0.X - 8.0D, Vertex0.Y, Convert.ToDouble(- Vertex0.Z));
                    GL.Vertex3(Vertex0.X + 8.0D, Vertex0.Y, Convert.ToDouble(- Vertex0.Z));
                    GL.Vertex3(Vertex0.X, Vertex0.Y, - Vertex0.Z - 8.0D);
                    GL.Vertex3(Vertex0.X, Vertex0.Y, - Vertex0.Z + 8.0D);
                    GL.End();
                }
                DebugGLError("Terrain selection vertex");
            }

            clsGateway Gateway = default(clsGateway);

            if ( App.Draw_Gateways )
            {
                GL.LineWidth(2.0F);
                foreach ( clsGateway tempLoopVar_Gateway in Gateways )
                {
                    Gateway = tempLoopVar_Gateway;
                    if ( Gateway.PosA.X == Gateway.PosB.X )
                    {
                        if ( Gateway.PosA.Y <= Gateway.PosB.Y )
                        {
                            C = Gateway.PosA.Y;
                            D = Gateway.PosB.Y;
                        }
                        else
                        {
                            C = Gateway.PosB.Y;
                            D = Gateway.PosA.Y;
                        }
                        X2 = Gateway.PosA.X;
                        for ( Y2 = C; Y2 <= D; Y2++ )
                        {
                            Vertex0.X = X2 * Constants.TerrainGridSpacing;
                            Vertex0.Y = Convert.ToDouble(Terrain.Vertices[X2, Y2].Height * HeightMultiplier);
                            Vertex0.Z = - Y2 * Constants.TerrainGridSpacing;
                            Vertex1.X = (X2 + 1) * Constants.TerrainGridSpacing;
                            Vertex1.Y = Convert.ToDouble(Terrain.Vertices[X2 + 1, Y2].Height * HeightMultiplier);
                            Vertex1.Z = - Y2 * Constants.TerrainGridSpacing;
                            Vertex2.X = X2 * Constants.TerrainGridSpacing;
                            Vertex2.Y = Convert.ToDouble(Terrain.Vertices[X2, Y2 + 1].Height * HeightMultiplier);
                            Vertex2.Z = - (Y2 + 1) * Constants.TerrainGridSpacing;
                            Vertex3.X = (X2 + 1) * Constants.TerrainGridSpacing;
                            Vertex3.Y = Convert.ToDouble(Terrain.Vertices[X2 + 1, Y2 + 1].Height * HeightMultiplier);
                            Vertex3.Z = - (Y2 + 1) * Constants.TerrainGridSpacing;
                            GL.Begin(BeginMode.LineLoop);
                            GL.Color3(0.75F, 1.0F, 0.0F);
                            GL.Vertex3(Vertex0.X, Vertex0.Y, Convert.ToDouble(- Vertex0.Z));
                            GL.Vertex3(Vertex1.X, Vertex1.Y, Convert.ToDouble(- Vertex1.Z));
                            GL.Vertex3(Vertex3.X, Vertex3.Y, Convert.ToDouble(- Vertex3.Z));
                            GL.Vertex3(Vertex2.X, Vertex2.Y, Convert.ToDouble(- Vertex2.Z));
                            GL.End();
                        }
                    }
                    else if ( Gateway.PosA.Y == Gateway.PosB.Y )
                    {
                        if ( Gateway.PosA.X <= Gateway.PosB.X )
                        {
                            C = Gateway.PosA.X;
                            D = Gateway.PosB.X;
                        }
                        else
                        {
                            C = Gateway.PosB.X;
                            D = Gateway.PosA.X;
                        }
                        Y2 = Gateway.PosA.Y;
                        for ( X2 = C; X2 <= D; X2++ )
                        {
                            Vertex0.X = X2 * Constants.TerrainGridSpacing;
                            Vertex0.Y = Convert.ToDouble(Terrain.Vertices[X2, Y2].Height * HeightMultiplier);
                            Vertex0.Z = - Y2 * Constants.TerrainGridSpacing;
                            Vertex1.X = (X2 + 1) * Constants.TerrainGridSpacing;
                            Vertex1.Y = Convert.ToDouble(Terrain.Vertices[X2 + 1, Y2].Height * HeightMultiplier);
                            Vertex1.Z = - Y2 * Constants.TerrainGridSpacing;
                            Vertex2.X = X2 * Constants.TerrainGridSpacing;
                            Vertex2.Y = Convert.ToDouble(Terrain.Vertices[X2, Y2 + 1].Height * HeightMultiplier);
                            Vertex2.Z = - (Y2 + 1) * Constants.TerrainGridSpacing;
                            Vertex3.X = (X2 + 1) * Constants.TerrainGridSpacing;
                            Vertex3.Y = Convert.ToDouble(Terrain.Vertices[X2 + 1, Y2 + 1].Height * HeightMultiplier);
                            Vertex3.Z = - (Y2 + 1) * Constants.TerrainGridSpacing;
                            GL.Begin(BeginMode.LineLoop);
                            GL.Color3(0.75F, 1.0F, 0.0F);
                            GL.Vertex3(Vertex0.X, Vertex0.Y, Convert.ToDouble(- Vertex0.Z));
                            GL.Vertex3(Vertex1.X, Vertex1.Y, Convert.ToDouble(- Vertex1.Z));
                            GL.Vertex3(Vertex3.X, Vertex3.Y, Convert.ToDouble(- Vertex3.Z));
                            GL.Vertex3(Vertex2.X, Vertex2.Y, Convert.ToDouble(- Vertex2.Z));
                            GL.End();
                        }
                    }
                    else
                    {
                        //draw invalid gateways as red tile borders
                        X2 = Gateway.PosA.X;
                        Y2 = Gateway.PosA.Y;

                        Vertex0.X = X2 * Constants.TerrainGridSpacing;
                        Vertex0.Y = Convert.ToDouble(Terrain.Vertices[X2, Y2].Height * HeightMultiplier);
                        Vertex0.Z = - Y2 * Constants.TerrainGridSpacing;
                        Vertex1.X = (X2 + 1) * Constants.TerrainGridSpacing;
                        Vertex1.Y = Convert.ToDouble(Terrain.Vertices[X2 + 1, Y2].Height * HeightMultiplier);
                        Vertex1.Z = - Y2 * Constants.TerrainGridSpacing;
                        Vertex2.X = X2 * Constants.TerrainGridSpacing;
                        Vertex2.Y = Convert.ToDouble(Terrain.Vertices[X2, Y2 + 1].Height * HeightMultiplier);
                        Vertex2.Z = - (Y2 + 1) * Constants.TerrainGridSpacing;
                        Vertex3.X = (X2 + 1) * Constants.TerrainGridSpacing;
                        Vertex3.Y = Convert.ToDouble(Terrain.Vertices[X2 + 1, Y2 + 1].Height * HeightMultiplier);
                        Vertex3.Z = - (Y2 + 1) * Constants.TerrainGridSpacing;
                        GL.Begin(BeginMode.LineLoop);
                        GL.Color3(1.0F, 0.0F, 0.0F);
                        GL.Vertex3(Vertex0.X, Vertex0.Y, Convert.ToDouble(- Vertex0.Z));
                        GL.Vertex3(Vertex1.X, Vertex1.Y, Convert.ToDouble(- Vertex1.Z));
                        GL.Vertex3(Vertex3.X, Vertex3.Y, Convert.ToDouble(- Vertex3.Z));
                        GL.Vertex3(Vertex2.X, Vertex2.Y, Convert.ToDouble(- Vertex2.Z));
                        GL.End();

                        X2 = Gateway.PosB.X;
                        Y2 = Gateway.PosB.Y;

                        Vertex0.X = X2 * Constants.TerrainGridSpacing;
                        Vertex0.Y = Convert.ToDouble(Terrain.Vertices[X2, Y2].Height * HeightMultiplier);
                        Vertex0.Z = - Y2 * Constants.TerrainGridSpacing;
                        Vertex1.X = (X2 + 1) * Constants.TerrainGridSpacing;
                        Vertex1.Y = Convert.ToDouble(Terrain.Vertices[X2 + 1, Y2].Height * HeightMultiplier);
                        Vertex1.Z = - Y2 * Constants.TerrainGridSpacing;
                        Vertex2.X = X2 * Constants.TerrainGridSpacing;
                        Vertex2.Y = Convert.ToDouble(Terrain.Vertices[X2, Y2 + 1].Height * HeightMultiplier);
                        Vertex2.Z = - (Y2 + 1) * Constants.TerrainGridSpacing;
                        Vertex3.X = (X2 + 1) * Constants.TerrainGridSpacing;
                        Vertex3.Y = Convert.ToDouble(Terrain.Vertices[X2 + 1, Y2 + 1].Height * HeightMultiplier);
                        Vertex3.Z = - (Y2 + 1) * Constants.TerrainGridSpacing;
                        GL.Begin(BeginMode.LineLoop);
                        GL.Color3(1.0F, 0.0F, 0.0F);
                        GL.Vertex3(Vertex0.X, Vertex0.Y, Convert.ToDouble(- Vertex0.Z));
                        GL.Vertex3(Vertex1.X, Vertex1.Y, Convert.ToDouble(- Vertex1.Z));
                        GL.Vertex3(Vertex3.X, Vertex3.Y, Convert.ToDouble(- Vertex3.Z));
                        GL.Vertex3(Vertex2.X, Vertex2.Y, Convert.ToDouble(- Vertex2.Z));
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
                        MathUtil.ReorderXY(Unit_Selected_Area_VertexA, MouseOverTerrain.Vertex.Normal, ref StartXY, ref FinishXY);
                        GL.LineWidth(2.0F);
                        GL.Color3(0.0F, 1.0F, 1.0F);
                        for ( X = StartXY.X; X <= FinishXY.X - 1; X++ )
                        {
                            Vertex0.X = X * Constants.TerrainGridSpacing;
                            Vertex0.Y = Convert.ToDouble(Terrain.Vertices[X, StartXY.Y].Height * HeightMultiplier);
                            Vertex0.Z = - StartXY.Y * Constants.TerrainGridSpacing;
                            Vertex1.X = (X + 1) * Constants.TerrainGridSpacing;
                            Vertex1.Y = Convert.ToDouble(Terrain.Vertices[X + 1, StartXY.Y].Height * HeightMultiplier);
                            Vertex1.Z = - StartXY.Y * Constants.TerrainGridSpacing;
                            GL.Begin(BeginMode.Lines);
                            GL.Vertex3(Vertex0.X, Vertex0.Y, Convert.ToDouble(- Vertex0.Z));
                            GL.Vertex3(Vertex1.X, Vertex1.Y, Convert.ToDouble(- Vertex1.Z));
                            GL.End();
                        }
                        for ( X = StartXY.X; X <= FinishXY.X - 1; X++ )
                        {
                            Vertex0.X = X * Constants.TerrainGridSpacing;
                            Vertex0.Y = Convert.ToDouble(Terrain.Vertices[X, FinishXY.Y].Height * HeightMultiplier);
                            Vertex0.Z = - FinishXY.Y * Constants.TerrainGridSpacing;
                            Vertex1.X = (X + 1) * Constants.TerrainGridSpacing;
                            Vertex1.Y = Convert.ToDouble(Terrain.Vertices[X + 1, FinishXY.Y].Height * HeightMultiplier);
                            Vertex1.Z = - FinishXY.Y * Constants.TerrainGridSpacing;
                            GL.Begin(BeginMode.Lines);
                            GL.Vertex3(Vertex0.X, Vertex0.Y, Convert.ToDouble(- Vertex0.Z));
                            GL.Vertex3(Vertex1.X, Vertex1.Y, Convert.ToDouble(- Vertex1.Z));
                            GL.End();
                        }
                        for ( Y = StartXY.Y; Y <= FinishXY.Y - 1; Y++ )
                        {
                            Vertex0.X = StartXY.X * Constants.TerrainGridSpacing;
                            Vertex0.Y = Convert.ToDouble(Terrain.Vertices[StartXY.X, Y].Height * HeightMultiplier);
                            Vertex0.Z = - Y * Constants.TerrainGridSpacing;
                            Vertex1.X = StartXY.X * Constants.TerrainGridSpacing;
                            Vertex1.Y = Convert.ToDouble(Terrain.Vertices[StartXY.X, Y + 1].Height * HeightMultiplier);
                            Vertex1.Z = - (Y + 1) * Constants.TerrainGridSpacing;
                            GL.Begin(BeginMode.Lines);
                            GL.Vertex3(Vertex0.X, Vertex0.Y, Convert.ToDouble(- Vertex0.Z));
                            GL.Vertex3(Vertex1.X, Vertex1.Y, Convert.ToDouble(- Vertex1.Z));
                            GL.End();
                        }
                        for ( Y = StartXY.Y; Y <= FinishXY.Y - 1; Y++ )
                        {
                            Vertex0.X = FinishXY.X * Constants.TerrainGridSpacing;
                            Vertex0.Y = Convert.ToDouble(Terrain.Vertices[FinishXY.X, Y].Height * HeightMultiplier);
                            Vertex0.Z = - Y * Constants.TerrainGridSpacing;
                            Vertex1.X = FinishXY.X * Constants.TerrainGridSpacing;
                            Vertex1.Y = Convert.ToDouble(Terrain.Vertices[FinishXY.X, Y + 1].Height * HeightMultiplier);
                            Vertex1.Z = - (Y + 1) * Constants.TerrainGridSpacing;
                            GL.Begin(BeginMode.Lines);
                            GL.Vertex3(Vertex0.X, Vertex0.Y, Convert.ToDouble(- Vertex0.Z));
                            GL.Vertex3(Vertex1.X, Vertex1.Y, Convert.ToDouble(- Vertex1.Z));
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
                        Vertex0.X = MouseOverTerrain.Side_Num.X * Constants.TerrainGridSpacing;
                        Vertex0.Y = Convert.ToDouble(Terrain.Vertices[MouseOverTerrain.Side_Num.X, MouseOverTerrain.Side_Num.Y].Height * HeightMultiplier);
                        Vertex0.Z = - MouseOverTerrain.Side_Num.Y * Constants.TerrainGridSpacing;
                        Vertex1.X = MouseOverTerrain.Side_Num.X * Constants.TerrainGridSpacing;
                        Vertex1.Y = Convert.ToDouble(Terrain.Vertices[MouseOverTerrain.Side_Num.X, MouseOverTerrain.Side_Num.Y + 1].Height * HeightMultiplier);
                        Vertex1.Z = - (MouseOverTerrain.Side_Num.Y + 1) * Constants.TerrainGridSpacing;
                    }
                    else
                    {
                        Vertex0.X = MouseOverTerrain.Side_Num.X * Constants.TerrainGridSpacing;
                        Vertex0.Y = Convert.ToDouble(Terrain.Vertices[MouseOverTerrain.Side_Num.X, MouseOverTerrain.Side_Num.Y].Height * HeightMultiplier);
                        Vertex0.Z = - MouseOverTerrain.Side_Num.Y * Constants.TerrainGridSpacing;
                        Vertex1.X = (MouseOverTerrain.Side_Num.X + 1) * Constants.TerrainGridSpacing;
                        Vertex1.Y = Convert.ToDouble(Terrain.Vertices[MouseOverTerrain.Side_Num.X + 1, MouseOverTerrain.Side_Num.Y].Height * HeightMultiplier);
                        Vertex1.Z = - MouseOverTerrain.Side_Num.Y * Constants.TerrainGridSpacing;
                    }

                    GL.Begin(BeginMode.Lines);
                    GL.Color3(0.0F, 1.0F, 1.0F);
                    GL.Vertex3(Vertex0.X, Vertex0.Y, Convert.ToDouble(- Vertex0.Z));
                    GL.Vertex3(Vertex1.X, Vertex1.Y, Convert.ToDouble(- Vertex1.Z));
                    GL.End();

                    DebugGLError("Road place brush");
                }
                else if ( modTools.Tool == modTools.Tools.RoadLines || modTools.Tool == modTools.Tools.Gateways || modTools.Tool == modTools.Tools.ObjectLines )
                {
                    GL.LineWidth(2.0F);

                    if ( Selected_Tile_A != null )
                    {
                        X2 = Selected_Tile_A.X;
                        Y2 = Selected_Tile_A.Y;

                        Vertex0.X = X2 * Constants.TerrainGridSpacing;
                        Vertex0.Y = Convert.ToDouble(Terrain.Vertices[X2, Y2].Height * HeightMultiplier);
                        Vertex0.Z = - Y2 * Constants.TerrainGridSpacing;
                        Vertex1.X = (X2 + 1) * Constants.TerrainGridSpacing;
                        Vertex1.Y = Convert.ToDouble(Terrain.Vertices[X2 + 1, Y2].Height * HeightMultiplier);
                        Vertex1.Z = - Y2 * Constants.TerrainGridSpacing;
                        Vertex2.X = X2 * Constants.TerrainGridSpacing;
                        Vertex2.Y = Convert.ToDouble(Terrain.Vertices[X2, Y2 + 1].Height * HeightMultiplier);
                        Vertex2.Z = - (Y2 + 1) * Constants.TerrainGridSpacing;
                        Vertex3.X = (X2 + 1) * Constants.TerrainGridSpacing;
                        Vertex3.Y = Convert.ToDouble(Terrain.Vertices[X2 + 1, Y2 + 1].Height * HeightMultiplier);
                        Vertex3.Z = - (Y2 + 1) * Constants.TerrainGridSpacing;
                        GL.Begin(BeginMode.LineLoop);
                        GL.Color3(0.0F, 1.0F, 1.0F);
                        GL.Vertex3(Vertex0.X, Vertex0.Y, Convert.ToDouble(- Vertex0.Z));
                        GL.Vertex3(Vertex1.X, Vertex1.Y, Convert.ToDouble(- Vertex1.Z));
                        GL.Vertex3(Vertex3.X, Vertex3.Y, Convert.ToDouble(- Vertex3.Z));
                        GL.Vertex3(Vertex2.X, Vertex2.Y, Convert.ToDouble(- Vertex2.Z));
                        GL.End();

                        if ( MouseOverTerrain.Tile.Normal.X == Selected_Tile_A.X )
                        {
                            if ( MouseOverTerrain.Tile.Normal.Y <= Selected_Tile_A.Y )
                            {
                                A = MouseOverTerrain.Tile.Normal.Y;
                                B = Selected_Tile_A.Y;
                            }
                            else
                            {
                                A = Selected_Tile_A.Y;
                                B = MouseOverTerrain.Tile.Normal.Y;
                            }
                            X2 = Selected_Tile_A.X;
                            for ( Y2 = A; Y2 <= B; Y2++ )
                            {
                                Vertex0.X = X2 * Constants.TerrainGridSpacing;
                                Vertex0.Y = Convert.ToDouble(Terrain.Vertices[X2, Y2].Height * HeightMultiplier);
                                Vertex0.Z = - Y2 * Constants.TerrainGridSpacing;
                                Vertex1.X = (X2 + 1) * Constants.TerrainGridSpacing;
                                Vertex1.Y = Convert.ToDouble(Terrain.Vertices[X2 + 1, Y2].Height * HeightMultiplier);
                                Vertex1.Z = - Y2 * Constants.TerrainGridSpacing;
                                Vertex2.X = X2 * Constants.TerrainGridSpacing;
                                Vertex2.Y = Convert.ToDouble(Terrain.Vertices[X2, Y2 + 1].Height * HeightMultiplier);
                                Vertex2.Z = - (Y2 + 1) * Constants.TerrainGridSpacing;
                                Vertex3.X = (X2 + 1) * Constants.TerrainGridSpacing;
                                Vertex3.Y = Convert.ToDouble(Terrain.Vertices[X2 + 1, Y2 + 1].Height * HeightMultiplier);
                                Vertex3.Z = - (Y2 + 1) * Constants.TerrainGridSpacing;
                                GL.Begin(BeginMode.LineLoop);
                                GL.Color3(0.0F, 1.0F, 1.0F);
                                GL.Vertex3(Vertex0.X, Vertex0.Y, Convert.ToDouble(- Vertex0.Z));
                                GL.Vertex3(Vertex1.X, Vertex1.Y, Convert.ToDouble(- Vertex1.Z));
                                GL.Vertex3(Vertex3.X, Vertex3.Y, Convert.ToDouble(- Vertex3.Z));
                                GL.Vertex3(Vertex2.X, Vertex2.Y, Convert.ToDouble(- Vertex2.Z));
                                GL.End();
                            }
                        }
                        else if ( MouseOverTerrain.Tile.Normal.Y == Selected_Tile_A.Y )
                        {
                            if ( MouseOverTerrain.Tile.Normal.X <= Selected_Tile_A.X )
                            {
                                A = MouseOverTerrain.Tile.Normal.X;
                                B = Selected_Tile_A.X;
                            }
                            else
                            {
                                A = Selected_Tile_A.X;
                                B = MouseOverTerrain.Tile.Normal.X;
                            }
                            Y2 = Selected_Tile_A.Y;
                            for ( X2 = A; X2 <= B; X2++ )
                            {
                                Vertex0.X = X2 * Constants.TerrainGridSpacing;
                                Vertex0.Y = Convert.ToDouble(Terrain.Vertices[X2, Y2].Height * HeightMultiplier);
                                Vertex0.Z = - Y2 * Constants.TerrainGridSpacing;
                                Vertex1.X = (X2 + 1) * Constants.TerrainGridSpacing;
                                Vertex1.Y = Convert.ToDouble(Terrain.Vertices[X2 + 1, Y2].Height * HeightMultiplier);
                                Vertex1.Z = - Y2 * Constants.TerrainGridSpacing;
                                Vertex2.X = X2 * Constants.TerrainGridSpacing;
                                Vertex2.Y = Convert.ToDouble(Terrain.Vertices[X2, Y2 + 1].Height * HeightMultiplier);
                                Vertex2.Z = - (Y2 + 1) * Constants.TerrainGridSpacing;
                                Vertex3.X = (X2 + 1) * Constants.TerrainGridSpacing;
                                Vertex3.Y = Convert.ToDouble(Terrain.Vertices[X2 + 1, Y2 + 1].Height * HeightMultiplier);
                                Vertex3.Z = - (Y2 + 1) * Constants.TerrainGridSpacing;
                                GL.Begin(BeginMode.LineLoop);
                                GL.Color3(0.0F, 1.0F, 1.0F);
                                GL.Vertex3(Vertex0.X, Vertex0.Y, Convert.ToDouble(- Vertex0.Z));
                                GL.Vertex3(Vertex1.X, Vertex1.Y, Convert.ToDouble(- Vertex1.Z));
                                GL.Vertex3(Vertex3.X, Vertex3.Y, Convert.ToDouble(- Vertex3.Z));
                                GL.Vertex3(Vertex2.X, Vertex2.Y, Convert.ToDouble(- Vertex2.Z));
                                GL.End();
                            }
                        }
                    }
                    else
                    {
                        X2 = MouseOverTerrain.Tile.Normal.X;
                        Y2 = MouseOverTerrain.Tile.Normal.Y;

                        Vertex0.X = X2 * Constants.TerrainGridSpacing;
                        Vertex0.Y = Convert.ToDouble(Terrain.Vertices[X2, Y2].Height * HeightMultiplier);
                        Vertex0.Z = - Y2 * Constants.TerrainGridSpacing;
                        Vertex1.X = (X2 + 1) * Constants.TerrainGridSpacing;
                        Vertex1.Y = Convert.ToDouble(Terrain.Vertices[X2 + 1, Y2].Height * HeightMultiplier);
                        Vertex1.Z = - Y2 * Constants.TerrainGridSpacing;
                        Vertex2.X = X2 * Constants.TerrainGridSpacing;
                        Vertex2.Y = Convert.ToDouble(Terrain.Vertices[X2, Y2 + 1].Height * HeightMultiplier);
                        Vertex2.Z = - (Y2 + 1) * Constants.TerrainGridSpacing;
                        Vertex3.X = (X2 + 1) * Constants.TerrainGridSpacing;
                        Vertex3.Y = Convert.ToDouble(Terrain.Vertices[X2 + 1, Y2 + 1].Height * HeightMultiplier);
                        Vertex3.Z = - (Y2 + 1) * Constants.TerrainGridSpacing;
                        GL.Begin(BeginMode.LineLoop);
                        GL.Color3(0.0F, 1.0F, 1.0F);
                        GL.Vertex3(Vertex0.X, Vertex0.Y, Convert.ToDouble(- Vertex0.Z));
                        GL.Vertex3(Vertex1.X, Vertex1.Y, Convert.ToDouble(- Vertex1.Z));
                        GL.Vertex3(Vertex3.X, Vertex3.Y, Convert.ToDouble(- Vertex3.Z));
                        GL.Vertex3(Vertex2.X, Vertex2.Y, Convert.ToDouble(- Vertex2.Z));
                        GL.End();
                    }
                    DebugGLError("Line brush");
                }

                //draw mouseover tiles

                clsBrush ToolBrush = default(clsBrush);

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
                    clsDrawTileOutline DrawTileOutline = new clsDrawTileOutline();
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

                    Vertex0.X = MouseOverTerrain.Vertex.Normal.X * Constants.TerrainGridSpacing;
                    Vertex0.Y = Convert.ToDouble(Terrain.Vertices[MouseOverTerrain.Vertex.Normal.X, MouseOverTerrain.Vertex.Normal.Y].Height * HeightMultiplier);
                    Vertex0.Z = - MouseOverTerrain.Vertex.Normal.Y * Constants.TerrainGridSpacing;
                    GL.Begin(BeginMode.Lines);
                    GL.Color3(0.0F, 1.0F, 1.0F);
                    GL.Vertex3(Vertex0.X - 8.0D, Vertex0.Y, Convert.ToDouble(- Vertex0.Z));
                    GL.Vertex3(Vertex0.X + 8.0D, Vertex0.Y, Convert.ToDouble(- Vertex0.Z));
                    GL.Vertex3(Vertex0.X, Vertex0.Y, Convert.ToDouble(- Vertex0.Z - 8.0D));
                    GL.Vertex3(Vertex0.X, Vertex0.Y, - Vertex0.Z + 8.0D);
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
                    clsDrawVertexMarker DrawVertexMarker = new clsDrawVertexMarker();
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
                App.VisionSectors.PerformActionMapSectors(DrawObjects, DrawCentreSector);
                GL.Disable(EnableCap.Texture2D);
                DebugGLError("Objects");
            }

            if ( MouseOverTerrain != null )
            {
                GL.Enable(EnableCap.Texture2D);
                if ( modTools.Tool == modTools.Tools.ObjectPlace )
                {
                    UnitTypeBase placeObject = Program.frmMainInstance.SingleSelectedObjectTypeBase;
                    if ( placeObject != null )
                    {
                        int Rotation = 0;
                        try
                        {
                            IOUtil.InvariantParse(Program.frmMainInstance.txtNewObjectRotation.Text, ref Rotation);
                            if ( Rotation < 0 | Rotation > 359 )
                            {
                                Rotation = 0;
                            }
                        }
                        catch
                        {
                            Rotation = 0;
                        }
                        WorldPos = TileAlignedPosFromMapPos(MouseOverTerrain.Pos.Horizontal, placeObject.get_GetFootprintSelected(Rotation));
                        GL.PushMatrix();
                        GL.Translate(WorldPos.Horizontal.X - ViewInfo.ViewPos.X, WorldPos.Altitude - ViewInfo.ViewPos.Y + 2.0D,
                            ViewInfo.ViewPos.Z + WorldPos.Horizontal.Y);
                        placeObject.GLDraw(Rotation);
                        GL.PopMatrix();
                    }
                }
                GL.Disable(EnableCap.Texture2D);
                DebugGLError("Mouse over object");
            }

            GL.Disable(EnableCap.DepthTest);

            clsTextLabels ScriptMarkerTextLabels = new clsTextLabels(256);
            clsTextLabel TextLabel = default(clsTextLabel);
            if ( App.Draw_ScriptMarkers )
            {
                clsScriptPosition ScriptPosition = default(clsScriptPosition);
                clsScriptArea ScriptArea = default(clsScriptArea);
                GL.PushMatrix();
                GL.Translate(Convert.ToDouble(- ViewInfo.ViewPos.X), Convert.ToDouble(- ViewInfo.ViewPos.Y), ViewInfo.ViewPos.Z);
                foreach ( clsScriptPosition tempLoopVar_ScriptPosition in ScriptPositions )
                {
                    ScriptPosition = tempLoopVar_ScriptPosition;
                    ScriptPosition.GLDraw();
                }
                foreach ( clsScriptArea tempLoopVar_ScriptArea in ScriptAreas )
                {
                    ScriptArea = tempLoopVar_ScriptArea;
                    ScriptArea.GLDraw();
                }
                foreach ( clsScriptPosition tempLoopVar_ScriptPosition in ScriptPositions )
                {
                    ScriptPosition = tempLoopVar_ScriptPosition;
                    if ( ScriptMarkerTextLabels.AtMaxCount() )
                    {
                        break;
                    }
                    XYZ_dbl.X = ScriptPosition.PosX - ViewInfo.ViewPos.X;
                    XYZ_dbl.Z = - ScriptPosition.PosY - ViewInfo.ViewPos.Z;
                    XYZ_dbl.Y = GetTerrainHeight(new XYInt(ScriptPosition.PosX, ScriptPosition.PosY)) - ViewInfo.ViewPos.Y;
                    Matrix3DMath.VectorRotationByMatrix(ViewInfo.ViewAngleMatrix_Inverted, XYZ_dbl, ref XYZ_dbl2);
                    if ( ViewInfo.Pos_Get_Screen_XY(XYZ_dbl2, ref ScreenPos) )
                    {
                        if ( ScreenPos.X >= 0 & ScreenPos.X <= GLSize.X & ScreenPos.Y >= 0 & ScreenPos.Y <= GLSize.Y )
                        {
                            TextLabel = new clsTextLabel();
                            TextLabel.Colour.Red = 1.0F;
                            TextLabel.Colour.Green = 1.0F;
                            TextLabel.Colour.Blue = 0.5F;
                            TextLabel.Colour.Alpha = 0.75F;
                            TextLabel.TextFont = App.UnitLabelFont;
                            TextLabel.SizeY = SettingsManager.Settings.FontSize;
                            TextLabel.Pos = ScreenPos;
                            TextLabel.Text = ScriptPosition.Label;
                            ScriptMarkerTextLabels.Add(TextLabel);
                        }
                    }
                }
                DebugGLError("Script positions");
                foreach ( clsScriptArea tempLoopVar_ScriptArea in ScriptAreas )
                {
                    ScriptArea = tempLoopVar_ScriptArea;
                    if ( ScriptMarkerTextLabels.AtMaxCount() )
                    {
                        break;
                    }
                    XYZ_dbl.X = ScriptArea.PosAX - ViewInfo.ViewPos.X;
                    XYZ_dbl.Z = - ScriptArea.PosAY - ViewInfo.ViewPos.Z;
                    XYZ_dbl.Y = GetTerrainHeight(new XYInt(ScriptArea.PosAX, ScriptArea.PosAY)) - ViewInfo.ViewPos.Y;
                    Matrix3DMath.VectorRotationByMatrix(ViewInfo.ViewAngleMatrix_Inverted, XYZ_dbl, ref XYZ_dbl2);
                    if ( ViewInfo.Pos_Get_Screen_XY(XYZ_dbl2, ref ScreenPos) )
                    {
                        if ( ScreenPos.X >= 0 & ScreenPos.X <= GLSize.X & ScreenPos.Y >= 0 & ScreenPos.Y <= GLSize.Y )
                        {
                            TextLabel = new clsTextLabel();
                            TextLabel.Colour.Red = 1.0F;
                            TextLabel.Colour.Green = 1.0F;
                            TextLabel.Colour.Blue = 0.5F;
                            TextLabel.Colour.Alpha = 0.75F;
                            TextLabel.TextFont = App.UnitLabelFont;
                            TextLabel.SizeY = SettingsManager.Settings.FontSize;
                            TextLabel.Pos = ScreenPos;
                            TextLabel.Text = ScriptArea.Label;
                            ScriptMarkerTextLabels.Add(TextLabel);
                        }
                    }
                }
                GL.PopMatrix();

                DebugGLError("Script areas");
            }

            clsTextLabels MessageTextLabels = new clsTextLabels(24);

            B = 0;
            for ( A = Math.Max(Messages.Count - MessageTextLabels.MaxCount, 0); A <= Messages.Count - 1; A++ )
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
                    TextLabel.Pos.X = 32 + MinimapSizeXY.X;
                    TextLabel.Pos.Y = 32 + (int)(Math.Ceiling((decimal)(B * TextLabel.SizeY)));
                    TextLabel.Text = Convert.ToString(Messages[A].Text);
                    MessageTextLabels.Add(TextLabel);
                    B++;
                }
            }

            //draw unit selection

            GL.Begin(BeginMode.Quads);
            foreach ( clsUnit tempLoopVar_Unit in SelectedUnits )
            {
                Unit = tempLoopVar_Unit;
                RGB_sng = GetUnitGroupColour(Unit.UnitGroup);
                ColourA = new sRGBA_sng((1.0F + RGB_sng.Red) / 2.0F, (1.0F + RGB_sng.Green) / 2.0F, (1.0F + RGB_sng.Blue) / 2.0F, 0.75F);
                ColourB = new sRGBA_sng(RGB_sng.Red, RGB_sng.Green, RGB_sng.Blue, 0.75F);
                DrawUnitRectangle(Unit, 8, ColourA, ColourB);
            }
            if ( MouseOverTerrain != null )
            {
                foreach ( clsUnit tempLoopVar_Unit in MouseOverTerrain.Units )
                {
                    Unit = tempLoopVar_Unit;
                    if ( Unit != null && modTools.Tool == modTools.Tools.ObjectSelect )
                    {
                        RGB_sng = GetUnitGroupColour(Unit.UnitGroup);
                        GL.Color4((0.5F + RGB_sng.Red) / 1.5F, (0.5F + RGB_sng.Green) / 1.5F, (0.5F + RGB_sng.Blue) / 1.5F, 0.75F);
                        ColourA = new sRGBA_sng((1.0F + RGB_sng.Red) / 2.0F, (1.0F + RGB_sng.Green) / 2.0F, (1.0F + RGB_sng.Blue) / 2.0F, 0.75F);
                        ColourB = new sRGBA_sng(RGB_sng.Red, RGB_sng.Green, RGB_sng.Blue, 0.875F);
                        DrawUnitRectangle(Unit, 16, ColourA, ColourB);
                    }
                }
            }
            GL.End();

            DebugGLError("Unit selection");

            GL.MatrixMode(MatrixMode.Projection);
            Matrix4 temp_mat2 = Matrix4.CreateOrthographicOffCenter(0.0F, GLSize.X, GLSize.Y, 0.0F, -1.0F, 1.0F);
            GL.LoadMatrix(ref temp_mat2);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            DebugGLError("Text label matrix modes");

            GL.Enable(EnableCap.Texture2D);

            ScriptMarkerTextLabels.Draw();
            DrawObjects.UnitTextLabels.Draw();
            SelectionLabel.Draw();
            MessageTextLabels.Draw();

            DebugGLError("Text labels");

            GL.Disable(EnableCap.Texture2D);

            GL.Disable(EnableCap.Blend);

            //draw minimap

            GL.MatrixMode(MatrixMode.Projection);
            Matrix4 temp_mat3 = Matrix4.CreateOrthographicOffCenter(0.0F, GLSize.X, 0.0F, GLSize.Y, -1.0F, 1.0F);
            GL.LoadMatrix(ref temp_mat3);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            DebugGLError("Minimap matrix modes");

            if ( Minimap_Texture_Size > 0 & ViewInfo.Tiles_Per_Minimap_Pixel > 0.0D )
            {
                GL.Translate(0.0F, GLSize.Y - MinimapSizeXY.Y, 0.0F);

                XYZ_dbl.X = (double)Terrain.TileSize.X / Minimap_Texture_Size;
                XYZ_dbl.Z = (double)Terrain.TileSize.Y / Minimap_Texture_Size;

                if ( Minimap_GLTexture > 0 )
                {
                    GL.Enable(EnableCap.Texture2D);
                    GL.BindTexture(TextureTarget.Texture2D, Minimap_GLTexture);
                    GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (int)TextureEnvMode.Decal);

                    GL.Begin(BeginMode.Quads);

                    GL.TexCoord2(0.0F, 0.0F);
                    GL.Vertex2(0, MinimapSizeXY.Y);

                    GL.TexCoord2((float)XYZ_dbl.X, 0.0F);
                    GL.Vertex2(MinimapSizeXY.X, MinimapSizeXY.Y);

                    GL.TexCoord2((float)XYZ_dbl.X, (float)XYZ_dbl.Z);
                    GL.Vertex2(MinimapSizeXY.X, 0);

                    GL.TexCoord2(0.0F, (float)XYZ_dbl.Z);
                    GL.Vertex2(0, 0);

                    GL.End();

                    GL.Disable(EnableCap.Texture2D);

                    DebugGLError("Minimap");
                }

                //draw minimap border

                GL.LineWidth(1.0F);
                GL.Begin(BeginMode.Lines);
                GL.Color3(0.75F, 0.75F, 0.75F);
                GL.Vertex2(MinimapSizeXY.X, 0.0F);
                GL.Vertex2(MinimapSizeXY.X, MinimapSizeXY.Y);
                GL.Vertex2(0.0F, 0.0F);
                GL.Vertex2(MinimapSizeXY.X, 0.0F);
                GL.End();

                DebugGLError("Minimap border");

                //draw minimap view pos box

                if ( ShowMinimapViewPosBox )
                {
                    dblTemp = Constants.TerrainGridSpacing * ViewInfo.Tiles_Per_Minimap_Pixel;

                    PosA.X = ViewCorner0.X / dblTemp;
                    PosA.Y = MinimapSizeXY.Y + ViewCorner0.Y / dblTemp;
                    PosB.X = ViewCorner1.X / dblTemp;
                    PosB.Y = MinimapSizeXY.Y + ViewCorner1.Y / dblTemp;
                    PosC.X = ViewCorner2.X / dblTemp;
                    PosC.Y = MinimapSizeXY.Y + ViewCorner2.Y / dblTemp;
                    PosD.X = ViewCorner3.X / dblTemp;
                    PosD.Y = MinimapSizeXY.Y + ViewCorner3.Y / dblTemp;

                    GL.LineWidth(1.0F);
                    GL.Begin(BeginMode.LineLoop);
                    GL.Color3(1.0F, 1.0F, 1.0F);
                    GL.Vertex2(PosA.X, PosA.Y);
                    GL.Vertex2(PosB.X, PosB.Y);
                    GL.Vertex2(PosC.X, PosC.Y);
                    GL.Vertex2(PosD.X, PosD.Y);
                    GL.End();

                    DebugGLError("Minimap view position polygon");
                }

                if ( Selected_Area_VertexA != null )
                {
                    DrawIt = false;
                    if ( Selected_Area_VertexB != null )
                    {
                        //area is selected
                        MathUtil.ReorderXY(Selected_Area_VertexA, Selected_Area_VertexB, ref StartXY, ref FinishXY);
                        DrawIt = true;
                    }
                    else if ( modTools.Tool == modTools.Tools.TerrainSelect )
                    {
                        if ( MouseOverTerrain != null )
                        {
                            //selection is changing under mouse
                            MathUtil.ReorderXY(Selected_Area_VertexA, MouseOverTerrain.Vertex.Normal, ref StartXY, ref FinishXY);
                            DrawIt = true;
                        }
                    }
                    if ( DrawIt )
                    {
                        GL.LineWidth(1.0F);
                        PosA.X = StartXY.X / ViewInfo.Tiles_Per_Minimap_Pixel;
                        PosA.Y = MinimapSizeXY.Y - StartXY.Y / ViewInfo.Tiles_Per_Minimap_Pixel;
                        PosB.X = FinishXY.X / ViewInfo.Tiles_Per_Minimap_Pixel;
                        PosB.Y = MinimapSizeXY.Y - StartXY.Y / ViewInfo.Tiles_Per_Minimap_Pixel;
                        PosC.X = FinishXY.X / ViewInfo.Tiles_Per_Minimap_Pixel;
                        PosC.Y = MinimapSizeXY.Y - FinishXY.Y / ViewInfo.Tiles_Per_Minimap_Pixel;
                        PosD.X = StartXY.X / ViewInfo.Tiles_Per_Minimap_Pixel;
                        PosD.Y = MinimapSizeXY.Y - FinishXY.Y / ViewInfo.Tiles_Per_Minimap_Pixel;
                        GL.Begin(BeginMode.LineLoop);
                        GL.Color3(1.0F, 1.0F, 1.0F);
                        GL.Vertex2(PosA.X, PosA.Y);
                        GL.Vertex2(PosB.X, PosB.Y);
                        GL.Vertex2(PosC.X, PosC.Y);
                        GL.Vertex2(PosD.X, PosD.Y);
                        GL.End();

                        DebugGLError("Minimap selection box");
                    }
                }
            }
        }

        private void DebugGLError(string Name)
        {
            if ( App.Debug_GL )
            {
                if ( Messages.Count < 8 )
                {
                    if ( GL.GetError() != ErrorCode.NoError )
                    {
                        clsMessage NewMessage = new clsMessage();
                        NewMessage.Text = "OpenGL Error (" + Name + ")";
                        Messages.Add(NewMessage);
                    }
                }
            }
        }

        public void DrawUnitRectangle(clsUnit Unit, int BorderInsideThickness, sRGBA_sng InsideColour, sRGBA_sng OutsideColour)
        {
            XYInt PosA = new XYInt();
            XYInt PosB = new XYInt();
            int A = 0;
            int Altitude = Unit.Pos.Altitude - ViewInfo.ViewPos.Y;

            GetFootprintTileRangeClamped(Unit.Pos.Horizontal, Unit.TypeBase.get_GetFootprintSelected(Unit.Rotation), ref PosA, ref PosB);
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