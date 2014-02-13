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

        public IniFeatures(int newFeatureCount)
        {
            int A = 0;

            FeatureCount = newFeatureCount;
            Features = new sFeatures[FeatureCount];
            for ( A = 0; A <= FeatureCount - 1; A++ )
            {
                Features[A].HealthPercent = -1;
            }
        }

        public override TranslatorResult Translate(int iNISectionNum, Section.SectionProperty iNIProperty)
        {
            if ( (string)iNIProperty.Name == "id" )
            {
                if ( !IOUtil.InvariantParse(Convert.ToString(iNIProperty.Value), ref Features[iNISectionNum].ID) )
                {
                    return TranslatorResult.ValueInvalid;
                }
            }
            else if ( (string)iNIProperty.Name == "name" )
            {
                Features[iNISectionNum].Code = Convert.ToString(iNIProperty.Value);
            }
            else if ( (string)iNIProperty.Name == "position" )
            {
                if ( !IOUtil.WorldPosFromINIText(Convert.ToString(iNIProperty.Value), ref Features[iNISectionNum].Pos) )
                {
                    return TranslatorResult.ValueInvalid;
                }
            }
            else if ( (string)iNIProperty.Name == "rotation" )
            {
                if ( !IOUtil.WZAngleFromINIText(Convert.ToString(iNIProperty.Value), ref Features[iNISectionNum].Rotation) )
                {
                    return TranslatorResult.ValueInvalid;
                }
            }
            else if ( (string)iNIProperty.Name == "health" )
            {
                if ( !IOUtil.HealthFromINIText(Convert.ToString(iNIProperty.Value), ref Features[iNISectionNum].HealthPercent) )
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