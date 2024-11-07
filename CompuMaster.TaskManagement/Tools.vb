Option Explicit On
Option Strict On

Imports CompuMaster.VisualBasicCompatibility

Friend NotInheritable Class Tools

    Public Shared Function CallingMethodName() As String
        Return New System.Diagnostics.StackFrame(1).GetMethod.Name
    End Function

    Public Shared Function CallingMethodName(skipFrames As Integer) As String
        Return New System.Diagnostics.StackFrame(1 + skipFrames).GetMethod.Name
    End Function

    Public Shared Function FindKeyValuePair(Of T As Structure, V)(list As List(Of KeyValuePair(Of T, V)), key As T) As KeyValuePair(Of T, V)
        For MyCounter As Integer = 0 To list.Count - 1
            Dim Item = list(MyCounter)
            If Item.Key.Equals(key) Then
                Return Item
            End If
        Next
        Return Nothing
    End Function

    ''' <summary>
    ''' Create a value from flag-enum matching all enum elements 
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <returns></returns>
    Public Shared Function FlagsValueOfAll(Of T As Structure)() As T
        If Not GetType(T).IsEnum Then Throw New NotSupportedException()
        Return CType(CType([Enum].GetValues(GetType(T)).OfType(Of T)().Sum(Function(v) CType(CType(v, Object), Integer)), Object), T)
    End Function

    ''' <summary>
    ''' Add/remove indentation (required line separator: CrLf)
    ''' </summary>
    ''' <param name="text"></param>
    ''' <param name="spaceCountForIndentation"></param>
    ''' <returns></returns>
    Public Shared Function IndentStringWithCrLf(text As String, spaceCountForIndentation As Integer) As String
        If text = Nothing OrElse spaceCountForIndentation = 0 Then
            Return text
        ElseIf spaceCountForIndentation > 0 Then 'Add indentation
            Return CompuMaster.VisualBasicCompatibility.Strings.Space(spaceCountForIndentation) & text.Replace(ControlChars.CrLf, ControlChars.CrLf & CompuMaster.VisualBasicCompatibility.Strings.Space(spaceCountForIndentation))
        Else 'Remove indentation
            If text.StartsWith(CompuMaster.VisualBasicCompatibility.Strings.Space(spaceCountForIndentation)) Then
                'remove indentation at begin of text
                Return text.Substring(spaceCountForIndentation).Replace(ControlChars.CrLf & CompuMaster.VisualBasicCompatibility.Strings.Space(spaceCountForIndentation), ControlChars.CrLf)
            Else
                'begin of text is not indented
                Return text.Replace(ControlChars.CrLf & CompuMaster.VisualBasicCompatibility.Strings.Space(spaceCountForIndentation), ControlChars.CrLf)
            End If
        End If
    End Function

    ''' <summary>
    ''' Add/remove indentation
    ''' </summary>
    ''' <param name="text"></param>
    ''' <param name="spaceCountForIndentation"></param>
    ''' <param name="lineBreak">The line break type which has been used in text</param>
    ''' <returns></returns>
    Public Shared Function IndentString(text As String, spaceCountForIndentation As Integer, lineBreak As String) As String
        If text = Nothing OrElse spaceCountForIndentation = 0 Then
            Return text
        ElseIf spaceCountForIndentation > 0 Then 'Add indentation
            Return CompuMaster.VisualBasicCompatibility.Strings.Space(spaceCountForIndentation) & text.Replace(lineBreak, lineBreak & CompuMaster.VisualBasicCompatibility.Strings.Space(spaceCountForIndentation))
        Else 'Remove indentation
            If text.StartsWith(CompuMaster.VisualBasicCompatibility.Strings.Space(spaceCountForIndentation)) Then
                'remove indentation at begin of text
                Return text.Substring(spaceCountForIndentation).Replace(lineBreak & CompuMaster.VisualBasicCompatibility.Strings.Space(spaceCountForIndentation), lineBreak)
            Else
                'begin of text is not indented
                Return text.Replace(lineBreak & CompuMaster.VisualBasicCompatibility.Strings.Space(spaceCountForIndentation), lineBreak)
            End If
        End If
    End Function

    ''' <summary>
    ''' Add/remove indentation
    ''' </summary>
    ''' <param name="text"></param>
    ''' <param name="spaceCountForIndentation"></param>
    ''' <returns></returns>
    Public Shared Function IndentStringStartingWith2ndLine(text As String, spaceCountForIndentation As Integer, lineBreak As String) As String
        If text = Nothing OrElse spaceCountForIndentation = 0 Then
            Return text
        ElseIf spaceCountForIndentation > 0 Then 'Add indentation
            Return text.Replace(lineBreak, lineBreak & CompuMaster.VisualBasicCompatibility.Strings.Space(spaceCountForIndentation))
        Else 'Remove indentation
            Return text.Replace(lineBreak & CompuMaster.VisualBasicCompatibility.Strings.Space(spaceCountForIndentation), lineBreak)
        End If
    End Function

    ''' <summary>
    ''' Add a prefix to each text line (required line separator: CrLf)
    ''' </summary>
    ''' <param name="text"></param>
    ''' <param name="prefixToAdd"></param>
    ''' <returns></returns>
    Public Shared Function PrefixString(text As String, prefixToAdd As String) As String
        If text = Nothing OrElse prefixToAdd = Nothing Then
            Return text
        Else
            Return prefixToAdd & text.Replace(ControlChars.CrLf, ControlChars.CrLf & prefixToAdd)
        End If
    End Function

    ''' <summary>
    ''' Nicely combine an absolute path with a relative path which might contain leading '..\' that should cause to cut directory names of absolutePath in result
    ''' </summary>
    ''' <param name="absolutePath"></param>
    ''' <param name="relativePath"></param>
    ''' <returns>'Dir\SubDir' and '..\SubDir2' will return 'Dir\SubDir2' (instead of 'Dir\SubDir\..\SubDir2' by System.IO.Path.Combine)</returns>
    Public Shared Function PathCombine(absolutePath As String, relativePath As String) As String
        absolutePath = ConvertPathWithDirectorySeparatorsForCurrentPlatform(absolutePath)
        relativePath = ConvertPathWithDirectorySeparatorsForCurrentPlatform(relativePath)
        If relativePath.StartsWith("." & System.IO.Path.DirectorySeparatorChar) Then
            Return PathCombine(absolutePath, relativePath.Substring(2))
        ElseIf relativePath.StartsWith(".." & System.IO.Path.DirectorySeparatorChar) Then
            Return PathCombine(System.IO.Path.GetDirectoryName(absolutePath), relativePath.Substring(3))
        Else
            Return System.IO.Path.Combine(absolutePath, relativePath)
        End If
    End Function

    ''' <summary>
    ''' Convert a path with directory separator ("/", "\") to a path compatible for the current platform
    ''' </summary>
    ''' <param name="path"></param>
    ''' <returns></returns>
    Public Shared Function ConvertPathWithDirectorySeparatorsForCurrentPlatform(path As String) As String
        Dim Result As String = path
        Result = Result.Replace(System.IO.Path.AltDirectorySeparatorChar, System.IO.Path.DirectorySeparatorChar) 'convert to platform-specific dir separator
        Result = Result.Replace("\", System.IO.Path.DirectorySeparatorChar) 'convert to platform-specific dir separator of "\" when AltDirectorySeparatorChar contains "/" on (some) mono frameworks
        Return Result
    End Function

    ''' <summary>
    ''' Für das FileSystem kompatibler String zur Verwendung als Teil von Dateinamen (Deutsche Sprachversion)
    ''' </summary>
    ''' <param name="partialFileName"></param>
    ''' <returns></returns>
    Public Shared Function FileSystemCompatibleName(partialFileName As String) As String
        Dim Result As String = partialFileName
        Result = Result.Replace("&", "_und_").Replace(" | ", "_").Replace("|", "_").Replace(" ", "_").Replace(":", "_").Replace("\", "_").Replace("/", "_")
        For Each ForbiddenChar As Char In System.IO.Path.GetInvalidFileNameChars
            Result = Result.Replace(ForbiddenChar, "_")
        Next
        For Each ForbiddenChar As Char In System.IO.Path.GetInvalidPathChars
            Result = Result.Replace(ForbiddenChar, "_")
        Next
        Return Result
    End Function

    ''' <summary>
    ''' Create a relative path representation if possible
    ''' </summary>
    ''' <param name="absolutePath"></param>
    ''' <param name="basePathForRelativePath"></param>
    ''' <returns></returns>
    Public Shared Function MakePathRelativeIfPossible(absolutePath As String, basePathForRelativePath As String) As String
        absolutePath = ConvertPathWithDirectorySeparatorsForCurrentPlatform(absolutePath)
        basePathForRelativePath = ConvertPathWithDirectorySeparatorsForCurrentPlatform(basePathForRelativePath)
        Return MakePathRelativeIfPossible(absolutePath, basePathForRelativePath, 0)
    End Function

    Private Shared Function MakePathRelativeIfPossible(absolutePath As String, basePathForRelativePath As String, parentLevel As Integer) As String
        If absolutePath = Nothing Then
            Return Nothing
        ElseIf basePathForRelativePath = Nothing Then
            Return absolutePath
        ElseIf basePathForRelativePath.EndsWith(System.IO.Path.DirectorySeparatorChar) AndAlso absolutePath.StartsWith(basePathForRelativePath) Then
            Dim ParentPathChain As String = ""
            For MyCounter As Integer = 1 To parentLevel
                ParentPathChain &= ".." & System.IO.Path.DirectorySeparatorChar
            Next
            Return ParentPathChain & absolutePath.Replace(basePathForRelativePath, "")
        ElseIf absolutePath.StartsWith(basePathForRelativePath & System.IO.Path.DirectorySeparatorChar) Then
            Dim ParentPathChain As String = ""
            For MyCounter As Integer = 1 To parentLevel
                ParentPathChain &= ".." & System.IO.Path.DirectorySeparatorChar
            Next
            Return ParentPathChain & absolutePath.Replace(basePathForRelativePath & System.IO.Path.DirectorySeparatorChar, "")
        Else
            'Sample: 
            'absolutePath:                                 T:\OwnCloud_CM\Bierdeckel-Report-Service_CM-only\Transfer StructureWorker (PROD+TEST)\Pörsch GmbH
            'basePathForRelativePath:                      T:\OwnCloud_CM\Bierdeckel-Report-Service_CM_x_MH\Kunden
            'Required relative path starting from Workdir: ..\Bierdeckel-Report-Service_CM-only\Transfer StructureWorker (PROD+TEST)\Pörsch GmbH
            If System.IO.Path.IsPathRooted(absolutePath) AndAlso System.IO.Path.IsPathRooted(basePathForRelativePath) Then
                'check if a parent of basePathForRelativePath might match
                Dim BasePathParent As String = System.IO.Path.GetDirectoryName(basePathForRelativePath)
                Return MakePathRelativeIfPossible(absolutePath, BasePathParent, parentLevel + 1)
            Else
                Return absolutePath
            End If
        End If
    End Function

    ''' <summary>
    ''' Convert a path with parent directory references ".." to a proper representation
    ''' </summary>
    ''' <param name="absolutePathContainingParentPathReferences"></param>
    ''' <returns></returns>
    ''' <example>A path "c:\windows\system32\..\temp" will be converted to "c:\windows\temp"</example>
    Public Shared Function ExecuteParentPathReferencesIfPossible(absolutePathContainingParentPathReferences As String) As String
        Dim DirInfo As New System.IO.DirectoryInfo(absolutePathContainingParentPathReferences)
        Return DirInfo.FullName
        'absolutePathContainingParentPathReferences = ConvertPathWithDirectorySeparatorsForCurrentPlatform(absolutePathContainingParentPathReferences)
        'Return ExecuteParentPathReferencesIfPossible(absolutePathContainingParentPathReferences, 0)
    End Function

    ''' <summary>
    ''' Convert a path with parent directory references ".." to a proper representation
    ''' </summary>
    ''' <param name="absolutePathContainingParentPathReferences"></param>
    ''' <returns></returns>
    ''' <example>A path "c:\windows\system32\..\temp" will be converted to "c:\windows\temp"</example>
    Private Shared Function ExecuteParentPathReferencesIfPossible(absolutePathContainingParentPathReferences As String, parentLevel As Integer) As String
        If absolutePathContainingParentPathReferences = Nothing Then
            Return Nothing
        Else
            'Sample: 
            'absolutePathContainingParentPathReferences:   T:\OwnCloud_CM\Bierdeckel-Report-Service_CM-only\Customers\..\Transfer StructureWorker (PROD+TEST)\Pörsch GmbH
            'Required path:                                T:\OwnCloud_CM\Bierdeckel-Report-Service_CM-only\Bierdeckel-Report-Service_CM-only\Transfer StructureWorker (PROD+TEST)\Pörsch GmbH
            If System.IO.Path.IsPathRooted(absolutePathContainingParentPathReferences) AndAlso System.IO.Path.IsPathRooted(absolutePathContainingParentPathReferences) Then
                'check if a parent of basePathForRelativePath might match
                Dim BasePathParent As String = System.IO.Path.GetDirectoryName(absolutePathContainingParentPathReferences)
                Return MakePathRelativeIfPossible(absolutePathContainingParentPathReferences, BasePathParent, parentLevel + 1)
            Else
                Return absolutePathContainingParentPathReferences
            End If
        End If
    End Function


    ''' <summary>
    ''' Check if a text contains all required search values
    ''' </summary>
    ''' <param name="text"></param>
    ''' <param name="searchValues"></param>
    ''' <returns></returns>
    Public Shared Function StringContainsAllMustValues(text As String, ParamArray searchValues As String()) As Boolean
        If text = Nothing Then Throw New ArgumentNullException(NameOf(text))
        If searchValues.Length = 0 Then Throw New ArgumentNullException(NameOf(searchValues))
        For MyCounter As Integer = 0 To searchValues.Length - 1
            If text.Contains(searchValues(MyCounter)) = False Then Return False
        Next
        Return True
    End Function

    ''' <summary>
    ''' Check if a text contains at least 1 of required search values
    ''' </summary>
    ''' <param name="text"></param>
    ''' <param name="searchValues"></param>
    ''' <returns></returns>
    Public Shared Function StringContainsAtLeastOneOfAllValues(text As String, ParamArray searchValues As String()) As Boolean
        If text = Nothing Then Throw New ArgumentNullException(NameOf(text))
        If searchValues.Length = 0 Then Throw New ArgumentNullException(NameOf(searchValues))
        For MyCounter As Integer = 0 To searchValues.Length - 1
            If text.Contains(searchValues(MyCounter)) = True Then Return True
        Next
        Return False
    End Function

    Public Shared Function NotNaNOrAlternativeValue(firstChoice As Double, alternativeChoice As Double) As Double
        If Not Double.IsNaN(firstChoice) Then
            Return firstChoice
        Else
            Return alternativeChoice
        End If
    End Function

    Public Shared Function NotNullOrEmptyStringValue(value As String) As String
        If value <> Nothing Then
            Return value
        Else
            Return String.Empty
        End If
    End Function

    Public Shared Function NotEmptyOrAlternativeValue(firstChoice As String, alternativeChoice As String) As String
        If firstChoice <> Nothing Then
            Return firstChoice
        Else
            Return alternativeChoice
        End If
    End Function

    Public Shared Function NotEmptyOrAlternativeValue(firstChoice As DateTime, alternativeChoice As DateTime) As DateTime
        If firstChoice <> Nothing Then
            Return firstChoice
        Else
            Return alternativeChoice
        End If
    End Function

    Public Shared Function NotZeroOrAlternativeValueToString(value As Integer, alternativeChoice As String) As String
        If value = Nothing Then
            Return alternativeChoice
        Else
            Return value.ToString
        End If
    End Function

    Public Shared Function IIf(expression As Boolean, firstChoice As String, alternativeChoice As String) As String
        If expression Then
            Return firstChoice
        Else
            Return alternativeChoice
        End If
    End Function

    Public Shared Function IIf(Of T)(expression As Boolean, firstChoice As T, alternativeChoice As T) As T
        If expression Then
            Return firstChoice
        Else
            Return alternativeChoice
        End If
    End Function

    ''' <summary>
    ''' Convert a nullable type to its string representation
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="value"></param>
    ''' <returns></returns>
    Public Shared Function ConvertToString(Of T As Structure)(ByVal value As Nullable(Of T)) As String
        If value.HasValue() Then
            Return value.ToString
        Else
            Return Nothing
        End If
    End Function

    ''' <summary>
    ''' Trim all white spaces incl. space, CR, LF, TAB
    ''' </summary>
    ''' <param name="value"></param>
    ''' <returns></returns>
    Public Shared Function TrimAllWhiteSpaces(value As String) As String
        Dim Result As New System.Text.StringBuilder(value)
        Do While Result.Length <> 0
            Select Case Result.Chars(0)
                Case " "c, ControlChars.Cr, ControlChars.Lf, ControlChars.Tab
                    Result.Remove(0, 1)
                Case Else
                    Exit Do
            End Select
        Loop
        For MyCounter As Integer = Result.Length - 1 To 0 Step -1
            Select Case Result.Chars(MyCounter)
                Case " "c, ControlChars.Cr, ControlChars.Lf, ControlChars.Tab
                    Result.Remove(MyCounter, 1)
                Case Else
                    Exit For
            End Select
        Next
        Return Result.ToString
    End Function

    ''' <summary>
    ''' Check if a value is member of an array of values
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="value"></param>
    ''' <param name="allowedValues"></param>
    ''' <returns></returns>
    Public Shared Function IsOneOf(Of T)(value As T, ParamArray allowedValues As T()) As Boolean
        Dim GType As Type = GetType(T)
        If GType.IsArray Then
            Throw New NotSupportedException("Arrays as generic type not supported")
        ElseIf GType.IsInterface Then
            Throw New NotSupportedException("Interfaces as generic type not supported")
        ElseIf GType.IsClass AndAlso GType Is GetType(String) Then
            If allowedValues Is Nothing OrElse allowedValues.Length = 0 Then
                Return False
            Else
                Return allowedValues.Contains(value)
            End If
        ElseIf GType.IsClass Then
            If allowedValues Is Nothing OrElse allowedValues.Length = 0 Then
                Return False
            Else
                Return allowedValues.Contains(value)
            End If
        ElseIf GType.IsValueType Then
            If allowedValues Is Nothing OrElse allowedValues.Length = 0 Then
                Return False
            Else
                Return allowedValues.Contains(value)
            End If
        Else
            Throw New NotSupportedException("Unsupported generic type " & GType.FullName & "/" & GType.GetGenericTypeDefinition.FullName)
        End If
    End Function

    ''' <summary>
    ''' Replace occurance of searchValue only if it is found at the very end of a string
    ''' </summary>
    ''' <param name="text"></param>
    ''' <param name="searchValue"></param>
    ''' <param name="replacement"></param>
    ''' <returns></returns>
    Public Shared Function ReplaceAtEndOfString(text As String, searchValue As String, replacement As String) As String
        If text = Nothing OrElse searchValue = Nothing OrElse text.Length < searchValue.Length Then Return text
        If text.EndsWith(searchValue) Then
            Return text.Substring(0, text.Length - searchValue.Length) & replacement
        Else
            Return text
        End If
    End Function

End Class
