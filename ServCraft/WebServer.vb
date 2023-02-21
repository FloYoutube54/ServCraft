Imports System.IO
Imports System.Net

Public Class WebServer

    Dim Listener As HttpListener
    Dim PagesCollection As List(Of WebServerPage)
    Dim APICollection As List(Of WebServerAPI)

    Public Sub New(Port As Integer)
        Listener = New HttpListener()
        Listener.Prefixes.Add($"http://localhost:{Port}/")
        PagesCollection = New List(Of WebServerPage)
        APICollection = New List(Of WebServerAPI)
    End Sub

    Public Function StartServer() As Boolean
        Try
            Listener.Start()
        Catch ex As Exception
            MessageBox.Show($"Erreur lors du démarrage du serveur web : {ex.Message}", "Erreur !", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End Try
        Threading.ThreadPool.QueueUserWorkItem(AddressOf WorkLoad)
        Return True
    End Function

    Private Sub WorkLoad()
        Do
            Dim Context As HttpListenerContext = Listener.GetContext()
            Dim Request As HttpListenerRequest = Context.Request
            Dim Response As HttpListenerResponse = Context.Response
            Dim Page As WebServerPage = PagesCollection.Find(Function(x) x.Path = Request.Url.AbsolutePath)
            Dim API As WebServerAPI = APICollection.Find(Function(x) x.Path = Request.Url.AbsolutePath)
            If Page IsNot Nothing Then
                Console.WriteLine($"Loading {Page.Page} (attached to {Request.Url.AbsolutePath})")
                LoadPage(Request.Url.AbsolutePath, Page.Page, Response)
            ElseIf API IsNot Nothing Then
                Console.WriteLine($"Invoking {API.Path} (attached to {Request.Url.AbsolutePath})")
                If API.Func.Invoke(New StreamReader(Request.InputStream, Request.ContentEncoding).ReadToEnd).IsSuccessful Then
                    Response.StatusCode = 200
                Else
                    Response.StatusCode = 500
                End If
                Response.Close()
            Else
                Console.WriteLine($"The page {Request.Url.AbsolutePath} does not exist.")
                Response.StatusCode = 404
                Response.Close()
            End If
        Loop
    End Sub

    Public Function StopServer() As Boolean
        Listener.Stop()
        Return True
    End Function

    Private Function LoadPage(Path As String, Page As String, Response As HttpListenerResponse)
        Dim Data As Byte() = Text.Encoding.UTF8.GetBytes(WebServeContent.ResGetter.GetPage(Page))
        Response.ContentLength64 = Data.Length
        Response.OutputStream.Write(Data, 0, Data.Length)
        Response.OutputStream.Close()
    End Function

    Public Function AddPage(Path As String, Page As String)
        PagesCollection.Add(New WebServerPage With {.Path = Path, .Page = Page})
    End Function

    Public Function AddAPI(Path As String, Func As Func(Of String, WebServerAPIFunction))
        APICollection.Add(New WebServerAPI With {.Path = Path, .Func = Func})
    End Function

End Class

Class WebServerPage
    Property Path As String
    Property Page As String
End Class

Class WebServerAPI
    Property Path As String
    Property Func As Func(Of String, WebServerAPIFunction)
End Class

Public Class WebServerAPIFunction
    Property DataType As Type
    Property Data As Object
    Property IsSuccessful As Boolean
End Class
