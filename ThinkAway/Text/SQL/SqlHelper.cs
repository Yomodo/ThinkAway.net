using System;
using System.Text;
using ThinkAway.Data;
/*
 * Lsong
 * i@lsong.org
 * http://lsong.org
 */
namespace ThinkAway.Text.SQL
{
    /// <summary>
    /// 提供了一种关于 T-SQL 语法的简单实现
    /// </summary>
    public class SqlHelper : SqlBase
    {
        private const string SPACE_CHAR = " ";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="tableValues"></param>
        /// <returns></returns>
        protected virtual string CreateTableSQL(string tableName, TableValues tableValues)
        {
            if (tableValues == null)
            {
                throw new ArgumentNullException("tableValues");
            }
            StringBuilder stringBuilder = new StringBuilder("CREATE TABLE");
            stringBuilder.Append(SPACE_CHAR);
            stringBuilder.Append(tableName);
            stringBuilder.Append(";");
            return stringBuilder.ToString();
        }
        /// <summary>
        /// CreateSelectSQL
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columnNames"></param>
        /// <param name="whereSql"></param>
        /// <param name="whereArgs"></param>
        /// <returns></returns>
        protected virtual string CreateSelectSQL(string tableName, string[] columnNames, string whereSql, DataValues whereArgs)
        {
            StringBuilder stringBuilder = new StringBuilder("SELECT");
            stringBuilder.Append(" ");
            if (columnNames == null || columnNames.Length == 0)
            {
                stringBuilder.Append("*");
            }
            else
            {
                for (int i = 0; i < columnNames.Length; i++)
                {
                    string columnName = columnNames[i];
                    stringBuilder.AppendFormat("[{0}]", columnName);
                    if ((i + 1) < columnNames.Length)
                    {
                        stringBuilder.Append(",");
                    }
                }
            }
            if (!String.IsNullOrEmpty(tableName))
            {
                stringBuilder.Append(" ");
                stringBuilder.Append("FROM");
                stringBuilder.Append(" ");
                stringBuilder.AppendFormat("[{0}]", tableName);
            }
            if (!String.IsNullOrEmpty(whereSql))
            {
                whereSql = CreateWhereSql(whereSql, whereArgs);
                stringBuilder.Append(" ");
                stringBuilder.Append(whereSql);
            }
            stringBuilder.Append(";");
            return stringBuilder.ToString();
        }
        /// <summary>
        /// CreateWhereSql
        /// </summary>
        /// <param name="whereSql"></param>
        /// <param name="whereArgs"></param>
        /// <returns></returns>
        protected virtual string CreateWhereSql(string whereSql, DataValues whereArgs)
        {
            if (String.IsNullOrEmpty(whereSql))
            {
                throw new ArgumentException("whereSql");
            }

            StringBuilder stringBuilder = new StringBuilder("WHERE");
            string[] args = whereSql.Split(',', ';');
            if (args.Length > 0)
            {
                stringBuilder.Append(" ");
                for (int i = 0; i < args.Length; i++)
                {
                    string str = args[i];
                    string[] aa = str.Split('=', '<', '>');
                    string key = aa[0].Trim();
                    string value = aa[1].Trim();
                    if (Equals(value, "?"))
                    {
                        if (whereArgs == null || whereArgs.Count == 0)
                        {
                            throw new ArgumentException("contentValues");
                        }
                        value = whereArgs.Get(key);
                    }
                    stringBuilder.AppendFormat("[{0}] = {1}", key, value);
                    if ((i + 1) < args.Length)
                    {
                        stringBuilder.Append(" ");
                        stringBuilder.Append("and");
                        stringBuilder.Append(" ");
                    }
                }
            }
            return stringBuilder.ToString();
        }
        /// <summary>
        /// CreateInsertSQL
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="contentValues"></param>
        /// <returns></returns>
        public string CreateInsertSQL(string tableName, DataValues contentValues)
        {
            StringBuilder stringBuilder = new StringBuilder("INSERT INTO");
            stringBuilder.Append(" ");
            if (String.IsNullOrEmpty(tableName))
            {
                throw new ArgumentException("tableName");
            }
            stringBuilder.AppendFormat("[{0}]", tableName);

            if (contentValues == null || contentValues.Count == 0)
            {
                throw new ArgumentException("contentValues");
            }
            string[] keys = contentValues.Keys;
            StringBuilder kBuilder = new StringBuilder();
            StringBuilder vBuilder = new StringBuilder();
            for (int i = 0; i < keys.Length; i++)
            {
                string key = keys[i];
                kBuilder.AppendFormat("[{0}]", key);
                vBuilder.AppendFormat("{0}", contentValues.Get(key));
                if ((i + 1) < keys.Length)
                {
                    kBuilder.Append(",");
                    vBuilder.Append(",");
                }
            }
            stringBuilder.AppendFormat("({0})", kBuilder);
            stringBuilder.AppendFormat("VALUES({0})", vBuilder);
            stringBuilder.Append(";");
            return stringBuilder.ToString();
        }
        /// <summary>
        /// CreateUpdateSQL
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="values"></param>
        /// <param name="whereSql"></param>
        /// <param name="whereArgs"></param>
        /// <returns></returns>
        public string CreateUpdateSQL(string tableName, DataValues values, string whereSql, DataValues whereArgs)
        {
            StringBuilder stringBuilder = new StringBuilder("UPDATE");
            stringBuilder.Append(" ");
            stringBuilder.AppendFormat("[{0}]", tableName);
            string[] keys = values.Keys;
            if (keys == null || keys.Length == 0)
            {
                throw new ArgumentException("values");
            }
            stringBuilder.Append(" ");
            stringBuilder.Append("SET");
            stringBuilder.Append(" ");
            for (int i = 0; i < values.Count; i++)
            {
                string key = keys[i];
                stringBuilder.AppendFormat("[{0}] = {1}", key, values.Get(key));
                if ((i + 1) < keys.Length)
                {
                    stringBuilder.Append(",");
                }
            }
            if (!String.IsNullOrEmpty(whereSql))
            {
                whereSql = CreateWhereSql(whereSql, whereArgs);
                stringBuilder.Append(" ");
                stringBuilder.Append(whereSql);
            }
            stringBuilder.Append(";");
            return stringBuilder.ToString();
        }
        /// <summary>
        /// CreateDeleteSQL
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="whereSql"></param>
        /// <param name="whereArgs"></param>
        /// <returns></returns>
        public string CreateDeleteSQL(string tableName, string whereSql, DataValues whereArgs)
        {
            StringBuilder stringBuilder = new StringBuilder("DELETE");
            stringBuilder.Append(" ");
            stringBuilder.Append("FROM");
            stringBuilder.Append(" ");
            stringBuilder.Append(tableName);
            stringBuilder.Append(" ");
            if (!String.IsNullOrEmpty(whereSql))
            {
                whereSql = CreateWhereSql(whereSql, whereArgs);
                stringBuilder.Append(" ");
                stringBuilder.Append(whereSql);
            }
            stringBuilder.Append(";");
            return stringBuilder.ToString();
        }
    }
}
