namespace FlaME
{
    using Matrix3D;
    using Microsoft.VisualBasic;
    using Microsoft.VisualBasic.CompilerServices;
    using System;
    using System.Collections;
    using System.Windows.Forms;

    public class clsViewInfo
    {
        public float FieldOfViewY;
        public double FOVMultiplier;
        public double FOVMultiplierExponent;
        public clsMap Map;
        public ctrlMapView MapView;
        public clsMouseDown MouseLeftDown;
        public clsMouseOver MouseOver;
        public clsMouseDown MouseRightDown;
        public double Tiles_Per_Minimap_Pixel;
        public Matrix3DMath.Matrix3D ViewAngleMatrix = new Matrix3DMath.Matrix3D();
        public Matrix3DMath.Matrix3D ViewAngleMatrix_Inverted = new Matrix3DMath.Matrix3D();
        public Angles.AngleRPY ViewAngleRPY;
        public modMath.sXYZ_int ViewPos;

        public clsViewInfo(clsMap Map, ctrlMapView MapView)
        {
            this.Map = Map;
            this.MapView = MapView;
            this.ViewPos = new modMath.sXYZ_int(0, 0xc00, 0);
            this.FOV_Multiplier_Set(modSettings.Settings.FOVDefault);
            this.ViewAngleSetToDefault();
            modMath.sXY_int horizontal = new modMath.sXY_int((int) Math.Round((double) (((double) (Map.Terrain.TileSize.X * 0x80)) / 2.0)), (int) Math.Round((double) (((double) (Map.Terrain.TileSize.Y * 0x80)) / 2.0)));
            this.LookAtPos(horizontal);
        }

        public void Apply_Cliff()
        {
            clsMouseOver.clsOverTerrain mouseOverTerrain = this.GetMouseOverTerrain();
            if (mouseOverTerrain != null)
            {
                double num;
                clsMap.clsApplyCliff tool = new clsMap.clsApplyCliff {
                    Map = this.Map
                };
                if (modIO.InvariantParse_dbl(modMain.frmMainInstance.txtAutoCliffSlope.Text, ref num))
                {
                    tool.Angle = modMath.Clamp_dbl(num * 0.017453292519943295, 0.0, 1.5707963267948966);
                    tool.SetTris = modMain.frmMainInstance.cbxCliffTris.Checked;
                    modProgram.CliffBrush.PerformActionMapTiles(tool, mouseOverTerrain.Tile);
                    this.Map.Update();
                    this.MapView.DrawViewLater();
                }
            }
        }

        public void Apply_Cliff_Remove()
        {
            clsMouseOver.clsOverTerrain mouseOverTerrain = this.GetMouseOverTerrain();
            if (mouseOverTerrain != null)
            {
                clsMap.clsApplyCliffRemove tool = new clsMap.clsApplyCliffRemove {
                    Map = this.Map
                };
                modProgram.CliffBrush.PerformActionMapTiles(tool, mouseOverTerrain.Tile);
                this.Map.Update();
                this.MapView.DrawViewLater();
            }
        }

        public void Apply_CliffTriangle(bool Remove)
        {
            clsMouseOver.clsOverTerrain mouseOverTerrain = this.GetMouseOverTerrain();
            if (mouseOverTerrain != null)
            {
                if (Remove)
                {
                    new clsMap.clsApplyCliffTriangleRemove { Map = this.Map, PosNum = mouseOverTerrain.Tile.Normal, Triangle = mouseOverTerrain.Triangle }.ActionPerform();
                }
                else
                {
                    new clsMap.clsApplyCliffTriangle { Map = this.Map, PosNum = mouseOverTerrain.Tile.Normal, Triangle = mouseOverTerrain.Triangle }.ActionPerform();
                }
                this.Map.Update();
                this.MapView.DrawViewLater();
            }
        }

        public void Apply_Gateway()
        {
            clsMouseOver.clsOverTerrain mouseOverTerrain = this.GetMouseOverTerrain();
            if (mouseOverTerrain != null)
            {
                modMath.sXY_int normal = mouseOverTerrain.Tile.Normal;
                if (modControls.KeyboardProfile.Active(modControls.Control_Gateway_Delete))
                {
                    for (int i = 0; i < this.Map.Gateways.Count; i++)
                    {
                        modMath.sXY_int _int2;
                        modMath.sXY_int _int3;
                        modMath.ReorderXY(this.Map.Gateways[i].PosA, this.Map.Gateways[i].PosB, ref _int3, ref _int2);
                        if ((((_int3.X <= normal.X) & (_int2.X >= normal.X)) & (_int3.Y <= normal.Y)) & (_int2.Y >= normal.Y))
                        {
                            this.Map.GatewayRemoveStoreChange(i);
                            this.Map.UndoStepCreate("Gateway Delete");
                            this.Map.MinimapMakeLater();
                            this.MapView.DrawViewLater();
                            break;
                        }
                    }
                }
                else if (this.Map.Selected_Tile_A == null)
                {
                    this.Map.Selected_Tile_A = new modMath.clsXY_int(normal);
                    this.MapView.DrawViewLater();
                }
                else if (((normal.X == this.Map.Selected_Tile_A.X) | (normal.Y == this.Map.Selected_Tile_A.Y)) && (this.Map.GatewayCreateStoreChange(this.Map.Selected_Tile_A.XY, normal) != null))
                {
                    this.Map.UndoStepCreate("Gateway Place");
                    this.Map.Selected_Tile_A = null;
                    this.Map.Selected_Tile_B = null;
                    this.Map.MinimapMakeLater();
                    this.MapView.DrawViewLater();
                }
            }
        }

        public void Apply_Height_Change(double Rate)
        {
            clsMouseOver.clsOverTerrain mouseOverTerrain = this.GetMouseOverTerrain();
            if (mouseOverTerrain != null)
            {
                clsMap.clsApplyHeightChange tool = new clsMap.clsApplyHeightChange {
                    Map = this.Map,
                    Rate = Rate,
                    UseEffect = modMain.frmMainInstance.cbxHeightChangeFade.Checked
                };
                modProgram.HeightBrush.PerformActionMapVertices(tool, mouseOverTerrain.Vertex);
                this.Map.Update();
                this.MapView.DrawViewLater();
            }
        }

        public void Apply_Height_Set(clsBrush Brush, byte Height)
        {
            clsMouseOver.clsOverTerrain mouseOverTerrain = this.GetMouseOverTerrain();
            if (mouseOverTerrain != null)
            {
                clsMap.clsApplyHeightSet tool = new clsMap.clsApplyHeightSet {
                    Map = this.Map,
                    Height = Height
                };
                Brush.PerformActionMapVertices(tool, mouseOverTerrain.Vertex);
                this.Map.Update();
                this.MapView.DrawViewLater();
            }
        }

        public void Apply_HeightSmoothing(double Ratio)
        {
            clsMouseOver.clsOverTerrain mouseOverTerrain = this.GetMouseOverTerrain();
            if (mouseOverTerrain != null)
            {
                modMath.sXY_int _int;
                clsMap.clsApplyHeightSmoothing tool = new clsMap.clsApplyHeightSmoothing {
                    Map = this.Map,
                    Ratio = Ratio
                };
                int num = (int) Math.Round(Math.Ceiling(modProgram.HeightBrush.Radius));
                modMath.sXY_int posNum = modProgram.HeightBrush.GetPosNum(mouseOverTerrain.Vertex);
                tool.Offset.X = modMath.Clamp_int(posNum.X - num, 0, this.Map.Terrain.TileSize.X);
                tool.Offset.Y = modMath.Clamp_int(posNum.Y - num, 0, this.Map.Terrain.TileSize.Y);
                _int.X = modMath.Clamp_int(posNum.X + num, 0, this.Map.Terrain.TileSize.X);
                _int.Y = modMath.Clamp_int(posNum.Y + num, 0, this.Map.Terrain.TileSize.Y);
                tool.AreaTileSize.X = _int.X - tool.Offset.X;
                tool.AreaTileSize.Y = _int.Y - tool.Offset.Y;
                tool.Start();
                modProgram.HeightBrush.PerformActionMapVertices(tool, mouseOverTerrain.Vertex);
                tool.Finish();
                this.Map.Update();
                this.MapView.DrawViewLater();
            }
        }

        public void Apply_Road()
        {
            clsMouseOver.clsOverTerrain mouseOverTerrain = this.GetMouseOverTerrain();
            if (mouseOverTerrain != null)
            {
                modMath.sXY_int _int2;
                modMath.sXY_int _int = mouseOverTerrain.Side_Num;
                if (mouseOverTerrain.Side_IsV)
                {
                    if (this.Map.Terrain.SideV[_int.X, _int.Y].Road != modProgram.SelectedRoad)
                    {
                        this.Map.Terrain.SideV[_int.X, _int.Y].Road = modProgram.SelectedRoad;
                        if (_int.X > 0)
                        {
                            _int2.X = _int.X - 1;
                            _int2.Y = _int.Y;
                            this.Map.AutoTextureChanges.TileChanged(_int2);
                            this.Map.SectorGraphicsChanges.TileChanged(_int2);
                            this.Map.SectorTerrainUndoChanges.TileChanged(_int2);
                        }
                        if (_int.X < this.Map.Terrain.TileSize.X)
                        {
                            _int2 = _int;
                            this.Map.AutoTextureChanges.TileChanged(_int2);
                            this.Map.SectorGraphicsChanges.TileChanged(_int2);
                            this.Map.SectorTerrainUndoChanges.TileChanged(_int2);
                        }
                        this.Map.Update();
                        this.Map.UndoStepCreate("Road Side");
                        this.MapView.DrawViewLater();
                    }
                }
                else if (this.Map.Terrain.SideH[_int.X, _int.Y].Road != modProgram.SelectedRoad)
                {
                    this.Map.Terrain.SideH[_int.X, _int.Y].Road = modProgram.SelectedRoad;
                    if (_int.Y > 0)
                    {
                        _int2.X = _int.X;
                        _int2.Y = _int.Y - 1;
                        this.Map.AutoTextureChanges.TileChanged(_int2);
                        this.Map.SectorGraphicsChanges.TileChanged(_int2);
                        this.Map.SectorTerrainUndoChanges.TileChanged(_int2);
                    }
                    if (_int.Y < this.Map.Terrain.TileSize.X)
                    {
                        _int2 = _int;
                        this.Map.AutoTextureChanges.TileChanged(_int2);
                        this.Map.SectorGraphicsChanges.TileChanged(_int2);
                        this.Map.SectorTerrainUndoChanges.TileChanged(_int2);
                    }
                    this.Map.Update();
                    this.Map.UndoStepCreate("Road Side");
                    this.MapView.DrawViewLater();
                }
            }
        }

        public void Apply_Road_Line_Selection()
        {
            clsMouseOver.clsOverTerrain mouseOverTerrain = this.GetMouseOverTerrain();
            if (mouseOverTerrain != null)
            {
                modMath.sXY_int normal = mouseOverTerrain.Tile.Normal;
                if (this.Map.Selected_Tile_A != null)
                {
                    int y;
                    int x;
                    int num3;
                    modMath.sXY_int _int;
                    if (normal.X == this.Map.Selected_Tile_A.X)
                    {
                        if (normal.Y <= this.Map.Selected_Tile_A.Y)
                        {
                            y = normal.Y;
                            x = this.Map.Selected_Tile_A.Y;
                        }
                        else
                        {
                            y = this.Map.Selected_Tile_A.Y;
                            x = normal.Y;
                        }
                        int num4 = x;
                        for (num3 = y + 1; num3 <= num4; num3++)
                        {
                            this.Map.Terrain.SideH[this.Map.Selected_Tile_A.X, num3].Road = modProgram.SelectedRoad;
                            _int.X = this.Map.Selected_Tile_A.X;
                            _int.Y = num3;
                            this.Map.AutoTextureChanges.SideHChanged(_int);
                            this.Map.SectorGraphicsChanges.SideHChanged(_int);
                            this.Map.SectorTerrainUndoChanges.SideHChanged(_int);
                        }
                        this.Map.Update();
                        this.Map.UndoStepCreate("Road Line");
                        this.Map.Selected_Tile_A = null;
                        this.MapView.DrawViewLater();
                    }
                    else if (normal.Y == this.Map.Selected_Tile_A.Y)
                    {
                        if (normal.X <= this.Map.Selected_Tile_A.X)
                        {
                            y = normal.X;
                            x = this.Map.Selected_Tile_A.X;
                        }
                        else
                        {
                            y = this.Map.Selected_Tile_A.X;
                            x = normal.X;
                        }
                        int num5 = x;
                        for (num3 = y + 1; num3 <= num5; num3++)
                        {
                            this.Map.Terrain.SideV[num3, this.Map.Selected_Tile_A.Y].Road = modProgram.SelectedRoad;
                            _int.X = num3;
                            _int.Y = this.Map.Selected_Tile_A.Y;
                            this.Map.AutoTextureChanges.SideVChanged(_int);
                            this.Map.SectorGraphicsChanges.SideVChanged(_int);
                            this.Map.SectorTerrainUndoChanges.SideVChanged(_int);
                        }
                        this.Map.Update();
                        this.Map.UndoStepCreate("Road Line");
                        this.Map.Selected_Tile_A = null;
                        this.MapView.DrawViewLater();
                    }
                }
                else
                {
                    this.Map.Selected_Tile_A = new modMath.clsXY_int(normal);
                }
            }
        }

        public void Apply_Road_Remove()
        {
            clsMouseOver.clsOverTerrain mouseOverTerrain = this.GetMouseOverTerrain();
            if (mouseOverTerrain != null)
            {
                clsMap.clsApplyRoadRemove tool = new clsMap.clsApplyRoadRemove {
                    Map = this.Map
                };
                modProgram.CliffBrush.PerformActionMapTiles(tool, mouseOverTerrain.Tile);
                this.Map.Update();
                this.MapView.DrawViewLater();
            }
        }

        public void Apply_Terrain()
        {
            clsMouseOver.clsOverTerrain mouseOverTerrain = this.GetMouseOverTerrain();
            if (mouseOverTerrain != null)
            {
                clsMap.clsApplyVertexTerrain tool = new clsMap.clsApplyVertexTerrain {
                    Map = this.Map,
                    VertexTerrain = modProgram.SelectedTerrain
                };
                modProgram.TerrainBrush.PerformActionMapVertices(tool, mouseOverTerrain.Vertex);
                this.Map.Update();
                this.MapView.DrawViewLater();
            }
        }

        public void Apply_Terrain_Fill(modProgram.enumFillCliffAction CliffAction, bool Inside)
        {
            clsMouseOver.clsOverTerrain mouseOverTerrain = this.GetMouseOverTerrain();
            if (mouseOverTerrain != null)
            {
                modMath.sXY_int normal = mouseOverTerrain.Vertex.Normal;
                clsPainter.clsTerrain selectedTerrain = modProgram.SelectedTerrain;
                clsPainter.clsTerrain terrain = this.Map.Terrain.Vertices[normal.X, normal.Y].Terrain;
                if (selectedTerrain != terrain)
                {
                    modMath.sXY_int[] _intArray = new modMath.sXY_int[0x80001];
                    _intArray[0] = normal;
                    int index = 1;
                    int num6 = 0;
                    while (num6 < index)
                    {
                        bool flag;
                        modMath.sXY_int vertexNum = _intArray[num6];
                        if (CliffAction == modProgram.enumFillCliffAction.StopBefore)
                        {
                            flag = this.Map.VertexIsCliffEdge(vertexNum);
                        }
                        else
                        {
                            flag = false;
                        }
                        bool flag2 = false;
                        if (Inside)
                        {
                            if (vertexNum.X > 0)
                            {
                                if ((vertexNum.Y > 0) && ((this.Map.Terrain.Vertices[vertexNum.X - 1, vertexNum.Y - 1].Terrain != terrain) & (this.Map.Terrain.Vertices[vertexNum.X - 1, vertexNum.Y - 1].Terrain != selectedTerrain)))
                                {
                                    flag2 = true;
                                }
                                if ((this.Map.Terrain.Vertices[vertexNum.X - 1, vertexNum.Y].Terrain != terrain) & (this.Map.Terrain.Vertices[vertexNum.X - 1, vertexNum.Y].Terrain != selectedTerrain))
                                {
                                    flag2 = true;
                                }
                                if ((vertexNum.Y < this.Map.Terrain.TileSize.Y) && ((this.Map.Terrain.Vertices[vertexNum.X - 1, vertexNum.Y + 1].Terrain != terrain) & (this.Map.Terrain.Vertices[vertexNum.X - 1, vertexNum.Y + 1].Terrain != selectedTerrain)))
                                {
                                    flag2 = true;
                                }
                            }
                            if ((vertexNum.Y > 0) && ((this.Map.Terrain.Vertices[vertexNum.X, vertexNum.Y - 1].Terrain != terrain) & (this.Map.Terrain.Vertices[vertexNum.X, vertexNum.Y - 1].Terrain != selectedTerrain)))
                            {
                                flag2 = true;
                            }
                            if (vertexNum.X < this.Map.Terrain.TileSize.X)
                            {
                                if ((vertexNum.Y > 0) && ((this.Map.Terrain.Vertices[vertexNum.X + 1, vertexNum.Y - 1].Terrain != terrain) & (this.Map.Terrain.Vertices[vertexNum.X + 1, vertexNum.Y - 1].Terrain != selectedTerrain)))
                                {
                                    flag2 = true;
                                }
                                if ((this.Map.Terrain.Vertices[vertexNum.X + 1, vertexNum.Y].Terrain != terrain) & (this.Map.Terrain.Vertices[vertexNum.X + 1, vertexNum.Y].Terrain != selectedTerrain))
                                {
                                    flag2 = true;
                                }
                                if ((vertexNum.Y < this.Map.Terrain.TileSize.Y) && ((this.Map.Terrain.Vertices[vertexNum.X + 1, vertexNum.Y + 1].Terrain != terrain) & (this.Map.Terrain.Vertices[vertexNum.X + 1, vertexNum.Y + 1].Terrain != selectedTerrain)))
                                {
                                    flag2 = true;
                                }
                            }
                            if ((vertexNum.Y < this.Map.Terrain.TileSize.Y) && ((this.Map.Terrain.Vertices[vertexNum.X, vertexNum.Y + 1].Terrain != terrain) & (this.Map.Terrain.Vertices[vertexNum.X, vertexNum.Y + 1].Terrain != selectedTerrain)))
                            {
                                flag2 = true;
                            }
                        }
                        if (!(flag | flag2) && (this.Map.Terrain.Vertices[vertexNum.X, vertexNum.Y].Terrain == terrain))
                        {
                            modMath.sXY_int _int2;
                            modMath.sXY_int _int4;
                            this.Map.Terrain.Vertices[vertexNum.X, vertexNum.Y].Terrain = selectedTerrain;
                            this.Map.SectorGraphicsChanges.VertexChanged(vertexNum);
                            this.Map.SectorTerrainUndoChanges.VertexChanged(vertexNum);
                            this.Map.AutoTextureChanges.VertexChanged(vertexNum);
                            _int2.X = vertexNum.X + 1;
                            _int2.Y = vertexNum.Y;
                            if ((((_int2.X >= 0) & (_int2.X <= this.Map.Terrain.TileSize.X)) & (_int2.Y >= 0)) & (_int2.Y <= this.Map.Terrain.TileSize.Y))
                            {
                                if (CliffAction == modProgram.enumFillCliffAction.StopAfter)
                                {
                                    _int4 = new modMath.sXY_int(vertexNum.X, vertexNum.Y);
                                    flag = this.Map.SideHIsCliffOnBothSides(_int4);
                                }
                                else
                                {
                                    flag = false;
                                }
                                if (!flag && (this.Map.Terrain.Vertices[_int2.X, _int2.Y].Terrain == terrain))
                                {
                                    if (_intArray.GetUpperBound(0) < index)
                                    {
                                        _intArray = (modMath.sXY_int[]) Utils.CopyArray((Array) _intArray, new modMath.sXY_int[((index * 2) + 1) + 1]);
                                    }
                                    _intArray[index] = _int2;
                                    index++;
                                }
                            }
                            _int2.X = vertexNum.X - 1;
                            _int2.Y = vertexNum.Y;
                            if ((((_int2.X >= 0) & (_int2.X <= this.Map.Terrain.TileSize.X)) & (_int2.Y >= 0)) & (_int2.Y <= this.Map.Terrain.TileSize.Y))
                            {
                                if (CliffAction == modProgram.enumFillCliffAction.StopAfter)
                                {
                                    _int4 = new modMath.sXY_int(vertexNum.X - 1, vertexNum.Y);
                                    flag = this.Map.SideHIsCliffOnBothSides(_int4);
                                }
                                else
                                {
                                    flag = false;
                                }
                                if (!flag && (this.Map.Terrain.Vertices[_int2.X, _int2.Y].Terrain == terrain))
                                {
                                    if (_intArray.GetUpperBound(0) < index)
                                    {
                                        _intArray = (modMath.sXY_int[]) Utils.CopyArray((Array) _intArray, new modMath.sXY_int[((index * 2) + 1) + 1]);
                                    }
                                    _intArray[index] = _int2;
                                    index++;
                                }
                            }
                            _int2.X = vertexNum.X;
                            _int2.Y = vertexNum.Y + 1;
                            if ((((_int2.X >= 0) & (_int2.X <= this.Map.Terrain.TileSize.X)) & (_int2.Y >= 0)) & (_int2.Y <= this.Map.Terrain.TileSize.Y))
                            {
                                if (CliffAction == modProgram.enumFillCliffAction.StopAfter)
                                {
                                    _int4 = new modMath.sXY_int(vertexNum.X, vertexNum.Y);
                                    flag = this.Map.SideVIsCliffOnBothSides(_int4);
                                }
                                else
                                {
                                    flag = false;
                                }
                                if (!flag && (this.Map.Terrain.Vertices[_int2.X, _int2.Y].Terrain == terrain))
                                {
                                    if (_intArray.GetUpperBound(0) < index)
                                    {
                                        _intArray = (modMath.sXY_int[]) Utils.CopyArray((Array) _intArray, new modMath.sXY_int[((index * 2) + 1) + 1]);
                                    }
                                    _intArray[index] = _int2;
                                    index++;
                                }
                            }
                            _int2.X = vertexNum.X;
                            _int2.Y = vertexNum.Y - 1;
                            if ((((_int2.X >= 0) & (_int2.X <= this.Map.Terrain.TileSize.X)) & (_int2.Y >= 0)) & (_int2.Y <= this.Map.Terrain.TileSize.Y))
                            {
                                if (CliffAction == modProgram.enumFillCliffAction.StopAfter)
                                {
                                    _int4 = new modMath.sXY_int(vertexNum.X, vertexNum.Y - 1);
                                    flag = this.Map.SideVIsCliffOnBothSides(_int4);
                                }
                                else
                                {
                                    flag = false;
                                }
                                if (!flag && (this.Map.Terrain.Vertices[_int2.X, _int2.Y].Terrain == terrain))
                                {
                                    if (_intArray.GetUpperBound(0) < index)
                                    {
                                        _intArray = (modMath.sXY_int[]) Utils.CopyArray((Array) _intArray, new modMath.sXY_int[((index * 2) + 1) + 1]);
                                    }
                                    _intArray[index] = _int2;
                                    index++;
                                }
                            }
                        }
                        num6++;
                        if (num6 >= 0x20000)
                        {
                            int num4 = index - num6;
                            int num2 = Math.Min(num6, num4);
                            int num3 = index - num2;
                            int num7 = num2 - 1;
                            for (int i = 0; i <= num7; i++)
                            {
                                _intArray[i] = _intArray[num3 + i];
                            }
                            index -= num6;
                            num6 = 0;
                            if ((index * 3) < (_intArray.GetUpperBound(0) + 1))
                            {
                                _intArray = (modMath.sXY_int[]) Utils.CopyArray((Array) _intArray, new modMath.sXY_int[((index * 2) + 1) + 1]);
                            }
                        }
                    }
                    this.Map.Update();
                    this.Map.UndoStepCreate("Ground Fill");
                    this.MapView.DrawViewLater();
                }
            }
        }

        public void Apply_Texture()
        {
            clsMouseOver.clsOverTerrain mouseOverTerrain = this.GetMouseOverTerrain();
            if (mouseOverTerrain != null)
            {
                clsMap.clsApplyTexture tool = new clsMap.clsApplyTexture {
                    Map = this.Map,
                    TextureNum = modProgram.SelectedTextureNum,
                    SetTexture = modMain.frmMainInstance.chkSetTexture.Checked,
                    Orientation = modProgram.TextureOrientation,
                    RandomOrientation = modMain.frmMainInstance.chkTextureOrientationRandomize.Checked,
                    SetOrientation = modMain.frmMainInstance.chkSetTextureOrientation.Checked,
                    TerrainAction = modMain.frmMainInstance.TextureTerrainAction
                };
                modProgram.TextureBrush.PerformActionMapTiles(tool, mouseOverTerrain.Tile);
                this.Map.Update();
                this.MapView.DrawViewLater();
            }
        }

        public void Apply_Texture_Clockwise()
        {
            clsMouseOver.clsOverTerrain mouseOverTerrain = this.GetMouseOverTerrain();
            if (mouseOverTerrain != null)
            {
                modMath.sXY_int normal = mouseOverTerrain.Tile.Normal;
                this.Map.Terrain.Tiles[normal.X, normal.Y].Texture.Orientation.RotateClockwise();
                this.Map.TileTextureChangeTerrainAction(normal, modMain.frmMainInstance.TextureTerrainAction);
                this.Map.SectorGraphicsChanges.TileChanged(normal);
                this.Map.SectorTerrainUndoChanges.TileChanged(normal);
                this.Map.Update();
                this.Map.UndoStepCreate("Texture Rotate");
                this.MapView.DrawViewLater();
            }
        }

        public void Apply_Texture_CounterClockwise()
        {
            clsMouseOver.clsOverTerrain mouseOverTerrain = this.GetMouseOverTerrain();
            if (mouseOverTerrain != null)
            {
                modMath.sXY_int normal = mouseOverTerrain.Tile.Normal;
                this.Map.Terrain.Tiles[normal.X, normal.Y].Texture.Orientation.RotateAnticlockwise();
                this.Map.TileTextureChangeTerrainAction(normal, modMain.frmMainInstance.TextureTerrainAction);
                this.Map.SectorGraphicsChanges.TileChanged(normal);
                this.Map.SectorTerrainUndoChanges.TileChanged(normal);
                this.Map.Update();
                this.Map.UndoStepCreate("Texture Rotate");
                this.MapView.DrawViewLater();
            }
        }

        public void Apply_Texture_FlipX()
        {
            clsMouseOver.clsOverTerrain mouseOverTerrain = this.GetMouseOverTerrain();
            if (mouseOverTerrain != null)
            {
                modMath.sXY_int normal = mouseOverTerrain.Tile.Normal;
                this.Map.Terrain.Tiles[normal.X, normal.Y].Texture.Orientation.ResultXFlip = !this.Map.Terrain.Tiles[normal.X, normal.Y].Texture.Orientation.ResultXFlip;
                this.Map.TileTextureChangeTerrainAction(normal, modMain.frmMainInstance.TextureTerrainAction);
                this.Map.SectorGraphicsChanges.TileChanged(normal);
                this.Map.SectorTerrainUndoChanges.TileChanged(normal);
                this.Map.Update();
                this.Map.UndoStepCreate("Texture Rotate");
                this.MapView.DrawViewLater();
            }
        }

        public void Apply_Tri_Flip()
        {
            clsMouseOver.clsOverTerrain mouseOverTerrain = this.GetMouseOverTerrain();
            if (mouseOverTerrain != null)
            {
                modMath.sXY_int normal = mouseOverTerrain.Tile.Normal;
                this.Map.Terrain.Tiles[normal.X, normal.Y].Tri = !this.Map.Terrain.Tiles[normal.X, normal.Y].Tri;
                this.Map.SectorGraphicsChanges.TileChanged(normal);
                this.Map.SectorTerrainUndoChanges.TileChanged(normal);
                this.Map.Update();
                this.Map.UndoStepCreate("Triangle Flip");
                this.MapView.DrawViewLater();
            }
        }

        public void ApplyObjectLine()
        {
            if ((modMain.frmMainInstance.SingleSelectedObjectType != null) & (this.Map.SelectedUnitGroup != null))
            {
                clsMouseOver.clsOverTerrain mouseOverTerrain = this.GetMouseOverTerrain();
                if (mouseOverTerrain != null)
                {
                    modMath.sXY_int normal = mouseOverTerrain.Tile.Normal;
                    if (this.Map.Selected_Tile_A != null)
                    {
                        int y;
                        int x;
                        int num3;
                        if (normal.X == this.Map.Selected_Tile_A.X)
                        {
                            if (normal.Y <= this.Map.Selected_Tile_A.Y)
                            {
                                y = normal.Y;
                                x = this.Map.Selected_Tile_A.Y;
                            }
                            else
                            {
                                y = this.Map.Selected_Tile_A.Y;
                                x = normal.Y;
                            }
                            clsMap.clsUnitCreate objectCreator = new clsMap.clsUnitCreate();
                            this.Map.SetObjectCreatorDefaults(objectCreator);
                            int num4 = x;
                            for (num3 = y; num3 <= num4; num3++)
                            {
                                objectCreator.Horizontal.X = (int) Math.Round((double) ((normal.X + 0.5) * 128.0));
                                objectCreator.Horizontal.Y = (int) Math.Round((double) ((num3 + 0.5) * 128.0));
                                objectCreator.Perform();
                            }
                            this.Map.UndoStepCreate("Object Line");
                            this.Map.Update();
                            this.Map.MinimapMakeLater();
                            this.Map.Selected_Tile_A = null;
                            this.MapView.DrawViewLater();
                        }
                        else if (normal.Y == this.Map.Selected_Tile_A.Y)
                        {
                            if (normal.X <= this.Map.Selected_Tile_A.X)
                            {
                                y = normal.X;
                                x = this.Map.Selected_Tile_A.X;
                            }
                            else
                            {
                                y = this.Map.Selected_Tile_A.X;
                                x = normal.X;
                            }
                            clsMap.clsUnitCreate create2 = new clsMap.clsUnitCreate();
                            this.Map.SetObjectCreatorDefaults(create2);
                            int num5 = x;
                            for (num3 = y; num3 <= num5; num3++)
                            {
                                create2.Horizontal.X = (int) Math.Round((double) ((num3 + 0.5) * 128.0));
                                create2.Horizontal.Y = (int) Math.Round((double) ((normal.Y + 0.5) * 128.0));
                                create2.Perform();
                            }
                            this.Map.UndoStepCreate("Object Line");
                            this.Map.Update();
                            this.Map.MinimapMakeLater();
                            this.Map.Selected_Tile_A = null;
                            this.MapView.DrawViewLater();
                        }
                    }
                    else
                    {
                        this.Map.Selected_Tile_A = new modMath.clsXY_int(normal);
                    }
                }
            }
        }

        public void FOV_Calc()
        {
            this.FieldOfViewY = (float) (Math.Atan((this.MapView.GLSize.Y * this.FOVMultiplier) / 2.0) * 2.0);
            if (this.FieldOfViewY < 0.001745329f)
            {
                this.FieldOfViewY = 0.001745329f;
                if (this.MapView.GLSize.Y > 0)
                {
                    this.FOVMultiplier = (2.0 * Math.Tan(((double) this.FieldOfViewY) / 2.0)) / ((double) this.MapView.GLSize.Y);
                    this.FOVMultiplierExponent = Math.Log(this.FOVMultiplier) / Math.Log(2.0);
                }
            }
            else if (this.FieldOfViewY > 3.124139f)
            {
                this.FieldOfViewY = 3.124139f;
                if (this.MapView.GLSize.Y > 0)
                {
                    this.FOVMultiplier = (2.0 * Math.Tan(((double) this.FieldOfViewY) / 2.0)) / ((double) this.MapView.GLSize.Y);
                    this.FOVMultiplierExponent = Math.Log(this.FOVMultiplier) / Math.Log(2.0);
                }
            }
            this.MapView.DrawViewLater();
        }

        public void FOV_Multiplier_Set(double Value)
        {
            this.FOVMultiplier = Value;
            this.FOVMultiplierExponent = Math.Log(this.FOVMultiplier) / Math.Log(2.0);
            this.FOV_Calc();
        }

        public void FOV_Scale_2E_Change(double PowerChange)
        {
            this.FOVMultiplierExponent += PowerChange;
            this.FOVMultiplier = Math.Pow(2.0, this.FOVMultiplierExponent);
            this.FOV_Calc();
        }

        public void FOV_Scale_2E_Set(double Power)
        {
            this.FOVMultiplierExponent = Power;
            this.FOVMultiplier = Math.Pow(2.0, this.FOVMultiplierExponent);
            this.FOV_Calc();
        }

        public void FOV_Set(double Radians, ctrlMapView MapView)
        {
            this.FOVMultiplier = (Math.Tan(Radians / 2.0) / ((double) MapView.GLSize.Y)) * 2.0;
            this.FOVMultiplierExponent = Math.Log(this.FOVMultiplier) / Math.Log(2.0);
            this.FOV_Calc();
        }

        public clsMouseDown.clsOverMinimap GetMouseLeftDownOverMinimap()
        {
            if (this.MouseLeftDown == null)
            {
                return null;
            }
            return this.MouseLeftDown.OverMinimap;
        }

        public clsMouseDown.clsOverTerrain GetMouseLeftDownOverTerrain()
        {
            if (this.MouseLeftDown == null)
            {
                return null;
            }
            return this.MouseLeftDown.OverTerrain;
        }

        public clsMouseOver.clsOverTerrain GetMouseOverTerrain()
        {
            if (this.MouseOver == null)
            {
                return null;
            }
            return this.MouseOver.OverTerrain;
        }

        public clsMouseDown.clsOverMinimap GetMouseRightDownOverMinimap()
        {
            if (this.MouseRightDown == null)
            {
                return null;
            }
            return this.MouseRightDown.OverMinimap;
        }

        public clsMouseDown.clsOverTerrain GetMouseRightDownOverTerrain()
        {
            if (this.MouseRightDown == null)
            {
                return null;
            }
            return this.MouseRightDown.OverTerrain;
        }

        public bool IsViewPosOverMinimap(modMath.sXY_int Pos)
        {
            return ((((Pos.X >= 0) & (Pos.X < (((double) this.Map.Terrain.TileSize.X) / this.Tiles_Per_Minimap_Pixel))) & (Pos.Y >= 0)) & (Pos.Y < (((double) this.Map.Terrain.TileSize.Y) / this.Tiles_Per_Minimap_Pixel)));
        }

        public void LookAtPos(modMath.sXY_int Horizontal)
        {
            Position.XYZ_dbl _dbl;
            modMath.sXYZ_int _int;
            Matrix3DMath.Matrix3D matrix = new Matrix3DMath.Matrix3D();
            Matrix3DMath.VectorForwardsRotationByMatrix(this.ViewAngleMatrix, ref _dbl);
            double terrainHeight = this.Map.GetTerrainHeight(Horizontal);
            int num = ((int) Math.Round(Math.Ceiling(terrainHeight))) + 0x80;
            if (this.ViewPos.Y < num)
            {
                this.ViewPos.Y = num;
            }
            if (_dbl.Y > -0.33333333333333331)
            {
                Angles.AnglePY epy;
                _dbl.Y = -0.33333333333333331;
                Matrix3DMath.VectorToPY(_dbl, ref epy);
                Matrix3DMath.MatrixSetToPY(matrix, epy);
                this.ViewAngleSet(matrix);
            }
            terrainHeight = (this.ViewPos.Y - terrainHeight) / _dbl.Y;
            _int.X = (int) Math.Round((double) (Horizontal.X + (terrainHeight * _dbl.X)));
            _int.Y = this.ViewPos.Y;
            _int.Z = (int) Math.Round((double) ((0 - Horizontal.Y) + (terrainHeight * _dbl.Z)));
            this.ViewPosSet(_int);
        }

        public void LookAtTile(modMath.sXY_int TileNum)
        {
            modMath.sXY_int _int;
            _int.X = (int) Math.Round((double) ((TileNum.X + 0.5) * 128.0));
            _int.Y = (int) Math.Round((double) ((TileNum.Y + 0.5) * 128.0));
            this.LookAtPos(_int);
        }

        public void MouseDown(MouseEventArgs e)
        {
            modMath.sXY_int _int;
            this.Map.SuppressMinimap = true;
            _int.X = e.X;
            _int.Y = e.Y;
            if (e.Button == MouseButtons.Left)
            {
                this.MouseLeftDown = new clsMouseDown();
                if (this.IsViewPosOverMinimap(_int))
                {
                    this.MouseLeftDown.OverMinimap = new clsMouseDown.clsOverMinimap();
                    this.MouseLeftDown.OverMinimap.DownPos = _int;
                    modMath.sXY_int tileNum = new modMath.sXY_int((int) Math.Round(((double) (_int.X * this.Tiles_Per_Minimap_Pixel))), (int) Math.Round(((double) (_int.Y * this.Tiles_Per_Minimap_Pixel))));
                    this.Map.TileNumClampToMap(tileNum);
                    this.LookAtTile(tileNum);
                }
                else
                {
                    clsMouseOver.clsOverTerrain mouseOverTerrain = this.GetMouseOverTerrain();
                    if (mouseOverTerrain != null)
                    {
                        this.MouseLeftDown.OverTerrain = new clsMouseDown.clsOverTerrain();
                        this.MouseLeftDown.OverTerrain.DownPos = mouseOverTerrain.Pos;
                        if (modTools.Tool == modTools.Tools.ObjectSelect)
                        {
                            if (modControls.KeyboardProfile.Active(modControls.Control_Picker))
                            {
                                if (mouseOverTerrain.Units.Count > 0)
                                {
                                    if (mouseOverTerrain.Units.Count == 1)
                                    {
                                        modMain.frmMainInstance.ObjectPicker(mouseOverTerrain.Units[0].Type);
                                    }
                                    else
                                    {
                                        this.MapView.ListSelectBegin(true);
                                    }
                                }
                            }
                            else if (modControls.KeyboardProfile.Active(modControls.Control_ScriptPosition))
                            {
                                clsMap.clsScriptPosition position = clsMap.clsScriptPosition.Create(this.Map);
                                if (position != null)
                                {
                                    position.PosX = this.MouseLeftDown.OverTerrain.DownPos.Horizontal.X;
                                    position.PosY = this.MouseLeftDown.OverTerrain.DownPos.Horizontal.Y;
                                    modMain.frmMainInstance.ScriptMarkerLists_Update();
                                }
                            }
                            else
                            {
                                if (!modControls.KeyboardProfile.Active(modControls.Control_Unit_Multiselect))
                                {
                                    this.Map.SelectedUnits.Clear();
                                }
                                modMain.frmMainInstance.SelectedObject_Changed();
                                this.Map.Unit_Selected_Area_VertexA = new modMath.clsXY_int(mouseOverTerrain.Vertex.Normal);
                                this.MapView.DrawViewLater();
                            }
                        }
                        else if (modTools.Tool == modTools.Tools.TerrainBrush)
                        {
                            if (this.Map.Tileset != null)
                            {
                                if (modControls.KeyboardProfile.Active(modControls.Control_Picker))
                                {
                                    modMain.frmMainInstance.TerrainPicker();
                                }
                                else
                                {
                                    this.Apply_Terrain();
                                    if (modMain.frmMainInstance.cbxAutoTexSetHeight.Checked)
                                    {
                                        this.Apply_Height_Set(modProgram.TerrainBrush, modMain.frmMainInstance.HeightSetPalette[modMain.frmMainInstance.tabHeightSetL.SelectedIndex]);
                                    }
                                }
                            }
                        }
                        else if (modTools.Tool == modTools.Tools.HeightSetBrush)
                        {
                            if (modControls.KeyboardProfile.Active(modControls.Control_Picker))
                            {
                                modMain.frmMainInstance.HeightPickerL();
                            }
                            else
                            {
                                this.Apply_Height_Set(modProgram.HeightBrush, modMain.frmMainInstance.HeightSetPalette[modMain.frmMainInstance.tabHeightSetL.SelectedIndex]);
                            }
                        }
                        else if (modTools.Tool == modTools.Tools.TextureBrush)
                        {
                            if (this.Map.Tileset != null)
                            {
                                if (modControls.KeyboardProfile.Active(modControls.Control_Picker))
                                {
                                    modMain.frmMainInstance.TexturePicker();
                                }
                                else
                                {
                                    this.Apply_Texture();
                                }
                            }
                        }
                        else if (modTools.Tool == modTools.Tools.CliffTriangle)
                        {
                            this.Apply_CliffTriangle(false);
                        }
                        else if (modTools.Tool == modTools.Tools.CliffBrush)
                        {
                            this.Apply_Cliff();
                        }
                        else if (modTools.Tool == modTools.Tools.CliffRemove)
                        {
                            this.Apply_Cliff_Remove();
                        }
                        else if (modTools.Tool == modTools.Tools.TerrainFill)
                        {
                            if (this.Map.Tileset != null)
                            {
                                if (modControls.KeyboardProfile.Active(modControls.Control_Picker))
                                {
                                    modMain.frmMainInstance.TerrainPicker();
                                }
                                else
                                {
                                    this.Apply_Terrain_Fill(modMain.frmMainInstance.FillCliffAction, modMain.frmMainInstance.cbxFillInside.Checked);
                                    this.MapView.DrawViewLater();
                                }
                            }
                        }
                        else if (modTools.Tool == modTools.Tools.RoadPlace)
                        {
                            if (this.Map.Tileset != null)
                            {
                                this.Apply_Road();
                            }
                        }
                        else if (modTools.Tool == modTools.Tools.RoadLines)
                        {
                            if (this.Map.Tileset != null)
                            {
                                this.Apply_Road_Line_Selection();
                            }
                        }
                        else if (modTools.Tool == modTools.Tools.RoadRemove)
                        {
                            this.Apply_Road_Remove();
                        }
                        else if (modTools.Tool == modTools.Tools.ObjectPlace)
                        {
                            if ((modMain.frmMainInstance.SingleSelectedObjectType != null) & (this.Map.SelectedUnitGroup != null))
                            {
                                clsMap.clsUnitCreate objectCreator = new clsMap.clsUnitCreate();
                                this.Map.SetObjectCreatorDefaults(objectCreator);
                                objectCreator.Horizontal = mouseOverTerrain.Pos.Horizontal;
                                objectCreator.Perform();
                                this.Map.UndoStepCreate("Place Object");
                                this.Map.Update();
                                this.Map.MinimapMakeLater();
                                this.MapView.DrawViewLater();
                            }
                        }
                        else if (modTools.Tool == modTools.Tools.ObjectLines)
                        {
                            this.ApplyObjectLine();
                        }
                        else if (modTools.Tool == modTools.Tools.TerrainSelect)
                        {
                            if (this.Map.Selected_Area_VertexA == null)
                            {
                                this.Map.Selected_Area_VertexA = new modMath.clsXY_int(mouseOverTerrain.Vertex.Normal);
                                this.MapView.DrawViewLater();
                            }
                            else if (this.Map.Selected_Area_VertexB == null)
                            {
                                this.Map.Selected_Area_VertexB = new modMath.clsXY_int(mouseOverTerrain.Vertex.Normal);
                                this.MapView.DrawViewLater();
                            }
                            else
                            {
                                this.Map.Selected_Area_VertexA = null;
                                this.Map.Selected_Area_VertexB = null;
                                this.MapView.DrawViewLater();
                            }
                        }
                        else if (modTools.Tool == modTools.Tools.Gateways)
                        {
                            this.Apply_Gateway();
                        }
                    }
                    else if (modTools.Tool == modTools.Tools.ObjectSelect)
                    {
                        this.Map.SelectedUnits.Clear();
                        modMain.frmMainInstance.SelectedObject_Changed();
                    }
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                this.MouseRightDown = new clsMouseDown();
                if (this.IsViewPosOverMinimap(_int))
                {
                    this.MouseRightDown.OverMinimap = new clsMouseDown.clsOverMinimap();
                    this.MouseRightDown.OverMinimap.DownPos = _int;
                }
                else
                {
                    clsMouseOver.clsOverTerrain terrain2 = this.GetMouseOverTerrain();
                    if (terrain2 != null)
                    {
                        this.MouseRightDown.OverTerrain = new clsMouseDown.clsOverTerrain();
                        this.MouseRightDown.OverTerrain.DownPos = terrain2.Pos;
                    }
                }
                if ((modTools.Tool == modTools.Tools.RoadLines) | (modTools.Tool == modTools.Tools.ObjectLines))
                {
                    this.Map.Selected_Tile_A = null;
                    this.MapView.DrawViewLater();
                }
                else if (modTools.Tool == modTools.Tools.TerrainSelect)
                {
                    this.Map.Selected_Area_VertexA = null;
                    this.Map.Selected_Area_VertexB = null;
                    this.MapView.DrawViewLater();
                }
                else if (modTools.Tool == modTools.Tools.CliffTriangle)
                {
                    this.Apply_CliffTriangle(true);
                }
                else if (modTools.Tool == modTools.Tools.Gateways)
                {
                    this.Map.Selected_Tile_A = null;
                    this.Map.Selected_Tile_B = null;
                    this.MapView.DrawViewLater();
                }
                else if (modTools.Tool == modTools.Tools.HeightSetBrush)
                {
                    if (modControls.KeyboardProfile.Active(modControls.Control_Picker))
                    {
                        modMain.frmMainInstance.HeightPickerR();
                    }
                    else
                    {
                        this.Apply_Height_Set(modProgram.HeightBrush, modMain.frmMainInstance.HeightSetPalette[modMain.frmMainInstance.tabHeightSetR.SelectedIndex]);
                    }
                }
            }
        }

        public void MouseOver_Pos_Calc()
        {
            if (this.GetMouseLeftDownOverMinimap() != null)
            {
                if ((this.MouseOver != null) && this.IsViewPosOverMinimap(this.MouseOver.ScreenPos))
                {
                    modMath.sXY_int tileNum = new modMath.sXY_int((int) Math.Round(((double) (this.MouseOver.ScreenPos.X * this.Tiles_Per_Minimap_Pixel))), (int) Math.Round(((double) (this.MouseOver.ScreenPos.Y * this.Tiles_Per_Minimap_Pixel))));
                    this.Map.TileNumClampToMap(tileNum);
                    this.LookAtTile(tileNum);
                }
            }
            else
            {
                Position.XY_dbl _dbl;
                clsMouseOver.clsOverTerrain terrain = new clsMouseOver.clsOverTerrain();
                bool flag = false;
                if (modSettings.Settings.DirectPointer)
                {
                    if (this.ScreenXY_Get_TerrainPos(this.MouseOver.ScreenPos, ref terrain.Pos) && this.Map.PosIsOnMap(terrain.Pos.Horizontal))
                    {
                        flag = true;
                    }
                }
                else
                {
                    terrain.Pos.Altitude = (int) Math.Round((double) (127.5 * this.Map.HeightMultiplier));
                    if (this.ScreenXY_Get_ViewPlanePos(this.MouseOver.ScreenPos, (double) terrain.Pos.Altitude, ref _dbl))
                    {
                        terrain.Pos.Horizontal.X = (int) Math.Round(_dbl.X);
                        terrain.Pos.Horizontal.Y = (int) Math.Round(-_dbl.Y);
                        if (this.Map.PosIsOnMap(terrain.Pos.Horizontal))
                        {
                            terrain.Pos.Altitude = (int) Math.Round(this.Map.GetTerrainHeight(terrain.Pos.Horizontal));
                            flag = true;
                        }
                    }
                }
                if (flag)
                {
                    IEnumerator enumerator;
                    this.MouseOver.OverTerrain = terrain;
                    terrain.Tile.Normal.X = (int) Math.Round(((double) (((double) terrain.Pos.Horizontal.X) / 128.0)));
                    terrain.Tile.Normal.Y = (int) Math.Round(((double) (((double) terrain.Pos.Horizontal.Y) / 128.0)));
                    terrain.Vertex.Normal.X = (int) Math.Round(Math.Round((double) (((double) terrain.Pos.Horizontal.X) / 128.0)));
                    terrain.Vertex.Normal.Y = (int) Math.Round(Math.Round((double) (((double) terrain.Pos.Horizontal.Y) / 128.0)));
                    terrain.Tile.Alignment = terrain.Vertex.Normal;
                    terrain.Vertex.Alignment = new modMath.sXY_int(terrain.Tile.Normal.X + 1, terrain.Tile.Normal.Y + 1);
                    terrain.Triangle = this.Map.GetTerrainTri(terrain.Pos.Horizontal);
                    _dbl.X = terrain.Pos.Horizontal.X - (terrain.Vertex.Normal.X * 0x80);
                    _dbl.Y = terrain.Pos.Horizontal.Y - (terrain.Vertex.Normal.Y * 0x80);
                    double introduced10 = Math.Abs(_dbl.Y);
                    if (introduced10 <= Math.Abs(_dbl.X))
                    {
                        terrain.Side_IsV = false;
                        terrain.Side_Num.X = terrain.Tile.Normal.X;
                        terrain.Side_Num.Y = terrain.Vertex.Normal.Y;
                    }
                    else
                    {
                        terrain.Side_IsV = true;
                        terrain.Side_Num.X = terrain.Vertex.Normal.X;
                        terrain.Side_Num.Y = terrain.Tile.Normal.Y;
                    }
                    modMath.sXY_int posSectorNum = this.Map.GetPosSectorNum(terrain.Pos.Horizontal);
                    try
                    {
                        enumerator = this.Map.Sectors[posSectorNum.X, posSectorNum.Y].Units.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            clsMap.clsUnitSectorConnection current = (clsMap.clsUnitSectorConnection) enumerator.Current;
                            clsMap.clsUnit newItem = current.Unit;
                            _dbl.X = newItem.Pos.Horizontal.X - terrain.Pos.Horizontal.X;
                            _dbl.Y = newItem.Pos.Horizontal.Y - terrain.Pos.Horizontal.Y;
                            modMath.sXY_int _int = newItem.Type.get_GetFootprintSelected(newItem.Rotation);
                            if ((Math.Abs(_dbl.X) <= (Math.Max((double) (((double) _int.X) / 2.0), (double) 0.5) * 128.0)) & (Math.Abs(_dbl.Y) <= (Math.Max((double) (((double) _int.Y) / 2.0), (double) 0.5) * 128.0)))
                            {
                                terrain.Units.Add(newItem);
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
                    if (this.MouseLeftDown != null)
                    {
                        if (modTools.Tool == modTools.Tools.TerrainBrush)
                        {
                            this.Apply_Terrain();
                            if (modMain.frmMainInstance.cbxAutoTexSetHeight.Checked)
                            {
                                this.Apply_Height_Set(modProgram.TerrainBrush, modMain.frmMainInstance.HeightSetPalette[modMain.frmMainInstance.tabHeightSetL.SelectedIndex]);
                            }
                        }
                        else if (modTools.Tool == modTools.Tools.HeightSetBrush)
                        {
                            this.Apply_Height_Set(modProgram.HeightBrush, modMain.frmMainInstance.HeightSetPalette[modMain.frmMainInstance.tabHeightSetL.SelectedIndex]);
                        }
                        else if (modTools.Tool == modTools.Tools.TextureBrush)
                        {
                            this.Apply_Texture();
                        }
                        else if (modTools.Tool == modTools.Tools.CliffTriangle)
                        {
                            this.Apply_CliffTriangle(false);
                        }
                        else if (modTools.Tool == modTools.Tools.CliffBrush)
                        {
                            this.Apply_Cliff();
                        }
                        else if (modTools.Tool == modTools.Tools.CliffRemove)
                        {
                            this.Apply_Cliff_Remove();
                        }
                        else if (modTools.Tool == modTools.Tools.RoadPlace)
                        {
                            this.Apply_Road();
                        }
                        else if (modTools.Tool == modTools.Tools.RoadRemove)
                        {
                            this.Apply_Road_Remove();
                        }
                    }
                    if (this.MouseRightDown != null)
                    {
                        if (modTools.Tool == modTools.Tools.HeightSetBrush)
                        {
                            if (this.MouseLeftDown == null)
                            {
                                this.Apply_Height_Set(modProgram.HeightBrush, modMain.frmMainInstance.HeightSetPalette[modMain.frmMainInstance.tabHeightSetR.SelectedIndex]);
                            }
                        }
                        else if (modTools.Tool == modTools.Tools.CliffTriangle)
                        {
                            this.Apply_CliffTriangle(true);
                        }
                    }
                }
            }
            this.MapView.Pos_Display_Update();
            this.MapView.DrawViewLater();
        }

        public void MoveToViewTerrainPosFromDistance(Position.XYZ_dbl TerrainPos, double Distance)
        {
            Position.XYZ_dbl _dbl;
            modMath.sXYZ_int _int;
            Matrix3DMath.VectorForwardsRotationByMatrix(this.ViewAngleMatrix, ref _dbl);
            _int.X = (int) Math.Round((double) (TerrainPos.X - (_dbl.X * Distance)));
            _int.Y = (int) Math.Round((double) (TerrainPos.Y - (_dbl.Y * Distance)));
            _int.Z = (int) Math.Round((double) (-TerrainPos.Z - (_dbl.Z * Distance)));
            this.ViewPosSet(_int);
        }

        public bool Pos_Get_Screen_XY(Position.XYZ_dbl Pos, ref modMath.sXY_int Result)
        {
            if (Pos.Z > 0.0)
            {
                try
                {
                    double num = 1.0 / (this.FOVMultiplier * Pos.Z);
                    Result.X = (int) Math.Round((double) ((((double) this.MapView.GLSize.X) / 2.0) + (Pos.X * num)));
                    Result.Y = (int) Math.Round((double) ((((double) this.MapView.GLSize.Y) / 2.0) - (Pos.Y * num)));
                    return true;
                }
                catch (Exception exception1)
                {
                    ProjectData.SetProjectError(exception1);
                    ProjectData.ClearProjectError();
                }
            }
            return false;
        }

        public bool ScreenXY_Get_TerrainPos(modMath.sXY_int ScreenPos, ref modProgram.sWorldPos ResultPos)
        {
            try
            {
                Position.XYZ_dbl _dbl;
                Position.XY_dbl _dbl3;
                Position.XY_dbl _dbl4;
                modMath.sXY_int _int;
                modMath.sXY_int _int2;
                Position.XYZ_dbl _dbl5;
                Position.XYZ_dbl _dbl6;
                Position.XYZ_dbl _dbl8;
                _dbl5.X = this.ViewPos.X;
                _dbl5.Y = this.ViewPos.Y;
                _dbl5.Z = 0 - this.ViewPos.Z;
                _dbl8.X = (ScreenPos.X - (((double) this.MapView.GLSize.X) / 2.0)) * this.FOVMultiplier;
                _dbl8.Y = ((((double) this.MapView.GLSize.Y) / 2.0) - ScreenPos.Y) * this.FOVMultiplier;
                _dbl8.Z = 1.0;
                Matrix3DMath.VectorRotationByMatrix(this.ViewAngleMatrix, _dbl8, ref _dbl6);
                _dbl6.Y = -_dbl6.Y;
                _dbl6.Z = -_dbl6.Z;
                double num2 = (_dbl5.Y - (0xff * this.Map.HeightMultiplier)) / _dbl6.Y;
                _dbl3.X = _dbl5.X + (_dbl6.X * num2);
                _dbl3.Y = _dbl5.Z + (_dbl6.Z * num2);
                num2 = _dbl5.Y / _dbl6.Y;
                _dbl4.X = _dbl5.X + (_dbl6.X * num2);
                _dbl4.Y = _dbl5.Z + (_dbl6.Z * num2);
                _int2.X = Math.Max((int) Math.Round(((double) (Math.Min(_dbl3.X, _dbl4.X) / 128.0))), 0);
                _int2.Y = Math.Max((int) Math.Round(((double) (Math.Min(_dbl3.Y, _dbl4.Y) / 128.0))), 0);
                _int.X = Math.Min((int) Math.Round(((double) (Math.Max(_dbl3.X, _dbl4.X) / 128.0))), this.Map.Terrain.TileSize.X - 1);
                _int.Y = Math.Min((int) Math.Round(((double) (Math.Max(_dbl3.Y, _dbl4.Y) / 128.0))), this.Map.Terrain.TileSize.Y - 1);
                double maxValue = double.MaxValue;
                _dbl.X = double.NaN;
                _dbl.Y = double.NaN;
                _dbl.Z = double.NaN;
                int y = _int.Y;
                for (int i = _int2.Y; i <= y; i++)
                {
                    int x = _int.X;
                    for (int j = _int2.X; j <= x; j++)
                    {
                        Position.XYZ_dbl _dbl2;
                        double magnitude;
                        double num4;
                        double num5;
                        Position.XY_dbl _dbl7;
                        double num6;
                        double num7;
                        double num8;
                        _dbl7.X = j * 0x80;
                        _dbl7.Y = i * 0x80;
                        if (this.Map.Terrain.Tiles[j, i].Tri)
                        {
                            num8 = this.Map.Terrain.Vertices[j, i].Height * this.Map.HeightMultiplier;
                            num6 = (this.Map.Terrain.Vertices[j + 1, i].Height * this.Map.HeightMultiplier) - num8;
                            num7 = (this.Map.Terrain.Vertices[j, i + 1].Height * this.Map.HeightMultiplier) - num8;
                            _dbl8.Y = (num8 + ((((num6 * (_dbl5.X - _dbl7.X)) + (num7 * (_dbl5.Z - _dbl7.Y))) + ((((num6 * _dbl6.X) + (num7 * _dbl6.Z)) * _dbl5.Y) / _dbl6.Y)) / 128.0)) / (1.0 + (((num6 * _dbl6.X) + (num7 * _dbl6.Z)) / (_dbl6.Y * 128.0)));
                            _dbl8.X = _dbl5.X + ((_dbl6.X * (_dbl5.Y - _dbl8.Y)) / _dbl6.Y);
                            _dbl8.Z = _dbl5.Z + ((_dbl6.Z * (_dbl5.Y - _dbl8.Y)) / _dbl6.Y);
                            num4 = (_dbl8.X / 128.0) - j;
                            num5 = (_dbl8.Z / 128.0) - i;
                            if (((((num5 <= (1.0 - num4)) & (num4 >= 0.0)) & (num5 >= 0.0)) & (num4 <= 1.0)) & (num5 <= 1.0))
                            {
                                _dbl2 = _dbl8 - _dbl5;
                                magnitude = _dbl2.GetMagnitude();
                                if (magnitude < maxValue)
                                {
                                    maxValue = magnitude;
                                    _dbl = _dbl8;
                                }
                            }
                            num8 = this.Map.Terrain.Vertices[j + 1, i + 1].Height * this.Map.HeightMultiplier;
                            num6 = (this.Map.Terrain.Vertices[j, i + 1].Height * this.Map.HeightMultiplier) - num8;
                            num7 = (this.Map.Terrain.Vertices[j + 1, i].Height * this.Map.HeightMultiplier) - num8;
                            _dbl8.Y = (((num8 + num6) + num7) + ((((num6 * (_dbl7.X - _dbl5.X)) + (num7 * (_dbl7.Y - _dbl5.Z))) - ((((num6 * _dbl6.X) + (num7 * _dbl6.Z)) * _dbl5.Y) / _dbl6.Y)) / 128.0)) / (1.0 - (((num6 * _dbl6.X) + (num7 * _dbl6.Z)) / (_dbl6.Y * 128.0)));
                            _dbl8.X = _dbl5.X + ((_dbl6.X * (_dbl5.Y - _dbl8.Y)) / _dbl6.Y);
                            _dbl8.Z = _dbl5.Z + ((_dbl6.Z * (_dbl5.Y - _dbl8.Y)) / _dbl6.Y);
                            num4 = (_dbl8.X / 128.0) - j;
                            num5 = (_dbl8.Z / 128.0) - i;
                            if (((((num5 >= (1.0 - num4)) & (num4 >= 0.0)) & (num5 >= 0.0)) & (num4 <= 1.0)) & (num5 <= 1.0))
                            {
                                _dbl2 = _dbl8 - _dbl5;
                                magnitude = _dbl2.GetMagnitude();
                                if (magnitude < maxValue)
                                {
                                    maxValue = magnitude;
                                    _dbl = _dbl8;
                                }
                            }
                        }
                        else
                        {
                            num8 = this.Map.Terrain.Vertices[j + 1, i].Height * this.Map.HeightMultiplier;
                            num6 = (this.Map.Terrain.Vertices[j, i].Height * this.Map.HeightMultiplier) - num8;
                            num7 = (this.Map.Terrain.Vertices[j + 1, i + 1].Height * this.Map.HeightMultiplier) - num8;
                            _dbl8.Y = ((num8 + num6) + ((((num6 * (_dbl7.X - _dbl5.X)) + (num7 * (_dbl5.Z - _dbl7.Y))) - ((((num6 * _dbl6.X) - (num7 * _dbl6.Z)) * _dbl5.Y) / _dbl6.Y)) / 128.0)) / (1.0 - (((num6 * _dbl6.X) - (num7 * _dbl6.Z)) / (_dbl6.Y * 128.0)));
                            _dbl8.X = _dbl5.X + ((_dbl6.X * (_dbl5.Y - _dbl8.Y)) / _dbl6.Y);
                            _dbl8.Z = _dbl5.Z + ((_dbl6.Z * (_dbl5.Y - _dbl8.Y)) / _dbl6.Y);
                            num4 = (_dbl8.X / 128.0) - j;
                            num5 = (_dbl8.Z / 128.0) - i;
                            if (((((num5 <= num4) & (num4 >= 0.0)) & (num5 >= 0.0)) & (num4 <= 1.0)) & (num5 <= 1.0))
                            {
                                _dbl2 = _dbl8 - _dbl5;
                                magnitude = _dbl2.GetMagnitude();
                                if (magnitude < maxValue)
                                {
                                    maxValue = magnitude;
                                    _dbl = _dbl8;
                                }
                            }
                            num8 = this.Map.Terrain.Vertices[j, i + 1].Height * this.Map.HeightMultiplier;
                            num6 = (this.Map.Terrain.Vertices[j + 1, i + 1].Height * this.Map.HeightMultiplier) - num8;
                            num7 = (this.Map.Terrain.Vertices[j, i].Height * this.Map.HeightMultiplier) - num8;
                            _dbl8.Y = ((num8 + num7) + ((((num6 * (_dbl5.X - _dbl7.X)) + (num7 * (_dbl7.Y - _dbl5.Z))) + ((((num6 * _dbl6.X) - (num7 * _dbl6.Z)) * _dbl5.Y) / _dbl6.Y)) / 128.0)) / (1.0 + (((num6 * _dbl6.X) - (num7 * _dbl6.Z)) / (_dbl6.Y * 128.0)));
                            _dbl8.X = _dbl5.X + ((_dbl6.X * (_dbl5.Y - _dbl8.Y)) / _dbl6.Y);
                            _dbl8.Z = _dbl5.Z + ((_dbl6.Z * (_dbl5.Y - _dbl8.Y)) / _dbl6.Y);
                            num4 = (_dbl8.X / 128.0) - j;
                            num5 = (_dbl8.Z / 128.0) - i;
                            if (((((num5 >= num4) & (num4 >= 0.0)) & (num5 >= 0.0)) & (num4 <= 1.0)) & (num5 <= 1.0))
                            {
                                magnitude = (_dbl8 - _dbl5).GetMagnitude();
                                if (magnitude < maxValue)
                                {
                                    maxValue = magnitude;
                                    _dbl = _dbl8;
                                }
                            }
                        }
                    }
                }
                if (_dbl.X == double.NaN)
                {
                    return false;
                }
                ResultPos.Horizontal.X = (int) Math.Round(_dbl.X);
                ResultPos.Altitude = (int) Math.Round(_dbl.Y);
                ResultPos.Horizontal.Y = (int) Math.Round(_dbl.Z);
            }
            catch (Exception exception1)
            {
                ProjectData.SetProjectError(exception1);
                ProjectData.ClearProjectError();
                return false;
            }
            return true;
        }

        public bool ScreenXY_Get_ViewPlanePos(modMath.sXY_int ScreenPos, double PlaneHeight, ref Position.XY_dbl ResultPos)
        {
            try
            {
                Position.XYZ_dbl _dbl;
                Position.XYZ_dbl _dbl2;
                _dbl.X = (ScreenPos.X - (((double) this.MapView.GLSize.X) / 2.0)) * this.FOVMultiplier;
                _dbl.Y = ((((double) this.MapView.GLSize.Y) / 2.0) - ScreenPos.Y) * this.FOVMultiplier;
                _dbl.Z = 1.0;
                Matrix3DMath.VectorRotationByMatrix(this.ViewAngleMatrix, _dbl, ref _dbl2);
                double num = (PlaneHeight - this.ViewPos.Y) / _dbl2.Y;
                ResultPos.X = this.ViewPos.X + (_dbl2.X * num);
                ResultPos.Y = this.ViewPos.Z + (_dbl2.Z * num);
            }
            catch (Exception exception1)
            {
                ProjectData.SetProjectError(exception1);
                ProjectData.ClearProjectError();
                return false;
            }
            return true;
        }

        public bool ScreenXY_Get_ViewPlanePos_ForwardDownOnly(int ScreenX, int ScreenY, double PlaneHeight, ref Position.XY_dbl ResultPos)
        {
            if (this.ViewPos.Y < PlaneHeight)
            {
                return false;
            }
            try
            {
                Position.XYZ_dbl _dbl;
                Position.XYZ_dbl _dbl2;
                double fOVMultiplier = this.FOVMultiplier;
                _dbl.X = (ScreenX - (((double) this.MapView.GLSize.X) / 2.0)) * fOVMultiplier;
                _dbl.Y = ((((double) this.MapView.GLSize.Y) / 2.0) - ScreenY) * fOVMultiplier;
                _dbl.Z = 1.0;
                Matrix3DMath.VectorRotationByMatrix(this.ViewAngleMatrix, _dbl, ref _dbl2);
                if (_dbl2.Y > 0.0)
                {
                    return false;
                }
                double num = (PlaneHeight - this.ViewPos.Y) / _dbl2.Y;
                ResultPos.X = this.ViewPos.X + (_dbl2.X * num);
                ResultPos.Y = this.ViewPos.Z + (_dbl2.Z * num);
            }
            catch (Exception exception1)
            {
                ProjectData.SetProjectError(exception1);
                ProjectData.ClearProjectError();
                return false;
            }
            return true;
        }

        public void TimedActions(double Zoom, double Move, double Pan, double Roll, double OrbitRate)
        {
            Angles.AnglePY epy;
            modMath.sXYZ_int _int;
            Position.XYZ_dbl _dbl2;
            double scale = Pan * this.FieldOfViewY;
            Matrix3DMath.Matrix3D matrix = new Matrix3DMath.Matrix3D();
            Matrix3DMath.Matrix3D resultMatrix = new Matrix3DMath.Matrix3D();
            Move *= (this.FOVMultiplier * (this.MapView.GLSize.X + this.MapView.GLSize.Y)) * Math.Max((double) Math.Abs(this.ViewPos.Y), 512.0);
            if (modControls.KeyboardProfile.Active(modControls.Control_View_Zoom_In))
            {
                this.FOV_Scale_2E_Change(-Zoom);
            }
            if (modControls.KeyboardProfile.Active(modControls.Control_View_Zoom_Out))
            {
                this.FOV_Scale_2E_Change(Zoom);
            }
            if (modProgram.ViewMoveType == modProgram.enumView_Move_Type.Free)
            {
                Position.XYZ_dbl _dbl;
                _int.X = 0;
                _int.Y = 0;
                _int.Z = 0;
                if (modControls.KeyboardProfile.Active(modControls.Control_View_Move_Forward))
                {
                    Matrix3DMath.VectorForwardsRotationByMatrix(this.ViewAngleMatrix, Move, ref _dbl2);
                    _int.Add_dbl(_dbl2);
                }
                if (modControls.KeyboardProfile.Active(modControls.Control_View_Move_Backward))
                {
                    Matrix3DMath.VectorBackwardsRotationByMatrix(this.ViewAngleMatrix, Move, ref _dbl2);
                    _int.Add_dbl(_dbl2);
                }
                if (modControls.KeyboardProfile.Active(modControls.Control_View_Move_Left))
                {
                    Matrix3DMath.VectorLeftRotationByMatrix(this.ViewAngleMatrix, Move, ref _dbl2);
                    _int.Add_dbl(_dbl2);
                }
                if (modControls.KeyboardProfile.Active(modControls.Control_View_Move_Right))
                {
                    Matrix3DMath.VectorRightRotationByMatrix(this.ViewAngleMatrix, Move, ref _dbl2);
                    _int.Add_dbl(_dbl2);
                }
                if (modControls.KeyboardProfile.Active(modControls.Control_View_Move_Up))
                {
                    Matrix3DMath.VectorUpRotationByMatrix(this.ViewAngleMatrix, Move, ref _dbl2);
                    _int.Add_dbl(_dbl2);
                }
                if (modControls.KeyboardProfile.Active(modControls.Control_View_Move_Down))
                {
                    Matrix3DMath.VectorDownRotationByMatrix(this.ViewAngleMatrix, Move, ref _dbl2);
                    _int.Add_dbl(_dbl2);
                }
                _dbl.X = 0.0;
                _dbl.Y = 0.0;
                _dbl.Z = 0.0;
                if (modControls.KeyboardProfile.Active(modControls.Control_View_Left))
                {
                    Matrix3DMath.VectorForwardsRotationByMatrix(this.ViewAngleMatrix, Roll, ref _dbl2);
                    _dbl += _dbl2;
                }
                if (modControls.KeyboardProfile.Active(modControls.Control_View_Right))
                {
                    Matrix3DMath.VectorBackwardsRotationByMatrix(this.ViewAngleMatrix, Roll, ref _dbl2);
                    _dbl += _dbl2;
                }
                if (modControls.KeyboardProfile.Active(modControls.Control_View_Backward))
                {
                    Matrix3DMath.VectorLeftRotationByMatrix(this.ViewAngleMatrix, scale, ref _dbl2);
                    _dbl += _dbl2;
                }
                if (modControls.KeyboardProfile.Active(modControls.Control_View_Forward))
                {
                    Matrix3DMath.VectorRightRotationByMatrix(this.ViewAngleMatrix, scale, ref _dbl2);
                    _dbl += _dbl2;
                }
                if (modControls.KeyboardProfile.Active(modControls.Control_View_Roll_Left))
                {
                    Matrix3DMath.VectorDownRotationByMatrix(this.ViewAngleMatrix, scale, ref _dbl2);
                    _dbl += _dbl2;
                }
                if (modControls.KeyboardProfile.Active(modControls.Control_View_Roll_Right))
                {
                    Matrix3DMath.VectorUpRotationByMatrix(this.ViewAngleMatrix, scale, ref _dbl2);
                    _dbl += _dbl2;
                }
                if (((_int.X != 0.0) | (_int.Y != 0.0)) | (_int.Z != 0.0))
                {
                    this.ViewPosChange(_int);
                }
                if ((!(_dbl.X == 0.0) | !(_dbl.Y == 0.0)) | !(_dbl.Z == 0.0))
                {
                    Matrix3DMath.VectorToPY(_dbl, ref epy);
                    Matrix3DMath.MatrixSetToPY(matrix, epy);
                    Matrix3DMath.MatrixRotationAroundAxis(this.ViewAngleMatrix, matrix, _dbl.GetMagnitude(), resultMatrix);
                    this.ViewAngleSet_Rotate(resultMatrix);
                }
            }
            else if (modProgram.ViewMoveType == modProgram.enumView_Move_Type.RTS)
            {
                _int = new modMath.sXYZ_int();
                Matrix3DMath.MatrixToPY(this.ViewAngleMatrix, ref epy);
                Matrix3DMath.MatrixSetToYAngle(matrix, epy.Yaw);
                if (modControls.KeyboardProfile.Active(modControls.Control_View_Move_Forward))
                {
                    Matrix3DMath.VectorForwardsRotationByMatrix(matrix, Move, ref _dbl2);
                    _int.Add_dbl(_dbl2);
                }
                if (modControls.KeyboardProfile.Active(modControls.Control_View_Move_Backward))
                {
                    Matrix3DMath.VectorBackwardsRotationByMatrix(matrix, Move, ref _dbl2);
                    _int.Add_dbl(_dbl2);
                }
                if (modControls.KeyboardProfile.Active(modControls.Control_View_Move_Left))
                {
                    Matrix3DMath.VectorLeftRotationByMatrix(matrix, Move, ref _dbl2);
                    _int.Add_dbl(_dbl2);
                }
                if (modControls.KeyboardProfile.Active(modControls.Control_View_Move_Right))
                {
                    Matrix3DMath.VectorRightRotationByMatrix(matrix, Move, ref _dbl2);
                    _int.Add_dbl(_dbl2);
                }
                if (modControls.KeyboardProfile.Active(modControls.Control_View_Move_Up))
                {
                    _int.Y += (int) Math.Round(Move);
                }
                if (modControls.KeyboardProfile.Active(modControls.Control_View_Move_Down))
                {
                    _int.Y -= (int) Math.Round(Move);
                }
                bool flag = false;
                if (modProgram.RTSOrbit)
                {
                    if (modControls.KeyboardProfile.Active(modControls.Control_View_Forward))
                    {
                        epy.Pitch = modMath.Clamp_dbl(epy.Pitch + OrbitRate, -1.5702509114036483, 1.5702509114036483);
                        flag = true;
                    }
                    if (modControls.KeyboardProfile.Active(modControls.Control_View_Backward))
                    {
                        epy.Pitch = modMath.Clamp_dbl(epy.Pitch - OrbitRate, -1.5702509114036483, 1.5702509114036483);
                        flag = true;
                    }
                    if (modControls.KeyboardProfile.Active(modControls.Control_View_Left))
                    {
                        epy.Yaw = modMath.AngleClamp(epy.Yaw + OrbitRate);
                        flag = true;
                    }
                    if (modControls.KeyboardProfile.Active(modControls.Control_View_Right))
                    {
                        epy.Yaw = modMath.AngleClamp(epy.Yaw - OrbitRate);
                        flag = true;
                    }
                }
                else
                {
                    if (modControls.KeyboardProfile.Active(modControls.Control_View_Forward))
                    {
                        epy.Pitch = modMath.Clamp_dbl(epy.Pitch - OrbitRate, -1.5702509114036483, 1.5702509114036483);
                        flag = true;
                    }
                    if (modControls.KeyboardProfile.Active(modControls.Control_View_Backward))
                    {
                        epy.Pitch = modMath.Clamp_dbl(epy.Pitch + OrbitRate, -1.5702509114036483, 1.5702509114036483);
                        flag = true;
                    }
                    if (modControls.KeyboardProfile.Active(modControls.Control_View_Left))
                    {
                        epy.Yaw = modMath.AngleClamp(epy.Yaw - OrbitRate);
                        flag = true;
                    }
                    if (modControls.KeyboardProfile.Active(modControls.Control_View_Right))
                    {
                        epy.Yaw = modMath.AngleClamp(epy.Yaw + OrbitRate);
                        flag = true;
                    }
                }
                if (((_int.X != 0.0) | (_int.Y != 0.0)) | (_int.Z != 0.0))
                {
                    this.ViewPosChange(_int);
                }
                if (flag)
                {
                    Matrix3DMath.MatrixSetToPY(matrix, epy);
                    this.ViewAngleSet_Rotate(matrix);
                }
            }
        }

        public void TimedTools()
        {
            if (modTools.Tool == modTools.Tools.HeightSmoothBrush)
            {
                double num;
                if (((this.GetMouseOverTerrain() != null) && (this.GetMouseLeftDownOverTerrain() != null)) && modIO.InvariantParse_dbl(modMain.frmMainInstance.txtSmoothRate.Text, ref num))
                {
                    this.Apply_HeightSmoothing(modMath.Clamp_dbl((num * modMain.frmMainInstance.tmrTool.Interval) / 1000.0, 0.0, 1.0));
                }
            }
            else
            {
                double num2;
                if (((modTools.Tool == modTools.Tools.HeightChangeBrush) && (this.GetMouseOverTerrain() != null)) && modIO.InvariantParse_dbl(modMain.frmMainInstance.txtHeightChangeRate.Text, ref num2))
                {
                    if (this.GetMouseLeftDownOverTerrain() != null)
                    {
                        this.Apply_Height_Change(modMath.Clamp_dbl(num2, -255.0, 255.0));
                    }
                    else if (this.GetMouseRightDownOverTerrain() != null)
                    {
                        this.Apply_Height_Change(modMath.Clamp_dbl(-num2, -255.0, 255.0));
                    }
                }
            }
        }

        public void ViewAngleSet(Matrix3DMath.Matrix3D NewMatrix)
        {
            Matrix3DMath.MatrixCopy(NewMatrix, this.ViewAngleMatrix);
            Matrix3DMath.MatrixNormalize(this.ViewAngleMatrix);
            Matrix3DMath.MatrixInvert(this.ViewAngleMatrix, this.ViewAngleMatrix_Inverted);
            Matrix3DMath.MatrixToRPY(this.ViewAngleMatrix, ref this.ViewAngleRPY);
            this.MapView.DrawViewLater();
        }

        public void ViewAngleSet_Rotate(Matrix3DMath.Matrix3D NewMatrix)
        {
            bool flag;
            Position.XYZ_dbl _dbl2;
            if ((modProgram.ViewMoveType == modProgram.enumView_Move_Type.RTS) & modProgram.RTSOrbit)
            {
                Position.XY_dbl _dbl;
                flag = true;
                if (this.ScreenXY_Get_ViewPlanePos_ForwardDownOnly((int) Math.Round(((double) (((double) this.MapView.GLSize.X) / 2.0))), (int) Math.Round(((double) (((double) this.MapView.GLSize.Y) / 2.0))), 127.5, ref _dbl))
                {
                    _dbl2.X = _dbl.X;
                    _dbl2.Y = 127.5;
                    _dbl2.Z = -_dbl.Y;
                }
                else
                {
                    flag = false;
                }
            }
            else
            {
                flag = false;
            }
            Matrix3DMath.MatrixToRPY(NewMatrix, ref this.ViewAngleRPY);
            if (flag && (this.ViewAngleRPY.Pitch < 0.17453292519943295))
            {
                this.ViewAngleRPY.Pitch = 0.17453292519943295;
            }
            Matrix3DMath.MatrixSetToRPY(this.ViewAngleMatrix, this.ViewAngleRPY);
            Matrix3DMath.MatrixInvert(this.ViewAngleMatrix, this.ViewAngleMatrix_Inverted);
            if (flag)
            {
                Position.XYZ_dbl _dbl3;
                _dbl3.X = this.ViewPos.X;
                _dbl3.Y = this.ViewPos.Y;
                _dbl3.Z = 0 - this.ViewPos.Z;
                this.MoveToViewTerrainPosFromDistance(_dbl2, (_dbl3 - _dbl2).GetMagnitude());
            }
            this.MapView.DrawViewLater();
        }

        public void ViewAngleSetToDefault()
        {
            Matrix3DMath.Matrix3D matrix = new Matrix3DMath.Matrix3D();
            Matrix3DMath.MatrixSetToXAngle(matrix, Math.Atan(2.0));
            this.ViewAngleSet(matrix);
            this.MapView.DrawViewLater();
        }

        public void ViewPosChange(modMath.sXYZ_int Displacement)
        {
            this.ViewPos.X += Displacement.X;
            this.ViewPos.Z += Displacement.Z;
            this.ViewPos.Y += Displacement.Y;
            this.ViewPosClamp();
            this.MapView.DrawViewLater();
        }

        private void ViewPosClamp()
        {
            this.ViewPos.X = modMath.Clamp_int(this.ViewPos.X, -1048576, (this.Map.Terrain.TileSize.X * 0x80) + 0x100000);
            this.ViewPos.Z = modMath.Clamp_int(this.ViewPos.Z, ((0 - this.Map.Terrain.TileSize.Y) * 0x80) - 0x100000, 0x100000);
            modMath.sXY_int horizontal = new modMath.sXY_int(this.ViewPos.X, 0 - this.ViewPos.Z);
            this.ViewPos.Y = modMath.Clamp_int(this.ViewPos.Y, ((int) Math.Round(Math.Ceiling(this.Map.GetTerrainHeight(horizontal)))) + 0x10, 0x100000);
        }

        public void ViewPosSet(modMath.sXYZ_int NewViewPos)
        {
            this.ViewPos = NewViewPos;
            this.ViewPosClamp();
            this.MapView.DrawViewLater();
        }

        public class clsMouseDown
        {
            public clsOverMinimap OverMinimap;
            public clsOverTerrain OverTerrain;

            public class clsOverMinimap
            {
                public modMath.sXY_int DownPos;
            }

            public class clsOverTerrain
            {
                public modProgram.sWorldPos DownPos;
            }
        }

        public class clsMouseOver
        {
            public clsOverTerrain OverTerrain;
            public modMath.sXY_int ScreenPos;

            public class clsOverTerrain
            {
                public modProgram.sWorldPos Pos;
                public bool Side_IsV;
                public modMath.sXY_int Side_Num;
                public clsBrush.sPosNum Tile;
                public bool Triangle;
                public modLists.SimpleClassList<clsMap.clsUnit> Units = new modLists.SimpleClassList<clsMap.clsUnit>();
                public clsBrush.sPosNum Vertex;
            }
        }
    }
}

