using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NationalCardBookingSystemWithoutCleanArch.Migrations
{
    /// <inheritdoc />
    public partial class AddEncryptionAndIndexing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BookingSettings_UserId",
                table: "BookingSettings");

            migrationBuilder.AlterColumn<string>(
                name: "NationalId",
                table: "FamilyMembers",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Office",
                table: "BookingSettings",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Governorate",
                table: "BookingSettings",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_BookingSettings_UserId_Governorate_Office",
                table: "BookingSettings",
                columns: new[] { "UserId", "Governorate", "Office" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BookingSettings_UserId_Governorate_Office",
                table: "BookingSettings");

            migrationBuilder.AlterColumn<string>(
                name: "NationalId",
                table: "FamilyMembers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "Office",
                table: "BookingSettings",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Governorate",
                table: "BookingSettings",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_BookingSettings_UserId",
                table: "BookingSettings",
                column: "UserId");
        }
    }
}
