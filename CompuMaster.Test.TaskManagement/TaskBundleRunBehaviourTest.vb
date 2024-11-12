Imports NUnit.Framework
Imports CompuMaster.TaskManagement

Namespace CompuMaster.Test.TaskManagement

    <TestFixture()>
    <NUnit.Framework.Parallelizable(ParallelScope.All)>
    Public Class TaskBundleRunBehaviourTest

        <Test()> Public Sub StaticFlow_Success()
            Dim TaskBundle = DummyTaskBundles.DummyTaskBundleSuccessful()
            TaskBundle.RunAllTasks()
            Assert.That(TaskBundle.Status, NUnit.Framework.Is.EqualTo(ProgressingTaskBundle.ProgressingTaskBundleStatus.CompletedSuccessfully))
            Assert.That(TaskBundle.LoggedExceptions.Count, NUnit.Framework.Is.EqualTo(0))
        End Sub

        <Test()> Public Sub StaticFlow_FailedButRolledBackSuccessfully()
            Dim TaskBundle = DummyTaskBundles.DummyTaskBundleFailingWithRollback()
            Assert.Catch(Of AggregateException)(Sub() TaskBundle.RunAllTasks())
            Assert.That(TaskBundle.Status, NUnit.Framework.Is.EqualTo(ProgressingTaskBundle.ProgressingTaskBundleStatus.FailedNonCritically))
            Assert.That(TaskBundle.LoggedExceptions.Count, NUnit.Framework.Is.EqualTo(1))
            Assert.That(CType(TaskBundle.LoggedExceptions(0), AggregateException).InnerExceptions.Count, NUnit.Framework.Is.EqualTo(1))
        End Sub

        <Test()> Public Sub StaticFlow_FailedButRolledBackFailingWithLoggedException()
            Dim TaskBundle = DummyTaskBundles.DummyTaskBundleFailingWithRollbackFailingWithLoggedException()
            Assert.Catch(Of AggregateException)(Sub() TaskBundle.RunAllTasks())
            Assert.That(TaskBundle.Status, NUnit.Framework.Is.EqualTo(ProgressingTaskBundle.ProgressingTaskBundleStatus.FailedInCriticalState))
            Assert.That(TaskBundle.LoggedExceptions.Count, NUnit.Framework.Is.EqualTo(2))
            Assert.That(CType(TaskBundle.LoggedExceptions(0), AggregateException).InnerExceptions.Count, NUnit.Framework.Is.EqualTo(1))
        End Sub

        <Test()> Public Sub StaticFlow_FailedButRolledBackFailingWithThrownException()
            Dim TaskBundle = DummyTaskBundles.DummyTaskBundleFailingWithRollbackFailingWithThrownException()
            Assert.Catch(Of AggregateException)(Sub() TaskBundle.RunAllTasks())
            Assert.That(TaskBundle.Status, NUnit.Framework.Is.EqualTo(ProgressingTaskBundle.ProgressingTaskBundleStatus.FailedInCriticalState))
            Assert.That(TaskBundle.LoggedExceptions.Count, NUnit.Framework.Is.EqualTo(2))
            Assert.That(CType(TaskBundle.LoggedExceptions(0), AggregateException).InnerExceptions.Count, NUnit.Framework.Is.EqualTo(1))
        End Sub

        <Test()> Public Sub StaticFlow_FailedInCriticalStepsWithoutRollbackPossibility()
            Dim TaskBundle = DummyTaskBundles.DummyTaskBundleFailingInCriticalStepsWithoutRollbackPossibility()
            Assert.Catch(Of AggregateException)(Sub() TaskBundle.RunAllTasks())
            Assert.That(TaskBundle.Status, NUnit.Framework.Is.EqualTo(ProgressingTaskBundle.ProgressingTaskBundleStatus.FailedInCriticalState))
            Assert.That(TaskBundle.LoggedExceptions.Count, NUnit.Framework.Is.EqualTo(1))
            Assert.That(TaskBundle.LoggedExceptions(0).GetType, NUnit.Framework.Is.EqualTo(GetType(AggregateException)))
            Assert.That(CType(TaskBundle.LoggedExceptions(0), AggregateException).InnerExceptions.Count, NUnit.Framework.Is.EqualTo(2))
        End Sub

        <Test()> Public Sub DynamicFailActionFlow_Success()
            Dim TaskBundle As ProgressingTaskBundle
            TaskBundle = DummyTaskBundlesExtended.DummyTaskBundleFailingStepActionWithFailAction(Function()
                                                                                                     'Success => no exception, but dynamic result
                                                                                                     Return New ProgressingTaskStepDynamicFailAction()
                                                                                                 End Function)
            TaskBundle.RunAllTasks()
            Assert.That(TaskBundle.Status, NUnit.Framework.Is.EqualTo(ProgressingTaskBundle.ProgressingTaskBundleStatus.CompletedSuccessfully))
            Assert.That(TaskBundle.LoggedExceptions.Count, NUnit.Framework.Is.EqualTo(0))
        End Sub

        <Test()> Public Sub DynamicFailActionFlow_SuccessButWithoutResult()
            Dim TaskBundle As ProgressingTaskBundle
            TaskBundle = DummyTaskBundlesExtended.DummyTaskBundleFailingStepActionWithFailAction(Function()
                                                                                                     'Success, but no ProgressingTaskStepDynamicFailAction
                                                                                                     Return Nothing
                                                                                                 End Function)
            Assert.Catch(Of AggregateException)(Sub() TaskBundle.RunAllTasks())
            Assert.That(TaskBundle.Status, NUnit.Framework.Is.EqualTo(ProgressingTaskBundle.ProgressingTaskBundleStatus.FailedInCriticalState))
            Assert.That(TaskBundle.LoggedExceptions.Count, NUnit.Framework.Is.EqualTo(1))
            Assert.That(TaskBundle.LoggedExceptions(0).GetType, NUnit.Framework.Is.EqualTo(GetType(NotImplementedException)))
        End Sub

        <Test()> Public Sub DynamicFailActionFlow_ExceptionButWithoutCatchingAndCorrectResult()
            Dim TaskBundle As ProgressingTaskBundle
            TaskBundle = DummyTaskBundlesExtended.DummyTaskBundleFailingStepActionWithFailAction(Function()
                                                                                                     'Uncatched exception failure
                                                                                                     Throw New Exception("Failure-Test in Dummy")
                                                                                                 End Function)
            Assert.Catch(Of AggregateException)(Sub() TaskBundle.RunAllTasks()) 'AggregateException is thrown first, before NotImplementedException would follow
            Assert.That(TaskBundle.Status, NUnit.Framework.Is.EqualTo(ProgressingTaskBundle.ProgressingTaskBundleStatus.FailedInCriticalState))
            Assert.That(TaskBundle.LoggedExceptions.Count, NUnit.Framework.Is.EqualTo(1))
            Assert.That(TaskBundle.LoggedExceptions(0).GetType, NUnit.Framework.Is.EqualTo(GetType(NotImplementedException)))
        End Sub

        <Test()> Public Sub DynamicFailActionFlow_ExceptionWithLoggingAndContinue()
            Dim TaskBundle As ProgressingTaskBundle
            TaskBundle = DummyTaskBundlesExtended.DummyTaskBundleFailingStepActionWithFailAction(Function()
                                                                                                     'Catched exception failure - LogAndContinue
                                                                                                     Return New ProgressingTaskStepDynamicFailAction(New CustomException("Failure-Test in Dummy"), ProgressingTaskStepBase.ProgressingTaskStepFailAction.LogExceptionAndContinue)
                                                                                                 End Function)
            Assert.Catch(Of AggregateException)(Sub() TaskBundle.RunAllTasks())
            Assert.That(TaskBundle.Status, NUnit.Framework.Is.EqualTo(ProgressingTaskBundle.ProgressingTaskBundleStatus.FailedInCriticalState))
            Assert.That(TaskBundle.LoggedExceptions.Count, NUnit.Framework.Is.EqualTo(1))
            Assert.That(TaskBundle.LoggedExceptions(0).GetType, NUnit.Framework.Is.EqualTo(GetType(AggregateException)))
            Assert.That(CType(TaskBundle.LoggedExceptions(0), AggregateException).InnerExceptions.Count, NUnit.Framework.Is.EqualTo(2))
            Assert.That(CType(TaskBundle.LoggedExceptions(0), AggregateException).InnerExceptions(0).GetType, NUnit.Framework.Is.EqualTo(GetType(CompuMaster.TaskManagement.Exceptions.StepException)))
            System.Console.WriteLine("Inner exception=" & CType(CType(TaskBundle.LoggedExceptions(0), AggregateException).InnerExceptions(0), CompuMaster.TaskManagement.Exceptions.StepException).InnerException.ToString)
            Assert.That(CType(CType(TaskBundle.LoggedExceptions(0), AggregateException).InnerExceptions(0), CompuMaster.TaskManagement.Exceptions.StepException).InnerException.GetType, NUnit.Framework.Is.EqualTo(GetType(CustomException)))
        End Sub

        <Test()> Public Sub DynamicFailActionFlow_ExceptionWithReThrowing()
            Dim TaskBundle As ProgressingTaskBundle
            TaskBundle = DummyTaskBundlesExtended.DummyTaskBundleFailingStepActionWithFailAction(Function()
                                                                                                     'Catched exception failure - Throw
                                                                                                     Return New ProgressingTaskStepDynamicFailAction(New CustomException("Failure-Test in Dummy"), ProgressingTaskStepBase.ProgressingTaskStepFailAction.ThrowException)
                                                                                                 End Function)
            Assert.Catch(Of AggregateException)(Sub() TaskBundle.RunAllTasks())
            Assert.That(TaskBundle.Status, NUnit.Framework.Is.EqualTo(ProgressingTaskBundle.ProgressingTaskBundleStatus.FailedInCriticalState))
            Assert.That(TaskBundle.LoggedExceptions.Count, NUnit.Framework.Is.EqualTo(1))
            Assert.That(TaskBundle.LoggedExceptions(0).GetType, NUnit.Framework.Is.EqualTo(GetType(AggregateException)))
            Assert.That(CType(TaskBundle.LoggedExceptions(0), AggregateException).InnerExceptions.Count, NUnit.Framework.Is.EqualTo(1))
            Assert.That(CType(TaskBundle.LoggedExceptions(0), AggregateException).InnerExceptions(0).GetType, NUnit.Framework.Is.EqualTo(GetType(CompuMaster.TaskManagement.Exceptions.StepException)))
            System.Console.WriteLine("Inner exception=" & CType(CType(TaskBundle.LoggedExceptions(0), AggregateException).InnerExceptions(0), CompuMaster.TaskManagement.Exceptions.StepException).InnerException.ToString)
            Assert.That(CType(CType(TaskBundle.LoggedExceptions(0), AggregateException).InnerExceptions(0), CompuMaster.TaskManagement.Exceptions.StepException).InnerException.GetType, NUnit.Framework.Is.EqualTo(GetType(CustomException)))
        End Sub

        Private Class CustomException
            Inherits Exception

            Public Sub New(message As String)
                MyBase.New(message)
            End Sub
        End Class

    End Class

End Namespace