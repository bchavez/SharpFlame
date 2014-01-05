<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmWZLoad
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
        Me.lstMap = New System.Windows.Forms.ListBox()
        Me.SuspendLayout()
        '
        'lstMap
        '
        Me.lstMap.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lstMap.FormattingEnabled = True
        Me.lstMap.ItemHeight = 16
        Me.lstMap.Location = New System.Drawing.Point(0, 0)
        Me.lstMap.Margin = New System.Windows.Forms.Padding(4)
        Me.lstMap.Name = "lstMap"
        Me.lstMap.Size = New System.Drawing.Size(619, 315)
        Me.lstMap.TabIndex = 1
        '
        'frmWZLoad
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(619, 315)
        Me.Controls.Add(Me.lstMap)
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.Name = "frmWZLoad"
        Me.Text = "frmWZLoad"
        Me.ResumeLayout(False)

    End Sub
    Public WithEvents lstMap As System.Windows.Forms.ListBox
End Class
