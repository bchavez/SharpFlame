namespace FlaME
{
    using FlaME.My;
    using ICSharpCode.SharpZipLib.Zip;
    using Matrix3D;
    using Microsoft.VisualBasic;
    using Microsoft.VisualBasic.CompilerServices;
    using OpenTK;
    using OpenTK.Graphics.OpenGL;
    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Windows.Forms;

    public class clsMap
    {
        [AccessedThroughProperty("MakeMinimapTimer")]
        private Timer _MakeMinimapTimer;
        private bool _ReadyForUserInput;
        private clsUnitGroupContainer _SelectedUnitGroup;
        public clsAutoSave AutoSave;
        public clsAutoTextureChanges AutoTextureChanges;
        public bool ChangedSinceSave;
        public frmCompile CompileScreen;
        public modLists.ConnectedListLink<clsMap, frmMain> frmMainLink;
        public modLists.SimpleClassList<clsGatewayChange> GatewayChanges;
        public modLists.ConnectedList<clsGateway, clsMap> Gateways;
        public int HeightMultiplier;
        public clsInterfaceOptions InterfaceOptions;
        public TabPage MapView_TabPage;
        public modLists.SimpleClassList<clsMessage> Messages;
        public int Minimap_GLTexture;
        public int Minimap_Texture_Size;
        private bool MinimapPending;
        public clsPainter Painter;
        public clsPathInfo PathInfo;
        public clsUnitGroup ScavengerUnitGroup;
        public modLists.ConnectedList<clsScriptArea, clsMap> ScriptAreas;
        public modLists.ConnectedList<clsScriptPosition, clsMap> ScriptPositions;
        public modMath.sXY_int SectorCount;
        public clsSectorChanges SectorGraphicsChanges;
        public clsSector[,] Sectors;
        public clsSectorChanges SectorTerrainUndoChanges;
        public clsSectorChanges SectorUnitHeightsChanges;
        public modMath.clsXY_int Selected_Area_VertexA;
        public modMath.clsXY_int Selected_Area_VertexB;
        public modMath.clsXY_int Selected_Tile_A;
        public modMath.clsXY_int Selected_Tile_B;
        public modLists.ConnectedList<clsUnit, clsMap> SelectedUnits;
        public clsShadowSector[,] ShadowSectors;
        public bool SuppressMinimap;
        public clsTerrain Terrain;
        public clsTerrainUpdate TerrainInterpretChanges;
        public byte[] Tile_TypeNum;
        public clsTileset Tileset;
        public int UndoPosition;
        public modLists.SimpleClassList<clsUndo> Undos;
        public modMath.clsXY_int Unit_Selected_Area_VertexA;
        public modLists.SimpleClassList<clsUnitChange> UnitChanges;
        public modLists.ConnectedList<clsUnitGroup, clsMap> UnitGroups;
        public modLists.ConnectedList<clsUnit, clsMap> Units;
        public clsViewInfo ViewInfo;

        public event ChangedEventHandler Changed;

        public clsMap()
        {
            this.frmMainLink = new modLists.ConnectedListLink<clsMap, frmMain>(this);
            this.Sectors = new clsSector[0, 0];
            this.ShadowSectors = new clsShadowSector[0, 0];
            this.HeightMultiplier = 2;
            this._ReadyForUserInput = false;
            this.ChangedSinceSave = false;
            this.AutoSave = new clsAutoSave();
            this.Painter = new clsPainter();
            this.Tile_TypeNum = new byte[0];
            this.Gateways = new modLists.ConnectedList<clsGateway, clsMap>(this);
            this.Units = new modLists.ConnectedList<clsUnit, clsMap>(this);
            this.UnitGroups = new modLists.ConnectedList<clsUnitGroup, clsMap>(this);
            this.ScriptPositions = new modLists.ConnectedList<clsScriptPosition, clsMap>(this);
            this.ScriptAreas = new modLists.ConnectedList<clsScriptArea, clsMap>(this);
            this.Initialize();
        }

        public clsMap(modMath.sXY_int TileSize)
        {
            this.frmMainLink = new modLists.ConnectedListLink<clsMap, frmMain>(this);
            this.Sectors = new clsSector[0, 0];
            this.ShadowSectors = new clsShadowSector[0, 0];
            this.HeightMultiplier = 2;
            this._ReadyForUserInput = false;
            this.ChangedSinceSave = false;
            this.AutoSave = new clsAutoSave();
            this.Painter = new clsPainter();
            this.Tile_TypeNum = new byte[0];
            this.Gateways = new modLists.ConnectedList<clsGateway, clsMap>(this);
            this.Units = new modLists.ConnectedList<clsUnit, clsMap>(this);
            this.UnitGroups = new modLists.ConnectedList<clsUnitGroup, clsMap>(this);
            this.ScriptPositions = new modLists.ConnectedList<clsScriptPosition, clsMap>(this);
            this.ScriptAreas = new modLists.ConnectedList<clsScriptArea, clsMap>(this);
            this.Initialize();
            this.TerrainBlank(TileSize);
            this.TileType_Reset();
        }

        public clsMap(clsMap MapToCopy, modMath.sXY_int Offset, modMath.sXY_int Area)
        {
            modMath.sXY_int _int2;
            int num3;
            int num4;
            modMath.sXY_int _int3;
            IEnumerator enumerator;
            IEnumerator enumerator2;
            this.frmMainLink = new modLists.ConnectedListLink<clsMap, frmMain>(this);
            this.Sectors = new clsSector[0, 0];
            this.ShadowSectors = new clsShadowSector[0, 0];
            this.HeightMultiplier = 2;
            this._ReadyForUserInput = false;
            this.ChangedSinceSave = false;
            this.AutoSave = new clsAutoSave();
            this.Painter = new clsPainter();
            this.Tile_TypeNum = new byte[0];
            this.Gateways = new modLists.ConnectedList<clsGateway, clsMap>(this);
            this.Units = new modLists.ConnectedList<clsUnit, clsMap>(this);
            this.UnitGroups = new modLists.ConnectedList<clsUnitGroup, clsMap>(this);
            this.ScriptPositions = new modLists.ConnectedList<clsScriptPosition, clsMap>(this);
            this.ScriptAreas = new modLists.ConnectedList<clsScriptArea, clsMap>(this);
            this.Initialize();
            int num = Math.Min(MapToCopy.Terrain.TileSize.X - Offset.X, Area.X);
            int num2 = Math.Min(MapToCopy.Terrain.TileSize.Y - Offset.Y, Area.Y);
            this.Terrain = new clsTerrain(Area);
            int num5 = this.Terrain.TileSize.Y - 1;
            for (num4 = 0; num4 <= num5; num4++)
            {
                int num6 = this.Terrain.TileSize.X - 1;
                num3 = 0;
                while (num3 <= num6)
                {
                    this.Terrain.Tiles[num3, num4].Texture.TextureNum = -1;
                    num3++;
                }
            }
            int num7 = num2;
            for (num4 = 0; num4 <= num7; num4++)
            {
                int num8 = num;
                num3 = 0;
                while (num3 <= num8)
                {
                    this.Terrain.Vertices[num3, num4].Height = MapToCopy.Terrain.Vertices[Offset.X + num3, Offset.Y + num4].Height;
                    this.Terrain.Vertices[num3, num4].Terrain = MapToCopy.Terrain.Vertices[Offset.X + num3, Offset.Y + num4].Terrain;
                    num3++;
                }
            }
            int num9 = num2 - 1;
            for (num4 = 0; num4 <= num9; num4++)
            {
                int num10 = num - 1;
                num3 = 0;
                while (num3 <= num10)
                {
                    this.Terrain.Tiles[num3, num4].Copy(MapToCopy.Terrain.Tiles[Offset.X + num3, Offset.Y + num4]);
                    num3++;
                }
            }
            int num11 = num2;
            for (num4 = 0; num4 <= num11; num4++)
            {
                int num12 = num - 1;
                num3 = 0;
                while (num3 <= num12)
                {
                    this.Terrain.SideH[num3, num4].Road = MapToCopy.Terrain.SideH[Offset.X + num3, Offset.Y + num4].Road;
                    num3++;
                }
            }
            int num13 = num2 - 1;
            for (num4 = 0; num4 <= num13; num4++)
            {
                int num14 = num;
                num3 = 0;
                while (num3 <= num14)
                {
                    this.Terrain.SideV[num3, num4].Road = MapToCopy.Terrain.SideV[Offset.X + num3, Offset.Y + num4].Road;
                    num3++;
                }
            }
            this.SectorCount.X = (int) Math.Round(Math.Ceiling((double) (((double) Area.X) / 8.0)));
            this.SectorCount.Y = (int) Math.Round(Math.Ceiling((double) (((double) Area.Y) / 8.0)));
            this.Sectors = new clsSector[(this.SectorCount.X - 1) + 1, (this.SectorCount.Y - 1) + 1];
            int num15 = this.SectorCount.Y - 1;
            for (num4 = 0; num4 <= num15; num4++)
            {
                int num16 = this.SectorCount.X - 1;
                for (num3 = 0; num3 <= num16; num3++)
                {
                    _int3 = new modMath.sXY_int(num3, num4);
                    this.Sectors[num3, num4] = new clsSector(_int3);
                }
            }
            clsUnitAdd add = new clsUnitAdd {
                Map = this
            };
            try
            {
                enumerator = MapToCopy.Gateways.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    clsGateway current = (clsGateway) enumerator.Current;
                    _int3 = new modMath.sXY_int(current.PosA.X - Offset.X, current.PosA.Y - Offset.Y);
                    modMath.sXY_int posB = new modMath.sXY_int(current.PosB.X - Offset.X, current.PosB.Y - Offset.Y);
                    this.GatewayCreate(_int3, posB);
                }
            }
            finally
            {
                if (enumerator is IDisposable)
                {
                    (enumerator as IDisposable).Dispose();
                }
            }
            _int2.X = (0 - Offset.X) * 0x80;
            _int2.Y = (0 - Offset.Y) * 0x80;
            try
            {
                enumerator2 = MapToCopy.Units.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    clsUnit unitToCopy = (clsUnit) enumerator2.Current;
                    modMath.sXY_int horizontal = unitToCopy.Pos.Horizontal + _int2;
                    if (this.PosIsOnMap(horizontal))
                    {
                        clsUnit unit = new clsUnit(unitToCopy, this);
                        unit.Pos.Horizontal = horizontal;
                        add.NewUnit = unit;
                        add.Label = unitToCopy.Label;
                        add.Perform();
                    }
                }
            }
            finally
            {
                if (enumerator2 is IDisposable)
                {
                    (enumerator2 as IDisposable).Dispose();
                }
            }
        }

        public clsResult AutoSavePerform()
        {
            clsResult result2 = new clsResult("Autosave");
            if (!Directory.Exists(modProgram.AutoSavePath))
            {
                try
                {
                    Directory.CreateDirectory(modProgram.AutoSavePath);
                }
                catch (Exception exception1)
                {
                    ProjectData.SetProjectError(exception1);
                    Exception exception = exception1;
                    result2.ProblemAdd("Unable to create directory at \"" + modProgram.AutoSavePath + "\"");
                    ProjectData.ClearProjectError();
                }
            }
            DateTime now = DateAndTime.Now;
            string path = modProgram.AutoSavePath + "autosaved-" + modIO.InvariantToString_int(now.Year) + "-" + modProgram.MinDigits(now.Month, 2) + "-" + modProgram.MinDigits(now.Day, 2) + "-" + modProgram.MinDigits(now.Hour, 2) + "-" + modProgram.MinDigits(now.Minute, 2) + "-" + modProgram.MinDigits(now.Second, 2) + "-" + modProgram.MinDigits(now.Millisecond, 3) + ".fmap";
            result2.Add(this.Write_FMap(path, false, modSettings.Settings.AutoSaveCompress));
            return result2;
        }

        public void AutoSaveTest()
        {
            if ((modSettings.Settings.AutoSaveEnabled && (this.AutoSave.ChangeCount >= modSettings.Settings.AutoSaveMinChanges)) && (DateAndTime.DateDiff("s", this.AutoSave.SavedDate, DateAndTime.Now, FirstDayOfWeek.Sunday, FirstWeekOfYear.Jan1) >= modSettings.Settings.AutoSaveMinInterval_s))
            {
                this.AutoSave.ChangeCount = 0;
                this.AutoSave.SavedDate = DateAndTime.Now;
                modProgram.ShowWarnings(this.AutoSavePerform());
            }
        }

        public void CancelUserInput()
        {
            if (this._ReadyForUserInput)
            {
                this._ReadyForUserInput = false;
                if (this.CompileScreen != null)
                {
                    this.CompileScreen.Close();
                    this.CompileScreen = null;
                }
                this.SectorAll_GLLists_Delete();
                this.MinimapGLDelete();
                this.ShadowSectors = null;
                int num3 = this.SectorCount.Y - 1;
                for (int i = 0; i <= num3; i++)
                {
                    int num4 = this.SectorCount.X - 1;
                    for (int j = 0; j <= num4; j++)
                    {
                        this.Sectors[j, i].Deallocate();
                    }
                }
                this.Sectors = null;
                this.SectorGraphicsChanges.Deallocate();
                this.SectorGraphicsChanges = null;
                this.SectorUnitHeightsChanges.Deallocate();
                this.SectorUnitHeightsChanges = null;
                this.SectorTerrainUndoChanges.Deallocate();
                this.SectorTerrainUndoChanges = null;
                this.AutoTextureChanges.Deallocate();
                this.AutoTextureChanges = null;
                this.TerrainInterpretChanges.Deallocate();
                this.TerrainInterpretChanges = null;
                this.UnitChanges = null;
                this.GatewayChanges = null;
                this.Undos = null;
                this.SelectedUnits.Deallocate();
                this.SelectedUnits = null;
                this.Selected_Tile_A = null;
                this.Selected_Tile_B = null;
                this.Selected_Area_VertexA = null;
                this.Selected_Area_VertexB = null;
                this.Unit_Selected_Area_VertexA = null;
                this.ViewInfo = null;
                this._SelectedUnitGroup = null;
                this.Messages = null;
            }
        }

        public bool CheckMessages()
        {
            DateTime now = DateAndTime.Now;
            bool flag = false;
            int position = 0;
            while (position < this.Messages.Count)
            {
                if (DateAndTime.DateDiff(DateInterval.Second, this.Messages[position].CreatedDate, now, FirstDayOfWeek.Sunday, FirstWeekOfYear.Jan1) >= 6L)
                {
                    this.Messages.Remove(position);
                    flag = true;
                }
                else
                {
                    position++;
                }
            }
            return flag;
        }

        public bool ClosePrompt()
        {
            if (!this.ChangedSinceSave)
            {
                return true;
            }
            frmClose close = new frmClose(this.GetTitle());
            switch (close.ShowDialog(modMain.frmMainInstance))
            {
                case DialogResult.OK:
                    return this.Save_FMap_Prompt();

                case DialogResult.Cancel:
                    return false;

                case DialogResult.Yes:
                    return this.Save_FMap_Quick();

                case DialogResult.No:
                    return true;
            }
            Debugger.Break();
            return false;
        }

        public clsResult CreateWZObjects(sCreateWZObjectsArgs Args)
        {
            int num;
            clsWZBJOUnit current;
            clsUnit unit3;
            IEnumerator enumerator;
            IEnumerator enumerator2;
            clsResult output = new clsResult("Creating objects");
            modLists.SimpleClassList<clsWZBJOUnit> bJOUnits = Args.BJOUnits;
            clsINIStructures iNIStructures = Args.INIStructures;
            clsINIDroids iNIDroids = Args.INIDroids;
            clsINIFeatures iNIFeatures = Args.INIFeatures;
            clsUnitAdd add = new clsUnitAdd {
                Map = this
            };
            uint num2 = 1;
            try
            {
                enumerator = bJOUnits.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    current = (clsWZBJOUnit) enumerator.Current;
                    if (current.ID >= num2)
                    {
                        num2 = current.ID + 1;
                    }
                }
            }
            finally
            {
                if (enumerator is IDisposable)
                {
                    (enumerator as IDisposable).Dispose();
                }
            }
            if (iNIStructures != null)
            {
                int num12 = iNIStructures.StructureCount - 1;
                for (num = 0; num <= num12; num++)
                {
                    if (iNIStructures.Structures[num].ID >= num2)
                    {
                        num2 = iNIStructures.Structures[num].ID + 1;
                    }
                }
            }
            if (iNIFeatures != null)
            {
                int num13 = iNIFeatures.FeatureCount - 1;
                for (num = 0; num <= num13; num++)
                {
                    if (iNIFeatures.Features[num].ID >= num2)
                    {
                        num2 = iNIFeatures.Features[num].ID + 1;
                    }
                }
            }
            if (iNIDroids != null)
            {
                int num14 = iNIDroids.DroidCount - 1;
                for (num = 0; num <= num14; num++)
                {
                    if (iNIDroids.Droids[num].ID >= num2)
                    {
                        num2 = iNIDroids.Droids[num].ID + 1;
                    }
                }
            }
            try
            {
                enumerator2 = bJOUnits.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    current = (clsWZBJOUnit) enumerator2.Current;
                    unit3 = new clsUnit {
                        ID = current.ID,
                        Type = modProgram.ObjectData.FindOrCreateUnitType(current.Code, current.ObjectType, -1)
                    };
                    if (unit3.Type == null)
                    {
                        output.ProblemAdd("Unable to create object type.");
                        return output;
                    }
                    if (current.Player >= 10L)
                    {
                        unit3.UnitGroup = this.ScavengerUnitGroup;
                    }
                    else
                    {
                        unit3.UnitGroup = this.UnitGroups[(int) current.Player];
                    }
                    unit3.Pos = current.Pos;
                    unit3.Rotation = (int) Math.Min(current.Rotation, 0x167);
                    if (current.ID == 0)
                    {
                        current.ID = num2;
                        modProgram.ZeroIDWarning(unit3, current.ID, output);
                    }
                    add.NewUnit = unit3;
                    add.ID = current.ID;
                    add.Perform();
                    modProgram.ErrorIDChange(current.ID, unit3, "CreateWZObjects");
                    if (num2 == current.ID)
                    {
                        num2 = unit3.ID + 1;
                    }
                }
            }
            finally
            {
                if (enumerator2 is IDisposable)
                {
                    (enumerator2 as IDisposable).Dispose();
                }
            }
            clsUnitType type7 = null;
            int num5 = 0;
            int num10 = 0;
            int num11 = 0;
            int num4 = 0;
            int num9 = 0;
            int num8 = 0;
            int num6 = 0;
            modMath.sXY_int startTile = new modMath.sXY_int(0, 0);
            clsStructureType type = modProgram.ObjectData.FindFirstStructureType(clsStructureType.enumStructureType.FactoryModule);
            clsStructureType type5 = modProgram.ObjectData.FindFirstStructureType(clsStructureType.enumStructureType.ResearchModule);
            clsStructureType type4 = modProgram.ObjectData.FindFirstStructureType(clsStructureType.enumStructureType.PowerModule);
            if (type == null)
            {
                output.WarningAdd("No factory module loaded.");
            }
            if (type5 == null)
            {
                output.WarningAdd("No research module loaded.");
            }
            if (type4 == null)
            {
                output.WarningAdd("No power module loaded.");
            }
            if (iNIStructures != null)
            {
                int num15 = iNIStructures.StructureCount - 1;
                for (num = 0; num <= num15; num++)
                {
                    int num7;
                    clsStructureType type3;
                    clsStructureType type6;
                    if (iNIStructures.Structures[num].Pos == null)
                    {
                        num9++;
                        continue;
                    }
                    if (!modProgram.PosIsWithinTileArea(iNIStructures.Structures[num].Pos.WorldPos.Horizontal, startTile, this.Terrain.TileSize))
                    {
                        num9++;
                        continue;
                    }
                    type7 = modProgram.ObjectData.FindOrCreateUnitType(iNIStructures.Structures[num].Code, clsUnitType.enumType.PlayerStructure, iNIStructures.Structures[num].WallType);
                    if (type7.Type == clsUnitType.enumType.PlayerStructure)
                    {
                        type6 = (clsStructureType) type7;
                    }
                    else
                    {
                        type6 = null;
                    }
                    if (type6 == null)
                    {
                        num5++;
                        continue;
                    }
                    unit3 = new clsUnit {
                        Type = type6
                    };
                    if (iNIStructures.Structures[num].UnitGroup == null)
                    {
                        unit3.UnitGroup = this.ScavengerUnitGroup;
                    }
                    else
                    {
                        unit3.UnitGroup = iNIStructures.Structures[num].UnitGroup;
                    }
                    unit3.Pos = iNIStructures.Structures[num].Pos.WorldPos;
                    unit3.Rotation = (int) Math.Round((double) ((iNIStructures.Structures[num].Rotation.Direction * 360.0) / 65536.0));
                    if (unit3.Rotation == 360)
                    {
                        unit3.Rotation = 0;
                    }
                    if (iNIStructures.Structures[num].HealthPercent >= 0)
                    {
                        unit3.Health = modMath.Clamp_dbl(((double) iNIStructures.Structures[num].HealthPercent) / 100.0, 0.01, 1.0);
                    }
                    if (iNIStructures.Structures[num].ID == 0)
                    {
                        iNIStructures.Structures[num].ID = num2;
                        modProgram.ZeroIDWarning(unit3, iNIStructures.Structures[num].ID, output);
                    }
                    add.NewUnit = unit3;
                    add.ID = iNIStructures.Structures[num].ID;
                    add.Perform();
                    modProgram.ErrorIDChange(iNIStructures.Structures[num].ID, unit3, "Load_WZ->INIStructures");
                    if (num2 == iNIStructures.Structures[num].ID)
                    {
                        num2 = unit3.ID + 1;
                    }
                    switch (type6.StructureType)
                    {
                        case clsStructureType.enumStructureType.Factory:
                            num7 = 2;
                            type3 = type;
                            break;

                        case clsStructureType.enumStructureType.VTOLFactory:
                            num7 = 2;
                            type3 = type;
                            break;

                        case clsStructureType.enumStructureType.PowerGenerator:
                            num7 = 1;
                            type3 = type4;
                            break;

                        case clsStructureType.enumStructureType.Research:
                            num7 = 1;
                            type3 = type5;
                            break;

                        default:
                            num7 = 0;
                            type3 = null;
                            break;
                    }
                    if (iNIStructures.Structures[num].ModuleCount > num7)
                    {
                        iNIStructures.Structures[num].ModuleCount = num7;
                        num8++;
                    }
                    else if (iNIStructures.Structures[num].ModuleCount < 0)
                    {
                        iNIStructures.Structures[num].ModuleCount = 0;
                        num8++;
                    }
                    if (type3 != null)
                    {
                        int num16 = iNIStructures.Structures[num].ModuleCount - 1;
                        for (int i = 0; i <= num16; i++)
                        {
                            clsUnit unit2 = new clsUnit {
                                Type = type3,
                                UnitGroup = unit3.UnitGroup,
                                Pos = unit3.Pos,
                                Rotation = unit3.Rotation
                            };
                            add.NewUnit = unit2;
                            add.ID = num2;
                            add.Perform();
                            num2 = unit2.ID + 1;
                        }
                    }
                }
                if (num9 > 0)
                {
                    output.WarningAdd(Conversions.ToString(num9) + " structures had an invalid position and were removed.");
                }
                if (num8 > 0)
                {
                    output.WarningAdd(Conversions.ToString(num8) + " structures had an invalid number of modules.");
                }
            }
            if (iNIFeatures != null)
            {
                int num17 = iNIFeatures.FeatureCount - 1;
                for (num = 0; num <= num17; num++)
                {
                    if (iNIFeatures.Features[num].Pos == null)
                    {
                        num6++;
                    }
                    else if (!modProgram.PosIsWithinTileArea(iNIFeatures.Features[num].Pos.WorldPos.Horizontal, startTile, this.Terrain.TileSize))
                    {
                        num6++;
                    }
                    else
                    {
                        clsFeatureType type2;
                        type7 = modProgram.ObjectData.FindOrCreateUnitType(iNIFeatures.Features[num].Code, clsUnitType.enumType.Feature, -1);
                        if (type7.Type == clsUnitType.enumType.Feature)
                        {
                            type2 = (clsFeatureType) type7;
                        }
                        else
                        {
                            type2 = null;
                        }
                        if (type2 == null)
                        {
                            num5++;
                        }
                        else
                        {
                            unit3 = new clsUnit {
                                Type = type2,
                                UnitGroup = this.ScavengerUnitGroup,
                                Pos = iNIFeatures.Features[num].Pos.WorldPos,
                                Rotation = (int) Math.Round((double) ((iNIFeatures.Features[num].Rotation.Direction * 360.0) / 65536.0))
                            };
                            if (unit3.Rotation == 360)
                            {
                                unit3.Rotation = 0;
                            }
                            if (iNIFeatures.Features[num].HealthPercent >= 0)
                            {
                                unit3.Health = modMath.Clamp_dbl(((double) iNIFeatures.Features[num].HealthPercent) / 100.0, 0.01, 1.0);
                            }
                            if (iNIFeatures.Features[num].ID == 0)
                            {
                                iNIFeatures.Features[num].ID = num2;
                                modProgram.ZeroIDWarning(unit3, iNIFeatures.Features[num].ID, output);
                            }
                            add.NewUnit = unit3;
                            add.ID = iNIFeatures.Features[num].ID;
                            add.Perform();
                            modProgram.ErrorIDChange(iNIFeatures.Features[num].ID, unit3, "Load_WZ->INIFeatures");
                            if (num2 == iNIFeatures.Features[num].ID)
                            {
                                num2 = unit3.ID + 1;
                            }
                        }
                    }
                }
                if (num6 > 0)
                {
                    output.WarningAdd(Conversions.ToString(num6) + " features had an invalid position and were removed.");
                }
            }
            if (iNIDroids != null)
            {
                int num18 = iNIDroids.DroidCount - 1;
                for (num = 0; num <= num18; num++)
                {
                    if (iNIDroids.Droids[num].Pos == null)
                    {
                        num4++;
                    }
                    else if (!modProgram.PosIsWithinTileArea(iNIDroids.Droids[num].Pos.WorldPos.Horizontal, startTile, this.Terrain.TileSize))
                    {
                        num4++;
                    }
                    else
                    {
                        clsDroidDesign design;
                        if ((iNIDroids.Droids[num].Template == null) | (iNIDroids.Droids[num].Template == ""))
                        {
                            clsDroidDesign.sLoadPartsArgs args;
                            design = new clsDroidDesign();
                            if (!design.SetDroidType((modProgram.enumDroidType) ((byte) iNIDroids.Droids[num].DroidType)))
                            {
                                num11++;
                            }
                            args.Body = modProgram.ObjectData.FindOrCreateBody(iNIDroids.Droids[num].Body);
                            if (args.Body == null)
                            {
                                num10++;
                            }
                            else if (args.Body.IsUnknown)
                            {
                                num10++;
                            }
                            args.Propulsion = modProgram.ObjectData.FindOrCreatePropulsion(iNIDroids.Droids[num].Propulsion);
                            if (args.Propulsion == null)
                            {
                                num10++;
                            }
                            else if (args.Propulsion.IsUnknown)
                            {
                                num10++;
                            }
                            args.Construct = modProgram.ObjectData.FindOrCreateConstruct(iNIDroids.Droids[num].Construct);
                            if (args.Construct == null)
                            {
                                num10++;
                            }
                            else if (args.Construct.IsUnknown)
                            {
                                num10++;
                            }
                            args.Repair = modProgram.ObjectData.FindOrCreateRepair(iNIDroids.Droids[num].Repair);
                            if (args.Repair == null)
                            {
                                num10++;
                            }
                            else if (args.Repair.IsUnknown)
                            {
                                num10++;
                            }
                            args.Sensor = modProgram.ObjectData.FindOrCreateSensor(iNIDroids.Droids[num].Sensor);
                            if (args.Sensor == null)
                            {
                                num10++;
                            }
                            else if (args.Sensor.IsUnknown)
                            {
                                num10++;
                            }
                            args.Brain = modProgram.ObjectData.FindOrCreateBrain(iNIDroids.Droids[num].Brain);
                            if (args.Brain == null)
                            {
                                num10++;
                            }
                            else if (args.Brain.IsUnknown)
                            {
                                num10++;
                            }
                            args.ECM = modProgram.ObjectData.FindOrCreateECM(iNIDroids.Droids[num].ECM);
                            if (args.ECM == null)
                            {
                                num10++;
                            }
                            else if (args.ECM.IsUnknown)
                            {
                                num10++;
                            }
                            args.Weapon1 = modProgram.ObjectData.FindOrCreateWeapon(iNIDroids.Droids[num].Weapons[0]);
                            if (args.Weapon1 == null)
                            {
                                num10++;
                            }
                            else if (args.Weapon1.IsUnknown)
                            {
                                num10++;
                            }
                            args.Weapon2 = modProgram.ObjectData.FindOrCreateWeapon(iNIDroids.Droids[num].Weapons[1]);
                            if (args.Weapon2 == null)
                            {
                                num10++;
                            }
                            else if (args.Weapon2.IsUnknown)
                            {
                                num10++;
                            }
                            args.Weapon3 = modProgram.ObjectData.FindOrCreateWeapon(iNIDroids.Droids[num].Weapons[2]);
                            if (args.Weapon3 == null)
                            {
                                num10++;
                            }
                            else if (args.Weapon3.IsUnknown)
                            {
                                num10++;
                            }
                            design.LoadParts(args);
                        }
                        else
                        {
                            type7 = modProgram.ObjectData.FindOrCreateUnitType(iNIDroids.Droids[num].Template, clsUnitType.enumType.PlayerDroid, -1);
                            if (type7 == null)
                            {
                                design = null;
                            }
                            else if (type7.Type == clsUnitType.enumType.PlayerDroid)
                            {
                                design = (clsDroidDesign) type7;
                            }
                            else
                            {
                                design = null;
                            }
                        }
                        if (design == null)
                        {
                            num5++;
                        }
                        else
                        {
                            unit3 = new clsUnit {
                                Type = design
                            };
                            if (iNIDroids.Droids[num].UnitGroup == null)
                            {
                                unit3.UnitGroup = this.ScavengerUnitGroup;
                            }
                            else
                            {
                                unit3.UnitGroup = iNIDroids.Droids[num].UnitGroup;
                            }
                            unit3.Pos = iNIDroids.Droids[num].Pos.WorldPos;
                            unit3.Rotation = (int) Math.Round((double) ((iNIDroids.Droids[num].Rotation.Direction * 360.0) / 65536.0));
                            if (unit3.Rotation == 360)
                            {
                                unit3.Rotation = 0;
                            }
                            if (iNIDroids.Droids[num].HealthPercent >= 0)
                            {
                                unit3.Health = modMath.Clamp_dbl(((double) iNIDroids.Droids[num].HealthPercent) / 100.0, 0.01, 1.0);
                            }
                            if (iNIDroids.Droids[num].ID == 0)
                            {
                                iNIDroids.Droids[num].ID = num2;
                                modProgram.ZeroIDWarning(unit3, iNIDroids.Droids[num].ID, output);
                            }
                            add.NewUnit = unit3;
                            add.ID = iNIDroids.Droids[num].ID;
                            add.Perform();
                            modProgram.ErrorIDChange(iNIDroids.Droids[num].ID, unit3, "Load_WZ->INIDroids");
                            if (num2 == iNIDroids.Droids[num].ID)
                            {
                                num2 = unit3.ID + 1;
                            }
                        }
                    }
                }
                if (num4 > 0)
                {
                    output.WarningAdd(Conversions.ToString(num4) + " droids had an invalid position and were removed.");
                }
                if (num11 > 0)
                {
                    output.WarningAdd(Conversions.ToString(num11) + " droid designs had an unrecognised droidType and were removed.");
                }
                if (num10 > 0)
                {
                    output.WarningAdd(Conversions.ToString(num10) + " droid designs had components that are not loaded.");
                }
            }
            if (num5 > 0)
            {
                output.WarningAdd("Object Create Error.");
            }
            return output;
        }

        public virtual void Deallocate()
        {
            this.CancelUserInput();
            this.MakeMinimapTimer.Enabled = false;
            this.MakeMinimapTimer.Dispose();
            this.MakeMinimapTimer = null;
            this.frmMainLink.Deallocate();
            this.frmMainLink = null;
            this.UnitGroups.Deallocate();
            this.UnitGroups = null;
            while (this.Units.Count > 0)
            {
                this.Units[0].Deallocate();
            }
            this.Units.Deallocate();
            this.Units = null;
            while (this.Gateways.Count > 0)
            {
                this.Gateways[0].Deallocate();
            }
            this.Gateways.Deallocate();
            this.Gateways = null;
            while (this.ScriptPositions.Count > 0)
            {
                this.ScriptPositions[0].Deallocate();
            }
            this.ScriptPositions.Deallocate();
            this.ScriptPositions = null;
            while (this.ScriptAreas.Count > 0)
            {
                this.ScriptAreas[0].Deallocate();
            }
            this.ScriptAreas.Deallocate();
            this.ScriptAreas = null;
        }

        private void DebugGLError(string Name)
        {
            if ((modProgram.Debug_GL && (this.Messages.Count < 8)) && (GL.GetError() != ErrorCode.NoError))
            {
                clsMessage newItem = new clsMessage {
                    Text = "OpenGL Error (" + Name + ")"
                };
                this.Messages.Add(newItem);
            }
        }

        public void DrawTileOrientation(modMath.sXY_int Tile)
        {
            modMath.sXY_int _int;
            TileOrientation.sTileOrientation orientation = this.Terrain.Tiles[Tile.X, Tile.Y].Texture.Orientation;
            _int.X = 0x20;
            _int.Y = 0x20;
            modProgram.sWorldPos tileOffsetRotatedWorldPos = this.GetTileOffsetRotatedWorldPos(Tile, _int);
            _int.X = 0x40;
            _int.Y = 0x20;
            modProgram.sWorldPos pos2 = this.GetTileOffsetRotatedWorldPos(Tile, _int);
            _int.X = 0x40;
            _int.Y = 0x40;
            modProgram.sWorldPos pos3 = this.GetTileOffsetRotatedWorldPos(Tile, _int);
            GL.Vertex3(tileOffsetRotatedWorldPos.Horizontal.X, tileOffsetRotatedWorldPos.Altitude, tileOffsetRotatedWorldPos.Horizontal.Y);
            GL.Vertex3(pos2.Horizontal.X, pos2.Altitude, pos2.Horizontal.Y);
            GL.Vertex3(pos3.Horizontal.X, pos3.Altitude, pos3.Horizontal.Y);
        }

        public void DrawTileWireframe(int TileX, int TileY)
        {
            modMath.sXYZ_sng _sng;
            modMath.sXYZ_sng _sng2;
            modMath.sXYZ_sng _sng3;
            modMath.sXYZ_sng _sng4;
            double[] numArray = new double[] { (double) this.Terrain.Vertices[TileX, TileY].Height, (double) this.Terrain.Vertices[TileX + 1, TileY].Height, (double) this.Terrain.Vertices[TileX, TileY + 1].Height, (double) this.Terrain.Vertices[TileX + 1, TileY + 1].Height };
            _sng.X = TileX * 0x80;
            _sng.Y = (float) (numArray[0] * this.HeightMultiplier);
            _sng.Z = (0 - TileY) * 0x80;
            _sng2.X = (TileX + 1) * 0x80;
            _sng2.Y = (float) (numArray[1] * this.HeightMultiplier);
            _sng2.Z = (0 - TileY) * 0x80;
            _sng3.X = TileX * 0x80;
            _sng3.Y = (float) (numArray[2] * this.HeightMultiplier);
            _sng3.Z = (0 - (TileY + 1)) * 0x80;
            _sng4.X = (TileX + 1) * 0x80;
            _sng4.Y = (float) (numArray[3] * this.HeightMultiplier);
            _sng4.Z = (0 - (TileY + 1)) * 0x80;
            GL.Begin(BeginMode.Lines);
            if (this.Terrain.Tiles[TileX, TileY].Tri)
            {
                GL.Vertex3(_sng.X, _sng.Y, -_sng.Z);
                GL.Vertex3(_sng3.X, _sng3.Y, -_sng3.Z);
                GL.Vertex3(_sng3.X, _sng3.Y, -_sng3.Z);
                GL.Vertex3(_sng2.X, _sng2.Y, -_sng2.Z);
                GL.Vertex3(_sng2.X, _sng2.Y, -_sng2.Z);
                GL.Vertex3(_sng.X, _sng.Y, -_sng.Z);
                GL.Vertex3(_sng2.X, _sng2.Y, -_sng2.Z);
                GL.Vertex3(_sng3.X, _sng3.Y, -_sng3.Z);
                GL.Vertex3(_sng3.X, _sng3.Y, -_sng3.Z);
                GL.Vertex3(_sng4.X, _sng4.Y, -_sng4.Z);
                GL.Vertex3(_sng4.X, _sng4.Y, -_sng4.Z);
                GL.Vertex3(_sng2.X, _sng2.Y, -_sng2.Z);
            }
            else
            {
                GL.Vertex3(_sng.X, _sng.Y, -_sng.Z);
                GL.Vertex3(_sng3.X, _sng3.Y, -_sng3.Z);
                GL.Vertex3(_sng3.X, _sng3.Y, -_sng3.Z);
                GL.Vertex3(_sng4.X, _sng4.Y, -_sng4.Z);
                GL.Vertex3(_sng4.X, _sng4.Y, -_sng4.Z);
                GL.Vertex3(_sng.X, _sng.Y, -_sng.Z);
                GL.Vertex3(_sng.X, _sng.Y, -_sng.Z);
                GL.Vertex3(_sng4.X, _sng4.Y, -_sng4.Z);
                GL.Vertex3(_sng4.X, _sng4.Y, -_sng4.Z);
                GL.Vertex3(_sng2.X, _sng2.Y, -_sng2.Z);
                GL.Vertex3(_sng2.X, _sng2.Y, -_sng2.Z);
                GL.Vertex3(_sng.X, _sng.Y, -_sng.Z);
            }
            GL.End();
        }

        public void DrawUnitRectangle(clsUnit Unit, int BorderInsideThickness, sRGBA_sng InsideColour, sRGBA_sng OutsideColour)
        {
            modMath.sXY_int _int;
            modMath.sXY_int _int2;
            int y = Unit.Pos.Altitude - this.ViewInfo.ViewPos.Y;
            this.GetFootprintTileRangeClamped(Unit.Pos.Horizontal, Unit.Type.get_GetFootprintSelected(Unit.Rotation), ref _int, ref _int2);
            int num = _int.Y;
            _int.X = (int) Math.Round((double) (((_int.X + 0.125) * 128.0) - this.ViewInfo.ViewPos.X));
            _int.Y = (int) Math.Round((double) (((_int2.Y + 0.875) * -128.0) - this.ViewInfo.ViewPos.Z));
            _int2.X = (int) Math.Round((double) (((_int2.X + 0.875) * 128.0) - this.ViewInfo.ViewPos.X));
            _int2.Y = (int) Math.Round((double) (((num + 0.125) * -128.0) - this.ViewInfo.ViewPos.Z));
            GL.Color4(OutsideColour.Red, OutsideColour.Green, OutsideColour.Blue, OutsideColour.Alpha);
            GL.Vertex3(_int2.X, y, 0 - _int.Y);
            GL.Vertex3(_int.X, y, 0 - _int.Y);
            GL.Color4(InsideColour.Red, InsideColour.Green, InsideColour.Blue, InsideColour.Alpha);
            GL.Vertex3(_int.X + BorderInsideThickness, y, 0 - (_int.Y + BorderInsideThickness));
            GL.Vertex3(_int2.X - BorderInsideThickness, y, 0 - (_int.Y + BorderInsideThickness));
            GL.Color4(OutsideColour.Red, OutsideColour.Green, OutsideColour.Blue, OutsideColour.Alpha);
            GL.Vertex3(_int.X, y, 0 - _int.Y);
            GL.Vertex3(_int.X, y, 0 - _int2.Y);
            GL.Color4(InsideColour.Red, InsideColour.Green, InsideColour.Blue, InsideColour.Alpha);
            GL.Vertex3(_int.X + BorderInsideThickness, y, 0 - (_int2.Y - BorderInsideThickness));
            GL.Vertex3(_int.X + BorderInsideThickness, y, 0 - (_int.Y + BorderInsideThickness));
            GL.Color4(OutsideColour.Red, OutsideColour.Green, OutsideColour.Blue, OutsideColour.Alpha);
            GL.Vertex3(_int2.X, y, 0 - _int2.Y);
            GL.Vertex3(_int2.X, y, 0 - _int.Y);
            GL.Color4(InsideColour.Red, InsideColour.Green, InsideColour.Blue, InsideColour.Alpha);
            GL.Vertex3(_int2.X - BorderInsideThickness, y, 0 - (_int.Y + BorderInsideThickness));
            GL.Vertex3(_int2.X - BorderInsideThickness, y, 0 - (_int2.Y - BorderInsideThickness));
            GL.Color4(OutsideColour.Red, OutsideColour.Green, OutsideColour.Blue, OutsideColour.Alpha);
            GL.Vertex3(_int.X, y, 0 - _int2.Y);
            GL.Vertex3(_int2.X, y, 0 - _int2.Y);
            GL.Color4(InsideColour.Red, InsideColour.Green, InsideColour.Blue, InsideColour.Alpha);
            GL.Vertex3(_int2.X - BorderInsideThickness, y, 0 - (_int2.Y - BorderInsideThickness));
            GL.Vertex3(_int.X + BorderInsideThickness, y, 0 - (_int2.Y - BorderInsideThickness));
        }

        public clsGateway GatewayCreate(modMath.sXY_int PosA, modMath.sXY_int PosB)
        {
            if (((((((((PosA.X >= 0) & (PosA.X < this.Terrain.TileSize.X)) & (PosA.Y >= 0)) & (PosA.Y < this.Terrain.TileSize.Y)) & (PosB.X >= 0)) & (PosB.X < this.Terrain.TileSize.X)) & (PosB.Y >= 0)) & (PosB.Y < this.Terrain.TileSize.Y)) && ((PosA.X == PosB.X) | (PosA.Y == PosB.Y)))
            {
                clsGateway gateway2 = new clsGateway {
                    PosA = PosA,
                    PosB = PosB
                };
                gateway2.MapLink.Connect(this.Gateways);
                return gateway2;
            }
            return null;
        }

        public clsGateway GatewayCreateStoreChange(modMath.sXY_int PosA, modMath.sXY_int PosB)
        {
            clsGateway gateway = this.GatewayCreate(PosA, PosB);
            clsGatewayChange newItem = new clsGatewayChange {
                Type = clsGatewayChange.enumType.Added,
                Gateway = gateway
            };
            this.GatewayChanges.Add(newItem);
            return gateway;
        }

        public void GatewayRemoveStoreChange(int Num)
        {
            clsGatewayChange newItem = new clsGatewayChange {
                Type = clsGatewayChange.enumType.Deleted,
                Gateway = this.Gateways[Num]
            };
            this.GatewayChanges.Add(newItem);
            this.Gateways[Num].MapLink.Disconnect();
        }

        public void GenerateMasterTerrain(ref sGenerateMasterTerrainArgs Args)
        {
            int num9;
            clsBooleanMap remove = new clsBooleanMap();
            clsBooleanMap[] mapArray = new clsBooleanMap[(Args.LayerCount - 1) + 1];
            clsBooleanMap map2 = new clsBooleanMap();
            clsHeightmap source = new clsHeightmap();
            clsHeightmap heightmap2 = new clsHeightmap();
            double num3 = Math.Atan(510.0 / ((2.0 * (Args.LevelCount - 1.0)) * 128.0)) - 0.017453292519943295;
            this.Tileset = Args.Tileset.Tileset;
            int num11 = this.Terrain.TileSize.Y - 1;
            int num10 = 0;
            while (num10 <= num11)
            {
                int num12 = this.Terrain.TileSize.X - 1;
                num9 = 0;
                while (num9 <= num12)
                {
                    bool flag;
                    double num5 = Math.Abs((double) (this.Terrain.Vertices[num9 + 1, num10 + 1].Height - this.Terrain.Vertices[num9, num10].Height));
                    double num6 = Math.Abs((double) (this.Terrain.Vertices[num9, num10 + 1].Height - this.Terrain.Vertices[num9 + 1, num10].Height));
                    if (num5 == num6)
                    {
                        if (App.Random.Next() >= 0.5f)
                        {
                            flag = false;
                        }
                        else
                        {
                            flag = true;
                        }
                    }
                    else if (num5 < num6)
                    {
                        flag = false;
                    }
                    else
                    {
                        flag = true;
                    }
                    if (this.Terrain.Tiles[num9, num10].Tri != flag)
                    {
                        this.Terrain.Tiles[num9, num10].Tri = flag;
                    }
                    num9++;
                }
                num10++;
            }
            int num13 = Args.LayerCount - 1;
            int index = 0;
            while (index <= num13)
            {
                Args.Layers[index].Terrainmap = new clsBooleanMap();
                if (Args.Layers[index].TerrainmapDensity == 1f)
                {
                    Args.Layers[index].Terrainmap.ValueData.Value = new bool[(this.Terrain.TileSize.Y - 1) + 1, (this.Terrain.TileSize.X - 1) + 1];
                    Args.Layers[index].Terrainmap.ValueData.Size = this.Terrain.TileSize;
                    int num14 = this.Terrain.TileSize.Y - 1;
                    for (num10 = 0; num10 <= num14; num10++)
                    {
                        int num15 = this.Terrain.TileSize.X - 1;
                        num9 = 0;
                        while (num9 <= num15)
                        {
                            Args.Layers[index].Terrainmap.ValueData.Value[num10, num9] = true;
                            num9++;
                        }
                    }
                }
                else
                {
                    source.GenerateNewOfSize(this.Terrain.TileSize.Y, this.Terrain.TileSize.X, Args.Layers[index].TerrainmapScale, 1.0);
                    heightmap2.Rescale(source, 0.0, 1.0);
                    Args.Layers[index].Terrainmap.Convert_Heightmap(heightmap2, (long) Math.Round((double) (((double) (1f - Args.Layers[index].TerrainmapDensity)) / heightmap2.HeightScale)));
                }
                index++;
            }
            int[,] numArray2 = new int[(this.Terrain.TileSize.X - 1) + 1, (this.Terrain.TileSize.Y - 1) + 1];
            float[,] numArray = new float[(this.Terrain.TileSize.X - 1) + 1, (this.Terrain.TileSize.Y - 1) + 1];
            int num16 = this.Terrain.TileSize.Y - 1;
            num10 = 0;
            while (num10 <= num16)
            {
                int num17 = this.Terrain.TileSize.X - 1;
                num9 = 0;
                while (num9 <= num17)
                {
                    modMath.sXY_int _int;
                    double num2 = 0.0;
                    _int.X = (int) Math.Round((double) ((num9 + 0.25) * 128.0));
                    _int.Y = (int) Math.Round((double) ((num10 + 0.25) * 128.0));
                    double terrainSlopeAngle = this.GetTerrainSlopeAngle(_int);
                    if (terrainSlopeAngle > num2)
                    {
                        num2 = terrainSlopeAngle;
                    }
                    _int.X = (int) Math.Round((double) ((num9 + 0.75) * 128.0));
                    _int.Y = (int) Math.Round((double) ((num10 + 0.25) * 128.0));
                    terrainSlopeAngle = this.GetTerrainSlopeAngle(_int);
                    if (terrainSlopeAngle > num2)
                    {
                        num2 = terrainSlopeAngle;
                    }
                    _int.X = (int) Math.Round((double) ((num9 + 0.25) * 128.0));
                    _int.Y = (int) Math.Round((double) ((num10 + 0.75) * 128.0));
                    terrainSlopeAngle = this.GetTerrainSlopeAngle(_int);
                    if (terrainSlopeAngle > num2)
                    {
                        num2 = terrainSlopeAngle;
                    }
                    _int.X = (int) Math.Round((double) ((num9 + 0.75) * 128.0));
                    _int.Y = (int) Math.Round((double) ((num10 + 0.75) * 128.0));
                    terrainSlopeAngle = this.GetTerrainSlopeAngle(_int);
                    if (terrainSlopeAngle > num2)
                    {
                        num2 = terrainSlopeAngle;
                    }
                    numArray[num9, num10] = (float) num2;
                    num9++;
                }
                num10++;
            }
            int num18 = Args.LayerCount - 1;
            for (int i = 0; i <= num18; i++)
            {
                int tileNum = Args.Layers[i].TileNum;
                if (tileNum >= 0)
                {
                    mapArray[i] = new clsBooleanMap();
                    mapArray[i].Copy(Args.Layers[i].Terrainmap);
                    if ((Args.Layers[i].WithinLayer >= 0) && (Args.Layers[i].WithinLayer < i))
                    {
                        remove.Within(mapArray[i], mapArray[Args.Layers[i].WithinLayer]);
                        mapArray[i].ValueData = remove.ValueData;
                        remove.ValueData = new clsBooleanMap.clsValueData();
                    }
                    int num19 = i - 1;
                    for (index = 0; index <= num19; index++)
                    {
                        if (Args.Layers[i].AvoidLayers[index])
                        {
                            remove.Expand_One_Tile(mapArray[index]);
                            map2.Remove(mapArray[i], remove);
                            mapArray[i].ValueData = map2.ValueData;
                            map2.ValueData = new clsBooleanMap.clsValueData();
                        }
                    }
                    int num20 = this.Terrain.TileSize.Y - 1;
                    num10 = 0;
                    while (num10 <= num20)
                    {
                        int num21 = this.Terrain.TileSize.X - 1;
                        num9 = 0;
                        while (num9 <= num21)
                        {
                            if (mapArray[i].ValueData.Value[num10, num9])
                            {
                                if ((this.Terrain.Vertices[num9, num10].Height < Args.Layers[i].HeightMin) | (this.Terrain.Vertices[num9, num10].Height > Args.Layers[i].HeightMax))
                                {
                                    mapArray[i].ValueData.Value[num10, num9] = false;
                                }
                                if ((Args.Layers[i].IsCliff && mapArray[i].ValueData.Value[num10, num9]) && (numArray[num9, num10] < num3))
                                {
                                    mapArray[i].ValueData.Value[num10, num9] = false;
                                }
                            }
                            num9++;
                        }
                        num10++;
                    }
                    int num22 = this.Terrain.TileSize.Y - 1;
                    num10 = 0;
                    while (num10 <= num22)
                    {
                        int num23 = this.Terrain.TileSize.X - 1;
                        num9 = 0;
                        while (num9 <= num23)
                        {
                            if (mapArray[i].ValueData.Value[num10, num9])
                            {
                                numArray2[num9, num10] = tileNum;
                            }
                            num9++;
                        }
                        num10++;
                    }
                }
            }
            int num24 = this.Terrain.TileSize.Y - 1;
            for (num10 = 0; num10 <= num24; num10++)
            {
                int num25 = this.Terrain.TileSize.X - 1;
                num9 = 0;
                while (num9 <= num25)
                {
                    if (Args.Watermap.ValueData.Value[num10, num9] && (numArray[num9, num10] < num3))
                    {
                        numArray2[num9, num10] = 0x11;
                    }
                    num9++;
                }
            }
            int num26 = this.Terrain.TileSize.Y - 1;
            num10 = 0;
            while (num10 <= num26)
            {
                num9 = 0;
                do
                {
                    numArray2[num9, num10] = Args.Tileset.BorderTextureNum;
                    num9++;
                }
                while (num9 <= 2);
                int num27 = this.Terrain.TileSize.X - 1;
                for (num9 = this.Terrain.TileSize.X - 4; num9 <= num27; num9++)
                {
                    numArray2[num9, num10] = Args.Tileset.BorderTextureNum;
                }
                num10++;
            }
            int num28 = this.Terrain.TileSize.X - 5;
            num9 = 3;
            while (num9 <= num28)
            {
                num10 = 0;
                do
                {
                    numArray2[num9, num10] = Args.Tileset.BorderTextureNum;
                    num10++;
                }
                while (num10 <= 2);
                int num29 = this.Terrain.TileSize.Y - 1;
                num10 = this.Terrain.TileSize.Y - 4;
                while (num10 <= num29)
                {
                    numArray2[num9, num10] = Args.Tileset.BorderTextureNum;
                    num10++;
                }
                num9++;
            }
            int num30 = this.Terrain.TileSize.Y - 1;
            for (num10 = 0; num10 <= num30; num10++)
            {
                int num31 = this.Terrain.TileSize.X - 1;
                for (num9 = 0; num9 <= num31; num9++)
                {
                    this.Terrain.Tiles[num9, num10].Texture.TextureNum = numArray2[num9, num10];
                }
            }
        }

        public clsBooleanMap GenerateTerrainMap(float Scale, float Density)
        {
            clsHeightmap source = new clsHeightmap();
            clsHeightmap heightmap2 = new clsHeightmap();
            source.GenerateNewOfSize(this.Terrain.TileSize.Y + 1, this.Terrain.TileSize.X + 1, Scale, 1.0);
            heightmap2.Rescale(source, 0.0, 1.0);
            clsBooleanMap map2 = new clsBooleanMap();
            map2.Convert_Heightmap(heightmap2, (long) Math.Round((double) ((1.0 - Density) / heightmap2.HeightScale)));
            return map2;
        }

        public uint GetAvailableID()
        {
            IEnumerator enumerator;
            uint num2 = 1;
            try
            {
                enumerator = this.Units.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    clsUnit current = (clsUnit) enumerator.Current;
                    if (current.ID >= num2)
                    {
                        num2 = current.ID + 1;
                    }
                }
            }
            finally
            {
                if (enumerator is IDisposable)
                {
                    (enumerator as IDisposable).Dispose();
                }
            }
            return num2;
        }

        public string GetDefaultScriptLabel(string Prefix)
        {
            int num = 1;
            do
            {
                string text = Prefix + modIO.InvariantToString_int(num);
                if (this.ScriptLabelIsValid(text).Success)
                {
                    return text;
                }
                num++;
            }
            while (num < 0x4000);
            Interaction.MsgBox("Error: Unable to set default script label.", MsgBoxStyle.ApplicationModal, null);
            return "";
        }

        public string GetDirectory()
        {
            if (this.PathInfo == null)
            {
                return MyProject.Computer.FileSystem.SpecialDirectories.MyDocuments;
            }
            modProgram.sSplitPath path = new modProgram.sSplitPath(this.PathInfo.Path);
            return path.FilePath;
        }

        public void GetFootprintTileRange(modMath.sXY_int Horizontal, modMath.sXY_int Footprint, ref modMath.sXY_int ResultStart, ref modMath.sXY_int ResultFinish)
        {
            int num2;
            modMath.sXY_int posTileNum = this.GetPosTileNum(Horizontal);
            int num = Math.DivRem(Footprint.X, 2, out num2);
            ResultStart.X = posTileNum.X - num;
            ResultFinish.X = (ResultStart.X + Footprint.X) - 1;
            num = Math.DivRem(Footprint.Y, 2, out num2);
            ResultStart.Y = posTileNum.Y - num;
            ResultFinish.Y = (ResultStart.Y + Footprint.Y) - 1;
        }

        public void GetFootprintTileRangeClamped(modMath.sXY_int Horizontal, modMath.sXY_int Footprint, ref modMath.sXY_int ResultStart, ref modMath.sXY_int ResultFinish)
        {
            int num2;
            modMath.sXY_int posTileNum = this.GetPosTileNum(Horizontal);
            int num = Math.DivRem(Footprint.X, 2, out num2);
            ResultStart.X = modMath.Clamp_int(posTileNum.X - num, 0, this.Terrain.TileSize.X - 1);
            ResultFinish.X = modMath.Clamp_int((ResultStart.X + Footprint.X) - 1, 0, this.Terrain.TileSize.X - 1);
            num = Math.DivRem(Footprint.Y, 2, out num2);
            ResultStart.Y = modMath.Clamp_int(posTileNum.Y - num, 0, this.Terrain.TileSize.Y - 1);
            ResultFinish.Y = modMath.Clamp_int((ResultStart.Y + Footprint.Y) - 1, 0, this.Terrain.TileSize.Y - 1);
        }

        public modMath.sXY_int GetPosSectorNum(modMath.sXY_int Horizontal)
        {
            return this.GetTileSectorNum(this.GetPosTileNum(Horizontal));
        }

        public modMath.sXY_int GetPosTileNum(modMath.sXY_int Horizontal)
        {
            modMath.sXY_int _int2;
            _int2.X = (int) Math.Round(((double) (((double) Horizontal.X) / 128.0)));
            _int2.Y = (int) Math.Round(((double) (((double) Horizontal.Y) / 128.0)));
            return _int2;
        }

        public modMath.sXY_int GetPosVertexNum(modMath.sXY_int Horizontal)
        {
            modMath.sXY_int _int2;
            _int2.X = (int) Math.Round(Math.Round((double) (((double) Horizontal.X) / 128.0)));
            _int2.Y = (int) Math.Round(Math.Round((double) (((double) Horizontal.Y) / 128.0)));
            return _int2;
        }

        public modMath.sXY_int GetSectorNumClamped(modMath.sXY_int SectorNum)
        {
            modMath.sXY_int _int2;
            _int2.X = modMath.Clamp_int(SectorNum.X, 0, this.SectorCount.X - 1);
            _int2.Y = modMath.Clamp_int(SectorNum.Y, 0, this.SectorCount.Y - 1);
            return _int2;
        }

        public double GetTerrainHeight(modMath.sXY_int Horizontal)
        {
            double num2;
            double num3;
            double height;
            double num7;
            double num8;
            int amount = (int) Math.Round(((double) (((double) Horizontal.X) / 128.0)));
            int num14 = (int) Math.Round(((double) (((double) Horizontal.Y) / 128.0)));
            double num4 = modMath.Clamp_dbl((((double) Horizontal.X) / 128.0) - amount, 0.0, 1.0);
            double num5 = modMath.Clamp_dbl((((double) Horizontal.Y) / 128.0) - num14, 0.0, 1.0);
            int num9 = modMath.Clamp_int(amount, 0, this.Terrain.TileSize.X - 1);
            int num12 = modMath.Clamp_int(num14, 0, this.Terrain.TileSize.Y - 1);
            int num10 = modMath.Clamp_int(amount + 1, 0, this.Terrain.TileSize.X);
            int num13 = modMath.Clamp_int(num14 + 1, 0, this.Terrain.TileSize.Y);
            if (this.Terrain.Tiles[num9, num12].Tri)
            {
                if (num5 <= (1.0 - num4))
                {
                    height = this.Terrain.Vertices[num9, num12].Height;
                    num2 = this.Terrain.Vertices[num10, num12].Height - height;
                    num3 = this.Terrain.Vertices[num9, num13].Height - height;
                    num7 = num4;
                    num8 = num5;
                }
                else
                {
                    height = this.Terrain.Vertices[num10, num13].Height;
                    num2 = this.Terrain.Vertices[num9, num13].Height - height;
                    num3 = this.Terrain.Vertices[num10, num12].Height - height;
                    num7 = 1.0 - num4;
                    num8 = 1.0 - num5;
                }
            }
            else if (num5 <= num4)
            {
                height = this.Terrain.Vertices[num10, num12].Height;
                num2 = this.Terrain.Vertices[num9, num12].Height - height;
                num3 = this.Terrain.Vertices[num10, num13].Height - height;
                num7 = 1.0 - num4;
                num8 = num5;
            }
            else
            {
                height = this.Terrain.Vertices[num9, num13].Height;
                num2 = this.Terrain.Vertices[num10, num13].Height - height;
                num3 = this.Terrain.Vertices[num9, num12].Height - height;
                num7 = num4;
                num8 = 1.0 - num5;
            }
            return (((height + (num2 * num7)) + (num3 * num8)) * this.HeightMultiplier);
        }

        public double GetTerrainSlopeAngle(modMath.sXY_int Horizontal)
        {
            double num2;
            double num3;
            double height;
            Position.XYZ_dbl _dbl;
            Position.XYZ_dbl _dbl2;
            Position.XYZ_dbl _dbl3;
            int amount = (int) Math.Round(((double) (((double) Horizontal.X) / 128.0)));
            int num12 = (int) Math.Round(((double) (((double) Horizontal.Y) / 128.0)));
            double num4 = modMath.Clamp_dbl((((double) Horizontal.X) / 128.0) - amount, 0.0, 1.0);
            double num5 = modMath.Clamp_dbl((((double) Horizontal.Y) / 128.0) - num12, 0.0, 1.0);
            int num7 = modMath.Clamp_int(amount, 0, this.Terrain.TileSize.X - 1);
            int num10 = modMath.Clamp_int(num12, 0, this.Terrain.TileSize.Y - 1);
            int num8 = modMath.Clamp_int(amount + 1, 0, this.Terrain.TileSize.X);
            int num11 = modMath.Clamp_int(num12 + 1, 0, this.Terrain.TileSize.Y);
            if (this.Terrain.Tiles[num7, num10].Tri)
            {
                if (num5 <= (1.0 - num4))
                {
                    height = this.Terrain.Vertices[num7, num10].Height;
                    num2 = this.Terrain.Vertices[num8, num10].Height - height;
                    num3 = this.Terrain.Vertices[num7, num11].Height - height;
                }
                else
                {
                    height = this.Terrain.Vertices[num8, num11].Height;
                    num2 = this.Terrain.Vertices[num7, num11].Height - height;
                    num3 = this.Terrain.Vertices[num8, num10].Height - height;
                }
            }
            else if (num5 <= num4)
            {
                height = this.Terrain.Vertices[num8, num10].Height;
                num2 = this.Terrain.Vertices[num7, num10].Height - height;
                num3 = this.Terrain.Vertices[num8, num11].Height - height;
            }
            else
            {
                height = this.Terrain.Vertices[num7, num11].Height;
                num2 = this.Terrain.Vertices[num8, num11].Height - height;
                num3 = this.Terrain.Vertices[num7, num10].Height - height;
            }
            _dbl.X = 128.0;
            _dbl.Y = num2 * this.HeightMultiplier;
            _dbl.Z = 0.0;
            _dbl2.X = 0.0;
            _dbl2.Y = num3 * this.HeightMultiplier;
            _dbl2.Z = 128.0;
            Matrix3DMath.VectorCrossProduct(_dbl, _dbl2, ref _dbl3);
            if (!(_dbl3.X == 0.0) | !(_dbl3.Z == 0.0))
            {
                Angles.AnglePY epy;
                Matrix3DMath.VectorToPY(_dbl3, ref epy);
                return (1.5707963267948966 - Math.Abs(epy.Pitch));
            }
            return 0.0;
        }

        public bool GetTerrainTri(modMath.sXY_int Horizontal)
        {
            int amount = (int) Math.Round(((double) (((double) Horizontal.X) / 128.0)));
            int num6 = (int) Math.Round(((double) (((double) Horizontal.Y) / 128.0)));
            double num = modMath.Clamp_dbl((((double) Horizontal.X) / 128.0) - amount, 0.0, 1.0);
            double num2 = modMath.Clamp_dbl((((double) Horizontal.Y) / 128.0) - num6, 0.0, 1.0);
            int num3 = modMath.Clamp_int(amount, 0, this.Terrain.TileSize.X - 1);
            int num5 = modMath.Clamp_int(num6, 0, this.Terrain.TileSize.Y - 1);
            if (this.Terrain.Tiles[num3, num5].Tri)
            {
                if (num2 <= (1.0 - num))
                {
                    return false;
                }
                return true;
            }
            return (num2 <= num);
        }

        public modProgram.sWorldPos GetTileOffsetRotatedWorldPos(modMath.sXY_int Tile, modMath.sXY_int TileOffsetToRotate)
        {
            modProgram.sWorldPos pos2;
            modMath.sXY_int tileRotatedOffset = TileOrientation.GetTileRotatedOffset(this.Terrain.Tiles[Tile.X, Tile.Y].Texture.Orientation, TileOffsetToRotate);
            pos2.Horizontal.X = (Tile.X * 0x80) + tileRotatedOffset.X;
            pos2.Horizontal.Y = (Tile.Y * 0x80) + tileRotatedOffset.Y;
            pos2.Altitude = (int) Math.Round(this.GetTerrainHeight(pos2.Horizontal));
            return pos2;
        }

        public modMath.sXY_int GetTileSectorNum(modMath.sXY_int Tile)
        {
            modMath.sXY_int _int2;
            _int2.X = (int) Math.Round(((double) (((double) Tile.X) / 8.0)));
            _int2.Y = (int) Math.Round(((double) (((double) Tile.Y) / 8.0)));
            return _int2;
        }

        public void GetTileSectorRange(modMath.sXY_int StartTile, modMath.sXY_int FinishTile, ref modMath.sXY_int ResultSectorStart, ref modMath.sXY_int ResultSectorFinish)
        {
            ResultSectorStart = this.GetTileSectorNum(StartTile);
            ResultSectorFinish = this.GetTileSectorNum(FinishTile);
            ResultSectorStart.X = modMath.Clamp_int(ResultSectorStart.X, 0, this.SectorCount.X - 1);
            ResultSectorStart.Y = modMath.Clamp_int(ResultSectorStart.Y, 0, this.SectorCount.Y - 1);
            ResultSectorFinish.X = modMath.Clamp_int(ResultSectorFinish.X, 0, this.SectorCount.X - 1);
            ResultSectorFinish.Y = modMath.Clamp_int(ResultSectorFinish.Y, 0, this.SectorCount.Y - 1);
        }

        public string GetTitle()
        {
            if (this.PathInfo == null)
            {
                return "Unsaved map";
            }
            modProgram.sSplitPath path = new modProgram.sSplitPath(this.PathInfo.Path);
            if (this.PathInfo.IsFMap)
            {
                return path.FileTitleWithoutExtension;
            }
            return path.FileTitle;
        }

        public sRGB_sng GetUnitGroupColour(clsUnitGroup ColourUnitGroup)
        {
            if (ColourUnitGroup.WZ_StartPos < 0)
            {
                return new sRGB_sng(1f, 1f, 1f);
            }
            return modProgram.PlayerColour[ColourUnitGroup.WZ_StartPos].Colour;
        }

        public sRGB_sng GetUnitGroupMinimapColour(clsUnitGroup ColourUnitGroup)
        {
            if (ColourUnitGroup.WZ_StartPos < 0)
            {
                return new sRGB_sng(1f, 1f, 1f);
            }
            return modProgram.PlayerColour[ColourUnitGroup.WZ_StartPos].MinimapColour;
        }

        public int GetVertexAltitude(modMath.sXY_int VertexNum)
        {
            return (this.Terrain.Vertices[VertexNum.X, VertexNum.Y].Height * this.HeightMultiplier);
        }

        public void GLDraw()
        {
            int num;
            int num2;
            sRGBA_sng _sng;
            sRGBA_sng _sng2;
            Position.XY_dbl _dbl;
            clsBrush.sPosNum num7;
            bool flag;
            modMath.sXY_int _int;
            modMath.sXY_int _int2;
            clsAction action;
            clsTextLabels labels;
            modMath.sXY_int _int4;
            sRGB_sng unitGroupColour;
            modMath.sXY_int _int5;
            bool flag2;
            modMath.sXY_int _int6;
            clsTextLabel label2;
            clsUnit unit;
            Position.XYZ_dbl _dbl6;
            Position.XYZ_dbl _dbl7;
            Position.XYZ_dbl _dbl8;
            Position.XYZ_dbl _dbl9;
            Position.XY_dbl _dbl10;
            Position.XY_dbl _dbl11;
            Position.XY_dbl _dbl12;
            Position.XY_dbl _dbl13;
            int x;
            Position.XYZ_dbl _dbl14;
            Position.XYZ_dbl _dbl15;
            int num11;
            clsScriptArea area;
            clsScriptPosition position;
            IEnumerator enumerator2;
            IEnumerator enumerator3;
            IEnumerator enumerator4;
            IEnumerator enumerator5;
            IEnumerator enumerator6;
            clsTextLabel label = new clsTextLabel();
            float[] @params = new float[4];
            Matrix3DMath.Matrix3D matrixd = new Matrix3DMath.Matrix3D();
            Matrix3DMath.Matrix3D resultMatrix = new Matrix3DMath.Matrix3D();
            ctrlMapView mapView = this.ViewInfo.MapView;
            modMath.sXY_int gLSize = this.ViewInfo.MapView.GLSize;
            double minimapSize = modSettings.Settings.MinimapSize;
            this.ViewInfo.Tiles_Per_Minimap_Pixel = Math.Sqrt((double) ((this.Terrain.TileSize.X * this.Terrain.TileSize.X) + (this.Terrain.TileSize.Y * this.Terrain.TileSize.Y))) / (1.4142135623730951 * minimapSize);
            if ((this.Minimap_Texture_Size > 0) & (this.ViewInfo.Tiles_Per_Minimap_Pixel > 0.0))
            {
                _int4.X = (int) Math.Round((double) (((double) this.Terrain.TileSize.X) / this.ViewInfo.Tiles_Per_Minimap_Pixel));
                _int4.Y = (int) Math.Round((double) (((double) this.Terrain.TileSize.Y) / this.ViewInfo.Tiles_Per_Minimap_Pixel));
            }
            modMath.sXY_int screenPos = new modMath.sXY_int((int) Math.Round((double) (((double) gLSize.X) / 2.0)), (int) Math.Round((double) (((double) gLSize.Y) / 2.0)));
            if (!this.ViewInfo.ScreenXY_Get_ViewPlanePos(screenPos, minimapSize, ref _dbl))
            {
                Matrix3DMath.VectorForwardsRotationByMatrix(this.ViewInfo.ViewAngleMatrix, ref _dbl14);
                double num6 = (modProgram.VisionRadius * 2.0) / Math.Sqrt((_dbl14.X * _dbl14.X) + (_dbl14.Z * _dbl14.Z));
                _dbl.X = this.ViewInfo.ViewPos.X + (_dbl14.X * num6);
                _dbl.Y = this.ViewInfo.ViewPos.Z + (_dbl14.Z * num6);
            }
            _dbl.X = modMath.Clamp_dbl(_dbl.X, 0.0, (this.Terrain.TileSize.X * 0x80) - 1.0);
            _dbl.Y = modMath.Clamp_dbl(-_dbl.Y, 0.0, (this.Terrain.TileSize.Y * 0x80) - 1.0);
            screenPos = new modMath.sXY_int((int) Math.Round(_dbl.X), (int) Math.Round(_dbl.Y));
            num7.Normal = this.GetPosSectorNum(screenPos);
            screenPos = new modMath.sXY_int((int) Math.Round((double) (_dbl.X - 512.0)), (int) Math.Round((double) (_dbl.Y - 512.0)));
            num7.Alignment = this.GetPosSectorNum(screenPos);
            clsDrawSectorObjects tool = new clsDrawSectorObjects {
                Map = this,
                UnitTextLabels = new clsTextLabels(0x40)
            };
            tool.Start();
            _dbl14.X = _dbl.X - this.ViewInfo.ViewPos.X;
            _dbl14.Y = 0x80 - this.ViewInfo.ViewPos.Y;
            _dbl14.Z = -_dbl.Y - this.ViewInfo.ViewPos.Z;
            float magnitude = (float) _dbl14.GetMagnitude();
            GL.Enable(EnableCap.DepthTest);
            GL.MatrixMode(OpenTK.Graphics.OpenGL.MatrixMode.Projection);
            GL.LoadMatrix(ref Matrix4.CreatePerspectiveFieldOfView(this.ViewInfo.FieldOfViewY, mapView.GLSize_XPerY, magnitude / 128f, magnitude * 128f));
            GL.MatrixMode(OpenTK.Graphics.OpenGL.MatrixMode.Modelview);
            GL.LoadIdentity();
            Matrix3DMath.MatrixRotationByMatrix(this.ViewInfo.ViewAngleMatrix_Inverted, modProgram.SunAngleMatrix, resultMatrix);
            Matrix3DMath.VectorForwardsRotationByMatrix(resultMatrix, ref _dbl14);
            @params[0] = (float) _dbl14.X;
            @params[1] = (float) _dbl14.Y;
            @params[2] = (float) -_dbl14.Z;
            @params[3] = 0f;
            GL.Light(LightName.Light0, LightParameter.Position, @params);
            GL.Light(LightName.Light1, LightParameter.Position, @params);
            GL.Disable(EnableCap.Light0);
            GL.Disable(EnableCap.Light1);
            if (modProgram.Draw_Lighting != modProgram.enumDrawLighting.Off)
            {
                if (modProgram.Draw_Lighting == modProgram.enumDrawLighting.Half)
                {
                    GL.Enable(EnableCap.Light0);
                }
                else if (modProgram.Draw_Lighting == modProgram.enumDrawLighting.Normal)
                {
                    GL.Enable(EnableCap.Light1);
                }
                GL.Enable(EnableCap.Lighting);
            }
            else
            {
                GL.Disable(EnableCap.Lighting);
            }
            minimapSize = 127.5 * this.HeightMultiplier;
            if (((this.ViewInfo.ScreenXY_Get_ViewPlanePos_ForwardDownOnly(0, 0, minimapSize, ref _dbl10) & this.ViewInfo.ScreenXY_Get_ViewPlanePos_ForwardDownOnly(gLSize.X, 0, minimapSize, ref _dbl11)) & this.ViewInfo.ScreenXY_Get_ViewPlanePos_ForwardDownOnly(gLSize.X, gLSize.Y, minimapSize, ref _dbl12)) & this.ViewInfo.ScreenXY_Get_ViewPlanePos_ForwardDownOnly(0, gLSize.Y, minimapSize, ref _dbl13))
            {
                flag2 = true;
            }
            else
            {
                flag2 = false;
            }
            GL.Rotate((double) (this.ViewInfo.ViewAngleRPY.Roll / 0.017453292519943295), (double) 0.0, (double) 0.0, (double) -1.0);
            GL.Rotate((double) (this.ViewInfo.ViewAngleRPY.Pitch / 0.017453292519943295), (double) 1.0, (double) 0.0, (double) 0.0);
            GL.Rotate((double) (this.ViewInfo.ViewAngleRPY.Yaw / 0.017453292519943295), (double) 0.0, (double) 1.0, (double) 0.0);
            GL.Translate((float) (0 - this.ViewInfo.ViewPos.X), (float) (0 - this.ViewInfo.ViewPos.Y), (float) this.ViewInfo.ViewPos.Z);
            GL.Enable(EnableCap.CullFace);
            this.DebugGLError("Matrix modes");
            if (modProgram.Draw_TileTextures)
            {
                GL.Color3((float) 1f, (float) 1f, (float) 1f);
                GL.Enable(EnableCap.Texture2D);
                action = new clsDrawCallTerrain {
                    Map = this
                };
                modProgram.VisionSectors.PerformActionMapSectors(action, num7);
                GL.Disable(EnableCap.Texture2D);
                this.DebugGLError("Tile textures");
            }
            GL.Disable(EnableCap.DepthTest);
            GL.Disable(EnableCap.Lighting);
            if (modProgram.Draw_TileWireframe)
            {
                GL.Color3((float) 0f, (float) 1f, (float) 0f);
                GL.LineWidth(1f);
                clsDrawCallTerrainWireframe wireframe = new clsDrawCallTerrainWireframe {
                    Map = this
                };
                modProgram.VisionSectors.PerformActionMapSectors(wireframe, num7);
                this.DebugGLError("Wireframe");
            }
            if (modProgram.DisplayTileOrientation)
            {
                GL.Disable(EnableCap.CullFace);
                GL.Begin(BeginMode.Triangles);
                GL.Color3((float) 1f, (float) 1f, (float) 0f);
                action = new clsDrawTileOrientation {
                    Map = this
                };
                modProgram.VisionSectors.PerformActionMapSectors(action, num7);
                GL.End();
                GL.Enable(EnableCap.CullFace);
                this.DebugGLError("Tile orientation");
            }
            clsViewInfo.clsMouseOver.clsOverTerrain mouseOverTerrain = this.ViewInfo.GetMouseOverTerrain();
            if (modProgram.Draw_VertexTerrain)
            {
                GL.LineWidth(1f);
                clsDrawVertexTerrain terrain2 = new clsDrawVertexTerrain {
                    Map = this,
                    ViewAngleMatrix = this.ViewInfo.ViewAngleMatrix
                };
                modProgram.VisionSectors.PerformActionMapSectors(terrain2, num7);
                this.DebugGLError("Terrain type markers");
            }
            label.Text = "";
            if (this.Selected_Area_VertexA != null)
            {
                flag = false;
                if (this.Selected_Area_VertexB != null)
                {
                    modMath.ReorderXY(this.Selected_Area_VertexA.XY, this.Selected_Area_VertexB.XY, ref _int6, ref _int);
                    _dbl14.X = (this.Selected_Area_VertexB.X * 0x80) - this.ViewInfo.ViewPos.X;
                    _dbl14.Z = ((0 - this.Selected_Area_VertexB.Y) * 0x80) - this.ViewInfo.ViewPos.Z;
                    _dbl14.Y = this.GetVertexAltitude(this.Selected_Area_VertexB.XY) - this.ViewInfo.ViewPos.Y;
                    flag = true;
                }
                else if ((modTools.Tool == modTools.Tools.TerrainSelect) && (mouseOverTerrain != null))
                {
                    modMath.ReorderXY(this.Selected_Area_VertexA.XY, mouseOverTerrain.Vertex.Normal, ref _int6, ref _int);
                    _dbl14.X = (mouseOverTerrain.Vertex.Normal.X * 0x80) - this.ViewInfo.ViewPos.X;
                    _dbl14.Z = ((0 - mouseOverTerrain.Vertex.Normal.Y) * 0x80) - this.ViewInfo.ViewPos.Z;
                    _dbl14.Y = this.GetVertexAltitude(mouseOverTerrain.Vertex.Normal) - this.ViewInfo.ViewPos.Y;
                    flag = true;
                }
                if (flag)
                {
                    Matrix3DMath.VectorRotationByMatrix(this.ViewInfo.ViewAngleMatrix_Inverted, _dbl14, ref _dbl15);
                    if (this.ViewInfo.Pos_Get_Screen_XY(_dbl15, ref _int5) && ((((_int5.X >= 0) & (_int5.X <= gLSize.X)) & (_int5.Y >= 0)) & (_int5.Y <= gLSize.Y)))
                    {
                        label.Colour.Red = 1f;
                        label.Colour.Green = 1f;
                        label.Colour.Blue = 1f;
                        label.Colour.Alpha = 1f;
                        label.TextFont = modProgram.UnitLabelFont;
                        label.SizeY = modSettings.Settings.FontSize;
                        label.Pos = _int5;
                        label.Text = Conversions.ToString((int) (_int.X - _int6.X)) + "x" + Conversions.ToString((int) (_int.Y - _int6.Y));
                    }
                    GL.LineWidth(3f);
                    new clsDrawTileAreaOutline { Map = this, StartXY = _int6, FinishXY = _int, Colour = new sRGBA_sng(1f, 1f, 1f, 1f) }.ActionPerform();
                }
                this.DebugGLError("Terrain selection box");
            }
            if (modTools.Tool == modTools.Tools.TerrainSelect)
            {
                if (mouseOverTerrain != null)
                {
                    GL.LineWidth(3f);
                    _dbl6.X = mouseOverTerrain.Vertex.Normal.X * 0x80;
                    _dbl6.Y = this.Terrain.Vertices[mouseOverTerrain.Vertex.Normal.X, mouseOverTerrain.Vertex.Normal.Y].Height * this.HeightMultiplier;
                    _dbl6.Z = (0 - mouseOverTerrain.Vertex.Normal.Y) * 0x80;
                    GL.Begin(BeginMode.Lines);
                    GL.Color3((float) 1f, (float) 1f, (float) 1f);
                    GL.Vertex3(_dbl6.X - 8.0, _dbl6.Y, -_dbl6.Z);
                    GL.Vertex3(_dbl6.X + 8.0, _dbl6.Y, -_dbl6.Z);
                    GL.Vertex3(_dbl6.X, _dbl6.Y, -_dbl6.Z - 8.0);
                    GL.Vertex3(_dbl6.X, _dbl6.Y, -_dbl6.Z + 8.0);
                    GL.End();
                }
                this.DebugGLError("Terrain selection vertex");
            }
            if (modProgram.Draw_Gateways)
            {
                IEnumerator enumerator;
                GL.LineWidth(2f);
                try
                {
                    enumerator = this.Gateways.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        int y;
                        int num4;
                        clsGateway current = (clsGateway) enumerator.Current;
                        if (current.PosA.X == current.PosB.X)
                        {
                            if (current.PosA.Y <= current.PosB.Y)
                            {
                                y = current.PosA.Y;
                                num4 = current.PosB.Y;
                            }
                            else
                            {
                                y = current.PosB.Y;
                                num4 = current.PosA.Y;
                            }
                            x = current.PosA.X;
                            int num14 = num4;
                            num11 = y;
                            while (num11 <= num14)
                            {
                                _dbl6.X = x * 0x80;
                                _dbl6.Y = this.Terrain.Vertices[x, num11].Height * this.HeightMultiplier;
                                _dbl6.Z = (0 - num11) * 0x80;
                                _dbl7.X = (x + 1) * 0x80;
                                _dbl7.Y = this.Terrain.Vertices[x + 1, num11].Height * this.HeightMultiplier;
                                _dbl7.Z = (0 - num11) * 0x80;
                                _dbl8.X = x * 0x80;
                                _dbl8.Y = this.Terrain.Vertices[x, num11 + 1].Height * this.HeightMultiplier;
                                _dbl8.Z = (0 - (num11 + 1)) * 0x80;
                                _dbl9.X = (x + 1) * 0x80;
                                _dbl9.Y = this.Terrain.Vertices[x + 1, num11 + 1].Height * this.HeightMultiplier;
                                _dbl9.Z = (0 - (num11 + 1)) * 0x80;
                                GL.Begin(BeginMode.LineLoop);
                                GL.Color3((float) 0.75f, (float) 1f, (float) 0f);
                                GL.Vertex3(_dbl6.X, _dbl6.Y, -_dbl6.Z);
                                GL.Vertex3(_dbl7.X, _dbl7.Y, -_dbl7.Z);
                                GL.Vertex3(_dbl9.X, _dbl9.Y, -_dbl9.Z);
                                GL.Vertex3(_dbl8.X, _dbl8.Y, -_dbl8.Z);
                                GL.End();
                                num11++;
                            }
                        }
                        else
                        {
                            if (current.PosA.Y == current.PosB.Y)
                            {
                                if (current.PosA.X <= current.PosB.X)
                                {
                                    y = current.PosA.X;
                                    num4 = current.PosB.X;
                                }
                                else
                                {
                                    y = current.PosB.X;
                                    num4 = current.PosA.X;
                                }
                                num11 = current.PosA.Y;
                                int num15 = num4;
                                x = y;
                                while (x <= num15)
                                {
                                    _dbl6.X = x * 0x80;
                                    _dbl6.Y = this.Terrain.Vertices[x, num11].Height * this.HeightMultiplier;
                                    _dbl6.Z = (0 - num11) * 0x80;
                                    _dbl7.X = (x + 1) * 0x80;
                                    _dbl7.Y = this.Terrain.Vertices[x + 1, num11].Height * this.HeightMultiplier;
                                    _dbl7.Z = (0 - num11) * 0x80;
                                    _dbl8.X = x * 0x80;
                                    _dbl8.Y = this.Terrain.Vertices[x, num11 + 1].Height * this.HeightMultiplier;
                                    _dbl8.Z = (0 - (num11 + 1)) * 0x80;
                                    _dbl9.X = (x + 1) * 0x80;
                                    _dbl9.Y = this.Terrain.Vertices[x + 1, num11 + 1].Height * this.HeightMultiplier;
                                    _dbl9.Z = (0 - (num11 + 1)) * 0x80;
                                    GL.Begin(BeginMode.LineLoop);
                                    GL.Color3((float) 0.75f, (float) 1f, (float) 0f);
                                    GL.Vertex3(_dbl6.X, _dbl6.Y, -_dbl6.Z);
                                    GL.Vertex3(_dbl7.X, _dbl7.Y, -_dbl7.Z);
                                    GL.Vertex3(_dbl9.X, _dbl9.Y, -_dbl9.Z);
                                    GL.Vertex3(_dbl8.X, _dbl8.Y, -_dbl8.Z);
                                    GL.End();
                                    x++;
                                }
                                continue;
                            }
                            x = current.PosA.X;
                            num11 = current.PosA.Y;
                            _dbl6.X = x * 0x80;
                            _dbl6.Y = this.Terrain.Vertices[x, num11].Height * this.HeightMultiplier;
                            _dbl6.Z = (0 - num11) * 0x80;
                            _dbl7.X = (x + 1) * 0x80;
                            _dbl7.Y = this.Terrain.Vertices[x + 1, num11].Height * this.HeightMultiplier;
                            _dbl7.Z = (0 - num11) * 0x80;
                            _dbl8.X = x * 0x80;
                            _dbl8.Y = this.Terrain.Vertices[x, num11 + 1].Height * this.HeightMultiplier;
                            _dbl8.Z = (0 - (num11 + 1)) * 0x80;
                            _dbl9.X = (x + 1) * 0x80;
                            _dbl9.Y = this.Terrain.Vertices[x + 1, num11 + 1].Height * this.HeightMultiplier;
                            _dbl9.Z = (0 - (num11 + 1)) * 0x80;
                            GL.Begin(BeginMode.LineLoop);
                            GL.Color3((float) 1f, (float) 0f, (float) 0f);
                            GL.Vertex3(_dbl6.X, _dbl6.Y, -_dbl6.Z);
                            GL.Vertex3(_dbl7.X, _dbl7.Y, -_dbl7.Z);
                            GL.Vertex3(_dbl9.X, _dbl9.Y, -_dbl9.Z);
                            GL.Vertex3(_dbl8.X, _dbl8.Y, -_dbl8.Z);
                            GL.End();
                            x = current.PosB.X;
                            num11 = current.PosB.Y;
                            _dbl6.X = x * 0x80;
                            _dbl6.Y = this.Terrain.Vertices[x, num11].Height * this.HeightMultiplier;
                            _dbl6.Z = (0 - num11) * 0x80;
                            _dbl7.X = (x + 1) * 0x80;
                            _dbl7.Y = this.Terrain.Vertices[x + 1, num11].Height * this.HeightMultiplier;
                            _dbl7.Z = (0 - num11) * 0x80;
                            _dbl8.X = x * 0x80;
                            _dbl8.Y = this.Terrain.Vertices[x, num11 + 1].Height * this.HeightMultiplier;
                            _dbl8.Z = (0 - (num11 + 1)) * 0x80;
                            _dbl9.X = (x + 1) * 0x80;
                            _dbl9.Y = this.Terrain.Vertices[x + 1, num11 + 1].Height * this.HeightMultiplier;
                            _dbl9.Z = (0 - (num11 + 1)) * 0x80;
                            GL.Begin(BeginMode.LineLoop);
                            GL.Color3((float) 1f, (float) 0f, (float) 0f);
                            GL.Vertex3(_dbl6.X, _dbl6.Y, -_dbl6.Z);
                            GL.Vertex3(_dbl7.X, _dbl7.Y, -_dbl7.Z);
                            GL.Vertex3(_dbl9.X, _dbl9.Y, -_dbl9.Z);
                            GL.Vertex3(_dbl8.X, _dbl8.Y, -_dbl8.Z);
                            GL.End();
                        }
                    }
                }
                finally
                {
                    if (enumerator is IDisposable)
                    {
                        (enumerator as IDisposable).Dispose();
                    }
                }
                this.DebugGLError("Gateways");
            }
            if (mouseOverTerrain != null)
            {
                clsBrush textureBrush;
                if (modTools.Tool == modTools.Tools.ObjectSelect)
                {
                    if (this.Unit_Selected_Area_VertexA != null)
                    {
                        int num8;
                        int num10;
                        modMath.ReorderXY(this.Unit_Selected_Area_VertexA.XY, mouseOverTerrain.Vertex.Normal, ref _int6, ref _int);
                        GL.LineWidth(2f);
                        GL.Color3((float) 0f, (float) 1f, (float) 1f);
                        int num16 = _int.X - 1;
                        for (num8 = _int6.X; num8 <= num16; num8++)
                        {
                            _dbl6.X = num8 * 0x80;
                            _dbl6.Y = this.Terrain.Vertices[num8, _int6.Y].Height * this.HeightMultiplier;
                            _dbl6.Z = (0 - _int6.Y) * 0x80;
                            _dbl7.X = (num8 + 1) * 0x80;
                            _dbl7.Y = this.Terrain.Vertices[num8 + 1, _int6.Y].Height * this.HeightMultiplier;
                            _dbl7.Z = (0 - _int6.Y) * 0x80;
                            GL.Begin(BeginMode.Lines);
                            GL.Vertex3(_dbl6.X, _dbl6.Y, -_dbl6.Z);
                            GL.Vertex3(_dbl7.X, _dbl7.Y, -_dbl7.Z);
                            GL.End();
                        }
                        int num17 = _int.X - 1;
                        for (num8 = _int6.X; num8 <= num17; num8++)
                        {
                            _dbl6.X = num8 * 0x80;
                            _dbl6.Y = this.Terrain.Vertices[num8, _int.Y].Height * this.HeightMultiplier;
                            _dbl6.Z = (0 - _int.Y) * 0x80;
                            _dbl7.X = (num8 + 1) * 0x80;
                            _dbl7.Y = this.Terrain.Vertices[num8 + 1, _int.Y].Height * this.HeightMultiplier;
                            _dbl7.Z = (0 - _int.Y) * 0x80;
                            GL.Begin(BeginMode.Lines);
                            GL.Vertex3(_dbl6.X, _dbl6.Y, -_dbl6.Z);
                            GL.Vertex3(_dbl7.X, _dbl7.Y, -_dbl7.Z);
                            GL.End();
                        }
                        int num18 = _int.Y - 1;
                        for (num10 = _int6.Y; num10 <= num18; num10++)
                        {
                            _dbl6.X = _int6.X * 0x80;
                            _dbl6.Y = this.Terrain.Vertices[_int6.X, num10].Height * this.HeightMultiplier;
                            _dbl6.Z = (0 - num10) * 0x80;
                            _dbl7.X = _int6.X * 0x80;
                            _dbl7.Y = this.Terrain.Vertices[_int6.X, num10 + 1].Height * this.HeightMultiplier;
                            _dbl7.Z = (0 - (num10 + 1)) * 0x80;
                            GL.Begin(BeginMode.Lines);
                            GL.Vertex3(_dbl6.X, _dbl6.Y, -_dbl6.Z);
                            GL.Vertex3(_dbl7.X, _dbl7.Y, -_dbl7.Z);
                            GL.End();
                        }
                        int num19 = _int.Y - 1;
                        for (num10 = _int6.Y; num10 <= num19; num10++)
                        {
                            _dbl6.X = _int.X * 0x80;
                            _dbl6.Y = this.Terrain.Vertices[_int.X, num10].Height * this.HeightMultiplier;
                            _dbl6.Z = (0 - num10) * 0x80;
                            _dbl7.X = _int.X * 0x80;
                            _dbl7.Y = this.Terrain.Vertices[_int.X, num10 + 1].Height * this.HeightMultiplier;
                            _dbl7.Z = (0 - (num10 + 1)) * 0x80;
                            GL.Begin(BeginMode.Lines);
                            GL.Vertex3(_dbl6.X, _dbl6.Y, -_dbl6.Z);
                            GL.Vertex3(_dbl7.X, _dbl7.Y, -_dbl7.Z);
                            GL.End();
                        }
                        this.DebugGLError("Object selection box");
                    }
                    else
                    {
                        GL.LineWidth(2f);
                        GL.Color3((float) 0f, (float) 1f, (float) 1f);
                        GL.Begin(BeginMode.Lines);
                        GL.Vertex3(mouseOverTerrain.Pos.Horizontal.X - 16.0, (double) mouseOverTerrain.Pos.Altitude, mouseOverTerrain.Pos.Horizontal.Y - 16.0);
                        GL.Vertex3(mouseOverTerrain.Pos.Horizontal.X + 16.0, (double) mouseOverTerrain.Pos.Altitude, mouseOverTerrain.Pos.Horizontal.Y + 16.0);
                        GL.Vertex3(mouseOverTerrain.Pos.Horizontal.X + 16.0, (double) mouseOverTerrain.Pos.Altitude, mouseOverTerrain.Pos.Horizontal.Y - 16.0);
                        GL.Vertex3(mouseOverTerrain.Pos.Horizontal.X - 16.0, (double) mouseOverTerrain.Pos.Altitude, mouseOverTerrain.Pos.Horizontal.Y + 16.0);
                        GL.End();
                        this.DebugGLError("Mouse over position");
                    }
                }
                if (modTools.Tool == modTools.Tools.RoadPlace)
                {
                    GL.LineWidth(2f);
                    if (mouseOverTerrain.Side_IsV)
                    {
                        _dbl6.X = mouseOverTerrain.Side_Num.X * 0x80;
                        _dbl6.Y = this.Terrain.Vertices[mouseOverTerrain.Side_Num.X, mouseOverTerrain.Side_Num.Y].Height * this.HeightMultiplier;
                        _dbl6.Z = (0 - mouseOverTerrain.Side_Num.Y) * 0x80;
                        _dbl7.X = mouseOverTerrain.Side_Num.X * 0x80;
                        _dbl7.Y = this.Terrain.Vertices[mouseOverTerrain.Side_Num.X, mouseOverTerrain.Side_Num.Y + 1].Height * this.HeightMultiplier;
                        _dbl7.Z = (0 - (mouseOverTerrain.Side_Num.Y + 1)) * 0x80;
                    }
                    else
                    {
                        _dbl6.X = mouseOverTerrain.Side_Num.X * 0x80;
                        _dbl6.Y = this.Terrain.Vertices[mouseOverTerrain.Side_Num.X, mouseOverTerrain.Side_Num.Y].Height * this.HeightMultiplier;
                        _dbl6.Z = (0 - mouseOverTerrain.Side_Num.Y) * 0x80;
                        _dbl7.X = (mouseOverTerrain.Side_Num.X + 1) * 0x80;
                        _dbl7.Y = this.Terrain.Vertices[mouseOverTerrain.Side_Num.X + 1, mouseOverTerrain.Side_Num.Y].Height * this.HeightMultiplier;
                        _dbl7.Z = (0 - mouseOverTerrain.Side_Num.Y) * 0x80;
                    }
                    GL.Begin(BeginMode.Lines);
                    GL.Color3((float) 0f, (float) 1f, (float) 1f);
                    GL.Vertex3(_dbl6.X, _dbl6.Y, -_dbl6.Z);
                    GL.Vertex3(_dbl7.X, _dbl7.Y, -_dbl7.Z);
                    GL.End();
                    this.DebugGLError("Road place brush");
                }
                else if (((modTools.Tool == modTools.Tools.RoadLines) | (modTools.Tool == modTools.Tools.Gateways)) | (modTools.Tool == modTools.Tools.ObjectLines))
                {
                    GL.LineWidth(2f);
                    if (this.Selected_Tile_A != null)
                    {
                        x = this.Selected_Tile_A.X;
                        num11 = this.Selected_Tile_A.Y;
                        _dbl6.X = x * 0x80;
                        _dbl6.Y = this.Terrain.Vertices[x, num11].Height * this.HeightMultiplier;
                        _dbl6.Z = (0 - num11) * 0x80;
                        _dbl7.X = (x + 1) * 0x80;
                        _dbl7.Y = this.Terrain.Vertices[x + 1, num11].Height * this.HeightMultiplier;
                        _dbl7.Z = (0 - num11) * 0x80;
                        _dbl8.X = x * 0x80;
                        _dbl8.Y = this.Terrain.Vertices[x, num11 + 1].Height * this.HeightMultiplier;
                        _dbl8.Z = (0 - (num11 + 1)) * 0x80;
                        _dbl9.X = (x + 1) * 0x80;
                        _dbl9.Y = this.Terrain.Vertices[x + 1, num11 + 1].Height * this.HeightMultiplier;
                        _dbl9.Z = (0 - (num11 + 1)) * 0x80;
                        GL.Begin(BeginMode.LineLoop);
                        GL.Color3((float) 0f, (float) 1f, (float) 1f);
                        GL.Vertex3(_dbl6.X, _dbl6.Y, -_dbl6.Z);
                        GL.Vertex3(_dbl7.X, _dbl7.Y, -_dbl7.Z);
                        GL.Vertex3(_dbl9.X, _dbl9.Y, -_dbl9.Z);
                        GL.Vertex3(_dbl8.X, _dbl8.Y, -_dbl8.Z);
                        GL.End();
                        if (mouseOverTerrain.Tile.Normal.X == this.Selected_Tile_A.X)
                        {
                            if (mouseOverTerrain.Tile.Normal.Y <= this.Selected_Tile_A.Y)
                            {
                                num = mouseOverTerrain.Tile.Normal.Y;
                                num2 = this.Selected_Tile_A.Y;
                            }
                            else
                            {
                                num = this.Selected_Tile_A.Y;
                                num2 = mouseOverTerrain.Tile.Normal.Y;
                            }
                            x = this.Selected_Tile_A.X;
                            int num20 = num2;
                            for (num11 = num; num11 <= num20; num11++)
                            {
                                _dbl6.X = x * 0x80;
                                _dbl6.Y = this.Terrain.Vertices[x, num11].Height * this.HeightMultiplier;
                                _dbl6.Z = (0 - num11) * 0x80;
                                _dbl7.X = (x + 1) * 0x80;
                                _dbl7.Y = this.Terrain.Vertices[x + 1, num11].Height * this.HeightMultiplier;
                                _dbl7.Z = (0 - num11) * 0x80;
                                _dbl8.X = x * 0x80;
                                _dbl8.Y = this.Terrain.Vertices[x, num11 + 1].Height * this.HeightMultiplier;
                                _dbl8.Z = (0 - (num11 + 1)) * 0x80;
                                _dbl9.X = (x + 1) * 0x80;
                                _dbl9.Y = this.Terrain.Vertices[x + 1, num11 + 1].Height * this.HeightMultiplier;
                                _dbl9.Z = (0 - (num11 + 1)) * 0x80;
                                GL.Begin(BeginMode.LineLoop);
                                GL.Color3((float) 0f, (float) 1f, (float) 1f);
                                GL.Vertex3(_dbl6.X, _dbl6.Y, -_dbl6.Z);
                                GL.Vertex3(_dbl7.X, _dbl7.Y, -_dbl7.Z);
                                GL.Vertex3(_dbl9.X, _dbl9.Y, -_dbl9.Z);
                                GL.Vertex3(_dbl8.X, _dbl8.Y, -_dbl8.Z);
                                GL.End();
                            }
                        }
                        else if (mouseOverTerrain.Tile.Normal.Y == this.Selected_Tile_A.Y)
                        {
                            if (mouseOverTerrain.Tile.Normal.X <= this.Selected_Tile_A.X)
                            {
                                num = mouseOverTerrain.Tile.Normal.X;
                                num2 = this.Selected_Tile_A.X;
                            }
                            else
                            {
                                num = this.Selected_Tile_A.X;
                                num2 = mouseOverTerrain.Tile.Normal.X;
                            }
                            num11 = this.Selected_Tile_A.Y;
                            int num21 = num2;
                            for (x = num; x <= num21; x++)
                            {
                                _dbl6.X = x * 0x80;
                                _dbl6.Y = this.Terrain.Vertices[x, num11].Height * this.HeightMultiplier;
                                _dbl6.Z = (0 - num11) * 0x80;
                                _dbl7.X = (x + 1) * 0x80;
                                _dbl7.Y = this.Terrain.Vertices[x + 1, num11].Height * this.HeightMultiplier;
                                _dbl7.Z = (0 - num11) * 0x80;
                                _dbl8.X = x * 0x80;
                                _dbl8.Y = this.Terrain.Vertices[x, num11 + 1].Height * this.HeightMultiplier;
                                _dbl8.Z = (0 - (num11 + 1)) * 0x80;
                                _dbl9.X = (x + 1) * 0x80;
                                _dbl9.Y = this.Terrain.Vertices[x + 1, num11 + 1].Height * this.HeightMultiplier;
                                _dbl9.Z = (0 - (num11 + 1)) * 0x80;
                                GL.Begin(BeginMode.LineLoop);
                                GL.Color3((float) 0f, (float) 1f, (float) 1f);
                                GL.Vertex3(_dbl6.X, _dbl6.Y, -_dbl6.Z);
                                GL.Vertex3(_dbl7.X, _dbl7.Y, -_dbl7.Z);
                                GL.Vertex3(_dbl9.X, _dbl9.Y, -_dbl9.Z);
                                GL.Vertex3(_dbl8.X, _dbl8.Y, -_dbl8.Z);
                                GL.End();
                            }
                        }
                    }
                    else
                    {
                        x = mouseOverTerrain.Tile.Normal.X;
                        num11 = mouseOverTerrain.Tile.Normal.Y;
                        _dbl6.X = x * 0x80;
                        _dbl6.Y = this.Terrain.Vertices[x, num11].Height * this.HeightMultiplier;
                        _dbl6.Z = (0 - num11) * 0x80;
                        _dbl7.X = (x + 1) * 0x80;
                        _dbl7.Y = this.Terrain.Vertices[x + 1, num11].Height * this.HeightMultiplier;
                        _dbl7.Z = (0 - num11) * 0x80;
                        _dbl8.X = x * 0x80;
                        _dbl8.Y = this.Terrain.Vertices[x, num11 + 1].Height * this.HeightMultiplier;
                        _dbl8.Z = (0 - (num11 + 1)) * 0x80;
                        _dbl9.X = (x + 1) * 0x80;
                        _dbl9.Y = this.Terrain.Vertices[x + 1, num11 + 1].Height * this.HeightMultiplier;
                        _dbl9.Z = (0 - (num11 + 1)) * 0x80;
                        GL.Begin(BeginMode.LineLoop);
                        GL.Color3((float) 0f, (float) 1f, (float) 1f);
                        GL.Vertex3(_dbl6.X, _dbl6.Y, -_dbl6.Z);
                        GL.Vertex3(_dbl7.X, _dbl7.Y, -_dbl7.Z);
                        GL.Vertex3(_dbl9.X, _dbl9.Y, -_dbl9.Z);
                        GL.Vertex3(_dbl8.X, _dbl8.Y, -_dbl8.Z);
                        GL.End();
                    }
                    this.DebugGLError("Line brush");
                }
                if (modTools.Tool == modTools.Tools.TextureBrush)
                {
                    textureBrush = modProgram.TextureBrush;
                }
                else if (modTools.Tool == modTools.Tools.CliffBrush)
                {
                    textureBrush = modProgram.CliffBrush;
                }
                else if (modTools.Tool == modTools.Tools.CliffRemove)
                {
                    textureBrush = modProgram.CliffBrush;
                }
                else if (modTools.Tool == modTools.Tools.RoadRemove)
                {
                    textureBrush = modProgram.CliffBrush;
                }
                else
                {
                    textureBrush = null;
                }
                if (textureBrush != null)
                {
                    GL.LineWidth(2f);
                    clsDrawTileOutline outline2 = new clsDrawTileOutline {
                        Map = this
                    };
                    outline2.Colour.Red = 0f;
                    outline2.Colour.Green = 1f;
                    outline2.Colour.Blue = 1f;
                    outline2.Colour.Alpha = 1f;
                    textureBrush.PerformActionMapTiles(outline2, mouseOverTerrain.Tile);
                    this.DebugGLError("Brush tiles");
                }
                if (modTools.Tool == modTools.Tools.TerrainFill)
                {
                    GL.LineWidth(2f);
                    _dbl6.X = mouseOverTerrain.Vertex.Normal.X * 0x80;
                    _dbl6.Y = this.Terrain.Vertices[mouseOverTerrain.Vertex.Normal.X, mouseOverTerrain.Vertex.Normal.Y].Height * this.HeightMultiplier;
                    _dbl6.Z = (0 - mouseOverTerrain.Vertex.Normal.Y) * 0x80;
                    GL.Begin(BeginMode.Lines);
                    GL.Color3((float) 0f, (float) 1f, (float) 1f);
                    GL.Vertex3(_dbl6.X - 8.0, _dbl6.Y, -_dbl6.Z);
                    GL.Vertex3(_dbl6.X + 8.0, _dbl6.Y, -_dbl6.Z);
                    GL.Vertex3(_dbl6.X, _dbl6.Y, -_dbl6.Z - 8.0);
                    GL.Vertex3(_dbl6.X, _dbl6.Y, -_dbl6.Z + 8.0);
                    GL.End();
                    this.DebugGLError("Mouse over vertex");
                }
                if (modTools.Tool == modTools.Tools.TerrainBrush)
                {
                    textureBrush = modProgram.TerrainBrush;
                }
                else if (modTools.Tool == modTools.Tools.HeightSetBrush)
                {
                    textureBrush = modProgram.HeightBrush;
                }
                else if (modTools.Tool == modTools.Tools.HeightChangeBrush)
                {
                    textureBrush = modProgram.HeightBrush;
                }
                else if (modTools.Tool == modTools.Tools.HeightSmoothBrush)
                {
                    textureBrush = modProgram.HeightBrush;
                }
                else
                {
                    textureBrush = null;
                }
                if (textureBrush != null)
                {
                    GL.LineWidth(2f);
                    clsDrawVertexMarker marker = new clsDrawVertexMarker {
                        Map = this
                    };
                    marker.Colour.Red = 0f;
                    marker.Colour.Green = 1f;
                    marker.Colour.Blue = 1f;
                    marker.Colour.Alpha = 1f;
                    textureBrush.PerformActionMapVertices(marker, mouseOverTerrain.Vertex);
                    this.DebugGLError("Brush vertices");
                }
            }
            GL.Enable(EnableCap.DepthTest);
            GL.Disable(EnableCap.CullFace);
            GL.LoadIdentity();
            GL.Rotate((double) (this.ViewInfo.ViewAngleRPY.Roll / 0.017453292519943295), (double) 0.0, (double) 0.0, (double) -1.0);
            GL.Rotate((double) (this.ViewInfo.ViewAngleRPY.Pitch / 0.017453292519943295), (double) 1.0, (double) 0.0, (double) 0.0);
            GL.Rotate((double) (this.ViewInfo.ViewAngleRPY.Yaw / 0.017453292519943295), (double) 0.0, (double) 1.0, (double) 0.0);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            this.DebugGLError("Object matrix modes");
            if (modProgram.Draw_Units)
            {
                GL.Color3((float) 1f, (float) 1f, (float) 1f);
                GL.Enable(EnableCap.Texture2D);
                modProgram.VisionSectors.PerformActionMapSectors(tool, num7);
                GL.Disable(EnableCap.Texture2D);
                this.DebugGLError("Objects");
            }
            if (mouseOverTerrain != null)
            {
                GL.Enable(EnableCap.Texture2D);
                if (modTools.Tool == modTools.Tools.ObjectPlace)
                {
                    clsUnitType singleSelectedObjectType = modMain.frmMainInstance.SingleSelectedObjectType;
                    if (singleSelectedObjectType != null)
                    {
                        int num13;
                        try
                        {
                            modIO.InvariantParse_int(modMain.frmMainInstance.txtNewObjectRotation.Text, ref num13);
                            if ((num13 < 0) | (num13 > 0x167))
                            {
                                num13 = 0;
                            }
                        }
                        catch (Exception exception1)
                        {
                            ProjectData.SetProjectError(exception1);
                            num13 = 0;
                            ProjectData.ClearProjectError();
                        }
                        modProgram.sWorldPos pos = this.TileAlignedPosFromMapPos(mouseOverTerrain.Pos.Horizontal, singleSelectedObjectType.get_GetFootprintSelected(num13));
                        GL.PushMatrix();
                        GL.Translate((double) (pos.Horizontal.X - this.ViewInfo.ViewPos.X), (pos.Altitude - this.ViewInfo.ViewPos.Y) + 2.0, (double) (this.ViewInfo.ViewPos.Z + pos.Horizontal.Y));
                        singleSelectedObjectType.GLDraw((float) num13);
                        GL.PopMatrix();
                    }
                }
                GL.Disable(EnableCap.Texture2D);
                this.DebugGLError("Mouse over object");
            }
            GL.Disable(EnableCap.DepthTest);
            clsTextLabels labels2 = new clsTextLabels(0x100);
            if (!modProgram.Draw_ScriptMarkers)
            {
                goto Label_3062;
            }
            GL.PushMatrix();
            GL.Translate((float) (0 - this.ViewInfo.ViewPos.X), (float) (0 - this.ViewInfo.ViewPos.Y), (float) this.ViewInfo.ViewPos.Z);
            try
            {
                enumerator2 = this.ScriptPositions.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    position = (clsScriptPosition) enumerator2.Current;
                    position.GLDraw();
                }
            }
            finally
            {
                if (enumerator2 is IDisposable)
                {
                    (enumerator2 as IDisposable).Dispose();
                }
            }
            try
            {
                enumerator3 = this.ScriptAreas.GetEnumerator();
                while (enumerator3.MoveNext())
                {
                    area = (clsScriptArea) enumerator3.Current;
                    area.GLDraw();
                }
            }
            finally
            {
                if (enumerator3 is IDisposable)
                {
                    (enumerator3 as IDisposable).Dispose();
                }
            }
            try
            {
                enumerator4 = this.ScriptPositions.GetEnumerator();
                while (enumerator4.MoveNext())
                {
                    position = (clsScriptPosition) enumerator4.Current;
                    if (labels2.AtMaxCount())
                    {
                        goto Label_2E85;
                    }
                    _dbl14.X = position.PosX - this.ViewInfo.ViewPos.X;
                    _dbl14.Z = (0 - position.PosY) - this.ViewInfo.ViewPos.Z;
                    screenPos = new modMath.sXY_int(position.PosX, position.PosY);
                    _dbl14.Y = this.GetTerrainHeight(screenPos) - this.ViewInfo.ViewPos.Y;
                    Matrix3DMath.VectorRotationByMatrix(this.ViewInfo.ViewAngleMatrix_Inverted, _dbl14, ref _dbl15);
                    if (this.ViewInfo.Pos_Get_Screen_XY(_dbl15, ref _int5) && ((((_int5.X >= 0) & (_int5.X <= gLSize.X)) & (_int5.Y >= 0)) & (_int5.Y <= gLSize.Y)))
                    {
                        label2 = new clsTextLabel();
                        label2.Colour.Red = 1f;
                        label2.Colour.Green = 1f;
                        label2.Colour.Blue = 0.5f;
                        label2.Colour.Alpha = 0.75f;
                        label2.TextFont = modProgram.UnitLabelFont;
                        label2.SizeY = modSettings.Settings.FontSize;
                        label2.Pos = _int5;
                        label2.Text = position.Label;
                        labels2.Add(label2);
                    }
                }
            }
            finally
            {
                if (enumerator4 is IDisposable)
                {
                    (enumerator4 as IDisposable).Dispose();
                }
            }
        Label_2E85:
            this.DebugGLError("Script positions");
            try
            {
                enumerator5 = this.ScriptAreas.GetEnumerator();
                while (enumerator5.MoveNext())
                {
                    area = (clsScriptArea) enumerator5.Current;
                    if (labels2.AtMaxCount())
                    {
                        goto Label_3052;
                    }
                    _dbl14.X = area.PosAX - this.ViewInfo.ViewPos.X;
                    _dbl14.Z = (0 - area.PosAY) - this.ViewInfo.ViewPos.Z;
                    screenPos = new modMath.sXY_int(area.PosAX, area.PosAY);
                    _dbl14.Y = this.GetTerrainHeight(screenPos) - this.ViewInfo.ViewPos.Y;
                    Matrix3DMath.VectorRotationByMatrix(this.ViewInfo.ViewAngleMatrix_Inverted, _dbl14, ref _dbl15);
                    if (this.ViewInfo.Pos_Get_Screen_XY(_dbl15, ref _int5) && ((((_int5.X >= 0) & (_int5.X <= gLSize.X)) & (_int5.Y >= 0)) & (_int5.Y <= gLSize.Y)))
                    {
                        label2 = new clsTextLabel();
                        label2.Colour.Red = 1f;
                        label2.Colour.Green = 1f;
                        label2.Colour.Blue = 0.5f;
                        label2.Colour.Alpha = 0.75f;
                        label2.TextFont = modProgram.UnitLabelFont;
                        label2.SizeY = modSettings.Settings.FontSize;
                        label2.Pos = _int5;
                        label2.Text = area.Label;
                        labels2.Add(label2);
                    }
                }
            }
            finally
            {
                if (enumerator5 is IDisposable)
                {
                    (enumerator5 as IDisposable).Dispose();
                }
            }
        Label_3052:
            GL.PopMatrix();
            this.DebugGLError("Script areas");
        Label_3062:
            labels = new clsTextLabels(0x18);
            num2 = 0;
            int num22 = this.Messages.Count - 1;
            for (num = Math.Max(this.Messages.Count - labels.MaxCount, 0); num <= num22; num++)
            {
                if (!labels.AtMaxCount())
                {
                    label2 = new clsTextLabel();
                    label2.Colour.Red = 0.875f;
                    label2.Colour.Green = 0.875f;
                    label2.Colour.Blue = 1f;
                    label2.Colour.Alpha = 1f;
                    label2.TextFont = modProgram.UnitLabelFont;
                    label2.SizeY = modSettings.Settings.FontSize;
                    label2.Pos.X = 0x20 + _int4.X;
                    label2.Pos.Y = 0x20 + ((int) Math.Round(Math.Ceiling((double) (num2 * label2.SizeY))));
                    label2.Text = this.Messages[num].Text;
                    labels.Add(label2);
                    num2++;
                }
            }
            GL.Begin(BeginMode.Quads);
            try
            {
                enumerator6 = this.SelectedUnits.GetEnumerator();
                while (enumerator6.MoveNext())
                {
                    unit = (clsUnit) enumerator6.Current;
                    _int2 = unit.Type.get_GetFootprintSelected(unit.Rotation);
                    unitGroupColour = this.GetUnitGroupColour(unit.UnitGroup);
                    _sng = new sRGBA_sng((1f + unitGroupColour.Red) / 2f, (1f + unitGroupColour.Green) / 2f, (1f + unitGroupColour.Blue) / 2f, 0.75f);
                    _sng2 = new sRGBA_sng(unitGroupColour.Red, unitGroupColour.Green, unitGroupColour.Blue, 0.75f);
                    this.DrawUnitRectangle(unit, 8, _sng, _sng2);
                }
            }
            finally
            {
                if (enumerator6 is IDisposable)
                {
                    (enumerator6 as IDisposable).Dispose();
                }
            }
            if (mouseOverTerrain != null)
            {
                IEnumerator enumerator7;
                try
                {
                    enumerator7 = mouseOverTerrain.Units.GetEnumerator();
                    while (enumerator7.MoveNext())
                    {
                        unit = (clsUnit) enumerator7.Current;
                        if ((unit != null) & (modTools.Tool == modTools.Tools.ObjectSelect))
                        {
                            unitGroupColour = this.GetUnitGroupColour(unit.UnitGroup);
                            GL.Color4((float) ((0.5f + unitGroupColour.Red) / 1.5f), (float) ((0.5f + unitGroupColour.Green) / 1.5f), (float) ((0.5f + unitGroupColour.Blue) / 1.5f), (float) 0.75f);
                            _int2 = unit.Type.get_GetFootprintSelected(unit.Rotation);
                            _sng = new sRGBA_sng((1f + unitGroupColour.Red) / 2f, (1f + unitGroupColour.Green) / 2f, (1f + unitGroupColour.Blue) / 2f, 0.75f);
                            _sng2 = new sRGBA_sng(unitGroupColour.Red, unitGroupColour.Green, unitGroupColour.Blue, 0.875f);
                            this.DrawUnitRectangle(unit, 0x10, _sng, _sng2);
                        }
                    }
                }
                finally
                {
                    if (enumerator7 is IDisposable)
                    {
                        (enumerator7 as IDisposable).Dispose();
                    }
                }
            }
            GL.End();
            this.DebugGLError("Unit selection");
            GL.MatrixMode(OpenTK.Graphics.OpenGL.MatrixMode.Projection);
            GL.LoadMatrix(ref Matrix4.CreateOrthographicOffCenter(0f, (float) gLSize.X, (float) gLSize.Y, 0f, -1f, 1f));
            GL.MatrixMode(OpenTK.Graphics.OpenGL.MatrixMode.Modelview);
            GL.LoadIdentity();
            this.DebugGLError("Text label matrix modes");
            GL.Enable(EnableCap.Texture2D);
            labels2.Draw();
            tool.UnitTextLabels.Draw();
            label.Draw();
            labels.Draw();
            this.DebugGLError("Text labels");
            GL.Disable(EnableCap.Texture2D);
            GL.Disable(EnableCap.Blend);
            GL.MatrixMode(OpenTK.Graphics.OpenGL.MatrixMode.Projection);
            GL.LoadMatrix(ref Matrix4.CreateOrthographicOffCenter(0f, (float) gLSize.X, 0f, (float) gLSize.Y, -1f, 1f));
            GL.MatrixMode(OpenTK.Graphics.OpenGL.MatrixMode.Modelview);
            GL.LoadIdentity();
            this.DebugGLError("Minimap matrix modes");
            if ((this.Minimap_Texture_Size > 0) & (this.ViewInfo.Tiles_Per_Minimap_Pixel > 0.0))
            {
                Position.XY_dbl _dbl2;
                Position.XY_dbl _dbl3;
                Position.XY_dbl _dbl4;
                Position.XY_dbl _dbl5;
                GL.Translate(0f, (float) (gLSize.Y - _int4.Y), 0f);
                _dbl14.X = ((double) this.Terrain.TileSize.X) / ((double) this.Minimap_Texture_Size);
                _dbl14.Z = ((double) this.Terrain.TileSize.Y) / ((double) this.Minimap_Texture_Size);
                if (this.Minimap_GLTexture > 0)
                {
                    GL.Enable(EnableCap.Texture2D);
                    GL.BindTexture(TextureTarget.Texture2D, this.Minimap_GLTexture);
                    GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, 0x2101);
                    GL.Begin(BeginMode.Quads);
                    GL.TexCoord2((float) 0f, (float) 0f);
                    GL.Vertex2(0, _int4.Y);
                    GL.TexCoord2((float) _dbl14.X, 0f);
                    GL.Vertex2(_int4.X, _int4.Y);
                    GL.TexCoord2((float) _dbl14.X, (float) _dbl14.Z);
                    GL.Vertex2(_int4.X, 0);
                    GL.TexCoord2(0f, (float) _dbl14.Z);
                    GL.Vertex2(0, 0);
                    GL.End();
                    GL.Disable(EnableCap.Texture2D);
                    this.DebugGLError("Minimap");
                }
                GL.LineWidth(1f);
                GL.Begin(BeginMode.Lines);
                GL.Color3((float) 0.75f, (float) 0.75f, (float) 0.75f);
                GL.Vertex2((float) _int4.X, 0f);
                GL.Vertex2(_int4.X, _int4.Y);
                GL.Vertex2((float) 0f, (float) 0f);
                GL.Vertex2((float) _int4.X, 0f);
                GL.End();
                this.DebugGLError("Minimap border");
                if (flag2)
                {
                    minimapSize = 128.0 * this.ViewInfo.Tiles_Per_Minimap_Pixel;
                    _dbl2.X = _dbl10.X / minimapSize;
                    _dbl2.Y = _int4.Y + (_dbl10.Y / minimapSize);
                    _dbl3.X = _dbl11.X / minimapSize;
                    _dbl3.Y = _int4.Y + (_dbl11.Y / minimapSize);
                    _dbl4.X = _dbl12.X / minimapSize;
                    _dbl4.Y = _int4.Y + (_dbl12.Y / minimapSize);
                    _dbl5.X = _dbl13.X / minimapSize;
                    _dbl5.Y = _int4.Y + (_dbl13.Y / minimapSize);
                    GL.LineWidth(1f);
                    GL.Begin(BeginMode.LineLoop);
                    GL.Color3((float) 1f, (float) 1f, (float) 1f);
                    GL.Vertex2(_dbl2.X, _dbl2.Y);
                    GL.Vertex2(_dbl3.X, _dbl3.Y);
                    GL.Vertex2(_dbl4.X, _dbl4.Y);
                    GL.Vertex2(_dbl5.X, _dbl5.Y);
                    GL.End();
                    this.DebugGLError("Minimap view position polygon");
                }
                if (this.Selected_Area_VertexA != null)
                {
                    flag = false;
                    if (this.Selected_Area_VertexB != null)
                    {
                        modMath.ReorderXY(this.Selected_Area_VertexA.XY, this.Selected_Area_VertexB.XY, ref _int6, ref _int);
                        flag = true;
                    }
                    else if ((modTools.Tool == modTools.Tools.TerrainSelect) && (mouseOverTerrain != null))
                    {
                        modMath.ReorderXY(this.Selected_Area_VertexA.XY, mouseOverTerrain.Vertex.Normal, ref _int6, ref _int);
                        flag = true;
                    }
                    if (flag)
                    {
                        GL.LineWidth(1f);
                        _dbl2.X = ((double) _int6.X) / this.ViewInfo.Tiles_Per_Minimap_Pixel;
                        _dbl2.Y = _int4.Y - (((double) _int6.Y) / this.ViewInfo.Tiles_Per_Minimap_Pixel);
                        _dbl3.X = ((double) _int.X) / this.ViewInfo.Tiles_Per_Minimap_Pixel;
                        _dbl3.Y = _int4.Y - (((double) _int6.Y) / this.ViewInfo.Tiles_Per_Minimap_Pixel);
                        _dbl4.X = ((double) _int.X) / this.ViewInfo.Tiles_Per_Minimap_Pixel;
                        _dbl4.Y = _int4.Y - (((double) _int.Y) / this.ViewInfo.Tiles_Per_Minimap_Pixel);
                        _dbl5.X = ((double) _int6.X) / this.ViewInfo.Tiles_Per_Minimap_Pixel;
                        _dbl5.Y = _int4.Y - (((double) _int.Y) / this.ViewInfo.Tiles_Per_Minimap_Pixel);
                        GL.Begin(BeginMode.LineLoop);
                        GL.Color3((float) 1f, (float) 1f, (float) 1f);
                        GL.Vertex2(_dbl2.X, _dbl2.Y);
                        GL.Vertex2(_dbl3.X, _dbl3.Y);
                        GL.Vertex2(_dbl4.X, _dbl4.Y);
                        GL.Vertex2(_dbl5.X, _dbl5.Y);
                        GL.End();
                        this.DebugGLError("Minimap selection box");
                    }
                }
            }
        }

        public clsUnit IDUsage(uint ID)
        {
            IEnumerator enumerator;
            try
            {
                enumerator = this.Units.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    clsUnit current = (clsUnit) enumerator.Current;
                    if (current.ID == ID)
                    {
                        return current;
                    }
                }
            }
            finally
            {
                if (enumerator is IDisposable)
                {
                    (enumerator as IDisposable).Dispose();
                }
            }
            return null;
        }

        public void Initialize()
        {
            this.MakeMinimapTimer = new Timer();
            this.MakeMinimapTimer.Interval = 100;
            this.MakeDefaultUnitGroups();
            this.ScriptPositions.MaintainOrder = true;
            this.ScriptAreas.MaintainOrder = true;
        }

        public void InitializeUserInput()
        {
            if (!this._ReadyForUserInput)
            {
                int num;
                modMath.sXY_int _int;
                IEnumerator enumerator;
                this._ReadyForUserInput = true;
                this.SectorCount.X = (int) Math.Round(Math.Ceiling((double) (((double) this.Terrain.TileSize.X) / 8.0)));
                this.SectorCount.Y = (int) Math.Round(Math.Ceiling((double) (((double) this.Terrain.TileSize.Y) / 8.0)));
                this.Sectors = new clsSector[(this.SectorCount.X - 1) + 1, (this.SectorCount.Y - 1) + 1];
                int num3 = this.SectorCount.Y - 1;
                int y = 0;
                while (y <= num3)
                {
                    int num4 = this.SectorCount.X - 1;
                    num = 0;
                    while (num <= num4)
                    {
                        _int = new modMath.sXY_int(num, y);
                        this.Sectors[num, y] = new clsSector(_int);
                        num++;
                    }
                    y++;
                }
                try
                {
                    enumerator = this.Units.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        clsUnit current = (clsUnit) enumerator.Current;
                        this.UnitSectorsCalc(current);
                    }
                }
                finally
                {
                    if (enumerator is IDisposable)
                    {
                        (enumerator as IDisposable).Dispose();
                    }
                }
                this.ShadowSectors = new clsShadowSector[(this.SectorCount.X - 1) + 1, (this.SectorCount.Y - 1) + 1];
                int num5 = this.SectorCount.Y - 1;
                for (y = 0; y <= num5; y++)
                {
                    int num6 = this.SectorCount.X - 1;
                    for (num = 0; num <= num6; num++)
                    {
                        _int = new modMath.sXY_int(num, y);
                        this.ShadowSector_Create(_int);
                    }
                }
                this.SectorGraphicsChanges = new clsSectorChanges(this);
                this.SectorGraphicsChanges.SetAllChanged();
                this.SectorUnitHeightsChanges = new clsSectorChanges(this);
                this.SectorTerrainUndoChanges = new clsSectorChanges(this);
                this.AutoTextureChanges = new clsAutoTextureChanges(this);
                this.TerrainInterpretChanges = new clsTerrainUpdate(this.Terrain.TileSize);
                this.UnitChanges = new modLists.SimpleClassList<clsUnitChange>();
                this.UnitChanges.MaintainOrder = true;
                this.GatewayChanges = new modLists.SimpleClassList<clsGatewayChange>();
                this.GatewayChanges.MaintainOrder = true;
                this.Undos = new modLists.SimpleClassList<clsUndo>();
                this.Undos.MaintainOrder = true;
                this.UndoPosition = 0;
                this.SelectedUnits = new modLists.ConnectedList<clsUnit, clsMap>(this);
                if (this.InterfaceOptions == null)
                {
                    this.InterfaceOptions = new clsInterfaceOptions();
                }
                this.ViewInfo = new clsViewInfo(this, modMain.frmMainInstance.MapView);
                this._SelectedUnitGroup = new clsUnitGroupContainer();
                this.SelectedUnitGroup.Item = this.ScavengerUnitGroup;
                this.Messages = new modLists.SimpleClassList<clsMessage>();
                this.Messages.MaintainOrder = true;
            }
        }

        public void LevelWater()
        {
            if (this.Tileset != null)
            {
                int num4 = this.Terrain.TileSize.Y - 1;
                for (int i = 0; i <= num4; i++)
                {
                    int num5 = this.Terrain.TileSize.X - 1;
                    for (int j = 0; j <= num5; j++)
                    {
                        int textureNum = this.Terrain.Tiles[j, i].Texture.TextureNum;
                        if (((textureNum >= 0) & (textureNum < this.Tileset.TileCount)) && (this.Tileset.Tiles[textureNum].Default_Type == 7))
                        {
                            this.Terrain.Vertices[j, i].Height = 0;
                            this.Terrain.Vertices[j + 1, i].Height = 0;
                            this.Terrain.Vertices[j, i + 1].Height = 0;
                            this.Terrain.Vertices[j + 1, i + 1].Height = 0;
                        }
                    }
                }
            }
        }

        public modMath.sXYZ_int LNDPos_From_MapPos(modMath.sXY_int Horizontal)
        {
            modMath.sXYZ_int _int2;
            _int2.X = Horizontal.X - ((int) Math.Round((double) (((double) (this.Terrain.TileSize.X * 0x80)) / 2.0)));
            _int2.Z = ((int) Math.Round((double) (((double) (this.Terrain.TileSize.Y * 0x80)) / 2.0))) - Horizontal.Y;
            _int2.Y = (int) Math.Round(this.GetTerrainHeight(Horizontal));
            return _int2;
        }

        public clsResult Load_FMap(string Path)
        {
            clsResult result2 = new clsResult("Loading FMap from \"" + Path + "\"");
            clsFMapInfo resultInfo = null;
            string zipPathToFind = "info.ini";
            clsZipStreamEntry entry = modIO.FindZipEntryFromPath(Path, zipPathToFind);
            if (entry == null)
            {
                result2.ProblemAdd("Unable to find file \"" + zipPathToFind + "\".");
                return result2;
            }
            StreamReader file = new StreamReader(entry.Stream);
            result2.Add(this.Read_FMap_Info(file, ref resultInfo));
            file.Close();
            if (!result2.HasProblems)
            {
                modMath.sXY_int terrainSize = resultInfo.TerrainSize;
                this.Tileset = resultInfo.Tileset;
                if ((terrainSize.X <= 0) | (terrainSize.X > 0x200))
                {
                    result2.ProblemAdd("Map width of " + Conversions.ToString(terrainSize.X) + " is not valid.");
                }
                if ((terrainSize.Y <= 0) | (terrainSize.Y > 0x200))
                {
                    result2.ProblemAdd("Map height of " + Conversions.ToString(terrainSize.Y) + " is not valid.");
                }
                if (result2.HasProblems)
                {
                    return result2;
                }
                this.SetPainterToDefaults();
                this.TerrainBlank(terrainSize);
                this.TileType_Reset();
                zipPathToFind = "vertexheight.dat";
                entry = modIO.FindZipEntryFromPath(Path, zipPathToFind);
                if (entry == null)
                {
                    result2.WarningAdd("Unable to find file \"" + zipPathToFind + "\".");
                }
                else
                {
                    BinaryReader reader2 = new BinaryReader(entry.Stream);
                    result2.Add(this.Read_FMap_VertexHeight(reader2));
                    reader2.Close();
                }
                zipPathToFind = "vertexterrain.dat";
                entry = modIO.FindZipEntryFromPath(Path, zipPathToFind);
                if (entry == null)
                {
                    result2.WarningAdd("Unable to find file \"" + zipPathToFind + "\".");
                }
                else
                {
                    BinaryReader reader3 = new BinaryReader(entry.Stream);
                    result2.Add(this.Read_FMap_VertexTerrain(reader3));
                    reader3.Close();
                }
                zipPathToFind = "tiletexture.dat";
                entry = modIO.FindZipEntryFromPath(Path, zipPathToFind);
                if (entry == null)
                {
                    result2.WarningAdd("Unable to find file \"" + zipPathToFind + "\".");
                }
                else
                {
                    BinaryReader reader4 = new BinaryReader(entry.Stream);
                    result2.Add(this.Read_FMap_TileTexture(reader4));
                    reader4.Close();
                }
                zipPathToFind = "tileorientation.dat";
                entry = modIO.FindZipEntryFromPath(Path, zipPathToFind);
                if (entry == null)
                {
                    result2.WarningAdd("Unable to find file \"" + zipPathToFind + "\".");
                }
                else
                {
                    BinaryReader reader5 = new BinaryReader(entry.Stream);
                    result2.Add(this.Read_FMap_TileOrientation(reader5));
                    reader5.Close();
                }
                zipPathToFind = "tilecliff.dat";
                entry = modIO.FindZipEntryFromPath(Path, zipPathToFind);
                if (entry == null)
                {
                    result2.WarningAdd("Unable to find file \"" + zipPathToFind + "\".");
                }
                else
                {
                    BinaryReader reader6 = new BinaryReader(entry.Stream);
                    result2.Add(this.Read_FMap_TileCliff(reader6));
                    reader6.Close();
                }
                zipPathToFind = "roads.dat";
                entry = modIO.FindZipEntryFromPath(Path, zipPathToFind);
                if (entry == null)
                {
                    result2.WarningAdd("Unable to find file \"" + zipPathToFind + "\".");
                }
                else
                {
                    BinaryReader reader7 = new BinaryReader(entry.Stream);
                    result2.Add(this.Read_FMap_Roads(reader7));
                    reader7.Close();
                }
                zipPathToFind = "objects.ini";
                entry = modIO.FindZipEntryFromPath(Path, zipPathToFind);
                if (entry == null)
                {
                    result2.WarningAdd("Unable to find file \"" + zipPathToFind + "\".");
                }
                else
                {
                    StreamReader reader8 = new StreamReader(entry.Stream);
                    result2.Add(this.Read_FMap_Objects(reader8));
                    reader8.Close();
                }
                zipPathToFind = "gateways.ini";
                entry = modIO.FindZipEntryFromPath(Path, zipPathToFind);
                if (entry == null)
                {
                    result2.WarningAdd("Unable to find file \"" + zipPathToFind + "\".");
                }
                else
                {
                    StreamReader reader9 = new StreamReader(entry.Stream);
                    result2.Add(this.Read_FMap_Gateways(reader9));
                    reader9.Close();
                }
                zipPathToFind = "tiletypes.dat";
                entry = modIO.FindZipEntryFromPath(Path, zipPathToFind);
                if (entry == null)
                {
                    result2.WarningAdd("Unable to find file \"" + zipPathToFind + "\".");
                }
                else
                {
                    BinaryReader reader10 = new BinaryReader(entry.Stream);
                    result2.Add(this.Read_FMap_TileTypes(reader10));
                    reader10.Close();
                }
                zipPathToFind = "scriptlabels.ini";
                entry = modIO.FindZipEntryFromPath(Path, zipPathToFind);
                if (entry != null)
                {
                    clsResult resultToAdd = new clsResult("Reading labels");
                    clsINIRead iNI = new clsINIRead();
                    StreamReader reader11 = new StreamReader(entry.Stream);
                    resultToAdd.Take(iNI.ReadFile(reader11));
                    reader11.Close();
                    resultToAdd.Take(this.Read_WZ_Labels(iNI, true));
                    result2.Add(resultToAdd);
                }
                this.InterfaceOptions = resultInfo.InterfaceOptions;
            }
            return result2;
        }

        public clsResult Load_FME(string Path)
        {
            BinaryReader reader;
            clsResult result2 = new clsResult("Loading FME from \"" + Path + "\"");
            try
            {
                reader = new BinaryReader(new FileStream(Path, FileMode.Open));
            }
            catch (Exception exception1)
            {
                ProjectData.SetProjectError(exception1);
                Exception exception = exception1;
                result2.ProblemAdd(exception.Message);
                clsResult result = result2;
                ProjectData.ClearProjectError();
                return result;
            }
            result2.Take(this.Read_FME(reader));
            reader.Close();
            return result2;
        }

        public clsResult Load_Game(string Path)
        {
            sCreateWZObjectsArgs args;
            string str2;
            clsResult result2 = new clsResult("Loading game file from \"" + Path + "\"");
            this.Tileset = null;
            this.TileType_Reset();
            this.SetPainterToDefaults();
            modProgram.sSplitPath path = new modProgram.sSplitPath(Path);
            string str = path.FilePath + path.FileTitleWithoutExtension + Conversions.ToString(modProgram.PlatformPathSeparator);
            FileStream output = null;
            modProgram.sResult result3 = modIO.TryOpenFileStream(Path, ref output);
            if (!result3.Success)
            {
                result2.ProblemAdd("Game file not found: " + result3.Problem);
                return result2;
            }
            BinaryReader file = new BinaryReader(output);
            result3 = this.Read_WZ_gam(file);
            file.Close();
            if (!result3.Success)
            {
                result2.ProblemAdd(result3.Problem);
                return result2;
            }
            if (!modIO.TryOpenFileStream(str + "game.map", ref output).Success)
            {
                if (Interaction.MsgBox("game.map file not found at " + str + "\r\nDo you want to select another directory to load the underlying map from?", MsgBoxStyle.Question | MsgBoxStyle.OkCancel, null) != MsgBoxResult.Ok)
                {
                    result2.ProblemAdd("Aborted.");
                    return result2;
                }
                FolderBrowserDialog dialog = new FolderBrowserDialog {
                    SelectedPath = str
                };
                if (dialog.ShowDialog() != DialogResult.OK)
                {
                    result2.ProblemAdd("Aborted.");
                    return result2;
                }
                str2 = dialog.SelectedPath + Conversions.ToString(modProgram.PlatformPathSeparator);
                result3 = modIO.TryOpenFileStream(str2 + "game.map", ref output);
                if (!result3.Success)
                {
                    result2.ProblemAdd("game.map file not found: " + result3.Problem);
                    return result2;
                }
            }
            else
            {
                str2 = str;
            }
            BinaryReader reader = new BinaryReader(output);
            result3 = this.Read_WZ_map(reader);
            reader.Close();
            if (!result3.Success)
            {
                result2.ProblemAdd(result3.Problem);
                return result2;
            }
            modLists.SimpleClassList<clsWZBJOUnit> wZUnits = new modLists.SimpleClassList<clsWZBJOUnit>();
            clsINIFeatures translator = null;
            if (modIO.TryOpenFileStream(str + "feature.ini", ref output).Success)
            {
                clsResult result5 = new clsResult("feature.ini");
                clsINIRead read = new clsINIRead();
                StreamReader reader3 = new StreamReader(output);
                result5.Take(read.ReadFile(reader3));
                reader3.Close();
                translator = new clsINIFeatures(read.Sections.Count);
                result5.Take(read.Translate(translator));
                result2.Add(result5);
            }
            if (translator == null)
            {
                clsResult result6 = new clsResult("feat.bjo");
                if (!modIO.TryOpenFileStream(str + "feat.bjo", ref output).Success)
                {
                    result6.WarningAdd("file not found");
                }
                else
                {
                    BinaryReader reader4 = new BinaryReader(output);
                    result3 = this.Read_WZ_Features(reader4, wZUnits);
                    reader4.Close();
                    if (!result3.Success)
                    {
                        result6.WarningAdd(result3.Problem);
                    }
                }
                result2.Add(result6);
            }
            clsResult resultToAdd = new clsResult("ttypes.ttp");
            if (!modIO.TryOpenFileStream(str2 + "ttypes.ttp", ref output).Success)
            {
                resultToAdd.WarningAdd("file not found");
            }
            else
            {
                BinaryReader reader5 = new BinaryReader(output);
                result3 = this.Read_WZ_TileTypes(reader5);
                reader5.Close();
                if (!result3.Success)
                {
                    resultToAdd.WarningAdd(result3.Problem);
                }
            }
            result2.Add(resultToAdd);
            clsINIStructures structures = null;
            if (modIO.TryOpenFileStream(str + "struct.ini", ref output).Success)
            {
                clsResult result8 = new clsResult("struct.ini");
                clsINIRead read2 = new clsINIRead();
                StreamReader reader6 = new StreamReader(output);
                result8.Take(read2.ReadFile(reader6));
                reader6.Close();
                structures = new clsINIStructures(read2.Sections.Count, this);
                result8.Take(read2.Translate(structures));
                result2.Add(result8);
            }
            if (structures == null)
            {
                clsResult result9 = new clsResult("struct.bjo");
                if (!modIO.TryOpenFileStream(str + "struct.bjo", ref output).Success)
                {
                    result9.WarningAdd("struct.bjo file not found.");
                }
                else
                {
                    BinaryReader reader7 = new BinaryReader(output);
                    result3 = this.Read_WZ_Structures(reader7, ref wZUnits);
                    reader7.Close();
                    if (!result3.Success)
                    {
                        result9.WarningAdd(result3.Problem);
                    }
                }
                result2.Add(result9);
            }
            clsINIDroids droids = null;
            if (modIO.TryOpenFileStream(str + "droid.ini", ref output).Success)
            {
                clsResult result10 = new clsResult("droid.ini");
                clsINIRead read3 = new clsINIRead();
                StreamReader reader8 = new StreamReader(output);
                result10.Take(read3.ReadFile(reader8));
                reader8.Close();
                droids = new clsINIDroids(read3.Sections.Count, this);
                result10.Take(read3.Translate(droids));
                result2.Add(result10);
            }
            if (structures == null)
            {
                clsResult result11 = new clsResult("dinit.bjo");
                if (!modIO.TryOpenFileStream(str + "dinit.bjo", ref output).Success)
                {
                    result11.WarningAdd("dinit.bjo file not found.");
                }
                else
                {
                    BinaryReader reader9 = new BinaryReader(output);
                    result3 = this.Read_WZ_Droids(reader9, wZUnits);
                    reader9.Close();
                    if (!result3.Success)
                    {
                        result11.WarningAdd(result3.Problem);
                    }
                }
                result2.Add(result11);
            }
            args.BJOUnits = wZUnits;
            args.INIStructures = structures;
            args.INIDroids = droids;
            args.INIFeatures = translator;
            result2.Add(this.CreateWZObjects(args));
            if (modIO.TryOpenFileStream(str + "labels.ini", ref output).Success)
            {
                clsResult result12 = new clsResult("labels.ini");
                clsINIRead iNI = new clsINIRead();
                StreamReader reader10 = new StreamReader(output);
                result12.Take(iNI.ReadFile(reader10));
                reader10.Close();
                result12.Take(this.Read_WZ_Labels(iNI, false));
                result2.Add(result12);
            }
            return result2;
        }

        public clsResult Load_LND(string Path)
        {
            clsResult result;
            clsResult output = new clsResult("Loading LND from \"" + Path + "\"");
            try
            {
                int num;
                clsLNDObject current;
                clsGateway gateway;
                sLNDTile[] tileArray;
                int num8;
                modMath.sXY_int _int;
                BinaryReader reader;
                int num10;
                int num11;
                int num12;
                int num14;
                IEnumerator enumerator;
                IEnumerator enumerator2;
                IEnumerator enumerator3;
                modLists.SimpleList<clsLNDObject> list3 = new modLists.SimpleList<clsLNDObject>();
                clsUnitAdd add = new clsUnitAdd {
                    Map = this
                };
                try
                {
                    reader = new BinaryReader(new FileStream(Path, FileMode.Open), modProgram.UTF8Encoding);
                }
                catch (Exception exception1)
                {
                    ProjectData.SetProjectError(exception1);
                    Exception exception = exception1;
                    output.ProblemAdd(exception.Message);
                    result = output;
                    ProjectData.ClearProjectError();
                    return result;
                }
                modLists.SimpleList<string> list = modIO.BytesToLinesRemoveComments(reader);
                reader.Close();
                tileArray = (sLNDTile[]) Utils.CopyArray((Array) tileArray, new sLNDTile[(list.Count - 1) + 1]);
                byte[] buffer = new byte[0];
                string[] strArray2 = new string[11];
                string[] strArray = new string[4];
                string[] strArray3 = new string[0x100];
                modLists.SimpleList<clsGateway> list2 = new modLists.SimpleList<clsGateway>();
                for (int i = 0; i < list.Count; i++)
                {
                    string str = list[i];
                    if (Strings.InStr(1, str, "TileWidth ", CompareMethod.Binary) != 0)
                    {
                    }
                    if (Strings.InStr(1, str, "TileHeight ", CompareMethod.Binary) != 0)
                    {
                    }
                    num = Strings.InStr(1, str, "MapWidth ", CompareMethod.Binary);
                    if (num != 0)
                    {
                        modIO.InvariantParse_int(Strings.Right(str, Strings.Len(str) - (num + 8)), ref _int.X);
                    }
                    else
                    {
                        num = Strings.InStr(1, str, "MapHeight ", CompareMethod.Binary);
                        if (num != 0)
                        {
                            modIO.InvariantParse_int(Strings.Right(str, Strings.Len(str) - (num + 9)), ref _int.Y);
                        }
                        else
                        {
                            string str2;
                            if (Strings.InStr(1, str, "Textures {", CompareMethod.Binary) != 0)
                            {
                                i++;
                                str = list[i];
                                str2 = Strings.LCase(str);
                                if (Strings.InStr(1, str2, "tertilesc1", CompareMethod.Binary) > 0)
                                {
                                    this.Tileset = modProgram.Tileset_Arizona;
                                }
                                else if (Strings.InStr(1, str2, "tertilesc2", CompareMethod.Binary) > 0)
                                {
                                    this.Tileset = modProgram.Tileset_Urban;
                                }
                                else if (Strings.InStr(1, str2, "tertilesc3", CompareMethod.Binary) > 0)
                                {
                                    this.Tileset = modProgram.Tileset_Rockies;
                                }
                            }
                            else
                            {
                                int num3;
                                bool flag6;
                                if (!((Strings.InStr(1, str, "Tiles {", CompareMethod.Binary) == 0) | flag6))
                                {
                                    i++;
                                    while (i < list.Count)
                                    {
                                        str = list[i];
                                        if (Strings.InStr(1, str, "}", CompareMethod.Binary) == 0)
                                        {
                                            num = Strings.InStr(1, str, "TID ", CompareMethod.Binary);
                                            if (num == 0)
                                            {
                                                output.ProblemAdd("Tile ID missing");
                                                return output;
                                            }
                                            str2 = Strings.Right(str, (str.Length - num) - 3);
                                            num = Strings.InStr(1, str2, " ", CompareMethod.Binary);
                                            if (num > 0)
                                            {
                                                str2 = Strings.Left(str2, num - 1);
                                            }
                                            modIO.InvariantParse_short(str2, ref tileArray[num10].TID);
                                            num = Strings.InStr(1, str, "VF ", CompareMethod.Binary);
                                            if (num == 0)
                                            {
                                                output.ProblemAdd("Tile VF missing");
                                                return output;
                                            }
                                            str2 = Strings.Right(str, (str.Length - num) - 2);
                                            num = Strings.InStr(1, str2, " ", CompareMethod.Binary);
                                            if (num > 0)
                                            {
                                                str2 = Strings.Left(str2, num - 1);
                                            }
                                            modIO.InvariantParse_short(str2, ref tileArray[num10].VF);
                                            num = Strings.InStr(1, str, "TF ", CompareMethod.Binary);
                                            if (num == 0)
                                            {
                                                output.ProblemAdd("Tile TF missing");
                                                return output;
                                            }
                                            str2 = Strings.Right(str, (str.Length - num) - 2);
                                            num = Strings.InStr(1, str2, " ", CompareMethod.Binary);
                                            if (num > 0)
                                            {
                                                str2 = Strings.Left(str2, num - 1);
                                            }
                                            modIO.InvariantParse_short(str2, ref tileArray[num10].TF);
                                            num = Strings.InStr(1, str, " F ", CompareMethod.Binary);
                                            if (num == 0)
                                            {
                                                output.ProblemAdd("Tile flip missing");
                                                return output;
                                            }
                                            str2 = Strings.Right(str, (str.Length - num) - 2);
                                            num = Strings.InStr(1, str2, " ", CompareMethod.Binary);
                                            if (num > 0)
                                            {
                                                str2 = Strings.Left(str2, num - 1);
                                            }
                                            modIO.InvariantParse_short(str2, ref tileArray[num10].F);
                                            num = Strings.InStr(1, str, " VH ", CompareMethod.Binary);
                                            if (num == 0)
                                            {
                                                output.ProblemAdd("Tile height is missing");
                                                return output;
                                            }
                                            string str3 = Strings.Right(str, (Strings.Len(str) - num) - 3);
                                            num = 0;
                                            do
                                            {
                                                num3 = Strings.InStr(1, str3, " ", CompareMethod.Binary);
                                                if (num3 == 0)
                                                {
                                                    output.ProblemAdd("A tile height value is missing");
                                                    return output;
                                                }
                                                str2 = Strings.Left(str3, num3 - 1);
                                                str3 = Strings.Right(str3, Strings.Len(str3) - num3);
                                                switch (num)
                                                {
                                                    case 0:
                                                        modIO.InvariantParse_short(str2, ref tileArray[num10].Vertex0Height);
                                                        break;

                                                    case 1:
                                                        modIO.InvariantParse_short(str2, ref tileArray[num10].Vertex1Height);
                                                        break;

                                                    case 2:
                                                        modIO.InvariantParse_short(str2, ref tileArray[num10].Vertex2Height);
                                                        break;
                                                }
                                                num++;
                                            }
                                            while (num <= 2);
                                            modIO.InvariantParse_short(str3, ref tileArray[num10].Vertex3Height);
                                            num10++;
                                        }
                                        else
                                        {
                                            flag6 = true;
                                            goto Label_0B6C;
                                        }
                                        i++;
                                    }
                                    flag6 = true;
                                }
                                else
                                {
                                    int num4;
                                    bool flag4;
                                    bool flag5;
                                    string[] strArray4;
                                    if (!((Strings.InStr(1, str, "Objects {", CompareMethod.Binary) == 0) | flag4))
                                    {
                                        i++;
                                        while (i < list.Count)
                                        {
                                            str = list[i];
                                            if (Strings.InStr(1, str, "}", CompareMethod.Binary) == 0)
                                            {
                                                double num6;
                                                num4 = 0;
                                                strArray2[0] = "";
                                                flag5 = false;
                                                int num13 = str.Length - 1;
                                                num3 = 0;
                                                while (num3 <= num13)
                                                {
                                                    if ((Conversions.ToString(str[num3]) != " ") & (str[num3] != '\t'))
                                                    {
                                                        flag5 = true;
                                                        strArray4 = strArray2;
                                                        num14 = num4;
                                                        strArray4[num14] = strArray4[num14] + Conversions.ToString(str[num3]);
                                                    }
                                                    else if (flag5)
                                                    {
                                                        num4++;
                                                        if (num4 == 11)
                                                        {
                                                            output.ProblemAdd("Too many fields for an object, or a space at the end.");
                                                            return output;
                                                        }
                                                        strArray2[num4] = "";
                                                        flag5 = false;
                                                    }
                                                    num3++;
                                                }
                                                clsLNDObject newItem = new clsLNDObject();
                                                modIO.InvariantParse_uint(strArray2[0], ref newItem.ID);
                                                modIO.InvariantParse_int(strArray2[1], ref newItem.TypeNum);
                                                newItem.Code = Strings.Mid(strArray2[2], 2, strArray2[2].Length - 2);
                                                modIO.InvariantParse_int(strArray2[3], ref newItem.PlayerNum);
                                                newItem.Name = Strings.Mid(strArray2[4], 2, strArray2[4].Length - 2);
                                                modIO.InvariantParse_sng(strArray2[5], ref newItem.Pos.X);
                                                modIO.InvariantParse_sng(strArray2[6], ref newItem.Pos.Y);
                                                modIO.InvariantParse_sng(strArray2[7], ref newItem.Pos.Z);
                                                if (modIO.InvariantParse_dbl(strArray2[8], ref num6))
                                                {
                                                    newItem.Rotation.X = (int) Math.Round(modMath.Clamp_dbl(num6, 0.0, 359.0));
                                                }
                                                if (modIO.InvariantParse_dbl(strArray2[9], ref num6))
                                                {
                                                    newItem.Rotation.Y = (int) Math.Round(modMath.Clamp_dbl(num6, 0.0, 359.0));
                                                }
                                                if (modIO.InvariantParse_dbl(strArray2[10], ref num6))
                                                {
                                                    newItem.Rotation.Z = (int) Math.Round(modMath.Clamp_dbl(num6, 0.0, 359.0));
                                                }
                                                list3.Add(newItem);
                                            }
                                            else
                                            {
                                                flag4 = true;
                                                goto Label_0B6C;
                                            }
                                            i++;
                                        }
                                        flag4 = true;
                                    }
                                    else
                                    {
                                        bool flag3;
                                        if (!((Strings.InStr(1, str, "Gates {", CompareMethod.Binary) == 0) | flag3))
                                        {
                                            i++;
                                            while (i < list.Count)
                                            {
                                                str = list[i];
                                                if (Strings.InStr(1, str, "}", CompareMethod.Binary) == 0)
                                                {
                                                    num4 = 0;
                                                    strArray[0] = "";
                                                    flag5 = false;
                                                    int num15 = str.Length - 1;
                                                    num3 = 0;
                                                    while (num3 <= num15)
                                                    {
                                                        if ((Conversions.ToString(str[num3]) != " ") & (str[num3] != '\t'))
                                                        {
                                                            flag5 = true;
                                                            strArray4 = strArray;
                                                            num14 = num4;
                                                            strArray4[num14] = strArray4[num14] + Conversions.ToString(str[num3]);
                                                        }
                                                        else if (flag5)
                                                        {
                                                            num4++;
                                                            if (num4 == 4)
                                                            {
                                                                output.ProblemAdd("Too many fields for a gateway, or a space at the end.");
                                                                return output;
                                                            }
                                                            strArray[num4] = "";
                                                            flag5 = false;
                                                        }
                                                        num3++;
                                                    }
                                                    gateway = new clsGateway();
                                                    clsGateway gateway2 = gateway;
                                                    modIO.InvariantParse_int(strArray[0], ref gateway2.PosA.X);
                                                    gateway2.PosA.X = Math.Max(gateway2.PosA.X, 0);
                                                    modIO.InvariantParse_int(strArray[1], ref gateway2.PosA.Y);
                                                    gateway2.PosA.Y = Math.Max(gateway2.PosA.Y, 0);
                                                    modIO.InvariantParse_int(strArray[2], ref gateway2.PosB.X);
                                                    gateway2.PosB.X = Math.Max(gateway2.PosB.X, 0);
                                                    modIO.InvariantParse_int(strArray[3], ref gateway2.PosB.Y);
                                                    gateway2.PosB.Y = Math.Max(gateway2.PosB.Y, 0);
                                                    gateway2 = null;
                                                    list2.Add(gateway);
                                                }
                                                else
                                                {
                                                    flag3 = true;
                                                    goto Label_0B6C;
                                                }
                                                i++;
                                            }
                                            flag3 = true;
                                        }
                                        else
                                        {
                                            bool flag7;
                                            if (!(((Strings.InStr(1, str, "Tiles {", CompareMethod.Binary) == 0) | flag7) | !flag6))
                                            {
                                                i++;
                                                while (i < list.Count)
                                                {
                                                    str = list[i];
                                                    if (Strings.InStr(1, str, "}", CompareMethod.Binary) == 0)
                                                    {
                                                        num4 = 0;
                                                        strArray3[0] = "";
                                                        flag5 = false;
                                                        int num16 = str.Length - 1;
                                                        for (num3 = 0; num3 <= num16; num3++)
                                                        {
                                                            if ((Conversions.ToString(str[num3]) != " ") & (str[num3] != '\t'))
                                                            {
                                                                flag5 = true;
                                                                strArray4 = strArray3;
                                                                num14 = num4;
                                                                strArray4[num14] = strArray4[num14] + Conversions.ToString(str[num3]);
                                                            }
                                                            else if (flag5)
                                                            {
                                                                num4++;
                                                                if (num4 == 0x100)
                                                                {
                                                                    output.ProblemAdd("Too many fields for tile types.");
                                                                    return output;
                                                                }
                                                                strArray3[num4] = "";
                                                                flag5 = false;
                                                            }
                                                        }
                                                        if ((strArray3[num4] == "") | (strArray3[num4] == " "))
                                                        {
                                                            num4--;
                                                        }
                                                        int num17 = num4;
                                                        for (int j = 0; j <= num17; j++)
                                                        {
                                                            buffer = (byte[]) Utils.CopyArray((Array) buffer, new byte[num8 + 1]);
                                                            buffer[num8] = Math.Min(Conversions.ToByte(strArray3[j]), 11);
                                                            num8++;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        flag7 = true;
                                                        goto Label_0B6C;
                                                    }
                                                    i++;
                                                }
                                                flag7 = true;
                                            }
                                        }
                                    }
                                }
                            Label_0B6C:;
                            }
                        }
                    }
                }
                tileArray = (sLNDTile[]) Utils.CopyArray((Array) tileArray, new sLNDTile[(num10 - 1) + 1]);
                this.SetPainterToDefaults();
                if ((_int.X < 1) | (_int.Y < 1))
                {
                    output.ProblemAdd("The LND's terrain dimensions are missing or invalid.");
                    return output;
                }
                this.TerrainBlank(_int);
                this.TileType_Reset();
                int num18 = this.Terrain.TileSize.Y - 1;
                for (num12 = 0; num12 <= num18; num12++)
                {
                    int num19 = this.Terrain.TileSize.X - 1;
                    num11 = 0;
                    while (num11 <= num19)
                    {
                        num10 = (num12 * this.Terrain.TileSize.X) + num11;
                        this.Terrain.Vertices[num11, num12].Height = (byte) tileArray[num10].Vertex0Height;
                        num11++;
                    }
                }
                int num20 = this.Terrain.TileSize.Y - 1;
                for (num12 = 0; num12 <= num20; num12++)
                {
                    int num21 = this.Terrain.TileSize.X - 1;
                    for (num11 = 0; num11 <= num21; num11++)
                    {
                        num10 = (num12 * this.Terrain.TileSize.X) + num11;
                        this.Terrain.Tiles[num11, num12].Texture.TextureNum = tileArray[num10].TID - 1;
                        num = (int) Math.Round(((double) (((double) tileArray[num10].F) / 64.0)));
                        tileArray[num10].F = (short) (tileArray[num10].F - (num * 0x40));
                        num = (int) Math.Round(((double) (((double) tileArray[num10].F) / 16.0)));
                        tileArray[num10].F = (short) (tileArray[num10].F - (num * 0x10));
                        if ((num < 0) | (num > 3))
                        {
                            output.ProblemAdd("Invalid flip value.");
                            return output;
                        }
                        byte oldRotation = (byte) num;
                        num = (int) Math.Round(((double) (((double) tileArray[num10].F) / 8.0)));
                        sLNDTile[] tileArray2 = tileArray;
                        num14 = num10;
                        tileArray2[num14].F = (short) (tileArray2[num14].F - ((short) (num * 8)));
                        bool oldFlipZ = num == 1;
                        num = (int) Math.Round(((double) (((double) tileArray[num10].F) / 4.0)));
                        tileArray2 = tileArray;
                        num14 = num10;
                        tileArray2[num14].F = (short) (tileArray2[num14].F - ((short) (num * 4)));
                        bool oldFlipX = num == 1;
                        num = (int) Math.Round(((double) (((double) tileArray[num10].F) / 2.0)));
                        tileArray2 = tileArray;
                        num14 = num10;
                        tileArray2[num14].F = (short) (tileArray2[num14].F - ((short) (num * 2)));
                        this.Terrain.Tiles[num11, num12].Tri = num == 1;
                        TileOrientation.OldOrientation_To_TileOrientation(oldRotation, oldFlipX, oldFlipZ, ref this.Terrain.Tiles[num11, num12].Texture.Orientation);
                    }
                }
                uint num2 = 1;
                try
                {
                    enumerator = list3.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        current = (clsLNDObject) enumerator.Current;
                        if (current.ID >= num2)
                        {
                            num2 = current.ID + 1;
                        }
                    }
                }
                finally
                {
                    if (enumerator is IDisposable)
                    {
                        (enumerator as IDisposable).Dispose();
                    }
                }
                try
                {
                    enumerator2 = list3.GetEnumerator();
                    while (enumerator2.MoveNext())
                    {
                        clsUnitType type;
                        current = (clsLNDObject) enumerator2.Current;
                        switch (current.TypeNum)
                        {
                            case 0:
                                type = modProgram.ObjectData.FindOrCreateUnitType(current.Code, clsUnitType.enumType.Feature, -1);
                                break;

                            case 1:
                                type = modProgram.ObjectData.FindOrCreateUnitType(current.Code, clsUnitType.enumType.PlayerStructure, -1);
                                break;

                            case 2:
                                type = modProgram.ObjectData.FindOrCreateUnitType(current.Code, clsUnitType.enumType.PlayerDroid, -1);
                                break;

                            default:
                                type = null;
                                break;
                        }
                        if (type != null)
                        {
                            modMath.sXYZ_int _int2;
                            clsUnit iDUnit = new clsUnit {
                                Type = type
                            };
                            if ((current.PlayerNum < 0) | (current.PlayerNum >= 10))
                            {
                                iDUnit.UnitGroup = this.ScavengerUnitGroup;
                            }
                            else
                            {
                                iDUnit.UnitGroup = this.UnitGroups[current.PlayerNum];
                            }
                            _int2.X = (int) Math.Round((double) current.Pos.X);
                            _int2.Y = (int) Math.Round((double) current.Pos.Y);
                            _int2.Z = (int) Math.Round((double) current.Pos.Z);
                            iDUnit.Pos = this.MapPos_From_LNDPos(_int2);
                            iDUnit.Rotation = current.Rotation.Y;
                            if (current.ID == 0)
                            {
                                current.ID = num2;
                                modProgram.ZeroIDWarning(iDUnit, current.ID, output);
                            }
                            add.NewUnit = iDUnit;
                            add.ID = current.ID;
                            add.Perform();
                            modProgram.ErrorIDChange(current.ID, iDUnit, "Load_LND");
                            if (num2 == current.ID)
                            {
                                num2 = iDUnit.ID + 1;
                            }
                        }
                    }
                }
                finally
                {
                    if (enumerator2 is IDisposable)
                    {
                        (enumerator2 as IDisposable).Dispose();
                    }
                }
                try
                {
                    enumerator3 = list2.GetEnumerator();
                    while (enumerator3.MoveNext())
                    {
                        gateway = (clsGateway) enumerator3.Current;
                        this.GatewayCreate(gateway.PosA, gateway.PosB);
                    }
                }
                finally
                {
                    if (enumerator3 is IDisposable)
                    {
                        (enumerator3 as IDisposable).Dispose();
                    }
                }
                if (this.Tileset != null)
                {
                    int num23 = Math.Min(num8 - 1, this.Tileset.TileCount) - 1;
                    for (num = 0; num <= num23; num++)
                    {
                        this.Tile_TypeNum[num] = buffer[num + 1];
                    }
                }
                return output;
            }
            catch (Exception exception3)
            {
                ProjectData.SetProjectError(exception3);
                Exception exception2 = exception3;
                output.ProblemAdd(exception2.Message);
                result = output;
                ProjectData.ClearProjectError();
                return result;
            }
            return output;
        }

        public modProgram.sResult Load_TTP(string Path)
        {
            BinaryReader reader;
            modProgram.sResult result2;
            result2.Success = false;
            result2.Problem = "";
            try
            {
                reader = new BinaryReader(new FileStream(Path, FileMode.Open));
            }
            catch (Exception exception1)
            {
                ProjectData.SetProjectError(exception1);
                Exception exception = exception1;
                result2.Problem = exception.Message;
                modProgram.sResult result = result2;
                ProjectData.ClearProjectError();
                return result;
            }
            result2 = this.Read_TTP(reader);
            reader.Close();
            return result2;
        }

        public clsResult Load_WZ(string Path)
        {
            int num;
            sCreateWZObjectsArgs args;
            FileStream stream;
            string name;
            ZipEntry entry;
            clsResult result2 = new clsResult("Loading WZ from \"" + Path + "\"");
            string str4 = "\"";
            modLists.SimpleList<clsWZMapEntry> list2 = new modLists.SimpleList<clsWZMapEntry>();
            clsTileset tileset = null;
            string str2 = "";
            try
            {
                stream = File.OpenRead(Path);
            }
            catch (Exception exception1)
            {
                ProjectData.SetProjectError(exception1);
                Exception exception = exception1;
                result2.ProblemAdd(exception.Message);
                clsResult result = result2;
                ProjectData.ClearProjectError();
                return result;
            }
            ZipInputStream input = new ZipInputStream(stream);
        Label_006D:
            entry = input.GetNextEntry();
            if (entry != null)
            {
                modProgram.sZipSplitPath path2 = new modProgram.sZipSplitPath(entry.Name);
                if ((path2.FileExtension == "lev") & (path2.PartCount == 1))
                {
                    if (entry.Size > 0xa00000L)
                    {
                        result2.ProblemAdd("lev file is too large.");
                        input.Close();
                        return result2;
                    }
                    BinaryReader reader = new BinaryReader(input);
                    modLists.SimpleList<string> list3 = modIO.BytesToLinesRemoveComments(reader);
                    int num5 = list3.Count - 1;
                    for (num = 0; num <= num5; num++)
                    {
                        if (Strings.LCase(Strings.Left(list3[num], 5)) != "level")
                        {
                            continue;
                        }
                        bool flag2 = false;
                        int num2 = 1;
                        while ((num + num2) < list3.Count)
                        {
                            if (Strings.LCase(Strings.Left(list3[num + num2], 4)) == "game")
                            {
                                int num3 = Strings.InStr(list3[num + num2], str4, CompareMethod.Binary);
                                int num4 = Strings.InStrRev(list3[num + num2], str4, -1, CompareMethod.Binary);
                                if (((num3 > 0) & (num4 > 0)) & ((num4 - num3) > 1))
                                {
                                    str2 = Strings.LCase(Strings.Mid(list3[num + num2], num3 + 1, (num4 - num3) - 1));
                                    int num6 = list2.Count - 1;
                                    num3 = 0;
                                    while (num3 <= num6)
                                    {
                                        if (str2 == list2[num3].Name)
                                        {
                                            break;
                                        }
                                        num3++;
                                    }
                                    if (num3 == list2.Count)
                                    {
                                        flag2 = true;
                                    }
                                }
                                break;
                            }
                            if (Strings.LCase(Strings.Left(list3[num + num2], 5)) == "level")
                            {
                                break;
                            }
                            num2++;
                        }
                        if (flag2)
                        {
                            bool flag = false;
                            for (num2 = 1; (num + num2) < list3.Count; num2++)
                            {
                                if (Strings.LCase(Strings.Left(list3[num + num2], 7)) == "dataset")
                                {
                                    switch (Strings.LCase(Strings.Right(list3[num + num2], 1)))
                                    {
                                        case "1":
                                            tileset = modProgram.Tileset_Arizona;
                                            flag = true;
                                            break;

                                        case "2":
                                            tileset = modProgram.Tileset_Urban;
                                            flag = true;
                                            break;

                                        case "3":
                                            tileset = modProgram.Tileset_Rockies;
                                            flag = true;
                                            break;
                                    }
                                    break;
                                }
                                if (Strings.LCase(Strings.Left(list3[num + num2], 5)) == "level")
                                {
                                    break;
                                }
                            }
                            if (flag)
                            {
                                clsWZMapEntry newItem = new clsWZMapEntry {
                                    Name = str2,
                                    Tileset = tileset
                                };
                                list2.Add(newItem);
                            }
                        }
                    }
                }
                goto Label_006D;
            }
            input.Close();
            if (list2.Count < 1)
            {
                result2.ProblemAdd("No maps found in file.");
                return result2;
            }
            if (list2.Count == 1)
            {
                name = list2[0].Name;
                this.Tileset = list2[0].Tileset;
            }
            else
            {
                frmWZLoad.clsOutput newOutput = new frmWZLoad.clsOutput();
                string[] mapNames = new string[(list2.Count - 1) + 1];
                int num7 = list2.Count - 1;
                for (num = 0; num <= num7; num++)
                {
                    mapNames[num] = list2[num].Name;
                }
                modProgram.sSplitPath path4 = new modProgram.sSplitPath(Path);
                new frmWZLoad(mapNames, newOutput, "Select a map from " + path4.FileTitle).ShowDialog();
                if (newOutput.Result < 0)
                {
                    result2.ProblemAdd("No map selected.");
                    return result2;
                }
                name = list2[newOutput.Result].Name;
                this.Tileset = list2[newOutput.Result].Tileset;
            }
            this.TileType_Reset();
            this.SetPainterToDefaults();
            modProgram.sZipSplitPath path = new modProgram.sZipSplitPath(name);
            string str = path.FilePath + path.FileTitleWithoutExtension + "/";
            clsZipStreamEntry entry2 = modIO.FindZipEntryFromPath(Path, name);
            if (entry2 == null)
            {
                result2.ProblemAdd("Game file not found.");
                return result2;
            }
            BinaryReader file = new BinaryReader(entry2.Stream);
            modProgram.sResult result3 = this.Read_WZ_gam(file);
            file.Close();
            if (!result3.Success)
            {
                result2.ProblemAdd(result3.Problem);
                return result2;
            }
            entry2 = modIO.FindZipEntryFromPath(Path, str + "game.map");
            if (entry2 == null)
            {
                result2.ProblemAdd("game.map file not found");
                return result2;
            }
            BinaryReader reader3 = new BinaryReader(entry2.Stream);
            result3 = this.Read_WZ_map(reader3);
            reader3.Close();
            if (!result3.Success)
            {
                result2.ProblemAdd(result3.Problem);
                return result2;
            }
            modLists.SimpleClassList<clsWZBJOUnit> wZUnits = new modLists.SimpleClassList<clsWZBJOUnit>();
            clsINIFeatures translator = null;
            entry2 = modIO.FindZipEntryFromPath(Path, str + "feature.ini");
            if (entry2 != null)
            {
                clsResult result4 = new clsResult("feature.ini");
                clsINIRead read = new clsINIRead();
                StreamReader reader4 = new StreamReader(entry2.Stream);
                result4.Take(read.ReadFile(reader4));
                reader4.Close();
                translator = new clsINIFeatures(read.Sections.Count);
                result4.Take(read.Translate(translator));
                result2.Add(result4);
            }
            if (translator == null)
            {
                clsResult result5 = new clsResult("feat.bjo");
                entry2 = modIO.FindZipEntryFromPath(Path, str + "feat.bjo");
                if (entry2 == null)
                {
                    result5.WarningAdd("file not found");
                }
                else
                {
                    BinaryReader reader5 = new BinaryReader(entry2.Stream);
                    result3 = this.Read_WZ_Features(reader5, wZUnits);
                    reader5.Close();
                    if (!result3.Success)
                    {
                        result5.WarningAdd(result3.Problem);
                    }
                }
                result2.Add(result5);
            }
            clsResult resultToAdd = new clsResult("ttypes.ttp");
            entry2 = modIO.FindZipEntryFromPath(Path, str + "ttypes.ttp");
            if (entry2 == null)
            {
                resultToAdd.WarningAdd("file not found");
            }
            else
            {
                BinaryReader reader6 = new BinaryReader(entry2.Stream);
                result3 = this.Read_WZ_TileTypes(reader6);
                reader6.Close();
                if (!result3.Success)
                {
                    resultToAdd.WarningAdd(result3.Problem);
                }
            }
            result2.Add(resultToAdd);
            clsINIStructures structures = null;
            entry2 = modIO.FindZipEntryFromPath(Path, str + "struct.ini");
            if (entry2 != null)
            {
                clsResult result7 = new clsResult("struct.ini");
                clsINIRead read2 = new clsINIRead();
                StreamReader reader7 = new StreamReader(entry2.Stream);
                result7.Take(read2.ReadFile(reader7));
                reader7.Close();
                structures = new clsINIStructures(read2.Sections.Count, this);
                result7.Take(read2.Translate(structures));
                result2.Add(result7);
            }
            if (structures == null)
            {
                clsResult result8 = new clsResult("struct.bjo");
                entry2 = modIO.FindZipEntryFromPath(Path, str + "struct.bjo");
                if (entry2 == null)
                {
                    result8.WarningAdd("file not found");
                }
                else
                {
                    BinaryReader reader8 = new BinaryReader(entry2.Stream);
                    result3 = this.Read_WZ_Structures(reader8, ref wZUnits);
                    reader8.Close();
                    if (!result3.Success)
                    {
                        result8.WarningAdd(result3.Problem);
                    }
                }
                result2.Add(result8);
            }
            clsINIDroids droids = null;
            entry2 = modIO.FindZipEntryFromPath(Path, str + "droid.ini");
            if (entry2 != null)
            {
                clsResult result9 = new clsResult("droid.ini");
                clsINIRead read3 = new clsINIRead();
                StreamReader reader9 = new StreamReader(entry2.Stream);
                result9.Take(read3.ReadFile(reader9));
                reader9.Close();
                droids = new clsINIDroids(read3.Sections.Count, this);
                result9.Take(read3.Translate(droids));
                result2.Add(result9);
            }
            if (droids == null)
            {
                clsResult result10 = new clsResult("dinit.bjo");
                entry2 = modIO.FindZipEntryFromPath(Path, str + "dinit.bjo");
                if (entry2 == null)
                {
                    result10.WarningAdd("file not found");
                }
                else
                {
                    BinaryReader reader10 = new BinaryReader(entry2.Stream);
                    result3 = this.Read_WZ_Droids(reader10, wZUnits);
                    reader10.Close();
                    if (!result3.Success)
                    {
                        result10.WarningAdd(result3.Problem);
                    }
                }
                result2.Add(result10);
            }
            args.BJOUnits = wZUnits;
            args.INIStructures = structures;
            args.INIDroids = droids;
            args.INIFeatures = translator;
            result2.Add(this.CreateWZObjects(args));
            entry2 = modIO.FindZipEntryFromPath(Path, str + "labels.ini");
            if (entry2 != null)
            {
                clsResult result11 = new clsResult("labels.ini");
                clsINIRead iNI = new clsINIRead();
                StreamReader reader11 = new StreamReader(entry2.Stream);
                result11.Take(iNI.ReadFile(reader11));
                reader11.Close();
                result11.Take(this.Read_WZ_Labels(iNI, false));
                result2.Add(result11);
            }
            return result2;
        }

        public void MakeDefaultUnitGroups()
        {
            this.UnitGroups.Clear();
            int num = 0;
            do
            {
                clsUnitGroup group = new clsUnitGroup {
                    WZ_StartPos = num
                };
                group.MapLink.Connect(this.UnitGroups);
                num++;
            }
            while (num <= 9);
            this.ScavengerUnitGroup = new clsUnitGroup();
            this.ScavengerUnitGroup.MapLink.Connect(this.UnitGroups);
            this.ScavengerUnitGroup.WZ_StartPos = -1;
        }

        public void MapInsert(clsMap MapToInsert, modMath.sXY_int Offset, modMath.sXY_int Area, bool InsertHeights, bool InsertTextures, bool InsertUnits, bool DeleteUnits, bool InsertGateways, bool DeleteGateways)
        {
            modMath.sXY_int _int;
            modMath.sXY_int _int2;
            modMath.sXY_int _int4;
            modMath.sXY_int _int6;
            int num;
            int num2;
            _int2.X = Math.Min(Offset.X + Math.Min(Area.X, MapToInsert.Terrain.TileSize.X), this.Terrain.TileSize.X);
            _int2.Y = Math.Min(Offset.Y + Math.Min(Area.Y, MapToInsert.Terrain.TileSize.Y), this.Terrain.TileSize.Y);
            _int.X = _int2.X - Offset.X;
            _int.Y = _int2.Y - Offset.Y;
            modMath.sXY_int startTile = new modMath.sXY_int(Offset.X - 1, Offset.Y - 1);
            this.GetTileSectorRange(startTile, _int2, ref _int6, ref _int4);
            int y = _int4.Y;
            for (num2 = _int6.Y; num2 <= y; num2++)
            {
                modMath.sXY_int _int5;
                _int5.Y = num2;
                int x = _int4.X;
                num = _int6.X;
                while (num <= x)
                {
                    _int5.X = num;
                    this.SectorGraphicsChanges.Changed(_int5);
                    this.SectorUnitHeightsChanges.Changed(_int5);
                    this.SectorTerrainUndoChanges.Changed(_int5);
                    num++;
                }
            }
            if (InsertHeights)
            {
                int num7 = _int.Y;
                for (num2 = 0; num2 <= num7; num2++)
                {
                    int num8 = _int.X;
                    num = 0;
                    while (num <= num8)
                    {
                        this.Terrain.Vertices[Offset.X + num, Offset.Y + num2].Height = MapToInsert.Terrain.Vertices[num, num2].Height;
                        num++;
                    }
                }
                int num9 = _int.Y - 1;
                for (num2 = 0; num2 <= num9; num2++)
                {
                    int num10 = _int.X - 1;
                    num = 0;
                    while (num <= num10)
                    {
                        this.Terrain.Tiles[Offset.X + num, Offset.Y + num2].Tri = MapToInsert.Terrain.Tiles[num, num2].Tri;
                        num++;
                    }
                }
            }
            if (InsertTextures)
            {
                int num11 = _int.Y;
                for (num2 = 0; num2 <= num11; num2++)
                {
                    int num12 = _int.X;
                    num = 0;
                    while (num <= num12)
                    {
                        this.Terrain.Vertices[Offset.X + num, Offset.Y + num2].Terrain = MapToInsert.Terrain.Vertices[num, num2].Terrain;
                        num++;
                    }
                }
                int num13 = _int.Y - 1;
                for (num2 = 0; num2 <= num13; num2++)
                {
                    int num14 = _int.X - 1;
                    num = 0;
                    while (num <= num14)
                    {
                        bool tri = this.Terrain.Tiles[Offset.X + num, Offset.Y + num2].Tri;
                        this.Terrain.Tiles[Offset.X + num, Offset.Y + num2].Copy(MapToInsert.Terrain.Tiles[num, num2]);
                        this.Terrain.Tiles[Offset.X + num, Offset.Y + num2].Tri = tri;
                        num++;
                    }
                }
                int num15 = _int.Y;
                for (num2 = 0; num2 <= num15; num2++)
                {
                    int num16 = _int.X - 1;
                    num = 0;
                    while (num <= num16)
                    {
                        this.Terrain.SideH[Offset.X + num, Offset.Y + num2].Road = MapToInsert.Terrain.SideH[num, num2].Road;
                        num++;
                    }
                }
                int num17 = _int.Y - 1;
                for (num2 = 0; num2 <= num17; num2++)
                {
                    int num18 = _int.X;
                    num = 0;
                    while (num <= num18)
                    {
                        this.Terrain.SideV[Offset.X + num, Offset.Y + num2].Road = MapToInsert.Terrain.SideV[num, num2].Road;
                        num++;
                    }
                }
            }
            modMath.sXY_int maximum = _int2;
            maximum.X--;
            maximum.Y--;
            if (DeleteGateways)
            {
                int num3 = 0;
                while (num3 < this.Gateways.Count)
                {
                    if (this.Gateways[num3].PosA.IsInRange(Offset, maximum) | this.Gateways[num3].PosB.IsInRange(Offset, maximum))
                    {
                        this.GatewayRemoveStoreChange(num3);
                    }
                    else
                    {
                        num3++;
                    }
                }
            }
            if (InsertGateways)
            {
                IEnumerator enumerator;
                try
                {
                    enumerator = MapToInsert.Gateways.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        modMath.sXY_int _int7;
                        modMath.sXY_int _int8;
                        clsGateway current = (clsGateway) enumerator.Current;
                        _int8.X = Offset.X + current.PosA.X;
                        _int8.Y = Offset.Y + current.PosA.Y;
                        _int7.X = Offset.X + current.PosB.X;
                        _int7.Y = Offset.Y + current.PosB.Y;
                        if (_int8.IsInRange(Offset, maximum) | _int7.IsInRange(Offset, maximum))
                        {
                            this.GatewayCreateStoreChange(_int8, _int7);
                        }
                    }
                }
                finally
                {
                    if (enumerator is IDisposable)
                    {
                        (enumerator as IDisposable).Dispose();
                    }
                }
            }
            if (DeleteUnits)
            {
                clsUnit unit;
                IEnumerator enumerator3;
                modLists.SimpleList<clsUnit> list = new modLists.SimpleList<clsUnit>();
                int num19 = _int4.Y;
                for (num2 = _int6.Y; num2 <= num19; num2++)
                {
                    int num20 = _int4.X;
                    for (num = _int6.X; num <= num20; num++)
                    {
                        IEnumerator enumerator2;
                        try
                        {
                            enumerator2 = this.Sectors[num, num2].Units.GetEnumerator();
                            while (enumerator2.MoveNext())
                            {
                                clsUnitSectorConnection connection = (clsUnitSectorConnection) enumerator2.Current;
                                unit = connection.Unit;
                                if (modProgram.PosIsWithinTileArea(unit.Pos.Horizontal, Offset, _int2))
                                {
                                    list.Add(unit);
                                }
                            }
                        }
                        finally
                        {
                            if (enumerator2 is IDisposable)
                            {
                                (enumerator2 as IDisposable).Dispose();
                            }
                        }
                    }
                }
                try
                {
                    enumerator3 = list.GetEnumerator();
                    while (enumerator3.MoveNext())
                    {
                        unit = (clsUnit) enumerator3.Current;
                        if (unit.MapLink.IsConnected)
                        {
                            this.UnitRemoveStoreChange(unit.MapLink.ArrayPosition);
                        }
                    }
                }
                finally
                {
                    if (enumerator3 is IDisposable)
                    {
                        (enumerator3 as IDisposable).Dispose();
                    }
                }
            }
            if (InsertUnits)
            {
                modMath.sXY_int _int9;
                IEnumerator enumerator4;
                modMath.sXY_int _int10 = new modMath.sXY_int(0, 0);
                clsUnitAdd add = new clsUnitAdd {
                    Map = this,
                    StoreChange = true
                };
                _int9.X = Offset.X * 0x80;
                _int9.Y = Offset.Y * 0x80;
                try
                {
                    enumerator4 = MapToInsert.Units.GetEnumerator();
                    while (enumerator4.MoveNext())
                    {
                        clsUnit unitToCopy = (clsUnit) enumerator4.Current;
                        if (modProgram.PosIsWithinTileArea(unitToCopy.Pos.Horizontal, _int10, _int))
                        {
                            clsUnit unit2 = new clsUnit(unitToCopy, this);
                            clsUnit unit4 = unit2;
                            unit4.Pos.Horizontal.X += _int9.X;
                            unit4 = unit2;
                            unit4.Pos.Horizontal.Y += _int9.Y;
                            add.NewUnit = unit2;
                            add.Label = unitToCopy.Label;
                            add.Perform();
                        }
                    }
                }
                finally
                {
                    if (enumerator4 is IDisposable)
                    {
                        (enumerator4 as IDisposable).Dispose();
                    }
                }
            }
            this.SectorsUpdateGraphics();
            this.SectorsUpdateUnitHeights();
            this.MinimapMakeLater();
        }

        public modProgram.sWorldPos MapPos_From_LNDPos(modMath.sXYZ_int Pos)
        {
            modProgram.sWorldPos pos2;
            pos2.Horizontal.X = Pos.X + ((int) Math.Round((double) (((double) (this.Terrain.TileSize.X * 0x80)) / 2.0)));
            pos2.Horizontal.Y = ((int) Math.Round((double) (((double) (this.Terrain.TileSize.Y * 0x80)) / 2.0))) - Pos.Z;
            pos2.Altitude = (int) Math.Round(this.GetTerrainHeight(pos2.Horizontal));
            return pos2;
        }

        public void MapTexturer(ref modProgram.sLayerList LayerList)
        {
            int num5;
            clsBooleanMap remove = new clsBooleanMap();
            clsBooleanMap map2 = new clsBooleanMap();
            clsBooleanMap[] mapArray = new clsBooleanMap[(LayerList.LayerCount - 1) + 1];
            clsPainter.clsTerrain[,] terrainArray = new clsPainter.clsTerrain[this.Terrain.TileSize.X + 1, this.Terrain.TileSize.Y + 1];
            float[,] numArray = new float[(this.Terrain.TileSize.X - 1) + 1, (this.Terrain.TileSize.Y - 1) + 1];
            int num7 = this.Terrain.TileSize.Y - 1;
            int num6 = 0;
            while (num6 <= num7)
            {
                int num8 = this.Terrain.TileSize.X - 1;
                num5 = 0;
                while (num5 <= num8)
                {
                    modMath.sXY_int _int;
                    double num2 = 0.0;
                    _int.X = (int) Math.Round((double) ((num5 + 0.25) * 128.0));
                    _int.Y = (int) Math.Round((double) ((num6 + 0.25) * 128.0));
                    double terrainSlopeAngle = this.GetTerrainSlopeAngle(_int);
                    if (terrainSlopeAngle > num2)
                    {
                        num2 = terrainSlopeAngle;
                    }
                    _int.X = (int) Math.Round((double) ((num5 + 0.75) * 128.0));
                    _int.Y = (int) Math.Round((double) ((num6 + 0.25) * 128.0));
                    terrainSlopeAngle = this.GetTerrainSlopeAngle(_int);
                    if (terrainSlopeAngle > num2)
                    {
                        num2 = terrainSlopeAngle;
                    }
                    _int.X = (int) Math.Round((double) ((num5 + 0.25) * 128.0));
                    _int.Y = (int) Math.Round((double) ((num6 + 0.75) * 128.0));
                    terrainSlopeAngle = this.GetTerrainSlopeAngle(_int);
                    if (terrainSlopeAngle > num2)
                    {
                        num2 = terrainSlopeAngle;
                    }
                    _int.X = (int) Math.Round((double) ((num5 + 0.75) * 128.0));
                    _int.Y = (int) Math.Round((double) ((num6 + 0.75) * 128.0));
                    terrainSlopeAngle = this.GetTerrainSlopeAngle(_int);
                    if (terrainSlopeAngle > num2)
                    {
                        num2 = terrainSlopeAngle;
                    }
                    numArray[num5, num6] = (float) num2;
                    num5++;
                }
                num6++;
            }
            int num9 = LayerList.LayerCount - 1;
            for (int i = 0; i <= num9; i++)
            {
                clsPainter.clsTerrain terrain = LayerList.Layers[i].Terrain;
                if (terrain != null)
                {
                    mapArray[i] = new clsBooleanMap();
                    mapArray[i].Copy(LayerList.Layers[i].Terrainmap);
                    if ((LayerList.Layers[i].WithinLayer >= 0) && (LayerList.Layers[i].WithinLayer < i))
                    {
                        remove.Within(mapArray[i], mapArray[LayerList.Layers[i].WithinLayer]);
                        mapArray[i].ValueData = remove.ValueData;
                        remove.ValueData = new clsBooleanMap.clsValueData();
                    }
                    int num10 = i - 1;
                    for (int j = 0; j <= num10; j++)
                    {
                        if (LayerList.Layers[i].AvoidLayers[j])
                        {
                            remove.Expand_One_Tile(mapArray[j]);
                            map2.Remove(mapArray[i], remove);
                            mapArray[i].ValueData = map2.ValueData;
                            map2.ValueData = new clsBooleanMap.clsValueData();
                        }
                    }
                    int num11 = this.Terrain.TileSize.Y;
                    num6 = 0;
                    while (num6 <= num11)
                    {
                        int x = this.Terrain.TileSize.X;
                        num5 = 0;
                        while (num5 <= x)
                        {
                            if (mapArray[i].ValueData.Value[num6, num5])
                            {
                                if ((this.Terrain.Vertices[num5, num6].Height < LayerList.Layers[i].HeightMin) | (this.Terrain.Vertices[num5, num6].Height > LayerList.Layers[i].HeightMax))
                                {
                                    mapArray[i].ValueData.Value[num6, num5] = false;
                                }
                                if (mapArray[i].ValueData.Value[num6, num5])
                                {
                                    bool flag = true;
                                    if (num5 > 0)
                                    {
                                        if ((num6 > 0) && ((numArray[num5 - 1, num6 - 1] < LayerList.Layers[i].SlopeMin) | (numArray[num5 - 1, num6 - 1] > LayerList.Layers[i].SlopeMax)))
                                        {
                                            flag = false;
                                        }
                                        if ((num6 < this.Terrain.TileSize.Y) && ((numArray[num5 - 1, num6] < LayerList.Layers[i].SlopeMin) | (numArray[num5 - 1, num6] > LayerList.Layers[i].SlopeMax)))
                                        {
                                            flag = false;
                                        }
                                    }
                                    if (num5 < this.Terrain.TileSize.X)
                                    {
                                        if ((num6 > 0) && ((numArray[num5, num6 - 1] < LayerList.Layers[i].SlopeMin) | (numArray[num5, num6 - 1] > LayerList.Layers[i].SlopeMax)))
                                        {
                                            flag = false;
                                        }
                                        if ((num6 < this.Terrain.TileSize.Y) && ((numArray[num5, num6] < LayerList.Layers[i].SlopeMin) | (numArray[num5, num6] > LayerList.Layers[i].SlopeMax)))
                                        {
                                            flag = false;
                                        }
                                    }
                                    if (!flag)
                                    {
                                        mapArray[i].ValueData.Value[num6, num5] = false;
                                    }
                                }
                            }
                            num5++;
                        }
                        num6++;
                    }
                    mapArray[i].Remove_Diagonals();
                    int num13 = this.Terrain.TileSize.Y;
                    num6 = 0;
                    while (num6 <= num13)
                    {
                        int num14 = this.Terrain.TileSize.X;
                        num5 = 0;
                        while (num5 <= num14)
                        {
                            if (mapArray[i].ValueData.Value[num6, num5])
                            {
                                terrainArray[num5, num6] = terrain;
                            }
                            num5++;
                        }
                        num6++;
                    }
                }
            }
            int y = this.Terrain.TileSize.Y;
            for (num6 = 0; num6 <= y; num6++)
            {
                int num16 = this.Terrain.TileSize.X;
                for (num5 = 0; num5 <= num16; num5++)
                {
                    if (terrainArray[num5, num6] != null)
                    {
                        this.Terrain.Vertices[num5, num6].Terrain = terrainArray[num5, num6];
                    }
                }
            }
            this.AutoTextureChanges.SetAllChanged();
            this.UpdateAutoTextures();
        }

        public void MinimapGLDelete()
        {
            if (this.Minimap_GLTexture != 0)
            {
                GL.DeleteTextures(1, ref this.Minimap_GLTexture);
                this.Minimap_GLTexture = 0;
            }
        }

        private void MinimapMake()
        {
            int num = (int) Math.Round(Math.Round(Math.Pow(2.0, Math.Ceiling((double) (Math.Log((double) Math.Max(this.Terrain.TileSize.X, this.Terrain.TileSize.Y)) / Math.Log(2.0))))));
            if (num != this.Minimap_Texture_Size)
            {
                this.Minimap_Texture_Size = num;
            }
            modMath.sXY_int size = new modMath.sXY_int(this.Minimap_Texture_Size, this.Minimap_Texture_Size);
            clsMinimapTexture texture = new clsMinimapTexture(size);
            this.MinimapTextureFill(texture);
            this.MinimapGLDelete();
            GL.GenTextures(1, out this.Minimap_GLTexture);
            GL.BindTexture(TextureTarget.Texture2D, this.Minimap_GLTexture);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, 0x812f);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, 0x812f);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, 0x2600);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, 0x2600);
            GL.TexImage2D<sRGBA_sng>(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, this.Minimap_Texture_Size, this.Minimap_Texture_Size, 0, PixelFormat.Rgba, PixelType.Float, texture.InlinePixels);
            modMain.frmMainInstance.View_DrawViewLater();
        }

        public void MinimapMakeLater()
        {
            if (this.MakeMinimapTimer.Enabled)
            {
                this.MinimapPending = true;
            }
            else
            {
                this.MakeMinimapTimer.Enabled = true;
                if (this.SuppressMinimap)
                {
                    this.MinimapPending = true;
                }
                else
                {
                    this.MinimapMake();
                }
            }
        }

        protected void MinimapTextureFill(clsMinimapTexture Texture)
        {
            float alpha;
            float num2;
            modMath.sXY_int _int2;
            modMath.sXY_int _int3;
            int num3;
            int num4;
            bool[,] flagArray = new bool[(Texture.Size.Y - 1) + 1, (Texture.Size.X - 1) + 1];
            float[,,] numArray = new float[(Texture.Size.Y - 1) + 1, (Texture.Size.X - 1) + 1, 3];
            if (modMain.frmMainInstance.menuMiniShowTex.Checked)
            {
                if (this.Tileset != null)
                {
                    int num7 = this.Terrain.TileSize.Y - 1;
                    for (num4 = 0; num4 <= num7; num4++)
                    {
                        int num8 = this.Terrain.TileSize.X - 1;
                        num3 = 0;
                        while (num3 <= num8)
                        {
                            if ((this.Terrain.Tiles[num3, num4].Texture.TextureNum >= 0) & (this.Terrain.Tiles[num3, num4].Texture.TextureNum < this.Tileset.TileCount))
                            {
                                numArray[num4, num3, 0] = this.Tileset.Tiles[this.Terrain.Tiles[num3, num4].Texture.TextureNum].AverageColour.Red;
                                numArray[num4, num3, 1] = this.Tileset.Tiles[this.Terrain.Tiles[num3, num4].Texture.TextureNum].AverageColour.Green;
                                numArray[num4, num3, 2] = this.Tileset.Tiles[this.Terrain.Tiles[num3, num4].Texture.TextureNum].AverageColour.Blue;
                            }
                            num3++;
                        }
                    }
                }
                if (modMain.frmMainInstance.menuMiniShowHeight.Checked)
                {
                    int num9 = this.Terrain.TileSize.Y - 1;
                    for (num4 = 0; num4 <= num9; num4++)
                    {
                        int num10 = this.Terrain.TileSize.X - 1;
                        num3 = 0;
                        while (num3 <= num10)
                        {
                            float num5 = ((float) (((this.Terrain.Vertices[num3, num4].Height + this.Terrain.Vertices[num3 + 1, num4].Height) + this.Terrain.Vertices[num3, num4 + 1].Height) + this.Terrain.Vertices[num3 + 1, num4 + 1].Height)) / 1020f;
                            numArray[num4, num3, 0] = ((numArray[num4, num3, 0] * 2f) + num5) / 3f;
                            numArray[num4, num3, 1] = ((numArray[num4, num3, 1] * 2f) + num5) / 3f;
                            numArray[num4, num3, 2] = ((numArray[num4, num3, 2] * 2f) + num5) / 3f;
                            num3++;
                        }
                    }
                }
            }
            else if (modMain.frmMainInstance.menuMiniShowHeight.Checked)
            {
                int num11 = this.Terrain.TileSize.Y - 1;
                for (num4 = 0; num4 <= num11; num4++)
                {
                    int num12 = this.Terrain.TileSize.X - 1;
                    num3 = 0;
                    while (num3 <= num12)
                    {
                        float num6 = ((float) (((this.Terrain.Vertices[num3, num4].Height + this.Terrain.Vertices[num3 + 1, num4].Height) + this.Terrain.Vertices[num3, num4 + 1].Height) + this.Terrain.Vertices[num3 + 1, num4 + 1].Height)) / 1020f;
                        numArray[num4, num3, 0] = num6;
                        numArray[num4, num3, 1] = num6;
                        numArray[num4, num3, 2] = num6;
                        num3++;
                    }
                }
            }
            else
            {
                int num13 = this.Terrain.TileSize.Y - 1;
                for (num4 = 0; num4 <= num13; num4++)
                {
                    int num14 = this.Terrain.TileSize.X - 1;
                    num3 = 0;
                    while (num3 <= num14)
                    {
                        numArray[num4, num3, 0] = 0f;
                        numArray[num4, num3, 1] = 0f;
                        numArray[num4, num3, 2] = 0f;
                        num3++;
                    }
                }
            }
            if (modMain.frmMainInstance.menuMiniShowCliffs.Checked && (this.Tileset != null))
            {
                alpha = modSettings.Settings.MinimapCliffColour.Alpha;
                num2 = 1f - alpha;
                int num15 = this.Terrain.TileSize.Y - 1;
                num4 = 0;
                while (num4 <= num15)
                {
                    int num16 = this.Terrain.TileSize.X - 1;
                    num3 = 0;
                    while (num3 <= num16)
                    {
                        if (((this.Terrain.Tiles[num3, num4].Texture.TextureNum >= 0) & (this.Terrain.Tiles[num3, num4].Texture.TextureNum < this.Tileset.TileCount)) && (this.Tileset.Tiles[this.Terrain.Tiles[num3, num4].Texture.TextureNum].Default_Type == 8))
                        {
                            numArray[num4, num3, 0] = (numArray[num4, num3, 0] * num2) + (modSettings.Settings.MinimapCliffColour.Red * alpha);
                            numArray[num4, num3, 1] = (numArray[num4, num3, 1] * num2) + (modSettings.Settings.MinimapCliffColour.Green * alpha);
                            numArray[num4, num3, 2] = (numArray[num4, num3, 2] * num2) + (modSettings.Settings.MinimapCliffColour.Blue * alpha);
                        }
                        num3++;
                    }
                    num4++;
                }
            }
            if (modMain.frmMainInstance.menuMiniShowGateways.Checked)
            {
                IEnumerator enumerator;
                try
                {
                    enumerator = this.Gateways.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        clsGateway current = (clsGateway) enumerator.Current;
                        modMath.ReorderXY(current.PosA, current.PosB, ref _int3, ref _int2);
                        int y = _int2.Y;
                        num4 = _int3.Y;
                        while (num4 <= y)
                        {
                            int x = _int2.X;
                            num3 = _int3.X;
                            while (num3 <= x)
                            {
                                numArray[num4, num3, 0] = 1f;
                                numArray[num4, num3, 1] = 1f;
                                numArray[num4, num3, 2] = 0f;
                                num3++;
                            }
                            num4++;
                        }
                    }
                }
                finally
                {
                    if (enumerator is IDisposable)
                    {
                        (enumerator as IDisposable).Dispose();
                    }
                }
            }
            if (modMain.frmMainInstance.menuMiniShowUnits.Checked)
            {
                bool flag;
                modMath.sXY_int _int;
                clsUnit unit;
                IEnumerator enumerator2;
                IEnumerator enumerator3;
                try
                {
                    enumerator2 = this.Units.GetEnumerator();
                    while (enumerator2.MoveNext())
                    {
                        unit = (clsUnit) enumerator2.Current;
                        flag = true;
                        if (unit.Type.UnitType_frmMainSelectedLink.IsConnected)
                        {
                            flag = false;
                        }
                        else
                        {
                            _int = unit.Type.get_GetFootprintSelected(unit.Rotation);
                        }
                        if (flag)
                        {
                            this.GetFootprintTileRangeClamped(unit.Pos.Horizontal, _int, ref _int3, ref _int2);
                            int num19 = _int2.Y;
                            for (num4 = _int3.Y; num4 <= num19; num4++)
                            {
                                int num20 = _int2.X;
                                num3 = _int3.X;
                                while (num3 <= num20)
                                {
                                    if (!flagArray[num4, num3])
                                    {
                                        flagArray[num4, num3] = true;
                                        if (modSettings.Settings.MinimapTeamColours)
                                        {
                                            if (modSettings.Settings.MinimapTeamColoursExceptFeatures & (unit.Type.Type == clsUnitType.enumType.Feature))
                                            {
                                                numArray[num4, num3, 0] = modProgram.MinimapFeatureColour.Red;
                                                numArray[num4, num3, 1] = modProgram.MinimapFeatureColour.Green;
                                                numArray[num4, num3, 2] = modProgram.MinimapFeatureColour.Blue;
                                            }
                                            else
                                            {
                                                sRGB_sng unitGroupMinimapColour = this.GetUnitGroupMinimapColour(unit.UnitGroup);
                                                numArray[num4, num3, 0] = unitGroupMinimapColour.Red;
                                                numArray[num4, num3, 1] = unitGroupMinimapColour.Green;
                                                numArray[num4, num3, 2] = unitGroupMinimapColour.Blue;
                                            }
                                        }
                                        else
                                        {
                                            numArray[num4, num3, 0] = (numArray[num4, num3, 0] * 0.6666667f) + 0.3333333f;
                                            numArray[num4, num3, 1] *= 0.6666667f;
                                            numArray[num4, num3, 2] *= 0.6666667f;
                                        }
                                    }
                                    num3++;
                                }
                            }
                        }
                    }
                }
                finally
                {
                    if (enumerator2 is IDisposable)
                    {
                        (enumerator2 as IDisposable).Dispose();
                    }
                }
                int num21 = Texture.Size.Y - 1;
                num4 = 0;
                while (num4 <= num21)
                {
                    int num22 = Texture.Size.X - 1;
                    num3 = 0;
                    while (num3 <= num22)
                    {
                        flagArray[num4, num3] = false;
                        num3++;
                    }
                    num4++;
                }
                alpha = modSettings.Settings.MinimapSelectedObjectsColour.Alpha;
                num2 = 1f - alpha;
                try
                {
                    enumerator3 = this.Units.GetEnumerator();
                    while (enumerator3.MoveNext())
                    {
                        unit = (clsUnit) enumerator3.Current;
                        flag = false;
                        if (unit.Type.UnitType_frmMainSelectedLink.IsConnected)
                        {
                            flag = true;
                            _int = unit.Type.get_GetFootprintSelected(unit.Rotation);
                            _int.X += 2;
                            _int.Y += 2;
                        }
                        if (flag)
                        {
                            this.GetFootprintTileRangeClamped(unit.Pos.Horizontal, _int, ref _int3, ref _int2);
                            int num23 = _int2.Y;
                            num4 = _int3.Y;
                            while (num4 <= num23)
                            {
                                int num24 = _int2.X;
                                num3 = _int3.X;
                                while (num3 <= num24)
                                {
                                    if (!flagArray[num4, num3])
                                    {
                                        flagArray[num4, num3] = true;
                                        numArray[num4, num3, 0] = (numArray[num4, num3, 0] * num2) + (modSettings.Settings.MinimapSelectedObjectsColour.Red * alpha);
                                        numArray[num4, num3, 1] = (numArray[num4, num3, 1] * num2) + (modSettings.Settings.MinimapSelectedObjectsColour.Green * alpha);
                                        numArray[num4, num3, 2] = (numArray[num4, num3, 2] * num2) + (modSettings.Settings.MinimapSelectedObjectsColour.Blue * alpha);
                                    }
                                    num3++;
                                }
                                num4++;
                            }
                        }
                    }
                }
                finally
                {
                    if (enumerator3 is IDisposable)
                    {
                        (enumerator3 as IDisposable).Dispose();
                    }
                }
            }
            int num25 = this.Terrain.TileSize.Y - 1;
            for (num4 = 0; num4 <= num25; num4++)
            {
                int num26 = this.Terrain.TileSize.X - 1;
                for (num3 = 0; num3 <= num26; num3++)
                {
                    sRGBA_sng _sng3 = new sRGBA_sng(numArray[num4, num3, 0], numArray[num4, num3, 1], numArray[num4, num3, 2], 1f);
                    Texture.set_Pixels(num3, num4, _sng3);
                }
            }
        }

        private void MinimapTimer_Tick(object sender, EventArgs e)
        {
            if (this.MainMap != this)
            {
                this.MinimapPending = false;
            }
            if (this.MinimapPending)
            {
                if (!this.SuppressMinimap)
                {
                    this.MinimapPending = false;
                    this.MinimapMake();
                }
            }
            else
            {
                this.MakeMinimapTimer.Enabled = false;
            }
        }

        public void PerformTileWall(clsWallType WallType, modMath.sXY_int TileNum, bool Expand)
        {
            modMath.sXY_int _int2;
            modMath.sXY_int _int3;
            clsUnit unit2;
            IEnumerator enumerator2;
            modProgram.enumTileWalls none = modProgram.enumTileWalls.None;
            modLists.SimpleList<clsUnit> list2 = new modLists.SimpleList<clsUnit>();
            modLists.SimpleList<clsUnit> list = new modLists.SimpleList<clsUnit>();
            _int3.X = TileNum.X - 1;
            _int3.Y = TileNum.Y - 1;
            _int2.X = TileNum.X + 1;
            _int2.Y = TileNum.Y + 1;
            modMath.sXY_int sectorNumClamped = this.GetSectorNumClamped(this.GetTileSectorNum(_int3));
            modMath.sXY_int _int4 = this.GetSectorNumClamped(this.GetTileSectorNum(_int2));
            int y = _int4.Y;
            for (int i = sectorNumClamped.Y; i <= y; i++)
            {
                int x = _int4.X;
                for (int j = sectorNumClamped.X; j <= x; j++)
                {
                    modMath.sXY_int _int5;
                    IEnumerator enumerator;
                    _int5.X = j;
                    _int5.Y = i;
                    try
                    {
                        enumerator = this.Sectors[_int5.X, _int5.Y].Units.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            clsUnitSectorConnection current = (clsUnitSectorConnection) enumerator.Current;
                            unit2 = current.Unit;
                            clsUnitType type3 = unit2.Type;
                            if (type3.Type == clsUnitType.enumType.PlayerStructure)
                            {
                                clsStructureType type2 = (clsStructureType) type3;
                                if (type2.WallLink.Source == WallType)
                                {
                                    modMath.sXY_int _int;
                                    modMath.sXY_int posTileNum = this.GetPosTileNum(unit2.Pos.Horizontal);
                                    _int.X = posTileNum.X - TileNum.X;
                                    _int.Y = posTileNum.Y - TileNum.Y;
                                    if (_int.Y == 1)
                                    {
                                        if (_int.X == 0)
                                        {
                                            none |= modProgram.enumTileWalls.Bottom;
                                            list2.Add(unit2);
                                        }
                                    }
                                    else
                                    {
                                        if (_int.Y == 0)
                                        {
                                            if (_int.X == 0)
                                            {
                                                list.Add(unit2);
                                            }
                                            else if (_int.X == -1)
                                            {
                                                none |= modProgram.enumTileWalls.Left;
                                                list2.Add(unit2);
                                            }
                                            else if (_int.X == 1)
                                            {
                                                none |= modProgram.enumTileWalls.Right;
                                                list2.Add(unit2);
                                            }
                                            continue;
                                        }
                                        if ((_int.Y == -1) && (_int.X == 0))
                                        {
                                            none |= modProgram.enumTileWalls.Top;
                                            list2.Add(unit2);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    finally
                    {
                        if (enumerator is IDisposable)
                        {
                            (enumerator as IDisposable).Dispose();
                        }
                    }
                }
            }
            try
            {
                enumerator2 = list.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    unit2 = (clsUnit) enumerator2.Current;
                    this.UnitRemoveStoreChange(unit2.MapLink.ArrayPosition);
                }
            }
            finally
            {
                if (enumerator2 is IDisposable)
                {
                    (enumerator2 as IDisposable).Dispose();
                }
            }
            clsUnit unit = new clsUnit();
            clsUnitType type = WallType.Segments[WallType.TileWalls_Segment[(int) none]];
            unit.Rotation = WallType.TileWalls_Direction[(int) none];
            if (Expand)
            {
                unit.UnitGroup = this.SelectedUnitGroup.Item;
            }
            else
            {
                if (list.Count == 0)
                {
                    Debugger.Break();
                    return;
                }
                unit.UnitGroup = list[0].UnitGroup;
            }
            modMath.sXY_int footprint = new modMath.sXY_int(1, 1);
            unit.Pos = this.TileAlignedPos(TileNum, footprint);
            unit.Type = type;
            new clsUnitAdd { Map = this, NewUnit = unit, StoreChange = true }.Perform();
            if (Expand)
            {
                IEnumerator enumerator3;
                try
                {
                    enumerator3 = list2.GetEnumerator();
                    while (enumerator3.MoveNext())
                    {
                        clsUnit unit3 = (clsUnit) enumerator3.Current;
                        this.PerformTileWall(WallType, this.GetPosTileNum(unit3.Pos.Horizontal), false);
                    }
                }
                finally
                {
                    if (enumerator3 is IDisposable)
                    {
                        (enumerator3 as IDisposable).Dispose();
                    }
                }
            }
        }

        public bool PosIsOnMap(modMath.sXY_int Horizontal)
        {
            modMath.sXY_int startTile = new modMath.sXY_int(0, 0);
            return modProgram.PosIsWithinTileArea(Horizontal, startTile, this.Terrain.TileSize);
        }

        public void RandomizeHeights(int LevelCount)
        {
            clsHeightmap.sHeights heights;
            sHeightmaps heightmaps;
            int num7;
            int num8;
            clsHeightmap hmSource = new clsHeightmap();
            clsHeightmap source = new clsHeightmap();
            clsHeightmap heightmap2 = new clsHeightmap();
            int num2 = LevelCount - 1;
            heights.Heights = new float[num2 + 1];
            clsHeightmap.sMinMax max = new clsHeightmap.sMinMax();
            heightmaps.Heightmaps = new clsHeightmap[num2 + 1];
            hmSource.HeightData.Height = new long[this.Terrain.TileSize.Y + 1, this.Terrain.TileSize.X + 1];
            hmSource.HeightData.SizeX = this.Terrain.TileSize.X + 1;
            hmSource.HeightData.SizeY = this.Terrain.TileSize.Y + 1;
            int y = this.Terrain.TileSize.Y;
            for (num8 = 0; num8 <= y; num8++)
            {
                int x = this.Terrain.TileSize.X;
                num7 = 0;
                while (num7 <= x)
                {
                    hmSource.HeightData.Height[num8, num7] = (long) Math.Round((double) (((double) this.Terrain.Vertices[num7, num8].Height) / hmSource.HeightScale));
                    num7++;
                }
            }
            hmSource.MinMaxGet(ref max);
            double num = 255.0;
            double num3 = num / ((double) num2);
            double num6 = num3 / 4.0;
            int num11 = num2;
            for (int i = 0; i <= num11; i++)
            {
                float num5 = (float) ((max.Min + (((double) (i * max.Max)) / ((double) num2))) * hmSource.HeightScale);
                heights.Heights[i] = num5;
                heightmap2.GenerateNewOfSize(this.Terrain.TileSize.Y + 1, this.Terrain.TileSize.X + 1, 2f, 10000.0);
                heightmaps.Heightmaps[i] = new clsHeightmap();
                heightmaps.Heightmaps[i].Rescale(heightmap2, num5 - num6, num5 + num6);
            }
            source.FadeMultiple(hmSource, ref heightmaps, ref heights);
            double heightMin = Math.Max((double) ((max.Min * hmSource.HeightScale) - num6), (double) 0.0);
            heightmap2.Rescale(source, heightMin, Math.Min((double) ((max.Max * hmSource.HeightScale) + num6), (double) 255.9));
            int num12 = this.Terrain.TileSize.Y;
            for (num8 = 0; num8 <= num12; num8++)
            {
                int num13 = this.Terrain.TileSize.X;
                for (num7 = 0; num7 <= num13; num7++)
                {
                    this.Terrain.Vertices[num7, num8].Height = (byte) Math.Round(((double) (heightmap2.HeightData.Height[num8, num7] * heightmap2.HeightScale)));
                }
            }
        }

        public void RandomizeTileOrientations()
        {
            int num3 = this.Terrain.TileSize.Y - 1;
            for (int i = 0; i <= num3; i++)
            {
                int num4 = this.Terrain.TileSize.X - 1;
                for (int j = 0; j <= num4; j++)
                {
                    this.Terrain.Tiles[j, i].Texture.Orientation = new TileOrientation.sTileOrientation(App.Random.Next() >= 0.5f, App.Random.Next() >= 0.5f, App.Random.Next() >= 0.5f);
                }
            }
            this.SectorTerrainUndoChanges.SetAllChanged();
            this.SectorGraphicsChanges.SetAllChanged();
        }

        public clsResult Read_FMap_Gateways(StreamReader File)
        {
            clsResult result2 = new clsResult("Reading gateways");
            clsINIRead read = new clsINIRead();
            result2.Take(read.ReadFile(File));
            clsFMap_INIGateways translator = new clsFMap_INIGateways(read.Sections.Count);
            result2.Take(read.Translate(translator));
            int num2 = 0;
            int num3 = translator.GatewayCount - 1;
            for (int i = 0; i <= num3; i++)
            {
                if (this.GatewayCreate(translator.Gateways[i].PosA, translator.Gateways[i].PosB) == null)
                {
                    num2++;
                }
            }
            if (num2 > 0)
            {
                result2.WarningAdd(Conversions.ToString(num2) + " gateways were invalid.");
            }
            return result2;
        }

        private clsResult Read_FMap_Info(StreamReader File, ref clsFMapInfo ResultInfo)
        {
            clsResult result2 = new clsResult("Read general map info");
            clsINIRead.clsSection section = new clsINIRead.clsSection();
            result2.Take(section.ReadFile(File));
            ResultInfo = new clsFMapInfo();
            result2.Take(section.Translate(ResultInfo));
            if ((ResultInfo.TerrainSize.X < 0) | (ResultInfo.TerrainSize.Y < 0))
            {
                result2.ProblemAdd("Map size was not specified or was invalid.");
            }
            return result2;
        }

        private clsResult Read_FMap_Objects(StreamReader File)
        {
            int num;
            int num3;
            int num4;
            int num6;
            int num7;
            int num8;
            int num9;
            clsResult output = new clsResult("Reading objects");
            clsINIRead read = new clsINIRead();
            output.Take(read.ReadFile(File));
            clsFMap_INIObjects translator = new clsFMap_INIObjects(read.Sections.Count);
            output.Take(read.Translate(translator));
            int num5 = 0x10;
            clsUnitAdd add = new clsUnitAdd();
            modMath.sXY_int startTile = new modMath.sXY_int(0, 0);
            add.Map = this;
            uint num2 = 1;
            int num11 = translator.ObjectCount - 1;
            for (num = 0; num <= num11; num++)
            {
                if (translator.Objects[num].ID >= num2)
                {
                    num2 = translator.Objects[num].ID + 1;
                }
            }
            int num12 = translator.ObjectCount - 1;
            for (num = 0; num <= num12; num++)
            {
                if (translator.Objects[num].Pos == null)
                {
                    num7++;
                }
                else if (!modProgram.PosIsWithinTileArea(translator.Objects[num].Pos.XY, startTile, this.Terrain.TileSize))
                {
                    num7++;
                }
                else
                {
                    clsUnitType type = null;
                    if (translator.Objects[num].Type != clsUnitType.enumType.Unspecified)
                    {
                        bool flag = false;
                        if ((translator.Objects[num].Type == clsUnitType.enumType.PlayerDroid) && !translator.Objects[num].IsTemplate)
                        {
                            flag = true;
                        }
                        if (flag)
                        {
                            clsDroidDesign design = new clsDroidDesign {
                                TemplateDroidType = translator.Objects[num].TemplateDroidType
                            };
                            if (design.TemplateDroidType == null)
                            {
                                design.TemplateDroidType = modProgram.TemplateDroidType_Droid;
                                num3++;
                            }
                            if (translator.Objects[num].BodyCode != "")
                            {
                                design.Body = modProgram.ObjectData.FindOrCreateBody(translator.Objects[num].BodyCode);
                                if (design.Body.IsUnknown)
                                {
                                    num4++;
                                }
                            }
                            if (translator.Objects[num].PropulsionCode != "")
                            {
                                design.Propulsion = modProgram.ObjectData.FindOrCreatePropulsion(translator.Objects[num].PropulsionCode);
                                if (design.Propulsion.IsUnknown)
                                {
                                    num4++;
                                }
                            }
                            design.TurretCount = (byte) translator.Objects[num].TurretCount;
                            if (translator.Objects[num].TurretCodes[0] != "")
                            {
                                design.Turret1 = modProgram.ObjectData.FindOrCreateTurret(translator.Objects[num].TurretTypes[0], translator.Objects[num].TurretCodes[0]);
                                if (design.Turret1.IsUnknown)
                                {
                                    num4++;
                                }
                            }
                            if (translator.Objects[num].TurretCodes[1] != "")
                            {
                                design.Turret2 = modProgram.ObjectData.FindOrCreateTurret(translator.Objects[num].TurretTypes[1], translator.Objects[num].TurretCodes[1]);
                                if (design.Turret2.IsUnknown)
                                {
                                    num4++;
                                }
                            }
                            if (translator.Objects[num].TurretCodes[2] != "")
                            {
                                design.Turret3 = modProgram.ObjectData.FindOrCreateTurret(translator.Objects[num].TurretTypes[2], translator.Objects[num].TurretCodes[2]);
                                if (design.Turret3.IsUnknown)
                                {
                                    num4++;
                                }
                            }
                            design.UpdateAttachments();
                            type = design;
                        }
                        else
                        {
                            type = modProgram.ObjectData.FindOrCreateUnitType(translator.Objects[num].Code, translator.Objects[num].Type, translator.Objects[num].WallType);
                            if (type.IsUnknown)
                            {
                                if (num9 < num5)
                                {
                                    output.WarningAdd("\"" + translator.Objects[num].Code + "\" is not a loaded object.");
                                }
                                num9++;
                            }
                        }
                    }
                    if (type == null)
                    {
                        num8++;
                    }
                    else
                    {
                        clsUnit iDUnit = new clsUnit {
                            Type = type
                        };
                        iDUnit.Pos.Horizontal.X = translator.Objects[num].Pos.X;
                        iDUnit.Pos.Horizontal.Y = translator.Objects[num].Pos.Y;
                        iDUnit.Health = translator.Objects[num].Health;
                        iDUnit.SavePriority = translator.Objects[num].Priority;
                        iDUnit.Rotation = (int) Math.Round(translator.Objects[num].Heading);
                        if (iDUnit.Rotation >= 360)
                        {
                            clsUnit unit2 = iDUnit;
                            unit2.Rotation -= 360;
                        }
                        if ((translator.Objects[num].UnitGroup == null) | (translator.Objects[num].UnitGroup == ""))
                        {
                            if (translator.Objects[num].Type != clsUnitType.enumType.Feature)
                            {
                                num6++;
                            }
                            iDUnit.UnitGroup = this.ScavengerUnitGroup;
                        }
                        else if (translator.Objects[num].UnitGroup.ToLower() == "scavenger")
                        {
                            iDUnit.UnitGroup = this.ScavengerUnitGroup;
                        }
                        else
                        {
                            clsUnitGroup scavengerUnitGroup;
                            try
                            {
                                uint num10;
                                if (!modIO.InvariantParse_uint(translator.Objects[num].UnitGroup, ref num10))
                                {
                                    throw new Exception();
                                }
                                if (num10 < 10L)
                                {
                                    scavengerUnitGroup = this.UnitGroups[(int) num10];
                                }
                                else
                                {
                                    scavengerUnitGroup = this.ScavengerUnitGroup;
                                    num6++;
                                }
                            }
                            catch (Exception exception1)
                            {
                                ProjectData.SetProjectError(exception1);
                                Exception exception = exception1;
                                num6++;
                                scavengerUnitGroup = this.ScavengerUnitGroup;
                                ProjectData.ClearProjectError();
                            }
                            iDUnit.UnitGroup = scavengerUnitGroup;
                        }
                        if (translator.Objects[num].ID == 0)
                        {
                            translator.Objects[num].ID = num2;
                            modProgram.ZeroIDWarning(iDUnit, translator.Objects[num].ID, output);
                        }
                        add.NewUnit = iDUnit;
                        add.ID = translator.Objects[num].ID;
                        add.Label = translator.Objects[num].Label;
                        add.Perform();
                        modProgram.ErrorIDChange(translator.Objects[num].ID, iDUnit, "Read_FMap_Objects");
                        if (num2 == translator.Objects[num].ID)
                        {
                            num2 = iDUnit.ID + 1;
                        }
                    }
                }
            }
            if (num9 > num5)
            {
                output.WarningAdd(Conversions.ToString(num9) + " objects were not in the loaded object data.");
            }
            if (num8 > 0)
            {
                output.WarningAdd(Conversions.ToString(num8) + " objects did not specify a type and were ignored.");
            }
            if (num4 > 0)
            {
                output.WarningAdd(Conversions.ToString(num4) + " components used by droids were loaded as unknowns.");
            }
            if (num6 > 0)
            {
                output.WarningAdd(Conversions.ToString(num6) + " objects had an invalid player number and were set to player 0.");
            }
            if (num7 > 0)
            {
                output.WarningAdd(Conversions.ToString(num7) + " objects had a position that was off-map and were ignored.");
            }
            if (num3 > 0)
            {
                output.WarningAdd(Conversions.ToString(num3) + " designed droids did not specify a template droid type and were set to droid.");
            }
            return output;
        }

        public clsResult Read_FMap_Roads(BinaryReader File)
        {
            int num2;
            clsResult result2 = new clsResult("Reading roads");
            try
            {
                int num;
                int num3;
                int num4;
                int y = this.Terrain.TileSize.Y;
                for (num4 = 0; num4 <= y; num4++)
                {
                    int num6 = this.Terrain.TileSize.X - 1;
                    num3 = 0;
                    while (num3 <= num6)
                    {
                        num = File.ReadByte() - 1;
                        if (num < 0)
                        {
                            this.Terrain.SideH[num3, num4].Road = null;
                        }
                        else if (num >= this.Painter.RoadCount)
                        {
                            if (num2 < 0x10)
                            {
                                result2.WarningAdd("Invalid road value for horizontal side " + Conversions.ToString(num3) + ", " + Conversions.ToString(num4) + ".");
                            }
                            num2++;
                            this.Terrain.SideH[num3, num4].Road = null;
                        }
                        else
                        {
                            this.Terrain.SideH[num3, num4].Road = this.Painter.Roads[num];
                        }
                        num3++;
                    }
                }
                int num7 = this.Terrain.TileSize.Y - 1;
                for (num4 = 0; num4 <= num7; num4++)
                {
                    int x = this.Terrain.TileSize.X;
                    for (num3 = 0; num3 <= x; num3++)
                    {
                        num = File.ReadByte() - 1;
                        if (num < 0)
                        {
                            this.Terrain.SideV[num3, num4].Road = null;
                        }
                        else if (num >= this.Painter.RoadCount)
                        {
                            if (num2 < 0x10)
                            {
                                result2.WarningAdd("Invalid road value for vertical side " + Conversions.ToString(num3) + ", " + Conversions.ToString(num4) + ".");
                            }
                            num2++;
                            this.Terrain.SideV[num3, num4].Road = null;
                        }
                        else
                        {
                            this.Terrain.SideV[num3, num4].Road = this.Painter.Roads[num];
                        }
                    }
                }
            }
            catch (Exception exception1)
            {
                ProjectData.SetProjectError(exception1);
                Exception exception = exception1;
                result2.ProblemAdd(exception.Message);
                clsResult result = result2;
                ProjectData.ClearProjectError();
                return result;
            }
            if (num2 > 0)
            {
                result2.WarningAdd(Conversions.ToString(num2) + " sides had an invalid road value.");
            }
            if (File.PeekChar() >= 0)
            {
                result2.WarningAdd("There were unread bytes at the end of the file.");
            }
            return result2;
        }

        public clsResult Read_FMap_TileCliff(BinaryReader File)
        {
            int num;
            int num4;
            clsResult result2 = new clsResult("Reading tile cliffs");
            try
            {
                int num7 = this.Terrain.TileSize.Y - 1;
                for (int i = 0; i <= num7; i++)
                {
                    int num8 = this.Terrain.TileSize.X - 1;
                    for (int j = 0; j <= num8; j++)
                    {
                        int num3 = File.ReadByte();
                        int num2 = (int) Math.Round(((double) (((double) num3) / 64.0)));
                        if (num2 > 0)
                        {
                            if (num4 < 0x10)
                            {
                                result2.WarningAdd("Unknown bits used for tile " + Conversions.ToString(j) + ", " + Conversions.ToString(i) + ".");
                            }
                            num4++;
                        }
                        num3 -= num2 * 0x40;
                        num2 = (int) Math.Round(((double) (((double) num3) / 8.0)));
                        switch (num2)
                        {
                            case 0:
                                this.Terrain.Tiles[j, i].DownSide = TileOrientation.TileDirection_None;
                                break;

                            case 1:
                                this.Terrain.Tiles[j, i].DownSide = TileOrientation.TileDirection_Top;
                                break;

                            case 2:
                                this.Terrain.Tiles[j, i].DownSide = TileOrientation.TileDirection_Left;
                                break;

                            case 3:
                                this.Terrain.Tiles[j, i].DownSide = TileOrientation.TileDirection_Right;
                                break;

                            case 4:
                                this.Terrain.Tiles[j, i].DownSide = TileOrientation.TileDirection_Bottom;
                                break;

                            default:
                                this.Terrain.Tiles[j, i].DownSide = TileOrientation.TileDirection_None;
                                if (num < 0x10)
                                {
                                    result2.WarningAdd("Down side value for tile " + Conversions.ToString(j) + ", " + Conversions.ToString(i) + " was invalid.");
                                }
                                num++;
                                break;
                        }
                        num3 -= num2 * 8;
                        num2 = (int) Math.Round(((double) (((double) num3) / 4.0)));
                        this.Terrain.Tiles[j, i].Terrain_IsCliff = num2 > 0;
                        num3 -= num2 * 4;
                        num2 = (int) Math.Round(((double) (((double) num3) / 2.0)));
                        if (this.Terrain.Tiles[j, i].Tri)
                        {
                            this.Terrain.Tiles[j, i].TriTopLeftIsCliff = num2 > 0;
                        }
                        else
                        {
                            this.Terrain.Tiles[j, i].TriBottomLeftIsCliff = num2 > 0;
                        }
                        num3 -= num2 * 2;
                        num2 = num3;
                        if (this.Terrain.Tiles[j, i].Tri)
                        {
                            this.Terrain.Tiles[j, i].TriBottomRightIsCliff = num2 > 0;
                        }
                        else
                        {
                            this.Terrain.Tiles[j, i].TriTopRightIsCliff = num2 > 0;
                        }
                    }
                }
            }
            catch (Exception exception1)
            {
                ProjectData.SetProjectError(exception1);
                Exception exception = exception1;
                result2.ProblemAdd(exception.Message);
                clsResult result = result2;
                ProjectData.ClearProjectError();
                return result;
            }
            if (num4 > 0)
            {
                result2.WarningAdd(Conversions.ToString(num4) + " tiles had unknown bits used.");
            }
            if (num > 0)
            {
                result2.WarningAdd(Conversions.ToString(num) + " tiles had invalid down side values.");
            }
            if (File.PeekChar() >= 0)
            {
                result2.WarningAdd("There were unread bytes at the end of the file.");
            }
            return result2;
        }

        public clsResult Read_FMap_TileOrientation(BinaryReader File)
        {
            int num3;
            clsResult result2 = new clsResult("Reading tile orientations");
            try
            {
                int num6 = this.Terrain.TileSize.Y - 1;
                for (int i = 0; i <= num6; i++)
                {
                    int num7 = this.Terrain.TileSize.X - 1;
                    for (int j = 0; j <= num7; j++)
                    {
                        int num2 = File.ReadByte();
                        int num = (int) Math.Round(Math.Floor((double) (((double) num2) / 16.0)));
                        if (num > 0)
                        {
                            if (num3 < 0x10)
                            {
                                result2.WarningAdd("Unknown bits used for tile " + Conversions.ToString(j) + ", " + Conversions.ToString(i) + ".");
                            }
                            num3++;
                        }
                        num2 -= num * 0x10;
                        num = (int) Math.Round(((double) (((double) num2) / 8.0)));
                        this.Terrain.Tiles[j, i].Texture.Orientation.SwitchedAxes = num > 0;
                        num2 -= num * 8;
                        num = (int) Math.Round(((double) (((double) num2) / 4.0)));
                        this.Terrain.Tiles[j, i].Texture.Orientation.ResultXFlip = num > 0;
                        num2 -= num * 4;
                        num = (int) Math.Round(((double) (((double) num2) / 2.0)));
                        this.Terrain.Tiles[j, i].Texture.Orientation.ResultYFlip = num > 0;
                        num2 -= num * 2;
                        num = num2;
                        this.Terrain.Tiles[j, i].Tri = num > 0;
                    }
                }
            }
            catch (Exception exception1)
            {
                ProjectData.SetProjectError(exception1);
                Exception exception = exception1;
                result2.ProblemAdd(exception.Message);
                clsResult result = result2;
                ProjectData.ClearProjectError();
                return result;
            }
            if (num3 > 0)
            {
                result2.WarningAdd(Conversions.ToString(num3) + " tiles had unknown bits used.");
            }
            if (File.PeekChar() >= 0)
            {
                result2.WarningAdd("There were unread bytes at the end of the file.");
            }
            return result2;
        }

        public clsResult Read_FMap_TileTexture(BinaryReader File)
        {
            clsResult result2 = new clsResult("Reading tile textures");
            try
            {
                int num4 = this.Terrain.TileSize.Y - 1;
                for (int i = 0; i <= num4; i++)
                {
                    int num5 = this.Terrain.TileSize.X - 1;
                    for (int j = 0; j <= num5; j++)
                    {
                        byte num = File.ReadByte();
                        this.Terrain.Tiles[j, i].Texture.TextureNum = num - 1;
                    }
                }
            }
            catch (Exception exception1)
            {
                ProjectData.SetProjectError(exception1);
                Exception exception = exception1;
                result2.ProblemAdd(exception.Message);
                clsResult result = result2;
                ProjectData.ClearProjectError();
                return result;
            }
            if (File.PeekChar() >= 0)
            {
                result2.WarningAdd("There were unread bytes at the end of the file.");
            }
            return result2;
        }

        public clsResult Read_FMap_TileTypes(BinaryReader File)
        {
            int num3;
            clsResult result2 = new clsResult("Reading tile types");
            try
            {
                if (this.Tileset != null)
                {
                    int num4 = this.Tileset.TileCount - 1;
                    for (int i = 0; i <= num4; i++)
                    {
                        byte num2 = File.ReadByte();
                        if (num2 >= modProgram.TileTypes.Count)
                        {
                            num3++;
                        }
                        else
                        {
                            this.Tile_TypeNum[i] = num2;
                        }
                    }
                }
            }
            catch (Exception exception1)
            {
                ProjectData.SetProjectError(exception1);
                Exception exception = exception1;
                result2.ProblemAdd(exception.Message);
                clsResult result = result2;
                ProjectData.ClearProjectError();
                return result;
            }
            if (num3 > 0)
            {
                result2.WarningAdd(Conversions.ToString(num3) + " tile types were invalid.");
            }
            if (File.PeekChar() >= 0)
            {
                result2.WarningAdd("There were unread bytes at the end of the file.");
            }
            return result2;
        }

        private clsResult Read_FMap_VertexHeight(BinaryReader File)
        {
            clsResult result2 = new clsResult("Reading vertex heights");
            try
            {
                int y = this.Terrain.TileSize.Y;
                for (int i = 0; i <= y; i++)
                {
                    int x = this.Terrain.TileSize.X;
                    for (int j = 0; j <= x; j++)
                    {
                        this.Terrain.Vertices[j, i].Height = File.ReadByte();
                    }
                }
            }
            catch (Exception exception1)
            {
                ProjectData.SetProjectError(exception1);
                Exception exception = exception1;
                result2.ProblemAdd(exception.Message);
                clsResult result = result2;
                ProjectData.ClearProjectError();
                return result;
            }
            if (File.PeekChar() >= 0)
            {
                result2.WarningAdd("There were unread bytes at the end of the file.");
            }
            return result2;
        }

        private clsResult Read_FMap_VertexTerrain(BinaryReader File)
        {
            int num3;
            clsResult result2 = new clsResult("Reading vertex terrain");
            try
            {
                int y = this.Terrain.TileSize.Y;
                for (int i = 0; i <= y; i++)
                {
                    int x = this.Terrain.TileSize.X;
                    for (int j = 0; j <= x; j++)
                    {
                        int index = File.ReadByte() - 1;
                        if (index < 0)
                        {
                            this.Terrain.Vertices[j, i].Terrain = null;
                        }
                        else if (index >= this.Painter.TerrainCount)
                        {
                            if (num3 < 0x10)
                            {
                                result2.WarningAdd("Painted terrain at vertex " + Conversions.ToString(j) + ", " + Conversions.ToString(i) + " was invalid.");
                            }
                            num3++;
                            this.Terrain.Vertices[j, i].Terrain = null;
                        }
                        else
                        {
                            this.Terrain.Vertices[j, i].Terrain = this.Painter.Terrains[index];
                        }
                    }
                }
            }
            catch (Exception exception1)
            {
                ProjectData.SetProjectError(exception1);
                Exception exception = exception1;
                result2.ProblemAdd(exception.Message);
                clsResult result = result2;
                ProjectData.ClearProjectError();
                return result;
            }
            if (num3 > 0)
            {
                result2.WarningAdd(Conversions.ToString(num3) + " painted terrain vertices were invalid.");
            }
            if (File.PeekChar() >= 0)
            {
                result2.WarningAdd("There were unread bytes at the end of the file.");
            }
            return result2;
        }

        private clsResult Read_FME(BinaryReader File)
        {
            clsResult output = new clsResult("Reading FME");
            clsInterfaceOptions options = new clsInterfaceOptions();
            clsUnitAdd add = new clsUnitAdd {
                Map = this
            };
            try
            {
                uint num = File.ReadUInt32();
                if (num <= 4)
                {
                    output.ProblemAdd("Version " + Conversions.ToString(num) + " is not supported.");
                    return output;
                }
                if (((num == 5) | (num == 6)) | (num == 7))
                {
                    int num2;
                    int num6;
                    int num12;
                    int num13;
                    byte num5 = File.ReadByte();
                    switch (num5)
                    {
                        case 0:
                            this.Tileset = null;
                            break;

                        case 1:
                            this.Tileset = modProgram.Tileset_Arizona;
                            break;

                        case 2:
                            this.Tileset = modProgram.Tileset_Urban;
                            break;

                        case 3:
                            this.Tileset = modProgram.Tileset_Rockies;
                            break;

                        default:
                            output.WarningAdd("Tileset value out of range.");
                            this.Tileset = null;
                            break;
                    }
                    this.SetPainterToDefaults();
                    ushort x = File.ReadUInt16();
                    ushort y = File.ReadUInt16();
                    if ((((x < 1) | (y < 1)) | (x > 0x200)) | (y > 0x200))
                    {
                        output.ProblemAdd("Map size is invalid.");
                        return output;
                    }
                    modMath.sXY_int tileSize = new modMath.sXY_int(x, y);
                    this.TerrainBlank(tileSize);
                    this.TileType_Reset();
                    int num11 = 0;
                    int num14 = this.Terrain.TileSize.Y;
                    for (num13 = 0; num13 <= num14; num13++)
                    {
                        int num15 = this.Terrain.TileSize.X;
                        num12 = 0;
                        while (num12 <= num15)
                        {
                            this.Terrain.Vertices[num12, num13].Height = File.ReadByte();
                            num6 = File.ReadByte() - 1;
                            if (num6 < 0)
                            {
                                this.Terrain.Vertices[num12, num13].Terrain = null;
                            }
                            else if (num6 >= this.Painter.TerrainCount)
                            {
                                num11++;
                                this.Terrain.Vertices[num12, num13].Terrain = null;
                            }
                            else
                            {
                                this.Terrain.Vertices[num12, num13].Terrain = this.Painter.Terrains[num6];
                            }
                            num12++;
                        }
                    }
                    if (num11 > 0)
                    {
                        output.WarningAdd(Conversions.ToString(num11) + " painted ground vertices were out of range.");
                    }
                    num11 = 0;
                    int num16 = this.Terrain.TileSize.Y - 1;
                    for (num13 = 0; num13 <= num16; num13++)
                    {
                        int num17 = this.Terrain.TileSize.X - 1;
                        num12 = 0;
                        while (num12 <= num17)
                        {
                            num5 = File.ReadByte();
                            this.Terrain.Tiles[num12, num13].Texture.TextureNum = num5 - 1;
                            num5 = File.ReadByte();
                            num6 = 0x80;
                            num2 = (int) Math.Round(((double) (((double) num5) / ((double) num6))));
                            num5 = (byte) (num5 - ((byte) (num2 * num6)));
                            this.Terrain.Tiles[num12, num13].Terrain_IsCliff = num2 == 1;
                            num6 = 0x40;
                            num2 = (int) Math.Round(((double) (((double) num5) / ((double) num6))));
                            num5 = (byte) (num5 - ((byte) (num2 * num6)));
                            this.Terrain.Tiles[num12, num13].Texture.Orientation.SwitchedAxes = num2 == 1;
                            num6 = 0x20;
                            num2 = (int) Math.Round(((double) (((double) num5) / ((double) num6))));
                            num5 = (byte) (num5 - ((byte) (num2 * num6)));
                            this.Terrain.Tiles[num12, num13].Texture.Orientation.ResultXFlip = num2 == 1;
                            num6 = 0x10;
                            num2 = (int) Math.Round(((double) (((double) num5) / ((double) num6))));
                            num5 = (byte) (num5 - ((byte) (num2 * num6)));
                            this.Terrain.Tiles[num12, num13].Texture.Orientation.ResultYFlip = num2 == 1;
                            num6 = 4;
                            num2 = (int) Math.Round(((double) (((double) num5) / ((double) num6))));
                            num5 = (byte) (num5 - ((byte) (num2 * num6)));
                            this.Terrain.Tiles[num12, num13].Tri = num2 == 1;
                            num6 = 2;
                            num2 = (int) Math.Round(((double) (((double) num5) / ((double) num6))));
                            num5 = (byte) (num5 - ((byte) (num2 * num6)));
                            if (this.Terrain.Tiles[num12, num13].Tri)
                            {
                                this.Terrain.Tiles[num12, num13].TriTopLeftIsCliff = num2 == 1;
                            }
                            else
                            {
                                this.Terrain.Tiles[num12, num13].TriBottomLeftIsCliff = num2 == 1;
                            }
                            num6 = 1;
                            num2 = (int) Math.Round(((double) (((double) num5) / ((double) num6))));
                            num5 = (byte) (num5 - ((byte) (num2 * num6)));
                            if (this.Terrain.Tiles[num12, num13].Tri)
                            {
                                this.Terrain.Tiles[num12, num13].TriBottomRightIsCliff = num2 == 1;
                            }
                            else
                            {
                                this.Terrain.Tiles[num12, num13].TriTopRightIsCliff = num2 == 1;
                            }
                            switch (File.ReadByte())
                            {
                                case 0:
                                    this.Terrain.Tiles[num12, num13].DownSide = TileOrientation.TileDirection_None;
                                    break;

                                case 1:
                                    this.Terrain.Tiles[num12, num13].DownSide = TileOrientation.TileDirection_Top;
                                    break;

                                case 2:
                                    this.Terrain.Tiles[num12, num13].DownSide = TileOrientation.TileDirection_Left;
                                    break;

                                case 3:
                                    this.Terrain.Tiles[num12, num13].DownSide = TileOrientation.TileDirection_Right;
                                    break;

                                case 4:
                                    this.Terrain.Tiles[num12, num13].DownSide = TileOrientation.TileDirection_Bottom;
                                    break;

                                default:
                                    num11++;
                                    break;
                            }
                            num12++;
                        }
                    }
                    if (num11 > 0)
                    {
                        output.WarningAdd(Conversions.ToString(num11) + " tile cliff down-sides were out of range.");
                    }
                    num11 = 0;
                    int num19 = this.Terrain.TileSize.Y;
                    for (num13 = 0; num13 <= num19; num13++)
                    {
                        int num20 = this.Terrain.TileSize.X - 1;
                        num12 = 0;
                        while (num12 <= num20)
                        {
                            num6 = File.ReadByte() - 1;
                            if (num6 < 0)
                            {
                                this.Terrain.SideH[num12, num13].Road = null;
                            }
                            else if (num6 >= this.Painter.RoadCount)
                            {
                                num11++;
                                this.Terrain.SideH[num12, num13].Road = null;
                            }
                            else
                            {
                                this.Terrain.SideH[num12, num13].Road = this.Painter.Roads[num6];
                            }
                            num12++;
                        }
                    }
                    int num21 = this.Terrain.TileSize.Y - 1;
                    for (num13 = 0; num13 <= num21; num13++)
                    {
                        int num22 = this.Terrain.TileSize.X;
                        for (num12 = 0; num12 <= num22; num12++)
                        {
                            num6 = File.ReadByte() - 1;
                            if (num6 < 0)
                            {
                                this.Terrain.SideV[num12, num13].Road = null;
                            }
                            else if (num6 >= this.Painter.RoadCount)
                            {
                                num11++;
                                this.Terrain.SideV[num12, num13].Road = null;
                            }
                            else
                            {
                                this.Terrain.SideV[num12, num13].Road = this.Painter.Roads[num6];
                            }
                        }
                    }
                    if (num11 > 0)
                    {
                        output.WarningAdd(Conversions.ToString(num11) + " roads were out of range.");
                    }
                    uint num10 = File.ReadUInt32();
                    sFMEUnit[] unitArray = new sFMEUnit[(((int) num10) - 1) + 1];
                    int num23 = ((int) num10) - 1;
                    for (num2 = 0; num2 <= num23; num2++)
                    {
                        unitArray[num2].Code = new string(File.ReadChars(40));
                        int num4 = Strings.InStr(unitArray[num2].Code, "\0", CompareMethod.Binary);
                        if (num4 > 0)
                        {
                            unitArray[num2].Code = Strings.Left(unitArray[num2].Code, num4 - 1);
                        }
                        unitArray[num2].LNDType = File.ReadByte();
                        unitArray[num2].ID = File.ReadUInt32();
                        if (num == 6)
                        {
                            unitArray[num2].SavePriority = File.ReadInt32();
                        }
                        unitArray[num2].X = File.ReadUInt32();
                        unitArray[num2].Z = File.ReadUInt32();
                        unitArray[num2].Y = File.ReadUInt32();
                        unitArray[num2].Rotation = File.ReadUInt16();
                        unitArray[num2].Name = modIO.ReadOldText(File);
                        unitArray[num2].Player = File.ReadByte();
                    }
                    clsUnitType type = null;
                    uint num3 = 1;
                    int num24 = ((int) num10) - 1;
                    for (num2 = 0; num2 <= num24; num2++)
                    {
                        if (unitArray[num2].ID >= num3)
                        {
                            num3 = unitArray[num2].ID + 1;
                        }
                    }
                    num11 = 0;
                    int num25 = ((int) num10) - 1;
                    for (num2 = 0; num2 <= num25; num2++)
                    {
                        switch (unitArray[num2].LNDType)
                        {
                            case 0:
                                type = modProgram.ObjectData.FindOrCreateUnitType(unitArray[num2].Code, clsUnitType.enumType.Feature, -1);
                                break;

                            case 1:
                                type = modProgram.ObjectData.FindOrCreateUnitType(unitArray[num2].Code, clsUnitType.enumType.PlayerStructure, -1);
                                break;

                            case 2:
                                type = modProgram.ObjectData.FindOrCreateUnitType(unitArray[num2].Code, clsUnitType.enumType.PlayerDroid, -1);
                                break;

                            default:
                                type = null;
                                break;
                        }
                        if (type != null)
                        {
                            clsUnit iDUnit = new clsUnit {
                                Type = type,
                                ID = unitArray[num2].ID,
                                SavePriority = unitArray[num2].SavePriority
                            };
                            if (unitArray[num2].Player >= 10)
                            {
                                iDUnit.UnitGroup = this.ScavengerUnitGroup;
                            }
                            else
                            {
                                iDUnit.UnitGroup = this.UnitGroups[unitArray[num2].Player];
                            }
                            iDUnit.Pos.Horizontal.X = (int) unitArray[num2].X;
                            iDUnit.Pos.Horizontal.Y = (int) unitArray[num2].Z;
                            iDUnit.Rotation = Math.Min(unitArray[num2].Rotation, 0x167);
                            if (unitArray[num2].ID == 0)
                            {
                                unitArray[num2].ID = num3;
                                modProgram.ZeroIDWarning(iDUnit, unitArray[num2].ID, output);
                            }
                            add.ID = unitArray[num2].ID;
                            add.NewUnit = iDUnit;
                            add.Perform();
                            modProgram.ErrorIDChange(unitArray[num2].ID, iDUnit, "Read_FMEv5+");
                            if (num3 == unitArray[num2].ID)
                            {
                                num3 = iDUnit.ID + 1;
                            }
                        }
                        else
                        {
                            num11++;
                        }
                    }
                    if (num11 > 0)
                    {
                        output.WarningAdd(Conversions.ToString(num11) + " types of units were invalid. That many units were ignored.");
                    }
                    uint num9 = File.ReadUInt32();
                    num11 = 0;
                    int num27 = ((int) num9) - 1;
                    for (num2 = 0; num2 <= num27; num2++)
                    {
                        modMath.sXY_int _int;
                        modMath.sXY_int _int2;
                        _int2.X = File.ReadUInt16();
                        _int2.Y = File.ReadUInt16();
                        _int.X = File.ReadUInt16();
                        _int.Y = File.ReadUInt16();
                        if (this.GatewayCreate(_int2, _int) == null)
                        {
                            num11++;
                        }
                    }
                    if (num11 > 0)
                    {
                        output.WarningAdd(Conversions.ToString(num11) + " gateways were invalid.");
                    }
                    if (this.Tileset != null)
                    {
                        int num28 = this.Tileset.TileCount - 1;
                        for (num2 = 0; num2 <= num28; num2++)
                        {
                            this.Tile_TypeNum[num2] = File.ReadByte();
                        }
                    }
                    options.ScrollMin.X = File.ReadInt32();
                    options.ScrollMin.Y = File.ReadInt32();
                    options.ScrollMax.X = File.ReadUInt32();
                    options.ScrollMax.Y = File.ReadUInt32();
                    string str = null;
                    options.CompileName = modIO.ReadOldText(File);
                    switch (File.ReadByte())
                    {
                    }
                    options.CompileMultiPlayers = modIO.ReadOldText(File);
                    switch (File.ReadByte())
                    {
                        case 0:
                            options.CompileMultiXPlayers = false;
                            break;

                        case 1:
                            options.CompileMultiXPlayers = true;
                            break;

                        default:
                            output.WarningAdd("Compile player format out of range.");
                            break;
                    }
                    options.CompileMultiAuthor = modIO.ReadOldText(File);
                    options.CompileMultiLicense = modIO.ReadOldText(File);
                    str = modIO.ReadOldText(File);
                    options.CampaignGameType = File.ReadInt32();
                    if ((options.CampaignGameType < -1) | (options.CampaignGameType >= 3))
                    {
                        output.WarningAdd("Compile campaign type out of range.");
                        options.CampaignGameType = -1;
                    }
                    if (File.PeekChar() >= 0)
                    {
                        output.WarningAdd("There were unread bytes at the end of the file.");
                    }
                }
                else
                {
                    output.ProblemAdd("File version number not recognised.");
                }
                this.InterfaceOptions = options;
            }
            catch (Exception exception1)
            {
                ProjectData.SetProjectError(exception1);
                Exception exception = exception1;
                output.ProblemAdd("Read error: " + exception.Message);
                ProjectData.ClearProjectError();
            }
            return output;
        }

        private modProgram.sResult Read_TTP(BinaryReader File)
        {
            modProgram.sResult result2;
            result2.Success = false;
            result2.Problem = "";
            try
            {
                if (modIO.ReadOldTextOfLength(File, 4) != "ttyp")
                {
                    result2.Problem = "Incorrect identifier.";
                    return result2;
                }
                if (File.ReadUInt32() != 8)
                {
                    result2.Problem = "Unknown version.";
                    return result2;
                }
                uint num2 = File.ReadUInt32();
                int num4 = ((int) Math.Min(num2, (uint) this.Tileset.TileCount)) - 1;
                for (int i = 0; i <= num4; i++)
                {
                    ushort num3 = File.ReadUInt16();
                    if (num3 > 11)
                    {
                        result2.Problem = "Unknown tile type number.";
                        return result2;
                    }
                    this.Tile_TypeNum[i] = (byte) num3;
                }
            }
            catch (Exception exception1)
            {
                ProjectData.SetProjectError(exception1);
                Exception exception = exception1;
                result2.Problem = exception.Message;
                modProgram.sResult result = result2;
                ProjectData.ClearProjectError();
                return result;
            }
            result2.Success = true;
            return result2;
        }

        private modProgram.sResult Read_WZ_Droids(BinaryReader File, modLists.SimpleClassList<clsWZBJOUnit> WZUnits)
        {
            modProgram.sResult result2;
            result2.Success = false;
            result2.Problem = "";
            try
            {
                if (modIO.ReadOldTextOfLength(File, 4) != "dint")
                {
                    result2.Problem = "Unknown dinit.bjo identifier.";
                    return result2;
                }
                if ((File.ReadUInt32() > 0x13) && (Interaction.MsgBox("dinit.bjo version is unknown. Continue?", MsgBoxStyle.Question | MsgBoxStyle.OkCancel, null) != MsgBoxResult.Ok))
                {
                    result2.Problem = "Aborted.";
                    return result2;
                }
                uint num3 = File.ReadUInt32();
                int num5 = ((int) num3) - 1;
                for (int i = 0; i <= num5; i++)
                {
                    clsWZBJOUnit newItem = new clsWZBJOUnit {
                        ObjectType = clsUnitType.enumType.PlayerDroid,
                        Code = modIO.ReadOldTextOfLength(File, 40)
                    };
                    int num2 = Strings.InStr(newItem.Code, "\0", CompareMethod.Binary);
                    if (num2 > 0)
                    {
                        newItem.Code = Strings.Left(newItem.Code, num2 - 1);
                    }
                    newItem.ID = File.ReadUInt32();
                    newItem.Pos.Horizontal.X = (int) File.ReadUInt32();
                    newItem.Pos.Horizontal.Y = (int) File.ReadUInt32();
                    newItem.Pos.Altitude = (int) File.ReadUInt32();
                    newItem.Rotation = File.ReadUInt32();
                    newItem.Player = File.ReadUInt32();
                    File.ReadBytes(12);
                    WZUnits.Add(newItem);
                }
            }
            catch (Exception exception1)
            {
                ProjectData.SetProjectError(exception1);
                Exception exception = exception1;
                result2.Problem = exception.Message;
                modProgram.sResult result = result2;
                ProjectData.ClearProjectError();
                return result;
            }
            result2.Success = true;
            return result2;
        }

        private modProgram.sResult Read_WZ_Features(BinaryReader File, modLists.SimpleClassList<clsWZBJOUnit> WZUnits)
        {
            modProgram.sResult result2;
            result2.Success = false;
            result2.Problem = "";
            try
            {
                if (modIO.ReadOldTextOfLength(File, 4) != "feat")
                {
                    result2.Problem = "Unknown feat.bjo identifier.";
                    return result2;
                }
                if ((File.ReadUInt32() != 8) && (Interaction.MsgBox("feat.bjo version is unknown. Continue?", MsgBoxStyle.Question | MsgBoxStyle.OkCancel, null) != MsgBoxResult.Ok))
                {
                    result2.Problem = "Aborted.";
                    return result2;
                }
                uint num3 = File.ReadUInt32();
                int num5 = ((int) num3) - 1;
                for (int i = 0; i <= num5; i++)
                {
                    clsWZBJOUnit newItem = new clsWZBJOUnit {
                        ObjectType = clsUnitType.enumType.Feature,
                        Code = modIO.ReadOldTextOfLength(File, 40)
                    };
                    int num2 = Strings.InStr(newItem.Code, "\0", CompareMethod.Binary);
                    if (num2 > 0)
                    {
                        newItem.Code = Strings.Left(newItem.Code, num2 - 1);
                    }
                    newItem.ID = File.ReadUInt32();
                    newItem.Pos.Horizontal.X = (int) File.ReadUInt32();
                    newItem.Pos.Horizontal.Y = (int) File.ReadUInt32();
                    newItem.Pos.Altitude = (int) File.ReadUInt32();
                    newItem.Rotation = File.ReadUInt32();
                    newItem.Player = File.ReadUInt32();
                    File.ReadBytes(12);
                    WZUnits.Add(newItem);
                }
            }
            catch (Exception exception1)
            {
                ProjectData.SetProjectError(exception1);
                Exception exception = exception1;
                result2.Problem = exception.Message;
                modProgram.sResult result = result2;
                ProjectData.ClearProjectError();
                return result;
            }
            result2.Success = true;
            return result2;
        }

        private modProgram.sResult Read_WZ_gam(BinaryReader File)
        {
            modProgram.sResult result2;
            result2.Success = false;
            result2.Problem = "";
            try
            {
                if (modIO.ReadOldTextOfLength(File, 4) != "game")
                {
                    result2.Problem = "Unknown game identifier.";
                    return result2;
                }
                if ((File.ReadUInt32() != 8) && (Interaction.MsgBox("Game file version is unknown. Continue?", MsgBoxStyle.Question | MsgBoxStyle.OkCancel, null) != MsgBoxResult.Ok))
                {
                    result2.Problem = "Aborted.";
                    return result2;
                }
                if (this.InterfaceOptions == null)
                {
                    this.InterfaceOptions = new clsInterfaceOptions();
                }
                File.ReadInt32();
                this.InterfaceOptions.CampaignGameType = File.ReadInt32();
                this.InterfaceOptions.AutoScrollLimits = false;
                this.InterfaceOptions.ScrollMin.X = File.ReadInt32();
                this.InterfaceOptions.ScrollMin.Y = File.ReadInt32();
                this.InterfaceOptions.ScrollMax.X = File.ReadUInt32();
                this.InterfaceOptions.ScrollMax.Y = File.ReadUInt32();
            }
            catch (Exception exception1)
            {
                ProjectData.SetProjectError(exception1);
                Exception exception = exception1;
                result2.Problem = exception.Message;
                modProgram.sResult result = result2;
                ProjectData.ClearProjectError();
                return result;
            }
            result2.Success = true;
            return result2;
        }

        public clsResult Read_WZ_Labels(clsINIRead INI, bool IsFMap)
        {
            IEnumerator enumerator;
            clsResult result2 = new clsResult("Reading labels");
            int num2 = 0;
            int num4 = 0;
            try
            {
                enumerator = INI.Sections.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    string str4;
                    string str5;
                    int num5;
                    clsINIRead.clsSection current = (clsINIRead.clsSection) enumerator.Current;
                    string name = current.Name;
                    int index = name.IndexOf('_');
                    switch (Strings.Left(name, index))
                    {
                        case "position":
                            num5 = 0;
                            break;

                        case "area":
                            num5 = 1;
                            break;

                        case "object":
                            if (IsFMap)
                            {
                                num5 = 0x7fffffff;
                                num2++;
                                continue;
                            }
                            num5 = 2;
                            break;

                        default:
                        {
                            num5 = 0x7fffffff;
                            num2++;
                            continue;
                        }
                    }
                    string lastPropertyValue = current.GetLastPropertyValue("label");
                    if (lastPropertyValue == null)
                    {
                        num2++;
                        continue;
                    }
                    lastPropertyValue = lastPropertyValue.Replace("\"", "");
                    switch (num5)
                    {
                        case 0:
                        {
                            str4 = current.GetLastPropertyValue("pos");
                            if (str4 != null)
                            {
                                break;
                            }
                            num2++;
                            continue;
                        }
                        case 1:
                        {
                            str4 = current.GetLastPropertyValue("pos1");
                            if (str4 != null)
                            {
                                goto Label_01DD;
                            }
                            num2++;
                            continue;
                        }
                        case 2:
                        {
                            uint num3;
                            if (modIO.InvariantParse_uint(current.GetLastPropertyValue("id"), ref num3))
                            {
                                clsUnit unit = this.IDUsage(num3);
                                if (unit == null)
                                {
                                    goto Label_030C;
                                }
                                if (!unit.SetLabel(lastPropertyValue).Success)
                                {
                                    num2++;
                                }
                            }
                            continue;
                        }
                        default:
                            goto Label_0314;
                    }
                    clsPositionFromText text = new clsPositionFromText();
                    if (text.Translate(str4))
                    {
                        clsScriptPosition position = clsScriptPosition.Create(this);
                        position.PosX = text.Pos.X;
                        position.PosY = text.Pos.Y;
                        position.SetLabel(lastPropertyValue);
                        if (((position.Label != lastPropertyValue) | (position.PosX != text.Pos.X)) | (position.PosY != text.Pos.Y))
                        {
                            num4++;
                        }
                    }
                    else
                    {
                        num2++;
                    }
                    continue;
                Label_01DD:
                    str5 = current.GetLastPropertyValue("pos2");
                    if (str5 == null)
                    {
                        num2++;
                    }
                    else
                    {
                        text = new clsPositionFromText();
                        clsPositionFromText text2 = new clsPositionFromText();
                        if (text.Translate(str4) & text2.Translate(str5))
                        {
                            clsScriptArea area = clsScriptArea.Create(this);
                            area.SetPositions(text.Pos, text2.Pos);
                            area.SetLabel(lastPropertyValue);
                            if (((((area.Label != lastPropertyValue) | (area.PosAX != text.Pos.X)) | (area.PosAY != text.Pos.Y)) | (area.PosBX != text2.Pos.X)) | (area.PosBY != text2.Pos.Y))
                            {
                                num4++;
                            }
                        }
                        else
                        {
                            num2++;
                        }
                    }
                    continue;
                Label_030C:
                    num2++;
                    continue;
                Label_0314:
                    result2.WarningAdd("Error! Bad type number for script label.");
                }
            }
            finally
            {
                if (enumerator is IDisposable)
                {
                    (enumerator as IDisposable).Dispose();
                }
            }
            if (num2 > 0)
            {
                result2.WarningAdd("Unable to translate " + Conversions.ToString(num2) + " script labels.");
            }
            if (num4 > 0)
            {
                result2.WarningAdd(Conversions.ToString(num4) + " script labels had invalid values and were modified.");
            }
            return result2;
        }

        private modProgram.sResult Read_WZ_map(BinaryReader File)
        {
            modProgram.sResult result2;
            result2.Success = false;
            result2.Problem = "";
            try
            {
                int num;
                if (modIO.ReadOldTextOfLength(File, 4) != "map ")
                {
                    result2.Problem = "Unknown game.map identifier.";
                    return result2;
                }
                uint num8 = File.ReadUInt32();
                if ((num8 != 10) && (Interaction.MsgBox("game.map version is unknown. Continue?", MsgBoxStyle.Question | MsgBoxStyle.OkCancel, null) != MsgBoxResult.Ok))
                {
                    result2.Problem = "Aborted.";
                    return result2;
                }
                uint num4 = File.ReadUInt32();
                uint num3 = File.ReadUInt32();
                if ((((num4 < 1) | (num4 > 0x200L)) | (num3 < 1)) | (num3 > 0x200L))
                {
                    result2.Problem = "Map size out of range.";
                    return result2;
                }
                modMath.sXY_int tileSize = new modMath.sXY_int((int) num4, (int) num3);
                this.TerrainBlank(tileSize);
                int num11 = this.Terrain.TileSize.Y - 1;
                for (int i = 0; i <= num11; i++)
                {
                    int num12 = this.Terrain.TileSize.X - 1;
                    for (int j = 0; j <= num12; j++)
                    {
                        byte num6 = File.ReadByte();
                        this.Terrain.Tiles[j, i].Texture.TextureNum = num6;
                        byte num2 = File.ReadByte();
                        this.Terrain.Vertices[j, i].Height = File.ReadByte();
                        num = (int) Math.Round(((double) (((double) num2) / 128.0)));
                        num2 = (byte) (num2 - ((byte) (num * 0x80)));
                        bool oldFlipX = num == 1;
                        num = (int) Math.Round(((double) (((double) num2) / 64.0)));
                        num2 = (byte) (num2 - ((byte) (num * 0x40)));
                        bool oldFlipZ = num == 1;
                        num = (int) Math.Round(((double) (((double) num2) / 16.0)));
                        num2 = (byte) (num2 - ((byte) (num * 0x10)));
                        byte oldRotation = (byte) num;
                        TileOrientation.OldOrientation_To_TileOrientation(oldRotation, oldFlipX, oldFlipZ, ref this.Terrain.Tiles[j, i].Texture.Orientation);
                        num = (int) Math.Round(((double) (((double) num2) / 8.0)));
                        num2 = (byte) (num2 - ((byte) (num * 8)));
                        this.Terrain.Tiles[j, i].Tri = num == 1;
                    }
                }
                if (num8 != 2)
                {
                    if (File.ReadUInt32() != 1L)
                    {
                        result2.Problem = "Bad gateway version number.";
                        return result2;
                    }
                    uint num7 = File.ReadUInt32();
                    int num13 = ((int) num7) - 1;
                    for (num = 0; num <= num13; num++)
                    {
                        modMath.sXY_int _int;
                        modMath.sXY_int _int2;
                        _int.X = File.ReadByte();
                        _int.Y = File.ReadByte();
                        _int2.X = File.ReadByte();
                        _int2.Y = File.ReadByte();
                        if (this.GatewayCreate(_int, _int2) == null)
                        {
                            result2.Problem = "Gateway placement error.";
                            return result2;
                        }
                    }
                }
            }
            catch (Exception exception1)
            {
                ProjectData.SetProjectError(exception1);
                Exception exception = exception1;
                result2.Problem = exception.Message;
                modProgram.sResult result = result2;
                ProjectData.ClearProjectError();
                return result;
            }
            result2.Success = true;
            return result2;
        }

        private modProgram.sResult Read_WZ_Structures(BinaryReader File, ref modLists.SimpleClassList<clsWZBJOUnit> WZUnits)
        {
            modProgram.sResult result2;
            result2.Success = false;
            result2.Problem = "";
            try
            {
                if (modIO.ReadOldTextOfLength(File, 4) != "stru")
                {
                    result2.Problem = "Unknown struct.bjo identifier.";
                    return result2;
                }
                if ((File.ReadUInt32() != 8) && (Interaction.MsgBox("struct.bjo version is unknown. Continue?", MsgBoxStyle.Question | MsgBoxStyle.OkCancel, null) != MsgBoxResult.Ok))
                {
                    result2.Problem = "Aborted.";
                    return result2;
                }
                uint num3 = File.ReadUInt32();
                int num5 = ((int) num3) - 1;
                for (int i = 0; i <= num5; i++)
                {
                    clsWZBJOUnit newItem = new clsWZBJOUnit {
                        ObjectType = clsUnitType.enumType.PlayerStructure,
                        Code = modIO.ReadOldTextOfLength(File, 40)
                    };
                    int num2 = Strings.InStr(newItem.Code, "\0", CompareMethod.Binary);
                    if (num2 > 0)
                    {
                        newItem.Code = Strings.Left(newItem.Code, num2 - 1);
                    }
                    newItem.ID = File.ReadUInt32();
                    newItem.Pos.Horizontal.X = (int) File.ReadUInt32();
                    newItem.Pos.Horizontal.Y = (int) File.ReadUInt32();
                    newItem.Pos.Altitude = (int) File.ReadUInt32();
                    newItem.Rotation = File.ReadUInt32();
                    newItem.Player = File.ReadUInt32();
                    File.ReadBytes(0x38);
                    WZUnits.Add(newItem);
                }
            }
            catch (Exception exception1)
            {
                ProjectData.SetProjectError(exception1);
                Exception exception = exception1;
                result2.Problem = exception.Message;
                modProgram.sResult result = result2;
                ProjectData.ClearProjectError();
                return result;
            }
            result2.Success = true;
            return result2;
        }

        private modProgram.sResult Read_WZ_TileTypes(BinaryReader File)
        {
            modProgram.sResult result2;
            result2.Success = false;
            result2.Problem = "";
            try
            {
                if (modIO.ReadOldTextOfLength(File, 4) != "ttyp")
                {
                    result2.Problem = "Unknown ttypes.ttp identifier.";
                    return result2;
                }
                if ((File.ReadUInt32() != 8) && (Interaction.MsgBox("ttypes.ttp version is unknown. Continue?", MsgBoxStyle.Question | MsgBoxStyle.OkCancel, null) != MsgBoxResult.Ok))
                {
                    result2.Problem = "Aborted.";
                    return result2;
                }
                uint num2 = File.ReadUInt32();
                if (this.Tileset != null)
                {
                    int num5 = Math.Min((int) num2, this.Tileset.TileCount) - 1;
                    for (int i = 0; i <= num5; i++)
                    {
                        ushort num3 = File.ReadUInt16();
                        if (num3 > 11)
                        {
                            result2.Problem = "Unknown tile type.";
                            return result2;
                        }
                        this.Tile_TypeNum[i] = (byte) num3;
                    }
                }
            }
            catch (Exception exception1)
            {
                ProjectData.SetProjectError(exception1);
                Exception exception = exception1;
                result2.Problem = exception.Message;
                modProgram.sResult result = result2;
                ProjectData.ClearProjectError();
                return result;
            }
            result2.Success = true;
            return result2;
        }

        public void RedoPerform()
        {
            IEnumerator enumerator;
            clsUndo undo = this.Undos[this.UndoPosition];
            modLists.SimpleList<clsShadowSector> list = new modLists.SimpleList<clsShadowSector>();
            try
            {
                enumerator = undo.ChangedSectors.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    clsShadowSector current = (clsShadowSector) enumerator.Current;
                    modMath.sXY_int sectorNum = current.Num;
                    clsShadowSector newItem = this.ShadowSectors[sectorNum.X, sectorNum.Y];
                    this.Sectors[sectorNum.X, sectorNum.Y].DeleteLists();
                    this.Undo_Sector_Rejoin(current);
                    this.ShadowSector_Create(sectorNum);
                    list.Add(newItem);
                    this.SectorGraphicsChanges.Changed(sectorNum);
                }
            }
            finally
            {
                if (enumerator is IDisposable)
                {
                    (enumerator as IDisposable).Dispose();
                }
            }
            undo.ChangedSectors = list;
            clsUnitAdd add = new clsUnitAdd {
                Map = this
            };
            int num4 = undo.UnitChanges.Count - 1;
            for (int i = 0; i <= num4; i++)
            {
                clsUnit iDUnit = undo.UnitChanges[i].Unit;
                switch (undo.UnitChanges[i].Type)
                {
                    case clsUnitChange.enumType.Added:
                    {
                        uint iD = iDUnit.ID;
                        add.ID = iD;
                        add.NewUnit = iDUnit;
                        add.Perform();
                        modProgram.ErrorIDChange(iD, iDUnit, "Redo_Perform");
                        break;
                    }
                    case clsUnitChange.enumType.Deleted:
                        this.UnitRemove(iDUnit.MapLink.ArrayPosition);
                        break;

                    default:
                        Debugger.Break();
                        break;
                }
            }
            int num5 = undo.GatewayChanges.Count - 1;
            for (int j = 0; j <= num5; j++)
            {
                clsGatewayChange change = undo.GatewayChanges[j];
                switch (change.Type)
                {
                    case clsGatewayChange.enumType.Added:
                        change.Gateway.MapLink.Connect(this.Gateways);
                        break;

                    case clsGatewayChange.enumType.Deleted:
                        change.Gateway.MapLink.Disconnect();
                        break;

                    default:
                        Debugger.Break();
                        break;
                }
            }
            this.UndoPosition++;
            this.SectorsUpdateGraphics();
            this.MinimapMakeLater();
            modMain.frmMainInstance.SelectedObject_Changed();
        }

        public void Rotate(TileOrientation.sTileOrientation Orientation, modProgram.enumObjectRotateMode ObjectRotateMode)
        {
            modMath.sXY_int _int8;
            modMath.sXY_int _int9;
            clsUnit current;
            int num2;
            int num3;
            IEnumerator enumerator;
            IEnumerator enumerator2;
            IEnumerator enumerator3;
            modMath.sXY_int pos = new modMath.sXY_int(0, 0);
            modMath.sXY_int _int3 = TileOrientation.GetRotatedPos(Orientation, pos, this.Terrain.TileSize);
            modMath.sXY_int _int4 = TileOrientation.GetRotatedPos(Orientation, this.Terrain.TileSize, this.Terrain.TileSize);
            int x = Math.Max(_int3.X, _int4.X);
            modMath.sXY_int newSize = new modMath.sXY_int(x, Math.Max(_int3.Y, _int4.Y));
            clsTerrain terrain = new clsTerrain(newSize);
            modMath.sXY_int limits = new modMath.sXY_int(terrain.TileSize.X - 1, terrain.TileSize.Y - 1);
            modMath.sXY_int _int = new modMath.sXY_int(terrain.TileSize.X - 1, terrain.TileSize.Y);
            modMath.sXY_int _int2 = new modMath.sXY_int(terrain.TileSize.X, terrain.TileSize.Y - 1);
            modMath.sXY_int _int7 = new modMath.sXY_int(this.Terrain.TileSize.X - 1, this.Terrain.TileSize.Y - 1);
            modMath.sXY_int _int6 = new modMath.sXY_int(this.Terrain.TileSize.X * 0x80, this.Terrain.TileSize.Y * 0x80);
            TileOrientation.sTileOrientation orientation = Orientation;
            orientation.Reverse();
            int y = terrain.TileSize.Y;
            for (num3 = 0; num3 <= y; num3++)
            {
                _int8.Y = num3;
                int num5 = terrain.TileSize.X;
                num2 = 0;
                while (num2 <= num5)
                {
                    _int8.X = num2;
                    _int9 = TileOrientation.GetRotatedPos(orientation, _int8, newSize);
                    terrain.Vertices[num2, num3].Height = this.Terrain.Vertices[_int9.X, _int9.Y].Height;
                    terrain.Vertices[num2, num3].Terrain = this.Terrain.Vertices[_int9.X, _int9.Y].Terrain;
                    num2++;
                }
            }
            int num6 = terrain.TileSize.Y - 1;
            for (num3 = 0; num3 <= num6; num3++)
            {
                _int8.Y = num3;
                int num7 = terrain.TileSize.X - 1;
                num2 = 0;
                while (num2 <= num7)
                {
                    TileOrientation.sTileDirection rotated;
                    _int8.X = num2;
                    _int9 = TileOrientation.GetRotatedPos(orientation, _int8, limits);
                    terrain.Tiles[num2, num3].Texture = this.Terrain.Tiles[_int9.X, _int9.Y].Texture;
                    terrain.Tiles[num2, num3].Texture.Orientation = terrain.Tiles[num2, num3].Texture.Orientation.GetRotated(Orientation);
                    terrain.Tiles[num2, num3].DownSide = this.Terrain.Tiles[_int9.X, _int9.Y].DownSide;
                    terrain.Tiles[num2, num3].DownSide = terrain.Tiles[num2, num3].DownSide.GetRotated(Orientation);
                    if (this.Terrain.Tiles[_int9.X, _int9.Y].Tri)
                    {
                        rotated = TileOrientation.TileDirection_TopLeft;
                    }
                    else
                    {
                        rotated = TileOrientation.TileDirection_TopRight;
                    }
                    rotated = rotated.GetRotated(Orientation);
                    terrain.Tiles[num2, num3].Tri = TileOrientation.IdenticalTileDirections(rotated, TileOrientation.TileDirection_TopLeft) | TileOrientation.IdenticalTileDirections(rotated, TileOrientation.TileDirection_BottomRight);
                    if (this.Terrain.Tiles[_int9.X, _int9.Y].Tri)
                    {
                        if (this.Terrain.Tiles[_int9.X, _int9.Y].TriTopLeftIsCliff)
                        {
                            TileOrientation.RotateDirection(TileOrientation.TileDirection_TopLeft, Orientation, ref rotated);
                            terrain.Tiles[num2, num3].TriCliffAddDirection(rotated);
                        }
                        if (this.Terrain.Tiles[_int9.X, _int9.Y].TriBottomRightIsCliff)
                        {
                            TileOrientation.RotateDirection(TileOrientation.TileDirection_BottomRight, Orientation, ref rotated);
                            terrain.Tiles[num2, num3].TriCliffAddDirection(rotated);
                        }
                    }
                    else
                    {
                        if (this.Terrain.Tiles[_int9.X, _int9.Y].TriTopRightIsCliff)
                        {
                            TileOrientation.RotateDirection(TileOrientation.TileDirection_TopRight, Orientation, ref rotated);
                            terrain.Tiles[num2, num3].TriCliffAddDirection(rotated);
                        }
                        if (this.Terrain.Tiles[_int9.X, _int9.Y].TriBottomLeftIsCliff)
                        {
                            TileOrientation.RotateDirection(TileOrientation.TileDirection_BottomLeft, Orientation, ref rotated);
                            terrain.Tiles[num2, num3].TriCliffAddDirection(rotated);
                        }
                    }
                    terrain.Tiles[num2, num3].Terrain_IsCliff = this.Terrain.Tiles[_int9.X, _int9.Y].Terrain_IsCliff;
                    num2++;
                }
            }
            if (Orientation.SwitchedAxes)
            {
                int num8 = terrain.TileSize.Y;
                for (num3 = 0; num3 <= num8; num3++)
                {
                    _int8.Y = num3;
                    int num9 = terrain.TileSize.X - 1;
                    num2 = 0;
                    while (num2 <= num9)
                    {
                        _int8.X = num2;
                        _int9 = TileOrientation.GetRotatedPos(orientation, _int8, _int);
                        terrain.SideH[num2, num3].Road = this.Terrain.SideV[_int9.X, _int9.Y].Road;
                        num2++;
                    }
                }
                int num10 = terrain.TileSize.Y - 1;
                for (num3 = 0; num3 <= num10; num3++)
                {
                    _int8.Y = num3;
                    int num11 = terrain.TileSize.X;
                    num2 = 0;
                    while (num2 <= num11)
                    {
                        _int8.X = num2;
                        _int9 = TileOrientation.GetRotatedPos(orientation, _int8, _int2);
                        terrain.SideV[num2, num3].Road = this.Terrain.SideH[_int9.X, _int9.Y].Road;
                        num2++;
                    }
                }
            }
            else
            {
                int num12 = terrain.TileSize.Y;
                for (num3 = 0; num3 <= num12; num3++)
                {
                    _int8.Y = num3;
                    int num13 = terrain.TileSize.X - 1;
                    num2 = 0;
                    while (num2 <= num13)
                    {
                        _int8.X = num2;
                        _int9 = TileOrientation.GetRotatedPos(orientation, _int8, _int);
                        terrain.SideH[num2, num3].Road = this.Terrain.SideH[_int9.X, _int9.Y].Road;
                        num2++;
                    }
                }
                int num14 = terrain.TileSize.Y - 1;
                for (num3 = 0; num3 <= num14; num3++)
                {
                    _int8.Y = num3;
                    int num15 = terrain.TileSize.X;
                    for (num2 = 0; num2 <= num15; num2++)
                    {
                        _int8.X = num2;
                        _int9 = TileOrientation.GetRotatedPos(orientation, _int8, _int2);
                        terrain.SideV[num2, num3].Road = this.Terrain.SideV[_int9.X, _int9.Y].Road;
                    }
                }
            }
            try
            {
                enumerator = this.Units.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    clsUnit unit2;
                    current = (clsUnit) enumerator.Current;
                    current.Sectors.Clear();
                    if (ObjectRotateMode == modProgram.enumObjectRotateMode.All)
                    {
                        current.Rotation = (int) Math.Round((double) (modMath.AngleClamp(6.2831853071795862 - TileOrientation.GetRotatedAngle(Orientation, modMath.AngleClamp(6.2831853071795862 - (current.Rotation * 0.017453292519943295)))) / 0.017453292519943295));
                        if (current.Rotation < 0)
                        {
                            unit2 = current;
                            unit2.Rotation += 360;
                        }
                    }
                    else if (((ObjectRotateMode == modProgram.enumObjectRotateMode.Walls) && (current.Type.Type == clsUnitType.enumType.PlayerStructure)) && (((clsStructureType) current.Type).StructureType == clsStructureType.enumStructureType.Wall))
                    {
                        current.Rotation = (int) Math.Round((double) (modMath.AngleClamp(6.2831853071795862 - TileOrientation.GetRotatedAngle(Orientation, modMath.AngleClamp(6.2831853071795862 - (current.Rotation * 0.017453292519943295)))) / 0.017453292519943295));
                        if (current.Rotation < 0)
                        {
                            unit2 = current;
                            unit2.Rotation += 360;
                        }
                    }
                    current.Pos.Horizontal = TileOrientation.GetRotatedPos(Orientation, current.Pos.Horizontal, _int6);
                }
            }
            finally
            {
                if (enumerator is IDisposable)
                {
                    (enumerator as IDisposable).Dispose();
                }
            }
            modMath.sXY_int startTile = new modMath.sXY_int(0, 0);
            try
            {
                enumerator2 = this.Units.GetItemsAsSimpleList().GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    current = (clsUnit) enumerator2.Current;
                    if (!modProgram.PosIsWithinTileArea(current.Pos.Horizontal, startTile, terrain.TileSize))
                    {
                        int arrayPosition = current.MapLink.ArrayPosition;
                        this.UnitRemove(arrayPosition);
                    }
                }
            }
            finally
            {
                if (enumerator2 is IDisposable)
                {
                    (enumerator2 as IDisposable).Dispose();
                }
            }
            this.Terrain = terrain;
            try
            {
                enumerator3 = this.Gateways.GetItemsAsSimpleClassList().GetEnumerator();
                while (enumerator3.MoveNext())
                {
                    clsGateway gateway = (clsGateway) enumerator3.Current;
                    this.GatewayCreate(TileOrientation.GetRotatedPos(Orientation, gateway.PosA, _int7), TileOrientation.GetRotatedPos(Orientation, gateway.PosB, _int7));
                    gateway.Deallocate();
                }
            }
            finally
            {
                if (enumerator3 is IDisposable)
                {
                    (enumerator3 as IDisposable).Dispose();
                }
            }
            if (this._ReadyForUserInput)
            {
                this.CancelUserInput();
                this.InitializeUserInput();
            }
        }

        public bool Save_FMap_Prompt()
        {
            SaveFileDialog dialog = new SaveFileDialog {
                InitialDirectory = this.GetDirectory(),
                FileName = "",
                Filter = "FlaME Map Files (*.fmap)|*.fmap"
            };
            if (dialog.ShowDialog(modMain.frmMainInstance) != DialogResult.OK)
            {
                return false;
            }
            modSettings.Settings.SavePath = System.IO.Path.GetDirectoryName(dialog.FileName);
            clsResult result = this.Write_FMap(dialog.FileName, true, true);
            if (!result.HasProblems)
            {
                this.PathInfo = new clsPathInfo(dialog.FileName, true);
                this.ChangedSinceSave = false;
            }
            modProgram.ShowWarnings(result);
            return !result.HasProblems;
        }

        public bool Save_FMap_Quick()
        {
            if (this.PathInfo == null)
            {
                return this.Save_FMap_Prompt();
            }
            if (!this.PathInfo.IsFMap)
            {
                return this.Save_FMap_Prompt();
            }
            clsResult result = this.Write_FMap(this.PathInfo.Path, true, true);
            if (!result.HasProblems)
            {
                this.ChangedSinceSave = false;
            }
            modProgram.ShowWarnings(result);
            return !result.HasProblems;
        }

        public modProgram.sResult ScriptLabelIsValid(string Text)
        {
            modProgram.sResult result;
            IEnumerator enumerator;
            IEnumerator enumerator2;
            IEnumerator enumerator3;
            result.Success = false;
            result.Problem = "";
            if (Text == null)
            {
                result.Problem = "Label cannot be nothing.";
                return result;
            }
            string str = Text.ToLower();
            if (str.Length < 1)
            {
                result.Problem = "Label cannot be nothing.";
                return result;
            }
            bool flag = false;
            string str2 = str;
            int num = 0;
            int length = str2.Length;
            while (num < length)
            {
                char ch = str2[num];
                if (!((((ch >= 'a') & (ch <= 'z')) | ((ch >= '0') & (ch <= '9'))) | (ch == '_')))
                {
                    flag = true;
                    break;
                }
                num++;
            }
            if (flag)
            {
                result.Problem = "Label contains invalid characters. Use only letters, numbers or underscores.";
                return result;
            }
            try
            {
                enumerator = this.Units.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    clsUnit current = (clsUnit) enumerator.Current;
                    if ((current.Label != null) && (str == current.Label.ToLower()))
                    {
                        result.Problem = "Label text is already in use.";
                        return result;
                    }
                }
            }
            finally
            {
                if (enumerator is IDisposable)
                {
                    (enumerator as IDisposable).Dispose();
                }
            }
            try
            {
                enumerator2 = this.ScriptPositions.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    clsScriptPosition position = (clsScriptPosition) enumerator2.Current;
                    if (str == position.Label.ToLower())
                    {
                        result.Problem = "Label text is already in use.";
                        return result;
                    }
                }
            }
            finally
            {
                if (enumerator2 is IDisposable)
                {
                    (enumerator2 as IDisposable).Dispose();
                }
            }
            try
            {
                enumerator3 = this.ScriptAreas.GetEnumerator();
                while (enumerator3.MoveNext())
                {
                    clsScriptArea area = (clsScriptArea) enumerator3.Current;
                    if (str == area.Label.ToLower())
                    {
                        result.Problem = "Label text is already in use.";
                        return result;
                    }
                }
            }
            finally
            {
                if (enumerator3 is IDisposable)
                {
                    (enumerator3 as IDisposable).Dispose();
                }
            }
            result.Success = true;
            return result;
        }

        public void Sector_GLList_Make(int X, int Y)
        {
            int num5;
            int num6;
            this.Sectors[X, Y].DeleteLists();
            int num3 = X * 8;
            int num4 = Y * 8;
            int num = Math.Min(num3 + 8, this.Terrain.TileSize.X) - 1;
            int num2 = Math.Min(num4 + 8, this.Terrain.TileSize.Y) - 1;
            this.Sectors[X, Y].GLList_Textured = GL.GenLists(1);
            GL.NewList(this.Sectors[X, Y].GLList_Textured, ListMode.Compile);
            if (modProgram.Draw_Units)
            {
                IEnumerator enumerator;
                bool[,] flagArray = new bool[8, 8];
                try
                {
                    enumerator = this.Sectors[X, Y].Units.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        clsUnitSectorConnection current = (clsUnitSectorConnection) enumerator.Current;
                        clsUnit unit = current.Unit;
                        if (unit.Type.Type == clsUnitType.enumType.PlayerStructure)
                        {
                            clsStructureType type = (clsStructureType) unit.Type;
                            if (type.StructureBasePlate != null)
                            {
                                modMath.sXY_int _int2;
                                modMath.sXY_int _int3;
                                modMath.sXY_int footprint = type.get_GetFootprintSelected(unit.Rotation);
                                this.GetFootprintTileRangeClamped(unit.Pos.Horizontal, footprint, ref _int3, ref _int2);
                                int num7 = Math.Min(_int2.Y, num2);
                                num6 = Math.Max(_int3.Y, num4);
                                while (num6 <= num7)
                                {
                                    int num8 = Math.Min(_int2.X, num);
                                    num5 = Math.Max(_int3.X, num3);
                                    while (num5 <= num8)
                                    {
                                        flagArray[num5 - num3, num6 - num4] = true;
                                        num5++;
                                    }
                                    num6++;
                                }
                            }
                        }
                    }
                }
                finally
                {
                    if (enumerator is IDisposable)
                    {
                        (enumerator as IDisposable).Dispose();
                    }
                }
                clsDrawTileOld old = new clsDrawTileOld {
                    Map = this
                };
                int num9 = num2;
                for (num6 = num4; num6 <= num9; num6++)
                {
                    old.TileY = num6;
                    int num10 = num;
                    num5 = num3;
                    while (num5 <= num10)
                    {
                        if (!flagArray[num5 - num3, num6 - num4])
                        {
                            old.TileX = num5;
                            old.Perform();
                        }
                        num5++;
                    }
                }
            }
            else
            {
                clsDrawTileOld old2 = new clsDrawTileOld {
                    Map = this
                };
                int num11 = num2;
                for (num6 = num4; num6 <= num11; num6++)
                {
                    old2.TileY = num6;
                    int num12 = num;
                    num5 = num3;
                    while (num5 <= num12)
                    {
                        old2.TileX = num5;
                        old2.Perform();
                        num5++;
                    }
                }
            }
            GL.EndList();
            this.Sectors[X, Y].GLList_Wireframe = GL.GenLists(1);
            GL.NewList(this.Sectors[X, Y].GLList_Wireframe, ListMode.Compile);
            int num13 = num2;
            for (num6 = num4; num6 <= num13; num6++)
            {
                int num14 = num;
                for (num5 = num3; num5 <= num14; num5++)
                {
                    this.DrawTileWireframe(num5, num6);
                }
            }
            GL.EndList();
        }

        public void SectorAll_GLLists_Delete()
        {
            int num3 = this.SectorCount.Y - 1;
            for (int i = 0; i <= num3; i++)
            {
                int num4 = this.SectorCount.X - 1;
                for (int j = 0; j <= num4; j++)
                {
                    this.Sectors[j, i].DeleteLists();
                }
            }
        }

        public void SectorsUpdateGraphics()
        {
            clsUpdateSectorGraphics tool = new clsUpdateSectorGraphics {
                Map = this
            };
            if (this.MainMap == this)
            {
                this.SectorGraphicsChanges.PerformTool(tool);
            }
            this.SectorGraphicsChanges.Clear();
        }

        public void SectorsUpdateUnitHeights()
        {
            clsUpdateSectorUnitHeights tool = new clsUpdateSectorUnitHeights {
                Map = this
            };
            tool.Start();
            this.SectorUnitHeightsChanges.PerformTool(tool);
            tool.Finish();
            this.SectorUnitHeightsChanges.Clear();
        }

        public void SelectedUnitsAction(clsObjectAction Tool)
        {
            clsObjectSelect tool = new clsObjectSelect();
            this.SelectedUnits.GetItemsAsSimpleClassList().PerformTool(Tool);
            this.SelectedUnits.Clear();
            Tool.ResultUnits.PerformTool(tool);
        }

        public clsResult Serialize_FMap_Gateways(clsINIWrite File)
        {
            clsResult result = new clsResult("Serializing gateways");
            try
            {
                int num2 = this.Gateways.Count - 1;
                for (int i = 0; i <= num2; i++)
                {
                    clsGateway gateway = this.Gateways[i];
                    File.SectionName_Append(modIO.InvariantToString_int(i));
                    File.Property_Append("AX", modIO.InvariantToString_int(gateway.PosA.X));
                    File.Property_Append("AY", modIO.InvariantToString_int(gateway.PosA.Y));
                    File.Property_Append("BX", modIO.InvariantToString_int(gateway.PosB.X));
                    File.Property_Append("BY", modIO.InvariantToString_int(gateway.PosB.Y));
                    File.Gap_Append();
                }
            }
            catch (Exception exception1)
            {
                ProjectData.SetProjectError(exception1);
                Exception exception = exception1;
                result.ProblemAdd(exception.Message);
                ProjectData.ClearProjectError();
            }
            return result;
        }

        public clsResult Serialize_FMap_Info(clsINIWrite File)
        {
            clsResult result = new clsResult("Serializing general map info");
            try
            {
                if (this.Tileset != null)
                {
                    if (this.Tileset == modProgram.Tileset_Arizona)
                    {
                        File.Property_Append("Tileset", "Arizona");
                    }
                    else if (this.Tileset == modProgram.Tileset_Urban)
                    {
                        File.Property_Append("Tileset", "Urban");
                    }
                    else if (this.Tileset == modProgram.Tileset_Rockies)
                    {
                        File.Property_Append("Tileset", "Rockies");
                    }
                }
                File.Property_Append("Size", modIO.InvariantToString_int(this.Terrain.TileSize.X) + ", " + modIO.InvariantToString_int(this.Terrain.TileSize.Y));
                File.Property_Append("AutoScrollLimits", modIO.InvariantToString_bool(this.InterfaceOptions.AutoScrollLimits));
                File.Property_Append("ScrollMinX", modIO.InvariantToString_int(this.InterfaceOptions.ScrollMin.X));
                File.Property_Append("ScrollMinY", modIO.InvariantToString_int(this.InterfaceOptions.ScrollMin.Y));
                File.Property_Append("ScrollMaxX", modIO.InvariantToString_uint(this.InterfaceOptions.ScrollMax.X));
                File.Property_Append("ScrollMaxY", modIO.InvariantToString_uint(this.InterfaceOptions.ScrollMax.Y));
                File.Property_Append("Name", this.InterfaceOptions.CompileName);
                File.Property_Append("Players", this.InterfaceOptions.CompileMultiPlayers);
                File.Property_Append("XPlayerLev", modIO.InvariantToString_bool(this.InterfaceOptions.CompileMultiXPlayers));
                File.Property_Append("Author", this.InterfaceOptions.CompileMultiAuthor);
                File.Property_Append("License", this.InterfaceOptions.CompileMultiLicense);
                if (this.InterfaceOptions.CampaignGameType >= 0)
                {
                    File.Property_Append("CampType", modIO.InvariantToString_int(this.InterfaceOptions.CampaignGameType));
                }
            }
            catch (Exception exception1)
            {
                ProjectData.SetProjectError(exception1);
                Exception exception = exception1;
                result.ProblemAdd(exception.Message);
                ProjectData.ClearProjectError();
            }
            return result;
        }

        public clsResult Serialize_FMap_Objects(clsINIWrite File)
        {
            int num2;
            clsResult result = new clsResult("Serializing objects");
            string str = null;
            try
            {
                int num3 = this.Units.Count - 1;
                for (int i = 0; i <= num3; i++)
                {
                    clsDroidDesign design;
                    clsUnit unit = this.Units[i];
                    File.SectionName_Append(modIO.InvariantToString_int(i));
                    switch (unit.Type.Type)
                    {
                        case clsUnitType.enumType.Feature:
                            File.Property_Append("Type", "Feature, " + ((clsFeatureType) unit.Type).Code);
                            goto Label_025C;

                        case clsUnitType.enumType.PlayerStructure:
                        {
                            clsStructureType type = (clsStructureType) unit.Type;
                            File.Property_Append("Type", "Structure, " + type.Code);
                            if (type.WallLink.IsConnected)
                            {
                                File.Property_Append("WallType", modIO.InvariantToString_int(type.WallLink.ArrayPosition));
                            }
                            goto Label_025C;
                        }
                        case clsUnitType.enumType.PlayerDroid:
                            design = (clsDroidDesign) unit.Type;
                            if (!design.IsTemplate)
                            {
                                break;
                            }
                            File.Property_Append("Type", "DroidTemplate, " + ((clsDroidTemplate) unit.Type).Code);
                            goto Label_025C;

                        default:
                            num2++;
                            goto Label_025C;
                    }
                    File.Property_Append("Type", "DroidDesign");
                    if (design.TemplateDroidType != null)
                    {
                        File.Property_Append("DroidType", design.TemplateDroidType.TemplateCode);
                    }
                    if (design.Body != null)
                    {
                        File.Property_Append("Body", design.Body.Code);
                    }
                    if (design.Propulsion != null)
                    {
                        File.Property_Append("Propulsion", design.Propulsion.Code);
                    }
                    File.Property_Append("TurretCount", modIO.InvariantToString_byte(design.TurretCount));
                    if ((design.Turret1 != null) && design.Turret1.GetTurretTypeName(ref str))
                    {
                        File.Property_Append("Turret1", str + ", " + design.Turret1.Code);
                    }
                    if ((design.Turret2 != null) && design.Turret2.GetTurretTypeName(ref str))
                    {
                        File.Property_Append("Turret2", str + ", " + design.Turret2.Code);
                    }
                    if ((design.Turret3 != null) && design.Turret3.GetTurretTypeName(ref str))
                    {
                        File.Property_Append("Turret3", str + ", " + design.Turret3.Code);
                    }
                Label_025C:
                    File.Property_Append("ID", modIO.InvariantToString_uint(unit.ID));
                    File.Property_Append("Priority", modIO.InvariantToString_int(unit.SavePriority));
                    File.Property_Append("Pos", modIO.InvariantToString_int(unit.Pos.Horizontal.X) + ", " + modIO.InvariantToString_int(unit.Pos.Horizontal.Y));
                    File.Property_Append("Heading", modIO.InvariantToString_int(unit.Rotation));
                    File.Property_Append("UnitGroup", unit.UnitGroup.GetFMapINIPlayerText());
                    if (unit.Health < 1.0)
                    {
                        File.Property_Append("Health", modIO.InvariantToString_dbl(unit.Health));
                    }
                    if (unit.Label != null)
                    {
                        File.Property_Append("ScriptLabel", unit.Label);
                    }
                    File.Gap_Append();
                }
            }
            catch (Exception exception1)
            {
                ProjectData.SetProjectError(exception1);
                Exception exception = exception1;
                result.ProblemAdd(exception.Message);
                ProjectData.ClearProjectError();
            }
            if (num2 > 0)
            {
                result.WarningAdd("Error: " + Conversions.ToString(num2) + " units were of an unhandled type.");
            }
            return result;
        }

        public clsResult Serialize_FMap_Roads(BinaryWriter File)
        {
            int num;
            clsResult result = new clsResult("Serializing roads");
            try
            {
                int num2;
                int num3;
                int num4;
                int y = this.Terrain.TileSize.Y;
                for (num4 = 0; num4 <= y; num4++)
                {
                    int num6 = this.Terrain.TileSize.X - 1;
                    num3 = 0;
                    while (num3 <= num6)
                    {
                        if (this.Terrain.SideH[num3, num4].Road == null)
                        {
                            num2 = 0;
                        }
                        else if (this.Terrain.SideH[num3, num4].Road.Num < 0)
                        {
                            num++;
                            num2 = 0;
                        }
                        else
                        {
                            num2 = this.Terrain.SideH[num3, num4].Road.Num + 1;
                            if (num2 > 0xff)
                            {
                                num++;
                                num2 = 0;
                            }
                        }
                        File.Write((byte) num2);
                        num3++;
                    }
                }
                int num7 = this.Terrain.TileSize.Y - 1;
                for (num4 = 0; num4 <= num7; num4++)
                {
                    int x = this.Terrain.TileSize.X;
                    for (num3 = 0; num3 <= x; num3++)
                    {
                        if (this.Terrain.SideV[num3, num4].Road == null)
                        {
                            num2 = 0;
                        }
                        else if (this.Terrain.SideV[num3, num4].Road.Num < 0)
                        {
                            num++;
                            num2 = 0;
                        }
                        else
                        {
                            num2 = this.Terrain.SideV[num3, num4].Road.Num + 1;
                            if (num2 > 0xff)
                            {
                                num++;
                                num2 = 0;
                            }
                        }
                        File.Write((byte) num2);
                    }
                }
            }
            catch (Exception exception1)
            {
                ProjectData.SetProjectError(exception1);
                Exception exception = exception1;
                result.ProblemAdd(exception.Message);
                ProjectData.ClearProjectError();
            }
            if (num > 0)
            {
                result.WarningAdd(Conversions.ToString(num) + " sides had an invalid road number.");
            }
            return result;
        }

        public clsResult Serialize_FMap_TileCliff(BinaryWriter File)
        {
            int num2;
            clsResult result = new clsResult("Serializing tile cliffs");
            try
            {
                int num6 = this.Terrain.TileSize.Y - 1;
                for (int i = 0; i <= num6; i++)
                {
                    int num7 = this.Terrain.TileSize.X - 1;
                    for (int j = 0; j <= num7; j++)
                    {
                        int num;
                        int num3 = 0;
                        if (this.Terrain.Tiles[j, i].Tri)
                        {
                            if (this.Terrain.Tiles[j, i].TriTopLeftIsCliff)
                            {
                                num3 += 2;
                            }
                            if (this.Terrain.Tiles[j, i].TriBottomRightIsCliff)
                            {
                                num3++;
                            }
                        }
                        else
                        {
                            if (this.Terrain.Tiles[j, i].TriBottomLeftIsCliff)
                            {
                                num3 += 2;
                            }
                            if (this.Terrain.Tiles[j, i].TriTopRightIsCliff)
                            {
                                num3++;
                            }
                        }
                        if (this.Terrain.Tiles[j, i].Terrain_IsCliff)
                        {
                            num3 += 4;
                        }
                        if (TileOrientation.IdenticalTileDirections(this.Terrain.Tiles[j, i].DownSide, TileOrientation.TileDirection_None))
                        {
                            num = 0;
                        }
                        else if (TileOrientation.IdenticalTileDirections(this.Terrain.Tiles[j, i].DownSide, TileOrientation.TileDirection_Top))
                        {
                            num = 1;
                        }
                        else if (TileOrientation.IdenticalTileDirections(this.Terrain.Tiles[j, i].DownSide, TileOrientation.TileDirection_Left))
                        {
                            num = 2;
                        }
                        else if (TileOrientation.IdenticalTileDirections(this.Terrain.Tiles[j, i].DownSide, TileOrientation.TileDirection_Right))
                        {
                            num = 3;
                        }
                        else if (TileOrientation.IdenticalTileDirections(this.Terrain.Tiles[j, i].DownSide, TileOrientation.TileDirection_Bottom))
                        {
                            num = 4;
                        }
                        else
                        {
                            num2++;
                            num = 0;
                        }
                        num3 += num * 8;
                        File.Write((byte) num3);
                    }
                }
            }
            catch (Exception exception1)
            {
                ProjectData.SetProjectError(exception1);
                Exception exception = exception1;
                result.ProblemAdd(exception.Message);
                ProjectData.ClearProjectError();
            }
            if (num2 > 0)
            {
                result.WarningAdd(Conversions.ToString(num2) + " tiles had an invalid cliff down side.");
            }
            return result;
        }

        public clsResult Serialize_FMap_TileOrientation(BinaryWriter File)
        {
            clsResult result = new clsResult("Serializing tile orientations");
            try
            {
                int num4 = this.Terrain.TileSize.Y - 1;
                for (int i = 0; i <= num4; i++)
                {
                    int num5 = this.Terrain.TileSize.X - 1;
                    for (int j = 0; j <= num5; j++)
                    {
                        int num = 0;
                        if (this.Terrain.Tiles[j, i].Texture.Orientation.SwitchedAxes)
                        {
                            num += 8;
                        }
                        if (this.Terrain.Tiles[j, i].Texture.Orientation.ResultXFlip)
                        {
                            num += 4;
                        }
                        if (this.Terrain.Tiles[j, i].Texture.Orientation.ResultYFlip)
                        {
                            num += 2;
                        }
                        if (this.Terrain.Tiles[j, i].Tri)
                        {
                            num++;
                        }
                        File.Write((byte) num);
                    }
                }
            }
            catch (Exception exception1)
            {
                ProjectData.SetProjectError(exception1);
                Exception exception = exception1;
                result.ProblemAdd(exception.Message);
                ProjectData.ClearProjectError();
            }
            return result;
        }

        public clsResult Serialize_FMap_TileTexture(BinaryWriter File)
        {
            int num;
            clsResult result = new clsResult("Serializing tile textures");
            try
            {
                int num5 = this.Terrain.TileSize.Y - 1;
                for (int i = 0; i <= num5; i++)
                {
                    int num6 = this.Terrain.TileSize.X - 1;
                    for (int j = 0; j <= num6; j++)
                    {
                        int num2 = this.Terrain.Tiles[j, i].Texture.TextureNum + 1;
                        if ((num2 < 0) | (num2 > 0xff))
                        {
                            num++;
                            num2 = 0;
                        }
                        File.Write((byte) num2);
                    }
                }
            }
            catch (Exception exception1)
            {
                ProjectData.SetProjectError(exception1);
                Exception exception = exception1;
                result.ProblemAdd(exception.Message);
                ProjectData.ClearProjectError();
            }
            if (num > 0)
            {
                result.WarningAdd(Conversions.ToString(num) + " tiles had an invalid texture number.");
            }
            return result;
        }

        public clsResult Serialize_FMap_TileTypes(BinaryWriter File)
        {
            clsResult result = new clsResult("Serializing tile types");
            try
            {
                if (this.Tileset == null)
                {
                    return result;
                }
                int num2 = this.Tileset.TileCount - 1;
                for (int i = 0; i <= num2; i++)
                {
                    File.Write(this.Tile_TypeNum[i]);
                }
            }
            catch (Exception exception1)
            {
                ProjectData.SetProjectError(exception1);
                Exception exception = exception1;
                result.ProblemAdd(exception.Message);
                ProjectData.ClearProjectError();
            }
            return result;
        }

        public clsResult Serialize_FMap_VertexHeight(BinaryWriter File)
        {
            clsResult result = new clsResult("Serializing vertex heights");
            try
            {
                int y = this.Terrain.TileSize.Y;
                for (int i = 0; i <= y; i++)
                {
                    int x = this.Terrain.TileSize.X;
                    for (int j = 0; j <= x; j++)
                    {
                        File.Write(this.Terrain.Vertices[j, i].Height);
                    }
                }
            }
            catch (Exception exception1)
            {
                ProjectData.SetProjectError(exception1);
                Exception exception = exception1;
                result.ProblemAdd(exception.Message);
                ProjectData.ClearProjectError();
            }
            return result;
        }

        public clsResult Serialize_FMap_VertexTerrain(BinaryWriter File)
        {
            int num;
            clsResult result = new clsResult("Serializing vertex terrain");
            try
            {
                int y = this.Terrain.TileSize.Y;
                for (int i = 0; i <= y; i++)
                {
                    int x = this.Terrain.TileSize.X;
                    for (int j = 0; j <= x; j++)
                    {
                        int num2;
                        if (this.Terrain.Vertices[j, i].Terrain == null)
                        {
                            num2 = 0;
                        }
                        else if (this.Terrain.Vertices[j, i].Terrain.Num < 0)
                        {
                            num++;
                            num2 = 0;
                        }
                        else
                        {
                            num2 = this.Terrain.Vertices[j, i].Terrain.Num + 1;
                            if (num2 > 0xff)
                            {
                                num++;
                                num2 = 0;
                            }
                        }
                        File.Write((byte) num2);
                    }
                }
            }
            catch (Exception exception1)
            {
                ProjectData.SetProjectError(exception1);
                Exception exception = exception1;
                result.ProblemAdd(exception.Message);
                ProjectData.ClearProjectError();
            }
            if (num > 0)
            {
                result.WarningAdd(Conversions.ToString(num) + " vertices had an invalid painted terrain number.");
            }
            return result;
        }

        public clsResult Serialize_WZ_DroidsINI(clsINIWrite File, int PlayerCount)
        {
            IEnumerator enumerator;
            clsResult result = new clsResult("Serializing droids INI");
            int num = 0;
            try
            {
                enumerator = this.Units.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    clsUnit current = (clsUnit) enumerator.Current;
                    if (current.Type.Type == clsUnitType.enumType.PlayerDroid)
                    {
                        bool preferPartsOutput;
                        clsDroidTemplate template;
                        clsDroidDesign type = (clsDroidDesign) current.Type;
                        bool flag2 = true;
                        if (current.ID <= 0L)
                        {
                            flag2 = false;
                            result.WarningAdd("Error. A droid's ID was zero. It was NOT saved. Delete and replace it to allow save.");
                        }
                        if (type.IsTemplate)
                        {
                            template = (clsDroidTemplate) type;
                            preferPartsOutput = current.PreferPartsOutput;
                        }
                        else
                        {
                            template = null;
                            preferPartsOutput = true;
                        }
                        if (preferPartsOutput)
                        {
                            if (type.Body == null)
                            {
                                flag2 = false;
                                num++;
                            }
                            else if (type.Propulsion == null)
                            {
                                flag2 = false;
                                num++;
                            }
                            else if (type.TurretCount >= 1)
                            {
                                if (type.Turret1 == null)
                                {
                                    flag2 = false;
                                    num++;
                                }
                            }
                            else if (type.TurretCount >= 2)
                            {
                                if (type.Turret2 == null)
                                {
                                    flag2 = false;
                                    num++;
                                }
                                else if (type.Turret2.TurretType != clsTurret.enumTurretType.Weapon)
                                {
                                    flag2 = false;
                                    num++;
                                }
                            }
                            else if ((type.TurretCount >= 3) & (type.Turret3 == null))
                            {
                                if (type.Turret3 == null)
                                {
                                    flag2 = false;
                                    num++;
                                }
                                else if (type.Turret3.TurretType != clsTurret.enumTurretType.Weapon)
                                {
                                    flag2 = false;
                                    num++;
                                }
                            }
                        }
                        if (flag2)
                        {
                            File.SectionName_Append("droid_" + modIO.InvariantToString_uint(current.ID));
                            File.Property_Append("id", modIO.InvariantToString_uint(current.ID));
                            if ((current.UnitGroup == this.ScavengerUnitGroup) | ((PlayerCount >= 0) & (current.UnitGroup.WZ_StartPos >= PlayerCount)))
                            {
                                File.Property_Append("player", "scavenger");
                            }
                            else
                            {
                                File.Property_Append("startpos", modIO.InvariantToString_int(current.UnitGroup.WZ_StartPos));
                            }
                            if (preferPartsOutput)
                            {
                                File.Property_Append("name", type.GenerateName());
                            }
                            else
                            {
                                template = (clsDroidTemplate) type;
                                File.Property_Append("template", template.Code);
                            }
                            File.Property_Append("position", current.GetINIPosition());
                            File.Property_Append("rotation", current.GetINIRotation());
                            if (current.Health < 1.0)
                            {
                                File.Property_Append("health", current.GetINIHealthPercent());
                            }
                            if (preferPartsOutput)
                            {
                                string code;
                                File.Property_Append("droidType", modIO.InvariantToString_int((int) type.GetDroidType()));
                                if (type.TurretCount == 0)
                                {
                                    code = "0";
                                }
                                else if (type.Turret1.TurretType == clsTurret.enumTurretType.Brain)
                                {
                                    if (((clsBrain) type.Turret1).Weapon == null)
                                    {
                                        code = "0";
                                    }
                                    else
                                    {
                                        code = "1";
                                    }
                                }
                                else if (type.Turret1.TurretType == clsTurret.enumTurretType.Weapon)
                                {
                                    code = modIO.InvariantToString_byte(type.TurretCount);
                                }
                                else
                                {
                                    code = "0";
                                }
                                File.Property_Append("weapons", code);
                                File.Property_Append(@"parts\body", type.Body.Code);
                                File.Property_Append(@"parts\propulsion", type.Propulsion.Code);
                                File.Property_Append(@"parts\sensor", type.GetSensorCode());
                                File.Property_Append(@"parts\construct", type.GetConstructCode());
                                File.Property_Append(@"parts\repair", type.GetRepairCode());
                                File.Property_Append(@"parts\brain", type.GetBrainCode());
                                File.Property_Append(@"parts\ecm", type.GetECMCode());
                                if (type.TurretCount >= 1)
                                {
                                    if (type.Turret1.TurretType == clsTurret.enumTurretType.Weapon)
                                    {
                                        File.Property_Append(@"parts\weapon\1", type.Turret1.Code);
                                        if ((type.TurretCount >= 2) && (type.Turret2.TurretType == clsTurret.enumTurretType.Weapon))
                                        {
                                            File.Property_Append(@"parts\weapon\2", type.Turret2.Code);
                                            if ((type.TurretCount >= 3) && (type.Turret3.TurretType == clsTurret.enumTurretType.Weapon))
                                            {
                                                File.Property_Append(@"parts\weapon\3", type.Turret3.Code);
                                            }
                                        }
                                    }
                                    else if (type.Turret1.TurretType == clsTurret.enumTurretType.Brain)
                                    {
                                        clsBrain brain = (clsBrain) type.Turret1;
                                        if (brain.Weapon == null)
                                        {
                                            code = "ZNULLWEAPON";
                                        }
                                        else
                                        {
                                            code = brain.Weapon.Code;
                                        }
                                        File.Property_Append(@"parts\weapon\1", code);
                                    }
                                }
                            }
                            File.Gap_Append();
                        }
                    }
                }
            }
            finally
            {
                if (enumerator is IDisposable)
                {
                    (enumerator as IDisposable).Dispose();
                }
            }
            if (num > 0)
            {
                result.WarningAdd("There were " + Conversions.ToString(num) + " droids with parts missing. They were not saved.");
            }
            return result;
        }

        public clsResult Serialize_WZ_FeaturesINI(clsINIWrite File)
        {
            IEnumerator enumerator;
            clsResult result = new clsResult("Serializing features INI");
            try
            {
                enumerator = this.Units.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    clsUnit current = (clsUnit) enumerator.Current;
                    if (current.Type.Type == clsUnitType.enumType.Feature)
                    {
                        clsFeatureType type = (clsFeatureType) current.Type;
                        bool flag = true;
                        if (current.ID <= 0L)
                        {
                            flag = false;
                            result.WarningAdd("Error. A features's ID was zero. It was NOT saved. Delete and replace it to allow save.");
                        }
                        if (flag)
                        {
                            File.SectionName_Append("feature_" + modIO.InvariantToString_uint(current.ID));
                            File.Property_Append("id", modIO.InvariantToString_uint(current.ID));
                            File.Property_Append("position", current.GetINIPosition());
                            File.Property_Append("rotation", current.GetINIRotation());
                            File.Property_Append("name", type.Code);
                            if (current.Health < 1.0)
                            {
                                File.Property_Append("health", current.GetINIHealthPercent());
                            }
                            File.Gap_Append();
                        }
                    }
                }
            }
            finally
            {
                if (enumerator is IDisposable)
                {
                    (enumerator as IDisposable).Dispose();
                }
            }
            return result;
        }

        public clsResult Serialize_WZ_LabelsINI(clsINIWrite File, int PlayerCount)
        {
            clsResult result = new clsResult("Serializing labels INI");
            try
            {
                IEnumerator enumerator;
                IEnumerator enumerator2;
                IEnumerator enumerator3;
                try
                {
                    enumerator = this.ScriptPositions.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        ((clsScriptPosition) enumerator.Current).WriteWZ(File);
                    }
                }
                finally
                {
                    if (enumerator is IDisposable)
                    {
                        (enumerator as IDisposable).Dispose();
                    }
                }
                try
                {
                    enumerator2 = this.ScriptAreas.GetEnumerator();
                    while (enumerator2.MoveNext())
                    {
                        ((clsScriptArea) enumerator2.Current).WriteWZ(File);
                    }
                }
                finally
                {
                    if (enumerator2 is IDisposable)
                    {
                        (enumerator2 as IDisposable).Dispose();
                    }
                }
                if (PlayerCount < 0)
                {
                    return result;
                }
                try
                {
                    enumerator3 = this.Units.GetEnumerator();
                    while (enumerator3.MoveNext())
                    {
                        ((clsUnit) enumerator3.Current).WriteWZLabel(File, PlayerCount);
                    }
                }
                finally
                {
                    if (enumerator3 is IDisposable)
                    {
                        (enumerator3 as IDisposable).Dispose();
                    }
                }
            }
            catch (Exception exception1)
            {
                ProjectData.SetProjectError(exception1);
                Exception exception = exception1;
                result.WarningAdd(exception.Message);
                ProjectData.ClearProjectError();
            }
            return result;
        }

        public clsResult Serialize_WZ_StructuresINI(clsINIWrite File, int PlayerCount)
        {
            int num;
            clsStructureType type2;
            int num5;
            clsUnit current;
            IEnumerator enumerator;
            clsResult result = new clsResult("Serializing structures INI");
            bool[] flagArray = new bool[(this.Units.Count - 1) + 1];
            int[] numArray = new int[(this.Units.Count - 1) + 1];
            clsStructureType.enumStructureType[] typeArray = new clsStructureType.enumStructureType[2];
            int num2 = 0;
            clsObjectPriorityOrderList list = new clsObjectPriorityOrderList();
            try
            {
                enumerator = this.Units.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    int num7;
                    IEnumerator enumerator2;
                    current = (clsUnit) enumerator.Current;
                    if (current.Type.Type != clsUnitType.enumType.PlayerStructure)
                    {
                        continue;
                    }
                    type2 = (clsStructureType) current.Type;
                    switch (type2.StructureType)
                    {
                        case clsStructureType.enumStructureType.PowerModule:
                            typeArray[0] = clsStructureType.enumStructureType.PowerGenerator;
                            num7 = 1;
                            break;

                        case clsStructureType.enumStructureType.ResearchModule:
                            typeArray[0] = clsStructureType.enumStructureType.Research;
                            num7 = 1;
                            break;

                        case clsStructureType.enumStructureType.FactoryModule:
                            typeArray[0] = clsStructureType.enumStructureType.Factory;
                            typeArray[1] = clsStructureType.enumStructureType.VTOLFactory;
                            num7 = 2;
                            break;

                        default:
                            num7 = 0;
                            break;
                    }
                    if (num7 == 0)
                    {
                        list.SetItem(current);
                        list.ActionPerform();
                        continue;
                    }
                    flagArray[current.MapLink.ArrayPosition] = true;
                    modMath.sXY_int posSectorNum = this.GetPosSectorNum(current.Pos.Horizontal);
                    clsUnit unit3 = null;
                    try
                    {
                        enumerator2 = this.Sectors[posSectorNum.X, posSectorNum.Y].Units.GetEnumerator();
                        while (enumerator2.MoveNext())
                        {
                            clsUnitSectorConnection connection = (clsUnitSectorConnection) enumerator2.Current;
                            clsUnit unit = connection.Unit;
                            if (unit.Type.Type == clsUnitType.enumType.PlayerStructure)
                            {
                                clsStructureType type = (clsStructureType) unit.Type;
                                if (unit.UnitGroup == current.UnitGroup)
                                {
                                    int num8 = num7 - 1;
                                    num = 0;
                                    while (num <= num8)
                                    {
                                        if (type.StructureType == typeArray[num])
                                        {
                                            break;
                                        }
                                        num++;
                                    }
                                    if (num < num7)
                                    {
                                        modMath.sXY_int _int2;
                                        modMath.sXY_int _int3;
                                        modMath.sXY_int _int = type.get_GetFootprintSelected(unit.Rotation);
                                        _int3.X = unit.Pos.Horizontal.X - ((int) Math.Round((double) (((double) (_int.X * 0x80)) / 2.0)));
                                        _int3.Y = unit.Pos.Horizontal.Y - ((int) Math.Round((double) (((double) (_int.Y * 0x80)) / 2.0)));
                                        _int2.X = unit.Pos.Horizontal.X + ((int) Math.Round((double) (((double) (_int.X * 0x80)) / 2.0)));
                                        _int2.Y = unit.Pos.Horizontal.Y + ((int) Math.Round((double) (((double) (_int.Y * 0x80)) / 2.0)));
                                        if (!((((current.Pos.Horizontal.X >= _int3.X) & (current.Pos.Horizontal.X < _int2.X)) & (current.Pos.Horizontal.Y >= _int3.Y)) & (current.Pos.Horizontal.Y < _int2.Y)))
                                        {
                                            continue;
                                        }
                                        int[] numArray2 = numArray;
                                        int arrayPosition = unit.MapLink.ArrayPosition;
                                        numArray2[arrayPosition]++;
                                        unit3 = unit;
                                        goto Label_0355;
                                    }
                                }
                            }
                        }
                    }
                    finally
                    {
                        if (enumerator2 is IDisposable)
                        {
                            (enumerator2 as IDisposable).Dispose();
                        }
                    }
                Label_0355:
                    if (unit3 == null)
                    {
                        num2++;
                    }
                }
            }
            finally
            {
                if (enumerator is IDisposable)
                {
                    (enumerator as IDisposable).Dispose();
                }
            }
            if (num2 > 0)
            {
                result.WarningAdd(Conversions.ToString(num2) + " modules had no underlying structure.");
            }
            int num6 = 0x10;
            int num10 = list.Result.Count - 1;
            for (num = 0; num <= num10; num++)
            {
                int num3;
                int num4;
                current = list.Result[num];
                type2 = (clsStructureType) current.Type;
                if (current.ID <= 0L)
                {
                    result.WarningAdd("Error. A structure's ID was zero. It was NOT saved. Delete and replace it to allow save.");
                    continue;
                }
                File.SectionName_Append("structure_" + modIO.InvariantToString_uint(current.ID));
                File.Property_Append("id", modIO.InvariantToString_uint(current.ID));
                if ((current.UnitGroup == this.ScavengerUnitGroup) | ((PlayerCount >= 0) & (current.UnitGroup.WZ_StartPos >= PlayerCount)))
                {
                    File.Property_Append("player", "scavenger");
                }
                else
                {
                    File.Property_Append("startpos", modIO.InvariantToString_int(current.UnitGroup.WZ_StartPos));
                }
                File.Property_Append("name", type2.Code);
                if (type2.WallLink.IsConnected)
                {
                    File.Property_Append("wall/type", modIO.InvariantToString_int(type2.WallLink.ArrayPosition));
                }
                File.Property_Append("position", current.GetINIPosition());
                File.Property_Append("rotation", current.GetINIRotation());
                if (current.Health < 1.0)
                {
                    File.Property_Append("health", current.GetINIHealthPercent());
                }
                switch (type2.StructureType)
                {
                    case clsStructureType.enumStructureType.Factory:
                        num4 = 2;
                        break;

                    case clsStructureType.enumStructureType.VTOLFactory:
                        num4 = 2;
                        break;

                    case clsStructureType.enumStructureType.PowerGenerator:
                        num4 = 1;
                        break;

                    case clsStructureType.enumStructureType.Research:
                        num4 = 1;
                        break;

                    default:
                        num4 = 0;
                        break;
                }
                if (numArray[current.MapLink.ArrayPosition] > num4)
                {
                    num3 = num4;
                    if (num5 < num6)
                    {
                        result.WarningAdd("Structure " + type2.GetDisplayTextCode() + " at " + current.GetPosText() + " has too many modules (" + Conversions.ToString(numArray[current.MapLink.ArrayPosition]) + ").");
                    }
                    num5++;
                }
                else
                {
                    num3 = numArray[current.MapLink.ArrayPosition];
                }
                File.Property_Append("modules", modIO.InvariantToString_int(num3));
                File.Gap_Append();
            }
            if (num5 > num6)
            {
                result.WarningAdd(Conversions.ToString(num5) + " structures had too many modules.");
            }
            return result;
        }

        public void SetChanged()
        {
            this.ChangedSinceSave = true;
            ChangedEventHandler changedEvent = this.ChangedEvent;
            if (changedEvent != null)
            {
                changedEvent();
            }
            clsAutoSave autoSave = this.AutoSave;
            autoSave.ChangeCount++;
            this.AutoSaveTest();
        }

        public void SetObjectCreatorDefaults(clsUnitCreate objectCreator)
        {
            objectCreator.Map = this;
            objectCreator.ObjectType = modMain.frmMainInstance.SingleSelectedObjectType;
            objectCreator.AutoWalls = modMain.frmMainInstance.cbxAutoWalls.Checked;
            objectCreator.UnitGroup = this.SelectedUnitGroup.Item;
            try
            {
                int num;
                modIO.InvariantParse_int(modMain.frmMainInstance.txtNewObjectRotation.Text, ref num);
                if ((num < 0) | (num > 0x167))
                {
                    objectCreator.Rotation = 0;
                }
                else
                {
                    objectCreator.Rotation = num;
                }
            }
            catch (Exception exception1)
            {
                ProjectData.SetProjectError(exception1);
                objectCreator.Rotation = 0;
                ProjectData.ClearProjectError();
            }
            objectCreator.RandomizeRotation = modMain.frmMainInstance.cbxObjectRandomRotation.Checked;
        }

        public void SetPainterToDefaults()
        {
            if (this.Tileset == null)
            {
                this.Painter = new clsPainter();
            }
            else if (this.Tileset == modProgram.Tileset_Arizona)
            {
                this.Painter = modProgram.Painter_Arizona;
            }
            else if (this.Tileset == modProgram.Tileset_Urban)
            {
                this.Painter = modProgram.Painter_Urban;
            }
            else if (this.Tileset == modProgram.Tileset_Rockies)
            {
                this.Painter = modProgram.Painter_Rockies;
            }
            else
            {
                this.Painter = new clsPainter();
            }
        }

        public void SetTabText()
        {
            string title = this.GetTitle();
            if (title.Length > 0x18)
            {
                title = Strings.Left(title, 0x15) + "...";
            }
            this.MapView_TabPage.Text = title;
        }

        public void ShadowSector_Create(modMath.sXY_int SectorNum)
        {
            int num5;
            int num6;
            int num7;
            int num8;
            clsShadowSector sector = new clsShadowSector();
            this.ShadowSectors[SectorNum.X, SectorNum.Y] = sector;
            sector.Num = SectorNum;
            int num3 = SectorNum.X * 8;
            int num4 = SectorNum.Y * 8;
            int num = Math.Min(8, this.Terrain.TileSize.X - num3);
            int num2 = Math.Min(8, this.Terrain.TileSize.Y - num4);
            int num9 = num2;
            for (num8 = 0; num8 <= num9; num8++)
            {
                int num10 = num;
                num7 = 0;
                while (num7 <= num10)
                {
                    num5 = num3 + num7;
                    num6 = num4 + num8;
                    sector.Terrain.Vertices[num7, num8].Height = this.Terrain.Vertices[num5, num6].Height;
                    sector.Terrain.Vertices[num7, num8].Terrain = this.Terrain.Vertices[num5, num6].Terrain;
                    num7++;
                }
            }
            int num11 = num2 - 1;
            for (num8 = 0; num8 <= num11; num8++)
            {
                int num12 = num - 1;
                num7 = 0;
                while (num7 <= num12)
                {
                    num5 = num3 + num7;
                    num6 = num4 + num8;
                    sector.Terrain.Tiles[num7, num8].Copy(this.Terrain.Tiles[num5, num6]);
                    num7++;
                }
            }
            int num13 = num2;
            for (num8 = 0; num8 <= num13; num8++)
            {
                int num14 = num - 1;
                num7 = 0;
                while (num7 <= num14)
                {
                    num5 = num3 + num7;
                    num6 = num4 + num8;
                    sector.Terrain.SideH[num7, num8].Road = this.Terrain.SideH[num5, num6].Road;
                    num7++;
                }
            }
            int num15 = num2 - 1;
            for (num8 = 0; num8 <= num15; num8++)
            {
                int num16 = num;
                for (num7 = 0; num7 <= num16; num7++)
                {
                    num5 = num3 + num7;
                    num6 = num4 + num8;
                    sector.Terrain.SideV[num7, num8].Road = this.Terrain.SideV[num5, num6].Road;
                }
            }
        }

        public bool SideHIsCliffOnBothSides(modMath.sXY_int SideNum)
        {
            modMath.sXY_int _int;
            if (SideNum.Y > 0)
            {
                _int.X = SideNum.X;
                _int.Y = SideNum.Y - 1;
                if (this.Terrain.Tiles[_int.X, _int.Y].Tri)
                {
                    if (!this.Terrain.Tiles[_int.X, _int.Y].TriBottomRightIsCliff)
                    {
                        return false;
                    }
                }
                else if (!this.Terrain.Tiles[_int.X, _int.Y].TriBottomLeftIsCliff)
                {
                    return false;
                }
            }
            if (SideNum.Y < this.Terrain.TileSize.Y)
            {
                _int.X = SideNum.X;
                _int.Y = SideNum.Y;
                if (this.Terrain.Tiles[_int.X, _int.Y].Tri)
                {
                    if (!this.Terrain.Tiles[_int.X, _int.Y].TriTopLeftIsCliff)
                    {
                        return false;
                    }
                }
                else if (!this.Terrain.Tiles[_int.X, _int.Y].TriTopRightIsCliff)
                {
                    return false;
                }
            }
            return true;
        }

        public bool SideVIsCliffOnBothSides(modMath.sXY_int SideNum)
        {
            modMath.sXY_int _int;
            if (SideNum.X > 0)
            {
                _int.X = SideNum.X - 1;
                _int.Y = SideNum.Y;
                if (this.Terrain.Tiles[_int.X, _int.Y].Tri)
                {
                    if (!this.Terrain.Tiles[_int.X, _int.Y].TriBottomRightIsCliff)
                    {
                        return false;
                    }
                }
                else if (!this.Terrain.Tiles[_int.X, _int.Y].TriTopRightIsCliff)
                {
                    return false;
                }
            }
            if (SideNum.X < this.Terrain.TileSize.X)
            {
                _int.X = SideNum.X;
                _int.Y = SideNum.Y;
                if (this.Terrain.Tiles[_int.X, _int.Y].Tri)
                {
                    if (!this.Terrain.Tiles[_int.X, _int.Y].TriTopLeftIsCliff)
                    {
                        return false;
                    }
                }
                else if (!this.Terrain.Tiles[_int.X, _int.Y].TriBottomLeftIsCliff)
                {
                    return false;
                }
            }
            return true;
        }

        protected void TerrainBlank(modMath.sXY_int TileSize)
        {
            this.Terrain = new clsTerrain(TileSize);
            this.SectorCount.X = (int) Math.Round(Math.Ceiling((double) (((double) this.Terrain.TileSize.X) / 8.0)));
            this.SectorCount.Y = (int) Math.Round(Math.Ceiling((double) (((double) this.Terrain.TileSize.Y) / 8.0)));
            this.Sectors = new clsSector[(this.SectorCount.X - 1) + 1, (this.SectorCount.Y - 1) + 1];
            int num3 = this.SectorCount.Y - 1;
            for (int i = 0; i <= num3; i++)
            {
                int num4 = this.SectorCount.X - 1;
                for (int j = 0; j <= num4; j++)
                {
                    modMath.sXY_int newPos = new modMath.sXY_int(j, i);
                    this.Sectors[j, i] = new clsSector(newPos);
                }
            }
        }

        public void TerrainInterpretUpdate()
        {
            clsApplyVertexTerrainInterpret tool = new clsApplyVertexTerrainInterpret();
            clsApplyTileTerrainInterpret interpret3 = new clsApplyTileTerrainInterpret();
            clsApplySideHTerrainInterpret interpret = new clsApplySideHTerrainInterpret();
            clsApplySideVTerrainInterpret interpret2 = new clsApplySideVTerrainInterpret();
            tool.Map = this;
            interpret3.Map = this;
            interpret.Map = this;
            interpret2.Map = this;
            this.TerrainInterpretChanges.Vertices.PerformTool(tool);
            this.TerrainInterpretChanges.Tiles.PerformTool(interpret3);
            this.TerrainInterpretChanges.SidesH.PerformTool(interpret);
            this.TerrainInterpretChanges.SidesV.PerformTool(interpret2);
            this.TerrainInterpretChanges.ClearAll();
        }

        public void TerrainResize(modMath.sXY_int Offset, modMath.sXY_int Size)
        {
            clsGateway gateway;
            clsUnit current;
            int num8;
            int num9;
            IEnumerator enumerator;
            IEnumerator enumerator2;
            IEnumerator enumerator3;
            IEnumerator enumerator4;
            IEnumerator enumerator5;
            IEnumerator enumerator6;
            clsTerrain terrain = new clsTerrain(Size);
            int num6 = Math.Max(0 - Offset.X, 0);
            int num7 = Math.Max(0 - Offset.Y, 0);
            int num = Math.Min(this.Terrain.TileSize.X - Offset.X, Size.X);
            int num2 = Math.Min(this.Terrain.TileSize.Y - Offset.Y, Size.Y);
            int num10 = terrain.TileSize.Y - 1;
            for (num9 = 0; num9 <= num10; num9++)
            {
                int num11 = terrain.TileSize.X - 1;
                num8 = 0;
                while (num8 <= num11)
                {
                    terrain.Tiles[num8, num9].Texture.TextureNum = -1;
                    num8++;
                }
            }
            int num12 = num2;
            for (num9 = num7; num9 <= num12; num9++)
            {
                int num13 = num;
                num8 = num6;
                while (num8 <= num13)
                {
                    terrain.Vertices[num8, num9].Height = this.Terrain.Vertices[Offset.X + num8, Offset.Y + num9].Height;
                    terrain.Vertices[num8, num9].Terrain = this.Terrain.Vertices[Offset.X + num8, Offset.Y + num9].Terrain;
                    num8++;
                }
            }
            int num14 = num2 - 1;
            for (num9 = num7; num9 <= num14; num9++)
            {
                int num15 = num - 1;
                num8 = num6;
                while (num8 <= num15)
                {
                    terrain.Tiles[num8, num9].Copy(this.Terrain.Tiles[Offset.X + num8, Offset.Y + num9]);
                    num8++;
                }
            }
            int num16 = num2;
            for (num9 = num7; num9 <= num16; num9++)
            {
                int num17 = num - 1;
                num8 = num6;
                while (num8 <= num17)
                {
                    terrain.SideH[num8, num9].Road = this.Terrain.SideH[Offset.X + num8, Offset.Y + num9].Road;
                    num8++;
                }
            }
            int num18 = num2 - 1;
            for (num9 = num7; num9 <= num18; num9++)
            {
                int num19 = num;
                for (num8 = num6; num8 <= num19; num8++)
                {
                    terrain.SideV[num8, num9].Road = this.Terrain.SideV[Offset.X + num8, Offset.Y + num9].Road;
                }
            }
            int num3 = (0 - Offset.X) * 0x80;
            int num4 = (0 - Offset.Y) * 0x80;
            try
            {
                enumerator = this.Units.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    current = (clsUnit) enumerator.Current;
                    clsUnit unit2 = current;
                    unit2.Pos.Horizontal.X += num3;
                    unit2 = current;
                    unit2.Pos.Horizontal.Y += num4;
                }
            }
            finally
            {
                if (enumerator is IDisposable)
                {
                    (enumerator as IDisposable).Dispose();
                }
            }
            try
            {
                enumerator2 = this.Gateways.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    gateway = (clsGateway) enumerator2.Current;
                    clsGateway gateway2 = gateway;
                    gateway2.PosA.X -= Offset.X;
                    gateway2 = gateway;
                    gateway2.PosA.Y -= Offset.Y;
                    gateway2 = gateway;
                    gateway2.PosB.X -= Offset.X;
                    gateway2 = gateway;
                    gateway2.PosB.Y -= Offset.Y;
                }
            }
            finally
            {
                if (enumerator2 is IDisposable)
                {
                    (enumerator2 as IDisposable).Dispose();
                }
            }
            modMath.sXY_int startTile = new modMath.sXY_int(0, 0);
            try
            {
                enumerator3 = this.Units.GetItemsAsSimpleList().GetEnumerator();
                while (enumerator3.MoveNext())
                {
                    current = (clsUnit) enumerator3.Current;
                    int arrayPosition = current.MapLink.ArrayPosition;
                    if (!modProgram.PosIsWithinTileArea(this.Units[arrayPosition].Pos.Horizontal, startTile, terrain.TileSize))
                    {
                        this.UnitRemove(arrayPosition);
                    }
                }
            }
            finally
            {
                if (enumerator3 is IDisposable)
                {
                    (enumerator3 as IDisposable).Dispose();
                }
            }
            this.Terrain = terrain;
            try
            {
                enumerator4 = this.Gateways.GetItemsAsSimpleList().GetEnumerator();
                while (enumerator4.MoveNext())
                {
                    gateway = (clsGateway) enumerator4.Current;
                    if (gateway.IsOffMap())
                    {
                        gateway.Deallocate();
                    }
                }
            }
            finally
            {
                if (enumerator4 is IDisposable)
                {
                    (enumerator4 as IDisposable).Dispose();
                }
            }
            modMath.sXY_int posOffset = new modMath.sXY_int(Offset.X * 0x80, Offset.Y * 0x80);
            try
            {
                enumerator5 = this.ScriptPositions.GetItemsAsSimpleList().GetEnumerator();
                while (enumerator5.MoveNext())
                {
                    ((clsScriptPosition) enumerator5.Current).MapResizing(posOffset);
                }
            }
            finally
            {
                if (enumerator5 is IDisposable)
                {
                    (enumerator5 as IDisposable).Dispose();
                }
            }
            try
            {
                enumerator6 = this.ScriptAreas.GetItemsAsSimpleList().GetEnumerator();
                while (enumerator6.MoveNext())
                {
                    ((clsScriptArea) enumerator6.Current).MapResizing(posOffset);
                }
            }
            finally
            {
                if (enumerator6 is IDisposable)
                {
                    (enumerator6 as IDisposable).Dispose();
                }
            }
            if (this._ReadyForUserInput)
            {
                this.CancelUserInput();
                this.InitializeUserInput();
            }
        }

        public modMath.sXYZ_sng TerrainVertexNormalCalc(int X, int Y)
        {
            modMath.sXYZ_sng _sng;
            Position.XYZ_dbl _dbl;
            Position.XYZ_dbl _dbl2;
            int num6 = modMath.Clamp_int(X - 1, 0, this.Terrain.TileSize.X);
            int num7 = modMath.Clamp_int(Y, 0, this.Terrain.TileSize.Y);
            int height = this.Terrain.Vertices[num6, num7].Height;
            num6 = modMath.Clamp_int(X + 1, 0, this.Terrain.TileSize.X);
            num7 = modMath.Clamp_int(Y, 0, this.Terrain.TileSize.Y);
            int num3 = this.Terrain.Vertices[num6, num7].Height;
            num6 = modMath.Clamp_int(X, 0, this.Terrain.TileSize.X);
            num7 = modMath.Clamp_int(Y - 1, 0, this.Terrain.TileSize.Y);
            int num4 = this.Terrain.Vertices[num6, num7].Height;
            num6 = modMath.Clamp_int(X, 0, this.Terrain.TileSize.X);
            num7 = modMath.Clamp_int(Y + 1, 0, this.Terrain.TileSize.Y);
            int num5 = this.Terrain.Vertices[num6, num7].Height;
            _dbl.X = (height - num3) * this.HeightMultiplier;
            _dbl.Y = 256.0;
            _dbl.Z = 0.0;
            _dbl2.X = 0.0;
            _dbl2.Y = 256.0;
            _dbl2.Z = (num4 - num5) * this.HeightMultiplier;
            _dbl.X += _dbl2.X;
            _dbl.Y += _dbl2.Y;
            _dbl.Z += _dbl2.Z;
            double num = Math.Sqrt(((_dbl.X * _dbl.X) + (_dbl.Y * _dbl.Y)) + (_dbl.Z * _dbl.Z));
            _sng.X = (float) (_dbl.X / num);
            _sng.Y = (float) (_dbl.Y / num);
            _sng.Z = (float) (_dbl.Z / num);
            return _sng;
        }

        public modProgram.sWorldPos TileAlignedPos(modMath.sXY_int TileNum, modMath.sXY_int Footprint)
        {
            modProgram.sWorldPos pos;
            pos.Horizontal.X = (int) Math.Round((double) ((TileNum.X + (((double) Footprint.X) / 2.0)) * 128.0));
            pos.Horizontal.Y = (int) Math.Round((double) ((TileNum.Y + (((double) Footprint.Y) / 2.0)) * 128.0));
            pos.Altitude = (int) Math.Round(this.GetTerrainHeight(pos.Horizontal));
            return pos;
        }

        public modProgram.sWorldPos TileAlignedPosFromMapPos(modMath.sXY_int Horizontal, modMath.sXY_int Footprint)
        {
            modProgram.sWorldPos pos;
            pos.Horizontal.X = (int) Math.Round((double) ((Math.Round((double) ((Horizontal.X - (((double) (Footprint.X * 0x80)) / 2.0)) / 128.0)) + (((double) Footprint.X) / 2.0)) * 128.0));
            pos.Horizontal.Y = (int) Math.Round((double) ((Math.Round((double) ((Horizontal.Y - (((double) (Footprint.Y * 0x80)) / 2.0)) / 128.0)) + (((double) Footprint.Y) / 2.0)) * 128.0));
            pos.Altitude = (int) Math.Round(this.GetTerrainHeight(pos.Horizontal));
            return pos;
        }

        public void TileNeedsInterpreting(modMath.sXY_int Pos)
        {
            this.TerrainInterpretChanges.Tiles.Changed(Pos);
            modMath.sXY_int num = new modMath.sXY_int(Pos.X, Pos.Y);
            this.TerrainInterpretChanges.Vertices.Changed(num);
            num = new modMath.sXY_int(Pos.X + 1, Pos.Y);
            this.TerrainInterpretChanges.Vertices.Changed(num);
            num = new modMath.sXY_int(Pos.X, Pos.Y + 1);
            this.TerrainInterpretChanges.Vertices.Changed(num);
            num = new modMath.sXY_int(Pos.X + 1, Pos.Y + 1);
            this.TerrainInterpretChanges.Vertices.Changed(num);
            num = new modMath.sXY_int(Pos.X, Pos.Y);
            this.TerrainInterpretChanges.SidesH.Changed(num);
            num = new modMath.sXY_int(Pos.X, Pos.Y + 1);
            this.TerrainInterpretChanges.SidesH.Changed(num);
            num = new modMath.sXY_int(Pos.X, Pos.Y);
            this.TerrainInterpretChanges.SidesV.Changed(num);
            num = new modMath.sXY_int(Pos.X + 1, Pos.Y);
            this.TerrainInterpretChanges.SidesV.Changed(num);
        }

        public modMath.sXY_int TileNumClampToMap(modMath.sXY_int TileNum)
        {
            modMath.sXY_int _int;
            _int.X = modMath.Clamp_int(TileNum.X, 0, this.Terrain.TileSize.X - 1);
            _int.Y = modMath.Clamp_int(TileNum.Y, 0, this.Terrain.TileSize.Y - 1);
            return _int;
        }

        public void TileTextureChangeTerrainAction(modMath.sXY_int Pos, modProgram.enumTextureTerrainAction Action)
        {
            switch (Action)
            {
                case modProgram.enumTextureTerrainAction.Reinterpret:
                    this.TileNeedsInterpreting(Pos);
                    break;

                case modProgram.enumTextureTerrainAction.Remove:
                    this.Terrain.Vertices[Pos.X, Pos.Y].Terrain = null;
                    this.Terrain.Vertices[Pos.X + 1, Pos.Y].Terrain = null;
                    this.Terrain.Vertices[Pos.X, Pos.Y + 1].Terrain = null;
                    this.Terrain.Vertices[Pos.X + 1, Pos.Y + 1].Terrain = null;
                    break;
            }
        }

        public void TileType_Reset()
        {
            if (this.Tileset == null)
            {
                this.Tile_TypeNum = new byte[0];
            }
            else
            {
                this.Tile_TypeNum = new byte[(this.Tileset.TileCount - 1) + 1];
                int num2 = this.Tileset.TileCount - 1;
                for (int i = 0; i <= num2; i++)
                {
                    this.Tile_TypeNum[i] = this.Tileset.Tiles[i].Default_Type;
                }
            }
        }

        public void Undo_Sector_Rejoin(clsShadowSector Shadow_Sector_To_Rejoin)
        {
            int num5;
            int num6;
            int num7;
            int num8;
            int num3 = Shadow_Sector_To_Rejoin.Num.X * 8;
            int num4 = Shadow_Sector_To_Rejoin.Num.Y * 8;
            int num = Math.Min(8, this.Terrain.TileSize.X - num3);
            int num2 = Math.Min(8, this.Terrain.TileSize.Y - num4);
            int num9 = num2;
            for (num8 = 0; num8 <= num9; num8++)
            {
                int num10 = num;
                num7 = 0;
                while (num7 <= num10)
                {
                    num5 = num3 + num7;
                    num6 = num4 + num8;
                    this.Terrain.Vertices[num5, num6].Height = Shadow_Sector_To_Rejoin.Terrain.Vertices[num7, num8].Height;
                    this.Terrain.Vertices[num5, num6].Terrain = Shadow_Sector_To_Rejoin.Terrain.Vertices[num7, num8].Terrain;
                    num7++;
                }
            }
            int num11 = num2 - 1;
            for (num8 = 0; num8 <= num11; num8++)
            {
                int num12 = num - 1;
                num7 = 0;
                while (num7 <= num12)
                {
                    num5 = num3 + num7;
                    num6 = num4 + num8;
                    this.Terrain.Tiles[num5, num6].Copy(Shadow_Sector_To_Rejoin.Terrain.Tiles[num7, num8]);
                    num7++;
                }
            }
            int num13 = num2;
            for (num8 = 0; num8 <= num13; num8++)
            {
                int num14 = num - 1;
                num7 = 0;
                while (num7 <= num14)
                {
                    num5 = num3 + num7;
                    num6 = num4 + num8;
                    this.Terrain.SideH[num5, num6].Road = Shadow_Sector_To_Rejoin.Terrain.SideH[num7, num8].Road;
                    num7++;
                }
            }
            int num15 = num2 - 1;
            for (num8 = 0; num8 <= num15; num8++)
            {
                int num16 = num;
                for (num7 = 0; num7 <= num16; num7++)
                {
                    num5 = num3 + num7;
                    num6 = num4 + num8;
                    this.Terrain.SideV[num5, num6].Road = Shadow_Sector_To_Rejoin.Terrain.SideV[num7, num8].Road;
                }
            }
        }

        public void UndoClear()
        {
            IEnumerator enumerator;
            this.UndoStepCreate("");
            try
            {
                enumerator = this.Undos.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    IEnumerator enumerator2;
                    clsUndo current = (clsUndo) enumerator.Current;
                    try
                    {
                        enumerator2 = current.UnitChanges.GetEnumerator();
                        while (enumerator2.MoveNext())
                        {
                            clsUnitChange change = (clsUnitChange) enumerator2.Current;
                            change.Unit.Deallocate();
                        }
                        continue;
                    }
                    finally
                    {
                        if (enumerator2 is IDisposable)
                        {
                            (enumerator2 as IDisposable).Dispose();
                        }
                    }
                }
            }
            finally
            {
                if (enumerator is IDisposable)
                {
                    (enumerator as IDisposable).Dispose();
                }
            }
            this.Undos.Clear();
            this.UndoPosition = this.Undos.Count;
        }

        public void UndoPerform()
        {
            IEnumerator enumerator;
            this.UndoStepCreate("Incomplete Action");
            this.UndoPosition--;
            clsUndo undo = this.Undos[this.UndoPosition];
            modLists.SimpleList<clsShadowSector> list = new modLists.SimpleList<clsShadowSector>();
            try
            {
                enumerator = undo.ChangedSectors.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    clsShadowSector current = (clsShadowSector) enumerator.Current;
                    modMath.sXY_int sectorNum = current.Num;
                    clsShadowSector newItem = this.ShadowSectors[sectorNum.X, sectorNum.Y];
                    this.Sectors[sectorNum.X, sectorNum.Y].DeleteLists();
                    this.Undo_Sector_Rejoin(current);
                    this.ShadowSector_Create(sectorNum);
                    list.Add(newItem);
                    this.SectorGraphicsChanges.Changed(sectorNum);
                }
            }
            finally
            {
                if (enumerator is IDisposable)
                {
                    (enumerator as IDisposable).Dispose();
                }
            }
            undo.ChangedSectors = list;
            clsUnitAdd add = new clsUnitAdd {
                Map = this
            };
            for (int i = undo.UnitChanges.Count - 1; i >= 0; i += -1)
            {
                clsUnit iDUnit = undo.UnitChanges[i].Unit;
                switch (undo.UnitChanges[i].Type)
                {
                    case clsUnitChange.enumType.Added:
                        this.UnitRemove(iDUnit.MapLink.ArrayPosition);
                        break;

                    case clsUnitChange.enumType.Deleted:
                    {
                        uint iD = iDUnit.ID;
                        add.ID = iD;
                        add.NewUnit = iDUnit;
                        add.Perform();
                        modProgram.ErrorIDChange(iD, iDUnit, "Undo_Perform");
                        break;
                    }
                    default:
                        Debugger.Break();
                        break;
                }
            }
            for (int j = undo.GatewayChanges.Count - 1; j >= 0; j += -1)
            {
                clsGatewayChange change = undo.GatewayChanges[j];
                switch (change.Type)
                {
                    case clsGatewayChange.enumType.Added:
                        change.Gateway.MapLink.Disconnect();
                        break;

                    case clsGatewayChange.enumType.Deleted:
                        change.Gateway.MapLink.Connect(this.Gateways);
                        break;

                    default:
                        Debugger.Break();
                        break;
                }
            }
            this.SectorsUpdateGraphics();
            this.MinimapMakeLater();
            modMain.frmMainInstance.SelectedObject_Changed();
        }

        public void UndoStepCreate(string StepName)
        {
            IEnumerator enumerator;
            clsUndo newItem = new clsUndo {
                Name = StepName
            };
            try
            {
                enumerator = this.SectorTerrainUndoChanges.ChangedPoints.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    modMath.clsXY_int current = (modMath.clsXY_int) enumerator.Current;
                    newItem.ChangedSectors.Add(this.ShadowSectors[current.X, current.Y]);
                    this.ShadowSector_Create(current.XY);
                }
            }
            finally
            {
                if (enumerator is IDisposable)
                {
                    (enumerator as IDisposable).Dispose();
                }
            }
            this.SectorTerrainUndoChanges.Clear();
            newItem.UnitChanges.AddSimpleList(this.UnitChanges);
            this.UnitChanges.Clear();
            newItem.GatewayChanges.AddSimpleList(this.GatewayChanges);
            this.GatewayChanges.Clear();
            if (((newItem.ChangedSectors.Count + newItem.UnitChanges.Count) + newItem.GatewayChanges.Count) > 0)
            {
                while (this.Undos.Count > this.UndoPosition)
                {
                    this.Undos.Remove(this.Undos.Count - 1);
                }
                this.Undos.Add(newItem);
                this.UndoPosition = this.Undos.Count;
                this.SetChanged();
            }
        }

        public void UnitRemove(int Num)
        {
            clsUnit unitToUpdateFor = this.Units[Num];
            if (this.SectorGraphicsChanges != null)
            {
                this.UnitSectorsGraphicsChanged(unitToUpdateFor);
            }
            if (this.ViewInfo != null)
            {
                clsViewInfo.clsMouseOver.clsOverTerrain mouseOverTerrain = this.ViewInfo.GetMouseOverTerrain();
                if (mouseOverTerrain != null)
                {
                    int position = mouseOverTerrain.Units.FindFirstItemPosition(unitToUpdateFor);
                    if (position >= 0)
                    {
                        mouseOverTerrain.Units.Remove(position);
                    }
                }
            }
            unitToUpdateFor.DisconnectFromMap();
        }

        public void UnitRemoveStoreChange(int Num)
        {
            clsUnitChange newItem = new clsUnitChange {
                Type = clsUnitChange.enumType.Deleted,
                Unit = this.Units[Num]
            };
            this.UnitChanges.Add(newItem);
            this.UnitRemove(Num);
        }

        public void UnitSectorsCalc(clsUnit Unit)
        {
            modMath.sXY_int _int3;
            modMath.sXY_int _int4;
            this.GetFootprintTileRangeClamped(Unit.Pos.Horizontal, Unit.Type.get_GetFootprintSelected(Unit.Rotation), ref _int4, ref _int3);
            modMath.sXY_int tileSectorNum = this.GetTileSectorNum(_int4);
            modMath.sXY_int _int = this.GetTileSectorNum(_int3);
            tileSectorNum.X = modMath.Clamp_int(tileSectorNum.X, 0, this.SectorCount.X - 1);
            tileSectorNum.Y = modMath.Clamp_int(tileSectorNum.Y, 0, this.SectorCount.Y - 1);
            _int.X = modMath.Clamp_int(_int.X, 0, this.SectorCount.X - 1);
            _int.Y = modMath.Clamp_int(_int.Y, 0, this.SectorCount.Y - 1);
            Unit.Sectors.Clear();
            int y = _int.Y;
            for (int i = tileSectorNum.Y; i <= y; i++)
            {
                int x = _int.X;
                for (int j = tileSectorNum.X; j <= x; j++)
                {
                    clsUnitSectorConnection connection = clsUnitSectorConnection.Create(Unit, this.Sectors[j, i]);
                }
            }
        }

        private void UnitSectorsGraphicsChanged(clsUnit UnitToUpdateFor)
        {
            if (this.SectorGraphicsChanges == null)
            {
                Debugger.Break();
            }
            else
            {
                IEnumerator enumerator;
                try
                {
                    enumerator = UnitToUpdateFor.Sectors.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        clsUnitSectorConnection current = (clsUnitSectorConnection) enumerator.Current;
                        this.SectorGraphicsChanges.Changed(current.Sector.Pos);
                    }
                }
                finally
                {
                    if (enumerator is IDisposable)
                    {
                        (enumerator as IDisposable).Dispose();
                    }
                }
            }
        }

        public void UnitSwap(clsUnit OldUnit, clsUnit NewUnit)
        {
            if (OldUnit.MapLink.Source != this)
            {
                Debugger.Break();
            }
            else
            {
                this.UnitRemoveStoreChange(OldUnit.MapLink.ArrayPosition);
                new clsUnitAdd { Map = this, StoreChange = true, ID = OldUnit.ID, NewUnit = NewUnit, Label = OldUnit.Label }.Perform();
                modProgram.ErrorIDChange(OldUnit.ID, NewUnit, "UnitSwap");
            }
        }

        public void Update()
        {
            bool suppressMinimap = this.SuppressMinimap;
            this.SuppressMinimap = true;
            this.UpdateAutoTextures();
            this.TerrainInterpretUpdate();
            this.SectorsUpdateGraphics();
            this.SectorsUpdateUnitHeights();
            this.SuppressMinimap = suppressMinimap;
        }

        public void UpdateAutoTextures()
        {
            clsUpdateAutotexture tool = new clsUpdateAutotexture {
                Map = this,
                MakeInvalidTiles = modMain.frmMainInstance.cbxInvalidTiles.Checked
            };
            this.AutoTextureChanges.PerformTool(tool);
            this.AutoTextureChanges.Clear();
        }

        public bool VertexIsCliffEdge(modMath.sXY_int VertexNum)
        {
            modMath.sXY_int _int;
            if (VertexNum.X > 0)
            {
                if (VertexNum.Y > 0)
                {
                    _int.X = VertexNum.X - 1;
                    _int.Y = VertexNum.Y - 1;
                    if (this.Terrain.Tiles[_int.X, _int.Y].Terrain_IsCliff)
                    {
                        return true;
                    }
                }
                if (VertexNum.Y < this.Terrain.TileSize.Y)
                {
                    _int.X = VertexNum.X - 1;
                    _int.Y = VertexNum.Y;
                    if (this.Terrain.Tiles[_int.X, _int.Y].Terrain_IsCliff)
                    {
                        return true;
                    }
                }
            }
            if (VertexNum.X < this.Terrain.TileSize.X)
            {
                if (VertexNum.Y > 0)
                {
                    _int.X = VertexNum.X;
                    _int.Y = VertexNum.Y - 1;
                    if (this.Terrain.Tiles[_int.X, _int.Y].Terrain_IsCliff)
                    {
                        return true;
                    }
                }
                if (VertexNum.Y < this.Terrain.TileSize.Y)
                {
                    _int.X = VertexNum.X;
                    _int.Y = VertexNum.Y;
                    if (this.Terrain.Tiles[_int.X, _int.Y].Terrain_IsCliff)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void WaterTriCorrection()
        {
            if (this.Tileset != null)
            {
                int num3 = this.Terrain.TileSize.Y - 1;
                for (int i = 0; i <= num3; i++)
                {
                    modMath.sXY_int _int;
                    _int.Y = i;
                    int num4 = this.Terrain.TileSize.X - 1;
                    for (int j = 0; j <= num4; j++)
                    {
                        _int.X = j;
                        if ((this.Terrain.Tiles[j, i].Tri && (this.Terrain.Tiles[j, i].Texture.TextureNum >= 0)) && (this.Tileset.Tiles[this.Terrain.Tiles[j, i].Texture.TextureNum].Default_Type == 7))
                        {
                            this.Terrain.Tiles[j, i].Tri = false;
                            this.SectorGraphicsChanges.TileChanged(_int);
                            this.SectorTerrainUndoChanges.TileChanged(_int);
                        }
                    }
                }
            }
        }

        public clsResult Write_FMap(string Path, bool Overwrite, bool Compress)
        {
            FileStream stream;
            clsResult result = new clsResult("Writing FMap to \"" + Path + "\"");
            if (!Overwrite && File.Exists(Path))
            {
                result.ProblemAdd("The file already exists");
                return result;
            }
            try
            {
                stream = File.Create(Path);
            }
            catch (Exception exception1)
            {
                ProjectData.SetProjectError(exception1);
                Exception exception = exception1;
                result.ProblemAdd("Unable to create file");
                clsResult result2 = result;
                ProjectData.ClearProjectError();
                return result2;
            }
            ZipOutputStream output = new ZipOutputStream(stream) {
                UseZip64 = UseZip64.Off
            };
            if (Compress)
            {
                output.SetLevel(9);
            }
            else
            {
                output.SetLevel(0);
            }
            BinaryWriter file = new BinaryWriter(output, modProgram.UTF8Encoding);
            StreamWriter writer2 = new StreamWriter(output, modProgram.UTF8Encoding);
            string path = "Info.ini";
            if (modIO.ZipMakeEntry(output, path, result) != null)
            {
                clsINIWrite write = new clsINIWrite {
                    File = writer2
                };
                result.Add(this.Serialize_FMap_Info(write));
                writer2.Flush();
                output.CloseEntry();
            }
            path = "VertexHeight.dat";
            if (modIO.ZipMakeEntry(output, path, result) != null)
            {
                result.Add(this.Serialize_FMap_VertexHeight(file));
                file.Flush();
                output.CloseEntry();
            }
            path = "VertexTerrain.dat";
            if (modIO.ZipMakeEntry(output, path, result) != null)
            {
                result.Add(this.Serialize_FMap_VertexTerrain(file));
                file.Flush();
                output.CloseEntry();
            }
            path = "TileTexture.dat";
            if (modIO.ZipMakeEntry(output, path, result) != null)
            {
                result.Add(this.Serialize_FMap_TileTexture(file));
                file.Flush();
                output.CloseEntry();
            }
            path = "TileOrientation.dat";
            if (modIO.ZipMakeEntry(output, path, result) != null)
            {
                result.Add(this.Serialize_FMap_TileOrientation(file));
                file.Flush();
                output.CloseEntry();
            }
            path = "TileCliff.dat";
            if (modIO.ZipMakeEntry(output, path, result) != null)
            {
                result.Add(this.Serialize_FMap_TileCliff(file));
                file.Flush();
                output.CloseEntry();
            }
            path = "Roads.dat";
            if (modIO.ZipMakeEntry(output, path, result) != null)
            {
                result.Add(this.Serialize_FMap_Roads(file));
                file.Flush();
                output.CloseEntry();
            }
            path = "Objects.ini";
            if (modIO.ZipMakeEntry(output, path, result) != null)
            {
                clsINIWrite write2 = new clsINIWrite {
                    File = writer2
                };
                result.Add(this.Serialize_FMap_Objects(write2));
                writer2.Flush();
                output.CloseEntry();
            }
            path = "Gateways.ini";
            if (modIO.ZipMakeEntry(output, path, result) != null)
            {
                clsINIWrite write3 = new clsINIWrite {
                    File = writer2
                };
                result.Add(this.Serialize_FMap_Gateways(write3));
                writer2.Flush();
                output.CloseEntry();
            }
            path = "TileTypes.dat";
            if (modIO.ZipMakeEntry(output, path, result) != null)
            {
                result.Add(this.Serialize_FMap_TileTypes(file));
                file.Flush();
                output.CloseEntry();
            }
            path = "ScriptLabels.ini";
            if (modIO.ZipMakeEntry(output, path, result) != null)
            {
                clsINIWrite write4 = new clsINIWrite {
                    File = writer2
                };
                result.Add(this.Serialize_WZ_LabelsINI(write4, -1));
                writer2.Flush();
                output.CloseEntry();
            }
            output.Finish();
            output.Close();
            file.Close();
            return result;
        }

        public modProgram.sResult Write_FME(string Path, bool Overwrite, byte ScavengerPlayerNum)
        {
            modProgram.sResult result;
            result.Success = false;
            result.Problem = "";
            if (File.Exists(Path))
            {
                if (!Overwrite)
                {
                    result.Problem = "The selected file already exists.";
                    return result;
                }
                File.Delete(Path);
            }
            BinaryWriter file = null;
            try
            {
                int num;
                clsUnit current;
                int num6;
                int num7;
                IEnumerator enumerator;
                IEnumerator enumerator2;
                file = new BinaryWriter(new FileStream(Path, FileMode.CreateNew));
                file.Write((uint) 6);
                if (this.Tileset == null)
                {
                    file.Write((byte) 0);
                }
                else if (this.Tileset == modProgram.Tileset_Arizona)
                {
                    file.Write((byte) 1);
                }
                else if (this.Tileset == modProgram.Tileset_Urban)
                {
                    file.Write((byte) 2);
                }
                else if (this.Tileset == modProgram.Tileset_Rockies)
                {
                    file.Write((byte) 3);
                }
                file.Write((ushort) this.Terrain.TileSize.X);
                file.Write((ushort) this.Terrain.TileSize.Y);
                int y = this.Terrain.TileSize.Y;
                for (num7 = 0; num7 <= y; num7++)
                {
                    int x = this.Terrain.TileSize.X;
                    num6 = 0;
                    while (num6 <= x)
                    {
                        file.Write(this.Terrain.Vertices[num6, num7].Height);
                        if (this.Terrain.Vertices[num6, num7].Terrain == null)
                        {
                            file.Write((byte) 0);
                        }
                        else
                        {
                            if (this.Terrain.Vertices[num6, num7].Terrain.Num < 0)
                            {
                                result.Problem = "Terrain number out of range.";
                                return result;
                            }
                            file.Write((byte) (this.Terrain.Vertices[num6, num7].Terrain.Num + 1));
                        }
                        num6++;
                    }
                }
                int num10 = this.Terrain.TileSize.Y - 1;
                for (num7 = 0; num7 <= num10; num7++)
                {
                    int num11 = this.Terrain.TileSize.X - 1;
                    num6 = 0;
                    while (num6 <= num11)
                    {
                        byte num2;
                        file.Write((byte) (this.Terrain.Tiles[num6, num7].Texture.TextureNum + 1));
                        byte num5 = 0;
                        if (this.Terrain.Tiles[num6, num7].Terrain_IsCliff)
                        {
                            num5 = (byte) (num5 + 0x80);
                        }
                        if (this.Terrain.Tiles[num6, num7].Texture.Orientation.SwitchedAxes)
                        {
                            num5 = (byte) (num5 + 0x40);
                        }
                        if (this.Terrain.Tiles[num6, num7].Texture.Orientation.ResultXFlip)
                        {
                            num5 = (byte) (num5 + 0x20);
                        }
                        if (this.Terrain.Tiles[num6, num7].Texture.Orientation.ResultYFlip)
                        {
                            num5 = (byte) (num5 + 0x10);
                        }
                        if (this.Terrain.Tiles[num6, num7].Tri)
                        {
                            num5 = (byte) (num5 + 4);
                            if (this.Terrain.Tiles[num6, num7].TriTopLeftIsCliff)
                            {
                                num5 = (byte) (num5 + 2);
                            }
                            if (this.Terrain.Tiles[num6, num7].TriBottomRightIsCliff)
                            {
                                num5 = (byte) (num5 + 1);
                            }
                        }
                        else
                        {
                            if (this.Terrain.Tiles[num6, num7].TriBottomLeftIsCliff)
                            {
                                num5 = (byte) (num5 + 2);
                            }
                            if (this.Terrain.Tiles[num6, num7].TriTopRightIsCliff)
                            {
                                num5 = (byte) (num5 + 1);
                            }
                        }
                        file.Write(num5);
                        if (TileOrientation.IdenticalTileDirections(this.Terrain.Tiles[num6, num7].DownSide, TileOrientation.TileDirection_Top))
                        {
                            num2 = 1;
                        }
                        else if (TileOrientation.IdenticalTileDirections(this.Terrain.Tiles[num6, num7].DownSide, TileOrientation.TileDirection_Left))
                        {
                            num2 = 2;
                        }
                        else if (TileOrientation.IdenticalTileDirections(this.Terrain.Tiles[num6, num7].DownSide, TileOrientation.TileDirection_Right))
                        {
                            num2 = 3;
                        }
                        else if (TileOrientation.IdenticalTileDirections(this.Terrain.Tiles[num6, num7].DownSide, TileOrientation.TileDirection_Bottom))
                        {
                            num2 = 4;
                        }
                        else
                        {
                            num2 = 0;
                        }
                        file.Write(num2);
                        num6++;
                    }
                }
                int num12 = this.Terrain.TileSize.Y;
                for (num7 = 0; num7 <= num12; num7++)
                {
                    int num13 = this.Terrain.TileSize.X - 1;
                    num6 = 0;
                    while (num6 <= num13)
                    {
                        if (this.Terrain.SideH[num6, num7].Road == null)
                        {
                            file.Write((byte) 0);
                        }
                        else
                        {
                            if (this.Terrain.SideH[num6, num7].Road.Num < 0)
                            {
                                result.Problem = "Road number out of range.";
                                return result;
                            }
                            file.Write((byte) (this.Terrain.SideH[num6, num7].Road.Num + 1));
                        }
                        num6++;
                    }
                }
                int num14 = this.Terrain.TileSize.Y - 1;
                for (num7 = 0; num7 <= num14; num7++)
                {
                    int num15 = this.Terrain.TileSize.X;
                    for (num6 = 0; num6 <= num15; num6++)
                    {
                        if (this.Terrain.SideV[num6, num7].Road == null)
                        {
                            file.Write((byte) 0);
                        }
                        else
                        {
                            if (this.Terrain.SideV[num6, num7].Road.Num < 0)
                            {
                                result.Problem = "Road number out of range.";
                                return result;
                            }
                            file.Write((byte) (this.Terrain.SideV[num6, num7].Road.Num + 1));
                        }
                    }
                }
                clsUnit[] unitArray = new clsUnit[(this.Units.Count - 1) + 1];
                string[] strArray = new string[(this.Units.Count - 1) + 1];
                int index = 0;
                try
                {
                    enumerator = this.Units.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        current = (clsUnit) enumerator.Current;
                        if (current.Type.GetCode(ref strArray[index]))
                        {
                            unitArray[index] = current;
                            index++;
                        }
                    }
                }
                finally
                {
                    if (enumerator is IDisposable)
                    {
                        (enumerator as IDisposable).Dispose();
                    }
                }
                file.Write((uint) index);
                int num16 = index - 1;
                for (num = 0; num <= num16; num++)
                {
                    current = unitArray[num];
                    modIO.WriteTextOfLength(file, 40, strArray[num]);
                    switch (current.Type.Type)
                    {
                        case clsUnitType.enumType.Feature:
                            file.Write((byte) 0);
                            break;

                        case clsUnitType.enumType.PlayerStructure:
                            file.Write((byte) 1);
                            break;

                        case clsUnitType.enumType.PlayerDroid:
                            file.Write((byte) 2);
                            break;
                    }
                    file.Write(current.ID);
                    file.Write(current.SavePriority);
                    file.Write((uint) current.Pos.Horizontal.X);
                    file.Write((uint) current.Pos.Horizontal.Y);
                    file.Write((uint) current.Pos.Altitude);
                    file.Write((ushort) current.Rotation);
                    modIO.WriteText(file, true, "");
                    if (current.UnitGroup == this.ScavengerUnitGroup)
                    {
                        file.Write(ScavengerPlayerNum);
                    }
                    else
                    {
                        file.Write((byte) current.UnitGroup.WZ_StartPos);
                    }
                }
                file.Write((uint) this.Gateways.Count);
                try
                {
                    enumerator2 = this.Gateways.GetEnumerator();
                    while (enumerator2.MoveNext())
                    {
                        clsGateway gateway = (clsGateway) enumerator2.Current;
                        file.Write((ushort) gateway.PosA.X);
                        file.Write((ushort) gateway.PosA.Y);
                        file.Write((ushort) gateway.PosB.X);
                        file.Write((ushort) gateway.PosB.Y);
                    }
                }
                finally
                {
                    if (enumerator2 is IDisposable)
                    {
                        (enumerator2 as IDisposable).Dispose();
                    }
                }
                if (this.Tileset != null)
                {
                    int num17 = this.Tileset.TileCount - 1;
                    for (num = 0; num <= num17; num++)
                    {
                        file.Write(this.Tile_TypeNum[num]);
                    }
                }
                file.Write(this.InterfaceOptions.ScrollMin.X);
                file.Write(this.InterfaceOptions.ScrollMin.Y);
                file.Write(this.InterfaceOptions.ScrollMax.X);
                file.Write(this.InterfaceOptions.ScrollMax.Y);
                modIO.WriteText(file, true, this.InterfaceOptions.CompileName);
                file.Write((byte) 0);
                modIO.WriteText(file, true, this.InterfaceOptions.CompileMultiPlayers);
                file.Write(this.InterfaceOptions.CompileMultiXPlayers);
                modIO.WriteText(file, true, this.InterfaceOptions.CompileMultiAuthor);
                modIO.WriteText(file, true, this.InterfaceOptions.CompileMultiLicense);
                modIO.WriteText(file, true, "0");
                int campaignGameType = this.InterfaceOptions.CampaignGameType;
                file.Write(campaignGameType);
            }
            catch (Exception exception1)
            {
                ProjectData.SetProjectError(exception1);
                Exception exception = exception1;
                result.Problem = exception.Message;
                modProgram.sResult result2 = result;
                ProjectData.ClearProjectError();
                return result2;
            }
            if (file != null)
            {
                file.Close();
            }
            result.Success = true;
            return result;
        }

        public modProgram.sResult Write_Heightmap(string Path, bool Overwrite)
        {
            System.Drawing.Bitmap bitmapToSave = new System.Drawing.Bitmap(this.Terrain.TileSize.X + 1, this.Terrain.TileSize.Y + 1);
            int y = this.Terrain.TileSize.Y;
            for (int i = 0; i <= y; i++)
            {
                int x = this.Terrain.TileSize.X;
                for (int j = 0; j <= x; j++)
                {
                    bitmapToSave.SetPixel(j, i, ColorTranslator.FromOle(modColour.OSRGB(this.Terrain.Vertices[j, i].Height, this.Terrain.Vertices[j, i].Height, this.Terrain.Vertices[j, i].Height)));
                }
            }
            return modBitmap.SaveBitmap(Path, Overwrite, bitmapToSave);
        }

        public clsResult Write_LND(string Path, bool Overwrite)
        {
            clsResult result = new clsResult("Writing LND to \"" + Path + "\"");
            if (File.Exists(Path))
            {
                if (!Overwrite)
                {
                    result.ProblemAdd("The selected file already exists.");
                    return result;
                }
                File.Delete(Path);
            }
            StreamWriter writer = null;
            try
            {
                int num;
                int num2;
                string str2;
                IEnumerator enumerator;
                IEnumerator enumerator2;
                char ch2 = '"';
                char ch = '\n';
                writer = new StreamWriter(new FileStream(Path, FileMode.CreateNew), new UTF8Encoding(false, false));
                if (this.Tileset == modProgram.Tileset_Arizona)
                {
                    str2 = "DataSet WarzoneDataC1.eds" + Conversions.ToString(ch);
                }
                else if (this.Tileset == modProgram.Tileset_Urban)
                {
                    str2 = "DataSet WarzoneDataC2.eds" + Conversions.ToString(ch);
                }
                else if (this.Tileset == modProgram.Tileset_Rockies)
                {
                    str2 = "DataSet WarzoneDataC3.eds" + Conversions.ToString(ch);
                }
                else
                {
                    str2 = "DataSet " + Conversions.ToString(ch);
                }
                writer.Write(str2);
                str2 = "GrdLand {" + Conversions.ToString(ch);
                writer.Write(str2);
                str2 = "    Version 4" + Conversions.ToString(ch);
                writer.Write(str2);
                str2 = "    3DPosition 0.000000 3072.000000 0.000000" + Conversions.ToString(ch);
                writer.Write(str2);
                str2 = "    3DRotation 80.000000 0.000000 0.000000" + Conversions.ToString(ch);
                writer.Write(str2);
                str2 = "    2DPosition 0 0" + Conversions.ToString(ch);
                writer.Write(str2);
                str2 = "    CustomSnap 16 16" + Conversions.ToString(ch);
                writer.Write(str2);
                str2 = "    SnapMode 0" + Conversions.ToString(ch);
                writer.Write(str2);
                str2 = "    Gravity 1" + Conversions.ToString(ch);
                writer.Write(str2);
                str2 = "    HeightScale " + modIO.InvariantToString_int(this.HeightMultiplier) + Conversions.ToString(ch);
                writer.Write(str2);
                str2 = "    MapWidth " + modIO.InvariantToString_int(this.Terrain.TileSize.X) + Conversions.ToString(ch);
                writer.Write(str2);
                str2 = "    MapHeight " + modIO.InvariantToString_int(this.Terrain.TileSize.Y) + Conversions.ToString(ch);
                writer.Write(str2);
                str2 = "    TileWidth 128" + Conversions.ToString(ch);
                writer.Write(str2);
                str2 = "    TileHeight 128" + Conversions.ToString(ch);
                writer.Write(str2);
                str2 = "    SeaLevel 0" + Conversions.ToString(ch);
                writer.Write(str2);
                str2 = "    TextureWidth 64" + Conversions.ToString(ch);
                writer.Write(str2);
                str2 = "    TextureHeight 64" + Conversions.ToString(ch);
                writer.Write(str2);
                str2 = "    NumTextures 1" + Conversions.ToString(ch);
                writer.Write(str2);
                str2 = "    Textures {" + Conversions.ToString(ch);
                writer.Write(str2);
                if (this.Tileset == modProgram.Tileset_Arizona)
                {
                    str2 = @"        texpages\tertilesc1.pcx" + Conversions.ToString(ch);
                }
                else if (this.Tileset == modProgram.Tileset_Urban)
                {
                    str2 = @"        texpages\tertilesc2.pcx" + Conversions.ToString(ch);
                }
                else if (this.Tileset == modProgram.Tileset_Rockies)
                {
                    str2 = @"        texpages\tertilesc3.pcx" + Conversions.ToString(ch);
                }
                else
                {
                    str2 = "        " + Conversions.ToString(ch);
                }
                writer.Write(str2);
                str2 = "    }" + Conversions.ToString(ch);
                writer.Write(str2);
                str2 = "    NumTiles " + modIO.InvariantToString_int(this.Terrain.TileSize.X * this.Terrain.TileSize.Y) + Conversions.ToString(ch);
                writer.Write(str2);
                str2 = "    Tiles {" + Conversions.ToString(ch);
                writer.Write(str2);
                int num11 = this.Terrain.TileSize.Y - 1;
                for (int i = 0; i <= num11; i++)
                {
                    int num12 = this.Terrain.TileSize.X - 1;
                    for (int j = 0; j <= num12; j++)
                    {
                        bool flag;
                        byte num6;
                        int num7;
                        int num8;
                        TileOrientation.TileOrientation_To_OldOrientation(this.Terrain.Tiles[j, i].Texture.Orientation, ref num6, ref flag);
                        byte num5 = 0;
                        if (this.Terrain.Tiles[j, i].Tri)
                        {
                            num5 = (byte) (num5 + 2);
                        }
                        if (flag)
                        {
                            num5 = (byte) (num5 + 4);
                        }
                        num5 = (byte) (num5 + ((byte) (num6 * 0x10)));
                        if (this.Terrain.Tiles[j, i].Tri)
                        {
                            num8 = 1;
                        }
                        else
                        {
                            num8 = 0;
                        }
                        if (flag)
                        {
                            num7 = 1;
                        }
                        else
                        {
                            num7 = 0;
                        }
                        str2 = "        TID " + Conversions.ToString((int) (this.Terrain.Tiles[j, i].Texture.TextureNum + 1)) + " VF " + modIO.InvariantToString_int(num8) + " TF " + modIO.InvariantToString_int(num7) + " F " + modIO.InvariantToString_int(num5) + " VH " + modIO.InvariantToString_byte(this.Terrain.Vertices[j, i].Height) + " " + modIO.InvariantToString_byte(this.Terrain.Vertices[j + 1, i].Height) + " " + Conversions.ToString(this.Terrain.Vertices[j + 1, i + 1].Height) + " " + modIO.InvariantToString_byte(this.Terrain.Vertices[j, i + 1].Height) + Conversions.ToString(ch);
                        writer.Write(str2);
                    }
                }
                str2 = "    }" + Conversions.ToString(ch);
                writer.Write(str2);
                str2 = "}" + Conversions.ToString(ch);
                writer.Write(str2);
                str2 = "ObjectList {" + Conversions.ToString(ch);
                writer.Write(str2);
                str2 = "    Version 3" + Conversions.ToString(ch);
                writer.Write(str2);
                if (this.Tileset == modProgram.Tileset_Arizona)
                {
                    str2 = "\tFeatureSet WarzoneDataC1.eds" + Conversions.ToString(ch);
                }
                else if (this.Tileset == modProgram.Tileset_Urban)
                {
                    str2 = "\tFeatureSet WarzoneDataC2.eds" + Conversions.ToString(ch);
                }
                else if (this.Tileset == modProgram.Tileset_Rockies)
                {
                    str2 = "\tFeatureSet WarzoneDataC3.eds" + Conversions.ToString(ch);
                }
                else
                {
                    str2 = "\tFeatureSet " + Conversions.ToString(ch);
                }
                writer.Write(str2);
                str2 = "    NumObjects " + modIO.InvariantToString_int(this.Units.Count) + Conversions.ToString(ch);
                writer.Write(str2);
                str2 = "    Objects {" + Conversions.ToString(ch);
                writer.Write(str2);
                string str = null;
                int num4 = 0;
                try
                {
                    enumerator = this.Units.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        modMath.sXYZ_int _int;
                        clsUnit current = (clsUnit) enumerator.Current;
                        switch (current.Type.Type)
                        {
                            case clsUnitType.enumType.Feature:
                                num2 = 0;
                                goto Label_07CE;

                            case clsUnitType.enumType.PlayerStructure:
                                num2 = 1;
                                goto Label_07CE;

                            case clsUnitType.enumType.PlayerDroid:
                                if (!((clsDroidDesign) current.Type).IsTemplate)
                                {
                                    break;
                                }
                                num2 = 2;
                                goto Label_07CE;

                            default:
                                num2 = -1;
                                result.WarningAdd("Unit type classification not accounted for.");
                                goto Label_07CE;
                        }
                        num2 = -1;
                    Label_07CE:
                        _int = this.LNDPos_From_MapPos(this.Units[num].Pos.Horizontal);
                        if (num2 >= 0)
                        {
                            if (current.Type.GetCode(ref str))
                            {
                                str2 = "        " + modIO.InvariantToString_uint(current.ID) + " " + Conversions.ToString(num2) + " " + Conversions.ToString(ch2) + str + Conversions.ToString(ch2) + " " + current.UnitGroup.GetLNDPlayerText() + " " + Conversions.ToString(ch2) + "NONAME" + Conversions.ToString(ch2) + " " + modIO.InvariantToString_int(_int.X) + ".00 " + modIO.InvariantToString_int(_int.Y) + ".00 " + modIO.InvariantToString_int(_int.Z) + ".00 0.00 " + modIO.InvariantToString_int(current.Rotation) + ".00 0.00" + Conversions.ToString(ch);
                                writer.Write(str2);
                            }
                            else
                            {
                                result.WarningAdd("Error. Code not found.");
                            }
                        }
                        else
                        {
                            num4++;
                        }
                    }
                }
                finally
                {
                    if (enumerator is IDisposable)
                    {
                        (enumerator as IDisposable).Dispose();
                    }
                }
                str2 = "    }" + Conversions.ToString(ch);
                writer.Write(str2);
                str2 = "}" + Conversions.ToString(ch);
                writer.Write(str2);
                str2 = "ScrollLimits {" + Conversions.ToString(ch);
                writer.Write(str2);
                str2 = "    Version 1" + Conversions.ToString(ch);
                writer.Write(str2);
                str2 = "    NumLimits 1" + Conversions.ToString(ch);
                writer.Write(str2);
                str2 = "    Limits {" + Conversions.ToString(ch);
                writer.Write(str2);
                str2 = "        " + Conversions.ToString(ch2) + "Entire Map" + Conversions.ToString(ch2) + " 0 0 0 " + modIO.InvariantToString_int(this.Terrain.TileSize.X) + " " + modIO.InvariantToString_int(this.Terrain.TileSize.Y) + Conversions.ToString(ch);
                writer.Write(str2);
                str2 = "    }" + Conversions.ToString(ch);
                writer.Write(str2);
                str2 = "}" + Conversions.ToString(ch);
                writer.Write(str2);
                str2 = "Gateways {" + Conversions.ToString(ch);
                writer.Write(str2);
                str2 = "    Version 1" + Conversions.ToString(ch);
                writer.Write(str2);
                str2 = "    NumGateways " + modIO.InvariantToString_int(this.Gateways.Count) + Conversions.ToString(ch);
                writer.Write(str2);
                str2 = "    Gates {" + Conversions.ToString(ch);
                writer.Write(str2);
                try
                {
                    enumerator2 = this.Gateways.GetEnumerator();
                    while (enumerator2.MoveNext())
                    {
                        clsGateway gateway = (clsGateway) enumerator2.Current;
                        str2 = "        " + modIO.InvariantToString_int(gateway.PosA.X) + " " + modIO.InvariantToString_int(gateway.PosA.Y) + " " + modIO.InvariantToString_int(gateway.PosB.X) + " " + modIO.InvariantToString_int(gateway.PosB.Y) + Conversions.ToString(ch);
                        writer.Write(str2);
                    }
                }
                finally
                {
                    if (enumerator2 is IDisposable)
                    {
                        (enumerator2 as IDisposable).Dispose();
                    }
                }
                str2 = "    }" + Conversions.ToString(ch);
                writer.Write(str2);
                str2 = "}" + Conversions.ToString(ch);
                writer.Write(str2);
                str2 = "TileTypes {" + Conversions.ToString(ch);
                writer.Write(str2);
                str2 = "    NumTiles " + Conversions.ToString(this.Tileset.TileCount) + Conversions.ToString(ch);
                writer.Write(str2);
                str2 = "    Tiles {" + Conversions.ToString(ch);
                writer.Write(str2);
                int num13 = ((int) Math.Round(Math.Ceiling((double) (((double) (this.Tileset.TileCount + 1)) / 16.0)))) - 1;
                for (num = 0; num <= num13; num++)
                {
                    str2 = "        ";
                    int num3 = (num * 0x10) - 1;
                    int num14 = Math.Min(0x10, this.Tileset.TileCount - num3) - 1;
                    for (num2 = 0; num2 <= num14; num2++)
                    {
                        if ((num3 + num2) < 0)
                        {
                            str2 = str2 + "2 ";
                        }
                        else
                        {
                            str2 = str2 + modIO.InvariantToString_byte(this.Tile_TypeNum[num3 + num2]) + " ";
                        }
                    }
                    str2 = str2 + Conversions.ToString(ch);
                    writer.Write(str2);
                }
                str2 = "    }" + Conversions.ToString(ch);
                writer.Write(str2);
                str2 = "}" + Conversions.ToString(ch);
                writer.Write(str2);
                str2 = "TileFlags {" + Conversions.ToString(ch);
                writer.Write(str2);
                str2 = "    NumTiles 90" + Conversions.ToString(ch);
                writer.Write(str2);
                str2 = "    Flags {" + Conversions.ToString(ch);
                writer.Write(str2);
                str2 = "        0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 " + Conversions.ToString(ch);
                writer.Write(str2);
                str2 = "        0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 " + Conversions.ToString(ch);
                writer.Write(str2);
                str2 = "        0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 " + Conversions.ToString(ch);
                writer.Write(str2);
                str2 = "        0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 " + Conversions.ToString(ch);
                writer.Write(str2);
                str2 = "        0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 " + Conversions.ToString(ch);
                writer.Write(str2);
                str2 = "        0 0 0 0 0 0 0 0 0 0 " + Conversions.ToString(ch);
                writer.Write(str2);
                str2 = "    }" + Conversions.ToString(ch);
                writer.Write(str2);
                str2 = "}" + Conversions.ToString(ch);
                writer.Write(str2);
                str2 = "Brushes {" + Conversions.ToString(ch);
                writer.Write(str2);
                str2 = "    Version 2" + Conversions.ToString(ch);
                writer.Write(str2);
                str2 = "    NumEdgeBrushes 0" + Conversions.ToString(ch);
                writer.Write(str2);
                str2 = "    NumUserBrushes 0" + Conversions.ToString(ch);
                writer.Write(str2);
                str2 = "    EdgeBrushes {" + Conversions.ToString(ch);
                writer.Write(str2);
                str2 = "    }" + Conversions.ToString(ch);
                writer.Write(str2);
                str2 = "}" + Conversions.ToString(ch);
                writer.Write(str2);
            }
            catch (Exception exception1)
            {
                ProjectData.SetProjectError(exception1);
                Exception exception = exception1;
                result.ProblemAdd(exception.Message);
                ProjectData.ClearProjectError();
            }
            if (writer != null)
            {
                writer.Close();
            }
            return result;
        }

        public modProgram.sResult Write_MinimapFile(string Path, bool Overwrite)
        {
            System.Drawing.Bitmap bitmapToSave = new System.Drawing.Bitmap(this.Terrain.TileSize.X, this.Terrain.TileSize.Y);
            modMath.sXY_int size = new modMath.sXY_int(this.Terrain.TileSize.X, this.Terrain.TileSize.Y);
            clsMinimapTexture texture = new clsMinimapTexture(size);
            this.MinimapTextureFill(texture);
            int num3 = this.Terrain.TileSize.Y - 1;
            for (int i = 0; i <= num3; i++)
            {
                int num4 = this.Terrain.TileSize.X - 1;
                for (int j = 0; j <= num4; j++)
                {
                    bitmapToSave.SetPixel(j, i, ColorTranslator.FromOle(modColour.OSRGB((int) Math.Round((double) modMath.Clamp_sng(texture.get_Pixels(j, i).Red * 255f, 0f, 255f)), (int) Math.Round((double) modMath.Clamp_sng(texture.get_Pixels(j, i).Green * 255f, 0f, 255f)), (int) Math.Round((double) modMath.Clamp_sng(texture.get_Pixels(j, i).Blue * 255f, 0f, 255f)))));
                }
            }
            return modBitmap.SaveBitmap(Path, Overwrite, bitmapToSave);
        }

        public modProgram.sResult Write_TTP(string Path, bool Overwrite)
        {
            BinaryWriter writer;
            modProgram.sResult result;
            result.Success = false;
            result.Problem = "";
            if (File.Exists(Path))
            {
                if (!Overwrite)
                {
                    result.Problem = "File already exists.";
                    return result;
                }
                File.Delete(Path);
            }
            try
            {
                writer = new BinaryWriter(new FileStream(Path, FileMode.CreateNew), modProgram.ASCIIEncoding);
            }
            catch (Exception exception1)
            {
                ProjectData.SetProjectError(exception1);
                Exception exception = exception1;
                result.Problem = exception.Message;
                modProgram.sResult result2 = result;
                ProjectData.ClearProjectError();
                return result2;
            }
            modIO.WriteText(writer, false, "ttyp");
            writer.Write((uint) 8);
            if (this.Tileset == null)
            {
                writer.Write((uint) 0);
            }
            else
            {
                writer.Write((uint) this.Tileset.TileCount);
                int num2 = this.Tileset.TileCount - 1;
                for (int i = 0; i <= num2; i++)
                {
                    writer.Write((ushort) this.Tile_TypeNum[i]);
                }
            }
            writer.Close();
            result.Success = true;
            return result;
        }

        public clsResult Write_WZ(sWrite_WZ_Args Args)
        {
            clsResult result2;
            clsResult result = new clsResult("Compiling to \"" + Args.Path + "\"");
            try
            {
                int num;
                int num3;
                clsStructureType type2;
                clsUnit unit;
                IEnumerator enumerator;
                IEnumerator enumerator2;
                IEnumerator enumerator3;
                IEnumerator enumerator4;
                IEnumerator enumerator5;
                switch (Args.CompileType)
                {
                    case sWrite_WZ_Args.enumCompileType.Multiplayer:
                        if (Args.Multiplayer != null)
                        {
                            break;
                        }
                        result.ProblemAdd("Multiplayer arguments were not passed.");
                        return result;

                    case sWrite_WZ_Args.enumCompileType.Campaign:
                        if (Args.Campaign != null)
                        {
                            goto Label_0106;
                        }
                        result.ProblemAdd("Campaign arguments were not passed.");
                        return result;

                    default:
                        result.ProblemAdd("Unknown compile method.");
                        return result;
                }
                if ((Args.Multiplayer.PlayerCount < 2) | (Args.Multiplayer.PlayerCount > 10))
                {
                    result.ProblemAdd("Number of players was below 2 or above 10.");
                    return result;
                }
                if (!Args.Multiplayer.IsBetaPlayerFormat && !(((Args.Multiplayer.PlayerCount == 2) | (Args.Multiplayer.PlayerCount == 4)) | (Args.Multiplayer.PlayerCount == 8)))
                {
                    result.ProblemAdd("Number of players was not 2, 4 or 8 in original format.");
                    return result;
                }
            Label_0106:
                if (!Args.Overwrite && File.Exists(Args.Path))
                {
                    result.ProblemAdd("The selected file already exists.");
                    return result;
                }
                char ch2 = '"';
                char ch = '\n';
                MemoryStream stream4 = new MemoryStream();
                StreamWriter writer4 = new StreamWriter(stream4, modProgram.UTF8Encoding);
                MemoryStream output = new MemoryStream();
                BinaryWriter file = new BinaryWriter(output, modProgram.ASCIIEncoding);
                MemoryStream stream3 = new MemoryStream();
                BinaryWriter writer3 = new BinaryWriter(stream3, modProgram.ASCIIEncoding);
                MemoryStream stream2 = new MemoryStream();
                BinaryWriter writer2 = new BinaryWriter(stream2, modProgram.ASCIIEncoding);
                MemoryStream stream9 = new MemoryStream();
                clsINIWrite write2 = clsINIWrite.CreateFile(stream9);
                MemoryStream stream7 = new MemoryStream();
                BinaryWriter writer7 = new BinaryWriter(stream7, modProgram.ASCIIEncoding);
                MemoryStream stream6 = new MemoryStream();
                BinaryWriter writer6 = new BinaryWriter(stream6, modProgram.ASCIIEncoding);
                MemoryStream stream11 = new MemoryStream();
                clsINIWrite write4 = clsINIWrite.CreateFile(stream11);
                MemoryStream stream = new MemoryStream();
                BinaryWriter writer = new BinaryWriter(stream, modProgram.ASCIIEncoding);
                MemoryStream stream8 = new MemoryStream();
                clsINIWrite write = clsINIWrite.CreateFile(stream8);
                MemoryStream stream10 = new MemoryStream();
                clsINIWrite write3 = clsINIWrite.CreateFile(stream10);
                string str = "";
                string str2 = "";
                if (Args.CompileType == sWrite_WZ_Args.enumCompileType.Multiplayer)
                {
                    string str4;
                    string str5;
                    str2 = modIO.InvariantToString_int(Args.Multiplayer.PlayerCount);
                    str = str2 + "c-";
                    if (this.Tileset == null)
                    {
                        result.ProblemAdd("Map must have a tileset.");
                        return result;
                    }
                    if (this.Tileset == modProgram.Tileset_Arizona)
                    {
                        str4 = "fog1.wrf";
                        str5 = "1";
                    }
                    else if (this.Tileset == modProgram.Tileset_Urban)
                    {
                        str4 = "fog2.wrf";
                        str5 = "2";
                    }
                    else if (this.Tileset == modProgram.Tileset_Rockies)
                    {
                        str4 = "fog3.wrf";
                        str5 = "3";
                    }
                    else
                    {
                        result.ProblemAdd("Unknown tileset selected.");
                        return result;
                    }
                    string str3 = "// Made with FlaME 1.29 Windows" + Conversions.ToString(ch);
                    writer4.Write(str3);
                    DateTime now = DateAndTime.Now;
                    str3 = "// Date: " + Conversions.ToString(now.Year) + "/" + modProgram.MinDigits(now.Month, 2) + "/" + modProgram.MinDigits(now.Day, 2) + " " + modProgram.MinDigits(now.Hour, 2) + ":" + modProgram.MinDigits(now.Minute, 2) + ":" + modProgram.MinDigits(now.Second, 2) + Conversions.ToString(ch);
                    writer4.Write(str3);
                    str3 = "// Author: " + Args.Multiplayer.AuthorName + Conversions.ToString(ch);
                    writer4.Write(str3);
                    str3 = "// License: " + Args.Multiplayer.License + Conversions.ToString(ch);
                    writer4.Write(str3);
                    str3 = Conversions.ToString(ch);
                    writer4.Write(str3);
                    str3 = "level   " + Args.MapName + "-T1" + Conversions.ToString(ch);
                    writer4.Write(str3);
                    str3 = "players " + str2 + Conversions.ToString(ch);
                    writer4.Write(str3);
                    str3 = "type    14" + Conversions.ToString(ch);
                    writer4.Write(str3);
                    str3 = "dataset MULTI_CAM_" + str5 + Conversions.ToString(ch);
                    writer4.Write(str3);
                    str3 = "game    " + Conversions.ToString(ch2) + "multiplay/maps/" + str + Args.MapName + ".gam" + Conversions.ToString(ch2) + Conversions.ToString(ch);
                    writer4.Write(str3);
                    str3 = "data    " + Conversions.ToString(ch2) + "wrf/multi/skirmish" + str2 + ".wrf" + Conversions.ToString(ch2) + Conversions.ToString(ch);
                    writer4.Write(str3);
                    str3 = "data    " + Conversions.ToString(ch2) + "wrf/multi/" + str4 + Conversions.ToString(ch2) + Conversions.ToString(ch);
                    writer4.Write(str3);
                    str3 = Conversions.ToString(ch);
                    writer4.Write(str3);
                    str3 = "level   " + Args.MapName + "-T2" + Conversions.ToString(ch);
                    writer4.Write(str3);
                    str3 = "players " + str2 + Conversions.ToString(ch);
                    writer4.Write(str3);
                    str3 = "type    18" + Conversions.ToString(ch);
                    writer4.Write(str3);
                    str3 = "dataset MULTI_T2_C" + str5 + Conversions.ToString(ch);
                    writer4.Write(str3);
                    str3 = "game    " + Conversions.ToString(ch2) + "multiplay/maps/" + str + Args.MapName + ".gam" + Conversions.ToString(ch2) + Conversions.ToString(ch);
                    writer4.Write(str3);
                    str3 = "data    " + Conversions.ToString(ch2) + "wrf/multi/t2-skirmish" + str2 + ".wrf" + Conversions.ToString(ch2) + Conversions.ToString(ch);
                    writer4.Write(str3);
                    str3 = "data    " + Conversions.ToString(ch2) + "wrf/multi/" + str4 + Conversions.ToString(ch2) + Conversions.ToString(ch);
                    writer4.Write(str3);
                    str3 = Conversions.ToString(ch);
                    writer4.Write(str3);
                    str3 = "level   " + Args.MapName + "-T3" + Conversions.ToString(ch);
                    writer4.Write(str3);
                    str3 = "players " + str2 + Conversions.ToString(ch);
                    writer4.Write(str3);
                    str3 = "type    19" + Conversions.ToString(ch);
                    writer4.Write(str3);
                    str3 = "dataset MULTI_T3_C" + str5 + Conversions.ToString(ch);
                    writer4.Write(str3);
                    str3 = "game    " + Conversions.ToString(ch2) + "multiplay/maps/" + str + Args.MapName + ".gam" + Conversions.ToString(ch2) + Conversions.ToString(ch);
                    writer4.Write(str3);
                    str3 = "data    " + Conversions.ToString(ch2) + "wrf/multi/t3-skirmish" + str2 + ".wrf" + Conversions.ToString(ch2) + Conversions.ToString(ch);
                    writer4.Write(str3);
                    str3 = "data    " + Conversions.ToString(ch2) + "wrf/multi/" + str4 + Conversions.ToString(ch2) + Conversions.ToString(ch);
                    writer4.Write(str3);
                }
                byte[] buffer3 = new byte[20];
                modIO.WriteText(writer3, false, "game");
                writer3.Write((uint) 8);
                writer3.Write((uint) 0);
                if (Args.CompileType == sWrite_WZ_Args.enumCompileType.Multiplayer)
                {
                    writer3.Write((uint) 0);
                }
                else if (Args.CompileType == sWrite_WZ_Args.enumCompileType.Campaign)
                {
                    writer3.Write(Args.Campaign.GAMType);
                }
                writer3.Write(Args.ScrollMin.X);
                writer3.Write(Args.ScrollMin.Y);
                writer3.Write(Args.ScrollMax.X);
                writer3.Write(Args.ScrollMax.Y);
                writer3.Write(buffer3);
                modIO.WriteText(file, false, "map ");
                file.Write((uint) 10);
                file.Write((uint) this.Terrain.TileSize.X);
                file.Write((uint) this.Terrain.TileSize.Y);
                int num8 = this.Terrain.TileSize.Y - 1;
                for (int i = 0; i <= num8; i++)
                {
                    int num9 = this.Terrain.TileSize.X - 1;
                    for (int j = 0; j <= num9; j++)
                    {
                        bool flag;
                        byte num4;
                        TileOrientation.TileOrientation_To_OldOrientation(this.Terrain.Tiles[j, i].Texture.Orientation, ref num4, ref flag);
                        byte num2 = 0;
                        if (this.Terrain.Tiles[j, i].Tri)
                        {
                            num2 = (byte) (num2 + 8);
                        }
                        num2 = (byte) (num2 + ((byte) (num4 * 0x10)));
                        if (flag)
                        {
                            num2 = (byte) (num2 + 0x80);
                        }
                        int textureNum = this.Terrain.Tiles[j, i].Texture.TextureNum;
                        if ((textureNum < 0) | (textureNum > 0xff))
                        {
                            textureNum = 0;
                            if (num3 < 0x10)
                            {
                                result.WarningAdd("Tile texture number " + Conversions.ToString(this.Terrain.Tiles[j, i].Texture.TextureNum) + " is invalid on tile " + Conversions.ToString(j) + ", " + Conversions.ToString(i) + " and was compiled as texture number " + Conversions.ToString(textureNum) + ".");
                            }
                            num3++;
                        }
                        file.Write((byte) textureNum);
                        file.Write(num2);
                        file.Write(this.Terrain.Vertices[j, i].Height);
                    }
                }
                if (num3 > 0)
                {
                    result.WarningAdd(Conversions.ToString(num3) + " tile texture numbers were invalid.");
                }
                file.Write((uint) 1);
                file.Write((uint) this.Gateways.Count);
                try
                {
                    enumerator = this.Gateways.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        clsGateway current = (clsGateway) enumerator.Current;
                        file.Write((byte) modMath.Clamp_int(current.PosA.X, 0, 0xff));
                        file.Write((byte) modMath.Clamp_int(current.PosA.Y, 0, 0xff));
                        file.Write((byte) modMath.Clamp_int(current.PosB.X, 0, 0xff));
                        file.Write((byte) modMath.Clamp_int(current.PosB.Y, 0, 0xff));
                    }
                }
                finally
                {
                    if (enumerator is IDisposable)
                    {
                        (enumerator as IDisposable).Dispose();
                    }
                }
                clsStructureWriteWZ tool = new clsStructureWriteWZ {
                    File = writer6,
                    CompileType = Args.CompileType
                };
                if (Args.CompileType == sWrite_WZ_Args.enumCompileType.Multiplayer)
                {
                    tool.PlayerCount = Args.Multiplayer.PlayerCount;
                }
                else
                {
                    tool.PlayerCount = 0;
                }
                byte[] buffer2 = new byte[12];
                modIO.WriteText(writer2, false, "feat");
                writer2.Write((uint) 8);
                clsObjectPriorityOrderList list2 = new clsObjectPriorityOrderList();
                try
                {
                    enumerator2 = this.Units.GetEnumerator();
                    while (enumerator2.MoveNext())
                    {
                        unit = (clsUnit) enumerator2.Current;
                        if (unit.Type.Type == clsUnitType.enumType.Feature)
                        {
                            list2.SetItem(unit);
                            list2.ActionPerform();
                        }
                    }
                }
                finally
                {
                    if (enumerator2 is IDisposable)
                    {
                        (enumerator2 as IDisposable).Dispose();
                    }
                }
                writer2.Write((uint) list2.Result.Count);
                int num10 = list2.Result.Count - 1;
                for (num = 0; num <= num10; num++)
                {
                    unit = list2.Result[num];
                    clsFeatureType type = (clsFeatureType) unit.Type;
                    modIO.WriteTextOfLength(writer2, 40, type.Code);
                    writer2.Write(unit.ID);
                    writer2.Write((uint) unit.Pos.Horizontal.X);
                    writer2.Write((uint) unit.Pos.Horizontal.Y);
                    writer2.Write((uint) unit.Pos.Altitude);
                    writer2.Write((uint) unit.Rotation);
                    switch (Args.CompileType)
                    {
                        case sWrite_WZ_Args.enumCompileType.Multiplayer:
                            writer2.Write(unit.GetBJOMultiplayerPlayerNum(Args.Multiplayer.PlayerCount));
                            break;

                        case sWrite_WZ_Args.enumCompileType.Campaign:
                            writer2.Write(unit.GetBJOCampaignPlayerNum());
                            break;

                        default:
                            Debugger.Break();
                            break;
                    }
                    writer2.Write(buffer2);
                }
                modIO.WriteText(writer7, false, "ttyp");
                writer7.Write((uint) 8);
                writer7.Write((uint) this.Tileset.TileCount);
                int num11 = this.Tileset.TileCount - 1;
                for (num = 0; num <= num11; num++)
                {
                    writer7.Write((ushort) this.Tile_TypeNum[num]);
                }
                modIO.WriteText(writer6, false, "stru");
                writer6.Write((uint) 8);
                clsObjectPriorityOrderList list4 = new clsObjectPriorityOrderList();
                try
                {
                    enumerator3 = this.Units.GetEnumerator();
                    while (enumerator3.MoveNext())
                    {
                        unit = (clsUnit) enumerator3.Current;
                        if (unit.Type.Type == clsUnitType.enumType.PlayerStructure)
                        {
                            type2 = (clsStructureType) unit.Type;
                            if (!type2.IsModule())
                            {
                                list4.SetItem(unit);
                                list4.ActionPerform();
                            }
                        }
                    }
                }
                finally
                {
                    if (enumerator3 is IDisposable)
                    {
                        (enumerator3 as IDisposable).Dispose();
                    }
                }
                clsObjectPriorityOrderList list3 = new clsObjectPriorityOrderList();
                try
                {
                    enumerator4 = this.Units.GetEnumerator();
                    while (enumerator4.MoveNext())
                    {
                        unit = (clsUnit) enumerator4.Current;
                        if (unit.Type.Type == clsUnitType.enumType.PlayerStructure)
                        {
                            type2 = (clsStructureType) unit.Type;
                            if (type2.IsModule())
                            {
                                list3.SetItem(unit);
                                list3.ActionPerform();
                            }
                        }
                    }
                }
                finally
                {
                    if (enumerator4 is IDisposable)
                    {
                        (enumerator4 as IDisposable).Dispose();
                    }
                }
                writer6.Write((uint) (list4.Result.Count + list3.Result.Count));
                list4.Result.PerformTool(tool);
                list3.Result.PerformTool(tool);
                byte[] buffer = new byte[12];
                modIO.WriteText(writer, false, "dint");
                writer.Write((uint) 8);
                clsObjectPriorityOrderList list = new clsObjectPriorityOrderList();
                try
                {
                    enumerator5 = this.Units.GetEnumerator();
                    while (enumerator5.MoveNext())
                    {
                        unit = (clsUnit) enumerator5.Current;
                        if (unit.Type.Type == clsUnitType.enumType.PlayerDroid)
                        {
                            clsDroidDesign design = (clsDroidDesign) unit.Type;
                            if (design.IsTemplate)
                            {
                                list.SetItem(unit);
                                list.ActionPerform();
                            }
                        }
                    }
                }
                finally
                {
                    if (enumerator5 is IDisposable)
                    {
                        (enumerator5 as IDisposable).Dispose();
                    }
                }
                writer.Write((uint) list.Result.Count);
                int num12 = list.Result.Count - 1;
                for (num = 0; num <= num12; num++)
                {
                    unit = list.Result[num];
                    clsDroidTemplate template = (clsDroidTemplate) unit.Type;
                    modIO.WriteTextOfLength(writer, 40, template.Code);
                    writer.Write(unit.ID);
                    writer.Write((uint) unit.Pos.Horizontal.X);
                    writer.Write((uint) unit.Pos.Horizontal.Y);
                    writer.Write((uint) unit.Pos.Altitude);
                    writer.Write((uint) unit.Rotation);
                    switch (Args.CompileType)
                    {
                        case sWrite_WZ_Args.enumCompileType.Multiplayer:
                            writer.Write(unit.GetBJOMultiplayerPlayerNum(Args.Multiplayer.PlayerCount));
                            break;

                        case sWrite_WZ_Args.enumCompileType.Campaign:
                            writer.Write(unit.GetBJOCampaignPlayerNum());
                            break;

                        default:
                            Debugger.Break();
                            break;
                    }
                    writer.Write(buffer);
                }
                result.Add(this.Serialize_WZ_FeaturesINI(write2));
                if (Args.CompileType == sWrite_WZ_Args.enumCompileType.Multiplayer)
                {
                    result.Add(this.Serialize_WZ_StructuresINI(write4, Args.Multiplayer.PlayerCount));
                    result.Add(this.Serialize_WZ_DroidsINI(write, Args.Multiplayer.PlayerCount));
                    result.Add(this.Serialize_WZ_LabelsINI(write3, Args.Multiplayer.PlayerCount));
                }
                else if (Args.CompileType == sWrite_WZ_Args.enumCompileType.Campaign)
                {
                    result.Add(this.Serialize_WZ_StructuresINI(write4, -1));
                    result.Add(this.Serialize_WZ_DroidsINI(write, -1));
                    result.Add(this.Serialize_WZ_LabelsINI(write3, 0));
                }
                writer4.Flush();
                file.Flush();
                writer3.Flush();
                writer2.Flush();
                write2.File.Flush();
                writer7.Flush();
                writer6.Flush();
                write4.File.Flush();
                writer.Flush();
                write.File.Flush();
                write3.File.Flush();
                if (Args.CompileType == sWrite_WZ_Args.enumCompileType.Multiplayer)
                {
                    ZipOutputStream stream12;
                    if (!Args.Overwrite)
                    {
                        if (File.Exists(Args.Path))
                        {
                            result.ProblemAdd("A file already exists at: " + Args.Path);
                            return result;
                        }
                    }
                    else if (File.Exists(Args.Path))
                    {
                        try
                        {
                            File.Delete(Args.Path);
                        }
                        catch (Exception exception1)
                        {
                            ProjectData.SetProjectError(exception1);
                            Exception exception = exception1;
                            result.ProblemAdd("Unable to delete existing file: " + exception.Message);
                            result2 = result;
                            ProjectData.ClearProjectError();
                            return result2;
                        }
                    }
                    try
                    {
                        stream12 = new ZipOutputStream(File.Create(Args.Path));
                    }
                    catch (Exception exception6)
                    {
                        ProjectData.SetProjectError(exception6);
                        Exception exception2 = exception6;
                        result.ProblemAdd(exception2.Message);
                        result2 = result;
                        ProjectData.ClearProjectError();
                        return result2;
                    }
                    stream12.SetLevel(9);
                    stream12.UseZip64 = UseZip64.Off;
                    try
                    {
                        string str6;
                        if (Args.Multiplayer.IsBetaPlayerFormat)
                        {
                            str6 = str + Args.MapName + ".xplayers.lev";
                        }
                        else
                        {
                            str6 = str + Args.MapName + ".addon.lev";
                        }
                        if (modIO.ZipMakeEntry(stream12, str6, result) != null)
                        {
                            stream4.WriteTo(stream12);
                            stream12.Flush();
                            stream12.CloseEntry();
                        }
                        ZipEntry entry = new ZipEntry("multiplay/");
                        stream12.PutNextEntry(entry);
                        entry = new ZipEntry("multiplay/maps/");
                        stream12.PutNextEntry(entry);
                        entry = new ZipEntry("multiplay/maps/" + str + Args.MapName + "/");
                        stream12.PutNextEntry(entry);
                        str6 = "multiplay/maps/" + str + Args.MapName + ".gam";
                        if (modIO.ZipMakeEntry(stream12, str6, result) != null)
                        {
                            result.Add(modIO.WriteMemoryToZipEntryAndFlush(stream3, stream12));
                        }
                        else
                        {
                            result.ProblemAdd("Unable to make entry " + str6);
                        }
                        str6 = "multiplay/maps/" + str + Args.MapName + "/dinit.bjo";
                        if (modIO.ZipMakeEntry(stream12, str6, result) != null)
                        {
                            result.Add(modIO.WriteMemoryToZipEntryAndFlush(stream, stream12));
                        }
                        else
                        {
                            result.ProblemAdd("Unable to make entry " + str6);
                        }
                        str6 = "multiplay/maps/" + str + Args.MapName + "/droid.ini";
                        if (modIO.ZipMakeEntry(stream12, str6, result) != null)
                        {
                            result.Add(modIO.WriteMemoryToZipEntryAndFlush(stream8, stream12));
                        }
                        else
                        {
                            result.ProblemAdd("Unable to make entry " + str6);
                        }
                        str6 = "multiplay/maps/" + str + Args.MapName + "/feat.bjo";
                        if (modIO.ZipMakeEntry(stream12, str6, result) != null)
                        {
                            result.Add(modIO.WriteMemoryToZipEntryAndFlush(stream2, stream12));
                        }
                        else
                        {
                            result.ProblemAdd("Unable to make entry " + str6);
                        }
                        str6 = "multiplay/maps/" + str + Args.MapName + "/feature.ini";
                        if (modIO.ZipMakeEntry(stream12, str6, result) != null)
                        {
                            result.Add(modIO.WriteMemoryToZipEntryAndFlush(stream9, stream12));
                        }
                        else
                        {
                            result.ProblemAdd("Unable to make entry " + str6);
                        }
                        str6 = "multiplay/maps/" + str + Args.MapName + "/game.map";
                        if (modIO.ZipMakeEntry(stream12, str6, result) != null)
                        {
                            result.Add(modIO.WriteMemoryToZipEntryAndFlush(output, stream12));
                        }
                        else
                        {
                            result.ProblemAdd("Unable to make entry " + str6);
                        }
                        str6 = "multiplay/maps/" + str + Args.MapName + "/struct.bjo";
                        if (modIO.ZipMakeEntry(stream12, str6, result) != null)
                        {
                            result.Add(modIO.WriteMemoryToZipEntryAndFlush(stream6, stream12));
                        }
                        else
                        {
                            result.ProblemAdd("Unable to make entry " + str6);
                        }
                        str6 = "multiplay/maps/" + str + Args.MapName + "/struct.ini";
                        if (modIO.ZipMakeEntry(stream12, str6, result) != null)
                        {
                            result.Add(modIO.WriteMemoryToZipEntryAndFlush(stream11, stream12));
                        }
                        else
                        {
                            result.ProblemAdd("Unable to make entry " + str6);
                        }
                        str6 = "multiplay/maps/" + str + Args.MapName + "/ttypes.ttp";
                        if (modIO.ZipMakeEntry(stream12, str6, result) != null)
                        {
                            result.Add(modIO.WriteMemoryToZipEntryAndFlush(stream7, stream12));
                        }
                        else
                        {
                            result.ProblemAdd("Unable to make entry " + str6);
                        }
                        if (stream10.Length > 0L)
                        {
                            str6 = "multiplay/maps/" + str + Args.MapName + "/labels.ini";
                            if (modIO.ZipMakeEntry(stream12, str6, result) != null)
                            {
                                result.Add(modIO.WriteMemoryToZipEntryAndFlush(stream10, stream12));
                            }
                            else
                            {
                                result.ProblemAdd("Unable to make entry " + str6);
                            }
                        }
                        stream12.Finish();
                        stream12.Close();
                        return result;
                    }
                    catch (Exception exception7)
                    {
                        ProjectData.SetProjectError(exception7);
                        Exception exception3 = exception7;
                        stream12.Close();
                        result.ProblemAdd(exception3.Message);
                        result2 = result;
                        ProjectData.ClearProjectError();
                        return result2;
                    }
                }
                if (Args.CompileType == sWrite_WZ_Args.enumCompileType.Campaign)
                {
                    string path = modProgram.EndWithPathSeperator(Args.Path);
                    if (!Directory.Exists(path))
                    {
                        result.ProblemAdd("Directory " + path + " does not exist.");
                        return result;
                    }
                    string str8 = path + Args.MapName + ".gam";
                    result.Add(modIO.WriteMemoryToNewFile(stream3, path + Args.MapName + ".gam"));
                    path = path + Args.MapName + Conversions.ToString(modProgram.PlatformPathSeparator);
                    try
                    {
                        Directory.CreateDirectory(path);
                    }
                    catch (Exception exception8)
                    {
                        ProjectData.SetProjectError(exception8);
                        Exception exception4 = exception8;
                        result.ProblemAdd("Unable to create directory " + path);
                        result2 = result;
                        ProjectData.ClearProjectError();
                        return result2;
                    }
                    str8 = path + "dinit.bjo";
                    result.Add(modIO.WriteMemoryToNewFile(stream, str8));
                    str8 = path + "droid.ini";
                    result.Add(modIO.WriteMemoryToNewFile(stream8, str8));
                    str8 = path + "feat.bjo";
                    result.Add(modIO.WriteMemoryToNewFile(stream2, str8));
                    str8 = path + "feature.ini";
                    result.Add(modIO.WriteMemoryToNewFile(stream9, str8));
                    str8 = path + "game.map";
                    result.Add(modIO.WriteMemoryToNewFile(output, str8));
                    str8 = path + "struct.bjo";
                    result.Add(modIO.WriteMemoryToNewFile(stream6, str8));
                    str8 = path + "struct.ini";
                    result.Add(modIO.WriteMemoryToNewFile(stream11, str8));
                    str8 = path + "ttypes.ttp";
                    result.Add(modIO.WriteMemoryToNewFile(stream7, str8));
                    str8 = path + "labels.ini";
                    result.Add(modIO.WriteMemoryToNewFile(stream10, str8));
                }
                return result;
            }
            catch (Exception exception9)
            {
                ProjectData.SetProjectError(exception9);
                Exception exception5 = exception9;
                Debugger.Break();
                result.ProblemAdd(exception5.Message);
                result2 = result;
                ProjectData.ClearProjectError();
                return result2;
            }
            return result;
        }

        public clsMap MainMap
        {
            get
            {
                if (!this.frmMainLink.IsConnected)
                {
                    return null;
                }
                return this.frmMainLink.Source.MainMap;
            }
        }

        private Timer MakeMinimapTimer
        {
            get
            {
                return this._MakeMinimapTimer;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.MinimapTimer_Tick);
                if (this._MakeMinimapTimer != null)
                {
                    this._MakeMinimapTimer.Tick -= handler;
                }
                this._MakeMinimapTimer = value;
                if (this._MakeMinimapTimer != null)
                {
                    this._MakeMinimapTimer.Tick += handler;
                }
            }
        }

        public bool ReadyForUserInput
        {
            get
            {
                return this._ReadyForUserInput;
            }
        }

        public clsUnitGroupContainer SelectedUnitGroup
        {
            get
            {
                return this._SelectedUnitGroup;
            }
        }

        public delegate void ChangedEventHandler();

        public abstract class clsAction
        {
            public double Effect;
            public clsMap Map;
            public modMath.sXY_int PosNum;
            public bool UseEffect;

            protected clsAction()
            {
            }

            public abstract void ActionPerform();
        }

        public class clsApplyAutoTri : clsMap.clsAction
        {
            private double difA;
            private double difB;
            private bool NewTri;

            public override void ActionPerform()
            {
                this.difA = Math.Abs((double) (base.Map.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y + 1].Height - base.Map.Terrain.Vertices[this.PosNum.X, this.PosNum.Y].Height));
                this.difB = Math.Abs((double) (base.Map.Terrain.Vertices[this.PosNum.X, this.PosNum.Y + 1].Height - base.Map.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y].Height));
                if (this.difA == this.difB)
                {
                    this.NewTri = App.Random.Next() < 0.5f;
                }
                else
                {
                    this.NewTri = this.difA > this.difB;
                }
                base.Map.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].Tri = this.NewTri;
                base.Map.SectorGraphicsChanges.TileChanged(base.PosNum);
                base.Map.SectorUnitHeightsChanges.TileChanged(base.PosNum);
                base.Map.SectorTerrainUndoChanges.TileChanged(base.PosNum);
            }
        }

        public class clsApplyCliff : clsMap.clsAction
        {
            public double Angle;
            private bool CliffChanged;
            private double DifA;
            private double DifB;
            private double HeightA;
            private double HeightB;
            private bool NewVal;
            private modMath.sXY_int Pos;
            private int RandomNum;
            public bool SetTris;
            private clsMap.clsTerrain Terrain;
            private double TriBottomLeftMaxSlope;
            private double TriBottomRightMaxSlope;
            private bool TriChanged;
            private double TriTopLeftMaxSlope;
            private double TriTopRightMaxSlope;

            public override void ActionPerform()
            {
                this.Terrain = base.Map.Terrain;
                this.HeightA = (this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y].Height + this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y].Height) / 2.0;
                this.HeightB = (this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y + 1].Height + this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y + 1].Height) / 2.0;
                this.DifA = this.HeightB - this.HeightA;
                this.HeightA = (this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y].Height + this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y + 1].Height) / 2.0;
                this.HeightB = (this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y].Height + this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y + 1].Height) / 2.0;
                this.DifB = this.HeightB - this.HeightA;
                if (Math.Abs(this.DifA) == Math.Abs(this.DifB))
                {
                    this.RandomNum = (int) Math.Round((double) ((float) (App.Random.Next() * 4f)));
                    if (this.RandomNum == 0)
                    {
                        this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].DownSide = TileOrientation.TileDirection_Top;
                    }
                    else if (this.RandomNum == 1)
                    {
                        this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].DownSide = TileOrientation.TileDirection_Right;
                    }
                    else if (this.RandomNum == 2)
                    {
                        this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].DownSide = TileOrientation.TileDirection_Bottom;
                    }
                    else
                    {
                        this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].DownSide = TileOrientation.TileDirection_Left;
                    }
                }
                else if (Math.Abs(this.DifA) > Math.Abs(this.DifB))
                {
                    if (this.DifA < 0.0)
                    {
                        this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].DownSide = TileOrientation.TileDirection_Bottom;
                    }
                    else
                    {
                        this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].DownSide = TileOrientation.TileDirection_Top;
                    }
                }
                else if (this.DifB < 0.0)
                {
                    this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].DownSide = TileOrientation.TileDirection_Right;
                }
                else
                {
                    this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].DownSide = TileOrientation.TileDirection_Left;
                }
                this.CliffChanged = false;
                this.TriChanged = false;
                if (this.SetTris)
                {
                    this.DifA = Math.Abs((double) (this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y + 1].Height - this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y].Height));
                    this.DifB = Math.Abs((double) (this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y + 1].Height - this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y].Height));
                    if (this.DifA == this.DifB)
                    {
                        if (App.Random.Next() >= 0.5f)
                        {
                            this.NewVal = false;
                        }
                        else
                        {
                            this.NewVal = true;
                        }
                    }
                    else if (this.DifA < this.DifB)
                    {
                        this.NewVal = false;
                    }
                    else
                    {
                        this.NewVal = true;
                    }
                    if (this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].Tri != this.NewVal)
                    {
                        this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].Tri = this.NewVal;
                        this.TriChanged = true;
                    }
                }
                if (this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].Tri)
                {
                    this.Pos.X = (int) Math.Round((double) ((this.PosNum.X + 0.25) * 128.0));
                    this.Pos.Y = (int) Math.Round((double) ((this.PosNum.Y + 0.25) * 128.0));
                    this.TriTopLeftMaxSlope = base.Map.GetTerrainSlopeAngle(this.Pos);
                    this.Pos.X = (int) Math.Round((double) ((this.PosNum.X + 0.75) * 128.0));
                    this.Pos.Y = (int) Math.Round((double) ((this.PosNum.Y + 0.75) * 128.0));
                    this.TriBottomRightMaxSlope = base.Map.GetTerrainSlopeAngle(this.Pos);
                    if (this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].TriTopRightIsCliff)
                    {
                        this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].TriTopRightIsCliff = false;
                        this.CliffChanged = true;
                    }
                    if (this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].TriBottomLeftIsCliff)
                    {
                        this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].TriBottomLeftIsCliff = false;
                        this.CliffChanged = true;
                    }
                    this.NewVal = this.TriTopLeftMaxSlope >= this.Angle;
                    if (this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].TriTopLeftIsCliff != this.NewVal)
                    {
                        this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].TriTopLeftIsCliff = this.NewVal;
                        this.CliffChanged = true;
                    }
                    this.NewVal = this.TriBottomRightMaxSlope >= this.Angle;
                    if (this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].TriBottomRightIsCliff != this.NewVal)
                    {
                        this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].TriBottomRightIsCliff = this.NewVal;
                        this.CliffChanged = true;
                    }
                    if (this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].TriTopLeftIsCliff | this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].TriBottomRightIsCliff)
                    {
                        this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].Terrain_IsCliff = true;
                    }
                    else
                    {
                        this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].Terrain_IsCliff = false;
                    }
                }
                else
                {
                    this.Pos.X = (int) Math.Round((double) ((this.PosNum.X + 0.75) * 128.0));
                    this.Pos.Y = (int) Math.Round((double) ((this.PosNum.Y + 0.25) * 128.0));
                    this.TriTopRightMaxSlope = base.Map.GetTerrainSlopeAngle(this.Pos);
                    this.Pos.X = (int) Math.Round((double) ((this.PosNum.X + 0.25) * 128.0));
                    this.Pos.Y = (int) Math.Round((double) ((this.PosNum.Y + 0.75) * 128.0));
                    this.TriBottomLeftMaxSlope = base.Map.GetTerrainSlopeAngle(this.Pos);
                    if (this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].TriBottomRightIsCliff)
                    {
                        this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].TriBottomRightIsCliff = false;
                        this.CliffChanged = true;
                    }
                    if (this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].TriTopLeftIsCliff)
                    {
                        this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].TriTopLeftIsCliff = false;
                        this.CliffChanged = true;
                    }
                    this.NewVal = this.TriTopRightMaxSlope >= this.Angle;
                    if (this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].TriTopRightIsCliff != this.NewVal)
                    {
                        this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].TriTopRightIsCliff = this.NewVal;
                        this.CliffChanged = true;
                    }
                    this.NewVal = this.TriBottomLeftMaxSlope >= this.Angle;
                    if (this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].TriBottomLeftIsCliff != this.NewVal)
                    {
                        this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].TriBottomLeftIsCliff = this.NewVal;
                        this.CliffChanged = true;
                    }
                    if (this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].TriTopRightIsCliff | this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].TriBottomLeftIsCliff)
                    {
                        this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].Terrain_IsCliff = true;
                    }
                    else
                    {
                        this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].Terrain_IsCliff = false;
                    }
                }
                if (this.CliffChanged)
                {
                    base.Map.AutoTextureChanges.TileChanged(base.PosNum);
                }
                if (this.TriChanged | this.CliffChanged)
                {
                    base.Map.SectorGraphicsChanges.TileChanged(base.PosNum);
                    base.Map.SectorTerrainUndoChanges.TileChanged(base.PosNum);
                }
            }
        }

        public class clsApplyCliffRemove : clsMap.clsAction
        {
            private clsMap.clsTerrain Terrain;

            public override void ActionPerform()
            {
                this.Terrain = base.Map.Terrain;
                if ((((this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].Terrain_IsCliff | this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].TriBottomLeftIsCliff) | this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].TriBottomRightIsCliff) | this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].TriTopLeftIsCliff) | this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].TriTopRightIsCliff)
                {
                    this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].Terrain_IsCliff = false;
                    this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].TriBottomLeftIsCliff = false;
                    this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].TriBottomRightIsCliff = false;
                    this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].TriTopLeftIsCliff = false;
                    this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].TriTopRightIsCliff = false;
                    base.Map.AutoTextureChanges.TileChanged(base.PosNum);
                    base.Map.SectorGraphicsChanges.TileChanged(base.PosNum);
                    base.Map.SectorTerrainUndoChanges.TileChanged(base.PosNum);
                }
            }
        }

        public class clsApplyCliffTriangle : clsMap.clsAction
        {
            private bool CliffChanged;
            private clsMap.clsTerrain Terrain;
            public bool Triangle;

            public override void ActionPerform()
            {
                this.Terrain = base.Map.Terrain;
                this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].Terrain_IsCliff = true;
                this.CliffChanged = false;
                if (this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].Tri)
                {
                    if (this.Triangle)
                    {
                        if (!this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].TriBottomRightIsCliff)
                        {
                            this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].TriBottomRightIsCliff = true;
                            this.CliffChanged = true;
                        }
                    }
                    else if (!this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].TriTopLeftIsCliff)
                    {
                        this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].TriTopLeftIsCliff = true;
                        this.CliffChanged = true;
                    }
                }
                else if (this.Triangle)
                {
                    if (!this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].TriTopRightIsCliff)
                    {
                        this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].TriTopRightIsCliff = true;
                        this.CliffChanged = true;
                    }
                }
                else if (!this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].TriBottomLeftIsCliff)
                {
                    this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].TriBottomLeftIsCliff = true;
                    this.CliffChanged = true;
                }
                if (this.CliffChanged)
                {
                    base.Map.AutoTextureChanges.TileChanged(base.PosNum);
                    base.Map.SectorGraphicsChanges.TileChanged(base.PosNum);
                    base.Map.SectorTerrainUndoChanges.TileChanged(base.PosNum);
                    double num4 = (this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y].Height + this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y].Height) / 2.0;
                    double num5 = (this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y + 1].Height + this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y + 1].Height) / 2.0;
                    double num2 = num5 - num4;
                    num4 = (this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y].Height + this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y + 1].Height) / 2.0;
                    num5 = (this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y].Height + this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y + 1].Height) / 2.0;
                    double num3 = num5 - num4;
                    if (Math.Abs(num2) == Math.Abs(num3))
                    {
                        switch (((int) Math.Round((double) ((float) (App.Random.Next() * 4f)))))
                        {
                            case 0:
                                this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].DownSide = TileOrientation.TileDirection_Top;
                                return;

                            case 1:
                                this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].DownSide = TileOrientation.TileDirection_Right;
                                return;

                            case 2:
                                this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].DownSide = TileOrientation.TileDirection_Bottom;
                                return;
                        }
                        this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].DownSide = TileOrientation.TileDirection_Left;
                    }
                    else if (Math.Abs(num2) > Math.Abs(num3))
                    {
                        if (num2 < 0.0)
                        {
                            this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].DownSide = TileOrientation.TileDirection_Bottom;
                        }
                        else
                        {
                            this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].DownSide = TileOrientation.TileDirection_Top;
                        }
                    }
                    else if (num3 < 0.0)
                    {
                        this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].DownSide = TileOrientation.TileDirection_Right;
                    }
                    else
                    {
                        this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].DownSide = TileOrientation.TileDirection_Left;
                    }
                }
            }
        }

        public class clsApplyCliffTriangleRemove : clsMap.clsAction
        {
            private bool CliffChanged;
            private clsMap.clsTerrain Terrain;
            public bool Triangle;

            public override void ActionPerform()
            {
                this.Terrain = base.Map.Terrain;
                this.CliffChanged = false;
                if (this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].Tri)
                {
                    if (this.Triangle)
                    {
                        if (this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].TriBottomRightIsCliff)
                        {
                            this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].TriBottomRightIsCliff = false;
                            this.CliffChanged = true;
                        }
                    }
                    else if (this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].TriTopLeftIsCliff)
                    {
                        this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].TriTopLeftIsCliff = false;
                        this.CliffChanged = true;
                    }
                    this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].Terrain_IsCliff = this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].TriBottomRightIsCliff | this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].TriTopLeftIsCliff;
                }
                else
                {
                    if (this.Triangle)
                    {
                        if (this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].TriTopRightIsCliff)
                        {
                            this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].TriTopRightIsCliff = false;
                            this.CliffChanged = true;
                        }
                    }
                    else if (this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].TriBottomLeftIsCliff)
                    {
                        this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].TriBottomLeftIsCliff = false;
                        this.CliffChanged = true;
                    }
                    this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].Terrain_IsCliff = this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].TriTopRightIsCliff | this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].TriBottomLeftIsCliff;
                }
                if (this.CliffChanged)
                {
                    base.Map.AutoTextureChanges.TileChanged(base.PosNum);
                    base.Map.SectorGraphicsChanges.TileChanged(base.PosNum);
                    base.Map.SectorTerrainUndoChanges.TileChanged(base.PosNum);
                }
            }
        }

        public class clsApplyHeightChange : clsMap.clsAction
        {
            public double Rate;
            private clsMap.clsTerrain Terrain;

            public override void ActionPerform()
            {
                this.Terrain = base.Map.Terrain;
                this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y].Height = (byte) modMath.Clamp_int(this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y].Height + ((int) Math.Round((double) (this.Rate * base.Effect))), 0, 0xff);
                base.Map.SectorGraphicsChanges.VertexAndNormalsChanged(base.PosNum);
                base.Map.SectorUnitHeightsChanges.VertexChanged(base.PosNum);
                base.Map.SectorTerrainUndoChanges.VertexChanged(base.PosNum);
            }
        }

        public class clsApplyHeightSet : clsMap.clsAction
        {
            public byte Height;
            private clsMap.clsTerrain Terrain;

            public override void ActionPerform()
            {
                this.Terrain = base.Map.Terrain;
                if (this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y].Height != this.Height)
                {
                    this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y].Height = this.Height;
                    base.Map.SectorGraphicsChanges.VertexAndNormalsChanged(base.PosNum);
                    base.Map.SectorUnitHeightsChanges.VertexChanged(base.PosNum);
                    base.Map.SectorTerrainUndoChanges.VertexChanged(base.PosNum);
                }
            }
        }

        public class clsApplyHeightSmoothing : clsMap.clsAction
        {
            public modMath.sXY_int AreaTileSize;
            private int LimitX;
            private int LimitY;
            private byte[,] NewHeight;
            public modMath.sXY_int Offset;
            public double Ratio;
            private int Samples;
            private bool Started;
            private int TempHeight;
            private clsMap.clsTerrain Terrain;
            private modMath.sXY_int VertexNum;
            private int XNum;

            public override void ActionPerform()
            {
                if (!this.Started)
                {
                    Debugger.Break();
                }
                else
                {
                    this.Terrain = base.Map.Terrain;
                    this.LimitX = this.Terrain.TileSize.X;
                    this.LimitY = this.Terrain.TileSize.Y;
                    this.TempHeight = 0;
                    this.Samples = 0;
                    int introduced6 = modMath.Clamp_int(modProgram.SmoothRadius.Tiles.YMin + this.PosNum.Y, 0, this.LimitY);
                    int introduced7 = modMath.Clamp_int(modProgram.SmoothRadius.Tiles.YMax + this.PosNum.Y, 0, this.LimitY);
                    int num5 = introduced7 - this.PosNum.Y;
                    for (int i = introduced6 - this.PosNum.Y; i <= num5; i++)
                    {
                        int num4 = this.PosNum.Y + i;
                        this.XNum = i - modProgram.SmoothRadius.Tiles.YMin;
                        int introduced8 = modMath.Clamp_int(modProgram.SmoothRadius.Tiles.XMin[this.XNum] + this.PosNum.X, 0, this.LimitX);
                        int introduced9 = modMath.Clamp_int(modProgram.SmoothRadius.Tiles.XMax[this.XNum] + this.PosNum.X, 0, this.LimitX);
                        int num6 = introduced9 - this.PosNum.X;
                        for (int j = introduced8 - this.PosNum.X; j <= num6; j++)
                        {
                            int num2 = this.PosNum.X + j;
                            this.TempHeight += this.Terrain.Vertices[num2, num4].Height;
                            this.Samples++;
                        }
                    }
                    this.NewHeight[this.PosNum.X - this.Offset.X, this.PosNum.Y - this.Offset.Y] = (byte) Math.Min((int) Math.Round((double) ((this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y].Height * (1.0 - this.Ratio)) + ((((double) this.TempHeight) / ((double) this.Samples)) * this.Ratio))), 0xff);
                }
            }

            public void Finish()
            {
                if (!this.Started)
                {
                    Debugger.Break();
                }
                else
                {
                    this.Terrain = base.Map.Terrain;
                    int y = this.AreaTileSize.Y;
                    for (int i = 0; i <= y; i++)
                    {
                        this.VertexNum.Y = this.Offset.Y + i;
                        int x = this.AreaTileSize.X;
                        for (int j = 0; j <= x; j++)
                        {
                            this.VertexNum.X = this.Offset.X + j;
                            this.Terrain.Vertices[this.VertexNum.X, this.VertexNum.Y].Height = this.NewHeight[j, i];
                            base.Map.SectorGraphicsChanges.VertexAndNormalsChanged(this.VertexNum);
                            base.Map.SectorUnitHeightsChanges.VertexChanged(this.VertexNum);
                            base.Map.SectorTerrainUndoChanges.VertexChanged(this.VertexNum);
                        }
                    }
                    this.Started = false;
                }
            }

            public void Start()
            {
                this.Terrain = base.Map.Terrain;
                this.NewHeight = new byte[this.AreaTileSize.X + 1, this.AreaTileSize.Y + 1];
                int y = this.AreaTileSize.Y;
                for (int i = 0; i <= y; i++)
                {
                    int x = this.AreaTileSize.X;
                    for (int j = 0; j <= x; j++)
                    {
                        this.NewHeight[j, i] = this.Terrain.Vertices[this.Offset.X + j, this.Offset.Y + i].Height;
                    }
                }
                this.Started = true;
            }
        }

        public class clsApplyRoadRemove : clsMap.clsAction
        {
            private clsMap.clsTerrain Terrain;

            public override void ActionPerform()
            {
                this.ToolPerformSideH(base.PosNum);
                modMath.sXY_int sideNum = new modMath.sXY_int(this.PosNum.X, this.PosNum.Y + 1);
                this.ToolPerformSideH(sideNum);
                this.ToolPerformSideV(base.PosNum);
                sideNum = new modMath.sXY_int(this.PosNum.X + 1, this.PosNum.Y);
                this.ToolPerformSideV(sideNum);
            }

            private void ToolPerformSideH(modMath.sXY_int SideNum)
            {
                this.Terrain = base.Map.Terrain;
                if (this.Terrain.SideH[SideNum.X, SideNum.Y].Road != null)
                {
                    this.Terrain.SideH[SideNum.X, SideNum.Y].Road = null;
                    base.Map.AutoTextureChanges.SideHChanged(SideNum);
                    base.Map.SectorGraphicsChanges.SideHChanged(SideNum);
                    base.Map.SectorTerrainUndoChanges.SideHChanged(SideNum);
                }
            }

            private void ToolPerformSideV(modMath.sXY_int SideNum)
            {
                this.Terrain = base.Map.Terrain;
                if (this.Terrain.SideV[SideNum.X, SideNum.Y].Road != null)
                {
                    this.Terrain.SideV[SideNum.X, SideNum.Y].Road = null;
                    base.Map.AutoTextureChanges.SideVChanged(SideNum);
                    base.Map.SectorGraphicsChanges.SideVChanged(SideNum);
                    base.Map.SectorTerrainUndoChanges.SideVChanged(SideNum);
                }
            }
        }

        public class clsApplySideHTerrainInterpret : clsMap.clsApplySideTerrainInterpret
        {
            public override void ActionPerform()
            {
                base.ActionPerform();
                if (this.PosNum.Y > 0)
                {
                    base.SideDirection = TileOrientation.TileDirection_Bottom;
                    base.Tile = base.Terrain.Tiles[this.PosNum.X, this.PosNum.Y - 1];
                    base.Texture = this.Tile.Texture;
                    this.ToolPerformTile();
                }
                if (this.PosNum.Y < base.Terrain.TileSize.Y)
                {
                    base.SideDirection = TileOrientation.TileDirection_Top;
                    base.Tile = base.Terrain.Tiles[this.PosNum.X, this.PosNum.Y];
                    base.Texture = this.Tile.Texture;
                    this.ToolPerformTile();
                }
                base.BestNum = -1;
                base.BestCount = 0;
                int num2 = base.Painter.RoadCount - 1;
                for (int i = 0; i <= num2; i++)
                {
                    if (base.RoadCount[i] > base.BestCount)
                    {
                        base.BestNum = i;
                        base.BestCount = base.RoadCount[i];
                    }
                }
                if (base.BestCount > 0)
                {
                    base.Terrain.SideH[this.PosNum.X, this.PosNum.Y].Road = base.Painter.Roads[base.BestNum];
                }
                else
                {
                    base.Terrain.SideH[this.PosNum.X, this.PosNum.Y].Road = null;
                }
                base.Map.SectorTerrainUndoChanges.SideHChanged(base.PosNum);
            }
        }

        public abstract class clsApplySideTerrainInterpret : clsMap.clsAction
        {
            protected int BestCount;
            protected int BestNum;
            protected TileOrientation.sTileDirection OppositeDirection;
            protected clsPainter Painter;
            protected clsPainter.clsRoad PainterRoad;
            protected clsPainter.clsTerrain PainterTerrain;
            protected clsPainter.clsTileList.sTileOrientationChance PainterTexture;
            protected TileOrientation.sTileDirection ResultDirection;
            protected int[] RoadCount;
            protected TileOrientation.sTileDirection SideDirection;
            protected clsMap.clsTerrain Terrain;
            protected FlaME.clsMap.clsTerrain.Tile.sTexture Texture;
            protected FlaME.clsMap.clsTerrain.Tile Tile;

            protected clsApplySideTerrainInterpret()
            {
            }

            public override void ActionPerform()
            {
                this.Terrain = base.Map.Terrain;
                this.Painter = base.Map.Painter;
                this.RoadCount = new int[(this.Painter.RoadCount - 1) + 1];
            }

            protected void ToolPerformTile()
            {
                int num3 = this.Painter.RoadBrushCount - 1;
                for (int i = 0; i <= num3; i++)
                {
                    int[] roadCount;
                    int num;
                    this.PainterRoad = this.Painter.RoadBrushes[i].Road;
                    this.PainterTerrain = this.Painter.RoadBrushes[i].Terrain;
                    int num4 = this.Painter.RoadBrushes[i].Tile_Corner_In.TileCount - 1;
                    int index = 0;
                    while (index <= num4)
                    {
                        this.PainterTexture = this.Painter.RoadBrushes[i].Tile_Corner_In.Tiles[index];
                        if (this.PainterTexture.TextureNum == this.Texture.TextureNum)
                        {
                            TileOrientation.RotateDirection(this.PainterTexture.Direction, this.Texture.Orientation, ref this.ResultDirection);
                            if (TileOrientation.DirectionsOnSameSide(this.SideDirection, this.ResultDirection))
                            {
                                roadCount = this.RoadCount;
                                num = this.PainterRoad.Num;
                                roadCount[num]++;
                            }
                        }
                        index++;
                    }
                    int num6 = this.Painter.RoadBrushes[i].Tile_CrossIntersection.TileCount - 1;
                    index = 0;
                    while (index <= num6)
                    {
                        this.PainterTexture = this.Painter.RoadBrushes[i].Tile_CrossIntersection.Tiles[index];
                        if (this.PainterTexture.TextureNum == this.Texture.TextureNum)
                        {
                            roadCount = this.RoadCount;
                            num = this.PainterRoad.Num;
                            roadCount[num]++;
                        }
                        index++;
                    }
                    int num7 = this.Painter.RoadBrushes[i].Tile_End.TileCount - 1;
                    index = 0;
                    while (index <= num7)
                    {
                        this.PainterTexture = this.Painter.RoadBrushes[i].Tile_End.Tiles[index];
                        if (this.PainterTexture.TextureNum == this.Texture.TextureNum)
                        {
                            TileOrientation.RotateDirection(this.PainterTexture.Direction, this.Texture.Orientation, ref this.ResultDirection);
                            if (TileOrientation.IdenticalTileDirections(this.SideDirection, this.ResultDirection))
                            {
                                roadCount = this.RoadCount;
                                num = this.PainterRoad.Num;
                                roadCount[num]++;
                            }
                        }
                        index++;
                    }
                    int num8 = this.Painter.RoadBrushes[i].Tile_Straight.TileCount - 1;
                    index = 0;
                    while (index <= num8)
                    {
                        this.PainterTexture = this.Painter.RoadBrushes[i].Tile_Straight.Tiles[index];
                        if (this.PainterTexture.TextureNum == this.Texture.TextureNum)
                        {
                            TileOrientation.RotateDirection(this.PainterTexture.Direction, this.Texture.Orientation, ref this.ResultDirection);
                            if (TileOrientation.DirectionsAreInLine(this.SideDirection, this.ResultDirection))
                            {
                                roadCount = this.RoadCount;
                                num = this.PainterRoad.Num;
                                roadCount[num]++;
                            }
                        }
                        index++;
                    }
                    int num9 = this.Painter.RoadBrushes[i].Tile_TIntersection.TileCount - 1;
                    for (index = 0; index <= num9; index++)
                    {
                        this.PainterTexture = this.Painter.RoadBrushes[i].Tile_TIntersection.Tiles[index];
                        if (this.PainterTexture.TextureNum == this.Texture.TextureNum)
                        {
                            TileOrientation.RotateDirection(this.PainterTexture.Direction, this.Texture.Orientation, ref this.ResultDirection);
                            if (!TileOrientation.DirectionsOnSameSide(this.SideDirection, this.ResultDirection))
                            {
                                roadCount = this.RoadCount;
                                num = this.PainterRoad.Num;
                                roadCount[num]++;
                            }
                        }
                    }
                }
            }
        }

        public class clsApplySideVTerrainInterpret : clsMap.clsApplySideTerrainInterpret
        {
            public override void ActionPerform()
            {
                base.ActionPerform();
                if (this.PosNum.X > 0)
                {
                    base.SideDirection = TileOrientation.TileDirection_Right;
                    base.Tile = base.Terrain.Tiles[this.PosNum.X - 1, this.PosNum.Y];
                    base.Texture = this.Tile.Texture;
                    this.ToolPerformTile();
                }
                if (this.PosNum.X < base.Terrain.TileSize.X)
                {
                    base.SideDirection = TileOrientation.TileDirection_Left;
                    base.Tile = base.Terrain.Tiles[this.PosNum.X, this.PosNum.Y];
                    base.Texture = this.Tile.Texture;
                    this.ToolPerformTile();
                }
                base.BestNum = -1;
                base.BestCount = 0;
                int num2 = base.Painter.RoadCount - 1;
                for (int i = 0; i <= num2; i++)
                {
                    if (base.RoadCount[i] > base.BestCount)
                    {
                        base.BestNum = i;
                        base.BestCount = base.RoadCount[i];
                    }
                }
                if (base.BestCount > 0)
                {
                    base.Terrain.SideV[this.PosNum.X, this.PosNum.Y].Road = base.Painter.Roads[base.BestNum];
                }
                else
                {
                    base.Terrain.SideV[this.PosNum.X, this.PosNum.Y].Road = null;
                }
                base.Map.SectorTerrainUndoChanges.SideVChanged(base.PosNum);
            }
        }

        public class clsApplyTexture : clsMap.clsAction
        {
            public TileOrientation.sTileOrientation Orientation;
            public bool RandomOrientation;
            public bool SetOrientation;
            public bool SetTexture;
            private clsMap.clsTerrain Terrain;
            public modProgram.enumTextureTerrainAction TerrainAction;
            public int TextureNum;

            public override void ActionPerform()
            {
                this.Terrain = base.Map.Terrain;
                this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].Terrain_IsCliff = false;
                this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].TriTopLeftIsCliff = false;
                this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].TriTopRightIsCliff = false;
                this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].TriBottomLeftIsCliff = false;
                this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].TriBottomRightIsCliff = false;
                if (this.SetTexture)
                {
                    this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].Texture.TextureNum = this.TextureNum;
                }
                if (this.SetOrientation)
                {
                    if (this.RandomOrientation)
                    {
                        this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].Texture.Orientation = new TileOrientation.sTileOrientation(App.Random.Next() < 0.5f, App.Random.Next() < 0.5f, App.Random.Next() < 0.5f);
                    }
                    else
                    {
                        this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].Texture.Orientation = this.Orientation;
                    }
                }
                base.Map.TileTextureChangeTerrainAction(base.PosNum, this.TerrainAction);
                base.Map.SectorGraphicsChanges.TileChanged(base.PosNum);
                base.Map.SectorTerrainUndoChanges.TileChanged(base.PosNum);
            }
        }

        public class clsApplyTileTerrainInterpret : clsMap.clsAction
        {
            private TileOrientation.sTileDirection OppositeDirection;
            private clsPainter Painter;
            private clsPainter.clsTerrain PainterTerrainA;
            private clsPainter.clsTerrain PainterTerrainB;
            private clsPainter.clsTileList.sTileOrientationChance PainterTexture;
            private TileOrientation.sTileDirection ResultDirection;
            private clsMap.clsTerrain Terrain;
            private FlaME.clsMap.clsTerrain.Tile.sTexture Texture;
            private FlaME.clsMap.clsTerrain.Tile Tile;

            public override void ActionPerform()
            {
                this.Terrain = base.Map.Terrain;
                this.Painter = base.Map.Painter;
                this.Tile = this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y];
                this.Texture = this.Tile.Texture;
                this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].TriTopLeftIsCliff = false;
                this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].TriTopRightIsCliff = false;
                this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].TriBottomLeftIsCliff = false;
                this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].TriBottomRightIsCliff = false;
                this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].DownSide = TileOrientation.TileDirection_None;
                int num3 = this.Painter.CliffBrushCount - 1;
                for (int i = 0; i <= num3; i++)
                {
                    this.PainterTerrainA = this.Painter.CliffBrushes[i].Terrain_Inner;
                    this.PainterTerrainB = this.Painter.CliffBrushes[i].Terrain_Outer;
                    int num4 = this.Painter.CliffBrushes[i].Tiles_Straight.TileCount - 1;
                    int index = 0;
                    while (index <= num4)
                    {
                        this.PainterTexture = this.Painter.CliffBrushes[i].Tiles_Straight.Tiles[index];
                        if (this.PainterTexture.TextureNum == this.Texture.TextureNum)
                        {
                            if (this.Tile.Tri)
                            {
                                this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].TriTopLeftIsCliff = true;
                                this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].TriBottomRightIsCliff = true;
                            }
                            else
                            {
                                this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].TriTopRightIsCliff = true;
                                this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].TriBottomLeftIsCliff = true;
                            }
                            this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].Terrain_IsCliff = true;
                            TileOrientation.RotateDirection(this.PainterTexture.Direction, this.Texture.Orientation, ref this.ResultDirection);
                            this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].DownSide = this.ResultDirection;
                        }
                        index++;
                    }
                    int num5 = this.Painter.CliffBrushes[i].Tiles_Corner_In.TileCount - 1;
                    index = 0;
                    while (index <= num5)
                    {
                        this.PainterTexture = this.Painter.CliffBrushes[i].Tiles_Corner_In.Tiles[index];
                        if (this.PainterTexture.TextureNum == this.Texture.TextureNum)
                        {
                            TileOrientation.RotateDirection(this.PainterTexture.Direction, this.Texture.Orientation, ref this.ResultDirection);
                            if (this.Tile.Tri)
                            {
                                if (TileOrientation.IdenticalTileDirections(this.ResultDirection, TileOrientation.TileDirection_TopLeft))
                                {
                                    this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].TriTopLeftIsCliff = true;
                                    this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].Terrain_IsCliff = true;
                                }
                                else if (TileOrientation.IdenticalTileDirections(this.ResultDirection, TileOrientation.TileDirection_BottomRight))
                                {
                                    this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].TriBottomRightIsCliff = true;
                                    this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].Terrain_IsCliff = true;
                                }
                            }
                            else if (TileOrientation.IdenticalTileDirections(this.ResultDirection, TileOrientation.TileDirection_TopRight))
                            {
                                this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].TriTopRightIsCliff = true;
                                this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].Terrain_IsCliff = true;
                            }
                            else if (TileOrientation.IdenticalTileDirections(this.ResultDirection, TileOrientation.TileDirection_BottomLeft))
                            {
                                this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].TriBottomLeftIsCliff = true;
                                this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].Terrain_IsCliff = true;
                            }
                        }
                        index++;
                    }
                    int num6 = this.Painter.CliffBrushes[i].Tiles_Corner_Out.TileCount - 1;
                    for (index = 0; index <= num6; index++)
                    {
                        this.PainterTexture = this.Painter.CliffBrushes[i].Tiles_Corner_Out.Tiles[index];
                        if (this.PainterTexture.TextureNum == this.Texture.TextureNum)
                        {
                            this.OppositeDirection = this.PainterTexture.Direction;
                            this.OppositeDirection.FlipX();
                            this.OppositeDirection.FlipY();
                            TileOrientation.RotateDirection(this.OppositeDirection, this.Texture.Orientation, ref this.ResultDirection);
                            if (this.Tile.Tri)
                            {
                                if (TileOrientation.IdenticalTileDirections(this.ResultDirection, TileOrientation.TileDirection_TopLeft))
                                {
                                    this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].TriTopLeftIsCliff = true;
                                    this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].Terrain_IsCliff = true;
                                }
                                else if (TileOrientation.IdenticalTileDirections(this.ResultDirection, TileOrientation.TileDirection_BottomRight))
                                {
                                    this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].TriBottomRightIsCliff = true;
                                    this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].Terrain_IsCliff = true;
                                }
                            }
                            else if (TileOrientation.IdenticalTileDirections(this.ResultDirection, TileOrientation.TileDirection_TopRight))
                            {
                                this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].TriTopRightIsCliff = true;
                                this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].Terrain_IsCliff = true;
                            }
                            else if (TileOrientation.IdenticalTileDirections(this.ResultDirection, TileOrientation.TileDirection_BottomLeft))
                            {
                                this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].TriBottomLeftIsCliff = true;
                                this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].Terrain_IsCliff = true;
                            }
                        }
                    }
                }
                base.Map.SectorTerrainUndoChanges.TileChanged(base.PosNum);
            }
        }

        public class clsApplyVertexTerrain : clsMap.clsAction
        {
            private clsMap.clsTerrain Terrain;
            public clsPainter.clsTerrain VertexTerrain;

            public override void ActionPerform()
            {
                this.Terrain = base.Map.Terrain;
                if (this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y].Terrain != this.VertexTerrain)
                {
                    this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y].Terrain = this.VertexTerrain;
                    base.Map.SectorGraphicsChanges.VertexChanged(base.PosNum);
                    base.Map.SectorTerrainUndoChanges.VertexChanged(base.PosNum);
                    base.Map.AutoTextureChanges.VertexChanged(base.PosNum);
                }
            }
        }

        public class clsApplyVertexTerrainInterpret : clsMap.clsAction
        {
            private int BestCount;
            private int BestNum;
            private TileOrientation.sTileDirection OppositeDirection;
            private clsPainter Painter;
            private clsPainter.clsTerrain PainterTerrainA;
            private clsPainter.clsTerrain PainterTerrainB;
            private clsPainter.clsTileList.sTileOrientationChance PainterTexture;
            private TileOrientation.sTileDirection ResultDirection;
            private clsMap.clsTerrain Terrain;
            private int[] TerrainCount;
            private FlaME.clsMap.clsTerrain.Tile.sTexture Texture;
            private FlaME.clsMap.clsTerrain.Tile Tile;
            private TileOrientation.sTileDirection VertexDirection;

            public override void ActionPerform()
            {
                this.Terrain = base.Map.Terrain;
                this.Painter = base.Map.Painter;
                this.TerrainCount = new int[(this.Painter.TerrainCount - 1) + 1];
                if (this.PosNum.Y > 0)
                {
                    if (this.PosNum.X > 0)
                    {
                        this.VertexDirection = TileOrientation.TileDirection_BottomRight;
                        this.Tile = this.Terrain.Tiles[this.PosNum.X - 1, this.PosNum.Y - 1];
                        this.Texture = this.Tile.Texture;
                        this.ToolPerformTile();
                    }
                    if (this.PosNum.X < this.Terrain.TileSize.X)
                    {
                        this.VertexDirection = TileOrientation.TileDirection_BottomLeft;
                        this.Tile = this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y - 1];
                        this.Texture = this.Tile.Texture;
                        this.ToolPerformTile();
                    }
                }
                if (this.PosNum.Y < this.Terrain.TileSize.Y)
                {
                    if (this.PosNum.X > 0)
                    {
                        this.VertexDirection = TileOrientation.TileDirection_TopRight;
                        this.Tile = this.Terrain.Tiles[this.PosNum.X - 1, this.PosNum.Y];
                        this.Texture = this.Tile.Texture;
                        this.ToolPerformTile();
                    }
                    if (this.PosNum.X < this.Terrain.TileSize.X)
                    {
                        this.VertexDirection = TileOrientation.TileDirection_TopLeft;
                        this.Tile = this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y];
                        this.Texture = this.Tile.Texture;
                        this.ToolPerformTile();
                    }
                }
                this.BestNum = -1;
                this.BestCount = 0;
                int num2 = this.Painter.TerrainCount - 1;
                for (int i = 0; i <= num2; i++)
                {
                    if (this.TerrainCount[i] > this.BestCount)
                    {
                        this.BestNum = i;
                        this.BestCount = this.TerrainCount[i];
                    }
                }
                if (this.BestCount > 0)
                {
                    this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y].Terrain = this.Painter.Terrains[this.BestNum];
                }
                else
                {
                    this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y].Terrain = null;
                }
                base.Map.SectorTerrainUndoChanges.VertexChanged(base.PosNum);
            }

            private void ToolPerformTile()
            {
                int num;
                int num2;
                int[] terrainCount;
                int num5;
                int num3 = this.Painter.TerrainCount - 1;
                for (num2 = 0; num2 <= num3; num2++)
                {
                    this.PainterTerrainA = this.Painter.Terrains[num2];
                    int num4 = this.PainterTerrainA.Tiles.TileCount - 1;
                    num = 0;
                    while (num <= num4)
                    {
                        this.PainterTexture = this.PainterTerrainA.Tiles.Tiles[num];
                        if (this.PainterTexture.TextureNum == this.Texture.TextureNum)
                        {
                            terrainCount = this.TerrainCount;
                            num5 = this.PainterTerrainA.Num;
                            terrainCount[num5]++;
                        }
                        num++;
                    }
                }
                int num6 = this.Painter.TransitionBrushCount - 1;
                for (num2 = 0; num2 <= num6; num2++)
                {
                    this.PainterTerrainA = this.Painter.TransitionBrushes[num2].Terrain_Inner;
                    this.PainterTerrainB = this.Painter.TransitionBrushes[num2].Terrain_Outer;
                    int num7 = this.Painter.TransitionBrushes[num2].Tiles_Straight.TileCount - 1;
                    num = 0;
                    while (num <= num7)
                    {
                        this.PainterTexture = this.Painter.TransitionBrushes[num2].Tiles_Straight.Tiles[num];
                        if (this.PainterTexture.TextureNum == this.Texture.TextureNum)
                        {
                            TileOrientation.RotateDirection(this.PainterTexture.Direction, this.Texture.Orientation, ref this.ResultDirection);
                            if (TileOrientation.DirectionsOnSameSide(this.VertexDirection, this.ResultDirection))
                            {
                                terrainCount = this.TerrainCount;
                                num5 = this.PainterTerrainB.Num;
                                terrainCount[num5]++;
                            }
                            else
                            {
                                terrainCount = this.TerrainCount;
                                num5 = this.PainterTerrainA.Num;
                                terrainCount[num5]++;
                            }
                        }
                        num++;
                    }
                    int num8 = this.Painter.TransitionBrushes[num2].Tiles_Corner_In.TileCount - 1;
                    num = 0;
                    while (num <= num8)
                    {
                        this.PainterTexture = this.Painter.TransitionBrushes[num2].Tiles_Corner_In.Tiles[num];
                        if (this.PainterTexture.TextureNum == this.Texture.TextureNum)
                        {
                            TileOrientation.RotateDirection(this.PainterTexture.Direction, this.Texture.Orientation, ref this.ResultDirection);
                            if (TileOrientation.IdenticalTileDirections(this.VertexDirection, this.ResultDirection))
                            {
                                terrainCount = this.TerrainCount;
                                num5 = this.PainterTerrainB.Num;
                                terrainCount[num5]++;
                            }
                            else
                            {
                                terrainCount = this.TerrainCount;
                                num5 = this.PainterTerrainA.Num;
                                terrainCount[num5]++;
                            }
                        }
                        num++;
                    }
                    int num9 = this.Painter.TransitionBrushes[num2].Tiles_Corner_Out.TileCount - 1;
                    num = 0;
                    while (num <= num9)
                    {
                        this.PainterTexture = this.Painter.TransitionBrushes[num2].Tiles_Corner_Out.Tiles[num];
                        if (this.PainterTexture.TextureNum == this.Texture.TextureNum)
                        {
                            this.OppositeDirection = this.PainterTexture.Direction;
                            this.OppositeDirection.FlipX();
                            this.OppositeDirection.FlipY();
                            TileOrientation.RotateDirection(this.OppositeDirection, this.Texture.Orientation, ref this.ResultDirection);
                            if (TileOrientation.IdenticalTileDirections(this.VertexDirection, this.ResultDirection))
                            {
                                terrainCount = this.TerrainCount;
                                num5 = this.PainterTerrainA.Num;
                                terrainCount[num5]++;
                            }
                            else
                            {
                                terrainCount = this.TerrainCount;
                                num5 = this.PainterTerrainB.Num;
                                terrainCount[num5]++;
                            }
                        }
                        num++;
                    }
                }
                int num10 = this.Painter.CliffBrushCount - 1;
                for (num2 = 0; num2 <= num10; num2++)
                {
                    this.PainterTerrainA = this.Painter.CliffBrushes[num2].Terrain_Inner;
                    this.PainterTerrainB = this.Painter.CliffBrushes[num2].Terrain_Outer;
                    int num11 = this.Painter.CliffBrushes[num2].Tiles_Straight.TileCount - 1;
                    num = 0;
                    while (num <= num11)
                    {
                        this.PainterTexture = this.Painter.CliffBrushes[num2].Tiles_Straight.Tiles[num];
                        if (this.PainterTexture.TextureNum == this.Texture.TextureNum)
                        {
                            TileOrientation.RotateDirection(this.PainterTexture.Direction, this.Texture.Orientation, ref this.ResultDirection);
                            if (TileOrientation.DirectionsOnSameSide(this.VertexDirection, this.ResultDirection))
                            {
                                terrainCount = this.TerrainCount;
                                num5 = this.PainterTerrainB.Num;
                                terrainCount[num5]++;
                            }
                            else
                            {
                                terrainCount = this.TerrainCount;
                                num5 = this.PainterTerrainA.Num;
                                terrainCount[num5]++;
                            }
                        }
                        num++;
                    }
                    int num12 = this.Painter.CliffBrushes[num2].Tiles_Corner_In.TileCount - 1;
                    num = 0;
                    while (num <= num12)
                    {
                        this.PainterTexture = this.Painter.CliffBrushes[num2].Tiles_Corner_In.Tiles[num];
                        if (this.PainterTexture.TextureNum == this.Texture.TextureNum)
                        {
                            TileOrientation.RotateDirection(this.PainterTexture.Direction, this.Texture.Orientation, ref this.ResultDirection);
                            if (TileOrientation.IdenticalTileDirections(this.VertexDirection, this.ResultDirection))
                            {
                                terrainCount = this.TerrainCount;
                                num5 = this.PainterTerrainA.Num;
                                terrainCount[num5]++;
                            }
                            else
                            {
                                terrainCount = this.TerrainCount;
                                num5 = this.PainterTerrainB.Num;
                                terrainCount[num5]++;
                            }
                        }
                        num++;
                    }
                    int num13 = this.Painter.CliffBrushes[num2].Tiles_Corner_Out.TileCount - 1;
                    num = 0;
                    while (num <= num13)
                    {
                        this.PainterTexture = this.Painter.CliffBrushes[num2].Tiles_Corner_Out.Tiles[num];
                        if (this.PainterTexture.TextureNum == this.Texture.TextureNum)
                        {
                            this.OppositeDirection = this.PainterTexture.Direction;
                            this.OppositeDirection.FlipX();
                            this.OppositeDirection.FlipY();
                            TileOrientation.RotateDirection(this.OppositeDirection, this.Texture.Orientation, ref this.ResultDirection);
                            if (TileOrientation.IdenticalTileDirections(this.VertexDirection, this.ResultDirection))
                            {
                                terrainCount = this.TerrainCount;
                                num5 = this.PainterTerrainA.Num;
                                terrainCount[num5]++;
                            }
                            else
                            {
                                terrainCount = this.TerrainCount;
                                num5 = this.PainterTerrainB.Num;
                                terrainCount[num5]++;
                            }
                        }
                        num++;
                    }
                }
                int num14 = this.Painter.RoadBrushCount - 1;
                for (num2 = 0; num2 <= num14; num2++)
                {
                    this.PainterTerrainA = this.Painter.RoadBrushes[num2].Terrain;
                    int num15 = this.Painter.RoadBrushes[num2].Tile_Corner_In.TileCount - 1;
                    num = 0;
                    while (num <= num15)
                    {
                        this.PainterTexture = this.Painter.RoadBrushes[num2].Tile_Corner_In.Tiles[num];
                        if (this.PainterTexture.TextureNum == this.Texture.TextureNum)
                        {
                            terrainCount = this.TerrainCount;
                            num5 = this.PainterTerrainA.Num;
                            terrainCount[num5]++;
                        }
                        num++;
                    }
                    int num16 = this.Painter.RoadBrushes[num2].Tile_CrossIntersection.TileCount - 1;
                    num = 0;
                    while (num <= num16)
                    {
                        this.PainterTexture = this.Painter.RoadBrushes[num2].Tile_CrossIntersection.Tiles[num];
                        if (this.PainterTexture.TextureNum == this.Texture.TextureNum)
                        {
                            terrainCount = this.TerrainCount;
                            num5 = this.PainterTerrainA.Num;
                            terrainCount[num5]++;
                        }
                        num++;
                    }
                    int num17 = this.Painter.RoadBrushes[num2].Tile_End.TileCount - 1;
                    num = 0;
                    while (num <= num17)
                    {
                        this.PainterTexture = this.Painter.RoadBrushes[num2].Tile_End.Tiles[num];
                        if (this.PainterTexture.TextureNum == this.Texture.TextureNum)
                        {
                            terrainCount = this.TerrainCount;
                            num5 = this.PainterTerrainA.Num;
                            terrainCount[num5]++;
                        }
                        num++;
                    }
                    int num18 = this.Painter.RoadBrushes[num2].Tile_Straight.TileCount - 1;
                    num = 0;
                    while (num <= num18)
                    {
                        this.PainterTexture = this.Painter.RoadBrushes[num2].Tile_Straight.Tiles[num];
                        if (this.PainterTexture.TextureNum == this.Texture.TextureNum)
                        {
                            terrainCount = this.TerrainCount;
                            num5 = this.PainterTerrainA.Num;
                            terrainCount[num5]++;
                        }
                        num++;
                    }
                    int num19 = this.Painter.RoadBrushes[num2].Tile_TIntersection.TileCount - 1;
                    for (num = 0; num <= num19; num++)
                    {
                        this.PainterTexture = this.Painter.RoadBrushes[num2].Tile_TIntersection.Tiles[num];
                        if (this.PainterTexture.TextureNum == this.Texture.TextureNum)
                        {
                            terrainCount = this.TerrainCount;
                            num5 = this.PainterTerrainA.Num;
                            terrainCount[num5]++;
                        }
                    }
                }
            }
        }

        public class clsAutoSave
        {
            public int ChangeCount;
            public DateTime SavedDate = DateAndTime.Now;
        }

        public class clsAutoTextureChanges : clsMap.clsMapTileChanges
        {
            public clsAutoTextureChanges(clsMap Map) : base(Map, Map.Terrain.TileSize)
            {
            }

            public override void TileChanged(modMath.sXY_int Num)
            {
                this.Changed(Num);
            }
        }

        public class clsDrawCallTerrain : clsMap.clsAction
        {
            public override void ActionPerform()
            {
                GL.CallList(base.Map.Sectors[this.PosNum.X, this.PosNum.Y].GLList_Textured);
            }
        }

        public class clsDrawCallTerrainWireframe : clsMap.clsAction
        {
            public override void ActionPerform()
            {
                GL.CallList(base.Map.Sectors[this.PosNum.X, this.PosNum.Y].GLList_Wireframe);
            }
        }

        public class clsDrawHorizontalPosOnTerrain
        {
            public sRGBA_sng Colour;
            public modMath.sXY_int Horizontal;
            public clsMap Map;
            private modMath.sXYZ_int Vertex0;

            public void ActionPerform()
            {
                this.Vertex0.X = this.Horizontal.X;
                this.Vertex0.Y = (int) Math.Round(this.Map.GetTerrainHeight(this.Horizontal));
                this.Vertex0.Z = 0 - this.Horizontal.Y;
                GL.Begin(BeginMode.Lines);
                GL.Color4(this.Colour.Red, this.Colour.Green, this.Colour.Blue, this.Colour.Alpha);
                GL.Vertex3(this.Vertex0.X - 8, this.Vertex0.Y, 0 - this.Vertex0.Z);
                GL.Vertex3(this.Vertex0.X + 8, this.Vertex0.Y, 0 - this.Vertex0.Z);
                GL.Vertex3(this.Vertex0.X, this.Vertex0.Y, (0 - this.Vertex0.Z) - 8);
                GL.Vertex3(this.Vertex0.X, this.Vertex0.Y, (0 - this.Vertex0.Z) + 8);
                GL.End();
            }
        }

        public class clsDrawSectorObjects : clsMap.clsAction
        {
            private bool Started;
            private bool[] UnitDrawn;
            public clsTextLabels UnitTextLabels;

            public override void ActionPerform()
            {
                if (!this.Started)
                {
                    Debugger.Break();
                }
                else
                {
                    IEnumerator enumerator;
                    clsMap.clsSector sector = base.Map.Sectors[this.PosNum.X, this.PosNum.Y];
                    clsViewInfo viewInfo = base.Map.ViewInfo;
                    clsViewInfo.clsMouseOver.clsOverTerrain mouseOverTerrain = viewInfo.GetMouseOverTerrain();
                    try
                    {
                        enumerator = sector.Units.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            clsMap.clsUnitSectorConnection current = (clsMap.clsUnitSectorConnection) enumerator.Current;
                            clsMap.clsUnit unit = current.Unit;
                            if (!this.UnitDrawn[unit.MapLink.ArrayPosition])
                            {
                                Position.XYZ_dbl _dbl;
                                this.UnitDrawn[unit.MapLink.ArrayPosition] = true;
                                _dbl.X = unit.Pos.Horizontal.X - viewInfo.ViewPos.X;
                                _dbl.Y = unit.Pos.Altitude - viewInfo.ViewPos.Y;
                                _dbl.Z = (0 - unit.Pos.Horizontal.Y) - viewInfo.ViewPos.Z;
                                bool flag = false;
                                if (unit.Type.IsUnknown)
                                {
                                    flag = true;
                                }
                                else
                                {
                                    GL.PushMatrix();
                                    GL.Translate(_dbl.X, _dbl.Y, -_dbl.Z);
                                    unit.Type.GLDraw((float) unit.Rotation);
                                    GL.PopMatrix();
                                    if ((unit.Type.Type == clsUnitType.enumType.PlayerDroid) && ((clsDroidDesign) unit.Type).AlwaysDrawTextLabel)
                                    {
                                        flag = true;
                                    }
                                    if (((mouseOverTerrain != null) && (mouseOverTerrain.Units.Count > 0)) && (mouseOverTerrain.Units[0] == unit))
                                    {
                                        flag = true;
                                    }
                                }
                                if (flag & !this.UnitTextLabels.AtMaxCount())
                                {
                                    modMath.sXY_int _int;
                                    Position.XYZ_dbl _dbl2;
                                    Matrix3DMath.VectorRotationByMatrix(viewInfo.ViewAngleMatrix_Inverted, _dbl, ref _dbl2);
                                    if (viewInfo.Pos_Get_Screen_XY(_dbl2, ref _int) && ((((_int.X >= 0) & (_int.X <= viewInfo.MapView.GLSize.X)) & (_int.Y >= 0)) & (_int.Y <= viewInfo.MapView.GLSize.Y)))
                                    {
                                        clsTextLabel newItem = new clsTextLabel();
                                        clsTextLabel label2 = newItem;
                                        label2.TextFont = modProgram.UnitLabelFont;
                                        label2.SizeY = modSettings.Settings.FontSize;
                                        label2.Colour.Red = 1f;
                                        label2.Colour.Green = 1f;
                                        label2.Colour.Blue = 1f;
                                        label2.Colour.Alpha = 1f;
                                        label2.Pos.X = _int.X + 0x20;
                                        label2.Pos.Y = _int.Y;
                                        label2.Text = unit.Type.GetDisplayTextCode();
                                        label2 = null;
                                        this.UnitTextLabels.Add(newItem);
                                    }
                                }
                            }
                        }
                    }
                    finally
                    {
                        if (enumerator is IDisposable)
                        {
                            (enumerator as IDisposable).Dispose();
                        }
                    }
                }
            }

            public void Start()
            {
                this.UnitDrawn = new bool[(base.Map.Units.Count - 1) + 1];
                this.Started = true;
            }
        }

        public class clsDrawTerrainLine
        {
            public sRGBA_sng Colour;
            private modMath.sXY_int FinishTile;
            public modMath.sXY_int FinishXY;
            private modMath.sXY_int Horizontal;
            private modMath.sIntersectPos IntersectX;
            private modMath.sIntersectPos IntersectY;
            private int LastXTile;
            public clsMap Map;
            private modMath.sXY_int StartTile;
            public modMath.sXY_int StartXY;
            private modMath.sXY_int TileEdgeFinish;
            private modMath.sXY_int TileEdgeStart;
            private modMath.sXYZ_int Vertex;

            public void ActionPerform()
            {
                int num;
                GL.Begin(BeginMode.LineStrip);
                GL.Color4(this.Colour.Red, this.Colour.Green, this.Colour.Blue, this.Colour.Alpha);
                this.StartTile.Y = (int) Math.Round(((double) (((double) this.StartXY.Y) / 128.0)));
                this.FinishTile.Y = (int) Math.Round(((double) (((double) this.FinishXY.Y) / 128.0)));
                this.LastXTile = (int) Math.Round(((double) (((double) this.StartXY.X) / 128.0)));
                this.Horizontal = this.StartXY;
                this.Vertex.X = this.Horizontal.X;
                this.Vertex.Y = (int) Math.Round(this.Map.GetTerrainHeight(this.Horizontal));
                this.Vertex.Z = 0 - this.Horizontal.Y;
                GL.Vertex3(this.Vertex.X, this.Vertex.Y, 0 - this.Vertex.Z);
                if ((this.StartTile.Y + 1) <= this.FinishTile.Y)
                {
                    int y = this.FinishTile.Y;
                    for (int i = this.StartTile.Y + 1; i <= y; i++)
                    {
                        this.TileEdgeStart.X = 0;
                        this.TileEdgeStart.Y = i * 0x80;
                        this.TileEdgeFinish.X = this.Map.Terrain.TileSize.X * 0x80;
                        this.TileEdgeFinish.Y = i * 0x80;
                        this.IntersectY = modMath.GetLinesIntersectBetween(this.StartXY, this.FinishXY, this.TileEdgeStart, this.TileEdgeFinish);
                        if (this.IntersectY.Exists)
                        {
                            this.StartTile.X = this.LastXTile;
                            this.FinishTile.X = (int) Math.Round(((double) (((double) this.IntersectY.Pos.X) / 128.0)));
                            int x = this.FinishTile.X;
                            num = this.StartTile.X + 1;
                            while (num <= x)
                            {
                                this.TileEdgeStart.X = num * 0x80;
                                this.TileEdgeStart.Y = 0;
                                this.TileEdgeFinish.X = num * 0x80;
                                this.TileEdgeFinish.Y = this.Map.Terrain.TileSize.Y * 0x80;
                                this.IntersectX = modMath.GetLinesIntersectBetween(this.StartXY, this.FinishXY, this.TileEdgeStart, this.TileEdgeFinish);
                                if (this.IntersectX.Exists)
                                {
                                    this.Horizontal = this.IntersectX.Pos;
                                    this.Vertex.X = this.Horizontal.X;
                                    this.Vertex.Y = (int) Math.Round(this.Map.GetTerrainHeight(this.Horizontal));
                                    this.Vertex.Z = 0 - this.Horizontal.Y;
                                    GL.Vertex3(this.Vertex.X, this.Vertex.Y, 0 - this.Vertex.Z);
                                }
                                num++;
                            }
                            this.LastXTile = this.FinishTile.X;
                            this.Horizontal = this.IntersectY.Pos;
                            this.Vertex.X = this.Horizontal.X;
                            this.Vertex.Y = (int) Math.Round(this.Map.GetTerrainHeight(this.Horizontal));
                            this.Vertex.Z = 0 - this.Horizontal.Y;
                            GL.Vertex3(this.Vertex.X, this.Vertex.Y, 0 - this.Vertex.Z);
                        }
                    }
                }
                else
                {
                    this.StartTile.X = this.LastXTile;
                    this.FinishTile.X = (int) Math.Round(((double) (((double) this.FinishXY.X) / 128.0)));
                    int num5 = this.FinishTile.X;
                    for (num = this.StartTile.X + 1; num <= num5; num++)
                    {
                        this.TileEdgeStart.X = num * 0x80;
                        this.TileEdgeStart.Y = 0;
                        this.TileEdgeFinish.X = num * 0x80;
                        this.TileEdgeFinish.Y = this.Map.Terrain.TileSize.Y * 0x80;
                        this.IntersectX = modMath.GetLinesIntersectBetween(this.StartXY, this.FinishXY, this.TileEdgeStart, this.TileEdgeFinish);
                        if (this.IntersectX.Exists)
                        {
                            this.Horizontal = this.IntersectX.Pos;
                            this.Vertex.X = this.Horizontal.X;
                            this.Vertex.Y = (int) Math.Round(this.Map.GetTerrainHeight(this.Horizontal));
                            this.Vertex.Z = 0 - this.Horizontal.Y;
                            GL.Vertex3(this.Vertex.X, this.Vertex.Y, 0 - this.Vertex.Z);
                        }
                    }
                }
                this.Horizontal = this.FinishXY;
                this.Vertex.X = this.Horizontal.X;
                this.Vertex.Y = (int) Math.Round(this.Map.GetTerrainHeight(this.Horizontal));
                this.Vertex.Z = 0 - this.Horizontal.Y;
                GL.Vertex3(this.Vertex.X, this.Vertex.Y, 0 - this.Vertex.Z);
                GL.End();
            }
        }

        public abstract class clsDrawTile
        {
            public clsMap Map;
            public int TileX;
            public int TileY;

            protected clsDrawTile()
            {
            }

            public abstract void Perform();
        }

        public class clsDrawTileAreaOutline
        {
            public sRGBA_sng Colour;
            public modMath.sXY_int FinishXY;
            public clsMap Map;
            public modMath.sXY_int StartXY;
            private modMath.sXYZ_int Vertex0;
            private modMath.sXYZ_int Vertex1;

            public void ActionPerform()
            {
                int num;
                int num2;
                GL.Begin(BeginMode.Lines);
                GL.Color4(this.Colour.Red, this.Colour.Green, this.Colour.Blue, this.Colour.Alpha);
                int num3 = this.FinishXY.X - 1;
                for (num = this.StartXY.X; num <= num3; num++)
                {
                    this.Vertex0.X = num * 0x80;
                    this.Vertex0.Y = this.Map.Terrain.Vertices[num, this.StartXY.Y].Height * this.Map.HeightMultiplier;
                    this.Vertex0.Z = (0 - this.StartXY.Y) * 0x80;
                    this.Vertex1.X = (num + 1) * 0x80;
                    this.Vertex1.Y = this.Map.Terrain.Vertices[num + 1, this.StartXY.Y].Height * this.Map.HeightMultiplier;
                    this.Vertex1.Z = (0 - this.StartXY.Y) * 0x80;
                    GL.Vertex3(this.Vertex0.X, this.Vertex0.Y, 0 - this.Vertex0.Z);
                    GL.Vertex3(this.Vertex1.X, this.Vertex1.Y, 0 - this.Vertex1.Z);
                }
                int num4 = this.FinishXY.X - 1;
                for (num = this.StartXY.X; num <= num4; num++)
                {
                    this.Vertex0.X = num * 0x80;
                    this.Vertex0.Y = this.Map.Terrain.Vertices[num, this.FinishXY.Y].Height * this.Map.HeightMultiplier;
                    this.Vertex0.Z = (0 - this.FinishXY.Y) * 0x80;
                    this.Vertex1.X = (num + 1) * 0x80;
                    this.Vertex1.Y = this.Map.Terrain.Vertices[num + 1, this.FinishXY.Y].Height * this.Map.HeightMultiplier;
                    this.Vertex1.Z = (0 - this.FinishXY.Y) * 0x80;
                    GL.Vertex3(this.Vertex0.X, this.Vertex0.Y, 0 - this.Vertex0.Z);
                    GL.Vertex3(this.Vertex1.X, this.Vertex1.Y, 0 - this.Vertex1.Z);
                }
                int num5 = this.FinishXY.Y - 1;
                for (num2 = this.StartXY.Y; num2 <= num5; num2++)
                {
                    this.Vertex0.X = this.StartXY.X * 0x80;
                    this.Vertex0.Y = this.Map.Terrain.Vertices[this.StartXY.X, num2].Height * this.Map.HeightMultiplier;
                    this.Vertex0.Z = (0 - num2) * 0x80;
                    this.Vertex1.X = this.StartXY.X * 0x80;
                    this.Vertex1.Y = this.Map.Terrain.Vertices[this.StartXY.X, num2 + 1].Height * this.Map.HeightMultiplier;
                    this.Vertex1.Z = (0 - (num2 + 1)) * 0x80;
                    GL.Vertex3(this.Vertex0.X, this.Vertex0.Y, 0 - this.Vertex0.Z);
                    GL.Vertex3(this.Vertex1.X, this.Vertex1.Y, 0 - this.Vertex1.Z);
                }
                int num6 = this.FinishXY.Y - 1;
                for (num2 = this.StartXY.Y; num2 <= num6; num2++)
                {
                    this.Vertex0.X = this.FinishXY.X * 0x80;
                    this.Vertex0.Y = this.Map.Terrain.Vertices[this.FinishXY.X, num2].Height * this.Map.HeightMultiplier;
                    this.Vertex0.Z = (0 - num2) * 0x80;
                    this.Vertex1.X = this.FinishXY.X * 0x80;
                    this.Vertex1.Y = this.Map.Terrain.Vertices[this.FinishXY.X, num2 + 1].Height * this.Map.HeightMultiplier;
                    this.Vertex1.Z = (0 - (num2 + 1)) * 0x80;
                    GL.Vertex3(this.Vertex0.X, this.Vertex0.Y, 0 - this.Vertex0.Z);
                    GL.Vertex3(this.Vertex1.X, this.Vertex1.Y, 0 - this.Vertex1.Z);
                }
                GL.End();
            }
        }

        public class clsDrawTileOld : clsMap.clsDrawTile
        {
            public override void Perform()
            {
                modMath.sXY_sng _sng5;
                modMath.sXY_sng _sng6;
                modMath.sXY_sng _sng7;
                modMath.sXY_sng _sng8;
                modMath.sXYZ_sng _sng9;
                modMath.sXYZ_sng _sng10;
                modMath.sXYZ_sng _sng11;
                modMath.sXYZ_sng _sng12;
                clsMap.clsTerrain terrain = base.Map.Terrain;
                clsTileset tileset = base.Map.Tileset;
                double[] numArray = new double[4];
                if (terrain.Tiles[base.TileX, base.TileY].Texture.TextureNum < 0)
                {
                    GL.BindTexture(TextureTarget.Texture2D, modProgram.GLTexture_NoTile);
                }
                else if (tileset == null)
                {
                    GL.BindTexture(TextureTarget.Texture2D, modProgram.GLTexture_OverflowTile);
                }
                else if (terrain.Tiles[base.TileX, base.TileY].Texture.TextureNum < tileset.TileCount)
                {
                    int texture = tileset.Tiles[terrain.Tiles[base.TileX, base.TileY].Texture.TextureNum].MapView_GL_Texture_Num;
                    if (texture == 0)
                    {
                        GL.BindTexture(TextureTarget.Texture2D, modProgram.GLTexture_OverflowTile);
                    }
                    else
                    {
                        GL.BindTexture(TextureTarget.Texture2D, texture);
                    }
                }
                else
                {
                    GL.BindTexture(TextureTarget.Texture2D, modProgram.GLTexture_OverflowTile);
                }
                GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, 0x2100);
                numArray[0] = terrain.Vertices[base.TileX, base.TileY].Height;
                numArray[1] = terrain.Vertices[base.TileX + 1, base.TileY].Height;
                numArray[2] = terrain.Vertices[base.TileX, base.TileY + 1].Height;
                numArray[3] = terrain.Vertices[base.TileX + 1, base.TileY + 1].Height;
                TileOrientation.GetTileRotatedTexCoords(terrain.Tiles[base.TileX, base.TileY].Texture.Orientation, ref _sng5, ref _sng6, ref _sng7, ref _sng8);
                _sng9.X = base.TileX * 0x80;
                _sng9.Y = (float) (numArray[0] * base.Map.HeightMultiplier);
                _sng9.Z = (0 - base.TileY) * 0x80;
                _sng10.X = (base.TileX + 1) * 0x80;
                _sng10.Y = (float) (numArray[1] * base.Map.HeightMultiplier);
                _sng10.Z = (0 - base.TileY) * 0x80;
                _sng11.X = base.TileX * 0x80;
                _sng11.Y = (float) (numArray[2] * base.Map.HeightMultiplier);
                _sng11.Z = (0 - (base.TileY + 1)) * 0x80;
                _sng12.X = (base.TileX + 1) * 0x80;
                _sng12.Y = (float) (numArray[3] * base.Map.HeightMultiplier);
                _sng12.Z = (0 - (base.TileY + 1)) * 0x80;
                modMath.sXYZ_sng _sng = base.Map.TerrainVertexNormalCalc(base.TileX, base.TileY);
                modMath.sXYZ_sng _sng2 = base.Map.TerrainVertexNormalCalc(base.TileX + 1, base.TileY);
                modMath.sXYZ_sng _sng3 = base.Map.TerrainVertexNormalCalc(base.TileX, base.TileY + 1);
                modMath.sXYZ_sng _sng4 = base.Map.TerrainVertexNormalCalc(base.TileX + 1, base.TileY + 1);
                GL.Begin(BeginMode.Triangles);
                if (terrain.Tiles[base.TileX, base.TileY].Tri)
                {
                    GL.Normal3(_sng.X, _sng.Y, -_sng.Z);
                    GL.TexCoord2(_sng5.X, _sng5.Y);
                    GL.Vertex3(_sng9.X, _sng9.Y, -_sng9.Z);
                    GL.Normal3(_sng3.X, _sng3.Y, -_sng3.Z);
                    GL.TexCoord2(_sng7.X, _sng7.Y);
                    GL.Vertex3(_sng11.X, _sng11.Y, -_sng11.Z);
                    GL.Normal3(_sng2.X, _sng2.Y, -_sng2.Z);
                    GL.TexCoord2(_sng6.X, _sng6.Y);
                    GL.Vertex3(_sng10.X, _sng10.Y, -_sng10.Z);
                    GL.Normal3(_sng2.X, _sng2.Y, -_sng2.Z);
                    GL.TexCoord2(_sng6.X, _sng6.Y);
                    GL.Vertex3(_sng10.X, _sng10.Y, -_sng10.Z);
                    GL.Normal3(_sng3.X, _sng3.Y, -_sng3.Z);
                    GL.TexCoord2(_sng7.X, _sng7.Y);
                    GL.Vertex3(_sng11.X, _sng11.Y, -_sng11.Z);
                    GL.Normal3(_sng4.X, _sng4.Y, -_sng4.Z);
                    GL.TexCoord2(_sng8.X, _sng8.Y);
                    GL.Vertex3(_sng12.X, _sng12.Y, -_sng12.Z);
                }
                else
                {
                    GL.Normal3(_sng.X, _sng.Y, -_sng.Z);
                    GL.TexCoord2(_sng5.X, _sng5.Y);
                    GL.Vertex3(_sng9.X, _sng9.Y, -_sng9.Z);
                    GL.Normal3(_sng3.X, _sng3.Y, -_sng3.Z);
                    GL.TexCoord2(_sng7.X, _sng7.Y);
                    GL.Vertex3(_sng11.X, _sng11.Y, -_sng11.Z);
                    GL.Normal3(_sng4.X, _sng4.Y, -_sng4.Z);
                    GL.TexCoord2(_sng8.X, _sng8.Y);
                    GL.Vertex3(_sng12.X, _sng12.Y, -_sng12.Z);
                    GL.Normal3(_sng.X, _sng.Y, -_sng.Z);
                    GL.TexCoord2(_sng5.X, _sng5.Y);
                    GL.Vertex3(_sng9.X, _sng9.Y, -_sng9.Z);
                    GL.Normal3(_sng4.X, _sng4.Y, -_sng4.Z);
                    GL.TexCoord2(_sng8.X, _sng8.Y);
                    GL.Vertex3(_sng12.X, _sng12.Y, -_sng12.Z);
                    GL.Normal3(_sng2.X, _sng2.Y, -_sng2.Z);
                    GL.TexCoord2(_sng6.X, _sng6.Y);
                    GL.Vertex3(_sng10.X, _sng10.Y, -_sng10.Z);
                }
                GL.End();
            }
        }

        public class clsDrawTileOrientation : clsMap.clsAction
        {
            public override void ActionPerform()
            {
                int num3 = Math.Min((int) (((this.PosNum.Y + 1) * 8) - 1), (int) (base.Map.Terrain.TileSize.Y - 1));
                for (int i = this.PosNum.Y * 8; i <= num3; i++)
                {
                    int num4 = Math.Min((int) (((this.PosNum.X + 1) * 8) - 1), (int) (base.Map.Terrain.TileSize.X - 1));
                    for (int j = this.PosNum.X * 8; j <= num4; j++)
                    {
                        modMath.sXY_int tile = new modMath.sXY_int(j, i);
                        base.Map.DrawTileOrientation(tile);
                    }
                }
            }
        }

        public class clsDrawTileOutline : clsMap.clsAction
        {
            public sRGBA_sng Colour;
            private modMath.sXYZ_int Vertex0;
            private modMath.sXYZ_int Vertex1;
            private modMath.sXYZ_int Vertex2;
            private modMath.sXYZ_int Vertex3;

            public override void ActionPerform()
            {
                this.Vertex0.X = this.PosNum.X * 0x80;
                this.Vertex0.Y = base.Map.Terrain.Vertices[this.PosNum.X, this.PosNum.Y].Height * base.Map.HeightMultiplier;
                this.Vertex0.Z = (0 - this.PosNum.Y) * 0x80;
                this.Vertex1.X = (this.PosNum.X + 1) * 0x80;
                this.Vertex1.Y = base.Map.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y].Height * base.Map.HeightMultiplier;
                this.Vertex1.Z = (0 - this.PosNum.Y) * 0x80;
                this.Vertex2.X = this.PosNum.X * 0x80;
                this.Vertex2.Y = base.Map.Terrain.Vertices[this.PosNum.X, this.PosNum.Y + 1].Height * base.Map.HeightMultiplier;
                this.Vertex2.Z = (0 - (this.PosNum.Y + 1)) * 0x80;
                this.Vertex3.X = (this.PosNum.X + 1) * 0x80;
                this.Vertex3.Y = base.Map.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y + 1].Height * base.Map.HeightMultiplier;
                this.Vertex3.Z = (0 - (this.PosNum.Y + 1)) * 0x80;
                GL.Begin(BeginMode.LineLoop);
                GL.Color4(this.Colour.Red, this.Colour.Green, this.Colour.Blue, this.Colour.Alpha);
                GL.Vertex3(this.Vertex0.X, this.Vertex0.Y, 0 - this.Vertex0.Z);
                GL.Vertex3(this.Vertex1.X, this.Vertex1.Y, 0 - this.Vertex1.Z);
                GL.Vertex3(this.Vertex3.X, this.Vertex3.Y, 0 - this.Vertex3.Z);
                GL.Vertex3(this.Vertex2.X, this.Vertex2.Y, 0 - this.Vertex2.Z);
                GL.End();
            }
        }

        public class clsDrawVertexMarker : clsMap.clsAction
        {
            public sRGBA_sng Colour;
            private modMath.sXYZ_int Vertex0;

            public override void ActionPerform()
            {
                this.Vertex0.X = this.PosNum.X * 0x80;
                this.Vertex0.Y = base.Map.Terrain.Vertices[this.PosNum.X, this.PosNum.Y].Height * base.Map.HeightMultiplier;
                this.Vertex0.Z = (0 - this.PosNum.Y) * 0x80;
                GL.Begin(BeginMode.Lines);
                GL.Color4(this.Colour.Red, this.Colour.Green, this.Colour.Blue, this.Colour.Alpha);
                GL.Vertex3(this.Vertex0.X - 8, this.Vertex0.Y, 0 - this.Vertex0.Z);
                GL.Vertex3(this.Vertex0.X + 8, this.Vertex0.Y, 0 - this.Vertex0.Z);
                GL.Vertex3(this.Vertex0.X, this.Vertex0.Y, (0 - this.Vertex0.Z) - 8);
                GL.Vertex3(this.Vertex0.X, this.Vertex0.Y, (0 - this.Vertex0.Z) + 8);
                GL.End();
            }
        }

        public class clsDrawVertexTerrain : clsMap.clsAction
        {
            private sRGB_sng RGB_sng;
            private sRGB_sng RGB_sng2;
            private Matrix3D.Position.XYZ_dbl Vertex0;
            private Matrix3D.Position.XYZ_dbl Vertex1;
            private Matrix3D.Position.XYZ_dbl Vertex2;
            private Matrix3D.Position.XYZ_dbl Vertex3;
            public Matrix3DMath.Matrix3D ViewAngleMatrix;
            private Matrix3D.Position.XYZ_dbl XYZ_dbl;
            private Matrix3D.Position.XYZ_dbl XYZ_dbl2;
            private Matrix3D.Position.XYZ_dbl XYZ_dbl3;

            public override void ActionPerform()
            {
                int num4 = Math.Min(((this.PosNum.Y + 1) * 8) - 1, base.Map.Terrain.TileSize.Y);
                for (int i = this.PosNum.Y * 8; i <= num4; i++)
                {
                    int num5 = Math.Min(((this.PosNum.X + 1) * 8) - 1, base.Map.Terrain.TileSize.X);
                    for (int j = this.PosNum.X * 8; j <= num5; j++)
                    {
                        if (base.Map.Terrain.Vertices[j, i].Terrain != null)
                        {
                            int index = base.Map.Terrain.Vertices[j, i].Terrain.Num;
                            if ((index < base.Map.Painter.TerrainCount) && (base.Map.Painter.Terrains[index].Tiles.TileCount >= 1))
                            {
                                this.RGB_sng = base.Map.Tileset.Tiles[base.Map.Painter.Terrains[index].Tiles.Tiles[0].TextureNum].AverageColour;
                                if (((this.RGB_sng.Red + this.RGB_sng.Green) + this.RGB_sng.Blue) < 1.5f)
                                {
                                    this.RGB_sng2.Red = (this.RGB_sng.Red + 1f) / 2f;
                                    this.RGB_sng2.Green = (this.RGB_sng.Green + 1f) / 2f;
                                    this.RGB_sng2.Blue = (this.RGB_sng.Blue + 1f) / 2f;
                                }
                                else
                                {
                                    this.RGB_sng2.Red = this.RGB_sng.Red / 2f;
                                    this.RGB_sng2.Green = this.RGB_sng.Green / 2f;
                                    this.RGB_sng2.Blue = this.RGB_sng.Blue / 2f;
                                }
                                this.XYZ_dbl.X = j * 0x80;
                                this.XYZ_dbl.Y = base.Map.Terrain.Vertices[j, i].Height * base.Map.HeightMultiplier;
                                this.XYZ_dbl.Z = (0 - i) * 0x80;
                                this.XYZ_dbl2.X = 10.0;
                                this.XYZ_dbl2.Y = 10.0;
                                this.XYZ_dbl2.Z = 0.0;
                                Matrix3DMath.VectorRotationByMatrix(this.ViewAngleMatrix, this.XYZ_dbl2, ref this.XYZ_dbl3);
                                this.Vertex0.X = this.XYZ_dbl.X + this.XYZ_dbl3.X;
                                this.Vertex0.Y = this.XYZ_dbl.Y + this.XYZ_dbl3.Y;
                                this.Vertex0.Z = this.XYZ_dbl.Z + this.XYZ_dbl3.Z;
                                this.XYZ_dbl2.X = -10.0;
                                this.XYZ_dbl2.Y = 10.0;
                                this.XYZ_dbl2.Z = 0.0;
                                Matrix3DMath.VectorRotationByMatrix(this.ViewAngleMatrix, this.XYZ_dbl2, ref this.XYZ_dbl3);
                                this.Vertex1.X = this.XYZ_dbl.X + this.XYZ_dbl3.X;
                                this.Vertex1.Y = this.XYZ_dbl.Y + this.XYZ_dbl3.Y;
                                this.Vertex1.Z = this.XYZ_dbl.Z + this.XYZ_dbl3.Z;
                                this.XYZ_dbl2.X = -10.0;
                                this.XYZ_dbl2.Y = -10.0;
                                this.XYZ_dbl2.Z = 0.0;
                                Matrix3DMath.VectorRotationByMatrix(this.ViewAngleMatrix, this.XYZ_dbl2, ref this.XYZ_dbl3);
                                this.Vertex2.X = this.XYZ_dbl.X + this.XYZ_dbl3.X;
                                this.Vertex2.Y = this.XYZ_dbl.Y + this.XYZ_dbl3.Y;
                                this.Vertex2.Z = this.XYZ_dbl.Z + this.XYZ_dbl3.Z;
                                this.XYZ_dbl2.X = 10.0;
                                this.XYZ_dbl2.Y = -10.0;
                                this.XYZ_dbl2.Z = 0.0;
                                Matrix3DMath.VectorRotationByMatrix(this.ViewAngleMatrix, this.XYZ_dbl2, ref this.XYZ_dbl3);
                                this.Vertex3.X = this.XYZ_dbl.X + this.XYZ_dbl3.X;
                                this.Vertex3.Y = this.XYZ_dbl.Y + this.XYZ_dbl3.Y;
                                this.Vertex3.Z = this.XYZ_dbl.Z + this.XYZ_dbl3.Z;
                                GL.Begin(BeginMode.Quads);
                                GL.Color3(this.RGB_sng.Red, this.RGB_sng.Green, this.RGB_sng.Blue);
                                GL.Vertex3(this.Vertex0.X, this.Vertex0.Y, -this.Vertex0.Z);
                                GL.Vertex3(this.Vertex1.X, this.Vertex1.Y, -this.Vertex1.Z);
                                GL.Vertex3(this.Vertex2.X, this.Vertex2.Y, -this.Vertex2.Z);
                                GL.Vertex3(this.Vertex3.X, this.Vertex3.Y, -this.Vertex3.Z);
                                GL.End();
                                GL.Begin(BeginMode.LineLoop);
                                GL.Color3(this.RGB_sng2.Red, this.RGB_sng2.Green, this.RGB_sng2.Blue);
                                GL.Vertex3(this.Vertex0.X, this.Vertex0.Y, -this.Vertex0.Z);
                                GL.Vertex3(this.Vertex1.X, this.Vertex1.Y, -this.Vertex1.Z);
                                GL.Vertex3(this.Vertex2.X, this.Vertex2.Y, -this.Vertex2.Z);
                                GL.Vertex3(this.Vertex3.X, this.Vertex3.Y, -this.Vertex3.Z);
                                GL.End();
                            }
                        }
                    }
                }
            }
        }

        public class clsFMap_INIGateways : clsINIRead.clsSectionTranslator
        {
            public int GatewayCount;
            public sGateway[] Gateways;

            public clsFMap_INIGateways(int NewGatewayCount)
            {
                this.GatewayCount = NewGatewayCount;
                this.Gateways = new sGateway[(this.GatewayCount - 1) + 1];
                int num2 = this.GatewayCount - 1;
                for (int i = 0; i <= num2; i++)
                {
                    this.Gateways[i].PosA.X = -1;
                    this.Gateways[i].PosA.Y = -1;
                    this.Gateways[i].PosB.X = -1;
                    this.Gateways[i].PosB.Y = -1;
                }
            }

            public override clsINIRead.enumTranslatorResult Translate(int INISectionNum, clsINIRead.clsSection.sProperty INIProperty)
            {
                string name = INIProperty.Name;
                if (name == "ax")
                {
                    if (!modIO.InvariantParse_int(INIProperty.Value, ref this.Gateways[INISectionNum].PosA.X))
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                }
                else if (name == "ay")
                {
                    if (!modIO.InvariantParse_int(INIProperty.Value, ref this.Gateways[INISectionNum].PosA.Y))
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                }
                else if (name == "bx")
                {
                    if (!modIO.InvariantParse_int(INIProperty.Value, ref this.Gateways[INISectionNum].PosB.X))
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                }
                else if (name == "by")
                {
                    if (!modIO.InvariantParse_int(INIProperty.Value, ref this.Gateways[INISectionNum].PosB.Y))
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                }
                else
                {
                    return clsINIRead.enumTranslatorResult.NameUnknown;
                }
                return clsINIRead.enumTranslatorResult.Translated;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct sGateway
            {
                public modMath.sXY_int PosA;
                public modMath.sXY_int PosB;
            }
        }

        public class clsFMap_INIObjects : clsINIRead.clsSectionTranslator
        {
            public int ObjectCount;
            public sObject[] Objects;

            public clsFMap_INIObjects(int NewObjectCount)
            {
                this.ObjectCount = NewObjectCount;
                this.Objects = new sObject[(this.ObjectCount - 1) + 1];
                int num3 = this.ObjectCount - 1;
                for (int i = 0; i <= num3; i++)
                {
                    this.Objects[i].Type = clsUnitType.enumType.Unspecified;
                    this.Objects[i].Health = 1.0;
                    this.Objects[i].WallType = -1;
                    this.Objects[i].TurretCodes = new string[3];
                    this.Objects[i].TurretTypes = new clsTurret.enumTurretType[3];
                    int index = 0;
                    do
                    {
                        this.Objects[i].TurretTypes[index] = clsTurret.enumTurretType.Unknown;
                        index++;
                    }
                    while (index <= 2);
                }
            }

            public override clsINIRead.enumTranslatorResult Translate(int INISectionNum, clsINIRead.clsSection.sProperty INIProperty)
            {
                string name = INIProperty.Name;
                switch (name)
                {
                    case "type":
                    {
                        string[] strArray = INIProperty.Value.Split(new char[] { ',' });
                        int num2 = strArray.GetUpperBound(0) + 1;
                        if (num2 < 1)
                        {
                            return clsINIRead.enumTranslatorResult.ValueInvalid;
                        }
                        int num15 = num2 - 1;
                        for (int i = 0; i <= num15; i++)
                        {
                            strArray[i] = strArray[i].Trim();
                        }
                        string str2 = strArray[0].ToLower();
                        if (str2 != "feature")
                        {
                            if (str2 != "structure")
                            {
                                if (str2 != "droidtemplate")
                                {
                                    if (str2 != "droiddesign")
                                    {
                                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                                    }
                                    this.Objects[INISectionNum].Type = clsUnitType.enumType.PlayerDroid;
                                }
                                else
                                {
                                    this.Objects[INISectionNum].Type = clsUnitType.enumType.PlayerDroid;
                                    this.Objects[INISectionNum].IsTemplate = true;
                                    this.Objects[INISectionNum].Code = strArray[1];
                                }
                            }
                            else
                            {
                                this.Objects[INISectionNum].Type = clsUnitType.enumType.PlayerStructure;
                                this.Objects[INISectionNum].Code = strArray[1];
                            }
                        }
                        else
                        {
                            this.Objects[INISectionNum].Type = clsUnitType.enumType.Feature;
                            this.Objects[INISectionNum].Code = strArray[1];
                        }
                        goto Label_06CB;
                    }
                    case "droidtype":
                    {
                        clsDroidDesign.clsTemplateDroidType templateDroidTypeFromTemplateCode = modProgram.GetTemplateDroidTypeFromTemplateCode(INIProperty.Value);
                        if (templateDroidTypeFromTemplateCode == null)
                        {
                            return clsINIRead.enumTranslatorResult.ValueInvalid;
                        }
                        this.Objects[INISectionNum].TemplateDroidType = templateDroidTypeFromTemplateCode;
                        goto Label_06CB;
                    }
                    case "body":
                        this.Objects[INISectionNum].BodyCode = INIProperty.Value;
                        goto Label_06CB;

                    case "propulsion":
                        this.Objects[INISectionNum].PropulsionCode = INIProperty.Value;
                        goto Label_06CB;

                    case "turretcount":
                        int num3;
                        if (!modIO.InvariantParse_int(INIProperty.Value, ref num3))
                        {
                            return clsINIRead.enumTranslatorResult.ValueInvalid;
                        }
                        if ((num3 < 0) | (num3 > 3))
                        {
                            return clsINIRead.enumTranslatorResult.ValueInvalid;
                        }
                        this.Objects[INISectionNum].TurretCount = num3;
                        goto Label_06CB;

                    case "turret1":
                    {
                        string[] strArray2 = INIProperty.Value.Split(new char[] { ',' });
                        int num5 = strArray2.GetUpperBound(0) + 1;
                        if (num5 < 2)
                        {
                            return clsINIRead.enumTranslatorResult.ValueInvalid;
                        }
                        int num16 = num5 - 1;
                        for (int j = 0; j <= num16; j++)
                        {
                            strArray2[j] = strArray2[j].Trim();
                        }
                        clsTurret.enumTurretType turretTypeFromName = modProgram.GetTurretTypeFromName(strArray2[0]);
                        if (turretTypeFromName != clsTurret.enumTurretType.Unknown)
                        {
                            this.Objects[INISectionNum].TurretTypes[0] = turretTypeFromName;
                            this.Objects[INISectionNum].TurretCodes[0] = strArray2[1];
                        }
                        goto Label_06CB;
                    }
                    case "turret2":
                    {
                        string[] strArray3 = INIProperty.Value.Split(new char[] { ',' });
                        int num7 = strArray3.GetUpperBound(0) + 1;
                        if (num7 < 2)
                        {
                            return clsINIRead.enumTranslatorResult.ValueInvalid;
                        }
                        int num17 = num7 - 1;
                        for (int k = 0; k <= num17; k++)
                        {
                            strArray3[k] = strArray3[k].Trim();
                        }
                        clsTurret.enumTurretType type3 = modProgram.GetTurretTypeFromName(strArray3[0]);
                        if (type3 != clsTurret.enumTurretType.Unknown)
                        {
                            this.Objects[INISectionNum].TurretTypes[1] = type3;
                            this.Objects[INISectionNum].TurretCodes[1] = strArray3[1];
                        }
                        goto Label_06CB;
                    }
                    case "turret3":
                    {
                        string[] strArray4 = INIProperty.Value.Split(new char[] { ',' });
                        int num9 = strArray4.GetUpperBound(0) + 1;
                        if (num9 < 2)
                        {
                            return clsINIRead.enumTranslatorResult.ValueInvalid;
                        }
                        int num18 = num9 - 1;
                        for (int m = 0; m <= num18; m++)
                        {
                            strArray4[m] = strArray4[m].Trim();
                        }
                        clsTurret.enumTurretType type4 = modProgram.GetTurretTypeFromName(strArray4[0]);
                        if (type4 != clsTurret.enumTurretType.Unknown)
                        {
                            this.Objects[INISectionNum].TurretTypes[2] = type4;
                            this.Objects[INISectionNum].TurretCodes[2] = strArray4[1];
                        }
                        goto Label_06CB;
                    }
                    case "id":
                        if (!modIO.InvariantParse_uint(INIProperty.Value, ref this.Objects[INISectionNum].ID))
                        {
                            return clsINIRead.enumTranslatorResult.ValueInvalid;
                        }
                        goto Label_06CB;

                    case "priority":
                        if (!modIO.InvariantParse_int(INIProperty.Value, ref this.Objects[INISectionNum].Priority))
                        {
                            return clsINIRead.enumTranslatorResult.ValueInvalid;
                        }
                        goto Label_06CB;

                    case "pos":
                    {
                        modMath.sXY_int _int;
                        string[] strArray5 = INIProperty.Value.Split(new char[] { ',' });
                        int num11 = strArray5.GetUpperBound(0) + 1;
                        if (num11 < 2)
                        {
                            return clsINIRead.enumTranslatorResult.ValueInvalid;
                        }
                        int num19 = num11 - 1;
                        for (int n = 0; n <= num19; n++)
                        {
                            strArray5[n] = strArray5[n].Trim();
                        }
                        if (!modIO.InvariantParse_int(strArray5[0], ref _int.X))
                        {
                            return clsINIRead.enumTranslatorResult.ValueInvalid;
                        }
                        if (!modIO.InvariantParse_int(strArray5[1], ref _int.Y))
                        {
                            return clsINIRead.enumTranslatorResult.ValueInvalid;
                        }
                        try
                        {
                            this.Objects[INISectionNum].Pos = new modMath.clsXY_int(_int);
                            goto Label_06CB;
                        }
                        catch (Exception exception1)
                        {
                            ProjectData.SetProjectError(exception1);
                            Exception exception = exception1;
                            clsINIRead.enumTranslatorResult valueInvalid = clsINIRead.enumTranslatorResult.ValueInvalid;
                            ProjectData.ClearProjectError();
                            return valueInvalid;
                        }
                        break;
                    }
                    case "heading":
                        double num12;
                        if (!modIO.InvariantParse_dbl(INIProperty.Value, ref num12))
                        {
                            return clsINIRead.enumTranslatorResult.ValueInvalid;
                        }
                        if ((num12 < 0.0) | (num12 >= 360.0))
                        {
                            return clsINIRead.enumTranslatorResult.ValueInvalid;
                        }
                        this.Objects[INISectionNum].Heading = num12;
                        goto Label_06CB;

                    case "unitgroup":
                        this.Objects[INISectionNum].UnitGroup = INIProperty.Value;
                        goto Label_06CB;

                    case "health":
                        double num13;
                        if (!modIO.InvariantParse_dbl(INIProperty.Value, ref num13))
                        {
                            return clsINIRead.enumTranslatorResult.ValueInvalid;
                        }
                        if ((num13 < 0.0) | (num13 >= 1.0))
                        {
                            return clsINIRead.enumTranslatorResult.ValueInvalid;
                        }
                        this.Objects[INISectionNum].Health = num13;
                        goto Label_06CB;

                    case "walltype":
                    {
                        int result = -1;
                        if (!modIO.InvariantParse_int(INIProperty.Value, ref result))
                        {
                            return clsINIRead.enumTranslatorResult.ValueInvalid;
                        }
                        if ((result >= 0) & (result <= 3))
                        {
                            this.Objects[INISectionNum].WallType = result;
                        }
                        goto Label_06CB;
                    }
                    case "scriptlabel":
                        this.Objects[INISectionNum].Label = INIProperty.Value;
                        goto Label_06CB;
                }
                if (name == "scriptlabel")
                {
                    this.Objects[INISectionNum].Label = INIProperty.Value;
                }
                else
                {
                    return clsINIRead.enumTranslatorResult.NameUnknown;
                }
            Label_06CB:
                return clsINIRead.enumTranslatorResult.Translated;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct sObject
            {
                public uint ID;
                public clsUnitType.enumType Type;
                public bool IsTemplate;
                public string Code;
                public string UnitGroup;
                public bool GotAltitude;
                public modMath.clsXY_int Pos;
                public double Heading;
                public double Health;
                public clsDroidDesign.clsTemplateDroidType TemplateDroidType;
                public string BodyCode;
                public string PropulsionCode;
                public clsTurret.enumTurretType[] TurretTypes;
                public string[] TurretCodes;
                public int TurretCount;
                public int Priority;
                public string Label;
                public int WallType;
            }
        }

        public class clsFMapInfo : clsINIRead.clsTranslator
        {
            public clsMap.clsInterfaceOptions InterfaceOptions = new clsMap.clsInterfaceOptions();
            public modMath.sXY_int TerrainSize = new modMath.sXY_int(-1, -1);
            public clsTileset Tileset;

            public override clsINIRead.enumTranslatorResult Translate(clsINIRead.clsSection.sProperty INIProperty)
            {
                string name = INIProperty.Name;
                switch (name)
                {
                    case "tileset":
                    {
                        string str2 = INIProperty.Value.ToLower();
                        if (str2 != "arizona")
                        {
                            if (str2 != "urban")
                            {
                                if (str2 != "rockies")
                                {
                                    return clsINIRead.enumTranslatorResult.ValueInvalid;
                                }
                                this.Tileset = modProgram.Tileset_Rockies;
                            }
                            else
                            {
                                this.Tileset = modProgram.Tileset_Urban;
                            }
                        }
                        else
                        {
                            this.Tileset = modProgram.Tileset_Arizona;
                        }
                        break;
                    }
                    case "size":
                    {
                        modMath.sXY_int _int;
                        string[] strArray = INIProperty.Value.Split(new char[] { ',' });
                        if ((strArray.GetUpperBound(0) + 1) < 2)
                        {
                            return clsINIRead.enumTranslatorResult.ValueInvalid;
                        }
                        int upperBound = strArray.GetUpperBound(0);
                        for (int i = 0; i <= upperBound; i++)
                        {
                            strArray[i] = strArray[i].Trim();
                        }
                        if (!modIO.InvariantParse_int(strArray[0], ref _int.X))
                        {
                            return clsINIRead.enumTranslatorResult.ValueInvalid;
                        }
                        if (!modIO.InvariantParse_int(strArray[1], ref _int.Y))
                        {
                            return clsINIRead.enumTranslatorResult.ValueInvalid;
                        }
                        if ((((_int.X < 1) | (_int.Y < 1)) | (_int.X > 0x200)) | (_int.Y > 0x200))
                        {
                            return clsINIRead.enumTranslatorResult.ValueInvalid;
                        }
                        this.TerrainSize = _int;
                        break;
                    }
                    default:
                        if (name == "autoscrolllimits")
                        {
                            if (!modIO.InvariantParse_bool(INIProperty.Value, ref this.InterfaceOptions.AutoScrollLimits))
                            {
                                return clsINIRead.enumTranslatorResult.ValueInvalid;
                            }
                        }
                        else if (name == "scrollminx")
                        {
                            if (!modIO.InvariantParse_int(INIProperty.Value, ref this.InterfaceOptions.ScrollMin.X))
                            {
                                return clsINIRead.enumTranslatorResult.ValueInvalid;
                            }
                        }
                        else if (name == "scrollminy")
                        {
                            if (!modIO.InvariantParse_int(INIProperty.Value, ref this.InterfaceOptions.ScrollMin.Y))
                            {
                                return clsINIRead.enumTranslatorResult.ValueInvalid;
                            }
                        }
                        else if (name == "scrollmaxx")
                        {
                            if (!modIO.InvariantParse_uint(INIProperty.Value, ref this.InterfaceOptions.ScrollMax.X))
                            {
                                return clsINIRead.enumTranslatorResult.ValueInvalid;
                            }
                        }
                        else if (name == "scrollmaxy")
                        {
                            if (!modIO.InvariantParse_uint(INIProperty.Value, ref this.InterfaceOptions.ScrollMax.Y))
                            {
                                return clsINIRead.enumTranslatorResult.ValueInvalid;
                            }
                        }
                        else if (name == "name")
                        {
                            this.InterfaceOptions.CompileName = INIProperty.Value;
                        }
                        else if (name == "players")
                        {
                            this.InterfaceOptions.CompileMultiPlayers = INIProperty.Value;
                        }
                        else if (name == "xplayerlev")
                        {
                            if (!modIO.InvariantParse_bool(INIProperty.Value, ref this.InterfaceOptions.CompileMultiXPlayers))
                            {
                                return clsINIRead.enumTranslatorResult.ValueInvalid;
                            }
                        }
                        else if (name == "author")
                        {
                            this.InterfaceOptions.CompileMultiAuthor = INIProperty.Value;
                        }
                        else if (name == "license")
                        {
                            this.InterfaceOptions.CompileMultiLicense = INIProperty.Value;
                        }
                        else if (name != "camptime")
                        {
                            if (name != "camptype")
                            {
                                return clsINIRead.enumTranslatorResult.NameUnknown;
                            }
                            if (!modIO.InvariantParse_int(INIProperty.Value, ref this.InterfaceOptions.CampaignGameType))
                            {
                                return clsINIRead.enumTranslatorResult.ValueInvalid;
                            }
                        }
                        break;
                }
                return clsINIRead.enumTranslatorResult.Translated;
            }
        }

        public class clsGateway
        {
            public modLists.ConnectedListLink<clsMap.clsGateway, clsMap> MapLink;
            public modMath.sXY_int PosA;
            public modMath.sXY_int PosB;

            public clsGateway()
            {
                this.MapLink = new modLists.ConnectedListLink<clsMap.clsGateway, clsMap>(this);
            }

            public void Deallocate()
            {
                this.MapLink.Deallocate();
            }

            public bool IsOffMap()
            {
                modMath.sXY_int tileSize = this.MapLink.Source.Terrain.TileSize;
                return ((((((((this.PosA.X < 0) | (this.PosA.X >= tileSize.X)) | (this.PosA.Y < 0)) | (this.PosA.Y >= tileSize.Y)) | (this.PosB.X < 0)) | (this.PosB.X >= tileSize.X)) | (this.PosB.Y < 0)) | (this.PosB.Y >= tileSize.Y));
            }
        }

        public class clsGatewayChange
        {
            public clsMap.clsGateway Gateway;
            public enumType Type;

            public enum enumType : byte
            {
                Added = 0,
                Deleted = 1
            }
        }

        public class clsINIDroids : clsINIRead.clsSectionTranslator
        {
            public int DroidCount;
            public sDroid[] Droids;
            private clsMap ParentMap;

            public clsINIDroids(int NewDroidCount, clsMap NewParentMap)
            {
                this.ParentMap = NewParentMap;
                this.DroidCount = NewDroidCount;
                this.Droids = new sDroid[(this.DroidCount - 1) + 1];
                int num2 = this.DroidCount - 1;
                for (int i = 0; i <= num2; i++)
                {
                    this.Droids[i].HealthPercent = -1;
                    this.Droids[i].DroidType = -1;
                    this.Droids[i].Weapons = new string[3];
                }
            }

            public override clsINIRead.enumTranslatorResult Translate(int INISectionNum, clsINIRead.clsSection.sProperty INIProperty)
            {
                string name = INIProperty.Name;
                switch (name)
                {
                    case "id":
                        uint num;
                        if (!modIO.InvariantParse_uint(INIProperty.Value, ref num))
                        {
                            return clsINIRead.enumTranslatorResult.ValueInvalid;
                        }
                        if (num > 0L)
                        {
                            this.Droids[INISectionNum].ID = num;
                        }
                        break;

                    case "template":
                        this.Droids[INISectionNum].Template = INIProperty.Value;
                        break;

                    case "startpos":
                        int num2;
                        if (!modIO.InvariantParse_int(INIProperty.Value, ref num2))
                        {
                            return clsINIRead.enumTranslatorResult.ValueInvalid;
                        }
                        if ((num2 < 0) | (num2 >= 10))
                        {
                            return clsINIRead.enumTranslatorResult.ValueInvalid;
                        }
                        this.Droids[INISectionNum].UnitGroup = this.ParentMap.UnitGroups[num2];
                        break;

                    case "player":
                        if (INIProperty.Value.ToLower() != "scavenger")
                        {
                            return clsINIRead.enumTranslatorResult.ValueInvalid;
                        }
                        this.Droids[INISectionNum].UnitGroup = this.ParentMap.ScavengerUnitGroup;
                        break;

                    default:
                        if (name != "name")
                        {
                            if (name != "position")
                            {
                                if (name != "rotation")
                                {
                                    if (name != "health")
                                    {
                                        if (name != "droidtype")
                                        {
                                            if (name != "weapons")
                                            {
                                                if (name != @"parts\body")
                                                {
                                                    if (name != @"parts\propulsion")
                                                    {
                                                        if (name != @"parts\brain")
                                                        {
                                                            if (name != @"parts\repair")
                                                            {
                                                                if (name != @"parts\ecm")
                                                                {
                                                                    if (name != @"parts\sensor")
                                                                    {
                                                                        if (name != @"parts\construct")
                                                                        {
                                                                            if (name != @"parts\weapon\1")
                                                                            {
                                                                                if (name != @"parts\weapon\2")
                                                                                {
                                                                                    if (name != @"parts\weapon\3")
                                                                                    {
                                                                                        return clsINIRead.enumTranslatorResult.NameUnknown;
                                                                                    }
                                                                                    this.Droids[INISectionNum].Weapons[2] = INIProperty.Value;
                                                                                }
                                                                                else
                                                                                {
                                                                                    this.Droids[INISectionNum].Weapons[1] = INIProperty.Value;
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                this.Droids[INISectionNum].Weapons[0] = INIProperty.Value;
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            this.Droids[INISectionNum].Construct = INIProperty.Value;
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        this.Droids[INISectionNum].Sensor = INIProperty.Value;
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    this.Droids[INISectionNum].ECM = INIProperty.Value;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                this.Droids[INISectionNum].Repair = INIProperty.Value;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            this.Droids[INISectionNum].Brain = INIProperty.Value;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        this.Droids[INISectionNum].Propulsion = INIProperty.Value;
                                                    }
                                                }
                                                else
                                                {
                                                    this.Droids[INISectionNum].Body = INIProperty.Value;
                                                }
                                            }
                                            else if (!modIO.InvariantParse_int(INIProperty.Value, ref this.Droids[INISectionNum].WeaponCount))
                                            {
                                                return clsINIRead.enumTranslatorResult.ValueInvalid;
                                            }
                                        }
                                        else if (!modIO.InvariantParse_int(INIProperty.Value, ref this.Droids[INISectionNum].DroidType))
                                        {
                                            return clsINIRead.enumTranslatorResult.ValueInvalid;
                                        }
                                    }
                                    else if (!modIO.HealthFromINIText(INIProperty.Value, ref this.Droids[INISectionNum].HealthPercent))
                                    {
                                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                                    }
                                }
                                else if (!modIO.WZAngleFromINIText(INIProperty.Value, ref this.Droids[INISectionNum].Rotation))
                                {
                                    return clsINIRead.enumTranslatorResult.ValueInvalid;
                                }
                            }
                            else if (!modIO.WorldPosFromINIText(INIProperty.Value, ref this.Droids[INISectionNum].Pos))
                            {
                                return clsINIRead.enumTranslatorResult.ValueInvalid;
                            }
                        }
                        break;
                }
                return clsINIRead.enumTranslatorResult.Translated;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct sDroid
            {
                public uint ID;
                public string Template;
                public clsMap.clsUnitGroup UnitGroup;
                public modProgram.clsWorldPos Pos;
                public modProgram.sWZAngle Rotation;
                public int HealthPercent;
                public int DroidType;
                public string Body;
                public string Propulsion;
                public string Brain;
                public string Repair;
                public string ECM;
                public string Sensor;
                public string Construct;
                public string[] Weapons;
                public int WeaponCount;
            }
        }

        public class clsINIFeatures : clsINIRead.clsSectionTranslator
        {
            public int FeatureCount;
            public sFeatures[] Features;

            public clsINIFeatures(int NewFeatureCount)
            {
                this.FeatureCount = NewFeatureCount;
                this.Features = new sFeatures[(this.FeatureCount - 1) + 1];
                int num2 = this.FeatureCount - 1;
                for (int i = 0; i <= num2; i++)
                {
                    this.Features[i].HealthPercent = -1;
                }
            }

            public override clsINIRead.enumTranslatorResult Translate(int INISectionNum, clsINIRead.clsSection.sProperty INIProperty)
            {
                string name = INIProperty.Name;
                if (name == "id")
                {
                    uint num;
                    if (!modIO.InvariantParse_uint(INIProperty.Value, ref num))
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                    if (num > 0L)
                    {
                        this.Features[INISectionNum].ID = num;
                    }
                }
                else if (name == "name")
                {
                    this.Features[INISectionNum].Code = INIProperty.Value;
                }
                else if (name == "position")
                {
                    if (!modIO.WorldPosFromINIText(INIProperty.Value, ref this.Features[INISectionNum].Pos))
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                }
                else if (name == "rotation")
                {
                    if (!modIO.WZAngleFromINIText(INIProperty.Value, ref this.Features[INISectionNum].Rotation))
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                }
                else if (name == "health")
                {
                    if (!modIO.HealthFromINIText(INIProperty.Value, ref this.Features[INISectionNum].HealthPercent))
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                }
                else
                {
                    return clsINIRead.enumTranslatorResult.NameUnknown;
                }
                return clsINIRead.enumTranslatorResult.Translated;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct sFeatures
            {
                public uint ID;
                public string Code;
                public modProgram.clsWorldPos Pos;
                public modProgram.sWZAngle Rotation;
                public int HealthPercent;
            }
        }

        public class clsINIStructures : clsINIRead.clsSectionTranslator
        {
            private clsMap ParentMap;
            public int StructureCount;
            public sStructure[] Structures;

            public clsINIStructures(int NewStructureCount, clsMap NewParentMap)
            {
                this.ParentMap = NewParentMap;
                this.StructureCount = NewStructureCount;
                this.Structures = new sStructure[(this.StructureCount - 1) + 1];
                int num2 = this.StructureCount - 1;
                for (int i = 0; i <= num2; i++)
                {
                    this.Structures[i].HealthPercent = -1;
                    this.Structures[i].WallType = -1;
                }
            }

            public override clsINIRead.enumTranslatorResult Translate(int INISectionNum, clsINIRead.clsSection.sProperty INIProperty)
            {
                string name = INIProperty.Name;
                switch (name)
                {
                    case "id":
                        uint num;
                        if (!modIO.InvariantParse_uint(INIProperty.Value, ref num))
                        {
                            return clsINIRead.enumTranslatorResult.ValueInvalid;
                        }
                        if (num > 0L)
                        {
                            this.Structures[INISectionNum].ID = num;
                        }
                        break;

                    case "name":
                        this.Structures[INISectionNum].Code = INIProperty.Value;
                        break;

                    case "startpos":
                        int num2;
                        if (!modIO.InvariantParse_int(INIProperty.Value, ref num2))
                        {
                            return clsINIRead.enumTranslatorResult.ValueInvalid;
                        }
                        if ((num2 < 0) | (num2 >= 10))
                        {
                            return clsINIRead.enumTranslatorResult.ValueInvalid;
                        }
                        this.Structures[INISectionNum].UnitGroup = this.ParentMap.UnitGroups[num2];
                        break;

                    case "player":
                        if (INIProperty.Value.ToLower() != "scavenger")
                        {
                            return clsINIRead.enumTranslatorResult.ValueInvalid;
                        }
                        this.Structures[INISectionNum].UnitGroup = this.ParentMap.ScavengerUnitGroup;
                        break;

                    case "position":
                        if (!modIO.WorldPosFromINIText(INIProperty.Value, ref this.Structures[INISectionNum].Pos))
                        {
                            return clsINIRead.enumTranslatorResult.ValueInvalid;
                        }
                        break;

                    case "rotation":
                        if (!modIO.WZAngleFromINIText(INIProperty.Value, ref this.Structures[INISectionNum].Rotation))
                        {
                            return clsINIRead.enumTranslatorResult.ValueInvalid;
                        }
                        break;

                    case "modules":
                        int num3;
                        if (!modIO.InvariantParse_int(INIProperty.Value, ref num3))
                        {
                            return clsINIRead.enumTranslatorResult.ValueInvalid;
                        }
                        if (num3 < 0)
                        {
                            return clsINIRead.enumTranslatorResult.ValueInvalid;
                        }
                        this.Structures[INISectionNum].ModuleCount = num3;
                        break;

                    default:
                        if (name == "health")
                        {
                            if (!modIO.HealthFromINIText(INIProperty.Value, ref this.Structures[INISectionNum].HealthPercent))
                            {
                                return clsINIRead.enumTranslatorResult.ValueInvalid;
                            }
                        }
                        else
                        {
                            int num4;
                            if (name != "wall/type")
                            {
                                return clsINIRead.enumTranslatorResult.NameUnknown;
                            }
                            if (!modIO.InvariantParse_int(INIProperty.Value, ref num4))
                            {
                                return clsINIRead.enumTranslatorResult.ValueInvalid;
                            }
                            if (num4 < 0)
                            {
                                return clsINIRead.enumTranslatorResult.ValueInvalid;
                            }
                            this.Structures[INISectionNum].WallType = num4;
                        }
                        break;
                }
                return clsINIRead.enumTranslatorResult.Translated;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct sStructure
            {
                public uint ID;
                public string Code;
                public clsMap.clsUnitGroup UnitGroup;
                public modProgram.clsWorldPos Pos;
                public modProgram.sWZAngle Rotation;
                public int ModuleCount;
                public int HealthPercent;
                public int WallType;
            }
        }

        public class clsInterfaceOptions
        {
            public bool AutoScrollLimits = true;
            public int CampaignGameType;
            public string CompileMultiAuthor = "";
            public string CompileMultiLicense = "";
            public string CompileMultiPlayers = modIO.InvariantToString_int(2);
            public bool CompileMultiXPlayers = false;
            public string CompileName = "";
            public modMath.sXY_uint ScrollMax;
            public modMath.sXY_int ScrollMin;

            public clsInterfaceOptions()
            {
                this.ScrollMin.X = 0;
                this.ScrollMin.Y = 0;
                this.ScrollMax.X = 0;
                this.ScrollMax.Y = 0;
                this.CampaignGameType = -1;
            }
        }

        public class clsLNDObject
        {
            public string Code;
            public uint ID;
            public string Name;
            public int PlayerNum;
            public modMath.sXYZ_sng Pos;
            public modMath.sXYZ_int Rotation;
            public int TypeNum;
        }

        public abstract class clsMapTileChanges : clsMap.clsPointChanges
        {
            public clsMap Map;
            public clsMap.clsTerrain Terrain;

            public clsMapTileChanges(clsMap Map, modMath.sXY_int PointSize) : base(PointSize)
            {
                this.Map = Map;
                this.Terrain = Map.Terrain;
            }

            public void Deallocate()
            {
                this.Map = null;
            }

            public void SideHChanged(modMath.sXY_int Num)
            {
                if (Num.Y > 0)
                {
                    modMath.sXY_int num = new modMath.sXY_int(Num.X, Num.Y - 1);
                    this.TileChanged(num);
                }
                if (Num.Y < this.Map.Terrain.TileSize.Y)
                {
                    this.TileChanged(Num);
                }
            }

            public void SideVChanged(modMath.sXY_int Num)
            {
                if (Num.X > 0)
                {
                    modMath.sXY_int num = new modMath.sXY_int(Num.X - 1, Num.Y);
                    this.TileChanged(num);
                }
                if (Num.X < this.Map.Terrain.TileSize.X)
                {
                    this.TileChanged(Num);
                }
            }

            public abstract void TileChanged(modMath.sXY_int Num);
            public void VertexAndNormalsChanged(modMath.sXY_int Num)
            {
                modMath.sXY_int _int;
                if (Num.X > 1)
                {
                    if (Num.Y > 0)
                    {
                        _int = new modMath.sXY_int(Num.X - 2, Num.Y - 1);
                        this.TileChanged(_int);
                    }
                    if (Num.Y < this.Terrain.TileSize.Y)
                    {
                        _int = new modMath.sXY_int(Num.X - 2, Num.Y);
                        this.TileChanged(_int);
                    }
                }
                if (Num.X > 0)
                {
                    if (Num.Y > 1)
                    {
                        _int = new modMath.sXY_int(Num.X - 1, Num.Y - 2);
                        this.TileChanged(_int);
                    }
                    if (Num.Y > 0)
                    {
                        _int = new modMath.sXY_int(Num.X - 1, Num.Y - 1);
                        this.TileChanged(_int);
                    }
                    if (Num.Y < this.Terrain.TileSize.Y)
                    {
                        _int = new modMath.sXY_int(Num.X - 1, Num.Y);
                        this.TileChanged(_int);
                    }
                    if (Num.Y < (this.Terrain.TileSize.Y - 1))
                    {
                        _int = new modMath.sXY_int(Num.X - 1, Num.Y + 1);
                        this.TileChanged(_int);
                    }
                }
                if (Num.X < this.Terrain.TileSize.X)
                {
                    if (Num.Y > 1)
                    {
                        _int = new modMath.sXY_int(Num.X, Num.Y - 2);
                        this.TileChanged(_int);
                    }
                    if (Num.Y > 0)
                    {
                        _int = new modMath.sXY_int(Num.X, Num.Y - 1);
                        this.TileChanged(_int);
                    }
                    if (Num.Y < this.Terrain.TileSize.Y)
                    {
                        this.TileChanged(Num);
                    }
                    if (Num.Y < (this.Terrain.TileSize.Y - 1))
                    {
                        _int = new modMath.sXY_int(Num.X, Num.Y + 1);
                        this.TileChanged(_int);
                    }
                }
                if (Num.X < (this.Terrain.TileSize.X - 1))
                {
                    if (Num.Y > 0)
                    {
                        _int = new modMath.sXY_int(Num.X + 1, Num.Y - 1);
                        this.TileChanged(_int);
                    }
                    if (Num.Y < this.Terrain.TileSize.Y)
                    {
                        _int = new modMath.sXY_int(Num.X + 1, Num.Y);
                        this.TileChanged(_int);
                    }
                }
            }

            public void VertexChanged(modMath.sXY_int Num)
            {
                modMath.sXY_int _int;
                if (Num.X > 0)
                {
                    if (Num.Y > 0)
                    {
                        _int = new modMath.sXY_int(Num.X - 1, Num.Y - 1);
                        this.TileChanged(_int);
                    }
                    if (Num.Y < this.Terrain.TileSize.Y)
                    {
                        _int = new modMath.sXY_int(Num.X - 1, Num.Y);
                        this.TileChanged(_int);
                    }
                }
                if (Num.X < this.Terrain.TileSize.X)
                {
                    if (Num.Y > 0)
                    {
                        _int = new modMath.sXY_int(Num.X, Num.Y - 1);
                        this.TileChanged(_int);
                    }
                    if (Num.Y < this.Terrain.TileSize.Y)
                    {
                        this.TileChanged(Num);
                    }
                }
            }
        }

        public class clsMessage
        {
            private DateTime _CreatedDate = DateAndTime.Now;
            public string Text;

            public DateTime CreatedDate
            {
                get
                {
                    return this._CreatedDate;
                }
            }
        }

        public class clsMinimapTexture
        {
            public sRGBA_sng[] InlinePixels;
            public modMath.sXY_int Size;

            public clsMinimapTexture(modMath.sXY_int Size)
            {
                this.Size = Size;
                this.InlinePixels = new sRGBA_sng[((Size.X * Size.Y) - 1) + 1];
            }

            public sRGBA_sng this[int X, int Y]
            {
                get
                {
                    return this.InlinePixels[(Y * this.Size.X) + X];
                }
                set
                {
                    this.InlinePixels[(Y * this.Size.X) + X] = value;
                }
            }
        }

        public abstract class clsObjectAction : modLists.SimpleListTool<clsMap.clsUnit>
        {
            private modLists.SimpleClassList<clsMap.clsUnit> _ResultUnits = new modLists.SimpleClassList<clsMap.clsUnit>();
            public bool ActionPerformed;
            public clsMap Map;
            protected clsMap.clsUnit ResultUnit;
            public clsMap.clsUnit Unit;

            protected clsObjectAction()
            {
            }

            protected abstract void _ActionPerform();
            protected virtual void ActionCondition()
            {
            }

            public void ActionPerform()
            {
                this.ResultUnit = null;
                this.ActionPerformed = false;
                if (this.Unit == null)
                {
                    Debugger.Break();
                }
                else
                {
                    this.ActionPerformed = true;
                    this.ActionCondition();
                    if (this.ActionPerformed)
                    {
                        this.ResultUnit = new clsMap.clsUnit(this.Unit, this.Map);
                        this._ActionPerform();
                        if (this.ResultUnit == null)
                        {
                            this.ResultUnit = this.Unit;
                        }
                        else
                        {
                            this._ResultUnits.Add(this.ResultUnit);
                            this.Map.UnitSwap(this.Unit, this.ResultUnit);
                        }
                    }
                }
            }

            public void SetItem(clsMap.clsUnit Item)
            {
                this.Unit = Item;
            }

            public modLists.SimpleClassList<clsMap.clsUnit> ResultUnits
            {
                get
                {
                    return this._ResultUnits;
                }
            }
        }

        public class clsObjectAlignment : clsMap.clsObjectAction
        {
            protected override void _ActionPerform()
            {
                base.ResultUnit.Pos = base.Unit.MapLink.Source.TileAlignedPosFromMapPos(base.Unit.Pos.Horizontal, base.Unit.Type.get_GetFootprintNew(base.Unit.Rotation));
            }
        }

        public class clsObjectBody : clsMap.clsObjectComponent
        {
            public clsBody Body;

            protected override void ChangeComponent()
            {
                base.NewDroidType.Body = this.Body;
            }
        }

        public abstract class clsObjectComponent : clsMap.clsObjectAction
        {
            protected clsDroidDesign NewDroidType;
            private clsDroidDesign OldDroidType;

            protected clsObjectComponent()
            {
            }

            protected override void _ActionPerform()
            {
                this.NewDroidType = new clsDroidDesign();
                base.ResultUnit.Type = this.NewDroidType;
                this.NewDroidType.CopyDesign(this.OldDroidType);
                this.ChangeComponent();
                this.NewDroidType.UpdateAttachments();
            }

            protected override void ActionCondition()
            {
                base.ActionCondition();
                if (base.Unit.Type.Type == clsUnitType.enumType.PlayerDroid)
                {
                    this.OldDroidType = (clsDroidDesign) base.Unit.Type;
                    base.ActionPerformed = !this.OldDroidType.IsTemplate;
                }
                else
                {
                    this.OldDroidType = null;
                    base.ActionPerformed = false;
                }
            }

            protected abstract void ChangeComponent();
        }

        public class clsObjectDroidType : clsMap.clsObjectComponent
        {
            public clsDroidDesign.clsTemplateDroidType DroidType;

            protected override void ChangeComponent()
            {
                base.NewDroidType.TemplateDroidType = this.DroidType;
            }
        }

        public class clsObjectFlattenTerrain : modLists.SimpleListTool<clsMap.clsUnit>
        {
            private clsMap.clsUnit Unit;

            public void ActionPerform()
            {
                modMath.sXY_int _int;
                int num2;
                modMath.sXY_int _int3;
                double num3;
                modMath.sXY_int _int4;
                int x;
                int num5;
                clsMap source = this.Unit.MapLink.Source;
                modMath.sXY_int footprint = this.Unit.Type.get_GetFootprintSelected(this.Unit.Rotation);
                source.GetFootprintTileRangeClamped(this.Unit.Pos.Horizontal, footprint, ref _int3, ref _int);
                int num6 = _int.Y + 1;
                for (num5 = _int3.Y; num5 <= num6; num5++)
                {
                    _int4.Y = num5;
                    int num7 = _int.X + 1;
                    x = _int3.X;
                    while (x <= num7)
                    {
                        _int4.X = x;
                        num3 += source.Terrain.Vertices[_int4.X, _int4.Y].Height;
                        num2++;
                        x++;
                    }
                }
                if (num2 >= 1)
                {
                    byte num = (byte) modMath.Clamp_int((int) Math.Round((double) (num3 / ((double) num2))), 0, 0xff);
                    int num8 = _int.Y + 1;
                    for (num5 = _int3.Y; num5 <= num8; num5++)
                    {
                        _int4.Y = num5;
                        int num9 = _int.X + 1;
                        for (x = _int3.X; x <= num9; x++)
                        {
                            _int4.X = x;
                            source.Terrain.Vertices[_int4.X, _int4.Y].Height = num;
                            source.SectorGraphicsChanges.VertexAndNormalsChanged(_int4);
                            source.SectorUnitHeightsChanges.VertexChanged(_int4);
                            source.SectorTerrainUndoChanges.VertexChanged(_int4);
                        }
                    }
                }
            }

            public void SetItem(clsMap.clsUnit Item)
            {
                this.Unit = Item;
            }
        }

        public class clsObjectHealth : clsMap.clsObjectAction
        {
            public double Health;

            protected override void _ActionPerform()
            {
                base.ResultUnit.Health = this.Health;
            }
        }

        public class clsObjectPosOffset : clsMap.clsObjectAction
        {
            private modMath.sXY_int NewPos;
            public modMath.sXY_int Offset;

            protected override void _ActionPerform()
            {
                this.NewPos.X = base.Unit.Pos.Horizontal.X + this.Offset.X;
                this.NewPos.Y = base.Unit.Pos.Horizontal.Y + this.Offset.Y;
                base.ResultUnit.Pos = base.Map.TileAlignedPosFromMapPos(this.NewPos, base.ResultUnit.Type.get_GetFootprintSelected(base.Unit.Rotation));
            }
        }

        public class clsObjectPriority : clsMap.clsObjectAction
        {
            public int Priority;

            protected override void _ActionPerform()
            {
                base.ResultUnit.SavePriority = this.Priority;
            }
        }

        public class clsObjectPriorityOrderList : modLists.SimpleListTool<clsMap.clsUnit>
        {
            private modLists.SimpleClassList<clsMap.clsUnit> _Result = new modLists.SimpleClassList<clsMap.clsUnit>();
            private clsMap.clsUnit Unit;

            public clsObjectPriorityOrderList()
            {
                this._Result.MaintainOrder = true;
            }

            public void ActionPerform()
            {
                int num2 = this._Result.Count - 1;
                int position = 0;
                while (position <= num2)
                {
                    if (this.Unit.SavePriority > this._Result[position].SavePriority)
                    {
                        break;
                    }
                    position++;
                }
                this._Result.Insert(this.Unit, position);
            }

            public void SetItem(clsMap.clsUnit Item)
            {
                this.Unit = Item;
            }

            public modLists.SimpleClassList<clsMap.clsUnit> Result
            {
                get
                {
                    return this._Result;
                }
            }
        }

        public class clsObjectPropulsion : clsMap.clsObjectComponent
        {
            public clsPropulsion Propulsion;

            protected override void ChangeComponent()
            {
                base.NewDroidType.Propulsion = this.Propulsion;
            }
        }

        public class clsObjectRotation : clsMap.clsObjectAction
        {
            public int Angle;

            protected override void _ActionPerform()
            {
                base.ResultUnit.Rotation = this.Angle;
            }
        }

        public class clsObjectRotationOffset : clsMap.clsObjectAction
        {
            private modMath.sXY_int NewPos;
            public int Offset;

            protected override void _ActionPerform()
            {
                clsMap.clsUnit resultUnit;
                base.ResultUnit.Rotation = base.Unit.Rotation + this.Offset;
                if (base.ResultUnit.Rotation < 0)
                {
                    resultUnit = base.ResultUnit;
                    resultUnit.Rotation += 360;
                }
                else if (base.ResultUnit.Rotation >= 360)
                {
                    resultUnit = base.ResultUnit;
                    resultUnit.Rotation -= 360;
                }
            }
        }

        public class clsObjectSelect : modLists.SimpleListTool<clsMap.clsUnit>
        {
            private clsMap.clsUnit Unit;

            public void ActionPerform()
            {
                this.Unit.MapSelect();
            }

            public void SetItem(clsMap.clsUnit Item)
            {
                this.Unit = Item;
            }
        }

        public class clsObjectTemplateToDesign : clsMap.clsObjectAction
        {
            private clsDroidDesign NewDroidType;
            private clsDroidDesign OldDroidType;

            protected override void _ActionPerform()
            {
                this.NewDroidType = new clsDroidDesign();
                base.ResultUnit.Type = this.NewDroidType;
                this.NewDroidType.CopyDesign(this.OldDroidType);
                this.NewDroidType.UpdateAttachments();
            }

            protected override void ActionCondition()
            {
                base.ActionCondition();
                if (base.Unit.Type.Type == clsUnitType.enumType.PlayerDroid)
                {
                    this.OldDroidType = (clsDroidDesign) base.Unit.Type;
                    base.ActionPerformed = this.OldDroidType.IsTemplate;
                }
                else
                {
                    this.OldDroidType = null;
                    base.ActionPerformed = false;
                }
            }
        }

        public class clsObjectTurret : clsMap.clsObjectComponent
        {
            public clsTurret Turret;
            public int TurretNum;

            protected override void ChangeComponent()
            {
                switch (this.TurretNum)
                {
                    case 0:
                        base.NewDroidType.Turret1 = this.Turret;
                        break;

                    case 1:
                        base.NewDroidType.Turret2 = this.Turret;
                        break;

                    case 2:
                        base.NewDroidType.Turret3 = this.Turret;
                        break;
                }
            }
        }

        public class clsObjectTurretCount : clsMap.clsObjectComponent
        {
            public byte TurretCount;

            protected override void ChangeComponent()
            {
                base.NewDroidType.TurretCount = this.TurretCount;
            }
        }

        public class clsObjectUnitGroup : clsMap.clsObjectAction
        {
            public clsMap.clsUnitGroup UnitGroup;

            protected override void _ActionPerform()
            {
                base.ResultUnit.UnitGroup = this.UnitGroup;
            }
        }

        public class clsPathInfo
        {
            private bool _IsFMap;
            private string _Path;

            public clsPathInfo(string Path, bool IsFMap)
            {
                this._Path = Path;
                this._IsFMap = IsFMap;
            }

            public bool IsFMap
            {
                get
                {
                    return this._IsFMap;
                }
            }

            public string Path
            {
                get
                {
                    return this._Path;
                }
            }
        }

        public class clsPointChanges
        {
            public modLists.SimpleList<modMath.clsXY_int> ChangedPoints = new modLists.SimpleList<modMath.clsXY_int>();
            public bool[,] PointIsChanged;

            public clsPointChanges(modMath.sXY_int PointSize)
            {
                this.PointIsChanged = new bool[(PointSize.X - 1) + 1, (PointSize.Y - 1) + 1];
                this.ChangedPoints.MinSize = PointSize.X * PointSize.Y;
                this.ChangedPoints.Clear();
            }

            public void Changed(modMath.sXY_int Num)
            {
                if (!this.PointIsChanged[Num.X, Num.Y])
                {
                    this.PointIsChanged[Num.X, Num.Y] = true;
                    this.ChangedPoints.Add(new modMath.clsXY_int(Num));
                }
            }

            public void Clear()
            {
                IEnumerator enumerator;
                try
                {
                    enumerator = this.ChangedPoints.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        modMath.clsXY_int current = (modMath.clsXY_int) enumerator.Current;
                        this.PointIsChanged[current.X, current.Y] = false;
                    }
                }
                finally
                {
                    if (enumerator is IDisposable)
                    {
                        (enumerator as IDisposable).Dispose();
                    }
                }
                this.ChangedPoints.Clear();
            }

            public void PerformTool(clsMap.clsAction Tool)
            {
                IEnumerator enumerator;
                try
                {
                    enumerator = this.ChangedPoints.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        modMath.clsXY_int current = (modMath.clsXY_int) enumerator.Current;
                        Tool.PosNum = current.XY;
                        Tool.ActionPerform();
                    }
                }
                finally
                {
                    if (enumerator is IDisposable)
                    {
                        (enumerator as IDisposable).Dispose();
                    }
                }
            }

            public void SetAllChanged()
            {
                int upperBound = this.PointIsChanged.GetUpperBound(1);
                for (int i = 0; i <= upperBound; i++)
                {
                    modMath.sXY_int _int;
                    _int.Y = i;
                    int num4 = this.PointIsChanged.GetUpperBound(0);
                    for (int j = 0; j <= num4; j++)
                    {
                        _int.X = j;
                        this.Changed(_int);
                    }
                }
            }
        }

        public class clsScriptArea
        {
            private string _Label;
            private modLists.ConnectedListLink<clsMap.clsScriptArea, clsMap> _ParentMapLink;
            private modMath.sXY_int _PosA;
            private modMath.sXY_int _PosB;

            protected clsScriptArea()
            {
                this._ParentMapLink = new modLists.ConnectedListLink<clsMap.clsScriptArea, clsMap>(this);
            }

            public static clsMap.clsScriptArea Create(clsMap Map)
            {
                clsMap.clsScriptArea area2 = new clsMap.clsScriptArea {
                    _Label = Map.GetDefaultScriptLabel("Area")
                };
                area2._ParentMapLink.Connect(Map.ScriptAreas);
                return area2;
            }

            public void Deallocate()
            {
                this._ParentMapLink.Deallocate();
            }

            public void GLDraw()
            {
                clsMap.clsDrawTerrainLine line = new clsMap.clsDrawTerrainLine {
                    Map = this._ParentMapLink.Source
                };
                if (modMain.frmMainInstance.SelectedScriptMarker == this)
                {
                    GL.LineWidth(4.5f);
                    line.Colour = new sRGBA_sng(1f, 1f, 0.5f, 0.75f);
                }
                else
                {
                    GL.LineWidth(3f);
                    line.Colour = new sRGBA_sng(1f, 1f, 0f, 0.5f);
                }
                line.StartXY = this._PosA;
                line.FinishXY.X = this._PosB.X;
                line.FinishXY.Y = this._PosA.Y;
                line.ActionPerform();
                line.StartXY = this._PosA;
                line.FinishXY.X = this._PosA.X;
                line.FinishXY.Y = this._PosB.Y;
                line.ActionPerform();
                line.StartXY.X = this._PosB.X;
                line.StartXY.Y = this._PosA.Y;
                line.FinishXY = this._PosB;
                line.ActionPerform();
                line.StartXY.X = this._PosA.X;
                line.StartXY.Y = this._PosB.Y;
                line.FinishXY = this._PosB;
                line.ActionPerform();
            }

            public void MapResizing(modMath.sXY_int PosOffset)
            {
                modMath.sXY_int posA = new modMath.sXY_int(this._PosA.X - PosOffset.X, this._PosA.Y - PosOffset.Y);
                modMath.sXY_int posB = new modMath.sXY_int(this._PosB.X - PosOffset.X, this._PosB.Y - PosOffset.Y);
                this.SetPositions(posA, posB);
            }

            public modProgram.sResult SetLabel(string Text)
            {
                modProgram.sResult result = this._ParentMapLink.Source.ScriptLabelIsValid(Text);
                if (result.Success)
                {
                    this._Label = Text;
                }
                return result;
            }

            public void SetPositions(modMath.sXY_int PosA, modMath.sXY_int PosB)
            {
                clsMap source = this._ParentMapLink.Source;
                PosA.X = modMath.Clamp_int(PosA.X, 0, (source.Terrain.TileSize.X * 0x80) - 1);
                PosA.Y = modMath.Clamp_int(PosA.Y, 0, (source.Terrain.TileSize.Y * 0x80) - 1);
                PosB.X = modMath.Clamp_int(PosB.X, 0, (source.Terrain.TileSize.X * 0x80) - 1);
                PosB.Y = modMath.Clamp_int(PosB.Y, 0, (source.Terrain.TileSize.Y * 0x80) - 1);
                modMath.ReorderXY(PosA, PosB, ref this._PosA, ref this._PosB);
            }

            public void WriteWZ(clsINIWrite File)
            {
                File.SectionName_Append("area_" + modIO.InvariantToString_int(this._ParentMapLink.ArrayPosition));
                File.Property_Append("pos1", modIO.InvariantToString_int(this._PosA.X) + ", " + modIO.InvariantToString_int(this._PosA.Y));
                File.Property_Append("pos2", modIO.InvariantToString_int(this._PosB.X) + ", " + modIO.InvariantToString_int(this._PosB.Y));
                File.Property_Append("label", this._Label);
                File.Gap_Append();
            }

            public string Label
            {
                get
                {
                    return this._Label;
                }
            }

            public modLists.ConnectedListLink<clsMap.clsScriptArea, clsMap> ParentMap
            {
                get
                {
                    return this._ParentMapLink;
                }
            }

            public modMath.sXY_int PosA
            {
                set
                {
                    clsMap source = this._ParentMapLink.Source;
                    this._PosA.X = modMath.Clamp_int(value.X, 0, (source.Terrain.TileSize.X * 0x80) - 1);
                    this._PosA.Y = modMath.Clamp_int(value.Y, 0, (source.Terrain.TileSize.Y * 0x80) - 1);
                    modMath.ReorderXY(this._PosA, this._PosB, ref this._PosA, ref this._PosB);
                }
            }

            public int PosAX
            {
                get
                {
                    return this._PosA.X;
                }
                set
                {
                    this._PosA.X = modMath.Clamp_int(value, 0, (this._ParentMapLink.Source.Terrain.TileSize.X * 0x80) - 1);
                    modMath.ReorderXY(this._PosA, this._PosB, ref this._PosA, ref this._PosB);
                }
            }

            public int PosAY
            {
                get
                {
                    return this._PosA.Y;
                }
                set
                {
                    this._PosA.Y = modMath.Clamp_int(value, 0, (this._ParentMapLink.Source.Terrain.TileSize.Y * 0x80) - 1);
                    modMath.ReorderXY(this._PosA, this._PosB, ref this._PosA, ref this._PosB);
                }
            }

            public modMath.sXY_int PosB
            {
                set
                {
                    clsMap source = this._ParentMapLink.Source;
                    this._PosB.X = modMath.Clamp_int(value.X, 0, (source.Terrain.TileSize.X * 0x80) - 1);
                    this._PosB.Y = modMath.Clamp_int(value.Y, 0, (source.Terrain.TileSize.Y * 0x80) - 1);
                    modMath.ReorderXY(this._PosA, this._PosB, ref this._PosA, ref this._PosB);
                }
            }

            public int PosBX
            {
                get
                {
                    return this._PosB.X;
                }
                set
                {
                    this._PosB.X = modMath.Clamp_int(value, 0, (this._ParentMapLink.Source.Terrain.TileSize.X * 0x80) - 1);
                    modMath.ReorderXY(this._PosA, this._PosB, ref this._PosA, ref this._PosB);
                }
            }

            public int PosBY
            {
                get
                {
                    return this._PosB.Y;
                }
                set
                {
                    this._PosB.Y = modMath.Clamp_int(value, 0, (this._ParentMapLink.Source.Terrain.TileSize.Y * 0x80) - 1);
                    modMath.ReorderXY(this._PosA, this._PosB, ref this._PosA, ref this._PosB);
                }
            }
        }

        public class clsScriptPosition
        {
            private string _Label;
            private modLists.ConnectedListLink<clsMap.clsScriptPosition, clsMap> _ParentMapLink;
            private modMath.sXY_int _Pos;

            private clsScriptPosition()
            {
                this._ParentMapLink = new modLists.ConnectedListLink<clsMap.clsScriptPosition, clsMap>(this);
            }

            public static clsMap.clsScriptPosition Create(clsMap Map)
            {
                clsMap.clsScriptPosition position2 = new clsMap.clsScriptPosition {
                    _Label = Map.GetDefaultScriptLabel("Position")
                };
                position2._ParentMapLink.Connect(Map.ScriptPositions);
                return position2;
            }

            public void Deallocate()
            {
                this._ParentMapLink.Deallocate();
            }

            public void GLDraw()
            {
                clsMap.clsDrawHorizontalPosOnTerrain terrain = new clsMap.clsDrawHorizontalPosOnTerrain {
                    Map = this._ParentMapLink.Source,
                    Horizontal = this._Pos
                };
                if (modMain.frmMainInstance.SelectedScriptMarker == this)
                {
                    GL.LineWidth(4.5f);
                    terrain.Colour = new sRGBA_sng(1f, 1f, 0.5f, 1f);
                }
                else
                {
                    GL.LineWidth(3f);
                    terrain.Colour = new sRGBA_sng(1f, 1f, 0f, 0.75f);
                }
                terrain.ActionPerform();
            }

            public void MapResizing(modMath.sXY_int PosOffset)
            {
                this.PosX = this._Pos.X - PosOffset.X;
                this.PosY = this._Pos.Y - PosOffset.Y;
            }

            public modProgram.sResult SetLabel(string Text)
            {
                modProgram.sResult result = this._ParentMapLink.Source.ScriptLabelIsValid(Text);
                if (result.Success)
                {
                    this._Label = Text;
                }
                return result;
            }

            public void WriteWZ(clsINIWrite File)
            {
                File.SectionName_Append("position_" + modIO.InvariantToString_int(this._ParentMapLink.ArrayPosition));
                File.Property_Append("pos", modIO.InvariantToString_int(this._Pos.X) + ", " + modIO.InvariantToString_int(this._Pos.Y));
                File.Property_Append("label", this._Label);
                File.Gap_Append();
            }

            public string Label
            {
                get
                {
                    return this._Label;
                }
            }

            public modLists.ConnectedListLink<clsMap.clsScriptPosition, clsMap> ParentMap
            {
                get
                {
                    return this._ParentMapLink;
                }
            }

            public int PosX
            {
                get
                {
                    return this._Pos.X;
                }
                set
                {
                    this._Pos.X = modMath.Clamp_int(value, 0, (this._ParentMapLink.Source.Terrain.TileSize.X * 0x80) - 1);
                }
            }

            public int PosY
            {
                get
                {
                    return this._Pos.Y;
                }
                set
                {
                    this._Pos.Y = modMath.Clamp_int(value, 0, (this._ParentMapLink.Source.Terrain.TileSize.Y * 0x80) - 1);
                }
            }
        }

        public class clsSector
        {
            public int GLList_Textured;
            public int GLList_Wireframe;
            public modMath.sXY_int Pos;
            public modLists.ConnectedList<clsMap.clsUnitSectorConnection, clsMap.clsSector> Units;

            public clsSector(modMath.sXY_int NewPos)
            {
                this.Units = new modLists.ConnectedList<clsMap.clsUnitSectorConnection, clsMap.clsSector>(this);
                this.Pos = NewPos;
            }

            public void Deallocate()
            {
                this.Units.Deallocate();
            }

            public void DeleteLists()
            {
                if (this.GLList_Textured != 0)
                {
                    GL.DeleteLists(this.GLList_Textured, 1);
                    this.GLList_Textured = 0;
                }
                if (this.GLList_Wireframe != 0)
                {
                    GL.DeleteLists(this.GLList_Wireframe, 1);
                    this.GLList_Wireframe = 0;
                }
            }
        }

        public class clsSectorChanges : clsMap.clsMapTileChanges
        {
            public clsSectorChanges(clsMap Map) : base(Map, Map.SectorCount)
            {
            }

            public override void TileChanged(modMath.sXY_int Num)
            {
                modMath.sXY_int tileSectorNum = base.Map.GetTileSectorNum(Num);
                this.Changed(tileSectorNum);
            }
        }

        public class clsShadowSector
        {
            public modMath.sXY_int Num;
            public clsMap.clsTerrain Terrain;

            public clsShadowSector()
            {
                modMath.sXY_int newSize = new modMath.sXY_int(8, 8);
                this.Terrain = new clsMap.clsTerrain(newSize);
            }
        }

        public class clsStructureWriteWZ : modLists.SimpleListTool<clsMap.clsUnit>
        {
            public clsMap.sWrite_WZ_Args.enumCompileType CompileType;
            public BinaryWriter File;
            public int PlayerCount;
            private clsStructureType StructureType;
            private byte[] StruZeroBytesA = new byte[12];
            private byte[] StruZeroBytesB = new byte[40];
            private clsMap.clsUnit Unit;

            public void ActionPerform()
            {
                if (this.CompileType == clsMap.sWrite_WZ_Args.enumCompileType.Unspecified)
                {
                    Debugger.Break();
                }
                else
                {
                    this.StructureType = (clsStructureType) this.Unit.Type;
                    modIO.WriteTextOfLength(this.File, 40, this.StructureType.Code);
                    this.File.Write(this.Unit.ID);
                    this.File.Write((uint) this.Unit.Pos.Horizontal.X);
                    this.File.Write((uint) this.Unit.Pos.Horizontal.Y);
                    this.File.Write((uint) this.Unit.Pos.Altitude);
                    this.File.Write((uint) this.Unit.Rotation);
                    switch (this.CompileType)
                    {
                        case clsMap.sWrite_WZ_Args.enumCompileType.Multiplayer:
                            this.File.Write(this.Unit.GetBJOMultiplayerPlayerNum(this.PlayerCount));
                            break;

                        case clsMap.sWrite_WZ_Args.enumCompileType.Campaign:
                            this.File.Write(this.Unit.GetBJOCampaignPlayerNum());
                            break;

                        default:
                            Debugger.Break();
                            break;
                    }
                    this.File.Write(this.StruZeroBytesA);
                    this.File.Write((byte) 1);
                    this.File.Write((byte) 0x1a);
                    this.File.Write((byte) 0x7f);
                    this.File.Write((byte) 0);
                    this.File.Write(this.StruZeroBytesB);
                }
            }

            public void SetItem(clsMap.clsUnit Item)
            {
                this.Unit = Item;
            }
        }

        public class clsTerrain
        {
            public Side[,] SideH;
            public Side[,] SideV;
            public Tile[,] Tiles;
            public modMath.sXY_int TileSize;
            public Vertex[,] Vertices;

            public clsTerrain(modMath.sXY_int NewSize)
            {
                this.TileSize = NewSize;
                this.Vertices = new Vertex[this.TileSize.X + 1, this.TileSize.Y + 1];
                this.Tiles = new Tile[(this.TileSize.X - 1) + 1, (this.TileSize.Y - 1) + 1];
                this.SideH = new Side[(this.TileSize.X - 1) + 1, this.TileSize.Y + 1];
                this.SideV = new Side[this.TileSize.X + 1, (this.TileSize.Y - 1) + 1];
                int num3 = this.TileSize.Y - 1;
                for (int i = 0; i <= num3; i++)
                {
                    int num4 = this.TileSize.X - 1;
                    for (int j = 0; j <= num4; j++)
                    {
                        this.Tiles[j, i].Texture.TextureNum = -1;
                        this.Tiles[j, i].DownSide = TileOrientation.TileDirection_None;
                    }
                }
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct Side
            {
                public clsPainter.clsRoad Road;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct Tile
            {
                public sTexture Texture;
                public bool Tri;
                public bool TriTopLeftIsCliff;
                public bool TriTopRightIsCliff;
                public bool TriBottomLeftIsCliff;
                public bool TriBottomRightIsCliff;
                public bool Terrain_IsCliff;
                public TileOrientation.sTileDirection DownSide;
                public void Copy(clsMap.clsTerrain.Tile TileToCopy)
                {
                    this.Texture = TileToCopy.Texture;
                    this.Tri = TileToCopy.Tri;
                    this.TriTopLeftIsCliff = TileToCopy.TriTopLeftIsCliff;
                    this.TriTopRightIsCliff = TileToCopy.TriTopRightIsCliff;
                    this.TriBottomLeftIsCliff = TileToCopy.TriBottomLeftIsCliff;
                    this.TriBottomRightIsCliff = TileToCopy.TriBottomRightIsCliff;
                    this.Terrain_IsCliff = TileToCopy.Terrain_IsCliff;
                    this.DownSide = TileToCopy.DownSide;
                }

                public void TriCliffAddDirection(TileOrientation.sTileDirection Direction)
                {
                    if (Direction.X == 0)
                    {
                        if (Direction.Y == 0)
                        {
                            this.TriTopLeftIsCliff = true;
                        }
                        else if (Direction.Y == 2)
                        {
                            this.TriBottomLeftIsCliff = true;
                        }
                        else
                        {
                            Debugger.Break();
                        }
                    }
                    else if (Direction.X == 2)
                    {
                        if (Direction.Y == 0)
                        {
                            this.TriTopRightIsCliff = true;
                        }
                        else if (Direction.Y == 2)
                        {
                            this.TriBottomRightIsCliff = true;
                        }
                        else
                        {
                            Debugger.Break();
                        }
                    }
                    else
                    {
                        Debugger.Break();
                    }
                }
                [StructLayout(LayoutKind.Sequential)]
                public struct sTexture
                {
                    public int TextureNum;
                    public TileOrientation.sTileOrientation Orientation;
                }
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct Vertex
            {
                public byte Height;
                public clsPainter.clsTerrain Terrain;
            }
        }

        public class clsTerrainUpdate
        {
            public clsMap.clsPointChanges SidesH;
            public clsMap.clsPointChanges SidesV;
            public clsMap.clsPointChanges Tiles;
            public clsMap.clsPointChanges Vertices;

            public clsTerrainUpdate(modMath.sXY_int TileSize)
            {
                modMath.sXY_int pointSize = new modMath.sXY_int(TileSize.X + 1, TileSize.Y + 1);
                this.Vertices = new clsMap.clsPointChanges(pointSize);
                pointSize = new modMath.sXY_int(TileSize.X, TileSize.Y);
                this.Tiles = new clsMap.clsPointChanges(pointSize);
                pointSize = new modMath.sXY_int(TileSize.X, TileSize.Y + 1);
                this.SidesH = new clsMap.clsPointChanges(pointSize);
                pointSize = new modMath.sXY_int(TileSize.X + 1, TileSize.Y);
                this.SidesV = new clsMap.clsPointChanges(pointSize);
            }

            public void ClearAll()
            {
                this.Vertices.Clear();
                this.Tiles.Clear();
                this.SidesH.Clear();
                this.SidesV.Clear();
            }

            public void Deallocate()
            {
            }

            public void SetAllChanged()
            {
                this.Vertices.SetAllChanged();
                this.Tiles.SetAllChanged();
                this.SidesH.SetAllChanged();
                this.SidesV.SetAllChanged();
            }
        }

        public class clsUndo
        {
            public modLists.SimpleList<clsMap.clsShadowSector> ChangedSectors = new modLists.SimpleList<clsMap.clsShadowSector>();
            public modLists.SimpleList<clsMap.clsGatewayChange> GatewayChanges = new modLists.SimpleList<clsMap.clsGatewayChange>();
            public string Name;
            public modLists.SimpleList<clsMap.clsUnitChange> UnitChanges = new modLists.SimpleList<clsMap.clsUnitChange>();
        }

        public class clsUnit
        {
            private string _Label;
            public double Health;
            public uint ID;
            public modLists.ConnectedListLink<clsMap.clsUnit, clsMap> MapLink;
            public modLists.ConnectedListLink<clsMap.clsUnit, clsMap> MapSelectedUnitLink;
            public modProgram.sWorldPos Pos;
            public bool PreferPartsOutput;
            public int Rotation;
            public int SavePriority;
            public modLists.ConnectedList<clsMap.clsUnitSectorConnection, clsMap.clsUnit> Sectors;
            public clsUnitType Type;
            public clsMap.clsUnitGroup UnitGroup;

            public clsUnit()
            {
                this.MapLink = new modLists.ConnectedListLink<clsMap.clsUnit, clsMap>(this);
                this.MapSelectedUnitLink = new modLists.ConnectedListLink<clsMap.clsUnit, clsMap>(this);
                this.Sectors = new modLists.ConnectedList<clsMap.clsUnitSectorConnection, clsMap.clsUnit>(this);
                this.Health = 1.0;
                this.PreferPartsOutput = false;
            }

            public clsUnit(clsMap.clsUnit UnitToCopy, clsMap TargetMap)
            {
                bool flag;
                this.MapLink = new modLists.ConnectedListLink<clsMap.clsUnit, clsMap>(this);
                this.MapSelectedUnitLink = new modLists.ConnectedListLink<clsMap.clsUnit, clsMap>(this);
                this.Sectors = new modLists.ConnectedList<clsMap.clsUnitSectorConnection, clsMap.clsUnit>(this);
                this.Health = 1.0;
                this.PreferPartsOutput = false;
                if (UnitToCopy.Type.Type == clsUnitType.enumType.PlayerDroid)
                {
                    flag = !((clsDroidDesign) UnitToCopy.Type).IsTemplate;
                }
                else
                {
                    flag = false;
                }
                if (flag)
                {
                    clsDroidDesign design = new clsDroidDesign();
                    this.Type = design;
                    design.CopyDesign((clsDroidDesign) UnitToCopy.Type);
                    design.UpdateAttachments();
                }
                else
                {
                    this.Type = UnitToCopy.Type;
                }
                this.Pos = UnitToCopy.Pos;
                this.Rotation = UnitToCopy.Rotation;
                clsMap.clsUnitGroup unitGroup = UnitToCopy.UnitGroup;
                if (unitGroup.WZ_StartPos < 0)
                {
                    this.UnitGroup = TargetMap.ScavengerUnitGroup;
                }
                else
                {
                    this.UnitGroup = TargetMap.UnitGroups[unitGroup.WZ_StartPos];
                }
                this.SavePriority = UnitToCopy.SavePriority;
                this.Health = UnitToCopy.Health;
                this.PreferPartsOutput = UnitToCopy.PreferPartsOutput;
            }

            public void Deallocate()
            {
                this.MapLink.Deallocate();
                this.MapSelectedUnitLink.Deallocate();
                this.Sectors.Deallocate();
            }

            public void DisconnectFromMap()
            {
                if (this.MapLink.IsConnected)
                {
                    this.MapLink.Disconnect();
                }
                if (this.MapSelectedUnitLink.IsConnected)
                {
                    this.MapSelectedUnitLink.Disconnect();
                }
                this.Sectors.Clear();
            }

            public uint GetBJOCampaignPlayerNum()
            {
                int num2;
                if ((this.UnitGroup == this.MapLink.Source.ScavengerUnitGroup) | (this.UnitGroup.WZ_StartPos < 0))
                {
                    num2 = 7;
                }
                else
                {
                    num2 = this.UnitGroup.WZ_StartPos;
                }
                return (uint) num2;
            }

            public uint GetBJOMultiplayerPlayerNum(int PlayerCount)
            {
                int num2;
                if ((this.UnitGroup == this.MapLink.Source.ScavengerUnitGroup) | (this.UnitGroup.WZ_StartPos < 0))
                {
                    num2 = Math.Max(PlayerCount, 7);
                }
                else
                {
                    num2 = this.UnitGroup.WZ_StartPos;
                }
                return (uint) num2;
            }

            public string GetINIHealthPercent()
            {
                return (modIO.InvariantToString_int((int) Math.Round(modMath.Clamp_dbl(this.Health * 100.0, 1.0, 100.0))) + "%");
            }

            public string GetINIPosition()
            {
                return (modIO.InvariantToString_int(this.Pos.Horizontal.X) + ", " + modIO.InvariantToString_int(this.Pos.Horizontal.Y) + ", 0");
            }

            public string GetINIRotation()
            {
                int num = (int) Math.Round((double) (((double) (this.Rotation * 0x10000)) / 360.0));
                if (num >= 0x10000)
                {
                    num -= 0x10000;
                }
                else if (num < 0)
                {
                    Debugger.Break();
                    num += 0x10000;
                }
                return (modIO.InvariantToString_int(num) + ", 0, 0");
            }

            public string GetPosText()
            {
                return (modIO.InvariantToString_int(this.Pos.Horizontal.X) + ", " + modIO.InvariantToString_int(this.Pos.Horizontal.Y));
            }

            public void MapDeselect()
            {
                if (!this.MapSelectedUnitLink.IsConnected)
                {
                    Debugger.Break();
                }
                else
                {
                    this.MapSelectedUnitLink.Disconnect();
                }
            }

            public void MapSelect()
            {
                if (this.MapSelectedUnitLink.IsConnected)
                {
                    Debugger.Break();
                }
                else
                {
                    this.MapSelectedUnitLink.Connect(this.MapLink.Source.SelectedUnits);
                }
            }

            public modProgram.sResult SetLabel(string Text)
            {
                modProgram.sResult result;
                if (this.Type.Type == clsUnitType.enumType.PlayerStructure)
                {
                    clsStructureType type = (clsStructureType) this.Type;
                    clsStructureType.enumStructureType structureType = type.StructureType;
                    if (((structureType == clsStructureType.enumStructureType.FactoryModule) | (structureType == clsStructureType.enumStructureType.PowerModule)) | (structureType == clsStructureType.enumStructureType.ResearchModule))
                    {
                        result.Problem = "Error: Trying to assign label to structure module.";
                        return result;
                    }
                }
                if (!this.MapLink.IsConnected)
                {
                    Debugger.Break();
                    result.Problem = "Error: Unit not on a map.";
                    return result;
                }
                if (Text == null)
                {
                    this._Label = null;
                    result.Success = true;
                    result.Problem = "";
                    return result;
                }
                result = this.MapLink.Source.ScriptLabelIsValid(Text);
                if (result.Success)
                {
                    this._Label = Text;
                }
                return result;
            }

            public void WriteWZLabel(clsINIWrite File, int PlayerCount)
            {
                if (this._Label != null)
                {
                    int num;
                    switch (this.Type.Type)
                    {
                        case clsUnitType.enumType.Feature:
                            num = 2;
                            break;

                        case clsUnitType.enumType.PlayerStructure:
                            num = 1;
                            break;

                        case clsUnitType.enumType.PlayerDroid:
                            num = 0;
                            break;

                        default:
                            return;
                    }
                    File.SectionName_Append("object_" + modIO.InvariantToString_int(this.MapLink.ArrayPosition));
                    File.Property_Append("id", modIO.InvariantToString_uint(this.ID));
                    if (PlayerCount >= 0)
                    {
                        File.Property_Append("type", modIO.InvariantToString_int(num));
                        File.Property_Append("player", modIO.InvariantToString_int(this.UnitGroup.GetPlayerNum(PlayerCount)));
                    }
                    File.Property_Append("label", this._Label);
                    File.Gap_Append();
                }
            }

            public string Label
            {
                get
                {
                    return this._Label;
                }
            }
        }

        public class clsUnitAdd
        {
            public uint ID = 0;
            public string Label = null;
            public clsMap Map;
            public clsMap.clsUnit NewUnit;
            public bool StoreChange = false;

            public bool Perform()
            {
                if (this.Map == null)
                {
                    Debugger.Break();
                    return false;
                }
                if (this.NewUnit == null)
                {
                    Debugger.Break();
                    return false;
                }
                if (this.NewUnit.MapLink.IsConnected)
                {
                    Interaction.MsgBox("Error: Added object already has a map assigned.", MsgBoxStyle.ApplicationModal, null);
                    return false;
                }
                if (this.NewUnit.UnitGroup == null)
                {
                    Interaction.MsgBox("Error: Added object has no group.", MsgBoxStyle.ApplicationModal, null);
                    this.NewUnit.UnitGroup = this.Map.ScavengerUnitGroup;
                    return false;
                }
                if (this.NewUnit.UnitGroup.MapLink.Source != this.Map)
                {
                    Interaction.MsgBox("Error: Something terrible happened.", MsgBoxStyle.ApplicationModal, null);
                    return false;
                }
                if (this.StoreChange)
                {
                    clsMap.clsUnitChange newItem = new clsMap.clsUnitChange {
                        Type = clsMap.clsUnitChange.enumType.Added,
                        Unit = this.NewUnit
                    };
                    this.Map.UnitChanges.Add(newItem);
                }
                if (this.ID <= 0)
                {
                    this.ID = this.Map.GetAvailableID();
                }
                else if (this.Map.IDUsage(this.ID) != null)
                {
                    this.ID = this.Map.GetAvailableID();
                }
                this.NewUnit.ID = this.ID;
                this.NewUnit.MapLink.Connect(this.Map.Units);
                this.NewUnit.Pos.Horizontal.X = modMath.Clamp_int(this.NewUnit.Pos.Horizontal.X, 0, (this.Map.Terrain.TileSize.X * 0x80) - 1);
                this.NewUnit.Pos.Horizontal.Y = modMath.Clamp_int(this.NewUnit.Pos.Horizontal.Y, 0, (this.Map.Terrain.TileSize.Y * 0x80) - 1);
                this.NewUnit.Pos.Altitude = (int) Math.Round(Math.Ceiling(this.Map.GetTerrainHeight(this.NewUnit.Pos.Horizontal)));
                if (this.Label != null)
                {
                    this.NewUnit.SetLabel(this.Label);
                }
                this.Map.UnitSectorsCalc(this.NewUnit);
                if (this.Map.SectorGraphicsChanges != null)
                {
                    this.Map.UnitSectorsGraphicsChanged(this.NewUnit);
                }
                return true;
            }
        }

        public class clsUnitChange
        {
            public enumType Type;
            public clsMap.clsUnit Unit;

            public enum enumType : byte
            {
                Added = 0,
                Deleted = 1
            }
        }

        public class clsUnitCreate
        {
            public bool AutoWalls = false;
            public modMath.sXY_int Horizontal;
            public clsMap Map;
            public clsUnitType ObjectType;
            public bool RandomizeRotation = false;
            public int Rotation = 0;
            public clsMap.clsUnitGroup UnitGroup;

            public clsMap.clsUnit Perform()
            {
                if (this.AutoWalls && (this.ObjectType.Type == clsUnitType.enumType.PlayerStructure))
                {
                    clsStructureType objectType = (clsStructureType) this.ObjectType;
                    if (objectType.WallLink.IsConnected)
                    {
                        clsWallType wallType = null;
                        wallType = objectType.WallLink.Source;
                        this.Map.PerformTileWall(wallType, this.Map.GetPosTileNum(this.Horizontal), true);
                        return null;
                    }
                }
                clsMap.clsUnit unit = new clsMap.clsUnit();
                if (this.RandomizeRotation)
                {
                    unit.Rotation = (int) Math.Round(((double) (App.Random.Next() * 360.0)));
                }
                else
                {
                    unit.Rotation = this.Rotation;
                }
                unit.UnitGroup = this.UnitGroup;
                unit.Pos = this.Map.TileAlignedPosFromMapPos(this.Horizontal, this.ObjectType.get_GetFootprintSelected(unit.Rotation));
                unit.Type = this.ObjectType;
                new clsMap.clsUnitAdd { Map = this.Map, NewUnit = unit, StoreChange = true }.Perform();
                return unit;
            }
        }

        public class clsUnitGroup
        {
            public modLists.ConnectedListLink<clsMap.clsUnitGroup, clsMap> MapLink;
            public int WZ_StartPos;

            public clsUnitGroup()
            {
                this.MapLink = new modLists.ConnectedListLink<clsMap.clsUnitGroup, clsMap>(this);
                this.WZ_StartPos = -1;
            }

            public string GetFMapINIPlayerText()
            {
                if ((this.WZ_StartPos < 0) | (this.WZ_StartPos >= 10))
                {
                    return "scavenger";
                }
                return modIO.InvariantToString_int(this.WZ_StartPos);
            }

            public string GetLNDPlayerText()
            {
                if ((this.WZ_StartPos < 0) | (this.WZ_StartPos >= 10))
                {
                    return modIO.InvariantToString_int(7);
                }
                return modIO.InvariantToString_int(this.WZ_StartPos);
            }

            public int GetPlayerNum(int PlayerCount)
            {
                if ((this.WZ_StartPos < 0) | (this.WZ_StartPos >= 10))
                {
                    return Math.Max(PlayerCount, 7);
                }
                return this.WZ_StartPos;
            }
        }

        public class clsUnitGroupContainer
        {
            private clsMap.clsUnitGroup _Item;

            public event ChangedEventHandler Changed;

            public clsMap.clsUnitGroup Item
            {
                get
                {
                    return this._Item;
                }
                set
                {
                    if (value != this._Item)
                    {
                        this._Item = value;
                        ChangedEventHandler changedEvent = this.ChangedEvent;
                        if (changedEvent != null)
                        {
                            changedEvent();
                        }
                    }
                }
            }

            public delegate void ChangedEventHandler();
        }

        public class clsUnitSectorConnection
        {
            protected Link<clsMap.clsSector> _SectorLink;
            protected Link<clsMap.clsUnit> _UnitLink;

            protected clsUnitSectorConnection()
            {
                this._UnitLink = new Link<clsMap.clsUnit>(this);
                this._SectorLink = new Link<clsMap.clsSector>(this);
            }

            public static clsMap.clsUnitSectorConnection Create(clsMap.clsUnit Unit, clsMap.clsSector Sector)
            {
                if (Unit == null)
                {
                    return null;
                }
                if (Unit.Sectors == null)
                {
                    return null;
                }
                if (Unit.Sectors.IsBusy)
                {
                    return null;
                }
                if (Sector == null)
                {
                    return null;
                }
                if (Sector.Units == null)
                {
                    return null;
                }
                if (Sector.Units.IsBusy)
                {
                    return null;
                }
                clsMap.clsUnitSectorConnection connection2 = new clsMap.clsUnitSectorConnection();
                connection2._UnitLink.Connect(Unit.Sectors);
                connection2._SectorLink.Connect(Sector.Units);
                return connection2;
            }

            public void Deallocate()
            {
                this._UnitLink.Deallocate();
                this._SectorLink.Deallocate();
            }

            public virtual clsMap.clsSector Sector
            {
                get
                {
                    return this._SectorLink.Source;
                }
            }

            public virtual clsMap.clsUnit Unit
            {
                get
                {
                    return this._UnitLink.Source;
                }
            }

            protected class Link<SourceType> : modLists.ConnectedListLink<clsMap.clsUnitSectorConnection, SourceType> where SourceType: class
            {
                public Link(clsMap.clsUnitSectorConnection Owner) : base(Owner)
                {
                }

                public override void AfterRemove()
                {
                    base.AfterRemove();
                    this.Item.Deallocate();
                }
            }
        }

        public class clsUpdateAutotexture : clsMap.clsAction
        {
            public bool MakeInvalidTiles;
            private clsPainter Painter;
            private TileOrientation.sTileDirection ResultDirection;
            private clsPainter.clsTileList.sTileOrientationChance ResultTexture;
            private clsPainter.clsTileList ResultTiles;
            private clsPainter.clsRoad Road;
            private bool RoadBottom;
            private bool RoadLeft;
            private bool RoadRight;
            private bool RoadTop;
            private clsMap.clsTerrain Terrain;
            private clsPainter.clsTerrain Terrain_Inner;
            private clsPainter.clsTerrain Terrain_Outer;

            public override void ActionPerform()
            {
                int num11;
                this.Terrain = base.Map.Terrain;
                this.Painter = base.Map.Painter;
                this.ResultTiles = null;
                this.ResultDirection = TileOrientation.TileDirection_None;
                if (!this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].Terrain_IsCliff)
                {
                    int num21 = this.Painter.TerrainCount - 1;
                    for (int i = 0; i <= num21; i++)
                    {
                        this.Terrain_Inner = this.Painter.Terrains[i];
                        if (((this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y].Terrain == this.Terrain_Inner) && (this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y].Terrain == this.Terrain_Inner)) && ((this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y + 1].Terrain == this.Terrain_Inner) && (this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y + 1].Terrain == this.Terrain_Inner)))
                        {
                            this.ResultTiles = this.Terrain_Inner.Tiles;
                            this.ResultDirection = TileOrientation.TileDirection_None;
                        }
                    }
                }
                if (!this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].Terrain_IsCliff)
                {
                    int num22 = this.Painter.TransitionBrushCount - 1;
                    for (int j = 0; j <= num22; j++)
                    {
                        this.Terrain_Inner = this.Painter.TransitionBrushes[j].Terrain_Inner;
                        this.Terrain_Outer = this.Painter.TransitionBrushes[j].Terrain_Outer;
                        if (this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y].Terrain == this.Terrain_Inner)
                        {
                            if (this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y].Terrain == this.Terrain_Inner)
                            {
                                if (this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y + 1].Terrain == this.Terrain_Inner)
                                {
                                    if (this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y + 1].Terrain != this.Terrain_Inner)
                                    {
                                        if (this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y + 1].Terrain != this.Terrain_Outer)
                                        {
                                            continue;
                                        }
                                        this.ResultTiles = this.Painter.TransitionBrushes[j].Tiles_Corner_In;
                                        this.ResultDirection = TileOrientation.TileDirection_BottomRight;
                                    }
                                }
                                else
                                {
                                    if (this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y + 1].Terrain != this.Terrain_Outer)
                                    {
                                        continue;
                                    }
                                    if (this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y + 1].Terrain == this.Terrain_Inner)
                                    {
                                        this.ResultTiles = this.Painter.TransitionBrushes[j].Tiles_Corner_In;
                                        this.ResultDirection = TileOrientation.TileDirection_BottomLeft;
                                    }
                                    else
                                    {
                                        if (this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y + 1].Terrain != this.Terrain_Outer)
                                        {
                                            continue;
                                        }
                                        this.ResultTiles = this.Painter.TransitionBrushes[j].Tiles_Straight;
                                        this.ResultDirection = TileOrientation.TileDirection_Bottom;
                                    }
                                }
                            }
                            else
                            {
                                if (this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y].Terrain != this.Terrain_Outer)
                                {
                                    continue;
                                }
                                if (this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y + 1].Terrain == this.Terrain_Inner)
                                {
                                    if (this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y + 1].Terrain == this.Terrain_Inner)
                                    {
                                        this.ResultTiles = this.Painter.TransitionBrushes[j].Tiles_Corner_In;
                                        this.ResultDirection = TileOrientation.TileDirection_TopRight;
                                    }
                                    else
                                    {
                                        if (this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y + 1].Terrain != this.Terrain_Outer)
                                        {
                                            continue;
                                        }
                                        this.ResultTiles = this.Painter.TransitionBrushes[j].Tiles_Straight;
                                        this.ResultDirection = TileOrientation.TileDirection_Right;
                                    }
                                }
                                else
                                {
                                    if (this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y + 1].Terrain != this.Terrain_Outer)
                                    {
                                        continue;
                                    }
                                    if (this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y + 1].Terrain == this.Terrain_Inner)
                                    {
                                        this.ResultTiles = null;
                                        this.ResultDirection = TileOrientation.TileDirection_None;
                                    }
                                    else
                                    {
                                        if (this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y + 1].Terrain != this.Terrain_Outer)
                                        {
                                            continue;
                                        }
                                        this.ResultTiles = this.Painter.TransitionBrushes[j].Tiles_Corner_Out;
                                        this.ResultDirection = TileOrientation.TileDirection_BottomRight;
                                    }
                                }
                            }
                            break;
                        }
                        if (this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y].Terrain == this.Terrain_Outer)
                        {
                            if (this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y].Terrain == this.Terrain_Inner)
                            {
                                if (this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y + 1].Terrain == this.Terrain_Inner)
                                {
                                    if (this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y + 1].Terrain == this.Terrain_Inner)
                                    {
                                        this.ResultTiles = this.Painter.TransitionBrushes[j].Tiles_Corner_In;
                                        this.ResultDirection = TileOrientation.TileDirection_TopLeft;
                                    }
                                    else
                                    {
                                        if (this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y + 1].Terrain != this.Terrain_Outer)
                                        {
                                            continue;
                                        }
                                        this.ResultTiles = null;
                                        this.ResultDirection = TileOrientation.TileDirection_None;
                                    }
                                }
                                else
                                {
                                    if (this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y + 1].Terrain != this.Terrain_Outer)
                                    {
                                        continue;
                                    }
                                    if (this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y + 1].Terrain == this.Terrain_Inner)
                                    {
                                        this.ResultTiles = this.Painter.TransitionBrushes[j].Tiles_Straight;
                                        this.ResultDirection = TileOrientation.TileDirection_Left;
                                    }
                                    else
                                    {
                                        if (this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y + 1].Terrain != this.Terrain_Outer)
                                        {
                                            continue;
                                        }
                                        this.ResultTiles = this.Painter.TransitionBrushes[j].Tiles_Corner_Out;
                                        this.ResultDirection = TileOrientation.TileDirection_BottomLeft;
                                    }
                                }
                                break;
                            }
                            if (this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y].Terrain == this.Terrain_Outer)
                            {
                                if (this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y + 1].Terrain == this.Terrain_Inner)
                                {
                                    if (this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y + 1].Terrain == this.Terrain_Inner)
                                    {
                                        this.ResultTiles = this.Painter.TransitionBrushes[j].Tiles_Straight;
                                        this.ResultDirection = TileOrientation.TileDirection_Top;
                                    }
                                    else
                                    {
                                        if (this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y + 1].Terrain != this.Terrain_Outer)
                                        {
                                            continue;
                                        }
                                        this.ResultTiles = this.Painter.TransitionBrushes[j].Tiles_Corner_Out;
                                        this.ResultDirection = TileOrientation.TileDirection_TopRight;
                                    }
                                    break;
                                }
                                if (this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y + 1].Terrain == this.Terrain_Outer)
                                {
                                    if (this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y + 1].Terrain == this.Terrain_Inner)
                                    {
                                        this.ResultTiles = this.Painter.TransitionBrushes[j].Tiles_Corner_Out;
                                        this.ResultDirection = TileOrientation.TileDirection_TopLeft;
                                        break;
                                    }
                                    if (this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y + 1].Terrain == this.Terrain_Outer)
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                if (!this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].Tri)
                {
                    if (!this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].TriTopRightIsCliff)
                    {
                        if (this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].TriBottomLeftIsCliff)
                        {
                            int index = 0;
                            int num28 = this.Painter.CliffBrushCount - 1;
                            index = 0;
                            while (index <= num28)
                            {
                                this.Terrain_Inner = this.Painter.CliffBrushes[index].Terrain_Inner;
                                this.Terrain_Outer = this.Painter.CliffBrushes[index].Terrain_Outer;
                                if (this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y + 1].Terrain == this.Terrain_Outer)
                                {
                                    int num17 = 0;
                                    if (this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y].Terrain == this.Terrain_Inner)
                                    {
                                        num17++;
                                    }
                                    if (this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y].Terrain == this.Terrain_Inner)
                                    {
                                        num17++;
                                    }
                                    if (this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y + 1].Terrain == this.Terrain_Inner)
                                    {
                                        num17++;
                                    }
                                    if (num17 < 2)
                                    {
                                        goto Label_2914;
                                    }
                                    this.ResultTiles = this.Painter.CliffBrushes[index].Tiles_Corner_In;
                                    this.ResultDirection = TileOrientation.TileDirection_BottomLeft;
                                    break;
                                }
                                if (this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y + 1].Terrain == this.Terrain_Inner)
                                {
                                    int num18 = 0;
                                    if (this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y].Terrain == this.Terrain_Outer)
                                    {
                                        num18++;
                                    }
                                    if (this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y].Terrain == this.Terrain_Outer)
                                    {
                                        num18++;
                                    }
                                    if (this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y + 1].Terrain == this.Terrain_Outer)
                                    {
                                        num18++;
                                    }
                                    if (num18 >= 2)
                                    {
                                        this.ResultTiles = this.Painter.CliffBrushes[index].Tiles_Corner_Out;
                                        this.ResultDirection = TileOrientation.TileDirection_TopRight;
                                        break;
                                    }
                                }
                            Label_2914:
                                index++;
                            }
                            if (index == this.Painter.CliffBrushCount)
                            {
                                this.ResultTiles = null;
                                this.ResultDirection = TileOrientation.TileDirection_None;
                            }
                        }
                        goto Label_2944;
                    }
                    if (!this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].TriBottomLeftIsCliff)
                    {
                        int num13 = 0;
                        int num27 = this.Painter.CliffBrushCount - 1;
                        num13 = 0;
                        while (num13 <= num27)
                        {
                            this.Terrain_Inner = this.Painter.CliffBrushes[num13].Terrain_Inner;
                            this.Terrain_Outer = this.Painter.CliffBrushes[num13].Terrain_Outer;
                            if (this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y].Terrain == this.Terrain_Outer)
                            {
                                int num14 = 0;
                                if (this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y].Terrain == this.Terrain_Inner)
                                {
                                    num14++;
                                }
                                if (this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y + 1].Terrain == this.Terrain_Inner)
                                {
                                    num14++;
                                }
                                if (this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y + 1].Terrain == this.Terrain_Inner)
                                {
                                    num14++;
                                }
                                if (num14 < 2)
                                {
                                    goto Label_262F;
                                }
                                this.ResultTiles = this.Painter.CliffBrushes[num13].Tiles_Corner_In;
                                this.ResultDirection = TileOrientation.TileDirection_TopRight;
                                break;
                            }
                            if (this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y].Terrain == this.Terrain_Inner)
                            {
                                int num15 = 0;
                                if (this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y].Terrain == this.Terrain_Outer)
                                {
                                    num15++;
                                }
                                if (this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y + 1].Terrain == this.Terrain_Outer)
                                {
                                    num15++;
                                }
                                if (this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y + 1].Terrain == this.Terrain_Outer)
                                {
                                    num15++;
                                }
                                if (num15 >= 2)
                                {
                                    this.ResultTiles = this.Painter.CliffBrushes[num13].Tiles_Corner_Out;
                                    this.ResultDirection = TileOrientation.TileDirection_BottomLeft;
                                    break;
                                }
                            }
                        Label_262F:
                            num13++;
                        }
                        if (num13 == this.Painter.CliffBrushCount)
                        {
                            this.ResultTiles = null;
                            this.ResultDirection = TileOrientation.TileDirection_None;
                        }
                        goto Label_2944;
                    }
                    num11 = 0;
                    int num26 = this.Painter.CliffBrushCount - 1;
                    num11 = 0;
                    while (num11 <= num26)
                    {
                        this.Terrain_Inner = this.Painter.CliffBrushes[num11].Terrain_Inner;
                        this.Terrain_Outer = this.Painter.CliffBrushes[num11].Terrain_Outer;
                        if (this.Terrain_Inner == this.Terrain_Outer)
                        {
                            int num12 = 0;
                            if (this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y].Terrain == this.Terrain_Inner)
                            {
                                num12++;
                            }
                            if (this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y].Terrain == this.Terrain_Inner)
                            {
                                num12++;
                            }
                            if (this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y + 1].Terrain == this.Terrain_Inner)
                            {
                                num12++;
                            }
                            if (this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y + 1].Terrain == this.Terrain_Inner)
                            {
                                num12++;
                            }
                            if (num12 >= 3)
                            {
                                this.ResultTiles = this.Painter.CliffBrushes[num11].Tiles_Straight;
                                this.ResultDirection = this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].DownSide;
                                break;
                            }
                        }
                        if ((((this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y].Terrain == this.Terrain_Inner) & (this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y].Terrain == this.Terrain_Inner)) & ((this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y + 1].Terrain == this.Terrain_Outer) | (this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y + 1].Terrain == this.Terrain_Outer))) | (((this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y].Terrain == this.Terrain_Inner) | (this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y].Terrain == this.Terrain_Inner)) & ((this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y + 1].Terrain == this.Terrain_Outer) & (this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y + 1].Terrain == this.Terrain_Outer))))
                        {
                            this.ResultTiles = this.Painter.CliffBrushes[num11].Tiles_Straight;
                            this.ResultDirection = TileOrientation.TileDirection_Bottom;
                            break;
                        }
                        if ((((this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y].Terrain == this.Terrain_Outer) & (this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y + 1].Terrain == this.Terrain_Outer)) & ((this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y].Terrain == this.Terrain_Inner) | (this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y + 1].Terrain == this.Terrain_Inner))) | (((this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y].Terrain == this.Terrain_Outer) | (this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y + 1].Terrain == this.Terrain_Outer)) & ((this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y].Terrain == this.Terrain_Inner) & (this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y + 1].Terrain == this.Terrain_Inner))))
                        {
                            this.ResultTiles = this.Painter.CliffBrushes[num11].Tiles_Straight;
                            this.ResultDirection = TileOrientation.TileDirection_Left;
                            break;
                        }
                        if ((((this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y].Terrain == this.Terrain_Outer) & (this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y].Terrain == this.Terrain_Outer)) & ((this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y + 1].Terrain == this.Terrain_Inner) | (this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y + 1].Terrain == this.Terrain_Inner))) | (((this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y].Terrain == this.Terrain_Outer) | (this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y].Terrain == this.Terrain_Outer)) & ((this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y + 1].Terrain == this.Terrain_Inner) & (this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y + 1].Terrain == this.Terrain_Inner))))
                        {
                            this.ResultTiles = this.Painter.CliffBrushes[num11].Tiles_Straight;
                            this.ResultDirection = TileOrientation.TileDirection_Top;
                            break;
                        }
                        if ((((this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y].Terrain == this.Terrain_Inner) & (this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y + 1].Terrain == this.Terrain_Inner)) & ((this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y].Terrain == this.Terrain_Outer) | (this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y + 1].Terrain == this.Terrain_Outer))) | (((this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y].Terrain == this.Terrain_Inner) | (this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y + 1].Terrain == this.Terrain_Inner)) & ((this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y].Terrain == this.Terrain_Outer) & (this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y + 1].Terrain == this.Terrain_Outer))))
                        {
                            this.ResultTiles = this.Painter.CliffBrushes[num11].Tiles_Straight;
                            this.ResultDirection = TileOrientation.TileDirection_Right;
                            break;
                        }
                        num11++;
                    }
                }
                else
                {
                    int num8;
                    if (!this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].TriTopLeftIsCliff)
                    {
                        if (!this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].TriBottomRightIsCliff)
                        {
                            goto Label_2944;
                        }
                        num8 = 0;
                        int num25 = this.Painter.CliffBrushCount - 1;
                        num8 = 0;
                        while (num8 <= num25)
                        {
                            this.Terrain_Inner = this.Painter.CliffBrushes[num8].Terrain_Inner;
                            this.Terrain_Outer = this.Painter.CliffBrushes[num8].Terrain_Outer;
                            if (this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y + 1].Terrain == this.Terrain_Outer)
                            {
                                int num9 = 0;
                                if (this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y].Terrain == this.Terrain_Inner)
                                {
                                    num9++;
                                }
                                if (this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y].Terrain == this.Terrain_Inner)
                                {
                                    num9++;
                                }
                                if (this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y + 1].Terrain == this.Terrain_Inner)
                                {
                                    num9++;
                                }
                                if (num9 < 2)
                                {
                                    goto Label_19D5;
                                }
                                this.ResultTiles = this.Painter.CliffBrushes[num8].Tiles_Corner_In;
                                this.ResultDirection = TileOrientation.TileDirection_BottomRight;
                                break;
                            }
                            if (this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y + 1].Terrain == this.Terrain_Inner)
                            {
                                int num10 = 0;
                                if (this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y].Terrain == this.Terrain_Outer)
                                {
                                    num10++;
                                }
                                if (this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y].Terrain == this.Terrain_Outer)
                                {
                                    num10++;
                                }
                                if (this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y + 1].Terrain == this.Terrain_Outer)
                                {
                                    num10++;
                                }
                                if (num10 >= 2)
                                {
                                    this.ResultTiles = this.Painter.CliffBrushes[num8].Tiles_Corner_Out;
                                    this.ResultDirection = TileOrientation.TileDirection_TopLeft;
                                    break;
                                }
                            }
                        Label_19D5:
                            num8++;
                        }
                    }
                    else
                    {
                        int num5;
                        if (!this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].TriBottomRightIsCliff)
                        {
                            num5 = 0;
                            int num24 = this.Painter.CliffBrushCount - 1;
                            num5 = 0;
                            while (num5 <= num24)
                            {
                                this.Terrain_Inner = this.Painter.CliffBrushes[num5].Terrain_Inner;
                                this.Terrain_Outer = this.Painter.CliffBrushes[num5].Terrain_Outer;
                                if (this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y].Terrain == this.Terrain_Outer)
                                {
                                    int num6 = 0;
                                    if (this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y].Terrain == this.Terrain_Inner)
                                    {
                                        num6++;
                                    }
                                    if (this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y + 1].Terrain == this.Terrain_Inner)
                                    {
                                        num6++;
                                    }
                                    if (this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y + 1].Terrain == this.Terrain_Inner)
                                    {
                                        num6++;
                                    }
                                    if (num6 < 2)
                                    {
                                        goto Label_16F0;
                                    }
                                    this.ResultTiles = this.Painter.CliffBrushes[num5].Tiles_Corner_In;
                                    this.ResultDirection = TileOrientation.TileDirection_TopLeft;
                                    break;
                                }
                                if (this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y].Terrain == this.Terrain_Inner)
                                {
                                    int num7 = 0;
                                    if (this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y].Terrain == this.Terrain_Outer)
                                    {
                                        num7++;
                                    }
                                    if (this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y + 1].Terrain == this.Terrain_Outer)
                                    {
                                        num7++;
                                    }
                                    if (this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y + 1].Terrain == this.Terrain_Outer)
                                    {
                                        num7++;
                                    }
                                    if (num7 >= 2)
                                    {
                                        this.ResultTiles = this.Painter.CliffBrushes[num5].Tiles_Corner_Out;
                                        this.ResultDirection = TileOrientation.TileDirection_BottomRight;
                                        break;
                                    }
                                }
                            Label_16F0:
                                num5++;
                            }
                        }
                        else
                        {
                            int num3 = 0;
                            int num23 = this.Painter.CliffBrushCount - 1;
                            num3 = 0;
                            while (num3 <= num23)
                            {
                                this.Terrain_Inner = this.Painter.CliffBrushes[num3].Terrain_Inner;
                                this.Terrain_Outer = this.Painter.CliffBrushes[num3].Terrain_Outer;
                                if (this.Terrain_Inner == this.Terrain_Outer)
                                {
                                    int num4 = 0;
                                    if (this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y].Terrain == this.Terrain_Inner)
                                    {
                                        num4++;
                                    }
                                    if (this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y].Terrain == this.Terrain_Inner)
                                    {
                                        num4++;
                                    }
                                    if (this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y + 1].Terrain == this.Terrain_Inner)
                                    {
                                        num4++;
                                    }
                                    if (this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y + 1].Terrain == this.Terrain_Inner)
                                    {
                                        num4++;
                                    }
                                    if (num4 >= 3)
                                    {
                                        this.ResultTiles = this.Painter.CliffBrushes[num3].Tiles_Straight;
                                        this.ResultDirection = this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].DownSide;
                                        break;
                                    }
                                }
                                if ((((this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y].Terrain == this.Terrain_Inner) & (this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y].Terrain == this.Terrain_Inner)) & ((this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y + 1].Terrain == this.Terrain_Outer) | (this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y + 1].Terrain == this.Terrain_Outer))) | (((this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y].Terrain == this.Terrain_Inner) | (this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y].Terrain == this.Terrain_Inner)) & ((this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y + 1].Terrain == this.Terrain_Outer) & (this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y + 1].Terrain == this.Terrain_Outer))))
                                {
                                    this.ResultTiles = this.Painter.CliffBrushes[num3].Tiles_Straight;
                                    this.ResultDirection = TileOrientation.TileDirection_Bottom;
                                    break;
                                }
                                if ((((this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y].Terrain == this.Terrain_Outer) & (this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y + 1].Terrain == this.Terrain_Outer)) & ((this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y].Terrain == this.Terrain_Inner) | (this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y + 1].Terrain == this.Terrain_Inner))) | (((this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y].Terrain == this.Terrain_Outer) | (this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y + 1].Terrain == this.Terrain_Outer)) & ((this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y].Terrain == this.Terrain_Inner) & (this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y + 1].Terrain == this.Terrain_Inner))))
                                {
                                    this.ResultTiles = this.Painter.CliffBrushes[num3].Tiles_Straight;
                                    this.ResultDirection = TileOrientation.TileDirection_Left;
                                    break;
                                }
                                if ((((this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y].Terrain == this.Terrain_Outer) & (this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y].Terrain == this.Terrain_Outer)) & ((this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y + 1].Terrain == this.Terrain_Inner) | (this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y + 1].Terrain == this.Terrain_Inner))) | (((this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y].Terrain == this.Terrain_Outer) | (this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y].Terrain == this.Terrain_Outer)) & ((this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y + 1].Terrain == this.Terrain_Inner) & (this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y + 1].Terrain == this.Terrain_Inner))))
                                {
                                    this.ResultTiles = this.Painter.CliffBrushes[num3].Tiles_Straight;
                                    this.ResultDirection = TileOrientation.TileDirection_Top;
                                    break;
                                }
                                if ((((this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y].Terrain == this.Terrain_Inner) & (this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y + 1].Terrain == this.Terrain_Inner)) & ((this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y].Terrain == this.Terrain_Outer) | (this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y + 1].Terrain == this.Terrain_Outer))) | (((this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y].Terrain == this.Terrain_Inner) | (this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y + 1].Terrain == this.Terrain_Inner)) & ((this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y].Terrain == this.Terrain_Outer) & (this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y + 1].Terrain == this.Terrain_Outer))))
                                {
                                    this.ResultTiles = this.Painter.CliffBrushes[num3].Tiles_Straight;
                                    this.ResultDirection = TileOrientation.TileDirection_Right;
                                    break;
                                }
                                num3++;
                            }
                            if (num3 == this.Painter.CliffBrushCount)
                            {
                                this.ResultTiles = null;
                                this.ResultDirection = TileOrientation.TileDirection_None;
                            }
                            goto Label_2944;
                        }
                        if (num5 == this.Painter.CliffBrushCount)
                        {
                            this.ResultTiles = null;
                            this.ResultDirection = TileOrientation.TileDirection_None;
                        }
                        goto Label_2944;
                    }
                    if (num8 == this.Painter.CliffBrushCount)
                    {
                        this.ResultTiles = null;
                        this.ResultDirection = TileOrientation.TileDirection_None;
                    }
                    goto Label_2944;
                }
                if (num11 == this.Painter.CliffBrushCount)
                {
                    this.ResultTiles = null;
                    this.ResultDirection = TileOrientation.TileDirection_None;
                }
            Label_2944:
                this.Road = null;
                if (this.Terrain.SideH[this.PosNum.X, this.PosNum.Y].Road != null)
                {
                    this.Road = this.Terrain.SideH[this.PosNum.X, this.PosNum.Y].Road;
                }
                else if (this.Terrain.SideH[this.PosNum.X, this.PosNum.Y + 1].Road != null)
                {
                    this.Road = this.Terrain.SideH[this.PosNum.X, this.PosNum.Y + 1].Road;
                }
                else if (this.Terrain.SideV[this.PosNum.X + 1, this.PosNum.Y].Road != null)
                {
                    this.Road = this.Terrain.SideV[this.PosNum.X + 1, this.PosNum.Y].Road;
                }
                else if (this.Terrain.SideV[this.PosNum.X, this.PosNum.Y].Road != null)
                {
                    this.Road = this.Terrain.SideV[this.PosNum.X, this.PosNum.Y].Road;
                }
                if (this.Road != null)
                {
                    int num19 = 0;
                    int num29 = this.Painter.RoadBrushCount - 1;
                    num19 = 0;
                    while (num19 <= num29)
                    {
                        if (this.Painter.RoadBrushes[num19].Road == this.Road)
                        {
                            this.Terrain_Outer = this.Painter.RoadBrushes[num19].Terrain;
                            int num20 = 0;
                            if (this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y].Terrain == this.Terrain_Outer)
                            {
                                num20++;
                            }
                            if (this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y].Terrain == this.Terrain_Outer)
                            {
                                num20++;
                            }
                            if (this.Terrain.Vertices[this.PosNum.X, this.PosNum.Y + 1].Terrain == this.Terrain_Outer)
                            {
                                num20++;
                            }
                            if (this.Terrain.Vertices[this.PosNum.X + 1, this.PosNum.Y + 1].Terrain == this.Terrain_Outer)
                            {
                                num20++;
                            }
                            if (num20 >= 2)
                            {
                                break;
                            }
                        }
                        num19++;
                    }
                    this.ResultTiles = null;
                    this.ResultDirection = TileOrientation.TileDirection_None;
                    if (num19 < this.Painter.RoadBrushCount)
                    {
                        this.RoadTop = this.Terrain.SideH[this.PosNum.X, this.PosNum.Y].Road == this.Road;
                        this.RoadLeft = this.Terrain.SideV[this.PosNum.X, this.PosNum.Y].Road == this.Road;
                        this.RoadRight = this.Terrain.SideV[this.PosNum.X + 1, this.PosNum.Y].Road == this.Road;
                        this.RoadBottom = this.Terrain.SideH[this.PosNum.X, this.PosNum.Y + 1].Road == this.Road;
                        if (((this.RoadTop & this.RoadLeft) & this.RoadRight) & this.RoadBottom)
                        {
                            this.ResultTiles = this.Painter.RoadBrushes[num19].Tile_CrossIntersection;
                            this.ResultDirection = TileOrientation.TileDirection_None;
                        }
                        else if ((this.RoadTop & this.RoadLeft) & this.RoadRight)
                        {
                            this.ResultTiles = this.Painter.RoadBrushes[num19].Tile_TIntersection;
                            this.ResultDirection = TileOrientation.TileDirection_Top;
                        }
                        else if ((this.RoadTop & this.RoadLeft) & this.RoadBottom)
                        {
                            this.ResultTiles = this.Painter.RoadBrushes[num19].Tile_TIntersection;
                            this.ResultDirection = TileOrientation.TileDirection_Left;
                        }
                        else if ((this.RoadTop & this.RoadRight) & this.RoadBottom)
                        {
                            this.ResultTiles = this.Painter.RoadBrushes[num19].Tile_TIntersection;
                            this.ResultDirection = TileOrientation.TileDirection_Right;
                        }
                        else if ((this.RoadLeft & this.RoadRight) & this.RoadBottom)
                        {
                            this.ResultTiles = this.Painter.RoadBrushes[num19].Tile_TIntersection;
                            this.ResultDirection = TileOrientation.TileDirection_Bottom;
                        }
                        else if (this.RoadTop & this.RoadBottom)
                        {
                            this.ResultTiles = this.Painter.RoadBrushes[num19].Tile_Straight;
                            if (App.Random.Next() >= 0.5f)
                            {
                                this.ResultDirection = TileOrientation.TileDirection_Top;
                            }
                            else
                            {
                                this.ResultDirection = TileOrientation.TileDirection_Bottom;
                            }
                        }
                        else if (this.RoadLeft & this.RoadRight)
                        {
                            this.ResultTiles = this.Painter.RoadBrushes[num19].Tile_Straight;
                            if (App.Random.Next() >= 0.5f)
                            {
                                this.ResultDirection = TileOrientation.TileDirection_Left;
                            }
                            else
                            {
                                this.ResultDirection = TileOrientation.TileDirection_Right;
                            }
                        }
                        else if (this.RoadTop & this.RoadLeft)
                        {
                            this.ResultTiles = this.Painter.RoadBrushes[num19].Tile_Corner_In;
                            this.ResultDirection = TileOrientation.TileDirection_TopLeft;
                        }
                        else if (this.RoadTop & this.RoadRight)
                        {
                            this.ResultTiles = this.Painter.RoadBrushes[num19].Tile_Corner_In;
                            this.ResultDirection = TileOrientation.TileDirection_TopRight;
                        }
                        else if (this.RoadLeft & this.RoadBottom)
                        {
                            this.ResultTiles = this.Painter.RoadBrushes[num19].Tile_Corner_In;
                            this.ResultDirection = TileOrientation.TileDirection_BottomLeft;
                        }
                        else if (this.RoadRight & this.RoadBottom)
                        {
                            this.ResultTiles = this.Painter.RoadBrushes[num19].Tile_Corner_In;
                            this.ResultDirection = TileOrientation.TileDirection_BottomRight;
                        }
                        else if (this.RoadTop)
                        {
                            this.ResultTiles = this.Painter.RoadBrushes[num19].Tile_End;
                            this.ResultDirection = TileOrientation.TileDirection_Top;
                        }
                        else if (this.RoadLeft)
                        {
                            this.ResultTiles = this.Painter.RoadBrushes[num19].Tile_End;
                            this.ResultDirection = TileOrientation.TileDirection_Left;
                        }
                        else if (this.RoadRight)
                        {
                            this.ResultTiles = this.Painter.RoadBrushes[num19].Tile_End;
                            this.ResultDirection = TileOrientation.TileDirection_Right;
                        }
                        else if (this.RoadBottom)
                        {
                            this.ResultTiles = this.Painter.RoadBrushes[num19].Tile_End;
                            this.ResultDirection = TileOrientation.TileDirection_Bottom;
                        }
                    }
                }
                if (this.ResultTiles == null)
                {
                    this.ResultTexture.TextureNum = -1;
                    this.ResultTexture.Direction = TileOrientation.TileDirection_None;
                }
                else
                {
                    this.ResultTexture = this.ResultTiles.GetRandom();
                }
                if (this.ResultTexture.TextureNum < 0)
                {
                    if (this.MakeInvalidTiles)
                    {
                        this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].Texture = TileOrientation.OrientateTile(ref this.ResultTexture, this.ResultDirection);
                    }
                }
                else
                {
                    this.Terrain.Tiles[this.PosNum.X, this.PosNum.Y].Texture = TileOrientation.OrientateTile(ref this.ResultTexture, this.ResultDirection);
                }
                base.Map.SectorGraphicsChanges.TileChanged(base.PosNum);
                base.Map.SectorTerrainUndoChanges.TileChanged(base.PosNum);
            }
        }

        public class clsUpdateSectorGraphics : clsMap.clsAction
        {
            public override void ActionPerform()
            {
                base.Map.Sector_GLList_Make(this.PosNum.X, this.PosNum.Y);
                base.Map.MinimapMakeLater();
            }
        }

        public class clsUpdateSectorUnitHeights : clsMap.clsAction
        {
            private uint ID;
            private int NewAltitude;
            private clsMap.clsUnit NewUnit;
            private int OldUnitCount = 0;
            private clsMap.clsUnit[] OldUnits;
            private bool Started;

            public override void ActionPerform()
            {
                if (!this.Started)
                {
                    Debugger.Break();
                }
                else
                {
                    IEnumerator enumerator;
                    clsMap.clsSector sector = base.Map.Sectors[this.PosNum.X, this.PosNum.Y];
                    try
                    {
                        enumerator = sector.Units.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            clsMap.clsUnitSectorConnection current = (clsMap.clsUnitSectorConnection) enumerator.Current;
                            clsMap.clsUnit unit = current.Unit;
                            int num2 = this.OldUnitCount - 1;
                            int index = 0;
                            while (index <= num2)
                            {
                                if (this.OldUnits[index] == unit)
                                {
                                    break;
                                }
                                index++;
                            }
                            if (index == this.OldUnitCount)
                            {
                                this.OldUnits[this.OldUnitCount] = unit;
                                this.OldUnitCount++;
                            }
                        }
                    }
                    finally
                    {
                        if (enumerator is IDisposable)
                        {
                            (enumerator as IDisposable).Dispose();
                        }
                    }
                }
            }

            public void Finish()
            {
                if (!this.Started)
                {
                    Debugger.Break();
                }
                else
                {
                    clsMap.clsUnitAdd add = new clsMap.clsUnitAdd {
                        Map = base.Map,
                        StoreChange = true
                    };
                    int num2 = this.OldUnitCount - 1;
                    for (int i = 0; i <= num2; i++)
                    {
                        clsMap.clsUnit unitToCopy = this.OldUnits[i];
                        this.NewAltitude = (int) Math.Round(base.Map.GetTerrainHeight(unitToCopy.Pos.Horizontal));
                        if (this.NewAltitude != unitToCopy.Pos.Altitude)
                        {
                            this.NewUnit = new clsMap.clsUnit(unitToCopy, base.Map);
                            this.ID = unitToCopy.ID;
                            base.Map.UnitRemoveStoreChange(unitToCopy.MapLink.ArrayPosition);
                            add.NewUnit = this.NewUnit;
                            add.ID = this.ID;
                            add.Perform();
                            modProgram.ErrorIDChange(this.ID, this.NewUnit, "UpdateSectorUnitHeights");
                        }
                    }
                    this.Started = false;
                }
            }

            public void Start()
            {
                this.OldUnits = new clsMap.clsUnit[(base.Map.Units.Count - 1) + 1];
                this.Started = true;
            }
        }

        public class clsWZBJOUnit
        {
            public string Code;
            public uint ID;
            public clsUnitType.enumType ObjectType;
            public uint Player;
            public modProgram.sWorldPos Pos;
            public uint Rotation;
        }

        public class clsWZMapEntry
        {
            public string Name;
            public clsTileset Tileset;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct sCreateWZObjectsArgs
        {
            public modLists.SimpleClassList<clsMap.clsWZBJOUnit> BJOUnits;
            public clsMap.clsINIStructures INIStructures;
            public clsMap.clsINIDroids INIDroids;
            public clsMap.clsINIFeatures INIFeatures;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct sFMEUnit
        {
            public string Code;
            public uint ID;
            public int SavePriority;
            public byte LNDType;
            public uint X;
            public uint Y;
            public uint Z;
            public ushort Rotation;
            public string Name;
            public byte Player;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct sGenerateMasterTerrainArgs
        {
            public clsGeneratorTileset Tileset;
            public int LevelCount;
            public clsLayer[] Layers;
            public int LayerCount;
            public clsBooleanMap Watermap;
            public class clsLayer
            {
                public bool[] AvoidLayers;
                public float HeightMax;
                public float HeightMin;
                public bool IsCliff;
                public clsBooleanMap Terrainmap;
                public float TerrainmapDensity;
                public float TerrainmapScale;
                public int TileNum;
                public int WithinLayer;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct sLNDTile
        {
            public short Vertex0Height;
            public short Vertex1Height;
            public short Vertex2Height;
            public short Vertex3Height;
            public short TID;
            public short VF;
            public short TF;
            public short F;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct sWrite_WZ_Args
        {
            public string Path;
            public bool Overwrite;
            public string MapName;
            public clsMultiplayer Multiplayer;
            public clsCampaign Campaign;
            public modMath.sXY_int ScrollMin;
            public modMath.sXY_uint ScrollMax;
            public enumCompileType CompileType;
            public class clsCampaign
            {
                public uint GAMType;
            }

            public class clsMultiplayer
            {
                public string AuthorName;
                public bool IsBetaPlayerFormat;
                public string License;
                public int PlayerCount;
            }

            public enum enumCompileType : byte
            {
                Campaign = 2,
                Multiplayer = 1,
                Unspecified = 0
            }
        }
    }
}

