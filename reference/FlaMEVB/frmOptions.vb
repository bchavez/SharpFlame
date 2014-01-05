
Public Class frmOptions

    Private DisplayFont As Font

    Private MinimapCliffColour As clsRGBA_sng
    Private clrMinimapCliffs As ctrlColour
    Private MinimapSelectedObjectColour As clsRGBA_sng
    Private clrMinimapSelectedObjects As ctrlColour
    Private ObjectDataPathSet As New ctrlPathSet("Object Data Directories")
    Private TilesetsPathSet As New ctrlPathSet("Tilesets Directories")

    Private ChangedKeyControls As clsKeyboardProfile

    Private lstKeyboardControls_Items(-1) As clsOption(Of clsKeyboardControl)

    Public Sub New()
        InitializeComponent()

        Icon = ProgramIcon

#If Mono Then
        For Each tab As TabPage In TabControl1.TabPages
            tab.Text &= " "c
        Next
#End If

        TilesetsPathSet.Dock = DockStyle.Fill
        ObjectDataPathSet.Dock = DockStyle.Fill
        TableLayoutPanel1.Controls.Add(TilesetsPathSet, 0, 0)
        TableLayoutPanel1.Controls.Add(ObjectDataPathSet, 0, 1)

        ChangedKeyControls = CType(KeyboardProfile.GetCopy(New clsKeyboardProfileCreator), clsKeyboardProfile)

        txtAutosaveChanges.Text = InvariantToString_uint(Settings.AutoSaveMinChanges)
        txtAutosaveInterval.Text = InvariantToString_uint(Settings.AutoSaveMinInterval_s)
        cbxAutosaveCompression.Checked = Settings.AutoSaveCompress
        cbxAutosaveEnabled.Checked = Settings.AutoSaveEnabled
        cbxAskDirectories.Checked = Settings.DirectoriesPrompt
        cbxPointerDirect.Checked = Settings.DirectPointer
        DisplayFont = Settings.MakeFont
        UpdateDisplayFontLabel()
        txtFOV.Text = InvariantToString_dbl(Settings.FOVDefault)

        MinimapCliffColour = New clsRGBA_sng(Settings.MinimapCliffColour)
        clrMinimapCliffs = New ctrlColour(MinimapCliffColour)
        pnlMinimapCliffColour.Controls.Add(clrMinimapCliffs)

        MinimapSelectedObjectColour = New clsRGBA_sng(Settings.MinimapSelectedObjectsColour)
        clrMinimapSelectedObjects = New ctrlColour(MinimapSelectedObjectColour)
        pnlMinimapSelectedObjectColour.Controls.Add(clrMinimapSelectedObjects)

        txtMinimapSize.Text = InvariantToString_int(Settings.MinimapSize)
        cbxMinimapObjectColours.Checked = Settings.MinimapTeamColours
        cbxMinimapTeamColourFeatures.Checked = Settings.MinimapTeamColoursExceptFeatures
        cbxMipmaps.Checked = Settings.Mipmaps
        cbxMipmapsHardware.Checked = Settings.MipmapsHardware
        txtUndoSteps.Text = InvariantToString_uint(Settings.UndoLimit)

        TilesetsPathSet.SetPaths(Settings.TilesetDirectories)
        ObjectDataPathSet.SetPaths(Settings.ObjectDataDirectories)
        TilesetsPathSet.SelectedNum = Clamp_int(CType(Settings.Value(Setting_DefaultTilesetsPathNum), Integer), -1, Settings.TilesetDirectories.Count - 1)
        ObjectDataPathSet.SelectedNum = Clamp_int(CType(Settings.Value(Setting_DefaultObjectDataPathNum), Integer), -1, Settings.ObjectDataDirectories.Count - 1)

        txtMapBPP.Text = InvariantToString_int(Settings.MapViewBPP)
        txtMapDepth.Text = InvariantToString_int(Settings.MapViewDepth)
        txtTexturesBPP.Text = InvariantToString_int(Settings.TextureViewBPP)
        txtTexturesDepth.Text = InvariantToString_int(Settings.TextureViewDepth)

        cbxPickerOrientation.Checked = Settings.PickOrientation

        UpdateKeyboardControls(-1)
    End Sub

    Private Sub btnSave_Click(sender As System.Object, e As System.EventArgs) Handles btnSave.Click

        Dim NewSettings As clsSettings = CType(Settings.GetCopy(New clsSettingsCreator), clsSettings)
        Dim dblTemp As Double
        Dim intTemp As Integer

        If InvariantParse_dbl(txtAutosaveChanges.Text, dblTemp) Then
            NewSettings.Changes(Setting_AutoSaveMinChanges) = New clsOptionProfile.clsChange(Of UInteger)(CUInt(Clamp_dbl(dblTemp, 1.0#, CDbl(UInteger.MaxValue) - 1.0#)))
        End If
        If InvariantParse_dbl(txtAutosaveInterval.Text, dblTemp) Then
            NewSettings.Changes(Setting_AutoSaveMinInterval_s) = New clsOptionProfile.clsChange(Of UInteger)(CUInt(Clamp_dbl(dblTemp, 1.0#, CDbl(UInteger.MaxValue) - 1.0#)))
        End If
        NewSettings.Changes(Setting_AutoSaveCompress) = New clsOptionProfile.clsChange(Of Boolean)(cbxAutosaveCompression.Checked)
        NewSettings.Changes(Setting_AutoSaveEnabled) = New clsOptionProfile.clsChange(Of Boolean)(cbxAutosaveEnabled.Checked)
        NewSettings.Changes(Setting_DirectoriesPrompt) = New clsOptionProfile.clsChange(Of Boolean)(cbxAskDirectories.Checked)
        NewSettings.Changes(Setting_DirectPointer) = New clsOptionProfile.clsChange(Of Boolean)(cbxPointerDirect.Checked)
        NewSettings.Changes(Setting_FontFamily) = New clsOptionProfile.clsChange(Of FontFamily)(DisplayFont.FontFamily)
        If InvariantParse_dbl(txtFOV.Text, dblTemp) Then
            NewSettings.Changes(Setting_FOVDefault) = New clsOptionProfile.clsChange(Of Double)(dblTemp)
        End If
        NewSettings.Changes(Setting_MinimapCliffColour) = New clsOptionProfile.clsChange(Of clsRGBA_sng)(MinimapCliffColour)
        NewSettings.Changes(Setting_MinimapSelectedObjectsColour) = New clsOptionProfile.clsChange(Of clsRGBA_sng)(MinimapSelectedObjectColour)
        If InvariantParse_int(txtMinimapSize.Text, intTemp) Then
            NewSettings.Changes(Setting_MinimapSize) = New clsOptionProfile.clsChange(Of Integer)(intTemp)
        End If
        NewSettings.Changes(Setting_MinimapTeamColours) = New clsOptionProfile.clsChange(Of Boolean)(cbxMinimapObjectColours.Checked)
        NewSettings.Changes(Setting_MinimapTeamColoursExceptFeatures) = New clsOptionProfile.clsChange(Of Boolean)(cbxMinimapTeamColourFeatures.Checked)
        NewSettings.Changes(Setting_Mipmaps) = New clsOptionProfile.clsChange(Of Boolean)(cbxMipmaps.Checked)
        NewSettings.Changes(Setting_MipmapsHardware) = New clsOptionProfile.clsChange(Of Boolean)(cbxMipmapsHardware.Checked)
        If InvariantParse_int(txtUndoSteps.Text, intTemp) Then
            NewSettings.Changes(Setting_UndoLimit) = New clsOptionProfile.clsChange(Of Integer)(intTemp)
        End If
        Dim tilesetPaths As New SimpleList(Of String)
        Dim objectsPaths As New SimpleList(Of String)
        Dim controlTilesetPaths() As String = TilesetsPathSet.GetPaths
        Dim controlobjectsPaths() As String = ObjectDataPathSet.GetPaths
        For i As Integer = 0 To controlTilesetPaths.GetUpperBound(0)
            tilesetPaths.Add(controlTilesetPaths(i))
        Next
        For i As Integer = 0 To controlobjectsPaths.GetUpperBound(0)
            objectsPaths.Add(controlobjectsPaths(i))
        Next
        NewSettings.Changes(Setting_TilesetDirectories) = New clsOptionProfile.clsChange(Of SimpleList(Of String))(tilesetPaths)
        NewSettings.Changes(Setting_ObjectDataDirectories) = New clsOptionProfile.clsChange(Of SimpleList(Of String))(objectsPaths)
        NewSettings.Changes(Setting_DefaultTilesetsPathNum) = New clsOptionProfile.clsChange(Of Integer)(TilesetsPathSet.SelectedNum)
        NewSettings.Changes(Setting_DefaultObjectDataPathNum) = New clsOptionProfile.clsChange(Of Integer)(ObjectDataPathSet.SelectedNum)
        If InvariantParse_int(txtMapBPP.Text, intTemp) Then
            NewSettings.Changes(Setting_MapViewBPP) = New clsOptionProfile.clsChange(Of Integer)(intTemp)
        End If
        If InvariantParse_int(txtMapDepth.Text, intTemp) Then
            NewSettings.Changes(Setting_MapViewDepth) = New clsOptionProfile.clsChange(Of Integer)(intTemp)
        End If
        If InvariantParse_int(txtTexturesBPP.Text, intTemp) Then
            NewSettings.Changes(Setting_TextureViewBPP) = New clsOptionProfile.clsChange(Of Integer)(intTemp)
        End If
        If InvariantParse_int(txtTexturesDepth.Text, intTemp) Then
            NewSettings.Changes(Setting_TextureViewDepth) = New clsOptionProfile.clsChange(Of Integer)(intTemp)
        End If
        NewSettings.Changes(Setting_PickOrientation) = New clsOptionProfile.clsChange(Of Boolean)(cbxPickerOrientation.Checked)

        UpdateSettings(NewSettings)

        Dim Map As clsMap = frmMainInstance.MainMap
        If Map IsNot Nothing Then
            Map.MinimapMakeLater()
        End If
        frmMainInstance.View_DrawViewLater()

        KeyboardProfile = ChangedKeyControls

        Finish(Windows.Forms.DialogResult.OK)
    End Sub

    Private Sub btnCancel_Click(sender As System.Object, e As System.EventArgs) Handles btnCancel.Click

        Finish(Windows.Forms.DialogResult.Cancel)
    End Sub

    Private AllowClose As Boolean = False

    Private Sub frmOptions_FormClosing(sender As Object, e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing

        e.Cancel = Not AllowClose
    End Sub

    'setting DialogResult in mono tries to close the form

    Private Sub Finish(result As Windows.Forms.DialogResult)

        AllowClose = True
        frmOptionsInstance = Nothing
        If Modal Then
            DialogResult = result 'mono closes here
#If Not Mono Then
            Close()
#End If
        Else
            Close()
        End If
    End Sub

    Private Sub btnFont_Click(sender As System.Object, e As System.EventArgs) Handles btnFont.Click
        Dim FontDialog As New Windows.Forms.FontDialog

        Dim Result As DialogResult
        Try 'mono 267 has crashed here
            FontDialog.Font = DisplayFont
            FontDialog.FontMustExist = True
            Result = FontDialog.ShowDialog(Me)
        Catch
            Result = Windows.Forms.DialogResult.Cancel
        End Try
        If Result = Windows.Forms.DialogResult.OK Then
            DisplayFont = FontDialog.Font
            UpdateDisplayFontLabel()
        End If
    End Sub

    Private Sub btnAutosaveOpen_Click(sender As System.Object, e As System.EventArgs) Handles btnAutosaveOpen.Click

        frmMainInstance.Load_Autosave_Prompt()
    End Sub

    Private Sub UpdateDisplayFontLabel()

        lblFont.Text = DisplayFont.FontFamily.Name & " " & DisplayFont.SizeInPoints & " "
        If DisplayFont.Bold Then
            lblFont.Text &= "B"
        End If
        If DisplayFont.Italic Then
            lblFont.Text &= "I"
        End If
    End Sub

    Private Sub UpdateKeyboardControl(index As Integer)

        lstKeyboardControls.Items(index) = GetKeyControlText(CType(Options_KeyboardControls.Options(index), clsOption(Of clsKeyboardControl)))
    End Sub

    Private Sub UpdateKeyboardControls(selectedIndex As Integer)

        lstKeyboardControls.Hide()
        lstKeyboardControls.Items.Clear()
        ReDim lstKeyboardControls_Items(Options_KeyboardControls.Options.Count - 1)
        For i As Integer = 0 To Options_KeyboardControls.Options.Count - 1
            Dim item As clsOption(Of clsKeyboardControl) = CType(Options_KeyboardControls.Options(i), clsOption(Of clsKeyboardControl))
            Dim text As String = GetKeyControlText(item)
            lstKeyboardControls_Items(lstKeyboardControls.Items.Add(text)) = item
        Next
        lstKeyboardControls.SelectedIndex = selectedIndex
        lstKeyboardControls.Show()
    End Sub

    Private Function GetKeyControlText(item As clsOption(Of clsKeyboardControl)) As String

        Dim text As String = item.SaveKey & " = "
        Dim control As clsKeyboardControl = CType(ChangedKeyControls.Value(item), clsKeyboardControl)
        For j As Integer = 0 To control.Keys.GetUpperBound(0)
            Dim key As Keys = control.Keys(j)
            Dim keyText As String = [Enum].GetName(GetType(Keys), key)
            If keyText Is Nothing Then
                text &= InvariantToString_int(key)
            Else
                text &= keyText
            End If
            If j < control.Keys.GetUpperBound(0) Then
                text &= " + "
            End If
        Next
        If control.UnlessKeys.GetUpperBound(0) >= 0 Then
            text &= " unless "
            For j As Integer = 0 To control.UnlessKeys.GetUpperBound(0)
                Dim key As Keys = control.UnlessKeys(j)
                Dim keyText As String = [Enum].GetName(GetType(Keys), key)
                If keyText Is Nothing Then
                    text &= InvariantToString_int(key)
                Else
                    text &= keyText
                End If
                If j < control.UnlessKeys.GetUpperBound(0) Then
                    text &= ", "
                End If
            Next
        End If
        If control IsNot item.DefaultValue Then
            text &= " (modified)"
        End If

        Return text
    End Function

    Private Sub btnKeyControlChange_Click(sender As System.Object, e As System.EventArgs) Handles btnKeyControlChange.Click

        If lstKeyboardControls.SelectedIndex < 0 Then
            Exit Sub
        End If

        Dim capture As New frmKeyboardControl
        If capture.ShowDialog() <> Windows.Forms.DialogResult.OK Then
            Exit Sub
        End If
        If capture.Results.Count = 0 Then
            Exit Sub
        End If
        Dim keyOption As clsOption(Of clsKeyboardControl) = lstKeyboardControls_Items(lstKeyboardControls.SelectedIndex)
        Dim previous As clsKeyboardControl = CType(ChangedKeyControls.Value(keyOption), clsKeyboardControl)
        Dim keys() As Keys
        ReDim keys(capture.Results.Count - 1)
        For i As Integer = 0 To capture.Results.Count - 1
            keys(i) = capture.Results(i).Item
        Next
        Dim copy As clsKeyboardControl = New clsKeyboardControl(keys, previous.UnlessKeys)
        ChangedKeyControls.Changes(keyOption) = New clsOptionProfile.clsChange(Of clsKeyboardControl)(copy)
        UpdateKeyboardControl(keyOption.GroupLink.ArrayPosition)
    End Sub

    Private Sub btnKeyControlChangeUnless_Click(sender As System.Object, e As System.EventArgs) Handles btnKeyControlChangeUnless.Click

        If lstKeyboardControls.SelectedIndex < 0 Then
            Exit Sub
        End If

        Dim capture As New frmKeyboardControl
        If capture.ShowDialog() <> Windows.Forms.DialogResult.OK Then
            Exit Sub
        End If
        If capture.Results.Count = 0 Then
            Exit Sub
        End If
        Dim keyOption As clsOption(Of clsKeyboardControl) = lstKeyboardControls_Items(lstKeyboardControls.SelectedIndex)
        Dim previous As clsKeyboardControl = CType(ChangedKeyControls.Value(keyOption), clsKeyboardControl)
        Dim unlessKeys() As Keys
        ReDim unlessKeys(capture.Results.Count - 1)
        For i As Integer = 0 To capture.Results.Count - 1
            unlessKeys(i) = capture.Results(i).Item
        Next
        Dim copy As clsKeyboardControl = New clsKeyboardControl(previous.Keys, unlessKeys)
        ChangedKeyControls.Changes(keyOption) = New clsOptionProfile.clsChange(Of clsKeyboardControl)(copy)
        UpdateKeyboardControl(keyOption.GroupLink.ArrayPosition)
    End Sub

    Private Sub btnKeyControlChangeDefault_Click(sender As System.Object, e As System.EventArgs) Handles btnKeyControlChangeDefault.Click

        If lstKeyboardControls.SelectedIndex < 0 Then
            Exit Sub
        End If

        Dim keyOption As clsOption(Of clsKeyboardControl) = lstKeyboardControls_Items(lstKeyboardControls.SelectedIndex)
        ChangedKeyControls.Changes(keyOption) = Nothing
        UpdateKeyboardControl(keyOption.GroupLink.ArrayPosition)
    End Sub
End Class
