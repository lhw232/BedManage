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
    public class TypeController : Controller 
    {

        // GET: Type
        [Help.ActionFilter]
        public ActionResult List()
        {
            return View();
        }
        public ActionResult Add() {
            TypesInfo model = new TypesInfo();
            model.no = -1;
            ViewData["TypesInfo"] = model;
            return View();
        }
        public ActionResult Edit(string JsonModel)
        {
            TypesInfo model = JsonConvert.DeserializeObject<TypesInfo>(JsonModel);
            ViewData["TypesInfo"] = model;
            return View("Add");
        }

        public string Save(string JsonModel) {
            return new Types().Save(JsonModel);
        }

        public string Del(string nos)
        {
            return new Types().Del(nos);
        }
        public string Update(string jsonModel) {
            return new Types().Update(jsonModel);
        }

        [HttpPost]
        public string UploadImg(int no,string imagesrc) {
            HttpRequest request = System.Web.HttpContext.Current.Request;
            return new Types().UploadImg(request, no, imagesrc);
        }

        public string ListData(int page, int limit) {
            return new Types().getList(page, limit);
        }
    }
}