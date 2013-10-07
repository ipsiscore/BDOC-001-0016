USE [hospira]
GO
/****** Object:  StoredProcedure [dbo].[usp_NominaObtenerGravadosExentos]    Script Date: 05/11/2012 20:03:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:	José Antonio Campos
-- Create date: Julio 26, 2011
-- Description:	Obtiene el listado de gravados y exentos por periodo.
-- =============================================
-- =============================================
-- Editor:	José Antonio Campos
-- Modified date: Julio 15, 2012
-- Description:	Se agrego gravados y exentos para PTU
-- =============================================
ALTER PROC [dbo].[usp_NominaObtenerGravadosExentos]
@Año INT,
@Mes INT,
@Tipo TINYINT
AS 
BEGIN
	DECLARE
	@conceptos VARCHAR(500)
	IF @Tipo = 4
		SET @conceptos = '101, 600, 800, 326, 616, 801'
	ELSE 
		SET @conceptos = '32,35,38,43,44,301,302,304,314,318,320,321,322,324,329,330,331,332,333,335,610,611,612,613,619'

	SELECT DISTINCT
	  nc.Trab_ID,
	  e.Paterno + ' ' + e.Materno + ' ' + e.Nombre NombreEmpleado,
	  d.CentroCosto,
	  ISNULL(d.CuentaAcumuladora, '') NombreCentroCosto,
	  ec.Concepto_Equivalencia Concepto_ID,
	  CASE WHEN  ec.Concepto_Equivalencia = '3813'  THEN 'GRAVADO AGUINALDO'
		   WHEN  ec.Concepto_Equivalencia = '6056'  THEN 'EXENTO AGUINALDO'
	  ELSE c.Descripcion END Concepto,
	  CASE WHEN  ec.Concepto_Equivalencia = '3813'  THEN nc.Gravado
		   WHEN  ec.Concepto_Equivalencia = '6056'  THEN CAST(0 AS MONEY)
		   WHEN  c.Concepto_ID BETWEEN 601 AND 799 THEN nc.Gravado * -1
		   WHEN  c.Concepto_ID = 613 AND nc.Gravado = 0 THEN nc.Importe
		   WHEN  nc.Gravado = 0 AND nc.Excento = 0 AND ec.Concepto_Equivalencia NOT IN ('1026','1067','5915','9921') THEN nc.Importe
	  ELSE nc.Gravado END Gravado,
	  CASE WHEN  ec.Concepto_Equivalencia = '3813'  THEN CAST(0 AS MONEY)
		   WHEN  ec.Concepto_Equivalencia = '6056'  THEN nc.Excento
		   WHEN  nc.Gravado = 0 AND nc.Excento = 0 AND ec.Concepto_Equivalencia IN ('1026','1067','5915','9921') THEN nc.Importe
	  ELSE nc.Excento END Excento,
	  m.Centro_ID,
	 CASE WHEN  ec.Concepto_Equivalencia = '3813'  THEN 1
		  WHEN  ec.Concepto_Equivalencia = '6056'  THEN 2
		  WHEN  c.Concepto_ID = 613 AND nc.Gravado = 0 THEN 1
	      WHEN nc.Gravado != CAST(0 AS MONEY) THEN 1 
	      WHEN  nc.Gravado = 0 AND nc.Excento = 0 AND ec.Concepto_Equivalencia NOT IN ('1026','1067','5915','9921') THEN 1
	      WHEN  nc.Gravado = 0 AND nc.Excento = 0 AND ec.Concepto_Equivalencia IN ('1026','1067','5915','9921') THEN 2
	  ELSE 2 END Tipo,
	  c.Tipo_IDa
	FROM vw_NomCalculo nc
	INNER JOIN Empleado e ON nc.Trab_ID = e.Trab_ID 
	INNER JOIN Conceptos c ON nc.Concepto_ID = c.Concepto_ID
	INNER JOIN ( SELECT Concepto_ID, Concepto_Equivalencia
				 FROM EquivalenciaConceptos WHERE Concepto_ID != 318
				 UNION SELECT 318,'3813' UNION SELECT 318, '6056' UNION SELECT 336, '1003' UNION SELECT 337,'1026') ec
	  ON c.Concepto_ID = ec.Concepto_ID
	INNER JOIN (SELECT  
				  m.Trab_ID, m.FechaMov, m.Puesto_ID, m.TipoEmpleado_ID, 
				  m.Centro_ID, m.Depto_ID, m.Salario,IntegradoIMSS 
				FROM Movimiento m 
				INNER JOIN (SELECT Trab_ID, MAX(Ptr) Ptr FROM Movimiento GROUP BY Trab_ID) mm
				  ON m.Trab_ID = mm.Trab_ID AND m.Ptr = mm.Ptr) m 
	  ON nc.Trab_ID = m.Trab_ID 
	INNER JOIN TBDepto d ON m.Depto_ID = d.Depto_ID
	WHERE nc.Ano = @Año
	AND nc.Periodo = @Mes
	AND (nc.TipoNomina_ID = @Tipo OR @Tipo = 0)
	AND c.Concepto_ID IN (SELECT Value FROM dbo.UTILfn_Split(@conceptos,','))
	ORDER BY NombreEmpleado, Tipo,ec.Concepto_Equivalencia
END