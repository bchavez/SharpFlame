
Public Class clsViewInfo

    Public Map As clsMap
    Public MapView As ctrlMapView

    Public ViewPos As sXYZ_int
    Public ViewAngleMatrix As New Matrix3D.Matrix3D
    Public ViewAngleMatrix_Inverted As New Matrix3D.Matrix3D
    Public ViewAngleRPY As Matrix3D.AngleRPY
    Public FOVMultiplier As Double
    Public FOVMultiplierExponent As Double
    Public FieldOfViewY As Single

    Public Sub New(Map As clsMap, MapView As ctrlMapView)

        Me.Map = Map
        Me.MapView = MapView

        ViewPos = New sXYZ_int(0, 3072, 0)
        FOV_Multiplier_Set(Settings.FOVDefault)
        ViewAngleSetToDefault()
        LookAtPos(New sXY_int(CInt(Map.Terrain.TileSize.X * TerrainGridSpacing / 2.0#), CInt(Map.Terrain.TileSize.Y * TerrainGridSpacing / 2.0#)))
    End Sub

    Public Sub FOV_Scale_2E_Set(Power As Double)

        FOVMultiplierExponent = Power
        FOVMultiplier = 2.0# ^ FOVMultiplierExponent

        FOV_Calc()
    End Sub

    Public Sub FOV_Scale_2E_Change(PowerChange As Double)

        FOVMultiplierExponent += PowerChange
        FOVMultiplier = 2.0# ^ FOVMultiplierExponent

        FOV_Calc()
    End Sub

    Public Sub FOV_Set(Radians As Double, MapView As ctrlMapView)

        FOVMultiplier = Math.Tan(Radians / 2.0#) / MapView.GLSize.Y * 2.0#
        FOVMultiplierExponent = Math.Log(FOVMultiplier) / Math.Log(2.0#)

        FOV_Calc()
    End Sub

    Public Sub FOV_Multiplier_Set(Value As Double)

        FOVMultiplier = Value
        FOVMultiplierExponent = Math.Log(FOVMultiplier) / Math.Log(2.0#)

        FOV_Calc()
    End Sub

    Public Sub FOV_Calc()
        Const Min As Single = 0.1# * RadOf1Deg
        Const Max As Single = 179.0# * RadOf1Deg

        FieldOfViewY = CSng(Math.Atan(MapView.GLSize.Y * FOVMultiplier / 2.0#) * 2.0#)
        If FieldOfViewY < Min Then
            FieldOfViewY = Min
            If MapView.GLSize.Y > 0 Then
                FOVMultiplier = 2.0# * Math.Tan(FieldOfViewY / 2.0#) / MapView.GLSize.Y
                FOVMultiplierExponent = Math.Log(FOVMultiplier) / Math.Log(2.0#)
            End If
        ElseIf FieldOfViewY > Max Then
            FieldOfViewY = Max
            If MapView.GLSize.Y > 0 Then
                FOVMultiplier = 2.0# * Math.Tan(FieldOfViewY / 2.0#) / MapView.GLSize.Y
                FOVMultiplierExponent = Math.Log(FOVMultiplier) / Math.Log(2.0#)
            End If
        End If

        MapView.DrawViewLater()
    End Sub

    Public Sub ViewPosSet(NewViewPos As sXYZ_int)

        ViewPos = NewViewPos
        ViewPosClamp()

        MapView.DrawViewLater()
    End Sub

    Public Sub ViewPosChange(Displacement As sXYZ_int)

        ViewPos.X += Displacement.X
        ViewPos.Z += Displacement.Z
        ViewPos.Y += Displacement.Y
        ViewPosClamp()

        MapView.DrawViewLater()
    End Sub

    Private Sub ViewPosClamp()
        Const MaxHeight As Integer = 1048576
        Const MaxDist As Integer = 1048576

        ViewPos.X = Clamp_int(ViewPos.X, -MaxDist, Map.Terrain.TileSize.X * TerrainGridSpacing + MaxDist)
        ViewPos.Z = Clamp_int(ViewPos.Z, -Map.Terrain.TileSize.Y * TerrainGridSpacing - MaxDist, MaxDist)
        ViewPos.Y = Clamp_int(ViewPos.Y, CInt(Math.Ceiling(Map.GetTerrainHeight(New sXY_int(ViewPos.X, -ViewPos.Z)))) + 16, MaxHeight)
    End Sub

    Public Sub ViewAngleSet(NewMatrix As Matrix3D.Matrix3D)

        Matrix3D.MatrixCopy(NewMatrix, ViewAngleMatrix)
        Matrix3D.MatrixNormalize(ViewAngleMatrix)
        Matrix3D.MatrixInvert(ViewAngleMatrix, ViewAngleMatrix_Inverted)
        Matrix3D.MatrixToRPY(ViewAngleMatrix, ViewAngleRPY)

        MapView.DrawViewLater()
    End Sub

    Public Sub ViewAngleSetToDefault()

        Dim matrixA As New Matrix3D.Matrix3D
        Matrix3D.MatrixSetToXAngle(matrixA, Math.Atan(2.0#))
        ViewAngleSet(matrixA)

        MapView.DrawViewLater()
    End Sub

    Public Sub ViewAngleSet_Rotate(NewMatrix As Matrix3D.Matrix3D)
        Dim Flag As Boolean
        Dim XYZ_dbl As Matrix3D.XYZ_dbl
        Dim XYZ_dbl2 As Matrix3D.XYZ_dbl
        'Dim XYZ_lng As sXYZ_lng
        Dim XY_dbl As Matrix3D.XY_dbl

        If ViewMoveType = enumView_Move_Type.RTS And RTSOrbit Then
            Flag = True
            'If ScreenXY_Get_TerrainPos(CInt(Int(GLSize.X / 2.0#)), CInt(Int(GLSize.Y / 2.0#)), XYZ_lng) Then
            '    XYZ_dbl.X = XYZ_lng.X
            '    XYZ_dbl.Y = XYZ_lng.Y
            '    XYZ_dbl.Z = XYZ_lng.Z
            'Else
            If ScreenXY_Get_ViewPlanePos_ForwardDownOnly(CInt(Int(MapView.GLSize.X / 2.0#)), CInt(Int(MapView.GLSize.Y / 2.0#)), 127.5#, XY_dbl) Then
                XYZ_dbl.X = XY_dbl.X
                XYZ_dbl.Y = 127.5#
                XYZ_dbl.Z = -XY_dbl.Y
            Else
                Flag = False
            End If
            'End If
        Else
            Flag = False
        End If

        Matrix3D.MatrixToRPY(NewMatrix, ViewAngleRPY)
        If Flag Then
            If ViewAngleRPY.Pitch < RadOf1Deg * 10.0# Then
                ViewAngleRPY.Pitch = RadOf1Deg * 10.0#
            End If
        End If
        Matrix3D.MatrixSetToRPY(ViewAngleMatrix, ViewAngleRPY)
        Matrix3D.MatrixInvert(ViewAngleMatrix, ViewAngleMatrix_Inverted)

        If Flag Then
            XYZ_dbl2.X = ViewPos.X
            XYZ_dbl2.Y = ViewPos.Y
            XYZ_dbl2.Z = -ViewPos.Z
            MoveToViewTerrainPosFromDistance(XYZ_dbl, (XYZ_dbl2 - XYZ_dbl).GetMagnitude)
        End If

        MapView.DrawViewLater()
    End Sub

    Public Sub LookAtTile(TileNum As sXY_int)
        Dim Pos As sXY_int

        Pos.X = CInt((TileNum.X + 0.5#) * TerrainGridSpacing)
        Pos.Y = CInt((TileNum.Y + 0.5#) * TerrainGridSpacing)
        LookAtPos(Pos)
    End Sub

    Public Sub LookAtPos(Horizontal As sXY_int)
        Dim XYZ_dbl As Matrix3D.XYZ_dbl
        Dim XYZ_int As sXYZ_int
        Dim dblTemp As Double
        Dim A As Integer
        Dim matrixA As New Matrix3D.Matrix3D
        Dim AnglePY As Matrix3D.AnglePY

        Matrix3D.VectorForwardsRotationByMatrix(ViewAngleMatrix, XYZ_dbl)
        dblTemp = Map.GetTerrainHeight(Horizontal)
        A = CInt(Math.Ceiling(dblTemp)) + 128
        If ViewPos.Y < A Then
            ViewPos.Y = A
        End If
        If XYZ_dbl.Y > -0.33333333333333331# Then
            XYZ_dbl.Y = -0.33333333333333331#
            Matrix3D.VectorToPY(XYZ_dbl, AnglePY)
            Matrix3D.MatrixSetToPY(matrixA, AnglePY)
            ViewAngleSet(matrixA)
        End If
        dblTemp = (ViewPos.Y - dblTemp) / XYZ_dbl.Y

        XYZ_int.X = CInt(Horizontal.X + dblTemp * XYZ_dbl.X)
        XYZ_int.Y = ViewPos.Y
        XYZ_int.Z = CInt(-Horizontal.Y + dblTemp * XYZ_dbl.Z)

        ViewPosSet(XYZ_int)
    End Sub

    Public Sub MoveToViewTerrainPosFromDistance(TerrainPos As Matrix3D.XYZ_dbl, Distance As Double)
        Dim XYZ_dbl As Matrix3D.XYZ_dbl
        Dim XYZ_int As sXYZ_int

        Matrix3D.VectorForwardsRotationByMatrix(ViewAngleMatrix, XYZ_dbl)

        XYZ_int.X = CInt(TerrainPos.X - XYZ_dbl.X * Distance)
        XYZ_int.Y = CInt(TerrainPos.Y - XYZ_dbl.Y * Distance)
        XYZ_int.Z = CInt(-TerrainPos.Z - XYZ_dbl.Z * Distance)

        ViewPosSet(XYZ_int)
    End Sub

    Public Function Pos_Get_Screen_XY(Pos As Matrix3D.XYZ_dbl, ByRef Result As sXY_int) As Boolean

        If Pos.Z <= 0.0# Then
            Return False
        End If

        Try
            Dim RatioZ_px As Double = 1.0# / (FOVMultiplier * Pos.Z)
            Result.X = CInt(MapView.GLSize.X / 2.0# + (Pos.X * RatioZ_px))
            Result.Y = CInt(MapView.GLSize.Y / 2.0# - (Pos.Y * RatioZ_px))
            Return True
        Catch

        End Try

        Return False
    End Function

    Public Function ScreenXY_Get_ViewPlanePos(ScreenPos As sXY_int, PlaneHeight As Double, ByRef ResultPos As Matrix3D.XY_dbl) As Boolean
        Dim dblTemp As Double
        Dim XYZ_dbl As Matrix3D.XYZ_dbl
        Dim XYZ_dbl2 As Matrix3D.XYZ_dbl

        Try
            'convert screen pos to vector of one pos unit
            XYZ_dbl.X = (ScreenPos.X - MapView.GLSize.X / 2.0#) * FOVMultiplier
            XYZ_dbl.Y = (MapView.GLSize.Y / 2.0# - ScreenPos.Y) * FOVMultiplier
            XYZ_dbl.Z = 1.0#
            'factor in the view angle
            Matrix3D.VectorRotationByMatrix(ViewAngleMatrix, XYZ_dbl, XYZ_dbl2)
            'get distance to cover the height
            dblTemp = (PlaneHeight - ViewPos.Y) / XYZ_dbl2.Y
            ResultPos.X = ViewPos.X + XYZ_dbl2.X * dblTemp
            ResultPos.Y = ViewPos.Z + XYZ_dbl2.Z * dblTemp
        Catch
            Return False
        End Try
        Return True
    End Function

    Public Function ScreenXY_Get_TerrainPos(ScreenPos As sXY_int, ByRef ResultPos As sWorldPos) As Boolean
        Dim dblTemp As Double
        Dim XYZ_dbl As Matrix3D.XYZ_dbl
        Dim TerrainViewVector As Matrix3D.XYZ_dbl
        Dim X As Integer
        Dim Y As Integer
        Dim LimitA As Matrix3D.XY_dbl
        Dim LimitB As Matrix3D.XY_dbl
        Dim Min As sXY_int
        Dim Max As sXY_int
        Dim TriGradientX As Double
        Dim TriGradientZ As Double
        Dim TriHeightOffset As Double
        Dim Dist As Double
        Dim BestPos As Matrix3D.XYZ_dbl
        Dim BestDist As Double
        Dim Dif As Matrix3D.XYZ_dbl
        Dim InTileX As Double
        Dim InTileZ As Double
        Dim TilePos As Matrix3D.XY_dbl
        Dim TerrainViewPos As Matrix3D.XYZ_dbl

        Try

            TerrainViewPos.X = ViewPos.X
            TerrainViewPos.Y = ViewPos.Y
            TerrainViewPos.Z = -ViewPos.Z

            'convert screen pos to vector of one pos unit
            XYZ_dbl.X = (ScreenPos.X - MapView.GLSize.X / 2.0#) * FOVMultiplier
            XYZ_dbl.Y = (MapView.GLSize.Y / 2.0# - ScreenPos.Y) * FOVMultiplier
            XYZ_dbl.Z = 1.0#
            'rotate the vector so that it points forward and level
            Matrix3D.VectorRotationByMatrix(ViewAngleMatrix, XYZ_dbl, TerrainViewVector)
            TerrainViewVector.Y = -TerrainViewVector.Y 'get the amount of looking down, not up
            TerrainViewVector.Z = -TerrainViewVector.Z 'convert to terrain coordinates from view coordinates
            'get range of possible tiles
            dblTemp = (TerrainViewPos.Y - 255 * Map.HeightMultiplier) / TerrainViewVector.Y
            LimitA.X = TerrainViewPos.X + TerrainViewVector.X * dblTemp
            LimitA.Y = TerrainViewPos.Z + TerrainViewVector.Z * dblTemp
            dblTemp = TerrainViewPos.Y / TerrainViewVector.Y
            LimitB.X = TerrainViewPos.X + TerrainViewVector.X * dblTemp
            LimitB.Y = TerrainViewPos.Z + TerrainViewVector.Z * dblTemp
            Min.X = Math.Max(CInt(Int(Math.Min(LimitA.X, LimitB.X) / TerrainGridSpacing)), 0)
            Min.Y = Math.Max(CInt(Int(Math.Min(LimitA.Y, LimitB.Y) / TerrainGridSpacing)), 0)
            Max.X = Math.Min(CInt(Int(Math.Max(LimitA.X, LimitB.X) / TerrainGridSpacing)), Map.Terrain.TileSize.X - 1)
            Max.Y = Math.Min(CInt(Int(Math.Max(LimitA.Y, LimitB.Y) / TerrainGridSpacing)), Map.Terrain.TileSize.Y - 1)
            'find the nearest valid tile to the view
            BestDist = Double.MaxValue
            BestPos.X = Double.NaN
            BestPos.Y = Double.NaN
            BestPos.Z = Double.NaN
            For Y = Min.Y To Max.Y
                For X = Min.X To Max.X

                    TilePos.X = X * TerrainGridSpacing
                    TilePos.Y = Y * TerrainGridSpacing

                    If Map.Terrain.Tiles(X, Y).Tri Then
                        TriHeightOffset = Map.Terrain.Vertices(X, Y).Height * Map.HeightMultiplier
                        TriGradientX = Map.Terrain.Vertices(X + 1, Y).Height * Map.HeightMultiplier - TriHeightOffset
                        TriGradientZ = Map.Terrain.Vertices(X, Y + 1).Height * Map.HeightMultiplier - TriHeightOffset
                        XYZ_dbl.Y = (TriHeightOffset + (TriGradientX * (TerrainViewPos.X - TilePos.X) + TriGradientZ * (TerrainViewPos.Z - TilePos.Y) + (TriGradientX * TerrainViewVector.X + TriGradientZ * TerrainViewVector.Z) * TerrainViewPos.Y / TerrainViewVector.Y) / TerrainGridSpacing) / (1.0# + (TriGradientX * TerrainViewVector.X + TriGradientZ * TerrainViewVector.Z) / (TerrainViewVector.Y * TerrainGridSpacing))
                        XYZ_dbl.X = TerrainViewPos.X + TerrainViewVector.X * (TerrainViewPos.Y - XYZ_dbl.Y) / TerrainViewVector.Y
                        XYZ_dbl.Z = TerrainViewPos.Z + TerrainViewVector.Z * (TerrainViewPos.Y - XYZ_dbl.Y) / TerrainViewVector.Y
                        InTileX = XYZ_dbl.X / TerrainGridSpacing - X
                        InTileZ = XYZ_dbl.Z / TerrainGridSpacing - Y
                        If InTileZ <= 1.0# - InTileX And InTileX >= 0.0# And InTileZ >= 0.0# And InTileX <= 1.0# And InTileZ <= 1.0# Then
                            Dif = XYZ_dbl - TerrainViewPos
                            Dist = Dif.GetMagnitude
                            If Dist < BestDist Then
                                BestDist = Dist
                                BestPos = XYZ_dbl
                            End If
                        End If

                        TriHeightOffset = Map.Terrain.Vertices(X + 1, Y + 1).Height * Map.HeightMultiplier
                        TriGradientX = Map.Terrain.Vertices(X, Y + 1).Height * Map.HeightMultiplier - TriHeightOffset
                        TriGradientZ = Map.Terrain.Vertices(X + 1, Y).Height * Map.HeightMultiplier - TriHeightOffset
                        XYZ_dbl.Y = (TriHeightOffset + TriGradientX + TriGradientZ + (TriGradientX * (TilePos.X - TerrainViewPos.X) + TriGradientZ * (TilePos.Y - TerrainViewPos.Z) - (TriGradientX * TerrainViewVector.X + TriGradientZ * TerrainViewVector.Z) * TerrainViewPos.Y / TerrainViewVector.Y) / TerrainGridSpacing) / (1.0# - (TriGradientX * TerrainViewVector.X + TriGradientZ * TerrainViewVector.Z) / (TerrainViewVector.Y * TerrainGridSpacing))
                        XYZ_dbl.X = TerrainViewPos.X + TerrainViewVector.X * (TerrainViewPos.Y - XYZ_dbl.Y) / TerrainViewVector.Y
                        XYZ_dbl.Z = TerrainViewPos.Z + TerrainViewVector.Z * (TerrainViewPos.Y - XYZ_dbl.Y) / TerrainViewVector.Y
                        InTileX = XYZ_dbl.X / TerrainGridSpacing - X
                        InTileZ = XYZ_dbl.Z / TerrainGridSpacing - Y
                        If InTileZ >= 1.0# - InTileX And InTileX >= 0.0# And InTileZ >= 0.0# And InTileX <= 1.0# And InTileZ <= 1.0# Then
                            Dif = XYZ_dbl - TerrainViewPos
                            Dist = Dif.GetMagnitude
                            If Dist < BestDist Then
                                BestDist = Dist
                                BestPos = XYZ_dbl
                            End If
                        End If

                    Else

                        TriHeightOffset = Map.Terrain.Vertices(X + 1, Y).Height * Map.HeightMultiplier
                        TriGradientX = Map.Terrain.Vertices(X, Y).Height * Map.HeightMultiplier - TriHeightOffset
                        TriGradientZ = Map.Terrain.Vertices(X + 1, Y + 1).Height * Map.HeightMultiplier - TriHeightOffset
                        XYZ_dbl.Y = (TriHeightOffset + TriGradientX + (TriGradientX * (TilePos.X - TerrainViewPos.X) + TriGradientZ * (TerrainViewPos.Z - TilePos.Y) - (TriGradientX * TerrainViewVector.X - TriGradientZ * TerrainViewVector.Z) * TerrainViewPos.Y / TerrainViewVector.Y) / TerrainGridSpacing) / (1.0# - (TriGradientX * TerrainViewVector.X - TriGradientZ * TerrainViewVector.Z) / (TerrainViewVector.Y * TerrainGridSpacing))
                        XYZ_dbl.X = TerrainViewPos.X + TerrainViewVector.X * (TerrainViewPos.Y - XYZ_dbl.Y) / TerrainViewVector.Y
                        XYZ_dbl.Z = TerrainViewPos.Z + TerrainViewVector.Z * (TerrainViewPos.Y - XYZ_dbl.Y) / TerrainViewVector.Y
                        InTileX = XYZ_dbl.X / TerrainGridSpacing - X
                        InTileZ = XYZ_dbl.Z / TerrainGridSpacing - Y
                        If InTileZ <= InTileX And InTileX >= 0.0# And InTileZ >= 0.0# And InTileX <= 1.0# And InTileZ <= 1.0# Then
                            Dif = XYZ_dbl - TerrainViewPos
                            Dist = Dif.GetMagnitude
                            If Dist < BestDist Then
                                BestDist = Dist
                                BestPos = XYZ_dbl
                            End If
                        End If

                        TriHeightOffset = Map.Terrain.Vertices(X, Y + 1).Height * Map.HeightMultiplier
                        TriGradientX = Map.Terrain.Vertices(X + 1, Y + 1).Height * Map.HeightMultiplier - TriHeightOffset
                        TriGradientZ = Map.Terrain.Vertices(X, Y).Height * Map.HeightMultiplier - TriHeightOffset
                        XYZ_dbl.Y = (TriHeightOffset + TriGradientZ + (TriGradientX * (TerrainViewPos.X - TilePos.X) + TriGradientZ * (TilePos.Y - TerrainViewPos.Z) + (TriGradientX * TerrainViewVector.X - TriGradientZ * TerrainViewVector.Z) * TerrainViewPos.Y / TerrainViewVector.Y) / TerrainGridSpacing) / (1.0# + (TriGradientX * TerrainViewVector.X - TriGradientZ * TerrainViewVector.Z) / (TerrainViewVector.Y * TerrainGridSpacing))
                        XYZ_dbl.X = TerrainViewPos.X + TerrainViewVector.X * (TerrainViewPos.Y - XYZ_dbl.Y) / TerrainViewVector.Y
                        XYZ_dbl.Z = TerrainViewPos.Z + TerrainViewVector.Z * (TerrainViewPos.Y - XYZ_dbl.Y) / TerrainViewVector.Y
                        InTileX = XYZ_dbl.X / TerrainGridSpacing - X
                        InTileZ = XYZ_dbl.Z / TerrainGridSpacing - Y
                        If InTileZ >= InTileX And InTileX >= 0.0# And InTileZ >= 0.0# And InTileX <= 1.0# And InTileZ <= 1.0# Then
                            Dif = XYZ_dbl - TerrainViewPos
                            Dist = Dif.GetMagnitude
                            If Dist < BestDist Then
                                BestDist = Dist
                                BestPos = XYZ_dbl
                            End If
                        End If

                    End If
                Next
            Next

            If BestPos.X = Double.NaN Then
                Return False
            End If

            ResultPos.Horizontal.X = CInt(BestPos.X)
            ResultPos.Altitude = CInt(BestPos.Y)
            ResultPos.Horizontal.Y = CInt(BestPos.Z)
        Catch
            Return False
        End Try
        Return True
    End Function

    Public Function ScreenXY_Get_ViewPlanePos_ForwardDownOnly(ScreenX As Integer, ScreenY As Integer, PlaneHeight As Double, ByRef ResultPos As Matrix3D.XY_dbl) As Boolean
        Dim dblTemp As Double
        Dim XYZ_dbl As Matrix3D.XYZ_dbl
        Dim XYZ_dbl2 As Matrix3D.XYZ_dbl
        Dim dblTemp2 As Double

        If ViewPos.Y < PlaneHeight Then
            Return False
        End If

        Try

            'convert screen pos to vector of one pos unit
            dblTemp2 = FOVMultiplier
            XYZ_dbl.X = (ScreenX - MapView.GLSize.X / 2.0#) * dblTemp2
            XYZ_dbl.Y = (MapView.GLSize.Y / 2.0# - ScreenY) * dblTemp2
            XYZ_dbl.Z = 1.0#
            'factor in the view angle
            Matrix3D.VectorRotationByMatrix(ViewAngleMatrix, XYZ_dbl, XYZ_dbl2)
            'get distance to cover the height
            If XYZ_dbl2.Y > 0.0# Then
                Return False
            End If
            dblTemp = (PlaneHeight - ViewPos.Y) / XYZ_dbl2.Y
            ResultPos.X = ViewPos.X + XYZ_dbl2.X * dblTemp
            ResultPos.Y = ViewPos.Z + XYZ_dbl2.Z * dblTemp
        Catch
            Return False
        End Try
        Return True
    End Function

    Public Tiles_Per_Minimap_Pixel As Double

    Public Sub MouseOver_Pos_Calc()
        Dim XY_dbl As Matrix3D.XY_dbl
        Dim Flag As Boolean
        Dim Footprint As sXY_int
        Dim MouseLeftDownOverMinimap As clsMouseDown.clsOverMinimap = GetMouseLeftDownOverMinimap()

        If MouseLeftDownOverMinimap IsNot Nothing Then
            If MouseOver Is Nothing Then

            ElseIf IsViewPosOverMinimap(MouseOver.ScreenPos) Then
                Dim Pos As New sXY_int(CInt(Int(MouseOver.ScreenPos.X * Tiles_Per_Minimap_Pixel)), CInt(Int(MouseOver.ScreenPos.Y * Tiles_Per_Minimap_Pixel)))
                Map.TileNumClampToMap(Pos)
                LookAtTile(Pos)
            End If
        Else
            Dim MouseOverTerrain As New clsMouseOver.clsOverTerrain
            Flag = False
            If Settings.DirectPointer Then
                If ScreenXY_Get_TerrainPos(MouseOver.ScreenPos, MouseOverTerrain.Pos) Then
                    If Map.PosIsOnMap(MouseOverTerrain.Pos.Horizontal) Then
                        Flag = True
                    End If
                End If
            Else
                MouseOverTerrain.Pos.Altitude = CInt(255.0# / 2.0# * Map.HeightMultiplier)
                If ScreenXY_Get_ViewPlanePos(MouseOver.ScreenPos, MouseOverTerrain.Pos.Altitude, XY_dbl) Then
                    MouseOverTerrain.Pos.Horizontal.X = CInt(XY_dbl.X)
                    MouseOverTerrain.Pos.Horizontal.Y = CInt(-XY_dbl.Y)
                    If Map.PosIsOnMap(MouseOverTerrain.Pos.Horizontal) Then
                        MouseOverTerrain.Pos.Altitude = CInt(Map.GetTerrainHeight(MouseOverTerrain.Pos.Horizontal))
                        Flag = True
                    End If
                End If
            End If
            If Flag Then
                MouseOver.OverTerrain = MouseOverTerrain
                MouseOverTerrain.Tile.Normal.X = CInt(Int(MouseOverTerrain.Pos.Horizontal.X / TerrainGridSpacing))
                MouseOverTerrain.Tile.Normal.Y = CInt(Int(MouseOverTerrain.Pos.Horizontal.Y / TerrainGridSpacing))
                MouseOverTerrain.Vertex.Normal.X = CInt(Math.Round(MouseOverTerrain.Pos.Horizontal.X / TerrainGridSpacing))
                MouseOverTerrain.Vertex.Normal.Y = CInt(Math.Round(MouseOverTerrain.Pos.Horizontal.Y / TerrainGridSpacing))
                MouseOverTerrain.Tile.Alignment = MouseOverTerrain.Vertex.Normal
                MouseOverTerrain.Vertex.Alignment = New sXY_int(MouseOverTerrain.Tile.Normal.X + 1, MouseOverTerrain.Tile.Normal.Y + 1)
                MouseOverTerrain.Triangle = Map.GetTerrainTri(MouseOverTerrain.Pos.Horizontal)
                XY_dbl.X = MouseOverTerrain.Pos.Horizontal.X - MouseOverTerrain.Vertex.Normal.X * TerrainGridSpacing
                XY_dbl.Y = MouseOverTerrain.Pos.Horizontal.Y - MouseOverTerrain.Vertex.Normal.Y * TerrainGridSpacing
                If Math.Abs(XY_dbl.Y) <= Math.Abs(XY_dbl.X) Then
                    MouseOverTerrain.Side_IsV = False
                    MouseOverTerrain.Side_Num.X = MouseOverTerrain.Tile.Normal.X
                    MouseOverTerrain.Side_Num.Y = MouseOverTerrain.Vertex.Normal.Y
                Else
                    MouseOverTerrain.Side_IsV = True
                    MouseOverTerrain.Side_Num.X = MouseOverTerrain.Vertex.Normal.X
                    MouseOverTerrain.Side_Num.Y = MouseOverTerrain.Tile.Normal.Y
                End If
                Dim SectorNum As sXY_int = Map.GetPosSectorNum(MouseOverTerrain.Pos.Horizontal)
                Dim Unit As clsMap.clsUnit
                Dim Connection As clsMap.clsUnitSectorConnection
                For Each Connection In Map.Sectors(SectorNum.X, SectorNum.Y).Units
                    Unit = Connection.Unit
                    XY_dbl.X = Unit.Pos.Horizontal.X - MouseOverTerrain.Pos.Horizontal.X
                    XY_dbl.Y = Unit.Pos.Horizontal.Y - MouseOverTerrain.Pos.Horizontal.Y
                    Footprint = Unit.Type.GetFootprintSelected(Unit.Rotation)
                    If Math.Abs(XY_dbl.X) <= Math.Max(Footprint.X / 2.0#, 0.5#) * TerrainGridSpacing _
                    And Math.Abs(XY_dbl.Y) <= Math.Max(Footprint.Y / 2.0#, 0.5#) * TerrainGridSpacing Then
                        MouseOverTerrain.Units.Add(Unit)
                    End If
                Next

                If MouseLeftDown IsNot Nothing Then
                    If Tool Is Tools.TerrainBrush Then
                        Apply_Terrain()
                        If frmMainInstance.cbxAutoTexSetHeight.Checked Then
                            Apply_Height_Set(TerrainBrush, frmMainInstance.HeightSetPalette(frmMainInstance.tabHeightSetL.SelectedIndex))
                        End If
                    ElseIf Tool Is Tools.HeightSetBrush Then
                        Apply_Height_Set(HeightBrush, frmMainInstance.HeightSetPalette(frmMainInstance.tabHeightSetL.SelectedIndex))
                    ElseIf Tool Is Tools.TextureBrush Then
                        Apply_Texture()
                    ElseIf Tool Is Tools.CliffTriangle Then
                        Apply_CliffTriangle(False)
                    ElseIf Tool Is Tools.CliffBrush Then
                        Apply_Cliff()
                    ElseIf Tool Is Tools.CliffRemove Then
                        Apply_Cliff_Remove()
                    ElseIf Tool Is Tools.RoadPlace Then
                        Apply_Road()
                    ElseIf Tool Is Tools.RoadRemove Then
                        Apply_Road_Remove()
                    End If
                End If
                If MouseRightDown IsNot Nothing Then
                    If Tool Is Tools.HeightSetBrush Then
                        If MouseLeftDown Is Nothing Then
                            Apply_Height_Set(HeightBrush, frmMainInstance.HeightSetPalette(frmMainInstance.tabHeightSetR.SelectedIndex))
                        End If
                    ElseIf Tool Is Tools.CliffTriangle Then
                        Apply_CliffTriangle(True)
                    End If
                End If
            End If
        End If
        MapView.Pos_Display_Update()
        MapView.DrawViewLater()
    End Sub

    Public Function GetMouseOverTerrain() As clsMouseOver.clsOverTerrain

        If MouseOver Is Nothing Then
            Return Nothing
        Else
            Return MouseOver.OverTerrain
        End If
    End Function

    Public Function GetMouseLeftDownOverTerrain() As clsMouseDown.clsOverTerrain

        If MouseLeftDown Is Nothing Then
            Return Nothing
        Else
            Return MouseLeftDown.OverTerrain
        End If
    End Function

    Public Function GetMouseRightDownOverTerrain() As clsMouseDown.clsOverTerrain

        If MouseRightDown Is Nothing Then
            Return Nothing
        Else
            Return MouseRightDown.OverTerrain
        End If
    End Function

    Public Function GetMouseLeftDownOverMinimap() As clsMouseDown.clsOverMinimap

        If MouseLeftDown Is Nothing Then
            Return Nothing
        Else
            Return MouseLeftDown.OverMinimap
        End If
    End Function

    Public Function GetMouseRightDownOverMinimap() As clsMouseDown.clsOverMinimap

        If MouseRightDown Is Nothing Then
            Return Nothing
        Else
            Return MouseRightDown.OverMinimap
        End If
    End Function

    Public Class clsMouseOver
        Public ScreenPos As sXY_int
        Public Class clsOverTerrain
            Public Pos As sWorldPos
            Public Units As New SimpleClassList(Of clsMap.clsUnit)
            Public Tile As clsBrush.sPosNum
            Public Vertex As clsBrush.sPosNum
            Public Triangle As Boolean
            Public Side_Num As sXY_int
            Public Side_IsV As Boolean
        End Class
        Public OverTerrain As clsOverTerrain
    End Class
    Public Class clsMouseDown
        Public Class clsOverTerrain
            Public DownPos As sWorldPos
        End Class
        Public OverTerrain As clsOverTerrain
        Public Class clsOverMinimap
            Public DownPos As sXY_int
        End Class
        Public OverMinimap As clsOverMinimap
    End Class

    Public MouseOver As clsMouseOver

    Public MouseLeftDown As clsMouseDown
    Public MouseRightDown As clsMouseDown

    Public Function IsViewPosOverMinimap(Pos As sXY_int) As Boolean

        If Pos.X >= 0 And Pos.X < Map.Terrain.TileSize.X / Tiles_Per_Minimap_Pixel _
            And Pos.Y >= 0 And Pos.Y < Map.Terrain.TileSize.Y / Tiles_Per_Minimap_Pixel Then
            Return True
        Else
            Return False
        End If
    End Function

    Public Sub Apply_Terrain()
        Dim MouseOverTerrain As clsMouseOver.clsOverTerrain = GetMouseOverTerrain()

        If MouseOverTerrain Is Nothing Then
            Exit Sub
        End If

        Dim ApplyVertexTerrain As New clsMap.clsApplyVertexTerrain
        ApplyVertexTerrain.Map = Map
        ApplyVertexTerrain.VertexTerrain = SelectedTerrain
        TerrainBrush.PerformActionMapVertices(ApplyVertexTerrain, MouseOverTerrain.Vertex)

        Map.Update()

        MapView.DrawViewLater()
    End Sub

    Public Sub Apply_Road()
        Dim MouseOverTerrain As clsMouseOver.clsOverTerrain = GetMouseOverTerrain()

        If MouseOverTerrain Is Nothing Then
            Exit Sub
        End If

        Dim Side_Num As sXY_int = MouseOverTerrain.Side_Num
        Dim TileNum As sXY_int

        If MouseOverTerrain.Side_IsV Then
            If Map.Terrain.SideV(Side_Num.X, Side_Num.Y).Road IsNot SelectedRoad Then

                Map.Terrain.SideV(Side_Num.X, Side_Num.Y).Road = SelectedRoad

                If Side_Num.X > 0 Then
                    TileNum.X = Side_Num.X - 1
                    TileNum.Y = Side_Num.Y
                    Map.AutoTextureChanges.TileChanged(TileNum)
                    Map.SectorGraphicsChanges.TileChanged(TileNum)
                    Map.SectorTerrainUndoChanges.TileChanged(TileNum)
                End If
                If Side_Num.X < Map.Terrain.TileSize.X Then
                    TileNum = Side_Num
                    Map.AutoTextureChanges.TileChanged(TileNum)
                    Map.SectorGraphicsChanges.TileChanged(TileNum)
                    Map.SectorTerrainUndoChanges.TileChanged(TileNum)
                End If

                Map.Update()

                Map.UndoStepCreate("Road Side")

                MapView.DrawViewLater()
            End If
        Else
            If Map.Terrain.SideH(Side_Num.X, Side_Num.Y).Road IsNot SelectedRoad Then

                Map.Terrain.SideH(Side_Num.X, Side_Num.Y).Road = SelectedRoad

                If Side_Num.Y > 0 Then
                    TileNum.X = Side_Num.X
                    TileNum.Y = Side_Num.Y - 1
                    Map.AutoTextureChanges.TileChanged(TileNum)
                    Map.SectorGraphicsChanges.TileChanged(TileNum)
                    Map.SectorTerrainUndoChanges.TileChanged(TileNum)
                End If
                If Side_Num.Y < Map.Terrain.TileSize.X Then
                    TileNum = Side_Num
                    Map.AutoTextureChanges.TileChanged(TileNum)
                    Map.SectorGraphicsChanges.TileChanged(TileNum)
                    Map.SectorTerrainUndoChanges.TileChanged(TileNum)
                End If

                Map.Update()

                Map.UndoStepCreate("Road Side")

                MapView.DrawViewLater()
            End If
        End If
    End Sub

    Public Sub Apply_Road_Line_Selection()
        Dim MouseOverTerrian As clsMouseOver.clsOverTerrain = GetMouseOverTerrain()

        If MouseOverTerrian Is Nothing Then
            Exit Sub
        End If

        Dim Num As Integer
        Dim A As Integer
        Dim B As Integer
        Dim Tile As sXY_int = MouseOverTerrian.Tile.Normal
        Dim SideNum As sXY_int

        If Map.Selected_Tile_A IsNot Nothing Then
            If Tile.X = Map.Selected_Tile_A.X Then
                If Tile.Y <= Map.Selected_Tile_A.Y Then
                    A = Tile.Y
                    B = Map.Selected_Tile_A.Y
                Else
                    A = Map.Selected_Tile_A.Y
                    B = Tile.Y
                End If
                For Num = A + 1 To B
                    Map.Terrain.SideH(Map.Selected_Tile_A.X, Num).Road = SelectedRoad
                    SideNum.X = Map.Selected_Tile_A.X
                    SideNum.Y = Num
                    Map.AutoTextureChanges.SideHChanged(SideNum)
                    Map.SectorGraphicsChanges.SideHChanged(SideNum)
                    Map.SectorTerrainUndoChanges.SideHChanged(SideNum)
                Next

                Map.Update()

                Map.UndoStepCreate("Road Line")

                Map.Selected_Tile_A = Nothing
                MapView.DrawViewLater()
            ElseIf Tile.Y = Map.Selected_Tile_A.Y Then
                If Tile.X <= Map.Selected_Tile_A.X Then
                    A = Tile.X
                    B = Map.Selected_Tile_A.X
                Else
                    A = Map.Selected_Tile_A.X
                    B = Tile.X
                End If
                For Num = A + 1 To B
                    Map.Terrain.SideV(Num, Map.Selected_Tile_A.Y).Road = SelectedRoad
                    SideNum.X = Num
                    SideNum.Y = Map.Selected_Tile_A.Y
                    Map.AutoTextureChanges.SideVChanged(SideNum)
                    Map.SectorGraphicsChanges.SideVChanged(SideNum)
                    Map.SectorTerrainUndoChanges.SideVChanged(SideNum)
                Next

                Map.Update()

                Map.UndoStepCreate("Road Line")

                Map.Selected_Tile_A = Nothing
                MapView.DrawViewLater()
            Else

            End If
        Else
            Map.Selected_Tile_A = New clsXY_int(Tile)
        End If
    End Sub

    Public Sub Apply_Terrain_Fill(CliffAction As enumFillCliffAction, Inside As Boolean)
        Dim MouseOverTerrain As clsMouseOver.clsOverTerrain = GetMouseOverTerrain()

        If MouseOverTerrain Is Nothing Then
            Exit Sub
        End If

        Dim FillType As clsPainter.clsTerrain
        Dim ReplaceType As clsPainter.clsTerrain
        Dim StartVertex As sXY_int = MouseOverTerrain.Vertex.Normal

        FillType = SelectedTerrain
        ReplaceType = Map.Terrain.Vertices(StartVertex.X, StartVertex.Y).Terrain
        If FillType Is ReplaceType Then
            Exit Sub 'otherwise will cause endless loop
        End If

        Dim A As Integer
        Dim SourceOfFill(524288) As sXY_int
        Dim SourceOfFillCount As Integer
        Dim SourceOfFillNum As Integer
        Dim MoveCount As Integer
        Dim RemainingCount As Integer
        Dim MoveOffset As Integer
        Dim CurrentSource As sXY_int
        Dim NextSource As sXY_int
        Dim StopForCliff As Boolean
        Dim StopForEdge As Boolean

        SourceOfFill(0) = StartVertex
        SourceOfFillCount = 1
        SourceOfFillNum = 0
        Do While SourceOfFillNum < SourceOfFillCount
            CurrentSource = SourceOfFill(SourceOfFillNum)

            If CliffAction = enumFillCliffAction.StopBefore Then
                StopForCliff = Map.VertexIsCliffEdge(CurrentSource)
            Else
                StopForCliff = False
            End If
            StopForEdge = False
            If Inside Then
                If CurrentSource.X > 0 Then
                    If CurrentSource.Y > 0 Then
                        If Map.Terrain.Vertices(CurrentSource.X - 1, CurrentSource.Y - 1).Terrain IsNot ReplaceType _
                            And Map.Terrain.Vertices(CurrentSource.X - 1, CurrentSource.Y - 1).Terrain IsNot FillType Then
                            StopForEdge = True
                        End If
                    End If
                    If Map.Terrain.Vertices(CurrentSource.X - 1, CurrentSource.Y).Terrain IsNot ReplaceType _
                        And Map.Terrain.Vertices(CurrentSource.X - 1, CurrentSource.Y).Terrain IsNot FillType Then
                        StopForEdge = True
                    End If
                    If CurrentSource.Y < Map.Terrain.TileSize.Y Then
                        If Map.Terrain.Vertices(CurrentSource.X - 1, CurrentSource.Y + 1).Terrain IsNot ReplaceType _
                            And Map.Terrain.Vertices(CurrentSource.X - 1, CurrentSource.Y + 1).Terrain IsNot FillType Then
                            StopForEdge = True
                        End If
                    End If
                End If
                If CurrentSource.Y > 0 Then
                    If Map.Terrain.Vertices(CurrentSource.X, CurrentSource.Y - 1).Terrain IsNot ReplaceType _
                        And Map.Terrain.Vertices(CurrentSource.X, CurrentSource.Y - 1).Terrain IsNot FillType Then
                        StopForEdge = True
                    End If
                End If
                If CurrentSource.X < Map.Terrain.TileSize.X Then
                    If CurrentSource.Y > 0 Then
                        If Map.Terrain.Vertices(CurrentSource.X + 1, CurrentSource.Y - 1).Terrain IsNot ReplaceType _
                            And Map.Terrain.Vertices(CurrentSource.X + 1, CurrentSource.Y - 1).Terrain IsNot FillType Then
                            StopForEdge = True
                        End If
                    End If
                    If Map.Terrain.Vertices(CurrentSource.X + 1, CurrentSource.Y).Terrain IsNot ReplaceType _
                        And Map.Terrain.Vertices(CurrentSource.X + 1, CurrentSource.Y).Terrain IsNot FillType Then
                        StopForEdge = True
                    End If
                    If CurrentSource.Y < Map.Terrain.TileSize.Y Then
                        If Map.Terrain.Vertices(CurrentSource.X + 1, CurrentSource.Y + 1).Terrain IsNot ReplaceType _
                            And Map.Terrain.Vertices(CurrentSource.X + 1, CurrentSource.Y + 1).Terrain IsNot FillType Then
                            StopForEdge = True
                        End If
                    End If
                End If
                If CurrentSource.Y < Map.Terrain.TileSize.Y Then
                    If Map.Terrain.Vertices(CurrentSource.X, CurrentSource.Y + 1).Terrain IsNot ReplaceType _
                        And Map.Terrain.Vertices(CurrentSource.X, CurrentSource.Y + 1).Terrain IsNot FillType Then
                        StopForEdge = True
                    End If
                End If
            End If

            If Not (StopForCliff Or StopForEdge) Then
                If Map.Terrain.Vertices(CurrentSource.X, CurrentSource.Y).Terrain Is ReplaceType Then
                    Map.Terrain.Vertices(CurrentSource.X, CurrentSource.Y).Terrain = FillType
                    Map.SectorGraphicsChanges.VertexChanged(CurrentSource)
                    Map.SectorTerrainUndoChanges.VertexChanged(CurrentSource)
                    Map.AutoTextureChanges.VertexChanged(CurrentSource)

                    NextSource.X = CurrentSource.X + 1
                    NextSource.Y = CurrentSource.Y
                    If NextSource.X >= 0 And NextSource.X <= Map.Terrain.TileSize.X _
                     And NextSource.Y >= 0 And NextSource.Y <= Map.Terrain.TileSize.Y Then
                        If CliffAction = enumFillCliffAction.StopAfter Then
                            StopForCliff = Map.SideHIsCliffOnBothSides(New sXY_int(CurrentSource.X, CurrentSource.Y))
                        Else
                            StopForCliff = False
                        End If
                        If Not StopForCliff Then
                            If Map.Terrain.Vertices(NextSource.X, NextSource.Y).Terrain Is ReplaceType Then
                                If SourceOfFill.GetUpperBound(0) < SourceOfFillCount Then
                                    ReDim Preserve SourceOfFill(SourceOfFillCount * 2 + 1)
                                End If
                                SourceOfFill(SourceOfFillCount) = NextSource
                                SourceOfFillCount += 1
                            End If
                        End If
                    End If

                    NextSource.X = CurrentSource.X - 1
                    NextSource.Y = CurrentSource.Y
                    If NextSource.X >= 0 And NextSource.X <= Map.Terrain.TileSize.X _
                     And NextSource.Y >= 0 And NextSource.Y <= Map.Terrain.TileSize.Y Then
                        If CliffAction = enumFillCliffAction.StopAfter Then
                            StopForCliff = Map.SideHIsCliffOnBothSides(New sXY_int(CurrentSource.X - 1, CurrentSource.Y))
                        Else
                            StopForCliff = False
                        End If
                        If Not StopForCliff Then
                            If Map.Terrain.Vertices(NextSource.X, NextSource.Y).Terrain Is ReplaceType Then
                                If SourceOfFill.GetUpperBound(0) < SourceOfFillCount Then
                                    ReDim Preserve SourceOfFill(SourceOfFillCount * 2 + 1)
                                End If
                                SourceOfFill(SourceOfFillCount) = NextSource
                                SourceOfFillCount += 1
                            End If
                        End If
                    End If

                    NextSource.X = CurrentSource.X
                    NextSource.Y = CurrentSource.Y + 1
                    If NextSource.X >= 0 And NextSource.X <= Map.Terrain.TileSize.X _
                     And NextSource.Y >= 0 And NextSource.Y <= Map.Terrain.TileSize.Y Then
                        If CliffAction = enumFillCliffAction.StopAfter Then
                            StopForCliff = Map.SideVIsCliffOnBothSides(New sXY_int(CurrentSource.X, CurrentSource.Y))
                        Else
                            StopForCliff = False
                        End If
                        If Not StopForCliff Then
                            If Map.Terrain.Vertices(NextSource.X, NextSource.Y).Terrain Is ReplaceType Then
                                If SourceOfFill.GetUpperBound(0) < SourceOfFillCount Then
                                    ReDim Preserve SourceOfFill(SourceOfFillCount * 2 + 1)
                                End If
                                SourceOfFill(SourceOfFillCount) = NextSource
                                SourceOfFillCount += 1
                            End If
                        End If
                    End If

                    NextSource.X = CurrentSource.X
                    NextSource.Y = CurrentSource.Y - 1
                    If NextSource.X >= 0 And NextSource.X <= Map.Terrain.TileSize.X _
                     And NextSource.Y >= 0 And NextSource.Y <= Map.Terrain.TileSize.Y Then
                        If CliffAction = enumFillCliffAction.StopAfter Then
                            StopForCliff = Map.SideVIsCliffOnBothSides(New sXY_int(CurrentSource.X, CurrentSource.Y - 1))
                        Else
                            StopForCliff = False
                        End If
                        If Not StopForCliff Then
                            If Map.Terrain.Vertices(NextSource.X, NextSource.Y).Terrain Is ReplaceType Then
                                If SourceOfFill.GetUpperBound(0) < SourceOfFillCount Then
                                    ReDim Preserve SourceOfFill(SourceOfFillCount * 2 + 1)
                                End If
                                SourceOfFill(SourceOfFillCount) = NextSource
                                SourceOfFillCount += 1
                            End If
                        End If
                    End If

                End If
            End If

            SourceOfFillNum += 1

            If SourceOfFillNum >= 131072 Then
                RemainingCount = SourceOfFillCount - SourceOfFillNum
                MoveCount = Math.Min(SourceOfFillNum, RemainingCount)
                MoveOffset = SourceOfFillCount - MoveCount
                For A = 0 To MoveCount - 1
                    SourceOfFill(A) = SourceOfFill(MoveOffset + A)
                Next
                SourceOfFillCount -= SourceOfFillNum
                SourceOfFillNum = 0
                If SourceOfFillCount * 3 < SourceOfFill.GetUpperBound(0) + 1 Then
                    ReDim Preserve SourceOfFill(SourceOfFillCount * 2 + 1)
                End If
            End If
        Loop

        Map.Update()

        Map.UndoStepCreate("Ground Fill")

        MapView.DrawViewLater()
    End Sub

    Public Sub Apply_Texture()
        Dim MouseOverTerrain As clsMouseOver.clsOverTerrain = GetMouseOverTerrain()

        If MouseOverTerrain Is Nothing Then
            Exit Sub
        End If

        Dim ApplyTexture As New clsMap.clsApplyTexture
        ApplyTexture.Map = Map
        ApplyTexture.TextureNum = SelectedTextureNum
        ApplyTexture.SetTexture = frmMainInstance.chkSetTexture.Checked
        ApplyTexture.Orientation = TextureOrientation
        ApplyTexture.RandomOrientation = frmMainInstance.chkTextureOrientationRandomize.Checked
        ApplyTexture.SetOrientation = frmMainInstance.chkSetTextureOrientation.Checked
        ApplyTexture.TerrainAction = frmMainInstance.TextureTerrainAction
        TextureBrush.PerformActionMapTiles(ApplyTexture, MouseOverTerrain.Tile)

        Map.Update()

        MapView.DrawViewLater()
    End Sub

    Public Sub Apply_CliffTriangle(Remove As Boolean)
        Dim MouseOverTerrain As clsMouseOver.clsOverTerrain = GetMouseOverTerrain()

        If MouseOverTerrain Is Nothing Then
            Exit Sub
        End If

        If Remove Then
            Dim ApplyCliffTriangleRemove As New clsMap.clsApplyCliffTriangleRemove
            ApplyCliffTriangleRemove.Map = Map
            ApplyCliffTriangleRemove.PosNum = MouseOverTerrain.Tile.Normal
            ApplyCliffTriangleRemove.Triangle = MouseOverTerrain.Triangle
            ApplyCliffTriangleRemove.ActionPerform()
        Else
            Dim ApplyCliffTriangle As New clsMap.clsApplyCliffTriangle
            ApplyCliffTriangle.Map = Map
            ApplyCliffTriangle.PosNum = MouseOverTerrain.Tile.Normal
            ApplyCliffTriangle.Triangle = MouseOverTerrain.Triangle
            ApplyCliffTriangle.ActionPerform()
        End If

        Map.Update()

        MapView.DrawViewLater()
    End Sub

    Public Sub Apply_Cliff()
        Dim MouseOverTerrain As clsMouseOver.clsOverTerrain = GetMouseOverTerrain()

        If MouseOverTerrain Is Nothing Then
            Exit Sub
        End If

        Dim ApplyCliff As New clsMap.clsApplyCliff
        ApplyCliff.Map = Map
        Dim Angle As Double
        If Not InvariantParse_dbl(frmMainInstance.txtAutoCliffSlope.Text, Angle) Then
            Exit Sub
        End If
        ApplyCliff.Angle = Clamp_dbl(Angle * RadOf1Deg, 0.0#, RadOf90Deg)
        ApplyCliff.SetTris = frmMainInstance.cbxCliffTris.Checked
        CliffBrush.PerformActionMapTiles(ApplyCliff, MouseOverTerrain.Tile)

        Map.Update()

        MapView.DrawViewLater()
    End Sub

    Public Sub Apply_Cliff_Remove()
        Dim MouseOverTerrain As clsMouseOver.clsOverTerrain = GetMouseOverTerrain()

        If MouseOverTerrain Is Nothing Then
            Exit Sub
        End If

        Dim ApplyCliffRemove As New clsMap.clsApplyCliffRemove
        ApplyCliffRemove.Map = Map
        CliffBrush.PerformActionMapTiles(ApplyCliffRemove, MouseOverTerrain.Tile)

        Map.Update()

        MapView.DrawViewLater()
    End Sub

    Public Sub Apply_Road_Remove()
        Dim MouseOverTerrain As clsMouseOver.clsOverTerrain = GetMouseOverTerrain()

        If MouseOverTerrain Is Nothing Then
            Exit Sub
        End If

        Dim ApplyRoadRemove As New clsMap.clsApplyRoadRemove
        ApplyRoadRemove.Map = Map
        CliffBrush.PerformActionMapTiles(ApplyRoadRemove, MouseOverTerrain.Tile)

        Map.Update()

        MapView.DrawViewLater()
    End Sub

    Public Sub Apply_Texture_Clockwise()
        Dim MouseOverTerrain As clsMouseOver.clsOverTerrain = GetMouseOverTerrain()

        If MouseOverTerrain Is Nothing Then
            Exit Sub
        End If

        Dim Tile As sXY_int = MouseOverTerrain.Tile.Normal

        Map.Terrain.Tiles(Tile.X, Tile.Y).Texture.Orientation.RotateClockwise()
        Map.TileTextureChangeTerrainAction(Tile, frmMainInstance.TextureTerrainAction)

        Map.SectorGraphicsChanges.TileChanged(Tile)
        Map.SectorTerrainUndoChanges.TileChanged(Tile)

        Map.Update()

        Map.UndoStepCreate("Texture Rotate")

        MapView.DrawViewLater()
    End Sub

    Public Sub Apply_Texture_CounterClockwise()
        Dim MouseOverTerrain As clsMouseOver.clsOverTerrain = GetMouseOverTerrain()

        If MouseOverTerrain Is Nothing Then
            Exit Sub
        End If

        Dim Tile As sXY_int = MouseOverTerrain.Tile.Normal

        Map.Terrain.Tiles(Tile.X, Tile.Y).Texture.Orientation.RotateAnticlockwise()
        Map.TileTextureChangeTerrainAction(Tile, frmMainInstance.TextureTerrainAction)

        Map.SectorGraphicsChanges.TileChanged(Tile)
        Map.SectorTerrainUndoChanges.TileChanged(Tile)

        Map.Update()

        Map.UndoStepCreate("Texture Rotate")

        MapView.DrawViewLater()
    End Sub

    Public Sub Apply_Texture_FlipX()
        Dim MouseOverTerrain As clsMouseOver.clsOverTerrain = GetMouseOverTerrain()

        If MouseOverTerrain Is Nothing Then
            Exit Sub
        End If

        Dim Tile As sXY_int = MouseOverTerrain.Tile.Normal

        Map.Terrain.Tiles(Tile.X, Tile.Y).Texture.Orientation.ResultXFlip = Not Map.Terrain.Tiles(Tile.X, Tile.Y).Texture.Orientation.ResultXFlip
        Map.TileTextureChangeTerrainAction(Tile, frmMainInstance.TextureTerrainAction)

        Map.SectorGraphicsChanges.TileChanged(Tile)
        Map.SectorTerrainUndoChanges.TileChanged(Tile)

        Map.Update()

        Map.UndoStepCreate("Texture Rotate")

        MapView.DrawViewLater()
    End Sub

    Public Sub Apply_Tri_Flip()
        Dim MouseOverTerrain As clsMouseOver.clsOverTerrain = GetMouseOverTerrain()

        If MouseOverTerrain Is Nothing Then
            Exit Sub
        End If

        Dim Tile As sXY_int = MouseOverTerrain.Tile.Normal

        Map.Terrain.Tiles(Tile.X, Tile.Y).Tri = Not Map.Terrain.Tiles(Tile.X, Tile.Y).Tri

        Map.SectorGraphicsChanges.TileChanged(Tile)
        Map.SectorTerrainUndoChanges.TileChanged(Tile)

        Map.Update()

        Map.UndoStepCreate("Triangle Flip")

        MapView.DrawViewLater()
    End Sub

    Public Sub Apply_HeightSmoothing(Ratio As Double)
        Dim MouseOverTerrain As clsMouseOver.clsOverTerrain = GetMouseOverTerrain()

        If MouseOverTerrain Is Nothing Then
            Exit Sub
        End If

        Dim ApplyHeightSmoothing As New clsMap.clsApplyHeightSmoothing
        ApplyHeightSmoothing.Map = Map
        ApplyHeightSmoothing.Ratio = Ratio
        Dim Radius As Integer = CInt(Math.Ceiling(HeightBrush.Radius))
        Dim PosNum As sXY_int = HeightBrush.GetPosNum(MouseOverTerrain.Vertex)
        ApplyHeightSmoothing.Offset.X = Clamp_int(PosNum.X - Radius, 0, Map.Terrain.TileSize.X)
        ApplyHeightSmoothing.Offset.Y = Clamp_int(PosNum.Y - Radius, 0, Map.Terrain.TileSize.Y)
        Dim PosEnd As sXY_int
        PosEnd.X = Clamp_int(PosNum.X + Radius, 0, Map.Terrain.TileSize.X)
        PosEnd.Y = Clamp_int(PosNum.Y + Radius, 0, Map.Terrain.TileSize.Y)
        ApplyHeightSmoothing.AreaTileSize.X = PosEnd.X - ApplyHeightSmoothing.Offset.X
        ApplyHeightSmoothing.AreaTileSize.Y = PosEnd.Y - ApplyHeightSmoothing.Offset.Y
        ApplyHeightSmoothing.Start()
        HeightBrush.PerformActionMapVertices(ApplyHeightSmoothing, MouseOverTerrain.Vertex)
        ApplyHeightSmoothing.Finish()

        Map.Update()

        MapView.DrawViewLater()
    End Sub

    Public Sub Apply_Height_Change(Rate As Double)
        Dim MouseOverTerrain As clsMouseOver.clsOverTerrain = GetMouseOverTerrain()

        If MouseOverTerrain Is Nothing Then
            Exit Sub
        End If

        Dim ApplyHeightChange As New clsMap.clsApplyHeightChange
        ApplyHeightChange.Map = Map
        ApplyHeightChange.Rate = Rate
        ApplyHeightChange.UseEffect = frmMainInstance.cbxHeightChangeFade.Checked
        HeightBrush.PerformActionMapVertices(ApplyHeightChange, MouseOverTerrain.Vertex)

        Map.Update()

        MapView.DrawViewLater()
    End Sub

    Public Sub Apply_Height_Set(Brush As clsBrush, Height As Byte)
        Dim MouseOverTerrain As clsMouseOver.clsOverTerrain = GetMouseOverTerrain()

        If MouseOverTerrain Is Nothing Then
            Exit Sub
        End If

        Dim ApplyHeightSet As New clsMap.clsApplyHeightSet
        ApplyHeightSet.Map = Map
        ApplyHeightSet.Height = Height
        Brush.PerformActionMapVertices(ApplyHeightSet, MouseOverTerrain.Vertex)

        Map.Update()

        MapView.DrawViewLater()
    End Sub

    Public Sub Apply_Gateway()
        Dim MouseOverTerrain As clsMouseOver.clsOverTerrain = GetMouseOverTerrain()

        If MouseOverTerrain Is Nothing Then
            Exit Sub
        End If

        Dim Tile As sXY_int = MouseOverTerrain.Tile.Normal

        If KeyboardProfile.Active(Control_Gateway_Delete) Then
            Dim A As Integer
            Dim Low As sXY_int
            Dim High As sXY_int
            A = 0
            Do While A < Map.Gateways.Count
                ReorderXY(Map.Gateways.Item(A).PosA, Map.Gateways.Item(A).PosB, Low, High)
                If Low.X <= Tile.X _
                And High.X >= Tile.X _
                And Low.Y <= Tile.Y _
                And High.Y >= Tile.Y Then
                    Map.GatewayRemoveStoreChange(A)
                    Map.UndoStepCreate("Gateway Delete")
                    Map.MinimapMakeLater()
                    MapView.DrawViewLater()
                    Exit Do
                End If
                A += 1
            Loop
        Else
            If Map.Selected_Tile_A Is Nothing Then
                Map.Selected_Tile_A = New clsXY_int(Tile)
                MapView.DrawViewLater()
            ElseIf Tile.X = Map.Selected_Tile_A.X Or Tile.Y = Map.Selected_Tile_A.Y Then
                If Map.GatewayCreateStoreChange(Map.Selected_Tile_A.XY, Tile) IsNot Nothing Then
                    Map.UndoStepCreate("Gateway Place")
                    Map.Selected_Tile_A = Nothing
                    Map.Selected_Tile_B = Nothing
                    Map.MinimapMakeLater()
                    MapView.DrawViewLater()
                End If
            End If
        End If
    End Sub

    Public Sub MouseDown(e As MouseEventArgs)
        Dim ScreenPos As sXY_int

        Map.SuppressMinimap = True

        ScreenPos.X = e.X
        ScreenPos.Y = e.Y
        If e.Button = Windows.Forms.MouseButtons.Left Then
            MouseLeftDown = New clsViewInfo.clsMouseDown
            If IsViewPosOverMinimap(ScreenPos) Then
                MouseLeftDown.OverMinimap = New clsViewInfo.clsMouseDown.clsOverMinimap
                MouseLeftDown.OverMinimap.DownPos = ScreenPos
                Dim Pos As New sXY_int(CInt(Int(ScreenPos.X * Tiles_Per_Minimap_Pixel)), CInt(Int(ScreenPos.Y * Tiles_Per_Minimap_Pixel)))
                Map.TileNumClampToMap(Pos)
                LookAtTile(Pos)
            Else
                Dim MouseOverTerrain As clsMouseOver.clsOverTerrain = GetMouseOverTerrain()
                If MouseOverTerrain IsNot Nothing Then
                    MouseLeftDown.OverTerrain = New clsMouseDown.clsOverTerrain
                    MouseLeftDown.OverTerrain.DownPos = MouseOverTerrain.Pos
                    If Tool Is Tools.ObjectSelect Then
                        If KeyboardProfile.Active(Control_Picker) Then
                            If MouseOverTerrain.Units.Count > 0 Then
                                If MouseOverTerrain.Units.Count = 1 Then
                                    frmMainInstance.ObjectPicker(MouseOverTerrain.Units.Item(0).Type)
                                Else
                                    MapView.ListSelectBegin(True)
                                End If
                            End If
                        ElseIf KeyboardProfile.Active(Control_ScriptPosition) Then
                            Dim NewPosition As clsMap.clsScriptPosition = clsMap.clsScriptPosition.Create(Map)
                            If NewPosition IsNot Nothing Then
                                NewPosition.PosX = MouseLeftDown.OverTerrain.DownPos.Horizontal.X
                                NewPosition.PosY = MouseLeftDown.OverTerrain.DownPos.Horizontal.Y
                                frmMainInstance.ScriptMarkerLists_Update()
                            End If
                        Else
                            If Not KeyboardProfile.Active(Control_Unit_Multiselect) Then
                                Map.SelectedUnits.Clear()
                            End If
                            frmMainInstance.SelectedObject_Changed()
                            Map.Unit_Selected_Area_VertexA = New clsXY_int(MouseOverTerrain.Vertex.Normal)
                            MapView.DrawViewLater()
                        End If
                    ElseIf Tool Is Tools.TerrainBrush Then
                        If Map.Tileset IsNot Nothing Then
                            If KeyboardProfile.Active(Control_Picker) Then
                                frmMainInstance.TerrainPicker()
                            Else
                                Apply_Terrain()
                                If frmMainInstance.cbxAutoTexSetHeight.Checked Then
                                    Apply_Height_Set(TerrainBrush, frmMainInstance.HeightSetPalette(frmMainInstance.tabHeightSetL.SelectedIndex))
                                End If
                            End If
                        End If
                    ElseIf Tool Is Tools.HeightSetBrush Then
                        If KeyboardProfile.Active(Control_Picker) Then
                            frmMainInstance.HeightPickerL()
                        Else
                            Apply_Height_Set(HeightBrush, frmMainInstance.HeightSetPalette(frmMainInstance.tabHeightSetL.SelectedIndex))
                        End If
                    ElseIf Tool Is Tools.TextureBrush Then
                        If Map.Tileset IsNot Nothing Then
                            If KeyboardProfile.Active(Control_Picker) Then
                                frmMainInstance.TexturePicker()
                            Else
                                Apply_Texture()
                            End If
                        End If
                    ElseIf Tool Is Tools.CliffTriangle Then
                        Apply_CliffTriangle(False)
                    ElseIf Tool Is Tools.CliffBrush Then
                        Apply_Cliff()
                    ElseIf Tool Is Tools.CliffRemove Then
                        Apply_Cliff_Remove()
                    ElseIf Tool Is Tools.TerrainFill Then
                        If Map.Tileset IsNot Nothing Then
                            If KeyboardProfile.Active(Control_Picker) Then
                                frmMainInstance.TerrainPicker()
                            Else
                                Apply_Terrain_Fill(frmMainInstance.FillCliffAction, frmMainInstance.cbxFillInside.Checked)
                                MapView.DrawViewLater()
                            End If
                        End If
                    ElseIf Tool Is Tools.RoadPlace Then
                        If Map.Tileset IsNot Nothing Then
                            Apply_Road()
                        End If
                    ElseIf Tool Is Tools.RoadLines Then
                        If Map.Tileset IsNot Nothing Then
                            Apply_Road_Line_Selection()
                        End If
                    ElseIf Tool Is Tools.RoadRemove Then
                        Apply_Road_Remove()
                    ElseIf Tool Is Tools.ObjectPlace Then
                        If frmMainInstance.SingleSelectedObjectType IsNot Nothing And Map.SelectedUnitGroup IsNot Nothing Then
                            Dim objectCreator As New clsMap.clsUnitCreate
                            Map.SetObjectCreatorDefaults(objectCreator)
                            objectCreator.Horizontal = MouseOverTerrain.Pos.Horizontal
                            objectCreator.Perform()
                            Map.UndoStepCreate("Place Object")
                            Map.Update()
                            Map.MinimapMakeLater()
                            MapView.DrawViewLater()
                        End If
                    ElseIf Tool Is Tools.ObjectLines Then
                        ApplyObjectLine()
                    ElseIf Tool Is Tools.TerrainSelect Then
                        If Map.Selected_Area_VertexA Is Nothing Then
                            Map.Selected_Area_VertexA = New clsXY_int(MouseOverTerrain.Vertex.Normal)
                            MapView.DrawViewLater()
                        ElseIf Map.Selected_Area_VertexB Is Nothing Then
                            Map.Selected_Area_VertexB = New clsXY_int(MouseOverTerrain.Vertex.Normal)
                            MapView.DrawViewLater()
                        Else
                            Map.Selected_Area_VertexA = Nothing
                            Map.Selected_Area_VertexB = Nothing
                            MapView.DrawViewLater()
                        End If
                    ElseIf Tool Is Tools.Gateways Then
                        Apply_Gateway()
                    End If
                ElseIf Tool Is Tools.ObjectSelect Then
                    Map.SelectedUnits.Clear()
                    frmMainInstance.SelectedObject_Changed()
                End If
            End If
        ElseIf e.Button = Windows.Forms.MouseButtons.Right Then
            MouseRightDown = New clsMouseDown
            If IsViewPosOverMinimap(ScreenPos) Then
                MouseRightDown.OverMinimap = New clsMouseDown.clsOverMinimap
                MouseRightDown.OverMinimap.DownPos = ScreenPos
            Else
                Dim MouseOverTerrain As clsMouseOver.clsOverTerrain = GetMouseOverTerrain()
                If MouseOverTerrain IsNot Nothing Then
                    MouseRightDown.OverTerrain = New clsMouseDown.clsOverTerrain
                    MouseRightDown.OverTerrain.DownPos = MouseOverTerrain.Pos
                End If
            End If
            If Tool Is Tools.RoadLines Or Tool Is Tools.ObjectLines Then
                Map.Selected_Tile_A = Nothing
                MapView.DrawViewLater()
            ElseIf Tool Is Tools.TerrainSelect Then
                Map.Selected_Area_VertexA = Nothing
                Map.Selected_Area_VertexB = Nothing
                MapView.DrawViewLater()
            ElseIf Tool Is Tools.CliffTriangle Then
                Apply_CliffTriangle(True)
            ElseIf Tool Is Tools.Gateways Then
                Map.Selected_Tile_A = Nothing
                Map.Selected_Tile_B = Nothing
                MapView.DrawViewLater()
            ElseIf Tool Is Tools.HeightSetBrush Then
                If KeyboardProfile.Active(Control_Picker) Then
                    frmMainInstance.HeightPickerR()
                Else
                    Apply_Height_Set(HeightBrush, frmMainInstance.HeightSetPalette(frmMainInstance.tabHeightSetR.SelectedIndex))
                End If
            End If
        End If
    End Sub

    Public Sub TimedActions(Zoom As Double, Move As Double, Pan As Double, Roll As Double, OrbitRate As Double)
        Dim XYZ_dbl As Matrix3D.XYZ_dbl
        Dim PanRate As Double = Pan * FieldOfViewY
        Dim AnglePY As Matrix3D.AnglePY
        Dim matrixA As New Matrix3D.Matrix3D
        Dim matrixB As New Matrix3D.Matrix3D
        Dim ViewAngleChange As Matrix3D.XYZ_dbl
        Dim ViewPosChangeXYZ As sXYZ_int
        Dim AngleChanged As Boolean

        Move *= FOVMultiplier * (MapView.GLSize.X + MapView.GLSize.Y) * Math.Max(Math.Abs(ViewPos.Y), 512.0#)

        If KeyboardProfile.Active(Control_View_Zoom_In) Then
            FOV_Scale_2E_Change(-Zoom)
        End If
        If KeyboardProfile.Active(Control_View_Zoom_Out) Then
            FOV_Scale_2E_Change(Zoom)
        End If

        If ViewMoveType = enumView_Move_Type.Free Then
            ViewPosChangeXYZ.X = 0
            ViewPosChangeXYZ.Y = 0
            ViewPosChangeXYZ.Z = 0
            If KeyboardProfile.Active(Control_View_Move_Forward) Then
                Matrix3D.VectorForwardsRotationByMatrix(ViewAngleMatrix, Move, XYZ_dbl)
                ViewPosChangeXYZ.Add_dbl(XYZ_dbl)
            End If
            If KeyboardProfile.Active(Control_View_Move_Backward) Then
                Matrix3D.VectorBackwardsRotationByMatrix(ViewAngleMatrix, Move, XYZ_dbl)
                ViewPosChangeXYZ.Add_dbl(XYZ_dbl)
            End If
            If KeyboardProfile.Active(Control_View_Move_Left) Then
                Matrix3D.VectorLeftRotationByMatrix(ViewAngleMatrix, Move, XYZ_dbl)
                ViewPosChangeXYZ.Add_dbl(XYZ_dbl)
            End If
            If KeyboardProfile.Active(Control_View_Move_Right) Then
                Matrix3D.VectorRightRotationByMatrix(ViewAngleMatrix, Move, XYZ_dbl)
                ViewPosChangeXYZ.Add_dbl(XYZ_dbl)
            End If
            If KeyboardProfile.Active(Control_View_Move_Up) Then
                Matrix3D.VectorUpRotationByMatrix(ViewAngleMatrix, Move, XYZ_dbl)
                ViewPosChangeXYZ.Add_dbl(XYZ_dbl)
            End If
            If KeyboardProfile.Active(Control_View_Move_Down) Then
                Matrix3D.VectorDownRotationByMatrix(ViewAngleMatrix, Move, XYZ_dbl)
                ViewPosChangeXYZ.Add_dbl(XYZ_dbl)
            End If

            ViewAngleChange.X = 0.0#
            ViewAngleChange.Y = 0.0#
            ViewAngleChange.Z = 0.0#
            If KeyboardProfile.Active(Control_View_Left) Then
                Matrix3D.VectorForwardsRotationByMatrix(ViewAngleMatrix, Roll, XYZ_dbl)
                ViewAngleChange += XYZ_dbl
            End If
            If KeyboardProfile.Active(Control_View_Right) Then
                Matrix3D.VectorBackwardsRotationByMatrix(ViewAngleMatrix, Roll, XYZ_dbl)
                ViewAngleChange += XYZ_dbl
            End If
            If KeyboardProfile.Active(Control_View_Backward) Then
                Matrix3D.VectorLeftRotationByMatrix(ViewAngleMatrix, PanRate, XYZ_dbl)
                ViewAngleChange += XYZ_dbl
            End If
            If KeyboardProfile.Active(Control_View_Forward) Then
                Matrix3D.VectorRightRotationByMatrix(ViewAngleMatrix, PanRate, XYZ_dbl)
                ViewAngleChange += XYZ_dbl
            End If
            If KeyboardProfile.Active(Control_View_Roll_Left) Then
                Matrix3D.VectorDownRotationByMatrix(ViewAngleMatrix, PanRate, XYZ_dbl)
                ViewAngleChange += XYZ_dbl
            End If
            If KeyboardProfile.Active(Control_View_Roll_Right) Then
                Matrix3D.VectorUpRotationByMatrix(ViewAngleMatrix, PanRate, XYZ_dbl)
                ViewAngleChange += XYZ_dbl
            End If

            If ViewPosChangeXYZ.X <> 0.0# Or ViewPosChangeXYZ.Y <> 0.0# Or ViewPosChangeXYZ.Z <> 0.0# Then
                ViewPosChange(ViewPosChangeXYZ)
            End If
            'do rotation
            If ViewAngleChange.X <> 0.0# Or ViewAngleChange.Y <> 0.0# Or ViewAngleChange.Z <> 0.0# Then
                Matrix3D.VectorToPY(ViewAngleChange, AnglePY)
                Matrix3D.MatrixSetToPY(matrixA, AnglePY)
                Matrix3D.MatrixRotationAroundAxis(ViewAngleMatrix, matrixA, ViewAngleChange.GetMagnitude, matrixB)
                ViewAngleSet_Rotate(matrixB)
            End If
        ElseIf ViewMoveType = enumView_Move_Type.RTS Then
            ViewPosChangeXYZ = New sXYZ_int

            Matrix3D.MatrixToPY(ViewAngleMatrix, AnglePY)
            Matrix3D.MatrixSetToYAngle(matrixA, AnglePY.Yaw)
            If KeyboardProfile.Active(Control_View_Move_Forward) Then
                Matrix3D.VectorForwardsRotationByMatrix(matrixA, Move, XYZ_dbl)
                ViewPosChangeXYZ.Add_dbl(XYZ_dbl)
            End If
            If KeyboardProfile.Active(Control_View_Move_Backward) Then
                Matrix3D.VectorBackwardsRotationByMatrix(matrixA, Move, XYZ_dbl)
                ViewPosChangeXYZ.Add_dbl(XYZ_dbl)
            End If
            If KeyboardProfile.Active(Control_View_Move_Left) Then
                Matrix3D.VectorLeftRotationByMatrix(matrixA, Move, XYZ_dbl)
                ViewPosChangeXYZ.Add_dbl(XYZ_dbl)
            End If
            If KeyboardProfile.Active(Control_View_Move_Right) Then
                Matrix3D.VectorRightRotationByMatrix(matrixA, Move, XYZ_dbl)
                ViewPosChangeXYZ.Add_dbl(XYZ_dbl)
            End If
            If KeyboardProfile.Active(Control_View_Move_Up) Then
                ViewPosChangeXYZ.Y += CInt(Move)
            End If
            If KeyboardProfile.Active(Control_View_Move_Down) Then
                ViewPosChangeXYZ.Y -= CInt(Move)
            End If

            AngleChanged = False

            If RTSOrbit Then
                If KeyboardProfile.Active(Control_View_Forward) Then
                    AnglePY.Pitch = Clamp_dbl(AnglePY.Pitch + OrbitRate, -RadOf90Deg + 0.03125# * RadOf1Deg, RadOf90Deg - 0.03125# * RadOf1Deg)
                    AngleChanged = True
                End If
                If KeyboardProfile.Active(Control_View_Backward) Then
                    AnglePY.Pitch = Clamp_dbl(AnglePY.Pitch - OrbitRate, -RadOf90Deg + 0.03125# * RadOf1Deg, RadOf90Deg - 0.03125# * RadOf1Deg)
                    AngleChanged = True
                End If
                If KeyboardProfile.Active(Control_View_Left) Then
                    AnglePY.Yaw = AngleClamp(AnglePY.Yaw + OrbitRate)
                    AngleChanged = True
                End If
                If KeyboardProfile.Active(Control_View_Right) Then
                    AnglePY.Yaw = AngleClamp(AnglePY.Yaw - OrbitRate)
                    AngleChanged = True
                End If
            Else
                If KeyboardProfile.Active(Control_View_Forward) Then
                    AnglePY.Pitch = Clamp_dbl(AnglePY.Pitch - OrbitRate, -RadOf90Deg + 0.03125# * RadOf1Deg, RadOf90Deg - 0.03125# * RadOf1Deg)
                    AngleChanged = True
                End If
                If KeyboardProfile.Active(Control_View_Backward) Then
                    AnglePY.Pitch = Clamp_dbl(AnglePY.Pitch + OrbitRate, -RadOf90Deg + 0.03125# * RadOf1Deg, RadOf90Deg - 0.03125# * RadOf1Deg)
                    AngleChanged = True
                End If
                If KeyboardProfile.Active(Control_View_Left) Then
                    AnglePY.Yaw = AngleClamp(AnglePY.Yaw - OrbitRate)
                    AngleChanged = True
                End If
                If KeyboardProfile.Active(Control_View_Right) Then
                    AnglePY.Yaw = AngleClamp(AnglePY.Yaw + OrbitRate)
                    AngleChanged = True
                End If
            End If

            'Dim HeightChange As Double
            'HeightChange = Map.Terrain_Height_Get(view.View_Pos.X + ViewPosChange.X, view.View_Pos.Z + ViewPosChange.Z) - Map.Terrain_Height_Get(view.View_Pos.X, view.View_Pos.Z)

            'ViewPosChange.Y = ViewPosChange.Y + HeightChange

            If ViewPosChangeXYZ.X <> 0.0# Or ViewPosChangeXYZ.Y <> 0.0# Or ViewPosChangeXYZ.Z <> 0.0# Then
                ViewPosChange(ViewPosChangeXYZ)
            End If
            If AngleChanged Then
                Matrix3D.MatrixSetToPY(matrixA, AnglePY)
                ViewAngleSet_Rotate(matrixA)
            End If
        End If
    End Sub

    Public Sub TimedTools()

        If Tool Is Tools.HeightSmoothBrush Then
            If GetMouseOverTerrain() IsNot Nothing Then
                If GetMouseLeftDownOverTerrain() IsNot Nothing Then
                    Dim dblTemp As Double
                    If Not InvariantParse_dbl(frmMainInstance.txtSmoothRate.Text, dblTemp) Then
                        Exit Sub
                    End If
                    Apply_HeightSmoothing(Clamp_dbl(dblTemp * frmMainInstance.tmrTool.Interval / 1000.0#, 0.0#, 1.0#))
                End If
            End If
        ElseIf Tool Is Tools.HeightChangeBrush Then
            If GetMouseOverTerrain() IsNot Nothing Then
                Dim dblTemp As Double
                If Not InvariantParse_dbl(frmMainInstance.txtHeightChangeRate.Text, dblTemp) Then
                    Exit Sub
                End If
                If GetMouseLeftDownOverTerrain() IsNot Nothing Then
                    Apply_Height_Change(Clamp_dbl(dblTemp, -255.0#, 255.0#))
                ElseIf GetMouseRightDownOverTerrain() IsNot Nothing Then
                    Apply_Height_Change(Clamp_dbl(-dblTemp, -255.0#, 255.0#))
                End If
            End If
        End If
    End Sub

    Public Sub ApplyObjectLine()

        If frmMainInstance.SingleSelectedObjectType IsNot Nothing And Map.SelectedUnitGroup IsNot Nothing Then
            Dim MouseOverTerrian As clsMouseOver.clsOverTerrain = GetMouseOverTerrain()

            If MouseOverTerrian Is Nothing Then
                Exit Sub
            End If

            Dim Num As Integer
            Dim A As Integer
            Dim B As Integer
            Dim Tile As sXY_int = MouseOverTerrian.Tile.Normal

            If Map.Selected_Tile_A IsNot Nothing Then
                If Tile.X = Map.Selected_Tile_A.X Then
                    If Tile.Y <= Map.Selected_Tile_A.Y Then
                        A = Tile.Y
                        B = Map.Selected_Tile_A.Y
                    Else
                        A = Map.Selected_Tile_A.Y
                        B = Tile.Y
                    End If
                    Dim objectCreator As New clsMap.clsUnitCreate
                    Map.SetObjectCreatorDefaults(objectCreator)
                    For Num = A To B
                        objectCreator.Horizontal.X = CInt((Tile.X + 0.5#) * TerrainGridSpacing)
                        objectCreator.Horizontal.Y = CInt((Num + 0.5#) * TerrainGridSpacing)
                        objectCreator.Perform()
                    Next

                    Map.UndoStepCreate("Object Line")
                    Map.Update()
                    Map.MinimapMakeLater()
                    Map.Selected_Tile_A = Nothing
                    MapView.DrawViewLater()
                ElseIf Tile.Y = Map.Selected_Tile_A.Y Then
                    If Tile.X <= Map.Selected_Tile_A.X Then
                        A = Tile.X
                        B = Map.Selected_Tile_A.X
                    Else
                        A = Map.Selected_Tile_A.X
                        B = Tile.X
                    End If
                    Dim objectCreator As New clsMap.clsUnitCreate
                    Map.SetObjectCreatorDefaults(objectCreator)
                    For Num = A To B
                        objectCreator.Horizontal.X = CInt((Num + 0.5#) * TerrainGridSpacing)
                        objectCreator.Horizontal.Y = CInt((Tile.Y + 0.5#) * TerrainGridSpacing)
                        objectCreator.Perform()
                    Next

                    Map.UndoStepCreate("Object Line")
                    Map.Update()
                    Map.MinimapMakeLater()
                    Map.Selected_Tile_A = Nothing
                    MapView.DrawViewLater()
                Else

                End If
            Else
                Map.Selected_Tile_A = New clsXY_int(Tile)
            End If
        End If
    End Sub
End Class
