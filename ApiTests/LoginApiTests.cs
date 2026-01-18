using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Traeq.ApiTests
{
    public class LoginApiTests
    {
        private HttpClient CreateClient()
        {
            var factory = new WebApplicationFactory<Program>();
            return factory.CreateClient();
        }

        [Fact]
        public async Task POST_Login_Returns_OK()
        {
            var client = CreateClient();

            var json = new StringContent(
                "{\"username\":\"test\",\"password\":\"test\"}",
                Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync("/User/Login", json);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
