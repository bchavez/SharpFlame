using System;
using SharpFlame.FileIO;
using SharpFlame.FileIO.Ini;
using SharpFlame.Mapping.Objects;
using SharpFlame.Util;

namespace SharpFlame.Mapping.Wz
{
    public class IniStructures : SectionTranslator
    {
        private clsMap ParentMap;

        public struct sStructure
        {
            public UInt32 ID;
            public string Code;
            public clsUnitGroup UnitGroup;
            public clsWorldPos Pos;
            public sWZAngle Rotation;
            public int ModuleCount;
            public int HealthPercent;
            public int WallType;
        }

        public sStructure[] Structures;
        public int StructureCount;

        public IniStructures(int newStructureCount, clsMap newParentMap)
        {
            int A = 0;

            ParentMap = newParentMap;

            StructureCount = newStructureCount;
            Structures = new sStructure[StructureCount];
            for ( A = 0; A <= StructureCount - 1; A++ )
            {
                Structures[A].HealthPercent = -1;
                Structures[A].WallType = -1;
            }
        }

        public override TranslatorResult Translate(int iNISectionNum, Section.SectionProperty iNIProperty)
        {
            if ( (string)iNIProperty.Name == "id" )
            {
                if ( !IOUtil.InvariantParse(iNIProperty.Value, ref Structures[iNISectionNum].ID) )
                {
                    return TranslatorResult.ValueInvalid;
                }
            }
            else if ( (string)iNIProperty.Name == "name" )
            {
                Structures[iNISectionNum].Code = Convert.ToString(iNIProperty.Value);
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
                Structures[iNISectionNum].UnitGroup = ParentMap.UnitGroups[StartPos];
            }
            else if ( (string)iNIProperty.Name == "player" )
            {
                if ( iNIProperty.Value.ToLower() != "scavenger" )
                {
                    return TranslatorResult.ValueInvalid;
                }
                Structures[iNISectionNum].UnitGroup = ParentMap.ScavengerUnitGroup;
            }
            else if ( (string)iNIProperty.Name == "position" )
            {
                if ( !IOUtil.WorldPosFromINIText(Convert.ToString(iNIProperty.Value), ref Structures [iNISectionNum].Pos) )
                {
                    return TranslatorResult.ValueInvalid;
                }
            }
            else if ( (string)iNIProperty.Name == "rotation" )
            {
                if ( !IOUtil.WZAngleFromINIText(Convert.ToString(iNIProperty.Value), ref Structures[iNISectionNum].Rotation) )
                {
                    return TranslatorResult.ValueInvalid;
                }
            }
            else if ( (string)iNIProperty.Name == "modules" )
            {
                int ModuleCount = 0;
                if ( !IOUtil.InvariantParse(Convert.ToString(iNIProperty.Value), ref ModuleCount) )
                {
                    return TranslatorResult.ValueInvalid;
                }
                if ( ModuleCount < 0 )
                {
                    return TranslatorResult.ValueInvalid;
                }
                Structures[iNISectionNum].ModuleCount = ModuleCount;
            }
            else if ( (string)iNIProperty.Name == "health" )
            {
                if ( !IOUtil.HealthFromINIText(Convert.ToString(iNIProperty.Value), ref Structures[iNISectionNum].HealthPercent) )
                {
                    return TranslatorResult.ValueInvalid;
                }
            }
            else if ( (string)iNIProperty.Name == "wall/type" )
            {
                int WallType = 0;
                if ( !IOUtil.InvariantParse(Convert.ToString(iNIProperty.Value), ref WallType) )
                {
                    return TranslatorResult.ValueInvalid;
                }
                if ( WallType < 0 )
                {
                    return TranslatorResult.ValueInvalid;
                }
                Structures[iNISectionNum].WallType = WallType;
            }
            else
            {
                return TranslatorResult.NameUnknown;
            }

            return TranslatorResult.Translated;
        }
    }
}