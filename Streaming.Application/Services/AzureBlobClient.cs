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
		private readonly string defaultEndpointsProtocol;
		private readonly string accountName;
		private readonly string accountKey;
		private readonly string endpointSuffix;


		public AzureBlobClient(string ConnectionString)
		{
			var regex = new Regex(@"DefaultEndpointsProtocol=(.*);AccountName=(.*);AccountKey=(.*);EndpointSuffix=(.*)");
			var matches = regex.Match(ConnectionString);

			defaultEndpointsProtocol = matches.Groups[1].Value;
			accountName = matches.Groups[2].Value;
			accountKey = matches.Groups[3].Value;
			endpointSuffix = matches.Groups[4].Value;
		}

		public async Task<Stream> GetFileAsync(string ContainerName, string FileName)
		{
			var client = new HttpClient();
            var request = createBlobRequest($"{defaultEndpointsProtocol}://{accountName}.blob.{endpointSuffix}/{ContainerName}/{FileName}", HttpMethod.Get);
            var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
			return await response.Content.ReadAsStreamAsync();
		}

		public async Task UploadFileAsync(string ContainerName, string FileName, Stream InputStream)
		{
			var client = new HttpClient();
            var request = createBlobRequest($"{defaultEndpointsProtocol}://{accountName}.blob.{endpointSuffix}/{ContainerName}/{FileName}", HttpMethod.Put, InputStream);
            var response = await client.SendAsync(request);
		}

		#region AzureBlobHelpers
		private (string Key, string Value) getAuthorizationHeader(HttpRequestMessage message, Stream contentStream = null)
		{
			var stringBuilder = new StringBuilder();
			stringBuilder.Append($"{message.Method.ToString().ToUpper()}\n");
			stringBuilder.Append(String.Format("\n\n{0}\n\n\n\n\n\n\n\n\n", 
				contentStream?.Length.ToString() ?? ""));

			// Creating CanonicalizedHeaders
			// Check https://docs.microsoft.com/en-us/azure/storage/common/storage-rest-api-auth for more information
			foreach (var header in message.Headers.Where(x => x.Key.StartsWith("x-ms-")).OrderBy(x => x.Key))
			{
				stringBuilder.Append($"{header.Key}:{header.Value.First()}\n");
			}

			// Canonicalized Resource
			// Check https://docs.microsoft.com/en-us/azure/storage/common/storage-rest-api-auth for more information

			stringBuilder.Append($"/{accountName}").Append(message.RequestUri.AbsolutePath);

			// Address.Query is the resource, such as "?comp=list".
			// This ends up with a NameValueCollection with 1 entry having key=comp, value=list.
			// It will have more entries if you have more query parameters.
			NameValueCollection values = HttpUtility.ParseQueryString(message.RequestUri.Query);

			foreach (var item in values.AllKeys.OrderBy(k => k))
			{
				stringBuilder.Append('\n').Append(item).Append(':').Append(values[item]);
			}

			var hasher = new HMACSHA256(Convert.FromBase64String(accountKey));
			var hash = hasher.ComputeHash(Encoding.UTF8.GetBytes(stringBuilder.ToString()));

			return ("Authorization", $"SharedKey {accountName}:{Convert.ToBase64String(hash)}");
		}

		HttpRequestMessage createBlobRequest(string Url, HttpMethod method, Stream contentStream = null)
		{
			var nowDate = DateTime.UtcNow;
			var message = new HttpRequestMessage
			{
				Method = method,
				Headers = {
					{ "x-ms-version", "2017-07-29" },
					{ "x-ms-date", nowDate.ToString("r") },
				},
				RequestUri = new Uri(Url)
			};

			if (contentStream != null)
			{
				message.Headers.Add("x-ms-blob-type", "BlockBlob");
				message.Content = new StreamContent(contentStream);
				message.Content.Headers.Add("Content-Length", contentStream.Length.ToString());
			}

			var authorizationHeader = getAuthorizationHeader(message, contentStream);
			message.Headers.Add(authorizationHeader.Key, authorizationHeader.Value);
			return message;
		}
		#endregion
	}
}
