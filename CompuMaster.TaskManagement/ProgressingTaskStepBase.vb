Imports CompuMaster.TaskManagement.ProgressingTaskBundle
Imports CompuMaster.TaskManagement.Exceptions

Public MustInherit Class ProgressingTaskStepBase

    ''' <summary>
    ''' Step action item - Step action doesn't need any try-catch handling
    ''' </summary>
    ''' <param name="stepTitle"></param>
    ''' <param name="stepAction"></param>
    ''' <param name="estimatedTimeToRun"></param>
    ''' <param name="failAction"></param>
    Public Sub New(stepTitle As String, stepAction As StepActionMethod, estimatedTimeToRun As TimeSpan?, failAction As ProgressingTaskStepFailAction)
        If failAction = ProgressingTaskStepFailAction.DependingOnResultOfStepActionMethodWithFailAction Then Throw New ArgumentException("Not supported: fail action = " & failAction.ToString, NameOf(failAction))
        Me.StepTitle = stepTitle
        Me.StepAction = stepAction
        Me.EstimatedTimeToRun = estimatedTimeToRun
        Me.FailAction = failAction
    End Sub

    ''' <summary>
    ''' Step action item - Step action must catch exceptions and assign result ProgressingTaskStepDynamicFailAction
    ''' </summary>
    ''' <param name="stepTitle"></param>
    ''' <param name="stepAction"></param>
    ''' <param name="estimatedTimeToRun"></param>
    Protected Sub New(stepTitle As String, stepAction As StepActionMethodWithFailAction, estimatedTimeToRun As TimeSpan?)
        Me.StepTitle = stepTitle
        Me.StepActionWithFailAction = stepAction
        Me.EstimatedTimeToRun = estimatedTimeToRun
        Me.FailAction = ProgressingTaskStepFailAction.DependingOnResultOfStepActionMethodWithFailAction
    End Sub

    ''' <summary>
    ''' A method with actions for this step - Step action doesn't need any try-catch handling
    ''' </summary>
    ''' <param name="taskStep"></param>
    Public Delegate Sub StepActionMethod(taskStep As ProgressingTaskStepBase)

    ''' <summary>
    ''' A method with actions for this step - Step action must catch exceptions and assign FailAction before re-throwing the exception
    ''' </summary>
    ''' <param name="taskstep"></param>
    ''' <returns></returns>

    Public Delegate Function StepActionMethodWithFailAction(taskstep As ProgressingTaskStepBase) As ProgressingTaskStepDynamicFailAction

    Public Property ParentTask As ProgressingTaskItem
    Public Property StepTitle As String
    Public Property StepAction As StepActionMethod
    Public Property StepActionWithFailAction As StepActionMethodWithFailAction
    Public Property EstimatedTimeToRun As TimeSpan?

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

    Friend Property StepExecutionResult As ProgressingTaskStepDynamicFailAction

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
                    Me.FoundException = ex
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
    ''' Lauzeit
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

    Public Enum ProgressingTaskStepStatus As Byte
        NotStarted = 0
        InProgress = 1
        Completed = 2
        Failed = 3
    End Enum

    Public Property Status As ProgressingTaskStepStatus = ProgressingTaskStepStatus.NotStarted

    Public Enum ProgressingTaskStepFailAction As Integer
        ''' <summary>
        ''' The delegate method of type StepActionMethodWithFailAction also declares the fail action
        ''' </summary>
        DependingOnResultOfStepActionMethodWithFailAction = -1
        ''' <summary>
        ''' Log the exception, mark this step as failed (will lead to continue with next step in ProgressingTaskItem)
        ''' </summary>
        LogExceptionAndContinue = 0
        ''' <summary>
        ''' Log the exception, mark this step as failed, throw the exception (will lead to fail ProgressingTaskItem.RunAllSteps)
        ''' </summary>
        ThrowException = 1
    End Enum

    Public Property FailAction As ProgressingTaskStepFailAction

    Public Property FoundException As Exception

    Public Overrides Function ToString() As String
        Return Me.StepTitle & " [" & Me.Status.ToString() & "]"
    End Function

    Private ReadOnly _CollectedWarnings As List(Of ValueTuple(Of String, String)) = New List(Of ValueTuple(Of String, String))

    Public ReadOnly Property CollectedWarnings As List(Of ValueTuple(Of String, String))
        Get
            Return _CollectedWarnings
        End Get
    End Property

    Public Sub AddWarning(warning As String)
        AddWarning(warning, GetStackTraceWithoutLastMethod)
    End Sub

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
