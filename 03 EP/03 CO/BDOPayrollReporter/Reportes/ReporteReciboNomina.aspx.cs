using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using BDOPayrollReporter.Business;
using BDOPayrollReporter.Business.Enums;
using BDOPayrollReporter.Business.Objects;
using Microsoft.Reporting.WebForms;

namespace BDOPayrollReporter.Reportes
{
    public partial class ReporteReciboNomina : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                try
                {
                    Utilities.ObtenerAñosPorMovimiento(this.DropDownListAño);
                    Utilities.ObtenerMesesEnAñosPorMovimiento(this.DropDownListAño, this.DropDownListPeriodo);
                    Utilities.ObtenerDiasEnMeses(this.DropDownListAño, this.DropDownListPeriodo, this.DropDownListDia);
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
            Utilities.ObtenerMesesEnAñosPorMovimiento(this.DropDownListAño, this.DropDownListPeriodo);
        }

        protected void DropDownListPeriodo_SelectedIndexChanged(object sender, EventArgs e)
        {
            Utilities.ObtenerDiasEnMeses(this.DropDownListAño, this.DropDownListPeriodo, this.DropDownListDia);
        }

        protected void ButtonGenerar_Click(object sender, EventArgs e)
        {
            this.GenerarReporte(false, new Empleado());
            this.ListBoxEmpleados.ClearSelection();
        }

        protected void ButtonEmail_Click(object sender, EventArgs e)
        {
            DataAccess SqlHelper = new DataAccess();
            string path = Request.PhysicalApplicationPath.ToString() + @"Reportes\Recibos\";
            string[] fileList = Directory.GetFiles(path, "*.pdf");

            foreach (string file in fileList)
            {
                File.Delete(file);
            }

            foreach (Empleado empleado in SqlHelper.GetEmpleados())
            {
                this.GenerarReporte(true, empleado);
            }

            this.ListBoxEmpleados.ClearSelection();
            this.LabelMensaje.Text = "Se enviaron correctamente los recibos.";
        }

        private void GenerarReporte(bool sendMail, Empleado empleado)
        {
            const int maxcount = 20;
            int mes, año, dia, empcount;
            short tipo;
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
                        if (Utilities.IsNumeric(this.DropDownListDia.SelectedValue))
                        {
                            List<string> empleadoList = new List<string>();
                            mes = Int32.Parse(this.DropDownListPeriodo.SelectedValue);
                            año = Int32.Parse(this.DropDownListAño.SelectedValue);
                            dia = Int32.Parse(this.DropDownListDia.SelectedValue);
                            tipo = Int16.Parse(this.DropDownListTipoNomina.SelectedValue);
                            if (sendMail)
                            {
                                int count = Utilities.ObtenerListEmpleados(this.ListBoxEmpleados).AsEnumerable().Count(id => id.Contains(empleado.Id));
                                if (count == 0)
                                    return;
                                else
                                    empleadoList.Add(empleado.Id);
                            }
                            else
                                empleadoList = Utilities.ObtenerListEmpleados(this.ListBoxEmpleados);

                            datos = SqlHelper.ReporteReciboNomina(año, mes, tipo,
                                              !DropDownListEstadoNomina.SelectedValue.Equals("0") ? true : false,
                                              empleadoList, sendMail);

                            var empleados = from ho in datos.AsEnumerable()
                                            group ho by new
                                            {
                                                Empleado = ho["Empleado"],
                                            }
                                                into grp
                                                select new
                                                {
                                                    Empleado = grp.Key,
                                                    Count = grp.Count(t => t["Empleado"] != null)
                                                };

                            foreach (var oempleado in empleados)
                            {
                                empcount = oempleado.Count;
                                while (++empcount <= maxcount)
                                {
                                    DataRow newrow = datos.NewRow();

                                    var data = (from e in datos.AsEnumerable()
                                                where e["Empleado"] == oempleado.Empleado.Empleado
                                                select new
                                                     {
                                                         Empleado = e["Empleado"],
                                                         Nombre = e["Nombre"],
                                                         Rfc = e["Rfc"],
                                                         Curp = e["Curp"],
                                                         FechaIngreso = e["FechaIngreso"],
                                                         FechaBaja = e["FechaBaja"],
                                                         Nss = e["Nss"],
                                                         Sd = e["Sd"],
                                                         Sdi = e["Sdi"],
                                                         Convenio = e["Convenio"],
                                                         Puesto = e["Puesto"],
                                                         CentroTrabajo = e["CentroTrabajo"],
                                                         Departamento = e["Departamento"],
                                                         CentroCosto = e["CentroCosto"],
                                                         ValesDespensa = e["ValesDespensa"],
                                                         ValesGasolina = e["ValesGasolina"],
                                                         Tipo = e["Tipo"]
                                                     }).First();

                                    newrow["Empleado"] = data.Empleado;
                                    newrow["Nombre"] = data.Nombre;
                                    newrow["Rfc"] = data.Rfc;
                                    newrow["Curp"] = data.Curp;
                                    newrow["FechaIngreso"] = data.FechaIngreso;
                                    newrow["FechaBaja"] = data.FechaBaja;
                                    newrow["Nss"] = data.Nss;
                                    newrow["Sd"] = data.Sd;
                                    newrow["Sdi"] = data.Sdi;
                                    newrow["Convenio"] = data.Convenio;
                                    newrow["Puesto"] = data.Puesto;
                                    newrow["CentroTrabajo"] = data.CentroTrabajo;
                                    newrow["Departamento"] = data.Departamento;
                                    newrow["CentroCosto"] = data.CentroCosto;
                                    newrow["Concepto"] = empcount.ToString();
                                    newrow["Descripcion"] = string.Empty;
                                    newrow["Unidad"] = 0;
                                    newrow["Percepcion"] = 0;
                                    newrow["Deduccion"] = 0;
                                    newrow["ValesDespensa"] = data.ValesDespensa;
                                    newrow["ValesGasolina"] = data.ValesGasolina;
                                    newrow["Tipo"] = 2;
                                    datos.Rows.Add(newrow);
                                }
                            }

                            if (datos.Rows.Count > 0)
                            {
                                LabelMensaje.Text = String.Empty;
                                string path = Request.PhysicalApplicationPath.ToString() + @"Reportes\Recibos\";
                                string reportName = Utilities.GetName(ReportName.Recibodenomina, año, mes);

                                parametros.Add(new ReportParameter("p_Mes", mes.ToString()));
                                parametros.Add(new ReportParameter("p_Año", año.ToString()));
                                parametros.Add(new ReportParameter("p_Dia", dia.ToString()));
                                parametros.Add(new ReportParameter("p_Tipo", tipo.ToString()));

                                ReportViewerReporte.ProcessingMode = ProcessingMode.Local;
                                ReportViewerReporte.LocalReport.DisplayName = reportName;
                                ReportViewerReporte.LocalReport.ReportPath = Request.PhysicalApplicationPath.ToString() + "Reportes\\rdlc\\ReportReciboNomina.rdlc";
                                ReportViewerReporte.LocalReport.SetParameters(parametros);
                                ReportViewerReporte.LocalReport.DataSources.Clear();
                                ReportViewerReporte.LocalReport.DataSources.Add(new ReportDataSource("dtReciboNomina", datos));

                                if (sendMail)
                                {
                                    Byte[] result;
                                    string format = "PDF";
                                    string mimetype;
                                    string encoding;
                                    string fileNameExtension;
                                    Warning[] warnings;
                                    string[] streamids;

                                    string filename = string.Format("{0}{1}_{2}.pdf", path, reportName, empleado.Id);
                                    result = ReportViewerReporte.LocalReport.Render(format, null, out mimetype, out encoding, out fileNameExtension, out streamids, out warnings);

                                    System.IO.FileStream _FileStream = new System.IO.FileStream(filename, System.IO.FileMode.Create, System.IO.FileAccess.Write);
                                    _FileStream.Write(result, 0, result.Length);
                                    _FileStream.Close();

                                    Utilities.SendMail(empleado, filename, CheckBoxTest.Checked, mes, año);
                                    ReportViewerReporte.Visible = false;
                                }
                                else
                                {
                                    ReportViewerReporte.Visible = true;
                                }
                            }
                            else
                            {
                                this.LabelMensaje.Text = "No hay información de exportación.";
                                ReportViewerReporte.Visible = false;
                            }
                        }
                        else
                        {
                            this.LabelMensaje.Text = "El día seleccionado no es válido.";
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
            catch (Exception oe)
            {
                this.LabelMensaje.Text = "Error: " + oe.Message;
            }
        }
    }
}