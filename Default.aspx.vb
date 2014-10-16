Option Explicit Off

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Configuration
Imports System.Web.UI
Imports System.Web.UI.HtmlControls

Partial Class _Default



    Inherits Page

    Protected WithEvents HeadContent As Global.System.Web.UI.WebControls.ContentPlaceHolder
    'Protected WithEvents HorMenu As System.Web.UI.HtmlControls.HtmlGenericControl

    Shared Sub Main()

    End Sub

    Public Sub HorMenu_Load(sender As Object, e As EventArgs)

        Dim queryString As String = "SELECT ColoMenu, RigaMenu, TipoVoceMenu, VoceMenu, SorgenteKML, Acronimi FROM HorMenu WHERE ColoMenu = "
        Dim I As Integer
        Dim stringa As String
        Dim DescrizioneColonnaMenu As String
        Dim UseDb As String
        Dim HtmlInterno As String
        Dim VoceMenu As HtmlGenericControl
        Dim XMLvar As String

        'MsgBox(Server.MachineName)

        Select Case Server.MachineName
            Case "DANERI"
                UseDb = "Impianti"
            Case Else
                UseDb = "DbImpianti"
        End Select

        XMLvar = "<menu> </menu>"

        Using connection As New SqlConnection(GetConnectionStringByName(UseDb))


            connection.Open()

            For I = 1 To 6 'Itero sulle sei colonne del menù
                VoceMenu = sender.FindControl("Menu" & Trim(Str(I)))
                DescrizioneColonnaMenu = "Menu" & Trim(Str(I))

                Dim commandHead As New SqlCommand(queryString & Trim(Str(I)) & " and RigaMenu = 0", connection)
                Dim readerHead As SqlDataReader = commandHead.ExecuteReader()

                Try
                    While readerHead.Read()
                        If (readerHead Is Nothing) Then

                        Else
                            DescrizioneColonnaMenu = readerHead.Item(3)

                        End If
                    End While
                Finally
                    readerHead.Close()
                End Try

                HtmlInterno = DescrizioneColonnaMenu & "<ul>"

                Dim command As New SqlCommand(queryString & Trim(Str(I)) & " and RigaMenu > 0", connection)
                Dim reader As SqlDataReader = command.ExecuteReader()

                Try
                    While reader.Read()

                        If VoceMenu Is Nothing Then

                        Else
                            Select Case reader.Item(2).ToString
                                Case "input type=""checkbox"""
                                    stringa = "<" & reader.Item(2) & " class=""visto"" id=""" & LCase(reader.Item(3)) & """ onclick=""MenuElement(this.id)"" data-kml=""" & reader.Item(4) & """>" & _
                                        "<label class=""pulsante"" id=""lab_" & LCase(reader.Item(3)) & """ for=""" & LCase(reader.Item(3)) & """>" & reader.Item(3) & "</label>"
                                    HtmlInterno = HtmlInterno & "<li>" & stringa & "</li>"
                                Case "a href=""#"""
                                    HtmlInterno = HtmlInterno & "<li>" & reader.Item(3) & "</li>"
                                Case "label"
                                    HtmlInterno = HtmlInterno & "<li>" & reader.Item(3) & "</li>"
                                Case Else
                            End Select

                        End If

                    End While

                Finally
                    reader.Close()
                End Try
                HtmlInterno = HtmlInterno & "</ul>"
                VoceMenu.InnerHtml = HtmlInterno
            Next I
        End Using

    End Sub

    Private Shared Function GetConnectionStringByName(ByVal name As String) As String

        ' Assume failure
        Dim returnValue As String = Nothing

        ' Look for the name in the connectionStrings section.
        Dim settings As ConnectionStringSettings = ConfigurationManager.ConnectionStrings(name)

        ' If found, return the connection string.
        If Not settings Is Nothing Then
            returnValue = settings.ConnectionString
        End If

        Return returnValue
    End Function

End Class