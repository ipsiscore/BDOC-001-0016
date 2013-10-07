-- =============================================
-- Author:	    José Antonio Campos
-- Create date: Junio 08, 2012
-- Description:	Obtiene los empleado dados de alta en el mes solicitado, con los datos
--              necesarios para solicitar tarjetas SiVale 
-- =============================================
CREATE PROCEDURE usp_LayoutSiValeEmision
@Mes INT = 0,
@Año INT = 0,
@Titular BIT
AS
BEGIN
	SELECT 
		e.Trab_ID Referencia,
		CASE WHEN @Titular = 0 THEN ee.NOMCORTO ELSE ee.NOMCORTOADI END NOMCORTO 
	FROM dbo.Empleado e
	INNER JOIN dbo.EmpleadoExtra ee ON e.Trab_ID = ee.Trab_ID
	WHERE YEAR(e.FechaIngreso) = @Año
	AND MONTH(e.FechaIngreso) = @Mes
END	