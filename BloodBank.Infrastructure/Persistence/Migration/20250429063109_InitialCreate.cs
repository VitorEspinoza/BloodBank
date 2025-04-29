using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BloodBank.Infrastructure.Persistence.Migration
{
    /// <inheritdoc />
    public partial class InitialCreate : Microsoft.EntityFrameworkCore.Migrations.Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateSequence(
                name: "donation_hilo",
                schema: "dbo",
                incrementBy: 10);

            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Street = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Neighborhood = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ZipCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Number = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Complement = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BloodStocks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BloodGroup = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    RhFactor = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    QuantityInMl = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BloodStocks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BloodDonors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BiologicalSex = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    Weight = table.Column<double>(type: "float", nullable: false),
                    BloodGroup = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    RhFactor = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    AddressId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BloodDonors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BloodDonors_Addresses_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Donations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    BloodDonorId = table.Column<int>(type: "int", nullable: false),
                    DonationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    QuantityInMl = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Donations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Donations_BloodDonors_BloodDonorId",
                        column: x => x.BloodDonorId,
                        principalTable: "BloodDonors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_ZipCode_Number",
                table: "Addresses",
                columns: new[] { "ZipCode", "Number" },
                unique: true,
                filter: "([Complement] IS NULL)");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_ZipCode_Number_Complement",
                table: "Addresses",
                columns: new[] { "ZipCode", "Number", "Complement" },
                unique: true,
                filter: "([Complement] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "IX_BloodDonors_AddressId",
                table: "BloodDonors",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_BloodDonors_Email",
                table: "BloodDonors",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Donations_BloodDonorId",
                table: "Donations",
                column: "BloodDonorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BloodStocks");

            migrationBuilder.DropTable(
                name: "Donations");

            migrationBuilder.DropTable(
                name: "BloodDonors");

            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropSequence(
                name: "donation_hilo",
                schema: "dbo");
        }
    }
}
