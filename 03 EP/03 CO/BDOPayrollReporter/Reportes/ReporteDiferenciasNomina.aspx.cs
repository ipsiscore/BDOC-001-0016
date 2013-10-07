using System;
using System.Collections.Generic;
using System.Data;
using BDOPayrollReporter.Business;
using BDOPayrollReporter.Business.Enums;
using Microsoft.Reporting.WebForms;

namespace BDOPayrollReporter.Reportes
{
    public partial class ReporteDiferenciasNomina : System.Web.UI.Page
    {
        #region Events

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
        }

        #endregion Events

        #region Methods

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
                        datos = SqlHelper.ReporteDiferenciasNomina(año, mes, 1);

                        if (datos.Rows.Count > 0)
                        {
                            string periodo = string.Format("{0} {1}", Utilities.GetMonthTitle(mes).ToUpper(), año);
                            parametros.Add(new ReportParameter("periodo", periodo));
                            parametros.Add(new ReportParameter("mes", mes.ToString()));

                            string fileName = Utilities.GetName(ReportName.ReporteDiferenciasNomina, año, mes);
                            ReportViewerReporte.LocalReport.DisplayName = fileName;
                            ReportViewerReporte.ProcessingMode = ProcessingMode.Local;
                            ReportViewerReporte.LocalReport.ReportPath = Request.PhysicalApplicationPath.ToString() + @"Reportes\rdlc\ReporteDiferenciasNomina.rdlc";
                            ReportViewerReporte.LocalReport.SetParameters(parametros);
                            ReportViewerReporte.LocalReport.DataSources.Clear();
                            ReportViewerReporte.LocalReport.DataSources.Add(new ReportDataSource("dtDiferenciasNomina", datos));
                            ReportViewerReporte.Visible = true;
                            this.LabelMensaje.Text = String.Empty;

                            Byte[] result;
                            string format = "Excel";
                            string mimetype;
                            string encoding;
                            string fileNameExtension;
                            Warning[] warnings;
                            string[] streamids;
                            result = ReportViewerReporte.LocalReport.Render(format, null, out mimetype, out encoding, out fileNameExtension, out streamids, out warnings);
                            Response.Clear();
                            // Response.AppendHeader("content-length", result.Length.ToString());
                            Response.AddHeader("content-disposition", "attachment; filename=" + fileName + ".xls");
                            Response.ContentType = mimetype;
                            Response.BinaryWrite(result);
                            Response.Flush();
                            Response.Close();
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

        #endregion Methods
    }
}