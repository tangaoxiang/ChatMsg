USE [CrmChat14]
GO

/****** Object:  StoredProcedure [dbo].[Usp_GetTotalCount15]    Script Date: 10/05/2014 11:44:23 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<ivan>
-- Create date: <Create Date,,>
-- Description:	<���ڻ�ȡָ���������������>
-- =============================================
Create PROCEDURE [dbo].[Usp_GetTotalCount15]
	@tablename nvarchar(20) --������
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	declare @sql nvarchar(200)
	--��@sql��ֵ
	set @sql='declare @tcount int; select @tcount=COUNT(1) from '+ @tablename+' select totalcount=@tcount '
	
	--ִ��sql
	exec(@sql)	
	
	select totalcount= 1;
END

GO


