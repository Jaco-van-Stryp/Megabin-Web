using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Megabin_Web.Migrations
{
    /// <inheritdoc />
    public partial class AddAddressToScheduledCollections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AddressId",
                table: "ScheduledCollections",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "RouteSequence",
                table: "ScheduledCollections",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledCollections_AddressId",
                table: "ScheduledCollections",
                column: "AddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduledCollections_Addresses_AddressId",
                table: "ScheduledCollections",
                column: "AddressId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScheduledCollections_Addresses_AddressId",
                table: "ScheduledCollections");

            migrationBuilder.DropIndex(
                name: "IX_ScheduledCollections_AddressId",
                table: "ScheduledCollections");

            migrationBuilder.DropColumn(
                name: "AddressId",
                table: "ScheduledCollections");

            migrationBuilder.DropColumn(
                name: "RouteSequence",
                table: "ScheduledCollections");
        }
    }
}
