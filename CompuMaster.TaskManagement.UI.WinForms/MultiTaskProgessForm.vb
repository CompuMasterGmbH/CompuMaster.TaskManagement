Option Strict On
Option Explicit On

Imports System.ComponentModel

Public Class MultiTaskProgessForm

    <System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>
    <Obsolete("Use overloaded constructor", True)>
    Public Sub New()
        InitializeComponent()
        If Me.DesignMode Then
            'Add 2 additional progress bars
            AddAdditionalProgressBar()
            AddAdditionalProgressBar()
        End If
    End Sub

    Public Sub New(taskBundle As ProgressingTaskBundle)
        If taskBundle Is Nothing OrElse taskBundle.Tasks.Count = 0 Then
            Throw New ArgumentNullException(NameOf(taskBundle), "No tasks to run")
        End If
        InitializeComponent()
        Me.TaskBundle = taskBundle
    End Sub

    Public Property TaskBundle As ProgressingTaskBundle

    Public Property AutoStartTaskBundleRunner As Boolean

    Private Sub MultiTaskProgessForm_Load(sender As Object, e As EventArgs) Handles Me.Load
        RefreshAllControls()
        If AutoStartTaskBundleRunner Then
            Me.ButtonStart_Click(sender, e)
        End If
    End Sub

    Public Sub RefreshAllControls()
        RefreshVisibilityAndStatusOfProgressbars()
        RefreshButtonStatus()
        RefreshFormTitle()
    End Sub

    Private Sub RefreshFormTitle()
        Me.Text = Me.TaskBundle.DisplayTitleAndStatusAndETA
    End Sub

    Public Sub RefreshButtonStatus()
        Me.ButtonStart.Visible = Me.TaskBundle.Status = ProgressingTaskBundle.ProgressingTaskBundleStatus.NotStarted
        Me.ButtonCancel.Visible = Me.TaskBundle.IsRunningOrFailing
        Me.ButtonCancel.Enabled = Not Me.TaskBundle.CancellationTokenSource.Token.IsCancellationRequested
        Me.ButtonClose.Visible = Not (Me.ButtonStart.Visible OrElse Me.ButtonCancel.Visible)
    End Sub

    Public Sub RefreshVisibilityAndStatusOfProgressbars()
        For MyCounter As Integer = 0 To System.Math.Max(Me.FlowLayoutPanelForProgressingTasks.Controls.Count, Me.TaskBundle.Tasks.Count) - 1
            If MyCounter >= Me.FlowLayoutPanelForProgressingTasks.Controls.Count Then
                AddAdditionalProgressBar()
            ElseIf MyCounter >= Me.TaskBundle.Tasks.Count Then
                Me.FlowLayoutPanelForProgressingTasks.Controls(MyCounter).Visible = False
                Continue For
            End If
            Dim CurrentTask As ProgressingTaskItem = Me.TaskBundle.Tasks(MyCounter)
            UpdateProgressBar(CType(Me.FlowLayoutPanelForProgressingTasks.Controls(MyCounter), ProgressingTaskControl), CurrentTask)
        Next
    End Sub

    Private Sub AddAdditionalProgressBar()
        Dim Prototype As ProgressingTaskControl = Me.ProgressBar1
        Dim NewControl As New ProgressingTaskControl
        NewControl.Anchor = Prototype.Anchor
        NewControl.Location = Prototype.Location
        NewControl.Name = NameOf(ProgressingTaskControl) & FlowLayoutPanelForProgressingTasks.Controls.Count
        NewControl.Size = Prototype.Size
        NewControl.TabIndex = Prototype.TabIndex
        FlowLayoutPanelForProgressingTasks.Controls.Add(NewControl)
    End Sub

    Private Enum TaskResultToolTipType
        ProgressErrors
        ProgressInformation
    End Enum

    Private Sub SetToolTipForTaskControl(control As ProgressingTaskControl, text As String, toolTipType As TaskResultToolTipType)
        Dim SetToolTipProgressErrors As Action(Of String) = Sub(t)
                                                                If Me.ToolTipProgressErrors.GetToolTip(control.GroupBox) <> t Then
                                                                    Me.ToolTipProgressErrors.SetToolTip(control.GroupBox, t)
                                                                    Me.ToolTipProgressErrors.SetToolTip(control.ProgressBar, t)
                                                                    Me.ToolTipProgressErrors.SetToolTip(control.LabelStepInfo, t)
                                                                End If
                                                            End Sub
        Dim SetToolTipProgressInfo As Action(Of String) = Sub(t)
                                                              If Me.ToolTipProgressInfo.GetToolTip(control.GroupBox) <> t Then
                                                                  Me.ToolTipProgressInfo.SetToolTip(control.GroupBox, t)
                                                                  Me.ToolTipProgressInfo.SetToolTip(control.ProgressBar, t)
                                                                  Me.ToolTipProgressInfo.SetToolTip(control.LabelStepInfo, t)
                                                              End If
                                                          End Sub

        Select Case toolTipType
            Case TaskResultToolTipType.ProgressErrors
                SetToolTipProgressErrors(text)
                SetToolTipProgressInfo(Nothing)
            Case TaskResultToolTipType.ProgressInformation
                SetToolTipProgressErrors(Nothing)
                SetToolTipProgressInfo(text)
            Case Else
                Throw New NotSupportedException("Unknown type: " & toolTipType.ToString)
        End Select
    End Sub

    Private Sub UpdateProgressBar(control As ProgressingTaskControl, taskItem As ProgressingTaskItem)
        Dim ProgressMaximum As Integer = taskItem.TotalStepsCount * 100
        Dim StepProgressValue As Integer = If(taskItem.RunningStep?.EstimatedTimeToRun.HasValue AndAlso taskItem.RunningStep?.ConsumedTime.HasValue, System.Math.Min(100, CInt(taskItem.RunningStep.ConsumedTime.Value.TotalSeconds / taskItem.RunningStep.EstimatedTimeToRun.Value.TotalSeconds * 100)), 0)
        Dim StepProgressValueText As String = If(taskItem.RunningStep?.EstimatedTimeToRun.HasValue, " (ca. " & StepProgressValue.ToString(System.Globalization.CultureInfo.InvariantCulture) & " %)", "")
        Dim NewProgressValue As Integer = System.Math.Min(ProgressMaximum, taskItem.RunningTotalStepNumber.GetValueOrDefault * 100 + StepProgressValue)
        control.Visible = True
        control.GroupBox.Text = taskItem.TaskTitle
        control.ProgressBar.Maximum = ProgressMaximum
        control.ProgressBar.Value = NewProgressValue
        Select Case taskItem.Status
            Case ProgressingTaskItem.ProgressingTaskStatus.NotStarted
                control.LabelStepInfo.Text = "Noch nicht gestartet, insgesamt " & taskItem.TotalStepsCount & " Schritte"
                If taskItem.EstimatedTimeOfArrival.HasValue Then
                    control.LabelStepInfo.Text &= System.Environment.NewLine & "ETA: " & taskItem.EstimatedTimeOfArrival.Value.ToString("d\.hh\:mm\:ss", System.Globalization.CultureInfo.InvariantCulture) & ""
                End If
                SetToolTipForTaskControl(control, taskItem.SummaryText, TaskResultToolTipType.ProgressInformation)
            Case ProgressingTaskItem.ProgressingTaskStatus.InProgress
                control.LabelStepInfo.Text = "In Bearbeitung: Schritt " & taskItem.RunningTotalStepNumber.Value & "/" & taskItem.TotalStepsCount & ": " & taskItem.RunningStep.StepTitle & StepProgressValueText
                SetToolTipForTaskControl(control, taskItem.SummaryText, TaskResultToolTipType.ProgressInformation)
                If taskItem.EstimatedTimeOfArrival.HasValue Then
                    control.LabelStepInfo.Text &= System.Environment.NewLine & "ETA: " & taskItem.EstimatedTimeOfArrival.Value.ToString("d\.hh\:mm\:ss", System.Globalization.CultureInfo.InvariantCulture) & ""
                End If
            Case ProgressingTaskItem.ProgressingTaskStatus.FailingInCriticalState
                control.LabelStepInfo.Text = "Fehlschlag erkannt, aktuell in Bearbeitung: Schritt " & taskItem.RunningTotalStepNumber.Value & "/" & taskItem.TotalStepsCount & ": " & taskItem.RunningStep.StepTitle & StepProgressValueText
                SetToolTipForTaskControl(control, taskItem.SummaryText, TaskResultToolTipType.ProgressErrors)
                CType(control, ProgressingTaskControl).GroupBox.BackColor = Color.Red
                If taskItem.EstimatedTimeOfArrival.HasValue Then
                    control.LabelStepInfo.Text &= System.Environment.NewLine & "ETA: " & taskItem.EstimatedTimeOfArrival.Value.ToString("d\.hh\:mm\:ss", System.Globalization.CultureInfo.InvariantCulture) & ""
                End If
            Case ProgressingTaskItem.ProgressingTaskStatus.FailingWithRollbackOption
                control.LabelStepInfo.Text = "Fehlschlag erkannt, aktuell in Bearbeitung: Schritt " & taskItem.RunningTotalStepNumber.Value & "/" & taskItem.TotalStepsCount & ": " & taskItem.RunningStep.StepTitle & StepProgressValueText
                SetToolTipForTaskControl(control, taskItem.SummaryText, TaskResultToolTipType.ProgressErrors)
                CType(control, ProgressingTaskControl).GroupBox.BackColor = Color.Yellow
            Case ProgressingTaskItem.ProgressingTaskStatus.Aborting
                control.LabelStepInfo.Text = "Abbruch angefordert, aktuell in Bearbeitung: Schritt " & taskItem.RunningTotalStepNumber.Value & "/" & taskItem.TotalStepsCount & ": " & taskItem.RunningStep.StepTitle & StepProgressValueText
                SetToolTipForTaskControl(control, taskItem.SummaryText, TaskResultToolTipType.ProgressErrors)
                CType(control, ProgressingTaskControl).GroupBox.BackColor = Color.Yellow
            Case ProgressingTaskItem.ProgressingTaskStatus.Completed
                control.LabelStepInfo.Text = "Erfolgreich abgeschlossen in " & taskItem.ConsumedTime.Value.ToString("d\.hh\:mm\:ss", System.Globalization.CultureInfo.InvariantCulture) & " (d.hh:min:sec)"
                SetToolTipForTaskControl(control, taskItem.SummaryText, TaskResultToolTipType.ProgressInformation)
                CType(control, ProgressingTaskControl).GroupBox.BackColor = Color.Green
            Case ProgressingTaskItem.ProgressingTaskStatus.FailedWithRollbackOption
                control.LabelStepInfo.Text = "Fehlgeschlagen nach " & taskItem.ConsumedTime.Value.ToString("d\.hh\:mm\:ss", System.Globalization.CultureInfo.InvariantCulture) & " (d.hh:min:sec): Schritt " & taskItem.RunningTotalStepNumber.Value & "/" & taskItem.TotalStepsCount & ": " & taskItem.RunningStep.StepTitle
                SetToolTipForTaskControl(control, taskItem.SummaryText, TaskResultToolTipType.ProgressErrors)
                CType(control, ProgressingTaskControl).GroupBox.BackColor = Color.Yellow
                control.ProgressBar.Value = control.ProgressBar.Maximum 'stops the animation effect
                control.ProgressBar.Value = NewProgressValue
            Case ProgressingTaskItem.ProgressingTaskStatus.FailedInCriticalState
                control.LabelStepInfo.Text = "Fehlgeschlagen nach " & taskItem.ConsumedTime.Value.ToString("d\.hh\:mm\:ss", System.Globalization.CultureInfo.InvariantCulture) & " (d.hh:min:sec): Schritt " & taskItem.RunningTotalStepNumber.Value & "/" & taskItem.TotalStepsCount & ": " & taskItem.RunningStep.StepTitle
                SetToolTipForTaskControl(control, taskItem.SummaryText, TaskResultToolTipType.ProgressErrors)
                CType(control, ProgressingTaskControl).GroupBox.BackColor = Color.Red
                control.ProgressBar.Value = control.ProgressBar.Maximum 'stops the animation effect
                control.ProgressBar.Value = NewProgressValue
            Case ProgressingTaskItem.ProgressingTaskStatus.Skipped
                control.LabelStepInfo.Text = "Übersprungen"
                SetToolTipForTaskControl(control, taskItem.SummaryText, TaskResultToolTipType.ProgressInformation)
            Case ProgressingTaskItem.ProgressingTaskStatus.Aborted
                control.LabelStepInfo.Text = "Abgebrochen nach " & taskItem.ConsumedTime.Value.ToString("d\.hh\:mm\:ss", System.Globalization.CultureInfo.InvariantCulture) & " (d.hh:min:sec): Schritt " & taskItem.RunningTotalStepNumber.Value & "/" & taskItem.TotalStepsCount & ": " & taskItem.RunningStep.StepTitle
                SetToolTipForTaskControl(control, taskItem.SummaryText, TaskResultToolTipType.ProgressErrors)
                CType(control, ProgressingTaskControl).GroupBox.BackColor = Color.Yellow
                control.ProgressBar.Value = control.ProgressBar.Maximum 'stops the animation effect
                control.ProgressBar.Value = NewProgressValue
            Case Else
                Stop
                Throw New NotSupportedException("Unknown status")
        End Select
    End Sub

    Private _AsyncRunnerTask As Task

    Public Event TaskBundleCompleted(status As ProgressingTaskBundle.ProgressingTaskBundleStatus)


    Private Sub ButtonStart_Click(sender As Object, e As EventArgs) Handles ButtonStart.Click
        UITools.TryRun(Me,
                       Sub()
                           TimerRefreshAllControls.Enabled = True
                           'Task.Run(Sub() _AsyncRunnerTask = Me.TaskBundle.RunAllTasksAsync())
                           _AsyncRunnerTask = Task.Run(Async Function()
                                                           Try
                                                               Await Me.TaskBundle.RunAllTasksAsync()
                                                           Finally
                                                               Invoke(Sub() OnTaskBundleCompleted(Me.TaskBundle.Status))
                                                           End Try
                                                       End Function)
                       End Sub,
                       Sub(switch)
                           Me.Cursor = If(switch, Cursors.WaitCursor, Cursors.Default)
                           Me.UseWaitCursor = switch
                       End Sub
                       )
    End Sub

    Public Delegate Sub OnTaskBundleCompletedActionMethod(status As ProgressingTaskBundle.ProgressingTaskBundleStatus)

    ''' <summary>
    ''' Actions to call on task bundle completed
    ''' </summary>
    ''' <returns></returns>
    Public Property OnTaskBundleCompletedActions As New List(Of OnTaskBundleCompletedActionMethod)

    Protected Overridable Sub OnTaskBundleCompleted(status As ProgressingTaskBundle.ProgressingTaskBundleStatus)
        For Each ActionItem In OnTaskBundleCompletedActions
            ActionItem(status)
        Next
        RaiseEvent TaskBundleCompleted(status)
    End Sub

    ''' <summary>
    ''' Automatically show message box with task bundle status after task bundle completed
    ''' </summary>
    ''' <returns></returns>
    Public Property ShowMessageBoxOnTaskBundleCompleted As Boolean = True

    ''' <summary>
    ''' Automatically close form after task bundle completed
    ''' </summary>
    ''' <returns></returns>
    Public Property AutoCloseFormOnTaskBundleCompleted As Boolean = False

    Private Sub MyForm_TaskBundleCompleted(status As ProgressingTaskBundle.ProgressingTaskBundleStatus) Handles Me.TaskBundleCompleted
        UITools.TryRun(Me,
                       Sub()
                           RefreshAllControls()
                       End Sub,
                       Sub(ex)
                           MessageBox.Show(Me, "ERROR: " & ex.ToString, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
                       End Sub,
                       Sub(switch)
                       End Sub
                       )
        TimerRefreshAllControls.Enabled = False
        If AutoCloseFormOnTaskBundleCompleted Then
            Me.Close()
        ElseIf ShowMessageBoxOnTaskBundleCompleted Then
            Select Case status
                Case ProgressingTaskBundle.ProgressingTaskBundleStatus.CompletedSuccessfully
                    MessageBox.Show(Me, "Der Task wurde erfolgreich abgeschlossen.", Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Information)
                Case ProgressingTaskBundle.ProgressingTaskBundleStatus.FailedNonCritically
                    Me.ButtonCopyResultsToClipboard.Visible = True
                    MessageBox.Show(Me, "Der Task wurde mit Fehlern abgeschlossen.", Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                Case ProgressingTaskBundle.ProgressingTaskBundleStatus.FailedInCriticalState
                    Me.ButtonCopyResultsToClipboard.Visible = True
                    MessageBox.Show(Me, "Der Task wurde mit Fehlern abgeschlossen. ACHTUNG: Aufgrund von nicht korrigierbaren Fehlern verbleibt das System in einem undefinierten, kritischen Zustand.", Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Case ProgressingTaskBundle.ProgressingTaskBundleStatus.Aborted
                    Me.ButtonCopyResultsToClipboard.Visible = True
                    MessageBox.Show(Me, "Der Task wurde abgebrochen.", Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                Case Else
                    MessageBox.Show(Me, "Der Task wurde abgeschlossen: " & status.ToString, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End Select
        End If
    End Sub

    Private Sub ButtonCancel_Click(sender As Object, e As EventArgs) Handles ButtonCancel.Click
        Me.ButtonCancel.Enabled = False
        Me.TaskBundle.CancelAllTasks()
    End Sub

    Private Sub MultiTaskProgessForm_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        If Me.TaskBundle.IsRunningOrAbortingOrFailing AndAlso Me._AsyncRunnerTask IsNot Nothing AndAlso Me._AsyncRunnerTask.IsCompleted = False Then
            e.Cancel = True
        End If
    End Sub

    Private Sub TimerRefreshAllControls_Tick(sender As Object, e As EventArgs) Handles TimerRefreshAllControls.Tick
        UITools.TryRun(Me,
                       Sub()
                           RefreshAllControls()
                           If _AsyncRunnerTask IsNot Nothing AndAlso _AsyncRunnerTask.IsCompleted AndAlso Me.TaskBundle.IsRunningOrAbortingOrFailing Then
                               Throw New InvalidOperationException("AsyncTask is completed, but TaskBundle is still running")
                           End If
                           If Me.TaskBundle.IsRunningOrAbortingOrFailing = False Then '_AsynRunnerTask IsNot Nothing AndAlso _AsynRunnerTask.IsCompleted Then
                               TimerRefreshAllControls.Enabled = False
                               RefreshAllControls()
                           End If
                       End Sub,
                       Sub(ex)
                           TimerRefreshAllControls.Enabled = False
                           RefreshAllControls()
                           MessageBox.Show(Me, "ERROR: " & ex.ToString, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
                       End Sub,
                       Sub(switch)
                           Me.Cursor = If(switch, Cursors.WaitCursor, Cursors.Default)
                           Me.UseWaitCursor = switch
                       End Sub
                       )
    End Sub

    Private Sub ButtonClose_Click(sender As Object, e As EventArgs) Handles ButtonClose.Click
        Me.Close()
    End Sub

    Private Sub ButtonCopyResultsToClipboard_Click(sender As Object, e As EventArgs) Handles ButtonCopyResultsToClipboard.Click
        UITools.TryRun(
            Me,
            Sub()
                Dim ResultText As New System.Text.StringBuilder
                For MyCounter As Integer = 0 To Me.TaskBundle.Tasks.Count - 1
                    Dim TaskItem As ProgressingTaskItem = Me.TaskBundle.Tasks(MyCounter)
                    ResultText.AppendLine("## TASK " & MyCounter + 1 & " " & TaskItem.ToString)
                    ResultText.AppendLine(TaskItem.SummaryText(True))
                Next
                Clipboard.SetText(ResultText.ToString)
            End Sub,
            Sub()
            End Sub
            )
    End Sub

End Class