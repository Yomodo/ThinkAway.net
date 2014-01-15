using System.Net;
using ThinkAway.Net.Http;
using ThinkAway.Text;

namespace ThinkAway.Plus.Oray
{
    public class OrayClient
    {
        private readonly WebHelper _httpClient;

        public OrayClient()
        {
            _httpClient = new WebHelper();
        }

        public System.Net.IPAddress CheckIP()
        {
            const string checkIpApi = "http://ddns.oray.com/checkip";
            string result = _httpClient.Get(checkIpApi);
            string[] strings = result.Split('<','>',':');
            string ip = strings[13].Trim();
            return IPAddress.Parse(ip);
        }

        public void Update(string user,string password,string host, string ipaddress)
        {
            const string updateApi = "http://{0}:{1}@ddns.oray.com/ph/update?hostname={2}&myip={3}";
            string url = string.Format(updateApi, Base64.Encode(System.Text.Encoding.UTF8.GetBytes(user)), Base64.Encode(System.Text.Encoding.UTF8.GetBytes(password)), host, ipaddress);
            string result = _httpClient.Get(url);

        }
    }
}
