using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Streaming.Application.Interfaces.Services;
using Streaming.Application.Interfaces.Settings;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Streaming.Application.Services
{
    public class AzureBlobClient : IAzureBlobClient
	{
        private CloudStorageAccount storageAccount;
        private readonly Lazy<CloudBlobClient> client;

        public AzureBlobClient(IAzureBlobConnectionString blobConnectionString)
		{
            storageAccount = CloudStorageAccount.Parse(blobConnectionString.AzureBlobConnectionString);
            client = new Lazy<CloudBlobClient>(() => storageAccount.CreateCloudBlobClient());
        }

        public string GetFileLinkSecuredSAS(string ContainerName, string FileName)
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
            return $"{GetFileLink(ContainerName, FileName)}{token}";
        }

        public string GetFileLink(string ContainerName, string FileName)
            => string.Format($"{storageAccount.BlobEndpoint}{ContainerName}/{FileName}");

        public async Task<Stream> GetFileAsync(string ContainerName, string FileName)
		{
            var blob = await client.Value.GetBlobReferenceFromServerAsync(new Uri($"{storageAccount.BlobEndpoint}{ContainerName}/{FileName}"));
            return await blob.OpenReadAsync();
		}

		public async Task UploadFileAsync(string ContainerName, string FileName, Stream InputStream)
		{
            var blob = client.Value.GetContainerReference(ContainerName).GetBlockBlobReference(FileName);
            await blob.UploadFromStreamAsync(InputStream);
        }
    }
}
