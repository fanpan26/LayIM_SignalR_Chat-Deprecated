using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace LayIM_SignalR_Chat
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                  name: "getuserfriend",
                url: "friend",
                defaults: new { controller = "Home", action = "GetUserFriend", id = UrlParameter.Optional }
                );
            routes.MapRoute(
                 name: "apply",
               url: "apply",
               defaults: new { controller = "Home", action = "AddApply", id = UrlParameter.Optional }
               );
            routes.MapRoute(
                name: "executeapply",
              url: "executeapply",
              defaults: new { controller = "Home", action = "ExecuteFriendApply", id = UrlParameter.Optional }
              );
            
            routes.MapRoute(
                  name: "getchatlog",
                url: "log",
                defaults: new { controller = "Home", action = "GetChatLog", id = UrlParameter.Optional }
                );
            routes.MapRoute(
                name: "getdefaultgroup",
              url: "group",
              defaults: new { controller = "Home", action = "GetDefaultGroup", id = UrlParameter.Optional }
              );
            routes.MapRoute(
               name: "getdefaultgroupmembers",
             url: "groups",
             defaults: new { controller = "Home", action = "GetDefaultGroupMembers", id = UrlParameter.Optional }
             );
            routes.MapRoute(
             name: "history",
           url: "history",
           defaults: new { controller = "Home", action = "GetHistory", id = UrlParameter.Optional }
           );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
