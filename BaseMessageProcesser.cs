using BaseChessCard.Core;
using Common.Logging;
using DataHub.Contract;
using DataHub.Core.Interface;
using System;
using System.Collections.Generic;

namespace DataHub.Core
{
    /// <summary>
    /// 消息处理器基类
    /// </summary>
    public abstract class BaseMessageProcesser : IOfflineHandler
    {
        protected ILog loger;
        public ILog Loger
        {
            get { return loger; }
            set { loger = value; }
        }

        /// <summary>
        /// 命令字典
        /// </summary>
        protected IDictionary<int, IGameCommand> GameCommandDictionary = new Dictionary<int, IGameCommand>();


        protected IMessageHandler messageHandler;
        /// <summary>
        /// 消息处理器
        /// </summary>
        public IMessageHandler MessageHandler
        {
            get { return messageHandler; }
            set { messageHandler = value; }
        }

        protected BaseOfflineHandler offlineHandler;
        public BaseOfflineHandler OfflineHandler
        {
            get { return offlineHandler; }
            set { offlineHandler = value; }
        }


        protected BaseMessageProcesser()
        {
            this.LoadCommand();
            this.Initialize();
        }


        /// <summary>
        /// 处理掉线的玩家
        /// </summary>
        /// <param name="userID"></param>
        public void HandlePlayerOffline(string userID)
        {
            if (string.IsNullOrEmpty(userID))
            {
                return;
            }

            this.offlineHandler.HandlePlayerOffline(userID);

        }

        public abstract void Initialize();

        protected abstract void LoadCommand();

        /// <summary>
        /// 处理消息
        /// </summary>
        /// <param name="session"></param>
        /// <param name="messageHead"></param>
        /// <param name="body"></param>
        internal void ProcessMessage(IPlayerSession session, IMessageHead messageHead, byte[] body)
        {
            //loger.Info(String.Format(" 收到 contractType => {0}", messageHead.ContractType));
            if (messageHead.ContractType != DataHubContractTypes.HeartBeat && !GameCommandDictionary.ContainsKey(messageHead.ContractType))
            {
                loger.Error(String.Format(" 不存在该消息类型 contractType => {0}", messageHead.ContractType));
                LogManager.GetLogger<BaseMessageProcesser>().Info(string.Format("ProcessMessage()playerID:{0}", session.PlayerID));
                session.Close();
                return;
            }

            if (messageHead.ContractType == DataHubContractTypes.Login)
            {
                //优先处理登录
                (GameCommandDictionary[messageHead.ContractType] as BaseLogin).ProcessMessage(session, body);
                return;
            }

            if (messageHead.ContractType == DataHubContractTypes.HeartBeat)
            {
                session.SendMessage(DataHubContractTypes.HeartBeat, new NullBodyContract());
                return;
            }

            //当签名不通过时，直接拦截处理
            if (false)
            {
                //返回空 此时引擎将会主动断掉该链接Session
            }

            if (string.IsNullOrEmpty(session.PlayerID))
            {
                loger.Error(String.Format("session.UserID 为空 消息类型 contractType => {0}", messageHead.ContractType));
                LogManager.GetLogger<BaseMessageProcesser>().Info(string.Format("ProcessMessage()playerID:{0}", session.PlayerID));
                session.Close();
                return;
            }
            if (!GameCommandDictionary.ContainsKey(messageHead.ContractType))
            {
                return;
            }
            GameCommandDictionary[messageHead.ContractType].ProcessMessage(session.PlayerID, body);
        }
    }
}
