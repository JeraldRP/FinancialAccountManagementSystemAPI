namespace FinancialAccountManagementSystem.Models.Entities
{
    public class Transaction
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public string? TransactionType { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }

        public Account? Account { get; set; }
    }
}
