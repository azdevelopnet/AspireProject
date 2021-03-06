using System.Collections.Generic;
using System.Threading.Tasks;
using Aspire.ApiServices.Models;
using Xunit;

namespace Aspire.ApiServices.XTest
{
    public class HttpTest
    {
        private UnitTestHelper helper;

        string baseUrl = "https://localhost:5001";

        public HttpTest()
        {
            helper = new UnitTestHelper();
        }

        [Fact]
        public async Task Authenticate()
        {
            var url = baseUrl + "/Auth/Authenticate";
            var usr = new User()
            {
                Email = "admin@aspire.com",
                Password = helper.CreateMD5("admin")
            };
            var result = await helper.PostAsync<RawString>(url, usr, new ApiKey()
            {
                Name = "ApiKey",
                KeyValue = "f07eb39e-71f9-484f-8438-4b22eb4b4d95"
            });

            helper.BearerToken = result.Content;

        }

        [Fact]
        public async Task GetAllUsers()
        {
            if (helper.BearerToken == null)
            {
                await Authenticate();
            }

            var url = baseUrl + "/Users/GetAll";
            var result = await helper.GetAsync<List<User>>(url);
            Assert.NotEmpty(result);

        }
    }
}
