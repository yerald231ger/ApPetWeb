using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace AppWebApp.Data.Migrations
{
    public partial class initialModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "tblUser",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "tblUser",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModDate",
                table: "tblUser",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpDate",
                table: "tblUser",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "tblPetTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsActive = table.Column<bool>(nullable: false),
                    ModDate = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    UpDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblPetTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tblVeterinaries",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Address = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    ImageProfileId = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    Latitud = table.Column<float>(nullable: false),
                    Longitud = table.Column<float>(nullable: false),
                    ModDate = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    UpDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblVeterinaries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tblVetServices",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    ModDate = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Price = table.Column<float>(nullable: false),
                    ShowPrice = table.Column<bool>(nullable: false),
                    UpDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblVetServices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tblPets",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Birthdate = table.Column<DateTime>(nullable: false),
                    ImageProfileId = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    ModDate = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    PetTypeId = table.Column<int>(nullable: false),
                    Race = table.Column<string>(nullable: true),
                    UpDate = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    Wight = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblPets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblPets_tblPetTypes_PetTypeId",
                        column: x => x.PetTypeId,
                        principalTable: "tblPetTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tblPets_tblUser_UserId",
                        column: x => x.UserId,
                        principalTable: "tblUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VeterinaryVetService",
                columns: table => new
                {
                    VeterinaryId = table.Column<int>(nullable: false),
                    VetServiceId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VeterinaryVetService", x => new { x.VeterinaryId, x.VetServiceId });
                    table.ForeignKey(
                        name: "FK_VeterinaryVetService_tblVetServices_VetServiceId",
                        column: x => x.VetServiceId,
                        principalTable: "tblVetServices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VeterinaryVetService_tblVeterinaries_VeterinaryId",
                        column: x => x.VeterinaryId,
                        principalTable: "tblVeterinaries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tblPets_PetTypeId",
                table: "tblPets",
                column: "PetTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_tblPets_UserId",
                table: "tblPets",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_VeterinaryVetService_VetServiceId",
                table: "VeterinaryVetService",
                column: "VetServiceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tblPets");

            migrationBuilder.DropTable(
                name: "VeterinaryVetService");

            migrationBuilder.DropTable(
                name: "tblPetTypes");

            migrationBuilder.DropTable(
                name: "tblVetServices");

            migrationBuilder.DropTable(
                name: "tblVeterinaries");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "tblUser");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "tblUser");

            migrationBuilder.DropColumn(
                name: "ModDate",
                table: "tblUser");

            migrationBuilder.DropColumn(
                name: "UpDate",
                table: "tblUser");
        }
    }
}
