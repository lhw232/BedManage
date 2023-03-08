using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BedAppManage.Help
{
    public class ActionFilter : ActionFilterAttribute   //继承过滤器类
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //判断session是否为空，为空则跳转到登入页面
            if (filterContext.HttpContext.Session["UserName"] == null)
            {
                //filterContext.HttpContext.Response.Redirect("/Home/Login");    //参数为自己写的登入页面的url
                filterContext.HttpContext.Response.Write("<script type='text/javascript'>window.parent.location='/Home/Login'</script>");
            }
        }
    }
}