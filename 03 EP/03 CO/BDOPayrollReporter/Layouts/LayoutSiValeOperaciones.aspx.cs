using System;
using BDOPayrollReporter.Business;
using BDOPayrollReporter.Business.Enums;

namespace BDOPayrollReporter.Layouts
{
    public partial class LayoutSiValeOperaciones : System.Web.UI.Page
    {
        #region Eventos

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                try
                {
                    Utilities.ObtenerAñosPorMovimiento(this.ddlAno);
                    Utilities.ObtenerMesesEnAñosPorMovimiento(this.ddlAno, this.ddlPeriodo);

                    StringEnum Operaciones = new StringEnum(typeof(OperacionSiVale));
                    ddlOperacion.DataSource = Operaciones.GetListValues();
                    ddlOperacion.DataValueField = "key";
                    ddlOperacion.DataTextField = "value";
                    ddlOperacion.DataBind();
                }
                catch (Exception ex)
                {
                    this.lblMensaje.Text = "Error: " + ex.Message;
                }
            }
        }

        protected void ddlAno_SelectedIndexChanged(object sender, EventArgs e)
        {
            Utilities.ObtenerMesesEnAñosPorMovimiento(this.ddlAno, this.ddlPeriodo);
        }

        protected void btnGenerar_Click(object sender, EventArgs e)
        {
            try
            {
                GenerarReporte();
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error: " + ex.Message;
            }
        }

        #endregion Eventos

        #region Metodos

        private void GenerarReporte()
        {
            int mes, año;
            Byte[] file;
            TextRenderer TextHelper;

            try
            {
                TextHelper = new TextRenderer();

                if (Utilities.IsNumeric(this.ddlAno.SelectedValue))
                {
                    if (Utilities.IsNumeric(this.ddlPeriodo.SelectedValue))
                    {
                        mes = Int32.Parse(this.ddlPeriodo.SelectedValue);
                        año = Int32.Parse(this.ddlAno.SelectedValue);

                        if (Utilities.ValidarAño(año))
                        {
                            if (Utilities.ValidarMes(mes))
                            {
                                string nombrearchivo = "";
                                OperacionSiVale operacion = (OperacionSiVale)Enum.Parse(typeof(OperacionSiVale), ddlOperacion.SelectedValue);
                                file = TextHelper.RenderLayoutSivaleOperaciones(año, mes, operacion, ref nombrearchivo);

                                if (file.Length > 0)
                                {
                                    this.lblMensaje.Text = "";
                                    Response.Clear();
                                    Response.AddHeader("content-disposition", string.Format("attachment; filename={0}.txt", nombrearchivo));
                                    Response.ContentType = "text/plain";
                                    Response.BinaryWrite(file);
                                    Response.End();
                                }
                                else
                                    this.lblMensaje.Text = "No hay información de exportación.";
                            }
                            else
                                this.lblMensaje.Text = "El Mes no es válido";
                        }
                        else
                            this.lblMensaje.Text = "El Año no es válido";
                    }
                    else
                        this.lblMensaje.Text = "El periodo seleccionado no es válido.";
                }
                else
                    this.lblMensaje.Text = "El año seleccionado no es válido.";
            }
            catch (Exception e)
            {
                this.lblMensaje.Text = "Error: " + e.Message;
            }
        }

        #endregion Metodos
    }
}