using FinancialAccountManagementSystem.Data;
using FinancialAccountManagementSystem.Models;
using FinancialAccountManagementSystem.Models.Entities;
using FinancialAccountManagementSystem.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinancialAccountManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly AppDbContext appDbContext;
        private readonly AccountService accountService;

        public AccountsController(AppDbContext appDbContext, AccountService accountService)
        {
            this.appDbContext = appDbContext;
            this.accountService = accountService;
        }

        // GET: api/transactions/account/{accountId}
        [HttpGet("account/{accountId}")]
        public async Task<ActionResult<IEnumerable<TransactionDTO>>> GetTransactionsByAccountId(int accountId)
        {
            var transactions = await accountService.GetTransactionsForAccount(accountId);

            if (!transactions.Any())
            {
                return NotFound($"No transactions found for account with ID {accountId}.");
            }

            return Ok(transactions);
        }

        // GET: api/accounts/total-balance
        [HttpGet("total-balance")]
        public async Task<ActionResult<decimal>> GetTotalBalanceOfAllAccounts()
        {
            var totalBalance = await appDbContext.Accounts.SumAsync(a => a.Balance);

            return Ok(totalBalance);
        }
        // GET: api/accounts/below-balance/{threshold}
        [HttpGet("below-balance/{threshold}")]
        public async Task<ActionResult<IEnumerable<AccountDTO>>> GetAccountsBelowBalance(decimal threshold)
        {
            var accounts = await accountService.GetBelowBalance(threshold);

            if (!accounts.Any())
            {
                return NotFound($"No accounts found with balance below {threshold}.");
            }

            return Ok(accounts);
        }
        // GET: api/accounts/top-5
        [HttpGet("top-5")]
        public async Task<ActionResult<IEnumerable<AccountDTO>>> GetTop5AccountsByBalance()
        {
            var topAccounts = await accountService.GetTopFiveAccountsByBalance();
            return Ok(topAccounts);
        }

            [HttpGet]
        public async Task<ActionResult<IEnumerable<Account>>> GetAllAccounts()
        {
            return Ok(await appDbContext.Accounts.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Account>> GetAccount(int id)
        {
            var accountEntity = await appDbContext.Accounts
                .Where(a => a.Id == id)
                .Select(a => new Account
                {
                    Id = a.Id,
                    AccountNumber = a.AccountNumber,
                    AccountHolder = a.AccountHolder,
                    Balance = a.Balance,
                    Transactions = a.Transactions
                }).FirstOrDefaultAsync();

            if (accountEntity == null)
            {
                return NotFound();
            }

            return Ok(accountEntity);
        }

        [HttpPost]
        public async Task<ActionResult<AccountDTO>> AddAccount(AccountDTO accountDTO)
        {
            // Validate the incoming DTO
            if (accountDTO == null || string.IsNullOrWhiteSpace(accountDTO.AccountNumber) || string.IsNullOrWhiteSpace(accountDTO.AccountHolder))
            {
                return BadRequest("Invalid account data. Please provide an account number and an account holder name.");
            }

            var accountEntity = new Account()
            {
                AccountNumber = accountDTO.AccountNumber,
                AccountHolder = accountDTO.AccountHolder,
                Balance = accountDTO.Balance
            };

            appDbContext.Accounts.Add(accountEntity);
            await appDbContext.SaveChangesAsync();

            accountDTO.Id = accountEntity.Id;

            return CreatedAtAction(nameof(GetAccount), new { id = accountDTO.Id }, accountDTO);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<AccountDTO>> UpdateAccount(int id, AccountDTO accountDTO)
        {
            // Validate the incoming DTO
            if (accountDTO == null || string.IsNullOrWhiteSpace(accountDTO.AccountNumber) || string.IsNullOrWhiteSpace(accountDTO.AccountHolder))
            {
                return BadRequest("Invalid account data. Please provide an account number and an account holder name.");
            }

            // Find the existing account by Id
            var existingAccount = await appDbContext.Accounts.FindAsync(id);
            if (existingAccount == null)
            {
                return NotFound($"Account with ID {id} not found.");
            }

            // Update
            existingAccount.AccountNumber = accountDTO.AccountNumber;
            existingAccount.AccountHolder = accountDTO.AccountHolder;
            existingAccount.Balance = accountDTO.Balance;

            await appDbContext.SaveChangesAsync();

            // Return the updated DTO
            accountDTO.Id = existingAccount.Id;
            return Ok(accountDTO);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            // Find the existing account by Id
            var existingAccount = await appDbContext.Accounts.FindAsync(id);
            if (existingAccount == null)
            {
                return NotFound($"Account with ID {id} not found.");
            }

            // Delete
            appDbContext.Accounts.Remove(existingAccount);
            await appDbContext.SaveChangesAsync();

            return Ok(existingAccount);
        }



    }
}
