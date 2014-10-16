<%@ Page Language="VB" AutoEventWireup="false" CodeFile="SezioneSinistra.aspx.vb" Inherits="Content_Archivio_SezioneSinistra" EnableViewState="true" EnableEventValidation="false" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Informazione Impianti</title>
    <webopt:bundlereference runat="server" path="~/Content/css" />
</head>
<body>
    <script type="text/javascript">
        window.onload = function () {
            function listener(event) {
                //if (event.origin !== "http://localhost:49554")
                if (event.origin !== "http://impiantiadf.azurewebsites.net")
                    return;
                var TabInform = document.getElementById("InfoTavola");
                if (TabInform) {
                    TabInform.innerHTML = ""
                };
                __doPostBack('CustomPostBack', event.data);
                }
            if (window.addEventListener) {
                addEventListener("message", listener, false)
            }
            else {
            attachEvent("onmessage", listener)
            }
        }
    </script>
    
    <script type="text/javascript">
        function AbilitaModifiche(Dati) {
            //alert(Dati.Dati.abilitato);
            Dati.Dati.abilitato = document.getElementById("Abilita").checked;
            //document.getElementById("InfoTavola").innerHTML = ""
            __doPostBack('CustomPostBack', JSON.stringify(Dati));
        }
    </script>

    <form id="InfoImpianto" runat="server">

    </form>
    
</body>
</html>
