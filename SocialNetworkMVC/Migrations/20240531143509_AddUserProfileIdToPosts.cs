using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialNetworkMVC.Migrations
{
    /// <inheritdoc />
    public partial class AddUserProfileIdToPosts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserProfileId",
                table: "Posts",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Posts_UserProfileId",
                table: "Posts",
                column: "UserProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_AspNetUsers_UserProfileId",
                table: "Posts",
                column: "UserProfileId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_AspNetUsers_UserProfileId",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_UserProfileId",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "UserProfileId",
                table: "Posts");
        }
    }
}
