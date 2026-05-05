Namespace Exceptions

#Disable Warning CA2237 ' Mark ISerializable types with serializable
#Disable Warning CA1032 ' Implement standard exception constructors
    ''' <summary>
    ''' Represents an exception that occurred while running a task step.
    ''' </summary>
    Public Class StepException

#Enable Warning CA1032 ' Implement standard exception constructors
#Enable Warning CA2237 ' Mark ISerializable types with serializable
        Inherits System.Exception

        ''' <summary>
        ''' Initializes a new step exception.
        ''' </summary>
        ''' <param name="message">The exception message.</param>
        Public Sub New(message As String)
            MyBase.New(message)
        End Sub

        ''' <summary>
        ''' Initializes a new step exception with an inner exception.
        ''' </summary>
        ''' <param name="message">The exception message.</param>
        ''' <param name="innerException">The exception that caused this exception.</param>
        Public Sub New(message As String, innerException As Exception)
            MyBase.New(message, innerException)
        End Sub

    End Class

End Namespace
