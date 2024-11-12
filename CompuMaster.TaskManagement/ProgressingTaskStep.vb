''' <summary>
''' A task step with configurable fail action
''' </summary>
Public Class ProgressingTaskStep
    Inherits ProgressingTaskStepBase

    Public Sub New(stepTitle As String, stepAction As StepActionMethod, estimatedTimeToRun As TimeSpan?, failAction As ProgressingTaskStepFailAction)
        MyBase.New(stepTitle, stepAction, estimatedTimeToRun, failAction)
    End Sub

    Public Sub New(stepTitle As String, stepAction As StepActionMethodWithFailAction, estimatedTimeToRun As TimeSpan?)
        MyBase.New(stepTitle, stepAction, estimatedTimeToRun)
    End Sub

End Class
