using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Traeq.IntegrationTests
{
    public class CheckoutIntegrationTests
    {
        private HttpClient CreateClient()
        {
            var factory = new WebApplicationFactory<Program>();
            return factory.CreateClient();
        }

        [Fact]
        public async Task OrderConfirmation_WithAnyId_ReturnsOk()
        {
            var client = CreateClient();

            var response = await client.GetAsync("/Checkout/OrderConfirmation/34");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Checkout_Page_LoadsSuccessfully()
        {
            var client = CreateClient();

            var response = await client.GetAsync("/Checkout");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}

