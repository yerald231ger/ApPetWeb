using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AppWebApp.Data.Migrations
{
    public partial class changeTableNameall : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoleClaim_Role_RoleId",
                table: "RoleClaim");

            migrationBuilder.DropForeignKey(
                name: "FK_UserClaim_tblUser_UserId",
                table: "UserClaim");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLogin_tblUser_UserId",
                table: "UserLogin");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRole_Role_RoleId",
                table: "UserRole");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRole_tblUser_UserId",
                table: "UserRole");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserToken",
                table: "UserToken");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserRole",
                table: "UserRole");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserLogin",
                table: "UserLogin");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserClaim",
                table: "UserClaim");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RoleClaim",
                table: "RoleClaim");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Role",
                table: "Role");

            migrationBuilder.RenameTable(
                name: "UserToken",
                newName: "tblUserToken");

            migrationBuilder.RenameTable(
                name: "UserRole",
                newName: "tblUserRole");

            migrationBuilder.RenameTable(
                name: "UserLogin",
                newName: "tblUserLogin");

            migrationBuilder.RenameTable(
                name: "UserClaim",
                newName: "tblUserClaim");

            migrationBuilder.RenameTable(
                name: "RoleClaim",
                newName: "tblRoleClaim");

            migrationBuilder.RenameTable(
                name: "Role",
                newName: "tblRole");

            migrationBuilder.RenameIndex(
                name: "IX_UserRole_RoleId",
                table: "tblUserRole",
                newName: "IX_tblUserRole_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_UserLogin_UserId",
                table: "tblUserLogin",
                newName: "IX_tblUserLogin_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserClaim_UserId",
                table: "tblUserClaim",
                newName: "IX_tblUserClaim_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_RoleClaim_RoleId",
                table: "tblRoleClaim",
                newName: "IX_tblRoleClaim_RoleId");

            migrationBuilder.RenameColumn(
                name: "AspNetUserId",
                table: "tblUser",
                newName: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tblUserToken",
                table: "tblUserToken",
                columns: new[] { "UserId", "LoginProvider", "Name" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_tblUserRole",
                table: "tblUserRole",
                columns: new[] { "UserId", "RoleId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_tblUserLogin",
                table: "tblUserLogin",
                columns: new[] { "LoginProvider", "ProviderKey" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_tblUserClaim",
                table: "tblUserClaim",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tblRoleClaim",
                table: "tblRoleClaim",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tblRole",
                table: "tblRole",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_tblRoleClaim_tblRole_RoleId",
                table: "tblRoleClaim",
                column: "RoleId",
                principalTable: "tblRole",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tblUserClaim_tblUser_UserId",
                table: "tblUserClaim",
                column: "UserId",
                principalTable: "tblUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tblUserLogin_tblUser_UserId",
                table: "tblUserLogin",
                column: "UserId",
                principalTable: "tblUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tblUserRole_tblRole_RoleId",
                table: "tblUserRole",
                column: "RoleId",
                principalTable: "tblRole",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tblUserRole_tblUser_UserId",
                table: "tblUserRole",
                column: "UserId",
                principalTable: "tblUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tblRoleClaim_tblRole_RoleId",
                table: "tblRoleClaim");

            migrationBuilder.DropForeignKey(
                name: "FK_tblUserClaim_tblUser_UserId",
                table: "tblUserClaim");

            migrationBuilder.DropForeignKey(
                name: "FK_tblUserLogin_tblUser_UserId",
                table: "tblUserLogin");

            migrationBuilder.DropForeignKey(
                name: "FK_tblUserRole_tblRole_RoleId",
                table: "tblUserRole");

            migrationBuilder.DropForeignKey(
                name: "FK_tblUserRole_tblUser_UserId",
                table: "tblUserRole");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tblUserToken",
                table: "tblUserToken");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tblUserRole",
                table: "tblUserRole");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tblUserLogin",
                table: "tblUserLogin");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tblUserClaim",
                table: "tblUserClaim");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tblRoleClaim",
                table: "tblRoleClaim");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tblRole",
                table: "tblRole");

            migrationBuilder.RenameTable(
                name: "tblUserToken",
                newName: "UserToken");

            migrationBuilder.RenameTable(
                name: "tblUserRole",
                newName: "UserRole");

            migrationBuilder.RenameTable(
                name: "tblUserLogin",
                newName: "UserLogin");

            migrationBuilder.RenameTable(
                name: "tblUserClaim",
                newName: "UserClaim");

            migrationBuilder.RenameTable(
                name: "tblRoleClaim",
                newName: "RoleClaim");

            migrationBuilder.RenameTable(
                name: "tblRole",
                newName: "Role");

            migrationBuilder.RenameIndex(
                name: "IX_tblUserRole_RoleId",
                table: "UserRole",
                newName: "IX_UserRole_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_tblUserLogin_UserId",
                table: "UserLogin",
                newName: "IX_UserLogin_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_tblUserClaim_UserId",
                table: "UserClaim",
                newName: "IX_UserClaim_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_tblRoleClaim_RoleId",
                table: "RoleClaim",
                newName: "IX_RoleClaim_RoleId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "tblUser",
                newName: "AspNetUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserToken",
                table: "UserToken",
                columns: new[] { "UserId", "LoginProvider", "Name" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserRole",
                table: "UserRole",
                columns: new[] { "UserId", "RoleId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserLogin",
                table: "UserLogin",
                columns: new[] { "LoginProvider", "ProviderKey" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserClaim",
                table: "UserClaim",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoleClaim",
                table: "RoleClaim",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Role",
                table: "Role",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RoleClaim_Role_RoleId",
                table: "RoleClaim",
                column: "RoleId",
                principalTable: "Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserClaim_tblUser_UserId",
                table: "UserClaim",
                column: "UserId",
                principalTable: "tblUser",
                principalColumn: "AspNetUserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserLogin_tblUser_UserId",
                table: "UserLogin",
                column: "UserId",
                principalTable: "tblUser",
                principalColumn: "AspNetUserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRole_Role_RoleId",
                table: "UserRole",
                column: "RoleId",
                principalTable: "Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRole_tblUser_UserId",
                table: "UserRole",
                column: "UserId",
                principalTable: "tblUser",
                principalColumn: "AspNetUserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
