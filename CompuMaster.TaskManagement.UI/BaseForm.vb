<DebuggerStepThrough> Public Class BaseForm
    Inherits System.Windows.Forms.Form

    Public Shared Property DefaultIcon As Icon

    Private Sub BierdeckelForm_Load(sender As Object, e As EventArgs) Handles Me.Load
        'Use always the same application icon
        Me.Icon = DefaultIcon
        'If not defined separately, use the size of the form as minimum window size
        If Me.DesignMode = False AndAlso Me.MinimumSize = Nothing Then
            Me.MinimumSize = Me.Size
        End If
    End Sub

End Class
