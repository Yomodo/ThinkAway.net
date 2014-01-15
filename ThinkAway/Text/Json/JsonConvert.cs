using System;
using System.Collections.Specialized;

namespace ThinkAway.Text.Json
{
    public class JsonConvert
    {
        public static System.Collections.Specialized.NameValueCollection ToNameValue(string json)
        {
            NameValueCollection nameValue = new NameValueCollection();
            string resJson = json.Trim('{', '}');
            string[] strings = resJson.Split(',');
            foreach (string s in strings)
            {
                string[] str = s.Split(new[] { ":" }, 2, StringSplitOptions.None);
                string name = str[0].Trim(' ', '"');
                string value = str[1].Trim(' ', '"');
                nameValue.Add(name, value);
            }
            return nameValue;
        }
    }
}
