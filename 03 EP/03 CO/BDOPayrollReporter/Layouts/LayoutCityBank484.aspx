<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LayoutCityBank484.aspx.cs"
    Inherits="BDOPayrollReporter.Layouts.LayoutCityBank484" MasterPageFile="~/MasterPages/Default.Master"
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
                Layout de CityBank 484</h2>
            <table width="100%">
                <tr>
                    <td colspan="2">
                        Layout para subir a CITIBANK
                    </td>
                </tr>
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
                        Fecha de Dispersión:
                    </td>
                    <td>
                        <asp:TextBox CssClass="date" ID="TextBoxFechaDispesion" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        Ultimo Folio:
                    </td>
                    <td>
                        <asp:TextBox CssClass="numeric" ID="TextoConsecutivo" runat="server" MaxLength="8"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:CheckBox ID="chkEsPrueba" runat="server" Text="Prueba" TextAlign="Left" />
                    </td>
                    <td>
                        &nbsp;
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