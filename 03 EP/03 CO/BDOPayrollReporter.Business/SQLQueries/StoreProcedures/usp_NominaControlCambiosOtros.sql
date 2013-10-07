USE [hospira]
GO
/****** Object:  StoredProcedure [dbo].[usp_NominaControlCambiosOtros]    Script Date: 05/11/2012 20:01:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		José Antonio Campos
-- Create date: Julio 28, 2011
-- Description:	Obtiene los acumulados de conceptos por empleado
-- =============================================
ALTER PROCEDURE [dbo].[usp_NominaControlCambiosOtros]
@Año INT,
@Mes INT
AS
DECLARE 
@FechaIni DATETIME,
@FechaFin DATETIME

SET DATEFORMAT dmy

SET @FechaIni = CAST('1/' + CAST(@Mes AS CHAR) + '/' + CAST(@Año AS CHAR)  AS DATETIME)
SET @FechaFin = DATEADD(MONTH,1,@FechaIni)

SELECT 
	e.Trab_ID,
	e.Paterno + ' ' + e.Materno + ' ' + e.Nombre  NombreEmpleado,
	mo.FechaMov,
	mo.Descripcion,
	--CAST(mo.ValorNuevo AS MONEY) ValorNuevo
	mo.ValorNuevo
FROM Empleado e
INNER JOIN (SELECT ROW_NUMBER() OVER (ORDER BY FechaCaptura desc) x,
			Trab_ID,CONVERT(VARCHAR(10),FechaCaptura,103) FechaMov,
			Campo Descripcion,ValorNuevo FROM MovimientoActualiza
			WHERE Campo = 'Salario' AND FechaCaptura BETWEEN @FechaIni AND @FechaFin) mo
	ON e.Trab_ID = mo.Trab_ID AND mo.x = 1
UNION ALL
SELECT 
	e.Trab_ID,
	e.Paterno + ' ' + e.Materno + ' ' + e.Nombre  NombreEmpleado,
	CONVERT(VARCHAR(10),FechaCaptura,103) FechaMov,
	'Puesto' Descripcion,
	mo.ValorNuevo + ' ' + ISNULL(p.Descripcion,'') ValorNuevo
FROM MovimientoActualiza mo
INNER JOIN Empleado e ON mo.Trab_ID = e.Trab_ID
INNER JOIN TBPuesto p ON mo.ValorNuevo = p.Puesto_ID
WHERE mo.Campo = 'Puesto_ID'
AND mo.FechaCaptura BETWEEN @FechaIni AND @FechaFin
UNION ALL
SELECT 
	e.Trab_ID,
	e.Paterno + ' ' + e.Materno + ' ' + e.Nombre  NombreEmpleado,
	CONVERT(VARCHAR(10),FechaCaptura,103) FechaMov,
	'Tipo Empleado' Descripcion,
	mo.ValorNuevo + ' ' + ISNULL(t.Descripcion,'') ValorNuevo
FROM MovimientoActualiza mo
INNER JOIN Empleado e ON mo.Trab_ID = e.Trab_ID
INNER JOIN TBTipoEmpleado t ON mo.ValorNuevo = t.TipoEmpleado_ID
WHERE mo.Campo = 'TipoEmpleado_ID'
AND mo.FechaCaptura BETWEEN @FechaIni AND @FechaFin






