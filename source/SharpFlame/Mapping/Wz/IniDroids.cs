using System;
using SharpFlame.FileIO;
using SharpFlame.FileIO.Ini;
using SharpFlame.Mapping.Objects;
using SharpFlame.Util;

namespace SharpFlame.Mapping.Wz
{
    public class IniDroids : SectionTranslator
    {
        private clsMap ParentMap;

        public struct sDroid
        {
            public UInt32 ID;
            public string Template;
            public clsUnitGroup UnitGroup;
            public clsWorldPos Pos;
            public sWZAngle Rotation;
            public int HealthPercent;
            public int DroidType;
            public string Body;
            public string Propulsion;
            public string Brain;
            public string Repair;
            public string ECM;
            public string Sensor;
            public string Construct;
            public string[] Weapons;
            public int WeaponCount;
        }

        public sDroid[] Droids;
        public int DroidCount;

        public IniDroids(int NewDroidCount, clsMap NewParentMap)
        {
            int A = 0;

            ParentMap = NewParentMap;

            DroidCount = NewDroidCount;
            Droids = new sDroid[DroidCount];
            for ( A = 0; A <= DroidCount - 1; A++ )
            {
                Droids[A].HealthPercent = -1;
                Droids[A].DroidType = -1;
                Droids[A].Weapons = new string[3];
            }
        }

        public override TranslatorResult Translate(int INISectionNum, Section.SectionProperty INIProperty)
        {
            if ( (string)INIProperty.Name == "id" )
            {
                if ( !IOUtil.InvariantParse(Convert.ToString(INIProperty.Value), ref Droids[INISectionNum].ID) )
                {
                    return TranslatorResult.ValueInvalid;
                }
            }
            else if ( (string)INIProperty.Name == "template" )
            {
                Droids[INISectionNum].Template = Convert.ToString(INIProperty.Value);
            }
            else if ( (string)INIProperty.Name == "startpos" )
            {
                int StartPos = 0;
                if ( !IOUtil.InvariantParse(Convert.ToString(INIProperty.Value), ref StartPos) )
                {
                    return TranslatorResult.ValueInvalid;
                }
                if ( StartPos < 0 | StartPos >= Constants.PlayerCountMax )
                {
                    return TranslatorResult.ValueInvalid;
                }
                Droids[INISectionNum].UnitGroup = ParentMap.UnitGroups[StartPos];
            }
            else if ( (string)INIProperty.Name == "player" )
            {
                if ( INIProperty.Value.ToLower() != "scavenger" )
                {
                    return TranslatorResult.ValueInvalid;
                }
                Droids[INISectionNum].UnitGroup = ParentMap.ScavengerUnitGroup;
            }
            else if ( (string)INIProperty.Name == "name" )
            {
                //ignore
            }
            else if ( (string)INIProperty.Name == "position" )
            {
                if ( !IOUtil.WorldPosFromINIText(Convert.ToString(INIProperty.Value), ref Droids[INISectionNum].Pos) )
                {
                    return TranslatorResult.ValueInvalid;
                }
            }
            else if ( (string)INIProperty.Name == "rotation" )
            {
                if ( !IOUtil.WZAngleFromINIText(Convert.ToString(INIProperty.Value), ref Droids[INISectionNum].Rotation) )
                {
                    return TranslatorResult.ValueInvalid;
                }
            }
            else if ( (string)INIProperty.Name == "health" )
            {
                int temp_Result3 = Droids[INISectionNum].HealthPercent;
                if ( !IOUtil.HealthFromINIText(Convert.ToString(INIProperty.Value), ref temp_Result3) )
                {
                    return TranslatorResult.ValueInvalid;
                }
            }
            else if ( (string)INIProperty.Name == "droidtype" )
            {
                if ( !IOUtil.InvariantParse(Convert.ToString(INIProperty.Value), ref Droids[INISectionNum].DroidType) )
                {
                    return TranslatorResult.ValueInvalid;
                }
            }
            else if ( (string)INIProperty.Name == "weapons" )
            {
                if ( !IOUtil.InvariantParse(Convert.ToString(INIProperty.Value), ref Droids[INISectionNum].WeaponCount) )
                {
                    return TranslatorResult.ValueInvalid;
                }
            }
            else if ( (string)INIProperty.Name == "parts\\body" )
            {
                Droids[INISectionNum].Body = Convert.ToString(INIProperty.Value);
            }
            else if ( (string)INIProperty.Name == "parts\\propulsion" )
            {
                Droids[INISectionNum].Propulsion = Convert.ToString(INIProperty.Value);
            }
            else if ( (string)INIProperty.Name == "parts\\brain" )
            {
                Droids[INISectionNum].Brain = Convert.ToString(INIProperty.Value);
            }
            else if ( (string)INIProperty.Name == "parts\\repair" )
            {
                Droids[INISectionNum].Repair = Convert.ToString(INIProperty.Value);
            }
            else if ( (string)INIProperty.Name == "parts\\ecm" )
            {
                Droids[INISectionNum].ECM = Convert.ToString(INIProperty.Value);
            }
            else if ( (string)INIProperty.Name == "parts\\sensor" )
            {
                Droids[INISectionNum].Sensor = Convert.ToString(INIProperty.Value);
            }
            else if ( (string)INIProperty.Name == "parts\\construct" )
            {
                Droids[INISectionNum].Construct = Convert.ToString(INIProperty.Value);
            }
            else if ( (string)INIProperty.Name == "parts\\weapon\\1" )
            {
                Droids[INISectionNum].Weapons[0] = INIProperty.Value;
            }
            else if ( (string)INIProperty.Name == "parts\\weapon\\2" )
            {
                Droids[INISectionNum].Weapons[1] = INIProperty.Value;
            }
            else if ( (string)INIProperty.Name == "parts\\weapon\\3" )
            {
                Droids[INISectionNum].Weapons[2] = INIProperty.Value;
            }
            else
            {
                return TranslatorResult.NameUnknown;
            }
            return TranslatorResult.Translated;
        }
    }
}