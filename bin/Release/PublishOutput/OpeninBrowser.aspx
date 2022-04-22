<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OpeninBrowser.aspx.cs" Inherits="ExportExcelProject.OpeninBrowser" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
    <script>
        function init()
        {
        document.attachEvent('onkeydown',disable_f5);
		document.attachEvent('oncontextmenu',DisableRClick);
        }
        
        if (document.all)
{   
 document.onkeydown = function ()
 {
         var key_f5 = 116;   
		
  if (key_f5==event.keyCode)
  {
	        event.keyCode=0;
          return false;
  }
 }
        }

        function DisableRClick(){
	return false;
}
function disable_f5(){
         var key_f5 = 116; // 116 = F5  
  if (key_f5==event.keyCode)
  {
           event.keyCode=0;
          return false;
  }
 }

        </script>
<body onload="init();" oncontextmenu="return false;">
    <form id="form1" runat="server">
        <div>
            <asp:Label ID="Label1" runat="server" Text=""></asp:Label>
        </div>
    </form>
</body>
</html>
