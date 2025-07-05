using DataHub.Core.Interface;

namespace DataHub.Core
{
    /// <summary>
    /// 消息处理器
    /// </summary>
    public sealed class MessageProcesserFacade
    {
        private BaseMessageProcesser messageProcesser;

        public MessageProcesserFacade()
        {
        }

        /// <summary>
        /// 处理消息
        /// </summary>
        /// <param name="session"></param>
        /// <param name="messageHead"></param>
        /// <param name="body"></param>
        public void ProcessMessage(IPlayerSession session, IMessageHead messageHead, byte[] body)
        {
            messageProcesser.ProcessMessage(session, messageHead, body);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="messageProcesser"></param>
        public void Initialize(BaseMessageProcesser messageProcesser)
        {
            this.messageProcesser = messageProcesser;
        }

        /// <summary>
        /// 处理玩家掉线
        /// </summary>
        /// <param name="playerID"></param>
        public void HandlePlayerOffline(string playerID)
        {
            messageProcesser.HandlePlayerOffline(playerID);
        }
    }
}
