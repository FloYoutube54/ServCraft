Public Class ResGetter

    Public Shared Function GetPage(Page As String) As String
        Select Case Page
            Case "Index"
                Return My.Resources.WebPages.Index
            Case "404"
                Return My.Resources.WebPages._404
        End Select
    End Function

End Class
