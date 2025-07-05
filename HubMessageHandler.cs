using BaseServer.Core;
using BaseServer.Interface;
using Common.Logging;
using ESBasic.Collections;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataHub.Core
{
    /// <summary>
    /// 消息处理器
    /// </summary>
    public class HubMessageHandler : IMessageHandler
    {
        protected ILog loger;
        /// <summary>
        /// 日志类
        /// </summary>
        public ILog Loger
        {
            get { return loger; }
            set { loger = value; }
        }

        private bool asynchronisMode;
        /// <summary>
        /// 是否同步
        /// </summary>
        public bool AsynchronisMode
        {
            set { asynchronisMode = value; }
        }

        private ConcurrentDictionary<string, IClientSession> sessionDictionary = new ConcurrentDictionary<string, IClientSession>();

        public HubMessageHandler()
        {
            loger = LogManager.GetLogger("DataHubServiceExceptionLoger");
        }
        /// <summary>
        /// 发送消息给所有玩家
        /// </summary>
        /// <typeparam name="TBody"></typeparam>
        /// <param name="contractType"></param>
        /// <param name="bodyContract"></param>
        public void SendMessageToAll<TBody>(int contractType, TBody bodyContract) where TBody : class
        {
            Parallel.ForEach(sessionDictionary, session =>
            {
                session.Value.SendMessage<TBody>(contractType, bodyContract);
            });
        }

        /// <summary>
        /// 发送消息给某些玩家
        /// </summary>
        /// <typeparam name="TBody"></typeparam>
        /// <param name="contractType"></param>
        /// <param name="bodyContract"></param>
        /// <param name="userIDArray"></param>
        public void SendMessage<TBody>(int contractType, TBody bodyContract, params string[] playerIDArray) where TBody : class
        {
            try
            {
                IList<string> playerIDList = CollectionConverter.ConvertArrayToList<string>(playerIDArray);
                this.SendMessage<TBody>(contractType, bodyContract, playerIDList);
            }
            catch (Exception ex)
            {
                loger.Error(ex);
            }
        }

        /// <summary>
        /// 发送消息给某些玩家
        /// </summary>
        /// <typeparam name="TBody"></typeparam>
        /// <param name="contractType"></param>
        /// <param name="bodyContract"></param>
        /// <param name="userIDList"></param>
        public void SendMessage<TBody>(int contractType, TBody bodyContract, IList<string> playerIDList) where TBody : class
        {
            try
            {
                foreach (string playerID in playerIDList)
                {
                    IClientSession session;
                    this.sessionDictionary.TryGetValue(playerID, out session);
                    if (session != null)
                    {
                        session.SendMessage<TBody>(contractType, bodyContract);
                    }
                    else
                    {
                        loger.Info("Session is null, playerID:" + playerID);
                    }
                }
            }
            catch (Exception ex)
            {
                loger.Error(ex);
            }
        }

        /// <summary>
        /// 添加玩家会话
        /// </summary>
        /// <param name="playerID"></param>
        /// <param name="session"></param>
        internal void TryAdd(string playerId, IClientSession session)
        {
            sessionDictionary.TryAdd(playerId, session);
        }

        /// <summary>
        /// 移除玩家会话
        /// </summary>
        /// <param name="playerID"></param>
        internal void TryRemove(string playerId)
        {
            IClientSession session;
            sessionDictionary.TryRemove(playerId, out session);
            if (session != null)
            {
                session.PlayerID = null;
            }
        }

        public void KickUser(params string[] userIDArray)
        {
            try
            {
                foreach (string playerID in userIDArray)
                {
                    IClientSession session;
                    this.sessionDictionary.TryGetValue(playerID, out session);
                    if (session != null)
                    {
                        session.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                loger.Error(ex);
            }
        }

        public void SendMessageToAllSP(int contractType, Sproto.SprotoTypeBase bodyContract)
        {
            throw new NotImplementedException();
        }

        public void SendMassageSP(int contreactType, Sproto.SprotoTypeBase bodyContract, params string[] userIDArray)
        {
            throw new NotImplementedException();
        }

        public void SendMessageSP(int contractType, Sproto.SprotoTypeBase bodyContract, IList<string> userIDList)
        {
            throw new NotImplementedException();
        }

        public void SendMessageToAllNoDeskUsersSP(int contractType, Sproto.SprotoTypeBase bodyContract)
        {
            throw new NotImplementedException();
        }

        public void SendMessageToAllDeskUsersSP(IDesk myDesk, int contractType, Sproto.SprotoTypeBase bodyContract)
        {
            throw new NotImplementedException();
        }

        public void SendMessageToPatternsSP(string myUserID, IDesk myDesk, int contractType, Sproto.SprotoTypeBase bodyContract)
        {
            throw new NotImplementedException();
        }

        public void SendMessageToAllNoDeskUsers<TBody>(int contractType, TBody bodyContract) where TBody : class
        {
            throw new NotImplementedException();
        }

        public void SendMessageToAllDeskUsers<TBody>(IDesk myDesk, int contractType, TBody bodyContract) where TBody : class
        {
            throw new NotImplementedException();
        }

        public void SendMessageToPatterns<TBody>(string myUserID, IDesk myDesk, int contractType, TBody bodyContract) where TBody : class
        {
            throw new NotImplementedException();
        }

        public bool IsHaveUser(string playerID)
        {
            throw new NotImplementedException();
        }
    }
}
