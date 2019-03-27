using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace ChatManager
{
    using Chat.Model;

    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“IChatMgr”。
    [ServiceContract]
    public interface IChatMgr
    {
        /// <summary>
        /// 负责接收客户端发送过来的消息后马上响应回去
        /// </summary>
        /// <param name="msg"></param>
        [OperationContract(IsOneWay=true)]
        void SetMessage(ChatMsg msg);

        /// <summary>
        /// 根据用户id获取此用户的消息记录返回
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        [OperationContract]
        List<ChatMsg> GetMessage(int userid);

        [OperationContract]
        List<Usp_GetHistroryMsg15_Result> GetHistoryMessage(int userid, DateTime begintime, DateTime endtime);
    }
}
