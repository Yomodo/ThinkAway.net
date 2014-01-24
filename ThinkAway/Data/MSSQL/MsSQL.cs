using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
/*
 * Lsong
 * i@lsong.org
 * http://lsong.org
 */
namespace ThinkAway.Data.MSSQL
{
    /// <summary>
    /// MSSql 数据操作类
    /// </summary>
    public class MsSQL : DbHelper
    {
        private string _database;

        /// <summary>
        /// 获取或设置数据库名称
        /// </summary>
        public string Database
        {
            get { return _database; }
            set { _database = value; }
        }

        private string _userName;

        /// <summary>
        /// 获取或设置数据连接的登录名
        /// </summary>
        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }

        private string _password;

        /// <summary>
        /// 获取或设置数据连接的登录密码
        /// </summary>
        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        private string _dataSource;

        /// <summary>
        /// 获取或设置数据连接的数据源
        /// </summary>
        public string DataSource
        {
            get { return _dataSource; }
            set { _dataSource = value; }
        }

        /// <summary>
        /// 表示 SQL Server 数据库的一个打开的连接。无法继承此类。
        /// </summary>
        private SqlConnection _sqlConnection;

        /// <summary>
        /// 根据当前应用程序的默认配置名称获取连接字符串创建数据访问实例
        /// </summary>
        /// <param name="connectionName"></param>
        public MsSQL(string connectionName): base(connectionName)
        {
        }
        /// <summary>
        /// 根据指定的数据源和登录用户名和密码创建数据访问实例
        /// </summary>
        /// <param name="dataSource">数据源</param>
        /// <param name="databaseName"></param>
        /// <param name="userId">用户名</param>
        /// <param name="password">密码</param>
        public MsSQL(string dataSource, string databaseName, string userId, string password): base(string.Empty)
        {
            DataSource = dataSource;
            Database = databaseName;
            UserName = userId;
            Password = password;
            //
            UpdateConnectionString();
        }
        /// <summary>
        /// 创建数据连接并尝试打开该数据连接
        /// <a>该函数将始终重复使用同一连接,如果需要使用新的连接实例,请确保使用前调用 <c>Close</c> 函数</a>
        /// </summary>
        /// <returns></returns>
        public override DbConnection Open()
        {
            if (_sqlConnection == null)
            {
                _sqlConnection = new SqlConnection(ConnectionString);
            }
            if (_sqlConnection.State != ConnectionState.Open)
            {
                _sqlConnection.Open();
            }
            return _sqlConnection;
        }
        /// <summary>
        /// 关闭该实例中的使用的数据连接
        /// </summary>
        public override void Close()
        {
            if (_sqlConnection != null && _sqlConnection.State != ConnectionState.Closed)
            {
                _sqlConnection.Close();
            }
        }

        private void UpdateConnectionString()
        {
            ConnectionString = string.Format("Data Source={0};Initial Catalog={1};User ID={2};Password={3};", DataSource, Database, UserName, Password);
        }

        /// <summary>
        /// 对数据访问类执行 T-SQL 并返回一个数据指针
        /// </summary>
        /// <param name="sql"></param>
        /// <exception cref="DbException"></exception>
        /// <returns></returns>
        public override Cursor Query(string sql)
        {
            Cursor cursor;
            try
            {
                SqlConnection sqlConnection = (SqlConnection)Open();
                SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection);
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader(CommandBehavior.Default);
                cursor = new Cursor(sqlDataReader);
            }
            catch (Exception exception)
            {
                DbException dbException = new DbException(string.Format("执行 Query 时发生意外:{0}", sql), exception);
                dbException.Sql = sql;
                throw dbException;
            }
            return cursor;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public override DataSet GetDataSet(string sql)
        {
            DataSet dataSet = new DataSet();
            SqlConnection sqlConnection = (SqlConnection)Open();
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sql, sqlConnection);
            int fill = sqlDataAdapter.Fill(dataSet);
            return dataSet;
        }

        /// <summary>
        /// 对数据访问类执行 T-SQL 并返回影响的行数
        /// </summary>
        /// <param name="sql"></param>
        /// <exception cref="DbException"></exception>
        /// <returns></returns>
        public override int ExecSql(string sql)
        {
            int result;
            try
            {
                SqlConnection sqlConnection = (SqlConnection)Open();
                SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection);
                result = sqlCommand.ExecuteNonQuery();
            }
            catch (Exception innerException)
            {
                DbException exception = new DbException(string.Format("执行 ExecSql 时发生意外:{0}", sql), innerException);
                exception.Sql = sql;
                throw exception;
            }
            return result;
        }
    }
}
