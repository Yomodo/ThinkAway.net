using System;
using System.Data.Common;
/*
 * Lsong
 * i@lsong.org
 * http://lsong.org
 */
namespace ThinkAway.Data
{
    /// <summary>
    /// 数据游标指针
    /// <summary>提供一种数据流读取的只进流的方式,无法继承此类</summary>
    ///  </summary>
    public sealed class Cursor : IDisposable
    {
        /// <summary>
        /// 从数据源读取行的一个只进流
        /// </summary>
        private readonly DbDataReader _dbDataReader;

        /// <summary>
        /// 根据指定的 DbDataReader 创建数据游标
        /// </summary>
        /// <param name="dbDataReader">sqlDataReader</param>
        public Cursor(DbDataReader dbDataReader)
        {
            _dbDataReader = dbDataReader;
        }

        /// <summary>
        /// 前进游标指针
        /// <summary>将数据游标前进到数据集的下一个记录</summary>
        /// </summary>
        /// <returns>是否存在下一记录</returns>
        public bool Next()
        {
            return _dbDataReader.Read();
        }

        /// <summary>
        /// 获取 数据集中指定列中数据的 <c>String</c> 类型实例
        /// </summary>
        /// <param name="columnName">columnName</param>
        /// <returns></returns>
        public string GetString(string columnName)
        {
            object obj = this[columnName];
            string value = Convert.ToString(obj);
            return value;
        }
        /// <summary>
        /// 获取 数据集中指定列中数据的 <c>Int32</c> 类型实例
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public int GetInt(string columnName)
        {
            object obj = this[columnName];
            int value = Convert.ToInt32(obj);
            return value;
        }

        public double GetDouble(string columnName)
        {
            object obj = this[columnName];
            double value = Convert.ToDouble(obj);
            return value;
        }
        /// <summary>
        /// 获取 数据集中指定列中数据的 <c>Boolean</c> 类型实例
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public bool GetBoolean(string columnName)
        {
            object obj = this[columnName];
            bool value = Convert.ToBoolean(obj);
            return value;
        }
        /// <summary>
        /// 获取 数据集中指定列中数据的 <c>DateTime</c> 类型实例
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public DateTime GetDataTime(string columnName)
        {
            object obj = this[columnName];
            DateTime value = Convert.ToDateTime(obj);
            return value;
        }

        /// <summary>
        /// 获取指定的 <c>columnName</c> 获取数据集中列中数据的对象实例
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public object this[string columnName]
        {
            get { return _dbDataReader[columnName]; }
        }

        /// <summary>
        /// 获取指定的 <c>index</c> 获取数据集中列中数据的对象实例
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public object this[int index]
        {
            get { return _dbDataReader[index]; }
        }

        /// <summary>
        /// 关闭 Cursor 对象实例
        /// </summary>
        public void Close()
        {
            _dbDataReader.Close();
        }

        public void Dispose()
        {
            this.Close();
            GC.SuppressFinalize(this);
        }

        ~Cursor()
        {
            _dbDataReader.Dispose();
        }
    }
}
