using BaseServer.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseServer.Core
{
    /// <summary>
    /// 掉线处理器接口
    /// </summary>
    public interface IOfflineHandler
    {
        /// <summary>
        /// 处理玩家下线
        /// </summary>
        /// <param name="playerID"></param>
        void HandlePlayerOffline(IClientSession session);
    }
}
