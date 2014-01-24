using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace ThinkAway.Net.Http
{
    public class WebServerRequest :WebRequest
    {
        public WebServerRequest(Socket handle) : base(handle)
        {
        }

        public override Stream GetRequestStream()
        {
            int pos;
            byte[] buffer = new byte[1024];
            ArrayList arrayList = new ArrayList();
            pos = SocketHandler.Receive(buffer, buffer.Length, SocketFlags.None);
            //while ((pos = socket.Receive(buffer, buffer.Length, SocketFlags.None)) != 0)
            //{
            arrayList.AddRange(buffer);
            byte[] data = (byte[])arrayList.ToArray(typeof(byte));
            //}
            Encoding encoding = Encoding.GetEncoding(Headers.Encoding);
            string str = encoding.GetString(data);
            if (!String.IsNullOrEmpty(str))
            {
                StringReader stringReader = new StringReader(str.Trim('\0'));
                string readLine = stringReader.ReadLine();
                if (readLine != null)
                {
                    string[] strings = readLine.Split(' ');

                    Headers.RequestUrl = strings[1];
                    Headers.HttpVersion = strings[2];

                    while (stringReader.Peek() != -1)
                    {
                        string line = stringReader.ReadLine();

                        if (!string.IsNullOrEmpty(line))
                        {
                            string[] split = line.Split(':');
                            Headers.Add(split[0], split[1]);
                        }
                    }
                }
            }
            return new MemoryStream(data, false);
        }
    }
}
