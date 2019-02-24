using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Streaming.Auth0
{
    internal static class RsaSecurityKeyHelper
    {
        private static string DownloadJWSK(string authenticationDomain)
        {
            var uri = new Uri(authenticationDomain);
            using (var webClient = new WebClient())
            {
                var str = $"{uri.AbsoluteUri}.well-known/jwks.json";
                return Encoding.UTF8.GetString(webClient.DownloadData(str));
            }
        }

        private static SecurityKey GetSecurityKeyFromJWSK(string JWSK)
        {
            var jwksJObject = JObject.Parse(JWSK);

            var parameters = new RSAParameters();
            parameters.Exponent = Base64UrlEncoder.DecodeBytes(jwksJObject["keys"][0]["e"].ToString());
            parameters.Modulus = Base64UrlEncoder.DecodeBytes(jwksJObject["keys"][0]["n"].ToString());
            return new RsaSecurityKey(parameters);
        }

        /// <summary>
        /// Gets security keys remotely if not localFileName is exists. Otherwise if localFileName is not exists and is not null
        /// the downloaded security key is saved in that file
        /// </summary>
        /// <param name="authenticationDomain">Authentication domain from which JWSK is downloaded</param>
        /// <param name="localFileName">(optional) Firstly try to read JWSK from this file and if not exists save JWSK to that file</param>
        /// <returns></returns>
        internal static SecurityKey GetSecurityKey(string authenticationDomain, string localFileName = null)
        {
            string jwsk;

            if (localFileName != null && File.Exists(localFileName))
            {
                jwsk = File.ReadAllText(localFileName);
            }
            else
            {
                jwsk = DownloadJWSK(authenticationDomain);

                if (localFileName != null)
                    File.WriteAllText(localFileName, jwsk);
            }

            return GetSecurityKeyFromJWSK(jwsk);
        }
    }
}
