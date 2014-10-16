<%@ Page Title="Home Page" Language="VB" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="~/Default.aspx.vb" Inherits="_Default" %>


<asp:Content runat="server" ID="FeaturedContent" ContentPlaceHolderID="FeaturedContent" >
    <div class="content-wrapper">
        <ul class="HorMenu" id="HorMenu" runat="server" onload="HorMenu_Load">
            <li class="pulsante" id="Menu1" title="Gallerie" runat="server">
                Gallerie
            </li>
            <li class="pulsante" id="Menu2" title="Caselli" runat="server">
                Caselli
            </li>
		    <li class="pulsante" id="Menu3" title="ImpSpec" runat="server">
                Impianti Speciali
            </li>
            <li class="pulsante" id="Menu4" title="ImpTLC" runat="server">
                Impianti TLC
            </li>
		    <li class="pulsante" id="Menu5" title="Edifici" runat="server">
                Edifici
            </li>
		    <li class="pulsante" id="Menu6" title="Help" runat="server">
                Help
            </li>
		    <li class="pulsante" id="Spazio1"  runat="server">
                
            </li>
		    <li class="pulsante" id="Spazio2"  runat="server">
                
            </li>
   		    <li class="pulsante" id="Spazio3"  runat="server">
                
            </li>
   		    <li class="pulsante" id="Spazio4"  runat="server">
                
            </li>


        </ul>
    </div>
</asp:Content>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    

    <div id="SezioneSinistra" style="float:left; display:block">
        <!--<iframe id="Mappa" width="1024" height="680" src="http://impiantiadf.azurewebsites.net/Content/Archivio/AdF.html"></iframe-->
    <!--http://localhost:49554/Content/Archivio/AdF.html -->
    <!--http://impiantiadf.azurewebsites.net/Content/Archivio/AdF.html -->
    </div>
    <div id="SezioneDestra" style="float:left; display:block" >
        <!--iframe id="WidgInfo" name="WidgInfo" width="680" height="680" src="http://impiantiadf.azurewebsites.net/Content/Archivio/SezioneSinistra.aspx"></iframe-->  
    <!--http://impiantiadf.azurewebsites.net/Content/Archivio/SezioneSinistra.aspx -->
    <!--http://localhost:49554/Content/Archivio/SezioneSinistra.aspx -->
    </div>
    <div style="clear:both">

    </div>

</asp:Content>

