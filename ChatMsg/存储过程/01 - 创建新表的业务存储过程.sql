USE [CrmChat14]
GO

/****** Object:  StoredProcedure [dbo].[Usp_CreateTable15]    Script Date: 10/05/2014 11:44:02 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<ivan>
-- Create date: <Create Date,,>
-- Description:	<根据表名称创建一个物理表,同时向路由表中插入一条数据，同时修改上一条数据的结束时间>
-- =============================================
CREATE PROCEDURE [dbo].[Usp_CreateTable15]
	@TableName nvarchar(20), --表名称
	@begintime datetime --新数据的开始时间和上一条数据的结束时间
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

  --1.0 根据表名称创建一个物理表
  declare @tableSql nvarchar(2000)=''
  set @tableSql='
  CREATE TABLE [dbo].['+@TableName+'](
	[cid] [int] IDENTITY(1,1) NOT NULL,
	[ToUserId] [int] NOT NULL,
	[ToRealName] [nvarchar](100) NOT NULL,
	[FromUserId] [int] NULL,
	[FromRealName] [nvarchar](100) NOT NULL,
	[MessageBody] [nvarchar](max) NULL,
	[SendTime] [datetime] NOT NULL,
 CONSTRAINT [PK_'+@TableName+'] PRIMARY KEY CLUSTERED 
(
	[cid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]'

--执行物理表的创建工作
exec(@tableSql);

--2.0 修改上一条数据的结束时间
declare @lastrid int --最后一个表的主键
select @lastrid=MAX(rid) from dbo.RouteMsgTable
update RouteMsgTable set EndTime=@begintime where rid=@lastrid


--3.0 向路由表中插入一条数据
INSERT INTO [RouteMsgTable]
           ([TableName]
           ,[BeginTime]
           ,[EndTime])
     VALUES
           (
           @TableName,
           @begintime,
           NULL
           )
END

GO


