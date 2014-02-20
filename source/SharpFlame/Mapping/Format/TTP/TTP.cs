using System;
using System.Diagnostics;
using System.IO;
using NLog;
using SharpFlame.FileIO;

namespace SharpFlame.Mapping.Format.TTP
{
    public class TTP
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        protected readonly clsMap map;

        public TTP(clsMap newMap)
        {
            map = newMap;
        }

        public clsResult Load(string path)
        {
            try {
                using (var file = new BinaryReader(new FileStream(path, FileMode.Open))) {
                    return Load (file);
                }
            }
            catch (Exception ex) {
                Debugger.Break ();
                var returnResult = new clsResult ("Loading .ttp", false);
                returnResult.ProblemAdd (string.Format ("Failed to open .ttp, failure was: {0}", ex.Message));
                logger.ErrorException ("Failed to open .ttp", ex);
                return returnResult;
            }
        }

        public clsResult Load(BinaryReader file)
        {
            var returnResult = new clsResult ("Loading .ttp", false);
            logger.Info ("Loading .ttp");

            var strTemp = "";
            UInt32 uintTemp = 0;
            UInt16 ushortTemp = 0;
            var A = 0;

            if (map.Tileset == null) {
                returnResult.ProblemAdd ("Set a tileset first.");
                return returnResult;
            }

            try
            {
                strTemp = IOUtil.ReadOldTextOfLength(file, 4);
                if ( strTemp != "ttyp" )
                {
                    returnResult.ProblemAdd("Incorrect identifier.");
                    return returnResult;
                }

                uintTemp = file.ReadUInt32();
                if ( uintTemp != 8U )
                {
                    returnResult.ProblemAdd("Unknown version.");
                    return returnResult;
                }
                uintTemp = file.ReadUInt32();
                for ( A = 0; A <= ((int)(Math.Min(uintTemp, (uint)map.Tileset.TileCount))) - 1; A++ )
                {
                    ushortTemp = file.ReadUInt16();
                    if ( ushortTemp > 11 )
                    {
                        returnResult.ProblemAdd("Unknown tile type number.");
                        return returnResult;
                    }
                    map.Tile_TypeNum[A] = (byte)ushortTemp;
                }
            }
            catch ( Exception ex )
            {
                Debugger.Break ();
                returnResult.ProblemAdd (ex.Message);
                logger.ErrorException("Got an exception", ex);
                return returnResult;
            }

            return returnResult;
        }

        public clsResult Save(string path, bool overwrite, bool compress = false) // compress is ignored.
        {
            var returnResult = new clsResult(string.Format("Writing .ttp to \"{0}\"", path), false);
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

        public clsResult Save(Stream stream)
        {            
            var returnResult = new clsResult("Serializing ttypes.ttp", false);
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

