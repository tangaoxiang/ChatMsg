using Chat.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Lib
{
    using Chat.Logger;
    using ServiceStack.Redis;
    using System.Data;
    /// <summary>
    /// 负责开启死循环，每隔固定的时间间隔后处理历史记录存储逻辑
    /// </summary>
    public class BizProcessor
    {
        /// <summary>
        /// 处理入口
        /// </summary>
        public void Process()
        {
            DateTime now = DateTime.Now;

            //0.0 获取web.config中配置的时间间隔
            string ts = System.Configuration.ConfigurationManager.AppSettings["historybizProcessTimeSpan"];
            int timespan = 0;
            if (int.TryParse(ts, out timespan) == false)
            {
                timespan = 5;
            }

            //0.0.2 获取每个表的最大存储条数
            string maxnum = System.Configuration.ConfigurationManager.AppSettings["maxnum"];
            int maxnumer = 0;
            if (int.TryParse(maxnum, out maxnumer) == false)
            {
                maxnumer = 5;
            }

            LogHelper.WriteInfoLog("historybizProcessTimeSpan(历史记录保存线程的执行间隔)=" + timespan + ", maxnum(每个表的存储最大条数)=" + maxnum);

            while (true)
            {
                try
                {
                    //0.0.1 判断当前执行的服务器时间和now 的时间间隔是否超出了timespan
                    if ((DateTime.Now - now).TotalSeconds > timespan)
                    {
                        //1.0 获取redis中的历史消息记录
                        var historyMsgList = GetHistoryMsgList();
                        if (historyMsgList.Any() == false)
                        {
                            return;
                        }

                        //2.0 在RouteMsgTable 中获取当前使用的表名称
                        CrmChat14Entities db = new CrmChat14Entities();
                        string currentTableName = db.RouteMsgTable.OrderBy(c => c.rid).ToList().LastOrDefault().TableName;
                        LogHelper.WriteInfoLog("当前表名称="+currentTableName);

                        //3.0 根据表名称获取此表中的数据总条数
                        int? tcount = db.Usp_GetTotalCount15(currentTableName).FirstOrDefault();
                        int totalcount = tcount.HasValue ? tcount.Value : 0;
                        LogHelper.WriteInfoLog("当前表中的记录总条数=" + totalcount);
                        //4.0 和web.config中的每个表的最大存储条数(存储过程来获取的)比对，如果大于最大条数，则创建新表
                        //5.0 if(满足大于最大条数)
                        if (totalcount >= maxnumer)
                        {
                            //5.0.1 创建一个新表，命名方式是在当前使用表的索引上加1，但前缀固定为 ChatMsg+index
                            string newTableNameLeft = "ChatMsg";
                            string index = currentTableName.Replace("ChatMsg", "");
                            int newIndex = int.Parse(index) + 1;
                            string newTableName = newTableNameLeft + newIndex; //新表名称
                            LogHelper.WriteInfoLog("新的表名称=" + newTableName);
                            //5.0.2 （注意使用事务）新建一个存储过程来创建一个物理表,同时向RouteMsgTable 新增一条记录，将RouteMsgTable中上一个表的记录中endTime结束
                            using (System.Transactions.TransactionScope scop = new System.Transactions.TransactionScope())
                            {
                                db.Usp_CreateTable15(newTableName, historyMsgList.FirstOrDefault().SendTime);

                                scop.Complete();
                            }

                            //5.0.3调用sqlbulkcopy将historyMsgList批量插入当前表
                            BlukCopy(newTableName, historyMsgList);
                        }
                        else
                        {
                            //6.0 else {继续调用sqlbulkcopy将historyMsgList批量插入当前表}
                            BlukCopy(currentTableName, historyMsgList);
                        }

                        now = DateTime.Now;
                    }

                    System.Threading.Thread.Sleep(1000);
                }
                catch (Exception ex)
                {
                    //Log4Net日志记录
                    LogHelper.WriteErrorLog(ex.ToString());
                }
            }
        }

        /// <summary>
        /// 负责去redis中根据    string cacheKey = "History"; 获取历史消息数据集合
        /// </summary>
        /// <returns></returns>
        List<ChatMsg> GetHistoryMsgList()
        {
            List<ChatMsg> list = new List<ChatMsg>();

            string cacheKey = "History";

            using (var client = RedisClientFactory.Instance.CreateRedisClient("127.0.0.1", 6379))
            {
                int count = client.GetListCount(cacheKey);
                for (int i = 0; i < count; i++)
                {
                    list.Add(Kits.Deserialize<ChatMsg>(client.DequeueItemFromList(cacheKey)));
                }
            }
            return list;
        }

        void BlukCopy(string tablename, List<ChatMsg> list)
        {
            //1.0 创建一个内存表
            DataTable dt = new DataTable();
            dt.Columns.Add("ToUserId", typeof(int));
            dt.Columns.Add("ToRealName", typeof(string));
            dt.Columns.Add("FromUserId", typeof(int));
            dt.Columns.Add("FromRealName", typeof(string));
            dt.Columns.Add("MessageBody", typeof(string));
            dt.Columns.Add("SendTime", typeof(DateTime));

            DataRow datarow;
            foreach (var item in list)
            {
                datarow = dt.NewRow();
                datarow["ToUserId"] = item.ToUserId.ToString();
                datarow["ToRealName"] = item.ToRealName;
                datarow["FromUserId"] = item.FromUserId.ToString();
                datarow["FromRealName"] = item.FromRealName;
                datarow["MessageBody"] = item.MessageBody;
                datarow["SendTime"] = item.SendTime.ToString();

                dt.Rows.Add(datarow);
            }

            string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["copyConn"].ConnectionString;
            using (System.Data.SqlClient.SqlBulkCopy copy = new System.Data.SqlClient.SqlBulkCopy(connStr))
            {
                copy.DestinationTableName = tablename;
                copy.ColumnMappings.Add("ToUserId", "ToUserId");
                copy.ColumnMappings.Add("ToRealName", "ToRealName");
                copy.ColumnMappings.Add("FromUserId", "FromUserId");
                copy.ColumnMappings.Add("FromRealName", "FromRealName");
                copy.ColumnMappings.Add("MessageBody", "MessageBody");
                copy.ColumnMappings.Add("SendTime", "SendTime");
                copy.WriteToServer(dt);
            }
        }
    }
}
