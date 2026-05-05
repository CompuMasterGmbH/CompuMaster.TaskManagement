Imports CompuMaster.TaskManagement.ProgressingTaskBundle
Imports CompuMaster.TaskManagement.Exceptions

''' <summary>
''' Provides common state and execution logic for task steps.
''' </summary>
Public MustInherit Class ProgressingTaskStepBase

    ''' <summary>
    ''' Initializes a new task step.
    ''' </summary>
    ''' <param name="stepTitle">The display title of the step.</param>
    ''' <param name="stepAction">The action to execute for the step.</param>
    ''' <param name="estimatedTimeToRun">The estimated duration of the step, or <see langword="Nothing"/> if the duration is unknown.</param>
    ''' <param name="failAction">The action to apply when the step fails.</param>
    Public Sub New(stepTitle As String, stepAction As StepActionMethod, estimatedTimeToRun As TimeSpan?, failAction As ProgressingTaskStepFailAction)
        If failAction = ProgressingTaskStepFailAction.DependingOnResultOfStepActionMethodWithFailAction Then Throw New ArgumentException("Not supported: fail action = " & failAction.ToString, NameOf(failAction))
        Me.StepTitle = stepTitle
        Me.StepAction = stepAction
        Me.EstimatedTimeToRun = estimatedTimeToRun
        Me.FailAction = failAction
    End Sub

    ''' <summary>
    ''' Initializes a new task step with a dynamic fail action.
    ''' </summary>
    ''' <param name="stepTitle">The display title of the step.</param>
    ''' <param name="stepAction">The action to execute for the step and return the step result.</param>
    ''' <param name="estimatedTimeToRun">The estimated duration of the step, or <see langword="Nothing"/> if the duration is unknown.</param>
    Protected Sub New(stepTitle As String, stepAction As StepActionMethodWithFailAction, estimatedTimeToRun As TimeSpan?)
        Me.StepTitle = stepTitle
        Me.StepActionWithFailAction = stepAction
        Me.EstimatedTimeToRun = estimatedTimeToRun
        Me.FailAction = ProgressingTaskStepFailAction.DependingOnResultOfStepActionMethodWithFailAction
    End Sub

    ''' <summary>
    ''' Represents a method that executes a step action.
    ''' </summary>
    ''' <param name="taskStep">The step that is being executed.</param>
    Public Delegate Sub StepActionMethod(taskStep As ProgressingTaskStepBase)

    ''' <summary>
    ''' Represents a method that executes a step action and returns the failure behavior.
    ''' </summary>
    ''' <param name="taskstep">The step that is being executed.</param>
    ''' <returns>The result of the step action.</returns>

    Public Delegate Function StepActionMethodWithFailAction(taskstep As ProgressingTaskStepBase) As TaskStepActionResult

    ''' <summary>
    ''' Gets or sets the parent task that owns this step.
    ''' </summary>
    Public Property ParentTask As ProgressingTaskItem

    ''' <summary>
    ''' Gets or sets the display title of the step.
    ''' </summary>
    Public Property StepTitle As String

    ''' <summary>
    ''' Gets or sets the action to execute for the step.
    ''' </summary>
    Public Property StepAction As StepActionMethod

    ''' <summary>
    ''' Gets or sets the dynamic action to execute for the step.
    ''' </summary>
    Public Property StepActionWithFailAction As StepActionMethodWithFailAction

    ''' <summary>
    ''' Gets or sets the estimated duration of the step.
    ''' </summary>
    Public Property EstimatedTimeToRun As TimeSpan?

    ''' <summary>
    ''' Gets the estimated remaining duration of the step.
    ''' </summary>
    Public ReadOnly Property EstimatedTimeOfArrival As TimeSpan?
        Get
            If Me.EstimatedTimeToRun.HasValue = False Then
                ' Wenn die geschätzte Laufzeit nicht verfügbar ist, geben Sie Nothing zurück
                Return Nothing
            ElseIf Me.Status = ProgressingTaskStepStatus.NotStarted Then
                ' Wenn der Schritt noch nicht gestartet wurde, geben Sie die vollständige geschätzte Laufzeit zurück
                Return Me.EstimatedTimeToRun
            ElseIf Me.ConsumedTime.HasValue = False Then
                ' Wenn die verbrauchte Laufzeit nicht verfügbar ist, geben Sie die vollständige geschätzte Laufzeit zurück
                Return Me.EstimatedTimeToRun.Value
            ElseIf Me.IsRunningOrAbortingOrFailing AndAlso Me.EstimatedTimeToRun.Value > Me.ConsumedTime.Value Then
                ' Wenn der Schritt noch läuft und die geschätzte Laufzeit noch nicht überschritten wurde, geben Sie die verbleibende Zeit zurück
                Return Me.EstimatedTimeToRun.Value - Me.ConsumedTime.Value
            Else
                ' Wenn der Schritt abgeschlossen ist oder die geschätzte Laufzeit bereits überschritten wurde, geben Sie TimeSpan.Zero zurück
                Return TimeSpan.Zero
            End If
        End Get
    End Property

    Friend Property StepExecutionResult As TaskStepActionResult

    ''' <summary>
    ''' Runs the step action.
    ''' </summary>
    Public Sub Run()
        Select Case Me.Status
            Case ProgressingTaskStepStatus.NotStarted
                If Me.ParentTask Is Nothing Then Throw New InvalidOperationException("ParentTask is not set")
                Me.Status = ProgressingTaskStepStatus.InProgress
                _StartTime = DateTime.Now
                Try
                    If Me.StepAction IsNot Nothing Then
                        Me.StepAction.Invoke(Me)
                    ElseIf Me.StepActionWithFailAction IsNot Nothing Then
                        Me.StepExecutionResult = Me.StepActionWithFailAction.Invoke(Me)
                        If Me.StepExecutionResult Is Nothing Then Throw New NotImplementedException("Implementation incomplete at step " & Me.StepTitle & ": Step action method must return ProgressingTaskStepDynamicFailAction")
                        Me.FailAction = StepExecutionResult.FailAction
                        If Me.StepExecutionResult.CompletedSuccessful = False Then
                            Throw New InternalStepExecutionException(Me.StepExecutionResult.Exception)
                        End If
                    Else
                        Throw New InvalidOperationException("No step action defined")
                    End If
                    _EndTime = DateTime.Now
                    Me.Status = ProgressingTaskStepStatus.Completed
                Catch ex As InternalStepExecutionException
                    Me.Status = ProgressingTaskStepStatus.Failed
                    _EndTime = DateTime.Now
                    Me.FoundException = ex.InnerException
                    Throw New StepException("Fehlgeschlagener Einzelschritt: " & StepTitle, ex.InnerException)
                Catch ex As Exception
                    Me.Status = ProgressingTaskStepStatus.Failed
                    _EndTime = DateTime.Now
                    Me.FoundException = ex
                    Throw New StepException("Fehlgeschlagener Einzelschritt: " & StepTitle, ex)
                End Try
            Case ProgressingTaskStepStatus.InProgress
                Throw New InvalidOperationException("This step is already in progress")
            Case ProgressingTaskStepStatus.Completed
                Throw New InvalidOperationException("This step already completed")
            Case ProgressingTaskStepStatus.Failed
                Throw New InvalidOperationException("This step already ran and failed")
            Case Else
                Throw New NotSupportedException("Unknown status")
        End Select
    End Sub

    Private _StartTime As DateTime?
    Private _EndTime As DateTime?
    ''' <summary>
    ''' Gets the consumed runtime of the step.
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
    ''' Defines the execution status of a task step.
    ''' </summary>
    Public Enum ProgressingTaskStepStatus As Byte
        ''' <summary>
        ''' The step has not started.
        ''' </summary>
        NotStarted = 0
        ''' <summary>
        ''' The step is running.
        ''' </summary>
        InProgress = 1
        ''' <summary>
        ''' The step completed successfully.
        ''' </summary>
        Completed = 2
        ''' <summary>
        ''' The step failed.
        ''' </summary>
        Failed = 3
    End Enum

    ''' <summary>
    ''' Gets or sets the execution status of the step.
    ''' </summary>
    Public Property Status As ProgressingTaskStepStatus = ProgressingTaskStepStatus.NotStarted

    ''' <summary>
    ''' Defines how a failed task step is handled.
    ''' </summary>
    Public Enum ProgressingTaskStepFailAction As Integer
        ''' <summary>
        ''' The step action result declares the fail action.
        ''' </summary>
        DependingOnResultOfStepActionMethodWithFailAction = -1
        ''' <summary>
        ''' Logs the exception and continues with the next step.
        ''' </summary>
        LogExceptionAndContinue = 0
        ''' <summary>
        ''' Logs the exception and throws it to fail the task.
        ''' </summary>
        ThrowException = 1
    End Enum

    ''' <summary>
    ''' Gets or sets the action to apply when the step fails.
    ''' </summary>
    Public Property FailAction As ProgressingTaskStepFailAction

    ''' <summary>
    ''' Gets or sets the exception found during step execution.
    ''' </summary>
    Public Property FoundException As Exception

    ''' <inheritdoc/>
    Public Overrides Function ToString() As String
        Return Me.StepTitle & " [" & Me.Status.ToString() & "]"
    End Function

    Private ReadOnly _CollectedWarnings As List(Of ValueTuple(Of String, String)) = New List(Of ValueTuple(Of String, String))

    ''' <summary>
    ''' Gets the warnings collected for this step.
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
    ''' Gets a value indicating whether the step is currently running.
    ''' </summary>
    Public ReadOnly Property IsRunningOrAbortingOrFailing As Boolean
        Get
            Select Case Me.Status
                Case ProgressingTaskStepStatus.InProgress ', ProgressingTaskStepStatus.Aborting, ProgressingTaskStepStatus.FailingNonCritically, ProgressingTaskStepStatus.FailingInCriticalState
                    Return True
                Case Else
                    Return False
            End Select
        End Get
    End Property

End Class
