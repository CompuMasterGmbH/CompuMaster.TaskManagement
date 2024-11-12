Public Class ProgressingTaskStepDynamicFailAction

    Public Sub New()
        Me.CompletedSuccessful = True
    End Sub

    Public Sub New(exception As Exception, failAction As ProgressingTaskStepBase.ProgressingTaskStepFailAction)
        Me.CompletedSuccessful = False
        Me.Exception = exception
        Me.FailAction = failAction
    End Sub

    Public Property CompletedSuccessful As Boolean

    Public Property Exception As Exception

    Public Property FailAction As ProgressingTaskStepBase.ProgressingTaskStepFailAction

End Class
