﻿using System;
using System.Collections.Generic;
using System.Data;
using BDOPayrollReporter.Business;
using BDOPayrollReporter.Business.Enums;
using Microsoft.Reporting.WebForms;

namespace BDOPayrollReporter.Reportes
{
    public partial class ReporteListadoDeValesDeDespensa : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                Utilities.ObtenerAñosPorMovimiento(this.DropDownListAño);
                Utilities.ObtenerMesesEnAñosPorMovimiento(this.DropDownListAño, this.DropDownListPeriodo);
                Utilities.ObtenerTiposNomina(this.DropDownListTipoNomina);
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
            DataTable datos;
            List<ReportParameter> parametros;
            DataAccess SqlHelper;

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
                        datos = SqlHelper.ReporteListadoDeValesDeDespensa(año, mes, Int16.Parse(this.DropDownListTipoNomina.SelectedValue), !DropDownListEstadoNomina.SelectedValue.Equals("0") ? true : false);

                        if (datos.Rows.Count > 0)
                        {
                            parametros.Add(new ReportParameter("p_Mes", mes.ToString()));
                            parametros.Add(new ReportParameter("p_Año", año.ToString()));

                            ReportViewerReporte.ProcessingMode = ProcessingMode.Local;
                            ReportViewerReporte.LocalReport.DisplayName = Utilities.GetName(ReportName.Listadodevalesdedespensa, año, mes);
                            ReportViewerReporte.LocalReport.ReportPath = Request.PhysicalApplicationPath.ToString() + "Reportes\\rdlc\\ReportListadoDeValesDeDespensa.rdlc";
                            ReportViewerReporte.LocalReport.SetParameters(parametros);
                            ReportViewerReporte.LocalReport.DataSources.Clear();
                            ReportViewerReporte.LocalReport.DataSources.Add(new ReportDataSource("dtListadoDeValesDeDespensa", datos));
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