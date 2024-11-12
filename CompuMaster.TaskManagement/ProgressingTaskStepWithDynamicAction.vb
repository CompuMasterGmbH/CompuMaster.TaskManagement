''' <summary>
''' A task step which will fail fast (throw exception) if the step fails
''' </summary>
Public Class ProgressingTaskStepWithDynamicAction
    Inherits ProgressingTaskStep

    Public Sub New(stepTitle As String, stepAction As StepActionMethodWithFailAction, estimatedTimeToRun As TimeSpan?)
        MyBase.New(stepTitle, stepAction, estimatedTimeToRun)
    End Sub

End Class
