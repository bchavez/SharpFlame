Imports OpenTK.Graphics.OpenGL

Partial Public Class clsMap

    Public MustInherit Class clsDrawTile

        Public Map As clsMap
        Public TileX As Integer
        Public TileY As Integer

        Public MustOverride Sub Perform()
    End Class

    Public Class clsDrawTileOld
        Inherits clsDrawTile

        Public Overrides Sub Perform()
            Dim Terrain As clsTerrain = Map.Terrain
            Dim Tileset As clsTileset = Map.Tileset
            Dim TileTerrainHeight(3) As Double
            Dim Vertex0 As sXYZ_sng
            Dim Vertex1 As sXYZ_sng
            Dim Vertex2 As sXYZ_sng
            Dim Vertex3 As sXYZ_sng
            Dim Normal0 As sXYZ_sng
            Dim Normal1 As sXYZ_sng
            Dim Normal2 As sXYZ_sng
            Dim Normal3 As sXYZ_sng
            Dim TexCoord0 As sXY_sng
            Dim TexCoord1 As sXY_sng
            Dim TexCoord2 As sXY_sng
            Dim TexCoord3 As sXY_sng
            Dim A As Integer

            If Terrain.Tiles(TileX, TileY).Texture.TextureNum < 0 Then
                GL.BindTexture(TextureTarget.Texture2D, GLTexture_NoTile)
            ElseIf Tileset Is Nothing Then
                GL.BindTexture(TextureTarget.Texture2D, GLTexture_OverflowTile)
            ElseIf Terrain.Tiles(TileX, TileY).Texture.TextureNum < Tileset.TileCount Then
                A = Tileset.Tiles(Terrain.Tiles(TileX, TileY).Texture.TextureNum).MapView_GL_Texture_Num
                If A = 0 Then
                    GL.BindTexture(TextureTarget.Texture2D, GLTexture_OverflowTile)
                Else
                    GL.BindTexture(TextureTarget.Texture2D, A)
                End If
            Else
                GL.BindTexture(TextureTarget.Texture2D, GLTexture_OverflowTile)
            End If
            GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, TextureEnvMode.Modulate)

            TileTerrainHeight(0) = Terrain.Vertices(TileX, TileY).Height
            TileTerrainHeight(1) = Terrain.Vertices(TileX + 1, TileY).Height
            TileTerrainHeight(2) = Terrain.Vertices(TileX, TileY + 1).Height
            TileTerrainHeight(3) = Terrain.Vertices(TileX + 1, TileY + 1).Height

            GetTileRotatedTexCoords(Terrain.Tiles(TileX, TileY).Texture.Orientation, TexCoord0, TexCoord1, TexCoord2, TexCoord3)

            Vertex0.X = CSng(TileX * TerrainGridSpacing)
            Vertex0.Y = CSng(TileTerrainHeight(0) * Map.HeightMultiplier)
            Vertex0.Z = CSng(-TileY * TerrainGridSpacing)
            Vertex1.X = CSng((TileX + 1) * TerrainGridSpacing)
            Vertex1.Y = CSng(TileTerrainHeight(1) * Map.HeightMultiplier)
            Vertex1.Z = CSng(-TileY * TerrainGridSpacing)
            Vertex2.X = CSng(TileX * TerrainGridSpacing)
            Vertex2.Y = CSng(TileTerrainHeight(2) * Map.HeightMultiplier)
            Vertex2.Z = CSng(-(TileY + 1) * TerrainGridSpacing)
            Vertex3.X = CSng((TileX + 1) * TerrainGridSpacing)
            Vertex3.Y = CSng(TileTerrainHeight(3) * Map.HeightMultiplier)
            Vertex3.Z = CSng(-(TileY + 1) * TerrainGridSpacing)

            Normal0 = Map.TerrainVertexNormalCalc(TileX, TileY)
            Normal1 = Map.TerrainVertexNormalCalc(TileX + 1, TileY)
            Normal2 = Map.TerrainVertexNormalCalc(TileX, TileY + 1)
            Normal3 = Map.TerrainVertexNormalCalc(TileX + 1, TileY + 1)

            GL.Begin(BeginMode.Triangles)
            If Terrain.Tiles(TileX, TileY).Tri Then
                GL.Normal3(Normal0.X, Normal0.Y, -Normal0.Z)
                GL.TexCoord2(TexCoord0.X, TexCoord0.Y)
                GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)
                GL.Normal3(Normal2.X, Normal2.Y, -Normal2.Z)
                GL.TexCoord2(TexCoord2.X, TexCoord2.Y)
                GL.Vertex3(Vertex2.X, Vertex2.Y, -Vertex2.Z)
                GL.Normal3(Normal1.X, Normal1.Y, -Normal1.Z)
                GL.TexCoord2(TexCoord1.X, TexCoord1.Y)
                GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)

                GL.Normal3(Normal1.X, Normal1.Y, -Normal1.Z)
                GL.TexCoord2(TexCoord1.X, TexCoord1.Y)
                GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)
                GL.Normal3(Normal2.X, Normal2.Y, -Normal2.Z)
                GL.TexCoord2(TexCoord2.X, TexCoord2.Y)
                GL.Vertex3(Vertex2.X, Vertex2.Y, -Vertex2.Z)
                GL.Normal3(Normal3.X, Normal3.Y, -Normal3.Z)
                GL.TexCoord2(TexCoord3.X, TexCoord3.Y)
                GL.Vertex3(Vertex3.X, Vertex3.Y, -Vertex3.Z)
            Else
                GL.Normal3(Normal0.X, Normal0.Y, -Normal0.Z)
                GL.TexCoord2(TexCoord0.X, TexCoord0.Y)
                GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)
                GL.Normal3(Normal2.X, Normal2.Y, -Normal2.Z)
                GL.TexCoord2(TexCoord2.X, TexCoord2.Y)
                GL.Vertex3(Vertex2.X, Vertex2.Y, -Vertex2.Z)
                GL.Normal3(Normal3.X, Normal3.Y, -Normal3.Z)
                GL.TexCoord2(TexCoord3.X, TexCoord3.Y)
                GL.Vertex3(Vertex3.X, Vertex3.Y, -Vertex3.Z)

                GL.Normal3(Normal0.X, Normal0.Y, -Normal0.Z)
                GL.TexCoord2(TexCoord0.X, TexCoord0.Y)
                GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)
                GL.Normal3(Normal3.X, Normal3.Y, -Normal3.Z)
                GL.TexCoord2(TexCoord3.X, TexCoord3.Y)
                GL.Vertex3(Vertex3.X, Vertex3.Y, -Vertex3.Z)
                GL.Normal3(Normal1.X, Normal1.Y, -Normal1.Z)
                GL.TexCoord2(TexCoord1.X, TexCoord1.Y)
                GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)
            End If
            GL.End()
        End Sub
    End Class

    'Public Class clsBufferData

    '    Public Structure sVertex
    '        Public Pos As sXYZ_sng
    '        Public Normal As sXYZ_sng
    '        Public TexCoord As sXY_sng
    '        Public RGBA As sRGBA_sng
    '        Private PaddingA As Integer
    '        Private PaddingB As Integer
    '        Private PaddingC As Integer
    '        Private PaddingD As Integer
    '    End Structure
    '    Public Vertices() As sVertex

    '    Public Position As Integer = 0

    '    Public Sub SendData(GLBufferNum As Integer)

    '        ReDim Preserve Vertices(Position - 1)

    '        GL.BindBuffer(BufferTarget.ArrayBuffer, GLBufferNum)
    '        GL.BufferData(Of sVertex)(BufferTarget.ArrayBuffer, CType(Position * 64, IntPtr), Vertices, BufferUsageHint.DynamicDraw)
    '        GL.BindBuffer(BufferTarget.ArrayBuffer, 0)

    '        Vertices = Nothing
    '    End Sub
    'End Class
End Class
