using Common;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace SSODemoNew1
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new LoginAttribute());

        }
    }

    /// <summary>
    /// 登录全局Filter （判断用户是否登录）
    /// </summary>
    public class LoginAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
          
            var allowMous = filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true);
            //后台
            if (!CurrentUser.IsLogin && !allowMous)
            {
                filterContext.Result = new RedirectResult("/Home/Login");
            }
            base.OnActionExecuting(filterContext);
        }
    }
}
