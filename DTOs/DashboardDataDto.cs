namespace ShopEProduction.DTOs
{
    public class DashboardDataDto
    {
        public int Year { get; set; }
        public int Period { get; set; } // Month, Week, or Day depending on granularity
        public int OrderCount { get; set; }
        public decimal TotalSpent { get; set; }
    }
}
