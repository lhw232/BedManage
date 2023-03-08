using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace HW.Utils
{
    public static class JsonHelper
    {
        /// <summary>
        /// 将对象转换为Json字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToJson(object obj)
        {
            if (obj is DataTable)
            {
                DataTable table = obj as DataTable;
                return DataTable2Json(table);
            }
            if (obj is DataSet)
            {
                DataSet daSet = obj as DataSet;
                return Dataset2Json(daSet);
            }
            System.Web.Script.Serialization.JavaScriptSerializer diSer = new System.Web.Script.Serialization.JavaScriptSerializer();
            return diSer.Serialize(obj);
        }
        /// <summary>
        /// 解析当前页回传的视图状态
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static Dictionary<string, object> JsonToDic(string json)
        {
            Dictionary<string, object> list = new Dictionary<string, object>();
            System.Web.Script.Serialization.JavaScriptSerializer diSer = new System.Web.Script.Serialization.JavaScriptSerializer();
            return diSer.Deserialize<Dictionary<string, object>>(json);
        }

        public static T JsonToObj<T>(string json)
        {
            System.Web.Script.Serialization.JavaScriptSerializer diSer = new System.Web.Script.Serialization.JavaScriptSerializer();
            return diSer.Deserialize<T>(json);
        }

        #region dataTable转换成Json格式
        /// <summary>  
        /// dataTable转换成Json格式  
        /// </summary>  
        /// <param name="dt"></param>  
        /// <returns></returns>  
        public static string DataTableToJson(this DataTable dt)
        {
            int pagecount = 1;

            StringBuilder jsonBuilder = new StringBuilder();
            jsonBuilder.Append("{");
            jsonBuilder.Append("\"count\":");
            jsonBuilder.Append("\"" + dt.Rows.Count + "\",");
            jsonBuilder.Append("\"pagecount\":");
            jsonBuilder.Append("\"" + pagecount + "\",");
            jsonBuilder.Append("\"data\":[");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                jsonBuilder.Append("{");
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    jsonBuilder.Append("\"");
                    jsonBuilder.Append(dt.Columns[j].ColumnName);
                    jsonBuilder.Append("\":\"");
                    jsonBuilder.Append(dt.Rows[i][j].ToString());
                    jsonBuilder.Append("\",");
                }
                jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
                jsonBuilder.Append("},");
            }

            if (dt.Rows.Count > 0)
            {
                jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
            }

            jsonBuilder.Append("]");
            jsonBuilder.Append("}");
            return jsonBuilder.ToString();
        }

        /// <summary>  
        /// dataTable转换成Json格式  
        /// </summary>  
        /// <param name="dt"></param>  
        /// <returns></returns>  
        public static string DataTableToJson(this DataTable dt, int pagecount, int recordcount, object o = null)
        {
            StringBuilder jsonBuilder = new StringBuilder();
            jsonBuilder.Append("{");
            if (o != null)
            {
                Dictionary<string, object> dict = o as Dictionary<string, object>;
                try
                {
                    foreach (var s in dict.Keys)
                    {
                        jsonBuilder.Append("\"" + s + "\":");
                        jsonBuilder.Append("\"" + Convert.ToString(dict[s]) + "\",");
                    }
                }
                catch { }
            }

            jsonBuilder.Append("\"count\":");
            jsonBuilder.Append("\"" + dt.Rows.Count + "\",");
            jsonBuilder.Append("\"pagecount\":");
            jsonBuilder.Append("\"" + pagecount + "\",");
            jsonBuilder.Append("\"recordcount\":");
            jsonBuilder.Append("\"" + recordcount + "\",");
            jsonBuilder.Append("\"data\":[");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                jsonBuilder.Append("{");
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    jsonBuilder.Append("\"");
                    jsonBuilder.Append(dt.Columns[j].ColumnName);
                    jsonBuilder.Append("\":\"");
                    jsonBuilder.Append(dt.Rows[i][j].ToString());
                    jsonBuilder.Append("\",");
                }
                jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
                jsonBuilder.Append("},");
            }

            if (dt.Rows.Count > 0)
            {
                jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
            }

            jsonBuilder.Append("]");
            jsonBuilder.Append("}");
            return jsonBuilder.ToString();
        }

        /// <summary>  
        /// dataTable转换成Json格式  
        /// </summary>  
        /// <param name="dt"></param>  
        /// <returns></returns>  
        public static string DataTable2Json(this DataTable dt)
        {
            StringBuilder jsonBuilder = new StringBuilder();
            jsonBuilder.Append("[");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                jsonBuilder.Append("{");
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    jsonBuilder.Append("\"");
                    jsonBuilder.Append(dt.Columns[j].ColumnName);
                    jsonBuilder.Append("\":\"");
                    jsonBuilder.Append(dt.Rows[i][j].ToString());
                    jsonBuilder.Append("\",");
                }
                jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
                jsonBuilder.Append("},");
            }

            if (dt.Rows.Count > 0)
            {
                jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
            }
            jsonBuilder.Append("]");
            return jsonBuilder.ToString();
        }

        #endregion dataTable转换成Json格式
        #region DataSet转换成Json格式
        /// <summary>  
        /// DataSet转换成Json格式  
        /// </summary>  
        /// <param name="ds">DataSet</param> 
        /// <returns></returns>  
        private static string Dataset2Json(DataSet ds)
        {
            StringBuilder json = new StringBuilder();
            json.Append("{");
            foreach (DataTable dt in ds.Tables)
            {

                json.Append("\"" + dt.TableName);
                json.Append("\":");
                json.Append(DataTable2Json(dt));
                json.Append(",");

            }
            json.Remove(json.ToString().LastIndexOf(','), 1);
            return json.Append("}").ToString();
        }
        #endregion

        /// <summary>
        /// Msdn
        /// </summary>
        /// <param name="jsonName"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        private static string DataTableToJson(string jsonName, DataTable dt)
        {
            StringBuilder Json = new StringBuilder();
            Json.Append("{\"" + jsonName + "\":[");
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Json.Append("{");
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        Json.Append("\"" + dt.Columns[j].ColumnName.ToString() + "\":\"" + dt.Rows[i][j].ToString() + "\"");
                        if (j < dt.Columns.Count - 1)
                        {
                            Json.Append(",");
                        }
                    }
                    Json.Append("}");
                    if (i < dt.Rows.Count - 1)
                    {
                        Json.Append(",");
                    }
                }
            }
            Json.Append("]}");
            return Json.ToString();
        }
    }
}