using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ATWebLogger
{
    public class BaseController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (SessionHelper.GetSession() == null)
            {
                filterContext.Result = new RedirectToRouteResult(new
                    RouteValueDictionary(new { controller = "Login", action = "Index" }));
            }
            base.OnActionExecuting(filterContext);
        }
    }

    [Serializable]
    public class UserSession
    {
        public string User { get; set; }
        public string Role { get; set; }
    }

    public class SessionHelper
    {
        public static void SetSession(UserSession session)
        {
            HttpContext.Current.Session["loginSession"] = session;
        }

        public static UserSession GetSession()
        {
            var session = HttpContext.Current.Session["loginSession"];
            if (session == null)
                return null;
            else
                return session as UserSession;
        }

        public static void ClearSession()
        {
            HttpContext.Current.Session["loginSession"] = null;
        }
    }
}