<%@ Page Title="Home Page" Language="VB" AutoEventWireup="True"%>
<script runat="server">
	Sub Prova()
		Etic.Text="La Mappa delle Isole Galapagos"
	End Sub
</script>
	
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN"
    "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head >
<script type="text/javascript">
	
</script>
</head>
<body>
    <form >
	<p><%Response.Write(now())%></p>
	<input type="checkbox" ID="Tast" onclick="Prova">
	<label for="Tast">Tasto</label> 
	<asp:Label ID="Etic" runat="server" Text="Mappa delle Galapagos" onclick="Prova"></asp:Label>
    <asp:Image ID="Mapp" runat="server" src="Content/Galapagos_Map.jpg" onclick="Prova"/></asp:Image>
    </form>
</body>
</html>