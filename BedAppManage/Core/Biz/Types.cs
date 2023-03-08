using BedAppManage.Models;
using HW.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;

namespace BedAppManage.Core.Biz
{
    /// <summary>
    /// 商品分类表业务处理类；
    /// </summary>
    public class Types : baseHandlerAPI
    {
        const string CODE_PATH = "Core.Types.";
        DAL.Types typesObj = null;

        public Types()
        {
            typesObj = new DAL.Types();
        }

        public string getList(int pageIndex,int limit)
        {
            int recordCount = 0;
            int pageCount = 0;
            DataTable dt = typesObj.GetDataList(pageIndex, limit,""," no asc", out recordCount, out pageCount);


            return ResultInfo("1", "1", JsonHelper.DataTableToJson(dt, pageIndex, recordCount));

        }

        public string UploadImg(HttpRequest request,int no,string img)
        {
            string returnValue = string.Empty;

            string outImg = string.Empty;
            bool uloadFlag = new Upload().UploadImg(request,out outImg,"types");

            if (uloadFlag)
            { //上传成功
              //修改
                if (no != -1)
                {
                    TypesInfo model = GetEntity(no);

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
                    Update(model);


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
                 returnValue = "{\"code\": \"0\" ,\"msg\": \"上传成功！\",\"data\":{\"src\":\"" + outImg + "\"} }";
            }
            else {
                 returnValue = "{\"code\": \"1\" ,\"msg\": \"上传失败！\",\"data\":{\"src\":\"" + outImg + "\"} }";
            }
          
            
            return ResultSuccess(returnValue);


        }

        public string Save(string JsonModel) {
            try
            {
                JsonModel = HttpUtility.UrlDecode(JsonModel);
                TypesInfo model = JsonConvert.DeserializeObject<TypesInfo>(JsonModel);

                Add(model);
                return ResultSuccess("添加成功！");
            }
            catch (Exception ex)
            {
                return ResultError("添加失败," + ex.Message);
            }
        }

        #region Add 添加
        /// <summary>
        /// 添加；
        /// </summary>
        /// <param name="entity">商品分类表实体对象</param>
        /// <param name="errors">添加失败的原因（仅当添加失败时有效）；</param>
        /// <param name="args">扩展参数（不对业务处理构成影响）；</param>
        /// <returns>如果插入成功，则返回true，否则，返回false</returns>
        public bool Add(TypesInfo entity, params object[] args)
        {

            try
            {
                return typesObj.Insert(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(CODE_PATH + "Types.Add(...):" + ex.Message);
            }
        }
        #endregion

        #region Delete 删除

        public class NosInfo
        {
            public string no { get; set; }
            public string img { get; set; }
        }

        public string Del(string nos)
        {
            List<NosInfo> list = JsonConvert.DeserializeObject<List<NosInfo>>(nos);

            foreach (NosInfo model in list) {
                if (!string.IsNullOrEmpty(model.no))
                {
                    Delete(Convert.ToInt32(model.no));

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
            return ResultSuccess("删除成功！");

        }

        /// <summary>
        /// 删除；
        /// </summary>
		/// <param name="no">分类编号</param>
		/// <param name="errors">删除失败的原因（仅当删除失败时有效）；</param>
		/// <param name="args">扩展参数（不对业务处理构成影响）；</param>
		/// <returns>如果删除成功，则返回商品分类表代码，否则，返回空字符串</returns>
        public bool Delete(int no,  params object[] args)
        {

            try
            {
                return typesObj.Delete(no);
            }
            catch (Exception ex)
            {
                throw new Exception(CODE_PATH + "Types.Delete(...):" + ex.Message);
            }
        }
        #endregion		

        #region Update 更新

        public string Update(string JsonModel)
        {
            JsonModel = HttpUtility.UrlDecode(JsonModel);
            TypesInfo model = JsonConvert.DeserializeObject<TypesInfo>(JsonModel);


            Update(model);
            return ResultSuccess("修改成功！");

        }

        /// <summary>
        /// 更新；
        /// </summary>
        /// <param name="entity">实体对象</param>
		/// <param name="errors">删除失败的原因（仅当删除失败时有效）；</param>
		/// <param name="args">扩展参数（不对业务处理构成影响）；</param>
		/// <returns>如果更新成功，则返回true，否则，返回false</returns>
        public bool Update(TypesInfo entity,  params object[] args)
        {

            try
            {
                return typesObj.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(CODE_PATH + "Types.Update(...):" + ex.Message);
            }
        }
        #endregion

        #region ExistsByID 判断系统中是否存在具有相同ID号的记录；
        /// <summary>
        /// 判断系统中是否存在具有相同ID号的记录；
        /// </summary>
        /// <param name="no">分类编号</param>
        /// <returns></returns>
        public bool ExistsByID(int no)
        {
            try
            {
                return typesObj.ExistsByID(no);
            }
            catch (Exception ex)
            {
                throw new Exception(CODE_PATH + "Types.ExistsByID(...):" + ex.Message);
            }
        }
        #endregion		

        #region GetEntity 得到实体
        /// <summary>
        /// 得到实体；
        /// </summary>
		/// <param name="no">分类编号</param>
		/// <returns>商品分类表实体</returns>
        public TypesInfo GetEntity(int no)
        {
            try
            {
                return typesObj.GetEntity(no);
            }
            catch (Exception ex)
            {
                throw new Exception(CODE_PATH + "Types.GetEntity(...):" + ex.Message);
            }
        }
        #endregion

        #region GetList 得到商品分类表数据集
        /// <summary>
        /// 得到商品分类表数据集；
        /// </summary>
        /// <param name="sqlCondition">查询条件</param>
		/// <param name="orderBy">排序依据（例如：ORDER BY ID DESC）</param>
		/// <returns>商品分类表数据集</returns>
        public DataTable GetDataList(string sqlCondition, string orderBy)
        {
            try
            {
                DataTable dtb = typesObj.GetDataList(sqlCondition, orderBy);
                return dtb;
            }
            catch (Exception ex)
            {
                throw new Exception(CODE_PATH + "Types.GetList(...):" + ex.Message);
            }
        }
        #endregion
    } //class end.
}