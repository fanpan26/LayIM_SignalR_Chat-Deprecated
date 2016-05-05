using LayIM.BLL;
using LayIM.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LayIM_SignalR_Chat.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        #region 业务
        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public JsonResult GetUserFriend(int userid)
        {
            var json = UserBLL.GetFriends(userid);
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetChatLog(int userid)
        {
            var json = UserBLL.GetChatLog(userid);
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDefaultGroup()
        {
            var json = UserBLL.GetGroup();
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDefaultGroupMembers()
        {
            var json = UserBLL.GetDefaultGroupMembers();
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        ///登录
        public JsonResult Login(string username, string pwd)
        {
            var json = UserBLL.Login(username, pwd);
            return Json(json, JsonRequestBehavior.DenyGet);
        }
        /// <summary>
        /// 获取历史记录
        /// </summary>
        /// <param name="lastid"></param>
        /// <param name="rid"></param>
        /// <param name="type"></param>
        /// <param name="groupid"></param>
        /// <returns></returns>
        public JsonResult GetHistory(string lastid,string rid,string type, string groupid)
        {
            if (type == Config.Chat_One) {
                groupid = MessageHelper.GetGroupName(groupid, rid);
            }
           var history = UserBLL.GetHistory(int.Parse(groupid), rid, type, lastid);
            return Json(history, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 添加好友申请
        [HttpPost]
        public JsonResult AddApply(int userid, int applyid, string reason)
        {
            UserBLL.ApplyFriend(userid, applyid, reason);
            return Json("", JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult ExecuteFriendApply(int userid, int applyid, bool isagree, int logid)
        {
            var json = UserBLL.ExecuteFriendApply(userid, applyid, isagree, logid);
            return Json(json, JsonRequestBehavior.DenyGet);
        }
        #endregion
        public ViewResult Login()
        {
            return View();
        }
    }
}