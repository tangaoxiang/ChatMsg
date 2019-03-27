
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<ʵ�ָ���ʱ�䷶Χ�Ͳ�ѯ�û���id��ȡ��ʷ��Ϣ��¼>
-- =============================================
alter PROCEDURE Usp_GetHistroryMsg15
	@userid int ,  --�û�id
	@begintime datetime, --��ʼ����
	@endtime datetime    --��������
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    --1.0 ��������ȥ·�ɱ��л�ȡ��ʵ�洢�����ݱ�����
    --1.0.1 ����һ���ڴ��
    declare @tables table(rownum int,talbename nvarchar(20))
    --1.0.2�����ݲ����ڴ��
    insert into @tables
    select ROW_NUMBER() over(order by rid),TableName from RouteMsgTable where 
	datediff(day,@begintime,EndTime)>=0 and
	datediff(day,@endtime,BeginTime)<=0
	
	--2.0 ������1���е��������ݱ�
	declare @count int =0,@index int =1,@unionallSql nvarchar(max)='',@tbname nvarchar(20)
	select @count=COUNT(1) from @tables  --��ȡ�ڴ���е�����������
	while(@index<=@count)
	begin
	   --��ȡ��ǰ�������ı�����
	    select @tbname=talbename from @tables where rownum=@index
	   --�������б�ƴ�ӳ�union all ��ѯ���
	   if(@index=@count)
	   begin
	   set @unionallSql=@unionallSql+' select * from '+@tbname+' where ToUserId='+cast(@userid as nvarchar(20))+' and  DATEDIFF(DAY,'''+CAST(@begintime as nvarchar(20))+''',SendTime)>=0 and DATEDIFF(DAY,'''+CAST(@endtime as nvarchar(20))+''',SendTime)<=0 '

	   end
	   else
	   begin
		 set @unionallSql=@unionallSql+' select * from '+@tbname+' where ToUserId='+cast(@userid as nvarchar(20))+' and  DATEDIFF(DAY,'''+CAST(@begintime as nvarchar(20))+''',SendTime)>=0 and DATEDIFF(DAY,'''+CAST(@endtime as nvarchar(20))+''',SendTime)<=0 union all '
		    end
	   set @index =@index +1;
	end
	--ִ��sql���
	exec(@unionallSql)
	
END
GO
