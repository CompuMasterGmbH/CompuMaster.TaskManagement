Public NotInheritable Class DummyTaskBundles

    ''' <summary>
    ''' Dummy Task Bundle with exception in first steps and rollback
    ''' </summary>
    ''' <returns></returns>
    Public Shared Function DummyTaskBundleFailingWithRollback() As ProgressingTaskBundle
        Dim Result As New ProgressingTaskBundle("Dummy Task Bundle with exception and rollback")
        Dim Task1 As New ProgressingTaskItem(Result, "Dummy Task 1")
        Task1.FirstStepsWhichCanBeRolledBack.Add(New ProgressingTaskFailFastStep("Dummy Step 1.1", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1)))
        Task1.FirstStepsWhichCanBeRolledBack.Add(New ProgressingTaskFailFastStep("Dummy Step 1.2", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1)))
        Task1.FirstStepsWhichCanBeRolledBack.Add(New ProgressingTaskFailFastStep("Dummy Step 1.3", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1)))
        Task1.FirstStepsWhichCanBeRolledBack.Add(New ProgressingTaskFailFastStep("Dummy Step 1.4", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1)))
        Task1.FirstStepsWhichCanBeRolledBack.Add(New ProgressingTaskFailFastStep("Dummy Step 1.5", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1)))
        Task1.FirstStepsWhichCanBeRolledBack.Add(New ProgressingTaskFailFastStep("Dummy Step 1.6 FAILING", Sub() Throw New Exception("Failure-Test in Dummy"), New TimeSpan(0, 0, 1)))
        Task1.FirstStepsWhichCanBeRolledBack.Add(New ProgressingTaskFailFastStep("Dummy Step 1.7", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1)))
        Task1.SecondStepsWithoutRollbackOption.Add(New ProgressingTaskStep("Dummy Step 2.1", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1), ProgressingTaskStep.ProgressingTaskStepFailAction.LogExceptionAndContinue))
        Task1.SecondStepsWithoutRollbackOption.Add(New ProgressingTaskStep("Dummy Step 2.2", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1), ProgressingTaskStep.ProgressingTaskStepFailAction.LogExceptionAndContinue))
        Task1.SecondStepsWithoutRollbackOption.Add(New ProgressingTaskStep("Dummy Step 2.3", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1), ProgressingTaskStep.ProgressingTaskStepFailAction.LogExceptionAndContinue))
        Task1.SecondStepsWithoutRollbackOption.Add(New ProgressingTaskStep("Dummy Step 2.4", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1), ProgressingTaskStep.ProgressingTaskStepFailAction.LogExceptionAndContinue))
        Task1.RollbackSteps.Add(New ProgressingTaskStep("Dummy Rollback Step 1", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1), ProgressingTaskStep.ProgressingTaskStepFailAction.LogExceptionAndContinue))
        Task1.RollbackSteps.Add(New ProgressingTaskStep("Dummy Rollback Step 2", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1), ProgressingTaskStep.ProgressingTaskStepFailAction.LogExceptionAndContinue))
        Task1.RollbackSteps.Add(New ProgressingTaskStep("Dummy Rollback Step 3", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1), ProgressingTaskStep.ProgressingTaskStepFailAction.LogExceptionAndContinue))
        Task1.RollbackSteps.Add(New ProgressingTaskStep("Dummy Rollback Step 4", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1), ProgressingTaskStep.ProgressingTaskStepFailAction.LogExceptionAndContinue))
        Dim Task2 As New ProgressingTaskItem(Result, "Dummy Task 2")
        Task2.SecondStepsWithoutRollbackOption.Add(New ProgressingTaskStep("Dummy Task 2 Step 1", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1), ProgressingTaskStep.ProgressingTaskStepFailAction.LogExceptionAndContinue))
        Task2.SecondStepsWithoutRollbackOption.Add(New ProgressingTaskStep("Dummy Task 2 Step 2", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1), ProgressingTaskStep.ProgressingTaskStepFailAction.LogExceptionAndContinue))
        Return Result
    End Function

    ''' <summary>
    ''' After rollback, Dummy Rollback Step 3 will fail and Step 4 will be executed
    ''' </summary>
    ''' <returns></returns>
    Public Shared Function DummyTaskBundleFailingWithRollbackFailingWithLoggedException() As ProgressingTaskBundle
        Dim Result As New ProgressingTaskBundle("Dummy Task Bundle with exception and rollback with failing rollback step but continueing last rollback steps")
        Dim Task1 As New ProgressingTaskItem(Result, "Dummy Task 1")
        Task1.FirstStepsWhichCanBeRolledBack.Add(New ProgressingTaskFailFastStep("Dummy Step 1.1", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1)))
        Task1.FirstStepsWhichCanBeRolledBack.Add(New ProgressingTaskFailFastStep("Dummy Step 1.2", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1)))
        Task1.FirstStepsWhichCanBeRolledBack.Add(New ProgressingTaskFailFastStep("Dummy Step 1.3", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1)))
        Task1.FirstStepsWhichCanBeRolledBack.Add(New ProgressingTaskFailFastStep("Dummy Step 1.4", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1)))
        Task1.FirstStepsWhichCanBeRolledBack.Add(New ProgressingTaskFailFastStep("Dummy Step 1.5", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1)))
        Task1.FirstStepsWhichCanBeRolledBack.Add(New ProgressingTaskFailFastStep("Dummy Step 1.6 FAILING", Sub() Throw New Exception("Failure-Test in Dummy"), New TimeSpan(0, 0, 1)))
        Task1.FirstStepsWhichCanBeRolledBack.Add(New ProgressingTaskFailFastStep("Dummy Step 1.7", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1)))
        Task1.SecondStepsWithoutRollbackOption.Add(New ProgressingTaskStep("Dummy Step 2.1", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1), ProgressingTaskStep.ProgressingTaskStepFailAction.LogExceptionAndContinue))
        Task1.SecondStepsWithoutRollbackOption.Add(New ProgressingTaskStep("Dummy Step 2.2", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1), ProgressingTaskStep.ProgressingTaskStepFailAction.LogExceptionAndContinue))
        Task1.SecondStepsWithoutRollbackOption.Add(New ProgressingTaskStep("Dummy Step 2.3", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1), ProgressingTaskStep.ProgressingTaskStepFailAction.LogExceptionAndContinue))
        Task1.SecondStepsWithoutRollbackOption.Add(New ProgressingTaskStep("Dummy Step 2.4", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1), ProgressingTaskStep.ProgressingTaskStepFailAction.LogExceptionAndContinue))
        Task1.RollbackSteps.Add(New ProgressingTaskStep("Dummy Rollback Step 1", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1), ProgressingTaskStep.ProgressingTaskStepFailAction.LogExceptionAndContinue))
        Task1.RollbackSteps.Add(New ProgressingTaskStep("Dummy Rollback Step 2", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1), ProgressingTaskStep.ProgressingTaskStepFailAction.LogExceptionAndContinue))
        Task1.RollbackSteps.Add(New ProgressingTaskStep("Dummy Rollback Step 3 FAILING", Sub() Throw New Exception("Failure-Test in Dummy"), New TimeSpan(0, 0, 1), ProgressingTaskStep.ProgressingTaskStepFailAction.LogExceptionAndContinue))
        Task1.RollbackSteps.Add(New ProgressingTaskStep("Dummy Rollback Step 4", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1), ProgressingTaskStep.ProgressingTaskStepFailAction.LogExceptionAndContinue))
        Dim Task2 As New ProgressingTaskItem(Result, "Dummy Task 2")
        Task2.SecondStepsWithoutRollbackOption.Add(New ProgressingTaskStep("Dummy Task 2 Step 1", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1), ProgressingTaskStep.ProgressingTaskStepFailAction.LogExceptionAndContinue))
        Task2.SecondStepsWithoutRollbackOption.Add(New ProgressingTaskStep("Dummy Task 2 Step 2", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1), ProgressingTaskStep.ProgressingTaskStepFailAction.LogExceptionAndContinue))
        Return Result
    End Function

    ''' <summary>
    ''' After rollback, Dummy Rollback Step 3 will fail and Step 4 won't be executed
    ''' </summary>
    ''' <returns></returns>
    Public Shared Function DummyTaskBundleFailingWithRollbackFailingWithThrownException() As ProgressingTaskBundle
        Dim Result As New ProgressingTaskBundle("Dummy Task Bundle with exception in 1st steps and failig again in rollback stopping rollback totally")
        Dim Task1 As New ProgressingTaskItem(Result, "Dummy Task 1")
        Task1.FirstStepsWhichCanBeRolledBack.Add(New ProgressingTaskFailFastStep("Dummy Step 1.1", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1)))
        Task1.FirstStepsWhichCanBeRolledBack.Add(New ProgressingTaskFailFastStep("Dummy Step 1.2", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1)))
        Task1.FirstStepsWhichCanBeRolledBack.Add(New ProgressingTaskFailFastStep("Dummy Step 1.3", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1)))
        Task1.FirstStepsWhichCanBeRolledBack.Add(New ProgressingTaskFailFastStep("Dummy Step 1.4", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1)))
        Task1.FirstStepsWhichCanBeRolledBack.Add(New ProgressingTaskFailFastStep("Dummy Step 1.5", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1)))
        Task1.FirstStepsWhichCanBeRolledBack.Add(New ProgressingTaskFailFastStep("Dummy Step 1.6 FAILING", Sub() Throw New Exception("Failure-Test in Dummy"), New TimeSpan(0, 0, 1)))
        Task1.FirstStepsWhichCanBeRolledBack.Add(New ProgressingTaskFailFastStep("Dummy Step 1.7", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1)))
        Task1.SecondStepsWithoutRollbackOption.Add(New ProgressingTaskStep("Dummy Step 2.1", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1), ProgressingTaskStep.ProgressingTaskStepFailAction.LogExceptionAndContinue))
        Task1.SecondStepsWithoutRollbackOption.Add(New ProgressingTaskStep("Dummy Step 2.2", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1), ProgressingTaskStep.ProgressingTaskStepFailAction.LogExceptionAndContinue))
        Task1.SecondStepsWithoutRollbackOption.Add(New ProgressingTaskStep("Dummy Step 2.3", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1), ProgressingTaskStep.ProgressingTaskStepFailAction.LogExceptionAndContinue))
        Task1.SecondStepsWithoutRollbackOption.Add(New ProgressingTaskStep("Dummy Step 2.4", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1), ProgressingTaskStep.ProgressingTaskStepFailAction.LogExceptionAndContinue))
        Task1.RollbackSteps.Add(New ProgressingTaskStep("Dummy Rollback Step 1", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1), ProgressingTaskStep.ProgressingTaskStepFailAction.LogExceptionAndContinue))
        Task1.RollbackSteps.Add(New ProgressingTaskStep("Dummy Rollback Step 2", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1), ProgressingTaskStep.ProgressingTaskStepFailAction.LogExceptionAndContinue))
        Task1.RollbackSteps.Add(New ProgressingTaskStep("Dummy Rollback Step 3 FAILING", Sub() Throw New Exception("Failure-Test in Dummy"), New TimeSpan(0, 0, 1), ProgressingTaskStep.ProgressingTaskStepFailAction.ThrowException))
        Task1.RollbackSteps.Add(New ProgressingTaskStep("Dummy Rollback Step 4", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1), ProgressingTaskStep.ProgressingTaskStepFailAction.ThrowException))
        Dim Task2 As New ProgressingTaskItem(Result, "Dummy Task 2")
        Task2.SecondStepsWithoutRollbackOption.Add(New ProgressingTaskStep("Dummy Task 2 Step 1", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1), ProgressingTaskStep.ProgressingTaskStepFailAction.LogExceptionAndContinue))
        Task2.SecondStepsWithoutRollbackOption.Add(New ProgressingTaskStep("Dummy Task 2 Step 2", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1), ProgressingTaskStep.ProgressingTaskStepFailAction.LogExceptionAndContinue))
        Return Result
    End Function

    ''' <summary>
    ''' Dummy Task Bundle without exceptions runs first steps and seconds steps only (no rollback)
    ''' </summary>
    ''' <returns></returns>
    Public Shared Function DummyTaskBundleSuccessful() As ProgressingTaskBundle
        Dim Result As New ProgressingTaskBundle("Dummy Task Bundle for successful run")
        Dim Task1 As New ProgressingTaskItem(Result, "Dummy Task 1")
        Task1.FirstStepsWhichCanBeRolledBack.Add(New ProgressingTaskFailFastStep("Dummy Step 1.1", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1)))
        Task1.FirstStepsWhichCanBeRolledBack.Add(New ProgressingTaskFailFastStep("Dummy Step 1.2", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1)))
        Task1.FirstStepsWhichCanBeRolledBack.Add(New ProgressingTaskFailFastStep("Dummy Step 1.3", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1)))
        Task1.FirstStepsWhichCanBeRolledBack.Add(New ProgressingTaskFailFastStep("Dummy Step 1.4", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1)))
        Task1.FirstStepsWhichCanBeRolledBack.Add(New ProgressingTaskFailFastStep("Dummy Step 1.5", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1)))
        Task1.FirstStepsWhichCanBeRolledBack.Add(New ProgressingTaskFailFastStep("Dummy Step 1.6", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1)))
        Task1.FirstStepsWhichCanBeRolledBack.Add(New ProgressingTaskFailFastStep("Dummy Step 1.7", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1)))
        Task1.SecondStepsWithoutRollbackOption.Add(New ProgressingTaskStep("Dummy Step 2.1", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1), ProgressingTaskStep.ProgressingTaskStepFailAction.LogExceptionAndContinue))
        Task1.SecondStepsWithoutRollbackOption.Add(New ProgressingTaskStep("Dummy Step 2.2", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1), ProgressingTaskStep.ProgressingTaskStepFailAction.LogExceptionAndContinue))
        Task1.SecondStepsWithoutRollbackOption.Add(New ProgressingTaskStep("Dummy Step 2.3", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1), ProgressingTaskStep.ProgressingTaskStepFailAction.LogExceptionAndContinue))
        Task1.SecondStepsWithoutRollbackOption.Add(New ProgressingTaskStep("Dummy Step 2.4", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1), ProgressingTaskStep.ProgressingTaskStepFailAction.LogExceptionAndContinue))
        Task1.RollbackSteps.Add(New ProgressingTaskStep("Dummy Rollback Step 1", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1), ProgressingTaskStep.ProgressingTaskStepFailAction.LogExceptionAndContinue))
        Task1.RollbackSteps.Add(New ProgressingTaskStep("Dummy Rollback Step 2", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1), ProgressingTaskStep.ProgressingTaskStepFailAction.LogExceptionAndContinue))
        Task1.RollbackSteps.Add(New ProgressingTaskStep("Dummy Rollback Step 3", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1), ProgressingTaskStep.ProgressingTaskStepFailAction.LogExceptionAndContinue))
        Task1.RollbackSteps.Add(New ProgressingTaskStep("Dummy Rollback Step 4", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1), ProgressingTaskStep.ProgressingTaskStepFailAction.LogExceptionAndContinue))
        Dim Task2 As New ProgressingTaskItem(Result, "Dummy Task 2")
        Task2.SecondStepsWithoutRollbackOption.Add(New ProgressingTaskStep("Dummy Task 2 Step 1", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1), ProgressingTaskStep.ProgressingTaskStepFailAction.LogExceptionAndContinue))
        Task2.SecondStepsWithoutRollbackOption.Add(New ProgressingTaskStep("Dummy Task 2 Step 2", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1), ProgressingTaskStep.ProgressingTaskStepFailAction.LogExceptionAndContinue))
        Return Result
    End Function

    ''' <summary>
    ''' Dummy Task Bundle without exceptions in first steps but failing in secondary steps (no rollback possible => critical breakdown)
    ''' </summary>
    ''' <returns></returns>
    Public Shared Function DummyTaskBundleFailingInCriticalStepsWithoutRollbackPossibility() As ProgressingTaskBundle
        Dim Result As New ProgressingTaskBundle("Dummy Task Bundle failing in critical steps without rollback possibility -> fails with critical state")
        Dim Task1 As New ProgressingTaskItem(Result, "Dummy Task 1")
        Task1.FirstStepsWhichCanBeRolledBack.Add(New ProgressingTaskFailFastStep("Dummy Step 1.1", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1)))
        Task1.FirstStepsWhichCanBeRolledBack.Add(New ProgressingTaskFailFastStep("Dummy Step 1.2", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1)))
        Task1.SecondStepsWithoutRollbackOption.Add(New ProgressingTaskStep("Dummy Step 2.1", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1), ProgressingTaskStep.ProgressingTaskStepFailAction.LogExceptionAndContinue))
        Task1.SecondStepsWithoutRollbackOption.Add(New ProgressingTaskStep("Dummy Step 2.2", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1), ProgressingTaskStep.ProgressingTaskStepFailAction.LogExceptionAndContinue))
        Task1.SecondStepsWithoutRollbackOption.Add(New ProgressingTaskStep("Dummy Step 2.3", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1), ProgressingTaskStep.ProgressingTaskStepFailAction.LogExceptionAndContinue))
        Task1.SecondStepsWithoutRollbackOption.Add(New ProgressingTaskStep("Dummy Step 2.4", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1), ProgressingTaskStep.ProgressingTaskStepFailAction.LogExceptionAndContinue))
        Task1.SecondStepsWithoutRollbackOption.Add(New ProgressingTaskStep("Dummy Step 2.5 FAILING", Sub() Throw New Exception("Critical Failure-Test 1 in Dummy"), New TimeSpan(0, 0, 1), ProgressingTaskStep.ProgressingTaskStepFailAction.LogExceptionAndContinue))
        Task1.SecondStepsWithoutRollbackOption.Add(New ProgressingTaskStep("Dummy Step 2.6 FAILING", Sub() Throw New Exception("Critical Failure-Test 2 in Dummy"), New TimeSpan(0, 0, 1), ProgressingTaskStep.ProgressingTaskStepFailAction.LogExceptionAndContinue))
        Task1.SecondStepsWithoutRollbackOption.Add(New ProgressingTaskStep("Dummy Step 2.7", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1), ProgressingTaskStep.ProgressingTaskStepFailAction.LogExceptionAndContinue))
        Task1.RollbackSteps.Add(New ProgressingTaskStep("Dummy Rollback Step 1", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1), ProgressingTaskStep.ProgressingTaskStepFailAction.LogExceptionAndContinue))
        Task1.RollbackSteps.Add(New ProgressingTaskStep("Dummy Rollback Step 2", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1), ProgressingTaskStep.ProgressingTaskStepFailAction.LogExceptionAndContinue))
        Task1.RollbackSteps.Add(New ProgressingTaskStep("Dummy Rollback Step 3", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1), ProgressingTaskStep.ProgressingTaskStepFailAction.LogExceptionAndContinue))
        Task1.RollbackSteps.Add(New ProgressingTaskStep("Dummy Rollback Step 4", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1), ProgressingTaskStep.ProgressingTaskStepFailAction.LogExceptionAndContinue))
        Dim Task2 As New ProgressingTaskItem(Result, "Dummy Task 2")
        Task2.SecondStepsWithoutRollbackOption.Add(New ProgressingTaskStep("Dummy Task 2 Step 1", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1), ProgressingTaskStep.ProgressingTaskStepFailAction.LogExceptionAndContinue))
        Task2.SecondStepsWithoutRollbackOption.Add(New ProgressingTaskStep("Dummy Task 2 Step 2", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1), ProgressingTaskStep.ProgressingTaskStepFailAction.LogExceptionAndContinue))
        Return Result
    End Function

End Class
