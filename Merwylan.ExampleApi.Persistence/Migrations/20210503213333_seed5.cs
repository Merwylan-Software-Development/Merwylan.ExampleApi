using Microsoft.EntityFrameworkCore.Migrations;

namespace Merwylan.ExampleApi.Persistence.Migrations
{
    public partial class seed5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "HashedPassword", "Username" },
                values: new object[] { 1, "$2a$11$CvBqa.owIsNm1bVVdg4sSO1p0SmEkRmpmljo3Ys9jj/Wg2eN5wcxK", "root" });
        }
    }
}
