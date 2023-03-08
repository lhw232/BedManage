using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace HW.Utils
{
    public sealed class SQLHelper
    {
        #region 内部方法
        private SQLHelper()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }


        ///<summary>
        ///将参数集合添加到SQLCOMMAND对象中
        ///</summary>
        ///<param name="command">SqlCommand对象</param>
        ///<param name="commandParameter">希望添加的参数集合（数组）</param>
        private static void AttachParameters(SqlCommand command, SqlParameter[] commandParameter)
        {

            foreach (SqlParameter p in commandParameter)
            {
                //if ((p.Direction == ParameterDirection.InputOutput) && (p.Value == null))
                //{
                //    p.Value = DBNull.Value;
                //}
                //command.Parameters.Add(p);

                if (((p.Direction == ParameterDirection.InputOutput) || (p.Direction == ParameterDirection.Input)) && (p.Value == null))
                {
                    p.Value = DBNull.Value;
                }
                command.Parameters.Add(p);
            }

        }


        /// <summary>
        /// 将数组的值赋给SqlParameter对象的VALUE属性
        /// </summary>
        /// <param name="commandParameter">接收数据的SqlParameter类型的数组</param>
        /// <param name="parameterValues">传输数据的数组</param>
        private static void AssignParameterValues(SqlParameter[] commandParameter, object[] parameterValues)
        {
            if ((commandParameter == null) || (parameterValues == null))
            {
                //如果数据为空，返回NOTHING
                return;
            }

            //检验数组维数是否相同，非常必要
            if (commandParameter.Length != parameterValues.Length)
            {
                throw new ArgumentException("数组维数不同，无法完成赋值");
            }

            //进行赋值操作
            for (int i = 0, j = commandParameter.Length; i < j; i++)
            {
                commandParameter[i].Value = parameterValues[i];
            }
        }


        /// <summary>
        /// 初始化一个SqlCommand对象
        /// </summary>
        /// <param name="command">需要初始化的SqlCommand对象</param>
        /// <param name="connection">一个SQL连接对象</param>
        /// <param name="transaction">一个事务对象</param>
        /// <param name="commandtype">SQL命令的类型，COMMANDTYPE枚举类型</param>
        /// <param name="commandText">SQL命令</param>
        /// <param name="commandParameter">参数集合数组</param>
        private static void PrepareCommand(SqlCommand command, SqlConnection connection, SqlTransaction transaction, CommandType commandtype, string commandText, SqlParameter[] commandParameter)
        {
            //判断连接状态,如果未打开连接，则打开
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            //设置COMMAND的连接属性
            command.Connection = connection;

            //设置COMMANDTEXT属性
            command.CommandText = commandText;

            //设置COMMANDtype属性
            command.CommandType = commandtype;

            //设置连接超时的时间
            command.CommandTimeout = 3000000;

            //如果事务对象可用，则设置COMMAND的事务属性
            if (transaction != null)
            {
                command.Transaction = transaction;
            }

            //设置COMMAND的参数集合
            if (commandParameter != null)
            {
                AttachParameters(command, commandParameter);
            }

            return;
        }

        /// <summary>
        /// This method opens (if necessary) and assigns a connection, transaction, command type and parameters 
        /// to the provided command
        /// </summary>
        /// <param name="command">The SqlCommand to be prepared</param>
        /// <param name="connection">A valid SqlConnection, on which to execute this command</param>
        /// <param name="transaction">A valid SqlTransaction, or 'null'</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">An array of SqlParameters to be associated with the command or 'null' if no parameters are required</param>
        /// <param name="mustCloseConnection"><c>true</c> if the connection was opened by the method, otherwose is false.</param>
        private static void PrepareCommand(SqlCommand command, SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, SqlParameter[] commandParameters, out bool mustCloseConnection)
        {
            if (command == null) throw new ArgumentNullException("command");
            if (commandText == null || commandText.Length == 0) throw new ArgumentNullException("commandText");

            // If the provided connection is not open, we will open it
            if (connection.State != ConnectionState.Open)
            {
                mustCloseConnection = true;
                connection.Open();
            }
            else
            {
                mustCloseConnection = false;
            }

            // Associate the connection with the command
            command.Connection = connection;

            // Set the command text (stored procedure name or SQL statement)
            command.CommandText = commandText;

            // If we were provided a transaction, assign it
            if (transaction != null)
            {
                if (transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
                command.Transaction = transaction;
            }

            // Set the command type
            command.CommandType = commandType;

            // Attach the command parameters if they are provided
            if (commandParameters != null)
            {
                AttachParameters(command, commandParameters);
            }
            return;
        }

        #endregion

        #region 执行SQL命令而不返回数据记录 & ExecuteNonQuery
        #region --------------------------重载 1-----------------------
        //基础函数
        /// <summary>
        /// 执行SQL命令而不返回查询数据，通常执行关于对数据源进行添加，删除，更新的操作
        /// </summary>
        /// <param name="connection">Sql Server连接对象</param>
        /// <param name="transaction">SQL事务对象</param>
        /// <param name="commandtype">SQL命令的类型，COMMANDTYPE枚举类型</param>
        /// <param name="commandText">SQL命令</param>
        /// <param name="commandParameter">参数集合数组</param>
        /// <returns>返回受影响的行数</returns>
        public static int ExecuteNonQuery(SqlConnection connection, SqlTransaction transaction, CommandType commandtype, String commandText, params SqlParameter[] commandParameter)
        {
            //创建SqlCommand对象，并且初始化它
            SqlCommand cmd = new SqlCommand();

            PrepareCommand(cmd, connection, transaction, commandtype, commandText, commandParameter);

            int i = cmd.ExecuteNonQuery();

            //清除SqlCommand的参数集合，或许下次用到
            cmd.Parameters.Clear();

            return i;
        }
        #endregion

        #region --------------------------重载 2-----------------------
        /// <summary>
        /// 执行SQL命令而不返回查询数据，通常执行关于对数据源进行添加，删除，更新的操作
        /// </summary>
        /// <param name="connection">Sql Server连接对象</param>
        /// <param name="commandtype">SQL命令的类型，COMMANDTYPE枚举类型</param>
        /// <param name="commandText">SQL命令</param>
        /// <param name="commandParameter">参数集合数组</param>
        /// <returns>返回受影响的行数</returns>
        public static int ExecuteNonQuery(SqlConnection connection, CommandType commandtype, String commandText, params SqlParameter[] commandParameter)
        {
            return ExecuteNonQuery(connection, (SqlTransaction)null, commandtype, commandText, commandParameter);
        }
        #endregion --------------------------2-----------------------

        #region --------------------------重载 3-----------------------
        /// <summary>
        /// 执行SQL命令而不返回查询数据，通常执行关于对数据源进行添加，删除，更新的操作
        /// </summary>
        /// <param name="transaction">SQL事务对象</param>
        /// <param name="commandtype">SQL命令的类型，COMMANDTYPE枚举类型</param>
        /// <param name="commandText">SQL命令</param>
        /// <param name="commandParameter">参数集合数组</param>
        /// <returns>返回受影响的行数</returns>
        public static int ExecuteNonQuery(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            return ExecuteNonQuery(transaction.Connection, transaction, commandType, commandText, commandParameters);
        }
        #endregion --------------------------3-----------------------

        #region --------------------------重载 4-----------------------
        /// <summary>
        /// 执行SQL命令而不返回查询数据，通常执行关于对数据源进行添加，删除，更新的操作
        /// </summary>
        /// <param name="connectionString">sql连接字符串</param>
        /// <param name="commandType">SQL命令的类型，COMMANDTYPE枚举类型</param>
        /// <param name="commandText">SQL命令</param>
        /// <param name="commandParameters">参数集合数组</param>
        /// <returns>返回受影响的行数</returns>
        public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();

                return ExecuteNonQuery(cn, commandType, commandText, commandParameters);
            }
        }
        #endregion --------------------------4-----------------------

        #region --------------------------重载 5-----------------------
        /// <summary>
        /// 执行SQL命令而不返回查询数据，通常执行关于对数据源进行添加，删除，更新的操作
        /// </summary>
        /// <param name="connectionString">sql连接字符串</param>
        /// <param name="commandType">SQL命令的类型，COMMANDTYPE枚举类型</param>
        /// <param name="commandText">SQL命令</param>
        /// <returns>返回受影响的行数</returns>
        public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText)
        {
            return ExecuteNonQuery(connectionString, commandType, commandText, (SqlParameter[])null);
        }
        #endregion --------------------------5-----------------------

        #region	--------------------------重载 6-----------------------

        /// <summary>
        /// 执行SQL命令而不返回查询数据，通常执行关于对数据源进行添加，删除，更新的操作
        /// </summary>
        /// <param name="connection">sql连接对象</param>
        /// <param name="spName">存储过程名称</param>
        /// <param name="parameterValues">对应存储过程参数的值</param>
        /// <returns>返回受影响的行数</returns>
        public static int ExecuteNonQuery(SqlConnection connection, string spName, params object[] parameterValues)
        {
            if (parameterValues != null && parameterValues.Length > 0)
            {
                //从缓存中取出参数结构
                SqlParameter[] commandParameter = SqlCache.GetSpParameterSet(connection.ConnectionString, spName);

                //将值赋于参数结构
                AssignParameterValues(commandParameter, parameterValues);

                return ExecuteNonQuery(connection, CommandType.StoredProcedure, spName, commandParameter);
            }
            else
            {
                return ExecuteNonQuery(connection, CommandType.StoredProcedure, spName);
            }

        }
        #endregion----------------------------6---------------------

        /// <summary>
        /// 执行SQL命令而不返回查询数据，通常执行关于对数据源进行添加，删除，更新的操作
        /// </summary>
        /// <param name="connectionstring">连接字符</param>
        /// <param name="spName">存储过程名称</param>
        /// <param name="parameterValues">对应存储过程参数的值</param>
        /// <returns>返回受影响的行数</returns>
        public static int ExecuteNonQuery(string connectionString, string spName, params object[] parameterValues)
        {

            if ((parameterValues != null) && (parameterValues.Length > 0))
            {

                SqlParameter[] commandParameters = SqlCache.GetSpParameterSet(connectionString, spName);

                AssignParameterValues(commandParameters, parameterValues);


                return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }

            else
            {
                return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName);
            }
        }
        #region	--------------------------重载 7-----------------------

        /// <summary>
        /// 执行SQL命令而不返回查询数据，通常执行关于对数据源进行添加，删除，更新的操作
        /// </summary>
        /// <param name="connection">sql连接对象</param>
        /// <param name="spName">存储过程名称</param>
        /// <param name="parameterValues">对应存储过程参数的值</param>
        /// <returns>返回受影响的行数</returns>
        public static int ExecuteNonQuery(SqlTransaction transaction, string spName, params object[] parameterValues)
        {
            if (parameterValues != null && parameterValues.Length > 0)
            {
                //从缓存中取出参数结构
                SqlParameter[] commandParameter = SqlCache.GetSpParameterSet(transaction.Connection.ConnectionString, spName);

                //将值赋于参数结构
                AssignParameterValues(commandParameter, parameterValues);

                return ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName, commandParameter);
            }
            else
            {
                return ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName);
            }

        }
        #endregion----------------------------7---------------------

        #region --------------------------重载 8-----------------------
        /// <summary>
        /// 执行SQL命令而不返回查询数据，通常执行关于对数据源进行添加，删除，更新的操作
        /// </summary>
        /// <param name="transaction">SQL事物对象</param>
        /// <param name="commandType">SQL命令的类型，COMMANDTYPE枚举类型</param>
        /// <param name="commandText">SQL命令</param>
        /// <returns>返回受影响的行数</returns>
        public static int ExecuteNonQuery(SqlTransaction transaction, CommandType commandType, string commandText)
        {
            return ExecuteNonQuery(transaction, commandType, commandText, (SqlParameter[])null);
        }
        #endregion

        #region --------------------------重载 9-----------------------
        /// <summary>
        ///  执行SQL命令而不返回查询数据，通常执行关于对数据源进行添加，删除，更新的操作
        /// </summary>
        /// <param name="connection">SQL连接对象</param>
        /// <param name="commandType">SQL命令的类型，COMMANDTYPE枚举类型</param>
        /// <param name="commandText">SQL命令</param>
        /// <returns>返回受影响的行数</returns>
        public static int ExecuteNonQuery(SqlConnection connection, CommandType commandType, string commandText)
        {
            return ExecuteNonQuery(connection, commandType, commandText, (SqlParameter[])null);
        }

        /// <summary>
        /// 执行SQL命令而不返回查询数据，通常执行关于对数据源进行添加，删除，更新的操作
        /// </summary>
        /// <param name="strConnection">SQL连接字符串</param>
        /// <param name="commandText">SQL命令</param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string strConnection, string commandText)
        {
            return ExecuteNonQuery(strConnection, CommandType.Text, commandText);
        }
        #endregion

        #endregion

        #region 执行SQL命令返回数据集 & ExecuteDataSet
        /// <summary>
        /// 执行SQL命令返回数据集
        /// </summary>
        /// <param name="connection">SQL连接对象</param>
        /// <param name="transaction">SQL事务</param>
        /// <param name="commandType">SQL命令的类型，COMMANDTYPE枚举类型</param>
        /// <param name="commandText">SQL命令</param>
        /// <param name="commandParameters">参数集合数组</param>
        /// <returns>数据集</returns>
        public static DataSet ExecuteDataset(SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            PrepareCommand(cmd, connection, transaction, commandType, commandText, commandParameters);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();

            da.Fill(ds);
            cmd.Parameters.Clear();
            return ds;


        }


        /// <summary>
        /// 执行SQL命令返回数据集
        /// </summary>
        /// <param name="connectionString">SQL连接字符串</param>
        /// <param name="commandType">SQL命令的类型，COMMANDTYPE枚举类型</param>
        /// <param name="commandText">SQL命令</param>
        /// <returns>数据集</returns>
        public static DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText)
        {
            return ExecuteDataset(connectionString, commandType, commandText, (SqlParameter[])null);
        }


        /// <summary>
        /// 执行SQL命令返回数据集
        /// </summary>
        /// <param name="connectionString">SQL连接字符串</param>
        /// <param name="commandType">SQL命令的类型，COMMANDTYPE枚举类型</param>
        /// <param name="commandText">SQL命令</param>
        /// <param name="commandParameters">参数集合数组</param>
        /// <returns>数据集</returns>
        public static DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                return ExecuteDataset(cn, commandType, commandText, commandParameters);
            }
        }

        /// <summary>
        /// Execute a SqlCommand (that returns a resultset and takes no parameters) against the database specified in 
        /// the connection string. 
        /// </summary>
        /// <remarks>
        /// Added by 含笑/2005-4-29
        /// e.g.:  
        ///  DataSet ds = ExecuteDataset(connString, CommandType.StoredProcedure, "GetOrders");
        /// </remarks>
        /// <param name="connectionString">A valid connection string for a SqlConnection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <param name="startRecord"></param>
        /// <param name="maxRecords"></param>
        /// <returns>A dataset containing the resultset generated by the command</returns>
        public static DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText, int startRecord, int maxRecords)
        {
            // Pass through the call providing null for the set of SqlParameters
            return ExecuteDataset(connectionString, commandType, commandText, startRecord, maxRecords, (SqlParameter[])null);
        }

        /// <summary>
        /// Execute a SqlCommand (that returns a resultset) against the database specified in the connection string 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// Added by 含笑/2005-4-29
        /// e.g.:  
        ///  DataSet ds = ExecuteDataset(connString, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">A valid connection string for a SqlConnection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <param name="startRecord"></param>
        /// <param name="maxRecords"></param>
        /// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
        /// <returns>A dataset containing the resultset generated by the command</returns>
        public static DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText, int startRecord, int maxRecords, params SqlParameter[] commandParameters)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");

            // Create & open a SqlConnection, and dispose of it after we are done
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Call the overload that takes a connection in place of the connection string
                return ExecuteDataset(connection, commandType, commandText, startRecord, maxRecords, commandParameters);
            }
        }

        /// <summary>
        /// Execute a SqlCommand (that returns a resultset and takes no parameters) against the provided SqlConnection. 
        /// </summary>
        /// <remarks>
        /// Added by 含笑/2005-4-29
        /// e.g.:  
        ///  DataSet ds = ExecuteDataset(conn, CommandType.StoredProcedure, "GetOrders");
        /// </remarks>
        /// <param name="connection">A valid SqlConnection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <param name="startRecord"></param>
        /// <param name="maxRecords"></param>
        /// <returns>A dataset containing the resultset generated by the command</returns>
        public static DataSet ExecuteDataset(SqlConnection connection, CommandType commandType, string commandText, int startRecord, int maxRecords)
        {
            // Pass through the call providing null for the set of SqlParameters
            return ExecuteDataset(connection, commandType, commandText, startRecord, maxRecords, (SqlParameter[])null);
        }

        /// <summary>
        /// Execute a SqlCommand (that returns a resultset) against the specified SqlConnection 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// Added by 含笑/2005-4-29
        /// e.g.:  
        ///  DataSet ds = ExecuteDataset(conn, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connection">A valid SqlConnection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
        /// <param name="startRecord"></param>
        /// <param name="maxRecords"></param>
        /// <returns>A dataset containing the resultset generated by the command</returns>
        public static DataSet ExecuteDataset(SqlConnection connection, CommandType commandType, string commandText, int startRecord, int maxRecords, params SqlParameter[] commandParameters)
        {
            if (connection == null) throw new ArgumentNullException("connection");

            // Create a command and prepare it for execution
            SqlCommand cmd = new SqlCommand();
            bool mustCloseConnection = false;
            PrepareCommand(cmd, connection, (SqlTransaction)null, commandType, commandText, commandParameters, out mustCloseConnection);

            // Create the DataAdapter & DataSet
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                DataSet ds = new DataSet();

                // Fill the DataSet using default values for DataTable names, etc
                da.Fill(ds, startRecord, maxRecords, "db");

                // Detach the SqlParameters from the command object, so they can be used again
                cmd.Parameters.Clear();

                if (mustCloseConnection)
                    connection.Close();

                // Return the dataset
                return ds;
            }
        }


        /// <summary>
        /// 执行SQL命令返回数据集
        /// </summary>
        /// <param name="connectionString">SQL连接字符串</param>
        /// <param name="spName">存储过程名称</param>
        /// <param name="parameterValues">针对存储过程参数的值数组</param>
        /// <returns>数据集</returns>
        public static DataSet ExecuteDataset(string connectionString, string spName, params object[] parameterValues)
        {
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                SqlParameter[] arrParameter = SqlCache.GetSpParameterSet(connectionString, spName);

                AssignParameterValues(arrParameter, parameterValues);

                return ExecuteDataset(connectionString, CommandType.StoredProcedure, spName, arrParameter);

            }
            else
            {
                return ExecuteDataset(connectionString, CommandType.StoredProcedure, spName);
            }
        }


        /// <summary>
        /// 执行SQL命令返回数据集
        /// </summary>
        /// <param name="connection">SQL连接对象</param>
        /// <param name="commandType">SQL命令的类型，COMMANDTYPE枚举类型</param>
        /// <param name="commandText">SQL命令</param>
        /// <returns>数据集</returns>
        public static DataSet ExecuteDataset(SqlConnection connection, CommandType commandType, string commandText)
        {
            return ExecuteDataset(connection, commandType, commandText, (SqlParameter[])null);
        }


        /// <summary>
        /// 执行SQL命令返回数据集
        /// </summary>
        /// <param name="connection">SQL连接对象</param>
        /// <param name="commandType">SQL命令的类型，COMMANDTYPE枚举类型</param>
        /// <param name="commandText">SQL命令</param>
        /// <param name="commandParameters">参数集合数组</param>
        /// <returns>数据集</returns>
        public static DataSet ExecuteDataset(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            return ExecuteDataset(connection, (SqlTransaction)null, commandType, commandText, commandParameters);
        }


        /// <summary>
        /// 执行SQL命令返回数据集
        /// </summary>
        /// <param name="connection">SQL连接对象</param>
        /// <param name="spName">存储过程名称</param>
        /// <param name="parameterValues">针对存储过程参数的值数组</param>
        /// <returns>数据集</returns>
        public static DataSet ExecuteDataset(SqlConnection connection, string spName, params object[] parameterValues)
        {
            return ExecuteDataset(connection.ConnectionString, spName, parameterValues);
        }


        /// <summary>
        /// 执行SQL命令返回数据集
        /// </summary>
        /// <param name="transaction">SQL事务</param>
        /// <param name="commandType">SQL命令的类型，COMMANDTYPE枚举类型</param>
        /// <param name="commandText">SQL命令</param>
        /// <returns>数据集</returns>
        public static DataSet ExecuteDataset(SqlTransaction transaction, CommandType commandType, string commandText)
        {
            return ExecuteDataset(transaction, commandType, commandText, (SqlParameter[])null);
        }


        /// <summary>
        /// 执行SQL命令返回数据集
        /// </summary>
        /// <param name="transaction">SQL事务</param>
        /// <param name="commandType">SQL命令的类型，COMMANDTYPE枚举类型</param>
        /// <param name="commandText">SQL命令</param>
        /// <param name="commandParameters">参数集合数组</param>
        /// <returns></returns>
        public static DataSet ExecuteDataset(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            return ExecuteDataset(transaction.Connection, transaction, commandType, commandText, commandParameters);
        }


        /// <summary>
        /// 执行SQL命令返回数据集
        /// </summary>
        /// <param name="transaction">SQL事务</param>
        /// <param name="spName">存储过程名称</param>
        /// <param name="parameterValues">针对存储过程参数的值数组</param>
        /// <returns>数据集</returns>
        public static DataSet ExecuteDataset(SqlTransaction transaction, string spName, params object[] parameterValues)
        {
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                SqlParameter[] arrParameter = SqlCache.GetSpParameterSet(transaction.Connection.ConnectionString, spName);

                AssignParameterValues(arrParameter, parameterValues);

                return ExecuteDataset(transaction, CommandType.StoredProcedure, spName, arrParameter);
            }
            else
            {
                return ExecuteDataset(transaction, CommandType.StoredProcedure, spName);
            }
        }

        #endregion

        #region 执行SQL命令返回数据表 & ExecuteDataTable
        /// <summary>
        /// 执行SQL命令返回数据表
        /// </summary>
        /// <param name="connection">SQL连接对象</param>
        /// <param name="transaction">SQL事务</param>
        /// <param name="commandType">SQL命令的类型，COMMANDTYPE枚举类型</param>
        /// <param name="commandText">SQL命令</param>
        /// <param name="commandParameters">参数集合数组</param>
        /// <returns>数据表</returns>
        public static DataTable ExecuteDataTable(SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            PrepareCommand(cmd, connection, transaction, commandType, commandText, commandParameters);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            da.Fill(dt);

            cmd.Parameters.Clear();

            return dt;
        }


        /// <summary>
        /// 执行SQL命令返回数据表
        /// </summary>
        /// <param name="connectionString">SQL连接字符串</param>
        /// <param name="commandType">SQL命令的类型，COMMANDTYPE枚举类型</param>
        /// <param name="commandText">SQL命令</param>
        /// <returns>数据表</returns>
        public static DataTable ExecuteDataTable(string connectionString, CommandType commandType, string commandText)
        {
            return ExecuteDataTable(connectionString, commandType, commandText, (SqlParameter[])null);
        }


        /// <summary>
        /// 执行SQL命令返回数据表
        /// </summary>
        /// <param name="connectionString">SQL连接字符串</param>
        /// <param name="commandType">SQL命令的类型，COMMANDTYPE枚举类型</param>
        /// <param name="commandText">SQL命令</param>
        /// <param name="commandParameters">参数集合数组</param>
        /// <returns>数据表</returns>
        public static DataTable ExecuteDataTable(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                return ExecuteDataTable(cn, commandType, commandText, commandParameters);
            }

        }


        /// <summary>
        /// 执行SQL命令返回数据表
        /// </summary>
        /// <param name="connectionString">SQL连接字符串</param>
        /// <param name="spName">存储过程名称</param>
        /// <param name="parameterValues">针对存储过程参数的值数组</param>
        /// <returns>数据表</returns>
        public static DataTable ExecuteDataTable(string connectionString, string spName, params object[] parameterValues)
        {
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                SqlParameter[] arrParameter = SqlCache.GetSpParameterSet(connectionString, spName);

                AssignParameterValues(arrParameter, parameterValues);

                return ExecuteDataTable(connectionString, CommandType.StoredProcedure, spName, arrParameter);

            }
            else
            {
                return ExecuteDataTable(connectionString, CommandType.StoredProcedure, spName);
            }
        }


        /// <summary>
        /// 执行SQL命令返回数据表
        /// </summary>
        /// <param name="connection">SQL连接对象</param>
        /// <param name="commandType">SQL命令的类型，COMMANDTYPE枚举类型</param>
        /// <param name="commandText">SQL命令</param>
        /// <returns>数据表</returns>
        public static DataTable ExecuteDataTable(SqlConnection connection, CommandType commandType, string commandText)
        {
            return ExecuteDataTable(connection, commandType, commandText, (SqlParameter[])null);
        }


        /// <summary>
        /// 执行SQL命令返回数据表
        /// </summary>
        /// <param name="connection">SQL连接对象</param>
        /// <param name="commandType">SQL命令的类型，COMMANDTYPE枚举类型</param>
        /// <param name="commandText">SQL命令</param>
        /// <param name="commandParameters">参数集合数组</param>
        /// <returns>数据表</returns>
        public static DataTable ExecuteDataTable(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            return ExecuteDataTable(connection, (SqlTransaction)null, commandType, commandText, commandParameters);
        }


        /// <summary>
        /// 执行SQL命令返回数据表
        /// </summary>
        /// <param name="connection">SQL连接对象</param>
        /// <param name="spName">存储过程名称</param>
        /// <param name="parameterValues">针对存储过程参数的值数组</param>
        /// <returns>数据表</returns>
        public static DataTable ExecuteDataTable(SqlConnection connection, string spName, params object[] parameterValues)
        {
            return ExecuteDataTable(connection.ConnectionString, spName, parameterValues);
        }


        /// <summary>
        /// 执行SQL命令返回数据表
        /// </summary>
        /// <param name="transaction">SQL事务</param>
        /// <param name="commandType">SQL命令的类型，COMMANDTYPE枚举类型</param>
        /// <param name="commandText">SQL命令</param>
        /// <returns>数据表</returns>
        public static DataTable ExecuteDataTable(SqlTransaction transaction, CommandType commandType, string commandText)
        {
            return ExecuteDataTable(transaction, commandType, commandText, (SqlParameter[])null);
        }


        /// <summary>
        /// 执行SQL命令返回数据表
        /// </summary>
        /// <param name="transaction">SQL事务</param>
        /// <param name="commandType">SQL命令的类型，COMMANDTYPE枚举类型</param>
        /// <param name="commandText">SQL命令</param>
        /// <param name="commandParameters">参数集合数组</param>
        /// <returns></returns>
        public static DataTable ExecuteDataTable(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            return ExecuteDataTable(transaction.Connection, transaction, commandType, commandText, commandParameters);
        }


        /// <summary>
        /// 执行SQL命令返回数据表
        /// </summary>
        /// <param name="transaction">SQL事务</param>
        /// <param name="spName">存储过程名称</param>
        /// <param name="parameterValues">针对存储过程参数的值数组</param>
        /// <returns>数据表</returns>
        public static DataTable ExecuteDataTable(SqlTransaction transaction, string spName, params object[] parameterValues)
        {
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                SqlParameter[] arrParameter = SqlCache.GetSpParameterSet(transaction.Connection.ConnectionString, spName);

                AssignParameterValues(arrParameter, parameterValues);

                return ExecuteDataTable(transaction, CommandType.StoredProcedure, spName, arrParameter);
            }
            else
            {
                return ExecuteDataTable(transaction, CommandType.StoredProcedure, spName);
            }
        }

        #endregion

        #region 执行SQL命令返回数据行 & ExecuteDataRow

        #region 返回指定的数据行
        /// <summary>
        /// 执行SQL命令返回数据行
        /// </summary>
        /// <param name="connection">SQL连接对象</param>
        /// <param name="transaction">SQL事务</param>
        /// <param name="commandType">SQL命令的类型，COMMANDTYPE枚举类型</param>
        /// <param name="commandText">SQL命令</param>
        /// <param name="commandParameters">参数集合数组</param>
        /// <param name="rowIndex">返回的行索引</param>
        /// <returns>数据行</returns>
        public static System.Data.DataRow ExecuteDataRow(SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, Int16 rowIndex, params SqlParameter[] commandParameters)
        {
            DataTable dt = new DataTable();
            dt = ExecuteDataTable(connection, transaction, commandType, commandText, commandParameters);

            if ((dt != null) && (dt.Rows.Count > 0))
            {
                if (rowIndex <= dt.Rows.Count - 1)
                {
                    DataRow dw = dt.Rows[rowIndex];
                    return dw;
                }
                else
                {
                    throw new ArgumentException("没有相应的索引行");
                }
            }
            else
            {
                return (DataRow)null;
            }
        }


        /// <summary>
        /// 执行SQL命令返回数据行
        /// </summary>
        /// <param name="connection">SQL连接对象</param>
        /// <param name="commandType">SQL命令的类型，COMMANDTYPE枚举类型</param>
        /// <param name="commandText">SQL命令</param>
        /// <param name="commandParameters">参数集合数组</param>
        /// <param name="rowIndex">返回的行索引</param>
        /// <returns>数据行</returns>
        public static System.Data.DataRow ExecuteDataRow(SqlConnection connection, CommandType commandType, string commandText, Int16 rowIndex, params SqlParameter[] commandParameters)
        {
            return ExecuteDataRow(connection, (SqlTransaction)null, commandType, commandText, rowIndex, commandParameters);
        }


        /// <summary>
        /// 执行SQL命令返回数据行
        /// </summary>
        /// <param name="connection">SQL连接对象</param>
        /// <param name="commandType">SQL命令的类型，COMMANDTYPE枚举类型</param>
        /// <param name="commandText">SQL命令</param>
        /// <param name="rowIndex">返回的行索引</param>
        /// <returns>数据行</returns>
        public static System.Data.DataRow ExecuteDataRow(SqlConnection connection, CommandType commandType, string commandText, Int16 rowIndex)
        {
            return ExecuteDataRow(connection, commandType, commandText, rowIndex, (SqlParameter[])null);
        }


        /// <summary>
        /// 执行SQL命令返回数据行
        /// </summary>
        /// <param name="connection">SQL连接对象</param>
        /// <param name="spName">存储过程名称</param>
        /// <param name="rowIndex">返回的行索引</param>
        /// <param name="paramValues">针对存储过程的参数的值的数组</param>
        /// <returns>数据行</returns>
        public static System.Data.DataRow ExecuteDataRow(SqlConnection connection, string spName, Int16 rowIndex, params object[] paramValues)
        {
            return ExecuteDataRow(connection.ConnectionString, spName, rowIndex, paramValues);
        }


        /// <summary>
        /// 执行SQL命令返回数据行
        /// </summary>
        /// <param name="connectionString">SQL连接字符串</param>
        /// <param name="commandType">SQL命令的类型，COMMANDTYPE枚举类型</param>
        /// <param name="commandText">SQL命令</param>
        /// <param name="commandParameters">参数集合数组</param>
        /// <param name="rowIndex">返回的行索引</param>
        /// <returns>数据行</returns>
        public static System.Data.DataRow ExecuteDataRow(string connectionString, CommandType commandType, string commandText, Int16 rowIndex, params SqlParameter[] commandParameters)
        {
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                return ExecuteDataRow(cn, commandType, commandText, rowIndex, commandParameters);
            }
        }


        /// <summary>
        /// 执行SQL命令返回数据行
        /// </summary>
        /// <param name="connectionString">SQL连接字符串</param>
        /// <param name="commandType">SQL命令的类型，COMMANDTYPE枚举类型</param>
        /// <param name="commandText">SQL命令</param>
        /// <param name="rowIndex">返回的行索引</param>
        /// <returns>数据行</returns>
        public static System.Data.DataRow ExecuteDataRow(string connectionString, CommandType commandType, string commandText, Int16 rowIndex)
        {
            return ExecuteDataRow(connectionString, commandType, commandText, rowIndex, (SqlParameter[])null);
        }


        /// <summary>
        /// 执行SQL命令返回数据行
        /// </summary>
        /// <param name="connectionString">SQL连接字符串</param>
        /// <param name="spName">存储过程名称</param>
        /// <param name="rowIndex">返回的行索引</param>
        /// <param name="paramValues">针对存储过程的参数的值的数组</param>
        /// <returns>数据行</returns>
        public static System.Data.DataRow ExecuteDataRow(string connectionString, string spName, Int16 rowIndex, params object[] paramValues)
        {
            if ((paramValues != null) && (paramValues.Length > 0))
            {
                SqlParameter[] commandParameters = commandParameters = SqlCache.GetSpParameterSet(connectionString, spName);

                AssignParameterValues(commandParameters, paramValues);

                return ExecuteDataRow(connectionString, CommandType.StoredProcedure, spName, rowIndex, commandParameters);

            }
            else
            {
                return ExecuteDataRow(connectionString, CommandType.StoredProcedure, spName, rowIndex);
            }
        }


        /// <summary>
        /// 执行SQL命令返回数据行
        /// </summary>
        /// <param name="transaction">SQL事务</param>
        /// <param name="commandType">SQL命令的类型，COMMANDTYPE枚举类型</param>
        /// <param name="commandText">SQL命令</param>
        /// <param name="commandParameters">参数集合数组</param>
        /// <param name="rowIndex">返回的行索引</param>
        /// <returns>数据行</returns>
        public static System.Data.DataRow ExecuteDataRow(SqlTransaction transaction, CommandType commandType, string commandText, Int16 rowIndex, params SqlParameter[] commandParameters)
        {
            return ExecuteDataRow(transaction.Connection, transaction, commandType, commandText, rowIndex, commandParameters);
        }

        /// <summary>
        /// 执行SQL命令返回数据行
        /// </summary>
        /// <param name="transaction">SQL事务</param>
        /// <param name="commandType">SQL命令的类型，COMMANDTYPE枚举类型</param>
        /// <param name="commandText">SQL命令</param>
        /// <param name="rowIndex">返回的行索引</param>
        /// <returns>数据行</returns>
        public static System.Data.DataRow ExecuteDataRow(SqlTransaction transaction, CommandType commandType, string commandText, Int16 rowIndex)
        {
            return ExecuteDataRow(transaction, commandType, commandText, rowIndex, (SqlParameter[])null);
        }

        /// <summary>
        /// 执行SQL命令返回数据行
        /// </summary>
        /// <param name="transaction">SQL事务</param>
        /// <param name="spName">存储过程名称</param>
        /// <param name="rowIndex">返回的行索引</param>
        /// <param name="paramValues">针对存储过程的参数的值的数组</param>
        /// <returns>数据行</returns>
        public static System.Data.DataRow ExecuteDataRow(SqlTransaction transaction, string spName, Int16 rowIndex, params object[] paramValues)
        {
            return ExecuteDataRow(transaction.Connection.ConnectionString, spName, rowIndex, paramValues);
        }

        #endregion

        #region 返回第一行数据行
        /// <summary>
        /// 执行SQL命令返回数据行
        /// </summary>
        /// <param name="connection">SQL连接对象</param>
        /// <param name="transaction">SQL事务</param>
        /// <param name="commandType">SQL命令的类型，COMMANDTYPE枚举类型</param>
        /// <param name="commandText">SQL命令</param>
        /// <param name="commandParameters">参数集合数组</param>
        /// <returns>数据行</returns>
        public static System.Data.DataRow ExecuteDataRow(SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, SqlParameter[] commandParameters)
        {
            return ExecuteDataRow(connection, transaction, commandType, commandText, 0, commandParameters);
        }

        /// <summary>
        /// 执行SQL命令返回数据行
        /// </summary>
        /// <param name="connection">SQL连接对象</param>
        /// <param name="commandType">SQL命令的类型，COMMANDTYPE枚举类型</param>
        /// <param name="commandText">SQL命令</param>
        /// <param name="commandParameters">参数集合数组</param>
        /// <returns>数据行</returns>
        public static System.Data.DataRow ExecuteDataRow(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            return ExecuteDataRow(connection, (SqlTransaction)null, commandType, commandText, commandParameters);
        }


        /// <summary>
        /// 执行SQL命令返回数据行
        /// </summary>
        /// <param name="connection">SQL连接对象</param>
        /// <param name="commandType">SQL命令的类型，COMMANDTYPE枚举类型</param>
        /// <param name="commandText">SQL命令</param>
        /// <returns>数据行</returns>
        public static System.Data.DataRow ExecuteDataRow(SqlConnection connection, CommandType commandType, string commandText)
        {
            return ExecuteDataRow(connection, commandType, commandText, (SqlParameter[])null);
        }


        /// <summary>
        /// 执行SQL命令返回数据行
        /// </summary>
        /// <param name="connection">SQL连接对象</param>
        /// <param name="spName">存储过程名称</param>
        /// <param name="paramValues">针对存储过程的参数的值的数组</param>
        /// <returns>数据行</returns>
        public static System.Data.DataRow ExecuteDataRow(SqlConnection connection, string spName, params object[] paramValues)
        {
            return ExecuteDataRow(connection.ConnectionString, spName, paramValues);
        }


        /// <summary>
        /// 执行SQL命令返回数据行
        /// </summary>
        /// <param name="connectionString">SQL连接字符串</param>
        /// <param name="commandType">SQL命令的类型，COMMANDTYPE枚举类型</param>
        /// <param name="commandText">SQL命令</param>
        /// <param name="commandParameters">参数集合数组</param>
        /// <returns>数据行</returns>
        public static System.Data.DataRow ExecuteDataRow(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            return ExecuteDataRow(new SqlConnection(connectionString), commandType, commandText, commandParameters);
        }


        /// <summary>
        /// 执行SQL命令返回数据行
        /// </summary>
        /// <param name="connectionString">SQL连接字符串</param>
        /// <param name="commandType">SQL命令的类型，COMMANDTYPE枚举类型</param>
        /// <param name="commandText">SQL命令</param>
        /// <returns>数据行</returns>
        public static System.Data.DataRow ExecuteDataRow(string connectionString, CommandType commandType, string commandText)
        {
            return ExecuteDataRow(connectionString, commandType, commandText, (SqlParameter[])null);
        }


        /// <summary>
        /// 执行SQL命令返回数据行
        /// </summary>
        /// <param name="connectionString">SQL连接字符串</param>
        /// <param name="spName">存储过程名称</param>
        /// <param name="paramValues">针对存储过程的参数的值的数组</param>
        /// <returns>数据行</returns>
        public static System.Data.DataRow ExecuteDataRow(string connectionString, string spName, params object[] paramValues)
        {
            if ((paramValues != null) && (paramValues.Length > 0))
            {
                SqlParameter[] commandParameters = commandParameters = SqlCache.GetSpParameterSet(connectionString, spName);

                AssignParameterValues(commandParameters, paramValues);

                return ExecuteDataRow(connectionString, CommandType.StoredProcedure, spName, commandParameters);

            }
            else
            {
                return ExecuteDataRow(connectionString, CommandType.StoredProcedure, spName);
            }
        }




        /// <summary>
        /// 执行SQL命令返回数据行
        /// </summary>
        /// <param name="transaction">SQL事务</param>
        /// <param name="commandType">SQL命令的类型，COMMANDTYPE枚举类型</param>
        /// <param name="commandText">SQL命令</param>
        /// <param name="commandParameters">参数集合数组</param>
        /// <returns>数据行</returns>
        public static System.Data.DataRow ExecuteDataRow(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            return ExecuteDataRow(transaction.Connection, transaction, commandType, commandText, commandParameters);
        }

        /// <summary>
        /// 执行SQL命令返回数据行
        /// </summary>
        /// <param name="transaction">SQL事务</param>
        /// <param name="commandType">SQL命令的类型，COMMANDTYPE枚举类型</param>
        /// <param name="commandText">SQL命令</param>
        /// <returns>数据行</returns>
        public static System.Data.DataRow ExecuteDataRow(SqlTransaction transaction, CommandType commandType, string commandText)
        {
            return ExecuteDataRow(transaction, commandType, commandText, (SqlParameter[])null);
        }

        /// <summary>
        /// 执行SQL命令返回数据行
        /// </summary>
        /// <param name="transaction">SQL事务</param>
        /// <param name="spName">存储过程名称</param>
        /// <param name="rowIndex">返回的行索引</param>
        /// <param name="paramValues">针对存储过程的参数的值的数组</param>
        /// <returns>数据行</returns>
        public static System.Data.DataRow ExecuteDataRow(SqlTransaction transaction, string spName, params object[] paramValues)
        {
            return ExecuteDataRow(transaction.Connection.ConnectionString, spName, paramValues);
        }

        #endregion 返回第一行数据行

        #endregion

        #region 执行SQL命令返回DataReader & ExecuteReader
        /// <summary>
        /// 这个枚举的作用是，无论Connection对象有谁来提供（方法调用者或SqlHP自动产生），
        /// 在调用ExecuteReader时，我们能很好用CommandBehavior来控制
        /// </summary>
        private enum SqlConnectionOwnership
        {
            /// <summary>
            /// Connection 有SqlHP产生，在完成ExecuteReader时，关闭Connection
            /// </summary>
            Internal,

            /// <summary>
            /// Connection 有调用者提供，在完成ExecuteReader后，仍要保持Connection的状态
            /// </summary>
            External
        }

        /// <summary>
        /// 根据Connection的提供者的不同，执行SQL命令返回一个SqlDataReader对象
        /// </summary>
        /// <param name="connection">sql对象</param>
        /// <param name="transaction">SQL事务</param>
        /// <param name="commandType">SQL命令的类型，COMMANDTYPE枚举类型</param>
        /// <param name="commandText">SQL命令</param>
        /// <param name="connectionOwner">枚举，表示Connection的提供者</param>
        /// <param name="commandParameter">参数集合数组</param>
        /// <returns>SqlDataReader对象</returns>
        private static System.Data.SqlClient.SqlDataReader ExecuteReader(SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, SqlConnectionOwnership connectionOwner, params SqlParameter[] commandParameter)
        {
            //创建SqlCommand对象
            SqlCommand cmd = new SqlCommand();
            //初始化 cmd
            PrepareCommand(cmd, connection, transaction, commandType, commandText, commandParameter);


            SqlDataReader dr;
            if (connectionOwner == SqlConnectionOwnership.External)
            {
                //保持Connection原有状态
                dr = cmd.ExecuteReader();
            }
            else
            {
                //完成操作后关闭连接
                dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            }

            cmd.Parameters.Clear();

            return dr;

        }

        /// <summary>
        /// 执行SQL命令返回一个SqlDataReader对象
        /// </summary>
        /// <param name="connection">sql对象</param>
        /// <param name="transaction">SQL事务</param>
        /// <param name="commandType">SQL命令的类型，COMMANDTYPE枚举类型</param>
        /// <param name="commandText">SQL命令</param>
        /// <param name="commandParameter">参数集合数组</param>
        /// <returns>SqlDataReader对象</returns>
        public static SqlDataReader ExecuteReader(SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameter)
        {
            return ExecuteReader(connection, transaction, commandType, commandText, SqlConnectionOwnership.External, commandParameter);
        }

        /// <summary>
        /// 执行SQL命令返回一个SqlDataReader对象
        /// </summary>
        /// <param name="connection">sql对象</param>
        /// <param name="commandType">SQL命令的类型，COMMANDTYPE枚举类型</param>
        /// <param name="commandText">SQL命令</param>
        /// <param name="commandParameter">参数集合数组</param>
        /// <returns>SqlDataReader对象</returns>
        public static SqlDataReader ExecuteReader(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameter)
        {
            return ExecuteReader(connection, (SqlTransaction)null, commandType, commandText, commandParameter);
        }

        /// <summary>
        /// 执行SQL命令返回一个SqlDataReader对象
        /// </summary>
        /// <param name="connection">sql对象</param>
        /// <param name="commandType">SQL命令的类型，COMMANDTYPE枚举类型</param>
        /// <param name="commandText">SQL命令</param>
        /// <returns>SqlDataReader对象</returns>
        public static SqlDataReader ExecuteReader(SqlConnection connection, CommandType commandType, string commandText)
        {
            return ExecuteReader(connection, commandType, commandText, (SqlParameter[])null);
        }

        /// <summary>
        ///  执行SQL命令返回一个SqlDataReader对象
        /// </summary>
        /// <param name="connection">sql对象</param>
        /// <param name="spName">存储过程名称</param>
        /// <param name="parameterValues">针对存储过程的参数结构的值数组</param>
        /// <returns>SqlDataReader对象</returns>
        public static SqlDataReader ExecuteReader(SqlConnection connection, string spName, params object[] parameterValues)
        {

            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                SqlParameter[] commandParameters = SqlCache.GetSpParameterSet(connection.ConnectionString, spName);

                AssignParameterValues(commandParameters, parameterValues);

                return ExecuteReader(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteReader(connection, CommandType.StoredProcedure, spName);
            }
        }


        /// <summary>
        /// 执行SQL命令返回一个SqlDataReader对象
        /// </summary>
        /// <param name="trancsaction">sql事务</param>
        /// <param name="commandType">SQL命令的类型，COMMANDTYPE枚举类型</param>
        /// <param name="commandText">SQL命令</param>
        /// <param name="commandParameter">参数集合数组</param>
        /// <returns>SqlDataReader对象</returns>
        public static SqlDataReader ExecuteReader(SqlTransaction trancsaction, CommandType commandType, string commandText, params SqlParameter[] commandParameter)
        {
            return ExecuteReader(trancsaction.Connection, trancsaction, commandType, commandText, commandParameter);
        }

        /// <summary>
        /// 执行SQL命令返回一个SqlDataReader对象
        /// </summary>
        /// <param name="trancsaction">sql事务</param>
        /// <param name="commandType">SQL命令的类型，COMMANDTYPE枚举类型</param>
        /// <param name="commandText">SQL命令</param>
        /// <returns>SqlDataReader对象</returns>
        public static SqlDataReader ExecuteReader(SqlTransaction trancsaction, CommandType commandType, string commandText)
        {
            return ExecuteReader(trancsaction.Connection, trancsaction, commandType, commandText, (SqlParameter[])null);
        }

        /// <summary>
        ///  执行SQL命令返回一个SqlDataReader对象
        /// </summary>
        /// <param name="trancsaction">sql事务</param>
        /// <param name="spName">存储过程名称</param>
        /// <param name="parameterValues">针对存储过程的参数结构的值数组</param>
        /// <returns>SqlDataReader对象</returns>
        public static SqlDataReader ExecuteReader(SqlTransaction trancsaction, string spName, params object[] parameterValues)
        {
            return ExecuteReader(trancsaction.Connection, spName, parameterValues);
        }



        /// <summary>
        /// 执行SQL命令返回一个SqlDataReader对象
        /// </summary>
        /// <param name="connectionString">sql连接字符串</param>
        /// <param name="transaction">SQL事务</param>
        /// <param name="commandType">SQL命令的类型，COMMANDTYPE枚举类型</param>
        /// <param name="commandText">SQL命令</param>
        /// <param name="commandParameter">参数集合数组</param>
        /// <returns>SqlDataReader对象</returns>
        public static SqlDataReader ExecuteReader(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameter)
        {
            SqlConnection cn = new SqlConnection(connectionString);
            cn.Open();

            try
            {
                return ExecuteReader(cn, null, commandType, commandText, SqlConnectionOwnership.Internal, commandParameter);
            }
            catch
            {
                cn.Close();
                throw;
            }
        }



        /// <summary>
        /// 执行SQL命令返回一个SqlDataReader对象
        /// </summary>
        /// <param name="connectionString">sql连接字符串</param>
        /// <param name="commandType">SQL命令的类型，COMMANDTYPE枚举类型</param>
        /// <param name="commandText">SQL命令</param>
        /// <returns>SqlDataReader对象</returns>
        public static SqlDataReader ExecuteReader(string connectionString, CommandType commandType, string commandText)
        {
            return ExecuteReader(connectionString, commandType, commandText, (SqlParameter[])null);
        }

        /// <summary>
        /// 执行SQL命令返回一个SqlDataReader对象
        /// </summary>
        /// <param name="connectionString">sql连接字符串</param>
        /// <param name="commandText">SQL命令</param>
        /// <returns>SqlDataReader对象</returns>
        public static SqlDataReader ExecuteReader(string connectionString, string commandText)
        {
            return ExecuteReader(connectionString, CommandType.Text, commandText);
        }

        /// <summary>
        ///  执行SQL命令返回一个SqlDataReader对象
        /// </summary>
        /// <param name="connectionString">sql连接字符串</param>
        /// <param name="spName">存储过程名称</param>
        /// <param name="parameterValues">针对存储过程的参数结构的值数组</param>
        /// <returns>SqlDataReader对象</returns>
        public static SqlDataReader ExecuteReader(string connectionString, string spName, params object[] parameterValues)
        {

            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                SqlParameter[] commandParameters = SqlCache.GetSpParameterSet(connectionString, spName);

                AssignParameterValues(commandParameters, parameterValues);

                return ExecuteReader(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteReader(connectionString, CommandType.StoredProcedure, spName);
            }
        }

        #endregion

        #region 执行SQL命令返回单值 & ExecuteScalar
        /// <summary>
        /// 执行SQL命令返回单值，例如：返回 SELECT COUNT（*） FROM TABLES 的命令的结果
        /// </summary>
        /// <param name="connection">SQL连接对象</param>
        /// <param name="transaction">SQL事务</param>
        /// <param name="commandType">SQL命令的类型，COMMANDTYPE枚举类型</param>
        /// <param name="commandText">SQL命令</param>
        /// <param name="commandParameters">参数集合数组</param>
        /// <returns>返回单值</returns>
        public static object ExecuteScalar(SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            PrepareCommand(cmd, connection, transaction, commandType, commandText, commandParameters);

            object i;

            i = cmd.ExecuteScalar();

            cmd.Parameters.Clear();

            return i;
        }

        /// <summary>
        /// 执行SQL命令返回单值，例如：返回 SELECT COUNT（*） FROM TABLES 的命令的结果
        /// </summary>
        /// <param name="connection">SQL连接对象</param>
        /// <param name="commandType">SQL命令的类型，COMMANDTYPE枚举类型</param>
        /// <param name="commandText">SQL命令</param>
        /// <param name="commandParameters">参数集合数组</param>
        /// <returns>返回单值</returns>
        public static object ExecuteScalar(SqlConnection connection, CommandType commandType, string commandText, SqlParameter[] commandParameters)
        {
            return ExecuteScalar(connection, (SqlTransaction)null, commandType, commandText, commandParameters);
        }

        /// <summary>
        /// 执行SQL命令返回单值，例如：返回 SELECT COUNT（*） FROM TABLES 的命令的结果
        /// </summary>
        /// <param name="connection">SQL连接对象</param>
        /// <param name="commandType">SQL命令的类型，COMMANDTYPE枚举类型</param>
        /// <param name="commandText">SQL命令</param>
        /// <returns>返回单值</returns>
        public static object ExecuteScalar(SqlConnection connection, CommandType commandType, string commandText)
        {
            return ExecuteScalar(connection, commandType, commandText, (SqlParameter[])null);
        }

        /// <summary>
        /// 执行SQL命令返回单值，例如：返回 SELECT COUNT（*） FROM TABLES 的命令的结果
        /// </summary>
        /// <param name="connection">SQL连接对象</param>
        /// <param name="spName">存储过程名称</param>
        /// <param name="parameterValues">针对存储过程的参数结构的值数组</param>
        /// <returns>返回单值</returns>
        public static object ExecuteScalar(SqlConnection connection, string spName, object[] parameterValues)
        {
            return ExecuteScalar(connection.ConnectionString, spName, parameterValues);
        }


        /// <summary>
        /// 执行SQL命令返回单值，例如：返回 SELECT COUNT（*） FROM TABLES 的命令的结果
        /// </summary>
        /// <param name="transaction">SQL事务</param>
        /// <param name="commandType">SQL命令的类型，COMMANDTYPE枚举类型</param>
        /// <param name="commandText">SQL命令</param>
        /// <param name="commandParameters">参数集合数组</param>
        /// <returns>返回单值</returns>
        public static object ExecuteScalar(SqlTransaction transaction, CommandType commandType, string commandText, SqlParameter[] commandParameters)
        {
            return ExecuteScalar(transaction.Connection, transaction, commandType, commandText, commandParameters);
        }

        /// <summary>
        /// 执行SQL命令返回单值，例如：返回 SELECT COUNT（*） FROM TABLES 的命令的结果
        /// </summary>
        /// <param name="transaction">SQL事务</param>
        /// <param name="commandType">SQL命令的类型，COMMANDTYPE枚举类型</param>
        /// <param name="commandText">SQL命令</param>
        /// <returns>返回单值</returns>
        public static object ExecuteScalar(SqlTransaction transaction, CommandType commandType, string commandText)
        {
            return ExecuteScalar(transaction, commandType, commandText, (SqlParameter[])null);
        }


        /// <summary>
        /// 执行SQL命令返回单值，例如：返回 SELECT COUNT（*） FROM TABLES 的命令的结果
        /// </summary>
        ///<param name="transaction">SQL事务</param>
        /// <param name="spName">存储过程名称</param>
        /// <param name="parameterValues">针对存储过程的参数结构的值数组</param>
        /// <returns>返回单值</returns>
        public static object ExecuteScalar(SqlTransaction transaction, string spName, object[] parameterValues)
        {
            return ExecuteScalar(transaction.Connection.ConnectionString, spName, parameterValues);
        }


        /// <summary>
        /// 执行SQL命令返回单值，例如：返回 SELECT COUNT（*） FROM TABLES 的命令的结果
        /// </summary>
        /// <param name="connectionString" >SQL连接字符串</param>
        /// <param name="commandType">SQL命令的类型，COMMANDTYPE枚举类型</param>
        /// <param name="commandText">SQL命令</param>
        /// <param name="commandParameters">参数集合数组</param>
        /// <returns>返回单值</returns>
        public static object ExecuteScalar(string connectionString, CommandType commandType, string commandText, SqlParameter[] commandParameters)
        {
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                try
                {
                    cn.Open();
                    return ExecuteScalar(cn, commandType, commandText, commandParameters);

                }
                catch
                {
                    return "Connect failed";

                }
            }
        }


        /// <summary>
        /// 执行SQL命令返回单值，例如：返回 SELECT COUNT（*） FROM TABLES 的命令的结果
        /// </summary>
        /// <param name="connectionString" >SQL连接字符串</param>
        /// <param name="commandType">SQL命令的类型，COMMANDTYPE枚举类型</param>
        /// <param name="commandText">SQL命令</param>
        /// <returns>返回单值</returns>
        public static object ExecuteScalar(string connectionString, CommandType commandType, string commandText)
        {
            return ExecuteScalar(connectionString, commandType, commandText, (SqlParameter[])null);
        }

        /// <summary>
        /// 执行SQL命令返回单值，例如：返回 SELECT COUNT（*） FROM TABLES 的命令的结果
        /// </summary>
        /// <param name="connectionString">SQL连接字符串</param>
        /// <param name="commandText">SQL命令</param>
        /// <returns>返回单值</returns>
        public static object ExecuteScalar(string connectionString, string commandText)
        {
            return ExecuteScalar(connectionString, CommandType.Text, commandText);
        }

        /// <summary>
        /// 执行SQL命令返回单值，例如：返回 SELECT COUNT（*） FROM TABLES 的命令的结果
        /// </summary>
        /// <param name="connectionString" >SQL连接字符串</param>
        /// <param name="spName">存储过程名称</param>
        /// <param name="parameterValues">针对存储过程的参数结构的值数组</param>
        /// <returns>返回单值</returns>
        public static object ExecuteScalar(string connectionString, string spName, object[] parameterValues)
        {
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                SqlParameter[] commandParameter = SqlCache.GetSpParameterSet(connectionString, spName);

                AssignParameterValues(commandParameter, parameterValues);

                return ExecuteScalar(connectionString, CommandType.StoredProcedure, spName, commandParameter);
            }
            else
            {
                return ExecuteScalar(connectionString, CommandType.StoredProcedure, spName);
            }
        }
        #endregion


        #region MakeParam

        public static SqlParameter MakeInParam(string paramName, SqlDbType paramDbType, int paramSize, object paramValue)
        {
            SqlParameter param = new SqlParameter(paramName, paramDbType, paramSize);
            param.Direction = ParameterDirection.Input;
            param.Value = paramValue;

            return param;
        }

        public static SqlParameter MakeInParam(string paramName, SqlDbType paramDbType, object paramValue)
        {
            SqlParameter param = new SqlParameter(paramName, paramDbType);
            param.Direction = ParameterDirection.Input;
            param.Value = paramValue;

            return param;
        }

        public static SqlParameter MakeOutParam(string paramName, SqlDbType paramDbType)
        {
            SqlParameter param = new SqlParameter(paramName, paramDbType);
            param.Direction = ParameterDirection.Output;

            return param;
        }

        public static SqlParameter MakeOutParam(string paramName, SqlDbType paramDbType, int paramSize)
        {
            SqlParameter param = new SqlParameter(paramName, paramDbType, paramSize);
            param.Direction = ParameterDirection.Output;

            return param;
        }


        public static SqlParameter MakeParam(string paramName, SqlDbType paramDbType, int paramSize, ParameterDirection direction, object paramValue)
        {
            SqlParameter param = new SqlParameter(paramName, paramDbType, paramSize);
            param.Direction = direction;
            param.Value = paramValue;

            return param;
        }

        public static SqlParameter MakeParam(string paramName, SqlDbType paramDbType, ParameterDirection direction, object paramValue)
        {
            SqlParameter param = new SqlParameter(paramName, paramDbType);
            param.Direction = direction;
            param.Value = paramValue;

            return param;
        }

        #endregion

        #region 获取可空类型的值 GetDecimalNullable
        /// <summary>
        /// 获取可空类型的值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static decimal? GetDecimalNullable(object obj)
        {
            if (obj == null ||
                obj == DBNull.Value ||
                string.IsNullOrEmpty(obj.ToString()))
            {
                return null;
            }

            return Convert.ToDecimal(obj.ToString());
        }
        #endregion

        #region 获取可空类型的值 GetIntNullable
        /// <summary>
        /// 获取可空类型的值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int? GetIntNullable(object obj)
        {
            if (obj == null ||
                obj == DBNull.Value ||
                string.IsNullOrEmpty(obj.ToString()))
            {
                return null;
            }

            return Convert.ToInt32(obj.ToString());
        }
        #endregion

        #region 获取可空类型的值 GetDateTimeNullable
        /// <summary>
        /// 获取可空类型的值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static DateTime? GetDateTimeNullable(object obj)
        {
            if (obj == null ||
                obj == DBNull.Value ||
                string.IsNullOrEmpty(obj.ToString()))
            {
                return null;
            }

            return Convert.ToDateTime(obj.ToString());
        }
        #endregion



        //private static string GetCon()
        //{
        //    string contt = System.Configuration.ConfigurationManager.ConnectionStrings["zrsmsEntities"].ToString();
        //    int i = contt.IndexOf("Data Source");
        //    int j = contt.IndexOf("MultipleActiveResultSets");
        //    string constr = contt.Substring(i, j - i).Replace("Data Source", "server").Replace("Initial Catalog", "database").Replace("Persist Security Info", "Enlist").Replace("True", "false");
        //    return constr;
        //}
    }




    /// <summary>
    /// 参数缓存器
    /// </summary>
    public sealed class SqlCache
    {
        #region 内部方法
        private SqlCache() { }

        //创建静态的Hashtable（表示键/值对的集合）类型的变量，做为参数集合的缓存器
        private static Hashtable paramCache = Hashtable.Synchronized(new Hashtable());

        /// <summary>
        /// 从数据库中获得存储过程的参数集合
        /// </summary>
        ///<param name="connectionString">sql server连接字符串</param>
        /// <param name="SpName">存储过程名称</param>
        /// <param name="IsReturnValueParameter">是否返回值参数</param>
        /// <returns>存储过程的参数集合</returns>
        private static SqlParameter[] DiscoverSpParameterSet(string connectionString, string SpName, bool IsReturnValueParameter)
        {
            using (SqlConnection cn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(SpName, cn))
            {
                cn.Open();

                cmd.CommandType = CommandType.StoredProcedure;

                //将参数集合从数据库中取出给CMD的Parameter
                SqlCommandBuilder.DeriveParameters(cmd);

                if (!IsReturnValueParameter)
                {
                    //删除VALUE
                    cmd.Parameters.RemoveAt(0);
                }

                SqlParameter[] arrParameter = new SqlParameter[cmd.Parameters.Count];

                cmd.Parameters.CopyTo(arrParameter, 0);

                return arrParameter;
            }

        }
        /// <summary>
        /// 创建SqlParameter副本
        /// </summary>
        /// <param name="originalParameters">源SqlParameter对象数组</param>
        /// <returns>SqlParameter副本</returns>
        private static SqlParameter[] CloneParameter(SqlParameter[] originalParameters)
        {
            SqlParameter[] arrParameter = new SqlParameter[originalParameters.Length];

            for (int i = 0, j = arrParameter.Length; i < j; i++)
            {
                //创建作为当前实例副本的新对象
                arrParameter[i] = (SqlParameter)((ICloneable)originalParameters[i]).Clone();
            }

            return arrParameter;
        }
        #endregion

        #region 管理存储过程参数集合缓存

        /// <summary>
        /// 将参数集合写入缓存器
        /// </summary>
        /// <param name="connectionString">SQL SERVER连接字符串</param>
        ///<param name="commandText">SQL命令</param>
        /// <param name="cmmandParameters">参数集合</param>
        public static void CacheParameterSet(string connectionString, string commandText, params SqlParameter[] cmmandParameters)
        {
            //
            string CacheName = connectionString + ":" + commandText;

            //将参数集合写入缓存器
            paramCache[CacheName] = cmmandParameters;
        }

        public static SqlParameter[] GetCacheParameterSet(string connectionString, string commandText)
        {
            string CacheName = connectionString + ":" + commandText;

            SqlParameter[] cacheParameter = (SqlParameter[])paramCache[CacheName];

            if (cacheParameter == null)
            {
                return null;
            }
            else
            {
                return CloneParameter(cacheParameter);
            }
        }

        /// <summary>
        /// 从缓存中取出存储过程的参数集合
        /// </summary>
        /// <param name="connectionString">SQL SERVER连接字符串</param>
        /// <param name="spName">存储过程名称</param>
        /// <param name="isReturnValueParameter">是否返回值参数</param>
        /// <returns></returns>
        public static SqlParameter[] GetSpParameterSet(string connectionString, string spName, bool isReturnValueParameter)
        {
            string CacheName = connectionString + ":" + spName + (isReturnValueParameter ? ": ReturnValueParameter" : "");

            SqlParameter[] arrParameter = (SqlParameter[])paramCache[CacheName];

            if (arrParameter == null)
            {
                //从数据库中取出参数集合
                paramCache[CacheName] = DiscoverSpParameterSet(connectionString, spName, isReturnValueParameter);

                arrParameter = (SqlParameter[])paramCache[CacheName];
            }

            return CloneParameter(arrParameter);
        }

        /// <summary>
        /// 从缓存中取出存储过程的参数集合
        /// </summary>
        /// <param name="connectionString">SQL SERVER连接字符串</param>
        /// <param name="spName">存储过程名称</param>
        /// <returns></returns>
        public static SqlParameter[] GetSpParameterSet(string connectionString, string spName)
        {
            return GetSpParameterSet(connectionString, spName, false);
        }
        #endregion

    }
}