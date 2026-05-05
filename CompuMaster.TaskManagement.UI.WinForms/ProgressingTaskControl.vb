''' <summary>
''' Represents a Windows Forms control that displays task progress.
''' </summary>
Public Class ProgressingTaskControl

    ''' <inheritdoc/>
    Public Overrides Property Text As String
        Get
            Return Me.GroupBox.Text
        End Get
        Set(value As String)
            Me.GroupBox.Text = value
        End Set
    End Property

    ''' <inheritdoc/>
    Public Overrides Function ToString() As String
        Return NameOf(ProgressingTaskControl) & ": " & Me.Text
    End Function

End Class
