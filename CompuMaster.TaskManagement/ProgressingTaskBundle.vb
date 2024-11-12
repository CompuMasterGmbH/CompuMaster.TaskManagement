Imports CompuMaster.TaskManagement.ProgressingTaskItem

Public Class ProgressingTaskBundle

    Public Sub New(title As String)
        Me.TaskBundleTitle = title
    End Sub

    Public Property TaskBundleTitle As String

    Private ReadOnly _Tasks As New List(Of ProgressingTaskItem)

    Public ReadOnly Property Tasks As IReadOnlyList(Of ProgressingTaskItem)
        Get
            Return _Tasks
        End Get
    End Property

    Public Function CreateAndAddNewTask(taskTitle As String) As ProgressingTaskItem
        Return New ProgressingTaskItem(Me, taskTitle)
    End Function

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

    Public Enum ProgressingTaskBundleStatus As Byte
        ''' <summary>
        ''' Not yet started
        ''' </summary>
        NotStarted = 0
        ''' <summary>
        ''' Running regularly without any problems
        ''' </summary>
        InProgress = 1
        ''' <summary>
        ''' Completed without any problems
        ''' </summary>
        CompletedSuccessfully = 2
        ''' <summary>
        ''' Failed non-critically (some tasks failed, but the whole task bundle ended and left the system in a defined/stable situation)
        ''' </summary>
        FailedNonCritically = 30
        ''' <summary>
        ''' Failed in a critical status (some tasks failed and the whole task bundle stopped, leaving the system in an undefined/unstable/critical situation)
        ''' </summary>
        FailedInCriticalState = 31
        ''' <summary>
        ''' User cancelled the task bundle execution (a rollback task ran successfully and left the system in a defined/stable situation)
        ''' </summary>
        Aborted = 5
        ''' <summary>
        ''' User requested to abort the task bundle execution (a rollback task is running and will leave the system in a defined/stable situation)
        ''' </summary>
        Aborting = 6
        ''' <summary>
        ''' One or more task steps failed, so the task bundle will fail non-critically (some tasks failed, but the whole task bundle will end and leave the system in a defined/stable situation)
        ''' </summary>
        FailingNonCritically = 40
        ''' <summary>
        ''' One or more task steps failed, so the task bundle will fail in a critical status (some tasks failed and the whole task bundle will stop, leaving the system in an undefined/unstable/critical situation)
        ''' </summary>
        FailingInCriticalState = 41
    End Enum

    Public Property Status As ProgressingTaskBundle.ProgressingTaskBundleStatus = ProgressingTaskBundle.ProgressingTaskBundleStatus.NotStarted

    Public Sub CancelAllTasks()
        Me.CancellationTokenSource.Cancel()
    End Sub

    Public ReadOnly Property CancellationTokenSource As New Threading.CancellationTokenSource

    Public Async Function RunAllTasksAsync() As Task
        Await RunAllTasksAsync(Me.CancellationTokenSource.Token)
    End Function

    Public Sub RunAllTasks()
        Dim runAllTask As Task = RunAllTasksAsync(CancellationTokenSource.Token)
        runAllTask.Wait()
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

    Public ReadOnly Property LoggedExceptions As New List(Of Exception)

    Public Overrides Function ToString() As String
        Return Me.TaskBundleTitle & " [" & Me.Status.ToString() & "]"
    End Function

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
    ''' Estimated time to run all steps
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
    ''' Estimated remaining time to run
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
    ''' The task bundle title, its status and (if availabe) the ETA (estimated time of arrival)
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
    ''' A summary text for display
    ''' </summary>
    ''' <returns></returns>
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
