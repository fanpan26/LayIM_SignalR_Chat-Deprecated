using API.BLL;
using LayIM.Model;
using LayIM.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayIM.BLL
{
    /// <summary>
    /// 
    /// </summary>
    public class UserBLL
    {
        /// <summary>
        /// 获取好友列表
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public static JsonResult GetFriends(int userid)
        {
            var data = new List<FriendGroup>();

            string proceName = "Proc_GetChatUserFriendList";
            var parameters = new List<SqlParameter> {
                DBUtil.MakeParameterInt("userid",userid)
            };
            var ds = DBUtil.ExecuteDataSetStoreProcedure(proceName, parameters.ToArray());
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                int id = int.Parse(dr["groupid"].ToString());
                DataRow[] users = ds.Tables[1].Select("groupid=" + id);
                var items = new List<User>();
                foreach (DataRow drUser in users)
                {
                    items.Add(new User
                    {
                        id = int.Parse(drUser["friendid"].ToString()),
                        face = drUser["photo"].ToString(),
                        name = drUser["name"].ToString()
                    });
                }
                data.Add(new FriendGroup
                {
                    id = id,
                    name = dr["groupname"].ToString(),
                    nums = users.Length,
                    item = items
                });
            }

            return new JsonResult
            {
                status = Config.code_success,
                data = data
            };
        }

        /// <summary>
        /// 添加消息
        /// </summary>
        /// <param name="type"></param>
        /// <param name="msg"></param>
        /// <param name="fromuser"></param>
        /// <param name="groupid"></param>
        /// <returns></returns>
        public static JsonResult AddMessage(int type, string msg, int fromuser, int groupid,string msgid,string images,string files)
        {
            Task.Factory.StartNew(() =>
             {
                 var spName = "Proc_Chat_AddMessage";

                 var parameters = new List<SqlParameter> {
                     DBUtil.MakeParameterVarChar("msgid",msgid),
                DBUtil.MakeParameterInt("type",type),
                DBUtil.MakeParameterVarChar("msg",msg),
                DBUtil.MakeParameterInt("fromuser",fromuser),
                DBUtil.MakeParameterInt("groupid",groupid),
                DBUtil.MakeParameterVarChar("images",images),
                DBUtil.MakeParameterVarChar("files",files)
             };
                 var result = DBUtil.ExecuteNonQueryStoreProcedure(spName, parameters.ToArray());
                 return new JsonResult
                 {
                     status = result > 0 ? 1 : 0,
                     data = ""
                 };
             });
            return null;
        }
        /// <summary>
        /// 获取默认分组
        /// </summary>
        /// <returns></returns>
        public static JsonResult GetGroup()
        {
            var data = new List<FriendGroup>();
            data.Add(new FriendGroup
            {
                id = 1,
                name = "默认分组",
                nums = 1,
                item = new List<User> { new User { id = 0, name = "注册用户", face = "/images/register.png" } }
            });
            var json = new JsonResult
            {
                status = 1,
                data = data
            };
            return json;
        }

        /// <summary>
        /// 获取申请记录
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public static JsonResult GetChatLog(int userid)
        {
            string sql = "	SELECT * FROM V_Chat_User_Apply WHERE userid=" + userid +"order by addtime desc";
            var dt = DBUtil.ExecuteDateTableSQL(sql, null);

            var chatLog = dt.Rows.Cast<DataRow>().Select(x => new ChatLog
            {
                logid=int.Parse(x["id"].ToString()),
                id = int.Parse(x["friendid"].ToString()),
                name = x["name"].ToString(),
                face = x["photo"].ToString(),
                handle=x["handle"].ToString(),
                reason=x["reason"].ToString(),
                time = x["addtime"].ToString()
            }).ToList();

            var json = new JsonResult
            {
                status = 1,
                data = chatLog
            };
            return json;
        }
        /// <summary>
        /// 处理申请
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="applyid"></param>
        /// <param name="isagree"></param>
        /// <param name="logid"></param>
        /// <returns></returns>
        public static JsonResult ExecuteFriendApply(int userid, int applyid, bool isagree, int logid)
        {

            var spName = "Proc_Chat_AddFriendWithGroup";
            var parameters = new List<SqlParameter> {
                DBUtil.MakeParameterInt("userid",userid),
                DBUtil.MakeParameterInt("friendid",applyid),
                DBUtil.MakeParameterBit("isadd",isagree),
                DBUtil.MakeParameterInt("id",logid)
            };
            DBUtil.ExecuteNonQueryStoreProcedure(spName, parameters.ToArray());
            var json = new JsonResult
            {
                status = 1,
                data = ""
            };
            return json;
        }

        /// <summary>
        /// 读取历史记录
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="type"></param>
        /// <param name="lastId"></param>
        /// <returns></returns>
        public static List<ChatMessageResult> GetHistory(int groupId,string rid, string type, string lastId)
        {

            var spName = "Proc_Chat_GetHistory";
            var parameters = new List<SqlParameter> {
                DBUtil.MakeParameterInt("groupid",groupId),
                DBUtil.MakeParameterVarChar("lastid",lastId)
            };
            var dt = DBUtil.ExecuteDataTableStoreProcedure(spName, parameters.ToArray());
            List<ChatMessageResult> history = new List<ChatMessageResult>();
            foreach (DataRow dr in dt.Rows)
            {
                history.Add(new ChatMessageResult
                {
                    fromuser = new User
                    {
                        face = dr["photo"].ToString(),
                        id = int.Parse(dr["fromuser"].ToString()),
                        name = dr["name"].ToString()
                    },
                    addtime = dr["addtime"].ToString(),
                    message = dr["msg"].ToString(),
                    files=JsonHelper.DeserializeObject<List<ChatImg>>(dr["files"].ToString()),
                    images = JsonHelper.DeserializeObject<List<ChatImg>>(dr["images"].ToString()),
                    msgtype = MessageType.Custom,
                    type = type,
                    msgid=dr["id"].ToString(),
                    touser=new User
                    {
                        id = int.Parse(rid)
                    }
                });
            }
            return history;
        }
        /// <summary>
        /// 获取所有的用户
        /// </summary>
        /// <returns></returns>
        public static JsonResult GetDefaultGroupMembers()
        {
            string sql = "SELECT userid AS id,name,photo AS face FROM chat_user";
            var dt = DBUtil.ExecuteDateTableSQL(sql);
            var user = dt.Rows.Cast<DataRow>().Select(x => new User
            {
                id = int.Parse(x["id"].ToString()),
                face = x["face"].ToString(),
                name = x["name"].ToString()
            }).ToList();
            return new JsonResult { status = 1, data =user };
        }
        /// <summary>
        /// 登录或者注册
        /// </summary>
        /// <param name="username"></param>
        /// <param name="userpwd"></param>
        /// <returns></returns>
        public static JsonResult Login(string username, string userpwd)
        {
            var spName = "Proc_Chat_LoginOrRegister";
            var parameters = new List<SqlParameter> {
                DBUtil.MakeParameterVarChar("username",username),
                DBUtil.MakeParameterVarChar("userphoto",""),
                DBUtil.MakeParameterVarChar("userpwd",userpwd)
            };

           var dt = DBUtil.ExecuteDataTableStoreProcedure(spName, parameters.ToArray());
            return new JsonResult
            {
                status = 1,
                data = JsonHelper.SerializeObject(dt)
            };
        }

        public static dynamic IsFriend(int userid, int friendid)
        {
            if (userid == friendid) { return true; }
            var spName = "Proc_Chat_IsFriend";
            var parameters = new List<SqlParameter> {
                DBUtil.MakeParameterInt("userid",userid),
                DBUtil.MakeParameterInt("friendid",friendid)
            };
            var dt = DBUtil.ExecuteDataTableStoreProcedure(spName, parameters.ToArray());
            return new { handle = dt.Rows[0][0].ToString(), friend = dt.Rows[0][1].ToString(),reason=dt.Rows[0][2].ToString(),logid=dt.Rows[0][3].ToString() };
        }

        /// <summary>
        /// 添加好友申请
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="applyid"></param>
        /// <param name="reason"></param>
        public static void ApplyFriend(int userid, int applyid, string reason)
        {
            var spName = "Proc_Chat_FriendApply";
            var parameters = new List<SqlParameter> {
                DBUtil.MakeParameterInt("userid",userid),
               DBUtil.MakeParameterInt("applyid",applyid),
                DBUtil.MakeParameterVarChar("reason",reason)
            };
            DBUtil.ExecuteNonQueryStoreProcedure(spName, parameters.ToArray());
        }

        public static bool UpdateUserPhoto(string photo, int userid)
        {
            string sql = "UPDATE chat_user SET photo = @photo WHERE userid=@userid";
            var parameters = new List<SqlParameter> {
                DBUtil.MakeParameterInt("userid",userid),
                DBUtil.MakeParameterVarChar("photo",photo)
            };
            return DBUtil.ExecuteNonQuerySQL(sql, parameters.ToArray()) > 0;
        }
    }
}
