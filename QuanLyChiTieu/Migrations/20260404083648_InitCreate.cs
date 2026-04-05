using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyChiTieu.Migrations
{
    /// <inheritdoc />
    public partial class InitCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CATEGORY",
                columns: table => new
                {
                    categoryId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    categoryName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    type = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__CATEGORY__23CAF1D853E3E3EC", x => x.categoryId);
                });

            migrationBuilder.CreateTable(
                name: "USER_ACCOUNT",
                columns: table => new
                {
                    userId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    username = table.Column<string>(type: "nvarchar(50)", unicode: false, maxLength: 50, nullable: false),
                    password = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    email = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__USER_ACC__CB9A1CFF29145B10", x => x.userId);
                });

            migrationBuilder.CreateTable(
                name: "BUDGET",
                columns: table => new
                {
                    budgetId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    userId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    categoryId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    limitAmount = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    monthYear = table.Column<string>(type: "varchar(7)", unicode: false, maxLength: 7, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__BUDGET__1E2B71364D1A2E66", x => x.budgetId);
                    table.ForeignKey(
                        name: "FK__BUDGET__category__59063A47",
                        column: x => x.categoryId,
                        principalTable: "CATEGORY",
                        principalColumn: "categoryId");
                    table.ForeignKey(
                        name: "FK__BUDGET__userId__5812160E",
                        column: x => x.userId,
                        principalTable: "USER_ACCOUNT",
                        principalColumn: "userId");
                });

            migrationBuilder.CreateTable(
                name: "WALLET",
                columns: table => new
                {
                    walletId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    userId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    walletName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    balance = table.Column<decimal>(type: "decimal(15,2)", nullable: true, defaultValue: 0m)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__WALLET__3785C8703C8EA1A6", x => x.walletId);
                    table.ForeignKey(
                        name: "FK__WALLET__userId__4D94879B",
                        column: x => x.userId,
                        principalTable: "USER_ACCOUNT",
                        principalColumn: "userId");
                });

            migrationBuilder.CreateTable(
                name: "TRANSACTION",
                columns: table => new
                {
                    transactionId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    walletId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    categoryId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    amount = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    note = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    transactionDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TRANSACT__9B57CF72CFF80705", x => x.transactionId);
                    table.ForeignKey(
                        name: "FK__TRANSACTI__categ__5535A963",
                        column: x => x.categoryId,
                        principalTable: "CATEGORY",
                        principalColumn: "categoryId");
                    table.ForeignKey(
                        name: "FK__TRANSACTI__walle__5441852A",
                        column: x => x.walletId,
                        principalTable: "WALLET",
                        principalColumn: "walletId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BUDGET_categoryId",
                table: "BUDGET",
                column: "categoryId");

            migrationBuilder.CreateIndex(
                name: "IX_BUDGET_userId",
                table: "BUDGET",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_TRANSACTION_categoryId",
                table: "TRANSACTION",
                column: "categoryId");

            migrationBuilder.CreateIndex(
                name: "IX_TRANSACTION_walletId",
                table: "TRANSACTION",
                column: "walletId");

            migrationBuilder.CreateIndex(
                name: "IX_WALLET_userId",
                table: "WALLET",
                column: "userId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BUDGET");

            migrationBuilder.DropTable(
                name: "TRANSACTION");

            migrationBuilder.DropTable(
                name: "CATEGORY");

            migrationBuilder.DropTable(
                name: "WALLET");

            migrationBuilder.DropTable(
                name: "USER_ACCOUNT");
        }
    }
}
