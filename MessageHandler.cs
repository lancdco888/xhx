using BaseServer.BaseContract;
using BaseServer.Interface;
using BaseServer.Room;
using Common.Logging;
using ESBasic.Collections;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BaseServer.Core
{
    /// <summary>
    /// 消息处理器
    /// </summary>
    public class MessageHandler : IMessageHandler
    {
        protected ILog loger;

        private IGameRoom gameRoom;
        /// <summary>
        /// 游戏房间
        /// </summary>
        public IGameRoom GameRoom
        {
            get { return gameRoom; }
        }

        public MessageHandler(IGameRoom gameRoom)
        {
            loger = LogManager.GetLogger("GameLogic");
            this.gameRoom = gameRoom;
        }

        private ConcurrentDictionary<string, IClientSession> sessionDictionary = new ConcurrentDictionary<string, IClientSession>();

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
        /// 发送消息给所有未进桌玩家
        /// </summary>
        /// <typeparam name="TBody"></typeparam>
        /// <param name="contractType"></param>
        /// <param name="bodyContract"></param>
        public void SendMessageToAllNoDeskUsers<TBody>(int contractType, TBody bodyContract) where TBody : class
        {
            Parallel.ForEach(sessionDictionary, session =>
            {
                if (gameRoom.GetPlayerDesk(session.Key) == null)
                {
                    session.Value.SendMessage<TBody>(contractType, bodyContract);
                }
            }); ;
        }

        /// <summary>
        /// 发送消息给同桌所有玩家
        /// </summary>
        /// <typeparam name="TBody"></typeparam>
        /// <param name="myDesk"></param>
        /// <param name="contractType"></param>
        /// <param name="bodyContract"></param>
        public void SendMessageToAllDeskUsers<TBody>(IDesk myDesk, int contractType, TBody bodyContract) where TBody : class
        {
            IList<string> userListCopy = myDesk.GetPlayerListCopy();
            this.SendMessage<TBody>(contractType, bodyContract, userListCopy);
        }

        /// <summary>
        /// 发送消息给同桌
        /// </summary>
        /// <typeparam name="TBody"></typeparam>
        /// <param name="myUserID"></param>
        /// <param name="myDesk"></param>
        /// <param name="contractType"></param>
        /// <param name="bodyContract"></param>
        public void SendMessageToPatterns<TBody>(string myPlyerID, Interface.IDesk myDesk, int contractType, TBody bodyContract) where TBody : class
        {
            IList<string> playerListCopy = myDesk.GetPlayerListCopy();

            if (playerListCopy.Contains(myPlyerID))
            {
                playerListCopy.Remove(myPlyerID);
            }

            this.SendMessage<TBody>(contractType, bodyContract, playerListCopy);
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
                }
                //this.SendMessageToAIPlayer<TBody>(contractType, bodyContract, playerIDList);
            }
            catch (Exception ex)
            {
                loger.Error(ex);
            }
        }

        /// <summary>
        /// 发送消息给AI玩家
        /// </summary>
        /// <typeparam name="TBody"></typeparam>
        /// <param name="contractType"></param>
        /// <param name="bodyContract"></param>
        /// <param name="userIDList"></param>
        private void SendMessageToAIPlayer<TBody>(int contractType, TBody bodyContract, IList<string> playerIDList) where TBody : class
        {
            try
            {
                foreach (string playerID in playerIDList)
                {
                    IAIPlayer player = this.gameRoom.GetPlayer(playerID) as IAIPlayer;
                    if (player != null)
                    {
                        player.ProcessMessage<TBody>(new AIMessage<TBody>(contractType, bodyContract));
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
        public void TryAdd(string playerId, IClientSession session)
        {
            sessionDictionary.TryAdd(playerId, session);
        }

        /// <summary>
        /// 移除玩家会话
        /// </summary>
        /// <param name="playerID"></param>
        public void TryRemove(string playerId)
        {
            IClientSession session;
            sessionDictionary.TryRemove(playerId, out session);
            session = null;
        }

        /// <summary>
        /// 账号被挤走
        /// </summary>
        /// <param name="playerId"></param>
        internal void CrowdingOut(string playerId)
        {
            IClientSession session;
            sessionDictionary.TryRemove(playerId, out session);
            if (session != null)
            {
                session.SendMessageSP(SprotoType.Protocol.kickout.Tag, new SprotoType.kickout.response());
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

        public void LoginSucceed(string playerId, IClientSession loginSession)
        {
            IClientSession session;
            sessionDictionary.TryGetValue(playerId, out session);
            if (session != null)
            {
                if (session != loginSession)
                {
                    sessionDictionary.TryRemove(playerId, out session);
                    session.SendMessageSP(SprotoType.Protocol.kickout.Tag, new SprotoType.kickout.response());
                    session.Close();
                    session.PlayerID = null;
                    sessionDictionary.TryAdd(playerId, loginSession);
                }
            }
            else
            {
                sessionDictionary.TryAdd(playerId, loginSession);
            }
        }

        /// <summary>
        /// 判断玩家是否存在
        /// </summary>
        /// <param name="playerID"></param>
        /// <returns></returns>
        public bool IsHaveUser(string playerID)
        {
            return sessionDictionary.ContainsKey(playerID);
        }

        public void SendMessageToAllSP(int contractType, Sproto.SprotoTypeBase bodyContract)
        {
            try
            {
                Parallel.ForEach(sessionDictionary, session =>
                            {
                                session.Value.SendMessageSP(contractType, bodyContract);
                            });
            }
            catch (Exception ex)
            {
                loger.Error(ex);
            }

        }

        public void SendMassageSP(int contractType, Sproto.SprotoTypeBase bodyContract, params string[] userIDArray)
        {
            try
            {
                IList<string> playerIDList = CollectionConverter.ConvertArrayToList<string>(userIDArray);
                this.SendMessageSP(contractType, bodyContract, playerIDList);
            }
            catch (Exception ex)
            {
                loger.Error(ex);
            }
        }

        public void SendMessageSP(int contractType, Sproto.SprotoTypeBase bodyContract, IList<string> userIDList)
        {
            try
            {
                foreach (string playerID in userIDList)
                {
                    IClientSession session;
                    this.sessionDictionary.TryGetValue(playerID, out session);
                    if (session != null)
                    {
                        session.SendMessageSP(contractType, bodyContract);
                    }
                }
                //this.SendMessageToAIPlayer<TBody>(contractType, bodyContract, playerIDList);
            }
            catch (Exception ex)
            {
                loger.Error(ex);
            }
        }

        public void SendMessageToAllNoDeskUsersSP(int contractType, Sproto.SprotoTypeBase bodyContract)
        {
            try
            {
                Parallel.ForEach(sessionDictionary, session =>
                            {
                                if (gameRoom.GetPlayerDesk(session.Key) == null)
                                {
                                    session.Value.SendMessageSP(contractType, bodyContract);
                                }
                            });
            }
            catch (Exception ex)
            {
                loger.Error(ex);
            }

        }

        public void SendMessageToAllDeskUsersSP(IDesk myDesk, int contractType, Sproto.SprotoTypeBase bodyContract)
        {
            try
            {
                IList<string> userListCopy = myDesk.GetPlayerListCopy();
                this.SendMessageSP(contractType, bodyContract, userListCopy);
            }
            catch (Exception ex)
            {
                loger.Error(ex);
            }
        }

        public void SendMessageToPatternsSP(string myUserID, IDesk myDesk, int contractType, Sproto.SprotoTypeBase bodyContract)
        {
            try
            {
                IList<string> playerListCopy = myDesk.GetPlayerListCopy();

                if (playerListCopy.Contains(myUserID))
                {
                    playerListCopy.Remove(myUserID);
                }

                this.SendMessageSP(contractType, bodyContract, playerListCopy);
            }
            catch (Exception ex)
            {
                loger.Error(ex);
            }
        }
    }
}
