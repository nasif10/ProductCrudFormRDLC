using Dapper;
using ProductCrudFormRDLC.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ProductCrudFormRDLC.Pages
{
    public partial class ProductsForm : System.Web.UI.Page
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Date1.Text = DateTime.Today.AddDays(-30).ToString("dd-MM-yyyy");
                Date2.Text = DateTime.Today.ToString("dd-MM-yyyy");
                LoadData();
            }
        }

        protected void LoadData()
        {
            try
            {
                string fromDate = DateTime.Parse(Date1.Text).ToString("yyyy-MM-dd");
                string toDate = DateTime.Parse(Date2.Text).ToString("yyyy-MM-dd");
                string search = SrcTxt.Text;

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string query1 = "Select * from Products p INNER JOIN Categories c ON p.CategoryId = c.Id";
                    if (!string.IsNullOrEmpty(fromDate) && !string.IsNullOrEmpty(toDate))
                    {
                        query1 += " WHERE p.Created BETWEEN @Date1 AND @Date2";
                    }
                    if (!string.IsNullOrEmpty(search))
                    {
                        query1 += " AND p.Name LIKE @Search";
                    }

                    var parameters = new
                    {
                        Date1 = fromDate,
                        Date2 = toDate,
                        Search = string.IsNullOrEmpty(search) ? null : "%" + search + "%"
                    };

                    List<Product> products = connection.Query<Product, Category, Product>(query1, (product, category) => {
                        product.Category = category;
                        return product;
                    }, parameters).ToList();

                    ViewState["lProducts"] = products;


                    string query2 = "Select * from Categories";
                    List<Category> categories = connection.Query<Category>(query2).ToList();

                    ViewState["lCategories"] = categories;

                    Data_Bind();
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "Notifications", "Toast('Error', '" + ex.Message + "', 'error');", true);
            }
        }

        private void Data_Bind()
        {
            if (ViewState["lProducts"] != null)
            {
                gvProducts.DataSource = (List<Product>)ViewState["lProducts"];
                gvProducts.DataBind();
            }

            if (ViewState["lCategories"] != null)
            {
                ddlCategory.DataSource = (List<Category>)ViewState["lCategories"];
                ddlCategory.DataBind();
            }
        }

        protected void OkBtn_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string name = tbName.Text;
            string categoryId = ddlCategory.SelectedValue;
            string description = tbDescription.Text;
            string price = tbPrice.Text;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "INSERT INTO Products (Name, CategoryId, Description, Price, Created) VALUES (@Name, @CategoryId, @Description, @Price, @Created);";
                var parameters = new
                {
                    Name = name,
                    CategoryId = categoryId,
                    Description = description,
                    Price = price,
                    Created = DateTime.Now
                };
                int affectedRows = connection.Execute(query, parameters);
                if (affectedRows > 0)
                {
                    List<Product> products = ViewState["lProducts"] as List<Product>;
                    Product newProduct = new Product{ Name = name, CategoryId = Convert.ToInt32(categoryId), Description = description, Price = decimal.Parse(price),Created = DateTime.Now };
                    products.Add(newProduct);
                    ViewState["lProducts"] = products;
                    Data_Bind();

                    ScriptManager.RegisterStartupScript(this, GetType(), "Notification", "Toast('Success','Record created successfully!', 'success');", true);
                    mvProducts.ActiveViewIndex = 0;
                }
                else
                    ScriptManager.RegisterStartupScript(this, GetType(), "Notifications", "Toast('Error', 'Record could not be created!', 'error');", true);

            }
        }

        protected void gvProducts_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvProducts.EditIndex = e.NewEditIndex;
            Data_Bind();
        }

        protected void gvProducts_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    GridViewRow row = gvProducts.Rows[e.RowIndex];
                    int id = Convert.ToInt32(((Label)row.FindControl("lbl_Id")).Text);
                    string name = ((TextBox)row.FindControl("txt_Name")).Text;
                    string categoryId = ((DropDownList)row.FindControl("ddlCategoryId")).Text;
                    string description = ((TextBox)row.FindControl("txt_Description")).Text;
                    string price = ((TextBox)row.FindControl("txt_Price")).Text;

                    string query = "UPDATE Products SET Name = @Name, CategoryId = @CategoryId, Description = @Description, Price = @Price WHERE Id = @Id";
                    var parameters = new { Name = name, CategoryId = categoryId, Description = description, Price = price, Id = id };
                    int affectedRows = connection.Execute(query, parameters);

                    if (affectedRows > 0)
                        ScriptManager.RegisterStartupScript(this, GetType(), "Notification", "Toast('Success','Record updated successfully!', 'success');", true);
                    else
                        ScriptManager.RegisterStartupScript(this, GetType(), "Notifications", "Toast('Error', 'Record not found or could not be updated!', 'error');", true);

                    gvProducts.EditIndex = -1;
                    LoadData();
                }
            }
            catch(Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "Notifications", "Toast('Error', '" + ex.Message + "', 'error');", true);
            }
        }

        protected void gvProducts_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvProducts.EditIndex = -1;
            Response.Redirect(Request.RawUrl);
        }

        protected void gvProducts_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    GridViewRow row = gvProducts.Rows[e.RowIndex];
                    int id = Convert.ToInt32(((Label)row.FindControl("lbl_Id")).Text);
                    string query = "DELETE FROM Products WHERE Id = @Id";
                    int affectedRows = connection.Execute(query, new { Id = id });

                    if (affectedRows > 0)
                        ScriptManager.RegisterStartupScript(this, GetType(), "Notification", "Toast('Success','Record deleted successfully!', 'success');", true);
                    else
                        ScriptManager.RegisterStartupScript(this, GetType(), "Notifications", "Toast('Error', 'Record not found or could not be deleted!', 'error');", true);
                    LoadData();
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "Notifications", "Toast('Error', '" + ex.Message + "', 'error');", true);
            }
        }

        protected void gvProducts_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    if (e.Row.RowType == DataControlRowType.DataRow)
                    {
                        HyperLink hlinkProduct = e.Row.FindControl("hlinkProInfo") as HyperLink;
                        var id = DataBinder.Eval(e.Row.DataItem, "Id").ToString();
                        hlinkProduct.NavigateUrl = "RptViewerProInfo.aspx?Id=" + id;
                    }

                    if (e.Row.RowType == DataControlRowType.DataRow && gvProducts.EditIndex == e.Row.RowIndex)
                    {
                        DropDownList ddlCategoryId = (DropDownList)e.Row.FindControl("ddlCategoryId");
                        if (ddlCategoryId != null)
                        {
                            ddlCategoryId.DataSource = (List<Category>)ViewState["lCategories"];
                            ddlCategoryId.DataBind();

                            ddlCategoryId.SelectedValue = DataBinder.Eval(e.Row.DataItem, "CategoryId").ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "Notifications", "Toast('Error', '" + ex.Message + "', 'error');", true);
            }
        }

        protected void gvProducts_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvProducts.PageIndex = e.NewPageIndex;
            Data_Bind();
        }

        protected void Page_PreInit(object sender, EventArgs e)
        {
            MasterControl();
        }

        protected void MasterControl()
        {
            ((LinkButton)this.Master.FindControl("lnkPrint")).Click += new EventHandler(lnkPrint_Click);
        }

        protected void lnkPrint_Click(object sender, EventArgs e)
        {
            try
            {
                Session["PrintData"] = ViewState["lProducts"];
                Response.Redirect("RptViewerProList.aspx");
            }
            catch(Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "Notifications", "Toast('Warning', '" + ex.Message + "', 'warning');", true);
            }
        }

        protected void hlinkExcel_Click(object sender, EventArgs e)
        {
            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", $"attachment;filename=ProductList_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xls");
            Response.Charset = "";
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            for (int i = 1; i <= 3; i++)
            {
                gvProducts.Columns[gvProducts.Columns.Count - i].Visible = false;
            }
            
            gvProducts.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
        }

        public override void VerifyRenderingInServerForm(Control control)
        {
            
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            mvProducts.ActiveViewIndex = 1;
        }


    }
}