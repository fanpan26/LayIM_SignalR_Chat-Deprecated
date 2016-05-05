using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayIM.Model
{
    /// <summary>
    /// 在线用户Model
    /// </summary>
    public class OnlineUser
    {
        public int UserId { get; set; }
        public string ConnectionId { get; set; }
        public string GroupId { get; set; }
    }
}
