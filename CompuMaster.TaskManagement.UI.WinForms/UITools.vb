Imports CompuMaster.TaskManagement.Exceptions

Public Module UITools

    Public Delegate Sub TryRunMethod()
    Public Delegate Sub TryRunSwitchWaitCursorMethod(enableWaitCursor As Boolean)
    Public Delegate Sub TryRunCatchedExceptionMethod(ex As Exception)

    ''' <summary>
    ''' Provide a safe pattern for correct cursor handling and exception handling
    ''' </summary>
    ''' <param name="action"></param>
    ''' <param name="switchCursorAction"></param>
    ''' <returns>True if successful, False if exceptions occured (and have been shown in a message box to user)</returns>
    Public Function TryRun(errorMessageBoxOwner As Form, action As TryRunMethod, switchCursorAction As TryRunSwitchWaitCursorMethod) As Boolean
        Return TryRun(errorMessageBoxOwner, action, Nothing, switchCursorAction)
    End Function

    ''' <summary>
    ''' Provide a safe pattern for correct cursor handling and exception handling
    ''' </summary>
    ''' <param name="action"></param>
    ''' <param name="switchCursorAction"></param>
    ''' <returns>True if successful, False if exceptions occured (and have been shown in a message box to user)</returns>
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
