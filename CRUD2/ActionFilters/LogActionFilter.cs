using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CRUD2.ActionFilters
{
    public class LogActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            Log("OnActionExecuted", filterContext.RouteData);
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Log("OnActionExecuting", filterContext.RouteData);
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            Log("OnResultExecutied", filterContext.RouteData);
        }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            Log("OnResultExecuting", filterContext.RouteData);
        }

        private void Log(string methodName, RouteData routeData)
        {
            var controllerName = routeData.Values["controller"];
            var actionName = routeData.Values["action"];
            var message = String.Format("{3} :  {0}- controller: {1}- action: {2}", methodName, controllerName, actionName, DateTime.Today);

            string path = @"~/Content/TextFile/Log.txt";
            path = HttpContext.Current.Server.MapPath(path);

            if (File.Exists(path))
            {
                using (StreamWriter sw = new StreamWriter(path, true))
                {
                    sw.WriteLine(message);
                }
            }

            Debug.WriteLine(message);
        }
    }
}