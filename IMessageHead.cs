using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseServer.Core
{
    /// <summary>
    /// 消息头接口
    /// </summary>
    public interface IMessageHead
    {
        /// <summary>
        /// 消息类型
        /// </summary>
        int ContractType { set; get; }

        /// <summary>
        /// 客户端消息编码
        /// </summary>
        int UserMessageCode { set; get; }

        /// <summary>
        /// 服务端消息编码
        /// </summary>
        int ServerMessageCode { set; get; }

        /// <summary>
        /// 签名
        /// </summary>
        string Sign { set; get; }

        /// <summary>
        /// 时间戳
        /// </summary>
        long Timestamp { set; get; }

        /// <summary>
        /// 玩家ID
        /// </summary>
        string PlayerID { set; get; }

        /// <summary>
        /// 游戏ID
        /// </summary>
        int ServerID { set; get; }

        /// <summary>
        /// 是否来自服务器
        /// </summary>
        bool IsFromServer { get; set; }
    }
}
