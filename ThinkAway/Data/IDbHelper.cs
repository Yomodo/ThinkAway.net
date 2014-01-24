using System.Data;
using System.Data.Common;
/*
 * Lsong
 * i@lsong.org
 * http://lsong.org
 */
namespace ThinkAway.Data
{
    /// <summary>
    /// 表示数据操作连接
    /// </summary>
    public interface IDbHelper
    {
        /// <summary>
        /// 创建数据连接并尝试打开该数据连接
        /// </summary>
        /// <returns></returns>
        DbConnection Open();
        /// <summary>
        /// 关闭该实例中的使用的数据连接
        /// </summary>
        void Close();
        /// <summary>
        /// 对数据访问类执行 T-SQL 并返回影响的行数
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        int ExecSql(string sql);
        /// <summary>
        /// 根据指定的表名称以及所需要的数据创建数据表
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="tableValues"></param>
        /// <returns></returns>
        int CreateTable(string tableName,TableValues tableValues);
        /// <summary>
        /// 对数据访问类执行 T-SQL 并返回一个数据指针
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        Cursor Query(string sql);

        /// <summary>
        /// 根据指定的 <c>表名称</c> <c>列名称</c> <c>条件表达式</c> <c>条件参数</c>执行数据查询操作并返回一个数据指针
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columnNames"></param>
        /// <param name="whereSql"></param>
        /// <param name="whereArgs"></param>
        /// <returns></returns>
        Cursor Query(string tableName, string[] columnNames, string whereSql, DataValues whereArgs);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        DataSet GetDataSet(string sql);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columnNames"></param>
        /// <param name="whereSql"></param>
        /// <param name="whereArgs"></param>
        /// <returns></returns>
        DataSet GetDataSet(string tableName, string[] columnNames, string whereSql, DataValues whereArgs);
        /// <summary>
        /// 根据指定的 <param name="tableName">表名称</param>执行插入操作
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="DataValues"></param>
        /// <returns></returns>
        int Insert(string tableName, DataValues DataValues);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="values"></param>
        /// <param name="whereSql"></param>
        /// <param name="whereArgs"></param>
        /// <returns></returns>
        int Update(string tableName, DataValues values, string whereSql, DataValues whereArgs);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="whereSql"></param>
        /// <param name="whereArgs"></param>
        /// <returns></returns>
        int Delete(string tableName, string whereSql, DataValues whereArgs);
    }
}
