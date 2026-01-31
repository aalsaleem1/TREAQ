using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Traeq.ApiTests
{
    public class CartApiTests
    {
        private HttpClient CreateClient()
        {
            var factory = new WebApplicationFactory<Program>();
            return factory.CreateClient();
        }

        [Fact]
        public async Task GET_Cart_Returns_OK()
        {
            var client = CreateClient();

            var response = await client.GetAsync("/Cart");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);


        }
    }
}
