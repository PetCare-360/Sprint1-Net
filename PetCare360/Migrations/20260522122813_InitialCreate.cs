using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetCare360.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Name = table.Column<string>(type: "NVARCHAR2(120)", maxLength: 120, nullable: false),
                    Email = table.Column<string>(type: "NVARCHAR2(160)", maxLength: 160, nullable: false),
                    PASSWORD_HASH = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    CREATED_AT = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "pets",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    USER_ID = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(120)", maxLength: 120, nullable: false),
                    Age = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Weight = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Breed = table.Column<string>(type: "NVARCHAR2(120)", maxLength: 120, nullable: false),
                    DEVICE_ID = table.Column<string>(type: "NVARCHAR2(80)", maxLength: 80, nullable: false),
                    CREATED_AT = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_pets_users_USER_ID",
                        column: x => x.USER_ID,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "alerts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    PET_ID = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    Type = table.Column<string>(type: "NVARCHAR2(40)", maxLength: 40, nullable: false),
                    Message = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false),
                    LEVEL_ALERT = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    CREATED_AT = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_alerts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_alerts_pets_PET_ID",
                        column: x => x.PET_ID,
                        principalTable: "pets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "devices",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    DEVICE_ID = table.Column<string>(type: "NVARCHAR2(80)", maxLength: 80, nullable: false),
                    PET_ID = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    Status = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    Battery = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    LAST_SEEN = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_devices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_devices_pets_PET_ID",
                        column: x => x.PET_ID,
                        principalTable: "pets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sensor_data",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    DEVICE_ID_FK = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    SENSOR_TIMESTAMP = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: false),
                    Temperature = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    HEART_RATE = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ACTIVITY_LEVEL = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Latitude = table.Column<decimal>(type: "decimal(10,6)", nullable: true),
                    Longitude = table.Column<decimal>(type: "decimal(10,6)", nullable: true),
                    Battery = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    Status = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sensor_data", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sensor_data_devices_DEVICE_ID_FK",
                        column: x => x.DEVICE_ID_FK,
                        principalTable: "devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_alerts_PET_ID",
                table: "alerts",
                column: "PET_ID");

            migrationBuilder.CreateIndex(
                name: "IX_devices_DEVICE_ID",
                table: "devices",
                column: "DEVICE_ID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_devices_PET_ID",
                table: "devices",
                column: "PET_ID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_pets_DEVICE_ID",
                table: "pets",
                column: "DEVICE_ID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_pets_USER_ID",
                table: "pets",
                column: "USER_ID");

            migrationBuilder.CreateIndex(
                name: "IX_sensor_data_DEVICE_ID_FK",
                table: "sensor_data",
                column: "DEVICE_ID_FK");

            migrationBuilder.CreateIndex(
                name: "IX_users_Email",
                table: "users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "alerts");

            migrationBuilder.DropTable(
                name: "sensor_data");

            migrationBuilder.DropTable(
                name: "devices");

            migrationBuilder.DropTable(
                name: "pets");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
