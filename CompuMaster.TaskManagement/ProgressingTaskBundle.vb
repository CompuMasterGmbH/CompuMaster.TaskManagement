Imports CompuMaster.TaskManagement.ProgressingTaskItem

''' <summary>
''' Represents a bundle of progressing tasks.
''' </summary>
Public Class ProgressingTaskBundle

    ''' <summary>
    ''' Initializes a new task bundle.
    ''' </summary>
    ''' <param name="title">The display title of the task bundle.</param>
    Public Sub New(title As String)
        Me.TaskBundleTitle = title
    End Sub

    ''' <summary>
    ''' Gets or sets the display title of the task bundle.
    ''' </summary>
    Public Property TaskBundleTitle As String

    Private ReadOnly _Tasks As New List(Of ProgressingTaskItem)

    ''' <summary>
    ''' Gets the tasks in this bundle.
    ''' </summary>
    Public ReadOnly Property Tasks As IReadOnlyList(Of ProgressingTaskItem)
        Get
            Return _Tasks
        End Get
    End Property

    ''' <summary>
    ''' Creates a task and adds it to this bundle.
    ''' </summary>
    ''' <param name="taskTitle">The display title of the task.</param>
    ''' <returns>The created task.</returns>
    Public Function CreateAndAddNewTask(taskTitle As String) As ProgressingTaskItem
        Return New ProgressingTaskItem(Me, taskTitle)
    End Function

    ''' <summary>
    ''' Creates a task with the specified run behavior and adds it to this bundle.
    ''' </summary>
    ''' <param name="taskTitle">The display title of the task.</param>
    ''' <param name="runBehaviourIfPreviousTasksFailed">The run behavior when previous tasks failed or were cancelled.</param>
    ''' <returns>The created task.</returns>
    Public Function CreateAndAddNewTask(taskTitle As String, runBehaviourIfPreviousTasksFailed As RunBehaviourIfPreviousTasksFailedEnum) As ProgressingTaskItem
        Return New ProgressingTaskItem(Me, taskTitle, runBehaviourIfPreviousTasksFailed)
    End Function

    Friend Sub AddTask(taskItem As ProgressingTaskItem)
        If taskItem Is Nothing Then
            Throw New ArgumentNullException(NameOf(taskItem))
        End If
        If _Tasks.Contains(taskItem) Then
            Throw New InvalidOperationException("TaskItem already added to this TaskBundle")
        End If
        If taskItem.ParentTaskBundle IsNot Me Then
            Throw New InvalidOperationException("TaskItem does not belong to this TaskBundle")
        End If
        _Tasks.Add(taskItem)
    End Sub

    ''' <summary>
    ''' Defines the execution status of a task bundle.
    ''' </summary>
    Public Enum ProgressingTaskBundleStatus As Byte
        ''' <summary>
        ''' The task bundle has not started.
        ''' </summary>
        NotStarted = 0
        ''' <summary>
        ''' The task bundle is running.
        ''' </summary>
        InProgress = 1
        ''' <summary>
        ''' The task bundle completed successfully.
        ''' </summary>
        CompletedSuccessfully = 2
        ''' <summary>
        ''' The task bundle failed but ended in a defined state.
        ''' </summary>
        FailedNonCritically = 30
        ''' <summary>
        ''' The task bundle failed and ended in a critical state.
        ''' </summary>
        FailedInCriticalState = 31
        ''' <summary>
        ''' The user cancelled the task bundle and rollback completed successfully.
        ''' </summary>
        Aborted = 5
        ''' <summary>
        ''' The task bundle is aborting after a user cancellation request.
        ''' </summary>
        Aborting = 6
        ''' <summary>
        ''' The task bundle is failing but is expected to end in a defined state.
        ''' </summary>
        FailingNonCritically = 40
        ''' <summary>
        ''' The task bundle is failing and is expected to end in a critical state.
        ''' </summary>
        FailingInCriticalState = 41
    End Enum

    ''' <summary>
    ''' Gets or sets the execution status of the task bundle.
    ''' </summary>
    Public Property Status As ProgressingTaskBundle.ProgressingTaskBundleStatus = ProgressingTaskBundle.ProgressingTaskBundleStatus.NotStarted

    ''' <summary>
    ''' Requests cancellation for all running tasks.
    ''' </summary>
    Public Sub CancelAllTasks()
        Me.CancellationTokenSource.Cancel()
    End Sub

    ''' <summary>
    ''' Gets the cancellation token source used by this task bundle.
    ''' </summary>
    Public ReadOnly Property CancellationTokenSource As New Threading.CancellationTokenSource

    ''' <summary>
    ''' Runs all tasks asynchronously.
    ''' </summary>
    ''' <returns>A task that represents the asynchronous run operation.</returns>
    Public Async Function RunAllTasksAsync() As Task
        Await RunAllTasksAsync(Me.CancellationTokenSource.Token)
    End Function

    ''' <summary>
    ''' Runs all tasks synchronously.
    ''' </summary>
    Public Sub RunAllTasks()
        RunAllTasksAsync(CancellationTokenSource.Token).GetAwaiter().GetResult()
    End Sub

    Private Async Function RunAllTasksAsync(cancellationToken As Threading.CancellationToken) As Task
        ' Überprüfung auf doppelte Instanzen (Objektidentität)
        Dim duplicates = Tasks.GroupBy(Function(t) t).Where(Function(g) g.Count() > 1).Select(Function(g) g.Key).ToList()
        If duplicates.Count <> 0 Then
            Throw New InvalidOperationException("Doppelte Instanzen von ProgressingTaskItem in der Aufgabenliste gefunden.")
        End If

        'Prüfung auf Status und ggf. Ausführung
        Select Case Me.Status
            Case ProgressingTaskBundle.ProgressingTaskBundleStatus.NotStarted
                Me.Status = ProgressingTaskBundle.ProgressingTaskBundleStatus.InProgress

                Dim MyCounter As Integer = 0
                While MyCounter < Tasks.Count
                    Dim CurrentTask As ProgressingTaskItem = Tasks(MyCounter)
                    Dim NoCancellationRequestedAndNoExceptionsLogged As Boolean = Not cancellationToken.IsCancellationRequested AndAlso Me.LoggedExceptions.Count = 0
                    Dim IsRunAlwaysTask As Boolean = CurrentTask.RunBehaviourIfPreviousTasksFailed = ProgressingTaskItem.RunBehaviourIfPreviousTasksFailedEnum.RunAlways
                    If IsRunAlwaysTask OrElse NoCancellationRequestedAndNoExceptionsLogged Then
                        'Run only those tasks which must always run OR run all tasks if no exceptions occured yet
                        '=> Run the task or the always-run-task
                        Try
                            Await CurrentTask.RunAllStepsAsync(cancellationToken)
                            If CurrentTask.LoggedExceptions.Count <> 0 Then
                                Throw New InvalidOperationException("Task failed, but no exception was thrown -> invalid implementation detected")
                            End If
                            'Task completed successfully, we can continue with next task (keep status as is at InProgress/Aborting/Failing)
                        Catch ex As Exception
                            Me.LoggedExceptions.Add(ex)
                            Select Case CurrentTask.Status
                                Case ProgressingTaskItem.ProgressingTaskStatus.FailedWithRollbackOption
                                    If Me.Status = ProgressingTaskBundleStatus.FailingInCriticalState Then
                                        'don't override the critical status
                                    Else
                                        Me.Status = ProgressingTaskBundleStatus.FailingNonCritically
                                    End If
                                Case ProgressingTaskItem.ProgressingTaskStatus.FailedInCriticalState
                                    Me.Status = ProgressingTaskBundleStatus.FailingInCriticalState
                                Case ProgressingTaskItem.ProgressingTaskStatus.Aborted
                                    'Status will be refreshed later if no previous tasks already failed
                                Case Else
                                    Throw New NotImplementedException("Unexpected task status: " & CurrentTask.Status.ToString())
                            End Select
                        End Try
                    Else
                        ' Skip the task 
                        CurrentTask.MarkAsSkipped()
                    End If

                    If Me.Status = ProgressingTaskBundleStatus.InProgress AndAlso CurrentTask.Status = ProgressingTaskItem.ProgressingTaskStatus.Aborted Then 'if already failing, don't change status to aborting
                        'Mark the whole task bundle as aborted
                        Me.Status = ProgressingTaskBundleStatus.Aborting
                    End If
                    MyCounter += 1
                End While

                'Switch to final status
                Select Case Me.Status
                    Case ProgressingTaskBundle.ProgressingTaskBundleStatus.Aborting
                        'Task bundle was aborted
                        Me.Status = ProgressingTaskBundle.ProgressingTaskBundleStatus.Aborted
                        Throw New Exceptions.UserAbortedMessageException("Aufgabe " & TaskBundleTitle & " wurde abgebrochen")
                    Case ProgressingTaskBundle.ProgressingTaskBundleStatus.FailingNonCritically
                        'Task bundle failed, but no critical operations were affected
                        Me.Status = ProgressingTaskBundle.ProgressingTaskBundleStatus.FailedNonCritically
                        Throw New AggregateException("ERROR: Aufgabe " & TaskBundleTitle & " fehlgeschlagen")
                    Case ProgressingTaskBundle.ProgressingTaskBundleStatus.FailingInCriticalState
                        'Task bundle failed and critical operations left the system in an undefined/critical state
                        Me.Status = ProgressingTaskBundle.ProgressingTaskBundleStatus.FailedInCriticalState
                        Throw New AggregateException("ERROR: Aufgabe " & TaskBundleTitle & " fehlgeschlagen - ACHTUNG: das System befindet sich aufgrund fehlender Korrektur-Möglichkeiten in einem undefinierten/kritischen Zustand")
                    Case ProgressingTaskBundle.ProgressingTaskBundleStatus.InProgress
                        'Task bundle completed successfully
                        Me.Status = ProgressingTaskBundle.ProgressingTaskBundleStatus.CompletedSuccessfully
                    Case Else
                        Throw New NotImplementedException("Unexpected task bundle status: " & Me.Status.ToString())
                End Select
                'If Me.LoggedExceptions.Count <> 0 Then
                '    If Me.Status = ProgressingTaskBundleStatus.Aborting AndAlso Me.LoggedExceptions.Count = 1 AndAlso Me.LoggedExceptions(0).GetType Is GetType(Exceptions.UserAbortedMessageException) Then 'if aborted and no rollback exceptions occured -> logged exceptions contains only 1 aborted-exception
                '        Me.Status = ProgressingTaskBundle.ProgressingTaskBundleStatus.Aborted
                '        Throw New AggregateException("Aufgabe " & TaskBundleTitle & " wurde abgebrochen")
                '    Else
                '        Me.Status = ProgressingTaskBundle.ProgressingTaskBundleStatus.FailedNonCritically
                '        For Each CurrentTask In Tasks
                '            If CurrentTask.Status = ProgressingTaskItem.ProgressingTaskStatus.FailedInCriticalState Then
                '                Me.Status = ProgressingTaskBundle.ProgressingTaskBundleStatus.FailedInCriticalStatus
                '                Exit For
                '            End If
                '        Next
                '        Throw New AggregateException("ERROR: Aufgabe " & TaskBundleTitle & " fehlgeschlagen")
                '    End If
                'Else
                '    Me.Status = ProgressingTaskBundle.ProgressingTaskBundleStatus.CompletedSuccessfully
                'End If
            Case ProgressingTaskBundle.ProgressingTaskBundleStatus.InProgress
                Throw New InvalidOperationException("This task bundle is already in progress")
            Case ProgressingTaskBundle.ProgressingTaskBundleStatus.CompletedSuccessfully
                Throw New InvalidOperationException("This task bundle already completed")
            Case ProgressingTaskBundle.ProgressingTaskBundleStatus.FailedNonCritically, ProgressingTaskBundleStatus.FailedInCriticalState
                Throw New InvalidOperationException("This task bundle already ran and failed")
            Case ProgressingTaskBundle.ProgressingTaskBundleStatus.Aborting
                Throw New InvalidOperationException("This task bundle is already aborting")
            Case ProgressingTaskBundle.ProgressingTaskBundleStatus.Aborted
                Throw New InvalidOperationException("This task bundle already aborted")
            Case Else
                Throw New NotSupportedException("Unknown status")
        End Select
    End Function

    ''' <summary>
    ''' Gets the exceptions logged during task bundle execution.
    ''' </summary>
    Public ReadOnly Property LoggedExceptions As New List(Of Exception)

    ''' <inheritdoc/>
    Public Overrides Function ToString() As String
        Return Me.TaskBundleTitle & " [" & Me.Status.ToString() & "]"
    End Function

    ''' <summary>
    ''' Gets a value indicating whether the task bundle is running, aborting, or failing.
    ''' </summary>
    Public ReadOnly Property IsRunningOrAbortingOrFailing As Boolean
        Get
            Select Case Me.Status
                Case ProgressingTaskBundleStatus.InProgress, ProgressingTaskBundleStatus.Aborting, ProgressingTaskBundleStatus.FailingNonCritically, ProgressingTaskBundleStatus.FailingInCriticalState
                    Return True
                Case Else
                    Return False
            End Select
        End Get
    End Property

    ''' <summary>
    ''' Gets a value indicating whether the task bundle is running or failing.
    ''' </summary>
    Public ReadOnly Property IsRunningOrFailing As Boolean
        Get
            Select Case Me.Status
                Case ProgressingTaskBundleStatus.InProgress, ProgressingTaskBundleStatus.FailingNonCritically, ProgressingTaskBundleStatus.FailingInCriticalState
                    Return True
                Case Else
                    Return False
            End Select
        End Get
    End Property

    ''' <summary>
    ''' Gets the estimated duration for all tasks.
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property EstimatedTimeToRun As TimeSpan?
        Get
            Dim Result As New TimeSpan?
            For Each CurrentTask In Tasks
                Dim TaskItemTimeSpan As TimeSpan? = CurrentTask.EstimatedTimeToRun
                If TaskItemTimeSpan.HasValue Then
                    If Result.HasValue = False Then Result = TimeSpan.Zero
                    Result = Result + TaskItemTimeSpan
                End If
            Next
            Return Result
        End Get
    End Property

    ''' <summary>
    ''' Gets the estimated remaining duration for all tasks.
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property EstimatedTimeOfArrival As TimeSpan?
        Get
            Dim Result As New TimeSpan?
            For Each CurrentTask In Tasks
                Dim TaskItemTimeSpan As TimeSpan? = CurrentTask.EstimatedTimeOfArrival
                If TaskItemTimeSpan.HasValue Then
                    If Result.HasValue = False Then Result = TimeSpan.Zero
                    Result = Result + TaskItemTimeSpan
                End If
            Next
            Return Result
        End Get
    End Property

    ''' <summary>
    ''' Gets the task bundle title, status, and estimated time of arrival.
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property DisplayTitleAndStatusAndETA As String
        Get
            Dim Result As String

            'Add task title + status to the form title
            Select Case Me.Status
                Case ProgressingTaskBundle.ProgressingTaskBundleStatus.NotStarted
                    Result = Me.TaskBundleTitle & " - Noch nicht gestartet"
                Case ProgressingTaskBundle.ProgressingTaskBundleStatus.InProgress
                    Result = Me.TaskBundleTitle & " - In Bearbeitung"
                Case ProgressingTaskBundle.ProgressingTaskBundleStatus.FailingNonCritically
                    Result = Me.TaskBundleTitle & " - Fehlgeschlagen - Wird beendet"
                Case ProgressingTaskBundle.ProgressingTaskBundleStatus.FailingInCriticalState
                    Result = Me.TaskBundleTitle & " - Fehlgeschlagen - Wird beendet - Kritischer System-Zustand wird verbleiben"
                Case ProgressingTaskBundle.ProgressingTaskBundleStatus.Aborting
                    Result = Me.TaskBundleTitle & " - Wird abgebrochen"
                Case ProgressingTaskBundle.ProgressingTaskBundleStatus.CompletedSuccessfully
                    Result = Me.TaskBundleTitle & " - Erfolgreich abgeschlossen"
                Case ProgressingTaskBundle.ProgressingTaskBundleStatus.FailedNonCritically
                    Result = Me.TaskBundleTitle & " - Fehlgeschlagen"
                Case ProgressingTaskBundle.ProgressingTaskBundleStatus.FailedInCriticalState
                    Result = Me.TaskBundleTitle & " - Fehlgeschlagen - Kritischer System-Zustand verblieben"
                Case ProgressingTaskBundle.ProgressingTaskBundleStatus.Aborted
                    Result = Me.TaskBundleTitle & " - Abgebrochen"
                Case Else
                    Throw New NotSupportedException("Unknown status")
            End Select

            'Add ETA or ETR to the form title
            With Nothing
                Dim ETA = Me.EstimatedTimeOfArrival
                If ETA.HasValue Then
                    Result &= " - ETA: " & ETA.Value.ToString("d\.hh\:mm\:ss", System.Globalization.CultureInfo.InvariantCulture)
                Else
                    Dim ETR = Me.EstimatedTimeToRun
                    If ETR.HasValue Then
                        Result &= " - ETA: " & ETR.Value.ToString("d\.hh\:mm\:ss", System.Globalization.CultureInfo.InvariantCulture)
                    End If
                End If
            End With

            Return Result
        End Get
    End Property

    ''' <summary>
    ''' Creates a summary text for display.
    ''' </summary>
    ''' <returns>The display summary.</returns>
    Public Function DisplaySummary() As String
        Dim Result As New System.Text.StringBuilder
        For Each TaskItem In Me.Tasks
            If Result.Length <> 0 Then
                Result.AppendLine()
                Result.AppendLine()
            End If
            Result.AppendLine("## " & TaskItem.TaskTitle)
            Result.AppendLine()
            Result.Append(TaskItem.SummaryText)
        Next
        Return Result.ToString
    End Function

End Class
