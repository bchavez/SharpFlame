Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL

Public Class ctrlMapView

    Private _Owner As frmMain

    Public DrawPending As Boolean

    Public GLSize As sXY_int
    Public GLSize_XPerY As Single

    Public DrawView_Enabled As Boolean = False

    Private GLInitializeDelayTimer As Timer
    Public IsGLInitialized As Boolean = False

    Private WithEvents tmrDraw As Timer
    Private WithEvents tmrDrawDelay As Timer

    Public OpenGLControl As OpenTK.GLControl

    Public Sub New(Owner As frmMain)

        _Owner = Owner

        InitializeComponent()

        ListSelect = New ContextMenuStrip
        UndoMessageTimer = New Timer

        OpenGLControl = OpenGL1
        pnlDraw.Controls.Add(OpenGLControl)

        GLInitializeDelayTimer = New Timer
        GLInitializeDelayTimer.Interval = 50
        AddHandler GLInitializeDelayTimer.Tick, AddressOf GLInitialize
        GLInitializeDelayTimer.Enabled = True

        tmrDraw = New Timer
        tmrDraw.Interval = 1

        tmrDrawDelay = New Timer
        tmrDrawDelay.Interval = 30

        UndoMessageTimer.Interval = 4000
    End Sub

    Public Sub ResizeOpenGL()

        If OpenGLControl.Context Is Nothing Then
            Exit Sub
        End If

        OpenGLControl.Width = pnlDraw.Width
        OpenGLControl.Height = pnlDraw.Height
    End Sub

    Public Sub DrawView_SetEnabled(Value As Boolean)

        If Value Then
            If Not DrawView_Enabled Then
                DrawView_Enabled = True
                DrawViewLater()
            End If
        Else
            tmrDraw.Enabled = False
            DrawView_Enabled = False
        End If
    End Sub

    Public Sub DrawViewLater()

        DrawPending = True
        If Not tmrDrawDelay.Enabled Then
            tmrDraw.Enabled = True
        End If
    End Sub

    Private Sub tmrDraw_Tick(sender As System.Object, e As System.EventArgs) Handles tmrDraw.Tick

        tmrDraw.Enabled = False
        If DrawPending Then
            DrawView()
            DrawPending = False
            tmrDrawDelay.Enabled = True
        End If
    End Sub

    Private Sub GLInitialize(sender As Object, e As EventArgs)

        If OpenGLControl.Context Is Nothing Then
            Exit Sub
        End If

        GLInitializeDelayTimer.Enabled = False
        RemoveHandler GLInitializeDelayTimer.Tick, AddressOf GLInitialize
        GLInitializeDelayTimer.Dispose()
        GLInitializeDelayTimer = Nothing

        ResizeOpenGL()

        AddHandler OpenGLControl.MouseDown, AddressOf OpenGL_MouseDown
        AddHandler OpenGLControl.MouseUp, AddressOf OpenGL_MouseUp
        AddHandler OpenGLControl.MouseWheel, AddressOf OpenGL_MouseWheel
        AddHandler OpenGLControl.MouseMove, AddressOf OpenGL_MouseMove
        AddHandler OpenGLControl.MouseEnter, AddressOf OpenGL_MouseEnter
        AddHandler OpenGLControl.MouseLeave, AddressOf OpenGL_MouseLeave
        AddHandler OpenGLControl.Resize, AddressOf OpenGL_Resize
        AddHandler OpenGLControl.Leave, AddressOf OpenGL_LostFocus
        AddHandler OpenGLControl.PreviewKeyDown, AddressOf OpenGL_KeyDown
        AddHandler OpenGLControl.KeyUp, AddressOf OpenGL_KeyUp

        If GraphicsContext.CurrentContext IsNot OpenGLControl.Context Then
            OpenGLControl.MakeCurrent()
        End If

        GL.PixelStore(PixelStoreParameter.PackAlignment, 1)
        GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1)
        GL.ClearColor(0.0F, 0.0F, 0.0F, 1.0F)
        GL.Clear(ClearBufferMask.ColorBufferBit)
        GL.ShadeModel(ShadingModel.Smooth)
        GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest)
        GL.Enable(EnableCap.DepthTest)

        Dim ambient(3) As Single
        Dim specular(3) As Single
        Dim diffuse(3) As Single

        ambient(0) = 0.333333343F
        ambient(1) = 0.333333343F
        ambient(2) = 0.333333343F
        ambient(3) = 1.0F
        specular(0) = 0.6666667F
        specular(1) = 0.6666667F
        specular(2) = 0.6666667F
        specular(3) = 1.0F
        diffuse(0) = 0.75F
        diffuse(1) = 0.75F
        diffuse(2) = 0.75F
        diffuse(3) = 1.0F
        GL.Light(LightName.Light0, LightParameter.Diffuse, diffuse)
        GL.Light(LightName.Light0, LightParameter.Specular, specular)
        GL.Light(LightName.Light0, LightParameter.Ambient, ambient)

        ambient(0) = 0.25F
        ambient(1) = 0.25F
        ambient(2) = 0.25F
        ambient(3) = 1.0F
        specular(0) = 0.5F
        specular(1) = 0.5F
        specular(2) = 0.5F
        specular(3) = 1.0F
        diffuse(0) = 0.5625F
        diffuse(1) = 0.5625F
        diffuse(2) = 0.5625F
        diffuse(3) = 1.0F
        GL.Light(LightName.Light1, LightParameter.Diffuse, diffuse)
        GL.Light(LightName.Light1, LightParameter.Specular, specular)
        GL.Light(LightName.Light1, LightParameter.Ambient, ambient)

        Dim mat_diffuse(3) As Single
        Dim mat_specular(3) As Single
        Dim mat_ambient(3) As Single
        Dim mat_shininess(0) As Single

        mat_specular(0) = 0.0F
        mat_specular(1) = 0.0F
        mat_specular(2) = 0.0F
        mat_specular(3) = 0.0F
        mat_ambient(0) = 1.0F
        mat_ambient(1) = 1.0F
        mat_ambient(2) = 1.0F
        mat_ambient(3) = 1.0F
        mat_diffuse(0) = 1.0F
        mat_diffuse(1) = 1.0F
        mat_diffuse(2) = 1.0F
        mat_diffuse(3) = 1.0F
        mat_shininess(0) = 0.0F

        GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Ambient, mat_ambient)
        GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Specular, mat_specular)
        GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Diffuse, mat_diffuse)
        GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Shininess, mat_shininess)

        IsGLInitialized = True
    End Sub

    Public Sub Viewport_Resize()

        If Not ProgramInitialized Then
            Exit Sub
        End If

        If GraphicsContext.CurrentContext IsNot OpenGLControl.Context Then
            OpenGLControl.MakeCurrent()
        End If
        GL.Viewport(0, 0, GLSize.X, GLSize.Y)

        GL.Clear(ClearBufferMask.ColorBufferBit)
        GL.Flush()
        OpenGLControl.SwapBuffers()
        Refresh()

        DrawViewLater()
    End Sub

    Private Sub DrawView()

        If Not (DrawView_Enabled And IsGLInitialized) Then
            Exit Sub
        End If

        If GraphicsContext.CurrentContext IsNot OpenGLControl.Context Then
            OpenGLControl.MakeCurrent()
        End If

        Dim Map As clsMap = MainMap
        Dim BGColour As sRGB_sng

        If Map Is Nothing Then
            BGColour.Red = 0.5F
            BGColour.Green = 0.5F
            BGColour.Blue = 0.5F
        ElseIf Map.Tileset Is Nothing Then
            BGColour.Red = 0.5F
            BGColour.Green = 0.5F
            BGColour.Blue = 0.5F
        Else
            BGColour = Map.Tileset.BGColour
        End If

        GL.ClearColor(BGColour.Red, BGColour.Green, BGColour.Blue, 1.0F)
        GL.Clear(ClearBufferMask.ColorBufferBit Or ClearBufferMask.DepthBufferBit)

        If Map IsNot Nothing Then
            Map.GLDraw()
        End If

        GL.Flush()
        OpenGLControl.SwapBuffers()

        Refresh()
    End Sub

    Public Sub OpenGL_MouseMove(sender As Object, e As System.Windows.Forms.MouseEventArgs)
        Dim Map As clsMap = MainMap

        If Map Is Nothing Then
            Exit Sub
        End If

        Map.ViewInfo.Map.ViewInfo.MouseOver = New clsViewInfo.clsMouseOver
        Map.ViewInfo.MouseOver.ScreenPos.X = e.X
        Map.ViewInfo.MouseOver.ScreenPos.Y = e.Y

        Map.ViewInfo.MouseOver_Pos_Calc()
    End Sub

    Public Sub Pos_Display_Update()
        Dim Map As clsMap = MainMap
        Dim MouseOverTerrain As clsViewInfo.clsMouseOver.clsOverTerrain = Map.ViewInfo.GetMouseOverTerrain()

        If MouseOverTerrain Is Nothing Then
            lblTile.Text = ""
            lblVertex.Text = ""
            lblPos.Text = ""
        Else
            lblTile.Text = "Tile x:" & MouseOverTerrain.Tile.Normal.X & ", y:" & MouseOverTerrain.Tile.Normal.Y
            lblVertex.Text = "Vertex  x:" & MouseOverTerrain.Vertex.Normal.X & ", y:" & MouseOverTerrain.Vertex.Normal.Y & ", alt:" & map.Terrain.Vertices(MouseOverTerrain.Vertex.Normal.X, MouseOverTerrain.Vertex.Normal.Y).Height * map.HeightMultiplier & " (" & map.Terrain.Vertices(MouseOverTerrain.Vertex.Normal.X, MouseOverTerrain.Vertex.Normal.Y).Height & "x" & map.HeightMultiplier & ")"
            lblPos.Text = "Pos x:" & MouseOverTerrain.Pos.Horizontal.X & ", y:" & MouseOverTerrain.Pos.Horizontal.Y & ", alt:" & MouseOverTerrain.Pos.Altitude & ", slope: " & Math.Round(map.GetTerrainSlopeAngle(MouseOverTerrain.Pos.Horizontal) / RadOf1Deg * 10.0#) / 10.0# & "°"
        End If
    End Sub

    Public Sub OpenGL_LostFocus(eventSender As System.Object, eventArgs As System.EventArgs)
        Dim Map As clsMap = MainMap

        If Map Is Nothing Then
            Exit Sub
        End If

        Map.SuppressMinimap = False

        Map.ViewInfo.MouseOver = Nothing
        Map.ViewInfo.MouseLeftDown = Nothing
        Map.ViewInfo.MouseRightDown = Nothing

        ViewKeyDown_Clear()
    End Sub

    Private WithEvents ListSelect As ContextMenuStrip
    Private ListSelectIsPicker As Boolean
    Private ListSelectItems(-1) As ToolStripItem

    Private Sub ListSelect_Click(Sender As Object, e As ToolStripItemClickedEventArgs) Handles ListSelect.ItemClicked
        Dim Button As ToolStripItem = e.ClickedItem
        Dim Unit As clsMap.clsUnit = CType(Button.Tag, clsMap.clsUnit)

        If ListSelectIsPicker Then
            frmMainInstance.ObjectPicker(Unit.Type)
        Else
            If Unit.MapSelectedUnitLink.IsConnected Then
                Unit.MapDeselect()
            Else
                Unit.MapSelect()
            End If
            frmMainInstance.SelectedObject_Changed()
            DrawViewLater()
        End If
    End Sub

    Private Sub ListSelect_Close(sender As Object, e As ToolStripDropDownClosedEventArgs) Handles ListSelect.Closed
        Dim A As Integer

        For A = 0 To ListSelectItems.GetUpperBound(0)
            ListSelectItems(A).Tag = Nothing
            ListSelectItems(A).Dispose()
        Next
        ListSelect.Items.Clear()
        ReDim ListSelectItems(-1)

        ViewKeyDown_Clear()
    End Sub

    Private Sub OpenGL_MouseDown(sender As Object, e As MouseEventArgs)
        Dim Map As clsMap = MainMap

        If Map Is Nothing Then
            Exit Sub
        End If

        Map.ViewInfo.MouseDown(e)
    End Sub

    Private Sub OpenGL_KeyDown(sender As Object, e As PreviewKeyDownEventArgs)
        Dim Map As clsMap = MainMap

        If Map Is Nothing Then
            Exit Sub
        End If

        Dim matrixA As New Matrix3D.Matrix3D
        Dim MouseOverTerrain As clsViewInfo.clsMouseOver.clsOverTerrain = Map.ViewInfo.GetMouseOverTerrain()

        IsViewKeyDown.Keys(e.KeyCode) = True

        For Each control As clsOption(Of clsKeyboardControl) In Options_KeyboardControls.Options
            CType(KeyboardProfile.Value(control), clsKeyboardControl).KeysChanged(IsViewKeyDown)
        Next

        If KeyboardProfile.Active(Control_Undo) Then
            Dim Message As String
            If Map.UndoPosition > 0 Then
                Message = "Undid: " & Map.Undos.Item(Map.UndoPosition - 1).Name
                Dim MapMessage As New clsMap.clsMessage
                MapMessage.Text = Message
                Map.Messages.Add(MapMessage)
                Map.UndoPerform()
                DrawViewLater()
            Else
                Message = "Nothing to undo"
            End If
            DisplayUndoMessage(Message)
        End If
        If KeyboardProfile.Active(Control_Redo) Then
            Dim Message As String
            If Map.UndoPosition < Map.Undos.Count Then
                Message = "Redid: " & Map.Undos.Item(Map.UndoPosition).Name
                Dim MapMessage As New clsMap.clsMessage
                MapMessage.Text = Message
                Map.Messages.Add(MapMessage)
                Map.RedoPerform()
                DrawViewLater()
            Else
                Message = "Nothing to redo"
            End If
            DisplayUndoMessage(Message)
        End If
        If IsViewKeyDown.Keys(Keys.ControlKey) Then
            If e.KeyCode = Keys.D1 Then
                VisionRadius_2E = 6
            ElseIf e.KeyCode = Keys.D2 Then
                VisionRadius_2E = 7
            ElseIf e.KeyCode = Keys.D3 Then
                VisionRadius_2E = 8
            ElseIf e.KeyCode = Keys.D4 Then
                VisionRadius_2E = 9
            ElseIf e.KeyCode = Keys.D5 Then
                VisionRadius_2E = 10
            ElseIf e.KeyCode = Keys.D6 Then
                VisionRadius_2E = 11
            ElseIf e.KeyCode = Keys.D7 Then
                VisionRadius_2E = 12
            ElseIf e.KeyCode = Keys.D8 Then
                VisionRadius_2E = 13
            ElseIf e.KeyCode = Keys.D9 Then
                VisionRadius_2E = 14
            ElseIf e.KeyCode = Keys.D0 Then
                VisionRadius_2E = 15
            End If
            VisionRadius_2E_Changed()
        End If

        If KeyboardProfile.Active(Control_View_Move_Type) Then
            If ViewMoveType = enumView_Move_Type.Free Then
                ViewMoveType = enumView_Move_Type.RTS
            ElseIf ViewMoveType = enumView_Move_Type.RTS Then
                ViewMoveType = enumView_Move_Type.Free
            End If
        End If
        If KeyboardProfile.Active(Control_View_Rotate_Type) Then
            RTSOrbit = Not RTSOrbit
        End If
        If KeyboardProfile.Active(Control_View_Reset) Then
            Map.ViewInfo.FOV_Multiplier_Set(Settings.FOVDefault)
            If ViewMoveType = enumView_Move_Type.Free Then
                Matrix3D.MatrixSetToXAngle(matrixA, Math.Atan(2.0#))
                Map.ViewInfo.ViewAngleSet_Rotate(matrixA)
            ElseIf ViewMoveType = enumView_Move_Type.RTS Then
                Matrix3D.MatrixSetToXAngle(matrixA, Math.Atan(2.0#))
                Map.ViewInfo.ViewAngleSet_Rotate(matrixA)
            End If
        End If
        If KeyboardProfile.Active(Control_View_Textures) Then
            Draw_TileTextures = Not Draw_TileTextures
            DrawViewLater()
        End If
        If KeyboardProfile.Active(Control_View_Wireframe) Then
            Draw_TileWireframe = Not Draw_TileWireframe
            DrawViewLater()
        End If
        If KeyboardProfile.Active(Control_View_Units) Then
            Draw_Units = Not Draw_Units
            Dim X As Integer
            Dim Y As Integer
            Dim SectorNum As sXY_int
            Dim Unit As clsMap.clsUnit
            Dim Connection As clsMap.clsUnitSectorConnection
            For Y = 0 To Map.SectorCount.Y - 1
                For X = 0 To Map.SectorCount.X - 1
                    For Each Connection In Map.Sectors(X, Y).Units
                        Unit = Connection.Unit
                        If Unit.Type.Type = clsUnitType.enumType.PlayerStructure Then
                            If CType(Unit.Type, clsStructureType).StructureBasePlate IsNot Nothing Then
                                SectorNum.X = X
                                SectorNum.Y = Y
                                Map.SectorGraphicsChanges.Changed(SectorNum)
                                Exit For
                            End If
                        End If
                    Next
                Next
            Next
            Map.Update()
            DrawViewLater()
        End If
        If KeyboardProfile.Active(Control_View_ScriptMarkers) Then
            Draw_ScriptMarkers = Not Draw_ScriptMarkers
            DrawViewLater()
        End If
        If KeyboardProfile.Active(Control_View_Lighting) Then
            If Draw_Lighting = enumDrawLighting.Off Then
                Draw_Lighting = enumDrawLighting.Half
            ElseIf Draw_Lighting = enumDrawLighting.Half Then
                Draw_Lighting = enumDrawLighting.Normal
            ElseIf Draw_Lighting = enumDrawLighting.Normal Then
                Draw_Lighting = enumDrawLighting.Off
            End If
            DrawViewLater()
        End If
        If Tool Is Tools.TextureBrush Then
            If MouseOverTerrain IsNot Nothing Then
                If KeyboardProfile.Active(Control_Clockwise) Then
                    Map.ViewInfo.Apply_Texture_Clockwise()
                End If
                If KeyboardProfile.Active(Control_CounterClockwise) Then
                    Map.ViewInfo.Apply_Texture_CounterClockwise()
                End If
                If KeyboardProfile.Active(Control_Texture_Flip) Then
                    Map.ViewInfo.Apply_Texture_FlipX()
                End If
                If KeyboardProfile.Active(Control_Tri_Flip) Then
                    Map.ViewInfo.Apply_Tri_Flip()
                End If
            End If
        End If
        If Tool Is Tools.ObjectSelect Then
            If KeyboardProfile.Active(Control_Unit_Delete) Then
                If Map.SelectedUnits.Count > 0 Then
                    Dim Unit As clsMap.clsUnit
                    For Each Unit In Map.SelectedUnits.GetItemsAsSimpleList
                        Map.UnitRemoveStoreChange(Unit.MapLink.ArrayPosition)
                    Next
                    frmMainInstance.SelectedObject_Changed()
                    Map.UndoStepCreate("Object Deleted")
                    Map.Update()
                    Map.MinimapMakeLater()
                    DrawViewLater()
                End If
            End If
            If KeyboardProfile.Active(Control_Unit_Move) Then
                If MouseOverTerrain IsNot Nothing Then
                    If Map.SelectedUnits.Count > 0 Then
                        Dim Centre As Matrix3D.XY_dbl = CalcUnitsCentrePos(Map.SelectedUnits.GetItemsAsSimpleList)
                        Dim Offset As sXY_int
                        Offset.X = CInt(Math.Round((MouseOverTerrain.Pos.Horizontal.X - Centre.X) / TerrainGridSpacing)) * TerrainGridSpacing
                        Offset.Y = CInt(Math.Round((MouseOverTerrain.Pos.Horizontal.Y - Centre.Y) / TerrainGridSpacing)) * TerrainGridSpacing
                        Dim ObjectPosOffset As New clsMap.clsObjectPosOffset
                        ObjectPosOffset.Map = Map
                        ObjectPosOffset.Offset = Offset
                        Map.SelectedUnitsAction(ObjectPosOffset)

                        Map.UndoStepCreate("Objects Moved")
                        Map.Update()
                        Map.MinimapMakeLater()
                        frmMainInstance.SelectedObject_Changed()
                        DrawViewLater()
                    End If
                End If
            End If
            If KeyboardProfile.Active(Control_Clockwise) Then
                Dim ObjectRotationOffset As New clsMap.clsObjectRotationOffset
                ObjectRotationOffset.Map = Map
                ObjectRotationOffset.Offset = -90
                Map.SelectedUnitsAction(ObjectRotationOffset)
                Map.Update()
                frmMainInstance.SelectedObject_Changed()
                Map.UndoStepCreate("Object Rotated")
                DrawViewLater()
            End If
            If KeyboardProfile.Active(Control_CounterClockwise) Then
                Dim ObjectRotationOffset As New clsMap.clsObjectRotationOffset
                ObjectRotationOffset.Map = Map
                ObjectRotationOffset.Offset = 90
                Map.SelectedUnitsAction(ObjectRotationOffset)
                Map.Update()
                frmMainInstance.SelectedObject_Changed()
                Map.UndoStepCreate("Object Rotated")
                DrawViewLater()
            End If
        End If

        If KeyboardProfile.Active(Control_Deselect) Then
            Tool = Tools.ObjectSelect
            DrawViewLater()
        End If

        If KeyboardProfile.Active(Control_PreviousTool) Then
            Tool = PreviousTool
            DrawViewLater()
        End If
    End Sub

    Private Sub OpenGL_KeyUp(sender As Object, e As KeyEventArgs)

        IsViewKeyDown.Keys(e.KeyCode) = False

        For Each control As clsOption(Of clsKeyboardControl) In Options_KeyboardControls.Options
            CType(KeyboardProfile.Value(control), clsKeyboardControl).KeysChanged(IsViewKeyDown)
        Next
    End Sub

    Private Sub OpenGL_MouseUp(sender As Object, e As System.Windows.Forms.MouseEventArgs)
        Dim Map As clsMap = MainMap

        If Map Is Nothing Then
            Exit Sub
        End If

        Dim MouseOverTerrain As clsViewInfo.clsMouseOver.clsOverTerrain = Map.ViewInfo.GetMouseOverTerrain()

        Map.SuppressMinimap = False

        If e.Button = Windows.Forms.MouseButtons.Left Then
            If Map.ViewInfo.GetMouseLeftDownOverMinimap() IsNot Nothing Then

            Else
                If Tool Is Tools.TerrainBrush Then
                    Map.UndoStepCreate("Ground Painted")
                ElseIf Tool Is Tools.CliffTriangle Then
                    Map.UndoStepCreate("Cliff Triangles")
                ElseIf Tool Is Tools.CliffBrush Then
                    Map.UndoStepCreate("Cliff Brush")
                ElseIf Tool Is Tools.CliffRemove Then
                    Map.UndoStepCreate("Cliff Remove Brush")
                ElseIf Tool Is Tools.HeightChangeBrush Then
                    Map.UndoStepCreate("Height Change")
                ElseIf Tool Is Tools.HeightSetBrush Then
                    Map.UndoStepCreate("Height Set")
                ElseIf Tool Is Tools.HeightSmoothBrush Then
                    Map.UndoStepCreate("Height Smooth")
                ElseIf Tool Is Tools.TextureBrush Then
                    Map.UndoStepCreate("Texture")
                ElseIf Tool Is Tools.RoadRemove Then
                    Map.UndoStepCreate("Road Remove")
                ElseIf Tool Is Tools.ObjectSelect Then
                    If Map.Unit_Selected_Area_VertexA IsNot Nothing Then
                        If MouseOverTerrain IsNot Nothing Then
                            SelectUnits(Map.Unit_Selected_Area_VertexA.XY, MouseOverTerrain.Vertex.Normal)
                        End If
                        Map.Unit_Selected_Area_VertexA = Nothing
                    End If
                End If
            End If
            Map.ViewInfo.MouseLeftDown = Nothing
        ElseIf e.Button = Windows.Forms.MouseButtons.Right Then
            If Map.ViewInfo.GetMouseRightDownOverMinimap() IsNot Nothing Then

            Else
                If Tool Is Tools.HeightChangeBrush Then
                    Map.UndoStepCreate("Height Change")
                ElseIf Tool Is Tools.HeightSetBrush Then
                    Map.UndoStepCreate("Height Set")
                End If
            End If
            Map.ViewInfo.MouseRightDown = Nothing
        End If
    End Sub

    Private Sub SelectUnits(VertexA As sXY_int, VertexB As sXY_int)
        Dim Map As clsMap = MainMap
        Dim MouseOverTerrain As clsViewInfo.clsMouseOver.clsOverTerrain = Map.ViewInfo.GetMouseOverTerrain()
        Dim SectorNum As sXY_int
        Dim Unit As clsMap.clsUnit
        Dim SectorStart As sXY_int
        Dim SectorFinish As sXY_int
        Dim StartPos As sXY_int
        Dim FinishPos As sXY_int
        Dim StartVertex As sXY_int
        Dim FinishVertex As sXY_int

        If Math.Abs(VertexA.X - VertexB.X) <= 1 And _
          Math.Abs(VertexA.Y - VertexB.Y) <= 1 And _
          MouseOverTerrain IsNot Nothing Then
            If MouseOverTerrain.Units.Count > 0 Then
                If MouseOverTerrain.Units.Count = 1 Then
                    Unit = MouseOverTerrain.Units.Item(0)
                    If Unit.MapSelectedUnitLink.IsConnected Then
                        Unit.MapDeselect()
                    Else
                        Unit.MapSelect()
                    End If
                Else
                    ListSelectBegin(False)
                End If
            End If
        Else
            ReorderXY(VertexA, VertexB, StartVertex, FinishVertex)
            StartPos.X = StartVertex.X * TerrainGridSpacing
            StartPos.Y = StartVertex.Y * TerrainGridSpacing
            FinishPos.X = FinishVertex.X * TerrainGridSpacing
            FinishPos.Y = FinishVertex.Y * TerrainGridSpacing
            SectorStart.X = Math.Min(CInt(Int(StartVertex.X / SectorTileSize)), Map.SectorCount.X - 1)
            SectorStart.Y = Math.Min(CInt(Int(StartVertex.Y / SectorTileSize)), Map.SectorCount.Y - 1)
            SectorFinish.X = Math.Min(CInt(Int(FinishVertex.X / SectorTileSize)), Map.SectorCount.X - 1)
            SectorFinish.Y = Math.Min(CInt(Int(FinishVertex.Y / SectorTileSize)), Map.SectorCount.Y - 1)
            For SectorNum.Y = SectorStart.Y To SectorFinish.Y
                For SectorNum.X = SectorStart.X To SectorFinish.X
                    Dim Connection As clsMap.clsUnitSectorConnection
                    For Each Connection In Map.Sectors(SectorNum.X, SectorNum.Y).Units
                        Unit = Connection.Unit
                        If Unit.Pos.Horizontal.X >= StartPos.X And Unit.Pos.Horizontal.Y >= StartPos.Y And _
                            Unit.Pos.Horizontal.X <= FinishPos.X And Unit.Pos.Horizontal.Y <= FinishPos.Y Then
                            If Not Unit.MapSelectedUnitLink.IsConnected Then
                                Unit.MapSelect()
                            End If
                        End If
                    Next
                Next
            Next
        End If
        frmMainInstance.SelectedObject_Changed()
        DrawViewLater()
    End Sub

    Private Sub tmrDrawDelay_Tick(sender As System.Object, e As System.EventArgs) Handles tmrDrawDelay.Tick

        If DrawPending Then
            DrawPending = False
            DrawView()
        Else
            tmrDrawDelay.Enabled = False
        End If
    End Sub

    Private Sub pnlDraw_Resize(sender As Object, e As System.EventArgs) Handles pnlDraw.Resize

        If OpenGLControl IsNot Nothing Then
            ResizeOpenGL()
        End If
    End Sub

    Public Sub OpenGL_Resize(eventSender As System.Object, eventArgs As System.EventArgs)

        Dim Map As clsMap = MainMap

        GLSize.X = OpenGLControl.Width
        GLSize.Y = OpenGLControl.Height
        If GLSize.Y <> 0 Then
            GLSize_XPerY = CSng(GLSize.X / GLSize.Y)
        End If
        Viewport_Resize()
        If Map IsNot Nothing Then
            Map.ViewInfo.FOV_Calc()
        End If
        DrawViewLater()
    End Sub

    Public Sub OpenGL_MouseEnter(sender As Object, e As System.EventArgs)

        If Form.ActiveForm Is frmMainInstance Then
            OpenGLControl.Focus()
        End If
    End Sub

    Public Sub OpenGL_MouseWheel(sender As Object, e As System.Windows.Forms.MouseEventArgs)
        Dim Map As clsMap = MainMap

        If Map Is Nothing Then
            Exit Sub
        End If

        Dim Move As sXYZ_int
        Dim XYZ_dbl As Matrix3D.XYZ_dbl
        Dim A As Integer

        For A = 0 To CInt(Math.Abs(e.Delta / 120.0#))
            Matrix3D.VectorForwardsRotationByMatrix(Map.ViewInfo.ViewAngleMatrix, Math.Sign(e.Delta) * Math.Max(Map.ViewInfo.ViewPos.Y, 512.0#) / 24.0#, XYZ_dbl)
            Move.Set_dbl(XYZ_dbl)
            Map.ViewInfo.ViewPosChange(Move)
        Next
    End Sub

    Public Function CreateGLFont(BaseFont As Font) As GLFont

        Return New GLFont(New Font(BaseFont.FontFamily, 24.0F, BaseFont.Style, GraphicsUnit.Pixel))
    End Function

    Public WithEvents UndoMessageTimer As Timer

    Public Sub RemoveUndoMessage(sender As Object, e As EventArgs) Handles UndoMessageTimer.Tick

        UndoMessageTimer.Enabled = False
        lblUndo.Text = ""
    End Sub

    Public Sub DisplayUndoMessage(Text As String)

        lblUndo.Text = Text
        UndoMessageTimer.Enabled = False
        UndoMessageTimer.Enabled = True
    End Sub

    Private Sub OpenGL_MouseLeave(sender As Object, e As System.EventArgs)
        Dim Map As clsMap = MainMap

        If Map Is Nothing Then
            Exit Sub
        End If

        Map.ViewInfo.MouseOver = Nothing
    End Sub

    Public Sub ListSelectBegin(isPicker As Boolean)
        Dim Map As clsMap = MainMap
        Dim MouseOverTerrain As clsViewInfo.clsMouseOver.clsOverTerrain = Map.ViewInfo.GetMouseOverTerrain

        If MouseOverTerrain Is Nothing Then
            Stop
            Exit Sub
        End If

        Dim A As Integer
        Dim Unit As clsMap.clsUnit

        ListSelect.Close()
        ListSelect.Items.Clear()
        ReDim ListSelectItems(MouseOverTerrain.Units.Count - 1)
        For A = 0 To MouseOverTerrain.Units.Count - 1
            Unit = MouseOverTerrain.Units(A)
            ListSelectItems(A) = New ToolStripButton(Unit.Type.GetDisplayTextCode)
            ListSelectItems(A).Tag = Unit
            ListSelect.Items.Add(ListSelectItems(A))
        Next
        ListSelectIsPicker = isPicker
        ListSelect.Show(Me, New Drawing.Point(Map.ViewInfo.MouseOver.ScreenPos.X, Map.ViewInfo.MouseOver.ScreenPos.Y))
    End Sub

    Private Sub tabMaps_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles tabMaps.SelectedIndexChanged

        If Not tabMaps.Enabled Then
            Exit Sub
        End If

        If tabMaps.SelectedTab Is Nothing Then
            _Owner.SetMainMap(Nothing)
            Exit Sub
        End If

        Dim Map As clsMap = CType(tabMaps.SelectedTab.Tag, clsMap)

        _Owner.SetMainMap(Map)
    End Sub

    Private Sub btnClose_Click(sender As System.Object, e As System.EventArgs) Handles btnClose.Click
        Dim Map As clsMap = MainMap

        If Map Is Nothing Then
            Exit Sub
        End If
        If Not Map.frmMainLink.IsConnected Then
            MsgBox("Error: Map should be closed already.")
            Exit Sub
        End If

        If Not Map.ClosePrompt() Then
            Exit Sub
        End If

        Map.Deallocate()
    End Sub

    Public Sub UpdateTabs()
        Dim Map As clsMap

        tabMaps.Enabled = False
        tabMaps.TabPages.Clear()
        For Each Map In _Owner.LoadedMaps
            tabMaps.TabPages.Add(Map.MapView_TabPage)
        Next
        Map = MainMap
        If Map IsNot Nothing Then
            tabMaps.SelectedIndex = Map.frmMainLink.ArrayPosition
        Else
            tabMaps.SelectedIndex = -1
        End If
        tabMaps.Enabled = True
    End Sub

    Private ReadOnly Property MainMap As clsMap
        Get
            Return _Owner.MainMap
        End Get
    End Property
End Class
