using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CMSManagementConsole.Helpers
    {
    public class AuthorizationFilter : ActionFilterAttribute
        {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
            {
            if (System.Web.HttpContext.Current.Session["accessToken"] == null)
                {
                filterContext.RequestContext.HttpContext.Response.Redirect("/Home/Login");
                }
            }

        }
    }