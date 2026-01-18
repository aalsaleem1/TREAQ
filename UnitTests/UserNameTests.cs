using Traeq.DTO;
using Xunit;

namespace Traeq.UnitTests
{
    public class UserNameTests
    {
        [Fact] 
        public void Username_Valid_IsAccepted()
        {
            var dto = new RegisterDTO
            {
                Username = "Omar03",
                FullName = "Omar",
                Email = "omar@gmail.com",
                Password = "Omar@2003",
                ConfirmPassword = "Omar@2003" ,
                AccountType = "User"
            };

            var results = TestHelper.Validate(dto);
            Assert.Empty(results);
        }

        [Fact] 
        public void Username_TooShort_Fails()
        {
            var dto = new RegisterDTO { Username = "Om" };
            var results = TestHelper.Validate(dto);
            Assert.NotEmpty(results);
        }

        [Fact]
        public void Username_WithSymbol_Fails()
        {
            var dto = new RegisterDTO { Username = "Omar@3" };
            var results = TestHelper.Validate(dto);
            Assert.NotEmpty(results);
        }

        [Fact]
        public void Username_TooLong_Fails()
        {
            var dto = new RegisterDTO
            {
                Username = new string('a', 30)
            };

            var results = TestHelper.Validate(dto);
            Assert.NotEmpty(results);
        }
    }
}
