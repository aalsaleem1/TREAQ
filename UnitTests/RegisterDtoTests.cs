using Traeq.DTO;
using Xunit;

namespace Traeq.UnitTests
{
    public class RegisterDtoTests
    {
        [Fact] 
        public void Register_ValidData_IsValid()
        {
            var dto = new RegisterDTO
            {
                Username = "Omar03",
                FullName = "Omar Kamal",
                Email = "omar@gmail.com",
                Password = "Omar@2003",
                ConfirmPassword = "Omar@2003",
                AccountType = "User"
            };

            var results = TestHelper.Validate(dto);
            Assert.Empty(results);
        }

        [Fact] 
        public void Register_InvalidEmail_Fails()
        {
            var dto = new RegisterDTO
            {
                Username = "Omar03",
                FullName = "Omar",
                Email = "omargmail.com",
                Password = "Omar@2003",
                ConfirmPassword = "Omar@2003"
            };

            var results = TestHelper.Validate(dto);
            Assert.NotEmpty(results);
        }

        [Fact] 
        public void Register_PasswordMismatch_Fails()
        {
            var dto = new RegisterDTO
            {
                Username = "Omar03",
                FullName = "Omar",
                Email = "omar@gmail.com",
                Password = "Omar@2003",
                ConfirmPassword = "123"
            };

            var results = TestHelper.Validate(dto);
            Assert.NotEmpty(results);
        }

        [Fact] 
        public void Register_EmptyModel_Fails()
        {
            var dto = new RegisterDTO();
            var results = TestHelper.Validate(dto);
            Assert.NotEmpty(results);
        }
    }
}
