#region

using System;
using System.Diagnostics;
using SharpFlame.Collections;
using SharpFlame.Core.Extensions;
using SharpFlame.FileIO;
using SharpFlame.Core;
using SharpFlame.Core.Collections;
using SharpFlame.Core.Domain;
using SharpFlame.Core.Parsers.Ini;
using SharpFlame.Domain;
using SharpFlame.Maths;
using SharpFlame.Util;

#endregion

namespace SharpFlame.Mapping.Objects
{
    public class Unit : IEquatable<Unit>
    {
        public double Health = 1.0D;
        public UInt32 ID;
        public ConnectedListLink<Unit, Map> MapLink;
        public ConnectedListLink<Unit, Map> MapSelectedUnitLink;
        public WorldPos Pos;
        public bool PreferPartsOutput = false;
        public int Rotation;
        public int SavePriority;
        public ConnectedList<clsUnitSectorConnection, Unit> Sectors;
        public UnitTypeBase TypeBase;
        public clsUnitGroup UnitGroup;

        private string label;

        public Unit()
        {
            MapLink = new ConnectedListLink<Unit, Map>(this);
            MapSelectedUnitLink = new ConnectedListLink<Unit, Map>(this);
            Sectors = new ConnectedList<clsUnitSectorConnection, Unit>(this);
        }

        public Unit(Unit unitToCopy, Map targetMap)
        {
            MapLink = new ConnectedListLink<Unit, Map>(this);
            MapSelectedUnitLink = new ConnectedListLink<Unit, Map>(this);
            Sectors = new ConnectedList<clsUnitSectorConnection, Unit>(this);

            var IsDesign = default(bool);

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
                var DroidDesign = new DroidDesign();
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
            var otherUnitGroup = default(clsUnitGroup);
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

        public string Label
        {
            get { return label; }
        }

        public bool Equals(Unit other)
        {
            if ( other == null )
                return false;
            return (ID.Equals(other.ID));
        }

        public string GetINIPosition()
        {
            return Pos.Horizontal.X.ToStringInvariant() + ", " + Pos.Horizontal.Y.ToStringInvariant() + ", 0";
        }

        public string GetINIRotation()
        {
            var rotation16 = 0;

            rotation16 = (Rotation * Constants.IniRotationMax / 360.0D).ToInt();
            if ( rotation16 >= Constants.IniRotationMax )
            {
                rotation16 -= Constants.IniRotationMax;
            }
            else if ( rotation16 < 0 )
            {
                Debugger.Break();
                rotation16 += Constants.IniRotationMax;
            }

            return string.Format("{0}, 0, 0", rotation16);
        }

        public string GetINIHealthPercent()
        {
            return string.Format("{0}%", MathUtil.ClampDbl(Health * 100.0D, 1.0D, 100.0D).ToInt());
        }

        public string GetPosText()
        {
            return Pos.Horizontal.X.ToStringInvariant() + ", " + Pos.Horizontal.Y.ToStringInvariant();
        }

        public SimpleResult SetLabel(string Text)
        {
            var Result = new SimpleResult();

            if ( TypeBase.Type == UnitType.PlayerStructure )
            {
                var structureTypeBase = (StructureTypeBase)TypeBase;
                var StructureTypeType = structureTypeBase.StructureType;
                if ( StructureTypeType == StructureType.FactoryModule
                     | StructureTypeType == StructureType.PowerModule
                     | StructureTypeType == StructureType.ResearchModule )
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
            Result = MapLink.Source.ScriptLabelIsValid(Text);
            if ( Result.Success )
            {
                label = Text;
            }
            return Result;
        }

        public void WriteWZLabel(IniWriter File, int PlayerCount)
        {
            if ( label != null )
            {
                var TypeNum = 0;
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
            var PlayerNum = 0;

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
            var PlayerNum = 0;

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
            if ( obj == null )
                return false;

            var objAsUnit = obj as Unit;
            if ( objAsUnit == null )
                return false;

            return Equals(objAsUnit);
        }

        public override int GetHashCode()
        {
            return (int)ID;
        }
    }
}