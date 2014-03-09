#region License
 /*
 The MIT License (MIT)

 Copyright (c) 2013-2014 The SharpFlame Authors.

 Permission is hereby granted, free of charge, to any person obtaining a copy
 of this software and associated documentation files (the "Software"), to deal
 in the Software without restriction, including without limitation the rights
 to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 copies of the Software, and to permit persons to whom the Software is
 furnished to do so, subject to the following conditions:

 The above copyright notice and this permission notice shall be included in
 all copies or substantial portions of the Software.

 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 THE SOFTWARE.
 */
#endregion

using System;
using System.Diagnostics;
using Appccelerate.EventBroker;
using Appccelerate.EventBroker.Handlers;
using Eto.Forms;
using Eto.Gl;
using Ninject;
using Ninject.Extensions.Logging;
using OpenTK.Graphics.OpenGL;
using SharpFlame.Core;
using SharpFlame.Core.Domain;
using SharpFlame.Core.Domain.Colors;
using SharpFlame.Domain;
using SharpFlame.Infrastructure;
using SharpFlame.Mapping;
using SharpFlame.Mapping.Tools;
using SharpFlame.Maths;
using SharpFlame.Settings;
using SharpFlame.Util;

namespace SharpFlame.Gui.Sections
{
    public class MainMapView : Panel, IInitializable
	{
        private ILogger logger;

        [Inject]
        internal ILoggerFactory logFactory 
        { 
            set { logger = value.GetCurrentClassLogger(); } 
        }

        [Inject]
        internal KeyboardManager KeyboardManager { get; set; }

        [Inject, Named(NamedBinding.MapView)]
        public GLSurface GLSurface { get; set; }

        [Inject]
        internal IEventBroker EventBroker { get; set; }

        private Map map;
        public Map Map { 
            get { return map; }
            set {
                if(map != null)
                {
                    GLSurface.MouseDown -= map.ViewInfo.HandleMouseDown;
                    GLSurface.MouseUp -= map.ViewInfo.HandleMouseUp;
                    GLSurface.MouseMove -= map.ViewInfo.HandleMouseMove;
                }

                map = value;
                GLSurface.MouseDown += map.ViewInfo.HandleMouseDown;
                GLSurface.MouseUp += map.ViewInfo.HandleMouseUp;
                GLSurface.MouseMove += map.ViewInfo.HandleMouseMove;

                DrawLater();
            }
        }

        private UITimer tmrDraw;
        private UITimer tmrKey;

        private bool drawPending = false;

        void IInitializable.Initialize()
        {
		    var mainLayout = new DynamicLayout();
            mainLayout.AddSeparateRow(
                new Label { }
            );
            mainLayout.Add(GLSurface, true, true);
            mainLayout.AddSeparateRow(
                new Label { }
            );

		    Content = mainLayout;           

            setBindings();
            EventBroker.Register(this);
		}

        void setBindings() 
        {
            this.GLSurface.KeyDown += this.KeyboardManager.HandleKeyDown;
            this.GLSurface.KeyUp += this.KeyboardManager.HandleKeyUp;

            this.GLSurface.Initialized += initalizeGlSurface;
            this.GLSurface.Resize += resizeMapView;
        }

        private void initalizeGlSurface(object sender, EventArgs e)
        {
            this.GLSurface.MakeCurrent();

            GL.PixelStore(PixelStoreParameter.PackAlignment, 1);
            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
            GL.ClearColor(0.0F, 0.0F, 0.0F, 1.0F);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.ShadeModel(ShadingModel.Smooth);
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
            GL.Enable(EnableCap.DepthTest);

            var ambient = new float[4];
            var specular = new float[4];
            var diffuse = new float[4];

            ambient[0] = 0.333333343F;
            ambient[1] = 0.333333343F;
            ambient[2] = 0.333333343F;
            ambient[3] = 1.0F;
            specular[0] = 0.6666667F;
            specular[1] = 0.6666667F;
            specular[2] = 0.6666667F;
            specular[3] = 1.0F;
            diffuse[0] = 0.75F;
            diffuse[1] = 0.75F;
            diffuse[2] = 0.75F;
            diffuse[3] = 1.0F;
            GL.Light(LightName.Light0, LightParameter.Diffuse, diffuse);
            GL.Light(LightName.Light0, LightParameter.Specular, specular);
            GL.Light(LightName.Light0, LightParameter.Ambient, ambient);

            ambient[0] = 0.25F;
            ambient[1] = 0.25F;
            ambient[2] = 0.25F;
            ambient[3] = 1.0F;
            specular[0] = 0.5F;
            specular[1] = 0.5F;
            specular[2] = 0.5F;
            specular[3] = 1.0F;
            diffuse[0] = 0.5625F;
            diffuse[1] = 0.5625F;
            diffuse[2] = 0.5625F;
            diffuse[3] = 1.0F;
            GL.Light(LightName.Light1, LightParameter.Diffuse, diffuse);
            GL.Light(LightName.Light1, LightParameter.Specular, specular);
            GL.Light(LightName.Light1, LightParameter.Ambient, ambient);

            var mat_diffuse = new float[4];
            var mat_specular = new float[4];
            var mat_ambient = new float[4];
            var mat_shininess = new float[1];

            mat_specular[0] = 0.0F;
            mat_specular[1] = 0.0F;
            mat_specular[2] = 0.0F;
            mat_specular[3] = 0.0F;
            mat_ambient[0] = 1.0F;
            mat_ambient[1] = 1.0F;
            mat_ambient[2] = 1.0F;
            mat_ambient[3] = 1.0F;
            mat_diffuse[0] = 1.0F;
            mat_diffuse[1] = 1.0F;
            mat_diffuse[2] = 1.0F;
            mat_diffuse[3] = 1.0F;
            mat_shininess[0] = 0.0F;

            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Ambient, mat_ambient);
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Specular, mat_specular);
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Diffuse, mat_diffuse);
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Shininess, mat_shininess);

            tmrDraw = new UITimer { Interval = 0.016 }; // Every Millisecond.
            tmrDraw.Elapsed += timedDraw;
            tmrDraw.Start();

            tmrKey = new UITimer { Interval = 0.030 }; // Every 30 milliseconds.
            tmrKey.Elapsed += timedKey;
            tmrKey.Start();
        }

        private void resizeMapView(object sender, EventArgs e)
        {
            GLSurface.MakeCurrent();

            var glSize = GLSurface.Size;

            // send the resize event to the Graphics card.
            GL.Viewport(0, 0, glSize.Width, glSize.Height);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.Flush();

            GLSurface.SwapBuffers();

            if(Map != null)
            {
                Map.ViewInfo.FovCalc();
            }

            DrawLater();              
        }

        public void DrawLater()
        {
            drawPending = true;
        }            

        private void timedDraw(object sender, EventArgs e)
        {
            if(!drawPending || 
                Map == null || 
                !this.GLSurface.IsInitialized)
            {
                return;
            }

            this.GLSurface.MakeCurrent();

            var bgColour = new SRgb();
            if ( map.Tileset == null )
            {
                bgColour.Red = 0.5F;
                bgColour.Green = 0.5F;
                bgColour.Blue = 0.5F;
            }
            else
            {
                bgColour = map.Tileset.BGColour;
            }

            GL.ClearColor(bgColour.Red, bgColour.Green, bgColour.Blue, 1.0F);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            try {
                Map.GLDraw();
            } catch (Exception ex) {
                Debugger.Break();
                logger.Error(ex, "Got an exception");
            }

            GL.Flush();
            this.GLSurface.SwapBuffers();

            drawPending = false;
        }

        private void timedKey(object sender, EventArgs e)
        {
            if ( Map == null )
            {
                return;
            }

            double Rate = 0;
            double Zoom = 0;
            double Move = 0;
            double Roll = 0;
            double Pan = 0;
            double OrbitRate = 0;

            if ( KeyboardManager.Keys[KeyboardKeys.ViewFast].Active )
            {
                if (KeyboardManager.Keys[KeyboardKeys.ViewSlow].Active)
                {
                    Rate = 8.0D;
                }
                else
                {
                    Rate = 4.0D;
                }
            }
            else if (KeyboardManager.Keys[KeyboardKeys.ViewSlow].Active)
            {
                Rate = 0.25D;
            }
            else
            {
                Rate = 1.0D;
            }

            Zoom = tmrKey.Interval * 1000 * 0.002D;
            Move = tmrKey.Interval * 1000 * Rate / 2048.0D;
            Roll = 5.0D * MathUtil.RadOf1Deg;
            Pan = 1.0D / 16.0D;
            OrbitRate = 1.0D / 32.0D;

            if ( Map != null )
            {
                Map.ViewInfo.TimedActions(Zoom, Move, Pan, Roll, OrbitRate);

                if ( Map.CheckMessages() )
                {
                    DrawLater();
                }
            }
        }

        protected override void Dispose(bool disposing) 
        {
            if(disposing)
            {
                if(GLSurface.IsInitialized)
                {
                    tmrDraw.Stop();
                    tmrDraw = null;

                    tmrKey.Stop();
                    tmrKey = null;
                }
            }

            base.Dispose(disposing);
        }

        [EventSubscription(KeyboardManagerEvents.OnKeyDown, typeof(OnPublisher))]
        public void HandleKeyDown(object sender, KeyboardEventArgs e)
        {
            if(map == null)
            {
                return;
            }

            if(KeyboardManager.Keys[KeyboardKeys.VisionRadius6].Active)
            {
                App.VisionRadius_2E = 6;
                App.VisionRadius_2E_Changed();
                DrawLater();
            }
            if(KeyboardManager.Keys[KeyboardKeys.VisionRadius7].Active)
            {
                App.VisionRadius_2E = 7;
                App.VisionRadius_2E_Changed();
                DrawLater();
            }
            if(KeyboardManager.Keys[KeyboardKeys.VisionRadius8].Active)
            {
                App.VisionRadius_2E = 8;
                App.VisionRadius_2E_Changed();
                DrawLater();
            }
            if(KeyboardManager.Keys[KeyboardKeys.VisionRadius9].Active)
            {
                App.VisionRadius_2E = 9;
                App.VisionRadius_2E_Changed();
                DrawLater();
            }
            if(KeyboardManager.Keys[KeyboardKeys.VisionRadius10].Active)
            {
                App.VisionRadius_2E = 10;
                App.VisionRadius_2E_Changed();
                DrawLater();
            }
            if(KeyboardManager.Keys[KeyboardKeys.VisionRadius11].Active)
            {
                App.VisionRadius_2E = 11;
                App.VisionRadius_2E_Changed();
                DrawLater();
            }
            if(KeyboardManager.Keys[KeyboardKeys.VisionRadius12].Active)
            {
                App.VisionRadius_2E = 12;
                App.VisionRadius_2E_Changed();
                DrawLater();
            }
            if(KeyboardManager.Keys[KeyboardKeys.VisionRadius13].Active)
            {
                App.VisionRadius_2E = 13;
                App.VisionRadius_2E_Changed();
                DrawLater();
            }
            if(KeyboardManager.Keys[KeyboardKeys.VisionRadius14].Active)
            {
                App.VisionRadius_2E = 14;
                App.VisionRadius_2E_Changed();
                DrawLater();
            }
            if(KeyboardManager.Keys[KeyboardKeys.VisionRadius15].Active)
            {
                App.VisionRadius_2E = 15;
                App.VisionRadius_2E_Changed();
                DrawLater();
            }

            if (KeyboardManager.Keys[KeyboardKeys.ViewMoveMode].Active)
            {
                if ( App.ViewMoveType == ViewMoveType.Free )
                {
                    App.ViewMoveType = ViewMoveType.RTS;
                }
                else if ( App.ViewMoveType == ViewMoveType.RTS )
                {
                    App.ViewMoveType = ViewMoveType.Free;
                }
            }
            if (KeyboardManager.Keys[KeyboardKeys.ViewRotateMode].Active)
            {
                App.RTSOrbit = !App.RTSOrbit;
            }
            if (KeyboardManager.Keys[KeyboardKeys.ViewReset].Active)
            {
                var matrixA = new Matrix3DMath.Matrix3D();

                map.ViewInfo.FovMultiplierSet(App.SettingsManager.FOVDefault);
                if ( App.ViewMoveType == ViewMoveType.Free )
                {
                    Matrix3DMath.MatrixSetToXAngle(matrixA, Math.Atan(2.0D));
                    map.ViewInfo.ViewAngleSetRotate(matrixA);
                }
                else if ( App.ViewMoveType == ViewMoveType.RTS )
                {
                    Matrix3DMath.MatrixSetToXAngle(matrixA, Math.Atan(2.0D));
                    map.ViewInfo.ViewAngleSetRotate(matrixA);
                }
            }
            if (KeyboardManager.Keys[KeyboardKeys.ShowTextures].Active)
            {
                App.Draw_TileTextures = !App.Draw_TileTextures;
                DrawLater();
            }
            if (KeyboardManager.Keys[KeyboardKeys.ShowWireframe].Active)
            {
                App.Draw_TileWireframe = !App.Draw_TileWireframe;
                DrawLater();
            }
            if (KeyboardManager.Keys[KeyboardKeys.ShowObjects].Active)
            {
                App.Draw_Units = !App.Draw_Units;
                
                var sectorNum = new XYInt();
                for (var y = 0; y <= map.SectorCount.Y - 1; y++ )
                {
                    for (var  x = 0; x <= map.SectorCount.X - 1; x++ )
                    {
                        foreach ( var connection in map.Sectors[x, y].Units )
                        {    
                            var Unit = connection.Unit;
                            if ( Unit.TypeBase.Type == UnitType.PlayerStructure )
                            {
                                if ( ((StructureTypeBase)Unit.TypeBase).StructureBasePlate != null )
                                {
                                    sectorNum.X = x;
                                    sectorNum.Y = y;
                                    map.SectorGraphicsChanges.Changed(sectorNum);
                                    break;
                                }
                            }
                        }
                    }
                }
                map.Update();
                DrawLater();
            }
            if (KeyboardManager.Keys[KeyboardKeys.ShowLabels].Active)
            {
                App.Draw_ScriptMarkers = !App.Draw_ScriptMarkers;
                DrawLater();
            }
            if (KeyboardManager.Keys[KeyboardKeys.ShowLighting].Active)
            {
                if ( App.Draw_Lighting == DrawLighting.Off )
                {
                    App.Draw_Lighting = DrawLighting.Half;
                }
                else if ( App.Draw_Lighting == DrawLighting.Half )
                {
                    App.Draw_Lighting = DrawLighting.Normal;
                }
                else if ( App.Draw_Lighting == DrawLighting.Normal )
                {
                    App.Draw_Lighting = DrawLighting.Off;
                }
                DrawLater();
            }

            var mouseOverTerrain = map.ViewInfo.GetMouseOverTerrain();
            if ( modTools.Tool == modTools.Tools.TextureBrush )
            {
                if ( mouseOverTerrain != null )
                {
                    if (KeyboardManager.Keys[KeyboardKeys.Clockwise].Active)
                    {
                        map.ViewInfo.ApplyTextureClockwise();
                    }
                    if (KeyboardManager.Keys[KeyboardKeys.CounterClockwise].Active)
                    {
                        map.ViewInfo.ApplyTextureCounterClockwise();
                    }
                    if (KeyboardManager.Keys[KeyboardKeys.TextureFlip].Active)
                    {
                        map.ViewInfo.ApplyTextureFlipX();
                    }
                    if (KeyboardManager.Keys[KeyboardKeys.TriangleFlip].Active)
                    {
                        map.ViewInfo.ApplyTriFlip();
                    }
                }
            }
            if ( modTools.Tool == modTools.Tools.ObjectSelect )
            {
                if (KeyboardManager.Keys[KeyboardKeys.DeleteObjects].Active)
                {
                    if ( map.SelectedUnits.Count > 0 )
                    {
                        foreach ( var unit in map.SelectedUnits.GetItemsAsSimpleList() )
                        {
                            
                            map.UnitRemoveStoreChange(unit.MapLink.ArrayPosition);
                        }
                        Program.frmMainInstance.SelectedObject_Changed();
                        map.UndoStepCreate("Object Deleted");
                        map.Update();
                        map.MinimapMakeLater();
                        DrawLater();
                    }
                }
                if (KeyboardManager.Keys[KeyboardKeys.MoveObjects].Active)
                {
                    if ( mouseOverTerrain != null )
                    {
                        if ( map.SelectedUnits.Count > 0 )
                        {
                            var centre = App.CalcUnitsCentrePos(map.SelectedUnits.GetItemsAsSimpleList());
                            var offset = new XYInt();
                            offset.X = ((int)(Math.Round(Convert.ToDouble((mouseOverTerrain.Pos.Horizontal.X - centre.X) / Constants.TerrainGridSpacing)))) *
                                       Constants.TerrainGridSpacing;
                            offset.Y = ((int)(Math.Round(Convert.ToDouble((mouseOverTerrain.Pos.Horizontal.Y - centre.Y) / Constants.TerrainGridSpacing)))) *
                                       Constants.TerrainGridSpacing;
                            var objectPosOffset = new clsObjectPosOffset
                                {
                                    Map = map,
                                    Offset = offset
                                };
                            map.SelectedUnitsAction(objectPosOffset);

                            map.UndoStepCreate("Objects Moved");
                            map.Update();
                            map.MinimapMakeLater();
                            Program.frmMainInstance.SelectedObject_Changed();
                            DrawLater();
                        }
                    }
                }
                if (KeyboardManager.Keys[KeyboardKeys.Clockwise].Active)
                {
                    var objectRotationOffset = new clsObjectRotationOffset
                        {
                            Map = map,
                            Offset = -90
                        };
                    map.SelectedUnitsAction(objectRotationOffset);
                    map.Update();
                    Program.frmMainInstance.SelectedObject_Changed();
                    map.UndoStepCreate("Object Rotated");
                    DrawLater();
                }
                if (KeyboardManager.Keys[KeyboardKeys.CounterClockwise].Active)
                {
                    var objectRotationOffset = new clsObjectRotationOffset
                        {
                            Map = map,
                            Offset = 90
                        };
                    map.SelectedUnitsAction(objectRotationOffset);
                    map.Update();
                    Program.frmMainInstance.SelectedObject_Changed();
                    map.UndoStepCreate("Object Rotated");
                    DrawLater();
                }
            }

            if (KeyboardManager.Keys[KeyboardKeys.ObjectSelectTool].Active)
            {
                modTools.Tool = modTools.Tools.ObjectSelect;
                DrawLater();
            }

            if (KeyboardManager.Keys[KeyboardKeys.PreviousTool].Active)
            {
                modTools.Tool = modTools.PreviousTool;
                DrawLater();
            }
        }            
	}
}

