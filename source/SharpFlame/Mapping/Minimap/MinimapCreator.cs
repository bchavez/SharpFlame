 
using System;
using Eto.Forms;
using Eto.Gl;
using Ninject;
using OpenTK.Graphics.OpenGL;
using SharpFlame.Core;
using SharpFlame.Core.Domain;
using SharpFlame.Core.Domain.Colors;
using SharpFlame.Core.Extensions;
using SharpFlame.Domain;
using SharpFlame.Infrastructure;
using SharpFlame.Mapping;
using SharpFlame.Mapping.Objects;
using SharpFlame.Maths;
using SharpFlame.Settings;

namespace SharpFlame.Mapping.Minimap
{
    public class MinimapCreator : IDisposable
    {
        private Map map;

        public Map Map
        {
            get { return map; }
            set
            {
                // Delete old Texture
                glDelete();

                map = value;
                if( map != null )
                {
                    // Make new one later
                    Refresh = true;
                }
                else
                {
                    timer.Stop();
                }
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
        private readonly UiOptions.Minimap options;

        public MinimapCreator(IKernel kernel, SettingsManager argSettings, UiOptions.Options argUiOptions)
        {
            this.GLSurface = App.MapGLSurface;
            kernel.Inject(this); // For GLSurface
            settings = argSettings;
            options = argUiOptions.Minimap;
            

            Suppress = false;

            timer = new UITimer {Interval = Constants.MinimapDelay};
            timer.Elapsed += Tick;

            options.PropertyChanged += delegate
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

            FillTexture(texture, Map);

            glDelete();

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

        private void glDelete()
        {
            if( GLTexture != 0 )
            {
                GL.DeleteTextures(1, ref GLTexture);
                GLTexture = 0;
            }
        }

        private SRgb getUnitGroupColour(clsUnitGroup ColourUnitGroup)
        {
            if( ColourUnitGroup.WZ_StartPos < 0 )
            {
                return new SRgb(1.0F, 1.0F, 1.0F);
            }
            return App.PlayerColour[ColourUnitGroup.WZ_StartPos].MinimapColour;
        }

        public void FillTexture(MinimapTexture Texture, Map myMap)
        {
            if( myMap == null )
            {
                return;
            }

            var terrain = myMap.Terrain;
            var tileset = myMap.Tileset;
            var units = myMap.Units;
            var gateways = myMap.Gateways;

            var low = new XYInt();
            var high = new XYInt();
            var footprint = new XYInt();
            var flag = default(bool);
            var unitMap = new bool[Texture.Size.Y, Texture.Size.X];
            var sngTexture = new float[Texture.Size.Y, Texture.Size.X, 3];
            float alpha = 0;
            float antiAlpha = 0;
            var rGBSng = new SRgb();

            if( options.Textures )
            {
                if( tileset != null )
                {
                    for( var Y = 0; Y <= terrain.TileSize.Y - 1; Y++ )
                    {
                        for( var X = 0; X <= terrain.TileSize.X - 1; X++ )
                        {
                            if( terrain.Tiles[X, Y].Texture.TextureNum >= 0 && terrain.Tiles[X, Y].Texture.TextureNum < tileset.Tiles.Count )
                            {
                                sngTexture[Y, X, 0] = tileset.Tiles[terrain.Tiles[X, Y].Texture.TextureNum].AverageColour.Red;
                                sngTexture[Y, X, 1] = tileset.Tiles[terrain.Tiles[X, Y].Texture.TextureNum].AverageColour.Green;
                                sngTexture[Y, X, 2] = tileset.Tiles[terrain.Tiles[X, Y].Texture.TextureNum].AverageColour.Blue;
                            }
                        }
                    }
                }
                if( options.Heights )
                {
                    float Height = 0;
                    for( var Y = 0; Y <= terrain.TileSize.Y - 1; Y++ )
                    {
                        for( var X = 0; X <= terrain.TileSize.X - 1; X++ )
                        {
                            Height =
                                Convert.ToSingle(((terrain.Vertices[X, Y].Height) + terrain.Vertices[X + 1, Y].Height + terrain.Vertices[X, Y + 1].Height +
                                                  terrain.Vertices[X + 1, Y + 1].Height) / 1020.0F);
                            sngTexture[Y, X, 0] = (sngTexture[Y, X, 0] * 2.0F + Height) / 3.0F;
                            sngTexture[Y, X, 1] = (sngTexture[Y, X, 1] * 2.0F + Height) / 3.0F;
                            sngTexture[Y, X, 2] = (sngTexture[Y, X, 2] * 2.0F + Height) / 3.0F;
                        }
                    }
                }
            }
            else if( options.Heights )
            {
                for( var y = 0; y <= terrain.TileSize.Y - 1; y++ )
                {
                    for( var x = 0; x <= terrain.TileSize.X - 1; x++ )
                    {
                        var height =
                            Convert.ToSingle(((terrain.Vertices[x, y].Height) + terrain.Vertices[x + 1, y].Height + terrain.Vertices[x, y + 1].Height +
                                              terrain.Vertices[x + 1, y + 1].Height) / 1020.0F);
                        sngTexture[y, x, 0] = height;
                        sngTexture[y, x, 1] = height;
                        sngTexture[y, x, 2] = height;
                    }
                }
            }
            else
            {
                for( var y = 0; y <= terrain.TileSize.Y - 1; y++ )
                {
                    for( var x = 0; x <= terrain.TileSize.X - 1; x++ )
                    {
                        sngTexture[y, x, 0] = 0.0F;
                        sngTexture[y, x, 1] = 0.0F;
                        sngTexture[y, x, 2] = 0.0F;
                    }
                }
            }
            if( options.Cliffs )
            {
                if( tileset != null )
                {
                    alpha = settings.MinimapCliffColour.Alpha;
                    antiAlpha = 1.0F - alpha;
                    for( var y = 0; y <= terrain.TileSize.Y - 1; y++ )
                    {
                        for( var x = 0; x <= terrain.TileSize.X - 1; x++ )
                        {
                            if( terrain.Tiles[x, y].Texture.TextureNum >= 0 && terrain.Tiles[x, y].Texture.TextureNum < tileset.Tiles.Count )
                            {
                                if( tileset.Tiles[terrain.Tiles[x, y].Texture.TextureNum].DefaultType == Constants.TileTypeNumCliff )
                                {
                                    sngTexture[y, x, 0] = sngTexture[y, x, 0] * antiAlpha + settings.MinimapCliffColour.Red * alpha;
                                    sngTexture[y, x, 1] = sngTexture[y, x, 1] * antiAlpha + settings.MinimapCliffColour.Green * alpha;
                                    sngTexture[y, x, 2] = sngTexture[y, x, 2] * antiAlpha + settings.MinimapCliffColour.Blue * alpha;
                                }
                            }
                        }
                    }
                }
            }
            if( options.Gateways )
            {
                foreach( var gateway in gateways )
                {
                    MathUtil.ReorderXY(gateway.PosA, gateway.PosB, ref low, ref high);
                    for( var y = low.Y; y <= high.Y; y++ )
                    {
                        for( var x = low.X; x <= high.X; x++ )
                        {
                            sngTexture[y, x, 0] = 1.0F;
                            sngTexture[y, x, 1] = 1.0F;
                            sngTexture[y, x, 2] = 0.0F;
                        }
                    }
                }
            }
            if( options.Objects )
            {
                //units that are not selected
                foreach( var unit in units )
                {
                    flag = true;
                    if( unit.TypeBase.UnitType_frmMainSelectedLink.IsConnected )
                    {
                        flag = false;
                    }
                    else
                    {
                        footprint = unit.TypeBase.GetGetFootprintSelected(unit.Rotation);
                    }
                    if( flag )
                    {
                        myMap.GetFootprintTileRangeClamped(unit.Pos.Horizontal, footprint, ref low, ref high);
                        for( var y = low.Y; y <= high.Y; y++ )
                        {
                            for( var x = low.X; x <= high.X; x++ )
                            {
                                if( !unitMap[y, x] )
                                {
                                    unitMap[y, x] = true;
                                    if( settings.MinimapTeamColours )
                                    {
                                        if( settings.MinimapTeamColoursExceptFeatures & unit.TypeBase.Type == UnitType.Feature )
                                        {
                                            sngTexture[y, x, 0] = App.MinimapFeatureColour.Red;
                                            sngTexture[y, x, 1] = App.MinimapFeatureColour.Green;
                                            sngTexture[y, x, 2] = App.MinimapFeatureColour.Blue;
                                        }
                                        else
                                        {
                                            rGBSng = getUnitGroupColour(unit.UnitGroup);
                                            sngTexture[y, x, 0] = rGBSng.Red;
                                            sngTexture[y, x, 1] = rGBSng.Green;
                                            sngTexture[y, x, 2] = rGBSng.Blue;
                                        }
                                    }
                                    else
                                    {
                                        sngTexture[y, x, 0] = sngTexture[y, x, 0] * 0.6666667F + 0.333333343F;
                                        sngTexture[y, x, 1] = sngTexture[y, x, 1] * 0.6666667F;
                                        sngTexture[y, x, 2] = sngTexture[y, x, 2] * 0.6666667F;
                                    }
                                }
                            }
                        }
                    }
                }
                //reset unit map
                for( var y = 0; y <= Texture.Size.Y - 1; y++ )
                {
                    for( var x = 0; x <= Texture.Size.X - 1; x++ )
                    {
                        unitMap[y, x] = false;
                    }
                }
                //units that are selected and highlighted
                alpha = settings.MinimapSelectedObjectsColour.Alpha;
                antiAlpha = 1.0F - alpha;
                foreach( var unit in units )
                {
                    flag = false;
                    if( unit.TypeBase.UnitType_frmMainSelectedLink.IsConnected )
                    {
                        flag = true;
                        footprint = unit.TypeBase.GetGetFootprintSelected(unit.Rotation);
                        footprint.X += 2;
                        footprint.Y += 2;
                    }
                    if( flag )
                    {
                        myMap.GetFootprintTileRangeClamped(unit.Pos.Horizontal, footprint, ref low, ref high);
                        for( var y = low.Y; y <= high.Y; y++ )
                        {
                            for( var x = low.X; x <= high.X; x++ )
                            {
                                if( !unitMap[y, x] )
                                {
                                    unitMap[y, x] = true;
                                    sngTexture[y, x, 0] = sngTexture[y, x, 0] * antiAlpha + settings.MinimapSelectedObjectsColour.Red * alpha;
                                    sngTexture[y, x, 1] = sngTexture[y, x, 1] * antiAlpha + settings.MinimapSelectedObjectsColour.Green * alpha;
                                    sngTexture[y, x, 2] = sngTexture[y, x, 2] * antiAlpha + settings.MinimapSelectedObjectsColour.Blue * alpha;
                                }
                            }
                        }
                    }
                }
            }
            for( var y = 0; y <= terrain.TileSize.Y - 1; y++ )
            {
                for( var x = 0; x <= terrain.TileSize.X - 1; x++ )
                {
                    Texture.set(x, y, new SRgba(
                        sngTexture[y, x, 0],
                        sngTexture[y, x, 1],
                        sngTexture[y, x, 2],
                        1.0F));
                }
            }
        }

        //~MinimapCreator()
        //{
        //    Dispose(false);
        //}

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            Map = null; // Will clean up everthing.
            GC.SuppressFinalize(this);
        }

    }
}

