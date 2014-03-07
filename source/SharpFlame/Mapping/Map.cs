﻿#region

using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Eto.Gl;
using Ninject;
using Ninject.Extensions.Logging;
using OpenTK.Graphics.OpenGL;
using SharpFlame.Core;
using SharpFlame.Core.Collections;
using SharpFlame.Core.Domain;
using SharpFlame.Core.Domain.Colors;
using SharpFlame.Domain;
using SharpFlame.Mapping.Changes;
using SharpFlame.Mapping.IO.FMap;
using SharpFlame.Mapping.Objects;
using SharpFlame.Mapping.Renderers;
using SharpFlame.Mapping.Script;
using SharpFlame.Mapping.Tiles;
using SharpFlame.Mapping.Tools;
using SharpFlame.Maths;
using SharpFlame.Painters;
using SharpFlame.Util;

#endregion

namespace SharpFlame.Mapping
{
    public partial class Map
    {
        public delegate void ChangedEventHandler();

        private readonly ILogger logger;
        public AutoSave AutoSave = new AutoSave();
        private ChangedEventHandler changedEvent;
        public bool ChangedSinceSave = false;
        public frmCompile CompileScreen;

        public SimpleClassList<GatewayChange> GatewayChanges;
        public ConnectedList<Gateway, Map> Gateways;

        public int HeightMultiplier = Constants.DefaultHeightMultiplier;
        public InterfaceOptions InterfaceOptions;
        private Timer makeMinimapTimer;
        public TabPage MapViewTabPage;
        public SimpleClassList<Message> Messages;
        private bool minimapPending;

        public int MinimapGlTexture;
        public int MinimapTextureSize;

        public Painter Painter = new Painter();
        public PathInfo PathInfo;
        public XYInt SectorCount;
        public Sector[,] Sectors = new Sector[0, 0];
        public ConnectedList<Unit, Map> SelectedUnits;
        public XYInt SelectedAreaVertexA;
        public XYInt SelectedAreaVertexB;
        public XYInt SelectedTileA;
        public XYInt SelectedTileB;
        public ShadowSector[,] ShadowSectors = new ShadowSector[0, 0];
        public bool SuppressMinimap;
        public clsTerrain Terrain;

        public byte[] TileTypeNum = new byte[0];
        public Tileset Tileset;
        public int UndoPosition;
        public SimpleClassList<Undo> Undos;
        public SimpleClassList<UnitChange> UnitChanges;
        public XYInt UnitSelectedAreaVertexA;
        private bool readyForUserInput;
        public ConnectedListLink<Map, frmMain> FrmMainLink;       
       
        public Map(ILoggerFactory logFactory)
        {
            logger = logFactory.GetCurrentClassLogger();

            SectorCount = new XYInt(0, 0);
            SelectedAreaVertexA = new XYInt(0, 0);
            SelectedAreaVertexB = new XYInt(0, 0);           

            FrmMainLink = new ConnectedListLink<Map, frmMain>(this);
            Sectors = new Sector[0, 0];
            ShadowSectors = new ShadowSector[0, 0];
            HeightMultiplier = 2;
            readyForUserInput = false;
            ChangedSinceSave = false;
            AutoSave = new AutoSave();
            Painter = new Painter();
            TileTypeNum = new byte[0];
            Gateways = new ConnectedList<Gateway, Map>(this);
            Units = new ConnectedList<Unit, Map>(this);
            UnitGroups = new ConnectedList<clsUnitGroup, Map>(this);
            ScriptPositions = new ConnectedList<clsScriptPosition, Map>(this);
            ScriptAreas = new ConnectedList<clsScriptArea, Map>(this);

            Initialize();
        }

        public Map Create(XYInt tileSize) {
            TerrainBlank(tileSize);
            TileType_Reset();

            return this;
        }

        public Map Copy(Map mapToCopy, XYInt offset, XYInt area)
        {
            var endX = 0;
            var endY = 0;
            var index0 = 0;
            var index1 = 0;

            //make some map data for selection

            endX = Math.Min(mapToCopy.Terrain.TileSize.X - offset.X, area.X);
            endY = Math.Min(mapToCopy.Terrain.TileSize.Y - offset.Y, area.Y);

            Terrain = new clsTerrain(area);

            for ( index1 = 0; index1 <= Terrain.TileSize.Y - 1; index1++ )
            {
                for ( index0 = 0; index0 <= Terrain.TileSize.X - 1; index0++ )
                {
                    Terrain.Tiles[index0, index1].Texture.TextureNum = -1;
                }
            }

            for ( index1 = 0; index1 <= endY; index1++ )
            {
                for ( index0 = 0; index0 <= endX; index0++ )
                {
                    Terrain.Vertices[index0, index1].Height = mapToCopy.Terrain.Vertices[offset.X + index0, offset.Y + index1].Height;
                    Terrain.Vertices[index0, index1].Terrain = mapToCopy.Terrain.Vertices[offset.X + index0, offset.Y + index1].Terrain;
                }
            }
            for ( index1 = 0; index1 <= endY - 1; index1++ )
            {
                for ( index0 = 0; index0 <= endX - 1; index0++ )
                {
                    Terrain.Tiles[index0, index1].Copy(mapToCopy.Terrain.Tiles[offset.X + index0, offset.Y + index1]);
                }
            }
            for ( index1 = 0; index1 <= endY; index1++ )
            {
                for ( index0 = 0; index0 <= endX - 1; index0++ )
                {
                    Terrain.SideH[index0, index1].Road = mapToCopy.Terrain.SideH[offset.X + index0, offset.Y + index1].Road;
                }
            }
            for ( index1 = 0; index1 <= endY - 1; index1++ )
            {
                for ( index0 = 0; index0 <= endX; index0++ )
                {
                    Terrain.SideV[index0, index1].Road = mapToCopy.Terrain.SideV[offset.X + index0, offset.Y + index1].Road;
                }
            }

            SectorCount.X = (int)(Math.Ceiling(((double)area.X / Constants.SectorTileSize)));
            SectorCount.Y = (int)(Math.Ceiling(((double)area.Y / Constants.SectorTileSize)));
            Sectors = new Sector[SectorCount.X, SectorCount.Y];
            for ( index1 = 0; index1 <= SectorCount.Y - 1; index1++ )
            {
                for ( index0 = 0; index0 <= SectorCount.X - 1; index0++ )
                {
                    Sectors[index0, index1] = new Sector(new XYInt(index0, index1));
                }
            }

            var posDif = new XYInt();
            var newUnitAdd = new clsUnitAdd();
            newUnitAdd.Map = this;

            foreach ( var gateway in mapToCopy.Gateways )
            {
                GatewayCreate(new XYInt(gateway.PosA.X - offset.X, gateway.PosA.Y - offset.Y),
                    new XYInt(gateway.PosB.X - offset.X, gateway.PosB.Y - offset.Y));
            }

            posDif.X = - offset.X * Constants.TerrainGridSpacing;
            posDif.Y = - offset.Y * Constants.TerrainGridSpacing;
            foreach ( var unit in mapToCopy.Units )
            {
                var newPos = unit.Pos.Horizontal + posDif;
                if ( PosIsOnMap(newPos) )
                {
                    var newUnit = new Unit(unit, this);
                    newUnit.Pos.Horizontal = newPos;
                    newUnitAdd.NewUnit = newUnit;
                    newUnitAdd.Label = unit.Label;
                    newUnitAdd.Perform();
                }
            }

            return this;
        }

        public bool ReadyForUserInput
        {
            get { return readyForUserInput; }
        }

        public Map MainMap
        {
            get
            {
                if ( !FrmMainLink.IsConnected )
                {
                    return null;
                }
                return FrmMainLink.Source.MainMap;
            }
        }

        public event ChangedEventHandler Changed
        {
            add { changedEvent = (ChangedEventHandler)Delegate.Combine(changedEvent, value); }
            remove { changedEvent = (ChangedEventHandler)Delegate.Remove(changedEvent, value); }
        }

        public void Initialize()
        {
            makeMinimapTimer = new Timer();
            makeMinimapTimer.Tick += MinimapTimer_Tick;
            makeMinimapTimer.Interval = Constants.MinimapDelay;

            MakeDefaultUnitGroups();
            ScriptPositions.MaintainOrder = true;
            ScriptAreas.MaintainOrder = true;
        }

        public void TerrainBlank(XYInt tileSize)
        {
            var index0 = 0;
            var index1 = 0;

            Terrain = new clsTerrain(tileSize);
            SectorCount.X = (int)(Math.Ceiling(((double)Terrain.TileSize.X / Constants.SectorTileSize)));
            SectorCount.Y = (int)(Math.Ceiling(((double)Terrain.TileSize.Y / Constants.SectorTileSize)));
            Sectors = new Sector[SectorCount.X, SectorCount.Y];
            for ( index1 = 0; index1 <= SectorCount.Y - 1; index1++ )
            {
                for ( index0 = 0; index0 <= SectorCount.X - 1; index0++ )
                {
                    Sectors[index0, index1] = new Sector(new XYInt(index0, index1));
                }
            }
        }

        public bool GetTerrainTri(XYInt horizontal)
        {
            var x1 = 0;
            var y1 = 0;
            double inTileX = 0;
            double inTileZ = 0;
            var xg = 0;
            var yg = 0;

            xg = horizontal.X / Constants.TerrainGridSpacing;
            yg = (horizontal.Y / Constants.TerrainGridSpacing);
            inTileX = MathUtil.ClampDbl(horizontal.X / Constants.TerrainGridSpacing - xg, 0.0D, 1.0D);
            inTileZ = MathUtil.ClampDbl(horizontal.Y / Constants.TerrainGridSpacing - yg, 0.0D, 1.0D);
            x1 = MathUtil.ClampInt(xg, 0, Terrain.TileSize.X - 1);
            y1 = MathUtil.ClampInt(yg, 0, Terrain.TileSize.Y - 1);
            if ( Terrain.Tiles[x1, y1].Tri )
            {
                if ( inTileZ <= 1.0D - inTileX )
                {
                    return false;
                }
                return true;
            }
            if ( inTileZ <= inTileX )
            {
                return true;
            }
            return false;
        }

        public double GetTerrainSlopeAngle(XYInt horizontal)
        {
            var X1 = 0;
            var X2 = 0;
            var Y1 = 0;
            var Y2 = 0;
            double InTileX = 0;
            double InTileZ = 0;
            var XG = 0;
            var YG = 0;
            double GradientX = 0;
            double GradientY = 0;
            double Offset = 0;
            var XYZ_dbl = default(XYZDouble);
            var XYZ_dbl2 = default(XYZDouble);
            var XYZ_dbl3 = default(XYZDouble);
            var AnglePY = default(Angles.AnglePY);

            XG = (horizontal.X / Constants.TerrainGridSpacing);
            YG = horizontal.Y / Constants.TerrainGridSpacing;
            InTileX = MathUtil.ClampDbl(horizontal.X / Constants.TerrainGridSpacing - XG, 0.0D, 1.0D);
            InTileZ = MathUtil.ClampDbl(horizontal.Y / Constants.TerrainGridSpacing - YG, 0.0D, 1.0D);
            X1 = MathUtil.ClampInt(XG, 0, Terrain.TileSize.X - 1);
            Y1 = MathUtil.ClampInt(YG, 0, Terrain.TileSize.Y - 1);
            X2 = MathUtil.ClampInt(XG + 1, 0, Terrain.TileSize.X);
            Y2 = MathUtil.ClampInt(YG + 1, 0, Terrain.TileSize.Y);
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

            XYZ_dbl.X = Constants.TerrainGridSpacing;
            XYZ_dbl.Y = GradientX * HeightMultiplier;
            XYZ_dbl.Z = 0.0D;
            XYZ_dbl2.X = 0.0D;
            XYZ_dbl2.Y = GradientY * HeightMultiplier;
            XYZ_dbl2.Z = Constants.TerrainGridSpacing;
            Matrix3DMath.VectorCrossProduct(XYZ_dbl, XYZ_dbl2, ref XYZ_dbl3);
            if ( XYZ_dbl3.X != 0.0D | XYZ_dbl3.Z != 0.0D )
            {
                Matrix3DMath.VectorToPY(XYZ_dbl3, ref AnglePY);
                return MathUtil.RadOf90Deg - Math.Abs(AnglePY.Pitch);
            }
            return 0.0D;
        }

        public double GetTerrainHeight(XYInt Horizontal)
        {
            var X1 = 0;
            var X2 = 0;
            var Y1 = 0;
            var Y2 = 0;
            double InTileX = 0;
            double InTileZ = 0;
            var XG = 0;
            var YG = 0;
            double GradientX = 0;
            double GradientY = 0;
            double Offset = 0;
            double RatioX = 0;
            double RatioY = 0;

            XG = (Horizontal.X / Constants.TerrainGridSpacing);
            YG = Horizontal.Y / Constants.TerrainGridSpacing;
            InTileX = MathUtil.ClampDbl(Horizontal.X / Constants.TerrainGridSpacing - XG, 0.0D, 1.0D);
            InTileZ = MathUtil.ClampDbl(Horizontal.Y / Constants.TerrainGridSpacing - YG, 0.0D, 1.0D);
            X1 = MathUtil.ClampInt(XG, 0, Terrain.TileSize.X - 1);
            Y1 = MathUtil.ClampInt(YG, 0, Terrain.TileSize.Y - 1);
            X2 = MathUtil.ClampInt(XG + 1, 0, Terrain.TileSize.X);
            Y2 = MathUtil.ClampInt(YG + 1, 0, Terrain.TileSize.Y);
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

        public XYZDouble TerrainVertexNormalCalc(int X, int Y)
        {
            var ReturnResult = default(XYZDouble);
            var TerrainHeightX1 = 0;
            var TerrainHeightX2 = 0;
            var TerrainHeightY1 = 0;
            var TerrainHeightY2 = 0;
            var X2 = 0;
            var Y2 = 0;
            var vector1 = default(XYZDouble);
            var vector2 = default(XYZDouble);
            double dblTemp = 0;

            X2 = MathUtil.ClampInt(X - 1, 0, Terrain.TileSize.X);
            Y2 = MathUtil.ClampInt(Y, 0, Terrain.TileSize.Y);
            TerrainHeightX1 = Terrain.Vertices[X2, Y2].Height;
            X2 = MathUtil.ClampInt(X + 1, 0, Terrain.TileSize.X);
            Y2 = MathUtil.ClampInt(Y, 0, Terrain.TileSize.Y);
            TerrainHeightX2 = Terrain.Vertices[X2, Y2].Height;
            X2 = MathUtil.ClampInt(X, 0, Terrain.TileSize.X);
            Y2 = MathUtil.ClampInt(Y - 1, 0, Terrain.TileSize.Y);
            TerrainHeightY1 = Terrain.Vertices[X2, Y2].Height;
            X2 = MathUtil.ClampInt(X, 0, Terrain.TileSize.X);
            Y2 = MathUtil.ClampInt(Y + 1, 0, Terrain.TileSize.Y);
            TerrainHeightY2 = Terrain.Vertices[X2, Y2].Height;
            vector1.X = (TerrainHeightX1 - TerrainHeightX2) * HeightMultiplier;
            vector1.Y = Constants.TerrainGridSpacing * 2.0D;
            vector1.Z = 0.0D;
            vector2.X = 0.0D;
            vector2.Y = Constants.TerrainGridSpacing * 2.0D;
            vector2.Z = (TerrainHeightY1 - TerrainHeightY2) * HeightMultiplier;
            vector1.X += vector2.X;
            vector1.Y += vector2.Y;
            vector1.Z += vector2.Z;
            //dblTemp = Math.Sqrt(vector1.X * vector1.X + vector1.Y * vector1.Y + vector1.Z * vector1.Z);
            dblTemp = vector1.GetMagnitude();
            ReturnResult.X = vector1.X / dblTemp;
            ReturnResult.Y = vector1.Y / dblTemp;
            ReturnResult.Z = vector1.Z / dblTemp;
            return ReturnResult;
        }

        public void SectorAll_GLLists_Delete()
        {
            var X = 0;
            var Y = 0;

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

            makeMinimapTimer.Enabled = false;
            makeMinimapTimer.Dispose();
            makeMinimapTimer = null;

            FrmMainLink.Deallocate();
            FrmMainLink = null;

            UnitGroups.Deallocate();
            UnitGroups = null;

            while ( Units.Count > 0 )
            {
                if (Units [0] != null) {
                    Units [0].Deallocate ();
                }
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

        public void TerrainResize(XYInt Offset, XYInt Size)
        {
            var StartX = 0;
            var StartY = 0;
            var EndX = 0;
            var EndY = 0;
            var X = 0;
            var Y = 0;
            var NewTerrain = new clsTerrain(Size);

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

            var PosDifX = 0;
            var PosDifZ = 0;
            var Unit = default(Unit);
            var Gateway = default(Gateway);

            PosDifX = - Offset.X * Constants.TerrainGridSpacing;
            PosDifZ = - Offset.Y * Constants.TerrainGridSpacing;
            foreach ( var tempLoopVar_Unit in Units )
            {
                Unit = tempLoopVar_Unit;
                Unit.Pos.Horizontal.X += PosDifX;
                Unit.Pos.Horizontal.Y += PosDifZ;
            }
            foreach ( var tempLoopVar_Gateway in Gateways )
            {
                Gateway = tempLoopVar_Gateway;
                Gateway.PosA.X -= Offset.X;
                Gateway.PosA.Y -= Offset.Y;
                Gateway.PosB.X -= Offset.X;
                Gateway.PosB.Y -= Offset.Y;
            }

            var ZeroPos = new XYInt(0, 0);

            var Position = 0;
            foreach ( var tempLoopVar_Unit in Units.GetItemsAsSimpleList() )
            {
                Unit = tempLoopVar_Unit;
                Position = Unit.MapLink.ArrayPosition;
                if ( !App.PosIsWithinTileArea(Units[Position].Pos.Horizontal, ZeroPos, NewTerrain.TileSize) )
                {
                    UnitRemove(Position);
                }
            }

            Terrain = NewTerrain;

            foreach ( var tempLoopVar_Gateway in Gateways.GetItemsAsSimpleList() )
            {
                Gateway = tempLoopVar_Gateway;
                if ( Gateway.IsOffMap() )
                {
                    Gateway.Deallocate();
                }
            }

            var PosOffset = new XYInt(Offset.X * Constants.TerrainGridSpacing, Offset.Y * Constants.TerrainGridSpacing);

            var ScriptPosition = default(clsScriptPosition);
            foreach ( var tempLoopVar_ScriptPosition in ScriptPositions.GetItemsAsSimpleList() )
            {
                ScriptPosition = tempLoopVar_ScriptPosition;
                ScriptPosition.MapResizing(PosOffset);
            }

            var ScriptArea = default(clsScriptArea);
            foreach ( var tempLoopVar_ScriptArea in ScriptAreas.GetItemsAsSimpleList() )
            {
                ScriptArea = tempLoopVar_ScriptArea;
                ScriptArea.MapResizing(PosOffset);
            }

            if ( readyForUserInput )
            {
                CancelUserInput();
                InitializeUserInput();
            }
        }

        public void Sector_GLList_Make(int X, int Y)
        {
            var TileX = 0;
            var TileY = 0;
            var StartX = 0;
            var StartY = 0;
            var FinishX = 0;
            var FinishY = 0;

            Sectors[X, Y].DeleteLists();

            StartX = X * Constants.SectorTileSize;
            StartY = Y * Constants.SectorTileSize;
            FinishX = Math.Min(StartX + Constants.SectorTileSize, Terrain.TileSize.X) - 1;
            FinishY = Math.Min(StartY + Constants.SectorTileSize, Terrain.TileSize.Y) - 1;

            Sectors[X, Y].GLList_Textured = GL.GenLists(1);
            GL.NewList(Convert.ToInt32(Sectors[X, Y].GLList_Textured), ListMode.Compile);

            if ( App.Draw_Units )
            {
                var IsBasePlate = new bool[Constants.SectorTileSize, Constants.SectorTileSize];
                var Unit = default(Unit);
                var structureTypeBase = default(StructureTypeBase);
                var Footprint = new XYInt();
                var Connection = default(clsUnitSectorConnection);
                var FootprintStart = new XYInt();
                var FootprintFinish = new XYInt();
                foreach ( var tempLoopVar_Connection in Sectors[X, Y].Units )
                {
                    Connection = tempLoopVar_Connection;
                    Unit = Connection.Unit;
                    if ( Unit.TypeBase.Type == UnitType.PlayerStructure )
                    {
                        structureTypeBase = (StructureTypeBase)Unit.TypeBase;
                        if ( structureTypeBase.StructureBasePlate != null )
                        {
                            Footprint = structureTypeBase.GetGetFootprintSelected(Unit.Rotation);
                            GetFootprintTileRangeClamped(Unit.Pos.Horizontal, Footprint, ref FootprintStart, ref FootprintFinish);
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
                clsDrawTile drawTile = new clsDrawTileMiddleVertex();
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
                clsDrawTile drawTile = new clsDrawTileMiddleVertex();
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
            var TileTerrainHeight = new double[4];
            var Vertex0 = new XYZDouble();
            var Vertex1 = new XYZDouble();
            var Vertex2 = new XYZDouble();
            var Vertex3 = new XYZDouble();

            TileTerrainHeight[0] = Terrain.Vertices[TileX, TileY].Height;
            TileTerrainHeight[1] = Terrain.Vertices[TileX + 1, TileY].Height;
            TileTerrainHeight[2] = Terrain.Vertices[TileX, TileY + 1].Height;
            TileTerrainHeight[3] = Terrain.Vertices[TileX + 1, TileY + 1].Height;

            Vertex0.X = TileX * Constants.TerrainGridSpacing;
            Vertex0.Y = (float)(TileTerrainHeight[0] * HeightMultiplier);
            Vertex0.Z = - TileY * Constants.TerrainGridSpacing;
            Vertex1.X = (TileX + 1) * Constants.TerrainGridSpacing;
            Vertex1.Y = (float)(TileTerrainHeight[1] * HeightMultiplier);
            Vertex1.Z = - TileY * Constants.TerrainGridSpacing;
            Vertex2.X = TileX * Constants.TerrainGridSpacing;
            Vertex2.Y = (float)(TileTerrainHeight[2] * HeightMultiplier);
            Vertex2.Z = - (TileY + 1) * Constants.TerrainGridSpacing;
            Vertex3.X = (TileX + 1) * Constants.TerrainGridSpacing;
            Vertex3.Y = (float)(TileTerrainHeight[3] * HeightMultiplier);
            Vertex3.Z = - (TileY + 1) * Constants.TerrainGridSpacing;

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

        public void DrawTileOrientation(XYInt Tile)
        {
            var UnrotatedPos = new XYInt();
            var Vertex0 = new WorldPos();
            var Vertex1 = new WorldPos();
            var Vertex2 = new WorldPos();

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

        public void MinimapTextureFill(clsMinimapTexture Texture)
        {
            var X = 0;
            var Y = 0;
            var Low = new XYInt();
            var High = new XYInt();
            var Footprint = new XYInt();
            var Flag = default(bool);
            var UnitMap = new bool[Texture.Size.Y, Texture.Size.X];
            var sngTexture = new float[Texture.Size.Y, Texture.Size.X, 3];
            float Alpha = 0;
            float AntiAlpha = 0;
            var RGB_sng = new SRgb();

            if ( Program.frmMainInstance.menuMiniShowTex.Checked )
            {
                if ( Tileset != null )
                {
                    for ( Y = 0; Y <= Terrain.TileSize.Y - 1; Y++ )
                    {
                        for ( X = 0; X <= Terrain.TileSize.X - 1; X++ )
                        {
                            if ( Terrain.Tiles[X, Y].Texture.TextureNum >= 0 && Terrain.Tiles[X, Y].Texture.TextureNum < Tileset.Tiles.Count )
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
                    Alpha = App.SettingsManager.MinimapCliffColour.Alpha;
                    AntiAlpha = 1.0F - Alpha;
                    for ( Y = 0; Y <= Terrain.TileSize.Y - 1; Y++ )
                    {
                        for ( X = 0; X <= Terrain.TileSize.X - 1; X++ )
                        {
                            if ( Terrain.Tiles[X, Y].Texture.TextureNum >= 0 && Terrain.Tiles[X, Y].Texture.TextureNum < Tileset.Tiles.Count )
                            {
                                if ( Tileset.Tiles[Terrain.Tiles[X, Y].Texture.TextureNum].DefaultType == Constants.TileTypeNumCliff )
                                {
                                    sngTexture[Y, X, 0] = sngTexture[Y, X, 0] * AntiAlpha + App.SettingsManager.MinimapCliffColour.Red * Alpha;
                                    sngTexture[Y, X, 1] = sngTexture[Y, X, 1] * AntiAlpha + App.SettingsManager.MinimapCliffColour.Green * Alpha;
                                    sngTexture[Y, X, 2] = sngTexture[Y, X, 2] * AntiAlpha + App.SettingsManager.MinimapCliffColour.Blue * Alpha;
                                }
                            }
                        }
                    }
                }
            }
            if ( Program.frmMainInstance.menuMiniShowGateways.Checked )
            {
                var Gateway = default(Gateway);
                foreach ( var tempLoopVar_Gateway in Gateways )
                {
                    Gateway = tempLoopVar_Gateway;
                    MathUtil.ReorderXY(Gateway.PosA, Gateway.PosB, ref Low, ref High);
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
                var Unit = default(Unit);
                foreach ( var tempLoopVar_Unit in Units )
                {
                    Unit = tempLoopVar_Unit;
                    Flag = true;
                    if ( Unit.TypeBase.UnitType_frmMainSelectedLink.IsConnected )
                    {
                        Flag = false;
                    }
                    else
                    {
                        Footprint = Unit.TypeBase.GetGetFootprintSelected(Unit.Rotation);
                    }
                    if ( Flag )
                    {
                        GetFootprintTileRangeClamped(Unit.Pos.Horizontal, Footprint, ref Low, ref High);
                        for ( Y = Low.Y; Y <= High.Y; Y++ )
                        {
                            for ( X = Low.X; X <= High.X; X++ )
                            {
                                if ( !UnitMap[Y, X] )
                                {
                                    UnitMap[Y, X] = true;
                                    if ( App.SettingsManager.MinimapTeamColours )
                                    {
                                        if ( App.SettingsManager.MinimapTeamColoursExceptFeatures & Unit.TypeBase.Type == UnitType.Feature )
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
                Alpha = App.SettingsManager.MinimapSelectedObjectsColour.Alpha;
                AntiAlpha = 1.0F - Alpha;
                foreach ( var tempLoopVar_Unit in Units )
                {
                    Unit = tempLoopVar_Unit;
                    Flag = false;
                    if ( Unit.TypeBase.UnitType_frmMainSelectedLink.IsConnected )
                    {
                        Flag = true;
                        Footprint = Unit.TypeBase.GetGetFootprintSelected(Unit.Rotation);
                        Footprint.X += 2;
                        Footprint.Y += 2;
                    }
                    if ( Flag )
                    {
                        GetFootprintTileRangeClamped(Unit.Pos.Horizontal, Footprint, ref Low, ref High);
                        for ( Y = Low.Y; Y <= High.Y; Y++ )
                        {
                            for ( X = Low.X; X <= High.X; X++ )
                            {
                                if ( !UnitMap[Y, X] )
                                {
                                    UnitMap[Y, X] = true;
                                    sngTexture[Y, X, 0] = sngTexture[Y, X, 0] * AntiAlpha + App.SettingsManager.MinimapSelectedObjectsColour.Red * Alpha;
                                    sngTexture[Y, X, 1] = sngTexture[Y, X, 1] * AntiAlpha + App.SettingsManager.MinimapSelectedObjectsColour.Green * Alpha;
                                    sngTexture[Y, X, 2] = sngTexture[Y, X, 2] * AntiAlpha + App.SettingsManager.MinimapSelectedObjectsColour.Blue * Alpha;
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
                    Texture.set_Pixels(X, Y, new SRgba(
                        sngTexture[Y, X, 0],
                        sngTexture[Y, X, 1],
                        sngTexture[Y, X, 2],
                        1.0F));
                }
            }
        }

        private void MinimapTimer_Tick(object sender, EventArgs e)
        {
            if ( MainMap != this )
            {
                minimapPending = false;
            }
            if ( minimapPending )
            {
                if ( !SuppressMinimap )
                {
                    minimapPending = false;
                    MinimapMake();
                }
            }
            else
            {
                makeMinimapTimer.Enabled = false;
            }
        }

        public void MinimapMakeLater()
        {
            if ( makeMinimapTimer.Enabled )
            {
                minimapPending = true;
            }
            else
            {
                makeMinimapTimer.Enabled = true;
                if ( SuppressMinimap )
                {
                    minimapPending = true;
                }
                else
                {
                    MinimapMake();
                }
            }
        }

        private void MinimapMake()
        {
            var newTextureSize =
                (int)
                    (Math.Round(
                        Convert.ToDouble(Math.Pow(2.0D, Math.Ceiling(Math.Log(Math.Max(Terrain.TileSize.X, Terrain.TileSize.Y)) / Math.Log(2.0D))))));

            if ( newTextureSize != MinimapTextureSize )
            {
                MinimapTextureSize = newTextureSize;
            }

            var texture = new clsMinimapTexture(new XYInt(MinimapTextureSize, MinimapTextureSize));

            MinimapTextureFill(texture);

            MinimapGLDelete();

            GL.GenTextures(1, out MinimapGlTexture);
            GL.BindTexture(TextureTarget.Texture2D, MinimapGlTexture);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, MinimapTextureSize, MinimapTextureSize, 0, PixelFormat.Rgba,
                PixelType.Float, texture.InlinePixels);

            Program.frmMainInstance.View_DrawViewLater();
        }

        public void MinimapGLDelete()
        {
            if ( MinimapGlTexture != 0 )
            {
                GL.DeleteTextures(1, ref MinimapGlTexture);
                MinimapGlTexture = 0;
            }
        }

        public XYInt GetTileSectorNum(XYInt Tile)
        {
            var Result = new XYInt();

            Result.X = (Tile.X / Constants.SectorTileSize);
            Result.Y = (Tile.Y / Constants.SectorTileSize);

            return Result;
        }

        public void GetTileSectorRange(XYInt StartTile, XYInt FinishTile, ref XYInt ResultSectorStart,
            ref XYInt ResultSectorFinish)
        {
            ResultSectorStart = GetTileSectorNum(StartTile);
            ResultSectorFinish = GetTileSectorNum(FinishTile);
            ResultSectorStart.X = MathUtil.ClampInt(ResultSectorStart.X, 0, SectorCount.X - 1);
            ResultSectorStart.Y = MathUtil.ClampInt(ResultSectorStart.Y, 0, SectorCount.Y - 1);
            ResultSectorFinish.X = MathUtil.ClampInt(ResultSectorFinish.X, 0, SectorCount.X - 1);
            ResultSectorFinish.Y = MathUtil.ClampInt(ResultSectorFinish.Y, 0, SectorCount.Y - 1);
        }

        public WorldPos TileAlignedPos(XYInt TileNum, XYInt Footprint)
        {
            var Result = new WorldPos();

            Result.Horizontal.X = (int)((TileNum.X + Footprint.X / 2.0D) * Constants.TerrainGridSpacing);
            Result.Horizontal.Y = (int)((TileNum.Y + Footprint.Y / 2.0D) * Constants.TerrainGridSpacing);
            Result.Altitude = (int)(GetTerrainHeight(Result.Horizontal));

            return Result;
        }

        public WorldPos TileAlignedPosFromMapPos(XYInt Horizontal, XYInt Footprint)
        {
            var Result = new WorldPos();

            Result.Horizontal.X =
                (int)
                    ((Math.Round(Convert.ToDouble((Horizontal.X - Footprint.X * Constants.TerrainGridSpacing / 2.0D) / Constants.TerrainGridSpacing)) +
                      Footprint.X / 2.0D) * Constants.TerrainGridSpacing);
            Result.Horizontal.Y =
                (int)
                    ((Math.Round(Convert.ToDouble((Horizontal.Y - Footprint.Y * Constants.TerrainGridSpacing / 2.0D) / Constants.TerrainGridSpacing)) +
                      Footprint.Y / 2.0D) * Constants.TerrainGridSpacing);
            Result.Altitude = (int)(GetTerrainHeight(Result.Horizontal));

            return Result;
        }

        // TODO: Think that code is useless - René
        public void UnitSectorsCalc(Unit Unit)
        {
            var Start = new XYInt();
            var Finish = new XYInt();
            var TileStart = new XYInt();
            var TileFinish = new XYInt();
            var X = 0;
            var Y = 0;

            GetFootprintTileRangeClamped(Unit.Pos.Horizontal, Unit.TypeBase.GetGetFootprintSelected(Unit.Rotation), ref TileStart, ref TileFinish);
            Start = GetTileSectorNum(TileStart);
            Finish = GetTileSectorNum(TileFinish);
            Start.X = MathUtil.ClampInt(Start.X, 0, SectorCount.X - 1);
            Start.Y = MathUtil.ClampInt(Start.Y, 0, SectorCount.Y - 1);
            Finish.X = MathUtil.ClampInt(Finish.X, 0, SectorCount.X - 1);
            Finish.Y = MathUtil.ClampInt(Finish.Y, 0, SectorCount.Y - 1);
            Unit.Sectors.Clear();
            for ( Y = Start.Y; Y <= Finish.Y; Y++ )
            {
                for ( X = Start.X; X <= Finish.X; X++ )
                {
                    clsUnitSectorConnection.Create(Unit, Sectors[X, Y]);
                }
            }
        }

        public void AutoSaveTest()
        {
            if ( !App.SettingsManager.AutoSaveEnabled )
            {
                return;
            }
            if ( AutoSave.ChangeCount < App.SettingsManager.AutoSaveMinIntervalSeconds )
            {
                return;
            }

            var timeDiff = DateTime.Now - AutoSave.SavedDate;
            if ( timeDiff.Seconds < App.SettingsManager.AutoSaveMinIntervalSeconds )
            {
                logger.Debug(string.Format("No autosave, we have {0} seconds of {1}",
                    timeDiff.Seconds, App.SettingsManager.AutoSaveMinIntervalSeconds));
                return;
            }

            AutoSave.ChangeCount = 0;
            AutoSave.SavedDate = DateTime.Now;

            App.ShowWarnings(AutoSavePerform());
        }

        public Result AutoSavePerform()
        {
            var ReturnResult = new Result("Autosave", false);

            var DateNow = DateTime.Now;
            var path = string.Format("{0}autosaved-{1}-{2}-{3}-{4}-{5}-{6}-{7}.fmap",
                App.AutoSavePath, DateNow.Year, App.MinDigits(DateNow.Month, 2),
                App.MinDigits(DateNow.Day, 2), App.MinDigits(DateNow.Hour, 2),
                App.MinDigits(DateNow.Minute, 2), App.MinDigits(DateNow.Second, 2),
                App.MinDigits(DateNow.Millisecond, 3));

            logger.Info(string.Format("Autosave to: \"{0}\"", path));

            var fmap = new FMapSaver (this);
            ReturnResult.Add(fmap.Save(path, false, App.SettingsManager.AutoSaveCompress));

            return ReturnResult;
        }

        public void UndoStepCreate(string StepName)
        {
            var NewUndo = new Undo();

            NewUndo.Name = StepName;

            var SectorNum = default(XYInt);
            foreach ( var tempLoopVar_SectorNum in SectorTerrainUndoChanges.ChangedPoints )
            {
                SectorNum = tempLoopVar_SectorNum;
                NewUndo.ChangedSectors.Add(ShadowSectors[SectorNum.X, SectorNum.Y]);
                ShadowSector_Create(SectorNum);
            }
            SectorTerrainUndoChanges.Clear();

            NewUndo.UnitChanges.AddRange(UnitChanges);
            UnitChanges.Clear();

            NewUndo.GatewayChanges.AddRange(GatewayChanges);
            GatewayChanges.Clear();

            if ( NewUndo.ChangedSectors.Count + NewUndo.UnitChanges.Count + NewUndo.GatewayChanges.Count > 0 )
            {
                while ( Undos.Count > UndoPosition ) //a new line has been started so remove redos
                {
                    Undos.RemoveAt(Undos.Count - 1);
                }

                Undos.Add(NewUndo);
                UndoPosition = Undos.Count;

                SetChanged();
            }
        }

        public void ShadowSector_Create(XYInt SectorNum)
        {
            var TileX = 0;
            var TileY = 0;
            var StartX = 0;
            var StartY = 0;
            var X = 0;
            var Y = 0;
            var Sector = default(ShadowSector);
            var LastTileX = 0;
            var LastTileY = 0;

            Sector = new ShadowSector();
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
            var UnitChange = default(UnitChange);
            var Undo = default(Undo);

            foreach ( var tempLoopVar_Undo in Undos )
            {
                Undo = tempLoopVar_Undo;
                foreach ( var tempLoopVar_UnitChange in Undo.UnitChanges )
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
            var ThisUndo = default(Undo);

            UndoStepCreate("Incomplete Action"); //make another redo step incase something has changed, such as if user presses undo while still dragging a tool

            UndoPosition--;

            ThisUndo = Undos[UndoPosition];

            var SectorNum = new XYInt();
            var CurrentSector = default(ShadowSector);
            var UndoSector = default(ShadowSector);
            var NewSectorsForThisUndo = new SimpleList<ShadowSector>();
            foreach ( var tempLoopVar_UndoSector in ThisUndo.ChangedSectors )
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
            var UnitAdd = new clsUnitAdd();
            UnitAdd.Map = this;
            var Unit = default(Unit);
            for ( var A = ThisUndo.UnitChanges.Count - 1; A >= 0; A-- ) //must do in reverse order, otherwise may try to delete units that havent been added yet
            {
                Unit = ThisUndo.UnitChanges[A].Unit;
                if ( ThisUndo.UnitChanges[A].Type == UnitChangeType.Added )
                {
                    //remove the unit from the map
                    UnitRemove(Unit.MapLink.ArrayPosition);
                }
                else if ( ThisUndo.UnitChanges[A].Type == UnitChangeType.Deleted )
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

            var GatewayChange = default(GatewayChange);
            for ( var A = ThisUndo.GatewayChanges.Count - 1; A >= 0; A-- )
            {
                GatewayChange = ThisUndo.GatewayChanges[A];
                switch ( GatewayChange.Type )
                {
                    case GatewayChangeType.Added:
                        //remove the unit from the map
                        GatewayChange.Gateway.MapLink.Disconnect();
                        break;
                    case GatewayChangeType.Deleted:
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
            var ThisUndo = default(Undo);

            ThisUndo = Undos[UndoPosition];

            var SectorNum = new XYInt();
            var CurrentSector = default(ShadowSector);
            var UndoSector = default(ShadowSector);
            var NewSectorsForThisUndo = new SimpleList<ShadowSector>();
            foreach ( var tempLoopVar_UndoSector in ThisUndo.ChangedSectors )
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
            var UnitAdd = new clsUnitAdd();
            UnitAdd.Map = this;
            var Unit = default(Unit);
            for ( var A = 0; A <= ThisUndo.UnitChanges.Count - 1; A++ ) //forward order is important
            {
                Unit = ThisUndo.UnitChanges[A].Unit;
                if ( ThisUndo.UnitChanges[A].Type == UnitChangeType.Added )
                {
                    //add the unit back on to the map
                    ID = Unit.ID;
                    UnitAdd.ID = ID;
                    UnitAdd.NewUnit = Unit;
                    UnitAdd.Perform();
                    App.ErrorIDChange(ID, Unit, "Redo_Perform");
                }
                else if ( ThisUndo.UnitChanges[A].Type == UnitChangeType.Deleted )
                {
                    //remove the unit from the map
                    UnitRemove(Unit.MapLink.ArrayPosition);
                }
                else
                {
                    Debugger.Break();
                }
            }

            var GatewayChange = default(GatewayChange);
            for ( var A = 0; A <= ThisUndo.GatewayChanges.Count - 1; A++ ) //forward order is important
            {
                GatewayChange = ThisUndo.GatewayChanges[A];
                switch ( GatewayChange.Type )
                {
                    case GatewayChangeType.Added:
                        //add the unit back on to the map
                        GatewayChange.Gateway.MapLink.Connect(Gateways);
                        break;
                    case GatewayChangeType.Deleted:
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

        public void Undo_Sector_Rejoin(ShadowSector Shadow_Sector_To_Rejoin)
        {
            var TileX = 0;
            var TileZ = 0;
            var StartX = 0;
            var StartZ = 0;
            var X = 0;
            var Y = 0;
            var LastTileX = 0;
            var LastTileZ = 0;

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

        public void MapInsert(Map MapToInsert, XYInt Offset, XYInt Area, bool InsertHeights, bool InsertTextures, bool InsertUnits,
            bool DeleteUnits, bool InsertGateways, bool DeleteGateways)
        {
            var Finish = new XYInt();
            var X = 0;
            var Y = 0;
            var SectorStart = new XYInt();
            var SectorFinish = new XYInt();
            var AreaAdjusted = new XYInt();
            var SectorNum = new XYInt();

            Finish.X = Math.Min(Offset.X + Math.Min(Area.X, MapToInsert.Terrain.TileSize.X), Terrain.TileSize.X);
            Finish.Y = Math.Min(Offset.Y + Math.Min(Area.Y, MapToInsert.Terrain.TileSize.Y), Terrain.TileSize.Y);
            AreaAdjusted.X = Finish.X - Offset.X;
            AreaAdjusted.Y = Finish.Y - Offset.Y;

            GetTileSectorRange(new XYInt(Offset.X - 1, Offset.Y - 1), Finish, ref SectorStart, ref SectorFinish);
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
                var TriDirection = default(bool);
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

            var LastTile = new XYInt();
            LastTile = Finish;
            LastTile.X--;
            LastTile.Y--;
            if ( DeleteGateways )
            {
                var A = 0;
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
                var GateStart = new XYInt();
                var GateFinish = new XYInt();
                var Gateway = default(Gateway);
                foreach ( var tempLoopVar_Gateway in MapToInsert.Gateways )
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
                var UnitsToDelete = new SimpleList<Unit>();
                var Unit = default(Unit);
                for ( Y = SectorStart.Y; Y <= SectorFinish.Y; Y++ )
                {
                    for ( X = SectorStart.X; X <= SectorFinish.X; X++ )
                    {
                        var Connection = default(clsUnitSectorConnection);
                        foreach ( var tempLoopVar_Connection in Sectors[X, Y].Units )
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
                foreach ( var tempLoopVar_Unit in UnitsToDelete )
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
                var PosDif = new XYInt();
                var NewUnit = default(Unit);
                var Unit = default(Unit);
                var ZeroPos = new XYInt(0, 0);
                var UnitAdd = new clsUnitAdd();

                UnitAdd.Map = this;
                UnitAdd.StoreChange = true;

                PosDif.X = Offset.X * Constants.TerrainGridSpacing;
                PosDif.Y = Offset.Y * Constants.TerrainGridSpacing;
                foreach ( var tempLoopVar_Unit in MapToInsert.Units )
                {
                    Unit = tempLoopVar_Unit;
                    if ( App.PosIsWithinTileArea(Unit.Pos.Horizontal, ZeroPos, AreaAdjusted) )
                    {
                        NewUnit = new Unit(Unit, this);
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

        public Gateway GatewayCreate(XYInt PosA, XYInt PosB)
        {
            if ( PosA.X >= 0 & PosA.X < Terrain.TileSize.X &
                 PosA.Y >= 0 & PosA.Y < Terrain.TileSize.Y &
                 PosB.X >= 0 & PosB.X < Terrain.TileSize.X &
                 PosB.Y >= 0 & PosB.Y < Terrain.TileSize.Y ) //is on map
            {
                if ( PosA.X == PosB.X | PosA.Y == PosB.Y ) //is straight
                {
                    var Gateway = new Gateway();

                    Gateway.PosA = PosA;
                    Gateway.PosB = PosB;

                    Gateway.MapLink.Connect(Gateways);

                    return Gateway;
                }
                return null;
            }
            return null;
        }

        public Gateway GatewayCreateStoreChange(XYInt PosA, XYInt PosB)
        {
            var Gateway = default(Gateway);

            Gateway = GatewayCreate(PosA, PosB);

            var GatewayChange = new GatewayChange();
            GatewayChange.Type = GatewayChangeType.Added;
            GatewayChange.Gateway = Gateway;
            GatewayChanges.Add(GatewayChange);

            return Gateway;
        }

        public void GatewayRemoveStoreChange(int Num)
        {
            var GatewayChange = new GatewayChange();
            GatewayChange.Type = GatewayChangeType.Deleted;
            GatewayChange.Gateway = Gateways[Num];
            GatewayChanges.Add(GatewayChange);

            Gateways[Num].MapLink.Disconnect();
        }

        public void TileType_Reset()
        {
            if ( Tileset == null )
            {
                TileTypeNum = new byte[0];
            }
            else
            {
                var A = 0;

                TileTypeNum = new byte[Tileset.Tiles.Count];
                for ( A = 0; A <= Tileset.Tiles.Count - 1; A++ )
                {
                    TileTypeNum[A] = Tileset.Tiles[A].DefaultType;
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

        internal void UnitSectorsGraphicsChanged(Unit UnitToUpdateFor)
        {
            if ( SectorGraphicsChanges == null )
            {
                Debugger.Break();
                return;
            }

            var Connection = default(clsUnitSectorConnection);

            foreach ( var tempLoopVar_Connection in UnitToUpdateFor.Sectors )
            {
                Connection = tempLoopVar_Connection;
                SectorGraphicsChanges.Changed(Connection.Sector.Pos);
            }
        }

        public WorldPos GetTileOffsetRotatedWorldPos(XYInt Tile, XYInt TileOffsetToRotate)
        {
            var Result = new WorldPos();

            var RotatedOffset = new XYInt();

            RotatedOffset = TileUtil.GetTileRotatedOffset(Terrain.Tiles[Tile.X, Tile.Y].Texture.Orientation, TileOffsetToRotate);
            Result.Horizontal.X = Tile.X * Constants.TerrainGridSpacing + RotatedOffset.X;
            Result.Horizontal.Y = Tile.Y * Constants.TerrainGridSpacing + RotatedOffset.Y;
            Result.Altitude = (int)(GetTerrainHeight(Result.Horizontal));

            return Result;
        }

        public void GetFootprintTileRangeClamped(XYInt Horizontal, XYInt Footprint, ref XYInt ResultStart, ref XYInt ResultFinish)
        {
            var Remainder = 0;
            var Centre = GetPosTileNum(Horizontal);
            var Half = 0;

            Half = Math.DivRem(Footprint.X, 2, out Remainder);
            ResultStart.X = MathUtil.ClampInt(Centre.X - Half, 0, Terrain.TileSize.X - 1);
            ResultFinish.X = MathUtil.ClampInt(ResultStart.X + Footprint.X - 1, 0, Terrain.TileSize.X - 1);
            Half = Math.DivRem(Footprint.Y, 2, out Remainder);
            ResultStart.Y = MathUtil.ClampInt(Centre.Y - Half, 0, Terrain.TileSize.Y - 1);
            ResultFinish.Y = MathUtil.ClampInt(ResultStart.Y + Footprint.Y - 1, 0, Terrain.TileSize.Y - 1);
        }

        public void GetFootprintTileRange(XYInt Horizontal, XYInt Footprint, ref XYInt ResultStart, ref XYInt ResultFinish)
        {
            var Remainder = 0;
            var Centre = GetPosTileNum(Horizontal);
            var Half = 0;

            Half = Math.DivRem(Footprint.X, 2, out Remainder);
            ResultStart.X = Centre.X - Half;
            ResultFinish.X = ResultStart.X + Footprint.X - 1;
            Half = Math.DivRem(Footprint.Y, 2, out Remainder);
            ResultStart.Y = Centre.Y - Half;
            ResultFinish.Y = ResultStart.Y + Footprint.Y - 1;
        }

        public XYInt GetPosTileNum(XYInt Horizontal)
        {
            var Result = new XYInt();

            Result.X = Horizontal.X / Constants.TerrainGridSpacing;
            Result.Y = (Horizontal.Y / Constants.TerrainGridSpacing);

            return Result;
        }

        public XYInt GetPosVertexNum(XYInt Horizontal)
        {
            var Result = new XYInt();

            Result.X = (int)(Math.Round(((double)Horizontal.X / Constants.TerrainGridSpacing)));
            Result.Y = (int)(Math.Round(((double)Horizontal.Y / Constants.TerrainGridSpacing)));

            return Result;
        }

        public XYInt GetPosSectorNum(XYInt Horizontal)
        {
            var Result = new XYInt();

            Result = GetTileSectorNum(GetPosTileNum(Horizontal));

            return Result;
        }

        public XYInt GetSectorNumClamped(XYInt SectorNum)
        {
            var Result = new XYInt();

            Result.X = MathUtil.ClampInt(SectorNum.X, 0, SectorCount.X - 1);
            Result.Y = MathUtil.ClampInt(SectorNum.Y, 0, SectorCount.Y - 1);

            return Result;
        }

        public int GetVertexAltitude(XYInt VertexNum)
        {
            return Terrain.Vertices[VertexNum.X, VertexNum.Y].Height * HeightMultiplier;
        }

        public bool PosIsOnMap(XYInt Horizontal)
        {
            return App.PosIsWithinTileArea(Horizontal, new XYInt(0, 0), Terrain.TileSize);
        }

        public XYInt TileNumClampToMap(XYInt TileNum)
        {
            var Result = new XYInt();

            Result.X = MathUtil.ClampInt(TileNum.X, 0, Terrain.TileSize.X - 1);
            Result.Y = MathUtil.ClampInt(TileNum.Y, 0, Terrain.TileSize.Y - 1);

            return Result;
        }

        public void CancelUserInput()
        {
            if ( !readyForUserInput )
            {
                return;
            }

            readyForUserInput = false;

            var X = 0;
            var Y = 0;

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

            SelectedTileA = new XYInt(0, 0);
            SelectedTileB = new XYInt(0, 0);
            SelectedAreaVertexA = new XYZInt(0, 0, 0);
            SelectedAreaVertexB = new XYZInt(0, 0, 0);
            UnitSelectedAreaVertexA = new XYZInt(0, 0, 0);

            ViewInfo = null;

            _SelectedUnitGroup = null;

            Messages = null;
        }

        public void InitializeUserInput()
        {
            if ( readyForUserInput )
            {
                return;
            }

            readyForUserInput = true;

            var X = 0;
            var Y = 0;

            SectorCount.X = (int)(Math.Ceiling(((double)Terrain.TileSize.X / Constants.SectorTileSize)));
            SectorCount.Y = (int)(Math.Ceiling(((double)Terrain.TileSize.Y / Constants.SectorTileSize)));
            Sectors = new Sector[SectorCount.X, SectorCount.Y];
            for ( Y = 0; Y <= SectorCount.Y - 1; Y++ )
            {
                for ( X = 0; X <= SectorCount.X - 1; X++ )
                {
                    Sectors[X, Y] = new Sector(new XYInt(X, Y));
                }
            }

            var Unit = default(Unit);
            foreach ( var tempLoopVar_Unit in Units )
            {
                Unit = tempLoopVar_Unit;
                UnitSectorsCalc(Unit);
            }

            ShadowSectors = new ShadowSector[SectorCount.X, SectorCount.Y];
            for ( Y = 0; Y <= SectorCount.Y - 1; Y++ )
            {
                for ( X = 0; X <= SectorCount.X - 1; X++ )
                {
                    ShadowSector_Create(new XYInt(X, Y));
                }
            }

            SectorGraphicsChanges = new clsSectorChanges(this);
            SectorGraphicsChanges.SetAllChanged();
            SectorUnitHeightsChanges = new clsSectorChanges(this);
            SectorTerrainUndoChanges = new clsSectorChanges(this);
            AutoTextureChanges = new clsAutoTextureChanges(this);
            TerrainInterpretChanges = new clsTerrainUpdate(Terrain.TileSize);

            UnitChanges = new SimpleClassList<UnitChange>();
            UnitChanges.MaintainOrder = true;
            GatewayChanges = new SimpleClassList<GatewayChange>();
            GatewayChanges.MaintainOrder = true;
            Undos = new SimpleClassList<Undo>();
            Undos.MaintainOrder = true;
            UndoPosition = 0;

            SelectedUnits = new ConnectedList<Unit, Map>(this);

            if ( InterfaceOptions == null )
            {
                InterfaceOptions = new InterfaceOptions();
            }

            ViewInfo = new clsViewInfo(this, App.MapViewGlSurface);

            _SelectedUnitGroup = new clsUnitGroupContainer();
            SelectedUnitGroup.Item = ScavengerUnitGroup;

            Messages = new SimpleClassList<Message>();
            Messages.MaintainOrder = true;
        }

        public void Update()
        {
            var PrevSuppress = SuppressMinimap;

            SuppressMinimap = true;
            UpdateAutoTextures();
            TerrainInterpretUpdate();
            SectorsUpdateGraphics();
            SectorsUpdateUnitHeights();
            SuppressMinimap = PrevSuppress;
        }

        public void SectorsUpdateUnitHeights()
        {
            var UpdateSectorUnitHeights = new clsUpdateSectorUnitHeights();
            UpdateSectorUnitHeights.Map = this;

            UpdateSectorUnitHeights.Start();
            SectorUnitHeightsChanges.PerformTool(UpdateSectorUnitHeights);
            UpdateSectorUnitHeights.Finish();
            SectorUnitHeightsChanges.Clear();
        }

        public void SectorsUpdateGraphics()
        {
            var UpdateSectorGraphics = new clsUpdateSectorGraphics();
            UpdateSectorGraphics.Map = this;

            if ( MainMap == this )
            {
                SectorGraphicsChanges.PerformTool(UpdateSectorGraphics);
            }
            SectorGraphicsChanges.Clear();
        }

        public void UpdateAutoTextures()
        {
            var UpdateAutotextures = new clsUpdateAutotexture();
            UpdateAutotextures.Map = this;
            UpdateAutotextures.MakeInvalidTiles = Program.frmMainInstance.cbxInvalidTiles.Checked;

            AutoTextureChanges.PerformTool(UpdateAutotextures);
            AutoTextureChanges.Clear();
        }

        public void TerrainInterpretUpdate()
        {
            var ApplyVertexInterpret = new clsApplyVertexTerrainInterpret();
            var ApplyTileInterpret = new clsApplyTileTerrainInterpret();
            var ApplySideHInterpret = new clsApplySideHTerrainInterpret();
            var ApplySideVInterpret = new clsApplySideVTerrainInterpret();
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

        public void TileNeedsInterpreting(XYInt Pos)
        {
            TerrainInterpretChanges.Tiles.Changed(Pos);
            TerrainInterpretChanges.Vertices.Changed(new XYInt(Pos.X, Pos.Y));
            TerrainInterpretChanges.Vertices.Changed(new XYInt(Pos.X + 1, Pos.Y));
            TerrainInterpretChanges.Vertices.Changed(new XYInt(Pos.X, Pos.Y + 1));
            TerrainInterpretChanges.Vertices.Changed(new XYInt(Pos.X + 1, Pos.Y + 1));
            TerrainInterpretChanges.SidesH.Changed(new XYInt(Pos.X, Pos.Y));
            TerrainInterpretChanges.SidesH.Changed(new XYInt(Pos.X, Pos.Y + 1));
            TerrainInterpretChanges.SidesV.Changed(new XYInt(Pos.X, Pos.Y));
            TerrainInterpretChanges.SidesV.Changed(new XYInt(Pos.X + 1, Pos.Y));
        }

        public void TileTextureChangeTerrainAction(XYInt Pos, TextureTerrainAction Action)
        {
            switch ( Action )
            {
                case TextureTerrainAction.Ignore:
                    break;

                case TextureTerrainAction.Reinterpret:
                    TileNeedsInterpreting(Pos);
                    break;
                case TextureTerrainAction.Remove:
                    Terrain.Vertices[Pos.X, Pos.Y].Terrain = null;
                    Terrain.Vertices[Pos.X + 1, Pos.Y].Terrain = null;
                    Terrain.Vertices[Pos.X, Pos.Y + 1].Terrain = null;
                    Terrain.Vertices[Pos.X + 1, Pos.Y + 1].Terrain = null;
                    break;
            }
        }

        public string GetTitle()
        {
            var ReturnResult = "";

            if ( PathInfo == null )
            {
                ReturnResult = "Unsaved map";
            }
            else
            {
                var SplitPath = new sSplitPath(PathInfo.Path);
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
            if ( changedEvent != null )
                changedEvent();

            AutoSave.ChangeCount++;
            AutoSaveTest();
        }

        public void SetTabText()
        {
            const int MaxLength = 24;

            var Result = "";
            Result = GetTitle();
            if ( Result.Length > MaxLength )
            {
                Result = Result.Substring(0, MaxLength - 3) + "...";
            }
            MapViewTabPage.Text = Result;
        }

        public bool SideHIsCliffOnBothSides(XYInt SideNum)
        {
            var TileNum = new XYInt();

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

        public bool SideVIsCliffOnBothSides(XYInt SideNum)
        {
            var TileNum = new XYInt();

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

        public bool VertexIsCliffEdge(XYInt VertexNum)
        {
            var TileNum = new XYInt();

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
            var SelectAction = new clsObjectSelect();

            SelectedUnits.GetItemsAsSimpleClassList().PerformTool(Tool);
            SelectedUnits.Clear();
            Tool.ResultUnits.PerformTool(SelectAction);
        }

        public bool CheckMessages()
        {
            var A = 0;
            var DateNow = DateTime.Now;
            var Changed = false;

            A = 0;
            while ( A < Messages.Count )
            {
                var timeDiff = DateTime.Now - Convert.ToDateTime(Messages[A].CreatedDate);
                if ( timeDiff.Seconds >= 6 )
                {
                    Messages.RemoveAt(A);
                    Changed = true;
                }
                else
                {
                    A++;
                }
            }
            return Changed;
        }

        public void PerformTileWall(clsWallType WallType, XYInt TileNum, bool Expand)
        {
            var SectorNum = new XYInt();
            var Unit = default(Unit);
            var UnitTile = new XYInt();
            var Difference = new XYInt();
            var TileWalls = Util.TileWalls.None;
            var Walls = new SimpleList<Unit>();
            var Removals = new SimpleList<Unit>();
            var unitTypeBase = default(UnitTypeBase);
            var structureTypeBase = default(StructureTypeBase);
            var X = 0;
            var Y = 0;
            var MinTile = new XYInt();
            var MaxTile = new XYInt();
            var Connection = default(clsUnitSectorConnection);
            MinTile.X = TileNum.X - 1;
            MinTile.Y = TileNum.Y - 1;
            MaxTile.X = TileNum.X + 1;
            MaxTile.Y = TileNum.Y + 1;
            var SectorStart = GetSectorNumClamped(GetTileSectorNum(MinTile));
            var SectorFinish = GetSectorNumClamped(GetTileSectorNum(MaxTile));

            for ( Y = SectorStart.Y; Y <= SectorFinish.Y; Y++ )
            {
                for ( X = SectorStart.X; X <= SectorFinish.X; X++ )
                {
                    SectorNum.X = X;
                    SectorNum.Y = Y;
                    foreach ( var tempLoopVar_Connection in Sectors[SectorNum.X, SectorNum.Y].Units )
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
                                        TileWalls = TileWalls | TileWalls.Bottom;
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
                                        TileWalls = TileWalls | TileWalls.Left;
                                        Walls.Add(Unit);
                                    }
                                    else if ( Difference.X == 1 )
                                    {
                                        TileWalls = TileWalls | TileWalls.Right;
                                        Walls.Add(Unit);
                                    }
                                }
                                else if ( Difference.Y == -1 )
                                {
                                    if ( Difference.X == 0 )
                                    {
                                        TileWalls = TileWalls | TileWalls.Top;
                                        Walls.Add(Unit);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            foreach ( var tempLoopVar_Unit in Removals )
            {
                Unit = tempLoopVar_Unit;
                UnitRemoveStoreChange(Unit.MapLink.ArrayPosition);
            }

            var NewUnit = new Unit();
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
            NewUnit.Pos = TileAlignedPos(TileNum, new XYInt(1, 1));
            NewUnit.TypeBase = newUnitTypeBase;
            var UnitAdd = new clsUnitAdd();
            UnitAdd.Map = this;
            UnitAdd.NewUnit = NewUnit;
            UnitAdd.StoreChange = true;
            UnitAdd.Perform();

            if ( Expand )
            {
                var Wall = default(Unit);
                foreach ( var tempLoopVar_Wall in Walls )
                {
                    Wall = tempLoopVar_Wall;
                    PerformTileWall(WallType, GetPosTileNum(Wall.Pos.Horizontal), false);
                }
            }
        }

        public bool Save_FMap_Prompt()
        {
            var Dialog = new SaveFileDialog();

            Dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            Dialog.FileName = "";
            Dialog.Filter = Constants.ProgramName + " Map Files (*.fmap)|*.fmap";
            if ( Dialog.ShowDialog(Program.frmMainInstance) != DialogResult.OK )
            {
                return false;
            }
            App.SettingsManager.SavePath = Path.GetDirectoryName(Dialog.FileName);
            var fMap = new FMapSaver (this);
            var result = fMap.Save(Dialog.FileName, true, true);
            if ( !result.HasProblems )
            {
                PathInfo = new PathInfo(Dialog.FileName, true);
                ChangedSinceSave = false;
            }
            App.ShowWarnings(result);
            return !result.HasProblems;
        }

        public bool Save_FMap_Quick()
        {
            if ( PathInfo == null )
            {
                return Save_FMap_Prompt();
            }
            if ( PathInfo.IsFMap )
            {
                var fMap = new FMapSaver (this);
                var result = fMap.Save(PathInfo.Path, true, true);
                if ( !result.HasProblems )
                {
                    ChangedSinceSave = false;
                }
                App.ShowWarnings(result);
                return !result.HasProblems;
            }
            return Save_FMap_Prompt();
        }

        public bool ClosePrompt()
        {
            if ( ChangedSinceSave )
            {
                var Prompt = new frmClose(GetTitle());
                var Result = Prompt.ShowDialog(Program.frmMainInstance);
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
            return true;
        }
    }
}