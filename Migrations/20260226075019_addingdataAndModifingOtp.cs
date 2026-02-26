using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NationalCardBookingSystemWithoutCleanArch.Migrations
{
    /// <inheritdoc />
    public partial class addingdataAndModifingOtp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FailedOtpAttempts",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LockUntil",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "OtpExpireAt",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtpHash",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FailedOtpAttempts",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LockUntil",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "OtpExpireAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "OtpHash",
                table: "Users");
        }
    }
}
