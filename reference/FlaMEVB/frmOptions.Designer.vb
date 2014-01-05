<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmOptions
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        Me.TabPage3 = New System.Windows.Forms.TabPage()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.cbxAskDirectories = New System.Windows.Forms.CheckBox()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.TabPage1 = New System.Windows.Forms.TabPage()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.cbxPickerOrientation = New System.Windows.Forms.CheckBox()
        Me.GroupBox8 = New System.Windows.Forms.GroupBox()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.txtTexturesDepth = New System.Windows.Forms.TextBox()
        Me.txtTexturesBPP = New System.Windows.Forms.TextBox()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.txtMapDepth = New System.Windows.Forms.TextBox()
        Me.txtMapBPP = New System.Windows.Forms.TextBox()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.cbxMipmapsHardware = New System.Windows.Forms.CheckBox()
        Me.cbxMipmaps = New System.Windows.Forms.CheckBox()
        Me.GroupBox7 = New System.Windows.Forms.GroupBox()
        Me.txtFOV = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.GroupBox6 = New System.Windows.Forms.GroupBox()
        Me.cbxPointerDirect = New System.Windows.Forms.CheckBox()
        Me.GroupBox5 = New System.Windows.Forms.GroupBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.pnlMinimapSelectedObjectColour = New System.Windows.Forms.Panel()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.pnlMinimapCliffColour = New System.Windows.Forms.Panel()
        Me.txtMinimapSize = New System.Windows.Forms.TextBox()
        Me.cbxMinimapTeamColourFeatures = New System.Windows.Forms.CheckBox()
        Me.cbxMinimapObjectColours = New System.Windows.Forms.CheckBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.GroupBox4 = New System.Windows.Forms.GroupBox()
        Me.lblFont = New System.Windows.Forms.Label()
        Me.btnFont = New System.Windows.Forms.Button()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.txtAutosaveInterval = New System.Windows.Forms.TextBox()
        Me.txtAutosaveChanges = New System.Windows.Forms.TextBox()
        Me.btnAutosaveOpen = New System.Windows.Forms.Button()
        Me.cbxAutosaveCompression = New System.Windows.Forms.CheckBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.cbxAutosaveEnabled = New System.Windows.Forms.CheckBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.txtUndoSteps = New System.Windows.Forms.TextBox()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.TabPage2 = New System.Windows.Forms.TabPage()
        Me.btnKeyControlChangeDefault = New System.Windows.Forms.Button()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.btnKeyControlChangeUnless = New System.Windows.Forms.Button()
        Me.btnKeyControlChange = New System.Windows.Forms.Button()
        Me.lstKeyboardControls = New System.Windows.Forms.ListBox()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.btnSave = New System.Windows.Forms.Button()
        Me.TabControl1.SuspendLayout()
        Me.TabPage3.SuspendLayout()
        Me.TabPage1.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.GroupBox8.SuspendLayout()
        Me.GroupBox7.SuspendLayout()
        Me.GroupBox6.SuspendLayout()
        Me.GroupBox5.SuspendLayout()
        Me.GroupBox4.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.TabPage2.SuspendLayout()
        Me.SuspendLayout()
        '
        'TabControl1
        '
        Me.TabControl1.Controls.Add(Me.TabPage3)
        Me.TabControl1.Controls.Add(Me.TabPage1)
        Me.TabControl1.Controls.Add(Me.TabPage2)
        Me.TabControl1.Location = New System.Drawing.Point(12, 12)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(637, 398)
        Me.TabControl1.TabIndex = 35
        '
        'TabPage3
        '
        Me.TabPage3.Controls.Add(Me.Label12)
        Me.TabPage3.Controls.Add(Me.cbxAskDirectories)
        Me.TabPage3.Controls.Add(Me.TableLayoutPanel1)
        Me.TabPage3.Location = New System.Drawing.Point(4, 25)
        Me.TabPage3.Margin = New System.Windows.Forms.Padding(0)
        Me.TabPage3.Name = "TabPage3"
        Me.TabPage3.Size = New System.Drawing.Size(629, 369)
        Me.TabPage3.TabIndex = 2
        Me.TabPage3.Text = "Directories"
        Me.TabPage3.UseVisualStyleBackColor = True
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Location = New System.Drawing.Point(302, 15)
        Me.Label12.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(245, 20)
        Me.Label12.TabIndex = 42
        Me.Label12.Text = "Options on this tab take effect on restart."
        Me.Label12.UseCompatibleTextRendering = True
        '
        'cbxAskDirectories
        '
        Me.cbxAskDirectories.AutoSize = True
        Me.cbxAskDirectories.Location = New System.Drawing.Point(24, 14)
        Me.cbxAskDirectories.Margin = New System.Windows.Forms.Padding(4)
        Me.cbxAskDirectories.Name = "cbxAskDirectories"
        Me.cbxAskDirectories.Size = New System.Drawing.Size(225, 21)
        Me.cbxAskDirectories.TabIndex = 39
        Me.cbxAskDirectories.Text = "Show options before loading data"
        Me.cbxAskDirectories.UseCompatibleTextRendering = True
        Me.cbxAskDirectories.UseVisualStyleBackColor = True
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 1
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(3, 42)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 2
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(623, 324)
        Me.TableLayoutPanel1.TabIndex = 41
        '
        'TabPage1
        '
        Me.TabPage1.Controls.Add(Me.GroupBox3)
        Me.TabPage1.Controls.Add(Me.GroupBox8)
        Me.TabPage1.Controls.Add(Me.GroupBox7)
        Me.TabPage1.Controls.Add(Me.GroupBox6)
        Me.TabPage1.Controls.Add(Me.GroupBox5)
        Me.TabPage1.Controls.Add(Me.GroupBox4)
        Me.TabPage1.Controls.Add(Me.GroupBox2)
        Me.TabPage1.Controls.Add(Me.GroupBox1)
        Me.TabPage1.Location = New System.Drawing.Point(4, 25)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage1.Size = New System.Drawing.Size(629, 369)
        Me.TabPage1.TabIndex = 0
        Me.TabPage1.Text = "General"
        Me.TabPage1.UseVisualStyleBackColor = True
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.cbxPickerOrientation)
        Me.GroupBox3.Location = New System.Drawing.Point(316, 288)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(304, 54)
        Me.GroupBox3.TabIndex = 45
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "Picker"
        Me.GroupBox3.UseCompatibleTextRendering = True
        '
        'cbxPickerOrientation
        '
        Me.cbxPickerOrientation.AutoSize = True
        Me.cbxPickerOrientation.Location = New System.Drawing.Point(8, 22)
        Me.cbxPickerOrientation.Margin = New System.Windows.Forms.Padding(4)
        Me.cbxPickerOrientation.Name = "cbxPickerOrientation"
        Me.cbxPickerOrientation.Size = New System.Drawing.Size(192, 21)
        Me.cbxPickerOrientation.TabIndex = 51
        Me.cbxPickerOrientation.Text = "Capture texture orientations"
        Me.cbxPickerOrientation.UseCompatibleTextRendering = True
        Me.cbxPickerOrientation.UseVisualStyleBackColor = True
        '
        'GroupBox8
        '
        Me.GroupBox8.Controls.Add(Me.Label13)
        Me.GroupBox8.Controls.Add(Me.txtTexturesDepth)
        Me.GroupBox8.Controls.Add(Me.txtTexturesBPP)
        Me.GroupBox8.Controls.Add(Me.Label10)
        Me.GroupBox8.Controls.Add(Me.txtMapDepth)
        Me.GroupBox8.Controls.Add(Me.txtMapBPP)
        Me.GroupBox8.Controls.Add(Me.Label8)
        Me.GroupBox8.Controls.Add(Me.Label9)
        Me.GroupBox8.Controls.Add(Me.cbxMipmapsHardware)
        Me.GroupBox8.Controls.Add(Me.cbxMipmaps)
        Me.GroupBox8.Location = New System.Drawing.Point(6, 244)
        Me.GroupBox8.Name = "GroupBox8"
        Me.GroupBox8.Size = New System.Drawing.Size(304, 119)
        Me.GroupBox8.TabIndex = 45
        Me.GroupBox8.TabStop = False
        Me.GroupBox8.Text = "Graphics"
        Me.GroupBox8.UseCompatibleTextRendering = True
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Location = New System.Drawing.Point(7, 96)
        Me.Label13.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(90, 20)
        Me.Label13.TabIndex = 50
        Me.Label13.Text = "Textures View"
        Me.Label13.UseCompatibleTextRendering = True
        '
        'txtTexturesDepth
        '
        Me.txtTexturesDepth.Location = New System.Drawing.Point(180, 93)
        Me.txtTexturesDepth.Margin = New System.Windows.Forms.Padding(4)
        Me.txtTexturesDepth.Name = "txtTexturesDepth"
        Me.txtTexturesDepth.Size = New System.Drawing.Size(61, 22)
        Me.txtTexturesDepth.TabIndex = 49
        '
        'txtTexturesBPP
        '
        Me.txtTexturesBPP.Location = New System.Drawing.Point(105, 93)
        Me.txtTexturesBPP.Margin = New System.Windows.Forms.Padding(4)
        Me.txtTexturesBPP.Name = "txtTexturesBPP"
        Me.txtTexturesBPP.Size = New System.Drawing.Size(61, 22)
        Me.txtTexturesBPP.TabIndex = 48
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Location = New System.Drawing.Point(33, 67)
        Me.Label10.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(64, 20)
        Me.Label10.TabIndex = 46
        Me.Label10.Text = "Map View"
        Me.Label10.UseCompatibleTextRendering = True
        '
        'txtMapDepth
        '
        Me.txtMapDepth.Location = New System.Drawing.Point(180, 67)
        Me.txtMapDepth.Margin = New System.Windows.Forms.Padding(4)
        Me.txtMapDepth.Name = "txtMapDepth"
        Me.txtMapDepth.Size = New System.Drawing.Size(61, 22)
        Me.txtMapDepth.TabIndex = 44
        '
        'txtMapBPP
        '
        Me.txtMapBPP.Location = New System.Drawing.Point(105, 67)
        Me.txtMapBPP.Margin = New System.Windows.Forms.Padding(4)
        Me.txtMapBPP.Name = "txtMapBPP"
        Me.txtMapBPP.Size = New System.Drawing.Size(61, 22)
        Me.txtMapBPP.TabIndex = 42
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(96, 47)
        Me.Label8.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(70, 20)
        Me.Label8.TabIndex = 45
        Me.Label8.Text = "Colour Bits"
        Me.Label8.UseCompatibleTextRendering = True
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(174, 47)
        Me.Label9.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(67, 20)
        Me.Label9.TabIndex = 43
        Me.Label9.Text = "Depth Bits"
        Me.Label9.UseCompatibleTextRendering = True
        '
        'cbxMipmapsHardware
        '
        Me.cbxMipmapsHardware.AutoSize = True
        Me.cbxMipmapsHardware.Location = New System.Drawing.Point(169, 22)
        Me.cbxMipmapsHardware.Margin = New System.Windows.Forms.Padding(4)
        Me.cbxMipmapsHardware.Name = "cbxMipmapsHardware"
        Me.cbxMipmapsHardware.Size = New System.Drawing.Size(112, 21)
        Me.cbxMipmapsHardware.TabIndex = 41
        Me.cbxMipmapsHardware.Text = "Use Hardware"
        Me.cbxMipmapsHardware.UseCompatibleTextRendering = True
        Me.cbxMipmapsHardware.UseVisualStyleBackColor = True
        '
        'cbxMipmaps
        '
        Me.cbxMipmaps.AutoSize = True
        Me.cbxMipmaps.Location = New System.Drawing.Point(8, 22)
        Me.cbxMipmaps.Margin = New System.Windows.Forms.Padding(4)
        Me.cbxMipmaps.Name = "cbxMipmaps"
        Me.cbxMipmaps.Size = New System.Drawing.Size(141, 21)
        Me.cbxMipmaps.TabIndex = 40
        Me.cbxMipmaps.Text = "Generate mipmaps"
        Me.cbxMipmaps.UseCompatibleTextRendering = True
        Me.cbxMipmaps.UseVisualStyleBackColor = True
        '
        'GroupBox7
        '
        Me.GroupBox7.Controls.Add(Me.txtFOV)
        Me.GroupBox7.Controls.Add(Me.Label4)
        Me.GroupBox7.Location = New System.Drawing.Point(316, 232)
        Me.GroupBox7.Name = "GroupBox7"
        Me.GroupBox7.Size = New System.Drawing.Size(304, 50)
        Me.GroupBox7.TabIndex = 44
        Me.GroupBox7.TabStop = False
        Me.GroupBox7.Text = "Field Of View"
        Me.GroupBox7.UseCompatibleTextRendering = True
        '
        'txtFOV
        '
        Me.txtFOV.Location = New System.Drawing.Point(159, 15)
        Me.txtFOV.Margin = New System.Windows.Forms.Padding(4)
        Me.txtFOV.Name = "txtFOV"
        Me.txtFOV.Size = New System.Drawing.Size(138, 22)
        Me.txtFOV.TabIndex = 25
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(8, 18)
        Me.Label4.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(105, 20)
        Me.Label4.TabIndex = 26
        Me.Label4.Text = "Default Multiplier"
        Me.Label4.UseCompatibleTextRendering = True
        '
        'GroupBox6
        '
        Me.GroupBox6.Controls.Add(Me.cbxPointerDirect)
        Me.GroupBox6.Location = New System.Drawing.Point(316, 176)
        Me.GroupBox6.Name = "GroupBox6"
        Me.GroupBox6.Size = New System.Drawing.Size(304, 50)
        Me.GroupBox6.TabIndex = 43
        Me.GroupBox6.TabStop = False
        Me.GroupBox6.Text = "Pointer"
        Me.GroupBox6.UseCompatibleTextRendering = True
        '
        'cbxPointerDirect
        '
        Me.cbxPointerDirect.AutoSize = True
        Me.cbxPointerDirect.Location = New System.Drawing.Point(7, 22)
        Me.cbxPointerDirect.Margin = New System.Windows.Forms.Padding(4)
        Me.cbxPointerDirect.Name = "cbxPointerDirect"
        Me.cbxPointerDirect.Size = New System.Drawing.Size(62, 21)
        Me.cbxPointerDirect.TabIndex = 40
        Me.cbxPointerDirect.Text = "Direct"
        Me.cbxPointerDirect.UseCompatibleTextRendering = True
        Me.cbxPointerDirect.UseVisualStyleBackColor = True
        '
        'GroupBox5
        '
        Me.GroupBox5.Controls.Add(Me.Label6)
        Me.GroupBox5.Controls.Add(Me.pnlMinimapSelectedObjectColour)
        Me.GroupBox5.Controls.Add(Me.Label5)
        Me.GroupBox5.Controls.Add(Me.pnlMinimapCliffColour)
        Me.GroupBox5.Controls.Add(Me.txtMinimapSize)
        Me.GroupBox5.Controls.Add(Me.cbxMinimapTeamColourFeatures)
        Me.GroupBox5.Controls.Add(Me.cbxMinimapObjectColours)
        Me.GroupBox5.Controls.Add(Me.Label3)
        Me.GroupBox5.Location = New System.Drawing.Point(316, 7)
        Me.GroupBox5.Name = "GroupBox5"
        Me.GroupBox5.Size = New System.Drawing.Size(304, 163)
        Me.GroupBox5.TabIndex = 42
        Me.GroupBox5.TabStop = False
        Me.GroupBox5.Text = "Minimap"
        Me.GroupBox5.UseCompatibleTextRendering = True
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(8, 100)
        Me.Label6.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(99, 20)
        Me.Label6.TabIndex = 45
        Me.Label6.Text = "Object Highlight"
        Me.Label6.UseCompatibleTextRendering = True
        '
        'pnlMinimapSelectedObjectColour
        '
        Me.pnlMinimapSelectedObjectColour.Location = New System.Drawing.Point(132, 100)
        Me.pnlMinimapSelectedObjectColour.Name = "pnlMinimapSelectedObjectColour"
        Me.pnlMinimapSelectedObjectColour.Size = New System.Drawing.Size(164, 29)
        Me.pnlMinimapSelectedObjectColour.TabIndex = 44
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(8, 67)
        Me.Label5.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(71, 20)
        Me.Label5.TabIndex = 43
        Me.Label5.Text = "Cliff Colour"
        Me.Label5.UseCompatibleTextRendering = True
        '
        'pnlMinimapCliffColour
        '
        Me.pnlMinimapCliffColour.Location = New System.Drawing.Point(132, 67)
        Me.pnlMinimapCliffColour.Name = "pnlMinimapCliffColour"
        Me.pnlMinimapCliffColour.Size = New System.Drawing.Size(164, 29)
        Me.pnlMinimapCliffColour.TabIndex = 42
        '
        'txtMinimapSize
        '
        Me.txtMinimapSize.Location = New System.Drawing.Point(159, 15)
        Me.txtMinimapSize.Margin = New System.Windows.Forms.Padding(4)
        Me.txtMinimapSize.Name = "txtMinimapSize"
        Me.txtMinimapSize.Size = New System.Drawing.Size(61, 22)
        Me.txtMinimapSize.TabIndex = 25
        '
        'cbxMinimapTeamColourFeatures
        '
        Me.cbxMinimapTeamColourFeatures.AutoSize = True
        Me.cbxMinimapTeamColourFeatures.Location = New System.Drawing.Point(147, 42)
        Me.cbxMinimapTeamColourFeatures.Margin = New System.Windows.Forms.Padding(4)
        Me.cbxMinimapTeamColourFeatures.Name = "cbxMinimapTeamColourFeatures"
        Me.cbxMinimapTeamColourFeatures.Size = New System.Drawing.Size(139, 21)
        Me.cbxMinimapTeamColourFeatures.TabIndex = 41
        Me.cbxMinimapTeamColourFeatures.Text = "Except for features"
        Me.cbxMinimapTeamColourFeatures.UseCompatibleTextRendering = True
        Me.cbxMinimapTeamColourFeatures.UseVisualStyleBackColor = True
        '
        'cbxMinimapObjectColours
        '
        Me.cbxMinimapObjectColours.AutoSize = True
        Me.cbxMinimapObjectColours.Location = New System.Drawing.Point(8, 42)
        Me.cbxMinimapObjectColours.Margin = New System.Windows.Forms.Padding(4)
        Me.cbxMinimapObjectColours.Name = "cbxMinimapObjectColours"
        Me.cbxMinimapObjectColours.Size = New System.Drawing.Size(131, 21)
        Me.cbxMinimapObjectColours.TabIndex = 40
        Me.cbxMinimapObjectColours.Text = "Use team colours"
        Me.cbxMinimapObjectColours.UseCompatibleTextRendering = True
        Me.cbxMinimapObjectColours.UseVisualStyleBackColor = True
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(8, 18)
        Me.Label3.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(31, 20)
        Me.Label3.TabIndex = 26
        Me.Label3.Text = "Size"
        Me.Label3.UseCompatibleTextRendering = True
        '
        'GroupBox4
        '
        Me.GroupBox4.Controls.Add(Me.lblFont)
        Me.GroupBox4.Controls.Add(Me.btnFont)
        Me.GroupBox4.Location = New System.Drawing.Point(6, 176)
        Me.GroupBox4.Name = "GroupBox4"
        Me.GroupBox4.Size = New System.Drawing.Size(304, 62)
        Me.GroupBox4.TabIndex = 41
        Me.GroupBox4.TabStop = False
        Me.GroupBox4.Text = "Display Font"
        Me.GroupBox4.UseCompatibleTextRendering = True
        '
        'lblFont
        '
        Me.lblFont.Location = New System.Drawing.Point(8, 27)
        Me.lblFont.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblFont.Name = "lblFont"
        Me.lblFont.Size = New System.Drawing.Size(182, 29)
        Me.lblFont.TabIndex = 39
        Me.lblFont.Text = "Current font"
        Me.lblFont.UseCompatibleTextRendering = True
        '
        'btnFont
        '
        Me.btnFont.Location = New System.Drawing.Point(208, 21)
        Me.btnFont.Name = "btnFont"
        Me.btnFont.Size = New System.Drawing.Size(89, 29)
        Me.btnFont.TabIndex = 38
        Me.btnFont.Text = "Select"
        Me.btnFont.UseCompatibleTextRendering = True
        Me.btnFont.UseVisualStyleBackColor = True
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.txtAutosaveInterval)
        Me.GroupBox2.Controls.Add(Me.txtAutosaveChanges)
        Me.GroupBox2.Controls.Add(Me.btnAutosaveOpen)
        Me.GroupBox2.Controls.Add(Me.cbxAutosaveCompression)
        Me.GroupBox2.Controls.Add(Me.Label2)
        Me.GroupBox2.Controls.Add(Me.cbxAutosaveEnabled)
        Me.GroupBox2.Controls.Add(Me.Label1)
        Me.GroupBox2.Location = New System.Drawing.Point(6, 63)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(304, 107)
        Me.GroupBox2.TabIndex = 37
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Autosave"
        Me.GroupBox2.UseCompatibleTextRendering = True
        '
        'txtAutosaveInterval
        '
        Me.txtAutosaveInterval.Location = New System.Drawing.Point(140, 74)
        Me.txtAutosaveInterval.Margin = New System.Windows.Forms.Padding(4)
        Me.txtAutosaveInterval.Name = "txtAutosaveInterval"
        Me.txtAutosaveInterval.Size = New System.Drawing.Size(61, 22)
        Me.txtAutosaveInterval.TabIndex = 25
        '
        'txtAutosaveChanges
        '
        Me.txtAutosaveChanges.Location = New System.Drawing.Point(140, 51)
        Me.txtAutosaveChanges.Margin = New System.Windows.Forms.Padding(4)
        Me.txtAutosaveChanges.Name = "txtAutosaveChanges"
        Me.txtAutosaveChanges.Size = New System.Drawing.Size(61, 22)
        Me.txtAutosaveChanges.TabIndex = 22
        '
        'btnAutosaveOpen
        '
        Me.btnAutosaveOpen.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnAutosaveOpen.Location = New System.Drawing.Point(209, 71)
        Me.btnAutosaveOpen.Name = "btnAutosaveOpen"
        Me.btnAutosaveOpen.Size = New System.Drawing.Size(89, 29)
        Me.btnAutosaveOpen.TabIndex = 39
        Me.btnAutosaveOpen.Text = "Open Map"
        Me.btnAutosaveOpen.UseCompatibleTextRendering = True
        Me.btnAutosaveOpen.UseVisualStyleBackColor = True
        '
        'cbxAutosaveCompression
        '
        Me.cbxAutosaveCompression.AutoSize = True
        Me.cbxAutosaveCompression.Location = New System.Drawing.Point(140, 19)
        Me.cbxAutosaveCompression.Margin = New System.Windows.Forms.Padding(4)
        Me.cbxAutosaveCompression.Name = "cbxAutosaveCompression"
        Me.cbxAutosaveCompression.Size = New System.Drawing.Size(130, 21)
        Me.cbxAutosaveCompression.TabIndex = 27
        Me.cbxAutosaveCompression.Text = "Use compression"
        Me.cbxAutosaveCompression.UseCompatibleTextRendering = True
        Me.cbxAutosaveCompression.UseVisualStyleBackColor = True
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(7, 54)
        Me.Label2.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(125, 20)
        Me.Label2.TabIndex = 26
        Me.Label2.Text = "Number of changes:"
        Me.Label2.UseCompatibleTextRendering = True
        '
        'cbxAutosaveEnabled
        '
        Me.cbxAutosaveEnabled.AutoSize = True
        Me.cbxAutosaveEnabled.Location = New System.Drawing.Point(7, 22)
        Me.cbxAutosaveEnabled.Margin = New System.Windows.Forms.Padding(4)
        Me.cbxAutosaveEnabled.Name = "cbxAutosaveEnabled"
        Me.cbxAutosaveEnabled.Size = New System.Drawing.Size(76, 21)
        Me.cbxAutosaveEnabled.TabIndex = 3
        Me.cbxAutosaveEnabled.Text = "Enabled"
        Me.cbxAutosaveEnabled.UseCompatibleTextRendering = True
        Me.cbxAutosaveEnabled.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(7, 74)
        Me.Label1.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(104, 20)
        Me.Label1.TabIndex = 24
        Me.Label1.Text = "Time interval (s):"
        Me.Label1.UseCompatibleTextRendering = True
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.txtUndoSteps)
        Me.GroupBox1.Controls.Add(Me.Label11)
        Me.GroupBox1.Location = New System.Drawing.Point(6, 6)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(304, 51)
        Me.GroupBox1.TabIndex = 36
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Undo"
        Me.GroupBox1.UseCompatibleTextRendering = True
        '
        'txtUndoSteps
        '
        Me.txtUndoSteps.Location = New System.Drawing.Point(158, 15)
        Me.txtUndoSteps.Margin = New System.Windows.Forms.Padding(4)
        Me.txtUndoSteps.Name = "txtUndoSteps"
        Me.txtUndoSteps.Size = New System.Drawing.Size(61, 22)
        Me.txtUndoSteps.TabIndex = 22
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Location = New System.Drawing.Point(7, 18)
        Me.Label11.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(143, 20)
        Me.Label11.TabIndex = 24
        Me.Label11.Text = "Maximum stored steps:"
        Me.Label11.UseCompatibleTextRendering = True
        '
        'TabPage2
        '
        Me.TabPage2.Controls.Add(Me.btnKeyControlChangeDefault)
        Me.TabPage2.Controls.Add(Me.Label7)
        Me.TabPage2.Controls.Add(Me.btnKeyControlChangeUnless)
        Me.TabPage2.Controls.Add(Me.btnKeyControlChange)
        Me.TabPage2.Controls.Add(Me.lstKeyboardControls)
        Me.TabPage2.Location = New System.Drawing.Point(4, 25)
        Me.TabPage2.Name = "TabPage2"
        Me.TabPage2.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage2.Size = New System.Drawing.Size(629, 369)
        Me.TabPage2.TabIndex = 1
        Me.TabPage2.Text = "Keyboard"
        Me.TabPage2.UseVisualStyleBackColor = True
        '
        'btnKeyControlChangeDefault
        '
        Me.btnKeyControlChangeDefault.Location = New System.Drawing.Point(417, 156)
        Me.btnKeyControlChangeDefault.Name = "btnKeyControlChangeDefault"
        Me.btnKeyControlChangeDefault.Size = New System.Drawing.Size(160, 35)
        Me.btnKeyControlChangeDefault.TabIndex = 4
        Me.btnKeyControlChangeDefault.Text = "Set To Default"
        Me.btnKeyControlChangeDefault.UseCompatibleTextRendering = True
        Me.btnKeyControlChangeDefault.UseVisualStyleBackColor = True
        '
        'Label7
        '
        Me.Label7.Location = New System.Drawing.Point(419, 98)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(158, 66)
        Me.Label7.TabIndex = 3
        Me.Label7.Text = "The key combination will be ignored while an ""unless key"" is pressed."
        Me.Label7.UseCompatibleTextRendering = True
        '
        'btnKeyControlChangeUnless
        '
        Me.btnKeyControlChangeUnless.Location = New System.Drawing.Point(417, 59)
        Me.btnKeyControlChangeUnless.Name = "btnKeyControlChangeUnless"
        Me.btnKeyControlChangeUnless.Size = New System.Drawing.Size(160, 35)
        Me.btnKeyControlChangeUnless.TabIndex = 2
        Me.btnKeyControlChangeUnless.Text = "Change Unless Keys"
        Me.btnKeyControlChangeUnless.UseCompatibleTextRendering = True
        Me.btnKeyControlChangeUnless.UseVisualStyleBackColor = True
        '
        'btnKeyControlChange
        '
        Me.btnKeyControlChange.Location = New System.Drawing.Point(417, 18)
        Me.btnKeyControlChange.Name = "btnKeyControlChange"
        Me.btnKeyControlChange.Size = New System.Drawing.Size(160, 35)
        Me.btnKeyControlChange.TabIndex = 1
        Me.btnKeyControlChange.Text = "Change Keys"
        Me.btnKeyControlChange.UseCompatibleTextRendering = True
        Me.btnKeyControlChange.UseVisualStyleBackColor = True
        '
        'lstKeyboardControls
        '
        Me.lstKeyboardControls.FormattingEnabled = True
        Me.lstKeyboardControls.ItemHeight = 16
        Me.lstKeyboardControls.Location = New System.Drawing.Point(17, 18)
        Me.lstKeyboardControls.Name = "lstKeyboardControls"
        Me.lstKeyboardControls.ScrollAlwaysVisible = True
        Me.lstKeyboardControls.Size = New System.Drawing.Size(394, 324)
        Me.lstKeyboardControls.TabIndex = 0
        '
        'btnCancel
        '
        Me.btnCancel.Location = New System.Drawing.Point(549, 416)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(100, 29)
        Me.btnCancel.TabIndex = 39
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.UseCompatibleTextRendering = True
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'btnSave
        '
        Me.btnSave.Location = New System.Drawing.Point(443, 416)
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(100, 29)
        Me.btnSave.TabIndex = 40
        Me.btnSave.Text = "Accept"
        Me.btnSave.UseCompatibleTextRendering = True
        Me.btnSave.UseVisualStyleBackColor = True
        '
        'frmOptions
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.ClientSize = New System.Drawing.Size(659, 452)
        Me.Controls.Add(Me.btnSave)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.TabControl1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmOptions"
        Me.Text = "Options"
        Me.TabControl1.ResumeLayout(False)
        Me.TabPage3.ResumeLayout(False)
        Me.TabPage3.PerformLayout()
        Me.TabPage1.ResumeLayout(False)
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox3.PerformLayout()
        Me.GroupBox8.ResumeLayout(False)
        Me.GroupBox8.PerformLayout()
        Me.GroupBox7.ResumeLayout(False)
        Me.GroupBox7.PerformLayout()
        Me.GroupBox6.ResumeLayout(False)
        Me.GroupBox6.PerformLayout()
        Me.GroupBox5.ResumeLayout(False)
        Me.GroupBox5.PerformLayout()
        Me.GroupBox4.ResumeLayout(False)
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.TabPage2.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Public WithEvents TabControl1 As System.Windows.Forms.TabControl
    Public WithEvents TabPage1 As System.Windows.Forms.TabPage
    Public WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Public WithEvents Label11 As System.Windows.Forms.Label
    Public WithEvents txtUndoSteps As System.Windows.Forms.TextBox
    Public WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Public WithEvents cbxAutosaveCompression As System.Windows.Forms.CheckBox
    Public WithEvents Label2 As System.Windows.Forms.Label
    Public WithEvents txtAutosaveInterval As System.Windows.Forms.TextBox
    Public WithEvents cbxAutosaveEnabled As System.Windows.Forms.CheckBox
    Public WithEvents Label1 As System.Windows.Forms.Label
    Public WithEvents txtAutosaveChanges As System.Windows.Forms.TextBox
    Public WithEvents GroupBox4 As System.Windows.Forms.GroupBox
    Public WithEvents lblFont As System.Windows.Forms.Label
    Public WithEvents btnFont As System.Windows.Forms.Button
    Public WithEvents cbxAskDirectories As System.Windows.Forms.CheckBox
    Public WithEvents btnCancel As System.Windows.Forms.Button
    Public WithEvents btnSave As System.Windows.Forms.Button
    Public WithEvents GroupBox5 As System.Windows.Forms.GroupBox
    Public WithEvents cbxMinimapObjectColours As System.Windows.Forms.CheckBox
    Public WithEvents Label3 As System.Windows.Forms.Label
    Public WithEvents txtMinimapSize As System.Windows.Forms.TextBox
    Public WithEvents GroupBox7 As System.Windows.Forms.GroupBox
    Public WithEvents Label4 As System.Windows.Forms.Label
    Public WithEvents txtFOV As System.Windows.Forms.TextBox
    Public WithEvents GroupBox6 As System.Windows.Forms.GroupBox
    Public WithEvents cbxPointerDirect As System.Windows.Forms.CheckBox
    Public WithEvents btnAutosaveOpen As System.Windows.Forms.Button
    Public WithEvents cbxMinimapTeamColourFeatures As System.Windows.Forms.CheckBox
    Friend WithEvents pnlMinimapCliffColour As System.Windows.Forms.Panel
    Public WithEvents Label5 As System.Windows.Forms.Label
    Public WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents pnlMinimapSelectedObjectColour As System.Windows.Forms.Panel
    Public WithEvents GroupBox8 As System.Windows.Forms.GroupBox
    Public WithEvents cbxMipmapsHardware As System.Windows.Forms.CheckBox
    Public WithEvents cbxMipmaps As System.Windows.Forms.CheckBox
    Friend WithEvents TabPage2 As System.Windows.Forms.TabPage
    Friend WithEvents lstKeyboardControls As System.Windows.Forms.ListBox
    Friend WithEvents btnKeyControlChange As System.Windows.Forms.Button
    Friend WithEvents btnKeyControlChangeUnless As System.Windows.Forms.Button
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents btnKeyControlChangeDefault As System.Windows.Forms.Button
    Friend WithEvents TabPage3 As System.Windows.Forms.TabPage
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Public WithEvents Label13 As System.Windows.Forms.Label
    Public WithEvents txtTexturesDepth As System.Windows.Forms.TextBox
    Public WithEvents txtTexturesBPP As System.Windows.Forms.TextBox
    Public WithEvents Label10 As System.Windows.Forms.Label
    Public WithEvents txtMapDepth As System.Windows.Forms.TextBox
    Public WithEvents txtMapBPP As System.Windows.Forms.TextBox
    Public WithEvents Label8 As System.Windows.Forms.Label
    Public WithEvents Label9 As System.Windows.Forms.Label
    Public WithEvents GroupBox3 As System.Windows.Forms.GroupBox
    Public WithEvents cbxPickerOrientation As System.Windows.Forms.CheckBox
    Public WithEvents Label12 As System.Windows.Forms.Label
End Class
