namespace ThinkAway.Net.Http
{
    /// <summary>
    /// UrlParameter
    /// </summary>
    public class UrlParameter
    {

        public UrlParameter(string key, string value)
        {
            // TODO: Complete member initialization
            this.Key = key;
            this.Value = value;
        }

        private string _value;

        /// <summary>
        /// Value
        /// </summary>
        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        private string _key;

        /// <summary>
        /// Key
        /// </summary>
        public string Key
        {
            get { return _key; }
            set { _key = value; }
        }


        public override string ToString()
        {
            return string.Format("{0}={1}", Key, Value);
        }
    }
}
