using System.Collections.Generic;
using System.Linq;
using Moq;
using Streaming.Application.Interfaces.Repositories;
using Streaming.Domain.Models;

namespace Streaming.Tests.Mocks
{
    public class UserRepositoryMock
    {
        /// <summary>
        /// Create mocked IUserRepository that get users from given Dictionary,
        /// can throw exceptions if the user with specific ID does not exists
        /// </summary>
        public static Mock<IUserRepository> CreateForData(IDictionary<string, UserDetails> users)
        {
            var mock = new Mock<IUserRepository>();

            mock.Setup(x => x.GetSingleAsync(It.IsAny<string>()))
                .ReturnsAsync((string id) => users[id]);
            mock.Setup(x => x.GetAsync(It.IsAny<string[]>()))
                .ReturnsAsync((string[] ids) => users.Where(x => ids.Contains(x.Key)).Select(x => x.Value));
            
            return mock;
        }
        
        /// <summary>
        /// Create mocked IUserRepository that always return specific user
        /// </summary>
        public static Mock<IUserRepository> CreateForData(UserDetails user)
        {
            var mock = new Mock<IUserRepository>();

            mock.Setup(x => x.GetSingleAsync(It.IsAny<string>()))
                .ReturnsAsync(user);
            mock.Setup(x => x.GetAsync(It.IsAny<string[]>()))
                .ReturnsAsync(new List<UserDetails> {user});
            
            return mock;
        }
    }
}