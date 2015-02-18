using System;
using Appccelerate.EventBroker;
using Appccelerate.EventBroker.Handlers;
using Appccelerate.Events;
using Eto.Forms;
using Eto.Gl;
using OpenTK.Graphics.OpenGL;
using SharpFlame.Core;
using SharpFlame.Core.Domain;
using SharpFlame.Core.Extensions;
using SharpFlame.Settings;

namespace SharpFlame.Mapping.Minimap
{
    public class MinimapGl : IDisposable
    {
        private Map map;

        public Map Map
        {
            get { return map; }
        }

        [EventSubscription(EventTopics.OnMapLoad, typeof(OnPublisher))]
        public void HandleMapLoad(Map newMap)
        {
            // Delete old Texture
            GlDelete();

            map = newMap;
            if( map != null )
            {
                // Make new one later
                Refresh = true;
            }
            else
            {
                this.timer.Stop();
            }
        }

        private bool refresh;

        public bool Refresh
        {
            get { return refresh; }
            set
            {
                refresh = value;
                if( refresh && !timer.Started )
                {
                    timer.Start();
                }
            }
        }

        public int GLTexture;
        public int TextureSize { get; private set; }
        public bool Suppress { get; set; }

        internal GLSurface GLSurface { get; set; }

        private readonly UITimer timer;
        private readonly SettingsManager settings;
        private readonly UiOptions.MinimapOpts miniOpts;

        public MinimapGl(SettingsManager argSettings, UiOptions.Options opts, GLSurface mapGl)
        {
            this.GLSurface = mapGl;
            settings = argSettings;
            miniOpts = opts.MinimapOpts;
            

            Suppress = false;

            timer = new UITimer {Interval = Constants.MinimapDelay};
            timer.Elapsed += Tick;

            miniOpts.PropertyChanged += delegate
                {
                    Refresh = true;
                };
        }

        private void Tick(object sender, EventArgs e)
        {
            if( Map != null && Refresh )
            {
                if( !Suppress ) // Try again on next call, let the timer run.
                {
                    Make();
                    Refresh = false;
                }
            }
            else
            {
                timer.Stop();
            }
        }

        private void Make()
        {
            if( Map == null || !GLSurface.IsInitialized )
            {
                return;
            }
            this.GLSurface.MakeCurrent();

            var terrain = Map.Terrain;

            TextureSize = Math.Round(Math.Pow(2.0D, Math.Ceiling(Math.Log(Math.Max(terrain.TileSize.X, terrain.TileSize.Y)) / Math.Log(2.0D)))).ToInt();

            var texture = new MinimapTexture(new XYInt(TextureSize, TextureSize));

            MinimapRender.FillTexture(texture, Map, this.miniOpts, this.settings);

            GlDelete();

            GL.GenTextures(1, out GLTexture);
            GL.BindTexture(TextureTarget.Texture2D, GLTexture);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, TextureSize, TextureSize, 0, PixelFormat.Rgba,
                PixelType.Float, texture.InlinePixels);

            // TODO: Here was a MainMapView.DrawLater not sure its required.
        }

        private void GlDelete()
        {
            if( GLTexture != 0 )
            {
                GL.DeleteTextures(1, ref GLTexture);
                GLTexture = 0;
            }
        }
        
        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            //Map = null; // Will clean up everthing.
            GlDelete();
            this.timer.Stop();

            GC.SuppressFinalize(this);
        }

    }
}