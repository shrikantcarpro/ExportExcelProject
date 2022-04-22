<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Query1.aspx.cs" Inherits="ExportExcelProject.Query1" EnableEventValidation = "false"    %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script language="javascript" src="/EngDev/GenScripts94/GenFunctions.js"></script>
     <link type="text/css" rel="Stylesheet" href="/EngDev/GenScripts94/jquery-ui.css" />
<script type="text/javascript" src="/EngDev/GenScripts94/jquery.min.js" >
</script>
    <style type="text/css">


    .rounded_corners
    {
        
        border: 1px solid #A1DCF2;
        -webkit-border-radius: 8px;
        -moz-border-radius: 8px;
        border-radius: 8px;
        overflow-x: scroll;
    }
    .rounded_corners td, .rounded_corners th
    {
        border: 1px solid #A1DCF2;
        font-family: Arial;
        font-size: 10pt;
        text-align: center;
    }
    .rounded_corners table table td
    {
        border-style: none;
    }
     .header
        {
            background-color: #646464;
            color:White;
            font-family: Arial;
            color: White;
            border: none 0px transparent;
            height: 25px;
            text-align: center;
            font-size: 16px;
        }
 
     .pager
        {
            background-color: white;
            font-family: Arial;
            color: black;
            height: 30px;
            text-align: left;
        }
     
  
</style>


<script type="text/javascript" src="/EngDev/GenScripts94/jquery-ui.min.js" >
</script>


      <script type = "text/javascript">
            function isNumberKey(evt)
      {
         var charCode = (evt.which) ? evt.which : evt.keyCode;
         if (charCode > 31 && (charCode < 48 || charCode > 57))
            return false;    
         return true;
      }
       $(function () {
           $(".date").datepicker({
                changeMonth: true,
                changeYear: true,
                //showButtonPanel: true,
               dateFormat: 'dd/mm/y',
               beforeShow: function () {
                   $(".ui-datepicker").css('font-size', 12)
               }
           });
          });

      $(function () {            
             $('#<%=GridView1.ClientID %>').colResizable({
                liveDrag: true,
                gripInnerHtml: "<div class='grip'></div>",
                draggingClass: "dragging",              
            });
        });      
 
  </script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:Panel ID="ddlPanel" runat="server"> 

        <div id="dvTableContent" runat="server">


            </div>


      <div  id="myWorkContent" runat="server" > 



            <asp:GridView ID="GridView1" runat="server"     RowStyle-BackColor="#A1DCF2" AlternatingRowStyle-BackColor="White"
        RowStyle-ForeColor="#3A3A3A"  PagerStyle-CssClass="pager" HeaderStyle-CssClass="header"   HeaderStyle-BackColor="#3AC0F2"
        HeaderStyle-ForeColor="White" AllowPaging="True" OnPageIndexChanging="OnPageIndexChanging" ShowFooter="false" AllowSorting="True" OnSorting="OnSorting" BorderStyle="None" AllowColumnsReorder="True" RowDataBound="GridView1_RowDataBound" >
              
                    

                <PagerSettings  Mode="NumericFirstLast" FirstPageText="First" LastPageText="Last" />


                    

            </asp:GridView>

            <asp:SqlDataSource ID="SqlDataSource1" runat="server"></asp:SqlDataSource>

        </div>
        <div id="TotalRecDis" style="display:none" runat="server">
             Total No. Records&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
             <asp:Label ID="Label1" runat="server" Text=""></asp:Label> 
             &nbsp;&nbsp;&nbsp; Page Size:&nbsp;&nbsp;&nbsp;
             <asp:TextBox ID="Pagesize" runat="server" Height="16px" Width="41px" AutoPostBack="True" OnTextChanged="txtsize_textchanged" ></asp:TextBox>

        </div>
        <div>
        Select Report   <asp:DropDownList ID="DropDownList1" runat="server" DataSourceID="SqlDataSource2" DataTextField="rptname" DataValueField="rptquery">
        </asp:DropDownList>
        <asp:SqlDataSource ID="SqlDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:ReportConnectionString %>" SelectCommand="SELECT [rptno], [rptname], [rptquery] FROM [cmb_rpt_query] WHERE ([rptno] = @rptno)">
            <SelectParameters>
                <asp:QueryStringParameter Name="rptno" QueryStringField="ReportID" Type="Int32" />
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:Button ID="Button4" runat="server"   OnClick="Button4_Click" style="margin-bottom: 0px" Text="Export Excel" />
        <asp:Button ID="Button3" runat="server" OnClick="Button3_Click" Text="LoadData"  />
        </div>
            </asp:Panel>
    </form>
</body>
</html>
