using BaseServer.Core;
using BaseServer.Interface;
using BaseServer.Util;
using Common.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wisher.Basic.Log;

namespace GatewayServer.GameServer
{
    public class GameServerProcesser
    {
        private ILog loger = LogManager.GetLogger("GameServerProcesser");

        private ConcurrentDictionary<string, BaseClientConnect> clients = new ConcurrentDictionary<string, BaseClientConnect>();

        public bool CreateConnect(string serverIP, int serverPort, int serverID, string playerID, byte[] data, IPlayerSession session)
        {
            try
            {
                if (clients.ContainsKey(playerID))
                {
                    session.Close();
                    MyLog.Info(typeof(GameServerProcesser), string.Format("isConnected playerID:{0}", playerID));
                }
                else
                {
                    GameServerClientConnect clientConnect = new GameServerClientConnect(data, playerID, session);

                    clientConnect.EventConnected += ClientConnected;
                    clientConnect.Connect(serverIP, serverPort);

                    clients.TryAdd(playerID, clientConnect);
                }

                return true;
            }
            catch (Exception ex)
            {
                loger.Error(typeof(GameServerProcesser), ex);
            }

            return false;
        }

        private void ClientConnected()
        {
            MyLog.Trace(typeof(GameServerProcesser), string.Format("PlatformConnent_Connected()"));
        }

        public void SendMessageToServer(string playerID, byte[] data)
        {
            BaseClientConnect gameServerClient;

            clients.TryGetValue(playerID, out gameServerClient);

            if (gameServerClient != null)
            {
                gameServerClient.SendMeassage(data);
            }
            else
            {
                MyLog.Error(typeof(GameServerProcesser), "The GameServerClient is null playerID: " + playerID);
            }
        }

        public void RemoveGameServerClient(string playerID)
        {
            if (string.IsNullOrEmpty(playerID))
            {
                MyLog.Info(typeof(GameServerClientConnect), "Player Offline RemoveGameServerClient: is null");
            }

            MyLog.Info(typeof(GameServerClientConnect), string.Format("Player Offline RemoveGameServerClient: {0}", playerID));

            if (clients.ContainsKey(playerID))
            {
                BaseClientConnect gameServerClient;
                clients.TryRemove(playerID, out gameServerClient);
                gameServerClient.Close();
                gameServerClient = null;
            }
        }
    }
}
