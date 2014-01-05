<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ctrlPlayerNum
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
        Me.tsPlayerNum1 = New System.Windows.Forms.ToolStrip()
        Me.tsPlayerNum2 = New System.Windows.Forms.ToolStrip()
        Me.SuspendLayout()
        '
        'tsPlayerNum1
        '
        Me.tsPlayerNum1.Font = New System.Drawing.Font("Microsoft Sans Serif", 7.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tsPlayerNum1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.tsPlayerNum1.Location = New System.Drawing.Point(0, 0)
        Me.tsPlayerNum1.Name = "tsPlayerNum1"
        Me.tsPlayerNum1.Size = New System.Drawing.Size(56, 25)
        Me.tsPlayerNum1.TabIndex = 0
        Me.tsPlayerNum1.Text = "ToolStrip1"
        '
        'tsPlayerNum2
        '
        Me.tsPlayerNum2.Font = New System.Drawing.Font("Microsoft Sans Serif", 7.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tsPlayerNum2.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.tsPlayerNum2.Location = New System.Drawing.Point(0, 25)
        Me.tsPlayerNum2.Name = "tsPlayerNum2"
        Me.tsPlayerNum2.Size = New System.Drawing.Size(56, 25)
        Me.tsPlayerNum2.TabIndex = 1
        Me.tsPlayerNum2.Text = "ToolStrip1"
        '
        'ctrlPlayerNum
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.Controls.Add(Me.tsPlayerNum2)
        Me.Controls.Add(Me.tsPlayerNum1)
        Me.Name = "ctrlPlayerNum"
        Me.Size = New System.Drawing.Size(56, 50)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Public WithEvents tsPlayerNum1 As System.Windows.Forms.ToolStrip
    Public WithEvents tsPlayerNum2 As System.Windows.Forms.ToolStrip

End Class
