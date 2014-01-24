using System.Data;
using MySql.Data.MySqlClient;
/*
 * Lsong
 * i@lsong.org
 * http://lsong.org
 */
namespace ThinkAway.Data.MySQL
{
    /// <summary>
    ///MySQL 的摘要说明
    /// </summary>
    public class MySQL : DbHelper
    {
        private string _database;
        public string Database
        {
            get { return _database; }
            set { _database = value; }
        }

        private string _userName;
        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        private string _dataSource;
        public string DataSource
        {
            get { return _dataSource; }
            set { _dataSource = value; }
        }

        /// <summary>
        /// _mySqlConnection
        /// </summary>
        private MySqlConnection _mySqlConnection;

        /// <summary>
        /// 根据当前应用程序的默认配置名称获取连接字符串创建数据访问实例
        /// </summary>
        /// <param name="connectionName"></param>
        public MySQL(string connectionName)
            : base(connectionName)
        {
        }
        /// <summary>
        /// MySQL
        /// </summary>
        /// <param name="dataSource"></param>
        /// <param name="databaseName"></param>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        public MySQL(string dataSource, string databaseName, string userId, string password)
            : base(string.Empty)
        {
            UserName = userId;
            Password = password;
            DataSource = dataSource;
            Database = databaseName;
            //
            UpdateConnectionString();
        }


        private void UpdateConnectionString()
        {
            ConnectionString = string.Format("Data Source={0};Database={1};User ID={2};Password={3};charset='{4}';pooling={5}", DataSource, Database, UserName, Password,"utf8",true);
        }

        #region Implementation of IDbHelper

        public override int ExecSql(string sql)
        {
            MySqlConnection mySqlConnection = (MySqlConnection)Open();
            MySqlCommand mySqlCommand = new MySqlCommand(sql, mySqlConnection);
            int result = mySqlCommand.ExecuteNonQuery();
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public override DataSet GetDataSet(string sql)
        {
            DataSet dataSet = new DataSet();
            MySqlConnection mySqlConnection = (MySqlConnection)Open();
            MySqlCommand mySqlCommand = new MySqlCommand(sql, mySqlConnection);
            MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(mySqlCommand);
            mySqlDataAdapter.Fill(dataSet);
            return dataSet;
        }

        public override Cursor Query(string sql)
        {
            MySqlConnection mySqlConnection = (MySqlConnection) Open();
            MySqlCommand mySqlCommand = new MySqlCommand(sql,mySqlConnection);
            MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader();
            Cursor cursor = new Cursor(mySqlDataReader);
            return cursor;
        }

        #endregion

        public override System.Data.Common.DbConnection Open()
        {
            if(_mySqlConnection == null || _mySqlConnection.State != ConnectionState.Open)
            {
                _mySqlConnection = new MySqlConnection(ConnectionString);
            }
            if (_mySqlConnection.State != ConnectionState.Open)
            {
                _mySqlConnection.Open();
            }
            return _mySqlConnection;
        }

        public override void Close()
        {
           if(_mySqlConnection != null && _mySqlConnection.State != ConnectionState.Closed)
           {
               _mySqlConnection.Close();
           }
        }
    }
}