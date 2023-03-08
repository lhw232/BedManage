using BedAppManage.Core;
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
    public class LunboController : baseHandlerAPI
    {
        #region 公共变量
        Lunbo lunboObj = new Lunbo();
        #endregion
        // GET: Lunbo
        [Help.ActionFilter]
        public ActionResult List()
        {
            return View();
        }
        [Help.ActionFilter]
        public ActionResult Add()
        {
            LunboInfo model = new LunboInfo();
            model.no = -1;
            ViewData["LunboInfo"] = model;

            DataTable dt = lunboObj.GetDataList("","");
            if (dt.Rows.Count > 0)
            {
                model.orderNo = dt.Rows.Count + 1;
            }
            else
            {
                model.orderNo = 1;
            }

            return View();
        }
        [Help.ActionFilter]
        public ActionResult Edit(string JsonModel)
        {
            LunboInfo model = JsonConvert.DeserializeObject<LunboInfo>(JsonModel);
            ViewData["LunboInfo"] = model;
            return View("Add");
        }
        public class NosInfo
        {
            public string no { get; set; }
            public string img { get; set; }
        }
        public string Del(string nos) {
            List<NosInfo> list = JsonConvert.DeserializeObject<List<NosInfo>>(nos);

            foreach (NosInfo model in list)
            {
                if (!string.IsNullOrEmpty(model.no))
                {
                    bool b = lunboObj.Delete(Convert.ToInt32(model.no));
                    // 已知查询到的文件相对路径为file_path// 获取程序物理路径
                    string str = System.Web.HttpRuntime.AppDomainAppPath.ToString();
                    bool isFile = false;
                    // 路径拼接,获取文件在服务器或本地的全部路径
                    var path = str + model.img;
                    // 判断文件是否存在,并删除
                    isFile = System.IO.File.Exists(path);
                    if (isFile)
                    {
                        // 删除文件
                        System.IO.File.Delete(path);
                    }
                   
                }
            }
            return ResultSuccess("删除成功");

        }

        public string Save(string JsonModel) {
            try
            {
                JsonModel = Server.UrlDecode(JsonModel);
                LunboInfo model = JsonConvert.DeserializeObject<LunboInfo>(JsonModel);
                bool b = false;
                if (model.no == -1)
                {
                    b = lunboObj.Add(model);
                }
                else
                {
                    b = lunboObj.Update(model);
                }
                if (b)
                {
                    return ResultSuccess("提交成功");
                }
                else
                {
                    return ResultError("提交失败");
                }
            }
            catch
            {
                return ResultError("提交失败");
            }
        }

        public string ListData() {
            DataTable dt = lunboObj.GetDataList("","");
            return ResultInfo("1", "1", JsonHelper.DataTable2Json(dt));
        }

        [HttpPost]
        public string UploadImg(int no, string imagesrc)
        {
            HttpRequest request = System.Web.HttpContext.Current.Request;

            string returnValue = string.Empty;

            string outImg = string.Empty;
            bool uloadFlag = new Upload().UploadImg(request, out outImg, "lunbo");

            if (uloadFlag)
            { //上传成功
              //修改
                if (no != -1)
                {
                    LunboInfo model = lunboObj.GetEntity(no);

                    #region 删除原有图片
                    // 已知查询到的文件相对路径为file_path// 获取程序物理路径
                    string str = System.Web.HttpRuntime.AppDomainAppPath.ToString();
                    bool isFile = false;
                    // 路径拼接,获取文件在服务器或本地的全部路径
                    var path = str + model.img;
                    // 判断文件是否存在,并删除
                    isFile = System.IO.File.Exists(path);
                    if (isFile)
                    {
                        // 删除文件
                        System.IO.File.Delete(path);
                    }
                    #endregion

                    //更新路径
                    model.img = outImg;
                    lunboObj.Update(model);


                }
                else
                {
                    if (!string.IsNullOrEmpty(imagesrc))
                    {
                        #region 删除原有图片
                        // 已知查询到的文件相对路径为file_path// 获取程序物理路径
                        string str = System.Web.HttpRuntime.AppDomainAppPath.ToString();
                        bool isFile = false;
                        // 路径拼接,获取文件在服务器或本地的全部路径
                        var path = str + imagesrc;
                        // 判断文件是否存在,并删除
                        isFile = System.IO.File.Exists(path);
                        if (isFile)
                        {
                            // 删除文件
                            System.IO.File.Delete(path);
                        }
                        #endregion
                    }
                }
                returnValue = "{\"code\": \"0\" ,\"msg\": \"上传成功！\",\"data\":{\"src\":\"" + outImg + "\"} }";
            }
            else
            {
                returnValue = "{\"code\": \"1\" ,\"msg\": \"上传失败！\",\"data\":{\"src\":\"" + outImg + "\"} }";
            }

            return ResultSuccess(returnValue);
           
        }
    }
}