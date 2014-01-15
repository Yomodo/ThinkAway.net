using System;
using System.Net;
using ThinkAway.Plus.Oray;

namespace ThinkAway.Test
{
    class OrayTest
    {
        public OrayTest()
        {
            OrayClient orayClient = new OrayClient();
            IPAddress ipAddress =  orayClient.CheckIP();
            orayClient.Update("song940","lsong940","lsong.oicp.net",ipAddress.ToString());
            Console.WriteLine(ipAddress);
        }
    }
}
