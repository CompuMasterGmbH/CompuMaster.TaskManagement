<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ProgressingTaskControl
    Inherits System.Windows.Forms.UserControl

    'UserControl überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Wird vom Windows Form-Designer benötigt.
    Private components As System.ComponentModel.IContainer

    'Hinweis: Die folgende Prozedur ist für den Windows Form-Designer erforderlich.
    'Das Bearbeiten ist mit dem Windows Form-Designer möglich.  
    'Das Bearbeiten mit dem Code-Editor ist nicht möglich.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        GroupBox = New GroupBox()
        ProgressBar = New ProgressBar()
        LabelStepInfo = New Label()
        GroupBox.SuspendLayout()
        SuspendLayout()
        ' 
        ' GroupBox
        ' 
        GroupBox.Controls.Add(ProgressBar)
        GroupBox.Controls.Add(LabelStepInfo)
        GroupBox.Dock = DockStyle.Fill
        GroupBox.Location = New Point(0, 0)
        GroupBox.Name = "GroupBox"
        GroupBox.Size = New Size(544, 150)
        GroupBox.TabIndex = 1
        GroupBox.TabStop = False
        GroupBox.Text = "GroupBox"
        ' 
        ' ProgressBar
        ' 
        ProgressBar.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        ProgressBar.Location = New Point(19, 33)
        ProgressBar.Name = "ProgressBar"
        ProgressBar.Size = New Size(504, 29)
        ProgressBar.TabIndex = 2
        ' 
        ' LabelStepInfo
        ' 
        LabelStepInfo.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        LabelStepInfo.Location = New Point(19, 74)
        LabelStepInfo.Name = "LabelStepInfo"
        LabelStepInfo.Size = New Size(504, 60)
        LabelStepInfo.TabIndex = 1
        LabelStepInfo.Text = "Label1"
        ' 
        ' ProgressingTaskControl
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        Controls.Add(GroupBox)
        Name = "ProgressingTaskControl"
        Size = New Size(544, 150)
        GroupBox.ResumeLayout(False)
        ResumeLayout(False)
    End Sub

    Friend WithEvents GroupBox As GroupBox
    Friend WithEvents ProgressBar As ProgressBar
    Friend WithEvents LabelStepInfo As Label

End Class
