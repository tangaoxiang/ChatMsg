
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<实现根据时间范围和查询用户的id获取历史消息记录>
-- =============================================
alter PROCEDURE Usp_GetHistroryMsg15
	@userid int ,  --用户id
	@begintime datetime, --开始日期
	@endtime datetime    --结束日期
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    --1.0 根据条件去路由表中获取真实存储的数据表名称
    --1.0.1 定义一个内存表
    declare @tables table(rownum int,talbename nvarchar(20))
    --1.0.2将数据插入内存表
    insert into @tables
    select ROW_NUMBER() over(order by rid),TableName from RouteMsgTable where 
	datediff(day,@begintime,EndTime)>=0 and
	datediff(day,@endtime,BeginTime)<=0
	
	--2.0 遍历第1步中的所有数据表
	declare @count int =0,@index int =1,@unionallSql nvarchar(max)='',@tbname nvarchar(20)
	select @count=COUNT(1) from @tables  --获取内存表中的数据总条数
	while(@index<=@count)
	begin
	   --获取当前遍历到的表名称
	    select @tbname=talbename from @tables where rownum=@index
	   --遍历所有表拼接成union all 查询语句
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
	--执行sql语句
	exec(@unionallSql)
	
END
GO
