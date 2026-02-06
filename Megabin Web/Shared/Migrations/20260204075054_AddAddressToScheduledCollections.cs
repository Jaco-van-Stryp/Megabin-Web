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
            // Step 1: Add AddressId as nullable to avoid FK violations on existing rows
            migrationBuilder.AddColumn<Guid>(
                name: "AddressId",
                table: "ScheduledCollections",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RouteSequence",
                table: "ScheduledCollections",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            // Step 2: Delete any existing scheduled collections that have no valid address
            // (these are orphaned records from before the schema change)
            migrationBuilder.Sql(
                "DELETE FROM \"ScheduledCollections\" WHERE \"AddressId\" IS NULL");

            // Step 3: Make AddressId non-nullable now that orphaned rows are removed
            migrationBuilder.AlterColumn<Guid>(
                name: "AddressId",
                table: "ScheduledCollections",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

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
