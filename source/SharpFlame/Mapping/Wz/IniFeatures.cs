using System;
using SharpFlame.FileIO;
using SharpFlame.FileIO.Ini;
using SharpFlame.Util;

namespace SharpFlame.Mapping.Wz
{
    public class IniFeatures : SectionTranslator
    {
        public struct sFeatures
        {
            public UInt32 ID;
            public string Code;
            public clsWorldPos Pos;
            public sWZAngle Rotation;
            public int HealthPercent;
        }

        public sFeatures[] Features;
        public int FeatureCount;

        public IniFeatures(int NewFeatureCount)
        {
            int A = 0;

            FeatureCount = NewFeatureCount;
            Features = new sFeatures[FeatureCount];
            for ( A = 0; A <= FeatureCount - 1; A++ )
            {
                Features[A].HealthPercent = -1;
            }
        }

        public override TranslatorResult Translate(int INISectionNum, Section.SectionProperty INIProperty)
        {
            if ( (string)INIProperty.Name == "id" )
            {
                UInt32 uintTemp = 0;
                if ( IOUtil.InvariantParse(Convert.ToString(INIProperty.Value), uintTemp) )
                {
                    if ( uintTemp > 0 )
                    {
                        Features[INISectionNum].ID = uintTemp;
                    }
                }
                else
                {
                    return TranslatorResult.ValueInvalid;
                }
            }
            else if ( (string)INIProperty.Name == "name" )
            {
                Features[INISectionNum].Code = Convert.ToString(INIProperty.Value);
            }
            else if ( (string)INIProperty.Name == "position" )
            {
                clsWorldPos temp_Result = Features[INISectionNum].Pos;
                if ( !IOUtil.WorldPosFromINIText(Convert.ToString(INIProperty.Value), ref temp_Result) )
                {
                    return TranslatorResult.ValueInvalid;
                }
            }
            else if ( (string)INIProperty.Name == "rotation" )
            {
                sWZAngle temp_Result2 = Features[INISectionNum].Rotation;
                if ( !IOUtil.WZAngleFromINIText(Convert.ToString(INIProperty.Value), ref temp_Result2) )
                {
                    return TranslatorResult.ValueInvalid;
                }
            }
            else if ( (string)INIProperty.Name == "health" )
            {
                Int32 temp_Result3 = Features[INISectionNum].HealthPercent;
                if ( !IOUtil.HealthFromINIText(Convert.ToString(INIProperty.Value), ref temp_Result3) )
                {
                    return TranslatorResult.ValueInvalid;
                }
            }
            else
            {
                return TranslatorResult.NameUnknown;
            }
            return TranslatorResult.Translated;
        }
    }
}