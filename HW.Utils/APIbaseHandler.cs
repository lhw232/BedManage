using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HW.Utils
{
    public  class APIbaseHandler
    {
        public static ResultData ResultInfo(string code, string msg, string data)
        {
            ResultData resData = new ResultData();
            resData._OP_FLAG = code;
            resData._OP_NOTE = msg;
            resData._DATA = data;


            return resData;
        }
    }
}