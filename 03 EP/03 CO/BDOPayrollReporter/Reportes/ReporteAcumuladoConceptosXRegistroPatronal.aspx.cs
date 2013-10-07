using System;
using System.Collections.Generic;
using System.Data;
using BDOPayrollReporter.Business;
using BDOPayrollReporter.Business.Enums;
using Microsoft.Reporting.WebForms;

namespace BDOPayrollReporter.Reportes
{
    public partial class ReporteAcumuladoConceptosXRegistroPatronal : System.Web.UI.Page
    {
        #region Properties

        public int Tipo
        {
            get
            {
                if (ViewState["Tipo"] == null)
                    return 1;
                else
                    return (int)ViewState["Tipo"];
            }
            set
            {
                ViewState["Tipo"] = value;
            }
        }

        #endregion Properties

        #region Eventos

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                try
                {
                    Utilities.ObtenerAñosPorMovimiento(this.DropDownListAño);
                    Utilities.ObtenerMesesEnAñosPorMovimiento(this.DropDownListAño, this.DropDownListPeriodo);
                    if (Request.QueryString["tipo"] != null)
                        Tipo = Convert.ToInt32(Request.QueryString["tipo"].ToString());
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

        #endregion Eventos

        #region Metodos

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
                        datos = SqlHelper.ReporteAcumuladoConceptos(año, mes, Tipo);

                        if (datos.Rows.Count > 0)
                        {
                            parametros.Add(new ReportParameter("p_Mes", mes.ToString()));
                            parametros.Add(new ReportParameter("p_Año", año.ToString()));
                            parametros.Add(new ReportParameter("p_Tipo", Tipo.ToString()));

                            ReportViewerReporte.ProcessingMode = ProcessingMode.Local;

                            if (Tipo == 1)
                                ReportViewerReporte.LocalReport.DisplayName = Utilities.GetName(ReportName.Acumuladodeconceptostotal, año, mes);
                            else
                                ReportViewerReporte.LocalReport.DisplayName = Utilities.GetName(ReportName.Acumuladoconceptosperiodo, año, mes);

                            ReportViewerReporte.LocalReport.ReportPath = Request.PhysicalApplicationPath.ToString() + "Reportes\\rdlc\\ReporteAcumuladoConceptosXRegistroPatronal.rdlc";
                            ReportViewerReporte.LocalReport.SetParameters(parametros);
                            ReportViewerReporte.LocalReport.DataSources.Clear();
                            ReportViewerReporte.LocalReport.DataSources.Add(new ReportDataSource("dtAcumuladoConceptosXEmpleado", datos));
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

        #endregion Metodos
    }
}