using BaseChessCard.Core;
using DataHub.Core.Interface;
using ESBasic.Threading.Timers;

namespace DataHub.Core
{
    /// <summary>
    /// 掉线处理器基类
    /// </summary>
    public abstract class BaseOfflineHandler : IOfflineHandler
    {

        protected ICallbackTimer<string> notifyTimer;
        public ICallbackTimer<string> NotifyTimer { set { notifyTimer = value; } }

        protected IMessageHandler messageHandler;
        public IMessageHandler MessageHandler { set { messageHandler = value; } }

        protected BaseOfflineHandler() { }


        /// <summary>
        /// 处理断线
        /// </summary>
        /// <param name="userID"></param>
        protected abstract void HandleOffline(string userID);

        /// <summary>
        /// 处理玩家掉线
        /// </summary>
        /// <param name="playerID"></param>
        public void HandlePlayerOffline(string playerID)
        {
            this.HandleOffline(playerID);
            (messageHandler as MessageHandler).TryRemove(playerID);
        }
    }
}
