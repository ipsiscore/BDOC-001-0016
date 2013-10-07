using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Web.UI.WebControls;
using BDOPayrollReporter.Business.Enums;
using BDOPayrollReporter.Business.Objects;
using SIO.Services.Data.Services;

namespace BDOPayrollReporter.Business
{
    public class Utilities
    {
        #region Fields

        private static string[] meses = new string[12] { "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre" };

        private static string[] mesesAbrev = new string[12] { "ene", "feb", "mar", "abr", "may", "jun", "jul", "ago", "sep", "oct", "nov", "dic" };

        #endregion Fields

        /// <summary>
        /// Comprueba si un valor puede ser convertido en numérico.
        /// </summary>
        /// <param name="Expression">Expresión a evaluar.</param>
        /// <returns><para>Devuelve verdadero si el valor puede ser convertido en número. Si el valor es nulo o está vacío devuelve falso.</para></returns>
        public static bool IsNumeric(object Expression)
        {
            bool isNum;
            double retNum;

            isNum = Double.TryParse(Convert.ToString(Expression), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
            return isNum;
        }

        /// <summary>
        /// Comprueba si un valor puede ser convertido en fecha.
        /// </summary>
        /// <param name="Expression">Expresión a evaluar.</param>
        /// <returns><para>Devuelve verdadero si el valor puede ser convertido en fecha. Si el valor es nulo o está vacío devuelve falso.</para></returns>
        public static bool IsDate(object Expression)
        {
            bool isDate;
            DateTime retDate;

            isDate = DateTime.TryParse(Convert.ToString(Expression), System.Globalization.DateTimeFormatInfo.CurrentInfo, System.Globalization.DateTimeStyles.AssumeLocal, out retDate);
            return isDate;
        }

        public static bool ValidarAño(int Año)
        {
            if (Año >= 2000 && Año <= 4999)
                return true;
            else
                return false;
        }

        public static bool ValidarMes(int Mes)
        {
            if (Mes >= 1 || Mes <= 12)
                return true;
            else
                return false;
        }

        public static int ObtenerAñosPorMovimiento(global::System.Web.UI.WebControls.DropDownList list)
        {
            StringBuilder query = new StringBuilder();
            DataTable result;

            query.AppendLine("SELECT");
            query.AppendLine("	Año");
            query.AppendLine("FROM");
            query.AppendLine("	(");
            query.AppendLine("		SELECT");
            query.AppendLine("			[Ano] AS Año");
            query.AppendLine("		FROM");
            query.AppendLine("			[dbo].[NomCalculoHistorico]");
            query.AppendLine("		UNION");
            query.AppendLine("		SELECT");
            query.AppendLine("			[Ano] AS Año");
            query.AppendLine("		FROM");
            query.AppendLine("			[dbo].[NomCalculo]");
            query.AppendLine("		) AS [NomCalculoHistorico]");
            query.AppendLine("GROUP BY [Año]");
            query.AppendLine("ORDER BY [Año] DESC");

            try
            {
                SQLDataManager connector = new SQLDataManager();
                result = connector.ExecuteQuery(query.ToString());
                list.DataTextField = "Año";
                list.DataValueField = "Año";
                list.DataSource = result;
                list.DataBind();
                list.SelectedIndex = 0;
            }
            catch (Exception e)
            {
                result = null;
                throw e;
            }

            return list.Items.Count;
        }

        public static int ObtenerMesesEnAñosPorMovimiento(global::System.Web.UI.WebControls.DropDownList listaño, global::System.Web.UI.WebControls.DropDownList listmes)
        {
            int Año;
            StringBuilder query = new StringBuilder();
            DataTable result;

            try
            {
                Año = System.Convert.ToInt32(listaño.SelectedValue);
            }
            catch (Exception)
            {
                Año = 0;
            }

            if (Utilities.ValidarAño(Año))
            {
                query.Append("SELECT [Mes], ");
                query.Append("   CASE WHEN [Mes] =  1 THEN 'Enero' ");
                query.Append("       WHEN [Mes] =  2 THEN 'Febrero' ");
                query.Append("       WHEN [Mes] =  3 THEN 'Marzo' ");
                query.Append("       WHEN [Mes] =  4 THEN 'Abril' ");
                query.Append("       WHEN [Mes] =  5 THEN 'Mayo' ");
                query.Append("       WHEN [Mes] =  6 THEN 'Junio' ");
                query.Append("       WHEN [Mes] =  7 THEN 'Julio' ");
                query.Append("       WHEN [Mes] =  8 THEN 'Agosto' ");
                query.Append("       WHEN [Mes] =  9 THEN 'Septiembre' ");
                query.Append("       WHEN [Mes] = 10 THEN 'Octubre' ");
                query.Append("       WHEN [Mes] = 11 THEN 'Noviembre' ");
                query.Append("       WHEN [Mes] = 12 THEN 'Diciembre' ");
                query.Append("   ELSE 'Revisar periodos' END [MesTexto] FROM ( ");
                query.AppendFormat("SELECT [Periodo] AS Mes FROM [dbo].[NomCalculoHistorico] WHERE [Ano] = {0} ", Año);
                query.Append("UNION ");
                query.AppendFormat("SELECT [Periodo] AS Mes FROM [dbo].[NomCalculo] WHERE [Ano] = {0} ", Año);
                query.Append(") AS [NomCalculoHistorico] ");
                query.Append("GROUP BY [Mes] ORDER BY [Mes] ");

                try
                {
                    SQLDataManager connector = new SQLDataManager();
                    result = connector.ExecuteQuery(query.ToString());
                    listmes.DataTextField = "MesTexto";
                    listmes.DataValueField = "Mes";
                    listmes.DataSource = result;
                    listmes.DataBind();
                    listmes.SelectedIndex = 0;
                }
                catch (Exception e)
                {
                    result = null;
                    throw e;
                }
            }
            else
                throw new ArgumentException("El periodo no es válido", "listmes");

            return listmes.Items.Count;
        }

        public static int ObtenerDiasEnMeses(global::System.Web.UI.WebControls.DropDownList listaño, global::System.Web.UI.WebControls.DropDownList listmes, global::System.Web.UI.WebControls.DropDownList listdia)
        {
            int Año, Mes, DiaMax, indx;

            try
            {
                Año = System.Convert.ToInt32(listaño.SelectedValue);
            }
            catch (Exception)
            {
                Año = 0;
            }

            try
            {
                Mes = System.Convert.ToInt32(listmes.SelectedValue);
            }
            catch (Exception)
            {
                Mes = 0;
            }
            indx = 0;

            if (Utilities.ValidarMes(Mes))
            {
                if (Utilities.ValidarMes(Mes))
                {
                    try
                    {
                        DiaMax = DateTime.DaysInMonth(Año, Mes);
                        listdia.Items.Clear();
                        while (++indx <= DiaMax)
                        {
                            listdia.Items.Add(new global::System.Web.UI.WebControls.ListItem(indx.ToString(), indx.ToString()));
                        }
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
                else
                    throw new ArgumentException("El periodo no es válido", "listmes");
            }
            else
                throw new ArgumentException("El año no es válido", "listaño");

            return listmes.Items.Count;
        }

        public static int ObtenerTiposNomina(global::System.Web.UI.WebControls.DropDownList listtipos)
        {
            StringBuilder query = new StringBuilder();
            DataTable result;

            query.AppendLine("SELECT");
            query.AppendLine("	[TipoNomina_ID] AS Id,");
            query.AppendLine("	[Descripcion] AS Descripcion");
            query.AppendLine("FROM [dbo].[TBTipoNomina]");

            try
            {
                SQLDataManager connector = new SQLDataManager();
                result = connector.ExecuteQuery(query.ToString());
                listtipos.DataTextField = "Descripcion";
                listtipos.DataValueField = "Id";
                listtipos.DataSource = result;
                listtipos.DataBind();
            }
            catch (Exception e)
            {
                result = null;
                throw e;
            }

            return listtipos.Items.Count;
        }

        public static int ObtenerTiposNomina(global::System.Web.UI.WebControls.DropDownList listtipos, string Text, string Value)
        {
            StringBuilder query = new StringBuilder();
            DataTable result;

            query.AppendLine("SELECT ");
            query.AppendFormat("{0} AS Id,", Value);
            query.AppendFormat("'{0}' AS Descripcion", Text);
            query.AppendLine(" UNION ALL ");
            query.AppendLine("SELECT");
            query.AppendLine("	[TipoNomina_ID] AS Id,");
            query.AppendLine("	[Descripcion] AS Descripcion");
            query.AppendLine("FROM [dbo].[TBTipoNomina]");

            try
            {
                SQLDataManager connector = new SQLDataManager();
                result = connector.ExecuteQuery(query.ToString());
                listtipos.DataTextField = "Descripcion";
                listtipos.DataValueField = "Id";
                listtipos.DataSource = result;
                listtipos.DataBind();
            }
            catch (Exception e)
            {
                result = null;
                throw e;
            }

            return listtipos.Items.Count;
        }

        public static int ObtenerEmpleados(global::System.Web.UI.WebControls.ListBox listempleados)
        {
            StringBuilder query = new StringBuilder();
            DataTable result;

            query.AppendLine("SELECT");
            query.AppendLine("	Trab_ID,");
            query.AppendLine("	Paterno + ' ' + Materno + ' ' + Nombre Nombre");
            query.AppendLine("FROM Empleado");
            query.AppendLine("ORDER BY Paterno");

            try
            {
                SQLDataManager connector = new SQLDataManager();
                result = connector.ExecuteQuery(query.ToString());
                listempleados.DataTextField = "Nombre";
                listempleados.DataValueField = "Trab_ID";
                listempleados.DataSource = result;
                listempleados.DataBind();
            }
            catch (Exception e)
            {
                result = null;
                throw e;
            }

            return listempleados.Items.Count;
        }

        public static List<string> ObtenerListEmpleados(global::System.Web.UI.WebControls.ListBox listempleados)
        {
            List<string> datos = new List<string>();
            try
            {
                foreach (ListItem item in listempleados.Items)
                {
                    if (item.Selected)
                        datos.Add(item.Value);
                }

                if (datos.Count == 0)
                {
                    foreach (ListItem item in listempleados.Items)
                    {
                        datos.Add(item.Value);
                    }
                }
            }
            catch (Exception oe)
            {
                throw oe;
            }
            return datos;
        }

        public static string GetName(ReportName name, int año, int mes)
        {
            string reportName = string.Empty;
            try
            {
                switch (name)
                {
                    case ReportName.Recibodenomina:
                        reportName = string.Format("recibos_hs_{0}_{1}", mesesAbrev[mes - 1], año);
                        break;

                    case ReportName.Empleadoporconcepto:
                        reportName = string.Format("empleados_x_cpto_hs_{0}_{1}", mesesAbrev[mes - 1], año);
                        break;

                    case ReportName.Acumuladosdeconceptosporempleado:
                        reportName = string.Format("reporte_total_empleado_{0}_{1}", mesesAbrev[mes - 1], año);
                        break;

                    case ReportName.Resumendeconceptosporempresa:
                        reportName = string.Format("res_empre_hs_{0}_{1}", mesesAbrev[mes - 1], año);
                        break;

                    case ReportName.Resumendeconceptosporcentrodecosto:
                        reportName = string.Format("res_centro_costo_hs_{0}_{1}", mesesAbrev[mes - 1], año);
                        break;

                    case ReportName.Acumuladodeconceptostotal:
                        reportName = string.Format("reporte_total_hs_{0}_{1}", mesesAbrev[mes - 1], año);
                        break;

                    case ReportName.Reporteimpuestosobrelanomina:
                        reportName = string.Format("impuesto_nomina_{0}_{1}", mesesAbrev[mes - 1], año);
                        break;

                    case ReportName.Listadodeimportesgravadosyexcentos:
                        reportName = string.Format("list_grav_hs_{0}{1}", mesesAbrev[mes - 1], año);
                        break;

                    case ReportName.Listadodevalesdedespensacentrodecostos:
                        reportName = string.Format("listado_vales_hs_cc_{0}_{1}", mesesAbrev[mes - 1], año);
                        break;

                    case ReportName.Listadodevalesdedespensa:
                        reportName = string.Format("listado_vales_hs_{0}_{1}", mesesAbrev[mes - 1], año);
                        break;

                    case ReportName.Reportedeaportacionesfondodeahorro:
                        reportName = string.Format("acum_fondo_ahorro_{0}_{1}", mesesAbrev[mes - 1], año);
                        break;

                    case ReportName.Reportefondodeahorro:
                        reportName = string.Format("hs_fa_cc_{0}_{1}", mesesAbrev[mes - 1], año);
                        break;

                    case ReportName.Reporteplandepensiones:
                        reportName = string.Format("hs_pp_cc_{0}_{1}", mesesAbrev[mes - 1], año);
                        break;

                    case ReportName.Reportedeaportacionesplandepensiones:
                        reportName = string.Format("acum_plan_pensiones_{0}_{1}", mesesAbrev[mes - 1], año);
                        break;

                    case ReportName.Maestrodeempleados:
                        reportName = string.Format("maestro_hs_{0}_{1}", mesesAbrev[mes - 1], año);
                        break;

                    case ReportName.Reportedecontroldecambioennomina:
                        reportName = string.Format("cambioshs_{0}_{1}", mesesAbrev[mes - 1], año);
                        break;

                    case ReportName.Provisionesporcentrodecosto:
                        reportName = string.Format("prov_centro_costo_hs_{0}_{1}", mesesAbrev[mes - 1], año);
                        break;

                    case ReportName.Transferenciabanamex:
                        reportName = string.Format("rep_banamex_{0}_{1}", mesesAbrev[mes - 1], año);
                        break;

                    case ReportName.Listadodenomina:
                        reportName = string.Format("listado_nom_hs_{0}_{1}", mesesAbrev[mes - 1], año);
                        break;

                    case ReportName.Layoutdecitybank484:
                        reportName = string.Format("depositos_citidirect_{0}{1}", mes.ToString().Length == 1 ? string.Format("0{0}", mes) : mes.ToString(), año);
                        break;

                    case ReportName.Layoutdecitybank485:
                        reportName = string.Format("asignar", mesesAbrev[mes - 1], año);
                        break;

                    case ReportName.Layoutdealtasdebanorte:
                        reportName = string.Format("altas_banorte_{0}_{1}", mesesAbrev[mes - 1], año);
                        break;

                    case ReportName.Layoutdebanorte:
                        reportName = string.Format("movimientos_banorte_{0}_{1} ", mesesAbrev[mes - 1], año);
                        break;

                    case ReportName.Layoutdesivalesolicituddetarjetas:
                        reportName = string.Format("altas_sivale_{0}_{1}", mesesAbrev[mes - 1], año);
                        break;

                    case ReportName.Layoutdesivalecargadesaldos:
                        reportName = string.Format("saldos_sivale_{0}_{1}", mesesAbrev[mes - 1], año);
                        break;

                    case ReportName.Layoutdealtasskandia:
                        reportName = string.Format("altas_skandia_{0}_{1}", meses[mes - 1].ToLower(), año);
                        break;

                    case ReportName.Layoutdeaportacionesskandia:
                        reportName = string.Format("aportaciones_skandia_{0}_{1}", meses[mes - 1], año);
                        break;

                    case ReportName.Layoutdebajasskandia:
                        reportName = string.Format("bajas_skandia_{0}_{1}", mesesAbrev[mes - 1], año);
                        break;

                    case ReportName.Layoutimportacionasap:
                        reportName = string.Format("inbound_layout_sap_{0}_{1}", mesesAbrev[mes - 1], año);
                        break;

                    case ReportName.Acumuladoconceptosperiodo:
                        reportName = string.Format("acum_empre_hs_{0}_{1}", mesesAbrev[mes - 1], año);
                        break;

                    case ReportName.ReporteDiferenciasNomina:
                        reportName = string.Format("dif_nom_{0}_{1}", mesesAbrev[mes - 1], año);
                        break;
                }
            }
            catch (Exception oe)
            {
                throw oe;
            }
            return reportName;
        }

        public static bool SendMail(Empleado empleado, string path, bool test, int periodo, int año)
        {
            bool successfull = false;
            try
            {
                string mailServer = Encryption.Decrypt(ConfigurationManager.AppSettings["MailServer"].ToString());
                int port = int.Parse(Encryption.Decrypt(ConfigurationManager.AppSettings["Port"].ToString()));

                string accountTo = Encryption.Decrypt(ConfigurationManager.AppSettings["AccountTo"].ToString());

                string accountFrom = Encryption.Decrypt(ConfigurationManager.AppSettings["AccountFrom"].ToString());
                string password = Encryption.Decrypt(ConfigurationManager.AppSettings["AccountPassword"].ToString());

                MailAddress to;
                if (test)
                    to = new MailAddress(accountTo, empleado.NombreCompleto);
                else
                    to = new MailAddress(empleado.Email, empleado.NombreCompleto);
                MailAddress from = new MailAddress(accountFrom, "BDO Nóminas");
                MailMessage mail = new MailMessage(from, to);
                mail.Subject = string.Format("Recibo de nómina del mes de {0} {1}", meses[periodo - 1], año);
                mail.SubjectEncoding = Encoding.GetEncoding("iso-8859-1");
                mail.IsBodyHtml = true;
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(@"<html><div style=""font-family:Trebuchet MS;"">");
                sb.AppendFormat("{0},", empleado.NombreCompleto);
                sb.AppendLine("<br/><br/>");
                sb.AppendFormat("Adjunto encontara el recibo de nómina correspondiente al mes de {0} {1}.", meses[periodo - 1], año);
                sb.AppendLine("<br/><br/>");
                sb.AppendLine("Muchas gracias.");
                sb.AppendLine("</div></table>");
                mail.Body = sb.ToString();
                SmtpClient smtpclient = new SmtpClient(mailServer, port);
                Attachment data = new Attachment(path, MediaTypeNames.Application.Octet);

                ContentDisposition disposition = data.ContentDisposition;
                disposition.CreationDate = System.IO.File.GetCreationTime(path);
                disposition.ModificationDate = System.IO.File.GetLastWriteTime(path);
                disposition.ReadDate = System.IO.File.GetLastAccessTime(path);

                mail.Attachments.Add(data);
                smtpclient.UseDefaultCredentials = false;
                smtpclient.Credentials = new NetworkCredential(accountFrom, password);
                smtpclient.Send(mail);
            }
            catch (Exception oe)
            {
                throw oe;
            }

            return successfull;
        }

        public static string GetMonthTitle(int mes)
        {
            string mesTitulo = string.Empty;
            try
            {
                mesTitulo = meses[mes - 1];
            }
            catch (Exception oe)
            {
                throw oe;
            }
            return mesTitulo;
        }

        public static string GetFileHeaderSiVale(OperacionSiVale operacion)
        {
            string fileHeader = string.Empty;
            try
            {
                switch (operacion)
                {
                    case OperacionSiVale.MisComprasSolicitud:
                        fileHeader = "AKL.TIT.MIS";
                        break;

                    case OperacionSiVale.MisComprasSolicitudAdicionales:
                        fileHeader = "AKL.ADI.MIS";
                        break;

                    case OperacionSiVale.MisComprasAlimentacionSolicitud:
                        fileHeader = "AKL.TIT.MCA";
                        break;

                    case OperacionSiVale.MisComprasAlimentacionSolicitudAdicionales:
                        fileHeader = "AKL.ADI.MCA";
                        break;

                    case OperacionSiVale.MisComprasDespensaSaldo:
                        fileHeader = "STK.SALTIT.MIS";
                        break;

                    case OperacionSiVale.MisComprasComidaSaldo:
                        fileHeader = "STK.SALTIT.MCA";
                        break;

                    case OperacionSiVale.MisComprasAlimentacionSaldo:
                        fileHeader = "STK.SALTIT.MCA";
                        break;

                    default:
                        break;
                }
            }
            catch (Exception oe)
            {
                throw oe;
            }
            return fileHeader;
        }

        public static string GetFirstLineSiVale(OperacionSiVale operacion, int count, decimal total)
        {
            string firstLine = string.Empty;
            try
            {
                string numeroCliente = "10146480";
                string tipoRegistro = "03";
                string producto = "59";
                string filler = string.Empty;

                switch (operacion)
                {
                    case OperacionSiVale.MisComprasSolicitud:
                        tipoRegistro = "03";
                        producto = "48";
                        break;

                    case OperacionSiVale.MisComprasSolicitudAdicionales:
                        tipoRegistro = "03";
                        producto = "48";
                        break;

                    case OperacionSiVale.MisComprasAlimentacionSolicitud:
                        tipoRegistro = "03";
                        producto = "59";
                        break;

                    case OperacionSiVale.MisComprasAlimentacionSolicitudAdicionales:
                        tipoRegistro = "03";
                        producto = "59";
                        break;

                    case OperacionSiVale.MisComprasDespensaSaldo:
                        tipoRegistro = "05";
                        producto = "48";
                        break;

                    case OperacionSiVale.MisComprasComidaSaldo:
                        tipoRegistro = "05";
                        producto = "59";
                        break;

                    case OperacionSiVale.MisComprasAlimentacionSaldo:
                        tipoRegistro = "05";
                        producto = "59";
                        break;
                }

                if (tipoRegistro == "03")
                {
                    firstLine = string.Format("{0}{1}{2}{3}",
                                          tipoRegistro, //Tipo de Registro
                                          numeroCliente.PadLeft(8, '0'), //Número del Cliente
                                          count.ToString().PadLeft(7, '0'), //Total de Tarjetas
                                          producto);//Producto
                }
                else
                {
                    firstLine = string.Format("{0}{1}{2}{3}{4}{5}",
                                          tipoRegistro, //Tipo de Registro
                                          numeroCliente.PadLeft(8, '0'), //Número del Cliente
                                          filler.PadLeft(30, ' '), //FILLER
                                          count.ToString().PadLeft(7, '0'), //Total de Empleados
                                          total.ToString("0.00").PadLeft(12, '0'), //Monto total
                                          producto);//Producto
                }
            }
            catch (Exception oe)
            {
                throw oe;
            }
            return firstLine;
        }

        public static string GetLineSiValeEmision(int index, string empleadoID, string nombre)
        {
            string line = string.Empty;
            try
            {
                string lugarEntrega = "1";
                string filler = string.Empty;
                line = string.Format("{0}{1}{2}{3}{4}",
                                    "04", //Tipo de Registro
                                    lugarEntrega.PadLeft(5, '0'),//Lugar de entrega
                                    index.ToString().PadLeft(7, '0'),//Consecutivo de captura
                                    empleadoID.PadLeft(10, '0'),//Referencia
                                    nombre.PadRight(26, ' ')); //Nombre del empleado
            }
            catch (Exception oe)
            {
                throw oe;
            }
            return line;
        }

        public static string GetLineSiValeSaldo(int index, string tarjeta, decimal importe)
        {
            string line = string.Empty;
            try
            {
                string filler = string.Empty;
                line = string.Format("{0}{1}{2}{3}",
                                    "06", //Tipo de Registro
                                    index.ToString().PadLeft(7, '0'),//Consecutivo de captura
                                    tarjeta.PadLeft(16, '0'),//Número de Tarjeta
                                    importe.ToString("0.00").PadLeft(10, '0')); //Importe
            }
            catch (Exception oe)
            {
                throw oe;
            }
            return line;
        }
    }
}