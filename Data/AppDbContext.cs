using FinancialAccountManagementSystem.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinancialAccountManagementSystem.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { 
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureRelationships(modelBuilder);
            ConfigureDecimalPrecision(modelBuilder);

            SeedAccount(modelBuilder);
            SeedTransaction(modelBuilder);
        }

        private void ConfigureRelationships(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>()
                .HasMany(a => a.Transactions)
                .WithOne(t => t.Account)
                .HasForeignKey(a => a.AccountId);
        }
        private void ConfigureDecimalPrecision(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>()
                .Property(a => a.Balance)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Transaction>()
                .Property(t => t.Amount)
                .HasColumnType("decimal(18,2)");
        }

        private void SeedAccount(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>().HasData(
           new Account
           {
               Id = 1,
               AccountNumber = "ACC12345",
               AccountHolder = "John Doe",
               Balance = 1500.00m
           },
           new Account
           {
               Id = 2,
               AccountNumber = "ACC67890",
               AccountHolder = "Jane Smith",
               Balance = 2500.00m
           });
        }

        private void SeedTransaction(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Transaction>().HasData(
            new Transaction
            {
                Id = 1,
                AccountId = 1,
                TransactionType = "Deposit",
                Amount = 500.00m,
                TransactionDate = new DateTime(2024, 9, 1)
            },
            new Transaction
            {
                Id = 2,
                AccountId = 1,
                TransactionType = "Withdrawal",
                Amount = 200.00m,
                TransactionDate = new DateTime(2024, 9, 5)
            },
            new Transaction
            {
                Id = 3,
                AccountId = 2,
                TransactionType = "Deposit",
                Amount = 800.00m,
                TransactionDate = new DateTime(2024, 9, 2)
            },
            new Transaction
            {
                Id = 4,
                AccountId = 2,
                TransactionType = "Withdrawal",
                Amount = 100.00m,
                TransactionDate = new DateTime(2024, 9, 6)
            });
        }

    }
}
