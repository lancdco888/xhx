using BaseServer.Core;
using BaseServer.Kernel;
using Common.Logging;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wisher.Basic.Log;

namespace GatewayServer
{
    public class GatewayAppServer : AppServer<ClientSession, BaseRequestInfo>
    {
        private MessageProcesserFacade messageProcesserFacade;
        private MessageProcesser messageProcesser;
        // public SettingGame gameSetting = SettingGame.GetInstance();

        public GatewayAppServer()
            : base(new DefaultReceiveFilterFactory<AppReceiveFilterClient, BaseRequestInfo>())
        {
            try
            {
                //Logger.Info("服务器启动");
                this.messageProcesserFacade = new MessageProcesserFacade();
                //IApplicationContext applicationContext = ContextRegistry.GetContext();
                //PlatformNotifier platformNotifier = applicationContext.GetObject("platformNotifier") as PlatformNotifier;
                messageProcesser = new MessageProcesser();
                this.messageProcesserFacade.Initialize(messageProcesser);
            }
            catch (Exception ex)
            {
                MyLog.Error(typeof(GatewayAppServer), ex.Message);
            }
        }

        // override AppServer
        public override bool Start()
        {
            LogManager.GetLogger<GatewayAppServer>().Info("Start");

            MyLog.Info(typeof(GatewayAppServer), "Start()");

            this.NewRequestReceived += PlayerAppServer_NewRequestReceived;
            this.SessionClosed += PlayerAppServer_SessionClosed;
            return base.Start();
        }

        void PlayerAppServer_SessionClosed(ClientSession session, CloseReason value)
        {
            LogManager.GetLogger<GatewayAppServer>().Info(value.ToString());

            MyLog.Info(typeof(GatewayAppServer), value.ToString());

            this.messageProcesserFacade.HandlePlayerOffline(session);
        }

        // override AppServer
        public override void Stop()
        {
            MyLog.Info(typeof(GatewayAppServer), "Stop()");
            base.Stop();
        }

        // override AppServerBase
        protected override void OnStarted()
        {
            MyLog.Info(typeof(GatewayAppServer), "OnStarted()");
            base.OnStarted();
        }

        // override AppServerBase
        protected override void OnStopped()
        {
            try
            {
                //IApplicationContext applicationContext = ContextRegistry.GetContext();
                //PlatformNotifier platformNotifier = applicationContext.GetObject("platformNotifier") as PlatformNotifier;
                //RoomConfig roomConfig = applicationContext.GetObject("roomConfig") as RoomConfig;
                //PlatformNotifier.GameServerStateService.Offline(roomConfig.GameId, roomConfig.RoomId);
                //DataHubService
                messageProcesser.Stop();
                MyLog.Info(typeof(GatewayAppServer), "OnStopped()");
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                MyLog.Error(typeof(GatewayAppServer), ex.Message);
            }

            base.OnStopped();
        }

        // override AppServerBase
        protected override void OnNewSessionConnected(ClientSession session)
        {
            MyLog.Info(typeof(GatewayAppServer), string.Format("OnNewSessionConnected()PlayerID:{0}", session.UserNetAddress.ToString()));
            base.OnNewSessionConnected(session);
        }

        /// <summary>
        /// 收到协议处理
        /// </summary>
        /// <param name="session"></param>
        /// <param name="requestInfo"></param>
        void PlayerAppServer_NewRequestReceived(ClientSession session, BaseRequestInfo requestInfo)
        {
            int type = int.Parse(requestInfo.Key);
            //MyLog.Info(typeof(GatewayAppServer), string.Format("NewRequestReceived()session:{0},requestInfo:{1},type:{2}", session, requestInfo, type));
            this.messageProcesserFacade.ProcessMessage(session, requestInfo.MessageHead, requestInfo.Body);
        }
    }
}
