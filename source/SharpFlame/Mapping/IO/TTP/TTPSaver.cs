using System;
using System.Diagnostics;
using System.IO;
using NLog;
using SharpFlame.Mapping.IO;
using SharpFlame.Core;
using SharpFlame.Core.Interfaces.Mapping.IO;
using SharpFlame.FileIO;

namespace SharpFlame.Mapping.IO.TTP
{
    public class TTPSaver : IIOSaver
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        protected readonly Map map;

        public TTPSaver(Map newMap)
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

