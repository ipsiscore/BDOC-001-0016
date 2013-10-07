using System;
using System.Collections.Generic;
using System.Data;
using BDOPayrollReporter.Business;
using BDOPayrollReporter.Business.Enums;
using Microsoft.Reporting.WebForms;

namespace BDOPayrollReporter.Reportes
{
    public partial class WebFormListadoNomina : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                try
                {
                    Utilities.ObtenerAñosPorMovimiento(this.DropDownListAño);
                    Utilities.ObtenerMesesEnAñosPorMovimiento(this.DropDownListAño, this.DropDownListPeriodo);
                    Utilities.ObtenerTiposNomina(this.DropDownListTipoNomina);
                    Utilities.ObtenerEmpleados(this.ListBoxEmpleados);
                }
                catch (Exception ex)
                {
                    this.LabelMensaje.Text = "Error: " + ex.Message;
                }
            }
        }

        protected void DropDownListAño_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                Utilities.ObtenerMesesEnAñosPorMovimiento(this.DropDownListAño, this.DropDownListPeriodo);
            }
            catch (Exception ex)
            {
                this.LabelMensaje.Text = "Error: " + ex.Message;
            }
        }

        protected void ButtonGenerar_Click(object sender, EventArgs e)
        {
            this.GenerarReporte();
            this.ListBoxEmpleados.ClearSelection();
        }

        private void GenerarReporte()
        {
            int mes, año;
            bool prenomina = false;
            DataTable datos;
            List<ReportParameter> parametros;
            DataAccess SqlHelper;
            List<string> empleadoList = new List<string>();

            try
            {
                parametros = new List<ReportParameter>();
                SqlHelper = new DataAccess();

                if (Utilities.IsNumeric(this.DropDownListAño.SelectedValue))
                {
                    if (Utilities.IsNumeric(this.DropDownListPeriodo.SelectedValue))
                    {
                        mes = Int32.Parse(this.DropDownListPeriodo.SelectedValue);
                        año = Int32.Parse(this.DropDownListAño.SelectedValue);
                        prenomina = ChkPrenomina.Checked;
                        empleadoList = Utilities.ObtenerListEmpleados(this.ListBoxEmpleados);
                        datos = SqlHelper.ReporteListadoNomina(año, mes, Int16.Parse(this.DropDownListTipoNomina.SelectedValue), empleadoList);

                        if (datos.Rows.Count > 0)
                        {
                            parametros.Add(new ReportParameter("p_Mes", mes.ToString()));
                            parametros.Add(new ReportParameter("p_Año", año.ToString()));
                            parametros.Add(new ReportParameter("EsPrenomina", prenomina ? "True" : "False"));

                            ReportViewerReporte.ProcessingMode = ProcessingMode.Local;
                            string pre = prenomina ? "prenom" : "nom";
                            ReportViewerReporte.LocalReport.DisplayName = Utilities.GetName(ReportName.Listadodenomina, año, mes).Replace("nom", pre);
                            ReportViewerReporte.LocalReport.ReportPath = Request.PhysicalApplicationPath.ToString() + "Reportes\\rdlc\\ReportListadoDeNomina.rdlc";
                            ReportViewerReporte.LocalReport.SetParameters(parametros);
                            ReportViewerReporte.LocalReport.DataSources.Clear();
                            ReportViewerReporte.LocalReport.DataSources.Add(new ReportDataSource("dtListadoNomina", datos));
                            ReportViewerReporte.Visible = true;
                            this.LabelMensaje.Text = String.Empty;
                        }
                        else
                        {
                            this.LabelMensaje.Text = "No hay información de exportación.";
                            ReportViewerReporte.Visible = false;
                        }
                    }
                    else
                    {
                        this.LabelMensaje.Text = "El periodo seleccionado no es válido.";
                        ReportViewerReporte.Visible = false;
                    }
                }
                else
                {
                    this.LabelMensaje.Text = "El año seleccionado no es válido.";
                    ReportViewerReporte.Visible = false;
                }
            }
            catch (Exception e)
            {
                this.LabelMensaje.Text = "Error: " + e.Message;
            }
        }
    }
}