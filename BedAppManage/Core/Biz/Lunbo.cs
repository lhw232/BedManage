using BedAppManage.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace BedAppManage.Core.Biz
{
    /// <summary>
    /// 轮播图业务处理类；
    /// </summary>
    public class Lunbo
    {
        const string CODE_PATH = "Core.Lunbo.";
        DAL.Lunbo lunboObj = null;

        public Lunbo()
        {
            lunboObj = new DAL.Lunbo();
        }

        #region Add 添加
        /// <summary>
        /// 添加；
        /// </summary>
        /// <param name="entity">轮播图实体对象</param>
		/// <param name="errors">添加失败的原因（仅当添加失败时有效）；</param>
		/// <param name="args">扩展参数（不对业务处理构成影响）；</param>
		/// <returns>如果插入成功，则返回true，否则，返回false</returns>
        public bool Add(LunboInfo entity, params object[] args)
        {

            try
            {
                return lunboObj.Insert(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(CODE_PATH + "Lunbo.Add(...):" + ex.Message);
            }
        }
        #endregion

        #region Delete 删除
        /// <summary>
        /// 删除；
        /// </summary>
		/// <param name="no"></param>
		/// <param name="errors">删除失败的原因（仅当删除失败时有效）；</param>
		/// <param name="args">扩展参数（不对业务处理构成影响）；</param>
		/// <returns>如果删除成功，则返回轮播图代码，否则，返回空字符串</returns>
        public bool Delete(int no, params object[] args)
        {
            try
            {
                return lunboObj.Delete(no);
            }
            catch (Exception ex)
            {
                throw new Exception(CODE_PATH + "Lunbo.Delete(...):" + ex.Message);
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
        public bool Update(LunboInfo entity,  params object[] args)
        {

            try
            {
                return lunboObj.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(CODE_PATH + "Lunbo.Update(...):" + ex.Message);
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
                return lunboObj.ExistsByID(no);
            }
            catch (Exception ex)
            {
                throw new Exception(CODE_PATH + "Lunbo.ExistsByID(...):" + ex.Message);
            }
        }
        #endregion		
        #region GetEntity 得到实体
        /// <summary>
        /// 得到实体；
        /// </summary>
		/// <param name="no"></param>
		/// <returns>轮播图实体</returns>
        public LunboInfo GetEntity(int no)
        {
            try
            {
                return lunboObj.GetEntity(no);
            }
            catch (Exception ex)
            {
                throw new Exception(CODE_PATH + "Lunbo.GetEntity(...):" + ex.Message);
            }
        }
        #endregion

        #region GetList 得到轮播图数据集
        /// <summary>
        /// 得到轮播图数据集；
        /// </summary>
        /// <param name="sqlCondition">查询条件</param>
		/// <param name="orderBy">排序依据（例如：ORDER BY ID DESC）</param>
		/// <returns>轮播图数据集</returns>
        public DataTable GetDataList(string sqlCondition, string orderBy)
        {
            try
            {
                DataTable dtb = lunboObj.GetDataList(sqlCondition, orderBy);
                return dtb;
            }
            catch (Exception ex)
            {
                throw new Exception(CODE_PATH + "Lunbo.GetList(...):" + ex.Message);
            }
        }
        #endregion
    } //class end.
}