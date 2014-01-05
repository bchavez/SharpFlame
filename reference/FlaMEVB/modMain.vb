Imports OpenTK.Graphics

Public Module modMain

    Public InitializeDelay As Timer
    Public InitializeResult As New clsResult("Startup result")

    Public frmMainInstance As frmMain
    Public frmGeneratorInstance As frmGenerator
    Public frmOptionsInstance As frmOptions

    Public OpenGL1 As OpenTK.GLControl
    Public OpenGL2 As OpenTK.GLControl

    Sub Main()

        Windows.Forms.Application.EnableVisualStyles()

        PlatformPathSeparator = IO.Path.DirectorySeparatorChar
        SetProgramSubDirs()

        CreateSettingOptions()
        CreateControls() 'needed to load key control settings
        Dim SettingsLoadResult As clsResult = Settings_Load(InitializeSettings)
        InitializeResult.Add(SettingsLoadResult)

        OpenGL1 = New OpenTK.GLControl(New GraphicsMode(New ColorFormat(InitializeSettings.MapViewBPP), InitializeSettings.MapViewDepth, 0))
        OpenGL2 = New OpenTK.GLControl(New GraphicsMode(New ColorFormat(InitializeSettings.TextureViewBPP), InitializeSettings.TextureViewDepth, 0))

        Do While OpenGL1.Context Is Nothing Or OpenGL2.Context Is Nothing
            'todo, why is this needed?
        Loop

        frmMainInstance = New frmMain

        Try
            ProgramIcon = New Icon(My.Application.Info.DirectoryPath & PlatformPathSeparator & "flaME.ico")
        Catch ex As Exception
            InitializeResult.WarningAdd(ProgramName & " icon is missing: " & ex.Message)
        End Try
        frmMainInstance.Icon = ProgramIcon
        frmGeneratorInstance.Icon = ProgramIcon

        InitializeDelay = New Timer
        AddHandler InitializeDelay.Tick, AddressOf frmMainInstance.Initialize
        InitializeDelay.Interval = 50
        InitializeDelay.Enabled = True

        Do Until ProgramInitializeFinished
            Threading.Thread.Sleep(50)
            Application.DoEvents()
        Loop

        Windows.Forms.Application.Run(frmMainInstance)
    End Sub
End Module
