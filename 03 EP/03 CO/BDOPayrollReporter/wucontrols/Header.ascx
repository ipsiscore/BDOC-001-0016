<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Header.ascx.cs" Inherits="BDOPayrollReporter.Header" %>
<script src="../scripts/jquery-1.4.1.min.js" type="text/javascript"></script>
<script src="../scripts/jquery.maskedinput-1.3.min.js" type="text/javascript"></script>
<script src="../scripts/jquery.functions.js" type="text/javascript"></script>
<div id="header">
    <a href='<%# ResolveUrl("~/")%>'>
        <asp:Image ID="imgLogo" runat="server" ImageUrl="~/images/logo.png" />
    </a>
</div>