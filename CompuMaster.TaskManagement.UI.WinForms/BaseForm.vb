''' <summary>
''' Provides a Windows Forms base form with a shared default icon.
''' </summary>
<DebuggerStepThrough>
Public Class BaseForm
    Inherits System.Windows.Forms.Form

    ''' <summary>
    ''' Gets or sets the default icon assigned to forms derived from this base form.
    ''' </summary>
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
