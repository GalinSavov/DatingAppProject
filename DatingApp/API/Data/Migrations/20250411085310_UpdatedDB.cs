using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserInterests_Interests_InterestId",
                table: "UserInterests");

            migrationBuilder.AddForeignKey(
                name: "FK_UserInterests_Interests_InterestId",
                table: "UserInterests",
                column: "InterestId",
                principalTable: "Interests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserInterests_Interests_InterestId",
                table: "UserInterests");

            migrationBuilder.AddForeignKey(
                name: "FK_UserInterests_Interests_InterestId",
                table: "UserInterests",
                column: "InterestId",
                principalTable: "Interests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
