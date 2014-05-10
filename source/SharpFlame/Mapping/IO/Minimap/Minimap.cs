using System;
using System.Drawing;
using Ninject;
using NLog;
using SharpFlame.Bitmaps;
using SharpFlame.Colors;
using SharpFlame.Core;
using SharpFlame.Core.Domain;
using SharpFlame.Core.Extensions;
using SharpFlame.Core.Interfaces.Mapping.IO;
using SharpFlame.Mapping.Minimap;
using SharpFlame.Maths;

namespace SharpFlame.Mapping.IO.Minimap
{
    public class MinimapSaver : IIOSaver
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        protected readonly Map map;

        public MinimapSaver(Map newMap)
        {
            map = newMap;
        }

        public Result Save(string path, bool overwrite, bool compress = false) // compress is ignored.
        {
            var returnResult = new Result(string.Format("Saving minimap to \"{0}\".", path), false);
            logger.Info ("Saving minimap to \"{0}\"", path);

            var minimapBitmap = new Bitmap(map.Terrain.TileSize.X, map.Terrain.TileSize.Y);

            var mmc = App.Kernel.Get<MinimapCreator>();

            var texture = new MinimapTexture(new XYInt(map.Terrain.TileSize.X, map.Terrain.TileSize.Y));
            mmc.FillTexture(texture, map);

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

