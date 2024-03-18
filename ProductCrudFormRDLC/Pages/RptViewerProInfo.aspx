<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RptViewerProInfo.aspx.cs" Inherits="ProductCrudFormRDLC.Pages.RptViewerProInfo" %>

<%@ Register assembly="Microsoft.ReportViewer.WebForms" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>RptViewerProInfo</title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager runat="server"></asp:ScriptManager>
        <div>
            <rsweb:ReportViewer ID="rvProInfo" runat="server" Width="100%">
            </rsweb:ReportViewer>
        </div>
    </form>
</body>
</html>
