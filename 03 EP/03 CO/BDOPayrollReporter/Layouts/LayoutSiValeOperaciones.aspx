<%@ Page Language="C#" MasterPageFile="~/MasterPages/Default.Master" AutoEventWireup="true"
    CodeBehind="LayoutSiValeOperaciones.aspx.cs" Inherits="BDOPayrollReporter.Layouts.LayoutSiValeOperaciones"
    Theme="default" Title="BDO México" %>

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
                Layout de SíVale</h2>
            <table width="100%">
                <tr>
                    <td colspan="2">
                        Se puden realizar layouts de Emisión y Recarga
                        <br />
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
                        Operación
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlOperacion" runat="server">
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="rfvOperacion" runat="server" ControlToValidate="ddlOperacion"
                            Display="Dynamic" ErrorMessage="Seleccione una Opción" InitialValue="Seleccione"></asp:RequiredFieldValidator>
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