using System;
using System.Drawing;
using System.Net.Mime;
using Ninject;
using Ninject.Extensions.Logging;
using SharpFlame.Bitmaps;
using SharpFlame.Colors;
using SharpFlame.Core;
using SharpFlame.Core.Domain;
using SharpFlame.Core.Extensions;
using SharpFlame.Mapping.Minimap;
using SharpFlame.Maths;
using SharpFlame.Settings;

namespace SharpFlame.Mapping.IO.Minimap
{
    public class MinimapSaver : IIOSaver
    {
        private readonly ILogger logger;

        [Inject]
        internal SettingsManager Settings { get; set; }

        [Inject]
        internal UiOptions.MinimapOpts MiniOpts { get; set; }

        public MinimapSaver(ILoggerFactory logFactory)
        {
            logger = logFactory.GetCurrentClassLogger();
        }

        public Result Save(string path, Map map, bool overwrite, bool compress = false) // compress is ignored.
        {
            var returnResult = new Result(string.Format("Saving minimap to \"{0}\".", path), false);
            logger.Info ("Saving minimap to \"{0}\"", path);

            var minimapBitmap = new Bitmap(map.Terrain.TileSize.X, map.Terrain.TileSize.Y);

            var texture = new MinimapTexture(new XYInt(map.Terrain.TileSize.X, map.Terrain.TileSize.Y));
            MinimapRender.FillTexture(texture, map, this.MiniOpts, this.Settings);

            for ( var y = 0; y <= map.Terrain.TileSize.Y - 1; y++ )
            {
                for ( var x = 0; x <= map.Terrain.TileSize.X - 1; x++ )
                {
                    minimapBitmap.SetPixel(x, y,
                        ColorTranslator.FromOle(
                            ColorUtil.OsRgb(
                                MathUtil.ClampSng(Convert.ToSingle(texture.get(x, y).Red * 255.0F), 0.0F, 255.0F).ToInt(),
                                (MathUtil.ClampSng(Convert.ToSingle(texture.get(x, y).Green * 255.0F), 0.0F, 255.0F)).ToInt(),
                                (MathUtil.ClampSng(Convert.ToSingle(texture.get(x, y).Blue * 255.0F), 0.0F, 255.0F)).ToInt()
                                )));
                }
            }

            var subResult = BitmapUtil.SaveBitmap(path, overwrite, minimapBitmap);
            if (!subResult.Success) {
                returnResult.ProblemAdd (subResult.Problem);
            }

            return returnResult;
        }

    }
}

