using Xunit;
using System;
using System.Collections.Generic;
using FinancialAccountManagementSystem.Models.Entities;


namespace FinancialAccountManagementSystem.UnitTests
{
    public class AccountTransactionTests
    {
        [Fact]
        public void CanAddTransactionToAccount()
        {
           
            var account = new Account
            {
                Balance = 1000m,
                Transactions = new List<Transaction>()
            };

            var transaction = new Transaction
            {
                Amount = 100m,
                AccountId = account.Id 
            };

            account.Transactions.Add(transaction);

            Assert.Single(account.Transactions); 
            Assert.Equal(100m, account.Transactions.ElementAt(0).Amount); 
        }

        [Fact]
        public void CanDepositValidAmount()
        {
        
            var account = new Account
            {
                Balance = 1000m
            };
            decimal depositAmount = 100m;
            account.Deposit(depositAmount);

            Assert.Equal(1100m, account.Balance); 
        }

        [Fact]
        public void Deposit_NegativeAmount_ThrowsArgumentException()
        {

            var account = new Account();
            decimal depositAmount = -100m;

            Assert.Throws<ArgumentException>(() => account.Deposit(depositAmount)); 
        }

        [Fact]
        public void Deposit_ZeroAmount_ThrowsArgumentException()
        {
            var account = new Account();
            decimal depositAmount = 0m;

            Assert.Throws<ArgumentException>(() => account.Deposit(depositAmount)); 
        }
    }

}
