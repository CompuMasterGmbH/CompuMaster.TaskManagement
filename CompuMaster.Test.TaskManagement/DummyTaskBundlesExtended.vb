Imports CompuMaster.TaskManagement

Public NotInheritable Class DummyTaskBundlesExtended

    ''' <summary>
    ''' Dummy Task Bundle with step action failing, but declaring appropriate fail action just-in-time
    ''' </summary>
    ''' <returns></returns>
    Public Shared Function DummyTaskBundleFailingStepActionWithFailAction(stepAction As ProgressingTaskStepBase.StepActionMethodWithFailAction) As ProgressingTaskBundle
        Dim Result As New ProgressingTaskBundle("Dummy Task Bundle with exception and rollback")
        Dim Task1 As ProgressingTaskItem = Result.CreateAndAddNewTask("Dummy Task 1")
        Task1.FirstStepsWhichCanBeRolledBack.Add(New ProgressingTaskFailFastStep("Dummy Step 1.1", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1)))
        Task1.FirstStepsWhichCanBeRolledBack.Add(New ProgressingTaskFailFastStep("Dummy Step 1.2", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1)))
        Task1.SecondStepsWithoutRollbackOption.Add(New ProgressingTaskStep("Dummy Step 2.1", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1), ProgressingTaskStep.ProgressingTaskStepFailAction.LogExceptionAndContinue))
        Task1.SecondStepsWithoutRollbackOption.Add(New ProgressingTaskStep("Dummy Step 2.2", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1), ProgressingTaskStep.ProgressingTaskStepFailAction.LogExceptionAndContinue))
        Task1.SecondStepsWithoutRollbackOption.Add(New ProgressingTaskStepWithDynamicAction("Dummy Step 2.3 FAILING+DYNFAILACTION", stepAction, New TimeSpan(0, 0, 1)))
        Task1.SecondStepsWithoutRollbackOption.Add(New ProgressingTaskStepWithDynamicAction("Dummy Step 2.4 FAILING+DYNFAILACTION", stepAction, New TimeSpan(0, 0, 1)))
        Task1.RollbackSteps.Add(New ProgressingTaskStep("Dummy Rollback Step 1", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1), ProgressingTaskStep.ProgressingTaskStepFailAction.LogExceptionAndContinue))
        Task1.RollbackSteps.Add(New ProgressingTaskStep("Dummy Rollback Step 2", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1), ProgressingTaskStep.ProgressingTaskStepFailAction.LogExceptionAndContinue))
        Dim Task2 As ProgressingTaskItem = Result.CreateAndAddNewTask("Dummy Task 2")
        Task2.SecondStepsWithoutRollbackOption.Add(New ProgressingTaskStep("Dummy Task 2 Step 1", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1), ProgressingTaskStep.ProgressingTaskStepFailAction.LogExceptionAndContinue))
        Task2.SecondStepsWithoutRollbackOption.Add(New ProgressingTaskStep("Dummy Task 2 Step 2", Sub() Threading.Thread.Sleep(1000), New TimeSpan(0, 0, 1), ProgressingTaskStep.ProgressingTaskStepFailAction.LogExceptionAndContinue))
        Return Result
    End Function

End Class
