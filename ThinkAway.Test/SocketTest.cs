using System;
using System.Globalization;
using System.IO;
using ThinkAway.Net.Sockets;

namespace ThinkAway.Test
{
    [Serializable]
    public class FileProtocol : ObjectProtocol
    {
        public string FileName { get; set; }

        public int Offset { get; set; }

        public new byte[] Data { get; set; }
    }

    class SocketTest : AppServer<FileProtocol>
    {
        public SocketTest(): base(8099)
        {
            FileStream fileStream = null;
            Connected += ((x, y) => { Console.WriteLine(y); });
            Disconnected += (x, y) => { Console.WriteLine(y); };
            Received += (x, y) =>
                            {
                                Console.WriteLine(y.Protocol.Data);
                                string key = x.GetHashCode().ToString(CultureInfo.InvariantCulture);
                                if(fileStream == null)
                                {
                                   fileStream =  new FileStream("", FileMode.CreateNew);
                                }
                            };
        }



        internal void SocketClientTest()
        {
            AppClient<FileProtocol> appClient = new AppClient<FileProtocol>();
            appClient.Connected += (sender, args) => 
            { 
                Console.WriteLine(args);
                FileProtocol protocol = new FileProtocol();
                protocol.FileName = "test.file";
                FileStream fileStream = new FileStream("c:\\aa.file",FileMode.Open);
                byte[] buffer = new byte[1024];
                int offset = 0;
                do
                {
                    offset = fileStream.Read(buffer, offset, buffer.Length);
                    protocol.Offset = offset;
                    protocol.Data = buffer;
                    appClient.Send(protocol);
                } while (offset != 0);
            };
            appClient.Received += (sender, args) => { Console.WriteLine(args); };
            appClient.Disconnected += (sender, args) => { Console.WriteLine(args);};
            ////appClient.Connect("192.168.0.36", 2001);
            appClient.Connect("127.0.0.1", 8099);
            
        }
    }
}
