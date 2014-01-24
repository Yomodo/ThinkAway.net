using System;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Threading;
/*
 * Lsong
 * i@lsong.org
 * http://lsong.org
 */
namespace ThinkAway.Data.SQLite
{
    /// <summary>
    /// SQLite 的摘要说明。
    /// </summary>
    public class SQLite : DbHelper
    {
        /// <summary>
        /// _lock
        /// </summary>
        private bool _lock;

        /// <summary>
        /// SQLiteConnection
        /// </summary>
        private SQLiteConnection _sqLiteConnection;

        /// <summary>
        /// lock
        /// </summary>
        private readonly object _objectRoot =  new object();

        /// <summary>
        /// 写数据 信号量
        /// </summary>
        private readonly ManualResetEvent _writeManualResetEvent;

        /// <summary>
        /// 根据当前应用程序的默认配置名称获取连接字符串创建数据访问实例
        /// </summary>
        /// <param name="connectionName"></param>
        public SQLite(string connectionName) : base(connectionName)
        {
        }

        /// <summary>
        /// SQLite
        /// </summary>
        /// <param name="source"></param>
        /// <param name="pooling"></param>
        /// <param name="failIfMissing"></param>
        public SQLite(string source, bool pooling, bool failIfMissing)
            : base(string.Empty)
        {
            if (_writeManualResetEvent == null)
            {
                _writeManualResetEvent = new ManualResetEvent(false);
            }
            ConnectionString = string.Format("Data Source={0};Pooling={1};FailIfMissing={2}", source, pooling, failIfMissing);
        }

        /// <summary>
        /// get lock.
        /// </summary>
        public void GetLock()
        {
            //防止抢占 lock 资源
            lock (_objectRoot)
            {
                if(_lock)
                {
                    _writeManualResetEvent.WaitOne();
                    _lock = false;
                }
                else
                {
                    _writeManualResetEvent.Reset();
                    _lock = true;
                }
            }
            
        }
        /// <summary>
        /// free lock.
        /// </summary>
        public void FreeLock()
        {
            //lock (_objectRoot)
            //{
                if (_lock)
                {
                    _lock = false;
                    _writeManualResetEvent.Set();
                }
            //}
        }

        /// <summary>
        /// 关闭数据库联接
        /// </summary>
        public override void Close()
        {
            if (_sqLiteConnection != null && _sqLiteConnection.State != ConnectionState.Closed)
            {
                _sqLiteConnection.Close();
            }
        }

        public override Cursor Query(string sql)
        {
            SQLiteConnection sqLiteConnection = (SQLiteConnection) Open();
            SQLiteCommand sqLiteCommand = new SQLiteCommand(sql, sqLiteConnection);
            SQLiteDataReader sqLiteDataReader = sqLiteCommand.ExecuteReader(CommandBehavior.CloseConnection);
            Cursor cursor = new Cursor(sqLiteDataReader);
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
            SQLiteConnection sqLiteConnection = (SQLiteConnection)Open();
            SQLiteCommand sqLiteCommand = new SQLiteCommand(sql, sqLiteConnection);
            SQLiteDataAdapter sqLiteDataAdapter = new SQLiteDataAdapter(sqLiteCommand);
            sqLiteDataAdapter.Fill(dataSet);
            return dataSet;
        }

        public override DbConnection Open()
        {
            if (_sqLiteConnection == null)
            {
                _sqLiteConnection = new SQLiteConnection(ConnectionString);
            }
            if (_sqLiteConnection.State != ConnectionState.Open)
            {
                _sqLiteConnection.Open();
            }
            return _sqLiteConnection;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public override int ExecSql(string sql)
        {
            //get lock.
            GetLock();

            SQLiteConnection sqLiteConnection = (SQLiteConnection) Open();
            SQLiteCommand sqLiteCommand = new SQLiteCommand(sql, sqLiteConnection);
            int result = sqLiteCommand.ExecuteNonQuery();

            //free lock.
            FreeLock();

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public bool Create(string tableName)
        {
            throw new NotImplementedException();
        }
    }
}
