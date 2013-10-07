USE [hospira]
GO
/****** Object:  StoredProcedure [dbo].[usp_NominaObtenerAcumuladoConcpetosXEmpleado]    Script Date: 05/11/2012 20:02:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		José Antonio Campos
-- Create date: Julio 28, 2011
-- Description:	Obtiene los acumulados de conceptos por empleado
-- =============================================
-- =============================================
-- Modifier:		José Antonio Campos
-- Update date: Mayo 30, 2012
-- Description:	Se modifico para mostrar total de nómina pagada
-- =============================================
ALTER PROCEDURE [dbo].[usp_NominaObtenerAcumuladoConcpetosXEmpleado]
@Año INT ,
@Mes INT 
AS
BEGIN

	SELECT 
	  e.Trab_ID, 
	  e.Paterno + ' ' + e.Materno + ' ' + e.Nombre NombreEmpleado,  
	  CAST(e.FechaIngreso AS DATETIME) FechaIngreso,  
	  CAST(ISNULL(eb.FechaBaja,'18000101') AS DATETIME) FechaBaja,  
	  te.Descripcion Convenio,  
	  m.Centro_ID,  
	  d.CentroCosto,  
	  rp.RegPat_ID,  
	  rp.Registro,  
	  p.Puesto_ID,  
	  c.Concepto_ID ConceptoSICOSS,
	  ec.Concepto_Equivalencia Concepto_ID,  
	  c.Descripcion Concepto, 
	  nc.Importe,
	  CASE WHEN nc.Concepto_ID = 334 AND ec.Concepto_Equivalencia = '5910' THEN 'Deduccion'
	       WHEN nc.Concepto_ID = 31 OR nc.Concepto_ID = 32 OR
				nc.Concepto_ID = 38 OR nc.Concepto_ID = 91 OR nc.Concepto_ID = 93 THEN 'Auxiliar'
		   WHEN nc.Concepto_ID IN (35, 43,44, 336, 337) THEN 'Percepcion'
		   WHEN nc.Concepto_ID >= 301  AND nc.Concepto_ID <= 599 THEN 'Percepcion'
		   WHEN nc.Concepto_ID >= 601  AND nc.Concepto_ID <= 799 THEN 'Deduccion'
		   WHEN nc.Concepto_ID >= 901  AND nc.Concepto_ID <= 998 THEN 'Provision' END Tipo,
		   
	  CASE WHEN nc.Concepto_ID = 334 AND ec.Concepto_Equivalencia = '5910' THEN 2
		   WHEN nc.Concepto_ID = 31 OR nc.Concepto_ID = 32 OR
				nc.Concepto_ID = 38 OR nc.Concepto_ID = 91 OR nc.Concepto_ID = 93 THEN 4
		   WHEN nc.Concepto_ID IN (35,43,44,336,337) THEN 1
		   WHEN nc.Concepto_ID >= 301  AND nc.Concepto_ID <= 599 THEN 1
		   WHEN nc.Concepto_ID >= 601  AND nc.Concepto_ID <= 799 THEN 2
		   WHEN nc.Concepto_ID = 91 THEN 3
		   WHEN nc.Concepto_ID >= 901  AND nc.Concepto_ID <= 998 THEN 3 END Tipo_ID,
	nc.Periodo,
	map.Provision,
	map.Gasto,
	nc.TipoNomina_ID
	FROM dbo.Empleado e
	INNER JOIN (SELECT   
					m.Trab_ID, 
					m.FechaMov, 
					m.Puesto_ID, 
					m.TipoEmpleado_ID, 
				    m.Centro_ID, 
				    m.Depto_ID, 
				    m.Salario,
				    m.IntegradoIMSS 
				FROM dbo.Movimiento m 
				INNER JOIN (SELECT 
								Trab_ID, 
								MAX(Ptr) Ptr 
							FROM dbo.Movimiento 
							GROUP BY Trab_ID) mm 
				  ON m.Trab_ID = mm.Trab_ID AND m.Ptr = mm.Ptr) m 
	  ON e.Trab_ID = m.Trab_ID 
	INNER JOIN dbo.TBTipoEmpleado TE ON m.TipoEmpleado_ID = te.TipoEmpleado_ID 
	INNER JOIN dbo.TBDepto d ON m.Depto_ID = d.Depto_ID 
	INNER JOIN dbo.TBPuesto p ON m.Puesto_ID = p.Puesto_ID 
	INNER JOIN dbo.vw_NomCalculo nc ON e.Trab_ID = nc.Trab_ID 
	INNER JOIN dbo.TBCentros ct ON m.Centro_ID = ct.Centro_ID
	INNER JOIN dbo.TBRegPat rp ON ct.RegPat_ID = rp.RegPat_ID
	INNER JOIN (SELECT Concepto_ID, Descripcion, x FROM (SELECT  
															Concepto_ID, 
															Descripcion,
															ROW_NUMBER() OVER (PARTITION BY Concepto_ID ORDER BY Concepto_ID) x
														FROM dbo.Conceptos) cs) c
		ON nc.Concepto_ID = c.Concepto_ID AND c.x = 1
	INNER JOIN (SELECT 
					Concepto_ID , 
					Concepto_Equivalencia 
				FROM dbo.equivalenciaConceptos WHERE Concepto_ID != 312 
				UNION 
				SELECT 603 Concepto_ID, '9316' Concepto_Equivalencia
				UNION
				--SELECT 334 Concepto_ID, '5910' Concepto_Equivalencia
				SELECT 623 Concepto_ID, '5910' Concepto_Equivalencia
				UNION
				SELECT 336 Concepto_ID, '1003' Concepto_Equivalencia
				UNION
				SELECT 337 Concepto_ID, '1067' Concepto_Equivalencia
				UNION
				SELECT 312 Concepto_ID, '1003' Concepto_Equivalencia
				) ec
		ON c.Concepto_ID = ec.Concepto_ID 
	LEFT JOIN dbo.MapeoProGTo map ON ec.Concepto_Equivalencia = map.Concepto_Hospira
	LEFT JOIN (SELECT
					Trab_ID,
					MAX(Ptr) Ptr, 
					MAX(FechaMov) FechaBaja
			   FROM Movimiento 
			   WHERE Mov_ID = 'B' 
			   GROUP BY Trab_ID) eb 
	  ON e.Trab_ID = eb.Trab_ID 
	WHERE nc.Ano = @Año 
	AND nc.Periodo <= @Mes 
	AND nc.Concepto_ID IN (32,35,38,43,44,91,93,301,302,304,305,307,311,312,313,314,316,317,318,318,318,320,321,322,323,324,
						   325,326,327,328,329,330,331,332,333,334,336,337,601,602,603,604,606,607,608,609,610,611,612,
						   613,615,616,617,618,619,620,621,622,623,624,625,905,908,909,910,911,912,913,914,915,916,917)
END
