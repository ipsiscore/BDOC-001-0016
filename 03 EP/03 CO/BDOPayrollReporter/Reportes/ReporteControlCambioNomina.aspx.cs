using System;
using System.Collections.Generic;
using System.Data;
using BDOPayrollReporter.Business;
using BDOPayrollReporter.Business.Enums;
using Microsoft.Reporting.WebForms;

namespace BDOPayrollReporter.Reportes
{
    public partial class ReporteControlCambioNomina : System.Web.UI.Page
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
            DataTable ControlCambioNomina_Altas;
            DataTable ControlCambioNomina_Bajas;
            DataTable ControlCambioNomina_OtrasPercepciones;
            DataTable ControlCambioNomina_OtrasDeducciones;
            DataTable ControlCambioNomina_CambiosCuenta;
            DataTable ControlCambioNomina_OtrosCambios;
            List<ReportParameter> parametros;
            DataAccess SqlHelper;

            try
            {
                parametros = new List<ReportParameter>();
                SqlHelper = new DataAccess();

                parametros = new List<ReportParameter>();
                SqlHelper = new DataAccess();

                if (Utilities.IsNumeric(this.DropDownListAño.SelectedValue))
                {
                    if (Utilities.IsNumeric(this.DropDownListPeriodo.SelectedValue))
                    {
                        mes = Int32.Parse(this.DropDownListPeriodo.SelectedValue);
                        año = Int32.Parse(this.DropDownListAño.SelectedValue);
                        ControlCambioNomina_Altas = SqlHelper.ReporteControlCambioNomina_Altas(año, mes);
                        ControlCambioNomina_Bajas = SqlHelper.ReporteControlCambioNomina_Bajas(año, mes);
                        ControlCambioNomina_OtrasPercepciones = SqlHelper.ReporteControlCambioNomina_OtrasPercepciones(año, mes);
                        ControlCambioNomina_OtrasDeducciones = SqlHelper.ReporteControlCambioNomina_OtrasDeducciones(año, mes);
                        ControlCambioNomina_CambiosCuenta = SqlHelper.ReporteControlCambioNomina_CambiosCuenta(año, mes);
                        ControlCambioNomina_OtrosCambios = SqlHelper.ReporteControlCambioNomina_OtrosCambios(año, mes);

                        if (ControlCambioNomina_Bajas.Rows.Count > 0 || ControlCambioNomina_OtrasPercepciones.Rows.Count > 0 || ControlCambioNomina_OtrasDeducciones.Rows.Count > 0 || ControlCambioNomina_CambiosCuenta.Rows.Count > 0)
                        {
                            parametros.Add(new ReportParameter("p_Mes", mes.ToString()));
                            parametros.Add(new ReportParameter("p_Año", año.ToString()));

                            ReportViewerReporte.ProcessingMode = ProcessingMode.Local;
                            ReportViewerReporte.LocalReport.DisplayName = Utilities.GetName(ReportName.Reportedecontroldecambioennomina, año, mes);
                            ReportViewerReporte.LocalReport.ReportPath = Request.PhysicalApplicationPath.ToString() + "Reportes\\rdlc\\ReportControlCambioNomina.rdlc";
                            ReportViewerReporte.LocalReport.SetParameters(parametros);
                            ReportViewerReporte.LocalReport.DataSources.Clear();
                            ReportViewerReporte.LocalReport.DataSources.Add(new ReportDataSource("dtControlCambioNomina_Altas", ControlCambioNomina_Altas));
                            ReportViewerReporte.LocalReport.DataSources.Add(new ReportDataSource("dtControlCambioNomina_Bajas", ControlCambioNomina_Bajas));
                            ReportViewerReporte.LocalReport.DataSources.Add(new ReportDataSource("dtControlCambioNomina_OtrasPercepciones", ControlCambioNomina_OtrasPercepciones));
                            ReportViewerReporte.LocalReport.DataSources.Add(new ReportDataSource("dtControlCambioNomina_OtrasDeducciones", ControlCambioNomina_OtrasDeducciones));
                            ReportViewerReporte.LocalReport.DataSources.Add(new ReportDataSource("dtControlCambioNomina_CambiosCuenta", ControlCambioNomina_CambiosCuenta));
                            ReportViewerReporte.LocalReport.DataSources.Add(new ReportDataSource("dtControlCambioNomina_OtrosCambios", ControlCambioNomina_OtrosCambios));
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