using BaseServer.AIPlayer;
using BaseServer.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseServer.Core
{
    /// <summary>
    /// 房间事件处理器基类
    /// </summary>
    public abstract class BaseRoomEventHandler : BaseHandler, IRoomEventHandler
    {
        protected IGameRoom gameRoom;
        /// <summary>
        /// 游戏房间
        /// </summary>
        public IGameRoom GameRoom
        {
            set { gameRoom = value; }
        }

        protected ContainerStyleComputerResultDecider computerResultDecider;
        public ContainerStyleComputerResultDecider ComputerResultDecider
        {
            set { computerResultDecider = value; }
        }

        public BaseRoomEventHandler(IGameRoom gameRoom)
        {
            this.gameRoom = gameRoom;
            Initialize();
        }

        private void Initialize()
        {
            if (this.gameRoom is IAdaptiveGameRoom)
            {
                (this.gameRoom as IAdaptiveGameRoom).DeskExpired += new CbDesk(this.OnDeskExpired);
                (this.gameRoom as IAdaptiveGameRoom).OnAdaptiveDeskGameStarting += new CbDesk(this.OnAdaptiveDeskGameStarting);
            }
            else
            {
                //VIP 房间暂时保留
            }
        }

        /// <summary>
        /// 删除死亡桌子
        /// </summary>
        /// <param name="deskID"></param>
        protected abstract void RemoveDeadDesk(int deskID);

        /// <summary>
        /// 作蔽处罚，设置必赢玩家
        /// </summary>
        /// <param name="desk"></param>
        /// <param name="cheatDictionary"></param>
        /// <param name="processFunc"></param>
        protected void SetWinPlayer(IDesk desk, IDictionary<long, bool> cheatDictionary, Action<IPlayer> processFunc)
        {

        }

        /// <summary>
        /// 游戏开始
        /// </summary>
        /// <param name="desk"></param>
        public abstract void OnAdaptiveDeskGameStarting(IDesk desk);

        /// <summary>
        /// AI移除
        /// </summary>
        /// <param name="aiPlayer"></param>
        public abstract void OnAIRemoved(IPlayer aiPlayer);

        /// <summary>
        /// 桌子过期
        /// </summary>
        /// <param name="deskID"></param>
        public abstract void OnDeskExpired(IDesk deskID);

        /// <summary>
        /// VIP桌子终止
        /// </summary>
        /// <param name="userList"></param>
        public abstract void OnVipDeskTerminated(IList<string> userList);

        /// <summary>
        /// VIP游戏开始
        /// </summary>
        /// <param name="desk"></param>
        public abstract void OnVipDeskGameStarting(IDesk desk);

        /// <summary>
        /// 玩家离开
        /// </summary>
        /// <param name="player"></param>
        public abstract void SomeoneUserLeave(Interface.IPlayer player);
    }
}
