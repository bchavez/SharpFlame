
Public Module modSettings

    Public Options_Settings As New clsOptionGroup
    Public InitializeSettings As clsSettings
    Public Settings As clsSettings

    Public Setting_AutoSaveEnabled As clsOption(Of Boolean)
    Public Setting_AutoSaveCompress As clsOption(Of Boolean)
    Public Setting_AutoSaveMinInterval_s As clsOption(Of UInteger)
    Public Setting_AutoSaveMinChanges As clsOption(Of UInteger)
    Public Setting_UndoLimit As clsOption(Of UInteger)
    Public Setting_DirectoriesPrompt As clsOption(Of Boolean)
    Public Setting_DirectPointer As clsOption(Of Boolean)
    Public Setting_FontFamily As clsOption(Of FontFamily)
    Public Setting_FontBold As clsOption(Of Boolean)
    Public Setting_FontItalic As clsOption(Of Boolean)
    Public Class clsOption_FontSize
        Inherits clsOption(Of Single)

        Public Sub New(saveKey As String, defaultValue As Single)
            MyBase.New(saveKey, defaultValue)
        End Sub

        Public Overrides Function IsValueValid(value As Object) As Boolean
            Return CType(value, Single) >= 0.0F
        End Function
    End Class
    Public Class clsOptionCreator_FontSize
        Inherits clsOptionCreator(Of Single)

        Public Overrides Function Create() As clsOption(Of Single)
            Return New clsOption_FontSize(SaveKey, DefaultValue)
        End Function
    End Class
    Public Setting_FontSize As clsOption_FontSize
    Public Class clsOption_MinimapSize
        Inherits clsOption(Of Integer)

        Public Sub New(saveKey As String, defaultValue As Integer)
            MyBase.New(saveKey, defaultValue)
        End Sub

        Public Overrides Function IsValueValid(value As Object) As Boolean
            Dim intValue As Integer = CType(value, Integer)
            Return intValue >= 0 And intValue <= MinimapMaxSize
        End Function
    End Class
    Public Class clsOptionCreator_MinimapSize
        Inherits clsOptionCreator(Of Integer)

        Public Overrides Function Create() As clsOption(Of Integer)
            Return New clsOption_MinimapSize(SaveKey, DefaultValue)
        End Function
    End Class
    Public Setting_MinimapSize As clsOption_MinimapSize
    Public Setting_MinimapTeamColours As clsOption(Of Boolean)
    Public Setting_MinimapTeamColoursExceptFeatures As clsOption(Of Boolean)
    Public Setting_MinimapCliffColour As clsOption(Of clsRGBA_sng)
    Public Setting_MinimapSelectedObjectsColour As clsOption(Of clsRGBA_sng)
    Public Class clsOption_FOVDefault
        Inherits clsOption(Of Double)

        Public Sub New(saveKey As String, defaultValue As Double)
            MyBase.New(saveKey, defaultValue)
        End Sub

        Public Overrides Function IsValueValid(value As Object) As Boolean
            Dim dblValue As Double = CType(value, Double)
            Return dblValue >= 0.00005# And dblValue <= 0.005#
        End Function
    End Class
    Public Class clsOptionCreator_FOVDefault
        Inherits clsOptionCreator(Of Double)

        Public Overrides Function Create() As clsOption(Of Double)
            Return New clsOption_FOVDefault(SaveKey, DefaultValue)
        End Function
    End Class
    Public Setting_FOVDefault As clsOption_FOVDefault
    Public Setting_Mipmaps As clsOption(Of Boolean)
    Public Setting_MipmapsHardware As clsOption(Of Boolean)
    Public Setting_OpenPath As clsOption(Of String)
    Public Setting_SavePath As clsOption(Of String)
    Public Setting_MapViewBPP As clsOption(Of Integer)
    Public Setting_TextureViewBPP As clsOption(Of Integer)
    Public Setting_MapViewDepth As clsOption(Of Integer)
    Public Setting_TextureViewDepth As clsOption(Of Integer)
    Public Setting_TilesetDirectories As clsOption(Of SimpleList(Of String))
    Public Setting_ObjectDataDirectories As clsOption(Of SimpleList(Of String))
    Public Setting_DefaultTilesetsPathNum As clsOption(Of Integer)
    Public Setting_DefaultObjectDataPathNum As clsOption(Of Integer)
    Public Setting_PickOrientation As clsOption(Of Boolean)

    Public Class clsSettings
        Inherits clsOptionProfile

        Public Sub New()
            MyBase.New(Options_Settings)
        End Sub

        Public ReadOnly Property AutoSaveEnabled As Boolean
            Get
                Return CType(Value(Setting_AutoSaveEnabled), Boolean)
            End Get
        End Property
        Public ReadOnly Property AutoSaveCompress As Boolean
            Get
                Return CType(Value(Setting_AutoSaveCompress), Boolean)
            End Get
        End Property
        Public ReadOnly Property AutoSaveMinInterval_s As UInteger
            Get
                Return CType(Value(Setting_AutoSaveMinInterval_s), UInteger)
            End Get
        End Property
        Public ReadOnly Property AutoSaveMinChanges As UInteger
            Get
                Return CType(Value(Setting_AutoSaveMinChanges), UInteger)
            End Get
        End Property
        Public ReadOnly Property UndoLimit As UInteger
            Get
                Return CType(Value(Setting_UndoLimit), UInteger)
            End Get
        End Property
        Public ReadOnly Property DirectoriesPrompt As Boolean
            Get
                Return CType(Value(Setting_DirectoriesPrompt), Boolean)
            End Get
        End Property
        Public ReadOnly Property DirectPointer As Boolean
            Get
                Return CType(Value(Setting_DirectPointer), Boolean)
            End Get
        End Property
        Public ReadOnly Property FontFamily As FontFamily
            Get
                Return CType(Value(Setting_FontFamily), FontFamily)
            End Get
        End Property
        Public ReadOnly Property FontBold As Boolean
            Get
                Return CType(Value(Setting_FontBold), Boolean)
            End Get
        End Property
        Public ReadOnly Property FontItalic As Boolean
            Get
                Return CType(Value(Setting_FontItalic), Boolean)
            End Get
        End Property
        Public ReadOnly Property FontSize As Single
            Get
                Return CType(Value(Setting_FontSize), Single)
            End Get
        End Property
        Public ReadOnly Property MinimapSize As Integer
            Get
                Return CType(Value(Setting_MinimapSize), Integer)
            End Get
        End Property
        Public ReadOnly Property MinimapTeamColours As Boolean
            Get
                Return CType(Value(Setting_MinimapTeamColours), Boolean)
            End Get
        End Property
        Public ReadOnly Property MinimapTeamColoursExceptFeatures As Boolean
            Get
                Return CType(Value(Setting_MinimapTeamColoursExceptFeatures), Boolean)
            End Get
        End Property
        Public ReadOnly Property MinimapCliffColour As clsRGBA_sng
            Get
                Return CType(Value(Setting_MinimapCliffColour), clsRGBA_sng)
            End Get
        End Property
        Public ReadOnly Property MinimapSelectedObjectsColour As clsRGBA_sng
            Get
                Return CType(Value(Setting_MinimapSelectedObjectsColour), clsRGBA_sng)
            End Get
        End Property
        Public ReadOnly Property FOVDefault As Double
            Get
                Return CType(Value(Setting_FOVDefault), Double)
            End Get
        End Property
        Public ReadOnly Property Mipmaps As Boolean
            Get
                Return CType(Value(Setting_Mipmaps), Boolean)
            End Get
        End Property
        Public ReadOnly Property MipmapsHardware As Boolean
            Get
                Return CType(Value(Setting_MipmapsHardware), Boolean)
            End Get
        End Property
        Public Property OpenPath As String
            Get
                Return CType(Value(Setting_OpenPath), String)
            End Get
            Set(value As String)
                Changes(Setting_OpenPath) = New clsOptionProfile.clsChange(Of String)(value)
            End Set
        End Property
        Public Property SavePath As String
            Get
                Return CType(Value(Setting_SavePath), String)
            End Get
            Set(value As String)
                Changes(Setting_SavePath) = New clsOptionProfile.clsChange(Of String)(value)
            End Set
        End Property
        Public ReadOnly Property MapViewBPP As Integer
            Get
                Return CType(Value(Setting_MapViewBPP), Integer)
            End Get
        End Property
        Public ReadOnly Property TextureViewBPP As Integer
            Get
                Return CType(Value(Setting_TextureViewBPP), Integer)
            End Get
        End Property
        Public ReadOnly Property MapViewDepth As Integer
            Get
                Return CType(Value(Setting_MapViewDepth), Integer)
            End Get
        End Property
        Public ReadOnly Property TextureViewDepth As Integer
            Get
                Return CType(Value(Setting_TextureViewDepth), Integer)
            End Get
        End Property
        Public ReadOnly Property TilesetDirectories As SimpleList(Of String)
            Get
                Return CType(Value(Setting_TilesetDirectories), SimpleList(Of String))
            End Get
        End Property
        Public ReadOnly Property ObjectDataDirectories As SimpleList(Of String)
            Get
                Return CType(Value(Setting_ObjectDataDirectories), SimpleList(Of String))
            End Get
        End Property
        Public ReadOnly Property PickOrientation As Boolean
            Get
                Return CType(Value(Setting_PickOrientation), Boolean)
            End Get
        End Property

        Public Function MakeFont() As Font

            Dim style As FontStyle = FontStyle.Regular
            If FontBold Then
                style = style Or FontStyle.Bold
            End If
            If FontItalic Then
                style = style Or FontStyle.Italic
            End If
            Return New Font(FontFamily, FontSize, style, GraphicsUnit.Point)
        End Function
    End Class
    Public Class clsSettingsCreator
        Inherits clsOptionProfileCreator

        Public Overrides Function Create() As clsOptionProfile
            Return New clsSettings
        End Function
    End Class

    Private Function CreateSetting(Of T)(saveKey As String, defaultValue As T) As clsOption(Of T)

        Dim creator As New clsOptionCreator(Of T)
        creator.SaveKey = saveKey
        creator.DefaultValue = defaultValue
        Dim result As clsOption(Of T) = creator.Create
        Options_Settings.Options.Add(result.GroupLink)
        Return result
    End Function

    Private Function CreateSetting(Of T)(creator As clsOptionCreator(Of T), saveKey As String, defaultValue As T) As clsOption(Of T)

        creator.SaveKey = saveKey
        creator.DefaultValue = defaultValue
        Dim result As clsOption(Of T) = creator.Create
        Options_Settings.Options.Add(result.GroupLink)
        Return result
    End Function

    Public Sub CreateSettingOptions()

        Setting_AutoSaveEnabled = CreateSetting(Of Boolean)("AutoSave", True)
        Setting_AutoSaveCompress = CreateSetting(Of Boolean)("AutoSaveCompress", False)
        Setting_AutoSaveMinInterval_s = CreateSetting(Of UInteger)("AutoSaveMinInterval", 180UI)
        Setting_AutoSaveMinChanges = CreateSetting(Of UInteger)("AutoSaveMinChanges", 20UI)
        Setting_UndoLimit = CreateSetting(Of UInteger)("UndoLimit", 256UI)
        Setting_DirectoriesPrompt = CreateSetting(Of Boolean)("DirectoriesPrompt", True)
        Setting_DirectPointer = CreateSetting(Of Boolean)("DirectPointer", True)
        Setting_FontFamily = CreateSetting(Of FontFamily)("FontFamily", FontFamily.GenericSerif)
        Setting_FontBold = CreateSetting(Of Boolean)("FontBold", True)
        Setting_FontItalic = CreateSetting(Of Boolean)("FontItalic", False)
        Setting_FontSize = CType(CreateSetting(Of Single)(New clsOptionCreator_FontSize, "FontSize", 20.0F), clsOption_FontSize)
        Setting_MinimapSize = CType(CreateSetting(Of Integer)(New clsOptionCreator_MinimapSize, "MinimapSize", 160), clsOption_MinimapSize)
        Setting_MinimapTeamColours = CreateSetting(Of Boolean)("MinimapTeamColours", True)
        Setting_MinimapTeamColoursExceptFeatures = CreateSetting(Of Boolean)("MinimapTeamColoursExceptFeatures", True)
        Setting_MinimapCliffColour = CreateSetting(Of clsRGBA_sng)("MinimapCliffColour", New clsRGBA_sng(1.0F, 0.25F, 0.25F, 0.5F))
        Setting_MinimapSelectedObjectsColour = CreateSetting(Of clsRGBA_sng)("MinimapSelectedObjectsColour", New clsRGBA_sng(1.0F, 1.0F, 1.0F, 0.75F))
        Setting_FOVDefault = CType(CreateSetting(Of Double)(New clsOptionCreator_FOVDefault, "FOVDefault", 30.0# / (50.0# * 900.0#)), clsOption_FOVDefault)  'screenVerticalSize/(screenDist*screenVerticalPixels)
        Setting_Mipmaps = CreateSetting(Of Boolean)("Mipmaps", False)
        Setting_MipmapsHardware = CreateSetting(Of Boolean)("MipmapsHardware", False)
        Setting_OpenPath = CreateSetting(Of String)("OpenPath", Nothing)
        Setting_SavePath = CreateSetting(Of String)("SavePath", Nothing)
        Setting_MapViewBPP = CreateSetting(Of Integer)("MapViewBPP", OpenTK.DisplayDevice.Default.BitsPerPixel)
        Setting_TextureViewBPP = CreateSetting(Of Integer)("TextureViewBPP", OpenTK.DisplayDevice.Default.BitsPerPixel)
        Setting_MapViewDepth = CreateSetting(Of Integer)("MapViewDepth", 24)
        Setting_TextureViewDepth = CreateSetting(Of Integer)("TextureViewDepth", 24)
        Setting_TilesetDirectories = CreateSetting(Of SimpleList(Of String))("TilesetsPath", New SimpleList(Of String))
        Setting_ObjectDataDirectories = CreateSetting(Of SimpleList(Of String))("ObjectDataPath", New SimpleList(Of String))
        Setting_DefaultTilesetsPathNum = CreateSetting(Of Integer)("DefaultTilesetsPathNum", -1)
        Setting_DefaultObjectDataPathNum = CreateSetting(Of Integer)("DefaultObjectDataPathNum", -1)
        Setting_PickOrientation = CreateSetting(Of Boolean)("PickOrientation", True)
    End Sub

    Public Function Read_Settings(File As IO.StreamReader, ByRef Result As clsSettings) As clsResult
        Dim ReturnResult As New clsResult("Reading settings")

        Dim INIReader As New clsINIRead
        ReturnResult.Take(INIReader.ReadFile(File))
        Result = New clsSettings
        ReturnResult.Take(INIReader.RootSection.Translate(Result))
        For Each section As clsINIRead.clsSection In INIReader.Sections
            If section.Name.ToLower = "keyboardcontrols" Then
                Dim keyResults As New clsResult("Keyboard controls")
                keyResults.Take(section.Translate(KeyboardProfile))
                ReturnResult.Take(keyResults)
            End If
        Next

        Return ReturnResult
    End Function

    Public Sub UpdateSettings(NewSettings As clsSettings)
        Dim FontChanged As Boolean

        If Settings Is Nothing Then
            FontChanged = True
        Else
            If Settings.FontFamily Is Nothing Then
                FontChanged = (NewSettings.FontFamily IsNot Nothing)
            Else
                If NewSettings.FontFamily Is Nothing Then
                    FontChanged = True
                Else
                    If Settings.FontFamily.Name = NewSettings.FontFamily.Name _
                        And Settings.FontBold = NewSettings.FontBold _
                        And Settings.FontItalic = NewSettings.FontItalic _
                        And Settings.FontSize = NewSettings.FontSize Then
                        FontChanged = False
                    Else
                        FontChanged = True
                    End If
                End If
            End If
        End If
        If FontChanged Then
            SetFont(NewSettings.MakeFont)
        End If

        Settings = NewSettings
    End Sub

    Private Sub SetFont(newFont As Font)

        If UnitLabelFont IsNot Nothing Then
            UnitLabelFont.Deallocate()
        End If
        UnitLabelFont = frmMainInstance.MapView.CreateGLFont(newFont)
    End Sub

    Public Function Settings_Write() As clsResult
        Dim ReturnResult As New clsResult("Writing settings to " & ControlChars.Quote & SettingsPath & ControlChars.Quote)

#If Not Portable Then
        If Not IO.Directory.Exists(MyDocumentsProgramPath) Then
            Try
                IO.Directory.CreateDirectory(MyDocumentsProgramPath)
            Catch ex As Exception
                ReturnResult.ProblemAdd("Unable to create folder " & ControlChars.Quote & MyDocumentsProgramPath & ControlChars.Quote & ": " & ex.Message)
                Return ReturnResult
            End Try
        End If
#End If

        Dim INI_Settings As clsINIWrite

        Try
            INI_Settings = clsINIWrite.CreateFile(IO.File.Create(SettingsPath))
        Catch ex As Exception
            ReturnResult.ProblemAdd(ex.Message)
            Return ReturnResult
        End Try

        ReturnResult.Take(Serialize_Settings(INI_Settings))
        INI_Settings.File.Close()

        Return ReturnResult
    End Function

    Private Function Serialize_Settings(File As clsINIWrite) As clsResult
        Dim ReturnResult As New clsResult("Serializing settings")

        ReturnResult.Take(Settings.INIWrite(File))
        If KeyboardProfile.IsAnythingChanged Then
            File.SectionName_Append("KeyboardControls")
            ReturnResult.Take(KeyboardProfile.INIWrite(File))
        End If

        Return ReturnResult
    End Function

    Public Function Settings_Load(ByRef Result As clsSettings) As clsResult
        Dim ReturnResult As New clsResult("Loading settings from " & ControlChars.Quote & SettingsPath & ControlChars.Quote)

        Dim File_Settings As IO.StreamReader
        Try
            File_Settings = New IO.StreamReader(SettingsPath)
        Catch
            Result = New clsSettings
            Return ReturnResult
        End Try

        ReturnResult.Take(Read_Settings(File_Settings, Result))

        File_Settings.Close()

        Return ReturnResult
    End Function
End Module
