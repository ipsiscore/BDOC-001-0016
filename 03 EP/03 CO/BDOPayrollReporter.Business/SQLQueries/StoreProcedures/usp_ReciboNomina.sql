USE [hospira]
GO
/****** Object:  StoredProcedure [dbo].[usp_ReciboNomina]    Script Date: 07/02/2012 11:53:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		José Antonio Campos
-- Create date: Septiembre 20, 2011
-- Description:	Obtiene los conceptos para recibos de nómina
-- =============================================
-- =============================================
-- Modifier:		José Antonio Campos
-- Modification date : Abril 27, 2012
-- Description:	Se modificó para agregar devolución de IMSS en un finiquito
-- =============================================
-- =============================================
-- Modifier:		José Antonio Campos
-- Modification date : Julio 2, 2012
-- Description:	Se agrego filtro dde tipo nómina al fondo de ahorro
-- =============================================
ALTER PROCEDURE [dbo].[usp_ReciboNomina]
	@Año INT,
	@Mes INT,
	@Tipo TINYINT,
	@Ids NVARCHAR(4000)
AS
	DECLARE @TopeINFONAVIT money
	SET @TopeINFONAVIT = (SELECT TOP 1 MinZona1 * TopeInfonavit FROM TBMinTop 
	WHERE DATEPART(YEAR,FECHA) = @Año ORDER BY Fecha DESC)
	
	
	SELECT DISTINCT
		e.Trab_ID Empleado,
		e.Paterno + ' ' + e.Materno + ' ' + e.Nombre  Nombre,
		REPLACE(e.RFC, '-', '')  Rfc,
		e.Curp  Curp,
		CAST(e.FechaIngreso AS DATETIME) FechaIngreso,
		(SELECT TOP 1 CAST(FechaMov AS DATETIME) FROM Movimiento WHERE Trab_Id = e.Trab_ID AND Mov_ID = 'B' ORDER BY FechaMov DESC) FechaBaja,
		REPLACE(e.IMSS, '-', '')  Nss,
		m.Salario  Sd,
		CASE WHEN m.IntegradoIMSS > @TopeINFONAVIT THEN @TopeINFONAVIT ELSE ISNULL(m.IntegradoIMSS, 0.0) END Sdi,
		t.Descripcion  Convenio,
		p.Descripcion  Puesto,
		c.Descripcion  CentroTrabajo,
		d.Descripcion  Departamento,
		d.CentroCosto  CentroCosto,
		ISNULL(ec.Concepto_Equivalencia, n.Concepto_ID)  Concepto,
		CASE WHEN @Tipo = 1 AND n.Concepto_ID = 313 THEN 'F.A. FINIQ EMPRESA' 
			 /*Se agregó para un finiquito*/
			 WHEN @Tipo = 2 AND n.Concepto_ID = 339 THEN 'DEVOLUCION DE IMSS' 
			 WHEN @Tipo = 2 AND n.Concepto_ID = 340 THEN 'DEVOLUCION DE IMSS' 
			 /*Se agregó para un finiquito*/
		ELSE cn.Descripcion END  Descripcion,
		n.Dato  Unidad,
		CAST(CASE 
				 /*Se agregó para un finiquito*/
				 WHEN @Tipo = 2 AND n.Concepto_ID = 339 THEN 0 
				 WHEN @Tipo = 2 AND n.Concepto_ID = 340 THEN 0  
				 /*Se agregó para un finiquito*/
				 WHEN n.Concepto_ID = 32 THEN n.Importe
				 WHEN n.Concepto_ID >= 301 AND n.Concepto_ID <= 599 THEN n.Importe ELSE 0 END AS MONEY)  Percepcion,
		CAST(CASE 
				 /*Se agregó para un finiquito*/
				WHEN @Tipo = 2 AND n.Concepto_ID = 339 THEN n.Importe  
				WHEN @Tipo = 2 AND n.Concepto_ID = 340 THEN n.Importe 
				 /*Se agregó para un finiquito*/
				WHEN n.Concepto_ID >= 601 AND n.Concepto_ID <= 799 THEN n.Importe ELSE 0 END AS MONEY)  Deduccion,
		v.Despensa  ValesDespensa,
		0  ValesGasolina,
		CASE WHEN s.FondoFiniquito = 0 THEN s.FondoAhorro ELSE s.FondoFiniquito END FondoAhorro,
		CASE WHEN n.Concepto_ID >= 601 AND n.Concepto_ID <= 799 THEN 1 ELSE 0 END Tipo,
		(SELECT CAST(ncd.Importe AS INT) Importe FROM vw_NomCalculo ncd WHERE ncd.Trab_ID = e.Trab_ID 
			AND ncd.Concepto_ID = 12 AND ncd.Ano = @Año AND ncd.Periodo = @Mes AND ncd.TipoNomina_ID = @Tipo) Dias,
		CASE WHEN  @Tipo = 2 THEN 0 ELSE(SELECT ncd.Importe FROM NomCalculo ncd WHERE ncd.Trab_ID = e.Trab_ID AND ncd.Concepto_ID = 313 
			AND ncd.Ano = @Año AND ncd.TipoNomina_ID = 6) END Descuento
	FROM Empleado e
	INNER JOIN vw_NomCalculo n ON e.Trab_ID = n.Trab_ID
	INNER JOIN (SELECT  m.Trab_ID, m.FechaMov, m.Puesto_ID, m.TipoEmpleado_ID,
						m.Centro_ID, m.Depto_ID, m.Salario, m.IntegradoIMSS
				FROM Movimiento m
				INNER JOIN (SELECT Trab_ID, MAX(Ptr) Ptr FROM Movimiento GROUP BY Trab_ID) mm
					ON m.Trab_ID = mm.Trab_ID AND m.Ptr = mm.Ptr) m
		ON e.Trab_ID = m.Trab_ID
	INNER JOIN TBPuesto p ON m.Puesto_ID = p.Puesto_ID
	INNER JOIN TBTipoEmpleado t ON m.TipoEmpleado_ID = t.TipoEmpleado_ID
	INNER JOIN TBCentros c ON m.Centro_ID = c.Centro_ID
	INNER JOIN TBDepto d ON m.Depto_ID = d.Depto_ID
	INNER JOIN Conceptos cn ON n.Concepto_ID = cn.Concepto_ID
	INNER JOIN (SELECT Concepto_ID, Concepto_Equivalencia, @Tipo Tipo FROM EquivalenciaConceptos
				WHERE Concepto_ID != CASE WHEN @Tipo = 2 THEN 313 ELSE 0 END
				UNION ALL SELECT 336 Concepto_ID, '1003' Concepto_Equivalencia, 2 Tipo
				UNION ALL SELECT 337 Concepto_ID, '1026' Concepto_Equivalencia, 2 Tipo
				UNION ALL SELECT 313 Concepto_ID, '9921' Concepto_Equivalencia, 2 Tipo
				UNION ALL SELECT 623 Concepto_ID, '5910' Concepto_Equivalencia, 2 Tipo
				/*Se agregó para un finiquito*/
				UNION ALL SELECT 339 Concepto_ID, '9500' Concepto_Equivalencia, 2 Tipo
				UNION ALL SELECT 340 Concepto_ID, '9500' Concepto_Equivalencia, 2 Tipo
				/*Se agregó para un finiquito*/
		) ec ON n.Concepto_ID = ec.Concepto_ID AND Tipo = @Tipo
	LEFT JOIN IMSSLiquidacion97 l ON m.Trab_ID = l.Trab_ID 
		AND YEAR(m.FechaMov) = l.Ano AND MONTH(m.FechaMov) = l.Periodo
	LEFT JOIN (SELECT V.Trab_ID, SUM(v.Importe)  Despensa FROM vw_NomCalculo v 
			   WHERE v.Concepto_ID = 32 AND v.Ano = @Año AND v.Periodo = @Mes 
			   AND v.TipoNomina_ID = @Tipo GROUP BY v.Trab_ID) v
		ON e.Trab_ID = v.Trab_ID
	LEFT JOIN (SELECT Trab_ID, SUM(ISNULL(FondoAhorro,0)) FondoAhorro, SUM(ISNULL(FondoFiniquito,0)) FondoFiniquito FROM (
						SELECT Trab_ID, Importe FondoAhorro, CAST(0  AS MONEY) FondoFiniquito 
						FROM vw_NomCalculo WHERE Concepto_ID = 94 AND  Ano = @Año AND Periodo = @Mes AND TipoNomina_ID = @Tipo
						UNION ALL 
						SELECT Trab_ID, CAST(0  AS MONEY) FondoAhorro, Importe FondoFiniquito 
						FROM vw_NomCalculo WHERE Concepto_ID = 95 AND  Ano = @Año AND Periodo = @Mes AND TipoNomina_ID = @Tipo) s 
						GROUP BY Trab_ID) s 
		ON  e.Trab_ID = s.Trab_ID 

	WHERE n.Ano = @Año
	AND n.Periodo = @Mes
	AND n.TipoNomina_ID = @Tipo
	AND (n.Concepto_ID >= 301 AND n.Concepto_ID <= 599
		 OR n.Concepto_ID >= 601 AND n.Concepto_ID <= 799
		 OR n.Concepto_ID = CASE WHEN @Tipo = 2 THEN 32 ELSE 0 END)
	AND e.Trab_ID IN (SELECT Value FROM dbo.UTILfn_Split(@IDS,','))
	ORDER BY Nombre, Concepto
