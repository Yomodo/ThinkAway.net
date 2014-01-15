using System;
using System.Text;

namespace ThinkAway.Text.SQL
{
    public class SqlBase
    {
        private readonly StringBuilder _stringBuilder;

        protected SqlBase()
        {
            _stringBuilder = new StringBuilder();
        }

        public SqlBase Select(params string[] colum)
        {
            _stringBuilder.AppendFormat("select {0}",colum);
            return this;
        }

        public SqlBase From(string tableName)
        {
            _stringBuilder.AppendFormat("from {0}",tableName);
            return this;
        }

        public SqlBase Where(string colum, string value)
        {
            throw new NotImplementedException();
        }

        public SqlBase Order(string p, int p_2)
        {
            throw new NotImplementedException();
        }

        public SqlBase Limit(int p, int p_2)
        {
            throw new NotImplementedException();
        }

        public SqlBase Delete(string p)
        {
            throw new NotImplementedException();
        }
    }
}
