#region

using System;
using SharpFlame.Core.Domain;
using SharpFlame.Domain;
using SharpFlame.FileIO;
using SharpFlame.FileIO.Ini;

#endregion

namespace SharpFlame.Mapping.FMap
{
    public class FMapIniObjects : SectionTranslator
    {
        public int ObjectCount;
        public sObject[] Objects;

        public FMapIniObjects(int NewObjectCount)
        {
            var A = 0;
            var B = 0;

            ObjectCount = NewObjectCount;
            Objects = new sObject[ObjectCount];
            for ( A = 0; A <= ObjectCount - 1; A++ )
            {
                Objects[A].Type = UnitType.Unspecified;
                Objects[A].Health = 1.0D;
                Objects[A].WallType = -1;
                Objects[A].TurretCodes = new string[3];
                Objects[A].TurretTypes = new enumTurretType[3];
                for ( B = 0; B <= Constants.MaxDroidWeapons - 1; B++ )
                {
                    Objects[A].TurretTypes[B] = enumTurretType.Unknown;
                }
            }
        }

        public override TranslatorResult Translate(int INISectionNum, Section.SectionProperty INIProperty)
        {
            if ( INIProperty.Name == "type" )
            {
                string[] CommaText = null;
                var CommaTextCount = 0;
                var A = 0;
                CommaText = INIProperty.Value.Split(',');
                CommaTextCount = CommaText.GetUpperBound(0) + 1;
                if ( CommaTextCount < 1 )
                {
                    return TranslatorResult.ValueInvalid;
                }
                for ( A = 0; A <= CommaTextCount - 1; A++ )
                {
                    CommaText[A] = Convert.ToString(CommaText[A].Trim());
                }
                switch ( CommaText[0].ToLower() )
                {
                    case "feature":
                        Objects[INISectionNum].Type = UnitType.Feature;
                        Objects[INISectionNum].Code = CommaText[1];
                        break;
                    case "structure":
                        Objects[INISectionNum].Type = UnitType.PlayerStructure;
                        Objects[INISectionNum].Code = CommaText[1];
                        break;
                    case "droidtemplate":
                        Objects[INISectionNum].Type = UnitType.PlayerDroid;
                        Objects[INISectionNum].IsTemplate = true;
                        Objects[INISectionNum].Code = CommaText[1];
                        break;
                    case "droiddesign":
                        Objects[INISectionNum].Type = UnitType.PlayerDroid;
                        break;
                    default:
                        return TranslatorResult.ValueInvalid;
                }
            }
            else if ( INIProperty.Name == "droidtype" )
            {
                var DroidType = App.GetTemplateDroidTypeFromTemplateCode(Convert.ToString(INIProperty.Value));
                if ( DroidType == null )
                {
                    return TranslatorResult.ValueInvalid;
                }
                Objects[INISectionNum].TemplateDroidType = DroidType;
            }
            else if ( INIProperty.Name == "body" )
            {
                Objects[INISectionNum].BodyCode = Convert.ToString(INIProperty.Value);
            }
            else if ( INIProperty.Name == "propulsion" )
            {
                Objects[INISectionNum].PropulsionCode = Convert.ToString(INIProperty.Value);
            }
            else if ( INIProperty.Name == "turretcount" )
            {
                var NewTurretCount = 0;
                if ( !IOUtil.InvariantParse(Convert.ToString(INIProperty.Value), ref NewTurretCount) )
                {
                    return TranslatorResult.ValueInvalid;
                }
                if ( NewTurretCount < 0 | NewTurretCount > Constants.MaxDroidWeapons )
                {
                    return TranslatorResult.ValueInvalid;
                }
                Objects[INISectionNum].TurretCount = NewTurretCount;
            }
            else if ( INIProperty.Name == "turret1" )
            {
                string[] CommaText = null;
                var CommaTextCount = 0;
                var A = 0;
                CommaText = INIProperty.Value.Split(',');
                CommaTextCount = CommaText.GetUpperBound(0) + 1;
                if ( CommaTextCount < 2 )
                {
                    return TranslatorResult.ValueInvalid;
                }
                for ( A = 0; A <= CommaTextCount - 1; A++ )
                {
                    CommaText[A] = Convert.ToString(CommaText[A].Trim());
                }
                var TurretType = default(enumTurretType);
                TurretType = App.GetTurretTypeFromName(CommaText[0]);
                if ( TurretType != enumTurretType.Unknown )
                {
                    Objects[INISectionNum].TurretTypes[0] = TurretType;
                    Objects[INISectionNum].TurretCodes[0] = CommaText[1];
                }
            }
            else if ( INIProperty.Name == "turret2" )
            {
                string[] CommaText = null;
                var CommaTextCount = 0;
                var A = 0;
                CommaText = INIProperty.Value.Split(',');
                CommaTextCount = CommaText.GetUpperBound(0) + 1;
                if ( CommaTextCount < 2 )
                {
                    return TranslatorResult.ValueInvalid;
                }
                for ( A = 0; A <= CommaTextCount - 1; A++ )
                {
                    CommaText[A] = Convert.ToString(CommaText[A].Trim());
                }
                var TurretType = default(enumTurretType);
                TurretType = App.GetTurretTypeFromName(CommaText[0]);
                if ( TurretType != enumTurretType.Unknown )
                {
                    Objects[INISectionNum].TurretTypes[1] = TurretType;
                    Objects[INISectionNum].TurretCodes[1] = CommaText[1];
                }
            }
            else if ( INIProperty.Name == "turret3" )
            {
                string[] CommaText = null;
                var CommaTextCount = 0;
                var A = 0;
                CommaText = INIProperty.Value.Split(',');
                CommaTextCount = CommaText.GetUpperBound(0) + 1;
                if ( CommaTextCount < 2 )
                {
                    return TranslatorResult.ValueInvalid;
                }
                for ( A = 0; A <= CommaTextCount - 1; A++ )
                {
                    CommaText[A] = Convert.ToString(CommaText[A].Trim());
                }
                var TurretType = default(enumTurretType);
                TurretType = App.GetTurretTypeFromName(CommaText[0]);
                if ( TurretType != enumTurretType.Unknown )
                {
                    Objects[INISectionNum].TurretTypes[2] = TurretType;
                    Objects[INISectionNum].TurretCodes[2] = CommaText[1];
                }
            }
            else if ( INIProperty.Name == "id" )
            {
                if ( !IOUtil.InvariantParse(Convert.ToString(INIProperty.Value), ref Objects[INISectionNum].ID) )
                {
                    return TranslatorResult.ValueInvalid;
                }
            }
            else if ( INIProperty.Name == "priority" )
            {
                var temp_Result = Objects[INISectionNum].Priority;
                if ( !IOUtil.InvariantParse(Convert.ToString(INIProperty.Value), ref temp_Result) )
                {
                    return TranslatorResult.ValueInvalid;
                }
            }
            else if ( INIProperty.Name == "pos" )
            {
                string[] CommaText = null;
                var CommaTextCount = 0;
                var A = 0;
                CommaText = INIProperty.Value.Split(',');
                CommaTextCount = CommaText.GetUpperBound(0) + 1;
                if ( CommaTextCount < 2 )
                {
                    return TranslatorResult.ValueInvalid;
                }
                for ( A = 0; A <= CommaTextCount - 1; A++ )
                {
                    CommaText[A] = Convert.ToString(CommaText[A].Trim());
                }
                var Pos = new XYInt();
                if ( !IOUtil.InvariantParse(CommaText[0], ref Pos.X) )
                {
                    return TranslatorResult.ValueInvalid;
                }
                if ( !IOUtil.InvariantParse(CommaText[1], ref Pos.Y) )
                {
                    return TranslatorResult.ValueInvalid;
                }
                try
                {
                    Objects[INISectionNum].Pos = Pos;
                }
                catch ( Exception )
                {
                    return TranslatorResult.ValueInvalid;
                }
            }
            else if ( INIProperty.Name == "heading" )
            {
                double dblTemp = 0;
                if ( !IOUtil.InvariantParse(Convert.ToString(INIProperty.Value), ref dblTemp) )
                {
                    return TranslatorResult.ValueInvalid;
                }
                if ( dblTemp < 0.0D | dblTemp >= 360.0D )
                {
                    return TranslatorResult.ValueInvalid;
                }
                Objects[INISectionNum].Heading = dblTemp;
            }
            else if ( INIProperty.Name == "unitgroup" )
            {
                Objects[INISectionNum].UnitGroup = Convert.ToString(INIProperty.Value);
            }
            else if ( INIProperty.Name == "health" )
            {
                double NewHealth = 0;
                if ( !IOUtil.InvariantParse(Convert.ToString(INIProperty.Value), ref NewHealth) )
                {
                    return TranslatorResult.ValueInvalid;
                }
                if ( NewHealth < 0.0D | NewHealth >= 1.0D )
                {
                    return TranslatorResult.ValueInvalid;
                }
                Objects[INISectionNum].Health = NewHealth;
            }
            else if ( INIProperty.Name == "walltype" )
            {
                var WallType = -1;
                if ( !IOUtil.InvariantParse(Convert.ToString(INIProperty.Value), ref WallType) )
                {
                    return TranslatorResult.ValueInvalid;
                }
                if ( WallType >= 0 & WallType <= 3 )
                {
                    Objects[INISectionNum].WallType = WallType;
                }
            }
            else if ( INIProperty.Name == "scriptlabel" )
            {
                Objects[INISectionNum].Label = Convert.ToString(INIProperty.Value);
            }
            else
            {
                return TranslatorResult.NameUnknown;
            }
            return TranslatorResult.Translated;
        }

        public struct sObject
        {
            public string BodyCode;
            public string Code;
            public bool GotAltitude;
            public double Heading;
            public double Health;
            public UInt32 ID;
            public bool IsTemplate;
            public string Label;
            public XYInt Pos;
            public int Priority;
            public string PropulsionCode;
            public DroidDesign.clsTemplateDroidType TemplateDroidType;
            public string[] TurretCodes;
            public int TurretCount;
            public enumTurretType[] TurretTypes;
            public UnitType Type;
            public string UnitGroup;
            public int WallType;
        }
    }
}