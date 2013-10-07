using System;
using System.Data;
using System.Text;
using BDOPayrollReporter.Business.Enums;

namespace BDOPayrollReporter.Business
{
    public class TextRenderer
    {
        public byte[] RenderLayoutSiValeAlta(int Año, int Periodo)
        {
            int registro;
            DateTime fecha;
            DataTable datos;
            System.IO.MemoryStream mst;
            System.IO.StreamWriter stw;
            DataAccess SqlHelper;
            try
            {
                mst = new System.IO.MemoryStream();
                stw = new System.IO.StreamWriter(mst);

                SqlHelper = new DataAccess();
                datos = SqlHelper.RenderLayoutSiValeAlta(Año, Periodo);

                if (datos.Rows.Count > 0)
                {
                    registro = 0;
                    fecha = DateTime.Now;

                    stw.Write("05"); //Tipo de Registro 2
                    stw.Write("1014648"); //Número del Cliente 7
                    stw.Write("HOSPIRA S. DE R.L. DE C.V.    "); //Nombre del Cliente 30
                    stw.Write(datos.Rows.Count.ToString().PadLeft(7, '0').Substring(0, 7)); //Total de Empleados 7
                    stw.Write(string.Format("{0}-{1}-{2}", fecha.Day.ToString().PadLeft(2, '0').Substring(0, 2), System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(fecha.Month).ToString().Substring(0, 3).ToUpper(), fecha.Year.ToString().Substring(2, 2)).PadLeft(9, '0').Substring(0, 9)); //Fecha de envío 9

                    foreach (DataRow row in datos.Rows)
                    {
                        stw.WriteLine();
                        stw.Write("04"); //Tipo de Registro 2
                        stw.Write("00001"); //Lugar de Entrega 5
                        stw.Write(row["Centro"].ToString().PadRight(5, ' ').Substring(0, 5)); //Área, Departamento o Centro de Pago.	5
                        stw.Write((++registro).ToString().PadLeft(7, '0').Substring(0, 7)); //Consecutivo de captura 7
                        stw.Write(row["Empleado"].ToString().PadLeft(10, '0').Substring(0, 10)); //Número de empleado 10
                        stw.Write(row["Nombre"].ToString().PadRight(26, ' ').Substring(0, 26)); //Nombre del empleado.	26
                        stw.Write(row["Rfc"].ToString().PadRight(15, ' ').Substring(0, 15)); //R.F.C.	15
                        stw.Write("0"); //Tarjeta Adicional	1
                    }
                    stw.Flush();
                }
            }
            catch (Exception ex)
            {
                System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
                return encoding.GetBytes(ex.Message);
            }

            return mst.ToArray();
        }

        public byte[] RenderLayoutSiVale(int Año, int Periodo, DateTime FechaDispersion, short Tipo, bool Abierta)
        {
            int importe, registro;
            DataTable datos;
            System.IO.MemoryStream mst;
            System.IO.StreamWriter stw;
            DataAccess SqlHelper;
            try
            {
                mst = new System.IO.MemoryStream();
                stw = new System.IO.StreamWriter(mst);

                SqlHelper = new DataAccess();
                datos = SqlHelper.RenderLayoutSiVale(Año, Periodo, Tipo, Abierta, 32);

                if (datos.Rows.Count > 0)
                {
                    try
                    {
                        importe = System.Convert.ToInt32(Double.Parse(datos.Compute("SUM(Importe)", "").ToString()));
                    }
                    catch (Exception)
                    {
                        importe = 0;
                    }
                    registro = 0;

                    stw.Write("05"); //Tipo de Registro 2
                    stw.Write("1014648"); //Número del Cliente 7
                    stw.Write("HOSPIRA S. DE R.L. DE C.V.    "); //Nombre del Cliente 30
                    stw.Write(datos.Rows.Count.ToString().PadLeft(7, '0').Substring(0, 7)); //Total de Empleados 7
                    stw.Write((importe).ToString().PadLeft(9, '0').Substring(0, 9) + ".00"); //Monto total 12
                    stw.Write("Despensa".PadLeft(8, '0').Substring(0, 8)); //Tipo de Producto 8
                    stw.Write(string.Format("{0}-{1}-{2}", FechaDispersion.Day.ToString().PadLeft(2, '0').Substring(0, 2), System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(FechaDispersion.Month).ToString().Substring(0, 3).ToUpper(), FechaDispersion.Year.ToString().Substring(2, 2)).PadLeft(9, '0').Substring(0, 9)); //Fecha de envío 9

                    foreach (DataRow row in datos.Rows)
                    {
                        importe = System.Convert.ToInt32(Double.Parse(row["Importe"].ToString()));
                        stw.WriteLine();
                        stw.Write("06"); //Tipo de Registro 2
                        stw.Write("00001"); //Lugar de Entrega 5
                        stw.Write(row["Cuenta"].ToString().PadLeft(8, '0').Substring(0, 8)); //Número de Cuenta 8
                        stw.Write((++registro).ToString().PadLeft(7, '0').Substring(0, 7)); //Consecutivo de captura 7
                        stw.Write(row["Empleado"].ToString().PadLeft(10, '0').Substring(0, 10)); //Número de empleado 10
                        stw.Write(row["Nombre"].ToString().PadRight(26, ' ').Substring(0, 26)); //Nombre del empleado 26
                        stw.Write((importe).ToString().PadLeft(7, '0').Substring(0, 7) + ".00"); //Importe 10
                    }
                    stw.Flush();
                }
            }
            catch (Exception ex)
            {
                System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
                return encoding.GetBytes(ex.Message);
            }

            return mst.ToArray();
        }

        public byte[] RenderLayoutCitybank484(int Año, int Periodo, DateTime FechaDispersion, short Tipo, bool Abierta, bool EsPrueba, int contador)
        {
            int registro;
            decimal importeTotal;
            decimal importe;
            DataTable datos;
            System.IO.MemoryStream mst;
            System.IO.StreamWriter stw;
            DataAccess SqlHelper;
            try
            {
                mst = new System.IO.MemoryStream();
                stw = new System.IO.StreamWriter(mst);

                SqlHelper = new DataAccess();
                datos = SqlHelper.RenderLayoutCitybank484(Año, Periodo, Tipo, Abierta, EsPrueba);

                if (datos.Rows.Count > 0)
                {
                    try
                    {
                        importeTotal = System.Convert.ToDecimal(decimal.Parse(datos.Compute("SUM(Importe)", "").ToString()));
                    }
                    catch (Exception)
                    {
                        importeTotal = 0;
                    }

                    registro = 0;

                    foreach (DataRow row in datos.Rows)
                    {
                        importe = decimal.Parse(row["Importe"].ToString());
                        stw.Write("PAY"); //Tipo de Registro 1 3
                        stw.Write("484"); //Código de País del Cliente 4 3
                        stw.Write(row["CuentaCliente"].ToString().PadRight(10, ' ').Substring(0, 10)); //Número de cuenta del Cliente	7	10
                        stw.Write(string.Format("{0}{1}{2}", FechaDispersion.Year.ToString().Substring(2, 2), FechaDispersion.Month.ToString().PadLeft(2, '0').Substring(0, 2).ToUpper(), FechaDispersion.Day.ToString().PadLeft(2, '0').Substring(0, 2)).PadLeft(6, '0').Substring(0, 6)); //Fecha de Transacción	17	6
                        stw.Write("071"); //Código de Transacción	23	3
                        stw.Write(string.Format("NOMINA{0}{1}{2}{3}", FechaDispersion.Day.ToString().PadLeft(2, '0').Substring(0, 2), System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(FechaDispersion.Month).ToString().Substring(0, 3).ToUpper(), FechaDispersion.Year.ToString().Substring(2, 2), (++registro).ToString().PadLeft(2, '0').Substring(0, 2)).PadRight(15, '0').Substring(0, 15)); //Referencia de la transacción  del cliente	26	15
                        stw.Write((++contador).ToString().PadLeft(8, '0').Substring(0, 8)); //Número de secuencia de la transacción	41	8
                        stw.Write(row["RFC"].ToString().PadRight(20, ' ').Substring(0, 20)); //RFC del Beneficiario 	49	20	A
                        stw.Write("MXN"); //Moneda del número de cuenta del cliente	69	3	A
                        stw.Write(row["Empleado"].ToString().PadRight(20, ' ').Substring(0, 20)); //Código del Beneficiario	72	20	A
                        stw.Write(String.Format("{0:0.00}", importe).Replace(".", "").PadLeft(15, '0').Substring(0, 15)); //Importe de la Transacción	92	15	N
                        stw.Write("      "); //Maturity Date 	107	6	D
                        stw.Write("                                   "); //Detalles de la Transacción Línea 1	113	35	A
                        stw.Write("                                   "); //Detalles de la Transacción Línea 2	148	35	A
                        stw.Write("                                   "); //Detalles de la Transacción Línea 3	183	35	A
                        stw.Write("                                   "); //Detalles de la Transacción Línea 4	218	35	A
                        stw.Write("05"); //Código de Transacción Local  	253	2	N
                        stw.Write("01"); //Tipo de cuenta del cliente 	255	2	N
                        stw.Write(row["NombreEmpleado"].ToString().PadRight(80, ' ').Substring(0, 80)); //Nombre del Beneficiario	257	80	A
                        stw.Write(row["Calle"].ToString().PadRight(35, ' ').Substring(0, 35)); //Dirección del Beneficiario 1	337	35	A
                        stw.Write(row["Colonia"].ToString().PadRight(35, ' ').Substring(0, 35)); //Dirección del Beneficiario 2	372	35	A
                        stw.Write(row["Municipio"].ToString().PadRight(15, ' ').Substring(0, 15)); //Ciudad del Beneficiario	407	15	A
                        stw.Write("24"); //Código del Estado	422	2	A
                        stw.Write(row["CodigoPostal"].ToString().PadLeft(12, ' ').Substring(0, 12)); //Código Postal	424	12	A
                        stw.Write("0000000000000000"); //Número telefónico del Beneficiario	436	16	N
                        stw.Write(row["Banco"].ToString().PadLeft(3, '0').Substring(0, 3)); //Código  del Banco del Beneficiario	452	3	N
                        stw.Write("001     "); //Agencia del Banco del Beneficiario 	455	8	A
                        string cuentaEmpleado = row["CuentaEmpleado"].ToString();
                        int len = cuentaEmpleado.Length;
                        stw.Write(cuentaEmpleado.PadRight(35, ' ').Substring(0, 35)); //Número de cuenta del Beneficiario 463	35	A
                        stw.Write(len == 18 ? "05" : "02"); //Tipo de cuenta del Beneficiario   498	2	N
                        stw.Write("MEXICO                        "); //Dirección del Banco 	C	500	30
                        stw.Write("01"); //Número de Entidad	530	2
                        stw.Write("001"); //Número de Localidad	532	3
                        stw.Write("MEXICO        "); //Nombre de la Localidad	535	14
                        stw.Write("001"); //Número de la sucursal	549	3
                        stw.Write("MEXICO             "); //Nombre de la sucursal 	552	19
                        stw.Write("                "); //Fax del Beneficiario	571	16
                        stw.Write("                    "); //Contacto del Fax del Beneficiario	587	20
                        stw.Write("               "); //Departamento del Fax del Beneficiario	607	15
                        stw.Write("          "); //Número de cuenta del Beneficiario (Para cuentas que son Tipo Citibank)	622	10
                        stw.Write("  "); //Tipo de cuenta (Para cuentas que son Tipo Citibank)	632	2
                        stw.Write("001"); //Método de entrega de transacciones 	634	3
                        stw.Write("                                                  "); //Identificación de un titulo	637	50
                        stw.Write("     "); //Código de activación del beneficiario	687	5
                        stw.Write("                                                  "); //Correo electrónico del Beneficiario	692	50
                        stw.Write("999999999999999"); //Importe máximo por Pago	742	15
                        stw.Write(" "); //Actualización del registro de Pago	757	1
                        stw.Write("00000000000"); //Número de cheque o Número de Transferencia	758	11
                        stw.Write(" "); //Marca del Cheque	769	1
                        stw.Write(" "); //Bandera del Cheque	770	1
                        stw.Write("".PadLeft(254)); //254 caracteres en blanco	771

                        stw.WriteLine();
                    }

                    stw.Write("TRL"); //Tipo de Registro 3
                    stw.Write(datos.Rows.Count.ToString().PadLeft(15, '0').Substring(0, 15)); //Número de transacciones 15
                    stw.Write(String.Format("{0:0.00}", importeTotal).Replace(".", "").PadLeft(15, '0').Substring(0, 15)); //Suma de todos los importes del registro de pagos 15
                    stw.Write("000000000000000"); //Número de registros de beneficiario 15
                    stw.Write(datos.Rows.Count.ToString().PadLeft(15, '0').Substring(0, 15)); //Numero total de registros a transmitir 15
                    stw.Write("                                     "); //Reservado 37
                    stw.Flush();
                }
            }
            catch (Exception ex)
            {
                System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
                return encoding.GetBytes(ex.Message);
            }

            return mst.ToArray();
        }

        public byte[] RenderLayoutCitybank485(int Año, int Periodo, DateTime FechaDispersion, short Tipo, bool Abierta, bool EsPrueba, int contador)
        {
            int registro;
            decimal importeTotal;
            decimal importe;
            DataTable datos;
            System.IO.MemoryStream mst;
            System.IO.StreamWriter stw;
            DataAccess SqlHelper;
            try
            {
                mst = new System.IO.MemoryStream();
                stw = new System.IO.StreamWriter(mst);

                SqlHelper = new DataAccess();
                datos = SqlHelper.RenderLayoutCitybank485(Año, Periodo, Tipo, Abierta, EsPrueba);

                if (datos.Rows.Count > 0)
                {
                    try
                    {
                        importeTotal = System.Convert.ToDecimal(decimal.Parse(datos.Compute("SUM(Importe)", "").ToString()));
                    }
                    catch (Exception)
                    {
                        importeTotal = 0;
                    }

                    registro = 0;

                    foreach (DataRow row in datos.Rows)
                    {
                        string cuentaEmpleado = row["CuentaEmpleado"].ToString();
                        int len = cuentaEmpleado.Length;
                        importe = decimal.Parse(row["Importe"].ToString());

                        stw.Write("PAY"); //Tipo de Registro 1 3
                        stw.Write("485"); //Código de País del Cliente 4 3
                        stw.Write(string.Empty.PadRight(10, ' ').Substring(0, 10)); //Número de cuenta del Cliente	7	10
                        stw.Write(string.Format("{0}{1}{2}", FechaDispersion.Year.ToString().Substring(2, 2), FechaDispersion.Month.ToString().PadLeft(2, '0').Substring(0, 2).ToUpper(), FechaDispersion.Day.ToString().PadLeft(2, '0').Substring(0, 2)).PadLeft(6, '0').Substring(0, 6)); //Fecha de Transacción	17	6
                        stw.Write("001"); //Código de Transacción	23	3
                        stw.Write(string.Format("NOMINA{0}{1}{2}{3}", FechaDispersion.Day.ToString().PadLeft(2, '0').Substring(0, 2), System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(FechaDispersion.Month).ToString().Substring(0, 3).ToUpper(), FechaDispersion.Year.ToString().Substring(2, 2), (++registro).ToString().PadLeft(2, '0').Substring(0, 2)).PadRight(15, '0').Substring(0, 15)); //Referencia de la transacción  del cliente	26	15
                        stw.Write((++contador).ToString().PadLeft(8, '0').Substring(0, 8)); //Número de secuencia de la transacción	41	8
                        stw.Write(row["RFC"].ToString().PadRight(20, ' ').Substring(0, 20)); //RFC del Beneficiario 	49	20	A
                        stw.Write("MXN"); //Moneda del número de cuenta del cliente	69	3	A
                        stw.Write(row["Empleado"].ToString().PadRight(20, ' ').Substring(0, 20)); //Código del Beneficiario	72	20	A
                        stw.Write(string.Format("{0:0.00}", importe).Replace(".", "").PadLeft(15, '0').Substring(0, 15)); //Importe de la Transacción	92	15	N
                        stw.Write(string.Empty.PadRight(6, ' ')); //Maturity Date 	107	6	D
                        stw.Write(string.Empty.PadRight(35, ' ')); //Detalles de la Transacción Línea 1	113	35	A
                        stw.Write(string.Empty.PadRight(35, ' ')); //Detalles de la Transacción Línea 2	148	35	A
                        stw.Write(string.Empty.PadRight(35, ' ')); //Detalles de la Transacción Línea 3	183	35	A
                        stw.Write(string.Empty.PadRight(35, ' ')); //Detalles de la Transacción Línea 4	218	35	A
                        stw.Write("05"); //Código de Transacción Local  	253	2	N
                        stw.Write("01"); //Tipo de cuenta del cliente 	255	2	N
                        stw.Write(row["NombreEmpleado"].ToString().PadRight(80, ' ').Substring(0, 80)); //Nombre del Beneficiario	257	80	A
                        stw.Write(row["Calle"].ToString().PadRight(35, ' ').Substring(0, 35)); //Dirección del Beneficiario 1	337	35	A
                        stw.Write(row["Colonia"].ToString().PadRight(35, ' ').Substring(0, 35)); //Dirección del Beneficiario 2	372	35	A
                        stw.Write(string.Empty.PadRight(15, ' ')); //Ciudad del Beneficiario	407	15	A
                        stw.Write(string.Empty.PadRight(2, ' ')); //Código del Estado	422	2	A
                        stw.Write(string.Empty.PadRight(12, ' ')); //Código Postal	424	12	A
                        stw.Write(string.Empty.PadRight(16, ' ')); //Número telefónico del Beneficiario	436	16	N
                        stw.Write(row["Banco"].ToString().PadLeft(3, '0').Substring(0, 3)); //Código  del Banco del Beneficiario	452	3	N
                        stw.Write(string.Empty.PadRight(8, ' ')); //Agencia del Banco del Beneficiario 	455	8	A
                        stw.Write(cuentaEmpleado.PadRight(35, ' ').Substring(0, 35)); //Número de cuenta del Beneficiario 463	35	A
                        stw.Write(len == 18 ? "05" : "03"); //Tipo de cuenta del Beneficiario   498	2	N
                        stw.Write(string.Empty.PadRight(30, ' '));//Dirección del Banco 	C	500	30
                        stw.Write(string.Empty.PadRight(17, ' '));//O 530 17 Importe del Impuesto
                        stw.Write("N");//O 547 1 Bandera de Prioridad
                        stw.Write("N");//O 548 1 Confidencial
                        stw.Write(len == 18 ? "  " : "01"); // O 549 2 Fecha de Acreditación al Beneficiario
                        stw.Write(row["CuentaCliente"].ToString().PadRight(20, ' ').Substring(0, 20)); //M 551 20 Número de cuenta de débito Banamex
                        stw.Write(string.Empty.PadRight(16, ' '));//O 571 16 Fax del Beneficiario
                        stw.Write(string.Empty.PadRight(20, ' '));//O 587 20 Contacto del Fax del Beneficiario
                        stw.Write(string.Empty.PadRight(15, ' '));//O 607 15 Departamento del Fax del Beneficiario
                        stw.Write(string.Empty.PadRight(10, ' '));//O 622 10 Número de cuenta del Beneficiario
                        stw.Write(string.Empty.PadRight(2, ' '));//O 632  2 Tipo de cuenta
                        stw.Write(string.Empty.PadRight(3, ' '));//C 634 3 Método de entrega de transacciones
                        stw.Write(string.Empty.PadRight(50, ' '));//O 637 50 Identificación de un titulo
                        stw.Write("00000");//O 687  5 Código de activación del beneficiario
                        stw.Write(string.Empty.PadRight(50, ' '));//O 692 50 Correo electrónico del Beneficiario
                        stw.Write("999999999999999");//M 742 15 Importe máximo por Pago
                        stw.Write(string.Empty.PadRight(1, ' '));//O 757  1 Actualización del registro de Pago
                        stw.Write("NONE".PadRight(11, ' '));//O 758 11 Notificación al Beneficiario
                        stw.Write(string.Empty.PadRight(1, ' '));//O 769  1 Marca del Cheque
                        stw.Write(string.Empty.PadRight(1, ' '));//O 770  1 Bandera del Cheque
                        stw.Write(string.Empty.PadLeft(253, ' '));//M 771 253
                        stw.WriteLine();
                    }

                    stw.Write("TRL"); //Tipo de Registro 3
                    stw.Write(datos.Rows.Count.ToString().PadLeft(15, '0').Substring(0, 15)); //Número de transacciones 15
                    stw.Write(String.Format("{0:0.00}", importeTotal).Replace(".", "").PadLeft(15, '0').Substring(0, 15)); //Suma de todos los importes del registro de pagos 15
                    stw.Write("000000000000000"); //Número de registros de beneficiario 15
                    stw.Write(datos.Rows.Count.ToString().PadLeft(15, '0').Substring(0, 15)); //Numero total de registros a transmitir 15
                    stw.Write(string.Empty.PadRight(37)); //Reservado 37
                    stw.Flush();
                }
            }
            catch (Exception ex)
            {
                System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
                return encoding.GetBytes(ex.Message);
            }

            return mst.ToArray();
        }

        public byte[] RenderLayoutBanorteAlta(int Año, int Periodo)
        {
            double sueldo;
            DateTime fechaingreso;
            DataTable datos;
            System.IO.MemoryStream mst;
            System.IO.StreamWriter stw;
            DataAccess SqlHelper;
            try
            {
                mst = new System.IO.MemoryStream();
                stw = new System.IO.StreamWriter(mst);

                SqlHelper = new DataAccess();
                datos = SqlHelper.RenderLayoutBanorteAlta(Año, Periodo);

                if (datos.Rows.Count > 0)
                {
                    foreach (DataRow row in datos.Rows)
                    {
                        sueldo = Double.Parse(row["Sueldo"].ToString());
                        fechaingreso = DateTime.Parse(row["FechaIngreso"].ToString());
                        stw.Write(row["Empleado"].ToString().PadRight(16, ' ').Substring(0, 16)); //PARTICIPANTE	Caracter	16
                        stw.Write(row["Rfc"].ToString().PadRight(20, ' ').Substring(0, 20)); //REG.FED.CAU.	20
                        stw.Write(row["Empleado"].ToString().PadRight(6, ' ').Substring(0, 6)); //NÚM. EMPLEADO	6
                        stw.Write("00000006"); //NUM.EMPRESA	8
                        stw.Write(string.Format("{0}{1}{2}", fechaingreso.Year.ToString(), fechaingreso.Month.ToString().PadLeft(2, '0').Substring(0, 2).ToUpper(), fechaingreso.Day.ToString().PadLeft(2, '0').Substring(0, 2)).PadLeft(8, '0').Substring(0, 8)); //FECHA DE ALTA	8
                        stw.Write(row["NombreEmpleado"].ToString().PadRight(16, ' ').Substring(0, 16)); //NOMBRE1	16
                        stw.Write("X               "); //NOMBRE2	16
                        stw.Write(row["ApellidoPaternoEmpleado"].ToString().PadRight(16, ' ').Substring(0, 16)); //APELLIDO1	16
                        stw.Write(row["ApellidoMaternoEmpleado"].ToString().PadRight(16, ' ').Substring(0, 16)); //APELLIDO2	16
                        stw.Write("E"); //TPO DE NOMINA	1
                        stw.Write("M"); //TPO DE APORT.	1
                        stw.Write("C"); //TPO DE PAGO	1
                        stw.Write("                "); //CTA DE DEPOSITO	16
                        stw.Write("21"); //CVE BANCO	2
                        stw.Write("9999"); //SUCURSAL	4
                        stw.Write(String.Format("{0:0.00}", sueldo).PadLeft(20, '0').Substring(0, 20)); //SUELDO (MENSUAL)	20
                        stw.Write("*"); //CONSTANTE	1
                        stw.WriteLine();
                    }
                    stw.Flush();
                }
            }
            catch (Exception ex)
            {
                System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
                return encoding.GetBytes(ex.Message);
            }

            return mst.ToArray();
        }

        public byte[] RenderLayoutBanorte(int Año, int Periodo, DateTime FechaDispersion, short Tipo, bool Abierta)
        {
            double importe;
            DataTable datos;
            System.IO.MemoryStream mst;
            System.IO.StreamWriter stw;
            DataAccess SqlHelper;
            try
            {
                mst = new System.IO.MemoryStream();
                stw = new System.IO.StreamWriter(mst);

                SqlHelper = new DataAccess();
                datos = SqlHelper.RenderLayoutBanorte(Año, Periodo, Tipo, Abierta);

                if (datos.Rows.Count > 0)
                {
                    foreach (DataRow row in datos.Rows)
                    {
                        importe = Double.Parse(row["Importe"].ToString());
                        stw.Write(row["Concepto"].ToString().Equals("621") ? "02" : "01"); //TPO DE MOVIM.	2
                        stw.Write(row["Empleado"].ToString().PadRight(16, ' ').Substring(0, 16)); //PARTICIPANTE	16
                        //stw.Write(String.Format("{0}{1}{2}", FechaDispersion.Year.ToString(), FechaDispersion.Month.ToString().PadLeft(2, '0').Substring(0, 2).ToUpper(), FechaDispersion.Day.ToString().PadLeft(2, '0').Substring(0, 2)).PadLeft(8, '0').Substring(0, 8)); //FECHA	8

                        //stw.Write(String.Format("{0}{1}{2}/{1}/{0}",
                        stw.Write(String.Format("{0}{1}{2}",
                            FechaDispersion.Year.ToString(),
                            FechaDispersion.Month.ToString().PadLeft(2, '0'),
                            FechaDispersion.Day.ToString().PadLeft(2, '0'))); //FECHA	8

                        stw.Write(String.Format("{0:0.00}", importe).PadLeft(20, '0').Substring(0, 20)); //IMPORTE	20
                        stw.Write("M"); //INDICADOR	1
                        stw.Write("000.00"); //TASA	6
                        stw.Write("000"); //PLAZO	3
                        stw.Write("00"); //FOLIO	2
                        stw.Write("00"); //NUM. DE PAGOS	2
                        stw.Write("0               "); //NUM. DE AVAL	16
                        stw.Write("*"); //CONSTANTE	1
                        stw.WriteLine();
                    }
                    stw.Flush();
                }
            }
            catch (Exception ex)
            {
                System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
                return encoding.GetBytes(ex.Message);
            }

            return mst.ToArray();
        }

        public byte[] RenderLayoutSkandiaAlta(int Año, int Periodo)
        {
            int total;
            double sueldo;
            DateTime fechanacimiento, fechaingreso;
            DataTable datos;
            System.IO.MemoryStream mst;
            System.IO.StreamWriter stw;
            DataAccess SqlHelper;
            try
            {
                total = 0;
                mst = new System.IO.MemoryStream();
                stw = new System.IO.StreamWriter(mst);

                SqlHelper = new DataAccess();
                datos = SqlHelper.RenderLayoutSkandiaAlta(Año, Periodo);

                if (datos.Rows.Count > 0)
                {
                    foreach (DataRow row in datos.Rows)
                    {
                        sueldo = Double.Parse(row["Sueldo"].ToString());
                        fechanacimiento = DateTime.Parse(row["FechaNacimiento"].ToString());
                        fechaingreso = DateTime.Parse(row["FechaIngreso"].ToString());
                        stw.Write("01"); //Código de Registro	2
                        stw.Write("C00128"); //Clave de Identificación del Plan	6
                        stw.Write("         "); //Número de Identificación del Participante	9
                        stw.Write("161 "); //Código Empresa, Planta ó División	4
                        stw.Write(row["Empleado"].ToString().PadRight(13, ' ').Substring(0, 13)); //Número de empleado	13
                        stw.Write(row["Curp"].ToString().PadRight(18, ' ').Substring(0, 18)); //CURP o RFC	18
                        stw.Write(row["NombreEmpleado"].ToString().PadRight(30, ' ').Substring(0, 30)); //Apellido Paterno	30
                        stw.Write(row["ApellidoPaternoEmpleado"].ToString().PadRight(30, ' ').Substring(0, 30)); //Apellido Materno	30
                        stw.Write(row["ApellidoMaternoEmpleado"].ToString().PadRight(20, ' ').Substring(0, 20)); //Nombre(s)	20
                        stw.Write(string.Format("{0}{1}{2}", fechanacimiento.Day.ToString().PadLeft(2, '0').Substring(0, 2), fechanacimiento.Month.ToString().PadLeft(2, '0').Substring(0, 2).ToUpper(), fechanacimiento.Year.ToString()).PadLeft(8, '0').Substring(0, 8)); //Fecha de nacimiento	8
                        stw.Write(string.Format("{0}{1}{2}", fechaingreso.Day.ToString().PadLeft(2, '0').Substring(0, 2), fechaingreso.Month.ToString().PadLeft(2, '0').Substring(0, 2).ToUpper(), fechaingreso.Year.ToString()).PadLeft(8, '0').Substring(0, 8)); //Fecha de Contratación	8
                        stw.Write(string.Format("{0}{1}{2}", fechaingreso.Day.ToString().PadLeft(2, '0').Substring(0, 2), fechaingreso.Month.ToString().PadLeft(2, '0').Substring(0, 2).ToUpper(), fechaingreso.Year.ToString()).PadLeft(8, '0').Substring(0, 8)); //Fecha de Ingreso al Plan	8
                        stw.Write((System.Convert.ToInt32(row["IdSexo"]) + 1).ToString().PadRight(1, '0').Substring(0, 1)); //Sexo	1
                        stw.Write(row["Calle"].ToString().PadRight(40, ' ').Substring(0, 40)); //Dirección 1	40
                        stw.Write("                                        "); //Dirección 2	40
                        stw.Write(row["Ciudad"].ToString().PadRight(28, ' ').Substring(0, 28)); //Ciudad	28
                        stw.Write(row["Colonia"].ToString().PadRight(28, ' ').Substring(0, 28)); //Colonia	28
                        stw.Write(row["Delegacion"].ToString().PadRight(28, ' ').Substring(0, 28)); //Delegación	28
                        stw.Write(row["Estado"].ToString().PadRight(3, ' ').Substring(0, 3)); //Estado	3
                        stw.Write(row["CodigoPostal"].ToString().PadRight(9, ' ').Substring(0, 9)); //Código Postal	9
                        stw.Write("                                                  "); //Correo Electrónico	50
                        stw.Write(row["CuentaBancaria"].ToString().PadRight(18, ' ').Substring(0, 18)); //Número de cuenta bancaria/CLABE	18
                        stw.Write(row["Banco"].ToString().PadRight(30, ' ').Substring(0, 30)); //Banco 30
                        stw.Write("    "); //Sucursal Banco	4
                        stw.Write(String.Format("{0:0.00}", sueldo).PadLeft(12, '0').Substring(0, 12)); //Salario Base (Mensual)	12
                        stw.Write("4"); //Frecuencia de Pago	1
                        stw.Write(row["CentroCosto"].ToString().PadRight(10, ' ').Substring(0, 10)); //Centro de Costos	10
                        stw.Write("          "); //Clave RH	10
                        stw.Write("             "); //Reservado	13
                        stw.Write(row["Rfc"].ToString().PadRight(18, ' ').Substring(0, 18)); //Clave usuario SkandiaNet	18
                        stw.WriteLine();
                        total++;
                    }

                    stw.Write("51"); //Código de Registro	2
                    stw.Write(total.ToString().PadLeft(6, '0').Substring(0, 6)); //Número de Registros	6
                    stw.Write("                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                           "); //Espacios en blanco

                    stw.Flush();
                }
            }
            catch (Exception ex)
            {
                System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
                return encoding.GetBytes(ex.Message);
            }

            return mst.ToArray();
        }

        public byte[] RenderLayoutSkandiaAportaciones(int Año, int Periodo, DateTime FechaDispersion, short Tipo, bool Abierta)
        {
            int total;
            double importeempleado, importeempresa, importeadicional, importetotal;
            DataTable datos;
            System.IO.MemoryStream mst;
            System.IO.StreamWriter stw;
            DataAccess SqlHelper;
            try
            {
                total = 0;
                importetotal = 0;
                mst = new System.IO.MemoryStream();
                stw = new System.IO.StreamWriter(mst);

                SqlHelper = new DataAccess();
                datos = SqlHelper.RenderLayoutSkandiaAportaciones(Año, Periodo, Tipo, Abierta);

                if (datos.Rows.Count > 0)
                {
                    foreach (DataRow row in datos.Rows)
                    {
                        importeempleado = Double.Parse(row["ImporteEmpleado"].ToString());
                        importeempresa = Double.Parse(row["ImporteEmpresa"].ToString());
                        importeadicional = Double.Parse(row["ImporteAdicional"].ToString());
                        stw.Write("04"); //Código de Registro	2
                        stw.Write("C00128"); //Clave de Identificación del Plan	6
                        stw.Write("         "); //Número de Identificacióndel Participante	9
                        stw.Write(row["Empleado"].ToString().PadRight(13, ' ').Substring(0, 13)); //Número de empleado	13
                        stw.Write(row["Curp"].ToString().PadRight(18, ' ').Substring(0, 18)); //CURP o RFC	18
                        stw.Write(string.Format("{0}{1}{2}", FechaDispersion.Day.ToString().PadLeft(2, '0').Substring(0, 2), FechaDispersion.Month.ToString().PadLeft(2, '0').Substring(0, 2).ToUpper(), FechaDispersion.Year.ToString()).PadLeft(8, '0').Substring(0, 8)); //Fecha de la Aportación	8
                        stw.Write("A"); //Clave Subcuenta 1	1
                        stw.Write(String.Format("{0:0.00}", importeempleado).PadLeft(10, '0').Substring(0, 10)); //Importe aportacion Subcuenta 1	10
                        stw.Write("D"); //Clave Subcuenta 2	1
                        stw.Write(String.Format("{0:0.00}", importeempresa).PadLeft(10, '0').Substring(0, 10)); //Importe aportacion Subcuenta 2	10
                        stw.Write("F"); //Clave Subcuenta 3	1
                        stw.Write(String.Format("{0:0.00}", importeadicional).PadLeft(10, '0').Substring(0, 10)); //Importe aportacion Subcuenta 3	10
                        stw.Write("S"); //Clave Subcuenta 4	1
                        stw.Write("0000000.00"); //Importe aportacion Subcuenta 4	10
                        stw.Write("T"); //Clave Subcuenta 5	1
                        stw.Write("0000000.00"); //Importe aportacion Subcuenta 5	10
                        stw.Write(" "); //Clave Subcuenta 6	1
                        stw.Write("0000000.00"); //Importe aportacion Subcuenta 6	10
                        stw.Write("          "); //Clave RH	10
                        stw.Write(row["CentroCosto"].ToString().PadRight(13, ' ').Substring(0, 13)); //Código Empresa, Planta ó División	13

                        stw.WriteLine();
                        total++;
                        importetotal = importetotal + (importeempleado + importeempresa + importeadicional);
                    }

                    stw.Write("54"); //Código de Registro	2
                    stw.Write(total.ToString().PadLeft(6, '0').Substring(0, 6)); //Número de Registros	6
                    stw.Write(String.Format("{0:0.00}", importetotal).PadLeft(16, '0').Substring(0, 16)); //Importe Total de las Aportaciones	16
                    stw.Write("                                                                                                                         "); //Espacios en blanco

                    stw.Flush();
                }
            }
            catch (Exception ex)
            {
                System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
                return encoding.GetBytes(ex.Message);
            }

            return mst.ToArray();
        }

        public byte[] RenderLayoutSkandiaBajas(int Año, int Periodo)
        {
            String motivobaja;
            DateTime fecha;
            DataTable datos;
            System.IO.MemoryStream mst;
            System.IO.StreamWriter stw;
            DataAccess SqlHelper;
            try
            {
                mst = new System.IO.MemoryStream();
                stw = new System.IO.StreamWriter(mst);

                SqlHelper = new DataAccess();
                datos = SqlHelper.RenderLayoutSkandiaBajas(Año, Periodo);
                fecha = DateTime.Now;

                if (datos.Rows.Count > 0)
                {
                    foreach (DataRow row in datos.Rows)
                    {
                        if (row["MotivoBaja"].ToString().Equals("101") || row["MotivoBaja"].ToString().Equals("102"))
                            motivobaja = "0";
                        else
                            motivobaja = "2";

                        stw.Write("01"); //Código de Registro	2
                        stw.Write("C00128"); //Clave de Identificación del Plan	6
                        stw.Write("         "); //Número de Identificación del Participante	9
                        stw.Write(row["Empleado"].ToString().PadRight(13, ' ').Substring(0, 13)); //Número de empleado	13
                        stw.Write(row["Curp"].ToString().PadRight(18, ' ').Substring(0, 18)); //CURP o RFC	18
                        stw.Write(row["NombreEmpleado"].ToString().PadRight(30, ' ').Substring(0, 30)); //Apellido Paterno	30
                        stw.Write(row["ApellidoPaternoEmpleado"].ToString().PadRight(30, ' ').Substring(0, 30)); //Apellido Materno	30
                        stw.Write(row["ApellidoMaternoEmpleado"].ToString().PadRight(20, ' ').Substring(0, 20)); //Nombre(s)	20
                        stw.Write(string.Format("{0}{1}{2}", fecha.Day.ToString().PadLeft(2, '0').Substring(0, 2), fecha.Month.ToString().PadLeft(2, '0').Substring(0, 2).ToUpper(), fecha.Year.ToString()).PadLeft(8, '0').Substring(0, 8)); //Fecha de baja	8
                        stw.Write(motivobaja.ToString()); //Motivo baja	1
                        stw.WriteLine();
                    }

                    stw.Write("53"); //Código de Registro	2
                    stw.Write(datos.Rows.Count.ToString().PadLeft(6, '0').Substring(0, 6)); //Número de Registros	6
                    stw.Write("                                                                                                                                 "); //Espacios en blanco
                    stw.Flush();
                }
            }
            catch (Exception ex)
            {
                System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
                return encoding.GetBytes(ex.Message);
            }

            return mst.ToArray();
        }

        public byte[] RenderLayoutInboundSap(int Año, int Periodo, DateTime FechaNomina, short Tipo, bool Abierta)
        {
            Double importeempleado;
            DateTime inicio, fin;
            DataTable datos;
            System.IO.MemoryStream mst;
            System.IO.StreamWriter stw;
            DataAccess SqlHelper;
            try
            {
                mst = new System.IO.MemoryStream();
                stw = new System.IO.StreamWriter(mst);

                SqlHelper = new DataAccess();
                datos = SqlHelper.RenderLayoutInboundSap(Año, Periodo, Tipo, Abierta);

                inicio = new DateTime(Año, Periodo, 1);
                fin = inicio.AddMonths(1).AddDays(-1);

                if (datos.Rows.Count > 0)
                {
                    foreach (DataRow row in datos.Rows)
                    {
                        DateTime fecha = Convert.ToDateTime(row["FechaIngreso"].ToString());
                        if (fecha >= inicio && fecha <= fin)
                        {
                            inicio = fecha;
                        }

                        importeempleado = Double.Parse(row["Importe"].ToString());
                        stw.Write(row["Empleado"].ToString().PadRight(8, ' ').Substring(0, 8) + ","); //Número de empleado que maneje Hospira	8
                        stw.Write("3,"); //Dejar por default el número 3	1
                        stw.Write("00001,"); //Dejar por default el número  00001	5
                        stw.Write(" ,"); //Dejar en blaco	1
                        stw.Write(row["Concepto"].ToString().PadRight(4, ' ').Substring(0, 4) + ","); //Elegir el concepto de nómina, del catálgo de nómina que hayan definido con el cliente, en este caso anexo en otro archivo nuestro catálogo (México)	4

                        stw.Write(string.Format("{0}{1}{2},", FechaNomina.Year.ToString(), FechaNomina.Month.ToString().PadLeft(2, '0').Substring(0, 2), FechaNomina.Day.ToString().PadLeft(2, '0').Substring(0, 2)).PadLeft(8, '0').Substring(0, 8) + ","); //Fecha de pago de la nómina, seguir el formato AAAAMMDD	8

                        stw.Write(FechaNomina.Day.ToString().PadLeft(2, '0').Substring(0, 2) + ","); //Día de pago de la nómina, seguir el formato DD	2
                        stw.Write(FechaNomina.Year.ToString() + ","); //Año de pago de la nómina, seguir el formato AAAA	4
                        stw.Write(FechaNomina.Month.ToString().PadLeft(2, '0').Substring(0, 2) + ","); //Mes de pago de la nómina, seguir el formato MM	2
                        stw.Write(string.Format("{0}{1}{2},", inicio.Year.ToString(), inicio.Month.ToString().PadLeft(2, '0').Substring(0, 2), inicio.Day.ToString().PadLeft(2, '0').Substring(0, 2)).PadLeft(8, '0').Substring(0, 8) + ","); //Fecha  en que inicia el período de nómina, seguir el formato AAAAMMDD	8

                        stw.Write(string.Format("{0}{1}{2},", fin.Year.ToString(), fin.Month.ToString().PadLeft(2, '0').Substring(0, 2), fin.Day.ToString().PadLeft(2, '0').Substring(0, 2)).PadLeft(8, '0').Substring(0, 8) + ","); //Fecha  en que termina el período de nómina, seguir el formato AAAAMMDD	8
                        stw.Write("   ,"); //Dejar en blanco	3
                        stw.Write("               ,"); //Dejar en blanco	15
                        stw.Write("               ,"); //Dejar en blanco	15
                        stw.Write(String.Format("{0:00000000000.00}", importeempleado).PadLeft(15, '0').Substring(0, 15)); //Importe de cada uno de los conceptos de nómina	15
                        stw.WriteLine();
                    }
                    stw.Flush();
                }
            }
            catch (Exception ex)
            {
                System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
                return encoding.GetBytes(ex.Message);
            }

            return mst.ToArray();
        }

        /// <summary>
        /// Renderiza un arreglo de byte con los datos de la Emisión
        /// </summary>
        /// <param name="Año">Año a buscar</param>
        /// <param name="Periodo">Peiodo a buscar</param>
        /// <param name="consec">Consecutivo a agregar</param>
        /// <returns>Devuelve un valor de tipo <see cref="System.Byte"/> con el resultado de los datos</returns>
        public byte[] RenderLayoutTepcaEmision(int Año, int Periodo, int consec, ref string nombrearchivo)
        {
            DateTime fecha = DateTime.Now;
            DataTable datos;
            System.IO.MemoryStream mst;
            System.IO.StreamWriter stw;
            DataAccess SqlHelper;
            try
            {
                mst = new System.IO.MemoryStream();
                stw = new System.IO.StreamWriter(mst);

                SqlHelper = new DataAccess();
                datos = SqlHelper.RenderLayoutSiValeAlta(Año, Periodo);

                if (datos.Rows.Count > 0)
                {
                    fecha = DateTime.Now;

                    stw.Write("0"); //Tipo de Registro 2
                    nombrearchivo = fecha.Year.ToString().Substring(2, 2) +
                             (fecha.Month.ToString().Length == 1 ? "0" + fecha.Month.ToString() : fecha.Month.ToString()) +
                             (fecha.Day.ToString().Length == 1 ? "0" + fecha.Day.ToString() : fecha.Day.ToString()) + consec.ToString().PadLeft(2, '0').Substring(0, 2);
                    stw.Write(nombrearchivo); //Número de Lote 8
                    stw.Write("HOS040429V63   "); //ID de la Empresa 15
                    stw.Write(datos.Rows.Count.ToString().PadLeft(5, '0').Substring(0, 5)); //Cantidad de Registros 5
                    stw.Write("1"); //Tipo de Lote 1
                    stw.Write(string.Format("{0}{1}{2}", DateTime.Now.Year, DateTime.Now.Month.ToString().PadLeft(2, '0').Substring(0, 2), DateTime.Now.Day.ToString().PadLeft(2, '0').Substring(0, 2)).Substring(0, 8)); //Fecha del Lote 8
                    stw.Write("".PadLeft(18, '0').Substring(0, 18)); //Monto del Lote 18

                    foreach (DataRow row in datos.Rows)
                    {
                        stw.WriteLine();
                        stw.Write("1"); //Tipo de Registro 2
                        stw.Write(fecha.Year.ToString().Substring(2, 2) +
                             (fecha.Month.ToString().Length == 1 ? "0" + fecha.Month.ToString() : fecha.Month.ToString()) +
                             (fecha.Day.ToString().Length == 1 ? "0" + fecha.Day.ToString() : fecha.Day.ToString()) + consec.ToString().PadLeft(2, '0').Substring(0, 2)); //Número de Lote 8
                        stw.Write(row["Rfc"].ToString().Substring(0, 10).PadRight(15, ' ')); //Id Empleado 15
                        stw.Write(row["NombreCompleto"].ToString().PadRight(15, ' ').Substring(0, 15)); //Nombre del empleado.	15
                        stw.Write(row["Paterno"].ToString().PadRight(15, ' ').Substring(0, 15)); //Apellido del empleado. 15
                        stw.Write("0".PadRight(15, ' ').Substring(0, 15)); //Tarjeta Adicional	1
                    }
                    stw.Flush();
                }
            }
            catch (Exception ex)
            {
                System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
                return encoding.GetBytes(ex.Message);
            }

            return mst.ToArray();
        }

        public byte[] RenderLayoutTepcaRecarga(int Año, int Periodo, int Nomina, int Operacion, bool estado, int consec, ref string nombrearchivo, bool prueba)
        {
            DateTime fecha = DateTime.Now;
            DataTable datos;
            System.IO.MemoryStream mst;
            System.IO.StreamWriter stw;
            DataAccess SqlHelper;
            decimal importe;

            try
            {
                mst = new System.IO.MemoryStream();
                stw = new System.IO.StreamWriter(mst);

                SqlHelper = new DataAccess();
                datos = SqlHelper.RenderLayoutSiVale(Año, Periodo, short.Parse(Nomina.ToString()), estado, Operacion);

                if (datos.Rows.Count > 0)
                {
                    try
                    {
                        if (!prueba)
                            importe = decimal.Parse(datos.Compute("SUM(Importe)", "").ToString());
                        else
                            importe = datos.Rows.Count;
                    }
                    catch (Exception)
                    {
                        importe = decimal.Parse("0");
                    }

                    fecha = DateTime.Now;

                    stw.Write("0"); //Tipo de Registro 2
                    nombrearchivo = fecha.Year.ToString().Substring(2, 2) +
                             (fecha.Month.ToString().Length == 1 ? "0" + fecha.Month.ToString() : fecha.Month.ToString()) +
                             (fecha.Day.ToString().Length == 1 ? "0" + fecha.Day.ToString() : fecha.Day.ToString()) + consec.ToString().PadLeft(2, '0').Substring(0, 2);
                    stw.Write(nombrearchivo); //Número de Lote 8
                    stw.Write("HOS040429V63   "); //ID de la Empresa 15
                    stw.Write(datos.Rows.Count.ToString().PadLeft(5, '0').Substring(0, 5)); //Cantidad de Registros 5
                    stw.Write("2"); //Tipo de Lote 1
                    stw.Write(string.Format("{0}{1}{2}", DateTime.Now.Year, DateTime.Now.Month.ToString().PadLeft(2, '0').Substring(0, 2), DateTime.Now.Day.ToString().PadLeft(2, '0').Substring(0, 2)).Substring(0, 8)); //Fecha del Lote 8
                    stw.Write(importe.ToString("#.00").Replace(".", "").PadLeft(18, '0').Substring(0, 18)); //Monto del Lote 18

                    foreach (DataRow row in datos.Rows)
                    {
                        stw.WriteLine();
                        stw.Write("2"); //Tipo de Registro 2
                        stw.Write(fecha.Year.ToString().Substring(2, 2) +
                             (fecha.Month.ToString().Length == 1 ? "0" + fecha.Month.ToString() : fecha.Month.ToString()) +
                             (fecha.Day.ToString().Length == 1 ? "0" + fecha.Day.ToString() : fecha.Day.ToString()) + consec.ToString().PadLeft(2, '0').Substring(0, 2)); //Número de Lote 8
                        stw.Write(row["Rfc"].ToString().Substring(0, 10).PadRight(15, ' ')); //Id Empleado 15
                        stw.Write(decimal.Parse(!prueba ? row["Importe"].ToString() : "1").ToString("#.00").Replace(".", "").PadLeft(18, '0').Substring(0, 18)); //Monto Cargado
                    }
                    stw.Flush();
                }
            }
            catch (Exception ex)
            {
                System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
                return encoding.GetBytes(ex.Message);
            }

            return mst.ToArray();
        }

        public byte[] RenderLayoutSivaleOperaciones(int Año, int Periodo, OperacionSiVale operacion, ref string nombrearchivo)
        {
            DateTime fecha = DateTime.Now;
            DataTable datos;
            System.IO.MemoryStream mst;
            System.IO.StreamWriter stw;
            DataAccess SqlHelper;
            try
            {
                mst = new System.IO.MemoryStream();
                stw = new System.IO.StreamWriter(mst);

                SqlHelper = new DataAccess();
                datos = SqlHelper.RenderLayoutSiValeOperaciones(Año, Periodo, operacion);

                StringBuilder sb = new StringBuilder();

                if (datos.Rows.Count > 0)
                {
                    decimal importeTotal = 0;

                    string fileHeader = Utilities.GetFileHeaderSiVale(operacion);

                    string id = operacion == OperacionSiVale.MisComprasComidaSaldo ? "comida_" : operacion == OperacionSiVale.MisComprasDespensaSaldo ? "despensa_" : string.Empty;
                    nombrearchivo = string.Format("{0}{1}", id, fileHeader.Replace(".", "_"));

                    try
                    {
                        importeTotal = decimal.Parse(datos.Compute("SUM(Importe)", "").ToString());
                    }
                    catch (Exception)
                    {
                        importeTotal = 0;
                    }
                    string firtLine = Utilities.GetFirstLineSiVale(operacion, datos.Rows.Count, importeTotal);

                    stw.Write(fileHeader);
                    stw.WriteLine();
                    stw.Write(firtLine);

                    int index = 1;
                    foreach (DataRow row in datos.Rows)
                    {
                        stw.WriteLine();
                        switch (operacion)
                        {
                            case OperacionSiVale.MisComprasSolicitud:
                            case OperacionSiVale.MisComprasSolicitudAdicionales:
                            case OperacionSiVale.MisComprasAlimentacionSolicitud:
                            case OperacionSiVale.MisComprasAlimentacionSolicitudAdicionales:
                                sb.AppendFormat("{0}<br/>", row["NOMCORTO"].ToString() == string.Empty ? row["Referencia"].ToString() : string.Empty);
                                stw.Write(Utilities.GetLineSiValeEmision(index++, row["Referencia"].ToString(), row["NOMCORTO"].ToString()));
                                break;

                            case OperacionSiVale.MisComprasDespensaSaldo:
                            case OperacionSiVale.MisComprasComidaSaldo:
                            case OperacionSiVale.MisComprasAlimentacionSaldo:
                                sb.AppendFormat("{0}<br/>", row["Tarjeta"].ToString() == string.Empty ? row["Referencia"].ToString() : string.Empty);
                                stw.Write(Utilities.GetLineSiValeSaldo(index++, row["Tarjeta"].ToString(), decimal.Parse(row["Importe"].ToString())));
                                break;
                        }
                    }
                    stw.Flush();
                }
            }
            catch (Exception ex)
            {
                System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
                return encoding.GetBytes(ex.Message);
            }

            return mst.ToArray();
        }
    }
}