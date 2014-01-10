using System;
using System.Diagnostics;
using SharpFlame.Collections;
using SharpFlame.Domain;
using SharpFlame.FileIO;
using SharpFlame.FileIO.Ini;
using SharpFlame.Maths;

namespace SharpFlame.Mapping.Objects
{
    public class clsUnit
    {
        public clsUnit()
        {
            MapLink = new ConnectedListLink<clsUnit, clsMap>(this);
            MapSelectedUnitLink = new ConnectedListLink<clsUnit, clsMap>(this);
            Sectors = new ConnectedList<clsUnitSectorConnection, clsUnit>(this);
        }

        public ConnectedListLink<clsUnit, clsMap> MapLink;
        public ConnectedListLink<clsUnit, clsMap> MapSelectedUnitLink;
        public ConnectedList<clsUnitSectorConnection, clsUnit> Sectors;

        public UInt32 ID;
        public UnitTypeBase TypeBase;
        public App.sWorldPos Pos;
        public int Rotation;
        public clsUnitGroup UnitGroup;
        public int SavePriority;
        public double Health = 1.0D;
        public bool PreferPartsOutput = false;

        private string _Label;

        //public clsUnit()
        //{

        //MapLink = new ConnectedListLink<clsUnit, clsMap>(this);
        //MapSelectedUnitLink = new ConnectedListLink<clsUnit, clsMap>(this);
        //Sectors = new ConnectedList<clsUnitSectorConnection, clsUnit>(this);


        //}

        public clsUnit(clsUnit UnitToCopy, clsMap TargetMap)
        {
            MapLink = new ConnectedListLink<clsUnit, clsMap>(this);
            MapSelectedUnitLink = new ConnectedListLink<clsUnit, clsMap>(this);
            Sectors = new ConnectedList<clsUnitSectorConnection, clsUnit>(this);

            bool IsDesign = default(bool);

            if ( UnitToCopy.TypeBase.Type == UnitType.PlayerDroid )
            {
                IsDesign = !((DroidDesign)UnitToCopy.TypeBase).IsTemplate;
            }
            else
            {
                IsDesign = false;
            }
            if ( IsDesign )
            {
                DroidDesign DroidDesign = new DroidDesign();
                TypeBase = DroidDesign;
                DroidDesign.CopyDesign((DroidDesign)UnitToCopy.TypeBase);
                DroidDesign.UpdateAttachments();
            }
            else
            {
                TypeBase = UnitToCopy.TypeBase;
            }
            Pos = UnitToCopy.Pos;
            Rotation = UnitToCopy.Rotation;
            clsUnitGroup OtherUnitGroup = default(clsUnitGroup);
            OtherUnitGroup = UnitToCopy.UnitGroup;
            if ( OtherUnitGroup.WZ_StartPos < 0 )
            {
                UnitGroup = TargetMap.ScavengerUnitGroup;
            }
            else
            {
                UnitGroup = TargetMap.UnitGroups[OtherUnitGroup.WZ_StartPos];
            }
            SavePriority = UnitToCopy.SavePriority;
            Health = UnitToCopy.Health;
            PreferPartsOutput = UnitToCopy.PreferPartsOutput;
        }

        public string Label
        {
            get { return _Label; }
        }

        public string GetINIPosition()
        {
            return Pos.Horizontal.X.ToStringInvariant() + ", " + Pos.Horizontal.Y.ToStringInvariant() + ", 0";
        }

        public string GetINIRotation()
        {
            int Rotation16 = 0;

            Rotation16 = (int)(Rotation * App.INIRotationMax / 360.0D);
            if ( Rotation16 >= App.INIRotationMax )
            {
                Rotation16 -= App.INIRotationMax;
            }
            else if ( Rotation16 < 0 )
            {
                Debugger.Break();
                Rotation16 += App.INIRotationMax;
            }

            return Rotation16.ToStringInvariant() + ", 0, 0";
        }

        public string GetINIHealthPercent()
        {
            return ((int)(MathUtil.Clamp_dbl(Health * 100.0D, 1.0D, 100.0D))).ToStringInvariant() + "%";
        }

        public string GetPosText()
        {
            return Pos.Horizontal.X.ToStringInvariant() + ", " + Pos.Horizontal.Y.ToStringInvariant();
        }

        public App.sResult SetLabel(string Text)
        {
            App.sResult Result = new App.sResult();

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
                _Label = null;
                Result.Success = true;
                Result.Problem = "";
                return Result;
            }
            else
            {
                Result = MapLink.Source.ScriptLabelIsValid(Text);
                if ( Result.Success )
                {
                    _Label = Text;
                }
                return Result;
            }
        }

        public void WriteWZLabel(IniWriter File, int PlayerCount)
        {
            if ( _Label != null )
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
                File.AppendSectionName("object_" + MapLink.ArrayPosition.ToStringInvariant());
                File.AppendProperty("id", ID.ToStringInvariant());
                if ( PlayerCount >= 0 ) //not an FMap
                {
                    File.AppendProperty("type", TypeNum.ToStringInvariant());
                    File.AppendProperty("player", UnitGroup.GetPlayerNum(PlayerCount).ToStringInvariant());
                }
                File.AppendProperty("label", _Label);
                File.Gap_Append();
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
    }
}