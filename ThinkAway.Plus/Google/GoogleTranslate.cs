using System.Collections.Specialized;
using ThinkAway.Net.Http;
using ThinkAway.Text.Json;

namespace ThinkAway.Plus.Google
{
    public class GoogleTranslate
    {
        private readonly WebHelper webHelper;

        public GoogleTranslate()
        {
            webHelper = new WebHelper();
        }

        public string TranslateV1(string sourceWord, string fromLanguage, string toLanguage)
        {
            /* 
             调用： http://ajax.googleapis.com/ajax/services/language/translate?v=1.0&langpair=zh-CN|en&q=中国
             返回的json格式如下：
             {"responseData": {"translatedText":"Chinese people are good people"}, "responseDetails": null, "responseStatus": 200}*/
            const string api = "http://ajax.googleapis.com/ajax/services/language/translate";
            UrlBuilder urlBuilder = new UrlBuilder(api);
            urlBuilder.Add("v", "1.0");
            urlBuilder.Add("langpair", string.Format("{0}|{1}", fromLanguage, toLanguage));
            urlBuilder.Add("q",sourceWord);
            
            string resJson = webHelper.Get(urlBuilder);
            NameValueCollection nameValue = JsonConvert.ToNameValue(resJson);
            return nameValue["responseData"];
        }
        public string TranslateV2(string word, string target)
        {
            const string api = "https://www.googleapis.com/language/translate/v2/languages";
            UrlBuilder urlBuilder = new UrlBuilder(api);
            urlBuilder.Add("key","11111");
            urlBuilder.Add("target","en");
            urlBuilder.Add("q", word);
            string resJson = webHelper.Get(urlBuilder);
            NameValueCollection nameValue = JsonConvert.ToNameValue(resJson);
            return nameValue["responseData"];
        }
        public string Translate(string sourceWord, string fromLanguage, string toLanguage)
        {
            return TranslateV1(sourceWord, fromLanguage, toLanguage);
        }
    }
}
