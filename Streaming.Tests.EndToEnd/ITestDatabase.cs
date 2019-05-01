namespace Streaming.Tests.EndToEnd
{
    internal interface ITestDatabase
    {
        /// <summary>
        /// Start clean test database
        /// </summary>
        /// <returns>Connection string to database</returns>
        string Start();
    }
}
