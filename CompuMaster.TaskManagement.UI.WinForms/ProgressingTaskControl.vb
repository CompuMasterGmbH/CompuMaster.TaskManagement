Public Class ProgressingTaskControl

    Public Overrides Property Text As String
        Get
            Return Me.GroupBox.Text
        End Get
        Set(value As String)
            Me.GroupBox.Text = value
        End Set
    End Property

    Public Overrides Function ToString() As String
        Return NameOf(ProgressingTaskControl) & ": " & Me.Text
    End Function

End Class
