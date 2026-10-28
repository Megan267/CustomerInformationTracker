using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomerTrackerAPI.Migrations
{
    /// <inheritdoc />
    public partial class SeedCustomers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "CustomerId", "ContactPersonEmail", "ContactPersonName", "CreatedAt", "IsActive", "Name", "TelephoneNumber", "VatNumber" },
                values: new object[,]
                {
                    { 1, "thandi@acacialogistics.co.za", "Thandi Mokoena", new DateTime(2026, 10, 28, 9, 0, 0, 0, DateTimeKind.Unspecified), true, "Acacia Logistics", "011 555 0101", "4012345671" },
                    { 2, "pieter@bluecranefoods.co.za", "Pieter van der Merwe", new DateTime(2026, 10, 28, 9, 0, 0, 0, DateTimeKind.Unspecified), true, "Blue Crane Foods", "021 555 0202", "4098765432" },
                    { 3, "ayesha@umhlangamarine.co.za", "Ayesha Naidoo", new DateTime(2026, 10, 28, 9, 0, 0, 0, DateTimeKind.Unspecified), true, "Umhlanga Marine Supplies", "031 555 0303", "4111222333" },
                    { 4, "lerato@karoosolar.co.za", "Lerato Dlamini", new DateTime(2026, 10, 28, 9, 0, 0, 0, DateTimeKind.Unspecified), true, "Karoo Solar Installations", "051 555 0404", null },
                    { 5, "sipho@baobabprint.co.za", "Sipho Khumalo", new DateTime(2026, 10, 28, 9, 0, 0, 0, DateTimeKind.Unspecified), true, "Baobab Print Studio", null, null },
                    { 6, "james@thamesvalleyimports.co.uk", "James Whitfield", new DateTime(2026, 10, 28, 9, 0, 0, 0, DateTimeKind.Unspecified), true, "Thames Valley Imports", "+44 20 7946 0958", "GB123456789" }
                });

            migrationBuilder.InsertData(
                table: "Addresses",
                columns: new[] { "AddressId", "City", "Country", "CustomerId", "PostalCode", "Province", "Street" },
                values: new object[,]
                {
                    { 1, "Sandton", "South Africa", 1, "2196", "Gauteng", "14 Wierda Road" },
                    { 2, "Cape Town", "South Africa", 2, "8001", "Western Cape", "8 Bree Street" },
                    { 3, "Umhlanga", "South Africa", 3, "4319", "KwaZulu-Natal", "22 Lighthouse Road" },
                    { 4, "Bloemfontein", "South Africa", 4, "9301", "Free State", "3 Nelson Mandela Drive" },
                    { 5, "Pretoria", "South Africa", 5, "0002", "Gauteng", "55 Church Street" },
                    { 6, "London", "United Kingdom", 6, "EC2V 8EA", null, "10 King Street" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Addresses",
                keyColumn: "AddressId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Addresses",
                keyColumn: "AddressId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Addresses",
                keyColumn: "AddressId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Addresses",
                keyColumn: "AddressId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Addresses",
                keyColumn: "AddressId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Addresses",
                keyColumn: "AddressId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 6);
        }
    }
}
