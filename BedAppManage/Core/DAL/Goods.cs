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
    /// 商品表数据操作类；
    /// </summary>
    public class Goods
    {
        const string CODE_PATH = "DAL.Goods.";
        SqlParameter[] parms = null;

        string SQL_INSERT = "INSERT INTO Goods(typeNo,name,price,falsePrice,img1,img2,img3)VALUES(@typeNo,@name,@price,@falsePrice,@img1,@img2,@img3)";
        string SQL_DELETE = "DELETE FROM Goods WHERE no=@no";
        string SQL_UPDATE = "UPDATE Goods SET typeNo=@typeNo,name=@name,price=@price,falsePrice=@falsePrice,img1=@img1,img2=@img2,img3=@img3 WHERE no=@no";
        string SQL_GETENTITY = "SELECT * FROM Goods WHERE no=@no";
        string SQL_EXISTS_BY_ID = "SELECT * FROM Goods WHERE no=@no";
        string SQL_EXISTS_BY_NAME = "SELECT * FROM Goods WHERE name=@name";
        string SQL_GETLIST = "SELECT * FROM Goods";
        string PRO_GETLIST = "pro_getList_goods";

        #region Insert 插入
        /// <summary>
        /// 插入；
        /// </summary>
        /// <param name="entity">商品表实体对象</param>
		/// <returns>如果插入成功，则返回true，否则，返回false</returns>
        public bool Insert(GoodsInfo entity)
        {
            try
            {
                parms = GetParametersForAdd(entity);
                int affectedRows = SQLHelper.ExecuteNonQuery(DBConfig.ConnectionString, CommandType.Text, SQL_INSERT, parms);

                return affectedRows > 0 ? true : false;
            }
            catch (Exception ex)
            {
                throw new Exception(CODE_PATH + "Goods.Insert(...):" + ex.Message);
            }
        }
        #endregion

        #region Update 更新
        /// <summary>
        /// 更新；
        /// </summary>
        /// <param name="entity">商品表实体对象</param>
        /// <returns>如果更新成功，则返回true，否则，返回false</returns>
        public bool Update(GoodsInfo entity)
        {
            try
            {
                parms = GetParametersForUpdate(entity);
                int affectedRows = SQLHelper.ExecuteNonQuery(DBConfig.ConnectionString, CommandType.Text, SQL_UPDATE, parms);

                return affectedRows > 0 ? true : false;
            }
            catch (Exception ex)
            {
                throw new Exception(CODE_PATH + "Goods.Update(...):" + ex.Message);
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
                throw new Exception(CODE_PATH + "Goods.Delete(...):" + ex.Message);
            }
        }
        #endregion



        #region GetEntity 得到商品表实体
        /// <summary>
        /// 得到商品表实体；
        /// </summary>
        /// <param name="no"></param>
        /// <returns>商品表实体</returns>
        public GoodsInfo GetEntity(int no)
        {
            GoodsInfo entity = null;


            try
            {
                SqlParameter[] parms = GetKeyParameter(no);
                SqlDataReader dr = SQLHelper.ExecuteReader(DBConfig.ConnectionString, CommandType.Text, SQL_GETENTITY, parms);
                if (dr.Read())
                {
                    entity = new GoodsInfo();
                    entity.no = Convert.ToInt32(dr["no"]);
                    entity.typeNo = Convert.ToInt32(dr["typeNo"]);
                    entity.name = Convert.ToString(dr["name"]);
                    entity.price = Convert.ToString(dr["price"]);
                    entity.falsePrice = Convert.ToString(dr["falsePrice"]);
                    entity.img1 = Convert.ToString(dr["img1"]);
                    entity.img2 = Convert.ToString(dr["img2"]);
                    entity.img3 = Convert.ToString(dr["img3"]);
                }
                dr.Close();

                return entity;
            }
            catch (Exception ex)
            {
                throw new Exception(CODE_PATH + "Goods.GetEntity(...):" + ex.Message);
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
                throw new Exception(CODE_PATH + "Goods.GetDataList(...):" + ex.Message);
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
                throw new Exception(CODE_PATH + "Goods.GetDataList(...):" + ex.Message);
            }
        }
        #endregion

        #region ExistsByID 判断系统中是否存在具有相同ID的商品表
        /// <summary>
        /// 判断系统中是否存在具有相同ID的商品表；
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
                throw new Exception(CODE_PATH + "Goods.ExistsByID(...):" + ex.Message);
            }
        }
        #endregion

        #region ExistsByName 判断系统中是否存在具有相同名称（或标题）的商品表
        /// <summary>
        /// 判断系统中是否存在具有相同名称（或标题）的商品表；
        /// </summary>
        /// <param name="name"></param>
        /// <returns>如果存在，则返回true，否则，返回false</returns>
        public bool ExistsByName(string name)
        {
            bool result = false;

            try
            {
                SqlParameter[] parms = new SqlParameter[]
                {
                    SQLHelper.MakeInParam("@name", SqlDbType.VarChar, 20, name)
                };

                SqlDataReader dr = SQLHelper.ExecuteReader(DBConfig.ConnectionString, CommandType.Text, SQL_EXISTS_BY_NAME, parms);
                if (dr.Read())
                {
                    result = true;
                }
                dr.Close();

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(CODE_PATH + "Goods.ExistsByName(...):" + ex.Message);
            }
        }
        #endregion

        #region GetParameters、GetKeyParameter 获取参数数组
        /// <summary>
        /// 获取参数数组；
        /// </summary>
        /// <param name=" entity">实体</param>
        /// <returns>SqlParameter</returns>
        SqlParameter[] GetParametersForAdd(GoodsInfo entity)
        {
            return new SqlParameter[]
                {
                    SQLHelper.MakeInParam("@no", SqlDbType.Int, entity.no),
                    SQLHelper.MakeInParam("@typeNo", SqlDbType.Int, entity.typeNo),
                    SQLHelper.MakeInParam("@name", SqlDbType.VarChar, 20, entity.name),
                    SQLHelper.MakeInParam("@price", SqlDbType.VarChar, 20, entity.price),
                    SQLHelper.MakeInParam("@falsePrice", SqlDbType.VarChar, 20, entity.falsePrice),
                    SQLHelper.MakeInParam("@img1", SqlDbType.VarChar, 100, entity.img1),
                    SQLHelper.MakeInParam("@img2", SqlDbType.VarChar, 100, entity.img2),
                    SQLHelper.MakeInParam("@img3", SqlDbType.VarChar, 100, entity.img3)
                };
        }


        /// <summary>
        /// 获取参数数组；
        /// </summary>
        /// <param name=" entity">实体</param>
        /// <returns>SqlParameter</returns>
        SqlParameter[] GetParametersForUpdate(GoodsInfo entity)
        {
            return new SqlParameter[]
                {
                    SQLHelper.MakeInParam("@typeNo", SqlDbType.Int, entity.typeNo),
                    SQLHelper.MakeInParam("@name", SqlDbType.VarChar, 20, entity.name),
                    SQLHelper.MakeInParam("@price", SqlDbType.VarChar, 20, entity.price),
                    SQLHelper.MakeInParam("@falsePrice", SqlDbType.VarChar, 20, entity.falsePrice),
                    SQLHelper.MakeInParam("@img1", SqlDbType.VarChar, 100, entity.img1),
                    SQLHelper.MakeInParam("@img2", SqlDbType.VarChar, 100, entity.img2),
                    SQLHelper.MakeInParam("@img3", SqlDbType.VarChar, 100, entity.img3),
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