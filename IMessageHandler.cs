using BaseServer.Interface;
using Sproto;
using System.Collections.Generic;

namespace BaseServer.Core
{
    /// <summary>
    /// 消息处理器接口
    /// </summary>
    public interface IMessageHandler
    {
        /// <summary>
        /// 发送消息给所有玩家(sp)
        /// </summary>
        /// <param name="contractType"></param>
        /// <param name="bodyContract"></param>
        void SendMessageToAllSP(int contractType, SprotoTypeBase bodyContract);


        void SendMassageSP(int contreactType, SprotoTypeBase bodyContract, params string[] userIDArray);

        void SendMessageSP(int contractType, SprotoTypeBase bodyContract, IList<string> userIDList);

        void SendMessageToAllNoDeskUsersSP(int contractType, SprotoTypeBase bodyContract);

        void SendMessageToAllDeskUsersSP(IDesk myDesk, int contractType, SprotoTypeBase bodyContract);

        void SendMessageToPatternsSP(string myUserID, IDesk myDesk, int contractType, SprotoTypeBase bodyContract);
 
        /// <summary>
        /// 发送消息给所有玩家
        /// </summary>
        /// <typeparam name="TBody"></typeparam>
        /// <param name="contractType"></param>
        /// <param name="bodyContract"></param>
        void SendMessageToAll<TBody>(int contractType, TBody bodyContract) where TBody : class;

        /// <summary>
        /// 发送消息给某些玩家
        /// </summary>
        /// <typeparam name="TBody"></typeparam>
        /// <param name="contractType"></param>
        /// <param name="bodyContract"></param>
        /// <param name="userIDArray"></param>
        void SendMessage<TBody>(int contractType, TBody bodyContract, params string[] userIDArray) where TBody : class;

        /// <summary>
        /// 发送消息给某些玩家
        /// </summary>
        /// <typeparam name="TBody"></typeparam>
        /// <param name="contractType"></param>
        /// <param name="bodyContract"></param>
        /// <param name="userIDList"></param>
        void SendMessage<TBody>(int contractType, TBody bodyContract, System.Collections.Generic.IList<string> userIDList) where TBody : class;

        /// <summary>
        /// 发送消息给所有未进桌玩家
        /// </summary>
        /// <typeparam name="TBody"></typeparam>
        /// <param name="contractType"></param>
        /// <param name="bodyContract"></param>
        void SendMessageToAllNoDeskUsers<TBody>(int contractType, TBody bodyContract) where TBody : class;

        /// <summary>
        /// 发送消息给同桌所有玩家
        /// </summary>
        /// <typeparam name="TBody"></typeparam>
        /// <param name="myDesk"></param>
        /// <param name="contractType"></param>
        /// <param name="bodyContract"></param>
        void SendMessageToAllDeskUsers<TBody>(IDesk myDesk, int contractType, TBody bodyContract) where TBody : class;

        /// <summary>
        /// 发送消息给同桌
        /// </summary>
        /// <typeparam name="TBody"></typeparam>
        /// <param name="myUserID"></param>
        /// <param name="myDesk"></param>
        /// <param name="contractType"></param>
        /// <param name="bodyContract"></param>
        void SendMessageToPatterns<TBody>(string myUserID, IDesk myDesk, int contractType, TBody bodyContract) where TBody : class;

        /// <summary>
        /// 踢除某些玩家
        /// </summary>
        /// <param name="userIDArray"></param>
        void KickUser(params string[] userIDArray);

        /// <summary>
        /// 判断玩家是否存在
        /// </summary>
        /// <param name="playerID"></param>
        /// <returns></returns>
        bool IsHaveUser(string playerID);
    }
}
