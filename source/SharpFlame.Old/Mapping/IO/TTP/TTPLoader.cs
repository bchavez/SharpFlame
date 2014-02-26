using System;
using System.Diagnostics;
using System.IO;
using NLog;
using SharpFlame.Old.Mapping.IO;
using SharpFlame.Core;
using SharpFlame.Core.Interfaces.Mapping.IO;
using SharpFlame.Old.FileIO;

namespace SharpFlame.Old.Mapping.IO.TTP
{
    public class TTPLoader : IIOLoader
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        protected readonly Map map;

        public TTPLoader(Map newMap)
        {
            map = newMap;
        }

        public Result Load(string path)
        {
            try {
                using (var file = new BinaryReader(new FileStream(path, FileMode.Open))) {
                    return Load (file);
                }
            }
            catch (Exception ex) {
                Debugger.Break ();
                var returnResult = new Result ("Loading .ttp", false);
                returnResult.ProblemAdd (string.Format ("Failed to open .ttp, failure was: {0}", ex.Message));
                logger.ErrorException ("Failed to open .ttp", ex);
                return returnResult;
            }
        }

        public Result Load(BinaryReader file)
        {
            var returnResult = new Result ("Loading .ttp", false);
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
                    map.TileTypeNum[A] = (byte)ushortTemp;
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
    }
}

