using BaseServer.Core;
using BaseServer.Interface;
using BaseServer.Kernel;
using BaseServer.Platform;
using BaseServer.Util;
using Common.Logging;
using DataHub.Game.DTO;
using DataHub.Game.IService;
using GatewayServer.CMD;
using GatewayServer.GameServer;
using SprotoType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatewayServer
{
    public class MessageProcesser : BaseMessageProcesser
    {
        ServerClientProcesser gameServer = new ServerClientProcesser();

        public override void Initialize()
        {
            Loger = LogManager.GetLogger("GatewayServer");

            ConnectPlatormWCF();

            InitGameServer();
        }

        private bool ConnectPlatormWCF()
        {
            //连接db服的WCF服务
            DataHubService.Initializer();

            return true;
        }

        public void Stop()
        {
            // DataHubServer
            //if (!SettingGame.GetInstance().TestNoDataHub)
            //{
            //    GDGameServerStopDTO dto = new GDGameServerStopDTO
            //    {
            //        ServerID = gameSetting.ServerID,
            //        GameID = gameSetting.GameID,
            //        AgentID = gameSetting.AgentID
            //    };
            //    DataHubService.CallService<IGameService>(c => c.GameServerStop(dto));
            //}

            //GameRoom thisGameRoom = (gameRoom as GameRoom);
            //if (thisGameRoom != null)
            //    thisGameRoom.Stop();
        }

        private bool InitGameServer()
        {
            try
            {
                //List<GDGameServerInfoDTO> serverInfos = DataHubService.CallService<IGameService, GDGameServerInfoDTO>(c => c.GameServerInfo(null));

                //foreach (GDGameServerInfoDTO info in serverInfos)
                //{
                //    gameServer.CreateConnect(info.ServerIP, info.ServerPort, info.ServerID);
                //}
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
            }

            return false;
        }

        protected override void LoadGameCommand()
        {
        }

        public override void ProcessMessage(IClientSession session, IMessageHead messageHead, byte[] body)
        {
            try
            {
                byte[] data = SprotoBufCodec.Encode(messageHead, body, true);

                switch (messageHead.ContractType)
                {
                    case Protocol.LoginHall.Tag:
                        session.PlayerID = messageHead.PlayerID;
                        string[] connectInfo = PlatformGlobalData.Instance.PlatformIP.Split(':');
                        gameServer.CreateConnect(connectInfo[0], Convert.ToInt32(connectInfo[1]), -1, session.PlayerID, data, session, 0);
                        break;
                    case Protocol.LoginGame.Tag:
                        session.PlayerID = messageHead.PlayerID;

                        //(this.messageHandler as MessageHandler).TryRemove(messageHead.PlayerID);
                        //(this.messageHandler as MessageHandler).TryAdd(messageHead.PlayerID, session);

                        GDGameServerStartDTO dto = new GDGameServerStartDTO
                        {
                            ServerID = messageHead.ServerID,
                        };

                        GDGameServerInfoDTO info = DataHubService.CallService<IGameService, GDGameServerInfoDTO>(c => c.GameServerInfo(dto));
                        session.ServerID = messageHead.ServerID.ToString();
                        gameServer.CreateConnect(info.ServerIP, info.ServerPort, info.ServerID, messageHead.PlayerID, data, session, 1);
                        break;
                    case Protocol.ExitGame.Tag:
                        gameServer.RemoveGameServerClient(messageHead.PlayerID);
                        break;
                    case Protocol.heardbeat.Tag:
                        gameServer.SendHeardbeatToServer(messageHead.PlayerID, data);
                        break;
                    default:
                        gameServer.SendMessageToServer(session.PlayerID, data, messageHead.ServerID);
                        break;
                }
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
            }
        }

        public override void HandlePlayerOffline(IClientSession session)
        {
            try
            {
                gameServer.RemoveServerClient(session);
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
            }

        }
    }
}
