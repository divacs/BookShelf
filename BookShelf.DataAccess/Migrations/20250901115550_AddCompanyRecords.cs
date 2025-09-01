using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookShelf.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyRecords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "City", "Name", "PhoneNumber", "PostalCode", "StreetAddress" },
                values: new object[] { "Mil City", "Vivid Tech", "09752353425", "112200", "999 mil st" });

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "City", "Name", "PhoneNumber", "PostalCode", "State", "StreetAddress" },
                values: new object[] { "Mark City", "Mark Books", "654444444442", "1122099", "KJ", "999 mark st" });

            migrationBuilder.InsertData(
                table: "Companies",
                columns: new[] { "Id", "City", "Name", "PhoneNumber", "PostalCode", "State", "StreetAddress" },
                values: new object[] { 1, "Vid City", "Vivid Books", "9798786776", "7777777", "TL", "999 vid st" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "City", "Name", "PhoneNumber", "PostalCode", "StreetAddress" },
                values: new object[] { "Vid City", "Vivid Books", "9798786776", "7777777", "999 vid st" });

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "City", "Name", "PhoneNumber", "PostalCode", "State", "StreetAddress" },
                values: new object[] { "Mil City", "Vivid Tech", "09752353425", "112200", "TL", "999 mil st" });

            migrationBuilder.InsertData(
                table: "Companies",
                columns: new[] { "Id", "City", "Name", "PhoneNumber", "PostalCode", "State", "StreetAddress" },
                values: new object[] { 4, "Mark City", "Mark Books", "654444444442", "1122099", "KJ", "999 mark st" });
        }
    }
}
