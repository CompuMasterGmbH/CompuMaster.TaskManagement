''' <summary>
''' A task step which will fail fast (throw exception) if the step fails
''' </summary>
Public Class ProgressingTaskFailFastStep
    Inherits ProgressingTaskStepBase

    Public Sub New(stepTitle As String, stepAction As StepActionMethod, estimatedTimeToRun As TimeSpan?)
        MyBase.New(stepTitle, stepAction, estimatedTimeToRun, ProgressingTaskStepFailAction.ThrowException)
    End Sub

End Class
