''' <summary>
''' Represents the result of a step action method.
''' </summary>
Public Class TaskStepActionResult

    ''' <summary>
    ''' Initializes a successful step action result.
    ''' </summary>
    Public Sub New()
        Me.CompletedSuccessful = True
    End Sub

    ''' <summary>
    ''' Initializes a failed step action result.
    ''' </summary>
    ''' <param name="exception">The exception that caused the step to fail.</param>
    ''' <param name="failAction">The action to apply when the step fails.</param>
    Public Sub New(exception As Exception, failAction As ProgressingTaskStepBase.ProgressingTaskStepFailAction)
        Me.CompletedSuccessful = False
        Me.Exception = exception
        Me.FailAction = failAction
    End Sub

    ''' <summary>
    ''' Gets or sets a value indicating whether the step completed successfully.
    ''' </summary>
    Public Property CompletedSuccessful As Boolean

    ''' <summary>
    ''' Gets or sets the exception that caused the step to fail.
    ''' </summary>
    Public Property Exception As Exception

    ''' <summary>
    ''' Gets or sets the action to apply when the step fails.
    ''' </summary>
    Public Property FailAction As ProgressingTaskStepBase.ProgressingTaskStepFailAction

End Class
