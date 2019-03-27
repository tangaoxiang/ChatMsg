using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Lib
{
    using Chat.Model;
    using ServiceStack.Redis;

    /// <summary>
    /// 负责管理实时聊天消息,单例类
    /// </summary>
    public class MessageMgr
    {
        #region 1.0 单例类的写法

        private static readonly MessageMgr _instance;
        static MessageMgr()
        {
            _instance = new MessageMgr();
            InitIp();
            InitPort();
        }

        private MessageMgr() { }

        public static MessageMgr Instance
        {
            get
            {
                return _instance;
            }
        }

        #endregion

        private static string redisIp;
        private static int redisPort;

        #region 2.0 初始化方法

        private static void InitIp()
        {
            redisIp = System.Configuration.ConfigurationManager.AppSettings["redisIp"];
            if (string.IsNullOrEmpty(redisIp))
            {
                redisIp = "127.0.0.1";
            }
        }

        private static void InitPort()
        {
            string redisPortStr = System.Configuration.ConfigurationManager.AppSettings["redisPort"];
            if (int.TryParse(redisPortStr, out redisPort) == false)
            {
                redisPort = 6379;
            }
        }

        #endregion

        private static object syncHelper = new object();

        #region 2.0 业务逻辑

        /// <summary>
        /// 保存消息到服务器队列中(Redis -> userinfo8->ChatMsg)
        /// </summary>
        /// <param name="msg"></param>
        public void SetMessage(ChatMsg msg)
        {
            string redisKey = "userinfo" + msg.ToUserId;
            //创建redis的客户端连接
            using (var client = RedisClientFactory.Instance.CreateRedisClient(redisIp, redisPort))
            {
                lock (syncHelper)
                {
                    //入队成功
                    client.EnqueueItemOnList(redisKey, Kits.ToJsonString(msg));
                }
            }
        }

        /// <summary>
        /// 获取消息
        /// </summary>
        /// <param name="userid"></param>
        public List<ChatMsg> GetMessage(int userid)
        {
            List<ChatMsg> list = new List<ChatMsg>();
            string redisKey = "userinfo" + userid;

            int count = 0;
            string jsonStr = "";
            using (var client = RedisClientFactory.Instance.CreateRedisClient(redisIp, redisPort))
            {
                lock (syncHelper)
                {
                    count = client.GetListCount(redisKey);
                    for (int i = 0; i < count; i++)
                    {
                        //出队一个元素并且将此元素删除
                        jsonStr = client.DequeueItemFromList(redisKey);
                        //将json字符串反序列化程ChatMsg
                        var chatmsg = Kits.Deserialize<ChatMsg>(jsonStr);
                        //将ChatMsg对象实例追加到list集合中
                        list.Add(chatmsg);
                    }
                }
            }

            return list;
        }

        #endregion
    }
}
