using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrainningManagement.SessionValidate
{
    public class ActionAuthorize:System.Web.Mvc.ActionFilterAttribute,System.Web.Mvc.IActionFilter
    {
        public override void OnActionExecuting(System.Web.Mvc.ActionExecutingContext filterconte)
        {
            if (HttpContext.Current.Session["EmployeeData"] == null)
            {
                filterconte.Result = new System.Web.Mvc.RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary
                {
                    {"Controller","Login" },
                    {"Action","Index" }
                });
            }
            base.OnActionExecuting(filterconte);
        }
    }
}