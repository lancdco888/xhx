using BaseServer.Core;
using BaseServer.Interface;
using BaseServer.Util;
using SprotoType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatewayServer.GameServer
{
    public class GameServerClientConnect : BaseClientConnect
    {
        private byte[] loginData;

        private string playerID;

        private IPlayerSession session;

        private bool isLogin;

        public GameServerClientConnect(byte[] loginData, string playerID, IPlayerSession session)
        {
            this.loginData = loginData;
            this.playerID = playerID;
            this.session = session;

            EventConnected += OnConnected;
            EventMessageReceived += OnMessageReceived;
            EventConnectionInterrupted += OnConnectClose;
        }

        protected void OnConnected()
        {
            if (!isLogin)
            {
               // Console.WriteLine("Send Login playerID:{0}", playerID);
                SendMeassage(loginData);
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
            session.Close();
        }

        public override void SendHeartBeat()
        {
            //byte[] data = SprotoBufCodec.Encode(Protocol.heardbeat.Tag, new heardbeat.request());
            //SendMeassage(data);
        }
    }
}
