using FinancialAccountManagementSystem.Data;
using FinancialAccountManagementSystem.Models;
using FinancialAccountManagementSystem.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinancialAccountManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly AppDbContext appDbContext;

        public TransactionsController(AppDbContext context)
        {
            appDbContext = context;
        }

        [HttpPost("deposit")]
        public async Task<ActionResult<TransactionDTO>> Deposit(TransactionCreateDTO transactionCreateDTO)
        {
            if (transactionCreateDTO == null || transactionCreateDTO.AccountId <= 0 || transactionCreateDTO.Amount <= 0)
            {
                return BadRequest("Invalid transaction data. Please provide a valid account ID and amount.");
            }

            var account = await appDbContext.Accounts.FindAsync(transactionCreateDTO.AccountId);
            if (account == null)
            {
                return NotFound($"Account with ID {transactionCreateDTO.AccountId} not found.");
            }

            var transaction = new Transaction
            {
                AccountId = transactionCreateDTO.AccountId,
                TransactionType = "Deposit",
                Amount = transactionCreateDTO.Amount,
                TransactionDate = DateTime.UtcNow
            };

            account.Balance += transaction.Amount;

            appDbContext.Transactions.Add(transaction);
            await appDbContext.SaveChangesAsync();

            var transactionDTO = new TransactionDTO
            {
                Id = transaction.Id,
                AccountId = transaction.AccountId,
                TransactionType = transaction.TransactionType,
                Amount = transaction.Amount,
                TransactionDate = transaction.TransactionDate

            };

            return CreatedAtAction(nameof(GetTransaction), new { id = transaction.Id }, transactionDTO);
        }

        [HttpPost("withdraw")]
        public async Task<ActionResult<TransactionDTO>> Withdraw(TransactionCreateDTO transactionCreateDTO)
        {
            if (transactionCreateDTO == null || transactionCreateDTO.AccountId <= 0 || transactionCreateDTO.Amount <= 0)
            {
                return BadRequest("Invalid transaction data. Please provide a valid account ID and amount.");
            }

            var account = await appDbContext.Accounts.FindAsync(transactionCreateDTO.AccountId);
            if (account == null)
            {
                return NotFound($"Account with ID {transactionCreateDTO.AccountId} not found.");
            }

            if (account.Balance < transactionCreateDTO.Amount)
            {
                return BadRequest("Insufficient funds for this withdrawal.");
            }

            var transaction = new Transaction
            {
                AccountId = transactionCreateDTO.AccountId,
                TransactionType = "Withdrawal",
                Amount = transactionCreateDTO.Amount,
                TransactionDate = DateTime.UtcNow
            };

            account.Balance -= transaction.Amount;

            appDbContext.Transactions.Add(transaction);
            await appDbContext.SaveChangesAsync();

            var transactionDTO = new TransactionDTO
            {
                Id = transaction.Id,
                AccountId = transaction.AccountId,
                TransactionType = transaction.TransactionType,
                Amount = transaction.Amount,
                TransactionDate = transaction.TransactionDate
            };

            return CreatedAtAction(nameof(GetTransaction), new { id = transaction.Id }, transactionDTO);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TransactionDTO>> GetTransaction(int id)
        {
            var transaction = await appDbContext.Transactions.FindAsync(id);
            if (transaction == null)
            {
                return NotFound($"Transaction with ID {id} not found.");
            }

            var transactionDTO = new TransactionDTO
            {
                Id = transaction.Id,
                AccountId = transaction.AccountId,
                TransactionType = transaction.TransactionType,
                Amount = transaction.Amount,
                TransactionDate = transaction.TransactionDate
            };

            return Ok(transactionDTO);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TransactionDTO>>> GetAllTransactions()
        {
            var transactions = await appDbContext.Transactions.ToListAsync();
            var transactionDTOs = transactions.Select(t => new TransactionDTO
            {
                Id = t.Id,
                AccountId = t.AccountId,
                TransactionType = t.TransactionType,
                Amount = t.Amount,
                TransactionDate = t.TransactionDate
            }).ToList();

            return Ok(transactionDTOs);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTransaction(int id, TransactionUpdateDTO transactionUpdateDTO)
        {
            if (id <= 0 || transactionUpdateDTO == null)
            {
                return BadRequest("Invalid transaction ID or update data.");
            }

            var transaction = await appDbContext.Transactions.FindAsync(id);
            if (transaction == null)
            {
                return NotFound($"Transaction with ID {id} not found.");
            }

            var account = await appDbContext.Accounts.FindAsync(transaction.AccountId);
            if (account == null)
            {
                return NotFound($"Account with ID {transaction.AccountId} not found.");
            }

            // Adjust balance if the transaction type or amount is changed
            if (transaction.TransactionType == "Withdrawal" && transactionUpdateDTO.Amount != transaction.Amount)
            {
                var balanceDifference = transaction.Amount - transactionUpdateDTO.Amount;
                account.Balance += balanceDifference;
            }

            // Update transaction 
            transaction.Amount = transactionUpdateDTO.Amount;
            transaction.TransactionType = transactionUpdateDTO.TransactionType;
            transaction.TransactionDate = DateTime.UtcNow; 

            await appDbContext.SaveChangesAsync();

            var transactionDtos = new TransactionDTO
            {
                Id = transaction.Id,
                AccountId = transaction.AccountId,
                TransactionType = transaction.TransactionType,
                Amount = transaction.Amount,
                TransactionDate = transaction.TransactionDate
            };

            return Ok(transactionDtos);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            var transaction = await appDbContext.Transactions.FindAsync(id);
            if (transaction == null)
            {
                return NotFound($"Transaction with ID {id} not found.");
            }

            if (transaction.TransactionType == "Withdrawal")
            {
                var account = await appDbContext.Accounts.FindAsync(transaction.AccountId);
                if (account != null)
                {
                    account.Balance += transaction.Amount; // Restore the amount if the transaction is a withdrawal
                }
            }

            appDbContext.Transactions.Remove(transaction);
            await appDbContext.SaveChangesAsync();

            return Ok(transaction);
        }
    }
}
