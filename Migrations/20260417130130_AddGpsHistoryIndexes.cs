using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DACS.Migrations
{
    /// <inheritdoc />
    public partial class AddGpsHistoryIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GPSHistories_DeviceId",
                table: "GPSHistories");

            migrationBuilder.CreateIndex(
                name: "IX_GPSHistories_DeviceId_Timestamp",
                table: "GPSHistories",
                columns: new[] { "DeviceId", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_GPSHistories_Timestamp",
                table: "GPSHistories",
                column: "Timestamp");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GPSHistories_DeviceId_Timestamp",
                table: "GPSHistories");

            migrationBuilder.DropIndex(
                name: "IX_GPSHistories_Timestamp",
                table: "GPSHistories");

            migrationBuilder.CreateIndex(
                name: "IX_GPSHistories_DeviceId",
                table: "GPSHistories",
                column: "DeviceId");
        }
    }
}
