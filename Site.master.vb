Imports System.Data
Imports System.Data.SqlClient
Imports Microsoft.Reporting.WebForms

Partial Class SiteMaster
    Inherits MasterPage
    Private Const AntiXsrfTokenKey As String = "__AntiXsrfToken"
    Private Const AntiXsrfUserNameKey As String = "__AntiXsrfUserName"
    Private _antiXsrfTokenValue As String

    Protected Sub Page_Init(sender As Object, e As EventArgs)
        ' Il codice seguente facilita la protezione da attacchi XSRF
        Dim requestCookie = Request.Cookies(AntiXsrfTokenKey)
        Dim requestCookieGuidValue As Guid
        If requestCookie IsNot Nothing AndAlso Guid.TryParse(requestCookie.Value, requestCookieGuidValue) Then
            ' Utilizzare il token Anti-XSRF dal cookie
            _antiXsrfTokenValue = requestCookie.Value
            Page.ViewStateUserKey = _antiXsrfTokenValue
        Else
            ' Generare un nuovo token Anti-XSRF e salvarlo nel cookie
            _antiXsrfTokenValue = Guid.NewGuid().ToString("N")
            Page.ViewStateUserKey = _antiXsrfTokenValue

            Dim responseCookie = New HttpCookie(AntiXsrfTokenKey) With {
                .HttpOnly = True,
                .Value = _antiXsrfTokenValue
            }
            If FormsAuthentication.RequireSSL AndAlso Request.IsSecureConnection Then
                responseCookie.Secure = True
            End If
            Response.Cookies.[Set](responseCookie)
        End If

        AddHandler Page.PreLoad, AddressOf master_Page_PreLoad
    End Sub

    Protected Sub master_Page_PreLoad(sender As Object, e As EventArgs)
        If Not IsPostBack Then
            ' Impostare il token Anti-XSRF
            ViewState(AntiXsrfTokenKey) = Page.ViewStateUserKey
            ViewState(AntiXsrfUserNameKey) = If(Context.User.Identity.Name, [String].Empty)
        Else
            ' Convalidare il token Anti-XSRF
            If DirectCast(ViewState(AntiXsrfTokenKey), String) <> _antiXsrfTokenValue OrElse DirectCast(ViewState(AntiXsrfUserNameKey), String) <> (If(Context.User.Identity.Name, [String].Empty)) Then
                Throw New InvalidOperationException("Convalida del token Anti-XSRF non riuscita.")
            End If
        End If
    End Sub

    Protected Sub Page_Load(sender As Object, e As EventArgs)

    End Sub

    Protected Sub Unnamed_LoggingOut(sender As Object, e As LoginCancelEventArgs)
        Context.GetOwinContext().Authentication.SignOut()
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
            Dim commandHead As New SqlCommand("SET DateFormat MDY", connection)
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
