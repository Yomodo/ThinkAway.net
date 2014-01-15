using System.Net;
using System.Net.Sockets;

namespace ThinkAway.Net
{
    public class DomainNameServer
    {

        internal static IPAddress FindIPv4Address(IPAddress[] iPAddress)
        {
            foreach (IPAddress ipAddress in iPAddress)
            {
                if(ipAddress.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ipAddress;
                }
            }
            return null;
        }
    }
}
