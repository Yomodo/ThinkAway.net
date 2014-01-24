using System;
using NUnit.Framework;
using ThinkAway.Net.FTP;

namespace ThinkAway.Test
{
    class FtpTest
    {
        [Test]
        public void Test()
        {
            using (FTPClient ftpClient = new FTPClient("127.0.0.1"))
            {
                ftpClient.Mode = FtpMode.Passive;
                ftpClient.Connect("127.0.0.1");
                ftpClient.Login("admin", "123");
                ftpClient.SendCommand("SYST", true);
                //ftpClient.SendCommand("FEAT",true);
                //ftpClient.SendCommand("CLNT 1.0.0.0",true);
                //ftpClient.SendCommand("OPTS UTF8 ON",true);
                ftpClient.SendCommand("PWD", true);
                //ftpClient.SetCurrentDirectory("/myphp");
                string[] strings = ftpClient.Dir("");
                foreach (string s in strings)
                {
                    ftpClient.GetFile(s, "D:\\" + s);
                }
                //ftpClient.SendFile(@"C:\test.zip", "aa.zip", FtpType.Binary);
                //ftpClient.GetFile("aa.zip", @"D:\test.zip", FtpType.Binary);
                //ftpClient.MoveFile("aa.zip", "test");
            }

        }



        static void OnSecurityNotAvailable(FtpSecurityNotAvailable e)
        {
            // SSL/TLS could not be negotiated with the AUTH command.
            // If you do not want login credentials to be sent in plain
            // text set the e.Cancel property true to cancel the login.
            // Doing so with trigger a FtpCommandException to be thrown
            // for the failed AUTH command.
            e.Cancel = false;
        }

        public void Download()
        {
            using (FtpClient cl = new FtpClient("ftp.netbsd.org", 21))
            {

                cl.Username = "ftp";
                cl.Password = "ftp";
                /////
                // Logging the transaction
                /////
                // log the ftp transactions to stdout
                cl.FtpLogStream = Console.OpenStandardOutput();
                // flush the output buffer so that the transaction
                // stays in sync with out output going to screen
                cl.FtpLogFlushOnWrite = true;

                //////
                // SSL/TLS configuration
                // If you're going to use encryption you should handle the 2 events
                // below according to your needs. It's not uncommon for a server to
                // have a self signed certificate or name mismatch so at the very least
                // you should handle the InvalidCertificate event.
                //////
                // default, use AUTH command to setup encryption
                cl.SslMode = FtpSslMode.Explicit;
                // If you do not handle this event and the AUTH command fails the
                // login credentials will be sent in plain text!!!! See the event args
                // for this event handler.
                cl.SecurityNotAvailable += new SecurityNotAvailable(OnSecurityNotAvailable);
                cl.InvalidCertificate += new FtpInvalidCertificate(OnInvalidCertficate);

                try
                {
                    //////
                    // The actual download
                    //////
                    cl.TransferProgress += new FtpTransferProgress(OnTransferProgress);
                    cl.Download("/pub/pkgsrc/current/pkgsrc.tar.gz");
                }
                catch (Exception ex)
                {
                    Console.WriteLine();
                    Console.WriteLine(ex.ToString());
                }
            }

            Console.WriteLine("-- OPERATION COMPLETED, PRESS ANY KEY TO CLOSE --");
            Console.ReadKey();
        }


        public void Upload()
        {
            using (FtpClient cl = new FtpClient("ftp", "ftp", "server"))
            {
                /////
                // Logging the transaction
                /////
                // log the ftp transactions to stdout
                cl.FtpLogStream = Console.OpenStandardOutput();
                // flush the output buffer so that the transaction
                // stays in sync with out output going to screen
                cl.FtpLogFlushOnWrite = true;

                //////
                // SSL/TLS configuration
                // If you're going to use encryption you should handle the 2 events
                // below according to your needs. It's not uncommon for a server to
                // have a self signed certificate or name mismatch so at the very least
                // you should handle the InvalidCertificate event.
                //////
                // default, use AUTH command to setup encryption
                cl.SslMode = FtpSslMode.Explicit;
                // If you do not handle this event and the AUTH command fails the
                // login credentials will be sent in plain text!!!! See the event args
                // for this event handler.
                cl.SecurityNotAvailable += new SecurityNotAvailable(OnSecurityNotAvailable);
                cl.InvalidCertificate += new FtpInvalidCertificate(OnInvalidCertficate);

                try
                {
                    //////
                    // The actual download
                    //////
                    cl.TransferProgress += new FtpTransferProgress(OnTransferProgress);
                    cl.Upload("localfile.ext");
                }
                catch (Exception ex)
                {
                    Console.WriteLine();
                    Console.WriteLine(ex.Message);
                }
            }

            Console.WriteLine("-- OPERATION COMPLETED, PRESS ANY KEY TO CLOSE --");
            Console.ReadKey();
        }


        void OnInvalidCertficate(FtpChannel c, InvalidCertificateInfo e)
        {
            // we don't care if a certificate is invalid
            e.Ignore = true;
        }

        void OnTransferProgress(FtpTransferInfo e)
        {
            Console.Write("\r{0}/{1} {2}% {3}/s       ",
                e.Transferred, e.Length, e.Percentage, e.BytesPerSecond);

            if (e.Complete)
            {
                Console.WriteLine();
            }
        }
    }
}
