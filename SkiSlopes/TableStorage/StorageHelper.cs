using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;

namespace Persister.Storage
{
    internal class StorageHelper
    {
        private static CloudTable AuthTable()
        {
            string accountName = "YOUR ACCOUNT NAME HERE";
            string accountKey = "YOUR ACCOUNT KEY HERE";
            try
            {
                StorageCredentials creds = new(accountName, accountKey);
                CloudStorageAccount account = new(creds, useHttps: true);

                CloudTableClient client = account.CreateCloudTableClient();

                CloudTable table = client.GetTableReference("TARGET TABLE NAME");

                return table;
            }
            catch
            {
                return null;
            }
        }
    }
}
