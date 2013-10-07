using System;
using System.Xml;
using BDOPayrollReporter.Business;

namespace BDOPayrollReporter.Layouts
{
    public partial class LayoutTepca : System.Web.UI.Page
    {
        #region Propiedades

        /// <summary>
        /// Almacena el consecutivo de transacciones diarias.
        /// </summary>
        public int Consecutive
        {
            get
            {
                if (ViewState["Consecutive"] == null)
                    return 1;
                else
                    return (int)ViewState["Consecutive"];
            }
            set { ViewState["Consecutive"] = value; }
        }

        #endregion Propiedades

        #region Eventos

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                try
                {
                    Utilities.ObtenerAñosPorMovimiento(this.ddlAno);
                    Utilities.ObtenerMesesEnAñosPorMovimiento(this.ddlAno, this.ddlPeriodo);
                    Utilities.ObtenerTiposNomina(this.ddlTipoNomina);
                    //GetConsecutivo();
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
                if (ddlOperacion.SelectedValue == "1")
                    GenerarReporteEmision();
                else
                {
                    GeneraReporteRecarga();
                }
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error: " + ex.Message;
            }
        }

        #endregion Eventos

        #region Metodos

        private void GenerarReporteEmision()
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
                                GetConsecutivo();
                                SaveConsecutivo();
                                string nombrearchivo = "";
                                file = TextHelper.RenderLayoutTepcaEmision(año, mes, Consecutive, ref nombrearchivo);

                                if (file.Length > 0)
                                {
                                    this.lblMensaje.Text = "";
                                    SaveConsecutivo();
                                    Response.Clear();
                                    Response.AddHeader("content-disposition", string.Format("attachment; filename=loteEmision{0}.txt", nombrearchivo));
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

        private void GeneraReporteRecarga()
        {
            int mes, año, nomina, operacion;
            Byte[] file;
            TextRenderer TextHelper;

            try
            {
                TextHelper = new TextRenderer();

                if (Utilities.IsNumeric(this.ddlAno.SelectedValue))
                {
                    if (Utilities.IsNumeric(this.ddlPeriodo.SelectedValue))
                    {
                        if (Utilities.IsNumeric(this.ddlTipoNomina.SelectedValue))
                        {
                            mes = Int32.Parse(this.ddlPeriodo.SelectedValue);
                            año = Int32.Parse(this.ddlAno.SelectedValue);
                            nomina = Int32.Parse(this.ddlTipoNomina.SelectedValue);
                            operacion = Int32.Parse(this.ddlOperacion.SelectedValue);

                            if (Utilities.ValidarAño(año))
                            {
                                if (Utilities.ValidarMes(mes))
                                {
                                    GetConsecutivo();
                                    SaveConsecutivo();
                                    string nombrearchivo = "";

                                    file = TextHelper.RenderLayoutTepcaRecarga(año, mes, nomina, operacion, ddlEstadoNomina.SelectedValue.Equals("1") ? true : false, Consecutive, ref nombrearchivo, chkEsPrueba.Checked);

                                    if (file.Length > 0)
                                    {
                                        this.lblMensaje.Text = "";
                                        SaveConsecutivo();
                                        Response.Clear();
                                        Response.AddHeader("content-disposition", string.Format("attachment; filename=loteRecarga{0}.txt", nombrearchivo));
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
                            this.lblMensaje.Text = "El Tipo de Nomina no es válido";
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

        /// <summary>
        /// Metodo que obtiene el consecutivo de transacciones diaria.
        /// </summary>
        private void GetConsecutivo()
        {
            try
            {
                XmlDocument myXmlDocument = new XmlDocument();
                string pathFile = Request.PhysicalApplicationPath.ToString() + @"Layouts\Data.xml";
                myXmlDocument.Load(pathFile);
                XmlNode nodePrincipal;
                XmlNode node;
                nodePrincipal = myXmlDocument.DocumentElement;
                node = nodePrincipal.ChildNodes[0];
                DateTime fecha = DateTime.Parse(node.ChildNodes[0].InnerText);
                if (fecha.ToShortDateString() != DateTime.Now.ToShortDateString())
                {
                    Consecutive = 1;
                    node.ChildNodes[0].InnerText = DateTime.Now.ToShortDateString();
                    node.ChildNodes[1].InnerText = "1";
                    myXmlDocument.Save(pathFile);
                }
                else
                {
                    Consecutive = int.Parse(node.ChildNodes[1].InnerText);
                }
            }
            catch (Exception oe)
            {
                throw oe;
            }
        }

        /// <summary>
        /// Metodo que guarda el consecutivo de transacciones diaria.
        /// </summary>
        private void SaveConsecutivo()
        {
            try
            {
                int val = Consecutive;
                XmlDocument myXmlDocument = new XmlDocument();
                string pathFile = Request.PhysicalApplicationPath.ToString() + @"Layouts\Data.xml";
                myXmlDocument.Load(pathFile);
                XmlNode nodePrincipal;
                XmlNode node;
                nodePrincipal = myXmlDocument.DocumentElement;
                node = nodePrincipal.ChildNodes[0];
                node.ChildNodes[1].InnerText = (++val).ToString();
                myXmlDocument.Save(pathFile);
            }
            catch (Exception oe)
            {
                throw oe;
            }
        }

        #endregion Metodos
    }
}