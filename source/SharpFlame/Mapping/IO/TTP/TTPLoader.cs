using System;
using System.Diagnostics;
using System.IO;
using Ninject;
using Ninject.Extensions.Logging;
using SharpFlame.Mapping.IO;
using SharpFlame.Core;
using SharpFlame.FileIO;

namespace SharpFlame.Mapping.IO.TTP
{
    public class TTPLoader : IIOLoader
    {
        private readonly ILogger logger;
        private readonly IKernel kernel;

        public TTPLoader(IKernel argKernel, ILoggerFactory logFactory)
        {
            kernel = argKernel;
            logger = logFactory.GetCurrentClassLogger();
        }

        public GenericResult<Map> Load(string path, Map map = null)
        {
            if(map == null)
            {
                map = kernel.Get<Map>();
            }

            try {
                using (var file = new BinaryReader(new FileStream(path, FileMode.Open))) {
                    return Load (file, map);
                }
            }
            catch (Exception ex) {
                Debugger.Break ();
                var returnResult = new GenericResult<Map> ("Loading .ttp", false);
                returnResult.ProblemAdd (string.Format ("Failed to open .ttp, failure was: {0}", ex.Message));
                logger.Error (ex, "Failed to open .ttp");
                return returnResult;
            }
        }

        public GenericResult<Map> Load(BinaryReader file, Map map = null)
        {
            var returnResult = new GenericResult<Map> ("Loading .ttp", false);
            logger.Info ("Loading .ttp");

            if(map == null)
            {
                map = kernel.Get<Map>();
            }

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
                for ( A = 0; A <= Math.Min(uintTemp, (uint)map.Tileset.Tiles.Count) - 1; A++ )
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
                logger.Error(ex, "Got an exception");
                return returnResult;
            }

            returnResult.Value = map;
            return returnResult;
        }       
    }
}

