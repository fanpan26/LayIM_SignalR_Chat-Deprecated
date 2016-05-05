using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayIM.Model
{
    public enum MessageType
    {
        System = 1,
        Custom = 2
    }
    /// <summary>
    /// json结果
    /// </summary>
    public class JsonResult
    {
        public int status { get; set; }
        public string msg
        {
            get
            {
                return status == 1 ? "ok" : "err";
            }
        }
        public object data { get; set; }
    }

    public class ConnectionResult : JsonResult
    {
        /// <summary>
        /// 连接类型 one  group
        /// </summary>
        public string type { get; set; }
        public MessageType msgtype { get; set; }
    }

    /// <summary>
    /// 消息结果
    /// </summary>
    public class ChatMessageResult:ConnectionResult
    {
        public ChatMessageResult() {
            addtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
        public User fromuser { get; set; }
        public User touser { get; set; }
        public string message { get; set; }
        public string addtime { get; set; }
        public string msgid { get; set; }
    }


    /// <summary>
    /// 用户好友分组
    /// </summary>
    public class FriendGroup
    {
        public string name { get; set; }
        public int nums { get; set; }
        public int id { get; set; }
        public List<User> item { get; set; }
    }

    /// <summary>
    /// 用户
    /// </summary>
    public class User
    {
        public int id { get; set; }
        public string name { get; set; }
        public string face { get; set; }
    }

    public class ChatLog:User
    {
        public int logid { get; set; }
        public string time { get; set; }
        public string reason { get; set; }
        private string _handle;
        public string handle
        {
            get
            {
                switch (_handle) {
                    case "0":
                        return "申请成为你的好友";
                    case "1":
                        return "已同意该好友申请";
                    case "2":
                        return "已拒绝该好友申请";
                    default:
                        return "";
                }
            }
            set { _handle = value; }
        }
    }
}
