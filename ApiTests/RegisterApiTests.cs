using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Traeq.ApiTests
{
    public class RegisterApiTests
    {
        private HttpClient CreateClient()
        {
            var factory = new WebApplicationFactory<Program>();
            return factory.CreateClient();
        }

        [Fact]
        public async Task POST_Register_Returns_OK()
        {
            var client = CreateClient();

            var form = new MultipartFormDataContent
            {
                { new StringContent("Test User"), "FullName" },
                { new StringContent("testuser01"), "Username" },
                { new StringContent("test01@test.com"), "Email" },
                { new StringContent("Amman"), "City" },
                { new StringContent("Abdali"), "District" },
                { new StringContent("Test@123"), "Password" },
                { new StringContent("Test@123"), "ConfirmPassword" },
                { new StringContent("User"), "AccountType" }
            };

            var response = await client.PostAsync("/User/Register", form);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
