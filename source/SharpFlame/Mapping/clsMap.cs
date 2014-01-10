using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Matrix3D;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.Devices;
using OpenTK.Graphics.OpenGL;
using SharpFlame.AppSettings;
using SharpFlame.Collections;
using SharpFlame.Colors;
using SharpFlame.Domain;
using SharpFlame.FileIO;
using SharpFlame.Mapping.Changes;
using SharpFlame.Mapping.Objects;
using SharpFlame.Mapping.Renderers;
using SharpFlame.Mapping.Script;
using SharpFlame.Mapping.Tiles;
using SharpFlame.Mapping.Tools;
using SharpFlame.Maths;
using SharpFlame.Painters;

namespace SharpFlame.Mapping
{
    public partial class clsMap
    {
        public ConnectedListLink<clsMap, frmMain> frmMainLink;

        public clsTerrain Terrain;

        public clsSector[,] Sectors = new clsSector[0, 0];
        public sXY_int SectorCount;

        public clsShadowSector[,] ShadowSectors = new clsShadowSector[0, 0];

        public SimpleClassList<clsUndo> Undos;
        public int UndoPosition;

        public SimpleClassList<clsUnitChange> UnitChanges;
        public SimpleClassList<clsGatewayChange> GatewayChanges;

        public int HeightMultiplier = Constants.DefaultHeightMultiplier;

        public bool ReadyForUserInput
        {
            get { return _ReadyForUserInput; }
        }

        private bool _ReadyForUserInput = false;

        public ConnectedList<clsUnit, clsMap> SelectedUnits;
        public clsXY_int Selected_Tile_A;
        public clsXY_int Selected_Tile_B;
        public clsXY_int Selected_Area_VertexA;
        public clsXY_int Selected_Area_VertexB;
        public clsXY_int Unit_Selected_Area_VertexA;

        public int Minimap_GLTexture;
        public int Minimap_Texture_Size;

        public SimpleClassList<clsMessage> Messages;

        public clsTileset Tileset;

        public clsPathInfo PathInfo;

        public bool ChangedSinceSave = false;

        public delegate void ChangedEventHandler();

        private ChangedEventHandler ChangedEvent;

        public event ChangedEventHandler Changed
        {
            add { ChangedEvent = (ChangedEventHandler)Delegate.Combine(ChangedEvent, value); }
            remove { ChangedEvent = (ChangedEventHandler)Delegate.Remove(ChangedEvent, value); }
        }


        public clsAutoSave AutoSave = new clsAutoSave();

        public Painter Painter = new Painter();

        public byte[] Tile_TypeNum = new byte[0];

        public ConnectedList<clsGateway, clsMap> Gateways;

        public clsMap()
        {
            this.frmMainLink = new ConnectedListLink<clsMap, frmMain>( this );
            this.Sectors = new clsSector[0, 0];
            this.ShadowSectors = new clsShadowSector[0, 0];
            this.HeightMultiplier = 2;
            this._ReadyForUserInput = false;
            this.ChangedSinceSave = false;
            this.AutoSave = new clsAutoSave();
            this.Painter = new Painter();
            this.Tile_TypeNum = new byte[0];
            this.Gateways = new ConnectedList<clsGateway, clsMap>( this );
            this.Units = new ConnectedList<clsUnit, clsMap>( this );
            this.UnitGroups = new ConnectedList<clsUnitGroup, clsMap>( this );
            this.ScriptPositions = new ConnectedList<clsScriptPosition, clsMap>( this );
            this.ScriptAreas = new ConnectedList<clsScriptArea, clsMap>( this );

            Initialize();
        }

        public clsMap(sXY_int TileSize) : this()
        {
            Initialize();
            TerrainBlank(TileSize);
            TileType_Reset();
        }

        public void Initialize()
        {
            MakeMinimapTimer = new Timer();
            MakeMinimapTimer.Tick += MinimapTimer_Tick;
            MakeMinimapTimer.Interval = Constants.MinimapDelay;

            MakeDefaultUnitGroups();
            ScriptPositions.MaintainOrder = true;
            ScriptAreas.MaintainOrder = true;
        }

        public clsMap(clsMap MapToCopy, sXY_int Offset, sXY_int Area) : this()
        {
            int EndX = 0;
            int EndY = 0;
            int X = 0;
            int Y = 0;

            Initialize();

            //make some map data for selection

            EndX = Math.Min(MapToCopy.Terrain.TileSize.X - Offset.X, Area.X);
            EndY = Math.Min(MapToCopy.Terrain.TileSize.Y - Offset.Y, Area.Y);

            Terrain = new clsTerrain(Area);

            for ( Y = 0; Y <= Terrain.TileSize.Y - 1; Y++ )
            {
                for ( X = 0; X <= Terrain.TileSize.X - 1; X++ )
                {
                    Terrain.Tiles[X, Y].Texture.TextureNum = -1;
                }
            }

            for ( Y = 0; Y <= EndY; Y++ )
            {
                for ( X = 0; X <= EndX; X++ )
                {
                    Terrain.Vertices[X, Y].Height = MapToCopy.Terrain.Vertices[Offset.X + X, Offset.Y + Y].Height;
                    Terrain.Vertices[X, Y].Terrain = MapToCopy.Terrain.Vertices[Offset.X + X, Offset.Y + Y].Terrain;
                }
            }
            for ( Y = 0; Y <= EndY - 1; Y++ )
            {
                for ( X = 0; X <= EndX - 1; X++ )
                {
                    Terrain.Tiles[X, Y].Copy(MapToCopy.Terrain.Tiles[Offset.X + X, Offset.Y + Y]);
                }
            }
            for ( Y = 0; Y <= EndY; Y++ )
            {
                for ( X = 0; X <= EndX - 1; X++ )
                {
                    Terrain.SideH[X, Y].Road = MapToCopy.Terrain.SideH[Offset.X + X, Offset.Y + Y].Road;
                }
            }
            for ( Y = 0; Y <= EndY - 1; Y++ )
            {
                for ( X = 0; X <= EndX; X++ )
                {
                    Terrain.SideV[X, Y].Road = MapToCopy.Terrain.SideV[Offset.X + X, Offset.Y + Y].Road;
                }
            }

            SectorCount.X = (int)(Math.Ceiling((double)(Area.X / Constants.SectorTileSize)));
            SectorCount.Y = (int)(Math.Ceiling((double)(Area.Y / Constants.SectorTileSize)));
            Sectors = new clsSector[SectorCount.X, SectorCount.Y];
            for ( Y = 0; Y <= SectorCount.Y - 1; Y++ )
            {
                for ( X = 0; X <= SectorCount.X - 1; X++ )
                {
                    Sectors[X, Y] = new clsSector(new sXY_int(X, Y));
                }
            }

            sXY_int PosDif = new sXY_int();
            clsUnitAdd NewUnitAdd = new clsUnitAdd();
            NewUnitAdd.Map = this;
            clsUnit NewUnit = default(clsUnit);

            clsGateway Gateway = default(clsGateway);
            foreach ( clsGateway tempLoopVar_Gateway in MapToCopy.Gateways )
            {
                Gateway = tempLoopVar_Gateway;
                GatewayCreate(new sXY_int(Gateway.PosA.X - Offset.X, Gateway.PosA.Y - Offset.Y),
                    new sXY_int(Gateway.PosB.X - Offset.X, Gateway.PosB.Y - Offset.Y));
            }

            PosDif.X = - Offset.X * App.TerrainGridSpacing;
            PosDif.Y = - Offset.Y * App.TerrainGridSpacing;
            clsUnit Unit = default(clsUnit);
            sXY_int NewPos = new sXY_int();
            foreach ( clsUnit tempLoopVar_Unit in MapToCopy.Units )
            {
                Unit = tempLoopVar_Unit;
                NewPos = Unit.Pos.Horizontal + PosDif;
                if ( PosIsOnMap(NewPos) )
                {
                    NewUnit = new clsUnit(Unit, this);
                    NewUnit.Pos.Horizontal = NewPos;
                    NewUnitAdd.NewUnit = NewUnit;
                    NewUnitAdd.Label = Unit.Label;
                    NewUnitAdd.Perform();
                }
            }
        }

        protected void TerrainBlank(sXY_int TileSize)
        {
            int X = 0;
            int Y = 0;

            Terrain = new clsTerrain(TileSize);
            SectorCount.X = (int)(Math.Ceiling((double)(Terrain.TileSize.X / Constants.SectorTileSize)));
            SectorCount.Y = (int)(Math.Ceiling((double)(Terrain.TileSize.Y / Constants.SectorTileSize)));
            Sectors = new clsSector[SectorCount.X, SectorCount.Y];
            for ( Y = 0; Y <= SectorCount.Y - 1; Y++ )
            {
                for ( X = 0; X <= SectorCount.X - 1; X++ )
                {
                    Sectors[X, Y] = new clsSector(new sXY_int(X, Y));
                }
            }
        }

        public bool GetTerrainTri(sXY_int Horizontal)
        {
            int X1 = 0;
            int Y1 = 0;
            double InTileX = 0;
            double InTileZ = 0;
            int XG = 0;
            int YG = 0;

            XG = (int)(Conversion.Int(Horizontal.X / App.TerrainGridSpacing));
            YG = Conversion.Int(Horizontal.Y / App.TerrainGridSpacing);
            InTileX = MathUtil.Clamp_dbl(Horizontal.X / App.TerrainGridSpacing - XG, 0.0D, 1.0D);
            InTileZ = MathUtil.Clamp_dbl(Horizontal.Y / App.TerrainGridSpacing - YG, 0.0D, 1.0D);
            X1 = MathUtil.Clamp_int(XG, 0, Terrain.TileSize.X - 1);
            Y1 = MathUtil.Clamp_int(YG, 0, Terrain.TileSize.Y - 1);
            if ( Terrain.Tiles[X1, Y1].Tri )
            {
                if ( InTileZ <= 1.0D - InTileX )
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                if ( InTileZ <= InTileX )
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public double GetTerrainSlopeAngle(sXY_int Horizontal)
        {
            int X1 = 0;
            int X2 = 0;
            int Y1 = 0;
            int Y2 = 0;
            double InTileX = 0;
            double InTileZ = 0;
            int XG = 0;
            int YG = 0;
            double GradientX = 0;
            double GradientY = 0;
            double Offset = 0;
            Position.XYZ_dbl XYZ_dbl = default(Position.XYZ_dbl);
            Position.XYZ_dbl XYZ_dbl2 = default(Position.XYZ_dbl);
            Position.XYZ_dbl XYZ_dbl3 = default(Position.XYZ_dbl);
            Angles.AnglePY AnglePY = default(Angles.AnglePY);

            XG = Conversion.Int(Horizontal.X / App.TerrainGridSpacing);
            YG = (int)(Conversion.Int(Horizontal.Y / App.TerrainGridSpacing));
            InTileX = MathUtil.Clamp_dbl(Horizontal.X / App.TerrainGridSpacing - XG, 0.0D, 1.0D);
            InTileZ = MathUtil.Clamp_dbl(Horizontal.Y / App.TerrainGridSpacing - YG, 0.0D, 1.0D);
            X1 = MathUtil.Clamp_int(XG, 0, Terrain.TileSize.X - 1);
            Y1 = MathUtil.Clamp_int(YG, 0, Terrain.TileSize.Y - 1);
            X2 = MathUtil.Clamp_int(XG + 1, 0, Terrain.TileSize.X);
            Y2 = MathUtil.Clamp_int(YG + 1, 0, Terrain.TileSize.Y);
            if ( Terrain.Tiles[X1, Y1].Tri )
            {
                if ( InTileZ <= 1.0D - InTileX )
                {
                    Offset = Terrain.Vertices[X1, Y1].Height;
                    GradientX = Convert.ToDouble(Terrain.Vertices[X2, Y1].Height - Offset);
                    GradientY = Convert.ToDouble(Terrain.Vertices[X1, Y2].Height - Offset);
                }
                else
                {
                    Offset = Terrain.Vertices[X2, Y2].Height;
                    GradientX = Convert.ToDouble(Terrain.Vertices[X1, Y2].Height - Offset);
                    GradientY = Convert.ToDouble(Terrain.Vertices[X2, Y1].Height - Offset);
                }
            }
            else
            {
                if ( InTileZ <= InTileX )
                {
                    Offset = Terrain.Vertices[X2, Y1].Height;
                    GradientX = Convert.ToDouble(Terrain.Vertices[X1, Y1].Height - Offset);
                    GradientY = Convert.ToDouble(Terrain.Vertices[X2, Y2].Height - Offset);
                }
                else
                {
                    Offset = Terrain.Vertices[X1, Y2].Height;
                    GradientX = Convert.ToDouble(Terrain.Vertices[X2, Y2].Height - Offset);
                    GradientY = Convert.ToDouble(Terrain.Vertices[X1, Y1].Height - Offset);
                }
            }

            XYZ_dbl.X = App.TerrainGridSpacing;
            XYZ_dbl.Y = GradientX * HeightMultiplier;
            XYZ_dbl.Z = 0.0D;
            XYZ_dbl2.X = 0.0D;
            XYZ_dbl2.Y = GradientY * HeightMultiplier;
            XYZ_dbl2.Z = App.TerrainGridSpacing;
            Matrix3DMath.VectorCrossProduct(XYZ_dbl, XYZ_dbl2, ref XYZ_dbl3);
            if ( XYZ_dbl3.X != 0.0D | XYZ_dbl3.Z != 0.0D )
            {
                Matrix3DMath.VectorToPY(XYZ_dbl3, ref AnglePY);
                return MathUtil.RadOf90Deg - Math.Abs(AnglePY.Pitch);
            }
            else
            {
                return 0.0D;
            }
        }

        public double GetTerrainHeight(sXY_int Horizontal)
        {
            int X1 = 0;
            int X2 = 0;
            int Y1 = 0;
            int Y2 = 0;
            double InTileX = 0;
            double InTileZ = 0;
            int XG = 0;
            int YG = 0;
            double GradientX = 0;
            double GradientY = 0;
            double Offset = 0;
            double RatioX = 0;
            double RatioY = 0;

            XG = Conversion.Int(Horizontal.X / App.TerrainGridSpacing);
            YG = (int)(Conversion.Int(Horizontal.Y / App.TerrainGridSpacing));
            InTileX = MathUtil.Clamp_dbl(Horizontal.X / App.TerrainGridSpacing - XG, 0.0D, 1.0D);
            InTileZ = MathUtil.Clamp_dbl(Horizontal.Y / App.TerrainGridSpacing - YG, 0.0D, 1.0D);
            X1 = MathUtil.Clamp_int(XG, 0, Terrain.TileSize.X - 1);
            Y1 = MathUtil.Clamp_int(YG, 0, Terrain.TileSize.Y - 1);
            X2 = MathUtil.Clamp_int(XG + 1, 0, Terrain.TileSize.X);
            Y2 = MathUtil.Clamp_int(YG + 1, 0, Terrain.TileSize.Y);
            if ( Terrain.Tiles[X1, Y1].Tri )
            {
                if ( InTileZ <= 1.0D - InTileX )
                {
                    Offset = Terrain.Vertices[X1, Y1].Height;
                    GradientX = Convert.ToDouble(Terrain.Vertices[X2, Y1].Height - Offset);
                    GradientY = Convert.ToDouble(Terrain.Vertices[X1, Y2].Height - Offset);
                    RatioX = InTileX;
                    RatioY = InTileZ;
                }
                else
                {
                    Offset = Terrain.Vertices[X2, Y2].Height;
                    GradientX = Convert.ToDouble(Terrain.Vertices[X1, Y2].Height - Offset);
                    GradientY = Convert.ToDouble(Terrain.Vertices[X2, Y1].Height - Offset);
                    RatioX = 1.0D - InTileX;
                    RatioY = 1.0D - InTileZ;
                }
            }
            else
            {
                if ( InTileZ <= InTileX )
                {
                    Offset = Terrain.Vertices[X2, Y1].Height;
                    GradientX = Convert.ToDouble(Terrain.Vertices[X1, Y1].Height - Offset);
                    GradientY = Convert.ToDouble(Terrain.Vertices[X2, Y2].Height - Offset);
                    RatioX = 1.0D - InTileX;
                    RatioY = InTileZ;
                }
                else
                {
                    Offset = Terrain.Vertices[X1, Y2].Height;
                    GradientX = Convert.ToDouble(Terrain.Vertices[X2, Y2].Height - Offset);
                    GradientY = Convert.ToDouble(Terrain.Vertices[X1, Y1].Height - Offset);
                    RatioX = InTileX;
                    RatioY = 1.0D - InTileZ;
                }
            }
            return (Offset + GradientX * RatioX + GradientY * RatioY) * HeightMultiplier;
        }

        public sXYZ_sng TerrainVertexNormalCalc(int X, int Y)
        {
            sXYZ_sng ReturnResult = new sXYZ_sng();
            int TerrainHeightX1 = 0;
            int TerrainHeightX2 = 0;
            int TerrainHeightY1 = 0;
            int TerrainHeightY2 = 0;
            int X2 = 0;
            int Y2 = 0;
            Position.XYZ_dbl XYZ_dbl = default(Position.XYZ_dbl);
            Position.XYZ_dbl XYZ_dbl2 = default(Position.XYZ_dbl);
            double dblTemp = 0;

            X2 = MathUtil.Clamp_int(X - 1, 0, Terrain.TileSize.X);
            Y2 = MathUtil.Clamp_int(Y, 0, Terrain.TileSize.Y);
            TerrainHeightX1 = Terrain.Vertices[X2, Y2].Height;
            X2 = MathUtil.Clamp_int(X + 1, 0, Terrain.TileSize.X);
            Y2 = MathUtil.Clamp_int(Y, 0, Terrain.TileSize.Y);
            TerrainHeightX2 = Terrain.Vertices[X2, Y2].Height;
            X2 = MathUtil.Clamp_int(X, 0, Terrain.TileSize.X);
            Y2 = MathUtil.Clamp_int(Y - 1, 0, Terrain.TileSize.Y);
            TerrainHeightY1 = Terrain.Vertices[X2, Y2].Height;
            X2 = MathUtil.Clamp_int(X, 0, Terrain.TileSize.X);
            Y2 = MathUtil.Clamp_int(Y + 1, 0, Terrain.TileSize.Y);
            TerrainHeightY2 = Terrain.Vertices[X2, Y2].Height;
            XYZ_dbl.X = (TerrainHeightX1 - TerrainHeightX2) * HeightMultiplier;
            XYZ_dbl.Y = App.TerrainGridSpacing * 2.0D;
            XYZ_dbl.Z = 0.0D;
            XYZ_dbl2.X = 0.0D;
            XYZ_dbl2.Y = App.TerrainGridSpacing * 2.0D;
            XYZ_dbl2.Z = (TerrainHeightY1 - TerrainHeightY2) * HeightMultiplier;
            XYZ_dbl.X = XYZ_dbl.X + XYZ_dbl2.X;
            XYZ_dbl.Y = XYZ_dbl.Y + XYZ_dbl2.Y;
            XYZ_dbl.Z = XYZ_dbl.Z + XYZ_dbl2.Z;
            dblTemp = Math.Sqrt(XYZ_dbl.X * XYZ_dbl.X + XYZ_dbl.Y * XYZ_dbl.Y + XYZ_dbl.Z * XYZ_dbl.Z);
            ReturnResult.X = (float)(XYZ_dbl.X / dblTemp);
            ReturnResult.Y = (float)(XYZ_dbl.Y / dblTemp);
            ReturnResult.Z = (float)(XYZ_dbl.Z / dblTemp);
            return ReturnResult;
        }

        public void SectorAll_GLLists_Delete()
        {
            int X = 0;
            int Y = 0;

            for ( Y = 0; Y <= SectorCount.Y - 1; Y++ )
            {
                for ( X = 0; X <= SectorCount.X - 1; X++ )
                {
                    Sectors[X, Y].DeleteLists();
                }
            }
        }

        public virtual void Deallocate()
        {
            CancelUserInput();

            MakeMinimapTimer.Enabled = false;
            MakeMinimapTimer.Dispose();
            MakeMinimapTimer = null;
            MakeMinimapTimer.Tick += MinimapTimer_Tick;

            frmMainLink.Deallocate();
            frmMainLink = null;

            UnitGroups.Deallocate();
            UnitGroups = null;

            while ( Units.Count > 0 )
            {
                Units[0].Deallocate();
            }
            Units.Deallocate();
            Units = null;

            while ( Gateways.Count > 0 )
            {
                Gateways[0].Deallocate();
            }
            Gateways.Deallocate();
            Gateways = null;

            while ( ScriptPositions.Count > 0 )
            {
                ScriptPositions[0].Deallocate();
            }
            ScriptPositions.Deallocate();
            ScriptPositions = null;

            while ( ScriptAreas.Count > 0 )
            {
                ScriptAreas[0].Deallocate();
            }
            ScriptAreas.Deallocate();
            ScriptAreas = null;
        }

        public void TerrainResize(sXY_int Offset, sXY_int Size)
        {
            int StartX = 0;
            int StartY = 0;
            int EndX = 0;
            int EndY = 0;
            int X = 0;
            int Y = 0;
            clsTerrain NewTerrain = new clsTerrain(Size);

            StartX = Math.Max(0 - Offset.X, 0);
            StartY = Math.Max(0 - Offset.Y, 0);
            EndX = Math.Min(Terrain.TileSize.X - Offset.X, Size.X);
            EndY = Math.Min(Terrain.TileSize.Y - Offset.Y, Size.Y);

            for ( Y = 0; Y <= NewTerrain.TileSize.Y - 1; Y++ )
            {
                for ( X = 0; X <= NewTerrain.TileSize.X - 1; X++ )
                {
                    NewTerrain.Tiles[X, Y].Texture.TextureNum = -1;
                }
            }

            for ( Y = StartY; Y <= EndY; Y++ )
            {
                for ( X = StartX; X <= EndX; X++ )
                {
                    NewTerrain.Vertices[X, Y].Height = Terrain.Vertices[Offset.X + X, Offset.Y + Y].Height;
                    NewTerrain.Vertices[X, Y].Terrain = Terrain.Vertices[Offset.X + X, Offset.Y + Y].Terrain;
                }
            }
            for ( Y = StartY; Y <= EndY - 1; Y++ )
            {
                for ( X = StartX; X <= EndX - 1; X++ )
                {
                    NewTerrain.Tiles[X, Y].Copy(Terrain.Tiles[Offset.X + X, Offset.Y + Y]);
                }
            }
            for ( Y = StartY; Y <= EndY; Y++ )
            {
                for ( X = StartX; X <= EndX - 1; X++ )
                {
                    NewTerrain.SideH[X, Y].Road = Terrain.SideH[Offset.X + X, Offset.Y + Y].Road;
                }
            }
            for ( Y = StartY; Y <= EndY - 1; Y++ )
            {
                for ( X = StartX; X <= EndX; X++ )
                {
                    NewTerrain.SideV[X, Y].Road = Terrain.SideV[Offset.X + X, Offset.Y + Y].Road;
                }
            }

            int PosDifX = 0;
            int PosDifZ = 0;
            clsUnit Unit = default(clsUnit);
            clsGateway Gateway = default(clsGateway);

            PosDifX = - Offset.X * App.TerrainGridSpacing;
            PosDifZ = - Offset.Y * App.TerrainGridSpacing;
            foreach ( clsUnit tempLoopVar_Unit in Units )
            {
                Unit = tempLoopVar_Unit;
                Unit.Pos.Horizontal.X += PosDifX;
                Unit.Pos.Horizontal.Y += PosDifZ;
            }
            foreach ( clsGateway tempLoopVar_Gateway in Gateways )
            {
                Gateway = tempLoopVar_Gateway;
                Gateway.PosA.X -= Offset.X;
                Gateway.PosA.Y -= Offset.Y;
                Gateway.PosB.X -= Offset.X;
                Gateway.PosB.Y -= Offset.Y;
            }

            sXY_int ZeroPos = new sXY_int(0, 0);

            int Position = 0;
            foreach ( clsUnit tempLoopVar_Unit in Units.GetItemsAsSimpleList() )
            {
                Unit = tempLoopVar_Unit;
                Position = Unit.MapLink.ArrayPosition;
                if ( !App.PosIsWithinTileArea(Units[Position].Pos.Horizontal, ZeroPos, NewTerrain.TileSize) )
                {
                    UnitRemove(Position);
                }
            }

            Terrain = NewTerrain;

            foreach ( clsGateway tempLoopVar_Gateway in Gateways.GetItemsAsSimpleList() )
            {
                Gateway = tempLoopVar_Gateway;
                if ( Gateway.IsOffMap() )
                {
                    Gateway.Deallocate();
                }
            }

            sXY_int PosOffset = new sXY_int(Offset.X * App.TerrainGridSpacing, Offset.Y * App.TerrainGridSpacing);

            clsScriptPosition ScriptPosition = default(clsScriptPosition);
            foreach ( clsScriptPosition tempLoopVar_ScriptPosition in ScriptPositions.GetItemsAsSimpleList() )
            {
                ScriptPosition = tempLoopVar_ScriptPosition;
                ScriptPosition.MapResizing(PosOffset);
            }

            clsScriptArea ScriptArea = default(clsScriptArea);
            foreach ( clsScriptArea tempLoopVar_ScriptArea in ScriptAreas.GetItemsAsSimpleList() )
            {
                ScriptArea = tempLoopVar_ScriptArea;
                ScriptArea.MapResizing(PosOffset);
            }

            if ( _ReadyForUserInput )
            {
                CancelUserInput();
                InitializeUserInput();
            }
        }

        public void Sector_GLList_Make(int X, int Y)
        {
            int TileX = 0;
            int TileY = 0;
            int StartX = 0;
            int StartY = 0;
            int FinishX = 0;
            int FinishY = 0;

            Sectors[X, Y].DeleteLists();

            StartX = X * Constants.SectorTileSize;
            StartY = Y * Constants.SectorTileSize;
            FinishX = Math.Min(StartX + Constants.SectorTileSize, Terrain.TileSize.X) - 1;
            FinishY = Math.Min(StartY + Constants.SectorTileSize, Terrain.TileSize.Y) - 1;

            Sectors[X, Y].GLList_Textured = GL.GenLists(1);
            GL.NewList(Convert.ToInt32(Sectors[X, Y].GLList_Textured), ListMode.Compile);

            if ( App.Draw_Units )
            {
                bool[,] IsBasePlate = new bool[Constants.SectorTileSize, Constants.SectorTileSize];
                clsUnit Unit = default(clsUnit);
                StructureTypeBase structureTypeBase = default(StructureTypeBase);
                sXY_int Footprint = new sXY_int();
                clsUnitSectorConnection Connection = default(clsUnitSectorConnection);
                sXY_int FootprintStart = new sXY_int();
                sXY_int FootprintFinish = new sXY_int();
                foreach ( clsUnitSectorConnection tempLoopVar_Connection in Sectors[X, Y].Units )
                {
                    Connection = tempLoopVar_Connection;
                    Unit = Connection.Unit;
                    if ( Unit.TypeBase.Type == UnitType.PlayerStructure )
                    {
                        structureTypeBase = (StructureTypeBase)Unit.TypeBase;
                        if ( structureTypeBase.StructureBasePlate != null )
                        {
                            Footprint = structureTypeBase.get_GetFootprintSelected(Unit.Rotation);
                            GetFootprintTileRangeClamped(Unit.Pos.Horizontal, Footprint, FootprintStart, FootprintFinish);
                            for ( TileY = Math.Max(FootprintStart.Y, StartY); TileY <= Math.Min(FootprintFinish.Y, FinishY); TileY++ )
                            {
                                for ( TileX = Math.Max(FootprintStart.X, StartX); TileX <= Math.Min(FootprintFinish.X, FinishX); TileX++ )
                                {
                                    IsBasePlate[TileX - StartX, TileY - StartY] = true;
                                }
                            }
                        }
                    }
                }
                clsDrawTileOld drawTile = new clsDrawTileOld();
                drawTile.Map = this;
                for ( TileY = StartY; TileY <= FinishY; TileY++ )
                {
                    drawTile.TileY = TileY;
                    for ( TileX = StartX; TileX <= FinishX; TileX++ )
                    {
                        if ( !IsBasePlate[TileX - StartX, TileY - StartY] )
                        {
                            drawTile.TileX = TileX;
                            drawTile.Perform();
                        }
                    }
                }
            }
            else
            {
                clsDrawTileOld drawTile = new clsDrawTileOld();
                drawTile.Map = this;
                for ( TileY = StartY; TileY <= FinishY; TileY++ )
                {
                    drawTile.TileY = TileY;
                    for ( TileX = StartX; TileX <= FinishX; TileX++ )
                    {
                        drawTile.TileX = TileX;
                        drawTile.Perform();
                    }
                }
            }

            GL.EndList();

            Sectors[X, Y].GLList_Wireframe = GL.GenLists(1);
            GL.NewList(Sectors[X, Y].GLList_Wireframe, ListMode.Compile);

            for ( TileY = StartY; TileY <= FinishY; TileY++ )
            {
                for ( TileX = StartX; TileX <= FinishX; TileX++ )
                {
                    DrawTileWireframe(TileX, TileY);
                }
            }

            GL.EndList();
        }

        public void DrawTileWireframe(int TileX, int TileY)
        {
            double[] TileTerrainHeight = new double[4];
            sXYZ_sng Vertex0 = new sXYZ_sng();
            sXYZ_sng Vertex1 = new sXYZ_sng();
            sXYZ_sng Vertex2 = new sXYZ_sng();
            sXYZ_sng Vertex3 = new sXYZ_sng();

            TileTerrainHeight[0] = Terrain.Vertices[TileX, TileY].Height;
            TileTerrainHeight[1] = Terrain.Vertices[TileX + 1, TileY].Height;
            TileTerrainHeight[2] = Terrain.Vertices[TileX, TileY + 1].Height;
            TileTerrainHeight[3] = Terrain.Vertices[TileX + 1, TileY + 1].Height;

            Vertex0.X = TileX * App.TerrainGridSpacing;
            Vertex0.Y = (float)(TileTerrainHeight[0] * HeightMultiplier);
            Vertex0.Z = - TileY * App.TerrainGridSpacing;
            Vertex1.X = (TileX + 1) * App.TerrainGridSpacing;
            Vertex1.Y = (float)(TileTerrainHeight[1] * HeightMultiplier);
            Vertex1.Z = - TileY * App.TerrainGridSpacing;
            Vertex2.X = TileX * App.TerrainGridSpacing;
            Vertex2.Y = (float)(TileTerrainHeight[2] * HeightMultiplier);
            Vertex2.Z = - (TileY + 1) * App.TerrainGridSpacing;
            Vertex3.X = (TileX + 1) * App.TerrainGridSpacing;
            Vertex3.Y = (float)(TileTerrainHeight[3] * HeightMultiplier);
            Vertex3.Z = - (TileY + 1) * App.TerrainGridSpacing;

            GL.Begin(BeginMode.Lines);
            if ( Terrain.Tiles[TileX, TileY].Tri )
            {
                GL.Vertex3(Vertex0.X, Vertex0.Y, Convert.ToDouble(- Vertex0.Z));
                GL.Vertex3(Vertex2.X, Vertex2.Y, Convert.ToDouble(- Vertex2.Z));
                GL.Vertex3(Vertex2.X, Vertex2.Y, Convert.ToDouble(- Vertex2.Z));
                GL.Vertex3(Vertex1.X, Vertex1.Y, Convert.ToDouble(- Vertex1.Z));
                GL.Vertex3(Vertex1.X, Vertex1.Y, Convert.ToDouble(- Vertex1.Z));
                GL.Vertex3(Vertex0.X, Vertex0.Y, Convert.ToDouble(- Vertex0.Z));

                GL.Vertex3(Vertex1.X, Vertex1.Y, Convert.ToDouble(- Vertex1.Z));
                GL.Vertex3(Vertex2.X, Vertex2.Y, Convert.ToDouble(- Vertex2.Z));
                GL.Vertex3(Vertex2.X, Vertex2.Y, Convert.ToDouble(- Vertex2.Z));
                GL.Vertex3(Vertex3.X, Vertex3.Y, Convert.ToDouble(- Vertex3.Z));
                GL.Vertex3(Vertex3.X, Vertex3.Y, Convert.ToDouble(- Vertex3.Z));
                GL.Vertex3(Vertex1.X, Vertex1.Y, Convert.ToDouble(- Vertex1.Z));
            }
            else
            {
                GL.Vertex3(Vertex0.X, Vertex0.Y, Convert.ToDouble(- Vertex0.Z));
                GL.Vertex3(Vertex2.X, Vertex2.Y, Convert.ToDouble(- Vertex2.Z));
                GL.Vertex3(Vertex2.X, Vertex2.Y, Convert.ToDouble(- Vertex2.Z));
                GL.Vertex3(Vertex3.X, Vertex3.Y, Convert.ToDouble(- Vertex3.Z));
                GL.Vertex3(Vertex3.X, Vertex3.Y, Convert.ToDouble(- Vertex3.Z));
                GL.Vertex3(Vertex0.X, Vertex0.Y, Convert.ToDouble(- Vertex0.Z));

                GL.Vertex3(Vertex0.X, Vertex0.Y, Convert.ToDouble(- Vertex0.Z));
                GL.Vertex3(Vertex3.X, Vertex3.Y, Convert.ToDouble(- Vertex3.Z));
                GL.Vertex3(Vertex3.X, Vertex3.Y, Convert.ToDouble(- Vertex3.Z));
                GL.Vertex3(Vertex1.X, Vertex1.Y, Convert.ToDouble(- Vertex1.Z));
                GL.Vertex3(Vertex1.X, Vertex1.Y, Convert.ToDouble(- Vertex1.Z));
                GL.Vertex3(Vertex0.X, Vertex0.Y, Convert.ToDouble(- Vertex0.Z));
            }
            GL.End();
        }

        public void DrawTileOrientation(sXY_int Tile)
        {
            TileOrientation TileOrientation;
            sXY_int UnrotatedPos = new sXY_int();
            App.sWorldPos Vertex0 = new App.sWorldPos();
            App.sWorldPos Vertex1 = new App.sWorldPos();
            App.sWorldPos Vertex2 = new App.sWorldPos();

            TileOrientation = Terrain.Tiles[Tile.X, Tile.Y].Texture.Orientation;

            UnrotatedPos.X = 32;
            UnrotatedPos.Y = 32;
            Vertex0 = GetTileOffsetRotatedWorldPos(Tile, UnrotatedPos);

            UnrotatedPos.X = 64;
            UnrotatedPos.Y = 32;
            Vertex1 = GetTileOffsetRotatedWorldPos(Tile, UnrotatedPos);

            UnrotatedPos.X = 64;
            UnrotatedPos.Y = 64;
            Vertex2 = GetTileOffsetRotatedWorldPos(Tile, UnrotatedPos);

            GL.Vertex3(Vertex0.Horizontal.X, Vertex0.Altitude, Vertex0.Horizontal.Y);
            GL.Vertex3(Vertex1.Horizontal.X, Vertex1.Altitude, Vertex1.Horizontal.Y);
            GL.Vertex3(Vertex2.Horizontal.X, Vertex2.Altitude, Vertex2.Horizontal.Y);
        }

        protected void MinimapTextureFill(clsMinimapTexture Texture)
        {
            int X = 0;
            int Y = 0;
            sXY_int Low = new sXY_int();
            sXY_int High = new sXY_int();
            sXY_int Footprint = new sXY_int();
            bool Flag = default(bool);
            bool[,] UnitMap = new bool[Texture.Size.Y, Texture.Size.X];
            float[,,] sngTexture = new float[Texture.Size.Y, Texture.Size.X, 3];
            float Alpha = 0;
            float AntiAlpha = 0;
            sRGB_sng RGB_sng = new sRGB_sng();

            if ( Program.frmMainInstance.menuMiniShowTex.Checked )
            {
                if ( Tileset != null )
                {
                    for ( Y = 0; Y <= Terrain.TileSize.Y - 1; Y++ )
                    {
                        for ( X = 0; X <= Terrain.TileSize.X - 1; X++ )
                        {
                            if ( Terrain.Tiles[X, Y].Texture.TextureNum >= 0 && Terrain.Tiles[X, Y].Texture.TextureNum < Tileset.TileCount )
                            {
                                sngTexture[Y, X, 0] = Tileset.Tiles[Terrain.Tiles[X, Y].Texture.TextureNum].AverageColour.Red;
                                sngTexture[Y, X, 1] = Tileset.Tiles[Terrain.Tiles[X, Y].Texture.TextureNum].AverageColour.Green;
                                sngTexture[Y, X, 2] = Tileset.Tiles[Terrain.Tiles[X, Y].Texture.TextureNum].AverageColour.Blue;
                            }
                        }
                    }
                }
                if ( Program.frmMainInstance.menuMiniShowHeight.Checked )
                {
                    float Height = 0;
                    for ( Y = 0; Y <= Terrain.TileSize.Y - 1; Y++ )
                    {
                        for ( X = 0; X <= Terrain.TileSize.X - 1; X++ )
                        {
                            Height =
                                Convert.ToSingle(((Terrain.Vertices[X, Y].Height) + Terrain.Vertices[X + 1, Y].Height + Terrain.Vertices[X, Y + 1].Height +
                                                         Terrain.Vertices[X + 1, Y + 1].Height) / 1020.0F);
                            sngTexture[Y, X, 0] = (sngTexture[Y, X, 0] * 2.0F + Height) / 3.0F;
                            sngTexture[Y, X, 1] = (sngTexture[Y, X, 1] * 2.0F + Height) / 3.0F;
                            sngTexture[Y, X, 2] = (sngTexture[Y, X, 2] * 2.0F + Height) / 3.0F;
                        }
                    }
                }
            }
            else if ( Program.frmMainInstance.menuMiniShowHeight.Checked )
            {
                float Height = 0;
                for ( Y = 0; Y <= Terrain.TileSize.Y - 1; Y++ )
                {
                    for ( X = 0; X <= Terrain.TileSize.X - 1; X++ )
                    {
                        Height =
                            Convert.ToSingle(((Terrain.Vertices[X, Y].Height) + Terrain.Vertices[X + 1, Y].Height + Terrain.Vertices[X, Y + 1].Height +
                                                     Terrain.Vertices[X + 1, Y + 1].Height) / 1020.0F);
                        sngTexture[Y, X, 0] = Height;
                        sngTexture[Y, X, 1] = Height;
                        sngTexture[Y, X, 2] = Height;
                    }
                }
            }
            else
            {
                for ( Y = 0; Y <= Terrain.TileSize.Y - 1; Y++ )
                {
                    for ( X = 0; X <= Terrain.TileSize.X - 1; X++ )
                    {
                        sngTexture[Y, X, 0] = 0.0F;
                        sngTexture[Y, X, 1] = 0.0F;
                        sngTexture[Y, X, 2] = 0.0F;
                    }
                }
            }
            if ( Program.frmMainInstance.menuMiniShowCliffs.Checked )
            {
                if ( Tileset != null )
                {
                    Alpha = SettingsManager.Settings.MinimapCliffColour.Alpha;
                    AntiAlpha = 1.0F - Alpha;
                    for ( Y = 0; Y <= Terrain.TileSize.Y - 1; Y++ )
                    {
                        for ( X = 0; X <= Terrain.TileSize.X - 1; X++ )
                        {
                            if ( Terrain.Tiles[X, Y].Texture.TextureNum >= 0 && Terrain.Tiles[X, Y].Texture.TextureNum < Tileset.TileCount )
                            {
                                if ( Tileset.Tiles[Terrain.Tiles[X, Y].Texture.TextureNum].DefaultType == App.TileTypeNum_Cliff )
                                {
                                    sngTexture[Y, X, 0] = sngTexture[Y, X, 0] * AntiAlpha + SettingsManager.Settings.MinimapCliffColour.Red * Alpha;
                                    sngTexture[Y, X, 1] = sngTexture[Y, X, 1] * AntiAlpha + SettingsManager.Settings.MinimapCliffColour.Green * Alpha;
                                    sngTexture[Y, X, 2] = sngTexture[Y, X, 2] * AntiAlpha + SettingsManager.Settings.MinimapCliffColour.Blue * Alpha;
                                }
                            }
                        }
                    }
                }
            }
            if ( Program.frmMainInstance.menuMiniShowGateways.Checked )
            {
                clsGateway Gateway = default(clsGateway);
                foreach ( clsGateway tempLoopVar_Gateway in Gateways )
                {
                    Gateway = tempLoopVar_Gateway;
                    MathUtil.ReorderXY(Gateway.PosA, Gateway.PosB, Low, High);
                    for ( Y = Low.Y; Y <= High.Y; Y++ )
                    {
                        for ( X = Low.X; X <= High.X; X++ )
                        {
                            sngTexture[Y, X, 0] = 1.0F;
                            sngTexture[Y, X, 1] = 1.0F;
                            sngTexture[Y, X, 2] = 0.0F;
                        }
                    }
                }
            }
            if ( Program.frmMainInstance.menuMiniShowUnits.Checked )
            {
                //units that are not selected
                clsUnit Unit = default(clsUnit);
                foreach ( clsUnit tempLoopVar_Unit in Units )
                {
                    Unit = tempLoopVar_Unit;
                    Flag = true;
                    if ( Unit.TypeBase.UnitType_frmMainSelectedLink.IsConnected )
                    {
                        Flag = false;
                    }
                    else
                    {
                        Footprint = Unit.TypeBase.get_GetFootprintSelected(Unit.Rotation);
                    }
                    if ( Flag )
                    {
                        GetFootprintTileRangeClamped(Unit.Pos.Horizontal, Footprint, Low, High);
                        for ( Y = Low.Y; Y <= High.Y; Y++ )
                        {
                            for ( X = Low.X; X <= High.X; X++ )
                            {
                                if ( !UnitMap[Y, X] )
                                {
                                    UnitMap[Y, X] = true;
                                    if ( SettingsManager.Settings.MinimapTeamColours )
                                    {
                                        if ( SettingsManager.Settings.MinimapTeamColoursExceptFeatures & Unit.TypeBase.Type == UnitType.Feature )
                                        {
                                            sngTexture[Y, X, 0] = App.MinimapFeatureColour.Red;
                                            sngTexture[Y, X, 1] = App.MinimapFeatureColour.Green;
                                            sngTexture[Y, X, 2] = App.MinimapFeatureColour.Blue;
                                        }
                                        else
                                        {
                                            RGB_sng = GetUnitGroupMinimapColour(Unit.UnitGroup);
                                            sngTexture[Y, X, 0] = RGB_sng.Red;
                                            sngTexture[Y, X, 1] = RGB_sng.Green;
                                            sngTexture[Y, X, 2] = RGB_sng.Blue;
                                        }
                                    }
                                    else
                                    {
                                        sngTexture[Y, X, 0] = sngTexture[Y, X, 0] * 0.6666667F + 0.333333343F;
                                        sngTexture[Y, X, 1] = sngTexture[Y, X, 1] * 0.6666667F;
                                        sngTexture[Y, X, 2] = sngTexture[Y, X, 2] * 0.6666667F;
                                    }
                                }
                            }
                        }
                    }
                }
                //reset unit map
                for ( Y = 0; Y <= Texture.Size.Y - 1; Y++ )
                {
                    for ( X = 0; X <= Texture.Size.X - 1; X++ )
                    {
                        UnitMap[Y, X] = false;
                    }
                }
                //units that are selected and highlighted
                Alpha = SettingsManager.Settings.MinimapSelectedObjectsColour.Alpha;
                AntiAlpha = 1.0F - Alpha;
                foreach ( clsUnit tempLoopVar_Unit in Units )
                {
                    Unit = tempLoopVar_Unit;
                    Flag = false;
                    if ( Unit.TypeBase.UnitType_frmMainSelectedLink.IsConnected )
                    {
                        Flag = true;
                        Footprint = Unit.TypeBase.get_GetFootprintSelected(Unit.Rotation);
                        Footprint.X += 2;
                        Footprint.Y += 2;
                    }
                    if ( Flag )
                    {
                        GetFootprintTileRangeClamped(Unit.Pos.Horizontal, Footprint, Low, High);
                        for ( Y = Low.Y; Y <= High.Y; Y++ )
                        {
                            for ( X = Low.X; X <= High.X; X++ )
                            {
                                if ( !UnitMap[Y, X] )
                                {
                                    UnitMap[Y, X] = true;
                                    sngTexture[Y, X, 0] = sngTexture[Y, X, 0] * AntiAlpha + SettingsManager.Settings.MinimapSelectedObjectsColour.Red * Alpha;
                                    sngTexture[Y, X, 1] = sngTexture[Y, X, 1] * AntiAlpha + SettingsManager.Settings.MinimapSelectedObjectsColour.Green * Alpha;
                                    sngTexture[Y, X, 2] = sngTexture[Y, X, 2] * AntiAlpha + SettingsManager.Settings.MinimapSelectedObjectsColour.Blue * Alpha;
                                }
                            }
                        }
                    }
                }
            }
            for ( Y = 0; Y <= Terrain.TileSize.Y - 1; Y++ )
            {
                for ( X = 0; X <= Terrain.TileSize.X - 1; X++ )
                {
                    Texture.set_Pixels(X, Y, new sRGBA_sng(
                        sngTexture[Y, X, 0],
                        sngTexture[Y, X, 1],
                        sngTexture[Y, X, 2],
                        1.0F));
                }
            }
        }

        private bool MinimapPending;
        private Timer MakeMinimapTimer;
        public bool SuppressMinimap;

        private void MinimapTimer_Tick(object sender, EventArgs e)
        {
            if ( MainMap != this )
            {
                MinimapPending = false;
            }
            if ( MinimapPending )
            {
                if ( !SuppressMinimap )
                {
                    MinimapPending = false;
                    MinimapMake();
                }
            }
            else
            {
                MakeMinimapTimer.Enabled = false;
            }
        }

        public void MinimapMakeLater()
        {
            if ( MakeMinimapTimer.Enabled )
            {
                MinimapPending = true;
            }
            else
            {
                MakeMinimapTimer.Enabled = true;
                if ( SuppressMinimap )
                {
                    MinimapPending = true;
                }
                else
                {
                    MinimapMake();
                }
            }
        }

        private void MinimapMake()
        {
            int NewTextureSize =
                (int)
                    (Math.Round(
                        Convert.ToDouble(Math.Pow(2.0D, Math.Ceiling(Math.Log(Math.Max(Terrain.TileSize.X, Terrain.TileSize.Y)) / Math.Log(2.0D))))));

            if ( NewTextureSize != Minimap_Texture_Size )
            {
                Minimap_Texture_Size = NewTextureSize;
            }

            clsMinimapTexture Texture = new clsMinimapTexture(new sXY_int(Minimap_Texture_Size, Minimap_Texture_Size));

            MinimapTextureFill(Texture);

            MinimapGLDelete();

            GL.GenTextures(1, out Minimap_GLTexture);
            GL.BindTexture(TextureTarget.Texture2D, Minimap_GLTexture);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexImage2D<sRGBA_sng>(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Minimap_Texture_Size, Minimap_Texture_Size, 0, PixelFormat.Rgba,
                PixelType.Float, Texture.InlinePixels);

            Program.frmMainInstance.View_DrawViewLater();
        }

        public void MinimapGLDelete()
        {
            if ( Minimap_GLTexture != 0 )
            {
                GL.DeleteTextures(1, ref Minimap_GLTexture);
                Minimap_GLTexture = 0;
            }
        }

        public sXY_int GetTileSectorNum(sXY_int Tile)
        {
            sXY_int Result = new sXY_int();

            Result.X = Conversion.Int(Tile.X / Constants.SectorTileSize);
            Result.Y = Conversion.Int(Tile.Y / Constants.SectorTileSize);

            return Result;
        }

        public void GetTileSectorRange(sXY_int StartTile, sXY_int FinishTile, ref sXY_int ResultSectorStart,
            ref sXY_int ResultSectorFinish)
        {
            ResultSectorStart = GetTileSectorNum(StartTile);
            ResultSectorFinish = GetTileSectorNum(FinishTile);
            ResultSectorStart.X = MathUtil.Clamp_int(ResultSectorStart.X, 0, SectorCount.X - 1);
            ResultSectorStart.Y = MathUtil.Clamp_int(ResultSectorStart.Y, 0, SectorCount.Y - 1);
            ResultSectorFinish.X = MathUtil.Clamp_int(ResultSectorFinish.X, 0, SectorCount.X - 1);
            ResultSectorFinish.Y = MathUtil.Clamp_int(ResultSectorFinish.Y, 0, SectorCount.Y - 1);
        }

        public App.sWorldPos TileAlignedPos(sXY_int TileNum, sXY_int Footprint)
        {
            App.sWorldPos Result = new App.sWorldPos();

            Result.Horizontal.X = (int)((TileNum.X + Footprint.X / 2.0D) * App.TerrainGridSpacing);
            Result.Horizontal.Y = (int)((TileNum.Y + Footprint.Y / 2.0D) * App.TerrainGridSpacing);
            Result.Altitude = (int)(GetTerrainHeight(Result.Horizontal));

            return Result;
        }

        public App.sWorldPos TileAlignedPosFromMapPos(sXY_int Horizontal, sXY_int Footprint)
        {
            App.sWorldPos Result = new App.sWorldPos();

            Result.Horizontal.X =
                (int)
                    ((Math.Round(Convert.ToDouble((Horizontal.X - Footprint.X * App.TerrainGridSpacing / 2.0D) / App.TerrainGridSpacing)) +
                      Footprint.X / 2.0D) * App.TerrainGridSpacing);
            Result.Horizontal.Y =
                (int)
                    ((Math.Round(Convert.ToDouble((Horizontal.Y - Footprint.Y * App.TerrainGridSpacing / 2.0D) / App.TerrainGridSpacing)) +
                      Footprint.Y / 2.0D) * App.TerrainGridSpacing);
            Result.Altitude = (int)(GetTerrainHeight(Result.Horizontal));

            return Result;
        }

        public void UnitSectorsCalc(clsUnit Unit)
        {
            sXY_int Start = new sXY_int();
            sXY_int Finish = new sXY_int();
            sXY_int TileStart = new sXY_int();
            sXY_int TileFinish = new sXY_int();
            clsUnitSectorConnection Connection;
            int X = 0;
            int Y = 0;

            GetFootprintTileRangeClamped(Unit.Pos.Horizontal, Unit.TypeBase.get_GetFootprintSelected(Unit.Rotation), TileStart, TileFinish);
            Start = GetTileSectorNum(TileStart);
            Finish = GetTileSectorNum(TileFinish);
            Start.X = MathUtil.Clamp_int(Start.X, 0, SectorCount.X - 1);
            Start.Y = MathUtil.Clamp_int(Start.Y, 0, SectorCount.Y - 1);
            Finish.X = MathUtil.Clamp_int(Finish.X, 0, SectorCount.X - 1);
            Finish.Y = MathUtil.Clamp_int(Finish.Y, 0, SectorCount.Y - 1);
            Unit.Sectors.Clear();
            for ( Y = Start.Y; Y <= Finish.Y; Y++ )
            {
                for ( X = Start.X; X <= Finish.X; X++ )
                {
                    Connection = clsUnitSectorConnection.Create(Unit, Sectors[X, Y]);
                }
            }
        }

        public void AutoSaveTest()
        {
            if ( !SettingsManager.Settings.AutoSaveEnabled )
            {
                return;
            }
            if ( AutoSave.ChangeCount < SettingsManager.Settings.AutoSaveMinChanges )
            {
                return;
            }
            if (
                DateAndTime.DateDiff("s", AutoSave.SavedDate, DateTime.Now, (FirstDayOfWeek)FirstDayOfWeek.Sunday,
                    (FirstWeekOfYear)FirstWeekOfYear.Jan1) < SettingsManager.Settings.AutoSaveMinInterval_s )
            {
                return;
            }

            AutoSave.ChangeCount = 0;
            AutoSave.SavedDate = DateTime.Now;

            App.ShowWarnings(AutoSavePerform());
        }

        public clsResult AutoSavePerform()
        {
            clsResult ReturnResult = new clsResult("Autosave");

            if ( !Directory.Exists(App.AutoSavePath) )
            {
                try
                {
                    Directory.CreateDirectory(App.AutoSavePath);
                }
                catch ( Exception )
                {
                    ReturnResult.ProblemAdd("Unable to create directory at " + Convert.ToString(ControlChars.Quote) + App.AutoSavePath +
                                            Convert.ToString(ControlChars.Quote));
                }
            }

            DateTime DateNow = DateTime.Now;
            string Path = "";

            Path = App.AutoSavePath + "autosaved-" + DateNow.Year.ToStringInvariant() + "-" + App.MinDigits(DateNow.Month, 2) + "-" +
                   App.MinDigits(DateNow.Day, 2) + "-" + App.MinDigits(DateNow.Hour, 2) + "-" + App.MinDigits(DateNow.Minute, 2) + "-" +
                   App.MinDigits(DateNow.Second, 2) + "-" + App.MinDigits(DateNow.Millisecond, 3) + ".fmap";

            ReturnResult.Add(Write_FMap(Path, false, SettingsManager.Settings.AutoSaveCompress));

            return ReturnResult;
        }

        public void UndoStepCreate(string StepName)
        {
            clsUndo NewUndo = new clsUndo();

            NewUndo.Name = StepName;

            clsXY_int SectorNum = default(clsXY_int);
            foreach ( clsXY_int tempLoopVar_SectorNum in SectorTerrainUndoChanges.ChangedPoints )
            {
                SectorNum = tempLoopVar_SectorNum;
                NewUndo.ChangedSectors.Add(ShadowSectors[SectorNum.X, SectorNum.Y]);
                ShadowSector_Create(SectorNum.XY);
            }
            SectorTerrainUndoChanges.Clear();

            NewUndo.UnitChanges.AddSimpleList(UnitChanges);
            UnitChanges.Clear();

            NewUndo.GatewayChanges.AddSimpleList(GatewayChanges);
            GatewayChanges.Clear();

            if ( NewUndo.ChangedSectors.Count + NewUndo.UnitChanges.Count + NewUndo.GatewayChanges.Count > 0 )
            {
                while ( Undos.Count > UndoPosition ) //a new line has been started so remove redos
                {
                    Undos.Remove(Undos.Count - 1);
                }

                Undos.Add(NewUndo);
                UndoPosition = Undos.Count;

                SetChanged();
            }
        }

        public void ShadowSector_Create(sXY_int SectorNum)
        {
            int TileX = 0;
            int TileY = 0;
            int StartX = 0;
            int StartY = 0;
            int X = 0;
            int Y = 0;
            clsShadowSector Sector = default(clsShadowSector);
            int LastTileX = 0;
            int LastTileY = 0;

            Sector = new clsShadowSector();
            ShadowSectors[SectorNum.X, SectorNum.Y] = Sector;
            Sector.Num = SectorNum;
            StartX = SectorNum.X * Constants.SectorTileSize;
            StartY = SectorNum.Y * Constants.SectorTileSize;
            LastTileX = Math.Min(Constants.SectorTileSize, Terrain.TileSize.X - StartX);
            LastTileY = Math.Min(Constants.SectorTileSize, Terrain.TileSize.Y - StartY);
            for ( Y = 0; Y <= LastTileY; Y++ )
            {
                for ( X = 0; X <= LastTileX; X++ )
                {
                    TileX = StartX + X;
                    TileY = StartY + Y;
                    Sector.Terrain.Vertices[X, Y].Height = Terrain.Vertices[TileX, TileY].Height;
                    Sector.Terrain.Vertices[X, Y].Terrain = Terrain.Vertices[TileX, TileY].Terrain;
                }
            }
            for ( Y = 0; Y <= LastTileY - 1; Y++ )
            {
                for ( X = 0; X <= LastTileX - 1; X++ )
                {
                    TileX = StartX + X;
                    TileY = StartY + Y;
                    Sector.Terrain.Tiles[X, Y].Copy(Terrain.Tiles[TileX, TileY]);
                }
            }
            for ( Y = 0; Y <= LastTileY; Y++ )
            {
                for ( X = 0; X <= LastTileX - 1; X++ )
                {
                    TileX = StartX + X;
                    TileY = StartY + Y;
                    Sector.Terrain.SideH[X, Y].Road = Terrain.SideH[TileX, TileY].Road;
                }
            }
            for ( Y = 0; Y <= LastTileY - 1; Y++ )
            {
                for ( X = 0; X <= LastTileX; X++ )
                {
                    TileX = StartX + X;
                    TileY = StartY + Y;
                    Sector.Terrain.SideV[X, Y].Road = Terrain.SideV[TileX, TileY].Road;
                }
            }
        }

        public void UndoClear()
        {
            UndoStepCreate(""); //absorb current changes
            clsUnitChange UnitChange = default(clsUnitChange);
            clsUndo Undo = default(clsUndo);

            foreach ( clsUndo tempLoopVar_Undo in Undos )
            {
                Undo = tempLoopVar_Undo;
                foreach ( clsUnitChange tempLoopVar_UnitChange in Undo.UnitChanges )
                {
                    UnitChange = tempLoopVar_UnitChange;
                    UnitChange.Unit.Deallocate();
                }
            }

            Undos.Clear();
            UndoPosition = Undos.Count;
        }

        public void UndoPerform()
        {
            clsUndo ThisUndo = default(clsUndo);

            UndoStepCreate("Incomplete Action"); //make another redo step incase something has changed, such as if user presses undo while still dragging a tool

            UndoPosition--;

            ThisUndo = Undos[UndoPosition];

            sXY_int SectorNum = new sXY_int();
            clsShadowSector CurrentSector = default(clsShadowSector);
            clsShadowSector UndoSector = default(clsShadowSector);
            SimpleList<clsShadowSector> NewSectorsForThisUndo = new SimpleList<clsShadowSector>();
            foreach ( clsShadowSector tempLoopVar_UndoSector in ThisUndo.ChangedSectors )
            {
                UndoSector = tempLoopVar_UndoSector;
                SectorNum = UndoSector.Num;
                //store existing state for redo
                CurrentSector = ShadowSectors[SectorNum.X, SectorNum.Y];
                //remove graphics from sector
                Sectors[SectorNum.X, SectorNum.Y].DeleteLists();
                //perform the undo
                Undo_Sector_Rejoin(UndoSector);
                //update the backup
                ShadowSector_Create(SectorNum);
                //add old state to the redo step (that was this undo step)
                NewSectorsForThisUndo.Add(CurrentSector);
                //prepare to update graphics on this sector
                SectorGraphicsChanges.Changed(SectorNum);
            }
            ThisUndo.ChangedSectors = NewSectorsForThisUndo;

            UInt32 ID = 0;
            clsUnitAdd UnitAdd = new clsUnitAdd();
            UnitAdd.Map = this;
            clsUnit Unit = default(clsUnit);
            for ( int A = ThisUndo.UnitChanges.Count - 1; A >= 0; A-- ) //must do in reverse order, otherwise may try to delete units that havent been added yet
            {
                Unit = ThisUndo.UnitChanges[A].Unit;
                if ( ThisUndo.UnitChanges[A].Type == clsUnitChange.enumType.Added )
                {
                    //remove the unit from the map
                    UnitRemove(Unit.MapLink.ArrayPosition);
                }
                else if ( ThisUndo.UnitChanges[A].Type == clsUnitChange.enumType.Deleted )
                {
                    //add the unit back on to the map
                    ID = Unit.ID;
                    UnitAdd.ID = ID;
                    UnitAdd.NewUnit = Unit;
                    UnitAdd.Perform();
                    App.ErrorIDChange(ID, Unit, "Undo_Perform");
                }
                else
                {
                    Debugger.Break();
                }
            }

            clsGatewayChange GatewayChange = default(clsGatewayChange);
            for ( int A = ThisUndo.GatewayChanges.Count - 1; A >= 0; A-- )
            {
                GatewayChange = ThisUndo.GatewayChanges[A];
                switch ( GatewayChange.Type )
                {
                    case clsGatewayChange.enumType.Added:
                        //remove the unit from the map
                        GatewayChange.Gateway.MapLink.Disconnect();
                        break;
                    case clsGatewayChange.enumType.Deleted:
                        //add the unit back on to the map
                        GatewayChange.Gateway.MapLink.Connect(Gateways);
                        break;
                    default:
                        Debugger.Break();
                        break;
                }
            }

            SectorsUpdateGraphics();
            MinimapMakeLater();
            Program.frmMainInstance.SelectedObject_Changed();
        }

        public void RedoPerform()
        {
            clsUndo ThisUndo = default(clsUndo);

            ThisUndo = Undos[UndoPosition];

            sXY_int SectorNum = new sXY_int();
            clsShadowSector CurrentSector = default(clsShadowSector);
            clsShadowSector UndoSector = default(clsShadowSector);
            SimpleList<clsShadowSector> NewSectorsForThisUndo = new SimpleList<clsShadowSector>();
            foreach ( clsShadowSector tempLoopVar_UndoSector in ThisUndo.ChangedSectors )
            {
                UndoSector = tempLoopVar_UndoSector;
                SectorNum = UndoSector.Num;
                //store existing state for undo
                CurrentSector = ShadowSectors[SectorNum.X, SectorNum.Y];
                //remove graphics from sector
                Sectors[SectorNum.X, SectorNum.Y].DeleteLists();
                //perform the redo
                Undo_Sector_Rejoin(UndoSector);
                //update the backup
                ShadowSector_Create(SectorNum);
                //add old state to the undo step (that was this redo step)
                NewSectorsForThisUndo.Add(CurrentSector);
                //prepare to update graphics on this sector
                SectorGraphicsChanges.Changed(SectorNum);
            }
            ThisUndo.ChangedSectors = NewSectorsForThisUndo;

            UInt32 ID = 0;
            clsUnitAdd UnitAdd = new clsUnitAdd();
            UnitAdd.Map = this;
            clsUnit Unit = default(clsUnit);
            for ( int A = 0; A <= ThisUndo.UnitChanges.Count - 1; A++ ) //forward order is important
            {
                Unit = ThisUndo.UnitChanges[A].Unit;
                if ( ThisUndo.UnitChanges[A].Type == clsUnitChange.enumType.Added )
                {
                    //add the unit back on to the map
                    ID = Unit.ID;
                    UnitAdd.ID = ID;
                    UnitAdd.NewUnit = Unit;
                    UnitAdd.Perform();
                    App.ErrorIDChange(ID, Unit, "Redo_Perform");
                }
                else if ( ThisUndo.UnitChanges[A].Type == clsUnitChange.enumType.Deleted )
                {
                    //remove the unit from the map
                    UnitRemove(Unit.MapLink.ArrayPosition);
                }
                else
                {
                    Debugger.Break();
                }
            }

            clsGatewayChange GatewayChange = default(clsGatewayChange);
            for ( int A = 0; A <= ThisUndo.GatewayChanges.Count - 1; A++ ) //forward order is important
            {
                GatewayChange = ThisUndo.GatewayChanges[A];
                switch ( GatewayChange.Type )
                {
                    case clsGatewayChange.enumType.Added:
                        //add the unit back on to the map
                        GatewayChange.Gateway.MapLink.Connect(Gateways);
                        break;
                    case clsGatewayChange.enumType.Deleted:
                        //remove the unit from the map
                        GatewayChange.Gateway.MapLink.Disconnect();
                        break;
                    default:
                        Debugger.Break();
                        break;
                }
            }

            UndoPosition++;

            SectorsUpdateGraphics();
            MinimapMakeLater();
            Program.frmMainInstance.SelectedObject_Changed();
        }

        public void Undo_Sector_Rejoin(clsShadowSector Shadow_Sector_To_Rejoin)
        {
            int TileX = 0;
            int TileZ = 0;
            int StartX = 0;
            int StartZ = 0;
            int X = 0;
            int Y = 0;
            int LastTileX = 0;
            int LastTileZ = 0;

            StartX = Shadow_Sector_To_Rejoin.Num.X * Constants.SectorTileSize;
            StartZ = Shadow_Sector_To_Rejoin.Num.Y * Constants.SectorTileSize;
            LastTileX = Math.Min(Constants.SectorTileSize, Terrain.TileSize.X - StartX);
            LastTileZ = Math.Min(Constants.SectorTileSize, Terrain.TileSize.Y - StartZ);
            for ( Y = 0; Y <= LastTileZ; Y++ )
            {
                for ( X = 0; X <= LastTileX; X++ )
                {
                    TileX = StartX + X;
                    TileZ = StartZ + Y;
                    Terrain.Vertices[TileX, TileZ].Height = Shadow_Sector_To_Rejoin.Terrain.Vertices[X, Y].Height;
                    Terrain.Vertices[TileX, TileZ].Terrain = Shadow_Sector_To_Rejoin.Terrain.Vertices[X, Y].Terrain;
                }
            }
            for ( Y = 0; Y <= LastTileZ - 1; Y++ )
            {
                for ( X = 0; X <= LastTileX - 1; X++ )
                {
                    TileX = StartX + X;
                    TileZ = StartZ + Y;
                    Terrain.Tiles[TileX, TileZ].Copy(Shadow_Sector_To_Rejoin.Terrain.Tiles[X, Y]);
                }
            }
            for ( Y = 0; Y <= LastTileZ; Y++ )
            {
                for ( X = 0; X <= LastTileX - 1; X++ )
                {
                    TileX = StartX + X;
                    TileZ = StartZ + Y;
                    Terrain.SideH[TileX, TileZ].Road = Shadow_Sector_To_Rejoin.Terrain.SideH[X, Y].Road;
                }
            }
            for ( Y = 0; Y <= LastTileZ - 1; Y++ )
            {
                for ( X = 0; X <= LastTileX; X++ )
                {
                    TileX = StartX + X;
                    TileZ = StartZ + Y;
                    Terrain.SideV[TileX, TileZ].Road = Shadow_Sector_To_Rejoin.Terrain.SideV[X, Y].Road;
                }
            }
        }

        public void MapInsert(clsMap MapToInsert, sXY_int Offset, sXY_int Area, bool InsertHeights, bool InsertTextures, bool InsertUnits,
            bool DeleteUnits, bool InsertGateways, bool DeleteGateways)
        {
            sXY_int Finish = new sXY_int();
            int X = 0;
            int Y = 0;
            sXY_int SectorStart = new sXY_int();
            sXY_int SectorFinish = new sXY_int();
            sXY_int AreaAdjusted = new sXY_int();
            sXY_int SectorNum = new sXY_int();

            Finish.X = Math.Min(Offset.X + Math.Min(Area.X, MapToInsert.Terrain.TileSize.X), Terrain.TileSize.X);
            Finish.Y = Math.Min(Offset.Y + Math.Min(Area.Y, MapToInsert.Terrain.TileSize.Y), Terrain.TileSize.Y);
            AreaAdjusted.X = Finish.X - Offset.X;
            AreaAdjusted.Y = Finish.Y - Offset.Y;

            GetTileSectorRange(new sXY_int(Offset.X - 1, Offset.Y - 1), Finish, ref SectorStart, ref SectorFinish);
            for ( Y = SectorStart.Y; Y <= SectorFinish.Y; Y++ )
            {
                SectorNum.Y = Y;
                for ( X = SectorStart.X; X <= SectorFinish.X; X++ )
                {
                    SectorNum.X = X;
                    SectorGraphicsChanges.Changed(SectorNum);
                    SectorUnitHeightsChanges.Changed(SectorNum);
                    SectorTerrainUndoChanges.Changed(SectorNum);
                }
            }

            if ( InsertHeights )
            {
                for ( Y = 0; Y <= AreaAdjusted.Y; Y++ )
                {
                    for ( X = 0; X <= AreaAdjusted.X; X++ )
                    {
                        Terrain.Vertices[Offset.X + X, Offset.Y + Y].Height = MapToInsert.Terrain.Vertices[X, Y].Height;
                    }
                }
                for ( Y = 0; Y <= AreaAdjusted.Y - 1; Y++ )
                {
                    for ( X = 0; X <= AreaAdjusted.X - 1; X++ )
                    {
                        Terrain.Tiles[Offset.X + X, Offset.Y + Y].Tri = MapToInsert.Terrain.Tiles[X, Y].Tri;
                    }
                }
            }
            if ( InsertTextures )
            {
                for ( Y = 0; Y <= AreaAdjusted.Y; Y++ )
                {
                    for ( X = 0; X <= AreaAdjusted.X; X++ )
                    {
                        Terrain.Vertices[Offset.X + X, Offset.Y + Y].Terrain = MapToInsert.Terrain.Vertices[X, Y].Terrain;
                    }
                }
                bool TriDirection = default(bool);
                for ( Y = 0; Y <= AreaAdjusted.Y - 1; Y++ )
                {
                    for ( X = 0; X <= AreaAdjusted.X - 1; X++ )
                    {
                        TriDirection = Terrain.Tiles[Offset.X + X, Offset.Y + Y].Tri;
                        Terrain.Tiles[Offset.X + X, Offset.Y + Y].Copy(MapToInsert.Terrain.Tiles[X, Y]);
                        Terrain.Tiles[Offset.X + X, Offset.Y + Y].Tri = TriDirection;
                    }
                }
                for ( Y = 0; Y <= AreaAdjusted.Y; Y++ )
                {
                    for ( X = 0; X <= AreaAdjusted.X - 1; X++ )
                    {
                        Terrain.SideH[Offset.X + X, Offset.Y + Y].Road = MapToInsert.Terrain.SideH[X, Y].Road;
                    }
                }
                for ( Y = 0; Y <= AreaAdjusted.Y - 1; Y++ )
                {
                    for ( X = 0; X <= AreaAdjusted.X; X++ )
                    {
                        Terrain.SideV[Offset.X + X, Offset.Y + Y].Road = MapToInsert.Terrain.SideV[X, Y].Road;
                    }
                }
            }

            sXY_int LastTile = new sXY_int();
            LastTile = Finish;
            LastTile.X--;
            LastTile.Y--;
            if ( DeleteGateways )
            {
                int A = 0;
                A = 0;
                while ( A < Gateways.Count )
                {
                    if ( Gateways[A].PosA.IsInRange(Offset, LastTile) || Gateways[A].PosB.IsInRange(Offset, LastTile) )
                    {
                        GatewayRemoveStoreChange(A);
                    }
                    else
                    {
                        A++;
                    }
                }
            }
            if ( InsertGateways )
            {
                sXY_int GateStart = new sXY_int();
                sXY_int GateFinish = new sXY_int();
                clsGateway Gateway = default(clsGateway);
                foreach ( clsGateway tempLoopVar_Gateway in MapToInsert.Gateways )
                {
                    Gateway = tempLoopVar_Gateway;
                    GateStart.X = Offset.X + Gateway.PosA.X;
                    GateStart.Y = Offset.Y + Gateway.PosA.Y;
                    GateFinish.X = Offset.X + Gateway.PosB.X;
                    GateFinish.Y = Offset.Y + Gateway.PosB.Y;
                    if ( GateStart.IsInRange(Offset, LastTile) || GateFinish.IsInRange(Offset, LastTile) )
                    {
                        GatewayCreateStoreChange(GateStart, GateFinish);
                    }
                }
            }

            if ( DeleteUnits )
            {
                SimpleList<clsUnit> UnitsToDelete = new SimpleList<clsUnit>();
                int UnitToDeleteCount = 0;
                clsUnit Unit = default(clsUnit);
                for ( Y = SectorStart.Y; Y <= SectorFinish.Y; Y++ )
                {
                    for ( X = SectorStart.X; X <= SectorFinish.X; X++ )
                    {
                        clsUnitSectorConnection Connection = default(clsUnitSectorConnection);
                        foreach ( clsUnitSectorConnection tempLoopVar_Connection in Sectors[X, Y].Units )
                        {
                            Connection = tempLoopVar_Connection;
                            Unit = Connection.Unit;
                            if ( App.PosIsWithinTileArea(Unit.Pos.Horizontal, Offset, Finish) )
                            {
                                UnitsToDelete.Add(Unit);
                            }
                        }
                    }
                }
                foreach ( clsUnit tempLoopVar_Unit in UnitsToDelete )
                {
                    Unit = tempLoopVar_Unit;
                    if ( Unit.MapLink.IsConnected ) //units may be in the list multiple times and already be deleted
                    {
                        UnitRemoveStoreChange(Unit.MapLink.ArrayPosition);
                    }
                }
            }
            if ( InsertUnits )
            {
                sXY_int PosDif = new sXY_int();
                clsUnit NewUnit = default(clsUnit);
                clsUnit Unit = default(clsUnit);
                sXY_int ZeroPos = new sXY_int(0, 0);
                clsUnitAdd UnitAdd = new clsUnitAdd();

                UnitAdd.Map = this;
                UnitAdd.StoreChange = true;

                PosDif.X = Offset.X * App.TerrainGridSpacing;
                PosDif.Y = Offset.Y * App.TerrainGridSpacing;
                foreach ( clsUnit tempLoopVar_Unit in MapToInsert.Units )
                {
                    Unit = tempLoopVar_Unit;
                    if ( App.PosIsWithinTileArea(Unit.Pos.Horizontal, ZeroPos, AreaAdjusted) )
                    {
                        NewUnit = new clsUnit(Unit, this);
                        NewUnit.Pos.Horizontal.X += PosDif.X;
                        NewUnit.Pos.Horizontal.Y += PosDif.Y;
                        UnitAdd.NewUnit = NewUnit;
                        UnitAdd.Label = Unit.Label;
                        UnitAdd.Perform();
                    }
                }
            }

            SectorsUpdateGraphics();
            SectorsUpdateUnitHeights();
            MinimapMakeLater();
        }

        public clsGateway GatewayCreate(sXY_int PosA, sXY_int PosB)
        {
            if ( PosA.X >= 0 & PosA.X < Terrain.TileSize.X &
                 PosA.Y >= 0 & PosA.Y < Terrain.TileSize.Y &
                 PosB.X >= 0 & PosB.X < Terrain.TileSize.X &
                 PosB.Y >= 0 & PosB.Y < Terrain.TileSize.Y ) //is on map
            {
                if ( PosA.X == PosB.X | PosA.Y == PosB.Y ) //is straight
                {
                    clsGateway Gateway = new clsGateway();

                    Gateway.PosA = PosA;
                    Gateway.PosB = PosB;

                    Gateway.MapLink.Connect(Gateways);

                    return Gateway;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public clsGateway GatewayCreateStoreChange(sXY_int PosA, sXY_int PosB)
        {
            clsGateway Gateway = default(clsGateway);

            Gateway = GatewayCreate(PosA, PosB);

            clsGatewayChange GatewayChange = new clsGatewayChange();
            GatewayChange.Type = clsGatewayChange.enumType.Added;
            GatewayChange.Gateway = Gateway;
            GatewayChanges.Add(GatewayChange);

            return Gateway;
        }

        public void GatewayRemoveStoreChange(int Num)
        {
            clsGatewayChange GatewayChange = new clsGatewayChange();
            GatewayChange.Type = clsGatewayChange.enumType.Deleted;
            GatewayChange.Gateway = Gateways[Num];
            GatewayChanges.Add(GatewayChange);

            Gateways[Num].MapLink.Disconnect();
        }

        public void TileType_Reset()
        {
            if ( Tileset == null )
            {
                Tile_TypeNum = new byte[0];
            }
            else
            {
                int A = 0;

                Tile_TypeNum = new byte[Tileset.TileCount];
                for ( A = 0; A <= Tileset.TileCount - 1; A++ )
                {
                    Tile_TypeNum[A] = Tileset.Tiles[A].DefaultType;
                }
            }
        }

        public void SetPainterToDefaults()
        {
            if ( Tileset == null )
            {
                Painter = new Painter();
            }
            else if ( Tileset == App.Tileset_Arizona )
            {
                Painter = App.Painter_Arizona;
            }
            else if ( Tileset == App.Tileset_Urban )
            {
                Painter = App.Painter_Urban;
            }
            else if ( Tileset == App.Tileset_Rockies )
            {
                Painter = App.Painter_Rockies;
            }
            else
            {
                Painter = new Painter();
            }
        }

        internal void UnitSectorsGraphicsChanged(clsUnit UnitToUpdateFor)
        {
            if ( SectorGraphicsChanges == null )
            {
                Debugger.Break();
                return;
            }

            clsUnitSectorConnection Connection = default(clsUnitSectorConnection);

            foreach ( clsUnitSectorConnection tempLoopVar_Connection in UnitToUpdateFor.Sectors )
            {
                Connection = tempLoopVar_Connection;
                SectorGraphicsChanges.Changed(Connection.Sector.Pos);
            }
        }

        public App.sWorldPos GetTileOffsetRotatedWorldPos(sXY_int Tile, sXY_int TileOffsetToRotate)
        {
            App.sWorldPos Result = new App.sWorldPos();

            sXY_int RotatedOffset = new sXY_int();

            RotatedOffset = TileUtil.GetTileRotatedOffset(Terrain.Tiles[Tile.X, Tile.Y].Texture.Orientation, TileOffsetToRotate);
            Result.Horizontal.X = Tile.X * App.TerrainGridSpacing + RotatedOffset.X;
            Result.Horizontal.Y = Tile.Y * App.TerrainGridSpacing + RotatedOffset.Y;
            Result.Altitude = (int)(GetTerrainHeight(Result.Horizontal));

            return Result;
        }

        public void GetFootprintTileRangeClamped(sXY_int Horizontal, sXY_int Footprint, sXY_int ResultStart, sXY_int ResultFinish)
        {
            int Remainder = 0;
            sXY_int Centre = GetPosTileNum(Horizontal);
            int Half = 0;

            Half = Math.DivRem(Footprint.X, 2, out Remainder);
            ResultStart.X = MathUtil.Clamp_int(Centre.X - Half, 0, Terrain.TileSize.X - 1);
            ResultFinish.X = MathUtil.Clamp_int(ResultStart.X + Footprint.X - 1, 0, Terrain.TileSize.X - 1);
            Half = Math.DivRem(Footprint.Y, 2, out Remainder);
            ResultStart.Y = MathUtil.Clamp_int(Centre.Y - Half, 0, Terrain.TileSize.Y - 1);
            ResultFinish.Y = MathUtil.Clamp_int(ResultStart.Y + Footprint.Y - 1, 0, Terrain.TileSize.Y - 1);
        }

        public void GetFootprintTileRange(sXY_int Horizontal, sXY_int Footprint, sXY_int ResultStart, sXY_int ResultFinish)
        {
            int Remainder = 0;
            sXY_int Centre = GetPosTileNum(Horizontal);
            int Half = 0;

            Half = Math.DivRem(Footprint.X, 2, out Remainder);
            ResultStart.X = Centre.X - Half;
            ResultFinish.X = ResultStart.X + Footprint.X - 1;
            Half = Math.DivRem(Footprint.Y, 2, out Remainder);
            ResultStart.Y = Centre.Y - Half;
            ResultFinish.Y = ResultStart.Y + Footprint.Y - 1;
        }

        public sXY_int GetPosTileNum(sXY_int Horizontal)
        {
            sXY_int Result = new sXY_int();

            Result.X = (int)(Conversion.Int(Horizontal.X / App.TerrainGridSpacing));
            Result.Y = Conversion.Int(Horizontal.Y / App.TerrainGridSpacing);

            return Result;
        }

        public sXY_int GetPosVertexNum(sXY_int Horizontal)
        {
            sXY_int Result = new sXY_int();

            Result.X = (int)(Math.Round((double)(Horizontal.X / App.TerrainGridSpacing)));
            Result.Y = (int)(Math.Round((double)(Horizontal.Y / App.TerrainGridSpacing)));

            return Result;
        }

        public sXY_int GetPosSectorNum(sXY_int Horizontal)
        {
            sXY_int Result = new sXY_int();

            Result = GetTileSectorNum(GetPosTileNum(Horizontal));

            return Result;
        }

        public sXY_int GetSectorNumClamped(sXY_int SectorNum)
        {
            sXY_int Result = new sXY_int();

            Result.X = MathUtil.Clamp_int(SectorNum.X, 0, SectorCount.X - 1);
            Result.Y = MathUtil.Clamp_int(SectorNum.Y, 0, SectorCount.Y - 1);

            return Result;
        }

        public int GetVertexAltitude(sXY_int VertexNum)
        {
            return Terrain.Vertices[VertexNum.X, VertexNum.Y].Height * HeightMultiplier;
        }

        public bool PosIsOnMap(sXY_int Horizontal)
        {
            return App.PosIsWithinTileArea(Horizontal, new sXY_int(0, 0), Terrain.TileSize);
        }

        public sXY_int TileNumClampToMap(sXY_int TileNum)
        {
            sXY_int Result = new sXY_int();

            Result.X = MathUtil.Clamp_int(TileNum.X, 0, Terrain.TileSize.X - 1);
            Result.Y = MathUtil.Clamp_int(TileNum.Y, 0, Terrain.TileSize.Y - 1);

            return Result;
        }

        public frmCompile CompileScreen;

        public void CancelUserInput()
        {
            if ( !_ReadyForUserInput )
            {
                return;
            }

            _ReadyForUserInput = false;

            int X = 0;
            int Y = 0;

            if ( CompileScreen != null )
            {
                CompileScreen.Close();
                CompileScreen = null;
            }

            SectorAll_GLLists_Delete();
            MinimapGLDelete();

            ShadowSectors = null;
            for ( Y = 0; Y <= SectorCount.Y - 1; Y++ )
            {
                for ( X = 0; X <= SectorCount.X - 1; X++ )
                {
                    Sectors[X, Y].Deallocate();
                }
            }
            Sectors = null;
            SectorGraphicsChanges.Deallocate();
            SectorGraphicsChanges = null;
            SectorUnitHeightsChanges.Deallocate();
            SectorUnitHeightsChanges = null;
            SectorTerrainUndoChanges.Deallocate();
            SectorTerrainUndoChanges = null;
            AutoTextureChanges.Deallocate();
            AutoTextureChanges = null;
            TerrainInterpretChanges.Deallocate();
            TerrainInterpretChanges = null;

            UnitChanges = null;
            GatewayChanges = null;
            Undos = null;

            SelectedUnits.Deallocate();
            SelectedUnits = null;

            Selected_Tile_A = null;
            Selected_Tile_B = null;
            Selected_Area_VertexA = null;
            Selected_Area_VertexB = null;
            Unit_Selected_Area_VertexA = null;

            ViewInfo = null;

            _SelectedUnitGroup = null;

            Messages = null;
        }

        public void InitializeUserInput()
        {
            if ( _ReadyForUserInput )
            {
                return;
            }

            _ReadyForUserInput = true;

            int X = 0;
            int Y = 0;

            SectorCount.X = (int)(Math.Ceiling((double)(Terrain.TileSize.X / Constants.SectorTileSize)));
            SectorCount.Y = (int)(Math.Ceiling((double)(Terrain.TileSize.Y / Constants.SectorTileSize)));
            Sectors = new clsSector[SectorCount.X, SectorCount.Y];
            for ( Y = 0; Y <= SectorCount.Y - 1; Y++ )
            {
                for ( X = 0; X <= SectorCount.X - 1; X++ )
                {
                    Sectors[X, Y] = new clsSector(new sXY_int(X, Y));
                }
            }

            clsUnit Unit = default(clsUnit);
            foreach ( clsUnit tempLoopVar_Unit in Units )
            {
                Unit = tempLoopVar_Unit;
                UnitSectorsCalc(Unit);
            }

            ShadowSectors = new clsShadowSector[SectorCount.X, SectorCount.Y];
            for ( Y = 0; Y <= SectorCount.Y - 1; Y++ )
            {
                for ( X = 0; X <= SectorCount.X - 1; X++ )
                {
                    ShadowSector_Create(new sXY_int(X, Y));
                }
            }

            SectorGraphicsChanges = new clsSectorChanges(this);
            SectorGraphicsChanges.SetAllChanged();
            SectorUnitHeightsChanges = new clsSectorChanges(this);
            SectorTerrainUndoChanges = new clsSectorChanges(this);
            AutoTextureChanges = new clsAutoTextureChanges(this);
            TerrainInterpretChanges = new clsTerrainUpdate(Terrain.TileSize);

            UnitChanges = new SimpleClassList<clsUnitChange>();
            UnitChanges.MaintainOrder = true;
            GatewayChanges = new SimpleClassList<clsGatewayChange>();
            GatewayChanges.MaintainOrder = true;
            Undos = new SimpleClassList<clsUndo>();
            Undos.MaintainOrder = true;
            UndoPosition = 0;

            SelectedUnits = new ConnectedList<clsUnit, clsMap>(this);

            if ( InterfaceOptions == null )
            {
                InterfaceOptions = new clsInterfaceOptions();
            }

            ViewInfo = new clsViewInfo(this, Program.frmMainInstance.MapViewControl);

            _SelectedUnitGroup = new clsUnitGroupContainer();
            SelectedUnitGroup.Item = ScavengerUnitGroup;

            Messages = new SimpleClassList<clsMessage>();
            Messages.MaintainOrder = true;
        }

        public string GetDirectory()
        {
            if ( PathInfo == null )
            {
                return (new ServerComputer()).FileSystem.SpecialDirectories.MyDocuments;
            }
            else
            {
                App.sSplitPath SplitPath = new App.sSplitPath(PathInfo.Path);
                return SplitPath.FilePath;
            }
        }

        public void Update()
        {
            bool PrevSuppress = SuppressMinimap;

            SuppressMinimap = true;
            UpdateAutoTextures();
            TerrainInterpretUpdate();
            SectorsUpdateGraphics();
            SectorsUpdateUnitHeights();
            SuppressMinimap = PrevSuppress;
        }

        public void SectorsUpdateUnitHeights()
        {
            clsUpdateSectorUnitHeights UpdateSectorUnitHeights = new clsUpdateSectorUnitHeights();
            UpdateSectorUnitHeights.Map = this;

            UpdateSectorUnitHeights.Start();
            SectorUnitHeightsChanges.PerformTool(UpdateSectorUnitHeights);
            UpdateSectorUnitHeights.Finish();
            SectorUnitHeightsChanges.Clear();
        }

        public void SectorsUpdateGraphics()
        {
            clsUpdateSectorGraphics UpdateSectorGraphics = new clsUpdateSectorGraphics();
            UpdateSectorGraphics.Map = this;

            if ( MainMap == this )
            {
                SectorGraphicsChanges.PerformTool(UpdateSectorGraphics);
            }
            SectorGraphicsChanges.Clear();
        }

        public void UpdateAutoTextures()
        {
            clsUpdateAutotexture UpdateAutotextures = new clsUpdateAutotexture();
            UpdateAutotextures.Map = this;
            UpdateAutotextures.MakeInvalidTiles = Program.frmMainInstance.cbxInvalidTiles.Checked;

            AutoTextureChanges.PerformTool(UpdateAutotextures);
            AutoTextureChanges.Clear();
        }

        public void TerrainInterpretUpdate()
        {
            clsApplyVertexTerrainInterpret ApplyVertexInterpret = new clsApplyVertexTerrainInterpret();
            clsApplyTileTerrainInterpret ApplyTileInterpret = new clsApplyTileTerrainInterpret();
            clsApplySideHTerrainInterpret ApplySideHInterpret = new clsApplySideHTerrainInterpret();
            clsApplySideVTerrainInterpret ApplySideVInterpret = new clsApplySideVTerrainInterpret();
            ApplyVertexInterpret.Map = this;
            ApplyTileInterpret.Map = this;
            ApplySideHInterpret.Map = this;
            ApplySideVInterpret.Map = this;

            TerrainInterpretChanges.Vertices.PerformTool(ApplyVertexInterpret);
            TerrainInterpretChanges.Tiles.PerformTool(ApplyTileInterpret);
            TerrainInterpretChanges.SidesH.PerformTool(ApplySideHInterpret);
            TerrainInterpretChanges.SidesV.PerformTool(ApplySideVInterpret);
            TerrainInterpretChanges.ClearAll();
        }

        public void TileNeedsInterpreting(sXY_int Pos)
        {
            TerrainInterpretChanges.Tiles.Changed(Pos);
            TerrainInterpretChanges.Vertices.Changed(new sXY_int(Pos.X, Pos.Y));
            TerrainInterpretChanges.Vertices.Changed(new sXY_int(Pos.X + 1, Pos.Y));
            TerrainInterpretChanges.Vertices.Changed(new sXY_int(Pos.X, Pos.Y + 1));
            TerrainInterpretChanges.Vertices.Changed(new sXY_int(Pos.X + 1, Pos.Y + 1));
            TerrainInterpretChanges.SidesH.Changed(new sXY_int(Pos.X, Pos.Y));
            TerrainInterpretChanges.SidesH.Changed(new sXY_int(Pos.X, Pos.Y + 1));
            TerrainInterpretChanges.SidesV.Changed(new sXY_int(Pos.X, Pos.Y));
            TerrainInterpretChanges.SidesV.Changed(new sXY_int(Pos.X + 1, Pos.Y));
        }

        public void TileTextureChangeTerrainAction(sXY_int Pos, App.enumTextureTerrainAction Action)
        {
            switch ( Action )
            {
                case App.enumTextureTerrainAction.Ignore:
                    break;

                case App.enumTextureTerrainAction.Reinterpret:
                    TileNeedsInterpreting(Pos);
                    break;
                case App.enumTextureTerrainAction.Remove:
                    Terrain.Vertices[Pos.X, Pos.Y].Terrain = null;
                    Terrain.Vertices[Pos.X + 1, Pos.Y].Terrain = null;
                    Terrain.Vertices[Pos.X, Pos.Y + 1].Terrain = null;
                    Terrain.Vertices[Pos.X + 1, Pos.Y + 1].Terrain = null;
                    break;
            }
        }

        public clsInterfaceOptions InterfaceOptions;

        public string GetTitle()
        {
            string ReturnResult = "";

            if ( PathInfo == null )
            {
                ReturnResult = "Unsaved map";
            }
            else
            {
                App.sSplitPath SplitPath = new App.sSplitPath(PathInfo.Path);
                if ( PathInfo.IsFMap )
                {
                    ReturnResult = SplitPath.FileTitleWithoutExtension;
                }
                else
                {
                    ReturnResult = SplitPath.FileTitle;
                }
            }
            return ReturnResult;
        }

        public void SetChanged()
        {
            ChangedSinceSave = true;
            if ( ChangedEvent != null )
                ChangedEvent();

            AutoSave.ChangeCount++;
            AutoSaveTest();
        }

        public TabPage MapView_TabPage;

        public void SetTabText()
        {
            const int MaxLength = 24;

            string Result = "";
            Result = GetTitle();
            if ( Result.Length > MaxLength )
            {
                Result = Strings.Left(Result, MaxLength - 3) + "...";
            }
#if Mono
			MapView_TabPage.Text = Result + " ";
#else
            MapView_TabPage.Text = Result;
#endif
        }

        public bool SideHIsCliffOnBothSides(sXY_int SideNum)
        {
            sXY_int TileNum = new sXY_int();

            if ( SideNum.Y > 0 )
            {
                TileNum.X = SideNum.X;
                TileNum.Y = SideNum.Y - 1;
                if ( Terrain.Tiles[TileNum.X, TileNum.Y].Tri )
                {
                    if ( !Terrain.Tiles[TileNum.X, TileNum.Y].TriBottomRightIsCliff )
                    {
                        return false;
                    }
                }
                else
                {
                    if ( !Terrain.Tiles[TileNum.X, TileNum.Y].TriBottomLeftIsCliff )
                    {
                        return false;
                    }
                }
            }

            if ( SideNum.Y < Terrain.TileSize.Y )
            {
                TileNum.X = SideNum.X;
                TileNum.Y = SideNum.Y;
                if ( Terrain.Tiles[TileNum.X, TileNum.Y].Tri )
                {
                    if ( !Terrain.Tiles[TileNum.X, TileNum.Y].TriTopLeftIsCliff )
                    {
                        return false;
                    }
                }
                else
                {
                    if ( !Terrain.Tiles[TileNum.X, TileNum.Y].TriTopRightIsCliff )
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public bool SideVIsCliffOnBothSides(sXY_int SideNum)
        {
            sXY_int TileNum = new sXY_int();

            if ( SideNum.X > 0 )
            {
                TileNum.X = SideNum.X - 1;
                TileNum.Y = SideNum.Y;
                if ( Terrain.Tiles[TileNum.X, TileNum.Y].Tri )
                {
                    if ( !Terrain.Tiles[TileNum.X, TileNum.Y].TriBottomRightIsCliff )
                    {
                        return false;
                    }
                }
                else
                {
                    if ( !Terrain.Tiles[TileNum.X, TileNum.Y].TriTopRightIsCliff )
                    {
                        return false;
                    }
                }
            }

            if ( SideNum.X < Terrain.TileSize.X )
            {
                TileNum.X = SideNum.X;
                TileNum.Y = SideNum.Y;
                if ( Terrain.Tiles[TileNum.X, TileNum.Y].Tri )
                {
                    if ( !Terrain.Tiles[TileNum.X, TileNum.Y].TriTopLeftIsCliff )
                    {
                        return false;
                    }
                }
                else
                {
                    if ( !Terrain.Tiles[TileNum.X, TileNum.Y].TriBottomLeftIsCliff )
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public bool VertexIsCliffEdge(sXY_int VertexNum)
        {
            sXY_int TileNum = new sXY_int();

            if ( VertexNum.X > 0 )
            {
                if ( VertexNum.Y > 0 )
                {
                    TileNum.X = VertexNum.X - 1;
                    TileNum.Y = VertexNum.Y - 1;
                    if ( Terrain.Tiles[TileNum.X, TileNum.Y].Terrain_IsCliff )
                    {
                        return true;
                    }
                }
                if ( VertexNum.Y < Terrain.TileSize.Y )
                {
                    TileNum.X = VertexNum.X - 1;
                    TileNum.Y = VertexNum.Y;
                    if ( Terrain.Tiles[TileNum.X, TileNum.Y].Terrain_IsCliff )
                    {
                        return true;
                    }
                }
            }
            if ( VertexNum.X < Terrain.TileSize.X )
            {
                if ( VertexNum.Y > 0 )
                {
                    TileNum.X = VertexNum.X;
                    TileNum.Y = VertexNum.Y - 1;
                    if ( Terrain.Tiles[TileNum.X, TileNum.Y].Terrain_IsCliff )
                    {
                        return true;
                    }
                }
                if ( VertexNum.Y < Terrain.TileSize.Y )
                {
                    TileNum.X = VertexNum.X;
                    TileNum.Y = VertexNum.Y;
                    if ( Terrain.Tiles[TileNum.X, TileNum.Y].Terrain_IsCliff )
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void SelectedUnitsAction(clsObjectAction Tool)
        {
            clsObjectSelect SelectAction = new clsObjectSelect();

            SelectedUnits.GetItemsAsSimpleClassList().PerformTool(Tool);
            SelectedUnits.Clear();
            Tool.ResultUnits.PerformTool(SelectAction);
        }

        public bool CheckMessages()
        {
            int A = 0;
            DateTime DateNow = DateTime.Now;
            bool Changed = false;

            A = 0;
            while ( A < Messages.Count )
            {
                if (
                    DateAndTime.DateDiff(DateInterval.Second, Convert.ToDateTime(Messages[A].CreatedDate), DateNow,
                        (FirstDayOfWeek)FirstDayOfWeek.Sunday,
                        (FirstWeekOfYear)FirstWeekOfYear.Jan1) >= 6 )
                {
                    Messages.Remove(A);
                    Changed = true;
                }
                else
                {
                    A++;
                }
            }
            return Changed;
        }

        public clsMap MainMap
        {
            get
            {
                if ( !frmMainLink.IsConnected )
                {
                    return null;
                }
                else
                {
                    return frmMainLink.Source.MainMap;
                }
            }
        }

        public void PerformTileWall(clsWallType WallType, sXY_int TileNum, bool Expand)
        {
            sXY_int SectorNum = new sXY_int();
            clsUnit Unit = default(clsUnit);
            sXY_int UnitTile = new sXY_int();
            sXY_int Difference = new sXY_int();
            App.enumTileWalls TileWalls = App.enumTileWalls.None;
            SimpleList<clsUnit> Walls = new SimpleList<clsUnit>();
            SimpleList<clsUnit> Removals = new SimpleList<clsUnit>();
            UnitTypeBase unitTypeBase = default(UnitTypeBase);
            StructureTypeBase structureTypeBase = default(StructureTypeBase);
            int X = 0;
            int Y = 0;
            sXY_int MinTile = new sXY_int();
            sXY_int MaxTile = new sXY_int();
            clsUnitSectorConnection Connection = default(clsUnitSectorConnection);
            MinTile.X = TileNum.X - 1;
            MinTile.Y = TileNum.Y - 1;
            MaxTile.X = TileNum.X + 1;
            MaxTile.Y = TileNum.Y + 1;
            sXY_int SectorStart = GetSectorNumClamped(GetTileSectorNum(MinTile));
            sXY_int SectorFinish = GetSectorNumClamped(GetTileSectorNum(MaxTile));

            for ( Y = SectorStart.Y; Y <= SectorFinish.Y; Y++ )
            {
                for ( X = SectorStart.X; X <= SectorFinish.X; X++ )
                {
                    SectorNum.X = X;
                    SectorNum.Y = Y;
                    foreach ( clsUnitSectorConnection tempLoopVar_Connection in Sectors[SectorNum.X, SectorNum.Y].Units )
                    {
                        Connection = tempLoopVar_Connection;
                        Unit = Connection.Unit;
                        unitTypeBase = Unit.TypeBase;
                        if ( unitTypeBase.Type == UnitType.PlayerStructure )
                        {
                            structureTypeBase = (StructureTypeBase)unitTypeBase;
                            if ( structureTypeBase.WallLink.Source == WallType )
                            {
                                UnitTile = GetPosTileNum(Unit.Pos.Horizontal);
                                Difference.X = UnitTile.X - TileNum.X;
                                Difference.Y = UnitTile.Y - TileNum.Y;
                                if ( Difference.Y == 1 )
                                {
                                    if ( Difference.X == 0 )
                                    {
                                        TileWalls = (App.enumTileWalls)(TileWalls | App.enumTileWalls.Bottom);
                                        Walls.Add(Unit);
                                    }
                                }
                                else if ( Difference.Y == 0 )
                                {
                                    if ( Difference.X == 0 )
                                    {
                                        Removals.Add(Unit);
                                    }
                                    else if ( Difference.X == -1 )
                                    {
                                        TileWalls = (App.enumTileWalls)(TileWalls | App.enumTileWalls.Left);
                                        Walls.Add(Unit);
                                    }
                                    else if ( Difference.X == 1 )
                                    {
                                        TileWalls = (App.enumTileWalls)(TileWalls | App.enumTileWalls.Right);
                                        Walls.Add(Unit);
                                    }
                                }
                                else if ( Difference.Y == -1 )
                                {
                                    if ( Difference.X == 0 )
                                    {
                                        TileWalls = (App.enumTileWalls)(TileWalls | App.enumTileWalls.Top);
                                        Walls.Add(Unit);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            foreach ( clsUnit tempLoopVar_Unit in Removals )
            {
                Unit = tempLoopVar_Unit;
                UnitRemoveStoreChange(Unit.MapLink.ArrayPosition);
            }

            clsUnit NewUnit = new clsUnit();
            UnitTypeBase newUnitTypeBase = WallType.Segments[WallType.TileWalls_Segment[(int)TileWalls]];
            NewUnit.Rotation = WallType.TileWalls_Direction[(int)TileWalls];
            if ( Expand )
            {
                NewUnit.UnitGroup = SelectedUnitGroup.Item;
            }
            else
            {
                if ( Removals.Count == 0 )
                {
                    Debugger.Break();
                    return;
                }
                NewUnit.UnitGroup = Removals[0].UnitGroup;
            }
            NewUnit.Pos = TileAlignedPos(TileNum, new sXY_int(1, 1));
            NewUnit.TypeBase = newUnitTypeBase;
            clsUnitAdd UnitAdd = new clsUnitAdd();
            UnitAdd.Map = this;
            UnitAdd.NewUnit = NewUnit;
            UnitAdd.StoreChange = true;
            UnitAdd.Perform();

            if ( Expand )
            {
                clsUnit Wall = default(clsUnit);
                foreach ( clsUnit tempLoopVar_Wall in Walls )
                {
                    Wall = tempLoopVar_Wall;
                    PerformTileWall(WallType, GetPosTileNum(Wall.Pos.Horizontal), false);
                }
            }
        }

        public bool Save_FMap_Prompt()
        {
            SaveFileDialog Dialog = new SaveFileDialog();

            Dialog.InitialDirectory = GetDirectory();
            Dialog.FileName = "";
            Dialog.Filter = Constants.ProgramName + " Map Files (*.fmap)|*.fmap";
            if ( Dialog.ShowDialog(Program.frmMainInstance) != DialogResult.OK )
            {
                return false;
            }
            SettingsManager.Settings.SavePath = Path.GetDirectoryName(Dialog.FileName);
            clsResult Result = default(clsResult);
            Result = Write_FMap(Dialog.FileName, true, true);
            if ( !Result.HasProblems )
            {
                PathInfo = new clsPathInfo(Dialog.FileName, true);
                ChangedSinceSave = false;
            }
            App.ShowWarnings(Result);
            return !Result.HasProblems;
        }

        public bool Save_FMap_Quick()
        {
            if ( PathInfo == null )
            {
                return Save_FMap_Prompt();
            }
            else if ( PathInfo.IsFMap )
            {
                clsResult Result = Write_FMap(PathInfo.Path, true, true);
                if ( !Result.HasProblems )
                {
                    ChangedSinceSave = false;
                }
                App.ShowWarnings(Result);
                return !Result.HasProblems;
            }
            else
            {
                return Save_FMap_Prompt();
            }
        }

        public bool ClosePrompt()
        {
            if ( ChangedSinceSave )
            {
                frmClose Prompt = new frmClose(GetTitle());
                DialogResult Result = Prompt.ShowDialog(Program.frmMainInstance);
                switch ( Result )
                {
                    case DialogResult.OK:
                        return Save_FMap_Prompt();
                    case DialogResult.Yes:
                        return Save_FMap_Quick();
                    case DialogResult.No:
                        return true;
                    case DialogResult.Cancel:
                        return false;
                    default:
                        Debugger.Break();
                        return false;
                }
            }
            else
            {
                return true;
            }
        }
    }
}