#region

using System;
using SharpFlame.Core.Domain;
using SharpFlame.FileIO;
using SharpFlame.FileIO.Ini;

#endregion

namespace SharpFlame.Mapping.Format.FMap
{
    public class FMapIniGateways : SectionTranslator
    {
        public int GatewayCount;
        public sGateway[] Gateways;

        public FMapIniGateways(int NewGatewayCount)
        {
            var A = 0;

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
            if ( INIProperty.Name == "ax" )
            {
                var temp_Result = Gateways[INISectionNum].PosA.X;
                if ( !IOUtil.InvariantParse(Convert.ToString(INIProperty.Value), ref temp_Result) )
                {
                    return TranslatorResult.ValueInvalid;
                }
            }
            else if ( INIProperty.Name == "ay" )
            {
                var temp_Result2 = Gateways[INISectionNum].PosA.Y;
                if ( !IOUtil.InvariantParse(Convert.ToString(INIProperty.Value), ref temp_Result2) )
                {
                    return TranslatorResult.ValueInvalid;
                }
            }
            else if ( INIProperty.Name == "bx" )
            {
                var temp_Result3 = Gateways[INISectionNum].PosB.X;
                if ( !IOUtil.InvariantParse(Convert.ToString(INIProperty.Value), ref temp_Result3) )
                {
                    return TranslatorResult.ValueInvalid;
                }
            }
            else if ( INIProperty.Name == "by" )
            {
                var temp_Result4 = Gateways[INISectionNum].PosB.Y;
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

        public struct sGateway
        {
            public XYInt PosA;
            public XYInt PosB;
        }
    }
}