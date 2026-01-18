using Traeq.Models;
using Xunit;

namespace Traeq.UnitTests
{
    public class PromoCodeTests
    {
        [Fact] 
        public void PromoCode_Valid_IsAccepted()
        {
            var promo = new PharmacyPromoCode
            {
                Code = "TREAQ03",
                DiscountPercent = 10
            };

            var results = TestHelper.Validate(promo);
            Assert.Empty(results);
        }
        [Fact]
        public void PromoCode_WithSymbols_IsAccepted()
        {
            var promo = new PharmacyPromoCode
            {
                Code = "TREAQ03$$",
                DiscountPercent = 10
            };

            var results = TestHelper.Validate(promo);

            Assert.Empty(results);
        }


        [Fact] 
        public void PromoCode_DiscountOutOfRange_Fails()
        {
            var promo = new PharmacyPromoCode
            {
                Code = "TREAQ03",
                DiscountPercent = 150
            };

            var results = TestHelper.Validate(promo);
            Assert.NotEmpty(results);
        }
    }
}
