Namespace Exceptions

#Disable Warning CA2237 ' Mark ISerializable types with serializable
#Disable Warning CA1032 ' Implement standard exception constructors
    ''' <summary>
    ''' Represents an exception that reports a user-requested abort.
    ''' </summary>
    Public Class UserAbortedMessageException

#Enable Warning CA1032 ' Implement standard exception constructors
#Enable Warning CA2237 ' Mark ISerializable types with serializable
        Inherits System.Exception

        ''' <summary>
        ''' Initializes a new user-aborted exception.
        ''' </summary>
        ''' <param name="message">The exception message.</param>
        Public Sub New(message As String)
            MyBase.New(message)
        End Sub

    End Class

End Namespace
