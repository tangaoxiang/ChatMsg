using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chat.Logger
{
    public class LogHelper
    {
        public static readonly log4net.ILog logerror = log4net.LogManager.GetLogger("logerror");   //选择<logger name="logerror">的配置 

        static LogHelper()
        {
            SetConfig();
        }

        /// <summary>
        /// 默认配置。按配置文件
        /// </summary>
        public static void SetConfig()
        {
            log4net.Config.XmlConfigurator.Configure();
        }

        /// <summary>
        /// 写日志。出错时会写入
        /// </summary>
        /// <param name="info"></param>
        /// <param name="se"></param>
        public static void WriteErrorLog(string info, Exception se)
        {
            if (logerror.IsErrorEnabled)
            {
                logerror.Error(info, se);
            }
        }

        public static void WriteErrorLog(string info)
        {
            if (logerror.IsErrorEnabled)
            {
                logerror.Error(info);
            }
        }

        /// <summary>
        /// 记录信息日志
        /// </summary>
        /// <param name="info"></param>
        public static void WriteInfoLog(string info)
        {
            if (logerror.IsErrorEnabled)
            {
                logerror.Info(info);
            }
        }

        /// <summary>
        /// 记录警告日志
        /// </summary>
        /// <param name="info"></param>
        public static void WriteWarnLog(string info)
        {
            if (logerror.IsErrorEnabled)
            {
                logerror.Warn(info);
            }
        }
    }
}