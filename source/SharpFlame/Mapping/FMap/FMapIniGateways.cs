using System;
using SharpFlame.FileIO;
using SharpFlame.FileIO.Ini;
using SharpFlame.Maths;

namespace SharpFlame.Mapping.FMap
{
    public class FMapIniGateways : SectionTranslator
    {
        public struct sGateway
        {
            public sXY_int PosA;
            public sXY_int PosB;
        }

        public sGateway[] Gateways;
        public int GatewayCount;

        public FMapIniGateways(int NewGatewayCount)
        {
            int A = 0;

            GatewayCount = NewGatewayCount;
            Gateways = new sGateway[GatewayCount];
            for ( A = 0; A <= GatewayCount - 1; A++ )
            {
                Gateways[A].PosA.X = -1;
                Gateways[A].PosA.Y = -1;
                Gateways[A].PosB.X = -1;
                Gateways[A].PosB.Y = -1;
            }
        }

        public override TranslatorResult Translate(int INISectionNum, Section.SectionProperty INIProperty)
        {
            if ( (string)INIProperty.Name == "ax" )
            {
                int temp_Result = Gateways[INISectionNum].PosA.X;
                if ( !IOUtil.InvariantParse(Convert.ToString(INIProperty.Value), ref temp_Result) )
                {
                    return TranslatorResult.ValueInvalid;
                }
            }
            else if ( (string)INIProperty.Name == "ay" )
            {
                Int32 temp_Result2 = Gateways[INISectionNum].PosA.Y;
                if ( !IOUtil.InvariantParse(Convert.ToString(INIProperty.Value), ref temp_Result2) )
                {
                    return TranslatorResult.ValueInvalid;
                }
            }
            else if ( (string)INIProperty.Name == "bx" )
            {
                Int32 temp_Result3 = Gateways[INISectionNum].PosB.X;
                if ( !IOUtil.InvariantParse(Convert.ToString(INIProperty.Value), ref temp_Result3) )
                {
                    return TranslatorResult.ValueInvalid;
                }
            }
            else if ( (string)INIProperty.Name == "by" )
            {
                int temp_Result4 = Gateways[INISectionNum].PosB.Y;
                if ( !IOUtil.InvariantParse(Convert.ToString(INIProperty.Value), ref temp_Result4) )
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