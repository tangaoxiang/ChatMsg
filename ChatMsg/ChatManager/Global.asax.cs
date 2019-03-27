using Chat.Lib;
using Chat.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace ChatManager
{
    public class Global : System.Web.HttpApplication
    {
        System.Threading.Thread th;
        /// <summary>
        /// 开启子线程用于日志信息的定时保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_Start(object sender, EventArgs e)
        {
            th = new System.Threading.Thread(() =>
            {
                //开启死循环，每隔固定的时间间隔后处理历史记录存储逻辑
                new BizProcessor().Process();
            });

            th.IsBackground = true;
            th.Start();

            LogHelper.WriteInfoLog("历史记录处理线程启动成功....");
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 将子线程关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_End(object sender, EventArgs e)
        {
            if (th != null && th.ThreadState == System.Threading.ThreadState.Running)
            {
                th.Abort();
            }
            LogHelper.WriteInfoLog("历史记录处理线程已经停止....");
        }
    }
}