﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Streaming.Common.Extensions;

namespace Streaming.Tests.EndToEnd
{
    public class DockerTestDatabase : ITestDatabase
    {
        private const string MongoDbContainerName = "streaming-mongodb-tests";
        private static bool _databaseAlreadyRunning = false;
        private static string _alreadyRunningDatabaseConnectionString = "";

        private static void RemoveContainerByName(string containerName)
        {
            try
            {
                $"docker rm -f $(docker ps -q --filter name={containerName})"
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
                RemoveContainerByName(MongoDbContainerName);
                var containerId = $"docker run -d --name {MongoDbContainerName} -P mongo"
                    .ExecuteBashAsync().GetAwaiter().GetResult();
                var databasePort = Int32.Parse(($"docker ps --filter 'name={MongoDbContainerName}' --format " + "'{{.Ports}}'")
                    .ExecuteBashAsync().GetAwaiter().GetResult()
                    .Split("->").First().Split(":").Last());

                _databaseAlreadyRunning = true;
                _alreadyRunningDatabaseConnectionString = $"mongodb://localhost:{databasePort}";
            }
            else
            {
                var anyClassMap = BsonClassMap.GetRegisteredClassMaps().FirstOrDefault();
                if (anyClassMap != null)
                {
                    var classMapDictionaryField = typeof(BsonClassMap).GetField("__classMaps", BindingFlags.Static | BindingFlags.NonPublic);
                    var classMapDictionary = (Dictionary<Type, BsonClassMap>)classMapDictionaryField.GetValue(anyClassMap);
                    classMapDictionary.Clear();
                }

                var client = new MongoClient(_alreadyRunningDatabaseConnectionString);
                foreach (var name in client.ListDatabaseNames().ToEnumerable())
                {
                    if (!String.Equals(name, "admin", StringComparison.InvariantCultureIgnoreCase))
                        client.DropDatabase(name);
                }
            }
            return _alreadyRunningDatabaseConnectionString;
        }
    }
}