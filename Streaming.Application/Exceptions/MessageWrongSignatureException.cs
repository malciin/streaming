using System;

namespace Streaming.Application.Exceptions
{
    public class MessageWrongSignatureException : Exception
    {
        public override string Message => "Wrong message signature - hashes are not equal";
        public MessageWrongSignatureException(string expectedHash, string givenHash)
        {
            Data.Add("Expected hash", expectedHash);
            Data.Add("Given hash", givenHash);
        }
    }
}
