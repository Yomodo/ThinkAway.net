using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Serialization;
using ThinkAway.Net.Http;

namespace ThinkAway.Plus.Update
{
    public class Updater
    {
        

        public string ProductName { get; set; }
        
        public Version ProductVersion { get; set; }

        public event EventHandler<UpdateErrorArgs> Error;

        protected void OnError(UpdateErrorArgs e)
        {
            EventHandler<UpdateErrorArgs> handler = Error;
            if (handler != null) handler(this, e);
        }

        public event DownloadProgressChangedEventHandler ProgressChanged;

        protected virtual void OnProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            DownloadProgressChangedEventHandler handler = ProgressChanged;
            if (handler != null) handler(sender, e);
        }

        public event AsyncCompletedEventHandler Completed;

        protected virtual void OnCompleted(object sender, AsyncCompletedEventArgs e)
        {
            AsyncCompletedEventHandler handler = Completed;
            if (handler != null) handler(sender, e);
        }

        public event EventHandler<NewVersionEventArgs> NewVersion;

        protected void OnNewVersion(NewVersionEventArgs e)
        {
            EventHandler<NewVersionEventArgs> handler = NewVersion;
            if (handler != null) handler(this, e);
        }

        private HttpClient _httpClient;

        private readonly System.Net.WebClient webClient;

        public UpdateConfig Config { get; set; }


        public Updater()
        {
            Config = new UpdateConfig();
            Config.UpdateUrl = string.Format("http://lsong.org/products/update.php");

            webClient = new System.Net.WebClient();
            webClient.DownloadProgressChanged += this.OnProgressChanged;
            webClient.DownloadFileCompleted += this.OnCompleted;

        }

        public Updater(Control ctrl) : this(ctrl.ProductName,ctrl.ProductVersion)
        {
        }

        public Updater(string productName, string productVersion):this()
        {
            this.ProductName = productName;
            this.ProductVersion = new Version(productVersion);
        }

        public Products Check(bool async = true)
        {
            Products result = null;

            if (async)
            {
                AsyncGetProducts();
            }
            else
            {
                result = GetProducts();
            }

            return result;
        }

        private void AsyncGetProducts()
        {
            Thread thread = new Thread(() => GetProducts());
            thread.Start();
        }

        private Products GetProducts()
        {
            Products result = new Products();

            try
            {
                _httpClient = new HttpClient();
                string url = String.Format("{0}?ProductName={1}&Version={2}", this.Config.UpdateUrl, this.ProductName, this.ProductVersion);
                byte[] data = _httpClient.Get(url);
                using (MemoryStream memoryStream = new MemoryStream(data))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(UpdateResult));
                    UpdateResult updateResult = (UpdateResult)serializer.Deserialize(memoryStream);
                    //has error ?
                    if (!String.IsNullOrEmpty(updateResult.Error))
                    {
                        throw new Exception(updateResult.Error);
                    }
                    //find new version ..
                    result.AddRange(updateResult.Products.FindAll(obj => obj.ProductName.Equals(this.ProductName) &&
                                                            (new Version(obj.ProductVersion) > ProductVersion)));
                }
            }
            catch (Exception exception)
            {
                OnError(new UpdateErrorArgs(exception));
            }
            
            //has a new version ..
            if (result.Count > 0)
            {
                //tigger callback method .
                OnNewVersion(new NewVersionEventArgs(result));
            }
            else
            {
                OnError(new UpdateErrorArgs("hav't a new version .",-1));
            }
            return result;

        }

        public void Update(Product product)
        {
            string url = product.DownloadLink;
            string filePath = Path.Combine(Path.GetTempPath(), Path.GetFileName(url));
            if (webClient.IsBusy)
            {
                webClient.CancelAsync();
            }
            webClient.DownloadFileAsync(new Uri(url), filePath);
        }
    }
}
