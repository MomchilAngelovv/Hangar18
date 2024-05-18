using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hangar18.Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_parent_box_to_box : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ParentBoxId",
                table: "Boxes",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Boxes_ParentBoxId",
                table: "Boxes",
                column: "ParentBoxId");

            migrationBuilder.AddForeignKey(
                name: "FK_Boxes_Boxes_ParentBoxId",
                table: "Boxes",
                column: "ParentBoxId",
                principalTable: "Boxes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Boxes_Boxes_ParentBoxId",
                table: "Boxes");

            migrationBuilder.DropIndex(
                name: "IX_Boxes_ParentBoxId",
                table: "Boxes");

            migrationBuilder.DropColumn(
                name: "ParentBoxId",
                table: "Boxes");
        }
    }
}
