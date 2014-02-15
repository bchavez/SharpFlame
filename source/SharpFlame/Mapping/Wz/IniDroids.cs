using System;
using System.Collections.Generic;
using SharpFlame.FileIO;
using SharpFlame.FileIO.Ini;
using SharpFlame.Mapping.Objects;
using SharpFlame.Util;

namespace SharpFlame.Mapping.Wz
{
    public class IniDroids : SectionTranslator
    {
        public clsMap ParentMap;

        public class sDroid
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

        public List<sDroid> Droids;
        public int DroidCount;

        public IniDroids() {
        }

        public IniDroids(int newDroidCount, clsMap NewParentMap)
        {
            int a = 0;

            ParentMap = NewParentMap;

            DroidCount = newDroidCount;
            // Droids = new sDroid[DroidCount];
            for ( a = 0; a <= DroidCount - 1; a++ )
            {
                Droids.Add (new sDroid { 
                    HealthPercent = -1,
                    DroidType = -1,
                    Weapons = new string[3]
                });
            }
        }

        public override TranslatorResult Translate(int iNISectionNum, Section.SectionProperty iNIProperty)
        {
            if ( (string)iNIProperty.Name == "id" )
            {
                if ( !IOUtil.InvariantParse(Convert.ToString(iNIProperty.Value), ref Droids[iNISectionNum].ID) )
                {
                    return TranslatorResult.ValueInvalid;
                }
            }
            else if ( (string)iNIProperty.Name == "template" )
            {
                Droids[iNISectionNum].Template = Convert.ToString(iNIProperty.Value);
            }
            else if ( (string)iNIProperty.Name == "startpos" )
            {
                int StartPos = 0;
                if ( !IOUtil.InvariantParse(Convert.ToString(iNIProperty.Value), ref StartPos) )
                {
                    return TranslatorResult.ValueInvalid;
                }
                if ( StartPos < 0 | StartPos >= Constants.PlayerCountMax )
                {
                    return TranslatorResult.ValueInvalid;
                }
                Droids[iNISectionNum].UnitGroup = ParentMap.UnitGroups[StartPos];
            }
            else if ( (string)iNIProperty.Name == "player" )
            {
                if ( iNIProperty.Value.ToLower() != "scavenger" )
                {
                    return TranslatorResult.ValueInvalid;
                }
                Droids[iNISectionNum].UnitGroup = ParentMap.ScavengerUnitGroup;
            }
            else if ( (string)iNIProperty.Name == "name" )
            {
                //ignore
            }
            else if ( (string)iNIProperty.Name == "position" )
            {
                if ( !IOUtil.WorldPosFromINIText(Convert.ToString(iNIProperty.Value), ref Droids[iNISectionNum].Pos) )
                {
                    return TranslatorResult.ValueInvalid;
                }
            }
            else if ( (string)iNIProperty.Name == "rotation" )
            {
                if ( !IOUtil.WZAngleFromINIText(Convert.ToString(iNIProperty.Value), ref Droids[iNISectionNum].Rotation) )
                {
                    return TranslatorResult.ValueInvalid;
                }
            }
            else if ( (string)iNIProperty.Name == "health" )
            {
                int temp_Result3 = Droids[iNISectionNum].HealthPercent;
                if ( !IOUtil.HealthFromINIText(Convert.ToString(iNIProperty.Value), ref temp_Result3) )
                {
                    return TranslatorResult.ValueInvalid;
                }
            }
            else if ( (string)iNIProperty.Name == "droidtype" )
            {
                if ( !IOUtil.InvariantParse(Convert.ToString(iNIProperty.Value), ref Droids[iNISectionNum].DroidType) )
                {
                    return TranslatorResult.ValueInvalid;
                }
            }
            else if ( (string)iNIProperty.Name == "weapons" )
            {
                if ( !IOUtil.InvariantParse(Convert.ToString(iNIProperty.Value), ref Droids[iNISectionNum].WeaponCount) )
                {
                    return TranslatorResult.ValueInvalid;
                }
            }
            else if ( (string)iNIProperty.Name == "parts\\body" )
            {
                Droids[iNISectionNum].Body = Convert.ToString(iNIProperty.Value);
            }
            else if ( (string)iNIProperty.Name == "parts\\propulsion" )
            {
                Droids[iNISectionNum].Propulsion = Convert.ToString(iNIProperty.Value);
            }
            else if ( (string)iNIProperty.Name == "parts\\brain" )
            {
                Droids[iNISectionNum].Brain = Convert.ToString(iNIProperty.Value);
            }
            else if ( (string)iNIProperty.Name == "parts\\repair" )
            {
                Droids[iNISectionNum].Repair = Convert.ToString(iNIProperty.Value);
            }
            else if ( (string)iNIProperty.Name == "parts\\ecm" )
            {
                Droids[iNISectionNum].ECM = Convert.ToString(iNIProperty.Value);
            }
            else if ( (string)iNIProperty.Name == "parts\\sensor" )
            {
                Droids[iNISectionNum].Sensor = Convert.ToString(iNIProperty.Value);
            }
            else if ( (string)iNIProperty.Name == "parts\\construct" )
            {
                Droids[iNISectionNum].Construct = Convert.ToString(iNIProperty.Value);
            }
            else if ( (string)iNIProperty.Name == "parts\\weapon\\1" )
            {
                Droids[iNISectionNum].Weapons[0] = iNIProperty.Value;
            }
            else if ( (string)iNIProperty.Name == "parts\\weapon\\2" )
            {
                Droids[iNISectionNum].Weapons[1] = iNIProperty.Value;
            }
            else if ( (string)iNIProperty.Name == "parts\\weapon\\3" )
            {
                Droids[iNISectionNum].Weapons[2] = iNIProperty.Value;
            }
            else
            {
                return TranslatorResult.NameUnknown;
            }
            return TranslatorResult.Translated;
        }
    }
}