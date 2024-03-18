using Dapper;
using ProductCrudFormRDLC.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ProductCrudFormRDLC.Pages
{
    public partial class CategoriesForm : System.Web.UI.Page
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
        }

        protected void LoadData()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query1 = "Select * from Categories";
                    List<Category> categories = connection.Query<Category>(query1).ToList();
                    if (categories.Count > 0)
                    {
                        ViewState["dtCategories"] = categories;
                    }
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
            if (ViewState["dtCategories"] != null)
            {
                gvCategories.DataSource = (List<Category>)ViewState["dtCategories"];
                gvCategories.DataBind();
            }
        }

        protected void gvCategories_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvCategories.EditIndex = e.NewEditIndex;
            Data_Bind();
        }

        protected void gvCategories_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    GridViewRow row = gvCategories.Rows[e.RowIndex];
                    int id = Convert.ToInt32(((Label)row.FindControl("lbl_Id")).Text);
                    string name = ((TextBox)row.FindControl("txt_Name")).Text;

                    string query = "UPDATE Categories SET Name = @Name WHERE Id = @Id";
                    var parameters = new { Name = name, Id = id };
                    int affectedRows = connection.Execute(query, parameters);

                    if (affectedRows > 0)
                        ScriptManager.RegisterStartupScript(this, GetType(), "Notification", "Toast('Success','Record updated successfully!', 'success');", true);
                    else
                        ScriptManager.RegisterStartupScript(this, GetType(), "Notifications", "Toast('Error', 'Record not found or could not be updated!', 'error');", true);

                    gvCategories.EditIndex = -1;
                    LoadData();
                }
            }
            catch(Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "Notifications", "Toast('Error', '" + ex.Message + "', 'error');", true);
            }
        }

        protected void gvCategories_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvCategories.EditIndex = -1;
            Response.Redirect(Request.RawUrl);
        }

        protected void gvCategories_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    GridViewRow row = gvCategories.Rows[e.RowIndex];
                    int id = Convert.ToInt32(((Label)row.FindControl("lbl_Id")).Text);
                    string query = "DELETE FROM Categories WHERE Id = @Id";
                    int affectedRows = connection.Execute(query, new { Id = id });

                    if (affectedRows > 0)
                        ScriptManager.RegisterStartupScript(this, GetType(), "Notification", "Toast('Success','Record deleted successfully!', 'success');", true);
                    else
                        ScriptManager.RegisterStartupScript(this, GetType(), "Notifications", "Toast('Error', 'Record not found or could not be deleted!', 'error');", true);
                    LoadData();
                }
            }
            catch(Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "Notifications", "Toast('Error', '" + ex.Message + "', 'error');", true);
            }
        }

        protected void gvCategories_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvCategories.PageIndex = e.NewPageIndex;
            Data_Bind();
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            mvCategories.ActiveViewIndex = 1;
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                var name = txtName.Text;

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO Categories (Name) VALUES (@Name);";
                    var parameters = new
                    {
                        Name = name,
                    };
                    int affectedRows = connection.Execute(query, parameters);
                    if (affectedRows > 0)
                    {
                        LoadData();
                        ScriptManager.RegisterStartupScript(this, GetType(), "Notification", "Toast('Success','Record created successfully!', 'success');", true);
                        mvCategories.ActiveViewIndex = 0;
                    }
                    else
                        ScriptManager.RegisterStartupScript(this, GetType(), "Notifications", "Toast('Error', 'Record could not be created!', 'error');", true);
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "Notifications", "Toast('Error', '" + ex.Message + "', 'error');", true);
            }
        }


    }
}