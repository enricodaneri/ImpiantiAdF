﻿Imports System.Web
Imports System.Web.Routing
Imports Microsoft.AspNet.FriendlyUrls
Imports Microsoft.AspNet.FriendlyUrls.Resolvers

Public Partial Class ViewSwitcher
    Inherits System.Web.UI.UserControl
    Protected Property CurrentView As String
    Protected Property AlternateView As String
    Protected Property SwitchUrl As String

    Protected Sub Page_Load(sender As Object, e As EventArgs)
        ' Determinare la vista corrente
        Dim isMobile = WebFormsFriendlyUrlResolver.IsMobileView(New HttpContextWrapper(Context))
        CurrentView = If(isMobile, "Mobile", "Desktop")

        ' Determinare la vista alternativa
        AlternateView = If(isMobile, "Desktop", "Mobile")

        ' Creare un URL di commutazione dalla route, ad esempio ~/__FriendlyUrls_SwitchView/Mobile?ReturnUrl=/Page
        Dim switchViewRouteName = "AspNet.FriendlyUrls.SwitchView"
        Dim switchViewRoute = RouteTable.Routes(switchViewRouteName)
        If switchViewRoute Is Nothing Then
            ' URL amichevoli non è abilitato o il nome del route per cambiare visulizzazione non è sincronizzato
            Me.Visible = False
            Return
        End If
        Dim url = GetRouteUrl(switchViewRouteName, New With {
            .view = AlternateView,
            .__FriendlyUrls_SwitchViews = True
        })
        url += "?ReturnUrl=" & HttpUtility.UrlEncode(Request.RawUrl)
        SwitchUrl = url
    End Sub
End Class
