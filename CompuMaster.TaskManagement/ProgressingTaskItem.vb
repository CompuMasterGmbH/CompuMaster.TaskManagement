Option Explicit On
Option Strict On

Imports System.Threading
Imports CompuMaster.TaskManagement.ProgressingTaskStepBase
Imports CompuMaster.TaskManagement.Exceptions

Public Class ProgressingTaskItem

    ''' <summary>
    ''' Erstellt ein neues Task-Item und fügt es dem übergeordneten Task-Bundle hinzu
    ''' </summary>
    ''' <param name="parentTaskBundle"></param>
    ''' <param name="taskTitle"></param>
    Friend Sub New(parentTaskBundle As ProgressingTaskBundle, taskTitle As String)
        Me.ParentTaskBundle = parentTaskBundle
        Me.TaskTitle = taskTitle
        parentTaskBundle.AddTask(Me)
    End Sub

    ''' <summary>
    ''' Erstellt ein neues Task-Item und fügt es dem übergeordneten Task-Bundle hinzu
    ''' </summary>
    ''' <param name="parentTaskBundle"></param>
    ''' <param name="taskTitle"></param>
    ''' <param name="runBehaviourIfPreviousTasksFailed"></param>
    Friend Sub New(parentTaskBundle As ProgressingTaskBundle, taskTitle As String, runBehaviourIfPreviousTasksFailed As RunBehaviourIfPreviousTasksFailedEnum)
        Me.ParentTaskBundle = parentTaskBundle
        Me.TaskTitle = taskTitle
        Me.RunBehaviourIfPreviousTasksFailed = runBehaviourIfPreviousTasksFailed
        parentTaskBundle.AddTask(Me)
    End Sub

    Public Property ParentTaskBundle As ProgressingTaskBundle

    Public Property TaskTitle As String

    ''' <summary>
    ''' Verhalten, wenn vorherige Tasks fehlschlugen oder ein Abbruch angefordert wurde
    ''' </summary>
    ''' <returns></returns>
    Public Property RunBehaviourIfPreviousTasksFailed As ProgressingTaskItem.RunBehaviourIfPreviousTasksFailedEnum = ProgressingTaskItem.RunBehaviourIfPreviousTasksFailedEnum.SkipIfPreviousTasksFailedOrCancelled

    Public Enum RunBehaviourIfPreviousTasksFailedEnum
        ''' <summary>
        ''' Must always run (for e.g. rollback tasks), even if previous tasks failed or were cancelled
        ''' </summary>
        RunAlways = 0
        ''' <summary>
        ''' Run only if previous tasks completed successfully and no cancellation was requested, otherweise skip
        ''' </summary>
        SkipIfPreviousTasksFailedOrCancelled = 1
    End Enum

    ''' <summary>
    ''' Steps which allow rollback
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property FirstStepsWhichCanBeRolledBack As New List(Of ProgressingTaskFailFastStep)

    ''' <summary>
    ''' (Critical) steps which can't be rolled back
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property SecondStepsWithoutRollbackOption As New List(Of ProgressingTaskStep)

    ''' <summary>
    ''' Rollback actions
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property RollbackSteps As New List(Of ProgressingTaskStep)

    ''' <summary>
    ''' Adds a step which can be rolled back to the FirstStepsWhichCanBeRolledBack list
    ''' </summary>
    ''' <param name="taskStep"></param>
    Public Sub AddStepWhichCanBeRolledBack(taskStep As ProgressingTaskFailFastStep)
        FirstStepsWhichCanBeRolledBack.Add(taskStep)
    End Sub

    ''' <summary>
    ''' Adds a step which can be rolled back to the FirstStepsWhichCanBeRolledBack list
    ''' </summary>
    Public Sub AddStepWhichCanBeRolledBack(stepTitle As String, stepAction As StepActionMethod, estimatedTimeToRun As TimeSpan?)
        Dim taskStep As New ProgressingTaskFailFastStep(stepTitle, stepAction, estimatedTimeToRun)
        FirstStepsWhichCanBeRolledBack.Add(taskStep)
    End Sub

    ''' <summary>
    ''' Adds a step which can't be rolled back to the SecondsStepsWithoutRollbackOption list
    ''' </summary>
    ''' <param name="taskStep"></param>
    Public Sub AddStepWithoutRolledBackPossibility(taskStep As ProgressingTaskStep)
        SecondStepsWithoutRollbackOption.Add(taskStep)
    End Sub

    ''' <summary>
    ''' Adds a step which can't be rolled back to the SecondsStepsWithoutRollbackOption list
    ''' </summary>
    Public Sub AddStepWithoutRolledBackPossibility(stepTitle As String, stepAction As StepActionMethod, estimatedTimeToRun As TimeSpan?, failAction As ProgressingTaskStepFailAction)
        Dim taskStep As New ProgressingTaskStep(stepTitle, stepAction, estimatedTimeToRun, failAction)
        SecondStepsWithoutRollbackOption.Add(taskStep)
    End Sub

    ''' <summary>
    ''' Adds a step which can't be rolled back to the SecondsStepsWithoutRollbackOption list
    ''' </summary>
    Public Sub AddStepWithoutRolledBackPossibility(stepTitle As String, stepAction As StepActionMethodWithFailAction, estimatedTimeToRun As TimeSpan?)
        Dim taskStep As New ProgressingTaskStep(stepTitle, stepAction, estimatedTimeToRun)
        SecondStepsWithoutRollbackOption.Add(taskStep)
    End Sub

    ''' <summary>
    ''' Adds a rollback step to the RollbackSteps list
    ''' </summary>
    ''' <param name="taskStep"></param>
    Public Sub AddRollbackStep(taskStep As ProgressingTaskStep)
        RollbackSteps.Add(taskStep)
    End Sub

    ''' <summary>
    ''' Adds a rollback step to the RollbackSteps list
    ''' </summary>
    Public Sub AddRollbackStep(stepTitle As String, stepAction As StepActionMethod, estimatedTimeToRun As TimeSpan?, failAction As ProgressingTaskStepFailAction)
        Dim taskStep As New ProgressingTaskStep(stepTitle, stepAction, estimatedTimeToRun, failAction)
        RollbackSteps.Add(taskStep)
    End Sub

    Public Enum ProgressingTaskStatus As Byte
        NotStarted = 0
        InProgress = 1
        Completed = 2
        FailedWithRollbackOption = 31
        Skipped = 4
        Aborted = 5
        Aborting = 6
        FailedInCriticalState = 30
        FailingInCriticalState = 40
        FailingWithRollbackOption = 41
    End Enum

    Public Property Status As ProgressingTaskItem.ProgressingTaskStatus = ProgressingTaskItem.ProgressingTaskStatus.NotStarted

    Public Sub MarkAsSkipped()
        If Me.Status = ProgressingTaskItem.ProgressingTaskStatus.NotStarted Then
            Me.Status = ProgressingTaskItem.ProgressingTaskStatus.Skipped
        Else
            Throw New InvalidOperationException("This task is already in progress or has already finished")
        End If
    End Sub

    Public ReadOnly Property TotalStepsCount As Integer
        Get
            Return FirstStepsWhichCanBeRolledBack.Count + SecondStepsWithoutRollbackOption.Count
        End Get
    End Property

    Private _RunningStepNumber As Integer?
    ''' <summary>
    ''' Nummer des aktuell laufenden Schritts (1-basiert)
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property RunningTotalStepNumber As Integer?
        Get
            Return _RunningStepNumber
        End Get
    End Property

    ''' <summary>
    ''' Index des aktuell laufenden Schritts (0-basiert)
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property RunningTotalStepIndex As Integer?
        Get
            If _RunningStepNumber.HasValue Then
                Return _RunningStepNumber.Value - 1
            Else
                Return Nothing
            End If
        End Get
    End Property

    ''' <summary>
    ''' Der hinterlegte Schritt
    ''' </summary>
    ''' <param name="totalStepIndex"></param>
    ''' <returns></returns>
    Public Function TotalStepItem(totalStepIndex As Integer) As ProgressingTaskStepBase
        If totalStepIndex < 0 OrElse totalStepIndex >= TotalStepsCount Then
            Throw New ArgumentOutOfRangeException(NameOf(totalStepIndex), "Argument totalStepIndex=" & totalStepIndex.ToString(System.Globalization.CultureInfo.InvariantCulture) & ", but allowed range=0.." & TotalStepsCount - 1)
        End If
        If totalStepIndex < FirstStepsWhichCanBeRolledBack.Count Then
            Return FirstStepsWhichCanBeRolledBack(totalStepIndex)
        Else
            Return SecondStepsWithoutRollbackOption(totalStepIndex - FirstStepsWhichCanBeRolledBack.Count)
        End If
    End Function

    ''' <summary>
    ''' Aktuell laufender Schritt
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property RunningStep As ProgressingTaskStepBase
        Get
            If RunningTotalStepIndex.HasValue Then
                Return TotalStepItem(RunningTotalStepIndex.Value)
            Else
                Return Nothing
            End If
        End Get
    End Property

    ''' <summary>
    ''' Zuweisen des übergeordneten Tasks zu allen Schritten
    ''' </summary>
    Public Sub AssignParentTaskToAllSteps()
        For Each CurrentStep In FirstStepsWhichCanBeRolledBack
            CurrentStep.ParentTask = Me
        Next
        For Each CurrentStep In SecondStepsWithoutRollbackOption
            CurrentStep.ParentTask = Me
        Next
        For Each CurrentStep In RollbackSteps
            CurrentStep.ParentTask = Me
        Next
    End Sub

    Public Property AllowCancellationWhileRunningSecondStepsWithoutRollbackOption As Boolean

    ''' <summary>
    ''' Ausführen aller Schritte
    ''' </summary>
    Public Async Function RunAllStepsAsync(cancellationToken As Threading.CancellationToken) As Task
        Select Case Status
            Case ProgressingTaskItem.ProgressingTaskStatus.NotStarted
                If Me.FirstStepsWhichCanBeRolledBack.Count <> 0 AndAlso Me.RollbackSteps.Count = 0 Then
                    Throw New InvalidOperationException("No rollback steps defined")
                End If

                Status = ProgressingTaskStatus.InProgress
                _StartTime = DateTime.Now
                Me.AssignParentTaskToAllSteps()
                _RunningStepNumber = 0

                'Run first steps which can be rolled back
                With Nothing
                    Dim CurrentStep As ProgressingTaskFailFastStep = Nothing
                    Try
                        For MyCounter As Integer = 0 To FirstStepsWhichCanBeRolledBack.Count - 1
                            _RunningStepNumber += 1
                            CurrentStep = FirstStepsWhichCanBeRolledBack(MyCounter)
                            If cancellationToken.IsCancellationRequested Then
                                Throw New Exceptions.UserAbortedMessageException("Benutzer hat den Vorgang abgebrochen")
                            End If
                            Await Task.Run(Sub() CurrentStep.Run(), Threading.CancellationToken.None)
                        Next
                    Catch ex As Exceptions.UserAbortedMessageException
                        'Log exception
                        LoggedExceptions.Add(New StepException("Step " & _RunningStepNumber.Value & " aborted: " & CurrentStep.StepTitle, ex))
                        Status = ProgressingTaskItem.ProgressingTaskStatus.Aborting
                        _EndTime = DateTime.Now

                        'Rollback
                        CreateRollbackTaskItemAndAppendToTaskBundle()
                    Catch ex As Exception
                        'Log exception
                        LoggedExceptions.Add(New StepException("Step " & _RunningStepNumber.Value & " failed: " & CurrentStep.StepTitle, ex))
                        Status = ProgressingTaskItem.ProgressingTaskStatus.FailingWithRollbackOption
                        _EndTime = DateTime.Now

                        'Rollback
                        CreateRollbackTaskItemAndAppendToTaskBundle()
                    End Try
                End With

                'Run second steps which can't be rolled back
                If Status = ProgressingTaskStatus.InProgress Then
                    Try
                        Dim CurrentStep As ProgressingTaskStep = Nothing
                        For MyCounter As Integer = 0 To SecondStepsWithoutRollbackOption.Count - 1
                            _RunningStepNumber += 1
                            CurrentStep = SecondStepsWithoutRollbackOption(MyCounter)
                            If AllowCancellationWhileRunningSecondStepsWithoutRollbackOption AndAlso cancellationToken.IsCancellationRequested Then
                                Throw New Exceptions.UserAbortedMessageException("Benutzer hat den Vorgang abgebrochen")
                            End If
                            Select Case CurrentStep.FailAction
                                Case ProgressingTaskStep.ProgressingTaskStepFailAction.LogExceptionAndContinue
                                    Try
                                        Await Task.Run(Sub() CurrentStep.Run(), Threading.CancellationToken.None)
                                    Catch ex As Exception
                                        Me.Status = ProgressingTaskItem.ProgressingTaskStatus.FailingInCriticalState
                                        LoggedExceptions.Add(ex)
                                    End Try
                                Case ProgressingTaskStep.ProgressingTaskStepFailAction.ThrowException
                                    Await Task.Run(Sub() CurrentStep.Run(), Threading.CancellationToken.None)
                                Case ProgressingTaskStepFailAction.DependingOnResultOfStepActionMethodWithFailAction
                                    Try
                                        Await Task.Run(Sub() CurrentStep.Run(), Threading.CancellationToken.None)
                                    Catch ex As Exception
                                        Select Case CurrentStep.FailAction
                                            Case ProgressingTaskStepFailAction.DependingOnResultOfStepActionMethodWithFailAction
                                                'Step action failed and wasn't able to return final fail action before ending -> throw exception immediately
                                                Throw New NotImplementedException("Implementation incomplete at step " & CurrentStep.StepTitle & ": Step action must catch exceptions and assign FailAction before re-throwing the exception", ex)
                                            Case ProgressingTaskStepFailAction.ThrowException
                                                Throw
                                            Case ProgressingTaskStepFailAction.LogExceptionAndContinue
                                                Me.Status = ProgressingTaskItem.ProgressingTaskStatus.FailingInCriticalState
                                                LoggedExceptions.Add(ex)
                                            Case Else
                                                Throw New NotImplementedException
                                        End Select
                                    End Try
                                Case Else
                                    Throw New NotImplementedException("Unknown fail action")
                            End Select
                        Next
                    Catch ex As NotImplementedException
                        Me.Status = ProgressingTaskItem.ProgressingTaskStatus.FailedInCriticalState
                        Throw
                    Catch ex As Exception
                        Me.Status = ProgressingTaskItem.ProgressingTaskStatus.FailingInCriticalState
                        Me.LoggedExceptions.Add(ex)
                    End Try
                End If

                If Me.LoggedExceptions.Count <> 0 Then
                    Select Case Status
                        Case ProgressingTaskItem.ProgressingTaskStatus.FailingWithRollbackOption
                            Status = ProgressingTaskStatus.FailedWithRollbackOption
                        Case ProgressingTaskItem.ProgressingTaskStatus.FailingInCriticalState
                            Status = ProgressingTaskStatus.FailedInCriticalState
                        Case ProgressingTaskStatus.Aborting
                            Status = ProgressingTaskStatus.Aborted
                        Case Else
                            'should never happen since status should already be set to failed in code above
                            Stop
                            Throw New InvalidOperationException("Unknown status")
                    End Select
                    _EndTime = DateTime.Now
                    Throw New AggregateException("Fehlgeschlagener Task: " & Me.TaskTitle, Me.LoggedExceptions)
                ElseIf Status = ProgressingTaskStepBase.ProgressingTaskStepStatus.InProgress Then
                    Status = ProgressingTaskItem.ProgressingTaskStatus.Completed
                    _EndTime = DateTime.Now
                Else
                    Throw New InvalidOperationException("Unknown status")
                End If
            Case ProgressingTaskItem.ProgressingTaskStatus.InProgress, ProgressingTaskStatus.FailingWithRollbackOption, ProgressingTaskStatus.FailingInCriticalState, ProgressingTaskStatus.Aborting
                Throw New InvalidOperationException("This task is already in progress")
            Case ProgressingTaskItem.ProgressingTaskStatus.Completed
                Throw New InvalidOperationException("This task already completed")
            Case ProgressingTaskItem.ProgressingTaskStatus.FailedWithRollbackOption, ProgressingTaskStatus.FailedInCriticalState
                Throw New InvalidOperationException("This task already ran and failed")
            Case ProgressingTaskItem.ProgressingTaskStatus.Aborted
                Throw New InvalidOperationException("This task already aborted")
            Case ProgressingTaskItem.ProgressingTaskStatus.Skipped
                Throw New InvalidOperationException("This task was already skipped")
            Case Else
                Throw New NotSupportedException("Unknown status")
        End Select
    End Function

    Public Property LoggedExceptions As New List(Of Exception)

    ''' <summary>
    ''' Erstellt ein neues Task-Item für den Rollback und hinterlegt diesen im übergeordneten Task-Bundle
    ''' </summary>
    Private Sub CreateRollbackTaskItemAndAppendToTaskBundle()
        Dim MyRollbackTaskItem As New ProgressingTaskItem(Me.ParentTaskBundle, "Rollback " & Me.TaskTitle)
        MyRollbackTaskItem.RunBehaviourIfPreviousTasksFailed = RunBehaviourIfPreviousTasksFailedEnum.RunAlways

        'Add rollback steps
        For MyCounter As Integer = 0 To RollbackSteps.Count - 1
            MyRollbackTaskItem.SecondStepsWithoutRollbackOption.Add(RollbackSteps(MyCounter))
        Next
    End Sub

    ''' <summary>
    ''' Estimated time to run all steps
    ''' </summary>
    ''' <returns></returns>
    Public Function EstimatedTimeToRun() As TimeSpan?
        Dim Result As TimeSpan?
        For Each CurrentStep In FirstStepsWhichCanBeRolledBack
            If CurrentStep.EstimatedTimeToRun.HasValue Then
                If Result.HasValue = False Then
                    Result = TimeSpan.Zero
                End If
                Result += CurrentStep.EstimatedTimeToRun.Value
            End If
        Next
        For Each CurrentStep In SecondStepsWithoutRollbackOption
            If CurrentStep.EstimatedTimeToRun.HasValue Then
                If Result.HasValue = False Then
                    Result = TimeSpan.Zero
                End If
                Result += CurrentStep.EstimatedTimeToRun.Value
            End If
        Next
        Return Result
    End Function

    ''' <summary>
    ''' Estimated remaining time to run
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property EstimatedTimeOfArrival() As TimeSpan?
        Get
            If Me.RunningTotalStepIndex.HasValue = False Then
                'Task has not started yet -> return estimated time to run
                If Me.Status = ProgressingTaskStep.ProgressingTaskStepStatus.NotStarted Then
                    Return Me.EstimatedTimeToRun
                Else
                    Return Nothing
                End If
            Else
                'Task has started -> return estimated remaining time of arrival
                Dim Result As TimeSpan? = Nothing
                For MyCounter As Integer = Me.RunningTotalStepIndex.Value To Me.TotalStepsCount - 1
                    Dim CurrentStep As ProgressingTaskStepBase = Me.TotalStepItem(MyCounter)
                    If CurrentStep.EstimatedTimeOfArrival.HasValue Then
                        If Result.HasValue = False Then
                            Result = TimeSpan.Zero
                        End If
                        Result += CurrentStep.EstimatedTimeOfArrival.Value
                    End If
                Next
                Return Result
            End If
        End Get
    End Property

    Private _StartTime As DateTime?
    Private _EndTime As DateTime?

    ''' <summary>
    ''' Consumed time since start
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property ConsumedTime As TimeSpan?
        Get
            If _StartTime.HasValue = False Then
                Return Nothing
            ElseIf _EndTime.HasValue = False Then
                Return DateTime.Now - _StartTime.Value
            Else
                Return _EndTime.Value - _StartTime.Value
            End If
        End Get
    End Property

    ''' <summary>
    ''' Statistik zur verbrauchten Laufzeit zur Verwendung in Protokollen und Tooltips
    ''' </summary>
    ''' <returns></returns>
    Public Function ConsumedTimeStatisticsInfo(Optional fullLength As Boolean = False) As String
        Dim Result As New Text.StringBuilder
        Result.AppendLine("Consumed time statistics for task:")
        If Me.EstimatedTimeToRun.HasValue Then Result.AppendLine("* Estimated time: " & Me.EstimatedTimeToRun.Value.ToString("d\.hh\:mm\:ss", System.Globalization.CultureInfo.InvariantCulture))
        'Show consumed time statistics for each step if full length is requested or if there are less than 40 steps
        If Me.ConsumedTime.HasValue Then Result.AppendLine("* Consumed time: " & Me.ConsumedTime.Value.ToString("d\.hh\:mm\:ss", System.Globalization.CultureInfo.InvariantCulture))
        If Not Me.EstimatedTimeToRun.HasValue AndAlso Not Me.ConsumedTime.HasValue Then Result.AppendLine("* No time information available")

        Dim StartIndex As Integer
        Dim EndIndex As Integer
        If fullLength OrElse Me.TotalStepsCount < 20 Then
            'Show consumed time statistics for each step if full length is requested or if there are less than 40 steps
            StartIndex = System.Math.Max(0, Me.RunningTotalStepIndex.GetValueOrDefault - 15)
            EndIndex = System.Math.Min(Me.TotalStepsCount - 1, StartIndex + 20)
        Else
            StartIndex = 0
            EndIndex = Me.TotalStepsCount - 1
        End If

        For MyCounter As Integer = StartIndex To EndIndex
            Dim CurrentStep As ProgressingTaskStepBase = Me.TotalStepItem(MyCounter)
            Result.AppendLine()
            Result.AppendLine("Step " & MyCounter + 1 & ": " & CurrentStep.StepTitle)
            If CurrentStep.EstimatedTimeToRun.HasValue Then Result.AppendLine("* Estimated time: " & CurrentStep.EstimatedTimeToRun.Value.ToString("d\.hh\:mm\:ss", System.Globalization.CultureInfo.InvariantCulture))
            If CurrentStep.ConsumedTime.HasValue Then Result.AppendLine("* Consumed time: " & CurrentStep.ConsumedTime.Value.ToString("d\.hh\:mm\:ss", System.Globalization.CultureInfo.InvariantCulture))
            If Not CurrentStep.EstimatedTimeToRun.HasValue AndAlso Not CurrentStep.ConsumedTime.HasValue Then Result.AppendLine("* No time information available")
        Next
        Return Result.ToString
    End Function

    ''' <summary>
    ''' Textdarstellung des Task-Items aus Titel und Status
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property DisplayTitleAndStatus As String
        Get
            Return Me.TaskTitle & " [" & Me.Status.ToString() & "]"
        End Get
    End Property

    ''' <summary>
    ''' Textdarstellung des Task-Items aus Titel und Status
    ''' </summary>
    ''' <returns></returns>
    Public Overrides Function ToString() As String
        Return Me.TaskTitle & " [" & Me.Status.ToString() & "]"
    End Function

    ''' <summary>
    ''' Textdarstellung der gesammelten Exceptions
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property LoggedExceptionsToPlainText(fullLength As Boolean) As String
        Get
            Dim Result As New Text.StringBuilder
            For MyCounter As Integer = 0 To LoggedExceptions.Count - 1
                Dim CurrentException = LoggedExceptions(MyCounter)
                If Result.Length <> 0 Then Result.AppendLine()
                If fullLength Then
                    Result.AppendLine(CurrentException.ToString) 'Full stack trace for 1st exception
                Else
                    Result.AppendLine(CurrentException.Message) 'Only message for all other exceptions
                End If
            Next
            Return Result.ToString
        End Get
    End Property

    Private ReadOnly _CollectedWarnings As List(Of ValueTuple(Of String, String)) = New List(Of ValueTuple(Of String, String))

    ''' <summary>
    ''' Die gesammelten Warnungen
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property CollectedWarnings As List(Of ValueTuple(Of String, String))
        Get
            Return _CollectedWarnings
        End Get
    End Property

    ''' <summary>
    ''' Warnung hinzufügen
    ''' </summary>
    ''' <param name="warning"></param>
    Public Sub AddWarning(warning As String)
        AddWarning(warning, GetStackTraceWithoutLastMethod)
    End Sub

    ''' <summary>
    ''' Warnung hinzufügen
    ''' </summary>
    ''' <param name="warning"></param>
    ''' <param name="stacktrace"></param>
    Public Sub AddWarning(warning As String, stacktrace As String)
        CollectedWarnings.Add(New ValueTuple(Of String, String)(warning, stacktrace))
    End Sub

    ''' <summary>
    ''' Erstellen eines StackTrace ohne die letzte Methode
    ''' </summary>
    ''' <returns></returns>
    Private Shared Function GetStackTraceWithoutLastMethod() As String
        ' Holen Sie sich den vollständigen StackTrace
        Dim fullStackTrace As String = Environment.StackTrace

        ' Teilen Sie den StackTrace in einzelne Zeilen auf
        Dim stackTraceLines As String() = fullStackTrace.Split(New String() {Environment.NewLine}, StringSplitOptions.None)

        ' Entfernen Sie die letzte Methode (die letzte Zeile)
        Dim stackTraceWithoutLastMethod As String() = stackTraceLines.Take(stackTraceLines.Length - 2).ToArray()

        ' Fügen Sie die verbleibenden Zeilen wieder zu einem String zusammen
        Return String.Join(Environment.NewLine, stackTraceWithoutLastMethod)
    End Function

    ''' <summary>
    ''' Textdarstellung der gesammelten Warnungen
    ''' </summary>
    ''' <returns></returns>
    Public Function CollectedWarningsText() As String
        If Me.CollectedWarnings.Count <> 0 Then
            Dim Result As New System.Text.StringBuilder
            Result.AppendLine("Warnings of task:")
            For Each warning In Me.CollectedWarnings
                Result.AppendLine("* " & Tools.IndentString(warning.Item1, 2, System.Environment.NewLine))
                Result.AppendLine(Tools.IndentString(warning.Item2, 4, System.Environment.NewLine))
            Next
            For MyCounter As Integer = 0 To Me.TotalStepsCount - 1
                Dim CurrentStep As ProgressingTaskStepBase = Me.TotalStepItem(MyCounter)
                If CurrentStep.CollectedWarnings.Count <> 0 Then
                    Result.AppendLine()
                    Result.AppendLine("Step " & MyCounter + 1 & ": " & CurrentStep.StepTitle)
                    For Each warning In CurrentStep.CollectedWarnings
                        Result.AppendLine("* " & Tools.IndentString(warning.Item1, 2, System.Environment.NewLine))
                        Result.AppendLine(Tools.IndentString(warning.Item2, 4, System.Environment.NewLine))
                    Next
                End If
            Next
            Return Result.ToString
        Else
            Return Nothing
        End If
    End Function

    ''' <summary>
    ''' Status-Zusammenfassung des Task-Items
    ''' </summary>
    ''' <param name="fullLength"></param>
    ''' <returns></returns>
    Public Function SummaryText(Optional fullLength As Boolean = False) As String
        Dim Result As New System.Text.StringBuilder
        Result.Append(Me.LoggedExceptionsToPlainText(fullLength)) 'either empty or contains exceptions with line-break after last exception
        If Result.Length <> 0 Then Result.AppendLine()
        Result.Append(CollectedWarningsText) 'either empty or contains warnings with line-break at end of last warning 
        If Result.Length <> 0 Then Result.AppendLine()
        Result.Append(Me.ConsumedTimeStatisticsInfo(fullLength)) 'always contains some lines and line-break at last line
        Return Result.ToString
    End Function

End Class
