using BedAppManage.Models;
using HW.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace BedAppManage.Core.Biz
{
    /// <summary>
    /// 商品表业务处理类；
    /// </summary>
    public class Goods : baseHandlerAPI
    {
        const string CODE_PATH = "Core.Goods.";
        DAL.Goods goodsObj = null;

        public Goods()
        {
            goodsObj = new DAL.Goods();
        }

        #region 上传图片
        public string UploadImg(HttpRequest request, int no, string img, string up_image_src)
        {
            string returnValue = string.Empty;

            string outImg = string.Empty;
            bool uloadFlag = new Upload().UploadImgByImgSrc(up_image_src, out outImg,"goods");

            if (uloadFlag)
            { //上传成功
              //修改
                if (no != -1)
                {
                    GoodsInfo model = GetEntity(no);

                    #region 删除原有图片
                    // 已知查询到的文件相对路径为file_path// 获取程序物理路径
                    string str = System.Web.HttpRuntime.AppDomainAppPath.ToString();
                    bool isFile = false;
                    // 路径拼接,获取文件在服务器或本地的全部路径
                    var path = str + model.img1;
                    // 判断文件是否存在,并删除
                    isFile = System.IO.File.Exists(path);
                    if (isFile)
                    {
                        // 删除文件
                        System.IO.File.Delete(path);
                    }
                    #endregion

                    //更新路径
                    model.img1 = outImg;
                    //Update(model);


                }
                else
                {
                    if (!string.IsNullOrEmpty(img))
                    {
                        #region 删除原有图片
                        // 已知查询到的文件相对路径为file_path// 获取程序物理路径
                        string str = System.Web.HttpRuntime.AppDomainAppPath.ToString();
                        bool isFile = false;
                        // 路径拼接,获取文件在服务器或本地的全部路径
                        var path = str + img;
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
                var obj = new
                {
                    code = 1,
                    msg = "上传成功！",
                    src= outImg
                };
                returnValue = JsonHelper.ToJson(obj);
            }
            else
            {
                var obj = new
                {
                    code = 1,
                    msg = "上传失败！",
                    src = outImg
                };
                returnValue = JsonHelper.ToJson(obj);
            }


            return returnValue;


        }

        #endregion

        #region Add 添加
        /// <summary>
        /// 添加；
        /// </summary>
        public string Add(GoodsInfo model)
        {
            try
            {

                bool b = goodsObj.Insert(model);
                if (b)
                {
                    return ResultSuccess("添加成功！");
                }
                else
                {
                    return ResultError("添加失败！");
                }
            }
            catch (Exception ex)
            {
                return ResultError("添加失败！");
            }
        }
        #endregion

        #region Delete 删除
        public class NosInfo
        {
            public string no { get; set; }
            public string img { get; set; }
        }
        public string Delete(string nos)
        {

            try
            {
                List<NosInfo> list = JsonConvert.DeserializeObject<List<NosInfo>>(nos);

                foreach (NosInfo model in list)
                {
                    if (!string.IsNullOrEmpty(model.no))
                    {
                        GoodsInfo goodInfo = GetEntity(Convert.ToInt32(model.no));

                        DeleFile(goodInfo.img1);
                        DeleFile(goodInfo.img2);
                        DeleFile(goodInfo.img3);

                        goodsObj.Delete(Convert.ToInt32(model.no));
                        
                    }
                }
                return ResultSuccess("删除成功！");
            }
            catch (Exception ex)
            {
                return ResultError("删除成功！"+ ex.Message);
            }
        }
        void DeleFile(string imsrc) {
            // 已知查询到的文件相对路径为file_path// 获取程序物理路径
            string str = System.Web.HttpRuntime.AppDomainAppPath.ToString();
            bool isFile = false;
            // 路径拼接,获取文件在服务器或本地的全部路径

            var path = str + imsrc;
            // 判断文件是否存在,并删除
            isFile = System.IO.File.Exists(path);
            if (isFile)
            {
                // 删除文件
                System.IO.File.Delete(path);
            }
        }
        #endregion		

        #region Update 更新
        /// <summary>
        /// 更新；
        /// </summary>
        /// <param name="entity">实体对象</param>
		/// <param name="errors">删除失败的原因（仅当删除失败时有效）；</param>
		/// <param name="args">扩展参数（不对业务处理构成影响）；</param>
		/// <returns>如果更新成功，则返回true，否则，返回false</returns>
        public string Update(GoodsInfo entity)
        {
            try
            {
                bool b = goodsObj.Update(entity);
                if (b)
                {
                    return ResultSuccess("修改成功！");
                }
                else
                {
                    return ResultError("修改失败！");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(CODE_PATH + "Goods.Update(...):" + ex.Message);
            }
        }
        #endregion

        #region ExistsByID 判断系统中是否存在具有相同ID号的记录；
        /// <summary>
        /// 判断系统中是否存在具有相同ID号的记录；
        /// </summary>
        /// <param name="no"></param>
        /// <returns></returns>
        public bool ExistsByID(int no)
        {
            try
            {
                return goodsObj.ExistsByID(no);
            }
            catch (Exception ex)
            {
                throw new Exception(CODE_PATH + "Goods.ExistsByID(...):" + ex.Message);
            }
        }
        #endregion		

        #region GetEntity 得到实体
        /// <summary>
        /// 得到实体；
        /// </summary>
		/// <param name="no"></param>
		/// <returns>商品表实体</returns>
        public GoodsInfo GetEntity(int no)
        {
            try
            {
                return goodsObj.GetEntity(no);
            }
            catch (Exception ex)
            {
                throw new Exception(CODE_PATH + "Goods.GetEntity(...):" + ex.Message);
            }
        }
        #endregion

        #region GetList 得到商品表数据集
        /// <summary>
        /// 得到商品表数据集；
        /// </summary>
        /// <param name="sqlCondition">查询条件</param>
		/// <param name="orderBy">排序依据（例如：ORDER BY ID DESC）</param>
		/// <returns>商品表数据集</returns>
        public DataTable GetDataList(string sqlCondition, string orderBy)
        {
            try
            {
                DataTable dtb = goodsObj.GetDataList(sqlCondition, orderBy);
                return dtb;
            }
            catch (Exception ex)
            {
                throw new Exception(CODE_PATH + "Goods.GetList(...):" + ex.Message);
            }
        }
        public string GetDataList(int page, int limit) {
            int recordCount = 0;
            int pageCount = 0;
            DataTable dt = goodsObj.GetDataList(page, limit, "", " no asc", out recordCount, out pageCount);


            return ResultInfo("1", "1", JsonHelper.DataTableToJson(dt, page, recordCount));
        }
        #endregion
    } //class end.
}