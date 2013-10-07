<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LayoutSkandiaAlta.aspx.cs"
    Inherits="BDOPayrollReporter.Layouts.LayoutSkandiaAlta" MasterPageFile="~/MasterPages/Default.Master"
    Theme="default" Title="BDO México" %>

<%@ Register Src="~/wucontrols/Header.ascx" TagName="Header" TagPrefix="uc1" %>
<%@ Register Src="~/wucontrols/Menu.ascx" TagName="Menu" TagPrefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script language="javascript" type="text/javascript">        $().ready(function () { menuSelect('mlayouts'); });</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder" runat="server">
    <div id="breadcrumbs">
    </div>
    <div id="sections">
        <uc2:Menu ID="menu1" runat="server" />
    </div>
    <div id="righthalf">
        <div id="content" style="height: auto; width: auto; background-color: #FFFFFF;">
            <h2>
                Layout de altas Skandia</h2>
            <table width="100%">
                <tr>
                    <td>
                        Año:
                    </td>
                    <td>
                        <asp:DropDownList ID="DropDownListAño" runat="server" AutoPostBack="true" OnSelectedIndexChanged="DropDownListAño_SelectedIndexChanged">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td>
                        Periodo:
                    </td>
                    <td>
                        <asp:DropDownList ID="DropDownListPeriodo" runat="server">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td>
                    </td>
                    <td>
                        <asp:Button ID="ButtonGenerar" CssClass="operation" runat="server" Text="Generar reporte"
                            OnClick="ButtonGenerar_Click" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:Label ID="LabelMensaje" runat="server" Text=""></asp:Label>
                    </td>
                </tr>
            </table>
        </div>
    </div>
</asp:Content>