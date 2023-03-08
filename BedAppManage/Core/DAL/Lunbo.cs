using BedAppManage.Models;
using HW.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace BedAppManage.Core.DAL
{
    /// <summary>
    /// 轮播图数据操作类；
    /// </summary>
    public class Lunbo
    {
        const string CODE_PATH = "DAL.Lunbo.";
        SqlParameter[] parms = null;

        string SQL_INSERT = "INSERT INTO Lunbo(img,orderNo)VALUES(@img,@orderNo)";
        string SQL_DELETE = "DELETE FROM Lunbo WHERE no=@no";
        string SQL_UPDATE = "UPDATE Lunbo SET img=@img,orderNo=@orderNo WHERE no=@no";
        string SQL_GETENTITY = "SELECT * FROM Lunbo WHERE no=@no";
        string SQL_EXISTS_BY_ID = "SELECT * FROM Lunbo WHERE no=@no";
        string SQL_EXISTS_BY_NAME = "SELECT * FROM Lunbo WHERE ";
        string SQL_GETLIST = "SELECT * FROM Lunbo";
        string PRO_GETLIST = " ";

        #region Insert 插入
        /// <summary>
        /// 插入；
        /// </summary>
        /// <param name="entity">轮播图实体对象</param>
		/// <returns>如果插入成功，则返回true，否则，返回false</returns>
        public bool Insert(LunboInfo entity)
        {
            try
            {
                parms = GetParametersForAdd(entity);
                int affectedRows = SQLHelper.ExecuteNonQuery(DBConfig.ConnectionString, CommandType.Text, SQL_INSERT, parms);

                return affectedRows > 0 ? true : false;
            }
            catch (Exception ex)
            {
                throw new Exception(CODE_PATH + "Lunbo.Insert(...):" + ex.Message);
            }
        }
        #endregion

        #region Update 更新
        /// <summary>
        /// 更新；
        /// </summary>
        /// <param name="entity">轮播图实体对象</param>
        /// <returns>如果更新成功，则返回true，否则，返回false</returns>
        public bool Update(LunboInfo entity)
        {
            try
            {
                parms = GetParametersForUpdate(entity);
                int affectedRows = SQLHelper.ExecuteNonQuery(DBConfig.ConnectionString, CommandType.Text, SQL_UPDATE, parms);

                return affectedRows > 0 ? true : false;
            }
            catch (Exception ex)
            {
                throw new Exception(CODE_PATH + "Lunbo.Update(...):" + ex.Message);
            }
        }
        #endregion

        #region Delete 删除
        /// <summary>
        /// 删除；
        /// </summary>
        /// <param name="no"></param>
        /// <returns>如果删除成功，则返回true，否则，返回false</returns>
        public bool Delete(int no)
        {
            try
            {
                SqlParameter[] parms = GetKeyParameter(no);
                int affectedRows = SQLHelper.ExecuteNonQuery(DBConfig.ConnectionString, CommandType.Text, SQL_DELETE, parms);

                return affectedRows > 0 ? true : false;
            }
            catch (Exception ex)
            {
                throw new Exception(CODE_PATH + "Lunbo.Delete(...):" + ex.Message);
            }
        }
        #endregion



        #region GetEntity 得到轮播图实体
        /// <summary>
        /// 得到轮播图实体；
        /// </summary>
        /// <param name="no"></param>
        /// <returns>轮播图实体</returns>
        public LunboInfo GetEntity(int no)
        {
            LunboInfo entity = null;


            try
            {
                SqlParameter[] parms = GetKeyParameter(no);
                SqlDataReader dr = SQLHelper.ExecuteReader(DBConfig.ConnectionString, CommandType.Text, SQL_GETENTITY, parms);
                if (dr.Read())
                {
                    entity = new LunboInfo();
                    entity.no = Convert.ToInt32(dr["no"]);
                    entity.img = Convert.ToString(dr["img"]);
                    entity.orderNo = Convert.ToInt32(dr["orderNo"]);
                }
                dr.Close();

                return entity;
            }
            catch (Exception ex)
            {
                throw new Exception(CODE_PATH + "Lunbo.GetEntity(...):" + ex.Message);
            }
        }
        #endregion

        #region GetDataList 得到数据集
        /// <summary>
        /// 得到数据集；
        /// </summary>		
        /// <param name="sqlFilter">SQL筛选器。例如："NAME LIKE'%张三%' AND SEX='1'"</param>
		/// <param name="orderBy">排序依据。例如："ID DESC"</param>
        /// <returns>数据集（DataTable）</returns>
        public DataTable GetDataList(string sqlFilter, string orderBy)
        {
            sqlFilter = sqlFilter == null ? String.Empty : sqlFilter;
            string tmpSqlFilter = sqlFilter.ToUpper().Trim();
            if (tmpSqlFilter.StartsWith("WHERE"))
            {
                throw new Exception("参数[sqlFilter]的值不能以\"where\"开头！");
            }
            if (tmpSqlFilter.StartsWith("AND"))
            {
                throw new Exception("参数[sqlFilter]的值不能以\"and\"开头！");
            }

            string cmdText = SQL_GETLIST;

            if (!String.IsNullOrEmpty(sqlFilter))
            {
                cmdText += " WHERE 1=1 AND " + sqlFilter;
            }

            if (!String.IsNullOrEmpty(orderBy))
            {
                cmdText += " ORDER BY " + orderBy;
            }

            try
            {
                DataSet ds = SQLHelper.ExecuteDataset(DBConfig.ConnectionString, CommandType.Text, cmdText, parms);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw new Exception(CODE_PATH + "Lunbo.GetDataList(...):" + ex.Message);
            }
        }

        /// <summary>
        /// 得到数据集；
        /// </summary>
        /// <param name="currPage">当前页码</param>
        /// <param name="pageSize">每页显示的记录条数</param>
        /// <param name="sqlFilter">SQL筛选器。例如："NAME LIKE'%张三%' AND SEX='1'"</param>
        /// <param name="orderBy">排序依据，例如："ID DESC"</param>
        /// <param name="recordCount">返回的记录总数</param>
        /// <param name="pageCount">返回的总页数</param>
        /// <returns>数据集（DataTable）</returns>
        public DataTable GetDataList(int currPage, int pageSize, string sqlFilter, string orderBy, out int recordCount, out int pageCount)
        {
            recordCount = 0;
            pageCount = 0;

            sqlFilter = sqlFilter == null ? String.Empty : sqlFilter;
            string tmpSqlFilter = sqlFilter.ToUpper().Trim();
            if (tmpSqlFilter.StartsWith("WHERE"))
            {
                throw new Exception("参数[sqlFilter]的值不能以\"where\"开头！");
            }
            if (tmpSqlFilter.StartsWith("AND"))
            {
                throw new Exception("参数[sqlFilter]的值不能以\"and\"开头！");
            }

            SqlParameter[] parms = new SqlParameter[]
            {
                SQLHelper.MakeInParam("@CURRPAGE", SqlDbType.Int, currPage),
                SQLHelper.MakeInParam("@PAGESIZE", SqlDbType.Int, pageSize),
                SQLHelper.MakeInParam("@SQLFILTER", SqlDbType.VarChar, sqlFilter),
                SQLHelper.MakeInParam("@ORDERBY", SqlDbType.VarChar, orderBy),
                SQLHelper.MakeOutParam("@ROWCOUNT", SqlDbType.Int)
            };

            try
            {
                DataSet ds = SQLHelper.ExecuteDataset(DBConfig.ConnectionString, CommandType.StoredProcedure, PRO_GETLIST, parms);

                //recordCount = 0;
                //pageCount = 0;

                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw new Exception(CODE_PATH + "Lunbo.GetDataList(...):" + ex.Message);
            }
        }
        #endregion

        #region ExistsByID 判断系统中是否存在具有相同ID的轮播图
        /// <summary>
        /// 判断系统中是否存在具有相同ID的轮播图；
        /// </summary>
        /// <param name="no"></param>
        /// <returns>如果存在，则返回true，否则，返回false</returns>
        public bool ExistsByID(int no)
        {
            bool result = false;

            try
            {
                SqlParameter[] parms = GetKeyParameter(no);
                SqlDataReader dr = SQLHelper.ExecuteReader(DBConfig.ConnectionString, CommandType.Text, SQL_EXISTS_BY_ID, parms);
                if (dr.Read())
                {
                    result = true;
                }
                dr.Close();

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(CODE_PATH + "Lunbo.ExistsByID(...):" + ex.Message);
            }
        }
        #endregion

        #region GetParameters、GetKeyParameter 获取参数数组
        /// <summary>
        /// 获取参数数组；
        /// </summary>
        /// <param name=" entity">实体</param>
        /// <returns>SqlParameter</returns>
        SqlParameter[] GetParametersForAdd(LunboInfo entity)
        {
            return new SqlParameter[]
                {
                    SQLHelper.MakeInParam("@no", SqlDbType.Int, entity.no),
                    SQLHelper.MakeInParam("@img", SqlDbType.VarChar, 150, entity.img),
                    SQLHelper.MakeInParam("@orderNo", SqlDbType.Int, entity.orderNo)
                };
        }


        /// <summary>
        /// 获取参数数组；
        /// </summary>
        /// <param name=" entity">实体</param>
        /// <returns>SqlParameter</returns>
        SqlParameter[] GetParametersForUpdate(LunboInfo entity)
        {
            return new SqlParameter[]
                {
                    SQLHelper.MakeInParam("@img", SqlDbType.VarChar, 150, entity.img),
                    SQLHelper.MakeInParam("@orderNo", SqlDbType.Int, entity.orderNo),
                    SQLHelper.MakeInParam("@no", SqlDbType.Int, entity.no)
                };
        }

        /// <summary>
        /// 获取主键参数数组；
        /// </summary>
        /// <param name="no"></param>
        /// <returns></returns>
        SqlParameter[] GetKeyParameter(int no)
        {
            return new SqlParameter[]
                {
                    SQLHelper.MakeInParam("@no", SqlDbType.Int, no)
                };
        }
        #endregion

    } //class end.
}