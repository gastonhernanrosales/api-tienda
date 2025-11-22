using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebTonyWilly.Migrations
{
    /// <inheritdoc />
    public partial class AddTotalesTurno : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TotalEfectivo",
                table: "Turnos",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalMP",
                table: "Turnos",
                type: "numeric",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalEfectivo",
                table: "Turnos");

            migrationBuilder.DropColumn(
                name: "TotalMP",
                table: "Turnos");
        }
    }
}
