Imports CompuMaster.TaskManagement
Imports CompuMaster.TaskManagement.Exceptions
Imports CompuMaster.TaskManagement.UI.WinForms

Public Class MainForm

    ''' <summary>
    ''' Provide a safe pattern for correct cursor handling and exception handling
    ''' </summary>
    ''' <param name="action"></param>
    ''' <param name="switchCursor"></param>
    Private Sub TryRun(action As TryRunMethod, switchCursor As Boolean, requiresBereichSelection As Boolean)
        Try
            If switchCursor Then SwitchWaitCursor(True)
            action()
        Catch ex As UserAbortedMessageException
            If switchCursor Then SwitchWaitCursor(False)
            MessageBox.Show(Me, ex.Message, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        Catch ex As Exception
            Stop
            If switchCursor Then SwitchWaitCursor(False)
            MessageBox.Show(Me, "ERROR: " & ex.ToString, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If switchCursor Then SwitchWaitCursor(False)
        End Try
    End Sub

    Private Sub SwitchWaitCursor(showWaitCursor As Boolean)
        If showWaitCursor Then
            Me.UseWaitCursor = True
            Me.Cursor = Cursors.WaitCursor
            Cursor.Current = Cursors.WaitCursor
            Me.Refresh()
        Else
            Me.UseWaitCursor = False
            Cursor.Current = Cursors.Default
            Me.Cursor = Cursors.Default
            'Me.Refresh()
        End If
    End Sub


    Private Sub TestVonLongRunTaskBundlesButton_Click(sender As Object, e As EventArgs) Handles TestVonLongRunTaskBundlesButton.Click
        TryRun(Sub()
                   Dim f As MultiTaskProgessForm
                   f = New MultiTaskProgessForm(DummyTaskBundles.DummyTaskBundleFailingWithRollback)
                   f.Show()
                   f = New MultiTaskProgessForm(DummyTaskBundles.DummyTaskBundleFailingWithRollbackFailingWithThrownException)
                   f.Show()
                   f = New MultiTaskProgessForm(DummyTaskBundles.DummyTaskBundleFailingWithRollbackFailingWithLoggedException)
                   f.Show()
                   f = New MultiTaskProgessForm(DummyTaskBundles.DummyTaskBundleFailingInCriticalStepsWithoutRollbackPossibility)
                   f.Show()
                   f = New MultiTaskProgessForm(DummyTaskBundles.DummyTaskBundleSuccessful)
                   f.Show()
               End Sub, False, False)
    End Sub

End Class
