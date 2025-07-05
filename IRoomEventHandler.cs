using BaseServer.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseServer.Core
{
    /// <summary>
    /// 房间事件接口
    /// </summary>
    public interface IRoomEventHandler
    {
        /// <summary>
        /// 游戏开始
        /// </summary>
        /// <param name="desk"></param>
        void OnAdaptiveDeskGameStarting(IDesk desk);

        /// <summary>
        /// 电脑玩家移除
        /// </summary>
        /// <param name="aiPlayer"></param>
        void OnAIRemoved(IPlayer aiPlayer);

        /// <summary>
        /// 房间过期
        /// </summary>
        /// <param name="deskID"></param>
        void OnDeskExpired(IDesk deskID);

        /// <summary>
        /// VIP房间终止
        /// </summary>
        /// <param name="userList"></param>
        void OnVipDeskTerminated(IList<string> userList);

        /// <summary>
        /// 玩家离开
        /// </summary>
        /// <param name="player"></param>
        void SomeoneUserLeave(IPlayer player);
    }
}
