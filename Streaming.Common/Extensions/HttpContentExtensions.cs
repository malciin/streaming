using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Streaming.Common.Extensions
{
    public static class HttpContentExtensions
    {
        public static T ReadFromJsonAsObject<T>(this HttpContent content)
        {
            return JsonConvert.DeserializeObject<T>(content.ReadAsStringAsync().GetAwaiter().GetResult());
        }

        public static async Task<T> ReadFromJsonAsObjectAsync<T>(this HttpContent content)
        {
            return JsonConvert.DeserializeObject<T>(await content.ReadAsStringAsync());
        }
    }
}
