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
    public class MembersController : baseHandlerAPI
    {
        #region 公共变量
        Members membersObj = new Members();
        #endregion
        // GET: Members
        [Help.ActionFilter]
        public ActionResult List()
        {
            return View();
        }

        [Help.ActionFilter]
        public ActionResult Add()
        {
            MembersInfo model = new MembersInfo();
            model.no = -1;
            ViewData["MembersInfo"] = model;
            

            return View();
        }
        [Help.ActionFilter]
        public ActionResult Edit(string JsonModel)
        {
            MembersInfo model = JsonConvert.DeserializeObject<MembersInfo>(JsonModel);
            model = new Members().GetEntity(model.no);
            ViewData["MembersInfo"] = model;
            return View("Add");
        }
        public string ListData()
        {
            DataTable dt = membersObj.GetDataList("", "");
            return ResultInfo("1", "1", JsonHelper.DataTable2Json(dt));
        }

        [HttpPost]
        public string UploadImg(int no, string imagesrc)
        {
            HttpRequest request = System.Web.HttpContext.Current.Request;

            string returnValue = string.Empty;

            string outImg = string.Empty;
            bool uloadFlag = new Upload().UploadImg(request, out outImg, "members");

            if (uloadFlag)
            { //上传成功
              //修改
                if (no != -1)
                {
                    MembersInfo model = membersObj.GetEntity(no);

                    #region 删除原有图片
                    // 已知查询到的文件相对路径为file_path// 获取程序物理路径
                    string str = System.Web.HttpRuntime.AppDomainAppPath.ToString();
                    bool isFile = false;
                    // 路径拼接,获取文件在服务器或本地的全部路径
                    var path = str + model.faceurl;
                    // 判断文件是否存在,并删除
                    isFile = System.IO.File.Exists(path);
                    if (isFile)
                    {
                        // 删除文件
                        System.IO.File.Delete(path);
                    }
                    #endregion

                    //更新路径
                    model.faceurl = outImg;
                    membersObj.Update(model);


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