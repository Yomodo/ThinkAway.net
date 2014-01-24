using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using ThinkAway.Text.SQL;
/*
 * Lsong
 * i@lsong.org
 * http://lsong.org
 */
namespace ThinkAway.Data
{
    

    /// <summary>
    /// 表示数据操作连接,无法创建此类的实例
    /// <br></br>
    /// <example>
    /// eg:
    /// 
    /// 创建数据操作 可以通过多种方式 
    ///可以通过 IDbHelper 接口方式创建
    ///也可以 用抽象类 DbHelper 创建 
    ///还可以 用实例类 创建 , 如: MSSQL mssql = new MSSQL("test");
    ///操作接口中至少包含一个重载构造 那就是 读取项目文件中的配置的连接字符串
    ///子需要提供连接字符串的名字即可
    ///根据实际情况的不同可能还会提供更加丰富的重载 
    ///比如:dbHelper = new MSSQL(".","TESTDB","sa","123456");
    ///除此之外还提供了晚期指定连接字符串的机制
    ///dbHelper.ConnectionString = "Data Source=192.168.0.74;Initial Catalog=TEST;User ID=sa;Password=123456;";
    ///但是需要注意的是 连接字符串需要在 Open 函数调用之前设置
    /// </example>
    /// </summary>
    public abstract class DbHelper : SqlHelper, IDbHelper
    {
        /// <summary>
        /// 获取或设置用于数据访问的连接字符串
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// 根据当前应用程序的默认配置名称获取连接字符串创建数据访问实例
        /// </summary>
        /// <param name="connectionName"></param>
        protected DbHelper(string connectionName)
        {
            if (!String.IsNullOrEmpty(connectionName))
            {
                ConnectionString = ConfigurationManager.ConnectionStrings[connectionName].ConnectionString;
            }
        }

        /// <summary>
        /// 打开数据连接
        /// </summary>
        /// <returns></returns>
        public abstract DbConnection Open();

        /// <summary>
        /// 关闭数据库联接
        /// </summary>
        public abstract void Close();


        public int CreateTable(string tableName, TableValues tableValues)
        {
            string sql = CreateTableSQL(tableName,tableValues);
            return ExecSql(sql);
        }

        /// <summary>
        /// 对数据访问类执行 T-SQL 并返回一个数据指针
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public abstract Cursor Query(string sql);

        /// <summary>
        /// 对数据访问类执行 T-SQL 并返回影响的行数
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public abstract int ExecSql(string sql);

        /// <summary>
        /// 根据指定的 <c>表名称</c> <c>列名称</c> <c>条件表达式</c> <c>条件参数</c>执行数据查询操作并返回一个数据指针
        /// </summary>
        /// <param name="tableName">表名称</param>
        /// <param name="columnNames">列名称</param>
        /// <param name="whereSql">条件表达式</param>
        /// <param name="whereArgs">条件参数</param>
        /// <summary>执行数据操作并</summary>
        /// <returns>返回一个数据指针</returns>
        public virtual Cursor Query(string tableName, string[] columnNames, string whereSql, DataValues whereArgs)
        {
            string sql = CreateSelectSQL(tableName, columnNames, whereSql, whereArgs);
            return Query(sql);
        }

        /// <summary>
        /// GetDataSet
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public abstract DataSet GetDataSet(string sql);

        /// <summary>
        /// GetDataSet
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columnNames"></param>
        /// <param name="whereSql"></param>
        /// <param name="whereArgs"></param>
        /// <returns></returns>
        public virtual DataSet GetDataSet(string tableName, string[] columnNames, string whereSql,DataValues whereArgs)
        {
            string sql = CreateSelectSQL(tableName, columnNames, whereSql, whereArgs);
            return GetDataSet(sql);
        }

        /// <summary>
        /// 根据指定的 tableName  和 DataValues 在 数据连接上执行插入操作
        /// </summary>
        /// <example>插入方法需要提供 要插入到的表名称 和 要插入的数据 DataValues 返回 大于0 表示成功</example>
        /// <param name="tableName">tableName</param>
        /// <param name="DataValues">DataValues</param>
        /// <returns>rows</returns>
        public virtual int Insert(string tableName, DataValues DataValues)
        {

            string sql = CreateInsertSQL(tableName, DataValues);
            return ExecSql(sql);
        }

        /// <summary>
        /// 根据指定的 tableName,values,whereSql  和 whereArgs  在 数据连接上执行更新操作
        /// </summary>
        /// <param name="tableName">tableName</param>
        /// <param name="values">values</param>
        /// <param name="whereSql">whereSql</param>
        /// <param name="whereArgs">whereArgs</param>
        /// <returns></returns>
        public virtual int Update(string tableName, DataValues values, string whereSql, DataValues whereArgs)
        {
            string sql = CreateUpdateSQL(tableName, values, whereSql, whereArgs);
            return ExecSql(sql);
        }

        /// <summary>
        /// 根据指定的 tableName,whereSql  和 whereArgs  在 数据连接上执行删除操作
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="whereSql"></param>
        /// <param name="whereArgs"></param>
        /// <returns></returns>
        public virtual int Delete(string tableName, string whereSql, DataValues whereArgs)
        {
            string sql = CreateDeleteSQL(tableName,whereSql,whereArgs);
            return ExecSql(sql);
        }


    }
}
