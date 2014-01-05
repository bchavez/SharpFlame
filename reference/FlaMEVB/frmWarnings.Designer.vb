<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmWarnings
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
        Me.tvwWarnings = New System.Windows.Forms.TreeView()
        Me.SuspendLayout()
        '
        'tvwWarnings
        '
        Me.tvwWarnings.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tvwWarnings.Location = New System.Drawing.Point(0, 0)
        Me.tvwWarnings.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.tvwWarnings.Name = "tvwWarnings"
        Me.tvwWarnings.Size = New System.Drawing.Size(429, 247)
        Me.tvwWarnings.TabIndex = 0
        '
        'frmWarnings
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(429, 247)
        Me.Controls.Add(Me.tvwWarnings)
        Me.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.Name = "frmWarnings"
        Me.TopMost = True
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents tvwWarnings As System.Windows.Forms.TreeView
End Class
