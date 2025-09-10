namespace Shopping_Web.ViewModels
{
    public class DashboardReport
    {
        public int Year { get; set; }
        public int Month { get; set; }

        public decimal CurrentRevenue { get; set; }
        public decimal PreviousRevenue { get; set; }
        public decimal RevenueGrowthPercent { get; set; }

        public int CurrentOrders { get; set; }
        public int PreviousOrders { get; set; }
        public decimal OrderGrowthPercent { get; set; }

        public int CurrentCustomers { get; set; }
        public int PreviousCustomers { get; set; }
        public decimal CustomerGrowthPercent { get; set; }

        public decimal CurrentConversionRate { get; set; }
        public decimal PreviousConversionRate { get; set; }
        public decimal ConversionRateGrowthPercent { get; set; }
    }


}
