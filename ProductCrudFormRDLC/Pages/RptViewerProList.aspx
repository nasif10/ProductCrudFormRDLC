<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RptViewerProList.aspx.cs" Inherits="ProductCrudFormRDLC.Pages.RptViewerProList" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>RptViewerProList</title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager runat="server"></asp:ScriptManager>
        <div>
            <rsweb:ReportViewer ID="rvProList" runat="server" Width="100%">
            </rsweb:ReportViewer>
        </div>
    </form>
</body>
</html>
