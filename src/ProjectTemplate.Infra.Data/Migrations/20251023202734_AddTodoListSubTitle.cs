using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectTemplate.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTodoListSubTitle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
               name: "SubTitle",
               table: "TodoLists",
               type: "nvarchar(max)",
               nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
               name: "SubTitle",
               table: "TodoLists");
        }
    }
}
