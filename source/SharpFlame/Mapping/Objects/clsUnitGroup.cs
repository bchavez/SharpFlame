

using System;
using SharpFlame.Collections;
using SharpFlame.FileIO;
using SharpFlame.Core;
using SharpFlame.Core.Collections;


namespace SharpFlame.Mapping.Objects
{
    public class clsUnitGroup
    {
        public ConnectedListItem<clsUnitGroup, Map> MapLink;

        public int WZ_StartPos = -1;

        public clsUnitGroup()
        {
            MapLink = new ConnectedListItem<clsUnitGroup, Map>(this);
        }

        public string GetFMapINIPlayerText()
        {
            if ( WZ_StartPos < 0 | WZ_StartPos >= Constants.PlayerCountMax )
            {
                return "scavenger";
            }
            return WZ_StartPos.ToStringInvariant();
        }

        public string GetLNDPlayerText()
        {
            if ( WZ_StartPos < 0 | WZ_StartPos >= Constants.PlayerCountMax )
            {
                return 7.ToStringInvariant();
            }
            return WZ_StartPos.ToStringInvariant();
        }

        public int GetPlayerNum(int PlayerCount)
        {
            if ( WZ_StartPos < 0 | WZ_StartPos >= Constants.PlayerCountMax )
            {
                return Math.Max(PlayerCount, 7);
            }
            return WZ_StartPos;
        }
    }
}