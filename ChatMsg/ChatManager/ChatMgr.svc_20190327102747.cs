using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace ChatManager
{
    using Chat.Lib;
    using Chat.Logger;
    using Chat.Model;

    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“ChatMgr”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 ChatMgr.svc 或 ChatMgr.svc.cs，然后开始调试。
    public class ChatMgr : IChatMgr
    {
        public void SetMessage(Chat.Model.ChatMsg msg)
        {
            try
            {
                //1.0 将msg实体保存在实时聊天队列中
                MessageMgr.Instance.SetMessage(msg);

                //2.0 将msg实体保存在历史消息队列中
                HistoryMgr.Instance.SetMessage(msg);
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorLog(ex.ToString());
            }
        }

        public List<Chat.Model.ChatMsg> GetMessage(int userid)
        {
            try
            {
                return MessageMgr.Instance.GetMessage(userid);
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorLog(ex.ToString());

                return new List<Chat.Model.ChatMsg>();
            }
        }
        
        /// <summary>
        /// 负责获取指定用户在指定时间段中的历史记录
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="begintime"></param>
        /// <param name="endtime"></param>
        /// <returns></returns>
        public List<Chat.Model.Usp_GetHistroryMsg15_Result> GetHistoryMessage(int userid, DateTime begintime, DateTime endtime)
        {
            CrmChat14Entities db = new CrmChat14Entities();
            return db.Usp_GetHistroryMsg15(userid, begintime, endtime).ToList();
        }
    }
}
