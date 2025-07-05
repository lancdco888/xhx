using BaseServer.Core;
using BaseServer.Interface;
using BaseServer.Util;
using Common.Logging;
using SprotoType;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wisher.Basic.Log;

namespace GatewayServer.GameServer
{
    public class ServerClientProcesser
    {
        private ILog loger = LogManager.GetLogger("GameServerProcesser");

        private ConcurrentDictionary<string, BaseClientConnect> gameServerClients = new ConcurrentDictionary<string, BaseClientConnect>();

        private ConcurrentDictionary<string, BaseClientConnect> centerServerClients = new ConcurrentDictionary<string, BaseClientConnect>();

        public bool CreateConnect(string serverIP, int serverPort, int serverID, string playerID, byte[] data, IClientSession session, byte clientType)
        {
            try
            {
                Create(serverIP, serverPort, serverID, playerID, data, session, clientType);

                return true;
            }
            catch (Exception ex)
            {
                loger.Error(typeof(ServerClientProcesser), ex);
            }

            return false;
        }

        private void Create(string serverIP, int serverPort, int serverID, string playerID, byte[] data, IClientSession session, byte clientType)
        {
            ConcurrentDictionary<string, BaseClientConnect> connectDic;

            if (clientType == 0)
            {
                connectDic = centerServerClients;
            }
            else
            {
                connectDic = gameServerClients;
            }

            if (connectDic.ContainsKey(playerID))
            {
                BaseClientConnect connect;
                connectDic.TryRemove(playerID, out connect);
                connect.Close();
                //(connect as ServerClientConnect).Session.Close();
                //session.Close();
                MyLog.Info(typeof(ServerClientProcesser), string.Format("isConnected playerID:{0}", playerID));
            }

            ServerClientConnect clientConnect = new ServerClientConnect(data, playerID, session, clientType);

            //clientConnect.EventConnected += ClientConnected;

            clientConnect.Connect(serverIP, serverPort);

            connectDic.TryAdd(playerID, clientConnect);

        }

        private void ClientConnected()
        {
            MyLog.Trace(typeof(ServerClientProcesser), string.Format("PlatformConnent_Connected()"));
        }

        public void SendMessageToServer(string playerID, byte[] data, int serverID)
        {
            BaseClientConnect serverClient;

            ///发送给大厅的包
            if (serverID == 0)
            {
                centerServerClients.TryGetValue(playerID, out serverClient);
                if (serverClient != null)
                {
                    serverClient.SendMessage(data);
                }
                else
                {
                    MyLog.Error(typeof(ServerClientProcesser), "The CenterServerClients is null playerID: " + playerID);
                }

            }
            else
            {
                gameServerClients.TryGetValue(playerID, out serverClient);

                if (serverClient != null)
                {
                    serverClient.SendMessage(data);
                }
                else
                {
                    MyLog.Error(typeof(ServerClientProcesser), "The GameServerClient is null playerID: " + playerID);
                }
            }
        }

        public void SendHeardbeatToServer(string playerID, byte[] data)
        {
            BaseClientConnect serverClient;

            centerServerClients.TryGetValue(playerID, out serverClient);

            if (serverClient != null)
            {
                serverClient.SendMessage(data);
            }

            serverClient = null;

            gameServerClients.TryGetValue(playerID, out serverClient);

            if (serverClient != null)
            {
                serverClient.SendMessage(data);
            }
        }

        public void RemoveGameServerClient(string playerID)
        {
            if (string.IsNullOrEmpty(playerID))
            {
                MyLog.Info(typeof(ServerClientProcesser), "Player Offline RemoveGameServerClient: is null");
            }
            else
            {
                MyLog.Info(typeof(ServerClientProcesser), string.Format("Player Offline RemoveGameServerClient: {0}", playerID));

                if (gameServerClients.ContainsKey(playerID))
                {
                    BaseClientConnect serverClient;
                    gameServerClients.TryRemove(playerID, out serverClient);
                    (serverClient as ServerClientConnect).SendToClient(Protocol.ExitGame.Tag, new ExitGame.response());
                    serverClient.Close();
                    serverClient = null;
                }
            }

        }

        public void RemoveServerClient(IClientSession session)
        {
            if (string.IsNullOrEmpty(session.PlayerID))
            {
                MyLog.Info(typeof(ServerClientProcesser), "Player Offline RemoveServerClient: is null");
            }
            else
            {
                MyLog.Info(typeof(ServerClientProcesser), string.Format("Player Offline RemoveServerClient: {0}", session.PlayerID));

                if (centerServerClients.ContainsKey(session.PlayerID))
                {
                    BaseClientConnect serverClient;
                    centerServerClients.TryRemove(session.PlayerID, out serverClient);

                    if (session == (serverClient as ServerClientConnect).Session)
                    {
                        serverClient.Close();
                        serverClient = null;
                    }
                }

                if (gameServerClients.ContainsKey(session.PlayerID))
                {
                    BaseClientConnect serverClient;
                    gameServerClients.TryRemove(session.PlayerID, out serverClient);

                    if (session == (serverClient as ServerClientConnect).Session)
                    {
                        serverClient.Close();
                        serverClient = null;
                    }
                }
            }
        }
    }
}
