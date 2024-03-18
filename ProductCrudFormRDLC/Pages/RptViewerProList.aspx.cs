using Microsoft.Reporting.WebForms;
using ProductCrudFormRDLC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ProductCrudFormRDLC.Pages
{
    public partial class RptViewerProList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindReport();
            }
        }

        protected void BindReport()
        {
            try
            {
                if (Session["PrintData"] != null)
                {
                    List<Product> products = (List<Product>)Session["PrintData"];
                    List<ProductDto> productDtos = products.Select(p => new ProductDto
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Category = p.Category.Name, 
                        Description = p.Description,
                        Price = p.Price,
                        Created = p.Created
                    }).ToList();

                    rvProList.LocalReport.EnableExternalImages = true;
                    rvProList.LocalReport.ReportPath = Server.MapPath("~/Reports/RptProList.rdlc");
                    rvProList.LocalReport.DataSources.Clear();
                    ReportDataSource source = new ReportDataSource("ProDataSet", productDtos);
                    rvProList.LocalReport.DataSources.Add(source);
                    rvProList.LocalReport.SetParameters(new ReportParameter("footer", "Print Time: " + DateTime.Now.ToString("dd.MM.yyyy hh:mm:ss tt")));

                    string imageUrl = new Uri(Server.MapPath("~/Images/pcfrdlc.png")).AbsoluteUri;
                    rvProList.LocalReport.SetParameters(new ReportParameter("logoUrl", imageUrl));
                    rvProList.DataBind();
                    rvProList.LocalReport.Refresh();

                    Warning[] warnings;
                    string[] streamIds;
                    string mimeType, encoding, extension;
                    byte[] bytes = rvProList.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);

                    Response.Clear();
                    Response.Buffer = true;
                    Response.ContentType = mimeType;
                    Response.AddHeader("content-disposition", $"attachment; filename=RptProductList_{DateTime.Now.ToString("yyyyMMddHHmmss")}.pdf");
                    Response.BinaryWrite(bytes);
                    Response.Flush();
                    Response.End();
                    Session.Clear();
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "Notifications", "Toast('Error', '" + ex.Message + "', 'error');", true);
            }
        }
    }
}