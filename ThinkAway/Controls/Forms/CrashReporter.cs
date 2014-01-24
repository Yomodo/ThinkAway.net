using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ThinkAway.Core;

namespace ThinkAway.Controls.Forms
{
    public partial class CrashReporter : Form
    {
        private readonly MemoryStream memoryStream = new MemoryStream();

        public CrashReporter(Exception exception)
        {
            InitializeComponent();

            GenerateDialogReport(exception);
        }

        private void BugReportLoad(object sender, EventArgs e)
        {
            ChkHelperCheckedChanged(sender,e);
            this.Top += 50;
        }

        private void ChkHelperCheckedChanged(object sender, EventArgs e)
        {
            if (chkHelper.Checked)
            {
                while (this.Height != this.MaximumSize.Height)
                {
                    this.Height++;
                    if(this.Height % 2== 0)
                    {
                        this.Top--;
                    }
                }
            }
            else
            { 
            while(this.Height != this.MinimumSize.Height)
            {
                 this.Height--;
                 if (this.Height % 2 == 0)
                 {
                     this.Top++;
                 }
            }
            }
                
        }


        /// <summary>
        /// Fills in text box with exception info
        /// </summary>
        /// <param name="e"></param>
        private void GenerateDialogReport(Exception e)
        {
            StringBuilder sb = new StringBuilder();

            // Set picturebox to error
            this.pictureBoxErr.Image = SystemIcons.Error.ToBitmap();

            Process proc = Process.GetCurrentProcess();

            // dates and time
            sb.AppendLine(string.Format("Current Date/Time: {0}", DateTime.Now.ToString()));
            sb.AppendLine(string.Format("Exec. Date/Time: {0}", proc.StartTime.ToString()));
            sb.AppendLine(string.Format("Build Date: {0}", System.DateTime.Now));
            // os info
            sb.AppendLine(string.Format("OS: {0}", Environment.OSVersion.VersionString));
            sb.AppendLine(string.Format("Language: {0}", Application.CurrentCulture));
            // uptime stats
            sb.AppendLine(string.Format("System Uptime: {0} Days {1} Hours {2} Mins {3} Secs", Math.Round((decimal)Environment.TickCount / 86400000), Math.Round((decimal)Environment.TickCount / 3600000 % 24), Math.Round((decimal)Environment.TickCount / 120000 % 60), Math.Round((decimal)Environment.TickCount / 1000 % 60)));
            sb.AppendLine(string.Format("Program Uptime: {0}", proc.TotalProcessorTime.ToString()));
            // process id
            sb.AppendLine(string.Format("PID: {0}", proc.Id));
            // exe name
            sb.AppendLine(string.Format("Executable: {0}", Application.ExecutablePath));
            sb.AppendLine(string.Format("Process Name: {0}", proc));
            sb.AppendLine(string.Format("Main Module Name: {0}", proc.MainModule.ModuleName));
            // exe stats
            sb.AppendLine(string.Format("Module Count: {0}", proc.Modules.Count));
            sb.AppendLine(string.Format("Thread Count: {0}", proc.Threads.Count));
            sb.AppendLine(string.Format("Thread ID: {0}", Thread.CurrentThread.ManagedThreadId));
            sb.AppendLine(string.Format("Is Admin: {0}", Permissions.IsUserAdministrator));
            sb.AppendLine(string.Format("Is Debugged: {0}", Debugger.IsAttached));
            // versions
            sb.AppendLine(string.Format("Version: {0}", Application.ProductVersion));
            sb.AppendLine(string.Format("CLR Version: {0}", Environment.Version));


            Exception ex = e;
            for (int i = 0; ex != null; ex = ex.InnerException, i++)
            {
                sb.AppendLine();
                sb.AppendLine(string.Format("Type #{0} {1}", i, ex.GetType()));

                foreach (System.Reflection.PropertyInfo propInfo in ex.GetType().GetProperties())
                {
                    string fieldName = string.Format("{0} #{1}", propInfo.Name, i);
                    string fieldValue = string.Format("{0}", propInfo.GetValue(ex, null));

                    // Ignore stack trace + data
                    if (propInfo.Name == "StackTrace"
                        || propInfo.Name == "Data"
                        || string.IsNullOrEmpty(propInfo.Name)
                        || string.IsNullOrEmpty(fieldValue))
                        continue;

                    sb.AppendLine(string.Format("{0}: {1}", fieldName, fieldValue));
                }
                foreach (DictionaryEntry de in ex.Data)
                    sb.AppendLine(string.Format("Dictionary Entry #{0}: Key: {1} Value: {2}", i, de.Key, de.Value));
            }

            sb.AppendLine();
            sb.AppendLine("StackTrace:");
            sb.AppendLine(e.StackTrace);

            this.richTextBox1.Text = sb.ToString();

            byte[] b = Encoding.ASCII.GetBytes(sb.ToString());
            memoryStream.Write(b, 0, b.Length);
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            this.SendBugReport();

            this.Close();
        }

        /// <summary>
        /// Uses PHP to upload bug report and email it
        /// </summary>
        /// <returns>True if it was sent</returns>
        private bool SendBugReport()
        {
            const string fileFormName = "uploadedfile";
            const string contenttype = "application/octet-stream";
            const string fileName = "bugreport.txt";

            string boundary = string.Format("----------{0}", DateTime.Now.Ticks.ToString("x"));
            HttpWebRequest webrequest = (HttpWebRequest)HttpWebRequest.Create("http://lsong.org/crashreporter.php");
            webrequest.CookieContainer = new CookieContainer();
            webrequest.ContentType = "multipart/form-data; boundary=" + boundary;
            webrequest.Method = "POST";

            // Build up the post message header

            StringBuilder sb = new StringBuilder();
            sb.Append("--");
            sb.Append(boundary);
            sb.Append("\r\n");
            sb.Append("Content-Disposition: form-data; name=\"");
            sb.Append(fileFormName);
            sb.Append("\"; filename=\"");
            sb.Append(fileName);
            sb.Append("\"");
            sb.Append("\r\n");
            sb.Append("Content-Type: ");
            sb.Append(contenttype);
            sb.Append("\r\n");
            sb.Append("\r\n");

            string postHeader = sb.ToString();
            byte[] postHeaderBytes = Encoding.UTF8.GetBytes(postHeader);

            // Build the trailing boundary string as a byte array
            // ensuring the boundary appears on a line by itself

            byte[] boundaryBytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            //FileStream fileStream = new FileStream(uploadfile, FileMode.Open, FileAccess.Read);
            long length = postHeaderBytes.Length + this.memoryStream.Length + boundaryBytes.Length;
            webrequest.ContentLength = length;

            try
            {
                Stream requestStream = webrequest.GetRequestStream();

                // Write out our post header
                requestStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);

                // Write out the file contents
                byte[] buffer = this.memoryStream.ToArray();
                requestStream.Write(buffer, 0, buffer.Length);

                requestStream.Write(boundaryBytes, 0, boundaryBytes.Length);
            }
            catch (WebException e)
            {
                if (System.Windows.Forms.MessageBox.Show(this, e.Message, @"Error", MessageBoxButtons.RetryCancel) == DialogResult.Retry)
                    return SendBugReport();
                else
                    return false;
            }

            // Write out the trailing boundary
            HttpWebResponse response = (HttpWebResponse)webrequest.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                System.Windows.Forms.MessageBox.Show(this, @"Sent bug report successfully", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            System.Windows.Forms.MessageBox.Show(this, @"The bug report could not be sent", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }

        private void ErrorDlg_FormClosed(object sender, FormClosedEventArgs e)
        {
            //if (this.checkBoxRestart.Checked)
            //{
                Application.Restart();
                Process.GetCurrentProcess().Kill();
            //}
        }

        private void buttonDontSend_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
