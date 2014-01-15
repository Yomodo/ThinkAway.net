using System;
/*
 * Lsong
 * i@lsong.org
 * http://lsong.org
 */
namespace ThinkAway.Data
{
    public class DbException : Exception
    {
        private string _sql;

        public string Sql
        {
            get { return _sql; }
            set { _sql = value; }
        }

        public DbException(string message):base(message)
        {
            
        }

        public DbException(string message,Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
