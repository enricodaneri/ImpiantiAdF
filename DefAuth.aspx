<%@ Page Title="Home Page" Language="VB" MasterPageFile="~/Site.master" AutoEventWireup="false" CodeFile="~/DefAuth.aspx.vb" Inherits="_DefAuth" %>


<%@ Register assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>


<asp:Content runat="server" ID="FeaturedContent" ContentPlaceHolderID="FeaturedContent" >

    <div class="content-wrapper overLap">
        <link rel="stylesheet" href="https://impianti.blob.core.windows.net/js-rep/jquery-ui.css" />
        <link rel="stylesheet"  type="text/css"  href="https://cdn.datatables.net/1.10.13/css/jquery.dataTables.css"/>
        <link rel="stylesheet"  type="text/css"  href="https://cdn.datatables.net/buttons/1.2.4/css/buttons.dataTables.css"/>
        <script type="text/javascript" src="https://impianti.blob.core.windows.net/js-rep/jquery-2.1.1.js"></script>
        <!--script type="text/javascript" src="https://code.jquery.com/jquery-2.2.4.js"--><!--/script-->
        <script type="text/javascript" src="https://impianti.blob.core.windows.net/js-rep/jquery-ui.js"></script>
        <script type="text/javascript" src="https://impianti.blob.core.windows.net/js-rep/jquery.ui-contextmenu.js"></script>
        <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/jszip/2.5.0/jszip.js"></script>
        <script type="text/javascript" src="https://cdn.rawgit.com/bpampuch/pdfmake/0.1.18/build/pdfmake.js"></script>
        <script type="text/javascript" src="https://cdn.rawgit.com/bpampuch/pdfmake/0.1.18/build/vfs_fonts.js"></script>
        <script type="text/javascript" src="https://cdn.datatables.net/1.10.13/js/jquery.dataTables.js"></script>
        <script type="text/javascript" src="https://cdn.datatables.net/buttons/1.2.4/js/dataTables.buttons.js"></script>
        <script type="text/javascript" src="https://cdn.datatables.net/buttons/1.2.4/js/buttons.html5.js"></script>
        <script type="text/javascript" src="https://cdn.datatables.net/buttons/1.2.4/js/buttons.print.js"></script>
        <script type="text/javascript">
            function RightMenuElement(idChBox) {
                var coloMenu = document.getElementById(idChBox).parentNode.id;
                if (document.getElementById(idChBox).attributes["sub-menu"].value.length > 0) {
                    var allMenu = JSON.parse(document.getElementById(idChBox).attributes["sub-menu"].nodeValue.replace(/'/g, '"'));
                    $('#' + coloMenu).contextmenu({
                        delegate: ".hasMenu",
                        menu: allMenu,
                        select: function (event, ui) {
                            __doPostBack('CustomPostBack', ui.cmd);
                        }
                    });
                    $(".ui-menu-item").each(function (index) {
                        $(this).addClass("pulsante");
                        $(this).height("24px");
                    });
                    $(".ui-menu").each(function (index) {
                        $(this).attr("onmouseout", "mOut(this.id, event)");
                    });
                };
            };
        </script>
        <script type="text/javascript">
            //Costruisce il nome del file kml dei diversi impianti visualizzabili su google maps
            function MenuElement(idChBox) {
                var ColoMenu = ["Menu Impianti"];
                var MenuXML = "<menu><name>" + ColoMenu[0] + "</name>";
                var tabella = document.getElementById(idChBox).name;
                var bosco = GeneraBosco();
                for (i = 1; i < 7; i++) {
                    MenuXML = MenuXML + "<colo_menu><name>" + document.getElementById("FeaturedContent_Menu".concat(i)).title + "</name>";
                    ColoMenu[i] = document.getElementById("FeaturedContent_Menu".concat(i)).getElementsByTagName("input");
                    if (ColoMenu[i].length > 0) {
                        for (j = 0; j < ColoMenu[i].length; j++) {
                            MenuXML = MenuXML + "<voce_menu><name>" + ColoMenu[i][j].id + "</name><id>" + ColoMenu[i][j].name + "</id><selezionato>" + JSON.stringify(ColoMenu[i][j].checked) + "</selezionato><KML-file>" + ColoMenu[i][j].dataset.kml + "</KML-file></voce_menu>";
                        }
                    }
                    MenuXML = MenuXML + "</colo_menu>";
                }
                MenuXML = MenuXML + "</menu>";
                var xmlMenu = '<?xml version="1.0" encoding="UTF-8"?>' + MenuXML;
                //var jsonMenu = xml2json.fromStr(xmlMenu, 'string');
                //var Dati = '{"Dati": {"radice": "' + tabella + '", "albero": "' + tabella + '", "tabella": "' + tabella + '", "azione": "QueryPage", "abilitato": false, "bosco":[' + bosco + '], "XML": ' + jsonMenu + ' } }';
                var Dati = '{"Dati": {"radice": "' + tabella + '", "albero": "' + tabella + '", "tabella": "' + tabella + '", "azione": "QueryPage", "abilitato": false, "bosco":[' + bosco + '] } }';
                var win = document.getElementById("Mappa").contentWindow;
                var dow = document.getElementById("WidgInfo").contentWindow;
                var monHost = "http://" + self.location.host;
                $($('.Selezionatore')[0]).hide();
                win.postMessage(MenuXML, monHost);
                dow.postMessage(Dati, monHost);
            }
            // funzione per la scelta dall'elenco dei componenti
            function MenuComponent(Costui) {
                var tabella = Costui.attributes['title'].value;
                var bosco = GeneraBosco();
                var Dati = '{"Dati": {"albero": "' + tabella + '", "tabella": "' + tabella + '", "azione": "Lista", "abilitato": false, "bosco":[' + bosco +  ']} }';
                var dow = document.getElementById("WidgInfo").contentWindow;
                var monHost = "http://" + self.location.host;
                dow.postMessage(Dati, monHost);
                $($('.Selezionatore')[0]).hide();
            };
            function SelCreate(Costui) {
                $(".Selezionatore")[0].innerHTML = $($('.selComp')[0]).attr('ih').replace(/«/g, '<').replace(/»/g, '>');
                $($(".Selezionatore")[0]).show()
            };
            function GeneraBosco() {
                var vivaio = new Array();
                $(".visto").each(function () {
                    var albero = '';
                    albero = ($(this)[0]).attributes['name'].textContent;
                    if (($(this)[0]).checked) {
                        vivaio.push('"'+albero+'"');
                    }
                });
                return vivaio;
            };
        </script>
        <script type="text/javascript">
            function mOut(loId, e) {
                if (!e) var e = window.event;
                var tg = (window.event) ? e.srcElement : e.target;
                if (tg.nodeName != 'LI') return;
                var reltg = (e.relatedTarget) ? e.relatedTarget : e.toElement;
                if (reltg.nodeName != 'UL' && reltg.nodeName != 'LI') {
                    $('#' + loId).remove();
                }
            }
            function mExit(loId, e) {
                $("#" + loId)[0].childNodes[1].style.height = "125px";

            }
        </script>
        <script type="text/javascript">
            $(document).ready(function () {
                $(".expMenu").each(function (index) {
                    $(this).attr("onclick", "mExit(this.id, event)");
                });
                $($('.Selezionatore')[0]).hide();
                if ($('#FeaturedContent_head_table_id').length > 0) {
                    var HeadTable = $($('#FeaturedContent_head_table_id')[0]).attr('htmlin');
                    var BodyTable = $('#FeaturedContent_table_id').html();
                    $('#FeaturedContent_table_id').html(HeadTable + BodyTable);
                    var Table = $('#FeaturedContent_table_id').DataTable({
                        dom: 'Bfrtip',
                        buttons: ['copy', 'excel', 'print', 'pdf',
                            {text: 'Chiudi',
                            action: function (e, dt, node, config) {
                                    var Bosco = GeneraBosco();
                                    $('#FeaturedContent_table_id_wrapper').remove();
                                    $('#FeaturedContent_head_table_id').remove();
                                }
                            }
                        ]
                    })
                }
            })
        </script>
        <ul class="HorMenu" id="HorMenu" runat="server" onload="HorMenu_Load">
            <li class="pulsante expMenu" id="Menu1" title="Gallerie" runat="server">
                Gallerie
            </li>
            <li class="pulsante expMenu" id="Menu2" title="Caselli" runat="server">
                Caselli
            </li>
		    <li class="pulsante expMenu" id="Menu3" title="ImpSpec" runat="server">
                Impianti Speciali
            </li>
            <li class="pulsante expMenu" id="Menu4" title="ImpTLC" runat="server">
                Impianti TLC
            </li>
		    <li class="pulsante expMenu" id="Menu5" title="Cartografia" runat="server">
                Edifici
            </li>
		    <li class="pulsante expMenu" id="Menu6" title="Edifici" runat="server">
                Help
            </li>
		    <li class="pulsante expMenu" id="Menu7"  runat="server">

            </li>
		    <li class="pulsante expMenu" id="Menu8"  runat="server">
                
            </li>
   		    <li class="pulsante expMenu" id="Menu9"  runat="server">
                
            </li>
   		    <li class="pulsante expMenu" id="Menu10"  title="Help" runat="server">
                
            </li>
        </ul>
        <ol id="SeleComp" class="Selezionatore" runat="server">
        </ol>
    </div>
</asp:Content>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <div class="content-wrapper">
        <div id="SezioneSinistra" style="float:left; display:block; width:58.4%">
            <iframe id="Mappa" width="1040" height="680" src="Content/Archivio/AdF.html"></iframe>
        </div>
        <div id="SezioneDestra" style="float:left; display:block; width:38.9%">
            <iframe id="WidgInfo" name="WidgInfo" width="690" height="680" src="Content/Archivio/SezioneDestra.aspx"></iframe>
        </div>
        <div id="SezioneFinale" style="clear:both">

        </div>
    </div>
</asp:Content>