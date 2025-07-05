using BaseChessCard.Core;
using Common.Logging;
using DataHub.Core.Interface;

namespace DataHub.Core
{
    /// <summary>
    /// 协议命令基类
    /// </summary>
    public abstract class BaseCommand : IGameCommand
    {
        protected IMessageHandler messageHandler;
        public IMessageHandler MessageHandler { set { messageHandler = value; } }

        protected ILog loger;
        /// <summary>
        /// 日志类
        /// </summary>
        public ILog Loger
        {
            get { return loger; }
            set { loger = value; }
        }
        protected BaseCommand()
        { }

        /// <summary>
        /// 处理玩家消息
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="body"></param>
        public abstract void ProcessMessage(string userID, byte[] body);

    }
}
