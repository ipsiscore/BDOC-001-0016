using System;
using BDOPayrollReporter.Business;
using BDOPayrollReporter.Business.Enums;

namespace BDOPayrollReporter.Layouts
{
    public partial class LayoutBanorteAlta : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                try
                {
                    Utilities.ObtenerAñosPorMovimiento(this.DropDownListAño);
                    Utilities.ObtenerMesesEnAñosPorMovimiento(this.DropDownListAño, this.DropDownListPeriodo);
                }
                catch (Exception ex)
                {
                    this.LabelMensaje.Text = "Error: " + ex.Message;
                }
            }
        }

        protected void DropDownListAño_SelectedIndexChanged(object sender, EventArgs e)
        {
            Utilities.ObtenerMesesEnAñosPorMovimiento(this.DropDownListAño, this.DropDownListPeriodo);
        }

        protected void ButtonGenerar_Click(object sender, EventArgs e)
        {
            this.GenerarReporte();
        }

        private void GenerarReporte()
        {
            int mes, año;
            Byte[] file;
            TextRenderer TextHelper;

            try
            {
                TextHelper = new TextRenderer();

                if (Utilities.IsNumeric(this.DropDownListAño.SelectedValue))
                {
                    if (Utilities.IsNumeric(this.DropDownListPeriodo.SelectedValue))
                    {
                        mes = Int32.Parse(this.DropDownListPeriodo.SelectedValue);
                        año = Int32.Parse(this.DropDownListAño.SelectedValue);

                        if (Utilities.ValidarAño(año))
                        {
                            if (Utilities.ValidarMes(mes))
                            {
                                file = TextHelper.RenderLayoutBanorteAlta(año, mes);

                                if (file.Length > 0)
                                {
                                    this.LabelMensaje.Text = "";
                                    Response.Clear();
                                    string fileName = Utilities.GetName(ReportName.Layoutdealtasdebanorte, año, mes);
                                    Response.AddHeader("content-disposition", "attachment; filename=" + fileName + ".txt");
                                    Response.ContentType = "text/plain";
                                    Response.BinaryWrite(file);
                                    Response.End();
                                }
                                else
                                    this.LabelMensaje.Text = "No hay información de exportación.";
                            }
                            else
                                this.LabelMensaje.Text = "El Mes no es válido";
                        }
                        else
                            this.LabelMensaje.Text = "El Año no es válido";
                    }
                    else
                        this.LabelMensaje.Text = "El periodo seleccionado no es válido.";
                }
                else
                    this.LabelMensaje.Text = "El año seleccionado no es válido.";
            }
            catch (Exception e)
            {
                this.LabelMensaje.Text = "Error: " + e.Message;
            }
        }
    }
}