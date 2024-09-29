namespace FinancialAccountManagementSystem.Models.Entities
{
    public class Account
    {
        public int Id { get; set; }
        public string? AccountNumber { get; set; }
        public string? AccountHolder { get; set; }
        public decimal Balance { get; set; }

        public ICollection<Transaction> Transactions {get; set;} = new List<Transaction>();

        public void Deposit(decimal amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("Deposit amount must be positive.");
            }
            Balance += amount; 
            Transactions.Add(new Transaction { Amount = amount, AccountId = this.Id }); 
        }
    }
}
