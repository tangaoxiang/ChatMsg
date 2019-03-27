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
-- Description:	<���ݱ����ƴ���һ�������,ͬʱ��·�ɱ��в���һ�����ݣ�ͬʱ�޸���һ�����ݵĽ���ʱ��>
-- =============================================
CREATE PROCEDURE [dbo].[Usp_CreateTable15]
	@TableName nvarchar(20), --������
	@begintime datetime --�����ݵĿ�ʼʱ�����һ�����ݵĽ���ʱ��
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

  --1.0 ���ݱ����ƴ���һ�������
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

--ִ�������Ĵ�������
exec(@tableSql);

--2.0 �޸���һ�����ݵĽ���ʱ��
declare @lastrid int --���һ���������
select @lastrid=MAX(rid) from dbo.RouteMsgTable
update RouteMsgTable set EndTime=@begintime where rid=@lastrid


--3.0 ��·�ɱ��в���һ������
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


