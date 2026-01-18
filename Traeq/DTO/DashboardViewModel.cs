using Traeq.Models;

namespace Traeq.DTO
{
    public class DashboardViewModel
    {
        
        public string PharmacyName { get; set; }
        public string LicenseNumber { get; set; }
        public string OwnerName { get; set; }
        public string LogoUrl { get; set; }
        public string City { get; set; }
        public bool IsActive { get; set; } = true;

        
        public int TotalProducts { get; set; }
        public int LowStockCount { get; set; }
        public int TodayOrdersCount { get; set; }
        public decimal MonthlyRevenue { get; set; }

        public List<DashboardOrderDTO> RecentOrders { get; set; } = new List<DashboardOrderDTO>();
    }

    public class DashboardOrderDTO
    {
        public string OrderNumber { get; set; }
        public string CustomerName { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public DateTime Date { get; set; }
    }
}