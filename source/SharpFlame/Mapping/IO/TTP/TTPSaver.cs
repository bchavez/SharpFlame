using System;
using System.Diagnostics;
using System.IO;
using NLog;
using SharpFlame.Core;
using SharpFlame.FileIO;
using SharpFlame.Mapping.IO;

namespace SharpFlame.Mapping.IO.TTP
{
    public class TTPSaver : IIOSaver
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        protected readonly clsMap map;

        public TTPSaver(clsMap newMap)
        {
            map = newMap;
        }

        public Result Save(string path, bool overwrite, bool compress = false) // compress is ignored.
        {
            var returnResult = new Result(string.Format("Writing .ttp to \"{0}\"", path), false);
            logger.Info(string.Format("Writing .ttp to \"{0}\"", path));           

            try {
                if (File.Exists(path))
                {
                    if (overwrite)
                    {
                        File.Delete(path);
                    }
                    else
                    {
                        returnResult.ProblemAdd("File already exists.");
                        return returnResult;
                    }
                }

                using (var file = new FileStream(path, FileMode.CreateNew)) {
                    returnResult.Take(Save (file));
                }
            }            
            catch (Exception ex) {
                Debugger.Break ();
                returnResult.ProblemAdd (string.Format ("Failed to create .ttp, failure was: {0}", ex.Message));
                logger.ErrorException ("Failed to create .ttp", ex);
                return returnResult;
            }

            return returnResult;
        }

        public Result Save(Stream stream)
        {            
            var returnResult = new Result("Serializing ttypes.ttp", false);
            logger.Info("Serializing ttypes.ttp");

            var fileTTP = new BinaryWriter(stream, App.ASCIIEncoding);

            IOUtil.WriteText(fileTTP, false, "ttyp");
            fileTTP.Write(8U);
            fileTTP.Write((uint)map.Tileset.TileCount);
            for ( var a = 0; a <= map.Tileset.TileCount - 1; a++ )
            {
                fileTTP.Write((ushort)map.Tile_TypeNum[a]);
            }

            fileTTP.Flush();

            return returnResult;
        }
    }
}

