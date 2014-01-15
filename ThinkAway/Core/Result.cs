using System.Collections.Generic;
/*
 * Lsong
 * i@lsong.org
 * http://lsong.org
 */
namespace ThinkAway.Core
{
    /// <summary>
    /// 封装了一种用于返回数据的类型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Result<T>
    {
        public Result()
        {
            Code = 0;
            Desception = string.Empty;
            ResultObject = new List<T>();
        }

        private int _code;

        /// <summary>
        /// 返回码
        /// </summary>
        public int Code
        {
            get { return _code; }
            set { _code = value; }
        }

        private string _desception;

        /// <summary>
        /// 返回数据描述
        /// </summary>
        public string Desception
        {
            get { return _desception; }
            set { _desception = value; }
        }

        private List<T> _resultObject;

        /// <summary>
        /// 返回的数据对象集合
        /// </summary>
        public List<T> ResultObject
        {
            get { return _resultObject; }
            set { _resultObject = value; }
        }
    }
}
