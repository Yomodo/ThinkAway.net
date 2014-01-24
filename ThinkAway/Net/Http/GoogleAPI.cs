using System;
using System.Xml;
using ThinkAway.Drawing;

namespace ThinkAway.Net.Http
{
    class GoogleAPI
    {
        internal class Weather
        {
            internal class CityInfomaition
            {
                public String City;
                public String PostalCode;
                public String LatitudeE6;
                public String LongitudeE6;
                public String UnitSystem;
                public DateTime ForecastDate;
                public DateTime CurrentDateTime;

                public CityInfomaition(string city, string postalCode, string latitudeE6, string longitudeE6, string unitSystem, DateTime forecastDate, DateTime currentDateTime)
                {
                    City = city;
                    PostalCode = postalCode;
                    LatitudeE6 = latitudeE6;
                    LongitudeE6 = longitudeE6;
                    UnitSystem = unitSystem;
                    ForecastDate = forecastDate;
                    CurrentDateTime = currentDateTime;
                }
            }
            internal class DayWeather
            {
                public System.Drawing.Image Icon;
                public Int16 High;
                public Int16 Low;
                public String Condition;

                public DayWeather(short high, short low, string condition, System.Drawing.Image icon)
                {
                    Icon = icon;
                    this.High = high;
                    this.Low = low;
                    this.Condition = condition;
                }
            }
            internal class TodayWeather
            {
                public System.Drawing.Image Icon;
                public String Wind;
                public Int16 TempC;
                public Int16 TempF;
                public String Condition;
                public String Humidity;


                public TodayWeather(short tempC, short tempF, String condition, String humidity, String wind, System.Drawing.Image icon)
                {
                    this.Icon = icon;
                    this.TempC = tempC;
                    this.TempF = tempF;
                    this.Wind = wind;
                    this.Humidity = humidity;
                    this.Condition = condition;
                }
            }

            private readonly CityInfomaition _cityInfo;
            private readonly TodayWeather _today;
            private readonly DayWeather _tomorrow;
            private readonly DayWeather _third;
            private readonly DayWeather _fourth;

            public Weather(CityInfomaition cityInfo, TodayWeather today, DayWeather tomorrow, DayWeather third, DayWeather fourth)
            {
                _cityInfo = cityInfo;
                _today = today;
                _fourth = fourth;
                _third = third;
                _tomorrow = tomorrow;
            }
            public CityInfomaition CityInfo
            {
                get { return _cityInfo; }
            }
            public TodayWeather Today
            {
                get { return _today; }
            }
            public DayWeather Tomorrow
            {
                get { return _tomorrow; }
            }
            public DayWeather Third
            {
                get { return _third; }
            }
            public DayWeather Fourth
            {
                get { return _fourth; }
            }
        }

        public static string GoogleTranslate(string sourceWord, string fromLanguage, string toLanguage)
        {
            /* 
             调用： http://ajax.googleapis.com/ajax/services/language/translate?v=1.0&langpair=zh-CN|en&q=中国
             返回的json格式如下：
             {"responseData": {"translatedText":"Chinese people are good people"}, "responseDetails": null, "responseStatus": 200}*/
            string serverUrl = string.Format(@"http://ajax.googleapis.com/ajax/services/language/translate?v=1.0&langpair={0}|{1}&q={2}", fromLanguage, toLanguage, sourceWord);

            string resJson = new WebHelper().Get(serverUrl);
            int textIndex = resJson.IndexOf("translatedText") + 17;
            int textLen = resJson.IndexOf("\"", textIndex) - textIndex;
            return resJson.Substring(textIndex, textLen);
        }
        public static Weather GoogleWeather(string city)
        {
            const string baseUrl = @"https://www.google.com";
            WebHelper connectionBase = new WebHelper();
            UrlBuilder parameters = new UrlBuilder(string.Format(@"{0}/ig/api", baseUrl));
            parameters.Add("hl","zh-cn");
            parameters.Add("weather",city);
            XmlDocument xmlDocument = new XmlDocument();
           xmlDocument.LoadXml(connectionBase.Get(parameters.ToString()));
            XmlNodeList nodeCity = xmlDocument.SelectNodes("xml_api_reply/weather/forecast_information");
            Weather.CityInfomaition cityInfo = new Weather.CityInfomaition(
                nodeCity.Item(0).SelectSingleNode("city").Attributes["data"].InnerText,
                nodeCity.Item(0).SelectSingleNode("postal_code").Attributes["data"].InnerText,
                nodeCity.Item(0).SelectSingleNode("latitude_e6").Attributes["data"].InnerText,
                nodeCity.Item(0).SelectSingleNode("longitude_e6").Attributes["data"].InnerText,
                nodeCity.Item(0).SelectSingleNode("unit_system").Attributes["data"].InnerText,
                Convert.ToDateTime(nodeCity.Item(0).SelectSingleNode("forecast_date").Attributes["data"].InnerText),
                Convert.ToDateTime(nodeCity.Item(0).SelectSingleNode("current_date_time").Attributes["data"].InnerText));
            XmlNodeList nodeToday = xmlDocument.SelectNodes("xml_api_reply/weather/current_conditions");
            Weather.TodayWeather today = new Weather.TodayWeather(
                Convert.ToInt16(nodeToday.Item(0).SelectSingleNode("temp_c").Attributes["data"].InnerText),
                Convert.ToInt16(nodeToday.Item(0).SelectSingleNode("temp_f").Attributes["data"].InnerText),
                nodeToday.Item(0).SelectSingleNode("condition").Attributes["data"].InnerText,
                nodeToday.Item(0).SelectSingleNode("humidity").Attributes["data"].InnerText,
                nodeToday.Item(0).SelectSingleNode("wind_condition").Attributes["data"].InnerText,
                ImageHelper.GetImage(baseUrl + nodeToday.Item(0).SelectSingleNode("icon").Attributes["data"].InnerText));
            XmlNodeList nodeList = xmlDocument.SelectNodes("xml_api_reply/weather/forecast_conditions");
            Weather.DayWeather tomorrow = new Weather.DayWeather(
                Convert.ToInt16(nodeList.Item(1).SelectSingleNode("high").Attributes["data"].InnerText),
                Convert.ToInt16(nodeList.Item(1).SelectSingleNode("low").Attributes["data"].InnerText),
                nodeList.Item(1).SelectSingleNode("condition").Attributes["data"].InnerText,
                ImageHelper.GetImage(baseUrl + nodeToday.Item(0).SelectSingleNode("icon").Attributes["data"].InnerText));
            Weather.DayWeather third = new Weather.DayWeather(
                Convert.ToInt16(nodeList.Item(2).SelectSingleNode("high").Attributes["data"].InnerText),
                Convert.ToInt16(nodeList.Item(2).SelectSingleNode("low").Attributes["data"].InnerText),
                nodeList.Item(2).SelectSingleNode("condition").Attributes["data"].InnerText,
                ImageHelper.GetImage(baseUrl + nodeToday.Item(0).SelectSingleNode("icon").Attributes["data"].InnerText));
            Weather.DayWeather fourth = new Weather.DayWeather(
                Convert.ToInt16(nodeList.Item(3).SelectSingleNode("high").Attributes["data"].InnerText),
                Convert.ToInt16(nodeList.Item(3).SelectSingleNode("low").Attributes["data"].InnerText),
                nodeList.Item(3).SelectSingleNode("condition").Attributes["data"].InnerText,
                ImageHelper.GetImage(baseUrl + nodeToday.Item(0).SelectSingleNode("icon").Attributes["data"].InnerText));
            Weather weather = new Weather(cityInfo,today, tomorrow, third, fourth);
            return weather;
        }
    }
}
