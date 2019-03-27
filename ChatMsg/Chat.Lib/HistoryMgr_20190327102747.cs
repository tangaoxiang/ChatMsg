using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Lib
{
    public class HistoryMgr
    {
        #region 1.0 单例类
        private static readonly HistoryMgr _instance;

        static HistoryMgr()
        {
            _instance = new HistoryMgr();
        }

        private HistoryMgr() { }

        public static HistoryMgr Instance
        {
            get
            {
                return _instance;
            }
        }
        #endregion

        #region 2.0 业务逻辑

        private static object syncHelper = new object();


        public void SetMessage(Chat.Model.ChatMsg msg)
        {
            string cacheKey = "History";
            using (var client = RedisClientFactory.Instance.CreateRedisClient("127.0.0.1", 6379))
            {
                lock (syncHelper)
                {
                    //入队成功
                    client.EnqueueItemOnList(cacheKey, Kits.ToJsonString(msg));
                }
            }
        }

        /// <summary>
        /// 给子线程每隔一段时间进行请求后，将数据保存到db中
        /// </summary>
        /// <returns></returns>
        public List<Chat.Model.ChatMsg> GetMessage()
        {
            List<Chat.Model.ChatMsg> list = new List<Model.ChatMsg>();

            string cacheKey = "History";
            using (var client = RedisClientFactory.Instance.CreateRedisClient("127.0.0.1", 6379))
            {
                lock (syncHelper)
                {
                    int count = client.GetListCount(cacheKey);
                    //出队成功
                    for (int i = 0; i < count; i++)
                    {
                        var chatmsg = Kits.Deserialize<Chat.Model.ChatMsg>(client.DequeueItemFromList(cacheKey));
                        list.Add(chatmsg);
                    }
                }
            }

            return list;
        }

        #endregion
    }
}
