USE [hospira]
GO
/****** Object:  StoredProcedure [dbo].[usp_ReciboNominaEmpledosConEmail]    Script Date: 05/11/2012 20:03:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		José Antonio Campos
-- Create date: Septiembre 20, 2011
-- Description:	Obtiene los empleados que cuentan con correo electrónico
-- =============================================
ALTER PROCEDURE [dbo].[usp_ReciboNominaEmpledosConEmail]
AS
	SELECT DISTINCT
		e.Trab_ID,
		e.Nombre,
		e.Paterno,
		e.Materno,
		ee.EMAIL
	FROM Empleado e
	INNER JOIN EmpleadoExtra ee ON e.Trab_ID = ee.Trab_ID
	WHERE ee.EMAIL IS NOT NULL AND ee.EMAIL != ''
	ORDER BY e.Trab_ID
	
