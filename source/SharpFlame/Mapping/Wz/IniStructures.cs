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

        public IniStructures(int NewStructureCount, clsMap NewParentMap)
        {
            int A = 0;

            ParentMap = NewParentMap;

            StructureCount = NewStructureCount;
            Structures = new sStructure[StructureCount];
            for ( A = 0; A <= StructureCount - 1; A++ )
            {
                Structures[A].HealthPercent = -1;
                Structures[A].WallType = -1;
            }
        }

        public override TranslatorResult Translate(int INISectionNum, Section.SectionProperty INIProperty)
        {
            if ( (string)INIProperty.Name == "id" )
            {
                if ( !IOUtil.InvariantParse(INIProperty.Value, ref Structures[INISectionNum].ID) )
                {
                    return TranslatorResult.ValueInvalid;
                }
            }
            else if ( (string)INIProperty.Name == "name" )
            {
                Structures[INISectionNum].Code = Convert.ToString(INIProperty.Value);
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
                Structures[INISectionNum].UnitGroup = ParentMap.UnitGroups[StartPos];
            }
            else if ( (string)INIProperty.Name == "player" )
            {
                if ( INIProperty.Value.ToLower() != "scavenger" )
                {
                    return TranslatorResult.ValueInvalid;
                }
                Structures[INISectionNum].UnitGroup = ParentMap.ScavengerUnitGroup;
            }
            else if ( (string)INIProperty.Name == "position" )
            {
                if ( !IOUtil.WorldPosFromINIText(Convert.ToString(INIProperty.Value), ref Structures [INISectionNum].Pos) )
                {
                    return TranslatorResult.ValueInvalid;
                }
            }
            else if ( (string)INIProperty.Name == "rotation" )
            {
                if ( !IOUtil.WZAngleFromINIText(Convert.ToString(INIProperty.Value), ref Structures[INISectionNum].Rotation) )
                {
                    return TranslatorResult.ValueInvalid;
                }
            }
            else if ( (string)INIProperty.Name == "modules" )
            {
                int ModuleCount = 0;
                if ( !IOUtil.InvariantParse(Convert.ToString(INIProperty.Value), ref ModuleCount) )
                {
                    return TranslatorResult.ValueInvalid;
                }
                if ( ModuleCount < 0 )
                {
                    return TranslatorResult.ValueInvalid;
                }
                Structures[INISectionNum].ModuleCount = ModuleCount;
            }
            else if ( (string)INIProperty.Name == "health" )
            {
                if ( !IOUtil.HealthFromINIText(Convert.ToString(INIProperty.Value), ref Structures[INISectionNum].HealthPercent) )
                {
                    return TranslatorResult.ValueInvalid;
                }
            }
            else if ( (string)INIProperty.Name == "wall/type" )
            {
                int WallType = 0;
                if ( !IOUtil.InvariantParse(Convert.ToString(INIProperty.Value), ref WallType) )
                {
                    return TranslatorResult.ValueInvalid;
                }
                if ( WallType < 0 )
                {
                    return TranslatorResult.ValueInvalid;
                }
                Structures[INISectionNum].WallType = WallType;
            }
            else
            {
                return TranslatorResult.NameUnknown;
            }

            return TranslatorResult.Translated;
        }
    }
}