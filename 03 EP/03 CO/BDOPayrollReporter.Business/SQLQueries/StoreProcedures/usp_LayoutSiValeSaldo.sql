-- =============================================
-- Author:	    José Antonio Campos
-- Create date: Junio 08, 2012
-- Description:	Obtiene los importes para tarjetas SiVale 
-- =============================================
CREATE PROCEDURE usp_LayoutSiValeSaldo
@Mes INT,
@Año INT, 
@Concepto INT
AS
BEGIN
	SELECT
		nc.Trab_ID Referencia,
		CASE WHEN @Concepto = 97 THEN TARJTITALIM ELSE TARJTITPREVIS END Tarjeta,
		nc.Importe
	FROM dbo.vw_NomCalculo nc
	INNER JOIN dbo.EmpleadoExtra ee ON nc.Trab_ID = ee.Trab_ID
	WHERE nc.Ano = @Año
	AND nc.Periodo = @Mes
	AND nc.TipoNomina_ID = 1
	AND Concepto_ID = @Concepto
END
