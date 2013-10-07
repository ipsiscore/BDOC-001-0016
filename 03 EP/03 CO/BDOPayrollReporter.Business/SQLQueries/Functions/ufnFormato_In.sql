USE [hospira]
GO
/****** Object:  UserDefinedFunction [dbo].[ufnFormato_In]    Script Date: 05/11/2012 19:57:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER FUNCTION [dbo].[ufnFormato_In] (
	@Datos varchar(4000)
) 
RETURNS @tab TABLE (c1 VARCHAR(4000) NULL,c2 VARCHAR(4000) NULL) 
AS 
BEGIN 
	DECLARE @POSINI INT; 
	DECLARE @POSACT INT;
	DECLARE @LEN INT;       
	SET @LEN=0;       
	SET @POSINI=1;       
	SET @POSACT=0;       
	SET @LEN=LEN(@Datos);      
	WHILE @POSACT <= @LEN       
	BEGIN             
		SET @POSINI=@POSACT+1;             
		SET @POSACT=@POSACT+1;             
		SET @POSACT=CHARINDEX(',',@Datos,@POSACT);             
		IF @POSACT=0                   
		SET @POSACT=@LEN+1;             
		INSERT INTO @tab (c1) 
		VALUES(SUBSTRING(@Datos,@POSINI,@POSACT-@POSINI));       
	END       
	RETURN; 
END 