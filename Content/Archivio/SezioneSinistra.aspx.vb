Imports System.Collections.Generic
Imports System.Security.Claims
Imports System.Security.Principal
Imports System.Web
Imports System.Web.Security
Imports System.Web.UI
Imports System.Web.UI.HtmlControls
Imports System.Web.UI.WebControls
Imports System.Data.SqlClient
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.Data
Imports System.Data.Common

Class RowContr

    Inherits System.Web.HttpServerUtilityBase

    ' Sintassi della composizione Tavola di Riepilogo:
    ' [Riga 1];[Riga 2];....[Riga n]
    ' ciascuna Riga ha la seguente sintassi:
    ' xxKnumCarLnumCar....
    ' dove xx è il numero della prima colonna dati della tabella
    ' K,M.. è il tipo di Controllo
    ' numCar é il numero di caratteri di ciascun controllo
    ' K,M.. possono assumere i seguenti valori:
    ' L : Etichetta che precede una Casella di Testo (T) o una Casella Combinata (B)
    ' C : Caption che segue una Casella di Spunta (S)
    ' T : Casella di Testo
    ' B : Casella Combinata
    ' S : Caslla di Spunta

    Public RigaGrezza As String
    Public NumRiga As Integer       ' Numero riga nella Tavola di riepilogo dei dati (il numero di Celle per riga è dato dalla quantità di controlli per ciascuna riga)
    Public NumColo As Integer       ' Numero della Colonna nella tabella 
    Public Tabella As String        ' Nome della tabella (Per la Casella Combinata è la tabella dei dati della CC)  
    Public AcroTab As String        ' Nome della prima colonna (è sempre di due caratteri), nel caso della Casella Combinata è il nome della Prima colonna della Tabella Dati a cui si riferisce (attraverso la Tabella "Acronimi" si risale al nome della Tabella Dati ) 
    Public ColoDescr As String      ' Nome della seconda colonna della Tabella Dati , è utilizzato solo per la Casella Combinata
    Public Elemento As String       ' Chiave di ricerca dell'impianto selezionato (è nella prima colonna della Tabella
    Public TipoControllo As Char    ' Vedi intestazione
    Public Titolo As String         ' Titolo (nome) della Colonna
    Public NumeCarContr As Integer  ' Vedi intesatazione
    Public Value As String          ' Per i TipiControllo (L, C, T) è il testo, per S True/False, per B il valore (dalla prima colonna della tabella dei dati della CC)
    Public ID As String             ' ID
    Public Enabled As Boolean       ' Abilta o disabilita il controllo

    Public ReadOnly Property IsLabel As Boolean
        Get
            If TipoControllo = "L" Then
                Return True
            Else
                Return False
            End If
        End Get
    End Property

    Public ReadOnly Property HtmlValue As WebControl
        Get
            Select Case TipoControllo
                Case "L"
                    Dim Etichetta As New Label
                    With Etichetta
                        .Text = Value
                        '.Height = 18.75
                        .Width = 10 * NumeCarContr
                        .CssClass = "tLabel"
                        .Enabled = Enabled
                        .ID = ID
                    End With
                    Return Etichetta
                Case "C"
                    Dim Caption As New Label
                    With Caption
                        .Text = Value
                        '.Height = 18.75
                        .Width = 10 * NumeCarContr
                        .CssClass = "tLabel"
                        .Enabled = Enabled
                        .ID = ID
                    End With
                    Return Caption
                Case "T"
                    Dim CasellaTesto As New TextBox
                    With CasellaTesto
                        .Text = Value
                        '.Height = 18.75
                        .Width = 10 * NumeCarContr
                        .CssClass = "tText"
                        .Enabled = Enabled
                        .Attributes.Add("DefaultValue", "")
                        .ViewStateMode = ViewStateMode.Disabled
                        .ID = ID
                    End With
                    Return CasellaTesto
                Case "B"
                    Dim ComboBox As New DropDownList
                    With ComboBox
                        Dim SorgenteDati As DataTable
                        Dim queryString As String
                        queryString = "SELECT """ & Titolo & """, """ & ColoDescr & """ FROM [" & ProtectSpaces(Tabella, 1) & "]"
                        SorgenteDati = EstraiDati(queryString, Tabella, Titolo, ColoDescr)
                        '.Height = 18.75
                        .Width = 10 * NumeCarContr
                        .CssClass = "tText"
                        .Enabled = Enabled
                        .ID = ID
                        .DataSource = SorgenteDati
                        .DataTextField = SorgenteDati.Columns.Item(ColoDescr).ColumnName
                        .DataValueField = SorgenteDati.Columns.Item(Titolo).ColumnName
                        .DataBind()
                    End With
                    Return ComboBox
                Case "S"
                    Dim Spunta As New CheckBox
                    With Spunta
                        .Checked = (UCase(Value) = "X")
                        '.Height = 18.75
                        .Width = .Height
                        .CssClass = "tText"
                        .Enabled = Enabled
                        .ID = ID
                    End With
                    Return Spunta
                Case Else
                    MsgBox("Controllo sconosciuto")
                    Return Nothing
            End Select

        End Get
    End Property

    Function EstraiDati(ByVal queryString As String, ByVal Tab As String, ByVal Tit As String, ByVal Des As String) As DataTable

        Dim UseDb As String
        Dim dt As New DataTable
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
            'readerHead.Fill(dt)

            Dim mapping As DataTableMapping = readerHead.TableMappings.Add(Tabella, Tabella)
            mapping.ColumnMappings.Add(Titolo, Titolo)
            mapping.ColumnMappings.Add(ColoDescr, ColoDescr)


            readerHead.FillSchema(dt, SchemaType.Mapped)
            readerHead.Fill(dt)


        End Using
        EstraiDati = dt

    End Function

    Private Function ProtectSpaces(Frase As String, Apici As Integer) As String

        Dim StrApici = Left("''", Apici)

        If Len(Frase) = Len(Replace(Frase, " ", "")) Then
            ProtectSpaces = Frase
        Else
            ProtectSpaces = StrApici & Frase & StrApici
        End If

    End Function

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

Class chkAbil

    Public ValPost As String
    Public Checked As Boolean
    Private _HtmlValue As CheckBox

    Public ReadOnly Property IsChecked As Boolean
        Get
            If Checked Then
                Return True
            Else
                Return False
            End If
        End Get
    End Property

    Public ReadOnly Property HtmlValue As WebControl

        Get
            Dim Spunta As New CheckBox
            With Spunta
                .Width = .Height
                .ID = "Abilita"
                .Height = 25
                .Width = 180
                .Text = "Abilita Modifiche"
                .TextAlign = TextAlign.Right
                .InputAttributes.Add("Class", "tCheck visto")
                .InputAttributes.Add("onChange", "AbilitaModifiche(" & ValPost & ")")
                .InputAttributes.Add("StrRicerca", ValPost)
                .LabelAttributes.Add("Class", "tLabel")
                .LabelAttributes.Add("Width", "140px")
                .Checked = Checked
                .Enabled = True
                .AutoPostBack = False
            End With
            Return Spunta
        End Get

        'Set(value As Control)
        '    Return
        'End Set

    End Property

    ' Declare an event.
    Public Event Click(sender As Object, e As Abilitazione)

    Public Sub CauseEvent(sender As Object, e As Abilitazione)
        ' Raise an event on successful logon.
        e.Abilitato = IsChecked
        RaiseEvent Click(HtmlValue, EventArgs.Empty)
    End Sub

End Class

Class Abilitazione
    Inherits EventArgs

    Public Abilitato As Boolean

End Class

Partial Class Content_Archivio_SezioneSinistra

    Inherits System.Web.UI.Page

    Public Structure Dati
        Public tabella As String        'Acronimo della tabella (è l'indice della tabella Acronimi, e corrisponde al nome della prima colonna delle tabelle_dati ([nome]$)), 
        Public elemento As String       'Elemento selezionato (è l'indice sulla tabella_dati)
        Public abilitato As Boolean     'Controllo dell'abilitazione alle modifiche
        Public nomeTabella As String    'Nome della tabella_dati (è salvato, senza il $, per compatibilità con Excel, nella tabella Acronimi)
        Public strContr As String       'la stringa che contiene le informazioni di composizione della tavola informazioni
    End Structure

    Private WithEvents Abilitante As chkAbil
    Private valuePassed As String
    Private JsonValuePassed As Dati

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load

        Page.ClientScript.GetPostBackEventReference(Me, String.Empty)

        If Me.IsPostBack Then
            Dim eventTarget As String
            Dim eventArgument As String

            If (Me.Request("__EVENTTARGET") Is Nothing) Then
                eventTarget = String.Empty
            Else
                eventTarget = Me.Request("__EVENTTARGET")
                If (Me.Request("__EVENTARGUMENT") Is Nothing) Then
                    eventArgument = String.Empty
                Else
                    eventArgument = Me.Request("__EVENTARGUMENT")
                    Select Case eventTarget
                        Case Is = "CustomPostBack"
                            valuePassed = eventArgument
                            InfoImpianto.Controls.Clear()
                            InfoImpianto.Dispose()
                            InfoImpianto.Controls.Add(GetDatiDb())
                        Case Else

                    End Select
                End If
            End If
        Else

        End If
    End Sub

    Function GetDatiDb() As Table

        Dim jResults As JObject = JObject.Parse(valuePassed)

        With JsonValuePassed
            .tabella = jResults.SelectToken("Dati").SelectToken("tabella")
            .elemento = jResults.SelectToken("Dati").SelectToken("elemento")
            .abilitato = jResults.SelectToken("Dati").SelectToken("abilitato")
            .nomeTabella = EstraiDati("SELECT [Nome Foglio] FROM Acronimi WHERE Acronimo = '" & (jResults.SelectToken("Dati").SelectToken("tabella").ToString) & "'") & "$"
            .strContr = EstraiDati("SELECT [Controlli Form Proprietà] FROM Acronimi WHERE Acronimo = '" & (jResults.SelectToken("Dati").SelectToken("tabella").ToString) & "'") & "$"
        End With

        Dim Tavola As New Table
        Dim baseQuery As New ArrayList
        Dim headQuery As New ArrayList
        Dim footQuery As New ArrayList
        headQuery = Str2Intes()
        baseQuery = Str2Contr()
        footQuery = Str2Piede()
        If baseQuery.Count = 0 Then

        Else
            InfoImpianto.Controls.Add(Tavola)
            With Tavola
                .ID = "InfoTavola"
                .Caption = headQuery(0).Value
                .CaptionAlign = TableCaptionAlign.Top
                .CssClass = "InfoImpianto"
                .Height = (baseQuery.Count + 3) * 28
            End With
            Dim Righe As New ArrayList
            Dim Intestazione As New TableHeaderRow
            Dim headCell As New TableCell
            Intestazione.Cells.Add(headCell)
            Tavola.Rows.Add(Intestazione)

            For Z = 1 To 2
                Dim headCellContr As TextBox
                headCellContr = headQuery(Z).htmlValue
                headCell.Controls.Add(headCellContr)
            Next Z

            For Q = 0 To baseQuery.Count - 1
                Dim Riga As New TableRow
                Riga.Height = 24
                Riga.Width = 2
                Dim Cella As New TableCell
                For R = 1 To baseQuery(Q).count - 1
                    Dim LargCella As Integer
                    LargCella = CInt(Val(Riga.Width.ToString) + baseQuery(Q)(R).NumeCarContr * 8)
                    Riga.Width = CInt(LargCella + 4)
                    Cella.Width = LargCella
                    Cella.Controls.Add(baseQuery(Q)(R).HtmlValue)
                Next R
                Riga.Cells.Add(Cella)
                Righe.Add(Riga)
                Tavola.Rows.Add(Righe(Q))
            Next Q

            Dim PiePagina As New TableFooterRow
            Dim footCell As New TableCell
            PiePagina.Cells.Add(footCell)
            Tavola.Rows.Add(PiePagina)
            footCell.Controls.Add(footQuery(0).HtmlValue)
            For Z = 1 To 3
                footCell.Controls.Add(footQuery(Z))
            Next Z

        End If

        Dim Aringa As JToken
        Aringa = JToken.FromObject(JsonValuePassed)
        valuePassed = "{ ""Dati"": " & Aringa.ToString & "}"
        GetDatiDb = Tavola

    End Function

    Public Function Str2Intes() As ArrayList

        Dim E As New ArrayList
        Dim strQuery As String
        Dim IntCaption As New RowContr
        Dim IntHeadSig As New RowContr
        Dim IntHeadDes As New RowContr

        'StrQuery = "SELECT ""Caption scegli"" FROM [Acronimi] WHERE Acronimo = '" & AcroTab & "'"
        strQuery = "SELECT ""Caption scegli"" FROM [Acronimi] WHERE Acronimo = '" & JsonValuePassed.tabella & "'"
        With IntCaption
            .TipoControllo = "L"
            .Value = EstraiDati(StrQuery)
            .NumeCarContr = Len(.Value) + 1
        End With
        E.Add(IntCaption)

        With IntHeadSig
            .TipoControllo = "T"
            '.Enabled = False
            .Value = JsonValuePassed.elemento
            .NumeCarContr = Len(.Value) + 4
            .Enabled = JsonValuePassed.abilitato
        End With
        E.Add(IntHeadSig)

        Dim NomeColonna As String
        'StrQuery = "SELECT ""COLUMN_NAME"" FROM information_schema.columns WHERE table_name = '" & ProtectSpaces(Tabel, 2) & "' AND ORDINAL_POSITION = " & "2"
        strQuery = "SELECT ""COLUMN_NAME"" FROM information_schema.columns WHERE table_name = '" & ProtectSpaces(JsonValuePassed.nomeTabella, 2) & "' AND ORDINAL_POSITION = " & "2"
        NomeColonna = EstraiDati(StrQuery)
        'StrQuery = "SELECT """ & NomeColonna & """ FROM [" & ProtectSpaces(Tabel, 1) & "] WHERE " & AcroTab & " = '" & Elem & "'"
        strQuery = "SELECT """ & NomeColonna & """ FROM [" & ProtectSpaces(JsonValuePassed.nomeTabella, 1) & "] WHERE " & JsonValuePassed.tabella & " = '" & JsonValuePassed.elemento & "'"
        With IntHeadDes
            .TipoControllo = "T"
            '.Enabled = False
            .Value = EstraiDati(strQuery)
            .NumeCarContr = Len(.Value)
            .Enabled = JsonValuePassed.abilitato
        End With
        E.Add(IntHeadDes)

        Str2Intes = E

    End Function

    Public Function Str2Contr() As ArrayList

        Dim B As String = ""
        Dim C As Integer = 1
        Dim D As Integer = 0
        Dim DD As Integer = 0
        Dim E As New ArrayList
        Dim G As String
        Dim H As Char
        Dim strQuery As String

        For I = 1 To Len(JsonValuePassed.strContr)
            ' Divido il modulo delle righe [R1];[R2];....[Rn] nelle singole righe [R1] [R2]...[Rn]
            ' Il FOR sono le prime n-1 righe, l'IF l'ultima riga
            Dim A As Char = Mid(JsonValuePassed.strContr, I, 1)
            B = Mid(JsonValuePassed.strContr, C, I - C)
            If A = ";" Then
                C = I + 1
                I = I + 1
                D = D + 1
                Dim Controlli As New RowContr
                Dim F As New ArrayList
                Controlli.RigaGrezza = B
                F.Add(Controlli)
                E.Add(F)
            End If
        Next I
        If Len(B) > 0 Then
            D = D + 1
            B = B & Right(JsonValuePassed.strContr, 1)
            Dim Controlli As New RowContr
            Dim F As New ArrayList
            Controlli.RigaGrezza = B
            F.Add(Controlli)
            E.Add(F)
        End If

        If D = 0 Then
            MsgBox("Nessun elemento da visualizzare per la tabella richiesta!")
        Else
            For I = 0 To E.Count - 1
                E(I)(0).NumColo = CInt(Val(Mid(E(I)(0).RigaGrezza, 2, 2)))
                DD = E(I)(0).NumColo
                For J = 4 To Len(E(I)(0).RigaGrezza) - 1
                    Dim A As Char = UCase(Mid(E(I)(0).RigaGrezza, J, 1))
                    Select Case A
                        Case "L", "T", "B", "C", "S"
                            G = ""
                            Dim M As Integer = 0
                            Dim Controllo As New RowContr
                            Dim ID_Ctrl = A & "_" & Right(Str((100 + I + 1)), 2) & "_" & Right(Str((100 + E(I).Count)), 2)
                            E(I).Add(Controllo)
                            Do
                                M = M + 1
                                H = UCase(Mid(E(I)(0).RigaGrezza, J + M, 1))
                                If Not ((Asc(H) >= 48 And Asc(H) <= 57)) Then
                                    Exit Do
                                End If
                                If M > 100 Then
                                    MsgBox("Qualcosa non va!")
                                    Exit Do
                                End If
                                G = G & H
                            Loop While (Asc(H) >= 48 And Asc(H) <= 57)
                            J = J + M - 2
                            With E(I)(E(I).Count - 1)
                                .Tabella = JsonValuePassed.nomeTabella
                                .AcroTab = JsonValuePassed.tabella
                                .Elemento = JsonValuePassed.elemento
                                .Enabled = JsonValuePassed.abilitato
                                .NumRiga = I
                                .NumColo = DD
                                .ID = ID_Ctrl
                                .TipoControllo = A
                                .NumeCarContr = CInt(Val(G))
                            End With
                        Case Else

                    End Select
                    Select Case A
                        Case "L" ' Etichetta (è sempre legata ad una casella T, B - che precede)
                            With E(I)(E(I).Count - 1)
                                strQuery = "SELECT ""COLUMN_NAME"" FROM information_schema.columns WHERE table_name = '" & ProtectSpaces(.Tabella, 2) & "' AND ORDINAL_POSITION = " & .NumColo
                                .Titolo = EstraiDati(strQuery)
                                .Value = .Titolo
                            End With
                        Case "T" ' Casella di Testo (è preceduta da un'etichetta L)
                            With E(I)(E(I).Count - 1)
                                strQuery = "SELECT ""COLUMN_NAME"" FROM information_schema.columns WHERE table_name = '" & ProtectSpaces(.Tabella, 2) & "' AND ORDINAL_POSITION = " & .NumColo
                                .Titolo = EstraiDati(strQuery)
                                strQuery = "SELECT """ & .Titolo & """ FROM [" & ProtectSpaces(.Tabella, 1) & "] WHERE " & .AcroTab & " = '" & .Elemento & "'"
                                .Value = EstraiDati(strQuery)
                            End With
                            DD = DD + 1
                        Case "B" ' Casella Combinata (è preceduta da un'etichetta L)
                            With E(I)(E(I).Count - 1)
                                strQuery = "SELECT ""COLUMN_NAME"" FROM information_schema.columns WHERE table_name = '" & ProtectSpaces(JsonValuePassed.nomeTabella, 2) & "' AND ORDINAL_POSITION = " & .NumColo
                                .Titolo = EstraiDati(strQuery)
                                strQuery = "SELECT ""Nome Foglio"" FROM Acronimi WHERE Acronimo = '" & .Titolo & "'"
                                .Tabella = EstraiDati(strQuery) & "$"
                                strQuery = "SELECT ""COLUMN_NAME"" FROM information_schema.columns WHERE table_name = '" & ProtectSpaces(.Tabella, 2) & "' AND ORDINAL_POSITION = 2"
                                .ColoDescr = EstraiDati(strQuery)
                                strQuery = "SELECT """ & .Titolo & """ FROM [" & ProtectSpaces(JsonValuePassed.nomeTabella, 1) & "] WHERE " & JsonValuePassed.tabella & " = '" & .Elemento & "'"
                                .Value = EstraiDati(strQuery)
                            End With
                            DD = DD + 1
                        Case "C" ' Didascalia (è sempre legata ad una casella S - che segue)
                            With E(I)(E(I).Count - 1)
                                .Titolo = EstraiDati("SELECT ""COLUMN_NAME"" FROM information_schema.columns WHERE table_name = '" & ProtectSpaces(JsonValuePassed.nomeTabella, 2) & "' AND ORDINAL_POSITION = " & .NumColo)
                                .Value = .Titolo
                            End With
                            DD = DD + 1
                        Case "S" ' Casella di spunta (è seguita da una didascalia C)
                            With E(I)(E(I).Count - 1)
                                strQuery = "SELECT ""COLUMN_NAME"" FROM information_schema.columns WHERE table_name = '" & ProtectSpaces(JsonValuePassed.nomeTabella, 2) & "' AND ORDINAL_POSITION = " & .NumColo
                                .Titolo = EstraiDati(strQuery)
                                strQuery = "SELECT """ & .Titolo & """ FROM ['" & ProtectSpaces(.Tabella, 1) & "'] WHERE " & .AcroTab & " = '" & .Elemento & "'"
                                .Value = EstraiDati(strQuery)
                            End With
                        Case Else

                    End Select
                Next J
            Next I
        End If

        Str2Contr = E

    End Function

    Public Function Str2Piede() As ArrayList

        Dim E As New ArrayList

        Abilitante = New chkAbil
        E.Add(Abilitante)
        Dim Salva As New Button
        E.Add(Salva)
        Dim Esci As New Button
        E.Add(Esci)
        Dim Stampa As New Button
        E.Add(Stampa)

        With Abilitante
            .ValPost = valuePassed
            .Checked = JsonValuePassed.abilitato
        End With
        With Salva
            .Text = "Salva"
            .CssClass = "tButton"
            .Enabled = JsonValuePassed.abilitato
        End With
        With Esci
            .Text = "Esci"
            .CssClass = "tButton"
            .Enabled = JsonValuePassed.abilitato
        End With
        With Stampa
            .Text = "Stampa"
            .CssClass = "tButton"
            .Enabled = JsonValuePassed.abilitato
        End With

        Str2Piede = E

    End Function

    Private Function EstraiDati(QueryString As String) As String

        Dim UseDb As String
        Dim Colonna As String = ""

        EstraiDati = ""
        Select Case Environment.MachineName
            Case "DANERI"
                UseDb = "Impianti"
            Case Else
                UseDb = "DbImpianti"
        End Select

        Using connection As New SqlConnection(GetConnectionStringByName(UseDb))

            connection.Open()

            Dim commandHead As New SqlCommand(QueryString, connection)
            Dim readerHead As SqlDataReader = commandHead.ExecuteReader()

            Try
                While readerHead.Read()
                    If (readerHead Is Nothing) Then

                    Else
                        EstraiDati = readerHead.Item(0).ToString
                    End If
                End While
            Finally
                readerHead.Close()
            End Try

        End Using

        'MsgBox(EstraiDati)

    End Function

    Private Function ProtectSpaces(Frase As String, Apici As Integer) As String

        Dim StrApici = Left("''", Apici)

        If Len(Frase) = Len(Replace(Frase, " ", "")) Then
            ProtectSpaces = Frase
        Else
            ProtectSpaces = StrApici & Frase & StrApici
        End If

    End Function

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
