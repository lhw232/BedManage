using BedAppManage.Core.Biz;
using BedAppManage.Models;
using HW.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BedAppManage.Controllers
{
    public class HomeController : baseHandlerAPI
    {
        [Help.ActionFilter]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult Login()
        {
            return View();
        }

        public class param
        {
            public string registerUsername { get; set; }
            public string registerPassword { get; set; }
        }
        public string Register(string param)
        {
            if (!string.IsNullOrEmpty(param))
            {
                param pa = JsonConvert.DeserializeObject<param>(param);

                MembersInfo model = new MembersInfo();
                model.nickname = pa.registerUsername;
                model.passWord = pa.registerPassword;
                model.state = "1";

                string error = string.Empty;
                bool b = new Members().Add(model,out error);

                if (b)
                {
                    return ResultSuccess("注册成功！");
                }
                else {
                    return ResultError("注册失败！"+ "("+error+")");
                }


            }
            return ResultSuccess("注册成功！");
        }

        public string LoginCheck(string loginUsername, string loginPassword)
        {
            DataTable dt = new Members().GetDataList(" nickname='" + loginUsername + "'", "");
            if (dt.Rows.Count > 0)
            {
                DataTable dt1 = new Members().GetDataList(" nickname='" + loginUsername.Trim() + "' and passWord='" + loginPassword.Trim() + "'", "");
                if (dt1.Rows.Count > 0)
                {
                    MembersInfo model = new Members().GetEntity(Convert.ToInt32(dt1.Rows[0]["no"]));
                    if (model.state == "1")
                    {
                        Session["UserName"] = loginUsername;
                        Session["PWD"] = loginPassword;
                        return ResultSuccess("登录成功！");
                    }
                    else if (model.state == "0")
                    {
                        return ResultError("该账户已被停用，请联系管理员！");
                    }

                }
                else
                {
                    Session["UserName"] = "";
                    Session["PWD"] = "";
                    return ResultError("密码错误！");
                }
            }
            else
            {
                return ResultError("用户名不存在！");
            }
            return ResultError("登录失败！");
        }

    }
}