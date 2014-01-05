using System;
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using SharpFlame.Collections;
using SharpFlame.Colors;
using SharpFlame.FileIO;
using SharpFlame.FileIO.Ini;
using SharpFlame.MathExtra;

namespace SharpFlame
{
    public partial class clsMap
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
            public clsUnitType Type;
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

                if ( UnitToCopy.Type.Type == clsUnitType.enumType.PlayerDroid )
                {
                    IsDesign = !((clsDroidDesign)UnitToCopy.Type).IsTemplate;
                }
                else
                {
                    IsDesign = false;
                }
                if ( IsDesign )
                {
                    clsDroidDesign DroidDesign = new clsDroidDesign();
                    Type = DroidDesign;
                    DroidDesign.CopyDesign((clsDroidDesign)UnitToCopy.Type);
                    DroidDesign.UpdateAttachments();
                }
                else
                {
                    Type = UnitToCopy.Type;
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

                if ( Type.Type == clsUnitType.enumType.PlayerStructure )
                {
                    clsStructureType StructureType = (clsStructureType)Type;
                    clsStructureType.enumStructureType StructureTypeType = StructureType.StructureType;
                    if ( StructureTypeType == clsStructureType.enumStructureType.FactoryModule
                         | StructureTypeType == clsStructureType.enumStructureType.PowerModule
                         | StructureTypeType == clsStructureType.enumStructureType.ResearchModule )
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
                    switch ( Type.Type )
                    {
                        case clsUnitType.enumType.PlayerDroid:
                            TypeNum = 0;
                            break;
                        case clsUnitType.enumType.PlayerStructure:
                            TypeNum = 1;
                            break;
                        case clsUnitType.enumType.Feature:
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

        public ConnectedList<clsUnit, clsMap> Units;

        public class clsUnitSectorConnection
        {
            public clsUnitSectorConnection()
            {
                _UnitLink = new Link<clsUnit>(this);
                _SectorLink = new Link<clsSector>(this);
            }

            protected class Link<SourceType> : ConnectedListLink<clsUnitSectorConnection, SourceType> where SourceType : class
            {
                public Link(clsUnitSectorConnection Owner) : base(Owner)
                {
                }

                public override void AfterRemove()
                {
                    base.AfterRemove();

                    Item.Deallocate();
                }
            }

            protected Link<clsUnit> _UnitLink;
            protected Link<clsSector> _SectorLink;

            public virtual clsUnit Unit
            {
                get { return _UnitLink.Source; }
            }

            public virtual clsSector Sector
            {
                get { return _SectorLink.Source; }
            }

            public static clsUnitSectorConnection Create(clsUnit Unit, clsSector Sector)
            {
                if ( Unit == null )
                {
                    return null;
                }
                if ( Unit.Sectors == null )
                {
                    return null;
                }
                if ( Unit.Sectors.IsBusy )
                {
                    return null;
                }
                if ( Sector == null )
                {
                    return null;
                }
                if ( Sector.Units == null )
                {
                    return null;
                }
                if ( Sector.Units.IsBusy )
                {
                    return null;
                }

                clsUnitSectorConnection Result = new clsUnitSectorConnection();
                Result._UnitLink.Connect(Unit.Sectors);
                Result._SectorLink.Connect(Sector.Units);
                return Result;
            }

            //protected clsUnitSectorConnection()
            //{

            //_UnitLink = new Link<clsMap.clsUnit>(this);
            //_SectorLink = new Link<clsMap.clsSector>(this);


            //}

            public void Deallocate()
            {
                _UnitLink.Deallocate();
                _SectorLink.Deallocate();
            }
        }

        public class clsUnitGroupContainer
        {
            private clsUnitGroup _Item;

            public clsUnitGroup Item
            {
                get { return _Item; }
                set
                {
                    if ( value == _Item )
                    {
                        return;
                    }
                    _Item = value;
                    if ( ChangedEvent != null )
                        ChangedEvent();
                }
            }

            public delegate void ChangedEventHandler();

            private ChangedEventHandler ChangedEvent;

            public event ChangedEventHandler Changed
            {
                add { ChangedEvent = (ChangedEventHandler)Delegate.Combine(ChangedEvent, value); }
                remove { ChangedEvent = (ChangedEventHandler)Delegate.Remove(ChangedEvent, value); }
            }
        }

        private clsUnitGroupContainer _SelectedUnitGroup;

        public clsUnitGroupContainer SelectedUnitGroup
        {
            get { return _SelectedUnitGroup; }
        }

        public class clsUnitGroup
        {
            public clsUnitGroup()
            {
                MapLink = new ConnectedListLink<clsUnitGroup, clsMap>(this);
            }

            public ConnectedListLink<clsUnitGroup, clsMap> MapLink;

            public int WZ_StartPos = -1;

            public string GetFMapINIPlayerText()
            {
                if ( WZ_StartPos < 0 | WZ_StartPos >= Constants.PlayerCountMax )
                {
                    return "scavenger";
                }
                else
                {
                    return WZ_StartPos.ToStringInvariant();
                }
            }

            public string GetLNDPlayerText()
            {
                if ( WZ_StartPos < 0 | WZ_StartPos >= Constants.PlayerCountMax )
                {
                    return 7.ToStringInvariant();
                }
                else
                {
                    return WZ_StartPos.ToStringInvariant();
                }
            }

            public int GetPlayerNum(int PlayerCount)
            {
                if ( WZ_StartPos < 0 | WZ_StartPos >= Constants.PlayerCountMax )
                {
                    return Math.Max(PlayerCount, 7);
                }
                else
                {
                    return WZ_StartPos;
                }
            }
        }

        public ConnectedList<clsUnitGroup, clsMap> UnitGroups;
        public clsUnitGroup ScavengerUnitGroup;

        public UInt32 GetAvailableID()
        {
            clsUnit Unit = default(clsUnit);
            UInt32 ID = 0;

            ID = 1U;
            foreach ( clsUnit tempLoopVar_Unit in Units )
            {
                Unit = tempLoopVar_Unit;
                if ( Unit.ID >= ID )
                {
                    ID = Unit.ID + 1U;
                }
            }

            return ID;
        }

        public class clsUnitAdd
        {
            public clsMap Map;
            public clsUnit NewUnit;
            public UInt32 ID = 0U;
            public string Label = null;
            public bool StoreChange = false;

            public bool Perform()
            {
                if ( Map == null )
                {
                    Debugger.Break();
                    return false;
                }
                if ( NewUnit == null )
                {
                    Debugger.Break();
                    return false;
                }

                if ( NewUnit.MapLink.IsConnected )
                {
                    MessageBox.Show("Error: Added object already has a map assigned.");
                    return false;
                }
                if ( NewUnit.UnitGroup == null )
                {
                    MessageBox.Show("Error: Added object has no group.");
                    NewUnit.UnitGroup = Map.ScavengerUnitGroup;
                    return false;
                }
                if ( NewUnit.UnitGroup.MapLink.Source != Map )
                {
                    MessageBox.Show("Error: Something terrible happened.");
                    return false;
                }

                if ( StoreChange )
                {
                    clsUnitChange UnitChange = new clsUnitChange();
                    UnitChange.Type = clsUnitChange.enumType.Added;
                    UnitChange.Unit = NewUnit;
                    Map.UnitChanges.Add(UnitChange);
                }

                if ( ID <= 0U )
                {
                    ID = Map.GetAvailableID();
                }
                else if ( Map.IDUsage(ID) != null )
                {
                    ID = Map.GetAvailableID();
                }

                NewUnit.ID = ID;

                NewUnit.MapLink.Connect(Map.Units);

                NewUnit.Pos.Horizontal.X = MathUtil.Clamp_int(NewUnit.Pos.Horizontal.X, 0, Map.Terrain.TileSize.X * App.TerrainGridSpacing - 1);
                NewUnit.Pos.Horizontal.Y = MathUtil.Clamp_int(NewUnit.Pos.Horizontal.Y, 0, Map.Terrain.TileSize.Y * App.TerrainGridSpacing - 1);
                NewUnit.Pos.Altitude = (int)(Math.Ceiling(Map.GetTerrainHeight(NewUnit.Pos.Horizontal)));

                if ( Label != null )
                {
                    NewUnit.SetLabel(Label);
                }

                Map.UnitSectorsCalc(NewUnit);

                if ( Map.SectorGraphicsChanges != null )
                {
                    Map.UnitSectorsGraphicsChanged(NewUnit);
                }

                return true;
            }
        }

        public void UnitRemoveStoreChange(int Num)
        {
            clsUnitChange UnitChange = new clsUnitChange();
            UnitChange.Type = clsUnitChange.enumType.Deleted;
            UnitChange.Unit = Units[Num];
            UnitChanges.Add(UnitChange);

            UnitRemove(Num);
        }

        public void UnitRemove(int Num)
        {
            clsUnit Unit = default(clsUnit);

            Unit = Units[Num];

            if ( SectorGraphicsChanges != null )
            {
                UnitSectorsGraphicsChanged(Unit);
            }

            if ( ViewInfo != null )
            {
                clsViewInfo.clsMouseOver.clsOverTerrain MouseOverTerrain = ViewInfo.GetMouseOverTerrain();
                if ( MouseOverTerrain != null )
                {
                    int Pos = MouseOverTerrain.Units.FindFirstItemPosition(Unit);
                    if ( Pos >= 0 )
                    {
                        MouseOverTerrain.Units.Remove(Pos);
                    }
                }
            }

            Unit.DisconnectFromMap();
        }

        public void UnitSwap(clsUnit OldUnit, clsUnit NewUnit)
        {
            if ( OldUnit.MapLink.Source != this )
            {
                Debugger.Break();
                return;
            }

            UnitRemoveStoreChange(OldUnit.MapLink.ArrayPosition);
            clsUnitAdd UnitAdd = new clsUnitAdd();
            UnitAdd.Map = this;
            UnitAdd.StoreChange = true;
            UnitAdd.ID = OldUnit.ID;
            UnitAdd.NewUnit = NewUnit;
            UnitAdd.Label = OldUnit.Label;
            UnitAdd.Perform();
            App.ErrorIDChange(OldUnit.ID, NewUnit, "UnitSwap");
        }

        public void MakeDefaultUnitGroups()
        {
            int A = 0;
            clsUnitGroup NewGroup = default(clsUnitGroup);

            UnitGroups.Clear();
            for ( A = 0; A <= Constants.PlayerCountMax - 1; A++ )
            {
                NewGroup = new clsUnitGroup();
                NewGroup.WZ_StartPos = A;
                NewGroup.MapLink.Connect(UnitGroups);
            }
            ScavengerUnitGroup = new clsUnitGroup();
            ScavengerUnitGroup.MapLink.Connect(UnitGroups);
            ScavengerUnitGroup.WZ_StartPos = -1;
        }

        public sRGB_sng GetUnitGroupColour(clsUnitGroup ColourUnitGroup)
        {
            if ( ColourUnitGroup.WZ_StartPos < 0 )
            {
                return new sRGB_sng(1.0F, 1.0F, 1.0F);
            }
            else
            {
                return App.PlayerColour[ColourUnitGroup.WZ_StartPos].Colour;
            }
        }

        public sRGB_sng GetUnitGroupMinimapColour(clsUnitGroup ColourUnitGroup)
        {
            if ( ColourUnitGroup.WZ_StartPos < 0 )
            {
                return new sRGB_sng(1.0F, 1.0F, 1.0F);
            }
            else
            {
                return App.PlayerColour[ColourUnitGroup.WZ_StartPos].MinimapColour;
            }
        }

        public clsUnit IDUsage(UInt32 ID)
        {
            foreach ( clsUnit Unit in Units )
            {
                if ( Unit.ID == ID )
                {
                    return Unit;
                    //							break;
                }
            }

            return null;
        }

        public class clsUnitCreate
        {
            public clsMap Map;
            public clsUnitType ObjectType;
            public sXY_int Horizontal;
            public clsUnitGroup UnitGroup;
            public bool AutoWalls = false;
            public int Rotation = 0;
            public bool RandomizeRotation = false;

            public clsUnit Perform()
            {
                if ( AutoWalls )
                {
                    if ( ObjectType.Type == clsUnitType.enumType.PlayerStructure )
                    {
                        clsStructureType StructureType = (clsStructureType)ObjectType;
                        if ( StructureType.WallLink.IsConnected )
                        {
                            clsWallType AutoWallType = null;
                            AutoWallType = StructureType.WallLink.Source;
                            Map.PerformTileWall(AutoWallType, Map.GetPosTileNum(Horizontal), true);
                            return null;
                        }
                    }
                }
                clsUnit newUnit = new clsUnit();
                if ( RandomizeRotation )
                {
                    newUnit.Rotation = (int)(Conversion.Int(VBMath.Rnd() * 360.0D));
                }
                else
                {
                    newUnit.Rotation = Rotation;
                }
                newUnit.UnitGroup = UnitGroup;
                newUnit.Pos = Map.TileAlignedPosFromMapPos(Horizontal, ObjectType.get_GetFootprintSelected(newUnit.Rotation));
                newUnit.Type = ObjectType;
                clsUnitAdd UnitAdd = new clsUnitAdd();
                UnitAdd.Map = Map;
                UnitAdd.NewUnit = newUnit;
                UnitAdd.StoreChange = true;
                UnitAdd.Perform();
                return newUnit;
            }
        }

        public void SetObjectCreatorDefaults(clsUnitCreate objectCreator)
        {
            objectCreator.Map = this;

            objectCreator.ObjectType = Program.frmMainInstance.SingleSelectedObjectType;
            objectCreator.AutoWalls = Program.frmMainInstance.cbxAutoWalls.Checked;
            objectCreator.UnitGroup = SelectedUnitGroup.Item;
            try
            {
                int Rotation = 0;
                IOUtil.InvariantParse(Program.frmMainInstance.txtNewObjectRotation.Text, ref Rotation);
                if ( Rotation < 0 | Rotation > 359 )
                {
                    objectCreator.Rotation = 0;
                }
                else
                {
                    objectCreator.Rotation = Rotation;
                }
            }
            catch
            {
                objectCreator.Rotation = 0;
            }
            objectCreator.RandomizeRotation = Program.frmMainInstance.cbxObjectRandomRotation.Checked;
        }
    }
}