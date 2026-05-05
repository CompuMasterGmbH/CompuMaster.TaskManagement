Imports CompuMaster.TaskManagement.Exceptions

''' <summary>
''' Provides Windows Forms utility methods.
''' </summary>
Public Module UITools

    ''' <summary>
    ''' Represents an action that can be run by <see cref="TryRun(Form, TryRunMethod, TryRunSwitchWaitCursorMethod)"/>.
    ''' </summary>
    Public Delegate Sub TryRunMethod()

    ''' <summary>
    ''' Represents an action that switches the wait cursor state.
    ''' </summary>
    ''' <param name="enableWaitCursor">A value indicating whether the wait cursor shall be enabled.</param>
    Public Delegate Sub TryRunSwitchWaitCursorMethod(enableWaitCursor As Boolean)

    ''' <summary>
    ''' Represents an action that handles a caught exception.
    ''' </summary>
    ''' <param name="ex">The caught exception.</param>
    Public Delegate Sub TryRunCatchedExceptionMethod(ex As Exception)

    ''' <summary>
    ''' Runs an action with cursor and exception handling.
    ''' </summary>
    ''' <param name="errorMessageBoxOwner">The owner form for error message boxes.</param>
    ''' <param name="action">The action to run.</param>
    ''' <param name="switchCursorAction">The action that switches the wait cursor state.</param>
    ''' <returns><see langword="True"/> if the action succeeded; otherwise, <see langword="False"/>.</returns>
    Public Function TryRun(errorMessageBoxOwner As Form, action As TryRunMethod, switchCursorAction As TryRunSwitchWaitCursorMethod) As Boolean
        Return TryRun(errorMessageBoxOwner, action, Nothing, switchCursorAction)
    End Function

    ''' <summary>
    ''' Runs an action with cursor, exception handling, and a caught-exception callback.
    ''' </summary>
    ''' <param name="errorMessageBoxOwner">The owner form for error message boxes.</param>
    ''' <param name="action">The action to run.</param>
    ''' <param name="catchedExceptionAction">The action to call when an exception is caught.</param>
    ''' <param name="switchCursorAction">The action that switches the wait cursor state.</param>
    ''' <returns><see langword="True"/> if the action succeeded; otherwise, <see langword="False"/>.</returns>
    Public Function TryRun(errorMessageBoxOwner As Form, action As TryRunMethod, catchedExceptionAction As TryRunCatchedExceptionMethod, switchCursorAction As TryRunSwitchWaitCursorMethod) As Boolean
        Try
            If switchCursorAction IsNot Nothing Then switchCursorAction(True)
            action()
            Return True
        Catch ex As System.IO.FileNotFoundException
            If switchCursorAction IsNot Nothing Then switchCursorAction(False)
            If catchedExceptionAction IsNot Nothing Then catchedExceptionAction(ex)
            MessageBox.Show(errorMessageBoxOwner, ex.Message, errorMessageBoxOwner.Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Catch ex As System.MissingMemberException
            'Usually COM interop issues
            If switchCursorAction IsNot Nothing Then switchCursorAction(False)
            If catchedExceptionAction IsNot Nothing Then catchedExceptionAction(ex)
            MessageBox.Show(errorMessageBoxOwner, "ERROR: " & ex.ToString, errorMessageBoxOwner.Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
            'Catch ex As UserInfoMessageException
            '    If switchCursorAction IsNot Nothing Then switchCursorAction(False)
            '    If catchedExceptionAction IsNot Nothing Then catchedExceptionAction(ex)
            '    MessageBox.Show(errorMessageBoxOwner, ex.Message, errorMessageBoxOwner.Text, MessageBoxButtons.OK, MessageBoxIcon.Information)
            'Catch ex As UserWarningMessageException
            '    If switchCursorAction IsNot Nothing Then switchCursorAction(False)
            '    If catchedExceptionAction IsNot Nothing Then catchedExceptionAction(ex)
            '    MessageBox.Show(errorMessageBoxOwner, ex.Message, errorMessageBoxOwner.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            'Catch ex As UserMessageException
            '    If switchCursorAction IsNot Nothing Then switchCursorAction(False)
            '    If catchedExceptionAction IsNot Nothing Then catchedExceptionAction(ex)
            '    MessageBox.Show(errorMessageBoxOwner, ex.Message, errorMessageBoxOwner.Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Catch ex As UserAbortedMessageException
            If switchCursorAction IsNot Nothing Then switchCursorAction(False)
            If catchedExceptionAction IsNot Nothing Then catchedExceptionAction(ex)
            MessageBox.Show(errorMessageBoxOwner, ex.Message, errorMessageBoxOwner.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        Catch ex As Exception
            Stop
            If switchCursorAction IsNot Nothing Then switchCursorAction(False)
            If catchedExceptionAction IsNot Nothing Then catchedExceptionAction(ex)
            MessageBox.Show(errorMessageBoxOwner, "ERROR: " & ex.ToString, errorMessageBoxOwner.Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If switchCursorAction IsNot Nothing Then switchCursorAction(False)
        End Try
        Return False
    End Function

End Module
