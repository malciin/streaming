namespace Streaming.Application.Services
{
    public interface IMessageSignerService
    {
        /// <summary>
        /// Sign message with hidden key.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        byte[] SignMessage(byte[] message);

        /// <summary>
        /// Get message from signed message.
        /// </summary>
        /// <param name="signedMessage"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Thrown signed message is 
        /// malformed, for example when signed part is different</exception>
        byte[] GetMessage(byte[] signedMessage);
    }
}
