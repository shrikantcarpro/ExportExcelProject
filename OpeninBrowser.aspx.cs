using ExportToExcel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ExportExcelProject
{
    public partial class OpeninBrowser : System.Web.UI.Page
    {
        string ExportId = "";
        int RequestId = 0;
        string FullWhereStr = "";
        
        protected void Page_Load(object sender, EventArgs e)
        {

            ExportId = Request.QueryString["Id"];
            RequestId = Int32.Parse(Request.QueryString["ReportID"]);
           
            if (Session["WhereCondition"] != null)
            {
                FullWhereStr = (string)(Session["WhereCondition"]).ToString().Trim();
            }

            Export();
            Session.Clear();

        }

        protected void Export()
        {
            string fname = "";
            try
            {
                Label1.Text = "Processing please wait..";

                string strConnString = ConfigurationManager.ConnectionStrings["ReportConnectionString"].ConnectionString;
                //      DataTable dt1 = new DataTable();
                using (SqlConnection con = new SqlConnection(strConnString))
                {
                    var querystr = "";


                    if (ExportId.Trim() == "Export")
                    {

                        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ReportConnectionString"].ToString()))
                        {

                            string query = "select rptno,rptname,rptquery from cmb_rpt_query";
                            if (RequestId > 0)
                            {
                                query = query + " where rptno=" + RequestId;
                            }
                            SqlDataAdapter da = new SqlDataAdapter(query, conn);
                            conn.Open();
                            DataSet ds = new DataSet();

                            da.Fill(ds, "cmb_rpt_query13");

                            DataTable dt = ds.Tables["cmb_rpt_query13"];


                            if (ds.Tables[0].Rows.Count > 0)

                            {

                                querystr = dt.Rows[0]["rptquery"].ToString();
                                fname = dt.Rows[0]["rptname"].ToString();

                            }

                            conn.Close();

                        }

                    }

                    if (querystr != "")
                    {
                        if (FullWhereStr != "")
                        {
                            querystr = querystr +" "+ FullWhereStr;
                        }
                        using (SqlCommand cmd = new SqlCommand(querystr))
                        //using (SqlCommand cmd = new SqlCommand("select i.invoice_no,i.invoice_type, convert(date,i.invoice_date,112) as invoice_date,case when i.inv_status = 0 then 'cancelled' else 'normal' end as invoice_status,i.invoice_amount, i.AMOUNT_PID,i.balance,d.DEBITOR_NAME from invoices i(nolock) left outer join debitors d(nolock) on i.DEBITOR_NO = d.DEBITOR_CODE"))
                        {
                            using (SqlDataAdapter sda = new SqlDataAdapter())
                            {

                                cmd.Connection = con;
                                sda.SelectCommand = cmd;
                                using (DataTable dt1 = new DataTable())
                                {

                                   

                                        fname = Regex.Replace(fname, @"\s", "");
                                        sda.Fill(dt1);
                                    if (ConfigurationManager.AppSettings["ExportRec"] == ""|| dt1.Rows.Count < Int32.Parse(ConfigurationManager.AppSettings["ExportRec"]))
                                    {
                                        string excelFilename = fname + ".xlsx";// "d:\\DotNetTest\\Excel\\Sample.xlsx";
                                                                               //   CreateExcelFile.CreateExcelDocument(ds, excelFilename);

                                        var ret = CreateExcelFile.CreateExcelDocument(dt1, excelFilename, Response);
                                        if (ret == false)
                                        {
                                            throw new Exception("Couldn't create Excel file Please try again");
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception("Export record count shold not be more than " + ConfigurationManager.AppSettings["ExportRec"]);

                                    }
                                   
                                }
                            }
                        }
                    }
                }
                


            }
            catch (Exception ex)
            {
                Label1.Text = ex.Message;
                return;
            }
        }




    }
}