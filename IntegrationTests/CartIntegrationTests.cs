using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using Xunit;

namespace Traeq.IntegrationTests
{
    public class CartIntegrationTests
    {
        private HttpClient CreateClient()
        {
            var factory = new WebApplicationFactory<Program>();
            return factory.CreateClient();
        }

        [Fact]
        public async Task AddToCart_WithAnyMedicine_ReturnsSuccess()
        {
            var client = CreateClient();

            var response = await client.PostAsync(
                "/Cart/AddToCart?medicineId=24&quantity=1", null);

            Assert.True(
                response.StatusCode == HttpStatusCode.OK ||
                response.StatusCode == HttpStatusCode.Redirect
            );
        }

        [Fact]
        public async Task ViewCart_ReturnsOk()
        {
            var client = CreateClient();

            var response = await client.GetAsync("/Cart");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}

