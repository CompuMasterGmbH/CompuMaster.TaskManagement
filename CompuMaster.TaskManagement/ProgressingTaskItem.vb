Option Explicit On
Option Strict On

Imports System.Threading
Imports CompuMaster.TaskManagement.ProgressingTaskStepBase
Imports CompuMaster.TaskManagement.Exceptions

''' <summary>
''' Represents a progressing task in a task bundle.
''' </summary>
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

    ''' <summary>
    ''' Gets or sets the parent task bundle.
    ''' </summary>
    Public Property ParentTaskBundle As ProgressingTaskBundle

    ''' <summary>
    ''' Gets or sets the display title of the task.
    ''' </summary>
    Public Property TaskTitle As String

    ''' <summary>
    ''' Gets or sets the run behavior when previous tasks failed or were cancelled.
    ''' </summary>
    ''' <returns></returns>
    Public Property RunBehaviourIfPreviousTasksFailed As ProgressingTaskItem.RunBehaviourIfPreviousTasksFailedEnum = ProgressingTaskItem.RunBehaviourIfPreviousTasksFailedEnum.SkipIfPreviousTasksFailedOrCancelled

    ''' <summary>
    ''' Defines whether a task runs when previous tasks failed or were cancelled.
    ''' </summary>
    Public Enum RunBehaviourIfPreviousTasksFailedEnum
        ''' <summary>
        ''' The task always runs, even if previous tasks failed or were cancelled.
        ''' </summary>
        RunAlways = 0
        ''' <summary>
        ''' The task runs only if previous tasks completed successfully and no cancellation was requested.
        ''' </summary>
        SkipIfPreviousTasksFailedOrCancelled = 1
    End Enum

    ''' <summary>
    ''' Gets the steps that allow rollback.
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property FirstStepsWhichCanBeRolledBack As New List(Of ProgressingTaskFailFastStep)

    ''' <summary>
    ''' Gets the critical steps that cannot be rolled back.
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property SecondStepsWithoutRollbackOption As New List(Of ProgressingTaskStep)

    ''' <summary>
    ''' Gets the rollback steps.
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property RollbackSteps As New List(Of ProgressingTaskStep)

    ''' <summary>
    ''' Adds a step that can be rolled back.
    ''' </summary>
    ''' <param name="taskStep">The step to add.</param>
    Public Sub AddStepWhichCanBeRolledBack(taskStep As ProgressingTaskFailFastStep)
        FirstStepsWhichCanBeRolledBack.Add(taskStep)
    End Sub

    ''' <summary>
    ''' Adds a step that can be rolled back.
    ''' </summary>
    ''' <param name="stepTitle">The display title of the step.</param>
    ''' <param name="stepAction">The action to execute for the step.</param>
    ''' <param name="estimatedTimeToRun">The estimated duration of the step, or <see langword="Nothing"/> if the duration is unknown.</param>
    Public Sub AddStepWhichCanBeRolledBack(stepTitle As String, stepAction As StepActionMethod, estimatedTimeToRun As TimeSpan?)
        Dim taskStep As New ProgressingTaskFailFastStep(stepTitle, stepAction, estimatedTimeToRun)
        FirstStepsWhichCanBeRolledBack.Add(taskStep)
    End Sub

    ''' <summary>
    ''' Adds a step that cannot be rolled back.
    ''' </summary>
    ''' <param name="taskStep">The step to add.</param>
    Public Sub AddStepWithoutRolledBackPossibility(taskStep As ProgressingTaskStep)
        SecondStepsWithoutRollbackOption.Add(taskStep)
    End Sub

    ''' <summary>
    ''' Adds a step that cannot be rolled back.
    ''' </summary>
    ''' <param name="stepTitle">The display title of the step.</param>
    ''' <param name="stepAction">The action to execute for the step.</param>
    ''' <param name="estimatedTimeToRun">The estimated duration of the step, or <see langword="Nothing"/> if the duration is unknown.</param>
    ''' <param name="failAction">The action to apply when the step fails.</param>
    Public Sub AddStepWithoutRolledBackPossibility(stepTitle As String, stepAction As StepActionMethod, estimatedTimeToRun As TimeSpan?, failAction As ProgressingTaskStepFailAction)
        Dim taskStep As New ProgressingTaskStep(stepTitle, stepAction, estimatedTimeToRun, failAction)
        SecondStepsWithoutRollbackOption.Add(taskStep)
    End Sub

    ''' <summary>
    ''' Adds a step that cannot be rolled back.
    ''' </summary>
    ''' <param name="stepTitle">The display title of the step.</param>
    ''' <param name="stepAction">The action to execute for the step and return the step result.</param>
    ''' <param name="estimatedTimeToRun">The estimated duration of the step, or <see langword="Nothing"/> if the duration is unknown.</param>
    Public Sub AddStepWithoutRolledBackPossibility(stepTitle As String, stepAction As StepActionMethodWithFailAction, estimatedTimeToRun As TimeSpan?)
        Dim taskStep As New ProgressingTaskStep(stepTitle, stepAction, estimatedTimeToRun)
        SecondStepsWithoutRollbackOption.Add(taskStep)
    End Sub

    ''' <summary>
    ''' Adds a rollback step.
    ''' </summary>
    ''' <param name="taskStep">The rollback step to add.</param>
    Public Sub AddRollbackStep(taskStep As ProgressingTaskStep)
        RollbackSteps.Add(taskStep)
    End Sub

    ''' <summary>
    ''' Adds a rollback step.
    ''' </summary>
    ''' <param name="stepTitle">The display title of the rollback step.</param>
    ''' <param name="stepAction">The action to execute for the rollback step.</param>
    ''' <param name="estimatedTimeToRun">The estimated duration of the rollback step, or <see langword="Nothing"/> if the duration is unknown.</param>
    ''' <param name="failAction">The action to apply when the rollback step fails.</param>
    Public Sub AddRollbackStep(stepTitle As String, stepAction As StepActionMethod, estimatedTimeToRun As TimeSpan?, failAction As ProgressingTaskStepFailAction)
        Dim taskStep As New ProgressingTaskStep(stepTitle, stepAction, estimatedTimeToRun, failAction)
        RollbackSteps.Add(taskStep)
    End Sub

    ''' <summary>
    ''' Defines the execution status of a task.
    ''' </summary>
    Public Enum ProgressingTaskStatus As Byte
        ''' <summary>
        ''' The task has not started.
        ''' </summary>
        NotStarted = 0
        ''' <summary>
        ''' The task is running.
        ''' </summary>
        InProgress = 1
        ''' <summary>
        ''' The task completed successfully.
        ''' </summary>
        Completed = 2
        ''' <summary>
        ''' The task failed and rollback is possible.
        ''' </summary>
        FailedWithRollbackOption = 31
        ''' <summary>
        ''' The task was skipped.
        ''' </summary>
        Skipped = 4
        ''' <summary>
        ''' The task was aborted.
        ''' </summary>
        Aborted = 5
        ''' <summary>
        ''' The task is aborting.
        ''' </summary>
        Aborting = 6
        ''' <summary>
        ''' The task failed in a critical state.
        ''' </summary>
        FailedInCriticalState = 30
        ''' <summary>
        ''' The task is failing in a critical state.
        ''' </summary>
        FailingInCriticalState = 40
        ''' <summary>
        ''' The task is failing and rollback is possible.
        ''' </summary>
        FailingWithRollbackOption = 41
    End Enum

    ''' <summary>
    ''' Gets or sets the execution status of the task.
    ''' </summary>
    Public Property Status As ProgressingTaskItem.ProgressingTaskStatus = ProgressingTaskItem.ProgressingTaskStatus.NotStarted

    ''' <summary>
    ''' Marks the task as skipped.
    ''' </summary>
    Public Sub MarkAsSkipped()
        If Me.Status = ProgressingTaskItem.ProgressingTaskStatus.NotStarted Then
            Me.Status = ProgressingTaskItem.ProgressingTaskStatus.Skipped
        Else
            Throw New InvalidOperationException("This task is already in progress or has already finished")
        End If
    End Sub

    ''' <summary>
    ''' Gets the total number of non-rollback steps.
    ''' </summary>
    Public ReadOnly Property TotalStepsCount As Integer
        Get
            Return FirstStepsWhichCanBeRolledBack.Count + SecondStepsWithoutRollbackOption.Count
        End Get
    End Property

    Private _RunningStepNumber As Integer?
    ''' <summary>
    ''' Gets the one-based number of the currently running step.
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property RunningTotalStepNumber As Integer?
        Get
            Return _RunningStepNumber
        End Get
    End Property

    ''' <summary>
    ''' Gets the zero-based index of the currently running step.
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
    ''' Gets a step by its zero-based index.
    ''' </summary>
    ''' <param name="totalStepIndex">The zero-based step index.</param>
    ''' <returns>The step at the specified index.</returns>
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
    ''' Gets the currently running step.
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property RunningStep As ProgressingTaskStepBase
        Get
            If RunningTotalStepIndex.HasValue AndAlso RunningTotalStepIndex >= 0 Then
                Return TotalStepItem(RunningTotalStepIndex.Value)
            Else
                Return Nothing
            End If
        End Get
    End Property

    ''' <summary>
    ''' Assigns this task as parent task to all steps.
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

    ''' <summary>
    ''' Gets or sets a value indicating whether cancellation is allowed while running critical steps without rollback.
    ''' </summary>
    Public Property AllowCancellationWhileRunningSecondStepsWithoutRollbackOption As Boolean

    ''' <summary>
    ''' Runs all steps asynchronously.
    ''' </summary>
    ''' <param name="cancellationToken">The cancellation token used to request cancellation.</param>
    ''' <returns>A task that represents the asynchronous run operation.</returns>
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

    ''' <summary>
    ''' Gets or sets the exceptions logged during task execution.
    ''' </summary>
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
    ''' Gets the estimated duration for all steps.
    ''' </summary>
    ''' <returns>The estimated duration, or <see langword="Nothing"/> if no estimate is available.</returns>
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
    ''' Gets the estimated remaining duration for all steps.
    ''' </summary>
    ''' <returns>The estimated remaining duration, or <see langword="Nothing"/> if no estimate is available.</returns>
    Public ReadOnly Property EstimatedTimeOfArrival() As TimeSpan?
        Get
            If Me.RunningTotalStepIndex.HasValue = False OrElse Me.RunningTotalStepIndex < 0 OrElse Me.TotalStepsCount = 0 Then
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
    ''' Gets the consumed runtime since the task started.
    ''' </summary>
    ''' <returns>The consumed runtime, or <see langword="Nothing"/> if the task has not started.</returns>
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
    ''' Creates consumed runtime statistics for logs and tooltips.
    ''' </summary>
    ''' <param name="fullLength">A value indicating whether all step details shall be included.</param>
    ''' <returns>The consumed runtime statistics text.</returns>
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
            Select Case CurrentStep.Status
                Case ProgressingTaskStepStatus.Failed
                    Result.AppendLine("* Step failed: " & Tools.IndentStringStartingWith2ndLine(CurrentStep.FoundException.Message, 2, System.Environment.NewLine))
            End Select
        Next
        Return Result.ToString
    End Function

    ''' <summary>
    ''' Gets a display text containing the task title and status.
    ''' </summary>
    Public ReadOnly Property DisplayTitleAndStatus As String
        Get
            Return Me.TaskTitle & " [" & Me.Status.ToString() & "]"
        End Get
    End Property

    ''' <summary>
    ''' Returns a display text containing the task title and status.
    ''' </summary>
    ''' <returns>The task title and status.</returns>
    Public Overrides Function ToString() As String
        Return Me.TaskTitle & " [" & Me.Status.ToString() & "]"
    End Function

    ''' <summary>
    ''' Gets a plain text representation of the logged exceptions.
    ''' </summary>
    ''' <param name="fullLength">A value indicating whether full exception details shall be included.</param>
    ''' <returns>The logged exceptions as plain text.</returns>
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
    ''' Gets the warnings collected for this task.
    ''' </summary>
    Public ReadOnly Property CollectedWarnings As List(Of ValueTuple(Of String, String))
        Get
            Return _CollectedWarnings
        End Get
    End Property

    ''' <summary>
    ''' Adds a warning with the current stack trace.
    ''' </summary>
    ''' <param name="warning">The warning text.</param>
    Public Sub AddWarning(warning As String)
        AddWarning(warning, GetStackTraceWithoutLastMethod)
    End Sub

    ''' <summary>
    ''' Adds a warning with an explicit stack trace.
    ''' </summary>
    ''' <param name="warning">The warning text.</param>
    ''' <param name="stacktrace">The stack trace text.</param>
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
    ''' Creates a text representation of the collected warnings.
    ''' </summary>
    ''' <returns>The collected warnings as text, or <see langword="Nothing"/> if no warnings were collected.</returns>
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
    ''' Creates a status summary for this task.
    ''' </summary>
    ''' <param name="fullLength">A value indicating whether full details shall be included.</param>
    ''' <returns>The task summary text.</returns>
    Public Function SummaryText(Optional fullLength As Boolean = False) As String
        Dim Result As New System.Text.StringBuilder
        Result.Append(Me.LoggedExceptionsToPlainText(fullLength)) 'either empty or contains exceptions with line-break after last exception
        If Result.Length <> 0 Then Result.AppendLine()
        Dim Warnings = CollectedWarningsText()
        If Warnings <> Nothing Then
            Result.Append(CollectedWarningsText) 'either empty or contains warnings with line-break at end of last warning 
            If Result.Length <> 0 Then Result.AppendLine()
        End If
        Result.Append(Me.ConsumedTimeStatisticsInfo(fullLength)) 'always contains some lines and line-break at last line
        Return Result.ToString
    End Function

End Class
