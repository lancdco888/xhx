using BaseServer.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseServer.Core
{
    /// <summary>
    /// 协议命令接口
    /// </summary>
    public interface IGameCommand
    {
        /// <summary>
        /// 处理玩家消息
        /// </summary>
        /// <param name="playerID"></param>
        /// <param name="messageBody"></param>
        void ProcessMessage(string playerID, byte[] messageBody, IClientSession session);

        /// <summary>
        /// 处理AI消息
        /// </summary>
        /// <typeparam name="TBody"></typeparam>
        /// <param name="playerID"></param>
        /// <param name="body"></param>
        void ProcessMessage<TBody>(string playerID, TBody body);
    }
}
