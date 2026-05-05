''' <summary>
''' Represents a task step with a configurable fail action.
''' </summary>
Public Class ProgressingTaskStep
    Inherits ProgressingTaskStepBase

    ''' <summary>
    ''' Initializes a new task step.
    ''' </summary>
    ''' <param name="stepTitle">The display title of the step.</param>
    ''' <param name="stepAction">The action to execute for the step.</param>
    ''' <param name="estimatedTimeToRun">The estimated duration of the step, or <see langword="Nothing"/> if the duration is unknown.</param>
    ''' <param name="failAction">The action to apply when the step fails.</param>
    Public Sub New(stepTitle As String, stepAction As StepActionMethod, estimatedTimeToRun As TimeSpan?, failAction As ProgressingTaskStepFailAction)
        MyBase.New(stepTitle, stepAction, estimatedTimeToRun, failAction)
    End Sub

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
