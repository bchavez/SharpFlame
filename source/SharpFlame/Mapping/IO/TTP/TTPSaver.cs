using System;
using System.Diagnostics;
using System.IO;
using Ninject.Extensions.Logging;
using SharpFlame.Mapping.IO;
using SharpFlame.Core;
using SharpFlame.FileIO;

namespace SharpFlame.Mapping.IO.TTP
{
    public class TTPSaver : IIOSaver
    {
        private readonly ILogger logger;

        public TTPSaver(ILoggerFactory logFactory)
        {
            logger = logFactory.GetCurrentClassLogger();
        }

        public Result Save(string path, Map map, bool overwrite, bool compress = false) // compress is ignored.
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
                    returnResult.Take(Save (file, map));
                }
            }            
            catch (Exception ex) {
                Debugger.Break ();
                returnResult.ProblemAdd (string.Format ("Failed to create .ttp, failure was: {0}", ex.Message));
                logger.Error (ex, "Failed to create .ttp");
                return returnResult;
            }

            return returnResult;
        }

        public Result Save(Stream stream, Map map)
        {            
            var returnResult = new Result("Serializing ttypes.ttp", false);
            logger.Info("Serializing ttypes.ttp");

            var fileTTP = new BinaryWriter(stream, App.ASCIIEncoding);

            IOUtil.WriteText(fileTTP, false, "ttyp");
            fileTTP.Write(8U);
            fileTTP.Write((uint)map.Tileset.Tiles.Count);
            for ( var a = 0; a <= map.Tileset.Tiles.Count - 1; a++ )
            {
                fileTTP.Write((ushort)map.TileTypeNum[a]);
            }

            fileTTP.Flush();

            return returnResult;
        }
    }
}

