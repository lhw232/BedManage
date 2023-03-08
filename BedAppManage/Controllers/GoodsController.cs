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
    public class GoodsController : baseHandlerAPI
    {
        // GET: Goods
        [Help.ActionFilter]
        public ActionResult List()
        {
            return View();
        }
        public ActionResult Add()
        {
            GoodsInfo model = new GoodsInfo();
            model.no = -1;
            ViewData["GoodsInfo"] = model;

            DataTable dt_types = new Types().GetDataList("","");
            //DataRow dr = dt_types.NewRow();
            //dr["no"] = -1;
            //dr["name"] = "请选择";
            //dt_types.Rows.Add(dr);
            ViewData["TypesList"] = dt_types;
            return View();
        }
        public ActionResult Edit(string JsonModel)
        {
            GoodsInfo model = JsonConvert.DeserializeObject<GoodsInfo>(JsonModel);
            ViewData["GoodsInfo"] = model;

            DataTable dt_types = new Types().GetDataList("", "");
            //DataRow dr = dt_types.NewRow();
            //dr["no"] = -1;
            //dr["name"] = "请选择";
            //dt_types.Rows.Add(dr);
            ViewData["TypesList"] = dt_types;
            return View("Add");
        }
        public string ListData(int page, int limit)
        {
            return new Goods().GetDataList(page, limit);
        }
        public string goodList(string typeNo)
        {
            DataTable dt= new Goods().GetDataList(" typeNo='"+typeNo+"'","");
            return ResultInfo("1", "1", JsonHelper.DataTable2Json(dt));
        }

        public string Save(string JsonModel)
        {
            try
            {
                JsonModel = Server.UrlDecode(JsonModel);
                GoodsInfo model = JsonConvert.DeserializeObject<GoodsInfo>(JsonModel);

                if (model.no == -1)
                {
                    return new Goods().Add(model);
                }
                else
                {
                    return new Goods().Update(model);
                }

            }
            catch {
                return "{_OP_FLAG=/'0/'}";
            }

        }



        [HttpPost]
        public string UploadImg(int no, string del_image_src,string up_image_src)
        {
            
            HttpRequest request = System.Web.HttpContext.Current.Request;


            return new Goods().UploadImg(request, no, del_image_src, up_image_src);
        }

        #region 删除
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="nos"></param>
        /// <returns></returns>
        public string Del(string nos)
        {
            return new Goods().Delete(nos);
        }
        #endregion

        #region 获取商品详情
        public string GetGoodsDetails(string no) {
            GoodsInfo model = new Goods().GetEntity(Convert.ToInt32(no));
            return ResultInfo("1","",JsonHelper.ToJson(model));
        }
        #endregion

    }
}