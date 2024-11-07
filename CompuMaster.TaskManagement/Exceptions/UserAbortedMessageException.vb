﻿Namespace Exceptions

#Disable Warning CA2237 ' Mark ISerializable types with serializable
#Disable Warning CA1032 ' Implement standard exception constructors
    Public Class UserAbortedMessageException

#Enable Warning CA1032 ' Implement standard exception constructors
#Enable Warning CA2237 ' Mark ISerializable types with serializable
        Inherits System.Exception

        Public Sub New(message As String)
            MyBase.New(message)
        End Sub

    End Class

End Namespace