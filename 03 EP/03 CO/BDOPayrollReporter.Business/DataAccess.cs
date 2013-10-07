using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using BDOPayrollReporter.Business.Enums;
using BDOPayrollReporter.Business.Objects;
using BDOPayrollReporter.Business.Objects.Collections;
using SIO.Services.Data.Services;

namespace BDOPayrollReporter.Business
{
    public class DataAccess
    {
        public DataAccess()
        {
        }

        #region Utilities

        internal string ToSqlDateString(DateTime Date)
        {
            return this.ToSqlDateString(Date, false);
        }

        internal string ToSqlDateString(DateTime Date, bool IncludeTime)
        {
            string datestring = string.Empty;
            if (IncludeTime)
            {
                datestring = string.Format("{0}{1}{2} {3}:{4}:{5}.{6}", Date.Year.ToString().PadLeft(4, '0'), Date.Month.ToString().PadLeft(2, '0'), Date.Day.ToString().PadLeft(2, '0'), Date.Hour.ToString().PadLeft(2, '0'), Date.Minute.ToString().PadLeft(2, '0'), Date.Second.ToString().PadLeft(2, '0'), Date.Millisecond);
            }
            else
            {
                datestring = string.Format("{0}{1}{2}", Date.Year.ToString().PadLeft(4, '0'), Date.Month.ToString().PadLeft(2, '0'), Date.Day.ToString().PadLeft(2, '0'));
            }
            return datestring;
        }

        public EmpleadoList GetEmpleados()
        {
            EmpleadoList list = new EmpleadoList();
            try
            {
                SQLDataManager connector = new SQLDataManager();
                List<SqlParameter> parameters = new List<SqlParameter>();
                DataTable dt = connector.ExecuteProcedure("usp_ReciboNominaEmpledosConEmail", parameters);
                foreach (DataRow dr in dt.Rows)
                {
                    Empleado e = new Empleado();
                    e.Id = dr["Trab_ID"].ToString();
                    e.Nombre = dr["Nombre"].ToString();
                    e.Paterno = dr["Paterno"].ToString();
                    e.Materno = dr["Materno"].ToString();
                    e.Email = dr["EMAIL"].ToString();
                    list.Add(e);
                }
            }
            catch (Exception oe)
            {
                throw oe;
            }
            return list;
        }

        #endregion Utilities

        #region Layouts

        public DataTable RenderLayoutSiValeAlta(int Año, int Periodo)
        {
            List<SqlParameter> param = new List<SqlParameter>();
            StringBuilder query = new StringBuilder();
            DataTable result;

            if (Utilities.ValidarAño(Año))
            {
                if (Utilities.ValidarMes(Periodo))
                {
                    param.Add(new SqlParameter("Mes", Periodo));
                    param.Add(new SqlParameter("Año", Año));

                    try
                    {
                        SQLDataManager connector = new SQLDataManager();
                        result = connector.ExecuteProcedure("usp_LayoutSiValeAlta_Tepca", param);
                    }
                    catch (Exception e)
                    {
                        result = null;
                        throw e;
                    }
                }
                else
                    throw new ArgumentException("El periodo no es válido", "Periodo");
            }
            else
                throw new ArgumentException("El año no es válido", "Año");

            return result;
        }

        public DataTable RenderLayoutSiVale(int Año, int Periodo, short Tipo, bool Abierta, int Concepto)
        {
            StringBuilder query = new StringBuilder();
            DataTable result;

            if (Utilities.ValidarAño(Año))
            {
                if (Utilities.ValidarMes(Periodo))
                {
                    query.AppendLine("DECLARE @Mes INT");
                    query.AppendLine("DECLARE @Año INT");
                    query.AppendLine("DECLARE @Tipo TINYINT");
                    query.AppendLine("DECLARE @Concepto TINYINT");
                    query.AppendLine();
                    query.AppendFormat("SET @Mes = {0}", Periodo);
                    query.AppendLine();
                    query.AppendFormat("SET @Año = {0}", Año);
                    query.AppendLine();
                    query.AppendFormat("SET @Tipo = {0}", Tipo);
                    query.AppendLine();
                    query.AppendFormat("SET @Concepto = {0}", Concepto);
                    query.AppendLine();
                    query.AppendLine("SELECT DISTINCT");
                    query.AppendLine("	[Empleado].[Trab_ID] AS Empleado,");
                    query.AppendLine("	CASE WHEN [Empleado].[CuentaContable] = '' OR [Empleado].[CuentaContable] IS NULL ");
                    query.AppendLine("      THEN '11111111' ELSE [Empleado].[CuentaContable] END AS Cuenta,");
                    query.AppendLine("	REPLACE([Empleado].[Nombre], ' ', '') + ' ' + [Empleado].[Paterno] + ' ' + [Empleado].[Materno] AS Nombre,");
                    query.AppendLine("  REPLACE([Empleado].[Rfc], '-', '') AS Rfc,");
                    query.AppendLine("	CAST(ISNULL([NomCalculoHistorico].[Importe], 0) AS MONEY) Importe");
                    query.AppendLine("FROM");
                    query.AppendLine("	[dbo].[Empleado]");
                    if (Abierta)
                        query.AppendLine("	LEFT JOIN [dbo].[NomCalculo] AS [NomCalculoHistorico] ON [Empleado].[Trab_ID] = [NomCalculoHistorico].[Trab_ID]");
                    else
                        query.AppendLine("	LEFT JOIN [dbo].[NomCalculoHistorico] ON [Empleado].[Trab_ID] = [NomCalculoHistorico].[Trab_ID]");
                    query.AppendLine("WHERE [NomCalculoHistorico].[Periodo] = @Mes");
                    query.AppendLine("	AND [NomCalculoHistorico].[Ano] = @Año");
                    query.AppendLine("	AND [NomCalculoHistorico].[Concepto_ID] = @Concepto");
                    query.AppendLine("	AND [NomCalculoHistorico].[TipoNomina_ID] = @Tipo");
                    //query.AppendLine("AND [Empleado].[Trab_ID] NOT IN ('80003749','80092625','80092755')");

                    try
                    {
                        SQLDataManager connector = new SQLDataManager();
                        result = connector.ExecuteQuery(query.ToString());
                    }
                    catch (Exception e)
                    {
                        result = null;
                        throw e;
                    }
                }
                else
                    throw new ArgumentException("El periodo no es válido", "Periodo");
            }
            else
                throw new ArgumentException("El año no es válido", "Año");

            return result;
        }

        public DataTable RenderLayoutCitybank484(int Año, int Periodo, short Tipo, bool Abierta, bool EsPrueba)
        {
            StringBuilder query = new StringBuilder();
            DataTable result;

            if (Utilities.ValidarAño(Año))
            {
                if (Utilities.ValidarMes(Periodo))
                {
                    query.AppendLine("DECLARE @Mes INT");
                    query.AppendLine("DECLARE @Año INT");
                    query.AppendLine("DECLARE @Tipo TINYINT");
                    query.AppendLine();
                    query.AppendFormat("SET @Mes = {0}", Periodo);
                    query.AppendLine();
                    query.AppendFormat("SET @Año = {0}", Año);
                    query.AppendLine();
                    query.AppendFormat("SET @Tipo = {0}", Tipo);
                    query.AppendLine();
                    query.AppendLine();
                    query.AppendLine("SELECT DISTINCT");
                    query.AppendLine("	[TBRegPat].[Chequera] AS CuentaCliente,");
                    if (EsPrueba)
                        query.AppendLine("	CAST(1 AS MONEY) AS Importe,");
                    else
                    {
                        query.AppendLine("	SUM(CASE WHEN [NomCalculoHistorico].[Concepto_ID] = 801 THEN Importe ELSE CAST(0 AS MONEY) END) + ");
                        query.AppendLine("	SUM(CASE WHEN [NomCalculoHistorico].[Concepto_ID] = 616 THEN (Importe) * -1 ELSE CAST(0 AS MONEY) END) Importe ,");
                    }
                    query.AppendLine("	REPLACE(REPLACE([Empleado].[RFC], '-', ''),'_','') AS RFC,");
                    query.AppendLine("	[Empleado].[Trab_ID] AS Empleado,");
                    query.AppendLine("	[Empleado].[Paterno] + [Empleado].[Materno] + [Empleado].[Nombre] AS NombreEmpleado,");
                    query.AppendLine("	[Empleado].[Calle] AS Calle,");
                    query.AppendLine("	[Empleado].[Colonia] AS Colonia,");
                    query.AppendLine("	[Empleado].[Ciudad] AS Municipio,");
                    query.AppendLine("	[Empleado].[CP] AS CodigoPostal,");
                    query.AppendLine("	[Empleado].[BancoDeposito] AS Banco,");
                    query.AppendLine("	[Empleado].[CuentaDeposito] AS CuentaEmpleado");
                    query.AppendLine("FROM");
                    query.AppendLine("	[dbo].[Empleado]");
                    if (Abierta)
                        query.AppendLine("	LEFT JOIN [dbo].[NomCalculo] AS [NomCalculoHistorico] ON [Empleado].[Trab_ID] = [NomCalculoHistorico].[Trab_ID]");
                    else
                        query.AppendLine("	LEFT JOIN [dbo].[NomCalculoHistorico] ON [Empleado].[Trab_ID] = [NomCalculoHistorico].[Trab_ID]");
                    query.AppendLine("	INNER JOIN (");
                    query.AppendLine("		SELECT");
                    query.AppendLine("			[Movimientos].[Trab_ID],");
                    query.AppendLine("			[Movimientos].[FechaMov],");
                    query.AppendLine("			[Movimientos].[Centro_ID]");
                    query.AppendLine("		FROM");
                    query.AppendLine("			[dbo].[Movimiento] AS [Movimientos]");
                    query.AppendLine("			INNER JOIN (");
                    query.AppendLine("				SELECT");
                    query.AppendLine("					[Trab_ID],");
                    query.AppendLine("					MAX([Ptr]) AS [Ptr]");
                    query.AppendLine("				FROM");
                    query.AppendLine("					[dbo].[Movimiento]");
                    query.AppendLine("				GROUP BY");
                    query.AppendLine("					[Trab_ID]");
                    query.AppendLine("				) AS [MaxMovimiento] ON [MaxMovimiento].[Trab_ID] = [Movimientos].[Trab_ID] AND [MaxMovimiento].[Ptr] = [Movimientos].[Ptr]");
                    query.AppendLine("	) AS [Movimiento] ON [Empleado].[Trab_ID] = [Movimiento].[Trab_ID]");
                    query.AppendLine("	INNER JOIN [dbo].[TBCentros] ON [Movimiento].[Centro_ID] = [TBCentros].[Centro_ID]");
                    query.AppendLine("	INNER JOIN [dbo].[TBRegPat] ON [TBCentros].[RegPat_ID] = [TBRegPat].[RegPat_ID]");
                    query.AppendLine("WHERE");
                    query.AppendLine("	[NomCalculoHistorico].[Periodo] = @Mes");
                    query.AppendLine("	AND [NomCalculoHistorico].[Ano] = @Año");
                    query.AppendLine("	AND ([NomCalculoHistorico].[Concepto_ID] = 801 OR [NomCalculoHistorico].[Concepto_ID] = 616)");
                    query.AppendLine("	AND [NomCalculoHistorico].[TipoNomina_ID] = @Tipo");
                    //Empleado Especifico
                    //query.AppendLine("	AND [Empleado].[Trab_ID] IN ('80000095','80001794')");

                    query.AppendLine("GROUP BY ");
                    query.AppendLine("  [TBRegPat].[Chequera],");
                    query.AppendLine("  [Empleado].[RFC],");
                    query.AppendLine("  [Empleado].[Trab_ID],");
                    query.AppendLine("  [Empleado].[Paterno],");
                    query.AppendLine("  [Empleado].[Materno],");
                    query.AppendLine("  [Empleado].[Nombre],");
                    query.AppendLine("  [Empleado].[Calle],");
                    query.AppendLine("  [Empleado].[Colonia],");
                    query.AppendLine("  [Empleado].[Ciudad],");
                    query.AppendLine("  [Empleado].[CP],");
                    query.AppendLine("  [Empleado].[BancoDeposito],");
                    query.AppendLine("  [Empleado].[CuentaDeposito]");

                    try
                    {
                        SQLDataManager connector = new SQLDataManager();
                        result = connector.ExecuteQuery(query.ToString());
                    }
                    catch (Exception e)
                    {
                        result = null;
                        throw e;
                    }
                }
                else
                    throw new ArgumentException("El periodo no es válido", "Periodo");
            }
            else
                throw new ArgumentException("El año no es válido", "Año");

            return result;
        }

        public DataTable RenderLayoutCitybank485(int Año, int Periodo, short Tipo, bool Abierta, bool EsPrueba)
        {
            StringBuilder query = new StringBuilder();
            DataTable result;

            if (Utilities.ValidarAño(Año))
            {
                if (Utilities.ValidarMes(Periodo))
                {
                    query.AppendLine("DECLARE @Mes INT");
                    query.AppendLine("DECLARE @Año INT");
                    query.AppendLine("DECLARE @Tipo TINYINT");
                    query.AppendLine();
                    query.AppendFormat("SET @Mes = {0}", Periodo);
                    query.AppendLine();
                    query.AppendFormat("SET @Año = {0}", Año);
                    query.AppendLine();
                    query.AppendFormat("SET @Tipo = {0}", Tipo);
                    query.AppendLine();
                    query.AppendLine();
                    query.AppendLine("SELECT DISTINCT");
                    query.AppendLine("	[TBRegPat].[Chequera] AS CuentaCliente,");
                    if (EsPrueba)
                        query.AppendLine("	CAST(1 AS MONEY) AS Importe,");
                    else
                    {
                        query.AppendLine("	SUM(CASE WHEN [NomCalculoHistorico].[Concepto_ID] = 801 THEN Importe ELSE CAST(0 AS MONEY) END) + ");
                        query.AppendLine("	SUM(CASE WHEN [NomCalculoHistorico].[Concepto_ID] = 616 THEN (Importe) * -1 ELSE CAST(0 AS MONEY) END) Importe ,");
                    }
                    query.AppendLine("	REPLACE(REPLACE([Empleado].[RFC], '-', ''),'_','') AS RFC,");
                    query.AppendLine("	[Empleado].[Trab_ID] AS Empleado,");
                    query.AppendLine("	[Empleado].[Paterno] + [Empleado].[Materno] + [Empleado].[Nombre] AS NombreEmpleado,");
                    query.AppendLine("	[Empleado].[Calle] AS Calle,");
                    query.AppendLine("	[Empleado].[Colonia] AS Colonia,");
                    query.AppendLine("	[Empleado].[Ciudad] AS Municipio,");
                    query.AppendLine("	[Empleado].[CP] AS CodigoPostal,");
                    query.AppendLine("	[Empleado].[BancoDeposito] AS Banco,");
                    query.AppendLine("	[Empleado].[CuentaDeposito] AS CuentaEmpleado");
                    query.AppendLine("FROM");
                    query.AppendLine("	[dbo].[Empleado]");
                    if (Abierta)
                        query.AppendLine("	LEFT JOIN [dbo].[NomCalculo] AS [NomCalculoHistorico] ON [Empleado].[Trab_ID] = [NomCalculoHistorico].[Trab_ID]");
                    else
                        query.AppendLine("	LEFT JOIN [dbo].[NomCalculoHistorico] ON [Empleado].[Trab_ID] = [NomCalculoHistorico].[Trab_ID]");
                    query.AppendLine("	INNER JOIN (");
                    query.AppendLine("		SELECT");
                    query.AppendLine("			[Movimientos].[Trab_ID],");
                    query.AppendLine("			[Movimientos].[FechaMov],");
                    query.AppendLine("			[Movimientos].[Centro_ID]");
                    query.AppendLine("		FROM");
                    query.AppendLine("			[dbo].[Movimiento] AS [Movimientos]");
                    query.AppendLine("			INNER JOIN (");
                    query.AppendLine("				SELECT");
                    query.AppendLine("					[Trab_ID],");
                    query.AppendLine("					MAX([Ptr]) AS [Ptr]");
                    query.AppendLine("				FROM");
                    query.AppendLine("					[dbo].[Movimiento]");
                    query.AppendLine("				GROUP BY");
                    query.AppendLine("					[Trab_ID]");
                    query.AppendLine("				) AS [MaxMovimiento] ON [MaxMovimiento].[Trab_ID] = [Movimientos].[Trab_ID] AND [MaxMovimiento].[Ptr] = [Movimientos].[Ptr]");
                    query.AppendLine("	) AS [Movimiento] ON [Empleado].[Trab_ID] = [Movimiento].[Trab_ID]");
                    query.AppendLine("	INNER JOIN [dbo].[TBCentros] ON [Movimiento].[Centro_ID] = [TBCentros].[Centro_ID]");
                    query.AppendLine("	INNER JOIN [dbo].[TBRegPat] ON [TBCentros].[RegPat_ID] = [TBRegPat].[RegPat_ID]");
                    query.AppendLine("WHERE");
                    query.AppendLine("	[NomCalculoHistorico].[Periodo] = @Mes");
                    query.AppendLine("	AND [NomCalculoHistorico].[Ano] = @Año");
                    query.AppendLine("	AND ([NomCalculoHistorico].[Concepto_ID] = 801 OR [NomCalculoHistorico].[Concepto_ID] = 616)");
                    query.AppendLine("	AND [NomCalculoHistorico].[TipoNomina_ID] = @Tipo");
                    //Empleado Especifico
                    //query.AppendLine("	AND [Empleado].[Trab_ID] IN ('80000095','80001794')");

                    query.AppendLine("GROUP BY ");
                    query.AppendLine("  [TBRegPat].[Chequera],");
                    query.AppendLine("  [Empleado].[RFC],");
                    query.AppendLine("  [Empleado].[Trab_ID],");
                    query.AppendLine("  [Empleado].[Paterno],");
                    query.AppendLine("  [Empleado].[Materno],");
                    query.AppendLine("  [Empleado].[Nombre],");
                    query.AppendLine("  [Empleado].[Calle],");
                    query.AppendLine("  [Empleado].[Colonia],");
                    query.AppendLine("  [Empleado].[Ciudad],");
                    query.AppendLine("  [Empleado].[CP],");
                    query.AppendLine("  [Empleado].[BancoDeposito],");
                    query.AppendLine("  [Empleado].[CuentaDeposito]");

                    try
                    {
                        SQLDataManager connector = new SQLDataManager();
                        result = connector.ExecuteQuery(query.ToString());
                    }
                    catch (Exception e)
                    {
                        result = null;
                        throw e;
                    }
                }
                else
                    throw new ArgumentException("El periodo no es válido", "Periodo");
            }
            else
                throw new ArgumentException("El año no es válido", "Año");

            return result;
        }

        public DataTable RenderLayoutBanorteAlta(int Año, int Periodo)
        {
            StringBuilder query = new StringBuilder();
            DataTable result;

            if (Utilities.ValidarAño(Año))
            {
                if (Utilities.ValidarMes(Periodo))
                {
                    query.AppendLine("DECLARE @Mes INT");
                    query.AppendLine("DECLARE @Año INT");
                    query.AppendLine();
                    query.AppendFormat("SET @Mes = {0}", Periodo);
                    query.AppendLine();
                    query.AppendFormat("SET @Año = {0}", Año);
                    query.AppendLine();
                    query.AppendLine();
                    query.AppendLine("SELECT DISTINCT");
                    query.AppendLine("	[Empleado].[Trab_ID] AS Empleado,");
                    query.AppendLine("	[Empleado].[Nombre] AS NombreEmpleado,");
                    query.AppendLine("	[Empleado].[Paterno] AS ApellidoPaternoEmpleado,");
                    query.AppendLine("	[Empleado].[Materno] AS ApellidoMaternoEmpleado,");
                    query.AppendLine("	[Movimiento].[Salario] * 30 AS Sueldo,");
                    query.AppendLine("	[Empleado].[FechaIngreso] AS FechaIngreso,");
                    query.AppendLine("	REPLACE([Empleado].[Rfc], '-', '') AS Rfc");
                    query.AppendLine("FROM");
                    query.AppendLine("	[dbo].[Empleado]");
                    query.AppendLine("	INNER JOIN (");
                    query.AppendLine("		SELECT");
                    query.AppendLine("			[Movimientos].[Trab_ID],");
                    query.AppendLine("			[Movimientos].[FechaMov],");
                    query.AppendLine("			[Movimientos].[Salario]");
                    query.AppendLine("		FROM");
                    query.AppendLine("			[dbo].[Movimiento] AS [Movimientos]");
                    query.AppendLine("			INNER JOIN (");
                    query.AppendLine("				SELECT");
                    query.AppendLine("					[Trab_ID],");
                    query.AppendLine("					MAX([Ptr]) AS [Ptr]");
                    query.AppendLine("				FROM");
                    query.AppendLine("					[dbo].[Movimiento]");
                    query.AppendLine("				GROUP BY");
                    query.AppendLine("					[Trab_ID]");
                    query.AppendLine("				) AS [MaxMovimiento] ON [MaxMovimiento].[Trab_ID] = [Movimientos].[Trab_ID] AND [MaxMovimiento].[Ptr] = [Movimientos].[Ptr]");
                    query.AppendLine("	) AS [Movimiento] ON [Empleado].[Trab_ID] = [Movimiento].[Trab_ID]");
                    query.AppendLine("WHERE");
                    query.AppendLine("	YEAR([FechaIngreso]) = @Año");
                    query.AppendLine("	AND MONTH([FechaIngreso]) = @Mes");
                    query.AppendLine("ORDER BY");
                    query.Append("	[Empleado].[Trab_ID]");

                    try
                    {
                        SQLDataManager connector = new SQLDataManager();
                        result = connector.ExecuteQuery(query.ToString());
                    }
                    catch (Exception e)
                    {
                        result = null;
                        throw e;
                    }
                }
                else
                    throw new ArgumentException("El periodo no es válido", "Periodo");
            }
            else
                throw new ArgumentException("El año no es válido", "Año");

            return result;
        }

        public DataTable RenderLayoutBanorte(int Año, int Periodo, short Tipo, bool Abierta)
        {
            StringBuilder query = new StringBuilder();
            DataTable result;

            if (Utilities.ValidarAño(Año))
            {
                if (Utilities.ValidarMes(Periodo))
                {
                    query.AppendLine("DECLARE @Mes INT");
                    query.AppendLine("DECLARE @Año INT");
                    query.AppendLine("DECLARE @Tipo TINYINT");
                    query.AppendLine();
                    query.AppendFormat("SET @Mes = {0}", Periodo);
                    query.AppendLine();
                    query.AppendFormat("SET @Año = {0}", Año);
                    query.AppendLine();
                    query.AppendFormat("SET @Tipo = {0}", Tipo);
                    query.AppendLine();
                    query.AppendLine();
                    query.AppendLine("SELECT");
                    query.AppendLine("	[NomCalculoHistorico].[Concepto_ID] AS Concepto,");
                    query.AppendLine("	[TBRegPat].[Chequera] AS CuentaCliente,");
                    query.AppendLine("	[NomCalculoHistorico].[Importe] AS Importe,");
                    query.AppendLine("	[Empleado].[RFC] AS RFC,");
                    query.AppendLine("	[Empleado].[Trab_ID] AS Empleado,");
                    query.AppendLine("	[Empleado].[Paterno] + [Empleado].[Materno] + [Empleado].[Nombre] AS NombreEmpleado,");
                    query.AppendLine("	[Empleado].[Calle] AS Calle,");
                    query.AppendLine("	[Empleado].[Colonia] AS Colonia,");
                    query.AppendLine("	[Empleado].[Ciudad] AS Municipio,");
                    query.AppendLine("	[Empleado].[CP] AS CodigoPostal,");
                    query.AppendLine("	[Empleado].[BancoDeposito] AS Banco,");
                    query.AppendLine("	[EmpleadoExtra].[Campo1] AS CuentaEmpleado");
                    query.AppendLine("FROM");
                    query.AppendLine("	[dbo].[Empleado]");
                    if (Abierta)
                        query.AppendLine("	LEFT JOIN [dbo].[NomCalculo] AS [NomCalculoHistorico] ON [Empleado].[Trab_ID] = [NomCalculoHistorico].[Trab_ID]");
                    else
                        query.AppendLine("	LEFT JOIN [dbo].[NomCalculoHistorico] ON [Empleado].[Trab_ID] = [NomCalculoHistorico].[Trab_ID]");
                    query.AppendLine("	LEFT JOIN [dbo].[EquivalenciaConceptos] ON [NomCalculoHistorico].[Concepto_ID] = [EquivalenciaConceptos].[Concepto_ID]");
                    query.AppendLine("	INNER JOIN (");
                    query.AppendLine("		SELECT");
                    query.AppendLine("			[Movimientos].[Trab_ID],");
                    query.AppendLine("			[Movimientos].[FechaMov],");
                    query.AppendLine("			[Movimientos].[Centro_ID]");
                    query.AppendLine("		FROM");
                    query.AppendLine("			[dbo].[Movimiento] AS [Movimientos]");
                    query.AppendLine("			INNER JOIN (");
                    query.AppendLine("				SELECT");
                    query.AppendLine("					[Trab_ID],");
                    query.AppendLine("					MAX([Ptr]) AS [Ptr]");
                    query.AppendLine("				FROM");
                    query.AppendLine("					[dbo].[Movimiento]");
                    query.AppendLine("				GROUP BY");
                    query.AppendLine("					[Trab_ID]");
                    query.AppendLine("				) AS [MaxMovimiento] ON [MaxMovimiento].[Trab_ID] = [Movimientos].[Trab_ID] AND [MaxMovimiento].[Ptr] = [Movimientos].[Ptr]");
                    query.AppendLine("	) AS [Movimiento] ON [Empleado].[Trab_ID] = [Movimiento].[Trab_ID]");
                    query.AppendLine("	INNER JOIN [dbo].[TBCentros] ON [Movimiento].[Centro_ID] = [TBCentros].[Centro_ID]");
                    query.AppendLine("	INNER JOIN [dbo].[TBRegPat] ON [TBCentros].[RegPat_ID] = [TBRegPat].[RegPat_ID]");
                    query.AppendLine("	INNER JOIN [dbo].[EmpleadoExtra] ON [Empleado].[Trab_ID] = [EmpleadoExtra].[Trab_ID]");
                    query.AppendLine("WHERE");
                    query.AppendLine("	[NomCalculoHistorico].[Periodo] = @Mes");
                    query.AppendLine("	AND [NomCalculoHistorico].[Ano] = @Año");
                    query.AppendLine("	AND ([NomCalculoHistorico].[Concepto_ID] = 621 OR [NomCalculoHistorico].[Concepto_ID] = 38)");
                    query.AppendLine("	AND [NomCalculoHistorico].[TipoNomina_ID] = @Tipo");
                    query.AppendLine("ORDER BY");
                    query.AppendLine("	[Empleado].[Trab_ID],");
                    query.Append("	[NomCalculoHistorico].[Concepto_ID]");

                    try
                    {
                        SQLDataManager connector = new SQLDataManager();
                        result = connector.ExecuteQuery(query.ToString());
                    }
                    catch (Exception e)
                    {
                        result = null;
                        throw e;
                    }
                }
                else
                    throw new ArgumentException("El periodo no es válido", "Periodo");
            }
            else
                throw new ArgumentException("El año no es válido", "Año");

            return result;
        }

        public DataTable RenderLayoutSkandiaAlta(int Año, int Periodo)
        {
            StringBuilder query = new StringBuilder();
            DataTable result;

            if (Utilities.ValidarAño(Año))
            {
                if (Utilities.ValidarMes(Periodo))
                {
                    query.AppendLine("DECLARE @Mes INT");
                    query.AppendLine("DECLARE @Año INT");
                    query.AppendLine();
                    query.AppendFormat("SET @Mes = {0}", Periodo);
                    query.AppendLine();
                    query.AppendFormat("SET @Año = {0}", Año);
                    query.AppendLine();
                    query.AppendLine();
                    query.AppendLine("SELECT");
                    query.AppendLine("	[Empleado].[Trab_ID] AS Empleado,");
                    query.AppendLine("	[Empleado].[Nombre] AS NombreEmpleado,");
                    query.AppendLine("	[Empleado].[Paterno] AS ApellidoPaternoEmpleado,");
                    query.AppendLine("	[Empleado].[Materno] AS ApellidoMaternoEmpleado,");
                    query.AppendLine("	[Empleado].[FechaNacimiento] AS FechaNacimiento,");
                    query.AppendLine("	[Empleado].[FechaIngreso] AS FechaIngreso,");
                    query.AppendLine("	[Empleado].[Curp] AS Curp,");
                    query.AppendLine("	[Empleado].[Rfc] AS Rfc,");
                    query.AppendLine("	[Empleado].[Sexo_IDa] AS IdSexo,");
                    query.AppendLine("	[Empleado].[Calle] AS Calle,");
                    query.AppendLine("	[Empleado].[Ciudad] AS Ciudad,");
                    query.AppendLine("	[Empleado].[Colonia] AS Colonia,");
                    query.AppendLine("	[Empleado].[Ciudad] AS Delegacion,");
                    query.AppendLine("	[Empleado].[Estado] AS Estado,");
                    query.AppendLine("	[Empleado].[CP] AS CodigoPostal,");
                    query.AppendLine("	[Empleado].[CuentaDeposito] AS CuentaBancaria,");
                    query.AppendLine("  [TBBanco].[Descripcion] AS Banco,");
                    query.AppendLine("  [TBDepto].[CentroCosto] AS CentroCosto,");
                    query.AppendLine("  [Movimiento].[Salario] * 30 AS Sueldo");
                    query.AppendLine("FROM");
                    query.AppendLine("	[dbo].[Empleado]");
                    query.AppendLine("	INNER JOIN (");
                    query.AppendLine("		SELECT");
                    query.AppendLine("			[Movimientos].[Trab_ID],");
                    query.AppendLine("			[Movimientos].[FechaMov],");
                    query.AppendLine("			[Movimientos].[Depto_ID],");
                    query.AppendLine("			[Movimientos].[Salario]");
                    query.AppendLine("		FROM");
                    query.AppendLine("			[dbo].[Movimiento] AS [Movimientos]");
                    query.AppendLine("			INNER JOIN (");
                    query.AppendLine("				SELECT");
                    query.AppendLine("					[Trab_ID],");
                    query.AppendLine("					MAX([Ptr]) AS [Ptr]");
                    query.AppendLine("				FROM");
                    query.AppendLine("					[dbo].[Movimiento]");
                    query.AppendLine("				GROUP BY");
                    query.AppendLine("					[Trab_ID]");
                    query.AppendLine("				) AS [MaxMovimiento] ON [MaxMovimiento].[Trab_ID] = [Movimientos].[Trab_ID] AND [MaxMovimiento].[Ptr] = [Movimientos].[Ptr]");
                    query.AppendLine("	) AS [Movimiento] ON [Empleado].[Trab_ID] = [Movimiento].[Trab_ID]");
                    query.AppendLine("	INNER JOIN [dbo].[TBDepto] ON [Movimiento].[Depto_ID] = [TBDepto].[Depto_ID]");
                    query.AppendLine("	LEFT JOIN [dbo].[TBBanco] ON [Empleado].[BancoDeposito] = [TBBanco].[Banco_ID]");
                    query.AppendLine("WHERE");
                    query.AppendLine("	YEAR([FechaIngreso]) = @Año");
                    query.Append("	AND MONTH([FechaIngreso]) = @Mes");

                    try
                    {
                        SQLDataManager connector = new SQLDataManager();
                        result = connector.ExecuteQuery(query.ToString());
                    }
                    catch (Exception e)
                    {
                        result = null;
                        throw e;
                    }
                }
                else
                    throw new ArgumentException("El periodo no es válido", "Periodo");
            }
            else
                throw new ArgumentException("El año no es válido", "Año");

            return result;
        }

        public DataTable RenderLayoutSkandiaAportaciones(int Año, int Periodo, short Tipo, bool Abierta)
        {
            StringBuilder query = new StringBuilder();
            DataTable result;

            if (Utilities.ValidarAño(Año))
            {
                if (Utilities.ValidarMes(Periodo))
                {
                    query.AppendLine("DECLARE @Mes INT");
                    query.AppendLine("DECLARE @Año INT");
                    query.AppendLine("DECLARE @Tipo TINYINT");
                    query.AppendLine();
                    query.AppendFormat("SET @Mes = {0}", Periodo);
                    query.AppendLine();
                    query.AppendFormat("SET @Año = {0}", Año);
                    query.AppendLine();
                    query.AppendFormat("SET @Tipo = {0}", Tipo);
                    query.AppendLine();
                    query.AppendLine();
                    query.AppendLine("SELECT");
                    query.AppendLine("	[Empleado].[Trab_ID] AS Empleado,");
                    query.AppendLine("	[Empleado].[Curp] AS Curp,");
                    query.AppendLine("	[Empleado].[Paterno] + [Empleado].[Materno] + [Empleado].[Nombre] AS NombreEmpleado,");
                    query.AppendLine("	[TBDepto].[CentroCosto] AS CentroCosto,");
                    query.AppendLine("	SUM(CAST(CASE [NomCalculoHistorico].[Concepto_ID] WHEN 619 THEN [NomCalculoHistorico].[Importe] ELSE 0 END AS MONEY)) AS ImporteEmpleado,");
                    query.AppendLine("	SUM(CAST(CASE [NomCalculoHistorico].[Concepto_ID] WHEN 91 THEN [NomCalculoHistorico].[Importe] ELSE 0 END AS MONEY)) AS ImporteEmpresa,");
                    query.AppendLine("	SUM(CAST(CASE [NomCalculoHistorico].[Concepto_ID] WHEN 93 THEN [NomCalculoHistorico].[Importe] ELSE 0 END AS MONEY)) AS ImporteAdicional");
                    query.AppendLine("FROM");
                    query.AppendLine("	[dbo].[Empleado]");
                    if (Abierta)
                        query.AppendLine("	LEFT JOIN [dbo].[NomCalculo] AS [NomCalculoHistorico] ON [Empleado].[Trab_ID] = [NomCalculoHistorico].[Trab_ID]");
                    else
                        query.AppendLine("	LEFT JOIN [dbo].[NomCalculoHistorico] ON [Empleado].[Trab_ID] = [NomCalculoHistorico].[Trab_ID]");
                    query.AppendLine("	INNER JOIN (");
                    query.AppendLine("		SELECT");
                    query.AppendLine("			[Movimientos].[Trab_ID],");
                    query.AppendLine("			[Movimientos].[FechaMov],");
                    query.AppendLine("			[Movimientos].[Depto_ID]");
                    query.AppendLine("		FROM");
                    query.AppendLine("			[dbo].[Movimiento] AS [Movimientos]");
                    query.AppendLine("			INNER JOIN (");
                    query.AppendLine("				SELECT");
                    query.AppendLine("					[Trab_ID],");
                    query.AppendLine("					MAX([Ptr]) AS [Ptr]");
                    query.AppendLine("				FROM");
                    query.AppendLine("					[dbo].[Movimiento]");
                    query.AppendLine("				GROUP BY");
                    query.AppendLine("					[Trab_ID]");
                    query.AppendLine("				) AS [MaxMovimiento] ON [MaxMovimiento].[Trab_ID] = [Movimientos].[Trab_ID] AND [MaxMovimiento].[Ptr] = [Movimientos].[Ptr]");
                    query.AppendLine("	) AS [Movimiento] ON [Empleado].[Trab_ID] = [Movimiento].[Trab_ID]");
                    query.AppendLine("	INNER JOIN [dbo].[TBDepto] ON [Movimiento].[Depto_ID] = [TBDepto].[Depto_ID]");
                    query.AppendLine("WHERE");
                    query.AppendLine("	[NomCalculoHistorico].Ano = @Año");
                    query.AppendLine("	AND [NomCalculoHistorico].Periodo = @Mes");
                    query.AppendLine("	AND [NomCalculoHistorico].[Concepto_ID] IN (619, 91, 93)");
                    query.AppendLine("GROUP BY");
                    query.AppendLine("	[Empleado].[Trab_ID],");
                    query.AppendLine("	[Empleado].[Curp],");
                    query.AppendLine("	[Empleado].[Paterno],");
                    query.AppendLine("	[Empleado].[Materno],");
                    query.AppendLine("	[Empleado].[Nombre],");
                    query.Append("	[TBDepto].[CentroCosto]");

                    try
                    {
                        SQLDataManager connector = new SQLDataManager();
                        result = connector.ExecuteQuery(query.ToString());
                    }
                    catch (Exception e)
                    {
                        result = null;
                        throw e;
                    }
                }
                else
                    throw new ArgumentException("El periodo no es válido", "Periodo");
            }
            else
                throw new ArgumentException("El año no es válido", "Año");

            return result;
        }

        public DataTable RenderLayoutSkandiaBajas(int Año, int Periodo)
        {
            StringBuilder query = new StringBuilder();
            DataTable result;

            if (Utilities.ValidarAño(Año))
            {
                if (Utilities.ValidarMes(Periodo))
                {
                    query.AppendLine("DECLARE @Mes INT");
                    query.AppendLine("DECLARE @Año INT");
                    query.AppendLine();
                    query.AppendFormat("SET @Mes = {0}", Periodo);
                    query.AppendLine();
                    query.AppendFormat("SET @Año = {0}", Año);
                    query.AppendLine();
                    query.AppendLine();
                    query.AppendLine("SELECT");
                    query.AppendLine("	[Empleado].[Trab_ID] AS Empleado,");
                    query.AppendLine("	[Empleado].[Nombre] AS NombreEmpleado,");
                    query.AppendLine("	[Empleado].[Paterno] AS ApellidoPaternoEmpleado,");
                    query.AppendLine("	[Empleado].[Materno] AS ApellidoMaternoEmpleado,");
                    query.AppendLine("	[Empleado].[Curp] AS Curp,");
                    query.AppendLine("  [Movimiento].[Motivo_ID] AS MotivoBaja");
                    query.AppendLine("FROM");
                    query.AppendLine("	[dbo].[Empleado]");
                    query.AppendLine("	INNER JOIN (");
                    query.AppendLine("		SELECT");
                    query.AppendLine("			[Movimientos].[Trab_ID],");
                    query.AppendLine("			[Movimientos].[FechaMov],");
                    query.AppendLine("			[Movimientos].[Mov_ID],");
                    query.AppendLine("			[Movimientos].[Motivo_ID]");
                    query.AppendLine("		FROM");
                    query.AppendLine("			[dbo].[Movimiento] AS [Movimientos]");
                    query.AppendLine("			INNER JOIN (");
                    query.AppendLine("				SELECT");
                    query.AppendLine("					[Trab_ID],");
                    query.AppendLine("					MAX([Ptr]) AS [Ptr]");
                    query.AppendLine("				FROM");
                    query.AppendLine("					[dbo].[Movimiento]");
                    query.AppendLine("				GROUP BY");
                    query.AppendLine("					[Trab_ID]");
                    query.AppendLine("				) AS [MaxMovimiento] ON [MaxMovimiento].[Trab_ID] = [Movimientos].[Trab_ID] AND [MaxMovimiento].[Ptr] = [Movimientos].[Ptr]");
                    query.AppendLine("	) AS [Movimiento] ON [Empleado].[Trab_ID] = [Movimiento].[Trab_ID]");
                    query.AppendLine("WHERE");
                    query.AppendLine("	YEAR([Movimiento].[FechaMov]) = @Año");
                    query.AppendLine("	AND MONTH([Movimiento].[FechaMov]) = @Mes");
                    query.Append("	AND [Movimiento].[Mov_ID] = 'B'");

                    try
                    {
                        SQLDataManager connector = new SQLDataManager();
                        result = connector.ExecuteQuery(query.ToString());
                    }
                    catch (Exception e)
                    {
                        result = null;
                        throw e;
                    }
                }
                else
                    throw new ArgumentException("El periodo no es válido", "Periodo");
            }
            else
                throw new ArgumentException("El año no es válido", "Año");

            return result;
        }

        public DataTable RenderLayoutInboundSap(int Año, int Periodo, short Tipo, bool Abierta)
        {
            StringBuilder query = new StringBuilder();
            DataTable result;

            if (Utilities.ValidarAño(Año))
            {
                if (Utilities.ValidarMes(Periodo))
                {
                    query.AppendLine("DECLARE @Año INT");
                    query.AppendLine("DECLARE @Mes INT");
                    query.AppendLine("DECLARE @Tipo TINYINT");
                    query.AppendLine();
                    query.AppendFormat("SET @Año = {0}", Año);
                    query.AppendLine();
                    query.AppendFormat("SET @Mes = {0}", Periodo);
                    query.AppendLine();
                    query.AppendFormat("SET @Tipo = {0}", Tipo);
                    query.AppendLine();
                    query.AppendLine();
                    query.AppendLine("SELECT");
                    query.AppendLine("e.Trab_ID AS Empleado,");
                    query.AppendLine("ec.Concepto_Equivalencia AS Concepto,");
                    query.AppendLine("CASE WHEN nc.Concepto_ID >= 601 AND nc.Concepto_ID <= 799 THEN nc.Importe * -1");
                    query.AppendLine("     WHEN nc.Concepto_ID = 801 THEN nc.Importe + ISNULL((SELECT SUM(Importe) * -1 FROM NomCalculo");
                    query.AppendLine("      WHERE Ano = @Año AND Periodo = @Mes AND TipoNomina_ID = @Tipo AND Concepto_ID = 616 AND Trab_ID = e.Trab_ID),0)");

                    if (Tipo == 2)
                    {
                        if (Abierta)
                            query.AppendLine("     WHEN nc.Concepto_ID = 801 THEN nc.Importe + ISNULL((SELECT SUM(Importe) * -1 FROM NomCalculo");
                        else
                            query.AppendLine("     WHEN nc.Concepto_ID = 801 THEN nc.Importe + ISNULL((SELECT SUM(Importe) * -1 FROM NomCalculoHistorico");
                        query.AppendLine("      WHERE Ano = @Año AND Periodo = @Mes AND TipoNomina_ID = @Tipo AND Concepto_ID = 32 AND Trab_ID = e.Trab_ID),0)");
                    }

                    if (Tipo == 2)
                    {
                        if (Abierta)
                            query.AppendLine("     WHEN nc.Concepto_ID = 333 THEN nc.Importe + ISNULL((SELECT Importe FROM NomCalculo");
                        else
                            query.AppendLine("     WHEN nc.Concepto_ID = 333 THEN nc.Importe + ISNULL((SELECT Importe FROM NomCalculoHistorico");
                        query.AppendLine("      WHERE Ano = @Año AND Periodo = @Mes AND TipoNomina_ID = @Tipo AND Concepto_ID = 313 AND Trab_ID = e.Trab_ID),0)");
                    }

                    query.AppendLine("ELSE nc.Importe END AS Importe,");
                    query.AppendLine("e.FechaIngreso");
                    query.AppendLine("FROM Empleado e");
                    if (Abierta)
                        query.AppendLine("INNER JOIN NomCalculo nc ON e.Trab_ID = nc.Trab_ID");
                    else
                        query.AppendLine("INNER JOIN NomCalculoHistorico nc ON e.Trab_ID = nc.Trab_ID");
                    if (Tipo == 2)
                    {
                        query.AppendLine("INNER JOIN (SELECT Concepto_ID, Concepto_Equivalencia FROM EquivalenciaConceptos");
                        query.AppendLine("            WHERE Concepto_ID NOT IN (621) UNION SELECT 623,'5910' UNION SELECT 336, '1003' UNION SELECT 337,'1026')ec");
                    }
                    else
                        query.AppendLine("INNER JOIN EquivalenciaConceptos ec");
                    query.AppendLine("  ON nc.Concepto_ID = ec.Concepto_ID");
                    query.AppendLine("WHERE nc.Ano = @Año");
                    query.AppendLine("AND nc.Periodo = @Mes");
                    query.AppendLine("AND nc.TipoNomina_ID = @Tipo");
                    if (Tipo == 2)
                        query.AppendLine("AND nc.Concepto_ID IN (32,623,324,329,330,331,332,333,334,336,337,604,");
                    else
                        query.AppendLine("AND nc.Concepto_ID IN (621,38,91,");
                    //query.AppendLine("AND nc.Concepto_ID IN (32,621,38,91,");
                    query.AppendLine("93,301,302,304,305,307,311,314,316,317,318,320,321,323,326,327,328,335,601,602,606,607,608,609,");
                    query.AppendLine("610,611,612,615,616,617,618,619,620,622,624,801,905,908,909,910,911,912,913,914,915,916,917)");
                    query.AppendLine("ORDER BY Empleado");

                    try
                    {
                        SQLDataManager connector = new SQLDataManager();
                        result = connector.ExecuteQuery(query.ToString());
                    }
                    catch (Exception e)
                    {
                        result = null;
                        throw e;
                    }
                }
                else
                    throw new ArgumentException("El periodo no es válido", "Periodo");
            }
            else
                throw new ArgumentException("El año no es válido", "Año");

            return result;
        }

        public DataTable RenderLayoutSiValeOperaciones(int Año, int Periodo, OperacionSiVale operacion)
        {
            List<SqlParameter> param = new List<SqlParameter>();
            StringBuilder query = new StringBuilder();
            DataTable result;

            if (Utilities.ValidarAño(Año))
            {
                if (Utilities.ValidarMes(Periodo))
                {
                    param.Add(new SqlParameter("Mes", Periodo));
                    param.Add(new SqlParameter("Año", Año));

                    string spname = string.Empty;
                    switch (operacion)
                    {
                        case OperacionSiVale.MisComprasSolicitud:
                        case OperacionSiVale.MisComprasAlimentacionSolicitud:
                            param.Add(new SqlParameter("Titular", "1"));
                            spname = "usp_LayoutSiValeEmision";
                            break;

                        case OperacionSiVale.MisComprasSolicitudAdicionales:
                        case OperacionSiVale.MisComprasAlimentacionSolicitudAdicionales:
                            param.Add(new SqlParameter("Titular", "0"));
                            spname = "usp_LayoutSiValeEmision";
                            break;

                        case OperacionSiVale.MisComprasDespensaSaldo:
                            param.Add(new SqlParameter("Concepto", "32"));
                            spname = "usp_LayoutSiValeSaldo";
                            break;

                        case OperacionSiVale.MisComprasComidaSaldo:
                            param.Add(new SqlParameter("Concepto", "98"));
                            spname = "usp_LayoutSiValeSaldo";
                            break;

                        case OperacionSiVale.MisComprasAlimentacionSaldo:
                            param.Add(new SqlParameter("Concepto", "97"));
                            spname = "usp_LayoutSiValeSaldo";
                            break;
                    }

                    try
                    {
                        SQLDataManager connector = new SQLDataManager();
                        result = connector.ExecuteProcedure(spname, param);
                    }
                    catch (Exception e)
                    {
                        result = null;
                        throw e;
                    }
                }
                else
                    throw new ArgumentException("El periodo no es válido", "Periodo");
            }
            else
                throw new ArgumentException("El año no es válido", "Año");

            return result;
        }

        #endregion Layouts

        #region Reportes

        public DataTable ReporteListadoNomina(int Año, int Periodo, short Tipo, List<string> Empleados)
        {
            StringBuilder query = new StringBuilder();
            DataTable result;

            if (Utilities.ValidarAño(Año))
            {
                if (Utilities.ValidarMes(Periodo))
                {
                    query.AppendLine("DECLARE @Año INT");
                    query.AppendLine("DECLARE @Mes INT");
                    query.AppendLine("DECLARE @Tipo TINYINT");
                    query.AppendLine("DECLARE @TopeINFONAVIT MONEY");
                    query.AppendLine();
                    query.AppendFormat("SET @Año = {0}", Año);
                    query.AppendLine();
                    query.AppendFormat("SET @Mes = {0}", Periodo);
                    query.AppendLine();
                    query.AppendFormat("SET @Tipo = {0}", Tipo);
                    query.AppendLine();
                    query.AppendLine("SET @TopeINFONAVIT = (SELECT TOP 1 MinZona1 * TopeInfonavit FROM TBMinTop");
                    query.AppendLine("WHERE DATEPART(YEAR,FECHA) = @Año ORDER BY Fecha DESC)");
                    query.AppendLine();

                    query.AppendLine("SELECT DISTINCT");
                    query.AppendLine("  e.Trab_ID AS Empleado,");
                    query.AppendLine("  e.Paterno + ' ' + e.Materno + ' ' + e.Nombre AS Nombre,");
                    query.AppendLine("  tp.Descripcion AS Puesto,");
                    query.AppendLine("  REPLACE(e.RFC, '-', '') AS Rfc,");
                    query.AppendLine("  CAST(e.FechaIngreso AS DATETIME) FechaIngreso,");
                    query.AppendLine("  REPLACE(e.IMSS, '-', '') AS Nss,");
                    query.AppendLine("  e.CURP,");
                    query.AppendLine("  um.Salario Sd,");
                    query.AppendLine("  CASE WHEN um.IntegradoIMSS > @TopeINFONAVIT THEN @TopeINFONAVIT ELSE ISNULL(um.IntegradoIMSS, 0.0) END AS Sdi,");
                    query.AppendLine("  ISNULL(ind.Importe,0) AS SdiIndemnizacion,");
                    query.AppendLine("  te.Descripcion AS Convenio,");
                    query.AppendLine("  cte.Centro_ID AS ClaveCentroTrabajo,");
                    query.AppendLine("  cte.Descripcion AS NombreCentroTrabajo,");
                    query.AppendLine("  SUBSTRING(d.Depto_ID,1,5) AS ClaveDepartamento,");
                    query.AppendLine("  d.Descripcion AS NombreDepartamento,");
                    query.AppendLine("  ec.Concepto_ID,");
                    query.AppendLine("  ec.Concepto_Equivalencia AS Concepto,");
                    query.AppendLine("  CASE WHEN  ec.Concepto_Equivalencia = '3813'  THEN 'GRAVADO AGUINALDO'");
                    query.AppendLine("       WHEN  ec.Concepto_Equivalencia = '6056'  THEN 'EXENTO AGUINALDO'");
                    if (Tipo == 2)//Se agregó para un finiquito
                    {
                        query.AppendLine("      WHEN  ec.Concepto_ID = 313  THEN 'F.A. FINIQ EMPRESA'");
                        query.AppendLine("      WHEN  ec.Concepto_ID = 339  THEN 'DEVOLUCION DE IMSS'");
                        query.AppendLine("      WHEN  ec.Concepto_ID = 340  THEN 'DEVOLUCION DE IMSS'");
                    }
                    query.AppendLine("  ELSE c.Descripcion END AS Descripcion,");
                    query.AppendLine("  nc.Dato AS Unidad,");
                    if (Tipo == 2)//Se agregó para un finiquito
                    {
                        query.AppendLine("  CASE WHEN nc.Concepto_ID = 339 THEN CAST(0 AS MONEY)");
                        query.AppendLine("       WHEN nc.Concepto_ID = 340 THEN CAST(0 AS MONEY)");
                        query.AppendLine("       WHEN nc.Concepto_ID >= 601 AND nc.Concepto_ID <= 799 THEN CAST(0 AS MONEY)");
                    }
                    else
                    {
                        query.AppendLine("  CASE WHEN nc.Concepto_ID >= 601 AND nc.Concepto_ID <= 799 THEN CAST(0 AS MONEY)");
                    }

                    query.AppendLine("       WHEN ec.Concepto_Equivalencia IN ('3813','3823') THEN nc.Gravado");
                    query.AppendLine("       WHEN ec.Concepto_Equivalencia IN ('6056','6057') THEN nc.Excento");
                    query.AppendLine("  ELSE nc.Importe END Percepcion,");
                    if (Tipo == 2)//Se agregó para un finiquito
                    {
                        query.AppendLine("  CASE WHEN nc.Concepto_ID = 339 THEN nc.Importe");
                        query.AppendLine("       WHEN nc.Concepto_ID = 340 THEN nc.Importe");
                        query.AppendLine("       WHEN nc.Concepto_ID >= 601 AND nc.Concepto_ID <= 799 THEN nc.Importe ELSE CAST(0 AS MONEY) END Deduccion,");
                    }
                    else
                    {
                        query.AppendLine("  CASE WHEN nc.Concepto_ID >= 601 AND nc.Concepto_ID <= 799 THEN nc.Importe ELSE CAST(0 AS MONEY) END Deduccion,");
                    }
                    query.AppendLine("  d.CentroCosto");
                    query.AppendLine("FROM Empleado e");
                    query.AppendLine("INNER JOIN (SELECT m.Trab_ID, m.FechaMov, m.Puesto_ID, m.TipoEmpleado_ID,");
                    query.AppendLine("m.Centro_ID, m.Depto_ID, m.Salario, m.IntegradoIMSS");
                    query.AppendLine("FROM Movimiento m INNER JOIN (SELECT Trab_ID, MAX(Ptr) AS Ptr FROM Movimiento");
                    query.AppendLine("GROUP BY Trab_ID) u  ON m.Trab_ID = u.Trab_ID AND m.Ptr = u.Ptr) um");
                    query.AppendLine("ON e.Trab_ID = um.Trab_ID");
                    query.AppendLine("INNER JOIN vw_NomCalculo nc ON e.Trab_ID = nc.Trab_ID");
                    query.AppendLine("INNER JOIN Conceptos c ON nc.Concepto_ID = c.Concepto_ID");

                    if (Tipo == 2)
                    {
                        query.AppendLine("INNER JOIN ( SELECT Concepto_ID, Concepto_Equivalencia");
                        query.AppendLine("FROM EquivalenciaConceptos WHERE Concepto_ID NOT IN (313,318,323,621)");
                        query.AppendLine("UNION SELECT 339,'9500' UNION SELECT 340,'9500'");//Se agregó para un finiquito
                        query.AppendLine("UNION SELECT 318,'3813' UNION SELECT 318, '6056' UNION SELECT 623,'5910' UNION SELECT 336, '1003' UNION SELECT 337,'1026' UNION SELECT 313,'9921')ec");
                    }
                    else if (Tipo == 3)
                    {
                        query.AppendLine("INNER JOIN ( SELECT Concepto_ID, Concepto_Equivalencia");
                        query.AppendLine("FROM EquivalenciaConceptos WHERE Concepto_ID NOT IN (318,323)");
                        query.AppendLine("UNION SELECT 318,'3813' UNION SELECT 318, '6056' UNION SELECT 603, '9316')ec");
                    }
                    else
                    {
                        query.AppendLine("INNER JOIN ( SELECT Concepto_ID, Concepto_Equivalencia");
                        query.AppendLine("FROM EquivalenciaConceptos WHERE Concepto_ID NOT IN (318,323)");
                        query.AppendLine("UNION SELECT 318,'3813' UNION SELECT 318, '6056')ec");
                    }

                    query.AppendLine("ON c.Concepto_ID = ec.Concepto_ID");
                    query.AppendLine("INNER JOIN TBPuesto tp ON um.Puesto_ID = tp.Puesto_ID");
                    query.AppendLine("INNER JOIN TBTipoEmpleado te ON um.TipoEmpleado_ID = te.TipoEmpleado_ID");
                    query.AppendLine("INNER JOIN TBCentros cte ON um.Centro_ID = cte.Centro_ID");
                    query.AppendLine("INNER JOIN TBDepto d ON um.Depto_ID = d.Depto_ID");
                    query.AppendLine("LEFT JOIN vw_NomCalculo ind ON ind.Concepto_ID = 89 AND e.Trab_ID = ind.Trab_ID");
                    query.AppendLine("AND ind.Ano = @Año AND ind.Periodo = @Mes AND ind.TipoNomina_ID = @Tipo");
                    query.AppendLine("WHERE nc.Ano = @Año");
                    query.AppendLine("AND nc.Periodo = @Mes");
                    query.AppendLine("AND nc.TipoNomina_ID = @Tipo");
                    query.AppendLine("AND (nc.Concepto_ID >= 301 AND nc.Concepto_ID <= 599 OR nc.Concepto_ID >= 601 AND nc.Concepto_ID <= 799");
                    if (Tipo == 2)
                        query.AppendLine("OR nc.Concepto_ID = 35 OR nc.Concepto_ID = 43 OR nc.Concepto_ID = 44)");
                    //query.AppendLine("OR nc.Concepto_ID = 32 OR nc.Concepto_ID = 43 OR nc.Concepto_ID = 44)");
                    else
                        query.AppendLine("OR nc.Concepto_ID = 43 OR nc.Concepto_ID = 44)");
                    if (Empleados.Count > 0)
                    {
                        query.AppendLine("AND e.Trab_ID IN (");
                        for (int i = 0; i < Empleados.Count; i++)
                        {
                            if (i == Empleados.Count - 1)
                                query.AppendLine(string.Format("'{0}'", Empleados[i]));
                            else
                                query.AppendLine(string.Format("'{0}',", Empleados[i]));
                        }
                        query.AppendLine(")");
                    }

                    query.AppendLine("ORDER BY ClaveDepartamento, e.Trab_ID, Concepto");

                    try
                    {
                        SQLDataManager connector = new SQLDataManager();
                        result = connector.ExecuteQuery(query.ToString());
                    }
                    catch (Exception e)
                    {
                        result = null;
                        throw e;
                    }
                }
                else
                    throw new ArgumentException("El periodo no es válido", "Periodo");
            }
            else
                throw new ArgumentException("El año no es válido", "Año");

            return result;
        }

        public DataTable ReporteAportacionesFondoDeAhorro(int Año, int Periodo)
        {
            StringBuilder query = new StringBuilder();
            DataTable result;
            DateTime PeriodoInicio, PeriodoFin;

            if (Utilities.ValidarAño(Año))
            {
                if (Utilities.ValidarMes(Periodo))
                {
                    PeriodoInicio = new DateTime(Año, Periodo, 1, 0, 0, 0);
                    PeriodoFin = new DateTime(Año, Periodo, 1, 23, 59, 59).AddMonths(1).AddDays(-1);

                    query.AppendLine("SELECT");
                    query.AppendLine("	'6000HQ1010' AS CentroCosto,");
                    query.AppendLine("	'80081511' AS Empleado,");
                    query.AppendLine("	'BERRONES VALDES CLAUDIA LETICIA' AS NombreEmpleado,");
                    query.AppendLine("	2166.76 AS FondoAhorroEmpleado,");
                    query.AppendLine("	2166.76 AS FondoAhorroEmpresa,");
                    query.AppendLine("	0.00 AS FondoAhorroEmpleadoFin,");
                    query.AppendLine("	0.00 AS FondoAhorroEmpresaFin");
                    query.AppendLine("UNION ALL");
                    query.AppendLine("SELECT '6000HQ1010', '80081591', 'CRESTANI PEREZ BLANCA MARIA', 1358.50, 1358.50, 0.00, 0.00");
                    query.AppendLine("UNION ALL");
                    query.AppendLine("SELECT '6000HQ1010', '80089042', 'GARCIA ELIZONDO AMERICO', 2166.76, 2166.76, 0.00, 0.00");
                    query.AppendLine("UNION ALL");
                    query.AppendLine("SELECT '6000HQ1040', '10143932', 'GARCIA CASTILLO ROCIO ARELI', 2166.76, 2166.76, 0.00, 0.00");
                    query.AppendLine("UNION ALL");
                    query.AppendLine("SELECT '6000HQ1040', '80001966', 'CLAVEL PALOMEQUE YARA GABRIELA', 2142.40, 2142.40, 0.00, 0.00");
                    query.AppendLine("UNION ALL");
                    query.AppendLine("SELECT '6000HQ1040', '80079337', 'DE LA TORRE VILLANUEVA ABRAHAM YAMIR', 1669.20, 1669.20, 0.00, 0.00");
                    query.AppendLine("UNION ALL");
                    query.AppendLine("SELECT '6000HQ1040', '80079957', 'PALOMINO CERVANTES FERNANDO', 1669.20, 1669.20, 0.00, 0.00");
                    query.AppendLine("UNION ALL");
                    query.AppendLine("SELECT '6000HQ1040', '80087363', 'ALVARADO PAZ KEVIN DAVID STEFANO', 1560.00, 1560.00, 0.00, 0.00");
                    query.AppendLine("UNION ALL");
                    query.Append("SELECT '6000HQ1040', '80088845', 'SANDOVAL PERALTA MARIO ANTONIO', 1820.00, 1820.00, 0.00, 0.00");

                    try
                    {
                        SQLDataManager connector = new SQLDataManager();
                        result = connector.ExecuteQuery(query.ToString());
                    }
                    catch (Exception e)
                    {
                        result = null;
                        throw e;
                    }
                }
                else
                    throw new ArgumentException("El periodo no es válido", "Periodo");
            }
            else
                throw new ArgumentException("El año no es válido", "Año");

            return result;
        }

        public DataTable ReporteAportacionesPlanPensiones(int Año, int Periodo, short Tipo, bool Abierta)
        {
            StringBuilder query = new StringBuilder();
            DataTable result;
            DateTime PeriodoInicio, PeriodoFin;

            if (Utilities.ValidarAño(Año))
            {
                if (Utilities.ValidarMes(Periodo))
                {
                    PeriodoInicio = new DateTime(Año, Periodo, 1, 0, 0, 0);
                    PeriodoFin = new DateTime(Año, Periodo, 1, 23, 59, 59).AddMonths(1).AddDays(-1);

                    query.AppendLine("DECLARE @Año INT ");
                    query.AppendLine("DECLARE @Mes INT ");
                    query.AppendLine("DECLARE @Tipo TINYINT ");
                    query.AppendFormat("SET @Año = {0} ", Año);
                    query.AppendFormat("SET @Mes = {0} ", Periodo);
                    query.AppendFormat("SET @Tipo = {0} ", Tipo);
                    query.AppendLine();
                    query.AppendLine("SELECT ");
                    query.AppendLine("    d.CentroCosto,");
                    query.AppendLine("	ISNULL(d.CuentaAcumuladora, '<Sin nombre>') AS Descripcion,");
                    query.AppendLine("    e.Trab_ID,");
                    query.AppendLine("    e.Paterno + ' ' + e.Materno + ' ' + e.Nombre  NombreEmpleado,");
                    query.AppendLine("    ISNULL(CASE WHEN nc.Concepto_ID = 91 THEN nc.Importe ELSE 0 END,0 ) Fijo,");
                    query.AppendLine("    ISNULL(CASE WHEN nc.Concepto_ID = 93 THEN nc.Importe ELSE 0 END,0 ) Match,");
                    query.AppendLine("    ISNULL(CASE WHEN nc.Concepto_ID = 619 THEN nc.Importe ELSE 0 END,0 ) Empleado ");
                    query.AppendLine("FROM Empleado e ");
                    query.AppendLine("INNER JOIN (SELECT  ");
                    query.AppendLine("                m.Trab_ID, m.FechaMov, m.Puesto_ID, m.TipoEmpleado_ID,");
                    query.AppendLine("                m.Centro_ID, m.Depto_ID, m.Salario,IntegradoIMSS");
                    query.AppendLine("            FROM Movimiento m ");
                    query.AppendLine("            INNER JOIN (SELECT Trab_ID, MAX(Ptr) Ptr FROM Movimiento GROUP BY Trab_ID) mm ");
                    query.AppendLine("            ON m.Trab_ID = mm.Trab_ID AND m.Ptr = mm.Ptr) m");
                    query.AppendLine("    ON e.Trab_ID = m.Trab_ID ");
                    query.AppendLine("INNER JOIN TBDepto d ON m.Depto_ID = d.Depto_ID ");
                    if (Abierta)
                        query.AppendLine("INNER JOIN NomCalculo nc ON e.Trab_ID = nc.Trab_ID ");
                    else
                        query.AppendLine("INNER JOIN NomCalculoHistorico nc ON e.Trab_ID = nc.Trab_ID ");
                    query.AppendLine("WHERE nc.Concepto_ID IN (91, 93, 619) ");
                    query.AppendLine("AND nc.Ano = @Año ");
                    query.AppendLine("AND nc.Periodo = @Mes ");
                    query.AppendLine("AND (nc.TipoNomina_ID = @Tipo OR @Tipo = 0)");
                    query.AppendLine("ORDER BY d.CentroCosto,e.Trab_ID ");

                    try
                    {
                        SQLDataManager connector = new SQLDataManager();
                        result = connector.ExecuteQuery(query.ToString());
                    }
                    catch (Exception e)
                    {
                        result = null;
                        throw e;
                    }
                }
                else
                    throw new ArgumentException("El periodo no es válido", "Periodo");
            }
            else
                throw new ArgumentException("El año no es válido", "Año");

            return result;
        }

        public DataTable ReporteControlCambioNomina_Altas(int Año, int Periodo)
        {
            StringBuilder query = new StringBuilder();
            DataTable result;
            DateTime PeriodoInicio, PeriodoFin;

            if (Utilities.ValidarAño(Año))
            {
                if (Utilities.ValidarMes(Periodo))
                {
                    PeriodoInicio = new DateTime(Año, Periodo, 1, 0, 0, 0);
                    PeriodoFin = new DateTime(Año, Periodo, 1, 23, 59, 59).AddMonths(1).AddDays(-1);

                    query.AppendLine("DECLARE @Año INT");
                    query.AppendLine("DECLARE @Mes INT");
                    query.AppendLine("DECLARE @FechaIni DATETIME");
                    query.AppendLine("DECLARE @FechaFin DATETIME");
                    query.AppendLine("SET DATEFORMAT dmy");
                    query.AppendFormat("SET @Año = {0}", Año);
                    query.AppendLine();
                    query.AppendFormat("SET @Mes = {0}", Periodo);
                    query.AppendLine();
                    query.AppendLine("SET @FechaIni = CAST('1/' + CAST(@Mes AS CHAR) + '/' + CAST(@Año AS CHAR)  AS DATETIME)");
                    query.AppendLine("SET @FechaFin = DATEADD(MONTH,1,@FechaIni)");
                    query.AppendLine("SELECT");
                    query.AppendLine("  e.Trab_ID,");
                    query.AppendLine("  e.Paterno + ' ' + e.Materno + ' ' + e.Nombre  NombreEmpleado,");
                    query.AppendLine("  CONVERT(CHAR(10),mo.FechaMov,103) FechaMov,");
                    query.AppendLine("  CASE WHEN m.Mov_ID = 'R' THEN 'Reingreso' ELSE '' END Detalle");
                    query.AppendLine("FROM Movimiento mo");
                    query.AppendLine("INNER JOIN TBMotivo m ON mo.Motivo_ID  = m.Motivo_ID AND m.Mov_ID IN ('A','R')");
                    query.AppendLine("INNER JOIN Empleado e ON mo.Trab_ID = e.Trab_ID");
                    query.AppendLine("INNER JOIN (SELECT");
                    query.AppendLine("              m.Trab_ID, m.FechaMov, m.Puesto_ID, m.TipoEmpleado_ID,");
                    query.AppendLine("              m.Centro_ID, m.Depto_ID, m.Salario,IntegradoIMSS");
                    query.AppendLine("            FROM Movimiento m");
                    query.AppendLine("            INNER JOIN (SELECT Trab_ID, MAX(Ptr) Ptr FROM Movimiento GROUP BY Trab_ID) mm");
                    query.AppendLine("             ON m.Trab_ID = mm.Trab_ID AND m.Ptr = mm.Ptr) um");
                    query.AppendLine("  ON mo.Trab_ID = um.Trab_ID");
                    query.AppendLine("WHERE mo.FechaMov BETWEEN @FechaIni AND @FechaFin");

                    try
                    {
                        SQLDataManager connector = new SQLDataManager();
                        result = connector.ExecuteQuery(query.ToString());
                    }
                    catch (Exception e)
                    {
                        result = null;
                        throw e;
                    }
                }
                else
                    throw new ArgumentException("El periodo no es válido", "Periodo");
            }
            else
                throw new ArgumentException("El año no es válido", "Año");

            return result;
        }

        public DataTable ReporteControlCambioNomina_Bajas(int Año, int Periodo)
        {
            StringBuilder query = new StringBuilder();
            DataTable result;
            DateTime PeriodoInicio, PeriodoFin;

            if (Utilities.ValidarAño(Año))
            {
                if (Utilities.ValidarMes(Periodo))
                {
                    PeriodoInicio = new DateTime(Año, Periodo, 1, 0, 0, 0);
                    PeriodoFin = new DateTime(Año, Periodo, 1, 23, 59, 59).AddMonths(1).AddDays(-1);

                    query.AppendLine("DECLARE @Año INT");
                    query.AppendLine("DECLARE @Mes INT");
                    query.AppendLine("DECLARE @FechaIni DATETIME");
                    query.AppendLine("DECLARE @FechaFin DATETIME");
                    query.AppendLine("SET DATEFORMAT dmy");
                    query.AppendFormat("SET @Año = {0}", Año);
                    query.AppendLine();
                    query.AppendFormat("SET @Mes = {0}", Periodo);
                    query.AppendLine();
                    query.AppendLine("SET @FechaIni = CAST('1/' + CAST(@Mes AS CHAR) + '/' + CAST(@Año AS CHAR)  AS DATETIME)");
                    query.AppendLine("SET @FechaFin = DATEADD(MONTH,1,@FechaIni)");

                    query.AppendLine("SELECT");
                    query.AppendLine("  e.Trab_ID,");
                    query.AppendLine("  e.Paterno + ' ' + e.Materno + ' ' + e.Nombre  NombreEmpleado,");
                    query.AppendLine("  CONVERT(CHAR(10),mo.FechaMov,103) FechaMov,");
                    query.AppendLine("  m.Descripcion Motivo");
                    query.AppendLine("FROM Movimiento mo");
                    query.AppendLine("INNER JOIN TBMotivo m ON mo.Motivo_ID  = m.Motivo_ID AND m.Mov_ID = 'B'");
                    query.AppendLine("INNER JOIN Empleado e ON mo.Trab_ID = e.Trab_ID");
                    query.AppendLine("INNER JOIN (SELECT");
                    query.AppendLine("              m.Trab_ID, m.FechaMov, m.Puesto_ID, m.TipoEmpleado_ID,");
                    query.AppendLine("              m.Centro_ID, m.Depto_ID, m.Salario,IntegradoIMSS");
                    query.AppendLine("            FROM Movimiento m");
                    query.AppendLine("            INNER JOIN (SELECT Trab_ID, MAX(Ptr) Ptr FROM Movimiento GROUP BY Trab_ID) mm");
                    query.AppendLine("              ON m.Trab_ID = mm.Trab_ID AND m.Ptr = mm.Ptr) um");
                    query.AppendLine("  ON mo.Trab_ID = um.Trab_ID");
                    query.AppendLine("WHERE mo.FechaMov BETWEEN @FechaIni AND @FechaFin");

                    try
                    {
                        SQLDataManager connector = new SQLDataManager();
                        result = connector.ExecuteQuery(query.ToString());
                    }
                    catch (Exception e)
                    {
                        result = null;
                        throw e;
                    }
                }
                else
                    throw new ArgumentException("El periodo no es válido", "Periodo");
            }
            else
                throw new ArgumentException("El año no es válido", "Año");

            return result;
        }

        public DataTable ReporteControlCambioNomina_OtrasPercepciones(int Año, int Periodo)
        {
            StringBuilder query = new StringBuilder();
            DataTable result;
            DateTime PeriodoInicio, PeriodoFin;

            if (Utilities.ValidarAño(Año))
            {
                if (Utilities.ValidarMes(Periodo))
                {
                    PeriodoInicio = new DateTime(Año, Periodo, 1, 0, 0, 0);
                    PeriodoFin = new DateTime(Año, Periodo, 1, 23, 59, 59).AddMonths(1).AddDays(-1);

                    query.AppendLine("DECLARE @Año INT");
                    query.AppendLine("DECLARE @Mes INT");
                    query.AppendLine("DECLARE @FechaIni DATETIME");
                    query.AppendLine("DECLARE @FechaFin DATETIME");
                    query.AppendLine("SET DATEFORMAT dmy");
                    query.AppendFormat("SET @Año = {0}", Año);
                    query.AppendLine();
                    query.AppendFormat("SET @Mes = {0}", Periodo);
                    query.AppendLine();
                    query.AppendLine("SET @FechaIni = CAST('1/' + CAST(@Mes AS CHAR) + '/' + CAST(@Año AS CHAR)  AS DATETIME)");
                    query.AppendLine("SET @FechaFin = DATEADD(MONTH,1,@FechaIni)");

                    query.AppendLine("SELECT");
                    query.AppendLine("  e.Trab_ID,");
                    query.AppendLine("  e.Paterno + ' ' + e.Materno + ' ' + e.Nombre  NombreEmpleado,");
                    query.AppendLine("  c.Descripcion Concepto,");
                    query.AppendLine("  CASE WHEN nm.Importe = 0 THEN nm.Dato ELSE nm.Importe END Importe");
                    query.AppendLine("FROM NomMovimientos nm");
                    query.AppendLine("INNER JOIN Empleado e ON nm.Trab_ID = e.Trab_ID");
                    query.AppendLine("INNER JOIN Conceptos c ON nm.Concepto_ID = c.Concepto_ID");
                    query.AppendLine("WHERE c.Concepto_ID BETWEEN 301 AND 599");
                    query.AppendLine("AND nm.Fecha BETWEEN @FechaIni AND @FechaFin");

                    try
                    {
                        SQLDataManager connector = new SQLDataManager();
                        result = connector.ExecuteQuery(query.ToString());
                    }
                    catch (Exception e)
                    {
                        result = null;
                        throw e;
                    }
                }
                else
                    throw new ArgumentException("El periodo no es válido", "Periodo");
            }
            else
                throw new ArgumentException("El año no es válido", "Año");

            return result;
        }

        public DataTable ReporteControlCambioNomina_OtrasDeducciones(int Año, int Periodo)
        {
            StringBuilder query = new StringBuilder();
            DataTable result;
            DateTime PeriodoInicio, PeriodoFin;

            if (Utilities.ValidarAño(Año))
            {
                if (Utilities.ValidarMes(Periodo))
                {
                    PeriodoInicio = new DateTime(Año, Periodo, 1, 0, 0, 0);
                    PeriodoFin = new DateTime(Año, Periodo, 1, 23, 59, 59).AddMonths(1).AddDays(-1);

                    query.AppendLine("DECLARE @Año INT");
                    query.AppendLine("DECLARE @Mes INT");
                    query.AppendLine("DECLARE @FechaIni DATETIME");
                    query.AppendLine("DECLARE @FechaFin DATETIME");
                    query.AppendLine("SET DATEFORMAT dmy");
                    query.AppendFormat("SET @Año = {0}", Año);
                    query.AppendLine();
                    query.AppendFormat("SET @Mes = {0}", Periodo);
                    query.AppendLine();
                    query.AppendLine("SET @FechaIni = CAST('1/' + CAST(@Mes AS CHAR) + '/' + CAST(@Año AS CHAR)  AS DATETIME)");
                    query.AppendLine("SET @FechaFin = DATEADD(MONTH,1,@FechaIni)");

                    query.AppendLine("SELECT");
                    query.AppendLine("  e.Trab_ID,");
                    query.AppendLine("  e.Paterno + ' ' + e.Materno + ' ' + e.Nombre  NombreEmpleado,");
                    query.AppendLine("  c.Descripcion Concepto,");
                    query.AppendLine("  CASE WHEN nm.Importe = 0 THEN nm.Dato ELSE nm.Importe END Importe");
                    query.AppendLine("FROM NomMovimientos nm");
                    query.AppendLine("INNER JOIN Empleado e ON nm.Trab_ID = e.Trab_ID");
                    query.AppendLine("INNER JOIN Conceptos c ON nm.Concepto_ID = c.Concepto_ID");
                    query.AppendLine("WHERE c.Concepto_ID BETWEEN 601 AND 799");
                    query.AppendLine("AND  c.Concepto_ID NOT IN (603, 625)");
                    query.AppendLine("AND nm.Fecha BETWEEN @FechaIni AND @FechaFin");

                    try
                    {
                        SQLDataManager connector = new SQLDataManager();
                        result = connector.ExecuteQuery(query.ToString());
                    }
                    catch (Exception e)
                    {
                        result = null;
                        throw e;
                    }
                }
                else
                    throw new ArgumentException("El periodo no es válido", "Periodo");
            }
            else
                throw new ArgumentException("El año no es válido", "Año");

            return result;
        }

        public DataTable ReporteControlCambioNomina_CambiosCuenta(int Año, int Periodo)
        {
            StringBuilder query = new StringBuilder();
            DataTable result;
            DateTime PeriodoInicio, PeriodoFin;

            if (Utilities.ValidarAño(Año))
            {
                if (Utilities.ValidarMes(Periodo))
                {
                    PeriodoInicio = new DateTime(Año, Periodo, 1, 0, 0, 0);
                    PeriodoFin = new DateTime(Año, Periodo, 1, 23, 59, 59).AddMonths(1).AddDays(-1);

                    query.AppendLine("DECLARE @Año INT");
                    query.AppendLine("DECLARE @Mes INT");
                    query.AppendLine("DECLARE @FechaIni DATETIME");
                    query.AppendLine("DECLARE @FechaFin DATETIME");
                    query.AppendLine("SET DATEFORMAT dmy");
                    query.AppendFormat("SET @Año = {0}", Año);
                    query.AppendLine();
                    query.AppendFormat("SET @Mes = {0}", Periodo);
                    query.AppendLine();
                    query.AppendLine("SET @FechaIni = CAST('1/' + CAST(@Mes AS CHAR) + '/' + CAST(@Año AS CHAR)  AS DATETIME)");
                    query.AppendLine("SET @FechaFin = DATEADD(MONTH,1,@FechaIni)");

                    query.AppendLine("SELECT");
                    query.AppendLine("e.Trab_ID,");
                    query.AppendLine("e.Paterno + ' ' + e.Materno + ' ' + e.Nombre  NombreEmpleado,");
                    query.AppendLine("b.Descripcion BancoDeposito,");
                    query.AppendLine("mo.ValorNuevo CuentaDeposito");
                    query.AppendLine("FROM EmpleadoActualiza mo");
                    query.AppendLine("INNER JOIN Empleado e ON mo.Trab_ID = e.Trab_ID");
                    query.AppendLine("INNER JOIN TBBanco b ON e.BancoDeposito = CAST(b.Banco_ID AS INT)");
                    query.AppendLine("WHERE mo.Campo = 'CuentaDeposito'");
                    query.AppendLine("AND mo.FechaCaptura BETWEEN @FechaIni AND @FechaFin");

                    try
                    {
                        SQLDataManager connector = new SQLDataManager();
                        result = connector.ExecuteQuery(query.ToString());
                    }
                    catch (Exception e)
                    {
                        result = null;
                        throw e;
                    }
                }
                else
                    throw new ArgumentException("El periodo no es válido", "Periodo");
            }
            else
                throw new ArgumentException("El año no es válido", "Año");

            return result;
        }

        public DataTable ReporteControlCambioNomina_OtrosCambios(int Año, int Periodo)
        {
            StringBuilder query = new StringBuilder();
            DataTable result;

            if (Utilities.ValidarAño(Año))
            {
                if (Utilities.ValidarMes(Periodo))
                {
                    try
                    {
                        SQLDataManager connector = new SQLDataManager();
                        List<SqlParameter> parameters = new List<SqlParameter>();
                        parameters.Add(new SqlParameter("@Año", SqlDbType.Int, 4, ParameterDirection.Input, false, 10, 0, "", DataRowVersion.Current, Año));
                        parameters.Add(new SqlParameter("@Mes", SqlDbType.Int, 4, ParameterDirection.Input, false, 10, 0, "", DataRowVersion.Current, Periodo));
                        result = connector.ExecuteProcedure("usp_NominaControlCambiosOtros", parameters);
                    }
                    catch (Exception e)
                    {
                        result = null;
                        throw e;
                    }
                }
                else
                    throw new ArgumentException("El periodo no es válido", "Periodo");
            }
            else
                throw new ArgumentException("El año no es válido", "Año");

            return result;
        }

        public DataTable ReporteEmpleadosPorConcepto(int Año, int Periodo, short Tipo, bool Abierta)
        {
            StringBuilder query = new StringBuilder();
            DataTable result;

            if (Utilities.ValidarAño(Año))
            {
                if (Utilities.ValidarMes(Periodo))
                {
                    query.AppendLine("DECLARE @Año INT");
                    query.AppendLine("DECLARE @Mes INT");
                    query.AppendLine("DECLARE @Tipo TINYINT");
                    query.AppendLine();
                    query.AppendFormat("SET @Año = {0}", Año);
                    query.AppendLine();
                    query.AppendFormat("SET @Mes = {0}", Periodo);
                    query.AppendLine();
                    query.AppendFormat("SET @Tipo = {0}", Tipo);
                    query.AppendLine();
                    query.AppendLine();
                    query.AppendLine("SELECT DISTINCT");
                    query.AppendLine("	[Movimiento].[Trab_ID] AS Empleado,");
                    query.AppendLine("	[Empleado].[Paterno] + ' ' + [Empleado].[Materno] + ' ' + [Empleado].[Nombre] AS Nombre,");
                    query.AppendLine("	[TBCentros].[Centro_ID] AS ClaveCentroTrabajo,");
                    query.AppendLine("	[TBCentros].[Descripcion] AS NombreCentroTrabajo,");
                    query.AppendLine("	SUBSTRING([TBDepto].[Depto_ID],1,5) AS ClaveDepartamento,");
                    query.AppendLine("	[TBDepto].[Descripcion] AS NombreDepartamento,");
                    query.AppendLine("	ISNULL([EquivalenciaConceptos].[Concepto_Equivalencia], [NomCalculoHistorico].[Concepto_ID]) AS Concepto,");
                    query.AppendLine("	[Conceptos].[Descripcion] AS Descripcion,");
                    query.AppendLine("	CAST(CASE WHEN [NomCalculoHistorico].[Concepto_ID] >= 301 AND [NomCalculoHistorico].[Concepto_ID] <= 599 THEN [NomCalculoHistorico].[Importe] ELSE 0 END AS MONEY) AS Percepcion,");
                    query.AppendLine("	CAST(CASE WHEN [NomCalculoHistorico].[Concepto_ID] >= 601 AND [NomCalculoHistorico].[Concepto_ID] <= 799 THEN [NomCalculoHistorico].[Importe] ELSE 0 END AS MONEY) AS Deduccion");
                    query.AppendLine("FROM");
                    query.AppendLine("	[dbo].[Empleado]");
                    if (Abierta)
                        query.AppendLine("	LEFT JOIN [dbo].[NomCalculo] AS [NomCalculoHistorico] ON [Empleado].[Trab_ID] = [NomCalculoHistorico].[Trab_ID]");
                    else
                        query.AppendLine("	LEFT JOIN [dbo].[NomCalculoHistorico] ON [Empleado].[Trab_ID] = [NomCalculoHistorico].[Trab_ID]");
                    query.AppendLine("	INNER JOIN (");
                    query.AppendLine("		SELECT");
                    query.AppendLine("			[Movimientos].[Trab_ID],");
                    query.AppendLine("			[Movimientos].[FechaMov],");
                    query.AppendLine("			[Movimientos].[Puesto_ID],");
                    query.AppendLine("			[Movimientos].[TipoEmpleado_ID],");
                    query.AppendLine("			[Movimientos].[Centro_ID],");
                    query.AppendLine("			[Movimientos].[Depto_ID],");
                    query.AppendLine("			[Movimientos].[Salario]");
                    query.AppendLine("		FROM");
                    query.AppendLine("			[dbo].[Movimiento] AS [Movimientos]");
                    query.AppendLine("			INNER JOIN (");
                    query.AppendLine("				SELECT");
                    query.AppendLine("					[Trab_ID],");
                    query.AppendLine("					MAX([Ptr]) AS [Ptr]");
                    query.AppendLine("				FROM");
                    query.AppendLine("					[dbo].[Movimiento]");
                    query.AppendLine("				GROUP BY");
                    query.AppendLine("					[Trab_ID]");
                    query.AppendLine("				) AS [MaxMovimiento] ON [MaxMovimiento].[Trab_ID] = [Movimientos].[Trab_ID] AND [MaxMovimiento].[Ptr] = [Movimientos].[Ptr]");
                    query.AppendLine("	) AS [Movimiento] ON [Empleado].[Trab_ID] = [Movimiento].[Trab_ID]");
                    query.AppendLine("	INNER JOIN [dbo].[TBPuesto] ON [Movimiento].[Puesto_ID] = [TBPuesto].[Puesto_ID]");
                    query.AppendLine("	INNER JOIN [dbo].[TBTipoEmpleado] ON [Movimiento].[TipoEmpleado_ID] = [TBTipoEmpleado].[TipoEmpleado_ID]");
                    query.AppendLine("	INNER JOIN [dbo].[TBCentros] ON [Movimiento].[Centro_ID] = [TBCentros].[Centro_ID]");
                    query.AppendLine("	INNER JOIN [dbo].[TBDepto] ON [Movimiento].[Depto_ID] = [TBDepto].[Depto_ID]");
                    query.AppendLine("	LEFT JOIN [dbo].[IMSSLiquidacion97] ON [Movimiento].[Trab_ID] = [IMSSLiquidacion97].[Trab_ID] AND YEAR([Movimiento].[FechaMov]) = [IMSSLiquidacion97].[Ano] AND MONTH([Movimiento].[FechaMov]) = [IMSSLiquidacion97].[Periodo]");
                    query.AppendLine("	INNER JOIN [dbo].[Conceptos] ON [NomCalculoHistorico].[Concepto_ID] = [Conceptos].[Concepto_ID]");
                    query.AppendLine("	LEFT JOIN [dbo].[EquivalenciaConceptos] ON [Conceptos].[Concepto_ID] = [EquivalenciaConceptos].[Concepto_ID]");
                    query.AppendLine("WHERE");
                    query.AppendLine("	[NomCalculoHistorico].[Ano] = @Año");
                    query.AppendLine("	AND [NomCalculoHistorico].[Periodo] = @Mes");
                    query.AppendLine("	AND ([NomCalculoHistorico].[TipoNomina_ID] = @Tipo OR @Tipo = 0)");
                    query.AppendLine("	AND (");
                    query.AppendLine("		[NomCalculoHistorico].[Concepto_ID] >= 301 AND [NomCalculoHistorico].[Concepto_ID] <= 599");
                    query.AppendLine("		OR [NomCalculoHistorico].[Concepto_ID] >= 601 AND [NomCalculoHistorico].[Concepto_ID] <= 799");
                    query.Append("	)");
                    query.Append("ORDER BY Concepto,ClaveDepartamento, Empleado");

                    try
                    {
                        SQLDataManager connector = new SQLDataManager();
                        result = connector.ExecuteQuery(query.ToString());
                    }
                    catch (Exception e)
                    {
                        result = null;
                        throw e;
                    }
                }
                else
                    throw new ArgumentException("El periodo no es válido", "Periodo");
            }
            else
                throw new ArgumentException("El año no es válido", "Año");

            return result;
        }

        public DataTable ReporteFondoDeAhorro(int Año, int Periodo, short Tipo, bool Abierta)
        {
            StringBuilder query = new StringBuilder();
            DataTable result;
            DateTime PeriodoInicio, PeriodoFin;

            if (Utilities.ValidarAño(Año))
            {
                if (Utilities.ValidarMes(Periodo))
                {
                    PeriodoInicio = new DateTime(Año, Periodo, 1, 0, 0, 0);
                    PeriodoFin = new DateTime(Año, Periodo, 1, 23, 59, 59).AddMonths(1).AddDays(-1);

                    query.AppendLine("DECLARE @Año INT ");
                    query.AppendLine("DECLARE @Mes INT ");
                    query.AppendLine("DECLARE @Tipo TINYINT ");
                    query.AppendFormat("SET @Año = {0} ", Año);
                    query.AppendFormat("SET @Mes = {0} ", Periodo);
                    query.AppendFormat("SET @Tipo = {0} ", Tipo);
                    query.AppendLine();
                    query.AppendLine("SELECT  ");
                    query.AppendLine("d.CentroCosto,");
                    query.AppendLine("d.CuentaAcumuladora Descripcion,");
                    query.AppendLine("m.Trab_ID,");
                    query.AppendLine("e.Paterno + ' ' + e.Materno + ' ' + e.Nombre  NombreEmpleado,");
                    query.AppendLine("SUM(CASE WHEN c.Concepto_ID = 621 THEN nc.Importe ELSE CAST(0 AS MONEY) END) Empleado,");
                    query.AppendLine("SUM(CASE WHEN c.Concepto_ID = 38 THEN nc.Importe ELSE CAST(0 AS MONEY) END) Empresa,");
                    query.AppendLine("SUM(CASE WHEN c.Concepto_ID = 334 THEN nc.Importe ELSE CAST(0 AS MONEY) END) EmpleadoFiniquito,");
                    query.AppendLine("SUM(CASE WHEN c.Concepto_ID = 333 THEN nc.Importe ELSE CAST(0 AS MONEY) END) EmpresaFiniquito ");
                    if (Abierta)
                        query.AppendLine("FROM NomCalculo nc ");
                    else
                        query.AppendLine("FROM NomCalculoHistorico nc ");
                    query.AppendLine("INNER JOIN Empleado e ON nc.Trab_ID = e.Trab_ID ");
                    query.AppendLine("INNER JOIN Conceptos c ON nc.Concepto_ID = c.Concepto_ID ");
                    query.AppendLine("INNER JOIN (SELECT  ");
                    query.AppendLine("m.Trab_ID, m.FechaMov, m.Puesto_ID, m.TipoEmpleado_ID,  ");
                    query.AppendLine("m.Centro_ID, m.Depto_ID, m.Salario,IntegradoIMSS ");
                    query.AppendLine("FROM Movimiento m ");
                    query.AppendLine("INNER JOIN (SELECT Trab_ID, MAX(Ptr) Ptr FROM Movimiento GROUP BY Trab_ID) mm ");
                    query.AppendLine("ON m.Trab_ID = mm.Trab_ID AND m.Ptr = mm.Ptr) m ");
                    query.AppendLine("ON nc.Trab_ID = m.Trab_ID ");
                    query.AppendLine("INNER JOIN TBDepto d ON m.Depto_ID = d.Depto_ID ");
                    query.AppendLine("WHERE c.Concepto_ID IN (38,621) ");
                    query.AppendLine("AND nc.Ano = @Año ");
                    query.AppendLine("AND nc.Periodo = @Mes ");
                    query.AppendLine("AND (nc.TipoNomina_ID = @Tipo OR @Tipo = 0)");
                    query.AppendLine("GROUP BY d.CentroCosto, d.CuentaAcumuladora, m.Trab_ID , e.Paterno, e.Materno, e.Nombre");
                    query.AppendLine("ORDER BY d.CentroCosto, m.Trab_ID ");

                    try
                    {
                        SQLDataManager connector = new SQLDataManager();
                        result = connector.ExecuteQuery(query.ToString());
                    }
                    catch (Exception e)
                    {
                        result = null;
                        throw e;
                    }
                }
                else
                    throw new ArgumentException("El periodo no es válido", "Periodo");
            }
            else
                throw new ArgumentException("El año no es válido", "Año");

            return result;
        }

        public DataTable ReporteListadoDeValesDeDespensaCentroDeCostos(int Año, int Periodo, short Tipo, bool Abierta)
        {
            StringBuilder query = new StringBuilder();
            DataTable result;
            DateTime PeriodoInicio, PeriodoFin;

            if (Utilities.ValidarAño(Año))
            {
                if (Utilities.ValidarMes(Periodo))
                {
                    PeriodoInicio = new DateTime(Año, Periodo, 1, 0, 0, 0);
                    PeriodoFin = new DateTime(Año, Periodo, 1, 23, 59, 59).AddMonths(1).AddDays(-1);

                    query.AppendLine("DECLARE @Mes INT");
                    query.AppendLine("DECLARE @Año INT");
                    query.AppendLine("DECLARE @Tipo TINYINT");
                    query.AppendLine();
                    query.AppendFormat("SET @Año = {0}", Año);
                    query.AppendLine();
                    query.AppendFormat("SET @Mes = {0}", Periodo);
                    query.AppendLine();
                    query.AppendFormat("SET @Tipo = {0}", Tipo);
                    query.AppendLine();
                    query.AppendLine();
                    query.AppendLine("SELECT");
                    query.AppendLine("	[TBDepto].[CentroCosto] AS CentroCosto,");
                    query.AppendLine("	ISNULL([TBDepto].[CuentaAcumuladora], '<Sin nombre>') AS Descripcion,");
                    query.AppendLine("	ISNULL(SUM([NomCalculoHistorico].[Importe]), 0) AS Despensa");
                    query.AppendLine("FROM");
                    query.AppendLine("	[dbo].[Empleado]");
                    if (Abierta)
                        query.AppendLine("	LEFT JOIN [dbo].[NomCalculo] AS [NomCalculoHistorico] ON [Empleado].[Trab_ID] = [NomCalculoHistorico].[Trab_ID]");
                    else
                        query.AppendLine("	LEFT JOIN [dbo].[NomCalculoHistorico] ON [Empleado].[Trab_ID] = [NomCalculoHistorico].[Trab_ID]");
                    query.AppendLine("	INNER JOIN [dbo].[Conceptos] ON [NomCalculoHistorico].[Concepto_ID] = [Conceptos].[Concepto_ID]");
                    query.AppendLine("	INNER JOIN (");
                    query.AppendLine("		SELECT");
                    query.AppendLine("			[Movimientos].[Trab_ID],");
                    query.AppendLine("			[Movimientos].[FechaMov],");
                    query.AppendLine("			[Movimientos].[Puesto_ID],");
                    query.AppendLine("			[Movimientos].[TipoEmpleado_ID],");
                    query.AppendLine("			[Movimientos].[Centro_ID],");
                    query.AppendLine("			[Movimientos].[Depto_ID],");
                    query.AppendLine("			[Movimientos].[Salario]");
                    query.AppendLine("		FROM");
                    query.AppendLine("			[dbo].[Movimiento] AS [Movimientos]");
                    query.AppendLine("			INNER JOIN (");
                    query.AppendLine("				SELECT");
                    query.AppendLine("					[Trab_ID],");
                    query.AppendLine("					MAX([Ptr]) AS [Ptr]");
                    query.AppendLine("				FROM");
                    query.AppendLine("					[dbo].[Movimiento]");
                    query.AppendLine("				GROUP BY");
                    query.AppendLine("					[Trab_ID]");
                    query.AppendLine("				) AS [MaxMovimiento] ON [MaxMovimiento].[Trab_ID] = [Movimientos].[Trab_ID] AND [MaxMovimiento].[Ptr] = [Movimientos].[Ptr]");
                    query.AppendLine("	) AS [Movimiento] ON [Empleado].[Trab_ID] = [Movimiento].[Trab_ID]");
                    query.AppendLine("	INNER JOIN [dbo].[TBDepto] ON [Movimiento].[Depto_ID] = [TBDepto].[Depto_ID]");
                    query.AppendLine("	LEFT JOIN [dbo].[TBCentroCosto] ON [TBDepto].[CentroCosto] = [TBCentroCosto].[CentroCosto_ID]");
                    query.AppendLine("WHERE");
                    query.AppendLine("	[NomCalculoHistorico].[Periodo] = @Mes");
                    query.AppendLine("	AND [NomCalculoHistorico].[Ano] = @Año");
                    query.AppendLine("	AND [NomCalculoHistorico].[Concepto_ID] = 32");
                    query.AppendLine("	AND ([NomCalculoHistorico].[TipoNomina_ID] = @Tipo OR @Tipo = 0)");

                    //query.AppendLine("  AND [Empleado].[Trab_ID] NOT IN ('80003749','80092625','80092755')");

                    query.AppendLine("GROUP BY");
                    query.AppendLine("	[TBDepto].[CentroCosto],");
                    query.AppendLine("	[TBDepto].[CuentaAcumuladora]");
                    query.AppendLine("ORDER BY");
                    query.AppendLine("	[TBDepto].[CentroCosto]");

                    try
                    {
                        SQLDataManager connector = new SQLDataManager();
                        result = connector.ExecuteQuery(query.ToString());
                    }
                    catch (Exception e)
                    {
                        result = null;
                        throw e;
                    }
                }
                else
                    throw new ArgumentException("El periodo no es válido", "Periodo");
            }
            else
                throw new ArgumentException("El año no es válido", "Año");

            return result;
        }

        public DataTable ReporteListadoDeValesDeDespensa(int Año, int Periodo, short Tipo, bool Abierta)
        {
            StringBuilder query = new StringBuilder();
            DataTable result;
            DateTime PeriodoInicio, PeriodoFin;

            if (Utilities.ValidarAño(Año))
            {
                if (Utilities.ValidarMes(Periodo))
                {
                    PeriodoInicio = new DateTime(Año, Periodo, 1, 0, 0, 0);
                    PeriodoFin = new DateTime(Año, Periodo, 1, 23, 59, 59).AddMonths(1).AddDays(-1);

                    query.AppendLine("DECLARE @Mes INT");
                    query.AppendLine("DECLARE @Año INT");
                    query.AppendLine("DECLARE @Tipo TINYINT");
                    query.AppendLine();
                    query.AppendFormat("SET @Año = {0}", Año);
                    query.AppendLine();
                    query.AppendFormat("SET @Mes = {0}", Periodo);
                    query.AppendLine();
                    query.AppendFormat("SET @Tipo = {0}", Tipo);
                    query.AppendLine();
                    query.AppendLine();
                    query.AppendLine("SELECT DISTINCT");
                    query.AppendLine("	[Empleado].[Trab_ID] AS Empleado,");
                    query.AppendLine("	[Empleado].[Paterno] + ' ' + [Empleado].[Materno] + ' ' + [Empleado].[Nombre] AS Nombre,");
                    query.AppendLine("	ISNULL([NomCalculoHistorico].[Importe], 0) AS Despensa");
                    query.AppendLine("FROM");
                    query.AppendLine("	[dbo].[Empleado]");
                    if (Abierta)
                        query.AppendLine("	LEFT JOIN [dbo].[NomCalculo] AS [NomCalculoHistorico] ON [Empleado].[Trab_ID] = [NomCalculoHistorico].[Trab_ID]");
                    else
                        query.AppendLine("	LEFT JOIN [dbo].[NomCalculoHistorico] ON [Empleado].[Trab_ID] = [NomCalculoHistorico].[Trab_ID]");
                    query.AppendLine("	INNER JOIN [dbo].[Conceptos] ON [NomCalculoHistorico].[Concepto_ID] = [Conceptos].[Concepto_ID]");
                    query.AppendLine("WHERE");
                    query.AppendLine("	[NomCalculoHistorico].[Periodo] = @Mes");
                    query.AppendLine("	AND [NomCalculoHistorico].[Ano] = @Año");
                    query.AppendLine("	AND [NomCalculoHistorico].[Concepto_ID] = 32");
                    query.AppendLine("	AND [NomCalculoHistorico].[TipoNomina_ID] = @Tipo");
                    //query.AppendLine("AND [Empleado].[Trab_ID] NOT IN ('80003749','80092625','80092755')");

                    try
                    {
                        SQLDataManager connector = new SQLDataManager();
                        result = connector.ExecuteQuery(query.ToString());
                    }
                    catch (Exception e)
                    {
                        result = null;
                        throw e;
                    }
                }
                else
                    throw new ArgumentException("El periodo no es válido", "Periodo");
            }
            else
                throw new ArgumentException("El año no es válido", "Año");

            return result;
        }

        public DataTable ReporteMaestroDeEmpleados(int Año, int Periodo, short Tipo, bool Abierta)
        {
            StringBuilder query = new StringBuilder();
            DataTable result;

            query.AppendLine("DECLARE @Año INT");
            query.AppendLine("DECLARE @Mes INT");
            query.AppendLine("DECLARE @Tipo TINYINT");
            query.AppendLine("DECLARE @TopeINFONAVIT MONEY");
            query.AppendLine();
            query.AppendFormat("SET @Año = {0}", Año);
            query.AppendLine();
            query.AppendFormat("SET @Mes = {0}", Periodo);
            query.AppendLine();
            query.AppendFormat("SET @Tipo = {0}", Tipo);
            query.AppendLine();
            query.AppendLine("SET @TopeINFONAVIT = (SELECT TOP 1 MinZona1 * TopeInfonavit FROM TBMinTop");
            query.AppendLine("WHERE DATEPART(YEAR,FECHA) = @Año ORDER BY Fecha DESC)");
            query.AppendLine();
            query.AppendLine("SELECT DISTINCT");
            query.AppendLine("	[Empleado].[Trab_ID] AS Empleado,");
            query.AppendLine("	[Empleado].[Paterno] + ' ' + [Empleado].[Materno] + ' ' + [Empleado].[Nombre] AS Nombre,");
            query.AppendLine("	CAST([Empleado].[FechaIngreso] AS DATETIME) FechaIngreso,");
            query.AppendLine("  [Movimiento].[Salario] Sd,");
            query.AppendLine("  CASE WHEN [Movimiento].[IntegradoIMSS] > @TopeINFONAVIT THEN @TopeINFONAVIT ELSE ISNULL([Movimiento].[IntegradoIMSS], 0.0) END AS Sdi,");
            query.AppendLine("  ISNULL([NomCalculoI].[Importe],0) AS SdiIndemnizacion,");
            query.AppendLine("	REPLACE([Empleado].[IMSS],'-','') AS Nss,");
            query.AppendLine("	REPLACE([Empleado].[RFC],'-','') AS Rfc,");
            query.AppendLine("	[TBDepto].[Depto_ID] AS ClaveDepartamento,");
            query.AppendLine("	[TBDepto].[Descripcion] AS NombreDepartamento,");
            query.AppendLine("	[TBPuesto].[Descripcion] AS Puesto,");
            query.AppendLine("	[TBTipoEmpleado].[Descripcion] AS Convenio,");
            query.AppendLine("	[TBDepto].[CentroCosto] AS CentroCosto,");
            query.AppendLine("	[TBDepto].[CuentaAcumuladora] AS CentroCostoDes");
            query.AppendLine("FROM");
            query.AppendLine("	[dbo].[Empleado]");
            query.AppendLine("	INNER JOIN (");
            query.AppendLine("		SELECT");
            query.AppendLine("			[Movimientos].[Trab_ID],");
            query.AppendLine("			[Movimientos].[FechaMov],");
            query.AppendLine("			[Movimientos].[Puesto_ID],");
            query.AppendLine("			[Movimientos].[TipoEmpleado_ID],");
            query.AppendLine("			[Movimientos].[Centro_ID],");
            query.AppendLine("			[Movimientos].[Depto_ID],");
            query.AppendLine("			[Movimientos].[Salario],");
            query.AppendLine("			[Movimientos].[IntegradoIMSS]");
            query.AppendLine("		FROM");
            query.AppendLine("			[dbo].[Movimiento] AS [Movimientos]");
            query.AppendLine("			INNER JOIN (");
            query.AppendLine("				SELECT");
            query.AppendLine("					[Trab_ID],");
            query.AppendLine("					MAX([Ptr]) AS [Ptr]");
            query.AppendLine("				FROM");
            query.AppendLine("					[dbo].[Movimiento]");
            query.AppendLine("				GROUP BY");
            query.AppendLine("					[Trab_ID]");
            query.AppendLine("				) AS [MaxMovimiento] ON [MaxMovimiento].[Trab_ID] = [Movimientos].[Trab_ID] AND [MaxMovimiento].[Ptr] = [Movimientos].[Ptr]");
            query.AppendLine("	) AS [Movimiento] ON [Empleado].[Trab_ID] = [Movimiento].[Trab_ID]");
            if (Abierta)
                query.AppendLine("	INNER JOIN [dbo].[NomCalculo] AS [NomCalculoHistorico] ON [Empleado].[Trab_ID] = [NomCalculoHistorico].[Trab_ID]");
            else
                query.AppendLine("	INNER JOIN [dbo].[NomCalculoHistorico] ON [Empleado].[Trab_ID] = [NomCalculoHistorico].[Trab_ID]");
            query.AppendLine("	INNER JOIN [dbo].[TBPuesto] ON [Movimiento].[Puesto_ID] = [TBPuesto].[Puesto_ID]");
            query.AppendLine("	INNER JOIN [dbo].[TBTipoEmpleado] ON [Movimiento].[TipoEmpleado_ID] = [TBTipoEmpleado].[TipoEmpleado_ID]");
            query.AppendLine("	INNER JOIN [dbo].[TBCentros] ON [Movimiento].[Centro_ID] = [TBCentros].[Centro_ID]");
            query.AppendLine("	INNER JOIN [dbo].[TBDepto] ON [Movimiento].[Depto_ID] = [TBDepto].[Depto_ID]");
            query.AppendLine("	INNER JOIN [dbo].[Conceptos] ON [NomCalculoHistorico].[Concepto_ID] = [Conceptos].[Concepto_ID]");
            query.AppendLine("	INNER JOIN [dbo].[EquivalenciaConceptos] ON [Conceptos].[Concepto_ID] = [EquivalenciaConceptos].[Concepto_ID]");
            if (Abierta)
                query.AppendLine("	INNER JOIN [dbo].[NomCalculo] AS [NomCalculoI] ON [NomCalculoI].[Concepto_ID] = 89 AND [Empleado].[Trab_ID] = [NomCalculoI].[Trab_ID]");
            else
                query.AppendLine("	INNER JOIN [dbo].[NomCalculoHistorico] AS [NomCalculoI] ON [NomCalculoI].[Concepto_ID] = 89 AND [Empleado].[Trab_ID] = [NomCalculoI].[Trab_ID]");
            query.Append("AND [NomCalculoI].[Ano] = @Año AND [NomCalculoI].[Periodo] = @Mes");
            query.AppendLine("	LEFT JOIN [dbo].[IMSSLiquidacion97] ON [Movimiento].[Trab_ID] = [IMSSLiquidacion97].[Trab_ID] AND YEAR([Movimiento].[FechaMov]) = [IMSSLiquidacion97].[Ano] AND MONTH([Movimiento].[FechaMov]) = [IMSSLiquidacion97].[Periodo]");
            query.AppendLine("WHERE");
            query.AppendLine("	[NomCalculoHistorico].[Ano] = @Año");
            query.AppendLine("	AND [NomCalculoHistorico].[Periodo] = @Mes");
            query.AppendLine("	AND [NomCalculoHistorico].[TipoNomina_ID] = @Tipo");
            query.AppendLine("	AND (");
            query.AppendLine("		[NomCalculoHistorico].[Concepto_ID] >= 301 AND [NomCalculoHistorico].[Concepto_ID] <= 599");
            query.AppendLine("		OR [NomCalculoHistorico].[Concepto_ID] >= 601 AND [NomCalculoHistorico].[Concepto_ID] <= 799");
            query.AppendLine("	)");
            query.Append("ORDER BY CentroCosto,ClaveDepartamento,Empleado");

            try
            {
                SQLDataManager connector = new SQLDataManager();
                result = connector.ExecuteQuery(query.ToString());
            }
            catch (Exception e)
            {
                result = null;
                throw e;
            }

            return result;
        }

        public DataTable ReporteReciboNomina(int Año, int Periodo, short Tipo, bool Abierta, List<string> Empleados)
        {
            StringBuilder query = new StringBuilder();
            DataTable result;

            if (Utilities.ValidarAño(Año))
            {
                if (Utilities.ValidarMes(Periodo))
                {
                    query.AppendLine("DECLARE @Año INT");
                    query.AppendLine("DECLARE @Mes INT");
                    query.AppendLine("DECLARE @Tipo TINYINT");
                    query.AppendLine();
                    query.AppendFormat("SET @Año = {0}", Año);
                    query.AppendLine();
                    query.AppendFormat("SET @Mes = {0}", Periodo);
                    query.AppendLine();
                    query.AppendFormat("SET @Tipo = {0}", Tipo);
                    query.AppendLine();
                    query.AppendLine();
                    query.AppendLine("SELECT DISTINCT");
                    query.AppendLine("	[Empleado].[Trab_ID] AS Empleado,");
                    query.AppendLine("	[Empleado].[Paterno] + ' ' + [Empleado].[Materno] + ' ' + [Empleado].[Nombre] AS Nombre,");
                    query.AppendLine("	REPLACE([Empleado].[RFC], '-', '') AS Rfc,");
                    query.AppendLine("	[Empleado].[Curp] AS Curp,");
                    query.AppendLine("	CAST([Empleado].[FechaIngreso] AS DATETIME) FechaIngreso,");
                    query.AppendLine("	(SELECT CAST(FechaMov AS DATETIME) FROM Movimiento WHERE Trab_Id = [Empleado].[Trab_ID] AND Mov_ID = 'B') FechaBaja,");
                    query.AppendLine("	REPLACE([Empleado].[IMSS], '-', '') AS Nss,");
                    query.AppendLine("  [Movimiento].[Salario] AS Sd,");
                    query.AppendLine("  CASE WHEN [Movimiento].[IntegradoIMSS] > 1558.25 THEN 1558.25 ELSE ISNULL([Movimiento].[IntegradoIMSS], 0.0) END AS Sdi,");
                    query.AppendLine("	[TBTipoEmpleado].[Descripcion] AS Convenio,");
                    query.AppendLine("	[TBPuesto].[Descripcion] AS Puesto,");
                    query.AppendLine("	[TBCentros].[Descripcion] AS CentroTrabajo,");
                    query.AppendLine("	[TBDepto].[Descripcion] AS Departamento,");
                    query.AppendLine("	[TBDepto].[CentroCosto] AS CentroCosto,");
                    query.AppendLine("	ISNULL([EquivalenciaConceptos].[Concepto_Equivalencia], [NomCalculoHistorico].[Concepto_ID]) AS Concepto,");
                    if (Tipo == 2)
                        query.AppendLine("  CASE WHEN [NomCalculoHistorico].[Concepto_ID] = 313 THEN 'F.A. FINIQ EMPRESA' ELSE [Conceptos].[Descripcion] END AS Descripcion,");
                    else
                        query.AppendLine("	[Conceptos].[Descripcion] AS Descripcion,");
                    query.AppendLine("	[NomCalculoHistorico].[Dato] AS Unidad,");
                    query.AppendLine("	CAST(CASE WHEN [NomCalculoHistorico].[Concepto_ID] = 32 THEN [NomCalculoHistorico].[Importe]");
                    query.AppendLine("	          WHEN [NomCalculoHistorico].[Concepto_ID] >= 301 AND [NomCalculoHistorico].[Concepto_ID] <= 599 THEN [NomCalculoHistorico].[Importe] ELSE 0 END AS MONEY) AS Percepcion,");
                    query.AppendLine("	CAST(CASE WHEN [NomCalculoHistorico].[Concepto_ID] >= 601 AND [NomCalculoHistorico].[Concepto_ID] <= 799 THEN [NomCalculoHistorico].[Importe] ELSE 0 END AS MONEY) AS Deduccion,");
                    query.AppendLine("	[NomCalculoVales].[Despensa] AS ValesDespensa,");
                    query.AppendLine("	0 AS ValesGasolina,");
                    query.AppendLine("	CASE WHEN [SaldoFondoRetiro].[FondoFiniquito] = 0 THEN [SaldoFondoRetiro].[FondoAhorro] ELSE [SaldoFondoRetiro].[FondoFiniquito] END FondoAhorro,");
                    query.AppendLine("  CASE WHEN [NomCalculoHistorico].[Concepto_ID] >= 601 AND [NomCalculoHistorico].[Concepto_ID] <= 799 THEN 1 ELSE 0 END Tipo,");
                    if (Abierta)
                    {
                        query.AppendLine("  (SELECT CAST(ncd.Importe AS INT) Importe FROM NomCalculo ncd WHERE ncd.Trab_ID = [Empleado].[Trab_ID] AND ncd.Concepto_ID = 12 AND ncd.Ano = @Año");
                        query.AppendLine("      AND ncd.Periodo = @Mes AND ncd.TipoNomina_ID = @Tipo) Dias,");
                        query.AppendLine("  (SELECT ncd.Importe FROM NomCalculo ncd WHERE ncd.Trab_ID = [Empleado].[Trab_ID] AND ncd.Concepto_ID = 313 AND ncd.Ano = @Año AND ncd.TipoNomina_ID = 6) Descuento,");
                    }
                    else
                    {
                        query.AppendLine("  (SELECT CAST(ncd.Importe AS INT) Importe FROM NomCalculoHistorico ncd WHERE ncd.Trab_ID = [Empleado].[Trab_ID] AND ncd.Concepto_ID = 12 AND ncd.Ano = @Año");
                        query.AppendLine("      AND ncd.Periodo = @Mes AND ncd.TipoNomina_ID = @Tipo) Dias,");
                        query.AppendLine("  (SELECT ncd.Importe FROM NomCalculoHistorico ncd WHERE ncd.Trab_ID = [Empleado].[Trab_ID] AND ncd.Concepto_ID = 313 AND ncd.Ano = @Año AND ncd.TipoNomina_ID = 6) Descuento,");
                    }
                    query.AppendLine("FROM [dbo].[Empleado]");
                    if (Abierta)
                        query.AppendLine("	LEFT JOIN [dbo].[NomCalculo] AS [NomCalculoHistorico] ON [Empleado].[Trab_ID] = [NomCalculoHistorico].[Trab_ID]");
                    else
                        query.AppendLine("	LEFT JOIN [dbo].[NomCalculoHistorico] ON [Empleado].[Trab_ID] = [NomCalculoHistorico].[Trab_ID]");
                    query.AppendLine("	INNER JOIN (");
                    query.AppendLine("		SELECT");
                    query.AppendLine("			[Movimientos].[Trab_ID],");
                    query.AppendLine("			[Movimientos].[FechaMov],");
                    query.AppendLine("			[Movimientos].[Puesto_ID],");
                    query.AppendLine("			[Movimientos].[TipoEmpleado_ID],");
                    query.AppendLine("			[Movimientos].[Centro_ID],");
                    query.AppendLine("			[Movimientos].[Depto_ID],");
                    query.AppendLine("			[Movimientos].[Salario],");
                    query.AppendLine("			[Movimientos].[IntegradoIMSS]");

                    query.AppendLine("		FROM");
                    query.AppendLine("			[dbo].[Movimiento] AS [Movimientos]");
                    query.AppendLine("			INNER JOIN (");
                    query.AppendLine("				SELECT");
                    query.AppendLine("					[Trab_ID],");
                    query.AppendLine("					MAX([Ptr]) AS [Ptr]");
                    query.AppendLine("				FROM");
                    query.AppendLine("					[dbo].[Movimiento]");
                    query.AppendLine("				GROUP BY");
                    query.AppendLine("					[Trab_ID]");
                    query.AppendLine("				) AS [MaxMovimiento] ON [MaxMovimiento].[Trab_ID] = [Movimientos].[Trab_ID] AND [MaxMovimiento].[Ptr] = [Movimientos].[Ptr]");
                    query.AppendLine("	) AS [Movimiento] ON [Empleado].[Trab_ID] = [Movimiento].[Trab_ID]");
                    query.AppendLine("	INNER JOIN [dbo].[TBPuesto] ON [Movimiento].[Puesto_ID] = [TBPuesto].[Puesto_ID]");
                    query.AppendLine("	INNER JOIN [dbo].[TBTipoEmpleado] ON [Movimiento].[TipoEmpleado_ID] = [TBTipoEmpleado].[TipoEmpleado_ID]");
                    query.AppendLine("	INNER JOIN [dbo].[TBCentros] ON [Movimiento].[Centro_ID] = [TBCentros].[Centro_ID]");
                    query.AppendLine("	INNER JOIN [dbo].[TBDepto] ON [Movimiento].[Depto_ID] = [TBDepto].[Depto_ID]");
                    query.AppendLine("	LEFT JOIN [dbo].[IMSSLiquidacion97] ON [Movimiento].[Trab_ID] = [IMSSLiquidacion97].[Trab_ID] AND YEAR([Movimiento].[FechaMov]) = [IMSSLiquidacion97].[Ano] AND MONTH([Movimiento].[FechaMov]) = [IMSSLiquidacion97].[Periodo]");
                    query.AppendLine("	INNER JOIN [dbo].[Conceptos] ON [NomCalculoHistorico].[Concepto_ID] = [Conceptos].[Concepto_ID]");
                    query.AppendLine("	INNER JOIN (SELECT Concepto_ID, Concepto_Equivalencia FROM EquivalenciaConceptos");
                    if (Tipo == 2)
                        query.AppendLine(" WHERE Concepto_ID != 313 UNION SELECT 336, '1003' UNION SELECT 337,'1026' UNION SELECT 313,'9921' UNION SELECT 623,'5910'");
                    query.AppendLine(") [EquivalenciaConceptos] ON [NomCalculoHistorico].Concepto_ID = [EquivalenciaConceptos].Concepto_ID");
                    query.AppendLine("	LEFT JOIN (");
                    query.AppendLine("		SELECT");
                    query.AppendLine("			[NomCalculoVales].[Trab_ID],");
                    query.AppendLine("			SUM([NomCalculoVales].[Importe]) AS Despensa");
                    if (Abierta)
                        query.AppendLine("		FROM [dbo].[NomCalculo] AS [NomCalculoVales]");
                    else
                        query.AppendLine("		FROM [dbo].[NomCalculoHistorico] AS [NomCalculoVales]");
                    query.AppendLine("		WHERE");
                    query.AppendLine("			[NomCalculoVales].[Concepto_ID] = 32");
                    query.AppendLine("  			AND [NomCalculoVales].[Ano] = @Año");
                    query.AppendLine("			AND [NomCalculoVales].[Periodo] = @Mes");
                    query.AppendLine("			AND [NomCalculoVales].[TipoNomina_ID] = @Tipo");
                    query.AppendLine("		GROUP BY");
                    query.AppendLine("			[NomCalculoVales].[Trab_ID]");
                    query.AppendLine("	) AS [NomCalculoVales] ON [Empleado].[Trab_ID] = [NomCalculoVales].[Trab_ID]");
                    query.AppendLine("LEFT JOIN (");
                    query.AppendLine("      SELECT Trab_ID, SUM(FondoAhorro) FondoAhorro, SUM(FondoFiniquito)           FondoFiniquito FROM (");
                    query.AppendLine("SELECT Trab_ID, Importe FondoAhorro, CAST(0 AS MONEY) FondoFiniquito ");
                    query.AppendLine("FROM NomCalculo WHERE Concepto_ID = 94 AND  Ano = @Año AND Periodo = @Mes");
                    query.AppendLine("UNION ALL ");
                    query.AppendLine("SELECT Trab_ID, CAST(0 AS MONEY) FondoAhorro, Importe FondoFiniquito ");
                    query.AppendLine("FROM NomCalculo WHERE Concepto_ID = 95 AND  Ano = @Año AND Periodo = @Mes) [SaldoFondoAhorro] ");
                    query.AppendLine("GROUP BY Trab_ID) [SaldoFondoRetiro] ");
                    query.AppendLine("ON  [Empleado].[Trab_ID] = [SaldoFondoRetiro].[Trab_ID] ");
                    query.AppendLine("WHERE");
                    query.AppendLine("	[NomCalculoHistorico].[Ano] = @Año");
                    query.AppendLine("	AND [NomCalculoHistorico].[Periodo] = @Mes");
                    query.AppendLine("	AND [NomCalculoHistorico].[TipoNomina_ID] = @Tipo");
                    query.AppendLine("	AND (");
                    query.AppendLine("		[NomCalculoHistorico].[Concepto_ID] >= 301 AND [NomCalculoHistorico].[Concepto_ID] <= 599");
                    query.AppendLine("		OR [NomCalculoHistorico].[Concepto_ID] >= 601 AND [NomCalculoHistorico].[Concepto_ID] <= 799");
                    if (Tipo == 2)
                        query.AppendLine("  OR [NomCalculoHistorico].[Concepto_ID] = 32");
                    query.AppendLine("	)");
                    if (Empleados.Count > 0)
                    {
                        query.AppendLine("AND [Empleado].[Trab_ID] IN (");
                        for (int i = 0; i < Empleados.Count; i++)
                        {
                            if (i == Empleados.Count - 1)
                                query.AppendLine(string.Format("'{0}'", Empleados[i]));
                            else
                                query.AppendLine(string.Format("'{0}',", Empleados[i]));
                        }
                        query.AppendLine(")");
                    }

                    query.AppendLine("ORDER BY Nombre, Concepto");

                    try
                    {
                        SQLDataManager connector = new SQLDataManager();
                        result = connector.ExecuteQuery(query.ToString());
                    }
                    catch (Exception e)
                    {
                        result = null;
                        throw e;
                    }
                }
                else
                    throw new ArgumentException("El periodo no es válido", "Periodo");
            }
            else
                throw new ArgumentException("El año no es válido", "Año");

            return result;
        }

        public DataTable ReporteReciboNomina(int Año, int Periodo, short Tipo, bool Abierta, List<string> Empleados, bool envioEmail)
        {
            StringBuilder query = new StringBuilder();
            DataTable result;

            if (Utilities.ValidarAño(Año))
            {
                if (Utilities.ValidarMes(Periodo))
                {
                    SQLDataManager connector = new SQLDataManager();
                    List<SqlParameter> parameters = new List<SqlParameter>();

                    try
                    {
                        string empleados = string.Empty;
                        for (int i = 0; i < Empleados.Count; i++)
                        {
                            if (i == Empleados.Count - 1)
                                empleados += string.Format("{0}", Empleados[i]);
                            else
                                empleados += string.Format("{0},", Empleados[i]);
                        }

                        parameters.Add(new SqlParameter("@Año", SqlDbType.Int, 4, ParameterDirection.Input, false, 10, 0, "", DataRowVersion.Current, Año));
                        parameters.Add(new SqlParameter("@Mes", SqlDbType.Int, 4, ParameterDirection.Input, false, 10, 0, "", DataRowVersion.Current, Periodo));
                        parameters.Add(new SqlParameter("@Tipo", SqlDbType.TinyInt, 2, ParameterDirection.Input, false, 10, 0, "", DataRowVersion.Current, Tipo));
                        parameters.Add(new SqlParameter("@Ids", SqlDbType.NVarChar, 4000, ParameterDirection.Input, false, 10, 0, "", DataRowVersion.Current, empleados));
                        result = connector.ExecuteProcedure("usp_ReciboNomina", parameters);
                    }
                    catch (Exception e)
                    {
                        result = null;
                        throw e;
                    }
                }
                else
                    throw new ArgumentException("El periodo no es válido", "Periodo");
            }
            else
                throw new ArgumentException("El año no es válido", "Año");

            return result;
        }

        public DataTable ReporteResumenConceptosPorCentroCosto(int Año, int Periodo, short Tipo, bool Abierta)
        {
            StringBuilder query = new StringBuilder();
            DataTable result;

            if (Utilities.ValidarAño(Año))
            {
                if (Utilities.ValidarMes(Periodo))
                {
                    query.AppendLine("DECLARE @Año INT");
                    query.AppendLine("DECLARE @Mes INT");
                    query.AppendLine("DECLARE @Tipo TINYINT");
                    query.AppendLine();
                    query.AppendFormat("SET @Año = {0}", Año);
                    query.AppendLine();
                    query.AppendFormat("SET @Mes = {0}", Periodo);
                    query.AppendLine();
                    query.AppendFormat("SET @Tipo = {0}", Tipo);
                    query.AppendLine();
                    query.AppendLine();
                    query.AppendLine("SELECT");
                    query.AppendLine("  [NomCalculoHistorico].[Trab_ID],");
                    query.AppendLine("	[TBDepto].[CentroCosto] AS CentroCosto,");
                    query.AppendLine("	ISNULL([TBDepto].[CuentaAcumuladora], '') AS NombreCentroCosto,");
                    query.AppendLine("	ISNULL([EquivalenciaConceptos].[Concepto_Equivalencia], [NomCalculoHistorico].[Concepto_ID]) AS Concepto,");
                    query.AppendLine("	[Conceptos].[Descripcion] AS Descripcion,");
                    query.AppendLine("	CAST(CASE WHEN [NomCalculoHistorico].[Concepto_ID] >= 301 AND [NomCalculoHistorico].[Concepto_ID] <= 599 THEN [NomCalculoHistorico].[Importe] ELSE 0 END AS MONEY) AS Percepcion,");
                    query.AppendLine("	CAST(CASE WHEN [NomCalculoHistorico].[Concepto_ID] >= 601 AND [NomCalculoHistorico].[Concepto_ID] <= 799 THEN [NomCalculoHistorico].[Importe] ELSE 0 END AS MONEY) AS Deduccion");
                    query.AppendLine("FROM");
                    if (Abierta)
                        query.AppendLine("	[dbo].[NomCalculo] AS [NomCalculoHistorico]");
                    else
                        query.AppendLine("	[dbo].[NomCalculoHistorico] AS [NomCalculoHistorico]");
                    query.AppendLine("	INNER JOIN [dbo].[Conceptos] ON [NomCalculoHistorico].[Concepto_ID] = [Conceptos].[Concepto_ID]");
                    query.AppendLine("	LEFT JOIN [dbo].[EquivalenciaConceptos] ON [Conceptos].[Concepto_ID] = [EquivalenciaConceptos].[Concepto_ID]");
                    query.AppendLine("	INNER JOIN (");
                    query.AppendLine("		SELECT");
                    query.AppendLine("			[Movimientos].[Trab_ID],");
                    query.AppendLine("			[Movimientos].[FechaMov],");
                    query.AppendLine("			[Movimientos].[Puesto_ID],");
                    query.AppendLine("			[Movimientos].[TipoEmpleado_ID],");
                    query.AppendLine("			[Movimientos].[Centro_ID],");
                    query.AppendLine("			[Movimientos].[Depto_ID],");
                    query.AppendLine("			[Movimientos].[Salario]");
                    query.AppendLine("		FROM");
                    query.AppendLine("			[dbo].[Movimiento] AS [Movimientos]");
                    query.AppendLine("			INNER JOIN (");
                    query.AppendLine("				SELECT");
                    query.AppendLine("					[Trab_ID],");
                    query.AppendLine("					MAX([Ptr]) AS [Ptr]");
                    query.AppendLine("				FROM");
                    query.AppendLine("					[dbo].[Movimiento]");
                    query.AppendLine("				GROUP BY");
                    query.AppendLine("					[Trab_ID]");
                    query.AppendLine("				) AS [MaxMovimiento] ON [MaxMovimiento].[Trab_ID] = [Movimientos].[Trab_ID] AND [MaxMovimiento].[Ptr] = [Movimientos].[Ptr]");
                    query.AppendLine("	) AS [Movimiento] ON [NomCalculoHistorico].[Trab_ID] = [Movimiento].[Trab_ID]");
                    query.AppendLine("	LEFT JOIN [dbo].[TBDepto] ON [Movimiento].[Depto_ID] = [TBDepto].[Depto_ID]");
                    query.AppendLine("	LEFT JOIN [dbo].[TBCentroCosto] ON [TBDepto].[CentroCosto] = [TBCentroCosto].[CentroCosto_ID]");
                    query.AppendLine("WHERE");
                    query.AppendLine("	[NomCalculoHistorico].[Ano] = @Año");
                    query.AppendLine("	AND [NomCalculoHistorico].[Periodo] = @Mes");
                    query.AppendLine("	AND ([NomCalculoHistorico].[TipoNomina_ID] = @Tipo OR @Tipo = 0)");
                    query.AppendLine("	AND (");
                    query.AppendLine("		[NomCalculoHistorico].[Concepto_ID] >= 301 AND [NomCalculoHistorico].[Concepto_ID] <= 599");
                    query.AppendLine("		OR [NomCalculoHistorico].[Concepto_ID] >= 601 AND [NomCalculoHistorico].[Concepto_ID] <= 799");
                    query.AppendLine("	)");
                    query.AppendLine("ORDER BY");
                    query.AppendLine("	[TBDepto].[CentroCosto], [EquivalenciaConceptos].[Concepto_Equivalencia]");

                    try
                    {
                        SQLDataManager connector = new SQLDataManager();
                        result = connector.ExecuteQuery(query.ToString());
                    }
                    catch (Exception e)
                    {
                        result = null;
                        throw e;
                    }
                }
                else
                    throw new ArgumentException("El periodo no es válido", "Periodo");
            }
            else
                throw new ArgumentException("El año no es válido", "Año");

            return result;
        }

        public DataTable ReporteResumenConceptosPorEmpresa(int Año, int Periodo, short Tipo, bool Abierta)
        {
            StringBuilder query = new StringBuilder();
            DataTable result;

            if (Utilities.ValidarAño(Año))
            {
                if (Utilities.ValidarMes(Periodo))
                {
                    query.AppendLine("DECLARE @Año INT");
                    query.AppendLine("DECLARE @Mes INT");
                    query.AppendLine("DECLARE @Tipo TINYINT");
                    query.AppendLine();
                    query.AppendFormat("SET @Año = {0}", Año);
                    query.AppendLine();
                    query.AppendFormat("SET @Mes = {0}", Periodo);
                    query.AppendLine();
                    query.AppendFormat("SET @Tipo = {0}", Tipo);
                    query.AppendLine();
                    query.AppendLine();
                    query.AppendLine("SELECT");
                    query.AppendLine("  [NomCalculoHistorico].[Trab_ID],");
                    query.AppendLine("	ISNULL([EquivalenciaConceptos].[Concepto_Equivalencia], [NomCalculoHistorico].[Concepto_ID]) AS Concepto,");
                    query.AppendLine("	[Conceptos].[Descripcion] AS Descripcion,");
                    query.AppendLine("	CAST(CASE WHEN [NomCalculoHistorico].[Concepto_ID] >= 301 AND [NomCalculoHistorico].[Concepto_ID] <= 599 THEN [NomCalculoHistorico].[Importe] ELSE 0 END AS MONEY) AS Percepcion,");
                    query.AppendLine("	CAST(CASE WHEN [NomCalculoHistorico].[Concepto_ID] >= 601 AND [NomCalculoHistorico].[Concepto_ID] <= 799 THEN [NomCalculoHistorico].[Importe] ELSE 0 END AS MONEY) AS Deduccion");
                    query.AppendLine("FROM");
                    if (Abierta)
                        query.AppendLine("	[dbo].[NomCalculo] AS [NomCalculoHistorico]");
                    else
                        query.AppendLine("	[dbo].[NomCalculoHistorico] AS [NomCalculoHistorico]");
                    query.AppendLine("	INNER JOIN [dbo].[Conceptos] ON [NomCalculoHistorico].[Concepto_ID] = [Conceptos].[Concepto_ID]");
                    if (Tipo == 3)
                    {
                        query.AppendLine("INNER JOIN ( SELECT Concepto_ID, Concepto_Equivalencia");
                        query.AppendLine("FROM EquivalenciaConceptos ");
                        //query.AppendLine("FROM EquivalenciaConceptos WHERE Concepto_ID NOT IN (318,323)");
                        //query.AppendLine("UNION SELECT 318,'3813' UNION SELECT 318, '6056' UNION SELECT 603, '9316') [EquivalenciaConceptos] ON [Conceptos].[Concepto_ID] = [EquivalenciaConceptos].[Concepto_ID]");
                        query.AppendLine("UNION SELECT 603, '9316') [EquivalenciaConceptos] ON [Conceptos].[Concepto_ID] = [EquivalenciaConceptos].[Concepto_ID]");
                    }
                    else
                        query.AppendLine("	LEFT JOIN [dbo].[EquivalenciaConceptos] ON [Conceptos].[Concepto_ID] = [EquivalenciaConceptos].[Concepto_ID]");
                    query.AppendLine("WHERE");
                    query.AppendLine("	[NomCalculoHistorico].[Ano] = @Año");
                    query.AppendLine("	AND [NomCalculoHistorico].[Periodo] = @Mes");
                    query.AppendLine("	AND ([NomCalculoHistorico].[TipoNomina_ID] = @Tipo OR @Tipo =0)");
                    query.AppendLine("	AND (");
                    query.AppendLine("		[NomCalculoHistorico].[Concepto_ID] >= 301 AND [NomCalculoHistorico].[Concepto_ID] <= 599");
                    query.AppendLine("		OR [NomCalculoHistorico].[Concepto_ID] >= 601 AND [NomCalculoHistorico].[Concepto_ID] <= 799");
                    query.AppendLine("	)");
                    query.AppendLine("ORDER BY");
                    query.AppendLine("	[NomCalculoHistorico].[Concepto_ID]");

                    try
                    {
                        SQLDataManager connector = new SQLDataManager();
                        result = connector.ExecuteQuery(query.ToString());
                    }
                    catch (Exception e)
                    {
                        result = null;
                        throw e;
                    }
                }
                else
                    throw new ArgumentException("El periodo no es válido", "Periodo");
            }
            else
                throw new ArgumentException("El año no es válido", "Año");

            return result;
        }

        public DataTable ReporteImpuestosNominas(int Año, int Periodo, short Tipo, bool Abierta)
        {
            StringBuilder query = new StringBuilder();
            DataTable result;
            DateTime PeriodoInicio, PeriodoFin;

            if (Utilities.ValidarAño(Año))
            {
                if (Utilities.ValidarMes(Periodo))
                {
                    PeriodoInicio = new DateTime(Año, Periodo, 1, 0, 0, 0);
                    PeriodoFin = new DateTime(Año, Periodo, 1, 23, 59, 59).AddMonths(1).AddDays(-1);

                    query.AppendLine("DECLARE @Año INT ");
                    query.AppendLine("DECLARE @Mes INT ");
                    query.AppendLine("DECLARE @Tipo TINYINT ");
                    query.AppendFormat("SET @Año = {0} ", Año);
                    query.AppendFormat("SET @Mes = {0} ", Periodo);
                    query.AppendFormat("SET @Tipo = {0} ", Tipo);
                    query.AppendLine();
                    query.AppendLine("SELECT ");
                    query.AppendLine("  ct.Centro_ID, ");
                    query.AppendLine("  ct.Descripcion Centro, ");
                    query.AppendLine("  d.CentroCosto CentroCosto_ID, ");
                    query.AppendLine("  d.CuentaAcumuladora CentroCosto, ");
                    query.AppendLine("  e.Trab_ID, ");
                    query.AppendLine("  ec.Concepto_Equivalencia Concepto_ID, ");
                    query.AppendLine("  CASE WHEN  ec.Concepto_Equivalencia = '3813'  THEN 'GRAVADO AGUINALDO'");
                    query.AppendLine("       WHEN  ec.Concepto_Equivalencia = '6056'  THEN 'EXENTO AGUINALDO'");
                    query.AppendLine("  ELSE c.Descripcion END Concepto,");
                    query.AppendLine("  CASE WHEN  ec.Concepto_Equivalencia = '3813'  THEN nc.Gravado");
                    query.AppendLine("       WHEN  ec.Concepto_Equivalencia = '6056'  THEN nc.Excento");
                    query.AppendLine("       WHEN  ec.Concepto_ID IN (609,610,611,612)  THEN nc.Importe * -1");
                    query.AppendLine("  ELSE nc.Importe END Importe");
                    query.AppendLine("FROM Empleado e ");
                    query.AppendLine("INNER JOIN (  SELECT  ");
                    query.AppendLine("                  m.Trab_ID, m.FechaMov, m.Puesto_ID, m.TipoEmpleado_ID, ");
                    query.AppendLine("                  m.Centro_ID, m.Depto_ID, m.Salario,IntegradoIMSS ");
                    query.AppendLine("              FROM Movimiento m ");
                    query.AppendLine("              INNER JOIN (SELECT Trab_ID, MAX(Ptr) Ptr FROM Movimiento GROUP BY Trab_ID) mm ");
                    query.AppendLine("                  ON m.Trab_ID = mm.Trab_ID AND m.Ptr = mm.Ptr) m ");
                    query.AppendLine("  ON e.Trab_ID = m.Trab_ID ");
                    query.AppendLine("INNER JOIN TBDepto d ON m.Depto_ID = d.Depto_ID ");
                    query.AppendLine("INNER JOIN vw_NomCalculo nc ON e.Trab_ID = nc.Trab_ID ");
                    query.AppendLine("INNER JOIN TBCentros ct ON m.Centro_ID = ct.Centro_ID ");
                    query.AppendLine("INNER JOIN Conceptos c ON nc.Concepto_ID = c.Concepto_ID ");
                    query.AppendLine("INNER JOIN ( SELECT Concepto_ID, Concepto_Equivalencia");
                    query.AppendLine("FROM EquivalenciaConceptos WHERE Concepto_ID != 318");
                    query.AppendLine("UNION SELECT 318,'3813' UNION SELECT 318, '6056')ec");
                    query.AppendLine("  ON c.Concepto_ID = ec.Concepto_ID ");
                    query.AppendLine("WHERE nc.Concepto_ID IN ( 38,43,44,301,302,303,304,305,306,307,312,314,316,317,318,318,");
                    query.AppendLine("319,320,321,322,327,328,329,332,333,335,609,610,611,612)");
                    query.AppendLine("AND nc.Ano = @Año ");
                    query.AppendLine("AND nc.Periodo = @Mes ");
                    query.AppendLine("AND (nc.TipoNomina_ID = @Tipo OR @Tipo = 0)");
                    query.AppendLine("ORDER BY d.CentroCosto, ec.Concepto_Equivalencia  ");

                    try
                    {
                        SQLDataManager connector = new SQLDataManager();
                        result = connector.ExecuteQuery(query.ToString());
                    }
                    catch (Exception e)
                    {
                        result = null;
                        throw e;
                    }
                }
                else
                    throw new ArgumentException("El periodo no es válido", "Periodo");
            }
            else
                throw new ArgumentException("El año no es válido", "Año");

            return result;
        }

        public DataTable ReporteAcumuladoConceptos(int Año, int Periodo, int tipo)
        {
            StringBuilder query = new StringBuilder();
            DataTable result;

            if (Utilities.ValidarAño(Año))
            {
                if (Utilities.ValidarMes(Periodo))
                {
                    try
                    {
                        SQLDataManager connector = new SQLDataManager();
                        List<SqlParameter> parameters = new List<SqlParameter>();
                        parameters.Add(new SqlParameter("@Año", SqlDbType.Int, 4, ParameterDirection.Input, false, 10, 0, "", DataRowVersion.Current, Año));
                        parameters.Add(new SqlParameter("@Mes", SqlDbType.Int, 4, ParameterDirection.Input, false, 10, 0, "", DataRowVersion.Current, Periodo));
                        if (tipo == 1)
                            result = connector.ExecuteProcedure("usp_NominaObtenerAcumuladoConcpetosXEmpleado", parameters);
                        else
                            result = connector.ExecuteProcedure("usp_NominaObtenerAcumuladoConcpetosXEmpleadoAbierto", parameters);
                    }
                    catch (Exception e)
                    {
                        result = null;
                        throw e;
                    }
                }
                else
                    throw new ArgumentException("El periodo no es válido", "Periodo");
            }
            else
                throw new ArgumentException("El año no es válido", "Año");

            return result;
        }

        public DataTable ReporteProvisionesPorCentroCosto(int Año, int Periodo, short Tipo, bool Abierta)
        {
            StringBuilder query = new StringBuilder();
            DataTable result;

            if (Utilities.ValidarAño(Año))
            {
                if (Utilities.ValidarMes(Periodo))
                {
                    query.AppendLine("DECLARE @Año INT");
                    query.AppendLine("DECLARE @Mes INT");
                    query.AppendLine("DECLARE @Tipo TINYINT");
                    query.AppendLine();
                    query.AppendFormat("SET @Año = {0}", Año);
                    query.AppendLine();
                    query.AppendFormat("SET @Mes = {0}", Periodo);
                    query.AppendLine();
                    query.AppendFormat("SET @Tipo = {0}", Tipo);
                    query.AppendLine();
                    query.AppendLine("SELECT");
                    query.AppendLine("  nc.Trab_ID,");
                    query.AppendLine("  d.CentroCosto,");
                    query.AppendLine("  ISNULL(d.CuentaAcumuladora, '') NombreCentroCosto,");
                    query.AppendLine("  ISNULL(ec.Concepto_Equivalencia, nc.Concepto_ID) Concepto,");
                    query.AppendLine("  c.Descripcion ,");
                    query.AppendLine("  nc.Importe");
                    if (Abierta)
                        query.AppendLine("FROM NomCalculo nc");
                    else
                        query.AppendLine("FROM NomCalculoHistorico nc");
                    query.AppendLine("INNER JOIN Conceptos c ON nc.Concepto_ID = c.Concepto_ID");
                    query.AppendLine("INNER JOIN EquivalenciaConceptos ec ON c.Concepto_ID = ec.Concepto_ID");
                    query.AppendLine("INNER JOIN (SELECT  ");
                    query.AppendLine("              m.Trab_ID, m.FechaMov, m.Puesto_ID, m.TipoEmpleado_ID, ");
                    query.AppendLine("              m.Centro_ID, m.Depto_ID, m.Salario,IntegradoIMSS ");
                    query.AppendLine("            FROM Movimiento m ");
                    query.AppendLine("            INNER JOIN (SELECT Trab_ID, MAX(Ptr) Ptr FROM Movimiento GROUP BY Trab_ID) mm ");
                    query.AppendLine("              ON m.Trab_ID = mm.Trab_ID AND m.Ptr = mm.Ptr) m ");
                    query.AppendLine("  ON nc.Trab_ID = m.Trab_ID ");
                    query.AppendLine("INNER JOIN TBDepto d ON m.Depto_ID = d.Depto_ID");
                    query.AppendLine("WHERE nc.Ano = @Año");
                    query.AppendLine("AND nc.Periodo = @Mes");
                    query.AppendLine("AND (nc.TipoNomina_ID = @Tipo OR @Tipo = 0)");
                    query.AppendLine("AND nc.Concepto_ID >= 901 AND nc.Concepto_ID <= 998");
                    query.AppendLine("ORDER BY d.CentroCosto, Concepto");

                    try
                    {
                        SQLDataManager connector = new SQLDataManager();
                        result = connector.ExecuteQuery(query.ToString());
                    }
                    catch (Exception e)
                    {
                        result = null;
                        throw e;
                    }
                }
                else
                    throw new ArgumentException("El periodo no es válido", "Periodo");
            }
            else
                throw new ArgumentException("El año no es válido", "Año");

            return result;
        }

        public DataTable ReporteImportesGravadosExcentos(int Año, int Periodo, short Tipo)
        {
            StringBuilder query = new StringBuilder();
            DataTable result;

            if (Utilities.ValidarAño(Año))
            {
                if (Utilities.ValidarMes(Periodo))
                {
                    try
                    {
                        SQLDataManager connector = new SQLDataManager();
                        List<SqlParameter> parameters = new List<SqlParameter>();
                        parameters.Add(new SqlParameter("@Año", SqlDbType.Int, 4, ParameterDirection.Input, false, 10, 0, "", DataRowVersion.Current, Año));
                        parameters.Add(new SqlParameter("@Mes", SqlDbType.Int, 4, ParameterDirection.Input, false, 10, 0, "", DataRowVersion.Current, Periodo));
                        parameters.Add(new SqlParameter("@Tipo", SqlDbType.Int, 1, ParameterDirection.Input, false, 10, 0, "", DataRowVersion.Current, Tipo));
                        result = connector.ExecuteProcedure("usp_NominaObtenerGravadosExentos", parameters);
                    }
                    catch (Exception e)
                    {
                        result = null;
                        throw e;
                    }
                }
                else
                    throw new ArgumentException("El periodo no es válido", "Periodo");
            }
            else
                throw new ArgumentException("El año no es válido", "Año");

            return result;
        }

        public DataTable ReporteTransferenciaBMX(int Año, int Periodo, short Tipo, bool Abierta)
        {
            StringBuilder query = new StringBuilder();
            DataTable result;

            if (Utilities.ValidarAño(Año))
            {
                if (Utilities.ValidarMes(Periodo))
                {
                    query.AppendLine("DECLARE @Año INT");
                    query.AppendLine("DECLARE @Mes INT");
                    query.AppendLine("DECLARE @Tipo TINYINT");
                    query.AppendLine();
                    query.AppendFormat("SET @Año = {0}", Año);
                    query.AppendLine();
                    query.AppendFormat("SET @Mes = {0}", Periodo);
                    query.AppendLine();
                    query.AppendFormat("SET @Tipo = {0}", Tipo);
                    query.AppendLine();
                    query.AppendLine("SELECT");
                    query.AppendLine("  e.Trab_ID,");
                    query.AppendLine("  e.Paterno + ' ' + e.Materno + ' ' + e.Nombre NombreEmpleado,");
                    query.AppendLine("  SUBSTRING(d.Depto_ID,1,5) Depto_ID,");
                    query.AppendLine("  d.Descripcion Depto,");
                    query.AppendLine("  ct.Centro_ID,");
                    query.AppendLine("  ct.Descripcion Centro,");
                    query.AppendLine("  b.Descripcion Banco,");
                    query.AppendLine("  '001' Agencia,");
                    query.AppendLine("  e.CuentaDeposito,");
                    query.AppendLine("  CASE WHEN nc.Concepto_ID = 801 THEN Importe");
                    query.AppendLine("       WHEN nc.Concepto_ID = 616 THEN (Importe) * -1");
                    query.AppendLine("       ELSE CAST(0 AS MONEY) END Importe");
                    if (Abierta)
                        query.AppendLine("FROM NomCalculo nc");
                    else
                        query.AppendLine("FROM NomCalculoHistorico nc");
                    query.AppendLine("INNER JOIN Empleado e ON nc.Trab_ID = e.Trab_ID");
                    query.AppendLine("INNER JOIN Conceptos c ON nc.Concepto_ID = c.Concepto_ID");
                    query.AppendLine("INNER JOIN EquivalenciaConceptos ec ON c.Concepto_ID = ec.Concepto_ID");
                    query.AppendLine("INNER JOIN (SELECT");
                    query.AppendLine("              m.Trab_ID, m.FechaMov, m.Puesto_ID, m.TipoEmpleado_ID,");
                    query.AppendLine("              m.Centro_ID, m.Depto_ID, m.Salario,IntegradoIMSS");
                    query.AppendLine("            FROM Movimiento m");
                    query.AppendLine("            INNER JOIN (SELECT Trab_ID, MAX(Ptr) Ptr FROM Movimiento GROUP BY Trab_ID) mm");
                    query.AppendLine("                  ON m.Trab_ID = mm.Trab_ID AND m.Ptr = mm.Ptr) m");
                    query.AppendLine("  ON nc.Trab_ID = m.Trab_ID");
                    query.AppendLine("INNER JOIN TBDepto d ON m.Depto_ID = d.Depto_ID");
                    query.AppendLine("INNER JOIN TBCentros ct ON m.Centro_ID = ct.Centro_ID");
                    query.AppendLine("INNER JOIN TBRegPat rp  ON ct.RegPat_ID = rp.RegPat_ID");
                    query.AppendLine("INNER JOIN TBBanco b ON CAST(e.BancoDeposito AS INT) = CAST(b.Banco_ID AS INT)");
                    query.AppendLine("WHERE nc.Ano = @Año");
                    query.AppendLine("AND nc.Periodo = @Mes");
                    query.AppendLine("AND (nc.TipoNomina_ID = @Tipo OR @Tipo = 0)");
                    query.AppendLine("AND (nc.Concepto_ID = 801 OR nc.Concepto_ID = 616)");
                    query.AppendLine("ORDER BY SUBSTRING(d.Depto_ID,1,5), e.Trab_ID");

                    try
                    {
                        SQLDataManager connector = new SQLDataManager();
                        result = connector.ExecuteQuery(query.ToString());
                    }
                    catch (Exception e)
                    {
                        result = null;
                        throw e;
                    }
                }
                else
                    throw new ArgumentException("El periodo no es válido", "Periodo");
            }
            else
                throw new ArgumentException("El año no es válido", "Año");

            return result;
        }

        public DataTable ReporteDiferenciasNomina(int Año, int Periodo, int Tipo)
        {
            StringBuilder query = new StringBuilder();
            DataTable result;

            if (Utilities.ValidarAño(Año))
            {
                if (Utilities.ValidarMes(Periodo))
                {
                    try
                    {
                        SQLDataManager connector = new SQLDataManager();
                        List<SqlParameter> parameters = new List<SqlParameter>();
                        parameters.Add(new SqlParameter("@Año", SqlDbType.Int, 4, ParameterDirection.Input, false, 10, 0, "", DataRowVersion.Current, Año));
                        parameters.Add(new SqlParameter("@Mes", SqlDbType.Int, 4, ParameterDirection.Input, false, 10, 0, "", DataRowVersion.Current, Periodo));
                        parameters.Add(new SqlParameter("@Tipo", SqlDbType.Int, 4, ParameterDirection.Input, false, 10, 0, "", DataRowVersion.Current, Tipo));
                        result = connector.ExecuteProcedure("usp_DiferenciasNomina", parameters);
                    }
                    catch (Exception e)
                    {
                        result = null;
                        throw e;
                    }
                }
                else
                    throw new ArgumentException("El periodo no es válido", "Periodo");
            }
            else
                throw new ArgumentException("El año no es válido", "Año");

            return result;
        }

        #endregion Reportes
    }
}