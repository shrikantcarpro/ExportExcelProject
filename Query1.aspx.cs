using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Timers;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClosedXML.Excel;
using ExportToExcel;

namespace ExportExcelProject
{
    public partial class Query1 : System.Web.UI.Page
    {
        private SqlConnection con;
        private SqlCommand com;
        int rptid = 0;
       
        int RequestId = 0;
        string GlobalFilerName="";
        private string constr, query, FullwhereStr;
     
        System.Timers.Timer myTimer = new System.Timers.Timer();
        private void connection()
        {
            constr = ConfigurationManager.ConnectionStrings["ReportConnectionString"].ToString();
            con = new SqlConnection(constr);
           con.Open();

        }
        protected void Page_Load(object sender, EventArgs e)
        {

            
            if (!IsPostBack)
            {

                
                this.BindGrid();
                if (Int32.TryParse(Request.QueryString["ReportID"], out rptid))
                    {
                    RequestId = rptid;

                    AddCriteria();
                    
                }

                



            }
            if (IsPostBack)
            {
                RequestId = Int32.Parse(Request.QueryString["ReportID"]);
                AddCriteria();

                TotalRecDis.Attributes.CssStyle[HtmlTextWriterStyle.Display] = "";
                string ControlID = string.Empty;
                if (!String.IsNullOrEmpty(Request.Form["__EVENTTARGET"]))
                {
                    ControlID = Request.Form["__EVENTTARGET"];


                    if (ControlID.IndexOf("$Pagesize") >= 0)
                    {
                        this.BindGrid();
                    }
                   
                      //  Pagesize();
                   

                }
                //   this.BindGrid();

            }






        }

        private void AddCriteria()
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ReportConnectionString"].ToString()))
            {

                string query = "SELECT REPORT_NO,REPORT_FIL_NAME,REPORT_FIL_FIELD,REPORT_FIL_TYPE From  REPORT_FILTER";
                if (RequestId > 0)
                {
                    query = query + " where REPORT_NO=" + Request.QueryString["ReportID"] + " ORDER BY REPORT_NO, REPORT_FIL_FIELD,REPORT_FIL_TYPE";
                }
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                conn.Open();
                DataSet ds = new DataSet();

                da.Fill(ds, "REPORT_FILTER12");

                DataTable dt = ds.Tables["REPORT_FILTER12"];


                if (ds.Tables[0].Rows.Count > 0)

                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        GenerateTable(3, 1, dt.Rows[i]["REPORT_FIL_FIELD"].ToString(), dt.Rows[i]["REPORT_FIL_TYPE"].ToString(), dt.Rows[i]["REPORT_FIL_NAME"].ToString());
                    }
                   // if (dt.Rows[0]["REPORT_FIL_TYPE"].ToString() == "ALPHA")
                  //  {
                    //    GenerateTable(3, 1, dt.Rows[0]["REPORT_FIL_FIELD"].ToString(), dt.Rows[0]["REPORT_FIL_TYPE"].ToString());

                  //  }


                }

                conn.Close();

            }

        }




            private void PopulateReport()
        {
            connection();
            
            query = "select rptno,rptname,rptquery from cmb_rpt_query ";//not recommended this i have wrtten just for example,write stored procedure for security  
            if (RequestId > 0) { 
                    query= query+" where rptno=" + RequestId.ToString();
            }
            com = new SqlCommand(query, con);
            SqlDataReader dr = com.ExecuteReader();
            DropDownList1.DataSource = dr;
            DropDownList1.DataMember = "rptname";
      //      DropDownList1.DataTextField
                con.Close();

        }
        private string SortDirection
        {
            get { return ViewState["SortDirection"] != null ? ViewState["SortDirection"].ToString() : "ASC"; }
            set { ViewState["SortDirection"] = value; }
        }

        private void BindGrid(string sortExpression = null)
        {
      
            if (!IsPostBack)
            {
                              
                    GridView1.PageSize = Int32.Parse(ConfigurationManager.AppSettings["NoofRec"]);

            }
            if (IsPostBack)
            {
                if (GridView1.PageSize > 200)
                {
                    GridView1.PageSize = Int32.Parse(ConfigurationManager.AppSettings["NoofRec"]); ;
                }
            }
                    
            string strConnString = ConfigurationManager.ConnectionStrings["ReportConnectionString"].ConnectionString;
                using (SqlConnection con = new SqlConnection(strConnString))
                {
                    var querystr = DropDownList1.SelectedValue;

                    if (querystr != "")
                    {

                        FullwhereStr = BuildwhereString();

                        if (FullwhereStr.Trim() != "")
                        {
                            querystr = querystr + " " + FullwhereStr;
                        }

                        using (SqlCommand cmd = new SqlCommand(querystr))
                        //using (SqlCommand cmd = new SqlCommand("select i.invoice_no,i.invoice_type, convert(date,i.invoice_date,112) as invoice_date,case when i.inv_status = 0 then 'cancelled' else 'normal' end as invoice_status,i.invoice_amount, i.AMOUNT_PID,i.balance,d.DEBITOR_NAME from invoices i(nolock) left outer join debitors d(nolock) on i.DEBITOR_NO = d.DEBITOR_CODE"))
                        {
                            using (SqlDataAdapter sda = new SqlDataAdapter())
                            {
                                cmd.Connection = con;
                                sda.SelectCommand = cmd;
                            using (DataTable dt = new DataTable())
                            {
                                sda.Fill(dt);
                                Session["RowsCount"] = dt.Rows.Count;
                               
                                    myWorkContent.Attributes.Add("class", "rounded_corners");
                             
                                if (sortExpression != null)
                                {
                                    DataView dv = dt.AsDataView();
                                    this.SortDirection = this.SortDirection == "ASC" ? "DESC" : "ASC";

                                    dv.Sort = sortExpression + " " + this.SortDirection;
                                    GridView1.DataSource = dv;
                                }
                                else
                                {
                                    GridView1.DataSource = dt;
                                }
                                //  GridView1.DataSource = dt;

                                int colCount = dt.Columns.Count;
                                GridView1.DataBind();

                             
                                DisplyNoofRec(dt.Rows.Count,GridView1.PageSize);
                                

                            }
                           }
                        }
                    }
                }
            
        }

        private void DisplyNoofRec(int count,int Pagesize)
        {
            ((Label)ddlPanel.FindControl("Label1")).Text = count.ToString();
            ((TextBox)ddlPanel.FindControl("Pagesize")).Text = Pagesize.ToString();

        }

      

        protected void txtsize_textchanged(object sender, EventArgs e)
        {
            string txtsize="";
   
               
                    txtsize = ((TextBox)ddlPanel.FindControl("Pagesize")).Text;
            
 
            if (txtsize != "")
            {
                if (txtsize!="0")
                {
                    GridView1.PageSize = Convert.ToInt32(txtsize);
                }   
                if (Convert.ToInt32(txtsize) <= 200 )
                {
                    BindGrid();
                }
                else if (Convert.ToInt32(txtsize) > 200)
                {
                    ((TextBox)ddlPanel.FindControl("Pagesize")).Text = ConfigurationManager.AppSettings["NoofRec"];
                    BindGrid();
                    ClientMessageBox.Show("Plase Enter Page Size Less than 200", this);
                }
                else if (Convert.ToInt32(txtsize) == 0)
                {
                    ((TextBox)ddlPanel.FindControl("Pagesize")).Text = ConfigurationManager.AppSettings["NoofRec"];
                    ClientMessageBox.Show("Plase Enter Page Size more than 0", this);
                }
            }
        }

        public string BuildwhereString()
        {

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ReportConnectionString"].ToString()))
            {

                string WhereString = "WHERE";

                string query = "SELECT REPORT_NO,REPORT_FIL_NAME,REPORT_FIL_FIELD,REPORT_FIL_TYPE,DATE_FIL_FIELD From  REPORT_FILTER";
                if (RequestId > 0)
                {
                    query = query + " where REPORT_NO=" + RequestId;
                }
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                conn.Open();
                DataSet ds = new DataSet();

                da.Fill(ds, "REPORT_FILTER123");

                DataTable dt1 = ds.Tables["REPORT_FILTER123"];


                if (ds.Tables[0].Rows.Count > 0)

                {
                    for (int i = 0; i < dt1.Rows.Count; i++)
                    {
                        string Oparetor = "";
                        string Comboname = "";
                        string ReportFieldName = dt1.Rows[i]["REPORT_FIL_FIELD"].ToString().ToUpper().Trim();

                        string ReportFieldIdName = dt1.Rows[i]["REPORT_FIL_NAME"].ToString().ToUpper().Trim();

                        ReportFieldIdName = Regex.Replace(ReportFieldIdName, @"\s+", "");

                        string DateFilField= dt1.Rows[i]["DATE_FIL_FIELD"].ToString().ToUpper().Trim();
                        if (ReportFieldName.Trim()== "" && dt1.Rows[i]["REPORT_FIL_FIELD"].ToString().ToUpper().Trim() =="" && dt1.Rows[i]["REPORT_FIL_NAME"].ToString().ToUpper().Trim()!="")
                        {
                            ReportFieldName = dt1.Rows[i]["REPORT_FIL_NAME"].ToString().ToUpper().Trim();
                        }
                        string ReportFieldType = dt1.Rows[i]["REPORT_FIL_TYPE"].ToString().ToUpper().Trim();

                        if (ReportFieldType == "ALPHA")
                        {
                            Oparetor = "";
                            Comboname = "DD" + ReportFieldIdName;
                            DropDownList ddl = (DropDownList)ddlPanel.FindControl(Comboname);
                            string Combvalue = ddl.SelectedValue.ToString();


                            if (Combvalue != "")
                            {
                                if (Combvalue.ToUpper() == "EQUAL")
                                    Oparetor = "=";
                                else
                                    Oparetor = "LIKE";

                                string TextboxName = "TextBox" + ReportFieldIdName + "2";


                                if (WhereString.Trim() == "WHERE")
                                {
                                    if (Oparetor == "=")
                                        WhereString += " " + ReportFieldName + " = " + "'" + ((TextBox)ddlPanel.FindControl(TextboxName)).Text + "'";
                                    else
                                        WhereString += " " + ReportFieldName + " LIKE " + "'%" + ((TextBox)ddlPanel.FindControl(TextboxName)).Text + "%'";
                                }
                                else
                                {
                                    if (Oparetor == "=")
                                        WhereString += " AND " + ReportFieldName + " = " + "'" + ((TextBox)ddlPanel.FindControl(TextboxName)).Text + "'";
                                    else
                                        WhereString += " AND " + ReportFieldName + " LIKE " + "'%" + ((TextBox)ddlPanel.FindControl(TextboxName)).Text + "%'";

                                }


                            }
                        }

                         if (ReportFieldType == "NUMBER")
                         {
                                Oparetor = "";
                                Comboname = "DD" + ReportFieldIdName;
                                DropDownList ddl1 = (DropDownList)ddlPanel.FindControl(Comboname);
                                string Combvalue1 = ddl1.SelectedValue.ToString();

                                if (Combvalue1 != "")
                                {
                                    if (Combvalue1.ToUpper() == "EQUAL")
                                        Oparetor = "=";
                                    else if(Combvalue1.ToUpper() == "LESSTHAN")
                                        Oparetor = "<";
                                    else if (Combvalue1.ToUpper() == "GREATERTHAN")
                                        Oparetor = ">";
                                    else if (Combvalue1.ToUpper() == "LESSTHANEQUALTO")
                                        Oparetor = "<=";
                                    else if (Combvalue1.ToUpper() == "GREATERTHANEQUALTO")
                                        Oparetor = ">=";

                                    string TextboxName = "TextBox" + ReportFieldIdName + "2";
                                string TextboxNameBet = "";

                                if (Combvalue1.ToUpper() == "BETWEEN")
                                {
                                     TextboxNameBet = "TextBox" + ReportFieldIdName + "BET";
                                }

                                if (Oparetor != "" && Combvalue1.ToUpper() != "BETWEEN")
                                {
                                    if (WhereString.Trim() == "WHERE")
                                        WhereString += " " + ReportFieldName + " " + Oparetor + " " + "'" + ((TextBox)ddlPanel.FindControl(TextboxName)).Text + "'";
                                    else
                                        WhereString += " AND " + ReportFieldName + " " + Oparetor + " " + "'" + ((TextBox)ddlPanel.FindControl(TextboxName)).Text + "'";
                                }
                                else if (Oparetor == "" && Combvalue1.ToUpper() == "BETWEEN")
                                {
                                    if (WhereString.Trim() == "WHERE")
                                        WhereString += " " + ReportFieldName + " " + ">=" + " " + "'" + ((TextBox)ddlPanel.FindControl(TextboxName)).Text + "'" + " AND " + ReportFieldName + " " + "<=" + " " + "'" + ((TextBox)ddlPanel.FindControl(TextboxNameBet)).Text + "'";
                                    else
                                        WhereString += " AND " + ReportFieldName + " " + ">=" + " " + "'" + ((TextBox)ddlPanel.FindControl(TextboxName)).Text + "'" + " AND " + ReportFieldName + " " + "<=" + " " + "'" + ((TextBox)ddlPanel.FindControl(TextboxNameBet)).Text + "'";
                                }


                                }

                         }



                        if (ReportFieldType == "DATE")
                        {
                            string Testboxname1 = "";
                            string Testboxname2 = "";
                            Oparetor = "";
                            string Oparetor1 = "";
                            Testboxname1 = "TextBox" + ReportFieldIdName + "1";
                            Testboxname2 = "TextBox" + ReportFieldIdName + "2";
                            string TestboxValue1 = ((TextBox)ddlPanel.FindControl(Testboxname1)).Text;

                            string TestboxValue2 = ((TextBox)ddlPanel.FindControl(Testboxname2)).Text;

                            if (TestboxValue1 != "")
                            {
                                TestboxValue1 = GetstringDate(TestboxValue1);
                            }
                            if (TestboxValue2 != "")
                            {
                                TestboxValue2 = GetstringDate(TestboxValue2);
                            }


                            if (TestboxValue1 != "" || TestboxValue2 != "")
                            {
                                if (TestboxValue1 != "" && TestboxValue2 == "")
                                    Oparetor = ">=";
                                else if (TestboxValue1 == "" && TestboxValue2 != "")
                                    Oparetor = "<=";
                                else if (TestboxValue1 != "" && TestboxValue2 != "")
                                {
                                    Oparetor = ">=";
                                    Oparetor1 = "<=";
                                }


                                if (Oparetor != "" )
                                {
                                   
                                    if (WhereString.Trim() == "WHERE" )
                                    {
                                        if (TestboxValue1 != "" && TestboxValue2 == "")
                                        {
                                            WhereString += " " + ReportFieldName + " " + Oparetor + " " + "CONVERT(date,'" + TestboxValue1 + "')";
                                          
                                        }

                                        else if (TestboxValue1 == "" && TestboxValue2 != "")
                                        {
                                            WhereString += " " + ReportFieldName + " " + Oparetor + " " + "CONVERT(date,'" + TestboxValue2 + "')";
                                            
                                        }
                                        else if (TestboxValue1 != "" && TestboxValue2 != "")
                                        {
                                            WhereString += " " + ReportFieldName + " " + Oparetor + " " + "CONVERT(date,'" + TestboxValue1 + "')" + " AND " + ReportFieldName + " " + Oparetor1 + " " + "CONVERT(date,'" + TestboxValue2 + "')";
                                          
                                        }
                                    }
                                    else
                                    {

                                        if (TestboxValue1 != "" && TestboxValue2 == "")
                                        {
                                            WhereString += " AND " + ReportFieldName + " " + Oparetor + " " + "CONVERT(date,'" + TestboxValue1 + "')";
                                          
                                        }
                                        else if (TestboxValue1 == "" && TestboxValue2 != "")
                                        {
                                            WhereString += " AND " + ReportFieldName + " " + Oparetor + " " + "CONVERT(date,'" + TestboxValue2 + "')";
                                         
                                        }
                                        else if (TestboxValue1 != "" && TestboxValue2 != "")
                                        {
                                            WhereString += " AND " + ReportFieldName + " " + Oparetor + " " + "CONVERT(date,'" + TestboxValue1 + "')" + " AND " + ReportFieldName + " " + Oparetor1 + " " + "CONVERT(date,'" + TestboxValue2 + "')" ;
                                            
                                        }

                                    }
                                }
                            }

                        }




                    }
                   

                }
                conn.Close();

                if (WhereString.Trim() != "WHERE")
                {
                    return WhereString;
                }
                else
                {
                    return  "";
                }


                

            }



        }

        private string GetstringDate(string DateStr)
        {

            string StrDate = "";
            if (DateStr != "")
            {
                StrDate = DateStr.Substring(6, 4)+ DateStr.Substring(3, 2) + DateStr.Substring(0, 2) ;
                return StrDate.Trim();
            }
            else
            {
                return "";
            }
        }

        protected void OnPageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            if(Button4.Enabled)
            { 
            GridView1.PageIndex = e.NewPageIndex;
            this.BindGrid();
            }
        }




        protected void OnSorting(object sender, GridViewSortEventArgs e)
        {
            this.BindGrid(e.SortExpression);
        }

       
 
        protected void Button3_Click(object sender, EventArgs e)
        {
            string  message = Dispalyalert();
            if (message.Trim() == "")
            {
                BindGrid();
            }
            else
            {
           
                ClientMessageBox.Show(message, this);
            }
        }

        protected void Button4_Click(object sender, EventArgs e)
        {
            int RowsCount = 0;
            if (RequestId != 0)
            {
                if (ConfigurationManager.AppSettings["ExportRec"].ToString() != "")
                {
                    if (Session["RowsCount"] != null)
                    {
                        RowsCount = (int)(Session["RowsCount"]);
                    }

                    if (RowsCount < Int32.Parse(ConfigurationManager.AppSettings["ExportRec"].ToString()) || Session["RowsCount"] == null)
                    {
                        FullwhereStr = BuildwhereString();
                        var url = "OpeninBrowser.aspx?ReportID=" + Request.QueryString["ReportID"] + "&Id=Export";


                        Session["WhereCondition"] = FullwhereStr;
                        Response.Write("<script> window.open( '" + url + "','_blank' ); </script>");

                    }
                    else
                    {
                        string Mesage = "Max records Export Limit is " + ConfigurationManager.AppSettings["ExportRec"].ToString() + " Please filter the records in order to Export";
                        ClientMessageBox.Show(Mesage, this);
                    }
                }
                else

                {

                    ClientMessageBox.Show("KidlY contact CarPro Systems Export Max Limit is not Set ", this);
                }


            }

        }

        private void GenerateTable(int colsCount, int rowsCount ,string FilterName,string FieldType,string FilterDisName)
        {
            GlobalFilerName ="";
            //Creat the Table and Add it to the Page
            Table table = new Table();
            
                FilterName = Regex.Replace(FilterDisName, @"\s+", "")
;
            table.ID = FilterName.ToUpper();
            Page.Form.Controls.Add(table);
           

            // Now iterate through the table and add your controls 
            for (int i = 0; i < rowsCount; i++)
            {
                TableRow row = new TableRow();

                for (int j = 0; j < colsCount; j++)
                {
                    TableCell cell = new TableCell();
                    if (j == 0)
                    {
                        Label lb = new Label();
                        lb.ID = "Lable"+ FilterName.ToUpper() + j;
                        lb.Text = FilterDisName.ToUpper();
                        lb.Width = 150;
                        cell.Controls.Add(lb);
                        row.Cells.Add(cell);
                    }
                    if (FieldType.ToUpper() == "ALPHA")
                    {
                        if (j == 1)
                        {
                            DropDownList ddl = new DropDownList();

                            ddl.ID = "DD" + FilterName.ToUpper() ;

                            ddl.Items.Add(new ListItem("--Select--", ""));

                            ddl.Items.Add(new ListItem("EQUAL", "EQUAL"));

                            ddl.Items.Add(new ListItem("LIKE", "LIKE"));
                            ddl.Width = 100;

                            //  ddl.Items.Add(new ListItem("Three", "3"));

                            ddl.AutoPostBack = true;
                            ddl.SelectedIndexChanged += new EventHandler(this.ddl_AlphaSelIndexChanged);



                            cell.Controls.Add(ddl);



                            Literal lt = new Literal();

                            lt.Text = "<br />";

                            cell.Controls.Add(lt);

                            row.Cells.Add(cell);

                        }
                    }
                    if (FieldType.ToUpper() == "NUMBER")
                    {
                        if (j == 1)
                        {
                            DropDownList ddl = new DropDownList();

                            ddl.ID = "DD" + FilterName.ToUpper();
                            
                            ddl.Width = 100;
                            ddl.Items.Add(new ListItem("--Select--", ""));

                            ddl.Items.Add(new ListItem("EQUAL", "EQUAL"));

                            ddl.Items.Add(new ListItem("GREATER THAN", "GREATERTHAN"));

                              ddl.Items.Add(new ListItem("LESS THAN", "LESSTHAN"));

                            ddl.Items.Add(new ListItem("LESS THAN EQUAL TO", "LESSTHANEQUALTO"));

                            ddl.Items.Add(new ListItem("GREATER THAN EQUAL TO", "GREATERTHANEQUALTO"));

                            ddl.Items.Add(new ListItem("BETWEEN", "BETWEEN"));

                            ddl.AutoPostBack = true;
                            GlobalFilerName = FilterName;
                            ddl.SelectedIndexChanged += new EventHandler(this.ddl_SelIndexChanged);
                            



                            cell.Controls.Add(ddl);



                            Literal lt = new Literal();

                            lt.Text = "<br />";

                            cell.Controls.Add(lt);

                            row.Cells.Add(cell);

                        }
                    }

                    if (FieldType.ToUpper() == "DATE")
                    {
                        if (j == 1)
                        {
                            TextBox tb = new TextBox();



                            // Set a unique ID for each TextBox added
                            tb.ID = "TextBox" + FilterName.ToUpper() + j;
                            tb.Width = 95;
                            tb.CssClass = "date";
                            // Add the control to the TableCell
                            cell.Controls.Add(tb);
                            row.Cells.Add(cell);
                            tb.Attributes.Add("onChange", "isDate(this)");
                            tb.Attributes.Add("placeholder", "From Date");

                        }
                    }
                    if (j >1)
                    {
                        TextBox tb = new TextBox();



                        // Set a unique ID for each TextBox added
                        tb.ID = "TextBox" + FilterName.ToUpper() + j;
                        tb.Width = 95;
                       
                        // Add the control to the TableCell
                        cell.Controls.Add(tb);
                        row.Cells.Add(cell);
                        if (FieldType.ToUpper() == "DATE")
                        {
                            tb.CssClass = "date";
                            tb.Attributes.Add("onChange", "isDate(this)");
                            tb.Attributes.Add("placeholder", "To Date");

                        }
                        if (FieldType.ToUpper() == "ALPHA")
                        {
                            tb.Attributes.Add("placeholder", "Alpha Numeric");
                        }


                            if (FieldType.ToUpper() == "NUMBER")
                        {


                            tb.Attributes.Add("onkeypress", " return isNumberKey(event)");
                            Literal lt = new Literal();
                            tb.Attributes.Add("placeholder", "Number");
                            lt.Text = "  ";

                            cell.Controls.Add(lt);

                            row.Cells.Add(cell); 

                            TextBox tb1 = new TextBox();
                            // Set a unique ID for each TextBox added
                            tb1.ID = "TextBox" + FilterName.ToUpper() + "BET";
                                tb1.Width = 100;
                               tb1.Style["display"] = "none";
                            // Add the control to the TableCell
                            cell.Controls.Add(tb1);
                                    row.Cells.Add(cell);
                            
                            tb1.Attributes.Add("onkeypress", " return isNumberKey(event)");
                            tb1.Attributes.Add("placeholder", "Number");


                        }
                    }

                
                   

                    // Add the TableCell to the TableRow
                    row.Cells.Add(cell);
                }

                // Add the TableRow to the Table
                table.Rows.Add(row);
            }
            dvTableContent.Controls.Add(table);
        }

        private void ddl_AlphaSelIndexChanged(object sender, EventArgs e)
        {
            DisableControls(GridView1);
        }

        private void ddl_SelIndexChanged(object sender, EventArgs e)
        {
           
                DropDownList comboBox = (DropDownList)sender;
                string selected = comboBox.SelectedValue;
            string TextboxFilname = comboBox.ID.Remove(0, 2);
                

            string TextbName = "";

            TextbName = "TextBox" + TextboxFilname + "BET";
            
            if (selected.Trim() == "BETWEEN")
            {
                ((TextBox)ddlPanel.FindControl("TextBox" + TextboxFilname.Trim() + "2")).Text = "";
                ((TextBox)ddlPanel.FindControl(TextbName)).Text = "";
                ((TextBox)ddlPanel.FindControl(TextbName)).Style["display"] = "";
            }
            else
            {
                ((TextBox)ddlPanel.FindControl("TextBox" + TextboxFilname.Trim() + "2")).Text = "";
                ((TextBox)ddlPanel.FindControl(TextbName)).Text = "";
                ((TextBox)ddlPanel.FindControl(TextbName)).Style["display"] = "none";
            }

            DisableControls(GridView1);


        }

        public string Dispalyalert()
           {
          
            string message = "";
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ReportConnectionString"].ToString()))
                {

                   

                    string query = "SELECT REPORT_NO,REPORT_FIL_NAME,REPORT_FIL_FIELD,REPORT_FIL_TYPE From  REPORT_FILTER";
                    if (RequestId > 0)
                    {
                        query = query + " where REPORT_NO=" + RequestId + " ORDER BY REPORT_NO, REPORT_FIL_FIELD,REPORT_FIL_TYPE" ;
                    }
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    conn.Open();
                    DataSet ds = new DataSet();

                    da.Fill(ds, "REPORT_FILTER1234");

                    DataTable dt1 = ds.Tables["REPORT_FILTER1234"];


                    if (ds.Tables[0].Rows.Count > 0)

                    {
                        for (int i = 0; i < dt1.Rows.Count; i++)
                        {
                            
                            string Comboname = "";

                            string ReportFieldName = dt1.Rows[i]["REPORT_FIL_NAME"].ToString().ToUpper().Trim();
                            ReportFieldName = Regex.Replace(ReportFieldName, @"\s+", "");

                            if (ReportFieldName.Trim() == "" && dt1.Rows[i]["REPORT_FIL_NAME"].ToString().ToUpper().Trim() == "" && dt1.Rows[i]["REPORT_FIL_FIELD"].ToString().ToUpper().Trim() != "")
                            {
                                ReportFieldName = dt1.Rows[i]["REPORT_FIL_FIELD"].ToString().ToUpper().Trim();
                            }
                            string ReportFieldType = dt1.Rows[i]["REPORT_FIL_TYPE"].ToString().ToUpper().Trim();

                            if (ReportFieldType == "ALPHA")
                            {
                               
                                Comboname = "DD" + ReportFieldName;
                                DropDownList ddl = (DropDownList)ddlPanel.FindControl(Comboname);
                                string Combvalue = ddl.SelectedValue.ToString();


                                if (Combvalue != "")
                                {
                                  
                                    string TextboxName = "TextBox" + ReportFieldName + "2";


                                    if (((TextBox)ddlPanel.FindControl(TextboxName)).Text == "")
                                    {

                                  
                                        message = message + " " + "Enter data in " + ReportFieldName + " Field , ";
                               

                                    }

                                }
                            }
                            if (ReportFieldType == "DATE")
                            {

                                         string Testboxname1 = "";
                                            string Testboxname2 = "";
               
                           
                                          Testboxname1 = "TextBox" + ReportFieldName + "1";
                                         Testboxname2 = "TextBox" + ReportFieldName + "2";
                                     string TestboxValue1 = ((TextBox)ddlPanel.FindControl(Testboxname1)).Text;

                                  string TestboxValue2 = ((TextBox)ddlPanel.FindControl(Testboxname2)).Text;
    
                                    if (TestboxValue1 != "")
                                    {
                                            TestboxValue1 = GetstringDate(TestboxValue1);
                                    }
                                    if (TestboxValue2 != "")
                                    {
                                            TestboxValue2 = GetstringDate(TestboxValue2);
                                    }


                                    if (TestboxValue1 != "" && TestboxValue2 != "")
                                    {
                                        if (Int32.Parse(TestboxValue1) > Int32.Parse(TestboxValue2) && !message.Contains("From date Can not be less than To date"))
                                        {
                                            message = message + " " + "From date Can not be less than To date , ";
                                        }
                                    }
                            }



                        if (ReportFieldType == "NUMBER")
                        {

                          
                            Comboname = "DD" + ReportFieldName;
                            DropDownList ddl1 = (DropDownList)ddlPanel.FindControl(Comboname);
                            string Combvalue1 = ddl1.SelectedValue.ToString();

                            if (Combvalue1 != "")
                            {
                              

                                string TextboxName = "TextBox" + ReportFieldName + "2";
                                string TextboxNameValue =((TextBox)ddlPanel.FindControl(TextboxName)).Text;
                                string TextboxNameBet = "";
                                string TextboxNameBetValue = "";

                                if (Combvalue1.ToUpper() == "BETWEEN")
                                {
                                    TextboxNameBet = "TextBox" + ReportFieldName + "BET";
                                    TextboxNameBetValue= ((TextBox)ddlPanel.FindControl(TextboxNameBet)).Text;
                                }

                                if (Combvalue1.ToUpper() != "BETWEEN")
                                {
                                    if (TextboxNameValue == "")
                                    {
                                      
                                            message = message + "  " + "Enter data in " + ReportFieldName + " Field , ";
                                        
                                    }
                                }
                                else if (Combvalue1.ToUpper() == "BETWEEN")
                                {
                                    if (TextboxNameBetValue == "" && TextboxNameValue == "")
                                    {
                                      
                                            message = message + "  " + "Enter data in From and To " + ReportFieldName + " Field , ";
                        
                                        
                                    }

                                    if (TextboxNameValue=="" && TextboxNameBetValue != "")
                                    {
                                       
                                            message = message + "  " + "Enter data in From  " + ReportFieldName + " Field , ";
                                      

                                    }

                                    if (TextboxNameValue != "" && TextboxNameBetValue == "")
                                    {
                                       
                                            message = message + "  " + "Enter data in To  " + ReportFieldName + " Field ,";
                                      
                                    }

                                    if (TextboxNameValue != "" && TextboxNameBetValue != "")
                                    {
                                        if (Int32.Parse(TextboxNameValue) > Int32.Parse(TextboxNameBetValue))
                                        {
                                            if (!message.Contains("In Between Condition From Field Can not be greater than To Field"))
                                            {
                                                message = message + "  " + "In Between Condition From Field Can not be greater than To Field , ";
                                            }
                                            
                                        }
                                    }

                                }
                            }
                        }
                    }

                }
                    conn.Close();
                }
            return message;
        }

        private void DisableControls(System.Web.UI.Control control)
        {
            foreach (System.Web.UI.Control c in control.Controls)
            {
                // Get the Enabled property by reflection.
                Type type = c.GetType();
                PropertyInfo prop = type.GetProperty("Enabled");

                // Set it to False to disable the control.
                if (prop != null)
                {
                    prop.SetValue(c, false, null);
                }

                // Recurse into child controls.
                if (c.Controls.Count > 0)
                {
                    this.DisableControls(c);
                }
            }
        }

    }

    public static class ClientMessageBox
    {

        public static void Show(string message, Control owner)
        {
            Page page = (owner as Page) ?? owner.Page;
            if (page == null) return;

            page.ClientScript.RegisterStartupScript(owner.GetType(),
                "ShowMessage", string.Format("<script type='text/javascript'>alert('{0}')</script>",
                message));

        }

    }
}