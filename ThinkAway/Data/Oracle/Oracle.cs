using System.Data;
using System.Data.Common;
using System.Data.OracleClient;
/*
 * Lsong
 * i@lsong.org
 * http://lsong.org
 */
namespace ThinkAway.Data.Oracle
{
    /// <summary>
    /// Oracle数据库操作类
    /// </summary>
    public class Oracle : DbHelper
    {
        private OracleConnection _oracleConnection;

        /// <summary>
        /// 根据当前应用程序的默认配置名称获取连接字符串创建数据访问实例
        /// </summary>
        /// <param name="connectionName"></param>
        public Oracle(string connectionName) : base(connectionName)
        {
        }

        public override DbConnection Open()
        {
            if(_oracleConnection == null)
            {
                _oracleConnection = new OracleConnection(ConnectionString);
            }
            if(_oracleConnection.State != ConnectionState.Open)
            {
                _oracleConnection.Open();
            }
            return _oracleConnection;
        }

        public override void Close()
        {
            if(_oracleConnection!= null && _oracleConnection.State != ConnectionState.Closed)
            {
                _oracleConnection.Close();
            }
        }

        public override Cursor Query(string sql)
        {
            OracleConnection oracleConnection = (OracleConnection) Open();
            OracleCommand oracleCommand = new OracleCommand(sql,oracleConnection);
            OracleDataReader oracleDataReader = oracleCommand.ExecuteReader();
            Cursor cursor = new Cursor(oracleDataReader);
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
            OracleConnection oracleConnection = (OracleConnection)Open();
            OracleCommand oracleCommand = new OracleCommand(sql, oracleConnection);
            OracleDataAdapter oracleDataAdapter = new OracleDataAdapter(oracleCommand);
            oracleDataAdapter.Fill(dataSet);
            return dataSet;
        }

        public override int ExecSql(string sql)
        {
            OracleConnection oracleConnection = (OracleConnection)Open();
            OracleCommand oracleCommand = new OracleCommand(sql, oracleConnection);
            int result = oracleCommand.ExecuteNonQuery();
            return result;
        }
    }
}
