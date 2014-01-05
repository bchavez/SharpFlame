<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ctrlColour
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
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
        Me.pnlColour = New System.Windows.Forms.Panel()
        Me.nudAlpha = New System.Windows.Forms.NumericUpDown()
        CType(Me.nudAlpha, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'pnlColour
        '
        Me.pnlColour.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.pnlColour.Location = New System.Drawing.Point(0, 0)
        Me.pnlColour.Margin = New System.Windows.Forms.Padding(0)
        Me.pnlColour.Name = "pnlColour"
        Me.pnlColour.Size = New System.Drawing.Size(51, 24)
        Me.pnlColour.TabIndex = 1
        '
        'nudAlpha
        '
        Me.nudAlpha.DecimalPlaces = 2
        Me.nudAlpha.Increment = New Decimal(New Integer() {1, 0, 0, 65536})
        Me.nudAlpha.Location = New System.Drawing.Point(54, 0)
        Me.nudAlpha.Maximum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.nudAlpha.Name = "nudAlpha"
        Me.nudAlpha.Size = New System.Drawing.Size(50, 22)
        Me.nudAlpha.TabIndex = 2
        Me.nudAlpha.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'ctrlColour
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.Controls.Add(Me.nudAlpha)
        Me.Controls.Add(Me.pnlColour)
        Me.Margin = New System.Windows.Forms.Padding(0)
        Me.Name = "ctrlColour"
        Me.Size = New System.Drawing.Size(211, 39)
        CType(Me.nudAlpha, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents pnlColour As System.Windows.Forms.Panel
    Friend WithEvents nudAlpha As System.Windows.Forms.NumericUpDown
End Class
