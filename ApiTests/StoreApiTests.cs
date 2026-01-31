using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Traeq.ApiTests
{
    public class StoreApiTests
    {
        private HttpClient CreateClient()
        {
            var factory = new WebApplicationFactory<Program>();
            return factory.CreateClient();
        }

        [Fact]
        public async Task GET_Store_Returns_OK()
        {
            var client = CreateClient();

            var response = await client.GetAsync("Home/Stor");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        }
    }
}

