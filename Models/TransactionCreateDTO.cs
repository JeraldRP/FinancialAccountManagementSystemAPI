namespace FinancialAccountManagementSystem.Models
{
    public class TransactionCreateDTO
    {
        public int AccountId { get; set; }
        public string? TransactionType { get; set; }
        public decimal Amount { get; set; }
    }
}
