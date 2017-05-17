Option Explicit Off

Imports System.Data
Imports System.Data.SqlClient
'Imports System.Windows.Documents

Partial Class _DefAuth
    Inherits Page

    Protected WithEvents HeadContent As Global.System.Web.UI.WebControls.ContentPlaceHolder

    Shared Sub Main()

    End Sub

    Public Sub HorMenu_Load(sender As Object, e As EventArgs)

        Dim stringa As String
        Dim DescrizioneColonnaMenu As String
        Dim HtmlInterno As String
        Dim VoceMenu As HtmlGenericControl
        Dim Testata As New HtmlInputGenericControl

        If Page.IsPostBack Then
            Dim Bersaglio As String = Me.Request("__EVENTTARGET")
            Dim Argomento As String = Me.Request("__EVENTARGUMENT")
            Dim TabData As New HtmlTable
            Dim queryString As String
            If Bersaglio = "CustomPostBack" Then
                queryString = "SELECT Query FROM cmdQuery WHERE Comando = '" & Argomento & "'"
                queryString = EstraiDato(queryString)
                Dim Sorgente As New DataTable("Sorgente")
                Sorgente = EstraiDati(queryString)
                If Sorgente.Rows.Count > 0 Then
                    With TabData
                        Dim HtmlIn As String = "<thead> <tr>"
                        For j As Integer = 0 To Sorgente.Columns.Count - 1
                            Dim HtmlInIn As String = "<th>" + Sorgente.Columns(j).ColumnName + "</th>"
                            HtmlIn = HtmlIn + HtmlInIn
                        Next j
                        HtmlIn = HtmlIn + "</tr> </thead>"
                        Testata.Attributes.Add("HtmlIn", HtmlIn)
                        Testata.Attributes.Add("type", "hidden")
                        Testata.ID = "head_table_id"
                        .ID = "table_id"
                        .Attributes.Add("Class", "display")
                    End With
                    With Sorgente
                        For i As Integer = 0 To .Rows.Count - 1
                            Dim tr As HtmlTableRow = New HtmlTableRow()
                            For j As Integer = 0 To .Columns.Count - 1
                                Dim tc As HtmlTableCell = New HtmlTableCell()
                                tc.InnerText = .Rows(i).Item(j).ToString
                                tr.Cells.Add(tc)
                            Next j
                            TabData.Rows.Add(tr)
                        Next i
                    End With

                End If
                HorMenu.Controls.Add(Testata)
                HorMenu.Controls.Add(TabData)
            End If
        End If

        For I = 1 To 10 'Itero sulle sei colonne del menù
            VoceMenu = sender.FindControl("Menu" & Trim(Str(I)))
            Dim dT As DataTable = EstraiDati("SELECT * FROM HorMenu WHERE ColoMenu = " & I & " AND RigaMenu = 0")
            If dT.Rows.Count > 0 Then
                DescrizioneColonnaMenu = dT.Rows(0).Item("VoceMenu")
                'HtmlInterno = DescrizioneColonnaMenu & "<ul class=""MenuDaAprire_" & I & """ id=""ColoMenu_" & I & """>"
                HtmlInterno = DescrizioneColonnaMenu & "<ul class=""MenuDaAprire"" id=""ColoMenu_" & I & """>"
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
                                HtmlInterno = HtmlInterno & "<li class=""hasMenu"" oncontextmenu=""RightMenuElement(this.id)"" id=""input_" & LCase(.Item("VoceMenu")) & """ sub-menu=""" & .Item("SottoMenu") & """>" & stringa & "</li>"
                            End With
                        Case "a href=""#"""
                            HtmlInterno = HtmlInterno & "<li>" & dR.Item("VoceMenu") & "</li>"
                        Case "label"
                            HtmlInterno = HtmlInterno & "<li>" & dR.Item("VoceMenu") & "</li>"
                        Case "select"
                            Dim dq As DataTable = EstraiDati(dR.Item("SorgenteKML"))
                            'HtmlInterno = HtmlInterno & "<li ><label class=""pulsante"" onclick=""SelCreate(this)"">" & dR.Item("VoceMenu") & "</label></li>"
                            SeleComp.InnerHtml = ""
                            Dim InnHtm As String = ""
                            For J = 0 To dq.Rows.Count - 1
                                InnHtm = InnHtm + "&#171;li class=&quot;pulsante&quot; onclick=&quot;MenuComponent(this)&quot; title=&quot;" & dq.Rows(J).Item("Acronimo") & "&quot; &raquo;" & dq.Rows(J).Item("Nome Foglio") & "&#171;/li&raquo;"
                            Next J
                            HtmlInterno = HtmlInterno & "<li><label class=""selComp pulsante"" onclick=""SelCreate(this)"" IH=""" & InnHtm & """>" & dR.Item("VoceMenu") & "</label></li>"
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

    Shared Function EstraiDato(QueryString As String) As String

        Dim UseDb As String
        Dim Colonna As String = ""

        EstraiDato = ""
        Select Case Environment.MachineName
            Case "DANERI"
                UseDb = "Impianti"
            Case Else
                UseDb = "DbImpianti"
        End Select

        Using connection As New SqlConnection(GetConnectionStringByName(UseDb))
            connection.Open()
            Dim commandHead As New SqlCommand("Set DateFormat MDY", connection)
            commandHead.ExecuteNonQuery()
            Dim command As New SqlCommand(QueryString, connection)
            Dim readerHead As SqlDataReader = command.ExecuteReader()
            Try
                While readerHead.Read()
                    If (readerHead Is Nothing) Then
                    Else
                        If IsDate(readerHead.Item(0)) Then
                            EstraiDato = Format(readerHead.Item(0), "d")
                        Else
                            EstraiDato = readerHead.Item(0).ToString
                        End If
                    End If
                End While
            Finally
                readerHead.Close()
            End Try
            connection.Close()
        End Using

    End Function

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