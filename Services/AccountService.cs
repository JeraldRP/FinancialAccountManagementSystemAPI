using FinancialAccountManagementSystem.Data;
using FinancialAccountManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinancialAccountManagementSystem.Services
{
    public class AccountService
    {
        private readonly AppDbContext appDbContext;

        public AccountService(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public async Task<IEnumerable<TransactionDTO>> GetTransactionsForAccount(int accountId)
        {
            // get Account by Id
            var transactions = await appDbContext.Transactions
                .Where(t => t.AccountId == accountId)
                .Select(t => new TransactionDTO
                {
                    Id = t.Id,
                    AccountId = t.AccountId,
                    TransactionType = t.TransactionType,
                    Amount = t.Amount,
                    TransactionDate = t.TransactionDate
                }).ToListAsync();

            return transactions;
        }

        public async Task<IEnumerable<AccountDTO>> GetBelowBalance(decimal threshold)
        {
            var accounts = await appDbContext.Accounts
                                            .Where(a => a.Balance < threshold)
                                            .Select(a => new AccountDTO
                                            {
                                                Id = a.Id,
                                                AccountNumber = a.AccountNumber,
                                                AccountHolder = a.AccountHolder,
                                                Balance = a.Balance
                                            }).ToListAsync();
            return accounts;
        }

        public async Task<IEnumerable<AccountDTO>> GetTopFiveAccountsByBalance()
        {
            var topAccounts = await appDbContext.Accounts
                                                .OrderByDescending(a => a.Balance)
                                                .Take(5)
                                                .Select(a => new AccountDTO
                                                {
                                                    Id = a.Id,
                                                    AccountNumber = a.AccountNumber,
                                                    AccountHolder = a.AccountHolder,
                                                    Balance = a.Balance
                                                }).ToListAsync();
            return topAccounts;
        }

    }
}
