<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class MainForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        TestVonLongRunTaskBundlesButton = New Button()
        SuspendLayout()
        ' 
        ' TestVonLongRunTaskBundlesButton
        ' 
        TestVonLongRunTaskBundlesButton.Location = New Point(47, 47)
        TestVonLongRunTaskBundlesButton.Name = "TestVonLongRunTaskBundlesButton"
        TestVonLongRunTaskBundlesButton.Size = New Size(233, 23)
        TestVonLongRunTaskBundlesButton.TabIndex = 0
        TestVonLongRunTaskBundlesButton.Text = "Load test tasks"
        TestVonLongRunTaskBundlesButton.UseVisualStyleBackColor = True
        ' 
        ' MainForm
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(372, 127)
        Controls.Add(TestVonLongRunTaskBundlesButton)
        Name = "MainForm"
        Text = "Task-Management Demo App"
        ResumeLayout(False)
    End Sub

    Friend WithEvents TestVonLongRunTaskBundlesButton As Button

End Class
