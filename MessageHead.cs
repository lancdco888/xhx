using DataHub.Core.Interface;
namespace BaseChessCard.Core
{
    /// <summary>
    /// 消息头
    /// </summary>
    public class MessageHead : IMessageHead
    {
        /// <summary>
        /// 消息类型
        /// </summary>
        public int ContractType { get; set; }

        /// <summary>
        /// 签名
        /// </summary>
        public string Sign { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        public long Timestamp { get; set; }

        /// <summary>
        /// 玩家ID
        /// </summary>
        public string PlayerID { get; set; }

        public MessageHead()
        { }

        public MessageHead(string userID, int contractType)
        {
            this.PlayerID = userID;
            this.ContractType = contractType;
        }

        public MessageHead(int contractType)
        {
            this.ContractType = contractType;
        }

        public MessageHead(string userID, int contractType, long timestamp, string sign)
        {
            this.PlayerID = userID;
            this.ContractType = contractType;
            this.Timestamp = timestamp;
            this.Sign = sign;
        }
    }
}
