using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SixteenSounds.Migrations
{
    /// <inheritdoc />
    public partial class AddFileNameAndCreatedAtToSamples : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Samples_Users_UserId",
                table: "Samples");

            migrationBuilder.DropIndex(
                name: "IX_Samples_UserId",
                table: "Samples");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Samples");

            migrationBuilder.RenameColumn(
                name: "UploadedAt",
                table: "Samples",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "FilePath",
                table: "Samples",
                newName: "FileName");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Samples",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FileName",
                table: "Samples",
                newName: "FilePath");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Samples",
                newName: "UploadedAt");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Samples",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Samples",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Samples_UserId",
                table: "Samples",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Samples_Users_UserId",
                table: "Samples",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
