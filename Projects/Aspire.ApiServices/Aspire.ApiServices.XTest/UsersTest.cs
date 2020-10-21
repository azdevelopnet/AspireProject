using System.Collections.Generic;
using System.Threading.Tasks;
using Aspire.ApiServices.Controllers;
using Aspire.ApiServices.Models;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Aspire.ApiServices.XTest
{
    public class UsersTest
    {
        UsersController ctrl;

        public UsersTest()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            ctrl = new UsersController(config);
        }

        [Fact]
        public async Task GetAllUsers()
        {
            var userResults = await ctrl.GetAll();
            //There should be 401 users in the account
            Assert.Equal(401, userResults.Count);
        }
    }
}
