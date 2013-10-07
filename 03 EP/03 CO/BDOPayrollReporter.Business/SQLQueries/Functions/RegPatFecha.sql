USE [hospira]
GO
/****** Object:  UserDefinedFunction [dbo].[RegPatFecha]    Script Date: 05/11/2012 19:55:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER FUNCTION [dbo].[RegPatFecha]  (      
	@Trab_Id varchar(10),      
	@Fecha datetime  
)  
RETURNS int  
AS  
BEGIN  
	
	declare @Centro int;      
	declare @RegPat int;      
	select top 1 @Centro=Centro_id 
	from movimiento 
	where trab_id = @Trab_Id and fechamov <= @Fecha 
	order by fechamov desc      

	select @RegPat = RegPat_id 
	from tbcentros 
	where centro_id = @Centro  

	return @RegPat 
END  