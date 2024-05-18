using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hangar18.Data.Migrations
{
    /// <inheritdoc />
    public partial class Parent_box_id_is_no_longer_mandatory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Boxes_Pallets_PalletId",
                table: "Boxes");

            migrationBuilder.AlterColumn<string>(
                name: "PalletId",
                table: "Boxes",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_Boxes_Pallets_PalletId",
                table: "Boxes",
                column: "PalletId",
                principalTable: "Pallets",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Boxes_Pallets_PalletId",
                table: "Boxes");

            migrationBuilder.AlterColumn<string>(
                name: "PalletId",
                table: "Boxes",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Boxes_Pallets_PalletId",
                table: "Boxes",
                column: "PalletId",
                principalTable: "Pallets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
