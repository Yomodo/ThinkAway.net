using System.Data;
using System.Data.Common;
using System.Data.OleDb;
/*
 * Lsong
 * i@lsong.org
 * http://lsong.org
 */
namespace ThinkAway.Data.Access
{
    /// <summary>
    /// AcceHelper 的摘要说明
    /// </summary>
    public class Access : DbHelper
    {
        private OleDbConnection _oleDbConnection;

        #region Implementation of IDbHelper

        /// <summary>
        /// 根据当前应用程序的默认配置名称获取连接字符串创建数据访问实例
        /// </summary>
        /// <param name="connectionName"></param>
        public Access(string connectionName) : base(connectionName)
        {
        }
        public Access(string path,string userId , string password) : base(string.Empty)
        {
            string connectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}", path);
            ConnectionString = connectionString;
        }

        public override Cursor Query(string sql)
        {
            OleDbConnection oleDbConnection = (OleDbConnection)Open();
            OleDbCommand oleDbCommand = new OleDbCommand(sql, oleDbConnection);
            OleDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader(CommandBehavior.Default);
            Cursor cursor = new Cursor(oleDbDataReader);
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
            OleDbConnection oleDbConnection = (OleDbConnection)Open();
            OleDbCommand oleDbCommand = new OleDbCommand(sql, oleDbConnection);
            OleDbDataAdapter oleDbDataAdapter = new OleDbDataAdapter(oleDbCommand);
            oleDbDataAdapter.Fill(dataSet);
            return dataSet;
        }

        public override int ExecSql(string sql)
        {
            OleDbConnection oleDbConnection = (OleDbConnection) Open();
            OleDbCommand oleDbCommand = new OleDbCommand(sql, oleDbConnection);
            return oleDbCommand.ExecuteNonQuery();
        }
        
        #endregion

        public override DbConnection Open()
        {
            if (_oleDbConnection == null)
            {
                _oleDbConnection = new OleDbConnection(ConnectionString);
            }
            if (_oleDbConnection.State != ConnectionState.Open)
            {
                _oleDbConnection.Open();
            }
            return _oleDbConnection;
        }

        public override void Close()
        {
            if (_oleDbConnection != null && _oleDbConnection.State != ConnectionState.Closed)
            {
                _oleDbConnection.Close();
            }
        }
    }
}
