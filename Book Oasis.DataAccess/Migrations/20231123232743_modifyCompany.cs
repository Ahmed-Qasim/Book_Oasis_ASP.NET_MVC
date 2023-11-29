using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Book_Oasis.Migrations
{
    /// <inheritdoc />
    public partial class modifyCompany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PhoneNumber",
                table: "companies",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "companies");
        }
    }
}
