''' <summary>
''' A task step with configurable fail action
''' </summary>
Public Class ProgressingTaskStep
    Inherits ProgressingTaskStepBase

    Public Sub New(stepTitle As String, stepAction As StepActionMethod, estimatedTimeToRun As TimeSpan?, failAction As ProgressingTaskStepFailAction)
        MyBase.New(stepTitle, stepAction, estimatedTimeToRun, failAction)
    End Sub

End Class
