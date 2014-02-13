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
                if ( !IOUtil.InvariantParse(Convert.ToString(INIProperty.Value), ref Features[INISectionNum].ID) )
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
                if ( !IOUtil.WorldPosFromINIText(Convert.ToString(INIProperty.Value), ref Features[INISectionNum].Pos) )
                {
                    return TranslatorResult.ValueInvalid;
                }
            }
            else if ( (string)INIProperty.Name == "rotation" )
            {
                if ( !IOUtil.WZAngleFromINIText(Convert.ToString(INIProperty.Value), ref Features[INISectionNum].Rotation) )
                {
                    return TranslatorResult.ValueInvalid;
                }
            }
            else if ( (string)INIProperty.Name == "health" )
            {
                if ( !IOUtil.HealthFromINIText(Convert.ToString(INIProperty.Value), ref Features[INISectionNum].HealthPercent) )
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