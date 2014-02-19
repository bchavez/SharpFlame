using System;
using System.Diagnostics;
using SharpFlame.Collections;
using SharpFlame.Core.Domain;
using SharpFlame.Core.Parsers.Ini;
using SharpFlame.Domain;
using SharpFlame.FileIO;
using SharpFlame.FileIO.Ini;
using SharpFlame.Maths;
using SharpFlame.Util;

namespace SharpFlame.Mapping.Objects
{
    public class clsUnit : IEquatable<clsUnit>
    {
        public ConnectedListLink<clsUnit, clsMap> MapLink;
        public ConnectedListLink<clsUnit, clsMap> MapSelectedUnitLink;
        public ConnectedList<clsUnitSectorConnection, clsUnit> Sectors;

        public UInt32 ID;
        public UnitTypeBase TypeBase;
        public WorldPos Pos;
        public int Rotation;
        public clsUnitGroup UnitGroup;
        public int SavePriority;
        public double Health = 1.0D;
        public bool PreferPartsOutput = false;

        private string label;
        public string Label {
            get { return label; }
        }

        public clsUnit()
        {
            MapLink = new ConnectedListLink<clsUnit, clsMap>(this);
            MapSelectedUnitLink = new ConnectedListLink<clsUnit, clsMap>(this);
            Sectors = new ConnectedList<clsUnitSectorConnection, clsUnit>(this);
        }

        public clsUnit(clsUnit unitToCopy, clsMap targetMap)
        {
            MapLink = new ConnectedListLink<clsUnit, clsMap>(this);
            MapSelectedUnitLink = new ConnectedListLink<clsUnit, clsMap>(this);
            Sectors = new ConnectedList<clsUnitSectorConnection, clsUnit>(this);

            bool IsDesign = default(bool);

            if ( unitToCopy.TypeBase.Type == UnitType.PlayerDroid )
            {
                IsDesign = !((DroidDesign)unitToCopy.TypeBase).IsTemplate;
            }
            else
            {
                IsDesign = false;
            }
            if ( IsDesign )
            {
                DroidDesign DroidDesign = new DroidDesign();
                TypeBase = DroidDesign;
                DroidDesign.CopyDesign((DroidDesign)unitToCopy.TypeBase);
                DroidDesign.UpdateAttachments();
            }
            else
            {
                TypeBase = unitToCopy.TypeBase;
            }
            Pos = unitToCopy.Pos;
            Rotation = unitToCopy.Rotation;
            clsUnitGroup otherUnitGroup = default(clsUnitGroup);
            otherUnitGroup = unitToCopy.UnitGroup;
            if ( otherUnitGroup.WZ_StartPos < 0 )
            {
                UnitGroup = targetMap.ScavengerUnitGroup;
            }
            else
            {
                UnitGroup = targetMap.UnitGroups[otherUnitGroup.WZ_StartPos];
            }
            SavePriority = unitToCopy.SavePriority;
            Health = unitToCopy.Health;
            PreferPartsOutput = unitToCopy.PreferPartsOutput;
        }

        public string GetINIPosition()
        {
            return Pos.Horizontal.X.ToStringInvariant() + ", " + Pos.Horizontal.Y.ToStringInvariant() + ", 0";
        }

        public string GetINIRotation()
        {
            int rotation16 = 0;

            rotation16 = (int)(Rotation * Constants.INIRotationMax / 360.0D);
            if ( rotation16 >= Constants.INIRotationMax )
            {
                rotation16 -= Constants.INIRotationMax;
            }
            else if ( rotation16 < 0 )
            {
                Debugger.Break();
                rotation16 += Constants.INIRotationMax;
            }

            return string.Format("{0}, 0, 0", rotation16);
        }

        public string GetINIHealthPercent()
        {
            return string.Format ("{0}%", (int)(MathUtil.Clamp_dbl (Health * 100.0D, 1.0D, 100.0D)));
        }

        public string GetPosText()
        {
            return Pos.Horizontal.X.ToStringInvariant() + ", " + Pos.Horizontal.Y.ToStringInvariant();
        }

        public sResult SetLabel(string Text)
        {
            sResult Result = new sResult();

            if ( TypeBase.Type == UnitType.PlayerStructure )
            {
                StructureTypeBase structureTypeBase = (StructureTypeBase)TypeBase;
                StructureTypeBase.enumStructureType StructureTypeType = structureTypeBase.StructureType;
                if ( StructureTypeType == StructureTypeBase.enumStructureType.FactoryModule
                     | StructureTypeType == StructureTypeBase.enumStructureType.PowerModule
                     | StructureTypeType == StructureTypeBase.enumStructureType.ResearchModule )
                {
                    Result.Problem = "Error: Trying to assign label to structure module.";
                    return Result;
                }
            }

            if ( !MapLink.IsConnected )
            {
                Debugger.Break();
                Result.Problem = "Error: Unit not on a map.";
                return Result;
            }

            if ( Text == null )
            {
                label = null;
                Result.Success = true;
                Result.Problem = "";
                return Result;
            }
            else
            {
                Result = MapLink.Source.ScriptLabelIsValid(Text);
                if ( Result.Success )
                {
                    label = Text;
                }
                return Result;
            }
        }

        public void WriteWZLabel(IniWriter File, int PlayerCount)
        {
            if ( label != null )
            {
                int TypeNum = 0;
                switch ( TypeBase.Type )
                {
                    case UnitType.PlayerDroid:
                        TypeNum = 0;
                        break;
                    case UnitType.PlayerStructure:
                        TypeNum = 1;
                        break;
                    case UnitType.Feature:
                        TypeNum = 2;
                        break;
                    default:
                        return;
                }
                File.AddSection("object_" + MapLink.ArrayPosition.ToStringInvariant());
                File.AddProperty("id", ID.ToStringInvariant());
                if ( PlayerCount >= 0 ) //not an FMap
                {
                    File.AddProperty("type", TypeNum.ToStringInvariant());
                    File.AddProperty("player", UnitGroup.GetPlayerNum(PlayerCount).ToStringInvariant());
                }
                File.AddProperty("label", label);
            }
        }

        public UInt32 GetBJOMultiplayerPlayerNum(int PlayerCount)
        {
            int PlayerNum = 0;

            if ( UnitGroup == MapLink.Source.ScavengerUnitGroup || UnitGroup.WZ_StartPos < 0 )
            {
                PlayerNum = Math.Max(PlayerCount, 7);
            }
            else
            {
                PlayerNum = UnitGroup.WZ_StartPos;
            }
            return (uint)PlayerNum;
        }

        public UInt32 GetBJOCampaignPlayerNum()
        {
            int PlayerNum = 0;

            if ( UnitGroup == MapLink.Source.ScavengerUnitGroup || UnitGroup.WZ_StartPos < 0 )
            {
                PlayerNum = 7;
            }
            else
            {
                PlayerNum = UnitGroup.WZ_StartPos;
            }
            return (uint)PlayerNum;
        }

        public void MapSelect()
        {
            if ( MapSelectedUnitLink.IsConnected )
            {
                Debugger.Break();
                return;
            }

            MapSelectedUnitLink.Connect(MapLink.Source.SelectedUnits);
        }

        public void MapDeselect()
        {
            if ( !MapSelectedUnitLink.IsConnected )
            {
                Debugger.Break();
                return;
            }

            MapSelectedUnitLink.Disconnect();
        }

        public void DisconnectFromMap()
        {
            if ( MapLink.IsConnected )
            {
                MapLink.Disconnect();
            }
            if ( MapSelectedUnitLink.IsConnected )
            {
                MapSelectedUnitLink.Disconnect();
            }
            Sectors.Clear();
        }

        public void Deallocate()
        {
            MapLink.Deallocate();
            MapSelectedUnitLink.Deallocate();
            Sectors.Deallocate();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            clsUnit objAsUnit = obj as clsUnit;
            if (objAsUnit == null)
                return false;

            return Equals (objAsUnit);
        }

        public override int GetHashCode()
        {
            return (int)ID;
        }

        public bool Equals(clsUnit other)
        {
            if (other == null)
                return false;
            return (this.ID.Equals (other.ID));
        }
    }
}