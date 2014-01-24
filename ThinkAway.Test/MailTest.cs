using NUnit.Framework;
using ThinkAway.Net.Mail;
using ThinkAway.Net.Mail.POP3;
using ThinkAway.Net.Mail.POP3.Header;
using ThinkAway.Net.Mail.SMTP;
using MailMessage = ThinkAway.Net.Mail.SMTP.MailMessage;

namespace ThinkAway.Test
{
    class MailTest
    {

        [Test]
        public void SmtpTest()
        {

            using (SmtpClient smtpClient = new SmtpClient("smtp.163.com"))
            {

                smtpClient.Connect();

                smtpClient.UserName = "song940@163.com";
                smtpClient.Password = "song940@163.com";

                smtpClient.Authenticate("song940@163.com", "lsong940");

                MailAddress from = new MailAddress("Lsong", "song940@163.com");
                MailAddress to = new MailAddress("admin@lsong.org");
                MailAddress cc = new MailAddress("Test<cc@lsong.org>");

                MailMessage mailMessage = new MailMessage(from, to);
                mailMessage.To.Add("test1@lsong.org");
                mailMessage.To.Add("test2@lsong.org");
                mailMessage.To.Add("test3@lsong.org");
                mailMessage.To.Add("test4@lsong.org");

                mailMessage.AddRecipient(cc, AddressType.Cc);
                mailMessage.AddRecipient("test@lsong.org", AddressType.Bcc);

                mailMessage.Charset = "UTF-8";
                mailMessage.Priority = MailPriority.High;
                mailMessage.Notification = true;

                mailMessage.AddCustomHeader("X-CustomHeader", "Value");
                mailMessage.AddCustomHeader("X-CompanyName", "Value");

                //string testCid = mailMessage.AddImage("C:\\test.bmp");

                //mailMessage.AddAttachment("C:\\test.zip");

                mailMessage.Subject = "This's a test Mail.";
                mailMessage.Body = "hello everybody .";
                mailMessage.HtmlBody =
                    string.Format("<html><body>hello everybody .<br /><img src='cid:{0}' /></body></html>", "");

                smtpClient.SendMail(mailMessage);
            }
        }
        public void PopTest()
        {

            using (PopClient popClient = new PopClient("pop.lsong.org"))
            {
                popClient.Connect();
                popClient.Authenticate("admin@lsong.org", "lsong940");

                int messageCount = popClient.GetMessageCount();
                for (int i = messageCount; i > 0; i--)
                {
                    MessageHeader messageHeaders = popClient.GetMessageHeaders(i);
                }
            }
        }

    }
}
