''' <summary>
''' Represents a task step whose action returns the fail action dynamically.
''' </summary>
Public Class ProgressingTaskStepWithDynamicAction
    Inherits ProgressingTaskStep

    ''' <summary>
    ''' Initializes a new task step with a dynamic fail action.
    ''' </summary>
    ''' <param name="stepTitle">The display title of the step.</param>
    ''' <param name="stepAction">The action to execute for the step and return the step result.</param>
    ''' <param name="estimatedTimeToRun">The estimated duration of the step, or <see langword="Nothing"/> if the duration is unknown.</param>
    Public Sub New(stepTitle As String, stepAction As StepActionMethodWithFailAction, estimatedTimeToRun As TimeSpan?)
        MyBase.New(stepTitle, stepAction, estimatedTimeToRun)
    End Sub

End Class
