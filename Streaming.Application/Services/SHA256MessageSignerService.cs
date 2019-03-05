using Streaming.Application.Interfaces.Services;
using Streaming.Application.Interfaces.Settings;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Streaming.Application.Services
{
    public class SHA256MessageSignerService : IMessageSignerService
    {
        private readonly string secretKey;
        public SHA256MessageSignerService(ISecretServerKey secretServerKey)
        {
            this.secretKey = secretServerKey.SecretServerKey;
        }

        public byte[] GetMessage(byte[] signedMessage)
        {
            var hasher = KeyedHashAlgorithm.Create("HmacSHA256");
            hasher.Key = Encoding.UTF8.GetBytes(secretKey);

            var hashLengthBytes = hasher.HashSize / 8;
            var messageLengthBytes = signedMessage.Length - hashLengthBytes;

            if (messageLengthBytes <= 0)
            {
                throw new ArgumentException("Message malformed");
            }

            var message = signedMessage.Take(messageLengthBytes).ToArray();
            var hash = hasher.ComputeHash(message);
            if (!signedMessage.Skip(messageLengthBytes).ToArray().SequenceEqual(hash))
            {
                throw new ArgumentException("Message malformed");
            }

            return message;
        }

        public byte[] SignMessage(byte[] message)
        {
            var hasher = KeyedHashAlgorithm.Create("HmacSHA256");
            hasher.Key = Encoding.UTF8.GetBytes(secretKey);
            byte[] hash = hasher.ComputeHash(message);

            byte[] signedMessage = new byte[message.Length + hash.Length];
            Buffer.BlockCopy(message, 0, signedMessage, 0, message.Length);
            Buffer.BlockCopy(hash, 0, signedMessage, message.Length, hash.Length);
            return signedMessage;
        }
    }
}
