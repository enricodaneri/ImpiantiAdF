<%@ Page Title="Home Page" Language="VB" MasterPageFile="~/Site.Master" AutoEventWireup="true"  CodeFile="~/Default.aspx.vb" Inherits="_Default" %>

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
		    <li class="pulsante" id="Menu5" title="Cartografia" runat="server">
                Edifici
            </li>
		    <li class="pulsante" id="Menu6" title="Edifici" runat="server">
                Help
            </li>
		    <li class="pulsante" id="Menu7"  runat="server">

            </li>
		    <li class="pulsante" id="Menu8"  runat="server">
                
            </li>
   		    <li class="pulsante" id="Menu9"  runat="server">
                
            </li>
   		    <li class="pulsante" id="Menu10"  title="Help" runat="server">
                
            </li>
        </ul>
    </div>
</asp:Content>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <div class="content-wrapper">
        <div id="SezioneSinistra" style="float:left; display:block; width:58.4%">
            <iframe id="Mappa" width="1040" height="680" src="Content/Archivio/AdF.html"></iframe>
        </div>
        <div id="SezioneDestra" style="float:left; display:block; width:38.9%">
            <iframe id="WidgInfo" name="WidgInfo" width="690" height="680" src="Content/Archivio/ToPrint.aspx"></iframe>
        </div>
        <div id="SezioneFinale" style="clear:both">
        
        </div>
    </div>
</asp:Content>