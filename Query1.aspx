<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Query1.aspx.cs" Inherits="ExportExcelProject.Query1" EnableEventValidation = "false"    %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
   
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


          function isDate(field) {
	gField = field
	var thisYear = getTheYear()
			if(!isDateFormat((thisYear - 200),(thisYear + 100)))
			{
			event.returnValue=false;
			gField.value="";
			gField.focus();
			gField.select();
			return false
			}
	return true
          }
          function getTheYear() {
	var thisYear = (new Date()).getFullYear()
	thisYear = (thisYear < 100)? thisYear + 1900: thisYear
	return thisYear
          }

          	function isDateFormat(minYear,maxYear,minDays,maxDays) {
	        var inputStr = gField.value
	       
	            if ((inputStr=="00/00/0000") || (inputStr=="00/00/00")){
	              gField.value = "00/00/0000"
	                  return true
            	}

            	// convert hyphen delimiters to slashes
	   		        while (inputStr.indexOf("-") != -1) {
			           inputStr = replaceString(inputStr,"-","/")
			           }
	
	            var delim1 = inputStr.indexOf("/")
                	var delim2 = inputStr.lastIndexOf("/")
			   if (delim1 != -1 && delim1 == delim2) {
			   // there is only one delimiter in the string
			   alert("|^The date entry is not in an acceptable format.^| |^You can enter dates in the following formats:^| |^mmddyyyy, mm/dd/yyyy or mm-dd-yyyy.^| |^(If the month or date data is not available, enter \01\ in the appropriate location)^|")
			   return false;
			   }
			
			   if (delim1 != -1) {
			   // there are delimiters; extract component values
			   var dd = parseInt(inputStr.substring(0,delim1),10)
			   var mm = parseInt(inputStr.substring(delim1 + 1,delim2),10)
			   var yyyy = parseInt(inputStr.substring(delim2 + 1, inputStr.length),10)
			   }
			   else {
			   // there are no delimiters; extract component values
			   var dd = parseInt(inputStr.substring(0,2),10)
			   var mm = parseInt(inputStr.substring(2,4),10)
			   var yyyy = parseInt(inputStr.substring(4,inputStr.length),10)
			   }
			
			   if (isNaN(mm) || isNaN(dd) || isNaN(yyyy)) {
			   // there is a non-numeric character in one of the component values
			   alert("|^The date entry is not in an acceptable format.^| |^You can enter dates in the following formats:^| |^ddmmyyyy, dd/mm/yyyy, or dd-mm-yyyy.^|")
			   return false
			   }
		
			   if (dd < 1 || dd > 31) {
			   // date value is not 1 thru 31
			   alert("|^Days must be entered between the range of 01 and a maximum of 31 (depending on the month and year).^|")
			   return false
			   }
			
			   if (mm < 1 || mm > 12) {
			   // month value is not 1 thru 12
			   alert("|^Months must be entered between the range of 01 (January) and 12 (December).^|")
			   return false
			   }

			   // validate year, allowing for checks between year ranges
			   // passed as parameters from other validation functions
			   if (yyyy < 100) {
			   // entered value is two digits, which we allow for 1980-2030
			   	  		
						  if (yyyy >= 30) {
						  yyyy += 1900
						  }
						   else {
						  yyyy += 2000
						  }

						
//			     yyyy += 2000
			   }
			
            var today = new Date()
			  if (!minYear) {
			  // func called with specific day range parameters
			  var dateStr = new String(dd + "/" + mm + "/" + yyyy)
			  }
			  else if (minYear && maxYear) {
			  // func called with specific year range parameters
			   	  	if (yyyy < minYear || yyyy > maxYear) {
					// entered year is outside of range passed from calling function
					alert("|^The most likely range for this entry is between the years^|" + minYear + "|^and^| " + maxYear + ".")
					return false
					}
			  }
			  else if (yyyy < minYear || yyyy > maxYear) {
					alert("|^It is unusual for a date entry to be before^| " + minYear +" "+ "|^or after^|" + maxYear + "|^. Please verify this entry.^|")
					return false
					}
			
	          if (!checkMonthLength(dd,mm)) {
				return false
				}
			
			  if (mm == 2) {
			   	    if (!checkLeapMonth(dd,mm,yyyy)) {
			   			return false
			   		}
			  }

	            // put the Informix-friendly format back into the field
                var zero1="";//amir change
            var zero2="";
            if (dd<10) zero1="0";
                if (mm<10) zero2="0";
	
	
	            gField.value = zero1+dd + "/" +zero2+ mm + "/" + yyyy
	
	
	
	
	            return true
	}

          	function checkMonthLength(dd,mm) {
	var months = new Array("","|January^|","|^February^|","|^March^|","|^April^|","|^May^|","|^June^|","|^July^|","|^August^|","|^September^|","|^October^|","|^November^|","|^December^|")
	
		if ((mm == 4 || mm == 6 || mm == 9 || mm == 11) && dd > 30)
		{
		alert(months[mm] + "|^Has only 30 days.^|")
		return false
		}
		 else if (dd > 31) {
		alert(months[mm] + "|^Has only 31 days.^|")
		return false
		}
	return true
	}
 
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
