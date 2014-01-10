using System;
using SharpFlame.Collections;
using SharpFlame.FileIO;

namespace SharpFlame.Mapping.Objects
{
    public class clsUnitGroup
    {
        public clsUnitGroup()
        {
            MapLink = new ConnectedListLink<clsUnitGroup, clsMap>(this);
        }

        public ConnectedListLink<clsUnitGroup, clsMap> MapLink;

        public int WZ_StartPos = -1;

        public string GetFMapINIPlayerText()
        {
            if ( WZ_StartPos < 0 | WZ_StartPos >= Constants.PlayerCountMax )
            {
                return "scavenger";
            }
            else
            {
                return WZ_StartPos.ToStringInvariant();
            }
        }

        public string GetLNDPlayerText()
        {
            if ( WZ_StartPos < 0 | WZ_StartPos >= Constants.PlayerCountMax )
            {
                return 7.ToStringInvariant();
            }
            else
            {
                return WZ_StartPos.ToStringInvariant();
            }
        }

        public int GetPlayerNum(int PlayerCount)
        {
            if ( WZ_StartPos < 0 | WZ_StartPos >= Constants.PlayerCountMax )
            {
                return Math.Max(PlayerCount, 7);
            }
            else
            {
                return WZ_StartPos;
            }
        }
    }
}