<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LayoutTepca.aspx.cs" Inherits="BDOPayrollReporter.Layouts.LayoutTepca"
    MasterPageFile="~/MasterPages/Default.Master" Theme="default" Title="BDO México" %>

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
                Layouts para Tebca</h2>
            <table width="100%">
                <tr>
                    <td colspan="2">
                        Se puden realizar layouts de Emisión y Recarga
                        <br />
                        Nota: para emisión se tendran que hacer 3 archivos (Despensa, Comida y Restaurante)
                    </td>
                </tr>
                <tr>
                    <td>
                        Año:
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlAno" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlAno_SelectedIndexChanged">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td>
                        Periodo:
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlPeriodo" runat="server">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td>
                        Tipo de Nómina
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlTipoNomina" runat="server">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td>
                        Operación
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlOperacion" runat="server">
                            <asp:ListItem Value="Seleccione">Seleccione</asp:ListItem>
                            <asp:ListItem Value="1">Emisión</asp:ListItem>
                            <asp:ListItem Value="32">Recarga Vales Despensa</asp:ListItem>
                            <asp:ListItem Value="98">Recarga Vales Comida</asp:ListItem>
                            <asp:ListItem Value="97">Recarga Vales Restaurant</asp:ListItem>
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="rfvOperacion" runat="server" ControlToValidate="ddlOperacion"
                            Display="Dynamic" ErrorMessage="Seleccione una Opción" InitialValue="Seleccione"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td>
                        Estado de nómina:
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlEstadoNomina" runat="server">
                            <asp:ListItem Value="1">Abierta</asp:ListItem>
                            <asp:ListItem Value="0">Cerrada</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:CheckBox ID="chkEsPrueba" runat="server" Text="Prueba" TextAlign="Left" />
                    </td>
                    <td>
                    </td>
                </tr>
                <tr>
                    <td>
                    </td>
                    <td>
                        <asp:Button ID="btnGenerar" CssClass="operation" runat="server" Text="Generar reporte"
                            OnClick="btnGenerar_Click" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:Label ID="lblMensaje" runat="server" Text=""></asp:Label>
                    </td>
                </tr>
            </table>
        </div>
    </div>
</asp:Content>