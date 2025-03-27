namespace ShopEProduction.DTOs
{
    public class BankDepositResponse
    {
        public string TransactionId { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; } // Example: "SUCCESS"
    }

}
