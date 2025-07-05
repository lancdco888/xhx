using BaseServer.BaseContract;
using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Wisher.Basic.Protobuf;
using Wisher.Basic.Utility;
using Wisher.ClientEngine.Core;
using Wisher.ClientEngine.Interface;

namespace BaseServer.Core
{
    public class BaseClientConnect : INetService
    {
        private ILog loger = LogManager.GetLogger("BaseClientConnent");
        protected ITcpPassiveEngine tcpClient;
        private HeartBeatTimer heartBeatTimer;

        /// <summary>
        /// 主动断开socket
        /// </summary>
        public bool ActiveCloseSocket
        {
            get;
            private set;
        }

        /// <summary>
        /// 是否已经连接;
        /// </summary>
        public bool IsConnected
        {
            get
            {
                bool result = false;
                if (this.tcpClient != null)
                {
                    result = this.tcpClient.Connected;
                }
                return result;
            }
        }

        #region 事件
        private Action connectionInterrupted;

        public event Action EventConnectionInterrupted
        {
            add { connectionInterrupted += value; }
            remove { connectionInterrupted -= value; }
        }

        private void OnConnectionInterrupted()
        {
            if (connectionInterrupted != null)
            {
                connectionInterrupted();
            }
        }

        private Action connected;
        /// <summary>
        /// 连接服务器成功事件
        /// </summary>
        public event Action EventConnected
        {
            add { connected += value; }
            remove { connected -= value; }
        }

        /// <summary>
        /// 触发连接服务器成功事件
        /// </summary>
        private void OnConnected()
        {
            if (connected != null)
            {
                connected();
            }
        }

        private Action<byte[]> messageReceived;
        /// <summary>
        /// 接收服务器端消息事件
        /// </summary>
        public event Action<byte[]> EventMessageReceived
        {
            add { messageReceived += value; }
            remove { messageReceived -= value; }
        }

        /// <summary>
        /// 触发接收服务器端消息事件
        /// </summary>
        /// <param name="data"></param>
        private void OnMessageReceived(byte[] data)
        {
            if (messageReceived != null)
            {
                messageReceived(data);
            }
        }

        private Action connectClose;

        /// <summary>
        /// 断开服务器事件
        /// </summary>
        public event Action EventConnectClose
        {
            add { connectClose += value; }
            remove { connectClose -= value; }
        }

        protected void OnConnectClose()
        {
            if (connectClose != null)
            {
                connectClose();
            }
        }

        #endregion

        public void Connect(string IP, int port)
        {
            try
            {
                if (!Regex.IsMatch(IP, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$"))
                {
                    IPHostEntry ipHostInfo = Dns.GetHostEntry(IP);

                    IPAddress ipAddress = ipHostInfo.AddressList[0];

                    IP = ipAddress.AddressFamily.ToString();
                }

                heartBeatTimer = new HeartBeatTimer();
                heartBeatTimer.DetectSpanInSecs = 20;
                heartBeatTimer.EventHeartBeat = SendHeartBeat;

                this.tcpClient = NetEngineFactory.CreateStreamTcpPassivEngine(IP, port);
                this.tcpClient.EventConnectionSucceed += ConnectionSucceed;
                this.tcpClient.EventMessageReceived += MessageReceived;
                this.tcpClient.EventConnectionInterrupted += ConnectionInterrupted;
                this.tcpClient.EventEngineDisposed += EngineDisposed;
                this.tcpClient.LogFilePath = AppDomain.CurrentDomain.BaseDirectory + "/Logs/ClientEngineError.log";

                this.tcpClient.AutoReconnect = true;
                this.tcpClient.MaxRetryCount4AutoReconnect = 3;
                this.tcpClient.EventConnectionRebuildSucceed += EventConnectionRebuildSucceed;
                this.tcpClient.EventConnectionRebuildFailure += EventConnectionRebuildFailure;

                this.tcpClient.Initialize();
                heartBeatTimer.Start();
            }
            catch (Exception ex)
            {
                loger.Error(ex);
                heartBeatTimer.Stop();
            }
        }

        private void ConnectionSucceed()
        {
            OnConnected();
        }

        /// <summary>
        /// tcp释放后触发;
        /// </summary>
        /// <param name="obj"></param>
        private void EngineDisposed(INetworkEngine obj)
        {
            loger.Info("tcp释放,关闭");
            heartBeatTimer.Stop();
            OnConnectClose();
        }

        /// <summary>
        /// tcp断开后触发;
        /// </summary>
        private void ConnectionInterrupted()
        {
            if (!this.tcpClient.AutoReconnect)
            {
                loger.Info("网络断开");

                heartBeatTimer.Stop();
            }
            OnConnectionInterrupted();
        }

        /// <summary>
        /// 重连成功后，断线重连逻辑
        /// </summary>
        void EventConnectionRebuildSucceed()
        {
            loger.Info("重连成功");
        }

        //重连失败
        void EventConnectionRebuildFailure()
        {
            //提示网络断开
            loger.Info("重连失败，网络断开");
            heartBeatTimer.Stop();
        }

        public virtual void SendHeartBeat()
        {
            if (IsConnected)
            {
                SendMessage((int)BaseContractType.HeartBeat);
            }
        }

        //Tcp接收消息;
        private void MessageReceived(System.Net.IPEndPoint ipAndPoint, byte[] bytes)
        {
            OnMessageReceived(bytes);
        }

        #region 发送消息

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="data"></param>
        public void SendMessage(byte[] data)
        {
            tcpClient.SendMessage(data);
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="opcode"></param>
        /// <param name="args"></param>
        public void SendMessage(int contractId)
        {
            MessageContract messageContract = new MessageContract() { ContractType = contractId };

            byte[] message = ProtobufCodec.Encode<MessageContract>(messageContract);

            int len = message.Length + 1;

            if (tcpClient == null) return;

            byte[] byteLen = ConvertHelper.IntToBytes(len);

            List<byte> all = new List<byte>();

            //包标识
            //all.AddRange(magics);
            all.AddRange(byteLen);//包长
            all.AddRange(BitConverter.GetBytes(true));//标示包的来源1：表示服务器0：表示客户端
            all.AddRange(message); //包体
            tcpClient.SendMessage(all.ToArray());
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="opcode"></param>
        /// <param name="args"></param>
        public void SendMessage<T>(int contractId, T t) where T : class
        {
            byte[] body = ProtobufCodec.Encode<T>(t);

            MessageContract messageContract = new MessageContract() { ContractType = contractId, Body = body };

            byte[] message = ProtobufCodec.Encode<MessageContract>(messageContract);

            int len = message.Length + 1;

            if (tcpClient == null) return;

            byte[] byteLen = ConvertHelper.IntToBytes(len);

            List<byte> all = new List<byte>();


            //all.AddRange(magics);//包标识
            all.AddRange(byteLen);//包长
            all.AddRange(BitConverter.GetBytes(true));//标示包的来源1：表示服务器0：表示客户端
            all.AddRange(message);//包体
            tcpClient.SendMessage(all.ToArray());
        }
        #endregion

        /// <summary>
        /// 关闭Tcp
        /// </summary>
        public void Close()
        {
            if (this.tcpClient != null)
            {
                this.ActiveCloseSocket = true;
                this.tcpClient.CloseConnection();
                heartBeatTimer.Stop();
            }
        }

        public void Dispose()
        {
            this.Close();
        }

        /// <summary>
        /// Determines whether the specified return code is success.
        /// </summary>
        /// <param name="returnCode">The return code.</param>
        /// <returns><c>true</c> if the specified return code is success; otherwise, <c>false</c>.</returns>
        public bool IsSuccess(int returnCode)
        {
            return returnCode == 50000;
        }
    }
}
