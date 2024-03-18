using Dapper;
using Microsoft.Reporting.WebForms;
using ProductCrudFormRDLC.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ProductCrudFormRDLC.Pages
{
    public partial class RptViewerProInfo : System.Web.UI.Page
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindReport();
            }
        }

        protected void BindReport()
        {
            if (Request.QueryString["Id"] != null)
            {
                string productId = Request.QueryString["Id"];
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query1 = "Select * from Products p INNER JOIN Categories c ON p.CategoryId = c.Id WHERE p.Id = @Id";
                    Product product = connection.Query<Product, Category, Product>(query1, (p, c) => {
                        p.Category = c;
                        return p;
                    }, new { Id = productId }, splitOn: "Id").FirstOrDefault();

                    rvProInfo.LocalReport.EnableExternalImages = true;
                    rvProInfo.LocalReport.ReportPath = Server.MapPath("~/Reports/RptProInfo.rdlc");

                    rvProInfo.LocalReport.SetParameters(new ReportParameter("name", product.Name));
                    rvProInfo.LocalReport.SetParameters(new ReportParameter("category", product.Category.Name));
                    rvProInfo.LocalReport.SetParameters(new ReportParameter("description", product.Description));
                    rvProInfo.LocalReport.SetParameters(new ReportParameter("price", product.Price.ToString()));
                    rvProInfo.LocalReport.SetParameters(new ReportParameter("footer", "Print Time: " + DateTime.Now.ToString("dd.MM.yyyy hh:mm:ss tt")));
                    
                    
                    string imageUrl = new Uri(Server.MapPath("~/Images/pcfrdlc.png")).AbsoluteUri;
                    rvProInfo.LocalReport.SetParameters(new ReportParameter("logoUrl", imageUrl));
                    rvProInfo.DataBind();
                    rvProInfo.LocalReport.Refresh();

                    Warning[] warnings;
                    string[] streamIds;
                    string mimeType, encoding, extension;
                    byte[] bytes = rvProInfo.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);

                    Response.Clear();
                    Response.Buffer = true;
                    Response.ContentType = mimeType;
                    Response.AddHeader("content-disposition", $"attachment; filename=RptProductInfo_{productId}_{DateTime.Now.ToString("yyyyMMddHHmmss")}.pdf");
                    Response.BinaryWrite(bytes);
                    Response.Flush();
                    Response.End();
                }
            }
            else
                ScriptManager.RegisterStartupScript(this, GetType(), "Notifications", "Toast('Error', 'Record not found!', 'error');", true);
        }
    }
}