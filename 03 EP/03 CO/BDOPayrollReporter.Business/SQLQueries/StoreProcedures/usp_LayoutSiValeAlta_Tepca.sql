USE [hospira]
GO
/****** Object:  StoredProcedure [dbo].[usp_LayoutSiValeAlta_Tepca]    Script Date: 05/11/2012 20:00:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Suriel Lopez A.
-- Create date: 23/08/2011
-- Description:	
-- =============================================
ALTER PROCEDURE [dbo].[usp_LayoutSiValeAlta_Tepca]
	@Mes INT = 0,
	@Año INT = 0
AS
BEGIN
	
	SET NOCOUNT ON;

	SELECT [Empleado].[Trab_ID] AS Empleado,
	  REPLACE([Empleado].[Nombre], ' ', '') + ' ' + [Empleado].[Paterno] + ' ' + [Empleado].[Materno] AS Nombre,
	  RTRIM(LTRIM([Empleado].Nombre)) AS NombreCompleto,
	  RTRIM(LTRIM([Empleado].Paterno)) AS Paterno,
	  RTRIM(LTRIM([Empleado].Materno)) AS Materno,
	  REPLACE([Empleado].[Rfc], '-', '') AS Rfc,
	  [TBDepto].[Depto_ID] AS Centro
	FROM [dbo].[Empleado]
	  INNER JOIN (SELECT [Trab_ID], 
					MAX([FechaMov]) AS [FechaMov], 
					[Salario], [Depto_ID] 
				  FROM [dbo].[Movimiento] 
				  GROUP BY [Trab_ID], [Salario], [Depto_ID]) AS [Movimiento] ON [Empleado].[Trab_ID] = [Movimiento].[Trab_ID]
	  INNER JOIN [dbo].[TBDepto] ON [Movimiento].[Depto_ID] = [TBDepto].[Depto_ID]
	WHERE YEAR([FechaIngreso]) = @Año
	  AND MONTH([FechaIngreso]) = @Mes
    
END
