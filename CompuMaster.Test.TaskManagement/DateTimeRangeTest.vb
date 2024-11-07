Imports NUnit.Framework
Imports CompuMaster.TaskManagement
Imports System.Text.RegularExpressions

Namespace CompuMaster.Test.TaskManagement

    <TestFixture()> Public Class DateTimeRangeTest

        ''' <summary>
        ''' Test the string representation of a DateTimeRange object
        ''' </summary>
        ''' <param name="cultureName"></param>
        ''' <remarks>Mac systems might present "1/1/2005 12:00:00 AM - *" with an NNBSP char between seconds and AM, so convert them to regular space char just for this test</remarks>
        <Test()> Public Sub ToStringTest(<Values("", "de-DE", "en-US", "en-UK", "fr-FR")> cultureName As String)

        End Sub

    End Class

End Namespace