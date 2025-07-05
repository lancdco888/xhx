using Common.Logging;
using ESBasic.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseServer.Core
{
    /// <summary>
    /// 处理基类
    /// </summary>
    public abstract class BaseHandler
    {
        protected ILog loger;
        /// <summary>
        /// 日志实例
        /// </summary>
        public ILog Loger
        {
            set { loger = value; }
        }

        protected IMessageHandler messageHandler;
        /// <summary>
        /// 消息处理类
        /// </summary>
        public IMessageHandler MessageHandler
        {
            set { messageHandler = value; }
        }

        protected BaseHandler()
        {

        }
    }
}