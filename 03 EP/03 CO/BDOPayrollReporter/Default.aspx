<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="BDOPayrollReporter._Default"
    MasterPageFile="~/MasterPages/Default.Master" Theme="default" Title="BDO México" %>

<%@ Register Src="~/wucontrols/Header.ascx" TagName="Header" TagPrefix="uc1" %>
<%@ Register Src="~/wucontrols/Menu.ascx" TagName="Menu" TagPrefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder" runat="server">
    <div id="breadcrumbs">
    </div>
    <div id="sections">
        <uc2:Menu ID="menu" runat="server" />
    </div>
    <div id="righthalf">
        <div id="content">
            <h2>
                Hospira</h2>
            <asp:Image ID="imgHospira" runat="server" ImageUrl="~/App_Themes/default/images/Hospira.jpg"
                Width="500px" />
        </div>
    </div>
</asp:Content>