using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Threading.Tasks;
using LayIM.Util;
using LayIM.Model;
using LayIM.BLL;
using LayIM.Queue;

namespace LayIM_SignalR_Chat.Hubs
{
    [HubName("layimHub")]
    public class LayIMHub : Hub
    {

        public string CurrentConnectId
        {
            get
            {
                return Context.ConnectionId;
            }
        }
        public override Task OnConnected()
        {
            return Clients.Caller.receiveMessage("连接成功");
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            return Clients.Caller.receiveMessage("失去连接");
        }


        public override Task OnReconnected()
        {
            return Clients.Caller.receiveMessage("重新连接");
        }

        /// <summary>
        /// 单聊连接 1v1
        /// </summary>
        public Task ClientToClient(string sid, string rid)
        {
            var groupId = MessageHelper.GetGroupName(sid, rid);
            //将当前连接人加入到组织内
            Groups.Add(CurrentConnectId, groupId);
            var history = UserBLL.GetHistory(int.Parse(groupId),rid, Config.Chat_One, "");
            dynamic isFriend = UserBLL.IsFriend(int.Parse(sid), int.Parse(rid));
            var result = MessageHelper.GetCTCConnectedMessage(sid, rid,history:history,isFriend:isFriend);

            return Clients.Caller.receiveMessage(result);
        }

        /// <summary>
        /// 群聊
        /// </summary>
        /// <param name="sid"></param>
        /// <param name="rid"></param>
        /// <returns></returns>
        public Task ClientToGroup(string sid, string rid)
        {
            string groupid = "";
            if (rid == "0")
            {
                groupid = Config.Default_Group_Id;
            }
            else
            {
                //
            }
            Groups.Add(CurrentConnectId, groupid);
            var history = UserBLL.GetHistory(int.Parse(rid), rid, Config.Chat_Group, "");
            var result = MessageHelper.GetCTCConnectedMessage(sid, rid, history: history);
            return Clients.Caller.receiveMessage(result);
        }
        /// <summary>
        /// 客户端聊天消息
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public Task ClientSendMsgToClient(ChatMessageResult result)
        {
            var groupId = MessageHelper.GetGroupName(result.fromuser.id, result.touser.id);
            result.groupid = groupId;
            result.type = Config.Chat_One;//1v1
            result.msgtype = MessageType.Custom;//聊天消息，非系统消息
            result.status = 1;
            result.msgid = Guid.NewGuid().ToString();
            //发送给队列
            ChatQueue.PublishMessage(result);
            /*
           //如果没有队列的话，就将上边的注释掉然后切换到直接添加到数据库
            //UserBLL.AddMessage(result);
            */
            //发送给客户端
            return Clients.Group(groupId).receiveMessage(result);
        }
        /// <summary>
        /// 群组发送消息
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public Task ClientSendMsgToGroup(ChatMessageResult result)
        {
            var groupId = result.touser.id == 0 ? Config.Default_Group_Id : result.touser.id.ToString();
            result.groupid = groupId;
            result.type = Config.Chat_Group;//1v1
            result.msgtype = MessageType.Custom;//聊天消息，非系统消息
            result.status = 1;
            result.msgid = Guid.NewGuid().ToString();
            //发送给队列
            ChatQueue.PublishMessage(result);
            /*
             //如果没有队列的话，就将上边的注释掉然后切换到直接添加到数据库
            //UserBLL.AddMessage(result);
             */
            //发送给客户端
            return Clients.Group(groupId).receiveMessage(result);
        }


    }
}