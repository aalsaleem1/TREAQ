using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Text;
using Xunit;

namespace Traeq.IntegrationTests
{
    public class LoginIntegrationTests
    {
        private HttpClient CreateClient()
        {
            var factory = new WebApplicationFactory<Program>();
            return factory.CreateClient();
        }

        [Fact]
        public async Task Login_ValidUsernamePassword_ReturnsOk()
        {
            var client = CreateClient();

            var content = new StringContent(
                "{\"username\":\"omar\",\"password\":\"Omar@2003\"}",
                Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync("/User/Login", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Login_InvalidUsernamePassword_ReturnsOk()
        {
            var client = CreateClient();

            var content = new StringContent(
                "{\"username\":\"omar$$\",\"password\":\"Oma\"}",
                Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync("/User/Login", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        [Fact]
        public async Task Login_Empty_ReturnsOk()
        {
            var client = CreateClient();

            var response = await client.PostAsync("/User/Login", null);

            Assert.False(response.StatusCode == HttpStatusCode.OK );
        }
    }
}

