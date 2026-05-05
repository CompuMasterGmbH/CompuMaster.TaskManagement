''' <summary>
''' Represents a task step that throws an exception when the step fails.
''' </summary>
Public Class ProgressingTaskFailFastStep
    Inherits ProgressingTaskStepBase

    ''' <summary>
    ''' Initializes a new fail-fast task step.
    ''' </summary>
    ''' <param name="stepTitle">The display title of the step.</param>
    ''' <param name="stepAction">The action to execute for the step.</param>
    ''' <param name="estimatedTimeToRun">The estimated duration of the step, or <see langword="Nothing"/> if the duration is unknown.</param>
    Public Sub New(stepTitle As String, stepAction As StepActionMethod, estimatedTimeToRun As TimeSpan?)
        MyBase.New(stepTitle, stepAction, estimatedTimeToRun, ProgressingTaskStepFailAction.ThrowException)
    End Sub

End Class
