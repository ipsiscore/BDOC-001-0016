<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReporteListadoDeValesDeDespensa.aspx.cs"
    Inherits="BDOPayrollReporter.Reportes.ReporteListadoDeValesDeDespensa" MasterPageFile="~/MasterPages/Default.Master"
    Theme="default" Title="BDO México" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<%@ Register Src="~/wucontrols/Header.ascx" TagName="Header" TagPrefix="uc1" %>
<%@ Register Src="~/wucontrols/Menu.ascx" TagName="Menu" TagPrefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script language="javascript" type="text/javascript">        $().ready(function () { menuSelect('mreports'); });</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder" runat="server">
    <div id="breadcrumbs">
    </div>
    <div id="sections">
        <uc2:Menu ID="menu" runat="server" />
    </div>
    <div id="righthalf">
        <div id="content" style="height: auto; width: auto; background-color: #FFFFFF;">
            <h2>
                Listado de Vales de Despensa</h2>
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
                        Tipo de nómina:
                    </td>
                    <td>
                        <asp:DropDownList ID="DropDownListTipoNomina" runat="server">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td>
                        Estado de nómina:
                    </td>
                    <td>
                        <asp:DropDownList ID="DropDownListEstadoNomina" runat="server">
                            <asp:ListItem Value="1">Abierta</asp:ListItem>
                            <asp:ListItem Value="0">Cerrada</asp:ListItem>
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
            <rsweb:ReportViewer ID="ReportViewerReporte" runat="server" ProcessingMode="Local"
                Width="755px" Height="800px">
            </rsweb:ReportViewer>
        </div>
    </div>
</asp:Content>