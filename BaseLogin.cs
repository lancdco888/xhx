using DataHub.Contract;
using DataHub.Core.Interface;
using ESBasic.Threading.Timers;
using Wisher.Basic.Protobuf;

namespace DataHub.Core
{
    /// <summary>
    /// 登录基类
    /// </summary>
    public abstract class BaseLogin : BaseCommand
    {
        protected BaseLogin() { }

        /// <summary>
        /// 处理登录协议
        /// </summary>
        /// <param name="session"></param>
        /// <param name="body"></param>
        internal void ProcessMessage(IPlayerSession session, byte[] body)
        {
            LoginContract loginContract = ProtobufCodec.Decode<LoginContract>(body);
            var playerID = loginContract.PlayerID.ToLower();
            bool succeed = this.Login(session, loginContract);

            if (succeed)
            {
                (this.messageHandler as MessageHandler).TryRemove(playerID);
                (this.messageHandler as MessageHandler).TryAdd(playerID, session);
            }

            ProcessMessage(session.PlayerID, body);
        }

        public override void ProcessMessage(string userID, byte[] body)
        {

        }

        /// <summary>
        /// 会员登录
        /// </summary>
        /// <param name="session"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        protected abstract bool Login(IPlayerSession session, LoginContract body);
    }
}
