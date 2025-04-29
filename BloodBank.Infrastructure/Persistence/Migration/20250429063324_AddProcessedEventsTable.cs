using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BloodBank.Infrastructure.Persistence.Migration
{
    /// <inheritdoc />
    public partial class AddProcessedEventsTable : Microsoft.EntityFrameworkCore.Migrations.Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProcessedEvents",
                columns: table => new
                {
                    EventId = table.Column<int>(type: "int", nullable: false),
                    ConsumerName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EventType = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessedEvents", x => new { x.EventId, x.ConsumerName });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProcessedEvents");
        }
    }
}
