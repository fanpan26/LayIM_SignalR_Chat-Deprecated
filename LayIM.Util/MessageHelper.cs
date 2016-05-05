using LayIM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayIM.Util
{
    /// <summary>
    /// 消息帮助处理类
    /// </summary>
    public static class MessageHelper
    {
        /// <summary>
        ///  组织，发送人，接收人
        /// </summary>
        /// <param name="sendid"></param>
        /// <param name="receiveid"></param>
        /// <returns></returns>
        public static string GetGroupName(string sendid, string receiveid)
        {
            int compare = string.Compare(sendid, receiveid);
            if (compare > 0)
            {
                return string.Format("{0}{1}", sendid, receiveid);
            }
            return string.Format("{0}{1}", receiveid, sendid);
        }
        /// <summary>
        /// 获取连接成功的消息
        /// </summary>
        /// <param name="sendid"></param>
        /// <param name="receiveid"></param>
        /// <returns></returns>
        public static JsonResult GetCTCConnectedMessage(string sendid, string receiveid, string type = Config.Chat_One, List<ChatMessageResult> history = null, dynamic isFriend = null)
        {
            //获取两者是否是好友
            var result = new ConnectionResult
            {
                status = 1,
                type = type,
                msgtype = MessageType.System,
                data = new { sid = sendid, rid = receiveid,isfriend=isFriend, gid = GetGroupName(sendid, receiveid), history = history, msg = "连接成功" }
            };
            return result;
        }

        /// <summary>
        /// 组织，发送人，接收人
        /// </summary>
        /// <param name="sendid"></param>
        /// <param name="receiveid"></param>
        /// <returns></returns>
        public static string GetGroupName(int sendid, int receiveid)
        {
            return GetGroupName(sendid.ToString(), receiveid.ToString());
        }
    }
}
