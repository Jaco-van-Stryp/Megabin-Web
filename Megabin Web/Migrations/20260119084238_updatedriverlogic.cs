using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Megabin_Web.Migrations
{
    /// <inheritdoc />
    public partial class updatedriverlogic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Drivers_Addresses_HomeAddressID",
                table: "Drivers");

            migrationBuilder.DropIndex(
                name: "IX_Drivers_HomeAddressID",
                table: "Drivers");

            migrationBuilder.DropIndex(
                name: "IX_Drivers_UserId",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "HomeAddressID",
                table: "Drivers");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Drivers",
                newName: "HomeAddressLabel");

            migrationBuilder.AddColumn<string>(
                name: "DropoffLocationLabel",
                table: "Drivers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "DropoffLocationLat",
                table: "Drivers",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "DropoffLocationLong",
                table: "Drivers",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "HomeAddressLat",
                table: "Drivers",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "HomeAddressLong",
                table: "Drivers",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateIndex(
                name: "IX_Drivers_UserId",
                table: "Drivers",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Drivers_UserId",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "DropoffLocationLabel",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "DropoffLocationLat",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "DropoffLocationLong",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "HomeAddressLat",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "HomeAddressLong",
                table: "Drivers");

            migrationBuilder.RenameColumn(
                name: "HomeAddressLabel",
                table: "Drivers",
                newName: "Name");

            migrationBuilder.AddColumn<Guid>(
                name: "HomeAddressID",
                table: "Drivers",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Drivers_HomeAddressID",
                table: "Drivers",
                column: "HomeAddressID");

            migrationBuilder.CreateIndex(
                name: "IX_Drivers_UserId",
                table: "Drivers",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Drivers_Addresses_HomeAddressID",
                table: "Drivers",
                column: "HomeAddressID",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
