using System;
using System.Linq;
using NUnit.Framework;
using Streaming.Common.Extensions;

namespace Streaming.Tests.EndToEnd
{
    public class DockerMongoDbTestDatabase : ITestDatabase
    {
        private const string MongoDbContainerName = "streaming-mongodb-tests";
        private static bool _databaseAlreadyRunning = false;
        private static string _alreadyRunningDatabaseConnectionString = "";

        private static void RemoveContainerByName(string containerName)
        {
            try
            {
                $"docker rm -f $(docker ps -aq --filter name={containerName})"
                    .ExecuteBashAsync().GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                // Just container does not exists, ignore...
            }
        }

        public string Start()
        {
            if (!_databaseAlreadyRunning)
            {
                Assert.DoesNotThrowAsync(() => "docker info".ExecuteBashAsync(), "Please ensure you've got installed Docker - docker is required " +
                                                                                 "to run test mongo database");
                RemoveContainerByName(MongoDbContainerName);
                var containerId = $"docker run -d --name {MongoDbContainerName} -P mongo"
                    .ExecuteBashAsync().GetAwaiter().GetResult();
                var databasePort = Int32.Parse(($"docker ps --filter 'name={MongoDbContainerName}' --format " + "'{{.Ports}}'")
                    .ExecuteBashAsync().GetAwaiter().GetResult()
                    .Split("->").First().Split(":").Last());

                _databaseAlreadyRunning = true;
                _alreadyRunningDatabaseConnectionString = $"mongodb://localhost:{databasePort}";
            }
            return _alreadyRunningDatabaseConnectionString;
        }
    }
}