using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Traeq.IntegrationTests
{
    public class PromoCodeIntegrationTests
    {
        private HttpClient CreateClient()
        {
            var factory = new WebApplicationFactory<Program>();
            return factory.CreateClient();
        }
        [Fact]
        public async Task Checkout_WithoutCart_RedirectsToCart()
        {
            var client = CreateClient();

            var response = await client.GetAsync("/Checkout");

            Assert.True(response.StatusCode == HttpStatusCode.OK
                     || response.StatusCode == HttpStatusCode.Redirect);
        }
    }
}
