using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Matrix3D;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.Devices;
using OpenTK.Graphics.OpenGL;

namespace SharpFlame
{
    public partial class clsMap
    {
        public modLists.ConnectedListLink<clsMap, frmMain> frmMainLink;

        public class clsTerrain
        {
            public struct Vertex
            {
                public byte Height;
                public clsPainter.clsTerrain Terrain;
            }

            public struct Tile
            {
                public struct sTexture
                {
                    public int TextureNum;
                    public TileOrientation.sTileOrientation Orientation;
                }

                public sTexture Texture;
                public bool Tri;
                public bool TriTopLeftIsCliff;
                public bool TriTopRightIsCliff;
                public bool TriBottomLeftIsCliff;
                public bool TriBottomRightIsCliff;
                public bool Terrain_IsCliff;
                public TileOrientation.sTileDirection DownSide;

                public void Copy(Tile TileToCopy)
                {
                    Texture = TileToCopy.Texture;
                    Tri = TileToCopy.Tri;
                    TriTopLeftIsCliff = TileToCopy.TriTopLeftIsCliff;
                    TriTopRightIsCliff = TileToCopy.TriTopRightIsCliff;
                    TriBottomLeftIsCliff = TileToCopy.TriBottomLeftIsCliff;
                    TriBottomRightIsCliff = TileToCopy.TriBottomRightIsCliff;
                    Terrain_IsCliff = TileToCopy.Terrain_IsCliff;
                    DownSide = TileToCopy.DownSide;
                }

                public void TriCliffAddDirection(TileOrientation.sTileDirection Direction)
                {
                    if ( Direction.X == 0 )
                    {
                        if ( Direction.Y == 0 )
                        {
                            TriTopLeftIsCliff = true;
                        }
                        else if ( Direction.Y == 2 )
                        {
                            TriBottomLeftIsCliff = true;
                        }
                        else
                        {
                            Debugger.Break();
                        }
                    }
                    else if ( Direction.X == 2 )
                    {
                        if ( Direction.Y == 0 )
                        {
                            TriTopRightIsCliff = true;
                        }
                        else if ( Direction.Y == 2 )
                        {
                            TriBottomRightIsCliff = true;
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
            }

            public struct Side
            {
                public clsPainter.clsRoad Road;
            }

            public modMath.sXY_int TileSize;

            public Vertex[,] Vertices;
            public Tile[,] Tiles;
            public Side[,] SideH;
            public Side[,] SideV;

            public clsTerrain(modMath.sXY_int NewSize)
            {
                TileSize = NewSize;

                Vertices = new Vertex[TileSize.X + 1, TileSize.Y + 1];
                Tiles = new Tile[TileSize.X, TileSize.Y];
                SideH = new Side[TileSize.X, TileSize.Y + 1];
                SideV = new Side[TileSize.X + 1, TileSize.Y];
                int X = 0;
                int Y = 0;

                for ( Y = 0; Y <= TileSize.Y - 1; Y++ )
                {
                    for ( X = 0; X <= TileSize.X - 1; X++ )
                    {
                        Tiles[X, Y].Texture.TextureNum = -1;
                        Tiles[X, Y].DownSide = TileOrientation.TileDirection_None;
                    }
                }
            }
        }

        public clsTerrain Terrain;

        public class clsSector
        {
            public clsSector()
            {
                Units = new modLists.ConnectedList<clsUnitSectorConnection, clsSector>(this);
            }

            public modMath.sXY_int Pos;
            public int GLList_Textured;
            public int GLList_Wireframe;
            public modLists.ConnectedList<clsUnitSectorConnection, clsSector> Units;

            public void DeleteLists()
            {
                if ( GLList_Textured != 0 )
                {
                    GL.DeleteLists(GLList_Textured, 1);
                    GLList_Textured = 0;
                }
                if ( GLList_Wireframe != 0 )
                {
                    GL.DeleteLists(GLList_Wireframe, 1);
                    GLList_Wireframe = 0;
                }
            }

            public void Deallocate()
            {
                Units.Deallocate();
            }

            public clsSector(modMath.sXY_int NewPos)
            {
                Units = new modLists.ConnectedList<clsUnitSectorConnection, clsSector>(this);


                Pos = NewPos;
            }
        }

        public clsSector[,] Sectors = new clsSector[0, 0];
        public modMath.sXY_int SectorCount;

        public class clsShadowSector
        {
            public modMath.sXY_int Num;
            public clsTerrain Terrain = new clsTerrain(new modMath.sXY_int(modProgram.SectorTileSize, modProgram.SectorTileSize));
        }

        public clsShadowSector[,] ShadowSectors = new clsShadowSector[0, 0];

        public class clsUnitChange
        {
            public enum enumType
            {
                Added,
                Deleted
            }

            public enumType Type;
            public clsUnit Unit;
        }

        public class clsGatewayChange
        {
            public enum enumType
            {
                Added,
                Deleted
            }

            public enumType Type;
            public clsGateway Gateway;
        }

        public class clsUndo
        {
            public string Name;
            public modLists.SimpleList<clsShadowSector> ChangedSectors = new modLists.SimpleList<clsShadowSector>();
            public modLists.SimpleList<clsUnitChange> UnitChanges = new modLists.SimpleList<clsUnitChange>();
            public modLists.SimpleList<clsGatewayChange> GatewayChanges = new modLists.SimpleList<clsGatewayChange>();
        }

        public modLists.SimpleClassList<clsUndo> Undos;
        public int UndoPosition;

        public modLists.SimpleClassList<clsUnitChange> UnitChanges;
        public modLists.SimpleClassList<clsGatewayChange> GatewayChanges;

        public int HeightMultiplier = modProgram.DefaultHeightMultiplier;

        public bool ReadyForUserInput
        {
            get { return _ReadyForUserInput; }
        }

        private bool _ReadyForUserInput = false;

        public modLists.ConnectedList<clsUnit, clsMap> SelectedUnits;
        public modMath.clsXY_int Selected_Tile_A;
        public modMath.clsXY_int Selected_Tile_B;
        public modMath.clsXY_int Selected_Area_VertexA;
        public modMath.clsXY_int Selected_Area_VertexB;
        public modMath.clsXY_int Unit_Selected_Area_VertexA;

        public int Minimap_GLTexture;
        public int Minimap_Texture_Size;

        public class clsMessage
        {
            public string Text;
            private DateTime _CreatedDate = DateTime.Now;

            public DateTime CreatedDate
            {
                get { return _CreatedDate; }
            }
        }

        public modLists.SimpleClassList<clsMessage> Messages;

        public clsTileset Tileset;

        public class clsPathInfo
        {
            private string _Path;
            private bool _IsFMap;

            public string Path
            {
                get { return _Path; }
            }

            public bool IsFMap
            {
                get { return _IsFMap; }
            }

            public clsPathInfo(string Path, bool IsFMap)
            {
                _Path = Path;
                _IsFMap = IsFMap;
            }
        }

        public clsPathInfo PathInfo;

        public bool ChangedSinceSave = false;

        public delegate void ChangedEventHandler();

        private ChangedEventHandler ChangedEvent;

        public event ChangedEventHandler Changed
        {
            add { ChangedEvent = (ChangedEventHandler)Delegate.Combine(ChangedEvent, value); }
            remove { ChangedEvent = (ChangedEventHandler)Delegate.Remove(ChangedEvent, value); }
        }


        public class clsAutoSave
        {
            public int ChangeCount;
            public DateTime SavedDate;

            public clsAutoSave()
            {
                SavedDate = DateTime.Now;
            }
        }

        public clsAutoSave AutoSave = new clsAutoSave();

        public clsPainter Painter = new clsPainter();

        public byte[] Tile_TypeNum = new byte[0];

        public class clsGateway
        {
            public clsGateway()
            {
                MapLink = new modLists.ConnectedListLink<clsGateway, clsMap>(this);
            }

            public modLists.ConnectedListLink<clsGateway, clsMap> MapLink;
            public modMath.sXY_int PosA;
            public modMath.sXY_int PosB;

            public bool IsOffMap()
            {
                modMath.sXY_int TerrainSize = MapLink.Source.Terrain.TileSize;

                return PosA.X < 0
                       | PosA.X >= TerrainSize.X
                       | PosA.Y < 0
                       | PosA.Y >= TerrainSize.Y
                       | PosB.X < 0
                       | PosB.X >= TerrainSize.X
                       | PosB.Y < 0
                       | PosB.Y >= TerrainSize.Y;
            }

            public void Deallocate()
            {
                MapLink.Deallocate();
            }
        }

        public modLists.ConnectedList<clsGateway, clsMap> Gateways;

        public clsMap()
        {
            frmMainLink = new modLists.ConnectedListLink<clsMap, frmMain>(this);
            Gateways = new modLists.ConnectedList<clsGateway, clsMap>(this);


            Initialize();
        }

        public clsMap(modMath.sXY_int TileSize)
        {
            frmMainLink = new modLists.ConnectedListLink<clsMap, frmMain>(this);
            Gateways = new modLists.ConnectedList<clsGateway, clsMap>(this);


            Initialize();

            TerrainBlank(TileSize);
            TileType_Reset();
        }

        public void Initialize()
        {
            MakeMinimapTimer = new Timer();
            MakeMinimapTimer.Tick += MinimapTimer_Tick;
            MakeMinimapTimer.Interval = modProgram.MinimapDelay;

            MakeDefaultUnitGroups();
            ScriptPositions.MaintainOrder = true;
            ScriptAreas.MaintainOrder = true;
        }

        public clsMap(clsMap MapToCopy, modMath.sXY_int Offset, modMath.sXY_int Area)
        {
            frmMainLink = new modLists.ConnectedListLink<clsMap, frmMain>(this);
            Gateways = new modLists.ConnectedList<clsGateway, clsMap>(this);

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

            SectorCount.X = (int)(Math.Ceiling((double)(Area.X / modProgram.SectorTileSize)));
            SectorCount.Y = (int)(Math.Ceiling((double)(Area.Y / modProgram.SectorTileSize)));
            Sectors = new clsSector[SectorCount.X, SectorCount.Y];
            for ( Y = 0; Y <= SectorCount.Y - 1; Y++ )
            {
                for ( X = 0; X <= SectorCount.X - 1; X++ )
                {
                    Sectors[X, Y] = new clsSector(new modMath.sXY_int(X, Y));
                }
            }

            modMath.sXY_int PosDif = new modMath.sXY_int();
            clsUnitAdd NewUnitAdd = new clsUnitAdd();
            NewUnitAdd.Map = this;
            clsUnit NewUnit = default(clsUnit);

            clsGateway Gateway = default(clsGateway);
            foreach ( clsGateway tempLoopVar_Gateway in MapToCopy.Gateways )
            {
                Gateway = tempLoopVar_Gateway;
                GatewayCreate(new modMath.sXY_int(Gateway.PosA.X - Offset.X, Gateway.PosA.Y - Offset.Y),
                    new modMath.sXY_int(Gateway.PosB.X - Offset.X, Gateway.PosB.Y - Offset.Y));
            }

            PosDif.X = - Offset.X * modProgram.TerrainGridSpacing;
            PosDif.Y = - Offset.Y * modProgram.TerrainGridSpacing;
            clsUnit Unit = default(clsUnit);
            modMath.sXY_int NewPos = new modMath.sXY_int();
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

        protected void TerrainBlank(modMath.sXY_int TileSize)
        {
            int X = 0;
            int Y = 0;

            Terrain = new clsTerrain(TileSize);
            SectorCount.X = (int)(Math.Ceiling((double)(Terrain.TileSize.X / modProgram.SectorTileSize)));
            SectorCount.Y = (int)(Math.Ceiling((double)(Terrain.TileSize.Y / modProgram.SectorTileSize)));
            Sectors = new clsSector[SectorCount.X, SectorCount.Y];
            for ( Y = 0; Y <= SectorCount.Y - 1; Y++ )
            {
                for ( X = 0; X <= SectorCount.X - 1; X++ )
                {
                    Sectors[X, Y] = new clsSector(new modMath.sXY_int(X, Y));
                }
            }
        }

        public bool GetTerrainTri(modMath.sXY_int Horizontal)
        {
            int X1 = 0;
            int Y1 = 0;
            double InTileX = 0;
            double InTileZ = 0;
            int XG = 0;
            int YG = 0;

            XG = (int)(Conversion.Int(Horizontal.X / modProgram.TerrainGridSpacing));
            YG = Conversion.Int(Horizontal.Y / modProgram.TerrainGridSpacing);
            InTileX = modMath.Clamp_dbl(Horizontal.X / modProgram.TerrainGridSpacing - XG, 0.0D, 1.0D);
            InTileZ = modMath.Clamp_dbl(Horizontal.Y / modProgram.TerrainGridSpacing - YG, 0.0D, 1.0D);
            X1 = modMath.Clamp_int(XG, 0, Terrain.TileSize.X - 1);
            Y1 = modMath.Clamp_int(YG, 0, Terrain.TileSize.Y - 1);
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

        public double GetTerrainSlopeAngle(modMath.sXY_int Horizontal)
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

            XG = Conversion.Int(Horizontal.X / modProgram.TerrainGridSpacing);
            YG = (int)(Conversion.Int(Horizontal.Y / modProgram.TerrainGridSpacing));
            InTileX = modMath.Clamp_dbl(Horizontal.X / modProgram.TerrainGridSpacing - XG, 0.0D, 1.0D);
            InTileZ = modMath.Clamp_dbl(Horizontal.Y / modProgram.TerrainGridSpacing - YG, 0.0D, 1.0D);
            X1 = modMath.Clamp_int(XG, 0, Terrain.TileSize.X - 1);
            Y1 = modMath.Clamp_int(YG, 0, Terrain.TileSize.Y - 1);
            X2 = modMath.Clamp_int(XG + 1, 0, Terrain.TileSize.X);
            Y2 = modMath.Clamp_int(YG + 1, 0, Terrain.TileSize.Y);
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

            XYZ_dbl.X = modProgram.TerrainGridSpacing;
            XYZ_dbl.Y = GradientX * HeightMultiplier;
            XYZ_dbl.Z = 0.0D;
            XYZ_dbl2.X = 0.0D;
            XYZ_dbl2.Y = GradientY * HeightMultiplier;
            XYZ_dbl2.Z = modProgram.TerrainGridSpacing;
            Matrix3DMath.VectorCrossProduct(XYZ_dbl, XYZ_dbl2, ref XYZ_dbl3);
            if ( XYZ_dbl3.X != 0.0D | XYZ_dbl3.Z != 0.0D )
            {
                Matrix3DMath.VectorToPY(XYZ_dbl3, ref AnglePY);
                return modMath.RadOf90Deg - Math.Abs(AnglePY.Pitch);
            }
            else
            {
                return 0.0D;
            }
        }

        public double GetTerrainHeight(modMath.sXY_int Horizontal)
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

            XG = Conversion.Int(Horizontal.X / modProgram.TerrainGridSpacing);
            YG = (int)(Conversion.Int(Horizontal.Y / modProgram.TerrainGridSpacing));
            InTileX = modMath.Clamp_dbl(Horizontal.X / modProgram.TerrainGridSpacing - XG, 0.0D, 1.0D);
            InTileZ = modMath.Clamp_dbl(Horizontal.Y / modProgram.TerrainGridSpacing - YG, 0.0D, 1.0D);
            X1 = modMath.Clamp_int(XG, 0, Terrain.TileSize.X - 1);
            Y1 = modMath.Clamp_int(YG, 0, Terrain.TileSize.Y - 1);
            X2 = modMath.Clamp_int(XG + 1, 0, Terrain.TileSize.X);
            Y2 = modMath.Clamp_int(YG + 1, 0, Terrain.TileSize.Y);
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

        public modMath.sXYZ_sng TerrainVertexNormalCalc(int X, int Y)
        {
            modMath.sXYZ_sng ReturnResult = new modMath.sXYZ_sng();
            int TerrainHeightX1 = 0;
            int TerrainHeightX2 = 0;
            int TerrainHeightY1 = 0;
            int TerrainHeightY2 = 0;
            int X2 = 0;
            int Y2 = 0;
            Position.XYZ_dbl XYZ_dbl = default(Position.XYZ_dbl);
            Position.XYZ_dbl XYZ_dbl2 = default(Position.XYZ_dbl);
            double dblTemp = 0;

            X2 = modMath.Clamp_int(X - 1, 0, Terrain.TileSize.X);
            Y2 = modMath.Clamp_int(Y, 0, Terrain.TileSize.Y);
            TerrainHeightX1 = Terrain.Vertices[X2, Y2].Height;
            X2 = modMath.Clamp_int(X + 1, 0, Terrain.TileSize.X);
            Y2 = modMath.Clamp_int(Y, 0, Terrain.TileSize.Y);
            TerrainHeightX2 = Terrain.Vertices[X2, Y2].Height;
            X2 = modMath.Clamp_int(X, 0, Terrain.TileSize.X);
            Y2 = modMath.Clamp_int(Y - 1, 0, Terrain.TileSize.Y);
            TerrainHeightY1 = Terrain.Vertices[X2, Y2].Height;
            X2 = modMath.Clamp_int(X, 0, Terrain.TileSize.X);
            Y2 = modMath.Clamp_int(Y + 1, 0, Terrain.TileSize.Y);
            TerrainHeightY2 = Terrain.Vertices[X2, Y2].Height;
            XYZ_dbl.X = (TerrainHeightX1 - TerrainHeightX2) * HeightMultiplier;
            XYZ_dbl.Y = modProgram.TerrainGridSpacing * 2.0D;
            XYZ_dbl.Z = 0.0D;
            XYZ_dbl2.X = 0.0D;
            XYZ_dbl2.Y = modProgram.TerrainGridSpacing * 2.0D;
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

        public void TerrainResize(modMath.sXY_int Offset, modMath.sXY_int Size)
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

            PosDifX = - Offset.X * modProgram.TerrainGridSpacing;
            PosDifZ = - Offset.Y * modProgram.TerrainGridSpacing;
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

            modMath.sXY_int ZeroPos = new modMath.sXY_int(0, 0);

            int Position = 0;
            foreach ( clsUnit tempLoopVar_Unit in Units.GetItemsAsSimpleList() )
            {
                Unit = tempLoopVar_Unit;
                Position = Unit.MapLink.ArrayPosition;
                if ( !modProgram.PosIsWithinTileArea(Units[Position].Pos.Horizontal, ZeroPos, NewTerrain.TileSize) )
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

            modMath.sXY_int PosOffset = new modMath.sXY_int(Offset.X * modProgram.TerrainGridSpacing, Offset.Y * modProgram.TerrainGridSpacing);

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

            StartX = X * modProgram.SectorTileSize;
            StartY = Y * modProgram.SectorTileSize;
            FinishX = Math.Min(StartX + modProgram.SectorTileSize, Terrain.TileSize.X) - 1;
            FinishY = Math.Min(StartY + modProgram.SectorTileSize, Terrain.TileSize.Y) - 1;

            Sectors[X, Y].GLList_Textured = GL.GenLists(1);
            GL.NewList(Convert.ToInt32(Sectors[X, Y].GLList_Textured), ListMode.Compile);

            if ( modProgram.Draw_Units )
            {
                bool[,] IsBasePlate = new bool[modProgram.SectorTileSize, modProgram.SectorTileSize];
                clsUnit Unit = default(clsUnit);
                clsStructureType StructureType = default(clsStructureType);
                modMath.sXY_int Footprint = new modMath.sXY_int();
                clsUnitSectorConnection Connection = default(clsUnitSectorConnection);
                modMath.sXY_int FootprintStart = new modMath.sXY_int();
                modMath.sXY_int FootprintFinish = new modMath.sXY_int();
                foreach ( clsUnitSectorConnection tempLoopVar_Connection in Sectors[X, Y].Units )
                {
                    Connection = tempLoopVar_Connection;
                    Unit = Connection.Unit;
                    if ( Unit.Type.Type == clsUnitType.enumType.PlayerStructure )
                    {
                        StructureType = (clsStructureType)Unit.Type;
                        if ( StructureType.StructureBasePlate != null )
                        {
                            Footprint = StructureType.get_GetFootprintSelected(Unit.Rotation);
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
            modMath.sXYZ_sng Vertex0 = new modMath.sXYZ_sng();
            modMath.sXYZ_sng Vertex1 = new modMath.sXYZ_sng();
            modMath.sXYZ_sng Vertex2 = new modMath.sXYZ_sng();
            modMath.sXYZ_sng Vertex3 = new modMath.sXYZ_sng();

            TileTerrainHeight[0] = Terrain.Vertices[TileX, TileY].Height;
            TileTerrainHeight[1] = Terrain.Vertices[TileX + 1, TileY].Height;
            TileTerrainHeight[2] = Terrain.Vertices[TileX, TileY + 1].Height;
            TileTerrainHeight[3] = Terrain.Vertices[TileX + 1, TileY + 1].Height;

            Vertex0.X = TileX * modProgram.TerrainGridSpacing;
            Vertex0.Y = (float)(TileTerrainHeight[0] * HeightMultiplier);
            Vertex0.Z = - TileY * modProgram.TerrainGridSpacing;
            Vertex1.X = (TileX + 1) * modProgram.TerrainGridSpacing;
            Vertex1.Y = (float)(TileTerrainHeight[1] * HeightMultiplier);
            Vertex1.Z = - TileY * modProgram.TerrainGridSpacing;
            Vertex2.X = TileX * modProgram.TerrainGridSpacing;
            Vertex2.Y = (float)(TileTerrainHeight[2] * HeightMultiplier);
            Vertex2.Z = - (TileY + 1) * modProgram.TerrainGridSpacing;
            Vertex3.X = (TileX + 1) * modProgram.TerrainGridSpacing;
            Vertex3.Y = (float)(TileTerrainHeight[3] * HeightMultiplier);
            Vertex3.Z = - (TileY + 1) * modProgram.TerrainGridSpacing;

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

        public void DrawTileOrientation(modMath.sXY_int Tile)
        {
            TileOrientation.sTileOrientation TileOrientation;
            modMath.sXY_int UnrotatedPos = new modMath.sXY_int();
            modProgram.sWorldPos Vertex0 = new modProgram.sWorldPos();
            modProgram.sWorldPos Vertex1 = new modProgram.sWorldPos();
            modProgram.sWorldPos Vertex2 = new modProgram.sWorldPos();

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
            modMath.sXY_int Low = new modMath.sXY_int();
            modMath.sXY_int High = new modMath.sXY_int();
            modMath.sXY_int Footprint = new modMath.sXY_int();
            bool Flag = default(bool);
            bool[,] UnitMap = new bool[Texture.Size.Y, Texture.Size.X];
            float[,,] sngTexture = new float[Texture.Size.Y, Texture.Size.X, 3];
            float Alpha = 0;
            float AntiAlpha = 0;
            sRGB_sng RGB_sng = new sRGB_sng();

            if ( modMain.frmMainInstance.menuMiniShowTex.Checked )
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
                if ( modMain.frmMainInstance.menuMiniShowHeight.Checked )
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
            else if ( modMain.frmMainInstance.menuMiniShowHeight.Checked )
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
            if ( modMain.frmMainInstance.menuMiniShowCliffs.Checked )
            {
                if ( Tileset != null )
                {
                    Alpha = modSettings.Settings.MinimapCliffColour.Alpha;
                    AntiAlpha = 1.0F - Alpha;
                    for ( Y = 0; Y <= Terrain.TileSize.Y - 1; Y++ )
                    {
                        for ( X = 0; X <= Terrain.TileSize.X - 1; X++ )
                        {
                            if ( Terrain.Tiles[X, Y].Texture.TextureNum >= 0 && Terrain.Tiles[X, Y].Texture.TextureNum < Tileset.TileCount )
                            {
                                if ( Tileset.Tiles[Terrain.Tiles[X, Y].Texture.TextureNum].Default_Type == modProgram.TileTypeNum_Cliff )
                                {
                                    sngTexture[Y, X, 0] = sngTexture[Y, X, 0] * AntiAlpha + modSettings.Settings.MinimapCliffColour.Red * Alpha;
                                    sngTexture[Y, X, 1] = sngTexture[Y, X, 1] * AntiAlpha + modSettings.Settings.MinimapCliffColour.Green * Alpha;
                                    sngTexture[Y, X, 2] = sngTexture[Y, X, 2] * AntiAlpha + modSettings.Settings.MinimapCliffColour.Blue * Alpha;
                                }
                            }
                        }
                    }
                }
            }
            if ( modMain.frmMainInstance.menuMiniShowGateways.Checked )
            {
                clsGateway Gateway = default(clsGateway);
                foreach ( clsGateway tempLoopVar_Gateway in Gateways )
                {
                    Gateway = tempLoopVar_Gateway;
                    modMath.ReorderXY(Gateway.PosA, Gateway.PosB, Low, High);
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
            if ( modMain.frmMainInstance.menuMiniShowUnits.Checked )
            {
                //units that are not selected
                clsUnit Unit = default(clsUnit);
                foreach ( clsUnit tempLoopVar_Unit in Units )
                {
                    Unit = tempLoopVar_Unit;
                    Flag = true;
                    if ( Unit.Type.UnitType_frmMainSelectedLink.IsConnected )
                    {
                        Flag = false;
                    }
                    else
                    {
                        Footprint = Unit.Type.get_GetFootprintSelected(Unit.Rotation);
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
                                    if ( modSettings.Settings.MinimapTeamColours )
                                    {
                                        if ( modSettings.Settings.MinimapTeamColoursExceptFeatures & Unit.Type.Type == clsUnitType.enumType.Feature )
                                        {
                                            sngTexture[Y, X, 0] = modProgram.MinimapFeatureColour.Red;
                                            sngTexture[Y, X, 1] = modProgram.MinimapFeatureColour.Green;
                                            sngTexture[Y, X, 2] = modProgram.MinimapFeatureColour.Blue;
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
                Alpha = modSettings.Settings.MinimapSelectedObjectsColour.Alpha;
                AntiAlpha = 1.0F - Alpha;
                foreach ( clsUnit tempLoopVar_Unit in Units )
                {
                    Unit = tempLoopVar_Unit;
                    Flag = false;
                    if ( Unit.Type.UnitType_frmMainSelectedLink.IsConnected )
                    {
                        Flag = true;
                        Footprint = Unit.Type.get_GetFootprintSelected(Unit.Rotation);
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
                                    sngTexture[Y, X, 0] = sngTexture[Y, X, 0] * AntiAlpha + modSettings.Settings.MinimapSelectedObjectsColour.Red * Alpha;
                                    sngTexture[Y, X, 1] = sngTexture[Y, X, 1] * AntiAlpha + modSettings.Settings.MinimapSelectedObjectsColour.Green * Alpha;
                                    sngTexture[Y, X, 2] = sngTexture[Y, X, 2] * AntiAlpha + modSettings.Settings.MinimapSelectedObjectsColour.Blue * Alpha;
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

        public class clsMinimapTexture
        {
            public sRGBA_sng[] InlinePixels;
            public modMath.sXY_int Size;

            public sRGBA_sng get_Pixels(int X, int Y)
            {
                return InlinePixels[Y * Size.X + X];
            }

            public void set_Pixels(int X, int Y, sRGBA_sng value)
            {
                InlinePixels[Y * Size.X + X] = value;
            }

            public clsMinimapTexture(modMath.sXY_int Size)
            {
                this.Size = Size;
                InlinePixels = new sRGBA_sng[Size.X * Size.Y];
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

            clsMinimapTexture Texture = new clsMinimapTexture(new modMath.sXY_int(Minimap_Texture_Size, Minimap_Texture_Size));

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

            modMain.frmMainInstance.View_DrawViewLater();
        }

        public void MinimapGLDelete()
        {
            if ( Minimap_GLTexture != 0 )
            {
                GL.DeleteTextures(1, ref Minimap_GLTexture);
                Minimap_GLTexture = 0;
            }
        }

        public modMath.sXY_int GetTileSectorNum(modMath.sXY_int Tile)
        {
            modMath.sXY_int Result = new modMath.sXY_int();

            Result.X = Conversion.Int(Tile.X / modProgram.SectorTileSize);
            Result.Y = Conversion.Int(Tile.Y / modProgram.SectorTileSize);

            return Result;
        }

        public void GetTileSectorRange(modMath.sXY_int StartTile, modMath.sXY_int FinishTile, ref modMath.sXY_int ResultSectorStart,
            ref modMath.sXY_int ResultSectorFinish)
        {
            ResultSectorStart = GetTileSectorNum(StartTile);
            ResultSectorFinish = GetTileSectorNum(FinishTile);
            ResultSectorStart.X = modMath.Clamp_int(ResultSectorStart.X, 0, SectorCount.X - 1);
            ResultSectorStart.Y = modMath.Clamp_int(ResultSectorStart.Y, 0, SectorCount.Y - 1);
            ResultSectorFinish.X = modMath.Clamp_int(ResultSectorFinish.X, 0, SectorCount.X - 1);
            ResultSectorFinish.Y = modMath.Clamp_int(ResultSectorFinish.Y, 0, SectorCount.Y - 1);
        }

        public modProgram.sWorldPos TileAlignedPos(modMath.sXY_int TileNum, modMath.sXY_int Footprint)
        {
            modProgram.sWorldPos Result = new modProgram.sWorldPos();

            Result.Horizontal.X = (int)((TileNum.X + Footprint.X / 2.0D) * modProgram.TerrainGridSpacing);
            Result.Horizontal.Y = (int)((TileNum.Y + Footprint.Y / 2.0D) * modProgram.TerrainGridSpacing);
            Result.Altitude = (int)(GetTerrainHeight(Result.Horizontal));

            return Result;
        }

        public modProgram.sWorldPos TileAlignedPosFromMapPos(modMath.sXY_int Horizontal, modMath.sXY_int Footprint)
        {
            modProgram.sWorldPos Result = new modProgram.sWorldPos();

            Result.Horizontal.X =
                (int)
                    ((Math.Round(Convert.ToDouble((Horizontal.X - Footprint.X * modProgram.TerrainGridSpacing / 2.0D) / modProgram.TerrainGridSpacing)) +
                      Footprint.X / 2.0D) * modProgram.TerrainGridSpacing);
            Result.Horizontal.Y =
                (int)
                    ((Math.Round(Convert.ToDouble((Horizontal.Y - Footprint.Y * modProgram.TerrainGridSpacing / 2.0D) / modProgram.TerrainGridSpacing)) +
                      Footprint.Y / 2.0D) * modProgram.TerrainGridSpacing);
            Result.Altitude = (int)(GetTerrainHeight(Result.Horizontal));

            return Result;
        }

        public void UnitSectorsCalc(clsUnit Unit)
        {
            modMath.sXY_int Start = new modMath.sXY_int();
            modMath.sXY_int Finish = new modMath.sXY_int();
            modMath.sXY_int TileStart = new modMath.sXY_int();
            modMath.sXY_int TileFinish = new modMath.sXY_int();
            clsUnitSectorConnection Connection;
            int X = 0;
            int Y = 0;

            GetFootprintTileRangeClamped(Unit.Pos.Horizontal, Unit.Type.get_GetFootprintSelected(Unit.Rotation), TileStart, TileFinish);
            Start = GetTileSectorNum(TileStart);
            Finish = GetTileSectorNum(TileFinish);
            Start.X = modMath.Clamp_int(Start.X, 0, SectorCount.X - 1);
            Start.Y = modMath.Clamp_int(Start.Y, 0, SectorCount.Y - 1);
            Finish.X = modMath.Clamp_int(Finish.X, 0, SectorCount.X - 1);
            Finish.Y = modMath.Clamp_int(Finish.Y, 0, SectorCount.Y - 1);
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
            if ( !modSettings.Settings.AutoSaveEnabled )
            {
                return;
            }
            if ( AutoSave.ChangeCount < modSettings.Settings.AutoSaveMinChanges )
            {
                return;
            }
            if (
                DateAndTime.DateDiff("s", AutoSave.SavedDate, DateTime.Now, (FirstDayOfWeek)FirstDayOfWeek.Sunday,
                    (FirstWeekOfYear)FirstWeekOfYear.Jan1) < modSettings.Settings.AutoSaveMinInterval_s )
            {
                return;
            }

            AutoSave.ChangeCount = 0;
            AutoSave.SavedDate = DateTime.Now;

            modProgram.ShowWarnings(AutoSavePerform());
        }

        public clsResult AutoSavePerform()
        {
            clsResult ReturnResult = new clsResult("Autosave");

            if ( !Directory.Exists(modProgram.AutoSavePath) )
            {
                try
                {
                    Directory.CreateDirectory(modProgram.AutoSavePath);
                }
                catch ( Exception )
                {
                    ReturnResult.ProblemAdd("Unable to create directory at " + Convert.ToString(ControlChars.Quote) + modProgram.AutoSavePath +
                                            Convert.ToString(ControlChars.Quote));
                }
            }

            DateTime DateNow = DateTime.Now;
            string Path = "";

            Path = modProgram.AutoSavePath + "autosaved-" + modIO.InvariantToString_int(DateNow.Year) + "-" + modProgram.MinDigits(DateNow.Month, 2) + "-" +
                   modProgram.MinDigits(DateNow.Day, 2) + "-" + modProgram.MinDigits(DateNow.Hour, 2) + "-" + modProgram.MinDigits(DateNow.Minute, 2) + "-" +
                   modProgram.MinDigits(DateNow.Second, 2) + "-" + modProgram.MinDigits(DateNow.Millisecond, 3) + ".fmap";

            ReturnResult.Add(Write_FMap(Path, false, modSettings.Settings.AutoSaveCompress));

            return ReturnResult;
        }

        public void UndoStepCreate(string StepName)
        {
            clsUndo NewUndo = new clsUndo();

            NewUndo.Name = StepName;

            modMath.clsXY_int SectorNum = default(modMath.clsXY_int);
            foreach ( modMath.clsXY_int tempLoopVar_SectorNum in SectorTerrainUndoChanges.ChangedPoints )
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

        public void ShadowSector_Create(modMath.sXY_int SectorNum)
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
            StartX = SectorNum.X * modProgram.SectorTileSize;
            StartY = SectorNum.Y * modProgram.SectorTileSize;
            LastTileX = Math.Min(modProgram.SectorTileSize, Terrain.TileSize.X - StartX);
            LastTileY = Math.Min(modProgram.SectorTileSize, Terrain.TileSize.Y - StartY);
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

            modMath.sXY_int SectorNum = new modMath.sXY_int();
            clsShadowSector CurrentSector = default(clsShadowSector);
            clsShadowSector UndoSector = default(clsShadowSector);
            modLists.SimpleList<clsShadowSector> NewSectorsForThisUndo = new modLists.SimpleList<clsShadowSector>();
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
                    modProgram.ErrorIDChange(ID, Unit, "Undo_Perform");
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
            modMain.frmMainInstance.SelectedObject_Changed();
        }

        public void RedoPerform()
        {
            clsUndo ThisUndo = default(clsUndo);

            ThisUndo = Undos[UndoPosition];

            modMath.sXY_int SectorNum = new modMath.sXY_int();
            clsShadowSector CurrentSector = default(clsShadowSector);
            clsShadowSector UndoSector = default(clsShadowSector);
            modLists.SimpleList<clsShadowSector> NewSectorsForThisUndo = new modLists.SimpleList<clsShadowSector>();
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
                    modProgram.ErrorIDChange(ID, Unit, "Redo_Perform");
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
            modMain.frmMainInstance.SelectedObject_Changed();
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

            StartX = Shadow_Sector_To_Rejoin.Num.X * modProgram.SectorTileSize;
            StartZ = Shadow_Sector_To_Rejoin.Num.Y * modProgram.SectorTileSize;
            LastTileX = Math.Min(modProgram.SectorTileSize, Terrain.TileSize.X - StartX);
            LastTileZ = Math.Min(modProgram.SectorTileSize, Terrain.TileSize.Y - StartZ);
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

        public void MapInsert(clsMap MapToInsert, modMath.sXY_int Offset, modMath.sXY_int Area, bool InsertHeights, bool InsertTextures, bool InsertUnits,
            bool DeleteUnits, bool InsertGateways, bool DeleteGateways)
        {
            modMath.sXY_int Finish = new modMath.sXY_int();
            int X = 0;
            int Y = 0;
            modMath.sXY_int SectorStart = new modMath.sXY_int();
            modMath.sXY_int SectorFinish = new modMath.sXY_int();
            modMath.sXY_int AreaAdjusted = new modMath.sXY_int();
            modMath.sXY_int SectorNum = new modMath.sXY_int();

            Finish.X = Math.Min(Offset.X + Math.Min(Area.X, MapToInsert.Terrain.TileSize.X), Terrain.TileSize.X);
            Finish.Y = Math.Min(Offset.Y + Math.Min(Area.Y, MapToInsert.Terrain.TileSize.Y), Terrain.TileSize.Y);
            AreaAdjusted.X = Finish.X - Offset.X;
            AreaAdjusted.Y = Finish.Y - Offset.Y;

            GetTileSectorRange(new modMath.sXY_int(Offset.X - 1, Offset.Y - 1), Finish, ref SectorStart, ref SectorFinish);
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

            modMath.sXY_int LastTile = new modMath.sXY_int();
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
                modMath.sXY_int GateStart = new modMath.sXY_int();
                modMath.sXY_int GateFinish = new modMath.sXY_int();
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
                modLists.SimpleList<clsUnit> UnitsToDelete = new modLists.SimpleList<clsUnit>();
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
                            if ( modProgram.PosIsWithinTileArea(Unit.Pos.Horizontal, Offset, Finish) )
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
                modMath.sXY_int PosDif = new modMath.sXY_int();
                clsUnit NewUnit = default(clsUnit);
                clsUnit Unit = default(clsUnit);
                modMath.sXY_int ZeroPos = new modMath.sXY_int(0, 0);
                clsUnitAdd UnitAdd = new clsUnitAdd();

                UnitAdd.Map = this;
                UnitAdd.StoreChange = true;

                PosDif.X = Offset.X * modProgram.TerrainGridSpacing;
                PosDif.Y = Offset.Y * modProgram.TerrainGridSpacing;
                foreach ( clsUnit tempLoopVar_Unit in MapToInsert.Units )
                {
                    Unit = tempLoopVar_Unit;
                    if ( modProgram.PosIsWithinTileArea(Unit.Pos.Horizontal, ZeroPos, AreaAdjusted) )
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

        public clsGateway GatewayCreate(modMath.sXY_int PosA, modMath.sXY_int PosB)
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

        public clsGateway GatewayCreateStoreChange(modMath.sXY_int PosA, modMath.sXY_int PosB)
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
                    Tile_TypeNum[A] = Tileset.Tiles[A].Default_Type;
                }
            }
        }

        public void SetPainterToDefaults()
        {
            if ( Tileset == null )
            {
                Painter = new clsPainter();
            }
            else if ( Tileset == modProgram.Tileset_Arizona )
            {
                Painter = modProgram.Painter_Arizona;
            }
            else if ( Tileset == modProgram.Tileset_Urban )
            {
                Painter = modProgram.Painter_Urban;
            }
            else if ( Tileset == modProgram.Tileset_Rockies )
            {
                Painter = modProgram.Painter_Rockies;
            }
            else
            {
                Painter = new clsPainter();
            }
        }

        private void UnitSectorsGraphicsChanged(clsUnit UnitToUpdateFor)
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

        public modProgram.sWorldPos GetTileOffsetRotatedWorldPos(modMath.sXY_int Tile, modMath.sXY_int TileOffsetToRotate)
        {
            modProgram.sWorldPos Result = new modProgram.sWorldPos();

            modMath.sXY_int RotatedOffset = new modMath.sXY_int();

            RotatedOffset = TileOrientation.GetTileRotatedOffset(Terrain.Tiles[Tile.X, Tile.Y].Texture.Orientation, TileOffsetToRotate);
            Result.Horizontal.X = Tile.X * modProgram.TerrainGridSpacing + RotatedOffset.X;
            Result.Horizontal.Y = Tile.Y * modProgram.TerrainGridSpacing + RotatedOffset.Y;
            Result.Altitude = (int)(GetTerrainHeight(Result.Horizontal));

            return Result;
        }

        public void GetFootprintTileRangeClamped(modMath.sXY_int Horizontal, modMath.sXY_int Footprint, modMath.sXY_int ResultStart, modMath.sXY_int ResultFinish)
        {
            int Remainder = 0;
            modMath.sXY_int Centre = GetPosTileNum(Horizontal);
            int Half = 0;

            Half = Math.DivRem(Footprint.X, 2, out Remainder);
            ResultStart.X = modMath.Clamp_int(Centre.X - Half, 0, Terrain.TileSize.X - 1);
            ResultFinish.X = modMath.Clamp_int(ResultStart.X + Footprint.X - 1, 0, Terrain.TileSize.X - 1);
            Half = Math.DivRem(Footprint.Y, 2, out Remainder);
            ResultStart.Y = modMath.Clamp_int(Centre.Y - Half, 0, Terrain.TileSize.Y - 1);
            ResultFinish.Y = modMath.Clamp_int(ResultStart.Y + Footprint.Y - 1, 0, Terrain.TileSize.Y - 1);
        }

        public void GetFootprintTileRange(modMath.sXY_int Horizontal, modMath.sXY_int Footprint, modMath.sXY_int ResultStart, modMath.sXY_int ResultFinish)
        {
            int Remainder = 0;
            modMath.sXY_int Centre = GetPosTileNum(Horizontal);
            int Half = 0;

            Half = Math.DivRem(Footprint.X, 2, out Remainder);
            ResultStart.X = Centre.X - Half;
            ResultFinish.X = ResultStart.X + Footprint.X - 1;
            Half = Math.DivRem(Footprint.Y, 2, out Remainder);
            ResultStart.Y = Centre.Y - Half;
            ResultFinish.Y = ResultStart.Y + Footprint.Y - 1;
        }

        public modMath.sXY_int GetPosTileNum(modMath.sXY_int Horizontal)
        {
            modMath.sXY_int Result = new modMath.sXY_int();

            Result.X = (int)(Conversion.Int(Horizontal.X / modProgram.TerrainGridSpacing));
            Result.Y = Conversion.Int(Horizontal.Y / modProgram.TerrainGridSpacing);

            return Result;
        }

        public modMath.sXY_int GetPosVertexNum(modMath.sXY_int Horizontal)
        {
            modMath.sXY_int Result = new modMath.sXY_int();

            Result.X = (int)(Math.Round((double)(Horizontal.X / modProgram.TerrainGridSpacing)));
            Result.Y = (int)(Math.Round((double)(Horizontal.Y / modProgram.TerrainGridSpacing)));

            return Result;
        }

        public modMath.sXY_int GetPosSectorNum(modMath.sXY_int Horizontal)
        {
            modMath.sXY_int Result = new modMath.sXY_int();

            Result = GetTileSectorNum(GetPosTileNum(Horizontal));

            return Result;
        }

        public modMath.sXY_int GetSectorNumClamped(modMath.sXY_int SectorNum)
        {
            modMath.sXY_int Result = new modMath.sXY_int();

            Result.X = modMath.Clamp_int(SectorNum.X, 0, SectorCount.X - 1);
            Result.Y = modMath.Clamp_int(SectorNum.Y, 0, SectorCount.Y - 1);

            return Result;
        }

        public int GetVertexAltitude(modMath.sXY_int VertexNum)
        {
            return Terrain.Vertices[VertexNum.X, VertexNum.Y].Height * HeightMultiplier;
        }

        public bool PosIsOnMap(modMath.sXY_int Horizontal)
        {
            return modProgram.PosIsWithinTileArea(Horizontal, new modMath.sXY_int(0, 0), Terrain.TileSize);
        }

        public modMath.sXY_int TileNumClampToMap(modMath.sXY_int TileNum)
        {
            modMath.sXY_int Result = new modMath.sXY_int();

            Result.X = modMath.Clamp_int(TileNum.X, 0, Terrain.TileSize.X - 1);
            Result.Y = modMath.Clamp_int(TileNum.Y, 0, Terrain.TileSize.Y - 1);

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

            SectorCount.X = (int)(Math.Ceiling((double)(Terrain.TileSize.X / modProgram.SectorTileSize)));
            SectorCount.Y = (int)(Math.Ceiling((double)(Terrain.TileSize.Y / modProgram.SectorTileSize)));
            Sectors = new clsSector[SectorCount.X, SectorCount.Y];
            for ( Y = 0; Y <= SectorCount.Y - 1; Y++ )
            {
                for ( X = 0; X <= SectorCount.X - 1; X++ )
                {
                    Sectors[X, Y] = new clsSector(new modMath.sXY_int(X, Y));
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
                    ShadowSector_Create(new modMath.sXY_int(X, Y));
                }
            }

            SectorGraphicsChanges = new clsSectorChanges(this);
            SectorGraphicsChanges.SetAllChanged();
            SectorUnitHeightsChanges = new clsSectorChanges(this);
            SectorTerrainUndoChanges = new clsSectorChanges(this);
            AutoTextureChanges = new clsAutoTextureChanges(this);
            TerrainInterpretChanges = new clsTerrainUpdate(Terrain.TileSize);

            UnitChanges = new modLists.SimpleClassList<clsUnitChange>();
            UnitChanges.MaintainOrder = true;
            GatewayChanges = new modLists.SimpleClassList<clsGatewayChange>();
            GatewayChanges.MaintainOrder = true;
            Undos = new modLists.SimpleClassList<clsUndo>();
            Undos.MaintainOrder = true;
            UndoPosition = 0;

            SelectedUnits = new modLists.ConnectedList<clsUnit, clsMap>(this);

            if ( InterfaceOptions == null )
            {
                InterfaceOptions = new clsInterfaceOptions();
            }

            ViewInfo = new clsViewInfo(this, modMain.frmMainInstance.MapView);

            _SelectedUnitGroup = new clsUnitGroupContainer();
            SelectedUnitGroup.Item = ScavengerUnitGroup;

            Messages = new modLists.SimpleClassList<clsMessage>();
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
                modProgram.sSplitPath SplitPath = new modProgram.sSplitPath(PathInfo.Path);
                return SplitPath.FilePath;
            }
        }

        public class clsUpdateSectorGraphics : clsAction
        {
            public override void ActionPerform()
            {
                Map.Sector_GLList_Make(PosNum.X, PosNum.Y);
                Map.MinimapMakeLater();
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
            UpdateAutotextures.MakeInvalidTiles = modMain.frmMainInstance.cbxInvalidTiles.Checked;

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

        public class clsUpdateSectorUnitHeights : clsAction
        {
            private clsUnit NewUnit;
            private UInt32 ID;
            private clsUnit[] OldUnits;
            private int OldUnitCount = 0;
            private int NewAltitude;
            private bool Started;

            public void Start()
            {
                OldUnits = new clsUnit[Map.Units.Count];

                Started = true;
            }

            public void Finish()
            {
                if ( !Started )
                {
                    Debugger.Break();
                    return;
                }

                int A = 0;
                clsUnitAdd UnitAdd = new clsUnitAdd();
                clsUnit Unit = default(clsUnit);

                UnitAdd.Map = Map;
                UnitAdd.StoreChange = true;

                for ( A = 0; A <= OldUnitCount - 1; A++ )
                {
                    Unit = OldUnits[A];
                    NewAltitude = (int)(Map.GetTerrainHeight(Unit.Pos.Horizontal));
                    if ( NewAltitude != Unit.Pos.Altitude )
                    {
                        NewUnit = new clsUnit(Unit, Map);
                        ID = Unit.ID;
                        //NewUnit.Pos.Altitude = NewAltitude
                        //these create changed sectors and must be done before drawing the new sectors
                        Map.UnitRemoveStoreChange(Unit.MapLink.ArrayPosition);
                        UnitAdd.NewUnit = NewUnit;
                        UnitAdd.ID = ID;
                        UnitAdd.Perform();
                        modProgram.ErrorIDChange(ID, NewUnit, "UpdateSectorUnitHeights");
                    }
                }

                Started = false;
            }

            public override void ActionPerform()
            {
                if ( !Started )
                {
                    Debugger.Break();
                    return;
                }

                clsUnitSectorConnection Connection = default(clsUnitSectorConnection);
                clsUnit Unit = default(clsUnit);
                clsSector Sector = default(clsSector);
                int A = 0;

                Sector = Map.Sectors[PosNum.X, PosNum.Y];
                foreach ( clsUnitSectorConnection tempLoopVar_Connection in Sector.Units )
                {
                    Connection = tempLoopVar_Connection;
                    Unit = Connection.Unit;
                    //units can be in multiple sectors, so dont include multiple times
                    for ( A = 0; A <= OldUnitCount - 1; A++ )
                    {
                        if ( OldUnits[A] == Unit )
                        {
                            break;
                        }
                    }
                    if ( A == OldUnitCount )
                    {
                        OldUnits[OldUnitCount] = Unit;
                        OldUnitCount++;
                    }
                }
            }
        }

        public class clsUpdateAutotexture : clsAction
        {
            public bool MakeInvalidTiles;

            private clsPainter.clsTerrain Terrain_Inner;
            private clsPainter.clsTerrain Terrain_Outer;
            private clsPainter.clsRoad Road;
            private bool RoadTop;
            private bool RoadLeft;
            private bool RoadRight;
            private bool RoadBottom;
            private clsPainter Painter;
            private clsTerrain Terrain;
            private clsPainter.clsTileList ResultTiles;
            private TileOrientation.sTileDirection ResultDirection;
            private clsPainter.clsTileList.sTileOrientationChance ResultTexture;

            public override void ActionPerform()
            {
                Terrain = Map.Terrain;

                Painter = Map.Painter;

                ResultTiles = null;
                ResultDirection = TileOrientation.TileDirection_None;

                //apply centre brushes
                if ( !Terrain.Tiles[PosNum.X, PosNum.Y].Terrain_IsCliff )
                {
                    for ( int BrushNum = 0; BrushNum <= Painter.TerrainCount - 1; BrushNum++ )
                    {
                        Terrain_Inner = Painter.Terrains[BrushNum];
                        if ( Terrain.Vertices[PosNum.X, PosNum.Y].Terrain == Terrain_Inner )
                        {
                            if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == Terrain_Inner )
                            {
                                if ( Terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == Terrain_Inner )
                                {
                                    if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Inner )
                                    {
                                        //i i i i
                                        ResultTiles = Terrain_Inner.Tiles;
                                        ResultDirection = TileOrientation.TileDirection_None;
                                    }
                                }
                            }
                        }
                    }
                }

                //apply transition brushes
                if ( !Terrain.Tiles[PosNum.X, PosNum.Y].Terrain_IsCliff )
                {
                    for ( int BrushNum = 0; BrushNum <= Painter.TransitionBrushCount - 1; BrushNum++ )
                    {
                        Terrain_Inner = Painter.TransitionBrushes[BrushNum].Terrain_Inner;
                        Terrain_Outer = Painter.TransitionBrushes[BrushNum].Terrain_Outer;
                        if ( Terrain.Vertices[PosNum.X, PosNum.Y].Terrain == Terrain_Inner )
                        {
                            if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == Terrain_Inner )
                            {
                                if ( Terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == Terrain_Inner )
                                {
                                    if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Inner )
                                    {
                                        //i i i i
                                        //nothing to do here
                                        break;
                                    }
                                    else if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Outer )
                                    {
                                        //i i i o
                                        ResultTiles = Painter.TransitionBrushes[BrushNum].Tiles_Corner_In;
                                        ResultDirection = TileOrientation.TileDirection_BottomRight;
                                        break;
                                    }
                                }
                                else if ( Terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == Terrain_Outer )
                                {
                                    if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Inner )
                                    {
                                        //i i o i
                                        ResultTiles = Painter.TransitionBrushes[BrushNum].Tiles_Corner_In;
                                        ResultDirection = TileOrientation.TileDirection_BottomLeft;
                                        break;
                                    }
                                    else if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Outer )
                                    {
                                        //i i o o
                                        ResultTiles = Painter.TransitionBrushes[BrushNum].Tiles_Straight;
                                        ResultDirection = TileOrientation.TileDirection_Bottom;
                                        break;
                                    }
                                }
                            }
                            else if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == Terrain_Outer )
                            {
                                if ( Terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == Terrain_Inner )
                                {
                                    if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Inner )
                                    {
                                        //i o i i
                                        ResultTiles = Painter.TransitionBrushes[BrushNum].Tiles_Corner_In;
                                        ResultDirection = TileOrientation.TileDirection_TopRight;
                                        break;
                                    }
                                    else if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Outer )
                                    {
                                        //i o i o
                                        ResultTiles = Painter.TransitionBrushes[BrushNum].Tiles_Straight;
                                        ResultDirection = TileOrientation.TileDirection_Right;
                                        break;
                                    }
                                }
                                else if ( Terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == Terrain_Outer )
                                {
                                    if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Inner )
                                    {
                                        //i o o i
                                        ResultTiles = null;
                                        ResultDirection = TileOrientation.TileDirection_None;
                                        break;
                                    }
                                    else if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Outer )
                                    {
                                        //i o o o
                                        ResultTiles = Painter.TransitionBrushes[BrushNum].Tiles_Corner_Out;
                                        ResultDirection = TileOrientation.TileDirection_BottomRight;
                                        break;
                                    }
                                }
                            }
                        }
                        else if ( Terrain.Vertices[PosNum.X, PosNum.Y].Terrain == Terrain_Outer )
                        {
                            if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == Terrain_Inner )
                            {
                                if ( Terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == Terrain_Inner )
                                {
                                    if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Inner )
                                    {
                                        //o i i i
                                        ResultTiles = Painter.TransitionBrushes[BrushNum].Tiles_Corner_In;
                                        ResultDirection = TileOrientation.TileDirection_TopLeft;
                                        break;
                                    }
                                    else if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Outer )
                                    {
                                        //o i i o
                                        ResultTiles = null;
                                        ResultDirection = TileOrientation.TileDirection_None;
                                        break;
                                    }
                                }
                                else if ( Terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == Terrain_Outer )
                                {
                                    if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Inner )
                                    {
                                        //o i o i
                                        ResultTiles = Painter.TransitionBrushes[BrushNum].Tiles_Straight;
                                        ResultDirection = TileOrientation.TileDirection_Left;
                                        break;
                                    }
                                    else if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Outer )
                                    {
                                        //o i o o
                                        ResultTiles = Painter.TransitionBrushes[BrushNum].Tiles_Corner_Out;
                                        ResultDirection = TileOrientation.TileDirection_BottomLeft;
                                        break;
                                    }
                                }
                            }
                            else if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == Terrain_Outer )
                            {
                                if ( Terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == Terrain_Inner )
                                {
                                    if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Inner )
                                    {
                                        //o o i i
                                        ResultTiles = Painter.TransitionBrushes[BrushNum].Tiles_Straight;
                                        ResultDirection = TileOrientation.TileDirection_Top;
                                        break;
                                    }
                                    else if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Outer )
                                    {
                                        //o o i o
                                        ResultTiles = Painter.TransitionBrushes[BrushNum].Tiles_Corner_Out;
                                        ResultDirection = TileOrientation.TileDirection_TopRight;
                                        break;
                                    }
                                }
                                else if ( Terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == Terrain_Outer )
                                {
                                    if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Inner )
                                    {
                                        //o o o i
                                        ResultTiles = Painter.TransitionBrushes[BrushNum].Tiles_Corner_Out;
                                        ResultDirection = TileOrientation.TileDirection_TopLeft;
                                        break;
                                    }
                                    else if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Outer )
                                    {
                                        //o o o o
                                        //nothing to do here
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }

                //set cliff tiles
                if ( Terrain.Tiles[PosNum.X, PosNum.Y].Tri )
                {
                    if ( Terrain.Tiles[PosNum.X, PosNum.Y].TriTopLeftIsCliff )
                    {
                        if ( Terrain.Tiles[PosNum.X, PosNum.Y].TriBottomRightIsCliff )
                        {
                            int BrushNum = 0;
                            for ( BrushNum = 0; BrushNum <= Painter.CliffBrushCount - 1; BrushNum++ )
                            {
                                Terrain_Inner = Painter.CliffBrushes[BrushNum].Terrain_Inner;
                                Terrain_Outer = Painter.CliffBrushes[BrushNum].Terrain_Outer;
                                if ( Terrain_Inner == Terrain_Outer )
                                {
                                    int A = 0;
                                    if ( Terrain.Vertices[PosNum.X, PosNum.Y].Terrain == Terrain_Inner )
                                    {
                                        A++;
                                    }
                                    if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == Terrain_Inner )
                                    {
                                        A++;
                                    }
                                    if ( Terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == Terrain_Inner )
                                    {
                                        A++;
                                    }
                                    if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Inner )
                                    {
                                        A++;
                                    }
                                    if ( A >= 3 )
                                    {
                                        ResultTiles = Painter.CliffBrushes[BrushNum].Tiles_Straight;
                                        ResultDirection = Terrain.Tiles[PosNum.X, PosNum.Y].DownSide;
                                        break;
                                    }
                                }
                                if ( ((Terrain.Vertices[PosNum.X, PosNum.Y].Terrain == Terrain_Inner && Terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == Terrain_Inner) &&
                                      (Terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == Terrain_Outer ||
                                       Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Outer)) ||
                                     ((Terrain.Vertices[PosNum.X, PosNum.Y].Terrain == Terrain_Inner || Terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == Terrain_Inner) &&
                                      (Terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == Terrain_Outer &&
                                       Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Outer)) )
                                {
                                    ResultTiles = Painter.CliffBrushes[BrushNum].Tiles_Straight;
                                    ResultDirection = TileOrientation.TileDirection_Bottom;
                                    break;
                                }
                                else if ( ((Terrain.Vertices[PosNum.X, PosNum.Y].Terrain == Terrain_Outer &&
                                            Terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == Terrain_Outer) &&
                                           (Terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == Terrain_Inner ||
                                            Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Inner)) ||
                                          ((Terrain.Vertices[PosNum.X, PosNum.Y].Terrain == Terrain_Outer ||
                                            Terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == Terrain_Outer) &&
                                           (Terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == Terrain_Inner &&
                                            Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Inner)) )
                                {
                                    ResultTiles = Painter.CliffBrushes[BrushNum].Tiles_Straight;
                                    ResultDirection = TileOrientation.TileDirection_Left;
                                    break;
                                }
                                else if ( ((Terrain.Vertices[PosNum.X, PosNum.Y].Terrain == Terrain_Outer &&
                                            Terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == Terrain_Outer) &&
                                           (Terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == Terrain_Inner ||
                                            Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Inner)) ||
                                          ((Terrain.Vertices[PosNum.X, PosNum.Y].Terrain == Terrain_Outer ||
                                            Terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == Terrain_Outer) &&
                                           (Terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == Terrain_Inner &&
                                            Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Inner)) )
                                {
                                    ResultTiles = Painter.CliffBrushes[BrushNum].Tiles_Straight;
                                    ResultDirection = TileOrientation.TileDirection_Top;
                                    break;
                                }
                                else if ( ((Terrain.Vertices[PosNum.X, PosNum.Y].Terrain == Terrain_Inner &&
                                            Terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == Terrain_Inner) &&
                                           (Terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == Terrain_Outer ||
                                            Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Outer)) ||
                                          ((Terrain.Vertices[PosNum.X, PosNum.Y].Terrain == Terrain_Inner ||
                                            Terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == Terrain_Inner) &&
                                           (Terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == Terrain_Outer &&
                                            Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Outer)) )
                                {
                                    ResultTiles = Painter.CliffBrushes[BrushNum].Tiles_Straight;
                                    ResultDirection = TileOrientation.TileDirection_Right;
                                    break;
                                }
                            }
                            if ( BrushNum == Painter.CliffBrushCount )
                            {
                                ResultTiles = null;
                                ResultDirection = TileOrientation.TileDirection_None;
                            }
                        }
                        else
                        {
                            int BrushNum = 0;
                            for ( BrushNum = 0; BrushNum <= Painter.CliffBrushCount - 1; BrushNum++ )
                            {
                                Terrain_Inner = Painter.CliffBrushes[BrushNum].Terrain_Inner;
                                Terrain_Outer = Painter.CliffBrushes[BrushNum].Terrain_Outer;
                                if ( Terrain.Vertices[PosNum.X, PosNum.Y].Terrain == Terrain_Outer )
                                {
                                    int A = 0;
                                    if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == Terrain_Inner )
                                    {
                                        A++;
                                    }
                                    if ( Terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == Terrain_Inner )
                                    {
                                        A++;
                                    }
                                    if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Inner )
                                    {
                                        A++;
                                    }
                                    if ( A >= 2 )
                                    {
                                        ResultTiles = Painter.CliffBrushes[BrushNum].Tiles_Corner_In;
                                        ResultDirection = TileOrientation.TileDirection_TopLeft;
                                        break;
                                    }
                                }
                                else if ( Terrain.Vertices[PosNum.X, PosNum.Y].Terrain == Terrain_Inner )
                                {
                                    int A = 0;
                                    if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == Terrain_Outer )
                                    {
                                        A++;
                                    }
                                    if ( Terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == Terrain_Outer )
                                    {
                                        A++;
                                    }
                                    if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Outer )
                                    {
                                        A++;
                                    }
                                    if ( A >= 2 )
                                    {
                                        ResultTiles = Painter.CliffBrushes[BrushNum].Tiles_Corner_Out;
                                        ResultDirection = TileOrientation.TileDirection_BottomRight;
                                        break;
                                    }
                                }
                            }
                            if ( BrushNum == Painter.CliffBrushCount )
                            {
                                ResultTiles = null;
                                ResultDirection = TileOrientation.TileDirection_None;
                            }
                        }
                    }
                    else if ( Terrain.Tiles[PosNum.X, PosNum.Y].TriBottomRightIsCliff )
                    {
                        int BrushNum = 0;
                        for ( BrushNum = 0; BrushNum <= Painter.CliffBrushCount - 1; BrushNum++ )
                        {
                            Terrain_Inner = Painter.CliffBrushes[BrushNum].Terrain_Inner;
                            Terrain_Outer = Painter.CliffBrushes[BrushNum].Terrain_Outer;
                            if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Outer )
                            {
                                int A = 0;
                                if ( Terrain.Vertices[PosNum.X, PosNum.Y].Terrain == Terrain_Inner )
                                {
                                    A++;
                                }
                                if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == Terrain_Inner )
                                {
                                    A++;
                                }
                                if ( Terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == Terrain_Inner )
                                {
                                    A++;
                                }
                                if ( A >= 2 )
                                {
                                    ResultTiles = Painter.CliffBrushes[BrushNum].Tiles_Corner_In;
                                    ResultDirection = TileOrientation.TileDirection_BottomRight;
                                    break;
                                }
                            }
                            else if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Inner )
                            {
                                int A = 0;
                                if ( Terrain.Vertices[PosNum.X, PosNum.Y].Terrain == Terrain_Outer )
                                {
                                    A++;
                                }
                                if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == Terrain_Outer )
                                {
                                    A++;
                                }
                                if ( Terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == Terrain_Outer )
                                {
                                    A++;
                                }
                                if ( A >= 2 )
                                {
                                    ResultTiles = Painter.CliffBrushes[BrushNum].Tiles_Corner_Out;
                                    ResultDirection = TileOrientation.TileDirection_TopLeft;
                                    break;
                                }
                            }
                        }
                        if ( BrushNum == Painter.CliffBrushCount )
                        {
                            ResultTiles = null;
                            ResultDirection = TileOrientation.TileDirection_None;
                        }
                    }
                    else
                    {
                        //no cliff
                    }
                }
                else
                {
                    //default tri orientation
                    if ( Terrain.Tiles[PosNum.X, PosNum.Y].TriTopRightIsCliff )
                    {
                        if ( Terrain.Tiles[PosNum.X, PosNum.Y].TriBottomLeftIsCliff )
                        {
                            int BrushNum = 0;
                            for ( BrushNum = 0; BrushNum <= Painter.CliffBrushCount - 1; BrushNum++ )
                            {
                                Terrain_Inner = Painter.CliffBrushes[BrushNum].Terrain_Inner;
                                Terrain_Outer = Painter.CliffBrushes[BrushNum].Terrain_Outer;
                                if ( Terrain_Inner == Terrain_Outer )
                                {
                                    int A = 0;
                                    if ( Terrain.Vertices[PosNum.X, PosNum.Y].Terrain == Terrain_Inner )
                                    {
                                        A++;
                                    }
                                    if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == Terrain_Inner )
                                    {
                                        A++;
                                    }
                                    if ( Terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == Terrain_Inner )
                                    {
                                        A++;
                                    }
                                    if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Inner )
                                    {
                                        A++;
                                    }
                                    if ( A >= 3 )
                                    {
                                        ResultTiles = Painter.CliffBrushes[BrushNum].Tiles_Straight;
                                        ResultDirection = Terrain.Tiles[PosNum.X, PosNum.Y].DownSide;
                                        break;
                                    }
                                }
                                if ( ((Terrain.Vertices[PosNum.X, PosNum.Y].Terrain == Terrain_Inner && Terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == Terrain_Inner) &&
                                      (Terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == Terrain_Outer ||
                                       Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Outer)) ||
                                     ((Terrain.Vertices[PosNum.X, PosNum.Y].Terrain == Terrain_Inner || Terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == Terrain_Inner) &&
                                      (Terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == Terrain_Outer &&
                                       Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Outer)) )
                                {
                                    ResultTiles = Painter.CliffBrushes[BrushNum].Tiles_Straight;
                                    ResultDirection = TileOrientation.TileDirection_Bottom;
                                    break;
                                }
                                else if ( ((Terrain.Vertices[PosNum.X, PosNum.Y].Terrain == Terrain_Outer &&
                                            Terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == Terrain_Outer) &&
                                           (Terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == Terrain_Inner ||
                                            Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Inner)) ||
                                          ((Terrain.Vertices[PosNum.X, PosNum.Y].Terrain == Terrain_Outer ||
                                            Terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == Terrain_Outer) &&
                                           (Terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == Terrain_Inner &&
                                            Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Inner)) )
                                {
                                    ResultTiles = Painter.CliffBrushes[BrushNum].Tiles_Straight;
                                    ResultDirection = TileOrientation.TileDirection_Left;
                                    break;
                                }
                                else if ( ((Terrain.Vertices[PosNum.X, PosNum.Y].Terrain == Terrain_Outer &&
                                            Terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == Terrain_Outer) &&
                                           (Terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == Terrain_Inner ||
                                            Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Inner)) ||
                                          ((Terrain.Vertices[PosNum.X, PosNum.Y].Terrain == Terrain_Outer ||
                                            Terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == Terrain_Outer) &&
                                           (Terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == Terrain_Inner &&
                                            Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Inner)) )
                                {
                                    ResultTiles = Painter.CliffBrushes[BrushNum].Tiles_Straight;
                                    ResultDirection = TileOrientation.TileDirection_Top;
                                    break;
                                }
                                else if ( ((Terrain.Vertices[PosNum.X, PosNum.Y].Terrain == Terrain_Inner &&
                                            Terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == Terrain_Inner) &&
                                           (Terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == Terrain_Outer ||
                                            Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Outer)) ||
                                          ((Terrain.Vertices[PosNum.X, PosNum.Y].Terrain == Terrain_Inner ||
                                            Terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == Terrain_Inner) &&
                                           (Terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == Terrain_Outer &&
                                            Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Outer)) )
                                {
                                    ResultTiles = Painter.CliffBrushes[BrushNum].Tiles_Straight;
                                    ResultDirection = TileOrientation.TileDirection_Right;
                                    break;
                                }
                            }
                            if ( BrushNum == Painter.CliffBrushCount )
                            {
                                ResultTiles = null;
                                ResultDirection = TileOrientation.TileDirection_None;
                            }
                        }
                        else
                        {
                            int BrushNum = 0;
                            for ( BrushNum = 0; BrushNum <= Painter.CliffBrushCount - 1; BrushNum++ )
                            {
                                Terrain_Inner = Painter.CliffBrushes[BrushNum].Terrain_Inner;
                                Terrain_Outer = Painter.CliffBrushes[BrushNum].Terrain_Outer;
                                if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == Terrain_Outer )
                                {
                                    int A = 0;
                                    if ( Terrain.Vertices[PosNum.X, PosNum.Y].Terrain == Terrain_Inner )
                                    {
                                        A++;
                                    }
                                    if ( Terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == Terrain_Inner )
                                    {
                                        A++;
                                    }
                                    if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Inner )
                                    {
                                        A++;
                                    }
                                    if ( A >= 2 )
                                    {
                                        ResultTiles = Painter.CliffBrushes[BrushNum].Tiles_Corner_In;
                                        ResultDirection = TileOrientation.TileDirection_TopRight;
                                        break;
                                    }
                                }
                                else if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == Terrain_Inner )
                                {
                                    int A = 0;
                                    if ( Terrain.Vertices[PosNum.X, PosNum.Y].Terrain == Terrain_Outer )
                                    {
                                        A++;
                                    }
                                    if ( Terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == Terrain_Outer )
                                    {
                                        A++;
                                    }
                                    if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Outer )
                                    {
                                        A++;
                                    }
                                    if ( A >= 2 )
                                    {
                                        ResultTiles = Painter.CliffBrushes[BrushNum].Tiles_Corner_Out;
                                        ResultDirection = TileOrientation.TileDirection_BottomLeft;
                                        break;
                                    }
                                }
                            }
                            if ( BrushNum == Painter.CliffBrushCount )
                            {
                                ResultTiles = null;
                                ResultDirection = TileOrientation.TileDirection_None;
                            }
                        }
                    }
                    else if ( Terrain.Tiles[PosNum.X, PosNum.Y].TriBottomLeftIsCliff )
                    {
                        int BrushNum = 0;
                        for ( BrushNum = 0; BrushNum <= Painter.CliffBrushCount - 1; BrushNum++ )
                        {
                            Terrain_Inner = Painter.CliffBrushes[BrushNum].Terrain_Inner;
                            Terrain_Outer = Painter.CliffBrushes[BrushNum].Terrain_Outer;
                            if ( Terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == Terrain_Outer )
                            {
                                int A = 0;
                                if ( Terrain.Vertices[PosNum.X, PosNum.Y].Terrain == Terrain_Inner )
                                {
                                    A++;
                                }
                                if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == Terrain_Inner )
                                {
                                    A++;
                                }
                                if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Inner )
                                {
                                    A++;
                                }
                                if ( A >= 2 )
                                {
                                    ResultTiles = Painter.CliffBrushes[BrushNum].Tiles_Corner_In;
                                    ResultDirection = TileOrientation.TileDirection_BottomLeft;
                                    break;
                                }
                            }
                            else if ( Terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == Terrain_Inner )
                            {
                                int A = 0;
                                if ( Terrain.Vertices[PosNum.X, PosNum.Y].Terrain == Terrain_Outer )
                                {
                                    A++;
                                }
                                if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == Terrain_Outer )
                                {
                                    A++;
                                }
                                if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Outer )
                                {
                                    A++;
                                }
                                if ( A >= 2 )
                                {
                                    ResultTiles = Painter.CliffBrushes[BrushNum].Tiles_Corner_Out;
                                    ResultDirection = TileOrientation.TileDirection_TopRight;
                                    break;
                                }
                            }
                        }
                        if ( BrushNum == Painter.CliffBrushCount )
                        {
                            ResultTiles = null;
                            ResultDirection = TileOrientation.TileDirection_None;
                        }
                    }
                    else
                    {
                        //no cliff
                    }
                }

                //apply roads
                Road = null;
                if ( Terrain.SideH[PosNum.X, PosNum.Y].Road != null )
                {
                    Road = Terrain.SideH[PosNum.X, PosNum.Y].Road;
                }
                else if ( Terrain.SideH[PosNum.X, PosNum.Y + 1].Road != null )
                {
                    Road = Terrain.SideH[PosNum.X, PosNum.Y + 1].Road;
                }
                else if ( Terrain.SideV[PosNum.X + 1, PosNum.Y].Road != null )
                {
                    Road = Terrain.SideV[PosNum.X + 1, PosNum.Y].Road;
                }
                else if ( Terrain.SideV[PosNum.X, PosNum.Y].Road != null )
                {
                    Road = Terrain.SideV[PosNum.X, PosNum.Y].Road;
                }
                if ( Road != null )
                {
                    int BrushNum = 0;
                    for ( BrushNum = 0; BrushNum <= Painter.RoadBrushCount - 1; BrushNum++ )
                    {
                        if ( Painter.RoadBrushes[BrushNum].Road == Road )
                        {
                            Terrain_Outer = Painter.RoadBrushes[BrushNum].Terrain;
                            int A = 0;
                            if ( Terrain.Vertices[PosNum.X, PosNum.Y].Terrain == Terrain_Outer )
                            {
                                A++;
                            }
                            if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == Terrain_Outer )
                            {
                                A++;
                            }
                            if ( Terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == Terrain_Outer )
                            {
                                A++;
                            }
                            if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Outer )
                            {
                                A++;
                            }
                            if ( A >= 2 )
                            {
                                break;
                            }
                        }
                    }

                    ResultTiles = null;
                    ResultDirection = TileOrientation.TileDirection_None;

                    if ( BrushNum < Painter.RoadBrushCount )
                    {
                        RoadTop = Terrain.SideH[PosNum.X, PosNum.Y].Road == Road;
                        RoadLeft = Terrain.SideV[PosNum.X, PosNum.Y].Road == Road;
                        RoadRight = Terrain.SideV[PosNum.X + 1, PosNum.Y].Road == Road;
                        RoadBottom = Terrain.SideH[PosNum.X, PosNum.Y + 1].Road == Road;
                        //do cross intersection
                        if ( RoadTop && RoadLeft && RoadRight && RoadBottom )
                        {
                            ResultTiles = Painter.RoadBrushes[BrushNum].Tile_CrossIntersection;
                            ResultDirection = TileOrientation.TileDirection_None;
                            //do T intersection
                        }
                        else if ( RoadTop && RoadLeft && RoadRight )
                        {
                            ResultTiles = Painter.RoadBrushes[BrushNum].Tile_TIntersection;
                            ResultDirection = TileOrientation.TileDirection_Top;
                        }
                        else if ( RoadTop && RoadLeft && RoadBottom )
                        {
                            ResultTiles = Painter.RoadBrushes[BrushNum].Tile_TIntersection;
                            ResultDirection = TileOrientation.TileDirection_Left;
                        }
                        else if ( RoadTop && RoadRight && RoadBottom )
                        {
                            ResultTiles = Painter.RoadBrushes[BrushNum].Tile_TIntersection;
                            ResultDirection = TileOrientation.TileDirection_Right;
                        }
                        else if ( RoadLeft && RoadRight && RoadBottom )
                        {
                            ResultTiles = Painter.RoadBrushes[BrushNum].Tile_TIntersection;
                            ResultDirection = TileOrientation.TileDirection_Bottom;
                            //do straight
                        }
                        else if ( RoadTop && RoadBottom )
                        {
                            ResultTiles = Painter.RoadBrushes[BrushNum].Tile_Straight;
                            if ( VBMath.Rnd() >= 0.5F )
                            {
                                ResultDirection = TileOrientation.TileDirection_Top;
                            }
                            else
                            {
                                ResultDirection = TileOrientation.TileDirection_Bottom;
                            }
                        }
                        else if ( RoadLeft && RoadRight )
                        {
                            ResultTiles = Painter.RoadBrushes[BrushNum].Tile_Straight;
                            if ( VBMath.Rnd() >= 0.5F )
                            {
                                ResultDirection = TileOrientation.TileDirection_Left;
                            }
                            else
                            {
                                ResultDirection = TileOrientation.TileDirection_Right;
                            }
                            //do corner
                        }
                        else if ( RoadTop && RoadLeft )
                        {
                            ResultTiles = Painter.RoadBrushes[BrushNum].Tile_Corner_In;
                            ResultDirection = TileOrientation.TileDirection_TopLeft;
                        }
                        else if ( RoadTop && RoadRight )
                        {
                            ResultTiles = Painter.RoadBrushes[BrushNum].Tile_Corner_In;
                            ResultDirection = TileOrientation.TileDirection_TopRight;
                        }
                        else if ( RoadLeft && RoadBottom )
                        {
                            ResultTiles = Painter.RoadBrushes[BrushNum].Tile_Corner_In;
                            ResultDirection = TileOrientation.TileDirection_BottomLeft;
                        }
                        else if ( RoadRight && RoadBottom )
                        {
                            ResultTiles = Painter.RoadBrushes[BrushNum].Tile_Corner_In;
                            ResultDirection = TileOrientation.TileDirection_BottomRight;
                            //do end
                        }
                        else if ( RoadTop )
                        {
                            ResultTiles = Painter.RoadBrushes[BrushNum].Tile_End;
                            ResultDirection = TileOrientation.TileDirection_Top;
                        }
                        else if ( RoadLeft )
                        {
                            ResultTiles = Painter.RoadBrushes[BrushNum].Tile_End;
                            ResultDirection = TileOrientation.TileDirection_Left;
                        }
                        else if ( RoadRight )
                        {
                            ResultTiles = Painter.RoadBrushes[BrushNum].Tile_End;
                            ResultDirection = TileOrientation.TileDirection_Right;
                        }
                        else if ( RoadBottom )
                        {
                            ResultTiles = Painter.RoadBrushes[BrushNum].Tile_End;
                            ResultDirection = TileOrientation.TileDirection_Bottom;
                        }
                    }
                }

                if ( ResultTiles == null )
                {
                    ResultTexture.TextureNum = -1;
                    ResultTexture.Direction = TileOrientation.TileDirection_None;
                }
                else
                {
                    ResultTexture = ResultTiles.GetRandom();
                }
                if ( ResultTexture.TextureNum < 0 )
                {
                    if ( MakeInvalidTiles )
                    {
                        Terrain.Tiles[PosNum.X, PosNum.Y].Texture = TileOrientation.OrientateTile(ResultTexture, ResultDirection);
                    }
                }
                else
                {
                    Terrain.Tiles[PosNum.X, PosNum.Y].Texture = TileOrientation.OrientateTile(ResultTexture, ResultDirection);
                }

                Map.SectorGraphicsChanges.TileChanged(PosNum);
                Map.SectorTerrainUndoChanges.TileChanged(PosNum);
            }
        }

        public void TileNeedsInterpreting(modMath.sXY_int Pos)
        {
            TerrainInterpretChanges.Tiles.Changed(Pos);
            TerrainInterpretChanges.Vertices.Changed(new modMath.sXY_int(Pos.X, Pos.Y));
            TerrainInterpretChanges.Vertices.Changed(new modMath.sXY_int(Pos.X + 1, Pos.Y));
            TerrainInterpretChanges.Vertices.Changed(new modMath.sXY_int(Pos.X, Pos.Y + 1));
            TerrainInterpretChanges.Vertices.Changed(new modMath.sXY_int(Pos.X + 1, Pos.Y + 1));
            TerrainInterpretChanges.SidesH.Changed(new modMath.sXY_int(Pos.X, Pos.Y));
            TerrainInterpretChanges.SidesH.Changed(new modMath.sXY_int(Pos.X, Pos.Y + 1));
            TerrainInterpretChanges.SidesV.Changed(new modMath.sXY_int(Pos.X, Pos.Y));
            TerrainInterpretChanges.SidesV.Changed(new modMath.sXY_int(Pos.X + 1, Pos.Y));
        }

        public void TileTextureChangeTerrainAction(modMath.sXY_int Pos, modProgram.enumTextureTerrainAction Action)
        {
            switch ( Action )
            {
                case modProgram.enumTextureTerrainAction.Ignore:
                    break;

                case modProgram.enumTextureTerrainAction.Reinterpret:
                    TileNeedsInterpreting(Pos);
                    break;
                case modProgram.enumTextureTerrainAction.Remove:
                    Terrain.Vertices[Pos.X, Pos.Y].Terrain = null;
                    Terrain.Vertices[Pos.X + 1, Pos.Y].Terrain = null;
                    Terrain.Vertices[Pos.X, Pos.Y + 1].Terrain = null;
                    Terrain.Vertices[Pos.X + 1, Pos.Y + 1].Terrain = null;
                    break;
            }
        }

        public class clsInterfaceOptions
        {
            public string CompileName;
            public string CompileMultiPlayers;
            public bool CompileMultiXPlayers;
            public string CompileMultiAuthor;
            public string CompileMultiLicense;
            public bool AutoScrollLimits;
            public modMath.sXY_int ScrollMin;
            public modMath.sXY_uint ScrollMax;
            public int CampaignGameType;

            public clsInterfaceOptions()
            {
                //set to default
                CompileName = "";
                CompileMultiPlayers = modIO.InvariantToString_int(2);
                CompileMultiXPlayers = false;
                CompileMultiAuthor = "";
                CompileMultiLicense = "";
                AutoScrollLimits = true;
                ScrollMin.X = 0;
                ScrollMin.Y = 0;
                ScrollMax.X = 0U;
                ScrollMax.Y = 0U;
                CampaignGameType = -1;
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
                modProgram.sSplitPath SplitPath = new modProgram.sSplitPath(PathInfo.Path);
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

        public bool SideHIsCliffOnBothSides(modMath.sXY_int SideNum)
        {
            modMath.sXY_int TileNum = new modMath.sXY_int();

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

        public bool SideVIsCliffOnBothSides(modMath.sXY_int SideNum)
        {
            modMath.sXY_int TileNum = new modMath.sXY_int();

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

        public bool VertexIsCliffEdge(modMath.sXY_int VertexNum)
        {
            modMath.sXY_int TileNum = new modMath.sXY_int();

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

        public void PerformTileWall(clsWallType WallType, modMath.sXY_int TileNum, bool Expand)
        {
            modMath.sXY_int SectorNum = new modMath.sXY_int();
            clsUnit Unit = default(clsUnit);
            modMath.sXY_int UnitTile = new modMath.sXY_int();
            modMath.sXY_int Difference = new modMath.sXY_int();
            modProgram.enumTileWalls TileWalls = modProgram.enumTileWalls.None;
            modLists.SimpleList<clsUnit> Walls = new modLists.SimpleList<clsUnit>();
            modLists.SimpleList<clsUnit> Removals = new modLists.SimpleList<clsUnit>();
            clsUnitType UnitType = default(clsUnitType);
            clsStructureType StructureType = default(clsStructureType);
            int X = 0;
            int Y = 0;
            modMath.sXY_int MinTile = new modMath.sXY_int();
            modMath.sXY_int MaxTile = new modMath.sXY_int();
            clsUnitSectorConnection Connection = default(clsUnitSectorConnection);
            MinTile.X = TileNum.X - 1;
            MinTile.Y = TileNum.Y - 1;
            MaxTile.X = TileNum.X + 1;
            MaxTile.Y = TileNum.Y + 1;
            modMath.sXY_int SectorStart = GetSectorNumClamped(GetTileSectorNum(MinTile));
            modMath.sXY_int SectorFinish = GetSectorNumClamped(GetTileSectorNum(MaxTile));

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
                        UnitType = Unit.Type;
                        if ( UnitType.Type == clsUnitType.enumType.PlayerStructure )
                        {
                            StructureType = (clsStructureType)UnitType;
                            if ( StructureType.WallLink.Source == WallType )
                            {
                                UnitTile = GetPosTileNum(Unit.Pos.Horizontal);
                                Difference.X = UnitTile.X - TileNum.X;
                                Difference.Y = UnitTile.Y - TileNum.Y;
                                if ( Difference.Y == 1 )
                                {
                                    if ( Difference.X == 0 )
                                    {
                                        TileWalls = (modProgram.enumTileWalls)(TileWalls | modProgram.enumTileWalls.Bottom);
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
                                        TileWalls = (modProgram.enumTileWalls)(TileWalls | modProgram.enumTileWalls.Left);
                                        Walls.Add(Unit);
                                    }
                                    else if ( Difference.X == 1 )
                                    {
                                        TileWalls = (modProgram.enumTileWalls)(TileWalls | modProgram.enumTileWalls.Right);
                                        Walls.Add(Unit);
                                    }
                                }
                                else if ( Difference.Y == -1 )
                                {
                                    if ( Difference.X == 0 )
                                    {
                                        TileWalls = (modProgram.enumTileWalls)(TileWalls | modProgram.enumTileWalls.Top);
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
            clsUnitType NewUnitType = WallType.Segments[WallType.TileWalls_Segment[(int)TileWalls]];
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
            NewUnit.Pos = TileAlignedPos(TileNum, new modMath.sXY_int(1, 1));
            NewUnit.Type = NewUnitType;
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
            Dialog.Filter = modProgram.ProgramName + " Map Files (*.fmap)|*.fmap";
            if ( Dialog.ShowDialog(modMain.frmMainInstance) != DialogResult.OK )
            {
                return false;
            }
            modSettings.Settings.SavePath = Path.GetDirectoryName(Dialog.FileName);
            clsResult Result = default(clsResult);
            Result = Write_FMap(Dialog.FileName, true, true);
            if ( !Result.HasProblems )
            {
                PathInfo = new clsPathInfo(Dialog.FileName, true);
                ChangedSinceSave = false;
            }
            modProgram.ShowWarnings(Result);
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
                modProgram.ShowWarnings(Result);
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
                DialogResult Result = Prompt.ShowDialog(modMain.frmMainInstance);
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