using System;
using System.IO;
using System.Net.Sockets;
using System.Windows.Forms;
using NUnit.Framework;
using ThinkAway.IO;
using ThinkAway.Net.Http;
using ThinkAway.Plus.Update;

namespace ThinkAway.Test
{
    [TestFixture]
    class MainClass : WebServerHandler
    {
        [STAThread]
        public static void Main(string[] parameters)
        {
            MainClass mainClass = new MainClass();
            mainClass.Run();
        }

        private void Run()
        {
            WebServer webServer = new WebServer(8080,this);  
            webServer.StartListener();

            Console.ReadKey();
        }



    }
}
