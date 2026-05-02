using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PredictiveMaintenance.API.Migrations
{
    /// <inheritdoc />
    public partial class AddAcknowlegedAtToAlert : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Alerts",
                columns: table => new
                {
                    AlertId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MachineId = table.Column<int>(type: "int", nullable: false),
                    RiskLevel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsAcknowledged = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AcknowledgedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alerts", x => x.AlertId);
                });

            migrationBuilder.CreateTable(
                name: "Machines",
                columns: table => new
                {
                    MachineId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MachineName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MachineType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Machines", x => x.MachineId);
                });

            migrationBuilder.CreateTable(
                name: "Predictions",
                columns: table => new
                {
                    PredictionId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MachineId = table.Column<int>(type: "int", nullable: false),
                    RiskLevel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AnomalyScore = table.Column<double>(type: "float", nullable: true),
                    HealthIndex = table.Column<int>(type: "int", nullable: true),
                    PredictedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Predictions", x => x.PredictionId);
                });

            migrationBuilder.CreateTable(
                name: "SensorReadings",
                columns: table => new
                {
                    ReadingId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MachineId = table.Column<int>(type: "int", nullable: false),
                    Cycle = table.Column<int>(type: "int", nullable: false),
                    Temperature = table.Column<double>(type: "float", nullable: true),
                    Pressure = table.Column<double>(type: "float", nullable: true),
                    Vibration = table.Column<double>(type: "float", nullable: true),
                    Speed = table.Column<double>(type: "float", nullable: true),
                    RecordedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SensorReadings", x => x.ReadingId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Alerts");

            migrationBuilder.DropTable(
                name: "Machines");

            migrationBuilder.DropTable(
                name: "Predictions");

            migrationBuilder.DropTable(
                name: "SensorReadings");
        }
    }
}
