using BaseServer.Core;
using BaseServer.Interface;
using BaseServer.Util;
using Sproto;
using SprotoType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatewayServer.GameServer
{
    public class ServerClientConnect : BaseClientConnect
    {
        private byte[] loginData;

        private string playerID;

        private IClientSession session;

        public IClientSession Session
        {
            get { return session; }
            set { session = value; }
        }

        private bool isLogin;

        /// <summary>
        /// 0连接Center，1连接GameServer
        /// </summary>
        private byte clientType;

        public ServerClientConnect(byte[] loginData, string playerID, IClientSession session, byte clientType = 0)
        {
            this.loginData = loginData;
            this.playerID = playerID;
            this.session = session;
            this.clientType = clientType;

            EventConnected += OnConnected;
            EventMessageReceived += OnMessageReceived;
            EventConnectionInterrupted += OnConnectClose;
        }

        protected void OnConnected()
        {
            if (!isLogin)
            {
                //Console.WriteLine("Send Login playerID:{0}, session:{1}", playerID, session.UserNetAddress.ToString());
                SendMessage(loginData);
                isLogin = true;
            }
            else
            {
                tcpClient.CloseConnection();
            }
        }

        protected void OnMessageReceived(byte[] data)
        {
            List<byte> allData = new List<byte>();
            allData.AddRange(SprotoBufCodec.IntToBytes(data.Length));
            allData.AddRange(data);
            session.SendMessage(allData.ToArray());
        }

        protected new void OnConnectClose()
        {
            if (clientType == 0)
            {
                session.Close();
            }

        }

        public override void SendHeartBeat()
        {
            //byte[] data = SprotoBufCodec.Encode(Protocol.heardbeat.Tag, new heardbeat.request());
            //SendMeassage(data);
        }

        public void SendToClient(int contractType, SprotoTypeBase contractBody)
        {
            session.SendMessageSP(contractType, contractBody);
        }
    }
}
