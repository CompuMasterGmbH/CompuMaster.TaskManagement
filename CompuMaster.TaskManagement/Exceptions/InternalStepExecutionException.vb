Namespace Exceptions

#Disable Warning CA2237 ' Mark ISerializable types with serializable
#Disable Warning CA1032 ' Implement standard exception constructors
    Friend Class InternalStepExecutionException

#Enable Warning CA1032 ' Implement standard exception constructors
#Enable Warning CA2237 ' Mark ISerializable types with serializable
        Inherits System.Exception

        Public Sub New(innerException As Exception)
            MyBase.New(Nothing, innerException)
        End Sub

    End Class

End Namespace