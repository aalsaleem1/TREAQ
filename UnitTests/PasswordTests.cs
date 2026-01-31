using Traeq.DTO;
using Xunit;

namespace Traeq.UnitTests
{
    public class PasswordTests
    {
        [Fact] 
        public void Password_Valid_IsAccepted()
        {
            var dto = new RegisterDTO
            {
                Username = "Omar03",
                FullName = "Omar",
                Email = "omar@gmail.com",
                Password = "Omar@2003",
                ConfirmPassword = "Omar@2003",
                AccountType = "User"
            };

            var results = TestHelper.Validate(dto);
            Assert.Empty(results);
        }

        [Fact] 
        public void Password_TooShort_Fails()
        {
            var dto = new RegisterDTO
            {
                Password = "Om@1",
                ConfirmPassword = "Om@1"
            };

            var results = TestHelper.Validate(dto);
            Assert.NotEmpty(results);
        }


    }
}

