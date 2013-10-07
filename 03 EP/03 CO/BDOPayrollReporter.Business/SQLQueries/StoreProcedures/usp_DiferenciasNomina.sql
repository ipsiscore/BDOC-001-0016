-- =============================================
-- Author:		José Antonio Campos
-- Create date: Mayo 11, 2012
-- Description:	Obtiene los acumulados de nómina por empleado
-- =============================================
CREATE PROCEDURE [dbo].[usp_DiferenciasNomina]
@Mes INT,
@Año INT,
@Tipo TINYINT
AS
BEGIN

	SELECT 
		n.Trab_ID,
		e.Paterno + ' ' + ISNULL(e.Materno,'') + ' ' + e.Nombre Nombre,
		SUM(CASE WHEN n.Concepto_ID = 801 THEN Importe ELSE CAST(0 AS MONEY) END) + 
		SUM(CASE WHEN n.Concepto_ID = 616 THEN (Importe) * -1 ELSE CAST(0 AS MONEY) END) Importe ,
		Periodo
	FROM vw_NomCalculo n
	INNER JOIN Empleado e ON n.Trab_ID = e.Trab_ID
	WHERE n.Ano = @Año
	AND n.Periodo <= @Mes
	AND (n.Concepto_ID = 801 OR n.Concepto_ID = 616)
	AND n.TipoNomina_ID != 0 --@Tipo
GROUP BY n.Trab_ID, e.Paterno, e.Materno, e.Nombre, n.Periodo

END