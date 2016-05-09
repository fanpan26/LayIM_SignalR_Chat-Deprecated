using System.Web;
using System.Web.Mvc;

namespace LayIM_SignalR_Chat_V1
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
