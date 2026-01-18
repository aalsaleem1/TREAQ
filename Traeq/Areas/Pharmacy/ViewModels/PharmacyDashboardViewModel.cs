namespace Traeq.Areas.Pharmacy.ViewModels
{
    public class PharmacyDashboardViewModel
    {
        public string PharmacyName { get; set; }
        public string LicenseNumber { get; set; }
        public string OwnerName { get; set; }
        public string LogoUrl { get; set; }
        public string City { get; set; }

        public int TotalMedicines { get; set; }
        public int LowStockCount { get; set; }
        public decimal DailyRevenue { get; set; }
        public int DailyOrdersCount { get; set; }

        public List<RecentOrderViewModel> RecentOrders { get; set; }
    }

    public class RecentOrderViewModel
    {
        public int OrderNumber { get; set; }
        public string CustomerName { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public DateTime Date { get; set; }
    }
}