Option Explicit Off

Imports System.Data
Imports System.Data.SqlClient

Partial Class _Default
    Inherits Page
    Protected WithEvents HeadContent As Global.System.Web.UI.WebControls.ContentPlaceHolder

    Shared Sub Main()

    End Sub

    Public Sub HorMenu_Load(sender As Object, e As EventArgs)

        Dim I As Integer
        Dim stringa As String
        Dim DescrizioneColonnaMenu As String
        Dim HtmlInterno As String = ""
        Dim VoceMenu As HtmlGenericControl

        For I = 1 To 10 'Itero sulle sei colonne del menù
            VoceMenu = sender.FindControl("Menu" & Trim(Str(I)))
            Dim dT As DataTable = EstraiDati("SELECT * FROM HorMenu WHERE ColoMenu = " & I & " AND RigaMenu = 0")
            If dT.Rows.Count > 0 Then
                DescrizioneColonnaMenu = dT.Rows(0).Item("VoceMenu")
                HtmlInterno = DescrizioneColonnaMenu & "<ul>"
                dT = EstraiDati("SELECT * FROM HorMenu WHERE ColoMenu = " & I & " AND RigaMenu <> 0")
                Dim dR As DataRow
                For Each dR In dT.Rows
                    Select Case dR.Item("TipoVoceMenu")
                        Case "input type=""checkbox"""
                            Dim nome As String
                            With dR
                                If DBNull.Value.Equals(.Item("Acronimi")) Then
                                    nome = ""
                                Else
                                    nome = .Item("Acronimi")
                                End If

                                stringa = "<" & .Item("TipoVoceMenu") & " class=""visto"" id=""" & LCase(.Item("VoceMenu")) & """ onclick=""MenuElement(this.id)"" data-kml=""" & .Item("SorgenteKML") & """ name=""" & nome & """>" &
                                          "<label class=""pulsante"" id=""lab_" & LCase(.Item("VoceMenu")) & """ for=""" & LCase(.Item("VoceMenu")) & """>" & .Item("VoceMenu") & "</label>"
                            End With
                            HtmlInterno = HtmlInterno & "<li>" & stringa & "</li>"
                        Case "a href=""#"""
                            HtmlInterno = HtmlInterno & "<li>" & dR.Item("VoceMenu") & "</li>"
                        Case "label"
                            HtmlInterno = HtmlInterno & "<li>" & dR.Item("VoceMenu") & "</li>"
                        Case Else

                    End Select

                Next
                HtmlInterno = HtmlInterno & "</ul>"
                VoceMenu.InnerHtml = HtmlInterno
            Else
                VoceMenu.InnerHtml = "<ul> </ul>"
            End If
        Next I

    End Sub

    Shared Function EstraiDati(ByVal queryString As String) As DataTable

        Dim UseDb As String
        Dim dt As New DataTable
        Dim constrTab As New DataTable
        EstraiDati = Nothing

        Select Case Environment.MachineName
            Case "DANERI"
                UseDb = "Impianti"
            Case Else
                UseDb = "DbImpianti"
        End Select

        Using connection As New SqlConnection(GetConnectionStringByName(UseDb))
            connection.Open()
            Dim commandHead As New SqlCommand(queryString, connection)
            Dim readerHead As SqlDataAdapter = New SqlDataAdapter(commandHead)
            readerHead.FillSchema(dt, SchemaType.Source)
            readerHead.Fill(dt)
            readerHead.FillSchema(constrTab, SchemaType.Source)
            connection.Close()
        End Using
        EstraiDati = dt

    End Function

    Shared Function ProtectSpaces(Frase As String, Apici As Integer) As String

        Dim StrApici = Left("''", Apici)

        If Len(Frase) = Len(Replace(Frase, " ", "")) Then
            ProtectSpaces = Frase
        Else
            ProtectSpaces = StrApici & Frase & StrApici
        End If

    End Function

    Private Shared Function GetConnectionStringByName(ByVal name As String) As String

        Dim returnValue As String = Nothing
        Dim settings As ConnectionStringSettings = ConfigurationManager.ConnectionStrings(name)
        If Not settings Is Nothing Then
            returnValue = settings.ConnectionString
        End If

        Return returnValue

    End Function

End Class