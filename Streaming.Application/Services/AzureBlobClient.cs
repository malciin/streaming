using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Streaming.Application.Services
{
	public class AzureBlobClient
	{
        private CloudStorageAccount storageAccount;
        private readonly CloudBlobClient client;

        public AzureBlobClient(string ConnectionString)
		{
            storageAccount = CloudStorageAccount.Parse(ConnectionString);
            client = storageAccount.CreateCloudBlobClient();
        }

        public string GetFileLinkSASAuthorization(string ContainerName, string FileName)
        {            
            SharedAccessAccountPolicy policy = new SharedAccessAccountPolicy()
            {
                Permissions = SharedAccessAccountPermissions.Read,
                Services = SharedAccessAccountServices.Blob,
                ResourceTypes = SharedAccessAccountResourceTypes.Object,
                SharedAccessExpiryTime = DateTime.UtcNow.AddDays(1),
                Protocols = SharedAccessProtocol.HttpsOnly
            };

            var token = storageAccount.GetSharedAccessSignature(policy);
            return string.Format($"{storageAccount.BlobEndpoint}{ContainerName}/{FileName}{token}");
        }

        public async Task<Stream> GetFileAsync(string ContainerName, string FileName)
		{
            var blob = await client.GetBlobReferenceFromServerAsync(new Uri($"{storageAccount.BlobEndpoint}{ContainerName}/{FileName}"));
            return await blob.OpenReadAsync();
		}

		public async Task UploadFileAsync(string ContainerName, string FileName, Stream InputStream)
		{
            var blob = client.GetContainerReference(ContainerName).GetBlockBlobReference(FileName);
            await blob.UploadFromStreamAsync(InputStream);
        }
	}
}
