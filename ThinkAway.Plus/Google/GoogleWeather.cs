using System;
using System.Drawing;
using System.Xml;
using ThinkAway.Drawing;
using ThinkAway.Net.Http;

namespace ThinkAway.Plus.Google
{
    public class GoogleWeather
    {
        private readonly WebHelper webHelper;

        public class Weather
        {
            /// <summary>
            /// City infomation
            /// </summary>
            public class CityInfomaition
            {
                public String City;
                public String PostalCode;
                public String LatitudeE6;
                public String LongitudeE6;
                public String UnitSystem;
                public DateTime ForecastDate;
                public DateTime CurrentDateTime;

                public CityInfomaition(string city, string postalCode, string latitudeE6, string longitudeE6,
                                       string unitSystem, DateTime forecastDate, DateTime currentDateTime)
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
            /// <summary>
            /// 
            /// </summary>
            public class DayWeather
            {
                public Image Icon;
                public Int16 High;
                public Int16 Low;
                public String Condition;
                public String DayOfWeek;

                public DayWeather(string dayOfWeek, short high, short low, string condition, Image icon)
                {
                    this.Icon = icon;
                    this.High = high;
                    this.Low = low;
                    this.DayOfWeek = dayOfWeek;
                    this.Condition = condition;
                }
            }
            /// <summary>
            /// 
            /// </summary>
            public class TodayWeather
            {
                public Image Icon;
                public String Wind;
                public Int16 TempC;
                public Int16 TempF;
                public String Condition;
                public String Humidity;


                public TodayWeather(short tempC, short tempF, String condition, String humidity, String wind, Image icon)
                {
                    this.Icon = icon;
                    this.TempC = tempC;
                    this.TempF = tempF;
                    this.Wind = wind;
                    this.Humidity = humidity;
                    this.Condition = condition;
                }
            }
            /// <summary>
            /// CityInfomaition
            /// </summary>
            private readonly CityInfomaition _cityInfo;
            /// <summary>
            /// TodayWeather
            /// </summary>
            private readonly TodayWeather _today;
            /// <summary>
            /// DayWeather
            /// </summary>
            private readonly DayWeather[] _dayWeathers;
            /// <summary>
            /// Weather
            /// </summary>
            /// <param name="cityInfo"></param>
            /// <param name="today"></param>
            /// <param name="dayWeathers"></param>
            public Weather(CityInfomaition cityInfo, TodayWeather today, DayWeather[] dayWeathers)
            {
                _cityInfo = cityInfo;
                _today = today;
                _dayWeathers = dayWeathers;
            }
            /// <summary>
            /// CityInfo
            /// </summary>
            public CityInfomaition CityInfo
            {
                get { return _cityInfo; }
            }
            /// <summary>
            /// Today
            /// </summary>
            public TodayWeather Today
            {
                get { return _today; }
            }
            /// <summary>
            /// DayWeathers
            /// </summary>
            public DayWeather[] DayWeathers
            {
                get { return _dayWeathers; }
            }
        }

        public GoogleWeather()
        {
            webHelper = new WebHelper();
        }

        /// <summary>
        /// get weather with city
        /// </summary>
        /// <param name="city"></param>
        /// <returns></returns>
        public Weather GetWeather(string city)
        {
            const string baseUrl = @"https://www.google.com";
            UrlBuilder urlBuilder = new UrlBuilder(string.Format(@"{0}/ig/api", baseUrl));
            urlBuilder.Add("hl", "zh-cn");
            urlBuilder.Add("weather", city);

            string weatherXml = webHelper.Get(urlBuilder);
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(weatherXml);
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
                new ImageHelper(baseUrl + nodeToday.Item(0).SelectSingleNode("icon").Attributes["data"].InnerText).Image);

            XmlNodeList nodeList = xmlDocument.SelectNodes("xml_api_reply/weather/forecast_conditions");
            Weather.DayWeather[] dayWeathers = new Weather.DayWeather[nodeList.Count];
            for (int i = 0; i < nodeList.Count; i++)
            {
                string dayOfWeek = nodeList.Item(i).SelectSingleNode("day_of_week").Attributes["data"].InnerText;
                string height = nodeList.Item(i).SelectSingleNode("high").Attributes["data"].InnerText;
                string width = nodeList.Item(i).SelectSingleNode("low").Attributes["data"].InnerText;
                string condition = nodeList.Item(i).SelectSingleNode("condition").Attributes["data"].InnerText;
                string icon = nodeList.Item(i).SelectSingleNode("icon").Attributes["data"].InnerText;
                Weather.DayWeather dayWeather = new Weather.DayWeather(
                    dayOfWeek,
                    Convert.ToInt16(height),
                    Convert.ToInt16(width),
                    condition,
                    new ImageHelper(string.Concat(baseUrl, icon)).Image
                    );
                dayWeathers[i] = dayWeather;
            }
            Weather weather = new Weather(cityInfo, today, dayWeathers);
            return weather;
        }
    }
}
