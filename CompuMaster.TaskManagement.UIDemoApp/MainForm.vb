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

                   'Run tasks which doesn't use result data
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

                   'Run tasks which provide result data and which shall be displayed at GUI
                   Dim TaskBundleResult As DateTime
                   Dim tb = DummyTaskBundles.DummyTaskBundleProvidingSomeResultData(Sub() TaskBundleResult = Now)
                   f = New MultiTaskProgessForm(tb)
                   f.AutoStartTaskBundleRunner = True
                   f.AutoCloseFormOnTaskBundleCompleted = True
                   f.AllowCancellation = False
                   f.OnTaskBundleCompletedActions.Add(Sub()
                                                          Dim f2 As New Form
                                                          f2.Text = "Results of " & tb.TaskBundleTitle
                                                          f2.Controls.Add(New Label() With {.Dock = DockStyle.Fill, .TextAlign = ContentAlignment.MiddleCenter, .Text = "Task bundle completed on " & TaskBundleResult.ToString})
                                                          f2.Show()
                                                      End Sub)
                   f.Show()
               End Sub, False, False)
    End Sub

End Class
