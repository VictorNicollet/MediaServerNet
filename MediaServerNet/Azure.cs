using System.Configuration;
using System.Text;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;

namespace MediaServerNet
{
    public static class Azure
    {
        public static readonly CloudStorageAccount Account = new CloudStorageAccount(
            new StorageCredentials(
                ConfigurationManager.AppSettings["azure"],
                ConfigurationManager.ConnectionStrings["azure"].ConnectionString), 
            true);

        static Azure()
        {
            var client = Account.CreateCloudBlobClient();
            var container = client.GetContainerReference("gallery");
            var blob = container.GetBlockBlobReference("index.html");

            blob.Properties.ContentType = "text/html";
            using (var stream = blob.OpenWrite())
            {
                var bytes = Encoding.UTF8.GetBytes(Resources.index);
                stream.Write(bytes, 0, bytes.Length);
            }
        }
    }
}
