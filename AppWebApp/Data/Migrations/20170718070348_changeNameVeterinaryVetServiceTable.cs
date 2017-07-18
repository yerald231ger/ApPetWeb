using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AppWebApp.Data.Migrations
{
    public partial class changeNameVeterinaryVetServiceTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VeterinaryVetService_tblVetServices_VetServiceId",
                table: "VeterinaryVetService");

            migrationBuilder.DropForeignKey(
                name: "FK_VeterinaryVetService_tblVeterinaries_VeterinaryId",
                table: "VeterinaryVetService");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VeterinaryVetService",
                table: "VeterinaryVetService");

            migrationBuilder.RenameTable(
                name: "VeterinaryVetService",
                newName: "tblVeterinaryVetServices");

            migrationBuilder.RenameIndex(
                name: "IX_VeterinaryVetService_VetServiceId",
                table: "tblVeterinaryVetServices",
                newName: "IX_tblVeterinaryVetServices_VetServiceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tblVeterinaryVetServices",
                table: "tblVeterinaryVetServices",
                columns: new[] { "VeterinaryId", "VetServiceId" });

            migrationBuilder.AddForeignKey(
                name: "FK_tblVeterinaryVetServices_tblVetServices_VetServiceId",
                table: "tblVeterinaryVetServices",
                column: "VetServiceId",
                principalTable: "tblVetServices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tblVeterinaryVetServices_tblVeterinaries_VeterinaryId",
                table: "tblVeterinaryVetServices",
                column: "VeterinaryId",
                principalTable: "tblVeterinaries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tblVeterinaryVetServices_tblVetServices_VetServiceId",
                table: "tblVeterinaryVetServices");

            migrationBuilder.DropForeignKey(
                name: "FK_tblVeterinaryVetServices_tblVeterinaries_VeterinaryId",
                table: "tblVeterinaryVetServices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tblVeterinaryVetServices",
                table: "tblVeterinaryVetServices");

            migrationBuilder.RenameTable(
                name: "tblVeterinaryVetServices",
                newName: "VeterinaryVetService");

            migrationBuilder.RenameIndex(
                name: "IX_tblVeterinaryVetServices_VetServiceId",
                table: "VeterinaryVetService",
                newName: "IX_VeterinaryVetService_VetServiceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VeterinaryVetService",
                table: "VeterinaryVetService",
                columns: new[] { "VeterinaryId", "VetServiceId" });

            migrationBuilder.AddForeignKey(
                name: "FK_VeterinaryVetService_tblVetServices_VetServiceId",
                table: "VeterinaryVetService",
                column: "VetServiceId",
                principalTable: "tblVetServices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VeterinaryVetService_tblVeterinaries_VeterinaryId",
                table: "VeterinaryVetService",
                column: "VeterinaryId",
                principalTable: "tblVeterinaries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
