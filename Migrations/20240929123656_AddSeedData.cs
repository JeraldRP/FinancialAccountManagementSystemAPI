using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FinancialAccountManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "Id", "AccountHolder", "AccountNumber", "Balance" },
                values: new object[,]
                {
                    { 1, "John Doe", "ACC12345", 1500.00m },
                    { 2, "Jane Smith", "ACC67890", 2500.00m }
                });

            migrationBuilder.InsertData(
                table: "Transactions",
                columns: new[] { "Id", "AccountId", "Amount", "TransactionDate", "TransactionType" },
                values: new object[,]
                {
                    { 1, 1, 500.00m, new DateTime(2024, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Deposit" },
                    { 2, 1, 200.00m, new DateTime(2024, 9, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Withdrawal" },
                    { 3, 2, 800.00m, new DateTime(2024, 9, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "Deposit" },
                    { 4, 2, 100.00m, new DateTime(2024, 9, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "Withdrawal" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
