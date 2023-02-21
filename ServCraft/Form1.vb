Imports System.ComponentModel

Public Class Form1
    Dim WebServer As New WebServer(8080)
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        WebServer.AddPage("/home", "Index")
        WebServer.AddPage("/404", "404")
        WebServer.AddAPI("/api", AddressOf API)
    End Sub

    Function API(args As String) As WebServerAPIFunction
        MessageBox.Show(args)
        Return New WebServerAPIFunction With {.IsSuccessful = False}
    End Function

    Private Sub Form1_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        WebServerStatusLabel.Text = "Serveur web : Démarrage..."
        If WebServer.StartServer() Then
            WebServerStatusLabel.Text = "Serveur web : En ligne"
        Else
            WebServerStatusLabel.Text = "Serveur web : Hors ligne"
        End If
    End Sub

    Private Sub Form1_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        WebServerStatusLabel.Text = "Serveur web : Arrêt..."
        'WebServer.StopServer()
    End Sub
End Class
