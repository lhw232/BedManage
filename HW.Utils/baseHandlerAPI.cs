using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace HW.Utils
{
    public class baseHandlerAPI: Controller
    {
        public string ResultInfo(string code, string msg, string data)
        {
            ResultData resData = new ResultData();
            resData._OP_FLAG = code;
            resData._OP_NOTE = msg;
            resData._DATA = data;


            return JsonHelper.ToJson(resData);
        }

        public string ResultSuccess(string msg)
        {
            ResultData resData = new ResultData();
            resData._OP_FLAG = "1";
            resData._OP_NOTE = msg;


            return JsonHelper.ToJson(resData);
        }

        public string ResultError(string code, string msg)
        {
            ResultData resData = new ResultData();
            resData._OP_FLAG = code;
            resData._OP_NOTE = msg;


            return JsonHelper.ToJson(resData);
        }
        public string ResultError(string msg)
        {
            ResultData resData = new ResultData();
            resData._OP_FLAG = "0";
            resData._OP_NOTE = msg;


            return JsonHelper.ToJson(resData);
        }

        #region 物理路径和相对路径的转换
        //本地路径转换成URL相对路径 
        public string urlconvertor(string imagesurl1)
        {
            string tmpRootDir = Server.MapPath(System.Web.HttpContext.Current.Request.ApplicationPath.ToString());//获取程序根目录
            string imagesurl2 = imagesurl1.Replace(tmpRootDir, ""); //转换成相对路径
            imagesurl2 = imagesurl2.Replace(@"\", @"/");
            return imagesurl2;
        }
        //相对路径转换成服务器本地物理路径 
        public string urlconvertorlocal(string imagesurl1)
        {
            string tmpRootDir = Server.MapPath(System.Web.HttpContext.Current.Request.ApplicationPath.ToString());//获取程序根目录
            string imagesurl2 = tmpRootDir + imagesurl1.Replace(@"/", @"\"); //转换成绝对路径
            return imagesurl2;
        }
        #endregion
    }
}