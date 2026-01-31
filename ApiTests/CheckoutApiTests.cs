using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Traeq.ApiTests
{
    public class CheckoutApiTests
    {
        private HttpClient CreateClient()
        {
            var factory = new WebApplicationFactory<Program>();
            return factory.CreateClient();
        }

        [Fact]
        public async Task GET_Checkout_Returns_Response()
        {
            var client = CreateClient();

            var response = await client.GetAsync("/Checkout");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);


         
        }
    }
}
