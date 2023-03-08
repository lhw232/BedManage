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
    /// 用户表数据操作类；
    /// </summary>
    public class Members
    {
        const string CODE_PATH = "DAL.Members.";
        SqlParameter[] parms = null;

        string SQL_INSERT = "INSERT INTO Members(phone,nickname,faceurl,sex,birthday,state,passWord)VALUES(@phone,@nickname,@faceurl,@sex,@birthday,@state,@passWord)";
        string SQL_DELETE = "DELETE FROM Members WHERE no=@no";
        string SQL_UPDATE = "UPDATE Members SET no=@no,phone=@phone,nickname=@nickname,faceurl=@faceurl,sex=@sex,birthday=@birthday,state=@state,passWord=@passWord WHERE no=@no";
        string SQL_GETENTITY = "SELECT * FROM Members WHERE no=@no";
        string SQL_EXISTS_BY_ID = "SELECT * FROM Members WHERE no=@no";
        string SQL_EXISTS_BY_NAME = "SELECT * FROM Members WHERE nickname=@nickname";
        string SQL_GETLIST = "SELECT * FROM Members";
        string PRO_GETLIST = " ";

        #region Insert 插入
        /// <summary>
        /// 插入；
        /// </summary>
        /// <param name="entity">用户表实体对象</param>
		/// <returns>如果插入成功，则返回true，否则，返回false</returns>
        public bool Insert(MembersInfo entity)
        {
            try
            {
                parms = GetParametersForAdd(entity);
                int affectedRows = SQLHelper.ExecuteNonQuery(DBConfig.ConnectionString, CommandType.Text, SQL_INSERT, parms);

                return affectedRows > 0 ? true : false;
            }
            catch (Exception ex)
            {
                throw new Exception(CODE_PATH + "User.Insert(...):" + ex.Message);
            }
        }
        #endregion

        #region Update 更新
        /// <summary>
        /// 更新；
        /// </summary>
        /// <param name="entity">用户表实体对象</param>
        /// <returns>如果更新成功，则返回true，否则，返回false</returns>
        public bool Update(MembersInfo entity)
        {
            try
            {
                parms = GetParametersForUpdate(entity);
                int affectedRows = SQLHelper.ExecuteNonQuery(DBConfig.ConnectionString, CommandType.Text, SQL_UPDATE, parms);

                return affectedRows > 0 ? true : false;
            }
            catch (Exception ex)
            {
                throw new Exception(CODE_PATH + "User.Update(...):" + ex.Message);
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
                throw new Exception(CODE_PATH + "User.Delete(...):" + ex.Message);
            }
        }
        #endregion



        #region GetEntity 得到用户表实体
        /// <summary>
        /// 得到用户表实体；
        /// </summary>
        /// <param name="no"></param>
        /// <returns>用户表实体</returns>
        public MembersInfo GetEntity(int no)
        {
            MembersInfo entity = null;


            try
            {
                SqlParameter[] parms = GetKeyParameter(no);
                SqlDataReader dr = SQLHelper.ExecuteReader(DBConfig.ConnectionString, CommandType.Text, SQL_GETENTITY, parms);
                if (dr.Read())
                {
                    entity = new MembersInfo();
                    entity.no = Convert.ToInt32(dr["no"]);
                    entity.phone = Convert.ToInt32(dr["phone"]);
                    entity.nickname = Convert.ToString(dr["nickname"]);
                    entity.faceurl = Convert.ToString(dr["faceurl"]);
                    entity.sex = Convert.ToString(dr["sex"]);
                    entity.birthday = SQLHelper.GetDateTimeNullable(dr["birthday"]);
                    entity.state = Convert.ToString(dr["state"]);
                }
                dr.Close();

                return entity;
            }
            catch (Exception ex)
            {
                throw new Exception(CODE_PATH + "User.GetEntity(...):" + ex.Message);
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
                throw new Exception(CODE_PATH + "User.GetDataList(...):" + ex.Message);
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
                throw new Exception(CODE_PATH + "User.GetDataList(...):" + ex.Message);
            }
        }
        #endregion

        #region ExistsByID 判断系统中是否存在具有相同ID的用户表
        /// <summary>
        /// 判断系统中是否存在具有相同ID的用户表；
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
                throw new Exception(CODE_PATH + "User.ExistsByID(...):" + ex.Message);
            }
        }
        #endregion

        #region ExistsByName 判断系统中是否存在具有相同名称（或标题）的用户表
        /// <summary>
        /// 判断系统中是否存在具有相同名称（或标题）的用户表；
        /// </summary>
        /// <param name="nickname"></param>
        /// <returns>如果存在，则返回true，否则，返回false</returns>
        public bool ExistsByName(string nickname)
        {
            bool result = false;

            try
            {
                SqlParameter[] parms = new SqlParameter[]
                {
                    SQLHelper.MakeInParam("@nickname", SqlDbType.VarChar, 50, nickname)
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
                throw new Exception(CODE_PATH + "User.ExistsByName(...):" + ex.Message);
            }
        }
        #endregion

        #region GetParameters、GetKeyParameter 获取参数数组
        /// <summary>
        /// 获取参数数组；
        /// </summary>
        /// <param name=" entity">实体</param>
        /// <returns>SqlParameter</returns>
        SqlParameter[] GetParametersForAdd(MembersInfo entity)
        {
            return new SqlParameter[]
                {
                    SQLHelper.MakeInParam("@no", SqlDbType.Int, entity.no),
                    SQLHelper.MakeInParam("@phone", SqlDbType.Int, entity.phone),
                    SQLHelper.MakeInParam("@nickname", SqlDbType.VarChar, 50, entity.nickname),
                    SQLHelper.MakeInParam("@faceurl", SqlDbType.VarChar, 150, entity.faceurl),
                    SQLHelper.MakeInParam("@sex", SqlDbType.VarChar, 2, entity.sex),
                    SQLHelper.MakeInParam("@birthday", SqlDbType.DateTime, entity.birthday),
                    SQLHelper.MakeInParam("@state", SqlDbType.VarChar, 2, entity.state),
                    SQLHelper.MakeInParam("@passWord", SqlDbType.VarChar, 100, entity.passWord)
                };
        }


        /// <summary>
        /// 获取参数数组；
        /// </summary>
        /// <param name=" entity">实体</param>
        /// <returns>SqlParameter</returns>
        SqlParameter[] GetParametersForUpdate(MembersInfo entity)
        {
            return new SqlParameter[]
                {
                    SQLHelper.MakeInParam("@phone", SqlDbType.Int, entity.phone),
                    SQLHelper.MakeInParam("@nickname", SqlDbType.VarChar, 50, entity.nickname),
                    SQLHelper.MakeInParam("@faceurl", SqlDbType.VarChar, 150, entity.faceurl),
                    SQLHelper.MakeInParam("@sex", SqlDbType.VarChar, 2, entity.sex),
                    SQLHelper.MakeInParam("@birthday", SqlDbType.DateTime, entity.birthday),
                    SQLHelper.MakeInParam("@state", SqlDbType.VarChar, 2, entity.state),
                    SQLHelper.MakeInParam("@passWord", SqlDbType.VarChar, 100, entity.passWord),
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