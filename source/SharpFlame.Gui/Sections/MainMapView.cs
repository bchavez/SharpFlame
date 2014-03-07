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
using Eto.Forms;
using Eto.Gl;
using Ninject;
using Ninject.Extensions.Logging;
using OpenTK.Graphics.OpenGL;
using SharpFlame.Core.Domain.Colors;
using SharpFlame.Gui.Controls;
using SharpFlame.Gui.Infrastructure;
using SharpFlame.Old.Settings;
using SharpFlame.Old.Mapping;

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

        private Map map;
        public Map Map { 
            get { return map; }
            set {
                map = value;
                Draw();
            }
        }

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
		}

        void setBindings() 
        {
            this.GLSurface.GlKeyDown += this.KeyboardManager.HandleKeyDown;
            this.GLSurface.GlKeyUp += this.KeyboardManager.HandleKeyUp;

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
        }

        private void resizeMapView(object sender, EventArgs e)
        {
            this.GLSurface.MakeCurrent();

            var glSize = this.Size;

            // send the resize event to the Graphics card.
            GL.Viewport(0, 0, glSize.Width, glSize.Height);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.Flush();

            if(Map != null)
            {
                Draw();
            } else
            {
                this.GLSurface.SwapBuffers();
            }
        }

        public void Draw()
        {
            if(Map == null || !this.GLSurface.IsInitialized)
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

            Map.GLDraw();

            GL.Flush();
            this.GLSurface.SwapBuffers();
        }            
	}
}

