using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Traeq.IntegrationTests
{
    public class RegisterIntegrationTests
    {
        private HttpClient CreateClient()
        {
            var factory = new WebApplicationFactory<Program>();
            return factory.CreateClient();
        }

        [Fact]
        public async Task Register_ValidUser_ReturnsOk()
        {
            var client = CreateClient();

            var form = new MultipartFormDataContent
            {
                 {new StringContent("User"),"AccountType" },
                { new StringContent("testuser01"), "Username" },
                { new StringContent("Test User"), "FullName" },
                { new StringContent("test01@gmail.com"), "Email" },
                { new StringContent("Test@123"), "Password" },
                { new StringContent("Test@123"), "ConfirmPassword" }
            };

            var response = await client.PostAsync("/User/Register", form);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Register_MissingEmail_ReturnsOk()
        {
            var client = CreateClient();

            var form = new MultipartFormDataContent
            {
                 {new StringContent("User"),"AccountType" },
                { new StringContent("testuser02"), "Username" },
                { new StringContent("Test User"), "FullName" },
                { new StringContent("Test@123"), "Password" },
                { new StringContent("Test@123"), "ConfirmPassword" }
            };

            var response = await client.PostAsync("/User/Register", form);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Register_PasswordMismatch_ReturnsOk()
        {
            var client = CreateClient();

            var form = new MultipartFormDataContent
            {
                {new StringContent("User"),"AccountType" },
                { new StringContent("testuser03"), "Username" },
                { new StringContent("Test User"), "FullName" },
                { new StringContent("test03@gmail.com"), "Email" },
                { new StringContent("Test@123"), "Password" },
                { new StringContent("Test@321"), "ConfirmPassword" }
            };

            var response = await client.PostAsync("/User/Register", form);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Register_Empty_ReturnsOk()
        {
            var client = CreateClient();

            var response = await client.PostAsync("/User/Register", null);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
