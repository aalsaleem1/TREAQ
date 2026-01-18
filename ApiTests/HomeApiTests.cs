using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Xunit;

namespace Traeq.ApiTests
{
    public class HomeApiTests
    {
        private HttpClient CreateClient()
        {
            var factory = new WebApplicationFactory<Program>();
            return factory.CreateClient();
        }

        [Fact]
        public async Task GET_Home_Returns_OK()
        {
            var client = CreateClient();

            var response = await client.GetAsync("/");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
